namespace GDK_tester
{
    partial class form_backup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(form_backup));
            this.STC_SCREEN = new System.Windows.Forms.Label();
            this.STC_CONTROLLER = new System.Windows.Forms.Label();
            this.LSV_EVENT = new System.Windows.Forms.ListView();
            this.LSV_EVENT_COL_EVENT = new System.Windows.Forms.ColumnHeader();
            this.LSV_EVENT_COL_TRIGGERED = new System.Windows.Forms.ColumnHeader();
            this.LSV_EVENT_COL_DEVICE = new System.Windows.Forms.ColumnHeader();
            this.LSV_EVENT_COL_TIME = new System.Windows.Forms.ColumnHeader();
            this.STC_TIME_TABLE = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // STC_SCREEN
            // 
            this.STC_SCREEN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.STC_SCREEN.Location = new System.Drawing.Point(9, 11);
            this.STC_SCREEN.Name = "STC_SCREEN";
            this.STC_SCREEN.Size = new System.Drawing.Size(599, 365);
            this.STC_SCREEN.TabIndex = 1;
            this.STC_SCREEN.Text = "screen";
            this.STC_SCREEN.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.STC_SCREEN.Visible = false;
            // 
            // STC_CONTROLLER
            // 
            this.STC_CONTROLLER.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.STC_CONTROLLER.Location = new System.Drawing.Point(9, 382);
            this.STC_CONTROLLER.Name = "STC_CONTROLLER";
            this.STC_CONTROLLER.Size = new System.Drawing.Size(599, 162);
            this.STC_CONTROLLER.TabIndex = 8;
            this.STC_CONTROLLER.Text = "controller";
            this.STC_CONTROLLER.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.STC_CONTROLLER.Visible = false;
            // 
            // LSV_EVENT
            // 
            this.LSV_EVENT.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LSV_EVENT_COL_EVENT,
            this.LSV_EVENT_COL_TRIGGERED,
            this.LSV_EVENT_COL_DEVICE,
            this.LSV_EVENT_COL_TIME});
            this.LSV_EVENT.FullRowSelect = true;
            this.LSV_EVENT.GridLines = true;
            this.LSV_EVENT.HideSelection = false;
            this.LSV_EVENT.Location = new System.Drawing.Point(9, 551);
            this.LSV_EVENT.MultiSelect = false;
            this.LSV_EVENT.Name = "LSV_EVENT";
            this.LSV_EVENT.ShowItemToolTips = true;
            this.LSV_EVENT.Size = new System.Drawing.Size(599, 112);
            this.LSV_EVENT.TabIndex = 11;
            this.LSV_EVENT.UseCompatibleStateImageBehavior = false;
            this.LSV_EVENT.View = System.Windows.Forms.View.Details;
            this.LSV_EVENT.SelectedIndexChanged += new System.EventHandler(this.on_event_list_selected_changed);
            // 
            // LSV_EVENT_COL_EVENT
            // 
            this.LSV_EVENT_COL_EVENT.Text = "Event";
            this.LSV_EVENT_COL_EVENT.Width = 160;
            // 
            // LSV_EVENT_COL_TRIGGERED
            // 
            this.LSV_EVENT_COL_TRIGGERED.Text = "Triggered by";
            this.LSV_EVENT_COL_TRIGGERED.Width = 130;
            // 
            // LSV_EVENT_COL_DEVICE
            // 
            this.LSV_EVENT_COL_DEVICE.Text = "Device";
            this.LSV_EVENT_COL_DEVICE.Width = 130;
            // 
            // LSV_EVENT_COL_TIME
            // 
            this.LSV_EVENT_COL_TIME.Text = "Time";
            this.LSV_EVENT_COL_TIME.Width = 170;
            // 
            // STC_TIME_TABLE
            // 
            this.STC_TIME_TABLE.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.STC_TIME_TABLE.Location = new System.Drawing.Point(9, 551);
            this.STC_TIME_TABLE.Name = "STC_TIME_TABLE";
            this.STC_TIME_TABLE.Size = new System.Drawing.Size(599, 112);
            this.STC_TIME_TABLE.TabIndex = 12;
            this.STC_TIME_TABLE.Text = "time table";
            this.STC_TIME_TABLE.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.STC_TIME_TABLE.Visible = false;
            // 
            // form_backup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 672);
            this.Controls.Add(this.STC_TIME_TABLE);
            this.Controls.Add(this.LSV_EVENT);
            this.Controls.Add(this.STC_CONTROLLER);
            this.Controls.Add(this.STC_SCREEN);
            this.Font = new System.Drawing.Font("Tahoma", 8F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "form_backup";
            this.Text = "GDK Service B/a/c/k/u/p";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.on_form_closing);
            this.ResumeLayout(false);

        }

        #endregion

        private GDK_tester.screen_pane SCREEN_PANE;
        private System.Windows.Forms.Label STC_SCREEN;
        private System.Windows.Forms.Label STC_CONTROLLER;
        private System.Windows.Forms.ListView LSV_EVENT;
        private System.Windows.Forms.ColumnHeader LSV_EVENT_COL_EVENT;
        private System.Windows.Forms.ColumnHeader LSV_EVENT_COL_TRIGGERED;
        private System.Windows.Forms.ColumnHeader LSV_EVENT_COL_DEVICE;
        private System.Windows.Forms.ColumnHeader LSV_EVENT_COL_TIME;
        private System.Windows.Forms.Label STC_TIME_TABLE;
    }
}