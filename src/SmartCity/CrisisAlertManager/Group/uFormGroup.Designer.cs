namespace CrisisAlertManager.Group
{
    partial class uFormGroup
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

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.plCityHall = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.gridCityHall = new System.Windows.Forms.DataGridView();
            this.colCityHallName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCityHallRemove = new UnE.GUI.ImageButton();
            this.btnCityHallAdd = new UnE.GUI.ImageButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.gridMember = new System.Windows.Forms.DataGridView();
            this.eleFacilityType = new System.Windows.Forms.Integration.ElementHost();
            this.eleDepartment = new System.Windows.Forms.Integration.ElementHost();
            this.plSMSReport = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btnMemberRemove = new UnE.GUI.ImageButton();
            this.btnMemberAdd = new UnE.GUI.ImageButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRedo = new UnE.GUI.ImageButton();
            this.btnBack = new UnE.GUI.ImageButton();
            this.btnLoad = new UnE.GUI.ImageButton();
            this.btnSave = new UnE.GUI.ImageButton();
            this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCityHall = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDepartment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colJobLevel = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPhoneNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFacilityType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSelectFacilityType = new System.Windows.Forms.DataGridViewImageColumn();
            this.plCityHall.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridCityHall)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnCityHallRemove)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCityHallAdd)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridMember)).BeginInit();
            this.plSMSReport.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMemberRemove)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMemberAdd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRedo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLoad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSave)).BeginInit();
            this.SuspendLayout();
            // 
            // plCityHall
            // 
            this.plCityHall.BackColor = System.Drawing.Color.Transparent;
            this.plCityHall.Controls.Add(this.panel2);
            this.plCityHall.Controls.Add(this.panel1);
            this.plCityHall.Location = new System.Drawing.Point(32, 27);
            this.plCityHall.Name = "plCityHall";
            this.plCityHall.Size = new System.Drawing.Size(230, 830);
            this.plCityHall.TabIndex = 5;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.gridCityHall);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 54);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(230, 776);
            this.panel2.TabIndex = 1;
            // 
            // gridCityHall
            // 
            this.gridCityHall.AllowUserToAddRows = false;
            this.gridCityHall.BackgroundColor = System.Drawing.Color.White;
            this.gridCityHall.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridCityHall.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.gridCityHall.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridCityHall.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridCityHall.ColumnHeadersHeight = 40;
            this.gridCityHall.ColumnHeadersVisible = false;
            this.gridCityHall.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCityHallName,
            this.colCheck});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridCityHall.DefaultCellStyle = dataGridViewCellStyle2;
            this.gridCityHall.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.gridCityHall.Location = new System.Drawing.Point(0, 10);
            this.gridCityHall.Name = "gridCityHall";
            this.gridCityHall.RowHeadersVisible = false;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.gridCityHall.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.gridCityHall.RowTemplate.Height = 45;
            this.gridCityHall.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridCityHall.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridCityHall.Size = new System.Drawing.Size(230, 766);
            this.gridCityHall.TabIndex = 29;
            this.gridCityHall.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridCityHall_CellClick);
            this.gridCityHall.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridCityHall_CellEndEdit);
            this.gridCityHall.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridCityHall_CellValueChanged);
            this.gridCityHall.CurrentCellDirtyStateChanged += new System.EventHandler(this.gridCityHall_CurrentCellDirtyStateChanged);
            // 
            // colCityHallName
            // 
            this.colCityHallName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colCityHallName.HeaderText = "시/구 이름";
            this.colCityHallName.Name = "colCityHallName";
            this.colCityHallName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colCheck
            // 
            this.colCheck.FalseValue = "False";
            this.colCheck.HeaderText = "체크";
            this.colCheck.Name = "colCheck";
            this.colCheck.TrueValue = "True";
            this.colCheck.Width = 40;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(74)))), ((int)(((byte)(127)))));
            this.panel1.Controls.Add(this.btnCityHallRemove);
            this.panel1.Controls.Add(this.btnCityHallAdd);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(230, 54);
            this.panel1.TabIndex = 0;
            // 
            // btnCityHallRemove
            // 
            this.btnCityHallRemove.ButtonText = "";
            this.btnCityHallRemove.ImageClicked = global::CrisisAlertManager.Properties.Resources.CityHallRemove_Click;
            this.btnCityHallRemove.ImageDisabled = null;
            this.btnCityHallRemove.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.CityHallRemove_Hover;
            this.btnCityHallRemove.ImageNormal = global::CrisisAlertManager.Properties.Resources.CityHallRemove_Normal;
            this.btnCityHallRemove.Location = new System.Drawing.Point(119, 8);
            this.btnCityHallRemove.Name = "btnCityHallRemove";
            this.btnCityHallRemove.Owner = null;
            this.btnCityHallRemove.Size = new System.Drawing.Size(102, 36);
            this.btnCityHallRemove.TabIndex = 60;
            this.btnCityHallRemove.TabStop = false;
            this.btnCityHallRemove.TextColor = System.Drawing.Color.Black;
            this.btnCityHallRemove.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCityHallRemove.ToolTipText = "";
            this.btnCityHallRemove.UseToolTip = false;
            this.btnCityHallRemove.WindowRateWidth = 1F;
            this.btnCityHallRemove.Click += new System.EventHandler(this.btnCityHallRemove_Click);
            // 
            // btnCityHallAdd
            // 
            this.btnCityHallAdd.ButtonText = "";
            this.btnCityHallAdd.ImageClicked = global::CrisisAlertManager.Properties.Resources.CityHallAdd_Click;
            this.btnCityHallAdd.ImageDisabled = null;
            this.btnCityHallAdd.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.CityHallAdd_Hover;
            this.btnCityHallAdd.ImageNormal = global::CrisisAlertManager.Properties.Resources.CityHallAdd_Normal;
            this.btnCityHallAdd.Location = new System.Drawing.Point(9, 8);
            this.btnCityHallAdd.Name = "btnCityHallAdd";
            this.btnCityHallAdd.Owner = null;
            this.btnCityHallAdd.Size = new System.Drawing.Size(102, 36);
            this.btnCityHallAdd.TabIndex = 59;
            this.btnCityHallAdd.TabStop = false;
            this.btnCityHallAdd.TextColor = System.Drawing.Color.Black;
            this.btnCityHallAdd.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCityHallAdd.ToolTipText = "";
            this.btnCityHallAdd.UseToolTip = false;
            this.btnCityHallAdd.WindowRateWidth = 1F;
            this.btnCityHallAdd.Click += new System.EventHandler(this.btnCityHallAdd_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.gridMember);
            this.panel3.Controls.Add(this.eleFacilityType);
            this.panel3.Controls.Add(this.eleDepartment);
            this.panel3.Controls.Add(this.plSMSReport);
            this.panel3.Controls.Add(this.btnMemberRemove);
            this.panel3.Controls.Add(this.btnMemberAdd);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.btnRedo);
            this.panel3.Controls.Add(this.btnBack);
            this.panel3.Controls.Add(this.btnLoad);
            this.panel3.Controls.Add(this.btnSave);
            this.panel3.Location = new System.Drawing.Point(285, 27);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1130, 830);
            this.panel3.TabIndex = 6;
            // 
            // gridMember
            // 
            this.gridMember.AllowUserToAddRows = false;
            this.gridMember.AllowUserToResizeColumns = false;
            this.gridMember.AllowUserToResizeRows = false;
            this.gridMember.BackgroundColor = System.Drawing.Color.White;
            this.gridMember.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridMember.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.gridMember.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridMember.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.gridMember.ColumnHeadersHeight = 40;
            this.gridMember.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridMember.ColumnHeadersVisible = false;
            this.gridMember.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colCityHall,
            this.colDepartment,
            this.colJobLevel,
            this.colName,
            this.colPhoneNum,
            this.colFacilityType,
            this.colSelectFacilityType});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridMember.DefaultCellStyle = dataGridViewCellStyle6;
            this.gridMember.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.gridMember.Location = new System.Drawing.Point(22, 123);
            this.gridMember.Name = "gridMember";
            this.gridMember.RowHeadersVisible = false;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.gridMember.RowsDefaultCellStyle = dataGridViewCellStyle7;
            this.gridMember.RowTemplate.Height = 50;
            this.gridMember.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridMember.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridMember.Size = new System.Drawing.Size(1090, 707);
            this.gridMember.TabIndex = 69;
            this.gridMember.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridMember_CellClick);
            this.gridMember.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridMember_CellEndEdit);
            this.gridMember.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.gridMember_EditingControlShowing);
            // 
            // eleFacilityType
            // 
            this.eleFacilityType.BackColor = System.Drawing.Color.White;
            this.eleFacilityType.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.eleFacilityType.Location = new System.Drawing.Point(541, 18);
            this.eleFacilityType.Name = "eleFacilityType";
            this.eleFacilityType.Size = new System.Drawing.Size(125, 40);
            this.eleFacilityType.TabIndex = 71;
            this.eleFacilityType.Text = "elementHost1";
            this.eleFacilityType.Child = null;
            // 
            // eleDepartment
            // 
            this.eleDepartment.BackColor = System.Drawing.Color.White;
            this.eleDepartment.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.eleDepartment.Location = new System.Drawing.Point(219, 18);
            this.eleDepartment.Name = "eleDepartment";
            this.eleDepartment.Size = new System.Drawing.Size(125, 40);
            this.eleDepartment.TabIndex = 70;
            this.eleDepartment.Text = "elementHost1";
            this.eleDepartment.Child = null;
            // 
            // plSMSReport
            // 
            this.plSMSReport.Controls.Add(this.label9);
            this.plSMSReport.Controls.Add(this.label8);
            this.plSMSReport.Controls.Add(this.label3);
            this.plSMSReport.Controls.Add(this.label4);
            this.plSMSReport.Controls.Add(this.label5);
            this.plSMSReport.Controls.Add(this.label6);
            this.plSMSReport.Controls.Add(this.label7);
            this.plSMSReport.Controls.Add(this.pictureBox1);
            this.plSMSReport.Controls.Add(this.pictureBox2);
            this.plSMSReport.Location = new System.Drawing.Point(22, 71);
            this.plSMSReport.Name = "plSMSReport";
            this.plSMSReport.Size = new System.Drawing.Size(1090, 52);
            this.plSMSReport.TabIndex = 68;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label9.Location = new System.Drawing.Point(829, 19);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(163, 15);
            this.label9.TabIndex = 49;
            this.label9.Text = "기본 상황전파 대상자";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.Location = new System.Drawing.Point(673, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 15);
            this.label8.TabIndex = 48;
            this.label8.Text = "연락처";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(501, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 15);
            this.label3.TabIndex = 47;
            this.label3.Text = "이름";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(360, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 15);
            this.label4.TabIndex = 46;
            this.label4.Text = "직급";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.Location = new System.Drawing.Point(222, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 15);
            this.label5.TabIndex = 45;
            this.label5.Text = "부서명";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.Location = new System.Drawing.Point(99, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 15);
            this.label6.TabIndex = 44;
            this.label6.Text = "소속";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.Location = new System.Drawing.Point(18, 18);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 15);
            this.label7.TabIndex = 43;
            this.label7.Text = "No.";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(145)))), ((int)(((byte)(164)))), ((int)(((byte)(191)))));
            this.pictureBox1.Location = new System.Drawing.Point(0, 50);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1340, 2);
            this.pictureBox1.TabIndex = 42;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(145)))), ((int)(((byte)(164)))), ((int)(((byte)(191)))));
            this.pictureBox2.Location = new System.Drawing.Point(0, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(1340, 2);
            this.pictureBox2.TabIndex = 41;
            this.pictureBox2.TabStop = false;
            // 
            // btnMemberRemove
            // 
            this.btnMemberRemove.ButtonText = "";
            this.btnMemberRemove.ImageClicked = global::CrisisAlertManager.Properties.Resources.MemberRemove_Click;
            this.btnMemberRemove.ImageDisabled = null;
            this.btnMemberRemove.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.MemberRemove_Hover;
            this.btnMemberRemove.ImageNormal = global::CrisisAlertManager.Properties.Resources.MemberRemove_Normal;
            this.btnMemberRemove.Location = new System.Drawing.Point(1022, 18);
            this.btnMemberRemove.Name = "btnMemberRemove";
            this.btnMemberRemove.Owner = null;
            this.btnMemberRemove.Size = new System.Drawing.Size(90, 40);
            this.btnMemberRemove.TabIndex = 67;
            this.btnMemberRemove.TabStop = false;
            this.btnMemberRemove.TextColor = System.Drawing.Color.Black;
            this.btnMemberRemove.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnMemberRemove.ToolTipText = "";
            this.btnMemberRemove.UseToolTip = false;
            this.btnMemberRemove.WindowRateWidth = 1F;
            this.btnMemberRemove.Click += new System.EventHandler(this.btnMemberRemove_Click);
            // 
            // btnMemberAdd
            // 
            this.btnMemberAdd.ButtonText = "";
            this.btnMemberAdd.ImageClicked = global::CrisisAlertManager.Properties.Resources.MemberAdd_Click;
            this.btnMemberAdd.ImageDisabled = null;
            this.btnMemberAdd.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.MemberAdd_Hover;
            this.btnMemberAdd.ImageNormal = global::CrisisAlertManager.Properties.Resources.MemberAdd_Normal;
            this.btnMemberAdd.Location = new System.Drawing.Point(926, 18);
            this.btnMemberAdd.Name = "btnMemberAdd";
            this.btnMemberAdd.Owner = null;
            this.btnMemberAdd.Size = new System.Drawing.Size(90, 40);
            this.btnMemberAdd.TabIndex = 66;
            this.btnMemberAdd.TabStop = false;
            this.btnMemberAdd.TextColor = System.Drawing.Color.Black;
            this.btnMemberAdd.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnMemberAdd.ToolTipText = "";
            this.btnMemberAdd.UseToolTip = false;
            this.btnMemberAdd.WindowRateWidth = 1F;
            this.btnMemberAdd.Click += new System.EventHandler(this.btnMemberAdd_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(40)))), ((int)(((byte)(76)))));
            this.label1.Location = new System.Drawing.Point(378, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(163, 15);
            this.label1.TabIndex = 65;
            this.label1.Text = "상황전파 대상자 선택";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(40)))), ((int)(((byte)(76)))));
            this.label2.Location = new System.Drawing.Point(142, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 15);
            this.label2.TabIndex = 64;
            this.label2.Text = "부서 선택";
            // 
            // btnRedo
            // 
            this.btnRedo.ButtonText = "";
            this.btnRedo.Enabled = false;
            this.btnRedo.ImageClicked = global::CrisisAlertManager.Properties.Resources.GroupRedo_Click;
            this.btnRedo.ImageDisabled = global::CrisisAlertManager.Properties.Resources.GroupRedo_Hover;
            this.btnRedo.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.GroupRedo_Hover;
            this.btnRedo.ImageNormal = global::CrisisAlertManager.Properties.Resources.GroupRedo_Normal;
            this.btnRedo.Location = new System.Drawing.Point(314, 18);
            this.btnRedo.Name = "btnRedo";
            this.btnRedo.Owner = null;
            this.btnRedo.Size = new System.Drawing.Size(90, 40);
            this.btnRedo.TabIndex = 63;
            this.btnRedo.TabStop = false;
            this.btnRedo.TextColor = System.Drawing.Color.Black;
            this.btnRedo.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRedo.ToolTipText = "";
            this.btnRedo.UseToolTip = false;
            this.btnRedo.Visible = false;
            this.btnRedo.WindowRateWidth = 1F;
            this.btnRedo.Click += new System.EventHandler(this.btnRedo_Click);
            // 
            // btnBack
            // 
            this.btnBack.ButtonText = "";
            this.btnBack.Enabled = false;
            this.btnBack.ImageClicked = global::CrisisAlertManager.Properties.Resources.GroupBack_Click;
            this.btnBack.ImageDisabled = global::CrisisAlertManager.Properties.Resources.GroupBack_Hover;
            this.btnBack.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.GroupBack_Hover;
            this.btnBack.ImageNormal = global::CrisisAlertManager.Properties.Resources.GroupBack_Normal;
            this.btnBack.Location = new System.Drawing.Point(218, 18);
            this.btnBack.Name = "btnBack";
            this.btnBack.Owner = null;
            this.btnBack.Size = new System.Drawing.Size(90, 40);
            this.btnBack.TabIndex = 62;
            this.btnBack.TabStop = false;
            this.btnBack.TextColor = System.Drawing.Color.Black;
            this.btnBack.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnBack.ToolTipText = "";
            this.btnBack.UseToolTip = false;
            this.btnBack.Visible = false;
            this.btnBack.WindowRateWidth = 1F;
            // 
            // btnLoad
            // 
            this.btnLoad.ButtonText = "";
            this.btnLoad.ImageClicked = global::CrisisAlertManager.Properties.Resources.GroupLoad_Click;
            this.btnLoad.ImageDisabled = null;
            this.btnLoad.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.GroupLoad_Hover;
            this.btnLoad.ImageNormal = global::CrisisAlertManager.Properties.Resources.GroupLoad_Normal;
            this.btnLoad.Location = new System.Drawing.Point(122, 18);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Owner = null;
            this.btnLoad.Size = new System.Drawing.Size(90, 40);
            this.btnLoad.TabIndex = 61;
            this.btnLoad.TabStop = false;
            this.btnLoad.TextColor = System.Drawing.Color.Black;
            this.btnLoad.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnLoad.ToolTipText = "";
            this.btnLoad.UseToolTip = false;
            this.btnLoad.Visible = false;
            this.btnLoad.WindowRateWidth = 1F;
            // 
            // btnSave
            // 
            this.btnSave.ButtonText = "";
            this.btnSave.Enabled = false;
            this.btnSave.ImageClicked = global::CrisisAlertManager.Properties.Resources.GroupSave_Click;
            this.btnSave.ImageDisabled = global::CrisisAlertManager.Properties.Resources.GroupSave_Hover;
            this.btnSave.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.GroupSave_Hover;
            this.btnSave.ImageNormal = global::CrisisAlertManager.Properties.Resources.GroupSave_Normal;
            this.btnSave.Location = new System.Drawing.Point(26, 18);
            this.btnSave.Name = "btnSave";
            this.btnSave.Owner = null;
            this.btnSave.Size = new System.Drawing.Size(90, 40);
            this.btnSave.TabIndex = 60;
            this.btnSave.TabStop = false;
            this.btnSave.TextColor = System.Drawing.Color.Black;
            this.btnSave.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSave.ToolTipText = "";
            this.btnSave.UseToolTip = false;
            this.btnSave.WindowRateWidth = 1F;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dataGridViewImageColumn1
            // 
            this.dataGridViewImageColumn1.HeaderText = "선택";
            this.dataGridViewImageColumn1.Image = global::CrisisAlertManager.Properties.Resources.SelectGroup;
            this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            this.dataGridViewImageColumn1.Width = 50;
            // 
            // colNo
            // 
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle5;
            this.colNo.HeaderText = "No";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNo.Width = 55;
            // 
            // colCityHall
            // 
            this.colCityHall.HeaderText = "소속";
            this.colCityHall.Name = "colCityHall";
            this.colCityHall.ReadOnly = true;
            this.colCityHall.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colCityHall.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colCityHall.Width = 130;
            // 
            // colDepartment
            // 
            this.colDepartment.HeaderText = "부서명";
            this.colDepartment.Name = "colDepartment";
            this.colDepartment.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colDepartment.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colDepartment.Width = 130;
            // 
            // colJobLevel
            // 
            this.colJobLevel.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.colJobLevel.HeaderText = "직급";
            this.colJobLevel.Name = "colJobLevel";
            this.colJobLevel.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colJobLevel.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colJobLevel.Width = 130;
            // 
            // colName
            // 
            this.colName.HeaderText = "이름";
            this.colName.Name = "colName";
            this.colName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colName.Width = 150;
            // 
            // colPhoneNum
            // 
            this.colPhoneNum.HeaderText = "연락처";
            this.colPhoneNum.MaxInputLength = 11;
            this.colPhoneNum.Name = "colPhoneNum";
            this.colPhoneNum.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colPhoneNum.Width = 200;
            // 
            // colFacilityType
            // 
            this.colFacilityType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colFacilityType.HeaderText = "기본 상황전파 대상자";
            this.colFacilityType.Name = "colFacilityType";
            this.colFacilityType.ReadOnly = true;
            this.colFacilityType.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // colSelectFacilityType
            // 
            this.colSelectFacilityType.HeaderText = "선택";
            this.colSelectFacilityType.Image = global::CrisisAlertManager.Properties.Resources.SelectFacilityType;
            this.colSelectFacilityType.Name = "colSelectFacilityType";
            this.colSelectFacilityType.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colSelectFacilityType.Width = 60;
            // 
            // uFormGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.plCityHall);
            this.Name = "uFormGroup";
            this.Size = new System.Drawing.Size(1600, 970);
            this.plCityHall.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridCityHall)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnCityHallRemove)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCityHallAdd)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridMember)).EndInit();
            this.plSMSReport.ResumeLayout(false);
            this.plSMSReport.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMemberRemove)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMemberAdd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRedo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLoad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSave)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel plCityHall;
        private System.Windows.Forms.Panel panel1;
        private UnE.GUI.ImageButton btnCityHallAdd;
        private UnE.GUI.ImageButton btnCityHallRemove;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView gridCityHall;
        private System.Windows.Forms.Panel panel3;
        private UnE.GUI.ImageButton btnSave;
        private UnE.GUI.ImageButton btnRedo;
        private UnE.GUI.ImageButton btnBack;
        private UnE.GUI.ImageButton btnLoad;
        private System.Windows.Forms.Label label2;
        private UnE.GUI.ImageButton btnMemberRemove;
        private UnE.GUI.ImageButton btnMemberAdd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel plSMSReport;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCityHallName;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colCheck;
        private System.Windows.Forms.DataGridView gridMember;
        private System.Windows.Forms.Integration.ElementHost eleFacilityType;
        private System.Windows.Forms.Integration.ElementHost eleDepartment;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCityHall;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDepartment;
        private System.Windows.Forms.DataGridViewComboBoxColumn colJobLevel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPhoneNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFacilityType;
        private System.Windows.Forms.DataGridViewImageColumn colSelectFacilityType;
    }
}
