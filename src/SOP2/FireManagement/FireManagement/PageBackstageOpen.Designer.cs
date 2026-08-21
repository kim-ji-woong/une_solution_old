namespace FireManagement
{
    partial class PageBackstageOpen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageBackstageOpen));
            this.lblCaption = new System.Windows.Forms.Label();
            this.btnFileOpen = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOpenDXF = new System.Windows.Forms.Button();
            this.cboFloor = new System.Windows.Forms.ComboBox();
            this.cboBuildingGroup = new System.Windows.Forms.ComboBox();
            this.cboBuilding = new System.Windows.Forms.ComboBox();
            this.axBackstageBtnUser = new AxXtremeCommandBars.AxBackstageButton();
            this.lblBackstageSeparator4 = new AxXtremeCommandBars.AxBackstageSeparator();
            this.lblBackstageSeparator1 = new AxXtremeCommandBars.AxBackstageSeparator();
            ((System.ComponentModel.ISupportInitialize)(this.axBackstageBtnUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBackstageSeparator4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBackstageSeparator1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblCaption
            // 
            this.lblCaption.BackColor = System.Drawing.Color.White;
            this.lblCaption.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblCaption.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(59)))), ((int)(((byte)(59)))));
            this.lblCaption.Location = new System.Drawing.Point(12, 9);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblCaption.Size = new System.Drawing.Size(500, 35);
            this.lblCaption.TabIndex = 22;
            this.lblCaption.Text = "데이터 열기";
            // 
            // btnFileOpen
            // 
            this.btnFileOpen.Location = new System.Drawing.Point(651, 89);
            this.btnFileOpen.Name = "btnFileOpen";
            this.btnFileOpen.Size = new System.Drawing.Size(36, 23);
            this.btnFileOpen.TabIndex = 27;
            this.btnFileOpen.Text = "...";
            this.btnFileOpen.UseVisualStyleBackColor = true;
            this.btnFileOpen.Click += new System.EventHandler(this.btnFileOpen_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(344, 91);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(291, 21);
            this.textBox1.TabIndex = 26;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(229, 95);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 12);
            this.label1.TabIndex = 25;
            this.label1.Text = "관리점검 대상 파일";
            // 
            // btnOpenDXF
            // 
            this.btnOpenDXF.Location = new System.Drawing.Point(619, 243);
            this.btnOpenDXF.Name = "btnOpenDXF";
            this.btnOpenDXF.Size = new System.Drawing.Size(68, 23);
            this.btnOpenDXF.TabIndex = 28;
            this.btnOpenDXF.Text = "도면열기";
            this.btnOpenDXF.UseVisualStyleBackColor = true;
            this.btnOpenDXF.Click += new System.EventHandler(this.btnOpenDXF_Click);
            // 
            // cboFloor
            // 
            this.cboFloor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFloor.FormattingEnabled = true;
            this.cboFloor.Location = new System.Drawing.Point(630, 123);
            this.cboFloor.Name = "cboFloor";
            this.cboFloor.Size = new System.Drawing.Size(57, 20);
            this.cboFloor.Sorted = true;
            this.cboFloor.TabIndex = 34;
            this.cboFloor.Visible = false;
            // 
            // cboBuildingGroup
            // 
            this.cboBuildingGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuildingGroup.FormattingEnabled = true;
            this.cboBuildingGroup.Location = new System.Drawing.Point(232, 123);
            this.cboBuildingGroup.Name = "cboBuildingGroup";
            this.cboBuildingGroup.Size = new System.Drawing.Size(139, 20);
            this.cboBuildingGroup.TabIndex = 33;
            this.cboBuildingGroup.Visible = false;
            this.cboBuildingGroup.SelectedIndexChanged += new System.EventHandler(this.cboBuildingGroup_SelectedIndexChanged);
            // 
            // cboBuilding
            // 
            this.cboBuilding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuilding.FormattingEnabled = true;
            this.cboBuilding.Location = new System.Drawing.Point(377, 123);
            this.cboBuilding.Name = "cboBuilding";
            this.cboBuilding.Size = new System.Drawing.Size(247, 20);
            this.cboBuilding.TabIndex = 32;
            this.cboBuilding.Visible = false;
            this.cboBuilding.SelectedIndexChanged += new System.EventHandler(this.cboBuilding_SelectedIndexChanged);
            // 
            // axBackstageBtnUser
            // 
            this.axBackstageBtnUser.Location = new System.Drawing.Point(12, 71);
            this.axBackstageBtnUser.Name = "axBackstageBtnUser";
            this.axBackstageBtnUser.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axBackstageBtnUser.OcxState")));
            this.axBackstageBtnUser.Size = new System.Drawing.Size(175, 45);
            this.axBackstageBtnUser.TabIndex = 31;
            // 
            // lblBackstageSeparator4
            // 
            this.lblBackstageSeparator4.Enabled = true;
            this.lblBackstageSeparator4.Location = new System.Drawing.Point(193, 71);
            this.lblBackstageSeparator4.Name = "lblBackstageSeparator4";
            this.lblBackstageSeparator4.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("lblBackstageSeparator4.OcxState")));
            this.lblBackstageSeparator4.Size = new System.Drawing.Size(14, 435);
            this.lblBackstageSeparator4.TabIndex = 24;
            // 
            // lblBackstageSeparator1
            // 
            this.lblBackstageSeparator1.Enabled = true;
            this.lblBackstageSeparator1.Location = new System.Drawing.Point(12, 47);
            this.lblBackstageSeparator1.Name = "lblBackstageSeparator1";
            this.lblBackstageSeparator1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("lblBackstageSeparator1.OcxState")));
            this.lblBackstageSeparator1.Size = new System.Drawing.Size(675, 18);
            this.lblBackstageSeparator1.TabIndex = 23;
            // 
            // PageBackstageOpen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(910, 633);
            this.Controls.Add(this.cboFloor);
            this.Controls.Add(this.cboBuildingGroup);
            this.Controls.Add(this.cboBuilding);
            this.Controls.Add(this.axBackstageBtnUser);
            this.Controls.Add(this.btnOpenDXF);
            this.Controls.Add(this.btnFileOpen);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblBackstageSeparator4);
            this.Controls.Add(this.lblBackstageSeparator1);
            this.Controls.Add(this.lblCaption);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PageBackstageOpen";
            this.Text = "PageBackstageOpen";
            ((System.ComponentModel.ISupportInitialize)(this.axBackstageBtnUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBackstageSeparator4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBackstageSeparator1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public AxXtremeCommandBars.AxBackstageSeparator lblBackstageSeparator1;
        public System.Windows.Forms.Label lblCaption;
        public AxXtremeCommandBars.AxBackstageSeparator lblBackstageSeparator4;
        private System.Windows.Forms.Button btnFileOpen;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOpenDXF;
        public AxXtremeCommandBars.AxBackstageButton axBackstageBtnUser;
        private System.Windows.Forms.ComboBox cboFloor;
        private System.Windows.Forms.ComboBox cboBuildingGroup;
        private System.Windows.Forms.ComboBox cboBuilding;
    }
}