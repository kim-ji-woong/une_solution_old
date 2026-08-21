namespace SoilMan.DockingForm
{
    partial class FormLayer
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
            this.panelTop = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxSuchi = new System.Windows.Forms.CheckBox();
            this.checkBoxTozi = new System.Windows.Forms.CheckBox();
            this.checkBoxZigeoc = new System.Windows.Forms.CheckBox();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.White;
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(284, 38);
            this.panelTop.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 21);
            this.label1.TabIndex = 4;
            this.label1.Text = "도면층";
            // 
            // checkBoxSuchi
            // 
            this.checkBoxSuchi.AutoSize = true;
            this.checkBoxSuchi.Enabled = false;
            this.checkBoxSuchi.Location = new System.Drawing.Point(13, 64);
            this.checkBoxSuchi.Name = "checkBoxSuchi";
            this.checkBoxSuchi.Size = new System.Drawing.Size(84, 16);
            this.checkBoxSuchi.TabIndex = 1;
            this.checkBoxSuchi.Text = "수치지형도";
            this.checkBoxSuchi.UseVisualStyleBackColor = true;
            this.checkBoxSuchi.CheckedChanged += new System.EventHandler(this.checkBoxSuchi_CheckedChanged);
            // 
            // checkBoxTozi
            // 
            this.checkBoxTozi.AutoSize = true;
            this.checkBoxTozi.Enabled = false;
            this.checkBoxTozi.Location = new System.Drawing.Point(13, 99);
            this.checkBoxTozi.Name = "checkBoxTozi";
            this.checkBoxTozi.Size = new System.Drawing.Size(112, 16);
            this.checkBoxTozi.TabIndex = 1;
            this.checkBoxTozi.Text = "토지이용 계획도";
            this.checkBoxTozi.UseVisualStyleBackColor = true;
            this.checkBoxTozi.CheckedChanged += new System.EventHandler(this.checkBoxTozi_CheckedChanged);
            // 
            // checkBoxZigeoc
            // 
            this.checkBoxZigeoc.AutoSize = true;
            this.checkBoxZigeoc.Enabled = false;
            this.checkBoxZigeoc.Location = new System.Drawing.Point(12, 134);
            this.checkBoxZigeoc.Name = "checkBoxZigeoc";
            this.checkBoxZigeoc.Size = new System.Drawing.Size(60, 16);
            this.checkBoxZigeoc.TabIndex = 1;
            this.checkBoxZigeoc.Text = "지적도";
            this.checkBoxZigeoc.UseVisualStyleBackColor = true;
            this.checkBoxZigeoc.CheckedChanged += new System.EventHandler(this.checkBoxZigeoc_CheckedChanged);
            // 
            // FormLayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.checkBoxZigeoc);
            this.Controls.Add(this.checkBoxTozi);
            this.Controls.Add(this.checkBoxSuchi);
            this.Controls.Add(this.panelTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLayer";
            this.ShowInTaskbar = false;
            this.Text = "FormLayer";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxSuchi;
        private System.Windows.Forms.CheckBox checkBoxTozi;
        private System.Windows.Forms.CheckBox checkBoxZigeoc;
    }
}