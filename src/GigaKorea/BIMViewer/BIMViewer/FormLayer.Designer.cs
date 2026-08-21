namespace BIMViewer
{
    partial class FormLayer
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.checkBoxWall = new System.Windows.Forms.CheckBox();
            this.btnColorWall = new System.Windows.Forms.Button();
            this.checkBoxWallCenterLine = new System.Windows.Forms.CheckBox();
            this.btnColorWallCenterLine = new System.Windows.Forms.Button();
            this.checkBoxSpace = new System.Windows.Forms.CheckBox();
            this.btnColorSpace = new System.Windows.Forms.Button();
            this.checkBoxPOI = new System.Windows.Forms.CheckBox();
            this.btnColorPOI = new System.Windows.Forms.Button();
            this.checkBoxAll = new System.Windows.Forms.CheckBox();
            this.checkBoxDoor = new System.Windows.Forms.CheckBox();
            this.btnColorDoor = new System.Windows.Forms.Button();
            this.checkBoxDoorBoundary = new System.Windows.Forms.CheckBox();
            this.btnColorDoorBoundary = new System.Windows.Forms.Button();
            this.checkBoxWindow = new System.Windows.Forms.CheckBox();
            this.btnColorWindow = new System.Windows.Forms.Button();
            this.checkBoxWindowBoundary = new System.Windows.Forms.CheckBox();
            this.btnColorWindowBoundary = new System.Windows.Forms.Button();
            this.checkBoxWallBoundary = new System.Windows.Forms.CheckBox();
            this.btnColorWallBoundary = new System.Windows.Forms.Button();
            this.btnAddDXF = new System.Windows.Forms.Button();
            this.gridBackgroundDXF = new System.Windows.Forms.DataGridView();
            this.colVisible = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkBoxColumn = new System.Windows.Forms.CheckBox();
            this.btnColColumn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridBackgroundDXF)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBoxWall
            // 
            this.checkBoxWall.AutoSize = true;
            this.checkBoxWall.Location = new System.Drawing.Point(12, 43);
            this.checkBoxWall.Name = "checkBoxWall";
            this.checkBoxWall.Size = new System.Drawing.Size(48, 16);
            this.checkBoxWall.TabIndex = 0;
            this.checkBoxWall.Text = "벽체";
            this.checkBoxWall.UseVisualStyleBackColor = true;
            this.checkBoxWall.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // btnColorWall
            // 
            this.btnColorWall.BackColor = System.Drawing.Color.Silver;
            this.btnColorWall.Location = new System.Drawing.Point(120, 39);
            this.btnColorWall.Name = "btnColorWall";
            this.btnColorWall.Size = new System.Drawing.Size(21, 20);
            this.btnColorWall.TabIndex = 1;
            this.btnColorWall.UseVisualStyleBackColor = false;
            this.btnColorWall.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // checkBoxWallCenterLine
            // 
            this.checkBoxWallCenterLine.AutoSize = true;
            this.checkBoxWallCenterLine.Location = new System.Drawing.Point(12, 88);
            this.checkBoxWallCenterLine.Name = "checkBoxWallCenterLine";
            this.checkBoxWallCenterLine.Size = new System.Drawing.Size(88, 16);
            this.checkBoxWallCenterLine.TabIndex = 0;
            this.checkBoxWallCenterLine.Text = "벽체 중심선";
            this.checkBoxWallCenterLine.UseVisualStyleBackColor = true;
            this.checkBoxWallCenterLine.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // btnColorWallCenterLine
            // 
            this.btnColorWallCenterLine.BackColor = System.Drawing.Color.Yellow;
            this.btnColorWallCenterLine.Location = new System.Drawing.Point(120, 84);
            this.btnColorWallCenterLine.Name = "btnColorWallCenterLine";
            this.btnColorWallCenterLine.Size = new System.Drawing.Size(21, 20);
            this.btnColorWallCenterLine.TabIndex = 1;
            this.btnColorWallCenterLine.UseVisualStyleBackColor = false;
            this.btnColorWallCenterLine.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // checkBoxSpace
            // 
            this.checkBoxSpace.AutoSize = true;
            this.checkBoxSpace.Location = new System.Drawing.Point(12, 110);
            this.checkBoxSpace.Name = "checkBoxSpace";
            this.checkBoxSpace.Size = new System.Drawing.Size(48, 16);
            this.checkBoxSpace.TabIndex = 0;
            this.checkBoxSpace.Text = "공간";
            this.checkBoxSpace.UseVisualStyleBackColor = true;
            this.checkBoxSpace.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // btnColorSpace
            // 
            this.btnColorSpace.BackColor = System.Drawing.Color.Green;
            this.btnColorSpace.Location = new System.Drawing.Point(120, 106);
            this.btnColorSpace.Name = "btnColorSpace";
            this.btnColorSpace.Size = new System.Drawing.Size(21, 20);
            this.btnColorSpace.TabIndex = 1;
            this.btnColorSpace.UseVisualStyleBackColor = false;
            this.btnColorSpace.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // checkBoxPOI
            // 
            this.checkBoxPOI.AutoSize = true;
            this.checkBoxPOI.Location = new System.Drawing.Point(12, 242);
            this.checkBoxPOI.Name = "checkBoxPOI";
            this.checkBoxPOI.Size = new System.Drawing.Size(44, 16);
            this.checkBoxPOI.TabIndex = 0;
            this.checkBoxPOI.Text = "POI";
            this.checkBoxPOI.UseVisualStyleBackColor = true;
            this.checkBoxPOI.Visible = false;
            this.checkBoxPOI.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // btnColorPOI
            // 
            this.btnColorPOI.BackColor = System.Drawing.Color.Red;
            this.btnColorPOI.Location = new System.Drawing.Point(120, 238);
            this.btnColorPOI.Name = "btnColorPOI";
            this.btnColorPOI.Size = new System.Drawing.Size(21, 20);
            this.btnColorPOI.TabIndex = 1;
            this.btnColorPOI.UseVisualStyleBackColor = false;
            this.btnColorPOI.Visible = false;
            this.btnColorPOI.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // checkBoxAll
            // 
            this.checkBoxAll.AutoSize = true;
            this.checkBoxAll.Location = new System.Drawing.Point(12, 21);
            this.checkBoxAll.Name = "checkBoxAll";
            this.checkBoxAll.Size = new System.Drawing.Size(48, 16);
            this.checkBoxAll.TabIndex = 0;
            this.checkBoxAll.Text = "전체";
            this.checkBoxAll.UseVisualStyleBackColor = true;
            this.checkBoxAll.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBoxDoor
            // 
            this.checkBoxDoor.AutoSize = true;
            this.checkBoxDoor.Location = new System.Drawing.Point(12, 132);
            this.checkBoxDoor.Name = "checkBoxDoor";
            this.checkBoxDoor.Size = new System.Drawing.Size(36, 16);
            this.checkBoxDoor.TabIndex = 0;
            this.checkBoxDoor.Text = "문";
            this.checkBoxDoor.UseVisualStyleBackColor = true;
            this.checkBoxDoor.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // btnColorDoor
            // 
            this.btnColorDoor.BackColor = System.Drawing.Color.Green;
            this.btnColorDoor.Location = new System.Drawing.Point(120, 128);
            this.btnColorDoor.Name = "btnColorDoor";
            this.btnColorDoor.Size = new System.Drawing.Size(21, 20);
            this.btnColorDoor.TabIndex = 1;
            this.btnColorDoor.UseVisualStyleBackColor = false;
            this.btnColorDoor.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // checkBoxDoorBoundary
            // 
            this.checkBoxDoorBoundary.AutoSize = true;
            this.checkBoxDoorBoundary.Location = new System.Drawing.Point(12, 154);
            this.checkBoxDoorBoundary.Name = "checkBoxDoorBoundary";
            this.checkBoxDoorBoundary.Size = new System.Drawing.Size(76, 16);
            this.checkBoxDoorBoundary.TabIndex = 0;
            this.checkBoxDoorBoundary.Text = "문 외곽선";
            this.checkBoxDoorBoundary.UseVisualStyleBackColor = true;
            this.checkBoxDoorBoundary.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // btnColorDoorBoundary
            // 
            this.btnColorDoorBoundary.BackColor = System.Drawing.Color.Green;
            this.btnColorDoorBoundary.Location = new System.Drawing.Point(120, 150);
            this.btnColorDoorBoundary.Name = "btnColorDoorBoundary";
            this.btnColorDoorBoundary.Size = new System.Drawing.Size(21, 20);
            this.btnColorDoorBoundary.TabIndex = 1;
            this.btnColorDoorBoundary.UseVisualStyleBackColor = false;
            this.btnColorDoorBoundary.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // checkBoxWindow
            // 
            this.checkBoxWindow.AutoSize = true;
            this.checkBoxWindow.Location = new System.Drawing.Point(12, 176);
            this.checkBoxWindow.Name = "checkBoxWindow";
            this.checkBoxWindow.Size = new System.Drawing.Size(48, 16);
            this.checkBoxWindow.TabIndex = 0;
            this.checkBoxWindow.Text = "창문";
            this.checkBoxWindow.UseVisualStyleBackColor = true;
            this.checkBoxWindow.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // btnColorWindow
            // 
            this.btnColorWindow.BackColor = System.Drawing.Color.Green;
            this.btnColorWindow.Location = new System.Drawing.Point(120, 172);
            this.btnColorWindow.Name = "btnColorWindow";
            this.btnColorWindow.Size = new System.Drawing.Size(21, 20);
            this.btnColorWindow.TabIndex = 1;
            this.btnColorWindow.UseVisualStyleBackColor = false;
            this.btnColorWindow.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // checkBoxWindowBoundary
            // 
            this.checkBoxWindowBoundary.AutoSize = true;
            this.checkBoxWindowBoundary.Location = new System.Drawing.Point(12, 198);
            this.checkBoxWindowBoundary.Name = "checkBoxWindowBoundary";
            this.checkBoxWindowBoundary.Size = new System.Drawing.Size(88, 16);
            this.checkBoxWindowBoundary.TabIndex = 0;
            this.checkBoxWindowBoundary.Text = "창문 외곽선";
            this.checkBoxWindowBoundary.UseVisualStyleBackColor = true;
            this.checkBoxWindowBoundary.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // btnColorWindowBoundary
            // 
            this.btnColorWindowBoundary.BackColor = System.Drawing.Color.Green;
            this.btnColorWindowBoundary.Location = new System.Drawing.Point(120, 194);
            this.btnColorWindowBoundary.Name = "btnColorWindowBoundary";
            this.btnColorWindowBoundary.Size = new System.Drawing.Size(21, 20);
            this.btnColorWindowBoundary.TabIndex = 1;
            this.btnColorWindowBoundary.UseVisualStyleBackColor = false;
            this.btnColorWindowBoundary.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // checkBoxWallBoundary
            // 
            this.checkBoxWallBoundary.AutoSize = true;
            this.checkBoxWallBoundary.Location = new System.Drawing.Point(12, 65);
            this.checkBoxWallBoundary.Name = "checkBoxWallBoundary";
            this.checkBoxWallBoundary.Size = new System.Drawing.Size(88, 16);
            this.checkBoxWallBoundary.TabIndex = 0;
            this.checkBoxWallBoundary.Text = "벽체 외곽선";
            this.checkBoxWallBoundary.UseVisualStyleBackColor = true;
            this.checkBoxWallBoundary.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // btnColorWallBoundary
            // 
            this.btnColorWallBoundary.BackColor = System.Drawing.Color.Silver;
            this.btnColorWallBoundary.Location = new System.Drawing.Point(120, 61);
            this.btnColorWallBoundary.Name = "btnColorWallBoundary";
            this.btnColorWallBoundary.Size = new System.Drawing.Size(21, 20);
            this.btnColorWallBoundary.TabIndex = 1;
            this.btnColorWallBoundary.UseVisualStyleBackColor = false;
            this.btnColorWallBoundary.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnAddDXF
            // 
            this.btnAddDXF.Location = new System.Drawing.Point(14, 261);
            this.btnAddDXF.Name = "btnAddDXF";
            this.btnAddDXF.Size = new System.Drawing.Size(96, 23);
            this.btnAddDXF.TabIndex = 3;
            this.btnAddDXF.Text = "배경도면 추가";
            this.btnAddDXF.UseVisualStyleBackColor = true;
            this.btnAddDXF.Click += new System.EventHandler(this.btnAddDXF_Click);
            // 
            // gridBackgroundDXF
            // 
            this.gridBackgroundDXF.AllowUserToAddRows = false;
            this.gridBackgroundDXF.AllowUserToDeleteRows = false;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridBackgroundDXF.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.gridBackgroundDXF.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridBackgroundDXF.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colVisible,
            this.colName});
            this.gridBackgroundDXF.Location = new System.Drawing.Point(14, 290);
            this.gridBackgroundDXF.Name = "gridBackgroundDXF";
            this.gridBackgroundDXF.RowHeadersVisible = false;
            this.gridBackgroundDXF.RowTemplate.Height = 23;
            this.gridBackgroundDXF.Size = new System.Drawing.Size(200, 125);
            this.gridBackgroundDXF.TabIndex = 4;
            this.gridBackgroundDXF.Visible = false;
            this.gridBackgroundDXF.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridBackgroundDXF_CellContentClick);
            this.gridBackgroundDXF.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridBackgroundDXF_CellValueChanged);
            // 
            // colVisible
            // 
            this.colVisible.HeaderText = "";
            this.colVisible.Name = "colVisible";
            this.colVisible.Width = 40;
            // 
            // colName
            // 
            this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colName.HeaderText = "이름";
            this.colName.Name = "colName";
            this.colName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // checkBoxColumn
            // 
            this.checkBoxColumn.AutoSize = true;
            this.checkBoxColumn.Location = new System.Drawing.Point(12, 220);
            this.checkBoxColumn.Name = "checkBoxColumn";
            this.checkBoxColumn.Size = new System.Drawing.Size(48, 16);
            this.checkBoxColumn.TabIndex = 0;
            this.checkBoxColumn.Text = "기둥";
            this.checkBoxColumn.UseVisualStyleBackColor = true;
            this.checkBoxColumn.Visible = false;
            this.checkBoxColumn.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // btnColColumn
            // 
            this.btnColColumn.BackColor = System.Drawing.Color.Red;
            this.btnColColumn.Location = new System.Drawing.Point(120, 216);
            this.btnColColumn.Name = "btnColColumn";
            this.btnColColumn.Size = new System.Drawing.Size(21, 20);
            this.btnColColumn.TabIndex = 1;
            this.btnColColumn.UseVisualStyleBackColor = false;
            this.btnColColumn.Visible = false;
            this.btnColColumn.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // FormLayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(229, 427);
            this.Controls.Add(this.gridBackgroundDXF);
            this.Controls.Add(this.btnAddDXF);
            this.Controls.Add(this.btnColColumn);
            this.Controls.Add(this.btnColorPOI);
            this.Controls.Add(this.btnColorWindowBoundary);
            this.Controls.Add(this.btnColorWindow);
            this.Controls.Add(this.btnColorDoorBoundary);
            this.Controls.Add(this.btnColorDoor);
            this.Controls.Add(this.btnColorSpace);
            this.Controls.Add(this.btnColorWallCenterLine);
            this.Controls.Add(this.btnColorWallBoundary);
            this.Controls.Add(this.btnColorWall);
            this.Controls.Add(this.checkBoxColumn);
            this.Controls.Add(this.checkBoxPOI);
            this.Controls.Add(this.checkBoxWindowBoundary);
            this.Controls.Add(this.checkBoxWindow);
            this.Controls.Add(this.checkBoxDoorBoundary);
            this.Controls.Add(this.checkBoxDoor);
            this.Controls.Add(this.checkBoxSpace);
            this.Controls.Add(this.checkBoxWallCenterLine);
            this.Controls.Add(this.checkBoxAll);
            this.Controls.Add(this.checkBoxWallBoundary);
            this.Controls.Add(this.checkBoxWall);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormLayer";
            this.Text = "Layer";
            ((System.ComponentModel.ISupportInitialize)(this.gridBackgroundDXF)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxWall;
        private System.Windows.Forms.Button btnColorWall;
        private System.Windows.Forms.CheckBox checkBoxWallCenterLine;
        private System.Windows.Forms.Button btnColorWallCenterLine;
        private System.Windows.Forms.CheckBox checkBoxSpace;
        private System.Windows.Forms.Button btnColorSpace;
        private System.Windows.Forms.CheckBox checkBoxPOI;
        private System.Windows.Forms.Button btnColorPOI;
        private System.Windows.Forms.CheckBox checkBoxAll;
        private System.Windows.Forms.CheckBox checkBoxDoor;
        private System.Windows.Forms.Button btnColorDoor;
        private System.Windows.Forms.CheckBox checkBoxDoorBoundary;
        private System.Windows.Forms.Button btnColorDoorBoundary;
        private System.Windows.Forms.CheckBox checkBoxWindow;
        private System.Windows.Forms.Button btnColorWindow;
        private System.Windows.Forms.CheckBox checkBoxWindowBoundary;
        private System.Windows.Forms.Button btnColorWindowBoundary;
        private System.Windows.Forms.CheckBox checkBoxWallBoundary;
        private System.Windows.Forms.Button btnColorWallBoundary;
        private System.Windows.Forms.Button btnAddDXF;
        private System.Windows.Forms.DataGridView gridBackgroundDXF;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colVisible;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.CheckBox checkBoxColumn;
        private System.Windows.Forms.Button btnColColumn;
    }
}