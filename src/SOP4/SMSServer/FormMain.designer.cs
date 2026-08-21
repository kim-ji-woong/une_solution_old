namespace MessageServer
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
            m_finishProcess = true;

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
			this.btnInitMsgQueue = new System.Windows.Forms.Button();
			this.editQueuePath = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// btnInitMsgQueue
			// 
			this.btnInitMsgQueue.Location = new System.Drawing.Point(231, 59);
			this.btnInitMsgQueue.Name = "btnInitMsgQueue";
			this.btnInitMsgQueue.Size = new System.Drawing.Size(135, 23);
			this.btnInitMsgQueue.TabIndex = 0;
			this.btnInitMsgQueue.Text = "메시지큐 초기화";
			this.btnInitMsgQueue.UseVisualStyleBackColor = true;
			this.btnInitMsgQueue.Click += new System.EventHandler(this.btnInitMsgQueue_Click);
			// 
			// editQueuePath
			// 
			this.editQueuePath.Location = new System.Drawing.Point(30, 22);
			this.editQueuePath.Name = "editQueuePath";
			this.editQueuePath.Size = new System.Drawing.Size(336, 21);
			this.editQueuePath.TabIndex = 1;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(381, 120);
			this.Controls.Add(this.editQueuePath);
			this.Controls.Add(this.btnInitMsgQueue);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMain";
			this.Text = "스마트 재난관리 SMS 서버";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
			this.Load += new System.EventHandler(this.FormMain_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Button btnInitMsgQueue;
		private System.Windows.Forms.TextBox editQueuePath;

	}
}

