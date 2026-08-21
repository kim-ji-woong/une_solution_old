namespace SectionContents.Fancy
{
    partial class ComponentContents
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
            this.rbtnCollapse = new UnE.GUI.RibbonButton();
            this.rbtnNext = new UnE.GUI.RibbonButton();
            this.panelBody = new System.Windows.Forms.Panel();
            this.eleDecisions = new System.Windows.Forms.Integration.ElementHost();
            this.SuspendLayout();
            // 
            // rbtnCollapse
            // 
            this.rbtnCollapse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rbtnCollapse.BackColor = System.Drawing.Color.Transparent;
            this.rbtnCollapse.CheckButton = false;
            this.rbtnCollapse.CheckedBkgndImage = null;
            this.rbtnCollapse.CheckedImage = global::SectionContents.Properties.Resources.Collapsed_Normal;
            this.rbtnCollapse.CheckedMouseOver = global::SectionContents.Properties.Resources.Collapsed_MouseOver;
            this.rbtnCollapse.ClickedBackgroundImage = null;
            this.rbtnCollapse.ClickedImage = null;
            this.rbtnCollapse.CustomImageRect = new System.Drawing.Rectangle(0, 0, 30, 30);
            this.rbtnCollapse.DisabledBkgndImage = null;
            this.rbtnCollapse.DisabledImage = null;
            this.rbtnCollapse.ForeColor = System.Drawing.Color.White;
            this.rbtnCollapse.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnCollapse.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnCollapse.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnCollapse.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnCollapse.ForeColorsByTypeUse = false;
            this.rbtnCollapse.ID = -1;
            this.rbtnCollapse.InitButtonWidth = 30;
            this.rbtnCollapse.IsChecked = false;
            this.rbtnCollapse.Location = new System.Drawing.Point(754, 12);
            this.rbtnCollapse.MouseOverBkgndImage = null;
            this.rbtnCollapse.MouseOverImage = global::SectionContents.Properties.Resources.Extend_MouseOver;
            this.rbtnCollapse.Name = "rbtnCollapse";
            this.rbtnCollapse.NormalImage = global::SectionContents.Properties.Resources.Extend_Normal;
            this.rbtnCollapse.Owner = null;
            this.rbtnCollapse.Size = new System.Drawing.Size(30, 30);
            this.rbtnCollapse.TabIndex = 2;
            this.rbtnCollapse.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnCollapse.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnCollapse.ToolTipText = "";
            this.rbtnCollapse.UseCustomImageRect = true;
            this.rbtnCollapse.UseTextLocation = false;
            this.rbtnCollapse.UseVisualStyleBackColor = false;
            this.rbtnCollapse.Click += new System.EventHandler(this.rbtnCollapse_Click);
            // 
            // rbtnNext
            // 
            this.rbtnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rbtnNext.BackColor = System.Drawing.Color.Transparent;
            this.rbtnNext.CheckButton = false;
            this.rbtnNext.CheckedBkgndImage = null;
            this.rbtnNext.CheckedImage = null;
            this.rbtnNext.CheckedMouseOver = null;
            this.rbtnNext.ClickedBackgroundImage = null;
            this.rbtnNext.ClickedImage = global::SectionContents.Properties.Resources.Next_Clicked;
            this.rbtnNext.CustomImageRect = new System.Drawing.Rectangle(0, 0, 50, 34);
            this.rbtnNext.DisabledBkgndImage = null;
            this.rbtnNext.DisabledImage = global::SectionContents.Properties.Resources.Next_Disabled;
            this.rbtnNext.Enabled = false;
            this.rbtnNext.ForeColor = System.Drawing.Color.White;
            this.rbtnNext.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnNext.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnNext.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnNext.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnNext.ForeColorsByTypeUse = false;
            this.rbtnNext.ID = -1;
            this.rbtnNext.InitButtonWidth = 50;
            this.rbtnNext.IsChecked = false;
            this.rbtnNext.Location = new System.Drawing.Point(696, 12);
            this.rbtnNext.MouseOverBkgndImage = null;
            this.rbtnNext.MouseOverImage = global::SectionContents.Properties.Resources.Next_MouseOver;
            this.rbtnNext.Name = "rbtnNext";
            this.rbtnNext.NormalImage = global::SectionContents.Properties.Resources.Next_Normal;
            this.rbtnNext.Owner = null;
            this.rbtnNext.Size = new System.Drawing.Size(50, 34);
            this.rbtnNext.TabIndex = 2;
            this.rbtnNext.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnNext.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnNext.ToolTipText = "";
            this.rbtnNext.UseCustomImageRect = true;
            this.rbtnNext.UseTextLocation = false;
            this.rbtnNext.UseVisualStyleBackColor = false;
            this.rbtnNext.Click += new System.EventHandler(this.rbtnNext_Click);
            // 
            // panelBody
            // 
            this.panelBody.AutoScroll = true;
            this.panelBody.Location = new System.Drawing.Point(0, 60);
            this.panelBody.Name = "panelBody";
            this.panelBody.Size = new System.Drawing.Size(800, 300);
            this.panelBody.TabIndex = 3;
            // 
            // eleDecisions
            // 
            this.eleDecisions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.eleDecisions.Location = new System.Drawing.Point(496, 17);
            this.eleDecisions.Name = "eleDecisions";
            this.eleDecisions.Size = new System.Drawing.Size(194, 25);
            this.eleDecisions.TabIndex = 5;
            this.eleDecisions.Text = "elementHost1";
            this.eleDecisions.Visible = false;
            this.eleDecisions.Child = null;
            // 
            // ComponentContents
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.Controls.Add(this.eleDecisions);
            this.Controls.Add(this.panelBody);
            this.Controls.Add(this.rbtnNext);
            this.Controls.Add(this.rbtnCollapse);
            this.Name = "ComponentContents";
            this.Size = new System.Drawing.Size(800, 360);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ComponentContentsProcess_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ComponentContents_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ComponentContents_MouseUp);
            this.Resize += new System.EventHandler(this.ComponentContentsProcess_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private UnE.GUI.RibbonButton rbtnCollapse;
        private UnE.GUI.RibbonButton rbtnNext;
        private System.Windows.Forms.Panel panelBody;
        private System.Windows.Forms.Integration.ElementHost eleDecisions;
    }
}
