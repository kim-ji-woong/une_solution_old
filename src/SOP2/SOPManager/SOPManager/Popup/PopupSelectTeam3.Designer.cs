namespace SOPManager
{
    partial class PopupSelectTeam3
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.labelTeamType = new System.Windows.Forms.Label();
			this.treeViewTeam = new System.Windows.Forms.TreeView();
			this.btnChangeTeam = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.groupBox1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.labelTeamType);
			this.groupBox1.Controls.Add(this.treeViewTeam);
			this.groupBox1.Location = new System.Drawing.Point(9, 50);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(345, 365);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			// 
			// labelTeamType
			// 
			this.labelTeamType.AutoSize = true;
			this.labelTeamType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
			this.labelTeamType.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.labelTeamType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
			this.labelTeamType.Location = new System.Drawing.Point(10, -4);
			this.labelTeamType.Name = "labelTeamType";
			this.labelTeamType.Size = new System.Drawing.Size(59, 20);
			this.labelTeamType.TabIndex = 6;
			this.labelTeamType.Text = "전체 팀";
			// 
			// treeViewTeam
			// 
			this.treeViewTeam.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.treeViewTeam.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
			this.treeViewTeam.Location = new System.Drawing.Point(13, 19);
			this.treeViewTeam.Name = "treeViewTeam";
			this.treeViewTeam.Size = new System.Drawing.Size(317, 331);
			this.treeViewTeam.TabIndex = 4;
			// 
			// btnChangeTeam
			// 
			this.btnChangeTeam.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
			this.btnChangeTeam.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
			this.btnChangeTeam.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
			this.btnChangeTeam.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnChangeTeam.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.btnChangeTeam.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnChangeTeam.Location = new System.Drawing.Point(20, 431);
			this.btnChangeTeam.Name = "btnChangeTeam";
			this.btnChangeTeam.Size = new System.Drawing.Size(102, 31);
			this.btnChangeTeam.TabIndex = 22;
			this.btnChangeTeam.Text = "조직변경";
			this.btnChangeTeam.UseVisualStyleBackColor = false;
			this.btnChangeTeam.Click += new System.EventHandler(this.btnChangeTeam_Click);
			// 
			// btnOK
			// 
			this.btnOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(198)))), ((int)(((byte)(198)))));
			this.btnOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
			this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnOK.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.btnOK.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnOK.Location = new System.Drawing.Point(169, 431);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(88, 31);
			this.btnOK.TabIndex = 22;
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
			this.btnCancel.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnCancel.Location = new System.Drawing.Point(263, 431);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(83, 31);
			this.btnCancel.TabIndex = 22;
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
			this.label3.Location = new System.Drawing.Point(7, 4);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(95, 25);
			this.label3.TabIndex = 23;
			this.label3.Text = "조직 선택";
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(370, 35);
			this.panel1.TabIndex = 24;
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(68)))), ((int)(((byte)(82)))));
			this.panel2.Controls.Add(this.panel3);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Padding = new System.Windows.Forms.Padding(3);
			this.panel2.Size = new System.Drawing.Size(370, 486);
			this.panel2.TabIndex = 25;
			// 
			// panel3
			// 
			this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(3, 3);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(364, 480);
			this.panel3.TabIndex = 0;
			// 
			// PopupSelectTeam3
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
			this.ClientSize = new System.Drawing.Size(370, 486);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnChangeTeam);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panel2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "PopupSelectTeam3";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "조직 선택";
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PopupSelectTeam3_MouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PopupSelectTeam3_MouseMove);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PopupSelectTeam3_MouseUp);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelTeamType;
        private System.Windows.Forms.TreeView treeViewTeam;
        private System.Windows.Forms.Button btnChangeTeam;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;


    }
}