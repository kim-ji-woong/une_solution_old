namespace SoilMan.Popup
{
    partial class FormSetArea
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
            this.label1 = new System.Windows.Forms.Label();
            this.radioCircle = new System.Windows.Forms.RadioButton();
            this.radioRectangle = new System.Windows.Forms.RadioButton();
            this.radioPolygon = new System.Windows.Forms.RadioButton();
            this.btnDelete = new UnE.GUI.RibbonButton();
            this.btnDraw = new UnE.GUI.RibbonButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbText = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(178, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "분석대상 영역 설정";
            // 
            // radioCircle
            // 
            this.radioCircle.AutoSize = true;
            this.radioCircle.Checked = true;
            this.radioCircle.Location = new System.Drawing.Point(30, 25);
            this.radioCircle.Name = "radioCircle";
            this.radioCircle.Size = new System.Drawing.Size(103, 16);
            this.radioCircle.TabIndex = 1;
            this.radioCircle.TabStop = true;
            this.radioCircle.Text = "원형 영역 설정";
            this.radioCircle.UseVisualStyleBackColor = true;
            this.radioCircle.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioRectangle
            // 
            this.radioRectangle.AutoSize = true;
            this.radioRectangle.Location = new System.Drawing.Point(30, 56);
            this.radioRectangle.Name = "radioRectangle";
            this.radioRectangle.Size = new System.Drawing.Size(127, 16);
            this.radioRectangle.TabIndex = 1;
            this.radioRectangle.TabStop = true;
            this.radioRectangle.Text = "직사각형 영역 설정";
            this.radioRectangle.UseVisualStyleBackColor = true;
            this.radioRectangle.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioPolygon
            // 
            this.radioPolygon.AutoSize = true;
            this.radioPolygon.Location = new System.Drawing.Point(30, 85);
            this.radioPolygon.Name = "radioPolygon";
            this.radioPolygon.Size = new System.Drawing.Size(103, 16);
            this.radioPolygon.TabIndex = 1;
            this.radioPolygon.TabStop = true;
            this.radioPolygon.Text = "임의 영역 설정";
            this.radioPolygon.UseVisualStyleBackColor = true;
            this.radioPolygon.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // btnDelete
            // 
            this.btnDelete.AutoSize = true;
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.btnDelete.CheckButton = false;
            this.btnDelete.CheckedBkgndImage = null;
            this.btnDelete.CheckedImage = null;
            this.btnDelete.ClickedBackgroundImage = null;
            this.btnDelete.ClickedImage = null;
            this.btnDelete.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnDelete.DisabledBkgndImage = null;
            this.btnDelete.DisabledImage = null;
            this.btnDelete.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.ID = -1;
            this.btnDelete.InitButtonWidth = 38;
            this.btnDelete.IsChecked = false;
            this.btnDelete.Location = new System.Drawing.Point(122, 38);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnDelete.MouseOverBkgndImage = null;
            this.btnDelete.MouseOverImage = null;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.NormalImage = global::SoilMan.Properties.Resources.선택삭제;
            this.btnDelete.Owner = null;
            this.btnDelete.Size = new System.Drawing.Size(38, 38);
            this.btnDelete.TabIndex = 30;
            this.btnDelete.TextLocation = new System.Drawing.Point(0, 0);
            this.btnDelete.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnDelete.ToolTipText = "지정된 선택영역을 클릭하면 삭제합니다";
            this.btnDelete.UseCustomImageRect = false;
            this.btnDelete.UseTextLocation = false;
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDrawType_Click);
            this.btnDelete.MouseEnter += new System.EventHandler(this.btnDelete_MouseEnter);
            this.btnDelete.MouseLeave += new System.EventHandler(this.btnDelete_MouseLeave);
            this.btnDelete.MouseHover += new System.EventHandler(this.btnDelete_MouseHover);
            // 
            // btnDraw
            // 
            this.btnDraw.AutoSize = true;
            this.btnDraw.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.btnDraw.CheckButton = false;
            this.btnDraw.CheckedBkgndImage = null;
            this.btnDraw.CheckedImage = null;
            this.btnDraw.ClickedBackgroundImage = null;
            this.btnDraw.ClickedImage = null;
            this.btnDraw.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnDraw.DisabledBkgndImage = null;
            this.btnDraw.DisabledImage = null;
            this.btnDraw.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnDraw.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDraw.ForeColor = System.Drawing.Color.White;
            this.btnDraw.ID = -1;
            this.btnDraw.InitButtonWidth = 38;
            this.btnDraw.IsChecked = false;
            this.btnDraw.Location = new System.Drawing.Point(46, 38);
            this.btnDraw.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnDraw.MouseOverBkgndImage = null;
            this.btnDraw.MouseOverImage = null;
            this.btnDraw.Name = "btnDraw";
            this.btnDraw.NormalImage = global::SoilMan.Properties.Resources.선택;
            this.btnDraw.Owner = null;
            this.btnDraw.Size = new System.Drawing.Size(38, 38);
            this.btnDraw.TabIndex = 30;
            this.btnDraw.TextLocation = new System.Drawing.Point(0, 0);
            this.btnDraw.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnDraw.ToolTipText = "화면을 클릭시 영역지정 방법으로 필지를 선택합니다.";
            this.btnDraw.UseCustomImageRect = false;
            this.btnDraw.UseTextLocation = false;
            this.btnDraw.UseVisualStyleBackColor = false;
            this.btnDraw.Click += new System.EventHandler(this.btnDrawType_Click);
            this.btnDraw.MouseEnter += new System.EventHandler(this.btnDraw_MouseEnter);
            this.btnDraw.MouseLeave += new System.EventHandler(this.btnDraw_MouseLeave);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioRectangle);
            this.groupBox1.Controls.Add(this.radioCircle);
            this.groupBox1.Controls.Add(this.radioPolygon);
            this.groupBox1.Location = new System.Drawing.Point(17, 47);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(226, 114);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "영역 지정 방법";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbText);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.btnDelete);
            this.groupBox2.Controls.Add(this.btnDraw);
            this.groupBox2.Location = new System.Drawing.Point(17, 177);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(226, 134);
            this.groupBox2.TabIndex = 33;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "마우스 동작 선택";
            // 
            // lbText
            // 
            this.lbText.Location = new System.Drawing.Point(18, 90);
            this.lbText.Name = "lbText";
            this.lbText.Size = new System.Drawing.Size(202, 31);
            this.lbText.TabIndex = 34;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(104, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 33;
            this.label3.Text = "지정영역삭제";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 32;
            this.label2.Text = "영역지정";
            // 
            // FormSetArea
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 378);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormSetArea";
            this.Text = "FormSetArea";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioCircle;
        private System.Windows.Forms.RadioButton radioRectangle;
        private System.Windows.Forms.RadioButton radioPolygon;
        private UnE.GUI.RibbonButton btnDelete;
        private UnE.GUI.RibbonButton btnDraw;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lbText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
    }
}