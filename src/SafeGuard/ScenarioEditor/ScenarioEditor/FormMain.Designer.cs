namespace ScenarioEditor
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxOriginXML = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxSaveXML = new System.Windows.Forms.TextBox();
            this.btnLoadOrigin = new System.Windows.Forms.Button();
            this.btnLoadSave = new System.Windows.Forms.Button();
            this.radioDay = new System.Windows.Forms.RadioButton();
            this.radioNight = new System.Windows.Forms.RadioButton();
            this.checkBoxDisaster = new System.Windows.Forms.CheckBox();
            this.checkBoxSpread = new System.Windows.Forms.CheckBox();
            this.checkBoxTrans = new System.Windows.Forms.CheckBox();
            this.checkBoxControl = new System.Windows.Forms.CheckBox();
            this.checkBoxInitial = new System.Windows.Forms.CheckBox();
            this.checkBoxEvacuation = new System.Windows.Forms.CheckBox();
            this.checkBoxCommit = new System.Windows.Forms.CheckBox();
            this.checkBoxSuppress = new System.Windows.Forms.CheckBox();
            this.checkBoxRescue = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.checkBoxVariable = new System.Windows.Forms.CheckBox();
            this.checkBoxVariable2 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "원본 파일";
            // 
            // textBoxOriginXML
            // 
            this.textBoxOriginXML.Location = new System.Drawing.Point(75, 34);
            this.textBoxOriginXML.Name = "textBoxOriginXML";
            this.textBoxOriginXML.ReadOnly = true;
            this.textBoxOriginXML.Size = new System.Drawing.Size(283, 21);
            this.textBoxOriginXML.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "저장 파일";
            // 
            // textBoxSaveXML
            // 
            this.textBoxSaveXML.Location = new System.Drawing.Point(75, 73);
            this.textBoxSaveXML.Name = "textBoxSaveXML";
            this.textBoxSaveXML.Size = new System.Drawing.Size(283, 21);
            this.textBoxSaveXML.TabIndex = 1;
            // 
            // btnLoadOrigin
            // 
            this.btnLoadOrigin.Location = new System.Drawing.Point(367, 33);
            this.btnLoadOrigin.Name = "btnLoadOrigin";
            this.btnLoadOrigin.Size = new System.Drawing.Size(25, 23);
            this.btnLoadOrigin.TabIndex = 2;
            this.btnLoadOrigin.Text = "...";
            this.btnLoadOrigin.UseVisualStyleBackColor = true;
            this.btnLoadOrigin.Click += new System.EventHandler(this.btnLoadOrigin_Click);
            // 
            // btnLoadSave
            // 
            this.btnLoadSave.Location = new System.Drawing.Point(367, 72);
            this.btnLoadSave.Name = "btnLoadSave";
            this.btnLoadSave.Size = new System.Drawing.Size(25, 23);
            this.btnLoadSave.TabIndex = 2;
            this.btnLoadSave.Text = "...";
            this.btnLoadSave.UseVisualStyleBackColor = true;
            this.btnLoadSave.Click += new System.EventHandler(this.btnLoadSave_Click);
            // 
            // radioDay
            // 
            this.radioDay.AutoSize = true;
            this.radioDay.Checked = true;
            this.radioDay.Location = new System.Drawing.Point(14, 116);
            this.radioDay.Name = "radioDay";
            this.radioDay.Size = new System.Drawing.Size(47, 16);
            this.radioDay.TabIndex = 3;
            this.radioDay.TabStop = true;
            this.radioDay.Text = "주간";
            this.radioDay.UseVisualStyleBackColor = true;
            // 
            // radioNight
            // 
            this.radioNight.AutoSize = true;
            this.radioNight.Location = new System.Drawing.Point(97, 116);
            this.radioNight.Name = "radioNight";
            this.radioNight.Size = new System.Drawing.Size(47, 16);
            this.radioNight.TabIndex = 3;
            this.radioNight.TabStop = true;
            this.radioNight.Text = "야간";
            this.radioNight.UseVisualStyleBackColor = true;
            // 
            // checkBoxDisaster
            // 
            this.checkBoxDisaster.AutoSize = true;
            this.checkBoxDisaster.Location = new System.Drawing.Point(12, 166);
            this.checkBoxDisaster.Name = "checkBoxDisaster";
            this.checkBoxDisaster.Size = new System.Drawing.Size(88, 16);
            this.checkBoxDisaster.TabIndex = 4;
            this.checkBoxDisaster.Text = "재난 발생기";
            this.checkBoxDisaster.UseVisualStyleBackColor = true;
            // 
            // checkBoxSpread
            // 
            this.checkBoxSpread.AutoSize = true;
            this.checkBoxSpread.Location = new System.Drawing.Point(142, 166);
            this.checkBoxSpread.Name = "checkBoxSpread";
            this.checkBoxSpread.Size = new System.Drawing.Size(84, 16);
            this.checkBoxSpread.TabIndex = 4;
            this.checkBoxSpread.Text = "재난확대기";
            this.checkBoxSpread.UseVisualStyleBackColor = true;
            // 
            // checkBoxTrans
            // 
            this.checkBoxTrans.AutoSize = true;
            this.checkBoxTrans.Location = new System.Drawing.Point(270, 166);
            this.checkBoxTrans.Name = "checkBoxTrans";
            this.checkBoxTrans.Size = new System.Drawing.Size(140, 16);
            this.checkBoxTrans.TabIndex = 4;
            this.checkBoxTrans.Text = "신고접수 및 상황전파";
            this.checkBoxTrans.UseVisualStyleBackColor = true;
            // 
            // checkBoxControl
            // 
            this.checkBoxControl.AutoSize = true;
            this.checkBoxControl.Location = new System.Drawing.Point(12, 206);
            this.checkBoxControl.Name = "checkBoxControl";
            this.checkBoxControl.Size = new System.Drawing.Size(100, 16);
            this.checkBoxControl.TabIndex = 4;
            this.checkBoxControl.Text = "지휘체계 확립";
            this.checkBoxControl.UseVisualStyleBackColor = true;
            // 
            // checkBoxInitial
            // 
            this.checkBoxInitial.AutoSize = true;
            this.checkBoxInitial.Location = new System.Drawing.Point(142, 206);
            this.checkBoxInitial.Name = "checkBoxInitial";
            this.checkBoxInitial.Size = new System.Drawing.Size(76, 16);
            this.checkBoxInitial.TabIndex = 4;
            this.checkBoxInitial.Text = "초기 대응";
            this.checkBoxInitial.UseVisualStyleBackColor = true;
            // 
            // checkBoxEvacuation
            // 
            this.checkBoxEvacuation.AutoSize = true;
            this.checkBoxEvacuation.Location = new System.Drawing.Point(270, 206);
            this.checkBoxEvacuation.Name = "checkBoxEvacuation";
            this.checkBoxEvacuation.Size = new System.Drawing.Size(72, 16);
            this.checkBoxEvacuation.TabIndex = 4;
            this.checkBoxEvacuation.Text = "주민대피";
            this.checkBoxEvacuation.UseVisualStyleBackColor = true;
            // 
            // checkBoxCommit
            // 
            this.checkBoxCommit.AutoSize = true;
            this.checkBoxCommit.Location = new System.Drawing.Point(12, 245);
            this.checkBoxCommit.Name = "checkBoxCommit";
            this.checkBoxCommit.Size = new System.Drawing.Size(72, 16);
            this.checkBoxCommit.TabIndex = 4;
            this.checkBoxCommit.Text = "현장투입";
            this.checkBoxCommit.UseVisualStyleBackColor = true;
            // 
            // checkBoxSuppress
            // 
            this.checkBoxSuppress.AutoSize = true;
            this.checkBoxSuppress.Location = new System.Drawing.Point(142, 245);
            this.checkBoxSuppress.Name = "checkBoxSuppress";
            this.checkBoxSuppress.Size = new System.Drawing.Size(76, 16);
            this.checkBoxSuppress.TabIndex = 4;
            this.checkBoxSuppress.Text = "사고 진압";
            this.checkBoxSuppress.UseVisualStyleBackColor = true;
            // 
            // checkBoxRescue
            // 
            this.checkBoxRescue.AutoSize = true;
            this.checkBoxRescue.Location = new System.Drawing.Point(270, 245);
            this.checkBoxRescue.Name = "checkBoxRescue";
            this.checkBoxRescue.Size = new System.Drawing.Size(92, 16);
            this.checkBoxRescue.TabIndex = 4;
            this.checkBoxRescue.Text = "진압 및 구조";
            this.checkBoxRescue.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(317, 303);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "저장";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // checkBoxVariable
            // 
            this.checkBoxVariable.AutoSize = true;
            this.checkBoxVariable.Location = new System.Drawing.Point(12, 282);
            this.checkBoxVariable.Name = "checkBoxVariable";
            this.checkBoxVariable.Size = new System.Drawing.Size(72, 16);
            this.checkBoxVariable.TabIndex = 6;
            this.checkBoxVariable.Text = "공통변수";
            this.checkBoxVariable.UseVisualStyleBackColor = true;
            // 
            // checkBoxVariable2
            // 
            this.checkBoxVariable2.AutoSize = true;
            this.checkBoxVariable2.Location = new System.Drawing.Point(142, 282);
            this.checkBoxVariable2.Name = "checkBoxVariable2";
            this.checkBoxVariable2.Size = new System.Drawing.Size(78, 16);
            this.checkBoxVariable2.TabIndex = 6;
            this.checkBoxVariable2.Text = "공통변수2";
            this.checkBoxVariable2.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 348);
            this.Controls.Add(this.checkBoxVariable2);
            this.Controls.Add(this.checkBoxVariable);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.checkBoxRescue);
            this.Controls.Add(this.checkBoxEvacuation);
            this.Controls.Add(this.checkBoxTrans);
            this.Controls.Add(this.checkBoxSuppress);
            this.Controls.Add(this.checkBoxCommit);
            this.Controls.Add(this.checkBoxInitial);
            this.Controls.Add(this.checkBoxControl);
            this.Controls.Add(this.checkBoxSpread);
            this.Controls.Add(this.checkBoxDisaster);
            this.Controls.Add(this.radioNight);
            this.Controls.Add(this.radioDay);
            this.Controls.Add(this.btnLoadSave);
            this.Controls.Add(this.btnLoadOrigin);
            this.Controls.Add(this.textBoxSaveXML);
            this.Controls.Add(this.textBoxOriginXML);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FormMain";
            this.Text = "시나리오 편집기";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxOriginXML;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxSaveXML;
        private System.Windows.Forms.Button btnLoadOrigin;
        private System.Windows.Forms.Button btnLoadSave;
        private System.Windows.Forms.RadioButton radioDay;
        private System.Windows.Forms.RadioButton radioNight;
        private System.Windows.Forms.CheckBox checkBoxDisaster;
        private System.Windows.Forms.CheckBox checkBoxSpread;
        private System.Windows.Forms.CheckBox checkBoxTrans;
        private System.Windows.Forms.CheckBox checkBoxControl;
        private System.Windows.Forms.CheckBox checkBoxInitial;
        private System.Windows.Forms.CheckBox checkBoxEvacuation;
        private System.Windows.Forms.CheckBox checkBoxCommit;
        private System.Windows.Forms.CheckBox checkBoxSuppress;
        private System.Windows.Forms.CheckBox checkBoxRescue;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox checkBoxVariable;
        private System.Windows.Forms.CheckBox checkBoxVariable2;
    }
}

