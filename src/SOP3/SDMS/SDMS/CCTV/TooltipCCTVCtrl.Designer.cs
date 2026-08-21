namespace SDMS
{
    partial class TooltipCCTVCtrl
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

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TooltipCCTVCtrl));
            this.labelManager = new System.Windows.Forms.Label();
            this.checkBoxFix = new System.Windows.Forms.CheckBox();
            this.axxpressStrm1 = new AxxpressStrmLib.AxxpressStrm();
            ((System.ComponentModel.ISupportInitialize)(this.axxpressStrm1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelManager
            // 
            this.labelManager.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelManager.AutoSize = true;
            this.labelManager.Location = new System.Drawing.Point(14, 270);
            this.labelManager.Name = "labelManager";
            this.labelManager.Size = new System.Drawing.Size(49, 12);
            this.labelManager.TabIndex = 6;
            this.labelManager.Text = "담당자 :";
            // 
            // checkBoxFix
            // 
            this.checkBoxFix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxFix.AutoSize = true;
            this.checkBoxFix.Location = new System.Drawing.Point(199, 248);
            this.checkBoxFix.Name = "checkBoxFix";
            this.checkBoxFix.Size = new System.Drawing.Size(88, 16);
            this.checkBoxFix.TabIndex = 7;
            this.checkBoxFix.Text = "섬네일 고정";
            this.checkBoxFix.UseVisualStyleBackColor = true;
            // 
            // axxpressStrm1
            // 
            this.axxpressStrm1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.axxpressStrm1.Enabled = true;
            this.axxpressStrm1.Location = new System.Drawing.Point(16, 11);
            this.axxpressStrm1.Name = "axxpressStrm1";
            this.axxpressStrm1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axxpressStrm1.OcxState")));
            this.axxpressStrm1.Size = new System.Drawing.Size(271, 224);
            this.axxpressStrm1.TabIndex = 3;
            this.axxpressStrm1.Notify += new AxxpressStrmLib._DxpressStrmEvents_NotifyEventHandler(this.axxpressStrm1_Notify);
            // 
            // TooltipCCTVCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(303, 320);
            this.Controls.Add(this.checkBoxFix);
            this.Controls.Add(this.axxpressStrm1);
            this.Controls.Add(this.labelManager);
            this.MaximumSize = new System.Drawing.Size(840, 800);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(316, 296);
            this.Name = "TooltipCCTVCtrl";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TooltipCCTVCtrl_FormClosing);
            this.Load += new System.EventHandler(this.TooltipCCTVCtrl_Load);
            this.Shown += new System.EventHandler(this.TooltipCCTVCtrl_Shown);
            this.SizeChanged += new System.EventHandler(this.TooltipCCTVCtrl_SizeChanged);
            this.Move += new System.EventHandler(this.TooltipCCTVCtrl_Move);
            this.Resize += new System.EventHandler(this.TooltipCCTVCtrl_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.axxpressStrm1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelManager;
        private AxxpressStrmLib.AxxpressStrm axxpressStrm1;
        private System.Windows.Forms.CheckBox checkBoxFix;
    }
}
