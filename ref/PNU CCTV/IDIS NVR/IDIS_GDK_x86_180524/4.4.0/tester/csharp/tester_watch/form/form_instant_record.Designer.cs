namespace GDK_tester
{
    partial class form_instant_record
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
            this.STC_ACTION = new System.Windows.Forms.Label();
            this.CBX_ACTION = new System.Windows.Forms.ComboBox();
            this.SETTING_NVR_STC_PROFILE = new System.Windows.Forms.Label();
            this.SETTING_NVR_CBX_PROFILE = new System.Windows.Forms.ComboBox();
            this.SETTING_NVR_GRP = new System.Windows.Forms.GroupBox();
            this.SETTING_HANA_GRP = new System.Windows.Forms.GroupBox();
            this.SETTING_HANA_STC_RESOLUTION = new System.Windows.Forms.Label();
            this.SETTING_HANA_CBX_RESOLUTION = new System.Windows.Forms.ComboBox();
            this.SETTING_HANA_STC_QUALITY = new System.Windows.Forms.Label();
            this.SETTING_HANA_CBX_QUALITY = new System.Windows.Forms.ComboBox();
            this.SETTING_HANA_STC_IPS = new System.Windows.Forms.Label();
            this.SETTING_HANA_CBX_IPS = new System.Windows.Forms.ComboBox();
            this.SETTING_HANA_STC_PROFILE = new System.Windows.Forms.Label();
            this.SETTING_HANA_CBX_PROFILE = new System.Windows.Forms.ComboBox();
            this.STC_DURATION_PRE = new System.Windows.Forms.Label();
            this.STC_DURATION_POST = new System.Windows.Forms.Label();
            this.CBX_DURATION_PRE = new System.Windows.Forms.ComboBox();
            this.DTP_DURATION_POST = new System.Windows.Forms.DateTimePicker();
            this.TRV_CAMERAS = new System.Windows.Forms.TreeView();
            this.BTN_OK = new System.Windows.Forms.Button();
            this.BTN_CANCEL = new System.Windows.Forms.Button();
            this.SETTING_NVR_GRP.SuspendLayout();
            this.SETTING_HANA_GRP.SuspendLayout();
            this.SuspendLayout();
            // 
            // STC_ACTION
            // 
            this.STC_ACTION.AutoSize = true;
            this.STC_ACTION.Location = new System.Drawing.Point(76, 15);
            this.STC_ACTION.Name = "STC_ACTION";
            this.STC_ACTION.Size = new System.Drawing.Size(44, 13);
            this.STC_ACTION.TabIndex = 6;
            this.STC_ACTION.Text = "Action :";
            // 
            // CBX_ACTION
            // 
            this.CBX_ACTION.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CBX_ACTION.FormattingEnabled = true;
            this.CBX_ACTION.Location = new System.Drawing.Point(126, 12);
            this.CBX_ACTION.Name = "CBX_ACTION";
            this.CBX_ACTION.Size = new System.Drawing.Size(120, 21);
            this.CBX_ACTION.TabIndex = 0;
            this.CBX_ACTION.SelectedIndexChanged += new System.EventHandler(this.on_cbx_action_selected_index_changed);
            // 
            // SETTING_NVR_STC_PROFILE
            // 
            this.SETTING_NVR_STC_PROFILE.AutoSize = true;
            this.SETTING_NVR_STC_PROFILE.Location = new System.Drawing.Point(64, 22);
            this.SETTING_NVR_STC_PROFILE.Name = "SETTING_NVR_STC_PROFILE";
            this.SETTING_NVR_STC_PROFILE.Size = new System.Drawing.Size(44, 13);
            this.SETTING_NVR_STC_PROFILE.TabIndex = 1;
            this.SETTING_NVR_STC_PROFILE.Text = "Profile :";
            // 
            // SETTING_NVR_CBX_PROFILE
            // 
            this.SETTING_NVR_CBX_PROFILE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SETTING_NVR_CBX_PROFILE.FormattingEnabled = true;
            this.SETTING_NVR_CBX_PROFILE.Location = new System.Drawing.Point(114, 19);
            this.SETTING_NVR_CBX_PROFILE.Name = "SETTING_NVR_CBX_PROFILE";
            this.SETTING_NVR_CBX_PROFILE.Size = new System.Drawing.Size(120, 21);
            this.SETTING_NVR_CBX_PROFILE.TabIndex = 0;
            // 
            // SETTING_NVR_GRP
            // 
            this.SETTING_NVR_GRP.Controls.Add(this.SETTING_NVR_STC_PROFILE);
            this.SETTING_NVR_GRP.Controls.Add(this.SETTING_NVR_CBX_PROFILE);
            this.SETTING_NVR_GRP.Location = new System.Drawing.Point(12, 39);
            this.SETTING_NVR_GRP.Name = "SETTING_NVR_GRP";
            this.SETTING_NVR_GRP.Size = new System.Drawing.Size(244, 126);
            this.SETTING_NVR_GRP.TabIndex = 1;
            this.SETTING_NVR_GRP.TabStop = false;
            this.SETTING_NVR_GRP.Text = "Setting";
            // 
            // SETTING_HANA_GRP
            // 
            this.SETTING_HANA_GRP.Controls.Add(this.SETTING_HANA_STC_RESOLUTION);
            this.SETTING_HANA_GRP.Controls.Add(this.SETTING_HANA_CBX_RESOLUTION);
            this.SETTING_HANA_GRP.Controls.Add(this.SETTING_HANA_STC_QUALITY);
            this.SETTING_HANA_GRP.Controls.Add(this.SETTING_HANA_CBX_QUALITY);
            this.SETTING_HANA_GRP.Controls.Add(this.SETTING_HANA_STC_IPS);
            this.SETTING_HANA_GRP.Controls.Add(this.SETTING_HANA_CBX_IPS);
            this.SETTING_HANA_GRP.Controls.Add(this.SETTING_HANA_STC_PROFILE);
            this.SETTING_HANA_GRP.Controls.Add(this.SETTING_HANA_CBX_PROFILE);
            this.SETTING_HANA_GRP.Location = new System.Drawing.Point(12, 39);
            this.SETTING_HANA_GRP.Name = "SETTING_HANA_GRP";
            this.SETTING_HANA_GRP.Size = new System.Drawing.Size(244, 126);
            this.SETTING_HANA_GRP.TabIndex = 0;
            this.SETTING_HANA_GRP.TabStop = false;
            this.SETTING_HANA_GRP.Text = "Setting";
            // 
            // SETTING_HANA_STC_PROFILE
            // 
            this.SETTING_HANA_STC_PROFILE.AutoSize = true;
            this.SETTING_HANA_STC_PROFILE.Location = new System.Drawing.Point(64, 22);
            this.SETTING_HANA_STC_PROFILE.Name = "SETTING_HANA_STC_PROFILE";
            this.SETTING_HANA_STC_PROFILE.Size = new System.Drawing.Size(44, 13);
            this.SETTING_HANA_STC_PROFILE.TabIndex = 4;
            this.SETTING_HANA_STC_PROFILE.Text = "Profile :";
            // 
            // SETTING_HANA_STC_IPS
            // 
            this.SETTING_HANA_STC_IPS.AutoSize = true;
            this.SETTING_HANA_STC_IPS.Location = new System.Drawing.Point(79, 46);
            this.SETTING_HANA_STC_IPS.Name = "SETTING_HANA_STC_IPS";
            this.SETTING_HANA_STC_IPS.Size = new System.Drawing.Size(29, 13);
            this.SETTING_HANA_STC_IPS.TabIndex = 5;
            this.SETTING_HANA_STC_IPS.Text = "Ips :";
            // 
            // SETTING_HANA_STC_QUALITY
            // 
            this.SETTING_HANA_STC_QUALITY.AutoSize = true;
            this.SETTING_HANA_STC_QUALITY.Location = new System.Drawing.Point(60, 71);
            this.SETTING_HANA_STC_QUALITY.Name = "SETTING_HANA_STC_QUALITY";
            this.SETTING_HANA_STC_QUALITY.Size = new System.Drawing.Size(48, 13);
            this.SETTING_HANA_STC_QUALITY.TabIndex = 6;
            this.SETTING_HANA_STC_QUALITY.Text = "Quality :";
            // 
            // SETTING_HANA_STC_RESOLUTION
            // 
            this.SETTING_HANA_STC_RESOLUTION.AutoSize = true;
            this.SETTING_HANA_STC_RESOLUTION.Location = new System.Drawing.Point(44, 96);
            this.SETTING_HANA_STC_RESOLUTION.Name = "SETTING_HANA_STC_RESOLUTION";
            this.SETTING_HANA_STC_RESOLUTION.Size = new System.Drawing.Size(64, 13);
            this.SETTING_HANA_STC_RESOLUTION.TabIndex = 7;
            this.SETTING_HANA_STC_RESOLUTION.Text = "Resolution :";
            // 
            // SETTING_HANA_CBX_PROFILE
            // 
            this.SETTING_HANA_CBX_PROFILE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SETTING_HANA_CBX_PROFILE.FormattingEnabled = true;
            this.SETTING_HANA_CBX_PROFILE.Location = new System.Drawing.Point(114, 19);
            this.SETTING_HANA_CBX_PROFILE.Name = "SETTING_HANA_CBX_PROFILE";
            this.SETTING_HANA_CBX_PROFILE.Size = new System.Drawing.Size(120, 21);
            this.SETTING_HANA_CBX_PROFILE.TabIndex = 0;
            // 
            // SETTING_HANA_CBX_IPS
            // 
            this.SETTING_HANA_CBX_IPS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SETTING_HANA_CBX_IPS.FormattingEnabled = true;
            this.SETTING_HANA_CBX_IPS.Location = new System.Drawing.Point(114, 43);
            this.SETTING_HANA_CBX_IPS.Name = "SETTING_HANA_CBX_IPS";
            this.SETTING_HANA_CBX_IPS.Size = new System.Drawing.Size(120, 21);
            this.SETTING_HANA_CBX_IPS.TabIndex = 1;
            // 
            // SETTING_HANA_CBX_QUALITY
            // 
            this.SETTING_HANA_CBX_QUALITY.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SETTING_HANA_CBX_QUALITY.FormattingEnabled = true;
            this.SETTING_HANA_CBX_QUALITY.Location = new System.Drawing.Point(114, 67);
            this.SETTING_HANA_CBX_QUALITY.Name = "SETTING_HANA_CBX_QUALITY";
            this.SETTING_HANA_CBX_QUALITY.Size = new System.Drawing.Size(120, 21);
            this.SETTING_HANA_CBX_QUALITY.TabIndex = 2;
            // 
            // SETTING_HANA_CBX_RESOLUTION
            // 
            this.SETTING_HANA_CBX_RESOLUTION.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SETTING_HANA_CBX_RESOLUTION.FormattingEnabled = true;
            this.SETTING_HANA_CBX_RESOLUTION.Location = new System.Drawing.Point(114, 91);
            this.SETTING_HANA_CBX_RESOLUTION.Name = "SETTING_HANA_CBX_RESOLUTION";
            this.SETTING_HANA_CBX_RESOLUTION.Size = new System.Drawing.Size(120, 21);
            this.SETTING_HANA_CBX_RESOLUTION.TabIndex = 3;
            // 
            // STC_DURATION_PRE
            // 
            this.STC_DURATION_PRE.AutoSize = true;
            this.STC_DURATION_PRE.Location = new System.Drawing.Point(49, 179);
            this.STC_DURATION_PRE.Name = "STC_DURATION_PRE";
            this.STC_DURATION_PRE.Size = new System.Drawing.Size(71, 13);
            this.STC_DURATION_PRE.TabIndex = 3;
            this.STC_DURATION_PRE.Text = "PreDuration :";
            // 
            // STC_DURATION_POST
            // 
            this.STC_DURATION_POST.AutoSize = true;
            this.STC_DURATION_POST.Location = new System.Drawing.Point(44, 206);
            this.STC_DURATION_POST.Name = "STC_DURATION_POST";
            this.STC_DURATION_POST.Size = new System.Drawing.Size(76, 13);
            this.STC_DURATION_POST.TabIndex = 4;
            this.STC_DURATION_POST.Text = "PostDuration :";
            // 
            // CBX_DURATION_PRE
            // 
            this.CBX_DURATION_PRE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CBX_DURATION_PRE.FormattingEnabled = true;
            this.CBX_DURATION_PRE.Location = new System.Drawing.Point(126, 176);
            this.CBX_DURATION_PRE.Name = "CBX_DURATION_PRE";
            this.CBX_DURATION_PRE.Size = new System.Drawing.Size(120, 21);
            this.CBX_DURATION_PRE.TabIndex = 2;
            // 
            // DTP_DURATION_POST
            // 
            this.DTP_DURATION_POST.CustomFormat = " HH:mm:ss";
            this.DTP_DURATION_POST.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DTP_DURATION_POST.Location = new System.Drawing.Point(126, 204);
            this.DTP_DURATION_POST.Name = "DTP_DURATION_POST";
            this.DTP_DURATION_POST.ShowUpDown = true;
            this.DTP_DURATION_POST.Size = new System.Drawing.Size(75, 20);
            this.DTP_DURATION_POST.TabIndex = 3;
            this.DTP_DURATION_POST.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.on_dtp_duration_post_mouse_wheel);
            // 
            // TRV_CAMERAS
            // 
            this.TRV_CAMERAS.CheckBoxes = true;
            this.TRV_CAMERAS.Location = new System.Drawing.Point(12, 235);
            this.TRV_CAMERAS.Name = "TRV_CAMERAS";
            this.TRV_CAMERAS.Size = new System.Drawing.Size(244, 150);
            this.TRV_CAMERAS.TabIndex = 7;
            // 
            // BTN_OK
            // 
            this.BTN_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BTN_OK.Location = new System.Drawing.Point(100, 394);
            this.BTN_OK.Name = "BTN_OK";
            this.BTN_OK.Size = new System.Drawing.Size(70, 23);
            this.BTN_OK.TabIndex = 5;
            this.BTN_OK.Text = "OK";
            this.BTN_OK.UseVisualStyleBackColor = true;
            this.BTN_OK.Click += new System.EventHandler(this.on_btn_ok);
            // 
            // BTN_CANCEL
            // 
            this.BTN_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BTN_CANCEL.Location = new System.Drawing.Point(176, 394);
            this.BTN_CANCEL.Name = "BTN_CANCEL";
            this.BTN_CANCEL.Size = new System.Drawing.Size(70, 23);
            this.BTN_CANCEL.TabIndex = 6;
            this.BTN_CANCEL.Text = "Cancel";
            this.BTN_CANCEL.UseVisualStyleBackColor = true;
            // 
            // form_instant_record
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(266, 426);
            this.Controls.Add(this.SETTING_HANA_GRP);
            this.Controls.Add(this.BTN_CANCEL);
            this.Controls.Add(this.BTN_OK);
            this.Controls.Add(this.TRV_CAMERAS);
            this.Controls.Add(this.SETTING_NVR_GRP);
            this.Controls.Add(this.DTP_DURATION_POST);
            this.Controls.Add(this.CBX_DURATION_PRE);
            this.Controls.Add(this.CBX_ACTION);
            this.Controls.Add(this.STC_DURATION_POST);
            this.Controls.Add(this.STC_DURATION_PRE);
            this.Controls.Add(this.STC_ACTION);
            this.Font = new System.Drawing.Font("Tahoma", 8F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "form_instant_record";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Instant Recording";
            this.SETTING_NVR_GRP.ResumeLayout(false);
            this.SETTING_NVR_GRP.PerformLayout();
            this.SETTING_HANA_GRP.ResumeLayout(false);
            this.SETTING_HANA_GRP.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label STC_ACTION;
        private System.Windows.Forms.ComboBox CBX_ACTION;
        private System.Windows.Forms.Label SETTING_NVR_STC_PROFILE;
        private System.Windows.Forms.ComboBox SETTING_NVR_CBX_PROFILE;
        private System.Windows.Forms.GroupBox SETTING_NVR_GRP;
        private System.Windows.Forms.Label STC_DURATION_PRE;
        private System.Windows.Forms.Label STC_DURATION_POST;
        private System.Windows.Forms.ComboBox CBX_DURATION_PRE;
        private System.Windows.Forms.DateTimePicker DTP_DURATION_POST;
        private System.Windows.Forms.TreeView TRV_CAMERAS;
        private System.Windows.Forms.Button BTN_OK;
        private System.Windows.Forms.Button BTN_CANCEL;
        private System.Windows.Forms.GroupBox SETTING_HANA_GRP;
        private System.Windows.Forms.Label SETTING_HANA_STC_PROFILE;
        private System.Windows.Forms.Label SETTING_HANA_STC_IPS;
        private System.Windows.Forms.Label SETTING_HANA_STC_QUALITY;
        private System.Windows.Forms.Label SETTING_HANA_STC_RESOLUTION;
        private System.Windows.Forms.ComboBox SETTING_HANA_CBX_PROFILE;
        private System.Windows.Forms.ComboBox SETTING_HANA_CBX_IPS;
        private System.Windows.Forms.ComboBox SETTING_HANA_CBX_QUALITY;
        private System.Windows.Forms.ComboBox SETTING_HANA_CBX_RESOLUTION;
    }
}
