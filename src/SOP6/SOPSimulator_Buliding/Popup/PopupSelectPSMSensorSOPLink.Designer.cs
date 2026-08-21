namespace SOPMonitoringSystem.Popup
{
    partial class PopupSelectPSMSensorSOPLink
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.plTitle = new System.Windows.Forms.Panel();
            this.btnCancle = new UnE.GUI.RibbonButton();
            this.pbTitle = new System.Windows.Forms.PictureBox();
            this.lbTitle = new System.Windows.Forms.Label();
            this.gridSOP = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSOPFullPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridLocation = new System.Windows.Forms.DataGridView();
            this.colLocationNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLinkedSOP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridMaterial = new System.Windows.Forms.DataGridView();
            this.colMaterial = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.plPageBuildingSignal = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnDeleteSOP = new UnE.GUI.RibbonButton();
            this.btnChangeSOP = new UnE.GUI.RibbonButton();
            this.btnNewSOP = new UnE.GUI.RibbonButton();
            this.btnCancel = new UnE.GUI.RibbonButton();
            this.btnSave = new UnE.GUI.RibbonButton();
            this.ribbonButton = new UnE.GUI.RibbonButton();
            this.tabPageBuildingSignal = new UnE.GUI.RibbonButton();
            this.plTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridSOP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLocation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridMaterial)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.plPageBuildingSignal.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            this.SuspendLayout();
            // 
            // plTitle
            // 
            this.plTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.plTitle.Controls.Add(this.btnCancle);
            this.plTitle.Controls.Add(this.pbTitle);
            this.plTitle.Controls.Add(this.lbTitle);
            this.plTitle.Location = new System.Drawing.Point(0, 0);
            this.plTitle.Name = "plTitle";
            this.plTitle.Size = new System.Drawing.Size(1340, 60);
            this.plTitle.TabIndex = 4;
            this.plTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plTitle_MouseDown);
            this.plTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plTitle_MouseMove);
            this.plTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.plTitle_MouseUp);
            // 
            // btnCancle
            // 
            this.btnCancle.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.btnCancle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCancle.CheckButton = false;
            this.btnCancle.CheckedBkgndImage = null;
            this.btnCancle.CheckedImage = null;
            this.btnCancle.CheckedMouseOver = null;
            this.btnCancle.ClickedBackgroundImage = null;
            this.btnCancle.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.btnClose_Selected;
            this.btnCancle.CustomImageRect = new System.Drawing.Rectangle(0, 0, 22, 22);
            this.btnCancle.DisabledBkgndImage = null;
            this.btnCancle.DisabledImage = null;
            this.btnCancle.ForeColorChecked = System.Drawing.Color.White;
            this.btnCancle.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnCancle.ForeColorDisabled = System.Drawing.Color.White;
            this.btnCancle.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnCancle.ForeColorsByTypeUse = false;
            this.btnCancle.ID = -1;
            this.btnCancle.InitButtonWidth = 22;
            this.btnCancle.IsChecked = false;
            this.btnCancle.Location = new System.Drawing.Point(1298, 19);
            this.btnCancle.MouseOverBkgndImage = null;
            this.btnCancle.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.btnClose_MouseOver;
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.NormalImage = global::SOPMonitoringSystem.Properties.Resources.btnClose_Normal;
            this.btnCancle.Owner = null;
            this.btnCancle.Size = new System.Drawing.Size(22, 22);
            this.btnCancle.TabIndex = 132;
            this.btnCancle.TextLocation = new System.Drawing.Point(0, 0);
            this.btnCancle.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCancle.ToolTipText = "";
            this.btnCancle.UseCustomImageRect = false;
            this.btnCancle.UseTextLocation = false;
            this.btnCancle.UseVisualStyleBackColor = false;
            this.btnCancle.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pbTitle
            // 
            this.pbTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.pbTitle.Location = new System.Drawing.Point(22, 28);
            this.pbTitle.Margin = new System.Windows.Forms.Padding(0);
            this.pbTitle.Name = "pbTitle";
            this.pbTitle.Size = new System.Drawing.Size(5, 5);
            this.pbTitle.TabIndex = 3;
            this.pbTitle.TabStop = false;
            this.pbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbTitle_MouseDown);
            this.pbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbTitle_MouseMove);
            this.pbTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbTitle_MouseUp);
            // 
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbTitle.ForeColor = System.Drawing.Color.White;
            this.lbTitle.Location = new System.Drawing.Point(43, 20);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(335, 23);
            this.lbTitle.TabIndex = 1;
            this.lbTitle.Text = "유해화학물질 누출탐지 신호별 SOP 설정";
            this.lbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseDown);
            this.lbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseMove);
            this.lbTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseUp);
            // 
            // gridSOP
            // 
            this.gridSOP.AllowUserToAddRows = false;
            this.gridSOP.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridSOP.ColumnHeadersVisible = false;
            this.gridSOP.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colSOPFullPath});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("나눔바른고딕", 11.25F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridSOP.DefaultCellStyle = dataGridViewCellStyle3;
            this.gridSOP.Location = new System.Drawing.Point(14, 51);
            this.gridSOP.MultiSelect = false;
            this.gridSOP.Name = "gridSOP";
            this.gridSOP.RowHeadersVisible = false;
            this.gridSOP.RowTemplate.Height = 23;
            this.gridSOP.Size = new System.Drawing.Size(525, 488);
            this.gridSOP.TabIndex = 0;
            this.gridSOP.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridSOP_CellMouseClick);
            // 
            // colNo
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle1;
            this.colNo.HeaderText = "No";
            this.colNo.Name = "colNo";
            this.colNo.Width = 53;
            // 
            // colSOPFullPath
            // 
            this.colSOPFullPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colSOPFullPath.DefaultCellStyle = dataGridViewCellStyle2;
            this.colSOPFullPath.HeaderText = "SOP";
            this.colSOPFullPath.Name = "colSOPFullPath";
            // 
            // gridLocation
            // 
            this.gridLocation.AllowUserToAddRows = false;
            this.gridLocation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridLocation.ColumnHeadersVisible = false;
            this.gridLocation.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colLocationNo,
            this.colLocation,
            this.colLinkedSOP});
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("나눔바른고딕", 11.25F);
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridLocation.DefaultCellStyle = dataGridViewCellStyle7;
            this.gridLocation.Location = new System.Drawing.Point(192, 51);
            this.gridLocation.Name = "gridLocation";
            this.gridLocation.RowHeadersVisible = false;
            this.gridLocation.RowTemplate.Height = 23;
            this.gridLocation.Size = new System.Drawing.Size(525, 488);
            this.gridLocation.TabIndex = 0;
            this.gridLocation.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellEndEdit);
            this.gridLocation.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grid_CellMouseClick);
            // 
            // colLocationNo
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colLocationNo.DefaultCellStyle = dataGridViewCellStyle4;
            this.colLocationNo.HeaderText = "No";
            this.colLocationNo.Name = "colLocationNo";
            this.colLocationNo.Width = 53;
            // 
            // colLocation
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colLocation.DefaultCellStyle = dataGridViewCellStyle5;
            this.colLocation.HeaderText = "위치";
            this.colLocation.Name = "colLocation";
            this.colLocation.Width = 354;
            // 
            // colLinkedSOP
            // 
            this.colLinkedSOP.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colLinkedSOP.DefaultCellStyle = dataGridViewCellStyle6;
            this.colLinkedSOP.HeaderText = "SOP";
            this.colLinkedSOP.Name = "colLinkedSOP";
            // 
            // gridMaterial
            // 
            this.gridMaterial.AllowUserToAddRows = false;
            this.gridMaterial.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridMaterial.ColumnHeadersVisible = false;
            this.gridMaterial.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMaterial});
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("나눔바른고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridMaterial.DefaultCellStyle = dataGridViewCellStyle9;
            this.gridMaterial.Location = new System.Drawing.Point(14, 51);
            this.gridMaterial.Name = "gridMaterial";
            this.gridMaterial.RowHeadersVisible = false;
            this.gridMaterial.RowTemplate.Height = 23;
            this.gridMaterial.Size = new System.Drawing.Size(163, 488);
            this.gridMaterial.TabIndex = 0;
            this.gridMaterial.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grid_CellMouseClick);
            this.gridMaterial.SelectionChanged += new System.EventHandler(this.grid_SelectionChanged);
            // 
            // colMaterial
            // 
            this.colMaterial.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle8.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colMaterial.DefaultCellStyle = dataGridViewCellStyle8;
            this.colMaterial.HeaderText = "물질";
            this.colMaterial.Name = "colMaterial";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuSelectAll});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(123, 26);
            // 
            // tsMenuSelectAll
            // 
            this.tsMenuSelectAll.Name = "tsMenuSelectAll";
            this.tsMenuSelectAll.Size = new System.Drawing.Size(122, 22);
            this.tsMenuSelectAll.Text = "전체선택";
            this.tsMenuSelectAll.Click += new System.EventHandler(this.tsMenuSelectAll_Click);
            // 
            // plPageBuildingSignal
            // 
            this.plPageBuildingSignal.BackColor = System.Drawing.Color.White;
            this.plPageBuildingSignal.Controls.Add(this.panel1);
            this.plPageBuildingSignal.Controls.Add(this.panel2);
            this.plPageBuildingSignal.Controls.Add(this.gridLocation);
            this.plPageBuildingSignal.Controls.Add(this.gridMaterial);
            this.plPageBuildingSignal.Location = new System.Drawing.Point(15, 112);
            this.plPageBuildingSignal.Name = "plPageBuildingSignal";
            this.plPageBuildingSignal.Size = new System.Drawing.Size(733, 604);
            this.plPageBuildingSignal.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(119)))), ((int)(((byte)(141)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(14, 14);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(163, 37);
            this.panel1.TabIndex = 137;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("나눔바른고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(65, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 17);
            this.label1.TabIndex = 110;
            this.label1.Text = "물질";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(119)))), ((int)(((byte)(141)))));
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.pictureBox2);
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(192, 14);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(525, 37);
            this.panel2.TabIndex = 138;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("나눔바른고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(448, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 17);
            this.label4.TabIndex = 115;
            this.label4.Text = "SOP";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("나눔바른고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(212, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 17);
            this.label3.TabIndex = 114;
            this.label3.Text = "위치";
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.White;
            this.pictureBox2.Location = new System.Drawing.Point(408, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(1, 37);
            this.pictureBox2.TabIndex = 113;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Location = new System.Drawing.Point(54, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1, 37);
            this.pictureBox1.TabIndex = 112;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("나눔바른고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(17, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 17);
            this.label2.TabIndex = 110;
            this.label2.Text = "ID";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.panel7);
            this.panel3.Controls.Add(this.gridSOP);
            this.panel3.Controls.Add(this.btnDeleteSOP);
            this.panel3.Controls.Add(this.btnChangeSOP);
            this.panel3.Controls.Add(this.btnNewSOP);
            this.panel3.Location = new System.Drawing.Point(770, 112);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(555, 604);
            this.panel3.TabIndex = 7;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(119)))), ((int)(((byte)(141)))));
            this.panel7.Controls.Add(this.label10);
            this.panel7.Controls.Add(this.pictureBox6);
            this.panel7.Controls.Add(this.label12);
            this.panel7.Location = new System.Drawing.Point(14, 14);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(525, 37);
            this.panel7.TabIndex = 136;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("나눔바른고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(271, 11);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(37, 17);
            this.label10.TabIndex = 115;
            this.label10.Text = "SOP";
            // 
            // pictureBox6
            // 
            this.pictureBox6.BackColor = System.Drawing.Color.White;
            this.pictureBox6.Location = new System.Drawing.Point(54, 0);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(1, 37);
            this.pictureBox6.TabIndex = 112;
            this.pictureBox6.TabStop = false;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("나눔바른고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label12.ForeColor = System.Drawing.Color.White;
            this.label12.Location = new System.Drawing.Point(17, 11);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(30, 17);
            this.label12.TabIndex = 110;
            this.label12.Text = "NO";
            // 
            // btnDeleteSOP
            // 
            this.btnDeleteSOP.BackColor = System.Drawing.Color.Transparent;
            this.btnDeleteSOP.CheckButton = false;
            this.btnDeleteSOP.CheckedBkgndImage = null;
            this.btnDeleteSOP.CheckedImage = null;
            this.btnDeleteSOP.CheckedMouseOver = null;
            this.btnDeleteSOP.ClickedBackgroundImage = null;
            this.btnDeleteSOP.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.btnFireSensor_Selected;
            this.btnDeleteSOP.CustomImageRect = new System.Drawing.Rectangle(0, 0, 112, 33);
            this.btnDeleteSOP.DisabledBkgndImage = null;
            this.btnDeleteSOP.DisabledImage = null;
            this.btnDeleteSOP.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDeleteSOP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(57)))), ((int)(((byte)(87)))));
            this.btnDeleteSOP.ForeColorChecked = System.Drawing.Color.White;
            this.btnDeleteSOP.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnDeleteSOP.ForeColorDisabled = System.Drawing.Color.White;
            this.btnDeleteSOP.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnDeleteSOP.ForeColorsByTypeUse = false;
            this.btnDeleteSOP.ID = -1;
            this.btnDeleteSOP.InitButtonWidth = 112;
            this.btnDeleteSOP.IsChecked = false;
            this.btnDeleteSOP.Location = new System.Drawing.Point(256, 555);
            this.btnDeleteSOP.MouseOverBkgndImage = null;
            this.btnDeleteSOP.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.btnFireSensor_Mouseover;
            this.btnDeleteSOP.Name = "btnDeleteSOP";
            this.btnDeleteSOP.NormalImage = global::SOPMonitoringSystem.Properties.Resources.btnFireSensor_Normal;
            this.btnDeleteSOP.Owner = null;
            this.btnDeleteSOP.Size = new System.Drawing.Size(112, 33);
            this.btnDeleteSOP.TabIndex = 131;
            this.btnDeleteSOP.Text = "SOP 삭제";
            this.btnDeleteSOP.TextLocation = new System.Drawing.Point(20, 7);
            this.btnDeleteSOP.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnDeleteSOP.ToolTipText = "SOP 삭제";
            this.btnDeleteSOP.UseCustomImageRect = true;
            this.btnDeleteSOP.UseTextLocation = true;
            this.btnDeleteSOP.UseVisualStyleBackColor = false;
            this.btnDeleteSOP.Click += new System.EventHandler(this.btnDeleteSOP_Click);
            // 
            // btnChangeSOP
            // 
            this.btnChangeSOP.BackColor = System.Drawing.Color.Transparent;
            this.btnChangeSOP.CheckButton = false;
            this.btnChangeSOP.CheckedBkgndImage = null;
            this.btnChangeSOP.CheckedImage = null;
            this.btnChangeSOP.CheckedMouseOver = null;
            this.btnChangeSOP.ClickedBackgroundImage = null;
            this.btnChangeSOP.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.btnFireSensor_Selected;
            this.btnChangeSOP.CustomImageRect = new System.Drawing.Rectangle(0, 0, 112, 33);
            this.btnChangeSOP.DisabledBkgndImage = null;
            this.btnChangeSOP.DisabledImage = null;
            this.btnChangeSOP.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnChangeSOP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(57)))), ((int)(((byte)(87)))));
            this.btnChangeSOP.ForeColorChecked = System.Drawing.Color.White;
            this.btnChangeSOP.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnChangeSOP.ForeColorDisabled = System.Drawing.Color.White;
            this.btnChangeSOP.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnChangeSOP.ForeColorsByTypeUse = false;
            this.btnChangeSOP.ID = -1;
            this.btnChangeSOP.InitButtonWidth = 112;
            this.btnChangeSOP.IsChecked = false;
            this.btnChangeSOP.Location = new System.Drawing.Point(135, 555);
            this.btnChangeSOP.MouseOverBkgndImage = null;
            this.btnChangeSOP.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.btnFireSensor_Mouseover;
            this.btnChangeSOP.Name = "btnChangeSOP";
            this.btnChangeSOP.NormalImage = global::SOPMonitoringSystem.Properties.Resources.btnFireSensor_Normal;
            this.btnChangeSOP.Owner = null;
            this.btnChangeSOP.Size = new System.Drawing.Size(112, 33);
            this.btnChangeSOP.TabIndex = 130;
            this.btnChangeSOP.Text = "SOP 변경";
            this.btnChangeSOP.TextLocation = new System.Drawing.Point(20, 7);
            this.btnChangeSOP.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnChangeSOP.ToolTipText = "SOP 변경";
            this.btnChangeSOP.UseCustomImageRect = true;
            this.btnChangeSOP.UseTextLocation = true;
            this.btnChangeSOP.UseVisualStyleBackColor = false;
            this.btnChangeSOP.Click += new System.EventHandler(this.btnChangeSOP_Click);
            // 
            // btnNewSOP
            // 
            this.btnNewSOP.BackColor = System.Drawing.Color.Transparent;
            this.btnNewSOP.CheckButton = false;
            this.btnNewSOP.CheckedBkgndImage = null;
            this.btnNewSOP.CheckedImage = null;
            this.btnNewSOP.CheckedMouseOver = null;
            this.btnNewSOP.ClickedBackgroundImage = null;
            this.btnNewSOP.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.btnFireSensor_Selected;
            this.btnNewSOP.CustomImageRect = new System.Drawing.Rectangle(0, 0, 112, 33);
            this.btnNewSOP.DisabledBkgndImage = null;
            this.btnNewSOP.DisabledImage = null;
            this.btnNewSOP.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnNewSOP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(57)))), ((int)(((byte)(87)))));
            this.btnNewSOP.ForeColorChecked = System.Drawing.Color.White;
            this.btnNewSOP.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnNewSOP.ForeColorDisabled = System.Drawing.Color.White;
            this.btnNewSOP.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnNewSOP.ForeColorsByTypeUse = false;
            this.btnNewSOP.ID = -1;
            this.btnNewSOP.InitButtonWidth = 112;
            this.btnNewSOP.IsChecked = false;
            this.btnNewSOP.Location = new System.Drawing.Point(14, 555);
            this.btnNewSOP.MouseOverBkgndImage = null;
            this.btnNewSOP.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.btnFireSensor_Mouseover;
            this.btnNewSOP.Name = "btnNewSOP";
            this.btnNewSOP.NormalImage = global::SOPMonitoringSystem.Properties.Resources.btnFireSensor_Normal;
            this.btnNewSOP.Owner = null;
            this.btnNewSOP.Size = new System.Drawing.Size(112, 33);
            this.btnNewSOP.TabIndex = 129;
            this.btnNewSOP.Text = "SOP 추가";
            this.btnNewSOP.TextLocation = new System.Drawing.Point(20, 7);
            this.btnNewSOP.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnNewSOP.ToolTipText = "SOP 추가";
            this.btnNewSOP.UseCustomImageRect = true;
            this.btnNewSOP.UseTextLocation = true;
            this.btnNewSOP.UseVisualStyleBackColor = false;
            this.btnNewSOP.Click += new System.EventHandler(this.btnNewSOP_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.CheckButton = false;
            this.btnCancel.CheckedBkgndImage = null;
            this.btnCancel.CheckedImage = null;
            this.btnCancel.CheckedMouseOver = null;
            this.btnCancel.ClickedBackgroundImage = null;
            this.btnCancel.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.QuickCloseButton_Selected;
            this.btnCancel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 146, 45);
            this.btnCancel.DisabledBkgndImage = null;
            this.btnCancel.DisabledImage = null;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.ForeColorChecked = System.Drawing.Color.Black;
            this.btnCancel.ForeColorCheckedMouseOver = System.Drawing.Color.Black;
            this.btnCancel.ForeColorDisabled = System.Drawing.Color.Black;
            this.btnCancel.ForeColorMouseOver = System.Drawing.Color.Black;
            this.btnCancel.ForeColorsByTypeUse = false;
            this.btnCancel.ID = -1;
            this.btnCancel.InitButtonWidth = 146;
            this.btnCancel.IsChecked = false;
            this.btnCancel.Location = new System.Drawing.Point(1179, 730);
            this.btnCancel.MouseOverBkgndImage = null;
            this.btnCancel.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.QuickCloseButton_MouseOver;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.NormalImage = global::SOPMonitoringSystem.Properties.Resources.QuickCloseButton_Normal;
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(146, 45);
            this.btnCancel.TabIndex = 135;
            this.btnCancel.Text = "취소";
            this.btnCancel.TextLocation = new System.Drawing.Point(50, 12);
            this.btnCancel.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnCancel.ToolTipText = "취소";
            this.btnCancel.UseCustomImageRect = true;
            this.btnCancel.UseTextLocation = true;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.CheckButton = false;
            this.btnSave.CheckedBkgndImage = null;
            this.btnSave.CheckedImage = null;
            this.btnSave.CheckedMouseOver = null;
            this.btnSave.ClickedBackgroundImage = null;
            this.btnSave.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.QuickSaveButton_Selected;
            this.btnSave.CustomImageRect = new System.Drawing.Rectangle(0, 0, 146, 45);
            this.btnSave.DisabledBkgndImage = null;
            this.btnSave.DisabledImage = null;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.ForeColorChecked = System.Drawing.Color.White;
            this.btnSave.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnSave.ForeColorDisabled = System.Drawing.Color.White;
            this.btnSave.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnSave.ForeColorsByTypeUse = false;
            this.btnSave.ID = -1;
            this.btnSave.InitButtonWidth = 146;
            this.btnSave.IsChecked = false;
            this.btnSave.Location = new System.Drawing.Point(1025, 730);
            this.btnSave.MouseOverBkgndImage = null;
            this.btnSave.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.QuickSaveButton_MouseOver;
            this.btnSave.Name = "btnSave";
            this.btnSave.NormalImage = global::SOPMonitoringSystem.Properties.Resources.QuickSaveButton_Normal;
            this.btnSave.Owner = null;
            this.btnSave.Size = new System.Drawing.Size(146, 45);
            this.btnSave.TabIndex = 134;
            this.btnSave.Text = "저장";
            this.btnSave.TextLocation = new System.Drawing.Point(50, 12);
            this.btnSave.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnSave.ToolTipText = "저장";
            this.btnSave.UseCustomImageRect = true;
            this.btnSave.UseTextLocation = true;
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // ribbonButton
            // 
            this.ribbonButton.BackColor = System.Drawing.Color.Transparent;
            this.ribbonButton.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.tabFireSensorList_Selected;
            this.ribbonButton.CheckButton = false;
            this.ribbonButton.CheckedBkgndImage = null;
            this.ribbonButton.CheckedImage = null;
            this.ribbonButton.CheckedMouseOver = null;
            this.ribbonButton.ClickedBackgroundImage = null;
            this.ribbonButton.ClickedImage = null;
            this.ribbonButton.CustomImageRect = new System.Drawing.Rectangle(0, 0, 135, 37);
            this.ribbonButton.DisabledBkgndImage = null;
            this.ribbonButton.DisabledImage = null;
            this.ribbonButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ribbonButton.ForeColor = System.Drawing.Color.White;
            this.ribbonButton.ForeColorChecked = System.Drawing.Color.White;
            this.ribbonButton.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.ribbonButton.ForeColorDisabled = System.Drawing.Color.White;
            this.ribbonButton.ForeColorMouseOver = System.Drawing.Color.White;
            this.ribbonButton.ForeColorsByTypeUse = false;
            this.ribbonButton.ID = -1;
            this.ribbonButton.InitButtonWidth = 135;
            this.ribbonButton.IsChecked = false;
            this.ribbonButton.Location = new System.Drawing.Point(770, 75);
            this.ribbonButton.MouseOverBkgndImage = null;
            this.ribbonButton.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.tabFireSensorList_Mouseover;
            this.ribbonButton.Name = "ribbonButton";
            this.ribbonButton.NormalImage = null;
            this.ribbonButton.Owner = null;
            this.ribbonButton.Size = new System.Drawing.Size(135, 37);
            this.ribbonButton.TabIndex = 133;
            this.ribbonButton.Text = "SOP 목록";
            this.ribbonButton.TextLocation = new System.Drawing.Point(35, 9);
            this.ribbonButton.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.ribbonButton.ToolTipText = "SOP 목록";
            this.ribbonButton.UseCustomImageRect = true;
            this.ribbonButton.UseTextLocation = true;
            this.ribbonButton.UseVisualStyleBackColor = false;
            // 
            // tabPageBuildingSignal
            // 
            this.tabPageBuildingSignal.BackColor = System.Drawing.Color.Transparent;
            this.tabPageBuildingSignal.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.tabFireSensor_Selected;
            this.tabPageBuildingSignal.CheckButton = false;
            this.tabPageBuildingSignal.CheckedBkgndImage = null;
            this.tabPageBuildingSignal.CheckedImage = null;
            this.tabPageBuildingSignal.CheckedMouseOver = null;
            this.tabPageBuildingSignal.ClickedBackgroundImage = null;
            this.tabPageBuildingSignal.ClickedImage = null;
            this.tabPageBuildingSignal.CustomImageRect = new System.Drawing.Rectangle(0, 0, 178, 37);
            this.tabPageBuildingSignal.DisabledBkgndImage = null;
            this.tabPageBuildingSignal.DisabledImage = null;
            this.tabPageBuildingSignal.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.tabPageBuildingSignal.ForeColor = System.Drawing.Color.White;
            this.tabPageBuildingSignal.ForeColorChecked = System.Drawing.Color.White;
            this.tabPageBuildingSignal.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.tabPageBuildingSignal.ForeColorDisabled = System.Drawing.Color.White;
            this.tabPageBuildingSignal.ForeColorMouseOver = System.Drawing.Color.White;
            this.tabPageBuildingSignal.ForeColorsByTypeUse = false;
            this.tabPageBuildingSignal.ID = -1;
            this.tabPageBuildingSignal.InitButtonWidth = 178;
            this.tabPageBuildingSignal.IsChecked = true;
            this.tabPageBuildingSignal.Location = new System.Drawing.Point(15, 75);
            this.tabPageBuildingSignal.MouseOverBkgndImage = null;
            this.tabPageBuildingSignal.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.tabFireSensor_Mouseover;
            this.tabPageBuildingSignal.Name = "tabPageBuildingSignal";
            this.tabPageBuildingSignal.NormalImage = null;
            this.tabPageBuildingSignal.Owner = null;
            this.tabPageBuildingSignal.Size = new System.Drawing.Size(178, 37);
            this.tabPageBuildingSignal.TabIndex = 132;
            this.tabPageBuildingSignal.Text = "누출신호(물질별)";
            this.tabPageBuildingSignal.TextLocation = new System.Drawing.Point(34, 9);
            this.tabPageBuildingSignal.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.tabPageBuildingSignal.ToolTipText = "누출신호(물질별)";
            this.tabPageBuildingSignal.UseCustomImageRect = true;
            this.tabPageBuildingSignal.UseTextLocation = true;
            this.tabPageBuildingSignal.UseVisualStyleBackColor = false;
            this.tabPageBuildingSignal.Click += new System.EventHandler(this.tabPageBuildingSignal_Click);
            // 
            // PopupSelectPSMSensorSOPLink
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1340, 790);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.ribbonButton);
            this.Controls.Add(this.tabPageBuildingSignal);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.plTitle);
            this.Controls.Add(this.plPageBuildingSignal);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PopupSelectPSMSensorSOPLink";
            this.Text = "PopupSelectPSMSensorSOPLink";
            this.Load += new System.EventHandler(this.PopupSelectPSMSensorSOPLink_Load);
            this.plTitle.ResumeLayout(false);
            this.plTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridSOP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLocation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridMaterial)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.plPageBuildingSignal.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel plTitle;
        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.DataGridView gridSOP;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsMenuSelectAll;
        private System.Windows.Forms.DataGridView gridLocation;
        private System.Windows.Forms.DataGridView gridMaterial;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMaterial;
        private System.Windows.Forms.PictureBox pbTitle;
        private System.Windows.Forms.Panel plPageBuildingSignal;
        private System.Windows.Forms.Panel panel3;
        private UnE.GUI.RibbonButton btnDeleteSOP;
        private UnE.GUI.RibbonButton btnChangeSOP;
        private UnE.GUI.RibbonButton btnNewSOP;
        private UnE.GUI.RibbonButton ribbonButton;
        private UnE.GUI.RibbonButton tabPageBuildingSignal;
        private UnE.GUI.RibbonButton btnCancel;
        private UnE.GUI.RibbonButton btnSave;
        private UnE.GUI.RibbonButton btnCancle;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSOPFullPath;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLocationNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLinkedSOP;
    }
}