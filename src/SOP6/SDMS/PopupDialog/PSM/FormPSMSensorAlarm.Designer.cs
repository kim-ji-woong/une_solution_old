namespace SDMS.PopupDialog
{
    partial class FormPSMSensorAlarm
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
            this.lblLocation = new System.Windows.Forms.Label();
            this.lblMaterialName = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblSensorNo = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxDefLevel1 = new System.Windows.Forms.TextBox();
            this.textBoxDefLevel2 = new System.Windows.Forms.TextBox();
            this.textBoxDefLevel3 = new System.Windows.Forms.TextBox();
            this.lblDefUnit1 = new System.Windows.Forms.Label();
            this.lblDefUnit2 = new System.Windows.Forms.Label();
            this.lblDefUnit3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.lblCurrentUnit3 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblCurrentUnit2 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblCurrentUnit1 = new System.Windows.Forms.Label();
            this.textBoxCurrentLevel1 = new System.Windows.Forms.TextBox();
            this.textBoxCurrentLevel3 = new System.Windows.Forms.TextBox();
            this.textBoxCurrentLevel2 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnCancel = new UnE.GUI.ImageButton();
            this.btnOK = new UnE.GUI.ImageButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOK)).BeginInit();
            this.SuspendLayout();
            // 
            // lblLocation
            // 
            this.lblLocation.AutoSize = true;
            this.lblLocation.BackColor = System.Drawing.Color.Transparent;
            this.lblLocation.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblLocation.ForeColor = System.Drawing.Color.White;
            this.lblLocation.Location = new System.Drawing.Point(90, 88);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(72, 18);
            this.lblLocation.TabIndex = 5;
            this.lblLocation.Text = "위치 이름";
            // 
            // lblMaterialName
            // 
            this.lblMaterialName.AutoSize = true;
            this.lblMaterialName.BackColor = System.Drawing.Color.Transparent;
            this.lblMaterialName.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblMaterialName.ForeColor = System.Drawing.Color.White;
            this.lblMaterialName.Location = new System.Drawing.Point(90, 64);
            this.lblMaterialName.Name = "lblMaterialName";
            this.lblMaterialName.Size = new System.Drawing.Size(72, 18);
            this.lblMaterialName.TabIndex = 6;
            this.lblMaterialName.Text = "물질 이름";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(7, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 18);
            this.label4.TabIndex = 2;
            this.label4.Text = "위치          :";
            // 
            // lblSensorNo
            // 
            this.lblSensorNo.AutoSize = true;
            this.lblSensorNo.BackColor = System.Drawing.Color.Transparent;
            this.lblSensorNo.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblSensorNo.ForeColor = System.Drawing.Color.White;
            this.lblSensorNo.Location = new System.Drawing.Point(90, 41);
            this.lblSensorNo.Name = "lblSensorNo";
            this.lblSensorNo.Size = new System.Drawing.Size(72, 18);
            this.lblSensorNo.TabIndex = 7;
            this.lblSensorNo.Text = "센서 번호";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(7, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 18);
            this.label2.TabIndex = 3;
            this.label2.Text = "물질명      :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(7, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 18);
            this.label1.TabIndex = 4;
            this.label1.Text = "센서번호  :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(10, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 18);
            this.label3.TabIndex = 5;
            this.label3.Text = "1단계 알람 임계값 :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.Location = new System.Drawing.Point(10, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(140, 18);
            this.label5.TabIndex = 5;
            this.label5.Text = "2단계 알람 임계값 :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.Location = new System.Drawing.Point(10, 83);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(140, 18);
            this.label6.TabIndex = 5;
            this.label6.Text = "3단계 알람 임계값 :";
            // 
            // textBoxDefLevel1
            // 
            this.textBoxDefLevel1.Enabled = false;
            this.textBoxDefLevel1.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxDefLevel1.Location = new System.Drawing.Point(156, 20);
            this.textBoxDefLevel1.Name = "textBoxDefLevel1";
            this.textBoxDefLevel1.Size = new System.Drawing.Size(62, 27);
            this.textBoxDefLevel1.TabIndex = 8;
            this.textBoxDefLevel1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxDefLevel2
            // 
            this.textBoxDefLevel2.Enabled = false;
            this.textBoxDefLevel2.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxDefLevel2.Location = new System.Drawing.Point(156, 49);
            this.textBoxDefLevel2.Name = "textBoxDefLevel2";
            this.textBoxDefLevel2.Size = new System.Drawing.Size(62, 27);
            this.textBoxDefLevel2.TabIndex = 8;
            this.textBoxDefLevel2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxDefLevel3
            // 
            this.textBoxDefLevel3.Enabled = false;
            this.textBoxDefLevel3.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxDefLevel3.Location = new System.Drawing.Point(156, 78);
            this.textBoxDefLevel3.Name = "textBoxDefLevel3";
            this.textBoxDefLevel3.Size = new System.Drawing.Size(62, 27);
            this.textBoxDefLevel3.TabIndex = 8;
            this.textBoxDefLevel3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblDefUnit1
            // 
            this.lblDefUnit1.AutoSize = true;
            this.lblDefUnit1.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDefUnit1.Location = new System.Drawing.Point(224, 25);
            this.lblDefUnit1.Name = "lblDefUnit1";
            this.lblDefUnit1.Size = new System.Drawing.Size(38, 18);
            this.lblDefUnit1.TabIndex = 9;
            this.lblDefUnit1.Text = "단위";
            // 
            // lblDefUnit2
            // 
            this.lblDefUnit2.AutoSize = true;
            this.lblDefUnit2.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDefUnit2.Location = new System.Drawing.Point(224, 54);
            this.lblDefUnit2.Name = "lblDefUnit2";
            this.lblDefUnit2.Size = new System.Drawing.Size(38, 18);
            this.lblDefUnit2.TabIndex = 9;
            this.lblDefUnit2.Text = "단위";
            // 
            // lblDefUnit3
            // 
            this.lblDefUnit3.AutoSize = true;
            this.lblDefUnit3.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDefUnit3.Location = new System.Drawing.Point(224, 83);
            this.lblDefUnit3.Name = "lblDefUnit3";
            this.lblDefUnit3.Size = new System.Drawing.Size(38, 18);
            this.lblDefUnit3.TabIndex = 9;
            this.lblDefUnit3.Text = "단위";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.lblDefUnit3);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.lblDefUnit2);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.lblDefUnit1);
            this.groupBox1.Controls.Add(this.textBoxDefLevel1);
            this.groupBox1.Controls.Add(this.textBoxDefLevel3);
            this.groupBox1.Controls.Add(this.textBoxDefLevel2);
            this.groupBox1.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(12, 120);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(268, 112);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "알람 초기 설정값";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.lblCurrentUnit3);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.lblCurrentUnit2);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.lblCurrentUnit1);
            this.groupBox2.Controls.Add(this.textBoxCurrentLevel1);
            this.groupBox2.Controls.Add(this.textBoxCurrentLevel3);
            this.groupBox2.Controls.Add(this.textBoxCurrentLevel2);
            this.groupBox2.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(286, 120);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(268, 112);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "알람 현재 설정값";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.Location = new System.Drawing.Point(10, 25);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(140, 18);
            this.label7.TabIndex = 5;
            this.label7.Text = "1단계 알람 임계값 :";
            // 
            // lblCurrentUnit3
            // 
            this.lblCurrentUnit3.AutoSize = true;
            this.lblCurrentUnit3.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblCurrentUnit3.Location = new System.Drawing.Point(224, 83);
            this.lblCurrentUnit3.Name = "lblCurrentUnit3";
            this.lblCurrentUnit3.Size = new System.Drawing.Size(38, 18);
            this.lblCurrentUnit3.TabIndex = 9;
            this.lblCurrentUnit3.Text = "단위";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label9.Location = new System.Drawing.Point(10, 54);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(140, 18);
            this.label9.TabIndex = 5;
            this.label9.Text = "2단계 알람 임계값 :";
            // 
            // lblCurrentUnit2
            // 
            this.lblCurrentUnit2.AutoSize = true;
            this.lblCurrentUnit2.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblCurrentUnit2.Location = new System.Drawing.Point(224, 54);
            this.lblCurrentUnit2.Name = "lblCurrentUnit2";
            this.lblCurrentUnit2.Size = new System.Drawing.Size(38, 18);
            this.lblCurrentUnit2.TabIndex = 9;
            this.lblCurrentUnit2.Text = "단위";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label11.Location = new System.Drawing.Point(10, 83);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(140, 18);
            this.label11.TabIndex = 5;
            this.label11.Text = "3단계 알람 임계값 :";
            // 
            // lblCurrentUnit1
            // 
            this.lblCurrentUnit1.AutoSize = true;
            this.lblCurrentUnit1.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblCurrentUnit1.Location = new System.Drawing.Point(224, 25);
            this.lblCurrentUnit1.Name = "lblCurrentUnit1";
            this.lblCurrentUnit1.Size = new System.Drawing.Size(38, 18);
            this.lblCurrentUnit1.TabIndex = 9;
            this.lblCurrentUnit1.Text = "단위";
            // 
            // textBoxCurrentLevel1
            // 
            this.textBoxCurrentLevel1.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxCurrentLevel1.Location = new System.Drawing.Point(156, 20);
            this.textBoxCurrentLevel1.Name = "textBoxCurrentLevel1";
            this.textBoxCurrentLevel1.Size = new System.Drawing.Size(62, 27);
            this.textBoxCurrentLevel1.TabIndex = 0;
            this.textBoxCurrentLevel1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxCurrentLevel3
            // 
            this.textBoxCurrentLevel3.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxCurrentLevel3.Location = new System.Drawing.Point(156, 78);
            this.textBoxCurrentLevel3.Name = "textBoxCurrentLevel3";
            this.textBoxCurrentLevel3.Size = new System.Drawing.Size(62, 27);
            this.textBoxCurrentLevel3.TabIndex = 2;
            this.textBoxCurrentLevel3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxCurrentLevel2
            // 
            this.textBoxCurrentLevel2.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxCurrentLevel2.Location = new System.Drawing.Point(156, 49);
            this.textBoxCurrentLevel2.Name = "textBoxCurrentLevel2";
            this.textBoxCurrentLevel2.Size = new System.Drawing.Size(62, 27);
            this.textBoxCurrentLevel2.TabIndex = 1;
            this.textBoxCurrentLevel2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font(Program.prgFont, 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.label8.Location = new System.Drawing.Point(7, 6);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(146, 22);
            this.label8.TabIndex = 11;
            this.label8.Text = "센서 알람값 설정";
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.ButtonText = "";
            this.btnCancel.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ImageClicked = global::SDMS.Properties.Resources.BtnCancel_Click;
            this.btnCancel.ImageDisabled = null;
            this.btnCancel.ImageMouseOver = global::SDMS.Properties.Resources.BtnCancel_Click;
            this.btnCancel.ImageNormal = global::SDMS.Properties.Resources.BtnCancel_Default;
            this.btnCancel.Location = new System.Drawing.Point(502, 242);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(52, 28);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.TabStop = false;
            this.btnCancel.TextColor = System.Drawing.Color.Black;
            this.btnCancel.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ToolTipText = "";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.Transparent;
            this.btnOK.ButtonText = "";
            this.btnOK.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.ImageClicked = global::SDMS.Properties.Resources.BtnOk_Click;
            this.btnOK.ImageDisabled = null;
            this.btnOK.ImageMouseOver = global::SDMS.Properties.Resources.BtnOk_Click;
            this.btnOK.ImageNormal = global::SDMS.Properties.Resources.BtnOk_Default;
            this.btnOK.Location = new System.Drawing.Point(442, 242);
            this.btnOK.Name = "btnOK";
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(52, 28);
            this.btnOK.TabIndex = 12;
            this.btnOK.TabStop = false;
            this.btnOK.TextColor = System.Drawing.Color.Black;
            this.btnOK.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.ToolTipText = "";
            this.btnOK.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // FormPSMSensorAlarm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SDMS.Properties.Resources.PSMDepartment_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(566, 285);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblLocation);
            this.Controls.Add(this.lblMaterialName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblSensorNo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FormPSMSensorAlarm";
            this.Text = "센서 알람값 설정";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOK)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.Label lblMaterialName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblSensorNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxDefLevel1;
        private System.Windows.Forms.TextBox textBoxDefLevel2;
        private System.Windows.Forms.TextBox textBoxDefLevel3;
        private System.Windows.Forms.Label lblDefUnit1;
        private System.Windows.Forms.Label lblDefUnit2;
        private System.Windows.Forms.Label lblDefUnit3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblCurrentUnit3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblCurrentUnit2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblCurrentUnit1;
        private System.Windows.Forms.TextBox textBoxCurrentLevel1;
        private System.Windows.Forms.TextBox textBoxCurrentLevel3;
        private System.Windows.Forms.TextBox textBoxCurrentLevel2;
        private System.Windows.Forms.Label label8;
        private UnE.GUI.ImageButton btnCancel;
        private UnE.GUI.ImageButton btnOK;
    }
}