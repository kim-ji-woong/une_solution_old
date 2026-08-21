namespace SDMS.PopupDialog
{
    partial class FormPSMSensorData
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
            this.pnHeader = new System.Windows.Forms.Panel();
            this.TimePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.cmbSensor = new System.Windows.Forms.ComboBox();
            this.TimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.lblSensor = new System.Windows.Forms.Label();
            this.cmbSensorLocation = new System.Windows.Forms.ComboBox();
            this.lblSensorLocation = new System.Windows.Forms.Label();
            this.lblDateAnd = new System.Windows.Forms.Label();
            this.btnDateEnd = new System.Windows.Forms.Button();
            this.btnDateStart = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lblDatePreiod = new System.Windows.Forms.Label();
            this.cmbDateFix = new System.Windows.Forms.ComboBox();
            this.pnBody = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblDescription2 = new System.Windows.Forms.Label();
            this.lblSearchDate = new System.Windows.Forms.Label();
            this.lblSearchDateHeader = new System.Windows.Forms.Label();
            this.btnPreviousEnd = new System.Windows.Forms.Button();
            this.btnNextEnd = new System.Windows.Forms.Button();
            this.btnNextTen = new System.Windows.Forms.Button();
            this.btnPreviousTen = new System.Windows.Forms.Button();
            this.lblDescription1 = new System.Windows.Forms.Label();
            this.cmbPageIndex = new System.Windows.Forms.ComboBox();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.lblTotalPage = new System.Windows.Forms.Label();
            this.lblMaterialName = new System.Windows.Forms.Label();
            this.lblSensorName = new System.Windows.Forms.Label();
            this.lblMaterialNameHeader = new System.Windows.Forms.Label();
            this.lblSensorNameHeader = new System.Windows.Forms.Label();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.DatePickerStart = new System.Windows.Forms.DateTimePicker();
            this.DatePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            this.pnHeader.SuspendLayout();
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
            this.chart.Size = new System.Drawing.Size(1154, 786);
            this.chart.TabIndex = 9;
            this.chart.TabStop = false;
            // 
            // pnHeader
            // 
            this.pnHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnHeader.BackColor = System.Drawing.Color.White;
            this.pnHeader.Controls.Add(this.TimePickerEnd);
            this.pnHeader.Controls.Add(this.cmbSensor);
            this.pnHeader.Controls.Add(this.TimePickerStart);
            this.pnHeader.Controls.Add(this.lblSensor);
            this.pnHeader.Controls.Add(this.cmbSensorLocation);
            this.pnHeader.Controls.Add(this.lblSensorLocation);
            this.pnHeader.Controls.Add(this.lblDateAnd);
            this.pnHeader.Controls.Add(this.btnDateEnd);
            this.pnHeader.Controls.Add(this.btnDateStart);
            this.pnHeader.Controls.Add(this.btnSearch);
            this.pnHeader.Controls.Add(this.lblDatePreiod);
            this.pnHeader.Controls.Add(this.cmbDateFix);
            this.pnHeader.Location = new System.Drawing.Point(11, 73);
            this.pnHeader.Name = "pnHeader";
            this.pnHeader.Size = new System.Drawing.Size(1160, 75);
            this.pnHeader.TabIndex = 10;
            // 
            // TimePickerEnd
            // 
            this.TimePickerEnd.CustomFormat = "HH:mm:ss";
            this.TimePickerEnd.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.TimePickerEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.TimePickerEnd.Location = new System.Drawing.Point(284, 37);
            this.TimePickerEnd.Name = "TimePickerEnd";
            this.TimePickerEnd.ShowUpDown = true;
            this.TimePickerEnd.Size = new System.Drawing.Size(70, 27);
            this.TimePickerEnd.TabIndex = 23;
            // 
            // cmbSensor
            // 
            this.cmbSensor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSensor.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbSensor.Location = new System.Drawing.Point(791, 37);
            this.cmbSensor.Name = "cmbSensor";
            this.cmbSensor.Size = new System.Drawing.Size(260, 26);
            this.cmbSensor.TabIndex = 15;
            // 
            // TimePickerStart
            // 
            this.TimePickerStart.CustomFormat = "HH:mm:ss";
            this.TimePickerStart.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.TimePickerStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.TimePickerStart.Location = new System.Drawing.Point(106, 37);
            this.TimePickerStart.Name = "TimePickerStart";
            this.TimePickerStart.ShowUpDown = true;
            this.TimePickerStart.Size = new System.Drawing.Size(70, 27);
            this.TimePickerStart.TabIndex = 22;
            // 
            // lblSensor
            // 
            this.lblSensor.AutoSize = true;
            this.lblSensor.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblSensor.Location = new System.Drawing.Point(789, 17);
            this.lblSensor.Name = "lblSensor";
            this.lblSensor.Size = new System.Drawing.Size(38, 18);
            this.lblSensor.TabIndex = 14;
            this.lblSensor.Text = "센서";
            // 
            // cmbSensorLocation
            // 
            this.cmbSensorLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSensorLocation.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbSensorLocation.Location = new System.Drawing.Point(534, 37);
            this.cmbSensorLocation.Name = "cmbSensorLocation";
            this.cmbSensorLocation.Size = new System.Drawing.Size(220, 26);
            this.cmbSensorLocation.TabIndex = 13;
            // 
            // lblSensorLocation
            // 
            this.lblSensorLocation.AutoSize = true;
            this.lblSensorLocation.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblSensorLocation.Location = new System.Drawing.Point(532, 17);
            this.lblSensorLocation.Name = "lblSensorLocation";
            this.lblSensorLocation.Size = new System.Drawing.Size(53, 18);
            this.lblSensorLocation.TabIndex = 12;
            this.lblSensorLocation.Text = "시설명";
            // 
            // lblDateAnd
            // 
            this.lblDateAnd.AutoSize = true;
            this.lblDateAnd.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDateAnd.Location = new System.Drawing.Point(182, 42);
            this.lblDateAnd.Name = "lblDateAnd";
            this.lblDateAnd.Size = new System.Drawing.Size(18, 18);
            this.lblDateAnd.TabIndex = 11;
            this.lblDateAnd.Text = "~";
            // 
            // btnDateEnd
            // 
            this.btnDateEnd.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnDateEnd.Location = new System.Drawing.Point(201, 36);
            this.btnDateEnd.Name = "btnDateEnd";
            this.btnDateEnd.Size = new System.Drawing.Size(80, 22);
            this.btnDateEnd.TabIndex = 10;
            this.btnDateEnd.Text = "9999-12-31";
            this.btnDateEnd.UseVisualStyleBackColor = true;
            // 
            // btnDateStart
            // 
            this.btnDateStart.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnDateStart.Location = new System.Drawing.Point(23, 36);
            this.btnDateStart.Name = "btnDateStart";
            this.btnDateStart.Size = new System.Drawing.Size(80, 22);
            this.btnDateStart.TabIndex = 9;
            this.btnDateStart.Text = "0000-00-00";
            this.btnDateStart.UseVisualStyleBackColor = true;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(1085, 17);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(60, 43);
            this.btnSearch.TabIndex = 8;
            this.btnSearch.Text = "조회";
            this.btnSearch.UseVisualStyleBackColor = true;
            // 
            // lblDatePreiod
            // 
            this.lblDatePreiod.AutoSize = true;
            this.lblDatePreiod.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDatePreiod.Location = new System.Drawing.Point(21, 17);
            this.lblDatePreiod.Name = "lblDatePreiod";
            this.lblDatePreiod.Size = new System.Drawing.Size(68, 18);
            this.lblDatePreiod.TabIndex = 2;
            this.lblDatePreiod.Text = "조회기간";
            // 
            // cmbDateFix
            // 
            this.cmbDateFix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDateFix.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbDateFix.Location = new System.Drawing.Point(360, 37);
            this.cmbDateFix.Name = "cmbDateFix";
            this.cmbDateFix.Size = new System.Drawing.Size(115, 26);
            this.cmbDateFix.TabIndex = 0;
            // 
            // pnBody
            // 
            this.pnBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnBody.BackColor = System.Drawing.Color.White;
            this.pnBody.Controls.Add(this.panel1);
            this.pnBody.Controls.Add(this.lblDescription2);
            this.pnBody.Controls.Add(this.lblSearchDate);
            this.pnBody.Controls.Add(this.lblSearchDateHeader);
            this.pnBody.Controls.Add(this.btnPreviousEnd);
            this.pnBody.Controls.Add(this.btnNextEnd);
            this.pnBody.Controls.Add(this.btnNextTen);
            this.pnBody.Controls.Add(this.btnPreviousTen);
            this.pnBody.Controls.Add(this.lblDescription1);
            this.pnBody.Controls.Add(this.cmbPageIndex);
            this.pnBody.Controls.Add(this.btnNext);
            this.pnBody.Controls.Add(this.btnPrevious);
            this.pnBody.Controls.Add(this.lblTotalPage);
            this.pnBody.Controls.Add(this.lblMaterialName);
            this.pnBody.Controls.Add(this.lblSensorName);
            this.pnBody.Controls.Add(this.lblMaterialNameHeader);
            this.pnBody.Controls.Add(this.lblSensorNameHeader);
            this.pnBody.Controls.Add(this.chart);
            this.pnBody.Location = new System.Drawing.Point(11, 154);
            this.pnBody.Name = "pnBody";
            this.pnBody.Size = new System.Drawing.Size(1160, 917);
            this.pnBody.TabIndex = 11;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(3, 88);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1154, 1);
            this.panel1.TabIndex = 33;
            // 
            // lblDescription2
            // 
            this.lblDescription2.AutoSize = true;
            this.lblDescription2.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
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
            this.lblSearchDate.Location = new System.Drawing.Point(92, 17);
            this.lblSearchDate.Name = "lblSearchDate";
            this.lblSearchDate.Size = new System.Drawing.Size(668, 18);
            this.lblSearchDate.TabIndex = 31;
            this.lblSearchDate.Text = "[ 2016년 1월 1일 오전 01시 00분 00초 ] 부터 [ 2016년 12월 31일 오후 23시 59분 59초 ] 까지";
            // 
            // lblSearchDateHeader
            // 
            this.lblSearchDateHeader.AutoSize = true;
            this.lblSearchDateHeader.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblSearchDateHeader.Location = new System.Drawing.Point(21, 17);
            this.lblSearchDateHeader.Name = "lblSearchDateHeader";
            this.lblSearchDateHeader.Size = new System.Drawing.Size(81, 18);
            this.lblSearchDateHeader.TabIndex = 30;
            this.lblSearchDateHeader.Text = "조회기간 : ";
            // 
            // btnPreviousEnd
            // 
            this.btnPreviousEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPreviousEnd.BackgroundImage = global::SDMS.Properties.Resources.btn_previous_end;
            this.btnPreviousEnd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnPreviousEnd.Location = new System.Drawing.Point(880, 123);
            this.btnPreviousEnd.Name = "btnPreviousEnd";
            this.btnPreviousEnd.Size = new System.Drawing.Size(23, 22);
            this.btnPreviousEnd.TabIndex = 29;
            this.btnPreviousEnd.UseVisualStyleBackColor = true;
            // 
            // btnNextEnd
            // 
            this.btnNextEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNextEnd.BackgroundImage = global::SDMS.Properties.Resources.btn_next_end;
            this.btnNextEnd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnNextEnd.Location = new System.Drawing.Point(1127, 123);
            this.btnNextEnd.Name = "btnNextEnd";
            this.btnNextEnd.Size = new System.Drawing.Size(23, 22);
            this.btnNextEnd.TabIndex = 28;
            this.btnNextEnd.UseVisualStyleBackColor = true;
            // 
            // btnNextTen
            // 
            this.btnNextTen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNextTen.BackgroundImage = global::SDMS.Properties.Resources.btn_next_ten;
            this.btnNextTen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnNextTen.Location = new System.Drawing.Point(1105, 123);
            this.btnNextTen.Name = "btnNextTen";
            this.btnNextTen.Size = new System.Drawing.Size(23, 22);
            this.btnNextTen.TabIndex = 27;
            this.btnNextTen.UseVisualStyleBackColor = true;
            // 
            // btnPreviousTen
            // 
            this.btnPreviousTen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPreviousTen.BackgroundImage = global::SDMS.Properties.Resources.btn_previous_ten;
            this.btnPreviousTen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnPreviousTen.Location = new System.Drawing.Point(902, 123);
            this.btnPreviousTen.Name = "btnPreviousTen";
            this.btnPreviousTen.Size = new System.Drawing.Size(23, 22);
            this.btnPreviousTen.TabIndex = 26;
            this.btnPreviousTen.UseVisualStyleBackColor = true;
            // 
            // lblDescription1
            // 
            this.lblDescription1.AutoSize = true;
            this.lblDescription1.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDescription1.Location = new System.Drawing.Point(21, 106);
            this.lblDescription1.Name = "lblDescription1";
            this.lblDescription1.Size = new System.Drawing.Size(286, 18);
            this.lblDescription1.TabIndex = 25;
            this.lblDescription1.Text = "※ 측정 데이터는 약 2초 마다 기록합니다.";
            // 
            // cmbPageIndex
            // 
            this.cmbPageIndex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPageIndex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPageIndex.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbPageIndex.FormattingEnabled = true;
            this.cmbPageIndex.Location = new System.Drawing.Point(954, 124);
            this.cmbPageIndex.MaxDropDownItems = 20;
            this.cmbPageIndex.Name = "cmbPageIndex";
            this.cmbPageIndex.Size = new System.Drawing.Size(70, 26);
            this.cmbPageIndex.TabIndex = 24;
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.BackgroundImage = global::SDMS.Properties.Resources.btn_next;
            this.btnNext.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnNext.Location = new System.Drawing.Point(1083, 123);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(23, 22);
            this.btnNext.TabIndex = 23;
            this.btnNext.UseVisualStyleBackColor = true;
            // 
            // btnPrevious
            // 
            this.btnPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrevious.BackgroundImage = global::SDMS.Properties.Resources.btn_previous;
            this.btnPrevious.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnPrevious.Location = new System.Drawing.Point(924, 123);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(23, 22);
            this.btnPrevious.TabIndex = 22;
            this.btnPrevious.UseVisualStyleBackColor = true;
            // 
            // lblTotalPage
            // 
            this.lblTotalPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalPage.AutoSize = true;
            this.lblTotalPage.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTotalPage.Location = new System.Drawing.Point(1031, 129);
            this.lblTotalPage.Name = "lblTotalPage";
            this.lblTotalPage.Size = new System.Drawing.Size(69, 18);
            this.lblTotalPage.TabIndex = 21;
            this.lblTotalPage.Text = "/ 10000";
            // 
            // lblMaterialName
            // 
            this.lblMaterialName.AutoSize = true;
            this.lblMaterialName.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
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
            this.lblSensorNameHeader.Location = new System.Drawing.Point(21, 40);
            this.lblSensorNameHeader.Name = "lblSensorNameHeader";
            this.lblSensorNameHeader.Size = new System.Drawing.Size(81, 18);
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
            // DatePickerStart
            // 
            this.DatePickerStart.CustomFormat = "yyyy-MM-dd";
            this.DatePickerStart.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.DatePickerStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DatePickerStart.Location = new System.Drawing.Point(397, 28);
            this.DatePickerStart.Name = "DatePickerStart";
            this.DatePickerStart.Size = new System.Drawing.Size(100, 27);
            this.DatePickerStart.TabIndex = 24;
            this.DatePickerStart.Visible = false;
            // 
            // DatePickerEnd
            // 
            this.DatePickerEnd.CustomFormat = "yyyy-MM-dd";
            this.DatePickerEnd.Font = new System.Drawing.Font(Program.prgFont, 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.DatePickerEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DatePickerEnd.Location = new System.Drawing.Point(497, 28);
            this.DatePickerEnd.Name = "DatePickerEnd";
            this.DatePickerEnd.Size = new System.Drawing.Size(100, 27);
            this.DatePickerEnd.TabIndex = 25;
            this.DatePickerEnd.Visible = false;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            this.label1.Font = new System.Drawing.Font(Program.prgFont, 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(1184, 62);
            this.label1.TabIndex = 26;
            this.label1.Text = "  유해화학물질 측정 데이터";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormPSMSensorData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.BackgroundImage = global::SDMS.Properties.Resources.EditManager_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1184, 1083);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DatePickerStart);
            this.Controls.Add(this.DatePickerEnd);
            this.Controls.Add(this.pnBody);
            this.Controls.Add(this.pnHeader);
            this.Name = "FormPSMSensorData";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "유해화학물질 측정 데이터";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            this.pnHeader.ResumeLayout(false);
            this.pnHeader.PerformLayout();
            this.pnBody.ResumeLayout(false);
            this.pnBody.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ChartDirector.WinChartViewer chart;
        private System.Windows.Forms.Panel pnHeader;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label lblDatePreiod;
        private System.Windows.Forms.ComboBox cmbDateFix;
        private System.Windows.Forms.Label lblDateAnd;
        private System.Windows.Forms.Button btnDateEnd;
        private System.Windows.Forms.Button btnDateStart;
        private System.Windows.Forms.ComboBox cmbSensor;
        private System.Windows.Forms.Label lblSensor;
        private System.Windows.Forms.ComboBox cmbSensorLocation;
        private System.Windows.Forms.Label lblSensorLocation;
        private System.Windows.Forms.Panel pnBody;
        private System.Windows.Forms.Label lblMaterialName;
        private System.Windows.Forms.Label lblSensorName;
        private System.Windows.Forms.Label lblMaterialNameHeader;
        private System.Windows.Forms.Label lblSensorNameHeader;
        private System.Windows.Forms.ComboBox cmbPageIndex;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrevious;
        private System.Windows.Forms.Label lblTotalPage;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private System.Windows.Forms.DateTimePicker TimePickerStart;
        private System.Windows.Forms.DateTimePicker TimePickerEnd;
        private System.Windows.Forms.Label lblDescription1;
        private System.Windows.Forms.DateTimePicker DatePickerStart;
        private System.Windows.Forms.DateTimePicker DatePickerEnd;
        private System.Windows.Forms.Button btnNextTen;
        private System.Windows.Forms.Button btnPreviousTen;
        private System.Windows.Forms.Button btnPreviousEnd;
        private System.Windows.Forms.Button btnNextEnd;
        private System.Windows.Forms.Label lblSearchDate;
        private System.Windows.Forms.Label lblSearchDateHeader;
        private System.Windows.Forms.Label lblDescription2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
    }
}