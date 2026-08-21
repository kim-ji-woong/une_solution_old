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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMessageReceiver));
            this.labelSender = new System.Windows.Forms.Label();
            this.labelReceiveTime = new System.Windows.Forms.Label();
            this.rtbBody = new System.Windows.Forms.RichTextBox();
            this.gridUnread = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSender = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDelete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.labelUnread = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelNoMessage = new System.Windows.Forms.Label();
            this.btnOK = new UnE.GUI.ImageButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridUnread)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOK)).BeginInit();
            this.SuspendLayout();
            // 
            // labelSender
            // 
            this.labelSender.AutoSize = true;
            this.labelSender.BackColor = System.Drawing.Color.Transparent;
            this.labelSender.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelSender.ForeColor = System.Drawing.Color.White;
            this.labelSender.Location = new System.Drawing.Point(2, 31);
            this.labelSender.Name = "labelSender";
            this.labelSender.Size = new System.Drawing.Size(98, 18);
            this.labelSender.TabIndex = 0;
            this.labelSender.Text = "작성자    : ";
            // 
            // labelReceiveTime
            // 
            this.labelReceiveTime.AutoSize = true;
            this.labelReceiveTime.BackColor = System.Drawing.Color.Transparent;
            this.labelReceiveTime.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelReceiveTime.ForeColor = System.Drawing.Color.White;
            this.labelReceiveTime.Location = new System.Drawing.Point(2, 56);
            this.labelReceiveTime.Name = "labelReceiveTime";
            this.labelReceiveTime.Size = new System.Drawing.Size(98, 18);
            this.labelReceiveTime.TabIndex = 0;
            this.labelReceiveTime.Text = "작성시간 : ";
            // 
            // rtbBody
            // 
            this.rtbBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbBody.BackColor = System.Drawing.Color.White;
            this.rtbBody.Font = new System.Drawing.Font("굴림", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rtbBody.Location = new System.Drawing.Point(6, 105);
            this.rtbBody.Name = "rtbBody";
            this.rtbBody.ReadOnly = true;
            this.rtbBody.Size = new System.Drawing.Size(396, 89);
            this.rtbBody.TabIndex = 1;
            this.rtbBody.Text = "";
            // 
            // gridUnread
            // 
            this.gridUnread.AllowUserToAddRows = false;
            this.gridUnread.AllowUserToDeleteRows = false;
            this.gridUnread.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridUnread.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridUnread.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridUnread.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colMessage,
            this.colTime,
            this.colSender,
            this.colDelete});
            this.gridUnread.Location = new System.Drawing.Point(6, 220);
            this.gridUnread.MultiSelect = false;
            this.gridUnread.Name = "gridUnread";
            this.gridUnread.ReadOnly = true;
            this.gridUnread.RowHeadersVisible = false;
            this.gridUnread.RowTemplate.Height = 23;
            this.gridUnread.Size = new System.Drawing.Size(396, 140);
            this.gridUnread.TabIndex = 11;
            this.gridUnread.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridUnread_CellContentClick);
            this.gridUnread.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.gridUnread_CellPainting);
            this.gridUnread.SelectionChanged += new System.EventHandler(this.gridUnread_SelectionChanged);
            // 
            // colNo
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle2;
            this.colNo.HeaderText = "번호";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.Width = 60;
            // 
            // colMessage
            // 
            this.colMessage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colMessage.DefaultCellStyle = dataGridViewCellStyle3;
            this.colMessage.HeaderText = "메시지";
            this.colMessage.Name = "colMessage";
            this.colMessage.ReadOnly = true;
            // 
            // colTime
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colTime.DefaultCellStyle = dataGridViewCellStyle4;
            this.colTime.HeaderText = "작성시간";
            this.colTime.Name = "colTime";
            this.colTime.ReadOnly = true;
            this.colTime.Width = 120;
            // 
            // colSender
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colSender.DefaultCellStyle = dataGridViewCellStyle5;
            this.colSender.HeaderText = "작성자";
            this.colSender.Name = "colSender";
            this.colSender.ReadOnly = true;
            this.colSender.Width = 80;
            // 
            // colDelete
            // 
            this.colDelete.HeaderText = "삭제";
            this.colDelete.Name = "colDelete";
            this.colDelete.ReadOnly = true;
            this.colDelete.Width = 50;
            // 
            // labelUnread
            // 
            this.labelUnread.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelUnread.AutoSize = true;
            this.labelUnread.BackColor = System.Drawing.Color.Transparent;
            this.labelUnread.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelUnread.ForeColor = System.Drawing.Color.White;
            this.labelUnread.Location = new System.Drawing.Point(2, 197);
            this.labelUnread.Name = "labelUnread";
            this.labelUnread.Size = new System.Drawing.Size(104, 18);
            this.labelUnread.TabIndex = 12;
            this.labelUnread.Text = "메시지 목록";
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.BackColor = System.Drawing.Color.Transparent;
            this.labelTitle.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(2, 81);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(56, 18);
            this.labelTitle.TabIndex = 13;
            this.labelTitle.Text = "제목 :";
            // 
            // labelNoMessage
            // 
            this.labelNoMessage.AutoSize = true;
            this.labelNoMessage.BackColor = System.Drawing.Color.Transparent;
            this.labelNoMessage.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelNoMessage.ForeColor = System.Drawing.Color.White;
            this.labelNoMessage.Location = new System.Drawing.Point(128, 42);
            this.labelNoMessage.Name = "labelNoMessage";
            this.labelNoMessage.Size = new System.Drawing.Size(264, 20);
            this.labelNoMessage.TabIndex = 15;
            this.labelNoMessage.Text = "수신된 메시지가 없습니다.";
            this.labelNoMessage.Visible = false;
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.Transparent;
            this.btnOK.ButtonText = "";
            this.btnOK.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.ImageClicked = global::SDMS.Properties.Resources.Ok_101_57_Click;
            this.btnOK.ImageDisabled = null;
            this.btnOK.ImageMouseOver = global::SDMS.Properties.Resources.Ok_101_57_Click;
            this.btnOK.ImageNormal = global::SDMS.Properties.Resources.Ok_101_57_Default;
            this.btnOK.Location = new System.Drawing.Point(350, 366);
            this.btnOK.Name = "btnOK";
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(52, 29);
            this.btnOK.TabIndex = 18;
            this.btnOK.TabStop = false;
            this.btnOK.TextColor = System.Drawing.Color.Black;
            this.btnOK.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.ToolTipText = "";
            this.btnOK.UseToolTip = false;
            this.btnOK.WindowRateWidth = 1F;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // FormMessageReceiver
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SDMS.Properties.Resources.MessageReceiver_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(408, 400);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.labelNoMessage);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.labelReceiveTime);
            this.Controls.Add(this.labelSender);
            this.Controls.Add(this.labelUnread);
            this.Controls.Add(this.gridUnread);
            this.Controls.Add(this.rtbBody);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMessageReceiver";
            this.Text = "알림 메시지";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMessageReceiver_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.gridUnread)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOK)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelSender;
        private System.Windows.Forms.Label labelReceiveTime;
        private System.Windows.Forms.RichTextBox rtbBody;
        private System.Windows.Forms.DataGridView gridUnread;
        private System.Windows.Forms.Label labelUnread;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelNoMessage;
        private UnE.GUI.ImageButton btnOK;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMessage;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSender;
        private System.Windows.Forms.DataGridViewButtonColumn colDelete;
    }
}