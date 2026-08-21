namespace SOPMonitoringSystem
{
    partial class FormMissionStatus
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
            this.dataGridViewPrev = new System.Windows.Forms.DataGridView();
            this.dataGridViewNext = new System.Windows.Forms.DataGridView();
            this.dataGridViewCurrent = new System.Windows.Forms.DataGridView();
            this.labelTitle = new System.Windows.Forms.Label();
            this.pictureBoxLogo = new PictureBoxDB();
            this.pictureBoxTitle1Name = new PictureBoxDB();
            this.pictureBoxTitle1BG = new PictureBoxDB();
            this.pictureBoxTitlebar = new PictureBoxDB();
            this.pictureBoxNextTail = new PictureBoxDB();
            this.pictureBoxCurrentTail = new PictureBoxDB();
            this.pictureBoxPrevTail = new PictureBoxDB();
            this.pictureBoxNextBody = new PictureBoxDB();
            this.pictureBoxCurrentBody = new PictureBoxDB();
            this.pictureBoxPrevBody = new PictureBoxDB();
            this.pictureBoxNextHeader = new PictureBoxDB();
            this.pictureBoxCurrentHeader = new PictureBoxDB();
            this.pictureBoxPrevHeader = new PictureBoxDB();
            this.cmsMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuInitialize = new System.Windows.Forms.ToolStripMenuItem();
            this.btnClose = new System.Windows.Forms.Button();
            this.panelCurrentBody = new System.Windows.Forms.Panel();
            this.labelCurrentTitle = new System.Windows.Forms.Label();
            this.labelTarget = new System.Windows.Forms.Label();
            this.labelPrevTitle = new System.Windows.Forms.Label();
            this.panelPrevBody = new System.Windows.Forms.Panel();
            this.panelNextBody = new System.Windows.Forms.Panel();
            this.labelNextTitle = new System.Windows.Forms.Label();
            this.colPrevTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPrevContents = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNextTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNextContents = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCurrentSender = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCurrentContents = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCurrentSendSMS = new System.Windows.Forms.DataGridViewImageColumn();
            this.colCurrentComplate = new System.Windows.Forms.DataGridViewImageColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewNext)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCurrent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle1Name)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle1BG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitlebar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNextTail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCurrentTail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPrevTail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNextBody)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCurrentBody)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPrevBody)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNextHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCurrentHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPrevHeader)).BeginInit();
            this.cmsMain.SuspendLayout();
            this.panelCurrentBody.SuspendLayout();
            this.panelPrevBody.SuspendLayout();
            this.panelNextBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPrev)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewNext
            // 
            this.dataGridViewNext.AllowUserToAddRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("맑은 고딕", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewNext.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewNext.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewNext.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNextTitle,
            this.colNextContents});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewNext.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewNext.Location = new System.Drawing.Point(12, 425);
            this.dataGridViewNext.Name = "dataGridViewNext";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewNext.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewNext.RowHeadersVisible = false;
            this.dataGridViewNext.RowTemplate.Height = 23;
            this.dataGridViewNext.Size = new System.Drawing.Size(384, 141);
            this.dataGridViewNext.TabIndex = 0;
            this.dataGridViewNext.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // dataGridViewCurrent
            // 
            this.dataGridViewCurrent.AllowUserToAddRows = false;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("맑은 고딕", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewCurrent.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewCurrent.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCurrent.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCurrentSender,
            this.colCurrentContents,
            this.colCurrentSendSMS,
            this.colCurrentComplate});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewCurrent.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewCurrent.Location = new System.Drawing.Point(626, 210);
            this.dataGridViewCurrent.Name = "dataGridViewCurrent";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewCurrent.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewCurrent.RowHeadersVisible = false;
            this.dataGridViewCurrent.RowTemplate.Height = 23;
            this.dataGridViewCurrent.Size = new System.Drawing.Size(549, 356);
            this.dataGridViewCurrent.TabIndex = 1;
            this.dataGridViewCurrent.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("맑은 고딕", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTitle.ForeColor = System.Drawing.Color.Navy;
            this.labelTitle.Location = new System.Drawing.Point(19, 104);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(134, 37);
            this.labelTitle.TabIndex = 8;
            this.labelTitle.Text = "SOP 제목";
            this.labelTitle.DoubleClick += new System.EventHandler(this.FormMissionStatus_DoubleClick);
            this.labelTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Image = global::SOPMonitoringSystem.Properties.Resources.namdong_logo_new;
            this.pictureBoxLogo.Location = new System.Drawing.Point(911, 9);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(191, 36);
            this.pictureBoxLogo.TabIndex = 11;
            this.pictureBoxLogo.TabStop = false;
            this.pictureBoxLogo.DoubleClick += new System.EventHandler(this.FormMissionStatus_DoubleClick);
            this.pictureBoxLogo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // pictureBoxTitle1Name
            // 
            this.pictureBoxTitle1Name.Image = global::SOPMonitoringSystem.Properties.Resources.Title1_name;
            this.pictureBoxTitle1Name.Location = new System.Drawing.Point(0, 4);
            this.pictureBoxTitle1Name.Name = "pictureBoxTitle1Name";
            this.pictureBoxTitle1Name.Size = new System.Drawing.Size(910, 50);
            this.pictureBoxTitle1Name.TabIndex = 10;
            this.pictureBoxTitle1Name.TabStop = false;
            this.pictureBoxTitle1Name.DoubleClick += new System.EventHandler(this.FormMissionStatus_DoubleClick);
            this.pictureBoxTitle1Name.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // pictureBoxTitle1BG
            // 
            this.pictureBoxTitle1BG.Image = global::SOPMonitoringSystem.Properties.Resources.Title1_bg;
            this.pictureBoxTitle1BG.Location = new System.Drawing.Point(0, 4);
            this.pictureBoxTitle1BG.Name = "pictureBoxTitle1BG";
            this.pictureBoxTitle1BG.Size = new System.Drawing.Size(1200, 55);
            this.pictureBoxTitle1BG.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxTitle1BG.TabIndex = 9;
            this.pictureBoxTitle1BG.TabStop = false;
            this.pictureBoxTitle1BG.DoubleClick += new System.EventHandler(this.FormMissionStatus_DoubleClick);
            this.pictureBoxTitle1BG.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // pictureBoxTitlebar
            // 
            this.pictureBoxTitlebar.Image = global::SOPMonitoringSystem.Properties.Resources.Titlebar;
            this.pictureBoxTitlebar.Location = new System.Drawing.Point(0, 101);
            this.pictureBoxTitlebar.Name = "pictureBoxTitlebar";
            this.pictureBoxTitlebar.Size = new System.Drawing.Size(1197, 50);
            this.pictureBoxTitlebar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxTitlebar.TabIndex = 7;
            this.pictureBoxTitlebar.TabStop = false;
            this.pictureBoxTitlebar.DoubleClick += new System.EventHandler(this.FormMissionStatus_DoubleClick);
            this.pictureBoxTitlebar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // pictureBoxNextTail
            // 
            this.pictureBoxNextTail.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxNextTail.Image = global::SOPMonitoringSystem.Properties.Resources.mission_tail;
            this.pictureBoxNextTail.Location = new System.Drawing.Point(268, 386);
            this.pictureBoxNextTail.Name = "pictureBoxNextTail";
            this.pictureBoxNextTail.Size = new System.Drawing.Size(35, 60);
            this.pictureBoxNextTail.TabIndex = 4;
            this.pictureBoxNextTail.TabStop = false;
            this.pictureBoxNextTail.DoubleClick += new System.EventHandler(this.FormMissionStatus_DoubleClick);
            this.pictureBoxNextTail.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // pictureBoxCurrentTail
            // 
            this.pictureBoxCurrentTail.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCurrentTail.Image = global::SOPMonitoringSystem.Properties.Resources.current_mission_tail;
            this.pictureBoxCurrentTail.Location = new System.Drawing.Point(946, 171);
            this.pictureBoxCurrentTail.Name = "pictureBoxCurrentTail";
            this.pictureBoxCurrentTail.Size = new System.Drawing.Size(35, 60);
            this.pictureBoxCurrentTail.TabIndex = 4;
            this.pictureBoxCurrentTail.TabStop = false;
            this.pictureBoxCurrentTail.DoubleClick += new System.EventHandler(this.FormMissionStatus_DoubleClick);
            this.pictureBoxCurrentTail.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // pictureBoxPrevTail
            // 
            this.pictureBoxPrevTail.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxPrevTail.Image = global::SOPMonitoringSystem.Properties.Resources.mission_tail;
            this.pictureBoxPrevTail.Location = new System.Drawing.Point(268, 171);
            this.pictureBoxPrevTail.Name = "pictureBoxPrevTail";
            this.pictureBoxPrevTail.Size = new System.Drawing.Size(35, 60);
            this.pictureBoxPrevTail.TabIndex = 4;
            this.pictureBoxPrevTail.TabStop = false;
            this.pictureBoxPrevTail.DoubleClick += new System.EventHandler(this.FormMissionStatus_DoubleClick);
            this.pictureBoxPrevTail.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // pictureBoxNextBody
            // 
            this.pictureBoxNextBody.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxNextBody.Image = global::SOPMonitoringSystem.Properties.Resources.mission_body;
            this.pictureBoxNextBody.Location = new System.Drawing.Point(168, 386);
            this.pictureBoxNextBody.Name = "pictureBoxNextBody";
            this.pictureBoxNextBody.Size = new System.Drawing.Size(100, 60);
            this.pictureBoxNextBody.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxNextBody.TabIndex = 3;
            this.pictureBoxNextBody.TabStop = false;
            this.pictureBoxNextBody.DoubleClick += new System.EventHandler(this.FormMissionStatus_DoubleClick);
            this.pictureBoxNextBody.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // pictureBoxCurrentBody
            // 
            this.pictureBoxCurrentBody.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCurrentBody.Image = global::SOPMonitoringSystem.Properties.Resources.current_mission_body;
            this.pictureBoxCurrentBody.Location = new System.Drawing.Point(846, 171);
            this.pictureBoxCurrentBody.Name = "pictureBoxCurrentBody";
            this.pictureBoxCurrentBody.Size = new System.Drawing.Size(100, 60);
            this.pictureBoxCurrentBody.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxCurrentBody.TabIndex = 3;
            this.pictureBoxCurrentBody.TabStop = false;
            this.pictureBoxCurrentBody.DoubleClick += new System.EventHandler(this.FormMissionStatus_DoubleClick);
            this.pictureBoxCurrentBody.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // pictureBoxPrevBody
            // 
            this.pictureBoxPrevBody.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxPrevBody.Image = global::SOPMonitoringSystem.Properties.Resources.mission_body;
            this.pictureBoxPrevBody.Location = new System.Drawing.Point(168, 171);
            this.pictureBoxPrevBody.Name = "pictureBoxPrevBody";
            this.pictureBoxPrevBody.Size = new System.Drawing.Size(100, 60);
            this.pictureBoxPrevBody.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxPrevBody.TabIndex = 3;
            this.pictureBoxPrevBody.TabStop = false;
            this.pictureBoxPrevBody.DoubleClick += new System.EventHandler(this.FormMissionStatus_DoubleClick);
            this.pictureBoxPrevBody.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // pictureBoxNextHeader
            // 
            this.pictureBoxNextHeader.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxNextHeader.Image = global::SOPMonitoringSystem.Properties.Resources.next_mission_header;
            this.pictureBoxNextHeader.Location = new System.Drawing.Point(12, 386);
            this.pictureBoxNextHeader.Name = "pictureBoxNextHeader";
            this.pictureBoxNextHeader.Size = new System.Drawing.Size(200, 60);
            this.pictureBoxNextHeader.TabIndex = 2;
            this.pictureBoxNextHeader.TabStop = false;
            this.pictureBoxNextHeader.DoubleClick += new System.EventHandler(this.FormMissionStatus_DoubleClick);
            this.pictureBoxNextHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // pictureBoxCurrentHeader
            // 
            this.pictureBoxCurrentHeader.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCurrentHeader.Image = global::SOPMonitoringSystem.Properties.Resources.current_mission_header;
            this.pictureBoxCurrentHeader.Location = new System.Drawing.Point(626, 171);
            this.pictureBoxCurrentHeader.Name = "pictureBoxCurrentHeader";
            this.pictureBoxCurrentHeader.Size = new System.Drawing.Size(200, 60);
            this.pictureBoxCurrentHeader.TabIndex = 2;
            this.pictureBoxCurrentHeader.TabStop = false;
            this.pictureBoxCurrentHeader.DoubleClick += new System.EventHandler(this.FormMissionStatus_DoubleClick);
            this.pictureBoxCurrentHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // pictureBoxPrevHeader
            // 
            this.pictureBoxPrevHeader.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxPrevHeader.Image = global::SOPMonitoringSystem.Properties.Resources.prev_mission_header;
            this.pictureBoxPrevHeader.Location = new System.Drawing.Point(12, 171);
            this.pictureBoxPrevHeader.Name = "pictureBoxPrevHeader";
            this.pictureBoxPrevHeader.Size = new System.Drawing.Size(200, 60);
            this.pictureBoxPrevHeader.TabIndex = 2;
            this.pictureBoxPrevHeader.TabStop = false;
            this.pictureBoxPrevHeader.DoubleClick += new System.EventHandler(this.FormMissionStatus_DoubleClick);
            this.pictureBoxPrevHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // cmsMain
            // 
            this.cmsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuInitialize});
            this.cmsMain.Name = "contextMenuStrip1";
            this.cmsMain.Size = new System.Drawing.Size(111, 26);
            // 
            // tsMenuInitialize
            // 
            this.tsMenuInitialize.Name = "tsMenuInitialize";
            this.tsMenuInitialize.Size = new System.Drawing.Size(110, 22);
            this.tsMenuInitialize.Text = "초기화";
            this.tsMenuInitialize.Click += new System.EventHandler(this.tsMenuInitialize_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.CloseWindow_Normal;
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnClose.Location = new System.Drawing.Point(1135, -1);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(64, 48);
            this.btnClose.TabIndex = 12;
            this.btnClose.UseVisualStyleBackColor = false;
            // 
            // panelCurrentBody
            // 
            this.panelCurrentBody.BackColor = System.Drawing.Color.Transparent;
            this.panelCurrentBody.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.current_mission_body;
            this.panelCurrentBody.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelCurrentBody.Controls.Add(this.labelCurrentTitle);
            this.panelCurrentBody.Controls.Add(this.labelTarget);
            this.panelCurrentBody.Location = new System.Drawing.Point(832, 171);
            this.panelCurrentBody.Name = "panelCurrentBody";
            this.panelCurrentBody.Size = new System.Drawing.Size(100, 60);
            this.panelCurrentBody.TabIndex = 13;
            this.panelCurrentBody.DoubleClick += new System.EventHandler(this.FormMissionStatus_DoubleClick);
            this.panelCurrentBody.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // labelCurrentTitle
            // 
            this.labelCurrentTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelCurrentTitle.AutoSize = true;
            this.labelCurrentTitle.Font = new System.Drawing.Font("맑은 고딕", 17.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelCurrentTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.labelCurrentTitle.Location = new System.Drawing.Point(15, 8);
            this.labelCurrentTitle.Name = "labelCurrentTitle";
            this.labelCurrentTitle.Size = new System.Drawing.Size(115, 31);
            this.labelCurrentTitle.TabIndex = 17;
            this.labelCurrentTitle.Text = "미션 Title";
            // 
            // labelTarget
            // 
            this.labelTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelTarget.AutoSize = true;
            this.labelTarget.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTarget.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.labelTarget.Location = new System.Drawing.Point(40, 41);
            this.labelTarget.Name = "labelTarget";
            this.labelTarget.Size = new System.Drawing.Size(47, 17);
            this.labelTarget.TabIndex = 0;
            this.labelTarget.Text = "수신자";
            this.labelTarget.DoubleClick += new System.EventHandler(this.FormMissionStatus_DoubleClick);
            this.labelTarget.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // labelPrevTitle
            // 
            this.labelPrevTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelPrevTitle.AutoSize = true;
            this.labelPrevTitle.Font = new System.Drawing.Font("맑은 고딕", 17.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelPrevTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.labelPrevTitle.Location = new System.Drawing.Point(15, 13);
            this.labelPrevTitle.Name = "labelPrevTitle";
            this.labelPrevTitle.Size = new System.Drawing.Size(115, 31);
            this.labelPrevTitle.TabIndex = 14;
            this.labelPrevTitle.Text = "미션 Title";
            // 
            // panelPrevBody
            // 
            this.panelPrevBody.BackColor = System.Drawing.Color.Transparent;
            this.panelPrevBody.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.mission_body;
            this.panelPrevBody.Controls.Add(this.labelPrevTitle);
            this.panelPrevBody.Location = new System.Drawing.Point(420, 210);
            this.panelPrevBody.Name = "panelPrevBody";
            this.panelPrevBody.Size = new System.Drawing.Size(100, 60);
            this.panelPrevBody.TabIndex = 15;
            // 
            // panelNextBody
            // 
            this.panelNextBody.BackColor = System.Drawing.Color.Transparent;
            this.panelNextBody.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.mission_body;
            this.panelNextBody.Controls.Add(this.labelNextTitle);
            this.panelNextBody.Location = new System.Drawing.Point(446, 386);
            this.panelNextBody.Name = "panelNextBody";
            this.panelNextBody.Size = new System.Drawing.Size(100, 60);
            this.panelNextBody.TabIndex = 16;
            // 
            // labelNextTitle
            // 
            this.labelNextTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelNextTitle.AutoSize = true;
            this.labelNextTitle.Font = new System.Drawing.Font("맑은 고딕", 17.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelNextTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.labelNextTitle.Location = new System.Drawing.Point(15, 13);
            this.labelNextTitle.Name = "labelNextTitle";
            this.labelNextTitle.Size = new System.Drawing.Size(115, 31);
            this.labelNextTitle.TabIndex = 14;
            this.labelNextTitle.Text = "미션 Title";
            // 
            // dataGridViewPrev
            // 
            this.dataGridViewPrev.AllowUserToAddRows = false;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("맑은 고딕", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewPrev.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridViewPrev.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPrev.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPrevTitle,
            this.colPrevContents});
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewPrev.DefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridViewPrev.Location = new System.Drawing.Point(12, 210);
            this.dataGridViewPrev.Name = "dataGridViewPrev";
            this.dataGridViewPrev.ReadOnly = true;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewPrev.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.dataGridViewPrev.RowHeadersVisible = false;
            this.dataGridViewPrev.RowTemplate.Height = 23;
            this.dataGridViewPrev.Size = new System.Drawing.Size(384, 154);
            this.dataGridViewPrev.TabIndex = 0;
            this.dataGridViewPrev.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // colPrevTitle
            // 
            this.colPrevTitle.HeaderText = "실행자";
            this.colPrevTitle.Name = "colPrevTitle";
            this.colPrevTitle.ReadOnly = true;
            this.colPrevTitle.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colPrevTitle.Width = 225;
            // 
            // colPrevContents
            // 
            this.colPrevContents.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colPrevContents.HeaderText = "임무내용";
            this.colPrevContents.Name = "colPrevContents";
            this.colPrevContents.ReadOnly = true;
            this.colPrevContents.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colNextTitle
            // 
            this.colNextTitle.HeaderText = "실행자";
            this.colNextTitle.Name = "colNextTitle";
            this.colNextTitle.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNextTitle.Width = 225;
            // 
            // colNextContents
            // 
            this.colNextContents.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colNextContents.HeaderText = "임무내용";
            this.colNextContents.Name = "colNextContents";
            this.colNextContents.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colCurrentSender
            // 
            this.colCurrentSender.HeaderText = "실행자";
            this.colCurrentSender.Name = "colCurrentSender";
            this.colCurrentSender.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colCurrentSender.Width = 225;
            // 
            // colCurrentContents
            // 
            this.colCurrentContents.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colCurrentContents.HeaderText = "임무내용";
            this.colCurrentContents.Name = "colCurrentContents";
            this.colCurrentContents.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colCurrentSendSMS
            // 
            this.colCurrentSendSMS.HeaderText = "문자";
            this.colCurrentSendSMS.Name = "colCurrentSendSMS";
            this.colCurrentSendSMS.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colCurrentSendSMS.Width = 94;
            // 
            // colCurrentComplate
            // 
            this.colCurrentComplate.HeaderText = "완료";
            this.colCurrentComplate.Name = "colCurrentComplate";
            this.colCurrentComplate.Width = 94;
            // 
            // FormMissionStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.bg_missionStatus;
            this.ClientSize = new System.Drawing.Size(1199, 640);
            this.Controls.Add(this.panelNextBody);
            this.Controls.Add(this.panelPrevBody);
            this.Controls.Add(this.panelCurrentBody);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.pictureBoxLogo);
            this.Controls.Add(this.pictureBoxTitle1Name);
            this.Controls.Add(this.pictureBoxTitle1BG);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.pictureBoxTitlebar);
            this.Controls.Add(this.pictureBoxNextTail);
            this.Controls.Add(this.pictureBoxCurrentTail);
            this.Controls.Add(this.pictureBoxPrevTail);
            this.Controls.Add(this.pictureBoxNextBody);
            this.Controls.Add(this.pictureBoxCurrentBody);
            this.Controls.Add(this.pictureBoxPrevBody);
            this.Controls.Add(this.pictureBoxNextHeader);
            this.Controls.Add(this.pictureBoxCurrentHeader);
            this.Controls.Add(this.pictureBoxPrevHeader);
            this.Controls.Add(this.dataGridViewCurrent);
            this.Controls.Add(this.dataGridViewNext);
            this.Controls.Add(this.dataGridViewPrev);
            this.Name = "FormMissionStatus";
            this.Text = "실시간 임무 현황";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMissionStatus_FormClosing);
            this.Load += new System.EventHandler(this.FormMissionStatus_Load);
            this.DoubleClick += new System.EventHandler(this.FormMissionStatus_DoubleClick);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormMissionStatus_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPrev)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewNext)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCurrent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle1Name)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle1BG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitlebar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNextTail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCurrentTail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPrevTail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNextBody)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCurrentBody)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPrevBody)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNextHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCurrentHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPrevHeader)).EndInit();
            this.cmsMain.ResumeLayout(false);
            this.panelCurrentBody.ResumeLayout(false);
            this.panelCurrentBody.PerformLayout();
            this.panelPrevBody.ResumeLayout(false);
            this.panelPrevBody.PerformLayout();
            this.panelNextBody.ResumeLayout(false);
            this.panelNextBody.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewPrev;
        private System.Windows.Forms.DataGridView dataGridViewNext;
        private System.Windows.Forms.DataGridView dataGridViewCurrent;
        private System.Windows.Forms.PictureBox pictureBoxPrevHeader;
        private System.Windows.Forms.PictureBox pictureBoxPrevBody;
        private System.Windows.Forms.PictureBox pictureBoxPrevTail;
        private System.Windows.Forms.PictureBox pictureBoxNextHeader;
        private System.Windows.Forms.PictureBox pictureBoxNextBody;
        private System.Windows.Forms.PictureBox pictureBoxNextTail;
        private System.Windows.Forms.PictureBox pictureBoxCurrentHeader;
        private System.Windows.Forms.PictureBox pictureBoxCurrentBody;
        private System.Windows.Forms.PictureBox pictureBoxCurrentTail;
        private System.Windows.Forms.PictureBox pictureBoxTitlebar;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.PictureBox pictureBoxTitle1BG;
        private System.Windows.Forms.PictureBox pictureBoxTitle1Name;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.ContextMenuStrip cmsMain;
        private System.Windows.Forms.ToolStripMenuItem tsMenuInitialize;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panelCurrentBody;
        private System.Windows.Forms.Label labelTarget;
        private System.Windows.Forms.Label labelCurrentTitle;
        private System.Windows.Forms.Label labelPrevTitle;
        private System.Windows.Forms.Panel panelPrevBody;
        private System.Windows.Forms.Panel panelNextBody;
        private System.Windows.Forms.Label labelNextTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNextTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNextContents;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCurrentSender;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCurrentContents;
        private System.Windows.Forms.DataGridViewImageColumn colCurrentSendSMS;
        private System.Windows.Forms.DataGridViewImageColumn colCurrentComplate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrevTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrevContents;
    }
}