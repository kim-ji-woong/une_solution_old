namespace IntegratedManagement4
{
    partial class FormPreference
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
            this.label1 = new System.Windows.Forms.Label();
            this.labelNTPServer = new System.Windows.Forms.Label();
            this.labelDrawingFTP = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBoxDrawingFTP = new System.Windows.Forms.TextBox();
            this.textBoxNTPServer = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbMonitorSet = new UnE.GUI.RibbonButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbSiteName = new System.Windows.Forms.Label();
            this.ckbSituationMode = new System.Windows.Forms.CheckBox();
            this.ribbonButton1 = new UnE.GUI.RibbonButton();
            this.ribbonButtonSetup = new UnE.GUI.RibbonButton();
            this.btnShowCreator = new IntegratedManagement4.RibbonButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label1.Location = new System.Drawing.Point(18, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "서버 URL";
            // 
            // labelNTPServer
            // 
            this.labelNTPServer.AutoSize = true;
            this.labelNTPServer.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelNTPServer.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelNTPServer.Location = new System.Drawing.Point(18, 103);
            this.labelNTPServer.Name = "labelNTPServer";
            this.labelNTPServer.Size = new System.Drawing.Size(74, 18);
            this.labelNTPServer.TabIndex = 1;
            this.labelNTPServer.Text = "NTP 서버";
            this.labelNTPServer.Click += new System.EventHandler(this.label2_Click);
            // 
            // labelDrawingFTP
            // 
            this.labelDrawingFTP.AutoSize = true;
            this.labelDrawingFTP.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelDrawingFTP.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelDrawingFTP.Location = new System.Drawing.Point(18, 73);
            this.labelDrawingFTP.Name = "labelDrawingFTP";
            this.labelDrawingFTP.Size = new System.Drawing.Size(72, 18);
            this.labelDrawingFTP.TabIndex = 2;
            this.labelDrawingFTP.Text = "도면 FTP";
            this.labelDrawingFTP.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label4.Location = new System.Drawing.Point(18, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 18);
            this.label4.TabIndex = 3;
            this.label4.Text = "모니터1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label5.Location = new System.Drawing.Point(18, 62);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 18);
            this.label5.TabIndex = 4;
            this.label5.Text = "모니터3";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label6.Location = new System.Drawing.Point(248, 33);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 18);
            this.label6.TabIndex = 5;
            this.label6.Text = "모니터2";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label7.Location = new System.Drawing.Point(248, 62);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 18);
            this.label7.TabIndex = 6;
            this.label7.Text = "모니터4";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.comboBox1.ForeColor = System.Drawing.Color.Black;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "기본 설정",
            "SOPSimulator",
            "SDMS",
            "MissionList",
            "CCTV"});
            this.comboBox1.Location = new System.Drawing.Point(83, 30);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(143, 26);
            this.comboBox1.TabIndex = 7;
            this.comboBox1.SelectionChangeCommitted += new System.EventHandler(this.comboBox_SelectionChangeCommitted);
            this.comboBox1.Enter += new System.EventHandler(this.comboBox_Enter);
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.comboBox2.ForeColor = System.Drawing.Color.Black;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "기본 설정",
            "SOPSimulator",
            "SDMS",
            "MissionList",
            "CCTV"});
            this.comboBox2.Location = new System.Drawing.Point(315, 30);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(143, 26);
            this.comboBox2.TabIndex = 8;
            this.comboBox2.SelectionChangeCommitted += new System.EventHandler(this.comboBox_SelectionChangeCommitted);
            this.comboBox2.Enter += new System.EventHandler(this.comboBox_Enter);
            // 
            // comboBox3
            // 
            this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox3.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.comboBox3.ForeColor = System.Drawing.Color.Black;
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Items.AddRange(new object[] {
            "기본 설정",
            "SOPSimulator",
            "SDMS",
            "MissionList",
            "CCTV"});
            this.comboBox3.Location = new System.Drawing.Point(83, 59);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(143, 26);
            this.comboBox3.TabIndex = 9;
            this.comboBox3.SelectionChangeCommitted += new System.EventHandler(this.comboBox_SelectionChangeCommitted);
            this.comboBox3.Enter += new System.EventHandler(this.comboBox_Enter);
            // 
            // comboBox4
            // 
            this.comboBox4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox4.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.comboBox4.ForeColor = System.Drawing.Color.Black;
            this.comboBox4.FormattingEnabled = true;
            this.comboBox4.Items.AddRange(new object[] {
            "기본 설정",
            "SOPSimulator",
            "SDMS",
            "MissionList",
            "CCTV"});
            this.comboBox4.Location = new System.Drawing.Point(315, 59);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(143, 26);
            this.comboBox4.TabIndex = 10;
            this.comboBox4.SelectionChangeCommitted += new System.EventHandler(this.comboBox_SelectionChangeCommitted);
            this.comboBox4.Enter += new System.EventHandler(this.comboBox_Enter);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox1.Location = new System.Drawing.Point(106, 40);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(352, 27);
            this.textBox1.TabIndex = 11;
            // 
            // textBoxDrawingFTP
            // 
            this.textBoxDrawingFTP.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxDrawingFTP.Location = new System.Drawing.Point(106, 70);
            this.textBoxDrawingFTP.Name = "textBoxDrawingFTP";
            this.textBoxDrawingFTP.Size = new System.Drawing.Size(352, 27);
            this.textBoxDrawingFTP.TabIndex = 12;
            this.textBoxDrawingFTP.Visible = false;
            // 
            // textBoxNTPServer
            // 
            this.textBoxNTPServer.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxNTPServer.Location = new System.Drawing.Point(106, 100);
            this.textBoxNTPServer.Name = "textBoxNTPServer";
            this.textBoxNTPServer.Size = new System.Drawing.Size(352, 27);
            this.textBoxNTPServer.TabIndex = 13;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbMonitorSet);
            this.groupBox1.Controls.Add(this.comboBox4);
            this.groupBox1.Controls.Add(this.comboBox3);
            this.groupBox1.Controls.Add(this.comboBox2);
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox1.Location = new System.Drawing.Point(48, 151);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(508, 96);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "모니터 설정";
            // 
            // rbMonitorSet
            // 
            this.rbMonitorSet.BackColor = System.Drawing.Color.Transparent;
            this.rbMonitorSet.CheckButton = false;
            this.rbMonitorSet.CheckedBkgndImage = null;
            this.rbMonitorSet.CheckedImage = null;
            this.rbMonitorSet.ClickedBackgroundImage = null;
            this.rbMonitorSet.ClickedImage = global::IntegratedManagement4.Properties.Resources.btnPropertyClick;
            this.rbMonitorSet.CustomImageRect = new System.Drawing.Rectangle(0, 0, 33, 33);
            this.rbMonitorSet.DisabledBkgndImage = null;
            this.rbMonitorSet.DisabledImage = null;
            this.rbMonitorSet.ID = -1;
            this.rbMonitorSet.InitButtonWidth = 115;
            this.rbMonitorSet.IsChecked = false;
            this.rbMonitorSet.Location = new System.Drawing.Point(467, 54);
            this.rbMonitorSet.Margin = new System.Windows.Forms.Padding(0);
            this.rbMonitorSet.MouseOverBkgndImage = null;
            this.rbMonitorSet.MouseOverImage = global::IntegratedManagement4.Properties.Resources.btnPropertyClick;
            this.rbMonitorSet.Name = "rbMonitorSet";
            this.rbMonitorSet.NormalImage = global::IntegratedManagement4.Properties.Resources.btnProperty;
            this.rbMonitorSet.Owner = null;
            this.rbMonitorSet.Size = new System.Drawing.Size(115, 35);
            this.rbMonitorSet.TabIndex = 44;
            this.rbMonitorSet.TextLocation = new System.Drawing.Point(0, 0);
            this.rbMonitorSet.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbMonitorSet.ToolTipText = "";
            this.rbMonitorSet.UseCustomImageRect = true;
            this.rbMonitorSet.UseTextLocation = false;
            this.rbMonitorSet.UseVisualStyleBackColor = false;
            this.rbMonitorSet.Click += new System.EventHandler(this.rbMonitorSet_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbSiteName);
            this.groupBox2.Controls.Add(this.textBoxNTPServer);
            this.groupBox2.Controls.Add(this.textBoxDrawingFTP);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.labelDrawingFTP);
            this.groupBox2.Controls.Add(this.labelNTPServer);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.ckbSituationMode);
            this.groupBox2.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox2.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox2.Location = new System.Drawing.Point(48, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(508, 136);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "서버 설정";
            // 
            // lbSiteName
            // 
            this.lbSiteName.AutoSize = true;
            this.lbSiteName.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbSiteName.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lbSiteName.Location = new System.Drawing.Point(200, 17);
            this.lbSiteName.Name = "lbSiteName";
            this.lbSiteName.Size = new System.Drawing.Size(98, 18);
            this.lbSiteName.TabIndex = 15;
            this.lbSiteName.Text = "영흥화력본부";
            // 
            // ckbSituationMode
            // 
            this.ckbSituationMode.AutoSize = true;
            this.ckbSituationMode.Checked = true;
            this.ckbSituationMode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbSituationMode.Location = new System.Drawing.Point(339, 13);
            this.ckbSituationMode.Name = "ckbSituationMode";
            this.ckbSituationMode.Size = new System.Drawing.Size(102, 22);
            this.ckbSituationMode.TabIndex = 16;
            this.ckbSituationMode.Text = "상황실모드";
            this.ckbSituationMode.UseVisualStyleBackColor = true;
            this.ckbSituationMode.Visible = false;
            this.ckbSituationMode.CheckedChanged += new System.EventHandler(this.ckbSituationMode_CheckedChanged);
            // 
            // ribbonButton1
            // 
            this.ribbonButton1.BackColor = System.Drawing.Color.Transparent;
            this.ribbonButton1.CheckButton = false;
            this.ribbonButton1.CheckedBkgndImage = null;
            this.ribbonButton1.CheckedImage = null;
            this.ribbonButton1.ClickedBackgroundImage = null;
            this.ribbonButton1.ClickedImage = global::IntegratedManagement4.Properties.Resources.btnCloseClick;
            this.ribbonButton1.CustomImageRect = new System.Drawing.Rectangle(0, 0, 115, 45);
            this.ribbonButton1.DisabledBkgndImage = null;
            this.ribbonButton1.DisabledImage = null;
            this.ribbonButton1.ID = -1;
            this.ribbonButton1.InitButtonWidth = 115;
            this.ribbonButton1.IsChecked = false;
            this.ribbonButton1.Location = new System.Drawing.Point(302, 247);
            this.ribbonButton1.Margin = new System.Windows.Forms.Padding(0);
            this.ribbonButton1.MouseOverBkgndImage = null;
            this.ribbonButton1.MouseOverImage = global::IntegratedManagement4.Properties.Resources.btnCloseClick;
            this.ribbonButton1.Name = "ribbonButton1";
            this.ribbonButton1.NormalImage = global::IntegratedManagement4.Properties.Resources.btnClose;
            this.ribbonButton1.Owner = null;
            this.ribbonButton1.Size = new System.Drawing.Size(115, 45);
            this.ribbonButton1.TabIndex = 43;
            this.ribbonButton1.TextLocation = new System.Drawing.Point(0, 0);
            this.ribbonButton1.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.ribbonButton1.ToolTipText = "";
            this.ribbonButton1.UseCustomImageRect = true;
            this.ribbonButton1.UseTextLocation = false;
            this.ribbonButton1.UseVisualStyleBackColor = false;
            this.ribbonButton1.Click += new System.EventHandler(this.ribbonButton1_Click);
            // 
            // ribbonButtonSetup
            // 
            this.ribbonButtonSetup.BackColor = System.Drawing.Color.Transparent;
            this.ribbonButtonSetup.CheckButton = false;
            this.ribbonButtonSetup.CheckedBkgndImage = null;
            this.ribbonButtonSetup.CheckedImage = null;
            this.ribbonButtonSetup.ClickedBackgroundImage = null;
            this.ribbonButtonSetup.ClickedImage = global::IntegratedManagement4.Properties.Resources.btnSaveClick;
            this.ribbonButtonSetup.CustomImageRect = new System.Drawing.Rectangle(0, 0, 115, 45);
            this.ribbonButtonSetup.DisabledBkgndImage = null;
            this.ribbonButtonSetup.DisabledImage = null;
            this.ribbonButtonSetup.ID = -1;
            this.ribbonButtonSetup.InitButtonWidth = 115;
            this.ribbonButtonSetup.IsChecked = false;
            this.ribbonButtonSetup.Location = new System.Drawing.Point(184, 247);
            this.ribbonButtonSetup.Margin = new System.Windows.Forms.Padding(0);
            this.ribbonButtonSetup.MouseOverBkgndImage = null;
            this.ribbonButtonSetup.MouseOverImage = global::IntegratedManagement4.Properties.Resources.btnSaveClick;
            this.ribbonButtonSetup.Name = "ribbonButtonSetup";
            this.ribbonButtonSetup.NormalImage = global::IntegratedManagement4.Properties.Resources.btnSave;
            this.ribbonButtonSetup.Owner = null;
            this.ribbonButtonSetup.Size = new System.Drawing.Size(115, 45);
            this.ribbonButtonSetup.TabIndex = 42;
            this.ribbonButtonSetup.TextLocation = new System.Drawing.Point(0, 0);
            this.ribbonButtonSetup.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.ribbonButtonSetup.ToolTipText = "";
            this.ribbonButtonSetup.UseCustomImageRect = true;
            this.ribbonButtonSetup.UseTextLocation = false;
            this.ribbonButtonSetup.UseVisualStyleBackColor = false;
            this.ribbonButtonSetup.Click += new System.EventHandler(this.ribbonButtonSetup_Click);
            // 
            // btnShowCreator
            // 
            this.btnShowCreator.BackColor = System.Drawing.Color.Transparent;
            this.btnShowCreator.BackgroundImage = global::IntegratedManagement4.Properties.Resources.button;
            this.btnShowCreator.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnShowCreator.CheckedBkgndImage = null;
            this.btnShowCreator.CheckedImage = null;
            this.btnShowCreator.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnShowCreator.ForeColor = System.Drawing.Color.White;
            this.btnShowCreator.IsChecked = false;
            this.btnShowCreator.Location = new System.Drawing.Point(563, 269);
            this.btnShowCreator.MouseOverBkgndImage = null;
            this.btnShowCreator.Name = "btnShowCreator";
            this.btnShowCreator.NormalImage = null;
            this.btnShowCreator.Owner = null;
            this.btnShowCreator.Size = new System.Drawing.Size(75, 25);
            this.btnShowCreator.TabIndex = 23;
            this.btnShowCreator.Text = "만든이";
            this.btnShowCreator.UseVisualStyleBackColor = false;
            this.btnShowCreator.Visible = false;
            this.btnShowCreator.Click += new System.EventHandler(this.btnShowCreator_Click);
            // 
            // FormPreference
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.ClientSize = new System.Drawing.Size(600, 306);
            this.Controls.Add(this.ribbonButton1);
            this.Controls.Add(this.ribbonButtonSetup);
            this.Controls.Add(this.btnShowCreator);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPreference";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "FormPreference";
            this.Load += new System.EventHandler(this.FormPreference_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelNTPServer;
        private System.Windows.Forms.Label labelDrawingFTP;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.ComboBox comboBox4;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBoxDrawingFTP;
        private System.Windows.Forms.TextBox textBoxNTPServer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lbSiteName;
        private RibbonButton btnShowCreator;
        private System.Windows.Forms.CheckBox ckbSituationMode;
        private UnE.GUI.RibbonButton ribbonButton1;
        private UnE.GUI.RibbonButton ribbonButtonSetup;
        private UnE.GUI.RibbonButton rbMonitorSet;
    }
}