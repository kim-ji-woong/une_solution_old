namespace SDMS
{
    partial class NotOperationPage
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
			this.label2 = new System.Windows.Forms.Label();
			this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
			this.lineShape1 = new Microsoft.VisualBasic.PowerPacks.LineShape();
			this.label3 = new System.Windows.Forms.Label();
			this.winChartViewer1 = new ChartDirector.WinChartViewer();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.cboChart = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.lblMinDate = new System.Windows.Forms.Label();
			this.lblMaxDate = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.lblBuilding = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.winChartViewer1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.label2.Location = new System.Drawing.Point(23, 20);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(114, 24);
			this.label2.TabIndex = 1;
			this.label2.Text = "처리 이력";
			// 
			// shapeContainer1
			// 
			this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
			this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
			this.shapeContainer1.Name = "shapeContainer1";
			this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.lineShape1});
			this.shapeContainer1.Size = new System.Drawing.Size(908, 554);
			this.shapeContainer1.TabIndex = 2;
			this.shapeContainer1.TabStop = false;
			// 
			// lineShape1
			// 
			this.lineShape1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lineShape1.BorderWidth = 5;
			this.lineShape1.Name = "lineShape1";
			this.lineShape1.X1 = 28;
			this.lineShape1.X2 = 900;
			this.lineShape1.Y1 = 57;
			this.lineShape1.Y2 = 57;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(28, 73);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(417, 12);
			this.label3.TabIndex = 3;
			this.label3.Text = "화재 탐지 중 값들 중 센서 오류 및 특정 상황에 의한 오작동률을 표시합니다.";
			// 
			// winChartViewer1
			// 
			this.winChartViewer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.winChartViewer1.Location = new System.Drawing.Point(194, 101);
			this.winChartViewer1.Name = "winChartViewer1";
			this.winChartViewer1.Size = new System.Drawing.Size(593, 237);
			this.winChartViewer1.TabIndex = 9;
			this.winChartViewer1.TabStop = false;
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Location = new System.Drawing.Point(53, 413);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.ReadOnly = true;
			this.dataGridView1.RowHeadersVisible = false;
			this.dataGridView1.RowTemplate.Height = 23;
			this.dataGridView1.Size = new System.Drawing.Size(784, 116);
			this.dataGridView1.TabIndex = 10;
			// 
			// cboChart
			// 
			this.cboChart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboChart.FormattingEnabled = true;
			this.cboChart.Location = new System.Drawing.Point(53, 101);
			this.cboChart.Name = "cboChart";
			this.cboChart.Size = new System.Drawing.Size(121, 20);
			this.cboChart.TabIndex = 11;
			this.cboChart.SelectedIndexChanged += new System.EventHandler(this.cboChart_SelectedIndexChanged_1);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(201, 31);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(57, 12);
			this.label1.TabIndex = 13;
			this.label1.Text = "조회 기간";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.label4.Location = new System.Drawing.Point(254, 27);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(12, 19);
			this.label4.TabIndex = 14;
			this.label4.Text = "l";
			// 
			// lblMinDate
			// 
			this.lblMinDate.AutoSize = true;
			this.lblMinDate.Location = new System.Drawing.Point(265, 31);
			this.lblMinDate.Name = "lblMinDate";
			this.lblMinDate.Size = new System.Drawing.Size(51, 12);
			this.lblMinDate.TabIndex = 15;
			this.lblMinDate.Text = "MinDate";
			// 
			// lblMaxDate
			// 
			this.lblMaxDate.AutoSize = true;
			this.lblMaxDate.Location = new System.Drawing.Point(390, 31);
			this.lblMaxDate.Name = "lblMaxDate";
			this.lblMaxDate.Size = new System.Drawing.Size(55, 12);
			this.lblMaxDate.TabIndex = 16;
			this.lblMaxDate.Text = "MaxDate";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(558, 31);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(57, 12);
			this.label7.TabIndex = 17;
			this.label7.Text = "조회 범위";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.label8.Location = new System.Drawing.Point(612, 27);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(12, 19);
			this.label8.TabIndex = 18;
			this.label8.Text = "l";
			// 
			// lblBuilding
			// 
			this.lblBuilding.AutoSize = true;
			this.lblBuilding.Location = new System.Drawing.Point(625, 31);
			this.lblBuilding.Name = "lblBuilding";
			this.lblBuilding.Size = new System.Drawing.Size(57, 12);
			this.lblBuilding.TabIndex = 19;
			this.lblBuilding.Text = "모든 건물";
			// 
			// NotOperationPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(908, 554);
			this.Controls.Add(this.lblBuilding);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.lblMaxDate);
			this.Controls.Add(this.lblMinDate);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cboChart);
			this.Controls.Add(this.dataGridView1);
			this.Controls.Add(this.winChartViewer1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.shapeContainer1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "NotOperationPage";
			this.Text = "NotOperationPage";
			this.Load += new System.EventHandler(this.NotOperationPage_Load);
			this.Resize += new System.EventHandler(this.NotOperationPage_Resize);
			((System.ComponentModel.ISupportInitialize)(this.winChartViewer1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape1;
        private System.Windows.Forms.Label label3;
        private ChartDirector.WinChartViewer winChartViewer1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ComboBox cboChart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblMinDate;
        private System.Windows.Forms.Label lblMaxDate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblBuilding;

    }
}