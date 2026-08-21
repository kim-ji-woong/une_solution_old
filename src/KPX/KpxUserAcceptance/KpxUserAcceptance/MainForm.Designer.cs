namespace KpxUserAcceptance
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage_wait = new System.Windows.Forms.TabPage();
            this.button_watiSearch = new System.Windows.Forms.Button();
            this.button_waitOk = new System.Windows.Forms.Button();
            this.dataGridView_wait = new System.Windows.Forms.DataGridView();
            this.tabPage_user = new System.Windows.Forms.TabPage();
            this.button_modifySearch = new System.Windows.Forms.Button();
            this.button_modifyOk = new System.Windows.Forms.Button();
            this.dataGridView_user = new System.Windows.Forms.DataGridView();
            this.tabPage_userGroup = new System.Windows.Forms.TabPage();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.gridUserGroup = new System.Windows.Forms.DataGridView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.메뉴ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.관리자변경ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.비밀번호변경ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.종료ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUserGroup = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuSelectAllTankItems = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuUnselectAllTankItems = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuSelectAllTanks = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuUnselectAllTanks = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuDeleteSelectedUserGroups = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUser = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuDeleteSelectedUsers = new System.Windows.Forms.ToolStripMenuItem();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPipe = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colPipeAlarm = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colLiquidType = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colMass = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colLevel = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colLevelRange = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colGravity = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colFlow = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colTemp = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colTempRange = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colTankAlarm = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colNotice = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colLeak = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tabControl1.SuspendLayout();
            this.tabPage_wait.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_wait)).BeginInit();
            this.tabPage_user.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_user)).BeginInit();
            this.tabPage_userGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridUserGroup)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.menuUserGroup.SuspendLayout();
            this.menuUser.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage_wait);
            this.tabControl1.Controls.Add(this.tabPage_user);
            this.tabControl1.Controls.Add(this.tabPage_userGroup);
            this.tabControl1.Location = new System.Drawing.Point(0, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(645, 426);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage_wait
            // 
            this.tabPage_wait.Controls.Add(this.button_watiSearch);
            this.tabPage_wait.Controls.Add(this.button_waitOk);
            this.tabPage_wait.Controls.Add(this.dataGridView_wait);
            this.tabPage_wait.Location = new System.Drawing.Point(4, 22);
            this.tabPage_wait.Name = "tabPage_wait";
            this.tabPage_wait.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_wait.Size = new System.Drawing.Size(637, 400);
            this.tabPage_wait.TabIndex = 0;
            this.tabPage_wait.Text = "승인대기";
            this.tabPage_wait.UseVisualStyleBackColor = true;
            // 
            // button_watiSearch
            // 
            this.button_watiSearch.Location = new System.Drawing.Point(556, 6);
            this.button_watiSearch.Name = "button_watiSearch";
            this.button_watiSearch.Size = new System.Drawing.Size(75, 23);
            this.button_watiSearch.TabIndex = 2;
            this.button_watiSearch.Text = "조회";
            this.button_watiSearch.UseVisualStyleBackColor = true;
            this.button_watiSearch.Click += new System.EventHandler(this.button_watiSearch_Click);
            // 
            // button_waitOk
            // 
            this.button_waitOk.Location = new System.Drawing.Point(475, 6);
            this.button_waitOk.Name = "button_waitOk";
            this.button_waitOk.Size = new System.Drawing.Size(75, 23);
            this.button_waitOk.TabIndex = 1;
            this.button_waitOk.Text = "저장";
            this.button_waitOk.UseVisualStyleBackColor = true;
            this.button_waitOk.Click += new System.EventHandler(this.button_waitOk_Click);
            // 
            // dataGridView_wait
            // 
            this.dataGridView_wait.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_wait.Location = new System.Drawing.Point(0, 33);
            this.dataGridView_wait.Name = "dataGridView_wait";
            this.dataGridView_wait.RowTemplate.Height = 23;
            this.dataGridView_wait.Size = new System.Drawing.Size(637, 367);
            this.dataGridView_wait.TabIndex = 0;
            // 
            // tabPage_user
            // 
            this.tabPage_user.Controls.Add(this.button_modifySearch);
            this.tabPage_user.Controls.Add(this.button_modifyOk);
            this.tabPage_user.Controls.Add(this.dataGridView_user);
            this.tabPage_user.Location = new System.Drawing.Point(4, 22);
            this.tabPage_user.Name = "tabPage_user";
            this.tabPage_user.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_user.Size = new System.Drawing.Size(637, 400);
            this.tabPage_user.TabIndex = 1;
            this.tabPage_user.Text = "회원 편집";
            this.tabPage_user.UseVisualStyleBackColor = true;
            // 
            // button_modifySearch
            // 
            this.button_modifySearch.Location = new System.Drawing.Point(554, 6);
            this.button_modifySearch.Name = "button_modifySearch";
            this.button_modifySearch.Size = new System.Drawing.Size(75, 23);
            this.button_modifySearch.TabIndex = 4;
            this.button_modifySearch.Text = "조회";
            this.button_modifySearch.UseVisualStyleBackColor = true;
            this.button_modifySearch.Click += new System.EventHandler(this.button_modifySearch_Click);
            // 
            // button_modifyOk
            // 
            this.button_modifyOk.Location = new System.Drawing.Point(473, 6);
            this.button_modifyOk.Name = "button_modifyOk";
            this.button_modifyOk.Size = new System.Drawing.Size(75, 23);
            this.button_modifyOk.TabIndex = 3;
            this.button_modifyOk.Text = "저장";
            this.button_modifyOk.UseVisualStyleBackColor = true;
            this.button_modifyOk.Click += new System.EventHandler(this.button_modifyOk_Click);
            // 
            // dataGridView_user
            // 
            this.dataGridView_user.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_user.Location = new System.Drawing.Point(0, 33);
            this.dataGridView_user.Name = "dataGridView_user";
            this.dataGridView_user.RowTemplate.Height = 23;
            this.dataGridView_user.Size = new System.Drawing.Size(637, 367);
            this.dataGridView_user.TabIndex = 1;
            // 
            // tabPage_userGroup
            // 
            this.tabPage_userGroup.Controls.Add(this.btnSearch);
            this.tabPage_userGroup.Controls.Add(this.btnApply);
            this.tabPage_userGroup.Controls.Add(this.gridUserGroup);
            this.tabPage_userGroup.Location = new System.Drawing.Point(4, 22);
            this.tabPage_userGroup.Name = "tabPage_userGroup";
            this.tabPage_userGroup.Size = new System.Drawing.Size(637, 400);
            this.tabPage_userGroup.TabIndex = 2;
            this.tabPage_userGroup.Text = "사용자 그룹";
            this.tabPage_userGroup.UseVisualStyleBackColor = true;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(554, 6);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 6;
            this.btnSearch.Text = "조회";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(473, 6);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 6;
            this.btnApply.Text = "저장";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // gridUserGroup
            // 
            this.gridUserGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridUserGroup.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(222)))), ((int)(((byte)(239)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridUserGroup.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridUserGroup.ColumnHeadersHeight = 40;
            this.gridUserGroup.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colName,
            this.colPipe,
            this.colPipeAlarm,
            this.colLiquidType,
            this.colMass,
            this.colLevel,
            this.colLevelRange,
            this.colGravity,
            this.colFlow,
            this.colTemp,
            this.colTempRange,
            this.colTankAlarm,
            this.colNotice,
            this.colLeak});
            this.gridUserGroup.EnableHeadersVisualStyles = false;
            this.gridUserGroup.Location = new System.Drawing.Point(0, 33);
            this.gridUserGroup.Name = "gridUserGroup";
            this.gridUserGroup.RowHeadersVisible = false;
            this.gridUserGroup.RowTemplate.Height = 23;
            this.gridUserGroup.Size = new System.Drawing.Size(637, 367);
            this.gridUserGroup.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.메뉴ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(645, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 메뉴ToolStripMenuItem
            // 
            this.메뉴ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.관리자변경ToolStripMenuItem,
            this.비밀번호변경ToolStripMenuItem,
            this.종료ToolStripMenuItem});
            this.메뉴ToolStripMenuItem.Name = "메뉴ToolStripMenuItem";
            this.메뉴ToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.메뉴ToolStripMenuItem.Text = "메뉴";
            // 
            // 관리자변경ToolStripMenuItem
            // 
            this.관리자변경ToolStripMenuItem.Name = "관리자변경ToolStripMenuItem";
            this.관리자변경ToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.관리자변경ToolStripMenuItem.Text = "관리자 변경";
            this.관리자변경ToolStripMenuItem.Click += new System.EventHandler(this.관리자변경ToolStripMenuItem_Click);
            // 
            // 비밀번호변경ToolStripMenuItem
            // 
            this.비밀번호변경ToolStripMenuItem.Name = "비밀번호변경ToolStripMenuItem";
            this.비밀번호변경ToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.비밀번호변경ToolStripMenuItem.Text = "비밀번호 변경";
            this.비밀번호변경ToolStripMenuItem.Click += new System.EventHandler(this.비밀번호변경ToolStripMenuItem_Click);
            // 
            // 종료ToolStripMenuItem
            // 
            this.종료ToolStripMenuItem.Name = "종료ToolStripMenuItem";
            this.종료ToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.종료ToolStripMenuItem.Text = "종료";
            this.종료ToolStripMenuItem.Click += new System.EventHandler(this.종료ToolStripMenuItem_Click);
            // 
            // menuUserGroup
            // 
            this.menuUserGroup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuSelectAllTankItems,
            this.tsMenuUnselectAllTankItems,
            this.tsMenuSelectAllTanks,
            this.tsMenuUnselectAllTanks,
            this.tsMenuDeleteSelectedUserGroups});
            this.menuUserGroup.Name = "menuUserGroup";
            this.menuUserGroup.Size = new System.Drawing.Size(207, 114);
            // 
            // tsMenuSelectAllTankItems
            // 
            this.tsMenuSelectAllTankItems.Name = "tsMenuSelectAllTankItems";
            this.tsMenuSelectAllTankItems.Size = new System.Drawing.Size(206, 22);
            this.tsMenuSelectAllTankItems.Text = "탱크 아이템 모두 선택";
            // 
            // tsMenuUnselectAllTankItems
            // 
            this.tsMenuUnselectAllTankItems.Name = "tsMenuUnselectAllTankItems";
            this.tsMenuUnselectAllTankItems.Size = new System.Drawing.Size(206, 22);
            this.tsMenuUnselectAllTankItems.Text = "탱크 아이템 모두 해제";
            // 
            // tsMenuSelectAllTanks
            // 
            this.tsMenuSelectAllTanks.Name = "tsMenuSelectAllTanks";
            this.tsMenuSelectAllTanks.Size = new System.Drawing.Size(206, 22);
            this.tsMenuSelectAllTanks.Text = "모든 탱크 선택";
            // 
            // tsMenuUnselectAllTanks
            // 
            this.tsMenuUnselectAllTanks.Name = "tsMenuUnselectAllTanks";
            this.tsMenuUnselectAllTanks.Size = new System.Drawing.Size(206, 22);
            this.tsMenuUnselectAllTanks.Text = "모든 탱크 해제";
            // 
            // tsMenuDeleteSelectedUserGroups
            // 
            this.tsMenuDeleteSelectedUserGroups.Name = "tsMenuDeleteSelectedUserGroups";
            this.tsMenuDeleteSelectedUserGroups.Size = new System.Drawing.Size(206, 22);
            this.tsMenuDeleteSelectedUserGroups.Text = "선택된 사용자 그룹 삭제";
            // 
            // menuUser
            // 
            this.menuUser.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuDeleteSelectedUsers});
            this.menuUser.Name = "menuUser";
            this.menuUser.Size = new System.Drawing.Size(179, 26);
            // 
            // tsMenuDeleteSelectedUsers
            // 
            this.tsMenuDeleteSelectedUsers.Name = "tsMenuDeleteSelectedUsers";
            this.tsMenuDeleteSelectedUsers.Size = new System.Drawing.Size(178, 22);
            this.tsMenuDeleteSelectedUsers.Text = "선택된 사용자 삭제";
            // 
            // colNo
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle2;
            this.colNo.HeaderText = "번호";
            this.colNo.Name = "colNo";
            this.colNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNo.Width = 40;
            // 
            // colName
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colName.DefaultCellStyle = dataGridViewCellStyle3;
            this.colName.HeaderText = "그룹명";
            this.colName.Name = "colName";
            this.colName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colName.Width = 150;
            // 
            // colPipe
            // 
            this.colPipe.HeaderText = "배관";
            this.colPipe.Name = "colPipe";
            this.colPipe.Width = 40;
            // 
            // colPipeAlarm
            // 
            this.colPipeAlarm.HeaderText = "배관알람";
            this.colPipeAlarm.Name = "colPipeAlarm";
            this.colPipeAlarm.Width = 60;
            // 
            // colLiquidType
            // 
            this.colLiquidType.HeaderText = "유종";
            this.colLiquidType.Name = "colLiquidType";
            this.colLiquidType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colLiquidType.Width = 40;
            // 
            // colMass
            // 
            this.colMass.HeaderText = "재고";
            this.colMass.Name = "colMass";
            this.colMass.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colMass.Width = 40;
            // 
            // colLevel
            // 
            this.colLevel.HeaderText = "레벨";
            this.colLevel.Name = "colLevel";
            this.colLevel.Width = 40;
            // 
            // colLevelRange
            // 
            this.colLevelRange.HeaderText = "레벨한계";
            this.colLevelRange.Name = "colLevelRange";
            this.colLevelRange.Width = 60;
            // 
            // colGravity
            // 
            this.colGravity.HeaderText = "비중";
            this.colGravity.Name = "colGravity";
            this.colGravity.Width = 40;
            // 
            // colFlow
            // 
            this.colFlow.HeaderText = "유량";
            this.colFlow.Name = "colFlow";
            this.colFlow.Width = 40;
            // 
            // colTemp
            // 
            this.colTemp.HeaderText = "온도";
            this.colTemp.Name = "colTemp";
            this.colTemp.Width = 40;
            // 
            // colTempRange
            // 
            this.colTempRange.HeaderText = "온도범위";
            this.colTempRange.Name = "colTempRange";
            this.colTempRange.Width = 60;
            // 
            // colTankAlarm
            // 
            this.colTankAlarm.HeaderText = "탱크알람";
            this.colTankAlarm.Name = "colTankAlarm";
            this.colTankAlarm.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colTankAlarm.Width = 60;
            // 
            // colNotice
            // 
            this.colNotice.HeaderText = "공지사항";
            this.colNotice.Name = "colNotice";
            this.colNotice.Width = 60;
            // 
            // colLeak
            // 
            this.colLeak.HeaderText = "황산누출";
            this.colLeak.Name = "colLeak";
            this.colLeak.Width = 60;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(645, 452);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "사용자 승인 관리";
            this.tabControl1.ResumeLayout(false);
            this.tabPage_wait.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_wait)).EndInit();
            this.tabPage_user.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_user)).EndInit();
            this.tabPage_userGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridUserGroup)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.menuUserGroup.ResumeLayout(false);
            this.menuUser.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage_wait;
        private System.Windows.Forms.TabPage tabPage_user;
        private System.Windows.Forms.DataGridView dataGridView_wait;
        private System.Windows.Forms.Button button_watiSearch;
        private System.Windows.Forms.Button button_waitOk;
        private System.Windows.Forms.DataGridView dataGridView_user;
        private System.Windows.Forms.Button button_modifySearch;
        private System.Windows.Forms.Button button_modifyOk;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 메뉴ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 관리자변경ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 비밀번호변경ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 종료ToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage_userGroup;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.DataGridView gridUserGroup;
        private System.Windows.Forms.ContextMenuStrip menuUserGroup;
        private System.Windows.Forms.ToolStripMenuItem tsMenuSelectAllTankItems;
        private System.Windows.Forms.ToolStripMenuItem tsMenuUnselectAllTankItems;
        private System.Windows.Forms.ToolStripMenuItem tsMenuSelectAllTanks;
        private System.Windows.Forms.ToolStripMenuItem tsMenuUnselectAllTanks;
        private System.Windows.Forms.ToolStripMenuItem tsMenuDeleteSelectedUserGroups;
        private System.Windows.Forms.ContextMenuStrip menuUser;
        private System.Windows.Forms.ToolStripMenuItem tsMenuDeleteSelectedUsers;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colPipe;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colPipeAlarm;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colLiquidType;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colMass;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colLevel;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colLevelRange;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colGravity;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colFlow;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colTemp;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colTempRange;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colTankAlarm;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colNotice;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colLeak;
    }
}

