namespace UnE
{
    namespace GUI
    {
        partial class FormNoFrameSizable
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
			this.panelTop = new System.Windows.Forms.Panel();
			this.pictureBoxTitle = new System.Windows.Forms.PictureBox();
			this.btnClose = new System.Windows.Forms.Button();
			this.btnMax = new System.Windows.Forms.Button();
			this.btnMin = new System.Windows.Forms.Button();
			this.labelTitle = new System.Windows.Forms.Label();
			this.panelLeft = new System.Windows.Forms.Panel();
			this.panelRight = new System.Windows.Forms.Panel();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.panelLB = new System.Windows.Forms.Panel();
			this.panelRB = new System.Windows.Forms.Panel();
			this.panelTop.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).BeginInit();
			this.SuspendLayout();
			// 
			// panelTop
			// 
			this.panelTop.BackColor = System.Drawing.Color.Black;
			this.panelTop.Controls.Add(this.pictureBoxTitle);
			this.panelTop.Controls.Add(this.btnMin);
            this.panelTop.Controls.Add(this.btnMax);
            this.panelTop.Controls.Add(this.btnClose);
			this.panelTop.Controls.Add(this.labelTitle);
			this.panelTop.Location = new System.Drawing.Point(0, 0);
			this.panelTop.Name = "panelTop";
			this.panelTop.Size = new System.Drawing.Size(284, 20);
			this.panelTop.TabIndex = 0;
			this.panelTop.DoubleClick += new System.EventHandler(this.panelTop_DoubleClick);
			this.panelTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EdgePanelMouseDown);
			this.panelTop.MouseLeave += new System.EventHandler(this.EdgePanelMouseLeave);
			this.panelTop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.EdgePanelMouseMove);
			this.panelTop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.EdgePanelMouseUp);
			// 
			// pictureBoxTitle
			// 
			this.pictureBoxTitle.BackColor = System.Drawing.Color.Transparent;
			this.pictureBoxTitle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.pictureBoxTitle.Location = new System.Drawing.Point(0, 0);
			this.pictureBoxTitle.Name = "pictureBoxTitle";
			this.pictureBoxTitle.Size = new System.Drawing.Size(22, 20);
			this.pictureBoxTitle.TabIndex = 2;
			this.pictureBoxTitle.TabStop = false;			
			// 
			// btnClose
			// 
			this.btnClose.Font = new System.Drawing.Font("굴림", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.btnClose.Location = new System.Drawing.Point(262, 3);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(17, 15);
			this.btnClose.TabIndex = 1;
			this.btnClose.Text = "X";
			this.btnClose.UseVisualStyleBackColor = true;
			// 
			// btnMax
			// 
			this.btnMax.Font = new System.Drawing.Font("굴림", 6F);
			this.btnMax.Location = new System.Drawing.Point(244, 3);
			this.btnMax.Name = "btnMax";
			this.btnMax.Size = new System.Drawing.Size(17, 15);
			this.btnMax.TabIndex = 1;
			this.btnMax.Text = "ㅁ";
			this.btnMax.UseVisualStyleBackColor = true;
			// 
			// btnMin
			// 
			this.btnMin.Font = new System.Drawing.Font("굴림", 6F);
			this.btnMin.Location = new System.Drawing.Point(226, 3);
			this.btnMin.Name = "btnMin";
			this.btnMin.Size = new System.Drawing.Size(17, 15);
			this.btnMin.TabIndex = 1;
			this.btnMin.Text = "_";
			this.btnMin.UseVisualStyleBackColor = true;
			// 
			// labelTitle
			// 
			this.labelTitle.AutoSize = true;
			this.labelTitle.BackColor = System.Drawing.Color.Transparent;
			this.labelTitle.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.labelTitle.ForeColor = System.Drawing.Color.White;
			this.labelTitle.Location = new System.Drawing.Point(22, 4);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(29, 15);
			this.labelTitle.TabIndex = 0;
			this.labelTitle.Text = "Title";
			this.labelTitle.DoubleClick += new System.EventHandler(this.panelTop_DoubleClick);
			this.labelTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EdgePanelMouseDown);
			this.labelTitle.MouseLeave += new System.EventHandler(this.EdgePanelMouseLeave);
			this.labelTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.EdgePanelMouseMove);
			this.labelTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.EdgePanelMouseUp);
			// 
			// panelLeft
			// 
			this.panelLeft.BackColor = System.Drawing.Color.Black;
			this.panelLeft.Location = new System.Drawing.Point(0, 20);
			this.panelLeft.Name = "panelLeft";
			this.panelLeft.Size = new System.Drawing.Size(5, 237);
			this.panelLeft.TabIndex = 0;
			this.panelLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EdgePanelMouseDown);
			this.panelLeft.MouseLeave += new System.EventHandler(this.EdgePanelMouseLeave);
			this.panelLeft.MouseMove += new System.Windows.Forms.MouseEventHandler(this.EdgePanelMouseMove);
			this.panelLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.EdgePanelMouseUp);
			// 
			// panelRight
			// 
			this.panelRight.BackColor = System.Drawing.Color.Black;
			this.panelRight.Location = new System.Drawing.Point(279, 20);
			this.panelRight.Name = "panelRight";
			this.panelRight.Size = new System.Drawing.Size(5, 237);
			this.panelRight.TabIndex = 0;
			this.panelRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EdgePanelMouseDown);
			this.panelRight.MouseLeave += new System.EventHandler(this.EdgePanelMouseLeave);
			this.panelRight.MouseMove += new System.Windows.Forms.MouseEventHandler(this.EdgePanelMouseMove);
			this.panelRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.EdgePanelMouseUp);
			// 
			// panelBottom
			// 
			this.panelBottom.BackColor = System.Drawing.Color.Black;
			this.panelBottom.Location = new System.Drawing.Point(5, 257);
			this.panelBottom.Name = "panelBottom";
			this.panelBottom.Size = new System.Drawing.Size(274, 5);
			this.panelBottom.TabIndex = 0;
			this.panelBottom.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EdgePanelMouseDown);
			this.panelBottom.MouseLeave += new System.EventHandler(this.EdgePanelMouseLeave);
			this.panelBottom.MouseMove += new System.Windows.Forms.MouseEventHandler(this.EdgePanelMouseMove);
			this.panelBottom.MouseUp += new System.Windows.Forms.MouseEventHandler(this.EdgePanelMouseUp);
			// 
			// panelLB
			// 
			this.panelLB.BackColor = System.Drawing.Color.Black;
			this.panelLB.Location = new System.Drawing.Point(0, 257);
			this.panelLB.Name = "panelLB";
			this.panelLB.Size = new System.Drawing.Size(5, 5);
			this.panelLB.TabIndex = 0;
			this.panelLB.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EdgePanelMouseDown);
			this.panelLB.MouseLeave += new System.EventHandler(this.EdgePanelMouseLeave);
			this.panelLB.MouseMove += new System.Windows.Forms.MouseEventHandler(this.EdgePanelMouseMove);
			this.panelLB.MouseUp += new System.Windows.Forms.MouseEventHandler(this.EdgePanelMouseUp);
			// 
			// panelRB
			// 
			this.panelRB.BackColor = System.Drawing.Color.Black;
			this.panelRB.Location = new System.Drawing.Point(279, 257);
			this.panelRB.Name = "panelRB";
			this.panelRB.Size = new System.Drawing.Size(5, 5);
			this.panelRB.TabIndex = 0;
			this.panelRB.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EdgePanelMouseDown);
			this.panelRB.MouseLeave += new System.EventHandler(this.EdgePanelMouseLeave);
			this.panelRB.MouseMove += new System.Windows.Forms.MouseEventHandler(this.EdgePanelMouseMove);
			this.panelRB.MouseUp += new System.Windows.Forms.MouseEventHandler(this.EdgePanelMouseUp);
			// 
			// FormNoFrameSizable
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.panelRB);
			this.Controls.Add(this.panelLB);
			this.Controls.Add(this.panelBottom);
			this.Controls.Add(this.panelRight);
			this.Controls.Add(this.panelLeft);
			this.Controls.Add(this.panelTop);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "FormNoFrameSizable";
			this.Text = "FormNoFrameSizable";			
			this.panelTop.ResumeLayout(false);
			this.panelTop.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).EndInit();
			this.ResumeLayout(false);

            }

            #endregion

            protected System.Windows.Forms.Panel panelTop;
            protected System.Windows.Forms.Panel panelLeft;
            protected System.Windows.Forms.Panel panelRight;
            protected System.Windows.Forms.Panel panelBottom;
            protected System.Windows.Forms.Panel panelLB;
            protected System.Windows.Forms.Panel panelRB;
            protected System.Windows.Forms.Label labelTitle;
            protected System.Windows.Forms.Button btnClose;
            protected System.Windows.Forms.Button btnMax;
            protected System.Windows.Forms.Button btnMin;
            protected System.Windows.Forms.PictureBox pictureBoxTitle;
        }
    }
}