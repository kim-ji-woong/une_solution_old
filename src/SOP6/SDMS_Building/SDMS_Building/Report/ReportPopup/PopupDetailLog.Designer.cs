namespace SDMS_Building.Report.ReportPopup
{
    partial class PopupDetailLog
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
            this.dgvAction = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colManager = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBuilding = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFloor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvSMS = new System.Windows.Forms.DataGridView();
            this.colNoSMS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDateSMS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLocationSMS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTransferPersonSMS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTransferContentSMS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnClose = new UnE.GUI.ImageButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAction)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSMS)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvAction
            // 
            this.dgvAction.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAction.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colDate,
            this.colManager,
            this.colType,
            this.colBuilding,
            this.colFloor});
            this.dgvAction.Location = new System.Drawing.Point(12, 96);
            this.dgvAction.Name = "dgvAction";
            this.dgvAction.RowHeadersVisible = false;
            this.dgvAction.RowTemplate.Height = 30;
            this.dgvAction.Size = new System.Drawing.Size(900, 252);
            this.dgvAction.TabIndex = 0;
            // 
            // colNo
            // 
            this.colNo.HeaderText = "No";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.Width = 80;
            // 
            // colDate
            // 
            this.colDate.HeaderText = "일시";
            this.colDate.Name = "colDate";
            this.colDate.ReadOnly = true;
            this.colDate.Width = 180;
            // 
            // colManager
            // 
            this.colManager.HeaderText = "담당자";
            this.colManager.Name = "colManager";
            this.colManager.ReadOnly = true;
            this.colManager.Width = 157;
            // 
            // colType
            // 
            this.colType.HeaderText = "분류";
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            this.colType.Width = 190;
            // 
            // colBuilding
            // 
            this.colBuilding.HeaderText = "건물";
            this.colBuilding.Name = "colBuilding";
            this.colBuilding.ReadOnly = true;
            this.colBuilding.Width = 190;
            // 
            // colFloor
            // 
            this.colFloor.HeaderText = "층";
            this.colFloor.Name = "colFloor";
            this.colFloor.ReadOnly = true;
            // 
            // dgvSMS
            // 
            this.dgvSMS.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSMS.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNoSMS,
            this.colDateSMS,
            this.colLocationSMS,
            this.colTransferPersonSMS,
            this.colTransferContentSMS});
            this.dgvSMS.Location = new System.Drawing.Point(12, 395);
            this.dgvSMS.Name = "dgvSMS";
            this.dgvSMS.RowHeadersVisible = false;
            this.dgvSMS.RowTemplate.Height = 30;
            this.dgvSMS.Size = new System.Drawing.Size(900, 139);
            this.dgvSMS.TabIndex = 1;
            // 
            // colNoSMS
            // 
            this.colNoSMS.HeaderText = "No";
            this.colNoSMS.Name = "colNoSMS";
            this.colNoSMS.ReadOnly = true;
            this.colNoSMS.Width = 80;
            // 
            // colDateSMS
            // 
            this.colDateSMS.HeaderText = "일시";
            this.colDateSMS.Name = "colDateSMS";
            this.colDateSMS.ReadOnly = true;
            this.colDateSMS.Width = 180;
            // 
            // colLocationSMS
            // 
            this.colLocationSMS.HeaderText = "발생 장소";
            this.colLocationSMS.Name = "colLocationSMS";
            this.colLocationSMS.ReadOnly = true;
            this.colLocationSMS.Width = 150;
            // 
            // colTransferPersonSMS
            // 
            this.colTransferPersonSMS.HeaderText = "전송 인원";
            this.colTransferPersonSMS.Name = "colTransferPersonSMS";
            this.colTransferPersonSMS.ReadOnly = true;
            this.colTransferPersonSMS.Width = 80;
            // 
            // colTransferContentSMS
            // 
            this.colTransferContentSMS.HeaderText = "전송 내용";
            this.colTransferContentSMS.Name = "colTransferContentSMS";
            this.colTransferContentSMS.ReadOnly = true;
            this.colTransferContentSMS.Width = 407;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.label1.Location = new System.Drawing.Point(13, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "대응 이력";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.label2.Location = new System.Drawing.Point(13, 366);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 19);
            this.label2.TabIndex = 3;
            this.label2.Text = "문자 이력";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(926, 51);
            this.panel1.TabIndex = 24;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("나눔바른고딕", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(51, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 22);
            this.label3.TabIndex = 17;
            this.label3.Text = "상세 정보";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.panel3.Location = new System.Drawing.Point(28, 21);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(7, 7);
            this.panel3.TabIndex = 16;
            // 
            // btnClose
            // 
            this.btnClose.ButtonText = "";
            this.btnClose.ImageClicked = global::SDMS_Building.Properties.Resources.close_Click;
            this.btnClose.ImageDisabled = null;
            this.btnClose.ImageMouseOver = global::SDMS_Building.Properties.Resources.close_Hover;
            this.btnClose.ImageNormal = global::SDMS_Building.Properties.Resources.close_Normal;
            this.btnClose.Location = new System.Drawing.Point(892, 15);
            this.btnClose.Name = "btnClose";
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(20, 20);
            this.btnClose.TabIndex = 15;
            this.btnClose.TabStop = false;
            this.btnClose.TextColor = System.Drawing.Color.Black;
            this.btnClose.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ToolTipText = "";
            this.btnClose.UseToolTip = false;
            this.btnClose.WindowRateWidth = 1F;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // PopupDetailLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(926, 547);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvSMS);
            this.Controls.Add(this.dgvAction);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PopupDetailLog";
            this.Text = "상세보기";
            this.Load += new System.EventHandler(this.PopupDetailLog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAction)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSMS)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvAction;
        private System.Windows.Forms.DataGridView dgvSMS;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel3;
        private UnE.GUI.ImageButton btnClose;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colManager;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBuilding;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFloor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNoSMS;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDateSMS;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLocationSMS;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTransferPersonSMS;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTransferContentSMS;
    }
}