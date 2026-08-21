namespace SDMS
{
    partial class ActionEarthquakePage
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSelectDate = new System.Windows.Forms.Label();
            this.m_dataGridView = new System.Windows.Forms.DataGridView();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblMinDate = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblDefault = new System.Windows.Forms.Label();
            this.textBoxMemo = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnSaveHWP = new UnE.GUI.ImageButton();
            this.reactionLogBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSaveHWP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reactionLogBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(38, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(178, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "지진 대응 이력";
            this.label1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // lblSelectDate
            // 
            this.lblSelectDate.AutoSize = true;
            this.lblSelectDate.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblSelectDate.ForeColor = System.Drawing.Color.White;
            this.lblSelectDate.Location = new System.Drawing.Point(200, 63);
            this.lblSelectDate.Name = "lblSelectDate";
            this.lblSelectDate.Size = new System.Drawing.Size(46, 18);
            this.lblSelectDate.TabIndex = 2;
            this.lblSelectDate.Text = "Time";
            this.lblSelectDate.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // m_dataGridView
            // 
            this.m_dataGridView.AllowUserToAddRows = false;
            this.m_dataGridView.AllowUserToDeleteRows = false;
            this.m_dataGridView.AllowUserToResizeRows = false;
            this.m_dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.m_dataGridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.m_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_dataGridView.Location = new System.Drawing.Point(0, 93);
            this.m_dataGridView.Name = "m_dataGridView";
            this.m_dataGridView.ReadOnly = true;
            this.m_dataGridView.RowHeadersVisible = false;
            this.m_dataGridView.RowTemplate.Height = 30;
            this.m_dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.m_dataGridView.Size = new System.Drawing.Size(1834, 715);
            this.m_dataGridView.TabIndex = 4;
            this.m_dataGridView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(288, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(11, 18);
            this.label4.TabIndex = 10;
            this.label4.Text = "l";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(215, 29);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 18);
            this.label5.TabIndex = 11;
            this.label5.Text = "발생 시간";
            this.label5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // lblMinDate
            // 
            this.lblMinDate.AutoSize = true;
            this.lblMinDate.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblMinDate.ForeColor = System.Drawing.Color.White;
            this.lblMinDate.Location = new System.Drawing.Point(299, 29);
            this.lblMinDate.Name = "lblMinDate";
            this.lblMinDate.Size = new System.Drawing.Size(110, 18);
            this.lblMinDate.TabIndex = 12;
            this.lblMinDate.Text = " 데이터 없음";
            this.lblMinDate.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(39, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 18);
            this.label2.TabIndex = 14;
            this.label2.Text = "데이터 없음";
            this.label2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(144)))), ((int)(((byte)(139)))));
            this.panel1.Location = new System.Drawing.Point(43, 52);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1750, 3);
            this.panel1.TabIndex = 22;
            // 
            // lblDefault
            // 
            this.lblDefault.AutoSize = true;
            this.lblDefault.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDefault.ForeColor = System.Drawing.Color.White;
            this.lblDefault.Location = new System.Drawing.Point(39, 63);
            this.lblDefault.Name = "lblDefault";
            this.lblDefault.Size = new System.Drawing.Size(276, 18);
            this.lblDefault.TabIndex = 33;
            this.lblDefault.Text = "조회할 지진상황을 선택하세요.";
            this.lblDefault.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // textBoxMemo
            // 
            this.textBoxMemo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMemo.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxMemo.Location = new System.Drawing.Point(0, 816);
            this.textBoxMemo.Margin = new System.Windows.Forms.Padding(5);
            this.textBoxMemo.Multiline = true;
            this.textBoxMemo.Name = "textBoxMemo";
            this.textBoxMemo.ReadOnly = true;
            this.textBoxMemo.Size = new System.Drawing.Size(1834, 189);
            this.textBoxMemo.TabIndex = 37;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("굴림", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(28, 1859);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 29);
            this.label7.TabIndex = 36;
            this.label7.Text = "메모";
            // 
            // btnSaveHWP
            // 
            this.btnSaveHWP.ButtonText = "";
            this.btnSaveHWP.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSaveHWP.ImageClicked = global::SDMS.Properties.Resources.BtnSaveHWP_Click;
            this.btnSaveHWP.ImageDisabled = null;
            this.btnSaveHWP.ImageMouseOver = global::SDMS.Properties.Resources.BtnSaveHWP_Click;
            this.btnSaveHWP.ImageNormal = global::SDMS.Properties.Resources.BtnSaveHWP_Default;
            this.btnSaveHWP.Location = new System.Drawing.Point(1710, 18);
            this.btnSaveHWP.Name = "btnSaveHWP";
            this.btnSaveHWP.Owner = null;
            this.btnSaveHWP.Size = new System.Drawing.Size(83, 29);
            this.btnSaveHWP.TabIndex = 38;
            this.btnSaveHWP.TabStop = false;
            this.btnSaveHWP.TextColor = System.Drawing.Color.Black;
            this.btnSaveHWP.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSaveHWP.ToolTipText = "";
            this.btnSaveHWP.UseToolTip = false;
            this.btnSaveHWP.WindowRateWidth = 1F;
            this.btnSaveHWP.Click += new System.EventHandler(this.btnSaveHWP_Click);
            
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(1360, 63);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 18);
            this.label6.TabIndex = 15;
            this.label6.Text = "label6";
            this.label6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            // 
            // ActionEarthquakePage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(45)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(1834, 1005);
            this.Controls.Add(this.btnSaveHWP);
            this.Controls.Add(this.textBoxMemo);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblDefault);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.m_dataGridView);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblMinDate);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblSelectDate);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "ActionEarthquakePage";
            this.Text = "ActionEarthquakePage";
            this.Load += new System.EventHandler(this.ActionPage_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.this_MouseDown);
            this.Resize += new System.EventHandler(this.ActionPage_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSaveHWP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.reactionLogBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblSelectDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblMinDate;
        private System.Windows.Forms.DataGridView m_dataGridView;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblDefault;
        private System.Windows.Forms.BindingSource reactionLogBindingSource;
        private System.Windows.Forms.TextBox textBoxMemo;
        private System.Windows.Forms.Label label7;
        private UnE.GUI.ImageButton btnSaveHWP;
        private System.Windows.Forms.Label label6;
    }
}