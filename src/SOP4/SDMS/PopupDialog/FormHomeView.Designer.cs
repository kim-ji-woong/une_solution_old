namespace SDMS
{
    partial class FormHomeView
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
            this.btnHome = new System.Windows.Forms.Button();
            this.btn14Home = new System.Windows.Forms.Button();
            this.btn56Home = new System.Windows.Forms.Button();
            this.btnCoalHome = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnHome
            // 
            this.btnHome.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnHome.Location = new System.Drawing.Point(200, 7);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(58, 32);
            this.btnHome.TabIndex = 1;
            this.btnHome.Text = "#1";
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Visible = false;
            // 
            // btn14Home
            // 
            this.btn14Home.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn14Home.Location = new System.Drawing.Point(8, 7);
            this.btn14Home.Name = "btn14Home";
            this.btn14Home.Size = new System.Drawing.Size(58, 32);
            this.btn14Home.TabIndex = 2;
            this.btn14Home.Text = "#2";
            this.btn14Home.UseVisualStyleBackColor = true;
            // 
            // btn56Home
            // 
            this.btn56Home.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn56Home.Location = new System.Drawing.Point(72, 7);
            this.btn56Home.Name = "btn56Home";
            this.btn56Home.Size = new System.Drawing.Size(58, 32);
            this.btn56Home.TabIndex = 3;
            this.btn56Home.Text = "#3";
            this.btn56Home.UseVisualStyleBackColor = true;
            // 
            // btnCoalHome
            // 
            this.btnCoalHome.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCoalHome.Location = new System.Drawing.Point(136, 7);
            this.btnCoalHome.Name = "btnCoalHome";
            this.btnCoalHome.Size = new System.Drawing.Size(58, 32);
            this.btnCoalHome.TabIndex = 4;
            this.btnCoalHome.Text = "#4";
            this.btnCoalHome.UseVisualStyleBackColor = true;
            // 
            // FormHomeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(201, 46);
            this.ControlBox = false;
            this.Controls.Add(this.btnCoalHome);
            this.Controls.Add(this.btn56Home);
            this.Controls.Add(this.btn14Home);
            this.Controls.Add(this.btnHome);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormHomeView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "FormHomeView";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.Button btn14Home;
        private System.Windows.Forms.Button btn56Home;
        private System.Windows.Forms.Button btnCoalHome;
    }
}