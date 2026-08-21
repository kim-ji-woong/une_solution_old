namespace RoadMan
{
    partial class FormEditSection
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
            this.panelArea = new System.Windows.Forms.Panel();
            this.radioEditPolygon = new System.Windows.Forms.RadioButton();
            this.radioSelectArea = new System.Windows.Forms.RadioButton();
            this.panelPrev = new System.Windows.Forms.Panel();
            this.radioPrevBoundary = new System.Windows.Forms.RadioButton();
            this.radioPrevDirect = new System.Windows.Forms.RadioButton();
            this.panelNext = new System.Windows.Forms.Panel();
            this.radioNextBoundary = new System.Windows.Forms.RadioButton();
            this.radioNextDirect = new System.Windows.Forms.RadioButton();
            this.panelArea.SuspendLayout();
            this.panelPrev.SuspendLayout();
            this.panelNext.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelArea
            // 
            this.panelArea.BackColor = System.Drawing.Color.White;
            this.panelArea.Controls.Add(this.radioEditPolygon);
            this.panelArea.Controls.Add(this.radioSelectArea);
            this.panelArea.Location = new System.Drawing.Point(12, 12);
            this.panelArea.Name = "panelArea";
            this.panelArea.Size = new System.Drawing.Size(150, 90);
            this.panelArea.TabIndex = 0;
            // 
            // radioEditPolygon
            // 
            this.radioEditPolygon.AutoSize = true;
            this.radioEditPolygon.Location = new System.Drawing.Point(15, 54);
            this.radioEditPolygon.Name = "radioEditPolygon";
            this.radioEditPolygon.Size = new System.Drawing.Size(75, 16);
            this.radioEditPolygon.TabIndex = 0;
            this.radioEditPolygon.TabStop = true;
            this.radioEditPolygon.Text = "영역 편집";
            this.radioEditPolygon.UseVisualStyleBackColor = true;
            this.radioEditPolygon.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioSelectArea
            // 
            this.radioSelectArea.AutoSize = true;
            this.radioSelectArea.Location = new System.Drawing.Point(15, 19);
            this.radioSelectArea.Name = "radioSelectArea";
            this.radioSelectArea.Size = new System.Drawing.Size(75, 16);
            this.radioSelectArea.TabIndex = 0;
            this.radioSelectArea.TabStop = true;
            this.radioSelectArea.Text = "영역 선택";
            this.radioSelectArea.UseVisualStyleBackColor = true;
            this.radioSelectArea.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // panelPrev
            // 
            this.panelPrev.BackColor = System.Drawing.Color.White;
            this.panelPrev.Controls.Add(this.radioPrevBoundary);
            this.panelPrev.Controls.Add(this.radioPrevDirect);
            this.panelPrev.Location = new System.Drawing.Point(183, 12);
            this.panelPrev.Name = "panelPrev";
            this.panelPrev.Size = new System.Drawing.Size(253, 90);
            this.panelPrev.TabIndex = 0;
            // 
            // radioPrevBoundary
            // 
            this.radioPrevBoundary.AutoSize = true;
            this.radioPrevBoundary.Location = new System.Drawing.Point(15, 54);
            this.radioPrevBoundary.Name = "radioPrevBoundary";
            this.radioPrevBoundary.Size = new System.Drawing.Size(183, 16);
            this.radioPrevBoundary.TabIndex = 0;
            this.radioPrevBoundary.TabStop = true;
            this.radioPrevBoundary.Text = "이전 점과 외곽선을 따라 연결";
            this.radioPrevBoundary.UseVisualStyleBackColor = true;
            this.radioPrevBoundary.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioPrevDirect
            // 
            this.radioPrevDirect.AutoSize = true;
            this.radioPrevDirect.Location = new System.Drawing.Point(15, 19);
            this.radioPrevDirect.Name = "radioPrevDirect";
            this.radioPrevDirect.Size = new System.Drawing.Size(131, 16);
            this.radioPrevDirect.TabIndex = 0;
            this.radioPrevDirect.TabStop = true;
            this.radioPrevDirect.Text = "이전 점과 직접 연결";
            this.radioPrevDirect.UseVisualStyleBackColor = true;
            this.radioPrevDirect.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // panelNext
            // 
            this.panelNext.BackColor = System.Drawing.Color.White;
            this.panelNext.Controls.Add(this.radioNextBoundary);
            this.panelNext.Controls.Add(this.radioNextDirect);
            this.panelNext.Location = new System.Drawing.Point(458, 12);
            this.panelNext.Name = "panelNext";
            this.panelNext.Size = new System.Drawing.Size(253, 90);
            this.panelNext.TabIndex = 0;
            // 
            // radioNextBoundary
            // 
            this.radioNextBoundary.AutoSize = true;
            this.radioNextBoundary.Location = new System.Drawing.Point(15, 54);
            this.radioNextBoundary.Name = "radioNextBoundary";
            this.radioNextBoundary.Size = new System.Drawing.Size(183, 16);
            this.radioNextBoundary.TabIndex = 0;
            this.radioNextBoundary.TabStop = true;
            this.radioNextBoundary.Text = "다음 점과 외곽선을 따라 연결";
            this.radioNextBoundary.UseVisualStyleBackColor = true;
            this.radioNextBoundary.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioNextDirect
            // 
            this.radioNextDirect.AutoSize = true;
            this.radioNextDirect.Location = new System.Drawing.Point(15, 19);
            this.radioNextDirect.Name = "radioNextDirect";
            this.radioNextDirect.Size = new System.Drawing.Size(131, 16);
            this.radioNextDirect.TabIndex = 0;
            this.radioNextDirect.TabStop = true;
            this.radioNextDirect.Text = "다음 점과 직접 연결";
            this.radioNextDirect.UseVisualStyleBackColor = true;
            this.radioNextDirect.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // FormEditSection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(725, 112);
            this.Controls.Add(this.panelNext);
            this.Controls.Add(this.panelPrev);
            this.Controls.Add(this.panelArea);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormEditSection";
            this.ShowInTaskbar = false;
            this.Text = "구간 편집";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormEditSection_FormClosing);
            this.panelArea.ResumeLayout(false);
            this.panelArea.PerformLayout();
            this.panelPrev.ResumeLayout(false);
            this.panelPrev.PerformLayout();
            this.panelNext.ResumeLayout(false);
            this.panelNext.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelArea;
        private System.Windows.Forms.RadioButton radioSelectArea;
        private System.Windows.Forms.RadioButton radioEditPolygon;
        private System.Windows.Forms.Panel panelPrev;
        private System.Windows.Forms.RadioButton radioPrevBoundary;
        private System.Windows.Forms.RadioButton radioPrevDirect;
        private System.Windows.Forms.Panel panelNext;
        private System.Windows.Forms.RadioButton radioNextBoundary;
        private System.Windows.Forms.RadioButton radioNextDirect;
    }
}