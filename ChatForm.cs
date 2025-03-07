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
                client.GroupUpdated += UpdateGroupUsersList;
                client.GroupAdded += UpdateGroupsList;
                string response = await client.ConnectAsync(userLogin);

                if (!string.IsNullOrEmpty(response))
                {
                    StartReceivingMessages();
                }
                else
                {
                    MessageBox.Show("Не вдалось під'єднатися до сервера!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при підключенні до сервера: " + ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            cmbAddUser.Items.Clear();
            cmbAddUser.Items.AddRange(users.ToArray());
            cmbRemoveUser.Items.Clear();
            cmbRemoveUser.Items.AddRange(users.ToArray());
        }

        private void UpdateGroupsList(List<string> groups)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateGroupsList(groups)));
                return;
            }
            cmbGroups.Items.Clear();
            cmbGroups.Items.AddRange(groups.ToArray());
        }

        private void UpdateGroupUsersList(List<string> users, string group)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateGroupUsersList(users,group)));
                return;
            }
            string[] usersReceived = users.ToArray();
            if (usersReceived.Contains(userLogin) && !cmbGroups.Items.Contains(group))
            { 
                cmbGroups.Items.Add(group);
                client.usersInGroups[group] = usersReceived.ToList();
            }
            if(group == cmbGroups.Text)
            {
                lstGroupMembers.Items.Clear();
                lstGroupMembers.Items.AddRange(usersReceived);
            }
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


            if (!string.IsNullOrEmpty(recipient) && !string.IsNullOrEmpty(message) && client.connectedUsers.Contains(recipient))
            {
                await client.SendMessageAsync(recipient, message);
                lstChat.Items.Add($"Ви: {message}");
                txtMessage.Clear();
            }
        }

        private async void btnSendFile_Click(object sender, EventArgs e)
        {
            if (client == null || !client.isConnected)
            {
                MessageBox.Show("З'єднання втрачено!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string recipient = cmbContacts.Text.Trim();
                    string filePath = openFileDialog.FileName;

                    if (!string.IsNullOrEmpty(recipient) && client.connectedUsers.Contains(recipient))
                    {
                        await client.SendFileAsync(recipient, filePath);
                        lstChat.Items.Add($"Ви надіслали файл: {Path.GetFileName(filePath)}");
                        if (!client.messagesInChat.ContainsKey(recipient))
                        {
                            client.messagesInChat[recipient] = "";
                        }

                        client.messagesInChat[recipient] += "Ви надіслали файл: " + Path.GetFileName(filePath) + "\n";
                        MessageBox.Show("Файл успішно надіслан!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Оберіть отримувача", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            Invoke((MethodInvoker)(() => lstChat.Items.Add($"{sender}: надіслав файл: {fileName}")));
                        }
                    }
                    else if (receivedMessage.StartsWith("GROUPFILE:"))
                    {
                        string[] parts = receivedMessage.Split(':', 3);
                        if (parts.Length == 3)
                        {
                            string groupName = parts[1];
                            string[] senderAndFile = parts[2].Split('|');

                            if (senderAndFile.Length == 2)
                            {
                                string sender = senderAndFile[0];
                                string fileName = senderAndFile[1];

                                await client.ReceiveFileAsync(sender, fileName);

                                if (!client.messagesInChat.ContainsKey(groupName))
                                {
                                    client.messagesInChat[groupName] = "";
                                }
                                client.messagesInChat[groupName] += $"{sender} надіслав файл: {fileName}\n";

                                Invoke((MethodInvoker)(() => lstGroupChat.Items.Add($"{sender}: надіслав файл {fileName}")));
                            }
                        }
                    }
                    else
                    {
                        if (!receivedMessage.StartsWith("USERS") &&
                            !receivedMessage.StartsWith("ADD") &&
                            !receivedMessage.StartsWith("REMOVE") &&
                            !receivedMessage.StartsWith("OK") &&
                            !receivedMessage.StartsWith("ADDGROUP") &&
                            !receivedMessage.StartsWith("UPDATEGROUP"))
                        {
                            if (receivedMessage.StartsWith("GROUP"))
                            {
                                string message = receivedMessage.Substring(5);
                                string[] parts = message.Split(':', 3);
                                string groupName = parts[0];
                                string sender = parts[1];
                                string text = parts[2];
                                Invoke((MethodInvoker)(() => lstGroupChat.Items.Add(text)));
                            }
                            else
                            {
                                Invoke((MethodInvoker)(() => lstChat.Items.Add(receivedMessage)));

                            }
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

        private async void btnCreateGroup_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(txtGroupName.Text))
            {
                MessageBox.Show("Введіть назву групи", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(client.usersInGroups.ContainsKey(txtGroupName.Text))
            {
                MessageBox.Show("Група з такою назвою вже існує", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            client.groups.Add(txtGroupName.Text);
            client.usersInGroups[txtGroupName.Text] = new List<string>();
            client.usersInGroups[txtGroupName.Text].Add(userLogin);
            lstGroupMembers.Items.Add(userLogin);
            await client.AddGroup(txtGroupName.Text);
            txtGroupName.Clear();
        }

        private async void btnAddUser_Click(object sender, EventArgs e)
        {
            if (!client.usersInGroups.ContainsKey(cmbGroups.Text))
            {
                client.usersInGroups[cmbGroups.Text] = new List<string>();
            }
            if (client.usersInGroups[cmbGroups.Text].Contains(cmbAddUser.Text))
            {
                MessageBox.Show("Користувач вже у групі", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!client.connectedUsers.Contains(cmbAddUser.Text))
            {
                MessageBox.Show("Користувача не існує", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            client.usersInGroups[cmbGroups.Text].Add(cmbAddUser.Text);
            cmbAddUser.Text = "";
            lstGroupMembers.Items.Add(userLogin);
            lstGroupMembers.Items.Remove(userLogin);
            await client.UpdateGroup(cmbGroups.Text);
        }

        private async void btnRemoveUser_Click(object sender, EventArgs e)
        {
            if (!client.usersInGroups.ContainsKey(cmbGroups.Text))
            {
                client.usersInGroups[cmbGroups.Text] = new List<string>();
            }
            if (!client.usersInGroups[cmbGroups.Text].Contains(cmbRemoveUser.Text))
            {
                MessageBox.Show("Користувач не в групі", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            client.usersInGroups[cmbGroups.Text].Remove(cmbRemoveUser.Text);
            cmbRemoveUser.Text = "";
            await client.UpdateGroup(cmbGroups.Text);
        }

        private async void btnSendGroupMessage_Click(object sender, EventArgs e)
        {
            string recipient = cmbGroups.Text.Trim();
            string message = txtGroupMessage.Text;

            if (!string.IsNullOrEmpty(recipient) && !string.IsNullOrEmpty(message) && client.groups.Contains(recipient) && client.usersInGroups[recipient].Contains(userLogin))
            {

                await client.SendMessageAsync("", message, cmbGroups.Text);

                lstGroupChat.Items.Add($"Ви: {message}");
                txtGroupMessage.Clear();
            }
        }

        private async void btnSendGroupFile_Click(object sender, EventArgs e)
        {
            if (client == null || !client.isConnected)
            {
                MessageBox.Show("Помилка з'єднання", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string recipient = cmbGroups.Text.Trim();
                    string filePath = openFileDialog.FileName;

                    if (!string.IsNullOrEmpty(recipient) && client.groups.Contains(recipient) && client.usersInGroups[recipient].Contains(userLogin))
                    {

                        await client.SendFileAsync("", filePath, cmbGroups.Text);

                        await client.SendFileAsync(recipient, filePath);
                        lstGroupChat.Items.Add($"Ви надіслали файл: {Path.GetFileName(filePath)}");
                        if (!client.messagesInChat.ContainsKey(recipient))
                        {
                            client.messagesInChat[recipient] = "";
                        }

                        client.messagesInChat[recipient] += "Ви надіслали файл: " + Path.GetFileName(filePath) + "\n";
                        MessageBox.Show("Файл надіслано!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Оберіть групу", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void cmbGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedGroup = cmbGroups.Text;
            if (!string.IsNullOrEmpty(selectedGroup) && client.messagesInChat.ContainsKey(selectedGroup))
            {
                lstGroupChat.Items.Clear();
                string[] messages = client.messagesInChat[selectedGroup].Split('\n');
                foreach (string message in messages)
                {
                    if (!string.IsNullOrEmpty(message))
                    {
                        lstGroupChat.Items.Add(message);
                    }
                }
                lstGroupMembers.Items.Clear();
                lstGroupMembers.Items.AddRange(client.usersInGroups[selectedGroup].ToArray());
            }
        }
    }
}
