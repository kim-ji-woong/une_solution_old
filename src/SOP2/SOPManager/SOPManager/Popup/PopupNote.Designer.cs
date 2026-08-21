namespace SOPManager
{
    partial class PopupNote
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
			this.labelNote = new System.Windows.Forms.Label();
			this.textBox = new System.Windows.Forms.TextBox();
			this.labelWarning = new System.Windows.Forms.Label();
			this.labelWarning2 = new System.Windows.Forms.Label();
			this.labelWarning3 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnStandard = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelNote
			// 
			this.labelNote.AutoSize = true;
			this.labelNote.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.labelNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
			this.labelNote.Location = new System.Drawing.Point(12, 20);
			this.labelNote.Name = "labelNote";
			this.labelNote.Size = new System.Drawing.Size(54, 20);
			this.labelNote.TabIndex = 0;
			this.labelNote.Text = "메시지";
			// 
			// textBox
			// 
			this.textBox.Location = new System.Drawing.Point(14, 126);
			this.textBox.Multiline = true;
			this.textBox.Name = "textBox";
			this.textBox.Size = new System.Drawing.Size(407, 124);
			this.textBox.TabIndex = 1;
			// 
			// labelWarning
			// 
			this.labelWarning.AutoSize = true;
			this.labelWarning.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.labelWarning.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
			this.labelWarning.Location = new System.Drawing.Point(11, 48);
			this.labelWarning.Name = "labelWarning";
			this.labelWarning.Size = new System.Drawing.Size(287, 20);
			this.labelWarning.TabIndex = 0;
			this.labelWarning.Text = "(외부로 임무 내용이 전파될 수 있으므로, ";
			// 
			// labelWarning2
			// 
			this.labelWarning2.AutoSize = true;
			this.labelWarning2.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.labelWarning2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
			this.labelWarning2.Location = new System.Drawing.Point(14, 69);
			this.labelWarning2.Name = "labelWarning2";
			this.labelWarning2.Size = new System.Drawing.Size(309, 20);
			this.labelWarning2.TabIndex = 0;
			this.labelWarning2.Text = "개인 정보 보호를 위해서 특정 개인의 정보는";
			// 
			// labelWarning3
			// 
			this.labelWarning3.AutoSize = true;
			this.labelWarning3.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.labelWarning3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
			this.labelWarning3.Location = new System.Drawing.Point(14, 90);
			this.labelWarning3.Name = "labelWarning3";
			this.labelWarning3.Size = new System.Drawing.Size(177, 20);
			this.labelWarning3.TabIndex = 0;
			this.labelWarning3.Text = "입력하지 말아 주십시오.)";
			// 
			// btnOK
			// 
			this.btnOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
			this.btnOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
			this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnOK.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold);
			this.btnOK.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
			this.btnOK.Location = new System.Drawing.Point(94, 265);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(120, 27);
			this.btnOK.TabIndex = 16;
			this.btnOK.Text = "확인";
			this.btnOK.UseVisualStyleBackColor = false;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
			this.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
			this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnCancel.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold);
			this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
			this.btnCancel.Location = new System.Drawing.Point(225, 265);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(120, 27);
			this.btnCancel.TabIndex = 16;
			this.btnCancel.Text = "취소";
			this.btnCancel.UseVisualStyleBackColor = false;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnStandard
			// 
			this.btnStandard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnStandard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
			this.btnStandard.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
			this.btnStandard.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
			this.btnStandard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnStandard.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold);
			this.btnStandard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
			this.btnStandard.Location = new System.Drawing.Point(321, 27);
			this.btnStandard.Name = "btnStandard";
			this.btnStandard.Size = new System.Drawing.Size(96, 27);
			this.btnStandard.TabIndex = 16;
			this.btnStandard.Text = "표준문구";
			this.btnStandard.UseVisualStyleBackColor = false;
			this.btnStandard.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
			this.panel2.Controls.Add(this.panel3);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Padding = new System.Windows.Forms.Padding(3);
			this.panel2.Size = new System.Drawing.Size(433, 306);
			this.panel2.TabIndex = 23;
			// 
			// panel3
			// 
			this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(3, 3);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(427, 300);
			this.panel3.TabIndex = 0;
			// 
			// PopupNote
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
			this.ClientSize = new System.Drawing.Size(433, 306);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnStandard);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.textBox);
			this.Controls.Add(this.labelWarning3);
			this.Controls.Add(this.labelWarning2);
			this.Controls.Add(this.labelWarning);
			this.Controls.Add(this.labelNote);
			this.Controls.Add(this.panel2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PopupNote";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "메시지 작성";
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PopupNote_MouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PopupNote_MouseMove);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PopupNote_MouseUp);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelNote;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Label labelWarning;
        private System.Windows.Forms.Label labelWarning2;
        private System.Windows.Forms.Label labelWarning3;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnStandard;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
    }
}