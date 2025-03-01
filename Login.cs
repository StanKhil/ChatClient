using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class Login : Form
    {
        public string UserLogin { get; private set; }
        private Client client;

        public Login()
        {
            InitializeComponent();
            client = new Client("127.0.0.1", 5000);
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            UserLogin = txtLogin.Text.Trim();

            if (string.IsNullOrEmpty(UserLogin))
            {
                MessageBox.Show("Введите логин!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnLogin.Enabled = false;

            try
            {
                string loginSuccess = await client.ConnectAsync(UserLogin);

                if (loginSuccess == "OK")
                {
                    MessageBox.Show("Вы успешно вошли!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    DialogResult = DialogResult.OK;
                    client.Close();
                    client.isConnected = false;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Этот логин уже используется!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtLogin.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }
    }
}
