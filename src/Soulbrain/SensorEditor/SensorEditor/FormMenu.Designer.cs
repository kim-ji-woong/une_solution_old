
namespace SensorEditor
{
    partial class FormMenu
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
            this.cboBuildingGroup = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboBuilding = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cboZone = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cboSensorType = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cboSensor = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxX = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxZ = new System.Windows.Forms.TextBox();
            this.btnSaveDB = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "건물그룹";
            // 
            // cboBuildingGroup
            // 
            this.cboBuildingGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuildingGroup.FormattingEnabled = true;
            this.cboBuildingGroup.Location = new System.Drawing.Point(96, 22);
            this.cboBuildingGroup.Name = "cboBuildingGroup";
            this.cboBuildingGroup.Size = new System.Drawing.Size(162, 20);
            this.cboBuildingGroup.TabIndex = 1;
            this.cboBuildingGroup.SelectedIndexChanged += new System.EventHandler(this.cboBuildingGroup_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "건물";
            // 
            // cboBuilding
            // 
            this.cboBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuilding.FormattingEnabled = true;
            this.cboBuilding.Location = new System.Drawing.Point(96, 62);
            this.cboBuilding.Name = "cboBuilding";
            this.cboBuilding.Size = new System.Drawing.Size(162, 20);
            this.cboBuilding.TabIndex = 1;
            this.cboBuilding.SelectedIndexChanged += new System.EventHandler(this.cboBuilding_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Zone";
            // 
            // cboZone
            // 
            this.cboZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboZone.FormattingEnabled = true;
            this.cboZone.Location = new System.Drawing.Point(96, 103);
            this.cboZone.Name = "cboZone";
            this.cboZone.Size = new System.Drawing.Size(162, 20);
            this.cboZone.TabIndex = 1;
            this.cboZone.SelectedIndexChanged += new System.EventHandler(this.cboZone_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(330, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "센서타입";
            // 
            // cboSensorType
            // 
            this.cboSensorType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSensorType.FormattingEnabled = true;
            this.cboSensorType.Items.AddRange(new object[] {
            "화재",
            "CCTV"});
            this.cboSensorType.Location = new System.Drawing.Point(400, 22);
            this.cboSensorType.Name = "cboSensorType";
            this.cboSensorType.Size = new System.Drawing.Size(162, 20);
            this.cboSensorType.TabIndex = 1;
            this.cboSensorType.SelectedIndexChanged += new System.EventHandler(this.cboSensorType_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(330, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "센서";
            // 
            // cboSensor
            // 
            this.cboSensor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSensor.FormattingEnabled = true;
            this.cboSensor.Location = new System.Drawing.Point(400, 62);
            this.cboSensor.Name = "cboSensor";
            this.cboSensor.Size = new System.Drawing.Size(162, 20);
            this.cboSensor.TabIndex = 1;
            this.cboSensor.SelectedIndexChanged += new System.EventHandler(this.cboSensor_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(388, 107);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(13, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "X";
            // 
            // textBoxX
            // 
            this.textBoxX.Location = new System.Drawing.Point(407, 102);
            this.textBoxX.Name = "textBoxX";
            this.textBoxX.Size = new System.Drawing.Size(65, 21);
            this.textBoxX.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(478, 107);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(13, 12);
            this.label7.TabIndex = 2;
            this.label7.Text = "Z";
            // 
            // textBoxZ
            // 
            this.textBoxZ.Location = new System.Drawing.Point(497, 102);
            this.textBoxZ.Name = "textBoxZ";
            this.textBoxZ.Size = new System.Drawing.Size(65, 21);
            this.textBoxZ.TabIndex = 3;
            // 
            // btnSaveDB
            // 
            this.btnSaveDB.Location = new System.Drawing.Point(487, 146);
            this.btnSaveDB.Name = "btnSaveDB";
            this.btnSaveDB.Size = new System.Drawing.Size(75, 23);
            this.btnSaveDB.TabIndex = 4;
            this.btnSaveDB.Text = "DB 저장";
            this.btnSaveDB.UseVisualStyleBackColor = true;
            this.btnSaveDB.Click += new System.EventHandler(this.btnSaveDB_Click);
            // 
            // FormMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnSaveDB);
            this.Controls.Add(this.textBoxZ);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxX);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cboSensor);
            this.Controls.Add(this.cboSensorType);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cboZone);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cboBuilding);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cboBuildingGroup);
            this.Controls.Add(this.label1);
            this.Name = "FormMenu";
            this.Text = "FormMenu";
            this.Load += new System.EventHandler(this.FormMenu_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboBuildingGroup;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboBuilding;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboZone;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboSensorType;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboSensor;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxX;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxZ;
        private System.Windows.Forms.Button btnSaveDB;
    }
}