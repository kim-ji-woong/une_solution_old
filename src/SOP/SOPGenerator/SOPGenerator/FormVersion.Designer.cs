namespace SOPGen
{
    partial class FormVersion
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
            this.versionDataGrid = new System.Windows.Forms.DataGridView();
            this.ColVersionName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColOwner = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColCreateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColLastAccessTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkBoxNewVersion = new System.Windows.Forms.CheckBox();
            this.textBoxNewVersion = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.versionDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // versionDataGrid
            // 
            this.versionDataGrid.AllowUserToAddRows = false;
            this.versionDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.versionDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColVersionName,
            this.ColOwner,
            this.ColCreateTime,
            this.ColLastAccessTime,
            this.ColDescription});
            this.versionDataGrid.Dock = System.Windows.Forms.DockStyle.Top;
            this.versionDataGrid.Location = new System.Drawing.Point(0, 0);
            this.versionDataGrid.MultiSelect = false;
            this.versionDataGrid.Name = "versionDataGrid";
            this.versionDataGrid.ReadOnly = true;
            this.versionDataGrid.RowHeadersVisible = false;
            this.versionDataGrid.RowTemplate.Height = 23;
            this.versionDataGrid.Size = new System.Drawing.Size(652, 189);
            this.versionDataGrid.TabIndex = 0;
            // 
            // ColVersionName
            // 
            this.ColVersionName.HeaderText = "버전명";
            this.ColVersionName.Name = "ColVersionName";
            this.ColVersionName.ReadOnly = true;
            // 
            // ColOwner
            // 
            this.ColOwner.HeaderText = "작성자";
            this.ColOwner.Name = "ColOwner";
            this.ColOwner.ReadOnly = true;
            // 
            // ColCreateTime
            // 
            this.ColCreateTime.HeaderText = "생성일자";
            this.ColCreateTime.Name = "ColCreateTime";
            this.ColCreateTime.ReadOnly = true;
            // 
            // ColLastAccessTime
            // 
            this.ColLastAccessTime.HeaderText = "수정일자";
            this.ColLastAccessTime.Name = "ColLastAccessTime";
            this.ColLastAccessTime.ReadOnly = true;
            // 
            // ColDescription
            // 
            this.ColDescription.HeaderText = "부가 설명";
            this.ColDescription.Name = "ColDescription";
            this.ColDescription.ReadOnly = true;
            this.ColDescription.Width = 250;
            // 
            // checkBoxNewVersion
            // 
            this.checkBoxNewVersion.AutoSize = true;
            this.checkBoxNewVersion.Location = new System.Drawing.Point(22, 202);
            this.checkBoxNewVersion.Name = "checkBoxNewVersion";
            this.checkBoxNewVersion.Size = new System.Drawing.Size(116, 16);
            this.checkBoxNewVersion.TabIndex = 1;
            this.checkBoxNewVersion.Text = "새 버전으로 저장";
            this.checkBoxNewVersion.UseVisualStyleBackColor = true;
            this.checkBoxNewVersion.CheckedChanged += new System.EventHandler(this.checkBoxNewVersion_CheckedChanged);
            // 
            // textBoxNewVersion
            // 
            this.textBoxNewVersion.Location = new System.Drawing.Point(211, 200);
            this.textBoxNewVersion.Name = "textBoxNewVersion";
            this.textBoxNewVersion.Size = new System.Drawing.Size(153, 21);
            this.textBoxNewVersion.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(164, 203);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "버전명 :";
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(462, 200);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(74, 21);
            this.buttonSave.TabIndex = 4;
            this.buttonSave.Text = "저장";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(551, 200);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(74, 21);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "취소";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Location = new System.Drawing.Point(212, 227);
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(153, 21);
            this.textBoxDescription.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(149, 230);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "부가설명 :";
            // 
            // FormVersion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(652, 263);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.textBoxNewVersion);
            this.Controls.Add(this.checkBoxNewVersion);
            this.Controls.Add(this.versionDataGrid);
            this.Name = "FormVersion";
            this.Text = "FormVersion";
            this.Load += new System.EventHandler(this.FormVersion_Load);
            ((System.ComponentModel.ISupportInitialize)(this.versionDataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView versionDataGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColVersionName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColOwner;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColCreateTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColLastAccessTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColDescription;
        private System.Windows.Forms.CheckBox checkBoxNewVersion;
        private System.Windows.Forms.TextBox textBoxNewVersion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Label label2;
    }
}