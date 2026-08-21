namespace SDMS.PopupDialog
{
    partial class FormPSMList
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPSMList));
            this.rdoTankList = new System.Windows.Forms.RadioButton();
            this.rdoSensorList = new System.Windows.Forms.RadioButton();
            this.panelGuide = new System.Windows.Forms.Panel();
            this.gridGuide = new System.Windows.Forms.DataGridView();
            this.col1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnGuide = new System.Windows.Forms.Button();
            this.lblOnOff = new System.Windows.Forms.Label();
            this.lblLoacation = new System.Windows.Forms.Label();
            this.lblInOut = new System.Windows.Forms.Label();
            this.lblPSMMaterial = new System.Windows.Forms.Label();
            this.gvPSMSensor = new System.Windows.Forms.DataGridView();
            this.colPSMSensorNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPSMSensorLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPSMSensorMaterialName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPSMSensorCurrentOverflow = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAlarmDepth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCCTV = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colPSMSensorIsWorking = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gvPSMTank = new System.Windows.Forms.DataGridView();
            this.colPSMTankIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPSMTankLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTankName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTankAlarm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPSMTankRemains = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTankCCTV = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colPSMTankMaterialName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.dataGridViewButtonColumn1 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.dataGridViewButtonColumn2 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.popupMenuSensor = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuShowSensorChart = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSensorOnOff = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSensorOn = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSensorOff = new System.Windows.Forms.ToolStripMenuItem();
            this.menuShowSensorLifeTime = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDepartment = new System.Windows.Forms.ToolStripMenuItem();
            this.menuShowSensorCCTV = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditSensorAlarm = new System.Windows.Forms.ToolStripMenuItem();
            this.popupMenuTank = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuShowTankDetail = new System.Windows.Forms.ToolStripMenuItem();
            this.menuShowTankCCTV = new System.Windows.Forms.ToolStripMenuItem();
            this.btnShowSensorManual = new UnE.GUI.ImageButton();
            this.btnSearch = new UnE.GUI.ImageButton();
            this.cmbPSMMaterial = new UnE.GUI.ImageComboBox();
            this.cmbInOut = new UnE.GUI.ImageComboBox();
            this.cmbLocation = new UnE.GUI.ImageComboBox();
            this.cmbOnOff = new UnE.GUI.ImageComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnChk12 = new UnE.GUI.ImageButton();
            this.chk12Text = new System.Windows.Forms.Label();
            this.chk34Text = new System.Windows.Forms.Label();
            this.btnChk34 = new UnE.GUI.ImageButton();
            this.chk56Text = new System.Windows.Forms.Label();
            this.btnChk56 = new UnE.GUI.ImageButton();
            this.chkWaterText = new System.Windows.Forms.Label();
            this.btnChkWater = new UnE.GUI.ImageButton();
            this.chkETCText = new System.Windows.Forms.Label();
            this.btnChkETC = new UnE.GUI.ImageButton();
            this.panelGuide.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridGuide)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvPSMSensor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvPSMTank)).BeginInit();
            this.popupMenuSensor.SuspendLayout();
            this.popupMenuTank.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnShowSensorManual)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnChk12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnChk34)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnChk56)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnChkWater)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnChkETC)).BeginInit();
            this.SuspendLayout();
            // 
            // rdoTankList
            // 
            this.rdoTankList.AutoSize = true;
            this.rdoTankList.Font = new System.Drawing.Font("굴림", 12.5F);
            this.rdoTankList.ForeColor = System.Drawing.Color.White;
            this.rdoTankList.Location = new System.Drawing.Point(243, 14);
            this.rdoTankList.Name = "rdoTankList";
            this.rdoTankList.Size = new System.Drawing.Size(138, 21);
            this.rdoTankList.TabIndex = 0;
            this.rdoTankList.TabStop = true;
            this.rdoTankList.Text = "탱크 목록 보기";
            this.rdoTankList.UseVisualStyleBackColor = true;
            // 
            // rdoSensorList
            // 
            this.rdoSensorList.AutoSize = true;
            this.rdoSensorList.Font = new System.Drawing.Font("굴림", 12.5F);
            this.rdoSensorList.ForeColor = System.Drawing.Color.White;
            this.rdoSensorList.Location = new System.Drawing.Point(382, 14);
            this.rdoSensorList.Name = "rdoSensorList";
            this.rdoSensorList.Size = new System.Drawing.Size(138, 21);
            this.rdoSensorList.TabIndex = 1;
            this.rdoSensorList.TabStop = true;
            this.rdoSensorList.Text = "센서 목록 보기";
            this.rdoSensorList.UseVisualStyleBackColor = true;
            // 
            // panelGuide
            // 
            this.panelGuide.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelGuide.Controls.Add(this.gridGuide);
            this.panelGuide.Controls.Add(this.btnGuide);
            this.panelGuide.Location = new System.Drawing.Point(6, 8);
            this.panelGuide.Name = "panelGuide";
            this.panelGuide.Size = new System.Drawing.Size(220, 38);
            this.panelGuide.TabIndex = 3;
            // 
            // gridGuide
            // 
            this.gridGuide.AllowUserToAddRows = false;
            this.gridGuide.AllowUserToDeleteRows = false;
            this.gridGuide.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridGuide.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridGuide.ColumnHeadersVisible = false;
            this.gridGuide.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col1,
            this.col2,
            this.Column3});
            this.gridGuide.Enabled = false;
            this.gridGuide.Location = new System.Drawing.Point(24, 0);
            this.gridGuide.Name = "gridGuide";
            this.gridGuide.RowHeadersVisible = false;
            this.gridGuide.RowTemplate.Height = 23;
            this.gridGuide.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.gridGuide.Size = new System.Drawing.Size(196, 38);
            this.gridGuide.TabIndex = 1;
            // 
            // col1
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.col1.DefaultCellStyle = dataGridViewCellStyle1;
            this.col1.HeaderText = "Column1";
            this.col1.Name = "col1";
            // 
            // col2
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.col2.DefaultCellStyle = dataGridViewCellStyle2;
            this.col2.HeaderText = "Column2";
            this.col2.Name = "col2";
            // 
            // Column3
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column3.DefaultCellStyle = dataGridViewCellStyle3;
            this.Column3.HeaderText = "Column3";
            this.Column3.Name = "Column3";
            // 
            // btnGuide
            // 
            this.btnGuide.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnGuide.Enabled = false;
            this.btnGuide.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuide.Location = new System.Drawing.Point(0, 0);
            this.btnGuide.Name = "btnGuide";
            this.btnGuide.Size = new System.Drawing.Size(24, 38);
            this.btnGuide.TabIndex = 0;
            this.btnGuide.Text = "범례";
            this.btnGuide.UseVisualStyleBackColor = false;
            // 
            // lblOnOff
            // 
            this.lblOnOff.AutoSize = true;
            this.lblOnOff.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblOnOff.ForeColor = System.Drawing.Color.White;
            this.lblOnOff.Location = new System.Drawing.Point(461, 57);
            this.lblOnOff.Name = "lblOnOff";
            this.lblOnOff.Size = new System.Drawing.Size(120, 17);
            this.lblOnOff.TabIndex = 5;
            this.lblOnOff.Text = "센서 작동 여부";
            // 
            // lblLoacation
            // 
            this.lblLoacation.AutoSize = true;
            this.lblLoacation.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblLoacation.ForeColor = System.Drawing.Color.White;
            this.lblLoacation.Location = new System.Drawing.Point(164, 57);
            this.lblLoacation.Name = "lblLoacation";
            this.lblLoacation.Size = new System.Drawing.Size(42, 17);
            this.lblLoacation.TabIndex = 4;
            this.lblLoacation.Text = "위치";
            // 
            // lblInOut
            // 
            this.lblInOut.AutoSize = true;
            this.lblInOut.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblInOut.ForeColor = System.Drawing.Color.White;
            this.lblInOut.Location = new System.Drawing.Point(164, 56);
            this.lblInOut.Name = "lblInOut";
            this.lblInOut.Size = new System.Drawing.Size(82, 17);
            this.lblInOut.TabIndex = 3;
            this.lblInOut.Text = "실내/실외";
            // 
            // lblPSMMaterial
            // 
            this.lblPSMMaterial.AutoSize = true;
            this.lblPSMMaterial.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblPSMMaterial.ForeColor = System.Drawing.Color.White;
            this.lblPSMMaterial.Location = new System.Drawing.Point(12, 56);
            this.lblPSMMaterial.Name = "lblPSMMaterial";
            this.lblPSMMaterial.Size = new System.Drawing.Size(115, 17);
            this.lblPSMMaterial.TabIndex = 2;
            this.lblPSMMaterial.Text = "유해물질 종류";
            // 
            // gvPSMSensor
            // 
            this.gvPSMSensor.AllowUserToAddRows = false;
            this.gvPSMSensor.AllowUserToResizeRows = false;
            this.gvPSMSensor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gvPSMSensor.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.gvPSMSensor.ColumnHeadersHeight = 35;
            this.gvPSMSensor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gvPSMSensor.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPSMSensorNo,
            this.colPSMSensorLocation,
            this.colPSMSensorMaterialName,
            this.colPSMSensorCurrentOverflow,
            this.colAlarmDepth,
            this.colCCTV,
            this.colPSMSensorIsWorking});
            this.gvPSMSensor.Location = new System.Drawing.Point(5, 163);
            this.gvPSMSensor.MultiSelect = false;
            this.gvPSMSensor.Name = "gvPSMSensor";
            this.gvPSMSensor.RowHeadersVisible = false;
            this.gvPSMSensor.RowTemplate.Height = 23;
            this.gvPSMSensor.Size = new System.Drawing.Size(640, 764);
            this.gvPSMSensor.TabIndex = 1;
            this.gvPSMSensor.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridCellContentClick);
            this.gvPSMSensor.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.gvPSMSensor_CellPainting);
            this.gvPSMSensor.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gvPSMSensor_MouseUp);
            // 
            // colPSMSensorNo
            // 
            this.colPSMSensorNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.NullValue = null;
            this.colPSMSensorNo.DefaultCellStyle = dataGridViewCellStyle5;
            this.colPSMSensorNo.HeaderText = "No";
            this.colPSMSensorNo.Name = "colPSMSensorNo";
            this.colPSMSensorNo.ReadOnly = true;
            this.colPSMSensorNo.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colPSMSensorNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colPSMSensorNo.Width = 45;
            // 
            // colPSMSensorLocation
            // 
            this.colPSMSensorLocation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colPSMSensorLocation.DefaultCellStyle = dataGridViewCellStyle6;
            this.colPSMSensorLocation.HeaderText = "위치";
            this.colPSMSensorLocation.Name = "colPSMSensorLocation";
            this.colPSMSensorLocation.ReadOnly = true;
            this.colPSMSensorLocation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colPSMSensorMaterialName
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colPSMSensorMaterialName.DefaultCellStyle = dataGridViewCellStyle7;
            this.colPSMSensorMaterialName.HeaderText = "물질명";
            this.colPSMSensorMaterialName.Name = "colPSMSensorMaterialName";
            this.colPSMSensorMaterialName.ReadOnly = true;
            this.colPSMSensorMaterialName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colPSMSensorMaterialName.Width = 78;
            // 
            // colPSMSensorCurrentOverflow
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colPSMSensorCurrentOverflow.DefaultCellStyle = dataGridViewCellStyle8;
            this.colPSMSensorCurrentOverflow.HeaderText = "누출농도";
            this.colPSMSensorCurrentOverflow.Name = "colPSMSensorCurrentOverflow";
            this.colPSMSensorCurrentOverflow.ReadOnly = true;
            this.colPSMSensorCurrentOverflow.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colPSMSensorCurrentOverflow.Width = 83;
            // 
            // colAlarmDepth
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colAlarmDepth.DefaultCellStyle = dataGridViewCellStyle9;
            this.colAlarmDepth.HeaderText = "알람";
            this.colAlarmDepth.Name = "colAlarmDepth";
            this.colAlarmDepth.ReadOnly = true;
            this.colAlarmDepth.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colAlarmDepth.Width = 70;
            // 
            // colCCTV
            // 
            this.colCCTV.HeaderText = "현장사진 및 CCTV";
            this.colCCTV.Name = "colCCTV";
            this.colCCTV.Width = 78;
            // 
            // colPSMSensorIsWorking
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colPSMSensorIsWorking.DefaultCellStyle = dataGridViewCellStyle10;
            this.colPSMSensorIsWorking.HeaderText = "On / Off";
            this.colPSMSensorIsWorking.Name = "colPSMSensorIsWorking";
            this.colPSMSensorIsWorking.ReadOnly = true;
            this.colPSMSensorIsWorking.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colPSMSensorIsWorking.Width = 75;
            // 
            // gvPSMTank
            // 
            this.gvPSMTank.AllowUserToAddRows = false;
            this.gvPSMTank.AllowUserToResizeRows = false;
            this.gvPSMTank.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gvPSMTank.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle11;
            this.gvPSMTank.ColumnHeadersHeight = 35;
            this.gvPSMTank.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gvPSMTank.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPSMTankIndex,
            this.colPSMTankLocation,
            this.colTankName,
            this.colTankAlarm,
            this.colPSMTankRemains,
            this.colTankCCTV,
            this.colPSMTankMaterialName});
            this.gvPSMTank.Location = new System.Drawing.Point(5, 163);
            this.gvPSMTank.MultiSelect = false;
            this.gvPSMTank.Name = "gvPSMTank";
            this.gvPSMTank.RowHeadersVisible = false;
            this.gvPSMTank.RowTemplate.Height = 23;
            this.gvPSMTank.Size = new System.Drawing.Size(640, 764);
            this.gvPSMTank.TabIndex = 0;
            this.gvPSMTank.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridCellContentClick);
            this.gvPSMTank.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.gvPSMTank_CellPainting);
            this.gvPSMTank.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gvPSMTank_MouseUp);
            // 
            // colPSMTankIndex
            // 
            this.colPSMTankIndex.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colPSMTankIndex.DefaultCellStyle = dataGridViewCellStyle12;
            this.colPSMTankIndex.HeaderText = "No";
            this.colPSMTankIndex.Name = "colPSMTankIndex";
            this.colPSMTankIndex.ReadOnly = true;
            this.colPSMTankIndex.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colPSMTankIndex.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colPSMTankIndex.Width = 45;
            // 
            // colPSMTankLocation
            // 
            this.colPSMTankLocation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle13.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colPSMTankLocation.DefaultCellStyle = dataGridViewCellStyle13;
            this.colPSMTankLocation.HeaderText = "위치";
            this.colPSMTankLocation.Name = "colPSMTankLocation";
            this.colPSMTankLocation.ReadOnly = true;
            this.colPSMTankLocation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colTankName
            // 
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle14.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colTankName.DefaultCellStyle = dataGridViewCellStyle14;
            this.colTankName.HeaderText = "탱크이름";
            this.colTankName.Name = "colTankName";
            this.colTankName.ReadOnly = true;
            this.colTankName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colTankName.Width = 130;
            // 
            // colTankAlarm
            // 
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colTankAlarm.DefaultCellStyle = dataGridViewCellStyle15;
            this.colTankAlarm.HeaderText = "알람";
            this.colTankAlarm.Name = "colTankAlarm";
            this.colTankAlarm.ReadOnly = true;
            this.colTankAlarm.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colTankAlarm.Width = 61;
            // 
            // colPSMTankRemains
            // 
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle16.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colPSMTankRemains.DefaultCellStyle = dataGridViewCellStyle16;
            this.colPSMTankRemains.HeaderText = "잔량";
            this.colPSMTankRemains.Name = "colPSMTankRemains";
            this.colPSMTankRemains.ReadOnly = true;
            this.colPSMTankRemains.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colPSMTankRemains.Width = 58;
            // 
            // colTankCCTV
            // 
            this.colTankCCTV.HeaderText = "현장사진 및 CCTV";
            this.colTankCCTV.Name = "colTankCCTV";
            this.colTankCCTV.ReadOnly = true;
            this.colTankCCTV.Width = 78;
            // 
            // colPSMTankMaterialName
            // 
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle17.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colPSMTankMaterialName.DefaultCellStyle = dataGridViewCellStyle17;
            this.colPSMTankMaterialName.HeaderText = "물질명";
            this.colPSMTankMaterialName.Name = "colPSMTankMaterialName";
            this.colPSMTankMaterialName.ReadOnly = true;
            this.colPSMTankMaterialName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colPSMTankMaterialName.Width = 95;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // dataGridViewButtonColumn1
            // 
            this.dataGridViewButtonColumn1.HeaderText = "CCTV";
            this.dataGridViewButtonColumn1.Name = "dataGridViewButtonColumn1";
            this.dataGridViewButtonColumn1.Width = 90;
            // 
            // dataGridViewButtonColumn2
            // 
            this.dataGridViewButtonColumn2.HeaderText = "CCTV";
            this.dataGridViewButtonColumn2.Name = "dataGridViewButtonColumn2";
            this.dataGridViewButtonColumn2.ReadOnly = true;
            this.dataGridViewButtonColumn2.Width = 78;
            // 
            // popupMenuSensor
            // 
            this.popupMenuSensor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuShowSensorChart,
            this.menuSensorOnOff,
            this.menuShowSensorLifeTime,
            this.toolStripMenuItemDepartment,
            this.menuShowSensorCCTV,
            this.menuEditSensorAlarm});
            this.popupMenuSensor.Name = "popupMenuSensor";
            this.popupMenuSensor.Size = new System.Drawing.Size(179, 136);
            // 
            // menuShowSensorChart
            // 
            this.menuShowSensorChart.Name = "menuShowSensorChart";
            this.menuShowSensorChart.Size = new System.Drawing.Size(178, 22);
            this.menuShowSensorChart.Text = "센서 감지이력 보기";
            this.menuShowSensorChart.Click += new System.EventHandler(this.menuShowSensorChart_Click);
            // 
            // menuSensorOnOff
            // 
            this.menuSensorOnOff.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSensorOn,
            this.menuSensorOff});
            this.menuSensorOnOff.Name = "menuSensorOnOff";
            this.menuSensorOnOff.Size = new System.Drawing.Size(178, 22);
            this.menuSensorOnOff.Text = "센서 On/Off";
            // 
            // menuSensorOn
            // 
            this.menuSensorOn.Name = "menuSensorOn";
            this.menuSensorOn.Size = new System.Drawing.Size(119, 22);
            this.menuSensorOn.Text = "센서 On";
            this.menuSensorOn.Click += new System.EventHandler(this.menuSensorOn_Click);
            // 
            // menuSensorOff
            // 
            this.menuSensorOff.Name = "menuSensorOff";
            this.menuSensorOff.Size = new System.Drawing.Size(119, 22);
            this.menuSensorOff.Text = "센서 Off";
            this.menuSensorOff.Click += new System.EventHandler(this.menuSensorOff_Click);
            // 
            // menuShowSensorLifeTime
            // 
            this.menuShowSensorLifeTime.Name = "menuShowSensorLifeTime";
            this.menuShowSensorLifeTime.Size = new System.Drawing.Size(178, 22);
            this.menuShowSensorLifeTime.Text = "센서 교체주기 설정";
            this.menuShowSensorLifeTime.Click += new System.EventHandler(this.menuShowSensorLifeTime_Click);
            // 
            // toolStripMenuItemDepartment
            // 
            this.toolStripMenuItemDepartment.Name = "toolStripMenuItemDepartment";
            this.toolStripMenuItemDepartment.Size = new System.Drawing.Size(178, 22);
            this.toolStripMenuItemDepartment.Text = "담당부서 설정";
            this.toolStripMenuItemDepartment.Click += new System.EventHandler(this.toolStripMenuItemDepartment_Click);
            // 
            // menuShowSensorCCTV
            // 
            this.menuShowSensorCCTV.Name = "menuShowSensorCCTV";
            this.menuShowSensorCCTV.Size = new System.Drawing.Size(178, 22);
            this.menuShowSensorCCTV.Text = "CCTV 보기";
            this.menuShowSensorCCTV.Click += new System.EventHandler(this.menuShowSensorCCTV_Click);
            // 
            // menuEditSensorAlarm
            // 
            this.menuEditSensorAlarm.Name = "menuEditSensorAlarm";
            this.menuEditSensorAlarm.Size = new System.Drawing.Size(178, 22);
            this.menuEditSensorAlarm.Text = "센서 알람값 설정";
            this.menuEditSensorAlarm.Visible = false;
            this.menuEditSensorAlarm.Click += new System.EventHandler(this.menuEditSensorAlarm_Click);
            // 
            // popupMenuTank
            // 
            this.popupMenuTank.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuShowTankDetail,
            this.menuShowTankCCTV});
            this.popupMenuTank.Name = "popupMenuTank";
            this.popupMenuTank.Size = new System.Drawing.Size(151, 48);
            // 
            // menuShowTankDetail
            // 
            this.menuShowTankDetail.Name = "menuShowTankDetail";
            this.menuShowTankDetail.Size = new System.Drawing.Size(150, 22);
            this.menuShowTankDetail.Text = "탱크 상세보기";
            this.menuShowTankDetail.Click += new System.EventHandler(this.menuShowTankDetail_Click);
            // 
            // menuShowTankCCTV
            // 
            this.menuShowTankCCTV.Name = "menuShowTankCCTV";
            this.menuShowTankCCTV.Size = new System.Drawing.Size(150, 22);
            this.menuShowTankCCTV.Text = "CCTV 보기";
            this.menuShowTankCCTV.Click += new System.EventHandler(this.menuShowTankCCTV_Click);
            // 
            // btnShowSensorManual
            // 
            this.btnShowSensorManual.ButtonText = "";
            this.btnShowSensorManual.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnShowSensorManual.ImageClicked = global::SDMS.Properties.Resources.PSMList_ShowManual_Click;
            this.btnShowSensorManual.ImageDisabled = null;
            this.btnShowSensorManual.ImageMouseOver = global::SDMS.Properties.Resources.PSMList_ShowManual_Click;
            this.btnShowSensorManual.ImageNormal = global::SDMS.Properties.Resources.PSMList_ShowManual_Default;
            this.btnShowSensorManual.Location = new System.Drawing.Point(522, 12);
            this.btnShowSensorManual.Name = "btnShowSensorManual";
            this.btnShowSensorManual.Owner = null;
            this.btnShowSensorManual.Size = new System.Drawing.Size(123, 24);
            this.btnShowSensorManual.TabIndex = 7;
            this.btnShowSensorManual.TabStop = false;
            this.btnShowSensorManual.TextColor = System.Drawing.Color.Black;
            this.btnShowSensorManual.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnShowSensorManual.ToolTipText = "";
            this.btnShowSensorManual.UseToolTip = false;
            this.btnShowSensorManual.WindowRateWidth = 1F;
            this.btnShowSensorManual.Click += new System.EventHandler(this.btnShowSensorManual_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.ButtonText = "";
            this.btnSearch.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSearch.ImageClicked = global::SDMS.Properties.Resources.Search_Over;
            this.btnSearch.ImageDisabled = null;
            this.btnSearch.ImageMouseOver = global::SDMS.Properties.Resources.Search_Over;
            this.btnSearch.ImageNormal = global::SDMS.Properties.Resources.Search_Default;
            this.btnSearch.Location = new System.Drawing.Point(593, 77);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Owner = null;
            this.btnSearch.Size = new System.Drawing.Size(47, 29);
            this.btnSearch.TabIndex = 8;
            this.btnSearch.TabStop = false;
            this.btnSearch.TextColor = System.Drawing.Color.Black;
            this.btnSearch.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSearch.ToolTipText = "";
            this.btnSearch.UseToolTip = false;
            this.btnSearch.WindowRateWidth = 1F;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // cmbPSMMaterial
            // 
            this.cmbPSMMaterial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPSMMaterial.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbPSMMaterial.FormattingEnabled = true;
            this.cmbPSMMaterial.ImageClicked = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cmbPSMMaterial.ImageDisabled = null;
            this.cmbPSMMaterial.ImageMouseOver = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cmbPSMMaterial.ImageNormal = global::SDMS.Properties.Resources.ComboBoxDropDownBtn3_Default;
            this.cmbPSMMaterial.Location = new System.Drawing.Point(12, 77);
            this.cmbPSMMaterial.Name = "cmbPSMMaterial";
            this.cmbPSMMaterial.Owner = null;
            this.cmbPSMMaterial.Size = new System.Drawing.Size(146, 25);
            this.cmbPSMMaterial.TabIndex = 15;
            this.cmbPSMMaterial.TextColor = System.Drawing.Color.Black;
            this.cmbPSMMaterial.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbPSMMaterial.SelectedIndexChanged += new System.EventHandler(this.cmbPSMMaterial_SelectedIndexChanged);
            // 
            // cmbInOut
            // 
            this.cmbInOut.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInOut.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbInOut.FormattingEnabled = true;
            this.cmbInOut.ImageClicked = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cmbInOut.ImageDisabled = null;
            this.cmbInOut.ImageMouseOver = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cmbInOut.ImageNormal = global::SDMS.Properties.Resources.ComboBoxDropDownBtn3_Default;
            this.cmbInOut.Location = new System.Drawing.Point(164, 78);
            this.cmbInOut.Name = "cmbInOut";
            this.cmbInOut.Owner = null;
            this.cmbInOut.Size = new System.Drawing.Size(146, 25);
            this.cmbInOut.TabIndex = 16;
            this.cmbInOut.TextColor = System.Drawing.Color.Black;
            this.cmbInOut.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            // 
            // cmbLocation
            // 
            this.cmbLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLocation.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbLocation.FormattingEnabled = true;
            this.cmbLocation.ImageClicked = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cmbLocation.ImageDisabled = null;
            this.cmbLocation.ImageMouseOver = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cmbLocation.ImageNormal = global::SDMS.Properties.Resources.ComboBoxDropDownBtn3_Default;
            this.cmbLocation.Location = new System.Drawing.Point(164, 78);
            this.cmbLocation.Name = "cmbLocation";
            this.cmbLocation.Owner = null;
            this.cmbLocation.Size = new System.Drawing.Size(294, 25);
            this.cmbLocation.TabIndex = 17;
            this.cmbLocation.TextColor = System.Drawing.Color.Black;
            this.cmbLocation.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            // 
            // cmbOnOff
            // 
            this.cmbOnOff.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOnOff.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbOnOff.FormattingEnabled = true;
            this.cmbOnOff.ImageClicked = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cmbOnOff.ImageDisabled = null;
            this.cmbOnOff.ImageMouseOver = global::SDMS.Properties.Resources.ComboBoxDropDownBtn2_Click;
            this.cmbOnOff.ImageNormal = global::SDMS.Properties.Resources.ComboBoxDropDownBtn3_Default;
            this.cmbOnOff.Location = new System.Drawing.Point(464, 78);
            this.cmbOnOff.Name = "cmbOnOff";
            this.cmbOnOff.Owner = null;
            this.cmbOnOff.Size = new System.Drawing.Size(123, 25);
            this.cmbOnOff.TabIndex = 18;
            this.cmbOnOff.TextColor = System.Drawing.Color.Black;
            this.cmbOnOff.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 115);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 17);
            this.label1.TabIndex = 19;
            this.label1.Text = "설비영역";
            // 
            // btnChk12
            // 
            this.btnChk12.BackColor = System.Drawing.Color.Transparent;
            this.btnChk12.ButtonText = "";
            this.btnChk12.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnChk12.ImageClicked = ((System.Drawing.Image)(resources.GetObject("btnChk12.ImageClicked")));
            this.btnChk12.ImageDisabled = null;
            this.btnChk12.ImageMouseOver = ((System.Drawing.Image)(resources.GetObject("btnChk12.ImageMouseOver")));
            this.btnChk12.ImageNormal = ((System.Drawing.Image)(resources.GetObject("btnChk12.ImageNormal")));
            this.btnChk12.Location = new System.Drawing.Point(15, 138);
            this.btnChk12.Name = "btnChk12";
            this.btnChk12.Owner = null;
            this.btnChk12.Size = new System.Drawing.Size(16, 16);
            this.btnChk12.TabIndex = 20;
            this.btnChk12.TabStop = false;
            this.btnChk12.TextColor = System.Drawing.Color.Black;
            this.btnChk12.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnChk12.ToolTipText = "";
            this.btnChk12.UseToolTip = false;
            this.btnChk12.WindowRateWidth = 1F;
            this.btnChk12.Click += new System.EventHandler(this.btnChk12_Click);
            // 
            // chk12Text
            // 
            this.chk12Text.AutoSize = true;
            this.chk12Text.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.chk12Text.ForeColor = System.Drawing.Color.White;
            this.chk12Text.Location = new System.Drawing.Point(34, 140);
            this.chk12Text.Name = "chk12Text";
            this.chk12Text.Size = new System.Drawing.Size(97, 14);
            this.chk12Text.TabIndex = 21;
            this.chk12Text.Text = "#1,2호기 설비";
            // 
            // chk34Text
            // 
            this.chk34Text.AutoSize = true;
            this.chk34Text.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.chk34Text.ForeColor = System.Drawing.Color.White;
            this.chk34Text.Location = new System.Drawing.Point(150, 140);
            this.chk34Text.Name = "chk34Text";
            this.chk34Text.Size = new System.Drawing.Size(97, 14);
            this.chk34Text.TabIndex = 23;
            this.chk34Text.Text = "#3,4호기 설비";
            // 
            // btnChk34
            // 
            this.btnChk34.BackColor = System.Drawing.Color.Transparent;
            this.btnChk34.ButtonText = "";
            this.btnChk34.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnChk34.ImageClicked = ((System.Drawing.Image)(resources.GetObject("btnChk34.ImageClicked")));
            this.btnChk34.ImageDisabled = null;
            this.btnChk34.ImageMouseOver = ((System.Drawing.Image)(resources.GetObject("btnChk34.ImageMouseOver")));
            this.btnChk34.ImageNormal = ((System.Drawing.Image)(resources.GetObject("btnChk34.ImageNormal")));
            this.btnChk34.Location = new System.Drawing.Point(131, 138);
            this.btnChk34.Name = "btnChk34";
            this.btnChk34.Owner = null;
            this.btnChk34.Size = new System.Drawing.Size(16, 16);
            this.btnChk34.TabIndex = 22;
            this.btnChk34.TabStop = false;
            this.btnChk34.TextColor = System.Drawing.Color.Black;
            this.btnChk34.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnChk34.ToolTipText = "";
            this.btnChk34.UseToolTip = false;
            this.btnChk34.WindowRateWidth = 1F;
            this.btnChk34.Click += new System.EventHandler(this.btnChk34_Click);
            // 
            // chk56Text
            // 
            this.chk56Text.AutoSize = true;
            this.chk56Text.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.chk56Text.ForeColor = System.Drawing.Color.White;
            this.chk56Text.Location = new System.Drawing.Point(268, 140);
            this.chk56Text.Name = "chk56Text";
            this.chk56Text.Size = new System.Drawing.Size(97, 14);
            this.chk56Text.TabIndex = 25;
            this.chk56Text.Text = "#5,6호기 설비";
            // 
            // btnChk56
            // 
            this.btnChk56.BackColor = System.Drawing.Color.Transparent;
            this.btnChk56.ButtonText = "";
            this.btnChk56.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnChk56.ImageClicked = ((System.Drawing.Image)(resources.GetObject("btnChk56.ImageClicked")));
            this.btnChk56.ImageDisabled = null;
            this.btnChk56.ImageMouseOver = ((System.Drawing.Image)(resources.GetObject("btnChk56.ImageMouseOver")));
            this.btnChk56.ImageNormal = ((System.Drawing.Image)(resources.GetObject("btnChk56.ImageNormal")));
            this.btnChk56.Location = new System.Drawing.Point(249, 138);
            this.btnChk56.Name = "btnChk56";
            this.btnChk56.Owner = null;
            this.btnChk56.Size = new System.Drawing.Size(16, 16);
            this.btnChk56.TabIndex = 24;
            this.btnChk56.TabStop = false;
            this.btnChk56.TextColor = System.Drawing.Color.Black;
            this.btnChk56.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnChk56.ToolTipText = "";
            this.btnChk56.UseToolTip = false;
            this.btnChk56.WindowRateWidth = 1F;
            this.btnChk56.Click += new System.EventHandler(this.btnChk56_Click);
            // 
            // chkWaterText
            // 
            this.chkWaterText.AutoSize = true;
            this.chkWaterText.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.chkWaterText.ForeColor = System.Drawing.Color.White;
            this.chkWaterText.Location = new System.Drawing.Point(385, 139);
            this.chkWaterText.Name = "chkWaterText";
            this.chkWaterText.Size = new System.Drawing.Size(116, 14);
            this.chkWaterText.TabIndex = 27;
            this.chkWaterText.Text = "탈황/수처리 설비";
            // 
            // btnChkWater
            // 
            this.btnChkWater.BackColor = System.Drawing.Color.Transparent;
            this.btnChkWater.ButtonText = "";
            this.btnChkWater.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnChkWater.ImageClicked = ((System.Drawing.Image)(resources.GetObject("btnChkWater.ImageClicked")));
            this.btnChkWater.ImageDisabled = null;
            this.btnChkWater.ImageMouseOver = ((System.Drawing.Image)(resources.GetObject("btnChkWater.ImageMouseOver")));
            this.btnChkWater.ImageNormal = ((System.Drawing.Image)(resources.GetObject("btnChkWater.ImageNormal")));
            this.btnChkWater.Location = new System.Drawing.Point(366, 138);
            this.btnChkWater.Name = "btnChkWater";
            this.btnChkWater.Owner = null;
            this.btnChkWater.Size = new System.Drawing.Size(16, 16);
            this.btnChkWater.TabIndex = 26;
            this.btnChkWater.TabStop = false;
            this.btnChkWater.TextColor = System.Drawing.Color.Black;
            this.btnChkWater.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnChkWater.ToolTipText = "";
            this.btnChkWater.UseToolTip = false;
            this.btnChkWater.WindowRateWidth = 1F;
            this.btnChkWater.Click += new System.EventHandler(this.btnChkWater_Click);
            // 
            // chkETCText
            // 
            this.chkETCText.AutoSize = true;
            this.chkETCText.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.chkETCText.ForeColor = System.Drawing.Color.White;
            this.chkETCText.Location = new System.Drawing.Point(518, 139);
            this.chkETCText.Name = "chkETCText";
            this.chkETCText.Size = new System.Drawing.Size(68, 14);
            this.chkETCText.TabIndex = 29;
            this.chkETCText.Text = "기타 설비";
            // 
            // btnChkETC
            // 
            this.btnChkETC.BackColor = System.Drawing.Color.Transparent;
            this.btnChkETC.ButtonText = "";
            this.btnChkETC.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnChkETC.ImageClicked = ((System.Drawing.Image)(resources.GetObject("btnChkETC.ImageClicked")));
            this.btnChkETC.ImageDisabled = null;
            this.btnChkETC.ImageMouseOver = ((System.Drawing.Image)(resources.GetObject("btnChkETC.ImageMouseOver")));
            this.btnChkETC.ImageNormal = ((System.Drawing.Image)(resources.GetObject("btnChkETC.ImageNormal")));
            this.btnChkETC.Location = new System.Drawing.Point(503, 138);
            this.btnChkETC.Name = "btnChkETC";
            this.btnChkETC.Owner = null;
            this.btnChkETC.Size = new System.Drawing.Size(16, 16);
            this.btnChkETC.TabIndex = 28;
            this.btnChkETC.TabStop = false;
            this.btnChkETC.TextColor = System.Drawing.Color.Black;
            this.btnChkETC.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnChkETC.ToolTipText = "";
            this.btnChkETC.UseToolTip = false;
            this.btnChkETC.WindowRateWidth = 1F;
            this.btnChkETC.Click += new System.EventHandler(this.btnChkETC_Click);
            // 
            // FormPSMList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(652, 933);
            this.Controls.Add(this.chkETCText);
            this.Controls.Add(this.btnChkETC);
            this.Controls.Add(this.chkWaterText);
            this.Controls.Add(this.btnChkWater);
            this.Controls.Add(this.chk56Text);
            this.Controls.Add(this.btnChk56);
            this.Controls.Add(this.chk34Text);
            this.Controls.Add(this.btnChk34);
            this.Controls.Add(this.chk12Text);
            this.Controls.Add(this.btnChk12);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbOnOff);
            this.Controls.Add(this.cmbLocation);
            this.Controls.Add(this.cmbInOut);
            this.Controls.Add(this.cmbPSMMaterial);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.gvPSMSensor);
            this.Controls.Add(this.gvPSMTank);
            this.Controls.Add(this.lblOnOff);
            this.Controls.Add(this.btnShowSensorManual);
            this.Controls.Add(this.lblLoacation);
            this.Controls.Add(this.panelGuide);
            this.Controls.Add(this.lblInOut);
            this.Controls.Add(this.rdoTankList);
            this.Controls.Add(this.lblPSMMaterial);
            this.Controls.Add(this.rdoSensorList);
            this.Name = "FormPSMList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "유해물질 탱크 및 센서 목록";
            this.Shown += new System.EventHandler(this.FormPSMList_Shown);
            this.panelGuide.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridGuide)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvPSMSensor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvPSMTank)).EndInit();
            this.popupMenuSensor.ResumeLayout(false);
            this.popupMenuTank.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnShowSensorManual)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnChk12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnChk34)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnChk56)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnChkWater)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnChkETC)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rdoTankList;
        private System.Windows.Forms.RadioButton rdoSensorList;
        private System.Windows.Forms.Label lblOnOff;
        private System.Windows.Forms.Label lblLoacation;
        private System.Windows.Forms.Label lblInOut;
        private System.Windows.Forms.Label lblPSMMaterial;
        private System.Windows.Forms.DataGridView gvPSMTank;
        private System.Windows.Forms.DataGridView gvPSMSensor;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.DataGridViewButtonColumn dataGridViewButtonColumn1;
        private System.Windows.Forms.DataGridViewButtonColumn dataGridViewButtonColumn2;
        private System.Windows.Forms.Panel panelGuide;
        private System.Windows.Forms.Button btnGuide;
        private System.Windows.Forms.DataGridView gridGuide;
        private System.Windows.Forms.ContextMenuStrip popupMenuSensor;
        private System.Windows.Forms.ToolStripMenuItem menuShowSensorChart;
        private System.Windows.Forms.ToolStripMenuItem menuSensorOnOff;
        private System.Windows.Forms.ToolStripMenuItem menuShowSensorLifeTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn col1;
        private System.Windows.Forms.DataGridViewTextBoxColumn col2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.ToolStripMenuItem menuSensorOn;
        private System.Windows.Forms.ToolStripMenuItem menuSensorOff;
        private System.Windows.Forms.ToolStripMenuItem menuShowSensorCCTV;
        private System.Windows.Forms.ContextMenuStrip popupMenuTank;
        private System.Windows.Forms.ToolStripMenuItem menuShowTankDetail;
        private System.Windows.Forms.ToolStripMenuItem menuShowTankCCTV;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDepartment;
        private System.Windows.Forms.ToolStripMenuItem menuEditSensorAlarm;
        private UnE.GUI.ImageButton btnShowSensorManual;
        private UnE.GUI.ImageButton btnSearch;
        private UnE.GUI.ImageComboBox cmbPSMMaterial;
        private UnE.GUI.ImageComboBox cmbInOut;
        private UnE.GUI.ImageComboBox cmbLocation;
        private UnE.GUI.ImageComboBox cmbOnOff;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPSMTankIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPSMTankLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTankName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTankAlarm;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPSMTankRemains;
        private System.Windows.Forms.DataGridViewButtonColumn colTankCCTV;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPSMTankMaterialName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPSMSensorNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPSMSensorLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPSMSensorMaterialName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPSMSensorCurrentOverflow;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAlarmDepth;
        private System.Windows.Forms.DataGridViewButtonColumn colCCTV;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPSMSensorIsWorking;
        private UnE.GUI.ImageButton btnChk12;
        private System.Windows.Forms.Label chk12Text;
        private System.Windows.Forms.Label chk34Text;
        private UnE.GUI.ImageButton btnChk34;
        private System.Windows.Forms.Label chk56Text;
        private UnE.GUI.ImageButton btnChk56;
        private System.Windows.Forms.Label chkWaterText;
        private UnE.GUI.ImageButton btnChkWater;
        private System.Windows.Forms.Label chkETCText;
        private UnE.GUI.ImageButton btnChkETC;
    }
}