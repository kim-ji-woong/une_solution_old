namespace SOP1_CCTV_Tester
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
            this.cboCCTV = new System.Windows.Forms.ComboBox();
            this.panelCCTV = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // cboCCTV
            // 
            this.cboCCTV.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCCTV.FormattingEnabled = true;
            this.cboCCTV.Location = new System.Drawing.Point(316, 353);
            this.cboCCTV.Name = "cboCCTV";
            this.cboCCTV.Size = new System.Drawing.Size(289, 20);
            this.cboCCTV.TabIndex = 1;
            this.cboCCTV.SelectedIndexChanged += new System.EventHandler(this.cboCCTV_SelectedIndexChanged);
            // 
            // panelCCTV
            // 
            this.panelCCTV.BackColor = System.Drawing.Color.Black;
            this.panelCCTV.Location = new System.Drawing.Point(12, 12);
            this.panelCCTV.Name = "panelCCTV";
            this.panelCCTV.Size = new System.Drawing.Size(593, 335);
            this.panelCCTV.TabIndex = 2;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(617, 385);
            this.Controls.Add(this.panelCCTV);
            this.Controls.Add(this.cboCCTV);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormMain";
            this.Text = "CCTV 테스터";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboCCTV;
        private System.Windows.Forms.Panel panelCCTV;
    }
}

