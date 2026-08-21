
namespace SVMSServer
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.textBoxStatus = new System.Windows.Forms.TextBox();
            this.labelSVMSStatus = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnUpdateCCTV = new System.Windows.Forms.Button();
            this.textBoxCCTVID = new System.Windows.Forms.TextBox();
            this.btnSendSVMSEvent = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxStatus
            // 
            this.textBoxStatus.Location = new System.Drawing.Point(12, 12);
            this.textBoxStatus.Multiline = true;
            this.textBoxStatus.Name = "textBoxStatus";
            this.textBoxStatus.Size = new System.Drawing.Size(430, 295);
            this.textBoxStatus.TabIndex = 0;
            // 
            // labelSVMSStatus
            // 
            this.labelSVMSStatus.AutoSize = true;
            this.labelSVMSStatus.Location = new System.Drawing.Point(12, 335);
            this.labelSVMSStatus.Name = "labelSVMSStatus";
            this.labelSVMSStatus.Size = new System.Drawing.Size(40, 12);
            this.labelSVMSStatus.TabIndex = 1;
            this.labelSVMSStatus.Text = "SVMS";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnUpdateCCTV
            // 
            this.btnUpdateCCTV.Location = new System.Drawing.Point(337, 330);
            this.btnUpdateCCTV.Name = "btnUpdateCCTV";
            this.btnUpdateCCTV.Size = new System.Drawing.Size(103, 23);
            this.btnUpdateCCTV.TabIndex = 2;
            this.btnUpdateCCTV.Text = "CCTV 업데이트";
            this.btnUpdateCCTV.UseVisualStyleBackColor = true;
            this.btnUpdateCCTV.Click += new System.EventHandler(this.btnUpdateCCTV_Click);
            // 
            // textBoxCCTVID
            // 
            this.textBoxCCTVID.Location = new System.Drawing.Point(94, 326);
            this.textBoxCCTVID.Name = "textBoxCCTVID";
            this.textBoxCCTVID.Size = new System.Drawing.Size(72, 21);
            this.textBoxCCTVID.TabIndex = 3;
            // 
            // btnSendSVMSEvent
            // 
            this.btnSendSVMSEvent.Location = new System.Drawing.Point(172, 326);
            this.btnSendSVMSEvent.Name = "btnSendSVMSEvent";
            this.btnSendSVMSEvent.Size = new System.Drawing.Size(86, 23);
            this.btnSendSVMSEvent.TabIndex = 4;
            this.btnSendSVMSEvent.Text = "SVMS 신호";
            this.btnSendSVMSEvent.UseVisualStyleBackColor = true;
            this.btnSendSVMSEvent.Click += new System.EventHandler(this.btnSendSVMSEvent_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 371);
            this.Controls.Add(this.btnSendSVMSEvent);
            this.Controls.Add(this.textBoxCCTVID);
            this.Controls.Add(this.btnUpdateCCTV);
            this.Controls.Add(this.labelSVMSStatus);
            this.Controls.Add(this.textBoxStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormMain";
            this.Text = "SVMS Server";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxStatus;
        private System.Windows.Forms.Label labelSVMSStatus;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnUpdateCCTV;
        private System.Windows.Forms.TextBox textBoxCCTVID;
        private System.Windows.Forms.Button btnSendSVMSEvent;
    }
}

