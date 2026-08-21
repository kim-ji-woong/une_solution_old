namespace GDK_tester
{
    partial class form_live
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(form_live));
            this.STC_SCREEN = new System.Windows.Forms.Label();
            this.GRP_CONTROL_COLOR = new System.Windows.Forms.GroupBox();
            this.CONTROL_COLOR_BRIGHT_DEC = new GDK_tester.button_dwell();
            this.CONTROL_COLOR_CONTRAST_DEC = new GDK_tester.button_dwell();
            this.CONTROL_COLOR_HUE_DEC = new GDK_tester.button_dwell();
            this.CONTROL_COLOR_BRIGHT_INC = new GDK_tester.button_dwell();
            this.CONTROL_COLOR_SATURATION_INC = new GDK_tester.button_dwell();
            this.CONTROL_COLOR_HUE_INC = new GDK_tester.button_dwell();
            this.CONTROL_COLOR_SATURATION_DEC = new GDK_tester.button_dwell();
            this.CONTROL_COLOR_CONTRAST_INC = new GDK_tester.button_dwell();
            this.CONTROL_COLOR_RESET = new System.Windows.Forms.Button();
            this.GRP_CONTROL_PTZ = new System.Windows.Forms.GroupBox();
            this.CONTROL_PTZ_OSD = new System.Windows.Forms.Button();
            this.CONTROL_PTZ_MOVE_NW = new GDK_tester.button_dwell();
            this.CONTROL_PTZ_MOVE_N = new GDK_tester.button_dwell();
            this.CONTROL_PTZ_MOVE_NE = new GDK_tester.button_dwell();
            this.CONTROL_PTZ_MOVE_W = new GDK_tester.button_dwell();
            this.CONTROL_PTZ_MOVE_E = new GDK_tester.button_dwell();
            this.CONTROL_PTZ_MOVE_SW = new GDK_tester.button_dwell();
            this.CONTROL_PTZ_MOVE_S = new GDK_tester.button_dwell();
            this.CONTROL_PTZ_MOVE_SE = new GDK_tester.button_dwell();
            this.CONTROL_PTZ_ZOOM_INC = new GDK_tester.button_dwell();
            this.CONTROL_PTZ_ZOOM_DEC = new GDK_tester.button_dwell();
            this.CONTROL_PTZ_IRIS_INC = new GDK_tester.button_dwell();
            this.CONTROL_PTZ_IRIS_DEC = new GDK_tester.button_dwell();
            this.CONTROL_PTZ_FOCUS_INC = new GDK_tester.button_dwell();
            this.CONTROL_PTZ_FOCUS_DEC = new GDK_tester.button_dwell();
            this.CONTROL_PTZ_PRESET_SAVE = new System.Windows.Forms.Button();
            this.CONTROL_PTZ_PRESET_MOVE = new System.Windows.Forms.Button();
            this.CONTROL_PTZ_ONE_PUSH = new System.Windows.Forms.Button();
            this.CONTROL_PTZ_MENU = new System.Windows.Forms.Button();
            this.GRP_CONTROL_AUDIO = new System.Windows.Forms.GroupBox();
            this.CONTROL_AUDIO_OUT = new System.Windows.Forms.CheckBox();
            this.CONTROL_AUDIO_IN = new System.Windows.Forms.CheckBox();
            this.GRP_CONTROL_STREAM = new System.Windows.Forms.GroupBox();
            this.CONTROL_MULTI_STREAM = new System.Windows.Forms.Button();
            this.CONTROL_MULTI_STREAM_CHK_TRANSFER = new System.Windows.Forms.CheckBox();
            this.GRP_CONTROL_ALARM_OUT = new System.Windows.Forms.GroupBox();
            this.CONTROL_ALARM_OFF = new GDK_tester.button_dwell();
            this.CONTROL_ALARM_ON = new GDK_tester.button_dwell();
            this.CONTROL_ALARM_CHECK_LIST = new System.Windows.Forms.CheckedListBox();
            this.GRP_CONTROL_BEEP = new System.Windows.Forms.GroupBox();
            this.CONTROL_BEEP_OFF = new GDK_tester.button_dwell();
            this.CONTROL_BEEP_ON = new GDK_tester.button_dwell();
            this.CONTROL_BEEP_CHECK_LIST = new System.Windows.Forms.CheckedListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.BTN_INST_REC_END = new System.Windows.Forms.Button();
            this.BTN_INST_REC_START = new System.Windows.Forms.Button();
            this.GRP_CONTROL_COLOR.SuspendLayout();
            this.GRP_CONTROL_PTZ.SuspendLayout();
            this.GRP_CONTROL_AUDIO.SuspendLayout();
            this.GRP_CONTROL_STREAM.SuspendLayout();
            this.GRP_CONTROL_ALARM_OUT.SuspendLayout();
            this.GRP_CONTROL_BEEP.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // STC_SCREEN
            // 
            this.STC_SCREEN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.STC_SCREEN.Location = new System.Drawing.Point(9, 11);
            this.STC_SCREEN.Name = "STC_SCREEN";
            this.STC_SCREEN.Size = new System.Drawing.Size(680, 414);
            this.STC_SCREEN.TabIndex = 0;
            this.STC_SCREEN.Text = "screen";
            this.STC_SCREEN.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.STC_SCREEN.Visible = false;
            // 
            // GRP_CONTROL_COLOR
            // 
            this.GRP_CONTROL_COLOR.Controls.Add(this.CONTROL_COLOR_BRIGHT_DEC);
            this.GRP_CONTROL_COLOR.Controls.Add(this.CONTROL_COLOR_CONTRAST_DEC);
            this.GRP_CONTROL_COLOR.Controls.Add(this.CONTROL_COLOR_HUE_DEC);
            this.GRP_CONTROL_COLOR.Controls.Add(this.CONTROL_COLOR_BRIGHT_INC);
            this.GRP_CONTROL_COLOR.Controls.Add(this.CONTROL_COLOR_SATURATION_INC);
            this.GRP_CONTROL_COLOR.Controls.Add(this.CONTROL_COLOR_HUE_INC);
            this.GRP_CONTROL_COLOR.Controls.Add(this.CONTROL_COLOR_SATURATION_DEC);
            this.GRP_CONTROL_COLOR.Controls.Add(this.CONTROL_COLOR_CONTRAST_INC);
            this.GRP_CONTROL_COLOR.Controls.Add(this.CONTROL_COLOR_RESET);
            this.GRP_CONTROL_COLOR.Location = new System.Drawing.Point(9, 428);
            this.GRP_CONTROL_COLOR.Name = "GRP_CONTROL_COLOR";
            this.GRP_CONTROL_COLOR.Size = new System.Drawing.Size(130, 110);
            this.GRP_CONTROL_COLOR.TabIndex = 8;
            this.GRP_CONTROL_COLOR.TabStop = false;
            this.GRP_CONTROL_COLOR.Text = "Color";
            // 
            // CONTROL_COLOR_BRIGHT_DEC
            // 
            this.CONTROL_COLOR_BRIGHT_DEC.Enabled = false;
            this.CONTROL_COLOR_BRIGHT_DEC.Location = new System.Drawing.Point(7, 17);
            this.CONTROL_COLOR_BRIGHT_DEC.Name = "CONTROL_COLOR_BRIGHT_DEC";
            this.CONTROL_COLOR_BRIGHT_DEC.Size = new System.Drawing.Size(29, 23);
            this.CONTROL_COLOR_BRIGHT_DEC.TabIndex = 0;
            this.CONTROL_COLOR_BRIGHT_DEC.Text = "b-";
            this.CONTROL_COLOR_BRIGHT_DEC.UseVisualStyleBackColor = true;
            this.CONTROL_COLOR_BRIGHT_DEC.handler += new System.EventHandler(this.on_feature_control_color);
            // 
            // CONTROL_COLOR_CONTRAST_DEC
            // 
            this.CONTROL_COLOR_CONTRAST_DEC.Enabled = false;
            this.CONTROL_COLOR_CONTRAST_DEC.Location = new System.Drawing.Point(36, 17);
            this.CONTROL_COLOR_CONTRAST_DEC.Name = "CONTROL_COLOR_CONTRAST_DEC";
            this.CONTROL_COLOR_CONTRAST_DEC.Size = new System.Drawing.Size(29, 23);
            this.CONTROL_COLOR_CONTRAST_DEC.TabIndex = 2;
            this.CONTROL_COLOR_CONTRAST_DEC.Text = "c-";
            this.CONTROL_COLOR_CONTRAST_DEC.UseVisualStyleBackColor = true;
            this.CONTROL_COLOR_CONTRAST_DEC.handler += new System.EventHandler(this.on_feature_control_color);
            // 
            // CONTROL_COLOR_HUE_DEC
            // 
            this.CONTROL_COLOR_HUE_DEC.Enabled = false;
            this.CONTROL_COLOR_HUE_DEC.Location = new System.Drawing.Point(94, 17);
            this.CONTROL_COLOR_HUE_DEC.Name = "CONTROL_COLOR_HUE_DEC";
            this.CONTROL_COLOR_HUE_DEC.Size = new System.Drawing.Size(29, 23);
            this.CONTROL_COLOR_HUE_DEC.TabIndex = 6;
            this.CONTROL_COLOR_HUE_DEC.Text = "h-";
            this.CONTROL_COLOR_HUE_DEC.UseVisualStyleBackColor = true;
            this.CONTROL_COLOR_HUE_DEC.handler += new System.EventHandler(this.on_feature_control_color);
            // 
            // CONTROL_COLOR_BRIGHT_INC
            // 
            this.CONTROL_COLOR_BRIGHT_INC.Enabled = false;
            this.CONTROL_COLOR_BRIGHT_INC.Location = new System.Drawing.Point(7, 44);
            this.CONTROL_COLOR_BRIGHT_INC.Name = "CONTROL_COLOR_BRIGHT_INC";
            this.CONTROL_COLOR_BRIGHT_INC.Size = new System.Drawing.Size(29, 23);
            this.CONTROL_COLOR_BRIGHT_INC.TabIndex = 1;
            this.CONTROL_COLOR_BRIGHT_INC.Text = "b+";
            this.CONTROL_COLOR_BRIGHT_INC.UseVisualStyleBackColor = true;
            this.CONTROL_COLOR_BRIGHT_INC.handler += new System.EventHandler(this.on_feature_control_color);
            // 
            // CONTROL_COLOR_SATURATION_INC
            // 
            this.CONTROL_COLOR_SATURATION_INC.Enabled = false;
            this.CONTROL_COLOR_SATURATION_INC.Location = new System.Drawing.Point(65, 44);
            this.CONTROL_COLOR_SATURATION_INC.Name = "CONTROL_COLOR_SATURATION_INC";
            this.CONTROL_COLOR_SATURATION_INC.Size = new System.Drawing.Size(29, 23);
            this.CONTROL_COLOR_SATURATION_INC.TabIndex = 5;
            this.CONTROL_COLOR_SATURATION_INC.Text = "s+";
            this.CONTROL_COLOR_SATURATION_INC.UseVisualStyleBackColor = true;
            this.CONTROL_COLOR_SATURATION_INC.handler += new System.EventHandler(this.on_feature_control_color);
            // 
            // CONTROL_COLOR_HUE_INC
            // 
            this.CONTROL_COLOR_HUE_INC.Enabled = false;
            this.CONTROL_COLOR_HUE_INC.Location = new System.Drawing.Point(94, 44);
            this.CONTROL_COLOR_HUE_INC.Name = "CONTROL_COLOR_HUE_INC";
            this.CONTROL_COLOR_HUE_INC.Size = new System.Drawing.Size(29, 23);
            this.CONTROL_COLOR_HUE_INC.TabIndex = 7;
            this.CONTROL_COLOR_HUE_INC.Text = "h+";
            this.CONTROL_COLOR_HUE_INC.UseVisualStyleBackColor = true;
            this.CONTROL_COLOR_HUE_INC.handler += new System.EventHandler(this.on_feature_control_color);
            // 
            // CONTROL_COLOR_SATURATION_DEC
            // 
            this.CONTROL_COLOR_SATURATION_DEC.Enabled = false;
            this.CONTROL_COLOR_SATURATION_DEC.Location = new System.Drawing.Point(65, 17);
            this.CONTROL_COLOR_SATURATION_DEC.Name = "CONTROL_COLOR_SATURATION_DEC";
            this.CONTROL_COLOR_SATURATION_DEC.Size = new System.Drawing.Size(29, 23);
            this.CONTROL_COLOR_SATURATION_DEC.TabIndex = 4;
            this.CONTROL_COLOR_SATURATION_DEC.Text = "s-";
            this.CONTROL_COLOR_SATURATION_DEC.UseVisualStyleBackColor = true;
            this.CONTROL_COLOR_SATURATION_DEC.handler += new System.EventHandler(this.on_feature_control_color);
            // 
            // CONTROL_COLOR_CONTRAST_INC
            // 
            this.CONTROL_COLOR_CONTRAST_INC.Enabled = false;
            this.CONTROL_COLOR_CONTRAST_INC.Location = new System.Drawing.Point(36, 44);
            this.CONTROL_COLOR_CONTRAST_INC.Name = "CONTROL_COLOR_CONTRAST_INC";
            this.CONTROL_COLOR_CONTRAST_INC.Size = new System.Drawing.Size(29, 23);
            this.CONTROL_COLOR_CONTRAST_INC.TabIndex = 3;
            this.CONTROL_COLOR_CONTRAST_INC.Text = "c+";
            this.CONTROL_COLOR_CONTRAST_INC.UseVisualStyleBackColor = true;
            this.CONTROL_COLOR_CONTRAST_INC.handler += new System.EventHandler(this.on_feature_control_color);
            // 
            // CONTROL_COLOR_RESET
            // 
            this.CONTROL_COLOR_RESET.Enabled = false;
            this.CONTROL_COLOR_RESET.Location = new System.Drawing.Point(7, 71);
            this.CONTROL_COLOR_RESET.Name = "CONTROL_COLOR_RESET";
            this.CONTROL_COLOR_RESET.Size = new System.Drawing.Size(58, 23);
            this.CONTROL_COLOR_RESET.TabIndex = 8;
            this.CONTROL_COLOR_RESET.Text = "reset";
            this.CONTROL_COLOR_RESET.UseVisualStyleBackColor = true;
            this.CONTROL_COLOR_RESET.Click += new System.EventHandler(this.on_feature_control_color);
            // 
            // GRP_CONTROL_PTZ
            // 
            this.GRP_CONTROL_PTZ.Controls.Add(this.CONTROL_PTZ_OSD);
            this.GRP_CONTROL_PTZ.Controls.Add(this.CONTROL_PTZ_MOVE_NW);
            this.GRP_CONTROL_PTZ.Controls.Add(this.CONTROL_PTZ_MOVE_N);
            this.GRP_CONTROL_PTZ.Controls.Add(this.CONTROL_PTZ_MOVE_NE);
            this.GRP_CONTROL_PTZ.Controls.Add(this.CONTROL_PTZ_MOVE_W);
            this.GRP_CONTROL_PTZ.Controls.Add(this.CONTROL_PTZ_MOVE_E);
            this.GRP_CONTROL_PTZ.Controls.Add(this.CONTROL_PTZ_MOVE_SW);
            this.GRP_CONTROL_PTZ.Controls.Add(this.CONTROL_PTZ_MOVE_S);
            this.GRP_CONTROL_PTZ.Controls.Add(this.CONTROL_PTZ_MOVE_SE);
            this.GRP_CONTROL_PTZ.Controls.Add(this.CONTROL_PTZ_ZOOM_INC);
            this.GRP_CONTROL_PTZ.Controls.Add(this.CONTROL_PTZ_ZOOM_DEC);
            this.GRP_CONTROL_PTZ.Controls.Add(this.CONTROL_PTZ_IRIS_INC);
            this.GRP_CONTROL_PTZ.Controls.Add(this.CONTROL_PTZ_IRIS_DEC);
            this.GRP_CONTROL_PTZ.Controls.Add(this.CONTROL_PTZ_FOCUS_INC);
            this.GRP_CONTROL_PTZ.Controls.Add(this.CONTROL_PTZ_FOCUS_DEC);
            this.GRP_CONTROL_PTZ.Controls.Add(this.CONTROL_PTZ_PRESET_SAVE);
            this.GRP_CONTROL_PTZ.Controls.Add(this.CONTROL_PTZ_PRESET_MOVE);
            this.GRP_CONTROL_PTZ.Controls.Add(this.CONTROL_PTZ_ONE_PUSH);
            this.GRP_CONTROL_PTZ.Controls.Add(this.CONTROL_PTZ_MENU);
            this.GRP_CONTROL_PTZ.Location = new System.Drawing.Point(9, 544);
            this.GRP_CONTROL_PTZ.Name = "GRP_CONTROL_PTZ";
            this.GRP_CONTROL_PTZ.Size = new System.Drawing.Size(209, 120);
            this.GRP_CONTROL_PTZ.TabIndex = 9;
            this.GRP_CONTROL_PTZ.TabStop = false;
            this.GRP_CONTROL_PTZ.Text = "PTZ";
            // 
            // CONTROL_PTZ_OSD
            // 
            this.CONTROL_PTZ_OSD.Enabled = false;
            this.CONTROL_PTZ_OSD.Location = new System.Drawing.Point(47, 42);
            this.CONTROL_PTZ_OSD.Name = "CONTROL_PTZ_OSD";
            this.CONTROL_PTZ_OSD.Size = new System.Drawing.Size(18, 20);
            this.CONTROL_PTZ_OSD.TabIndex = 19;
            this.CONTROL_PTZ_OSD.UseVisualStyleBackColor = true;
            this.CONTROL_PTZ_OSD.Click += new System.EventHandler(this.on_feature_control_PTZ_function);
            // 
            // CONTROL_PTZ_MOVE_NW
            // 
            this.CONTROL_PTZ_MOVE_NW.Enabled = false;
            this.CONTROL_PTZ_MOVE_NW.Location = new System.Drawing.Point(6, 15);
            this.CONTROL_PTZ_MOVE_NW.Name = "CONTROL_PTZ_MOVE_NW";
            this.CONTROL_PTZ_MOVE_NW.Size = new System.Drawing.Size(33, 23);
            this.CONTROL_PTZ_MOVE_NW.TabIndex = 0;
            this.CONTROL_PTZ_MOVE_NW.Text = "NW";
            this.CONTROL_PTZ_MOVE_NW.UseVisualStyleBackColor = true;
            this.CONTROL_PTZ_MOVE_NW.handler += new System.EventHandler(this.on_feature_control_PTZ_move);
            // 
            // CONTROL_PTZ_MOVE_N
            // 
            this.CONTROL_PTZ_MOVE_N.Enabled = false;
            this.CONTROL_PTZ_MOVE_N.Location = new System.Drawing.Point(40, 15);
            this.CONTROL_PTZ_MOVE_N.Name = "CONTROL_PTZ_MOVE_N";
            this.CONTROL_PTZ_MOVE_N.Size = new System.Drawing.Size(33, 23);
            this.CONTROL_PTZ_MOVE_N.TabIndex = 1;
            this.CONTROL_PTZ_MOVE_N.Text = "N";
            this.CONTROL_PTZ_MOVE_N.UseVisualStyleBackColor = true;
            this.CONTROL_PTZ_MOVE_N.handler += new System.EventHandler(this.on_feature_control_PTZ_move);
            // 
            // CONTROL_PTZ_MOVE_NE
            // 
            this.CONTROL_PTZ_MOVE_NE.Enabled = false;
            this.CONTROL_PTZ_MOVE_NE.Location = new System.Drawing.Point(73, 15);
            this.CONTROL_PTZ_MOVE_NE.Name = "CONTROL_PTZ_MOVE_NE";
            this.CONTROL_PTZ_MOVE_NE.Size = new System.Drawing.Size(33, 23);
            this.CONTROL_PTZ_MOVE_NE.TabIndex = 2;
            this.CONTROL_PTZ_MOVE_NE.Text = "NE";
            this.CONTROL_PTZ_MOVE_NE.UseVisualStyleBackColor = true;
            this.CONTROL_PTZ_MOVE_NE.handler += new System.EventHandler(this.on_feature_control_PTZ_move);
            // 
            // CONTROL_PTZ_MOVE_W
            // 
            this.CONTROL_PTZ_MOVE_W.Enabled = false;
            this.CONTROL_PTZ_MOVE_W.Location = new System.Drawing.Point(6, 40);
            this.CONTROL_PTZ_MOVE_W.Name = "CONTROL_PTZ_MOVE_W";
            this.CONTROL_PTZ_MOVE_W.Size = new System.Drawing.Size(33, 23);
            this.CONTROL_PTZ_MOVE_W.TabIndex = 3;
            this.CONTROL_PTZ_MOVE_W.Text = "W";
            this.CONTROL_PTZ_MOVE_W.UseVisualStyleBackColor = true;
            this.CONTROL_PTZ_MOVE_W.handler += new System.EventHandler(this.on_feature_control_PTZ_move);
            // 
            // CONTROL_PTZ_MOVE_E
            // 
            this.CONTROL_PTZ_MOVE_E.Enabled = false;
            this.CONTROL_PTZ_MOVE_E.Location = new System.Drawing.Point(73, 40);
            this.CONTROL_PTZ_MOVE_E.Name = "CONTROL_PTZ_MOVE_E";
            this.CONTROL_PTZ_MOVE_E.Size = new System.Drawing.Size(33, 23);
            this.CONTROL_PTZ_MOVE_E.TabIndex = 4;
            this.CONTROL_PTZ_MOVE_E.Text = "E";
            this.CONTROL_PTZ_MOVE_E.UseVisualStyleBackColor = true;
            this.CONTROL_PTZ_MOVE_E.handler += new System.EventHandler(this.on_feature_control_PTZ_move);
            // 
            // CONTROL_PTZ_MOVE_SW
            // 
            this.CONTROL_PTZ_MOVE_SW.Enabled = false;
            this.CONTROL_PTZ_MOVE_SW.Location = new System.Drawing.Point(6, 65);
            this.CONTROL_PTZ_MOVE_SW.Name = "CONTROL_PTZ_MOVE_SW";
            this.CONTROL_PTZ_MOVE_SW.Size = new System.Drawing.Size(33, 23);
            this.CONTROL_PTZ_MOVE_SW.TabIndex = 5;
            this.CONTROL_PTZ_MOVE_SW.Text = "SW";
            this.CONTROL_PTZ_MOVE_SW.UseVisualStyleBackColor = true;
            this.CONTROL_PTZ_MOVE_SW.handler += new System.EventHandler(this.on_feature_control_PTZ_move);
            // 
            // CONTROL_PTZ_MOVE_S
            // 
            this.CONTROL_PTZ_MOVE_S.Enabled = false;
            this.CONTROL_PTZ_MOVE_S.Location = new System.Drawing.Point(40, 65);
            this.CONTROL_PTZ_MOVE_S.Name = "CONTROL_PTZ_MOVE_S";
            this.CONTROL_PTZ_MOVE_S.Size = new System.Drawing.Size(33, 23);
            this.CONTROL_PTZ_MOVE_S.TabIndex = 6;
            this.CONTROL_PTZ_MOVE_S.Text = "S";
            this.CONTROL_PTZ_MOVE_S.UseVisualStyleBackColor = true;
            this.CONTROL_PTZ_MOVE_S.handler += new System.EventHandler(this.on_feature_control_PTZ_move);
            // 
            // CONTROL_PTZ_MOVE_SE
            // 
            this.CONTROL_PTZ_MOVE_SE.Enabled = false;
            this.CONTROL_PTZ_MOVE_SE.Location = new System.Drawing.Point(73, 65);
            this.CONTROL_PTZ_MOVE_SE.Name = "CONTROL_PTZ_MOVE_SE";
            this.CONTROL_PTZ_MOVE_SE.Size = new System.Drawing.Size(33, 23);
            this.CONTROL_PTZ_MOVE_SE.TabIndex = 7;
            this.CONTROL_PTZ_MOVE_SE.Text = "SE";
            this.CONTROL_PTZ_MOVE_SE.UseVisualStyleBackColor = true;
            this.CONTROL_PTZ_MOVE_SE.handler += new System.EventHandler(this.on_feature_control_PTZ_move);
            // 
            // CONTROL_PTZ_ZOOM_INC
            // 
            this.CONTROL_PTZ_ZOOM_INC.Enabled = false;
            this.CONTROL_PTZ_ZOOM_INC.Location = new System.Drawing.Point(114, 38);
            this.CONTROL_PTZ_ZOOM_INC.Name = "CONTROL_PTZ_ZOOM_INC";
            this.CONTROL_PTZ_ZOOM_INC.Size = new System.Drawing.Size(29, 23);
            this.CONTROL_PTZ_ZOOM_INC.TabIndex = 9;
            this.CONTROL_PTZ_ZOOM_INC.Text = "Z+";
            this.CONTROL_PTZ_ZOOM_INC.UseVisualStyleBackColor = true;
            this.CONTROL_PTZ_ZOOM_INC.handler += new System.EventHandler(this.on_feature_control_PTZ_function);
            // 
            // CONTROL_PTZ_ZOOM_DEC
            // 
            this.CONTROL_PTZ_ZOOM_DEC.Enabled = false;
            this.CONTROL_PTZ_ZOOM_DEC.Location = new System.Drawing.Point(114, 15);
            this.CONTROL_PTZ_ZOOM_DEC.Name = "CONTROL_PTZ_ZOOM_DEC";
            this.CONTROL_PTZ_ZOOM_DEC.Size = new System.Drawing.Size(29, 23);
            this.CONTROL_PTZ_ZOOM_DEC.TabIndex = 8;
            this.CONTROL_PTZ_ZOOM_DEC.Text = "Z-";
            this.CONTROL_PTZ_ZOOM_DEC.UseVisualStyleBackColor = true;
            this.CONTROL_PTZ_ZOOM_DEC.handler += new System.EventHandler(this.on_feature_control_PTZ_function);
            // 
            // CONTROL_PTZ_IRIS_INC
            // 
            this.CONTROL_PTZ_IRIS_INC.Enabled = false;
            this.CONTROL_PTZ_IRIS_INC.Location = new System.Drawing.Point(174, 38);
            this.CONTROL_PTZ_IRIS_INC.Name = "CONTROL_PTZ_IRIS_INC";
            this.CONTROL_PTZ_IRIS_INC.Size = new System.Drawing.Size(29, 23);
            this.CONTROL_PTZ_IRIS_INC.TabIndex = 13;
            this.CONTROL_PTZ_IRIS_INC.Text = "I+";
            this.CONTROL_PTZ_IRIS_INC.UseVisualStyleBackColor = true;
            this.CONTROL_PTZ_IRIS_INC.handler += new System.EventHandler(this.on_feature_control_PTZ_function);
            // 
            // CONTROL_PTZ_IRIS_DEC
            // 
            this.CONTROL_PTZ_IRIS_DEC.Enabled = false;
            this.CONTROL_PTZ_IRIS_DEC.Location = new System.Drawing.Point(174, 15);
            this.CONTROL_PTZ_IRIS_DEC.Name = "CONTROL_PTZ_IRIS_DEC";
            this.CONTROL_PTZ_IRIS_DEC.Size = new System.Drawing.Size(29, 23);
            this.CONTROL_PTZ_IRIS_DEC.TabIndex = 12;
            this.CONTROL_PTZ_IRIS_DEC.Text = "I-";
            this.CONTROL_PTZ_IRIS_DEC.UseVisualStyleBackColor = true;
            this.CONTROL_PTZ_IRIS_DEC.handler += new System.EventHandler(this.on_feature_control_PTZ_function);
            // 
            // CONTROL_PTZ_FOCUS_INC
            // 
            this.CONTROL_PTZ_FOCUS_INC.Enabled = false;
            this.CONTROL_PTZ_FOCUS_INC.Location = new System.Drawing.Point(144, 38);
            this.CONTROL_PTZ_FOCUS_INC.Name = "CONTROL_PTZ_FOCUS_INC";
            this.CONTROL_PTZ_FOCUS_INC.Size = new System.Drawing.Size(29, 23);
            this.CONTROL_PTZ_FOCUS_INC.TabIndex = 11;
            this.CONTROL_PTZ_FOCUS_INC.Text = "F+";
            this.CONTROL_PTZ_FOCUS_INC.UseVisualStyleBackColor = true;
            this.CONTROL_PTZ_FOCUS_INC.handler += new System.EventHandler(this.on_feature_control_PTZ_function);
            // 
            // CONTROL_PTZ_FOCUS_DEC
            // 
            this.CONTROL_PTZ_FOCUS_DEC.Enabled = false;
            this.CONTROL_PTZ_FOCUS_DEC.Location = new System.Drawing.Point(144, 15);
            this.CONTROL_PTZ_FOCUS_DEC.Name = "CONTROL_PTZ_FOCUS_DEC";
            this.CONTROL_PTZ_FOCUS_DEC.Size = new System.Drawing.Size(29, 23);
            this.CONTROL_PTZ_FOCUS_DEC.TabIndex = 10;
            this.CONTROL_PTZ_FOCUS_DEC.Text = "F-";
            this.CONTROL_PTZ_FOCUS_DEC.UseVisualStyleBackColor = true;
            this.CONTROL_PTZ_FOCUS_DEC.handler += new System.EventHandler(this.on_feature_control_PTZ_function);
            // 
            // CONTROL_PTZ_PRESET_SAVE
            // 
            this.CONTROL_PTZ_PRESET_SAVE.Enabled = false;
            this.CONTROL_PTZ_PRESET_SAVE.Location = new System.Drawing.Point(36, 90);
            this.CONTROL_PTZ_PRESET_SAVE.Name = "CONTROL_PTZ_PRESET_SAVE";
            this.CONTROL_PTZ_PRESET_SAVE.Size = new System.Drawing.Size(29, 23);
            this.CONTROL_PTZ_PRESET_SAVE.TabIndex = 15;
            this.CONTROL_PTZ_PRESET_SAVE.Text = "Ps";
            this.CONTROL_PTZ_PRESET_SAVE.UseVisualStyleBackColor = true;
            this.CONTROL_PTZ_PRESET_SAVE.Click += new System.EventHandler(this.on_feature_control_PTZ_preset);
            // 
            // CONTROL_PTZ_PRESET_MOVE
            // 
            this.CONTROL_PTZ_PRESET_MOVE.Enabled = false;
            this.CONTROL_PTZ_PRESET_MOVE.Location = new System.Drawing.Point(6, 90);
            this.CONTROL_PTZ_PRESET_MOVE.Name = "CONTROL_PTZ_PRESET_MOVE";
            this.CONTROL_PTZ_PRESET_MOVE.Size = new System.Drawing.Size(29, 23);
            this.CONTROL_PTZ_PRESET_MOVE.TabIndex = 14;
            this.CONTROL_PTZ_PRESET_MOVE.Text = "Pv";
            this.CONTROL_PTZ_PRESET_MOVE.UseVisualStyleBackColor = true;
            this.CONTROL_PTZ_PRESET_MOVE.Click += new System.EventHandler(this.on_feature_control_PTZ_preset);
            // 
            // CONTROL_PTZ_ONE_PUSH
            // 
            this.CONTROL_PTZ_ONE_PUSH.Enabled = false;
            this.CONTROL_PTZ_ONE_PUSH.Location = new System.Drawing.Point(144, 61);
            this.CONTROL_PTZ_ONE_PUSH.Name = "CONTROL_PTZ_ONE_PUSH";
            this.CONTROL_PTZ_ONE_PUSH.Size = new System.Drawing.Size(29, 23);
            this.CONTROL_PTZ_ONE_PUSH.TabIndex = 16;
            this.CONTROL_PTZ_ONE_PUSH.Text = "AF";
            this.CONTROL_PTZ_ONE_PUSH.UseVisualStyleBackColor = true;
            this.CONTROL_PTZ_ONE_PUSH.Click += new System.EventHandler(this.on_feature_control_PTZ_function);
            // 
            // CONTROL_PTZ_MENU
            // 
            this.CONTROL_PTZ_MENU.Enabled = false;
            this.CONTROL_PTZ_MENU.Location = new System.Drawing.Point(144, 90);
            this.CONTROL_PTZ_MENU.Name = "CONTROL_PTZ_MENU";
            this.CONTROL_PTZ_MENU.Size = new System.Drawing.Size(58, 23);
            this.CONTROL_PTZ_MENU.TabIndex = 17;
            this.CONTROL_PTZ_MENU.Text = "menu";
            this.CONTROL_PTZ_MENU.UseVisualStyleBackColor = true;
            this.CONTROL_PTZ_MENU.Click += new System.EventHandler(this.on_feature_control_PTZ_function);
            // 
            // GRP_CONTROL_AUDIO
            // 
            this.GRP_CONTROL_AUDIO.Controls.Add(this.CONTROL_AUDIO_OUT);
            this.GRP_CONTROL_AUDIO.Controls.Add(this.CONTROL_AUDIO_IN);
            this.GRP_CONTROL_AUDIO.Location = new System.Drawing.Point(145, 488);
            this.GRP_CONTROL_AUDIO.Name = "GRP_CONTROL_AUDIO";
            this.GRP_CONTROL_AUDIO.Size = new System.Drawing.Size(193, 50);
            this.GRP_CONTROL_AUDIO.TabIndex = 10;
            this.GRP_CONTROL_AUDIO.TabStop = false;
            this.GRP_CONTROL_AUDIO.Text = "Audio";
            // 
            // CONTROL_AUDIO_OUT
            // 
            this.CONTROL_AUDIO_OUT.AutoSize = true;
            this.CONTROL_AUDIO_OUT.Enabled = false;
            this.CONTROL_AUDIO_OUT.Location = new System.Drawing.Point(104, 20);
            this.CONTROL_AUDIO_OUT.Margin = new System.Windows.Forms.Padding(0);
            this.CONTROL_AUDIO_OUT.Name = "CONTROL_AUDIO_OUT";
            this.CONTROL_AUDIO_OUT.Size = new System.Drawing.Size(71, 17);
            this.CONTROL_AUDIO_OUT.TabIndex = 1;
            this.CONTROL_AUDIO_OUT.Text = "audio out";
            this.CONTROL_AUDIO_OUT.UseVisualStyleBackColor = true;
            this.CONTROL_AUDIO_OUT.Click += new System.EventHandler(this.on_feature_control_audio_out);
            // 
            // CONTROL_AUDIO_IN
            // 
            this.CONTROL_AUDIO_IN.AutoSize = true;
            this.CONTROL_AUDIO_IN.Enabled = false;
            this.CONTROL_AUDIO_IN.Location = new System.Drawing.Point(24, 20);
            this.CONTROL_AUDIO_IN.Margin = new System.Windows.Forms.Padding(0);
            this.CONTROL_AUDIO_IN.Name = "CONTROL_AUDIO_IN";
            this.CONTROL_AUDIO_IN.Size = new System.Drawing.Size(63, 17);
            this.CONTROL_AUDIO_IN.TabIndex = 0;
            this.CONTROL_AUDIO_IN.Text = "audio in";
            this.CONTROL_AUDIO_IN.UseVisualStyleBackColor = true;
            this.CONTROL_AUDIO_IN.Click += new System.EventHandler(this.on_feature_control_audio_in);
            // 
            // GRP_CONTROL_STREAM
            // 
            this.GRP_CONTROL_STREAM.Controls.Add(this.CONTROL_MULTI_STREAM);
            this.GRP_CONTROL_STREAM.Controls.Add(this.CONTROL_MULTI_STREAM_CHK_TRANSFER);
            this.GRP_CONTROL_STREAM.Location = new System.Drawing.Point(145, 428);
            this.GRP_CONTROL_STREAM.Name = "GRP_CONTROL_STREAM";
            this.GRP_CONTROL_STREAM.Size = new System.Drawing.Size(197, 58);
            this.GRP_CONTROL_STREAM.TabIndex = 11;
            this.GRP_CONTROL_STREAM.TabStop = false;
            this.GRP_CONTROL_STREAM.Text = "Multi-stream";
            // 
            // CONTROL_MULTI_STREAM
            // 
            this.CONTROL_MULTI_STREAM.Enabled = false;
            this.CONTROL_MULTI_STREAM.Location = new System.Drawing.Point(10, 21);
            this.CONTROL_MULTI_STREAM.Name = "CONTROL_MULTI_STREAM";
            this.CONTROL_MULTI_STREAM.Size = new System.Drawing.Size(97, 25);
            this.CONTROL_MULTI_STREAM.TabIndex = 13;
            this.CONTROL_MULTI_STREAM.Text = "multi-stream";
            this.CONTROL_MULTI_STREAM.UseVisualStyleBackColor = true;
            this.CONTROL_MULTI_STREAM.Click += new System.EventHandler(this.on_cotrol_multi_stream);
            // 
            // CONTROL_MULTI_STREAM_CHK_TRANSFER
            // 
            this.CONTROL_MULTI_STREAM_CHK_TRANSFER.AutoSize = true;
            this.CONTROL_MULTI_STREAM_CHK_TRANSFER.Enabled = false;
            this.CONTROL_MULTI_STREAM_CHK_TRANSFER.Location = new System.Drawing.Point(115, 24);
            this.CONTROL_MULTI_STREAM_CHK_TRANSFER.Name = "CONTROL_MULTI_STREAM_CHK_TRANSFER";
            this.CONTROL_MULTI_STREAM_CHK_TRANSFER.Size = new System.Drawing.Size(78, 17);
            this.CONTROL_MULTI_STREAM_CHK_TRANSFER.TabIndex = 12;
            this.CONTROL_MULTI_STREAM_CHK_TRANSFER.Text = "transfer all";
            this.CONTROL_MULTI_STREAM_CHK_TRANSFER.UseVisualStyleBackColor = true;
            this.CONTROL_MULTI_STREAM_CHK_TRANSFER.Click += new System.EventHandler(this.on_control_multi_stream_transfer);
            // 
            // GRP_CONTROL_ALARM_OUT
            // 
            this.GRP_CONTROL_ALARM_OUT.Controls.Add(this.CONTROL_ALARM_OFF);
            this.GRP_CONTROL_ALARM_OUT.Controls.Add(this.CONTROL_ALARM_ON);
            this.GRP_CONTROL_ALARM_OUT.Controls.Add(this.CONTROL_ALARM_CHECK_LIST);
            this.GRP_CONTROL_ALARM_OUT.Location = new System.Drawing.Point(348, 428);
            this.GRP_CONTROL_ALARM_OUT.Name = "GRP_CONTROL_ALARM_OUT";
            this.GRP_CONTROL_ALARM_OUT.Size = new System.Drawing.Size(211, 236);
            this.GRP_CONTROL_ALARM_OUT.TabIndex = 12;
            this.GRP_CONTROL_ALARM_OUT.TabStop = false;
            this.GRP_CONTROL_ALARM_OUT.Text = "Alarm Out";
            // 
            // CONTROL_ALARM_OFF
            // 
            this.CONTROL_ALARM_OFF.Enabled = false;
            this.CONTROL_ALARM_OFF.Location = new System.Drawing.Point(168, 198);
            this.CONTROL_ALARM_OFF.Name = "CONTROL_ALARM_OFF";
            this.CONTROL_ALARM_OFF.Size = new System.Drawing.Size(33, 23);
            this.CONTROL_ALARM_OFF.TabIndex = 21;
            this.CONTROL_ALARM_OFF.Text = "off";
            this.CONTROL_ALARM_OFF.UseVisualStyleBackColor = true;
            this.CONTROL_ALARM_OFF.Click += new System.EventHandler(this.on_feature_control_alarm_out);
            // 
            // CONTROL_ALARM_ON
            // 
            this.CONTROL_ALARM_ON.Enabled = false;
            this.CONTROL_ALARM_ON.Location = new System.Drawing.Point(131, 198);
            this.CONTROL_ALARM_ON.Name = "CONTROL_ALARM_ON";
            this.CONTROL_ALARM_ON.Size = new System.Drawing.Size(33, 23);
            this.CONTROL_ALARM_ON.TabIndex = 20;
            this.CONTROL_ALARM_ON.Text = "on";
            this.CONTROL_ALARM_ON.UseVisualStyleBackColor = true;
            this.CONTROL_ALARM_ON.Click += new System.EventHandler(this.on_feature_control_alarm_out);
            // 
            // CONTROL_ALARM_CHECK_LIST
            // 
            this.CONTROL_ALARM_CHECK_LIST.CheckOnClick = true;
            this.CONTROL_ALARM_CHECK_LIST.Enabled = false;
            this.CONTROL_ALARM_CHECK_LIST.FormattingEnabled = true;
            this.CONTROL_ALARM_CHECK_LIST.HorizontalScrollbar = true;
            this.CONTROL_ALARM_CHECK_LIST.Location = new System.Drawing.Point(8, 20);
            this.CONTROL_ALARM_CHECK_LIST.Name = "CONTROL_ALARM_CHECK_LIST";
            this.CONTROL_ALARM_CHECK_LIST.Size = new System.Drawing.Size(195, 169);
            this.CONTROL_ALARM_CHECK_LIST.TabIndex = 0;
            // 
            // GRP_CONTROL_BEEP
            // 
            this.GRP_CONTROL_BEEP.Controls.Add(this.CONTROL_BEEP_OFF);
            this.GRP_CONTROL_BEEP.Controls.Add(this.CONTROL_BEEP_ON);
            this.GRP_CONTROL_BEEP.Controls.Add(this.CONTROL_BEEP_CHECK_LIST);
            this.GRP_CONTROL_BEEP.Location = new System.Drawing.Point(559, 428);
            this.GRP_CONTROL_BEEP.Name = "GRP_CONTROL_BEEP";
            this.GRP_CONTROL_BEEP.Size = new System.Drawing.Size(130, 236);
            this.GRP_CONTROL_BEEP.TabIndex = 13;
            this.GRP_CONTROL_BEEP.TabStop = false;
            this.GRP_CONTROL_BEEP.Text = "Beep";
            // 
            // CONTROL_BEEP_OFF
            // 
            this.CONTROL_BEEP_OFF.Enabled = false;
            this.CONTROL_BEEP_OFF.Location = new System.Drawing.Point(86, 198);
            this.CONTROL_BEEP_OFF.Name = "CONTROL_BEEP_OFF";
            this.CONTROL_BEEP_OFF.Size = new System.Drawing.Size(33, 23);
            this.CONTROL_BEEP_OFF.TabIndex = 22;
            this.CONTROL_BEEP_OFF.Text = "off";
            this.CONTROL_BEEP_OFF.UseVisualStyleBackColor = true;
            this.CONTROL_BEEP_OFF.Click += new System.EventHandler(this.on_feature_control_beep);
            // 
            // CONTROL_BEEP_ON
            // 
            this.CONTROL_BEEP_ON.Enabled = false;
            this.CONTROL_BEEP_ON.Location = new System.Drawing.Point(50, 198);
            this.CONTROL_BEEP_ON.Name = "CONTROL_BEEP_ON";
            this.CONTROL_BEEP_ON.Size = new System.Drawing.Size(33, 23);
            this.CONTROL_BEEP_ON.TabIndex = 22;
            this.CONTROL_BEEP_ON.Text = "on";
            this.CONTROL_BEEP_ON.UseVisualStyleBackColor = true;
            this.CONTROL_BEEP_ON.Click += new System.EventHandler(this.on_feature_control_beep);
            // 
            // CONTROL_BEEP_CHECK_LIST
            // 
            this.CONTROL_BEEP_CHECK_LIST.CheckOnClick = true;
            this.CONTROL_BEEP_CHECK_LIST.Enabled = false;
            this.CONTROL_BEEP_CHECK_LIST.FormattingEnabled = true;
            this.CONTROL_BEEP_CHECK_LIST.HorizontalScrollbar = true;
            this.CONTROL_BEEP_CHECK_LIST.Location = new System.Drawing.Point(8, 20);
            this.CONTROL_BEEP_CHECK_LIST.Name = "CONTROL_BEEP_CHECK_LIST";
            this.CONTROL_BEEP_CHECK_LIST.Size = new System.Drawing.Size(114, 169);
            this.CONTROL_BEEP_CHECK_LIST.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.BTN_INST_REC_END);
            this.groupBox2.Controls.Add(this.BTN_INST_REC_START);
            this.groupBox2.Location = new System.Drawing.Point(224, 544);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(114, 120);
            this.groupBox2.TabIndex = 31;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Instant Record";
            // 
            // BTN_INST_REC_END
            // 
            this.BTN_INST_REC_END.Enabled = false;
            this.BTN_INST_REC_END.Location = new System.Drawing.Point(25, 65);
            this.BTN_INST_REC_END.Name = "BTN_INST_REC_END";
            this.BTN_INST_REC_END.Size = new System.Drawing.Size(64, 25);
            this.BTN_INST_REC_END.TabIndex = 1;
            this.BTN_INST_REC_END.Text = "End";
            this.BTN_INST_REC_END.UseVisualStyleBackColor = true;
            this.BTN_INST_REC_END.Click += new System.EventHandler(this.on_btn_instant_record_end);
            // 
            // BTN_INST_REC_START
            // 
            this.BTN_INST_REC_START.Enabled = false;
            this.BTN_INST_REC_START.Location = new System.Drawing.Point(25, 31);
            this.BTN_INST_REC_START.Name = "BTN_INST_REC_START";
            this.BTN_INST_REC_START.Size = new System.Drawing.Size(64, 25);
            this.BTN_INST_REC_START.TabIndex = 0;
            this.BTN_INST_REC_START.Text = "Start";
            this.BTN_INST_REC_START.UseVisualStyleBackColor = true;
            this.BTN_INST_REC_START.Click += new System.EventHandler(this.on_btn_instant_record_start);
            // 
            // form_live
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 672);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.GRP_CONTROL_BEEP);
            this.Controls.Add(this.GRP_CONTROL_ALARM_OUT);
            this.Controls.Add(this.GRP_CONTROL_STREAM);
            this.Controls.Add(this.GRP_CONTROL_AUDIO);
            this.Controls.Add(this.GRP_CONTROL_PTZ);
            this.Controls.Add(this.GRP_CONTROL_COLOR);
            this.Controls.Add(this.STC_SCREEN);
            this.Font = new System.Drawing.Font("Tahoma", 8F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "form_live";
            this.Text = "GDK Service L/i/v/e";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.on_form_closing);
            this.GRP_CONTROL_COLOR.ResumeLayout(false);
            this.GRP_CONTROL_PTZ.ResumeLayout(false);
            this.GRP_CONTROL_AUDIO.ResumeLayout(false);
            this.GRP_CONTROL_AUDIO.PerformLayout();
            this.GRP_CONTROL_STREAM.ResumeLayout(false);
            this.GRP_CONTROL_STREAM.PerformLayout();
            this.GRP_CONTROL_ALARM_OUT.ResumeLayout(false);
            this.GRP_CONTROL_BEEP.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private GDK_tester.screen_pane_service SCREEN_PANE;
        private System.Windows.Forms.Label STC_SCREEN;
        private System.Windows.Forms.GroupBox GRP_CONTROL_COLOR;
        private GDK_tester.button_dwell CONTROL_COLOR_BRIGHT_DEC;
        private GDK_tester.button_dwell CONTROL_COLOR_CONTRAST_DEC;
        private GDK_tester.button_dwell CONTROL_COLOR_HUE_DEC;
        private GDK_tester.button_dwell CONTROL_COLOR_BRIGHT_INC;
        private GDK_tester.button_dwell CONTROL_COLOR_SATURATION_INC;
        private GDK_tester.button_dwell CONTROL_COLOR_HUE_INC;
        private GDK_tester.button_dwell CONTROL_COLOR_SATURATION_DEC;
        private GDK_tester.button_dwell CONTROL_COLOR_CONTRAST_INC;
        private System.Windows.Forms.Button CONTROL_COLOR_RESET;
        private System.Windows.Forms.GroupBox GRP_CONTROL_PTZ;
        private GDK_tester.button_dwell CONTROL_PTZ_MOVE_NW;
        private GDK_tester.button_dwell CONTROL_PTZ_MOVE_N;
        private GDK_tester.button_dwell CONTROL_PTZ_MOVE_NE;
        private GDK_tester.button_dwell CONTROL_PTZ_MOVE_W;
        private GDK_tester.button_dwell CONTROL_PTZ_MOVE_E;
        private GDK_tester.button_dwell CONTROL_PTZ_MOVE_SW;
        private GDK_tester.button_dwell CONTROL_PTZ_MOVE_S;
        private GDK_tester.button_dwell CONTROL_PTZ_MOVE_SE;
        private System.Windows.Forms.Button CONTROL_PTZ_OSD;
        private GDK_tester.button_dwell CONTROL_PTZ_ZOOM_INC;
        private GDK_tester.button_dwell CONTROL_PTZ_ZOOM_DEC;
        private GDK_tester.button_dwell CONTROL_PTZ_IRIS_INC;
        private GDK_tester.button_dwell CONTROL_PTZ_IRIS_DEC;
        private GDK_tester.button_dwell CONTROL_PTZ_FOCUS_INC;
        private GDK_tester.button_dwell CONTROL_PTZ_FOCUS_DEC;
        private System.Windows.Forms.Button CONTROL_PTZ_PRESET_SAVE;
        private System.Windows.Forms.Button CONTROL_PTZ_PRESET_MOVE;
        private System.Windows.Forms.Button CONTROL_PTZ_ONE_PUSH;
        private System.Windows.Forms.Button CONTROL_PTZ_MENU;
        private System.Windows.Forms.GroupBox GRP_CONTROL_AUDIO;
        private System.Windows.Forms.CheckBox CONTROL_AUDIO_OUT;
        private System.Windows.Forms.CheckBox CONTROL_AUDIO_IN;
        private System.Windows.Forms.GroupBox GRP_CONTROL_STREAM;
        private System.Windows.Forms.CheckBox CONTROL_MULTI_STREAM_CHK_TRANSFER;
        private System.Windows.Forms.Button CONTROL_MULTI_STREAM;
        private System.Windows.Forms.GroupBox GRP_CONTROL_ALARM_OUT;
        private GDK_tester.button_dwell CONTROL_ALARM_OFF;
        private GDK_tester.button_dwell CONTROL_ALARM_ON;
        private System.Windows.Forms.CheckedListBox CONTROL_ALARM_CHECK_LIST;
        private System.Windows.Forms.GroupBox GRP_CONTROL_BEEP;
        private GDK_tester.button_dwell CONTROL_BEEP_OFF;
        private GDK_tester.button_dwell CONTROL_BEEP_ON;
        private System.Windows.Forms.CheckedListBox CONTROL_BEEP_CHECK_LIST;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button BTN_INST_REC_END;
        private System.Windows.Forms.Button BTN_INST_REC_START;
    }
}