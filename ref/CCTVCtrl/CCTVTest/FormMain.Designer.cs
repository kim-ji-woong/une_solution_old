namespace UnECCTV
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelRight = new System.Windows.Forms.Panel();
            this.btnConnectChannel3 = new System.Windows.Forms.Button();
            this.btnConnectChannel2 = new System.Windows.Forms.Button();
            this.btnConnectChannel1 = new System.Windows.Forms.Button();
            this.labelCCTVID = new System.Windows.Forms.Label();
            this.labelCCTVName = new System.Windows.Forms.Label();
            this.btnShowCCTVList = new System.Windows.Forms.Button();
            this.panelCCTV4 = new UnECCTV.CCTVPanel();
            this.panelCCTV2 = new UnECCTV.CCTVPanel();
            this.panelCCTV5 = new UnECCTV.CCTVPanel();
            this.panelCCTV3 = new UnECCTV.CCTVPanel();
            this.panelCCTV6 = new UnECCTV.CCTVPanel();
            this.panelCCTV1 = new UnECCTV.CCTVPanel();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.panelRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.btnDisconnect);
            this.panelRight.Controls.Add(this.btnConnectChannel3);
            this.panelRight.Controls.Add(this.btnConnectChannel2);
            this.panelRight.Controls.Add(this.btnConnectChannel1);
            this.panelRight.Controls.Add(this.labelCCTVID);
            this.panelRight.Controls.Add(this.labelCCTVName);
            this.panelRight.Controls.Add(this.btnShowCCTVList);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(993, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(173, 521);
            this.panelRight.TabIndex = 3;
            // 
            // btnConnectChannel3
            // 
            this.btnConnectChannel3.Location = new System.Drawing.Point(31, 175);
            this.btnConnectChannel3.Name = "btnConnectChannel3";
            this.btnConnectChannel3.Size = new System.Drawing.Size(98, 23);
            this.btnConnectChannel3.TabIndex = 6;
            this.btnConnectChannel3.Text = "3번 채널 접속";
            this.btnConnectChannel3.UseVisualStyleBackColor = true;
            this.btnConnectChannel3.Click += new System.EventHandler(this.btnConnectChannel_Click);
            // 
            // btnConnectChannel2
            // 
            this.btnConnectChannel2.Location = new System.Drawing.Point(31, 146);
            this.btnConnectChannel2.Name = "btnConnectChannel2";
            this.btnConnectChannel2.Size = new System.Drawing.Size(98, 23);
            this.btnConnectChannel2.TabIndex = 6;
            this.btnConnectChannel2.Text = "2번 채널 접속";
            this.btnConnectChannel2.UseVisualStyleBackColor = true;
            this.btnConnectChannel2.Click += new System.EventHandler(this.btnConnectChannel_Click);
            // 
            // btnConnectChannel1
            // 
            this.btnConnectChannel1.Location = new System.Drawing.Point(31, 117);
            this.btnConnectChannel1.Name = "btnConnectChannel1";
            this.btnConnectChannel1.Size = new System.Drawing.Size(98, 23);
            this.btnConnectChannel1.TabIndex = 6;
            this.btnConnectChannel1.Text = "1번 채널 접속";
            this.btnConnectChannel1.UseVisualStyleBackColor = true;
            this.btnConnectChannel1.Click += new System.EventHandler(this.btnConnectChannel_Click);
            // 
            // labelCCTVID
            // 
            this.labelCCTVID.AutoSize = true;
            this.labelCCTVID.Location = new System.Drawing.Point(17, 64);
            this.labelCCTVID.Name = "labelCCTVID";
            this.labelCCTVID.Size = new System.Drawing.Size(54, 12);
            this.labelCCTVID.TabIndex = 5;
            this.labelCCTVID.Text = "CCTV ID";
            // 
            // labelCCTVName
            // 
            this.labelCCTVName.AutoSize = true;
            this.labelCCTVName.Location = new System.Drawing.Point(17, 90);
            this.labelCCTVName.Name = "labelCCTVName";
            this.labelCCTVName.Size = new System.Drawing.Size(77, 12);
            this.labelCCTVName.TabIndex = 1;
            this.labelCCTVName.Text = "CCTV Name";
            // 
            // btnShowCCTVList
            // 
            this.btnShowCCTVList.Location = new System.Drawing.Point(31, 12);
            this.btnShowCCTVList.Name = "btnShowCCTVList";
            this.btnShowCCTVList.Size = new System.Drawing.Size(107, 28);
            this.btnShowCCTVList.TabIndex = 0;
            this.btnShowCCTVList.Text = "CCTV 목록확인";
            this.btnShowCCTVList.UseVisualStyleBackColor = true;
            this.btnShowCCTVList.Click += new System.EventHandler(this.btnShowCCTVList_Click);
            // 
            // panelCCTV4
            // 
            this.panelCCTV4.BackColor = System.Drawing.Color.Black;
            this.panelCCTV4.IsSelected = false;
            this.panelCCTV4.Location = new System.Drawing.Point(12, 269);
            this.panelCCTV4.Name = "panelCCTV4";
            this.panelCCTV4.Size = new System.Drawing.Size(316, 240);
            this.panelCCTV4.TabIndex = 4;
            // 
            // panelCCTV2
            // 
            this.panelCCTV2.BackColor = System.Drawing.Color.Black;
            this.panelCCTV2.IsSelected = false;
            this.panelCCTV2.Location = new System.Drawing.Point(342, 12);
            this.panelCCTV2.Name = "panelCCTV2";
            this.panelCCTV2.Size = new System.Drawing.Size(316, 240);
            this.panelCCTV2.TabIndex = 4;
            // 
            // panelCCTV5
            // 
            this.panelCCTV5.BackColor = System.Drawing.Color.Black;
            this.panelCCTV5.IsSelected = false;
            this.panelCCTV5.Location = new System.Drawing.Point(342, 269);
            this.panelCCTV5.Name = "panelCCTV5";
            this.panelCCTV5.Size = new System.Drawing.Size(316, 240);
            this.panelCCTV5.TabIndex = 4;
            // 
            // panelCCTV3
            // 
            this.panelCCTV3.BackColor = System.Drawing.Color.Black;
            this.panelCCTV3.IsSelected = false;
            this.panelCCTV3.Location = new System.Drawing.Point(671, 12);
            this.panelCCTV3.Name = "panelCCTV3";
            this.panelCCTV3.Size = new System.Drawing.Size(316, 240);
            this.panelCCTV3.TabIndex = 4;
            // 
            // panelCCTV6
            // 
            this.panelCCTV6.BackColor = System.Drawing.Color.Black;
            this.panelCCTV6.IsSelected = false;
            this.panelCCTV6.Location = new System.Drawing.Point(671, 269);
            this.panelCCTV6.Name = "panelCCTV6";
            this.panelCCTV6.Size = new System.Drawing.Size(316, 240);
            this.panelCCTV6.TabIndex = 4;
            // 
            // panelCCTV1
            // 
            this.panelCCTV1.BackColor = System.Drawing.Color.Black;
            this.panelCCTV1.IsSelected = false;
            this.panelCCTV1.Location = new System.Drawing.Point(12, 12);
            this.panelCCTV1.Name = "panelCCTV1";
            this.panelCCTV1.Size = new System.Drawing.Size(316, 240);
            this.panelCCTV1.TabIndex = 4;
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(31, 204);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(98, 23);
            this.btnDisconnect.TabIndex = 7;
            this.btnDisconnect.Text = "접속 해제";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // FormMain
            // 
            this.ClientSize = new System.Drawing.Size(1166, 521);
            this.Controls.Add(this.panelCCTV6);
            this.Controls.Add(this.panelCCTV5);
            this.Controls.Add(this.panelCCTV1);
            this.Controls.Add(this.panelCCTV4);
            this.Controls.Add(this.panelCCTV3);
            this.Controls.Add(this.panelCCTV2);
            this.Controls.Add(this.panelRight);
            this.Name = "FormMain";
            this.Text = "CCTV Tester";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            this.panelRight.ResumeLayout(false);
            this.panelRight.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelRight;
        private UnECCTV.CCTVPanel panelCCTV4;
        private UnECCTV.CCTVPanel panelCCTV2;
        private UnECCTV.CCTVPanel panelCCTV5;
        private UnECCTV.CCTVPanel panelCCTV3;
        private UnECCTV.CCTVPanel panelCCTV6;
        private System.Windows.Forms.Button btnShowCCTVList;
        private System.Windows.Forms.Button btnConnectChannel3;
        private System.Windows.Forms.Button btnConnectChannel2;
        private System.Windows.Forms.Button btnConnectChannel1;
        private System.Windows.Forms.Label labelCCTVID;
        private System.Windows.Forms.Label labelCCTVName;
        private UnECCTV.CCTVPanel panelCCTV1;
        private System.Windows.Forms.Button btnDisconnect;
    }
}

