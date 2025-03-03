namespace ChatClient
{
    partial class ChatForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ComboBox cmbContacts;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.ListBox lstChat;
        private System.Windows.Forms.Label lblContacts;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button btnSendFile;

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
            btnSend.Text = "Отправить";
            btnSend.Click += btnSend_Click;
            // 
            // btnSendFile
            // 
            btnSendFile.Location = new Point(370, 47);
            btnSendFile.Name = "btnSendFile";
            btnSendFile.Size = new Size(100, 23);
            btnSendFile.TabIndex = 6;
            btnSendFile.Text = "Отправить файл";
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
            lblContacts.Text = "Контакты:";
            // 
            // lblMessage
            // 
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(9, 47);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(76, 15);
            lblMessage.TabIndex = 2;
            lblMessage.Text = "Сообщение:";
            // 
            // ChatForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(546, 291);
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
