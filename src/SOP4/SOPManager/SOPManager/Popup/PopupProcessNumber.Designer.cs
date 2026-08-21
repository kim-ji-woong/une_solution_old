namespace SOPManager
{
    partial class PopupProcessNumber
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopupProcessNumber));
			this.cboOption = new System.Windows.Forms.ComboBox();
			this.cboNumber = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.label4 = new System.Windows.Forms.Label();
			this.panel2.SuspendLayout();
			this.panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// cboOption
			// 
			this.cboOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboOption.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.cboOption.FormattingEnabled = true;
			this.cboOption.Items.AddRange(new object[] {
            "전체기간중",
            "연중",
            "월중",
            "주중",
            "하루중",
            "시간당"});
			this.cboOption.Location = new System.Drawing.Point(22, 54);
			this.cboOption.Name = "cboOption";
			this.cboOption.Size = new System.Drawing.Size(122, 28);
			this.cboOption.TabIndex = 0;
			this.cboOption.SelectedIndexChanged += new System.EventHandler(this.cboOption_SelectedIndexChanged);
			// 
			// cboNumber
			// 
			this.cboNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboNumber.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.cboNumber.FormattingEnabled = true;
			this.cboNumber.Location = new System.Drawing.Point(150, 54);
			this.cboNumber.Name = "cboNumber";
			this.cboNumber.Size = new System.Drawing.Size(87, 28);
			this.cboNumber.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
			this.label1.Location = new System.Drawing.Point(244, 58);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(24, 20);
			this.label1.TabIndex = 4;
			this.label1.Text = "회";
			// 
			// btnOK
			// 
			this.btnOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
			this.btnOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
			this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnOK.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold);
			this.btnOK.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
			this.btnOK.Location = new System.Drawing.Point(28, 105);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(105, 27);
			this.btnOK.TabIndex = 16;
			this.btnOK.Text = "확인";
			this.btnOK.UseVisualStyleBackColor = false;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// button1
			// 
			this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
			this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
			this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button1.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold);
			this.button1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
			this.button1.Location = new System.Drawing.Point(145, 105);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(105, 27);
			this.button1.TabIndex = 16;
			this.button1.Text = "취소";
			this.button1.UseVisualStyleBackColor = false;
			this.button1.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
			this.panel2.Controls.Add(this.panel3);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Padding = new System.Windows.Forms.Padding(3);
			this.panel2.Size = new System.Drawing.Size(282, 159);
			this.panel2.TabIndex = 23;
			// 
			// panel3
			// 
			this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
			this.panel3.Controls.Add(this.label4);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(3, 3);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(276, 153);
			this.panel3.TabIndex = 0;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
			this.label4.Location = new System.Drawing.Point(21, 17);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(182, 20);
			this.label4.TabIndex = 3;
			this.label4.Text = "처리횟수를 지정하십시오.";
			// 
			// PopupProcessNumber
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
			this.ClientSize = new System.Drawing.Size(282, 159);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cboNumber);
			this.Controls.Add(this.cboOption);
			this.Controls.Add(this.panel2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PopupProcessNumber";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "처리횟수";
			this.Load += new System.EventHandler(this.PopupProcessNumber_Load);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PopupProcessNumber_MouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PopupProcessNumber_MouseMove);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PopupProcessNumber_MouseUp);
			this.panel2.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboOption;
        private System.Windows.Forms.ComboBox cboNumber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
		protected System.Windows.Forms.Label label4;
    }
}