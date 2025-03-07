namespace ChatClient
{
    partial class ChatForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ComboBox cmbContacts;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnSendFile;
        private System.Windows.Forms.ListBox lstChat;
        private System.Windows.Forms.Label lblContacts;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label lblGroupName;
        private System.Windows.Forms.TextBox txtGroupName;
        private System.Windows.Forms.Button btnCreateGroup;
        private System.Windows.Forms.ComboBox cmbGroups;
        private System.Windows.Forms.Label lblAddUser;
        private System.Windows.Forms.ComboBox cmbAddUser;
        private System.Windows.Forms.Button btnAddUser;
        private System.Windows.Forms.Label lblRemoveUser;
        private System.Windows.Forms.ComboBox cmbRemoveUser;
        private System.Windows.Forms.Button btnRemoveUser;
        private System.Windows.Forms.ListBox lstGroupMembers;
        private System.Windows.Forms.ListBox lstGroupChat;
        private System.Windows.Forms.TextBox txtGroupMessage;
        private System.Windows.Forms.Button btnSendGroupMessage;
        private System.Windows.Forms.Button btnSendGroupFile;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            cmbContacts = new ComboBox();
            txtMessage = new TextBox();
            btnSend = new Button();
            btnSendFile = new Button();
            lstChat = new ListBox();
            lblContacts = new Label();
            lblMessage = new Label();
            lblGroupName = new Label();
            txtGroupName = new TextBox();
            btnCreateGroup = new Button();
            cmbGroups = new ComboBox();
            lblAddUser = new Label();
            cmbAddUser = new ComboBox();
            btnAddUser = new Button();
            lblRemoveUser = new Label();
            cmbRemoveUser = new ComboBox();
            btnRemoveUser = new Button();
            lstGroupMembers = new ListBox();
            lstGroupChat = new ListBox();
            txtGroupMessage = new TextBox();
            btnSendGroupMessage = new Button();
            btnSendGroupFile = new Button();
            SuspendLayout();
            // 
            // cmbContacts
            // 
            cmbContacts.Location = new Point(97, 6);
            cmbContacts.Name = "cmbContacts";
            cmbContacts.Size = new Size(176, 23);
            cmbContacts.TabIndex = 1;
            cmbContacts.SelectedIndexChanged += cmbContacts_SelectedIndexChanged;
            cmbContacts.TextChanged += cmbContacts_SelectedIndexChanged;
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(97, 48);
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(176, 23);
            txtMessage.TabIndex = 3;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(288, 47);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(70, 23);
            btnSend.TabIndex = 4;
            btnSend.Text = "Надіслати";
            btnSend.Click += btnSend_Click;
            // 
            // btnSendFile
            // 
            btnSendFile.Location = new Point(370, 47);
            btnSendFile.Name = "btnSendFile";
            btnSendFile.Size = new Size(100, 23);
            btnSendFile.TabIndex = 6;
            btnSendFile.Text = "Надіслати файл";
            btnSendFile.Click += btnSendFile_Click;
            // 
            // lstChat
            // 
            lstChat.Location = new Point(9, 84);
            lstChat.Name = "lstChat";
            lstChat.Size = new Size(460, 184);
            lstChat.TabIndex = 5;
            // 
            // lblContacts
            // 
            lblContacts.AutoSize = true;
            lblContacts.Location = new Point(9, 9);
            lblContacts.Name = "lblContacts";
            lblContacts.Size = new Size(62, 15);
            lblContacts.TabIndex = 0;
            lblContacts.Text = "Контакти:";
            // 
            // lblMessage
            // 
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(9, 47);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(76, 15);
            lblMessage.TabIndex = 2;
            lblMessage.Text = "Повідомлення:";
            // 
            // lblGroupName
            // 
            lblGroupName.AutoSize = true;
            lblGroupName.Location = new Point(500, 9);
            lblGroupName.Name = "lblGroupName";
            lblGroupName.Size = new Size(106, 15);
            lblGroupName.TabIndex = 0;
            lblGroupName.Text = "Назва групи:";
            // 
            // txtGroupName
            // 
            txtGroupName.Location = new Point(620, 6);
            txtGroupName.Name = "txtGroupName";
            txtGroupName.Size = new Size(150, 23);
            txtGroupName.TabIndex = 1;
            // 
            // btnCreateGroup
            // 
            btnCreateGroup.Location = new Point(780, 6);
            btnCreateGroup.Name = "btnCreateGroup";
            btnCreateGroup.Size = new Size(75, 23);
            btnCreateGroup.TabIndex = 2;
            btnCreateGroup.Text = "Створити";
            btnCreateGroup.Click += btnCreateGroup_Click;
            // 
            // cmbGroups
            // 
            cmbGroups.Location = new Point(620, 45);
            cmbGroups.Name = "cmbGroups";
            cmbGroups.Size = new Size(150, 23);
            cmbGroups.TabIndex = 3;
            cmbGroups.SelectedIndexChanged += cmbGroups_SelectedIndexChanged;
            cmbGroups.TextChanged += cmbGroups_SelectedIndexChanged;
            // 
            // lblAddUser
            // 
            lblAddUser.AutoSize = true;
            lblAddUser.Location = new Point(500, 84);
            lblAddUser.Name = "lblAddUser";
            lblAddUser.Size = new Size(140, 15);
            lblAddUser.TabIndex = 4;
            lblAddUser.Text = "Додати:";
            // 
            // cmbAddUser
            // 
            cmbAddUser.Location = new Point(620, 81);
            cmbAddUser.Name = "cmbAddUser";
            cmbAddUser.Size = new Size(150, 23);
            cmbAddUser.TabIndex = 5;
            // 
            // btnAddUser
            // 
            btnAddUser.Location = new Point(780, 81);
            btnAddUser.Name = "btnAddUser";
            btnAddUser.Size = new Size(75, 23);
            btnAddUser.TabIndex = 6;
            btnAddUser.Text = "Додати";
            btnAddUser.Click += btnAddUser_Click;
            // 
            // lblRemoveUser
            // 
            lblRemoveUser.AutoSize = true;
            lblRemoveUser.Location = new Point(500, 123);
            lblRemoveUser.Name = "lblRemoveUser";
            lblRemoveUser.Size = new Size(132, 15);
            lblRemoveUser.TabIndex = 7;
            lblRemoveUser.Text = "Видалити:";
            // 
            // cmbRemoveUser
            // 
            cmbRemoveUser.Location = new Point(620, 120);
            cmbRemoveUser.Name = "cmbRemoveUser";
            cmbRemoveUser.Size = new Size(150, 23);
            cmbRemoveUser.TabIndex = 8;
            // 
            // btnRemoveUser
            // 
            btnRemoveUser.Location = new Point(780, 120);
            btnRemoveUser.Name = "btnRemoveUser";
            btnRemoveUser.Size = new Size(75, 23);
            btnRemoveUser.TabIndex = 9;
            btnRemoveUser.Text = "Видалити";
            btnRemoveUser.Click += btnRemoveUser_Click;
            // 
            // lstGroupMembers
            // 
            lstGroupMembers.Location = new Point(500, 160);
            lstGroupMembers.Name = "lstGroupMembers";
            lstGroupMembers.Size = new Size(380, 94);
            lstGroupMembers.TabIndex = 10;
            // 
            // lstGroupChat
            // 
            lstGroupChat.Location = new Point(500, 270);
            lstGroupChat.Name = "lstGroupChat";
            lstGroupChat.Size = new Size(380, 139);
            lstGroupChat.TabIndex = 11;
            // 
            // txtGroupMessage
            // 
            txtGroupMessage.Location = new Point(500, 420);
            txtGroupMessage.Name = "txtGroupMessage";
            txtGroupMessage.Size = new Size(270, 23);
            txtGroupMessage.TabIndex = 12;
            // 
            // btnSendGroupMessage
            // 
            btnSendGroupMessage.Location = new Point(780, 420);
            btnSendGroupMessage.Name = "btnSendGroupMessage";
            btnSendGroupMessage.Size = new Size(75, 23);
            btnSendGroupMessage.TabIndex = 13;
            btnSendGroupMessage.Text = "Надіслати";
            btnSendGroupMessage.Click += btnSendGroupMessage_Click;
            // 
            // btnSendGroupFile
            // 
            btnSendGroupFile.Location = new Point(860, 420);
            btnSendGroupFile.Name = "btnSendGroupFile";
            btnSendGroupFile.Size = new Size(75, 23);
            btnSendGroupFile.TabIndex = 14;
            btnSendGroupFile.Text = "Надіслати файл";
            btnSendGroupFile.Click += btnSendGroupFile_Click;
            // 
            // ChatForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1030, 487);
            Controls.Add(lblGroupName);
            Controls.Add(txtGroupName);
            Controls.Add(btnCreateGroup);
            Controls.Add(cmbGroups);
            Controls.Add(lblAddUser);
            Controls.Add(cmbAddUser);
            Controls.Add(btnAddUser);
            Controls.Add(lblRemoveUser);
            Controls.Add(cmbRemoveUser);
            Controls.Add(btnRemoveUser);
            Controls.Add(lstGroupMembers);
            Controls.Add(lstGroupChat);
            Controls.Add(txtGroupMessage);
            Controls.Add(btnSendGroupMessage);
            Controls.Add(btnSendGroupFile);
            Controls.Add(lblContacts);
            Controls.Add(cmbContacts);
            Controls.Add(lblMessage);
            Controls.Add(txtMessage);
            Controls.Add(btnSend);
            Controls.Add(btnSendFile);
            Controls.Add(lstChat);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "ChatForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Чат";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
