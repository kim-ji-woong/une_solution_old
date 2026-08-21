namespace UEControlSample
{
    partial class FormImageButton
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
            this.imageButton1 = new UnE.GUI.ImageButton();
            this.imageButton2 = new UnE.GUI.ImageButton();
            ((System.ComponentModel.ISupportInitialize)(this.imageButton1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageButton2)).BeginInit();
            this.SuspendLayout();
            // 
            // imageButton1
            // 
            this.imageButton1.ButtonText = "Enabled";
            this.imageButton1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.imageButton1.ImageClicked = global::UEControlSample.Properties.Resources.click_btnhome;
            this.imageButton1.ImageDisabled = global::UEControlSample.Properties.Resources.home_disabled;
            this.imageButton1.ImageMouseOver = global::UEControlSample.Properties.Resources.mouse_over_home;
            this.imageButton1.ImageNormal = global::UEControlSample.Properties.Resources.home_86_82;
            this.imageButton1.Location = new System.Drawing.Point(45, 47);
            this.imageButton1.Name = "imageButton1";
            this.imageButton1.Owner = null;
            this.imageButton1.Size = new System.Drawing.Size(100, 117);
            this.imageButton1.TabIndex = 0;
            this.imageButton1.TabStop = false;
            this.imageButton1.Text = "Enabled";
            this.imageButton1.TextColor = System.Drawing.Color.Goldenrod;
            this.imageButton1.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            // 
            // imageButton2
            // 
            this.imageButton2.ButtonText = "Disabled";
            this.imageButton2.Enabled = false;
            this.imageButton2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.imageButton2.ImageClicked = global::UEControlSample.Properties.Resources.click_btnhome;
            this.imageButton2.ImageDisabled = global::UEControlSample.Properties.Resources.home_disabled;
            this.imageButton2.ImageMouseOver = global::UEControlSample.Properties.Resources.mouse_over_home;
            this.imageButton2.ImageNormal = global::UEControlSample.Properties.Resources.home_86_82;
            this.imageButton2.Location = new System.Drawing.Point(151, 47);
            this.imageButton2.Name = "imageButton2";
            this.imageButton2.Owner = null;
            this.imageButton2.Size = new System.Drawing.Size(100, 117);
            this.imageButton2.TabIndex = 0;
            this.imageButton2.TabStop = false;
            this.imageButton2.Text = "Disabled";
            this.imageButton2.TextColor = System.Drawing.Color.Maroon;
            this.imageButton2.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            // 
            // FormImageButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.imageButton2);
            this.Controls.Add(this.imageButton1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormImageButton";
            this.Text = "FormImageButton";
            this.Load += new System.EventHandler(this.FormImageButton_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imageButton1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageButton2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private UnE.GUI.ImageButton imageButton1;
        private UnE.GUI.ImageButton imageButton2;
    }
}