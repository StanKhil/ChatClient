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
                    lstChat.Items.Add($"Вы: {message}");
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
                        Invoke((MethodInvoker)(() => lstChat.Items.Add(receivedMessage)));
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
