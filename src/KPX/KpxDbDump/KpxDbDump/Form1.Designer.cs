namespace KpxDbDump
{
    partial class Form1
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button_search = new System.Windows.Forms.Button();
            this.button_script = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.button_sqlSearch = new System.Windows.Forms.Button();
            this.button1_ex = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(13, 13);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(326, 20);
            this.comboBox1.TabIndex = 0;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(13, 40);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(688, 550);
            this.dataGridView1.TabIndex = 1;
            // 
            // button_search
            // 
            this.button_search.Location = new System.Drawing.Point(346, 12);
            this.button_search.Name = "button_search";
            this.button_search.Size = new System.Drawing.Size(75, 23);
            this.button_search.TabIndex = 2;
            this.button_search.Text = "조회";
            this.button_search.UseVisualStyleBackColor = true;
            this.button_search.Click += new System.EventHandler(this.button_search_Click);
            // 
            // button_script
            // 
            this.button_script.Location = new System.Drawing.Point(427, 12);
            this.button_script.Name = "button_script";
            this.button_script.Size = new System.Drawing.Size(75, 23);
            this.button_script.TabIndex = 4;
            this.button_script.Text = "Script";
            this.button_script.UseVisualStyleBackColor = true;
            this.button_script.Click += new System.EventHandler(this.button_script_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(707, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(701, 180);
            this.textBox1.TabIndex = 5;
            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(707, 230);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowTemplate.Height = 23;
            this.dataGridView2.Size = new System.Drawing.Size(701, 360);
            this.dataGridView2.TabIndex = 6;
            // 
            // button_sqlSearch
            // 
            this.button_sqlSearch.Location = new System.Drawing.Point(1333, 198);
            this.button_sqlSearch.Name = "button_sqlSearch";
            this.button_sqlSearch.Size = new System.Drawing.Size(75, 23);
            this.button_sqlSearch.TabIndex = 7;
            this.button_sqlSearch.Text = "조회";
            this.button_sqlSearch.UseVisualStyleBackColor = true;
            this.button_sqlSearch.Click += new System.EventHandler(this.button_sqlSearch_Click);
            // 
            // button1_ex
            // 
            this.button1_ex.Location = new System.Drawing.Point(1248, 198);
            this.button1_ex.Name = "button1_ex";
            this.button1_ex.Size = new System.Drawing.Size(75, 23);
            this.button1_ex.TabIndex = 8;
            this.button1_ex.Text = "button1";
            this.button1_ex.UseVisualStyleBackColor = true;
            this.button1_ex.Click += new System.EventHandler(this.button1_ex_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1420, 602);
            this.Controls.Add(this.button1_ex);
            this.Controls.Add(this.button_sqlSearch);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button_script);
            this.Controls.Add(this.button_search);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.comboBox1);
            this.Name = "Form1";
            this.Text = "KPX DB";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button_search;
        private System.Windows.Forms.Button button_script;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.Button button_sqlSearch;
        private System.Windows.Forms.Button button1_ex;
    }
}

