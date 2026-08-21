namespace RoadMan
{
    partial class FormSaveOption
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSaveOption));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.radioNoPassword = new System.Windows.Forms.RadioButton();
			this.radioSaveOnly = new System.Windows.Forms.RadioButton();
			this.radioReadWrite = new System.Windows.Forms.RadioButton();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxPassword = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(177, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "파일의 저장 옵션을 선택하세요.";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 33);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(233, 12);
			this.label2.TabIndex = 0;
			this.label2.Text = "보안을 위하여 옵션을 선택할 수 있습니다.";
			// 
			// radioNoPassword
			// 
			this.radioNoPassword.AutoSize = true;
			this.radioNoPassword.Checked = true;
			this.radioNoPassword.Location = new System.Drawing.Point(14, 72);
			this.radioNoPassword.Name = "radioNoPassword";
			this.radioNoPassword.Size = new System.Drawing.Size(127, 16);
			this.radioNoPassword.TabIndex = 1;
			this.radioNoPassword.TabStop = true;
			this.radioNoPassword.Text = "암호 사용하지 않음";
			this.radioNoPassword.UseVisualStyleBackColor = true;
			this.radioNoPassword.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
			// 
			// radioSaveOnly
			// 
			this.radioSaveOnly.AutoSize = true;
			this.radioSaveOnly.Location = new System.Drawing.Point(14, 96);
			this.radioSaveOnly.Name = "radioSaveOnly";
			this.radioSaveOnly.Size = new System.Drawing.Size(167, 16);
			this.radioSaveOnly.TabIndex = 1;
			this.radioSaveOnly.Text = "파일 변경시에만 암호 사용";
			this.radioSaveOnly.UseVisualStyleBackColor = true;
			this.radioSaveOnly.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
			// 
			// radioReadWrite
			// 
			this.radioReadWrite.AutoSize = true;
			this.radioReadWrite.Location = new System.Drawing.Point(14, 121);
			this.radioReadWrite.Name = "radioReadWrite";
			this.radioReadWrite.Size = new System.Drawing.Size(235, 16);
			this.radioReadWrite.TabIndex = 1;
			this.radioReadWrite.Text = "파일을 변경하거나 열때 모두 암호 사용";
			this.radioReadWrite.UseVisualStyleBackColor = true;
			this.radioReadWrite.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(151, 224);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(57, 26);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "확인";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(214, 224);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(57, 26);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "취소";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 166);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(37, 12);
			this.label3.TabIndex = 3;
			this.label3.Text = "암호 :";
			// 
			// textBoxPassword
			// 
			this.textBoxPassword.Location = new System.Drawing.Point(55, 163);
			this.textBoxPassword.Name = "textBoxPassword";
			this.textBoxPassword.PasswordChar = '*';
			this.textBoxPassword.Size = new System.Drawing.Size(216, 21);
			this.textBoxPassword.TabIndex = 4;
			this.textBoxPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxPassword_KeyDown);
			// 
			// FormSaveOption
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.textBoxPassword);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.radioReadWrite);
			this.Controls.Add(this.radioSaveOnly);
			this.Controls.Add(this.radioNoPassword);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSaveOption";
			this.Text = "파일 저장 옵션";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radioNoPassword;
        private System.Windows.Forms.RadioButton radioSaveOnly;
        private System.Windows.Forms.RadioButton radioReadWrite;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxPassword;
    }
}