namespace FireManagement
{
    partial class PageBackstageClose
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnCanel = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnSaveExit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.label1.Location = new System.Drawing.Point(291, 159);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(495, 80);
            this.label1.TabIndex = 0;
            this.label1.Text = "          시스템을 종료합니다.\r\n변경된 모든 값을 저장하시겠습니까?\r\n";
            // 
            // btnCanel
            // 
            this.btnCanel.BackgroundImage = global::FireManagement.Properties.Resources.ButtonArea;
            this.btnCanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCanel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnCanel.FlatAppearance.BorderSize = 0;
            this.btnCanel.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.btnCanel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnCanel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnCanel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCanel.Font = new System.Drawing.Font("맑은 고딕", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCanel.Location = new System.Drawing.Point(157, 305);
            this.btnCanel.Name = "btnCanel";
            this.btnCanel.Size = new System.Drawing.Size(207, 90);
            this.btnCanel.TabIndex = 1;
            this.btnCanel.Text = "취소";
            this.btnCanel.UseVisualStyleBackColor = true;
            this.btnCanel.Click += new System.EventHandler(this.btnCanel_Click);
            // 
            // btnExit
            // 
            this.btnExit.BackgroundImage = global::FireManagement.Properties.Resources.ButtonArea;
            this.btnExit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnExit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.btnExit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnExit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Font = new System.Drawing.Font("맑은 고딕", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnExit.Location = new System.Drawing.Point(429, 305);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(207, 90);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "아니오";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSaveExit
            // 
            this.btnSaveExit.BackgroundImage = global::FireManagement.Properties.Resources.ButtonArea;
            this.btnSaveExit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSaveExit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnSaveExit.FlatAppearance.BorderSize = 0;
            this.btnSaveExit.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.btnSaveExit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnSaveExit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnSaveExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveExit.Font = new System.Drawing.Font("맑은 고딕", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSaveExit.Location = new System.Drawing.Point(693, 305);
            this.btnSaveExit.Name = "btnSaveExit";
            this.btnSaveExit.Size = new System.Drawing.Size(207, 90);
            this.btnSaveExit.TabIndex = 3;
            this.btnSaveExit.Text = "예";
            this.btnSaveExit.UseVisualStyleBackColor = true;
            this.btnSaveExit.Click += new System.EventHandler(this.btnSaveExit_Click);
            // 
            // PageBackstageClose
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(1073, 659);
            this.Controls.Add(this.btnSaveExit);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnCanel);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(7)))), ((int)(((byte)(7)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PageBackstageClose";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "PageBackstageClose";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCanel;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnSaveExit;
    }
}