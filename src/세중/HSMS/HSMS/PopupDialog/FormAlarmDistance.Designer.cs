namespace HSMS
{
    partial class FormAlarmDistance
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cmbWorkerToEquipDistance = new System.Windows.Forms.ComboBox();
            this.cmbWorkerToZoneDistance = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxWorkerToEquipDistance = new System.Windows.Forms.TextBox();
            this.textBoxWorkerToZoneDistance = new System.Windows.Forms.TextBox();
            this.textBoxWorkerToCarDistanceOneSide = new System.Windows.Forms.TextBox();
            this.textBoxWorkerToCarDistanceBoth = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxCoGas = new System.Windows.Forms.TextBox();
            this.textBoxMethaneGas = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(523, 47);
            this.panel1.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(19, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 21);
            this.label3.TabIndex = 1;
            this.label3.Text = "알람거리";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.cmbWorkerToEquipDistance);
            this.panel2.Controls.Add(this.cmbWorkerToZoneDistance);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.label13);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.label12);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.textBoxWorkerToEquipDistance);
            this.panel2.Controls.Add(this.textBoxWorkerToZoneDistance);
            this.panel2.Controls.Add(this.textBoxMethaneGas);
            this.panel2.Controls.Add(this.textBoxWorkerToCarDistanceOneSide);
            this.panel2.Controls.Add(this.textBoxCoGas);
            this.panel2.Controls.Add(this.textBoxWorkerToCarDistanceBoth);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(12, 70);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(523, 320);
            this.panel2.TabIndex = 4;
            // 
            // cmbWorkerToEquipDistance
            // 
            this.cmbWorkerToEquipDistance.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWorkerToEquipDistance.FormattingEnabled = true;
            this.cmbWorkerToEquipDistance.Location = new System.Drawing.Point(183, 203);
            this.cmbWorkerToEquipDistance.Name = "cmbWorkerToEquipDistance";
            this.cmbWorkerToEquipDistance.Size = new System.Drawing.Size(149, 20);
            this.cmbWorkerToEquipDistance.TabIndex = 9;
            this.cmbWorkerToEquipDistance.SelectedIndexChanged += new System.EventHandler(this.cmbWorkerToEquipDistance_SelectedIndexChanged);
            // 
            // cmbWorkerToZoneDistance
            // 
            this.cmbWorkerToZoneDistance.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWorkerToZoneDistance.FormattingEnabled = true;
            this.cmbWorkerToZoneDistance.Location = new System.Drawing.Point(183, 140);
            this.cmbWorkerToZoneDistance.Name = "cmbWorkerToZoneDistance";
            this.cmbWorkerToZoneDistance.Size = new System.Drawing.Size(149, 20);
            this.cmbWorkerToZoneDistance.TabIndex = 9;
            this.cmbWorkerToZoneDistance.SelectedIndexChanged += new System.EventHandler(this.cmbWorkerToZoneDistance_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label9.Location = new System.Drawing.Point(483, 208);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(18, 15);
            this.label9.TabIndex = 8;
            this.label9.Text = "m";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.Location = new System.Drawing.Point(483, 145);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(18, 15);
            this.label8.TabIndex = 8;
            this.label8.Text = "m";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.Location = new System.Drawing.Point(483, 79);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(18, 15);
            this.label7.TabIndex = 8;
            this.label7.Text = "m";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.Location = new System.Drawing.Point(483, 46);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(18, 15);
            this.label6.TabIndex = 8;
            this.label6.Text = "m";
            // 
            // textBoxWorkerToEquipDistance
            // 
            this.textBoxWorkerToEquipDistance.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxWorkerToEquipDistance.Location = new System.Drawing.Point(361, 203);
            this.textBoxWorkerToEquipDistance.Name = "textBoxWorkerToEquipDistance";
            this.textBoxWorkerToEquipDistance.Size = new System.Drawing.Size(121, 23);
            this.textBoxWorkerToEquipDistance.TabIndex = 7;
            this.textBoxWorkerToEquipDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxWorkerToEquipDistance.TextChanged += new System.EventHandler(this.textBoxWorkerToEquipDistance_TextChanged);
            // 
            // textBoxWorkerToZoneDistance
            // 
            this.textBoxWorkerToZoneDistance.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxWorkerToZoneDistance.Location = new System.Drawing.Point(361, 140);
            this.textBoxWorkerToZoneDistance.Name = "textBoxWorkerToZoneDistance";
            this.textBoxWorkerToZoneDistance.Size = new System.Drawing.Size(121, 23);
            this.textBoxWorkerToZoneDistance.TabIndex = 7;
            this.textBoxWorkerToZoneDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxWorkerToZoneDistance.TextChanged += new System.EventHandler(this.textBoxWorkerToZoneDistance_TextChanged);
            // 
            // textBoxWorkerToCarDistanceOneSide
            // 
            this.textBoxWorkerToCarDistanceOneSide.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxWorkerToCarDistanceOneSide.Location = new System.Drawing.Point(361, 74);
            this.textBoxWorkerToCarDistanceOneSide.Name = "textBoxWorkerToCarDistanceOneSide";
            this.textBoxWorkerToCarDistanceOneSide.Size = new System.Drawing.Size(121, 23);
            this.textBoxWorkerToCarDistanceOneSide.TabIndex = 6;
            this.textBoxWorkerToCarDistanceOneSide.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxWorkerToCarDistanceBoth
            // 
            this.textBoxWorkerToCarDistanceBoth.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxWorkerToCarDistanceBoth.Location = new System.Drawing.Point(361, 41);
            this.textBoxWorkerToCarDistanceBoth.Name = "textBoxWorkerToCarDistanceBoth";
            this.textBoxWorkerToCarDistanceBoth.Size = new System.Drawing.Size(121, 23);
            this.textBoxWorkerToCarDistanceBoth.TabIndex = 5;
            this.textBoxWorkerToCarDistanceBoth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.Location = new System.Drawing.Point(23, 172);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(187, 17);
            this.label5.TabIndex = 1;
            this.label5.Text = "작업자와 위험설비간 안전거리";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(23, 109);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(187, 17);
            this.label4.TabIndex = 1;
            this.label4.Text = "작업자와 위험영역간 안전거리";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(23, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(259, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "작업자와 차량간 안전거리 - 한쪽에서 접근";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(23, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(233, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "작업자와 차량간 안전거리 - 상호 접근";
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.White;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.Location = new System.Drawing.Point(319, 402);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(105, 30);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "저장";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.Location = new System.Drawing.Point(430, 402);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(105, 30);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label10.Location = new System.Drawing.Point(23, 242);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(130, 17);
            this.label10.TabIndex = 1;
            this.label10.Text = "일산화탄소 안전농도";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label11.Location = new System.Drawing.Point(23, 275);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(117, 17);
            this.label11.TabIndex = 1;
            this.label11.Text = "메탄가스 안전농도";
            // 
            // textBoxCoGas
            // 
            this.textBoxCoGas.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxCoGas.Location = new System.Drawing.Point(361, 240);
            this.textBoxCoGas.Name = "textBoxCoGas";
            this.textBoxCoGas.Size = new System.Drawing.Size(121, 23);
            this.textBoxCoGas.TabIndex = 5;
            this.textBoxCoGas.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxMethaneGas
            // 
            this.textBoxMethaneGas.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxMethaneGas.Location = new System.Drawing.Point(361, 273);
            this.textBoxMethaneGas.Name = "textBoxMethaneGas";
            this.textBoxMethaneGas.Size = new System.Drawing.Size(121, 23);
            this.textBoxMethaneGas.TabIndex = 6;
            this.textBoxMethaneGas.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label12.Location = new System.Drawing.Point(483, 245);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(32, 15);
            this.label12.TabIndex = 8;
            this.label12.Text = "ppm";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label13.Location = new System.Drawing.Point(483, 278);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(32, 15);
            this.label13.TabIndex = 8;
            this.label13.Text = "ppm";
            // 
            // FormAlarmDistance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(550, 455);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormAlarmDistance";
            this.Text = "FormAlarmDistance";
            this.Shown += new System.EventHandler(this.FormAlarmDistance_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBoxWorkerToZoneDistance;
        private System.Windows.Forms.TextBox textBoxWorkerToCarDistanceOneSide;
        private System.Windows.Forms.TextBox textBoxWorkerToCarDistanceBoth;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxWorkerToEquipDistance;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmbWorkerToZoneDistance;
        private System.Windows.Forms.ComboBox cmbWorkerToEquipDistance;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxMethaneGas;
        private System.Windows.Forms.TextBox textBoxCoGas;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
    }
}