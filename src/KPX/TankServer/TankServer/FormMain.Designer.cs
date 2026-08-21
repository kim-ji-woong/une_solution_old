#if!SERVICE

namespace TankServer
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
            /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
            /// </summary>
            private void InitializeComponent()
            {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.button1 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label_211 = new System.Windows.Forms.Label();
            this.label_212 = new System.Windows.Forms.Label();
            this.label_214 = new System.Windows.Forms.Label();
            this.label_215 = new System.Windows.Forms.Label();
            this.label_216 = new System.Windows.Forms.Label();
            this.lable_evtStatus216 = new System.Windows.Forms.Label();
            this.lable_evtStatus215 = new System.Windows.Forms.Label();
            this.lable_evtStatus214 = new System.Windows.Forms.Label();
            this.lable_evtStatus212 = new System.Windows.Forms.Label();
            this.lable_evtStatus211 = new System.Windows.Forms.Label();
            this.lable_leakPosition216 = new System.Windows.Forms.Label();
            this.lable_leakPosition215 = new System.Windows.Forms.Label();
            this.lable_leakPosition214 = new System.Windows.Forms.Label();
            this.lable_leakPosition212 = new System.Windows.Forms.Label();
            this.lable_leakPosition211 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.label49 = new System.Windows.Forms.Label();
            this.label50 = new System.Windows.Forms.Label();
            this.label51 = new System.Windows.Forms.Label();
            this.label52 = new System.Windows.Forms.Label();
            this.label53 = new System.Windows.Forms.Label();
            this.label54 = new System.Windows.Forms.Label();
            this.button_1 = new System.Windows.Forms.Button();
            this.button_2 = new System.Windows.Forms.Button();
            this.button_3 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.열기ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.종료ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(58, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(91, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "서버 시작";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OnBeginServer);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(155, 12);
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
            // label_211
            // 
            this.label_211.AutoSize = true;
            this.label_211.Location = new System.Drawing.Point(31, 199);
            this.label_211.Name = "label_211";
            this.label_211.Size = new System.Drawing.Size(23, 12);
            this.label_211.TabIndex = 6;
            this.label_211.Text = "211";
            // 
            // label_212
            // 
            this.label_212.AutoSize = true;
            this.label_212.Location = new System.Drawing.Point(31, 241);
            this.label_212.Name = "label_212";
            this.label_212.Size = new System.Drawing.Size(23, 12);
            this.label_212.TabIndex = 7;
            this.label_212.Text = "212";
            // 
            // label_214
            // 
            this.label_214.AutoSize = true;
            this.label_214.Location = new System.Drawing.Point(31, 283);
            this.label_214.Name = "label_214";
            this.label_214.Size = new System.Drawing.Size(23, 12);
            this.label_214.TabIndex = 8;
            this.label_214.Text = "214";
            // 
            // label_215
            // 
            this.label_215.AutoSize = true;
            this.label_215.Location = new System.Drawing.Point(31, 324);
            this.label_215.Name = "label_215";
            this.label_215.Size = new System.Drawing.Size(23, 12);
            this.label_215.TabIndex = 9;
            this.label_215.Text = "215";
            // 
            // label_216
            // 
            this.label_216.AutoSize = true;
            this.label_216.Location = new System.Drawing.Point(31, 367);
            this.label_216.Name = "label_216";
            this.label_216.Size = new System.Drawing.Size(23, 12);
            this.label_216.TabIndex = 10;
            this.label_216.Text = "216";
            // 
            // lable_evtStatus216
            // 
            this.lable_evtStatus216.AutoSize = true;
            this.lable_evtStatus216.Location = new System.Drawing.Point(98, 367);
            this.lable_evtStatus216.Name = "lable_evtStatus216";
            this.lable_evtStatus216.Size = new System.Drawing.Size(11, 12);
            this.lable_evtStatus216.TabIndex = 15;
            this.lable_evtStatus216.Text = "-";
            // 
            // lable_evtStatus215
            // 
            this.lable_evtStatus215.AutoSize = true;
            this.lable_evtStatus215.Location = new System.Drawing.Point(98, 324);
            this.lable_evtStatus215.Name = "lable_evtStatus215";
            this.lable_evtStatus215.Size = new System.Drawing.Size(11, 12);
            this.lable_evtStatus215.TabIndex = 14;
            this.lable_evtStatus215.Text = "-";
            // 
            // lable_evtStatus214
            // 
            this.lable_evtStatus214.AutoSize = true;
            this.lable_evtStatus214.Location = new System.Drawing.Point(98, 283);
            this.lable_evtStatus214.Name = "lable_evtStatus214";
            this.lable_evtStatus214.Size = new System.Drawing.Size(11, 12);
            this.lable_evtStatus214.TabIndex = 13;
            this.lable_evtStatus214.Text = "-";
            // 
            // lable_evtStatus212
            // 
            this.lable_evtStatus212.AutoSize = true;
            this.lable_evtStatus212.Location = new System.Drawing.Point(98, 241);
            this.lable_evtStatus212.Name = "lable_evtStatus212";
            this.lable_evtStatus212.Size = new System.Drawing.Size(11, 12);
            this.lable_evtStatus212.TabIndex = 12;
            this.lable_evtStatus212.Text = "-";
            // 
            // lable_evtStatus211
            // 
            this.lable_evtStatus211.AutoSize = true;
            this.lable_evtStatus211.Location = new System.Drawing.Point(98, 199);
            this.lable_evtStatus211.Name = "lable_evtStatus211";
            this.lable_evtStatus211.Size = new System.Drawing.Size(11, 12);
            this.lable_evtStatus211.TabIndex = 11;
            this.lable_evtStatus211.Text = "-";
            // 
            // lable_leakPosition216
            // 
            this.lable_leakPosition216.AutoSize = true;
            this.lable_leakPosition216.Location = new System.Drawing.Point(191, 367);
            this.lable_leakPosition216.Name = "lable_leakPosition216";
            this.lable_leakPosition216.Size = new System.Drawing.Size(11, 12);
            this.lable_leakPosition216.TabIndex = 20;
            this.lable_leakPosition216.Text = "-";
            // 
            // lable_leakPosition215
            // 
            this.lable_leakPosition215.AutoSize = true;
            this.lable_leakPosition215.Location = new System.Drawing.Point(191, 324);
            this.lable_leakPosition215.Name = "lable_leakPosition215";
            this.lable_leakPosition215.Size = new System.Drawing.Size(11, 12);
            this.lable_leakPosition215.TabIndex = 19;
            this.lable_leakPosition215.Text = "-";
            // 
            // lable_leakPosition214
            // 
            this.lable_leakPosition214.AutoSize = true;
            this.lable_leakPosition214.Location = new System.Drawing.Point(191, 283);
            this.lable_leakPosition214.Name = "lable_leakPosition214";
            this.lable_leakPosition214.Size = new System.Drawing.Size(11, 12);
            this.lable_leakPosition214.TabIndex = 18;
            this.lable_leakPosition214.Text = "-";
            // 
            // lable_leakPosition212
            // 
            this.lable_leakPosition212.AutoSize = true;
            this.lable_leakPosition212.Location = new System.Drawing.Point(191, 241);
            this.lable_leakPosition212.Name = "lable_leakPosition212";
            this.lable_leakPosition212.Size = new System.Drawing.Size(11, 12);
            this.lable_leakPosition212.TabIndex = 17;
            this.lable_leakPosition212.Text = "-";
            // 
            // lable_leakPosition211
            // 
            this.lable_leakPosition211.AutoSize = true;
            this.lable_leakPosition211.Location = new System.Drawing.Point(191, 199);
            this.lable_leakPosition211.Name = "lable_leakPosition211";
            this.lable_leakPosition211.Size = new System.Drawing.Size(11, 12);
            this.lable_leakPosition211.TabIndex = 16;
            this.lable_leakPosition211.Text = "-";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(261, 367);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(11, 12);
            this.label16.TabIndex = 25;
            this.label16.Text = "-";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(261, 324);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(11, 12);
            this.label17.TabIndex = 24;
            this.label17.Text = "-";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(261, 283);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(11, 12);
            this.label18.TabIndex = 23;
            this.label18.Text = "-";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(261, 241);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(11, 12);
            this.label19.TabIndex = 22;
            this.label19.Text = "-";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(261, 199);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(11, 12);
            this.label20.TabIndex = 21;
            this.label20.Text = "-";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(338, 367);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(11, 12);
            this.label21.TabIndex = 30;
            this.label21.Text = "-";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(338, 324);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(11, 12);
            this.label22.TabIndex = 29;
            this.label22.Text = "-";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(338, 283);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(11, 12);
            this.label23.TabIndex = 28;
            this.label23.Text = "-";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(338, 241);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(11, 12);
            this.label24.TabIndex = 27;
            this.label24.Text = "-";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(338, 199);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(11, 12);
            this.label25.TabIndex = 26;
            this.label25.Text = "-";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(401, 367);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(11, 12);
            this.label26.TabIndex = 35;
            this.label26.Text = "-";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(401, 324);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(11, 12);
            this.label27.TabIndex = 34;
            this.label27.Text = "-";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(401, 283);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(11, 12);
            this.label28.TabIndex = 33;
            this.label28.Text = "-";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(401, 241);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(11, 12);
            this.label29.TabIndex = 32;
            this.label29.Text = "-";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(401, 199);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(11, 12);
            this.label30.TabIndex = 31;
            this.label30.Text = "-";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(478, 367);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(11, 12);
            this.label31.TabIndex = 40;
            this.label31.Text = "-";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(478, 324);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(11, 12);
            this.label32.TabIndex = 39;
            this.label32.Text = "-";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(478, 283);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(11, 12);
            this.label33.TabIndex = 38;
            this.label33.Text = "-";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(478, 241);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(11, 12);
            this.label34.TabIndex = 37;
            this.label34.Text = "-";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(478, 199);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(11, 12);
            this.label35.TabIndex = 36;
            this.label35.Text = "-";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(313, 162);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(68, 24);
            this.label36.TabIndex = 45;
            this.label36.Text = "R/G \r\nImpedence";
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(247, 162);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(45, 24);
            this.label37.TabIndex = 44;
            this.label37.Text = "Sensor\r\nLength";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(160, 166);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(77, 12);
            this.label38.TabIndex = 43;
            this.label38.Text = "LeakPosition";
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(86, 166);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(46, 12);
            this.label39.TabIndex = 42;
            this.label39.Text = "Evt상태";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(31, 166);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(29, 12);
            this.label40.TabIndex = 41;
            this.label40.Text = "탱크";
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(387, 162);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(68, 24);
            this.label41.TabIndex = 46;
            this.label41.Text = "Y/B\r\nImpedence";
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(461, 166);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(61, 12);
            this.label42.TabIndex = 47;
            this.label42.Text = "SenseCur";
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Location = new System.Drawing.Point(609, 162);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(51, 12);
            this.label43.TabIndex = 59;
            this.label43.Text = "Y/B Cur";
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(535, 162);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(52, 12);
            this.label44.TabIndex = 58;
            this.label44.Text = "R/G Cur";
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(625, 367);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(11, 12);
            this.label45.TabIndex = 57;
            this.label45.Text = "-";
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Location = new System.Drawing.Point(625, 324);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(11, 12);
            this.label46.TabIndex = 56;
            this.label46.Text = "-";
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Location = new System.Drawing.Point(625, 283);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(11, 12);
            this.label47.TabIndex = 55;
            this.label47.Text = "-";
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Location = new System.Drawing.Point(625, 241);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(11, 12);
            this.label48.TabIndex = 54;
            this.label48.Text = "-";
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Location = new System.Drawing.Point(625, 199);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(11, 12);
            this.label49.TabIndex = 53;
            this.label49.Text = "-";
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Location = new System.Drawing.Point(551, 367);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(11, 12);
            this.label50.TabIndex = 52;
            this.label50.Text = "-";
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Location = new System.Drawing.Point(551, 324);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(11, 12);
            this.label51.TabIndex = 51;
            this.label51.Text = "-";
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Location = new System.Drawing.Point(551, 283);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(11, 12);
            this.label52.TabIndex = 50;
            this.label52.Text = "-";
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Location = new System.Drawing.Point(551, 241);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(11, 12);
            this.label53.TabIndex = 49;
            this.label53.Text = "-";
            // 
            // label54
            // 
            this.label54.AutoSize = true;
            this.label54.Location = new System.Drawing.Point(551, 199);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(11, 12);
            this.label54.TabIndex = 48;
            this.label54.Text = "-";
            // 
            // button_1
            // 
            this.button_1.Location = new System.Drawing.Point(66, 19);
            this.button_1.Name = "button_1";
            this.button_1.Size = new System.Drawing.Size(193, 23);
            this.button_1.TabIndex = 60;
            this.button_1.Text = "Relay/알람설정모드";
            this.button_1.UseVisualStyleBackColor = true;
            this.button_1.Click += new System.EventHandler(this.button_1_Click);
            // 
            // button_2
            // 
            this.button_2.Location = new System.Drawing.Point(66, 48);
            this.button_2.Name = "button_2";
            this.button_2.Size = new System.Drawing.Size(193, 23);
            this.button_2.TabIndex = 61;
            this.button_2.Text = "Buzzer강제알람상태(정지)";
            this.button_2.UseVisualStyleBackColor = true;
            this.button_2.Click += new System.EventHandler(this.button_2_Click);
            // 
            // button_3
            // 
            this.button_3.Location = new System.Drawing.Point(66, 77);
            this.button_3.Name = "button_3";
            this.button_3.Size = new System.Drawing.Size(193, 23);
            this.button_3.TabIndex = 62;
            this.button_3.Text = "Reset Mode";
            this.button_3.UseVisualStyleBackColor = true;
            this.button_3.Click += new System.EventHandler(this.button_3_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.button_1);
            this.groupBox1.Controls.Add(this.button_3);
            this.groupBox1.Controls.Add(this.button_2);
            this.groupBox1.Location = new System.Drawing.Point(480, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(265, 109);
            this.groupBox1.TabIndex = 63;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Send";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(7, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 60);
            this.label1.TabIndex = 64;
            this.label1.Text = "211-11\r\n212-12\r\n214-13\r\n215-14\r\n216-15";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(7, 20);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(53, 21);
            this.textBox1.TabIndex = 63;
            this.textBox1.Text = "11";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "황산탱크서버";
            this.notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.열기ToolStripMenuItem,
            this.종료ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(99, 48);
            // 
            // 열기ToolStripMenuItem
            // 
            this.열기ToolStripMenuItem.Name = "열기ToolStripMenuItem";
            this.열기ToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.열기ToolStripMenuItem.Text = "열기";
            this.열기ToolStripMenuItem.Click += new System.EventHandler(this.열기ToolStripMenuItem_Click);
            // 
            // 종료ToolStripMenuItem
            // 
            this.종료ToolStripMenuItem.Name = "종료ToolStripMenuItem";
            this.종료ToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.종료ToolStripMenuItem.Text = "종료";
            this.종료ToolStripMenuItem.Click += new System.EventHandler(this.종료ToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(305, 62);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label43);
            this.Controls.Add(this.label44);
            this.Controls.Add(this.label45);
            this.Controls.Add(this.label46);
            this.Controls.Add(this.label47);
            this.Controls.Add(this.label48);
            this.Controls.Add(this.label49);
            this.Controls.Add(this.label50);
            this.Controls.Add(this.label51);
            this.Controls.Add(this.label52);
            this.Controls.Add(this.label53);
            this.Controls.Add(this.label54);
            this.Controls.Add(this.label42);
            this.Controls.Add(this.label41);
            this.Controls.Add(this.label36);
            this.Controls.Add(this.label37);
            this.Controls.Add(this.label38);
            this.Controls.Add(this.label39);
            this.Controls.Add(this.label40);
            this.Controls.Add(this.label31);
            this.Controls.Add(this.label32);
            this.Controls.Add(this.label33);
            this.Controls.Add(this.label34);
            this.Controls.Add(this.label35);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.label28);
            this.Controls.Add(this.label29);
            this.Controls.Add(this.label30);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.lable_leakPosition216);
            this.Controls.Add(this.lable_leakPosition215);
            this.Controls.Add(this.lable_leakPosition214);
            this.Controls.Add(this.lable_leakPosition212);
            this.Controls.Add(this.lable_leakPosition211);
            this.Controls.Add(this.lable_evtStatus216);
            this.Controls.Add(this.lable_evtStatus215);
            this.Controls.Add(this.lable_evtStatus214);
            this.Controls.Add(this.lable_evtStatus212);
            this.Controls.Add(this.lable_evtStatus211);
            this.Controls.Add(this.label_216);
            this.Controls.Add(this.label_215);
            this.Controls.Add(this.label_214);
            this.Controls.Add(this.label_212);
            this.Controls.Add(this.label_211);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "PSM 탱크레벨서버";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

            }

            #endregion

            private System.Windows.Forms.Button button1;
            private System.Windows.Forms.Button button6;
            private System.Windows.Forms.Timer timer1;
            private System.Windows.Forms.Label label_211;
            private System.Windows.Forms.Label label_212;
            private System.Windows.Forms.Label label_214;
            private System.Windows.Forms.Label label_215;
            private System.Windows.Forms.Label label_216;
            private System.Windows.Forms.Label lable_leakPosition216;
            private System.Windows.Forms.Label lable_leakPosition215;
            private System.Windows.Forms.Label lable_leakPosition214;
            private System.Windows.Forms.Label lable_leakPosition212;
            private System.Windows.Forms.Label lable_leakPosition211;
            private System.Windows.Forms.Label label16;
            private System.Windows.Forms.Label label17;
            private System.Windows.Forms.Label label18;
            private System.Windows.Forms.Label label19;
            private System.Windows.Forms.Label label20;
            private System.Windows.Forms.Label label21;
            private System.Windows.Forms.Label label22;
            private System.Windows.Forms.Label label23;
            private System.Windows.Forms.Label label24;
            private System.Windows.Forms.Label label25;
            private System.Windows.Forms.Label label26;
            private System.Windows.Forms.Label label27;
            private System.Windows.Forms.Label label28;
            private System.Windows.Forms.Label label29;
            private System.Windows.Forms.Label label30;
            private System.Windows.Forms.Label label31;
            private System.Windows.Forms.Label label32;
            private System.Windows.Forms.Label label33;
            private System.Windows.Forms.Label label34;
            private System.Windows.Forms.Label label35;
            private System.Windows.Forms.Label label36;
            private System.Windows.Forms.Label label37;
            private System.Windows.Forms.Label label38;
            private System.Windows.Forms.Label label39;
            private System.Windows.Forms.Label label40;
            private System.Windows.Forms.Label label41;
            private System.Windows.Forms.Label label42;
            private System.Windows.Forms.Label label43;
            private System.Windows.Forms.Label label44;
            private System.Windows.Forms.Label label45;
            private System.Windows.Forms.Label label46;
            private System.Windows.Forms.Label label47;
            private System.Windows.Forms.Label label48;
            private System.Windows.Forms.Label label49;
            private System.Windows.Forms.Label label50;
            private System.Windows.Forms.Label label51;
            private System.Windows.Forms.Label label52;
            private System.Windows.Forms.Label label53;
            private System.Windows.Forms.Label label54;
            private System.Windows.Forms.Button button_1;
            private System.Windows.Forms.Button button_2;
            private System.Windows.Forms.Button button_3;
            private System.Windows.Forms.GroupBox groupBox1;
            private System.Windows.Forms.TextBox textBox1;
            private System.Windows.Forms.Label label1;
            private System.Windows.Forms.Label lable_evtStatus216;
            private System.Windows.Forms.Label lable_evtStatus215;
            private System.Windows.Forms.Label lable_evtStatus214;
            private System.Windows.Forms.Label lable_evtStatus212;
            private System.Windows.Forms.Label lable_evtStatus211;
            private System.Windows.Forms.NotifyIcon notifyIcon1;
            private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
            private System.Windows.Forms.ToolStripMenuItem 열기ToolStripMenuItem;
            private System.Windows.Forms.ToolStripMenuItem 종료ToolStripMenuItem;
        }
 }

#endif