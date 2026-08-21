namespace SensorTester
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
			this.m_cmbRecivers = new System.Windows.Forms.ComboBox();
			this.m_textBox1 = new System.Windows.Forms.TextBox();
			this.m_btnPoll = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// m_cmbRecivers
			// 
			this.m_cmbRecivers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_cmbRecivers.FormattingEnabled = true;
			this.m_cmbRecivers.Location = new System.Drawing.Point(31, 23);
			this.m_cmbRecivers.Name = "m_cmbRecivers";
			this.m_cmbRecivers.Size = new System.Drawing.Size(318, 20);
			this.m_cmbRecivers.TabIndex = 0;
			this.m_cmbRecivers.SelectedIndexChanged += new System.EventHandler(this.m_cmbRecivers_SelectedIndexChanged);
			this.m_cmbRecivers.SelectionChangeCommitted += new System.EventHandler(this.m_cmbRecivers_SelectionChangeCommitted);
			// 
			// m_textBox1
			// 
			this.m_textBox1.AcceptsReturn = true;
			this.m_textBox1.AcceptsTab = true;
			this.m_textBox1.Location = new System.Drawing.Point(31, 71);
			this.m_textBox1.Multiline = true;
			this.m_textBox1.Name = "m_textBox1";
			this.m_textBox1.Size = new System.Drawing.Size(420, 200);
			this.m_textBox1.TabIndex = 1;
			// 
			// m_btnPoll
			// 
			this.m_btnPoll.Location = new System.Drawing.Point(372, 23);
			this.m_btnPoll.Name = "m_btnPoll";
			this.m_btnPoll.Size = new System.Drawing.Size(79, 20);
			this.m_btnPoll.TabIndex = 2;
			this.m_btnPoll.Text = "확인";
			this.m_btnPoll.UseVisualStyleBackColor = true;
			this.m_btnPoll.Click += new System.EventHandler(this.button1_Click_1);
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(485, 292);
			this.Controls.Add(this.m_btnPoll);
			this.Controls.Add(this.m_textBox1);
			this.Controls.Add(this.m_cmbRecivers);
			this.Name = "FormMain";
			this.Text = "Sensor Tester";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.FormMain_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.ComboBox m_cmbRecivers;
		private System.Windows.Forms.TextBox m_textBox1;
		private System.Windows.Forms.Button m_btnPoll;

	}
}

