namespace SOPManager
{
    partial class PopupTransSOP
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopupTransSOP));
            this.treeView = new System.Windows.Forms.TreeView();
            this.radioNormal = new System.Windows.Forms.RadioButton();
            this.radioAbnormal = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.rdLabel2 = new System.Windows.Forms.Label();
            this.rdLabel1 = new System.Windows.Forms.Label();
            this.rdPictureBox2 = new System.Windows.Forms.PictureBox();
            this.rdPictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.rdPictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdPictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeView.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            this.treeView.Location = new System.Drawing.Point(12, 44);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(460, 277);
            this.treeView.TabIndex = 0;
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // radioNormal
            // 
            this.radioNormal.AutoSize = true;
            this.radioNormal.BackColor = System.Drawing.Color.Transparent;
            this.radioNormal.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioNormal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.radioNormal.Location = new System.Drawing.Point(313, 0);
            this.radioNormal.Name = "radioNormal";
            this.radioNormal.Size = new System.Drawing.Size(77, 19);
            this.radioNormal.TabIndex = 4;
            this.radioNormal.TabStop = true;
            this.radioNormal.Text = "평일 모드";
            this.radioNormal.UseVisualStyleBackColor = false;
            this.radioNormal.CheckedChanged += new System.EventHandler(this.radioNormal_CheckedChanged);
            // 
            // radioAbnormal
            // 
            this.radioAbnormal.AutoSize = true;
            this.radioAbnormal.BackColor = System.Drawing.Color.Transparent;
            this.radioAbnormal.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioAbnormal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
            this.radioAbnormal.Location = new System.Drawing.Point(313, 18);
            this.radioAbnormal.Name = "radioAbnormal";
            this.radioAbnormal.Size = new System.Drawing.Size(121, 19);
            this.radioAbnormal.TabIndex = 4;
            this.radioAbnormal.TabStop = true;
            this.radioAbnormal.Text = "야간 및 휴일 모드";
            this.radioAbnormal.UseVisualStyleBackColor = false;
            this.radioAbnormal.CheckedChanged += new System.EventHandler(this.radioAbnormal_CheckedChanged);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
            this.btnOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.btnOK.Location = new System.Drawing.Point(242, 336);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(93, 31);
            this.btnOK.TabIndex = 23;
            this.btnOK.Text = "확인";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
            this.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.btnCancel.Location = new System.Drawing.Point(341, 336);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(93, 31);
            this.btnCancel.TabIndex = 23;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // rdLabel2
            // 
            this.rdLabel2.AutoSize = true;
            this.rdLabel2.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rdLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.rdLabel2.Location = new System.Drawing.Point(164, 14);
            this.rdLabel2.Name = "rdLabel2";
            this.rdLabel2.Size = new System.Drawing.Size(114, 17);
            this.rdLabel2.TabIndex = 28;
            this.rdLabel2.Text = "야간 및 휴일 모드";
            this.rdLabel2.Click += new System.EventHandler(this.rdLabel2_Click);
            // 
            // rdLabel1
            // 
            this.rdLabel1.AutoSize = true;
            this.rdLabel1.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rdLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.rdLabel1.Location = new System.Drawing.Point(50, 14);
            this.rdLabel1.Name = "rdLabel1";
            this.rdLabel1.Size = new System.Drawing.Size(65, 17);
            this.rdLabel1.TabIndex = 27;
            this.rdLabel1.Text = "평일 모드";
            this.rdLabel1.Click += new System.EventHandler(this.rdLabel1_Click);
            // 
            // rdPictureBox2
            // 
            this.rdPictureBox2.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_black;
            this.rdPictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rdPictureBox2.Location = new System.Drawing.Point(145, 13);
            this.rdPictureBox2.Name = "rdPictureBox2";
            this.rdPictureBox2.Size = new System.Drawing.Size(18, 17);
            this.rdPictureBox2.TabIndex = 25;
            this.rdPictureBox2.TabStop = false;
            this.rdPictureBox2.Click += new System.EventHandler(this.rdPictureBox2_Click);
            // 
            // rdPictureBox1
            // 
            this.rdPictureBox1.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_black;
            this.rdPictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rdPictureBox1.Location = new System.Drawing.Point(30, 13);
            this.rdPictureBox1.Name = "rdPictureBox1";
            this.rdPictureBox1.Size = new System.Drawing.Size(18, 17);
            this.rdPictureBox1.TabIndex = 26;
            this.rdPictureBox1.TabStop = false;
            this.rdPictureBox1.Click += new System.EventHandler(this.rdPictureBox1_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(3);
            this.panel2.Size = new System.Drawing.Size(484, 394);
            this.panel2.TabIndex = 30;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.panel3.Controls.Add(this.button1);
            this.panel3.Controls.Add(this.btnCancel);
            this.panel3.Controls.Add(this.btnOK);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(478, 388);
            this.panel3.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.button1.Location = new System.Drawing.Point(27, 336);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(93, 31);
            this.button1.TabIndex = 24;
            this.button1.Text = "선택취소";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // PopupTransSOP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.ClientSize = new System.Drawing.Size(484, 394);
            this.Controls.Add(this.rdLabel2);
            this.Controls.Add(this.rdLabel1);
            this.Controls.Add(this.rdPictureBox2);
            this.Controls.Add(this.rdPictureBox1);
            this.Controls.Add(this.radioAbnormal);
            this.Controls.Add(this.radioNormal);
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PopupTransSOP";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "다른 SOP로 전환";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PopupTransSOP_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PopupTransSOP_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PopupTransSOP_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.rdPictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdPictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.RadioButton radioNormal;
        private System.Windows.Forms.RadioButton radioAbnormal;
        private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label rdLabel2;
        private System.Windows.Forms.Label rdLabel1;
        private System.Windows.Forms.PictureBox rdPictureBox2;
		private System.Windows.Forms.PictureBox rdPictureBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button button1;
    }
}