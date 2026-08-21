namespace SOPMonitoringSystem
{
    partial class PopupExternalOption
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.button2 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.checkUseSMS = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.dataGridViewFax = new System.Windows.Forms.DataGridView();
			this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.textBox1 = new System.Windows.Forms.RichTextBox();
			this.dataGridViewSMS = new System.Windows.Forms.DataGridView();
			this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewFax)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewSMS)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.button2);
			this.groupBox1.Controls.Add(this.button1);
			this.groupBox1.Controls.Add(this.checkUseSMS);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.dataGridViewFax);
			this.groupBox1.Controls.Add(this.textBox1);
			this.groupBox1.Controls.Add(this.dataGridViewSMS);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(640, 367);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(484, 302);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(83, 30);
			this.button2.TabIndex = 19;
			this.button2.Text = "취소";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(395, 302);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(83, 30);
			this.button1.TabIndex = 20;
			this.button1.Text = "실행";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.runBtnClick);
			// 
			// checkUseSMS
			// 
			this.checkUseSMS.AutoSize = true;
			this.checkUseSMS.Enabled = false;
			this.checkUseSMS.Location = new System.Drawing.Point(21, 316);
			this.checkUseSMS.Name = "checkUseSMS";
			this.checkUseSMS.Size = new System.Drawing.Size(116, 16);
			this.checkUseSMS.TabIndex = 17;
			this.checkUseSMS.Text = "문자 메시지 사용";
			this.checkUseSMS.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(300, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(66, 12);
			this.label2.TabIndex = 16;
			this.label2.Text = "Fax 수신처";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(19, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(109, 12);
			this.label1.TabIndex = 15;
			this.label1.Text = "문자 메시지 수신처";
			// 
			// dataGridViewFax
			// 
			this.dataGridViewFax.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewFax.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
			this.dataGridViewFax.Location = new System.Drawing.Point(302, 52);
			this.dataGridViewFax.Name = "dataGridViewFax";
			this.dataGridViewFax.ReadOnly = true;
			this.dataGridViewFax.RowHeadersVisible = false;
			this.dataGridViewFax.RowTemplate.Height = 23;
			this.dataGridViewFax.Size = new System.Drawing.Size(265, 106);
			this.dataGridViewFax.TabIndex = 13;
			// 
			// dataGridViewTextBoxColumn1
			// 
			this.dataGridViewTextBoxColumn1.FillWeight = 150F;
			this.dataGridViewTextBoxColumn1.HeaderText = "수신처";
			this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
			this.dataGridViewTextBoxColumn1.ReadOnly = true;
			this.dataGridViewTextBoxColumn1.Width = 150;
			// 
			// dataGridViewTextBoxColumn2
			// 
			this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.dataGridViewTextBoxColumn2.HeaderText = "Fax 번호";
			this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
			this.dataGridViewTextBoxColumn2.ReadOnly = true;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(19, 180);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(266, 115);
			this.textBox1.TabIndex = 18;
			this.textBox1.Text = "";
			// 
			// dataGridViewSMS
			// 
			this.dataGridViewSMS.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewSMS.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
			this.dataGridViewSMS.Location = new System.Drawing.Point(19, 52);
			this.dataGridViewSMS.Name = "dataGridViewSMS";
			this.dataGridViewSMS.RowHeadersVisible = false;
			this.dataGridViewSMS.RowTemplate.Height = 23;
			this.dataGridViewSMS.Size = new System.Drawing.Size(265, 106);
			this.dataGridViewSMS.TabIndex = 14;
			// 
			// Column1
			// 
			this.Column1.FillWeight = 150F;
			this.Column1.HeaderText = "수신처";
			this.Column1.Name = "Column1";
			this.Column1.Width = 150;
			// 
			// Column2
			// 
			this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.Column2.HeaderText = "전화 번호";
			this.Column2.Name = "Column2";
			// 
			// PopupExternalOption
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(664, 393);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "PopupExternalOption";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "PopupExternalOption";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewFax)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewSMS)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkUseSMS;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridViewFax;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.RichTextBox textBox1;
        private System.Windows.Forms.DataGridView dataGridViewSMS;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;

    }
}