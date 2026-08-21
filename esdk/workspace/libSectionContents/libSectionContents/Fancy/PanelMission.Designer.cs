namespace SectionContents.Fancy
{
    partial class PanelMission
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
            this.components = new System.ComponentModel.Container();
            this.rbtnComplete = new UnE.GUI.RibbonButton();
            this.rbtnRun = new UnE.GUI.RibbonButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // rbtnComplete
            // 
            this.rbtnComplete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rbtnComplete.BackColor = System.Drawing.Color.Transparent;
            this.rbtnComplete.CheckButton = false;
            this.rbtnComplete.CheckedBkgndImage = null;
            this.rbtnComplete.CheckedImage = global::SectionContents.Properties.Resources.MissionComplete_Checked;
            this.rbtnComplete.CheckedMouseOver = global::SectionContents.Properties.Resources.MissionComplete_Checked_MouseOver;
            this.rbtnComplete.ClickedBackgroundImage = null;
            this.rbtnComplete.ClickedImage = null;
            this.rbtnComplete.CustomImageRect = new System.Drawing.Rectangle(0, 0, 30, 30);
            this.rbtnComplete.DisabledBkgndImage = null;
            this.rbtnComplete.DisabledImage = global::SectionContents.Properties.Resources.MissionComplete_Unchecked_Disabled;
            this.rbtnComplete.Enabled = false;
            this.rbtnComplete.ForeColor = System.Drawing.Color.White;
            this.rbtnComplete.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnComplete.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnComplete.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnComplete.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnComplete.ForeColorsByTypeUse = false;
            this.rbtnComplete.ID = -1;
            this.rbtnComplete.InitButtonWidth = 30;
            this.rbtnComplete.IsChecked = false;
            this.rbtnComplete.Location = new System.Drawing.Point(623, 14);
            this.rbtnComplete.MouseOverBkgndImage = null;
            this.rbtnComplete.MouseOverImage = global::SectionContents.Properties.Resources.MissionComplete_Unchecked_MouseOver;
            this.rbtnComplete.Name = "rbtnComplete";
            this.rbtnComplete.NormalImage = global::SectionContents.Properties.Resources.MissionComplete_Unchecked;
            this.rbtnComplete.Owner = null;
            this.rbtnComplete.Size = new System.Drawing.Size(30, 30);
            this.rbtnComplete.TabIndex = 2;
            this.rbtnComplete.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnComplete.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnComplete.ToolTipText = "";
            this.rbtnComplete.UseCustomImageRect = true;
            this.rbtnComplete.UseTextLocation = false;
            this.rbtnComplete.UseVisualStyleBackColor = false;
            this.rbtnComplete.Click += new System.EventHandler(this.rbtnComplete_Click);
            // 
            // rbtnRun
            // 
            this.rbtnRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rbtnRun.BackColor = System.Drawing.Color.Transparent;
            this.rbtnRun.CheckButton = false;
            this.rbtnRun.CheckedBkgndImage = null;
            this.rbtnRun.CheckedImage = null;
            this.rbtnRun.CheckedMouseOver = null;
            this.rbtnRun.ClickedBackgroundImage = null;
            this.rbtnRun.ClickedImage = global::SectionContents.Properties.Resources.RunButton_MouseOver;
            this.rbtnRun.CustomImageRect = new System.Drawing.Rectangle(0, 0, 37, 29);
            this.rbtnRun.DisabledBkgndImage = null;
            this.rbtnRun.DisabledImage = global::SectionContents.Properties.Resources.RunButton_Disabled;
            this.rbtnRun.Enabled = false;
            this.rbtnRun.ForeColor = System.Drawing.Color.White;
            this.rbtnRun.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnRun.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnRun.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnRun.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnRun.ForeColorsByTypeUse = false;
            this.rbtnRun.ID = -1;
            this.rbtnRun.InitButtonWidth = 37;
            this.rbtnRun.IsChecked = false;
            this.rbtnRun.Location = new System.Drawing.Point(488, 14);
            this.rbtnRun.MouseOverBkgndImage = null;
            this.rbtnRun.MouseOverImage = global::SectionContents.Properties.Resources.RunButton_MouseOver;
            this.rbtnRun.Name = "rbtnRun";
            this.rbtnRun.NormalImage = global::SectionContents.Properties.Resources.RunButton_Normal;
            this.rbtnRun.Owner = null;
            this.rbtnRun.Size = new System.Drawing.Size(37, 29);
            this.rbtnRun.TabIndex = 2;
            this.rbtnRun.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnRun.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnRun.ToolTipText = "";
            this.rbtnRun.UseCustomImageRect = true;
            this.rbtnRun.UseTextLocation = false;
            this.rbtnRun.UseVisualStyleBackColor = false;
            this.rbtnRun.Click += new System.EventHandler(this.rbtnRun_Click);
            // 
            // PanelMission
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.rbtnRun);
            this.Controls.Add(this.rbtnComplete);
            this.Name = "PanelMission";
            this.Size = new System.Drawing.Size(665, 60);
            this.SizeChanged += new System.EventHandler(this.PanelMission_SizeChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelMission_Paint);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PanelMission_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion

        private UnE.GUI.RibbonButton rbtnComplete;
        private UnE.GUI.RibbonButton rbtnRun;
        private System.Windows.Forms.Timer timer1;
    }
}
