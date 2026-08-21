namespace TrainingMessage
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
            this.plTop = new System.Windows.Forms.Panel();
            this.btnClose = new UnE.GUI.ImageButton();
            this.label2 = new System.Windows.Forms.Label();
            this.lbCount = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtReceiver = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnSearch = new UnE.GUI.ImageButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtSender = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnSpread = new UnE.GUI.ImageButton();
            this.plTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearch)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSpread)).BeginInit();
            this.SuspendLayout();
            // 
            // plTop
            // 
            this.plTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(26)))), ((int)(((byte)(47)))));
            this.plTop.Controls.Add(this.btnClose);
            this.plTop.Controls.Add(this.label2);
            this.plTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.plTop.Location = new System.Drawing.Point(0, 0);
            this.plTop.Name = "plTop";
            this.plTop.Size = new System.Drawing.Size(400, 40);
            this.plTop.TabIndex = 5;
            this.plTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            this.plTop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form_MouseMove);
            this.plTop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form_MouseUp);
            // 
            // btnClose
            // 
            this.btnClose.ButtonText = "";
            this.btnClose.ImageClicked = global::TrainingMessage.Properties.Resources.btnClose_Selected;
            this.btnClose.ImageDisabled = null;
            this.btnClose.ImageMouseOver = global::TrainingMessage.Properties.Resources.btnClose_MouseOver;
            this.btnClose.ImageNormal = global::TrainingMessage.Properties.Resources.btnClose_Normal;
            this.btnClose.Location = new System.Drawing.Point(363, 11);
            this.btnClose.Name = "btnClose";
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(20, 20);
            this.btnClose.TabIndex = 56;
            this.btnClose.TabStop = false;
            this.btnClose.TextColor = System.Drawing.Color.Black;
            this.btnClose.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ToolTipText = "";
            this.btnClose.UseToolTip = false;
            this.btnClose.WindowRateWidth = 1F;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(173)))), ((int)(((byte)(254)))));
            this.label2.Location = new System.Drawing.Point(15, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(210, 19);
            this.label2.TabIndex = 5;
            this.label2.Text = "훈련 정보 연계 메시지";
            this.label2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            this.label2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form_MouseMove);
            this.label2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form_MouseUp);
            // 
            // lbCount
            // 
            this.lbCount.AutoSize = true;
            this.lbCount.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbCount.Location = new System.Drawing.Point(15, 70);
            this.lbCount.Name = "lbCount";
            this.lbCount.Size = new System.Drawing.Size(69, 19);
            this.lbCount.TabIndex = 6;
            this.lbCount.Text = "발신자";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(12, 125);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 19);
            this.label1.TabIndex = 7;
            this.label1.Text = "수신자";
            // 
            // txtReceiver
            // 
            this.txtReceiver.BackColor = System.Drawing.Color.White;
            this.txtReceiver.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtReceiver.Enabled = false;
            this.txtReceiver.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReceiver.Location = new System.Drawing.Point(10, 8);
            this.txtReceiver.Name = "txtReceiver";
            this.txtReceiver.Size = new System.Drawing.Size(234, 17);
            this.txtReceiver.TabIndex = 59;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.txtReceiver);
            this.panel3.Controls.Add(this.btnSearch);
            this.panel3.Location = new System.Drawing.Point(89, 118);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(294, 34);
            this.panel3.TabIndex = 69;
            // 
            // btnSearch
            // 
            this.btnSearch.ButtonText = "";
            this.btnSearch.ImageClicked = global::TrainingMessage.Properties.Resources.btnSearch_Click;
            this.btnSearch.ImageDisabled = null;
            this.btnSearch.ImageMouseOver = global::TrainingMessage.Properties.Resources.btnSearch_MouseOver;
            this.btnSearch.ImageNormal = global::TrainingMessage.Properties.Resources.btnSearch_Normal;
            this.btnSearch.Location = new System.Drawing.Point(250, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Owner = null;
            this.btnSearch.Size = new System.Drawing.Size(36, 25);
            this.btnSearch.TabIndex = 58;
            this.btnSearch.TabStop = false;
            this.btnSearch.TextColor = System.Drawing.Color.Black;
            this.btnSearch.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSearch.ToolTipText = "";
            this.btnSearch.UseToolTip = false;
            this.btnSearch.WindowRateWidth = 1F;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.txtSender);
            this.panel1.Location = new System.Drawing.Point(90, 62);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(293, 34);
            this.panel1.TabIndex = 70;
            // 
            // txtSender
            // 
            this.txtSender.BackColor = System.Drawing.Color.White;
            this.txtSender.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSender.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.txtSender.Enabled = false;
            this.txtSender.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSender.Location = new System.Drawing.Point(10, 8);
            this.txtSender.Name = "txtSender";
            this.txtSender.ReadOnly = true;
            this.txtSender.Size = new System.Drawing.Size(267, 17);
            this.txtSender.TabIndex = 59;
            this.txtSender.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(15, 174);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 19);
            this.label3.TabIndex = 71;
            this.label3.Text = "내용";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.txtMessage);
            this.panel4.Location = new System.Drawing.Point(89, 174);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(294, 180);
            this.panel4.TabIndex = 75;
            // 
            // txtMessage
            // 
            this.txtMessage.BackColor = System.Drawing.Color.White;
            this.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMessage.Location = new System.Drawing.Point(10, 8);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(268, 156);
            this.txtMessage.TabIndex = 0;
            // 
            // btnSpread
            // 
            this.btnSpread.ButtonText = "";
            this.btnSpread.ImageClicked = global::TrainingMessage.Properties.Resources.btnSend_Click;
            this.btnSpread.ImageDisabled = null;
            this.btnSpread.ImageMouseOver = global::TrainingMessage.Properties.Resources.btnSend_Hover;
            this.btnSpread.ImageNormal = global::TrainingMessage.Properties.Resources.btnSend_Normal;
            this.btnSpread.Location = new System.Drawing.Point(303, 360);
            this.btnSpread.Name = "btnSpread";
            this.btnSpread.Owner = null;
            this.btnSpread.Size = new System.Drawing.Size(80, 35);
            this.btnSpread.TabIndex = 76;
            this.btnSpread.TabStop = false;
            this.btnSpread.TextColor = System.Drawing.Color.Black;
            this.btnSpread.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSpread.ToolTipText = "";
            this.btnSpread.UseToolTip = false;
            this.btnSpread.WindowRateWidth = 1F;
            this.btnSpread.Click += new System.EventHandler(this.btnSpread_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 406);
            this.Controls.Add(this.btnSpread);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbCount);
            this.Controls.Add(this.plTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.plTop.ResumeLayout(false);
            this.plTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearch)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSpread)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel plTop;
        private UnE.GUI.ImageButton btnClose;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbCount;
        private System.Windows.Forms.Label label1;
        private UnE.GUI.ImageButton btnSearch;
        private System.Windows.Forms.TextBox txtReceiver;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtSender;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox txtMessage;
        private UnE.GUI.ImageButton btnSpread;
    }
}

