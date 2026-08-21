namespace SOPBulletin
{
    partial class FormMain2
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain2));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panelStatusBottom = new System.Windows.Forms.Panel();
            this.panelStatusTop = new System.Windows.Forms.Panel();
            this.btnShowPrevLog = new System.Windows.Forms.Button();
            this.btnCloseCurrentLog = new System.Windows.Forms.Button();
            this.btnSaveToHWP = new System.Windows.Forms.Button();
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
            this.splitContainer1.SplitterDistance = 375;
            this.splitContainer1.TabIndex = 1;
            // 
            // panelStatusBottom
            // 
            this.panelStatusBottom.BackColor = System.Drawing.Color.White;
            this.panelStatusBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelStatusBottom.Location = new System.Drawing.Point(0, 20);
            this.panelStatusBottom.Name = "panelStatusBottom";
            this.panelStatusBottom.Size = new System.Drawing.Size(940, 353);
            this.panelStatusBottom.TabIndex = 0;
            // 
            // panelStatusTop
            // 
            this.panelStatusTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(124)))));
            this.panelStatusTop.Controls.Add(this.btnShowPrevLog);
            this.panelStatusTop.Controls.Add(this.btnCloseCurrentLog);
            this.panelStatusTop.Controls.Add(this.btnSaveToHWP);
            this.panelStatusTop.Controls.Add(this.label2);
            this.panelStatusTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelStatusTop.Location = new System.Drawing.Point(0, 0);
            this.panelStatusTop.Name = "panelStatusTop";
            this.panelStatusTop.Size = new System.Drawing.Size(940, 20);
            this.panelStatusTop.TabIndex = 0;
            // 
            // btnShowPrevLog
            // 
            this.btnShowPrevLog.Location = new System.Drawing.Point(342, 0);
            this.btnShowPrevLog.Name = "btnShowPrevLog";
            this.btnShowPrevLog.Size = new System.Drawing.Size(128, 19);
            this.btnShowPrevLog.TabIndex = 1;
            this.btnShowPrevLog.Text = "이전 SOP 로그 보기";
            this.btnShowPrevLog.UseVisualStyleBackColor = true;
            this.btnShowPrevLog.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnShowPrevLog_MouseDown);
            // 
            // btnCloseCurrentLog
            // 
            this.btnCloseCurrentLog.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCloseCurrentLog.Location = new System.Drawing.Point(208, 0);
            this.btnCloseCurrentLog.Name = "btnCloseCurrentLog";
            this.btnCloseCurrentLog.Size = new System.Drawing.Size(128, 21);
            this.btnCloseCurrentLog.TabIndex = 1;
            this.btnCloseCurrentLog.Text = "현재 SOP 로그 닫기";
            this.btnCloseCurrentLog.UseVisualStyleBackColor = true;
            this.btnCloseCurrentLog.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnCloseCurrentLog_MouseDown);
            // 
            // btnSaveToHWP
            // 
            this.btnSaveToHWP.Location = new System.Drawing.Point(94, 0);
            this.btnSaveToHWP.Name = "btnSaveToHWP";
            this.btnSaveToHWP.Size = new System.Drawing.Size(108, 19);
            this.btnSaveToHWP.TabIndex = 1;
            this.btnSaveToHWP.Text = "한글 파일 보기";
            this.btnSaveToHWP.UseVisualStyleBackColor = true;
            this.btnSaveToHWP.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnSaveToHWP_MouseDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(6, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "SOP 진행이력";
            // 
            // panelProgressBottom
            // 
            this.panelProgressBottom.BackColor = System.Drawing.Color.White;
            this.panelProgressBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelProgressBottom.Location = new System.Drawing.Point(0, 20);
            this.panelProgressBottom.Name = "panelProgressBottom";
            this.panelProgressBottom.Size = new System.Drawing.Size(940, 128);
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
            this.label1.Size = new System.Drawing.Size(70, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "SOP 진행률";
            // 
            // FormMain2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(942, 529);
            this.Controls.Add(this.splitContainer1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain2";
            this.Text = "SOP 상황판";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain2_FormClosing);
            this.Load += new System.EventHandler(this.FormMain2_Load);
            this.VisibleChanged += new System.EventHandler(this.FormMain2_VisibleChanged);
            this.Resize += new System.EventHandler(this.FormMain2_Resize);
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
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panelStatusBottom;
        private System.Windows.Forms.Panel panelStatusTop;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelProgressBottom;
        private System.Windows.Forms.Panel panelProgressTop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCloseCurrentLog;
        private System.Windows.Forms.Button btnSaveToHWP;
        private System.Windows.Forms.Button btnShowPrevLog;
    }
}