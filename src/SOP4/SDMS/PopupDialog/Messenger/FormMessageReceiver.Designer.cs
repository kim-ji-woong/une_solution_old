namespace SDMS.PopupDialog
{
    partial class FormMessageReceiver
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMessageReceiver));
            this.labelSender = new System.Windows.Forms.Label();
            this.labelReceiveTime = new System.Windows.Forms.Label();
            this.rtbBody = new System.Windows.Forms.RichTextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.gridUnread = new System.Windows.Forms.DataGridView();
            this.labelUnread = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.labelNoMessage = new System.Windows.Forms.Label();
            this.btnShowSendingForm = new System.Windows.Forms.Button();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSender = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDelete = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridUnread)).BeginInit();
            this.SuspendLayout();
            // 
            // labelSender
            // 
            this.labelSender.AutoSize = true;
            this.labelSender.Location = new System.Drawing.Point(12, 9);
            this.labelSender.Name = "labelSender";
            this.labelSender.Size = new System.Drawing.Size(65, 12);
            this.labelSender.TabIndex = 0;
            this.labelSender.Text = "작성자    : ";
            // 
            // labelReceiveTime
            // 
            this.labelReceiveTime.AutoSize = true;
            this.labelReceiveTime.Location = new System.Drawing.Point(12, 27);
            this.labelReceiveTime.Name = "labelReceiveTime";
            this.labelReceiveTime.Size = new System.Drawing.Size(65, 12);
            this.labelReceiveTime.TabIndex = 0;
            this.labelReceiveTime.Text = "작성시간 : ";
            // 
            // rtbBody
            // 
            this.rtbBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbBody.BackColor = System.Drawing.Color.White;
            this.rtbBody.Location = new System.Drawing.Point(12, 67);
            this.rtbBody.Name = "rtbBody";
            this.rtbBody.ReadOnly = true;
            this.rtbBody.Size = new System.Drawing.Size(492, 232);
            this.rtbBody.TabIndex = 1;
            this.rtbBody.Text = "";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(456, 479);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(48, 23);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "확인";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // gridUnread
            // 
            this.gridUnread.AllowUserToAddRows = false;
            this.gridUnread.AllowUserToDeleteRows = false;
            this.gridUnread.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridUnread.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridUnread.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colMessage,
            this.colTime,
            this.colSender,
            this.colDelete});
            this.gridUnread.Location = new System.Drawing.Point(12, 335);
            this.gridUnread.MultiSelect = false;
            this.gridUnread.Name = "gridUnread";
            this.gridUnread.ReadOnly = true;
            this.gridUnread.RowHeadersVisible = false;
            this.gridUnread.RowTemplate.Height = 23;
            this.gridUnread.Size = new System.Drawing.Size(492, 138);
            this.gridUnread.TabIndex = 11;
            this.gridUnread.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridUnread_CellContentClick);
            this.gridUnread.SelectionChanged += new System.EventHandler(this.gridUnread_SelectionChanged);
            // 
            // labelUnread
            // 
            this.labelUnread.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelUnread.AutoSize = true;
            this.labelUnread.Location = new System.Drawing.Point(12, 315);
            this.labelUnread.Name = "labelUnread";
            this.labelUnread.Size = new System.Drawing.Size(69, 12);
            this.labelUnread.TabIndex = 12;
            this.labelUnread.Text = "메시지 목록";
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Location = new System.Drawing.Point(12, 45);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(37, 12);
            this.labelTitle.TabIndex = 13;
            this.labelTitle.Text = "제목 :";
            // 
            // textBoxTitle
            // 
            this.textBoxTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTitle.BackColor = System.Drawing.Color.White;
            this.textBoxTitle.Location = new System.Drawing.Point(52, 42);
            this.textBoxTitle.Name = "textBoxTitle";
            this.textBoxTitle.ReadOnly = true;
            this.textBoxTitle.Size = new System.Drawing.Size(451, 21);
            this.textBoxTitle.TabIndex = 14;
            // 
            // labelNoMessage
            // 
            this.labelNoMessage.AutoSize = true;
            this.labelNoMessage.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelNoMessage.Location = new System.Drawing.Point(131, 9);
            this.labelNoMessage.Name = "labelNoMessage";
            this.labelNoMessage.Size = new System.Drawing.Size(240, 25);
            this.labelNoMessage.TabIndex = 15;
            this.labelNoMessage.Text = "수신된 메시지가 없습니다.";
            this.labelNoMessage.Visible = false;
            // 
            // btnShowSendingForm
            // 
            this.btnShowSendingForm.Location = new System.Drawing.Point(423, 4);
            this.btnShowSendingForm.Name = "btnShowSendingForm";
            this.btnShowSendingForm.Size = new System.Drawing.Size(80, 23);
            this.btnShowSendingForm.TabIndex = 16;
            this.btnShowSendingForm.Text = "메시지 작성";
            this.btnShowSendingForm.UseVisualStyleBackColor = true;
            this.btnShowSendingForm.Click += new System.EventHandler(this.btnShowSendingForm_Click);
            // 
            // colNo
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle1;
            this.colNo.HeaderText = " 번호";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.Width = 60;
            // 
            // colMessage
            // 
            this.colMessage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colMessage.DefaultCellStyle = dataGridViewCellStyle2;
            this.colMessage.HeaderText = "  메시지";
            this.colMessage.Name = "colMessage";
            this.colMessage.ReadOnly = true;
            // 
            // colTime
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colTime.DefaultCellStyle = dataGridViewCellStyle3;
            this.colTime.HeaderText = "  작성시간";
            this.colTime.Name = "colTime";
            this.colTime.ReadOnly = true;
            this.colTime.Width = 120;
            // 
            // colSender
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colSender.DefaultCellStyle = dataGridViewCellStyle4;
            this.colSender.HeaderText = "  작성자";
            this.colSender.Name = "colSender";
            this.colSender.ReadOnly = true;
            this.colSender.Width = 80;
            // 
            // colDelete
            // 
            this.colDelete.HeaderText = "  삭제";
            this.colDelete.Name = "colDelete";
            this.colDelete.ReadOnly = true;
            this.colDelete.Width = 60;
            // 
            // FormMessageReceiver
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 509);
            this.Controls.Add(this.btnShowSendingForm);
            this.Controls.Add(this.labelNoMessage);
            this.Controls.Add(this.textBoxTitle);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.labelUnread);
            this.Controls.Add(this.gridUnread);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.rtbBody);
            this.Controls.Add(this.labelReceiveTime);
            this.Controls.Add(this.labelSender);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMessageReceiver";
            this.Text = "알림 메시지";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMessageReceiver_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.gridUnread)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelSender;
        private System.Windows.Forms.Label labelReceiveTime;
        private System.Windows.Forms.RichTextBox rtbBody;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.DataGridView gridUnread;
        private System.Windows.Forms.Label labelUnread;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.TextBox textBoxTitle;
        private System.Windows.Forms.Label labelNoMessage;
        private System.Windows.Forms.Button btnShowSendingForm;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMessage;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSender;
        private System.Windows.Forms.DataGridViewButtonColumn colDelete;
    }
}