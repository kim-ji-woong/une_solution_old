namespace GDK_tester
{
    partial class form_admin
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("All Device");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Device Group");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(form_admin));
            this.BTN_DISCONNECT = new System.Windows.Forms.Button();
            this.TRV_DEVICE_LIST = new System.Windows.Forms.TreeView();
            this.BTN_BACKUP = new System.Windows.Forms.Button();
            this.BTN_PLAY = new System.Windows.Forms.Button();
            this.BTN_LIVE = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CB_GET_TYPE = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TB_GETTED_IP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TB_GETTED_MAC = new System.Windows.Forms.TextBox();
            this.TB_GETTED_MODEL = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.TB_GETTED_NAME = new System.Windows.Forms.TextBox();
            this.TB_GETTED_GUID = new System.Windows.Forms.TextBox();
            this.BTN_GET_GUID = new System.Windows.Forms.Button();
            this.TB_IP_ADDRESS_GET_GUID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.DGV_Record_Device_Status = new System.Windows.Forms.DataGridView();
            this.RootName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CameraName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Connection = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OnRecording = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AudioOn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LB_INFO = new System.Windows.Forms.Label();
            this.BTN_CONNECT_FORM = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.TAB_SYSTEM = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.BTN_SERVICE_LOG = new System.Windows.Forms.Button();
            this.LB_SERVICES = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.BTN_ADMIN_LOG = new System.Windows.Forms.Button();
            this.CB_ADMIN = new System.Windows.Forms.ComboBox();
            this.TAB_REC_STATUS = new System.Windows.Forms.TabPage();
            this.TAB_EVENT = new System.Windows.Forms.TabPage();
            this.LSV_INSTANT_EVENT = new System.Windows.Forms.ListView();
            this.LSV_INSTANT_COL_EVENT_TYPE = new System.Windows.Forms.ColumnHeader();
            this.LSV_INSTANT_COL_EVENT = new System.Windows.Forms.ColumnHeader();
            this.LSV_INSTANT_COL_DEVICE = new System.Windows.Forms.ColumnHeader();
            this.LSV_INSTANT_COL_TIME = new System.Windows.Forms.ColumnHeader();
            this.TAB_HEALTH = new System.Windows.Forms.TabPage();
            this.LSV_DEVICE_HEALTH = new System.Windows.Forms.ListView();
            this.LSV_DEVICE_HEALTH_COL_STATUS = new System.Windows.Forms.ColumnHeader();
            this.LSV_DEVICE_HEALTH_COL_PROBLEM = new System.Windows.Forms.ColumnHeader();
            this.LSV_DEVICE_HEALTH_COL_GROUP = new System.Windows.Forms.ColumnHeader();
            this.LSV_DEVICE_HEALTH_COL_SITE = new System.Windows.Forms.ColumnHeader();
            this.LSV_DEVICE_HEALTH_COL_ADDRESS = new System.Windows.Forms.ColumnHeader();
            this.LSV_DEVICE_HEALTH_COL_MAC = new System.Windows.Forms.ColumnHeader();
            this.LSV_DEVICE_HEALTH_COL_VERSION = new System.Windows.Forms.ColumnHeader();
            this.LSV_DEVICE_HEALTH_COL_CAMERAS = new System.Windows.Forms.ColumnHeader();
            this.LSV_DEVICE_HEALTH_COL_ALARM_IN = new System.Windows.Forms.ColumnHeader();
            this.LSV_DEVICE_HEALTH_COL_FAN = new System.Windows.Forms.ColumnHeader();
            this.LSV_DEVICE_HEALTH_COL_RECORD = new System.Windows.Forms.ColumnHeader();
            this.LSV_DEVICE_HEALTH_COL_RECORD_CHECK = new System.Windows.Forms.ColumnHeader();
            this.LSV_DEVICE_HEALTH_COL_RECORD_PERIOD = new System.Windows.Forms.ColumnHeader();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LB_TODO_LOG = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Record_Device_Status)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.TAB_SYSTEM.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.TAB_REC_STATUS.SuspendLayout();
            this.TAB_EVENT.SuspendLayout();
            this.TAB_HEALTH.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BTN_DISCONNECT
            // 
            this.BTN_DISCONNECT.Enabled = false;
            this.BTN_DISCONNECT.Location = new System.Drawing.Point(83, 7);
            this.BTN_DISCONNECT.Name = "BTN_DISCONNECT";
            this.BTN_DISCONNECT.Size = new System.Drawing.Size(68, 25);
            this.BTN_DISCONNECT.TabIndex = 6;
            this.BTN_DISCONNECT.Text = "Disconnect";
            this.BTN_DISCONNECT.UseVisualStyleBackColor = true;
            this.BTN_DISCONNECT.Click += new System.EventHandler(this.on_btn_disconnect);
            // 
            // TRV_DEVICE_LIST
            // 
            this.TRV_DEVICE_LIST.CheckBoxes = true;
            this.TRV_DEVICE_LIST.Enabled = false;
            this.TRV_DEVICE_LIST.Location = new System.Drawing.Point(3, 3);
            this.TRV_DEVICE_LIST.Name = "TRV_DEVICE_LIST";
            treeNode1.ImageIndex = 0;
            treeNode1.Name = "AllDevice";
            treeNode1.Text = "All Device";
            treeNode2.ImageIndex = 1;
            treeNode2.Name = "DeviceGroup";
            treeNode2.Text = "Device Group";
            this.TRV_DEVICE_LIST.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            this.TRV_DEVICE_LIST.Size = new System.Drawing.Size(225, 487);
            this.TRV_DEVICE_LIST.TabIndex = 25;
            this.TRV_DEVICE_LIST.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.on_check_device_list_item);
            this.TRV_DEVICE_LIST.Click += new System.EventHandler(this.on_device_list_click);
            // 
            // BTN_BACKUP
            // 
            this.BTN_BACKUP.Enabled = false;
            this.BTN_BACKUP.Location = new System.Drawing.Point(654, 7);
            this.BTN_BACKUP.Name = "BTN_BACKUP";
            this.BTN_BACKUP.Size = new System.Drawing.Size(64, 25);
            this.BTN_BACKUP.TabIndex = 3;
            this.BTN_BACKUP.Text = "Backup";
            this.BTN_BACKUP.UseVisualStyleBackColor = true;
            this.BTN_BACKUP.Click += new System.EventHandler(this.on_btn_backup);
            // 
            // BTN_PLAY
            // 
            this.BTN_PLAY.Enabled = false;
            this.BTN_PLAY.Location = new System.Drawing.Point(584, 7);
            this.BTN_PLAY.Name = "BTN_PLAY";
            this.BTN_PLAY.Size = new System.Drawing.Size(64, 25);
            this.BTN_PLAY.TabIndex = 1;
            this.BTN_PLAY.Text = "Play";
            this.BTN_PLAY.UseVisualStyleBackColor = true;
            this.BTN_PLAY.Click += new System.EventHandler(this.on_btn_play);
            // 
            // BTN_LIVE
            // 
            this.BTN_LIVE.Enabled = false;
            this.BTN_LIVE.Location = new System.Drawing.Point(514, 7);
            this.BTN_LIVE.Name = "BTN_LIVE";
            this.BTN_LIVE.Size = new System.Drawing.Size(64, 25);
            this.BTN_LIVE.TabIndex = 0;
            this.BTN_LIVE.Text = "Live";
            this.BTN_LIVE.UseVisualStyleBackColor = true;
            this.BTN_LIVE.Click += new System.EventHandler(this.on_btn_live);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.CB_GET_TYPE);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.BTN_GET_GUID);
            this.groupBox1.Controls.Add(this.TB_IP_ADDRESS_GET_GUID);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(325, 62);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(251, 385);
            this.groupBox1.TabIndex = 28;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Get GUID";
            // 
            // CB_GET_TYPE
            // 
            this.CB_GET_TYPE.Enabled = false;
            this.CB_GET_TYPE.FormattingEnabled = true;
            this.CB_GET_TYPE.Location = new System.Drawing.Point(81, 26);
            this.CB_GET_TYPE.Name = "CB_GET_TYPE";
            this.CB_GET_TYPE.Size = new System.Drawing.Size(158, 21);
            this.CB_GET_TYPE.TabIndex = 28;
            this.CB_GET_TYPE.SelectedIndexChanged += new System.EventHandler(this.selected_get_guid_type);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 29);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(63, 13);
            this.label10.TabIndex = 27;
            this.label10.Text = "Select Type";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.TB_GETTED_IP);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.TB_GETTED_MAC);
            this.groupBox4.Controls.Add(this.TB_GETTED_MODEL);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.TB_GETTED_NAME);
            this.groupBox4.Controls.Add(this.TB_GETTED_GUID);
            this.groupBox4.Location = new System.Drawing.Point(8, 138);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(236, 174);
            this.groupBox4.TabIndex = 26;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Result";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 141);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "IP :";
            // 
            // TB_GETTED_IP
            // 
            this.TB_GETTED_IP.Enabled = false;
            this.TB_GETTED_IP.Location = new System.Drawing.Point(52, 138);
            this.TB_GETTED_IP.Name = "TB_GETTED_IP";
            this.TB_GETTED_IP.Size = new System.Drawing.Size(177, 20);
            this.TB_GETTED_IP.TabIndex = 21;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 115);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "MAC :";
            // 
            // TB_GETTED_MAC
            // 
            this.TB_GETTED_MAC.Enabled = false;
            this.TB_GETTED_MAC.Location = new System.Drawing.Point(52, 112);
            this.TB_GETTED_MAC.Name = "TB_GETTED_MAC";
            this.TB_GETTED_MAC.Size = new System.Drawing.Size(177, 20);
            this.TB_GETTED_MAC.TabIndex = 19;
            // 
            // TB_GETTED_MODEL
            // 
            this.TB_GETTED_MODEL.Enabled = false;
            this.TB_GETTED_MODEL.Location = new System.Drawing.Point(52, 34);
            this.TB_GETTED_MODEL.Name = "TB_GETTED_MODEL";
            this.TB_GETTED_MODEL.Size = new System.Drawing.Size(177, 20);
            this.TB_GETTED_MODEL.TabIndex = 18;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 37);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(42, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Model :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 63);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Name :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 89);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "GUID :";
            // 
            // TB_GETTED_NAME
            // 
            this.TB_GETTED_NAME.Enabled = false;
            this.TB_GETTED_NAME.Location = new System.Drawing.Point(52, 60);
            this.TB_GETTED_NAME.Name = "TB_GETTED_NAME";
            this.TB_GETTED_NAME.Size = new System.Drawing.Size(177, 20);
            this.TB_GETTED_NAME.TabIndex = 16;
            // 
            // TB_GETTED_GUID
            // 
            this.TB_GETTED_GUID.Enabled = false;
            this.TB_GETTED_GUID.Location = new System.Drawing.Point(52, 86);
            this.TB_GETTED_GUID.Name = "TB_GETTED_GUID";
            this.TB_GETTED_GUID.Size = new System.Drawing.Size(177, 20);
            this.TB_GETTED_GUID.TabIndex = 13;
            // 
            // BTN_GET_GUID
            // 
            this.BTN_GET_GUID.Enabled = false;
            this.BTN_GET_GUID.Location = new System.Drawing.Point(182, 93);
            this.BTN_GET_GUID.Name = "BTN_GET_GUID";
            this.BTN_GET_GUID.Size = new System.Drawing.Size(57, 25);
            this.BTN_GET_GUID.TabIndex = 8;
            this.BTN_GET_GUID.Text = "Get";
            this.BTN_GET_GUID.UseVisualStyleBackColor = true;
            this.BTN_GET_GUID.Click += new System.EventHandler(this.on_btn_get_guid);
            // 
            // TB_IP_ADDRESS_GET_GUID
            // 
            this.TB_IP_ADDRESS_GET_GUID.Location = new System.Drawing.Point(62, 67);
            this.TB_IP_ADDRESS_GET_GUID.Name = "TB_IP_ADDRESS_GET_GUID";
            this.TB_IP_ADDRESS_GET_GUID.Size = new System.Drawing.Size(177, 20);
            this.TB_IP_ADDRESS_GET_GUID.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 70);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Input :";
            // 
            // DGV_Record_Device_Status
            // 
            this.DGV_Record_Device_Status.AllowUserToAddRows = false;
            this.DGV_Record_Device_Status.AllowUserToDeleteRows = false;
            this.DGV_Record_Device_Status.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_Record_Device_Status.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.RootName,
            this.CameraName,
            this.Connection,
            this.OnRecording,
            this.AudioOn,
            this.Type});
            this.DGV_Record_Device_Status.Enabled = false;
            this.DGV_Record_Device_Status.Location = new System.Drawing.Point(3, 4);
            this.DGV_Record_Device_Status.Name = "DGV_Record_Device_Status";
            this.DGV_Record_Device_Status.ReadOnly = true;
            this.DGV_Record_Device_Status.RowTemplate.Height = 23;
            this.DGV_Record_Device_Status.Size = new System.Drawing.Size(581, 452);
            this.DGV_Record_Device_Status.TabIndex = 0;
            // 
            // RootName
            // 
            this.RootName.HeaderText = "Root Name";
            this.RootName.Name = "RootName";
            this.RootName.ReadOnly = true;
            // 
            // CameraName
            // 
            this.CameraName.HeaderText = "Cam Name";
            this.CameraName.Name = "CameraName";
            this.CameraName.ReadOnly = true;
            // 
            // Connection
            // 
            this.Connection.HeaderText = "Connection";
            this.Connection.Name = "Connection";
            this.Connection.ReadOnly = true;
            // 
            // OnRecording
            // 
            this.OnRecording.HeaderText = "Record";
            this.OnRecording.Name = "OnRecording";
            this.OnRecording.ReadOnly = true;
            // 
            // AudioOn
            // 
            this.AudioOn.FillWeight = 48F;
            this.AudioOn.HeaderText = "Audio";
            this.AudioOn.Name = "AudioOn";
            this.AudioOn.ReadOnly = true;
            this.AudioOn.Width = 48;
            // 
            // Type
            // 
            this.Type.FillWeight = 88F;
            this.Type.HeaderText = "Type";
            this.Type.Name = "Type";
            this.Type.ReadOnly = true;
            this.Type.Width = 88;
            // 
            // LB_INFO
            // 
            this.LB_INFO.Location = new System.Drawing.Point(161, 7);
            this.LB_INFO.Name = "LB_INFO";
            this.LB_INFO.Size = new System.Drawing.Size(338, 25);
            this.LB_INFO.TabIndex = 32;
            this.LB_INFO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BTN_CONNECT_FORM
            // 
            this.BTN_CONNECT_FORM.Location = new System.Drawing.Point(9, 7);
            this.BTN_CONNECT_FORM.Name = "BTN_CONNECT_FORM";
            this.BTN_CONNECT_FORM.Size = new System.Drawing.Size(68, 25);
            this.BTN_CONNECT_FORM.TabIndex = 8;
            this.BTN_CONNECT_FORM.Text = "Connect";
            this.BTN_CONNECT_FORM.UseVisualStyleBackColor = true;
            this.BTN_CONNECT_FORM.Click += new System.EventHandler(this.on_btn_connect_form);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Location = new System.Drawing.Point(13, 62);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.TRV_DEVICE_LIST);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(846, 497);
            this.splitContainer1.SplitterDistance = 235;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 33;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.TAB_SYSTEM);
            this.tabControl1.Controls.Add(this.TAB_REC_STATUS);
            this.tabControl1.Controls.Add(this.TAB_EVENT);
            this.tabControl1.Controls.Add(this.TAB_HEALTH);
            this.tabControl1.Location = new System.Drawing.Point(4, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(596, 487);
            this.tabControl1.TabIndex = 0;
            // 
            // TAB_SYSTEM
            // 
            this.TAB_SYSTEM.Controls.Add(this.panel2);
            this.TAB_SYSTEM.Location = new System.Drawing.Point(4, 22);
            this.TAB_SYSTEM.Name = "TAB_SYSTEM";
            this.TAB_SYSTEM.Padding = new System.Windows.Forms.Padding(3);
            this.TAB_SYSTEM.Size = new System.Drawing.Size(588, 461);
            this.TAB_SYSTEM.TabIndex = 0;
            this.TAB_SYSTEM.Text = "System";
            this.TAB_SYSTEM.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.groupBox5);
            this.panel2.Controls.Add(this.groupBox3);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Location = new System.Drawing.Point(3, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(581, 452);
            this.panel2.TabIndex = 0;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.LB_TODO_LOG);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.BTN_SERVICE_LOG);
            this.groupBox5.Controls.Add(this.LB_SERVICES);
            this.groupBox5.Location = new System.Drawing.Point(3, 62);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(316, 385);
            this.groupBox5.TabIndex = 31;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Connectable Services";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(216, 24);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(86, 13);
            this.label9.TabIndex = 32;
            this.label9.Text = "Selected Service";
            // 
            // BTN_SERVICE_LOG
            // 
            this.BTN_SERVICE_LOG.Enabled = false;
            this.BTN_SERVICE_LOG.Location = new System.Drawing.Point(215, 40);
            this.BTN_SERVICE_LOG.Name = "BTN_SERVICE_LOG";
            this.BTN_SERVICE_LOG.Size = new System.Drawing.Size(75, 23);
            this.BTN_SERVICE_LOG.TabIndex = 31;
            this.BTN_SERVICE_LOG.Text = "View Log";
            this.BTN_SERVICE_LOG.UseVisualStyleBackColor = true;
            this.BTN_SERVICE_LOG.Click += new System.EventHandler(this.on_btn_service_log);
            // 
            // LB_SERVICES
            // 
            this.LB_SERVICES.FormattingEnabled = true;
            this.LB_SERVICES.Location = new System.Drawing.Point(6, 22);
            this.LB_SERVICES.Name = "LB_SERVICES";
            this.LB_SERVICES.Size = new System.Drawing.Size(203, 355);
            this.LB_SERVICES.TabIndex = 30;
            this.LB_SERVICES.SelectedIndexChanged += new System.EventHandler(this.selected_connectable_service);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.BTN_ADMIN_LOG);
            this.groupBox3.Controls.Add(this.CB_ADMIN);
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(473, 53);
            this.groupBox3.TabIndex = 29;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Select Admin Service";
            // 
            // BTN_ADMIN_LOG
            // 
            this.BTN_ADMIN_LOG.Enabled = false;
            this.BTN_ADMIN_LOG.Location = new System.Drawing.Point(351, 18);
            this.BTN_ADMIN_LOG.Name = "BTN_ADMIN_LOG";
            this.BTN_ADMIN_LOG.Size = new System.Drawing.Size(96, 23);
            this.BTN_ADMIN_LOG.TabIndex = 1;
            this.BTN_ADMIN_LOG.Text = "Admin Log";
            this.BTN_ADMIN_LOG.UseVisualStyleBackColor = true;
            this.BTN_ADMIN_LOG.Click += new System.EventHandler(this.on_btn_admin_log);
            // 
            // CB_ADMIN
            // 
            this.CB_ADMIN.Enabled = false;
            this.CB_ADMIN.FormattingEnabled = true;
            this.CB_ADMIN.Location = new System.Drawing.Point(23, 20);
            this.CB_ADMIN.Name = "CB_ADMIN";
            this.CB_ADMIN.Size = new System.Drawing.Size(308, 21);
            this.CB_ADMIN.TabIndex = 0;
            // 
            // TAB_REC_STATUS
            // 
            this.TAB_REC_STATUS.Controls.Add(this.DGV_Record_Device_Status);
            this.TAB_REC_STATUS.Location = new System.Drawing.Point(4, 22);
            this.TAB_REC_STATUS.Name = "TAB_REC_STATUS";
            this.TAB_REC_STATUS.Padding = new System.Windows.Forms.Padding(3);
            this.TAB_REC_STATUS.Size = new System.Drawing.Size(588, 461);
            this.TAB_REC_STATUS.TabIndex = 1;
            this.TAB_REC_STATUS.Text = "Record Status";
            this.TAB_REC_STATUS.UseVisualStyleBackColor = true;
            // 
            // TAB_EVENT
            // 
            this.TAB_EVENT.BackColor = System.Drawing.Color.Transparent;
            this.TAB_EVENT.Controls.Add(this.LSV_INSTANT_EVENT);
            this.TAB_EVENT.Location = new System.Drawing.Point(4, 22);
            this.TAB_EVENT.Name = "TAB_EVENT";
            this.TAB_EVENT.Size = new System.Drawing.Size(588, 461);
            this.TAB_EVENT.TabIndex = 2;
            this.TAB_EVENT.Text = "Monitor Event";
            this.TAB_EVENT.UseVisualStyleBackColor = true;
            // 
            // LSV_INSTANT_EVENT
            // 
            this.LSV_INSTANT_EVENT.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.LSV_INSTANT_EVENT.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LSV_INSTANT_COL_EVENT_TYPE,
            this.LSV_INSTANT_COL_EVENT,
            this.LSV_INSTANT_COL_DEVICE,
            this.LSV_INSTANT_COL_TIME});
            this.LSV_INSTANT_EVENT.FullRowSelect = true;
            this.LSV_INSTANT_EVENT.GridLines = true;
            this.LSV_INSTANT_EVENT.HideSelection = false;
            this.LSV_INSTANT_EVENT.HoverSelection = true;
            this.LSV_INSTANT_EVENT.Location = new System.Drawing.Point(9, 12);
            this.LSV_INSTANT_EVENT.MultiSelect = false;
            this.LSV_INSTANT_EVENT.Name = "LSV_INSTANT_EVENT";
            this.LSV_INSTANT_EVENT.ShowItemToolTips = true;
            this.LSV_INSTANT_EVENT.Size = new System.Drawing.Size(570, 439);
            this.LSV_INSTANT_EVENT.TabIndex = 10;
            this.LSV_INSTANT_EVENT.UseCompatibleStateImageBehavior = false;
            this.LSV_INSTANT_EVENT.View = System.Windows.Forms.View.Details;
            // 
            // LSV_INSTANT_COL_EVENT_TYPE
            // 
            this.LSV_INSTANT_COL_EVENT_TYPE.Text = "EventType";
            this.LSV_INSTANT_COL_EVENT_TYPE.Width = 138;
            // 
            // LSV_INSTANT_COL_EVENT
            // 
            this.LSV_INSTANT_COL_EVENT.Text = "Event";
            this.LSV_INSTANT_COL_EVENT.Width = 138;
            // 
            // LSV_INSTANT_COL_DEVICE
            // 
            this.LSV_INSTANT_COL_DEVICE.Text = "Device";
            this.LSV_INSTANT_COL_DEVICE.Width = 142;
            // 
            // LSV_INSTANT_COL_TIME
            // 
            this.LSV_INSTANT_COL_TIME.Text = "Time";
            this.LSV_INSTANT_COL_TIME.Width = 142;
            // 
            // TAB_HEALTH
            // 
            this.TAB_HEALTH.BackColor = System.Drawing.Color.Transparent;
            this.TAB_HEALTH.Controls.Add(this.LSV_DEVICE_HEALTH);
            this.TAB_HEALTH.Location = new System.Drawing.Point(4, 22);
            this.TAB_HEALTH.Name = "TAB_HEALTH";
            this.TAB_HEALTH.Size = new System.Drawing.Size(588, 461);
            this.TAB_HEALTH.TabIndex = 3;
            this.TAB_HEALTH.Text = "Monitor Device Health";
            this.TAB_HEALTH.UseVisualStyleBackColor = true;
            // 
            // LSV_DEVICE_HEALTH
            // 
            this.LSV_DEVICE_HEALTH.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LSV_DEVICE_HEALTH_COL_STATUS,
            this.LSV_DEVICE_HEALTH_COL_PROBLEM,
            this.LSV_DEVICE_HEALTH_COL_GROUP,
            this.LSV_DEVICE_HEALTH_COL_SITE,
            this.LSV_DEVICE_HEALTH_COL_ADDRESS,
            this.LSV_DEVICE_HEALTH_COL_MAC,
            this.LSV_DEVICE_HEALTH_COL_VERSION,
            this.LSV_DEVICE_HEALTH_COL_CAMERAS,
            this.LSV_DEVICE_HEALTH_COL_ALARM_IN,
            this.LSV_DEVICE_HEALTH_COL_FAN,
            this.LSV_DEVICE_HEALTH_COL_RECORD,
            this.LSV_DEVICE_HEALTH_COL_RECORD_CHECK,
            this.LSV_DEVICE_HEALTH_COL_RECORD_PERIOD});
            this.LSV_DEVICE_HEALTH.Enabled = false;
            this.LSV_DEVICE_HEALTH.FullRowSelect = true;
            this.LSV_DEVICE_HEALTH.GridLines = true;
            this.LSV_DEVICE_HEALTH.HideSelection = false;
            this.LSV_DEVICE_HEALTH.HoverSelection = true;
            this.LSV_DEVICE_HEALTH.Location = new System.Drawing.Point(9, 12);
            this.LSV_DEVICE_HEALTH.MultiSelect = false;
            this.LSV_DEVICE_HEALTH.Name = "LSV_DEVICE_HEALTH";
            this.LSV_DEVICE_HEALTH.ShowItemToolTips = true;
            this.LSV_DEVICE_HEALTH.Size = new System.Drawing.Size(570, 439);
            this.LSV_DEVICE_HEALTH.TabIndex = 12;
            this.LSV_DEVICE_HEALTH.UseCompatibleStateImageBehavior = false;
            this.LSV_DEVICE_HEALTH.View = System.Windows.Forms.View.Details;
            // 
            // LSV_DEVICE_HEALTH_COL_STATUS
            // 
            this.LSV_DEVICE_HEALTH_COL_STATUS.Text = "Status";
            // 
            // LSV_DEVICE_HEALTH_COL_PROBLEM
            // 
            this.LSV_DEVICE_HEALTH_COL_PROBLEM.Text = "Problem";
            // 
            // LSV_DEVICE_HEALTH_COL_GROUP
            // 
            this.LSV_DEVICE_HEALTH_COL_GROUP.Text = "Group";
            // 
            // LSV_DEVICE_HEALTH_COL_SITE
            // 
            this.LSV_DEVICE_HEALTH_COL_SITE.Text = "Site";
            // 
            // LSV_DEVICE_HEALTH_COL_ADDRESS
            // 
            this.LSV_DEVICE_HEALTH_COL_ADDRESS.Text = "Address";
            // 
            // LSV_DEVICE_HEALTH_COL_MAC
            // 
            this.LSV_DEVICE_HEALTH_COL_MAC.Text = "MAC";
            // 
            // LSV_DEVICE_HEALTH_COL_VERSION
            // 
            this.LSV_DEVICE_HEALTH_COL_VERSION.Text = "Version";
            // 
            // LSV_DEVICE_HEALTH_COL_CAMERAS
            // 
            this.LSV_DEVICE_HEALTH_COL_CAMERAS.Text = "Cameras";
            // 
            // LSV_DEVICE_HEALTH_COL_ALARM_IN
            // 
            this.LSV_DEVICE_HEALTH_COL_ALARM_IN.Text = "Alarm-In";
            // 
            // LSV_DEVICE_HEALTH_COL_FAN
            // 
            this.LSV_DEVICE_HEALTH_COL_FAN.Text = "Fan";
            // 
            // LSV_DEVICE_HEALTH_COL_RECORD
            // 
            this.LSV_DEVICE_HEALTH_COL_RECORD.Text = "Record";
            // 
            // LSV_DEVICE_HEALTH_COL_RECORD_CHECK
            // 
            this.LSV_DEVICE_HEALTH_COL_RECORD_CHECK.Text = "Record Check";
            // 
            // LSV_DEVICE_HEALTH_COL_RECORD_PERIOD
            // 
            this.LSV_DEVICE_HEALTH_COL_RECORD_PERIOD.Text = "Record Period";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.BTN_DISCONNECT);
            this.panel1.Controls.Add(this.BTN_BACKUP);
            this.panel1.Controls.Add(this.BTN_CONNECT_FORM);
            this.panel1.Controls.Add(this.LB_INFO);
            this.panel1.Controls.Add(this.BTN_LIVE);
            this.panel1.Controls.Add(this.BTN_PLAY);
            this.panel1.Location = new System.Drawing.Point(13, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(846, 43);
            this.panel1.TabIndex = 34;
            // 
            // LB_TODO_LOG
            // 
            this.LB_TODO_LOG.Location = new System.Drawing.Point(216, 74);
            this.LB_TODO_LOG.Name = "LB_TODO_LOG";
            this.LB_TODO_LOG.Size = new System.Drawing.Size(86, 114);
            this.LB_TODO_LOG.TabIndex = 33;
            // 
            // form_admin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 570);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Tahoma", 8F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "form_admin";
            this.Text = "GDK Service A/d/m/i/n";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.on_form_closing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Record_Device_Status)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.TAB_SYSTEM.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.TAB_REC_STATUS.ResumeLayout(false);
            this.TAB_EVENT.ResumeLayout(false);
            this.TAB_HEALTH.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BTN_DISCONNECT;
        private System.Windows.Forms.TreeView TRV_DEVICE_LIST;
        private System.Windows.Forms.Button BTN_LIVE;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox TB_GETTED_GUID;
        private System.Windows.Forms.Button BTN_GET_GUID;
        private System.Windows.Forms.TextBox TB_IP_ADDRESS_GET_GUID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button BTN_BACKUP;
        private System.Windows.Forms.Button BTN_PLAY;
        private System.Windows.Forms.TextBox TB_GETTED_NAME;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox TB_GETTED_MODEL;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.DataGridView DGV_Record_Device_Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn RootName;
        private System.Windows.Forms.DataGridViewTextBoxColumn CameraName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Connection;
        private System.Windows.Forms.DataGridViewTextBoxColumn OnRecording;
        private System.Windows.Forms.DataGridViewTextBoxColumn AudioOn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.Label LB_INFO;
        private System.Windows.Forms.Button BTN_CONNECT_FORM;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage TAB_SYSTEM;
        private System.Windows.Forms.TabPage TAB_REC_STATUS;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabPage TAB_EVENT;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ListBox LB_SERVICES;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button BTN_ADMIN_LOG;
        private System.Windows.Forms.ComboBox CB_ADMIN;
        private System.Windows.Forms.Button BTN_SERVICE_LOG;
        private System.Windows.Forms.ComboBox CB_GET_TYPE;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TB_GETTED_MAC;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TB_GETTED_IP;
        private System.Windows.Forms.ListView LSV_INSTANT_EVENT;
        private System.Windows.Forms.ColumnHeader LSV_INSTANT_COL_EVENT_TYPE;
        private System.Windows.Forms.ColumnHeader LSV_INSTANT_COL_EVENT;
        private System.Windows.Forms.ColumnHeader LSV_INSTANT_COL_DEVICE;
        private System.Windows.Forms.ColumnHeader LSV_INSTANT_COL_TIME;
        private System.Windows.Forms.TabPage TAB_HEALTH;
        private System.Windows.Forms.ListView LSV_DEVICE_HEALTH;
        private System.Windows.Forms.ColumnHeader LSV_DEVICE_HEALTH_COL_STATUS;
        private System.Windows.Forms.ColumnHeader LSV_DEVICE_HEALTH_COL_PROBLEM;
        private System.Windows.Forms.ColumnHeader LSV_DEVICE_HEALTH_COL_GROUP;
        private System.Windows.Forms.ColumnHeader LSV_DEVICE_HEALTH_COL_SITE;
        private System.Windows.Forms.ColumnHeader LSV_DEVICE_HEALTH_COL_ADDRESS;
        private System.Windows.Forms.ColumnHeader LSV_DEVICE_HEALTH_COL_MAC;
        private System.Windows.Forms.ColumnHeader LSV_DEVICE_HEALTH_COL_VERSION;
        private System.Windows.Forms.ColumnHeader LSV_DEVICE_HEALTH_COL_CAMERAS;
        private System.Windows.Forms.ColumnHeader LSV_DEVICE_HEALTH_COL_ALARM_IN;
        private System.Windows.Forms.ColumnHeader LSV_DEVICE_HEALTH_COL_FAN;
        private System.Windows.Forms.ColumnHeader LSV_DEVICE_HEALTH_COL_RECORD;
        private System.Windows.Forms.ColumnHeader LSV_DEVICE_HEALTH_COL_RECORD_CHECK;
        private System.Windows.Forms.ColumnHeader LSV_DEVICE_HEALTH_COL_RECORD_PERIOD;
        private System.Windows.Forms.Label LB_TODO_LOG;
    }
}

