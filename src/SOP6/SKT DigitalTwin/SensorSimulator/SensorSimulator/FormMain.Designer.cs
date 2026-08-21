namespace SensorSimulator
{
    partial class FormMain
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

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.gridAlarms = new System.Windows.Forms.DataGridView();
            this.colAlarm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.treeSensors = new System.Windows.Forms.TreeView();
            this.btnAlarmOn = new System.Windows.Forms.Button();
            this.btnAlarmOff = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnMalf = new System.Windows.Forms.Button();
            this.btnReal = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridAlarms)).BeginInit();
            this.SuspendLayout();
            // 
            // gridAlarms
            // 
            this.gridAlarms.AllowUserToAddRows = false;
            this.gridAlarms.AllowUserToDeleteRows = false;
            this.gridAlarms.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridAlarms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridAlarms.ColumnHeadersVisible = false;
            this.gridAlarms.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colAlarm});
            this.gridAlarms.Location = new System.Drawing.Point(10, 13);
            this.gridAlarms.MultiSelect = false;
            this.gridAlarms.Name = "gridAlarms";
            this.gridAlarms.ReadOnly = true;
            this.gridAlarms.RowHeadersVisible = false;
            this.gridAlarms.RowTemplate.Height = 23;
            this.gridAlarms.Size = new System.Drawing.Size(243, 462);
            this.gridAlarms.TabIndex = 0;
            this.gridAlarms.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridAlarms_CellClick);
            // 
            // colAlarm
            // 
            this.colAlarm.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colAlarm.HeaderText = "알람";
            this.colAlarm.Name = "colAlarm";
            this.colAlarm.ReadOnly = true;
            // 
            // treeSensors
            // 
            this.treeSensors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeSensors.Location = new System.Drawing.Point(269, 13);
            this.treeSensors.Name = "treeSensors";
            this.treeSensors.Size = new System.Drawing.Size(407, 417);
            this.treeSensors.TabIndex = 1;
            this.treeSensors.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeSensors_AfterSelect);
            // 
            // btnAlarmOn
            // 
            this.btnAlarmOn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAlarmOn.Enabled = false;
            this.btnAlarmOn.Location = new System.Drawing.Point(542, 450);
            this.btnAlarmOn.Name = "btnAlarmOn";
            this.btnAlarmOn.Size = new System.Drawing.Size(64, 25);
            this.btnAlarmOn.TabIndex = 2;
            this.btnAlarmOn.Text = "알람발생";
            this.btnAlarmOn.UseVisualStyleBackColor = true;
            this.btnAlarmOn.Click += new System.EventHandler(this.btnAlarmOn_Click);
            // 
            // btnAlarmOff
            // 
            this.btnAlarmOff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAlarmOff.Enabled = false;
            this.btnAlarmOff.Location = new System.Drawing.Point(611, 450);
            this.btnAlarmOff.Name = "btnAlarmOff";
            this.btnAlarmOff.Size = new System.Drawing.Size(64, 25);
            this.btnAlarmOff.TabIndex = 2;
            this.btnAlarmOff.Text = "알람복구";
            this.btnAlarmOff.UseVisualStyleBackColor = true;
            this.btnAlarmOff.Click += new System.EventHandler(this.btnAlarmOff_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.OnTimer);
            // 
            // btnMalf
            // 
            this.btnMalf.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMalf.Enabled = false;
            this.btnMalf.Location = new System.Drawing.Point(361, 450);
            this.btnMalf.Name = "btnMalf";
            this.btnMalf.Size = new System.Drawing.Size(64, 25);
            this.btnMalf.TabIndex = 2;
            this.btnMalf.Text = "오작동";
            this.btnMalf.UseVisualStyleBackColor = true;
            this.btnMalf.Click += new System.EventHandler(this.btnMalf_Click);
            // 
            // btnReal
            // 
            this.btnReal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReal.Enabled = false;
            this.btnReal.Location = new System.Drawing.Point(430, 450);
            this.btnReal.Name = "btnReal";
            this.btnReal.Size = new System.Drawing.Size(64, 25);
            this.btnReal.TabIndex = 2;
            this.btnReal.Text = "실제화재";
            this.btnReal.UseVisualStyleBackColor = true;
            this.btnReal.Click += new System.EventHandler(this.btnReal_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 488);
            this.Controls.Add(this.btnReal);
            this.Controls.Add(this.btnAlarmOff);
            this.Controls.Add(this.btnMalf);
            this.Controls.Add(this.btnAlarmOn);
            this.Controls.Add(this.treeSensors);
            this.Controls.Add(this.gridAlarms);
            this.Name = "FormMain";
            this.Text = "경상남도 화재센서 시뮬레이터";
            ((System.ComponentModel.ISupportInitialize)(this.gridAlarms)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridAlarms;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAlarm;
        private System.Windows.Forms.TreeView treeSensors;
        private System.Windows.Forms.Button btnAlarmOn;
        private System.Windows.Forms.Button btnAlarmOff;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnMalf;
        private System.Windows.Forms.Button btnReal;
    }
}

