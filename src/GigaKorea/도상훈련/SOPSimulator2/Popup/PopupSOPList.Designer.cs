namespace SOPMonitoringSystem
{
    partial class PopupSOPList
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioNormal = new System.Windows.Forms.RadioButton();
            this.radioAbnormal = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.treeView = new System.Windows.Forms.TreeView();
            this.btnLoadSOP = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.rTextBoxSOPInfo = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioRegular = new System.Windows.Forms.RadioButton();
            this.radioNonRegular = new System.Windows.Forms.RadioButton();
            this.btnShowSOPManual = new System.Windows.Forms.Button();
            this.btnNext = new UnE.GUI.RibbonButton();
            this.btnPrev = new UnE.GUI.RibbonButton();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioNormal);
            this.groupBox3.Controls.Add(this.radioAbnormal);
            this.groupBox3.Location = new System.Drawing.Point(14, 48);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(257, 39);
            this.groupBox3.TabIndex = 37;
            this.groupBox3.TabStop = false;
            // 
            // radioNormal
            // 
            this.radioNormal.AutoSize = true;
            this.radioNormal.Location = new System.Drawing.Point(13, 16);
            this.radioNormal.Name = "radioNormal";
            this.radioNormal.Size = new System.Drawing.Size(75, 16);
            this.radioNormal.TabIndex = 6;
            this.radioNormal.TabStop = true;
            this.radioNormal.Text = "평일 모드";
            this.radioNormal.UseVisualStyleBackColor = true;
            this.radioNormal.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // radioAbnormal
            // 
            this.radioAbnormal.AutoSize = true;
            this.radioAbnormal.Location = new System.Drawing.Point(118, 16);
            this.radioAbnormal.Name = "radioAbnormal";
            this.radioAbnormal.Size = new System.Drawing.Size(119, 16);
            this.radioAbnormal.TabIndex = 5;
            this.radioAbnormal.Text = "야간 및 휴일 모드";
            this.radioAbnormal.UseVisualStyleBackColor = true;
            this.radioAbnormal.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(10, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(188, 30);
            this.label1.TabIndex = 38;
            this.label1.Text = "시나리오 불러오기";
            // 
            // treeView
            // 
            this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView.Location = new System.Drawing.Point(15, 128);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(671, 483);
            this.treeView.TabIndex = 39;
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // btnLoadSOP
            // 
            this.btnLoadSOP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadSOP.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnLoadSOP.Location = new System.Drawing.Point(694, 580);
            this.btnLoadSOP.Name = "btnLoadSOP";
            this.btnLoadSOP.Size = new System.Drawing.Size(179, 31);
            this.btnLoadSOP.TabIndex = 44;
            this.btnLoadSOP.Text = "시나리오 열기";
            this.btnLoadSOP.UseVisualStyleBackColor = true;
            this.btnLoadSOP.Click += new System.EventHandler(this.btnLoadSOP_Click);
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.Location = new System.Drawing.Point(692, 113);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 12);
            this.label6.TabIndex = 40;
            this.label6.Text = "시나리오 정보";
            // 
            // rTextBoxSOPInfo
            // 
            this.rTextBoxSOPInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rTextBoxSOPInfo.BackColor = System.Drawing.Color.White;
            this.rTextBoxSOPInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rTextBoxSOPInfo.Location = new System.Drawing.Point(694, 128);
            this.rTextBoxSOPInfo.Name = "rTextBoxSOPInfo";
            this.rTextBoxSOPInfo.ReadOnly = true;
            this.rTextBoxSOPInfo.Size = new System.Drawing.Size(179, 441);
            this.rTextBoxSOPInfo.TabIndex = 45;
            this.rTextBoxSOPInfo.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioRegular);
            this.groupBox1.Controls.Add(this.radioNonRegular);
            this.groupBox1.Location = new System.Drawing.Point(296, 48);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(257, 39);
            this.groupBox1.TabIndex = 37;
            this.groupBox1.TabStop = false;
            this.groupBox1.Visible = false;
            // 
            // radioRegular
            // 
            this.radioRegular.AutoSize = true;
            this.radioRegular.Location = new System.Drawing.Point(13, 16);
            this.radioRegular.Name = "radioRegular";
            this.radioRegular.Size = new System.Drawing.Size(75, 16);
            this.radioRegular.TabIndex = 6;
            this.radioRegular.TabStop = true;
            this.radioRegular.Text = "등록 모드";
            this.radioRegular.UseVisualStyleBackColor = true;
            this.radioRegular.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // radioNonRegular
            // 
            this.radioNonRegular.AutoSize = true;
            this.radioNonRegular.Location = new System.Drawing.Point(118, 16);
            this.radioNonRegular.Name = "radioNonRegular";
            this.radioNonRegular.Size = new System.Drawing.Size(87, 16);
            this.radioNonRegular.TabIndex = 5;
            this.radioNonRegular.Text = "미등록 모드";
            this.radioNonRegular.UseVisualStyleBackColor = true;
            this.radioNonRegular.CheckedChanged += new System.EventHandler(this.radioMode_CheckedChanged);
            // 
            // btnShowSOPManual
            // 
            this.btnShowSOPManual.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShowSOPManual.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnShowSOPManual.Location = new System.Drawing.Point(693, 618);
            this.btnShowSOPManual.Name = "btnShowSOPManual";
            this.btnShowSOPManual.Size = new System.Drawing.Size(179, 31);
            this.btnShowSOPManual.TabIndex = 44;
            this.btnShowSOPManual.Text = "SOP 문서 파일 보기";
            this.btnShowSOPManual.UseVisualStyleBackColor = true;
            this.btnShowSOPManual.Visible = false;
            this.btnShowSOPManual.Click += new System.EventHandler(this.btnShowSOPManual_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNext.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.go;
            this.btnNext.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnNext.CheckButton = false;
            this.btnNext.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.go_click;
            this.btnNext.CheckedImage = null;
            this.btnNext.CheckedMouseOver = null;
            this.btnNext.ClickedBackgroundImage = null;
            this.btnNext.ClickedImage = null;
            this.btnNext.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnNext.DisabledBkgndImage = global::SOPMonitoringSystem.Properties.Resources.go_disable;
            this.btnNext.DisabledImage = null;
            this.btnNext.ForeColorChecked = System.Drawing.Color.White;
            this.btnNext.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnNext.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnNext.ForeColorsByTypeUse = false;
            this.btnNext.ID = -1;
            this.btnNext.InitButtonWidth = 60;
            this.btnNext.IsChecked = false;
            this.btnNext.Location = new System.Drawing.Point(95, 618);
            this.btnNext.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.go_over;
            this.btnNext.MouseOverImage = null;
            this.btnNext.Name = "btnNext";
            this.btnNext.NormalImage = null;
            this.btnNext.Owner = null;
            this.btnNext.Size = new System.Drawing.Size(60, 31);
            this.btnNext.TabIndex = 47;
            this.btnNext.TextLocation = new System.Drawing.Point(0, 0);
            this.btnNext.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnNext.ToolTipText = "";
            this.btnNext.UseCustomImageRect = false;
            this.btnNext.UseTextLocation = false;
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnPrevNext_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrev.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.back;
            this.btnPrev.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnPrev.CheckButton = false;
            this.btnPrev.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.back_click;
            this.btnPrev.CheckedImage = null;
            this.btnPrev.CheckedMouseOver = null;
            this.btnPrev.ClickedBackgroundImage = null;
            this.btnPrev.ClickedImage = null;
            this.btnPrev.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnPrev.DisabledBkgndImage = global::SOPMonitoringSystem.Properties.Resources.back_disable;
            this.btnPrev.DisabledImage = null;
            this.btnPrev.ForeColorChecked = System.Drawing.Color.White;
            this.btnPrev.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnPrev.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnPrev.ForeColorsByTypeUse = false;
            this.btnPrev.ID = -1;
            this.btnPrev.InitButtonWidth = 60;
            this.btnPrev.IsChecked = false;
            this.btnPrev.Location = new System.Drawing.Point(14, 618);
            this.btnPrev.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.back_over;
            this.btnPrev.MouseOverImage = null;
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.NormalImage = null;
            this.btnPrev.Owner = null;
            this.btnPrev.Size = new System.Drawing.Size(60, 31);
            this.btnPrev.TabIndex = 46;
            this.btnPrev.TextLocation = new System.Drawing.Point(0, 0);
            this.btnPrev.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnPrev.ToolTipText = "";
            this.btnPrev.UseCustomImageRect = false;
            this.btnPrev.UseTextLocation = false;
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrevNext_Click);
            // 
            // PopupSOPList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 661);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.rTextBoxSOPInfo);
            this.Controls.Add(this.btnShowSOPManual);
            this.Controls.Add(this.btnLoadSOP);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PopupSOPList";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "열기";
            this.Load += new System.EventHandler(this.PopupSOPList_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioNormal;
        private System.Windows.Forms.RadioButton radioAbnormal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Button btnLoadSOP;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RichTextBox rTextBoxSOPInfo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioRegular;
        private System.Windows.Forms.RadioButton radioNonRegular;
        private System.Windows.Forms.Button btnShowSOPManual;
        private UnE.GUI.RibbonButton btnNext;
        private UnE.GUI.RibbonButton btnPrev;
    }
}