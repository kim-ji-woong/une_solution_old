namespace GDK_tester
{
    partial class form_ptz_command
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
            this.CHK_PAN = new System.Windows.Forms.CheckBox();
            this.CHK_TILT = new System.Windows.Forms.CheckBox();
            this.CHK_ZOOM = new System.Windows.Forms.CheckBox();
            this.CHK_FOCUS = new System.Windows.Forms.CheckBox();
            this.CHK_IRIS = new System.Windows.Forms.CheckBox();
            this.EDT_PAN = new GDK_tester.text_box_numeric();
            this.EDT_TILT = new GDK_tester.text_box_numeric();
            this.EDT_ZOOM = new GDK_tester.text_box_numeric();
            this.EDT_FOCUS = new GDK_tester.text_box_numeric();
            this.EDT_IRIS = new GDK_tester.text_box_numeric();
            this.EDT_PAN_RANGE = new System.Windows.Forms.TextBox();
            this.EDT_TILT_RANGE = new System.Windows.Forms.TextBox();
            this.EDT_ZOOM_RANGE = new System.Windows.Forms.TextBox();
            this.EDT_FOCUS_RANGE = new System.Windows.Forms.TextBox();
            this.EDT_IRIS_RANGE = new System.Windows.Forms.TextBox();
            this.BTN_SET = new System.Windows.Forms.Button();
            this.BTN_CANCEL = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CHK_PAN
            // 
            this.CHK_PAN.AutoSize = true;
            this.CHK_PAN.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CHK_PAN.Location = new System.Drawing.Point(27, 14);
            this.CHK_PAN.Name = "CHK_PAN";
            this.CHK_PAN.Size = new System.Drawing.Size(44, 17);
            this.CHK_PAN.TabIndex = 0;
            this.CHK_PAN.Text = "Pan";
            this.CHK_PAN.UseVisualStyleBackColor = true;
            this.CHK_PAN.CheckedChanged += new System.EventHandler(this.on_chk_changed);
            // 
            // CHK_TILT
            // 
            this.CHK_TILT.AutoSize = true;
            this.CHK_TILT.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CHK_TILT.Location = new System.Drawing.Point(31, 40);
            this.CHK_TILT.Name = "CHK_TILT";
            this.CHK_TILT.Size = new System.Drawing.Size(40, 17);
            this.CHK_TILT.TabIndex = 2;
            this.CHK_TILT.Text = "Tilt";
            this.CHK_TILT.UseVisualStyleBackColor = true;
            this.CHK_TILT.CheckedChanged += new System.EventHandler(this.on_chk_changed);
            // 
            // CHK_ZOOM
            // 
            this.CHK_ZOOM.AutoSize = true;
            this.CHK_ZOOM.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CHK_ZOOM.Location = new System.Drawing.Point(19, 66);
            this.CHK_ZOOM.Name = "CHK_ZOOM";
            this.CHK_ZOOM.Size = new System.Drawing.Size(52, 17);
            this.CHK_ZOOM.TabIndex = 4;
            this.CHK_ZOOM.Text = "Zoom";
            this.CHK_ZOOM.UseVisualStyleBackColor = true;
            this.CHK_ZOOM.CheckedChanged += new System.EventHandler(this.on_chk_changed);
            // 
            // CHK_FOCUS
            // 
            this.CHK_FOCUS.AutoSize = true;
            this.CHK_FOCUS.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CHK_FOCUS.Location = new System.Drawing.Point(17, 92);
            this.CHK_FOCUS.Name = "CHK_FOCUS";
            this.CHK_FOCUS.Size = new System.Drawing.Size(54, 17);
            this.CHK_FOCUS.TabIndex = 6;
            this.CHK_FOCUS.Text = "Focus";
            this.CHK_FOCUS.UseVisualStyleBackColor = true;
            this.CHK_FOCUS.CheckedChanged += new System.EventHandler(this.on_chk_changed);
            // 
            // CHK_IRIS
            // 
            this.CHK_IRIS.AutoSize = true;
            this.CHK_IRIS.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CHK_IRIS.Location = new System.Drawing.Point(30, 118);
            this.CHK_IRIS.Name = "CHK_IRIS";
            this.CHK_IRIS.Size = new System.Drawing.Size(41, 17);
            this.CHK_IRIS.TabIndex = 8;
            this.CHK_IRIS.Text = "Iris";
            this.CHK_IRIS.UseVisualStyleBackColor = true;
            this.CHK_IRIS.CheckedChanged += new System.EventHandler(this.on_chk_changed);
            // 
            // EDT_PAN
            // 
            this.EDT_PAN.AllowSpace = false;
            this.EDT_PAN.Location = new System.Drawing.Point(77, 12);
            this.EDT_PAN.Name = "EDT_PAN";
            this.EDT_PAN.Size = new System.Drawing.Size(80, 20);
            this.EDT_PAN.TabIndex = 1;
            // 
            // EDT_TILT
            // 
            this.EDT_TILT.AllowSpace = false;
            this.EDT_TILT.Location = new System.Drawing.Point(77, 38);
            this.EDT_TILT.Name = "EDT_TILT";
            this.EDT_TILT.Size = new System.Drawing.Size(80, 20);
            this.EDT_TILT.TabIndex = 3;
            // 
            // EDT_ZOOM
            // 
            this.EDT_ZOOM.AllowSpace = false;
            this.EDT_ZOOM.Location = new System.Drawing.Point(77, 64);
            this.EDT_ZOOM.Name = "EDT_ZOOM";
            this.EDT_ZOOM.Size = new System.Drawing.Size(80, 20);
            this.EDT_ZOOM.TabIndex = 5;
            // 
            // EDT_FOCUS
            // 
            this.EDT_FOCUS.AllowSpace = false;
            this.EDT_FOCUS.Location = new System.Drawing.Point(77, 90);
            this.EDT_FOCUS.Name = "EDT_FOCUS";
            this.EDT_FOCUS.Size = new System.Drawing.Size(80, 20);
            this.EDT_FOCUS.TabIndex = 7;
            // 
            // EDT_IRIS
            // 
            this.EDT_IRIS.AllowSpace = false;
            this.EDT_IRIS.Location = new System.Drawing.Point(77, 116);
            this.EDT_IRIS.Name = "EDT_IRIS";
            this.EDT_IRIS.Size = new System.Drawing.Size(80, 20);
            this.EDT_IRIS.TabIndex = 9;
            // 
            // EDT_PAN_RANGE
            // 
            this.EDT_PAN_RANGE.Location = new System.Drawing.Point(161, 12);
            this.EDT_PAN_RANGE.Name = "EDT_PAN_RANGE";
            this.EDT_PAN_RANGE.ReadOnly = true;
            this.EDT_PAN_RANGE.Size = new System.Drawing.Size(90, 20);
            this.EDT_PAN_RANGE.TabIndex = 12;
            // 
            // EDT_TILT_RANGE
            // 
            this.EDT_TILT_RANGE.Location = new System.Drawing.Point(161, 38);
            this.EDT_TILT_RANGE.Name = "EDT_TILT_RANGE";
            this.EDT_TILT_RANGE.ReadOnly = true;
            this.EDT_TILT_RANGE.Size = new System.Drawing.Size(90, 20);
            this.EDT_TILT_RANGE.TabIndex = 13;
            // 
            // EDT_ZOOM_RANGE
            // 
            this.EDT_ZOOM_RANGE.Location = new System.Drawing.Point(161, 64);
            this.EDT_ZOOM_RANGE.Name = "EDT_ZOOM_RANGE";
            this.EDT_ZOOM_RANGE.ReadOnly = true;
            this.EDT_ZOOM_RANGE.Size = new System.Drawing.Size(90, 20);
            this.EDT_ZOOM_RANGE.TabIndex = 14;
            // 
            // EDT_FOCUS_RANGE
            // 
            this.EDT_FOCUS_RANGE.Location = new System.Drawing.Point(161, 90);
            this.EDT_FOCUS_RANGE.Name = "EDT_FOCUS_RANGE";
            this.EDT_FOCUS_RANGE.ReadOnly = true;
            this.EDT_FOCUS_RANGE.Size = new System.Drawing.Size(90, 20);
            this.EDT_FOCUS_RANGE.TabIndex = 15;
            // 
            // EDT_IRIS_RANGE
            // 
            this.EDT_IRIS_RANGE.Location = new System.Drawing.Point(161, 116);
            this.EDT_IRIS_RANGE.Name = "EDT_IRIS_RANGE";
            this.EDT_IRIS_RANGE.ReadOnly = true;
            this.EDT_IRIS_RANGE.Size = new System.Drawing.Size(90, 20);
            this.EDT_IRIS_RANGE.TabIndex = 16;
            // 
            // BTN_SET
            // 
            this.BTN_SET.Location = new System.Drawing.Point(95, 152);
            this.BTN_SET.Name = "BTN_SET";
            this.BTN_SET.Size = new System.Drawing.Size(75, 23);
            this.BTN_SET.TabIndex = 10;
            this.BTN_SET.Text = "Set";
            this.BTN_SET.UseVisualStyleBackColor = true;
            this.BTN_SET.Click += new System.EventHandler(this.on_btn_set);
            // 
            // BTN_CANCEL
            // 
            this.BTN_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BTN_CANCEL.Location = new System.Drawing.Point(176, 152);
            this.BTN_CANCEL.Name = "BTN_CANCEL";
            this.BTN_CANCEL.Size = new System.Drawing.Size(75, 23);
            this.BTN_CANCEL.TabIndex = 11;
            this.BTN_CANCEL.Text = "Cancel";
            this.BTN_CANCEL.UseVisualStyleBackColor = true;
            // 
            // form_ptz_command
            // 
            this.AcceptButton = this.BTN_SET;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BTN_CANCEL;
            this.ClientSize = new System.Drawing.Size(262, 184);
            this.Controls.Add(this.CHK_IRIS);
            this.Controls.Add(this.CHK_FOCUS);
            this.Controls.Add(this.CHK_ZOOM);
            this.Controls.Add(this.CHK_TILT);
            this.Controls.Add(this.CHK_PAN);
            this.Controls.Add(this.EDT_PAN);
            this.Controls.Add(this.EDT_TILT);
            this.Controls.Add(this.EDT_ZOOM);
            this.Controls.Add(this.EDT_FOCUS);
            this.Controls.Add(this.EDT_IRIS);
            this.Controls.Add(this.EDT_PAN_RANGE);
            this.Controls.Add(this.EDT_TILT_RANGE);
            this.Controls.Add(this.EDT_ZOOM_RANGE);
            this.Controls.Add(this.EDT_FOCUS_RANGE);
            this.Controls.Add(this.EDT_IRIS_RANGE);
            this.Controls.Add(this.BTN_SET);
            this.Controls.Add(this.BTN_CANCEL);
            this.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "form_ptz_command";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PTZ command control";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox CHK_PAN;
        private System.Windows.Forms.CheckBox CHK_TILT;
        private System.Windows.Forms.CheckBox CHK_ZOOM;
        private System.Windows.Forms.CheckBox CHK_FOCUS;
        private System.Windows.Forms.CheckBox CHK_IRIS;
        private GDK_tester.text_box_numeric EDT_PAN;
        private GDK_tester.text_box_numeric EDT_TILT;
        private GDK_tester.text_box_numeric EDT_ZOOM;
        private GDK_tester.text_box_numeric EDT_FOCUS;
        private GDK_tester.text_box_numeric EDT_IRIS;
        private System.Windows.Forms.TextBox EDT_PAN_RANGE;
        private System.Windows.Forms.TextBox EDT_TILT_RANGE;
        private System.Windows.Forms.TextBox EDT_ZOOM_RANGE;
        private System.Windows.Forms.TextBox EDT_FOCUS_RANGE;
        private System.Windows.Forms.TextBox EDT_IRIS_RANGE;
        private System.Windows.Forms.Button BTN_SET;
        private System.Windows.Forms.Button BTN_CANCEL;
    }
}
