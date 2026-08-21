namespace SOPMonitoringSystem
{
    partial class ComponentContents
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel = new System.Windows.Forms.Panel();
            this.cboDecisions = new SOPMonitoringSystem.DisabledComboBox();
            this.panelTitle = new System.Windows.Forms.Panel();
            this.labelSender = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            this.btnSMS = new System.Windows.Forms.Button();
            this.labelSMS = new System.Windows.Forms.Label();
            this.btnBroadcast = new System.Windows.Forms.Button();
            this.labelBroadcast = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.colPerformer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new SOPMonitoringSystem.DataGridViewDisableButtonColumn();
            this.colCheckComplete = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewDisableButtonColumn1 = new SOPMonitoringSystem.DataGridViewDisableButtonColumn();
            this.panelExternal = new System.Windows.Forms.Panel();
            this.panel.SuspendLayout();
            this.panelTitle.SuspendLayout();
            this.btnSMS.SuspendLayout();
            this.btnBroadcast.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.BackColor = System.Drawing.SystemColors.Control;
            this.panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel.Controls.Add(this.cboDecisions);
            this.panel.Controls.Add(this.panelTitle);
            this.panel.Controls.Add(this.btnSMS);
            this.panel.Controls.Add(this.btnBroadcast);
            this.panel.Controls.Add(this.pictureBox1);
            this.panel.Controls.Add(this.btnExecute);
            this.panel.Controls.Add(this.btnNext);
            this.panel.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel.Location = new System.Drawing.Point(1, 2);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(639, 35);
            this.panel.TabIndex = 0;
            this.panel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnDoubleClick);
            // 
            // cboDecisions
            // 
            this.cboDecisions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboDecisions.CanVisible = true;
            this.cboDecisions.Disabled = false;
            this.cboDecisions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDecisions.FormattingEnabled = true;
            this.cboDecisions.Location = new System.Drawing.Point(317, 6);
            this.cboDecisions.Name = "cboDecisions";
            this.cboDecisions.Size = new System.Drawing.Size(162, 20);
            this.cboDecisions.TabIndex = 10;
            this.cboDecisions.Visible = false;
            this.cboDecisions.SelectedIndexChanged += new System.EventHandler(this.cboDecisions_SelectedIndexChanged);
            // 
            // panelTitle
            // 
            this.panelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelTitle.Controls.Add(this.labelSender);
            this.panelTitle.Controls.Add(this.labelTitle);
            this.panelTitle.Location = new System.Drawing.Point(3, 3);
            this.panelTitle.Name = "panelTitle";
            this.panelTitle.Size = new System.Drawing.Size(474, 26);
            this.panelTitle.TabIndex = 11;
            // 
            // labelSender
            // 
            this.labelSender.AutoSize = true;
            this.labelSender.BackColor = System.Drawing.Color.Transparent;
            this.labelSender.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelSender.Location = new System.Drawing.Point(174, 6);
            this.labelSender.Name = "labelSender";
            this.labelSender.Size = new System.Drawing.Size(54, 20);
            this.labelSender.TabIndex = 5;
            this.labelSender.Text = "발신자";
            this.labelSender.TextChanged += new System.EventHandler(this.labelSender_TextChanged);
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.BackColor = System.Drawing.Color.Transparent;
            this.labelTitle.Font = new System.Drawing.Font("맑은 고딕", 13.25F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(0, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(126, 25);
            this.labelTitle.TabIndex = 4;
            this.labelTitle.Text = "화재발생 접수";
            this.labelTitle.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnDoubleClick);
            // 
            // btnSMS
            // 
            this.btnSMS.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(72)))), ((int)(((byte)(0)))));
            this.btnSMS.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSMS.Controls.Add(this.labelSMS);
            this.btnSMS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSMS.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSMS.Location = new System.Drawing.Point(3, 0);
            this.btnSMS.Margin = new System.Windows.Forms.Padding(0);
            this.btnSMS.Name = "btnSMS";
            this.btnSMS.Size = new System.Drawing.Size(31, 31);
            this.btnSMS.TabIndex = 9;
            this.btnSMS.TabStop = false;
            this.btnSMS.UseVisualStyleBackColor = false;
            this.btnSMS.Visible = false;
            this.btnSMS.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnDoubleClick);
            this.btnSMS.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxSMS_MouseClick);
            // 
            // labelSMS
            // 
            this.labelSMS.AutoSize = true;
            this.labelSMS.BackColor = System.Drawing.Color.Transparent;
            this.labelSMS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelSMS.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelSMS.ForeColor = System.Drawing.Color.White;
            this.labelSMS.Location = new System.Drawing.Point(-1, 6);
            this.labelSMS.Name = "labelSMS";
            this.labelSMS.Size = new System.Drawing.Size(34, 17);
            this.labelSMS.TabIndex = 10;
            this.labelSMS.Text = "문자";
            // 
            // btnBroadcast
            // 
            this.btnBroadcast.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(180)))), ((int)(((byte)(77)))));
            this.btnBroadcast.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnBroadcast.Controls.Add(this.labelBroadcast);
            this.btnBroadcast.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBroadcast.Location = new System.Drawing.Point(34, 0);
            this.btnBroadcast.Name = "btnBroadcast";
            this.btnBroadcast.Size = new System.Drawing.Size(35, 31);
            this.btnBroadcast.TabIndex = 8;
            this.btnBroadcast.TabStop = false;
            this.btnBroadcast.UseVisualStyleBackColor = false;
            this.btnBroadcast.Visible = false;
            this.btnBroadcast.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnDoubleClick);
            this.btnBroadcast.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxBroadcast_MouseClick);
            // 
            // labelBroadcast
            // 
            this.labelBroadcast.AutoSize = true;
            this.labelBroadcast.BackColor = System.Drawing.Color.Transparent;
            this.labelBroadcast.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelBroadcast.ForeColor = System.Drawing.Color.White;
            this.labelBroadcast.Location = new System.Drawing.Point(2, 7);
            this.labelBroadcast.Name = "labelBroadcast";
            this.labelBroadcast.Size = new System.Drawing.Size(34, 17);
            this.labelBroadcast.TabIndex = 10;
            this.labelBroadcast.Text = "방송";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = global::SOPMonitoringSystem.Properties.Resources.btn_arrow;
            this.pictureBox1.Location = new System.Drawing.Point(605, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(24, 24);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnDoubleClick);
            // 
            // btnExecute
            // 
            this.btnExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecute.Location = new System.Drawing.Point(483, 5);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(68, 23);
            this.btnExecute.TabIndex = 6;
            this.btnExecute.Text = "모두 실행";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Visible = false;
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Location = new System.Drawing.Point(557, 5);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(45, 23);
            this.btnNext.TabIndex = 6;
            this.btnNext.Text = "다음";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView.ColumnHeadersHeight = 30;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPerformer,
            this.Column4,
            this.Column3,
            this.Column1,
            this.colCheckComplete,
            this.colTime,
            this.colActor});
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridView.Location = new System.Drawing.Point(1, 36);
            this.dataGridView.Name = "dataGridView";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dataGridView.RowsDefaultCellStyle = dataGridViewCellStyle9;
            this.dataGridView.RowTemplate.Height = 23;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(639, 144);
            this.dataGridView.TabIndex = 1;
            this.dataGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView_CellBeginEdit);
            this.dataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellClick);
            this.dataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellContentClick);
            this.dataGridView.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_CellMouseDown);
            this.dataGridView.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_CellMouseUp);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            this.dataGridView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnDoubleClick);
            this.dataGridView.MouseCaptureChanged += new System.EventHandler(this.dataGridView_MouseCaptureChanged);
            // 
            // colPerformer
            // 
            this.colPerformer.FillWeight = 150F;
            this.colPerformer.HeaderText = "실행자";
            this.colPerformer.Name = "colPerformer";
            this.colPerformer.ReadOnly = true;
            this.colPerformer.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colPerformer.Width = 150;
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.Column4.DefaultCellStyle = dataGridViewCellStyle3;
            this.Column4.HeaderText = "임무내용";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column4.ToolTipText = "임무내용";
            // 
            // Column3
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.Column3.DefaultCellStyle = dataGridViewCellStyle4;
            this.Column3.FillWeight = 250F;
            this.Column3.HeaderText = "수신자";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column3.ToolTipText = "임무대상";
            this.Column3.Width = 150;
            // 
            // Column1
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.Column1.DefaultCellStyle = dataGridViewCellStyle5;
            this.Column1.FillWeight = 55F;
            this.Column1.HeaderText = "문자";
            this.Column1.Name = "Column1";
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column1.ToolTipText = "문자";
            this.Column1.Width = 55;
            // 
            // colCheckComplete
            // 
            this.colCheckComplete.FillWeight = 55F;
            this.colCheckComplete.HeaderText = "완료";
            this.colCheckComplete.Name = "colCheckComplete";
            this.colCheckComplete.Width = 55;
            // 
            // colTime
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colTime.DefaultCellStyle = dataGridViewCellStyle6;
            this.colTime.FillWeight = 55F;
            this.colTime.HeaderText = "시간";
            this.colTime.Name = "colTime";
            this.colTime.ReadOnly = true;
            this.colTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colTime.Width = 55;
            // 
            // colActor
            // 
            this.colActor.FillWeight = 150F;
            this.colActor.HeaderText = "발신자";
            this.colActor.Name = "colActor";
            this.colActor.ReadOnly = true;
            this.colActor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colActor.Visible = false;
            this.colActor.Width = 150;
            // 
            // dataGridViewDisableButtonColumn1
            // 
            this.dataGridViewDisableButtonColumn1.HeaderText = "실행";
            this.dataGridViewDisableButtonColumn1.Name = "dataGridViewDisableButtonColumn1";
            this.dataGridViewDisableButtonColumn1.Width = 60;
            // 
            // panelExternal
            // 
            this.panelExternal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelExternal.Location = new System.Drawing.Point(1, 36);
            this.panelExternal.Name = "panelExternal";
            this.panelExternal.Size = new System.Drawing.Size(639, 144);
            this.panelExternal.TabIndex = 2;
            this.panelExternal.Visible = false;
            // 
            // ComponentContents
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(641, 37);
            this.Controls.Add(this.panelExternal);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.panel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ComponentContents";
            this.Padding = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.Text = "ComponentContents";
            this.panel.ResumeLayout(false);
            this.panelTitle.ResumeLayout(false);
            this.panelTitle.PerformLayout();
            this.btnSMS.ResumeLayout(false);
            this.btnSMS.PerformLayout();
            this.btnBroadcast.ResumeLayout(false);
            this.btnBroadcast.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Button btnBroadcast;
        private System.Windows.Forms.Button btnSMS;
        private System.Windows.Forms.Label labelSMS;
        private System.Windows.Forms.Label labelBroadcast;
        private System.Windows.Forms.Button btnExecute;
        private DataGridViewDisableButtonColumn dataGridViewDisableButtonColumn1;
        private System.Windows.Forms.Panel panelExternal;
        private DisabledComboBox cboDecisions;
        private System.Windows.Forms.Panel panelTitle;
        private System.Windows.Forms.Label labelSender;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPerformer;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private DataGridViewDisableButtonColumn Column1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colCheckComplete;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colActor;

    }
}