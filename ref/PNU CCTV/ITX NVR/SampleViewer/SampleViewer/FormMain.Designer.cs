namespace SampleViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.btnShowCCTVList = new System.Windows.Forms.Button();
            this.axitxview1 = new AxitxviewLib.Axitxview();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.axitxview1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnShowCCTVList
            // 
            this.btnShowCCTVList.Location = new System.Drawing.Point(389, 318);
            this.btnShowCCTVList.Name = "btnShowCCTVList";
            this.btnShowCCTVList.Size = new System.Drawing.Size(75, 23);
            this.btnShowCCTVList.TabIndex = 1;
            this.btnShowCCTVList.Text = "List 보기";
            this.btnShowCCTVList.UseVisualStyleBackColor = true;
            this.btnShowCCTVList.Click += new System.EventHandler(this.btnShowCCTVList_Click);
            // 
            // axitxview1
            // 
            this.axitxview1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.axitxview1.Enabled = true;
            this.axitxview1.Location = new System.Drawing.Point(0, 0);
            this.axitxview1.Name = "axitxview1";
            this.axitxview1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axitxview1.OcxState")));
            this.axitxview1.Size = new System.Drawing.Size(476, 312);
            this.axitxview1.TabIndex = 2;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(476, 352);
            this.Controls.Add(this.axitxview1);
            this.Controls.Add(this.btnShowCCTVList);
            this.Name = "FormMain";
            this.Text = "ITX NVR Viewer";
            this.ResizeEnd += new System.EventHandler(this.FormMain_ResizeEnd);
            ((System.ComponentModel.ISupportInitialize)(this.axitxview1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnShowCCTVList;
        private AxitxviewLib.Axitxview axitxview1;
        private System.Windows.Forms.Timer timer1;
    }
}

