namespace SOPManager
{
    partial class PopupTransSOP
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopupTransSOP));
            this.treeView = new System.Windows.Forms.TreeView();
            this.radioNormal = new System.Windows.Forms.RadioButton();
            this.radioAbnormal = new System.Windows.Forms.RadioButton();
            this.rdLabel2 = new System.Windows.Forms.Label();
            this.rdLabel1 = new System.Windows.Forms.Label();
            this.rdPictureBox2 = new System.Windows.Forms.PictureBox();
            this.rdPictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.ribbonButton1 = new UnE.GUI.RibbonButton();
            this.btnCancel = new UnE.GUI.RibbonButton();
            this.btnOK = new UnE.GUI.RibbonButton();
            ((System.ComponentModel.ISupportInitialize)(this.rdPictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdPictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.Font = new System.Drawing.Font("나눔스퀘어", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeView.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            this.treeView.Location = new System.Drawing.Point(12, 44);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(460, 277);
            this.treeView.TabIndex = 0;
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // radioNormal
            // 
            this.radioNormal.AutoSize = true;
            this.radioNormal.BackColor = System.Drawing.Color.Transparent;
            this.radioNormal.Font = new System.Drawing.Font("나눔스퀘어", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioNormal.ForeColor = System.Drawing.Color.White;
            this.radioNormal.Location = new System.Drawing.Point(313, 5);
            this.radioNormal.Name = "radioNormal";
            this.radioNormal.Size = new System.Drawing.Size(72, 17);
            this.radioNormal.TabIndex = 4;
            this.radioNormal.TabStop = true;
            this.radioNormal.Text = "평일 모드";
            this.radioNormal.UseVisualStyleBackColor = false;
            this.radioNormal.CheckedChanged += new System.EventHandler(this.radioNormal_CheckedChanged);
            // 
            // radioAbnormal
            // 
            this.radioAbnormal.AutoSize = true;
            this.radioAbnormal.BackColor = System.Drawing.Color.Transparent;
            this.radioAbnormal.Font = new System.Drawing.Font("나눔스퀘어", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioAbnormal.ForeColor = System.Drawing.Color.White;
            this.radioAbnormal.Location = new System.Drawing.Point(313, 23);
            this.radioAbnormal.Name = "radioAbnormal";
            this.radioAbnormal.Size = new System.Drawing.Size(111, 17);
            this.radioAbnormal.TabIndex = 4;
            this.radioAbnormal.TabStop = true;
            this.radioAbnormal.Text = "야간 및 휴일 모드";
            this.radioAbnormal.UseVisualStyleBackColor = false;
            this.radioAbnormal.CheckedChanged += new System.EventHandler(this.radioAbnormal_CheckedChanged);
            // 
            // rdLabel2
            // 
            this.rdLabel2.AutoSize = true;
            this.rdLabel2.BackColor = System.Drawing.Color.Transparent;
            this.rdLabel2.Font = new System.Drawing.Font("나눔스퀘어", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rdLabel2.ForeColor = System.Drawing.Color.White;
            this.rdLabel2.Location = new System.Drawing.Point(164, 14);
            this.rdLabel2.Name = "rdLabel2";
            this.rdLabel2.Size = new System.Drawing.Size(118, 17);
            this.rdLabel2.TabIndex = 28;
            this.rdLabel2.Text = "야간 및 휴일 모드";
            this.rdLabel2.Click += new System.EventHandler(this.rdLabel2_Click);
            // 
            // rdLabel1
            // 
            this.rdLabel1.AutoSize = true;
            this.rdLabel1.BackColor = System.Drawing.Color.Transparent;
            this.rdLabel1.Font = new System.Drawing.Font("나눔스퀘어", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rdLabel1.ForeColor = System.Drawing.Color.White;
            this.rdLabel1.Location = new System.Drawing.Point(50, 14);
            this.rdLabel1.Name = "rdLabel1";
            this.rdLabel1.Size = new System.Drawing.Size(68, 17);
            this.rdLabel1.TabIndex = 27;
            this.rdLabel1.Text = "평일 모드";
            this.rdLabel1.Click += new System.EventHandler(this.rdLabel1_Click);
            // 
            // rdPictureBox2
            // 
            this.rdPictureBox2.BackgroundImage = global::SOPManager.Properties.Resources.@__SOPEDIT_Disable2;
            this.rdPictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rdPictureBox2.Location = new System.Drawing.Point(145, 13);
            this.rdPictureBox2.Name = "rdPictureBox2";
            this.rdPictureBox2.Size = new System.Drawing.Size(18, 17);
            this.rdPictureBox2.TabIndex = 25;
            this.rdPictureBox2.TabStop = false;
            this.rdPictureBox2.Click += new System.EventHandler(this.rdPictureBox2_Click);
            // 
            // rdPictureBox1
            // 
            this.rdPictureBox1.BackgroundImage = global::SOPManager.Properties.Resources.@__SOPEDIT_Disable2;
            this.rdPictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rdPictureBox1.Location = new System.Drawing.Point(30, 13);
            this.rdPictureBox1.Name = "rdPictureBox1";
            this.rdPictureBox1.Size = new System.Drawing.Size(18, 17);
            this.rdPictureBox1.TabIndex = 26;
            this.rdPictureBox1.TabStop = false;
            this.rdPictureBox1.Click += new System.EventHandler(this.rdPictureBox1_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(3);
            this.panel2.Size = new System.Drawing.Size(484, 394);
            this.panel2.TabIndex = 30;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Transparent;
            this.panel3.Controls.Add(this.ribbonButton1);
            this.panel3.Controls.Add(this.btnCancel);
            this.panel3.Controls.Add(this.btnOK);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(478, 388);
            this.panel3.TabIndex = 0;
            // 
            // ribbonButton1
            // 
            this.ribbonButton1.CheckButton = false;
            this.ribbonButton1.CheckedBkgndImage = null;
            this.ribbonButton1.CheckedImage = null;
            this.ribbonButton1.ClickedBackgroundImage = null;
            this.ribbonButton1.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_EmptyBackClick;
            this.ribbonButton1.CustomImageRect = new System.Drawing.Rectangle(0, 0, 100, 55);
            this.ribbonButton1.DisabledBkgndImage = null;
            this.ribbonButton1.DisabledImage = null;
            this.ribbonButton1.ID = -1;
            this.ribbonButton1.InitButtonWidth = 100;
            this.ribbonButton1.IsChecked = false;
            this.ribbonButton1.Location = new System.Drawing.Point(9, 330);
            this.ribbonButton1.MouseOverBkgndImage = null;
            this.ribbonButton1.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_EmptyBackClick;
            this.ribbonButton1.Name = "ribbonButton1";
            this.ribbonButton1.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_EmptyBack;
            this.ribbonButton1.Owner = null;
            this.ribbonButton1.Size = new System.Drawing.Size(100, 55);
            this.ribbonButton1.TabIndex = 44;
            this.ribbonButton1.Text = "선택취소";
            this.ribbonButton1.TextLocation = new System.Drawing.Point(-4, 17);
            this.ribbonButton1.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.ribbonButton1.ToolTipText = "선택취소";
            this.ribbonButton1.UseCustomImageRect = true;
            this.ribbonButton1.UseTextLocation = true;
            this.ribbonButton1.UseVisualStyleBackColor = true;
            this.ribbonButton1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.CheckButton = false;
            this.btnCancel.CheckedBkgndImage = null;
            this.btnCancel.CheckedImage = null;
            this.btnCancel.ClickedBackgroundImage = null;
            this.btnCancel.ClickedImage = ((System.Drawing.Image)(resources.GetObject("btnCancel.ClickedImage")));
            this.btnCancel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 100, 55);
            this.btnCancel.DisabledBkgndImage = null;
            this.btnCancel.DisabledImage = null;
            this.btnCancel.ID = -1;
            this.btnCancel.InitButtonWidth = 100;
            this.btnCancel.IsChecked = false;
            this.btnCancel.Location = new System.Drawing.Point(377, 337);
            this.btnCancel.MouseOverBkgndImage = null;
            this.btnCancel.MouseOverImage = ((System.Drawing.Image)(resources.GetObject("btnCancel.MouseOverImage")));
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.NormalImage = ((System.Drawing.Image)(resources.GetObject("btnCancel.NormalImage")));
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(100, 55);
            this.btnCancel.TabIndex = 43;
            this.btnCancel.TextLocation = new System.Drawing.Point(0, 0);
            this.btnCancel.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCancel.ToolTipText = "";
            this.btnCancel.UseCustomImageRect = false;
            this.btnCancel.UseTextLocation = false;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
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
            this.btnOK.Location = new System.Drawing.Point(282, 337);
            this.btnOK.MouseOverBkgndImage = null;
            this.btnOK.MouseOverImage = ((System.Drawing.Image)(resources.GetObject("btnOK.MouseOverImage")));
            this.btnOK.Name = "btnOK";
            this.btnOK.NormalImage = ((System.Drawing.Image)(resources.GetObject("btnOK.NormalImage")));
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(100, 55);
            this.btnOK.TabIndex = 42;
            this.btnOK.TextLocation = new System.Drawing.Point(0, 0);
            this.btnOK.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOK.ToolTipText = "";
            this.btnOK.UseCustomImageRect = false;
            this.btnOK.UseTextLocation = false;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // PopupTransSOP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.ClientSize = new System.Drawing.Size(484, 394);
            this.Controls.Add(this.rdLabel2);
            this.Controls.Add(this.rdLabel1);
            this.Controls.Add(this.rdPictureBox2);
            this.Controls.Add(this.rdPictureBox1);
            this.Controls.Add(this.radioAbnormal);
            this.Controls.Add(this.radioNormal);
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PopupTransSOP";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "다른 SOP로 전환";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PopupTransSOP_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PopupTransSOP_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PopupTransSOP_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.rdPictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdPictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.RadioButton radioNormal;
        private System.Windows.Forms.RadioButton radioAbnormal;
        private System.Windows.Forms.Label rdLabel2;
        private System.Windows.Forms.Label rdLabel1;
        private System.Windows.Forms.PictureBox rdPictureBox2;
		private System.Windows.Forms.PictureBox rdPictureBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private UnE.GUI.RibbonButton btnCancel;
        private UnE.GUI.RibbonButton btnOK;
        private UnE.GUI.RibbonButton ribbonButton1;
    }
}