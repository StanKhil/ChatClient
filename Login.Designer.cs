namespace ChatClient
{
    partial class Login
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtLogin;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label lblLogin;

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
            txtLogin = new TextBox();
            btnLogin = new Button();
            lblLogin = new Label();
            SuspendLayout();
            // 
            // txtLogin
            // 
            txtLogin.Location = new Point(178, 158);
            txtLogin.Name = "txtLogin";
            txtLogin.Size = new Size(176, 23);
            txtLogin.TabIndex = 1;
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(179, 187);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(175, 28);
            btnLogin.TabIndex = 2;
            btnLogin.Text = "Войти";
            btnLogin.Click += btnLogin_Click;
            // 
            // lblLogin
            // 
            lblLogin.AutoSize = true;
            lblLogin.Location = new Point(221, 140);
            lblLogin.Name = "lblLogin";
            lblLogin.Size = new Size(89, 15);
            lblLogin.TabIndex = 0;
            lblLogin.Text = "Введите логин:";
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(537, 349);
            Controls.Add(lblLogin);
            Controls.Add(txtLogin);
            Controls.Add(btnLogin);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "Login";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Вход в чат";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
