namespace SDMS.WeatherDisplay
{
    partial class FormWeatherDisplay
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
            this.components = new System.ComponentModel.Container();
            this.panelBody = new System.Windows.Forms.Panel();
            this.labelStatus = new System.Windows.Forms.Label();
            this.panelBottom = new SDMS.WeatherInfoPanel();
            this.tabEarthquake = new SDMS.WeatherDisplay.FormWeatherDisplay.TabButton();
            this.tabTyphoon = new SDMS.WeatherDisplay.FormWeatherDisplay.TabButton();
            this.tabRain = new SDMS.WeatherDisplay.FormWeatherDisplay.TabButton();
            this.timerDisplay = new System.Windows.Forms.Timer(this.components);
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelBody
            // 
            this.panelBody.Location = new System.Drawing.Point(0, 36);
            this.panelBody.Name = "panelBody";
            this.panelBody.Size = new System.Drawing.Size(476, 272);
            this.panelBody.TabIndex = 3;
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Font = new System.Drawing.Font("돋움", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(143)))), ((int)(((byte)(179)))));
            this.labelStatus.Location = new System.Drawing.Point(132, 13);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(125, 15);
            this.labelStatus.TabIndex = 0;
            this.labelStatus.Text = "기후정보 상태줄";
            this.labelStatus.Visible = false;
            // 
            // panelBottom
            // 
            this.panelBottom.BackgroundImage = global::SDMS.Properties.Resources.status_bar1;
            this.panelBottom.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelBottom.BufferLength = 34;
            this.panelBottom.Controls.Add(this.labelStatus);
            this.panelBottom.DisplayFont = new System.Drawing.Font("맑은 고딕", 23.5F);
            this.panelBottom.DisplayLength = 34;
            this.panelBottom.Location = new System.Drawing.Point(0, 308);
            this.panelBottom.MovingAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.NotMovingAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.panelBottom.RealTimeInfo = null;
            this.panelBottom.Size = new System.Drawing.Size(476, 37);
            this.panelBottom.TabIndex = 4;
            this.panelBottom.Text = "FormRealTimeInfo";
            this.panelBottom.TextColor = System.Drawing.Color.White;
            // 
            // tabEarthquake
            // 
            this.tabEarthquake.CheckedImage = null;
            this.tabEarthquake.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.tabEarthquake.Form = null;
            this.tabEarthquake.Location = new System.Drawing.Point(318, 0);
            this.tabEarthquake.Name = "tabEarthquake";
            this.tabEarthquake.NormalImage = null;
            this.tabEarthquake.Selected = true;
            this.tabEarthquake.Size = new System.Drawing.Size(159, 36);
            this.tabEarthquake.TabIndex = 2;
            this.tabEarthquake.UseVisualStyleBackColor = true;
            this.tabEarthquake.Click += new System.EventHandler(this.btnTab_Click);
            // 
            // tabTyphoon
            // 
            this.tabTyphoon.CheckedImage = null;
            this.tabTyphoon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.tabTyphoon.Form = null;
            this.tabTyphoon.Location = new System.Drawing.Point(159, 0);
            this.tabTyphoon.Name = "tabTyphoon";
            this.tabTyphoon.NormalImage = null;
            this.tabTyphoon.Selected = true;
            this.tabTyphoon.Size = new System.Drawing.Size(159, 36);
            this.tabTyphoon.TabIndex = 1;
            this.tabTyphoon.UseVisualStyleBackColor = true;
            this.tabTyphoon.Click += new System.EventHandler(this.btnTab_Click);
            // 
            // tabRain
            // 
            this.tabRain.CheckedImage = null;
            this.tabRain.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.tabRain.Form = null;
            this.tabRain.Location = new System.Drawing.Point(0, 0);
            this.tabRain.Name = "tabRain";
            this.tabRain.NormalImage = null;
            this.tabRain.Selected = true;
            this.tabRain.Size = new System.Drawing.Size(159, 36);
            this.tabRain.TabIndex = 0;
            this.tabRain.UseVisualStyleBackColor = true;
            this.tabRain.Click += new System.EventHandler(this.btnTab_Click);
            // 
            // timerDisplay
            // 
            this.timerDisplay.Interval = 5000;
            this.timerDisplay.Tick += new System.EventHandler(this.timerDisplay_Tick);
            // 
            // FormWeatherDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(476, 345);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelBody);
            this.Controls.Add(this.tabEarthquake);
            this.Controls.Add(this.tabTyphoon);
            this.Controls.Add(this.tabRain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormWeatherDisplay";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormWeatherDisplay_Load);
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TabButton tabRain;
        private TabButton tabTyphoon;
        private TabButton tabEarthquake;
        private System.Windows.Forms.Panel panelBody;
        private WeatherInfoPanel panelBottom;
        private System.Windows.Forms.Timer timerDisplay;
        private System.Windows.Forms.Label labelStatus;
    }
}