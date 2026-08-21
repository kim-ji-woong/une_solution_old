namespace SOPManager
{
    partial class PopupSpecialMessage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopupSpecialMessage));
            this.panelBackground = new System.Windows.Forms.Panel();
            this.panelHelp = new System.Windows.Forms.Panel();
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new UnE.GUI.RibbonButton();
            this.panelBackground.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelBackground
            // 
            this.panelBackground.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panelBackground.Controls.Add(this.panelHelp);
            this.panelBackground.Controls.Add(this.comboBoxType);
            this.panelBackground.Controls.Add(this.label1);
            this.panelBackground.Controls.Add(this.btnOK);
            this.panelBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBackground.Location = new System.Drawing.Point(0, 0);
            this.panelBackground.Name = "panelBackground";
            this.panelBackground.Size = new System.Drawing.Size(552, 684);
            this.panelBackground.TabIndex = 21;
            // 
            // panelHelp
            // 
            this.panelHelp.BackColor = System.Drawing.Color.Transparent;
            this.panelHelp.ForeColor = System.Drawing.Color.White;
            this.panelHelp.Location = new System.Drawing.Point(0, 56);
            this.panelHelp.Name = "panelHelp";
            this.panelHelp.Size = new System.Drawing.Size(552, 565);
            this.panelHelp.TabIndex = 23;
            // 
            // comboBoxType
            // 
            this.comboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxType.Font = new System.Drawing.Font("나눔스퀘어 Bold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.comboBoxType.FormattingEnabled = true;
            this.comboBoxType.Location = new System.Drawing.Point(100, 10);
            this.comboBoxType.Name = "comboBoxType";
            this.comboBoxType.Size = new System.Drawing.Size(145, 26);
            this.comboBoxType.TabIndex = 22;
            this.comboBoxType.SelectedIndexChanged += new System.EventHandler(this.comboBoxType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("나눔스퀘어 Bold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(17, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 18);
            this.label1.TabIndex = 21;
            this.label1.Text = "타입 선택";
            // 
            // btnOK
            // 
            this.btnOK.CheckButton = false;
            this.btnOK.CheckedBkgndImage = null;
            this.btnOK.CheckedImage = null;
            this.btnOK.ClickedBackgroundImage = null;
            this.btnOK.ClickedImage = ((System.Drawing.Image)(resources.GetObject("btnOK.ClickedImage")));
            this.btnOK.CustomImageRect = new System.Drawing.Rectangle(0, 0, 100, 55);
            this.btnOK.DisabledBkgndImage = null;
            this.btnOK.DisabledImage = null;
            this.btnOK.ID = -1;
            this.btnOK.InitButtonWidth = 100;
            this.btnOK.IsChecked = false;
            this.btnOK.Location = new System.Drawing.Point(449, 627);
            this.btnOK.MouseOverBkgndImage = null;
            this.btnOK.MouseOverImage = ((System.Drawing.Image)(resources.GetObject("btnOK.MouseOverImage")));
            this.btnOK.Name = "btnOK";
            this.btnOK.NormalImage = ((System.Drawing.Image)(resources.GetObject("btnOK.NormalImage")));
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(100, 55);
            this.btnOK.TabIndex = 95;
            this.btnOK.TextLocation = new System.Drawing.Point(0, 15);
            this.btnOK.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOK.ToolTipText = "";
            this.btnOK.UseCustomImageRect = false;
            this.btnOK.UseTextLocation = true;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // PopupSpecialMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.ClientSize = new System.Drawing.Size(552, 684);
            this.Controls.Add(this.panelBackground);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PopupSpecialMessage";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "특수 문자 입력";
            this.panelBackground.ResumeLayout(false);
            this.panelBackground.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelBackground;
        private System.Windows.Forms.ComboBox comboBoxType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelHelp;
        private UnE.GUI.RibbonButton btnOK;
    }
}