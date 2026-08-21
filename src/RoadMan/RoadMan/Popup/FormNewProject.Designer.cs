namespace RoadMan
{
    partial class FormNewProject
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormNewProject));
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxDXFPath = new System.Windows.Forms.TextBox();
			this.btnDXFPath = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(24, 35);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(89, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "도면파일 경로 :";
			// 
			// textBoxDXFPath
			// 
			this.textBoxDXFPath.Location = new System.Drawing.Point(119, 32);
			this.textBoxDXFPath.Name = "textBoxDXFPath";
			this.textBoxDXFPath.Size = new System.Drawing.Size(210, 21);
			this.textBoxDXFPath.TabIndex = 1;
			// 
			// btnDXFPath
			// 
			this.btnDXFPath.Location = new System.Drawing.Point(335, 30);
			this.btnDXFPath.Name = "btnDXFPath";
			this.btnDXFPath.Size = new System.Drawing.Size(27, 23);
			this.btnDXFPath.TabIndex = 2;
			this.btnDXFPath.Text = "...";
			this.btnDXFPath.UseVisualStyleBackColor = true;
			this.btnDXFPath.Click += new System.EventHandler(this.btnDXFPath_Click);
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(240, 84);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(58, 23);
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "확인";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(304, 84);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(58, 23);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "취소";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// FormNewProject
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(384, 124);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnDXFPath);
			this.Controls.Add(this.textBoxDXFPath);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormNewProject";
			this.ShowInTaskbar = false;
			this.Text = "새 프로젝트";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxDXFPath;
        private System.Windows.Forms.Button btnDXFPath;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}