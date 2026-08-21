namespace SOPBulletin
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
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panelStatusBottom = new System.Windows.Forms.Panel();
            this.panelStatusTop = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panelProgressBottom = new System.Windows.Forms.Panel();
            this.panelProgressTop = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panelStatusTop.SuspendLayout();
            this.panelProgressTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.OnTimer);
            // 
            // timer2
            // 
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.OnProcessedTimer);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panelStatusBottom);
            this.splitContainer1.Panel1.Controls.Add(this.panelStatusTop);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panelProgressBottom);
            this.splitContainer1.Panel2.Controls.Add(this.panelProgressTop);
            this.splitContainer1.Size = new System.Drawing.Size(942, 529);
            this.splitContainer1.SplitterDistance = 338;
            this.splitContainer1.TabIndex = 0;
            // 
            // panelStatusBottom
            // 
            this.panelStatusBottom.BackColor = System.Drawing.Color.White;
            this.panelStatusBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelStatusBottom.Location = new System.Drawing.Point(0, 20);
            this.panelStatusBottom.Name = "panelStatusBottom";
            this.panelStatusBottom.Size = new System.Drawing.Size(940, 316);
            this.panelStatusBottom.TabIndex = 1;
            // 
            // panelStatusTop
            // 
            this.panelStatusTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(124)))));
            this.panelStatusTop.Controls.Add(this.label2);
            this.panelStatusTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelStatusTop.Location = new System.Drawing.Point(0, 0);
            this.panelStatusTop.Name = "panelStatusTop";
            this.panelStatusTop.Size = new System.Drawing.Size(940, 20);
            this.panelStatusTop.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(6, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "상황판";
            // 
            // panelProgressBottom
            // 
            this.panelProgressBottom.BackColor = System.Drawing.Color.White;
            this.panelProgressBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelProgressBottom.Location = new System.Drawing.Point(0, 20);
            this.panelProgressBottom.Name = "panelProgressBottom";
            this.panelProgressBottom.Size = new System.Drawing.Size(940, 165);
            this.panelProgressBottom.TabIndex = 1;
            // 
            // panelProgressTop
            // 
            this.panelProgressTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(124)))));
            this.panelProgressTop.Controls.Add(this.label1);
            this.panelProgressTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelProgressTop.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panelProgressTop.Location = new System.Drawing.Point(0, 0);
            this.panelProgressTop.Name = "panelProgressTop";
            this.panelProgressTop.Size = new System.Drawing.Size(940, 20);
            this.panelProgressTop.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "SOP 진행현황";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(942, 529);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "SOP 상황판";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panelStatusTop.ResumeLayout(false);
            this.panelStatusTop.PerformLayout();
            this.panelProgressTop.ResumeLayout(false);
            this.panelProgressTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panelStatusBottom;
        private System.Windows.Forms.Panel panelStatusTop;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelProgressBottom;
        private System.Windows.Forms.Panel panelProgressTop;
        private System.Windows.Forms.Label label1;
    }
}

