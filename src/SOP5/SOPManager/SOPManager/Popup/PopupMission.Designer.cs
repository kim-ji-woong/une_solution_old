namespace SOPManager
{
    partial class PopupMission
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopupMission));
            this.textBox = new System.Windows.Forms.TextBox();
            this.labelWarning = new System.Windows.Forms.Label();
            this.labelNote = new System.Windows.Forms.Label();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.textBoxCommander = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSelectTeam = new System.Windows.Forms.TextBox();
            this.btnSelectCommander = new UnE.GUI.RibbonButton();
            this.btnSelectTeam = new UnE.GUI.RibbonButton();
            this.picAutoRun = new System.Windows.Forms.PictureBox();
            this.btnCancel = new UnE.GUI.RibbonButton();
            this.btnOK = new UnE.GUI.RibbonButton();
            this.btnShowSpecialMessage = new UnE.GUI.RibbonButton();
            this.btnDown = new UnE.GUI.RibbonButton();
            this.btnUp = new UnE.GUI.RibbonButton();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblAutoRun = new System.Windows.Forms.Label();
            this.checkBoxAutoRun = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAutoRun)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.textBox.Location = new System.Drawing.Point(112, 7);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(697, 27);
            this.textBox.TabIndex = 3;
            // 
            // labelWarning
            // 
            this.labelWarning.BackColor = System.Drawing.Color.Transparent;
            this.labelWarning.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelWarning.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            this.labelWarning.Location = new System.Drawing.Point(12, 437);
            this.labelWarning.Name = "labelWarning";
            this.labelWarning.Size = new System.Drawing.Size(791, 18);
            this.labelWarning.TabIndex = 1;
            this.labelWarning.Text = "(외부로 임무 내용이 전파될 수 있으므로, 개인정보 보호를 위해서 특정 개인의 정보는 입력하지 말아 주십시오.)\r\n";
            // 
            // labelNote
            // 
            this.labelNote.AutoSize = true;
            this.labelNote.BackColor = System.Drawing.Color.Transparent;
            this.labelNote.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelNote.ForeColor = System.Drawing.Color.White;
            this.labelNote.Location = new System.Drawing.Point(11, 12);
            this.labelNote.Name = "labelNote";
            this.labelNote.Size = new System.Drawing.Size(97, 17);
            this.labelNote.TabIndex = 0;
            this.labelNote.Text = "SOP 단계 :";
            // 
            // dataGridView
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            this.dataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.ColumnHeadersVisible = false;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column5,
            this.Column2,
            this.Column4,
            this.Column3});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridView.GridColor = System.Drawing.Color.Black;
            this.dataGridView.Location = new System.Drawing.Point(15, 130);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle7.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.White;
            this.dataGridView.RowsDefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridView.RowTemplate.Height = 30;
            this.dataGridView.Size = new System.Drawing.Size(794, 300);
            this.dataGridView.TabIndex = 17;
            this.dataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellClick);
            this.dataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellContentClick);
            this.dataGridView.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellContentDoubleClick);
            this.dataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellDoubleClick);
            this.dataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellEndEdit);
            this.dataGridView.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView_DefaultValuesNeeded);
            this.dataGridView.NewRowNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView_NewRowNeeded);
            this.dataGridView.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView_RowsAdded);
            this.dataGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView_KeyDown);
            // 
            // Column1
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column1.DefaultCellStyle = dataGridViewCellStyle3;
            this.Column1.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.Column1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Column1.HeaderText = "전파방법";
            this.Column1.Items.AddRange(new object[] {
            "구두",
            "전화",
            "무전기",
            "기타"});
            this.Column1.Name = "Column1";
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Column1.Visible = false;
            this.Column1.Width = 110;
            // 
            // Column5
            // 
            this.Column5.FillWeight = 220F;
            this.Column5.HeaderText = "멘트수행자";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.Visible = false;
            this.Column5.Width = 220;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.Column2.DefaultCellStyle = dataGridViewCellStyle4;
            this.Column2.HeaderText = "내용";
            this.Column2.Name = "Column2";
            // 
            // Column4
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column4.DefaultCellStyle = dataGridViewCellStyle5;
            this.Column4.HeaderText = "임무대상";
            this.Column4.Name = "Column4";
            this.Column4.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column4.Visible = false;
            this.Column4.Width = 120;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "...";
            this.Column3.Name = "Column3";
            this.Column3.Visible = false;
            this.Column3.Width = 30;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(16, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(792, 26);
            this.label2.TabIndex = 18;
            this.label2.Text = "임무내용";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(748, 80);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 16);
            this.label5.TabIndex = 20;
            this.label5.Text = "대상자";
            this.label5.Visible = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(3);
            this.panel2.Size = new System.Drawing.Size(829, 510);
            this.panel2.TabIndex = 22;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.textBoxCommander);
            this.panel3.Controls.Add(this.textBox);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.labelNote);
            this.panel3.Controls.Add(this.txtSelectTeam);
            this.panel3.Controls.Add(this.btnSelectCommander);
            this.panel3.Controls.Add(this.btnSelectTeam);
            this.panel3.Controls.Add(this.picAutoRun);
            this.panel3.Controls.Add(this.btnCancel);
            this.panel3.Controls.Add(this.btnOK);
            this.panel3.Controls.Add(this.btnShowSpecialMessage);
            this.panel3.Controls.Add(this.btnDown);
            this.panel3.Controls.Add(this.btnUp);
            this.panel3.Controls.Add(this.label6);
            this.panel3.Controls.Add(this.labelWarning);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.dataGridView);
            this.panel3.Controls.Add(this.lblAutoRun);
            this.panel3.Controls.Add(this.checkBoxAutoRun);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(823, 504);
            this.panel3.TabIndex = 0;
            // 
            // textBoxCommander
            // 
            this.textBoxCommander.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.textBoxCommander.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxCommander.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxCommander.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.textBoxCommander.Location = new System.Drawing.Point(323, 462);
            this.textBoxCommander.Name = "textBoxCommander";
            this.textBoxCommander.ReadOnly = true;
            this.textBoxCommander.Size = new System.Drawing.Size(252, 27);
            this.textBoxCommander.TabIndex = 22;
            this.textBoxCommander.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(32, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 17);
            this.label1.TabIndex = 17;
            this.label1.Text = "수신자 :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(243, 466);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 17);
            this.label3.TabIndex = 21;
            this.label3.Text = "발신자 :";
            this.label3.Visible = false;
            // 
            // txtSelectTeam
            // 
            this.txtSelectTeam.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.txtSelectTeam.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSelectTeam.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtSelectTeam.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.txtSelectTeam.Location = new System.Drawing.Point(112, 39);
            this.txtSelectTeam.Name = "txtSelectTeam";
            this.txtSelectTeam.ReadOnly = true;
            this.txtSelectTeam.Size = new System.Drawing.Size(637, 27);
            this.txtSelectTeam.TabIndex = 18;
            // 
            // btnSelectCommander
            // 
            this.btnSelectCommander.CheckButton = false;
            this.btnSelectCommander.CheckedBkgndImage = null;
            this.btnSelectCommander.CheckedImage = null;
            this.btnSelectCommander.ClickedBackgroundImage = null;
            this.btnSelectCommander.ClickedImage = global::SOPManager.Properties.Resources._PopupClick;
            this.btnSelectCommander.CustomImageRect = new System.Drawing.Rectangle(0, 0, 55, 35);
            this.btnSelectCommander.DisabledBkgndImage = null;
            this.btnSelectCommander.DisabledImage = null;
            this.btnSelectCommander.ID = -1;
            this.btnSelectCommander.InitButtonWidth = 55;
            this.btnSelectCommander.IsChecked = false;
            this.btnSelectCommander.Location = new System.Drawing.Point(576, 458);
            this.btnSelectCommander.MouseOverBkgndImage = null;
            this.btnSelectCommander.MouseOverImage = global::SOPManager.Properties.Resources._PopupClick;
            this.btnSelectCommander.Name = "btnSelectCommander";
            this.btnSelectCommander.NormalImage = global::SOPManager.Properties.Resources._Popup;
            this.btnSelectCommander.Owner = null;
            this.btnSelectCommander.Size = new System.Drawing.Size(55, 35);
            this.btnSelectCommander.TabIndex = 102;
            this.btnSelectCommander.TextLocation = new System.Drawing.Point(-1, 6);
            this.btnSelectCommander.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnSelectCommander.ToolTipText = "";
            this.btnSelectCommander.UseCustomImageRect = true;
            this.btnSelectCommander.UseTextLocation = true;
            this.btnSelectCommander.UseVisualStyleBackColor = true;
            this.btnSelectCommander.Visible = false;
            this.btnSelectCommander.Click += new System.EventHandler(this.btnSelectCommander_Click);
            this.btnSelectCommander.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SelectCommander_MouseDown);
            this.btnSelectCommander.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SelectCommander_MouseUp);
            // 
            // btnSelectTeam
            // 
            this.btnSelectTeam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSelectTeam.CheckButton = false;
            this.btnSelectTeam.CheckedBkgndImage = null;
            this.btnSelectTeam.CheckedImage = null;
            this.btnSelectTeam.ClickedBackgroundImage = null;
            this.btnSelectTeam.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_SelectClick;
            this.btnSelectTeam.CustomImageRect = new System.Drawing.Rectangle(0, 0, 64, 35);
            this.btnSelectTeam.DisabledBkgndImage = null;
            this.btnSelectTeam.DisabledImage = null;
            this.btnSelectTeam.ID = -1;
            this.btnSelectTeam.InitButtonWidth = 64;
            this.btnSelectTeam.IsChecked = false;
            this.btnSelectTeam.Location = new System.Drawing.Point(750, 35);
            this.btnSelectTeam.MouseOverBkgndImage = null;
            this.btnSelectTeam.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_SelectClick;
            this.btnSelectTeam.Name = "btnSelectTeam";
            this.btnSelectTeam.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Select;
            this.btnSelectTeam.Owner = null;
            this.btnSelectTeam.Size = new System.Drawing.Size(64, 35);
            this.btnSelectTeam.TabIndex = 101;
            this.btnSelectTeam.TextLocation = new System.Drawing.Point(0, 0);
            this.btnSelectTeam.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnSelectTeam.ToolTipText = "";
            this.btnSelectTeam.UseCustomImageRect = true;
            this.btnSelectTeam.UseTextLocation = true;
            this.btnSelectTeam.UseVisualStyleBackColor = true;
            this.btnSelectTeam.Click += new System.EventHandler(this.btnSelectTeam_Click);
            // 
            // picAutoRun
            // 
            this.picAutoRun.BackgroundImage = global::SOPManager.Properties.Resources.@__COMMON_ckb_enable;
            this.picAutoRun.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picAutoRun.Location = new System.Drawing.Point(16, 80);
            this.picAutoRun.Name = "picAutoRun";
            this.picAutoRun.Size = new System.Drawing.Size(20, 20);
            this.picAutoRun.TabIndex = 99;
            this.picAutoRun.TabStop = false;
            this.picAutoRun.Click += new System.EventHandler(this.AutoRun_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.CheckButton = false;
            this.btnCancel.CheckedBkgndImage = null;
            this.btnCancel.CheckedImage = null;
            this.btnCancel.ClickedBackgroundImage = null;
            this.btnCancel.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_CancelClick;
            this.btnCancel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 69, 37);
            this.btnCancel.DisabledBkgndImage = null;
            this.btnCancel.DisabledImage = null;
            this.btnCancel.ID = -1;
            this.btnCancel.InitButtonWidth = 69;
            this.btnCancel.IsChecked = false;
            this.btnCancel.Location = new System.Drawing.Point(746, 462);
            this.btnCancel.MouseOverBkgndImage = null;
            this.btnCancel.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_CancelClick;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Cancel;
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(69, 37);
            this.btnCancel.TabIndex = 44;
            this.btnCancel.TextLocation = new System.Drawing.Point(0, 0);
            this.btnCancel.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCancel.ToolTipText = "";
            this.btnCancel.UseCustomImageRect = true;
            this.btnCancel.UseTextLocation = false;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.CheckButton = false;
            this.btnOK.CheckedBkgndImage = null;
            this.btnOK.CheckedImage = null;
            this.btnOK.ClickedBackgroundImage = null;
            this.btnOK.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_OkClick;
            this.btnOK.CustomImageRect = new System.Drawing.Rectangle(0, 0, 69, 37);
            this.btnOK.DisabledBkgndImage = null;
            this.btnOK.DisabledImage = null;
            this.btnOK.ID = -1;
            this.btnOK.InitButtonWidth = 69;
            this.btnOK.IsChecked = false;
            this.btnOK.Location = new System.Drawing.Point(679, 462);
            this.btnOK.MouseOverBkgndImage = null;
            this.btnOK.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_OkClick;
            this.btnOK.Name = "btnOK";
            this.btnOK.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Ok;
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(69, 37);
            this.btnOK.TabIndex = 43;
            this.btnOK.TextLocation = new System.Drawing.Point(0, 0);
            this.btnOK.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOK.ToolTipText = "";
            this.btnOK.UseCustomImageRect = true;
            this.btnOK.UseTextLocation = false;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnShowSpecialMessage
            // 
            this.btnShowSpecialMessage.CheckButton = false;
            this.btnShowSpecialMessage.CheckedBkgndImage = null;
            this.btnShowSpecialMessage.CheckedImage = null;
            this.btnShowSpecialMessage.ClickedBackgroundImage = null;
            this.btnShowSpecialMessage.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_SpecialcharoptionClick;
            this.btnShowSpecialMessage.CustomImageRect = new System.Drawing.Rectangle(0, 0, 110, 37);
            this.btnShowSpecialMessage.DisabledBkgndImage = null;
            this.btnShowSpecialMessage.DisabledImage = null;
            this.btnShowSpecialMessage.ID = -1;
            this.btnShowSpecialMessage.InitButtonWidth = 110;
            this.btnShowSpecialMessage.IsChecked = false;
            this.btnShowSpecialMessage.Location = new System.Drawing.Point(87, 462);
            this.btnShowSpecialMessage.MouseOverBkgndImage = null;
            this.btnShowSpecialMessage.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_SpecialcharoptionClick;
            this.btnShowSpecialMessage.Name = "btnShowSpecialMessage";
            this.btnShowSpecialMessage.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Specialcharoption;
            this.btnShowSpecialMessage.Owner = null;
            this.btnShowSpecialMessage.Size = new System.Drawing.Size(110, 37);
            this.btnShowSpecialMessage.TabIndex = 42;
            this.btnShowSpecialMessage.TextLocation = new System.Drawing.Point(-3, 18);
            this.btnShowSpecialMessage.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnShowSpecialMessage.ToolTipText = "";
            this.btnShowSpecialMessage.UseCustomImageRect = true;
            this.btnShowSpecialMessage.UseTextLocation = true;
            this.btnShowSpecialMessage.UseVisualStyleBackColor = true;
            this.btnShowSpecialMessage.Click += new System.EventHandler(this.btnShowSpecialMessage_Click);
            // 
            // btnDown
            // 
            this.btnDown.BackColor = System.Drawing.Color.Transparent;
            this.btnDown.CheckButton = false;
            this.btnDown.CheckedBkgndImage = null;
            this.btnDown.CheckedImage = null;
            this.btnDown.ClickedBackgroundImage = null;
            this.btnDown.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_DownClick;
            this.btnDown.CustomImageRect = new System.Drawing.Rectangle(0, 0, 39, 37);
            this.btnDown.DisabledBkgndImage = null;
            this.btnDown.DisabledImage = null;
            this.btnDown.ID = -1;
            this.btnDown.InitButtonWidth = 39;
            this.btnDown.IsChecked = false;
            this.btnDown.Location = new System.Drawing.Point(50, 462);
            this.btnDown.MouseOverBkgndImage = null;
            this.btnDown.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_DownClick;
            this.btnDown.Name = "btnDown";
            this.btnDown.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Down;
            this.btnDown.Owner = null;
            this.btnDown.Size = new System.Drawing.Size(39, 37);
            this.btnDown.TabIndex = 27;
            this.btnDown.TextLocation = new System.Drawing.Point(0, 0);
            this.btnDown.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnDown.ToolTipText = "";
            this.btnDown.UseCustomImageRect = true;
            this.btnDown.UseTextLocation = false;
            this.btnDown.UseVisualStyleBackColor = false;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.BackColor = System.Drawing.Color.Transparent;
            this.btnUp.CheckButton = false;
            this.btnUp.CheckedBkgndImage = null;
            this.btnUp.CheckedImage = null;
            this.btnUp.ClickedBackgroundImage = null;
            this.btnUp.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_UpClick;
            this.btnUp.CustomImageRect = new System.Drawing.Rectangle(0, 0, 39, 37);
            this.btnUp.DisabledBkgndImage = null;
            this.btnUp.DisabledImage = null;
            this.btnUp.ID = -1;
            this.btnUp.InitButtonWidth = 39;
            this.btnUp.IsChecked = false;
            this.btnUp.Location = new System.Drawing.Point(12, 462);
            this.btnUp.MouseOverBkgndImage = null;
            this.btnUp.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_UpClick;
            this.btnUp.Name = "btnUp";
            this.btnUp.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Up;
            this.btnUp.Owner = null;
            this.btnUp.Size = new System.Drawing.Size(39, 37);
            this.btnUp.TabIndex = 26;
            this.btnUp.TextLocation = new System.Drawing.Point(0, 0);
            this.btnUp.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnUp.ToolTipText = "";
            this.btnUp.UseCustomImageRect = true;
            this.btnUp.UseTextLocation = false;
            this.btnUp.UseVisualStyleBackColor = false;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.SystemColors.Control;
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(16, 106);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(221, 26);
            this.label6.TabIndex = 23;
            this.label6.Text = "멘트 실행자";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(359, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(133, 16);
            this.label4.TabIndex = 19;
            this.label4.Text = "수신자 조치내용";
            this.label4.Visible = false;
            // 
            // lblAutoRun
            // 
            this.lblAutoRun.AutoSize = true;
            this.lblAutoRun.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblAutoRun.ForeColor = System.Drawing.Color.White;
            this.lblAutoRun.Location = new System.Drawing.Point(36, 81);
            this.lblAutoRun.Name = "lblAutoRun";
            this.lblAutoRun.Size = new System.Drawing.Size(86, 17);
            this.lblAutoRun.TabIndex = 100;
            this.lblAutoRun.Text = "자동 실행";
            this.lblAutoRun.Click += new System.EventHandler(this.AutoRun_Click);
            // 
            // checkBoxAutoRun
            // 
            this.checkBoxAutoRun.AutoSize = true;
            this.checkBoxAutoRun.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxAutoRun.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxAutoRun.ForeColor = System.Drawing.Color.White;
            this.checkBoxAutoRun.Location = new System.Drawing.Point(16, 81);
            this.checkBoxAutoRun.Name = "checkBoxAutoRun";
            this.checkBoxAutoRun.Size = new System.Drawing.Size(101, 20);
            this.checkBoxAutoRun.TabIndex = 25;
            this.checkBoxAutoRun.Text = "자동 실행";
            this.checkBoxAutoRun.UseVisualStyleBackColor = false;
            this.checkBoxAutoRun.Visible = false;
            // 
            // PopupMission
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.ClientSize = new System.Drawing.Size(829, 510);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 300);
            this.Name = "PopupMission";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "임무 내용 작성";
            this.Load += new System.EventHandler(this.PopupMission_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PopupMission_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PopupMission_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PopupMission_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAutoRun)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Label labelWarning;
        private System.Windows.Forms.Label labelNote;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSelectTeam;
        private System.Windows.Forms.TextBox textBoxCommander;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBoxAutoRun;
        private UnE.GUI.RibbonButton btnUp;
        private UnE.GUI.RibbonButton btnDown;
        private UnE.GUI.RibbonButton btnShowSpecialMessage;
        private UnE.GUI.RibbonButton btnCancel;
        private UnE.GUI.RibbonButton btnOK;
        private System.Windows.Forms.PictureBox picAutoRun;
        private System.Windows.Forms.Label lblAutoRun;
        private UnE.GUI.RibbonButton btnSelectTeam;
        private UnE.GUI.RibbonButton btnSelectCommander;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridViewComboBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewButtonColumn Column3;
    }
}