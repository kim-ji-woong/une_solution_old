namespace SDMS
{
    partial class TooltipFireEquipment
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelLastCheckedTime = new System.Windows.Forms.Label();
            this.labelTime = new System.Windows.Forms.Label();
            this.m_TextStatus = new System.Windows.Forms.Label();
            this.shapeContainer2 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.lineShape2 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.labelLastCheckedTime);
            this.panel1.Controls.Add(this.labelTime);
            this.panel1.Controls.Add(this.m_TextStatus);
            this.panel1.Controls.Add(this.shapeContainer2);
            this.panel1.Location = new System.Drawing.Point(3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(142, 106);
            this.panel1.TabIndex = 5;
            // 
            // labelLastCheckedTime
            // 
            this.labelLastCheckedTime.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelLastCheckedTime.ForeColor = System.Drawing.Color.LimeGreen;
            this.labelLastCheckedTime.Location = new System.Drawing.Point(9, 69);
            this.labelLastCheckedTime.Name = "labelLastCheckedTime";
            this.labelLastCheckedTime.Size = new System.Drawing.Size(123, 14);
            this.labelLastCheckedTime.TabIndex = 0;
            this.labelLastCheckedTime.Text = "점검 시간";
            this.labelLastCheckedTime.Visible = false;
            // 
            // labelTime
            // 
            this.labelTime.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTime.ForeColor = System.Drawing.Color.LimeGreen;
            this.labelTime.Location = new System.Drawing.Point(9, 43);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(123, 14);
            this.labelTime.TabIndex = 0;
            this.labelTime.Text = "마지막 점검 일자";
            // 
            // m_TextStatus
            // 
            this.m_TextStatus.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.m_TextStatus.ForeColor = System.Drawing.Color.LimeGreen;
            this.m_TextStatus.Location = new System.Drawing.Point(11, 9);
            this.m_TextStatus.Name = "m_TextStatus";
            this.m_TextStatus.Size = new System.Drawing.Size(123, 14);
            this.m_TextStatus.TabIndex = 0;
            this.m_TextStatus.Text = "상태정보";
            // 
            // shapeContainer2
            // 
            this.shapeContainer2.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer2.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer2.Name = "shapeContainer2";
            this.shapeContainer2.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.lineShape2});
            this.shapeContainer2.Size = new System.Drawing.Size(142, 106);
            this.shapeContainer2.TabIndex = 4;
            this.shapeContainer2.TabStop = false;
            // 
            // lineShape2
            // 
            this.lineShape2.BorderColor = System.Drawing.Color.Gray;
            this.lineShape2.Name = "lineShape2";
            this.lineShape2.X1 = 0;
            this.lineShape2.X2 = 141;
            this.lineShape2.Y1 = 31;
            this.lineShape2.Y2 = 31;
            // 
            // TooltipFireEquipment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(149, 115);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "TooltipFireEquipment";
            this.Text = "TooltipFireEquipment";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label m_TextStatus;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer2;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape2;
        private System.Windows.Forms.Label labelLastCheckedTime;
        private System.Windows.Forms.Label labelTime;

    }
}