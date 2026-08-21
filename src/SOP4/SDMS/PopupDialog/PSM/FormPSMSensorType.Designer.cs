namespace SDMS.PopupDialog
{
    partial class FormPSMSensorType
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
            this.textBoxTypeName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboLifeTime = new System.Windows.Forms.ComboBox();
            this.lblUserDefined = new System.Windows.Forms.Label();
            this.textBoxUserDefined = new System.Windows.Forms.TextBox();
            this.lblMonth = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnRemoveType = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "센서 타입명 :";
            // 
            // textBoxTypeName
            // 
            this.textBoxTypeName.Location = new System.Drawing.Point(95, 6);
            this.textBoxTypeName.Name = "textBoxTypeName";
            this.textBoxTypeName.Size = new System.Drawing.Size(140, 21);
            this.textBoxTypeName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "사용기한     :";
            // 
            // cboLifeTime
            // 
            this.cboLifeTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLifeTime.FormattingEnabled = true;
            this.cboLifeTime.Location = new System.Drawing.Point(95, 44);
            this.cboLifeTime.Name = "cboLifeTime";
            this.cboLifeTime.Size = new System.Drawing.Size(140, 20);
            this.cboLifeTime.TabIndex = 2;
            this.cboLifeTime.SelectedIndexChanged += new System.EventHandler(this.cboLifeTime_SelectedIndexChanged);
            // 
            // lblUserDefined
            // 
            this.lblUserDefined.AutoSize = true;
            this.lblUserDefined.Location = new System.Drawing.Point(43, 87);
            this.lblUserDefined.Name = "lblUserDefined";
            this.lblUserDefined.Size = new System.Drawing.Size(195, 12);
            this.lblUserDefined.TabIndex = 3;
            this.lblUserDefined.Text = "사용기한(개월수)을 입력해 주세요.";
            this.lblUserDefined.Visible = false;
            // 
            // textBoxUserDefined
            // 
            this.textBoxUserDefined.Location = new System.Drawing.Point(142, 102);
            this.textBoxUserDefined.Name = "textBoxUserDefined";
            this.textBoxUserDefined.Size = new System.Drawing.Size(55, 21);
            this.textBoxUserDefined.TabIndex = 4;
            this.textBoxUserDefined.Visible = false;
            // 
            // lblMonth
            // 
            this.lblMonth.AutoSize = true;
            this.lblMonth.Location = new System.Drawing.Point(202, 107);
            this.lblMonth.Name = "lblMonth";
            this.lblMonth.Size = new System.Drawing.Size(29, 12);
            this.lblMonth.TabIndex = 5;
            this.lblMonth.Text = "개월";
            this.lblMonth.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(186, 141);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(52, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(127, 141);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(52, 23);
            this.btnApply.TabIndex = 7;
            this.btnApply.Text = "확인";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnRemoveType
            // 
            this.btnRemoveType.Location = new System.Drawing.Point(14, 141);
            this.btnRemoveType.Name = "btnRemoveType";
            this.btnRemoveType.Size = new System.Drawing.Size(85, 23);
            this.btnRemoveType.TabIndex = 7;
            this.btnRemoveType.Text = "타입 지우기";
            this.btnRemoveType.UseVisualStyleBackColor = true;
            this.btnRemoveType.Click += new System.EventHandler(this.btnRemoveType_Click);
            // 
            // FormPSMSensorType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(250, 176);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnRemoveType);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.lblMonth);
            this.Controls.Add(this.textBoxUserDefined);
            this.Controls.Add(this.lblUserDefined);
            this.Controls.Add(this.cboLifeTime);
            this.Controls.Add(this.textBoxTypeName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormPSMSensorType";
            this.Text = "센서타입 정의";
            this.Load += new System.EventHandler(this.FormPSMSensorType_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxTypeName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboLifeTime;
        private System.Windows.Forms.Label lblUserDefined;
        private System.Windows.Forms.TextBox textBoxUserDefined;
        private System.Windows.Forms.Label lblMonth;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnRemoveType;
    }
}