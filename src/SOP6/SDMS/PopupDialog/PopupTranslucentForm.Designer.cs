namespace SDMS
{
    partial class PopupTranslucentForm
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
            this.button1 = new UnE.GUI.ImageButton();
            ((System.ComponentModel.ISupportInitialize)(this.button1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.ButtonText = "";
            this.button1.ImageClicked = global::SDMS.Properties.Resources.Close_40_40_Click;
            this.button1.ImageDisabled = null;
            this.button1.ImageMouseOver = global::SDMS.Properties.Resources.Close_40_40_Click;
            this.button1.ImageNormal = global::SDMS.Properties.Resources.Close_40_40_Default;
            this.button1.Location = new System.Drawing.Point(815, 10);
            this.button1.Name = "button1";
            this.button1.Owner = null;
            this.button1.Size = new System.Drawing.Size(40, 40);
            this.button1.TabIndex = 1;
            this.button1.TabStop = false;
            this.button1.TextColor = System.Drawing.Color.Black;
            this.button1.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button1.ToolTipText = "";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // PopupTranslucentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(867, 516);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PopupTranslucentForm";
            this.Opacity = 0.8D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "PopupTranslucent_Form";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PopupTranslucentForm_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.PopupTranslucentForm_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.button1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private UnE.GUI.ImageButton button1;

    }
}