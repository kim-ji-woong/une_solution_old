namespace DidUIEditor.Popups
{
    partial class FormPageSetting
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.colPage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSave = new UnE.GUI.ImageButton();
            this.btnDel = new UnE.GUI.ImageButton();
            this.btnDown = new UnE.GUI.ImageButton();
            this.btnUp = new UnE.GUI.ImageButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnUp)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("나눔바른고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPage});
            this.dataGridView1.Location = new System.Drawing.Point(15, 42);
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.RowHeadersVisible = false;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(288, 258);
            this.dataGridView1.TabIndex = 0;
            // 
            // colPage
            // 
            this.colPage.HeaderText = "Page";
            this.colPage.Name = "colPage";
            // 
            // btnSave
            // 
            this.btnSave.ButtonText = "";
            this.btnSave.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSave.ImageClicked = global::DidUIEditor.Properties.Resources.btnSave_PageSet_Click;
            this.btnSave.ImageDisabled = null;
            this.btnSave.ImageMouseOver = global::DidUIEditor.Properties.Resources.btnSave_PageSet_Click;
            this.btnSave.ImageNormal = global::DidUIEditor.Properties.Resources.btnSave_PageSet_Default;
            this.btnSave.Location = new System.Drawing.Point(223, 315);
            this.btnSave.Name = "btnSave";
            this.btnSave.Owner = null;
            this.btnSave.Size = new System.Drawing.Size(80, 28);
            this.btnSave.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnSave.TabIndex = 15;
            this.btnSave.TabStop = false;
            this.btnSave.TextColor = System.Drawing.Color.Black;
            this.btnSave.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSave.ToolTipText = "";
            this.btnSave.UseToolTip = false;
            this.btnSave.WindowRateWidth = 1F;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnDel
            // 
            this.btnDel.ButtonText = "";
            this.btnDel.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnDel.ImageClicked = global::DidUIEditor.Properties.Resources.btnDelete_PageSet_Click;
            this.btnDel.ImageDisabled = null;
            this.btnDel.ImageMouseOver = global::DidUIEditor.Properties.Resources.btnDelete_PageSet_Click;
            this.btnDel.ImageNormal = global::DidUIEditor.Properties.Resources.btnDelete_PageSet_Default;
            this.btnDel.Location = new System.Drawing.Point(79, 315);
            this.btnDel.Name = "btnDel";
            this.btnDel.Owner = null;
            this.btnDel.Size = new System.Drawing.Size(80, 28);
            this.btnDel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnDel.TabIndex = 14;
            this.btnDel.TabStop = false;
            this.btnDel.TextColor = System.Drawing.Color.Black;
            this.btnDel.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnDel.ToolTipText = "";
            this.btnDel.UseToolTip = false;
            this.btnDel.WindowRateWidth = 1F;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btnDown
            // 
            this.btnDown.ButtonText = "";
            this.btnDown.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnDown.ImageClicked = global::DidUIEditor.Properties.Resources.btnDown_Click;
            this.btnDown.ImageDisabled = null;
            this.btnDown.ImageMouseOver = global::DidUIEditor.Properties.Resources.btnDown_Click;
            this.btnDown.ImageNormal = global::DidUIEditor.Properties.Resources.btnDown_Default;
            this.btnDown.Location = new System.Drawing.Point(47, 315);
            this.btnDown.Name = "btnDown";
            this.btnDown.Owner = null;
            this.btnDown.Size = new System.Drawing.Size(26, 26);
            this.btnDown.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnDown.TabIndex = 13;
            this.btnDown.TabStop = false;
            this.btnDown.TextColor = System.Drawing.Color.Black;
            this.btnDown.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnDown.ToolTipText = "";
            this.btnDown.UseToolTip = false;
            this.btnDown.WindowRateWidth = 1F;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.ButtonText = "";
            this.btnUp.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnUp.ImageClicked = global::DidUIEditor.Properties.Resources.btnUp_Click;
            this.btnUp.ImageDisabled = null;
            this.btnUp.ImageMouseOver = global::DidUIEditor.Properties.Resources.btnUp_Click;
            this.btnUp.ImageNormal = global::DidUIEditor.Properties.Resources.btnUp_Default;
            this.btnUp.Location = new System.Drawing.Point(15, 315);
            this.btnUp.Name = "btnUp";
            this.btnUp.Owner = null;
            this.btnUp.Size = new System.Drawing.Size(26, 26);
            this.btnUp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnUp.TabIndex = 12;
            this.btnUp.TabStop = false;
            this.btnUp.TextColor = System.Drawing.Color.Black;
            this.btnUp.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnUp.ToolTipText = "";
            this.btnUp.UseToolTip = false;
            this.btnUp.WindowRateWidth = 1F;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(52)))), ((int)(((byte)(72)))));
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(318, 30);
            this.panel1.TabIndex = 16;
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Image = global::DidUIEditor.Properties.Resources.close2;
            this.btnClose.Location = new System.Drawing.Point(289, 7);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(14, 14);
            this.btnClose.TabIndex = 1;
            this.btnClose.TabStop = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FormPageSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(318, 358);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnDel);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormPageSetting";
            this.Text = "페이지 설정";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnUp)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPage;
        private UnE.GUI.ImageButton btnUp;
        private UnE.GUI.ImageButton btnDown;
        private UnE.GUI.ImageButton btnDel;
        private UnE.GUI.ImageButton btnSave;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox btnClose;
    }
}