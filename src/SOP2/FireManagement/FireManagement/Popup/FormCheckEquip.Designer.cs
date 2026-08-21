namespace FireManagement
{
    partial class FormCheckEquip2
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
            this.label13 = new System.Windows.Forms.Label();
            this.buttonComplete = new System.Windows.Forms.Button();
            this.buttonApplyNClear = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxOpinion = new System.Windows.Forms.TextBox();
            this.textBoxRFIDTagID = new System.Windows.Forms.TextBox();
            this.textBoxEquipID = new System.Windows.Forms.TextBox();
            this.textBoxEquipType = new System.Windows.Forms.TextBox();
            this.textBoxRFID = new System.Windows.Forms.TextBox();
            this.radioManual = new System.Windows.Forms.RadioButton();
            this.radioRFID = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxStatus = new System.Windows.Forms.ComboBox();
            this.textBoxLastCheckedTime = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(13, 209);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(53, 12);
            this.label13.TabIndex = 32;
            this.label13.Text = "점검의견";
            // 
            // buttonComplete
            // 
            this.buttonComplete.Location = new System.Drawing.Point(190, 282);
            this.buttonComplete.Name = "buttonComplete";
            this.buttonComplete.Size = new System.Drawing.Size(81, 24);
            this.buttonComplete.TabIndex = 28;
            this.buttonComplete.Text = "완료";
            this.buttonComplete.UseVisualStyleBackColor = true;
            this.buttonComplete.Click += new System.EventHandler(this.buttonComplete_Click);
            // 
            // buttonApplyNClear
            // 
            this.buttonApplyNClear.Location = new System.Drawing.Point(89, 282);
            this.buttonApplyNClear.Name = "buttonApplyNClear";
            this.buttonApplyNClear.Size = new System.Drawing.Size(81, 24);
            this.buttonApplyNClear.TabIndex = 29;
            this.buttonApplyNClear.Text = "적용";
            this.buttonApplyNClear.UseVisualStyleBackColor = true;
            this.buttonApplyNClear.Click += new System.EventHandler(this.buttonApplyNClear_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 61);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 12);
            this.label8.TabIndex = 21;
            this.label8.Text = "Tag 이름";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 117);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 17;
            this.label7.Text = "관리번호";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 143);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 20;
            this.label4.Text = "설비상태";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 19;
            this.label3.Text = "설비종류";
            // 
            // textBoxOpinion
            // 
            this.textBoxOpinion.Location = new System.Drawing.Point(79, 204);
            this.textBoxOpinion.Multiline = true;
            this.textBoxOpinion.Name = "textBoxOpinion";
            this.textBoxOpinion.Size = new System.Drawing.Size(190, 72);
            this.textBoxOpinion.TabIndex = 34;
            // 
            // textBoxRFIDTagID
            // 
            this.textBoxRFIDTagID.Location = new System.Drawing.Point(78, 56);
            this.textBoxRFIDTagID.Name = "textBoxRFIDTagID";
            this.textBoxRFIDTagID.ReadOnly = true;
            this.textBoxRFIDTagID.Size = new System.Drawing.Size(191, 21);
            this.textBoxRFIDTagID.TabIndex = 16;
            // 
            // textBoxEquipID
            // 
            this.textBoxEquipID.Location = new System.Drawing.Point(79, 113);
            this.textBoxEquipID.Name = "textBoxEquipID";
            this.textBoxEquipID.ReadOnly = true;
            this.textBoxEquipID.Size = new System.Drawing.Size(191, 21);
            this.textBoxEquipID.TabIndex = 26;
            // 
            // textBoxEquipType
            // 
            this.textBoxEquipType.Location = new System.Drawing.Point(79, 85);
            this.textBoxEquipType.Name = "textBoxEquipType";
            this.textBoxEquipType.ReadOnly = true;
            this.textBoxEquipType.Size = new System.Drawing.Size(191, 21);
            this.textBoxEquipType.TabIndex = 25;
            // 
            // textBoxRFID
            // 
            this.textBoxRFID.Location = new System.Drawing.Point(77, 28);
            this.textBoxRFID.Name = "textBoxRFID";
            this.textBoxRFID.ReadOnly = true;
            this.textBoxRFID.Size = new System.Drawing.Size(192, 21);
            this.textBoxRFID.TabIndex = 14;
            // 
            // radioManual
            // 
            this.radioManual.AutoSize = true;
            this.radioManual.Location = new System.Drawing.Point(199, 6);
            this.radioManual.Name = "radioManual";
            this.radioManual.Size = new System.Drawing.Size(75, 16);
            this.radioManual.TabIndex = 13;
            this.radioManual.TabStop = true;
            this.radioManual.Text = "화면 선택";
            this.radioManual.UseVisualStyleBackColor = true;
            this.radioManual.Visible = false;
            this.radioManual.CheckedChanged += new System.EventHandler(this.radioManual_CheckedChanged);
            // 
            // radioRFID
            // 
            this.radioRFID.AutoSize = true;
            this.radioRFID.Location = new System.Drawing.Point(77, 6);
            this.radioRFID.Name = "radioRFID";
            this.radioRFID.Size = new System.Drawing.Size(93, 16);
            this.radioRFID.TabIndex = 12;
            this.radioRFID.TabStop = true;
            this.radioRFID.Text = "RFID Reader";
            this.radioRFID.UseVisualStyleBackColor = true;
            this.radioRFID.Visible = false;
            this.radioRFID.CheckedChanged += new System.EventHandler(this.radioRFID_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 11;
            this.label1.Text = "설비선택";
            this.label1.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 12);
            this.label2.TabIndex = 11;
            this.label2.Text = "RFID";
            // 
            // comboBoxStatus
            // 
            this.comboBoxStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStatus.FormattingEnabled = true;
            this.comboBoxStatus.Items.AddRange(new object[] {
            "양호",
            "불량/고장",
            "수리중",
            "기타"});
            this.comboBoxStatus.Location = new System.Drawing.Point(79, 140);
            this.comboBoxStatus.Name = "comboBoxStatus";
            this.comboBoxStatus.Size = new System.Drawing.Size(190, 20);
            this.comboBoxStatus.TabIndex = 36;
            // 
            // textBoxLastCheckedTime
            // 
            this.textBoxLastCheckedTime.Location = new System.Drawing.Point(78, 171);
            this.textBoxLastCheckedTime.Name = "textBoxLastCheckedTime";
            this.textBoxLastCheckedTime.ReadOnly = true;
            this.textBoxLastCheckedTime.Size = new System.Drawing.Size(191, 21);
            this.textBoxLastCheckedTime.TabIndex = 26;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(13, 168);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 29);
            this.label5.TabIndex = 17;
            this.label5.Text = "마지막 점검시간";
            // 
            // FormCheckEquip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 314);
            this.Controls.Add(this.comboBoxStatus);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.buttonComplete);
            this.Controls.Add(this.buttonApplyNClear);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxOpinion);
            this.Controls.Add(this.textBoxRFIDTagID);
            this.Controls.Add(this.textBoxLastCheckedTime);
            this.Controls.Add(this.textBoxEquipID);
            this.Controls.Add(this.textBoxEquipType);
            this.Controls.Add(this.textBoxRFID);
            this.Controls.Add(this.radioManual);
            this.Controls.Add(this.radioRFID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "FormCheckEquip";
            this.Text = "설비 점검";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormCheckEquip_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button buttonComplete;
        private System.Windows.Forms.Button buttonApplyNClear;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxOpinion;
        private System.Windows.Forms.TextBox textBoxRFIDTagID;
        private System.Windows.Forms.TextBox textBoxEquipID;
        private System.Windows.Forms.TextBox textBoxEquipType;
        private System.Windows.Forms.TextBox textBoxRFID;
        private System.Windows.Forms.RadioButton radioManual;
        private System.Windows.Forms.RadioButton radioRFID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxStatus;
        private System.Windows.Forms.TextBox textBoxLastCheckedTime;
        private System.Windows.Forms.Label label5;
    }
}