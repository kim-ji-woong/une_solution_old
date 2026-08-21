namespace SOPManager
{
    partial class PopupProcessNumber
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopupProcessNumber));
            this.cboOption = new System.Windows.Forms.ComboBox();
            this.cboNumber = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnCancel = new UnE.GUI.RibbonButton();
            this.ribbonButton1 = new UnE.GUI.RibbonButton();
            this.label4 = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboOption
            // 
            this.cboOption.BackColor = System.Drawing.Color.White;
            this.cboOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOption.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboOption.ForeColor = System.Drawing.Color.Black;
            this.cboOption.FormattingEnabled = true;
            this.cboOption.Items.AddRange(new object[] {
            "전체기간중",
            "연중",
            "월중",
            "주중",
            "하루중",
            "시간당"});
            this.cboOption.Location = new System.Drawing.Point(22, 54);
            this.cboOption.Name = "cboOption";
            this.cboOption.Size = new System.Drawing.Size(122, 25);
            this.cboOption.TabIndex = 0;
            this.cboOption.SelectedIndexChanged += new System.EventHandler(this.cboOption_SelectedIndexChanged);
            // 
            // cboNumber
            // 
            this.cboNumber.BackColor = System.Drawing.Color.White;
            this.cboNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNumber.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboNumber.ForeColor = System.Drawing.Color.Black;
            this.cboNumber.FormattingEnabled = true;
            this.cboNumber.Location = new System.Drawing.Point(150, 54);
            this.cboNumber.Name = "cboNumber";
            this.cboNumber.Size = new System.Drawing.Size(87, 25);
            this.cboNumber.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font(Program.prgFont, 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.label1.Location = new System.Drawing.Point(244, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(22, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "회";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(3);
            this.panel2.Size = new System.Drawing.Size(270, 159);
            this.panel2.TabIndex = 23;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Transparent;
            this.panel3.Controls.Add(this.btnCancel);
            this.panel3.Controls.Add(this.ribbonButton1);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(264, 153);
            this.panel3.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.CheckButton = false;
            this.btnCancel.CheckedBkgndImage = null;
            this.btnCancel.CheckedImage = null;
            this.btnCancel.ClickedBackgroundImage = null;
            this.btnCancel.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_CancelClick;
            this.btnCancel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 69, 37);
            this.btnCancel.DisabledBkgndImage = null;
            this.btnCancel.DisabledImage = null;
            this.btnCancel.ID = -1;
            this.btnCancel.InitButtonWidth = 69;
            this.btnCancel.IsChecked = false;
            this.btnCancel.Location = new System.Drawing.Point(193, 111);
            this.btnCancel.MouseOverBkgndImage = null;
            this.btnCancel.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_CancelClick;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Cancel;
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(69, 37);
            this.btnCancel.TabIndex = 46;
            this.btnCancel.TextLocation = new System.Drawing.Point(0, 0);
            this.btnCancel.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCancel.ToolTipText = "";
            this.btnCancel.UseCustomImageRect = true;
            this.btnCancel.UseTextLocation = false;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ribbonButton1
            // 
            this.ribbonButton1.CheckButton = false;
            this.ribbonButton1.CheckedBkgndImage = null;
            this.ribbonButton1.CheckedImage = null;
            this.ribbonButton1.ClickedBackgroundImage = null;
            this.ribbonButton1.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_OkClick;
            this.ribbonButton1.CustomImageRect = new System.Drawing.Rectangle(0, 0, 69, 37);
            this.ribbonButton1.DisabledBkgndImage = null;
            this.ribbonButton1.DisabledImage = null;
            this.ribbonButton1.ID = -1;
            this.ribbonButton1.InitButtonWidth = 69;
            this.ribbonButton1.IsChecked = false;
            this.ribbonButton1.Location = new System.Drawing.Point(123, 111);
            this.ribbonButton1.MouseOverBkgndImage = null;
            this.ribbonButton1.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_OkClick;
            this.ribbonButton1.Name = "ribbonButton1";
            this.ribbonButton1.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Ok;
            this.ribbonButton1.Owner = null;
            this.ribbonButton1.Size = new System.Drawing.Size(69, 37);
            this.ribbonButton1.TabIndex = 45;
            this.ribbonButton1.TextLocation = new System.Drawing.Point(0, 0);
            this.ribbonButton1.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.ribbonButton1.ToolTipText = "";
            this.ribbonButton1.UseCustomImageRect = true;
            this.ribbonButton1.UseTextLocation = false;
            this.ribbonButton1.UseVisualStyleBackColor = true;
            this.ribbonButton1.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(14, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(181, 18);
            this.label4.TabIndex = 3;
            this.label4.Text = "처리횟수를 지정하십시오.";
            // 
            // PopupProcessNumber
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.ClientSize = new System.Drawing.Size(270, 159);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboNumber);
            this.Controls.Add(this.cboOption);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PopupProcessNumber";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "처리횟수";
            this.Load += new System.EventHandler(this.PopupProcessNumber_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PopupProcessNumber_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PopupProcessNumber_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PopupProcessNumber_MouseUp);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboOption;
        private System.Windows.Forms.ComboBox cboNumber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
		protected System.Windows.Forms.Label label4;
        private UnE.GUI.RibbonButton btnCancel;
        private UnE.GUI.RibbonButton ribbonButton1;
    }
}