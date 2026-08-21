namespace KpxPipeMonitoring.ChildForms
{
    partial class ChildPipe
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea9 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series9 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea10 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series10 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart_pressure = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblFlow = new System.Windows.Forms.Label();
            this.lblPressure = new System.Windows.Forms.Label();
            this.pictureBox_rangeRefresh = new System.Windows.Forms.PictureBox();
            this.label_workTime = new System.Windows.Forms.Label();
            this.label_flowRange = new System.Windows.Forms.Label();
            this.label_pressureRange = new System.Windows.Forms.Label();
            this.pictureBox_clearAlarm = new System.Windows.Forms.PictureBox();
            this.label_tankName = new System.Windows.Forms.Label();
            this.label_wait = new System.Windows.Forms.Label();
            this.label_pipeName = new System.Windows.Forms.Label();
            this.pictureBox_BeginWork = new System.Windows.Forms.PictureBox();
            this.pictureBox_EndWork = new System.Windows.Forms.PictureBox();
            this.pictureBox_title = new System.Windows.Forms.PictureBox();
            this.chart_flow = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.lblMemo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart_pressure)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_rangeRefresh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_clearAlarm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_BeginWork)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_EndWork)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_title)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart_flow)).BeginInit();
            this.SuspendLayout();
            // 
            // chart_pressure
            // 
            this.chart_pressure.BackColor = System.Drawing.Color.Transparent;
            this.chart_pressure.BackImageTransparentColor = System.Drawing.Color.Transparent;
            this.chart_pressure.BackSecondaryColor = System.Drawing.Color.Transparent;
            this.chart_pressure.BorderlineColor = System.Drawing.Color.Transparent;
            this.chart_pressure.BorderSkin.BackColor = System.Drawing.Color.Transparent;
            chartArea9.Name = "ChartArea1";
            this.chart_pressure.ChartAreas.Add(chartArea9);
            this.chart_pressure.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chart_pressure.Location = new System.Drawing.Point(5, 52);
            this.chart_pressure.Name = "chart_pressure";
            series9.ChartArea = "ChartArea1";
            series9.Name = "Series1";
            this.chart_pressure.Series.Add(series9);
            this.chart_pressure.Size = new System.Drawing.Size(620, 80);
            this.chart_pressure.TabIndex = 3;
            this.chart_pressure.Text = "chart1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblMemo);
            this.panel1.Controls.Add(this.lblFlow);
            this.panel1.Controls.Add(this.lblPressure);
            this.panel1.Controls.Add(this.pictureBox_rangeRefresh);
            this.panel1.Controls.Add(this.label_workTime);
            this.panel1.Controls.Add(this.label_flowRange);
            this.panel1.Controls.Add(this.label_pressureRange);
            this.panel1.Controls.Add(this.pictureBox_clearAlarm);
            this.panel1.Controls.Add(this.label_tankName);
            this.panel1.Controls.Add(this.label_wait);
            this.panel1.Controls.Add(this.label_pipeName);
            this.panel1.Controls.Add(this.pictureBox_BeginWork);
            this.panel1.Controls.Add(this.pictureBox_EndWork);
            this.panel1.Controls.Add(this.pictureBox_title);
            this.panel1.Controls.Add(this.chart_pressure);
            this.panel1.Controls.Add(this.chart_flow);
            this.panel1.Location = new System.Drawing.Point(5, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(630, 218);
            this.panel1.TabIndex = 4;
            // 
            // lblFlow
            // 
            this.lblFlow.AutoSize = true;
            this.lblFlow.Font = new System.Drawing.Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblFlow.Location = new System.Drawing.Point(550, 131);
            this.lblFlow.Name = "lblFlow";
            this.lblFlow.Size = new System.Drawing.Size(70, 22);
            this.lblFlow.TabIndex = 100;
            this.lblFlow.Text = "label2";
            this.lblFlow.Visible = false;
            // 
            // lblPressure
            // 
            this.lblPressure.AutoSize = true;
            this.lblPressure.Font = new System.Drawing.Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblPressure.Location = new System.Drawing.Point(502, 131);
            this.lblPressure.Name = "lblPressure";
            this.lblPressure.Size = new System.Drawing.Size(70, 22);
            this.lblPressure.TabIndex = 99;
            this.lblPressure.Text = "label1";
            this.lblPressure.Visible = false;
            // 
            // pictureBox_rangeRefresh
            // 
            this.pictureBox_rangeRefresh.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_rangeRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox_rangeRefresh.Image = global::KpxPipeMonitoring.Properties.Resources.RangeRefresh_Normal;
            this.pictureBox_rangeRefresh.Location = new System.Drawing.Point(329, 12);
            this.pictureBox_rangeRefresh.Name = "pictureBox_rangeRefresh";
            this.pictureBox_rangeRefresh.Size = new System.Drawing.Size(51, 33);
            this.pictureBox_rangeRefresh.TabIndex = 98;
            this.pictureBox_rangeRefresh.TabStop = false;
            this.pictureBox_rangeRefresh.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_rangeRefresh_MouseClick);
            // 
            // label_workTime
            // 
            this.label_workTime.AutoSize = true;
            this.label_workTime.BackColor = System.Drawing.Color.Transparent;
            this.label_workTime.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_workTime.ForeColor = System.Drawing.Color.White;
            this.label_workTime.Location = new System.Drawing.Point(407, 82);
            this.label_workTime.Name = "label_workTime";
            this.label_workTime.Size = new System.Drawing.Size(81, 19);
            this.label_workTime.TabIndex = 36;
            this.label_workTime.Text = "00:00:00";
            this.label_workTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_flowRange
            // 
            this.label_flowRange.BackColor = System.Drawing.Color.Transparent;
            this.label_flowRange.Font = new System.Drawing.Font("나눔바른고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_flowRange.ForeColor = System.Drawing.Color.Black;
            this.label_flowRange.Location = new System.Drawing.Point(151, 131);
            this.label_flowRange.Name = "label_flowRange";
            this.label_flowRange.Size = new System.Drawing.Size(375, 14);
            this.label_flowRange.TabIndex = 35;
            this.label_flowRange.Text = "유량 (kl/h)";
            this.label_flowRange.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_pressureRange
            // 
            this.label_pressureRange.BackColor = System.Drawing.Color.Transparent;
            this.label_pressureRange.Font = new System.Drawing.Font("나눔바른고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_pressureRange.ForeColor = System.Drawing.Color.Black;
            this.label_pressureRange.Location = new System.Drawing.Point(151, 50);
            this.label_pressureRange.Name = "label_pressureRange";
            this.label_pressureRange.Size = new System.Drawing.Size(375, 14);
            this.label_pressureRange.TabIndex = 34;
            this.label_pressureRange.Text = "압력 (kg/cm²) | 범위:3.8~2.4(20%) | 설정:1시 31분 | 유지:180분";
            this.label_pressureRange.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox_clearAlarm
            // 
            this.pictureBox_clearAlarm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox_clearAlarm.Image = global::KpxPipeMonitoring.Properties.Resources.AlarmClear;
            this.pictureBox_clearAlarm.Location = new System.Drawing.Point(386, 10);
            this.pictureBox_clearAlarm.Name = "pictureBox_clearAlarm";
            this.pictureBox_clearAlarm.Size = new System.Drawing.Size(88, 36);
            this.pictureBox_clearAlarm.TabIndex = 33;
            this.pictureBox_clearAlarm.TabStop = false;
            this.pictureBox_clearAlarm.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_clearAlarm_MouseClick);
            // 
            // label_tankName
            // 
            this.label_tankName.BackColor = System.Drawing.Color.Transparent;
            this.label_tankName.Font = new System.Drawing.Font("나눔바른고딕", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_tankName.ForeColor = System.Drawing.Color.Black;
            this.label_tankName.Location = new System.Drawing.Point(232, 27);
            this.label_tankName.Name = "label_tankName";
            this.label_tankName.Size = new System.Drawing.Size(83, 22);
            this.label_tankName.TabIndex = 19;
            this.label_tankName.Text = "TK-201";
            this.label_tankName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_wait
            // 
            this.label_wait.AutoSize = true;
            this.label_wait.BackColor = System.Drawing.Color.Transparent;
            this.label_wait.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_wait.ForeColor = System.Drawing.Color.White;
            this.label_wait.Location = new System.Drawing.Point(332, 29);
            this.label_wait.Name = "label_wait";
            this.label_wait.Size = new System.Drawing.Size(54, 19);
            this.label_wait.TabIndex = 17;
            this.label_wait.Text = "대기중";
            this.label_wait.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_pipeName
            // 
            this.label_pipeName.BackColor = System.Drawing.Color.Transparent;
            this.label_pipeName.Font = new System.Drawing.Font("나눔바른고딕", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_pipeName.ForeColor = System.Drawing.Color.Black;
            this.label_pipeName.Location = new System.Drawing.Point(52, 27);
            this.label_pipeName.Name = "label_pipeName";
            this.label_pipeName.Size = new System.Drawing.Size(134, 22);
            this.label_pipeName.TabIndex = 16;
            this.label_pipeName.Text = "PT-1002C/6";
            this.label_pipeName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox_BeginWork
            // 
            this.pictureBox_BeginWork.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_BeginWork.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox_BeginWork.Image = global::KpxPipeMonitoring.Properties.Resources.BeginWork_Click;
            this.pictureBox_BeginWork.Location = new System.Drawing.Point(492, 9);
            this.pictureBox_BeginWork.Name = "pictureBox_BeginWork";
            this.pictureBox_BeginWork.Size = new System.Drawing.Size(88, 36);
            this.pictureBox_BeginWork.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_BeginWork.TabIndex = 8;
            this.pictureBox_BeginWork.TabStop = false;
            this.pictureBox_BeginWork.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_BeginWork_MouseClick);
            // 
            // pictureBox_EndWork
            // 
            this.pictureBox_EndWork.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_EndWork.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox_EndWork.Image = global::KpxPipeMonitoring.Properties.Resources.EndWork_Click;
            this.pictureBox_EndWork.Location = new System.Drawing.Point(506, 13);
            this.pictureBox_EndWork.Name = "pictureBox_EndWork";
            this.pictureBox_EndWork.Size = new System.Drawing.Size(88, 36);
            this.pictureBox_EndWork.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_EndWork.TabIndex = 7;
            this.pictureBox_EndWork.TabStop = false;
            this.pictureBox_EndWork.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_EndWork_MouseClick);
            // 
            // pictureBox_title
            // 
            this.pictureBox_title.Image = global::KpxPipeMonitoring.Properties.Resources.PT_Orange;
            this.pictureBox_title.Location = new System.Drawing.Point(26, 4);
            this.pictureBox_title.Name = "pictureBox_title";
            this.pictureBox_title.Size = new System.Drawing.Size(578, 52);
            this.pictureBox_title.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_title.TabIndex = 5;
            this.pictureBox_title.TabStop = false;
            // 
            // chart_flow
            // 
            this.chart_flow.BackColor = System.Drawing.Color.Transparent;
            this.chart_flow.BackImageTransparentColor = System.Drawing.Color.Transparent;
            this.chart_flow.BackSecondaryColor = System.Drawing.Color.Transparent;
            this.chart_flow.BorderlineColor = System.Drawing.Color.Transparent;
            this.chart_flow.BorderSkin.BackColor = System.Drawing.Color.Transparent;
            chartArea10.Name = "ChartArea1";
            this.chart_flow.ChartAreas.Add(chartArea10);
            this.chart_flow.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chart_flow.Location = new System.Drawing.Point(5, 140);
            this.chart_flow.Name = "chart_flow";
            series10.ChartArea = "ChartArea1";
            series10.Name = "Series1";
            this.chart_flow.Series.Add(series10);
            this.chart_flow.Size = new System.Drawing.Size(620, 80);
            this.chart_flow.TabIndex = 18;
            this.chart_flow.Text = "chart2";
            // 
            // lblMemo
            // 
            this.lblMemo.AutoSize = true;
            this.lblMemo.Font = new System.Drawing.Font("나눔바른고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblMemo.ForeColor = System.Drawing.Color.Red;
            this.lblMemo.Location = new System.Drawing.Point(11, 51);
            this.lblMemo.Name = "lblMemo";
            this.lblMemo.Size = new System.Drawing.Size(87, 14);
            this.lblMemo.TabIndex = 101;
            this.lblMemo.Text = "현대EP 이송중";
            // 
            // ChildPipe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(640, 226);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ChildPipe";
            this.Text = "ChildPipeVertical";
            this.TransparencyKey = System.Drawing.Color.Transparent;
            ((System.ComponentModel.ISupportInitialize)(this.chart_pressure)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_rangeRefresh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_clearAlarm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_BeginWork)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_EndWork)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_title)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart_flow)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.DataVisualization.Charting.Chart chart_pressure;
        public System.Windows.Forms.PictureBox pictureBox_title;
        public System.Windows.Forms.PictureBox pictureBox_EndWork;
        public System.Windows.Forms.PictureBox pictureBox_BeginWork;
        public System.Windows.Forms.Label label_pipeName;
        public System.Windows.Forms.Label label_wait;
        public System.Windows.Forms.DataVisualization.Charting.Chart chart_flow;
        public System.Windows.Forms.Label label_tankName;
        private System.Windows.Forms.PictureBox pictureBox_clearAlarm;
        public System.Windows.Forms.Label label_flowRange;
        public System.Windows.Forms.Label label_pressureRange;
        public System.Windows.Forms.Label label_workTime;
        private System.Windows.Forms.PictureBox pictureBox_rangeRefresh;
        public System.Windows.Forms.Label lblFlow;
        public System.Windows.Forms.Label lblPressure;
        public System.Windows.Forms.Label lblMemo;
    }
}