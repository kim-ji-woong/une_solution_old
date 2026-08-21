namespace IntegratedManagement3.PopupDialog
{
    partial class SetChief
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetChief));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.colID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPosition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPhone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.treeViewTeam = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_CallerPhoneNumber = new System.Windows.Forms.TextBox();
            this.txt_DisplayText = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblRegular = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.ribbonButton1 = new UnE.GUI.RibbonButton();
            this.picRegular = new System.Windows.Forms.PictureBox();
            this.btn_cancel = new UnE.GUI.RibbonButton();
            this.btn_ok = new UnE.GUI.RibbonButton();
            this.picDay = new System.Windows.Forms.PictureBox();
            this.lblDay = new System.Windows.Forms.Label();
            this.lblNight = new System.Windows.Forms.Label();
            this.picNight = new System.Windows.Forms.PictureBox();
            this.picExternal = new System.Windows.Forms.PictureBox();
            this.lblExternal = new System.Windows.Forms.Label();
            this.picUserDefined = new System.Windows.Forms.PictureBox();
            this.lblUserDefined = new System.Windows.Forms.Label();
            this.picNormal = new System.Windows.Forms.PictureBox();
            this.lblNormal = new System.Windows.Forms.Label();
            this.picHoliday = new System.Windows.Forms.PictureBox();
            this.lblHoliday = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picRegular)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picExternal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUserDefined)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNormal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picHoliday)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle6.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridView1.ColumnHeadersHeight = 27;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colID,
            this.colName,
            this.colPosition,
            this.colPhone});
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridView1.Location = new System.Drawing.Point(198, 3);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.dataGridView1.RowHeadersVisible = false;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle10.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle10;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(353, 321);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellBorderStyleChanged += new System.EventHandler(this.dataGridView1_CellBorderStyleChanged);
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // colID
            // 
            this.colID.HeaderText = "ID";
            this.colID.Name = "colID";
            this.colID.ReadOnly = true;
            this.colID.Visible = false;
            // 
            // colName
            // 
            this.colName.HeaderText = "이름";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            // 
            // colPosition
            // 
            this.colPosition.HeaderText = "직위";
            this.colPosition.Name = "colPosition";
            this.colPosition.ReadOnly = true;
            // 
            // colPhone
            // 
            this.colPhone.HeaderText = "핸드폰번호";
            this.colPhone.Name = "colPhone";
            this.colPhone.ReadOnly = true;
            this.colPhone.Width = 150;
            // 
            // treeViewTeam
            // 
            this.treeViewTeam.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeViewTeam.Location = new System.Drawing.Point(3, 3);
            this.treeViewTeam.Name = "treeViewTeam";
            this.treeViewTeam.Size = new System.Drawing.Size(189, 321);
            this.treeViewTeam.TabIndex = 3;
            this.treeViewTeam.Click += new System.EventHandler(this.treeViewTeam_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(5, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "책임자 : ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(5, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "전화번호 : ";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_CallerPhoneNumber
            // 
            this.txt_CallerPhoneNumber.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txt_CallerPhoneNumber.Location = new System.Drawing.Point(114, 57);
            this.txt_CallerPhoneNumber.Name = "txt_CallerPhoneNumber";
            this.txt_CallerPhoneNumber.Size = new System.Drawing.Size(198, 26);
            this.txt_CallerPhoneNumber.TabIndex = 2;
            this.txt_CallerPhoneNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_CallerPhoneNumber_KeyPress);
            // 
            // txt_DisplayText
            // 
            this.txt_DisplayText.BackColor = System.Drawing.Color.White;
            this.txt_DisplayText.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txt_DisplayText.Location = new System.Drawing.Point(114, 25);
            this.txt_DisplayText.Name = "txt_DisplayText";
            this.txt_DisplayText.Size = new System.Drawing.Size(198, 26);
            this.txt_DisplayText.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txt_DisplayText);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txt_CallerPhoneNumber);
            this.groupBox1.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(25, 36);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(551, 100);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SOP 책임자 옵션";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.treeViewTeam);
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Location = new System.Drawing.Point(25, 218);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(555, 324);
            this.panel1.TabIndex = 12;
            // 
            // lblRegular
            // 
            this.lblRegular.AutoSize = true;
            this.lblRegular.BackColor = System.Drawing.Color.Transparent;
            this.lblRegular.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblRegular.ForeColor = System.Drawing.Color.White;
            this.lblRegular.Location = new System.Drawing.Point(46, 149);
            this.lblRegular.Name = "lblRegular";
            this.lblRegular.Size = new System.Drawing.Size(68, 18);
            this.lblRegular.TabIndex = 63;
            this.lblRegular.Text = "정규조직";
            this.lblRegular.Click += new System.EventHandler(this.Team_Regular_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.ribbonButton1);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(596, 27);
            this.panel2.TabIndex = 66;
            this.panel2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SetChief_MouseDown);
            this.panel2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SetChief_MouseMove);
            this.panel2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SetChief_MouseUp);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font(Program.prgFont, 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(4, 4);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 18);
            this.label4.TabIndex = 21;
            this.label4.Text = "책임자 설정";
            this.label4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SetChief_MouseDown);
            this.label4.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SetChief_MouseMove);
            this.label4.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SetChief_MouseUp);
            // 
            // ribbonButton1
            // 
            this.ribbonButton1.CheckButton = false;
            this.ribbonButton1.CheckedBkgndImage = null;
            this.ribbonButton1.CheckedImage = null;
            this.ribbonButton1.ClickedBackgroundImage = null;
            this.ribbonButton1.ClickedImage = global::IntegratedManagement3.Properties.Resources.Close_40_40_Click;
            this.ribbonButton1.CustomImageRect = new System.Drawing.Rectangle(0, 0, 22, 22);
            this.ribbonButton1.DisabledBkgndImage = null;
            this.ribbonButton1.DisabledImage = null;
            this.ribbonButton1.ID = -1;
            this.ribbonButton1.InitButtonWidth = 22;
            this.ribbonButton1.IsChecked = false;
            this.ribbonButton1.Location = new System.Drawing.Point(570, 2);
            this.ribbonButton1.Margin = new System.Windows.Forms.Padding(0);
            this.ribbonButton1.MouseOverBkgndImage = null;
            this.ribbonButton1.MouseOverImage = global::IntegratedManagement3.Properties.Resources.Close_40_40_Click;
            this.ribbonButton1.Name = "ribbonButton1";
            this.ribbonButton1.NormalImage = global::IntegratedManagement3.Properties.Resources.Close_40_40_Default;
            this.ribbonButton1.Owner = null;
            this.ribbonButton1.Size = new System.Drawing.Size(22, 22);
            this.ribbonButton1.TabIndex = 20;
            this.ribbonButton1.TextLocation = new System.Drawing.Point(0, 0);
            this.ribbonButton1.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.ribbonButton1.ToolTipText = "";
            this.ribbonButton1.UseCustomImageRect = true;
            this.ribbonButton1.UseTextLocation = false;
            this.ribbonButton1.UseVisualStyleBackColor = true;
            this.ribbonButton1.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // picRegular
            // 
            this.picRegular.BackColor = System.Drawing.Color.Transparent;
            this.picRegular.BackgroundImage = global::IntegratedManagement3.Properties.Resources.@__SOPEDIT_Enable2;
            this.picRegular.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picRegular.Location = new System.Drawing.Point(24, 146);
            this.picRegular.Margin = new System.Windows.Forms.Padding(0);
            this.picRegular.Name = "picRegular";
            this.picRegular.Size = new System.Drawing.Size(24, 24);
            this.picRegular.TabIndex = 62;
            this.picRegular.TabStop = false;
            this.picRegular.Click += new System.EventHandler(this.Team_Regular_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.CheckButton = false;
            this.btn_cancel.CheckedBkgndImage = null;
            this.btn_cancel.CheckedImage = null;
            this.btn_cancel.ClickedBackgroundImage = null;
            this.btn_cancel.ClickedImage = global::IntegratedManagement3.Properties.Resources.btnCancelClick;
            this.btn_cancel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 115, 45);
            this.btn_cancel.DisabledBkgndImage = null;
            this.btn_cancel.DisabledImage = null;
            this.btn_cancel.ID = -1;
            this.btn_cancel.InitButtonWidth = 120;
            this.btn_cancel.IsChecked = false;
            this.btn_cancel.Location = new System.Drawing.Point(466, 547);
            this.btn_cancel.Margin = new System.Windows.Forms.Padding(0);
            this.btn_cancel.MouseOverBkgndImage = null;
            this.btn_cancel.MouseOverImage = global::IntegratedManagement3.Properties.Resources.btnCancelClick;
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.NormalImage = global::IntegratedManagement3.Properties.Resources.btnCancel;
            this.btn_cancel.Owner = null;
            this.btn_cancel.Size = new System.Drawing.Size(120, 45);
            this.btn_cancel.TabIndex = 19;
            this.btn_cancel.TextLocation = new System.Drawing.Point(0, 0);
            this.btn_cancel.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btn_cancel.ToolTipText = "";
            this.btn_cancel.UseCustomImageRect = true;
            this.btn_cancel.UseTextLocation = false;
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // btn_ok
            // 
            this.btn_ok.CheckButton = false;
            this.btn_ok.CheckedBkgndImage = null;
            this.btn_ok.CheckedImage = null;
            this.btn_ok.ClickedBackgroundImage = null;
            this.btn_ok.ClickedImage = global::IntegratedManagement3.Properties.Resources.btnSettingClick;
            this.btn_ok.CustomImageRect = new System.Drawing.Rectangle(0, 0, 115, 45);
            this.btn_ok.DisabledBkgndImage = null;
            this.btn_ok.DisabledImage = null;
            this.btn_ok.ID = -1;
            this.btn_ok.InitButtonWidth = 120;
            this.btn_ok.IsChecked = false;
            this.btn_ok.Location = new System.Drawing.Point(353, 547);
            this.btn_ok.Margin = new System.Windows.Forms.Padding(0);
            this.btn_ok.MouseOverBkgndImage = null;
            this.btn_ok.MouseOverImage = global::IntegratedManagement3.Properties.Resources.btnSettingClick;
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.NormalImage = global::IntegratedManagement3.Properties.Resources.btnSetting;
            this.btn_ok.Owner = null;
            this.btn_ok.Size = new System.Drawing.Size(120, 45);
            this.btn_ok.TabIndex = 18;
            this.btn_ok.TextLocation = new System.Drawing.Point(0, 0);
            this.btn_ok.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btn_ok.ToolTipText = "";
            this.btn_ok.UseCustomImageRect = true;
            this.btn_ok.UseTextLocation = false;
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // picDay
            // 
            this.picDay.BackgroundImage = global::IntegratedManagement3.Properties.Resources.@__COMMON_ckb_enable;
            this.picDay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picDay.Location = new System.Drawing.Point(466, 150);
            this.picDay.Name = "picDay";
            this.picDay.Size = new System.Drawing.Size(22, 22);
            this.picDay.TabIndex = 74;
            this.picDay.TabStop = false;
            this.picDay.Click += new System.EventHandler(this.Day_Click);
            // 
            // lblDay
            // 
            this.lblDay.AutoSize = true;
            this.lblDay.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDay.ForeColor = System.Drawing.Color.White;
            this.lblDay.Location = new System.Drawing.Point(491, 152);
            this.lblDay.Name = "lblDay";
            this.lblDay.Size = new System.Drawing.Size(87, 18);
            this.lblDay.TabIndex = 75;
            this.lblDay.Text = "주간 책임자";
            this.lblDay.Click += new System.EventHandler(this.Day_Click);
            // 
            // lblNight
            // 
            this.lblNight.AutoSize = true;
            this.lblNight.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblNight.ForeColor = System.Drawing.Color.White;
            this.lblNight.Location = new System.Drawing.Point(491, 190);
            this.lblNight.Name = "lblNight";
            this.lblNight.Size = new System.Drawing.Size(87, 18);
            this.lblNight.TabIndex = 77;
            this.lblNight.Text = "야간 책임자";
            this.lblNight.Click += new System.EventHandler(this.Night_Click);
            // 
            // picNight
            // 
            this.picNight.BackgroundImage = global::IntegratedManagement3.Properties.Resources.@__COMMON_ckb_disable;
            this.picNight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picNight.Location = new System.Drawing.Point(466, 188);
            this.picNight.Name = "picNight";
            this.picNight.Size = new System.Drawing.Size(22, 22);
            this.picNight.TabIndex = 76;
            this.picNight.TabStop = false;
            this.picNight.Click += new System.EventHandler(this.Night_Click);
            // 
            // picExternal
            // 
            this.picExternal.BackColor = System.Drawing.Color.Transparent;
            this.picExternal.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picExternal.BackgroundImage")));
            this.picExternal.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picExternal.Location = new System.Drawing.Point(132, 146);
            this.picExternal.Margin = new System.Windows.Forms.Padding(0);
            this.picExternal.Name = "picExternal";
            this.picExternal.Size = new System.Drawing.Size(24, 24);
            this.picExternal.TabIndex = 78;
            this.picExternal.TabStop = false;
            this.picExternal.Click += new System.EventHandler(this.Team_External_Click);
            // 
            // lblExternal
            // 
            this.lblExternal.AutoSize = true;
            this.lblExternal.BackColor = System.Drawing.Color.Transparent;
            this.lblExternal.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblExternal.ForeColor = System.Drawing.Color.White;
            this.lblExternal.Location = new System.Drawing.Point(154, 149);
            this.lblExternal.Name = "lblExternal";
            this.lblExternal.Size = new System.Drawing.Size(68, 18);
            this.lblExternal.TabIndex = 79;
            this.lblExternal.Text = "외부조직";
            this.lblExternal.Click += new System.EventHandler(this.Team_External_Click);
            // 
            // picUserDefined
            // 
            this.picUserDefined.BackColor = System.Drawing.Color.Transparent;
            this.picUserDefined.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picUserDefined.BackgroundImage")));
            this.picUserDefined.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picUserDefined.Location = new System.Drawing.Point(231, 146);
            this.picUserDefined.Margin = new System.Windows.Forms.Padding(0);
            this.picUserDefined.Name = "picUserDefined";
            this.picUserDefined.Size = new System.Drawing.Size(24, 24);
            this.picUserDefined.TabIndex = 80;
            this.picUserDefined.TabStop = false;
            this.picUserDefined.Click += new System.EventHandler(this.Team_UserDefine_Click);
            // 
            // lblUserDefined
            // 
            this.lblUserDefined.AutoSize = true;
            this.lblUserDefined.BackColor = System.Drawing.Color.Transparent;
            this.lblUserDefined.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblUserDefined.ForeColor = System.Drawing.Color.White;
            this.lblUserDefined.Location = new System.Drawing.Point(253, 149);
            this.lblUserDefined.Name = "lblUserDefined";
            this.lblUserDefined.Size = new System.Drawing.Size(121, 18);
            this.lblUserDefined.TabIndex = 81;
            this.lblUserDefined.Text = "사용자 정의 조직";
            this.lblUserDefined.Click += new System.EventHandler(this.Team_UserDefine_Click);
            // 
            // picNormal
            // 
            this.picNormal.BackColor = System.Drawing.Color.Transparent;
            this.picNormal.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picNormal.BackgroundImage")));
            this.picNormal.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picNormal.Location = new System.Drawing.Point(24, 184);
            this.picNormal.Margin = new System.Windows.Forms.Padding(0);
            this.picNormal.Name = "picNormal";
            this.picNormal.Size = new System.Drawing.Size(24, 24);
            this.picNormal.TabIndex = 82;
            this.picNormal.TabStop = false;
            this.picNormal.Click += new System.EventHandler(this.Team_Normal_Click);
            // 
            // lblNormal
            // 
            this.lblNormal.AutoSize = true;
            this.lblNormal.BackColor = System.Drawing.Color.Transparent;
            this.lblNormal.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblNormal.ForeColor = System.Drawing.Color.White;
            this.lblNormal.Location = new System.Drawing.Point(46, 187);
            this.lblNormal.Name = "lblNormal";
            this.lblNormal.Size = new System.Drawing.Size(106, 18);
            this.lblNormal.TabIndex = 83;
            this.lblNormal.Text = "평일 비상 조직";
            this.lblNormal.Click += new System.EventHandler(this.Team_Normal_Click);
            // 
            // picHoliday
            // 
            this.picHoliday.BackColor = System.Drawing.Color.Transparent;
            this.picHoliday.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picHoliday.BackgroundImage")));
            this.picHoliday.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picHoliday.Location = new System.Drawing.Point(197, 184);
            this.picHoliday.Margin = new System.Windows.Forms.Padding(0);
            this.picHoliday.Name = "picHoliday";
            this.picHoliday.Size = new System.Drawing.Size(24, 24);
            this.picHoliday.TabIndex = 84;
            this.picHoliday.TabStop = false;
            this.picHoliday.Click += new System.EventHandler(this.Team_Emergency_Click);
            // 
            // lblHoliday
            // 
            this.lblHoliday.AutoSize = true;
            this.lblHoliday.BackColor = System.Drawing.Color.Transparent;
            this.lblHoliday.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblHoliday.ForeColor = System.Drawing.Color.White;
            this.lblHoliday.Location = new System.Drawing.Point(219, 187);
            this.lblHoliday.Name = "lblHoliday";
            this.lblHoliday.Size = new System.Drawing.Size(155, 18);
            this.lblHoliday.TabIndex = 85;
            this.lblHoliday.Text = "야간 및 휴일 비상조직";
            this.lblHoliday.Click += new System.EventHandler(this.Team_Emergency_Click);
            // 
            // SetChief
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.ClientSize = new System.Drawing.Size(596, 596);
            this.Controls.Add(this.picHoliday);
            this.Controls.Add(this.lblHoliday);
            this.Controls.Add(this.picNormal);
            this.Controls.Add(this.lblNormal);
            this.Controls.Add(this.picUserDefined);
            this.Controls.Add(this.lblUserDefined);
            this.Controls.Add(this.picExternal);
            this.Controls.Add(this.lblExternal);
            this.Controls.Add(this.lblNight);
            this.Controls.Add(this.picNight);
            this.Controls.Add(this.lblDay);
            this.Controls.Add(this.picDay);
            this.Controls.Add(this.picRegular);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.lblRegular);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SetChief";
            this.Text = "책임자 설정";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SetChief_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SetChief_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SetChief_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picRegular)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picExternal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUserDefined)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNormal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picHoliday)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TreeView treeViewTeam;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_CallerPhoneNumber;
        private System.Windows.Forms.TextBox txt_DisplayText;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPosition;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPhone;
        private UnE.GUI.RibbonButton btn_cancel;
        private UnE.GUI.RibbonButton btn_ok;
        private UnE.GUI.RibbonButton ribbonButton1;
        private System.Windows.Forms.Label lblRegular;
        private System.Windows.Forms.PictureBox picRegular;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox picDay;
        private System.Windows.Forms.Label lblDay;
        private System.Windows.Forms.Label lblNight;
        private System.Windows.Forms.PictureBox picNight;
        private System.Windows.Forms.PictureBox picExternal;
        private System.Windows.Forms.Label lblExternal;
        private System.Windows.Forms.PictureBox picUserDefined;
        private System.Windows.Forms.Label lblUserDefined;
        private System.Windows.Forms.PictureBox picNormal;
        private System.Windows.Forms.Label lblNormal;
        private System.Windows.Forms.PictureBox picHoliday;
        private System.Windows.Forms.Label lblHoliday;
    }
}