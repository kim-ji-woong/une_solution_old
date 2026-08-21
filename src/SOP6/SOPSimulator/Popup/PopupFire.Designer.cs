namespace SOPMonitoringSystem
{
    partial class PopupFire
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopupFire));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textPos2 = new System.Windows.Forms.TextBox();
            this.textAccType = new System.Windows.Forms.TextBox();
            this.textMessage = new System.Windows.Forms.RichTextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.cmbAnnoCount = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.m_comboDisaster = new System.Windows.Forms.ComboBox();
            this.btnCancel2 = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.checkUseSMS = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dataGridViewFax = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBox1 = new System.Windows.Forms.RichTextBox();
            this.dataGridViewSMS = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkBoxSiren = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSMS)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textPos2);
            this.groupBox1.Controls.Add(this.textAccType);
            this.groupBox1.Controls.Add(this.textMessage);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnOK);
            this.groupBox1.Controls.Add(this.cmbAnnoCount);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.m_comboDisaster);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(640, 386);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // textPos2
            // 
            this.textPos2.Location = new System.Drawing.Point(208, 174);
            this.textPos2.Name = "textPos2";
            this.textPos2.Size = new System.Drawing.Size(90, 21);
            this.textPos2.TabIndex = 53;
            // 
            // textAccType
            // 
            this.textAccType.Location = new System.Drawing.Point(14, 174);
            this.textAccType.Name = "textAccType";
            this.textAccType.Size = new System.Drawing.Size(90, 21);
            this.textAccType.TabIndex = 52;
            // 
            // textMessage
            // 
            this.textMessage.BackColor = System.Drawing.SystemColors.Control;
            this.textMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textMessage.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textMessage.Location = new System.Drawing.Point(14, 114);
            this.textMessage.Name = "textMessage";
            this.textMessage.ReadOnly = true;
            this.textMessage.Size = new System.Drawing.Size(612, 253);
            this.textMessage.TabIndex = 51;
            this.textMessage.Text = resources.GetString("textMessage.Text");
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(537, 33);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 46);
            this.btnCancel.TabIndex = 49;
            this.btnCancel.Text = "방송입력취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(442, 33);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 46);
            this.btnOK.TabIndex = 50;
            this.btnOK.Text = "방송입력";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // cmbAnnoCount
            // 
            this.cmbAnnoCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAnnoCount.FormattingEnabled = true;
            this.cmbAnnoCount.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "상황 종료시까지 무한 반복"});
            this.cmbAnnoCount.Location = new System.Drawing.Point(246, 59);
            this.cmbAnnoCount.Name = "cmbAnnoCount";
            this.cmbAnnoCount.Size = new System.Drawing.Size(161, 20);
            this.cmbAnnoCount.TabIndex = 48;
            this.cmbAnnoCount.SelectedIndexChanged += new System.EventHandler(this.cmbAnnoCount_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 12);
            this.label2.TabIndex = 47;
            this.label2.Text = "방송 반복 회수 :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 46;
            this.label1.Text = "재난 상황 :";
            // 
            // m_comboDisaster
            // 
            this.m_comboDisaster.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_comboDisaster.FormattingEnabled = true;
            this.m_comboDisaster.Items.AddRange(new object[] {
            "화재, 유출사고(비상상황)",
            "기타(시스템제공)",
            "사용자 입력",
            "시나리오"});
            this.m_comboDisaster.Location = new System.Drawing.Point(112, 33);
            this.m_comboDisaster.Name = "m_comboDisaster";
            this.m_comboDisaster.Size = new System.Drawing.Size(295, 20);
            this.m_comboDisaster.TabIndex = 45;
            this.m_comboDisaster.SelectedIndexChanged += new System.EventHandler(this.m_comboDisaster_SelectedIndexChanged);
            // 
            // btnCancel2
            // 
            this.btnCancel2.Location = new System.Drawing.Point(577, 412);
            this.btnCancel2.Name = "btnCancel2";
            this.btnCancel2.Size = new System.Drawing.Size(75, 23);
            this.btnCancel2.TabIndex = 51;
            this.btnCancel2.Text = "취소";
            this.btnCancel2.UseVisualStyleBackColor = true;
            this.btnCancel2.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(468, 412);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 50;
            this.btnNext.Text = "다음>";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(387, 412);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 23);
            this.btnBack.TabIndex = 49;
            this.btnBack.Text = "<이전";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.checkUseSMS);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.dataGridViewFax);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.dataGridViewSMS);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(640, 386);
            this.groupBox2.TabIndex = 52;
            this.groupBox2.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(484, 308);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(83, 30);
            this.button1.TabIndex = 20;
            this.button1.Text = "실행";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            // 
            // checkUseSMS
            // 
            this.checkUseSMS.AutoSize = true;
            this.checkUseSMS.Enabled = false;
            this.checkUseSMS.Location = new System.Drawing.Point(21, 316);
            this.checkUseSMS.Name = "checkUseSMS";
            this.checkUseSMS.Size = new System.Drawing.Size(116, 16);
            this.checkUseSMS.TabIndex = 17;
            this.checkUseSMS.Text = "문자 메시지 사용";
            this.checkUseSMS.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(300, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 12);
            this.label3.TabIndex = 16;
            this.label3.Text = "Fax 수신처";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 12);
            this.label4.TabIndex = 15;
            this.label4.Text = "문자 메시지 수신처";
            // 
            // dataGridViewFax
            // 
            this.dataGridViewFax.AllowUserToResizeColumns = false;
            this.dataGridViewFax.AllowUserToResizeRows = false;
            this.dataGridViewFax.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewFax.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.dataGridViewFax.Location = new System.Drawing.Point(302, 52);
            this.dataGridViewFax.Name = "dataGridViewFax";
            this.dataGridViewFax.ReadOnly = true;
            this.dataGridViewFax.RowHeadersVisible = false;
            this.dataGridViewFax.RowTemplate.Height = 23;
            this.dataGridViewFax.Size = new System.Drawing.Size(265, 106);
            this.dataGridViewFax.TabIndex = 13;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.FillWeight = 150F;
            this.dataGridViewTextBoxColumn1.HeaderText = "수신처";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 150;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.HeaderText = "Fax 번호";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(19, 180);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(266, 115);
            this.textBox1.TabIndex = 18;
            this.textBox1.Text = "";
            // 
            // dataGridViewSMS
            // 
            this.dataGridViewSMS.AllowUserToResizeColumns = false;
            this.dataGridViewSMS.AllowUserToResizeRows = false;
            this.dataGridViewSMS.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSMS.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.dataGridViewSMS.Location = new System.Drawing.Point(19, 52);
            this.dataGridViewSMS.Name = "dataGridViewSMS";
            this.dataGridViewSMS.ReadOnly = true;
            this.dataGridViewSMS.RowHeadersVisible = false;
            this.dataGridViewSMS.RowTemplate.Height = 23;
            this.dataGridViewSMS.Size = new System.Drawing.Size(265, 106);
            this.dataGridViewSMS.TabIndex = 14;
            // 
            // Column1
            // 
            this.Column1.FillWeight = 150F;
            this.Column1.HeaderText = "수신처";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 150;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.HeaderText = "전화 번호";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // checkBoxSiren
            // 
            this.checkBoxSiren.AutoSize = true;
            this.checkBoxSiren.Checked = true;
            this.checkBoxSiren.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSiren.Location = new System.Drawing.Point(33, 416);
            this.checkBoxSiren.Name = "checkBoxSiren";
            this.checkBoxSiren.Size = new System.Drawing.Size(88, 16);
            this.checkBoxSiren.TabIndex = 54;
            this.checkBoxSiren.Text = "사이렌 사용";
            this.checkBoxSiren.UseVisualStyleBackColor = true;
            // 
            // PopupFire
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 447);
            this.Controls.Add(this.checkBoxSiren);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnCancel2);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PopupFire";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "화재, 유출사고 안내 방송";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSMS)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textPos2;
        private System.Windows.Forms.TextBox textAccType;
        private System.Windows.Forms.RichTextBox textMessage;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ComboBox cmbAnnoCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox m_comboDisaster;
        private System.Windows.Forms.Button btnCancel2;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkUseSMS;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView dataGridViewFax;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.RichTextBox textBox1;
        private System.Windows.Forms.DataGridView dataGridViewSMS;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.CheckBox checkBoxSiren;



    }
}