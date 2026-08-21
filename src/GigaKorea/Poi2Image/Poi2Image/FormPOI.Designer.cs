namespace Poi2Image
{
    partial class FormPOI
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.textBoxMoveY = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxMoveX = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnApplyScale = new System.Windows.Forms.Button();
            this.btnSaveScale = new System.Windows.Forms.Button();
            this.textBoxScale = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnApply);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.textBoxMoveY);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxMoveX);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(682, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(106, 116);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Text 위치 조절";
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(11, 82);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(37, 23);
            this.btnApply.TabIndex = 2;
            this.btnApply.Text = "적용";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(56, 82);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(37, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "저장";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // textBoxMoveY
            // 
            this.textBoxMoveY.Location = new System.Drawing.Point(36, 55);
            this.textBoxMoveY.Name = "textBoxMoveY";
            this.textBoxMoveY.Size = new System.Drawing.Size(57, 21);
            this.textBoxMoveY.TabIndex = 1;
            this.textBoxMoveY.Text = "0";
            this.textBoxMoveY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "Y :";
            // 
            // textBoxMoveX
            // 
            this.textBoxMoveX.Location = new System.Drawing.Point(36, 28);
            this.textBoxMoveX.Name = "textBoxMoveX";
            this.textBoxMoveX.Size = new System.Drawing.Size(57, 21);
            this.textBoxMoveX.TabIndex = 1;
            this.textBoxMoveX.Text = "0";
            this.textBoxMoveX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "X :";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnApplyScale);
            this.groupBox2.Controls.Add(this.btnSaveScale);
            this.groupBox2.Controls.Add(this.textBoxScale);
            this.groupBox2.Location = new System.Drawing.Point(682, 134);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(106, 80);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Scale";
            // 
            // btnApplyScale
            // 
            this.btnApplyScale.Location = new System.Drawing.Point(11, 47);
            this.btnApplyScale.Name = "btnApplyScale";
            this.btnApplyScale.Size = new System.Drawing.Size(37, 23);
            this.btnApplyScale.TabIndex = 2;
            this.btnApplyScale.Text = "적용";
            this.btnApplyScale.UseVisualStyleBackColor = true;
            this.btnApplyScale.Click += new System.EventHandler(this.btnApplyScale_Click);
            // 
            // btnSaveScale
            // 
            this.btnSaveScale.Location = new System.Drawing.Point(56, 47);
            this.btnSaveScale.Name = "btnSaveScale";
            this.btnSaveScale.Size = new System.Drawing.Size(37, 23);
            this.btnSaveScale.TabIndex = 2;
            this.btnSaveScale.Text = "저장";
            this.btnSaveScale.UseVisualStyleBackColor = true;
            this.btnSaveScale.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // textBoxScale
            // 
            this.textBoxScale.Location = new System.Drawing.Point(36, 20);
            this.textBoxScale.Name = "textBoxScale";
            this.textBoxScale.Size = new System.Drawing.Size(57, 21);
            this.textBoxScale.TabIndex = 1;
            this.textBoxScale.Text = "1.0";
            this.textBoxScale.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // FormPOI
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormPOI";
            this.Text = "FormPOI";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.FormPOI_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.FormPOI_DragEnter);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormPOI_Paint);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox textBoxMoveY;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxMoveX;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnApplyScale;
        private System.Windows.Forms.Button btnSaveScale;
        private System.Windows.Forms.TextBox textBoxScale;
    }
}