namespace DivisysCCTVInfo
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
            this.btnToRTSP = new System.Windows.Forms.Button();
            this.textBoxJson = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxRTSP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxPW = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnToRTSP
            // 
            this.btnToRTSP.Location = new System.Drawing.Point(416, 12);
            this.btnToRTSP.Name = "btnToRTSP";
            this.btnToRTSP.Size = new System.Drawing.Size(128, 23);
            this.btnToRTSP.TabIndex = 2;
            this.btnToRTSP.Text = "RTSP로 변환";
            this.btnToRTSP.UseVisualStyleBackColor = true;
            this.btnToRTSP.Click += new System.EventHandler(this.btnToRTSP_Click);
            // 
            // textBoxJson
            // 
            this.textBoxJson.Location = new System.Drawing.Point(12, 86);
            this.textBoxJson.Multiline = true;
            this.textBoxJson.Name = "textBoxJson";
            this.textBoxJson.Size = new System.Drawing.Size(372, 382);
            this.textBoxJson.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(12, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 21);
            this.label1.TabIndex = 4;
            this.label1.Text = "CCTV 목록(Json)";
            // 
            // textBoxRTSP
            // 
            this.textBoxRTSP.Location = new System.Drawing.Point(416, 86);
            this.textBoxRTSP.Multiline = true;
            this.textBoxRTSP.Name = "textBoxRTSP";
            this.textBoxRTSP.Size = new System.Drawing.Size(372, 382);
            this.textBoxRTSP.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(416, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(138, 21);
            this.label2.TabIndex = 4;
            this.label2.Text = "CCTV 목록(RTSP)";
            // 
            // textBoxID
            // 
            this.textBoxID.Location = new System.Drawing.Point(44, 11);
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.Size = new System.Drawing.Size(100, 21);
            this.textBoxID.TabIndex = 5;
            this.textBoxID.Text = "guest";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "ID :";
            // 
            // textBoxPW
            // 
            this.textBoxPW.Location = new System.Drawing.Point(197, 12);
            this.textBoxPW.Name = "textBoxPW";
            this.textBoxPW.Size = new System.Drawing.Size(100, 21);
            this.textBoxPW.TabIndex = 5;
            this.textBoxPW.Text = "guest";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(163, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "PW :";
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.ForeColor = System.Drawing.Color.Green;
            this.labelStatus.Location = new System.Drawing.Point(578, 17);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(91, 12);
            this.labelStatus.TabIndex = 8;
            this.labelStatus.Text = "CCTV 연결상태";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 491);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxPW);
            this.Controls.Add(this.textBoxID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxRTSP);
            this.Controls.Add(this.textBoxJson);
            this.Controls.Add(this.btnToRTSP);
            this.Name = "FormMain";
            this.Text = "Divisys NVR CCTV 리스트 얻어오기";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnToRTSP;
        private System.Windows.Forms.TextBox textBoxJson;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxRTSP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxPW;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelStatus;
    }
}

