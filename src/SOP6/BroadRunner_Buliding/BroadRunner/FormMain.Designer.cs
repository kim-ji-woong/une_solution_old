namespace BroadRunner
{
    partial class FormMain
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

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.checkBoxUseSiren = new System.Windows.Forms.CheckBox();
            this.plTitle = new System.Windows.Forms.Panel();
            this.lbTitle = new System.Windows.Forms.Label();
            this.btnCancle = new UnE.GUI.RibbonButton();
            this.pbTitle = new System.Windows.Forms.PictureBox();
            this.btnRun = new UnE.GUI.ImageButton();
            this.plTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRun)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBoxMessage.Location = new System.Drawing.Point(22, 82);
            this.textBoxMessage.Multiline = true;
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.textBoxMessage.Size = new System.Drawing.Size(555, 225);
            this.textBoxMessage.TabIndex = 0;
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Font = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelStatus.Location = new System.Drawing.Point(22, 315);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(82, 15);
            this.labelStatus.TabIndex = 2;
            this.labelStatus.Text = "방송서버 상태";
            this.labelStatus.Visible = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // checkBoxUseSiren
            // 
            this.checkBoxUseSiren.AutoSize = true;
            this.checkBoxUseSiren.Font = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxUseSiren.Location = new System.Drawing.Point(22, 351);
            this.checkBoxUseSiren.Name = "checkBoxUseSiren";
            this.checkBoxUseSiren.Size = new System.Drawing.Size(155, 19);
            this.checkBoxUseSiren.TabIndex = 3;
            this.checkBoxUseSiren.Text = "방송 시작시 사이렌 사용";
            this.checkBoxUseSiren.UseVisualStyleBackColor = true;
            // 
            // plTitle
            // 
            this.plTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(65)))), ((int)(((byte)(109)))));
            this.plTitle.Controls.Add(this.btnCancle);
            this.plTitle.Controls.Add(this.pbTitle);
            this.plTitle.Controls.Add(this.lbTitle);
            this.plTitle.Location = new System.Drawing.Point(0, 0);
            this.plTitle.Name = "plTitle";
            this.plTitle.Size = new System.Drawing.Size(600, 60);
            this.plTitle.TabIndex = 25;
            this.plTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plTitle_MouseDown);
            this.plTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plTitle_MouseMove);
            this.plTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.plTitle_MouseUp);
            // 
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(65)))), ((int)(((byte)(109)))));
            this.lbTitle.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbTitle.ForeColor = System.Drawing.Color.White;
            this.lbTitle.Location = new System.Drawing.Point(42, 20);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(82, 23);
            this.lbTitle.TabIndex = 40;
            this.lbTitle.Text = "시험방송";
            this.lbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseDown);
            this.lbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseMove);
            this.lbTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseUp);
            // 
            // btnCancle
            // 
            this.btnCancle.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(65)))), ((int)(((byte)(109)))));
            this.btnCancle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCancle.CheckButton = false;
            this.btnCancle.CheckedBkgndImage = null;
            this.btnCancle.CheckedImage = null;
            this.btnCancle.CheckedMouseOver = null;
            this.btnCancle.ClickedBackgroundImage = null;
            this.btnCancle.ClickedImage = global::BroadRunner.Properties.Resources.btnClose_Selected;
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
            this.btnCancle.Location = new System.Drawing.Point(560, 18);
            this.btnCancle.MouseOverBkgndImage = null;
            this.btnCancle.MouseOverImage = global::BroadRunner.Properties.Resources.btnClose_MouseOver;
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.NormalImage = global::BroadRunner.Properties.Resources.btnClose_Normal;
            this.btnCancle.Owner = null;
            this.btnCancle.Size = new System.Drawing.Size(22, 22);
            this.btnCancle.TabIndex = 110;
            this.btnCancle.TextLocation = new System.Drawing.Point(0, 0);
            this.btnCancle.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCancle.ToolTipText = "";
            this.btnCancle.UseCustomImageRect = false;
            this.btnCancle.UseTextLocation = false;
            this.btnCancle.UseVisualStyleBackColor = false;
            this.btnCancle.Click += new System.EventHandler(this.btnCancle_Click);
            // 
            // pbTitle
            // 
            this.pbTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.pbTitle.Location = new System.Drawing.Point(22, 28);
            this.pbTitle.Margin = new System.Windows.Forms.Padding(0);
            this.pbTitle.Name = "pbTitle";
            this.pbTitle.Size = new System.Drawing.Size(5, 5);
            this.pbTitle.TabIndex = 39;
            this.pbTitle.TabStop = false;
            this.pbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbTitle_MouseDown);
            this.pbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbTitle_MouseMove);
            this.pbTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbTitle_MouseUp);
            // 
            // btnRun
            // 
            this.btnRun.BackColor = System.Drawing.SystemColors.Control;
            this.btnRun.ButtonText = "방송 실행";
            this.btnRun.ImageClicked = global::BroadRunner.Properties.Resources.btnRun_Selected;
            this.btnRun.ImageDisabled = global::BroadRunner.Properties.Resources.btnRun_Disabled;
            this.btnRun.ImageMouseOver = global::BroadRunner.Properties.Resources.btnRun_Selected;
            this.btnRun.ImageNormal = global::BroadRunner.Properties.Resources.btnRun_Normal;
            this.btnRun.Location = new System.Drawing.Point(465, 345);
            this.btnRun.Name = "btnRun";
            this.btnRun.Owner = null;
            this.btnRun.Size = new System.Drawing.Size(112, 33);
            this.btnRun.TabIndex = 24;
            this.btnRun.TabStop = false;
            this.btnRun.Text = "방송 실행";
            this.btnRun.TextColor = System.Drawing.Color.White;
            this.btnRun.TextFont = new System.Drawing.Font("나눔스퀘어", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRun.ToolTipText = "";
            this.btnRun.UseToolTip = false;
            this.btnRun.WindowRateWidth = 1F;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 414);
            this.Controls.Add(this.plTitle);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.checkBoxUseSiren);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.textBoxMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "시험방송";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.plTitle.ResumeLayout(false);
            this.plTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRun)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox checkBoxUseSiren;
        private UnE.GUI.ImageButton btnRun;
        private System.Windows.Forms.Panel plTitle;
        private UnE.GUI.RibbonButton btnCancle;
        private System.Windows.Forms.PictureBox pbTitle;
        private System.Windows.Forms.Label lbTitle;
    }
}

