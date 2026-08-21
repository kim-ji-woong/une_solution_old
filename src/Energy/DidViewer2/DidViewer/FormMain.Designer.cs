namespace DidViewer
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
            this.pnUI = new System.Windows.Forms.Panel();
            this.pnUIEmergency = new System.Windows.Forms.Panel();
            this.pnUITraning = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // pnUI
            // 
            this.pnUI.BackColor = System.Drawing.Color.White;
            this.pnUI.Location = new System.Drawing.Point(373, 117);
            this.pnUI.Name = "pnUI";
            this.pnUI.Size = new System.Drawing.Size(200, 100);
            this.pnUI.TabIndex = 0;
            // 
            // pnUIEmergency
            // 
            this.pnUIEmergency.BackColor = System.Drawing.Color.White;
            this.pnUIEmergency.Location = new System.Drawing.Point(373, 223);
            this.pnUIEmergency.Name = "pnUIEmergency";
            this.pnUIEmergency.Size = new System.Drawing.Size(200, 100);
            this.pnUIEmergency.TabIndex = 1;
            // 
            // pnUITraning
            // 
            this.pnUITraning.BackColor = System.Drawing.Color.White;
            this.pnUITraning.Location = new System.Drawing.Point(373, 329);
            this.pnUITraning.Name = "pnUITraning";
            this.pnUITraning.Size = new System.Drawing.Size(200, 100);
            this.pnUITraning.TabIndex = 2;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::DidViewer.Properties.Resources.BackgroundNormal;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pnUITraning);
            this.Controls.Add(this.pnUIEmergency);
            this.Controls.Add(this.pnUI);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnUI;
        private System.Windows.Forms.Panel pnUIEmergency;
        private System.Windows.Forms.Panel pnUITraning;
    }
}

