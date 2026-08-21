namespace SDMS.PopupDialog
{
    partial class FormPSMSensorTrendData
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
            this.chart = new ChartDirector.WinChartViewer();
            this.pnBody = new System.Windows.Forms.Panel();
            this.btnRedo = new System.Windows.Forms.Button();
            this.btnUndo = new System.Windows.Forms.Button();
            this.pnLine = new System.Windows.Forms.Panel();
            this.lblDescription2 = new System.Windows.Forms.Label();
            this.lblSearchDate = new System.Windows.Forms.Label();
            this.lblSearchDateHeader = new System.Windows.Forms.Label();
            this.lblDescription1 = new System.Windows.Forms.Label();
            this.lblMaterialName = new System.Windows.Forms.Label();
            this.lblSensorName = new System.Windows.Forms.Label();
            this.lblMaterialNameHeader = new System.Windows.Forms.Label();
            this.lblSensorNameHeader = new System.Windows.Forms.Label();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            this.pnBody.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart
            // 
            this.chart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart.Location = new System.Drawing.Point(3, 128);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(1554, 706);
            this.chart.TabIndex = 9;
            this.chart.TabStop = false;
            // 
            // pnBody
            // 
            this.pnBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnBody.BackColor = System.Drawing.Color.White;
            this.pnBody.Controls.Add(this.btnRedo);
            this.pnBody.Controls.Add(this.btnUndo);
            this.pnBody.Controls.Add(this.pnLine);
            this.pnBody.Controls.Add(this.lblDescription2);
            this.pnBody.Controls.Add(this.lblSearchDate);
            this.pnBody.Controls.Add(this.lblSearchDateHeader);
            this.pnBody.Controls.Add(this.lblDescription1);
            this.pnBody.Controls.Add(this.lblMaterialName);
            this.pnBody.Controls.Add(this.lblSensorName);
            this.pnBody.Controls.Add(this.lblMaterialNameHeader);
            this.pnBody.Controls.Add(this.lblSensorNameHeader);
            this.pnBody.Controls.Add(this.chart);
            this.pnBody.Location = new System.Drawing.Point(12, 12);
            this.pnBody.Name = "pnBody";
            this.pnBody.Size = new System.Drawing.Size(1560, 837);
            this.pnBody.TabIndex = 11;
            // 
            // btnRedo
            // 
            this.btnRedo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRedo.Image = global::SDMS.Properties.Resources.다시실행_normal;
            this.btnRedo.Location = new System.Drawing.Point(1505, 106);
            this.btnRedo.Name = "btnRedo";
            this.btnRedo.Size = new System.Drawing.Size(32, 32);
            this.btnRedo.TabIndex = 35;
            this.btnRedo.UseVisualStyleBackColor = true;
            // 
            // btnUndo
            // 
            this.btnUndo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUndo.Image = global::SDMS.Properties.Resources.되돌리기_normal;
            this.btnUndo.Location = new System.Drawing.Point(1468, 106);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(32, 32);
            this.btnUndo.TabIndex = 34;
            this.btnUndo.UseVisualStyleBackColor = true;
            // 
            // pnLine
            // 
            this.pnLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnLine.BackColor = System.Drawing.Color.Transparent;
            this.pnLine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnLine.Location = new System.Drawing.Point(3, 88);
            this.pnLine.Name = "pnLine";
            this.pnLine.Size = new System.Drawing.Size(1554, 1);
            this.pnLine.TabIndex = 33;
            // 
            // lblDescription2
            // 
            this.lblDescription2.AutoSize = true;
            this.lblDescription2.Location = new System.Drawing.Point(21, 129);
            this.lblDescription2.Name = "lblDescription2";
            this.lblDescription2.Size = new System.Drawing.Size(333, 12);
            this.lblDescription2.TabIndex = 32;
            this.lblDescription2.Text = "※ 측정 데이터는 한달간 보관되며 이후에는 삭제처리합니다.";
            // 
            // lblSearchDate
            // 
            this.lblSearchDate.AutoSize = true;
            this.lblSearchDate.Location = new System.Drawing.Point(92, 17);
            this.lblSearchDate.Name = "lblSearchDate";
            this.lblSearchDate.Size = new System.Drawing.Size(501, 12);
            this.lblSearchDate.TabIndex = 31;
            this.lblSearchDate.Text = "[ 2017년 1월 1일 오전 01시 00분 00초 ] 부터 [ 2017년 12월 31일 오후 23시 59분 59초 ] 까지";
            // 
            // lblSearchDateHeader
            // 
            this.lblSearchDateHeader.AutoSize = true;
            this.lblSearchDateHeader.Location = new System.Drawing.Point(21, 17);
            this.lblSearchDateHeader.Name = "lblSearchDateHeader";
            this.lblSearchDateHeader.Size = new System.Drawing.Size(65, 12);
            this.lblSearchDateHeader.TabIndex = 30;
            this.lblSearchDateHeader.Text = "조회기간 : ";
            // 
            // lblDescription1
            // 
            this.lblDescription1.AutoSize = true;
            this.lblDescription1.Location = new System.Drawing.Point(21, 106);
            this.lblDescription1.Name = "lblDescription1";
            this.lblDescription1.Size = new System.Drawing.Size(231, 12);
            this.lblDescription1.TabIndex = 25;
            this.lblDescription1.Text = "※ 측정 데이터는 약 2초 마다 기록합니다.";
            // 
            // lblMaterialName
            // 
            this.lblMaterialName.AutoSize = true;
            this.lblMaterialName.Location = new System.Drawing.Point(92, 63);
            this.lblMaterialName.Name = "lblMaterialName";
            this.lblMaterialName.Size = new System.Drawing.Size(65, 12);
            this.lblMaterialName.TabIndex = 13;
            this.lblMaterialName.Text = "암모니아수";
            // 
            // lblSensorName
            // 
            this.lblSensorName.AutoSize = true;
            this.lblSensorName.Location = new System.Drawing.Point(92, 40);
            this.lblSensorName.Name = "lblSensorName";
            this.lblSensorName.Size = new System.Drawing.Size(267, 12);
            this.lblSensorName.TabIndex = 12;
            this.lblSensorName.Text = "#5,6호기 약품주입설비 암모니아 경가스 센서 - 1";
            // 
            // lblMaterialNameHeader
            // 
            this.lblMaterialNameHeader.AutoSize = true;
            this.lblMaterialNameHeader.Location = new System.Drawing.Point(21, 63);
            this.lblMaterialNameHeader.Name = "lblMaterialNameHeader";
            this.lblMaterialNameHeader.Size = new System.Drawing.Size(65, 12);
            this.lblMaterialNameHeader.TabIndex = 11;
            this.lblMaterialNameHeader.Text = "물질이름 : ";
            // 
            // lblSensorNameHeader
            // 
            this.lblSensorNameHeader.AutoSize = true;
            this.lblSensorNameHeader.Location = new System.Drawing.Point(21, 40);
            this.lblSensorNameHeader.Name = "lblSensorNameHeader";
            this.lblSensorNameHeader.Size = new System.Drawing.Size(65, 12);
            this.lblSensorNameHeader.TabIndex = 10;
            this.lblSensorNameHeader.Text = "센서이름 : ";
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Size = new System.Drawing.Size(908, 554);
            this.shapeContainer1.TabIndex = 5;
            this.shapeContainer1.TabStop = false;
            // 
            // FormPSMSensorTrendData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(1584, 861);
            this.Controls.Add(this.pnBody);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormPSMSensorTrendData";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "유해화학물질 측정 데이터 추이";
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            this.pnBody.ResumeLayout(false);
            this.pnBody.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ChartDirector.WinChartViewer chart;
        private System.Windows.Forms.Panel pnBody;
        private System.Windows.Forms.Label lblMaterialName;
        private System.Windows.Forms.Label lblSensorName;
        private System.Windows.Forms.Label lblMaterialNameHeader;
        private System.Windows.Forms.Label lblSensorNameHeader;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private System.Windows.Forms.Label lblDescription1;
        private System.Windows.Forms.Label lblSearchDate;
        private System.Windows.Forms.Label lblSearchDateHeader;
        private System.Windows.Forms.Label lblDescription2;
        private System.Windows.Forms.Panel pnLine;
        private System.Windows.Forms.Button btnRedo;
        private System.Windows.Forms.Button btnUndo;
    }
}