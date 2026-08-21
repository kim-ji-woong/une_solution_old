namespace SOPManager
{
    partial class PopupNote
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopupNote));
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.textBox = new System.Windows.Forms.TextBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnCancel = new UnE.GUI.ImageButton();
            this.btnOK = new UnE.GUI.ImageButton();
            this.btnShowSpecialMessage = new UnE.GUI.RibbonButton();
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnStandard = new System.Windows.Forms.Button();
            this.labelWarning3 = new System.Windows.Forms.Label();
            this.labelWarning2 = new System.Windows.Forms.Label();
            this.labelWarning = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOK)).BeginInit();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(433, 290);
            this.panel2.TabIndex = 23;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panel3.Controls.Add(this.textBox);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Controls.Add(this.panelTop);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(433, 290);
            this.panel3.TabIndex = 0;
            // 
            // textBox
            // 
            this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox.Font = new System.Drawing.Font("나눔스퀘어", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox.Location = new System.Drawing.Point(0, 8);
            this.textBox.Margin = new System.Windows.Forms.Padding(0);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox.Size = new System.Drawing.Size(433, 234);
            this.textBox.TabIndex = 19;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panel4.Controls.Add(this.btnCancel);
            this.panel4.Controls.Add(this.btnOK);
            this.panel4.Controls.Add(this.btnShowSpecialMessage);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 242);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(433, 48);
            this.panel4.TabIndex = 20;
            // 
            // btnCancel
            // 
            this.btnCancel.ButtonText = "";
            this.btnCancel.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ImageClicked = global::SOPManager.Properties.Resources.@__COMMON_CancelClick;
            this.btnCancel.ImageDisabled = null;
            this.btnCancel.ImageMouseOver = global::SOPManager.Properties.Resources.@__COMMON_CancelClick;
            this.btnCancel.ImageNormal = global::SOPManager.Properties.Resources.@__COMMON_Cancel;
            this.btnCancel.Location = new System.Drawing.Point(366, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(69, 37);
            this.btnCancel.TabIndex = 46;
            this.btnCancel.TabStop = false;
            this.btnCancel.TextColor = System.Drawing.Color.Black;
            this.btnCancel.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ToolTipText = "";
            this.btnCancel.UseToolTip = false;
            this.btnCancel.WindowRateWidth = 1F;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.ButtonText = "";
            this.btnOK.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.ImageClicked = global::SOPManager.Properties.Resources.@__COMMON_OkClick;
            this.btnOK.ImageDisabled = null;
            this.btnOK.ImageMouseOver = global::SOPManager.Properties.Resources.@__COMMON_OkClick;
            this.btnOK.ImageNormal = global::SOPManager.Properties.Resources.@__COMMON_Ok;
            this.btnOK.Location = new System.Drawing.Point(298, 6);
            this.btnOK.Name = "btnOK";
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(69, 37);
            this.btnOK.TabIndex = 45;
            this.btnOK.TabStop = false;
            this.btnOK.TextColor = System.Drawing.Color.Black;
            this.btnOK.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.ToolTipText = "";
            this.btnOK.UseToolTip = false;
            this.btnOK.WindowRateWidth = 1F;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnShowSpecialMessage
            // 
            this.btnShowSpecialMessage.CheckButton = false;
            this.btnShowSpecialMessage.CheckedBkgndImage = null;
            this.btnShowSpecialMessage.CheckedImage = null;
            this.btnShowSpecialMessage.ClickedBackgroundImage = null;
            this.btnShowSpecialMessage.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_SpecialcharoptionClick;
            this.btnShowSpecialMessage.CustomImageRect = new System.Drawing.Rectangle(0, 0, 110, 37);
            this.btnShowSpecialMessage.DisabledBkgndImage = null;
            this.btnShowSpecialMessage.DisabledImage = null;
            this.btnShowSpecialMessage.ID = -1;
            this.btnShowSpecialMessage.InitButtonWidth = 110;
            this.btnShowSpecialMessage.IsChecked = false;
            this.btnShowSpecialMessage.Location = new System.Drawing.Point(3, 5);
            this.btnShowSpecialMessage.MouseOverBkgndImage = null;
            this.btnShowSpecialMessage.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_SpecialcharoptionClick;
            this.btnShowSpecialMessage.Name = "btnShowSpecialMessage";
            this.btnShowSpecialMessage.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Specialcharoption;
            this.btnShowSpecialMessage.Owner = null;
            this.btnShowSpecialMessage.Size = new System.Drawing.Size(110, 37);
            this.btnShowSpecialMessage.TabIndex = 42;
            this.btnShowSpecialMessage.TextLocation = new System.Drawing.Point(-3, 18);
            this.btnShowSpecialMessage.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnShowSpecialMessage.ToolTipText = "";
            this.btnShowSpecialMessage.UseCustomImageRect = true;
            this.btnShowSpecialMessage.UseTextLocation = true;
            this.btnShowSpecialMessage.UseVisualStyleBackColor = true;
            this.btnShowSpecialMessage.Click += new System.EventHandler(this.btnShowSpecialMessage_Click);
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panelTop.Controls.Add(this.btnStandard);
            this.panelTop.Controls.Add(this.labelWarning3);
            this.panelTop.Controls.Add(this.labelWarning2);
            this.panelTop.Controls.Add(this.labelWarning);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Margin = new System.Windows.Forms.Padding(0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(433, 80);
            this.panelTop.TabIndex = 17;
            // 
            // btnStandard
            // 
            this.btnStandard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStandard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
            this.btnStandard.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnStandard.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.btnStandard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStandard.Font = new System.Drawing.Font("나눔스퀘어", 10F, System.Drawing.FontStyle.Bold);
            this.btnStandard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.btnStandard.Location = new System.Drawing.Point(324, 9);
            this.btnStandard.Name = "btnStandard";
            this.btnStandard.Size = new System.Drawing.Size(96, 27);
            this.btnStandard.TabIndex = 20;
            this.btnStandard.Text = "표준문구";
            this.btnStandard.UseVisualStyleBackColor = false;
            this.btnStandard.Visible = false;
            // 
            // labelWarning3
            // 
            this.labelWarning3.AutoSize = true;
            this.labelWarning3.Font = new System.Drawing.Font("나눔스퀘어", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelWarning3.ForeColor = System.Drawing.Color.White;
            this.labelWarning3.Location = new System.Drawing.Point(12, 50);
            this.labelWarning3.Name = "labelWarning3";
            this.labelWarning3.Size = new System.Drawing.Size(165, 17);
            this.labelWarning3.TabIndex = 17;
            this.labelWarning3.Text = "입력하지 말아 주십시오.)";
            // 
            // labelWarning2
            // 
            this.labelWarning2.AutoSize = true;
            this.labelWarning2.Font = new System.Drawing.Font("나눔스퀘어", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelWarning2.ForeColor = System.Drawing.Color.White;
            this.labelWarning2.Location = new System.Drawing.Point(12, 29);
            this.labelWarning2.Name = "labelWarning2";
            this.labelWarning2.Size = new System.Drawing.Size(284, 17);
            this.labelWarning2.TabIndex = 18;
            this.labelWarning2.Text = "개인 정보 보호를 위해서 특정 개인의 정보는";
            // 
            // labelWarning
            // 
            this.labelWarning.AutoSize = true;
            this.labelWarning.Font = new System.Drawing.Font("나눔스퀘어", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelWarning.ForeColor = System.Drawing.Color.White;
            this.labelWarning.Location = new System.Drawing.Point(9, 8);
            this.labelWarning.Name = "labelWarning";
            this.labelWarning.Size = new System.Drawing.Size(265, 17);
            this.labelWarning.TabIndex = 19;
            this.labelWarning.Text = "(외부로 임무 내용이 전파될 수 있으므로, ";
            // 
            // PopupNote
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.ClientSize = new System.Drawing.Size(433, 290);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(433, 274);
            this.Name = "PopupNote";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "메시지 작성";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PopupNote_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PopupNote_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PopupNote_MouseUp);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOK)).EndInit();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Panel panel4;
        private UnE.GUI.ImageButton btnCancel;
        private UnE.GUI.ImageButton btnOK;
        private UnE.GUI.RibbonButton btnShowSpecialMessage;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnStandard;
        private System.Windows.Forms.Label labelWarning3;
        private System.Windows.Forms.Label labelWarning2;
        private System.Windows.Forms.Label labelWarning;
    }
}