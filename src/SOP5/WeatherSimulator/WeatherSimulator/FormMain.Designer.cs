namespace WeatherSimulator
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
            this.cboWeatherType = new System.Windows.Forms.ComboBox();
            this.cboDuration = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.checkBoxEditMode = new System.Windows.Forms.CheckBox();
            this.labelDataCreatedTime = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cboWeatherType
            // 
            this.cboWeatherType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboWeatherType.FormattingEnabled = true;
            this.cboWeatherType.Items.AddRange(new object[] {
            "강우 및 풍속",
            "태풍",
            "지진 / 해일"});
            this.cboWeatherType.Location = new System.Drawing.Point(12, 12);
            this.cboWeatherType.Name = "cboWeatherType";
            this.cboWeatherType.Size = new System.Drawing.Size(140, 20);
            this.cboWeatherType.TabIndex = 0;
            this.cboWeatherType.SelectedIndexChanged += new System.EventHandler(this.cboWeatherType_SelectedIndexChanged);
            // 
            // cboDuration
            // 
            this.cboDuration.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDuration.Enabled = false;
            this.cboDuration.FormattingEnabled = true;
            this.cboDuration.Items.AddRange(new object[] {
            "유효기간 없음",
            "1일",
            "2일",
            "3일",
            "4일",
            "5일",
            "6일",
            "7일",
            "8일",
            "9일",
            "10일",
            "11일",
            "12일",
            "13일",
            "14일",
            "15일",
            "16일",
            "17일",
            "18일",
            "19일",
            "20일",
            "21일",
            "22일",
            "23일",
            "24일",
            "25일",
            "26일",
            "27일",
            "28일",
            "29일",
            "30일"});
            this.cboDuration.Location = new System.Drawing.Point(313, 12);
            this.cboDuration.Name = "cboDuration";
            this.cboDuration.Size = new System.Drawing.Size(98, 20);
            this.cboDuration.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(206, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "데이터 유효기간 :";
            // 
            // panelMain
            // 
            this.panelMain.Location = new System.Drawing.Point(12, 48);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(844, 378);
            this.panelMain.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(791, 435);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(65, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnApply
            // 
            this.btnApply.Enabled = false;
            this.btnApply.Location = new System.Drawing.Point(717, 435);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(65, 23);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "적용";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // checkBoxEditMode
            // 
            this.checkBoxEditMode.AutoSize = true;
            this.checkBoxEditMode.Location = new System.Drawing.Point(473, 16);
            this.checkBoxEditMode.Name = "checkBoxEditMode";
            this.checkBoxEditMode.Size = new System.Drawing.Size(72, 16);
            this.checkBoxEditMode.TabIndex = 4;
            this.checkBoxEditMode.Text = "편집모드";
            this.checkBoxEditMode.UseVisualStyleBackColor = true;
            this.checkBoxEditMode.CheckedChanged += new System.EventHandler(this.checkBoxNewData_CheckedChanged);
            // 
            // labelDataCreatedTime
            // 
            this.labelDataCreatedTime.AutoSize = true;
            this.labelDataCreatedTime.Location = new System.Drawing.Point(622, 17);
            this.labelDataCreatedTime.Name = "labelDataCreatedTime";
            this.labelDataCreatedTime.Size = new System.Drawing.Size(61, 12);
            this.labelDataCreatedTime.TabIndex = 5;
            this.labelDataCreatedTime.Text = "작성시간 :";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(868, 467);
            this.Controls.Add(this.labelDataCreatedTime);
            this.Controls.Add(this.checkBoxEditMode);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboDuration);
            this.Controls.Add(this.cboWeatherType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormMain";
            this.Text = "기후정보 입력기";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboWeatherType;
        private System.Windows.Forms.ComboBox cboDuration;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.CheckBox checkBoxEditMode;
        private System.Windows.Forms.Label labelDataCreatedTime;
    }
}

