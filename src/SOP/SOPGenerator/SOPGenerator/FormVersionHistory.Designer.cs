namespace SOPGen
{
    partial class FormVersionHistory
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormVersionHistory));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.checkBoxNewBegin = new System.Windows.Forms.CheckBox();
            this.ColDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColVersionName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColOwner = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColCreateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColLastAccessTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.versionDataGrid = new System.Windows.Forms.DataGridView();
            this.axSkinFramework1 = new AxXtremeSkinFramework.AxSkinFramework();
            ((System.ComponentModel.ISupportInitialize)(this.versionDataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axSkinFramework1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(554, 205);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(74, 21);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "취소";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(465, 205);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(74, 21);
            this.buttonLoad.TabIndex = 2;
            this.buttonLoad.Text = "불러오기";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // checkBoxNewBegin
            // 
            this.checkBoxNewBegin.AutoSize = true;
            this.checkBoxNewBegin.Location = new System.Drawing.Point(325, 210);
            this.checkBoxNewBegin.Name = "checkBoxNewBegin";
            this.checkBoxNewBegin.Size = new System.Drawing.Size(116, 16);
            this.checkBoxNewBegin.TabIndex = 1;
            this.checkBoxNewBegin.Text = "빈 화면으로 시작";
            this.checkBoxNewBegin.UseVisualStyleBackColor = true;
            // 
            // ColDescription
            // 
            this.ColDescription.HeaderText = "부가 설명";
            this.ColDescription.Name = "ColDescription";
            this.ColDescription.ReadOnly = true;
            this.ColDescription.Width = 250;
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
            this.versionDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.versionDataGrid.Size = new System.Drawing.Size(652, 189);
            this.versionDataGrid.TabIndex = 0;
            this.versionDataGrid.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.versionDataGrid_CellContentDoubleClick);
            // 
            // axSkinFramework1
            // 
            this.axSkinFramework1.Enabled = true;
            this.axSkinFramework1.Location = new System.Drawing.Point(0, 210);
            this.axSkinFramework1.Name = "axSkinFramework1";
            this.axSkinFramework1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axSkinFramework1.OcxState")));
            this.axSkinFramework1.Size = new System.Drawing.Size(24, 24);
            this.axSkinFramework1.TabIndex = 13;
            // 
            // FormVersionHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(652, 245);
            this.Controls.Add(this.axSkinFramework1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.checkBoxNewBegin);
            this.Controls.Add(this.versionDataGrid);
            this.Name = "FormVersionHistory";
            this.Text = "버전 불러오기";
            this.Load += new System.EventHandler(this.FormVersionHistory_Load);
            ((System.ComponentModel.ISupportInitialize)(this.versionDataGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axSkinFramework1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.CheckBox checkBoxNewBegin;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColVersionName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColOwner;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColCreateTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColLastAccessTime;
        private System.Windows.Forms.DataGridView versionDataGrid;
        private AxXtremeSkinFramework.AxSkinFramework axSkinFramework1;
    }
}