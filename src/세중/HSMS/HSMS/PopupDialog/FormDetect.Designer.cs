namespace HSMS
{
    partial class FormDetect
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
            this.panelSpace1 = new System.Windows.Forms.Panel();
            this.panelSpace2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.cmbUseWorker = new System.Windows.Forms.ComboBox();
            this.cmbUseEquip = new System.Windows.Forms.ComboBox();
            this.cmbUseVehicle = new System.Windows.Forms.ComboBox();
            this.cmbEquips = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbVehicles = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbWorkers = new System.Windows.Forms.ComboBox();
            this.btnForSensors = new System.Windows.Forms.Button();
            this.btnForWorkers = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(599, 47);
            this.panel1.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(19, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 21);
            this.label3.TabIndex = 1;
            this.label3.Text = "탐지 관리";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.panelSpace1);
            this.panel2.Controls.Add(this.panelSpace2);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.btnForSensors);
            this.panel2.Controls.Add(this.btnForWorkers);
            this.panel2.Location = new System.Drawing.Point(12, 74);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(599, 405);
            this.panel2.TabIndex = 4;
            // 
            // panelSpace1
            // 
            this.panelSpace1.BackColor = System.Drawing.SystemColors.Control;
            this.panelSpace1.Location = new System.Drawing.Point(143, 29);
            this.panelSpace1.Name = "panelSpace1";
            this.panelSpace1.Size = new System.Drawing.Size(15, 34);
            this.panelSpace1.TabIndex = 10;
            // 
            // panelSpace2
            // 
            this.panelSpace2.BackColor = System.Drawing.SystemColors.Control;
            this.panelSpace2.Location = new System.Drawing.Point(143, 72);
            this.panelSpace2.Name = "panelSpace2";
            this.panelSpace2.Size = new System.Drawing.Size(15, 32);
            this.panelSpace2.TabIndex = 9;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.Control;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.cmbUseWorker);
            this.panel3.Controls.Add(this.cmbUseEquip);
            this.panel3.Controls.Add(this.cmbUseVehicle);
            this.panel3.Controls.Add(this.cmbEquips);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.cmbVehicles);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.cmbWorkers);
            this.panel3.Location = new System.Drawing.Point(143, 18);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(441, 370);
            this.panel3.TabIndex = 8;
            // 
            // cmbUseWorker
            // 
            this.cmbUseWorker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUseWorker.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbUseWorker.FormattingEnabled = true;
            this.cmbUseWorker.Items.AddRange(new object[] {
            "센서신호 사용",
            "사용안함"});
            this.cmbUseWorker.Location = new System.Drawing.Point(301, 37);
            this.cmbUseWorker.Name = "cmbUseWorker";
            this.cmbUseWorker.Size = new System.Drawing.Size(108, 23);
            this.cmbUseWorker.TabIndex = 8;
            this.cmbUseWorker.SelectedIndexChanged += new System.EventHandler(this.cmbUseWorker_SelectedIndexChanged);
            // 
            // cmbUseEquip
            // 
            this.cmbUseEquip.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUseEquip.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbUseEquip.FormattingEnabled = true;
            this.cmbUseEquip.Items.AddRange(new object[] {
            "센서신호 사용",
            "사용안함"});
            this.cmbUseEquip.Location = new System.Drawing.Point(301, 121);
            this.cmbUseEquip.Name = "cmbUseEquip";
            this.cmbUseEquip.Size = new System.Drawing.Size(108, 23);
            this.cmbUseEquip.TabIndex = 7;
            this.cmbUseEquip.SelectedIndexChanged += new System.EventHandler(this.cmbUseEquip_SelectedIndexChanged);
            // 
            // cmbUseVehicle
            // 
            this.cmbUseVehicle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUseVehicle.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbUseVehicle.FormattingEnabled = true;
            this.cmbUseVehicle.Items.AddRange(new object[] {
            "센서신호 사용",
            "사용안함"});
            this.cmbUseVehicle.Location = new System.Drawing.Point(301, 79);
            this.cmbUseVehicle.Name = "cmbUseVehicle";
            this.cmbUseVehicle.Size = new System.Drawing.Size(108, 23);
            this.cmbUseVehicle.TabIndex = 6;
            this.cmbUseVehicle.SelectedIndexChanged += new System.EventHandler(this.cmbUseVehicle_SelectedIndexChanged);
            // 
            // cmbEquips
            // 
            this.cmbEquips.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEquips.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbEquips.FormattingEnabled = true;
            this.cmbEquips.Location = new System.Drawing.Point(97, 121);
            this.cmbEquips.Name = "cmbEquips";
            this.cmbEquips.Size = new System.Drawing.Size(154, 23);
            this.cmbEquips.TabIndex = 5;
            this.cmbEquips.SelectedIndexChanged += new System.EventHandler(this.cmbEquips_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(31, 121);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "위험 설비";
            // 
            // cmbVehicles
            // 
            this.cmbVehicles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVehicles.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbVehicles.FormattingEnabled = true;
            this.cmbVehicles.Location = new System.Drawing.Point(97, 79);
            this.cmbVehicles.Name = "cmbVehicles";
            this.cmbVehicles.Size = new System.Drawing.Size(153, 23);
            this.cmbVehicles.TabIndex = 3;
            this.cmbVehicles.SelectedIndexChanged += new System.EventHandler(this.cmbVehicles_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(31, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "차량";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(31, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "작업자";
            // 
            // cmbWorkers
            // 
            this.cmbWorkers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWorkers.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbWorkers.FormattingEnabled = true;
            this.cmbWorkers.Location = new System.Drawing.Point(97, 37);
            this.cmbWorkers.Name = "cmbWorkers";
            this.cmbWorkers.Size = new System.Drawing.Size(154, 23);
            this.cmbWorkers.TabIndex = 0;
            this.cmbWorkers.SelectedIndexChanged += new System.EventHandler(this.cmbWorkers_SelectedIndexChanged);
            // 
            // btnForSensors
            // 
            this.btnForSensors.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnForSensors.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnForSensors.Location = new System.Drawing.Point(12, 70);
            this.btnForSensors.Name = "btnForSensors";
            this.btnForSensors.Size = new System.Drawing.Size(143, 36);
            this.btnForSensors.TabIndex = 10;
            this.btnForSensors.Text = "항목별 설정";
            this.btnForSensors.UseVisualStyleBackColor = true;
            this.btnForSensors.Click += new System.EventHandler(this.btnForSensors_Click);
            // 
            // btnForWorkers
            // 
            this.btnForWorkers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnForWorkers.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnForWorkers.Location = new System.Drawing.Point(12, 28);
            this.btnForWorkers.Name = "btnForWorkers";
            this.btnForWorkers.Size = new System.Drawing.Size(143, 36);
            this.btnForWorkers.TabIndex = 9;
            this.btnForWorkers.Text = "작업자별 설정";
            this.btnForWorkers.UseVisualStyleBackColor = true;
            this.btnForWorkers.Click += new System.EventHandler(this.btnForWorkers_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.White;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button1.Location = new System.Drawing.Point(399, 494);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(97, 31);
            this.button1.TabIndex = 10;
            this.button1.Text = "저장";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.Location = new System.Drawing.Point(502, 494);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(97, 31);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.White;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.Location = new System.Drawing.Point(273, 494);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(97, 31);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "저장 후 계속";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // FormDetect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(623, 537);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormDetect";
            this.Text = "FormDetect";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDetect_FormClosing);
            this.Load += new System.EventHandler(this.FormDetect_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnForSensors;
        private System.Windows.Forms.Button btnForWorkers;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ComboBox cmbUseWorker;
        private System.Windows.Forms.ComboBox cmbUseEquip;
        private System.Windows.Forms.ComboBox cmbUseVehicle;
        private System.Windows.Forms.ComboBox cmbEquips;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbVehicles;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbWorkers;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel panelSpace1;
        private System.Windows.Forms.Panel panelSpace2;
    }
}