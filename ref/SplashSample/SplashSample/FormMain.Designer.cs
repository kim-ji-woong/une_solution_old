namespace SplashSample
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
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.textBoxSeconds = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioEDisaster = new System.Windows.Forms.RadioButton();
            this.radioSmartDisaster = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioOrange = new System.Windows.Forms.RadioButton();
            this.radioGray = new System.Windows.Forms.RadioButton();
            this.radioBlue = new System.Windows.Forms.RadioButton();
            this.radioGreen = new System.Windows.Forms.RadioButton();
            this.radioDarkBlue = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // textBoxSeconds
            // 
            this.textBoxSeconds.Location = new System.Drawing.Point(120, 19);
            this.textBoxSeconds.Name = "textBoxSeconds";
            this.textBoxSeconds.Size = new System.Drawing.Size(39, 21);
            this.textBoxSeconds.TabIndex = 0;
            this.textBoxSeconds.Text = "10";
            this.textBoxSeconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Splash 운용 시간 :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(162, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "초";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(265, 215);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "시작";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioSmartDisaster);
            this.groupBox1.Controls.Add(this.radioEDisaster);
            this.groupBox1.Location = new System.Drawing.Point(13, 57);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(182, 90);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Text";
            // 
            // radioEDisaster
            // 
            this.radioEDisaster.AutoSize = true;
            this.radioEDisaster.Checked = true;
            this.radioEDisaster.Location = new System.Drawing.Point(14, 29);
            this.radioEDisaster.Name = "radioEDisaster";
            this.radioEDisaster.Size = new System.Drawing.Size(100, 16);
            this.radioEDisaster.TabIndex = 0;
            this.radioEDisaster.TabStop = true;
            this.radioEDisaster.Text = "e-재난 시스템";
            this.radioEDisaster.UseVisualStyleBackColor = true;
            // 
            // radioSmartDisaster
            // 
            this.radioSmartDisaster.AutoSize = true;
            this.radioSmartDisaster.Location = new System.Drawing.Point(14, 55);
            this.radioSmartDisaster.Name = "radioSmartDisaster";
            this.radioSmartDisaster.Size = new System.Drawing.Size(158, 16);
            this.radioSmartDisaster.TabIndex = 0;
            this.radioSmartDisaster.Text = "SMART 재난관리 시스템";
            this.radioSmartDisaster.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioGreen);
            this.groupBox2.Controls.Add(this.radioDarkBlue);
            this.groupBox2.Controls.Add(this.radioBlue);
            this.groupBox2.Controls.Add(this.radioOrange);
            this.groupBox2.Controls.Add(this.radioGray);
            this.groupBox2.Location = new System.Drawing.Point(12, 153);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(182, 90);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "색상";
            // 
            // radioOrange
            // 
            this.radioOrange.AutoSize = true;
            this.radioOrange.Location = new System.Drawing.Point(14, 55);
            this.radioOrange.Name = "radioOrange";
            this.radioOrange.Size = new System.Drawing.Size(47, 16);
            this.radioOrange.TabIndex = 0;
            this.radioOrange.Text = "주황";
            this.radioOrange.UseVisualStyleBackColor = true;
            // 
            // radioGray
            // 
            this.radioGray.AutoSize = true;
            this.radioGray.Checked = true;
            this.radioGray.Location = new System.Drawing.Point(14, 29);
            this.radioGray.Name = "radioGray";
            this.radioGray.Size = new System.Drawing.Size(47, 16);
            this.radioGray.TabIndex = 0;
            this.radioGray.TabStop = true;
            this.radioGray.Text = "회색";
            this.radioGray.UseVisualStyleBackColor = true;
            // 
            // radioBlue
            // 
            this.radioBlue.AutoSize = true;
            this.radioBlue.Location = new System.Drawing.Point(68, 29);
            this.radioBlue.Name = "radioBlue";
            this.radioBlue.Size = new System.Drawing.Size(47, 16);
            this.radioBlue.TabIndex = 0;
            this.radioBlue.Text = "파랑";
            this.radioBlue.UseVisualStyleBackColor = true;
            // 
            // radioGreen
            // 
            this.radioGreen.AutoSize = true;
            this.radioGreen.Location = new System.Drawing.Point(68, 55);
            this.radioGreen.Name = "radioGreen";
            this.radioGreen.Size = new System.Drawing.Size(47, 16);
            this.radioGreen.TabIndex = 0;
            this.radioGreen.Text = "초록";
            this.radioGreen.UseVisualStyleBackColor = true;
            // 
            // radioDarkBlue
            // 
            this.radioDarkBlue.AutoSize = true;
            this.radioDarkBlue.Location = new System.Drawing.Point(120, 29);
            this.radioDarkBlue.Name = "radioDarkBlue";
            this.radioDarkBlue.Size = new System.Drawing.Size(47, 16);
            this.radioDarkBlue.TabIndex = 0;
            this.radioDarkBlue.Text = "남색";
            this.radioDarkBlue.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 250);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxSeconds);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormMain";
            this.Text = "스플래쉬 예제";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox textBoxSeconds;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioSmartDisaster;
        private System.Windows.Forms.RadioButton radioEDisaster;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioGreen;
        private System.Windows.Forms.RadioButton radioDarkBlue;
        private System.Windows.Forms.RadioButton radioBlue;
        private System.Windows.Forms.RadioButton radioOrange;
        private System.Windows.Forms.RadioButton radioGray;
    }
}

