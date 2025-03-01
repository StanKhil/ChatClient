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

        private void cmbContacts_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedContact = cmbContacts.Text;
            MessageBox.Show(selectedContact);
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
            if (client == null || !client.isConnected)
            {
                MessageBox.Show("Соединение с сервером потеряно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string recipient = cmbContacts.Text;
            string message = txtMessage.Text;

            if (!string.IsNullOrEmpty(recipient) && !string.IsNullOrEmpty(message))
            {
                try
                {
                    await client.SendMessageAsync(recipient, message);

                    string formattedMessage = $"Вы: {message}";
                    if (!client.messagesInChat.ContainsKey(recipient))
                        client.messagesInChat[recipient] = "";

                    client.messagesInChat[recipient] += formattedMessage + "\n";

                    lstChat.Items.Add(formattedMessage);
                    txtMessage.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при отправке сообщения: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void StartReceivingMessages()
        {
            isReceiving = true;

            while (isReceiving && client != null && client.isConnected)
            {
                try
                {
                    string receivedMessage = await client.ReceiveMessageAsync();

                    if (!string.IsNullOrEmpty(receivedMessage))
                    {
                        string sender = receivedMessage.Split(':')[0];

                        if (cmbContacts.Text == sender)
                        {
                            Invoke((MethodInvoker)(() => lstChat.Items.Add(receivedMessage)));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при получении сообщений: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    isReceiving = false;
                    break;
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
