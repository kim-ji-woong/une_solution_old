namespace DidUIEditor.Popups
{
    partial class FormNewPage
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
            this.lblDisasterType = new System.Windows.Forms.Label();
            this.cbDisasterType = new System.Windows.Forms.ComboBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.btnAdd = new UnE.GUI.ImageButton();
            this.btnCancel = new UnE.GUI.ImageButton();
            this.picSystemStyle1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAdd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSystemStyle1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.SuspendLayout();
            // 
            // lblDisasterType
            // 
            this.lblDisasterType.AutoSize = true;
            this.lblDisasterType.Font = new System.Drawing.Font("나눔바른고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDisasterType.Location = new System.Drawing.Point(451, 498);
            this.lblDisasterType.Name = "lblDisasterType";
            this.lblDisasterType.Size = new System.Drawing.Size(73, 17);
            this.lblDisasterType.TabIndex = 6;
            this.lblDisasterType.Text = "재난 타입 : ";
            // 
            // cbDisasterType
            // 
            this.cbDisasterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDisasterType.Font = new System.Drawing.Font("나눔바른고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cbDisasterType.FormattingEnabled = true;
            this.cbDisasterType.Items.AddRange(new object[] {
            "화재",
            "누출"});
            this.cbDisasterType.Location = new System.Drawing.Point(526, 495);
            this.cbDisasterType.Name = "cbDisasterType";
            this.cbDisasterType.Size = new System.Drawing.Size(70, 25);
            this.cbDisasterType.TabIndex = 7;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ColumnHeadersVisible = false;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1});
            this.dataGridView1.Location = new System.Drawing.Point(12, 36);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(368, 486);
            this.dataGridView1.TabIndex = 9;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Column1";
            this.Column1.Name = "Column1";
            // 
            // btnAdd
            // 
            this.btnAdd.ButtonText = "";
            this.btnAdd.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnAdd.ImageClicked = global::DidUIEditor.Properties.Resources.btnAdd_Click;
            this.btnAdd.ImageDisabled = null;
            this.btnAdd.ImageMouseOver = global::DidUIEditor.Properties.Resources.btnAdd_Click;
            this.btnAdd.ImageNormal = global::DidUIEditor.Properties.Resources.btnAdd_Default;
            this.btnAdd.Location = new System.Drawing.Point(620, 494);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Owner = null;
            this.btnAdd.Size = new System.Drawing.Size(80, 28);
            this.btnAdd.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnAdd.TabIndex = 17;
            this.btnAdd.TabStop = false;
            this.btnAdd.TextColor = System.Drawing.Color.Black;
            this.btnAdd.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnAdd.ToolTipText = "";
            this.btnAdd.UseToolTip = false;
            this.btnAdd.WindowRateWidth = 1F;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.ButtonText = "";
            this.btnCancel.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ImageClicked = global::DidUIEditor.Properties.Resources.btnCancel_Click;
            this.btnCancel.ImageDisabled = null;
            this.btnCancel.ImageMouseOver = global::DidUIEditor.Properties.Resources.btnCancel_Click;
            this.btnCancel.ImageNormal = global::DidUIEditor.Properties.Resources.btnCancel_Default;
            this.btnCancel.Location = new System.Drawing.Point(710, 494);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(80, 28);
            this.btnCancel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnCancel.TabIndex = 16;
            this.btnCancel.TabStop = false;
            this.btnCancel.TextColor = System.Drawing.Color.Black;
            this.btnCancel.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ToolTipText = "";
            this.btnCancel.UseToolTip = false;
            this.btnCancel.WindowRateWidth = 1F;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // picSystemStyle1
            // 
            this.picSystemStyle1.Image = global::DidUIEditor.Properties.Resources._0_기본;
            this.picSystemStyle1.Location = new System.Drawing.Point(386, 36);
            this.picSystemStyle1.Name = "picSystemStyle1";
            this.picSystemStyle1.Size = new System.Drawing.Size(404, 235);
            this.picSystemStyle1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picSystemStyle1.TabIndex = 1;
            this.picSystemStyle1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(52)))), ((int)(((byte)(72)))));
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(802, 30);
            this.panel1.TabIndex = 18;
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Image = global::DidUIEditor.Properties.Resources.close2;
            this.btnClose.Location = new System.Drawing.Point(776, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(14, 14);
            this.btnClose.TabIndex = 1;
            this.btnClose.TabStop = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FormNewPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
            this.ClientSize = new System.Drawing.Size(802, 538);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.cbDisasterType);
            this.Controls.Add(this.picSystemStyle1);
            this.Controls.Add(this.lblDisasterType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormNewPage";
            this.Text = "새 페이지 추가";
            this.Load += new System.EventHandler(this.FormNewPage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAdd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSystemStyle1)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox picSystemStyle1;
        private System.Windows.Forms.Label lblDisasterType;
        private System.Windows.Forms.ComboBox cbDisasterType;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewImageColumn Column1;
        private UnE.GUI.ImageButton btnCancel;
        private UnE.GUI.ImageButton btnAdd;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox btnClose;
    }
}