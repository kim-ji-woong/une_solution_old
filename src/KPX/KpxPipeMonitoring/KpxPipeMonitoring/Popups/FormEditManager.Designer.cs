namespace KpxPipeMonitoring.Popups
{
    partial class FormEditManager
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.labelTitle = new System.Windows.Forms.Label();
            this.treeViewTeam = new System.Windows.Forms.TreeView();
            this.gridMember = new System.Windows.Forms.DataGridView();
            this.colIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMember = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridManager = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTeam = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.pictureBoxTitle2 = new System.Windows.Forms.PictureBox();
            this.pictureBoxTitle1 = new System.Windows.Forms.PictureBox();
            this.btnClose = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panelTemp = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.gridMember)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.BackColor = System.Drawing.Color.Transparent;
            this.labelTitle.Font = new System.Drawing.Font("나눔고딕", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(231, 3);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(122, 24);
            this.labelTitle.TabIndex = 9;
            this.labelTitle.Text = "담당자 편집";
            this.labelTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTitle_MouseDown);
            this.labelTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTitle_MouseMove);
            this.labelTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTitle_MouseUp);
            // 
            // treeViewTeam
            // 
            this.treeViewTeam.Location = new System.Drawing.Point(12, 47);
            this.treeViewTeam.Name = "treeViewTeam";
            this.treeViewTeam.Size = new System.Drawing.Size(315, 261);
            this.treeViewTeam.TabIndex = 10;
            this.treeViewTeam.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewTeam_NodeMouseClick);
            // 
            // gridMember
            // 
            this.gridMember.AllowUserToAddRows = false;
            this.gridMember.AllowUserToDeleteRows = false;
            this.gridMember.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridMember.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colIndex,
            this.colMember,
            this.colLevel});
            this.gridMember.Location = new System.Drawing.Point(333, 47);
            this.gridMember.Name = "gridMember";
            this.gridMember.RowHeadersVisible = false;
            this.gridMember.RowTemplate.Height = 23;
            this.gridMember.Size = new System.Drawing.Size(211, 261);
            this.gridMember.TabIndex = 11;
            this.gridMember.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridMember_CellClick);
            // 
            // colIndex
            // 
            this.colIndex.HeaderText = "No";
            this.colIndex.Name = "colIndex";
            this.colIndex.ReadOnly = true;
            this.colIndex.Width = 30;
            // 
            // colMember
            // 
            this.colMember.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colMember.HeaderText = "이름";
            this.colMember.Name = "colMember";
            this.colMember.ReadOnly = true;
            // 
            // colLevel
            // 
            this.colLevel.HeaderText = "직위";
            this.colLevel.Name = "colLevel";
            this.colLevel.ReadOnly = true;
            this.colLevel.Width = 60;
            // 
            // gridManager
            // 
            this.gridManager.AllowUserToAddRows = false;
            this.gridManager.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(73)))), ((int)(((byte)(106)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridManager.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridManager.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridManager.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colName,
            this.colTeam});
            this.gridManager.Location = new System.Drawing.Point(12, 346);
            this.gridManager.Name = "gridManager";
            this.gridManager.ReadOnly = true;
            this.gridManager.RowHeadersVisible = false;
            this.gridManager.RowTemplate.Height = 23;
            this.gridManager.Size = new System.Drawing.Size(531, 211);
            this.gridManager.TabIndex = 14;
            // 
            // colNo
            // 
            this.colNo.HeaderText = "No";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.Width = 30;
            // 
            // colName
            // 
            this.colName.HeaderText = "이름";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.Width = 170;
            // 
            // colTeam
            // 
            this.colTeam.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colTeam.HeaderText = "비고";
            this.colTeam.Name = "colTeam";
            this.colTeam.ReadOnly = true;
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.Transparent;
            this.btnOK.Image = global::KpxPipeMonitoring.Properties.Resources.OptionButtonConfirm;
            this.btnOK.Location = new System.Drawing.Point(442, 563);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(48, 26);
            this.btnOK.TabIndex = 13;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.Image = global::KpxPipeMonitoring.Properties.Resources.OptionButtonCancel;
            this.btnCancel.Location = new System.Drawing.Point(496, 563);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(48, 26);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.Transparent;
            this.btnAdd.Image = global::KpxPipeMonitoring.Properties.Resources.Add;
            this.btnAdd.Location = new System.Drawing.Point(442, 314);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(48, 26);
            this.btnAdd.TabIndex = 13;
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.Transparent;
            this.btnDelete.Image = global::KpxPipeMonitoring.Properties.Resources.Delete;
            this.btnDelete.Location = new System.Drawing.Point(496, 314);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(48, 26);
            this.btnDelete.TabIndex = 12;
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // pictureBoxTitle2
            // 
            this.pictureBoxTitle2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxTitle2.Image = global::KpxPipeMonitoring.Properties.Resources.OptionTop;
            this.pictureBoxTitle2.Location = new System.Drawing.Point(89, 0);
            this.pictureBoxTitle2.Name = "pictureBoxTitle2";
            this.pictureBoxTitle2.Size = new System.Drawing.Size(467, 35);
            this.pictureBoxTitle2.TabIndex = 8;
            this.pictureBoxTitle2.TabStop = false;
            this.pictureBoxTitle2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTitle_MouseDown);
            this.pictureBoxTitle2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTitle_MouseMove);
            this.pictureBoxTitle2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTitle_MouseUp);
            // 
            // pictureBoxTitle1
            // 
            this.pictureBoxTitle1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxTitle1.Image = global::KpxPipeMonitoring.Properties.Resources.OptionTop;
            this.pictureBoxTitle1.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxTitle1.Name = "pictureBoxTitle1";
            this.pictureBoxTitle1.Size = new System.Drawing.Size(467, 35);
            this.pictureBoxTitle1.TabIndex = 8;
            this.pictureBoxTitle1.TabStop = false;
            this.pictureBoxTitle1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTitle_MouseDown);
            this.pictureBoxTitle1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTitle_MouseMove);
            this.pictureBoxTitle1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTitle_MouseUp);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.BackgroundImage = global::KpxPipeMonitoring.Properties.Resources.OptionClose_normal;
            this.btnClose.Location = new System.Drawing.Point(528, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(18, 18);
            this.btnClose.TabIndex = 25;
            this.btnClose.TabStop = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            this.btnClose.MouseEnter += new System.EventHandler(this.btnClose_MouseEnter);
            this.btnClose.MouseLeave += new System.EventHandler(this.btnClose_MouseLeave);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panelTemp
            // 
            this.panelTemp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(133)))), ((int)(((byte)(0)))));
            this.panelTemp.Location = new System.Drawing.Point(75, 0);
            this.panelTemp.Name = "panelTemp";
            this.panelTemp.Size = new System.Drawing.Size(35, 23);
            this.panelTemp.TabIndex = 26;
            this.panelTemp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTitle_MouseDown);
            this.panelTemp.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTitle_MouseMove);
            this.panelTemp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxTitle_MouseUp);
            // 
            // FormEditManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(556, 594);
            this.Controls.Add(this.panelTemp);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.gridManager);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.gridMember);
            this.Controls.Add(this.treeViewTeam);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.pictureBoxTitle2);
            this.Controls.Add(this.pictureBoxTitle1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormEditManager";
            this.Text = "FormEditManager";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(165)))), ((int)(((byte)(0)))));
            this.Load += new System.EventHandler(this.FormEditManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridMember)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxTitle1;
        private System.Windows.Forms.PictureBox pictureBoxTitle2;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.TreeView treeViewTeam;
        private System.Windows.Forms.DataGridView gridMember;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.DataGridView gridManager;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTeam;
        private System.Windows.Forms.PictureBox btnClose;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panelTemp;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMember;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLevel;
    }
}