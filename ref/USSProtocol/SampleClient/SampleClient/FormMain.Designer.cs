namespace SampleClient
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.checkBoxFire = new System.Windows.Forms.CheckBox();
            this.checkBoxPowerOff = new System.Windows.Forms.CheckBox();
            this.checkBoxEarthquake = new System.Windows.Forms.CheckBox();
            this.checkBoxWind = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP :";
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(63, 36);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.Size = new System.Drawing.Size(100, 21);
            this.textBoxIP.TabIndex = 1;
            this.textBoxIP.Text = "127.0.0.1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "Port :";
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(63, 63);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(100, 21);
            this.textBoxPort.TabIndex = 1;
            this.textBoxPort.Text = "19100";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(105, 167);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(58, 23);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "접속";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // checkBoxFire
            // 
            this.checkBoxFire.AutoSize = true;
            this.checkBoxFire.Checked = true;
            this.checkBoxFire.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxFire.Location = new System.Drawing.Point(24, 111);
            this.checkBoxFire.Name = "checkBoxFire";
            this.checkBoxFire.Size = new System.Drawing.Size(48, 16);
            this.checkBoxFire.TabIndex = 3;
            this.checkBoxFire.Text = "화재";
            this.checkBoxFire.UseVisualStyleBackColor = true;
            this.checkBoxFire.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBoxPowerOff
            // 
            this.checkBoxPowerOff.AutoSize = true;
            this.checkBoxPowerOff.Checked = true;
            this.checkBoxPowerOff.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxPowerOff.Location = new System.Drawing.Point(91, 111);
            this.checkBoxPowerOff.Name = "checkBoxPowerOff";
            this.checkBoxPowerOff.Size = new System.Drawing.Size(48, 16);
            this.checkBoxPowerOff.TabIndex = 3;
            this.checkBoxPowerOff.Text = "정전";
            this.checkBoxPowerOff.UseVisualStyleBackColor = true;
            this.checkBoxPowerOff.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkBoxEarthquake
            // 
            this.checkBoxEarthquake.AutoSize = true;
            this.checkBoxEarthquake.Location = new System.Drawing.Point(24, 133);
            this.checkBoxEarthquake.Name = "checkBoxEarthquake";
            this.checkBoxEarthquake.Size = new System.Drawing.Size(48, 16);
            this.checkBoxEarthquake.TabIndex = 3;
            this.checkBoxEarthquake.Text = "지진";
            this.checkBoxEarthquake.UseVisualStyleBackColor = true;
            this.checkBoxEarthquake.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBoxWind
            // 
            this.checkBoxWind.AutoSize = true;
            this.checkBoxWind.Location = new System.Drawing.Point(91, 133);
            this.checkBoxWind.Name = "checkBoxWind";
            this.checkBoxWind.Size = new System.Drawing.Size(48, 16);
            this.checkBoxWind.TabIndex = 3;
            this.checkBoxWind.Text = "강풍";
            this.checkBoxWind.UseVisualStyleBackColor = true;
            this.checkBoxWind.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(181, 209);
            this.Controls.Add(this.checkBoxWind);
            this.Controls.Add(this.checkBoxEarthquake);
            this.Controls.Add(this.checkBoxPowerOff);
            this.Controls.Add(this.checkBoxFire);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.textBoxPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxIP);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormMain";
            this.Text = "Sample Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.CheckBox checkBoxFire;
        private System.Windows.Forms.CheckBox checkBoxPowerOff;
        private System.Windows.Forms.CheckBox checkBoxEarthquake;
        private System.Windows.Forms.CheckBox checkBoxWind;
    }
}

