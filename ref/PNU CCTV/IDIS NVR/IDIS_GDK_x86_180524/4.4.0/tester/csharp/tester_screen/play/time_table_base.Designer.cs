namespace GDK_tester
{
    partial class time_table_base
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
            this.SuspendLayout();
            // 
            // time_table_base
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 80);
            this.ControlBox = false;
            this.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "time_table_base";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "time_table_base";
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.on_mouse_up);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.on_paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.on_mouse_down);
            this.Resize += new System.EventHandler(this.on_resize);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.on_mouse_move);
            this.ResumeLayout(false);

        }

        #endregion
    }
}