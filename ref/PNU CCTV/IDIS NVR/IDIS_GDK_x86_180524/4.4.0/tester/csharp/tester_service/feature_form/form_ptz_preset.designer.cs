namespace GDK_tester
{
    partial class form_ptz_preset
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
            this.LST_PRESET = new System.Windows.Forms.ListView();
            this.LST_PRESET_COL_NUM = new System.Windows.Forms.ColumnHeader();
            this.LST_PRESET_COL_TITLE = new System.Windows.Forms.ColumnHeader();
            this.STC_TITLE = new System.Windows.Forms.Label();
            this.EDT_TITLE = new System.Windows.Forms.TextBox();
            this.BTN_OK = new System.Windows.Forms.Button();
            this.BTN_CANCEL = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LST_PRESET
            // 
            this.LST_PRESET.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LST_PRESET_COL_NUM,
            this.LST_PRESET_COL_TITLE});
            this.LST_PRESET.FullRowSelect = true;
            this.LST_PRESET.GridLines = true;
            this.LST_PRESET.HideSelection = false;
            this.LST_PRESET.Location = new System.Drawing.Point(10, 10);
            this.LST_PRESET.MultiSelect = false;
            this.LST_PRESET.Name = "LST_PRESET";
            this.LST_PRESET.Size = new System.Drawing.Size(221, 230);
            this.LST_PRESET.TabIndex = 0;
            this.LST_PRESET.UseCompatibleStateImageBehavior = false;
            this.LST_PRESET.View = System.Windows.Forms.View.Details;
            this.LST_PRESET.SelectedIndexChanged += new System.EventHandler(this.on_lst_select_changed);
            this.LST_PRESET.DoubleClick += new System.EventHandler(this.on_lst_double_click);
            // 
            // LST_PRESET_COL_NUM
            // 
            this.LST_PRESET_COL_NUM.Text = "no.";
            this.LST_PRESET_COL_NUM.Width = 34;
            // 
            // LST_PRESET_COL_TITLE
            // 
            this.LST_PRESET_COL_TITLE.Text = "";
            this.LST_PRESET_COL_TITLE.Width = 166;
            // 
            // STC_TITLE
            // 
            this.STC_TITLE.AutoSize = true;
            this.STC_TITLE.Location = new System.Drawing.Point(23, 249);
            this.STC_TITLE.Name = "STC_TITLE";
            this.STC_TITLE.Size = new System.Drawing.Size(27, 13);
            this.STC_TITLE.TabIndex = 1;
            this.STC_TITLE.Text = "Title";
            // 
            // EDT_TITLE
            // 
            this.EDT_TITLE.Location = new System.Drawing.Point(54, 246);
            this.EDT_TITLE.MaxLength = 64;
            this.EDT_TITLE.Name = "EDT_TITLE";
            this.EDT_TITLE.Size = new System.Drawing.Size(177, 20);
            this.EDT_TITLE.TabIndex = 1;
            // 
            // BTN_OK
            // 
            this.BTN_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BTN_OK.Enabled = false;
            this.BTN_OK.Location = new System.Drawing.Point(75, 272);
            this.BTN_OK.Name = "BTN_OK";
            this.BTN_OK.Size = new System.Drawing.Size(75, 23);
            this.BTN_OK.TabIndex = 2;
            this.BTN_OK.Text = "OK";
            this.BTN_OK.UseVisualStyleBackColor = true;
            this.BTN_OK.Click += new System.EventHandler(this.on_btn_ok);
            // 
            // BTN_CANCEL
            // 
            this.BTN_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BTN_CANCEL.Location = new System.Drawing.Point(156, 272);
            this.BTN_CANCEL.Name = "BTN_CANCEL";
            this.BTN_CANCEL.Size = new System.Drawing.Size(75, 23);
            this.BTN_CANCEL.TabIndex = 3;
            this.BTN_CANCEL.Text = "Cancel";
            this.BTN_CANCEL.UseVisualStyleBackColor = true;
            // 
            // form_ptz_preset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(240, 304);
            this.Controls.Add(this.BTN_CANCEL);
            this.Controls.Add(this.BTN_OK);
            this.Controls.Add(this.EDT_TITLE);
            this.Controls.Add(this.STC_TITLE);
            this.Controls.Add(this.LST_PRESET);
            this.AcceptButton = this.BTN_OK;
            this.CancelButton = this.BTN_CANCEL;
            this.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "form_ptz_preset";
            this.Opacity = 0.9;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PTZ preset";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView LST_PRESET;
        private System.Windows.Forms.ColumnHeader LST_PRESET_COL_NUM;
        private System.Windows.Forms.ColumnHeader LST_PRESET_COL_TITLE;
        private System.Windows.Forms.Label STC_TITLE;
        private System.Windows.Forms.TextBox EDT_TITLE;
        private System.Windows.Forms.Button BTN_OK;
        private System.Windows.Forms.Button BTN_CANCEL;
    }
}