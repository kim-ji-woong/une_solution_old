namespace SOPMonitoringSystem.Popup
{
    partial class PopupWorkflowOption
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
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnEditManualTime = new System.Windows.Forms.Button();
            this.labelManualTime = new System.Windows.Forms.Label();
            this.radioManual = new System.Windows.Forms.RadioButton();
            this.radioAuto = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Location = new System.Drawing.Point(22, 109);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(202, 16);
            this.checkBox2.TabIndex = 10;
            this.checkBox2.Text = "상황 시작/종료 문자 메시지 사용";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(173, 162);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(77, 29);
            this.button2.TabIndex = 9;
            this.button2.Text = "시작취소";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(90, 162);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(77, 29);
            this.button1.TabIndex = 8;
            this.button1.Text = "시작";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnEditManualTime
            // 
            this.btnEditManualTime.Location = new System.Drawing.Point(160, 49);
            this.btnEditManualTime.Name = "btnEditManualTime";
            this.btnEditManualTime.Size = new System.Drawing.Size(45, 23);
            this.btnEditManualTime.TabIndex = 14;
            this.btnEditManualTime.Text = "편집";
            this.btnEditManualTime.UseVisualStyleBackColor = true;
            this.btnEditManualTime.Visible = false;
            this.btnEditManualTime.Click += new System.EventHandler(this.btnEditManualTime_Click);
            // 
            // labelManualTime
            // 
            this.labelManualTime.AutoSize = true;
            this.labelManualTime.Location = new System.Drawing.Point(41, 56);
            this.labelManualTime.Name = "labelManualTime";
            this.labelManualTime.Size = new System.Drawing.Size(113, 12);
            this.labelManualTime.TabIndex = 13;
            this.labelManualTime.Text = "0000-00-00 00:00:00";
            this.labelManualTime.Visible = false;
            // 
            // radioManual
            // 
            this.radioManual.AutoSize = true;
            this.radioManual.Location = new System.Drawing.Point(22, 34);
            this.radioManual.Name = "radioManual";
            this.radioManual.Size = new System.Drawing.Size(123, 16);
            this.radioManual.TabIndex = 11;
            this.radioManual.Text = "재난발생시간 입력";
            this.radioManual.UseVisualStyleBackColor = true;
            this.radioManual.CheckedChanged += new System.EventHandler(this.radioManual_CheckedChanged);
            // 
            // radioAuto
            // 
            this.radioAuto.AutoSize = true;
            this.radioAuto.Checked = true;
            this.radioAuto.Location = new System.Drawing.Point(22, 12);
            this.radioAuto.Name = "radioAuto";
            this.radioAuto.Size = new System.Drawing.Size(211, 16);
            this.radioAuto.TabIndex = 12;
            this.radioAuto.TabStop = true;
            this.radioAuto.Text = "현재시간을 재난발생시간으로 설정";
            this.radioAuto.UseVisualStyleBackColor = true;
            this.radioAuto.CheckedChanged += new System.EventHandler(this.radioAuto_CheckedChanged);
            // 
            // PopupWorkflowOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(337, 203);
            this.ControlBox = false;
            this.Controls.Add(this.btnEditManualTime);
            this.Controls.Add(this.labelManualTime);
            this.Controls.Add(this.radioManual);
            this.Controls.Add(this.radioAuto);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "PopupWorkflowOption";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "시작 이벤트 옵션";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnEditManualTime;
        private System.Windows.Forms.Label labelManualTime;
        private System.Windows.Forms.RadioButton radioManual;
        private System.Windows.Forms.RadioButton radioAuto;

    }
}