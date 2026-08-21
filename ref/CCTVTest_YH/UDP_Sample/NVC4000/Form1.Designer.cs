namespace NVC4000
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.axAxVCA1 = new AxAXVCALib.AxAxVCA();
            ((System.ComponentModel.ISupportInitialize)(this.axAxVCA1)).BeginInit();
            this.SuspendLayout();
            // 
            // axAxVCA1
            // 
            this.axAxVCA1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axAxVCA1.Enabled = true;
            this.axAxVCA1.Location = new System.Drawing.Point(0, 0);
            this.axAxVCA1.Name = "axAxVCA1";
            this.axAxVCA1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axAxVCA1.OcxState")));
            this.axAxVCA1.Size = new System.Drawing.Size(284, 261);
            this.axAxVCA1.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.axAxVCA1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.axAxVCA1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxAXVCALib.AxAxVCA axAxVCA1;


    }
}

