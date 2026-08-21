namespace SamSMS
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("삼천포 화력");
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lableLength = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label13 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageToAll = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbToAll = new System.Windows.Forms.CheckBox();
            this.cbToExternal = new System.Windows.Forms.CheckBox();
            this.cbToTimeOff = new System.Windows.Forms.CheckBox();
            this.tabPageTeam = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.cbPos1 = new System.Windows.Forms.CheckBox();
            this.cbPos2 = new System.Windows.Forms.CheckBox();
            this.tabPageLevel = new System.Windows.Forms.TabPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbLevel4 = new System.Windows.Forms.CheckBox();
            this.cbLevel1 = new System.Windows.Forms.CheckBox();
            this.cbLevel3 = new System.Windows.Forms.CheckBox();
            this.cbLevel2 = new System.Windows.Forms.CheckBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lbLevelFour = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.lbLevelThree = new System.Windows.Forms.Label();
            this.lbLevelTwo = new System.Windows.Forms.Label();
            this.lbLevelOne = new System.Windows.Forms.Label();
            this.lbExternal = new System.Windows.Forms.Label();
            this.lbTimeOff = new System.Windows.Forms.Label();
            this.lbCompany = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPageToAll.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPageTeam.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPageLevel.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(557, 466);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 30);
            this.button1.TabIndex = 0;
            this.button1.Text = "전송";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnSendMessage_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(555, 416);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(92, 30);
            this.button2.TabIndex = 1;
            this.button2.Text = "초기화";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(0, 24);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(431, 106);
            this.textBox1.TabIndex = 11;
            this.textBox1.TextChanged += new System.EventHandler(this.tbMessageTextChanged);
            // 
            // lableLength
            // 
            this.lableLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lableLength.Location = new System.Drawing.Point(287, 133);
            this.lableLength.Name = "lableLength";
            this.lableLength.Size = new System.Drawing.Size(144, 18);
            this.lableLength.TabIndex = 13;
            this.lableLength.Text = "0/80 바이트";
            this.lableLength.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "uncheck.png");
            this.imageList1.Images.SetKeyName(1, "check.png");
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(12, 9);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(69, 12);
            this.label13.TabIndex = 14;
            this.label13.Text = "전송 메세지";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageToAll);
            this.tabControl1.Controls.Add(this.tabPageTeam);
            this.tabControl1.Controls.Add(this.tabPageLevel);
            this.tabControl1.Location = new System.Drawing.Point(0, 179);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(435, 347);
            this.tabControl1.TabIndex = 15;
            // 
            // tabPageToAll
            // 
            this.tabPageToAll.Controls.Add(this.panel1);
            this.tabPageToAll.Location = new System.Drawing.Point(4, 22);
            this.tabPageToAll.Name = "tabPageToAll";
            this.tabPageToAll.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageToAll.Size = new System.Drawing.Size(427, 321);
            this.tabPageToAll.TabIndex = 0;
            this.tabPageToAll.Text = "전체 임직원";
            this.tabPageToAll.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(421, 315);
            this.panel1.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbToAll);
            this.groupBox1.Controls.Add(this.cbToExternal);
            this.groupBox1.Controls.Add(this.cbToTimeOff);
            this.groupBox1.Location = new System.Drawing.Point(27, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(340, 69);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "전체 임직원 발송";
            // 
            // cbToAll
            // 
            this.cbToAll.AutoSize = true;
            this.cbToAll.Location = new System.Drawing.Point(15, 29);
            this.cbToAll.Name = "cbToAll";
            this.cbToAll.Size = new System.Drawing.Size(100, 16);
            this.cbToAll.TabIndex = 2;
            this.cbToAll.Text = "삼천포 임직원";
            this.cbToAll.UseVisualStyleBackColor = true;
            // 
            // cbToExternal
            // 
            this.cbToExternal.AutoSize = true;
            this.cbToExternal.Location = new System.Drawing.Point(222, 29);
            this.cbToExternal.Name = "cbToExternal";
            this.cbToExternal.Size = new System.Drawing.Size(72, 16);
            this.cbToExternal.TabIndex = 1;
            this.cbToExternal.Text = "외부업체";
            this.cbToExternal.UseVisualStyleBackColor = true;
            // 
            // cbToTimeOff
            // 
            this.cbToTimeOff.AutoSize = true;
            this.cbToTimeOff.Location = new System.Drawing.Point(132, 29);
            this.cbToTimeOff.Name = "cbToTimeOff";
            this.cbToTimeOff.Size = new System.Drawing.Size(60, 16);
            this.cbToTimeOff.TabIndex = 0;
            this.cbToTimeOff.Text = "휴직자";
            this.cbToTimeOff.UseVisualStyleBackColor = true;
            this.cbToTimeOff.Visible = false;
            // 
            // tabPageTeam
            // 
            this.tabPageTeam.Controls.Add(this.panel2);
            this.tabPageTeam.Location = new System.Drawing.Point(4, 22);
            this.tabPageTeam.Name = "tabPageTeam";
            this.tabPageTeam.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTeam.Size = new System.Drawing.Size(427, 321);
            this.tabPageTeam.TabIndex = 1;
            this.tabPageTeam.Text = "부서별";
            this.tabPageTeam.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(421, 315);
            this.panel2.TabIndex = 6;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.treeView1);
            this.groupBox2.Controls.Add(this.cbPos1);
            this.groupBox2.Controls.Add(this.cbPos2);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(412, 286);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "부서 별 발송";
            // 
            // treeView1
            // 
            this.treeView1.HideSelection = false;
            this.treeView1.Location = new System.Drawing.Point(6, 47);
            this.treeView1.Name = "treeView1";
            treeNode2.Name = "노드2";
            treeNode2.Text = "삼천포 화력";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2});
            this.treeView1.ShowLines = false;
            this.treeView1.Size = new System.Drawing.Size(400, 233);
            this.treeView1.StateImageList = this.imageList1;
            this.treeView1.TabIndex = 8;
            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck_1);
            // 
            // cbPos1
            // 
            this.cbPos1.AutoSize = true;
            this.cbPos1.Location = new System.Drawing.Point(65, 22);
            this.cbPos1.Name = "cbPos1";
            this.cbPos1.Size = new System.Drawing.Size(48, 16);
            this.cbPos1.TabIndex = 1;
            this.cbPos1.Text = "팀원";
            this.cbPos1.UseVisualStyleBackColor = true;
            // 
            // cbPos2
            // 
            this.cbPos2.AutoSize = true;
            this.cbPos2.Location = new System.Drawing.Point(155, 22);
            this.cbPos2.Name = "cbPos2";
            this.cbPos2.Size = new System.Drawing.Size(48, 16);
            this.cbPos2.TabIndex = 0;
            this.cbPos2.Text = "팀장";
            this.cbPos2.UseVisualStyleBackColor = true;
            // 
            // tabPageLevel
            // 
            this.tabPageLevel.Controls.Add(this.panel3);
            this.tabPageLevel.Location = new System.Drawing.Point(4, 22);
            this.tabPageLevel.Name = "tabPageLevel";
            this.tabPageLevel.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLevel.Size = new System.Drawing.Size(427, 321);
            this.tabPageLevel.TabIndex = 2;
            this.tabPageLevel.Text = "직급별";
            this.tabPageLevel.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.groupBox3);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(421, 315);
            this.panel3.TabIndex = 6;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbLevel4);
            this.groupBox3.Controls.Add(this.cbLevel1);
            this.groupBox3.Controls.Add(this.cbLevel3);
            this.groupBox3.Controls.Add(this.cbLevel2);
            this.groupBox3.Location = new System.Drawing.Point(38, 27);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(303, 69);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "직급 별 발송";
            // 
            // cbLevel4
            // 
            this.cbLevel4.AutoSize = true;
            this.cbLevel4.Location = new System.Drawing.Point(16, 29);
            this.cbLevel4.Name = "cbLevel4";
            this.cbLevel4.Size = new System.Drawing.Size(82, 16);
            this.cbLevel4.TabIndex = 3;
            this.cbLevel4.Text = "4 직급이하";
            this.cbLevel4.UseVisualStyleBackColor = true;
            // 
            // cbLevel1
            // 
            this.cbLevel1.AutoSize = true;
            this.cbLevel1.Location = new System.Drawing.Point(232, 29);
            this.cbLevel1.Name = "cbLevel1";
            this.cbLevel1.Size = new System.Drawing.Size(58, 16);
            this.cbLevel1.TabIndex = 2;
            this.cbLevel1.Text = "1 직급";
            this.cbLevel1.UseVisualStyleBackColor = true;
            // 
            // cbLevel3
            // 
            this.cbLevel3.AutoSize = true;
            this.cbLevel3.Location = new System.Drawing.Point(104, 29);
            this.cbLevel3.Name = "cbLevel3";
            this.cbLevel3.Size = new System.Drawing.Size(58, 16);
            this.cbLevel3.TabIndex = 1;
            this.cbLevel3.Text = "3 직급";
            this.cbLevel3.UseVisualStyleBackColor = true;
            // 
            // cbLevel2
            // 
            this.cbLevel2.AutoSize = true;
            this.cbLevel2.Location = new System.Drawing.Point(168, 29);
            this.cbLevel2.Name = "cbLevel2";
            this.cbLevel2.Size = new System.Drawing.Size(58, 16);
            this.cbLevel2.TabIndex = 0;
            this.cbLevel2.Text = "2 직급";
            this.cbLevel2.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.groupBox4);
            this.panel4.Location = new System.Drawing.Point(437, 12);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(210, 268);
            this.panel4.TabIndex = 16;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lbLevelFour);
            this.groupBox4.Controls.Add(this.label17);
            this.groupBox4.Controls.Add(this.lbLevelThree);
            this.groupBox4.Controls.Add(this.lbLevelTwo);
            this.groupBox4.Controls.Add(this.lbLevelOne);
            this.groupBox4.Controls.Add(this.lbExternal);
            this.groupBox4.Controls.Add(this.lbTimeOff);
            this.groupBox4.Controls.Add(this.lbCompany);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Location = new System.Drawing.Point(9, 10);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(194, 245);
            this.groupBox4.TabIndex = 18;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "삼천포 인원 현황";
            // 
            // lbLevelFour
            // 
            this.lbLevelFour.AutoSize = true;
            this.lbLevelFour.Location = new System.Drawing.Point(117, 205);
            this.lbLevelFour.Name = "lbLevelFour";
            this.lbLevelFour.Size = new System.Drawing.Size(35, 12);
            this.lbLevelFour.TabIndex = 17;
            this.lbLevelFour.Text = "000명";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(30, 205);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(63, 12);
            this.label17.TabIndex = 16;
            this.label17.Text = "4 직급이하";
            // 
            // lbLevelThree
            // 
            this.lbLevelThree.AutoSize = true;
            this.lbLevelThree.Location = new System.Drawing.Point(117, 181);
            this.lbLevelThree.Name = "lbLevelThree";
            this.lbLevelThree.Size = new System.Drawing.Size(35, 12);
            this.lbLevelThree.TabIndex = 15;
            this.lbLevelThree.Text = "000명";
            // 
            // lbLevelTwo
            // 
            this.lbLevelTwo.AutoSize = true;
            this.lbLevelTwo.Location = new System.Drawing.Point(117, 157);
            this.lbLevelTwo.Name = "lbLevelTwo";
            this.lbLevelTwo.Size = new System.Drawing.Size(35, 12);
            this.lbLevelTwo.TabIndex = 14;
            this.lbLevelTwo.Text = "000명";
            // 
            // lbLevelOne
            // 
            this.lbLevelOne.AutoSize = true;
            this.lbLevelOne.Location = new System.Drawing.Point(117, 133);
            this.lbLevelOne.Name = "lbLevelOne";
            this.lbLevelOne.Size = new System.Drawing.Size(35, 12);
            this.lbLevelOne.TabIndex = 13;
            this.lbLevelOne.Text = "000명";
            // 
            // lbExternal
            // 
            this.lbExternal.AutoSize = true;
            this.lbExternal.Location = new System.Drawing.Point(116, 64);
            this.lbExternal.Name = "lbExternal";
            this.lbExternal.Size = new System.Drawing.Size(35, 12);
            this.lbExternal.TabIndex = 11;
            this.lbExternal.Text = "000명";
            // 
            // lbTimeOff
            // 
            this.lbTimeOff.AutoSize = true;
            this.lbTimeOff.Location = new System.Drawing.Point(116, 89);
            this.lbTimeOff.Name = "lbTimeOff";
            this.lbTimeOff.Size = new System.Drawing.Size(35, 12);
            this.lbTimeOff.TabIndex = 10;
            this.lbTimeOff.Text = "000명";
            this.lbTimeOff.Visible = false;
            // 
            // lbCompany
            // 
            this.lbCompany.AutoSize = true;
            this.lbCompany.Location = new System.Drawing.Point(116, 40);
            this.lbCompany.Name = "lbCompany";
            this.lbCompany.Size = new System.Drawing.Size(35, 12);
            this.lbCompany.TabIndex = 9;
            this.lbCompany.Text = "000명";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(30, 181);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 12);
            this.label5.TabIndex = 7;
            this.label5.Text = "3 직급";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(30, 157);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 12);
            this.label6.TabIndex = 6;
            this.label6.Text = "2 직급";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(30, 133);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(39, 12);
            this.label7.TabIndex = 5;
            this.label7.Text = "1 직급";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(29, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "휴직자";
            this.label4.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(29, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "외부 업체 직원";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "삼천포 임직원";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 155);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 12);
            this.label1.TabIndex = 17;
            this.label1.Text = "전송 대상자 선택";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(659, 525);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.lableLength);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.Text = "삼천포 SMS 발송기";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPageToAll.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPageTeam.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPageLevel.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label lableLength;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageToAll;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbToAll;
        private System.Windows.Forms.CheckBox cbToExternal;
        private System.Windows.Forms.CheckBox cbToTimeOff;
        private System.Windows.Forms.TabPage tabPageTeam;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.CheckBox cbPos1;
        private System.Windows.Forms.CheckBox cbPos2;
        private System.Windows.Forms.TabPage tabPageLevel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox cbLevel4;
        private System.Windows.Forms.CheckBox cbLevel1;
        private System.Windows.Forms.CheckBox cbLevel3;
        private System.Windows.Forms.CheckBox cbLevel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lbLevelFour;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label lbLevelThree;
        private System.Windows.Forms.Label lbLevelTwo;
        private System.Windows.Forms.Label lbLevelOne;
        private System.Windows.Forms.Label lbExternal;
        private System.Windows.Forms.Label lbTimeOff;
        private System.Windows.Forms.Label lbCompany;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label1;
    }
}

