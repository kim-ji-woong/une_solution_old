namespace SoilMan.TabPages
{
    partial class Page기능회복기간
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.labelUnitInfo = new System.Windows.Forms.Label();
            this.checkBoxEditMode = new System.Windows.Forms.CheckBox();
            this.dataGridView1 = new UnE.Controls.MergedDataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFunction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFarming = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSteam = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWashing = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOxidation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHeat = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSave = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelUnitInfo
            // 
            this.labelUnitInfo.AutoSize = true;
            this.labelUnitInfo.Location = new System.Drawing.Point(766, 8);
            this.labelUnitInfo.Name = "labelUnitInfo";
            this.labelUnitInfo.Size = new System.Drawing.Size(29, 12);
            this.labelUnitInfo.TabIndex = 7;
            this.labelUnitInfo.Text = "단위";
            this.labelUnitInfo.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // checkBoxEditMode
            // 
            this.checkBoxEditMode.AutoSize = true;
            this.checkBoxEditMode.Location = new System.Drawing.Point(12, 4);
            this.checkBoxEditMode.Name = "checkBoxEditMode";
            this.checkBoxEditMode.Size = new System.Drawing.Size(72, 16);
            this.checkBoxEditMode.TabIndex = 6;
            this.checkBoxEditMode.Text = "편집모드";
            this.checkBoxEditMode.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colFunction,
            this.colBio,
            this.colFarming,
            this.colSteam,
            this.colWashing,
            this.colOxidation,
            this.colHeat});
            this.dataGridView1.Location = new System.Drawing.Point(0, 30);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(805, 460);
            this.dataGridView1.TabIndex = 8;
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridView1_CurrentCellDirtyStateChanged);
            // 
            // colNo
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle1;
            this.colNo.HeaderText = "번호";
            this.colNo.Name = "colNo";
            this.colNo.Width = 40;
            // 
            // colFunction
            // 
            this.colFunction.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.colFunction.DefaultCellStyle = dataGridViewCellStyle2;
            this.colFunction.HeaderText = "기능";
            this.colFunction.Name = "colFunction";
            this.colFunction.Width = 54;
            // 
            // colBio
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colBio.DefaultCellStyle = dataGridViewCellStyle3;
            this.colBio.HeaderText = "생물통풍";
            this.colBio.Name = "colBio";
            // 
            // colFarming
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colFarming.DefaultCellStyle = dataGridViewCellStyle4;
            this.colFarming.HeaderText = "토양경작";
            this.colFarming.Name = "colFarming";
            // 
            // colSteam
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colSteam.DefaultCellStyle = dataGridViewCellStyle5;
            this.colSteam.HeaderText = "증기추출";
            this.colSteam.Name = "colSteam";
            // 
            // colWashing
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colWashing.DefaultCellStyle = dataGridViewCellStyle6;
            this.colWashing.HeaderText = "토양세척";
            this.colWashing.Name = "colWashing";
            // 
            // colOxidation
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colOxidation.DefaultCellStyle = dataGridViewCellStyle7;
            this.colOxidation.HeaderText = "화학산화";
            this.colOxidation.Name = "colOxidation";
            // 
            // colHeat
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colHeat.DefaultCellStyle = dataGridViewCellStyle8;
            this.colHeat.HeaderText = "열탈착";
            this.colHeat.Name = "colHeat";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(90, 0);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(52, 23);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "저장";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(632, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "초기화";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Page기능회복기간
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 490);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.labelUnitInfo);
            this.Controls.Add(this.checkBoxEditMode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Page기능회복기간";
            this.Text = "Page기능회복기간";
            this.Load += new System.EventHandler(this.Page기능회복기간_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelUnitInfo;
        private System.Windows.Forms.CheckBox checkBoxEditMode;
        private UnE.Controls.MergedDataGridView dataGridView1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFunction;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBio;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFarming;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSteam;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWashing;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOxidation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHeat;
        private System.Windows.Forms.Button button1;
    }
}