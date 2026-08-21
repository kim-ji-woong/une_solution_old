namespace HSMS
{
    partial class FormDangerZone
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnAddGroup = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkLevelAll = new System.Windows.Forms.CheckBox();
            this.chkLevel4 = new System.Windows.Forms.CheckBox();
            this.chkLevel2 = new System.Windows.Forms.CheckBox();
            this.chkLevel5 = new System.Windows.Forms.CheckBox();
            this.chkLevel3 = new System.Windows.Forms.CheckBox();
            this.chkLevel1 = new System.Windows.Forms.CheckBox();
            this.cmbGroupList = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbZoneList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(450, 47);
            this.panel1.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(20, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 21);
            this.label3.TabIndex = 1;
            this.label3.Text = "위험영역 설정";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.btnAddGroup);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Controls.Add(this.cmbGroupList);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.cmbZoneList);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(12, 68);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(450, 343);
            this.panel2.TabIndex = 5;
            // 
            // btnAddGroup
            // 
            this.btnAddGroup.Location = new System.Drawing.Point(311, 51);
            this.btnAddGroup.Name = "btnAddGroup";
            this.btnAddGroup.Size = new System.Drawing.Size(66, 23);
            this.btnAddGroup.TabIndex = 9;
            this.btnAddGroup.Text = "그룹 추가";
            this.btnAddGroup.UseVisualStyleBackColor = true;
            this.btnAddGroup.Click += new System.EventHandler(this.btnAddGroup_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkLevelAll);
            this.groupBox1.Controls.Add(this.chkLevel4);
            this.groupBox1.Controls.Add(this.chkLevel2);
            this.groupBox1.Controls.Add(this.chkLevel5);
            this.groupBox1.Controls.Add(this.chkLevel3);
            this.groupBox1.Controls.Add(this.chkLevel1);
            this.groupBox1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox1.Location = new System.Drawing.Point(29, 96);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(394, 192);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "출입 등급 지정";
            // 
            // chkLevelAll
            // 
            this.chkLevelAll.AutoSize = true;
            this.chkLevelAll.Location = new System.Drawing.Point(221, 139);
            this.chkLevelAll.Name = "chkLevelAll";
            this.chkLevelAll.Size = new System.Drawing.Size(78, 19);
            this.chkLevelAll.TabIndex = 5;
            this.chkLevelAll.Text = "모두 허용";
            this.chkLevelAll.UseVisualStyleBackColor = true;
            this.chkLevelAll.CheckedChanged += new System.EventHandler(this.chkLevelAll_CheckedChanged_1);
            // 
            // chkLevel4
            // 
            this.chkLevel4.AutoSize = true;
            this.chkLevel4.Location = new System.Drawing.Point(221, 92);
            this.chkLevel4.Name = "chkLevel4";
            this.chkLevel4.Size = new System.Drawing.Size(89, 19);
            this.chkLevel4.TabIndex = 4;
            this.chkLevel4.Text = "4 레벨 허용";
            this.chkLevel4.UseVisualStyleBackColor = true;
            this.chkLevel4.CheckedChanged += new System.EventHandler(this.chkLevel4_CheckedChanged);
            // 
            // chkLevel2
            // 
            this.chkLevel2.AutoSize = true;
            this.chkLevel2.Location = new System.Drawing.Point(221, 45);
            this.chkLevel2.Name = "chkLevel2";
            this.chkLevel2.Size = new System.Drawing.Size(89, 19);
            this.chkLevel2.TabIndex = 3;
            this.chkLevel2.Text = "2 레벨 허용";
            this.chkLevel2.UseVisualStyleBackColor = true;
            this.chkLevel2.CheckedChanged += new System.EventHandler(this.chkLevel2_CheckedChanged);
            // 
            // chkLevel5
            // 
            this.chkLevel5.AutoSize = true;
            this.chkLevel5.Location = new System.Drawing.Point(51, 139);
            this.chkLevel5.Name = "chkLevel5";
            this.chkLevel5.Size = new System.Drawing.Size(89, 19);
            this.chkLevel5.TabIndex = 2;
            this.chkLevel5.Text = "5 레벨 허용";
            this.chkLevel5.UseVisualStyleBackColor = true;
            this.chkLevel5.CheckedChanged += new System.EventHandler(this.chkLevel5_CheckedChanged);
            // 
            // chkLevel3
            // 
            this.chkLevel3.AutoSize = true;
            this.chkLevel3.Location = new System.Drawing.Point(51, 92);
            this.chkLevel3.Name = "chkLevel3";
            this.chkLevel3.Size = new System.Drawing.Size(89, 19);
            this.chkLevel3.TabIndex = 1;
            this.chkLevel3.Text = "3 레벨 허용";
            this.chkLevel3.UseVisualStyleBackColor = true;
            this.chkLevel3.CheckedChanged += new System.EventHandler(this.chkLevel3_CheckedChanged);
            // 
            // chkLevel1
            // 
            this.chkLevel1.AutoSize = true;
            this.chkLevel1.Location = new System.Drawing.Point(51, 45);
            this.chkLevel1.Name = "chkLevel1";
            this.chkLevel1.Size = new System.Drawing.Size(89, 19);
            this.chkLevel1.TabIndex = 0;
            this.chkLevel1.Text = "1 레벨 허용";
            this.chkLevel1.UseVisualStyleBackColor = true;
            this.chkLevel1.CheckedChanged += new System.EventHandler(this.chkLevel1_CheckedChanged);
            // 
            // cmbGroupList
            // 
            this.cmbGroupList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGroupList.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbGroupList.FormattingEnabled = true;
            this.cmbGroupList.Location = new System.Drawing.Point(117, 51);
            this.cmbGroupList.Name = "cmbGroupList";
            this.cmbGroupList.Size = new System.Drawing.Size(187, 23);
            this.cmbGroupList.TabIndex = 7;
            this.cmbGroupList.SelectedIndexChanged += new System.EventHandler(this.cmbGroupList_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(46, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "영역 그룹";
            // 
            // cmbZoneList
            // 
            this.cmbZoneList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbZoneList.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cmbZoneList.FormattingEnabled = true;
            this.cmbZoneList.Location = new System.Drawing.Point(117, 21);
            this.cmbZoneList.Name = "cmbZoneList";
            this.cmbZoneList.Size = new System.Drawing.Size(187, 23);
            this.cmbZoneList.TabIndex = 7;
            this.cmbZoneList.SelectedIndexChanged += new System.EventHandler(this.cmbZoneList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(46, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "작업 영역";
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.Location = new System.Drawing.Point(346, 428);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(105, 31);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.White;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.Location = new System.Drawing.Point(235, 428);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(105, 31);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "저장";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // FormDangerZone
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(476, 478);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormDangerZone";
            this.Text = "FormDangerZone";
            this.Load += new System.EventHandler(this.FormDangerZone_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkLevelAll;
        private System.Windows.Forms.CheckBox chkLevel4;
        private System.Windows.Forms.CheckBox chkLevel2;
        private System.Windows.Forms.CheckBox chkLevel5;
        private System.Windows.Forms.CheckBox chkLevel3;
        private System.Windows.Forms.CheckBox chkLevel1;
        private System.Windows.Forms.ComboBox cmbZoneList;
        private System.Windows.Forms.Button btnAddGroup;
        private System.Windows.Forms.ComboBox cmbGroupList;
        private System.Windows.Forms.Label label2;
    }
}