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
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlType = new System.Windows.Forms.Panel();
            this.chkAll = new System.Windows.Forms.CheckBox();
            this.dtToDate = new System.Windows.Forms.DateTimePicker();
            this.dtToTime = new System.Windows.Forms.DateTimePicker();
            this.dtFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtFromTime = new System.Windows.Forms.DateTimePicker();
            this.lblSymbol = new System.Windows.Forms.Label();
            this.lblWork = new System.Windows.Forms.Label();
            this.rdoUnvisible = new System.Windows.Forms.RadioButton();
            this.rdoWork = new System.Windows.Forms.RadioButton();
            this.rdoLocalOff = new System.Windows.Forms.RadioButton();
            this.rdoON = new System.Windows.Forms.RadioButton();
            this.rdoOFF = new System.Windows.Forms.RadioButton();
            this.pnlType.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(206, 63);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "적용";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(287, 63);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "닫기";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // pnlType
            // 
            this.pnlType.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlType.BackColor = System.Drawing.Color.White;
            this.pnlType.Controls.Add(this.chkAll);
            this.pnlType.Controls.Add(this.dtToDate);
            this.pnlType.Controls.Add(this.dtToTime);
            this.pnlType.Controls.Add(this.dtFromDate);
            this.pnlType.Controls.Add(this.dtFromTime);
            this.pnlType.Controls.Add(this.lblSymbol);
            this.pnlType.Controls.Add(this.lblWork);
            this.pnlType.Controls.Add(this.rdoUnvisible);
            this.pnlType.Controls.Add(this.rdoWork);
            this.pnlType.Controls.Add(this.rdoLocalOff);
            this.pnlType.Controls.Add(this.rdoON);
            this.pnlType.Controls.Add(this.rdoOFF);
            this.pnlType.Location = new System.Drawing.Point(12, 12);
            this.pnlType.Name = "pnlType";
            this.pnlType.Size = new System.Drawing.Size(350, 45);
            this.pnlType.TabIndex = 0;
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.Location = new System.Drawing.Point(269, 15);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(72, 16);
            this.chkAll.TabIndex = 7;
            this.chkAll.Text = "일괄적용";
            this.chkAll.UseVisualStyleBackColor = true;
            // 
            // dtToDate
            // 
            this.dtToDate.CustomFormat = "yyyy-MM-dd";
            this.dtToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtToDate.Location = new System.Drawing.Point(354, 55);
            this.dtToDate.Name = "dtToDate";
            this.dtToDate.Size = new System.Drawing.Size(105, 21);
            this.dtToDate.TabIndex = 5;
            this.dtToDate.Value = new System.DateTime(2015, 12, 18, 0, 0, 0, 0);
            // 
            // dtToTime
            // 
            this.dtToTime.CustomFormat = "HH시 m분";
            this.dtToTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtToTime.Location = new System.Drawing.Point(465, 55);
            this.dtToTime.Name = "dtToTime";
            this.dtToTime.ShowUpDown = true;
            this.dtToTime.Size = new System.Drawing.Size(77, 21);
            this.dtToTime.TabIndex = 6;
            this.dtToTime.Value = new System.DateTime(2015, 12, 18, 10, 30, 45, 0);
            // 
            // dtFromDate
            // 
            this.dtFromDate.CustomFormat = "yyyy-MM-dd";
            this.dtFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFromDate.Location = new System.Drawing.Point(132, 55);
            this.dtFromDate.Name = "dtFromDate";
            this.dtFromDate.Size = new System.Drawing.Size(105, 21);
            this.dtFromDate.TabIndex = 3;
            this.dtFromDate.Value = new System.DateTime(2015, 12, 18, 0, 0, 0, 0);
            // 
            // dtFromTime
            // 
            this.dtFromTime.CustomFormat = "HH시 m분";
            this.dtFromTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFromTime.Location = new System.Drawing.Point(243, 55);
            this.dtFromTime.Name = "dtFromTime";
            this.dtFromTime.ShowUpDown = true;
            this.dtFromTime.Size = new System.Drawing.Size(77, 21);
            this.dtFromTime.TabIndex = 4;
            this.dtFromTime.Value = new System.DateTime(2015, 12, 18, 22, 30, 0, 0);
            // 
            // lblSymbol
            // 
            this.lblSymbol.AutoSize = true;
            this.lblSymbol.Location = new System.Drawing.Point(326, 61);
            this.lblSymbol.Name = "lblSymbol";
            this.lblSymbol.Size = new System.Drawing.Size(22, 12);
            this.lblSymbol.TabIndex = 6;
            this.lblSymbol.Text = " ~ ";
            // 
            // lblWork
            // 
            this.lblWork.AutoSize = true;
            this.lblWork.Location = new System.Drawing.Point(130, 37);
            this.lblWork.Name = "lblWork";
            this.lblWork.Size = new System.Drawing.Size(145, 12);
            this.lblWork.TabIndex = 3;
            this.lblWork.Text = "작업기간을 설정해 주세요";
            // 
            // rdoUnvisible
            // 
            this.rdoUnvisible.AutoSize = true;
            this.rdoUnvisible.Checked = true;
            this.rdoUnvisible.Location = new System.Drawing.Point(23, 77);
            this.rdoUnvisible.Name = "rdoUnvisible";
            this.rdoUnvisible.Size = new System.Drawing.Size(71, 16);
            this.rdoUnvisible.TabIndex = 2;
            this.rdoUnvisible.TabStop = true;
            this.rdoUnvisible.Text = "숨김버튼";
            this.rdoUnvisible.UseVisualStyleBackColor = true;
            this.rdoUnvisible.Visible = false;
            // 
            // rdoWork
            // 
            this.rdoWork.AutoSize = true;
            this.rdoWork.Location = new System.Drawing.Point(23, 59);
            this.rdoWork.Name = "rdoWork";
            this.rdoWork.Size = new System.Drawing.Size(86, 16);
            this.rdoWork.TabIndex = 2;
            this.rdoWork.Text = "작업중 OFF";
            this.rdoWork.UseVisualStyleBackColor = true;
            this.rdoWork.Visible = false;
            // 
            // rdoLocalOff
            // 
            this.rdoLocalOff.AutoSize = true;
            this.rdoLocalOff.Location = new System.Drawing.Point(132, 15);
            this.rdoLocalOff.Name = "rdoLocalOff";
            this.rdoLocalOff.Size = new System.Drawing.Size(66, 16);
            this.rdoLocalOff.TabIndex = 0;
            this.rdoLocalOff.Text = "센서 Off";
            this.rdoLocalOff.UseVisualStyleBackColor = true;
            // 
            // rdoON
            // 
            this.rdoON.AutoSize = true;
            this.rdoON.Location = new System.Drawing.Point(23, 15);
            this.rdoON.Name = "rdoON";
            this.rdoON.Size = new System.Drawing.Size(67, 16);
            this.rdoON.TabIndex = 0;
            this.rdoON.Text = "센서 On";
            this.rdoON.UseVisualStyleBackColor = true;
            // 
            // rdoOFF
            // 
            this.rdoOFF.AutoSize = true;
            this.rdoOFF.Location = new System.Drawing.Point(23, 37);
            this.rdoOFF.Name = "rdoOFF";
            this.rdoOFF.Size = new System.Drawing.Size(74, 16);
            this.rdoOFF.TabIndex = 1;
            this.rdoOFF.Text = "센서 OFF";
            this.rdoOFF.UseVisualStyleBackColor = true;
            this.rdoOFF.Visible = false;
            // 
            // FormPSMSensorWork
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(374, 98);
            this.Controls.Add(this.pnlType);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormPSMSensorWork";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "위험물질 감지센서 설정";
            this.pnlType.ResumeLayout(false);
            this.pnlType.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel pnlType;
        private System.Windows.Forms.RadioButton rdoON;
        private System.Windows.Forms.RadioButton rdoOFF;
        private System.Windows.Forms.RadioButton rdoWork;
        private System.Windows.Forms.DateTimePicker dtToDate;
        private System.Windows.Forms.DateTimePicker dtToTime;
        private System.Windows.Forms.DateTimePicker dtFromDate;
        private System.Windows.Forms.DateTimePicker dtFromTime;
        private System.Windows.Forms.Label lblSymbol;
        private System.Windows.Forms.Label lblWork;
        private System.Windows.Forms.RadioButton rdoUnvisible;
        private System.Windows.Forms.RadioButton rdoLocalOff;
        private System.Windows.Forms.CheckBox chkAll;
    }
}