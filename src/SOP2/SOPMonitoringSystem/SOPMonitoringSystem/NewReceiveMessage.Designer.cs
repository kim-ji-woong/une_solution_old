namespace SOPMonitoringSystem
{
    partial class NewReceiveMessage
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
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.btnOK = new System.Windows.Forms.Button();
            this.dgv_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgv_time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgv_Disa = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgv_act = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgv_id,
            this.dgv_time,
            this.dgv_Disa,
            this.dgv_act});
            this.dataGridView.Location = new System.Drawing.Point(12, 12);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.RowTemplate.Height = 23;
            this.dataGridView.Size = new System.Drawing.Size(906, 188);
            this.dataGridView.TabIndex = 1;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(379, 206);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(186, 30);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "확     인";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // dgv_id
            // 
            this.dgv_id.HeaderText = "ID";
            this.dgv_id.Name = "dgv_id";
            this.dgv_id.ReadOnly = true;
            this.dgv_id.Width = 40;
            // 
            // dgv_time
            // 
            this.dgv_time.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgv_time.HeaderText = "Time";
            this.dgv_time.Name = "dgv_time";
            this.dgv_time.ReadOnly = true;
            this.dgv_time.Width = 150;
            // 
            // dgv_Disa
            // 
            this.dgv_Disa.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgv_Disa.HeaderText = "SOP재난명";
            this.dgv_Disa.Name = "dgv_Disa";
            this.dgv_Disa.ReadOnly = true;
            this.dgv_Disa.Width = 200;
            // 
            // dgv_act
            // 
            this.dgv_act.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgv_act.HeaderText = "조치내용";
            this.dgv_act.Name = "dgv_act";
            this.dgv_act.ReadOnly = true;
            // 
            // NewReceiveMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(930, 248);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.dataGridView);
            this.Name = "NewReceiveMessage";
            this.Text = "NewReceiveMessage";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgv_id;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgv_time;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgv_Disa;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgv_act;
    }
}