namespace LandAddressReader
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
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTargetY = new System.Windows.Forms.TextBox();
            this.textBoxTargetX = new System.Windows.Forms.TextBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnOpenDXF = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.labelResult = new System.Windows.Forms.Label();
            this.labelOverLayers = new System.Windows.Forms.Label();
            this.labelEmptyLayers = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 142);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(143, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "Target Moved Vertex Y :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(143, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "Target Moved Vertex X :";
            // 
            // textBoxTargetY
            // 
            this.textBoxTargetY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxTargetY.Location = new System.Drawing.Point(155, 139);
            this.textBoxTargetY.Name = "textBoxTargetY";
            this.textBoxTargetY.Size = new System.Drawing.Size(118, 21);
            this.textBoxTargetY.TabIndex = 6;
            // 
            // textBoxTargetX
            // 
            this.textBoxTargetX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxTargetX.Location = new System.Drawing.Point(155, 112);
            this.textBoxTargetX.Name = "textBoxTargetX";
            this.textBoxTargetX.Size = new System.Drawing.Size(118, 21);
            this.textBoxTargetX.TabIndex = 7;
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExport.Location = new System.Drawing.Point(106, 205);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 10;
            this.btnExport.Text = "내보내기";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnOpenDXF
            // 
            this.btnOpenDXF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpenDXF.Location = new System.Drawing.Point(25, 205);
            this.btnOpenDXF.Name = "btnOpenDXF";
            this.btnOpenDXF.Size = new System.Drawing.Size(75, 23);
            this.btnOpenDXF.TabIndex = 11;
            this.btnOpenDXF.Text = "도면열기";
            this.btnOpenDXF.UseVisualStyleBackColor = true;
            this.btnOpenDXF.Click += new System.EventHandler(this.btnOpenDXF_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClose.Location = new System.Drawing.Point(188, 205);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 10;
            this.btnClose.Text = "닫기";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // labelResult
            // 
            this.labelResult.AutoSize = true;
            this.labelResult.Location = new System.Drawing.Point(23, 39);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(29, 12);
            this.labelResult.TabIndex = 12;
            this.labelResult.Text = "결과";
            this.labelResult.Visible = false;
            // 
            // labelOverLayers
            // 
            this.labelOverLayers.AutoSize = true;
            this.labelOverLayers.Location = new System.Drawing.Point(58, 54);
            this.labelOverLayers.Name = "labelOverLayers";
            this.labelOverLayers.Size = new System.Drawing.Size(112, 12);
            this.labelOverLayers.TabIndex = 12;
            this.labelOverLayers.Text = "Over PolyLine 개수";
            this.labelOverLayers.Visible = false;
            // 
            // labelEmptyLayers
            // 
            this.labelEmptyLayers.AutoSize = true;
            this.labelEmptyLayers.Location = new System.Drawing.Point(58, 69);
            this.labelEmptyLayers.Name = "labelEmptyLayers";
            this.labelEmptyLayers.Size = new System.Drawing.Size(81, 12);
            this.labelEmptyLayers.TabIndex = 12;
            this.labelEmptyLayers.Text = "빈 Layer 개수";
            this.labelEmptyLayers.Visible = false;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(280, 240);
            this.Controls.Add(this.labelEmptyLayers);
            this.Controls.Add(this.labelOverLayers);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnOpenDXF);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxTargetY);
            this.Controls.Add(this.textBoxTargetX);
            this.Name = "FormMain";
            this.Text = "지적도 읽기";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxTargetY;
        private System.Windows.Forms.TextBox textBoxTargetX;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnOpenDXF;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label labelResult;
        private System.Windows.Forms.Label labelOverLayers;
        private System.Windows.Forms.Label labelEmptyLayers;
    }
}

