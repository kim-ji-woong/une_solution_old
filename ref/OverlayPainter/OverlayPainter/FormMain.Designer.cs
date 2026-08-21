namespace WindowsFormsApplication4
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
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
		/// </summary>
		private void InitializeComponent()
		{
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.button5 = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.button7 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.button6 = new System.Windows.Forms.Button();
			this.mOverlayPane = new UnE.Overlay.OverlayPanel();
			this.button8 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(99, 15);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(37, 36);
			this.button1.TabIndex = 1;
			this.button1.Text = "선";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(142, 15);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(40, 35);
			this.button2.TabIndex = 2;
			this.button2.Text = "프리";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(188, 15);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(42, 36);
			this.button3.TabIndex = 3;
			this.button3.Text = "사각형";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictureBox1.Location = new System.Drawing.Point(378, 15);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(44, 36);
			this.pictureBox1.TabIndex = 5;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
			// 
			// pictureBox2
			// 
			this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictureBox2.Location = new System.Drawing.Point(428, 15);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(44, 36);
			this.pictureBox2.TabIndex = 6;
			this.pictureBox2.TabStop = false;
			this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
			// 
			// comboBox1
			// 
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Items.AddRange(new object[] {
            "1px",
            "3px",
            "5px",
            "8px"});
			this.comboBox1.Location = new System.Drawing.Point(305, 24);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(54, 20);
			this.comboBox1.TabIndex = 7;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(12, 15);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(36, 34);
			this.button5.TabIndex = 8;
			this.button5.Text = "삭제";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.button5_Click);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.button8);
			this.panel2.Controls.Add(this.button7);
			this.panel2.Controls.Add(this.button4);
			this.panel2.Controls.Add(this.button6);
			this.panel2.Controls.Add(this.pictureBox2);
			this.panel2.Controls.Add(this.comboBox1);
			this.panel2.Controls.Add(this.pictureBox1);
			this.panel2.Controls.Add(this.button5);
			this.panel2.Controls.Add(this.button1);
			this.panel2.Controls.Add(this.button2);
			this.panel2.Controls.Add(this.button3);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(703, 62);
			this.panel2.TabIndex = 9;
			// 
			// button7
			// 
			this.button7.Location = new System.Drawing.Point(565, 25);
			this.button7.Name = "button7";
			this.button7.Size = new System.Drawing.Size(75, 23);
			this.button7.TabIndex = 11;
			this.button7.Text = "삭제";
			this.button7.UseVisualStyleBackColor = true;
			this.button7.Click += new System.EventHandler(this.button7_Click);
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(54, 15);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(39, 36);
			this.button4.TabIndex = 10;
			this.button4.Text = "선택";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click_1);
			// 
			// button6
			// 
			this.button6.Location = new System.Drawing.Point(236, 15);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(42, 36);
			this.button6.TabIndex = 9;
			this.button6.Text = "타원";
			this.button6.UseVisualStyleBackColor = true;
			this.button6.Click += new System.EventHandler(this.button6_Click);
			// 
			// mOverlayPane
			// 
			this.mOverlayPane.BackColor = System.Drawing.Color.White;
			this.mOverlayPane.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mOverlayPane.DrawMode = 1;
			this.mOverlayPane.LineColor = System.Drawing.Color.Red;
			this.mOverlayPane.LineThick = 1F;
			this.mOverlayPane.Location = new System.Drawing.Point(0, 62);
			this.mOverlayPane.Name = "mOverlayPane";
			this.mOverlayPane.SelectObject = null;
			this.mOverlayPane.Size = new System.Drawing.Size(703, 405);
			this.mOverlayPane.TabIndex = 0;
			this.mOverlayPane.TextColor = System.Drawing.Color.Black;
			// 
			// button8
			// 
			this.button8.Location = new System.Drawing.Point(484, 27);
			this.button8.Name = "button8";
			this.button8.Size = new System.Drawing.Size(75, 23);
			this.button8.TabIndex = 12;
			this.button8.Text = "button8";
			this.button8.UseVisualStyleBackColor = true;
			this.button8.Click += new System.EventHandler(this.button8_Click);
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(703, 467);
			this.Controls.Add(this.mOverlayPane);
			this.Controls.Add(this.panel2);
			this.DoubleBuffered = true;
			this.Name = "FormMain";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button button6;
		private UnE.Overlay.OverlayPanel mOverlayPane;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button7;
		private System.Windows.Forms.Button button8;
	}
}

