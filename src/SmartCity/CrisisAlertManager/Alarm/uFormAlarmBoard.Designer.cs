namespace CrisisAlertManager.Alarm
{
    partial class uFormAlarmBoard
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

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.plHeader = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.gridAlarmList = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCheck = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCheckBtn = new System.Windows.Forms.DataGridViewImageColumn();
            this.rbtnFire = new UnE.GUI.RibbonButton();
            this.rbtnFlood = new UnE.GUI.RibbonButton();
            this.rbtnHeat = new UnE.GUI.RibbonButton();
            this.rbtnCollapse = new UnE.GUI.RibbonButton();
            this.panel1.SuspendLayout();
            this.plHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridAlarmList)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.plHeader);
            this.panel1.Controls.Add(this.gridAlarmList);
            this.panel1.Location = new System.Drawing.Point(30, 90);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1377, 775);
            this.panel1.TabIndex = 12;
            // 
            // plHeader
            // 
            this.plHeader.Controls.Add(this.label2);
            this.plHeader.Controls.Add(this.label3);
            this.plHeader.Controls.Add(this.label4);
            this.plHeader.Controls.Add(this.label5);
            this.plHeader.Controls.Add(this.label6);
            this.plHeader.Controls.Add(this.pictureBox1);
            this.plHeader.Controls.Add(this.pictureBox2);
            this.plHeader.Location = new System.Drawing.Point(18, 36);
            this.plHeader.Name = "plHeader";
            this.plHeader.Size = new System.Drawing.Size(1340, 52);
            this.plHeader.TabIndex = 42;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(1029, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 15);
            this.label2.TabIndex = 47;
            this.label2.Text = "확인 여부";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(705, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 15);
            this.label3.TabIndex = 46;
            this.label3.Text = "감지 위치";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(406, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 15);
            this.label4.TabIndex = 45;
            this.label4.Text = "일시";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.Location = new System.Drawing.Point(147, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(109, 15);
            this.label5.TabIndex = 44;
            this.label5.Text = "위기경보 수준";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.Location = new System.Drawing.Point(34, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 15);
            this.label6.TabIndex = 43;
            this.label6.Text = "No.";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(145)))), ((int)(((byte)(164)))), ((int)(((byte)(191)))));
            this.pictureBox1.Location = new System.Drawing.Point(0, 50);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1340, 2);
            this.pictureBox1.TabIndex = 42;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(145)))), ((int)(((byte)(164)))), ((int)(((byte)(191)))));
            this.pictureBox2.Location = new System.Drawing.Point(0, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(1340, 2);
            this.pictureBox2.TabIndex = 41;
            this.pictureBox2.TabStop = false;
            // 
            // gridAlarmList
            // 
            this.gridAlarmList.AllowUserToAddRows = false;
            this.gridAlarmList.BackgroundColor = System.Drawing.Color.White;
            this.gridAlarmList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridAlarmList.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.gridAlarmList.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridAlarmList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridAlarmList.ColumnHeadersHeight = 40;
            this.gridAlarmList.ColumnHeadersVisible = false;
            this.gridAlarmList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colLevel,
            this.colTime,
            this.colAddress,
            this.colCheck,
            this.colCheckBtn});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridAlarmList.DefaultCellStyle = dataGridViewCellStyle3;
            this.gridAlarmList.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.gridAlarmList.Location = new System.Drawing.Point(18, 88);
            this.gridAlarmList.Name = "gridAlarmList";
            this.gridAlarmList.RowHeadersVisible = false;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.gridAlarmList.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.gridAlarmList.RowTemplate.Height = 50;
            this.gridAlarmList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridAlarmList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridAlarmList.Size = new System.Drawing.Size(1340, 590);
            this.gridAlarmList.TabIndex = 31;
            this.gridAlarmList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridAlarmList_CellClick);
            // 
            // colNo
            // 
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle2;
            this.colNo.HeaderText = "No";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colLevel
            // 
            this.colLevel.HeaderText = "위기경보 수준";
            this.colLevel.Name = "colLevel";
            this.colLevel.ReadOnly = true;
            this.colLevel.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colLevel.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colLevel.Width = 200;
            // 
            // colTime
            // 
            this.colTime.HeaderText = "일시";
            this.colTime.Name = "colTime";
            this.colTime.ReadOnly = true;
            this.colTime.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colTime.Width = 250;
            // 
            // colAddress
            // 
            this.colAddress.HeaderText = "감지 위치";
            this.colAddress.Name = "colAddress";
            this.colAddress.ReadOnly = true;
            this.colAddress.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colAddress.Width = 400;
            // 
            // colCheck
            // 
            this.colCheck.HeaderText = "확인 여부";
            this.colCheck.Name = "colCheck";
            this.colCheck.ReadOnly = true;
            this.colCheck.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colCheck.Width = 250;
            // 
            // colCheckBtn
            // 
            this.colCheckBtn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colCheckBtn.HeaderText = "확인";
            this.colCheckBtn.Image = global::CrisisAlertManager.Properties.Resources.btnAlarmCheck;
            this.colCheckBtn.Name = "colCheckBtn";
            // 
            // rbtnFire
            // 
            this.rbtnFire.CheckButton = false;
            this.rbtnFire.CheckedBkgndImage = null;
            this.rbtnFire.CheckedImage = global::CrisisAlertManager.Properties.Resources.ReportFireTab_Click;
            this.rbtnFire.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.ReportFireTab_Click;
            this.rbtnFire.ClickedBackgroundImage = null;
            this.rbtnFire.ClickedImage = global::CrisisAlertManager.Properties.Resources.ReportFireTab_Click;
            this.rbtnFire.CustomImageRect = new System.Drawing.Rectangle(0, 0, 144, 36);
            this.rbtnFire.DisabledBkgndImage = null;
            this.rbtnFire.DisabledImage = null;
            this.rbtnFire.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFire.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnFire.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnFire.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFire.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFire.ForeColorsByTypeUse = true;
            this.rbtnFire.ID = -1;
            this.rbtnFire.InitButtonWidth = 144;
            this.rbtnFire.IsChecked = false;
            this.rbtnFire.Location = new System.Drawing.Point(30, 53);
            this.rbtnFire.MouseOverBkgndImage = null;
            this.rbtnFire.MouseOverImage = global::CrisisAlertManager.Properties.Resources.ReportFireTab_Hover;
            this.rbtnFire.Name = "rbtnFire";
            this.rbtnFire.NormalImage = global::CrisisAlertManager.Properties.Resources.ReportFireTab_Normal;
            this.rbtnFire.Owner = null;
            this.rbtnFire.Size = new System.Drawing.Size(144, 36);
            this.rbtnFire.TabIndex = 8;
            this.rbtnFire.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnFire.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnFire.ToolTipText = "";
            this.rbtnFire.UseCustomImageRect = true;
            this.rbtnFire.UseTextLocation = true;
            this.rbtnFire.UseVisualStyleBackColor = true;
            this.rbtnFire.Click += new System.EventHandler(this.rbtnFire_Click);
            // 
            // rbtnFlood
            // 
            this.rbtnFlood.CheckButton = false;
            this.rbtnFlood.CheckedBkgndImage = null;
            this.rbtnFlood.CheckedImage = global::CrisisAlertManager.Properties.Resources.ReportFloodTab_Click;
            this.rbtnFlood.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.ReportFloodTab_Click;
            this.rbtnFlood.ClickedBackgroundImage = null;
            this.rbtnFlood.ClickedImage = global::CrisisAlertManager.Properties.Resources.ReportFloodTab_Click;
            this.rbtnFlood.CustomImageRect = new System.Drawing.Rectangle(0, 0, 144, 36);
            this.rbtnFlood.DisabledBkgndImage = null;
            this.rbtnFlood.DisabledImage = null;
            this.rbtnFlood.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFlood.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnFlood.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnFlood.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFlood.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnFlood.ForeColorsByTypeUse = true;
            this.rbtnFlood.ID = -1;
            this.rbtnFlood.InitButtonWidth = 144;
            this.rbtnFlood.IsChecked = false;
            this.rbtnFlood.Location = new System.Drawing.Point(180, 53);
            this.rbtnFlood.MouseOverBkgndImage = null;
            this.rbtnFlood.MouseOverImage = global::CrisisAlertManager.Properties.Resources.ReportFloodTab_Hover;
            this.rbtnFlood.Name = "rbtnFlood";
            this.rbtnFlood.NormalImage = global::CrisisAlertManager.Properties.Resources.ReportFloodTab_Normal;
            this.rbtnFlood.Owner = null;
            this.rbtnFlood.Size = new System.Drawing.Size(144, 36);
            this.rbtnFlood.TabIndex = 9;
            this.rbtnFlood.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnFlood.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnFlood.ToolTipText = "";
            this.rbtnFlood.UseCustomImageRect = true;
            this.rbtnFlood.UseTextLocation = true;
            this.rbtnFlood.UseVisualStyleBackColor = true;
            this.rbtnFlood.Click += new System.EventHandler(this.rbtnFlood_Click);
            // 
            // rbtnHeat
            // 
            this.rbtnHeat.CheckButton = false;
            this.rbtnHeat.CheckedBkgndImage = null;
            this.rbtnHeat.CheckedImage = global::CrisisAlertManager.Properties.Resources.ReportHeatTab_Click;
            this.rbtnHeat.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.ReportHeatTab_Click;
            this.rbtnHeat.ClickedBackgroundImage = null;
            this.rbtnHeat.ClickedImage = global::CrisisAlertManager.Properties.Resources.ReportHeatTab_Click;
            this.rbtnHeat.CustomImageRect = new System.Drawing.Rectangle(0, 0, 144, 36);
            this.rbtnHeat.DisabledBkgndImage = null;
            this.rbtnHeat.DisabledImage = null;
            this.rbtnHeat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnHeat.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnHeat.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnHeat.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnHeat.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnHeat.ForeColorsByTypeUse = true;
            this.rbtnHeat.ID = -1;
            this.rbtnHeat.InitButtonWidth = 144;
            this.rbtnHeat.IsChecked = false;
            this.rbtnHeat.Location = new System.Drawing.Point(330, 53);
            this.rbtnHeat.MouseOverBkgndImage = null;
            this.rbtnHeat.MouseOverImage = global::CrisisAlertManager.Properties.Resources.ReportHeatTab_Hover;
            this.rbtnHeat.Name = "rbtnHeat";
            this.rbtnHeat.NormalImage = global::CrisisAlertManager.Properties.Resources.ReportHeatTab_Normal;
            this.rbtnHeat.Owner = null;
            this.rbtnHeat.Size = new System.Drawing.Size(144, 36);
            this.rbtnHeat.TabIndex = 10;
            this.rbtnHeat.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnHeat.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnHeat.ToolTipText = "";
            this.rbtnHeat.UseCustomImageRect = true;
            this.rbtnHeat.UseTextLocation = true;
            this.rbtnHeat.UseVisualStyleBackColor = true;
            this.rbtnHeat.Click += new System.EventHandler(this.rbtnHeat_Click);
            // 
            // rbtnCollapse
            // 
            this.rbtnCollapse.CheckButton = false;
            this.rbtnCollapse.CheckedBkgndImage = null;
            this.rbtnCollapse.CheckedImage = global::CrisisAlertManager.Properties.Resources.ReportCollapseTab_Click;
            this.rbtnCollapse.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.ReportCollapseTab_Click;
            this.rbtnCollapse.ClickedBackgroundImage = null;
            this.rbtnCollapse.ClickedImage = global::CrisisAlertManager.Properties.Resources.ReportCollapseTab_Click;
            this.rbtnCollapse.CustomImageRect = new System.Drawing.Rectangle(0, 0, 144, 36);
            this.rbtnCollapse.DisabledBkgndImage = null;
            this.rbtnCollapse.DisabledImage = null;
            this.rbtnCollapse.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnCollapse.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnCollapse.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnCollapse.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnCollapse.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnCollapse.ForeColorsByTypeUse = true;
            this.rbtnCollapse.ID = -1;
            this.rbtnCollapse.InitButtonWidth = 144;
            this.rbtnCollapse.IsChecked = false;
            this.rbtnCollapse.Location = new System.Drawing.Point(480, 53);
            this.rbtnCollapse.MouseOverBkgndImage = null;
            this.rbtnCollapse.MouseOverImage = global::CrisisAlertManager.Properties.Resources.ReportCollapseTab_Hover;
            this.rbtnCollapse.Name = "rbtnCollapse";
            this.rbtnCollapse.NormalImage = global::CrisisAlertManager.Properties.Resources.ReportCollapseTab_Normal;
            this.rbtnCollapse.Owner = null;
            this.rbtnCollapse.Size = new System.Drawing.Size(144, 36);
            this.rbtnCollapse.TabIndex = 11;
            this.rbtnCollapse.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnCollapse.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnCollapse.ToolTipText = "";
            this.rbtnCollapse.UseCustomImageRect = true;
            this.rbtnCollapse.UseTextLocation = true;
            this.rbtnCollapse.UseVisualStyleBackColor = true;
            this.rbtnCollapse.Click += new System.EventHandler(this.rbtnCollapse_Click);
            // 
            // uFormAlarmBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.rbtnFire);
            this.Controls.Add(this.rbtnFlood);
            this.Controls.Add(this.rbtnHeat);
            this.Controls.Add(this.rbtnCollapse);
            this.Name = "uFormAlarmBoard";
            this.Size = new System.Drawing.Size(1600, 970);
            this.panel1.ResumeLayout(false);
            this.plHeader.ResumeLayout(false);
            this.plHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridAlarmList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private UnE.GUI.RibbonButton rbtnFire;
        private UnE.GUI.RibbonButton rbtnFlood;
        private UnE.GUI.RibbonButton rbtnHeat;
        private UnE.GUI.RibbonButton rbtnCollapse;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel plHeader;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.DataGridView gridAlarmList;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLevel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAddress;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCheck;
        private System.Windows.Forms.DataGridViewImageColumn colCheckBtn;
    }
}
