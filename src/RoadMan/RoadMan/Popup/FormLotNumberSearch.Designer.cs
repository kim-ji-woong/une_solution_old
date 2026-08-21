namespace RoadMan
{
	partial class FormLotNumberSearch
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLotNumberSearch));
			this.gridSelected = new System.Windows.Forms.DataGridView();
			this.colSelLotNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.button3 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
			this.button6 = new System.Windows.Forms.Button();
			this.button7 = new System.Windows.Forms.Button();
			this.gridAll = new System.Windows.Forms.DataGridView();
			this.colAllLotNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.gridSelected)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridAll)).BeginInit();
			this.SuspendLayout();
			// 
			// gridSelected
			// 
			this.gridSelected.AllowUserToAddRows = false;
			this.gridSelected.AllowUserToDeleteRows = false;
			this.gridSelected.AllowUserToResizeColumns = false;
			this.gridSelected.AllowUserToResizeRows = false;
			this.gridSelected.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.gridSelected.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.gridSelected.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSelLotNum});
			this.gridSelected.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.gridSelected.Location = new System.Drawing.Point(368, 67);
			this.gridSelected.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.gridSelected.MultiSelect = false;
			this.gridSelected.Name = "gridSelected";
			this.gridSelected.ReadOnly = true;
			this.gridSelected.RowHeadersVisible = false;
			this.gridSelected.RowTemplate.Height = 23;
			this.gridSelected.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.gridSelected.Size = new System.Drawing.Size(240, 306);
			this.gridSelected.TabIndex = 1;
			this.gridSelected.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView2_CellContentClick);
			// 
			// colSelLotNum
			// 
			this.colSelLotNum.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colSelLotNum.HeaderText = "지번주소";
			this.colSelLotNum.Name = "colSelLotNum";
			this.colSelLotNum.ReadOnly = true;
			// 
			// button1
			// 
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button1.Location = new System.Drawing.Point(284, 161);
			this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(56, 43);
			this.button1.TabIndex = 2;
			this.button1.Text = ">>";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.btnAdded_Click);
			// 
			// button2
			// 
			this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button2.Location = new System.Drawing.Point(284, 212);
			this.button2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(56, 40);
			this.button2.TabIndex = 3;
			this.button2.Text = "<<";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.btnRemoved_Click);
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(16, 18);
			this.textBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(240, 23);
			this.textBox1.TabIndex = 4;
			// 
			// button3
			// 
			this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button3.Location = new System.Drawing.Point(533, 30);
			this.button3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(75, 29);
			this.button3.TabIndex = 5;
			this.button3.Text = "모두 삭제";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.btnClearSelected_Click);
			// 
			// button4
			// 
			this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button4.Location = new System.Drawing.Point(517, 402);
			this.button4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(91, 29);
			this.button4.TabIndex = 6;
			this.button4.Text = "취소";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// button5
			// 
			this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button5.Location = new System.Drawing.Point(417, 402);
			this.button5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(94, 29);
			this.button5.TabIndex = 7;
			this.button5.Text = "확인";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.button5_Click);
			// 
			// button6
			// 
			this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button6.Location = new System.Drawing.Point(275, 12);
			this.button6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(75, 32);
			this.button6.TabIndex = 8;
			this.button6.Text = "찿기";
			this.button6.UseVisualStyleBackColor = true;
			this.button6.Click += new System.EventHandler(this.btnLotNumSearch);
			// 
			// button7
			// 
			this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button7.Location = new System.Drawing.Point(16, 402);
			this.button7.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.button7.Name = "button7";
			this.button7.Size = new System.Drawing.Size(75, 29);
			this.button7.TabIndex = 9;
			this.button7.Text = "모든 지번";
			this.button7.UseVisualStyleBackColor = true;
			this.button7.Click += new System.EventHandler(this.button7_Click);
			// 
			// gridAll
			// 
			this.gridAll.AllowUserToAddRows = false;
			this.gridAll.AllowUserToDeleteRows = false;
			this.gridAll.AllowUserToResizeColumns = false;
			this.gridAll.AllowUserToResizeRows = false;
			this.gridAll.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.gridAll.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
			this.gridAll.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colAllLotNum});
			this.gridAll.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.gridAll.Location = new System.Drawing.Point(16, 67);
			this.gridAll.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.gridAll.Name = "gridAll";
			this.gridAll.ReadOnly = true;
			this.gridAll.RowHeadersVisible = false;
			this.gridAll.RowTemplate.Height = 23;
			this.gridAll.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.gridAll.Size = new System.Drawing.Size(240, 306);
			this.gridAll.TabIndex = 10;
			// 
			// colAllLotNum
			// 
			this.colAllLotNum.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colAllLotNum.HeaderText = "지번주소";
			this.colAllLotNum.Name = "colAllLotNum";
			this.colAllLotNum.ReadOnly = true;
			// 
			// FormLotNumberSearch
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(630, 456);
			this.Controls.Add(this.gridAll);
			this.Controls.Add(this.button7);
			this.Controls.Add(this.button6);
			this.Controls.Add(this.button5);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.gridSelected);
			this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormLotNumberSearch";
			this.ShowInTaskbar = false;
			this.Text = "지번 검색";
			this.TopMost = true;
			((System.ComponentModel.ISupportInitialize)(this.gridSelected)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridAll)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView gridSelected;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.Button button6;
		private System.Windows.Forms.Button button7;
		private System.Windows.Forms.DataGridViewTextBoxColumn colSelLotNum;
		private System.Windows.Forms.DataGridView gridAll;
		private System.Windows.Forms.DataGridViewTextBoxColumn colAllLotNum;
	}
}