namespace AirQualityServer
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
            this.labelConnectionStatus = new System.Windows.Forms.Label();
            this.labelO2 = new System.Windows.Forms.Label();
            this.textBoxO2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxCO2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxCO = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxCH4 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxTemp = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxHumi = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelConnectionStatus
            // 
            this.labelConnectionStatus.AutoSize = true;
            this.labelConnectionStatus.Location = new System.Drawing.Point(31, 46);
            this.labelConnectionStatus.Name = "labelConnectionStatus";
            this.labelConnectionStatus.Size = new System.Drawing.Size(107, 12);
            this.labelConnectionStatus.TabIndex = 0;
            this.labelConnectionStatus.Text = "Jubix DB 연결상태";
            // 
            // labelO2
            // 
            this.labelO2.AutoSize = true;
            this.labelO2.Location = new System.Drawing.Point(31, 113);
            this.labelO2.Name = "labelO2";
            this.labelO2.Size = new System.Drawing.Size(37, 12);
            this.labelO2.TabIndex = 1;
            this.labelO2.Text = "산소 :";
            // 
            // textBoxO2
            // 
            this.textBoxO2.Location = new System.Drawing.Point(110, 108);
            this.textBoxO2.Name = "textBoxO2";
            this.textBoxO2.ReadOnly = true;
            this.textBoxO2.Size = new System.Drawing.Size(54, 21);
            this.textBoxO2.TabIndex = 2;
            this.textBoxO2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(170, 113);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "%";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 140);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "이산화탄소 :";
            // 
            // textBoxCO2
            // 
            this.textBoxCO2.Location = new System.Drawing.Point(110, 135);
            this.textBoxCO2.Name = "textBoxCO2";
            this.textBoxCO2.ReadOnly = true;
            this.textBoxCO2.Size = new System.Drawing.Size(54, 21);
            this.textBoxCO2.TabIndex = 2;
            this.textBoxCO2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(170, 140);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "ppm";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 167);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "일산화탄소 :";
            // 
            // textBoxCO
            // 
            this.textBoxCO.Location = new System.Drawing.Point(110, 162);
            this.textBoxCO.Name = "textBoxCO";
            this.textBoxCO.ReadOnly = true;
            this.textBoxCO.Size = new System.Drawing.Size(54, 21);
            this.textBoxCO.TabIndex = 2;
            this.textBoxCO.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(170, 167);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "ppm";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(31, 194);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "메탄 :";
            // 
            // textBoxCH4
            // 
            this.textBoxCH4.Location = new System.Drawing.Point(110, 189);
            this.textBoxCH4.Name = "textBoxCH4";
            this.textBoxCH4.ReadOnly = true;
            this.textBoxCH4.Size = new System.Drawing.Size(54, 21);
            this.textBoxCH4.TabIndex = 2;
            this.textBoxCH4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(170, 194);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(27, 12);
            this.label7.TabIndex = 3;
            this.label7.Text = "LEL";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(31, 221);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 12);
            this.label8.TabIndex = 1;
            this.label8.Text = "온도 :";
            // 
            // textBoxTemp
            // 
            this.textBoxTemp.Location = new System.Drawing.Point(110, 216);
            this.textBoxTemp.Name = "textBoxTemp";
            this.textBoxTemp.ReadOnly = true;
            this.textBoxTemp.Size = new System.Drawing.Size(54, 21);
            this.textBoxTemp.TabIndex = 2;
            this.textBoxTemp.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(170, 221);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(17, 12);
            this.label9.TabIndex = 3;
            this.label9.Text = "도";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(31, 248);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(37, 12);
            this.label10.TabIndex = 1;
            this.label10.Text = "습도 :";
            // 
            // textBoxHumi
            // 
            this.textBoxHumi.Location = new System.Drawing.Point(110, 243);
            this.textBoxHumi.Name = "textBoxHumi";
            this.textBoxHumi.ReadOnly = true;
            this.textBoxHumi.Size = new System.Drawing.Size(54, 21);
            this.textBoxHumi.TabIndex = 2;
            this.textBoxHumi.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(170, 248);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(15, 12);
            this.label11.TabIndex = 3;
            this.label11.Text = "%";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(217, 294);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxHumi);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.textBoxTemp);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBoxCH4);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxCO);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxCO2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxO2);
            this.Controls.Add(this.labelO2);
            this.Controls.Add(this.labelConnectionStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormMain";
            this.Text = "실내공기질 센서 서버";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelConnectionStatus;
        private System.Windows.Forms.Label labelO2;
        private System.Windows.Forms.TextBox textBoxO2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxCO2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxCO;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxCH4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxTemp;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxHumi;
        private System.Windows.Forms.Label label11;
    }
}

