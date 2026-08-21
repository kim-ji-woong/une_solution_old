namespace UnityTester
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnRunUnity = new System.Windows.Forms.Button();
            this.cboScenes = new System.Windows.Forms.ComboBox();
            this.btnChangeScene = new System.Windows.Forms.Button();
            this.cboAlarmZones = new System.Windows.Forms.ComboBox();
            this.btnShowAlarmZone = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxImagePath = new System.Windows.Forms.TextBox();
            this.btnCaptureAlarmZone = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.checkBoxEditMode = new System.Windows.Forms.CheckBox();
            this.btnShowSensorList = new System.Windows.Forms.Button();
            this.labelEquipZoneID = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnRunUnity
            // 
            this.btnRunUnity.Location = new System.Drawing.Point(60, 30);
            this.btnRunUnity.Name = "btnRunUnity";
            this.btnRunUnity.Size = new System.Drawing.Size(75, 23);
            this.btnRunUnity.TabIndex = 0;
            this.btnRunUnity.Text = "Unity";
            this.btnRunUnity.UseVisualStyleBackColor = true;
            this.btnRunUnity.Click += new System.EventHandler(this.btnRunUnity_Click);
            // 
            // cboScenes
            // 
            this.cboScenes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboScenes.FormattingEnabled = true;
            this.cboScenes.Items.AddRange(new object[] {
            "외부모델 보기"});
            this.cboScenes.Location = new System.Drawing.Point(59, 111);
            this.cboScenes.Name = "cboScenes";
            this.cboScenes.Size = new System.Drawing.Size(121, 20);
            this.cboScenes.TabIndex = 1;
            this.cboScenes.SelectedIndexChanged += new System.EventHandler(this.cboScenes_SelectedIndexChanged);
            // 
            // btnChangeScene
            // 
            this.btnChangeScene.Location = new System.Drawing.Point(203, 111);
            this.btnChangeScene.Name = "btnChangeScene";
            this.btnChangeScene.Size = new System.Drawing.Size(75, 23);
            this.btnChangeScene.TabIndex = 2;
            this.btnChangeScene.Text = "화면전환";
            this.btnChangeScene.UseVisualStyleBackColor = true;
            this.btnChangeScene.Click += new System.EventHandler(this.btnChangeScene_Click);
            // 
            // cboAlarmZones
            // 
            this.cboAlarmZones.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAlarmZones.FormattingEnabled = true;
            this.cboAlarmZones.Location = new System.Drawing.Point(60, 175);
            this.cboAlarmZones.Name = "cboAlarmZones";
            this.cboAlarmZones.Size = new System.Drawing.Size(121, 20);
            this.cboAlarmZones.TabIndex = 3;
            this.cboAlarmZones.SelectedIndexChanged += new System.EventHandler(this.cboAlarmZones_SelectedIndexChanged);
            // 
            // btnShowAlarmZone
            // 
            this.btnShowAlarmZone.Location = new System.Drawing.Point(203, 175);
            this.btnShowAlarmZone.Name = "btnShowAlarmZone";
            this.btnShowAlarmZone.Size = new System.Drawing.Size(75, 23);
            this.btnShowAlarmZone.TabIndex = 2;
            this.btnShowAlarmZone.Text = "알람발생";
            this.btnShowAlarmZone.UseVisualStyleBackColor = true;
            this.btnShowAlarmZone.Click += new System.EventHandler(this.btnShowAlarmZone_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(58, 246);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "이미지 저장 폴더 :";
            // 
            // textBoxImagePath
            // 
            this.textBoxImagePath.Location = new System.Drawing.Point(169, 243);
            this.textBoxImagePath.Name = "textBoxImagePath";
            this.textBoxImagePath.Size = new System.Drawing.Size(219, 21);
            this.textBoxImagePath.TabIndex = 5;
            // 
            // btnCaptureAlarmZone
            // 
            this.btnCaptureAlarmZone.Location = new System.Drawing.Point(60, 288);
            this.btnCaptureAlarmZone.Name = "btnCaptureAlarmZone";
            this.btnCaptureAlarmZone.Size = new System.Drawing.Size(103, 23);
            this.btnCaptureAlarmZone.TabIndex = 6;
            this.btnCaptureAlarmZone.Text = "알람화면 저장";
            this.btnCaptureAlarmZone.UseVisualStyleBackColor = true;
            this.btnCaptureAlarmZone.Click += new System.EventHandler(this.btnCaptureAlarmZone_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(201, 296);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(57, 12);
            this.labelStatus.TabIndex = 10;
            this.labelStatus.Text = "저장 상태";
            this.labelStatus.Visible = false;
            // 
            // checkBoxEditMode
            // 
            this.checkBoxEditMode.AutoSize = true;
            this.checkBoxEditMode.Location = new System.Drawing.Point(295, 115);
            this.checkBoxEditMode.Name = "checkBoxEditMode";
            this.checkBoxEditMode.Size = new System.Drawing.Size(72, 16);
            this.checkBoxEditMode.TabIndex = 11;
            this.checkBoxEditMode.Text = "편집모드";
            this.checkBoxEditMode.UseVisualStyleBackColor = true;
            this.checkBoxEditMode.CheckedChanged += new System.EventHandler(this.checkBoxEditMode_CheckedChanged);
            // 
            // btnShowSensorList
            // 
            this.btnShowSensorList.Enabled = false;
            this.btnShowSensorList.Location = new System.Drawing.Point(432, 109);
            this.btnShowSensorList.Name = "btnShowSensorList";
            this.btnShowSensorList.Size = new System.Drawing.Size(75, 23);
            this.btnShowSensorList.TabIndex = 12;
            this.btnShowSensorList.Text = "센서목록";
            this.btnShowSensorList.UseVisualStyleBackColor = true;
            this.btnShowSensorList.Click += new System.EventHandler(this.btnShowSensorList_Click);
            // 
            // label2EquipZoneID
            // 
            this.labelEquipZoneID.AutoSize = true;
            this.labelEquipZoneID.Location = new System.Drawing.Point(7, 179);
            this.labelEquipZoneID.Name = "label2EquipZoneID";
            this.labelEquipZoneID.Size = new System.Drawing.Size(53, 12);
            this.labelEquipZoneID.TabIndex = 13;
            this.labelEquipZoneID.Text = "구역번호";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.labelEquipZoneID);
            this.Controls.Add(this.btnShowSensorList);
            this.Controls.Add(this.checkBoxEditMode);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.btnCaptureAlarmZone);
            this.Controls.Add(this.textBoxImagePath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboAlarmZones);
            this.Controls.Add(this.btnShowAlarmZone);
            this.Controls.Add(this.btnChangeScene);
            this.Controls.Add(this.cboScenes);
            this.Controls.Add(this.btnRunUnity);
            this.Name = "FormMain";
            this.Text = "FormMain";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRunUnity;
        private System.Windows.Forms.ComboBox cboScenes;
        private System.Windows.Forms.Button btnChangeScene;
        private System.Windows.Forms.ComboBox cboAlarmZones;
        private System.Windows.Forms.Button btnShowAlarmZone;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxImagePath;
        private System.Windows.Forms.Button btnCaptureAlarmZone;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.CheckBox checkBoxEditMode;
        private System.Windows.Forms.Button btnShowSensorList;
        private System.Windows.Forms.Label labelEquipZoneID;
    }
}

