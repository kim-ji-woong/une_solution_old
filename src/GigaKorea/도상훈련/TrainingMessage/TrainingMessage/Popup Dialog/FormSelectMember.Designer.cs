namespace TrainingMessage.Popup_Dialog
{
    partial class FormSelectMember
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.plTop = new System.Windows.Forms.Panel();
            this.btnClose = new UnE.GUI.ImageButton();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbManager = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gridMember = new System.Windows.Forms.DataGridView();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.btnConfirm = new UnE.GUI.ImageButton();
            this.btnCancle = new UnE.GUI.ImageButton();
            this.plTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridMember)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnConfirm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancle)).BeginInit();
            this.SuspendLayout();
            // 
            // plTop
            // 
            this.plTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(26)))), ((int)(((byte)(47)))));
            this.plTop.Controls.Add(this.btnClose);
            this.plTop.Controls.Add(this.label2);
            this.plTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.plTop.Location = new System.Drawing.Point(0, 0);
            this.plTop.Name = "plTop";
            this.plTop.Size = new System.Drawing.Size(240, 40);
            this.plTop.TabIndex = 6;
            this.plTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            this.plTop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form_MouseMove);
            this.plTop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form_MouseUp);
            // 
            // btnClose
            // 
            this.btnClose.ButtonText = "";
            this.btnClose.ImageClicked = global::TrainingMessage.Properties.Resources.btnClose_Selected;
            this.btnClose.ImageDisabled = null;
            this.btnClose.ImageMouseOver = global::TrainingMessage.Properties.Resources.btnClose_MouseOver;
            this.btnClose.ImageNormal = global::TrainingMessage.Properties.Resources.btnClose_Normal;
            this.btnClose.Location = new System.Drawing.Point(207, 11);
            this.btnClose.Name = "btnClose";
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(20, 20);
            this.btnClose.TabIndex = 56;
            this.btnClose.TabStop = false;
            this.btnClose.TextColor = System.Drawing.Color.Black;
            this.btnClose.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ToolTipText = "";
            this.btnClose.UseToolTip = false;
            this.btnClose.WindowRateWidth = 1F;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(173)))), ((int)(((byte)(254)))));
            this.label2.Location = new System.Drawing.Point(15, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 19);
            this.label2.TabIndex = 5;
            this.label2.Text = "수신자 선택";
            this.label2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            this.label2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form_MouseMove);
            this.label2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form_MouseUp);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.cbManager);
            this.panel1.Controls.Add(this.label1);
            this.panel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel1.Location = new System.Drawing.Point(17, 56);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(205, 32);
            this.panel1.TabIndex = 68;
            // 
            // cbManager
            // 
            this.cbManager.AutoSize = true;
            this.cbManager.Location = new System.Drawing.Point(175, 9);
            this.cbManager.Name = "cbManager";
            this.cbManager.Size = new System.Drawing.Size(15, 14);
            this.cbManager.TabIndex = 71;
            this.cbManager.UseVisualStyleBackColor = true;
            this.cbManager.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(74)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(21, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 61;
            this.label1.Text = "대상자";
            // 
            // gridMember
            // 
            this.gridMember.AllowUserToAddRows = false;
            this.gridMember.BackgroundColor = System.Drawing.Color.White;
            this.gridMember.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gridMember.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.gridMember.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridMember.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.gridMember.ColumnHeadersHeight = 40;
            this.gridMember.ColumnHeadersVisible = false;
            this.gridMember.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colCheck});
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridMember.DefaultCellStyle = dataGridViewCellStyle7;
            this.gridMember.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.gridMember.Location = new System.Drawing.Point(17, 87);
            this.gridMember.Name = "gridMember";
            this.gridMember.RowHeadersVisible = false;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle8.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.gridMember.RowsDefaultCellStyle = dataGridViewCellStyle8;
            this.gridMember.RowTemplate.Height = 50;
            this.gridMember.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridMember.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridMember.Size = new System.Drawing.Size(205, 188);
            this.gridMember.TabIndex = 67;
            this.gridMember.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridMember_CellValueChanged);
            this.gridMember.CurrentCellDirtyStateChanged += new System.EventHandler(this.gridMember_CurrentCellDirtyStateChanged);
            // 
            // colName
            // 
            this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            this.colName.DefaultCellStyle = dataGridViewCellStyle6;
            this.colName.HeaderText = "닉네임";
            this.colName.Name = "colName";
            this.colName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colCheck
            // 
            this.colCheck.FalseValue = "False";
            this.colCheck.HeaderText = "비고";
            this.colCheck.Name = "colCheck";
            this.colCheck.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colCheck.TrueValue = "True";
            this.colCheck.Width = 40;
            // 
            // btnConfirm
            // 
            this.btnConfirm.ButtonText = "";
            this.btnConfirm.ImageClicked = global::TrainingMessage.Properties.Resources.btnConfirm_Click;
            this.btnConfirm.ImageDisabled = null;
            this.btnConfirm.ImageMouseOver = global::TrainingMessage.Properties.Resources.btnConfirm_Hover;
            this.btnConfirm.ImageNormal = global::TrainingMessage.Properties.Resources.btnConfirm_Normal;
            this.btnConfirm.Location = new System.Drawing.Point(51, 281);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Owner = null;
            this.btnConfirm.Size = new System.Drawing.Size(80, 35);
            this.btnConfirm.TabIndex = 70;
            this.btnConfirm.TabStop = false;
            this.btnConfirm.TextColor = System.Drawing.Color.Black;
            this.btnConfirm.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnConfirm.ToolTipText = "";
            this.btnConfirm.UseToolTip = false;
            this.btnConfirm.WindowRateWidth = 1F;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnCancle
            // 
            this.btnCancle.ButtonText = "";
            this.btnCancle.ImageClicked = global::TrainingMessage.Properties.Resources.btnCancle_Click;
            this.btnCancle.ImageDisabled = null;
            this.btnCancle.ImageMouseOver = global::TrainingMessage.Properties.Resources.btnCancle_Hover;
            this.btnCancle.ImageNormal = global::TrainingMessage.Properties.Resources.btnCancle_Normal;
            this.btnCancle.Location = new System.Drawing.Point(142, 281);
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.Owner = null;
            this.btnCancle.Size = new System.Drawing.Size(80, 35);
            this.btnCancle.TabIndex = 69;
            this.btnCancle.TabStop = false;
            this.btnCancle.TextColor = System.Drawing.Color.Black;
            this.btnCancle.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancle.ToolTipText = "";
            this.btnCancle.UseToolTip = false;
            this.btnCancle.WindowRateWidth = 1F;
            this.btnCancle.Click += new System.EventHandler(this.btnCancle_Click);
            // 
            // FormSelectMember
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(240, 330);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnCancle);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.gridMember);
            this.Controls.Add(this.plTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormSelectMember";
            this.Text = "FormSelectMember";
            this.plTop.ResumeLayout(false);
            this.plTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridMember)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnConfirm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancle)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel plTop;
        private UnE.GUI.ImageButton btnClose;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox cbManager;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView gridMember;
        private UnE.GUI.ImageButton btnConfirm;
        private UnE.GUI.ImageButton btnCancle;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colCheck;
    }
}