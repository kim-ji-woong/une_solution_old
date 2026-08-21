namespace SimulationScenarioMaker
{
    partial class FormMain
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxRunningMinute = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxRunningSecond = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxEventMinute = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxEventSecond = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxPeriodSecond = new System.Windows.Forms.TextBox();
            this.radioWorker = new System.Windows.Forms.RadioButton();
            this.radioVehicle = new System.Windows.Forms.RadioButton();
            this.radioEquipment = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.cboSensorIDs = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxX = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxY = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.textBoxCoords = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.cboRepeat = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Running Time :";
            // 
            // textBoxRunningMinute
            // 
            this.textBoxRunningMinute.Location = new System.Drawing.Point(116, 22);
            this.textBoxRunningMinute.Name = "textBoxRunningMinute";
            this.textBoxRunningMinute.Size = new System.Drawing.Size(36, 21);
            this.textBoxRunningMinute.TabIndex = 1;
            this.textBoxRunningMinute.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(154, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "분";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(215, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "초";
            // 
            // textBoxRunningSecond
            // 
            this.textBoxRunningSecond.Location = new System.Drawing.Point(177, 22);
            this.textBoxRunningSecond.Name = "textBoxRunningSecond";
            this.textBoxRunningSecond.Size = new System.Drawing.Size(36, 21);
            this.textBoxRunningSecond.TabIndex = 1;
            this.textBoxRunningSecond.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "이벤트 시간 :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(154, 80);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "분";
            // 
            // textBoxEventMinute
            // 
            this.textBoxEventMinute.Location = new System.Drawing.Point(116, 77);
            this.textBoxEventMinute.Name = "textBoxEventMinute";
            this.textBoxEventMinute.Size = new System.Drawing.Size(36, 21);
            this.textBoxEventMinute.TabIndex = 1;
            this.textBoxEventMinute.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(215, 80);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "초";
            // 
            // textBoxEventSecond
            // 
            this.textBoxEventSecond.Location = new System.Drawing.Point(177, 77);
            this.textBoxEventSecond.Name = "textBoxEventSecond";
            this.textBoxEventSecond.Size = new System.Drawing.Size(36, 21);
            this.textBoxEventSecond.TabIndex = 1;
            this.textBoxEventSecond.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 110);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "주기 :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(215, 110);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "초";
            // 
            // textBoxPeriodSecond
            // 
            this.textBoxPeriodSecond.Location = new System.Drawing.Point(177, 107);
            this.textBoxPeriodSecond.Name = "textBoxPeriodSecond";
            this.textBoxPeriodSecond.Size = new System.Drawing.Size(36, 21);
            this.textBoxPeriodSecond.TabIndex = 1;
            this.textBoxPeriodSecond.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // radioWorker
            // 
            this.radioWorker.AutoSize = true;
            this.radioWorker.Checked = true;
            this.radioWorker.Location = new System.Drawing.Point(296, 21);
            this.radioWorker.Name = "radioWorker";
            this.radioWorker.Size = new System.Drawing.Size(59, 16);
            this.radioWorker.TabIndex = 2;
            this.radioWorker.TabStop = true;
            this.radioWorker.Text = "작업자";
            this.radioWorker.UseVisualStyleBackColor = true;
            this.radioWorker.CheckedChanged += new System.EventHandler(this.radioSensorType_CheckedChanged);
            // 
            // radioVehicle
            // 
            this.radioVehicle.AutoSize = true;
            this.radioVehicle.Location = new System.Drawing.Point(361, 21);
            this.radioVehicle.Name = "radioVehicle";
            this.radioVehicle.Size = new System.Drawing.Size(47, 16);
            this.radioVehicle.TabIndex = 2;
            this.radioVehicle.Text = "차량";
            this.radioVehicle.UseVisualStyleBackColor = true;
            this.radioVehicle.CheckedChanged += new System.EventHandler(this.radioSensorType_CheckedChanged);
            // 
            // radioEquipment
            // 
            this.radioEquipment.AutoSize = true;
            this.radioEquipment.Location = new System.Drawing.Point(426, 21);
            this.radioEquipment.Name = "radioEquipment";
            this.radioEquipment.Size = new System.Drawing.Size(47, 16);
            this.radioEquipment.TabIndex = 2;
            this.radioEquipment.Text = "설비";
            this.radioEquipment.UseVisualStyleBackColor = true;
            this.radioEquipment.CheckedChanged += new System.EventHandler(this.radioSensorType_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(294, 56);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(68, 12);
            this.label9.TabIndex = 3;
            this.label9.Text = "Sensor ID :";
            // 
            // cboSensorIDs
            // 
            this.cboSensorIDs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSensorIDs.FormattingEnabled = true;
            this.cboSensorIDs.Location = new System.Drawing.Point(368, 53);
            this.cboSensorIDs.Name = "cboSensorIDs";
            this.cboSensorIDs.Size = new System.Drawing.Size(121, 20);
            this.cboSensorIDs.Sorted = true;
            this.cboSensorIDs.TabIndex = 4;
            this.cboSensorIDs.SelectedIndexChanged += new System.EventHandler(this.cboSensorIDs_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 153);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(13, 12);
            this.label10.TabIndex = 5;
            this.label10.Text = "X";
            // 
            // textBoxX
            // 
            this.textBoxX.Location = new System.Drawing.Point(31, 150);
            this.textBoxX.Name = "textBoxX";
            this.textBoxX.Size = new System.Drawing.Size(64, 21);
            this.textBoxX.TabIndex = 6;
            this.textBoxX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(109, 153);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(13, 12);
            this.label11.TabIndex = 5;
            this.label11.Text = "Y";
            // 
            // textBoxY
            // 
            this.textBoxY.Location = new System.Drawing.Point(128, 150);
            this.textBoxY.Name = "textBoxY";
            this.textBoxY.Size = new System.Drawing.Size(64, 21);
            this.textBoxY.TabIndex = 6;
            this.textBoxY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(296, 124);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 7;
            this.btnAdd.Text = "좌표 추가";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // textBoxCoords
            // 
            this.textBoxCoords.Location = new System.Drawing.Point(12, 181);
            this.textBoxCoords.Multiline = true;
            this.textBoxCoords.Name = "textBoxCoords";
            this.textBoxCoords.Size = new System.Drawing.Size(477, 259);
            this.textBoxCoords.TabIndex = 8;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(296, 86);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "저장";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 53);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(61, 12);
            this.label12.TabIndex = 0;
            this.label12.Text = "반복회수 :";
            // 
            // cboRepeat
            // 
            this.cboRepeat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRepeat.FormattingEnabled = true;
            this.cboRepeat.Items.AddRange(new object[] {
            "무한 반복",
            "반복 없음",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.cboRepeat.Location = new System.Drawing.Point(116, 50);
            this.cboRepeat.Name = "cboRepeat";
            this.cboRepeat.Size = new System.Drawing.Size(97, 20);
            this.cboRepeat.TabIndex = 10;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(502, 452);
            this.Controls.Add(this.cboRepeat);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.textBoxCoords);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.textBoxY);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.textBoxX);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.cboSensorIDs);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.radioEquipment);
            this.Controls.Add(this.radioVehicle);
            this.Controls.Add(this.radioWorker);
            this.Controls.Add(this.textBoxPeriodSecond);
            this.Controls.Add(this.textBoxEventSecond);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBoxRunningSecond);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxEventMinute);
            this.Controls.Add(this.textBoxRunningMinute);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label1);
            this.Name = "FormMain";
            this.Text = "시나리오 생성기";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxRunningMinute;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxRunningSecond;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxEventMinute;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxEventSecond;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxPeriodSecond;
        private System.Windows.Forms.RadioButton radioWorker;
        private System.Windows.Forms.RadioButton radioVehicle;
        private System.Windows.Forms.RadioButton radioEquipment;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cboSensorIDs;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxX;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxY;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox textBoxCoords;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cboRepeat;
    }
}

