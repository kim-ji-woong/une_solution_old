
namespace SoulbrainSensorTester
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
            this.sensorTreeView = new System.Windows.Forms.TreeView();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbSensorType = new System.Windows.Forms.Label();
            this.lbSensorName = new System.Windows.Forms.Label();
            this.gridCurrent = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSelectReset = new System.Windows.Forms.Button();
            this.btnAllReset = new System.Windows.Forms.Button();
            this.btnProcessAllClear = new System.Windows.Forms.Button();
            this.textBoxMultipleAlarmCount = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnMultipleAlarms = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridCurrent)).BeginInit();
            this.SuspendLayout();
            // 
            // sensorTreeView
            // 
            this.sensorTreeView.Location = new System.Drawing.Point(264, 70);
            this.sensorTreeView.Name = "sensorTreeView";
            this.sensorTreeView.Size = new System.Drawing.Size(313, 230);
            this.sensorTreeView.TabIndex = 1;
            this.sensorTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.sensorTreeView_AfterSelect);
            // 
            // btnSend
            // 
            this.btnSend.Enabled = false;
            this.btnSend.Location = new System.Drawing.Point(599, 268);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "신호 전송";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnReset
            // 
            this.btnReset.Enabled = false;
            this.btnReset.Location = new System.Drawing.Point(693, 268);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 3;
            this.btnReset.Text = "신호복구";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(262, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "센서 리스트";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(597, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "타입 :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(597, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "이름 : ";
            // 
            // lbSensorType
            // 
            this.lbSensorType.AutoSize = true;
            this.lbSensorType.Location = new System.Drawing.Point(637, 70);
            this.lbSensorType.Name = "lbSensorType";
            this.lbSensorType.Size = new System.Drawing.Size(17, 12);
            this.lbSensorType.TabIndex = 7;
            this.lbSensorType.Text = "   ";
            // 
            // lbSensorName
            // 
            this.lbSensorName.AutoSize = true;
            this.lbSensorName.Location = new System.Drawing.Point(637, 98);
            this.lbSensorName.Name = "lbSensorName";
            this.lbSensorName.Size = new System.Drawing.Size(17, 12);
            this.lbSensorName.TabIndex = 8;
            this.lbSensorName.Text = "   ";
            // 
            // gridCurrent
            // 
            this.gridCurrent.AllowUserToAddRows = false;
            this.gridCurrent.AllowUserToDeleteRows = false;
            this.gridCurrent.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridCurrent.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1});
            this.gridCurrent.Location = new System.Drawing.Point(25, 53);
            this.gridCurrent.MultiSelect = false;
            this.gridCurrent.Name = "gridCurrent";
            this.gridCurrent.ReadOnly = true;
            this.gridCurrent.RowHeadersVisible = false;
            this.gridCurrent.RowTemplate.Height = 23;
            this.gridCurrent.Size = new System.Drawing.Size(222, 207);
            this.gridCurrent.TabIndex = 13;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn1.HeaderText = "현재 신호";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // btnSelectReset
            // 
            this.btnSelectReset.Location = new System.Drawing.Point(25, 268);
            this.btnSelectReset.Name = "btnSelectReset";
            this.btnSelectReset.Size = new System.Drawing.Size(111, 23);
            this.btnSelectReset.TabIndex = 14;
            this.btnSelectReset.Text = "선택한 센서 복구";
            this.btnSelectReset.UseVisualStyleBackColor = true;
            this.btnSelectReset.Click += new System.EventHandler(this.btnSelectReset_Click);
            // 
            // btnAllReset
            // 
            this.btnAllReset.Location = new System.Drawing.Point(145, 268);
            this.btnAllReset.Name = "btnAllReset";
            this.btnAllReset.Size = new System.Drawing.Size(102, 23);
            this.btnAllReset.TabIndex = 15;
            this.btnAllReset.Text = "모든 센서 복구";
            this.btnAllReset.UseVisualStyleBackColor = true;
            this.btnAllReset.Click += new System.EventHandler(this.btnAllReset_Click);
            // 
            // btnProcessAllClear
            // 
            this.btnProcessAllClear.Location = new System.Drawing.Point(111, 306);
            this.btnProcessAllClear.Name = "btnProcessAllClear";
            this.btnProcessAllClear.Size = new System.Drawing.Size(136, 23);
            this.btnProcessAllClear.TabIndex = 16;
            this.btnProcessAllClear.Text = "화재센서 모두 복구";
            this.btnProcessAllClear.UseVisualStyleBackColor = true;
            this.btnProcessAllClear.Click += new System.EventHandler(this.btnProcessAllClear_Click);
            // 
            // textBoxMultipleAlarmCount
            // 
            this.textBoxMultipleAlarmCount.Location = new System.Drawing.Point(354, 318);
            this.textBoxMultipleAlarmCount.Name = "textBoxMultipleAlarmCount";
            this.textBoxMultipleAlarmCount.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.textBoxMultipleAlarmCount.Size = new System.Drawing.Size(62, 21);
            this.textBoxMultipleAlarmCount.TabIndex = 17;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(269, 323);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 12);
            this.label4.TabIndex = 18;
            this.label4.Text = "한꺼번에 알람 ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(422, 323);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 12);
            this.label5.TabIndex = 18;
            this.label5.Text = "개";
            // 
            // btnMultipleAlarms
            // 
            this.btnMultipleAlarms.Location = new System.Drawing.Point(445, 314);
            this.btnMultipleAlarms.Name = "btnMultipleAlarms";
            this.btnMultipleAlarms.Size = new System.Drawing.Size(57, 27);
            this.btnMultipleAlarms.TabIndex = 19;
            this.btnMultipleAlarms.Text = "발생";
            this.btnMultipleAlarms.UseVisualStyleBackColor = true;
            this.btnMultipleAlarms.Click += new System.EventHandler(this.btnMultipleAlarms_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 362);
            this.Controls.Add(this.btnMultipleAlarms);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxMultipleAlarmCount);
            this.Controls.Add(this.btnProcessAllClear);
            this.Controls.Add(this.btnAllReset);
            this.Controls.Add(this.btnSelectReset);
            this.Controls.Add(this.gridCurrent);
            this.Controls.Add(this.lbSensorName);
            this.Controls.Add(this.lbSensorType);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.sensorTreeView);
            this.Name = "FormMain";
            this.Text = "Sensor Tester";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridCurrent)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TreeView sensorTreeView;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbSensorType;
        private System.Windows.Forms.Label lbSensorName;
        private System.Windows.Forms.DataGridView gridCurrent;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.Button btnSelectReset;
        private System.Windows.Forms.Button btnAllReset;
        private System.Windows.Forms.Button btnProcessAllClear;
        private System.Windows.Forms.TextBox textBoxMultipleAlarmCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnMultipleAlarms;
    }
}

