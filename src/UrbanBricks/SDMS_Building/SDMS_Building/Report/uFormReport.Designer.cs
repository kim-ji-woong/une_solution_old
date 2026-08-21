namespace SDMS_Building.Report
{
    partial class uFormReport
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.rbtnPareto = new UnE.GUI.RibbonButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbtnNotOperation = new UnE.GUI.RibbonButton();
            this.rbtnDetect = new UnE.GUI.RibbonButton();
            this.pnMain = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbtnPareto
            // 
            this.rbtnPareto.CheckButton = false;
            this.rbtnPareto.CheckedBkgndImage = null;
            this.rbtnPareto.CheckedImage = global::SDMS_Building.Properties.Resources.reportTab_checked;
            this.rbtnPareto.CheckedMouseOver = global::SDMS_Building.Properties.Resources.reportTab_checked;
            this.rbtnPareto.ClickedBackgroundImage = null;
            this.rbtnPareto.ClickedImage = global::SDMS_Building.Properties.Resources.reportTab_checked;
            this.rbtnPareto.CustomImageRect = new System.Drawing.Rectangle(0, 0, 200, 50);
            this.rbtnPareto.DisabledBkgndImage = null;
            this.rbtnPareto.DisabledImage = null;
            this.rbtnPareto.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnPareto.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnPareto.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnPareto.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnPareto.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnPareto.ForeColorsByTypeUse = true;
            this.rbtnPareto.ID = -1;
            this.rbtnPareto.InitButtonWidth = 200;
            this.rbtnPareto.IsChecked = false;
            this.rbtnPareto.Location = new System.Drawing.Point(50, 20);
            this.rbtnPareto.MouseOverBkgndImage = null;
            this.rbtnPareto.MouseOverImage = global::SDMS_Building.Properties.Resources.reportTab_hover;
            this.rbtnPareto.Name = "rbtnPareto";
            this.rbtnPareto.NormalImage = global::SDMS_Building.Properties.Resources.reportTab_unchecked;
            this.rbtnPareto.Owner = null;
            this.rbtnPareto.Size = new System.Drawing.Size(200, 50);
            this.rbtnPareto.TabIndex = 2;
            this.rbtnPareto.Text = "탐지 분석";
            this.rbtnPareto.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnPareto.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnPareto.ToolTipText = "탐지 분석";
            this.rbtnPareto.UseCustomImageRect = true;
            this.rbtnPareto.UseTextLocation = true;
            this.rbtnPareto.UseVisualStyleBackColor = true;
            this.rbtnPareto.Click += new System.EventHandler(this.rbtnPareto_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(198)))), ((int)(((byte)(215)))));
            this.panel1.Controls.Add(this.rbtnNotOperation);
            this.panel1.Controls.Add(this.rbtnDetect);
            this.panel1.Controls.Add(this.rbtnPareto);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(750, 70);
            this.panel1.TabIndex = 3;
            // 
            // rbtnNotOperation
            // 
            this.rbtnNotOperation.CheckButton = false;
            this.rbtnNotOperation.CheckedBkgndImage = null;
            this.rbtnNotOperation.CheckedImage = global::SDMS_Building.Properties.Resources.reportTab_checked;
            this.rbtnNotOperation.CheckedMouseOver = global::SDMS_Building.Properties.Resources.reportTab_checked;
            this.rbtnNotOperation.ClickedBackgroundImage = null;
            this.rbtnNotOperation.ClickedImage = global::SDMS_Building.Properties.Resources.reportTab_checked;
            this.rbtnNotOperation.CustomImageRect = new System.Drawing.Rectangle(0, 0, 200, 50);
            this.rbtnNotOperation.DisabledBkgndImage = null;
            this.rbtnNotOperation.DisabledImage = null;
            this.rbtnNotOperation.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnNotOperation.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnNotOperation.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnNotOperation.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnNotOperation.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnNotOperation.ForeColorsByTypeUse = true;
            this.rbtnNotOperation.ID = -1;
            this.rbtnNotOperation.InitButtonWidth = 200;
            this.rbtnNotOperation.IsChecked = false;
            this.rbtnNotOperation.Location = new System.Drawing.Point(502, 20);
            this.rbtnNotOperation.MouseOverBkgndImage = null;
            this.rbtnNotOperation.MouseOverImage = global::SDMS_Building.Properties.Resources.reportTab_hover;
            this.rbtnNotOperation.Name = "rbtnNotOperation";
            this.rbtnNotOperation.NormalImage = global::SDMS_Building.Properties.Resources.reportTab_unchecked;
            this.rbtnNotOperation.Owner = null;
            this.rbtnNotOperation.Size = new System.Drawing.Size(200, 50);
            this.rbtnNotOperation.TabIndex = 4;
            this.rbtnNotOperation.Text = "처리 이력";
            this.rbtnNotOperation.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnNotOperation.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnNotOperation.ToolTipText = "처리 이력";
            this.rbtnNotOperation.UseCustomImageRect = true;
            this.rbtnNotOperation.UseTextLocation = true;
            this.rbtnNotOperation.UseVisualStyleBackColor = true;
            this.rbtnNotOperation.Click += new System.EventHandler(this.rbtnNotOperation_Click);
            // 
            // rbtnDetect
            // 
            this.rbtnDetect.CheckButton = false;
            this.rbtnDetect.CheckedBkgndImage = null;
            this.rbtnDetect.CheckedImage = global::SDMS_Building.Properties.Resources.reportTab_checked;
            this.rbtnDetect.CheckedMouseOver = global::SDMS_Building.Properties.Resources.reportTab_checked;
            this.rbtnDetect.ClickedBackgroundImage = null;
            this.rbtnDetect.ClickedImage = global::SDMS_Building.Properties.Resources.reportTab_checked;
            this.rbtnDetect.CustomImageRect = new System.Drawing.Rectangle(0, 0, 200, 50);
            this.rbtnDetect.DisabledBkgndImage = null;
            this.rbtnDetect.DisabledImage = null;
            this.rbtnDetect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnDetect.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnDetect.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.rbtnDetect.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnDetect.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(96)))), ((int)(((byte)(151)))));
            this.rbtnDetect.ForeColorsByTypeUse = true;
            this.rbtnDetect.ID = -1;
            this.rbtnDetect.InitButtonWidth = 200;
            this.rbtnDetect.IsChecked = false;
            this.rbtnDetect.Location = new System.Drawing.Point(277, 20);
            this.rbtnDetect.MouseOverBkgndImage = null;
            this.rbtnDetect.MouseOverImage = global::SDMS_Building.Properties.Resources.reportTab_hover;
            this.rbtnDetect.Name = "rbtnDetect";
            this.rbtnDetect.NormalImage = global::SDMS_Building.Properties.Resources.reportTab_unchecked;
            this.rbtnDetect.Owner = null;
            this.rbtnDetect.Size = new System.Drawing.Size(200, 50);
            this.rbtnDetect.TabIndex = 3;
            this.rbtnDetect.Text = "탐지 이력";
            this.rbtnDetect.TextLocation = new System.Drawing.Point(0, 13);
            this.rbtnDetect.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnDetect.ToolTipText = "탐지 이력";
            this.rbtnDetect.UseCustomImageRect = true;
            this.rbtnDetect.UseTextLocation = true;
            this.rbtnDetect.UseVisualStyleBackColor = true;
            this.rbtnDetect.Click += new System.EventHandler(this.rbtnDetect_Click);
            // 
            // pnMain
            // 
            this.pnMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnMain.Location = new System.Drawing.Point(0, 70);
            this.pnMain.Name = "pnMain";
            this.pnMain.Size = new System.Drawing.Size(750, 364);
            this.pnMain.TabIndex = 4;
            // 
            // uFormReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(231)))), ((int)(((byte)(243)))));
            this.Controls.Add(this.pnMain);
            this.Controls.Add(this.panel1);
            this.Name = "uFormReport";
            this.Size = new System.Drawing.Size(750, 434);
            this.Load += new System.EventHandler(this.uFormReport_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private UnE.GUI.RibbonButton rbtnPareto;
        private System.Windows.Forms.Panel panel1;
        private UnE.GUI.RibbonButton rbtnNotOperation;
        private UnE.GUI.RibbonButton rbtnDetect;
        private System.Windows.Forms.Panel pnMain;
    }
}
