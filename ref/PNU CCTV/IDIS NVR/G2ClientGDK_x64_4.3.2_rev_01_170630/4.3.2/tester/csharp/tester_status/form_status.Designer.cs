namespace GDK_tester
{
    partial class form_status
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(form_status));
            this.BTN_CONNECT = new System.Windows.Forms.Button();
            this.BTN_DISCONNECT = new System.Windows.Forms.Button();
            this.LSV_HEALTH = new System.Windows.Forms.ListView();
            this.LSV_HEALTH_COL_STATUS = new System.Windows.Forms.ColumnHeader();
            this.LSV_HEALTH_COL_PROBLEM = new System.Windows.Forms.ColumnHeader();
            this.LSV_HEALTH_COL_MAC = new System.Windows.Forms.ColumnHeader();
            this.LSV_HEALTH_COL_VERSION = new System.Windows.Forms.ColumnHeader();
            this.LSV_HEALTH_COL_CAMERA = new System.Windows.Forms.ColumnHeader();
            this.LSV_HEALTH_COL_SENSOR = new System.Windows.Forms.ColumnHeader();
            this.LSV_HEALTH_COL_FAN = new System.Windows.Forms.ColumnHeader();
            this.LSV_HEALTH_COL_REC_CHECK = new System.Windows.Forms.ColumnHeader();
            this.LSV_HEALTH_COL_REC = new System.Windows.Forms.ColumnHeader();
            this.LSV_HEALTH_COL_REC_PERIOD = new System.Windows.Forms.ColumnHeader();
            this.LSV_INSTANT_RECORD = new System.Windows.Forms.ListView();
            this.LSV_INSTANT_RECORD_COL_CHANNEL = new System.Windows.Forms.ColumnHeader();
            this.LSV_INSTANT_RECORD_COL_DURATION = new System.Windows.Forms.ColumnHeader();
            this.LSV_INSTANT_RECORD_COL_RECORD = new System.Windows.Forms.ColumnHeader();
            this.LSV_INSTANT_RECORD_COL_SCOPE = new System.Windows.Forms.ColumnHeader();
            this.LSV_INSTANT_RECORD_COL_FAIL_REASON = new System.Windows.Forms.ColumnHeader();
            this.GRP_LOG = new System.Windows.Forms.GroupBox();
            this.LOG_STC_FROM = new System.Windows.Forms.Label();
            this.LOG_STC_TO = new System.Windows.Forms.Label();
            this.LOG_CHK_FIRST = new System.Windows.Forms.CheckBox();
            this.LOG_CHK_LAST = new System.Windows.Forms.CheckBox();
            this.LOG_DTP_FROM = new System.Windows.Forms.DateTimePicker();
            this.LOG_DTP_TO = new System.Windows.Forms.DateTimePicker();
            this.LOG_BTN_SYSTEM = new System.Windows.Forms.Button();
            this.LOG_BTN_EVENT = new System.Windows.Forms.Button();
            this.LOG_BTN_DEBUG = new System.Windows.Forms.Button();
            this.LOG_BTN_SYSTEM_MORE = new System.Windows.Forms.Button();
            this.LOG_BTN_EVENT_MORE = new System.Windows.Forms.Button();
            this.LOG_BTN_DEBUG_MORE = new System.Windows.Forms.Button();
            this.LSV_LOG_SYSTEM = new System.Windows.Forms.ListView();
            this.LSV_LOG_SYSTEM_COL_DATA = new System.Windows.Forms.ColumnHeader();
            this.LSV_LOG_SYSTEM_COL_TIME = new System.Windows.Forms.ColumnHeader();
            this.LSV_LOG_EVENT = new System.Windows.Forms.ListView();
            this.LSV_LOG_EVENT_COL_EVENT = new System.Windows.Forms.ColumnHeader();
            this.LSV_LOG_EVENT_COL_LABEL = new System.Windows.Forms.ColumnHeader();
            this.LSV_LOG_EVENT_COL_TIME = new System.Windows.Forms.ColumnHeader();
            this.CHK_INSTANT_RECORD_STATUS = new System.Windows.Forms.CheckBox();
            this.CHK_RECORD_PANIC = new System.Windows.Forms.CheckBox();
            this.RECORD_BTN_RECORDING = new System.Windows.Forms.Button();
            this.RECORD_BTN_PLAYBACK = new System.Windows.Forms.Button();
            this.RECORD_BTN_ARCHIVE = new System.Windows.Forms.Button();
            this.RECORD_BTN_CLIP_EXPORT = new System.Windows.Forms.Button();
            this.GRP_CALLBACK = new System.Windows.Forms.GroupBox();
            this.CALLBACK_BTN_STARTUP = new System.Windows.Forms.Button();
            this.CALLBACK_STC_PORT = new System.Windows.Forms.Label();
            this.CALLBACK_EDT_PORT = new GDK_tester.text_box_numeric();
            this.LSV_EVENT = new System.Windows.Forms.ListView();
            this.LSV_EVENT_COL_SITE = new System.Windows.Forms.ColumnHeader();
            this.LSV_EVENT_COL_EVENT = new System.Windows.Forms.ColumnHeader();
            this.LSV_EVENT_COL_DATA = new System.Windows.Forms.ColumnHeader();
            this.LSV_EVENT_COL_TIME = new System.Windows.Forms.ColumnHeader();
            this.STC_STATUS = new System.Windows.Forms.Label();
            this.BTN_REMOTE_SETUP = new System.Windows.Forms.Button();
            this.GRP_LOG.SuspendLayout();
            this.GRP_CALLBACK.SuspendLayout();
            this.SuspendLayout();
            // 
            // BTN_CONNECT
            // 
            this.BTN_CONNECT.Location = new System.Drawing.Point(615, 446);
            this.BTN_CONNECT.Name = "BTN_CONNECT";
            this.BTN_CONNECT.Size = new System.Drawing.Size(75, 23);
            this.BTN_CONNECT.TabIndex = 0;
            this.BTN_CONNECT.Text = "Connect";
            this.BTN_CONNECT.UseVisualStyleBackColor = true;
            this.BTN_CONNECT.Click += new System.EventHandler(this.on_btn_connect);
            // 
            // BTN_DISCONNECT
            // 
            this.BTN_DISCONNECT.Enabled = false;
            this.BTN_DISCONNECT.Location = new System.Drawing.Point(615, 472);
            this.BTN_DISCONNECT.Name = "BTN_DISCONNECT";
            this.BTN_DISCONNECT.Size = new System.Drawing.Size(75, 23);
            this.BTN_DISCONNECT.TabIndex = 1;
            this.BTN_DISCONNECT.Text = "Disconnect";
            this.BTN_DISCONNECT.UseVisualStyleBackColor = true;
            this.BTN_DISCONNECT.Click += new System.EventHandler(this.on_btn_disconnect);
            // 
            // LSV_HEALTH
            // 
            this.LSV_HEALTH.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LSV_HEALTH_COL_STATUS,
            this.LSV_HEALTH_COL_PROBLEM,
            this.LSV_HEALTH_COL_MAC,
            this.LSV_HEALTH_COL_VERSION,
            this.LSV_HEALTH_COL_CAMERA,
            this.LSV_HEALTH_COL_SENSOR,
            this.LSV_HEALTH_COL_FAN,
            this.LSV_HEALTH_COL_REC_CHECK,
            this.LSV_HEALTH_COL_REC,
            this.LSV_HEALTH_COL_REC_PERIOD});
            this.LSV_HEALTH.ForeColor = System.Drawing.Color.DimGray;
            this.LSV_HEALTH.GridLines = true;
            this.LSV_HEALTH.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LSV_HEALTH.Location = new System.Drawing.Point(10, 379);
            this.LSV_HEALTH.MultiSelect = false;
            this.LSV_HEALTH.Name = "LSV_HEALTH";
            this.LSV_HEALTH.Scrollable = false;
            this.LSV_HEALTH.ShowItemToolTips = true;
            this.LSV_HEALTH.Size = new System.Drawing.Size(680, 44);
            this.LSV_HEALTH.TabIndex = 12;
            this.LSV_HEALTH.UseCompatibleStateImageBehavior = false;
            this.LSV_HEALTH.View = System.Windows.Forms.View.Details;
            this.LSV_HEALTH.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.on_feature_health_selection_changed);
            // 
            // LSV_HEALTH_COL_STATUS
            // 
            this.LSV_HEALTH_COL_STATUS.Text = "Status";
            // 
            // LSV_HEALTH_COL_PROBLEM
            // 
            this.LSV_HEALTH_COL_PROBLEM.Text = "Problem";
            // 
            // LSV_HEALTH_COL_MAC
            // 
            this.LSV_HEALTH_COL_MAC.Text = "MAC";
            this.LSV_HEALTH_COL_MAC.Width = 70;
            // 
            // LSV_HEALTH_COL_VERSION
            // 
            this.LSV_HEALTH_COL_VERSION.Text = "Version";
            // 
            // LSV_HEALTH_COL_CAMERA
            // 
            this.LSV_HEALTH_COL_CAMERA.Text = "Camera";
            // 
            // LSV_HEALTH_COL_SENSOR
            // 
            this.LSV_HEALTH_COL_SENSOR.Text = "Alarm";
            // 
            // LSV_HEALTH_COL_FAN
            // 
            this.LSV_HEALTH_COL_FAN.Text = "Fan";
            // 
            // LSV_HEALTH_COL_REC_CHECK
            // 
            this.LSV_HEALTH_COL_REC_CHECK.Text = "Rec. Check";
            // 
            // LSV_HEALTH_COL_REC
            // 
            this.LSV_HEALTH_COL_REC.Text = "Record";
            // 
            // LSV_HEALTH_COL_REC_PERIOD
            // 
            this.LSV_HEALTH_COL_REC_PERIOD.Text = "Record Period";
            this.LSV_HEALTH_COL_REC_PERIOD.Width = 126;
            // 
            // LSV_INSTANT_RECORD
            // 
            this.LSV_INSTANT_RECORD.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LSV_INSTANT_RECORD_COL_CHANNEL,
            this.LSV_INSTANT_RECORD_COL_DURATION,
            this.LSV_INSTANT_RECORD_COL_RECORD,
            this.LSV_INSTANT_RECORD_COL_SCOPE,
            this.LSV_INSTANT_RECORD_COL_FAIL_REASON});
            this.LSV_INSTANT_RECORD.GridLines = true;
            this.LSV_INSTANT_RECORD.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LSV_INSTANT_RECORD.Location = new System.Drawing.Point(10, 537);
            this.LSV_INSTANT_RECORD.MultiSelect = false;
            this.LSV_INSTANT_RECORD.Name = "LSV_INSTANT_RECORD";
            this.LSV_INSTANT_RECORD.ShowItemToolTips = true;
            this.LSV_INSTANT_RECORD.Size = new System.Drawing.Size(680, 163);
            this.LSV_INSTANT_RECORD.TabIndex = 13;
            this.LSV_INSTANT_RECORD.UseCompatibleStateImageBehavior = false;
            this.LSV_INSTANT_RECORD.View = System.Windows.Forms.View.Details;
            this.LSV_INSTANT_RECORD.Visible = false;
            this.LSV_INSTANT_RECORD.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.on_feature_instant_record_selection_changed);
            // 
            // LSV_INSTANT_RECORD_COL_CHANNEL
            // 
            this.LSV_INSTANT_RECORD_COL_CHANNEL.Text = "Ch.";
            this.LSV_INSTANT_RECORD_COL_CHANNEL.Width = 30;
            // 
            // LSV_INSTANT_RECORD_COL_DURATION
            // 
            this.LSV_INSTANT_RECORD_COL_DURATION.Text = "Duration / Pre";
            this.LSV_INSTANT_RECORD_COL_DURATION.Width = 120;
            // 
            // LSV_INSTANT_RECORD_COL_RECORD
            // 
            this.LSV_INSTANT_RECORD_COL_RECORD.Text = "Record / Audio";
            this.LSV_INSTANT_RECORD_COL_RECORD.Width = 85;
            // 
            // LSV_INSTANT_RECORD_COL_SCOPE
            // 
            this.LSV_INSTANT_RECORD_COL_SCOPE.Text = "Record Period";
            this.LSV_INSTANT_RECORD_COL_SCOPE.Width = 230;
            // 
            // LSV_INSTANT_RECORD_COL_FAIL_REASON
            // 
            this.LSV_INSTANT_RECORD_COL_FAIL_REASON.Text = "Fail Reason";
            this.LSV_INSTANT_RECORD_COL_FAIL_REASON.Width = 194;
            // 
            // GRP_LOG
            // 
            this.GRP_LOG.Controls.Add(this.LOG_STC_FROM);
            this.GRP_LOG.Controls.Add(this.LOG_STC_TO);
            this.GRP_LOG.Controls.Add(this.LOG_CHK_FIRST);
            this.GRP_LOG.Controls.Add(this.LOG_CHK_LAST);
            this.GRP_LOG.Controls.Add(this.LOG_DTP_FROM);
            this.GRP_LOG.Controls.Add(this.LOG_DTP_TO);
            this.GRP_LOG.Controls.Add(this.LOG_BTN_SYSTEM);
            this.GRP_LOG.Controls.Add(this.LOG_BTN_EVENT);
            this.GRP_LOG.Controls.Add(this.LOG_BTN_DEBUG);
            this.GRP_LOG.Controls.Add(this.LOG_BTN_SYSTEM_MORE);
            this.GRP_LOG.Controls.Add(this.LOG_BTN_EVENT_MORE);
            this.GRP_LOG.Controls.Add(this.LOG_BTN_DEBUG_MORE);
            this.GRP_LOG.Location = new System.Drawing.Point(10, 429);
            this.GRP_LOG.Name = "GRP_LOG";
            this.GRP_LOG.Size = new System.Drawing.Size(397, 102);
            this.GRP_LOG.TabIndex = 3;
            this.GRP_LOG.TabStop = false;
            this.GRP_LOG.Text = "Log";
            // 
            // LOG_STC_FROM
            // 
            this.LOG_STC_FROM.Location = new System.Drawing.Point(6, 19);
            this.LOG_STC_FROM.Name = "LOG_STC_FROM";
            this.LOG_STC_FROM.Size = new System.Drawing.Size(55, 17);
            this.LOG_STC_FROM.TabIndex = 31;
            this.LOG_STC_FROM.Text = "From :";
            this.LOG_STC_FROM.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LOG_STC_TO
            // 
            this.LOG_STC_TO.Location = new System.Drawing.Point(6, 42);
            this.LOG_STC_TO.Name = "LOG_STC_TO";
            this.LOG_STC_TO.Size = new System.Drawing.Size(55, 17);
            this.LOG_STC_TO.TabIndex = 32;
            this.LOG_STC_TO.Text = "To :";
            this.LOG_STC_TO.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LOG_CHK_FIRST
            // 
            this.LOG_CHK_FIRST.AutoSize = true;
            this.LOG_CHK_FIRST.Location = new System.Drawing.Point(67, 20);
            this.LOG_CHK_FIRST.Name = "LOG_CHK_FIRST";
            this.LOG_CHK_FIRST.Size = new System.Drawing.Size(47, 17);
            this.LOG_CHK_FIRST.TabIndex = 0;
            this.LOG_CHK_FIRST.Text = "First";
            this.LOG_CHK_FIRST.UseVisualStyleBackColor = true;
            this.LOG_CHK_FIRST.CheckedChanged += new System.EventHandler(this.on_log_chk_first);
            // 
            // LOG_CHK_LAST
            // 
            this.LOG_CHK_LAST.AutoSize = true;
            this.LOG_CHK_LAST.Location = new System.Drawing.Point(67, 43);
            this.LOG_CHK_LAST.Name = "LOG_CHK_LAST";
            this.LOG_CHK_LAST.Size = new System.Drawing.Size(46, 17);
            this.LOG_CHK_LAST.TabIndex = 2;
            this.LOG_CHK_LAST.Text = "Last";
            this.LOG_CHK_LAST.UseVisualStyleBackColor = true;
            this.LOG_CHK_LAST.CheckedChanged += new System.EventHandler(this.on_log_chk_last);
            // 
            // LOG_DTP_FROM
            // 
            this.LOG_DTP_FROM.CustomFormat = " yyyy-MM-dd HH:mm:ss";
            this.LOG_DTP_FROM.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.LOG_DTP_FROM.Location = new System.Drawing.Point(120, 17);
            this.LOG_DTP_FROM.Name = "LOG_DTP_FROM";
            this.LOG_DTP_FROM.Size = new System.Drawing.Size(148, 20);
            this.LOG_DTP_FROM.TabIndex = 1;
            // 
            // LOG_DTP_TO
            // 
            this.LOG_DTP_TO.CustomFormat = " yyyy-MM-dd HH:mm:ss";
            this.LOG_DTP_TO.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.LOG_DTP_TO.Location = new System.Drawing.Point(119, 43);
            this.LOG_DTP_TO.Name = "LOG_DTP_TO";
            this.LOG_DTP_TO.Size = new System.Drawing.Size(149, 20);
            this.LOG_DTP_TO.TabIndex = 3;
            // 
            // LOG_BTN_SYSTEM
            // 
            this.LOG_BTN_SYSTEM.Location = new System.Drawing.Point(278, 17);
            this.LOG_BTN_SYSTEM.Name = "LOG_BTN_SYSTEM";
            this.LOG_BTN_SYSTEM.Size = new System.Drawing.Size(75, 23);
            this.LOG_BTN_SYSTEM.TabIndex = 4;
            this.LOG_BTN_SYSTEM.Text = "System";
            this.LOG_BTN_SYSTEM.UseVisualStyleBackColor = true;
            this.LOG_BTN_SYSTEM.Click += new System.EventHandler(this.on_log_btn_request);
            // 
            // LOG_BTN_EVENT
            // 
            this.LOG_BTN_EVENT.Location = new System.Drawing.Point(278, 43);
            this.LOG_BTN_EVENT.Name = "LOG_BTN_EVENT";
            this.LOG_BTN_EVENT.Size = new System.Drawing.Size(75, 23);
            this.LOG_BTN_EVENT.TabIndex = 6;
            this.LOG_BTN_EVENT.Text = "Event";
            this.LOG_BTN_EVENT.UseVisualStyleBackColor = true;
            this.LOG_BTN_EVENT.Click += new System.EventHandler(this.on_log_btn_request);
            // 
            // LOG_BTN_DEBUG
            // 
            this.LOG_BTN_DEBUG.Location = new System.Drawing.Point(278, 68);
            this.LOG_BTN_DEBUG.Name = "LOG_BTN_DEBUG";
            this.LOG_BTN_DEBUG.Size = new System.Drawing.Size(75, 23);
            this.LOG_BTN_DEBUG.TabIndex = 8;
            this.LOG_BTN_DEBUG.Text = "Debug";
            this.LOG_BTN_DEBUG.UseVisualStyleBackColor = true;
            this.LOG_BTN_DEBUG.Click += new System.EventHandler(this.on_log_btn_request);
            // 
            // LOG_BTN_SYSTEM_MORE
            // 
            this.LOG_BTN_SYSTEM_MORE.Location = new System.Drawing.Point(354, 18);
            this.LOG_BTN_SYSTEM_MORE.Name = "LOG_BTN_SYSTEM_MORE";
            this.LOG_BTN_SYSTEM_MORE.Size = new System.Drawing.Size(32, 23);
            this.LOG_BTN_SYSTEM_MORE.TabIndex = 5;
            this.LOG_BTN_SYSTEM_MORE.Text = ">";
            this.LOG_BTN_SYSTEM_MORE.UseVisualStyleBackColor = true;
            this.LOG_BTN_SYSTEM_MORE.Click += new System.EventHandler(this.on_log_btn_request_more);
            // 
            // LOG_BTN_EVENT_MORE
            // 
            this.LOG_BTN_EVENT_MORE.Location = new System.Drawing.Point(354, 43);
            this.LOG_BTN_EVENT_MORE.Name = "LOG_BTN_EVENT_MORE";
            this.LOG_BTN_EVENT_MORE.Size = new System.Drawing.Size(32, 23);
            this.LOG_BTN_EVENT_MORE.TabIndex = 7;
            this.LOG_BTN_EVENT_MORE.Text = ">";
            this.LOG_BTN_EVENT_MORE.UseVisualStyleBackColor = true;
            this.LOG_BTN_EVENT_MORE.Click += new System.EventHandler(this.on_log_btn_request_more);
            // 
            // LOG_BTN_DEBUG_MORE
            // 
            this.LOG_BTN_DEBUG_MORE.Location = new System.Drawing.Point(354, 68);
            this.LOG_BTN_DEBUG_MORE.Name = "LOG_BTN_DEBUG_MORE";
            this.LOG_BTN_DEBUG_MORE.Size = new System.Drawing.Size(32, 23);
            this.LOG_BTN_DEBUG_MORE.TabIndex = 9;
            this.LOG_BTN_DEBUG_MORE.Text = ">";
            this.LOG_BTN_DEBUG_MORE.UseVisualStyleBackColor = true;
            this.LOG_BTN_DEBUG_MORE.Click += new System.EventHandler(this.on_log_btn_request_more);
            // 
            // LSV_LOG_SYSTEM
            // 
            this.LSV_LOG_SYSTEM.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LSV_LOG_SYSTEM_COL_DATA,
            this.LSV_LOG_SYSTEM_COL_TIME});
            this.LSV_LOG_SYSTEM.FullRowSelect = true;
            this.LSV_LOG_SYSTEM.GridLines = true;
            this.LSV_LOG_SYSTEM.HideSelection = false;
            this.LSV_LOG_SYSTEM.Location = new System.Drawing.Point(10, 537);
            this.LSV_LOG_SYSTEM.MultiSelect = false;
            this.LSV_LOG_SYSTEM.Name = "LSV_LOG_SYSTEM";
            this.LSV_LOG_SYSTEM.Size = new System.Drawing.Size(680, 163);
            this.LSV_LOG_SYSTEM.TabIndex = 5;
            this.LSV_LOG_SYSTEM.UseCompatibleStateImageBehavior = false;
            this.LSV_LOG_SYSTEM.View = System.Windows.Forms.View.Details;
            // 
            // LSV_LOG_SYSTEM_COL_DATA
            // 
            this.LSV_LOG_SYSTEM_COL_DATA.Text = "Data";
            this.LSV_LOG_SYSTEM_COL_DATA.Width = 479;
            // 
            // LSV_LOG_SYSTEM_COL_TIME
            // 
            this.LSV_LOG_SYSTEM_COL_TIME.Text = "Time";
            this.LSV_LOG_SYSTEM_COL_TIME.Width = 180;
            // 
            // LSV_LOG_EVENT
            // 
            this.LSV_LOG_EVENT.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LSV_LOG_EVENT_COL_EVENT,
            this.LSV_LOG_EVENT_COL_LABEL,
            this.LSV_LOG_EVENT_COL_TIME});
            this.LSV_LOG_EVENT.FullRowSelect = true;
            this.LSV_LOG_EVENT.GridLines = true;
            this.LSV_LOG_EVENT.HideSelection = false;
            this.LSV_LOG_EVENT.Location = new System.Drawing.Point(10, 537);
            this.LSV_LOG_EVENT.MultiSelect = false;
            this.LSV_LOG_EVENT.Name = "LSV_LOG_EVENT";
            this.LSV_LOG_EVENT.ShowItemToolTips = true;
            this.LSV_LOG_EVENT.Size = new System.Drawing.Size(680, 163);
            this.LSV_LOG_EVENT.TabIndex = 5;
            this.LSV_LOG_EVENT.UseCompatibleStateImageBehavior = false;
            this.LSV_LOG_EVENT.View = System.Windows.Forms.View.Details;
            // 
            // LSV_LOG_EVENT_COL_EVENT
            // 
            this.LSV_LOG_EVENT_COL_EVENT.Text = "Event";
            this.LSV_LOG_EVENT_COL_EVENT.Width = 150;
            // 
            // LSV_LOG_EVENT_COL_LABEL
            // 
            this.LSV_LOG_EVENT_COL_LABEL.Text = "Data";
            this.LSV_LOG_EVENT_COL_LABEL.Width = 329;
            // 
            // LSV_LOG_EVENT_COL_TIME
            // 
            this.LSV_LOG_EVENT_COL_TIME.Text = "Time";
            this.LSV_LOG_EVENT_COL_TIME.Width = 180;
            // 
            // CHK_INSTANT_RECORD_STATUS
            // 
            this.CHK_INSTANT_RECORD_STATUS.AutoSize = true;
            this.CHK_INSTANT_RECORD_STATUS.Location = new System.Drawing.Point(420, 433);
            this.CHK_INSTANT_RECORD_STATUS.Name = "CHK_INSTANT_RECORD_STATUS";
            this.CHK_INSTANT_RECORD_STATUS.Size = new System.Drawing.Size(132, 17);
            this.CHK_INSTANT_RECORD_STATUS.TabIndex = 4;
            this.CHK_INSTANT_RECORD_STATUS.Text = "Instant Record Status";
            this.CHK_INSTANT_RECORD_STATUS.UseVisualStyleBackColor = true;
            this.CHK_INSTANT_RECORD_STATUS.CheckedChanged += new System.EventHandler(this.on_chk_instant_record_status_checked_changed);
            // 
            // CHK_RECORD_PANIC
            // 
            this.CHK_RECORD_PANIC.AutoSize = true;
            this.CHK_RECORD_PANIC.Location = new System.Drawing.Point(420, 451);
            this.CHK_RECORD_PANIC.Name = "CHK_RECORD_PANIC";
            this.CHK_RECORD_PANIC.Size = new System.Drawing.Size(88, 17);
            this.CHK_RECORD_PANIC.TabIndex = 5;
            this.CHK_RECORD_PANIC.Text = "Panic Record";
            this.CHK_RECORD_PANIC.UseVisualStyleBackColor = true;
            this.CHK_RECORD_PANIC.CheckedChanged += new System.EventHandler(this.on_chk_panic_record);
            // 
            // RECORD_BTN_RECORDING
            // 
            this.RECORD_BTN_RECORDING.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.RECORD_BTN_RECORDING.Location = new System.Drawing.Point(420, 472);
            this.RECORD_BTN_RECORDING.Name = "RECORD_BTN_RECORDING";
            this.RECORD_BTN_RECORDING.Size = new System.Drawing.Size(75, 22);
            this.RECORD_BTN_RECORDING.TabIndex = 6;
            this.RECORD_BTN_RECORDING.Text = "Recording";
            this.RECORD_BTN_RECORDING.UseVisualStyleBackColor = false;
            // 
            // RECORD_BTN_PLAYBACK
            // 
            this.RECORD_BTN_PLAYBACK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.RECORD_BTN_PLAYBACK.Location = new System.Drawing.Point(498, 472);
            this.RECORD_BTN_PLAYBACK.Name = "RECORD_BTN_PLAYBACK";
            this.RECORD_BTN_PLAYBACK.Size = new System.Drawing.Size(75, 22);
            this.RECORD_BTN_PLAYBACK.TabIndex = 7;
            this.RECORD_BTN_PLAYBACK.Text = "Playback";
            this.RECORD_BTN_PLAYBACK.UseVisualStyleBackColor = true;
            // 
            // RECORD_BTN_ARCHIVE
            // 
            this.RECORD_BTN_ARCHIVE.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.RECORD_BTN_ARCHIVE.Location = new System.Drawing.Point(420, 497);
            this.RECORD_BTN_ARCHIVE.Name = "RECORD_BTN_ARCHIVE";
            this.RECORD_BTN_ARCHIVE.Size = new System.Drawing.Size(75, 22);
            this.RECORD_BTN_ARCHIVE.TabIndex = 8;
            this.RECORD_BTN_ARCHIVE.Text = "Archive";
            this.RECORD_BTN_ARCHIVE.UseVisualStyleBackColor = true;
            // 
            // RECORD_BTN_CLIP_EXPORT
            // 
            this.RECORD_BTN_CLIP_EXPORT.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.RECORD_BTN_CLIP_EXPORT.Location = new System.Drawing.Point(498, 497);
            this.RECORD_BTN_CLIP_EXPORT.Name = "RECORD_BTN_CLIP_EXPORT";
            this.RECORD_BTN_CLIP_EXPORT.Size = new System.Drawing.Size(75, 22);
            this.RECORD_BTN_CLIP_EXPORT.TabIndex = 9;
            this.RECORD_BTN_CLIP_EXPORT.Text = "Clip Export";
            this.RECORD_BTN_CLIP_EXPORT.UseVisualStyleBackColor = true;
            // 
            // GRP_CALLBACK
            // 
            this.GRP_CALLBACK.Controls.Add(this.CALLBACK_BTN_STARTUP);
            this.GRP_CALLBACK.Controls.Add(this.CALLBACK_STC_PORT);
            this.GRP_CALLBACK.Controls.Add(this.CALLBACK_EDT_PORT);
            this.GRP_CALLBACK.Location = new System.Drawing.Point(10, 706);
            this.GRP_CALLBACK.Name = "GRP_CALLBACK";
            this.GRP_CALLBACK.Size = new System.Drawing.Size(142, 95);
            this.GRP_CALLBACK.TabIndex = 10;
            this.GRP_CALLBACK.TabStop = false;
            this.GRP_CALLBACK.Text = "Callback";
            // 
            // CALLBACK_BTN_STARTUP
            // 
            this.CALLBACK_BTN_STARTUP.Location = new System.Drawing.Point(49, 45);
            this.CALLBACK_BTN_STARTUP.Name = "CALLBACK_BTN_STARTUP";
            this.CALLBACK_BTN_STARTUP.Size = new System.Drawing.Size(80, 23);
            this.CALLBACK_BTN_STARTUP.TabIndex = 1;
            this.CALLBACK_BTN_STARTUP.Text = "Startup";
            this.CALLBACK_BTN_STARTUP.UseVisualStyleBackColor = true;
            this.CALLBACK_BTN_STARTUP.Click += new System.EventHandler(this.on_feature_callback_btn_startup);
            // 
            // CALLBACK_STC_PORT
            // 
            this.CALLBACK_STC_PORT.AutoSize = true;
            this.CALLBACK_STC_PORT.Location = new System.Drawing.Point(12, 22);
            this.CALLBACK_STC_PORT.Name = "CALLBACK_STC_PORT";
            this.CALLBACK_STC_PORT.Size = new System.Drawing.Size(34, 13);
            this.CALLBACK_STC_PORT.TabIndex = 2;
            this.CALLBACK_STC_PORT.Text = "Port :";
            this.CALLBACK_STC_PORT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CALLBACK_EDT_PORT
            // 
            this.CALLBACK_EDT_PORT.AllowSpace = false;
            this.CALLBACK_EDT_PORT.Location = new System.Drawing.Point(49, 19);
            this.CALLBACK_EDT_PORT.Name = "CALLBACK_EDT_PORT";
            this.CALLBACK_EDT_PORT.Size = new System.Drawing.Size(80, 20);
            this.CALLBACK_EDT_PORT.TabIndex = 0;
            // 
            // LSV_EVENT
            // 
            this.LSV_EVENT.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LSV_EVENT_COL_SITE,
            this.LSV_EVENT_COL_EVENT,
            this.LSV_EVENT_COL_DATA,
            this.LSV_EVENT_COL_TIME});
            this.LSV_EVENT.ForeColor = System.Drawing.SystemColors.WindowText;
            this.LSV_EVENT.FullRowSelect = true;
            this.LSV_EVENT.GridLines = true;
            this.LSV_EVENT.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LSV_EVENT.HideSelection = false;
            this.LSV_EVENT.Location = new System.Drawing.Point(158, 706);
            this.LSV_EVENT.MultiSelect = false;
            this.LSV_EVENT.Name = "LSV_EVENT";
            this.LSV_EVENT.ShowItemToolTips = true;
            this.LSV_EVENT.Size = new System.Drawing.Size(532, 95);
            this.LSV_EVENT.TabIndex = 14;
            this.LSV_EVENT.UseCompatibleStateImageBehavior = false;
            this.LSV_EVENT.View = System.Windows.Forms.View.Details;
            // 
            // LSV_EVENT_COL_SITE
            // 
            this.LSV_EVENT_COL_SITE.Text = "Site";
            this.LSV_EVENT_COL_SITE.Width = 120;
            // 
            // LSV_EVENT_COL_EVENT
            // 
            this.LSV_EVENT_COL_EVENT.Text = "Event";
            this.LSV_EVENT_COL_EVENT.Width = 120;
            // 
            // LSV_EVENT_COL_DATA
            // 
            this.LSV_EVENT_COL_DATA.Text = "Data";
            this.LSV_EVENT_COL_DATA.Width = 141;
            // 
            // LSV_EVENT_COL_TIME
            // 
            this.LSV_EVENT_COL_TIME.Text = "Time";
            this.LSV_EVENT_COL_TIME.Width = 130;
            // 
            // STC_STATUS
            // 
            this.STC_STATUS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.STC_STATUS.Location = new System.Drawing.Point(10, 10);
            this.STC_STATUS.Name = "STC_STATUS";
            this.STC_STATUS.Size = new System.Drawing.Size(680, 364);
            this.STC_STATUS.TabIndex = 11;
            this.STC_STATUS.Text = "status";
            this.STC_STATUS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.STC_STATUS.Visible = false;
            // 
            // BTN_REMOTE_SETUP
            // 
            this.BTN_REMOTE_SETUP.Enabled = false;
            this.BTN_REMOTE_SETUP.Location = new System.Drawing.Point(615, 497);
            this.BTN_REMOTE_SETUP.Name = "BTN_REMOTE_SETUP";
            this.BTN_REMOTE_SETUP.Size = new System.Drawing.Size(75, 23);
            this.BTN_REMOTE_SETUP.TabIndex = 2;
            this.BTN_REMOTE_SETUP.Text = "Setup";
            this.BTN_REMOTE_SETUP.UseVisualStyleBackColor = true;
            this.BTN_REMOTE_SETUP.Click += new System.EventHandler(this.on_btn_remote_setup);
            // 
            // form_status
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 812);
            this.Controls.Add(this.CHK_INSTANT_RECORD_STATUS);
            this.Controls.Add(this.LSV_INSTANT_RECORD);
            this.Controls.Add(this.BTN_REMOTE_SETUP);
            this.Controls.Add(this.STC_STATUS);
            this.Controls.Add(this.GRP_CALLBACK);
            this.Controls.Add(this.LSV_EVENT);
            this.Controls.Add(this.RECORD_BTN_CLIP_EXPORT);
            this.Controls.Add(this.RECORD_BTN_ARCHIVE);
            this.Controls.Add(this.RECORD_BTN_PLAYBACK);
            this.Controls.Add(this.RECORD_BTN_RECORDING);
            this.Controls.Add(this.LSV_HEALTH);
            this.Controls.Add(this.GRP_LOG);
            this.Controls.Add(this.LSV_LOG_EVENT);
            this.Controls.Add(this.LSV_LOG_SYSTEM);
            this.Controls.Add(this.BTN_DISCONNECT);
            this.Controls.Add(this.BTN_CONNECT);
            this.Controls.Add(this.CHK_RECORD_PANIC);
            this.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "form_status";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "GDK S/t/a/t/u/s";
            this.Load += new System.EventHandler(this.on_form_load);
            this.GRP_LOG.ResumeLayout(false);
            this.GRP_LOG.PerformLayout();
            this.GRP_CALLBACK.ResumeLayout(false);
            this.GRP_CALLBACK.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.Button BTN_CONNECT;
        private System.Windows.Forms.Button BTN_DISCONNECT;
        private System.Windows.Forms.ListView LSV_HEALTH;
        private System.Windows.Forms.ColumnHeader LSV_HEALTH_COL_STATUS;
        private System.Windows.Forms.ColumnHeader LSV_HEALTH_COL_PROBLEM;
        private System.Windows.Forms.ColumnHeader LSV_HEALTH_COL_MAC;
        private System.Windows.Forms.ColumnHeader LSV_HEALTH_COL_VERSION;
        private System.Windows.Forms.ColumnHeader LSV_HEALTH_COL_CAMERA;
        private System.Windows.Forms.ColumnHeader LSV_HEALTH_COL_SENSOR;
        private System.Windows.Forms.ColumnHeader LSV_HEALTH_COL_FAN;
        private System.Windows.Forms.ColumnHeader LSV_HEALTH_COL_REC_CHECK;
        private System.Windows.Forms.ColumnHeader LSV_HEALTH_COL_REC;
        private System.Windows.Forms.ColumnHeader LSV_HEALTH_COL_REC_PERIOD;
        private System.Windows.Forms.ListView LSV_INSTANT_RECORD;
        private System.Windows.Forms.ColumnHeader LSV_INSTANT_RECORD_COL_CHANNEL;
        private System.Windows.Forms.ColumnHeader LSV_INSTANT_RECORD_COL_DURATION;
        private System.Windows.Forms.ColumnHeader LSV_INSTANT_RECORD_COL_RECORD;
        private System.Windows.Forms.ColumnHeader LSV_INSTANT_RECORD_COL_SCOPE;
        private System.Windows.Forms.ColumnHeader LSV_INSTANT_RECORD_COL_FAIL_REASON;
        private System.Windows.Forms.GroupBox GRP_LOG;
        private System.Windows.Forms.Label LOG_STC_FROM;
        private System.Windows.Forms.Label LOG_STC_TO;
        private System.Windows.Forms.CheckBox LOG_CHK_FIRST;
        private System.Windows.Forms.CheckBox LOG_CHK_LAST;
        private System.Windows.Forms.DateTimePicker LOG_DTP_FROM;
        private System.Windows.Forms.DateTimePicker LOG_DTP_TO;
        private System.Windows.Forms.Button LOG_BTN_SYSTEM;
        private System.Windows.Forms.Button LOG_BTN_EVENT;
        private System.Windows.Forms.Button LOG_BTN_DEBUG;
        private System.Windows.Forms.Button LOG_BTN_SYSTEM_MORE;
        private System.Windows.Forms.Button LOG_BTN_EVENT_MORE;
        private System.Windows.Forms.Button LOG_BTN_DEBUG_MORE;
        private System.Windows.Forms.ListView LSV_LOG_SYSTEM;
        private System.Windows.Forms.ColumnHeader LSV_LOG_SYSTEM_COL_DATA;
        private System.Windows.Forms.ColumnHeader LSV_LOG_SYSTEM_COL_TIME;
        private System.Windows.Forms.ListView LSV_LOG_EVENT;
        private System.Windows.Forms.ColumnHeader LSV_LOG_EVENT_COL_EVENT;
        private System.Windows.Forms.ColumnHeader LSV_LOG_EVENT_COL_LABEL;
        private System.Windows.Forms.ColumnHeader LSV_LOG_EVENT_COL_TIME;
        private System.Windows.Forms.CheckBox CHK_INSTANT_RECORD_STATUS;
        private System.Windows.Forms.CheckBox CHK_RECORD_PANIC;
        private System.Windows.Forms.Button RECORD_BTN_RECORDING;
        private System.Windows.Forms.Button RECORD_BTN_PLAYBACK;
        private System.Windows.Forms.Button RECORD_BTN_ARCHIVE;
        private System.Windows.Forms.Button RECORD_BTN_CLIP_EXPORT;
        private System.Windows.Forms.GroupBox GRP_CALLBACK;
        private System.Windows.Forms.Label CALLBACK_STC_PORT;
        private System.Windows.Forms.Button CALLBACK_BTN_STARTUP;
        private System.Windows.Forms.ListView LSV_EVENT;
        private System.Windows.Forms.ColumnHeader LSV_EVENT_COL_SITE;
        private System.Windows.Forms.ColumnHeader LSV_EVENT_COL_EVENT;
        private System.Windows.Forms.ColumnHeader LSV_EVENT_COL_DATA;
        private System.Windows.Forms.ColumnHeader LSV_EVENT_COL_TIME;
        private System.Windows.Forms.Label STC_STATUS;
        private System.Windows.Forms.Button BTN_REMOTE_SETUP;
        private text_box_numeric CALLBACK_EDT_PORT;
    }
}
