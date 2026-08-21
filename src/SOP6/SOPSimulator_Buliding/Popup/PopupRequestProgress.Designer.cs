namespace SOPMonitoringSystem
{
    partial class PopupRequestProgress
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
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button1 = new UnE.GUI.RibbonButton();
            this.btnCancel = new UnE.GUI.RibbonButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("나눔스퀘어", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(102, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(168, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "제어권 요청중...";
            // 
            // progressBar1
            // 
            this.progressBar1.BackColor = System.Drawing.Color.Black;
            this.progressBar1.Location = new System.Drawing.Point(20, 120);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(320, 18);
            this.progressBar1.TabIndex = 1;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.button1.CheckButton = false;
            this.button1.CheckedBkgndImage = null;
            this.button1.CheckedImage = null;
            this.button1.CheckedMouseOver = null;
            this.button1.ClickedBackgroundImage = null;
            this.button1.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.PopupRequestProgressButton_Selected;
            this.button1.CustomImageRect = new System.Drawing.Rectangle(0, 0, 150, 40);
            this.button1.DisabledBkgndImage = null;
            this.button1.DisabledImage = null;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.ForeColor = System.Drawing.Color.Black;
            this.button1.ForeColorChecked = System.Drawing.Color.Black;
            this.button1.ForeColorCheckedMouseOver = System.Drawing.Color.Black;
            this.button1.ForeColorDisabled = System.Drawing.Color.Black;
            this.button1.ForeColorMouseOver = System.Drawing.Color.Black;
            this.button1.ForeColorsByTypeUse = false;
            this.button1.ID = -1;
            this.button1.InitButtonWidth = 150;
            this.button1.IsChecked = false;
            this.button1.Location = new System.Drawing.Point(20, 180);
            this.button1.MouseOverBkgndImage = null;
            this.button1.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.PopupRequestProgressButton_Mouseover;
            this.button1.Name = "button1";
            this.button1.NormalImage = global::SOPMonitoringSystem.Properties.Resources.PopupRequestProgressButton_Normal;
            this.button1.Owner = null;
            this.button1.Size = new System.Drawing.Size(150, 40);
            this.button1.TabIndex = 55;
            this.button1.Text = "제어권 강제 회수";
            this.button1.TextLocation = new System.Drawing.Point(14, 10);
            this.button1.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.button1.ToolTipText = "제어권 강제 회수";
            this.button1.UseCustomImageRect = true;
            this.button1.UseTextLocation = true;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.btnCancel.CheckButton = false;
            this.btnCancel.CheckedBkgndImage = null;
            this.btnCancel.CheckedImage = null;
            this.btnCancel.CheckedMouseOver = null;
            this.btnCancel.ClickedBackgroundImage = null;
            this.btnCancel.ClickedImage = global::SOPMonitoringSystem.Properties.Resources.PopupRequestProgressButton_Selected;
            this.btnCancel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 150, 40);
            this.btnCancel.DisabledBkgndImage = null;
            this.btnCancel.DisabledImage = null;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.ForeColorChecked = System.Drawing.Color.Black;
            this.btnCancel.ForeColorCheckedMouseOver = System.Drawing.Color.Black;
            this.btnCancel.ForeColorDisabled = System.Drawing.Color.Black;
            this.btnCancel.ForeColorMouseOver = System.Drawing.Color.Black;
            this.btnCancel.ForeColorsByTypeUse = false;
            this.btnCancel.ID = -1;
            this.btnCancel.InitButtonWidth = 150;
            this.btnCancel.IsChecked = false;
            this.btnCancel.Location = new System.Drawing.Point(190, 180);
            this.btnCancel.MouseOverBkgndImage = null;
            this.btnCancel.MouseOverImage = global::SOPMonitoringSystem.Properties.Resources.PopupRequestProgressButton_Mouseover;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.NormalImage = global::SOPMonitoringSystem.Properties.Resources.PopupRequestProgressButton_Normal;
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(150, 40);
            this.btnCancel.TabIndex = 56;
            this.btnCancel.Text = "요청 취소";
            this.btnCancel.TextLocation = new System.Drawing.Point(40, 10);
            this.btnCancel.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnCancel.ToolTipText = "요청 취소";
            this.btnCancel.UseCustomImageRect = true;
            this.btnCancel.UseTextLocation = true;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // PopupRequestProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.PopupRequestProgress_NewBkgnd;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(360, 270);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "PopupRequestProgress";
            this.Text = "제어권 요청중입니다";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PopupRequestProgress_FormClosing);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PopupRequestProgress_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PopupRequestProgress_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PopupRequestProgress_MouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Timer timer1;
        private UnE.GUI.RibbonButton button1;
        private UnE.GUI.RibbonButton btnCancel;
    }
}