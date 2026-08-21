namespace SOPGen
{
    partial class FormDockingMission
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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textMember = new System.Windows.Forms.TextBox();
            this.textMessenger = new System.Windows.Forms.TextBox();
            this.textPhone3 = new System.Windows.Forms.TextBox();
            this.textPhone2 = new System.Windows.Forms.TextBox();
            this.textCellPhone3 = new System.Windows.Forms.TextBox();
            this.textPhone1 = new System.Windows.Forms.TextBox();
            this.textCellPhone2 = new System.Windows.Forms.TextBox();
            this.textCellPhone1 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dataGridViewMission = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dataGridViewCheck = new System.Windows.Forms.DataGridView();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMissionMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.CheckMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuAddMission = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuDeleteMission = new System.Windows.Forms.ToolStripMenuItem();
            this.contextCheckMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuAddCheck = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuDeleteCheck = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMission)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCheck)).BeginInit();
            this.contextMissionMenu.SuspendLayout();
            this.contextCheckMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textMember);
            this.groupBox1.Controls.Add(this.textMessenger);
            this.groupBox1.Controls.Add(this.textPhone3);
            this.groupBox1.Controls.Add(this.textPhone2);
            this.groupBox1.Controls.Add(this.textCellPhone3);
            this.groupBox1.Controls.Add(this.textPhone1);
            this.groupBox1.Controls.Add(this.textCellPhone2);
            this.groupBox1.Controls.Add(this.textCellPhone1);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(391, 148);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "부서/담당자";
            // 
            // textMember
            // 
            this.textMember.BackColor = System.Drawing.SystemColors.Window;
            this.textMember.Location = new System.Drawing.Point(91, 22);
            this.textMember.Name = "textMember";
            this.textMember.ReadOnly = true;
            this.textMember.Size = new System.Drawing.Size(289, 21);
            this.textMember.TabIndex = 1;
            // 
            // textMessenger
            // 
            this.textMessenger.Location = new System.Drawing.Point(91, 51);
            this.textMessenger.Name = "textMessenger";
            this.textMessenger.Size = new System.Drawing.Size(289, 21);
            this.textMessenger.TabIndex = 15;
            // 
            // textPhone3
            // 
            this.textPhone3.Location = new System.Drawing.Point(210, 111);
            this.textPhone3.MaxLength = 4;
            this.textPhone3.Name = "textPhone3";
            this.textPhone3.Size = new System.Drawing.Size(45, 21);
            this.textPhone3.TabIndex = 13;
            this.textPhone3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textPhone3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textCellPhone1_KeyPress);
            this.textPhone3.Leave += new System.EventHandler(this.textPhone3_Leave);
            // 
            // textPhone2
            // 
            this.textPhone2.Location = new System.Drawing.Point(148, 111);
            this.textPhone2.MaxLength = 4;
            this.textPhone2.Name = "textPhone2";
            this.textPhone2.Size = new System.Drawing.Size(45, 21);
            this.textPhone2.TabIndex = 11;
            this.textPhone2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textPhone2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textCellPhone1_KeyPress);
            this.textPhone2.Leave += new System.EventHandler(this.textPhone2_Leave);
            // 
            // textCellPhone3
            // 
            this.textCellPhone3.Location = new System.Drawing.Point(210, 81);
            this.textCellPhone3.MaxLength = 4;
            this.textCellPhone3.Name = "textCellPhone3";
            this.textCellPhone3.Size = new System.Drawing.Size(45, 21);
            this.textCellPhone3.TabIndex = 7;
            this.textCellPhone3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textCellPhone3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textCellPhone1_KeyPress);
            this.textCellPhone3.Leave += new System.EventHandler(this.textCellPhone3_Leave);
            // 
            // textPhone1
            // 
            this.textPhone1.Location = new System.Drawing.Point(91, 111);
            this.textPhone1.MaxLength = 3;
            this.textPhone1.Name = "textPhone1";
            this.textPhone1.Size = new System.Drawing.Size(40, 21);
            this.textPhone1.TabIndex = 9;
            this.textPhone1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textPhone1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textCellPhone1_KeyPress);
            this.textPhone1.Leave += new System.EventHandler(this.textPhone1_Leave);
            // 
            // textCellPhone2
            // 
            this.textCellPhone2.Location = new System.Drawing.Point(148, 81);
            this.textCellPhone2.MaxLength = 4;
            this.textCellPhone2.Name = "textCellPhone2";
            this.textCellPhone2.Size = new System.Drawing.Size(45, 21);
            this.textCellPhone2.TabIndex = 5;
            this.textCellPhone2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textCellPhone2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textCellPhone1_KeyPress);
            this.textCellPhone2.Leave += new System.EventHandler(this.textCellPhone2_Leave);
            // 
            // textCellPhone1
            // 
            this.textCellPhone1.Location = new System.Drawing.Point(91, 81);
            this.textCellPhone1.MaxLength = 3;
            this.textCellPhone1.Name = "textCellPhone1";
            this.textCellPhone1.Size = new System.Drawing.Size(40, 21);
            this.textCellPhone1.TabIndex = 3;
            this.textCellPhone1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textCellPhone1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textCellPhone1_KeyPress);
            this.textCellPhone1.Leave += new System.EventHandler(this.textCellPhone1_Leave);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(196, 115);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(11, 12);
            this.label8.TabIndex = 12;
            this.label8.Text = "-";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(134, 115);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(11, 12);
            this.label7.TabIndex = 10;
            this.label7.Text = "-";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(196, 85);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(11, 12);
            this.label6.TabIndex = 6;
            this.label6.Text = "-";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(134, 85);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(11, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "-";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 12);
            this.label4.TabIndex = 14;
            this.label4.Text = "메신저 ID :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(50, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "전화 :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "휴대폰 :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "부서/담당자 :";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dataGridViewMission);
            this.groupBox2.Location = new System.Drawing.Point(12, 166);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(391, 212);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "임무";
            // 
            // dataGridViewMission
            // 
            this.dataGridViewMission.AllowUserToAddRows = false;
            this.dataGridViewMission.AllowUserToResizeRows = false;
            this.dataGridViewMission.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewMission.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4});
            this.dataGridViewMission.Location = new System.Drawing.Point(10, 20);
            this.dataGridViewMission.MultiSelect = false;
            this.dataGridViewMission.Name = "dataGridViewMission";
            this.dataGridViewMission.RowHeadersVisible = false;
            this.dataGridViewMission.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridViewMission.RowTemplate.Height = 23;
            this.dataGridViewMission.Size = new System.Drawing.Size(370, 180);
            this.dataGridViewMission.TabIndex = 0;
            this.dataGridViewMission.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewMission_CellClick);
            this.dataGridViewMission.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewMission_CellEndEdit);
            this.dataGridViewMission.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridViewMission_KeyDown);
            this.dataGridViewMission.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataGridViewMission_MouseClick);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "구분";
            this.Column1.Name = "Column1";
            this.Column1.Width = 60;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.HeaderText = "내용";
            this.Column2.Name = "Column2";
            // 
            // Column3
            // 
            this.Column3.HeaderText = "보고대상";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "위치";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Width = 60;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.dataGridViewCheck);
            this.groupBox3.Location = new System.Drawing.Point(12, 384);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(391, 201);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "점검항목";
            // 
            // dataGridViewCheck
            // 
            this.dataGridViewCheck.AllowUserToAddRows = false;
            this.dataGridViewCheck.AllowUserToResizeRows = false;
            this.dataGridViewCheck.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCheck.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column5,
            this.Column6,
            this.Column7,
            this.Column8,
            this.Column9});
            this.dataGridViewCheck.Location = new System.Drawing.Point(10, 20);
            this.dataGridViewCheck.MultiSelect = false;
            this.dataGridViewCheck.Name = "dataGridViewCheck";
            this.dataGridViewCheck.RowHeadersVisible = false;
            this.dataGridViewCheck.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridViewCheck.RowTemplate.Height = 23;
            this.dataGridViewCheck.Size = new System.Drawing.Size(370, 170);
            this.dataGridViewCheck.TabIndex = 0;
            this.dataGridViewCheck.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridViewCheck_KeyDown);
            this.dataGridViewCheck.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataGridViewCheck_MouseClick);
            // 
            // Column5
            // 
            this.Column5.HeaderText = "대분류";
            this.Column5.Name = "Column5";
            this.Column5.Width = 65;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "중분류";
            this.Column6.Name = "Column6";
            this.Column6.Width = 65;
            // 
            // Column7
            // 
            this.Column7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column7.HeaderText = "내용";
            this.Column7.Name = "Column7";
            // 
            // Column8
            // 
            this.Column8.HeaderText = "개수";
            this.Column8.Name = "Column8";
            this.Column8.Width = 60;
            // 
            // Column9
            // 
            this.Column9.HeaderText = "위치";
            this.Column9.Name = "Column9";
            this.Column9.ReadOnly = true;
            this.Column9.Width = 60;
            // 
            // contextMissionMenu
            // 
            this.contextMissionMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CheckMenu,
            this.tsMenuAddMission,
            this.tsMenuDeleteMission});
            this.contextMissionMenu.Name = "contextCheckMenu";
            this.contextMissionMenu.Size = new System.Drawing.Size(151, 70);
            // 
            // CheckMenu
            // 
            this.CheckMenu.Name = "CheckMenu";
            this.CheckMenu.Size = new System.Drawing.Size(150, 22);
            this.CheckMenu.Text = "점검항목 추가";
            this.CheckMenu.Click += new System.EventHandler(this.CheckMenu_Click);
            // 
            // tsMenuAddMission
            // 
            this.tsMenuAddMission.Name = "tsMenuAddMission";
            this.tsMenuAddMission.Size = new System.Drawing.Size(150, 22);
            this.tsMenuAddMission.Text = "임무 추가";
            this.tsMenuAddMission.Click += new System.EventHandler(this.tsMenuAddMission_Click);
            // 
            // tsMenuDeleteMission
            // 
            this.tsMenuDeleteMission.Name = "tsMenuDeleteMission";
            this.tsMenuDeleteMission.Size = new System.Drawing.Size(150, 22);
            this.tsMenuDeleteMission.Text = "임무 삭제";
            this.tsMenuDeleteMission.Click += new System.EventHandler(this.tsMenuDeleteMission_Click);
            // 
            // contextCheckMenu
            // 
            this.contextCheckMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuAddCheck,
            this.tsMenuDeleteCheck});
            this.contextCheckMenu.Name = "contextCheckMenu";
            this.contextCheckMenu.Size = new System.Drawing.Size(151, 48);
            // 
            // tsMenuAddCheck
            // 
            this.tsMenuAddCheck.Name = "tsMenuAddCheck";
            this.tsMenuAddCheck.Size = new System.Drawing.Size(152, 22);
            this.tsMenuAddCheck.Text = "점검항목 추가";
            this.tsMenuAddCheck.Click += new System.EventHandler(this.tsMenuAddCheck_Click);
            // 
            // tsMenuDeleteCheck
            // 
            this.tsMenuDeleteCheck.Name = "tsMenuDeleteCheck";
            this.tsMenuDeleteCheck.Size = new System.Drawing.Size(152, 22);
            this.tsMenuDeleteCheck.Text = "점검항목 삭제";
            this.tsMenuDeleteCheck.Click += new System.EventHandler(this.tsMenuDeleteCheck_Click);
            // 
            // FormDockingMission
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(415, 597);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormDockingMission";
            this.ShowInTaskbar = false;
            this.Text = "임무관리";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMission)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCheck)).EndInit();
            this.contextMissionMenu.ResumeLayout(false);
            this.contextCheckMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textMessenger;
        private System.Windows.Forms.TextBox textPhone3;
        private System.Windows.Forms.TextBox textPhone2;
        private System.Windows.Forms.TextBox textCellPhone3;
        private System.Windows.Forms.TextBox textPhone1;
        private System.Windows.Forms.TextBox textCellPhone2;
        private System.Windows.Forms.TextBox textCellPhone1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dataGridViewMission;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridView dataGridViewCheck;
        public System.Windows.Forms.TextBox textMember;
        private System.Windows.Forms.ContextMenuStrip contextMissionMenu;
        private System.Windows.Forms.ToolStripMenuItem CheckMenu;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.ToolStripMenuItem tsMenuAddMission;
        private System.Windows.Forms.ToolStripMenuItem tsMenuDeleteMission;
        private System.Windows.Forms.ContextMenuStrip contextCheckMenu;
        private System.Windows.Forms.ToolStripMenuItem tsMenuAddCheck;
        private System.Windows.Forms.ToolStripMenuItem tsMenuDeleteCheck;
    }
}