﻿using System.Net.Sockets;
using System.Text;


namespace ChatClient
{
    public class Client
    {
        public TcpClient tcpClient;
        private NetworkStream stream;
        private readonly string serverIp;
        private readonly int port;
        private string login;
        public bool isConnected = false;
        public Dictionary<string, string> messagesInChat = new Dictionary<string, string>();
        public List<string> connectedUsers = new List<string>();
        public event Action<List<string>> ContactsUpdated;

        public Client(string serverIp, int port)
        {
            this.serverIp = serverIp;
            this.port = port;
        }

        public async Task<string> ConnectAsync(string login)
        {
            try
            {
                this.login = login;
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(serverIp, port);
                stream = tcpClient.GetStream();

                byte[] loginData = Encoding.UTF8.GetBytes(login);
                await stream.WriteAsync(loginData, 0, loginData.Length);

                byte[] responseBuffer = new byte[256];
                int bytesRead = await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length);
                string response = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead).Trim();

                if (response == "ERROR")
                {
                    MessageBox.Show("Этот логин уже используется!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return "";
                }

                isConnected = true;
                return response;
            }
            catch
            {
                MessageBox.Show("Ошибка подключения к серверу!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return "";
            }
        }

        public async Task SendMessageAsync(string recipient, string message)
        {
            if (!isConnected || tcpClient == null || !tcpClient.Connected || stream == null)
            {
                MessageBox.Show("Соединение с сервером потеряно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string fullMessage = $"{recipient}:{message}";
                if (!messagesInChat.ContainsKey(recipient))
                {
                    messagesInChat[recipient] = $"Вы: {message}\n";
                }
                else   messagesInChat[recipient] += $"Вы: {message}\n";
                byte[] data = Encoding.UTF8.GetBytes(fullMessage);
                await stream.WriteAsync(data, 0, data.Length);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка при отправке сообщения! " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        public async Task<string> ReceiveMessageAsync()
        {
            if (!CheckConnection()) return "";

            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    Close();
                    await Reconnect();
                    return "";
                }

                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                ProcessReceivedMessage(receivedMessage);
                return receivedMessage;
            }
            catch (Exception ex)
            {
                HandleError("Ошибка при получении сообщения", ex);
                return "";
            }
        }

        private void ProcessReceivedMessage(string receivedMessage)
        {
            if (string.IsNullOrEmpty(receivedMessage)) return;

            if (receivedMessage.StartsWith("ADD:"))
            {
                string newUser = receivedMessage.Substring(4).Trim();
                if (!string.IsNullOrWhiteSpace(newUser) && !connectedUsers.Contains(newUser))
                {
                    connectedUsers.Add(newUser);
                    ContactsUpdated?.Invoke(connectedUsers);
                    MessageBox.Show($"Пользователь {newUser} присоединился к чату!", "Уведомление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            else if (receivedMessage.StartsWith("REMOVE:"))
            {
                string removedUser = receivedMessage.Substring(7);
                connectedUsers.Remove(removedUser);
                ContactsUpdated?.Invoke(connectedUsers);
            }
            else if (receivedMessage.StartsWith("USERS:"))
            {
                connectedUsers = receivedMessage.Substring(6)
                    .Split(',')
                    .Where(u => !string.IsNullOrWhiteSpace(u))
                    .ToList();
                ContactsUpdated?.Invoke(connectedUsers);
            }
            else
            {
                string[] parts = receivedMessage.Split(':', 2);
                if (parts.Length == 2)
                {
                    string sender = parts[0];
                    string message = parts[1];

                    //messagesInChat[sender] = messagesInChat.GetValueOrDefault(sender, "") + $"{sender}: {message}\n";
                    if(!messagesInChat.ContainsKey(sender))
                    {
                        messagesInChat[sender] = "";
                    }
                    messagesInChat[sender] += $"{sender}: {message}\n";
                }
            }
        }

        public async Task SendFileAsync(string recipient, string filePath)
        {
            if (!isConnected) return;

            try
            {
                if (!IsFileAvailable(filePath))
                {
                    MessageBox.Show("Файл используется другим процессом или заблокирован.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    string fileName = Path.GetFileName(filePath);
                    long fileSize = fileStream.Length;

                    string header = $"{recipient}:FILE: {fileName} {fileSize}\n";
                    byte[] headerBytes = Encoding.UTF8.GetBytes(header);

                    await stream.WriteAsync(headerBytes, 0, headerBytes.Length);
                    MessageBox.Show($"Отправлен заголовок: {header}");

                    /*byte[] confirmBuffer = new byte[1024];
                    int confirmBytes = await stream.ReadAsync(confirmBuffer, 0, confirmBuffer.Length);
                    string confirmMessage = Encoding.UTF8.GetString(confirmBuffer, 0, confirmBytes).Trim();

                    if (!confirmMessage.StartsWith("OK"))
                    {
                        MessageBox.Show($"Ошибка: сервер не принял заголовок! Ответ: {confirmMessage}");
                        return;
                    }*/

                    MessageBox.Show($"Сервер принял заголовок, начинаем отправку файла {fileName}");

                    byte[] buffer = new byte[1024];
                    int bytesRead;

                    while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await stream.WriteAsync(buffer, 0, bytesRead);
                    }
                    MessageBox.Show($"Файл {fileName} отправлен успешно!");
                    if(!messagesInChat.ContainsKey(recipient))
                    {
                        messagesInChat[recipient] = "";
                    }
                    messagesInChat[recipient] += $"Вы отправили файл: {fileName}\n";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отправки файла: {ex.Message}");
            }
        }

        public async Task ReceiveFileAsync(string sender, string fileName)
        {
            try
            {
                MessageBox.Show("Start receiving file", "File received", MessageBoxButtons.OK, MessageBoxIcon.Information);
                string safeFileName = fileName.Trim();
                foreach (char c in Path.GetInvalidFileNameChars())
                {
                    safeFileName = safeFileName.Replace(c, '_');
                }

                string saveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Downloads");
                Directory.CreateDirectory(saveDirectory);

                string savePath = Path.Combine(saveDirectory, safeFileName);

                Directory.CreateDirectory("Downloads");

                using (FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead;

                    MessageBox.Show("Receiving file", "File received", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        if (bytesRead < buffer.Length) break;
                    }

                    string message = $"Вы приняли файл: {fileName}";

                    if (!messagesInChat.ContainsKey(sender))
                        messagesInChat[sender] = "";

                    messagesInChat[sender] += message + "\n";

                    MessageBox.Show($"Файл {fileName} загружен в {savePath}", "Файл получен", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка получения файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Close()
        {
            isConnected = false;
            stream?.Close();
            tcpClient?.Close();
            stream = null;
            tcpClient = null;
        }

        private async Task Reconnect()
        {
            Close();
            MessageBox.Show("Переподключение к серверу...", "Соединение потеряно", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await Task.Delay(3000);
            await ConnectAsync(login);
        }

        private bool IsFileAvailable(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    return true;
                }
            }
            catch (IOException)
            {
                return false;
            }
        }

        private void HandleError(string message, Exception ex)
        {
            MessageBox.Show($"{message}: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
        }

        private bool CheckConnection()
        {
            if (isConnected && tcpClient?.Connected == true && stream != null) return true;

            MessageBox.Show("Соединение с сервером потеряно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }
}

