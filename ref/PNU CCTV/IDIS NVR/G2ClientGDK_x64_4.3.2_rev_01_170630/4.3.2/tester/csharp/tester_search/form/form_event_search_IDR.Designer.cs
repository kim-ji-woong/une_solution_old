namespace GDK_tester
{
    partial class form_event_search_IDR
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
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Root Device");
            this.STC_FROM = new System.Windows.Forms.Label();
            this.STC_TO = new System.Windows.Forms.Label();
            this.CHK_FIRST = new System.Windows.Forms.CheckBox();
            this.CHK_LAST = new System.Windows.Forms.CheckBox();
            this.DTP_FROM = new System.Windows.Forms.DateTimePicker();
            this.DTP_TO = new System.Windows.Forms.DateTimePicker();
            this.TRV_DEVICES = new System.Windows.Forms.TreeView();
            this.STC_EVENT = new System.Windows.Forms.Label();
            this.STC_EVENT_DWELL_TIME = new System.Windows.Forms.Label();
            this.CHK_ALARM_IN = new System.Windows.Forms.CheckBox();
            this.CHK_MOTION = new System.Windows.Forms.CheckBox();
            this.CHK_OBJECT = new System.Windows.Forms.CheckBox();
            this.CHK_VIDEO_LOSS = new System.Windows.Forms.CheckBox();
            this.CHK_VIDEO_ANALYTICS = new System.Windows.Forms.CheckBox();
            this.STC_DWELL_ALARM_IN = new System.Windows.Forms.Label();
            this.STC_DWELL_MOTION = new System.Windows.Forms.Label();
            this.STC_DWELL_OBJECT = new System.Windows.Forms.Label();
            this.STC_DWELL_VIDEO_LOSS = new System.Windows.Forms.Label();
            this.STC_DWELL_VIDEO_ANALYTICS = new System.Windows.Forms.Label();
            this.SLD_ALARM_IN = new System.Windows.Forms.TrackBar();
            this.SLD_MOTION = new System.Windows.Forms.TrackBar();
            this.SLD_OBJECT = new System.Windows.Forms.TrackBar();
            this.SLD_VIDEO_LOSS = new System.Windows.Forms.TrackBar();
            this.SLD_VIDEO_ANALYTICS = new System.Windows.Forms.TrackBar();
            this.BTN_OK = new System.Windows.Forms.Button();
            this.BTN_CANCEL = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.SLD_ALARM_IN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SLD_MOTION)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SLD_OBJECT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SLD_VIDEO_LOSS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SLD_VIDEO_ANALYTICS)).BeginInit();
            this.SuspendLayout();
            // 
            // STC_FROM
            // 
            this.STC_FROM.Location = new System.Drawing.Point(12, 11);
            this.STC_FROM.Name = "STC_FROM";
            this.STC_FROM.Size = new System.Drawing.Size(55, 17);
            this.STC_FROM.TabIndex = 14;
            this.STC_FROM.Text = "From :";
            this.STC_FROM.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // STC_TO
            // 
            this.STC_TO.Location = new System.Drawing.Point(12, 34);
            this.STC_TO.Name = "STC_TO";
            this.STC_TO.Size = new System.Drawing.Size(55, 17);
            this.STC_TO.TabIndex = 15;
            this.STC_TO.Text = "To :";
            this.STC_TO.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CHK_FIRST
            // 
            this.CHK_FIRST.AutoSize = true;
            this.CHK_FIRST.Location = new System.Drawing.Point(73, 12);
            this.CHK_FIRST.Name = "CHK_FIRST";
            this.CHK_FIRST.Size = new System.Drawing.Size(47, 17);
            this.CHK_FIRST.TabIndex = 0;
            this.CHK_FIRST.Text = "First";
            this.CHK_FIRST.UseVisualStyleBackColor = true;
            this.CHK_FIRST.CheckedChanged += new System.EventHandler(this.on_chk_first);
            // 
            // CHK_LAST
            // 
            this.CHK_LAST.AutoSize = true;
            this.CHK_LAST.Location = new System.Drawing.Point(74, 35);
            this.CHK_LAST.Name = "CHK_LAST";
            this.CHK_LAST.Size = new System.Drawing.Size(46, 17);
            this.CHK_LAST.TabIndex = 1;
            this.CHK_LAST.Text = "Last";
            this.CHK_LAST.UseVisualStyleBackColor = true;
            this.CHK_LAST.CheckedChanged += new System.EventHandler(this.on_chk_last);
            // 
            // DTP_FROM
            // 
            this.DTP_FROM.CustomFormat = "HH:mm:ss";
            this.DTP_FROM.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DTP_FROM.Location = new System.Drawing.Point(126, 9);
            this.DTP_FROM.Name = "DTP_FROM";
            this.DTP_FROM.ShowUpDown = true;
            this.DTP_FROM.Size = new System.Drawing.Size(80, 20);
            this.DTP_FROM.TabIndex = 2;
            this.DTP_FROM.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.on_dtp_mouse_wheel);
            // 
            // DTP_TO
            // 
            this.DTP_TO.CustomFormat = "HH:mm:ss";
            this.DTP_TO.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DTP_TO.Location = new System.Drawing.Point(126, 35);
            this.DTP_TO.Name = "DTP_TO";
            this.DTP_TO.ShowUpDown = true;
            this.DTP_TO.Size = new System.Drawing.Size(81, 20);
            this.DTP_TO.TabIndex = 3;
            this.DTP_TO.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.on_dtp_mouse_wheel);
            // 
            // TRV_DEVICES
            // 
            this.TRV_DEVICES.CheckBoxes = true;
            this.TRV_DEVICES.Location = new System.Drawing.Point(12, 61);
            this.TRV_DEVICES.Name = "TRV_DEVICES";
            treeNode2.Name = "ROOT_DEVICE";
            treeNode2.Text = "Root Device";
            this.TRV_DEVICES.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2});
            this.TRV_DEVICES.Size = new System.Drawing.Size(390, 200);
            this.TRV_DEVICES.TabIndex = 4;
            // 
            // STC_EVENT
            // 
            this.STC_EVENT.AutoSize = true;
            this.STC_EVENT.Location = new System.Drawing.Point(12, 276);
            this.STC_EVENT.Name = "STC_EVENT";
            this.STC_EVENT.Size = new System.Drawing.Size(35, 13);
            this.STC_EVENT.TabIndex = 21;
            this.STC_EVENT.Text = "Event";
            // 
            // STC_EVENT_DWELL_TIME
            // 
            this.STC_EVENT_DWELL_TIME.AutoSize = true;
            this.STC_EVENT_DWELL_TIME.Location = new System.Drawing.Point(186, 276);
            this.STC_EVENT_DWELL_TIME.Name = "STC_EVENT_DWELL_TIME";
            this.STC_EVENT_DWELL_TIME.Size = new System.Drawing.Size(163, 13);
            this.STC_EVENT_DWELL_TIME.TabIndex = 22;
            this.STC_EVENT_DWELL_TIME.Text = "Event Dwell Time (00:01~10:00)";
            // 
            // CHK_ALARM_IN
            // 
            this.CHK_ALARM_IN.AutoSize = true;
            this.CHK_ALARM_IN.Location = new System.Drawing.Point(15, 302);
            this.CHK_ALARM_IN.Name = "CHK_ALARM_IN";
            this.CHK_ALARM_IN.Size = new System.Drawing.Size(66, 17);
            this.CHK_ALARM_IN.TabIndex = 5;
            this.CHK_ALARM_IN.Text = "Alarm In";
            this.CHK_ALARM_IN.UseVisualStyleBackColor = true;
            // 
            // CHK_MOTION
            // 
            this.CHK_MOTION.AutoSize = true;
            this.CHK_MOTION.Location = new System.Drawing.Point(15, 325);
            this.CHK_MOTION.Name = "CHK_MOTION";
            this.CHK_MOTION.Size = new System.Drawing.Size(58, 17);
            this.CHK_MOTION.TabIndex = 6;
            this.CHK_MOTION.Text = "Motion";
            this.CHK_MOTION.UseVisualStyleBackColor = true;
            // 
            // CHK_OBJECT
            // 
            this.CHK_OBJECT.AutoSize = true;
            this.CHK_OBJECT.Location = new System.Drawing.Point(15, 348);
            this.CHK_OBJECT.Name = "CHK_OBJECT";
            this.CHK_OBJECT.Size = new System.Drawing.Size(58, 17);
            this.CHK_OBJECT.TabIndex = 7;
            this.CHK_OBJECT.Text = "Object";
            this.CHK_OBJECT.UseVisualStyleBackColor = true;
            // 
            // CHK_VIDEO_LOSS
            // 
            this.CHK_VIDEO_LOSS.AutoSize = true;
            this.CHK_VIDEO_LOSS.Location = new System.Drawing.Point(15, 371);
            this.CHK_VIDEO_LOSS.Name = "CHK_VIDEO_LOSS";
            this.CHK_VIDEO_LOSS.Size = new System.Drawing.Size(76, 17);
            this.CHK_VIDEO_LOSS.TabIndex = 8;
            this.CHK_VIDEO_LOSS.Text = "Video Loss";
            this.CHK_VIDEO_LOSS.UseVisualStyleBackColor = true;
            // 
            // CHK_VIDEO_ANALYTICS
            // 
            this.CHK_VIDEO_ANALYTICS.AutoSize = true;
            this.CHK_VIDEO_ANALYTICS.Location = new System.Drawing.Point(15, 394);
            this.CHK_VIDEO_ANALYTICS.Name = "CHK_VIDEO_ANALYTICS";
            this.CHK_VIDEO_ANALYTICS.Size = new System.Drawing.Size(98, 17);
            this.CHK_VIDEO_ANALYTICS.TabIndex = 9;
            this.CHK_VIDEO_ANALYTICS.Text = "Video Analytics";
            this.CHK_VIDEO_ANALYTICS.UseVisualStyleBackColor = true;
            // 
            // STC_DWELL_ALARM_IN
            // 
            this.STC_DWELL_ALARM_IN.Location = new System.Drawing.Point(186, 303);
            this.STC_DWELL_ALARM_IN.Name = "STC_DWELL_ALARM_IN";
            this.STC_DWELL_ALARM_IN.Size = new System.Drawing.Size(40, 12);
            this.STC_DWELL_ALARM_IN.TabIndex = 28;
            this.STC_DWELL_ALARM_IN.Text = "00:10";
            this.STC_DWELL_ALARM_IN.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // STC_DWELL_MOTION
            // 
            this.STC_DWELL_MOTION.Location = new System.Drawing.Point(186, 326);
            this.STC_DWELL_MOTION.Name = "STC_DWELL_MOTION";
            this.STC_DWELL_MOTION.Size = new System.Drawing.Size(40, 12);
            this.STC_DWELL_MOTION.TabIndex = 29;
            this.STC_DWELL_MOTION.Text = "00:10";
            this.STC_DWELL_MOTION.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // STC_DWELL_OBJECT
            // 
            this.STC_DWELL_OBJECT.Location = new System.Drawing.Point(186, 349);
            this.STC_DWELL_OBJECT.Name = "STC_DWELL_OBJECT";
            this.STC_DWELL_OBJECT.Size = new System.Drawing.Size(40, 12);
            this.STC_DWELL_OBJECT.TabIndex = 30;
            this.STC_DWELL_OBJECT.Text = "00:10";
            this.STC_DWELL_OBJECT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // STC_DWELL_VIDEO_LOSS
            // 
            this.STC_DWELL_VIDEO_LOSS.Location = new System.Drawing.Point(186, 372);
            this.STC_DWELL_VIDEO_LOSS.Name = "STC_DWELL_VIDEO_LOSS";
            this.STC_DWELL_VIDEO_LOSS.Size = new System.Drawing.Size(40, 12);
            this.STC_DWELL_VIDEO_LOSS.TabIndex = 31;
            this.STC_DWELL_VIDEO_LOSS.Text = "00:10";
            this.STC_DWELL_VIDEO_LOSS.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // STC_DWELL_VIDEO_ANALYTICS
            // 
            this.STC_DWELL_VIDEO_ANALYTICS.Location = new System.Drawing.Point(186, 395);
            this.STC_DWELL_VIDEO_ANALYTICS.Name = "STC_DWELL_VIDEO_ANALYTICS";
            this.STC_DWELL_VIDEO_ANALYTICS.Size = new System.Drawing.Size(40, 12);
            this.STC_DWELL_VIDEO_ANALYTICS.TabIndex = 32;
            this.STC_DWELL_VIDEO_ANALYTICS.Text = "00:10";
            this.STC_DWELL_VIDEO_ANALYTICS.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SLD_ALARM_IN
            // 
            this.SLD_ALARM_IN.AutoSize = false;
            this.SLD_ALARM_IN.Location = new System.Drawing.Point(230, 302);
            this.SLD_ALARM_IN.Maximum = 600;
            this.SLD_ALARM_IN.Minimum = 1;
            this.SLD_ALARM_IN.Name = "SLD_ALARM_IN";
            this.SLD_ALARM_IN.Size = new System.Drawing.Size(150, 23);
            this.SLD_ALARM_IN.TabIndex = 10;
            this.SLD_ALARM_IN.TickFrequency = 30;
            this.SLD_ALARM_IN.TickStyle = System.Windows.Forms.TickStyle.None;
            this.SLD_ALARM_IN.Value = 10;
            this.SLD_ALARM_IN.ValueChanged += new System.EventHandler(this.on_dwell_time_value_changed);
            this.SLD_ALARM_IN.MouseUp += new System.Windows.Forms.MouseEventHandler(this.on_dwell_time_value_reset);
            // 
            // SLD_MOTION
            // 
            this.SLD_MOTION.AutoSize = false;
            this.SLD_MOTION.Location = new System.Drawing.Point(230, 325);
            this.SLD_MOTION.Maximum = 600;
            this.SLD_MOTION.Minimum = 1;
            this.SLD_MOTION.Name = "SLD_MOTION";
            this.SLD_MOTION.Size = new System.Drawing.Size(150, 23);
            this.SLD_MOTION.TabIndex = 11;
            this.SLD_MOTION.TickFrequency = 30;
            this.SLD_MOTION.TickStyle = System.Windows.Forms.TickStyle.None;
            this.SLD_MOTION.Value = 10;
            this.SLD_MOTION.ValueChanged += new System.EventHandler(this.on_dwell_time_value_changed);
            this.SLD_MOTION.MouseUp += new System.Windows.Forms.MouseEventHandler(this.on_dwell_time_value_reset);
            // 
            // SLD_OBJECT
            // 
            this.SLD_OBJECT.AutoSize = false;
            this.SLD_OBJECT.Location = new System.Drawing.Point(230, 348);
            this.SLD_OBJECT.Maximum = 600;
            this.SLD_OBJECT.Minimum = 1;
            this.SLD_OBJECT.Name = "SLD_OBJECT";
            this.SLD_OBJECT.Size = new System.Drawing.Size(150, 23);
            this.SLD_OBJECT.TabIndex = 12;
            this.SLD_OBJECT.TickFrequency = 30;
            this.SLD_OBJECT.TickStyle = System.Windows.Forms.TickStyle.None;
            this.SLD_OBJECT.Value = 10;
            this.SLD_OBJECT.ValueChanged += new System.EventHandler(this.on_dwell_time_value_changed);
            this.SLD_OBJECT.MouseUp += new System.Windows.Forms.MouseEventHandler(this.on_dwell_time_value_reset);
            // 
            // SLD_VIDEO_LOSS
            // 
            this.SLD_VIDEO_LOSS.AutoSize = false;
            this.SLD_VIDEO_LOSS.Location = new System.Drawing.Point(230, 371);
            this.SLD_VIDEO_LOSS.Maximum = 600;
            this.SLD_VIDEO_LOSS.Minimum = 1;
            this.SLD_VIDEO_LOSS.Name = "SLD_VIDEO_LOSS";
            this.SLD_VIDEO_LOSS.Size = new System.Drawing.Size(150, 23);
            this.SLD_VIDEO_LOSS.TabIndex = 13;
            this.SLD_VIDEO_LOSS.TickFrequency = 30;
            this.SLD_VIDEO_LOSS.TickStyle = System.Windows.Forms.TickStyle.None;
            this.SLD_VIDEO_LOSS.Value = 10;
            this.SLD_VIDEO_LOSS.ValueChanged += new System.EventHandler(this.on_dwell_time_value_changed);
            this.SLD_VIDEO_LOSS.MouseUp += new System.Windows.Forms.MouseEventHandler(this.on_dwell_time_value_reset);
            // 
            // SLD_VIDEO_ANALYTICS
            // 
            this.SLD_VIDEO_ANALYTICS.AutoSize = false;
            this.SLD_VIDEO_ANALYTICS.Location = new System.Drawing.Point(230, 394);
            this.SLD_VIDEO_ANALYTICS.Maximum = 600;
            this.SLD_VIDEO_ANALYTICS.Minimum = 1;
            this.SLD_VIDEO_ANALYTICS.Name = "SLD_VIDEO_ANALYTICS";
            this.SLD_VIDEO_ANALYTICS.Size = new System.Drawing.Size(150, 23);
            this.SLD_VIDEO_ANALYTICS.TabIndex = 14;
            this.SLD_VIDEO_ANALYTICS.TickFrequency = 30;
            this.SLD_VIDEO_ANALYTICS.TickStyle = System.Windows.Forms.TickStyle.None;
            this.SLD_VIDEO_ANALYTICS.Value = 10;
            this.SLD_VIDEO_ANALYTICS.ValueChanged += new System.EventHandler(this.on_dwell_time_value_changed);
            this.SLD_VIDEO_ANALYTICS.MouseUp += new System.Windows.Forms.MouseEventHandler(this.on_dwell_time_value_reset);
            // 
            // BTN_OK
            // 
            this.BTN_OK.Location = new System.Drawing.Point(246, 428);
            this.BTN_OK.Name = "BTN_OK";
            this.BTN_OK.Size = new System.Drawing.Size(75, 23);
            this.BTN_OK.TabIndex = 15;
            this.BTN_OK.Text = "OK";
            this.BTN_OK.UseVisualStyleBackColor = true;
            this.BTN_OK.Click += new System.EventHandler(this.on_btn_OK);
            // 
            // BTN_CANCEL
            // 
            this.BTN_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BTN_CANCEL.Location = new System.Drawing.Point(327, 428);
            this.BTN_CANCEL.Name = "BTN_CANCEL";
            this.BTN_CANCEL.Size = new System.Drawing.Size(75, 23);
            this.BTN_CANCEL.TabIndex = 17;
            this.BTN_CANCEL.Text = "Cancel";
            this.BTN_CANCEL.UseVisualStyleBackColor = true;
            // 
            // form_event_search_IDR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 462);
            this.Controls.Add(this.STC_FROM);
            this.Controls.Add(this.STC_TO);
            this.Controls.Add(this.CHK_FIRST);
            this.Controls.Add(this.CHK_LAST);
            this.Controls.Add(this.DTP_FROM);
            this.Controls.Add(this.DTP_TO);
            this.Controls.Add(this.TRV_DEVICES);
            this.Controls.Add(this.STC_EVENT);
            this.Controls.Add(this.STC_EVENT_DWELL_TIME);
            this.Controls.Add(this.CHK_ALARM_IN);
            this.Controls.Add(this.CHK_MOTION);
            this.Controls.Add(this.CHK_OBJECT);
            this.Controls.Add(this.CHK_VIDEO_LOSS);
            this.Controls.Add(this.CHK_VIDEO_ANALYTICS);
            this.Controls.Add(this.STC_DWELL_ALARM_IN);
            this.Controls.Add(this.STC_DWELL_MOTION);
            this.Controls.Add(this.STC_DWELL_OBJECT);
            this.Controls.Add(this.STC_DWELL_VIDEO_LOSS);
            this.Controls.Add(this.STC_DWELL_VIDEO_ANALYTICS);
            this.Controls.Add(this.SLD_ALARM_IN);
            this.Controls.Add(this.SLD_MOTION);
            this.Controls.Add(this.SLD_OBJECT);
            this.Controls.Add(this.SLD_VIDEO_LOSS);
            this.Controls.Add(this.SLD_VIDEO_ANALYTICS);
            this.Controls.Add(this.BTN_OK);
            this.Controls.Add(this.BTN_CANCEL);
            this.AcceptButton = this.BTN_OK;
            this.CancelButton = this.BTN_CANCEL;
            this.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "form_event_search_IDR";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Event Search";
            ((System.ComponentModel.ISupportInitialize)(this.SLD_ALARM_IN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SLD_MOTION)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SLD_OBJECT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SLD_VIDEO_LOSS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SLD_VIDEO_ANALYTICS)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label STC_FROM;
        private System.Windows.Forms.Label STC_TO;
        private System.Windows.Forms.CheckBox CHK_FIRST;
        private System.Windows.Forms.CheckBox CHK_LAST;
        private System.Windows.Forms.DateTimePicker DTP_FROM;
        private System.Windows.Forms.DateTimePicker DTP_TO;
        private System.Windows.Forms.TreeView TRV_DEVICES;
        private System.Windows.Forms.Label STC_EVENT;
        private System.Windows.Forms.Label STC_EVENT_DWELL_TIME;
        private System.Windows.Forms.CheckBox CHK_ALARM_IN;
        private System.Windows.Forms.CheckBox CHK_MOTION;
        private System.Windows.Forms.CheckBox CHK_OBJECT;
        private System.Windows.Forms.CheckBox CHK_VIDEO_LOSS;
        private System.Windows.Forms.CheckBox CHK_VIDEO_ANALYTICS;
        private System.Windows.Forms.Label STC_DWELL_ALARM_IN;
        private System.Windows.Forms.Label STC_DWELL_MOTION;
        private System.Windows.Forms.Label STC_DWELL_OBJECT;
        private System.Windows.Forms.Label STC_DWELL_VIDEO_LOSS;
        private System.Windows.Forms.Label STC_DWELL_VIDEO_ANALYTICS;
        private System.Windows.Forms.TrackBar SLD_ALARM_IN;
        private System.Windows.Forms.TrackBar SLD_MOTION;
        private System.Windows.Forms.TrackBar SLD_OBJECT;
        private System.Windows.Forms.TrackBar SLD_VIDEO_LOSS;
        private System.Windows.Forms.TrackBar SLD_VIDEO_ANALYTICS;
        private System.Windows.Forms.Button BTN_OK;
        private System.Windows.Forms.Button BTN_CANCEL;
    }
}