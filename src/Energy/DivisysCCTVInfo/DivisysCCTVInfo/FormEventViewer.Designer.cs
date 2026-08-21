namespace DivisysCCTVInfo
{
    partial class FormEventViewer
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
            this.textBoxAllEvent = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxHostEvent = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxAllEvent
            // 
            this.textBoxAllEvent.Location = new System.Drawing.Point(12, 35);
            this.textBoxAllEvent.Multiline = true;
            this.textBoxAllEvent.Name = "textBoxAllEvent";
            this.textBoxAllEvent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxAllEvent.Size = new System.Drawing.Size(410, 403);
            this.textBoxAllEvent.TabIndex = 0;
            this.textBoxAllEvent.TextChanged += new System.EventHandler(this.TextBoxEvent_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "전체 이벤트";
            // 
            // textBoxHostEvent
            // 
            this.textBoxHostEvent.Location = new System.Drawing.Point(428, 35);
            this.textBoxHostEvent.Multiline = true;
            this.textBoxHostEvent.Name = "textBoxHostEvent";
            this.textBoxHostEvent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxHostEvent.Size = new System.Drawing.Size(410, 403);
            this.textBoxHostEvent.TabIndex = 0;
            this.textBoxHostEvent.TextChanged += new System.EventHandler(this.TextBoxEvent_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(428, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(166, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Host 및 채널 이벤트";
            // 
            // FormEventViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(847, 450);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxHostEvent);
            this.Controls.Add(this.textBoxAllEvent);
            this.Name = "FormEventViewer";
            this.Text = "Divisys OCX Event";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxAllEvent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxHostEvent;
        private System.Windows.Forms.Label label2;
    }
}