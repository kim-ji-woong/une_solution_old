namespace SectionContents.Fancy
{
    partial class PanelInternal
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.gridReceivers = new System.Windows.Forms.DataGridView();
            this.colReceiver = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rbtnComplete = new UnE.GUI.RibbonButton();
            this.rbtnSiren = new UnE.GUI.RibbonButton();
            this.rbtnSMS = new UnE.GUI.RibbonButton();
            this.rbtnSpecial = new UnE.GUI.RibbonButton();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuShowReceiverMembers = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuHideReceiverMembers = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.gridReceivers)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxMessage.Enabled = false;
            this.textBoxMessage.Font = new System.Drawing.Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxMessage.ForeColor = System.Drawing.Color.Black;
            this.textBoxMessage.Location = new System.Drawing.Point(30, 73);
            this.textBoxMessage.Multiline = true;
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.Size = new System.Drawing.Size(571, 232);
            this.textBoxMessage.TabIndex = 4;
            // 
            // gridReceivers
            // 
            this.gridReceivers.AllowUserToAddRows = false;
            this.gridReceivers.AllowUserToDeleteRows = false;
            this.gridReceivers.BackgroundColor = System.Drawing.Color.White;
            this.gridReceivers.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridReceivers.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridReceivers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridReceivers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colReceiver});
            this.gridReceivers.Location = new System.Drawing.Point(618, 73);
            this.gridReceivers.Name = "gridReceivers";
            this.gridReceivers.ReadOnly = true;
            this.gridReceivers.RowHeadersVisible = false;
            this.gridReceivers.RowTemplate.Height = 23;
            this.gridReceivers.Size = new System.Drawing.Size(166, 232);
            this.gridReceivers.TabIndex = 5;
            this.gridReceivers.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grid_MouseDown);
            // 
            // colReceiver
            // 
            this.colReceiver.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.colReceiver.DefaultCellStyle = dataGridViewCellStyle2;
            this.colReceiver.HeaderText = "수신자";
            this.colReceiver.Name = "colReceiver";
            this.colReceiver.ReadOnly = true;
            this.colReceiver.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // rbtnComplete
            // 
            this.rbtnComplete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rbtnComplete.BackColor = System.Drawing.Color.Transparent;
            this.rbtnComplete.CheckButton = false;
            this.rbtnComplete.CheckedBkgndImage = null;
            this.rbtnComplete.CheckedImage = global::SectionContents.Properties.Resources.MissionComplete_Checked;
            this.rbtnComplete.CheckedMouseOver = global::SectionContents.Properties.Resources.MissionComplete_Checked_MouseOver;
            this.rbtnComplete.ClickedBackgroundImage = null;
            this.rbtnComplete.ClickedImage = null;
            this.rbtnComplete.CustomImageRect = new System.Drawing.Rectangle(0, 0, 30, 30);
            this.rbtnComplete.DisabledBkgndImage = null;
            this.rbtnComplete.DisabledImage = global::SectionContents.Properties.Resources.MissionComplete_Unchecked_Disabled;
            this.rbtnComplete.Enabled = false;
            this.rbtnComplete.ForeColor = System.Drawing.Color.White;
            this.rbtnComplete.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnComplete.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnComplete.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnComplete.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnComplete.ForeColorsByTypeUse = false;
            this.rbtnComplete.ID = -1;
            this.rbtnComplete.InitButtonWidth = 30;
            this.rbtnComplete.IsChecked = false;
            this.rbtnComplete.Location = new System.Drawing.Point(754, 15);
            this.rbtnComplete.MouseOverBkgndImage = null;
            this.rbtnComplete.MouseOverImage = global::SectionContents.Properties.Resources.MissionComplete_Unchecked_MouseOver;
            this.rbtnComplete.Name = "rbtnComplete";
            this.rbtnComplete.NormalImage = global::SectionContents.Properties.Resources.MissionComplete_Unchecked;
            this.rbtnComplete.Owner = null;
            this.rbtnComplete.Size = new System.Drawing.Size(30, 30);
            this.rbtnComplete.TabIndex = 3;
            this.rbtnComplete.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnComplete.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnComplete.ToolTipText = "";
            this.rbtnComplete.UseCustomImageRect = true;
            this.rbtnComplete.UseTextLocation = false;
            this.rbtnComplete.UseVisualStyleBackColor = false;
            this.rbtnComplete.Click += new System.EventHandler(this.rbtnComplete_Click);
            // 
            // rbtnSiren
            // 
            this.rbtnSiren.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rbtnSiren.BackColor = System.Drawing.Color.Transparent;
            this.rbtnSiren.CheckButton = false;
            this.rbtnSiren.CheckedBkgndImage = null;
            this.rbtnSiren.CheckedImage = global::SectionContents.Properties.Resources.SirenUse_Selected;
            this.rbtnSiren.CheckedMouseOver = global::SectionContents.Properties.Resources.SirenUse_Selected_MouseOver;
            this.rbtnSiren.ClickedBackgroundImage = null;
            this.rbtnSiren.ClickedImage = null;
            this.rbtnSiren.CustomImageRect = new System.Drawing.Rectangle(0, 0, 22, 26);
            this.rbtnSiren.DisabledBkgndImage = null;
            this.rbtnSiren.DisabledImage = global::SectionContents.Properties.Resources.SirenNoUse_Disabled;
            this.rbtnSiren.Enabled = false;
            this.rbtnSiren.ForeColor = System.Drawing.Color.White;
            this.rbtnSiren.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnSiren.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnSiren.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnSiren.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnSiren.ForeColorsByTypeUse = false;
            this.rbtnSiren.ID = -1;
            this.rbtnSiren.InitButtonWidth = 22;
            this.rbtnSiren.IsChecked = false;
            this.rbtnSiren.Location = new System.Drawing.Point(587, 15);
            this.rbtnSiren.MouseOverBkgndImage = null;
            this.rbtnSiren.MouseOverImage = global::SectionContents.Properties.Resources.SirenNoUse_Selected_MouseOver;
            this.rbtnSiren.Name = "rbtnSiren";
            this.rbtnSiren.NormalImage = global::SectionContents.Properties.Resources.SirenNoUse_Selected;
            this.rbtnSiren.Owner = null;
            this.rbtnSiren.Size = new System.Drawing.Size(22, 63);
            this.rbtnSiren.TabIndex = 3;
            this.rbtnSiren.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnSiren.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnSiren.ToolTipText = "";
            this.rbtnSiren.UseCustomImageRect = true;
            this.rbtnSiren.UseTextLocation = false;
            this.rbtnSiren.UseVisualStyleBackColor = false;
            this.rbtnSiren.Visible = false;
            this.rbtnSiren.Click += new System.EventHandler(this.rbtnSiren_Click);
            // 
            // rbtnSMS
            // 
            this.rbtnSMS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rbtnSMS.BackColor = System.Drawing.Color.Transparent;
            this.rbtnSMS.CheckButton = false;
            this.rbtnSMS.CheckedBkgndImage = null;
            this.rbtnSMS.CheckedImage = null;
            this.rbtnSMS.CheckedMouseOver = null;
            this.rbtnSMS.ClickedBackgroundImage = null;
            this.rbtnSMS.ClickedImage = null;
            this.rbtnSMS.CustomImageRect = new System.Drawing.Rectangle(0, 0, 33, 24);
            this.rbtnSMS.DisabledBkgndImage = null;
            this.rbtnSMS.DisabledImage = global::SectionContents.Properties.Resources.SMS_Disabled;
            this.rbtnSMS.Enabled = false;
            this.rbtnSMS.ForeColor = System.Drawing.Color.White;
            this.rbtnSMS.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnSMS.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnSMS.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnSMS.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnSMS.ForeColorsByTypeUse = false;
            this.rbtnSMS.ID = -1;
            this.rbtnSMS.InitButtonWidth = 33;
            this.rbtnSMS.IsChecked = false;
            this.rbtnSMS.Location = new System.Drawing.Point(705, 18);
            this.rbtnSMS.MouseOverBkgndImage = null;
            this.rbtnSMS.MouseOverImage = global::SectionContents.Properties.Resources.SMS_Selected_MouseOver;
            this.rbtnSMS.Name = "rbtnSMS";
            this.rbtnSMS.NormalImage = global::SectionContents.Properties.Resources.SMS_Selected;
            this.rbtnSMS.Owner = null;
            this.rbtnSMS.Size = new System.Drawing.Size(33, 24);
            this.rbtnSMS.TabIndex = 3;
            this.rbtnSMS.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnSMS.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnSMS.ToolTipText = "";
            this.rbtnSMS.UseCustomImageRect = true;
            this.rbtnSMS.UseTextLocation = false;
            this.rbtnSMS.UseVisualStyleBackColor = false;
            this.rbtnSMS.Click += new System.EventHandler(this.rbtnSMS_Click);
            // 
            // rbtnSpecial
            // 
            this.rbtnSpecial.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rbtnSpecial.BackColor = System.Drawing.Color.Transparent;
            this.rbtnSpecial.CheckButton = false;
            this.rbtnSpecial.CheckedBkgndImage = null;
            this.rbtnSpecial.CheckedImage = null;
            this.rbtnSpecial.CheckedMouseOver = null;
            this.rbtnSpecial.ClickedBackgroundImage = null;
            this.rbtnSpecial.ClickedImage = null;
            this.rbtnSpecial.CustomImageRect = new System.Drawing.Rectangle(0, 0, 25, 25);
            this.rbtnSpecial.DisabledBkgndImage = null;
            this.rbtnSpecial.DisabledImage = global::SectionContents.Properties.Resources.Special_Disabled;
            this.rbtnSpecial.ForeColor = System.Drawing.Color.White;
            this.rbtnSpecial.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnSpecial.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.rbtnSpecial.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnSpecial.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnSpecial.ForeColorsByTypeUse = false;
            this.rbtnSpecial.ID = -1;
            this.rbtnSpecial.InitButtonWidth = 25;
            this.rbtnSpecial.IsChecked = false;
            this.rbtnSpecial.Location = new System.Drawing.Point(661, 17);
            this.rbtnSpecial.MouseOverBkgndImage = null;
            this.rbtnSpecial.MouseOverImage = global::SectionContents.Properties.Resources.Special_Selected_MouseOver;
            this.rbtnSpecial.Name = "rbtnSpecial";
            this.rbtnSpecial.NormalImage = global::SectionContents.Properties.Resources.Special_Selected;
            this.rbtnSpecial.Owner = null;
            this.rbtnSpecial.Size = new System.Drawing.Size(25, 25);
            this.rbtnSpecial.TabIndex = 3;
            this.rbtnSpecial.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnSpecial.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbtnSpecial.ToolTipText = "";
            this.rbtnSpecial.UseCustomImageRect = true;
            this.rbtnSpecial.UseTextLocation = false;
            this.rbtnSpecial.UseVisualStyleBackColor = false;
            this.rbtnSpecial.Click += new System.EventHandler(this.rbtnSpecial_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuShowReceiverMembers,
            this.tsMenuHideReceiverMembers});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(151, 48);
            // 
            // tsMenuShowReceiverMembers
            // 
            this.tsMenuShowReceiverMembers.Name = "tsMenuShowReceiverMembers";
            this.tsMenuShowReceiverMembers.Size = new System.Drawing.Size(150, 22);
            this.tsMenuShowReceiverMembers.Text = "상세정보 보기";
            this.tsMenuShowReceiverMembers.Click += new System.EventHandler(this.tsMenuReceiverMembers_Click);
            // 
            // tsMenuHideReceiverMembers
            // 
            this.tsMenuHideReceiverMembers.Name = "tsMenuHideReceiverMembers";
            this.tsMenuHideReceiverMembers.Size = new System.Drawing.Size(150, 22);
            this.tsMenuHideReceiverMembers.Text = "상세정보 닫기";
            this.tsMenuHideReceiverMembers.Click += new System.EventHandler(this.tsMenuReceiverMembers_Click);
            // 
            // PanelInternal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.gridReceivers);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.rbtnComplete);
            this.Controls.Add(this.rbtnSiren);
            this.Controls.Add(this.rbtnSMS);
            this.Controls.Add(this.rbtnSpecial);
            this.Name = "PanelInternal";
            this.Size = new System.Drawing.Size(800, 315);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelInternal_Paint);
            this.Resize += new System.EventHandler(this.PanelInternal_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.gridReceivers)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UnE.GUI.RibbonButton rbtnSpecial;
        private UnE.GUI.RibbonButton rbtnSMS;
        private UnE.GUI.RibbonButton rbtnComplete;
        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.DataGridView gridReceivers;
        private System.Windows.Forms.DataGridViewTextBoxColumn colReceiver;
        private UnE.GUI.RibbonButton rbtnSiren;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsMenuShowReceiverMembers;
        private System.Windows.Forms.ToolStripMenuItem tsMenuHideReceiverMembers;
    }
}
