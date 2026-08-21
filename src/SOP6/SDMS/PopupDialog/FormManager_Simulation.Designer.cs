namespace SDMS
{
    partial class FormManager_Simulation
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
            this.gridManager = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPhoneNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnClose = new UnE.GUI.ImageButton();
            this.btnEdit = new UnE.GUI.ImageButton();
            this.labelMiddle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEdit)).BeginInit();
            this.SuspendLayout();
            // 
            // gridManager
            // 
            this.gridManager.AllowUserToAddRows = false;
            this.gridManager.AllowUserToDeleteRows = false;
            this.gridManager.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridManager.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colName,
            this.colPhoneNumber});
            this.gridManager.Location = new System.Drawing.Point(25, 80);
            this.gridManager.Name = "gridManager";
            this.gridManager.ReadOnly = true;
            this.gridManager.RowHeadersVisible = false;
            this.gridManager.RowTemplate.Height = 23;
            this.gridManager.Size = new System.Drawing.Size(1085, 828);
            this.gridManager.TabIndex = 10;
            this.gridManager.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.gridManager_RowsAdded);
            this.gridManager.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.gridManager_RowsRemoved);
            this.gridManager.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridManager_KeyDown);
            // 
            // colNo
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle1;
            this.colNo.HeaderText = "No";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.Width = 30;
            // 
            // colName
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colName.DefaultCellStyle = dataGridViewCellStyle2;
            this.colName.HeaderText = "이름";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.Width = 150;
            // 
            // colPhoneNumber
            // 
            this.colPhoneNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colPhoneNumber.DefaultCellStyle = dataGridViewCellStyle3;
            this.colPhoneNumber.HeaderText = "전화번호";
            this.colPhoneNumber.Name = "colPhoneNumber";
            this.colPhoneNumber.ReadOnly = true;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ButtonText = "";
            this.btnClose.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ImageClicked = global::SDMS.Properties.Resources.Close_40_40_Click;
            this.btnClose.ImageDisabled = null;
            this.btnClose.ImageMouseOver = global::SDMS.Properties.Resources.Close_40_40_Click;
            this.btnClose.ImageNormal = global::SDMS.Properties.Resources.Close_40_40_Default;
            this.btnClose.Location = new System.Drawing.Point(1070, 9);
            this.btnClose.Name = "btnClose";
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(40, 40);
            this.btnClose.TabIndex = 23;
            this.btnClose.TabStop = false;
            this.btnClose.TextColor = System.Drawing.Color.Black;
            this.btnClose.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ToolTipText = "";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.Color.Transparent;
            this.btnEdit.ButtonText = "";
            this.btnEdit.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnEdit.ImageClicked = global::SDMS.Properties.Resources.CheckBox_Click;
            this.btnEdit.ImageDisabled = null;
            this.btnEdit.ImageMouseOver = global::SDMS.Properties.Resources.CheckBox_Default;
            this.btnEdit.ImageNormal = global::SDMS.Properties.Resources.CheckBox_Default;
            this.btnEdit.Location = new System.Drawing.Point(29, 914);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Owner = null;
            this.btnEdit.Size = new System.Drawing.Size(32, 32);
            this.btnEdit.TabIndex = 24;
            this.btnEdit.TabStop = false;
            this.btnEdit.TextColor = System.Drawing.Color.Black;
            this.btnEdit.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnEdit.ToolTipText = "";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // labelMiddle
            // 
            this.labelMiddle.AutoSize = true;
            this.labelMiddle.BackColor = System.Drawing.Color.Transparent;
            this.labelMiddle.Font = new System.Drawing.Font(Program.prgFont, 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelMiddle.ForeColor = System.Drawing.Color.White;
            this.labelMiddle.Location = new System.Drawing.Point(65, 912);
            this.labelMiddle.Name = "labelMiddle";
            this.labelMiddle.Size = new System.Drawing.Size(131, 35);
            this.labelMiddle.TabIndex = 25;
            this.labelMiddle.Text = "편집모드";
            this.labelMiddle.Click += new System.EventHandler(this.labelMiddle_Click);
            // 
            // FormManager_Simulation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.BackgroundImage = global::SDMS.Properties.Resources.Manager_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1132, 962);
            this.Controls.Add(this.labelMiddle);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.gridManager);
            this.Name = "FormManager_Simulation";
            this.Text = "FormManager_Simulation";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormManager_Simulation_FormClosing);
            this.Load += new System.EventHandler(this.FormManager_Simulation_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEdit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView gridManager;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPhoneNumber;
        private UnE.GUI.ImageButton btnClose;
        private UnE.GUI.ImageButton btnEdit;
        private System.Windows.Forms.Label labelMiddle;

    }
}