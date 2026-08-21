namespace UpdateCommander
{
    partial class FormMain
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

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelServerName = new System.Windows.Forms.Label();
            this.btnServerUpdate = new System.Windows.Forms.Button();
            this.textBoxServerName = new System.Windows.Forms.TextBox();
            this.textBoxServerUpdate = new System.Windows.Forms.TextBox();
            this.radioUpdateServer = new System.Windows.Forms.RadioButton();
            this.radioStopServer = new System.Windows.Forms.RadioButton();
            this.radioStartServer = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelClientName = new System.Windows.Forms.Label();
            this.btnClientUpdate = new System.Windows.Forms.Button();
            this.textBoxClientName = new System.Windows.Forms.TextBox();
            this.textBoxClientUpdate = new System.Windows.Forms.TextBox();
            this.radioUpdateClient = new System.Windows.Forms.RadioButton();
            this.radioStopClient = new System.Windows.Forms.RadioButton();
            this.radioStartClient = new System.Windows.Forms.RadioButton();
            this.checkBoxServer = new System.Windows.Forms.CheckBox();
            this.checkBoxClient = new System.Windows.Forms.CheckBox();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.cboSite = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelServerName);
            this.groupBox1.Controls.Add(this.btnServerUpdate);
            this.groupBox1.Controls.Add(this.textBoxServerName);
            this.groupBox1.Controls.Add(this.textBoxServerUpdate);
            this.groupBox1.Controls.Add(this.radioUpdateServer);
            this.groupBox1.Controls.Add(this.radioStopServer);
            this.groupBox1.Controls.Add(this.radioStartServer);
            this.groupBox1.Location = new System.Drawing.Point(25, 54);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(300, 165);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "서버";
            // 
            // labelServerName
            // 
            this.labelServerName.AutoSize = true;
            this.labelServerName.Location = new System.Drawing.Point(19, 133);
            this.labelServerName.Name = "labelServerName";
            this.labelServerName.Size = new System.Drawing.Size(65, 12);
            this.labelServerName.TabIndex = 3;
            this.labelServerName.Text = "서버 이름 :";
            // 
            // btnServerUpdate
            // 
            this.btnServerUpdate.Enabled = false;
            this.btnServerUpdate.Location = new System.Drawing.Point(265, 93);
            this.btnServerUpdate.Name = "btnServerUpdate";
            this.btnServerUpdate.Size = new System.Drawing.Size(31, 23);
            this.btnServerUpdate.TabIndex = 2;
            this.btnServerUpdate.Text = "...";
            this.btnServerUpdate.UseVisualStyleBackColor = true;
            this.btnServerUpdate.Click += new System.EventHandler(this.btnServerUpdate_Click);
            // 
            // textBoxServerName
            // 
            this.textBoxServerName.Location = new System.Drawing.Point(90, 130);
            this.textBoxServerName.Name = "textBoxServerName";
            this.textBoxServerName.Size = new System.Drawing.Size(169, 21);
            this.textBoxServerName.TabIndex = 1;
            this.textBoxServerName.Text = "SOPWebServer";
            // 
            // textBoxServerUpdate
            // 
            this.textBoxServerUpdate.Enabled = false;
            this.textBoxServerUpdate.Location = new System.Drawing.Point(21, 94);
            this.textBoxServerUpdate.Name = "textBoxServerUpdate";
            this.textBoxServerUpdate.Size = new System.Drawing.Size(238, 21);
            this.textBoxServerUpdate.TabIndex = 1;
            // 
            // radioUpdateServer
            // 
            this.radioUpdateServer.AutoSize = true;
            this.radioUpdateServer.Location = new System.Drawing.Point(21, 72);
            this.radioUpdateServer.Name = "radioUpdateServer";
            this.radioUpdateServer.Size = new System.Drawing.Size(99, 16);
            this.radioUpdateServer.TabIndex = 0;
            this.radioUpdateServer.TabStop = true;
            this.radioUpdateServer.Text = "서버 업데이트";
            this.radioUpdateServer.UseVisualStyleBackColor = true;
            this.radioUpdateServer.CheckedChanged += new System.EventHandler(this.radioServer_CheckedChanged);
            // 
            // radioStopServer
            // 
            this.radioStopServer.AutoSize = true;
            this.radioStopServer.Location = new System.Drawing.Point(21, 50);
            this.radioStopServer.Name = "radioStopServer";
            this.radioStopServer.Size = new System.Drawing.Size(75, 16);
            this.radioStopServer.TabIndex = 0;
            this.radioStopServer.TabStop = true;
            this.radioStopServer.Text = "서버 중지";
            this.radioStopServer.UseVisualStyleBackColor = true;
            this.radioStopServer.CheckedChanged += new System.EventHandler(this.radioServer_CheckedChanged);
            // 
            // radioStartServer
            // 
            this.radioStartServer.AutoSize = true;
            this.radioStartServer.Location = new System.Drawing.Point(21, 28);
            this.radioStartServer.Name = "radioStartServer";
            this.radioStartServer.Size = new System.Drawing.Size(75, 16);
            this.radioStartServer.TabIndex = 0;
            this.radioStartServer.TabStop = true;
            this.radioStartServer.Text = "서버 시작";
            this.radioStartServer.UseVisualStyleBackColor = true;
            this.radioStartServer.CheckedChanged += new System.EventHandler(this.radioServer_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelClientName);
            this.groupBox2.Controls.Add(this.btnClientUpdate);
            this.groupBox2.Controls.Add(this.textBoxClientName);
            this.groupBox2.Controls.Add(this.textBoxClientUpdate);
            this.groupBox2.Controls.Add(this.radioUpdateClient);
            this.groupBox2.Controls.Add(this.radioStopClient);
            this.groupBox2.Controls.Add(this.radioStartClient);
            this.groupBox2.Location = new System.Drawing.Point(350, 54);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(300, 165);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "클라이언트";
            // 
            // labelClientName
            // 
            this.labelClientName.AutoSize = true;
            this.labelClientName.Location = new System.Drawing.Point(19, 133);
            this.labelClientName.Name = "labelClientName";
            this.labelClientName.Size = new System.Drawing.Size(101, 12);
            this.labelClientName.TabIndex = 3;
            this.labelClientName.Text = "클라이언트 이름 :";
            // 
            // btnClientUpdate
            // 
            this.btnClientUpdate.Enabled = false;
            this.btnClientUpdate.Location = new System.Drawing.Point(265, 93);
            this.btnClientUpdate.Name = "btnClientUpdate";
            this.btnClientUpdate.Size = new System.Drawing.Size(31, 23);
            this.btnClientUpdate.TabIndex = 2;
            this.btnClientUpdate.Text = "...";
            this.btnClientUpdate.UseVisualStyleBackColor = true;
            this.btnClientUpdate.Click += new System.EventHandler(this.btnClientUpdate_Click);
            // 
            // textBoxClientName
            // 
            this.textBoxClientName.Location = new System.Drawing.Point(126, 130);
            this.textBoxClientName.Name = "textBoxClientName";
            this.textBoxClientName.Size = new System.Drawing.Size(133, 21);
            this.textBoxClientName.TabIndex = 1;
            this.textBoxClientName.Text = "SOPSimulator_SKT";
            // 
            // textBoxClientUpdate
            // 
            this.textBoxClientUpdate.Enabled = false;
            this.textBoxClientUpdate.Location = new System.Drawing.Point(21, 94);
            this.textBoxClientUpdate.Name = "textBoxClientUpdate";
            this.textBoxClientUpdate.Size = new System.Drawing.Size(238, 21);
            this.textBoxClientUpdate.TabIndex = 1;
            // 
            // radioUpdateClient
            // 
            this.radioUpdateClient.AutoSize = true;
            this.radioUpdateClient.Location = new System.Drawing.Point(21, 72);
            this.radioUpdateClient.Name = "radioUpdateClient";
            this.radioUpdateClient.Size = new System.Drawing.Size(135, 16);
            this.radioUpdateClient.TabIndex = 0;
            this.radioUpdateClient.TabStop = true;
            this.radioUpdateClient.Text = "클라이언트 업데이트";
            this.radioUpdateClient.UseVisualStyleBackColor = true;
            this.radioUpdateClient.CheckedChanged += new System.EventHandler(this.radioClient_CheckedChanged);
            // 
            // radioStopClient
            // 
            this.radioStopClient.AutoSize = true;
            this.radioStopClient.Location = new System.Drawing.Point(21, 50);
            this.radioStopClient.Name = "radioStopClient";
            this.radioStopClient.Size = new System.Drawing.Size(111, 16);
            this.radioStopClient.TabIndex = 0;
            this.radioStopClient.TabStop = true;
            this.radioStopClient.Text = "클라이언트 중지";
            this.radioStopClient.UseVisualStyleBackColor = true;
            this.radioStopClient.CheckedChanged += new System.EventHandler(this.radioClient_CheckedChanged);
            // 
            // radioStartClient
            // 
            this.radioStartClient.AutoSize = true;
            this.radioStartClient.Checked = true;
            this.radioStartClient.Location = new System.Drawing.Point(21, 28);
            this.radioStartClient.Name = "radioStartClient";
            this.radioStartClient.Size = new System.Drawing.Size(111, 16);
            this.radioStartClient.TabIndex = 0;
            this.radioStartClient.TabStop = true;
            this.radioStartClient.Text = "클라이언트 시작";
            this.radioStartClient.UseVisualStyleBackColor = true;
            this.radioStartClient.CheckedChanged += new System.EventHandler(this.radioClient_CheckedChanged);
            // 
            // checkBoxServer
            // 
            this.checkBoxServer.AutoSize = true;
            this.checkBoxServer.Location = new System.Drawing.Point(25, 237);
            this.checkBoxServer.Name = "checkBoxServer";
            this.checkBoxServer.Size = new System.Drawing.Size(100, 16);
            this.checkBoxServer.TabIndex = 1;
            this.checkBoxServer.Text = "서버 업데이트";
            this.checkBoxServer.UseVisualStyleBackColor = true;
            // 
            // checkBoxClient
            // 
            this.checkBoxClient.AutoSize = true;
            this.checkBoxClient.Location = new System.Drawing.Point(131, 237);
            this.checkBoxClient.Name = "checkBoxClient";
            this.checkBoxClient.Size = new System.Drawing.Size(136, 16);
            this.checkBoxClient.TabIndex = 1;
            this.checkBoxClient.Text = "클라이언트 업데이트";
            this.checkBoxClient.UseVisualStyleBackColor = true;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(574, 230);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.TabIndex = 2;
            this.btnUpdate.Text = "업데이트";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // cboSite
            // 
            this.cboSite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSite.FormattingEnabled = true;
            this.cboSite.Items.AddRange(new object[] {
            "선택된 사이트 없음",
            "마산의료원",
            "경남대표도서관",
            "경상남도기록원",
            "경남문화예술회관",
            "NC파크마산구장",
            "성산아트홀",
            "진해문화센터",
            "문신미술관",
            "문신원형미술관",
            "창원시립마산박물관",
            "김해문화의전당",
            "칠암도서관",
            "장유도서관",
            "기적의도서관",
            "진영한빛도서관",
            "창원시",
            "김해시",
            "경상남도"});
            this.cboSite.Location = new System.Drawing.Point(25, 12);
            this.cboSite.Name = "cboSite";
            this.cboSite.Size = new System.Drawing.Size(144, 20);
            this.cboSite.TabIndex = 3;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(672, 270);
            this.Controls.Add(this.cboSite);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.checkBoxClient);
            this.Controls.Add(this.checkBoxServer);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormMain";
            this.Text = "SKT 디지털트윈 업데이트 관리";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelServerName;
        private System.Windows.Forms.Button btnServerUpdate;
        private System.Windows.Forms.TextBox textBoxServerName;
        private System.Windows.Forms.TextBox textBoxServerUpdate;
        private System.Windows.Forms.RadioButton radioUpdateServer;
        private System.Windows.Forms.RadioButton radioStopServer;
        private System.Windows.Forms.RadioButton radioStartServer;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelClientName;
        private System.Windows.Forms.Button btnClientUpdate;
        private System.Windows.Forms.TextBox textBoxClientName;
        private System.Windows.Forms.TextBox textBoxClientUpdate;
        private System.Windows.Forms.RadioButton radioUpdateClient;
        private System.Windows.Forms.RadioButton radioStopClient;
        private System.Windows.Forms.RadioButton radioStartClient;
        private System.Windows.Forms.CheckBox checkBoxServer;
        private System.Windows.Forms.CheckBox checkBoxClient;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.ComboBox cboSite;
    }
}

