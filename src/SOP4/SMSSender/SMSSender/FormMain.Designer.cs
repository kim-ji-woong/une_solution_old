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
            this.btnMsgSend = new System.Windows.Forms.Button();
            this.btnMsgClear = new System.Windows.Forms.Button();
            this.btnClearReciver = new System.Windows.Forms.Button();
            this.btnAddReciver = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxSender
            // 
            this.textBoxSender.Location = new System.Drawing.Point(84, 15);
            this.textBoxSender.Name = "textBoxSender";
            this.textBoxSender.Size = new System.Drawing.Size(176, 21);
            this.textBoxSender.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "발신자";
            // 
            // textboxReciver
            // 
            this.textboxReciver.Location = new System.Drawing.Point(84, 46);
            this.textboxReciver.Name = "textboxReciver";
            this.textboxReciver.Size = new System.Drawing.Size(176, 21);
            this.textboxReciver.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "수신자";
            // 
            // textboxContent
            // 
            this.textboxContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxContent.Location = new System.Drawing.Point(84, 102);
            this.textboxContent.Multiline = true;
            this.textboxContent.Name = "textboxContent";
            this.textboxContent.Size = new System.Drawing.Size(235, 138);
            this.textboxContent.TabIndex = 2;
            this.textboxContent.TextChanged += new System.EventHandler(this.textboxContent_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "내용";
            // 
            // lableLength
            // 
            this.lableLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lableLength.Location = new System.Drawing.Point(175, 243);
            this.lableLength.Name = "lableLength";
            this.lableLength.Size = new System.Drawing.Size(144, 18);
            this.lableLength.TabIndex = 14;
            this.lableLength.Text = "0/80 바이트";
            this.lableLength.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnMsgSend
            // 
            this.btnMsgSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMsgSend.Location = new System.Drawing.Point(242, 266);
            this.btnMsgSend.Name = "btnMsgSend";
            this.btnMsgSend.Size = new System.Drawing.Size(77, 23);
            this.btnMsgSend.TabIndex = 3;
            this.btnMsgSend.Text = "전송하기";
            this.btnMsgSend.UseVisualStyleBackColor = true;
            this.btnMsgSend.Click += new System.EventHandler(this.btnMsgSend_Click);
            // 
            // btnMsgClear
            // 
            this.btnMsgClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnMsgClear.Location = new System.Drawing.Point(84, 266);
            this.btnMsgClear.Name = "btnMsgClear";
            this.btnMsgClear.Size = new System.Drawing.Size(102, 23);
            this.btnMsgClear.TabIndex = 16;
            this.btnMsgClear.Text = "내용 지우기";
            this.btnMsgClear.UseVisualStyleBackColor = true;
            this.btnMsgClear.Click += new System.EventHandler(this.btnMsgClear_Click);
            // 
            // btnClearReciver
            // 
            this.btnClearReciver.Location = new System.Drawing.Point(84, 73);
            this.btnClearReciver.Name = "btnClearReciver";
            this.btnClearReciver.Size = new System.Drawing.Size(102, 23);
            this.btnClearReciver.TabIndex = 17;
            this.btnClearReciver.Text = "수신자 지우기";
            this.btnClearReciver.UseVisualStyleBackColor = true;
            this.btnClearReciver.Click += new System.EventHandler(this.btnClearReciver_Click);
            // 
            // btnAddReciver
            // 
            this.btnAddReciver.Location = new System.Drawing.Point(192, 73);
            this.btnAddReciver.Name = "btnAddReciver";
            this.btnAddReciver.Size = new System.Drawing.Size(87, 23);
            this.btnAddReciver.TabIndex = 18;
            this.btnAddReciver.Text = "수신자추가";
            this.btnAddReciver.UseVisualStyleBackColor = true;
            this.btnAddReciver.Click += new System.EventHandler(this.btnAddReciver_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(363, 311);
            this.Controls.Add(this.btnAddReciver);
            this.Controls.Add(this.btnClearReciver);
            this.Controls.Add(this.btnMsgClear);
            this.Controls.Add(this.btnMsgSend);
            this.Controls.Add(this.lableLength);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textboxContent);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textboxReciver);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxSender);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(521, 420);
            this.MinimumSize = new System.Drawing.Size(320, 350);
            this.Name = "FormMain";
            this.Text = "문자전송";
            this.Load += new System.EventHandler(this.FormMain_Load);
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
        private System.Windows.Forms.Button btnMsgSend;
        private System.Windows.Forms.Button btnMsgClear;
        private System.Windows.Forms.Button btnClearReciver;
        private System.Windows.Forms.Button btnAddReciver;
    }
}

