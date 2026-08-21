namespace KpxPipeMonitoring.ChildForms
{
    partial class ChildDetailPipe
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_type = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.pictureBox_doubleRight = new System.Windows.Forms.PictureBox();
            this.pictureBox_right = new System.Windows.Forms.PictureBox();
            this.label_curPressure = new System.Windows.Forms.Label();
            this.pictureBox_left = new System.Windows.Forms.PictureBox();
            this.label_workTime = new System.Windows.Forms.Label();
            this.pictureBox_doubleLeft = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label_maxPage = new System.Windows.Forms.Label();
            this.label_avgFlow = new System.Windows.Forms.Label();
            this.label_avgPressure = new System.Windows.Forms.Label();
            this.pictureBox_close = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label_pipeName = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_doubleRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_right)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_left)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_doubleLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_close)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BackgroundImage = global::KpxPipeMonitoring.Properties.Resources.PipeDetail;
            this.panel1.Controls.Add(this.label_type);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.chart1);
            this.panel1.Controls.Add(this.pictureBox_doubleRight);
            this.panel1.Controls.Add(this.pictureBox_right);
            this.panel1.Controls.Add(this.label_curPressure);
            this.panel1.Controls.Add(this.pictureBox_left);
            this.panel1.Controls.Add(this.label_workTime);
            this.panel1.Controls.Add(this.pictureBox_doubleLeft);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label_maxPage);
            this.panel1.Controls.Add(this.label_avgFlow);
            this.panel1.Controls.Add(this.label_avgPressure);
            this.panel1.Controls.Add(this.pictureBox_close);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.comboBox1);
            this.panel1.Controls.Add(this.label_pipeName);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1594, 603);
            this.panel1.TabIndex = 35;
            // 
            // label_type
            // 
            this.label_type.BackColor = System.Drawing.Color.Transparent;
            this.label_type.Font = new System.Drawing.Font("나눔바른고딕", 19F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_type.ForeColor = System.Drawing.Color.White;
            this.label_type.Location = new System.Drawing.Point(821, 18);
            this.label_type.Name = "label_type";
            this.label_type.Size = new System.Drawing.Size(69, 44);
            this.label_type.TabIndex = 36;
            this.label_type.Text = "C/8";
            this.label_type.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox1.Location = new System.Drawing.Point(869, 62);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(44, 35);
            this.textBox1.TabIndex = 35;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox1.Visible = false;
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(30, 105);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(1017, 455);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // pictureBox_doubleRight
            // 
            this.pictureBox_doubleRight.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_doubleRight.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox_doubleRight.Image = global::KpxPipeMonitoring.Properties.Resources.DoubleRight;
            this.pictureBox_doubleRight.Location = new System.Drawing.Point(1013, 67);
            this.pictureBox_doubleRight.Name = "pictureBox_doubleRight";
            this.pictureBox_doubleRight.Size = new System.Drawing.Size(34, 29);
            this.pictureBox_doubleRight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_doubleRight.TabIndex = 34;
            this.pictureBox_doubleRight.TabStop = false;
            this.pictureBox_doubleRight.Visible = false;
            this.pictureBox_doubleRight.Click += new System.EventHandler(this.pictureBox_doubleRight_Click);
            // 
            // pictureBox_right
            // 
            this.pictureBox_right.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_right.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox_right.Image = global::KpxPipeMonitoring.Properties.Resources.Right;
            this.pictureBox_right.Location = new System.Drawing.Point(973, 67);
            this.pictureBox_right.Name = "pictureBox_right";
            this.pictureBox_right.Size = new System.Drawing.Size(34, 29);
            this.pictureBox_right.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_right.TabIndex = 33;
            this.pictureBox_right.TabStop = false;
            this.pictureBox_right.Visible = false;
            this.pictureBox_right.Click += new System.EventHandler(this.pictureBox_right_Click);
            // 
            // label_curPressure
            // 
            this.label_curPressure.BackColor = System.Drawing.Color.Transparent;
            this.label_curPressure.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_curPressure.ForeColor = System.Drawing.Color.White;
            this.label_curPressure.Location = new System.Drawing.Point(1321, 101);
            this.label_curPressure.Name = "label_curPressure";
            this.label_curPressure.Size = new System.Drawing.Size(237, 50);
            this.label_curPressure.TabIndex = 6;
            this.label_curPressure.Text = "대기중";
            this.label_curPressure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox_left
            // 
            this.pictureBox_left.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_left.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox_left.Image = global::KpxPipeMonitoring.Properties.Resources.Left;
            this.pictureBox_left.Location = new System.Drawing.Point(829, 65);
            this.pictureBox_left.Name = "pictureBox_left";
            this.pictureBox_left.Size = new System.Drawing.Size(34, 29);
            this.pictureBox_left.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_left.TabIndex = 32;
            this.pictureBox_left.TabStop = false;
            this.pictureBox_left.Visible = false;
            this.pictureBox_left.Click += new System.EventHandler(this.pictureBox_left_Click);
            // 
            // label_workTime
            // 
            this.label_workTime.BackColor = System.Drawing.Color.Transparent;
            this.label_workTime.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_workTime.ForeColor = System.Drawing.Color.White;
            this.label_workTime.Location = new System.Drawing.Point(1321, 178);
            this.label_workTime.Name = "label_workTime";
            this.label_workTime.Size = new System.Drawing.Size(237, 50);
            this.label_workTime.TabIndex = 5;
            this.label_workTime.Text = "-";
            this.label_workTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox_doubleLeft
            // 
            this.pictureBox_doubleLeft.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_doubleLeft.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox_doubleLeft.Image = global::KpxPipeMonitoring.Properties.Resources.DoubleLeft;
            this.pictureBox_doubleLeft.Location = new System.Drawing.Point(789, 65);
            this.pictureBox_doubleLeft.Name = "pictureBox_doubleLeft";
            this.pictureBox_doubleLeft.Size = new System.Drawing.Size(34, 29);
            this.pictureBox_doubleLeft.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_doubleLeft.TabIndex = 31;
            this.pictureBox_doubleLeft.TabStop = false;
            this.pictureBox_doubleLeft.Visible = false;
            this.pictureBox_doubleLeft.Click += new System.EventHandler(this.pictureBox_doubleLeft_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(1321, 494);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(237, 50);
            this.label1.TabIndex = 3;
            this.label1.Text = "-";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_maxPage
            // 
            this.label_maxPage.AutoSize = true;
            this.label_maxPage.BackColor = System.Drawing.Color.Transparent;
            this.label_maxPage.Font = new System.Drawing.Font("나눔바른고딕", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_maxPage.ForeColor = System.Drawing.Color.White;
            this.label_maxPage.Location = new System.Drawing.Point(919, 67);
            this.label_maxPage.Name = "label_maxPage";
            this.label_maxPage.Size = new System.Drawing.Size(51, 27);
            this.label_maxPage.TabIndex = 30;
            this.label_maxPage.Text = "/10";
            this.label_maxPage.Visible = false;
            // 
            // label_avgFlow
            // 
            this.label_avgFlow.BackColor = System.Drawing.Color.Transparent;
            this.label_avgFlow.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_avgFlow.ForeColor = System.Drawing.Color.White;
            this.label_avgFlow.Location = new System.Drawing.Point(1321, 261);
            this.label_avgFlow.Name = "label_avgFlow";
            this.label_avgFlow.Size = new System.Drawing.Size(237, 50);
            this.label_avgFlow.TabIndex = 4;
            this.label_avgFlow.Text = "-";
            this.label_avgFlow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_avgPressure
            // 
            this.label_avgPressure.BackColor = System.Drawing.Color.Transparent;
            this.label_avgPressure.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_avgPressure.ForeColor = System.Drawing.Color.White;
            this.label_avgPressure.Location = new System.Drawing.Point(1321, 417);
            this.label_avgPressure.Name = "label_avgPressure";
            this.label_avgPressure.Size = new System.Drawing.Size(237, 50);
            this.label_avgPressure.TabIndex = 2;
            this.label_avgPressure.Text = "-";
            this.label_avgPressure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox_close
            // 
            this.pictureBox_close.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_close.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox_close.Image = global::KpxPipeMonitoring.Properties.Resources.Close;
            this.pictureBox_close.Location = new System.Drawing.Point(1541, 21);
            this.pictureBox_close.Name = "pictureBox_close";
            this.pictureBox_close.Size = new System.Drawing.Size(22, 22);
            this.pictureBox_close.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_close.TabIndex = 29;
            this.pictureBox_close.TabStop = false;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(1321, 339);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(237, 50);
            this.label2.TabIndex = 1;
            this.label2.Text = "-";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(30, 64);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(169, 37);
            this.comboBox1.TabIndex = 9;
            // 
            // label_pipeName
            // 
            this.label_pipeName.BackColor = System.Drawing.Color.Transparent;
            this.label_pipeName.Font = new System.Drawing.Font("나눔바른고딕", 24.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_pipeName.ForeColor = System.Drawing.Color.White;
            this.label_pipeName.Location = new System.Drawing.Point(32, 13);
            this.label_pipeName.Name = "label_pipeName";
            this.label_pipeName.Size = new System.Drawing.Size(791, 52);
            this.label_pipeName.TabIndex = 1;
            this.label_pipeName.Text = "Title";
            this.label_pipeName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ChildDetailPipe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1594, 603);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ChildDetailPipe";
            this.Text = "ChildDetailPipe";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_doubleRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_right)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_left)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_doubleLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_close)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label_pipeName;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label_avgPressure;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_curPressure;
        private System.Windows.Forms.Label label_avgFlow;
        private System.Windows.Forms.Label label_workTime;
        public System.Windows.Forms.PictureBox pictureBox_close;
        private System.Windows.Forms.Label label_maxPage;
        private System.Windows.Forms.PictureBox pictureBox_doubleLeft;
        private System.Windows.Forms.PictureBox pictureBox_left;
        private System.Windows.Forms.PictureBox pictureBox_right;
        private System.Windows.Forms.PictureBox pictureBox_doubleRight;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox1;
        public System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Label label_type;
    }
}