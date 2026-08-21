namespace HSMS
{
    partial class FormVehicle
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.gridManager = new System.Windows.Forms.DataGridView();
            this.gridMember = new System.Windows.Forms.DataGridView();
            this.colCarName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStandard = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWidth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHeight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCarNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColSensorID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.treeViewTeam = new System.Windows.Forms.TreeView();
            this.btnOK = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.colNumber2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCarName2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStandard2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLength2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWidth2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHeight2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCarMaker2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCarUse = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDriverName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColSensorID2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridMember)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 15);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1145, 59);
            this.panel1.TabIndex = 65;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(16, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "차량 생성";
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(1021, 624);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(94, 31);
            this.btnCancel.TabIndex = 64;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackgroundImage = global::HSMS.Properties.Resources.Arrow_Down;
            this.btnAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(770, 233);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(59, 54);
            this.btnAdd.TabIndex = 61;
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.BackgroundImage = global::HSMS.Properties.Resources.Arrow_Up;
            this.btnRemove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(679, 233);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(59, 54);
            this.btnRemove.TabIndex = 62;
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // gridManager
            // 
            this.gridManager.AllowUserToAddRows = false;
            this.gridManager.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.gridManager.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.gridManager.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(73)))), ((int)(((byte)(106)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridManager.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.gridManager.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridManager.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNumber2,
            this.colCarName2,
            this.colStandard2,
            this.colType2,
            this.colLength2,
            this.colWidth2,
            this.colHeight2,
            this.colCarMaker2,
            this.colCarUse,
            this.colDriverName,
            this.ColSensorID2});
            this.gridManager.Location = new System.Drawing.Point(25, 382);
            this.gridManager.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridManager.Name = "gridManager";
            this.gridManager.ReadOnly = true;
            this.gridManager.RowHeadersVisible = false;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.gridManager.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.gridManager.RowTemplate.Height = 23;
            this.gridManager.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridManager.Size = new System.Drawing.Size(1115, 199);
            this.gridManager.TabIndex = 60;
            // 
            // gridMember
            // 
            this.gridMember.AllowUserToAddRows = false;
            this.gridMember.AllowUserToDeleteRows = false;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.gridMember.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.gridMember.BackgroundColor = System.Drawing.Color.White;
            this.gridMember.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridMember.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCarName,
            this.colStandard,
            this.colType,
            this.colLength,
            this.colWidth,
            this.colHeight,
            this.colCarNumber,
            this.ColSensorID});
            this.gridMember.Location = new System.Drawing.Point(346, 94);
            this.gridMember.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridMember.MultiSelect = false;
            this.gridMember.Name = "gridMember";
            this.gridMember.RowHeadersVisible = false;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.gridMember.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.gridMember.RowTemplate.Height = 23;
            this.gridMember.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridMember.Size = new System.Drawing.Size(794, 210);
            this.gridMember.TabIndex = 59;
            // 
            // colCarName
            // 
            this.colCarName.FillWeight = 20F;
            this.colCarName.HeaderText = "장비";
            this.colCarName.Name = "colCarName";
            this.colCarName.ReadOnly = true;
            // 
            // colStandard
            // 
            this.colStandard.FillWeight = 20F;
            this.colStandard.HeaderText = "규격";
            this.colStandard.Name = "colStandard";
            this.colStandard.ReadOnly = true;
            // 
            // colType
            // 
            this.colType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colType.FillWeight = 20F;
            this.colType.HeaderText = "분류";
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            // 
            // colLength
            // 
            this.colLength.HeaderText = "길이(mm)";
            this.colLength.Name = "colLength";
            // 
            // colWidth
            // 
            this.colWidth.HeaderText = "너비(mm)";
            this.colWidth.Name = "colWidth";
            // 
            // colHeight
            // 
            this.colHeight.HeaderText = "높이(mm)";
            this.colHeight.Name = "colHeight";
            // 
            // colCarNumber
            // 
            this.colCarNumber.HeaderText = "차량번호";
            this.colCarNumber.Name = "colCarNumber";
            // 
            // ColSensorID
            // 
            this.ColSensorID.FillWeight = 40F;
            this.ColSensorID.HeaderText = "센서 ID";
            this.ColSensorID.Name = "ColSensorID";
            this.ColSensorID.Width = 70;
            // 
            // treeViewTeam
            // 
            this.treeViewTeam.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeViewTeam.Location = new System.Drawing.Point(25, 94);
            this.treeViewTeam.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.treeViewTeam.Name = "treeViewTeam";
            this.treeViewTeam.Size = new System.Drawing.Size(315, 210);
            this.treeViewTeam.TabIndex = 58;
            this.treeViewTeam.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewTeam_BeforeSelect);
            this.treeViewTeam.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewTeam_AfterSelect);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.White;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.Location = new System.Drawing.Point(902, 624);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(94, 31);
            this.btnOK.TabIndex = 68;
            this.btnOK.Text = "저장";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.btnRemove);
            this.panel2.Controls.Add(this.btnAdd);
            this.panel2.Location = new System.Drawing.Point(12, 82);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1145, 523);
            this.panel2.TabIndex = 69;
            // 
            // colNumber2
            // 
            this.colNumber2.HeaderText = "No";
            this.colNumber2.Name = "colNumber2";
            this.colNumber2.ReadOnly = true;
            this.colNumber2.Width = 55;
            // 
            // colCarName2
            // 
            this.colCarName2.FillWeight = 30F;
            this.colCarName2.HeaderText = "장비";
            this.colCarName2.Name = "colCarName2";
            this.colCarName2.ReadOnly = true;
            // 
            // colStandard2
            // 
            this.colStandard2.HeaderText = "규격";
            this.colStandard2.Name = "colStandard2";
            this.colStandard2.ReadOnly = true;
            // 
            // colType2
            // 
            this.colType2.HeaderText = "분류";
            this.colType2.Name = "colType2";
            this.colType2.ReadOnly = true;
            // 
            // colLength2
            // 
            this.colLength2.HeaderText = "길이(mm)";
            this.colLength2.Name = "colLength2";
            this.colLength2.ReadOnly = true;
            // 
            // colWidth2
            // 
            this.colWidth2.HeaderText = "너비(mm)";
            this.colWidth2.Name = "colWidth2";
            this.colWidth2.ReadOnly = true;
            // 
            // colHeight2
            // 
            this.colHeight2.HeaderText = "높이(mm)";
            this.colHeight2.Name = "colHeight2";
            this.colHeight2.ReadOnly = true;
            // 
            // colCarMaker2
            // 
            this.colCarMaker2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colCarMaker2.FillWeight = 30F;
            this.colCarMaker2.HeaderText = "제작회사";
            this.colCarMaker2.Name = "colCarMaker2";
            this.colCarMaker2.ReadOnly = true;
            // 
            // colCarUse
            // 
            this.colCarUse.HeaderText = "사용용도";
            this.colCarUse.Name = "colCarUse";
            this.colCarUse.ReadOnly = true;
            this.colCarUse.Width = 135;
            // 
            // colDriverName
            // 
            this.colDriverName.HeaderText = "운전자 이름";
            this.colDriverName.Name = "colDriverName";
            this.colDriverName.ReadOnly = true;
            this.colDriverName.Width = 110;
            // 
            // ColSensorID2
            // 
            this.ColSensorID2.FillWeight = 20F;
            this.ColSensorID2.HeaderText = "센서ID";
            this.ColSensorID2.Name = "ColSensorID2";
            this.ColSensorID2.ReadOnly = true;
            this.ColSensorID2.Width = 70;
            // 
            // FormVehicle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(1169, 667);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.gridManager);
            this.Controls.Add(this.gridMember);
            this.Controls.Add(this.treeViewTeam);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormVehicle";
            this.Text = "FormVehicle";
            this.Load += new System.EventHandler(this.FormVehicle_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridMember)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.DataGridView gridManager;
        private System.Windows.Forms.DataGridView gridMember;
        private System.Windows.Forms.TreeView treeViewTeam;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCarName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStandard;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLength;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWidth;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHeight;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCarNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColSensorID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNumber2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCarName2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStandard2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLength2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWidth2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHeight2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCarMaker2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCarUse;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDriverName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColSensorID2;
    }
}