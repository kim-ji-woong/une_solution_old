namespace GDK_tester
{
    partial class form_server
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(form_server));
            this.STC_SCREEN = new System.Windows.Forms.Label();
            this.DEVICE1_EDT_SERVER = new System.Windows.Forms.TextBox();
            this.DEVICE1_EDT_PORT = new System.Windows.Forms.TextBox();
            this.DEVICE1_CHK_UNITY_PORT = new System.Windows.Forms.CheckBox();
            this.DEVICE1_CBX_CHANNEL = new System.Windows.Forms.ComboBox();
            this.DEVICE1_STC_NAME = new System.Windows.Forms.Label();
            this.DEVICE1_BTN_RUN = new System.Windows.Forms.Button();
            this.DEVICE1_EDT_ID = new System.Windows.Forms.TextBox();
            this.DEVICE1_EDT_PASSWORD = new System.Windows.Forms.TextBox();
            this.DEVICE1_STC_USER = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // STC_SCREEN
            // 
            this.STC_SCREEN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.STC_SCREEN.Location = new System.Drawing.Point(10, 10);
            this.STC_SCREEN.Name = "STC_SCREEN";
            this.STC_SCREEN.Size = new System.Drawing.Size(640, 360);
            this.STC_SCREEN.TabIndex = 0;
            this.STC_SCREEN.Text = "screen";
            this.STC_SCREEN.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.STC_SCREEN.Visible = false;
            // 
            // DEVICE1_EDT_SERVER
            // 
            this.DEVICE1_EDT_SERVER.Location = new System.Drawing.Point(70, 390);
            this.DEVICE1_EDT_SERVER.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.DEVICE1_EDT_SERVER.Name = "DEVICE1_EDT_SERVER";
            this.DEVICE1_EDT_SERVER.Size = new System.Drawing.Size(90, 20);
            this.DEVICE1_EDT_SERVER.TabIndex = 1;
            // 
            // DEVICE1_EDT_PORT
            // 
            this.DEVICE1_EDT_PORT.Font = new System.Drawing.Font("Tahoma", 8F);
            this.DEVICE1_EDT_PORT.Location = new System.Drawing.Point(164, 390);
            this.DEVICE1_EDT_PORT.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.DEVICE1_EDT_PORT.Name = "DEVICE1_EDT_PORT";
            this.DEVICE1_EDT_PORT.Size = new System.Drawing.Size(50, 20);
            this.DEVICE1_EDT_PORT.TabIndex = 2;
            // 
            // DEVICE1_CHK_UNITY_PORT
            // 
            this.DEVICE1_CHK_UNITY_PORT.AutoSize = true;
            this.DEVICE1_CHK_UNITY_PORT.Location = new System.Drawing.Point(220, 392);
            this.DEVICE1_CHK_UNITY_PORT.Name = "DEVICE1_CHK_UNITY_PORT";
            this.DEVICE1_CHK_UNITY_PORT.Size = new System.Drawing.Size(74, 17);
            this.DEVICE1_CHK_UNITY_PORT.TabIndex = 8;
            this.DEVICE1_CHK_UNITY_PORT.Text = "Unity Port";
            this.DEVICE1_CHK_UNITY_PORT.UseVisualStyleBackColor = true;
            // 
            // DEVICE1_CBX_CHANNEL
            // 
            this.DEVICE1_CBX_CHANNEL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DEVICE1_CBX_CHANNEL.FormattingEnabled = true;
            this.DEVICE1_CBX_CHANNEL.Location = new System.Drawing.Point(300, 389);
            this.DEVICE1_CBX_CHANNEL.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.DEVICE1_CBX_CHANNEL.Name = "DEVICE1_CBX_CHANNEL";
            this.DEVICE1_CBX_CHANNEL.Size = new System.Drawing.Size(50, 21);
            this.DEVICE1_CBX_CHANNEL.TabIndex = 3;
            // 
            // DEVICE1_STC_NAME
            // 
            this.DEVICE1_STC_NAME.AutoSize = true;
            this.DEVICE1_STC_NAME.Location = new System.Drawing.Point(12, 393);
            this.DEVICE1_STC_NAME.Name = "DEVICE1_STC_NAME";
            this.DEVICE1_STC_NAME.Size = new System.Drawing.Size(55, 13);
            this.DEVICE1_STC_NAME.TabIndex = 7;
            this.DEVICE1_STC_NAME.Text = "Device 1 :";
            this.DEVICE1_STC_NAME.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DEVICE1_BTN_RUN
            // 
            this.DEVICE1_BTN_RUN.Location = new System.Drawing.Point(570, 388);
            this.DEVICE1_BTN_RUN.Name = "DEVICE1_BTN_RUN";
            this.DEVICE1_BTN_RUN.Size = new System.Drawing.Size(80, 23);
            this.DEVICE1_BTN_RUN.TabIndex = 6;
            this.DEVICE1_BTN_RUN.Text = "Run";
            this.DEVICE1_BTN_RUN.UseVisualStyleBackColor = true;
            this.DEVICE1_BTN_RUN.Click += new System.EventHandler(this.on_btn_device1_run);
            // 
            // DEVICE1_EDT_ID
            // 
            this.DEVICE1_EDT_ID.Location = new System.Drawing.Point(392, 390);
            this.DEVICE1_EDT_ID.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.DEVICE1_EDT_ID.Name = "DEVICE1_EDT_ID";
            this.DEVICE1_EDT_ID.Size = new System.Drawing.Size(80, 20);
            this.DEVICE1_EDT_ID.TabIndex = 4;
            // 
            // DEVICE1_EDT_PASSWORD
            // 
            this.DEVICE1_EDT_PASSWORD.Location = new System.Drawing.Point(476, 390);
            this.DEVICE1_EDT_PASSWORD.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.DEVICE1_EDT_PASSWORD.Name = "DEVICE1_EDT_PASSWORD";
            this.DEVICE1_EDT_PASSWORD.PasswordChar = '*';
            this.DEVICE1_EDT_PASSWORD.Size = new System.Drawing.Size(80, 20);
            this.DEVICE1_EDT_PASSWORD.TabIndex = 5;
            // 
            // DEVICE1_STC_USER
            // 
            this.DEVICE1_STC_USER.AutoSize = true;
            this.DEVICE1_STC_USER.Location = new System.Drawing.Point(364, 393);
            this.DEVICE1_STC_USER.Name = "DEVICE1_STC_USER";
            this.DEVICE1_STC_USER.Size = new System.Drawing.Size(25, 13);
            this.DEVICE1_STC_USER.TabIndex = 9;
            this.DEVICE1_STC_USER.Text = "ID :";
            this.DEVICE1_STC_USER.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // form_server
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 426);
            this.Controls.Add(this.STC_SCREEN);
            this.Controls.Add(this.DEVICE1_CHK_UNITY_PORT);
            this.Controls.Add(this.DEVICE1_STC_USER);
            this.Controls.Add(this.DEVICE1_EDT_PASSWORD);
            this.Controls.Add(this.DEVICE1_EDT_ID);
            this.Controls.Add(this.DEVICE1_BTN_RUN);
            this.Controls.Add(this.DEVICE1_STC_NAME);
            this.Controls.Add(this.DEVICE1_CBX_CHANNEL);
            this.Controls.Add(this.DEVICE1_EDT_PORT);
            this.Controls.Add(this.DEVICE1_EDT_SERVER);
            this.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "form_server";
            this.Text = "GDK G2Frame R/e/l/a/y Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.on_form_closing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private screen_pane SCREEN_PANE;
        private System.Windows.Forms.Label STC_SCREEN;
        private System.Windows.Forms.TextBox DEVICE1_EDT_SERVER;
        private System.Windows.Forms.TextBox DEVICE1_EDT_PORT;
        private System.Windows.Forms.CheckBox DEVICE1_CHK_UNITY_PORT;
        private System.Windows.Forms.ComboBox DEVICE1_CBX_CHANNEL;
        private System.Windows.Forms.Label DEVICE1_STC_NAME;
        private System.Windows.Forms.Button DEVICE1_BTN_RUN;
        private System.Windows.Forms.TextBox DEVICE1_EDT_ID;
        private System.Windows.Forms.TextBox DEVICE1_EDT_PASSWORD;
        private System.Windows.Forms.Label DEVICE1_STC_USER;
    }
}
