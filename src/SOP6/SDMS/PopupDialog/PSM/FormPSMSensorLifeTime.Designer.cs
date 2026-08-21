namespace SDMS.PopupDialog
{
    partial class FormPSMSensorLifeTime
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
            this.lblSensorNo = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblMaterialName = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblLocation = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.dtpIntallDate = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblDeadLine = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCancel = new UnE.GUI.ImageButton();
            this.btnApply = new UnE.GUI.ImageButton();
            this.btnNewType = new UnE.GUI.ImageButton();
            this.cboSenosrType = new UnE.GUI.ImageComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnApply)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewType)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(9, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "센서번호 :";
            // 
            // lblSensorNo
            // 
            this.lblSensorNo.AutoSize = true;
            this.lblSensorNo.BackColor = System.Drawing.Color.Transparent;
            this.lblSensorNo.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblSensorNo.ForeColor = System.Drawing.Color.White;
            this.lblSensorNo.Location = new System.Drawing.Point(92, 40);
            this.lblSensorNo.Name = "lblSensorNo";
            this.lblSensorNo.Size = new System.Drawing.Size(72, 18);
            this.lblSensorNo.TabIndex = 1;
            this.lblSensorNo.Text = "센서 번호";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(9, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 18);
            this.label2.TabIndex = 0;
            this.label2.Text = "물질명    :";
            // 
            // lblMaterialName
            // 
            this.lblMaterialName.AutoSize = true;
            this.lblMaterialName.BackColor = System.Drawing.Color.Transparent;
            this.lblMaterialName.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblMaterialName.ForeColor = System.Drawing.Color.White;
            this.lblMaterialName.Location = new System.Drawing.Point(92, 67);
            this.lblMaterialName.Name = "lblMaterialName";
            this.lblMaterialName.Size = new System.Drawing.Size(72, 18);
            this.lblMaterialName.TabIndex = 1;
            this.lblMaterialName.Text = "물질 이름";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(9, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 18);
            this.label4.TabIndex = 0;
            this.label4.Text = "위치        :";
            // 
            // lblLocation
            // 
            this.lblLocation.AutoSize = true;
            this.lblLocation.BackColor = System.Drawing.Color.Transparent;
            this.lblLocation.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblLocation.ForeColor = System.Drawing.Color.White;
            this.lblLocation.Location = new System.Drawing.Point(92, 96);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(72, 18);
            this.lblLocation.TabIndex = 1;
            this.lblLocation.Text = "위치 이름";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(9, 155);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 18);
            this.label6.TabIndex = 0;
            this.label6.Text = "설치일자 :";
            // 
            // dtpIntallDate
            // 
            this.dtpIntallDate.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.dtpIntallDate.Location = new System.Drawing.Point(92, 151);
            this.dtpIntallDate.Name = "dtpIntallDate";
            this.dtpIntallDate.Size = new System.Drawing.Size(208, 27);
            this.dtpIntallDate.TabIndex = 2;
            this.dtpIntallDate.ValueChanged += new System.EventHandler(this.dtpIntallDate_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(9, 124);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 18);
            this.label7.TabIndex = 0;
            this.label7.Text = "설치타입 :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(9, 184);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 18);
            this.label8.TabIndex = 0;
            this.label8.Text = "사용기한 :";
            // 
            // lblDeadLine
            // 
            this.lblDeadLine.AutoSize = true;
            this.lblDeadLine.BackColor = System.Drawing.Color.Transparent;
            this.lblDeadLine.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDeadLine.ForeColor = System.Drawing.Color.White;
            this.lblDeadLine.Location = new System.Drawing.Point(92, 184);
            this.lblDeadLine.Name = "lblDeadLine";
            this.lblDeadLine.Size = new System.Drawing.Size(72, 18);
            this.lblDeadLine.TabIndex = 1;
            this.lblDeadLine.Text = "사용 기한";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font(Program.prgFont, 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.label3.Location = new System.Drawing.Point(8, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(128, 22);
            this.label3.TabIndex = 6;
            this.label3.Text = "센서 교체 주기";
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
            this.btnCancel.Location = new System.Drawing.Point(248, 214);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(52, 28);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.TabStop = false;
            this.btnCancel.TextColor = System.Drawing.Color.Black;
            this.btnCancel.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ToolTipText = "";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnApply
            // 
            this.btnApply.BackColor = System.Drawing.Color.Transparent;
            this.btnApply.ButtonText = "";
            this.btnApply.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnApply.ImageClicked = global::SDMS.Properties.Resources.BtnOk_Click;
            this.btnApply.ImageDisabled = null;
            this.btnApply.ImageMouseOver = global::SDMS.Properties.Resources.BtnOk_Click;
            this.btnApply.ImageNormal = global::SDMS.Properties.Resources.BtnOk_Default;
            this.btnApply.Location = new System.Drawing.Point(190, 214);
            this.btnApply.Name = "btnApply";
            this.btnApply.Owner = null;
            this.btnApply.Size = new System.Drawing.Size(52, 28);
            this.btnApply.TabIndex = 14;
            this.btnApply.TabStop = false;
            this.btnApply.TextColor = System.Drawing.Color.Black;
            this.btnApply.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnApply.ToolTipText = "";
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnNewType
            // 
            this.btnNewType.BackColor = System.Drawing.Color.Transparent;
            this.btnNewType.ButtonText = "";
            this.btnNewType.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnNewType.ImageClicked = global::SDMS.Properties.Resources.BtnNewType_Click;
            this.btnNewType.ImageDisabled = null;
            this.btnNewType.ImageMouseOver = global::SDMS.Properties.Resources.BtnNewType_Click;
            this.btnNewType.ImageNormal = global::SDMS.Properties.Resources.BtnNewType_Default;
            this.btnNewType.Location = new System.Drawing.Point(240, 121);
            this.btnNewType.Name = "btnNewType";
            this.btnNewType.Owner = null;
            this.btnNewType.Size = new System.Drawing.Size(60, 26);
            this.btnNewType.TabIndex = 16;
            this.btnNewType.TabStop = false;
            this.btnNewType.TextColor = System.Drawing.Color.Black;
            this.btnNewType.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnNewType.ToolTipText = "";
            this.btnNewType.Click += new System.EventHandler(this.btnNewType_Click);
            // 
            // cboSenosrType
            // 
            this.cboSenosrType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSenosrType.Font = new System.Drawing.Font(Program.prgFont, 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboSenosrType.FormattingEnabled = true;
            this.cboSenosrType.ImageClicked = global::SDMS.Properties.Resources.ComboBoxDropDownBtn_Click;
            this.cboSenosrType.ImageDisabled = null;
            this.cboSenosrType.ImageMouseOver = global::SDMS.Properties.Resources.ComboBoxDropDownBtn_Click;
            this.cboSenosrType.ImageNormal = global::SDMS.Properties.Resources.ComboBoxDropDownBtn3_Default;
            this.cboSenosrType.Items.AddRange(new object[] {
            "모든 탐지 값을 표시",
            "몇 분 동안 표시하지 않습니다",
            "몇 시간 동안 표시하지 않습니다",
            "몇 일 동안 표시하지 않습니다",
            "완전히 표시하지 않습니다"});
            this.cboSenosrType.Location = new System.Drawing.Point(92, 121);
            this.cboSenosrType.Name = "cboSenosrType";
            this.cboSenosrType.Owner = null;
            this.cboSenosrType.Size = new System.Drawing.Size(142, 26);
            this.cboSenosrType.TabIndex = 20;
            this.cboSenosrType.TextColor = System.Drawing.Color.Black;
            this.cboSenosrType.TextFont = new System.Drawing.Font(Program.prgFont, 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboSenosrType.SelectedIndexChanged += new System.EventHandler(this.cboSenosrType_SelectedIndexChanged);
            // 
            // FormPSMSensorLifeTime
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SDMS.Properties.Resources.PSMDepartment_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(317, 254);
            this.Controls.Add(this.cboSenosrType);
            this.Controls.Add(this.btnNewType);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dtpIntallDate);
            this.Controls.Add(this.lblDeadLine);
            this.Controls.Add(this.lblLocation);
            this.Controls.Add(this.lblMaterialName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblSensorNo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label1);
            this.Name = "FormPSMSensorLifeTime";
            this.Text = "센서 교체주기";
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnApply)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewType)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblSensorNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblMaterialName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dtpIntallDate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblDeadLine;
        private System.Windows.Forms.Label label3;
        private UnE.GUI.ImageButton btnCancel;
        private UnE.GUI.ImageButton btnApply;
        private UnE.GUI.ImageButton btnNewType;
        private UnE.GUI.ImageComboBox cboSenosrType;
    }
}