namespace GDK_tester
{
    partial class form_violet_layout
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
            this.LB_MONITOR = new System.Windows.Forms.Label();
            this.EDT_VIOLET_LAYOUT_MON = new System.Windows.Forms.TextBox();
            this.BTN_OK = new System.Windows.Forms.Button();
            this.LB_LAYOUT = new System.Windows.Forms.Label();
            this.CBX_VIOLET_LAYOUT_LAYOUT = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // LB_MONITOR
            // 
            this.LB_MONITOR.AutoSize = true;
            this.LB_MONITOR.Location = new System.Drawing.Point(12, 9);
            this.LB_MONITOR.Name = "LB_MONITOR";
            this.LB_MONITOR.Size = new System.Drawing.Size(55, 12);
            this.LB_MONITOR.TabIndex = 0;
            this.LB_MONITOR.Text = "Monitor :";
            // 
            // EDT_VIOLET_LAYOUT_MON
            // 
            this.EDT_VIOLET_LAYOUT_MON.Location = new System.Drawing.Point(73, 6);
            this.EDT_VIOLET_LAYOUT_MON.Name = "EDT_VIOLET_LAYOUT_MON";
            this.EDT_VIOLET_LAYOUT_MON.Size = new System.Drawing.Size(100, 21);
            this.EDT_VIOLET_LAYOUT_MON.TabIndex = 1;
            // 
            // BTN_OK
            // 
            this.BTN_OK.Location = new System.Drawing.Point(179, 4);
            this.BTN_OK.Name = "BTN_OK";
            this.BTN_OK.Size = new System.Drawing.Size(75, 23);
            this.BTN_OK.TabIndex = 2;
            this.BTN_OK.Text = "Ok";
            this.BTN_OK.UseVisualStyleBackColor = true;
            // 
            // LB_LAYOUT
            // 
            this.LB_LAYOUT.AutoSize = true;
            this.LB_LAYOUT.Location = new System.Drawing.Point(16, 41);
            this.LB_LAYOUT.Name = "LB_LAYOUT";
            this.LB_LAYOUT.Size = new System.Drawing.Size(51, 12);
            this.LB_LAYOUT.TabIndex = 3;
            this.LB_LAYOUT.Text = "Layout :";
            // 
            // CBX_VIOLET_LAYOUT_LAYOUT
            // 
            this.CBX_VIOLET_LAYOUT_LAYOUT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CBX_VIOLET_LAYOUT_LAYOUT.FormattingEnabled = true;
            this.CBX_VIOLET_LAYOUT_LAYOUT.Location = new System.Drawing.Point(73, 38);
            this.CBX_VIOLET_LAYOUT_LAYOUT.Name = "CBX_VIOLET_LAYOUT_LAYOUT";
            this.CBX_VIOLET_LAYOUT_LAYOUT.Size = new System.Drawing.Size(100, 20);
            this.CBX_VIOLET_LAYOUT_LAYOUT.TabIndex = 4;
            // 
            // form_violet_layout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(262, 68);
            this.Controls.Add(this.CBX_VIOLET_LAYOUT_LAYOUT);
            this.Controls.Add(this.LB_LAYOUT);
            this.Controls.Add(this.BTN_OK);
            this.Controls.Add(this.EDT_VIOLET_LAYOUT_MON);
            this.Controls.Add(this.LB_MONITOR);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "form_violet_layout";
            this.Text = "Set Layout";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LB_MONITOR;
        private System.Windows.Forms.TextBox EDT_VIOLET_LAYOUT_MON;
        private System.Windows.Forms.Button BTN_OK;
        private System.Windows.Forms.Label LB_LAYOUT;
        private System.Windows.Forms.ComboBox CBX_VIOLET_LAYOUT_LAYOUT;
    }
}