namespace GDK_tester
{
    partial class controller_search_g2
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
            this.CAL_MONTH = new System.Windows.Forms.MonthCalendar();
            this.DTP_GOTO = new System.Windows.Forms.DateTimePicker();
            this.BTN_GOTO = new System.Windows.Forms.Button();
            this.BTN_GOTO_FIRST = new System.Windows.Forms.Button();
            this.BTN_GOTO_LAST = new System.Windows.Forms.Button();
            this.CHK_GOTO_ADJACENT_FRAME = new System.Windows.Forms.CheckBox();
            this.BTN_STEP_PREV = new System.Windows.Forms.Button();
            this.BTN_STEP_NEXT = new System.Windows.Forms.Button();
            this.BTN_STEP_INTERVAL = new System.Windows.Forms.Button();
            this.BTN_PLAY = new System.Windows.Forms.Button();
            this.BTN_STOP = new System.Windows.Forms.Button();
            this.CHK_AUDIO_PLAY = new System.Windows.Forms.CheckBox();
            this.SLD_SPEED = new System.Windows.Forms.TrackBar();
            this.STC_SPEED = new System.Windows.Forms.Label();
            this.CHK_USE_SEGMENT = new System.Windows.Forms.CheckBox();
            this.BTN_MODE_TIMELAPSE = new System.Windows.Forms.Button();
            this.BTN_MODE_EVENT = new System.Windows.Forms.Button();
            this.BTN_EVENT_SEARCH = new System.Windows.Forms.Button();
            this.BTN_EVENT_SEARCH_MORE = new System.Windows.Forms.Button();
            this.BTN_TEXT_IN_SEARCH = new System.Windows.Forms.Button();
            this.BTN_TEXT_IN_SEARCH_MORE = new System.Windows.Forms.Button();
            this.BTN_EXPORT_CLIP = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.SLD_SPEED)).BeginInit();
            this.SuspendLayout();
            // 
            // CAL_MONTH
            // 
            this.CAL_MONTH.BackColor = System.Drawing.SystemColors.Window;
            this.CAL_MONTH.Location = new System.Drawing.Point(0, 0);
            this.CAL_MONTH.Margin = new System.Windows.Forms.Padding(0);
            this.CAL_MONTH.MaxSelectionCount = 1;
            this.CAL_MONTH.Name = "CAL_MONTH";
            this.CAL_MONTH.ShowTodayCircle = false;
            this.CAL_MONTH.TabIndex = 13;
            this.CAL_MONTH.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.on_cal_date_selected);
            // 
            // DTP_GOTO
            // 
            this.DTP_GOTO.CustomFormat = " yyyy-MM-dd HH:mm:ss";
            this.DTP_GOTO.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DTP_GOTO.Location = new System.Drawing.Point(231, 46);
            this.DTP_GOTO.Name = "DTP_GOTO";
            this.DTP_GOTO.Size = new System.Drawing.Size(144, 20);
            this.DTP_GOTO.TabIndex = 10;
            this.DTP_GOTO.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.on_dtp_goto_mouse_wheel);
            // 
            // BTN_GOTO
            // 
            this.BTN_GOTO.Location = new System.Drawing.Point(382, 44);
            this.BTN_GOTO.Name = "BTN_GOTO";
            this.BTN_GOTO.Size = new System.Drawing.Size(46, 23);
            this.BTN_GOTO.TabIndex = 11;
            this.BTN_GOTO.Text = "goto";
            this.BTN_GOTO.UseVisualStyleBackColor = true;
            this.BTN_GOTO.Click += new System.EventHandler(this.on_btn_goto);
            // 
            // BTN_GOTO_FIRST
            // 
            this.BTN_GOTO_FIRST.Location = new System.Drawing.Point(231, 12);
            this.BTN_GOTO_FIRST.Name = "BTN_GOTO_FIRST";
            this.BTN_GOTO_FIRST.Size = new System.Drawing.Size(34, 23);
            this.BTN_GOTO_FIRST.TabIndex = 6;
            this.BTN_GOTO_FIRST.Text = "first";
            this.BTN_GOTO_FIRST.UseVisualStyleBackColor = true;
            this.BTN_GOTO_FIRST.Click += new System.EventHandler(this.on_btn_goto_first);
            // 
            // BTN_GOTO_LAST
            // 
            this.BTN_GOTO_LAST.Location = new System.Drawing.Point(266, 12);
            this.BTN_GOTO_LAST.Name = "BTN_GOTO_LAST";
            this.BTN_GOTO_LAST.Size = new System.Drawing.Size(34, 23);
            this.BTN_GOTO_LAST.TabIndex = 5;
            this.BTN_GOTO_LAST.Text = "last";
            this.BTN_GOTO_LAST.UseVisualStyleBackColor = true;
            this.BTN_GOTO_LAST.Click += new System.EventHandler(this.on_btn_goto_last);
            // 
            // CHK_GOTO_ADJACENT_FRAME
            // 
            this.CHK_GOTO_ADJACENT_FRAME.AutoSize = true;
            this.CHK_GOTO_ADJACENT_FRAME.Location = new System.Drawing.Point(232, 69);
            this.CHK_GOTO_ADJACENT_FRAME.Name = "CHK_GOTO_ADJACENT_FRAME";
            this.CHK_GOTO_ADJACENT_FRAME.Size = new System.Drawing.Size(134, 17);
            this.CHK_GOTO_ADJACENT_FRAME.TabIndex = 21;
            this.CHK_GOTO_ADJACENT_FRAME.Text = "search adjacent frame";
            this.CHK_GOTO_ADJACENT_FRAME.UseVisualStyleBackColor = true;
            // 
            // BTN_STEP_PREV
            // 
            this.BTN_STEP_PREV.Location = new System.Drawing.Point(306, 12);
            this.BTN_STEP_PREV.Name = "BTN_STEP_PREV";
            this.BTN_STEP_PREV.Size = new System.Drawing.Size(18, 23);
            this.BTN_STEP_PREV.TabIndex = 9;
            this.BTN_STEP_PREV.Text = "<";
            this.BTN_STEP_PREV.UseVisualStyleBackColor = true;
            this.BTN_STEP_PREV.Click += new System.EventHandler(this.on_btn_step_prev);
            // 
            // BTN_STEP_NEXT
            // 
            this.BTN_STEP_NEXT.Location = new System.Drawing.Point(357, 12);
            this.BTN_STEP_NEXT.Name = "BTN_STEP_NEXT";
            this.BTN_STEP_NEXT.Size = new System.Drawing.Size(18, 23);
            this.BTN_STEP_NEXT.TabIndex = 8;
            this.BTN_STEP_NEXT.Text = ">";
            this.BTN_STEP_NEXT.UseVisualStyleBackColor = true;
            this.BTN_STEP_NEXT.Click += new System.EventHandler(this.on_btn_step_next);
            // 
            // BTN_STEP_INTERVAL
            // 
            this.BTN_STEP_INTERVAL.Location = new System.Drawing.Point(323, 12);
            this.BTN_STEP_INTERVAL.Name = "BTN_STEP_INTERVAL";
            this.BTN_STEP_INTERVAL.Size = new System.Drawing.Size(35, 23);
            this.BTN_STEP_INTERVAL.TabIndex = 7;
            this.BTN_STEP_INTERVAL.Text = "1F";
            this.BTN_STEP_INTERVAL.UseVisualStyleBackColor = true;
            this.BTN_STEP_INTERVAL.Click += new System.EventHandler(this.on_btn_step_move);
            // 
            // BTN_PLAY
            // 
            this.BTN_PLAY.Location = new System.Drawing.Point(382, 8);
            this.BTN_PLAY.Name = "BTN_PLAY";
            this.BTN_PLAY.Size = new System.Drawing.Size(46, 30);
            this.BTN_PLAY.TabIndex = 1;
            this.BTN_PLAY.Text = "Play";
            this.BTN_PLAY.UseVisualStyleBackColor = true;
            this.BTN_PLAY.Click += new System.EventHandler(this.on_btn_play);
            // 
            // BTN_STOP
            // 
            this.BTN_STOP.Location = new System.Drawing.Point(382, 8);
            this.BTN_STOP.Name = "BTN_STOP";
            this.BTN_STOP.Size = new System.Drawing.Size(46, 30);
            this.BTN_STOP.TabIndex = 1;
            this.BTN_STOP.Text = "Stop";
            this.BTN_STOP.UseVisualStyleBackColor = true;
            this.BTN_STOP.Click += new System.EventHandler(this.on_btn_stop);
            // 
            // CHK_AUDIO_PLAY
            // 
            this.CHK_AUDIO_PLAY.AutoSize = true;
            this.CHK_AUDIO_PLAY.Location = new System.Drawing.Point(435, 48);
            this.CHK_AUDIO_PLAY.Name = "CHK_AUDIO_PLAY";
            this.CHK_AUDIO_PLAY.Size = new System.Drawing.Size(75, 17);
            this.CHK_AUDIO_PLAY.TabIndex = 3;
            this.CHK_AUDIO_PLAY.Text = "play audio";
            this.CHK_AUDIO_PLAY.UseVisualStyleBackColor = true;
            this.CHK_AUDIO_PLAY.CheckedChanged += new System.EventHandler(this.on_chk_play_audio);
            // 
            // SLD_SPEED
            // 
            this.SLD_SPEED.AutoSize = false;
            this.SLD_SPEED.CausesValidation = false;
            this.SLD_SPEED.Location = new System.Drawing.Point(430, 12);
            this.SLD_SPEED.Maximum = 80;
            this.SLD_SPEED.Minimum = -80;
            this.SLD_SPEED.Name = "SLD_SPEED";
            this.SLD_SPEED.Size = new System.Drawing.Size(166, 23);
            this.SLD_SPEED.TabIndex = 2;
            this.SLD_SPEED.TickStyle = System.Windows.Forms.TickStyle.None;
            this.SLD_SPEED.ValueChanged += new System.EventHandler(this.on_speed_value_changed);
            this.SLD_SPEED.MouseUp += new System.Windows.Forms.MouseEventHandler(this.on_speed_mouse_up);
            // 
            // STC_SPEED
            // 
            this.STC_SPEED.Location = new System.Drawing.Point(498, 34);
            this.STC_SPEED.Name = "STC_SPEED";
            this.STC_SPEED.Size = new System.Drawing.Size(90, 13);
            this.STC_SPEED.TabIndex = 12;
            this.STC_SPEED.Text = "x1.0";
            this.STC_SPEED.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CHK_USE_SEGMENT
            // 
            this.CHK_USE_SEGMENT.AutoSize = true;
            this.CHK_USE_SEGMENT.Location = new System.Drawing.Point(435, 69);
            this.CHK_USE_SEGMENT.Name = "CHK_USE_SEGMENT";
            this.CHK_USE_SEGMENT.Size = new System.Drawing.Size(132, 17);
            this.CHK_USE_SEGMENT.TabIndex = 4;
            this.CHK_USE_SEGMENT.Text = "use segment selection";
            this.CHK_USE_SEGMENT.UseVisualStyleBackColor = true;
            // 
            // BTN_MODE_TIMELAPSE
            // 
            this.BTN_MODE_TIMELAPSE.Location = new System.Drawing.Point(231, 97);
            this.BTN_MODE_TIMELAPSE.Name = "BTN_MODE_TIMELAPSE";
            this.BTN_MODE_TIMELAPSE.Size = new System.Drawing.Size(49, 47);
            this.BTN_MODE_TIMELAPSE.TabIndex = 15;
            this.BTN_MODE_TIMELAPSE.Text = "time";
            this.BTN_MODE_TIMELAPSE.UseVisualStyleBackColor = true;
            this.BTN_MODE_TIMELAPSE.Visible = false;
            this.BTN_MODE_TIMELAPSE.Click += new System.EventHandler(this.on_btn_mode_timelapse);
            // 
            // BTN_MODE_EVENT
            // 
            this.BTN_MODE_EVENT.Location = new System.Drawing.Point(231, 97);
            this.BTN_MODE_EVENT.Name = "BTN_MODE_EVENT";
            this.BTN_MODE_EVENT.Size = new System.Drawing.Size(49, 47);
            this.BTN_MODE_EVENT.TabIndex = 17;
            this.BTN_MODE_EVENT.Text = "event";
            this.BTN_MODE_EVENT.UseVisualStyleBackColor = true;
            this.BTN_MODE_EVENT.Click += new System.EventHandler(this.on_btn_mode_event);
            // 
            // BTN_EVENT_SEARCH
            // 
            this.BTN_EVENT_SEARCH.Location = new System.Drawing.Point(286, 97);
            this.BTN_EVENT_SEARCH.Name = "BTN_EVENT_SEARCH";
            this.BTN_EVENT_SEARCH.Size = new System.Drawing.Size(89, 23);
            this.BTN_EVENT_SEARCH.TabIndex = 16;
            this.BTN_EVENT_SEARCH.Text = "event search";
            this.BTN_EVENT_SEARCH.UseVisualStyleBackColor = true;
            this.BTN_EVENT_SEARCH.Click += new System.EventHandler(this.on_btn_event_search);
            // 
            // BTN_EVENT_SEARCH_MORE
            // 
            this.BTN_EVENT_SEARCH_MORE.Location = new System.Drawing.Point(381, 97);
            this.BTN_EVENT_SEARCH_MORE.Name = "BTN_EVENT_SEARCH_MORE";
            this.BTN_EVENT_SEARCH_MORE.Size = new System.Drawing.Size(46, 23);
            this.BTN_EVENT_SEARCH_MORE.TabIndex = 17;
            this.BTN_EVENT_SEARCH_MORE.Text = "more";
            this.BTN_EVENT_SEARCH_MORE.UseVisualStyleBackColor = true;
            this.BTN_EVENT_SEARCH_MORE.Click += new System.EventHandler(this.on_btn_event_search_more);
            // 
            // BTN_TEXT_IN_SEARCH
            // 
            this.BTN_TEXT_IN_SEARCH.Location = new System.Drawing.Point(286, 121);
            this.BTN_TEXT_IN_SEARCH.Name = "BTN_TEXT_IN_SEARCH";
            this.BTN_TEXT_IN_SEARCH.Size = new System.Drawing.Size(89, 23);
            this.BTN_TEXT_IN_SEARCH.TabIndex = 18;
            this.BTN_TEXT_IN_SEARCH.Text = "text in search";
            this.BTN_TEXT_IN_SEARCH.UseVisualStyleBackColor = true;
            this.BTN_TEXT_IN_SEARCH.Click += new System.EventHandler(this.on_btn_text_in_search);
            // 
            // BTN_TEXT_IN_SEARCH_MORE
            // 
            this.BTN_TEXT_IN_SEARCH_MORE.Location = new System.Drawing.Point(381, 121);
            this.BTN_TEXT_IN_SEARCH_MORE.Name = "BTN_TEXT_IN_SEARCH_MORE";
            this.BTN_TEXT_IN_SEARCH_MORE.Size = new System.Drawing.Size(46, 23);
            this.BTN_TEXT_IN_SEARCH_MORE.TabIndex = 19;
            this.BTN_TEXT_IN_SEARCH_MORE.Text = "more";
            this.BTN_TEXT_IN_SEARCH_MORE.UseVisualStyleBackColor = true;
            this.BTN_TEXT_IN_SEARCH_MORE.Click += new System.EventHandler(this.on_btn_text_in_search_more);
            // 
            // BTN_EXPORT_CLIP
            // 
            this.BTN_EXPORT_CLIP.Location = new System.Drawing.Point(435, 97);
            this.BTN_EXPORT_CLIP.Name = "BTN_EXPORT_CLIP";
            this.BTN_EXPORT_CLIP.Size = new System.Drawing.Size(89, 23);
            this.BTN_EXPORT_CLIP.TabIndex = 20;
            this.BTN_EXPORT_CLIP.Text = "export clip";
            this.BTN_EXPORT_CLIP.UseVisualStyleBackColor = true;
            this.BTN_EXPORT_CLIP.Click += new System.EventHandler(this.on_btn_export_clip);
            // 
            // controller_search_g2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 162);
            this.ControlBox = false;
            this.Controls.Add(this.BTN_EXPORT_CLIP);
            this.Controls.Add(this.CAL_MONTH);
            this.Controls.Add(this.DTP_GOTO);
            this.Controls.Add(this.BTN_GOTO);
            this.Controls.Add(this.BTN_GOTO_FIRST);
            this.Controls.Add(this.BTN_GOTO_LAST);
            this.Controls.Add(this.CHK_GOTO_ADJACENT_FRAME);
            this.Controls.Add(this.BTN_STEP_PREV);
            this.Controls.Add(this.BTN_STEP_NEXT);
            this.Controls.Add(this.BTN_STEP_INTERVAL);
            this.Controls.Add(this.BTN_PLAY);
            this.Controls.Add(this.BTN_STOP);
            this.Controls.Add(this.CHK_AUDIO_PLAY);
            this.Controls.Add(this.SLD_SPEED);
            this.Controls.Add(this.STC_SPEED);
            this.Controls.Add(this.CHK_USE_SEGMENT);
            this.Controls.Add(this.BTN_MODE_TIMELAPSE);
            this.Controls.Add(this.BTN_MODE_EVENT);
            this.Controls.Add(this.BTN_EVENT_SEARCH);
            this.Controls.Add(this.BTN_EVENT_SEARCH_MORE);
            this.Controls.Add(this.BTN_TEXT_IN_SEARCH);
            this.Controls.Add(this.BTN_TEXT_IN_SEARCH_MORE);
            this.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "controller_search_g2";
            this.Text = "controller_search_g2";
            ((System.ComponentModel.ISupportInitialize)(this.SLD_SPEED)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MonthCalendar CAL_MONTH;
        private System.Windows.Forms.DateTimePicker DTP_GOTO;
        private System.Windows.Forms.Button BTN_GOTO;
        private System.Windows.Forms.Button BTN_GOTO_FIRST;
        private System.Windows.Forms.Button BTN_GOTO_LAST;
        private System.Windows.Forms.CheckBox CHK_GOTO_ADJACENT_FRAME;
        private System.Windows.Forms.Button BTN_STEP_PREV;
        private System.Windows.Forms.Button BTN_STEP_NEXT;
        private System.Windows.Forms.Button BTN_STEP_INTERVAL;
        private System.Windows.Forms.Button BTN_PLAY;
        private System.Windows.Forms.Button BTN_STOP;
        private System.Windows.Forms.CheckBox CHK_AUDIO_PLAY;
        private System.Windows.Forms.TrackBar SLD_SPEED;
        private System.Windows.Forms.Label STC_SPEED;
        private System.Windows.Forms.CheckBox CHK_USE_SEGMENT;
        private System.Windows.Forms.Button BTN_MODE_TIMELAPSE;
        private System.Windows.Forms.Button BTN_MODE_EVENT;
        private System.Windows.Forms.Button BTN_EVENT_SEARCH;
        private System.Windows.Forms.Button BTN_EVENT_SEARCH_MORE;
        private System.Windows.Forms.Button BTN_TEXT_IN_SEARCH;
        private System.Windows.Forms.Button BTN_TEXT_IN_SEARCH_MORE;
        private System.Windows.Forms.Button BTN_EXPORT_CLIP;
    }
}
