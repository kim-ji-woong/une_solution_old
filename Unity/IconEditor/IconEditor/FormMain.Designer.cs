namespace IconEditor
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
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.btnShow2POIs = new System.Windows.Forms.Button();
            this.labelFilePath = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cboFloors = new System.Windows.Forms.ComboBox();
            this.cboBuildings = new System.Windows.Forms.ComboBox();
            this.panel3D = new System.Windows.Forms.Panel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxPOI2Y = new System.Windows.Forms.TextBox();
            this.textBoxPOI1Y = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxPOI2X = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxPOI1X = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.radioMovePOI = new System.Windows.Forms.RadioButton();
            this.radioAddPOI = new System.Windows.Forms.RadioButton();
            this.checkBoxEditPOI = new System.Windows.Forms.CheckBox();
            this.checkBoxRightSide = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.AllowDrop = true;
            this.splitContainerMain.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.splitContainerMain.Panel1.Controls.Add(this.btnShow2POIs);
            this.splitContainerMain.Panel1.Controls.Add(this.labelFilePath);
            this.splitContainerMain.Panel1.Controls.Add(this.labelStatus);
            this.splitContainerMain.Panel1.Controls.Add(this.label2);
            this.splitContainerMain.Panel1.Controls.Add(this.label1);
            this.splitContainerMain.Panel1.Controls.Add(this.cboFloors);
            this.splitContainerMain.Panel1.Controls.Add(this.cboBuildings);
            this.splitContainerMain.Panel1.DragDrop += new System.Windows.Forms.DragEventHandler(this.splitContainerMain_Panel1_DragDrop);
            this.splitContainerMain.Panel1.DragEnter += new System.Windows.Forms.DragEventHandler(this.splitContainerMain_Panel1_DragEnter);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.panel3D);
            this.splitContainerMain.Panel2.Controls.Add(this.panelTop);
            this.splitContainerMain.Size = new System.Drawing.Size(1134, 684);
            this.splitContainerMain.SplitterDistance = 301;
            this.splitContainerMain.TabIndex = 0;
            // 
            // btnShow2POIs
            // 
            this.btnShow2POIs.Location = new System.Drawing.Point(197, 23);
            this.btnShow2POIs.Name = "btnShow2POIs";
            this.btnShow2POIs.Size = new System.Drawing.Size(91, 23);
            this.btnShow2POIs.TabIndex = 4;
            this.btnShow2POIs.Text = "기준점 보이기";
            this.btnShow2POIs.UseVisualStyleBackColor = true;
            this.btnShow2POIs.Click += new System.EventHandler(this.btnShow2POIs_Click);
            // 
            // labelFilePath
            // 
            this.labelFilePath.AutoSize = true;
            this.labelFilePath.Font = new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelFilePath.ForeColor = System.Drawing.Color.SandyBrown;
            this.labelFilePath.Location = new System.Drawing.Point(23, 221);
            this.labelFilePath.Name = "labelFilePath";
            this.labelFilePath.Size = new System.Drawing.Size(54, 14);
            this.labelFilePath.TabIndex = 3;
            this.labelFilePath.Text = "파일 경로";
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelStatus.ForeColor = System.Drawing.Color.SandyBrown;
            this.labelStatus.Location = new System.Drawing.Point(23, 195);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(65, 19);
            this.labelStatus.TabIndex = 3;
            this.labelStatus.Text = "상태정보";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.SandyBrown;
            this.label2.Location = new System.Drawing.Point(12, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "층    :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.SandyBrown;
            this.label1.Location = new System.Drawing.Point(12, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "건물 :";
            // 
            // cboFloors
            // 
            this.cboFloors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFloors.FormattingEnabled = true;
            this.cboFloors.Location = new System.Drawing.Point(55, 49);
            this.cboFloors.Name = "cboFloors";
            this.cboFloors.Size = new System.Drawing.Size(121, 20);
            this.cboFloors.TabIndex = 1;
            this.cboFloors.SelectedIndexChanged += new System.EventHandler(this.cboFloors_SelectedIndexChanged);
            // 
            // cboBuildings
            // 
            this.cboBuildings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuildings.FormattingEnabled = true;
            this.cboBuildings.Location = new System.Drawing.Point(55, 23);
            this.cboBuildings.Name = "cboBuildings";
            this.cboBuildings.Size = new System.Drawing.Size(121, 20);
            this.cboBuildings.TabIndex = 1;
            this.cboBuildings.SelectedIndexChanged += new System.EventHandler(this.cboBuildings_SelectedIndexChanged);
            // 
            // panel3D
            // 
            this.panel3D.BackColor = System.Drawing.Color.Black;
            this.panel3D.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3D.Location = new System.Drawing.Point(0, 100);
            this.panel3D.Name = "panel3D";
            this.panel3D.Size = new System.Drawing.Size(829, 584);
            this.panel3D.TabIndex = 0;
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(80)))));
            this.panelTop.Controls.Add(this.groupBox1);
            this.panelTop.Controls.Add(this.radioMovePOI);
            this.panelTop.Controls.Add(this.radioAddPOI);
            this.panelTop.Controls.Add(this.checkBoxRightSide);
            this.panelTop.Controls.Add(this.checkBoxEditPOI);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(829, 100);
            this.panelTop.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxPOI2Y);
            this.groupBox1.Controls.Add(this.textBoxPOI1Y);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBoxPOI2X);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.textBoxPOI1X);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.ForeColor = System.Drawing.Color.SandyBrown;
            this.groupBox1.Location = new System.Drawing.Point(226, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(310, 80);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "CAD 좌표";
            // 
            // textBoxPOI2Y
            // 
            this.textBoxPOI2Y.Location = new System.Drawing.Point(199, 47);
            this.textBoxPOI2Y.Name = "textBoxPOI2Y";
            this.textBoxPOI2Y.Size = new System.Drawing.Size(88, 21);
            this.textBoxPOI2Y.TabIndex = 1;
            // 
            // textBoxPOI1Y
            // 
            this.textBoxPOI1Y.Location = new System.Drawing.Point(199, 20);
            this.textBoxPOI1Y.Name = "textBoxPOI1Y";
            this.textBoxPOI1Y.Size = new System.Drawing.Size(88, 21);
            this.textBoxPOI1Y.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(183, 50);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(13, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "Y";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(183, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(13, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "Y";
            // 
            // textBoxPOI2X
            // 
            this.textBoxPOI2X.Location = new System.Drawing.Point(80, 47);
            this.textBoxPOI2X.Name = "textBoxPOI2X";
            this.textBoxPOI2X.Size = new System.Drawing.Size(88, 21);
            this.textBoxPOI2X.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(64, 50);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(13, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "X";
            // 
            // textBoxPOI1X
            // 
            this.textBoxPOI1X.Location = new System.Drawing.Point(80, 20);
            this.textBoxPOI1X.Name = "textBoxPOI1X";
            this.textBoxPOI1X.Size = new System.Drawing.Size(88, 21);
            this.textBoxPOI1X.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 50);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "POI2";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(64, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(13, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "X";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "POI1";
            // 
            // radioMovePOI
            // 
            this.radioMovePOI.AutoSize = true;
            this.radioMovePOI.ForeColor = System.Drawing.Color.SandyBrown;
            this.radioMovePOI.Location = new System.Drawing.Point(25, 67);
            this.radioMovePOI.Name = "radioMovePOI";
            this.radioMovePOI.Size = new System.Drawing.Size(71, 16);
            this.radioMovePOI.TabIndex = 4;
            this.radioMovePOI.Text = "POI 이동";
            this.radioMovePOI.UseVisualStyleBackColor = true;
            this.radioMovePOI.CheckedChanged += new System.EventHandler(this.radioPOI_CheckedChanged);
            // 
            // radioAddPOI
            // 
            this.radioAddPOI.AutoSize = true;
            this.radioAddPOI.Checked = true;
            this.radioAddPOI.ForeColor = System.Drawing.Color.SandyBrown;
            this.radioAddPOI.Location = new System.Drawing.Point(25, 43);
            this.radioAddPOI.Name = "radioAddPOI";
            this.radioAddPOI.Size = new System.Drawing.Size(71, 16);
            this.radioAddPOI.TabIndex = 4;
            this.radioAddPOI.TabStop = true;
            this.radioAddPOI.Text = "POI 추가";
            this.radioAddPOI.UseVisualStyleBackColor = true;
            this.radioAddPOI.CheckedChanged += new System.EventHandler(this.radioPOI_CheckedChanged);
            // 
            // checkBoxEditPOI
            // 
            this.checkBoxEditPOI.AutoSize = true;
            this.checkBoxEditPOI.ForeColor = System.Drawing.Color.SandyBrown;
            this.checkBoxEditPOI.Location = new System.Drawing.Point(25, 14);
            this.checkBoxEditPOI.Name = "checkBoxEditPOI";
            this.checkBoxEditPOI.Size = new System.Drawing.Size(76, 16);
            this.checkBoxEditPOI.TabIndex = 3;
            this.checkBoxEditPOI.Text = "Icon 편집";
            this.checkBoxEditPOI.UseVisualStyleBackColor = true;
            this.checkBoxEditPOI.CheckedChanged += new System.EventHandler(this.checkBoxEditPOI_CheckedChanged);
            // 
            // checkBoxRightSide
            // 
            this.checkBoxRightSide.AutoSize = true;
            this.checkBoxRightSide.ForeColor = System.Drawing.Color.SandyBrown;
            this.checkBoxRightSide.Location = new System.Drawing.Point(120, 14);
            this.checkBoxRightSide.Name = "checkBoxRightSide";
            this.checkBoxRightSide.Size = new System.Drawing.Size(72, 16);
            this.checkBoxRightSide.TabIndex = 3;
            this.checkBoxRightSide.Text = "반대방향";
            this.checkBoxRightSide.UseVisualStyleBackColor = true;
            this.checkBoxRightSide.CheckedChanged += new System.EventHandler(this.checkBoxRightSide_CheckedChanged);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1134, 684);
            this.Controls.Add(this.splitContainerMain);
            this.Name = "FormMain";
            this.Text = "유엔이 아이콘 Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResizeBegin += new System.EventHandler(this.FormMain_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.FormMain_ResizeEnd);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel1.PerformLayout();
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Panel panel3D;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboBuildings;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboFloors;
        private System.Windows.Forms.CheckBox checkBoxEditPOI;
        private System.Windows.Forms.RadioButton radioMovePOI;
        private System.Windows.Forms.RadioButton radioAddPOI;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxPOI2Y;
        private System.Windows.Forms.TextBox textBoxPOI1Y;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxPOI2X;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxPOI1X;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label labelFilePath;
        private System.Windows.Forms.Button btnShow2POIs;
        private System.Windows.Forms.CheckBox checkBoxRightSide;
    }
}

