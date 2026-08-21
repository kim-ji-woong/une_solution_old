namespace SMS_P_Simulator
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxDisasterCategory = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxDisasterSubCategory = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxActionStep = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioOver80 = new System.Windows.Forms.RadioButton();
            this.radioLess80 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxUseItem = new System.Windows.Forms.CheckBox();
            this.radioMission = new System.Windows.Forms.RadioButton();
            this.radioNormal = new System.Windows.Forms.RadioButton();
            this.radioEndSOP = new System.Windows.Forms.RadioButton();
            this.radioInternal = new System.Windows.Forms.RadioButton();
            this.radioBeginSOP = new System.Windows.Forms.RadioButton();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxPhoneNumber = new System.Windows.Forms.MaskedTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnTrans = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxActionSubStep = new System.Windows.Forms.ComboBox();
            this.btnMakeMessage = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.checkBoxOneByOne = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxCaller = new System.Windows.Forms.MaskedTextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBoxLocation = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "재난 Category : ";
            // 
            // comboBoxDisasterCategory
            // 
            this.comboBoxDisasterCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDisasterCategory.FormattingEnabled = true;
            this.comboBoxDisasterCategory.Items.AddRange(new object[] {
            "자연재해",
            "화재",
            "유출사고",
            "테러",
            "인명구조 및 의료지원",
            "기타"});
            this.comboBoxDisasterCategory.Location = new System.Drawing.Point(120, 34);
            this.comboBoxDisasterCategory.Name = "comboBoxDisasterCategory";
            this.comboBoxDisasterCategory.Size = new System.Drawing.Size(112, 20);
            this.comboBoxDisasterCategory.TabIndex = 1;
            this.comboBoxDisasterCategory.SelectedIndexChanged += new System.EventHandler(this.comboBoxDisasterCategory_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "재난 유형        : ";
            // 
            // comboBoxDisasterSubCategory
            // 
            this.comboBoxDisasterSubCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDisasterSubCategory.FormattingEnabled = true;
            this.comboBoxDisasterSubCategory.Location = new System.Drawing.Point(120, 58);
            this.comboBoxDisasterSubCategory.Name = "comboBoxDisasterSubCategory";
            this.comboBoxDisasterSubCategory.Size = new System.Drawing.Size(112, 20);
            this.comboBoxDisasterSubCategory.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "대응 단계        :";
            // 
            // comboBoxActionStep
            // 
            this.comboBoxActionStep.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxActionStep.FormattingEnabled = true;
            this.comboBoxActionStep.Items.AddRange(new object[] {
            "예방",
            "대비",
            "대응",
            "복구",
            "기타"});
            this.comboBoxActionStep.Location = new System.Drawing.Point(120, 83);
            this.comboBoxActionStep.Name = "comboBoxActionStep";
            this.comboBoxActionStep.Size = new System.Drawing.Size(112, 20);
            this.comboBoxActionStep.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioOver80);
            this.groupBox1.Controls.Add(this.radioLess80);
            this.groupBox1.Location = new System.Drawing.Point(259, 26);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(118, 102);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "메시지 길이";
            // 
            // radioOver80
            // 
            this.radioOver80.AutoSize = true;
            this.radioOver80.Location = new System.Drawing.Point(14, 47);
            this.radioOver80.Name = "radioOver80";
            this.radioOver80.Size = new System.Drawing.Size(88, 16);
            this.radioOver80.TabIndex = 0;
            this.radioOver80.TabStop = true;
            this.radioOver80.Text = "80Byte 초과";
            this.radioOver80.UseVisualStyleBackColor = true;
            // 
            // radioLess80
            // 
            this.radioLess80.AutoSize = true;
            this.radioLess80.Location = new System.Drawing.Point(14, 22);
            this.radioLess80.Name = "radioLess80";
            this.radioLess80.Size = new System.Drawing.Size(88, 16);
            this.radioLess80.TabIndex = 0;
            this.radioLess80.TabStop = true;
            this.radioLess80.Text = "80Byte 이내";
            this.radioLess80.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxUseItem);
            this.groupBox2.Controls.Add(this.radioMission);
            this.groupBox2.Controls.Add(this.radioNormal);
            this.groupBox2.Controls.Add(this.radioEndSOP);
            this.groupBox2.Controls.Add(this.radioInternal);
            this.groupBox2.Controls.Add(this.radioBeginSOP);
            this.groupBox2.Location = new System.Drawing.Point(403, 26);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(234, 102);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "메시지 Type";
            // 
            // checkBoxUseItem
            // 
            this.checkBoxUseItem.AutoSize = true;
            this.checkBoxUseItem.Location = new System.Drawing.Point(109, 72);
            this.checkBoxUseItem.Name = "checkBoxUseItem";
            this.checkBoxUseItem.Size = new System.Drawing.Size(100, 16);
            this.checkBoxUseItem.TabIndex = 1;
            this.checkBoxUseItem.Text = "점검항목 사용";
            this.checkBoxUseItem.UseVisualStyleBackColor = true;
            this.checkBoxUseItem.Visible = false;
            // 
            // radioMission
            // 
            this.radioMission.AutoSize = true;
            this.radioMission.Location = new System.Drawing.Point(107, 47);
            this.radioMission.Name = "radioMission";
            this.radioMission.Size = new System.Drawing.Size(101, 16);
            this.radioMission.TabIndex = 0;
            this.radioMission.TabStop = true;
            this.radioMission.Text = "임무/점검항목";
            this.radioMission.UseVisualStyleBackColor = true;
            this.radioMission.CheckedChanged += new System.EventHandler(this.radioMission_CheckedChanged);
            // 
            // radioNormal
            // 
            this.radioNormal.AutoSize = true;
            this.radioNormal.Location = new System.Drawing.Point(14, 71);
            this.radioNormal.Name = "radioNormal";
            this.radioNormal.Size = new System.Drawing.Size(71, 16);
            this.radioNormal.TabIndex = 0;
            this.radioNormal.TabStop = true;
            this.radioNormal.Text = "공지사항";
            this.radioNormal.UseVisualStyleBackColor = true;
            this.radioNormal.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioEndSOP
            // 
            this.radioEndSOP.AutoSize = true;
            this.radioEndSOP.Location = new System.Drawing.Point(14, 47);
            this.radioEndSOP.Name = "radioEndSOP";
            this.radioEndSOP.Size = new System.Drawing.Size(76, 16);
            this.radioEndSOP.TabIndex = 0;
            this.radioEndSOP.TabStop = true;
            this.radioEndSOP.Text = "SOP 종료";
            this.radioEndSOP.UseVisualStyleBackColor = true;
            this.radioEndSOP.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioInternal
            // 
            this.radioInternal.AutoSize = true;
            this.radioInternal.Location = new System.Drawing.Point(107, 24);
            this.radioInternal.Name = "radioInternal";
            this.radioInternal.Size = new System.Drawing.Size(71, 16);
            this.radioInternal.TabIndex = 0;
            this.radioInternal.TabStop = true;
            this.radioInternal.Text = "상황전파";
            this.radioInternal.UseVisualStyleBackColor = true;
            this.radioInternal.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioBeginSOP
            // 
            this.radioBeginSOP.AutoSize = true;
            this.radioBeginSOP.Location = new System.Drawing.Point(14, 24);
            this.radioBeginSOP.Name = "radioBeginSOP";
            this.radioBeginSOP.Size = new System.Drawing.Size(76, 16);
            this.radioBeginSOP.TabIndex = 0;
            this.radioBeginSOP.TabStop = true;
            this.radioBeginSOP.Text = "SOP 시작";
            this.radioBeginSOP.UseVisualStyleBackColor = true;
            this.radioBeginSOP.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(26, 177);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(611, 187);
            this.dataGridView1.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 407);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "수신 전화번호 :";
            // 
            // textBoxPhoneNumber
            // 
            this.textBoxPhoneNumber.Location = new System.Drawing.Point(118, 402);
            this.textBoxPhoneNumber.Mask = "00000000000";
            this.textBoxPhoneNumber.Name = "textBoxPhoneNumber";
            this.textBoxPhoneNumber.Size = new System.Drawing.Size(114, 21);
            this.textBoxPhoneNumber.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 431);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(209, 12);
            this.label5.TabIndex = 5;
            this.label5.Text = "(\'-\'이나 빈칸없이 숫자만 입력하세요)";
            // 
            // btnTrans
            // 
            this.btnTrans.Location = new System.Drawing.Point(555, 404);
            this.btnTrans.Name = "btnTrans";
            this.btnTrans.Size = new System.Drawing.Size(81, 30);
            this.btnTrans.TabIndex = 7;
            this.btnTrans.Text = "메시지 전송";
            this.btnTrans.UseVisualStyleBackColor = true;
            this.btnTrans.Click += new System.EventHandler(this.btnTrans_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(23, 113);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(97, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "상세대응단계   : ";
            // 
            // comboBoxActionSubStep
            // 
            this.comboBoxActionSubStep.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxActionSubStep.FormattingEnabled = true;
            this.comboBoxActionSubStep.Items.AddRange(new object[] {
            "사용안함",
            "관심",
            "주의",
            "경계",
            "심각",
            "준비",
            "기타"});
            this.comboBoxActionSubStep.Location = new System.Drawing.Point(120, 108);
            this.comboBoxActionSubStep.Name = "comboBoxActionSubStep";
            this.comboBoxActionSubStep.Size = new System.Drawing.Size(112, 20);
            this.comboBoxActionSubStep.TabIndex = 1;
            // 
            // btnMakeMessage
            // 
            this.btnMakeMessage.Location = new System.Drawing.Point(468, 404);
            this.btnMakeMessage.Name = "btnMakeMessage";
            this.btnMakeMessage.Size = new System.Drawing.Size(81, 30);
            this.btnMakeMessage.TabIndex = 7;
            this.btnMakeMessage.Text = "메시지 생성";
            this.btnMakeMessage.UseVisualStyleBackColor = true;
            this.btnMakeMessage.Click += new System.EventHandler(this.btnMakeMessage_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(336, 393);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(70, 12);
            this.labelStatus.TabIndex = 8;
            this.labelStatus.Text = "메시지 Byte";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelStatus.Visible = false;
            // 
            // checkBoxOneByOne
            // 
            this.checkBoxOneByOne.AutoSize = true;
            this.checkBoxOneByOne.Location = new System.Drawing.Point(286, 425);
            this.checkBoxOneByOne.Name = "checkBoxOneByOne";
            this.checkBoxOneByOne.Size = new System.Drawing.Size(140, 16);
            this.checkBoxOneByOne.TabIndex = 9;
            this.checkBoxOneByOne.Text = "메시지 하나씩 보내기";
            this.checkBoxOneByOne.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(24, 382);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 12);
            this.label7.TabIndex = 5;
            this.label7.Text = "발신 전화번호 :";
            // 
            // textBoxCaller
            // 
            this.textBoxCaller.Location = new System.Drawing.Point(119, 377);
            this.textBoxCaller.Mask = "00000000000";
            this.textBoxCaller.Name = "textBoxCaller";
            this.textBoxCaller.Size = new System.Drawing.Size(114, 21);
            this.textBoxCaller.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(23, 139);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(97, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "재난발생위치   : ";
            // 
            // comboBoxLocation
            // 
            this.comboBoxLocation.FormattingEnabled = true;
            this.comboBoxLocation.Items.AddRange(new object[] {
            "1, 2호기",
            "3, 4호기",
            "5, 6호기"});
            this.comboBoxLocation.Location = new System.Drawing.Point(120, 134);
            this.comboBoxLocation.Name = "comboBoxLocation";
            this.comboBoxLocation.Size = new System.Drawing.Size(112, 20);
            this.comboBoxLocation.TabIndex = 1;
            this.comboBoxLocation.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBoxLocation_KeyDown);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(674, 468);
            this.Controls.Add(this.checkBoxOneByOne);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.btnMakeMessage);
            this.Controls.Add(this.btnTrans);
            this.Controls.Add(this.textBoxCaller);
            this.Controls.Add(this.textBoxPhoneNumber);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.comboBoxLocation);
            this.Controls.Add(this.comboBoxActionSubStep);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.comboBoxActionStep);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxDisasterSubCategory);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxDisasterCategory);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "S-Protocol Simulator";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxDisasterCategory;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxDisasterSubCategory;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxActionStep;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioOver80;
        private System.Windows.Forms.RadioButton radioLess80;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioMission;
        private System.Windows.Forms.RadioButton radioEndSOP;
        private System.Windows.Forms.RadioButton radioInternal;
        private System.Windows.Forms.RadioButton radioBeginSOP;
        private System.Windows.Forms.RadioButton radioNormal;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.MaskedTextBox textBoxPhoneNumber;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnTrans;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxActionSubStep;
        private System.Windows.Forms.Button btnMakeMessage;
        private System.Windows.Forms.CheckBox checkBoxUseItem;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.CheckBox checkBoxOneByOne;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.MaskedTextBox textBoxCaller;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBoxLocation;
    }
}

