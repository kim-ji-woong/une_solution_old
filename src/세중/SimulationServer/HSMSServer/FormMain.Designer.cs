namespace HSMSServer
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
            this.textBoxDeviceID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxX = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxY = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxLatitude = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxLongitude = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxMethan = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxCO = new System.Windows.Forms.TextBox();
            this.btnRandom = new System.Windows.Forms.Button();
            this.btnSimulation = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.radioWorker = new System.Windows.Forms.RadioButton();
            this.radioVehicle = new System.Windows.Forms.RadioButton();
            this.radioEquip = new System.Windows.Forms.RadioButton();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Device ID";
            // 
            // textBoxDeviceID
            // 
            this.textBoxDeviceID.Location = new System.Drawing.Point(169, 6);
            this.textBoxDeviceID.Name = "textBoxDeviceID";
            this.textBoxDeviceID.Size = new System.Drawing.Size(100, 21);
            this.textBoxDeviceID.TabIndex = 1;
            this.textBoxDeviceID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "X";
            // 
            // textBoxX
            // 
            this.textBoxX.Location = new System.Drawing.Point(169, 33);
            this.textBoxX.Name = "textBoxX";
            this.textBoxX.Size = new System.Drawing.Size(100, 21);
            this.textBoxX.TabIndex = 1;
            this.textBoxX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(13, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Y";
            // 
            // textBoxY
            // 
            this.textBoxY.Location = new System.Drawing.Point(169, 60);
            this.textBoxY.Name = "textBoxY";
            this.textBoxY.Size = new System.Drawing.Size(100, 21);
            this.textBoxY.TabIndex = 1;
            this.textBoxY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "위도(°)";
            // 
            // textBoxLatitude
            // 
            this.textBoxLatitude.Location = new System.Drawing.Point(169, 87);
            this.textBoxLatitude.Name = "textBoxLatitude";
            this.textBoxLatitude.Size = new System.Drawing.Size(100, 21);
            this.textBoxLatitude.TabIndex = 1;
            this.textBoxLatitude.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 117);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "경도(°)";
            // 
            // textBoxLongitude
            // 
            this.textBoxLongitude.Location = new System.Drawing.Point(169, 114);
            this.textBoxLongitude.Name = "textBoxLongitude";
            this.textBoxLongitude.Size = new System.Drawing.Size(100, 21);
            this.textBoxLongitude.TabIndex = 1;
            this.textBoxLongitude.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 144);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "메탄가스(ppm)";
            // 
            // textBoxMethan
            // 
            this.textBoxMethan.Location = new System.Drawing.Point(169, 141);
            this.textBoxMethan.Name = "textBoxMethan";
            this.textBoxMethan.Size = new System.Drawing.Size(100, 21);
            this.textBoxMethan.TabIndex = 1;
            this.textBoxMethan.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 171);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "일산화가스(ppm)";
            // 
            // textBoxCO
            // 
            this.textBoxCO.Location = new System.Drawing.Point(169, 168);
            this.textBoxCO.Name = "textBoxCO";
            this.textBoxCO.Size = new System.Drawing.Size(100, 21);
            this.textBoxCO.TabIndex = 1;
            this.textBoxCO.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnRandom
            // 
            this.btnRandom.Location = new System.Drawing.Point(194, 207);
            this.btnRandom.Name = "btnRandom";
            this.btnRandom.Size = new System.Drawing.Size(75, 23);
            this.btnRandom.TabIndex = 2;
            this.btnRandom.Text = "Random";
            this.btnRandom.UseVisualStyleBackColor = true;
            this.btnRandom.Click += new System.EventHandler(this.btnRandom_Click);
            // 
            // btnSimulation
            // 
            this.btnSimulation.Location = new System.Drawing.Point(102, 207);
            this.btnSimulation.Name = "btnSimulation";
            this.btnSimulation.Size = new System.Drawing.Size(75, 23);
            this.btnSimulation.TabIndex = 2;
            this.btnSimulation.Text = "Simulation";
            this.btnSimulation.UseVisualStyleBackColor = true;
            this.btnSimulation.Click += new System.EventHandler(this.btnSimulation_Click);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(11, 207);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // radioWorker
            // 
            this.radioWorker.AutoSize = true;
            this.radioWorker.Checked = true;
            this.radioWorker.Location = new System.Drawing.Point(14, 236);
            this.radioWorker.Name = "radioWorker";
            this.radioWorker.Size = new System.Drawing.Size(59, 16);
            this.radioWorker.TabIndex = 3;
            this.radioWorker.TabStop = true;
            this.radioWorker.Text = "작업자";
            this.radioWorker.UseVisualStyleBackColor = true;
            // 
            // radioVehicle
            // 
            this.radioVehicle.AutoSize = true;
            this.radioVehicle.Enabled = false;
            this.radioVehicle.Location = new System.Drawing.Point(102, 236);
            this.radioVehicle.Name = "radioVehicle";
            this.radioVehicle.Size = new System.Drawing.Size(47, 16);
            this.radioVehicle.TabIndex = 3;
            this.radioVehicle.Text = "차량";
            this.radioVehicle.UseVisualStyleBackColor = true;
            // 
            // radioEquip
            // 
            this.radioEquip.AutoSize = true;
            this.radioEquip.Enabled = false;
            this.radioEquip.Location = new System.Drawing.Point(194, 236);
            this.radioEquip.Name = "radioEquip";
            this.radioEquip.Size = new System.Drawing.Size(47, 16);
            this.radioEquip.TabIndex = 3;
            this.radioEquip.Text = "설비";
            this.radioEquip.UseVisualStyleBackColor = true;
            // 
            // textBoxLog
            // 
            this.textBoxLog.BackColor = System.Drawing.Color.White;
            this.textBoxLog.Location = new System.Drawing.Point(14, 275);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(255, 214);
            this.textBoxLog.TabIndex = 4;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 501);
            this.Controls.Add(this.textBoxLog);
            this.Controls.Add(this.radioEquip);
            this.Controls.Add(this.radioVehicle);
            this.Controls.Add(this.radioWorker);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnSimulation);
            this.Controls.Add(this.btnRandom);
            this.Controls.Add(this.textBoxCO);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxMethan);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxLongitude);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxLatitude);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxY);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxX);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxDeviceID);
            this.Controls.Add(this.label1);
            this.Name = "FormMain";
            this.Text = "Simulation 서버";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxDeviceID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxX;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxY;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxLatitude;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxLongitude;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxMethan;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxCO;
        private System.Windows.Forms.Button btnRandom;
        private System.Windows.Forms.Button btnSimulation;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.RadioButton radioWorker;
        private System.Windows.Forms.RadioButton radioVehicle;
        private System.Windows.Forms.RadioButton radioEquip;
        private System.Windows.Forms.TextBox textBoxLog;
    }
}

