
namespace CrisisAlertManager.Popup_Dialog
{
    partial class FormSensorSearch
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.plTop = new System.Windows.Forms.Panel();
            this.pbSensorSearch = new System.Windows.Forms.PictureBox();
            this.btnClose = new UnE.GUI.ImageButton();
            this.plLevel = new System.Windows.Forms.Panel();
            this.cmbLevel = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.gridSensorAddress = new System.Windows.Forms.DataGridView();
            this.colCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSearch = new UnE.GUI.ImageButton();
            this.btnSave = new UnE.GUI.ImageButton();
            this.btnModifityCancle = new UnE.GUI.ImageButton();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.plTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSensorSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.plLevel.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridSensorAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnModifityCancle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // plTop
            // 
            this.plTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.plTop.Controls.Add(this.pbSensorSearch);
            this.plTop.Controls.Add(this.btnClose);
            this.plTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.plTop.Location = new System.Drawing.Point(0, 0);
            this.plTop.Name = "plTop";
            this.plTop.Size = new System.Drawing.Size(800, 60);
            this.plTop.TabIndex = 1;
            // 
            // pbSensorSearch
            // 
            this.pbSensorSearch.BackColor = System.Drawing.Color.Transparent;
            this.pbSensorSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbSensorSearch.Image = global::CrisisAlertManager.Properties.Resources.lbSearch;
            this.pbSensorSearch.Location = new System.Drawing.Point(323, 12);
            this.pbSensorSearch.Name = "pbSensorSearch";
            this.pbSensorSearch.Size = new System.Drawing.Size(130, 36);
            this.pbSensorSearch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbSensorSearch.TabIndex = 76;
            this.pbSensorSearch.TabStop = false;
            // 
            // btnClose
            // 
            this.btnClose.ButtonText = "";
            this.btnClose.ImageClicked = global::CrisisAlertManager.Properties.Resources.btnClose_Selected;
            this.btnClose.ImageDisabled = null;
            this.btnClose.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.btnClose_MouseOver;
            this.btnClose.ImageNormal = global::CrisisAlertManager.Properties.Resources.btnClose_Normal;
            this.btnClose.Location = new System.Drawing.Point(753, 17);
            this.btnClose.Name = "btnClose";
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(25, 25);
            this.btnClose.TabIndex = 55;
            this.btnClose.TabStop = false;
            this.btnClose.TextColor = System.Drawing.Color.Black;
            this.btnClose.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ToolTipText = "";
            this.btnClose.UseToolTip = false;
            this.btnClose.WindowRateWidth = 1F;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // plLevel
            // 
            this.plLevel.BackColor = System.Drawing.Color.White;
            this.plLevel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.plLevel.Controls.Add(this.cmbLevel);
            this.plLevel.Location = new System.Drawing.Point(146, 103);
            this.plLevel.Name = "plLevel";
            this.plLevel.Size = new System.Drawing.Size(100, 26);
            this.plLevel.TabIndex = 82;
            // 
            // cmbLevel
            // 
            this.cmbLevel.BackColor = System.Drawing.SystemColors.Window;
            this.cmbLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLevel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbLevel.FormattingEnabled = true;
            this.cmbLevel.IntegralHeight = false;
            this.cmbLevel.ItemHeight = 18;
            this.cmbLevel.Items.AddRange(new object[] {
            " 알람단계",
            " 평시",
            " 관심",
            " 주의",
            " 경계",
            " 심각"});
            this.cmbLevel.Location = new System.Drawing.Point(-1, -1);
            this.cmbLevel.Name = "cmbLevel";
            this.cmbLevel.Size = new System.Drawing.Size(100, 26);
            this.cmbLevel.TabIndex = 80;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.txtAddress);
            this.panel1.Location = new System.Drawing.Point(261, 103);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(427, 26);
            this.panel1.TabIndex = 83;
            // 
            // txtAddress
            // 
            this.txtAddress.BackColor = System.Drawing.Color.White;
            this.txtAddress.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddress.Location = new System.Drawing.Point(10, 4);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(412, 17);
            this.txtAddress.TabIndex = 59;
            // 
            // gridSensorAddress
            // 
            this.gridSensorAddress.AllowUserToAddRows = false;
            this.gridSensorAddress.AllowUserToResizeColumns = false;
            this.gridSensorAddress.AllowUserToResizeRows = false;
            this.gridSensorAddress.BackgroundColor = System.Drawing.Color.White;
            this.gridSensorAddress.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.gridSensorAddress.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridSensorAddress.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridSensorAddress.ColumnHeadersHeight = 40;
            this.gridSensorAddress.ColumnHeadersVisible = false;
            this.gridSensorAddress.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCheck,
            this.colAddress});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridSensorAddress.DefaultCellStyle = dataGridViewCellStyle2;
            this.gridSensorAddress.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.gridSensorAddress.Location = new System.Drawing.Point(146, 148);
            this.gridSensorAddress.Name = "gridSensorAddress";
            this.gridSensorAddress.ReadOnly = true;
            this.gridSensorAddress.RowHeadersVisible = false;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.gridSensorAddress.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.gridSensorAddress.RowTemplate.Height = 45;
            this.gridSensorAddress.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridSensorAddress.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridSensorAddress.Size = new System.Drawing.Size(622, 238);
            this.gridSensorAddress.TabIndex = 84;
            this.gridSensorAddress.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridSensorAddress_CellClick);
            // 
            // colCheck
            // 
            this.colCheck.FalseValue = "False";
            this.colCheck.HeaderText = "체크";
            this.colCheck.Name = "colCheck";
            this.colCheck.ReadOnly = true;
            this.colCheck.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colCheck.TrueValue = "True";
            this.colCheck.Width = 40;
            // 
            // colAddress
            // 
            this.colAddress.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colAddress.HeaderText = "주소";
            this.colAddress.Name = "colAddress";
            this.colAddress.ReadOnly = true;
            this.colAddress.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colAddress.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // btnSearch
            // 
            this.btnSearch.ButtonText = "";
            this.btnSearch.ImageClicked = global::CrisisAlertManager.Properties.Resources.btnSearch_Click;
            this.btnSearch.ImageDisabled = null;
            this.btnSearch.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.btnSearch_Hover;
            this.btnSearch.ImageNormal = global::CrisisAlertManager.Properties.Resources.btnSearch;
            this.btnSearch.Location = new System.Drawing.Point(708, 102);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Owner = null;
            this.btnSearch.Size = new System.Drawing.Size(60, 27);
            this.btnSearch.TabIndex = 87;
            this.btnSearch.TabStop = false;
            this.btnSearch.TextColor = System.Drawing.Color.Black;
            this.btnSearch.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSearch.ToolTipText = "";
            this.btnSearch.UseToolTip = false;
            this.btnSearch.WindowRateWidth = 1F;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnSave
            // 
            this.btnSave.ButtonText = "";
            this.btnSave.ImageClicked = global::CrisisAlertManager.Properties.Resources.btnSave_Click;
            this.btnSave.ImageDisabled = null;
            this.btnSave.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.btnSave_Hover;
            this.btnSave.ImageNormal = global::CrisisAlertManager.Properties.Resources.btnSave;
            this.btnSave.Location = new System.Drawing.Point(590, 399);
            this.btnSave.Name = "btnSave";
            this.btnSave.Owner = null;
            this.btnSave.Size = new System.Drawing.Size(80, 35);
            this.btnSave.TabIndex = 86;
            this.btnSave.TabStop = false;
            this.btnSave.TextColor = System.Drawing.Color.Black;
            this.btnSave.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSave.ToolTipText = "";
            this.btnSave.UseToolTip = false;
            this.btnSave.WindowRateWidth = 1F;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnModifityCancle
            // 
            this.btnModifityCancle.ButtonText = "";
            this.btnModifityCancle.ImageClicked = global::CrisisAlertManager.Properties.Resources.btnCancle_Click;
            this.btnModifityCancle.ImageDisabled = null;
            this.btnModifityCancle.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.btnCancle_Hover;
            this.btnModifityCancle.ImageNormal = global::CrisisAlertManager.Properties.Resources.btnCancle_Normal;
            this.btnModifityCancle.Location = new System.Drawing.Point(688, 399);
            this.btnModifityCancle.Name = "btnModifityCancle";
            this.btnModifityCancle.Owner = null;
            this.btnModifityCancle.Size = new System.Drawing.Size(80, 35);
            this.btnModifityCancle.TabIndex = 85;
            this.btnModifityCancle.TabStop = false;
            this.btnModifityCancle.TextColor = System.Drawing.Color.Black;
            this.btnModifityCancle.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnModifityCancle.ToolTipText = "";
            this.btnModifityCancle.UseToolTip = false;
            this.btnModifityCancle.WindowRateWidth = 1F;
            this.btnModifityCancle.Click += new System.EventHandler(this.btnModifityCancle_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox2.Image = global::CrisisAlertManager.Properties.Resources.lbResult;
            this.pictureBox2.Location = new System.Drawing.Point(23, 148);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(87, 26);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 78;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Image = global::CrisisAlertManager.Properties.Resources.lbAddress;
            this.pictureBox1.Location = new System.Drawing.Point(23, 102);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(92, 27);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 77;
            this.pictureBox1.TabStop = false;
            // 
            // FormSensorSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnModifityCancle);
            this.Controls.Add(this.gridSensorAddress);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.plLevel);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.plTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormSensorSearch";
            this.Text = "Form1";
            this.plTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbSensorSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.plLevel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridSensorAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnModifityCancle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel plTop;
        private System.Windows.Forms.PictureBox pbSensorSearch;
        private UnE.GUI.ImageButton btnClose;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Panel plLevel;
        private System.Windows.Forms.ComboBox cmbLevel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.DataGridView gridSensorAddress;
        private UnE.GUI.ImageButton btnSave;
        private UnE.GUI.ImageButton btnModifityCancle;
        private UnE.GUI.ImageButton btnSearch;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colCheck;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAddress;
    }
}