namespace SMSSender
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.textBoxSender = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textboxReciver = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textboxContent = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lableLength = new System.Windows.Forms.Label();
            this.plTitle = new System.Windows.Forms.Panel();
            this.btnCancle = new UnE.GUI.RibbonButton();
            this.pbTitle = new System.Windows.Forms.PictureBox();
            this.lbTitle = new System.Windows.Forms.Label();
            this.btnMsgSend = new UnE.GUI.ImageButton();
            this.btnMsgClear = new UnE.GUI.ImageButton();
            this.btnAddReciver = new UnE.GUI.ImageButton();
            this.btnClearReciver = new UnE.GUI.ImageButton();
            this.plTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMsgSend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMsgClear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAddReciver)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClearReciver)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxSender
            // 
            this.textBoxSender.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxSender.Location = new System.Drawing.Point(93, 82);
            this.textBoxSender.Name = "textBoxSender";
            this.textBoxSender.Size = new System.Drawing.Size(266, 26);
            this.textBoxSender.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(22, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "발신자";
            // 
            // textboxReciver
            // 
            this.textboxReciver.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textboxReciver.Location = new System.Drawing.Point(93, 131);
            this.textboxReciver.Name = "textboxReciver";
            this.textboxReciver.Size = new System.Drawing.Size(266, 26);
            this.textboxReciver.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(22, 139);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 19);
            this.label2.TabIndex = 3;
            this.label2.Text = "수신자";
            // 
            // textboxContent
            // 
            this.textboxContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxContent.Location = new System.Drawing.Point(93, 225);
            this.textboxContent.Multiline = true;
            this.textboxContent.Name = "textboxContent";
            this.textboxContent.Size = new System.Drawing.Size(266, 150);
            this.textboxContent.TabIndex = 2;
            this.textboxContent.TextChanged += new System.EventHandler(this.textboxContent_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(30, 233);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 19);
            this.label3.TabIndex = 5;
            this.label3.Text = "내용";
            // 
            // lableLength
            // 
            this.lableLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lableLength.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lableLength.Location = new System.Drawing.Point(215, 380);
            this.lableLength.Name = "lableLength";
            this.lableLength.Size = new System.Drawing.Size(144, 18);
            this.lableLength.TabIndex = 14;
            this.lableLength.Text = "0/80 바이트";
            this.lableLength.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // plTitle
            // 
            this.plTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(65)))), ((int)(((byte)(109)))));
            this.plTitle.Controls.Add(this.btnCancle);
            this.plTitle.Controls.Add(this.pbTitle);
            this.plTitle.Controls.Add(this.lbTitle);
            this.plTitle.Location = new System.Drawing.Point(0, 0);
            this.plTitle.Name = "plTitle";
            this.plTitle.Size = new System.Drawing.Size(375, 60);
            this.plTitle.TabIndex = 19;
            this.plTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plTitle_MouseDown);
            this.plTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plTitle_MouseMove);
            this.plTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.plTitle_MouseUp);
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
            this.btnCancle.ClickedImage = global::SMSSender.Properties.Resources.btnClose_Selected;
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
            this.btnCancle.Location = new System.Drawing.Point(340, 18);
            this.btnCancle.MouseOverBkgndImage = null;
            this.btnCancle.MouseOverImage = global::SMSSender.Properties.Resources.btnClose_MouseOver;
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.NormalImage = global::SMSSender.Properties.Resources.btnClose_Normal;
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
            this.lbTitle.Text = "문자전송";
            this.lbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseDown);
            this.lbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseMove);
            this.lbTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseUp);
            // 
            // btnMsgSend
            // 
            this.btnMsgSend.BackColor = System.Drawing.SystemColors.Control;
            this.btnMsgSend.ButtonText = "전송하기";
            this.btnMsgSend.ImageClicked = global::SMSSender.Properties.Resources.button_Selected;
            this.btnMsgSend.ImageDisabled = null;
            this.btnMsgSend.ImageMouseOver = global::SMSSender.Properties.Resources.button_MouseOver;
            this.btnMsgSend.ImageNormal = global::SMSSender.Properties.Resources.btnSend_Normal;
            this.btnMsgSend.Location = new System.Drawing.Point(247, 408);
            this.btnMsgSend.Name = "btnMsgSend";
            this.btnMsgSend.Owner = null;
            this.btnMsgSend.Size = new System.Drawing.Size(112, 33);
            this.btnMsgSend.TabIndex = 23;
            this.btnMsgSend.TabStop = false;
            this.btnMsgSend.Text = "전송하기";
            this.btnMsgSend.TextColor = System.Drawing.Color.White;
            this.btnMsgSend.TextFont = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnMsgSend.ToolTipText = "";
            this.btnMsgSend.UseToolTip = false;
            this.btnMsgSend.WindowRateWidth = 1F;
            this.btnMsgSend.Click += new System.EventHandler(this.btnMsgSend_Click);
            // 
            // btnMsgClear
            // 
            this.btnMsgClear.BackColor = System.Drawing.SystemColors.Control;
            this.btnMsgClear.ButtonText = "내용 지우기";
            this.btnMsgClear.ImageClicked = global::SMSSender.Properties.Resources.btnDelete_Selected;
            this.btnMsgClear.ImageDisabled = null;
            this.btnMsgClear.ImageMouseOver = global::SMSSender.Properties.Resources.button_MouseOver;
            this.btnMsgClear.ImageNormal = global::SMSSender.Properties.Resources.btnDelete_Normal;
            this.btnMsgClear.Location = new System.Drawing.Point(127, 408);
            this.btnMsgClear.Name = "btnMsgClear";
            this.btnMsgClear.Owner = null;
            this.btnMsgClear.Size = new System.Drawing.Size(112, 33);
            this.btnMsgClear.TabIndex = 22;
            this.btnMsgClear.TabStop = false;
            this.btnMsgClear.Text = "내용 지우기";
            this.btnMsgClear.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.btnMsgClear.TextFont = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnMsgClear.ToolTipText = "";
            this.btnMsgClear.UseToolTip = false;
            this.btnMsgClear.WindowRateWidth = 1F;
            this.btnMsgClear.Click += new System.EventHandler(this.btnMsgClear_Click);
            // 
            // btnAddReciver
            // 
            this.btnAddReciver.ButtonText = "수신자 추가";
            this.btnAddReciver.ImageClicked = global::SMSSender.Properties.Resources.button_Selected;
            this.btnAddReciver.ImageDisabled = null;
            this.btnAddReciver.ImageMouseOver = global::SMSSender.Properties.Resources.button_MouseOver;
            this.btnAddReciver.ImageNormal = global::SMSSender.Properties.Resources.button_Normal;
            this.btnAddReciver.Location = new System.Drawing.Point(231, 180);
            this.btnAddReciver.Name = "btnAddReciver";
            this.btnAddReciver.Owner = null;
            this.btnAddReciver.Size = new System.Drawing.Size(127, 33);
            this.btnAddReciver.TabIndex = 21;
            this.btnAddReciver.TabStop = false;
            this.btnAddReciver.Text = "수신자 추가";
            this.btnAddReciver.TextColor = System.Drawing.Color.White;
            this.btnAddReciver.TextFont = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnAddReciver.ToolTipText = "";
            this.btnAddReciver.UseToolTip = false;
            this.btnAddReciver.WindowRateWidth = 1F;
            this.btnAddReciver.Click += new System.EventHandler(this.btnAddReciver_Click);
            // 
            // btnClearReciver
            // 
            this.btnClearReciver.ButtonText = "수신자 지우기";
            this.btnClearReciver.ImageClicked = global::SMSSender.Properties.Resources.button_Selected;
            this.btnClearReciver.ImageDisabled = null;
            this.btnClearReciver.ImageMouseOver = global::SMSSender.Properties.Resources.button_MouseOver;
            this.btnClearReciver.ImageNormal = global::SMSSender.Properties.Resources.button_Normal;
            this.btnClearReciver.Location = new System.Drawing.Point(93, 180);
            this.btnClearReciver.Name = "btnClearReciver";
            this.btnClearReciver.Owner = null;
            this.btnClearReciver.Size = new System.Drawing.Size(127, 33);
            this.btnClearReciver.TabIndex = 20;
            this.btnClearReciver.TabStop = false;
            this.btnClearReciver.Text = "수신자 지우기";
            this.btnClearReciver.TextColor = System.Drawing.Color.White;
            this.btnClearReciver.TextFont = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClearReciver.ToolTipText = "";
            this.btnClearReciver.UseToolTip = false;
            this.btnClearReciver.WindowRateWidth = 1F;
            this.btnClearReciver.Click += new System.EventHandler(this.btnClearReciver_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 468);
            this.Controls.Add(this.btnMsgSend);
            this.Controls.Add(this.btnMsgClear);
            this.Controls.Add(this.btnAddReciver);
            this.Controls.Add(this.btnClearReciver);
            this.Controls.Add(this.plTitle);
            this.Controls.Add(this.lableLength);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textboxContent);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textboxReciver);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxSender);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(500, 625);
            this.MinimumSize = new System.Drawing.Size(320, 350);
            this.Name = "FormMain";
            this.Text = "문자전송";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.plTitle.ResumeLayout(false);
            this.plTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMsgSend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMsgClear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAddReciver)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClearReciver)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxSender;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textboxReciver;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textboxContent;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lableLength;
        private System.Windows.Forms.Panel plTitle;
        private System.Windows.Forms.PictureBox pbTitle;
        private System.Windows.Forms.Label lbTitle;
        private UnE.GUI.ImageButton btnClearReciver;
        private UnE.GUI.ImageButton btnAddReciver;
        private UnE.GUI.ImageButton btnMsgClear;
        private UnE.GUI.ImageButton btnMsgSend;
        private UnE.GUI.RibbonButton btnCancle;
    }
}

