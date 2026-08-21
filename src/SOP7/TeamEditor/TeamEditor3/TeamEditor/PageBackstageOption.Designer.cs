namespace TeamEditor
{
    partial class PageBackstageOption
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
            this.lblCaption = new System.Windows.Forms.Label();
            this.grpTree = new System.Windows.Forms.GroupBox();
            this.btnTreeInit = new System.Windows.Forms.Button();
            this.btnTreeFont = new System.Windows.Forms.Button();
            this.btnTreeBack = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.greGrid = new System.Windows.Forms.GroupBox();
            this.btnGridInit = new System.Windows.Forms.Button();
            this.btnGridFont = new System.Windows.Forms.Button();
            this.btnGridBack = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.btnSave = new System.Windows.Forms.Button();
            this.grpTree.SuspendLayout();
            this.greGrid.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblCaption
            // 
            this.lblCaption.BackColor = System.Drawing.Color.White;
            this.lblCaption.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblCaption.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(59)))), ((int)(((byte)(59)))));
            this.lblCaption.Location = new System.Drawing.Point(30, 10);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblCaption.Size = new System.Drawing.Size(500, 35);
            this.lblCaption.TabIndex = 20;
            this.lblCaption.Text = "환경설정";
            // 
            // grpTree
            // 
            this.grpTree.Controls.Add(this.btnTreeInit);
            this.grpTree.Controls.Add(this.btnTreeFont);
            this.grpTree.Controls.Add(this.btnTreeBack);
            this.grpTree.Controls.Add(this.label2);
            this.grpTree.Controls.Add(this.label1);
            this.grpTree.Location = new System.Drawing.Point(35, 72);
            this.grpTree.Name = "grpTree";
            this.grpTree.Size = new System.Drawing.Size(160, 112);
            this.grpTree.TabIndex = 21;
            this.grpTree.TabStop = false;
            this.grpTree.Text = "조직도 색상 정의";
            // 
            // btnTreeInit
            // 
            this.btnTreeInit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTreeInit.Location = new System.Drawing.Point(6, 83);
            this.btnTreeInit.Name = "btnTreeInit";
            this.btnTreeInit.Size = new System.Drawing.Size(148, 23);
            this.btnTreeInit.TabIndex = 8;
            this.btnTreeInit.Text = "기본값 적용";
            this.btnTreeInit.UseVisualStyleBackColor = true;
            this.btnTreeInit.Click += new System.EventHandler(this.btnTreeInit_Click);
            // 
            // btnTreeFont
            // 
            this.btnTreeFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTreeFont.BackColor = System.Drawing.Color.Black;
            this.btnTreeFont.Location = new System.Drawing.Point(99, 54);
            this.btnTreeFont.Name = "btnTreeFont";
            this.btnTreeFont.Size = new System.Drawing.Size(39, 21);
            this.btnTreeFont.TabIndex = 7;
            this.btnTreeFont.UseVisualStyleBackColor = false;
            // 
            // btnTreeBack
            // 
            this.btnTreeBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTreeBack.BackColor = System.Drawing.Color.White;
            this.btnTreeBack.Location = new System.Drawing.Point(99, 27);
            this.btnTreeBack.Name = "btnTreeBack";
            this.btnTreeBack.Size = new System.Drawing.Size(39, 21);
            this.btnTreeBack.TabIndex = 6;
            this.btnTreeBack.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "폰트";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "배경";
            // 
            // greGrid
            // 
            this.greGrid.Controls.Add(this.btnGridInit);
            this.greGrid.Controls.Add(this.btnGridFont);
            this.greGrid.Controls.Add(this.btnGridBack);
            this.greGrid.Controls.Add(this.label3);
            this.greGrid.Controls.Add(this.label4);
            this.greGrid.Location = new System.Drawing.Point(230, 72);
            this.greGrid.Name = "greGrid";
            this.greGrid.Size = new System.Drawing.Size(160, 112);
            this.greGrid.TabIndex = 22;
            this.greGrid.TabStop = false;
            this.greGrid.Text = "조직원 색상 정의";
            // 
            // btnGridInit
            // 
            this.btnGridInit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGridInit.Location = new System.Drawing.Point(6, 83);
            this.btnGridInit.Name = "btnGridInit";
            this.btnGridInit.Size = new System.Drawing.Size(148, 23);
            this.btnGridInit.TabIndex = 9;
            this.btnGridInit.Text = "기본값 적용";
            this.btnGridInit.UseVisualStyleBackColor = true;
            this.btnGridInit.Click += new System.EventHandler(this.btnGridInit_Click);
            // 
            // btnGridFont
            // 
            this.btnGridFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGridFont.BackColor = System.Drawing.Color.Black;
            this.btnGridFont.Location = new System.Drawing.Point(99, 54);
            this.btnGridFont.Name = "btnGridFont";
            this.btnGridFont.Size = new System.Drawing.Size(39, 21);
            this.btnGridFont.TabIndex = 5;
            this.btnGridFont.UseVisualStyleBackColor = false;
            // 
            // btnGridBack
            // 
            this.btnGridBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGridBack.BackColor = System.Drawing.Color.White;
            this.btnGridBack.Location = new System.Drawing.Point(99, 27);
            this.btnGridBack.Name = "btnGridBack";
            this.btnGridBack.Size = new System.Drawing.Size(39, 21);
            this.btnGridBack.TabIndex = 4;
            this.btnGridBack.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "폰트";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "배경";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(35, 195);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 23;
            this.btnSave.Text = "저장";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // PageBackstageOption
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(833, 605);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.greGrid);
            this.Controls.Add(this.grpTree);
            this.Controls.Add(this.lblCaption);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PageBackstageOption";
            this.Text = "PageBackstageOption";
            this.grpTree.ResumeLayout(false);
            this.grpTree.PerformLayout();
            this.greGrid.ResumeLayout(false);
            this.greGrid.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.GroupBox grpTree;
        private System.Windows.Forms.Button btnTreeFont;
        private System.Windows.Forms.Button btnTreeBack;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox greGrid;
        private System.Windows.Forms.Button btnGridFont;
        private System.Windows.Forms.Button btnGridBack;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Button btnTreeInit;
        private System.Windows.Forms.Button btnGridInit;
        private System.Windows.Forms.Button btnSave;
    }
}