namespace BIMViewer
{
    partial class FormLocation
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
            this.cmbSido = new System.Windows.Forms.ComboBox();
            this.cmbEmd = new System.Windows.Forms.ComboBox();
            this.cmbSgg = new System.Windows.Forms.ComboBox();
            this.rbtnOK = new UnE.GUI.RibbonButton();
            this.txtRoadName = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnClose = new UnE.GUI.ImageButton();
            this.lblSido = new System.Windows.Forms.Label();
            this.lblSgg = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblBuildName = new System.Windows.Forms.Label();
            this.lblBuildMenu = new System.Windows.Forms.Label();
            this.lblFloors = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.rbtnSearch = new UnE.GUI.RibbonButton();
            this.lblMain = new System.Windows.Forms.Label();
            this.txtMainNumber = new System.Windows.Forms.TextBox();
            this.chkEmd = new System.Windows.Forms.CheckBox();
            this.grdAdress = new BIMViewer.CustomControls.CustomGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdAdress)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbSido
            // 
            this.cmbSido.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.cmbSido.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSido.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSido.FormattingEnabled = true;
            this.cmbSido.Location = new System.Drawing.Point(64, 35);
            this.cmbSido.Name = "cmbSido";
            this.cmbSido.Size = new System.Drawing.Size(139, 20);
            this.cmbSido.TabIndex = 3;
            this.cmbSido.SelectedIndexChanged += new System.EventHandler(this.cmbSidoChanged);
            // 
            // cmbEmd
            // 
            this.cmbEmd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.cmbEmd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEmd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbEmd.FormattingEnabled = true;
            this.cmbEmd.Location = new System.Drawing.Point(559, 35);
            this.cmbEmd.Name = "cmbEmd";
            this.cmbEmd.Size = new System.Drawing.Size(140, 20);
            this.cmbEmd.TabIndex = 4;
            // 
            // cmbSgg
            // 
            this.cmbSgg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.cmbSgg.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSgg.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSgg.FormattingEnabled = true;
            this.cmbSgg.Location = new System.Drawing.Point(286, 35);
            this.cmbSgg.Name = "cmbSgg";
            this.cmbSgg.Size = new System.Drawing.Size(114, 20);
            this.cmbSgg.TabIndex = 5;
            this.cmbSgg.SelectedIndexChanged += new System.EventHandler(this.cmbSggChanged);
            // 
            // rbtnOK
            // 
            this.rbtnOK.BackColor = System.Drawing.Color.Transparent;
            this.rbtnOK.BackgroundImage = global::BIMViewer.Properties.Resources.green_gradation_button_01;
            this.rbtnOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rbtnOK.CheckButton = false;
            this.rbtnOK.CheckedBkgndImage = null;
            this.rbtnOK.CheckedImage = null;
            this.rbtnOK.CheckedMouseOver = null;
            this.rbtnOK.ClickedBackgroundImage = null;
            this.rbtnOK.ClickedImage = null;
            this.rbtnOK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbtnOK.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.rbtnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.rbtnOK.DisabledBkgndImage = null;
            this.rbtnOK.DisabledImage = null;
            this.rbtnOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.rbtnOK.ForeColor = System.Drawing.Color.White;
            this.rbtnOK.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnOK.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnOK.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnOK.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnOK.ForeColorsByTypeUse = false;
            this.rbtnOK.ID = -1;
            this.rbtnOK.InitButtonWidth = 60;
            this.rbtnOK.IsChecked = false;
            this.rbtnOK.Location = new System.Drawing.Point(771, 405);
            this.rbtnOK.MouseOverBkgndImage = null;
            this.rbtnOK.MouseOverImage = null;
            this.rbtnOK.Name = "rbtnOK";
            this.rbtnOK.NormalImage = null;
            this.rbtnOK.Owner = null;
            this.rbtnOK.Size = new System.Drawing.Size(60, 23);
            this.rbtnOK.TabIndex = 6;
            this.rbtnOK.Text = "확인";
            this.rbtnOK.TextLocation = new System.Drawing.Point(1, 3);
            this.rbtnOK.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnOK.ToolTipText = "확인";
            this.rbtnOK.UseCustomImageRect = false;
            this.rbtnOK.UseTextLocation = true;
            this.rbtnOK.UseVisualStyleBackColor = false;
            this.rbtnOK.Click += new System.EventHandler(this.RbtnOK_Click);
            // 
            // txtRoadName
            // 
            this.txtRoadName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(36)))), ((int)(((byte)(39)))));
            this.txtRoadName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRoadName.ForeColor = System.Drawing.Color.White;
            this.txtRoadName.ImeMode = System.Windows.Forms.ImeMode.Hangul;
            this.txtRoadName.Location = new System.Drawing.Point(64, 68);
            this.txtRoadName.Name = "txtRoadName";
            this.txtRoadName.Size = new System.Drawing.Size(139, 14);
            this.txtRoadName.TabIndex = 9;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::BIMViewer.Properties.Resources.green_gradation_01;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.lblTitle);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(845, 25);
            this.panel1.TabIndex = 14;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Panel1_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Panel1_MouseMove);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(10, 8);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(29, 12);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "위치";
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ButtonText = "";
            this.btnClose.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.Image = global::BIMViewer.Properties.Resources.Windowclose__Base;
            this.btnClose.ImageClicked = global::BIMViewer.Properties.Resources.Windowclose_1st_MSover;
            this.btnClose.ImageDisabled = null;
            this.btnClose.ImageMouseOver = global::BIMViewer.Properties.Resources.Windowclose_1st_MSover;
            this.btnClose.ImageNormal = global::BIMViewer.Properties.Resources.Windowclose__Base;
            this.btnClose.Location = new System.Drawing.Point(808, 1);
            this.btnClose.Name = "btnClose";
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(23, 23);
            this.btnClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnClose.TabIndex = 0;
            this.btnClose.TabStop = false;
            this.btnClose.TextColor = System.Drawing.Color.Black;
            this.btnClose.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ToolTipText = "";
            this.btnClose.UseToolTip = false;
            this.btnClose.WindowRateWidth = 1F;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // lblSido
            // 
            this.lblSido.AutoSize = true;
            this.lblSido.BackColor = System.Drawing.Color.Transparent;
            this.lblSido.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSido.ForeColor = System.Drawing.Color.White;
            this.lblSido.Location = new System.Drawing.Point(18, 39);
            this.lblSido.Name = "lblSido";
            this.lblSido.Size = new System.Drawing.Size(40, 15);
            this.lblSido.TabIndex = 15;
            this.lblSido.Text = "시 도 :";
            this.lblSido.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSgg
            // 
            this.lblSgg.AutoSize = true;
            this.lblSgg.BackColor = System.Drawing.Color.Transparent;
            this.lblSgg.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSgg.ForeColor = System.Drawing.Color.White;
            this.lblSgg.Location = new System.Drawing.Point(231, 39);
            this.lblSgg.Name = "lblSgg";
            this.lblSgg.Size = new System.Drawing.Size(49, 15);
            this.lblSgg.TabIndex = 16;
            this.lblSgg.Text = "시군구 :";
            this.lblSgg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(10, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 15);
            this.label1.TabIndex = 19;
            this.label1.Text = "도로명 :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBuildName
            // 
            this.lblBuildName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.lblBuildName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBuildName.ForeColor = System.Drawing.Color.White;
            this.lblBuildName.Location = new System.Drawing.Point(12, 96);
            this.lblBuildName.Name = "lblBuildName";
            this.lblBuildName.Size = new System.Drawing.Size(260, 23);
            this.lblBuildName.TabIndex = 22;
            this.lblBuildName.Text = "건물목록";
            this.lblBuildName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBuildMenu
            // 
            this.lblBuildMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.lblBuildMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBuildMenu.ForeColor = System.Drawing.Color.White;
            this.lblBuildMenu.Location = new System.Drawing.Point(272, 96);
            this.lblBuildMenu.Name = "lblBuildMenu";
            this.lblBuildMenu.Size = new System.Drawing.Size(120, 23);
            this.lblBuildMenu.TabIndex = 23;
            this.lblBuildMenu.Text = "건물용도";
            this.lblBuildMenu.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblFloors
            // 
            this.lblFloors.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.lblFloors.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFloors.ForeColor = System.Drawing.Color.White;
            this.lblFloors.Location = new System.Drawing.Point(532, 96);
            this.lblFloors.Name = "lblFloors";
            this.lblFloors.Size = new System.Drawing.Size(50, 23);
            this.lblFloors.TabIndex = 24;
            this.lblFloors.Text = "층수";
            this.lblFloors.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDate
            // 
            this.lblDate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.lblDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDate.ForeColor = System.Drawing.Color.White;
            this.lblDate.Location = new System.Drawing.Point(582, 96);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(250, 23);
            this.lblDate.TabIndex = 25;
            this.lblDate.Text = "작업일자";
            this.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rbtnSearch
            // 
            this.rbtnSearch.BackColor = System.Drawing.Color.Transparent;
            this.rbtnSearch.BackgroundImage = global::BIMViewer.Properties.Resources.green_gradation_button_01;
            this.rbtnSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rbtnSearch.CheckButton = false;
            this.rbtnSearch.CheckedBkgndImage = null;
            this.rbtnSearch.CheckedImage = null;
            this.rbtnSearch.CheckedMouseOver = null;
            this.rbtnSearch.ClickedBackgroundImage = null;
            this.rbtnSearch.ClickedImage = null;
            this.rbtnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbtnSearch.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.rbtnSearch.DisabledBkgndImage = null;
            this.rbtnSearch.DisabledImage = null;
            this.rbtnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.rbtnSearch.ForeColor = System.Drawing.Color.White;
            this.rbtnSearch.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnSearch.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnSearch.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnSearch.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnSearch.ForeColorsByTypeUse = false;
            this.rbtnSearch.ID = -1;
            this.rbtnSearch.InitButtonWidth = 60;
            this.rbtnSearch.IsChecked = false;
            this.rbtnSearch.Location = new System.Drawing.Point(771, 34);
            this.rbtnSearch.MouseOverBkgndImage = null;
            this.rbtnSearch.MouseOverImage = null;
            this.rbtnSearch.Name = "rbtnSearch";
            this.rbtnSearch.NormalImage = null;
            this.rbtnSearch.Owner = null;
            this.rbtnSearch.Size = new System.Drawing.Size(60, 23);
            this.rbtnSearch.TabIndex = 29;
            this.rbtnSearch.Text = "찾기";
            this.rbtnSearch.TextLocation = new System.Drawing.Point(0, 3);
            this.rbtnSearch.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnSearch.ToolTipText = "찾기";
            this.rbtnSearch.UseCustomImageRect = false;
            this.rbtnSearch.UseTextLocation = true;
            this.rbtnSearch.UseVisualStyleBackColor = false;
            this.rbtnSearch.Click += new System.EventHandler(this.RbtnSearch_Click);
            // 
            // lblMain
            // 
            this.lblMain.AutoSize = true;
            this.lblMain.BackColor = System.Drawing.Color.Transparent;
            this.lblMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMain.ForeColor = System.Drawing.Color.White;
            this.lblMain.Location = new System.Drawing.Point(219, 69);
            this.lblMain.Name = "lblMain";
            this.lblMain.Size = new System.Drawing.Size(61, 15);
            this.lblMain.TabIndex = 30;
            this.lblMain.Text = "건물번호 :";
            this.lblMain.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtMainNumber
            // 
            this.txtMainNumber.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(36)))), ((int)(((byte)(39)))));
            this.txtMainNumber.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMainNumber.ForeColor = System.Drawing.Color.White;
            this.txtMainNumber.ImeMode = System.Windows.Forms.ImeMode.Hangul;
            this.txtMainNumber.Location = new System.Drawing.Point(286, 68);
            this.txtMainNumber.Name = "txtMainNumber";
            this.txtMainNumber.Size = new System.Drawing.Size(117, 14);
            this.txtMainNumber.TabIndex = 32;
            // 
            // chkEmd
            // 
            this.chkEmd.AutoSize = true;
            this.chkEmd.BackColor = System.Drawing.Color.Transparent;
            this.chkEmd.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEmd.ForeColor = System.Drawing.Color.White;
            this.chkEmd.Location = new System.Drawing.Point(484, 38);
            this.chkEmd.Name = "chkEmd";
            this.chkEmd.Size = new System.Drawing.Size(68, 19);
            this.chkEmd.TabIndex = 34;
            this.chkEmd.Text = "읍면동 :";
            this.chkEmd.UseVisualStyleBackColor = false;
            this.chkEmd.CheckedChanged += new System.EventHandler(this.ChkEmd_CheckedChanged);
            // 
            // grdAdress
            // 
            this.grdAdress.AllowUserToAddRows = false;
            this.grdAdress.AllowUserToDeleteRows = false;
            this.grdAdress.AllowUserToResizeColumns = false;
            this.grdAdress.AllowUserToResizeRows = false;
            this.grdAdress.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(36)))), ((int)(((byte)(39)))));
            this.grdAdress.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grdAdress.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.grdAdress.ColumnHeadersVisible = false;
            this.grdAdress.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column5,
            this.Column3,
            this.Column4});
            this.grdAdress.Location = new System.Drawing.Point(12, 120);
            this.grdAdress.MultiSelect = false;
            this.grdAdress.Name = "grdAdress";
            this.grdAdress.ReadOnly = true;
            this.grdAdress.RowHeadersVisible = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(36)))), ((int)(((byte)(39)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(235)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.grdAdress.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.grdAdress.RowTemplate.Height = 23;
            this.grdAdress.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.grdAdress.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdAdress.Size = new System.Drawing.Size(819, 280);
            this.grdAdress.TabIndex = 28;
            this.grdAdress.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.GrdAdress_CellMouseClick);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Column1";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column1.Width = 260;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Column2";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column2.Width = 120;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "Column5";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.Width = 140;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Column3";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column3.Width = 50;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Column4";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column4.Width = 250;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(392, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 23);
            this.label2.TabIndex = 40;
            this.label2.Text = "건물명";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormLocation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::BIMViewer.Properties.Resources.background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(843, 440);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chkEmd);
            this.Controls.Add(this.txtMainNumber);
            this.Controls.Add(this.lblMain);
            this.Controls.Add(this.rbtnSearch);
            this.Controls.Add(this.grdAdress);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.lblFloors);
            this.Controls.Add(this.lblBuildMenu);
            this.Controls.Add(this.lblBuildName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblSgg);
            this.Controls.Add(this.lblSido);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtRoadName);
            this.Controls.Add(this.rbtnOK);
            this.Controls.Add(this.cmbSgg);
            this.Controls.Add(this.cmbEmd);
            this.Controls.Add(this.cmbSido);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLocation";
            this.Text = "주소찾기";
            this.Load += new System.EventHandler(this.FormUploadLocationLoad);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormLocation_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormLocation_MouseMove);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdAdress)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cmbSido;
        private System.Windows.Forms.ComboBox cmbEmd;
        private System.Windows.Forms.ComboBox cmbSgg;
        private UnE.GUI.RibbonButton rbtnOK;
        private System.Windows.Forms.TextBox txtRoadName;
        private System.Windows.Forms.Panel panel1;
        private UnE.GUI.ImageButton btnClose;
        private System.Windows.Forms.Label lblSido;
        private System.Windows.Forms.Label lblSgg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblBuildName;
        private System.Windows.Forms.Label lblBuildMenu;
        private System.Windows.Forms.Label lblFloors;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblTitle;
        private CustomControls.CustomGridView grdAdress;
        private UnE.GUI.RibbonButton rbtnSearch;
        private System.Windows.Forms.Label lblMain;
        private System.Windows.Forms.TextBox txtMainNumber;
        private System.Windows.Forms.CheckBox chkEmd;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.Label label2;
    }
}