namespace SDMS
{
	partial class DockingFormSensorProperties
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
            this.label3 = new System.Windows.Forms.Label();
            this.labelType = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelManagerName = new System.Windows.Forms.Label();
            this.labelPhoneNumber = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelEquipNo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 17;
            this.label3.Text = "연락처";
            // 
            // labelType
            // 
            this.labelType.AutoSize = true;
            this.labelType.Location = new System.Drawing.Point(80, 14);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(57, 12);
            this.labelType.TabIndex = 13;
            this.labelType.Text = "설비 유형";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 11;
            this.label2.Text = "관리자";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 16;
            this.label1.Text = "유형";
            // 
            // labelManagerName
            // 
            this.labelManagerName.AutoSize = true;
            this.labelManagerName.Location = new System.Drawing.Point(80, 65);
            this.labelManagerName.Name = "labelManagerName";
            this.labelManagerName.Size = new System.Drawing.Size(69, 12);
            this.labelManagerName.TabIndex = 13;
            this.labelManagerName.Text = "관리자 이름";
            // 
            // labelPhoneNumber
            // 
            this.labelPhoneNumber.AutoSize = true;
            this.labelPhoneNumber.Location = new System.Drawing.Point(80, 93);
            this.labelPhoneNumber.Name = "labelPhoneNumber";
            this.labelPhoneNumber.Size = new System.Drawing.Size(81, 12);
            this.labelPhoneNumber.TabIndex = 13;
            this.labelPhoneNumber.Text = "관리자 연락처";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 16;
            this.label4.Text = "설비번호";
            // 
            // labelEquipNo
            // 
            this.labelEquipNo.AutoSize = true;
            this.labelEquipNo.Location = new System.Drawing.Point(80, 38);
            this.labelEquipNo.Name = "labelEquipNo";
            this.labelEquipNo.Size = new System.Drawing.Size(57, 12);
            this.labelEquipNo.TabIndex = 13;
            this.labelEquipNo.Text = "설비 번호";
            // 
            // DockingFormSensorProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelPhoneNumber);
            this.Controls.Add(this.labelManagerName);
            this.Controls.Add(this.labelEquipNo);
            this.Controls.Add(this.labelType);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DockingFormSensorProperties";
            this.Text = "DockingFormFireEquipmentProperties";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelType;
        private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelManagerName;
        private System.Windows.Forms.Label labelPhoneNumber;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelEquipNo;
    }
}