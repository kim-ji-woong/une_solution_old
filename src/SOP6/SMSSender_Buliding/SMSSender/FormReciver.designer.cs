namespace SMSSender
{
    partial class FormReciver
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
            this.treeViewTeam = new System.Windows.Forms.TreeView();
            this.rbBtnExternal = new System.Windows.Forms.RadioButton();
            this.rbBtnRegular = new System.Windows.Forms.RadioButton();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.listReciver = new System.Windows.Forms.ListBox();
            this.labelMemberPath = new System.Windows.Forms.Label();
            this.plTitle = new System.Windows.Forms.Panel();
            this.lbTitle = new System.Windows.Forms.Label();
            this.btnCancel = new UnE.GUI.ImageButton();
            this.btnOK = new UnE.GUI.ImageButton();
            this.btnAddManual = new UnE.GUI.ImageButton();
            this.btnRemove = new UnE.GUI.ImageButton();
            this.btnSelectAll = new UnE.GUI.ImageButton();
            this.btnAdd = new UnE.GUI.ImageButton();
            this.btnCancle = new UnE.GUI.RibbonButton();
            this.pbTitle = new System.Windows.Forms.PictureBox();
            this.btnSearch = new UnE.GUI.RibbonButton();
            this.plTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAddManual)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRemove)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSelectAll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAdd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTitle)).BeginInit();
            this.SuspendLayout();
            // 
            // treeViewTeam
            // 
            this.treeViewTeam.BackColor = System.Drawing.Color.White;
            this.treeViewTeam.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeViewTeam.Font = new System.Drawing.Font("나눔바른고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeViewTeam.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(121)))), ((int)(((byte)(121)))));
            this.treeViewTeam.HideSelection = false;
            this.treeViewTeam.Location = new System.Drawing.Point(15, 113);
            this.treeViewTeam.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.treeViewTeam.MinimumSize = new System.Drawing.Size(378, 300);
            this.treeViewTeam.Name = "treeViewTeam";
            this.treeViewTeam.Size = new System.Drawing.Size(450, 375);
            this.treeViewTeam.TabIndex = 5;
            this.treeViewTeam.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewTeam_AfterSelect);
            this.treeViewTeam.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewTeam_NodeMouseDoubleClick);
            // 
            // rbBtnExternal
            // 
            this.rbBtnExternal.AutoSize = true;
            this.rbBtnExternal.Font = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnExternal.Location = new System.Drawing.Point(99, 75);
            this.rbBtnExternal.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbBtnExternal.Name = "rbBtnExternal";
            this.rbBtnExternal.Size = new System.Drawing.Size(73, 19);
            this.rbBtnExternal.TabIndex = 9;
            this.rbBtnExternal.Text = "외부조직";
            this.rbBtnExternal.UseVisualStyleBackColor = true;
            this.rbBtnExternal.CheckedChanged += new System.EventHandler(this.rbBtnExternal_CheckedChanged);
            // 
            // rbBtnRegular
            // 
            this.rbBtnRegular.AutoSize = true;
            this.rbBtnRegular.Checked = true;
            this.rbBtnRegular.Font = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnRegular.Location = new System.Drawing.Point(15, 75);
            this.rbBtnRegular.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbBtnRegular.Name = "rbBtnRegular";
            this.rbBtnRegular.Size = new System.Drawing.Size(73, 19);
            this.rbBtnRegular.TabIndex = 6;
            this.rbBtnRegular.TabStop = true;
            this.rbBtnRegular.Text = "정규조직";
            this.rbBtnRegular.UseVisualStyleBackColor = true;
            this.rbBtnRegular.CheckedChanged += new System.EventHandler(this.rbBtnRegular_CheckedChanged);
            // 
            // txtSearch
            // 
            this.txtSearch.Font = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtSearch.Location = new System.Drawing.Point(232, 73);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(135, 22);
            this.txtSearch.TabIndex = 29;
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            // 
            // listReciver
            // 
            this.listReciver.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.listReciver.FormattingEnabled = true;
            this.listReciver.ItemHeight = 14;
            this.listReciver.Location = new System.Drawing.Point(622, 113);
            this.listReciver.Name = "listReciver";
            this.listReciver.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listReciver.Size = new System.Drawing.Size(187, 368);
            this.listReciver.TabIndex = 31;
            // 
            // labelMemberPath
            // 
            this.labelMemberPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelMemberPath.AutoSize = true;
            this.labelMemberPath.Location = new System.Drawing.Point(15, 497);
            this.labelMemberPath.Name = "labelMemberPath";
            this.labelMemberPath.Size = new System.Drawing.Size(115, 15);
            this.labelMemberPath.TabIndex = 37;
            this.labelMemberPath.Text = "선택 직원 부서 정보";
            // 
            // plTitle
            // 
            this.plTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(65)))), ((int)(((byte)(109)))));
            this.plTitle.Controls.Add(this.btnCancle);
            this.plTitle.Controls.Add(this.pbTitle);
            this.plTitle.Controls.Add(this.lbTitle);
            this.plTitle.Location = new System.Drawing.Point(0, 0);
            this.plTitle.Name = "plTitle";
            this.plTitle.Size = new System.Drawing.Size(825, 60);
            this.plTitle.TabIndex = 40;
            this.plTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plTitle_MouseDown);
            this.plTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plTitle_MouseMove);
            this.plTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.plTitle_MouseUp);
            // 
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(65)))), ((int)(((byte)(109)))));
            this.lbTitle.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbTitle.ForeColor = System.Drawing.Color.White;
            this.lbTitle.Location = new System.Drawing.Point(42, 20);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(104, 23);
            this.lbTitle.TabIndex = 40;
            this.lbTitle.Text = "수신자 설정";
            this.lbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseDown);
            this.lbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseMove);
            this.lbTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseUp);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCancel.ButtonText = "취소";
            this.btnCancel.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ForeColor = System.Drawing.Color.Transparent;
            this.btnCancel.ImageClicked = global::SMSSender.Properties.Resources.btnFormReciverClose_Selected;
            this.btnCancel.ImageDisabled = null;
            this.btnCancel.ImageMouseOver = global::SMSSender.Properties.Resources.btnFormReciverClose_MouseOver;
            this.btnCancel.ImageNormal = global::SMSSender.Properties.Resources.btnFormReciverClose_Normal;
            this.btnCancel.Location = new System.Drawing.Point(667, 502);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(146, 45);
            this.btnCancel.TabIndex = 46;
            this.btnCancel.TabStop = false;
            this.btnCancel.Text = "취소";
            this.btnCancel.TextColor = System.Drawing.Color.Black;
            this.btnCancel.TextFont = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ToolTipText = "";
            this.btnCancel.UseToolTip = false;
            this.btnCancel.WindowRateWidth = 1F;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.Transparent;
            this.btnOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnOK.ButtonText = "확인";
            this.btnOK.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.ForeColor = System.Drawing.Color.Transparent;
            this.btnOK.ImageClicked = global::SMSSender.Properties.Resources.btnFormReciverOK_Selected;
            this.btnOK.ImageDisabled = null;
            this.btnOK.ImageMouseOver = global::SMSSender.Properties.Resources.btnFormReciverOK_MouseOver;
            this.btnOK.ImageNormal = global::SMSSender.Properties.Resources.btnFormReciverOK_Normal;
            this.btnOK.Location = new System.Drawing.Point(512, 502);
            this.btnOK.Name = "btnOK";
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(146, 45);
            this.btnOK.TabIndex = 47;
            this.btnOK.TabStop = false;
            this.btnOK.Text = "확인";
            this.btnOK.TextColor = System.Drawing.Color.White;
            this.btnOK.TextFont = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.ToolTipText = "";
            this.btnOK.UseToolTip = false;
            this.btnOK.WindowRateWidth = 1F;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnAddManual
            // 
            this.btnAddManual.BackColor = System.Drawing.SystemColors.Control;
            this.btnAddManual.ButtonText = "직접추가";
            this.btnAddManual.ImageClicked = global::SMSSender.Properties.Resources.btnDelete_Selected;
            this.btnAddManual.ImageDisabled = null;
            this.btnAddManual.ImageMouseOver = global::SMSSender.Properties.Resources.btnDelete_Selected;
            this.btnAddManual.ImageNormal = global::SMSSender.Properties.Resources.btnDelete_Normal;
            this.btnAddManual.Location = new System.Drawing.Point(487, 365);
            this.btnAddManual.Name = "btnAddManual";
            this.btnAddManual.Owner = null;
            this.btnAddManual.Size = new System.Drawing.Size(112, 33);
            this.btnAddManual.TabIndex = 45;
            this.btnAddManual.TabStop = false;
            this.btnAddManual.Text = "직접추가";
            this.btnAddManual.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.btnAddManual.TextFont = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnAddManual.ToolTipText = "";
            this.btnAddManual.UseToolTip = false;
            this.btnAddManual.WindowRateWidth = 1F;
            this.btnAddManual.Click += new System.EventHandler(this.btnAddManual_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.BackColor = System.Drawing.SystemColors.Control;
            this.btnRemove.ButtonText = "선택삭제";
            this.btnRemove.ImageClicked = global::SMSSender.Properties.Resources.btnDelete_Selected;
            this.btnRemove.ImageDisabled = null;
            this.btnRemove.ImageMouseOver = global::SMSSender.Properties.Resources.btnDelete_Selected;
            this.btnRemove.ImageNormal = global::SMSSender.Properties.Resources.btnDelete_Normal;
            this.btnRemove.Location = new System.Drawing.Point(487, 316);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Owner = null;
            this.btnRemove.Size = new System.Drawing.Size(112, 33);
            this.btnRemove.TabIndex = 44;
            this.btnRemove.TabStop = false;
            this.btnRemove.Text = "선택삭제";
            this.btnRemove.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.btnRemove.TextFont = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRemove.ToolTipText = "";
            this.btnRemove.UseToolTip = false;
            this.btnRemove.WindowRateWidth = 1F;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.BackColor = System.Drawing.SystemColors.Control;
            this.btnSelectAll.ButtonText = "전체선택";
            this.btnSelectAll.ImageClicked = global::SMSSender.Properties.Resources.btnDelete_Selected;
            this.btnSelectAll.ImageDisabled = null;
            this.btnSelectAll.ImageMouseOver = global::SMSSender.Properties.Resources.btnDelete_Selected;
            this.btnSelectAll.ImageNormal = global::SMSSender.Properties.Resources.btnDelete_Normal;
            this.btnSelectAll.Location = new System.Drawing.Point(487, 267);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Owner = null;
            this.btnSelectAll.Size = new System.Drawing.Size(112, 33);
            this.btnSelectAll.TabIndex = 43;
            this.btnSelectAll.TabStop = false;
            this.btnSelectAll.Text = "전체선택";
            this.btnSelectAll.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.btnSelectAll.TextFont = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSelectAll.ToolTipText = "";
            this.btnSelectAll.UseToolTip = false;
            this.btnSelectAll.WindowRateWidth = 1F;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.Control;
            this.btnAdd.ButtonText = "추가하기";
            this.btnAdd.ImageClicked = global::SMSSender.Properties.Resources.btnDelete_Selected;
            this.btnAdd.ImageDisabled = null;
            this.btnAdd.ImageMouseOver = global::SMSSender.Properties.Resources.btnDelete_Selected;
            this.btnAdd.ImageNormal = global::SMSSender.Properties.Resources.btnDelete_Normal;
            this.btnAdd.Location = new System.Drawing.Point(487, 218);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Owner = null;
            this.btnAdd.Size = new System.Drawing.Size(112, 33);
            this.btnAdd.TabIndex = 42;
            this.btnAdd.TabStop = false;
            this.btnAdd.Text = "추가하기";
            this.btnAdd.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.btnAdd.TextFont = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnAdd.ToolTipText = "";
            this.btnAdd.UseToolTip = false;
            this.btnAdd.WindowRateWidth = 1F;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCancle
            // 
            this.btnCancle.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(65)))), ((int)(((byte)(109)))));
            this.btnCancle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCancle.CheckButton = false;
            this.btnCancle.CheckedBkgndImage = null;
            this.btnCancle.CheckedImage = null;
            this.btnCancle.CheckedMouseOver = null;
            this.btnCancle.ClickedBackgroundImage = null;
            this.btnCancle.ClickedImage = global::SMSSender.Properties.Resources.btnClose_Selected;
            this.btnCancle.CustomImageRect = new System.Drawing.Rectangle(0, 0, 22, 22);
            this.btnCancle.DisabledBkgndImage = null;
            this.btnCancle.DisabledImage = null;
            this.btnCancle.ForeColorChecked = System.Drawing.Color.White;
            this.btnCancle.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnCancle.ForeColorDisabled = System.Drawing.Color.White;
            this.btnCancle.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnCancle.ForeColorsByTypeUse = false;
            this.btnCancle.ID = -1;
            this.btnCancle.InitButtonWidth = 22;
            this.btnCancle.IsChecked = false;
            this.btnCancle.Location = new System.Drawing.Point(789, 19);
            this.btnCancle.MouseOverBkgndImage = null;
            this.btnCancle.MouseOverImage = global::SMSSender.Properties.Resources.btnClose_MouseOver;
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.NormalImage = global::SMSSender.Properties.Resources.btnClose_Normal;
            this.btnCancle.Owner = null;
            this.btnCancle.Size = new System.Drawing.Size(22, 22);
            this.btnCancle.TabIndex = 110;
            this.btnCancle.TextLocation = new System.Drawing.Point(0, 0);
            this.btnCancle.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCancle.ToolTipText = "";
            this.btnCancle.UseCustomImageRect = false;
            this.btnCancle.UseTextLocation = false;
            this.btnCancle.UseVisualStyleBackColor = false;
            this.btnCancle.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pbTitle
            // 
            this.pbTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(140)))), ((int)(((byte)(255)))));
            this.pbTitle.Location = new System.Drawing.Point(22, 28);
            this.pbTitle.Margin = new System.Windows.Forms.Padding(0);
            this.pbTitle.Name = "pbTitle";
            this.pbTitle.Size = new System.Drawing.Size(5, 5);
            this.pbTitle.TabIndex = 39;
            this.pbTitle.TabStop = false;
            this.pbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbTitle_MouseDown);
            this.pbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbTitle_MouseMove);
            this.pbTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbTitle_MouseUp);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.Transparent;
            this.btnSearch.CheckButton = false;
            this.btnSearch.CheckedBkgndImage = null;
            this.btnSearch.CheckedImage = null;
            this.btnSearch.CheckedMouseOver = null;
            this.btnSearch.ClickedBackgroundImage = null;
            this.btnSearch.ClickedImage = global::SMSSender.Properties.Resources.button_Selected;
            this.btnSearch.CustomImageRect = new System.Drawing.Rectangle(0, 0, 90, 30);
            this.btnSearch.DisabledBkgndImage = null;
            this.btnSearch.DisabledImage = null;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.ForeColorChecked = System.Drawing.Color.White;
            this.btnSearch.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnSearch.ForeColorDisabled = System.Drawing.Color.White;
            this.btnSearch.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnSearch.ForeColorsByTypeUse = false;
            this.btnSearch.ID = -1;
            this.btnSearch.InitButtonWidth = 90;
            this.btnSearch.IsChecked = false;
            this.btnSearch.Location = new System.Drawing.Point(375, 69);
            this.btnSearch.MouseOverBkgndImage = null;
            this.btnSearch.MouseOverImage = global::SMSSender.Properties.Resources.button_MouseOver;
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.NormalImage = global::SMSSender.Properties.Resources.button_Normal;
            this.btnSearch.Owner = null;
            this.btnSearch.Size = new System.Drawing.Size(90, 30);
            this.btnSearch.TabIndex = 128;
            this.btnSearch.Text = "찾기";
            this.btnSearch.TextLocation = new System.Drawing.Point(27, 8);
            this.btnSearch.TextPos = UnE.GUI.RibbonButton.TextPosition.RIGHT;
            this.btnSearch.ToolTipText = "찾기";
            this.btnSearch.UseCustomImageRect = true;
            this.btnSearch.UseTextLocation = true;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // FormReciver
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(825, 563);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnAddManual);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnSelectAll);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.plTitle);
            this.Controls.Add(this.labelMemberPath);
            this.Controls.Add(this.listReciver);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.rbBtnExternal);
            this.Controls.Add(this.rbBtnRegular);
            this.Controls.Add(this.treeViewTeam);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormReciver";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "수신자 설정";
            this.Load += new System.EventHandler(this.FormTeamTree_Load);
            this.plTitle.ResumeLayout(false);
            this.plTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAddManual)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRemove)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSelectAll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAdd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTitle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewTeam;
        private System.Windows.Forms.RadioButton rbBtnExternal;
        private System.Windows.Forms.RadioButton rbBtnRegular;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.ListBox listReciver;
        private System.Windows.Forms.Label labelMemberPath;
        private System.Windows.Forms.Panel plTitle;
        private UnE.GUI.RibbonButton btnCancle;
        private System.Windows.Forms.PictureBox pbTitle;
        private System.Windows.Forms.Label lbTitle;
        private UnE.GUI.ImageButton btnAdd;
        private UnE.GUI.ImageButton btnSelectAll;
        private UnE.GUI.ImageButton btnRemove;
        private UnE.GUI.ImageButton btnAddManual;
        private UnE.GUI.ImageButton btnCancel;
        private UnE.GUI.ImageButton btnOK;
        private UnE.GUI.RibbonButton btnSearch;
    }
}