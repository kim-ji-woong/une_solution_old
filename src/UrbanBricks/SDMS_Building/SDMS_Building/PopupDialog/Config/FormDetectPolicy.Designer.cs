namespace SDMS_Building.PopupDialog.Config
{
    partial class FormDetectPolicy
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.eleSignal = new System.Windows.Forms.Integration.ElementHost();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSignal = new UnE.GUI.ImageButton();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.eleTime = new System.Windows.Forms.Integration.ElementHost();
            this.eleDetectPolicy = new System.Windows.Forms.Integration.ElementHost();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnGoOutside = new UnE.GUI.ImageButton();
            this.label9 = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSignal)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnGoOutside)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.BackgroundImage = global::SDMS_Building.Properties.Resources.pnBox;
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.eleSignal);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.btnSignal);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Location = new System.Drawing.Point(20, 184);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(920, 134);
            this.panel2.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("나눔바른고딕", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(34, 51);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 27);
            this.label6.TabIndex = 25;
            this.label6.Text = "수  신";
            // 
            // eleSignal
            // 
            this.eleSignal.BackColor = System.Drawing.Color.White;
            this.eleSignal.Location = new System.Drawing.Point(185, 60);
            this.eleSignal.Name = "eleSignal";
            this.eleSignal.Size = new System.Drawing.Size(420, 50);
            this.eleSignal.TabIndex = 23;
            this.eleSignal.Text = "elementHost2";
            this.eleSignal.Child = null;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(664, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 23);
            this.label4.TabIndex = 35;
            this.label4.Text = "수신";
            // 
            // btnSignal
            // 
            this.btnSignal.ButtonText = "";
            this.btnSignal.ImageClicked = global::SDMS_Building.Properties.Resources.check_Checked;
            this.btnSignal.ImageDisabled = null;
            this.btnSignal.ImageMouseOver = global::SDMS_Building.Properties.Resources.check_Hover;
            this.btnSignal.ImageNormal = global::SDMS_Building.Properties.Resources.check_UnChecked;
            this.btnSignal.Location = new System.Drawing.Point(627, 71);
            this.btnSignal.Name = "btnSignal";
            this.btnSignal.Owner = null;
            this.btnSignal.Size = new System.Drawing.Size(30, 30);
            this.btnSignal.TabIndex = 34;
            this.btnSignal.TabStop = false;
            this.btnSignal.TextColor = System.Drawing.Color.Black;
            this.btnSignal.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSignal.ToolTipText = "";
            this.btnSignal.UseToolTip = false;
            this.btnSignal.WindowRateWidth = 1F;
            this.btnSignal.Click += new System.EventHandler(this.btnSignal_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(174, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(458, 23);
            this.label3.TabIndex = 20;
            this.label3.Text = "센서 종류에 따라 신호 처리 여부를 결정하는 기능입니다.";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImage = global::SDMS_Building.Properties.Resources.pnBox;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.eleTime);
            this.panel1.Controls.Add(this.eleDetectPolicy);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(20, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(920, 134);
            this.panel1.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("나눔바른고딕", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(34, 53);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 27);
            this.label5.TabIndex = 24;
            this.label5.Text = "탐  지";
            // 
            // eleTime
            // 
            this.eleTime.BackColor = System.Drawing.Color.White;
            this.eleTime.Location = new System.Drawing.Point(750, 60);
            this.eleTime.Name = "eleTime";
            this.eleTime.Size = new System.Drawing.Size(150, 50);
            this.eleTime.TabIndex = 23;
            this.eleTime.Text = "elementHost1";
            this.eleTime.Child = null;
            // 
            // eleDetectPolicy
            // 
            this.eleDetectPolicy.BackColor = System.Drawing.Color.White;
            this.eleDetectPolicy.Location = new System.Drawing.Point(452, 60);
            this.eleDetectPolicy.Name = "eleDetectPolicy";
            this.eleDetectPolicy.Size = new System.Drawing.Size(288, 50);
            this.eleDetectPolicy.TabIndex = 22;
            this.eleDetectPolicy.Text = "elementHost1";
            this.eleDetectPolicy.Child = null;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("나눔바른고딕", 15.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(174, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(272, 24);
            this.label1.TabIndex = 19;
            this.label1.Text = "오작동 처리된 센서의 탐지값을";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(174, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(490, 23);
            this.label2.TabIndex = 18;
            this.label2.Text = "반복적으로 들어오는 오작동 값을 처리하기 위한 기능입니다.";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Transparent;
            this.panel3.BackgroundImage = global::SDMS_Building.Properties.Resources.pnBox;
            this.panel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.label8);
            this.panel3.Controls.Add(this.btnGoOutside);
            this.panel3.Controls.Add(this.label9);
            this.panel3.Location = new System.Drawing.Point(20, 344);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(920, 134);
            this.panel3.TabIndex = 36;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("나눔바른고딕", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(34, 52);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 27);
            this.label7.TabIndex = 25;
            this.label7.Text = "종  료";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(222, 75);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(162, 23);
            this.label8.TabIndex = 35;
            this.label8.Text = "외부 화면으로 이동";
            // 
            // btnGoOutside
            // 
            this.btnGoOutside.ButtonText = "";
            this.btnGoOutside.ImageClicked = global::SDMS_Building.Properties.Resources.check_Checked;
            this.btnGoOutside.ImageDisabled = null;
            this.btnGoOutside.ImageMouseOver = global::SDMS_Building.Properties.Resources.check_Hover;
            this.btnGoOutside.ImageNormal = global::SDMS_Building.Properties.Resources.check_UnChecked;
            this.btnGoOutside.Location = new System.Drawing.Point(185, 71);
            this.btnGoOutside.Name = "btnGoOutside";
            this.btnGoOutside.Owner = null;
            this.btnGoOutside.Size = new System.Drawing.Size(30, 30);
            this.btnGoOutside.TabIndex = 34;
            this.btnGoOutside.TabStop = false;
            this.btnGoOutside.TextColor = System.Drawing.Color.Black;
            this.btnGoOutside.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnGoOutside.ToolTipText = "";
            this.btnGoOutside.UseToolTip = false;
            this.btnGoOutside.WindowRateWidth = 1F;
            this.btnGoOutside.Click += new System.EventHandler(this.btnGoOutside_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(174, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(454, 23);
            this.label9.TabIndex = 20;
            this.label9.Text = "신호 종료시 외부화면 이동 여부를 결정하는 기능입니다.";
            // 
            // FormDetectPolicy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(960, 500);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormDetectPolicy";
            this.ShowInTaskbar = false;
            this.Text = "FormDetectPolicy";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSignal)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnGoOutside)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private UnE.GUI.ImageButton btnSignal;
        private System.Windows.Forms.Integration.ElementHost eleDetectPolicy;
        private System.Windows.Forms.Integration.ElementHost eleSignal;
        private System.Windows.Forms.Integration.ElementHost eleTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private UnE.GUI.ImageButton btnGoOutside;
        private System.Windows.Forms.Label label9;
    }
}