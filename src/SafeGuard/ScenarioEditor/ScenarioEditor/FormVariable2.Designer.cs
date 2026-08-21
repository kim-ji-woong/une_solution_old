namespace ScenarioEditor
{
    partial class FormVariable2
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
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxMaterial = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxInitialDistance = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxDistance = new System.Windows.Forms.TextBox();
            this.dataGridViewActionList = new System.Windows.Forms.DataGridView();
            this.colAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewPatient = new System.Windows.Forms.DataGridView();
            this.colPatient = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnApply = new System.Windows.Forms.Button();
            this.cboMixedFactor = new System.Windows.Forms.ComboBox();
            this.cboWeather = new System.Windows.Forms.ComboBox();
            this.cboReason = new System.Windows.Forms.ComboBox();
            this.cboMaterialName = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewActionList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPatient)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "물질명";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "사고원인";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(246, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "기상";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 102);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "발생물질";
            // 
            // textBoxMaterial
            // 
            this.textBoxMaterial.Location = new System.Drawing.Point(103, 99);
            this.textBoxMaterial.Name = "textBoxMaterial";
            this.textBoxMaterial.Size = new System.Drawing.Size(119, 21);
            this.textBoxMaterial.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(236, 139);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(98, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "초기이격거리(m)";
            // 
            // textBoxInitialDistance
            // 
            this.textBoxInitialDistance.Location = new System.Drawing.Point(337, 136);
            this.textBoxInitialDistance.Name = "textBoxInitialDistance";
            this.textBoxInitialDistance.Size = new System.Drawing.Size(119, 21);
            this.textBoxInitialDistance.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 179);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 0;
            this.label9.Text = "반응물질";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(246, 179);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(80, 12);
            this.label10.TabIndex = 0;
            this.label10.Text = "대피거리(km)";
            // 
            // textBoxDistance
            // 
            this.textBoxDistance.Location = new System.Drawing.Point(337, 176);
            this.textBoxDistance.Name = "textBoxDistance";
            this.textBoxDistance.Size = new System.Drawing.Size(119, 21);
            this.textBoxDistance.TabIndex = 1;
            // 
            // dataGridViewActionList
            // 
            this.dataGridViewActionList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewActionList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colAction});
            this.dataGridViewActionList.Location = new System.Drawing.Point(12, 219);
            this.dataGridViewActionList.Name = "dataGridViewActionList";
            this.dataGridViewActionList.RowHeadersVisible = false;
            this.dataGridViewActionList.RowTemplate.Height = 23;
            this.dataGridViewActionList.Size = new System.Drawing.Size(444, 163);
            this.dataGridViewActionList.TabIndex = 2;
            this.dataGridViewActionList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView_KeyDown);
            // 
            // colAction
            // 
            this.colAction.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colAction.HeaderText = "대응내용";
            this.colAction.Name = "colAction";
            // 
            // dataGridViewPatient
            // 
            this.dataGridViewPatient.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPatient.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPatient});
            this.dataGridViewPatient.Location = new System.Drawing.Point(12, 406);
            this.dataGridViewPatient.Name = "dataGridViewPatient";
            this.dataGridViewPatient.RowHeadersVisible = false;
            this.dataGridViewPatient.RowTemplate.Height = 23;
            this.dataGridViewPatient.Size = new System.Drawing.Size(444, 163);
            this.dataGridViewPatient.TabIndex = 2;
            this.dataGridViewPatient.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView_KeyDown);
            // 
            // colPatient
            // 
            this.colPatient.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colPatient.HeaderText = "환자응급조치";
            this.colPatient.Name = "colPatient";
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(381, 577);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "적용";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // cboMixedFactor
            // 
            this.cboMixedFactor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMixedFactor.FormattingEnabled = true;
            this.cboMixedFactor.Items.AddRange(new object[] {
            "물",
            "열",
            "없음"});
            this.cboMixedFactor.Location = new System.Drawing.Point(101, 177);
            this.cboMixedFactor.Name = "cboMixedFactor";
            this.cboMixedFactor.Size = new System.Drawing.Size(121, 20);
            this.cboMixedFactor.TabIndex = 4;
            // 
            // cboWeather
            // 
            this.cboWeather.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboWeather.FormattingEnabled = true;
            this.cboWeather.Items.AddRange(new object[] {
            "맑음",
            "강풍",
            "비"});
            this.cboWeather.Location = new System.Drawing.Point(337, 58);
            this.cboWeather.Name = "cboWeather";
            this.cboWeather.Size = new System.Drawing.Size(121, 20);
            this.cboWeather.TabIndex = 5;
            // 
            // cboReason
            // 
            this.cboReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboReason.FormattingEnabled = true;
            this.cboReason.Items.AddRange(new object[] {
            "화재",
            "누출"});
            this.cboReason.Location = new System.Drawing.Point(101, 58);
            this.cboReason.Name = "cboReason";
            this.cboReason.Size = new System.Drawing.Size(121, 20);
            this.cboReason.TabIndex = 6;
            // 
            // cboMaterialName
            // 
            this.cboMaterialName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMaterialName.FormattingEnabled = true;
            this.cboMaterialName.Items.AddRange(new object[] {
            "황산",
            "질산",
            "플루오르화수소",
            "플루오르화수소(용액)",
            "플루오르화수소(가스)",
            "벤젠",
            "산화질소",
            "암모니아",
            "염소"});
            this.cboMaterialName.Location = new System.Drawing.Point(101, 17);
            this.cboMaterialName.Name = "cboMaterialName";
            this.cboMaterialName.Size = new System.Drawing.Size(121, 20);
            this.cboMaterialName.TabIndex = 7;
            this.cboMaterialName.SelectedIndexChanged += new System.EventHandler(this.cboMaterialName_SelectedIndexChanged);
            // 
            // FormVariable2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 607);
            this.Controls.Add(this.cboMaterialName);
            this.Controls.Add(this.cboReason);
            this.Controls.Add(this.cboWeather);
            this.Controls.Add(this.cboMixedFactor);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.dataGridViewPatient);
            this.Controls.Add(this.dataGridViewActionList);
            this.Controls.Add(this.textBoxDistance);
            this.Controls.Add(this.textBoxInitialDistance);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.textBoxMaterial);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Name = "FormVariable2";
            this.Text = "공통변수";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormVariable2_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewActionList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPatient)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxMaterial;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxInitialDistance;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxDistance;
        private System.Windows.Forms.DataGridView dataGridViewActionList;
        private System.Windows.Forms.DataGridView dataGridViewPatient;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAction;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPatient;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.ComboBox cboMixedFactor;
        private System.Windows.Forms.ComboBox cboWeather;
        private System.Windows.Forms.ComboBox cboReason;
        private System.Windows.Forms.ComboBox cboMaterialName;
    }
}