namespace FireManagement
{
    partial class FormAddEquip
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
            this.radioRFID = new System.Windows.Forms.RadioButton();
            this.radioManual = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxRFID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboEquipType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxX = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxY = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxEquipID = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.checkBoxUseScren = new System.Windows.Forms.CheckBox();
            this.buttonAddNClear = new System.Windows.Forms.Button();
            this.buttonComplete = new System.Windows.Forms.Button();
            this.textBoxRFIDTagID = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxLocationName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "RFID";
            // 
            // radioRFID
            // 
            this.radioRFID.AutoSize = true;
            this.radioRFID.Location = new System.Drawing.Point(80, 54);
            this.radioRFID.Name = "radioRFID";
            this.radioRFID.Size = new System.Drawing.Size(93, 16);
            this.radioRFID.TabIndex = 1;
            this.radioRFID.TabStop = true;
            this.radioRFID.Text = "RFID Reader";
            this.radioRFID.UseVisualStyleBackColor = true;
            this.radioRFID.CheckedChanged += new System.EventHandler(this.radioRFID_CheckedChanged);
            // 
            // radioManual
            // 
            this.radioManual.AutoSize = true;
            this.radioManual.Location = new System.Drawing.Point(202, 54);
            this.radioManual.Name = "radioManual";
            this.radioManual.Size = new System.Drawing.Size(75, 16);
            this.radioManual.TabIndex = 2;
            this.radioManual.TabStop = true;
            this.radioManual.Text = "수동 입력";
            this.radioManual.UseVisualStyleBackColor = true;
            this.radioManual.CheckedChanged += new System.EventHandler(this.radioManual_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("굴림", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(14, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(172, 25);
            this.label2.TabIndex = 3;
            this.label2.Text = "소방설비 추가";
            // 
            // textBoxRFID
            // 
            this.textBoxRFID.BackColor = System.Drawing.Color.Gray;
            this.textBoxRFID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxRFID.Location = new System.Drawing.Point(80, 76);
            this.textBoxRFID.Name = "textBoxRFID";
            this.textBoxRFID.Size = new System.Drawing.Size(192, 21);
            this.textBoxRFID.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 138);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "설비종류";
            // 
            // comboEquipType
            // 
            this.comboEquipType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboEquipType.FormattingEnabled = true;
            this.comboEquipType.Items.AddRange(new object[] {
            "소화기",
            "소화전",
            "발신기"});
            this.comboEquipType.Location = new System.Drawing.Point(82, 133);
            this.comboEquipType.Name = "comboEquipType";
            this.comboEquipType.Size = new System.Drawing.Size(190, 20);
            this.comboEquipType.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 248);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "위치";
            // 
            // textBoxX
            // 
            this.textBoxX.Location = new System.Drawing.Point(102, 243);
            this.textBoxX.Name = "textBoxX";
            this.textBoxX.Size = new System.Drawing.Size(170, 21);
            this.textBoxX.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(83, 248);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(13, 12);
            this.label5.TabIndex = 5;
            this.label5.Text = "X";
            // 
            // textBoxY
            // 
            this.textBoxY.Location = new System.Drawing.Point(102, 270);
            this.textBoxY.Name = "textBoxY";
            this.textBoxY.Size = new System.Drawing.Size(170, 21);
            this.textBoxY.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(83, 275);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(13, 12);
            this.label6.TabIndex = 5;
            this.label6.Text = "Y";
            // 
            // textBoxEquipID
            // 
            this.textBoxEquipID.Location = new System.Drawing.Point(82, 172);
            this.textBoxEquipID.Name = "textBoxEquipID";
            this.textBoxEquipID.Size = new System.Drawing.Size(191, 21);
            this.textBoxEquipID.TabIndex = 6;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 177);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 5;
            this.label7.Text = "관리번호";
            // 
            // checkBoxUseScren
            // 
            this.checkBoxUseScren.AutoSize = true;
            this.checkBoxUseScren.Location = new System.Drawing.Point(103, 299);
            this.checkBoxUseScren.Name = "checkBoxUseScren";
            this.checkBoxUseScren.Size = new System.Drawing.Size(128, 16);
            this.checkBoxUseScren.TabIndex = 10;
            this.checkBoxUseScren.Text = "화면에서 위치 지정";
            this.checkBoxUseScren.UseVisualStyleBackColor = true;
            this.checkBoxUseScren.CheckedChanged += new System.EventHandler(this.checkBoxUseScren_CheckedChanged);
            // 
            // buttonAddNClear
            // 
            this.buttonAddNClear.Location = new System.Drawing.Point(90, 330);
            this.buttonAddNClear.Name = "buttonAddNClear";
            this.buttonAddNClear.Size = new System.Drawing.Size(81, 24);
            this.buttonAddNClear.TabIndex = 8;
            this.buttonAddNClear.Text = "적용";
            this.buttonAddNClear.UseVisualStyleBackColor = true;
            this.buttonAddNClear.Click += new System.EventHandler(this.buttonAddNClear_Click);
            // 
            // buttonComplete
            // 
            this.buttonComplete.Location = new System.Drawing.Point(191, 330);
            this.buttonComplete.Name = "buttonComplete";
            this.buttonComplete.Size = new System.Drawing.Size(81, 24);
            this.buttonComplete.TabIndex = 8;
            this.buttonComplete.Text = "완료";
            this.buttonComplete.UseVisualStyleBackColor = true;
            this.buttonComplete.Click += new System.EventHandler(this.buttonComplete_Click);
            // 
            // textBoxRFIDTagID
            // 
            this.textBoxRFIDTagID.Location = new System.Drawing.Point(81, 104);
            this.textBoxRFIDTagID.Name = "textBoxRFIDTagID";
            this.textBoxRFIDTagID.Size = new System.Drawing.Size(191, 21);
            this.textBoxRFIDTagID.TabIndex = 4;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 109);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 12);
            this.label8.TabIndex = 5;
            this.label8.Text = "Tag 이름";
            // 
            // textBoxLocationName
            // 
            this.textBoxLocationName.Location = new System.Drawing.Point(82, 216);
            this.textBoxLocationName.Name = "textBoxLocationName";
            this.textBoxLocationName.Size = new System.Drawing.Size(191, 21);
            this.textBoxLocationName.TabIndex = 7;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 221);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 12);
            this.label9.TabIndex = 5;
            this.label9.Text = "위치명";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 71);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(39, 12);
            this.label10.TabIndex = 9;
            this.label10.Text = "(필수)";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(18, 153);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(39, 12);
            this.label11.TabIndex = 9;
            this.label11.Text = "(필수)";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(18, 191);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(39, 12);
            this.label12.TabIndex = 9;
            this.label12.Text = "(필수)";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(16, 262);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(39, 12);
            this.label13.TabIndex = 9;
            this.label13.Text = "(필수)";
            // 
            // FormAddEquip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(296, 366);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.buttonComplete);
            this.Controls.Add(this.buttonAddNClear);
            this.Controls.Add(this.checkBoxUseScren);
            this.Controls.Add(this.comboEquipType);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxY);
            this.Controls.Add(this.textBoxRFIDTagID);
            this.Controls.Add(this.textBoxLocationName);
            this.Controls.Add(this.textBoxEquipID);
            this.Controls.Add(this.textBoxX);
            this.Controls.Add(this.textBoxRFID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.radioManual);
            this.Controls.Add(this.radioRFID);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "FormAddEquip";
            this.Text = "설비 추가하기";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormAddEquip_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioRFID;
        private System.Windows.Forms.RadioButton radioManual;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxRFID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboEquipType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxY;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxEquipID;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox checkBoxUseScren;
        private System.Windows.Forms.Button buttonAddNClear;
        private System.Windows.Forms.Button buttonComplete;
        private System.Windows.Forms.TextBox textBoxRFIDTagID;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxLocationName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
    }
}