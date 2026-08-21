namespace SDMS.PopupDialog
{
    partial class FormPSMSensorLifeTime
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
            this.label1 = new System.Windows.Forms.Label();
            this.lblSensorNo = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblMaterialName = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblLocation = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.dtpIntallDate = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.cboSenosrType = new System.Windows.Forms.ComboBox();
            this.btnNewType = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.lblDeadLine = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "센서번호 :";
            // 
            // lblSensorNo
            // 
            this.lblSensorNo.AutoSize = true;
            this.lblSensorNo.Location = new System.Drawing.Point(76, 9);
            this.lblSensorNo.Name = "lblSensorNo";
            this.lblSensorNo.Size = new System.Drawing.Size(57, 12);
            this.lblSensorNo.TabIndex = 1;
            this.lblSensorNo.Text = "센서 번호";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "물질명    :";
            // 
            // lblMaterialName
            // 
            this.lblMaterialName.AutoSize = true;
            this.lblMaterialName.Location = new System.Drawing.Point(76, 31);
            this.lblMaterialName.Name = "lblMaterialName";
            this.lblMaterialName.Size = new System.Drawing.Size(57, 12);
            this.lblMaterialName.TabIndex = 1;
            this.lblMaterialName.Text = "물질 이름";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "위치       :";
            // 
            // lblLocation
            // 
            this.lblLocation.AutoSize = true;
            this.lblLocation.Location = new System.Drawing.Point(76, 55);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(57, 12);
            this.lblLocation.TabIndex = 1;
            this.lblLocation.Text = "위치 이름";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 119);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "설치일자 :";
            // 
            // dtpIntallDate
            // 
            this.dtpIntallDate.Location = new System.Drawing.Point(78, 115);
            this.dtpIntallDate.Name = "dtpIntallDate";
            this.dtpIntallDate.Size = new System.Drawing.Size(200, 21);
            this.dtpIntallDate.TabIndex = 2;
            this.dtpIntallDate.ValueChanged += new System.EventHandler(this.dtpIntallDate_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 95);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(61, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "설치타입 :";
            // 
            // cboSenosrType
            // 
            this.cboSenosrType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSenosrType.FormattingEnabled = true;
            this.cboSenosrType.Location = new System.Drawing.Point(78, 92);
            this.cboSenosrType.Name = "cboSenosrType";
            this.cboSenosrType.Size = new System.Drawing.Size(142, 20);
            this.cboSenosrType.TabIndex = 3;
            this.cboSenosrType.SelectedIndexChanged += new System.EventHandler(this.cboSenosrType_SelectedIndexChanged);
            // 
            // btnNewType
            // 
            this.btnNewType.Location = new System.Drawing.Point(226, 91);
            this.btnNewType.Name = "btnNewType";
            this.btnNewType.Size = new System.Drawing.Size(53, 23);
            this.btnNewType.TabIndex = 4;
            this.btnNewType.Text = "새 타입";
            this.btnNewType.UseVisualStyleBackColor = true;
            this.btnNewType.Click += new System.EventHandler(this.btnNewType_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 143);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "사용기한 :";
            // 
            // lblDeadLine
            // 
            this.lblDeadLine.AutoSize = true;
            this.lblDeadLine.Location = new System.Drawing.Point(76, 143);
            this.lblDeadLine.Name = "lblDeadLine";
            this.lblDeadLine.Size = new System.Drawing.Size(57, 12);
            this.lblDeadLine.TabIndex = 1;
            this.lblDeadLine.Text = "사용 기한";
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(168, 167);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(52, 23);
            this.btnApply.TabIndex = 5;
            this.btnApply.Text = "확인";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(226, 167);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(52, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FormPSMSensorLifeTime
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(287, 202);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnNewType);
            this.Controls.Add(this.cboSenosrType);
            this.Controls.Add(this.dtpIntallDate);
            this.Controls.Add(this.lblDeadLine);
            this.Controls.Add(this.lblLocation);
            this.Controls.Add(this.lblMaterialName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblSensorNo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormPSMSensorLifeTime";
            this.Text = "센서 교체주기";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblSensorNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblMaterialName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dtpIntallDate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cboSenosrType;
        private System.Windows.Forms.Button btnNewType;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblDeadLine;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
    }
}