namespace GDK_tester
{
    partial class form_search
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(form_search));
            this.STC_SCREEN = new System.Windows.Forms.Label();
            this.STC_PROBE_PERF = new System.Windows.Forms.Label();
            this.STC_TIME_TABLE = new System.Windows.Forms.Label();
            this.STC_CONTROLLER = new System.Windows.Forms.Label();
            this.BTN_SCREEN_FORMAT = new System.Windows.Forms.Button();
            this.BTN_SCREEN_FORMAT_PREV = new System.Windows.Forms.Button();
            this.BTN_SCREEN_FORMAT_NEXT = new System.Windows.Forms.Button();
            this.LSV_EVENT = new System.Windows.Forms.ListView();
            this.LSV_EVENT_COL_EVENT = new System.Windows.Forms.ColumnHeader();
            this.LSV_EVENT_COL_TRIGGERED = new System.Windows.Forms.ColumnHeader();
            this.LSV_EVENT_COL_DATA = new System.Windows.Forms.ColumnHeader();
            this.LSV_EVENT_COL_TIME = new System.Windows.Forms.ColumnHeader();
            this.BTN_CONNECT = new System.Windows.Forms.Button();
            this.BTN_DISCONNECT = new System.Windows.Forms.Button();
            this.CHK_PROBE_PERF = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // STC_SCREEN
            // 
            this.STC_SCREEN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.STC_SCREEN.Location = new System.Drawing.Point(10, 10);
            this.STC_SCREEN.Name = "STC_SCREEN";
            this.STC_SCREEN.Size = new System.Drawing.Size(680, 414);
            this.STC_SCREEN.TabIndex = 3;
            this.STC_SCREEN.Text = "screen";
            this.STC_SCREEN.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.STC_SCREEN.Visible = false;
            // 
            // STC_PROBE_PERF
            // 
            this.STC_PROBE_PERF.Location = new System.Drawing.Point(162, 427);
            this.STC_PROBE_PERF.Name = "STC_PROBE_PERF";
            this.STC_PROBE_PERF.Size = new System.Drawing.Size(447, 23);
            this.STC_PROBE_PERF.TabIndex = 9;
            this.STC_PROBE_PERF.Text = "probe";
            this.STC_PROBE_PERF.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // STC_TIME_TABLE
            // 
            this.STC_TIME_TABLE.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.STC_TIME_TABLE.Location = new System.Drawing.Point(10, 623);
            this.STC_TIME_TABLE.Name = "STC_TIME_TABLE";
            this.STC_TIME_TABLE.Size = new System.Drawing.Size(680, 112);
            this.STC_TIME_TABLE.TabIndex = 0;
            this.STC_TIME_TABLE.Text = "time table";
            this.STC_TIME_TABLE.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.STC_TIME_TABLE.Visible = false;
            // 
            // STC_CONTROLLER
            // 
            this.STC_CONTROLLER.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.STC_CONTROLLER.Location = new System.Drawing.Point(10, 453);
            this.STC_CONTROLLER.Name = "STC_CONTROLLER";
            this.STC_CONTROLLER.Size = new System.Drawing.Size(599, 162);
            this.STC_CONTROLLER.TabIndex = 7;
            this.STC_CONTROLLER.Text = "controller";
            this.STC_CONTROLLER.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.STC_CONTROLLER.Visible = false;
            // 
            // BTN_SCREEN_FORMAT
            // 
            this.BTN_SCREEN_FORMAT.Location = new System.Drawing.Point(10, 427);
            this.BTN_SCREEN_FORMAT.Name = "BTN_SCREEN_FORMAT";
            this.BTN_SCREEN_FORMAT.Size = new System.Drawing.Size(90, 23);
            this.BTN_SCREEN_FORMAT.TabIndex = 4;
            this.BTN_SCREEN_FORMAT.Text = "Format";
            this.BTN_SCREEN_FORMAT.UseVisualStyleBackColor = true;
            this.BTN_SCREEN_FORMAT.Click += new System.EventHandler(this.on_control_screen_format);
            // 
            // BTN_SCREEN_FORMAT_PREV
            // 
            this.BTN_SCREEN_FORMAT_PREV.Location = new System.Drawing.Point(106, 427);
            this.BTN_SCREEN_FORMAT_PREV.Name = "BTN_SCREEN_FORMAT_PREV";
            this.BTN_SCREEN_FORMAT_PREV.Size = new System.Drawing.Size(24, 23);
            this.BTN_SCREEN_FORMAT_PREV.TabIndex = 5;
            this.BTN_SCREEN_FORMAT_PREV.Text = "<";
            this.BTN_SCREEN_FORMAT_PREV.UseVisualStyleBackColor = true;
            this.BTN_SCREEN_FORMAT_PREV.Click += new System.EventHandler(this.on_control_screen_format_prev);
            // 
            // BTN_SCREEN_FORMAT_NEXT
            // 
            this.BTN_SCREEN_FORMAT_NEXT.Location = new System.Drawing.Point(131, 427);
            this.BTN_SCREEN_FORMAT_NEXT.Name = "BTN_SCREEN_FORMAT_NEXT";
            this.BTN_SCREEN_FORMAT_NEXT.Size = new System.Drawing.Size(24, 23);
            this.BTN_SCREEN_FORMAT_NEXT.TabIndex = 6;
            this.BTN_SCREEN_FORMAT_NEXT.Text = ">";
            this.BTN_SCREEN_FORMAT_NEXT.UseVisualStyleBackColor = true;
            this.BTN_SCREEN_FORMAT_NEXT.Click += new System.EventHandler(this.on_control_screen_format_next);
            // 
            // LSV_EVENT
            // 
            this.LSV_EVENT.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LSV_EVENT_COL_EVENT,
            this.LSV_EVENT_COL_TRIGGERED,
            this.LSV_EVENT_COL_DATA,
            this.LSV_EVENT_COL_TIME});
            this.LSV_EVENT.FullRowSelect = true;
            this.LSV_EVENT.GridLines = true;
            this.LSV_EVENT.HideSelection = false;
            this.LSV_EVENT.Location = new System.Drawing.Point(10, 623);
            this.LSV_EVENT.MultiSelect = false;
            this.LSV_EVENT.Name = "LSV_EVENT";
            this.LSV_EVENT.ShowItemToolTips = true;
            this.LSV_EVENT.Size = new System.Drawing.Size(680, 112);
            this.LSV_EVENT.TabIndex = 8;
            this.LSV_EVENT.UseCompatibleStateImageBehavior = false;
            this.LSV_EVENT.View = System.Windows.Forms.View.Details;
            this.LSV_EVENT.SelectedIndexChanged += new System.EventHandler(this.on_event_list_selected_changed);
            // 
            // LSV_EVENT_COL_EVENT
            // 
            this.LSV_EVENT_COL_EVENT.Text = "Event";
            this.LSV_EVENT_COL_EVENT.Width = 150;
            // 
            // LSV_EVENT_COL_TRIGGERED
            // 
            this.LSV_EVENT_COL_TRIGGERED.Text = "Triggered by";
            this.LSV_EVENT_COL_TRIGGERED.Width = 119;
            // 
            // LSV_EVENT_COL_DATA
            // 
            this.LSV_EVENT_COL_DATA.Text = "Data";
            this.LSV_EVENT_COL_DATA.Width = 210;
            // 
            // LSV_EVENT_COL_TIME
            // 
            this.LSV_EVENT_COL_TIME.Text = "Time";
            this.LSV_EVENT_COL_TIME.Width = 180;
            // 
            // BTN_CONNECT
            // 
            this.BTN_CONNECT.Location = new System.Drawing.Point(615, 427);
            this.BTN_CONNECT.Name = "BTN_CONNECT";
            this.BTN_CONNECT.Size = new System.Drawing.Size(75, 23);
            this.BTN_CONNECT.TabIndex = 0;
            this.BTN_CONNECT.Text = "connect";
            this.BTN_CONNECT.UseVisualStyleBackColor = true;
            this.BTN_CONNECT.Click += new System.EventHandler(this.on_btn_connect);
            // 
            // BTN_DISCONNECT
            // 
            this.BTN_DISCONNECT.Enabled = false;
            this.BTN_DISCONNECT.Location = new System.Drawing.Point(615, 452);
            this.BTN_DISCONNECT.Name = "BTN_DISCONNECT";
            this.BTN_DISCONNECT.Size = new System.Drawing.Size(75, 23);
            this.BTN_DISCONNECT.TabIndex = 1;
            this.BTN_DISCONNECT.Text = "disconnect";
            this.BTN_DISCONNECT.UseVisualStyleBackColor = true;
            this.BTN_DISCONNECT.Click += new System.EventHandler(this.on_btn_disconnect);
            // 
            // CHK_PROBE_PERF
            // 
            this.CHK_PROBE_PERF.AutoSize = true;
            this.CHK_PROBE_PERF.Location = new System.Drawing.Point(616, 481);
            this.CHK_PROBE_PERF.Name = "CHK_PROBE_PERF";
            this.CHK_PROBE_PERF.Size = new System.Drawing.Size(54, 17);
            this.CHK_PROBE_PERF.TabIndex = 2;
            this.CHK_PROBE_PERF.Text = "probe";
            this.CHK_PROBE_PERF.UseVisualStyleBackColor = true;
            this.CHK_PROBE_PERF.Click += new System.EventHandler(this.on_chk_probe_perf);
            // 
            // form_search
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 747);
            this.Controls.Add(this.STC_SCREEN);
            this.Controls.Add(this.STC_PROBE_PERF);
            this.Controls.Add(this.STC_CONTROLLER);
            this.Controls.Add(this.STC_TIME_TABLE);
            this.Controls.Add(this.BTN_SCREEN_FORMAT_NEXT);
            this.Controls.Add(this.BTN_SCREEN_FORMAT_PREV);
            this.Controls.Add(this.BTN_SCREEN_FORMAT);
            this.Controls.Add(this.LSV_EVENT);
            this.Controls.Add(this.BTN_DISCONNECT);
            this.Controls.Add(this.BTN_CONNECT);
            this.Controls.Add(this.CHK_PROBE_PERF);
            this.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "form_search";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "GDK S/e/a/r/c/h";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.on_form_closing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private screen_pane SCREEN_PANE;
        private System.Windows.Forms.Label STC_SCREEN;
        private System.Windows.Forms.Label STC_PROBE_PERF;
        private System.Windows.Forms.Label STC_CONTROLLER;
        private System.Windows.Forms.Label STC_TIME_TABLE;
        private System.Windows.Forms.Button BTN_SCREEN_FORMAT;
        private System.Windows.Forms.Button BTN_SCREEN_FORMAT_PREV;
        private System.Windows.Forms.Button BTN_SCREEN_FORMAT_NEXT;
        private System.Windows.Forms.ListView LSV_EVENT;
        private System.Windows.Forms.ColumnHeader LSV_EVENT_COL_EVENT;
        private System.Windows.Forms.ColumnHeader LSV_EVENT_COL_TRIGGERED;
        private System.Windows.Forms.ColumnHeader LSV_EVENT_COL_DATA;
        private System.Windows.Forms.ColumnHeader LSV_EVENT_COL_TIME;
        private System.Windows.Forms.Button BTN_CONNECT;
        private System.Windows.Forms.Button BTN_DISCONNECT;
        private System.Windows.Forms.CheckBox CHK_PROBE_PERF;
    }
}
