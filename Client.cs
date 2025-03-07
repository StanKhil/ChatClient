using System.Net.Sockets;
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
        public Dictionary<string,List<string>> usersInGroups = new Dictionary<string, List<string>>();
        public List<string> groups = new List<string>();
        public event Action<List<string>> ContactsUpdated;
        public event Action<List<string>,string> GroupUpdated;
        public event Action<List<string>> GroupAdded;

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
                    MessageBox.Show("Цей логін вже використовується!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return "";
                }

                isConnected = true;
                return response;
            }
            catch
            {
                MessageBox.Show("Помилка при підключені до сервера!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return "";
            }
        }

        public async Task SendMessageAsync(string recipient, string message, string group = "")
        {
            if (!isConnected || tcpClient == null || !tcpClient.Connected || stream == null)
            {
                MessageBox.Show("З'днання втрачено!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string fullMessage;
                if (string.IsNullOrEmpty(group))
                {
                    fullMessage = $"MESSAGE:{recipient}:{message}\n";
                    if (!messagesInChat.ContainsKey(recipient))
                    {
                        messagesInChat[recipient] = $"Ви: {message}\n";
                    }
                    else
                    {
                        messagesInChat[recipient] += $"Ви: {message}\n";
                    }
                }
                else
                {
                    fullMessage = $"MESSAGE:GROUP:{group}:{recipient}:{message}\n";
                    if (!messagesInChat.ContainsKey(group))
                    {
                        messagesInChat[group] = $"Ви: {message}\n";
                    }
                    else
                    {
                        messagesInChat[group] += $"Ви: {message}\n";
                    }
                }

                byte[] data = Encoding.UTF8.GetBytes(fullMessage);
                await stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при відправці повідомлення! " + ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                HandleError("Помилка при отриманні повідомлення", ex);
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
            else if(receivedMessage.StartsWith("ADDGROUP:"))
            {
                List<string> newGroups = receivedMessage.Substring(9)
                    .Split(',')
                    .Where(g => !string.IsNullOrWhiteSpace(g))
                    .ToList();
                for(int i=0;i< newGroups.Count; i++)
                {
                    if (!groups.Contains(newGroups[i]))
                    {
                        groups.Add(newGroups[i]);
                    }
                }
                GroupAdded?.Invoke(groups);
            }
            else if (receivedMessage.StartsWith("UPDATEGROUP:"))
            {
                string[] parts = receivedMessage.Substring(11).Split(':');
                string groupName = parts[1];
                List<string> users = parts[2].Split(',').ToList();
                usersInGroups[groupName] = users;
                GroupUpdated?.Invoke(users,groupName);
            }
            else if (receivedMessage.StartsWith("GROUP:"))
            {
                string[] partsGroup = receivedMessage.Split(':', 4);
                if (partsGroup.Length == 4)
                {
                    string groupName = partsGroup[1];
                    string sender = partsGroup[2];
                    string message = partsGroup[3];

                    if (!messagesInChat.ContainsKey(groupName))
                    {
                        messagesInChat[groupName] = "";
                    }

                    messagesInChat[groupName] += $"{sender}: {message}\n";
                }
                
            }
            else
            {
                string[] parts = receivedMessage.Split(':', 2);
                if (parts.Length == 2)
                {
                    string sender = parts[0];
                    string message = parts[1];
                    if(!messagesInChat.ContainsKey(sender))
                    {
                        messagesInChat[sender] = "";
                    }
                    messagesInChat[sender] += $"{sender}: {message}\n";
                }
            }
        }

        public async Task SendFileAsync(string recipient, string filePath, string group = "")
        {
            if (!isConnected) return;

            try
            {
                if (!IsFileAvailable(filePath))
                {
                    MessageBox.Show("Файл недоступний.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    string fileName = Path.GetFileName(filePath);
                    long fileSize = fileStream.Length;

                    string header = group == ""
                        ? $"FILE:{recipient}:{fileName}:{fileSize}\n"
                        : $"FILE:GROUP:{group}:{fileName}:{fileSize}\n";

                    byte[] headerBytes = Encoding.UTF8.GetBytes(header);
                    await stream.WriteAsync(headerBytes, 0, headerBytes.Length);
                    await stream.FlushAsync();

                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await stream.WriteAsync(buffer, 0, bytesRead);
                        await stream.FlushAsync();
                    }

                    MessageBox.Show($"Файл {fileName} надіслано!");
                    string chatKey = group == "" ? recipient : group;
                    if (!messagesInChat.ContainsKey(chatKey))
                    {
                        messagesInChat[chatKey] = "";
                    }
                    messagesInChat[chatKey] += $"Ви надіслали файл: {fileName}\n";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка відправки файла: {ex.Message}");
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

                    string message = $"Ви приняли файл: {fileName}";

                    if (!messagesInChat.ContainsKey(sender))
                        messagesInChat[sender] = "";

                    messagesInChat[sender] += message + "\n";

                    MessageBox.Show($"Файл {fileName} загружен в {savePath}", "Файл получен", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка получения файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task AddGroup(string groupName)
        {
            string message = $"ADDGROUP:{groupName}\n";
            byte[] data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length);
        }

        public async Task UpdateGroup(string groupName)
        {
            string message = $"UPDATEGROUP:{groupName}:{string.Join(",", usersInGroups[groupName])}\n";
            byte[] data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length);
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
            MessageBox.Show("Перепідключення до сервера...", "Втрата з'єднання", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            MessageBox.Show($"{message}: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
        }

        private bool CheckConnection()
        {
            if (isConnected && tcpClient?.Connected == true && stream != null) return true;

            MessageBox.Show("З'єднання втрачено", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }
}

