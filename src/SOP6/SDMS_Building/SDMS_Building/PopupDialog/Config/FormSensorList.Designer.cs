namespace SDMS_Building.PopupDialog.Config
{
    partial class FormSensorList
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
            this.eleSensorType = new System.Windows.Forms.Integration.ElementHost();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.eleBuilding = new System.Windows.Forms.Integration.ElementHost();
            this.eleFloor = new System.Windows.Forms.Integration.ElementHost();
            this.lblFacilityName = new System.Windows.Forms.Label();
            this.btnSearch = new UnE.GUI.ImageButton();
            this.gridSensorList = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridSensorList)).BeginInit();
            this.SuspendLayout();
            // 
            // eleSensorType
            // 
            this.eleSensorType.Location = new System.Drawing.Point(20, 50);
            this.eleSensorType.Name = "eleSensorType";
            this.eleSensorType.Size = new System.Drawing.Size(156, 50);
            this.eleSensorType.TabIndex = 22;
            this.eleSensorType.Text = "elementHost1";
            this.eleSensorType.Child = null;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(91)))), ((int)(((byte)(91)))));
            this.label1.Location = new System.Drawing.Point(20, 20);
            this.label1.MinimumSize = new System.Drawing.Size(36, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 28);
            this.label1.TabIndex = 23;
            this.label1.Text = "유형선택";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(91)))), ((int)(((byte)(91)))));
            this.label3.Location = new System.Drawing.Point(20, 120);
            this.label3.MinimumSize = new System.Drawing.Size(36, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 28);
            this.label3.TabIndex = 26;
            this.label3.Text = "범위선택";
            // 
            // eleBuilding
            // 
            this.eleBuilding.Location = new System.Drawing.Point(20, 150);
            this.eleBuilding.Name = "eleBuilding";
            this.eleBuilding.Size = new System.Drawing.Size(650, 50);
            this.eleBuilding.TabIndex = 27;
            this.eleBuilding.Text = "elementHost3";
            this.eleBuilding.Child = null;
            // 
            // eleFloor
            // 
            this.eleFloor.Location = new System.Drawing.Point(700, 150);
            this.eleFloor.Name = "eleFloor";
            this.eleFloor.Size = new System.Drawing.Size(242, 50);
            this.eleFloor.TabIndex = 28;
            this.eleFloor.Text = "elementHost4";
            this.eleFloor.Child = null;
            // 
            // lblFacilityName
            // 
            this.lblFacilityName.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblFacilityName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(91)))), ((int)(((byte)(91)))));
            this.lblFacilityName.Location = new System.Drawing.Point(562, 62);
            this.lblFacilityName.MinimumSize = new System.Drawing.Size(36, 28);
            this.lblFacilityName.Name = "lblFacilityName";
            this.lblFacilityName.Size = new System.Drawing.Size(243, 28);
            this.lblFacilityName.TabIndex = 30;
            this.lblFacilityName.Text = "모든 설비 목록";
            this.lblFacilityName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnSearch
            // 
            this.btnSearch.ButtonText = "";
            this.btnSearch.ImageClicked = global::SDMS_Building.Properties.Resources.search_click;
            this.btnSearch.ImageDisabled = null;
            this.btnSearch.ImageMouseOver = global::SDMS_Building.Properties.Resources.search_normal;
            this.btnSearch.ImageNormal = global::SDMS_Building.Properties.Resources.search_normal;
            this.btnSearch.Location = new System.Drawing.Point(816, 50);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Owner = null;
            this.btnSearch.Size = new System.Drawing.Size(126, 50);
            this.btnSearch.TabIndex = 31;
            this.btnSearch.TabStop = false;
            this.btnSearch.TextColor = System.Drawing.Color.Black;
            this.btnSearch.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSearch.ToolTipText = "";
            this.btnSearch.UseToolTip = false;
            this.btnSearch.WindowRateWidth = 1F;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // gridSensorList
            // 
            this.gridSensorList.AllowUserToAddRows = false;
            this.gridSensorList.BackgroundColor = System.Drawing.Color.White;
            this.gridSensorList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gridSensorList.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("나눔바른고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridSensorList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridSensorList.ColumnHeadersHeight = 50;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridSensorList.DefaultCellStyle = dataGridViewCellStyle2;
            this.gridSensorList.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.gridSensorList.Location = new System.Drawing.Point(20, 225);
            this.gridSensorList.Name = "gridSensorList";
            this.gridSensorList.RowHeadersVisible = false;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("나눔바른고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.gridSensorList.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.gridSensorList.RowTemplate.Height = 50;
            this.gridSensorList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridSensorList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridSensorList.Size = new System.Drawing.Size(922, 250);
            this.gridSensorList.TabIndex = 32;
            // 
            // FormSensorList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(960, 500);
            this.Controls.Add(this.gridSensorList);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.lblFacilityName);
            this.Controls.Add(this.eleFloor);
            this.Controls.Add(this.eleBuilding);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.eleSensorType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormSensorList";
            this.ShowInTaskbar = false;
            this.Text = "FormSensorList";
            this.Load += new System.EventHandler(this.FormSensorList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.btnSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridSensorList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost eleSensorType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Integration.ElementHost eleBuilding;
        private System.Windows.Forms.Integration.ElementHost eleFloor;
        private System.Windows.Forms.Label lblFacilityName;
        private UnE.GUI.ImageButton btnSearch;
        private System.Windows.Forms.DataGridView gridSensorList;
    }
}