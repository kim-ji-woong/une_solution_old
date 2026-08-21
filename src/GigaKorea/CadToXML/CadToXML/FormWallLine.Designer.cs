namespace CadToXML
{
    partial class FormWallLine
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
            UnE.Geometry.Vertex2D vertex2D1 = new UnE.Geometry.Vertex2D();
            this.dxfControl = new DXFViewer.DXFControl();
            this.label1 = new System.Windows.Forms.Label();
            this.labelWallCount = new System.Windows.Forms.Label();
            this.checkBoxContinue = new System.Windows.Forms.CheckBox();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.labelWallIndex = new System.Windows.Forms.Label();
            this.textBoxWallVertex = new System.Windows.Forms.TextBox();
            this.btnToEnd = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dxfControl
            // 
            this.dxfControl.AntiAliasing = true;
            this.dxfControl.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dxfControl.DrawHatchFirst = true;
            this.dxfControl.ExternalPainter = null;
            this.dxfControl.GroupItemDistance = 30;
            this.dxfControl.GroupItemMinCount = 3;
            this.dxfControl.Location = new System.Drawing.Point(21, 12);
            this.dxfControl.MinimumSize = new System.Drawing.Size(100, 100);
            this.dxfControl.MovedVertex = vertex2D1;
            this.dxfControl.Name = "dxfControl";
            this.dxfControl.ObjectBR = null;
            this.dxfControl.ObjectTL = null;
            this.dxfControl.OpenNRefresh = true;
            this.dxfControl.Panning = false;
            this.dxfControl.PanningMouseButton = System.Windows.Forms.MouseButtons.Middle;
            this.dxfControl.PrintDocument = null;
            this.dxfControl.Renderer = DXFViewer.IPainter.RendererType.GDI_PLUS;
            this.dxfControl.Size = new System.Drawing.Size(533, 397);
            this.dxfControl.TabIndex = 8;
            this.dxfControl.UnitOfLength = DXFViewer.UnitOfLength.MILLIMETER;
            this.dxfControl.UseGroupItem = false;
            this.dxfControl.UseLastViewport = false;
            this.dxfControl.UseMouseWheel = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(574, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "벽체 총 개수 :";
            // 
            // labelWallCount
            // 
            this.labelWallCount.AutoSize = true;
            this.labelWallCount.Location = new System.Drawing.Point(657, 22);
            this.labelWallCount.Name = "labelWallCount";
            this.labelWallCount.Size = new System.Drawing.Size(33, 12);
            this.labelWallCount.TabIndex = 9;
            this.labelWallCount.Text = "몇 개";
            // 
            // checkBoxContinue
            // 
            this.checkBoxContinue.AutoSize = true;
            this.checkBoxContinue.Location = new System.Drawing.Point(576, 47);
            this.checkBoxContinue.Name = "checkBoxContinue";
            this.checkBoxContinue.Size = new System.Drawing.Size(112, 16);
            this.checkBoxContinue.TabIndex = 10;
            this.checkBoxContinue.Text = "연속으로 그리기";
            this.checkBoxContinue.UseVisualStyleBackColor = true;
            this.checkBoxContinue.CheckedChanged += new System.EventHandler(this.checkBoxContinue_CheckedChanged);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(682, 99);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(34, 23);
            this.btnNext.TabIndex = 11;
            this.btnNext.Text = "->";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(574, 99);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(34, 23);
            this.btnPrev.TabIndex = 12;
            this.btnPrev.Text = "<-";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // labelWallIndex
            // 
            this.labelWallIndex.AutoSize = true;
            this.labelWallIndex.Location = new System.Drawing.Point(617, 104);
            this.labelWallIndex.Name = "labelWallIndex";
            this.labelWallIndex.Size = new System.Drawing.Size(57, 12);
            this.labelWallIndex.TabIndex = 13;
            this.labelWallIndex.Text = "벽체 번호";
            // 
            // textBoxWallVertex
            // 
            this.textBoxWallVertex.BackColor = System.Drawing.Color.White;
            this.textBoxWallVertex.Location = new System.Drawing.Point(574, 185);
            this.textBoxWallVertex.Multiline = true;
            this.textBoxWallVertex.Name = "textBoxWallVertex";
            this.textBoxWallVertex.ReadOnly = true;
            this.textBoxWallVertex.Size = new System.Drawing.Size(214, 224);
            this.textBoxWallVertex.TabIndex = 14;
            // 
            // btnToEnd
            // 
            this.btnToEnd.Location = new System.Drawing.Point(735, 99);
            this.btnToEnd.Name = "btnToEnd";
            this.btnToEnd.Size = new System.Drawing.Size(53, 23);
            this.btnToEnd.TabIndex = 15;
            this.btnToEnd.Text = "한번에";
            this.btnToEnd.UseVisualStyleBackColor = true;
            this.btnToEnd.Click += new System.EventHandler(this.btnToEnd_Click);
            // 
            // FormWallLine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnToEnd);
            this.Controls.Add(this.textBoxWallVertex);
            this.Controls.Add(this.labelWallIndex);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.checkBoxContinue);
            this.Controls.Add(this.labelWallCount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dxfControl);
            this.Name = "FormWallLine";
            this.Text = "FormWallLine";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DXFViewer.DXFControl dxfControl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelWallCount;
        private System.Windows.Forms.CheckBox checkBoxContinue;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Label labelWallIndex;
        private System.Windows.Forms.TextBox textBoxWallVertex;
        private System.Windows.Forms.Button btnToEnd;
    }
}