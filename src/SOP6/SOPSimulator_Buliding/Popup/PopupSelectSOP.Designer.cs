namespace SOPMonitoringSystem.Popup
{
    partial class PopupSelectSOP
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
            this.components = new System.ComponentModel.Container();
            this.plTitle = new System.Windows.Forms.Panel();
            this.btnCancle = new UnE.GUI.RibbonButton();
            this.pbTitle = new System.Windows.Forms.PictureBox();
            this.lbTitle = new System.Windows.Forms.Label();
            this.lblSenario = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rdoEmergency = new System.Windows.Forms.RadioButton();
            this.rdoNormal = new System.Windows.Forms.RadioButton();
            this.btnClose = new UnE.GUI.RibbonButton();
            this.btnSelect = new UnE.GUI.RibbonButton();
            this.treeSOP = new SOPMonitoringSystem.Popup.SOPTreeSim();
            this.plTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTitle)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // plTitle
            // 
            this.plTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.plTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.plTitle.Controls.Add(this.btnCancle);
            this.plTitle.Controls.Add(this.pbTitle);
            this.plTitle.Controls.Add(this.lbTitle);
            this.plTitle.Location = new System.Drawing.Point(0, 0);
            this.plTitle.Name = "plTitle";
            this.plTitle.Size = new System.Drawing.Size(795, 60);
            this.plTitle.TabIndex = 4;
            this.plTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plTitle_MouseDown);
            this.plTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plTitle_MouseMove);
            this.plTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.plTitle_MouseUp);
            // 
            // btnCancle
            // 
            this.btnCancle.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.btnCancle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCancle.CheckButton = false;
            this.btnCancle.CheckedBkgndImage = null;
            this.btnCancle.CheckedImage = null;
            this.btnCancle.CheckedMouseOver = null;
            this.btnCancle.ClickedBackgroundImage = null;
            this.btnCancle.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.btnClose_Selected;
            this.btnCancle.CustomImageRect = new System.Drawing.Rectangle(0, 0, 22, 22);
            this.btnCancle.DisabledBkgndImage = null;
            this.btnCancle.DisabledImage = null;
            this.btnCancle.ForeColorChecked = System.Drawing.Color.White;
            this.btnCancle.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnCancle.ForeColorDisabled = System.Drawing.Color.White;
            this.btnCancle.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnCancle.ForeColorsByTypeUse = false;
            this.btnCancle.ID = -1;
            this.btnCancle.InitButtonWidth = 22;
            this.btnCancle.IsChecked = false;
            this.btnCancle.Location = new System.Drawing.Point(753, 20);
            this.btnCancle.MouseOverBkgndImage = null;
            this.btnCancle.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.btnClose_MouseOver;
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.NormalImage = global::SOPMonitoringSystem.Properties.Resources.btnClose_Normal;
            this.btnCancle.Owner = null;
            this.btnCancle.Size = new System.Drawing.Size(22, 22);
            this.btnCancle.TabIndex = 110;
            this.btnCancle.TextLocation = new System.Drawing.Point(0, 0);
            this.btnCancle.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCancle.ToolTipText = "";
            this.btnCancle.UseCustomImageRect = false;
            this.btnCancle.UseTextLocation = false;
            this.btnCancle.UseVisualStyleBackColor = false;
            this.btnCancle.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // pbTitle
            // 
            this.pbTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.pbTitle.Location = new System.Drawing.Point(22, 28);
            this.pbTitle.Margin = new System.Windows.Forms.Padding(0);
            this.pbTitle.Name = "pbTitle";
            this.pbTitle.Size = new System.Drawing.Size(5, 5);
            this.pbTitle.TabIndex = 3;
            this.pbTitle.TabStop = false;
            this.pbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbTitle_MouseDown);
            this.pbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbTitle_MouseMove);
            this.pbTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbTitle_MouseUp);
            // 
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.Font = new System.Drawing.Font("나눔스퀘어 Bold", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbTitle.ForeColor = System.Drawing.Color.White;
            this.lbTitle.Location = new System.Drawing.Point(43, 20);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(166, 22);
            this.lbTitle.TabIndex = 1;
            this.lbTitle.Text = "SOP 시나리오 선택";
            this.lbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseDown);
            this.lbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseMove);
            this.lbTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseUp);
            // 
            // lblSenario
            // 
            this.lblSenario.AutoSize = true;
            this.lblSenario.Font = new System.Drawing.Font("나눔스퀘어 Bold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblSenario.Location = new System.Drawing.Point(37, 82);
            this.lblSenario.Name = "lblSenario";
            this.lblSenario.Size = new System.Drawing.Size(102, 18);
            this.lblSenario.TabIndex = 65;
            this.lblSenario.Text = "평일 시나리오";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.treeSOP);
            this.panel2.Location = new System.Drawing.Point(30, 118);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(750, 524);
            this.panel2.TabIndex = 63;
            // 
            // rdoEmergency
            // 
            this.rdoEmergency.AutoSize = true;
            this.rdoEmergency.Location = new System.Drawing.Point(121, 655);
            this.rdoEmergency.Name = "rdoEmergency";
            this.rdoEmergency.Size = new System.Drawing.Size(119, 16);
            this.rdoEmergency.TabIndex = 61;
            this.rdoEmergency.TabStop = true;
            this.rdoEmergency.Text = "야간 및 휴일 모드";
            this.rdoEmergency.UseVisualStyleBackColor = true;
            this.rdoEmergency.Visible = false;
            // 
            // rdoNormal
            // 
            this.rdoNormal.AutoSize = true;
            this.rdoNormal.Location = new System.Drawing.Point(40, 655);
            this.rdoNormal.Name = "rdoNormal";
            this.rdoNormal.Size = new System.Drawing.Size(75, 16);
            this.rdoNormal.TabIndex = 60;
            this.rdoNormal.TabStop = true;
            this.rdoNormal.Text = "평일 모드";
            this.rdoNormal.UseVisualStyleBackColor = true;
            this.rdoNormal.Visible = false;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.CheckButton = false;
            this.btnClose.CheckedBkgndImage = null;
            this.btnClose.CheckedImage = null;
            this.btnClose.CheckedMouseOver = null;
            this.btnClose.ClickedBackgroundImage = null;
            this.btnClose.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.QuickCloseButton_Selected;
            this.btnClose.CustomImageRect = new System.Drawing.Rectangle(0, 0, 146, 45);
            this.btnClose.DisabledBkgndImage = null;
            this.btnClose.DisabledImage = null;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnClose.ForeColor = System.Drawing.Color.Black;
            this.btnClose.ForeColorChecked = System.Drawing.Color.Black;
            this.btnClose.ForeColorCheckedMouseOver = System.Drawing.Color.Black;
            this.btnClose.ForeColorDisabled = System.Drawing.Color.Black;
            this.btnClose.ForeColorMouseOver = System.Drawing.Color.Black;
            this.btnClose.ForeColorsByTypeUse = false;
            this.btnClose.ID = -1;
            this.btnClose.InitButtonWidth = 146;
            this.btnClose.IsChecked = false;
            this.btnClose.Location = new System.Drawing.Point(634, 657);
            this.btnClose.MouseOverBkgndImage = null;
            this.btnClose.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.QuickCloseButton_MouseOver;
            this.btnClose.Name = "btnClose";
            this.btnClose.NormalImage = global::SOPMonitoringSystem.Properties.Resources.QuickCloseButton_Normal;
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(146, 45);
            this.btnClose.TabIndex = 130;
            this.btnClose.Text = "닫기";
            this.btnClose.TextLocation = new System.Drawing.Point(50, 12);
            this.btnClose.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnClose.ToolTipText = "닫기";
            this.btnClose.UseCustomImageRect = true;
            this.btnClose.UseTextLocation = true;
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.BackColor = System.Drawing.Color.Transparent;
            this.btnSelect.CheckButton = false;
            this.btnSelect.CheckedBkgndImage = null;
            this.btnSelect.CheckedImage = null;
            this.btnSelect.CheckedMouseOver = null;
            this.btnSelect.ClickedBackgroundImage = null;
            this.btnSelect.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.QuickSaveButton_Selected;
            this.btnSelect.CustomImageRect = new System.Drawing.Rectangle(0, 0, 146, 45);
            this.btnSelect.DisabledBkgndImage = null;
            this.btnSelect.DisabledImage = global::SOPMonitoringSystem.Properties.Resources.QuickSaveButton_Disable;
            this.btnSelect.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSelect.ForeColor = System.Drawing.Color.White;
            this.btnSelect.ForeColorChecked = System.Drawing.Color.White;
            this.btnSelect.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnSelect.ForeColorDisabled = System.Drawing.Color.White;
            this.btnSelect.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnSelect.ForeColorsByTypeUse = false;
            this.btnSelect.ID = -1;
            this.btnSelect.InitButtonWidth = 146;
            this.btnSelect.IsChecked = false;
            this.btnSelect.Location = new System.Drawing.Point(480, 657);
            this.btnSelect.MouseOverBkgndImage = null;
            this.btnSelect.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.QuickSaveButton_MouseOver;
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.NormalImage = global::SOPMonitoringSystem.Properties.Resources.QuickSaveButton_Normal;
            this.btnSelect.Owner = null;
            this.btnSelect.Size = new System.Drawing.Size(146, 45);
            this.btnSelect.TabIndex = 129;
            this.btnSelect.Text = "선택";
            this.btnSelect.TextLocation = new System.Drawing.Point(50, 12);
            this.btnSelect.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnSelect.ToolTipText = "선택";
            this.btnSelect.UseCustomImageRect = true;
            this.btnSelect.UseTextLocation = true;
            this.btnSelect.UseVisualStyleBackColor = false;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // treeSOP
            // 
            this.treeSOP.BackColor = System.Drawing.SystemColors.Window;
            this.treeSOP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeSOP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeSOP.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold);
            this.treeSOP.IgnoreLoadSOP = false;
            this.treeSOP.IgnoreSelect = false;
            this.treeSOP.ImageIndex = 0;
            this.treeSOP.Location = new System.Drawing.Point(0, 0);
            this.treeSOP.Name = "treeSOP";
            this.treeSOP.PrevSelectedDisasterID = -1;
            this.treeSOP.PrevSelectedNode = null;
            this.treeSOP.SelectedImageIndex = 0;
            this.treeSOP.Size = new System.Drawing.Size(750, 524);
            this.treeSOP.TabIndex = 60;
            this.treeSOP.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeSOP_AfterSelect);
            // 
            // PopupSelectSOP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(795, 717);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.rdoEmergency);
            this.Controls.Add(this.rdoNormal);
            this.Controls.Add(this.lblSenario);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.plTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PopupSelectSOP";
            this.Text = "PopupSelectSOP";
            this.Load += new System.EventHandler(this.PopupSelectSOP_Load);
            this.plTitle.ResumeLayout(false);
            this.plTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTitle)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel plTitle;
        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.RadioButton rdoEmergency;
        private System.Windows.Forms.RadioButton rdoNormal;
        private System.Windows.Forms.Panel panel2;
        private SOPTreeSim treeSOP;
        private System.Windows.Forms.Label lblSenario;
        private System.Windows.Forms.PictureBox pbTitle;
        private UnE.GUI.RibbonButton btnCancle;
        private UnE.GUI.RibbonButton btnClose;
        private UnE.GUI.RibbonButton btnSelect;
    }
}