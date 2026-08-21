namespace GDK_tester
{
    partial class form_connect
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.CONNECT_STC_ADDRESS = new System.Windows.Forms.Label();
            this.CONNECT_STC_PORT = new System.Windows.Forms.Label();
            this.CONNECT_STC_PORT_REMOTE_SETUP = new System.Windows.Forms.Label();
            this.CONNECT_STC_PORT_REMOTE_SETUP_HELP = new System.Windows.Forms.Label();
            this.CONNECT_STC_PORT_CHECK = new System.Windows.Forms.Label();
            this.CONNECT_STC_PORT_CHECK_HELP = new System.Windows.Forms.Label();
            this.CONNECT_STC_FEN_SERVER = new System.Windows.Forms.Label();
            this.CONNECT_STC_FEN_PORT = new System.Windows.Forms.Label();
            this.CONNECT_STC_ID = new System.Windows.Forms.Label();
            this.CONNECT_STC_PASSWORD = new System.Windows.Forms.Label();
            this.CONNECT_EDT_ADDRESS = new System.Windows.Forms.TextBox();
            this.CONNECT_EDT_FEN_SERVER = new System.Windows.Forms.TextBox();
            this.CONNECT_EDT_ID = new System.Windows.Forms.TextBox();
            this.CONNECT_EDT_PASSWORD = new System.Windows.Forms.TextBox();
            this.CONNECT_CHK_FEN_USE = new System.Windows.Forms.CheckBox();
            this.CONNECT_CHK_FEN_QUERY = new System.Windows.Forms.CheckBox();
            this.CONNECT_CHK_CHECK_PORT_USE = new System.Windows.Forms.CheckBox();
            this.CONNECT_CHK_CHECK_PORT_UNITY = new System.Windows.Forms.CheckBox();
            this.CONNECT_CHK_CHECK_G2_SEARCH = new System.Windows.Forms.CheckBox();
            this.CONNECT_CHK_CHECK_IDR = new System.Windows.Forms.CheckBox();
            this.CONNECT_GRP_SITE = new System.Windows.Forms.GroupBox();
            this.CONNECT_EDT_PORT = new GDK_tester.text_box_numeric();
            this.CONNECT_EDT_PORT_REMOTE_SETUP = new GDK_tester.text_box_numeric();
            this.CONNECT_GRP_CHECK = new System.Windows.Forms.GroupBox();
            this.CONNECT_EDT_PORT_CHECK = new GDK_tester.text_box_numeric();
            this.CONNECT_GRP_FEN = new System.Windows.Forms.GroupBox();
            this.CONNECT_EDT_FEN_PORT = new GDK_tester.text_box_numeric();
            this.CONNECT_BTN_CONNECT = new System.Windows.Forms.Button();
            this.CONNECT_BTN_CANCEL = new System.Windows.Forms.Button();
            this.CONNECT_GRP_SITE.SuspendLayout();
            this.CONNECT_GRP_CHECK.SuspendLayout();
            this.CONNECT_GRP_FEN.SuspendLayout();
            this.SuspendLayout();
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
            this.CONNECT_STC_PORT.Location = new System.Drawing.Point(49, 50);
            this.CONNECT_STC_PORT.Name = "CONNECT_STC_PORT";
            this.CONNECT_STC_PORT.Size = new System.Drawing.Size(34, 13);
            this.CONNECT_STC_PORT.TabIndex = 4;
            this.CONNECT_STC_PORT.Text = "Port :";
            this.CONNECT_STC_PORT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CONNECT_STC_PORT_REMOTE_SETUP
            // 
            this.CONNECT_STC_PORT_REMOTE_SETUP.AutoSize = true;
            this.CONNECT_STC_PORT_REMOTE_SETUP.Location = new System.Drawing.Point(18, 74);
            this.CONNECT_STC_PORT_REMOTE_SETUP.Name = "CONNECT_STC_PORT_REMOTE_SETUP";
            this.CONNECT_STC_PORT_REMOTE_SETUP.Size = new System.Drawing.Size(65, 13);
            this.CONNECT_STC_PORT_REMOTE_SETUP.TabIndex = 5;
            this.CONNECT_STC_PORT_REMOTE_SETUP.Text = "Setup Port :";
            this.CONNECT_STC_PORT_REMOTE_SETUP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CONNECT_STC_PORT_REMOTE_SETUP_HELP
            // 
            this.CONNECT_STC_PORT_REMOTE_SETUP_HELP.AutoSize = true;
            this.CONNECT_STC_PORT_REMOTE_SETUP_HELP.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.CONNECT_STC_PORT_REMOTE_SETUP_HELP.Location = new System.Drawing.Point(151, 74);
            this.CONNECT_STC_PORT_REMOTE_SETUP_HELP.Name = "CONNECT_STC_PORT_REMOTE_SETUP_HELP";
            this.CONNECT_STC_PORT_REMOTE_SETUP_HELP.Size = new System.Drawing.Size(53, 13);
            this.CONNECT_STC_PORT_REMOTE_SETUP_HELP.TabIndex = 6;
            this.CONNECT_STC_PORT_REMOTE_SETUP_HELP.Text = "(optional)";
            this.CONNECT_STC_PORT_REMOTE_SETUP_HELP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CONNECT_STC_PORT_CHECK
            // 
            this.CONNECT_STC_PORT_CHECK.AutoSize = true;
            this.CONNECT_STC_PORT_CHECK.Location = new System.Drawing.Point(17, 43);
            this.CONNECT_STC_PORT_CHECK.Name = "CONNECT_STC_PORT_CHECK";
            this.CONNECT_STC_PORT_CHECK.Size = new System.Drawing.Size(66, 13);
            this.CONNECT_STC_PORT_CHECK.TabIndex = 5;
            this.CONNECT_STC_PORT_CHECK.Text = "Check Port :";
            this.CONNECT_STC_PORT_CHECK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CONNECT_STC_PORT_CHECK_HELP
            // 
            this.CONNECT_STC_PORT_CHECK_HELP.AutoSize = true;
            this.CONNECT_STC_PORT_CHECK_HELP.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.CONNECT_STC_PORT_CHECK_HELP.Location = new System.Drawing.Point(149, 43);
            this.CONNECT_STC_PORT_CHECK_HELP.Name = "CONNECT_STC_PORT_CHECK_HELP";
            this.CONNECT_STC_PORT_CHECK_HELP.Size = new System.Drawing.Size(67, 13);
            this.CONNECT_STC_PORT_CHECK_HELP.TabIndex = 6;
            this.CONNECT_STC_PORT_CHECK_HELP.Text = "(watch port)";
            this.CONNECT_STC_PORT_CHECK_HELP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CONNECT_STC_FEN_SERVER
            // 
            this.CONNECT_STC_FEN_SERVER.AutoSize = true;
            this.CONNECT_STC_FEN_SERVER.Enabled = false;
            this.CONNECT_STC_FEN_SERVER.Location = new System.Drawing.Point(15, 26);
            this.CONNECT_STC_FEN_SERVER.Name = "CONNECT_STC_FEN_SERVER";
            this.CONNECT_STC_FEN_SERVER.Size = new System.Drawing.Size(68, 13);
            this.CONNECT_STC_FEN_SERVER.TabIndex = 4;
            this.CONNECT_STC_FEN_SERVER.Text = "FEN Server :";
            this.CONNECT_STC_FEN_SERVER.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CONNECT_STC_FEN_PORT
            // 
            this.CONNECT_STC_FEN_PORT.AutoSize = true;
            this.CONNECT_STC_FEN_PORT.Enabled = false;
            this.CONNECT_STC_FEN_PORT.Location = new System.Drawing.Point(49, 50);
            this.CONNECT_STC_FEN_PORT.Name = "CONNECT_STC_FEN_PORT";
            this.CONNECT_STC_FEN_PORT.Size = new System.Drawing.Size(34, 13);
            this.CONNECT_STC_FEN_PORT.TabIndex = 5;
            this.CONNECT_STC_FEN_PORT.Text = "Port :";
            this.CONNECT_STC_FEN_PORT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CONNECT_STC_ID
            // 
            this.CONNECT_STC_ID.AutoSize = true;
            this.CONNECT_STC_ID.Location = new System.Drawing.Point(42, 131);
            this.CONNECT_STC_ID.Name = "CONNECT_STC_ID";
            this.CONNECT_STC_ID.Size = new System.Drawing.Size(50, 13);
            this.CONNECT_STC_ID.TabIndex = 6;
            this.CONNECT_STC_ID.Text = "User ID :";
            this.CONNECT_STC_ID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CONNECT_STC_PASSWORD
            // 
            this.CONNECT_STC_PASSWORD.AutoSize = true;
            this.CONNECT_STC_PASSWORD.Location = new System.Drawing.Point(32, 155);
            this.CONNECT_STC_PASSWORD.Name = "CONNECT_STC_PASSWORD";
            this.CONNECT_STC_PASSWORD.Size = new System.Drawing.Size(60, 13);
            this.CONNECT_STC_PASSWORD.TabIndex = 7;
            this.CONNECT_STC_PASSWORD.Text = "Password :";
            this.CONNECT_STC_PASSWORD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CONNECT_EDT_ADDRESS
            // 
            this.CONNECT_EDT_ADDRESS.Location = new System.Drawing.Point(85, 23);
            this.CONNECT_EDT_ADDRESS.MaxLength = 64;
            this.CONNECT_EDT_ADDRESS.Name = "CONNECT_EDT_ADDRESS";
            this.CONNECT_EDT_ADDRESS.Size = new System.Drawing.Size(154, 20);
            this.CONNECT_EDT_ADDRESS.TabIndex = 0;
            // 
            // CONNECT_EDT_FEN_SERVER
            // 
            this.CONNECT_EDT_FEN_SERVER.Enabled = false;
            this.CONNECT_EDT_FEN_SERVER.Location = new System.Drawing.Point(85, 22);
            this.CONNECT_EDT_FEN_SERVER.MaxLength = 64;
            this.CONNECT_EDT_FEN_SERVER.Name = "CONNECT_EDT_FEN_SERVER";
            this.CONNECT_EDT_FEN_SERVER.Size = new System.Drawing.Size(154, 20);
            this.CONNECT_EDT_FEN_SERVER.TabIndex = 1;
            // 
            // CONNECT_EDT_ID
            // 
            this.CONNECT_EDT_ID.Location = new System.Drawing.Point(94, 128);
            this.CONNECT_EDT_ID.MaxLength = 32;
            this.CONNECT_EDT_ID.Name = "CONNECT_EDT_ID";
            this.CONNECT_EDT_ID.Size = new System.Drawing.Size(154, 20);
            this.CONNECT_EDT_ID.TabIndex = 2;
            // 
            // CONNECT_EDT_PASSWORD
            // 
            this.CONNECT_EDT_PASSWORD.Location = new System.Drawing.Point(94, 152);
            this.CONNECT_EDT_PASSWORD.MaxLength = 32;
            this.CONNECT_EDT_PASSWORD.Name = "CONNECT_EDT_PASSWORD";
            this.CONNECT_EDT_PASSWORD.PasswordChar = '*';
            this.CONNECT_EDT_PASSWORD.Size = new System.Drawing.Size(154, 20);
            this.CONNECT_EDT_PASSWORD.TabIndex = 3;
            // 
            // CONNECT_CHK_FEN_USE
            // 
            this.CONNECT_CHK_FEN_USE.AutoSize = true;
            this.CONNECT_CHK_FEN_USE.Location = new System.Drawing.Point(17, 0);
            this.CONNECT_CHK_FEN_USE.Name = "CONNECT_CHK_FEN_USE";
            this.CONNECT_CHK_FEN_USE.Size = new System.Drawing.Size(66, 17);
            this.CONNECT_CHK_FEN_USE.TabIndex = 0;
            this.CONNECT_CHK_FEN_USE.Text = "Use FEN";
            this.CONNECT_CHK_FEN_USE.UseVisualStyleBackColor = true;
            this.CONNECT_CHK_FEN_USE.Click += new System.EventHandler(this.on_chk_fen_use);
            // 
            // CONNECT_CHK_FEN_QUERY
            // 
            this.CONNECT_CHK_FEN_QUERY.AutoSize = true;
            this.CONNECT_CHK_FEN_QUERY.Enabled = false;
            this.CONNECT_CHK_FEN_QUERY.Location = new System.Drawing.Point(85, 72);
            this.CONNECT_CHK_FEN_QUERY.Name = "CONNECT_CHK_FEN_QUERY";
            this.CONNECT_CHK_FEN_QUERY.Size = new System.Drawing.Size(78, 17);
            this.CONNECT_CHK_FEN_QUERY.TabIndex = 3;
            this.CONNECT_CHK_FEN_QUERY.Text = "FEN Query";
            this.CONNECT_CHK_FEN_QUERY.UseVisualStyleBackColor = true;
            // 
            // CONNECT_CHK_CHECK_PORT_USE
            // 
            this.CONNECT_CHK_CHECK_PORT_USE.AutoSize = true;
            this.CONNECT_CHK_CHECK_PORT_USE.Location = new System.Drawing.Point(85, 18);
            this.CONNECT_CHK_CHECK_PORT_USE.Name = "CONNECT_CHK_CHECK_PORT_USE";
            this.CONNECT_CHK_CHECK_PORT_USE.Size = new System.Drawing.Size(111, 17);
            this.CONNECT_CHK_CHECK_PORT_USE.TabIndex = 0;
            this.CONNECT_CHK_CHECK_PORT_USE.Text = "Use Check Device";
            this.CONNECT_CHK_CHECK_PORT_USE.UseVisualStyleBackColor = true;
            this.CONNECT_CHK_CHECK_PORT_USE.Click += new System.EventHandler(this.on_chk_check_port_use);
            // 
            // CONNECT_CHK_CHECK_PORT_UNITY
            // 
            this.CONNECT_CHK_CHECK_PORT_UNITY.AutoSize = true;
            this.CONNECT_CHK_CHECK_PORT_UNITY.Location = new System.Drawing.Point(85, 70);
            this.CONNECT_CHK_CHECK_PORT_UNITY.Name = "CONNECT_CHK_CHECK_PORT_UNITY";
            this.CONNECT_CHK_CHECK_PORT_UNITY.Size = new System.Drawing.Size(74, 17);
            this.CONNECT_CHK_CHECK_PORT_UNITY.TabIndex = 2;
            this.CONNECT_CHK_CHECK_PORT_UNITY.Text = "Port Unity";
            this.CONNECT_CHK_CHECK_PORT_UNITY.UseVisualStyleBackColor = true;
            // 
            // CONNECT_CHK_CHECK_G2_SEARCH
            // 
            this.CONNECT_CHK_CHECK_G2_SEARCH.AutoSize = true;
            this.CONNECT_CHK_CHECK_G2_SEARCH.Location = new System.Drawing.Point(85, 89);
            this.CONNECT_CHK_CHECK_G2_SEARCH.Name = "CONNECT_CHK_CHECK_G2_SEARCH";
            this.CONNECT_CHK_CHECK_G2_SEARCH.Size = new System.Drawing.Size(113, 17);
            this.CONNECT_CHK_CHECK_G2_SEARCH.TabIndex = 3;
            this.CONNECT_CHK_CHECK_G2_SEARCH.Text = "Search Version G2";
            this.CONNECT_CHK_CHECK_G2_SEARCH.UseVisualStyleBackColor = true;
            // 
            // CONNECT_CHK_CHECK_IDR
            // 
            this.CONNECT_CHK_CHECK_IDR.AutoSize = true;
            this.CONNECT_CHK_CHECK_IDR.Location = new System.Drawing.Point(85, 108);
            this.CONNECT_CHK_CHECK_IDR.Name = "CONNECT_CHK_CHECK_IDR";
            this.CONNECT_CHK_CHECK_IDR.Size = new System.Drawing.Size(114, 17);
            this.CONNECT_CHK_CHECK_IDR.TabIndex = 4;
            this.CONNECT_CHK_CHECK_IDR.Text = "Old PC-Based DVR";
            this.CONNECT_CHK_CHECK_IDR.UseVisualStyleBackColor = true;
            // 
            // CONNECT_GRP_SITE
            // 
            this.CONNECT_GRP_SITE.Controls.Add(this.CONNECT_STC_ADDRESS);
            this.CONNECT_GRP_SITE.Controls.Add(this.CONNECT_STC_PORT);
            this.CONNECT_GRP_SITE.Controls.Add(this.CONNECT_STC_PORT_REMOTE_SETUP);
            this.CONNECT_GRP_SITE.Controls.Add(this.CONNECT_STC_PORT_REMOTE_SETUP_HELP);
            this.CONNECT_GRP_SITE.Controls.Add(this.CONNECT_EDT_ADDRESS);
            this.CONNECT_GRP_SITE.Controls.Add(this.CONNECT_EDT_PORT);
            this.CONNECT_GRP_SITE.Controls.Add(this.CONNECT_EDT_PORT_REMOTE_SETUP);
            this.CONNECT_GRP_SITE.Location = new System.Drawing.Point(9, 6);
            this.CONNECT_GRP_SITE.Name = "CONNECT_GRP_SITE";
            this.CONNECT_GRP_SITE.Size = new System.Drawing.Size(255, 105);
            this.CONNECT_GRP_SITE.TabIndex = 0;
            this.CONNECT_GRP_SITE.TabStop = false;
            this.CONNECT_GRP_SITE.Text = "Site";
            // 
            // CONNECT_EDT_PORT
            // 
            this.CONNECT_EDT_PORT.AllowSpace = false;
            this.CONNECT_EDT_PORT.Location = new System.Drawing.Point(85, 47);
            this.CONNECT_EDT_PORT.MaxLength = 8;
            this.CONNECT_EDT_PORT.Name = "CONNECT_EDT_PORT";
            this.CONNECT_EDT_PORT.Size = new System.Drawing.Size(60, 20);
            this.CONNECT_EDT_PORT.TabIndex = 1;
            // 
            // CONNECT_EDT_PORT_REMOTE_SETUP
            // 
            this.CONNECT_EDT_PORT_REMOTE_SETUP.AllowSpace = false;
            this.CONNECT_EDT_PORT_REMOTE_SETUP.Location = new System.Drawing.Point(85, 71);
            this.CONNECT_EDT_PORT_REMOTE_SETUP.MaxLength = 8;
            this.CONNECT_EDT_PORT_REMOTE_SETUP.Name = "CONNECT_EDT_PORT_REMOTE_SETUP";
            this.CONNECT_EDT_PORT_REMOTE_SETUP.Size = new System.Drawing.Size(60, 20);
            this.CONNECT_EDT_PORT_REMOTE_SETUP.TabIndex = 2;
            // 
            // CONNECT_GRP_CHECK
            // 
            this.CONNECT_GRP_CHECK.Controls.Add(this.CONNECT_CHK_CHECK_G2_SEARCH);
            this.CONNECT_GRP_CHECK.Controls.Add(this.CONNECT_CHK_CHECK_IDR);
            this.CONNECT_GRP_CHECK.Controls.Add(this.CONNECT_CHK_CHECK_PORT_UNITY);
            this.CONNECT_GRP_CHECK.Controls.Add(this.CONNECT_CHK_CHECK_PORT_USE);
            this.CONNECT_GRP_CHECK.Controls.Add(this.CONNECT_STC_PORT_CHECK);
            this.CONNECT_GRP_CHECK.Controls.Add(this.CONNECT_EDT_PORT_CHECK);
            this.CONNECT_GRP_CHECK.Controls.Add(this.CONNECT_STC_PORT_CHECK_HELP);
            this.CONNECT_GRP_CHECK.Location = new System.Drawing.Point(270, 112);
            this.CONNECT_GRP_CHECK.Name = "CONNECT_GRP_CHECK";
            this.CONNECT_GRP_CHECK.Size = new System.Drawing.Size(255, 136);
            this.CONNECT_GRP_CHECK.TabIndex = 8;
            this.CONNECT_GRP_CHECK.TabStop = false;
            // 
            // CONNECT_EDT_PORT_CHECK
            // 
            this.CONNECT_EDT_PORT_CHECK.AllowSpace = false;
            this.CONNECT_EDT_PORT_CHECK.Location = new System.Drawing.Point(85, 40);
            this.CONNECT_EDT_PORT_CHECK.MaxLength = 8;
            this.CONNECT_EDT_PORT_CHECK.Name = "CONNECT_EDT_PORT_CHECK";
            this.CONNECT_EDT_PORT_CHECK.Size = new System.Drawing.Size(60, 20);
            this.CONNECT_EDT_PORT_CHECK.TabIndex = 1;
            // 
            // CONNECT_GRP_FEN
            // 
            this.CONNECT_GRP_FEN.Controls.Add(this.CONNECT_CHK_FEN_USE);
            this.CONNECT_GRP_FEN.Controls.Add(this.CONNECT_CHK_FEN_QUERY);
            this.CONNECT_GRP_FEN.Controls.Add(this.CONNECT_EDT_FEN_SERVER);
            this.CONNECT_GRP_FEN.Controls.Add(this.CONNECT_EDT_FEN_PORT);
            this.CONNECT_GRP_FEN.Controls.Add(this.CONNECT_STC_FEN_PORT);
            this.CONNECT_GRP_FEN.Controls.Add(this.CONNECT_STC_FEN_SERVER);
            this.CONNECT_GRP_FEN.Location = new System.Drawing.Point(270, 7);
            this.CONNECT_GRP_FEN.Name = "CONNECT_GRP_FEN";
            this.CONNECT_GRP_FEN.Size = new System.Drawing.Size(255, 104);
            this.CONNECT_GRP_FEN.TabIndex = 1;
            this.CONNECT_GRP_FEN.TabStop = false;
            // 
            // CONNECT_EDT_FEN_PORT
            // 
            this.CONNECT_EDT_FEN_PORT.AllowSpace = false;
            this.CONNECT_EDT_FEN_PORT.Enabled = false;
            this.CONNECT_EDT_FEN_PORT.Location = new System.Drawing.Point(85, 46);
            this.CONNECT_EDT_FEN_PORT.MaxLength = 8;
            this.CONNECT_EDT_FEN_PORT.Name = "CONNECT_EDT_FEN_PORT";
            this.CONNECT_EDT_FEN_PORT.Size = new System.Drawing.Size(60, 20);
            this.CONNECT_EDT_FEN_PORT.TabIndex = 2;
            // 
            // CONNECT_BTN_CONNECT
            // 
            this.CONNECT_BTN_CONNECT.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.CONNECT_BTN_CONNECT.Location = new System.Drawing.Point(116, 182);
            this.CONNECT_BTN_CONNECT.Name = "CONNECT_BTN_CONNECT";
            this.CONNECT_BTN_CONNECT.Size = new System.Drawing.Size(63, 25);
            this.CONNECT_BTN_CONNECT.TabIndex = 4;
            this.CONNECT_BTN_CONNECT.Text = "Connect";
            this.CONNECT_BTN_CONNECT.UseVisualStyleBackColor = true;
            this.CONNECT_BTN_CONNECT.Click += new System.EventHandler(this.on_btn_connect);
            // 
            // CONNECT_BTN_CANCEL
            // 
            this.CONNECT_BTN_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CONNECT_BTN_CANCEL.Location = new System.Drawing.Point(184, 182);
            this.CONNECT_BTN_CANCEL.Name = "CONNECT_BTN_CANCEL";
            this.CONNECT_BTN_CANCEL.Size = new System.Drawing.Size(64, 25);
            this.CONNECT_BTN_CANCEL.TabIndex = 5;
            this.CONNECT_BTN_CANCEL.Text = "Cancel";
            this.CONNECT_BTN_CANCEL.UseVisualStyleBackColor = true;
            this.CONNECT_BTN_CANCEL.Click += new System.EventHandler(this.on_btn_cancel);
            // 
            // form_connect
            // 
            this.AcceptButton = this.CONNECT_BTN_CONNECT;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CONNECT_BTN_CANCEL;
            this.ClientSize = new System.Drawing.Size(536, 262);
            this.Controls.Add(this.CONNECT_GRP_SITE);
            this.Controls.Add(this.CONNECT_GRP_CHECK);
            this.Controls.Add(this.CONNECT_GRP_FEN);
            this.Controls.Add(this.CONNECT_BTN_CANCEL);
            this.Controls.Add(this.CONNECT_BTN_CONNECT);
            this.Controls.Add(this.CONNECT_STC_ID);
            this.Controls.Add(this.CONNECT_STC_PASSWORD);
            this.Controls.Add(this.CONNECT_EDT_ID);
            this.Controls.Add(this.CONNECT_EDT_PASSWORD);
            this.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "form_connect";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Connect";
            this.Load += new System.EventHandler(this.on_load);
            this.CONNECT_GRP_SITE.ResumeLayout(false);
            this.CONNECT_GRP_SITE.PerformLayout();
            this.CONNECT_GRP_CHECK.ResumeLayout(false);
            this.CONNECT_GRP_CHECK.PerformLayout();
            this.CONNECT_GRP_FEN.ResumeLayout(false);
            this.CONNECT_GRP_FEN.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.Label CONNECT_STC_ADDRESS;
        private System.Windows.Forms.Label CONNECT_STC_PORT;
        private System.Windows.Forms.Label CONNECT_STC_PORT_REMOTE_SETUP;
        private System.Windows.Forms.Label CONNECT_STC_PORT_REMOTE_SETUP_HELP;
        private System.Windows.Forms.Label CONNECT_STC_PORT_CHECK;
        private System.Windows.Forms.Label CONNECT_STC_PORT_CHECK_HELP;
        private System.Windows.Forms.Label CONNECT_STC_FEN_SERVER;
        private System.Windows.Forms.Label CONNECT_STC_FEN_PORT;
        private System.Windows.Forms.Label CONNECT_STC_ID;
        private System.Windows.Forms.Label CONNECT_STC_PASSWORD;
        private System.Windows.Forms.TextBox CONNECT_EDT_ADDRESS;
        private System.Windows.Forms.TextBox CONNECT_EDT_FEN_SERVER;
        private System.Windows.Forms.TextBox CONNECT_EDT_ID;
        private System.Windows.Forms.TextBox CONNECT_EDT_PASSWORD;
        private text_box_numeric CONNECT_EDT_PORT;
        private text_box_numeric CONNECT_EDT_PORT_REMOTE_SETUP;
        private text_box_numeric CONNECT_EDT_PORT_CHECK;
        private text_box_numeric CONNECT_EDT_FEN_PORT;
        private System.Windows.Forms.CheckBox CONNECT_CHK_FEN_USE;
        private System.Windows.Forms.CheckBox CONNECT_CHK_FEN_QUERY;
        private System.Windows.Forms.CheckBox CONNECT_CHK_CHECK_PORT_USE;
        private System.Windows.Forms.CheckBox CONNECT_CHK_CHECK_PORT_UNITY;
        private System.Windows.Forms.CheckBox CONNECT_CHK_CHECK_G2_SEARCH;
        private System.Windows.Forms.CheckBox CONNECT_CHK_CHECK_IDR;
        private System.Windows.Forms.GroupBox CONNECT_GRP_SITE;
        private System.Windows.Forms.GroupBox CONNECT_GRP_CHECK;
        private System.Windows.Forms.GroupBox CONNECT_GRP_FEN;
        private System.Windows.Forms.Button CONNECT_BTN_CANCEL;
        private System.Windows.Forms.Button CONNECT_BTN_CONNECT;
    }
}
