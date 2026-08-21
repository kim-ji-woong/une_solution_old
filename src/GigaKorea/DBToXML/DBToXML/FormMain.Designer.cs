namespace DBToXML
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
            this.label1 = new System.Windows.Forms.Label();
            this.cboProjects = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxXMLPath = new System.Windows.Forms.TextBox();
            this.btnSavePath = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.checkBoxSameAsProject = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Project     :";
            // 
            // cboProjects
            // 
            this.cboProjects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProjects.FormattingEnabled = true;
            this.cboProjects.Location = new System.Drawing.Point(94, 19);
            this.cboProjects.Name = "cboProjects";
            this.cboProjects.Size = new System.Drawing.Size(261, 20);
            this.cboProjects.TabIndex = 1;
            this.cboProjects.SelectedIndexChanged += new System.EventHandler(this.cboProjects_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "XML 경로 :";
            // 
            // textBoxXMLPath
            // 
            this.textBoxXMLPath.Location = new System.Drawing.Point(94, 49);
            this.textBoxXMLPath.Name = "textBoxXMLPath";
            this.textBoxXMLPath.Size = new System.Drawing.Size(229, 21);
            this.textBoxXMLPath.TabIndex = 2;
            // 
            // btnSavePath
            // 
            this.btnSavePath.Location = new System.Drawing.Point(329, 48);
            this.btnSavePath.Name = "btnSavePath";
            this.btnSavePath.Size = new System.Drawing.Size(27, 23);
            this.btnSavePath.TabIndex = 3;
            this.btnSavePath.Text = "...";
            this.btnSavePath.UseVisualStyleBackColor = true;
            this.btnSavePath.Click += new System.EventHandler(this.btnSavePath_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(293, 77);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(62, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "변환하기";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // checkBoxSameAsProject
            // 
            this.checkBoxSameAsProject.AutoSize = true;
            this.checkBoxSameAsProject.Location = new System.Drawing.Point(94, 81);
            this.checkBoxSameAsProject.Name = "checkBoxSameAsProject";
            this.checkBoxSameAsProject.Size = new System.Drawing.Size(164, 16);
            this.checkBoxSameAsProject.TabIndex = 4;
            this.checkBoxSameAsProject.Text = "프로젝트 이름과 동일하게";
            this.checkBoxSameAsProject.UseVisualStyleBackColor = true;
            this.checkBoxSameAsProject.CheckedChanged += new System.EventHandler(this.checkBoxSameAsProject_CheckedChanged);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 106);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.checkBoxSameAsProject);
            this.Controls.Add(this.btnSavePath);
            this.Controls.Add(this.textBoxXMLPath);
            this.Controls.Add(this.cboProjects);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FormMain";
            this.Text = "DB To XML 변환기";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboProjects;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxXMLPath;
        private System.Windows.Forms.Button btnSavePath;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox checkBoxSameAsProject;
    }
}

