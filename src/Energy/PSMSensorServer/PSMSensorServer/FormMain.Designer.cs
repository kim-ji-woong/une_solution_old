namespace PSMSensorServer
{
    #if! SERVICE
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
            /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
            /// </summary>
            private void InitializeComponent()
            {
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.txtBaseAddress = new System.Windows.Forms.TextBox();
            this.txtHmiAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFunction = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtModbusAddress = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.progressBar3 = new System.Windows.Forms.ProgressBar();
            this.button2 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.pbOn1 = new System.Windows.Forms.PictureBox();
            this.label9 = new System.Windows.Forms.Label();
            this.pbOn2 = new System.Windows.Forms.PictureBox();
            this.pbOn3 = new System.Windows.Forms.PictureBox();
            this.pbAlarm8 = new System.Windows.Forms.PictureBox();
            this.pbAlarm7 = new System.Windows.Forms.PictureBox();
            this.pbAlarm5 = new System.Windows.Forms.PictureBox();
            this.pbAlarm4 = new System.Windows.Forms.PictureBox();
            this.label10 = new System.Windows.Forms.Label();
            this.pbAlarm2 = new System.Windows.Forms.PictureBox();
            this.pbAlarm1 = new System.Windows.Forms.PictureBox();
            this.pbAlarm9 = new System.Windows.Forms.PictureBox();
            this.pbAlarm6 = new System.Windows.Forms.PictureBox();
            this.pbAlarm3 = new System.Windows.Forms.PictureBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.pbAlarm12 = new System.Windows.Forms.PictureBox();
            this.pbAlarm11 = new System.Windows.Forms.PictureBox();
            this.pbAlarm10 = new System.Windows.Forms.PictureBox();
            this.pbOn4 = new System.Windows.Forms.PictureBox();
            this.label11 = new System.Windows.Forms.Label();
            this.progressBar4 = new System.Windows.Forms.ProgressBar();
            this.pbAlarm15 = new System.Windows.Forms.PictureBox();
            this.pbAlarm14 = new System.Windows.Forms.PictureBox();
            this.pbAlarm13 = new System.Windows.Forms.PictureBox();
            this.pbOn5 = new System.Windows.Forms.PictureBox();
            this.label12 = new System.Windows.Forms.Label();
            this.progressBar5 = new System.Windows.Forms.ProgressBar();
            this.lbValue1 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lbValue2 = new System.Windows.Forms.Label();
            this.lbValue3 = new System.Windows.Forms.Label();
            this.lbValue4 = new System.Windows.Forms.Label();
            this.lbValue5 = new System.Windows.Forms.Label();
            this.lbValue6 = new System.Windows.Forms.Label();
            this.pbAlarm18 = new System.Windows.Forms.PictureBox();
            this.pbAlarm17 = new System.Windows.Forms.PictureBox();
            this.pbAlarm16 = new System.Windows.Forms.PictureBox();
            this.pbOn6 = new System.Windows.Forms.PictureBox();
            this.label15 = new System.Windows.Forms.Label();
            this.progressBar6 = new System.Windows.Forms.ProgressBar();
            this.lbValue7 = new System.Windows.Forms.Label();
            this.pbAlarm21 = new System.Windows.Forms.PictureBox();
            this.pbAlarm20 = new System.Windows.Forms.PictureBox();
            this.pbAlarm19 = new System.Windows.Forms.PictureBox();
            this.pbOn7 = new System.Windows.Forms.PictureBox();
            this.label17 = new System.Windows.Forms.Label();
            this.progressBar7 = new System.Windows.Forms.ProgressBar();
            this.lbValue8 = new System.Windows.Forms.Label();
            this.pbAlarm24 = new System.Windows.Forms.PictureBox();
            this.pbAlarm23 = new System.Windows.Forms.PictureBox();
            this.pbAlarm22 = new System.Windows.Forms.PictureBox();
            this.pbOn8 = new System.Windows.Forms.PictureBox();
            this.label19 = new System.Windows.Forms.Label();
            this.progressBar8 = new System.Windows.Forms.ProgressBar();
            this.lbValue9 = new System.Windows.Forms.Label();
            this.pbAlarm27 = new System.Windows.Forms.PictureBox();
            this.pbAlarm26 = new System.Windows.Forms.PictureBox();
            this.pbAlarm25 = new System.Windows.Forms.PictureBox();
            this.pbOn9 = new System.Windows.Forms.PictureBox();
            this.label16 = new System.Windows.Forms.Label();
            this.progressBar9 = new System.Windows.Forms.ProgressBar();
            this.button4 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.button15 = new System.Windows.Forms.Button();
            this.button16 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbOn1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOn2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOn3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOn4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOn5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm18)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm17)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOn6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm20)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm19)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOn7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm24)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm23)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm22)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOn8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm27)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm26)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm25)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOn9)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(38, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(91, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "서버 시작";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OnBeginServer);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(135, 12);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(91, 23);
            this.button6.TabIndex = 5;
            this.button6.Text = "서버 중지";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.OnStopServer);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // txtBaseAddress
            // 
            this.txtBaseAddress.Location = new System.Drawing.Point(794, 88);
            this.txtBaseAddress.Name = "txtBaseAddress";
            this.txtBaseAddress.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtBaseAddress.Size = new System.Drawing.Size(124, 21);
            this.txtBaseAddress.TabIndex = 8;
            this.txtBaseAddress.Text = "0";
            this.txtBaseAddress.Visible = false;
            // 
            // txtHmiAddress
            // 
            this.txtHmiAddress.Location = new System.Drawing.Point(794, 135);
            this.txtHmiAddress.Name = "txtHmiAddress";
            this.txtHmiAddress.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtHmiAddress.Size = new System.Drawing.Size(124, 21);
            this.txtHmiAddress.TabIndex = 9;
            this.txtHmiAddress.Text = "1";
            this.txtHmiAddress.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(794, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "Value";
            this.label1.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(794, 120);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 12);
            this.label2.TabIndex = 11;
            this.label2.Text = "Register";
            this.label2.Visible = false;
            // 
            // txtFunction
            // 
            this.txtFunction.Location = new System.Drawing.Point(794, 182);
            this.txtFunction.Name = "txtFunction";
            this.txtFunction.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtFunction.Size = new System.Drawing.Size(124, 21);
            this.txtFunction.TabIndex = 12;
            this.txtFunction.Text = "5";
            this.txtFunction.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(792, 167);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 13;
            this.label3.Text = "Function";
            this.label3.Visible = false;
            // 
            // txtModbusAddress
            // 
            this.txtModbusAddress.Location = new System.Drawing.Point(794, 232);
            this.txtModbusAddress.Name = "txtModbusAddress";
            this.txtModbusAddress.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtModbusAddress.Size = new System.Drawing.Size(124, 21);
            this.txtModbusAddress.TabIndex = 14;
            this.txtModbusAddress.Text = "1";
            this.txtModbusAddress.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(794, 217);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 15;
            this.label4.Text = "Unit Address";
            this.label4.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(38, 130);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.progressBar1.RightToLeftLayout = true;
            this.progressBar1.Size = new System.Drawing.Size(213, 20);
            this.progressBar1.TabIndex = 16;
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(38, 174);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.progressBar2.RightToLeftLayout = true;
            this.progressBar2.Size = new System.Drawing.Size(213, 19);
            this.progressBar2.TabIndex = 17;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(36, 117);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 12);
            this.label5.TabIndex = 18;
            this.label5.Text = "Alarm Unit #1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(36, 159);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 12);
            this.label6.TabIndex = 19;
            this.label6.Text = "Alarm Unit #2";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(36, 203);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 12);
            this.label7.TabIndex = 21;
            this.label7.Text = "Alarm Unit #3";
            // 
            // progressBar3
            // 
            this.progressBar3.Location = new System.Drawing.Point(38, 218);
            this.progressBar3.Name = "progressBar3";
            this.progressBar3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.progressBar3.RightToLeftLayout = true;
            this.progressBar3.Size = new System.Drawing.Size(213, 19);
            this.progressBar3.TabIndex = 20;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(827, 271);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(91, 23);
            this.button2.TabIndex = 22;
            this.button2.Text = "전송";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "훈련",
            "실제"});
            this.comboBox1.Location = new System.Drawing.Point(101, 75);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(72, 20);
            this.comboBox1.TabIndex = 23;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(36, 78);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 12);
            this.label8.TabIndex = 24;
            this.label8.Text = "COM Unit";
            // 
            // pbOn1
            // 
            this.pbOn1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbOn1.Location = new System.Drawing.Point(337, 132);
            this.pbOn1.Name = "pbOn1";
            this.pbOn1.Size = new System.Drawing.Size(26, 18);
            this.pbOn1.TabIndex = 25;
            this.pbOn1.TabStop = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(330, 117);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(42, 12);
            this.label9.TabIndex = 27;
            this.label9.Text = "On/Off";
            // 
            // pbOn2
            // 
            this.pbOn2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbOn2.Location = new System.Drawing.Point(337, 174);
            this.pbOn2.Name = "pbOn2";
            this.pbOn2.Size = new System.Drawing.Size(26, 17);
            this.pbOn2.TabIndex = 28;
            this.pbOn2.TabStop = false;
            // 
            // pbOn3
            // 
            this.pbOn3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbOn3.Location = new System.Drawing.Point(337, 218);
            this.pbOn3.Name = "pbOn3";
            this.pbOn3.Size = new System.Drawing.Size(26, 17);
            this.pbOn3.TabIndex = 30;
            this.pbOn3.TabStop = false;
            // 
            // pbAlarm8
            // 
            this.pbAlarm8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm8.Location = new System.Drawing.Point(414, 218);
            this.pbAlarm8.Name = "pbAlarm8";
            this.pbAlarm8.Size = new System.Drawing.Size(26, 17);
            this.pbAlarm8.TabIndex = 38;
            this.pbAlarm8.TabStop = false;
            // 
            // pbAlarm7
            // 
            this.pbAlarm7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm7.Location = new System.Drawing.Point(380, 218);
            this.pbAlarm7.Name = "pbAlarm7";
            this.pbAlarm7.Size = new System.Drawing.Size(26, 17);
            this.pbAlarm7.TabIndex = 37;
            this.pbAlarm7.TabStop = false;
            // 
            // pbAlarm5
            // 
            this.pbAlarm5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm5.Location = new System.Drawing.Point(414, 174);
            this.pbAlarm5.Name = "pbAlarm5";
            this.pbAlarm5.Size = new System.Drawing.Size(26, 17);
            this.pbAlarm5.TabIndex = 36;
            this.pbAlarm5.TabStop = false;
            // 
            // pbAlarm4
            // 
            this.pbAlarm4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm4.Location = new System.Drawing.Point(380, 174);
            this.pbAlarm4.Name = "pbAlarm4";
            this.pbAlarm4.Size = new System.Drawing.Size(26, 17);
            this.pbAlarm4.TabIndex = 35;
            this.pbAlarm4.TabStop = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(387, 117);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(77, 12);
            this.label10.TabIndex = 34;
            this.label10.Text = "Alarm Status";
            // 
            // pbAlarm2
            // 
            this.pbAlarm2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm2.Location = new System.Drawing.Point(414, 132);
            this.pbAlarm2.Name = "pbAlarm2";
            this.pbAlarm2.Size = new System.Drawing.Size(26, 18);
            this.pbAlarm2.TabIndex = 33;
            this.pbAlarm2.TabStop = false;
            // 
            // pbAlarm1
            // 
            this.pbAlarm1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm1.Location = new System.Drawing.Point(380, 132);
            this.pbAlarm1.Name = "pbAlarm1";
            this.pbAlarm1.Size = new System.Drawing.Size(26, 18);
            this.pbAlarm1.TabIndex = 32;
            this.pbAlarm1.TabStop = false;
            // 
            // pbAlarm9
            // 
            this.pbAlarm9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm9.Location = new System.Drawing.Point(446, 218);
            this.pbAlarm9.Name = "pbAlarm9";
            this.pbAlarm9.Size = new System.Drawing.Size(26, 17);
            this.pbAlarm9.TabIndex = 41;
            this.pbAlarm9.TabStop = false;
            // 
            // pbAlarm6
            // 
            this.pbAlarm6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm6.Location = new System.Drawing.Point(446, 174);
            this.pbAlarm6.Name = "pbAlarm6";
            this.pbAlarm6.Size = new System.Drawing.Size(26, 17);
            this.pbAlarm6.TabIndex = 40;
            this.pbAlarm6.TabStop = false;
            // 
            // pbAlarm3
            // 
            this.pbAlarm3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm3.Location = new System.Drawing.Point(446, 132);
            this.pbAlarm3.Name = "pbAlarm3";
            this.pbAlarm3.Size = new System.Drawing.Size(26, 18);
            this.pbAlarm3.TabIndex = 39;
            this.pbAlarm3.TabStop = false;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(324, 12);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(94, 23);
            this.btnReset.TabIndex = 42;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // pbAlarm12
            // 
            this.pbAlarm12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm12.Location = new System.Drawing.Point(446, 263);
            this.pbAlarm12.Name = "pbAlarm12";
            this.pbAlarm12.Size = new System.Drawing.Size(26, 19);
            this.pbAlarm12.TabIndex = 50;
            this.pbAlarm12.TabStop = false;
            // 
            // pbAlarm11
            // 
            this.pbAlarm11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm11.Location = new System.Drawing.Point(414, 263);
            this.pbAlarm11.Name = "pbAlarm11";
            this.pbAlarm11.Size = new System.Drawing.Size(26, 19);
            this.pbAlarm11.TabIndex = 49;
            this.pbAlarm11.TabStop = false;
            // 
            // pbAlarm10
            // 
            this.pbAlarm10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm10.Location = new System.Drawing.Point(380, 263);
            this.pbAlarm10.Name = "pbAlarm10";
            this.pbAlarm10.Size = new System.Drawing.Size(26, 19);
            this.pbAlarm10.TabIndex = 48;
            this.pbAlarm10.TabStop = false;
            // 
            // pbOn4
            // 
            this.pbOn4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbOn4.Location = new System.Drawing.Point(337, 263);
            this.pbOn4.Name = "pbOn4";
            this.pbOn4.Size = new System.Drawing.Size(26, 19);
            this.pbOn4.TabIndex = 46;
            this.pbOn4.TabStop = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(36, 248);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(79, 12);
            this.label11.TabIndex = 45;
            this.label11.Text = "Alarm Unit #4";
            // 
            // progressBar4
            // 
            this.progressBar4.Location = new System.Drawing.Point(38, 263);
            this.progressBar4.Name = "progressBar4";
            this.progressBar4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.progressBar4.RightToLeftLayout = true;
            this.progressBar4.Size = new System.Drawing.Size(213, 21);
            this.progressBar4.TabIndex = 44;
            // 
            // pbAlarm15
            // 
            this.pbAlarm15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm15.Location = new System.Drawing.Point(446, 310);
            this.pbAlarm15.Name = "pbAlarm15";
            this.pbAlarm15.Size = new System.Drawing.Size(26, 16);
            this.pbAlarm15.TabIndex = 56;
            this.pbAlarm15.TabStop = false;
            // 
            // pbAlarm14
            // 
            this.pbAlarm14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm14.Location = new System.Drawing.Point(414, 310);
            this.pbAlarm14.Name = "pbAlarm14";
            this.pbAlarm14.Size = new System.Drawing.Size(26, 16);
            this.pbAlarm14.TabIndex = 55;
            this.pbAlarm14.TabStop = false;
            // 
            // pbAlarm13
            // 
            this.pbAlarm13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm13.Location = new System.Drawing.Point(380, 310);
            this.pbAlarm13.Name = "pbAlarm13";
            this.pbAlarm13.Size = new System.Drawing.Size(26, 16);
            this.pbAlarm13.TabIndex = 54;
            this.pbAlarm13.TabStop = false;
            // 
            // pbOn5
            // 
            this.pbOn5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbOn5.Location = new System.Drawing.Point(337, 310);
            this.pbOn5.Name = "pbOn5";
            this.pbOn5.Size = new System.Drawing.Size(26, 16);
            this.pbOn5.TabIndex = 53;
            this.pbOn5.TabStop = false;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(36, 295);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(79, 12);
            this.label12.TabIndex = 52;
            this.label12.Text = "Alarm Unit #5";
            // 
            // progressBar5
            // 
            this.progressBar5.Location = new System.Drawing.Point(38, 310);
            this.progressBar5.Name = "progressBar5";
            this.progressBar5.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.progressBar5.RightToLeftLayout = true;
            this.progressBar5.Size = new System.Drawing.Size(213, 18);
            this.progressBar5.TabIndex = 51;
            // 
            // lbValue1
            // 
            this.lbValue1.AutoSize = true;
            this.lbValue1.Location = new System.Drawing.Point(268, 140);
            this.lbValue1.Name = "lbValue1";
            this.lbValue1.Size = new System.Drawing.Size(11, 12);
            this.lbValue1.TabIndex = 57;
            this.lbValue1.Text = "0";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(268, 117);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(37, 12);
            this.label14.TabIndex = 58;
            this.label14.Text = "Value";
            // 
            // lbValue2
            // 
            this.lbValue2.AutoSize = true;
            this.lbValue2.Location = new System.Drawing.Point(269, 181);
            this.lbValue2.Name = "lbValue2";
            this.lbValue2.Size = new System.Drawing.Size(11, 12);
            this.lbValue2.TabIndex = 59;
            this.lbValue2.Text = "0";
            // 
            // lbValue3
            // 
            this.lbValue3.AutoSize = true;
            this.lbValue3.Location = new System.Drawing.Point(269, 225);
            this.lbValue3.Name = "lbValue3";
            this.lbValue3.Size = new System.Drawing.Size(11, 12);
            this.lbValue3.TabIndex = 60;
            this.lbValue3.Text = "0";
            // 
            // lbValue4
            // 
            this.lbValue4.AutoSize = true;
            this.lbValue4.Location = new System.Drawing.Point(269, 272);
            this.lbValue4.Name = "lbValue4";
            this.lbValue4.Size = new System.Drawing.Size(11, 12);
            this.lbValue4.TabIndex = 61;
            this.lbValue4.Text = "0";
            // 
            // lbValue5
            // 
            this.lbValue5.AutoSize = true;
            this.lbValue5.Location = new System.Drawing.Point(269, 316);
            this.lbValue5.Name = "lbValue5";
            this.lbValue5.Size = new System.Drawing.Size(11, 12);
            this.lbValue5.TabIndex = 62;
            this.lbValue5.Text = "0";
            // 
            // lbValue6
            // 
            this.lbValue6.AutoSize = true;
            this.lbValue6.Location = new System.Drawing.Point(269, 360);
            this.lbValue6.Name = "lbValue6";
            this.lbValue6.Size = new System.Drawing.Size(11, 12);
            this.lbValue6.TabIndex = 69;
            this.lbValue6.Text = "0";
            // 
            // pbAlarm18
            // 
            this.pbAlarm18.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm18.Location = new System.Drawing.Point(446, 354);
            this.pbAlarm18.Name = "pbAlarm18";
            this.pbAlarm18.Size = new System.Drawing.Size(26, 16);
            this.pbAlarm18.TabIndex = 68;
            this.pbAlarm18.TabStop = false;
            // 
            // pbAlarm17
            // 
            this.pbAlarm17.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm17.Location = new System.Drawing.Point(414, 354);
            this.pbAlarm17.Name = "pbAlarm17";
            this.pbAlarm17.Size = new System.Drawing.Size(26, 16);
            this.pbAlarm17.TabIndex = 67;
            this.pbAlarm17.TabStop = false;
            // 
            // pbAlarm16
            // 
            this.pbAlarm16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm16.Location = new System.Drawing.Point(380, 354);
            this.pbAlarm16.Name = "pbAlarm16";
            this.pbAlarm16.Size = new System.Drawing.Size(26, 16);
            this.pbAlarm16.TabIndex = 66;
            this.pbAlarm16.TabStop = false;
            // 
            // pbOn6
            // 
            this.pbOn6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbOn6.Location = new System.Drawing.Point(337, 354);
            this.pbOn6.Name = "pbOn6";
            this.pbOn6.Size = new System.Drawing.Size(26, 16);
            this.pbOn6.TabIndex = 65;
            this.pbOn6.TabStop = false;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(36, 339);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(79, 12);
            this.label15.TabIndex = 64;
            this.label15.Text = "Alarm Unit #6";
            // 
            // progressBar6
            // 
            this.progressBar6.Location = new System.Drawing.Point(38, 354);
            this.progressBar6.Name = "progressBar6";
            this.progressBar6.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.progressBar6.RightToLeftLayout = true;
            this.progressBar6.Size = new System.Drawing.Size(213, 18);
            this.progressBar6.TabIndex = 63;
            // 
            // lbValue7
            // 
            this.lbValue7.AutoSize = true;
            this.lbValue7.Location = new System.Drawing.Point(269, 399);
            this.lbValue7.Name = "lbValue7";
            this.lbValue7.Size = new System.Drawing.Size(11, 12);
            this.lbValue7.TabIndex = 76;
            this.lbValue7.Text = "0";
            // 
            // pbAlarm21
            // 
            this.pbAlarm21.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm21.Location = new System.Drawing.Point(446, 393);
            this.pbAlarm21.Name = "pbAlarm21";
            this.pbAlarm21.Size = new System.Drawing.Size(26, 16);
            this.pbAlarm21.TabIndex = 75;
            this.pbAlarm21.TabStop = false;
            // 
            // pbAlarm20
            // 
            this.pbAlarm20.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm20.Location = new System.Drawing.Point(414, 393);
            this.pbAlarm20.Name = "pbAlarm20";
            this.pbAlarm20.Size = new System.Drawing.Size(26, 16);
            this.pbAlarm20.TabIndex = 74;
            this.pbAlarm20.TabStop = false;
            // 
            // pbAlarm19
            // 
            this.pbAlarm19.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm19.Location = new System.Drawing.Point(380, 393);
            this.pbAlarm19.Name = "pbAlarm19";
            this.pbAlarm19.Size = new System.Drawing.Size(26, 16);
            this.pbAlarm19.TabIndex = 73;
            this.pbAlarm19.TabStop = false;
            // 
            // pbOn7
            // 
            this.pbOn7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbOn7.Location = new System.Drawing.Point(337, 393);
            this.pbOn7.Name = "pbOn7";
            this.pbOn7.Size = new System.Drawing.Size(26, 16);
            this.pbOn7.TabIndex = 72;
            this.pbOn7.TabStop = false;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(36, 378);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(79, 12);
            this.label17.TabIndex = 71;
            this.label17.Text = "Alarm Unit #7";
            // 
            // progressBar7
            // 
            this.progressBar7.Location = new System.Drawing.Point(38, 393);
            this.progressBar7.Name = "progressBar7";
            this.progressBar7.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.progressBar7.RightToLeftLayout = true;
            this.progressBar7.Size = new System.Drawing.Size(213, 18);
            this.progressBar7.TabIndex = 70;
            // 
            // lbValue8
            // 
            this.lbValue8.AutoSize = true;
            this.lbValue8.Location = new System.Drawing.Point(269, 442);
            this.lbValue8.Name = "lbValue8";
            this.lbValue8.Size = new System.Drawing.Size(11, 12);
            this.lbValue8.TabIndex = 83;
            this.lbValue8.Text = "0";
            // 
            // pbAlarm24
            // 
            this.pbAlarm24.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm24.Location = new System.Drawing.Point(446, 436);
            this.pbAlarm24.Name = "pbAlarm24";
            this.pbAlarm24.Size = new System.Drawing.Size(26, 16);
            this.pbAlarm24.TabIndex = 82;
            this.pbAlarm24.TabStop = false;
            // 
            // pbAlarm23
            // 
            this.pbAlarm23.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm23.Location = new System.Drawing.Point(414, 436);
            this.pbAlarm23.Name = "pbAlarm23";
            this.pbAlarm23.Size = new System.Drawing.Size(26, 16);
            this.pbAlarm23.TabIndex = 81;
            this.pbAlarm23.TabStop = false;
            // 
            // pbAlarm22
            // 
            this.pbAlarm22.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm22.Location = new System.Drawing.Point(380, 436);
            this.pbAlarm22.Name = "pbAlarm22";
            this.pbAlarm22.Size = new System.Drawing.Size(26, 16);
            this.pbAlarm22.TabIndex = 80;
            this.pbAlarm22.TabStop = false;
            // 
            // pbOn8
            // 
            this.pbOn8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbOn8.Location = new System.Drawing.Point(337, 436);
            this.pbOn8.Name = "pbOn8";
            this.pbOn8.Size = new System.Drawing.Size(26, 16);
            this.pbOn8.TabIndex = 79;
            this.pbOn8.TabStop = false;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(36, 421);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(79, 12);
            this.label19.TabIndex = 78;
            this.label19.Text = "Alarm Unit #8";
            // 
            // progressBar8
            // 
            this.progressBar8.Location = new System.Drawing.Point(38, 436);
            this.progressBar8.Name = "progressBar8";
            this.progressBar8.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.progressBar8.RightToLeftLayout = true;
            this.progressBar8.Size = new System.Drawing.Size(213, 18);
            this.progressBar8.TabIndex = 77;
            // 
            // lbValue9
            // 
            this.lbValue9.AutoSize = true;
            this.lbValue9.Location = new System.Drawing.Point(269, 490);
            this.lbValue9.Name = "lbValue9";
            this.lbValue9.Size = new System.Drawing.Size(11, 12);
            this.lbValue9.TabIndex = 90;
            this.lbValue9.Text = "0";
            // 
            // pbAlarm27
            // 
            this.pbAlarm27.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm27.Location = new System.Drawing.Point(446, 484);
            this.pbAlarm27.Name = "pbAlarm27";
            this.pbAlarm27.Size = new System.Drawing.Size(26, 16);
            this.pbAlarm27.TabIndex = 89;
            this.pbAlarm27.TabStop = false;
            // 
            // pbAlarm26
            // 
            this.pbAlarm26.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm26.Location = new System.Drawing.Point(414, 484);
            this.pbAlarm26.Name = "pbAlarm26";
            this.pbAlarm26.Size = new System.Drawing.Size(26, 16);
            this.pbAlarm26.TabIndex = 88;
            this.pbAlarm26.TabStop = false;
            // 
            // pbAlarm25
            // 
            this.pbAlarm25.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAlarm25.Location = new System.Drawing.Point(380, 484);
            this.pbAlarm25.Name = "pbAlarm25";
            this.pbAlarm25.Size = new System.Drawing.Size(26, 16);
            this.pbAlarm25.TabIndex = 87;
            this.pbAlarm25.TabStop = false;
            // 
            // pbOn9
            // 
            this.pbOn9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbOn9.Location = new System.Drawing.Point(337, 484);
            this.pbOn9.Name = "pbOn9";
            this.pbOn9.Size = new System.Drawing.Size(26, 16);
            this.pbOn9.TabIndex = 86;
            this.pbOn9.TabStop = false;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(36, 469);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(79, 12);
            this.label16.TabIndex = 85;
            this.label16.Text = "Alarm Unit #9";
            // 
            // progressBar9
            // 
            this.progressBar9.Location = new System.Drawing.Point(38, 484);
            this.progressBar9.Name = "progressBar9";
            this.progressBar9.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.progressBar9.RightToLeftLayout = true;
            this.progressBar9.Size = new System.Drawing.Size(213, 18);
            this.progressBar9.TabIndex = 84;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(232, 11);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(86, 24);
            this.button4.TabIndex = 91;
            this.button4.Text = "날씨정보";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(489, 124);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(57, 28);
            this.button8.TabIndex = 94;
            this.button8.Text = "해제";
            this.button8.UseVisualStyleBackColor = true;
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(489, 165);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(57, 28);
            this.button9.TabIndex = 95;
            this.button9.Text = "해제";
            this.button9.UseVisualStyleBackColor = true;
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(489, 209);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(57, 28);
            this.button10.TabIndex = 96;
            this.button10.Text = "해제";
            this.button10.UseVisualStyleBackColor = true;
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(489, 256);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(57, 28);
            this.button11.TabIndex = 97;
            this.button11.Text = "해제";
            this.button11.UseVisualStyleBackColor = true;
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(489, 300);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(57, 28);
            this.button12.TabIndex = 98;
            this.button12.Text = "해제";
            this.button12.UseVisualStyleBackColor = true;
            // 
            // button13
            // 
            this.button13.Location = new System.Drawing.Point(489, 344);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(57, 28);
            this.button13.TabIndex = 99;
            this.button13.Text = "해제";
            this.button13.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(489, 383);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(57, 28);
            this.button7.TabIndex = 101;
            this.button7.Text = "해제";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // button14
            // 
            this.button14.Location = new System.Drawing.Point(489, 424);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(57, 28);
            this.button14.TabIndex = 102;
            this.button14.Text = "해제";
            this.button14.UseVisualStyleBackColor = true;
            // 
            // button15
            // 
            this.button15.Location = new System.Drawing.Point(489, 474);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(57, 28);
            this.button15.TabIndex = 103;
            this.button15.Text = "해제";
            this.button15.UseVisualStyleBackColor = true;
            // 
            // button16
            // 
            this.button16.Location = new System.Drawing.Point(424, 11);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(86, 24);
            this.button16.TabIndex = 104;
            this.button16.Text = "알람상태";
            this.button16.UseVisualStyleBackColor = true;
            this.button16.Click += new System.EventHandler(this.button16_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(324, 42);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(94, 23);
            this.button3.TabIndex = 105;
            this.button3.Text = "테스트알람";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(558, 537);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button16);
            this.Controls.Add(this.button15);
            this.Controls.Add(this.button14);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button13);
            this.Controls.Add(this.button12);
            this.Controls.Add(this.button11);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.lbValue9);
            this.Controls.Add(this.pbAlarm27);
            this.Controls.Add(this.pbAlarm26);
            this.Controls.Add(this.pbAlarm25);
            this.Controls.Add(this.pbOn9);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.progressBar9);
            this.Controls.Add(this.lbValue8);
            this.Controls.Add(this.pbAlarm24);
            this.Controls.Add(this.pbAlarm23);
            this.Controls.Add(this.pbAlarm22);
            this.Controls.Add(this.pbOn8);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.progressBar8);
            this.Controls.Add(this.lbValue7);
            this.Controls.Add(this.pbAlarm21);
            this.Controls.Add(this.pbAlarm20);
            this.Controls.Add(this.pbAlarm19);
            this.Controls.Add(this.pbOn7);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.progressBar7);
            this.Controls.Add(this.lbValue6);
            this.Controls.Add(this.pbAlarm18);
            this.Controls.Add(this.pbAlarm17);
            this.Controls.Add(this.pbAlarm16);
            this.Controls.Add(this.pbOn6);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.progressBar6);
            this.Controls.Add(this.lbValue5);
            this.Controls.Add(this.lbValue4);
            this.Controls.Add(this.lbValue3);
            this.Controls.Add(this.lbValue2);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.lbValue1);
            this.Controls.Add(this.pbAlarm15);
            this.Controls.Add(this.pbAlarm14);
            this.Controls.Add(this.pbAlarm13);
            this.Controls.Add(this.pbOn5);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.progressBar5);
            this.Controls.Add(this.pbAlarm12);
            this.Controls.Add(this.pbAlarm11);
            this.Controls.Add(this.pbAlarm10);
            this.Controls.Add(this.pbOn4);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.progressBar4);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.pbAlarm9);
            this.Controls.Add(this.pbAlarm6);
            this.Controls.Add(this.pbAlarm3);
            this.Controls.Add(this.pbAlarm8);
            this.Controls.Add(this.pbAlarm7);
            this.Controls.Add(this.pbAlarm5);
            this.Controls.Add(this.pbAlarm4);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.pbAlarm2);
            this.Controls.Add(this.pbAlarm1);
            this.Controls.Add(this.pbOn3);
            this.Controls.Add(this.pbOn2);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.pbOn1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.progressBar3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtModbusAddress);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtFunction);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtHmiAddress);
            this.Controls.Add(this.txtBaseAddress);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button1);
            this.Name = "FormMain";
            this.Text = "에너지산업훈련 - PSM 센서서버";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbOn1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOn2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOn3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOn4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOn5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm18)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm17)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOn6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm20)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm19)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOn7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm24)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm23)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm22)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOn8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm27)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm26)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAlarm25)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOn9)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

            }

            #endregion

            private System.Windows.Forms.Button button1;
            private System.Windows.Forms.Button button6;
            private System.Windows.Forms.Timer timer1;
            private System.Windows.Forms.TextBox txtBaseAddress;
            private System.Windows.Forms.TextBox txtHmiAddress;
            private System.Windows.Forms.Label label1;
            private System.Windows.Forms.Label label2;
            private System.Windows.Forms.TextBox txtFunction;
            private System.Windows.Forms.Label label3;
            private System.Windows.Forms.TextBox txtModbusAddress;
            private System.Windows.Forms.Label label4;
            private System.Windows.Forms.ProgressBar progressBar1;
            private System.Windows.Forms.ProgressBar progressBar2;
            private System.Windows.Forms.Label label5;
            private System.Windows.Forms.Label label6;
            private System.Windows.Forms.Label label7;
            private System.Windows.Forms.ProgressBar progressBar3;
            private System.Windows.Forms.Button button2;
            private System.Windows.Forms.ComboBox comboBox1;
            private System.Windows.Forms.Label label8;
            private System.Windows.Forms.PictureBox pbOn1;
            private System.Windows.Forms.Label label9;
            private System.Windows.Forms.PictureBox pbOn2;
            private System.Windows.Forms.PictureBox pbOn3;
            private System.Windows.Forms.PictureBox pbAlarm8;
            private System.Windows.Forms.PictureBox pbAlarm7;
            private System.Windows.Forms.PictureBox pbAlarm5;
            private System.Windows.Forms.PictureBox pbAlarm4;
            private System.Windows.Forms.Label label10;
            private System.Windows.Forms.PictureBox pbAlarm2;
            private System.Windows.Forms.PictureBox pbAlarm1;
            private System.Windows.Forms.PictureBox pbAlarm9;
            private System.Windows.Forms.PictureBox pbAlarm6;
            private System.Windows.Forms.PictureBox pbAlarm3;
            private System.Windows.Forms.Button btnReset;
            private System.Windows.Forms.PictureBox pbAlarm12;
            private System.Windows.Forms.PictureBox pbAlarm11;
            private System.Windows.Forms.PictureBox pbAlarm10;
            private System.Windows.Forms.PictureBox pbOn4;
            private System.Windows.Forms.Label label11;
            private System.Windows.Forms.ProgressBar progressBar4;
            private System.Windows.Forms.PictureBox pbAlarm15;
            private System.Windows.Forms.PictureBox pbAlarm14;
            private System.Windows.Forms.PictureBox pbAlarm13;
            private System.Windows.Forms.PictureBox pbOn5;
            private System.Windows.Forms.Label label12;
            private System.Windows.Forms.ProgressBar progressBar5;
            private System.Windows.Forms.Label lbValue1;
            private System.Windows.Forms.Label label14;
            private System.Windows.Forms.Label lbValue2;
            private System.Windows.Forms.Label lbValue3;
            private System.Windows.Forms.Label lbValue4;
            private System.Windows.Forms.Label lbValue5;
            private System.Windows.Forms.Label lbValue6;
            private System.Windows.Forms.PictureBox pbAlarm18;
            private System.Windows.Forms.PictureBox pbAlarm17;
            private System.Windows.Forms.PictureBox pbAlarm16;
            private System.Windows.Forms.PictureBox pbOn6;
            private System.Windows.Forms.Label label15;
            private System.Windows.Forms.ProgressBar progressBar6;
            private System.Windows.Forms.Label lbValue7;
            private System.Windows.Forms.PictureBox pbAlarm21;
            private System.Windows.Forms.PictureBox pbAlarm20;
            private System.Windows.Forms.PictureBox pbAlarm19;
            private System.Windows.Forms.PictureBox pbOn7;
            private System.Windows.Forms.Label label17;
            private System.Windows.Forms.ProgressBar progressBar7;
            private System.Windows.Forms.Label lbValue8;
            private System.Windows.Forms.PictureBox pbAlarm24;
            private System.Windows.Forms.PictureBox pbAlarm23;
            private System.Windows.Forms.PictureBox pbAlarm22;
            private System.Windows.Forms.PictureBox pbOn8;
            private System.Windows.Forms.Label label19;
            private System.Windows.Forms.ProgressBar progressBar8;
            private System.Windows.Forms.Label lbValue9;
            private System.Windows.Forms.PictureBox pbAlarm27;
            private System.Windows.Forms.PictureBox pbAlarm26;
            private System.Windows.Forms.PictureBox pbAlarm25;
            private System.Windows.Forms.PictureBox pbOn9;
            private System.Windows.Forms.Label label16;
            private System.Windows.Forms.ProgressBar progressBar9;
            private System.Windows.Forms.Button button4;
            private System.Windows.Forms.Button button8;
            private System.Windows.Forms.Button button9;
            private System.Windows.Forms.Button button10;
            private System.Windows.Forms.Button button11;
            private System.Windows.Forms.Button button12;
            private System.Windows.Forms.Button button13;
            private System.Windows.Forms.Button button7;
            private System.Windows.Forms.Button button14;
            private System.Windows.Forms.Button button15;
            private System.Windows.Forms.Button button16;
            private System.Windows.Forms.Button button3;
        }

#endif
 }

