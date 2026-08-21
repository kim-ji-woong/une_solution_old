namespace SOPDisasterSystem
{
    partial class FormRightSituation
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panelDigitalWatch = new System.Windows.Forms.Panel();
            this.userControl = new SOPDisasterSystem.DigitalDisplayControl();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panelDigitalCalender = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupScenario = new System.Windows.Forms.GroupBox();
            this.tabCtrlScenario = new System.Windows.Forms.TabControl();
            this.tabPage = new System.Windows.Forms.TabPage();
            this.panelScenario = new System.Windows.Forms.Panel();
            this.labelActivity = new System.Windows.Forms.Label();
            this.dataGridSenario = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSensor = new System.Windows.Forms.CheckBox();
            this.btnCCTV = new System.Windows.Forms.CheckBox();
            this.btnEquipment = new System.Windows.Forms.CheckBox();
            this.tabCtrlSystem = new System.Windows.Forms.TabControl();
            this.tabEquipment = new System.Windows.Forms.TabPage();
            this.treeEquipment = new System.Windows.Forms.TreeView();
            this.tabSensor = new System.Windows.Forms.TabPage();
            this.tabCCTV = new System.Windows.Forms.TabPage();
            this.panelDigitalWatch.SuspendLayout();
            this.panelDigitalCalender.SuspendLayout();
            this.groupScenario.SuspendLayout();
            this.tabCtrlScenario.SuspendLayout();
            this.tabPage.SuspendLayout();
            this.panelScenario.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridSenario)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.tabCtrlSystem.SuspendLayout();
            this.tabEquipment.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelDigitalWatch
            // 
            this.panelDigitalWatch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDigitalWatch.BackColor = System.Drawing.SystemColors.ControlText;
            this.panelDigitalWatch.Controls.Add(this.userControl);
            this.panelDigitalWatch.Location = new System.Drawing.Point(12, 50);
            this.panelDigitalWatch.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.panelDigitalWatch.Name = "panelDigitalWatch";
            this.panelDigitalWatch.Size = new System.Drawing.Size(237, 40);
            this.panelDigitalWatch.TabIndex = 1;
            // 
            // userControl
            // 
            this.userControl.BackColor = System.Drawing.Color.Transparent;
            this.userControl.DigitColor = System.Drawing.Color.WhiteSmoke;
            this.userControl.DigitText = "00:00:00";
            this.userControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userControl.ForeColor = System.Drawing.SystemColors.ControlText;
            this.userControl.Location = new System.Drawing.Point(0, 0);
            this.userControl.Name = "userControl";
            this.userControl.Size = new System.Drawing.Size(237, 40);
            this.userControl.TabIndex = 0;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panelDigitalCalender
            // 
            this.panelDigitalCalender.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDigitalCalender.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.panelDigitalCalender.Controls.Add(this.textBox1);
            this.panelDigitalCalender.Location = new System.Drawing.Point(12, 10);
            this.panelDigitalCalender.Margin = new System.Windows.Forms.Padding(0);
            this.panelDigitalCalender.Name = "panelDigitalCalender";
            this.panelDigitalCalender.Size = new System.Drawing.Size(237, 40);
            this.panelDigitalCalender.TabIndex = 0;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.WindowText;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(237, 47);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupScenario
            // 
            this.groupScenario.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupScenario.Controls.Add(this.tabCtrlScenario);
            this.groupScenario.Location = new System.Drawing.Point(12, 96);
            this.groupScenario.Name = "groupScenario";
            this.groupScenario.Size = new System.Drawing.Size(237, 187);
            this.groupScenario.TabIndex = 2;
            this.groupScenario.TabStop = false;
            this.groupScenario.Text = "SOP 시나리오";
            // 
            // tabCtrlScenario
            // 
            this.tabCtrlScenario.Controls.Add(this.tabPage);
            this.tabCtrlScenario.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabCtrlScenario.Location = new System.Drawing.Point(3, 17);
            this.tabCtrlScenario.Name = "tabCtrlScenario";
            this.tabCtrlScenario.SelectedIndex = 0;
            this.tabCtrlScenario.Size = new System.Drawing.Size(231, 167);
            this.tabCtrlScenario.TabIndex = 0;
            // 
            // tabPage
            // 
            this.tabPage.Controls.Add(this.panelScenario);
            this.tabPage.Location = new System.Drawing.Point(4, 22);
            this.tabPage.Name = "tabPage";
            this.tabPage.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage.Size = new System.Drawing.Size(223, 141);
            this.tabPage.TabIndex = 0;
            this.tabPage.Text = "SOP";
            this.tabPage.UseVisualStyleBackColor = true;
            // 
            // panelScenario
            // 
            this.panelScenario.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.panelScenario.Controls.Add(this.labelActivity);
            this.panelScenario.Controls.Add(this.dataGridSenario);
            this.panelScenario.Controls.Add(this.label1);
            this.panelScenario.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelScenario.Location = new System.Drawing.Point(3, 3);
            this.panelScenario.Name = "panelScenario";
            this.panelScenario.Size = new System.Drawing.Size(217, 135);
            this.panelScenario.TabIndex = 0;
            // 
            // labelActivity
            // 
            this.labelActivity.AutoSize = true;
            this.labelActivity.Font = new System.Drawing.Font("굴림", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelActivity.ForeColor = System.Drawing.Color.DodgerBlue;
            this.labelActivity.Location = new System.Drawing.Point(73, 30);
            this.labelActivity.Name = "labelActivity";
            this.labelActivity.Size = new System.Drawing.Size(73, 29);
            this.labelActivity.TabIndex = 2;
            this.labelActivity.Text = "관심";
            // 
            // dataGridSenario
            // 
            this.dataGridSenario.AllowUserToAddRows = false;
            this.dataGridSenario.AllowUserToDeleteRows = false;
            this.dataGridSenario.AllowUserToResizeColumns = false;
            this.dataGridSenario.AllowUserToResizeRows = false;
            this.dataGridSenario.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridSenario.ColumnHeadersVisible = false;
            this.dataGridSenario.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridSenario.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridSenario.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridSenario.Enabled = false;
            this.dataGridSenario.Location = new System.Drawing.Point(0, 63);
            this.dataGridSenario.MultiSelect = false;
            this.dataGridSenario.Name = "dataGridSenario";
            this.dataGridSenario.ReadOnly = true;
            this.dataGridSenario.RowHeadersVisible = false;
            this.dataGridSenario.RowTemplate.Height = 23;
            this.dataGridSenario.Size = new System.Drawing.Size(217, 72);
            this.dataGridSenario.TabIndex = 1;
            // 
            // Column1
            // 
            this.Column1.Frozen = true;
            this.Column1.HeaderText = "Column1";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 70;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.HeaderText = "Column2";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(21, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "위기관리 활동단계";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnSensor);
            this.groupBox2.Controls.Add(this.btnCCTV);
            this.groupBox2.Controls.Add(this.btnEquipment);
            this.groupBox2.Controls.Add(this.tabCtrlSystem);
            this.groupBox2.Location = new System.Drawing.Point(15, 289);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(234, 208);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "연계 시스템";
            // 
            // btnSensor
            // 
            this.btnSensor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSensor.Enabled = false;
            this.btnSensor.Location = new System.Drawing.Point(154, 172);
            this.btnSensor.Name = "btnSensor";
            this.btnSensor.Size = new System.Drawing.Size(75, 30);
            this.btnSensor.TabIndex = 1;
            this.btnSensor.Text = "감지기";
            this.btnSensor.UseVisualStyleBackColor = true;
            this.btnSensor.Click += new System.EventHandler(this.btnSensor_Click);
            // 
            // btnCCTV
            // 
            this.btnCCTV.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCCTV.Enabled = false;
            this.btnCCTV.Location = new System.Drawing.Point(79, 172);
            this.btnCCTV.Name = "btnCCTV";
            this.btnCCTV.Size = new System.Drawing.Size(75, 30);
            this.btnCCTV.TabIndex = 1;
            this.btnCCTV.Text = "CCTV";
            this.btnCCTV.UseVisualStyleBackColor = true;
            this.btnCCTV.Click += new System.EventHandler(this.btnCCTV_Click);
            // 
            // btnEquipment
            // 
            this.btnEquipment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEquipment.Checked = true;
            this.btnEquipment.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnEquipment.Location = new System.Drawing.Point(4, 172);
            this.btnEquipment.Name = "btnEquipment";
            this.btnEquipment.Size = new System.Drawing.Size(75, 30);
            this.btnEquipment.TabIndex = 1;
            this.btnEquipment.Text = "소방설비";
            this.btnEquipment.UseVisualStyleBackColor = true;
            this.btnEquipment.Click += new System.EventHandler(this.btnEquipment_Click);
            // 
            // tabCtrlSystem
            // 
            this.tabCtrlSystem.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabCtrlSystem.Controls.Add(this.tabEquipment);
            this.tabCtrlSystem.Controls.Add(this.tabSensor);
            this.tabCtrlSystem.Controls.Add(this.tabCCTV);
            this.tabCtrlSystem.Location = new System.Drawing.Point(3, 17);
            this.tabCtrlSystem.Name = "tabCtrlSystem";
            this.tabCtrlSystem.SelectedIndex = 0;
            this.tabCtrlSystem.Size = new System.Drawing.Size(231, 155);
            this.tabCtrlSystem.TabIndex = 0;
            // 
            // tabEquipment
            // 
            this.tabEquipment.Controls.Add(this.treeEquipment);
            this.tabEquipment.Location = new System.Drawing.Point(4, 22);
            this.tabEquipment.Name = "tabEquipment";
            this.tabEquipment.Padding = new System.Windows.Forms.Padding(3);
            this.tabEquipment.Size = new System.Drawing.Size(223, 129);
            this.tabEquipment.TabIndex = 0;
            this.tabEquipment.Text = "소방설비";
            this.tabEquipment.UseVisualStyleBackColor = true;
            // 
            // treeEquipment
            // 
            this.treeEquipment.BackColor = System.Drawing.Color.White;
            this.treeEquipment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeEquipment.Location = new System.Drawing.Point(3, 3);
            this.treeEquipment.Name = "treeEquipment";
            this.treeEquipment.Size = new System.Drawing.Size(217, 123);
            this.treeEquipment.TabIndex = 0;
            // 
            // tabSensor
            // 
            this.tabSensor.Location = new System.Drawing.Point(4, 22);
            this.tabSensor.Name = "tabSensor";
            this.tabSensor.Padding = new System.Windows.Forms.Padding(3);
            this.tabSensor.Size = new System.Drawing.Size(223, 129);
            this.tabSensor.TabIndex = 1;
            this.tabSensor.Text = "화재감지";
            this.tabSensor.UseVisualStyleBackColor = true;
            // 
            // tabCCTV
            // 
            this.tabCCTV.Location = new System.Drawing.Point(4, 22);
            this.tabCCTV.Name = "tabCCTV";
            this.tabCCTV.Padding = new System.Windows.Forms.Padding(3);
            this.tabCCTV.Size = new System.Drawing.Size(223, 129);
            this.tabCCTV.TabIndex = 2;
            this.tabCCTV.Text = "CCTV";
            this.tabCCTV.UseVisualStyleBackColor = true;
            // 
            // FormRightSituation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(261, 509);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupScenario);
            this.Controls.Add(this.panelDigitalCalender);
            this.Controls.Add(this.panelDigitalWatch);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormRightSituation";
            this.Text = "상황";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormRightSituation_FormClosing);
            this.Load += new System.EventHandler(this.FormRightSituation_Load);
            this.panelDigitalWatch.ResumeLayout(false);
            this.panelDigitalCalender.ResumeLayout(false);
            this.panelDigitalCalender.PerformLayout();
            this.groupScenario.ResumeLayout(false);
            this.tabCtrlScenario.ResumeLayout(false);
            this.tabPage.ResumeLayout(false);
            this.panelScenario.ResumeLayout(false);
            this.panelScenario.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridSenario)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.tabCtrlSystem.ResumeLayout(false);
            this.tabEquipment.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelDigitalWatch;
        private SOPDisasterSystem.DigitalDisplayControl userControl;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panelDigitalCalender;
        private System.Windows.Forms.GroupBox groupScenario;
        private System.Windows.Forms.TabControl tabCtrlScenario;
        private System.Windows.Forms.TabPage tabPage;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TabControl tabCtrlSystem;
        private System.Windows.Forms.TabPage tabEquipment;
        private System.Windows.Forms.TabPage tabSensor;
        private System.Windows.Forms.TabPage tabCCTV;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox btnSensor;
        private System.Windows.Forms.CheckBox btnCCTV;
        private System.Windows.Forms.CheckBox btnEquipment;
        private System.Windows.Forms.TreeView treeEquipment;
        private System.Windows.Forms.Panel panelScenario;
        private System.Windows.Forms.Label labelActivity;
        private System.Windows.Forms.DataGridView dataGridSenario;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.Label label1;
    }
}