namespace SOPBulletin
{
    partial class FormMain2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain2));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panelStatusBottom = new System.Windows.Forms.Panel();
            this.panelStatusTop = new System.Windows.Forms.Panel();
            this.rbtnShowPrevLog = new UnE.GUI.RibbonButton();
            this.rbtnCloseCurrentLog = new UnE.GUI.RibbonButton();
            this.rbtnSaveToHWP = new UnE.GUI.RibbonButton();
            this.label2 = new System.Windows.Forms.Label();
            this.panelProgressBottom = new System.Windows.Forms.Panel();
            this.panelProgressTop = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panelStatusTop.SuspendLayout();
            this.panelProgressTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.OnTimer);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panelStatusBottom);
            this.splitContainer1.Panel1.Controls.Add(this.panelStatusTop);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panelProgressBottom);
            this.splitContainer1.Panel2.Controls.Add(this.panelProgressTop);
            this.splitContainer1.Size = new System.Drawing.Size(942, 529);
            this.splitContainer1.SplitterDistance = 413;
            this.splitContainer1.TabIndex = 1;
            // 
            // panelStatusBottom
            // 
            this.panelStatusBottom.BackColor = System.Drawing.Color.White;
            this.panelStatusBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelStatusBottom.Location = new System.Drawing.Point(0, 50);
            this.panelStatusBottom.Name = "panelStatusBottom";
            this.panelStatusBottom.Size = new System.Drawing.Size(940, 361);
            this.panelStatusBottom.TabIndex = 0;
            // 
            // panelStatusTop
            // 
            this.panelStatusTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(124)))));
            this.panelStatusTop.Controls.Add(this.rbtnShowPrevLog);
            this.panelStatusTop.Controls.Add(this.rbtnCloseCurrentLog);
            this.panelStatusTop.Controls.Add(this.rbtnSaveToHWP);
            this.panelStatusTop.Controls.Add(this.label2);
            this.panelStatusTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelStatusTop.Location = new System.Drawing.Point(0, 0);
            this.panelStatusTop.Name = "panelStatusTop";
            this.panelStatusTop.Size = new System.Drawing.Size(940, 50);
            this.panelStatusTop.TabIndex = 0;
            // 
            // rbtnShowPrevLog
            // 
            this.rbtnShowPrevLog.BackColor = System.Drawing.Color.Transparent;
            this.rbtnShowPrevLog.CheckButton = false;
            this.rbtnShowPrevLog.CheckedBkgndImage = null;
            this.rbtnShowPrevLog.CheckedImage = null;
            this.rbtnShowPrevLog.CheckedMouseOver = null;
            this.rbtnShowPrevLog.ClickedBackgroundImage = null;
            this.rbtnShowPrevLog.ClickedImage = global::SOPBulletin.Properties.Resources.MenuButton_Selected;
            this.rbtnShowPrevLog.CustomImageRect = new System.Drawing.Rectangle(0, 0, 145, 40);
            this.rbtnShowPrevLog.DisabledBkgndImage = null;
            this.rbtnShowPrevLog.DisabledImage = global::SOPBulletin.Properties.Resources.MenuButton_Disabled;
            this.rbtnShowPrevLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.rbtnShowPrevLog.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnShowPrevLog.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnShowPrevLog.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.rbtnShowPrevLog.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.rbtnShowPrevLog.ForeColorsByTypeUse = false;
            this.rbtnShowPrevLog.ID = -1;
            this.rbtnShowPrevLog.InitButtonWidth = 145;
            this.rbtnShowPrevLog.IsChecked = false;
            this.rbtnShowPrevLog.Location = new System.Drawing.Point(457, 5);
            this.rbtnShowPrevLog.MouseOverBkgndImage = null;
            this.rbtnShowPrevLog.MouseOverImage = global::SOPBulletin.Properties.Resources.MenuButton_MouseOver;
            this.rbtnShowPrevLog.Name = "rbtnShowPrevLog";
            this.rbtnShowPrevLog.NormalImage = global::SOPBulletin.Properties.Resources.MenuButton_Normal;
            this.rbtnShowPrevLog.Owner = null;
            this.rbtnShowPrevLog.Size = new System.Drawing.Size(145, 40);
            this.rbtnShowPrevLog.TabIndex = 2;
            this.rbtnShowPrevLog.Text = "이전 SOP 로그 보기";
            this.rbtnShowPrevLog.TextLocation = new System.Drawing.Point(0, 11);
            this.rbtnShowPrevLog.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnShowPrevLog.ToolTipText = "이전 SOP 로그 보기";
            this.rbtnShowPrevLog.UseCustomImageRect = true;
            this.rbtnShowPrevLog.UseTextLocation = true;
            this.rbtnShowPrevLog.UseVisualStyleBackColor = false;
            this.rbtnShowPrevLog.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnShowPrevLog_MouseDown);
            // 
            // rbtnCloseCurrentLog
            // 
            this.rbtnCloseCurrentLog.BackColor = System.Drawing.Color.Transparent;
            this.rbtnCloseCurrentLog.CheckButton = false;
            this.rbtnCloseCurrentLog.CheckedBkgndImage = null;
            this.rbtnCloseCurrentLog.CheckedImage = null;
            this.rbtnCloseCurrentLog.CheckedMouseOver = null;
            this.rbtnCloseCurrentLog.ClickedBackgroundImage = null;
            this.rbtnCloseCurrentLog.ClickedImage = global::SOPBulletin.Properties.Resources.MenuButton_Selected;
            this.rbtnCloseCurrentLog.CustomImageRect = new System.Drawing.Rectangle(0, 0, 145, 40);
            this.rbtnCloseCurrentLog.DisabledBkgndImage = null;
            this.rbtnCloseCurrentLog.DisabledImage = global::SOPBulletin.Properties.Resources.MenuButton_Disabled;
            this.rbtnCloseCurrentLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.rbtnCloseCurrentLog.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnCloseCurrentLog.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnCloseCurrentLog.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.rbtnCloseCurrentLog.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.rbtnCloseCurrentLog.ForeColorsByTypeUse = false;
            this.rbtnCloseCurrentLog.ID = -1;
            this.rbtnCloseCurrentLog.InitButtonWidth = 145;
            this.rbtnCloseCurrentLog.IsChecked = false;
            this.rbtnCloseCurrentLog.Location = new System.Drawing.Point(306, 5);
            this.rbtnCloseCurrentLog.MouseOverBkgndImage = null;
            this.rbtnCloseCurrentLog.MouseOverImage = global::SOPBulletin.Properties.Resources.MenuButton_MouseOver;
            this.rbtnCloseCurrentLog.Name = "rbtnCloseCurrentLog";
            this.rbtnCloseCurrentLog.NormalImage = global::SOPBulletin.Properties.Resources.MenuButton_Normal;
            this.rbtnCloseCurrentLog.Owner = null;
            this.rbtnCloseCurrentLog.Size = new System.Drawing.Size(145, 40);
            this.rbtnCloseCurrentLog.TabIndex = 2;
            this.rbtnCloseCurrentLog.Text = "현재 SOP 로그 닫기";
            this.rbtnCloseCurrentLog.TextLocation = new System.Drawing.Point(0, 11);
            this.rbtnCloseCurrentLog.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnCloseCurrentLog.ToolTipText = "현재 SOP 로그 닫기";
            this.rbtnCloseCurrentLog.UseCustomImageRect = true;
            this.rbtnCloseCurrentLog.UseTextLocation = true;
            this.rbtnCloseCurrentLog.UseVisualStyleBackColor = false;
            this.rbtnCloseCurrentLog.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnCloseCurrentLog_MouseDown);
            // 
            // rbtnSaveToHWP
            // 
            this.rbtnSaveToHWP.BackColor = System.Drawing.Color.Transparent;
            this.rbtnSaveToHWP.CheckButton = false;
            this.rbtnSaveToHWP.CheckedBkgndImage = null;
            this.rbtnSaveToHWP.CheckedImage = null;
            this.rbtnSaveToHWP.CheckedMouseOver = null;
            this.rbtnSaveToHWP.ClickedBackgroundImage = null;
            this.rbtnSaveToHWP.ClickedImage = global::SOPBulletin.Properties.Resources.MenuButton_Selected;
            this.rbtnSaveToHWP.CustomImageRect = new System.Drawing.Rectangle(0, 0, 145, 40);
            this.rbtnSaveToHWP.DisabledBkgndImage = null;
            this.rbtnSaveToHWP.DisabledImage = global::SOPBulletin.Properties.Resources.MenuButton_Disabled;
            this.rbtnSaveToHWP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.rbtnSaveToHWP.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnSaveToHWP.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnSaveToHWP.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.rbtnSaveToHWP.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(210)))), ((int)(((byte)(250)))));
            this.rbtnSaveToHWP.ForeColorsByTypeUse = false;
            this.rbtnSaveToHWP.ID = -1;
            this.rbtnSaveToHWP.InitButtonWidth = 145;
            this.rbtnSaveToHWP.IsChecked = false;
            this.rbtnSaveToHWP.Location = new System.Drawing.Point(155, 5);
            this.rbtnSaveToHWP.MouseOverBkgndImage = null;
            this.rbtnSaveToHWP.MouseOverImage = global::SOPBulletin.Properties.Resources.MenuButton_MouseOver;
            this.rbtnSaveToHWP.Name = "rbtnSaveToHWP";
            this.rbtnSaveToHWP.NormalImage = global::SOPBulletin.Properties.Resources.MenuButton_Normal;
            this.rbtnSaveToHWP.Owner = null;
            this.rbtnSaveToHWP.Size = new System.Drawing.Size(145, 40);
            this.rbtnSaveToHWP.TabIndex = 2;
            this.rbtnSaveToHWP.Text = "한글 파일 보기";
            this.rbtnSaveToHWP.TextLocation = new System.Drawing.Point(0, 11);
            this.rbtnSaveToHWP.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnSaveToHWP.ToolTipText = "한글 파일 보기";
            this.rbtnSaveToHWP.UseCustomImageRect = true;
            this.rbtnSaveToHWP.UseTextLocation = true;
            this.rbtnSaveToHWP.UseVisualStyleBackColor = false;
            this.rbtnSaveToHWP.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnSaveToHWP_MouseDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("나눔바른고딕", 15.75F);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(9, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(131, 24);
            this.label2.TabIndex = 0;
            this.label2.Text = "SOP 진행이력";
            // 
            // panelProgressBottom
            // 
            this.panelProgressBottom.BackColor = System.Drawing.Color.White;
            this.panelProgressBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelProgressBottom.Location = new System.Drawing.Point(0, 20);
            this.panelProgressBottom.Name = "panelProgressBottom";
            this.panelProgressBottom.Size = new System.Drawing.Size(940, 90);
            this.panelProgressBottom.TabIndex = 1;
            // 
            // panelProgressTop
            // 
            this.panelProgressTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(124)))));
            this.panelProgressTop.Controls.Add(this.label1);
            this.panelProgressTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelProgressTop.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panelProgressTop.Location = new System.Drawing.Point(0, 0);
            this.panelProgressTop.Name = "panelProgressTop";
            this.panelProgressTop.Size = new System.Drawing.Size(940, 20);
            this.panelProgressTop.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "SOP 진행률";
            // 
            // FormMain2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(942, 529);
            this.Controls.Add(this.splitContainer1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain2";
            this.Text = "SOP 상황판";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain2_FormClosing);
            this.Load += new System.EventHandler(this.FormMain2_Load);
            this.VisibleChanged += new System.EventHandler(this.FormMain2_VisibleChanged);
            this.Resize += new System.EventHandler(this.FormMain2_Resize);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panelStatusTop.ResumeLayout(false);
            this.panelStatusTop.PerformLayout();
            this.panelProgressTop.ResumeLayout(false);
            this.panelProgressTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panelStatusBottom;
        private System.Windows.Forms.Panel panelStatusTop;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelProgressBottom;
        private System.Windows.Forms.Panel panelProgressTop;
        private System.Windows.Forms.Label label1;
        private UnE.GUI.RibbonButton rbtnSaveToHWP;
        private UnE.GUI.RibbonButton rbtnShowPrevLog;
        private UnE.GUI.RibbonButton rbtnCloseCurrentLog;
    }
}