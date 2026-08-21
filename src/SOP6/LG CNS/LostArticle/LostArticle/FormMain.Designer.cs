namespace LostArticle
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxDeadCount = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.labelDeadCountUnit = new System.Windows.Forms.Label();
            this.labelPrevDeadCount = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelInjuryUnit = new System.Windows.Forms.Label();
            this.labelPrevInjuryCount = new System.Windows.Forms.Label();
            this.textBoxInjuryCount = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.labelLostUnit = new System.Windows.Forms.Label();
            this.labelPrevLostCount = new System.Windows.Forms.Label();
            this.textBoxLostCount = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.labelTankTemperatureUnit = new System.Windows.Forms.Label();
            this.labelPrevTankTemperature = new System.Windows.Forms.Label();
            this.textBoxTankTemperature = new System.Windows.Forms.TextBox();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxBody = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnApplyStatus = new System.Windows.Forms.Button();
            this.btnInitialize = new System.Windows.Forms.Button();
            this.btnSendArticle = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(26, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "사망자 :";
            // 
            // textBoxDeadCount
            // 
            this.textBoxDeadCount.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxDeadCount.Location = new System.Drawing.Point(106, 59);
            this.textBoxDeadCount.Name = "textBoxDeadCount";
            this.textBoxDeadCount.Size = new System.Drawing.Size(39, 26);
            this.textBoxDeadCount.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(183, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 19);
            this.label2.TabIndex = 0;
            this.label2.Text = "이전값";
            // 
            // labelDeadCountUnit
            // 
            this.labelDeadCountUnit.AutoSize = true;
            this.labelDeadCountUnit.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelDeadCountUnit.Location = new System.Drawing.Point(149, 62);
            this.labelDeadCountUnit.Name = "labelDeadCountUnit";
            this.labelDeadCountUnit.Size = new System.Drawing.Size(23, 19);
            this.labelDeadCountUnit.TabIndex = 0;
            this.labelDeadCountUnit.Text = "명";
            // 
            // labelPrevDeadCount
            // 
            this.labelPrevDeadCount.AutoSize = true;
            this.labelPrevDeadCount.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelPrevDeadCount.Location = new System.Drawing.Point(183, 62);
            this.labelPrevDeadCount.Name = "labelPrevDeadCount";
            this.labelPrevDeadCount.Size = new System.Drawing.Size(18, 19);
            this.labelPrevDeadCount.TabIndex = 0;
            this.labelPrevDeadCount.Text = "-";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(26, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 19);
            this.label4.TabIndex = 0;
            this.label4.Text = "부상자 :";
            // 
            // labelInjuryUnit
            // 
            this.labelInjuryUnit.AutoSize = true;
            this.labelInjuryUnit.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelInjuryUnit.Location = new System.Drawing.Point(149, 94);
            this.labelInjuryUnit.Name = "labelInjuryUnit";
            this.labelInjuryUnit.Size = new System.Drawing.Size(23, 19);
            this.labelInjuryUnit.TabIndex = 0;
            this.labelInjuryUnit.Text = "명";
            // 
            // labelPrevInjuryCount
            // 
            this.labelPrevInjuryCount.AutoSize = true;
            this.labelPrevInjuryCount.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelPrevInjuryCount.Location = new System.Drawing.Point(183, 94);
            this.labelPrevInjuryCount.Name = "labelPrevInjuryCount";
            this.labelPrevInjuryCount.Size = new System.Drawing.Size(18, 19);
            this.labelPrevInjuryCount.TabIndex = 0;
            this.labelPrevInjuryCount.Text = "-";
            // 
            // textBoxInjuryCount
            // 
            this.textBoxInjuryCount.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxInjuryCount.Location = new System.Drawing.Point(106, 91);
            this.textBoxInjuryCount.Name = "textBoxInjuryCount";
            this.textBoxInjuryCount.Size = new System.Drawing.Size(39, 26);
            this.textBoxInjuryCount.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.Location = new System.Drawing.Point(262, 62);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 19);
            this.label7.TabIndex = 0;
            this.label7.Text = "실종자 :";
            // 
            // labelLostUnit
            // 
            this.labelLostUnit.AutoSize = true;
            this.labelLostUnit.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelLostUnit.Location = new System.Drawing.Point(385, 62);
            this.labelLostUnit.Name = "labelLostUnit";
            this.labelLostUnit.Size = new System.Drawing.Size(23, 19);
            this.labelLostUnit.TabIndex = 0;
            this.labelLostUnit.Text = "명";
            // 
            // labelPrevLostCount
            // 
            this.labelPrevLostCount.AutoSize = true;
            this.labelPrevLostCount.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelPrevLostCount.Location = new System.Drawing.Point(419, 62);
            this.labelPrevLostCount.Name = "labelPrevLostCount";
            this.labelPrevLostCount.Size = new System.Drawing.Size(18, 19);
            this.labelPrevLostCount.TabIndex = 0;
            this.labelPrevLostCount.Text = "-";
            // 
            // textBoxLostCount
            // 
            this.textBoxLostCount.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxLostCount.Location = new System.Drawing.Point(342, 59);
            this.textBoxLostCount.Name = "textBoxLostCount";
            this.textBoxLostCount.Size = new System.Drawing.Size(39, 26);
            this.textBoxLostCount.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label10.Location = new System.Drawing.Point(262, 94);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(74, 19);
            this.label10.TabIndex = 0;
            this.label10.Text = "탱크온도 :";
            // 
            // labelTankTemperatureUnit
            // 
            this.labelTankTemperatureUnit.AutoSize = true;
            this.labelTankTemperatureUnit.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTankTemperatureUnit.Location = new System.Drawing.Point(384, 94);
            this.labelTankTemperatureUnit.Name = "labelTankTemperatureUnit";
            this.labelTankTemperatureUnit.Size = new System.Drawing.Size(26, 19);
            this.labelTankTemperatureUnit.TabIndex = 0;
            this.labelTankTemperatureUnit.Text = "°C";
            // 
            // labelPrevTankTemperature
            // 
            this.labelPrevTankTemperature.AutoSize = true;
            this.labelPrevTankTemperature.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelPrevTankTemperature.Location = new System.Drawing.Point(419, 94);
            this.labelPrevTankTemperature.Name = "labelPrevTankTemperature";
            this.labelPrevTankTemperature.Size = new System.Drawing.Size(18, 19);
            this.labelPrevTankTemperature.TabIndex = 0;
            this.labelPrevTankTemperature.Text = "-";
            // 
            // textBoxTankTemperature
            // 
            this.textBoxTankTemperature.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxTankTemperature.Location = new System.Drawing.Point(342, 91);
            this.textBoxTankTemperature.Name = "textBoxTankTemperature";
            this.textBoxTankTemperature.Size = new System.Drawing.Size(39, 26);
            this.textBoxTankTemperature.TabIndex = 3;
            // 
            // textBoxTitle
            // 
            this.textBoxTitle.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxTitle.Location = new System.Drawing.Point(30, 211);
            this.textBoxTitle.Name = "textBoxTitle";
            this.textBoxTitle.Size = new System.Drawing.Size(453, 26);
            this.textBoxTitle.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.Location = new System.Drawing.Point(26, 188);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(107, 19);
            this.label6.TabIndex = 0;
            this.label6.Text = "제목(필수입력)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label9.Location = new System.Drawing.Point(26, 246);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 19);
            this.label9.TabIndex = 0;
            this.label9.Text = "본문";
            // 
            // textBoxBody
            // 
            this.textBoxBody.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxBody.Location = new System.Drawing.Point(30, 268);
            this.textBoxBody.Multiline = true;
            this.textBoxBody.Name = "textBoxBody";
            this.textBoxBody.Size = new System.Drawing.Size(453, 168);
            this.textBoxBody.TabIndex = 5;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label12.Location = new System.Drawing.Point(416, 36);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(51, 19);
            this.label12.TabIndex = 0;
            this.label12.Text = "이전값";
            // 
            // btnApplyStatus
            // 
            this.btnApplyStatus.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnApplyStatus.Location = new System.Drawing.Point(30, 135);
            this.btnApplyStatus.Name = "btnApplyStatus";
            this.btnApplyStatus.Size = new System.Drawing.Size(191, 31);
            this.btnApplyStatus.TabIndex = 3;
            this.btnApplyStatus.Text = "인명피해 및 탱크정보 전송";
            this.btnApplyStatus.UseVisualStyleBackColor = true;
            this.btnApplyStatus.Click += new System.EventHandler(this.btnApplyStatus_Click);
            // 
            // btnInitialize
            // 
            this.btnInitialize.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnInitialize.Location = new System.Drawing.Point(240, 452);
            this.btnInitialize.Name = "btnInitialize";
            this.btnInitialize.Size = new System.Drawing.Size(62, 31);
            this.btnInitialize.TabIndex = 3;
            this.btnInitialize.Text = "초기화";
            this.btnInitialize.UseVisualStyleBackColor = true;
            this.btnInitialize.Click += new System.EventHandler(this.btnInitialize_Click);
            // 
            // btnSendArticle
            // 
            this.btnSendArticle.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSendArticle.Location = new System.Drawing.Point(30, 452);
            this.btnSendArticle.Name = "btnSendArticle";
            this.btnSendArticle.Size = new System.Drawing.Size(191, 31);
            this.btnSendArticle.TabIndex = 3;
            this.btnSendArticle.Text = "메시지 전송";
            this.btnSendArticle.UseVisualStyleBackColor = true;
            this.btnSendArticle.Click += new System.EventHandler(this.btnSendArticle_Click);
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.Location = new System.Drawing.Point(421, 452);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(62, 31);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "닫기";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 511);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnInitialize);
            this.Controls.Add(this.btnSendArticle);
            this.Controls.Add(this.btnApplyStatus);
            this.Controls.Add(this.textBoxBody);
            this.Controls.Add(this.textBoxTitle);
            this.Controls.Add(this.textBoxTankTemperature);
            this.Controls.Add(this.labelPrevTankTemperature);
            this.Controls.Add(this.textBoxLostCount);
            this.Controls.Add(this.labelPrevLostCount);
            this.Controls.Add(this.textBoxInjuryCount);
            this.Controls.Add(this.labelTankTemperatureUnit);
            this.Controls.Add(this.labelPrevInjuryCount);
            this.Controls.Add(this.labelLostUnit);
            this.Controls.Add(this.textBoxDeadCount);
            this.Controls.Add(this.labelInjuryUnit);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.labelPrevDeadCount);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.labelDeadCountUnit);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormMain";
            this.Text = "피해현황 입력창";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxDeadCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelDeadCountUnit;
        private System.Windows.Forms.Label labelPrevDeadCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelInjuryUnit;
        private System.Windows.Forms.Label labelPrevInjuryCount;
        private System.Windows.Forms.TextBox textBoxInjuryCount;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label labelLostUnit;
        private System.Windows.Forms.Label labelPrevLostCount;
        private System.Windows.Forms.TextBox textBoxLostCount;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label labelTankTemperatureUnit;
        private System.Windows.Forms.Label labelPrevTankTemperature;
        private System.Windows.Forms.TextBox textBoxTankTemperature;
        private System.Windows.Forms.TextBox textBoxTitle;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxBody;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnApplyStatus;
        private System.Windows.Forms.Button btnInitialize;
        private System.Windows.Forms.Button btnSendArticle;
        private System.Windows.Forms.Button btnClose;
    }
}

