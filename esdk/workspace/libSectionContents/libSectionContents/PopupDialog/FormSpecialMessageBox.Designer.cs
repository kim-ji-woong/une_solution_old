namespace SectionContents.PopupDialog
{
    partial class FormSpecialMessageBox
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
            this.plTitle = new System.Windows.Forms.Panel();
            this.imageButton1 = new UnE.GUI.ImageButton();
            this.pbTitle = new System.Windows.Forms.PictureBox();
            this.lbTitle = new System.Windows.Forms.Label();
            this.panelBackground = new System.Windows.Forms.Panel();
            this.btnOK = new UnE.GUI.RibbonButton();
            this.panelHelp = new System.Windows.Forms.Panel();
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.plTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageButton1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTitle)).BeginInit();
            this.panelBackground.SuspendLayout();
            this.SuspendLayout();
            // 
            // plTitle
            // 
            this.plTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.plTitle.Controls.Add(this.imageButton1);
            this.plTitle.Controls.Add(this.pbTitle);
            this.plTitle.Controls.Add(this.lbTitle);
            this.plTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.plTitle.Location = new System.Drawing.Point(0, 0);
            this.plTitle.Name = "plTitle";
            this.plTitle.Size = new System.Drawing.Size(500, 60);
            this.plTitle.TabIndex = 5;
            this.plTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseDown);
            this.plTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseMove);
            this.plTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseUp);
            // 
            // imageButton1
            // 
            this.imageButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.imageButton1.ButtonText = "";
            this.imageButton1.ImageClicked = global::SectionContents.Properties.Resources.btnClose_Selected;
            this.imageButton1.ImageDisabled = null;
            this.imageButton1.ImageMouseOver = global::SectionContents.Properties.Resources.btnClose_MouseOver;
            this.imageButton1.ImageNormal = global::SectionContents.Properties.Resources.btnClose_Normal;
            this.imageButton1.Location = new System.Drawing.Point(455, 15);
            this.imageButton1.Name = "imageButton1";
            this.imageButton1.Owner = null;
            this.imageButton1.Size = new System.Drawing.Size(30, 30);
            this.imageButton1.TabIndex = 4;
            this.imageButton1.TabStop = false;
            this.imageButton1.TextColor = System.Drawing.Color.Black;
            this.imageButton1.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.imageButton1.ToolTipText = "";
            this.imageButton1.UseToolTip = false;
            this.imageButton1.WindowRateWidth = 1F;
            this.imageButton1.Click += new System.EventHandler(this.btnOK_Click);
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
            this.pbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseDown);
            this.pbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseMove);
            this.pbTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseUp);
            // 
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.Font = new System.Drawing.Font("나눔스퀘어 Bold", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbTitle.ForeColor = System.Drawing.Color.White;
            this.lbTitle.Location = new System.Drawing.Point(47, 20);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(128, 22);
            this.lbTitle.TabIndex = 1;
            this.lbTitle.Text = "특수 문자 입력";
            this.lbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseDown);
            this.lbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseMove);
            this.lbTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseUp);
            // 
            // panelBackground
            // 
            this.panelBackground.BackColor = System.Drawing.SystemColors.Control;
            this.panelBackground.Controls.Add(this.btnOK);
            this.panelBackground.Controls.Add(this.panelHelp);
            this.panelBackground.Controls.Add(this.comboBoxType);
            this.panelBackground.Controls.Add(this.label1);
            this.panelBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBackground.Location = new System.Drawing.Point(0, 60);
            this.panelBackground.Name = "panelBackground";
            this.panelBackground.Size = new System.Drawing.Size(500, 695);
            this.panelBackground.TabIndex = 22;
            // 
            // btnOK
            // 
            this.btnOK.CheckButton = false;
            this.btnOK.CheckedBkgndImage = null;
            this.btnOK.CheckedImage = null;
            this.btnOK.CheckedMouseOver = null;
            this.btnOK.ClickedBackgroundImage = null;
            this.btnOK.ClickedImage = global::SectionContents.Properties.Resources.btnOk_Select;
            this.btnOK.CustomImageRect = new System.Drawing.Rectangle(0, 0, 150, 45);
            this.btnOK.DisabledBkgndImage = null;
            this.btnOK.DisabledImage = null;
            this.btnOK.ForeColorChecked = System.Drawing.Color.White;
            this.btnOK.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnOK.ForeColorDisabled = System.Drawing.Color.White;
            this.btnOK.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnOK.ForeColorsByTypeUse = false;
            this.btnOK.ID = -1;
            this.btnOK.InitButtonWidth = 150;
            this.btnOK.IsChecked = false;
            this.btnOK.Location = new System.Drawing.Point(343, 644);
            this.btnOK.MouseOverBkgndImage = null;
            this.btnOK.MouseOverImage = global::SectionContents.Properties.Resources.btnOk_Hover;
            this.btnOK.Name = "btnOK";
            this.btnOK.NormalImage = global::SectionContents.Properties.Resources.btnOk_Normal;
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(150, 45);
            this.btnOK.TabIndex = 109;
            this.btnOK.TextLocation = new System.Drawing.Point(0, 0);
            this.btnOK.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOK.ToolTipText = "";
            this.btnOK.UseCustomImageRect = true;
            this.btnOK.UseTextLocation = false;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // panelHelp
            // 
            this.panelHelp.BackColor = System.Drawing.Color.White;
            this.panelHelp.ForeColor = System.Drawing.Color.White;
            this.panelHelp.Location = new System.Drawing.Point(0, 43);
            this.panelHelp.Name = "panelHelp";
            this.panelHelp.Size = new System.Drawing.Size(500, 595);
            this.panelHelp.TabIndex = 23;
            // 
            // comboBoxType
            // 
            this.comboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxType.Font = new System.Drawing.Font("맑은 고딕", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.comboBoxType.FormattingEnabled = true;
            this.comboBoxType.Location = new System.Drawing.Point(99, 8);
            this.comboBoxType.Name = "comboBoxType";
            this.comboBoxType.Size = new System.Drawing.Size(145, 31);
            this.comboBoxType.TabIndex = 22;
            this.comboBoxType.SelectedIndexChanged += new System.EventHandler(this.comboBoxType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(13, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 23);
            this.label1.TabIndex = 21;
            this.label1.Text = "타입 선택";
            // 
            // FormSpecialMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 755);
            this.Controls.Add(this.panelBackground);
            this.Controls.Add(this.plTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormSpecialMessageBox";
            this.Text = "FormSpecialMessageBox";
            this.plTitle.ResumeLayout(false);
            this.plTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageButton1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTitle)).EndInit();
            this.panelBackground.ResumeLayout(false);
            this.panelBackground.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel plTitle;
        private System.Windows.Forms.PictureBox pbTitle;
        private System.Windows.Forms.Label lbTitle;
        private UnE.GUI.ImageButton imageButton1;
        private System.Windows.Forms.Panel panelBackground;
        private UnE.GUI.RibbonButton btnOK;
        private System.Windows.Forms.Panel panelHelp;
        private System.Windows.Forms.ComboBox comboBoxType;
        private System.Windows.Forms.Label label1;
    }
}