namespace SoilMan.Popup
{
    partial class FormDetailAttrib
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.btnDeleteSel = new System.Windows.Forms.Button();
            this.btnReverse = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnMove = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.labelTotalPageCount = new System.Windows.Forms.Label();
            this.textBoxPageIndex = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lbSelectArea = new System.Windows.Forms.Label();
            this.lbSelectRow = new System.Windows.Forms.Label();
            this.dataGridView1 = new SoilMan.Popup.DataGridViewEx();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLandID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colArea = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAddr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.btnDeleteSel);
            this.panel1.Controls.Add(this.btnReverse);
            this.panel1.Controls.Add(this.btnSelectAll);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.MinimumSize = new System.Drawing.Size(525, 40);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(525, 40);
            this.panel1.TabIndex = 10;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(249, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(172, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "선택되지 않은 전체 삭제";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnDeleteSel
            // 
            this.btnDeleteSel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteSel.Location = new System.Drawing.Point(427, 9);
            this.btnDeleteSel.Name = "btnDeleteSel";
            this.btnDeleteSel.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteSel.TabIndex = 12;
            this.btnDeleteSel.Text = "선택삭제";
            this.btnDeleteSel.UseVisualStyleBackColor = true;
            this.btnDeleteSel.Click += new System.EventHandler(this.btnDeleteSel_Click);
            // 
            // btnReverse
            // 
            this.btnReverse.Location = new System.Drawing.Point(93, 9);
            this.btnReverse.Name = "btnReverse";
            this.btnReverse.Size = new System.Drawing.Size(75, 23);
            this.btnReverse.TabIndex = 11;
            this.btnReverse.Text = "선택반전";
            this.btnReverse.UseVisualStyleBackColor = true;
            this.btnReverse.Click += new System.EventHandler(this.btnReverse_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(12, 9);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAll.TabIndex = 10;
            this.btnSelectAll.Text = "전체선택";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnPrev);
            this.panel2.Controls.Add(this.btnMove);
            this.panel2.Controls.Add(this.btnNext);
            this.panel2.Controls.Add(this.labelTotalPageCount);
            this.panel2.Controls.Add(this.textBoxPageIndex);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 301);
            this.panel2.MinimumSize = new System.Drawing.Size(525, 58);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(525, 58);
            this.panel2.TabIndex = 11;
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(205, 9);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(38, 23);
            this.btnPrev.TabIndex = 9;
            this.btnPrev.Text = "이전";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnMove
            // 
            this.btnMove.Location = new System.Drawing.Point(358, 31);
            this.btnMove.Name = "btnMove";
            this.btnMove.Size = new System.Drawing.Size(38, 23);
            this.btnMove.TabIndex = 10;
            this.btnMove.Text = "이동";
            this.btnMove.UseVisualStyleBackColor = true;
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(270, 9);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(38, 23);
            this.btnNext.TabIndex = 11;
            this.btnNext.Text = "다음";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // labelTotalPageCount
            // 
            this.labelTotalPageCount.AutoSize = true;
            this.labelTotalPageCount.Location = new System.Drawing.Point(247, 38);
            this.labelTotalPageCount.Name = "labelTotalPageCount";
            this.labelTotalPageCount.Size = new System.Drawing.Size(105, 12);
            this.labelTotalPageCount.TabIndex = 8;
            this.labelTotalPageCount.Text = "/ TotalPageCount";
            // 
            // textBoxPageIndex
            // 
            this.textBoxPageIndex.Location = new System.Drawing.Point(206, 33);
            this.textBoxPageIndex.Name = "textBoxPageIndex";
            this.textBoxPageIndex.Size = new System.Drawing.Size(35, 21);
            this.textBoxPageIndex.TabIndex = 7;
            this.textBoxPageIndex.Text = "1";
            this.textBoxPageIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lbSelectArea);
            this.panel3.Controls.Add(this.lbSelectRow);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 359);
            this.panel3.MinimumSize = new System.Drawing.Size(525, 30);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(525, 30);
            this.panel3.TabIndex = 13;
            // 
            // lbSelectArea
            // 
            this.lbSelectArea.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSelectArea.AutoSize = true;
            this.lbSelectArea.Location = new System.Drawing.Point(268, 9);
            this.lbSelectArea.Name = "lbSelectArea";
            this.lbSelectArea.Size = new System.Drawing.Size(53, 12);
            this.lbSelectArea.TabIndex = 1;
            this.lbSelectArea.Text = "총면적 : ";
            this.lbSelectArea.Click += new System.EventHandler(this.lbSelectArea_Click);
            // 
            // lbSelectRow
            // 
            this.lbSelectRow.AutoSize = true;
            this.lbSelectRow.Location = new System.Drawing.Point(34, 9);
            this.lbSelectRow.Name = "lbSelectRow";
            this.lbSelectRow.Size = new System.Drawing.Size(53, 12);
            this.lbSelectRow.TabIndex = 0;
            this.lbSelectRow.Text = "선택열 : ";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(241)))), ((int)(((byte)(222)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colLandID,
            this.colArea,
            this.colAddr,
            this.colCost});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView1.Location = new System.Drawing.Point(0, 40);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(525, 319);
            this.dataGridView1.TabIndex = 3;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            this.dataGridView1.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dataGridView1_ColumnWidthChanged);
            this.dataGridView1.RowStateChanged += new System.Windows.Forms.DataGridViewRowStateChangedEventHandler(this.dataGridView1_RowStateChanged);
            this.dataGridView1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dataGridView1_Scroll);
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            this.dataGridView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyDown);
            this.dataGridView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseDown);
            // 
            // colNo
            // 
            this.colNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colNo.HeaderText = "PNU";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.Width = 55;
            // 
            // colLandID
            // 
            this.colLandID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colLandID.HeaderText = "지목";
            this.colLandID.Name = "colLandID";
            this.colLandID.ReadOnly = true;
            this.colLandID.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colLandID.Width = 54;
            // 
            // colArea
            // 
            this.colArea.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colArea.HeaderText = "면적(m²)";
            this.colArea.Name = "colArea";
            this.colArea.Width = 79;
            // 
            // colAddr
            // 
            this.colAddr.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colAddr.HeaderText = "주소";
            this.colAddr.Name = "colAddr";
            // 
            // colCost
            // 
            this.colCost.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colCost.HeaderText = "개별공시지가(원)";
            this.colCost.Name = "colCost";
            this.colCost.Width = 124;
            // 
            // FormDetailAttrib
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 389);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(525, 389);
            this.Name = "FormDetailAttrib";
            this.Text = "FormDetailAttrib";
            this.Load += new System.EventHandler(this.FormDetailAttrib_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnDeleteSel;
        private System.Windows.Forms.Button btnReverse;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btnMove;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Label labelTotalPageCount;
        private System.Windows.Forms.TextBox textBoxPageIndex;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lbSelectArea;
        private System.Windows.Forms.Label lbSelectRow;
        private DataGridViewEx dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLandID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colArea;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAddr;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCost;
    }
}