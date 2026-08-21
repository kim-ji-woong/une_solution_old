namespace ConfirmScenario
{
    partial class FormMain
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.cboMaterialName = new System.Windows.Forms.ComboBox();
            this.cboWeather = new System.Windows.Forms.ComboBox();
            this.cboMixedFactor = new System.Windows.Forms.ComboBox();
            this.colPatient = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewPatient = new System.Windows.Forms.DataGridView();
            this.colAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cboReason = new System.Windows.Forms.ComboBox();
            this.dataGridViewActionList = new System.Windows.Forms.DataGridView();
            this.textBoxDistance = new System.Windows.Forms.TextBox();
            this.textBoxInitialDistance = new System.Windows.Forms.TextBox();
            this.textBoxCountOfDeath = new System.Windows.Forms.TextBox();
            this.textBoxPlace = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxCountOfBuilding = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxMaterial = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.radioDay = new System.Windows.Forms.RadioButton();
            this.radioNight = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPatient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewActionList)).BeginInit();
            this.SuspendLayout();
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
            this.cboMaterialName.Location = new System.Drawing.Point(101, 35);
            this.cboMaterialName.Name = "cboMaterialName";
            this.cboMaterialName.Size = new System.Drawing.Size(121, 20);
            this.cboMaterialName.TabIndex = 30;
            this.cboMaterialName.SelectedIndexChanged += new System.EventHandler(this.cboMaterialName_SelectedIndexChanged);
            // 
            // cboWeather
            // 
            this.cboWeather.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboWeather.FormattingEnabled = true;
            this.cboWeather.Items.AddRange(new object[] {
            "맑음",
            "강풍",
            "비"});
            this.cboWeather.Location = new System.Drawing.Point(337, 76);
            this.cboWeather.Name = "cboWeather";
            this.cboWeather.Size = new System.Drawing.Size(121, 20);
            this.cboWeather.TabIndex = 28;
            // 
            // cboMixedFactor
            // 
            this.cboMixedFactor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMixedFactor.FormattingEnabled = true;
            this.cboMixedFactor.Items.AddRange(new object[] {
            "물",
            "열",
            "없음"});
            this.cboMixedFactor.Location = new System.Drawing.Point(101, 195);
            this.cboMixedFactor.Name = "cboMixedFactor";
            this.cboMixedFactor.Size = new System.Drawing.Size(121, 20);
            this.cboMixedFactor.TabIndex = 27;
            // 
            // colPatient
            // 
            this.colPatient.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colPatient.HeaderText = "환자응급조치";
            this.colPatient.Name = "colPatient";
            // 
            // dataGridViewPatient
            // 
            this.dataGridViewPatient.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPatient.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPatient});
            this.dataGridViewPatient.Location = new System.Drawing.Point(12, 424);
            this.dataGridViewPatient.Name = "dataGridViewPatient";
            this.dataGridViewPatient.RowHeadersVisible = false;
            this.dataGridViewPatient.RowTemplate.Height = 23;
            this.dataGridViewPatient.Size = new System.Drawing.Size(444, 163);
            this.dataGridViewPatient.TabIndex = 25;
            // 
            // colAction
            // 
            this.colAction.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colAction.HeaderText = "대응내용";
            this.colAction.Name = "colAction";
            // 
            // cboReason
            // 
            this.cboReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboReason.FormattingEnabled = true;
            this.cboReason.Items.AddRange(new object[] {
            "화재",
            "누출"});
            this.cboReason.Location = new System.Drawing.Point(101, 76);
            this.cboReason.Name = "cboReason";
            this.cboReason.Size = new System.Drawing.Size(121, 20);
            this.cboReason.TabIndex = 29;
            // 
            // dataGridViewActionList
            // 
            this.dataGridViewActionList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewActionList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colAction});
            this.dataGridViewActionList.Location = new System.Drawing.Point(12, 237);
            this.dataGridViewActionList.Name = "dataGridViewActionList";
            this.dataGridViewActionList.RowHeadersVisible = false;
            this.dataGridViewActionList.RowTemplate.Height = 23;
            this.dataGridViewActionList.Size = new System.Drawing.Size(444, 163);
            this.dataGridViewActionList.TabIndex = 24;
            // 
            // textBoxDistance
            // 
            this.textBoxDistance.Location = new System.Drawing.Point(337, 194);
            this.textBoxDistance.Name = "textBoxDistance";
            this.textBoxDistance.Size = new System.Drawing.Size(119, 21);
            this.textBoxDistance.TabIndex = 20;
            // 
            // textBoxInitialDistance
            // 
            this.textBoxInitialDistance.Location = new System.Drawing.Point(337, 154);
            this.textBoxInitialDistance.Name = "textBoxInitialDistance";
            this.textBoxInitialDistance.Size = new System.Drawing.Size(119, 21);
            this.textBoxInitialDistance.TabIndex = 19;
            // 
            // textBoxCountOfDeath
            // 
            this.textBoxCountOfDeath.Location = new System.Drawing.Point(337, 117);
            this.textBoxCountOfDeath.Name = "textBoxCountOfDeath";
            this.textBoxCountOfDeath.Size = new System.Drawing.Size(119, 21);
            this.textBoxCountOfDeath.TabIndex = 23;
            // 
            // textBoxPlace
            // 
            this.textBoxPlace.Location = new System.Drawing.Point(337, 35);
            this.textBoxPlace.Name = "textBoxPlace";
            this.textBoxPlace.Size = new System.Drawing.Size(119, 21);
            this.textBoxPlace.TabIndex = 18;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(246, 197);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(80, 12);
            this.label10.TabIndex = 8;
            this.label10.Text = "대피거리(km)";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(236, 157);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(98, 12);
            this.label8.TabIndex = 15;
            this.label8.Text = "초기이격거리(m)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(246, 120);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 14;
            this.label6.Text = "사상자인원";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(246, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 13;
            this.label4.Text = "기상";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(246, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 12;
            this.label2.Text = "장소";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 197);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 11;
            this.label9.Text = "반응물질";
            // 
            // textBoxCountOfBuilding
            // 
            this.textBoxCountOfBuilding.Location = new System.Drawing.Point(103, 154);
            this.textBoxCountOfBuilding.Name = "textBoxCountOfBuilding";
            this.textBoxCountOfBuilding.Size = new System.Drawing.Size(119, 21);
            this.textBoxCountOfBuilding.TabIndex = 21;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 157);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 10;
            this.label7.Text = "건물숫자";
            // 
            // textBoxMaterial
            // 
            this.textBoxMaterial.Location = new System.Drawing.Point(103, 117);
            this.textBoxMaterial.Name = "textBoxMaterial";
            this.textBoxMaterial.Size = new System.Drawing.Size(119, 21);
            this.textBoxMaterial.TabIndex = 22;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 120);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "발생물질";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 16;
            this.label3.Text = "사고원인";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 17;
            this.label1.Text = "물질명";
            // 
            // radioDay
            // 
            this.radioDay.AutoSize = true;
            this.radioDay.Location = new System.Drawing.Point(14, 12);
            this.radioDay.Name = "radioDay";
            this.radioDay.Size = new System.Drawing.Size(47, 16);
            this.radioDay.TabIndex = 31;
            this.radioDay.TabStop = true;
            this.radioDay.Text = "주간";
            this.radioDay.UseVisualStyleBackColor = true;
            // 
            // radioNight
            // 
            this.radioNight.AutoSize = true;
            this.radioNight.Location = new System.Drawing.Point(67, 12);
            this.radioNight.Name = "radioNight";
            this.radioNight.Size = new System.Drawing.Size(47, 16);
            this.radioNight.TabIndex = 31;
            this.radioNight.TabStop = true;
            this.radioNight.Text = "야간";
            this.radioNight.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 602);
            this.Controls.Add(this.radioNight);
            this.Controls.Add(this.radioDay);
            this.Controls.Add(this.cboMaterialName);
            this.Controls.Add(this.cboWeather);
            this.Controls.Add(this.cboMixedFactor);
            this.Controls.Add(this.dataGridViewPatient);
            this.Controls.Add(this.cboReason);
            this.Controls.Add(this.dataGridViewActionList);
            this.Controls.Add(this.textBoxDistance);
            this.Controls.Add(this.textBoxInitialDistance);
            this.Controls.Add(this.textBoxCountOfDeath);
            this.Controls.Add(this.textBoxPlace);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.textBoxCountOfBuilding);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxMaterial);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Name = "FormMain";
            this.Text = "시나리오 확인";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.FormMain_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.FormMain_DragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPatient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewActionList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboMaterialName;
        private System.Windows.Forms.ComboBox cboWeather;
        private System.Windows.Forms.ComboBox cboMixedFactor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPatient;
        private System.Windows.Forms.DataGridView dataGridViewPatient;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAction;
        private System.Windows.Forms.ComboBox cboReason;
        private System.Windows.Forms.DataGridView dataGridViewActionList;
        private System.Windows.Forms.TextBox textBoxDistance;
        private System.Windows.Forms.TextBox textBoxInitialDistance;
        private System.Windows.Forms.TextBox textBoxCountOfDeath;
        private System.Windows.Forms.TextBox textBoxPlace;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxCountOfBuilding;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxMaterial;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioDay;
        private System.Windows.Forms.RadioButton radioNight;
    }
}

