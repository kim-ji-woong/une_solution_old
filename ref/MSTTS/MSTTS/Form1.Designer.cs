namespace TTS
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
            this.textBoxContents = new System.Windows.Forms.TextBox();
            this.btnRead = new System.Windows.Forms.Button();
            this.tbSpeed = new System.Windows.Forms.TrackBar();
            this.labelSpeed = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxFolder = new System.Windows.Forms.TextBox();
            this.btnSelectFolder = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioNumbering = new System.Windows.Forms.RadioButton();
            this.radioFileOverwrite = new System.Windows.Forms.RadioButton();
            this.btnSaveFile = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.tbSpeed)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxContents
            // 
            this.textBoxContents.Location = new System.Drawing.Point(12, 12);
            this.textBoxContents.Multiline = true;
            this.textBoxContents.Name = "textBoxContents";
            this.textBoxContents.Size = new System.Drawing.Size(260, 208);
            this.textBoxContents.TabIndex = 0;
            this.textBoxContents.Text = "관리사무소에서 안내말씀 드립니다. 내일은 관리비 납부 마감일입니다. 입주민 여러분께서는 관리비 납부 고지서를 확인하시고, 내일을 넘기지 않도록 주" +
    "의하시기 바랍니다.";
            // 
            // btnRead
            // 
            this.btnRead.Location = new System.Drawing.Point(380, 197);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(56, 23);
            this.btnRead.TabIndex = 1;
            this.btnRead.Text = "읽기";
            this.btnRead.UseVisualStyleBackColor = true;
            this.btnRead.Click += new System.EventHandler(this.btnRead_Click);
            // 
            // tbSpeed
            // 
            this.tbSpeed.Location = new System.Drawing.Point(340, 142);
            this.tbSpeed.Name = "tbSpeed";
            this.tbSpeed.Size = new System.Drawing.Size(104, 45);
            this.tbSpeed.TabIndex = 2;
            this.tbSpeed.Scroll += new System.EventHandler(this.tbSpeed_Scroll);
            // 
            // labelSpeed
            // 
            this.labelSpeed.AutoSize = true;
            this.labelSpeed.Location = new System.Drawing.Point(291, 147);
            this.labelSpeed.Name = "labelSpeed";
            this.labelSpeed.Size = new System.Drawing.Size(29, 12);
            this.labelSpeed.TabIndex = 3;
            this.labelSpeed.Text = "속도";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(291, 94);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "저장 폴더";
            // 
            // textBoxFolder
            // 
            this.textBoxFolder.Location = new System.Drawing.Point(290, 109);
            this.textBoxFolder.Name = "textBoxFolder";
            this.textBoxFolder.Size = new System.Drawing.Size(168, 21);
            this.textBoxFolder.TabIndex = 5;
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.Location = new System.Drawing.Point(464, 108);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(32, 23);
            this.btnSelectFolder.TabIndex = 6;
            this.btnSelectFolder.Text = "...";
            this.btnSelectFolder.UseVisualStyleBackColor = true;
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioNumbering);
            this.groupBox1.Controls.Add(this.radioFileOverwrite);
            this.groupBox1.Location = new System.Drawing.Point(290, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(206, 73);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "저장 옵션";
            // 
            // radioNumbering
            // 
            this.radioNumbering.AutoSize = true;
            this.radioNumbering.Location = new System.Drawing.Point(6, 42);
            this.radioNumbering.Name = "radioNumbering";
            this.radioNumbering.Size = new System.Drawing.Size(163, 16);
            this.radioNumbering.TabIndex = 0;
            this.radioNumbering.TabStop = true;
            this.radioNumbering.Text = "번호매김하여 파일 만들기";
            this.radioNumbering.UseVisualStyleBackColor = true;
            // 
            // radioFileOverwrite
            // 
            this.radioFileOverwrite.AutoSize = true;
            this.radioFileOverwrite.Checked = true;
            this.radioFileOverwrite.Location = new System.Drawing.Point(6, 20);
            this.radioFileOverwrite.Name = "radioFileOverwrite";
            this.radioFileOverwrite.Size = new System.Drawing.Size(99, 16);
            this.radioFileOverwrite.TabIndex = 0;
            this.radioFileOverwrite.TabStop = true;
            this.radioFileOverwrite.Text = "파일 덮어쓰기";
            this.radioFileOverwrite.UseVisualStyleBackColor = true;
            // 
            // btnSaveFile
            // 
            this.btnSaveFile.Location = new System.Drawing.Point(442, 197);
            this.btnSaveFile.Name = "btnSaveFile";
            this.btnSaveFile.Size = new System.Drawing.Size(56, 23);
            this.btnSaveFile.TabIndex = 1;
            this.btnSaveFile.Text = "저장";
            this.btnSaveFile.UseVisualStyleBackColor = true;
            this.btnSaveFile.Click += new System.EventHandler(this.btnSaveFile_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(318, 197);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(56, 23);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "중지";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 235);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSelectFolder);
            this.Controls.Add(this.textBoxFolder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelSpeed);
            this.Controls.Add(this.tbSpeed);
            this.Controls.Add(this.btnSaveFile);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnRead);
            this.Controls.Add(this.textBoxContents);
            this.Name = "Form1";
            this.Text = "Text to Speech";
            ((System.ComponentModel.ISupportInitialize)(this.tbSpeed)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxContents;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.TrackBar tbSpeed;
        private System.Windows.Forms.Label labelSpeed;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxFolder;
        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioNumbering;
        private System.Windows.Forms.RadioButton radioFileOverwrite;
        private System.Windows.Forms.Button btnSaveFile;
        private System.Windows.Forms.Button btnStop;
    }
}

