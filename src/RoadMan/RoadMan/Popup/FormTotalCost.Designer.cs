namespace RoadMan
{
    partial class FormTotalCost
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTotalCost));
			this.labelAddrName = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.btnMillionAroundCost = new System.Windows.Forms.Button();
			this.labelTotalCost = new System.Windows.Forms.Label();
			this.btnMillionObjectCost = new System.Windows.Forms.Button();
			this.label11 = new System.Windows.Forms.Label();
			this.btnMillionLandCost = new System.Windows.Forms.Button();
			this.label12 = new System.Windows.Forms.Label();
			this.textBoxAroundCost = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.textBoxObjectCost = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.textBoxLandCost = new System.Windows.Forms.TextBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelAddrName
			// 
			this.labelAddrName.AutoSize = true;
			this.labelAddrName.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.labelAddrName.Location = new System.Drawing.Point(12, 9);
			this.labelAddrName.Name = "labelAddrName";
			this.labelAddrName.Size = new System.Drawing.Size(58, 21);
			this.labelAddrName.TabIndex = 1;
			this.labelAddrName.Text = "구간명";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.btnMillionAroundCost);
			this.groupBox3.Controls.Add(this.labelTotalCost);
			this.groupBox3.Controls.Add(this.btnMillionObjectCost);
			this.groupBox3.Controls.Add(this.label11);
			this.groupBox3.Controls.Add(this.btnMillionLandCost);
			this.groupBox3.Controls.Add(this.label12);
			this.groupBox3.Controls.Add(this.textBoxAroundCost);
			this.groupBox3.Controls.Add(this.label10);
			this.groupBox3.Controls.Add(this.textBoxObjectCost);
			this.groupBox3.Controls.Add(this.label1);
			this.groupBox3.Controls.Add(this.label13);
			this.groupBox3.Controls.Add(this.textBoxLandCost);
			this.groupBox3.Location = new System.Drawing.Point(8, 44);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(312, 162);
			this.groupBox3.TabIndex = 7;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "사업비(원)";
			// 
			// btnMillionAroundCost
			// 
			this.btnMillionAroundCost.Location = new System.Drawing.Point(242, 116);
			this.btnMillionAroundCost.Name = "btnMillionAroundCost";
			this.btnMillionAroundCost.Size = new System.Drawing.Size(62, 23);
			this.btnMillionAroundCost.TabIndex = 9;
			this.btnMillionAroundCost.Text = "백만원";
			this.btnMillionAroundCost.UseVisualStyleBackColor = true;
			this.btnMillionAroundCost.Click += new System.EventHandler(this.btnMillion_Click);
			// 
			// labelTotalCost
			// 
			this.labelTotalCost.AutoSize = true;
			this.labelTotalCost.Location = new System.Drawing.Point(103, 27);
			this.labelTotalCost.Name = "labelTotalCost";
			this.labelTotalCost.Size = new System.Drawing.Size(41, 12);
			this.labelTotalCost.TabIndex = 2;
			this.labelTotalCost.Text = "사업비";
			// 
			// btnMillionObjectCost
			// 
			this.btnMillionObjectCost.Location = new System.Drawing.Point(242, 89);
			this.btnMillionObjectCost.Name = "btnMillionObjectCost";
			this.btnMillionObjectCost.Size = new System.Drawing.Size(62, 23);
			this.btnMillionObjectCost.TabIndex = 10;
			this.btnMillionObjectCost.Text = "백만원";
			this.btnMillionObjectCost.UseVisualStyleBackColor = true;
			this.btnMillionObjectCost.Click += new System.EventHandler(this.btnMillion_Click);
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(20, 27);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(77, 12);
			this.label11.TabIndex = 1;
			this.label11.Text = "사업비 총괄 :";
			// 
			// btnMillionLandCost
			// 
			this.btnMillionLandCost.Location = new System.Drawing.Point(242, 51);
			this.btnMillionLandCost.Name = "btnMillionLandCost";
			this.btnMillionLandCost.Size = new System.Drawing.Size(62, 23);
			this.btnMillionLandCost.TabIndex = 11;
			this.btnMillionLandCost.Text = "백만원";
			this.btnMillionLandCost.UseVisualStyleBackColor = true;
			this.btnMillionLandCost.Click += new System.EventHandler(this.btnMillion_Click);
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(20, 120);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(77, 12);
			this.label12.TabIndex = 1;
			this.label12.Text = "개략 공사비 :";
			// 
			// textBoxAroundCost
			// 
			this.textBoxAroundCost.Location = new System.Drawing.Point(131, 117);
			this.textBoxAroundCost.Name = "textBoxAroundCost";
			this.textBoxAroundCost.Size = new System.Drawing.Size(105, 21);
			this.textBoxAroundCost.TabIndex = 2;
			this.textBoxAroundCost.TextChanged += new System.EventHandler(this.textBox_TextChanged);
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(20, 93);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(89, 12);
			this.label10.TabIndex = 1;
			this.label10.Text = "지장물 보상비 :";
			// 
			// textBoxObjectCost
			// 
			this.textBoxObjectCost.Location = new System.Drawing.Point(131, 90);
			this.textBoxObjectCost.Name = "textBoxObjectCost";
			this.textBoxObjectCost.Size = new System.Drawing.Size(105, 21);
			this.textBoxObjectCost.TabIndex = 1;
			this.textBoxObjectCost.TextChanged += new System.EventHandler(this.textBox_TextChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(20, 69);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(107, 12);
			this.label1.TabIndex = 1;
			this.label1.Text = "(공시지가 X 1.5배)";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(20, 54);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(73, 12);
			this.label13.TabIndex = 1;
			this.label13.Text = "토지보상비 :";
			// 
			// textBoxLandCost
			// 
			this.textBoxLandCost.Location = new System.Drawing.Point(131, 51);
			this.textBoxLandCost.Name = "textBoxLandCost";
			this.textBoxLandCost.Size = new System.Drawing.Size(105, 21);
			this.textBoxLandCost.TabIndex = 0;
			this.textBoxLandCost.TextChanged += new System.EventHandler(this.textBox_TextChanged);
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(211, 212);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(53, 23);
			this.btnOK.TabIndex = 8;
			this.btnOK.Text = "확인";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(267, 212);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(53, 23);
			this.btnCancel.TabIndex = 8;
			this.btnCancel.Text = "취소";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// FormTotalCost
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(328, 248);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.labelAddrName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTotalCost";
			this.Text = "사업비 총괄";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.FormTotalCost_Load);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelAddrName;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label labelTotalCost;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxAroundCost;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxObjectCost;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBoxLandCost;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnMillionAroundCost;
        private System.Windows.Forms.Button btnMillionObjectCost;
        private System.Windows.Forms.Button btnMillionLandCost;
    }
}