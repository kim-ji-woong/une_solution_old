namespace GDK_tester
{
    partial class form_rec_failover
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
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
            this.DGV_REC_FAILOVER = new System.Windows.Forms.DataGridView();
            this.BTN_SWITCH_FAILOVER = new System.Windows.Forms.Button();
            this.BTN_CANCEL = new System.Windows.Forms.Button();
            this.scopeId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Scope = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_REC_FAILOVER)).BeginInit();
            this.SuspendLayout();
            // 
            // DGV_REC_FAILOVER
            // 
            this.DGV_REC_FAILOVER.AllowUserToAddRows = false;
            this.DGV_REC_FAILOVER.AllowUserToDeleteRows = false;
            this.DGV_REC_FAILOVER.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_REC_FAILOVER.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.scopeId,
            this.Scope});
            this.DGV_REC_FAILOVER.Enabled = false;
            this.DGV_REC_FAILOVER.Location = new System.Drawing.Point(13, 11);
            this.DGV_REC_FAILOVER.MultiSelect = false;
            this.DGV_REC_FAILOVER.Name = "DGV_REC_FAILOVER";
            this.DGV_REC_FAILOVER.ReadOnly = true;
            this.DGV_REC_FAILOVER.RowHeadersWidth = 5;
            this.DGV_REC_FAILOVER.RowTemplate.Height = 23;
            this.DGV_REC_FAILOVER.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.ColumnHeaderSelect;
            this.DGV_REC_FAILOVER.Size = new System.Drawing.Size(481, 203);
            this.DGV_REC_FAILOVER.TabIndex = 1;
            // 
            // BTN_SWITCH_FAILOVER
            // 
            this.BTN_SWITCH_FAILOVER.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BTN_SWITCH_FAILOVER.Enabled = false;
            this.BTN_SWITCH_FAILOVER.Location = new System.Drawing.Point(304, 227);
            this.BTN_SWITCH_FAILOVER.Name = "BTN_SWITCH_FAILOVER";
            this.BTN_SWITCH_FAILOVER.Size = new System.Drawing.Size(109, 23);
            this.BTN_SWITCH_FAILOVER.TabIndex = 2;
            this.BTN_SWITCH_FAILOVER.Text = "switch to failover";
            this.BTN_SWITCH_FAILOVER.UseVisualStyleBackColor = true;
            // 
            // BTN_CANCEL
            // 
            this.BTN_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BTN_CANCEL.Location = new System.Drawing.Point(419, 227);
            this.BTN_CANCEL.Name = "BTN_CANCEL";
            this.BTN_CANCEL.Size = new System.Drawing.Size(75, 23);
            this.BTN_CANCEL.TabIndex = 3;
            this.BTN_CANCEL.Text = "cancel";
            this.BTN_CANCEL.UseVisualStyleBackColor = true;
            // 
            // scopeId
            // 
            this.scopeId.HeaderText = "scope id";
            this.scopeId.Name = "scopeId";
            this.scopeId.ReadOnly = true;
            this.scopeId.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.scopeId.Width = 80;
            // 
            // Scope
            // 
            this.Scope.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Scope.HeaderText = "record scope";
            this.Scope.Name = "Scope";
            this.Scope.ReadOnly = true;
            this.Scope.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // form_rec_failover
            // 
            this.AcceptButton = this.BTN_SWITCH_FAILOVER;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BTN_CANCEL;
            this.ClientSize = new System.Drawing.Size(507, 261);
            this.Controls.Add(this.BTN_CANCEL);
            this.Controls.Add(this.BTN_SWITCH_FAILOVER);
            this.Controls.Add(this.DGV_REC_FAILOVER);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "form_rec_failover";
            this.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "recording failover information";
            ((System.ComponentModel.ISupportInitialize)(this.DGV_REC_FAILOVER)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView DGV_REC_FAILOVER;
        private System.Windows.Forms.Button BTN_SWITCH_FAILOVER;
        private System.Windows.Forms.Button BTN_CANCEL;
        private System.Windows.Forms.DataGridViewTextBoxColumn scopeId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Scope;


    }
}
