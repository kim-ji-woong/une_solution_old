namespace BuildingSMS
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.cboBuildings = new System.Windows.Forms.ComboBox();
            this.cboFloors = new System.Windows.Forms.ComboBox();
            this.btnFire = new System.Windows.Forms.Button();
            this.textBoxInput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxOutput = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSendSMS = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cboBuildings
            // 
            this.cboBuildings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuildings.FormattingEnabled = true;
            this.cboBuildings.Location = new System.Drawing.Point(26, 23);
            this.cboBuildings.Name = "cboBuildings";
            this.cboBuildings.Size = new System.Drawing.Size(121, 20);
            this.cboBuildings.TabIndex = 0;
            this.cboBuildings.SelectedIndexChanged += new System.EventHandler(this.cboBuildings_SelectedIndexChanged);
            // 
            // cboFloors
            // 
            this.cboFloors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFloors.FormattingEnabled = true;
            this.cboFloors.Location = new System.Drawing.Point(171, 23);
            this.cboFloors.Name = "cboFloors";
            this.cboFloors.Size = new System.Drawing.Size(121, 20);
            this.cboFloors.TabIndex = 0;
            // 
            // btnFire
            // 
            this.btnFire.Location = new System.Drawing.Point(332, 22);
            this.btnFire.Name = "btnFire";
            this.btnFire.Size = new System.Drawing.Size(75, 23);
            this.btnFire.TabIndex = 1;
            this.btnFire.Text = "화재발생";
            this.btnFire.UseVisualStyleBackColor = true;
            this.btnFire.Click += new System.EventHandler(this.btnFire_Click);
            // 
            // textBoxInput
            // 
            this.textBoxInput.Location = new System.Drawing.Point(26, 93);
            this.textBoxInput.Multiline = true;
            this.textBoxInput.Name = "textBoxInput";
            this.textBoxInput.Size = new System.Drawing.Size(381, 322);
            this.textBoxInput.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "입력문구";
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.Location = new System.Drawing.Point(427, 93);
            this.textBoxOutput.Multiline = true;
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.ReadOnly = true;
            this.textBoxOutput.Size = new System.Drawing.Size(381, 322);
            this.textBoxOutput.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(426, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "전송문구";
            // 
            // btnSendSMS
            // 
            this.btnSendSMS.Location = new System.Drawing.Point(733, 23);
            this.btnSendSMS.Name = "btnSendSMS";
            this.btnSendSMS.Size = new System.Drawing.Size(75, 23);
            this.btnSendSMS.TabIndex = 4;
            this.btnSendSMS.Text = "상황전파";
            this.btnSendSMS.UseVisualStyleBackColor = true;
            this.btnSendSMS.Click += new System.EventHandler(this.btnSendSMS_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(821, 450);
            this.Controls.Add(this.btnSendSMS);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxOutput);
            this.Controls.Add(this.textBoxInput);
            this.Controls.Add(this.btnFire);
            this.Controls.Add(this.cboFloors);
            this.Controls.Add(this.cboBuildings);
            this.Name = "FormMain";
            this.Text = "화재 비상상황 전파";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboBuildings;
        private System.Windows.Forms.ComboBox cboFloors;
        private System.Windows.Forms.Button btnFire;
        private System.Windows.Forms.TextBox textBoxInput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxOutput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSendSMS;
    }
}

