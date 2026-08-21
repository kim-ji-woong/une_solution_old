namespace GDK_tester
{
    partial class form_event_search
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Root Device");
            this.STC_FROM = new System.Windows.Forms.Label();
            this.STC_TO = new System.Windows.Forms.Label();
            this.CHK_FIRST = new System.Windows.Forms.CheckBox();
            this.CHK_LAST = new System.Windows.Forms.CheckBox();
            this.DTP_FROM = new System.Windows.Forms.DateTimePicker();
            this.DTP_TO = new System.Windows.Forms.DateTimePicker();
            this.TRV_DEVICES = new System.Windows.Forms.TreeView();
            this.BTN_OK = new System.Windows.Forms.Button();
            this.BTN_CANCEL = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // STC_FROM
            // 
            this.STC_FROM.Location = new System.Drawing.Point(12, 11);
            this.STC_FROM.Name = "STC_FROM";
            this.STC_FROM.Size = new System.Drawing.Size(55, 17);
            this.STC_FROM.TabIndex = 16;
            this.STC_FROM.Text = "From :";
            this.STC_FROM.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // STC_TO
            // 
            this.STC_TO.Location = new System.Drawing.Point(12, 34);
            this.STC_TO.Name = "STC_TO";
            this.STC_TO.Size = new System.Drawing.Size(55, 17);
            this.STC_TO.TabIndex = 17;
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
            this.CHK_LAST.Location = new System.Drawing.Point(73, 35);
            this.CHK_LAST.Name = "CHK_LAST";
            this.CHK_LAST.Size = new System.Drawing.Size(46, 17);
            this.CHK_LAST.TabIndex = 1;
            this.CHK_LAST.Text = "Last";
            this.CHK_LAST.UseVisualStyleBackColor = true;
            this.CHK_LAST.CheckedChanged += new System.EventHandler(this.on_chk_last);
            // 
            // DTP_FROM
            // 
            this.DTP_FROM.CustomFormat = " yyyy-MM-dd HH:mm:ss";
            this.DTP_FROM.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DTP_FROM.Location = new System.Drawing.Point(126, 9);
            this.DTP_FROM.Name = "DTP_FROM";
            this.DTP_FROM.Size = new System.Drawing.Size(148, 20);
            this.DTP_FROM.TabIndex = 2;
            this.DTP_FROM.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.on_dtp_mouse_wheel);
            // 
            // DTP_TO
            // 
            this.DTP_TO.CustomFormat = " yyyy-MM-dd HH:mm:ss";
            this.DTP_TO.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DTP_TO.Location = new System.Drawing.Point(125, 35);
            this.DTP_TO.Name = "DTP_TO";
            this.DTP_TO.Size = new System.Drawing.Size(149, 20);
            this.DTP_TO.TabIndex = 3;
            this.DTP_TO.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.on_dtp_mouse_wheel);
            // 
            // TRV_DEVICES
            // 
            this.TRV_DEVICES.CheckBoxes = true;
            this.TRV_DEVICES.Location = new System.Drawing.Point(12, 61);
            this.TRV_DEVICES.Name = "TRV_DEVICES";
            treeNode1.Name = "ROOT_DEVICE";
            treeNode1.Text = "Root Device";
            this.TRV_DEVICES.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.TRV_DEVICES.Size = new System.Drawing.Size(390, 200);
            this.TRV_DEVICES.TabIndex = 4;
            // 
            // BTN_OK
            // 
            this.BTN_OK.Location = new System.Drawing.Point(246, 267);
            this.BTN_OK.Name = "BTN_OK";
            this.BTN_OK.Size = new System.Drawing.Size(75, 23);
            this.BTN_OK.TabIndex = 5;
            this.BTN_OK.Text = "OK";
            this.BTN_OK.UseVisualStyleBackColor = true;
            this.BTN_OK.Click += new System.EventHandler(this.on_btn_OK);
            // 
            // BTN_CANCEL
            // 
            this.BTN_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BTN_CANCEL.Location = new System.Drawing.Point(327, 267);
            this.BTN_CANCEL.Name = "BTN_CANCEL";
            this.BTN_CANCEL.Size = new System.Drawing.Size(75, 23);
            this.BTN_CANCEL.TabIndex = 6;
            this.BTN_CANCEL.Text = "Cancel";
            this.BTN_CANCEL.UseVisualStyleBackColor = true;
            // 
            // form_event_search
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 303);
            this.Controls.Add(this.STC_TO);
            this.Controls.Add(this.STC_FROM);
            this.Controls.Add(this.TRV_DEVICES);
            this.Controls.Add(this.CHK_FIRST);
            this.Controls.Add(this.CHK_LAST);
            this.Controls.Add(this.DTP_FROM);
            this.Controls.Add(this.DTP_TO);
            this.Controls.Add(this.BTN_OK);
            this.Controls.Add(this.BTN_CANCEL);
            this.AcceptButton = this.BTN_OK;
            this.CancelButton = this.BTN_CANCEL;
            this.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "form_event_search";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Event Search";
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
        private System.Windows.Forms.Button BTN_OK;
        private System.Windows.Forms.Button BTN_CANCEL;
    }
}
