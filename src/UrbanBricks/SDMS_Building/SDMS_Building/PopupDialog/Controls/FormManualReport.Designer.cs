namespace SDMS_Building.PopupDialog.Controls
{
    partial class FormManualReport
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnClose = new UnE.GUI.ImageButton();
            this.eleFloor = new System.Windows.Forms.Integration.ElementHost();
            this.eleBuilding = new System.Windows.Forms.Integration.ElementHost();
            this.lblFloor = new System.Windows.Forms.Label();
            this.lblBuilding = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtName = new System.Windows.Forms.TextBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.txtTeam = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lblLevel1 = new System.Windows.Forms.Label();
            this.lblLevel2 = new System.Windows.Forms.Label();
            this.lblLevel3 = new System.Windows.Forms.Label();
            this.lblLevel4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.pnLevel = new System.Windows.Forms.Panel();
            this.pnMemo = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.txtMemo = new System.Windows.Forms.TextBox();
            this.rbtnCorona = new UnE.GUI.RibbonButton();
            this.rbtnBlackout = new UnE.GUI.RibbonButton();
            this.rbtnSubmergency = new UnE.GUI.RibbonButton();
            this.rbtnStrongwind = new UnE.GUI.RibbonButton();
            this.rbtnTerror = new UnE.GUI.RibbonButton();
            this.rbtnPSM = new UnE.GUI.RibbonButton();
            this.rbtnEarthquake = new UnE.GUI.RibbonButton();
            this.rbtnFire = new UnE.GUI.RibbonButton();
            this.btnConfirm = new UnE.GUI.ImageButton();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.pnLevel.SuspendLayout();
            this.pnMemo.SuspendLayout();
            this.panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnConfirm)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1000, 80);
            this.panel1.TabIndex = 23;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("나눔바른고딕", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(57, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 27);
            this.label1.TabIndex = 17;
            this.label1.Text = "재난 수동 신고";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.panel3.Location = new System.Drawing.Point(30, 38);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(7, 7);
            this.panel3.TabIndex = 16;
            // 
            // btnClose
            // 
            this.btnClose.ButtonText = "";
            this.btnClose.ImageClicked = global::SDMS_Building.Properties.Resources.close_Click;
            this.btnClose.ImageDisabled = null;
            this.btnClose.ImageMouseOver = global::SDMS_Building.Properties.Resources.close_Hover;
            this.btnClose.ImageNormal = global::SDMS_Building.Properties.Resources.close_Normal;
            this.btnClose.Location = new System.Drawing.Point(939, 26);
            this.btnClose.Name = "btnClose";
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(30, 30);
            this.btnClose.TabIndex = 15;
            this.btnClose.TabStop = false;
            this.btnClose.TextColor = System.Drawing.Color.Black;
            this.btnClose.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ToolTipText = "";
            this.btnClose.UseToolTip = false;
            this.btnClose.WindowRateWidth = 1F;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // eleFloor
            // 
            this.eleFloor.Location = new System.Drawing.Point(748, 250);
            this.eleFloor.Name = "eleFloor";
            this.eleFloor.Size = new System.Drawing.Size(176, 50);
            this.eleFloor.TabIndex = 27;
            this.eleFloor.Text = "elementHost2";
            this.eleFloor.Child = null;
            // 
            // eleBuilding
            // 
            this.eleBuilding.Location = new System.Drawing.Point(282, 250);
            this.eleBuilding.Name = "eleBuilding";
            this.eleBuilding.Size = new System.Drawing.Size(385, 50);
            this.eleBuilding.TabIndex = 26;
            this.eleBuilding.Text = "elementHost1";
            this.eleBuilding.Child = null;
            // 
            // lblFloor
            // 
            this.lblFloor.AutoSize = true;
            this.lblFloor.BackColor = System.Drawing.Color.White;
            this.lblFloor.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblFloor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lblFloor.Location = new System.Drawing.Point(694, 264);
            this.lblFloor.Name = "lblFloor";
            this.lblFloor.Size = new System.Drawing.Size(28, 23);
            this.lblFloor.TabIndex = 28;
            this.lblFloor.Text = "층";
            // 
            // lblBuilding
            // 
            this.lblBuilding.AutoSize = true;
            this.lblBuilding.BackColor = System.Drawing.Color.White;
            this.lblBuilding.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblBuilding.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lblBuilding.Location = new System.Drawing.Point(187, 264);
            this.lblBuilding.Name = "lblBuilding";
            this.lblBuilding.Size = new System.Drawing.Size(46, 23);
            this.lblBuilding.TabIndex = 29;
            this.lblBuilding.Text = "건물";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.White;
            this.label4.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.label4.Location = new System.Drawing.Point(184, 536);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 23);
            this.label4.TabIndex = 38;
            this.label4.Text = "소속";
            this.label4.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.White;
            this.label5.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.label5.Location = new System.Drawing.Point(184, 611);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 23);
            this.label5.TabIndex = 37;
            this.label5.Text = "이름";
            this.label5.Visible = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.txtName);
            this.panel2.Location = new System.Drawing.Point(282, 598);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(420, 50);
            this.panel2.TabIndex = 39;
            this.panel2.Visible = false;
            // 
            // txtName
            // 
            this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtName.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtName.Location = new System.Drawing.Point(12, 11);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(393, 23);
            this.txtName.TabIndex = 0;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.White;
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Controls.Add(this.txtTeam);
            this.panel5.Location = new System.Drawing.Point(282, 523);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(420, 50);
            this.panel5.TabIndex = 41;
            this.panel5.Visible = false;
            // 
            // txtTeam
            // 
            this.txtTeam.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTeam.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtTeam.Location = new System.Drawing.Point(12, 11);
            this.txtTeam.Name = "txtTeam";
            this.txtTeam.Size = new System.Drawing.Size(393, 23);
            this.txtTeam.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.label6.Location = new System.Drawing.Point(11, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 23);
            this.label6.TabIndex = 42;
            this.label6.Text = "위기단계";
            // 
            // lblLevel1
            // 
            this.lblLevel1.BackColor = System.Drawing.Color.White;
            this.lblLevel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLevel1.Font = new System.Drawing.Font("나눔바른고딕", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblLevel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(155)))), ((int)(((byte)(155)))));
            this.lblLevel1.Location = new System.Drawing.Point(110, 5);
            this.lblLevel1.Name = "lblLevel1";
            this.lblLevel1.Size = new System.Drawing.Size(80, 45);
            this.lblLevel1.TabIndex = 43;
            this.lblLevel1.Tag = "1";
            this.lblLevel1.Text = "관심";
            this.lblLevel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLevel1.Click += new System.EventHandler(this.lblLevel_Click);
            // 
            // lblLevel2
            // 
            this.lblLevel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.lblLevel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLevel2.Font = new System.Drawing.Font("나눔바른고딕", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblLevel2.ForeColor = System.Drawing.Color.White;
            this.lblLevel2.Location = new System.Drawing.Point(196, 5);
            this.lblLevel2.Name = "lblLevel2";
            this.lblLevel2.Size = new System.Drawing.Size(80, 45);
            this.lblLevel2.TabIndex = 44;
            this.lblLevel2.Tag = "2";
            this.lblLevel2.Text = "주의";
            this.lblLevel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLevel2.Click += new System.EventHandler(this.lblLevel_Click);
            // 
            // lblLevel3
            // 
            this.lblLevel3.BackColor = System.Drawing.Color.White;
            this.lblLevel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLevel3.Font = new System.Drawing.Font("나눔바른고딕", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblLevel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(155)))), ((int)(((byte)(155)))));
            this.lblLevel3.Location = new System.Drawing.Point(282, 5);
            this.lblLevel3.Name = "lblLevel3";
            this.lblLevel3.Size = new System.Drawing.Size(80, 45);
            this.lblLevel3.TabIndex = 45;
            this.lblLevel3.Tag = "3";
            this.lblLevel3.Text = "경계";
            this.lblLevel3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLevel3.Click += new System.EventHandler(this.lblLevel_Click);
            // 
            // lblLevel4
            // 
            this.lblLevel4.BackColor = System.Drawing.Color.White;
            this.lblLevel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLevel4.Font = new System.Drawing.Font("나눔바른고딕", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblLevel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(155)))), ((int)(((byte)(155)))));
            this.lblLevel4.Location = new System.Drawing.Point(368, 5);
            this.lblLevel4.Name = "lblLevel4";
            this.lblLevel4.Size = new System.Drawing.Size(80, 45);
            this.lblLevel4.TabIndex = 46;
            this.lblLevel4.Tag = "4";
            this.lblLevel4.Text = "심각";
            this.lblLevel4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLevel4.Click += new System.EventHandler(this.lblLevel_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.White;
            this.label7.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.label7.Location = new System.Drawing.Point(15, 15);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 23);
            this.label7.TabIndex = 47;
            this.label7.Text = "메모";
            // 
            // pnLevel
            // 
            this.pnLevel.BackColor = System.Drawing.Color.White;
            this.pnLevel.Controls.Add(this.lblLevel1);
            this.pnLevel.Controls.Add(this.label6);
            this.pnLevel.Controls.Add(this.lblLevel4);
            this.pnLevel.Controls.Add(this.lblLevel2);
            this.pnLevel.Controls.Add(this.lblLevel3);
            this.pnLevel.Location = new System.Drawing.Point(172, 392);
            this.pnLevel.Name = "pnLevel";
            this.pnLevel.Size = new System.Drawing.Size(456, 55);
            this.pnLevel.TabIndex = 48;
            // 
            // pnMemo
            // 
            this.pnMemo.BackColor = System.Drawing.Color.White;
            this.pnMemo.Controls.Add(this.panel7);
            this.pnMemo.Controls.Add(this.label7);
            this.pnMemo.Location = new System.Drawing.Point(172, 316);
            this.pnMemo.Name = "pnMemo";
            this.pnMemo.Size = new System.Drawing.Size(764, 55);
            this.pnMemo.TabIndex = 49;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.White;
            this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel7.Controls.Add(this.txtMemo);
            this.panel7.Location = new System.Drawing.Point(110, 2);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(642, 50);
            this.panel7.TabIndex = 42;
            // 
            // txtMemo
            // 
            this.txtMemo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMemo.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtMemo.Location = new System.Drawing.Point(12, 11);
            this.txtMemo.Name = "txtMemo";
            this.txtMemo.Size = new System.Drawing.Size(611, 23);
            this.txtMemo.TabIndex = 0;
            // 
            // rbtnCorona
            // 
            this.rbtnCorona.BackColor = System.Drawing.Color.White;
            this.rbtnCorona.CheckButton = false;
            this.rbtnCorona.CheckedBkgndImage = null;
            this.rbtnCorona.CheckedImage = global::SDMS_Building.Properties.Resources.report_corona_click;
            this.rbtnCorona.CheckedMouseOver = global::SDMS_Building.Properties.Resources.report_corona_click;
            this.rbtnCorona.ClickedBackgroundImage = null;
            this.rbtnCorona.ClickedImage = global::SDMS_Building.Properties.Resources.report_corona_click;
            this.rbtnCorona.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.rbtnCorona.DisabledBkgndImage = null;
            this.rbtnCorona.DisabledImage = null;
            this.rbtnCorona.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnCorona.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnCorona.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnCorona.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnCorona.ForeColorsByTypeUse = false;
            this.rbtnCorona.ID = -1;
            this.rbtnCorona.InitButtonWidth = 100;
            this.rbtnCorona.IsChecked = false;
            this.rbtnCorona.Location = new System.Drawing.Point(612, 117);
            this.rbtnCorona.MouseOverBkgndImage = null;
            this.rbtnCorona.MouseOverImage = global::SDMS_Building.Properties.Resources.report_corona_hover;
            this.rbtnCorona.Name = "rbtnCorona";
            this.rbtnCorona.NormalImage = global::SDMS_Building.Properties.Resources.report_corona_normal;
            this.rbtnCorona.Owner = null;
            this.rbtnCorona.Size = new System.Drawing.Size(100, 60);
            this.rbtnCorona.TabIndex = 50;
            this.rbtnCorona.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnCorona.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnCorona.ToolTipText = "";
            this.rbtnCorona.UseCustomImageRect = false;
            this.rbtnCorona.UseTextLocation = false;
            this.rbtnCorona.UseVisualStyleBackColor = false;
            this.rbtnCorona.Click += new System.EventHandler(this.rbtn_Click);
            // 
            // rbtnBlackout
            // 
            this.rbtnBlackout.BackColor = System.Drawing.Color.White;
            this.rbtnBlackout.CheckButton = false;
            this.rbtnBlackout.CheckedBkgndImage = null;
            this.rbtnBlackout.CheckedImage = global::SDMS_Building.Properties.Resources.report_blackout_click;
            this.rbtnBlackout.CheckedMouseOver = global::SDMS_Building.Properties.Resources.report_blackout_click;
            this.rbtnBlackout.ClickedBackgroundImage = null;
            this.rbtnBlackout.ClickedImage = global::SDMS_Building.Properties.Resources.report_blackout_click;
            this.rbtnBlackout.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.rbtnBlackout.DisabledBkgndImage = null;
            this.rbtnBlackout.DisabledImage = null;
            this.rbtnBlackout.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnBlackout.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnBlackout.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnBlackout.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnBlackout.ForeColorsByTypeUse = false;
            this.rbtnBlackout.ID = -1;
            this.rbtnBlackout.InitButtonWidth = 100;
            this.rbtnBlackout.IsChecked = false;
            this.rbtnBlackout.Location = new System.Drawing.Point(824, 117);
            this.rbtnBlackout.MouseOverBkgndImage = null;
            this.rbtnBlackout.MouseOverImage = global::SDMS_Building.Properties.Resources.report_blackout_hover;
            this.rbtnBlackout.Name = "rbtnBlackout";
            this.rbtnBlackout.NormalImage = global::SDMS_Building.Properties.Resources.report_blackout_normal;
            this.rbtnBlackout.Owner = null;
            this.rbtnBlackout.Size = new System.Drawing.Size(100, 60);
            this.rbtnBlackout.TabIndex = 36;
            this.rbtnBlackout.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnBlackout.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnBlackout.ToolTipText = "";
            this.rbtnBlackout.UseCustomImageRect = false;
            this.rbtnBlackout.UseTextLocation = false;
            this.rbtnBlackout.UseVisualStyleBackColor = false;
            this.rbtnBlackout.Click += new System.EventHandler(this.rbtn_Click);
            // 
            // rbtnSubmergency
            // 
            this.rbtnSubmergency.BackColor = System.Drawing.Color.White;
            this.rbtnSubmergency.CheckButton = false;
            this.rbtnSubmergency.CheckedBkgndImage = null;
            this.rbtnSubmergency.CheckedImage = global::SDMS_Building.Properties.Resources.report_submergence_click;
            this.rbtnSubmergency.CheckedMouseOver = global::SDMS_Building.Properties.Resources.report_submergence_click;
            this.rbtnSubmergency.ClickedBackgroundImage = null;
            this.rbtnSubmergency.ClickedImage = global::SDMS_Building.Properties.Resources.report_submergence_click;
            this.rbtnSubmergency.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.rbtnSubmergency.DisabledBkgndImage = null;
            this.rbtnSubmergency.DisabledImage = null;
            this.rbtnSubmergency.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnSubmergency.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnSubmergency.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnSubmergency.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnSubmergency.ForeColorsByTypeUse = false;
            this.rbtnSubmergency.ID = -1;
            this.rbtnSubmergency.InitButtonWidth = 100;
            this.rbtnSubmergency.IsChecked = false;
            this.rbtnSubmergency.Location = new System.Drawing.Point(505, 117);
            this.rbtnSubmergency.MouseOverBkgndImage = null;
            this.rbtnSubmergency.MouseOverImage = global::SDMS_Building.Properties.Resources.report_submergence_hover;
            this.rbtnSubmergency.Name = "rbtnSubmergency";
            this.rbtnSubmergency.NormalImage = global::SDMS_Building.Properties.Resources.report_submergence_normal;
            this.rbtnSubmergency.Owner = null;
            this.rbtnSubmergency.Size = new System.Drawing.Size(100, 60);
            this.rbtnSubmergency.TabIndex = 35;
            this.rbtnSubmergency.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnSubmergency.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnSubmergency.ToolTipText = "";
            this.rbtnSubmergency.UseCustomImageRect = false;
            this.rbtnSubmergency.UseTextLocation = false;
            this.rbtnSubmergency.UseVisualStyleBackColor = false;
            this.rbtnSubmergency.Click += new System.EventHandler(this.rbtn_Click);
            // 
            // rbtnStrongwind
            // 
            this.rbtnStrongwind.BackColor = System.Drawing.Color.White;
            this.rbtnStrongwind.CheckButton = false;
            this.rbtnStrongwind.CheckedBkgndImage = null;
            this.rbtnStrongwind.CheckedImage = global::SDMS_Building.Properties.Resources.report_strongwind_click;
            this.rbtnStrongwind.CheckedMouseOver = global::SDMS_Building.Properties.Resources.report_strongwind_click;
            this.rbtnStrongwind.ClickedBackgroundImage = null;
            this.rbtnStrongwind.ClickedImage = global::SDMS_Building.Properties.Resources.report_strongwind_click;
            this.rbtnStrongwind.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.rbtnStrongwind.DisabledBkgndImage = null;
            this.rbtnStrongwind.DisabledImage = null;
            this.rbtnStrongwind.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnStrongwind.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnStrongwind.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnStrongwind.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnStrongwind.ForeColorsByTypeUse = false;
            this.rbtnStrongwind.ID = -1;
            this.rbtnStrongwind.InitButtonWidth = 100;
            this.rbtnStrongwind.IsChecked = false;
            this.rbtnStrongwind.Location = new System.Drawing.Point(718, 183);
            this.rbtnStrongwind.MouseOverBkgndImage = null;
            this.rbtnStrongwind.MouseOverImage = global::SDMS_Building.Properties.Resources.report_strongwind_hover;
            this.rbtnStrongwind.Name = "rbtnStrongwind";
            this.rbtnStrongwind.NormalImage = global::SDMS_Building.Properties.Resources.report_stringwind_normal;
            this.rbtnStrongwind.Owner = null;
            this.rbtnStrongwind.Size = new System.Drawing.Size(100, 60);
            this.rbtnStrongwind.TabIndex = 34;
            this.rbtnStrongwind.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnStrongwind.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnStrongwind.ToolTipText = "";
            this.rbtnStrongwind.UseCustomImageRect = false;
            this.rbtnStrongwind.UseTextLocation = false;
            this.rbtnStrongwind.UseVisualStyleBackColor = false;
            this.rbtnStrongwind.Visible = false;
            this.rbtnStrongwind.Click += new System.EventHandler(this.rbtn_Click);
            // 
            // rbtnTerror
            // 
            this.rbtnTerror.BackColor = System.Drawing.Color.White;
            this.rbtnTerror.CheckButton = false;
            this.rbtnTerror.CheckedBkgndImage = null;
            this.rbtnTerror.CheckedImage = global::SDMS_Building.Properties.Resources.report_terror_click;
            this.rbtnTerror.CheckedMouseOver = global::SDMS_Building.Properties.Resources.report_terror_click;
            this.rbtnTerror.ClickedBackgroundImage = null;
            this.rbtnTerror.ClickedImage = global::SDMS_Building.Properties.Resources.report_terror_click;
            this.rbtnTerror.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.rbtnTerror.DisabledBkgndImage = null;
            this.rbtnTerror.DisabledImage = null;
            this.rbtnTerror.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnTerror.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnTerror.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnTerror.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnTerror.ForeColorsByTypeUse = false;
            this.rbtnTerror.ID = -1;
            this.rbtnTerror.InitButtonWidth = 100;
            this.rbtnTerror.IsChecked = false;
            this.rbtnTerror.Location = new System.Drawing.Point(394, 117);
            this.rbtnTerror.MouseOverBkgndImage = null;
            this.rbtnTerror.MouseOverImage = global::SDMS_Building.Properties.Resources.report_terror_hover;
            this.rbtnTerror.Name = "rbtnTerror";
            this.rbtnTerror.NormalImage = global::SDMS_Building.Properties.Resources.report_terror_normal;
            this.rbtnTerror.Owner = null;
            this.rbtnTerror.Size = new System.Drawing.Size(100, 60);
            this.rbtnTerror.TabIndex = 33;
            this.rbtnTerror.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnTerror.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnTerror.ToolTipText = "";
            this.rbtnTerror.UseCustomImageRect = false;
            this.rbtnTerror.UseTextLocation = false;
            this.rbtnTerror.UseVisualStyleBackColor = false;
            this.rbtnTerror.Click += new System.EventHandler(this.rbtn_Click);
            // 
            // rbtnPSM
            // 
            this.rbtnPSM.BackColor = System.Drawing.Color.White;
            this.rbtnPSM.CheckButton = false;
            this.rbtnPSM.CheckedBkgndImage = null;
            this.rbtnPSM.CheckedImage = global::SDMS_Building.Properties.Resources.report_psm_click;
            this.rbtnPSM.CheckedMouseOver = global::SDMS_Building.Properties.Resources.report_psm_click;
            this.rbtnPSM.ClickedBackgroundImage = null;
            this.rbtnPSM.ClickedImage = global::SDMS_Building.Properties.Resources.report_psm_click;
            this.rbtnPSM.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.rbtnPSM.DisabledBkgndImage = null;
            this.rbtnPSM.DisabledImage = null;
            this.rbtnPSM.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnPSM.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnPSM.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnPSM.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnPSM.ForeColorsByTypeUse = false;
            this.rbtnPSM.ID = -1;
            this.rbtnPSM.InitButtonWidth = 100;
            this.rbtnPSM.IsChecked = false;
            this.rbtnPSM.Location = new System.Drawing.Point(283, 117);
            this.rbtnPSM.MouseOverBkgndImage = null;
            this.rbtnPSM.MouseOverImage = global::SDMS_Building.Properties.Resources.report_psm_hover;
            this.rbtnPSM.Name = "rbtnPSM";
            this.rbtnPSM.NormalImage = global::SDMS_Building.Properties.Resources.report_psm_normal;
            this.rbtnPSM.Owner = null;
            this.rbtnPSM.Size = new System.Drawing.Size(100, 60);
            this.rbtnPSM.TabIndex = 32;
            this.rbtnPSM.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnPSM.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnPSM.ToolTipText = "";
            this.rbtnPSM.UseCustomImageRect = false;
            this.rbtnPSM.UseTextLocation = false;
            this.rbtnPSM.UseVisualStyleBackColor = false;
            this.rbtnPSM.Click += new System.EventHandler(this.rbtn_Click);
            // 
            // rbtnEarthquake
            // 
            this.rbtnEarthquake.BackColor = System.Drawing.Color.White;
            this.rbtnEarthquake.CheckButton = false;
            this.rbtnEarthquake.CheckedBkgndImage = null;
            this.rbtnEarthquake.CheckedImage = global::SDMS_Building.Properties.Resources.report_earthquake_click;
            this.rbtnEarthquake.CheckedMouseOver = global::SDMS_Building.Properties.Resources.report_earthquake_click;
            this.rbtnEarthquake.ClickedBackgroundImage = null;
            this.rbtnEarthquake.ClickedImage = global::SDMS_Building.Properties.Resources.report_earthquake_click;
            this.rbtnEarthquake.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.rbtnEarthquake.DisabledBkgndImage = null;
            this.rbtnEarthquake.DisabledImage = null;
            this.rbtnEarthquake.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnEarthquake.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnEarthquake.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnEarthquake.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnEarthquake.ForeColorsByTypeUse = false;
            this.rbtnEarthquake.ID = -1;
            this.rbtnEarthquake.InitButtonWidth = 100;
            this.rbtnEarthquake.IsChecked = false;
            this.rbtnEarthquake.Location = new System.Drawing.Point(718, 117);
            this.rbtnEarthquake.MouseOverBkgndImage = null;
            this.rbtnEarthquake.MouseOverImage = global::SDMS_Building.Properties.Resources.report_earthquake_hover;
            this.rbtnEarthquake.Name = "rbtnEarthquake";
            this.rbtnEarthquake.NormalImage = global::SDMS_Building.Properties.Resources.report_earthquake_normal;
            this.rbtnEarthquake.Owner = null;
            this.rbtnEarthquake.Size = new System.Drawing.Size(100, 60);
            this.rbtnEarthquake.TabIndex = 31;
            this.rbtnEarthquake.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnEarthquake.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnEarthquake.ToolTipText = "";
            this.rbtnEarthquake.UseCustomImageRect = false;
            this.rbtnEarthquake.UseTextLocation = false;
            this.rbtnEarthquake.UseVisualStyleBackColor = false;
            this.rbtnEarthquake.Click += new System.EventHandler(this.rbtn_Click);
            // 
            // rbtnFire
            // 
            this.rbtnFire.BackColor = System.Drawing.Color.White;
            this.rbtnFire.CheckButton = false;
            this.rbtnFire.CheckedBkgndImage = null;
            this.rbtnFire.CheckedImage = global::SDMS_Building.Properties.Resources.report_fire_click;
            this.rbtnFire.CheckedMouseOver = global::SDMS_Building.Properties.Resources.report_fire_click;
            this.rbtnFire.ClickedBackgroundImage = null;
            this.rbtnFire.ClickedImage = global::SDMS_Building.Properties.Resources.report_fire_click;
            this.rbtnFire.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.rbtnFire.DisabledBkgndImage = null;
            this.rbtnFire.DisabledImage = null;
            this.rbtnFire.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnFire.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnFire.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnFire.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnFire.ForeColorsByTypeUse = false;
            this.rbtnFire.ID = -1;
            this.rbtnFire.InitButtonWidth = 100;
            this.rbtnFire.IsChecked = true;
            this.rbtnFire.Location = new System.Drawing.Point(172, 117);
            this.rbtnFire.MouseOverBkgndImage = null;
            this.rbtnFire.MouseOverImage = global::SDMS_Building.Properties.Resources.report_fire_hover;
            this.rbtnFire.Name = "rbtnFire";
            this.rbtnFire.NormalImage = global::SDMS_Building.Properties.Resources.report_fire_normal;
            this.rbtnFire.Owner = null;
            this.rbtnFire.Size = new System.Drawing.Size(100, 60);
            this.rbtnFire.TabIndex = 30;
            this.rbtnFire.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnFire.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnFire.ToolTipText = "";
            this.rbtnFire.UseCustomImageRect = false;
            this.rbtnFire.UseTextLocation = false;
            this.rbtnFire.UseVisualStyleBackColor = false;
            this.rbtnFire.Click += new System.EventHandler(this.rbtn_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.ButtonText = "";
            this.btnConfirm.ImageClicked = global::SDMS_Building.Properties.Resources.manualReport2_click;
            this.btnConfirm.ImageDisabled = null;
            this.btnConfirm.ImageMouseOver = global::SDMS_Building.Properties.Resources.manualReport2_click;
            this.btnConfirm.ImageNormal = global::SDMS_Building.Properties.Resources.manualReport2_normal;
            this.btnConfirm.Location = new System.Drawing.Point(394, 504);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Owner = null;
            this.btnConfirm.Size = new System.Drawing.Size(195, 60);
            this.btnConfirm.TabIndex = 25;
            this.btnConfirm.TabStop = false;
            this.btnConfirm.TextColor = System.Drawing.Color.White;
            this.btnConfirm.TextFont = new System.Drawing.Font("나눔바른고딕", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnConfirm.ToolTipText = "";
            this.btnConfirm.UseToolTip = false;
            this.btnConfirm.WindowRateWidth = 1F;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // FormManualReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.ClientSize = new System.Drawing.Size(1000, 582);
            this.Controls.Add(this.rbtnCorona);
            this.Controls.Add(this.pnMemo);
            this.Controls.Add(this.pnLevel);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.rbtnBlackout);
            this.Controls.Add(this.rbtnSubmergency);
            this.Controls.Add(this.rbtnStrongwind);
            this.Controls.Add(this.rbtnTerror);
            this.Controls.Add(this.rbtnPSM);
            this.Controls.Add(this.rbtnEarthquake);
            this.Controls.Add(this.rbtnFire);
            this.Controls.Add(this.lblBuilding);
            this.Controls.Add(this.lblFloor);
            this.Controls.Add(this.eleFloor);
            this.Controls.Add(this.eleBuilding);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnConfirm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormManualReport";
            this.ShowInTaskbar = false;
            this.Text = "FormManualReport";
            this.Load += new System.EventHandler(this.FormManualReport_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormManualReport_Paint);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.pnLevel.ResumeLayout(false);
            this.pnLevel.PerformLayout();
            this.pnMemo.ResumeLayout(false);
            this.pnMemo.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnConfirm)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel3;
        private UnE.GUI.ImageButton btnClose;
        private UnE.GUI.ImageButton btnConfirm;
        private System.Windows.Forms.Integration.ElementHost eleFloor;
        private System.Windows.Forms.Integration.ElementHost eleBuilding;
        private System.Windows.Forms.Label lblFloor;
        private System.Windows.Forms.Label lblBuilding;
        private UnE.GUI.RibbonButton rbtnFire;
        private UnE.GUI.RibbonButton rbtnEarthquake;
        private UnE.GUI.RibbonButton rbtnPSM;
        private UnE.GUI.RibbonButton rbtnTerror;
        private UnE.GUI.RibbonButton rbtnStrongwind;
        private UnE.GUI.RibbonButton rbtnSubmergency;
        private UnE.GUI.RibbonButton rbtnBlackout;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TextBox txtTeam;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblLevel1;
        private System.Windows.Forms.Label lblLevel2;
        private System.Windows.Forms.Label lblLevel3;
        private System.Windows.Forms.Label lblLevel4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel pnLevel;
        private System.Windows.Forms.Panel pnMemo;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.TextBox txtMemo;
        private UnE.GUI.RibbonButton rbtnCorona;
    }
}