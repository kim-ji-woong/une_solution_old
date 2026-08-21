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
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnBody = new System.Windows.Forms.Panel();
            this.btnRedo = new UnE.GUI.ImageButton();
            this.btnUndo = new UnE.GUI.ImageButton();
            this.pnLine = new System.Windows.Forms.Panel();
            this.lblDescription2 = new System.Windows.Forms.Label();
            this.lblSearchDate = new System.Windows.Forms.Label();
            this.lblSearchDateHeader = new System.Windows.Forms.Label();
            this.lblDescription1 = new System.Windows.Forms.Label();
            this.lblMaterialName = new System.Windows.Forms.Label();
            this.lblSensorName = new System.Windows.Forms.Label();
            this.lblMaterialNameHeader = new System.Windows.Forms.Label();
            this.lblSensorNameHeader = new System.Windows.Forms.Label();
            this.chart = new ChartDirector.WinChartViewer();
            this.btnClose = new UnE.GUI.ImageButton();
            this.pnBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnRedo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnUndo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.SuspendLayout();
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
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            this.lblTitle.Font = new System.Drawing.Font(Program.prgFont, 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(1406, 62);
            this.lblTitle.TabIndex = 27;
            this.lblTitle.Text = "  유해화학물질 측정 데이터 추이";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnBody
            // 
            this.pnBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnBody.BackColor = System.Drawing.Color.Transparent;
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
            this.pnBody.Location = new System.Drawing.Point(9, 60);
            this.pnBody.Name = "pnBody";
            this.pnBody.Size = new System.Drawing.Size(1387, 894);
            this.pnBody.TabIndex = 11;
            // 
            // btnRedo
            // 
            this.btnRedo.ButtonText = "";
            this.btnRedo.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRedo.ImageClicked = global::SDMS.Properties.Resources.BtnRightArrow_Click;
            this.btnRedo.ImageDisabled = null;
            this.btnRedo.ImageMouseOver = global::SDMS.Properties.Resources.BtnRightArrow_Click;
            this.btnRedo.ImageNormal = global::SDMS.Properties.Resources.BtnRightArrow_Default;
            this.btnRedo.Location = new System.Drawing.Point(1357, 108);
            this.btnRedo.Name = "btnRedo";
            this.btnRedo.Owner = null;
            this.btnRedo.Size = new System.Drawing.Size(18, 30);
            this.btnRedo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnRedo.TabIndex = 72;
            this.btnRedo.TabStop = false;
            this.btnRedo.TextColor = System.Drawing.Color.Black;
            this.btnRedo.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRedo.ToolTipText = "";
            this.btnRedo.Click += new System.EventHandler(this.btnRedo_Click);
            // 
            // btnUndo
            // 
            this.btnUndo.ButtonText = "";
            this.btnUndo.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnUndo.ImageClicked = global::SDMS.Properties.Resources.BtnLeftArrow_Click;
            this.btnUndo.ImageDisabled = null;
            this.btnUndo.ImageMouseOver = global::SDMS.Properties.Resources.BtnLeftArrow_Click;
            this.btnUndo.ImageNormal = global::SDMS.Properties.Resources.BtnLeftArrow_Default;
            this.btnUndo.Location = new System.Drawing.Point(1317, 108);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Owner = null;
            this.btnUndo.Size = new System.Drawing.Size(16, 30);
            this.btnUndo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnUndo.TabIndex = 71;
            this.btnUndo.TabStop = false;
            this.btnUndo.TextColor = System.Drawing.Color.Black;
            this.btnUndo.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnUndo.ToolTipText = "";
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // pnLine
            // 
            this.pnLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnLine.BackColor = System.Drawing.Color.Transparent;
            this.pnLine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnLine.Location = new System.Drawing.Point(3, 88);
            this.pnLine.Name = "pnLine";
            this.pnLine.Size = new System.Drawing.Size(1381, 1);
            this.pnLine.TabIndex = 33;
            // 
            // lblDescription2
            // 
            this.lblDescription2.AutoSize = true;
            this.lblDescription2.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDescription2.ForeColor = System.Drawing.Color.White;
            this.lblDescription2.Location = new System.Drawing.Point(21, 129);
            this.lblDescription2.Name = "lblDescription2";
            this.lblDescription2.Size = new System.Drawing.Size(411, 18);
            this.lblDescription2.TabIndex = 32;
            this.lblDescription2.Text = "※ 측정 데이터는 한달간 보관되며 이후에는 삭제처리합니다.";
            // 
            // lblSearchDate
            // 
            this.lblSearchDate.AutoSize = true;
            this.lblSearchDate.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblSearchDate.ForeColor = System.Drawing.Color.White;
            this.lblSearchDate.Location = new System.Drawing.Point(92, 17);
            this.lblSearchDate.Name = "lblSearchDate";
            this.lblSearchDate.Size = new System.Drawing.Size(668, 18);
            this.lblSearchDate.TabIndex = 31;
            this.lblSearchDate.Text = "[ 2017년 1월 1일 오전 01시 00분 00초 ] 부터 [ 2017년 12월 31일 오후 23시 59분 59초 ] 까지";
            // 
            // lblSearchDateHeader
            // 
            this.lblSearchDateHeader.AutoSize = true;
            this.lblSearchDateHeader.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblSearchDateHeader.ForeColor = System.Drawing.Color.White;
            this.lblSearchDateHeader.Location = new System.Drawing.Point(21, 17);
            this.lblSearchDateHeader.Name = "lblSearchDateHeader";
            this.lblSearchDateHeader.Size = new System.Drawing.Size(81, 18);
            this.lblSearchDateHeader.TabIndex = 30;
            this.lblSearchDateHeader.Text = "조회기간 : ";
            // 
            // lblDescription1
            // 
            this.lblDescription1.AutoSize = true;
            this.lblDescription1.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDescription1.ForeColor = System.Drawing.Color.White;
            this.lblDescription1.Location = new System.Drawing.Point(21, 106);
            this.lblDescription1.Name = "lblDescription1";
            this.lblDescription1.Size = new System.Drawing.Size(286, 18);
            this.lblDescription1.TabIndex = 25;
            this.lblDescription1.Text = "※ 측정 데이터는 약 2초 마다 기록합니다.";
            // 
            // lblMaterialName
            // 
            this.lblMaterialName.AutoSize = true;
            this.lblMaterialName.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblMaterialName.ForeColor = System.Drawing.Color.White;
            this.lblMaterialName.Location = new System.Drawing.Point(92, 63);
            this.lblMaterialName.Name = "lblMaterialName";
            this.lblMaterialName.Size = new System.Drawing.Size(83, 18);
            this.lblMaterialName.TabIndex = 13;
            this.lblMaterialName.Text = "암모니아수";
            // 
            // lblSensorName
            // 
            this.lblSensorName.AutoSize = true;
            this.lblSensorName.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblSensorName.ForeColor = System.Drawing.Color.White;
            this.lblSensorName.Location = new System.Drawing.Point(92, 40);
            this.lblSensorName.Name = "lblSensorName";
            this.lblSensorName.Size = new System.Drawing.Size(338, 18);
            this.lblSensorName.TabIndex = 12;
            this.lblSensorName.Text = "#5,6호기 약품주입설비 암모니아 경가스 센서 - 1";
            // 
            // lblMaterialNameHeader
            // 
            this.lblMaterialNameHeader.AutoSize = true;
            this.lblMaterialNameHeader.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblMaterialNameHeader.ForeColor = System.Drawing.Color.White;
            this.lblMaterialNameHeader.Location = new System.Drawing.Point(21, 63);
            this.lblMaterialNameHeader.Name = "lblMaterialNameHeader";
            this.lblMaterialNameHeader.Size = new System.Drawing.Size(81, 18);
            this.lblMaterialNameHeader.TabIndex = 11;
            this.lblMaterialNameHeader.Text = "물질이름 : ";
            // 
            // lblSensorNameHeader
            // 
            this.lblSensorNameHeader.AutoSize = true;
            this.lblSensorNameHeader.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblSensorNameHeader.ForeColor = System.Drawing.Color.White;
            this.lblSensorNameHeader.Location = new System.Drawing.Point(21, 40);
            this.lblSensorNameHeader.Name = "lblSensorNameHeader";
            this.lblSensorNameHeader.Size = new System.Drawing.Size(81, 18);
            this.lblSensorNameHeader.TabIndex = 10;
            this.lblSensorNameHeader.Text = "센서이름 : ";
            // 
            // chart
            // 
            this.chart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart.Location = new System.Drawing.Point(3, 150);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(1381, 741);
            this.chart.TabIndex = 9;
            this.chart.TabStop = false;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            this.btnClose.ButtonText = "";
            this.btnClose.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ImageClicked = global::SDMS.Properties.Resources.Close_40_40_Click;
            this.btnClose.ImageDisabled = null;
            this.btnClose.ImageMouseOver = global::SDMS.Properties.Resources.Close_40_40_Click;
            this.btnClose.ImageNormal = global::SDMS.Properties.Resources.Close_40_40_Default;
            this.btnClose.Location = new System.Drawing.Point(1353, 10);
            this.btnClose.Name = "btnClose";
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(40, 40);
            this.btnClose.TabIndex = 28;
            this.btnClose.TabStop = false;
            this.btnClose.TextColor = System.Drawing.Color.Black;
            this.btnClose.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ToolTipText = "";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FormPSMSensorTrendData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.ClientSize = new System.Drawing.Size(1405, 966);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pnBody);
            this.Name = "FormPSMSensorTrendData";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "유해화학물질 측정 데이터 추이";
            this.pnBody.ResumeLayout(false);
            this.pnBody.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnRedo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnUndo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
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
        private System.Windows.Forms.Label lblTitle;
        private UnE.GUI.ImageButton btnClose;
        private UnE.GUI.ImageButton btnUndo;
        private UnE.GUI.ImageButton btnRedo;
    }
}