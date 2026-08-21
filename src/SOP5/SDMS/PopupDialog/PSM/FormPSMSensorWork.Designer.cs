namespace SDMS.PopupDialog
{
    partial class FormPSMSensorWork
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
            this.chkAll = new System.Windows.Forms.CheckBox();
            this.dtFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtFromTime = new System.Windows.Forms.DateTimePicker();
            this.lblSymbol = new System.Windows.Forms.Label();
            this.lblWork = new System.Windows.Forms.Label();
            this.rdoUnvisible = new System.Windows.Forms.RadioButton();
            this.rdoWork = new System.Windows.Forms.RadioButton();
            this.rdoLocalOff = new System.Windows.Forms.RadioButton();
            this.rdoON = new System.Windows.Forms.RadioButton();
            this.rdoOFF = new System.Windows.Forms.RadioButton();
            this.dtToTime = new System.Windows.Forms.DateTimePicker();
            this.dtToDate = new System.Windows.Forms.DateTimePicker();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnSave = new UnE.GUI.ImageButton();
            this.btnCancel = new UnE.GUI.ImageButton();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.btnSave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.chkAll.ForeColor = System.Drawing.Color.White;
            this.chkAll.Location = new System.Drawing.Point(209, 11);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(87, 22);
            this.chkAll.TabIndex = 7;
            this.chkAll.Text = "일괄적용";
            this.chkAll.UseVisualStyleBackColor = true;
            // 
            // dtFromDate
            // 
            this.dtFromDate.CustomFormat = "yyyy-MM-dd";
            this.dtFromDate.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.dtFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFromDate.Location = new System.Drawing.Point(124, 241);
            this.dtFromDate.Name = "dtFromDate";
            this.dtFromDate.Size = new System.Drawing.Size(105, 27);
            this.dtFromDate.TabIndex = 3;
            this.dtFromDate.Value = new System.DateTime(2015, 12, 18, 0, 0, 0, 0);
            // 
            // dtFromTime
            // 
            this.dtFromTime.CustomFormat = "HH시 m분";
            this.dtFromTime.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.dtFromTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFromTime.Location = new System.Drawing.Point(235, 241);
            this.dtFromTime.Name = "dtFromTime";
            this.dtFromTime.ShowUpDown = true;
            this.dtFromTime.Size = new System.Drawing.Size(77, 27);
            this.dtFromTime.TabIndex = 4;
            this.dtFromTime.Value = new System.DateTime(2015, 12, 18, 22, 30, 0, 0);
            // 
            // lblSymbol
            // 
            this.lblSymbol.AutoSize = true;
            this.lblSymbol.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblSymbol.ForeColor = System.Drawing.Color.White;
            this.lblSymbol.Location = new System.Drawing.Point(318, 247);
            this.lblSymbol.Name = "lblSymbol";
            this.lblSymbol.Size = new System.Drawing.Size(26, 18);
            this.lblSymbol.TabIndex = 6;
            this.lblSymbol.Text = " ~ ";
            // 
            // lblWork
            // 
            this.lblWork.AutoSize = true;
            this.lblWork.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblWork.ForeColor = System.Drawing.Color.White;
            this.lblWork.Location = new System.Drawing.Point(122, 223);
            this.lblWork.Name = "lblWork";
            this.lblWork.Size = new System.Drawing.Size(181, 18);
            this.lblWork.TabIndex = 3;
            this.lblWork.Text = "작업기간을 설정해 주세요";
            // 
            // rdoUnvisible
            // 
            this.rdoUnvisible.AutoSize = true;
            this.rdoUnvisible.Checked = true;
            this.rdoUnvisible.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rdoUnvisible.ForeColor = System.Drawing.Color.White;
            this.rdoUnvisible.Location = new System.Drawing.Point(15, 263);
            this.rdoUnvisible.Name = "rdoUnvisible";
            this.rdoUnvisible.Size = new System.Drawing.Size(86, 22);
            this.rdoUnvisible.TabIndex = 2;
            this.rdoUnvisible.TabStop = true;
            this.rdoUnvisible.Text = "숨김버튼";
            this.rdoUnvisible.UseVisualStyleBackColor = true;
            this.rdoUnvisible.Visible = false;
            // 
            // rdoWork
            // 
            this.rdoWork.AutoSize = true;
            this.rdoWork.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rdoWork.ForeColor = System.Drawing.Color.White;
            this.rdoWork.Location = new System.Drawing.Point(15, 245);
            this.rdoWork.Name = "rdoWork";
            this.rdoWork.Size = new System.Drawing.Size(107, 22);
            this.rdoWork.TabIndex = 2;
            this.rdoWork.Text = "작업중 OFF";
            this.rdoWork.UseVisualStyleBackColor = true;
            this.rdoWork.Visible = false;
            // 
            // rdoLocalOff
            // 
            this.rdoLocalOff.AutoSize = true;
            this.rdoLocalOff.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rdoLocalOff.ForeColor = System.Drawing.Color.White;
            this.rdoLocalOff.Location = new System.Drawing.Point(99, 11);
            this.rdoLocalOff.Name = "rdoLocalOff";
            this.rdoLocalOff.Size = new System.Drawing.Size(84, 22);
            this.rdoLocalOff.TabIndex = 0;
            this.rdoLocalOff.Text = "센서 Off";
            this.rdoLocalOff.UseVisualStyleBackColor = true;
            // 
            // rdoON
            // 
            this.rdoON.AutoSize = true;
            this.rdoON.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rdoON.ForeColor = System.Drawing.Color.White;
            this.rdoON.Location = new System.Drawing.Point(11, 11);
            this.rdoON.Name = "rdoON";
            this.rdoON.Size = new System.Drawing.Size(82, 22);
            this.rdoON.TabIndex = 0;
            this.rdoON.Text = "센서 On";
            this.rdoON.UseVisualStyleBackColor = true;
            // 
            // rdoOFF
            // 
            this.rdoOFF.AutoSize = true;
            this.rdoOFF.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rdoOFF.ForeColor = System.Drawing.Color.White;
            this.rdoOFF.Location = new System.Drawing.Point(15, 223);
            this.rdoOFF.Name = "rdoOFF";
            this.rdoOFF.Size = new System.Drawing.Size(92, 22);
            this.rdoOFF.TabIndex = 1;
            this.rdoOFF.Text = "센서 OFF";
            this.rdoOFF.UseVisualStyleBackColor = true;
            this.rdoOFF.Visible = false;
            // 
            // dtToTime
            // 
            this.dtToTime.CalendarFont = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.dtToTime.CustomFormat = "HH시 m분";
            this.dtToTime.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.dtToTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtToTime.Location = new System.Drawing.Point(236, 274);
            this.dtToTime.Name = "dtToTime";
            this.dtToTime.ShowUpDown = true;
            this.dtToTime.Size = new System.Drawing.Size(77, 27);
            this.dtToTime.TabIndex = 6;
            this.dtToTime.Value = new System.DateTime(2015, 12, 18, 10, 30, 45, 0);
            // 
            // dtToDate
            // 
            this.dtToDate.CalendarFont = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.dtToDate.CustomFormat = "yyyy-MM-dd";
            this.dtToDate.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.dtToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtToDate.Location = new System.Drawing.Point(125, 274);
            this.dtToDate.Name = "dtToDate";
            this.dtToDate.Size = new System.Drawing.Size(105, 27);
            this.dtToDate.TabIndex = 5;
            this.dtToDate.Value = new System.DateTime(2015, 12, 18, 0, 0, 0, 0);
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            this.lblTitle.Font = new System.Drawing.Font(Program.prgFont, 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(354, 32);
            this.lblTitle.TabIndex = 8;
            this.lblTitle.Text = "위험물질 감지센서 설정";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.ButtonText = "";
            this.btnSave.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSave.ImageClicked = global::SDMS.Properties.Resources.BtnSave_101_57_Click;
            this.btnSave.ImageDisabled = null;
            this.btnSave.ImageMouseOver = global::SDMS.Properties.Resources.BtnSave_101_57_Click;
            this.btnSave.ImageNormal = global::SDMS.Properties.Resources.BtnSave_101_57_Default;
            this.btnSave.Location = new System.Drawing.Point(222, 98);
            this.btnSave.Name = "btnSave";
            this.btnSave.Owner = null;
            this.btnSave.Size = new System.Drawing.Size(52, 27);
            this.btnSave.TabIndex = 12;
            this.btnSave.TabStop = false;
            this.btnSave.TextColor = System.Drawing.Color.Black;
            this.btnSave.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSave.ToolTipText = "";
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
            this.btnCancel.Location = new System.Drawing.Point(280, 98);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(52, 27);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.TabStop = false;
            this.btnCancel.TextColor = System.Drawing.Color.Black;
            this.btnCancel.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ToolTipText = "";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(45)))), ((int)(((byte)(40)))));
            this.panel1.Controls.Add(this.chkAll);
            this.panel1.Controls.Add(this.rdoLocalOff);
            this.panel1.Controls.Add(this.rdoON);
            this.panel1.Location = new System.Drawing.Point(24, 46);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(308, 45);
            this.panel1.TabIndex = 13;
            // 
            // FormPSMSensorWork
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.ClientSize = new System.Drawing.Size(354, 137);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.dtToDate);
            this.Controls.Add(this.dtToTime);
            this.Controls.Add(this.dtFromDate);
            this.Controls.Add(this.dtFromTime);
            this.Controls.Add(this.rdoOFF);
            this.Controls.Add(this.lblSymbol);
            this.Controls.Add(this.lblWork);
            this.Controls.Add(this.rdoUnvisible);
            this.Controls.Add(this.rdoWork);
            this.Name = "FormPSMSensorWork";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "위험물질 감지센서 설정";
            ((System.ComponentModel.ISupportInitialize)(this.btnSave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rdoON;
        private System.Windows.Forms.RadioButton rdoOFF;
        private System.Windows.Forms.RadioButton rdoWork;
        private System.Windows.Forms.DateTimePicker dtFromDate;
        private System.Windows.Forms.DateTimePicker dtFromTime;
        private System.Windows.Forms.Label lblSymbol;
        private System.Windows.Forms.Label lblWork;
        private System.Windows.Forms.RadioButton rdoUnvisible;
        private System.Windows.Forms.RadioButton rdoLocalOff;
        private System.Windows.Forms.CheckBox chkAll;
        private System.Windows.Forms.DateTimePicker dtToTime;
        private System.Windows.Forms.DateTimePicker dtToDate;
        private System.Windows.Forms.Label lblTitle;
        private UnE.GUI.ImageButton btnSave;
        private UnE.GUI.ImageButton btnCancel;
        private System.Windows.Forms.Panel panel1;
    }
}