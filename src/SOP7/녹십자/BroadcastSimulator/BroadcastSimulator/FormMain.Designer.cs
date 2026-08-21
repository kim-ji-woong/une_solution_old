
namespace BroadcastSimulator
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
            this.textBoxIP1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxPort1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.radio1 = new System.Windows.Forms.RadioButton();
            this.textBoxIP2 = new System.Windows.Forms.TextBox();
            this.textBoxPort2 = new System.Windows.Forms.TextBox();
            this.radio2 = new System.Windows.Forms.RadioButton();
            this.textBoxIP3 = new System.Windows.Forms.TextBox();
            this.textBoxPort3 = new System.Windows.Forms.TextBox();
            this.radio3 = new System.Windows.Forms.RadioButton();
            this.textBoxIP4 = new System.Windows.Forms.TextBox();
            this.textBoxPort4 = new System.Windows.Forms.TextBox();
            this.radio4 = new System.Windows.Forms.RadioButton();
            this.textBoxIP5 = new System.Windows.Forms.TextBox();
            this.textBoxPort5 = new System.Windows.Forms.TextBox();
            this.radio5 = new System.Windows.Forms.RadioButton();
            this.cboChannel = new System.Windows.Forms.ComboBox();
            this.cboMode = new System.Windows.Forms.ComboBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.labelConnection = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxIP1
            // 
            this.textBoxIP1.Location = new System.Drawing.Point(155, 55);
            this.textBoxIP1.Name = "textBoxIP1";
            this.textBoxIP1.Size = new System.Drawing.Size(100, 21);
            this.textBoxIP1.TabIndex = 1;
            this.textBoxIP1.Text = "127.0.0.1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(189, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "IP";
            // 
            // textBoxPort1
            // 
            this.textBoxPort1.Location = new System.Drawing.Point(270, 55);
            this.textBoxPort1.Name = "textBoxPort1";
            this.textBoxPort1.Size = new System.Drawing.Size(44, 21);
            this.textBoxPort1.TabIndex = 1;
            this.textBoxPort1.Text = "13000";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(276, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Port";
            // 
            // radio1
            // 
            this.radio1.AutoSize = true;
            this.radio1.Location = new System.Drawing.Point(13, 58);
            this.radio1.Name = "radio1";
            this.radio1.Size = new System.Drawing.Size(121, 16);
            this.radio1.TabIndex = 2;
            this.radio1.TabStop = true;
            this.radio1.Text = "1. QM관 방송장비";
            this.radio1.UseVisualStyleBackColor = true;
            // 
            // textBoxIP2
            // 
            this.textBoxIP2.Location = new System.Drawing.Point(155, 82);
            this.textBoxIP2.Name = "textBoxIP2";
            this.textBoxIP2.Size = new System.Drawing.Size(100, 21);
            this.textBoxIP2.TabIndex = 1;
            this.textBoxIP2.Text = "127.0.0.1";
            // 
            // textBoxPort2
            // 
            this.textBoxPort2.Location = new System.Drawing.Point(270, 82);
            this.textBoxPort2.Name = "textBoxPort2";
            this.textBoxPort2.Size = new System.Drawing.Size(44, 21);
            this.textBoxPort2.TabIndex = 1;
            this.textBoxPort2.Text = "13000";
            // 
            // radio2
            // 
            this.radio2.AutoSize = true;
            this.radio2.Location = new System.Drawing.Point(13, 85);
            this.radio2.Name = "radio2";
            this.radio2.Size = new System.Drawing.Size(133, 16);
            this.radio2.TabIndex = 2;
            this.radio2.TabStop = true;
            this.radio2.Text = "2. W&&FF관 방송장비";
            this.radio2.UseVisualStyleBackColor = true;
            // 
            // textBoxIP3
            // 
            this.textBoxIP3.Location = new System.Drawing.Point(155, 109);
            this.textBoxIP3.Name = "textBoxIP3";
            this.textBoxIP3.Size = new System.Drawing.Size(100, 21);
            this.textBoxIP3.TabIndex = 1;
            this.textBoxIP3.Text = "127.0.0.1";
            // 
            // textBoxPort3
            // 
            this.textBoxPort3.Location = new System.Drawing.Point(270, 109);
            this.textBoxPort3.Name = "textBoxPort3";
            this.textBoxPort3.Size = new System.Drawing.Size(44, 21);
            this.textBoxPort3.TabIndex = 1;
            this.textBoxPort3.Text = "13000";
            // 
            // radio3
            // 
            this.radio3.AutoSize = true;
            this.radio3.Location = new System.Drawing.Point(13, 112);
            this.radio3.Name = "radio3";
            this.radio3.Size = new System.Drawing.Size(137, 16);
            this.radio3.TabIndex = 2;
            this.radio3.TabStop = true;
            this.radio3.Text = "3. Admin관 방송장비";
            this.radio3.UseVisualStyleBackColor = true;
            // 
            // textBoxIP4
            // 
            this.textBoxIP4.Location = new System.Drawing.Point(155, 136);
            this.textBoxIP4.Name = "textBoxIP4";
            this.textBoxIP4.Size = new System.Drawing.Size(100, 21);
            this.textBoxIP4.TabIndex = 1;
            this.textBoxIP4.Text = "127.0.0.1";
            // 
            // textBoxPort4
            // 
            this.textBoxPort4.Location = new System.Drawing.Point(270, 136);
            this.textBoxPort4.Name = "textBoxPort4";
            this.textBoxPort4.Size = new System.Drawing.Size(44, 21);
            this.textBoxPort4.TabIndex = 1;
            this.textBoxPort4.Text = "13000";
            // 
            // radio4
            // 
            this.radio4.AutoSize = true;
            this.radio4.Location = new System.Drawing.Point(13, 139);
            this.radio4.Name = "radio4";
            this.radio4.Size = new System.Drawing.Size(123, 16);
            this.radio4.TabIndex = 2;
            this.radio4.TabStop = true;
            this.radio4.Text = "4. PD2관 방송장비";
            this.radio4.UseVisualStyleBackColor = true;
            // 
            // textBoxIP5
            // 
            this.textBoxIP5.Location = new System.Drawing.Point(155, 163);
            this.textBoxIP5.Name = "textBoxIP5";
            this.textBoxIP5.Size = new System.Drawing.Size(100, 21);
            this.textBoxIP5.TabIndex = 1;
            this.textBoxIP5.Text = "127.0.0.1";
            // 
            // textBoxPort5
            // 
            this.textBoxPort5.Location = new System.Drawing.Point(270, 163);
            this.textBoxPort5.Name = "textBoxPort5";
            this.textBoxPort5.Size = new System.Drawing.Size(44, 21);
            this.textBoxPort5.TabIndex = 1;
            this.textBoxPort5.Text = "13000";
            // 
            // radio5
            // 
            this.radio5.AutoSize = true;
            this.radio5.Location = new System.Drawing.Point(13, 166);
            this.radio5.Name = "radio5";
            this.radio5.Size = new System.Drawing.Size(123, 16);
            this.radio5.TabIndex = 2;
            this.radio5.TabStop = true;
            this.radio5.Text = "5. RP2관 방송장비";
            this.radio5.UseVisualStyleBackColor = true;
            // 
            // cboChannel
            // 
            this.cboChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboChannel.FormattingEnabled = true;
            this.cboChannel.Items.AddRange(new object[] {
            "1번 채널",
            "2번 채널",
            "3번 채널",
            "4번 채널",
            "5번 채널",
            "6번 채널",
            "7번 채널",
            "8번 채널",
            "9번 채널",
            "10번 채널"});
            this.cboChannel.Location = new System.Drawing.Point(25, 225);
            this.cboChannel.Name = "cboChannel";
            this.cboChannel.Size = new System.Drawing.Size(121, 20);
            this.cboChannel.TabIndex = 4;
            // 
            // cboMode
            // 
            this.cboMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMode.FormattingEnabled = true;
            this.cboMode.Items.AddRange(new object[] {
            "방송 On",
            "방송 Off"});
            this.cboMode.Location = new System.Drawing.Point(155, 225);
            this.cboMode.Name = "cboMode";
            this.cboMode.Size = new System.Drawing.Size(78, 20);
            this.cboMode.TabIndex = 4;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(249, 223);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(65, 23);
            this.btnSend.TabIndex = 5;
            this.btnSend.Text = "전송";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // labelConnection
            // 
            this.labelConnection.AutoSize = true;
            this.labelConnection.Location = new System.Drawing.Point(32, 199);
            this.labelConnection.Name = "labelConnection";
            this.labelConnection.Size = new System.Drawing.Size(133, 12);
            this.labelConnection.TabIndex = 6;
            this.labelConnection.Text = "접속된 클라이언트 없음";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 269);
            this.Controls.Add(this.labelConnection);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.cboMode);
            this.Controls.Add(this.cboChannel);
            this.Controls.Add(this.radio5);
            this.Controls.Add(this.radio4);
            this.Controls.Add(this.radio3);
            this.Controls.Add(this.radio2);
            this.Controls.Add(this.radio1);
            this.Controls.Add(this.textBoxPort5);
            this.Controls.Add(this.textBoxPort4);
            this.Controls.Add(this.textBoxPort3);
            this.Controls.Add(this.textBoxPort2);
            this.Controls.Add(this.textBoxPort1);
            this.Controls.Add(this.textBoxIP5);
            this.Controls.Add(this.textBoxIP4);
            this.Controls.Add(this.textBoxIP3);
            this.Controls.Add(this.textBoxIP2);
            this.Controls.Add(this.textBoxIP1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormMain";
            this.Text = "녹십자 방송 시뮬레이터";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxIP1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxPort1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radio1;
        private System.Windows.Forms.TextBox textBoxIP2;
        private System.Windows.Forms.TextBox textBoxPort2;
        private System.Windows.Forms.RadioButton radio2;
        private System.Windows.Forms.TextBox textBoxIP3;
        private System.Windows.Forms.TextBox textBoxPort3;
        private System.Windows.Forms.RadioButton radio3;
        private System.Windows.Forms.TextBox textBoxIP4;
        private System.Windows.Forms.TextBox textBoxPort4;
        private System.Windows.Forms.RadioButton radio4;
        private System.Windows.Forms.TextBox textBoxIP5;
        private System.Windows.Forms.TextBox textBoxPort5;
        private System.Windows.Forms.RadioButton radio5;
        private System.Windows.Forms.ComboBox cboChannel;
        private System.Windows.Forms.ComboBox cboMode;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Label labelConnection;
    }
}

