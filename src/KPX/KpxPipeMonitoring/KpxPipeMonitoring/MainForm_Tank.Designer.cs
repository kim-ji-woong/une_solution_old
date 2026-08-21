namespace KpxPipeMonitoring
{
    partial class MainForm_Tank
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm_Tank));
            this.pCenter = new System.Windows.Forms.Panel();
            this.panel_bottom = new System.Windows.Forms.Panel();
            this.pictureBox_back = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_notice = new System.Windows.Forms.Label();
            this.pictureBox_report = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel_top = new System.Windows.Forms.Panel();
            this.pictureBox_setting = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox_sound = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.label_date = new System.Windows.Forms.Label();
            this.panel_bottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_back)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_report)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel_top.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_setting)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_sound)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            this.SuspendLayout();
            // 
            // pCenter
            // 
            this.pCenter.BackColor = System.Drawing.Color.Transparent;
            this.pCenter.Location = new System.Drawing.Point(0, 83);
            this.pCenter.Name = "pCenter";
            this.pCenter.Size = new System.Drawing.Size(1920, 912);
            this.pCenter.TabIndex = 10;
            // 
            // panel_bottom
            // 
            this.panel_bottom.BackColor = System.Drawing.Color.Transparent;
            this.panel_bottom.BackgroundImage = global::KpxPipeMonitoring.Properties.Resources.Bottom;
            this.panel_bottom.Controls.Add(this.pictureBox_back);
            this.panel_bottom.Controls.Add(this.panel1);
            this.panel_bottom.Controls.Add(this.pictureBox_report);
            this.panel_bottom.Controls.Add(this.pictureBox1);
            this.panel_bottom.Location = new System.Drawing.Point(0, 995);
            this.panel_bottom.Name = "panel_bottom";
            this.panel_bottom.Size = new System.Drawing.Size(1920, 85);
            this.panel_bottom.TabIndex = 9;
            // 
            // pictureBox_back
            // 
            this.pictureBox_back.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_back.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox_back.Image = global::KpxPipeMonitoring.Properties.Resources.Back_Blue;
            this.pictureBox_back.Location = new System.Drawing.Point(1761, 24);
            this.pictureBox_back.Name = "pictureBox_back";
            this.pictureBox_back.Size = new System.Drawing.Size(130, 43);
            this.pictureBox_back.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_back.TabIndex = 19;
            this.pictureBox_back.TabStop = false;
            this.pictureBox_back.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_back_MouseClick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label_notice);
            this.panel1.Location = new System.Drawing.Point(197, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1437, 43);
            this.panel1.TabIndex = 18;
            // 
            // label_notice
            // 
            this.label_notice.AutoSize = true;
            this.label_notice.BackColor = System.Drawing.Color.Transparent;
            this.label_notice.Font = new System.Drawing.Font("굴림", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_notice.ForeColor = System.Drawing.Color.Yellow;
            this.label_notice.Location = new System.Drawing.Point(3, 7);
            this.label_notice.Name = "label_notice";
            this.label_notice.Size = new System.Drawing.Size(161, 27);
            this.label_notice.TabIndex = 10;
            this.label_notice.Text = "label_notice";
            // 
            // pictureBox_report
            // 
            this.pictureBox_report.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_report.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox_report.Image = global::KpxPipeMonitoring.Properties.Resources.Report_Blue;
            this.pictureBox_report.Location = new System.Drawing.Point(1761, 24);
            this.pictureBox_report.Name = "pictureBox_report";
            this.pictureBox_report.Size = new System.Drawing.Size(130, 43);
            this.pictureBox_report.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_report.TabIndex = 15;
            this.pictureBox_report.TabStop = false;
            this.pictureBox_report.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_report_MouseClick);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::KpxPipeMonitoring.Properties.Resources.notice_button;
            this.pictureBox1.Location = new System.Drawing.Point(45, 24);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(130, 43);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // panel_top
            // 
            this.panel_top.BackColor = System.Drawing.SystemColors.Control;
            this.panel_top.BackgroundImage = global::KpxPipeMonitoring.Properties.Resources.Top;
            this.panel_top.Controls.Add(this.pictureBox_setting);
            this.panel_top.Controls.Add(this.pictureBox2);
            this.panel_top.Controls.Add(this.pictureBox_sound);
            this.panel_top.Controls.Add(this.pictureBox5);
            this.panel_top.Controls.Add(this.pictureBox6);
            this.panel_top.Controls.Add(this.label_date);
            this.panel_top.Location = new System.Drawing.Point(0, 0);
            this.panel_top.Name = "panel_top";
            this.panel_top.Size = new System.Drawing.Size(1920, 85);
            this.panel_top.TabIndex = 8;
            this.panel_top.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panel_top_MouseDoubleClick);
            this.panel_top.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel_top_MouseDown);
            this.panel_top.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel_top_MouseMove);
            this.panel_top.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel_top_MouseUp);
            // 
            // pictureBox_setting
            // 
            this.pictureBox_setting.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_setting.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox_setting.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox_setting.Location = new System.Drawing.Point(1421, 22);
            this.pictureBox_setting.Name = "pictureBox_setting";
            this.pictureBox_setting.Size = new System.Drawing.Size(34, 34);
            this.pictureBox_setting.TabIndex = 18;
            this.pictureBox_setting.TabStop = false;
            this.pictureBox_setting.Click += new System.EventHandler(this.pictureBox_setting_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.Image = global::KpxPipeMonitoring.Properties.Resources.TankLegend;
            this.pictureBox2.Location = new System.Drawing.Point(559, 12);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(273, 60);
            this.pictureBox2.TabIndex = 23;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panel_top_MouseDoubleClick);
            this.pictureBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel_top_MouseDown);
            this.pictureBox2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel_top_MouseMove);
            this.pictureBox2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel_top_MouseUp);
            // 
            // pictureBox_sound
            // 
            this.pictureBox_sound.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_sound.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox_sound.Image = global::KpxPipeMonitoring.Properties.Resources.SoundOn;
            this.pictureBox_sound.Location = new System.Drawing.Point(1362, 15);
            this.pictureBox_sound.Name = "pictureBox_sound";
            this.pictureBox_sound.Size = new System.Drawing.Size(53, 49);
            this.pictureBox_sound.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_sound.TabIndex = 22;
            this.pictureBox_sound.TabStop = false;
            this.pictureBox_sound.Click += new System.EventHandler(this.pictureBox_sound_Click);
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox5.Image = global::KpxPipeMonitoring.Properties.Resources.small_line;
            this.pictureBox5.Location = new System.Drawing.Point(1471, 23);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(1, 30);
            this.pictureBox5.TabIndex = 19;
            this.pictureBox5.TabStop = false;
            this.pictureBox5.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panel_top_MouseDoubleClick);
            this.pictureBox5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel_top_MouseDown);
            this.pictureBox5.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel_top_MouseMove);
            this.pictureBox5.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel_top_MouseUp);
            // 
            // pictureBox6
            // 
            this.pictureBox6.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox6.Image = global::KpxPipeMonitoring.Properties.Resources.TitleTank;
            this.pictureBox6.Location = new System.Drawing.Point(12, 18);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(541, 48);
            this.pictureBox6.TabIndex = 13;
            this.pictureBox6.TabStop = false;
            this.pictureBox6.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panel_top_MouseDoubleClick);
            this.pictureBox6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel_top_MouseDown);
            this.pictureBox6.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel_top_MouseMove);
            this.pictureBox6.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel_top_MouseUp);
            // 
            // label_date
            // 
            this.label_date.AutoSize = true;
            this.label_date.BackColor = System.Drawing.Color.Transparent;
            this.label_date.Font = new System.Drawing.Font("나눔바른고딕", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_date.ForeColor = System.Drawing.Color.White;
            this.label_date.Location = new System.Drawing.Point(1478, 22);
            this.label_date.Name = "label_date";
            this.label_date.Size = new System.Drawing.Size(417, 37);
            this.label_date.TabIndex = 1;
            this.label_date.Text = "0000-00-00(0) 00:00:00";
            this.label_date.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panel_top_MouseDoubleClick);
            this.label_date.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel_top_MouseDown);
            this.label_date.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel_top_MouseMove);
            this.label_date.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel_top_MouseUp);
            // 
            // MainForm_Tank
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1920, 1080);
            this.Controls.Add(this.panel_bottom);
            this.Controls.Add(this.panel_top);
            this.Controls.Add(this.pCenter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "MainForm_Tank";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "탱크 모니터링";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_Tank_FormClosing);
            this.panel_bottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_back)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_report)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel_top.ResumeLayout(false);
            this.panel_top.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_setting)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_sound)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label_notice;
        private System.Windows.Forms.PictureBox pictureBox_report;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel_bottom;
        private System.Windows.Forms.PictureBox pictureBox_back;
        private System.Windows.Forms.Label label_date;
        private System.Windows.Forms.Panel panel_top;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.PictureBox pictureBox_setting;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.Panel pCenter;
        private System.Windows.Forms.PictureBox pictureBox_sound;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}

