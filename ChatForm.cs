using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class ChatForm : Form
    {
        private Client client;
        private string userLogin;
        private bool isReceiving = true;

        public ChatForm(string login)
        {
            userLogin = login;
            InitializeComponent();
            ConnectToServer();
        }

        private async void ConnectToServer()
        {
            try
            {
                client = new Client("127.0.0.1", 5000);
                client.ContactsUpdated += UpdateContactsList;
                string response = await client.ConnectAsync( userLogin);

                if (!string.IsNullOrEmpty(response))
                {
                    StartReceivingMessages();
                }
                else
                {
                    MessageBox.Show("Не удалось подключиться к серверу!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подключении к серверу: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void UpdateContactsList(List<string> users)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateContactsList(users)));
                return;
            }

            cmbContacts.Items.Clear();
            cmbContacts.Items.AddRange(users.ToArray());
        }

        private void cmbContacts_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedContact = cmbContacts.Text;
            if (!string.IsNullOrEmpty(selectedContact) && client.messagesInChat.ContainsKey(selectedContact))
            {
                lstChat.Items.Clear();
                string[] messages = client.messagesInChat[selectedContact].Split('\n');
                foreach (string message in messages)
                {
                    if (!string.IsNullOrEmpty(message))
                    {
                        lstChat.Items.Add(message);
                    }
                }
            }
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            string recipient = cmbContacts.Text.Trim();
            string message = txtMessage.Text;

            
            if (!string.IsNullOrEmpty(recipient) && !string.IsNullOrEmpty(message))
            {
                await client.SendMessageAsync(recipient, message);
                lstChat.Items.Add($"Вы: {message}");
                txtMessage.Clear();
            }
        }

        private async void btnSendFile_Click(object sender, EventArgs e)
        {
            if (client == null || !client.isConnected)
            {
                MessageBox.Show("Соединение с сервером потеряно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string recipient = cmbContacts.SelectedItem?.ToString().Trim();
                    string filePath = openFileDialog.FileName;

                    if (!string.IsNullOrEmpty(recipient))
                    {
                        await client.SendFileAsync(recipient, filePath);
                        lstChat.Items.Add($"Вы отправили файл: {Path.GetFileName(filePath)}");
                        if (!client.messagesInChat.ContainsKey(recipient))
                        {
                            client.messagesInChat[recipient] = ""; 
                        }

                        client.messagesInChat[recipient] += "Вы отправили файл: " + Path.GetFileName(filePath) + "\n";
                        MessageBox.Show("Файл успешно отправлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Выберите получателя перед отправкой файла!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }


        private async void StartReceivingMessages()
        {
            isReceiving = true;

            while (isReceiving && client != null && client.isConnected)
            {
                string receivedMessage = await client.ReceiveMessageAsync();
                if (!string.IsNullOrEmpty(receivedMessage))
                {
                    if (receivedMessage.StartsWith("FILE:"))
                    {
                        string[] parts = receivedMessage.Substring(5).Split('|');
                        if (parts.Length == 2)
                        {
                            string sender = parts[0];
                            string fileName = parts[1];

                            await client.ReceiveFileAsync(sender, fileName);
                            Invoke((MethodInvoker)(() => lstChat.Items.Add($"{sender}: отправил файл: {fileName}")));
                        }
                    }
                    else
                    {
                        if(!receivedMessage.StartsWith("USERS") && !receivedMessage.StartsWith("ADD") && !receivedMessage.StartsWith("REMOVE") && !receivedMessage.StartsWith("OK"))
                        {
                            Invoke((MethodInvoker)(() => lstChat.Items.Add(receivedMessage)));
                        }
                        
                    }
                }
            }
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            isReceiving = false;
            client?.Close();
            base.OnFormClosing(e);
        }
    }
}
