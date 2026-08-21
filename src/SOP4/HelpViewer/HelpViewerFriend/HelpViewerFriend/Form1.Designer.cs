namespace HelpViewerFriend
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
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.btnReceiveList = new System.Windows.Forms.Button();
            this.btnOpenPage = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.btnOpenPageFromID = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(12, 12);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(256, 382);
            this.treeView1.TabIndex = 0;
            // 
            // btnReceiveList
            // 
            this.btnReceiveList.Location = new System.Drawing.Point(339, 12);
            this.btnReceiveList.Name = "btnReceiveList";
            this.btnReceiveList.Size = new System.Drawing.Size(75, 23);
            this.btnReceiveList.TabIndex = 1;
            this.btnReceiveList.Text = "받아오기";
            this.btnReceiveList.UseVisualStyleBackColor = true;
            this.btnReceiveList.Click += new System.EventHandler(this.btnReceiveList_Click);
            // 
            // btnOpenPage
            // 
            this.btnOpenPage.Location = new System.Drawing.Point(339, 41);
            this.btnOpenPage.Name = "btnOpenPage";
            this.btnOpenPage.Size = new System.Drawing.Size(75, 23);
            this.btnOpenPage.TabIndex = 1;
            this.btnOpenPage.Text = "화면열기";
            this.btnOpenPage.UseVisualStyleBackColor = true;
            this.btnOpenPage.Click += new System.EventHandler(this.btnOpenPage_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(285, 182);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "ID :";
            // 
            // textBoxID
            // 
            this.textBoxID.Location = new System.Drawing.Point(314, 178);
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.Size = new System.Drawing.Size(100, 21);
            this.textBoxID.TabIndex = 3;
            // 
            // btnOpenPageFromID
            // 
            this.btnOpenPageFromID.Location = new System.Drawing.Point(314, 205);
            this.btnOpenPageFromID.Name = "btnOpenPageFromID";
            this.btnOpenPageFromID.Size = new System.Drawing.Size(100, 23);
            this.btnOpenPageFromID.TabIndex = 4;
            this.btnOpenPageFromID.Text = "ID로 화면열기";
            this.btnOpenPageFromID.UseVisualStyleBackColor = true;
            this.btnOpenPageFromID.Click += new System.EventHandler(this.btnOpenPageFromID_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 406);
            this.Controls.Add(this.btnOpenPageFromID);
            this.Controls.Add(this.textBoxID);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOpenPage);
            this.Controls.Add(this.btnReceiveList);
            this.Controls.Add(this.treeView1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button btnReceiveList;
        private System.Windows.Forms.Button btnOpenPage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxID;
        private System.Windows.Forms.Button btnOpenPageFromID;
    }
}

