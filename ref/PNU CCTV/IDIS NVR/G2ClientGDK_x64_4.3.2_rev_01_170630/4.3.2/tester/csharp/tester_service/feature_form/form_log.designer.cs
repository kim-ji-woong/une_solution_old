namespace GDK_tester
{
    partial class form_log
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(form_log));
            this.CMB_LOG = new System.Windows.Forms.ComboBox();
            this.LSV_LOG = new System.Windows.Forms.ListView();
            this.LSV_LOG_COL_DATE_TIME = new System.Windows.Forms.ColumnHeader();
            this.LSV_LOG_COL_LOG = new System.Windows.Forms.ColumnHeader();
            this.LABEL_LOG = new System.Windows.Forms.Label();
            this.BTN_MORE = new System.Windows.Forms.Button();
            this.LABEL_STATUS = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // CMB_LOG
            // 
            this.CMB_LOG.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_LOG.FormattingEnabled = true;
            this.CMB_LOG.Items.AddRange(new object[] {
            "Service Log",
            "Service Log DEBUG",
            "Device Log"});
            this.CMB_LOG.Location = new System.Drawing.Point(12, 9);
            this.CMB_LOG.Name = "CMB_LOG";
            this.CMB_LOG.Size = new System.Drawing.Size(296, 20);
            this.CMB_LOG.TabIndex = 0;
            this.CMB_LOG.SelectedIndexChanged += new System.EventHandler(this.update_log_list);
            this.CMB_LOG.Layout += new System.Windows.Forms.LayoutEventHandler(this.begin_update_log_list);
            // 
            // LSV_LOG
            // 
            this.LSV_LOG.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LSV_LOG_COL_DATE_TIME,
            this.LSV_LOG_COL_LOG});
            this.LSV_LOG.GridLines = true;
            this.LSV_LOG.Location = new System.Drawing.Point(12, 35);
            this.LSV_LOG.Name = "LSV_LOG";
            this.LSV_LOG.Size = new System.Drawing.Size(912, 531);
            this.LSV_LOG.TabIndex = 1;
            this.LSV_LOG.UseCompatibleStateImageBehavior = false;
            this.LSV_LOG.View = System.Windows.Forms.View.Details;
            // 
            // LSV_LOG_COL_DATE_TIME
            // 
            this.LSV_LOG_COL_DATE_TIME.Text = "Date/Time";
            this.LSV_LOG_COL_DATE_TIME.Width = 138;
            // 
            // LSV_LOG_COL_LOG
            // 
            this.LSV_LOG_COL_LOG.Text = "Log";
            this.LSV_LOG_COL_LOG.Width = 769;
            // 
            // LABEL_LOG
            // 
            this.LABEL_LOG.AutoSize = true;
            this.LABEL_LOG.Location = new System.Drawing.Point(847, 13);
            this.LABEL_LOG.Name = "LABEL_LOG";
            this.LABEL_LOG.Size = new System.Drawing.Size(38, 12);
            this.LABEL_LOG.TabIndex = 2;
            this.LABEL_LOG.Text = "label1";
            // 
            // BTN_MORE
            // 
            this.BTN_MORE.Location = new System.Drawing.Point(821, 572);
            this.BTN_MORE.Name = "BTN_MORE";
            this.BTN_MORE.Size = new System.Drawing.Size(103, 22);
            this.BTN_MORE.TabIndex = 3;
            this.BTN_MORE.Text = "More";
            this.BTN_MORE.UseVisualStyleBackColor = true;
            this.BTN_MORE.Click += new System.EventHandler(this.on_btn_more);
            // 
            // LABEL_STATUS
            // 
            this.LABEL_STATUS.AutoSize = true;
            this.LABEL_STATUS.Location = new System.Drawing.Point(325, 14);
            this.LABEL_STATUS.Name = "LABEL_STATUS";
            this.LABEL_STATUS.Size = new System.Drawing.Size(62, 12);
            this.LABEL_STATUS.TabIndex = 4;
            this.LABEL_STATUS.Text = "Loading...";
            // 
            // form_log
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(939, 601);
            this.Controls.Add(this.LABEL_STATUS);
            this.Controls.Add(this.BTN_MORE);
            this.Controls.Add(this.LABEL_LOG);
            this.Controls.Add(this.LSV_LOG);
            this.Controls.Add(this.CMB_LOG);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "form_log";
            this.Text = "GDK Service L/o/g";
            this.Shown += new System.EventHandler(this.on_form_shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.on_form_closing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox CMB_LOG;
        private System.Windows.Forms.ListView LSV_LOG;
        private System.Windows.Forms.ColumnHeader LSV_LOG_COL_DATE_TIME;
        private System.Windows.Forms.ColumnHeader LSV_LOG_COL_LOG;
        private System.Windows.Forms.Label LABEL_LOG;
        private System.Windows.Forms.Button BTN_MORE;
        private System.Windows.Forms.Label LABEL_STATUS;
    }
}