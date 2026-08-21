namespace CrisisAlertManager.Report
{
    partial class uFormReport
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.rbtnCollapse = new UnE.GUI.RibbonButton();
            this.rbtnHeat = new UnE.GUI.RibbonButton();
            this.rbtnFlood = new UnE.GUI.RibbonButton();
            this.rbtnFire = new UnE.GUI.RibbonButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbtnExport = new UnE.GUI.RibbonButton();
            this.rbtnSMS = new UnE.GUI.RibbonButton();
            this.rbtnAlert = new UnE.GUI.RibbonButton();
            this.rbtnData = new UnE.GUI.RibbonButton();
            this.plSMSReport = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.plReport = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.gridSMSReport = new System.Windows.Forms.DataGridView();
            this.colNoSMS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTypeSMS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTimeSMS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colManager = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridAlertReport = new System.Windows.Forms.DataGridView();
            this.colNoAlert = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTypeAlert = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTimeAlert = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDataNameAlert = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOriginDataAlert = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNewDataAlert = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridDataReport = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDataName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOriginData = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNewData = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            this.plSMSReport.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.plReport.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridSMSReport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridAlertReport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDataReport)).BeginInit();
            this.SuspendLayout();
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
            this.rbtnCollapse.TabIndex = 7;
            this.rbtnCollapse.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnCollapse.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnCollapse.ToolTipText = "";
            this.rbtnCollapse.UseCustomImageRect = true;
            this.rbtnCollapse.UseTextLocation = true;
            this.rbtnCollapse.UseVisualStyleBackColor = true;
            this.rbtnCollapse.Click += new System.EventHandler(this.rbtnCollapse_Click);
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
            this.rbtnHeat.TabIndex = 6;
            this.rbtnHeat.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnHeat.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnHeat.ToolTipText = "";
            this.rbtnHeat.UseCustomImageRect = true;
            this.rbtnHeat.UseTextLocation = true;
            this.rbtnHeat.UseVisualStyleBackColor = true;
            this.rbtnHeat.Click += new System.EventHandler(this.rbtnHeat_Click);
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
            this.rbtnFlood.TabIndex = 5;
            this.rbtnFlood.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnFlood.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnFlood.ToolTipText = "";
            this.rbtnFlood.UseCustomImageRect = true;
            this.rbtnFlood.UseTextLocation = true;
            this.rbtnFlood.UseVisualStyleBackColor = true;
            this.rbtnFlood.Click += new System.EventHandler(this.rbtnFlood_Click);
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
            this.rbtnFire.TabIndex = 2;
            this.rbtnFire.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnFire.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnFire.ToolTipText = "";
            this.rbtnFire.UseCustomImageRect = true;
            this.rbtnFire.UseTextLocation = true;
            this.rbtnFire.UseVisualStyleBackColor = true;
            this.rbtnFire.Click += new System.EventHandler(this.rbtnFire_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.rbtnExport);
            this.panel1.Controls.Add(this.rbtnSMS);
            this.panel1.Controls.Add(this.rbtnAlert);
            this.panel1.Controls.Add(this.rbtnData);
            this.panel1.Controls.Add(this.plSMSReport);
            this.panel1.Controls.Add(this.plReport);
            this.panel1.Controls.Add(this.gridSMSReport);
            this.panel1.Controls.Add(this.gridAlertReport);
            this.panel1.Controls.Add(this.gridDataReport);
            this.panel1.Location = new System.Drawing.Point(30, 90);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1377, 775);
            this.panel1.TabIndex = 8;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // rbtnExport
            // 
            this.rbtnExport.CheckButton = false;
            this.rbtnExport.CheckedBkgndImage = null;
            this.rbtnExport.CheckedImage = global::CrisisAlertManager.Properties.Resources.ReportExport_Click;
            this.rbtnExport.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.ReportExport_Click;
            this.rbtnExport.ClickedBackgroundImage = null;
            this.rbtnExport.ClickedImage = global::CrisisAlertManager.Properties.Resources.ReportExport_Click;
            this.rbtnExport.CustomImageRect = new System.Drawing.Rectangle(0, 0, 160, 40);
            this.rbtnExport.DisabledBkgndImage = null;
            this.rbtnExport.DisabledImage = null;
            this.rbtnExport.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnExport.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnExport.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnExport.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnExport.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnExport.ForeColorsByTypeUse = true;
            this.rbtnExport.ID = -1;
            this.rbtnExport.InitButtonWidth = 160;
            this.rbtnExport.IsChecked = false;
            this.rbtnExport.Location = new System.Drawing.Point(1198, 30);
            this.rbtnExport.MouseOverBkgndImage = null;
            this.rbtnExport.MouseOverImage = global::CrisisAlertManager.Properties.Resources.ReportExport_Hover;
            this.rbtnExport.Name = "rbtnExport";
            this.rbtnExport.NormalImage = global::CrisisAlertManager.Properties.Resources.ReportExport_Normal;
            this.rbtnExport.Owner = null;
            this.rbtnExport.Size = new System.Drawing.Size(160, 40);
            this.rbtnExport.TabIndex = 32;
            this.rbtnExport.TextLocation = new System.Drawing.Point(0, 10);
            this.rbtnExport.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnExport.ToolTipText = "";
            this.rbtnExport.UseCustomImageRect = true;
            this.rbtnExport.UseTextLocation = true;
            this.rbtnExport.UseVisualStyleBackColor = true;
            this.rbtnExport.Click += new System.EventHandler(this.rbtnExport_Click);
            // 
            // rbtnSMS
            // 
            this.rbtnSMS.CheckButton = false;
            this.rbtnSMS.CheckedBkgndImage = null;
            this.rbtnSMS.CheckedImage = global::CrisisAlertManager.Properties.Resources.ReportSMS_Click;
            this.rbtnSMS.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.ReportSMS_Click;
            this.rbtnSMS.ClickedBackgroundImage = null;
            this.rbtnSMS.ClickedImage = global::CrisisAlertManager.Properties.Resources.ReportSMS_Click;
            this.rbtnSMS.CustomImageRect = new System.Drawing.Rectangle(0, 0, 160, 40);
            this.rbtnSMS.DisabledBkgndImage = null;
            this.rbtnSMS.DisabledImage = null;
            this.rbtnSMS.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnSMS.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnSMS.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnSMS.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnSMS.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnSMS.ForeColorsByTypeUse = true;
            this.rbtnSMS.ID = -1;
            this.rbtnSMS.InitButtonWidth = 160;
            this.rbtnSMS.IsChecked = false;
            this.rbtnSMS.Location = new System.Drawing.Point(350, 30);
            this.rbtnSMS.MouseOverBkgndImage = null;
            this.rbtnSMS.MouseOverImage = global::CrisisAlertManager.Properties.Resources.ReportSMS_Hover;
            this.rbtnSMS.Name = "rbtnSMS";
            this.rbtnSMS.NormalImage = global::CrisisAlertManager.Properties.Resources.ReportSMS_Normal;
            this.rbtnSMS.Owner = null;
            this.rbtnSMS.Size = new System.Drawing.Size(160, 40);
            this.rbtnSMS.TabIndex = 11;
            this.rbtnSMS.TextLocation = new System.Drawing.Point(0, 10);
            this.rbtnSMS.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnSMS.ToolTipText = "";
            this.rbtnSMS.UseCustomImageRect = true;
            this.rbtnSMS.UseTextLocation = true;
            this.rbtnSMS.UseVisualStyleBackColor = true;
            this.rbtnSMS.Click += new System.EventHandler(this.rbtnSMS_Click);
            // 
            // rbtnAlert
            // 
            this.rbtnAlert.CheckButton = false;
            this.rbtnAlert.CheckedBkgndImage = null;
            this.rbtnAlert.CheckedImage = global::CrisisAlertManager.Properties.Resources.ReportAlert_Click;
            this.rbtnAlert.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.ReportAlert_Click;
            this.rbtnAlert.ClickedBackgroundImage = null;
            this.rbtnAlert.ClickedImage = global::CrisisAlertManager.Properties.Resources.ReportAlert_Click;
            this.rbtnAlert.CustomImageRect = new System.Drawing.Rectangle(0, 0, 160, 40);
            this.rbtnAlert.DisabledBkgndImage = null;
            this.rbtnAlert.DisabledImage = null;
            this.rbtnAlert.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnAlert.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnAlert.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnAlert.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnAlert.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnAlert.ForeColorsByTypeUse = true;
            this.rbtnAlert.ID = -1;
            this.rbtnAlert.InitButtonWidth = 160;
            this.rbtnAlert.IsChecked = false;
            this.rbtnAlert.Location = new System.Drawing.Point(184, 30);
            this.rbtnAlert.MouseOverBkgndImage = null;
            this.rbtnAlert.MouseOverImage = global::CrisisAlertManager.Properties.Resources.ReportAlert_Hover;
            this.rbtnAlert.Name = "rbtnAlert";
            this.rbtnAlert.NormalImage = global::CrisisAlertManager.Properties.Resources.ReportAlert_Normal;
            this.rbtnAlert.Owner = null;
            this.rbtnAlert.Size = new System.Drawing.Size(160, 40);
            this.rbtnAlert.TabIndex = 10;
            this.rbtnAlert.TextLocation = new System.Drawing.Point(0, 10);
            this.rbtnAlert.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnAlert.ToolTipText = "";
            this.rbtnAlert.UseCustomImageRect = true;
            this.rbtnAlert.UseTextLocation = true;
            this.rbtnAlert.UseVisualStyleBackColor = true;
            this.rbtnAlert.Click += new System.EventHandler(this.rbtnAlert_Click);
            // 
            // rbtnData
            // 
            this.rbtnData.CheckButton = false;
            this.rbtnData.CheckedBkgndImage = null;
            this.rbtnData.CheckedImage = global::CrisisAlertManager.Properties.Resources.ReportData_Click;
            this.rbtnData.CheckedMouseOver = global::CrisisAlertManager.Properties.Resources.ReportData_Click;
            this.rbtnData.ClickedBackgroundImage = null;
            this.rbtnData.ClickedImage = global::CrisisAlertManager.Properties.Resources.ReportData_Click;
            this.rbtnData.CustomImageRect = new System.Drawing.Rectangle(0, 0, 160, 40);
            this.rbtnData.DisabledBkgndImage = null;
            this.rbtnData.DisabledImage = null;
            this.rbtnData.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnData.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnData.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnData.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnData.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnData.ForeColorsByTypeUse = true;
            this.rbtnData.ID = -1;
            this.rbtnData.InitButtonWidth = 160;
            this.rbtnData.IsChecked = false;
            this.rbtnData.Location = new System.Drawing.Point(18, 30);
            this.rbtnData.MouseOverBkgndImage = null;
            this.rbtnData.MouseOverImage = global::CrisisAlertManager.Properties.Resources.ReportData_Hover;
            this.rbtnData.Name = "rbtnData";
            this.rbtnData.NormalImage = global::CrisisAlertManager.Properties.Resources.ReportData_Normal;
            this.rbtnData.Owner = null;
            this.rbtnData.Size = new System.Drawing.Size(160, 40);
            this.rbtnData.TabIndex = 9;
            this.rbtnData.TextLocation = new System.Drawing.Point(0, 10);
            this.rbtnData.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnData.ToolTipText = "";
            this.rbtnData.UseCustomImageRect = true;
            this.rbtnData.UseTextLocation = true;
            this.rbtnData.UseVisualStyleBackColor = true;
            this.rbtnData.Click += new System.EventHandler(this.rbtnData_Click);
            // 
            // plSMSReport
            // 
            this.plSMSReport.Controls.Add(this.label2);
            this.plSMSReport.Controls.Add(this.label3);
            this.plSMSReport.Controls.Add(this.label4);
            this.plSMSReport.Controls.Add(this.label5);
            this.plSMSReport.Controls.Add(this.label6);
            this.plSMSReport.Controls.Add(this.pictureBox1);
            this.plSMSReport.Controls.Add(this.pictureBox2);
            this.plSMSReport.Location = new System.Drawing.Point(18, 80);
            this.plSMSReport.Name = "plSMSReport";
            this.plSMSReport.Size = new System.Drawing.Size(1340, 52);
            this.plSMSReport.TabIndex = 42;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(1077, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 15);
            this.label2.TabIndex = 47;
            this.label2.Text = "담당자";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(600, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 15);
            this.label3.TabIndex = 46;
            this.label3.Text = "전송된 메시지";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(296, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 15);
            this.label4.TabIndex = 45;
            this.label4.Text = "일시";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.Location = new System.Drawing.Point(93, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 15);
            this.label5.TabIndex = 44;
            this.label5.Text = "유형";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.Location = new System.Drawing.Point(5, 18);
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
            // plReport
            // 
            this.plReport.Controls.Add(this.label7);
            this.plReport.Controls.Add(this.label8);
            this.plReport.Controls.Add(this.label9);
            this.plReport.Controls.Add(this.label10);
            this.plReport.Controls.Add(this.label11);
            this.plReport.Controls.Add(this.label12);
            this.plReport.Controls.Add(this.pictureBox3);
            this.plReport.Controls.Add(this.pictureBox4);
            this.plReport.Location = new System.Drawing.Point(18, 80);
            this.plReport.Name = "plReport";
            this.plReport.Size = new System.Drawing.Size(1340, 52);
            this.plReport.TabIndex = 41;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.Location = new System.Drawing.Point(1137, 18);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 15);
            this.label7.TabIndex = 48;
            this.label7.Text = "변동 데이터";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.Location = new System.Drawing.Point(835, 18);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(93, 15);
            this.label8.TabIndex = 47;
            this.label8.Text = "기존 데이터";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label9.Location = new System.Drawing.Point(528, 18);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(109, 15);
            this.label9.TabIndex = 46;
            this.label9.Text = "변동데이터 명";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label10.Location = new System.Drawing.Point(296, 18);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(39, 15);
            this.label10.TabIndex = 45;
            this.label10.Text = "일시";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label11.Location = new System.Drawing.Point(93, 18);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(39, 15);
            this.label11.TabIndex = 44;
            this.label11.Text = "유형";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label12.Location = new System.Drawing.Point(5, 18);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(33, 15);
            this.label12.TabIndex = 43;
            this.label12.Text = "No.";
            this.label12.Click += new System.EventHandler(this.label12_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(145)))), ((int)(((byte)(164)))), ((int)(((byte)(191)))));
            this.pictureBox3.Location = new System.Drawing.Point(0, 50);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(1340, 2);
            this.pictureBox3.TabIndex = 42;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(145)))), ((int)(((byte)(164)))), ((int)(((byte)(191)))));
            this.pictureBox4.Location = new System.Drawing.Point(0, 0);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(1340, 2);
            this.pictureBox4.TabIndex = 41;
            this.pictureBox4.TabStop = false;
            // 
            // gridSMSReport
            // 
            this.gridSMSReport.AllowUserToAddRows = false;
            this.gridSMSReport.BackgroundColor = System.Drawing.Color.White;
            this.gridSMSReport.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridSMSReport.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.gridSMSReport.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridSMSReport.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridSMSReport.ColumnHeadersHeight = 40;
            this.gridSMSReport.ColumnHeadersVisible = false;
            this.gridSMSReport.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNoSMS,
            this.colTypeSMS,
            this.colTimeSMS,
            this.colMessage,
            this.colManager});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridSMSReport.DefaultCellStyle = dataGridViewCellStyle3;
            this.gridSMSReport.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.gridSMSReport.Location = new System.Drawing.Point(18, 132);
            this.gridSMSReport.Name = "gridSMSReport";
            this.gridSMSReport.RowHeadersVisible = false;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.gridSMSReport.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.gridSMSReport.RowTemplate.Height = 50;
            this.gridSMSReport.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridSMSReport.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridSMSReport.Size = new System.Drawing.Size(1340, 590);
            this.gridSMSReport.TabIndex = 31;
            this.gridSMSReport.Visible = false;
            // 
            // colNoSMS
            // 
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            this.colNoSMS.DefaultCellStyle = dataGridViewCellStyle2;
            this.colNoSMS.Frozen = true;
            this.colNoSMS.HeaderText = "No";
            this.colNoSMS.Name = "colNoSMS";
            this.colNoSMS.ReadOnly = true;
            this.colNoSMS.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colNoSMS.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNoSMS.Width = 40;
            // 
            // colTypeSMS
            // 
            this.colTypeSMS.Frozen = true;
            this.colTypeSMS.HeaderText = "유형";
            this.colTypeSMS.Name = "colTypeSMS";
            this.colTypeSMS.ReadOnly = true;
            this.colTypeSMS.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colTypeSMS.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colTypeSMS.Width = 150;
            // 
            // colTimeSMS
            // 
            this.colTimeSMS.Frozen = true;
            this.colTimeSMS.HeaderText = "일시";
            this.colTimeSMS.Name = "colTimeSMS";
            this.colTimeSMS.ReadOnly = true;
            this.colTimeSMS.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colTimeSMS.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colTimeSMS.Width = 250;
            // 
            // colMessage
            // 
            this.colMessage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colMessage.HeaderText = "전송된 메시지";
            this.colMessage.Name = "colMessage";
            this.colMessage.ReadOnly = true;
            this.colMessage.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colMessage.Width = 450;
            // 
            // colManager
            // 
            this.colManager.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colManager.HeaderText = "담당자";
            this.colManager.Name = "colManager";
            this.colManager.ReadOnly = true;
            this.colManager.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // gridAlertReport
            // 
            this.gridAlertReport.AllowUserToAddRows = false;
            this.gridAlertReport.BackgroundColor = System.Drawing.Color.White;
            this.gridAlertReport.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridAlertReport.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.gridAlertReport.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridAlertReport.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.gridAlertReport.ColumnHeadersHeight = 40;
            this.gridAlertReport.ColumnHeadersVisible = false;
            this.gridAlertReport.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNoAlert,
            this.colTypeAlert,
            this.colTimeAlert,
            this.colDataNameAlert,
            this.colOriginDataAlert,
            this.colNewDataAlert});
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridAlertReport.DefaultCellStyle = dataGridViewCellStyle7;
            this.gridAlertReport.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.gridAlertReport.Location = new System.Drawing.Point(18, 132);
            this.gridAlertReport.Name = "gridAlertReport";
            this.gridAlertReport.RowHeadersVisible = false;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.gridAlertReport.RowsDefaultCellStyle = dataGridViewCellStyle8;
            this.gridAlertReport.RowTemplate.Height = 50;
            this.gridAlertReport.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridAlertReport.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridAlertReport.Size = new System.Drawing.Size(1340, 590);
            this.gridAlertReport.TabIndex = 30;
            this.gridAlertReport.Visible = false;
            // 
            // colNoAlert
            // 
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            this.colNoAlert.DefaultCellStyle = dataGridViewCellStyle6;
            this.colNoAlert.HeaderText = "No";
            this.colNoAlert.Name = "colNoAlert";
            this.colNoAlert.ReadOnly = true;
            this.colNoAlert.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colNoAlert.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNoAlert.Width = 40;
            // 
            // colTypeAlert
            // 
            this.colTypeAlert.HeaderText = "유형";
            this.colTypeAlert.Name = "colTypeAlert";
            this.colTypeAlert.ReadOnly = true;
            this.colTypeAlert.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colTypeAlert.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colTypeAlert.Width = 150;
            // 
            // colTimeAlert
            // 
            this.colTimeAlert.HeaderText = "일시";
            this.colTimeAlert.Name = "colTimeAlert";
            this.colTimeAlert.ReadOnly = true;
            this.colTimeAlert.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colTimeAlert.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colTimeAlert.Width = 250;
            // 
            // colDataNameAlert
            // 
            this.colDataNameAlert.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDataNameAlert.HeaderText = "변동데이터 명";
            this.colDataNameAlert.Name = "colDataNameAlert";
            this.colDataNameAlert.ReadOnly = true;
            this.colDataNameAlert.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // colOriginDataAlert
            // 
            this.colOriginDataAlert.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colOriginDataAlert.HeaderText = "기존데이터";
            this.colOriginDataAlert.Name = "colOriginDataAlert";
            this.colOriginDataAlert.ReadOnly = true;
            this.colOriginDataAlert.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // colNewDataAlert
            // 
            this.colNewDataAlert.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colNewDataAlert.HeaderText = "변동데이터";
            this.colNewDataAlert.Name = "colNewDataAlert";
            this.colNewDataAlert.ReadOnly = true;
            this.colNewDataAlert.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // gridDataReport
            // 
            this.gridDataReport.AllowUserToAddRows = false;
            this.gridDataReport.BackgroundColor = System.Drawing.Color.White;
            this.gridDataReport.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridDataReport.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.gridDataReport.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridDataReport.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.gridDataReport.ColumnHeadersHeight = 40;
            this.gridDataReport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridDataReport.ColumnHeadersVisible = false;
            this.gridDataReport.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colType,
            this.colTime,
            this.colDataName,
            this.colOriginData,
            this.colNewData});
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridDataReport.DefaultCellStyle = dataGridViewCellStyle11;
            this.gridDataReport.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.gridDataReport.Location = new System.Drawing.Point(18, 132);
            this.gridDataReport.Name = "gridDataReport";
            this.gridDataReport.RowHeadersVisible = false;
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.gridDataReport.RowsDefaultCellStyle = dataGridViewCellStyle12;
            this.gridDataReport.RowTemplate.Height = 50;
            this.gridDataReport.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridDataReport.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridDataReport.Size = new System.Drawing.Size(1340, 590);
            this.gridDataReport.TabIndex = 29;
            this.gridDataReport.Visible = false;
            // 
            // colNo
            // 
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.White;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle10;
            this.colNo.HeaderText = "No";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNo.Width = 40;
            // 
            // colType
            // 
            this.colType.HeaderText = "유형";
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            this.colType.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colType.Width = 150;
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
            // colDataName
            // 
            this.colDataName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDataName.HeaderText = "변동데이터 명";
            this.colDataName.Name = "colDataName";
            this.colDataName.ReadOnly = true;
            this.colDataName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // colOriginData
            // 
            this.colOriginData.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colOriginData.HeaderText = "기존데이터";
            this.colOriginData.Name = "colOriginData";
            this.colOriginData.ReadOnly = true;
            this.colOriginData.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // colNewData
            // 
            this.colNewData.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colNewData.HeaderText = "변동데이터";
            this.colNewData.Name = "colNewData";
            this.colNewData.ReadOnly = true;
            this.colNewData.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // uFormReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.rbtnFire);
            this.Controls.Add(this.rbtnFlood);
            this.Controls.Add(this.rbtnHeat);
            this.Controls.Add(this.rbtnCollapse);
            this.Name = "uFormReport";
            this.Size = new System.Drawing.Size(1600, 970);
            this.panel1.ResumeLayout(false);
            this.plSMSReport.ResumeLayout(false);
            this.plSMSReport.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.plReport.ResumeLayout(false);
            this.plReport.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridSMSReport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridAlertReport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDataReport)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private UnE.GUI.RibbonButton rbtnCollapse;
        private UnE.GUI.RibbonButton rbtnHeat;
        private UnE.GUI.RibbonButton rbtnFlood;
        private UnE.GUI.RibbonButton rbtnFire;
        private System.Windows.Forms.Panel panel1;
        private UnE.GUI.RibbonButton rbtnData;
        private UnE.GUI.RibbonButton rbtnSMS;
        private UnE.GUI.RibbonButton rbtnAlert;
        private System.Windows.Forms.DataGridView gridAlertReport;
        private System.Windows.Forms.DataGridView gridDataReport;
        private System.Windows.Forms.DataGridView gridSMSReport;
        private UnE.GUI.RibbonButton rbtnExport;
        private System.Windows.Forms.Panel plReport;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Panel plSMSReport;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNoAlert;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTypeAlert;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTimeAlert;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDataNameAlert;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOriginDataAlert;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNewDataAlert;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDataName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOriginData;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNewData;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNoSMS;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTypeSMS;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTimeSMS;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMessage;
        private System.Windows.Forms.DataGridViewTextBoxColumn colManager;
    }
}
