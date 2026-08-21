namespace GDK_tester
{
    partial class form_connect_admin
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.GRP_IP = new System.Windows.Forms.GroupBox();
            this.CONNECT_STC_ADDRESS = new System.Windows.Forms.Label();
            this.CONNECT_STC_PORT = new System.Windows.Forms.Label();
            this.CONNECT_EDT_ADDRESS = new System.Windows.Forms.TextBox();
            this.CONNECT_EDT_PORT = new GDK_tester.text_box_numeric();
            this.GRP_FEN = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.CONNECT_EDT_FEN_NAME = new System.Windows.Forms.TextBox();
            this.CONNECT_CHK_FEN_QUERY = new System.Windows.Forms.CheckBox();
            this.CONNECT_EDT_FEN_SERVER = new System.Windows.Forms.TextBox();
            this.CONNECT_EDT_FEN_PORT = new GDK_tester.text_box_numeric();
            this.CONNECT_STC_FEN_PORT = new System.Windows.Forms.Label();
            this.CONNECT_STC_FEN_SERVER = new System.Windows.Forms.Label();
            this.CB_MODE = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.CONNECT_BTN_CANCEL = new System.Windows.Forms.Button();
            this.CONNECT_BTN_CONNECT = new System.Windows.Forms.Button();
            this.CONNECT_STC_ID = new System.Windows.Forms.Label();
            this.CONNECT_STC_PASSWORD = new System.Windows.Forms.Label();
            this.CONNECT_EDT_ID = new System.Windows.Forms.TextBox();
            this.CONNECT_EDT_PASSWORD = new System.Windows.Forms.TextBox();
            this.GRP_IP.SuspendLayout();
            this.GRP_FEN.SuspendLayout();
            this.SuspendLayout();
            // 
            // GRP_IP
            // 
            this.GRP_IP.Controls.Add(this.CONNECT_STC_ADDRESS);
            this.GRP_IP.Controls.Add(this.CONNECT_STC_PORT);
            this.GRP_IP.Controls.Add(this.CONNECT_EDT_ADDRESS);
            this.GRP_IP.Controls.Add(this.CONNECT_EDT_PORT);
            this.GRP_IP.Location = new System.Drawing.Point(15, 54);
            this.GRP_IP.Name = "GRP_IP";
            this.GRP_IP.Size = new System.Drawing.Size(255, 129);
            this.GRP_IP.TabIndex = 1;
            this.GRP_IP.TabStop = false;
            this.GRP_IP.Visible = false;
            // 
            // CONNECT_STC_ADDRESS
            // 
            this.CONNECT_STC_ADDRESS.Location = new System.Drawing.Point(30, 26);
            this.CONNECT_STC_ADDRESS.Name = "CONNECT_STC_ADDRESS";
            this.CONNECT_STC_ADDRESS.Size = new System.Drawing.Size(53, 13);
            this.CONNECT_STC_ADDRESS.TabIndex = 3;
            this.CONNECT_STC_ADDRESS.Text = "Address :";
            this.CONNECT_STC_ADDRESS.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CONNECT_STC_PORT
            // 
            this.CONNECT_STC_PORT.AutoSize = true;
            this.CONNECT_STC_PORT.Location = new System.Drawing.Point(49, 51);
            this.CONNECT_STC_PORT.Name = "CONNECT_STC_PORT";
            this.CONNECT_STC_PORT.Size = new System.Drawing.Size(34, 13);
            this.CONNECT_STC_PORT.TabIndex = 4;
            this.CONNECT_STC_PORT.Text = "Port :";
            this.CONNECT_STC_PORT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CONNECT_EDT_ADDRESS
            // 
            this.CONNECT_EDT_ADDRESS.Location = new System.Drawing.Point(85, 22);
            this.CONNECT_EDT_ADDRESS.MaxLength = 64;
            this.CONNECT_EDT_ADDRESS.Name = "CONNECT_EDT_ADDRESS";
            this.CONNECT_EDT_ADDRESS.Size = new System.Drawing.Size(154, 20);
            this.CONNECT_EDT_ADDRESS.TabIndex = 1;
            // 
            // CONNECT_EDT_PORT
            // 
            this.CONNECT_EDT_PORT.AllowSpace = false;
            this.CONNECT_EDT_PORT.Location = new System.Drawing.Point(85, 48);
            this.CONNECT_EDT_PORT.MaxLength = 8;
            this.CONNECT_EDT_PORT.Name = "CONNECT_EDT_PORT";
            this.CONNECT_EDT_PORT.Size = new System.Drawing.Size(60, 20);
            this.CONNECT_EDT_PORT.TabIndex = 2;
            // 
            // GRP_FEN
            // 
            this.GRP_FEN.Controls.Add(this.label2);
            this.GRP_FEN.Controls.Add(this.CONNECT_EDT_FEN_NAME);
            this.GRP_FEN.Controls.Add(this.CONNECT_CHK_FEN_QUERY);
            this.GRP_FEN.Controls.Add(this.CONNECT_EDT_FEN_SERVER);
            this.GRP_FEN.Controls.Add(this.CONNECT_EDT_FEN_PORT);
            this.GRP_FEN.Controls.Add(this.CONNECT_STC_FEN_PORT);
            this.GRP_FEN.Controls.Add(this.CONNECT_STC_FEN_SERVER);
            this.GRP_FEN.Location = new System.Drawing.Point(15, 54);
            this.GRP_FEN.Name = "GRP_FEN";
            this.GRP_FEN.Size = new System.Drawing.Size(255, 129);
            this.GRP_FEN.TabIndex = 2;
            this.GRP_FEN.TabStop = false;
            this.GRP_FEN.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Name :";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CONNECT_EDT_FEN_NAME
            // 
            this.CONNECT_EDT_FEN_NAME.Location = new System.Drawing.Point(85, 74);
            this.CONNECT_EDT_FEN_NAME.MaxLength = 64;
            this.CONNECT_EDT_FEN_NAME.Name = "CONNECT_EDT_FEN_NAME";
            this.CONNECT_EDT_FEN_NAME.Size = new System.Drawing.Size(154, 20);
            this.CONNECT_EDT_FEN_NAME.TabIndex = 3;
            // 
            // CONNECT_CHK_FEN_QUERY
            // 
            this.CONNECT_CHK_FEN_QUERY.AutoSize = true;
            this.CONNECT_CHK_FEN_QUERY.Location = new System.Drawing.Point(85, 100);
            this.CONNECT_CHK_FEN_QUERY.Name = "CONNECT_CHK_FEN_QUERY";
            this.CONNECT_CHK_FEN_QUERY.Size = new System.Drawing.Size(78, 17);
            this.CONNECT_CHK_FEN_QUERY.TabIndex = 3;
            this.CONNECT_CHK_FEN_QUERY.Text = "FEN Query";
            this.CONNECT_CHK_FEN_QUERY.UseVisualStyleBackColor = true;
            // 
            // CONNECT_EDT_FEN_SERVER
            // 
            this.CONNECT_EDT_FEN_SERVER.Location = new System.Drawing.Point(85, 22);
            this.CONNECT_EDT_FEN_SERVER.MaxLength = 64;
            this.CONNECT_EDT_FEN_SERVER.Name = "CONNECT_EDT_FEN_SERVER";
            this.CONNECT_EDT_FEN_SERVER.Size = new System.Drawing.Size(154, 20);
            this.CONNECT_EDT_FEN_SERVER.TabIndex = 1;
            // 
            // CONNECT_EDT_FEN_PORT
            // 
            this.CONNECT_EDT_FEN_PORT.AllowSpace = false;
            this.CONNECT_EDT_FEN_PORT.Location = new System.Drawing.Point(85, 48);
            this.CONNECT_EDT_FEN_PORT.MaxLength = 8;
            this.CONNECT_EDT_FEN_PORT.Name = "CONNECT_EDT_FEN_PORT";
            this.CONNECT_EDT_FEN_PORT.Size = new System.Drawing.Size(60, 20);
            this.CONNECT_EDT_FEN_PORT.TabIndex = 2;
            // 
            // CONNECT_STC_FEN_PORT
            // 
            this.CONNECT_STC_FEN_PORT.AutoSize = true;
            this.CONNECT_STC_FEN_PORT.Location = new System.Drawing.Point(49, 51);
            this.CONNECT_STC_FEN_PORT.Name = "CONNECT_STC_FEN_PORT";
            this.CONNECT_STC_FEN_PORT.Size = new System.Drawing.Size(34, 13);
            this.CONNECT_STC_FEN_PORT.TabIndex = 2;
            this.CONNECT_STC_FEN_PORT.Text = "Port :";
            this.CONNECT_STC_FEN_PORT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CONNECT_STC_FEN_SERVER
            // 
            this.CONNECT_STC_FEN_SERVER.AutoSize = true;
            this.CONNECT_STC_FEN_SERVER.Location = new System.Drawing.Point(15, 26);
            this.CONNECT_STC_FEN_SERVER.Name = "CONNECT_STC_FEN_SERVER";
            this.CONNECT_STC_FEN_SERVER.Size = new System.Drawing.Size(68, 13);
            this.CONNECT_STC_FEN_SERVER.TabIndex = 4;
            this.CONNECT_STC_FEN_SERVER.Text = "FEN Server :";
            this.CONNECT_STC_FEN_SERVER.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_MODE
            // 
            this.CB_MODE.FormattingEnabled = true;
            this.CB_MODE.Location = new System.Drawing.Point(101, 17);
            this.CB_MODE.Name = "CB_MODE";
            this.CB_MODE.Size = new System.Drawing.Size(150, 21);
            this.CB_MODE.TabIndex = 8;
            this.CB_MODE.SelectedIndexChanged += new System.EventHandler(this.selected_connect_mode);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Connect Mode :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CONNECT_BTN_CANCEL
            // 
            this.CONNECT_BTN_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CONNECT_BTN_CANCEL.Location = new System.Drawing.Point(203, 254);
            this.CONNECT_BTN_CANCEL.Name = "CONNECT_BTN_CANCEL";
            this.CONNECT_BTN_CANCEL.Size = new System.Drawing.Size(64, 25);
            this.CONNECT_BTN_CANCEL.TabIndex = 7;
            this.CONNECT_BTN_CANCEL.Text = "Cancel";
            this.CONNECT_BTN_CANCEL.UseVisualStyleBackColor = true;
            // 
            // CONNECT_BTN_CONNECT
            // 
            this.CONNECT_BTN_CONNECT.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.CONNECT_BTN_CONNECT.Enabled = false;
            this.CONNECT_BTN_CONNECT.Location = new System.Drawing.Point(135, 254);
            this.CONNECT_BTN_CONNECT.Name = "CONNECT_BTN_CONNECT";
            this.CONNECT_BTN_CONNECT.Size = new System.Drawing.Size(63, 25);
            this.CONNECT_BTN_CONNECT.TabIndex = 6;
            this.CONNECT_BTN_CONNECT.Text = "Connect";
            this.CONNECT_BTN_CONNECT.UseVisualStyleBackColor = true;
            this.CONNECT_BTN_CONNECT.Click += new System.EventHandler(this.on_btn_connect);
            // 
            // CONNECT_STC_ID
            // 
            this.CONNECT_STC_ID.AutoSize = true;
            this.CONNECT_STC_ID.Location = new System.Drawing.Point(45, 196);
            this.CONNECT_STC_ID.Name = "CONNECT_STC_ID";
            this.CONNECT_STC_ID.Size = new System.Drawing.Size(50, 13);
            this.CONNECT_STC_ID.TabIndex = 10;
            this.CONNECT_STC_ID.Text = "User ID :";
            this.CONNECT_STC_ID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CONNECT_STC_PASSWORD
            // 
            this.CONNECT_STC_PASSWORD.AutoSize = true;
            this.CONNECT_STC_PASSWORD.Location = new System.Drawing.Point(35, 220);
            this.CONNECT_STC_PASSWORD.Name = "CONNECT_STC_PASSWORD";
            this.CONNECT_STC_PASSWORD.Size = new System.Drawing.Size(60, 13);
            this.CONNECT_STC_PASSWORD.TabIndex = 11;
            this.CONNECT_STC_PASSWORD.Text = "Password :";
            this.CONNECT_STC_PASSWORD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CONNECT_EDT_ID
            // 
            this.CONNECT_EDT_ID.Location = new System.Drawing.Point(97, 193);
            this.CONNECT_EDT_ID.MaxLength = 32;
            this.CONNECT_EDT_ID.Name = "CONNECT_EDT_ID";
            this.CONNECT_EDT_ID.Size = new System.Drawing.Size(154, 20);
            this.CONNECT_EDT_ID.TabIndex = 4;
            // 
            // CONNECT_EDT_PASSWORD
            // 
            this.CONNECT_EDT_PASSWORD.Location = new System.Drawing.Point(97, 217);
            this.CONNECT_EDT_PASSWORD.MaxLength = 32;
            this.CONNECT_EDT_PASSWORD.Name = "CONNECT_EDT_PASSWORD";
            this.CONNECT_EDT_PASSWORD.PasswordChar = '*';
            this.CONNECT_EDT_PASSWORD.Size = new System.Drawing.Size(154, 20);
            this.CONNECT_EDT_PASSWORD.TabIndex = 5;
            // 
            // form_connect_admin
            // 
            this.AcceptButton = this.CONNECT_BTN_CONNECT;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CONNECT_BTN_CANCEL;
            this.ClientSize = new System.Drawing.Size(284, 291);
            this.Controls.Add(this.CONNECT_STC_ID);
            this.Controls.Add(this.CONNECT_STC_PASSWORD);
            this.Controls.Add(this.CONNECT_EDT_ID);
            this.Controls.Add(this.CONNECT_EDT_PASSWORD);
            this.Controls.Add(this.CONNECT_BTN_CANCEL);
            this.Controls.Add(this.CONNECT_BTN_CONNECT);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.GRP_IP);
            this.Controls.Add(this.CB_MODE);
            this.Controls.Add(this.GRP_FEN);
            this.Font = new System.Drawing.Font("Tahoma", 8F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "form_connect_admin";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Connect";
            this.GRP_IP.ResumeLayout(false);
            this.GRP_IP.PerformLayout();
            this.GRP_FEN.ResumeLayout(false);
            this.GRP_FEN.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox GRP_IP;
        private System.Windows.Forms.Label CONNECT_STC_ADDRESS;
        private System.Windows.Forms.Label CONNECT_STC_PORT;
        private System.Windows.Forms.TextBox CONNECT_EDT_ADDRESS;
        private text_box_numeric CONNECT_EDT_PORT;
        private System.Windows.Forms.GroupBox GRP_FEN;
        private System.Windows.Forms.CheckBox CONNECT_CHK_FEN_QUERY;
        private System.Windows.Forms.TextBox CONNECT_EDT_FEN_SERVER;
        private text_box_numeric CONNECT_EDT_FEN_PORT;
        private System.Windows.Forms.Label CONNECT_STC_FEN_PORT;
        private System.Windows.Forms.Label CONNECT_STC_FEN_SERVER;
        private System.Windows.Forms.ComboBox CB_MODE;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox CONNECT_EDT_FEN_NAME;
        private System.Windows.Forms.Button CONNECT_BTN_CANCEL;
        private System.Windows.Forms.Button CONNECT_BTN_CONNECT;
        private System.Windows.Forms.Label CONNECT_STC_ID;
        private System.Windows.Forms.Label CONNECT_STC_PASSWORD;
        private System.Windows.Forms.TextBox CONNECT_EDT_ID;
        private System.Windows.Forms.TextBox CONNECT_EDT_PASSWORD;
    }
}