namespace GDK_tester
{
    partial class form_network_alarm
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
            this.STC_TYPE = new System.Windows.Forms.Label();
            this.STC_ID = new System.Windows.Forms.Label();
            this.STC_LEVEL = new System.Windows.Forms.Label();
            this.STC_TIME = new System.Windows.Forms.Label();
            this.STC_MSEC = new System.Windows.Forms.Label();
            this.STC_DATA = new System.Windows.Forms.Label();
            this.EDT_TYPE = new GDK_tester.text_box_numeric();
            this.CBX_ID = new System.Windows.Forms.ComboBox();
            this.CBX_LEVEL = new System.Windows.Forms.ComboBox();
            this.DTP_TIME = new System.Windows.Forms.DateTimePicker();
            this.EDT_MSEC = new GDK_tester.text_box_numeric();
            this.EDT_DATA = new System.Windows.Forms.TextBox();
            this.GRP_ALARM_INFO = new System.Windows.Forms.GroupBox();
            this.BTN_ON = new System.Windows.Forms.Button();
            this.BTN_OFF = new System.Windows.Forms.Button();
            this.GRP_ALARM_INFO.SuspendLayout();
            this.SuspendLayout();
            // 
            // STC_TYPE
            // 
            this.STC_TYPE.AutoSize = true;
            this.STC_TYPE.Location = new System.Drawing.Point(24, 22);
            this.STC_TYPE.Name = "STC_TYPE";
            this.STC_TYPE.Size = new System.Drawing.Size(38, 13);
            this.STC_TYPE.TabIndex = 6;
            this.STC_TYPE.Text = "Type :";
            this.STC_TYPE.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // STC_ID
            // 
            this.STC_ID.AutoSize = true;
            this.STC_ID.Location = new System.Drawing.Point(37, 48);
            this.STC_ID.Name = "STC_ID";
            this.STC_ID.Size = new System.Drawing.Size(25, 13);
            this.STC_ID.TabIndex = 7;
            this.STC_ID.Text = "ID :";
            this.STC_ID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // STC_LEVEL
            // 
            this.STC_LEVEL.AutoSize = true;
            this.STC_LEVEL.Location = new System.Drawing.Point(22, 74);
            this.STC_LEVEL.Name = "STC_LEVEL";
            this.STC_LEVEL.Size = new System.Drawing.Size(39, 13);
            this.STC_LEVEL.TabIndex = 8;
            this.STC_LEVEL.Text = "Level :";
            this.STC_LEVEL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // STC_TIME
            // 
            this.STC_TIME.AutoSize = true;
            this.STC_TIME.Location = new System.Drawing.Point(26, 98);
            this.STC_TIME.Name = "STC_TIME";
            this.STC_TIME.Size = new System.Drawing.Size(36, 13);
            this.STC_TIME.TabIndex = 9;
            this.STC_TIME.Text = "Time :";
            this.STC_TIME.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // STC_MSEC
            // 
            this.STC_MSEC.AutoSize = true;
            this.STC_MSEC.Location = new System.Drawing.Point(24, 127);
            this.STC_MSEC.Name = "STC_MSEC";
            this.STC_MSEC.Size = new System.Drawing.Size(38, 13);
            this.STC_MSEC.TabIndex = 10;
            this.STC_MSEC.Text = "Msec :";
            this.STC_MSEC.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // STC_DATA
            // 
            this.STC_DATA.AutoSize = true;
            this.STC_DATA.Location = new System.Drawing.Point(26, 153);
            this.STC_DATA.Name = "STC_DATA";
            this.STC_DATA.Size = new System.Drawing.Size(37, 13);
            this.STC_DATA.TabIndex = 11;
            this.STC_DATA.Text = "Data :";
            this.STC_DATA.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // EDT_TYPE
            // 
            this.EDT_TYPE.AllowSpace = false;
            this.EDT_TYPE.Location = new System.Drawing.Point(68, 19);
            this.EDT_TYPE.MaxLength = 10;
            this.EDT_TYPE.Name = "EDT_TYPE";
            this.EDT_TYPE.Size = new System.Drawing.Size(99, 20);
            this.EDT_TYPE.TabIndex = 0;
            // 
            // CBX_ID
            // 
            this.CBX_ID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CBX_ID.FormattingEnabled = true;
            this.CBX_ID.Location = new System.Drawing.Point(68, 45);
            this.CBX_ID.Name = "CBX_ID";
            this.CBX_ID.Size = new System.Drawing.Size(99, 21);
            this.CBX_ID.TabIndex = 1;
            // 
            // CBX_LEVEL
            // 
            this.CBX_LEVEL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CBX_LEVEL.FormattingEnabled = true;
            this.CBX_LEVEL.Location = new System.Drawing.Point(68, 71);
            this.CBX_LEVEL.Name = "CBX_LEVEL";
            this.CBX_LEVEL.Size = new System.Drawing.Size(99, 21);
            this.CBX_LEVEL.TabIndex = 2;
            // 
            // DTP_TIME
            // 
            this.DTP_TIME.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.DTP_TIME.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DTP_TIME.Location = new System.Drawing.Point(68, 98);
            this.DTP_TIME.Name = "DTP_TIME";
            this.DTP_TIME.Size = new System.Drawing.Size(155, 20);
            this.DTP_TIME.TabIndex = 3;
            // 
            // EDT_MSEC
            // 
            this.EDT_MSEC.AllowSpace = false;
            this.EDT_MSEC.Location = new System.Drawing.Point(68, 124);
            this.EDT_MSEC.MaxLength = 10;
            this.EDT_MSEC.Name = "EDT_MSEC";
            this.EDT_MSEC.Size = new System.Drawing.Size(99, 20);
            this.EDT_MSEC.TabIndex = 4;
            // 
            // EDT_DATA
            // 
            this.EDT_DATA.Location = new System.Drawing.Point(68, 150);
            this.EDT_DATA.MaxLength = 128;
            this.EDT_DATA.Name = "EDT_DATA";
            this.EDT_DATA.Size = new System.Drawing.Size(155, 20);
            this.EDT_DATA.TabIndex = 5;
            // 
            // GRP_ALARM_INFO
            // 
            this.GRP_ALARM_INFO.Controls.Add(this.EDT_DATA);
            this.GRP_ALARM_INFO.Controls.Add(this.EDT_MSEC);
            this.GRP_ALARM_INFO.Controls.Add(this.DTP_TIME);
            this.GRP_ALARM_INFO.Controls.Add(this.CBX_LEVEL);
            this.GRP_ALARM_INFO.Controls.Add(this.CBX_ID);
            this.GRP_ALARM_INFO.Controls.Add(this.EDT_TYPE);
            this.GRP_ALARM_INFO.Controls.Add(this.STC_DATA);
            this.GRP_ALARM_INFO.Controls.Add(this.STC_MSEC);
            this.GRP_ALARM_INFO.Controls.Add(this.STC_TIME);
            this.GRP_ALARM_INFO.Controls.Add(this.STC_LEVEL);
            this.GRP_ALARM_INFO.Controls.Add(this.STC_ID);
            this.GRP_ALARM_INFO.Controls.Add(this.STC_TYPE);
            this.GRP_ALARM_INFO.Location = new System.Drawing.Point(10, 8);
            this.GRP_ALARM_INFO.Name = "GRP_ALARM_INFO";
            this.GRP_ALARM_INFO.Size = new System.Drawing.Size(238, 181);
            this.GRP_ALARM_INFO.TabIndex = 0;
            this.GRP_ALARM_INFO.TabStop = false;
            this.GRP_ALARM_INFO.Text = "Information";
            // 
            // BTN_ON
            // 
            this.BTN_ON.Location = new System.Drawing.Point(107, 198);
            this.BTN_ON.Name = "BTN_ON";
            this.BTN_ON.Size = new System.Drawing.Size(60, 25);
            this.BTN_ON.TabIndex = 1;
            this.BTN_ON.Text = "On";
            this.BTN_ON.UseVisualStyleBackColor = true;
            this.BTN_ON.Click += new System.EventHandler(this.on_btn_control);
            // 
            // BTN_OFF
            // 
            this.BTN_OFF.Location = new System.Drawing.Point(173, 198);
            this.BTN_OFF.Name = "BTN_OFF";
            this.BTN_OFF.Size = new System.Drawing.Size(60, 25);
            this.BTN_OFF.TabIndex = 2;
            this.BTN_OFF.Text = "Off";
            this.BTN_OFF.UseVisualStyleBackColor = true;
            this.BTN_OFF.Click += new System.EventHandler(this.on_btn_control);
            // 
            // form_network_alarm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(258, 232);
            this.Controls.Add(this.BTN_OFF);
            this.Controls.Add(this.GRP_ALARM_INFO);
            this.Controls.Add(this.BTN_ON);
            this.Font = new System.Drawing.Font("Tahoma", 8F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "form_network_alarm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Network Alarm";
            this.GRP_ALARM_INFO.ResumeLayout(false);
            this.GRP_ALARM_INFO.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label STC_TYPE;
        private System.Windows.Forms.Label STC_ID;
        private System.Windows.Forms.Label STC_LEVEL;
        private System.Windows.Forms.Label STC_TIME;
        private System.Windows.Forms.Label STC_MSEC;
        private System.Windows.Forms.Label STC_DATA;
        private System.Windows.Forms.ComboBox CBX_ID;
        private System.Windows.Forms.ComboBox CBX_LEVEL;
        private System.Windows.Forms.DateTimePicker DTP_TIME;
        private System.Windows.Forms.TextBox EDT_DATA;
        private System.Windows.Forms.GroupBox GRP_ALARM_INFO;
        private System.Windows.Forms.Button BTN_ON;
        private System.Windows.Forms.Button BTN_OFF;
        private text_box_numeric EDT_TYPE;
        private text_box_numeric EDT_MSEC;
    }
}
