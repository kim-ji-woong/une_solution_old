namespace ControlTeamEditor
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.cmbLocation = new System.Windows.Forms.ComboBox();
            this.cmbTeam = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnDelete8 = new System.Windows.Forms.Button();
            this.btnDelete7 = new System.Windows.Forms.Button();
            this.btnDelete6 = new System.Windows.Forms.Button();
            this.btnDelete5 = new System.Windows.Forms.Button();
            this.btnDelete4 = new System.Windows.Forms.Button();
            this.btnDelete3 = new System.Windows.Forms.Button();
            this.btnDelete2 = new System.Windows.Forms.Button();
            this.btnDelete1 = new System.Windows.Forms.Button();
            this.btnSelect8 = new System.Windows.Forms.Button();
            this.btnSelect7 = new System.Windows.Forms.Button();
            this.btnSelect6 = new System.Windows.Forms.Button();
            this.btnSelect5 = new System.Windows.Forms.Button();
            this.btnSelect4 = new System.Windows.Forms.Button();
            this.btnSelect3 = new System.Windows.Forms.Button();
            this.btnSelect2 = new System.Windows.Forms.Button();
            this.btnSelect1 = new System.Windows.Forms.Button();
            this.editMember8 = new System.Windows.Forms.TextBox();
            this.editMember7 = new System.Windows.Forms.TextBox();
            this.editMember6 = new System.Windows.Forms.TextBox();
            this.editMember5 = new System.Windows.Forms.TextBox();
            this.editMember4 = new System.Windows.Forms.TextBox();
            this.editMember3 = new System.Windows.Forms.TextBox();
            this.editMember2 = new System.Windows.Forms.TextBox();
            this.editMember1 = new System.Windows.Forms.TextBox();
            this.editJob8 = new System.Windows.Forms.TextBox();
            this.editJob7 = new System.Windows.Forms.TextBox();
            this.editJob6 = new System.Windows.Forms.TextBox();
            this.editJob5 = new System.Windows.Forms.TextBox();
            this.editJob4 = new System.Windows.Forms.TextBox();
            this.editJob3 = new System.Windows.Forms.TextBox();
            this.editJob2 = new System.Windows.Forms.TextBox();
            this.editJob1 = new System.Windows.Forms.TextBox();
            this.treeViewTeam = new System.Windows.Forms.TreeView();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbWorkingTeam = new System.Windows.Forms.ComboBox();
            this.lbLocation = new System.Windows.Forms.Label();
            this.gridMemebers = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridMemebers)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbLocation
            // 
            this.cmbLocation.DisplayMember = "DisplayText";
            this.cmbLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLocation.FormattingEnabled = true;
            this.cmbLocation.Location = new System.Drawing.Point(73, 26);
            this.cmbLocation.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbLocation.Name = "cmbLocation";
            this.cmbLocation.Size = new System.Drawing.Size(99, 23);
            this.cmbLocation.TabIndex = 0;
            this.cmbLocation.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            this.cmbLocation.SelectionChangeCommitted += new System.EventHandler(this.cmbLocation_SelectionChangeCommitted);
            // 
            // cmbTeam
            // 
            this.cmbTeam.DisplayMember = "DisplayText";
            this.cmbTeam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTeam.FormattingEnabled = true;
            this.cmbTeam.Location = new System.Drawing.Point(28, 23);
            this.cmbTeam.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbTeam.Name = "cmbTeam";
            this.cmbTeam.Size = new System.Drawing.Size(91, 23);
            this.cmbTeam.TabIndex = 1;
            this.cmbTeam.SelectionChangeCommitted += new System.EventHandler(this.cmbTeam_SelectionChangeCommitted);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(903, 457);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(70, 31);
            this.button1.TabIndex = 2;
            this.button1.Text = "확인";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(979, 457);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(70, 31);
            this.button2.TabIndex = 3;
            this.button2.Text = "취소";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 15);
            this.label1.TabIndex = 8;
            this.label1.Text = "호기별";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnDelete8);
            this.groupBox1.Controls.Add(this.btnDelete7);
            this.groupBox1.Controls.Add(this.btnDelete6);
            this.groupBox1.Controls.Add(this.btnDelete5);
            this.groupBox1.Controls.Add(this.btnDelete4);
            this.groupBox1.Controls.Add(this.btnDelete3);
            this.groupBox1.Controls.Add(this.btnDelete2);
            this.groupBox1.Controls.Add(this.btnDelete1);
            this.groupBox1.Controls.Add(this.btnSelect8);
            this.groupBox1.Controls.Add(this.btnSelect7);
            this.groupBox1.Controls.Add(this.btnSelect6);
            this.groupBox1.Controls.Add(this.cmbTeam);
            this.groupBox1.Controls.Add(this.btnSelect5);
            this.groupBox1.Controls.Add(this.btnSelect4);
            this.groupBox1.Controls.Add(this.btnSelect3);
            this.groupBox1.Controls.Add(this.btnSelect2);
            this.groupBox1.Controls.Add(this.btnSelect1);
            this.groupBox1.Controls.Add(this.editMember8);
            this.groupBox1.Controls.Add(this.editMember7);
            this.groupBox1.Controls.Add(this.editMember6);
            this.groupBox1.Controls.Add(this.editMember5);
            this.groupBox1.Controls.Add(this.editMember4);
            this.groupBox1.Controls.Add(this.editMember3);
            this.groupBox1.Controls.Add(this.editMember2);
            this.groupBox1.Controls.Add(this.editMember1);
            this.groupBox1.Controls.Add(this.editJob8);
            this.groupBox1.Controls.Add(this.editJob7);
            this.groupBox1.Controls.Add(this.editJob6);
            this.groupBox1.Controls.Add(this.editJob5);
            this.groupBox1.Controls.Add(this.editJob4);
            this.groupBox1.Controls.Add(this.editJob3);
            this.groupBox1.Controls.Add(this.editJob2);
            this.groupBox1.Controls.Add(this.editJob1);
            this.groupBox1.Location = new System.Drawing.Point(29, 66);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(389, 373);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "근무자 입력";
            // 
            // btnDelete8
            // 
            this.btnDelete8.Location = new System.Drawing.Point(320, 327);
            this.btnDelete8.Name = "btnDelete8";
            this.btnDelete8.Size = new System.Drawing.Size(56, 23);
            this.btnDelete8.TabIndex = 39;
            this.btnDelete8.Text = "삭제";
            this.btnDelete8.UseVisualStyleBackColor = true;
            this.btnDelete8.Click += new System.EventHandler(this.OnDeleteMember);
            // 
            // btnDelete7
            // 
            this.btnDelete7.Location = new System.Drawing.Point(320, 291);
            this.btnDelete7.Name = "btnDelete7";
            this.btnDelete7.Size = new System.Drawing.Size(56, 23);
            this.btnDelete7.TabIndex = 38;
            this.btnDelete7.Text = "삭제";
            this.btnDelete7.UseVisualStyleBackColor = true;
            this.btnDelete7.Click += new System.EventHandler(this.OnDeleteMember);
            // 
            // btnDelete6
            // 
            this.btnDelete6.Location = new System.Drawing.Point(320, 253);
            this.btnDelete6.Name = "btnDelete6";
            this.btnDelete6.Size = new System.Drawing.Size(56, 23);
            this.btnDelete6.TabIndex = 37;
            this.btnDelete6.Text = "삭제";
            this.btnDelete6.UseVisualStyleBackColor = true;
            this.btnDelete6.Click += new System.EventHandler(this.OnDeleteMember);
            // 
            // btnDelete5
            // 
            this.btnDelete5.Location = new System.Drawing.Point(320, 217);
            this.btnDelete5.Name = "btnDelete5";
            this.btnDelete5.Size = new System.Drawing.Size(56, 23);
            this.btnDelete5.TabIndex = 36;
            this.btnDelete5.Text = "삭제";
            this.btnDelete5.UseVisualStyleBackColor = true;
            this.btnDelete5.Click += new System.EventHandler(this.OnDeleteMember);
            // 
            // btnDelete4
            // 
            this.btnDelete4.Location = new System.Drawing.Point(320, 179);
            this.btnDelete4.Name = "btnDelete4";
            this.btnDelete4.Size = new System.Drawing.Size(56, 23);
            this.btnDelete4.TabIndex = 35;
            this.btnDelete4.Text = "삭제";
            this.btnDelete4.UseVisualStyleBackColor = true;
            this.btnDelete4.Click += new System.EventHandler(this.OnDeleteMember);
            // 
            // btnDelete3
            // 
            this.btnDelete3.Location = new System.Drawing.Point(320, 142);
            this.btnDelete3.Name = "btnDelete3";
            this.btnDelete3.Size = new System.Drawing.Size(56, 23);
            this.btnDelete3.TabIndex = 34;
            this.btnDelete3.Text = "삭제";
            this.btnDelete3.UseVisualStyleBackColor = true;
            this.btnDelete3.Click += new System.EventHandler(this.OnDeleteMember);
            // 
            // btnDelete2
            // 
            this.btnDelete2.Location = new System.Drawing.Point(320, 106);
            this.btnDelete2.Name = "btnDelete2";
            this.btnDelete2.Size = new System.Drawing.Size(56, 23);
            this.btnDelete2.TabIndex = 33;
            this.btnDelete2.Text = "삭제";
            this.btnDelete2.UseVisualStyleBackColor = true;
            this.btnDelete2.Click += new System.EventHandler(this.OnDeleteMember);
            // 
            // btnDelete1
            // 
            this.btnDelete1.Location = new System.Drawing.Point(320, 69);
            this.btnDelete1.Name = "btnDelete1";
            this.btnDelete1.Size = new System.Drawing.Size(56, 23);
            this.btnDelete1.TabIndex = 32;
            this.btnDelete1.Text = "삭제";
            this.btnDelete1.UseVisualStyleBackColor = true;
            this.btnDelete1.Click += new System.EventHandler(this.OnDeleteMember);
            // 
            // btnSelect8
            // 
            this.btnSelect8.Location = new System.Drawing.Point(261, 327);
            this.btnSelect8.Name = "btnSelect8";
            this.btnSelect8.Size = new System.Drawing.Size(56, 23);
            this.btnSelect8.TabIndex = 31;
            this.btnSelect8.Text = "선택";
            this.btnSelect8.UseVisualStyleBackColor = true;
            this.btnSelect8.Click += new System.EventHandler(this.OnSelectMember);
            // 
            // btnSelect7
            // 
            this.btnSelect7.Location = new System.Drawing.Point(261, 291);
            this.btnSelect7.Name = "btnSelect7";
            this.btnSelect7.Size = new System.Drawing.Size(56, 23);
            this.btnSelect7.TabIndex = 30;
            this.btnSelect7.Text = "선택";
            this.btnSelect7.UseVisualStyleBackColor = true;
            this.btnSelect7.Click += new System.EventHandler(this.OnSelectMember);
            // 
            // btnSelect6
            // 
            this.btnSelect6.Location = new System.Drawing.Point(261, 253);
            this.btnSelect6.Name = "btnSelect6";
            this.btnSelect6.Size = new System.Drawing.Size(56, 23);
            this.btnSelect6.TabIndex = 29;
            this.btnSelect6.Text = "선택";
            this.btnSelect6.UseVisualStyleBackColor = true;
            this.btnSelect6.Click += new System.EventHandler(this.OnSelectMember);
            // 
            // btnSelect5
            // 
            this.btnSelect5.Location = new System.Drawing.Point(261, 217);
            this.btnSelect5.Name = "btnSelect5";
            this.btnSelect5.Size = new System.Drawing.Size(56, 23);
            this.btnSelect5.TabIndex = 28;
            this.btnSelect5.Text = "선택";
            this.btnSelect5.UseVisualStyleBackColor = true;
            this.btnSelect5.Click += new System.EventHandler(this.OnSelectMember);
            // 
            // btnSelect4
            // 
            this.btnSelect4.Location = new System.Drawing.Point(261, 179);
            this.btnSelect4.Name = "btnSelect4";
            this.btnSelect4.Size = new System.Drawing.Size(56, 23);
            this.btnSelect4.TabIndex = 27;
            this.btnSelect4.Text = "선택";
            this.btnSelect4.UseVisualStyleBackColor = true;
            this.btnSelect4.Click += new System.EventHandler(this.OnSelectMember);
            // 
            // btnSelect3
            // 
            this.btnSelect3.Location = new System.Drawing.Point(261, 142);
            this.btnSelect3.Name = "btnSelect3";
            this.btnSelect3.Size = new System.Drawing.Size(56, 23);
            this.btnSelect3.TabIndex = 26;
            this.btnSelect3.Text = "선택";
            this.btnSelect3.UseVisualStyleBackColor = true;
            this.btnSelect3.Click += new System.EventHandler(this.OnSelectMember);
            // 
            // btnSelect2
            // 
            this.btnSelect2.Location = new System.Drawing.Point(261, 106);
            this.btnSelect2.Name = "btnSelect2";
            this.btnSelect2.Size = new System.Drawing.Size(56, 23);
            this.btnSelect2.TabIndex = 25;
            this.btnSelect2.Text = "선택";
            this.btnSelect2.UseVisualStyleBackColor = true;
            this.btnSelect2.Click += new System.EventHandler(this.OnSelectMember);
            // 
            // btnSelect1
            // 
            this.btnSelect1.Location = new System.Drawing.Point(261, 69);
            this.btnSelect1.Name = "btnSelect1";
            this.btnSelect1.Size = new System.Drawing.Size(56, 23);
            this.btnSelect1.TabIndex = 24;
            this.btnSelect1.Text = "선택";
            this.btnSelect1.UseVisualStyleBackColor = true;
            this.btnSelect1.Click += new System.EventHandler(this.OnSelectMember);
            // 
            // editMember8
            // 
            this.editMember8.BackColor = System.Drawing.Color.White;
            this.editMember8.Location = new System.Drawing.Point(148, 328);
            this.editMember8.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.editMember8.Name = "editMember8";
            this.editMember8.ReadOnly = true;
            this.editMember8.Size = new System.Drawing.Size(101, 23);
            this.editMember8.TabIndex = 23;
            // 
            // editMember7
            // 
            this.editMember7.BackColor = System.Drawing.Color.White;
            this.editMember7.Location = new System.Drawing.Point(148, 291);
            this.editMember7.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.editMember7.Name = "editMember7";
            this.editMember7.ReadOnly = true;
            this.editMember7.Size = new System.Drawing.Size(101, 23);
            this.editMember7.TabIndex = 22;
            // 
            // editMember6
            // 
            this.editMember6.BackColor = System.Drawing.Color.White;
            this.editMember6.Location = new System.Drawing.Point(148, 254);
            this.editMember6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.editMember6.Name = "editMember6";
            this.editMember6.ReadOnly = true;
            this.editMember6.Size = new System.Drawing.Size(101, 23);
            this.editMember6.TabIndex = 21;
            // 
            // editMember5
            // 
            this.editMember5.BackColor = System.Drawing.Color.White;
            this.editMember5.Location = new System.Drawing.Point(148, 217);
            this.editMember5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.editMember5.Name = "editMember5";
            this.editMember5.ReadOnly = true;
            this.editMember5.Size = new System.Drawing.Size(101, 23);
            this.editMember5.TabIndex = 20;
            // 
            // editMember4
            // 
            this.editMember4.BackColor = System.Drawing.Color.White;
            this.editMember4.Location = new System.Drawing.Point(148, 180);
            this.editMember4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.editMember4.Name = "editMember4";
            this.editMember4.ReadOnly = true;
            this.editMember4.Size = new System.Drawing.Size(101, 23);
            this.editMember4.TabIndex = 19;
            // 
            // editMember3
            // 
            this.editMember3.BackColor = System.Drawing.Color.White;
            this.editMember3.Location = new System.Drawing.Point(148, 143);
            this.editMember3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.editMember3.Name = "editMember3";
            this.editMember3.ReadOnly = true;
            this.editMember3.Size = new System.Drawing.Size(101, 23);
            this.editMember3.TabIndex = 18;
            // 
            // editMember2
            // 
            this.editMember2.BackColor = System.Drawing.Color.White;
            this.editMember2.Location = new System.Drawing.Point(148, 106);
            this.editMember2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.editMember2.Name = "editMember2";
            this.editMember2.ReadOnly = true;
            this.editMember2.Size = new System.Drawing.Size(101, 23);
            this.editMember2.TabIndex = 17;
            // 
            // editMember1
            // 
            this.editMember1.BackColor = System.Drawing.Color.White;
            this.editMember1.Location = new System.Drawing.Point(148, 69);
            this.editMember1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.editMember1.Name = "editMember1";
            this.editMember1.ReadOnly = true;
            this.editMember1.Size = new System.Drawing.Size(101, 23);
            this.editMember1.TabIndex = 16;
            // 
            // editJob8
            // 
            this.editJob8.Enabled = false;
            this.editJob8.Location = new System.Drawing.Point(28, 328);
            this.editJob8.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.editJob8.Name = "editJob8";
            this.editJob8.Size = new System.Drawing.Size(101, 23);
            this.editJob8.TabIndex = 15;
            // 
            // editJob7
            // 
            this.editJob7.Enabled = false;
            this.editJob7.Location = new System.Drawing.Point(28, 291);
            this.editJob7.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.editJob7.Name = "editJob7";
            this.editJob7.Size = new System.Drawing.Size(101, 23);
            this.editJob7.TabIndex = 14;
            // 
            // editJob6
            // 
            this.editJob6.Enabled = false;
            this.editJob6.Location = new System.Drawing.Point(28, 254);
            this.editJob6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.editJob6.Name = "editJob6";
            this.editJob6.Size = new System.Drawing.Size(101, 23);
            this.editJob6.TabIndex = 13;
            // 
            // editJob5
            // 
            this.editJob5.Enabled = false;
            this.editJob5.Location = new System.Drawing.Point(28, 217);
            this.editJob5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.editJob5.Name = "editJob5";
            this.editJob5.Size = new System.Drawing.Size(101, 23);
            this.editJob5.TabIndex = 12;
            // 
            // editJob4
            // 
            this.editJob4.Enabled = false;
            this.editJob4.Location = new System.Drawing.Point(28, 180);
            this.editJob4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.editJob4.Name = "editJob4";
            this.editJob4.Size = new System.Drawing.Size(101, 23);
            this.editJob4.TabIndex = 11;
            // 
            // editJob3
            // 
            this.editJob3.Enabled = false;
            this.editJob3.Location = new System.Drawing.Point(28, 143);
            this.editJob3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.editJob3.Name = "editJob3";
            this.editJob3.Size = new System.Drawing.Size(101, 23);
            this.editJob3.TabIndex = 10;
            // 
            // editJob2
            // 
            this.editJob2.Enabled = false;
            this.editJob2.Location = new System.Drawing.Point(28, 106);
            this.editJob2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.editJob2.Name = "editJob2";
            this.editJob2.Size = new System.Drawing.Size(101, 23);
            this.editJob2.TabIndex = 9;
            // 
            // editJob1
            // 
            this.editJob1.Enabled = false;
            this.editJob1.Location = new System.Drawing.Point(28, 69);
            this.editJob1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.editJob1.Name = "editJob1";
            this.editJob1.Size = new System.Drawing.Size(101, 23);
            this.editJob1.TabIndex = 8;
            // 
            // treeViewTeam
            // 
            this.treeViewTeam.Location = new System.Drawing.Point(424, 74);
            this.treeViewTeam.Name = "treeViewTeam";
            this.treeViewTeam.Size = new System.Drawing.Size(284, 365);
            this.treeViewTeam.TabIndex = 11;
            this.treeViewTeam.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewTeam_AfterSelect);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(504, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 15);
            this.label3.TabIndex = 14;
            this.label3.Text = "현재 근무조";
            // 
            // cmbWorkingTeam
            // 
            this.cmbWorkingTeam.DisplayMember = "DisplayText";
            this.cmbWorkingTeam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWorkingTeam.FormattingEnabled = true;
            this.cmbWorkingTeam.Location = new System.Drawing.Point(598, 26);
            this.cmbWorkingTeam.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbWorkingTeam.Name = "cmbWorkingTeam";
            this.cmbWorkingTeam.Size = new System.Drawing.Size(91, 23);
            this.cmbWorkingTeam.TabIndex = 15;
            this.cmbWorkingTeam.SelectedIndexChanged += new System.EventHandler(this.cmbWorkingTeam_SelectedIndexChanged);
            this.cmbWorkingTeam.SelectionChangeCommitted += new System.EventHandler(this.cmbWorkingTeam_SelectionChangeCommitted);
            // 
            // lbLocation
            // 
            this.lbLocation.AutoSize = true;
            this.lbLocation.Location = new System.Drawing.Point(443, 29);
            this.lbLocation.Name = "lbLocation";
            this.lbLocation.Size = new System.Drawing.Size(43, 15);
            this.lbLocation.TabIndex = 16;
            this.lbLocation.Text = "   호기";
            // 
            // gridMemebers
            // 
            this.gridMemebers.AllowUserToAddRows = false;
            this.gridMemebers.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridMemebers.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridMemebers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridMemebers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3});
            this.gridMemebers.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.gridMemebers.Location = new System.Drawing.Point(714, 74);
            this.gridMemebers.MultiSelect = false;
            this.gridMemebers.Name = "gridMemebers";
            this.gridMemebers.RowHeadersVisible = false;
            this.gridMemebers.RowTemplate.Height = 23;
            this.gridMemebers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridMemebers.Size = new System.Drawing.Size(335, 365);
            this.gridMemebers.TabIndex = 17;
            // 
            // Column1
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column1.DefaultCellStyle = dataGridViewCellStyle2;
            this.Column1.HeaderText = "순번";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column2.DefaultCellStyle = dataGridViewCellStyle3;
            this.Column2.HeaderText = "직급";
            this.Column2.Name = "Column2";
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column3.DefaultCellStyle = dataGridViewCellStyle4;
            this.Column3.HeaderText = "이름";
            this.Column3.Name = "Column3";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(695, 26);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(143, 24);
            this.button3.TabIndex = 18;
            this.button3.Text = "현재 근무조 변경하기";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.OnSelectWorkingTeam);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1062, 501);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.gridMemebers);
            this.Controls.Add(this.lbLocation);
            this.Controls.Add(this.cmbWorkingTeam);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.treeViewTeam);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cmbLocation);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormMain";
            this.ShowIcon = false;
            this.Text = "근무표 입력";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridMemebers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbLocation;
        private System.Windows.Forms.ComboBox cmbTeam;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox editJob7;
        private System.Windows.Forms.TextBox editJob6;
        private System.Windows.Forms.TextBox editJob5;
        private System.Windows.Forms.TextBox editJob4;
        private System.Windows.Forms.TextBox editJob3;
        private System.Windows.Forms.TextBox editJob2;
        private System.Windows.Forms.TextBox editJob1;
        private System.Windows.Forms.TextBox editJob8;
        private System.Windows.Forms.Button btnSelect8;
        private System.Windows.Forms.Button btnSelect7;
        private System.Windows.Forms.Button btnSelect6;
        private System.Windows.Forms.Button btnSelect5;
        private System.Windows.Forms.Button btnSelect4;
        private System.Windows.Forms.Button btnSelect3;
        private System.Windows.Forms.Button btnSelect2;
        private System.Windows.Forms.Button btnSelect1;
        private System.Windows.Forms.TextBox editMember8;
        private System.Windows.Forms.TextBox editMember7;
        private System.Windows.Forms.TextBox editMember6;
        private System.Windows.Forms.TextBox editMember5;
        private System.Windows.Forms.TextBox editMember4;
        private System.Windows.Forms.TextBox editMember3;
        private System.Windows.Forms.TextBox editMember2;
        private System.Windows.Forms.TextBox editMember1;
        private System.Windows.Forms.TreeView treeViewTeam;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbWorkingTeam;
        private System.Windows.Forms.Label lbLocation;
        private System.Windows.Forms.DataGridView gridMemebers;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.Button btnDelete8;
        private System.Windows.Forms.Button btnDelete7;
        private System.Windows.Forms.Button btnDelete6;
        private System.Windows.Forms.Button btnDelete5;
        private System.Windows.Forms.Button btnDelete4;
        private System.Windows.Forms.Button btnDelete3;
        private System.Windows.Forms.Button btnDelete2;
        private System.Windows.Forms.Button btnDelete1;
        private System.Windows.Forms.Button button3;
    }
}

