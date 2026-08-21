namespace WindowsFormsApplication14
{
    partial class MainForm
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
            this.btnSearchFile = new System.Windows.Forms.Button();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.ckbSaveFile = new System.Windows.Forms.CheckBox();
            this.txtSQLDB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSQLServer = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSQLID = new System.Windows.Forms.TextBox();
            this.txtSQLPass = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnConTest = new System.Windows.Forms.Button();
            this.btnSaveDir = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.ckbMySQL = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnSearchFile
            // 
            this.btnSearchFile.Location = new System.Drawing.Point(317, 26);
            this.btnSearchFile.Name = "btnSearchFile";
            this.btnSearchFile.Size = new System.Drawing.Size(71, 28);
            this.btnSearchFile.TabIndex = 0;
            this.btnSearchFile.Text = "파일";
            this.btnSearchFile.UseVisualStyleBackColor = true;
            this.btnSearchFile.Click += new System.EventHandler(this.btnSearchFile_Click);
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(38, 31);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(273, 21);
            this.txtFileName.TabIndex = 1;
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(317, 228);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(71, 33);
            this.btnRun.TabIndex = 2;
            this.btnRun.Text = "실행하기";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // ckbSaveFile
            // 
            this.ckbSaveFile.AutoSize = true;
            this.ckbSaveFile.Location = new System.Drawing.Point(38, 237);
            this.ckbSaveFile.Name = "ckbSaveFile";
            this.ckbSaveFile.Size = new System.Drawing.Size(112, 16);
            this.ckbSaveFile.TabIndex = 3;
            this.ckbSaveFile.Text = "분리한파일 저장";
            this.ckbSaveFile.UseVisualStyleBackColor = true;
            this.ckbSaveFile.CheckedChanged += new System.EventHandler(this.ckbSaveFile_CheckedChanged);
            // 
            // txtSQLDB
            // 
            this.txtSQLDB.Location = new System.Drawing.Point(148, 97);
            this.txtSQLDB.Name = "txtSQLDB";
            this.txtSQLDB.Size = new System.Drawing.Size(116, 21);
            this.txtSQLDB.TabIndex = 4;
            this.txtSQLDB.Text = "SOP_1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 100);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "대상 데이터베이스";
            // 
            // txtSQLServer
            // 
            this.txtSQLServer.Location = new System.Drawing.Point(148, 70);
            this.txtSQLServer.Name = "txtSQLServer";
            this.txtSQLServer.Size = new System.Drawing.Size(116, 21);
            this.txtSQLServer.TabIndex = 6;
            this.txtSQLServer.Text = "127.0.0.1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "대상 서버";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(36, 130);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "ID";
            // 
            // txtSQLID
            // 
            this.txtSQLID.Location = new System.Drawing.Point(148, 127);
            this.txtSQLID.Name = "txtSQLID";
            this.txtSQLID.Size = new System.Drawing.Size(116, 21);
            this.txtSQLID.TabIndex = 9;
            this.txtSQLID.Text = "sa";
            // 
            // txtSQLPass
            // 
            this.txtSQLPass.Location = new System.Drawing.Point(148, 157);
            this.txtSQLPass.Name = "txtSQLPass";
            this.txtSQLPass.Size = new System.Drawing.Size(116, 21);
            this.txtSQLPass.TabIndex = 10;
            this.txtSQLPass.Text = "9449966Ab";
            this.txtSQLPass.UseSystemPasswordChar = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 160);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "Password";
            // 
            // btnConTest
            // 
            this.btnConTest.Location = new System.Drawing.Point(302, 145);
            this.btnConTest.Name = "btnConTest";
            this.btnConTest.Size = new System.Drawing.Size(86, 33);
            this.btnConTest.TabIndex = 12;
            this.btnConTest.Text = "연결테스트";
            this.btnConTest.UseVisualStyleBackColor = true;
            this.btnConTest.Click += new System.EventHandler(this.btnConTest_Click);
            // 
            // btnSaveDir
            // 
            this.btnSaveDir.Location = new System.Drawing.Point(156, 233);
            this.btnSaveDir.Name = "btnSaveDir";
            this.btnSaveDir.Size = new System.Drawing.Size(72, 22);
            this.btnSaveDir.TabIndex = 13;
            this.btnSaveDir.Text = "저장위치";
            this.btnSaveDir.UseVisualStyleBackColor = true;
            this.btnSaveDir.Click += new System.EventHandler(this.btnSaveDir_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(36, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 12);
            this.label5.TabIndex = 14;
            this.label5.Text = "대용량 SQL파일";
            // 
            // ckbMySQL
            // 
            this.ckbMySQL.AutoSize = true;
            this.ckbMySQL.Location = new System.Drawing.Point(38, 206);
            this.ckbMySQL.Name = "ckbMySQL";
            this.ckbMySQL.Size = new System.Drawing.Size(94, 16);
            this.ckbMySQL.TabIndex = 15;
            this.ckbMySQL.Text = "MySQL 사용";
            this.ckbMySQL.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(416, 281);
            this.Controls.Add(this.ckbMySQL);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnSaveDir);
            this.Controls.Add(this.btnConTest);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtSQLPass);
            this.Controls.Add(this.txtSQLID);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSQLServer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSQLDB);
            this.Controls.Add(this.ckbSaveFile);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.txtFileName);
            this.Controls.Add(this.btnSearchFile);
            this.Name = "MainForm";
            this.Text = "대용량SQL 처리기";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSearchFile;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.CheckBox ckbSaveFile;
        private System.Windows.Forms.TextBox txtSQLDB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSQLServer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSQLID;
        private System.Windows.Forms.TextBox txtSQLPass;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnConTest;
        private System.Windows.Forms.Button btnSaveDir;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox ckbMySQL;
    }
}

