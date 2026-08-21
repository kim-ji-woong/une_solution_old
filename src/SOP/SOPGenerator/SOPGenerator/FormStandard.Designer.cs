namespace SOPGen
{
    partial class FormStandard
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
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cboStandard = new System.Windows.Forms.ComboBox();
            this.textStandard = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(12, 109);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(93, 23);
            this.btnSelect.TabIndex = 0;
            this.btnSelect.Text = "선택";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(111, 109);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(93, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cboStandard
            // 
            this.cboStandard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStandard.FormattingEnabled = true;
            this.cboStandard.Items.AddRange(new object[] {
            "1. 출력4만㎾이상 고장정지시",
            "2. 설비운영 지장사고",
            "3. 인명사고(사망사고1명이상)",
            "4. 재난사고(화재, 지진, 재해 등)"});
            this.cboStandard.Location = new System.Drawing.Point(12, 81);
            this.cboStandard.Name = "cboStandard";
            this.cboStandard.Size = new System.Drawing.Size(192, 20);
            this.cboStandard.TabIndex = 2;
            this.cboStandard.SelectedIndexChanged += new System.EventHandler(this.cboStandard_SelectedIndexChanged);
            // 
            // textStandard
            // 
            this.textStandard.BackColor = System.Drawing.SystemColors.Window;
            this.textStandard.Location = new System.Drawing.Point(12, 7);
            this.textStandard.Multiline = true;
            this.textStandard.Name = "textStandard";
            this.textStandard.ReadOnly = true;
            this.textStandard.Size = new System.Drawing.Size(192, 68);
            this.textStandard.TabIndex = 3;
            // 
            // FormStandard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(216, 145);
            this.Controls.Add(this.textStandard);
            this.Controls.Add(this.cboStandard);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSelect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormStandard";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SMS 표준 문구";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cboStandard;
        private System.Windows.Forms.TextBox textStandard;
    }
}