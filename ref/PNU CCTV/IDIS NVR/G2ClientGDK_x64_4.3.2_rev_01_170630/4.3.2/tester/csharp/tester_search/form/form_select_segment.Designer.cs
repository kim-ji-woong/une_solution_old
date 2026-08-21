namespace GDK_tester
{
    partial class form_select_segment
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
            this.LST_SEGMENT = new System.Windows.Forms.ListView();
            this.LST_SEGMENT_COL_ID = new System.Windows.Forms.ColumnHeader();
            this.LST_SEGMENT_COL_TIME = new System.Windows.Forms.ColumnHeader();
            this.BTN_OK = new System.Windows.Forms.Button();
            this.BTN_CANCEL = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LST_SEGMENT
            // 
            this.LST_SEGMENT.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LST_SEGMENT_COL_ID,
            this.LST_SEGMENT_COL_TIME});
            this.LST_SEGMENT.FullRowSelect = true;
            this.LST_SEGMENT.GridLines = true;
            this.LST_SEGMENT.HideSelection = false;
            this.LST_SEGMENT.Location = new System.Drawing.Point(10, 10);
            this.LST_SEGMENT.MultiSelect = false;
            this.LST_SEGMENT.Name = "LST_SEGMENT";
            this.LST_SEGMENT.Size = new System.Drawing.Size(330, 112);
            this.LST_SEGMENT.TabIndex = 0;
            this.LST_SEGMENT.UseCompatibleStateImageBehavior = false;
            this.LST_SEGMENT.View = System.Windows.Forms.View.Details;
            this.LST_SEGMENT.SelectedIndexChanged += new System.EventHandler(this.on_lst_selected_index_changed);
            // 
            // LST_SEGMENT_COL_ID
            // 
            this.LST_SEGMENT_COL_ID.Text = "no.";
            this.LST_SEGMENT_COL_ID.Width = 28;
            // 
            // LST_SEGMENT_COL_TIME
            // 
            this.LST_SEGMENT_COL_TIME.Text = "Time";
            this.LST_SEGMENT_COL_TIME.Width = 280;
            // 
            // BTN_OK
            // 
            this.BTN_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BTN_OK.Location = new System.Drawing.Point(184, 130);
            this.BTN_OK.Name = "BTN_OK";
            this.BTN_OK.Size = new System.Drawing.Size(75, 23);
            this.BTN_OK.TabIndex = 1;
            this.BTN_OK.Text = "OK";
            this.BTN_OK.UseVisualStyleBackColor = true;
            // 
            // BTN_CANCEL
            // 
            this.BTN_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BTN_CANCEL.Location = new System.Drawing.Point(265, 130);
            this.BTN_CANCEL.Name = "BTN_CANCEL";
            this.BTN_CANCEL.Size = new System.Drawing.Size(75, 23);
            this.BTN_CANCEL.TabIndex = 2;
            this.BTN_CANCEL.Text = "Cancel";
            this.BTN_CANCEL.UseVisualStyleBackColor = true;
            // 
            // form_select_segment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 162);
            this.Controls.Add(this.LST_SEGMENT);
            this.Controls.Add(this.BTN_CANCEL);
            this.Controls.Add(this.BTN_OK);
            this.AcceptButton = this.BTN_OK;
            this.CancelButton = this.BTN_CANCEL;
            this.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "form_select_segment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Segment";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView LST_SEGMENT;
        private System.Windows.Forms.ColumnHeader LST_SEGMENT_COL_ID;
        private System.Windows.Forms.ColumnHeader LST_SEGMENT_COL_TIME;
        private System.Windows.Forms.Button BTN_OK;
        private System.Windows.Forms.Button BTN_CANCEL;
    }
}