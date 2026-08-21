namespace MessageSend
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.ReceiveGridView = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Disa = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Act = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Dele = new System.Windows.Forms.DataGridViewImageColumn();
            this.SendGridView = new System.Windows.Forms.DataGridView();
            this.Send_Disa = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Send_Act = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.lbMsg = new System.Windows.Forms.Label();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnGetSop = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ReceiveGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SendGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // ReceiveGridView
            // 
            this.ReceiveGridView.AllowUserToAddRows = false;
            this.ReceiveGridView.AllowUserToResizeColumns = false;
            this.ReceiveGridView.AllowUserToResizeRows = false;
            this.ReceiveGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ReceiveGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.ReceiveGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ReceiveGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.Time,
            this.Disa,
            this.Act,
            this.Dele});
            this.ReceiveGridView.Location = new System.Drawing.Point(11, 54);
            this.ReceiveGridView.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ReceiveGridView.Name = "ReceiveGridView";
            this.ReceiveGridView.ReadOnly = true;
            this.ReceiveGridView.RowHeadersVisible = false;
            this.ReceiveGridView.RowTemplate.Height = 23;
            this.ReceiveGridView.Size = new System.Drawing.Size(750, 150);
            this.ReceiveGridView.TabIndex = 0;
            this.ReceiveGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ReceiveGridView_CellContentClick);
            // 
            // ID
            // 
            this.ID.FillWeight = 47.1231F;
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            this.ID.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // Time
            // 
            this.Time.FillWeight = 98.32615F;
            this.Time.HeaderText = "시간";
            this.Time.Name = "Time";
            this.Time.ReadOnly = true;
            this.Time.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Time.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Disa
            // 
            this.Disa.FillWeight = 120.8132F;
            this.Disa.HeaderText = "SOP 재난명";
            this.Disa.Name = "Disa";
            this.Disa.ReadOnly = true;
            this.Disa.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Disa.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Act
            // 
            this.Act.FillWeight = 151.2723F;
            this.Act.HeaderText = "조치내용";
            this.Act.Name = "Act";
            this.Act.ReadOnly = true;
            this.Act.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Dele
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.NullValue = null;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.Dele.DefaultCellStyle = dataGridViewCellStyle1;
            this.Dele.FillWeight = 46.6277F;
            this.Dele.HeaderText = "삭제";
            this.Dele.Name = "Dele";
            this.Dele.ReadOnly = true;
            this.Dele.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // SendGridView
            // 
            this.SendGridView.AllowUserToAddRows = false;
            this.SendGridView.AllowUserToResizeColumns = false;
            this.SendGridView.AllowUserToResizeRows = false;
            this.SendGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SendGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.SendGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SendGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Send_Disa,
            this.Send_Act});
            this.SendGridView.Location = new System.Drawing.Point(11, 247);
            this.SendGridView.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.SendGridView.Name = "SendGridView";
            this.SendGridView.RowHeadersVisible = false;
            this.SendGridView.RowTemplate.Height = 23;
            this.SendGridView.Size = new System.Drawing.Size(750, 47);
            this.SendGridView.TabIndex = 1;
            this.SendGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SendGridView_KeyDown);
            // 
            // Send_Disa
            // 
            this.Send_Disa.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.Send_Disa.HeaderText = "SOP 재난명";
            this.Send_Disa.Name = "Send_Disa";
            this.Send_Disa.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Send_Disa.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Send_Act
            // 
            this.Send_Act.FillWeight = 134.0102F;
            this.Send_Act.HeaderText = "조치 내용";
            this.Send_Act.Name = "Send_Act";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.DarkGray;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 21);
            this.label1.TabIndex = 2;
            this.label1.Text = "메시지 전송 리스트";
            // 
            // lbMsg
            // 
            this.lbMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbMsg.AutoSize = true;
            this.lbMsg.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbMsg.ForeColor = System.Drawing.Color.DarkGray;
            this.lbMsg.Location = new System.Drawing.Point(12, 223);
            this.lbMsg.Name = "lbMsg";
            this.lbMsg.Size = new System.Drawing.Size(96, 21);
            this.lbMsg.TabIndex = 4;
            this.lbMsg.Text = "메시지 작성";
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.BackColor = System.Drawing.SystemColors.Control;
            this.btnSend.Location = new System.Drawing.Point(638, 312);
            this.btnSend.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(123, 31);
            this.btnSend.TabIndex = 5;
            this.btnSend.Text = "발    송";
            this.btnSend.UseVisualStyleBackColor = false;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(691, 15);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(70, 31);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "전체 삭제";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnGetSop
            // 
            this.btnGetSop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGetSop.Location = new System.Drawing.Point(12, 312);
            this.btnGetSop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnGetSop.Name = "btnGetSop";
            this.btnGetSop.Size = new System.Drawing.Size(112, 31);
            this.btnGetSop.TabIndex = 7;
            this.btnGetSop.Text = "새로고침";
            this.btnGetSop.UseVisualStyleBackColor = true;
            this.btnGetSop.Click += new System.EventHandler(this.btnGetSop_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(777, 361);
            this.Controls.Add(this.btnGetSop);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.lbMsg);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SendGridView);
            this.Controls.Add(this.ReceiveGridView);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "FormMain";
            this.Text = "SOP 메세지 전송";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.SizeChanged += new System.EventHandler(this.FormMain_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.ReceiveGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SendGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView ReceiveGridView;
        private System.Windows.Forms.DataGridView SendGridView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbMsg;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnGetSop;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Time;
        private System.Windows.Forms.DataGridViewTextBoxColumn Disa;
        private System.Windows.Forms.DataGridViewTextBoxColumn Act;
        private System.Windows.Forms.DataGridViewImageColumn Dele;
        private System.Windows.Forms.DataGridViewComboBoxColumn Send_Disa;
        private System.Windows.Forms.DataGridViewTextBoxColumn Send_Act;
    }
}

