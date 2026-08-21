namespace GDK_tester
{
    partial class form_event_search
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
            this.STC_FROM = new System.Windows.Forms.Label();
            this.STC_TO = new System.Windows.Forms.Label();
            this.CHK_FIRST = new System.Windows.Forms.CheckBox();
            this.CHK_LAST = new System.Windows.Forms.CheckBox();
            this.DTP_FROM = new System.Windows.Forms.DateTimePicker();
            this.DTP_TO = new System.Windows.Forms.DateTimePicker();
            this.TRV_DEVICES = new System.Windows.Forms.TreeView();
            this.BTN_OK = new System.Windows.Forms.Button();
            this.BTN_CANCEL = new System.Windows.Forms.Button();
            this.PANEL_EVENT_SERVICE_SITE = new System.Windows.Forms.Panel();
            this.CB_FACE_DETECTION = new System.Windows.Forms.CheckBox();
            this.CB_DEVICE_DISCONNECTION = new System.Windows.Forms.CheckBox();
            this.CB_AUDIO_DETECTION = new System.Windows.Forms.CheckBox();
            this.CB_VIDEO_ANALYTICS = new System.Windows.Forms.CheckBox();
            this.CB_TRIPZONE = new System.Windows.Forms.CheckBox();
            this.CB_VIDEO_LOSS = new System.Windows.Forms.CheckBox();
            this.CB_USER_ALARAM_IN = new System.Windows.Forms.CheckBox();
            this.CB_DEVICE_CONNECTION = new System.Windows.Forms.CheckBox();
            this.CB_TEXT_IN = new System.Windows.Forms.CheckBox();
            this.CB_TAMPERING = new System.Windows.Forms.CheckBox();
            this.CB_VIDEO_BLIND = new System.Windows.Forms.CheckBox();
            this.CB_MOTION_DETECTION = new System.Windows.Forms.CheckBox();
            this.CB_ALARM_IN = new System.Windows.Forms.CheckBox();
            this.PANEL_EVENT_DVR_SITE = new System.Windows.Forms.Panel();
            this.CB_CONSECUTIVE_INVALID_PASSWORD = new System.Windows.Forms.CheckBox();
            this.CB_FAN_ERROR_DVR = new System.Windows.Forms.CheckBox();
            this.CB_ALARM_IN_BAD = new System.Windows.Forms.CheckBox();
            this.CB_DISK_BAD = new System.Windows.Forms.CheckBox();
            this.CB_DISK_TEMPERATURE = new System.Windows.Forms.CheckBox();
            this.CB_DISK_SMART = new System.Windows.Forms.CheckBox();
            this.CB_DISK_ALMOST_FULL = new System.Windows.Forms.CheckBox();
            this.CB_DISK_DELETED = new System.Windows.Forms.CheckBox();
            this.CB_DISK_CONFIG_CHANGE = new System.Windows.Forms.CheckBox();
            this.CB_PANIC_RECORD = new System.Windows.Forms.CheckBox();
            this.CB_RECORD_BAD = new System.Windows.Forms.CheckBox();
            this.CB_ALARM_IN_DVR = new System.Windows.Forms.CheckBox();
            this.CB_VIDEO_LOSS_DVR = new System.Windows.Forms.CheckBox();
            this.CB_FAN_ERROR = new System.Windows.Forms.CheckBox();
            this.CB_FACE_DETECTION_DVR = new System.Windows.Forms.CheckBox();
            this.CB_TRIPZONE_DVR = new System.Windows.Forms.CheckBox();
            this.CB_NETWORK_ALARM_IN_DVR = new System.Windows.Forms.CheckBox();
            this.CB_AUDIO_DETECTION_DVR = new System.Windows.Forms.CheckBox();
            this.CB_MOTION_DETECTION_DVR = new System.Windows.Forms.CheckBox();
            this.CB_TAMPERING_DVR = new System.Windows.Forms.CheckBox();
            this.CB_RECORDING_FAIL_DVR = new System.Windows.Forms.CheckBox();
            this.CB_TEXT_IN_DVR = new System.Windows.Forms.CheckBox();
            this.PANEL_EVENT_SERVICE_SITE.SuspendLayout();
            this.PANEL_EVENT_DVR_SITE.SuspendLayout();
            this.SuspendLayout();
            // 
            // STC_FROM
            // 
            this.STC_FROM.Location = new System.Drawing.Point(12, 11);
            this.STC_FROM.Name = "STC_FROM";
            this.STC_FROM.Size = new System.Drawing.Size(55, 17);
            this.STC_FROM.TabIndex = 16;
            this.STC_FROM.Text = "From :";
            this.STC_FROM.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // STC_TO
            // 
            this.STC_TO.Location = new System.Drawing.Point(12, 34);
            this.STC_TO.Name = "STC_TO";
            this.STC_TO.Size = new System.Drawing.Size(55, 17);
            this.STC_TO.TabIndex = 17;
            this.STC_TO.Text = "To :";
            this.STC_TO.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CHK_FIRST
            // 
            this.CHK_FIRST.AutoSize = true;
            this.CHK_FIRST.Checked = true;
            this.CHK_FIRST.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHK_FIRST.Location = new System.Drawing.Point(73, 12);
            this.CHK_FIRST.Name = "CHK_FIRST";
            this.CHK_FIRST.Size = new System.Drawing.Size(47, 17);
            this.CHK_FIRST.TabIndex = 0;
            this.CHK_FIRST.Text = "First";
            this.CHK_FIRST.UseVisualStyleBackColor = true;
            this.CHK_FIRST.CheckedChanged += new System.EventHandler(this.on_chk_first);
            // 
            // CHK_LAST
            // 
            this.CHK_LAST.AutoSize = true;
            this.CHK_LAST.Checked = true;
            this.CHK_LAST.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHK_LAST.Location = new System.Drawing.Point(73, 35);
            this.CHK_LAST.Name = "CHK_LAST";
            this.CHK_LAST.Size = new System.Drawing.Size(46, 17);
            this.CHK_LAST.TabIndex = 1;
            this.CHK_LAST.Text = "Last";
            this.CHK_LAST.UseVisualStyleBackColor = true;
            this.CHK_LAST.CheckedChanged += new System.EventHandler(this.on_chk_last);
            // 
            // DTP_FROM
            // 
            this.DTP_FROM.CustomFormat = " yyyy-MM-dd HH:mm:ss";
            this.DTP_FROM.Enabled = false;
            this.DTP_FROM.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DTP_FROM.Location = new System.Drawing.Point(126, 9);
            this.DTP_FROM.Name = "DTP_FROM";
            this.DTP_FROM.Size = new System.Drawing.Size(148, 20);
            this.DTP_FROM.TabIndex = 2;
            this.DTP_FROM.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.on_dtp_mouse_wheel);
            // 
            // DTP_TO
            // 
            this.DTP_TO.CustomFormat = " yyyy-MM-dd HH:mm:ss";
            this.DTP_TO.Enabled = false;
            this.DTP_TO.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DTP_TO.Location = new System.Drawing.Point(125, 35);
            this.DTP_TO.Name = "DTP_TO";
            this.DTP_TO.Size = new System.Drawing.Size(149, 20);
            this.DTP_TO.TabIndex = 3;
            this.DTP_TO.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.on_dtp_mouse_wheel);
            // 
            // TRV_DEVICES
            // 
            this.TRV_DEVICES.CheckBoxes = true;
            this.TRV_DEVICES.Location = new System.Drawing.Point(12, 61);
            this.TRV_DEVICES.Name = "TRV_DEVICES";
            this.TRV_DEVICES.Size = new System.Drawing.Size(390, 200);
            this.TRV_DEVICES.TabIndex = 4;
            // 
            // BTN_OK
            // 
            this.BTN_OK.Location = new System.Drawing.Point(246, 572);
            this.BTN_OK.Name = "BTN_OK";
            this.BTN_OK.Size = new System.Drawing.Size(75, 23);
            this.BTN_OK.TabIndex = 5;
            this.BTN_OK.Text = "OK";
            this.BTN_OK.UseVisualStyleBackColor = true;
            this.BTN_OK.Click += new System.EventHandler(this.on_btn_OK);
            // 
            // BTN_CANCEL
            // 
            this.BTN_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BTN_CANCEL.Location = new System.Drawing.Point(327, 572);
            this.BTN_CANCEL.Name = "BTN_CANCEL";
            this.BTN_CANCEL.Size = new System.Drawing.Size(75, 23);
            this.BTN_CANCEL.TabIndex = 6;
            this.BTN_CANCEL.Text = "Cancel";
            this.BTN_CANCEL.UseVisualStyleBackColor = true;
            // 
            // PANEL_EVENT_SERVICE_SITE
            // 
            this.PANEL_EVENT_SERVICE_SITE.Controls.Add(this.CB_FACE_DETECTION);
            this.PANEL_EVENT_SERVICE_SITE.Controls.Add(this.CB_DEVICE_DISCONNECTION);
            this.PANEL_EVENT_SERVICE_SITE.Controls.Add(this.CB_AUDIO_DETECTION);
            this.PANEL_EVENT_SERVICE_SITE.Controls.Add(this.CB_VIDEO_ANALYTICS);
            this.PANEL_EVENT_SERVICE_SITE.Controls.Add(this.CB_TRIPZONE);
            this.PANEL_EVENT_SERVICE_SITE.Controls.Add(this.CB_VIDEO_LOSS);
            this.PANEL_EVENT_SERVICE_SITE.Controls.Add(this.CB_USER_ALARAM_IN);
            this.PANEL_EVENT_SERVICE_SITE.Controls.Add(this.CB_DEVICE_CONNECTION);
            this.PANEL_EVENT_SERVICE_SITE.Controls.Add(this.CB_TEXT_IN);
            this.PANEL_EVENT_SERVICE_SITE.Controls.Add(this.CB_TAMPERING);
            this.PANEL_EVENT_SERVICE_SITE.Controls.Add(this.CB_VIDEO_BLIND);
            this.PANEL_EVENT_SERVICE_SITE.Controls.Add(this.CB_MOTION_DETECTION);
            this.PANEL_EVENT_SERVICE_SITE.Controls.Add(this.CB_ALARM_IN);
            this.PANEL_EVENT_SERVICE_SITE.Location = new System.Drawing.Point(12, 267);
            this.PANEL_EVENT_SERVICE_SITE.Name = "PANEL_EVENT_SERVICE_SITE";
            this.PANEL_EVENT_SERVICE_SITE.Size = new System.Drawing.Size(390, 164);
            this.PANEL_EVENT_SERVICE_SITE.TabIndex = 31;
            this.PANEL_EVENT_SERVICE_SITE.Visible = false;
            // 
            // CB_FACE_DETECTION
            // 
            this.CB_FACE_DETECTION.AutoSize = true;
            this.CB_FACE_DETECTION.Checked = true;
            this.CB_FACE_DETECTION.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_FACE_DETECTION.Location = new System.Drawing.Point(8, 141);
            this.CB_FACE_DETECTION.Name = "CB_FACE_DETECTION";
            this.CB_FACE_DETECTION.Size = new System.Drawing.Size(98, 17);
            this.CB_FACE_DETECTION.TabIndex = 43;
            this.CB_FACE_DETECTION.Text = "Face Detection";
            this.CB_FACE_DETECTION.UseVisualStyleBackColor = true;
            // 
            // CB_DEVICE_DISCONNECTION
            // 
            this.CB_DEVICE_DISCONNECTION.AutoSize = true;
            this.CB_DEVICE_DISCONNECTION.Checked = true;
            this.CB_DEVICE_DISCONNECTION.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_DEVICE_DISCONNECTION.Location = new System.Drawing.Point(178, 118);
            this.CB_DEVICE_DISCONNECTION.Name = "CB_DEVICE_DISCONNECTION";
            this.CB_DEVICE_DISCONNECTION.Size = new System.Drawing.Size(127, 17);
            this.CB_DEVICE_DISCONNECTION.TabIndex = 42;
            this.CB_DEVICE_DISCONNECTION.Text = "Device Disconnection";
            this.CB_DEVICE_DISCONNECTION.UseVisualStyleBackColor = true;
            // 
            // CB_AUDIO_DETECTION
            // 
            this.CB_AUDIO_DETECTION.AutoSize = true;
            this.CB_AUDIO_DETECTION.Checked = true;
            this.CB_AUDIO_DETECTION.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_AUDIO_DETECTION.Location = new System.Drawing.Point(178, 95);
            this.CB_AUDIO_DETECTION.Name = "CB_AUDIO_DETECTION";
            this.CB_AUDIO_DETECTION.Size = new System.Drawing.Size(102, 17);
            this.CB_AUDIO_DETECTION.TabIndex = 41;
            this.CB_AUDIO_DETECTION.Text = "Audio Detection";
            this.CB_AUDIO_DETECTION.UseVisualStyleBackColor = true;
            // 
            // CB_VIDEO_ANALYTICS
            // 
            this.CB_VIDEO_ANALYTICS.AutoSize = true;
            this.CB_VIDEO_ANALYTICS.Checked = true;
            this.CB_VIDEO_ANALYTICS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_VIDEO_ANALYTICS.Location = new System.Drawing.Point(178, 72);
            this.CB_VIDEO_ANALYTICS.Name = "CB_VIDEO_ANALYTICS";
            this.CB_VIDEO_ANALYTICS.Size = new System.Drawing.Size(98, 17);
            this.CB_VIDEO_ANALYTICS.TabIndex = 40;
            this.CB_VIDEO_ANALYTICS.Text = "Video Analytics";
            this.CB_VIDEO_ANALYTICS.UseVisualStyleBackColor = true;
            // 
            // CB_TRIPZONE
            // 
            this.CB_TRIPZONE.AutoSize = true;
            this.CB_TRIPZONE.Checked = true;
            this.CB_TRIPZONE.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_TRIPZONE.Location = new System.Drawing.Point(178, 49);
            this.CB_TRIPZONE.Name = "CB_TRIPZONE";
            this.CB_TRIPZONE.Size = new System.Drawing.Size(68, 17);
            this.CB_TRIPZONE.TabIndex = 39;
            this.CB_TRIPZONE.Text = "TripZone";
            this.CB_TRIPZONE.UseVisualStyleBackColor = true;
            // 
            // CB_VIDEO_LOSS
            // 
            this.CB_VIDEO_LOSS.AutoSize = true;
            this.CB_VIDEO_LOSS.Checked = true;
            this.CB_VIDEO_LOSS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_VIDEO_LOSS.Location = new System.Drawing.Point(178, 26);
            this.CB_VIDEO_LOSS.Name = "CB_VIDEO_LOSS";
            this.CB_VIDEO_LOSS.Size = new System.Drawing.Size(76, 17);
            this.CB_VIDEO_LOSS.TabIndex = 38;
            this.CB_VIDEO_LOSS.Text = "Video Loss";
            this.CB_VIDEO_LOSS.UseVisualStyleBackColor = true;
            // 
            // CB_USER_ALARAM_IN
            // 
            this.CB_USER_ALARAM_IN.AutoSize = true;
            this.CB_USER_ALARAM_IN.Checked = true;
            this.CB_USER_ALARAM_IN.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_USER_ALARAM_IN.Location = new System.Drawing.Point(178, 3);
            this.CB_USER_ALARAM_IN.Name = "CB_USER_ALARAM_IN";
            this.CB_USER_ALARAM_IN.Size = new System.Drawing.Size(132, 17);
            this.CB_USER_ALARAM_IN.TabIndex = 37;
            this.CB_USER_ALARAM_IN.Text = "User Defined Alarm-In";
            this.CB_USER_ALARAM_IN.UseVisualStyleBackColor = true;
            // 
            // CB_DEVICE_CONNECTION
            // 
            this.CB_DEVICE_CONNECTION.AutoSize = true;
            this.CB_DEVICE_CONNECTION.Checked = true;
            this.CB_DEVICE_CONNECTION.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_DEVICE_CONNECTION.Location = new System.Drawing.Point(8, 118);
            this.CB_DEVICE_CONNECTION.Name = "CB_DEVICE_CONNECTION";
            this.CB_DEVICE_CONNECTION.Size = new System.Drawing.Size(115, 17);
            this.CB_DEVICE_CONNECTION.TabIndex = 36;
            this.CB_DEVICE_CONNECTION.Text = "Device Connection";
            this.CB_DEVICE_CONNECTION.UseVisualStyleBackColor = true;
            // 
            // CB_TEXT_IN
            // 
            this.CB_TEXT_IN.AutoSize = true;
            this.CB_TEXT_IN.Checked = true;
            this.CB_TEXT_IN.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_TEXT_IN.Location = new System.Drawing.Point(8, 95);
            this.CB_TEXT_IN.Name = "CB_TEXT_IN";
            this.CB_TEXT_IN.Size = new System.Drawing.Size(62, 17);
            this.CB_TEXT_IN.TabIndex = 35;
            this.CB_TEXT_IN.Text = "Text-In";
            this.CB_TEXT_IN.UseVisualStyleBackColor = true;
            // 
            // CB_TAMPERING
            // 
            this.CB_TAMPERING.AutoSize = true;
            this.CB_TAMPERING.Checked = true;
            this.CB_TAMPERING.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_TAMPERING.Location = new System.Drawing.Point(8, 72);
            this.CB_TAMPERING.Name = "CB_TAMPERING";
            this.CB_TAMPERING.Size = new System.Drawing.Size(76, 17);
            this.CB_TAMPERING.TabIndex = 34;
            this.CB_TAMPERING.Text = "Tampering";
            this.CB_TAMPERING.UseVisualStyleBackColor = true;
            // 
            // CB_VIDEO_BLIND
            // 
            this.CB_VIDEO_BLIND.AutoSize = true;
            this.CB_VIDEO_BLIND.Checked = true;
            this.CB_VIDEO_BLIND.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_VIDEO_BLIND.Location = new System.Drawing.Point(8, 49);
            this.CB_VIDEO_BLIND.Name = "CB_VIDEO_BLIND";
            this.CB_VIDEO_BLIND.Size = new System.Drawing.Size(77, 17);
            this.CB_VIDEO_BLIND.TabIndex = 33;
            this.CB_VIDEO_BLIND.Text = "Video Blind";
            this.CB_VIDEO_BLIND.UseVisualStyleBackColor = true;
            // 
            // CB_MOTION_DETECTION
            // 
            this.CB_MOTION_DETECTION.AutoSize = true;
            this.CB_MOTION_DETECTION.Checked = true;
            this.CB_MOTION_DETECTION.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_MOTION_DETECTION.Location = new System.Drawing.Point(8, 26);
            this.CB_MOTION_DETECTION.Name = "CB_MOTION_DETECTION";
            this.CB_MOTION_DETECTION.Size = new System.Drawing.Size(107, 17);
            this.CB_MOTION_DETECTION.TabIndex = 32;
            this.CB_MOTION_DETECTION.Text = "Motion Detection";
            this.CB_MOTION_DETECTION.UseVisualStyleBackColor = true;
            // 
            // CB_ALARM_IN
            // 
            this.CB_ALARM_IN.AutoSize = true;
            this.CB_ALARM_IN.Checked = true;
            this.CB_ALARM_IN.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_ALARM_IN.Location = new System.Drawing.Point(8, 3);
            this.CB_ALARM_IN.Name = "CB_ALARM_IN";
            this.CB_ALARM_IN.Size = new System.Drawing.Size(67, 17);
            this.CB_ALARM_IN.TabIndex = 31;
            this.CB_ALARM_IN.Text = "Alarm-In";
            this.CB_ALARM_IN.UseVisualStyleBackColor = true;
            // 
            // PANEL_EVENT_DVR_SITE
            // 
            this.PANEL_EVENT_DVR_SITE.Controls.Add(this.CB_TEXT_IN_DVR);
            this.PANEL_EVENT_DVR_SITE.Controls.Add(this.CB_RECORDING_FAIL_DVR);
            this.PANEL_EVENT_DVR_SITE.Controls.Add(this.CB_TAMPERING_DVR);
            this.PANEL_EVENT_DVR_SITE.Controls.Add(this.CB_AUDIO_DETECTION_DVR);
            this.PANEL_EVENT_DVR_SITE.Controls.Add(this.CB_MOTION_DETECTION_DVR);
            this.PANEL_EVENT_DVR_SITE.Controls.Add(this.CB_NETWORK_ALARM_IN_DVR);
            this.PANEL_EVENT_DVR_SITE.Controls.Add(this.CB_TRIPZONE_DVR);
            this.PANEL_EVENT_DVR_SITE.Controls.Add(this.CB_FACE_DETECTION_DVR);
            this.PANEL_EVENT_DVR_SITE.Controls.Add(this.CB_FAN_ERROR);
            this.PANEL_EVENT_DVR_SITE.Controls.Add(this.CB_VIDEO_LOSS_DVR);
            this.PANEL_EVENT_DVR_SITE.Controls.Add(this.CB_ALARM_IN_DVR);
            this.PANEL_EVENT_DVR_SITE.Controls.Add(this.CB_RECORD_BAD);
            this.PANEL_EVENT_DVR_SITE.Controls.Add(this.CB_PANIC_RECORD);
            this.PANEL_EVENT_DVR_SITE.Controls.Add(this.CB_DISK_CONFIG_CHANGE);
            this.PANEL_EVENT_DVR_SITE.Controls.Add(this.CB_DISK_DELETED);
            this.PANEL_EVENT_DVR_SITE.Controls.Add(this.CB_DISK_ALMOST_FULL);
            this.PANEL_EVENT_DVR_SITE.Controls.Add(this.CB_DISK_SMART);
            this.PANEL_EVENT_DVR_SITE.Controls.Add(this.CB_DISK_TEMPERATURE);
            this.PANEL_EVENT_DVR_SITE.Controls.Add(this.CB_DISK_BAD);
            this.PANEL_EVENT_DVR_SITE.Controls.Add(this.CB_ALARM_IN_BAD);
            this.PANEL_EVENT_DVR_SITE.Controls.Add(this.CB_FAN_ERROR_DVR);
            this.PANEL_EVENT_DVR_SITE.Controls.Add(this.CB_CONSECUTIVE_INVALID_PASSWORD);
            this.PANEL_EVENT_DVR_SITE.Location = new System.Drawing.Point(12, 267);
            this.PANEL_EVENT_DVR_SITE.Name = "PANEL_EVENT_DVR_SITE";
            this.PANEL_EVENT_DVR_SITE.Size = new System.Drawing.Size(390, 294);
            this.PANEL_EVENT_DVR_SITE.TabIndex = 32;
            this.PANEL_EVENT_DVR_SITE.Visible = false;
            // 
            // CB_CONSECUTIVE_INVALID_PASSWORD
            // 
            this.CB_CONSECUTIVE_INVALID_PASSWORD.AutoSize = true;
            this.CB_CONSECUTIVE_INVALID_PASSWORD.Location = new System.Drawing.Point(8, 271);
            this.CB_CONSECUTIVE_INVALID_PASSWORD.Name = "CB_CONSECUTIVE_INVALID_PASSWORD";
            this.CB_CONSECUTIVE_INVALID_PASSWORD.Size = new System.Drawing.Size(169, 17);
            this.CB_CONSECUTIVE_INVALID_PASSWORD.TabIndex = 0;
            this.CB_CONSECUTIVE_INVALID_PASSWORD.Text = "Consecutive Invalid Password";
            this.CB_CONSECUTIVE_INVALID_PASSWORD.UseVisualStyleBackColor = true;
            // 
            // CB_FAN_ERROR_DVR
            // 
            this.CB_FAN_ERROR_DVR.AutoSize = true;
            this.CB_FAN_ERROR_DVR.Location = new System.Drawing.Point(178, 248);
            this.CB_FAN_ERROR_DVR.Name = "CB_FAN_ERROR_DVR";
            this.CB_FAN_ERROR_DVR.Size = new System.Drawing.Size(71, 17);
            this.CB_FAN_ERROR_DVR.TabIndex = 1;
            this.CB_FAN_ERROR_DVR.Text = "Fan Error";
            this.CB_FAN_ERROR_DVR.UseVisualStyleBackColor = true;
            // 
            // CB_ALARM_IN_BAD
            // 
            this.CB_ALARM_IN_BAD.AutoSize = true;
            this.CB_ALARM_IN_BAD.Location = new System.Drawing.Point(178, 156);
            this.CB_ALARM_IN_BAD.Name = "CB_ALARM_IN_BAD";
            this.CB_ALARM_IN_BAD.Size = new System.Drawing.Size(88, 17);
            this.CB_ALARM_IN_BAD.TabIndex = 2;
            this.CB_ALARM_IN_BAD.Text = "Alarm-In Bad";
            this.CB_ALARM_IN_BAD.UseVisualStyleBackColor = true;
            // 
            // CB_DISK_BAD
            // 
            this.CB_DISK_BAD.AutoSize = true;
            this.CB_DISK_BAD.Location = new System.Drawing.Point(8, 179);
            this.CB_DISK_BAD.Name = "CB_DISK_BAD";
            this.CB_DISK_BAD.Size = new System.Drawing.Size(66, 17);
            this.CB_DISK_BAD.TabIndex = 3;
            this.CB_DISK_BAD.Text = "Disk Bad";
            this.CB_DISK_BAD.UseVisualStyleBackColor = true;
            // 
            // CB_DISK_TEMPERATURE
            // 
            this.CB_DISK_TEMPERATURE.AutoSize = true;
            this.CB_DISK_TEMPERATURE.Location = new System.Drawing.Point(178, 179);
            this.CB_DISK_TEMPERATURE.Name = "CB_DISK_TEMPERATURE";
            this.CB_DISK_TEMPERATURE.Size = new System.Drawing.Size(110, 17);
            this.CB_DISK_TEMPERATURE.TabIndex = 4;
            this.CB_DISK_TEMPERATURE.Text = "Disk Temperature";
            this.CB_DISK_TEMPERATURE.UseVisualStyleBackColor = true;
            // 
            // CB_DISK_SMART
            // 
            this.CB_DISK_SMART.AutoSize = true;
            this.CB_DISK_SMART.Location = new System.Drawing.Point(8, 202);
            this.CB_DISK_SMART.Name = "CB_DISK_SMART";
            this.CB_DISK_SMART.Size = new System.Drawing.Size(102, 17);
            this.CB_DISK_SMART.TabIndex = 5;
            this.CB_DISK_SMART.Text = "Disk S.M.A.R.T.";
            this.CB_DISK_SMART.UseVisualStyleBackColor = true;
            // 
            // CB_DISK_ALMOST_FULL
            // 
            this.CB_DISK_ALMOST_FULL.AutoSize = true;
            this.CB_DISK_ALMOST_FULL.Location = new System.Drawing.Point(178, 202);
            this.CB_DISK_ALMOST_FULL.Name = "CB_DISK_ALMOST_FULL";
            this.CB_DISK_ALMOST_FULL.Size = new System.Drawing.Size(99, 17);
            this.CB_DISK_ALMOST_FULL.TabIndex = 6;
            this.CB_DISK_ALMOST_FULL.Text = "Disk Almost Full";
            this.CB_DISK_ALMOST_FULL.UseVisualStyleBackColor = true;
            // 
            // CB_DISK_DELETED
            // 
            this.CB_DISK_DELETED.AutoSize = true;
            this.CB_DISK_DELETED.Location = new System.Drawing.Point(8, 225);
            this.CB_DISK_DELETED.Name = "CB_DISK_DELETED";
            this.CB_DISK_DELETED.Size = new System.Drawing.Size(85, 17);
            this.CB_DISK_DELETED.TabIndex = 7;
            this.CB_DISK_DELETED.Text = "Disk Deleted";
            this.CB_DISK_DELETED.UseVisualStyleBackColor = true;
            // 
            // CB_DISK_CONFIG_CHANGE
            // 
            this.CB_DISK_CONFIG_CHANGE.AutoSize = true;
            this.CB_DISK_CONFIG_CHANGE.Location = new System.Drawing.Point(178, 225);
            this.CB_DISK_CONFIG_CHANGE.Name = "CB_DISK_CONFIG_CHANGE";
            this.CB_DISK_CONFIG_CHANGE.Size = new System.Drawing.Size(119, 17);
            this.CB_DISK_CONFIG_CHANGE.TabIndex = 8;
            this.CB_DISK_CONFIG_CHANGE.Text = "Disk Config Change";
            this.CB_DISK_CONFIG_CHANGE.UseVisualStyleBackColor = true;
            // 
            // CB_PANIC_RECORD
            // 
            this.CB_PANIC_RECORD.AutoSize = true;
            this.CB_PANIC_RECORD.Location = new System.Drawing.Point(8, 248);
            this.CB_PANIC_RECORD.Name = "CB_PANIC_RECORD";
            this.CB_PANIC_RECORD.Size = new System.Drawing.Size(88, 17);
            this.CB_PANIC_RECORD.TabIndex = 9;
            this.CB_PANIC_RECORD.Text = "Panic Record";
            this.CB_PANIC_RECORD.UseVisualStyleBackColor = true;
            // 
            // CB_RECORD_BAD
            // 
            this.CB_RECORD_BAD.AutoSize = true;
            this.CB_RECORD_BAD.Location = new System.Drawing.Point(8, 156);
            this.CB_RECORD_BAD.Name = "CB_RECORD_BAD";
            this.CB_RECORD_BAD.Size = new System.Drawing.Size(81, 17);
            this.CB_RECORD_BAD.TabIndex = 10;
            this.CB_RECORD_BAD.Text = "Record Bad";
            this.CB_RECORD_BAD.UseVisualStyleBackColor = true;
            // 
            // CB_ALARM_IN_DVR
            // 
            this.CB_ALARM_IN_DVR.AutoSize = true;
            this.CB_ALARM_IN_DVR.Location = new System.Drawing.Point(8, 3);
            this.CB_ALARM_IN_DVR.Name = "CB_ALARM_IN_DVR";
            this.CB_ALARM_IN_DVR.Size = new System.Drawing.Size(67, 17);
            this.CB_ALARM_IN_DVR.TabIndex = 11;
            this.CB_ALARM_IN_DVR.Text = "Alarm-In";
            this.CB_ALARM_IN_DVR.UseVisualStyleBackColor = true;
            // 
            // CB_VIDEO_LOSS_DVR
            // 
            this.CB_VIDEO_LOSS_DVR.AutoSize = true;
            this.CB_VIDEO_LOSS_DVR.Location = new System.Drawing.Point(178, 26);
            this.CB_VIDEO_LOSS_DVR.Name = "CB_VIDEO_LOSS_DVR";
            this.CB_VIDEO_LOSS_DVR.Size = new System.Drawing.Size(76, 17);
            this.CB_VIDEO_LOSS_DVR.TabIndex = 12;
            this.CB_VIDEO_LOSS_DVR.Text = "Video Loss";
            this.CB_VIDEO_LOSS_DVR.UseVisualStyleBackColor = true;
            // 
            // CB_FAN_ERROR
            // 
            this.CB_FAN_ERROR.AutoSize = true;
            this.CB_FAN_ERROR.Location = new System.Drawing.Point(8, 95);
            this.CB_FAN_ERROR.Name = "CB_FAN_ERROR";
            this.CB_FAN_ERROR.Size = new System.Drawing.Size(71, 17);
            this.CB_FAN_ERROR.TabIndex = 14;
            this.CB_FAN_ERROR.Text = "Fan Error";
            this.CB_FAN_ERROR.UseVisualStyleBackColor = true;
            // 
            // CB_FACE_DETECTION_DVR
            // 
            this.CB_FACE_DETECTION_DVR.AutoSize = true;
            this.CB_FACE_DETECTION_DVR.Location = new System.Drawing.Point(8, 72);
            this.CB_FACE_DETECTION_DVR.Name = "CB_FACE_DETECTION_DVR";
            this.CB_FACE_DETECTION_DVR.Size = new System.Drawing.Size(98, 17);
            this.CB_FACE_DETECTION_DVR.TabIndex = 15;
            this.CB_FACE_DETECTION_DVR.Text = "Face Detection";
            this.CB_FACE_DETECTION_DVR.UseVisualStyleBackColor = true;
            // 
            // CB_TRIPZONE_DVR
            // 
            this.CB_TRIPZONE_DVR.AutoSize = true;
            this.CB_TRIPZONE_DVR.Location = new System.Drawing.Point(8, 49);
            this.CB_TRIPZONE_DVR.Name = "CB_TRIPZONE_DVR";
            this.CB_TRIPZONE_DVR.Size = new System.Drawing.Size(68, 17);
            this.CB_TRIPZONE_DVR.TabIndex = 16;
            this.CB_TRIPZONE_DVR.Text = "TripZone";
            this.CB_TRIPZONE_DVR.UseVisualStyleBackColor = true;
            // 
            // CB_NETWORK_ALARM_IN_DVR
            // 
            this.CB_NETWORK_ALARM_IN_DVR.AutoSize = true;
            this.CB_NETWORK_ALARM_IN_DVR.Location = new System.Drawing.Point(178, 3);
            this.CB_NETWORK_ALARM_IN_DVR.Name = "CB_NETWORK_ALARM_IN_DVR";
            this.CB_NETWORK_ALARM_IN_DVR.Size = new System.Drawing.Size(110, 17);
            this.CB_NETWORK_ALARM_IN_DVR.TabIndex = 17;
            this.CB_NETWORK_ALARM_IN_DVR.Text = "Network Alarm-In";
            this.CB_NETWORK_ALARM_IN_DVR.UseVisualStyleBackColor = true;
            // 
            // CB_AUDIO_DETECTION_DVR
            // 
            this.CB_AUDIO_DETECTION_DVR.AutoSize = true;
            this.CB_AUDIO_DETECTION_DVR.Location = new System.Drawing.Point(8, 118);
            this.CB_AUDIO_DETECTION_DVR.Name = "CB_AUDIO_DETECTION_DVR";
            this.CB_AUDIO_DETECTION_DVR.Size = new System.Drawing.Size(102, 17);
            this.CB_AUDIO_DETECTION_DVR.TabIndex = 18;
            this.CB_AUDIO_DETECTION_DVR.Text = "Audio Detection";
            this.CB_AUDIO_DETECTION_DVR.UseVisualStyleBackColor = true;
            // 
            // CB_MOTION_DETECTION_DVR
            // 
            this.CB_MOTION_DETECTION_DVR.AutoSize = true;
            this.CB_MOTION_DETECTION_DVR.Location = new System.Drawing.Point(8, 26);
            this.CB_MOTION_DETECTION_DVR.Name = "CB_MOTION_DETECTION_DVR";
            this.CB_MOTION_DETECTION_DVR.Size = new System.Drawing.Size(107, 17);
            this.CB_MOTION_DETECTION_DVR.TabIndex = 33;
            this.CB_MOTION_DETECTION_DVR.Text = "Motion Detection";
            this.CB_MOTION_DETECTION_DVR.UseVisualStyleBackColor = true;
            // 
            // CB_TAMPERING_DVR
            // 
            this.CB_TAMPERING_DVR.AutoSize = true;
            this.CB_TAMPERING_DVR.Location = new System.Drawing.Point(178, 49);
            this.CB_TAMPERING_DVR.Name = "CB_TAMPERING_DVR";
            this.CB_TAMPERING_DVR.Size = new System.Drawing.Size(76, 17);
            this.CB_TAMPERING_DVR.TabIndex = 34;
            this.CB_TAMPERING_DVR.Text = "Tampering";
            this.CB_TAMPERING_DVR.UseVisualStyleBackColor = true;
            // 
            // CB_RECORDING_FAIL_DVR
            // 
            this.CB_RECORDING_FAIL_DVR.AutoSize = true;
            this.CB_RECORDING_FAIL_DVR.Location = new System.Drawing.Point(178, 72);
            this.CB_RECORDING_FAIL_DVR.Name = "CB_RECORDING_FAIL_DVR";
            this.CB_RECORDING_FAIL_DVR.Size = new System.Drawing.Size(93, 17);
            this.CB_RECORDING_FAIL_DVR.TabIndex = 35;
            this.CB_RECORDING_FAIL_DVR.Text = "Recording Fail";
            this.CB_RECORDING_FAIL_DVR.UseVisualStyleBackColor = true;
            // 
            // CB_TEXT_IN_DVR
            // 
            this.CB_TEXT_IN_DVR.AutoSize = true;
            this.CB_TEXT_IN_DVR.Location = new System.Drawing.Point(178, 95);
            this.CB_TEXT_IN_DVR.Name = "CB_TEXT_IN_DVR";
            this.CB_TEXT_IN_DVR.Size = new System.Drawing.Size(62, 17);
            this.CB_TEXT_IN_DVR.TabIndex = 36;
            this.CB_TEXT_IN_DVR.Text = "Text-In";
            this.CB_TEXT_IN_DVR.UseVisualStyleBackColor = true;
            // 
            // form_event_search
            // 
            this.AcceptButton = this.BTN_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BTN_CANCEL;
            this.ClientSize = new System.Drawing.Size(414, 605);
            this.Controls.Add(this.PANEL_EVENT_DVR_SITE);
            this.Controls.Add(this.PANEL_EVENT_SERVICE_SITE);
            this.Controls.Add(this.STC_TO);
            this.Controls.Add(this.STC_FROM);
            this.Controls.Add(this.TRV_DEVICES);
            this.Controls.Add(this.CHK_FIRST);
            this.Controls.Add(this.CHK_LAST);
            this.Controls.Add(this.DTP_FROM);
            this.Controls.Add(this.DTP_TO);
            this.Controls.Add(this.BTN_OK);
            this.Controls.Add(this.BTN_CANCEL);
            this.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "form_event_search";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Event Search";
            this.PANEL_EVENT_SERVICE_SITE.ResumeLayout(false);
            this.PANEL_EVENT_SERVICE_SITE.PerformLayout();
            this.PANEL_EVENT_DVR_SITE.ResumeLayout(false);
            this.PANEL_EVENT_DVR_SITE.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label STC_FROM;
        private System.Windows.Forms.Label STC_TO;
        private System.Windows.Forms.CheckBox CHK_FIRST;
        private System.Windows.Forms.CheckBox CHK_LAST;
        private System.Windows.Forms.DateTimePicker DTP_FROM;
        private System.Windows.Forms.DateTimePicker DTP_TO;
        private System.Windows.Forms.TreeView TRV_DEVICES;
        private System.Windows.Forms.Button BTN_OK;
        private System.Windows.Forms.Button BTN_CANCEL;
        private System.Windows.Forms.Panel PANEL_EVENT_SERVICE_SITE;
        private System.Windows.Forms.CheckBox CB_FACE_DETECTION;
        private System.Windows.Forms.CheckBox CB_DEVICE_DISCONNECTION;
        private System.Windows.Forms.CheckBox CB_AUDIO_DETECTION;
        private System.Windows.Forms.CheckBox CB_VIDEO_ANALYTICS;
        private System.Windows.Forms.CheckBox CB_TRIPZONE;
        private System.Windows.Forms.CheckBox CB_VIDEO_LOSS;
        private System.Windows.Forms.CheckBox CB_USER_ALARAM_IN;
        private System.Windows.Forms.CheckBox CB_DEVICE_CONNECTION;
        private System.Windows.Forms.CheckBox CB_TEXT_IN;
        private System.Windows.Forms.CheckBox CB_TAMPERING;
        private System.Windows.Forms.CheckBox CB_VIDEO_BLIND;
        private System.Windows.Forms.CheckBox CB_MOTION_DETECTION;
        private System.Windows.Forms.CheckBox CB_ALARM_IN;
        private System.Windows.Forms.Panel PANEL_EVENT_DVR_SITE;
        private System.Windows.Forms.CheckBox CB_CONSECUTIVE_INVALID_PASSWORD;
        private System.Windows.Forms.CheckBox CB_FAN_ERROR_DVR;
        private System.Windows.Forms.CheckBox CB_PANIC_RECORD;
        private System.Windows.Forms.CheckBox CB_DISK_CONFIG_CHANGE;
        private System.Windows.Forms.CheckBox CB_DISK_DELETED;
        private System.Windows.Forms.CheckBox CB_DISK_ALMOST_FULL;
        private System.Windows.Forms.CheckBox CB_DISK_SMART;
        private System.Windows.Forms.CheckBox CB_DISK_TEMPERATURE;
        private System.Windows.Forms.CheckBox CB_DISK_BAD;
        private System.Windows.Forms.CheckBox CB_ALARM_IN_BAD;
        private System.Windows.Forms.CheckBox CB_RECORD_BAD;
        private System.Windows.Forms.CheckBox CB_ALARM_IN_DVR;
        private System.Windows.Forms.CheckBox CB_VIDEO_LOSS_DVR;
        private System.Windows.Forms.CheckBox CB_MOTION_DETECTION_DVR;
        private System.Windows.Forms.CheckBox CB_NETWORK_ALARM_IN_DVR;
        private System.Windows.Forms.CheckBox CB_TRIPZONE_DVR;
        private System.Windows.Forms.CheckBox CB_FACE_DETECTION_DVR;
        private System.Windows.Forms.CheckBox CB_FAN_ERROR;
        private System.Windows.Forms.CheckBox CB_AUDIO_DETECTION_DVR;
        private System.Windows.Forms.CheckBox CB_TEXT_IN_DVR;
        private System.Windows.Forms.CheckBox CB_RECORDING_FAIL_DVR;
        private System.Windows.Forms.CheckBox CB_TAMPERING_DVR;
    }
}
