namespace SDMS_Building.PopupDialog.Config
{
    partial class FormManagerEdit
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.eleType = new System.Windows.Forms.Integration.ElementHost();
            this.label2 = new System.Windows.Forms.Label();
            this.eleBuilding = new System.Windows.Forms.Integration.ElementHost();
            this.label3 = new System.Windows.Forms.Label();
            this.eleFloor = new System.Windows.Forms.Integration.ElementHost();
            this.gridMember = new System.Windows.Forms.DataGridView();
            this.treeViewTeam = new System.Windows.Forms.TreeView();
            this.gridManager = new System.Windows.Forms.DataGridView();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnOK = new UnE.GUI.ImageButton();
            this.btnCancel = new UnE.GUI.ImageButton();
            this.btnAdd = new UnE.GUI.ImageButton();
            this.btnRemove = new UnE.GUI.ImageButton();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridMember)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAdd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRemove)).BeginInit();
            this.SuspendLayout();
            // 
            // eleType
            // 
            this.eleType.BackColor = System.Drawing.Color.White;
            this.eleType.Location = new System.Drawing.Point(70, 15);
            this.eleType.Name = "eleType";
            this.eleType.Size = new System.Drawing.Size(144, 30);
            this.eleType.TabIndex = 22;
            this.eleType.Text = "elementHost1";
            this.eleType.Child = null;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(91)))), ((int)(((byte)(91)))));
            this.label2.Location = new System.Drawing.Point(12, 17);
            this.label2.MinimumSize = new System.Drawing.Size(36, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 28);
            this.label2.TabIndex = 21;
            this.label2.Text = "유형";
            // 
            // eleBuilding
            // 
            this.eleBuilding.BackColor = System.Drawing.Color.White;
            this.eleBuilding.Location = new System.Drawing.Point(70, 58);
            this.eleBuilding.Name = "eleBuilding";
            this.eleBuilding.Size = new System.Drawing.Size(184, 30);
            this.eleBuilding.TabIndex = 24;
            this.eleBuilding.Text = "elementHost1";
            this.eleBuilding.Child = null;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(91)))), ((int)(((byte)(91)))));
            this.label3.Location = new System.Drawing.Point(12, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 19);
            this.label3.TabIndex = 23;
            this.label3.Text = "범위";
            // 
            // eleFloor
            // 
            this.eleFloor.BackColor = System.Drawing.Color.White;
            this.eleFloor.Location = new System.Drawing.Point(284, 58);
            this.eleFloor.Name = "eleFloor";
            this.eleFloor.Size = new System.Drawing.Size(134, 30);
            this.eleFloor.TabIndex = 25;
            this.eleFloor.Text = "elementHost2";
            this.eleFloor.Child = null;
            // 
            // gridMember
            // 
            this.gridMember.AllowUserToAddRows = false;
            this.gridMember.BackgroundColor = System.Drawing.Color.White;
            this.gridMember.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gridMember.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("나눔바른고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridMember.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridMember.ColumnHeadersHeight = 40;
            this.gridMember.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colName});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridMember.DefaultCellStyle = dataGridViewCellStyle2;
            this.gridMember.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.gridMember.Location = new System.Drawing.Point(218, 94);
            this.gridMember.Name = "gridMember";
            this.gridMember.RowHeadersVisible = false;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("나눔바른고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.gridMember.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.gridMember.RowTemplate.Height = 50;
            this.gridMember.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridMember.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridMember.Size = new System.Drawing.Size(200, 240);
            this.gridMember.TabIndex = 26;
            this.gridMember.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridMember_CellClick);
            // 
            // treeViewTeam
            // 
            this.treeViewTeam.Location = new System.Drawing.Point(10, 94);
            this.treeViewTeam.Name = "treeViewTeam";
            this.treeViewTeam.Size = new System.Drawing.Size(200, 240);
            this.treeViewTeam.TabIndex = 27;
            this.treeViewTeam.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewTeam_NodeMouseClick);
            // 
            // gridManager
            // 
            this.gridManager.AllowUserToAddRows = false;
            this.gridManager.BackgroundColor = System.Drawing.Color.White;
            this.gridManager.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gridManager.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("나눔바른고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridManager.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.gridManager.ColumnHeadersHeight = 40;
            this.gridManager.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridManager.DefaultCellStyle = dataGridViewCellStyle6;
            this.gridManager.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.gridManager.Location = new System.Drawing.Point(10, 372);
            this.gridManager.Name = "gridManager";
            this.gridManager.RowHeadersVisible = false;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("나눔바른고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.gridManager.RowsDefaultCellStyle = dataGridViewCellStyle7;
            this.gridManager.RowTemplate.Height = 50;
            this.gridManager.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridManager.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridManager.Size = new System.Drawing.Size(408, 257);
            this.gridManager.TabIndex = 28;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.OnTimer);
            // 
            // btnOK
            // 
            this.btnOK.ButtonText = "";
            this.btnOK.ImageClicked = global::SDMS_Building.Properties.Resources.editOk_click;
            this.btnOK.ImageDisabled = null;
            this.btnOK.ImageMouseOver = global::SDMS_Building.Properties.Resources.editOk_hover;
            this.btnOK.ImageNormal = global::SDMS_Building.Properties.Resources.editOk_normal;
            this.btnOK.Location = new System.Drawing.Point(130, 640);
            this.btnOK.Name = "btnOK";
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(80, 26);
            this.btnOK.TabIndex = 32;
            this.btnOK.TabStop = false;
            this.btnOK.TextColor = System.Drawing.Color.White;
            this.btnOK.TextFont = new System.Drawing.Font("나눔바른고딕", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.ToolTipText = "";
            this.btnOK.UseToolTip = false;
            this.btnOK.WindowRateWidth = 1F;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.ButtonText = "";
            this.btnCancel.ImageClicked = global::SDMS_Building.Properties.Resources.editCancel_Click;
            this.btnCancel.ImageDisabled = null;
            this.btnCancel.ImageMouseOver = global::SDMS_Building.Properties.Resources.editCancel_Hover;
            this.btnCancel.ImageNormal = global::SDMS_Building.Properties.Resources.editCancel_Normal;
            this.btnCancel.Location = new System.Drawing.Point(218, 640);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(80, 26);
            this.btnCancel.TabIndex = 31;
            this.btnCancel.TabStop = false;
            this.btnCancel.TextColor = System.Drawing.Color.Black;
            this.btnCancel.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ToolTipText = "";
            this.btnCancel.UseToolTip = false;
            this.btnCancel.WindowRateWidth = 1F;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.ButtonText = "";
            this.btnAdd.ImageClicked = global::SDMS_Building.Properties.Resources.add_click;
            this.btnAdd.ImageDisabled = null;
            this.btnAdd.ImageMouseOver = global::SDMS_Building.Properties.Resources.add_hover;
            this.btnAdd.ImageNormal = global::SDMS_Building.Properties.Resources.add_normal;
            this.btnAdd.Location = new System.Drawing.Point(130, 340);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Owner = null;
            this.btnAdd.Size = new System.Drawing.Size(80, 26);
            this.btnAdd.TabIndex = 30;
            this.btnAdd.TabStop = false;
            this.btnAdd.TextColor = System.Drawing.Color.White;
            this.btnAdd.TextFont = new System.Drawing.Font("나눔바른고딕", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnAdd.ToolTipText = "";
            this.btnAdd.UseToolTip = false;
            this.btnAdd.WindowRateWidth = 1F;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.ButtonText = "";
            this.btnRemove.ImageClicked = global::SDMS_Building.Properties.Resources.delete_click;
            this.btnRemove.ImageDisabled = null;
            this.btnRemove.ImageMouseOver = global::SDMS_Building.Properties.Resources.delete_hover;
            this.btnRemove.ImageNormal = global::SDMS_Building.Properties.Resources.delete_normal;
            this.btnRemove.Location = new System.Drawing.Point(218, 340);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Owner = null;
            this.btnRemove.Size = new System.Drawing.Size(80, 26);
            this.btnRemove.TabIndex = 29;
            this.btnRemove.TabStop = false;
            this.btnRemove.TextColor = System.Drawing.Color.Black;
            this.btnRemove.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRemove.ToolTipText = "";
            this.btnRemove.UseToolTip = false;
            this.btnRemove.WindowRateWidth = 1F;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // colNo
            // 
            this.colNo.HeaderText = "No";
            this.colNo.Name = "colNo";
            this.colNo.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNo.Width = 40;
            // 
            // colName
            // 
            this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colName.HeaderText = "팀원";
            this.colName.Name = "colName";
            this.colName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewTextBoxColumn1.HeaderText = "No";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn1.Width = 40;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "이름";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn2.Width = 250;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn3.HeaderText = "비고";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // FormManagerEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 679);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.gridManager);
            this.Controls.Add(this.treeViewTeam);
            this.Controls.Add(this.gridMember);
            this.Controls.Add(this.eleFloor);
            this.Controls.Add(this.eleBuilding);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.eleType);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormManagerEdit";
            this.ShowInTaskbar = false;
            this.Text = "FormManagerEdit";
            this.Load += new System.EventHandler(this.FormManagerEdit_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridMember)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAdd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRemove)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Integration.ElementHost eleType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Integration.ElementHost eleBuilding;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Integration.ElementHost eleFloor;
        private System.Windows.Forms.DataGridView gridMember;
        private System.Windows.Forms.TreeView treeViewTeam;
        private System.Windows.Forms.DataGridView gridManager;
        private UnE.GUI.ImageButton btnAdd;
        private UnE.GUI.ImageButton btnRemove;
        private System.Windows.Forms.Timer timer1;
        private UnE.GUI.ImageButton btnOK;
        private UnE.GUI.ImageButton btnCancel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    }
}