namespace InternalMessagePopup
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.pictureBoxWorkType = new System.Windows.Forms.PictureBox();
            this.labelReceiver = new System.Windows.Forms.Label();
            this.labelReceiverName = new System.Windows.Forms.Label();
            this.labelWorkType = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWorkType)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Enabled = false;
            this.textBoxMessage.Font = new System.Drawing.Font("맑은 고딕", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxMessage.Location = new System.Drawing.Point(473, 54);
            this.textBoxMessage.Multiline = true;
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.Size = new System.Drawing.Size(944, 590);
            this.textBoxMessage.TabIndex = 3;
            this.textBoxMessage.Text = "지금은 훈련상황입니다.\r\n다시한번 알려드립니다.\r\n지금은 훈련상황입니다.\r\n\r\n현재 지진이 발생하였습니다.";
            this.textBoxMessage.EnabledChanged += new System.EventHandler(this.textBoxMessage_EnabledChanged);
            // 
            // pictureBoxWorkType
            // 
            this.pictureBoxWorkType.BackgroundImage = global::InternalMessagePopup.Properties.Resources.sms;
            this.pictureBoxWorkType.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBoxWorkType.Location = new System.Drawing.Point(61, 54);
            this.pictureBoxWorkType.Name = "pictureBoxWorkType";
            this.pictureBoxWorkType.Size = new System.Drawing.Size(256, 256);
            this.pictureBoxWorkType.TabIndex = 0;
            this.pictureBoxWorkType.TabStop = false;
            // 
            // labelReceiver
            // 
            this.labelReceiver.AutoSize = true;
            this.labelReceiver.Font = new System.Drawing.Font("맑은 고딕", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelReceiver.Location = new System.Drawing.Point(52, 412);
            this.labelReceiver.Name = "labelReceiver";
            this.labelReceiver.Size = new System.Drawing.Size(133, 50);
            this.labelReceiver.TabIndex = 1;
            this.labelReceiver.Text = "수신자";
            // 
            // labelReceiverName
            // 
            this.labelReceiverName.AutoSize = true;
            this.labelReceiverName.Font = new System.Drawing.Font("맑은 고딕", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelReceiverName.ForeColor = System.Drawing.Color.OrangeRed;
            this.labelReceiverName.Location = new System.Drawing.Point(50, 462);
            this.labelReceiverName.Name = "labelReceiverName";
            this.labelReceiverName.Size = new System.Drawing.Size(316, 65);
            this.labelReceiverName.TabIndex = 2;
            this.labelReceiverName.Text = "영흥발전본부";
            // 
            // labelWorkType
            // 
            this.labelWorkType.AutoSize = true;
            this.labelWorkType.Font = new System.Drawing.Font("맑은 고딕", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelWorkType.ForeColor = System.Drawing.Color.OrangeRed;
            this.labelWorkType.Location = new System.Drawing.Point(102, 326);
            this.labelWorkType.Name = "labelWorkType";
            this.labelWorkType.Size = new System.Drawing.Size(170, 50);
            this.labelWorkType.TabIndex = 0;
            this.labelWorkType.Text = "문자발송";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1429, 656);
            this.Controls.Add(this.labelWorkType);
            this.Controls.Add(this.labelReceiverName);
            this.Controls.Add(this.labelReceiver);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.pictureBoxWorkType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWorkType)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.PictureBox pictureBoxWorkType;
        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.Label labelReceiver;
        private System.Windows.Forms.Label labelReceiverName;
        private System.Windows.Forms.Label labelWorkType;
    }
}

