using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                byte[] data = Encoding.UTF8.GetBytes(fullMessage);
                await stream.WriteAsync(data, 0, data.Length);
            }
            catch
            {
                MessageBox.Show("Ошибка при отправке сообщения!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        public async Task<string> ReceiveMessageAsync()
        {
            if (!isConnected || tcpClient == null || !tcpClient.Connected || stream == null)
            {
                Reconnect();
                return "";
            }

            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                {
                    Close();
                    Reconnect();
                    return "";
                }

                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
            catch
            {
                Close();
                Reconnect();
                return "";
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

        private async void Reconnect()
        {
            Close();
            MessageBox.Show("Переподключение к серверу...", "Соединение потеряно", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await Task.Delay(3000);
            await ConnectAsync(login);
        }
    }
}
