namespace IntegratedManagement2
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ckbSituationMode = new System.Windows.Forms.CheckBox();
            this.lbSiteName = new System.Windows.Forms.Label();
            this.ribbonButtonSetup = new IntegratedManagement2.RibbonButton();
            this.ribbonButton1 = new IntegratedManagement2.RibbonButton();
            this.btnShowCreator = new IntegratedManagement2.RibbonButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label1.Location = new System.Drawing.Point(47, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "서버 URL";
            // 
            // labelNTPServer
            // 
            this.labelNTPServer.AutoSize = true;
            this.labelNTPServer.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelNTPServer.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelNTPServer.Location = new System.Drawing.Point(47, 104);
            this.labelNTPServer.Name = "labelNTPServer";
            this.labelNTPServer.Size = new System.Drawing.Size(63, 17);
            this.labelNTPServer.TabIndex = 1;
            this.labelNTPServer.Text = "NTP 서버";
            this.labelNTPServer.Click += new System.EventHandler(this.label2_Click);
            // 
            // labelDrawingFTP
            // 
            this.labelDrawingFTP.AutoSize = true;
            this.labelDrawingFTP.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelDrawingFTP.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelDrawingFTP.Location = new System.Drawing.Point(47, 74);
            this.labelDrawingFTP.Name = "labelDrawingFTP";
            this.labelDrawingFTP.Size = new System.Drawing.Size(59, 17);
            this.labelDrawingFTP.TabIndex = 2;
            this.labelDrawingFTP.Text = "도면 FTP";
            this.labelDrawingFTP.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label4.Location = new System.Drawing.Point(25, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 17);
            this.label4.TabIndex = 3;
            this.label4.Text = "모니터1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label5.Location = new System.Drawing.Point(25, 60);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 17);
            this.label5.TabIndex = 4;
            this.label5.Text = "모니터3";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label6.Location = new System.Drawing.Point(239, 31);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 17);
            this.label6.TabIndex = 5;
            this.label6.Text = "모니터2";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label7.Location = new System.Drawing.Point(239, 60);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 17);
            this.label7.TabIndex = 6;
            this.label7.Text = "모니터4";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.comboBox1.ForeColor = System.Drawing.Color.Black;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "기본 설정",
            "SOPSimulator",
            "SDMS",
            "MissionList",
            "CCTV"});
            this.comboBox1.Location = new System.Drawing.Point(80, 28);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(143, 25);
            this.comboBox1.TabIndex = 7;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.comboBox2.ForeColor = System.Drawing.Color.Black;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "기본 설정",
            "SOPSimulator",
            "SDMS",
            "MissionList",
            "CCTV"});
            this.comboBox2.Location = new System.Drawing.Point(296, 28);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(143, 25);
            this.comboBox2.TabIndex = 8;
            // 
            // comboBox3
            // 
            this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox3.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.comboBox3.ForeColor = System.Drawing.Color.Black;
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Items.AddRange(new object[] {
            "기본 설정",
            "SOPSimulator",
            "SDMS",
            "MissionList",
            "CCTV"});
            this.comboBox3.Location = new System.Drawing.Point(80, 57);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(143, 25);
            this.comboBox3.TabIndex = 9;
            // 
            // comboBox4
            // 
            this.comboBox4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox4.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.comboBox4.ForeColor = System.Drawing.Color.Black;
            this.comboBox4.FormattingEnabled = true;
            this.comboBox4.Items.AddRange(new object[] {
            "기본 설정",
            "SOPSimulator",
            "SDMS",
            "MissionList",
            "CCTV"});
            this.comboBox4.Location = new System.Drawing.Point(296, 57);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(143, 25);
            this.comboBox4.TabIndex = 10;
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox1.Location = new System.Drawing.Point(135, 41);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(284, 25);
            this.textBox1.TabIndex = 11;
            // 
            // textBoxDrawingFTP
            // 
            this.textBoxDrawingFTP.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxDrawingFTP.Location = new System.Drawing.Point(135, 71);
            this.textBoxDrawingFTP.Name = "textBoxDrawingFTP";
            this.textBoxDrawingFTP.Size = new System.Drawing.Size(284, 25);
            this.textBoxDrawingFTP.TabIndex = 12;
            this.textBoxDrawingFTP.Visible = false;
            // 
            // textBoxNTPServer
            // 
            this.textBoxNTPServer.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxNTPServer.Location = new System.Drawing.Point(135, 101);
            this.textBoxNTPServer.Name = "textBoxNTPServer";
            this.textBoxNTPServer.Size = new System.Drawing.Size(284, 25);
            this.textBoxNTPServer.TabIndex = 13;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBox4);
            this.groupBox1.Controls.Add(this.comboBox3);
            this.groupBox1.Controls.Add(this.comboBox2);
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Font = new System.Drawing.Font("맑은 고딕", 9.75F);
            this.groupBox1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox1.Location = new System.Drawing.Point(5, 138);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(475, 96);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "모니터 설정";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ckbSituationMode);
            this.groupBox2.Controls.Add(this.lbSiteName);
            this.groupBox2.Controls.Add(this.textBoxNTPServer);
            this.groupBox2.Controls.Add(this.textBoxDrawingFTP);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.labelDrawingFTP);
            this.groupBox2.Controls.Add(this.labelNTPServer);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox2.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox2.Location = new System.Drawing.Point(5, -1);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(475, 136);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "서버 설정";
            // 
            // ckbSituationMode
            // 
            this.ckbSituationMode.AutoSize = true;
            this.ckbSituationMode.Checked = true;
            this.ckbSituationMode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbSituationMode.Location = new System.Drawing.Point(377, 14);
            this.ckbSituationMode.Name = "ckbSituationMode";
            this.ckbSituationMode.Size = new System.Drawing.Size(92, 21);
            this.ckbSituationMode.TabIndex = 16;
            this.ckbSituationMode.Text = "상황실모드";
            this.ckbSituationMode.UseVisualStyleBackColor = true;
            this.ckbSituationMode.Visible = false;
            this.ckbSituationMode.CheckedChanged += new System.EventHandler(this.ckbSituationMode_CheckedChanged);
            // 
            // lbSiteName
            // 
            this.lbSiteName.AutoSize = true;
            this.lbSiteName.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbSiteName.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lbSiteName.Location = new System.Drawing.Point(194, 17);
            this.lbSiteName.Name = "lbSiteName";
            this.lbSiteName.Size = new System.Drawing.Size(86, 17);
            this.lbSiteName.TabIndex = 15;
            this.lbSiteName.Text = "영흥화력본부";
            // 
            // ribbonButtonSetup
            // 
            this.ribbonButtonSetup.BackColor = System.Drawing.Color.Transparent;
            this.ribbonButtonSetup.BackgroundImage = global::IntegratedManagement2.Properties.Resources.button;
            this.ribbonButtonSetup.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ribbonButtonSetup.CheckedBkgndImage = null;
            this.ribbonButtonSetup.CheckedImage = null;
            this.ribbonButtonSetup.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.ribbonButtonSetup.ForeColor = System.Drawing.Color.White;
            this.ribbonButtonSetup.IsChecked = false;
            this.ribbonButtonSetup.Location = new System.Drawing.Point(119, 240);
            this.ribbonButtonSetup.MouseOverBkgndImage = null;
            this.ribbonButtonSetup.Name = "ribbonButtonSetup";
            this.ribbonButtonSetup.NormalImage = null;
            this.ribbonButtonSetup.Owner = null;
            this.ribbonButtonSetup.Size = new System.Drawing.Size(107, 34);
            this.ribbonButtonSetup.TabIndex = 22;
            this.ribbonButtonSetup.Text = "저장하기";
            this.ribbonButtonSetup.UseVisualStyleBackColor = false;
            this.ribbonButtonSetup.Click += new System.EventHandler(this.ribbonButtonSetup_Click);
            // 
            // ribbonButton1
            // 
            this.ribbonButton1.BackColor = System.Drawing.Color.Transparent;
            this.ribbonButton1.BackgroundImage = global::IntegratedManagement2.Properties.Resources.button;
            this.ribbonButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ribbonButton1.CheckedBkgndImage = null;
            this.ribbonButton1.CheckedImage = null;
            this.ribbonButton1.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.ribbonButton1.ForeColor = System.Drawing.Color.White;
            this.ribbonButton1.IsChecked = false;
            this.ribbonButton1.Location = new System.Drawing.Point(240, 240);
            this.ribbonButton1.MouseOverBkgndImage = null;
            this.ribbonButton1.Name = "ribbonButton1";
            this.ribbonButton1.NormalImage = null;
            this.ribbonButton1.Owner = null;
            this.ribbonButton1.Size = new System.Drawing.Size(107, 34);
            this.ribbonButton1.TabIndex = 23;
            this.ribbonButton1.Text = "닫기";
            this.ribbonButton1.UseVisualStyleBackColor = false;
            this.ribbonButton1.Click += new System.EventHandler(this.ribbonButton1_Click);
            // 
            // btnShowCreator
            // 
            this.btnShowCreator.BackColor = System.Drawing.Color.Transparent;
            this.btnShowCreator.BackgroundImage = global::IntegratedManagement2.Properties.Resources.button;
            this.btnShowCreator.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnShowCreator.CheckedBkgndImage = null;
            this.btnShowCreator.CheckedImage = null;
            this.btnShowCreator.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnShowCreator.ForeColor = System.Drawing.Color.White;
            this.btnShowCreator.IsChecked = false;
            this.btnShowCreator.Location = new System.Drawing.Point(405, 249);
            this.btnShowCreator.MouseOverBkgndImage = null;
            this.btnShowCreator.Name = "btnShowCreator";
            this.btnShowCreator.NormalImage = null;
            this.btnShowCreator.Owner = null;
            this.btnShowCreator.Size = new System.Drawing.Size(75, 25);
            this.btnShowCreator.TabIndex = 23;
            this.btnShowCreator.Text = "만든이";
            this.btnShowCreator.UseVisualStyleBackColor = false;
            this.btnShowCreator.Click += new System.EventHandler(this.btnShowCreator_Click);
            // 
            // FormPreference
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.ClientSize = new System.Drawing.Size(492, 280);
            this.Controls.Add(this.btnShowCreator);
            this.Controls.Add(this.ribbonButton1);
            this.Controls.Add(this.ribbonButtonSetup);
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
        private RibbonButton ribbonButtonSetup;
        private RibbonButton ribbonButton1;
        private System.Windows.Forms.Label lbSiteName;
        private RibbonButton btnShowCreator;
        private System.Windows.Forms.CheckBox ckbSituationMode;
    }
}