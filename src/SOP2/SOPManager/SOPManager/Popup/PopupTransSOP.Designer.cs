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
			this.treeView = new System.Windows.Forms.TreeView();
			this.radioNormal = new System.Windows.Forms.RadioButton();
			this.radioAbnormal = new System.Windows.Forms.RadioButton();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.rdLabel2 = new System.Windows.Forms.Label();
			this.rdLabel1 = new System.Windows.Forms.Label();
			this.rdPictureBox2 = new System.Windows.Forms.PictureBox();
			this.rdPictureBox1 = new System.Windows.Forms.PictureBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.rdPictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rdPictureBox1)).BeginInit();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// treeView
			// 
			this.treeView.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.treeView.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
			this.treeView.Location = new System.Drawing.Point(12, 82);
			this.treeView.Name = "treeView";
			this.treeView.Size = new System.Drawing.Size(288, 363);
			this.treeView.TabIndex = 0;
			this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
			// 
			// radioNormal
			// 
			this.radioNormal.AutoSize = true;
			this.radioNormal.BackColor = System.Drawing.Color.Transparent;
			this.radioNormal.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.radioNormal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
			this.radioNormal.Location = new System.Drawing.Point(190, 0);
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
			this.radioAbnormal.Location = new System.Drawing.Point(190, 18);
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
			this.btnOK.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.btnOK.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
			this.btnOK.Location = new System.Drawing.Point(27, 456);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(120, 31);
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
			this.btnCancel.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
			this.btnCancel.Location = new System.Drawing.Point(162, 456);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(120, 31);
			this.btnCancel.TabIndex = 23;
			this.btnCancel.Text = "취소";
			this.btnCancel.UseVisualStyleBackColor = false;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
			this.label3.Font = new System.Drawing.Font("맑은 고딕", 14F, System.Drawing.FontStyle.Bold);
			this.label3.ForeColor = System.Drawing.Color.White;
			this.label3.Location = new System.Drawing.Point(3, 4);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(159, 25);
			this.label3.TabIndex = 24;
			this.label3.Text = "다른 SOP로 전환";
			// 
			// rdLabel2
			// 
			this.rdLabel2.AutoSize = true;
			this.rdLabel2.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.rdLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
			this.rdLabel2.Location = new System.Drawing.Point(164, 49);
			this.rdLabel2.Name = "rdLabel2";
			this.rdLabel2.Size = new System.Drawing.Size(129, 20);
			this.rdLabel2.TabIndex = 28;
			this.rdLabel2.Text = "야간 및 휴일 모드";
			this.rdLabel2.Click += new System.EventHandler(this.rdLabel2_Click);
			// 
			// rdLabel1
			// 
			this.rdLabel1.AutoSize = true;
			this.rdLabel1.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.rdLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
			this.rdLabel1.Location = new System.Drawing.Point(33, 49);
			this.rdLabel1.Name = "rdLabel1";
			this.rdLabel1.Size = new System.Drawing.Size(74, 20);
			this.rdLabel1.TabIndex = 27;
			this.rdLabel1.Text = "평일 모드";
			this.rdLabel1.Click += new System.EventHandler(this.rdLabel1_Click);
			// 
			// rdPictureBox2
			// 
			this.rdPictureBox2.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_black;
			this.rdPictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.rdPictureBox2.Location = new System.Drawing.Point(145, 51);
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
			this.rdPictureBox1.Location = new System.Drawing.Point(13, 51);
			this.rdPictureBox1.Name = "rdPictureBox1";
			this.rdPictureBox1.Size = new System.Drawing.Size(18, 17);
			this.rdPictureBox1.TabIndex = 26;
			this.rdPictureBox1.TabStop = false;
			this.rdPictureBox1.Click += new System.EventHandler(this.rdPictureBox1_Click);
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
			this.panel1.Controls.Add(this.label3);
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(311, 35);
			this.panel1.TabIndex = 29;
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
			this.panel2.Controls.Add(this.panel3);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Padding = new System.Windows.Forms.Padding(3);
			this.panel2.Size = new System.Drawing.Size(312, 507);
			this.panel2.TabIndex = 30;
			// 
			// panel3
			// 
			this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(3, 3);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(306, 501);
			this.panel3.TabIndex = 0;
			// 
			// PopupTransSOP
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
			this.ClientSize = new System.Drawing.Size(312, 507);
			this.Controls.Add(this.rdLabel2);
			this.Controls.Add(this.rdLabel1);
			this.Controls.Add(this.rdPictureBox2);
			this.Controls.Add(this.rdPictureBox1);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.radioAbnormal);
			this.Controls.Add(this.radioNormal);
			this.Controls.Add(this.treeView);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panel2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PopupTransSOP";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "다른 SOP로 전환";
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PopupTransSOP_MouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PopupTransSOP_MouseMove);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PopupTransSOP_MouseUp);
			((System.ComponentModel.ISupportInitialize)(this.rdPictureBox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rdPictureBox1)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.RadioButton radioNormal;
        private System.Windows.Forms.RadioButton radioAbnormal;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label rdLabel2;
        private System.Windows.Forms.Label rdLabel1;
        private System.Windows.Forms.PictureBox rdPictureBox2;
        private System.Windows.Forms.PictureBox rdPictureBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
    }
}