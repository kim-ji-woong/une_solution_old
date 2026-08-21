namespace SOPWebServer
{
    partial class FormMain
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnShowAlarms = new System.Windows.Forms.Button();
            this.textBoxResult = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.gridClients = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colClientType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colClientSubType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.popupMenuClients = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuCloseClient = new System.Windows.Forms.ToolStripMenuItem();
            this.btnShowSOPControl = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridClients)).BeginInit();
            this.popupMenuClients.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnShowAlarms
            // 
            this.btnShowAlarms.Location = new System.Drawing.Point(12, 12);
            this.btnShowAlarms.Name = "btnShowAlarms";
            this.btnShowAlarms.Size = new System.Drawing.Size(93, 23);
            this.btnShowAlarms.TabIndex = 0;
            this.btnShowAlarms.Text = "알람상태 보기";
            this.btnShowAlarms.UseVisualStyleBackColor = true;
            this.btnShowAlarms.Click += new System.EventHandler(this.btnShowAlarms_Click);
            // 
            // textBoxResult
            // 
            this.textBoxResult.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxResult.Location = new System.Drawing.Point(12, 87);
            this.textBoxResult.Multiline = true;
            this.textBoxResult.Name = "textBoxResult";
            this.textBoxResult.ReadOnly = true;
            this.textBoxResult.Size = new System.Drawing.Size(382, 276);
            this.textBoxResult.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "결과창";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(412, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "접속된 Client";
            // 
            // gridClients
            // 
            this.gridClients.AllowUserToAddRows = false;
            this.gridClients.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridClients.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridClients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridClients.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colClientType,
            this.colClientSubType,
            this.colIP});
            this.gridClients.Location = new System.Drawing.Point(414, 87);
            this.gridClients.Name = "gridClients";
            this.gridClients.ReadOnly = true;
            this.gridClients.RowHeadersVisible = false;
            this.gridClients.RowTemplate.Height = 23;
            this.gridClients.Size = new System.Drawing.Size(374, 276);
            this.gridClients.TabIndex = 3;
            this.gridClients.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gridClients_MouseUp);
            // 
            // colNo
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle2;
            this.colNo.HeaderText = "번호";
            this.colNo.Name = "colNo";
            this.colNo.ReadOnly = true;
            this.colNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNo.Width = 40;
            // 
            // colClientType
            // 
            this.colClientType.HeaderText = "타입";
            this.colClientType.Name = "colClientType";
            this.colClientType.ReadOnly = true;
            this.colClientType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colClientType.Width = 140;
            // 
            // colClientSubType
            // 
            this.colClientSubType.HeaderText = "하위타입";
            this.colClientSubType.Name = "colClientSubType";
            this.colClientSubType.ReadOnly = true;
            this.colClientSubType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colClientSubType.Width = 80;
            // 
            // colIP
            // 
            this.colIP.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colIP.DefaultCellStyle = dataGridViewCellStyle3;
            this.colIP.HeaderText = "IP";
            this.colIP.Name = "colIP";
            this.colIP.ReadOnly = true;
            this.colIP.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // popupMenuClients
            // 
            this.popupMenuClients.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuCloseClient});
            this.popupMenuClients.Name = "popupMenuClients";
            this.popupMenuClients.Size = new System.Drawing.Size(123, 26);
            // 
            // tsMenuCloseClient
            // 
            this.tsMenuCloseClient.Name = "tsMenuCloseClient";
            this.tsMenuCloseClient.Size = new System.Drawing.Size(122, 22);
            this.tsMenuCloseClient.Text = "연결끊기";
            this.tsMenuCloseClient.Click += new System.EventHandler(this.tsMenuCloseClient_Click);
            // 
            // btnShowSOPControl
            // 
            this.btnShowSOPControl.Location = new System.Drawing.Point(164, 12);
            this.btnShowSOPControl.Name = "btnShowSOPControl";
            this.btnShowSOPControl.Size = new System.Drawing.Size(106, 23);
            this.btnShowSOPControl.TabIndex = 0;
            this.btnShowSOPControl.Text = "제어권 상태 보기";
            this.btnShowSOPControl.UseVisualStyleBackColor = true;
            this.btnShowSOPControl.Click += new System.EventHandler(this.btnShowSOPControl_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.gridClients);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxResult);
            this.Controls.Add(this.btnShowSOPControl);
            this.Controls.Add(this.btnShowAlarms);
            this.Name = "FormMain";
            this.Text = "FormMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridClients)).EndInit();
            this.popupMenuClients.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnShowAlarms;
        private System.Windows.Forms.TextBox textBoxResult;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView gridClients;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colClientType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colClientSubType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIP;
        private System.Windows.Forms.ContextMenuStrip popupMenuClients;
        private System.Windows.Forms.ToolStripMenuItem tsMenuCloseClient;
        private System.Windows.Forms.Button btnShowSOPControl;
    }
}