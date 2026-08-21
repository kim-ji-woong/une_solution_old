namespace DXFUtility
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
            this.btnZoneBoundary = new System.Windows.Forms.Button();
            this.btnBuildingBoundary = new System.Windows.Forms.Button();
            this.btnBuildingDXF = new System.Windows.Forms.Button();
            this.btnDXFToDB = new System.Windows.Forms.Button();
            this.btnMakeBuildingZone = new System.Windows.Forms.Button();
            this.btnCSVToDB = new System.Windows.Forms.Button();
            this.btnFireEquipmentToDBFinal = new System.Windows.Forms.Button();
            this.btnMakeEquipmentZone = new System.Windows.Forms.Button();
            this.btnUpdateFireEquipmentTemp = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnZoneBoundary
            // 
            this.btnZoneBoundary.Location = new System.Drawing.Point(12, 114);
            this.btnZoneBoundary.Name = "btnZoneBoundary";
            this.btnZoneBoundary.Size = new System.Drawing.Size(300, 31);
            this.btnZoneBoundary.TabIndex = 0;
            this.btnZoneBoundary.Text = "DXF로부터 Zone Boundary 추출하여 DB에 저장";
            this.btnZoneBoundary.UseVisualStyleBackColor = true;
            this.btnZoneBoundary.Click += new System.EventHandler(this.btnZoneBoundary_Click);
            // 
            // btnBuildingBoundary
            // 
            this.btnBuildingBoundary.Location = new System.Drawing.Point(12, 62);
            this.btnBuildingBoundary.Name = "btnBuildingBoundary";
            this.btnBuildingBoundary.Size = new System.Drawing.Size(300, 31);
            this.btnBuildingBoundary.TabIndex = 0;
            this.btnBuildingBoundary.Text = "DXF로부터 건물 Boundary 추출하여 DB에 저장";
            this.btnBuildingBoundary.UseVisualStyleBackColor = true;
            this.btnBuildingBoundary.Click += new System.EventHandler(this.btnBuildingBoundary_Click);
            // 
            // btnBuildingDXF
            // 
            this.btnBuildingDXF.Location = new System.Drawing.Point(12, 168);
            this.btnBuildingDXF.Name = "btnBuildingDXF";
            this.btnBuildingDXF.Size = new System.Drawing.Size(300, 31);
            this.btnBuildingDXF.TabIndex = 0;
            this.btnBuildingDXF.Text = "DXF 파일 경로를 DB에 저장";
            this.btnBuildingDXF.UseVisualStyleBackColor = true;
            this.btnBuildingDXF.Click += new System.EventHandler(this.btnBuildingDXF_Click);
            // 
            // btnDXFToDB
            // 
            this.btnDXFToDB.Location = new System.Drawing.Point(12, 219);
            this.btnDXFToDB.Name = "btnDXFToDB";
            this.btnDXFToDB.Size = new System.Drawing.Size(300, 31);
            this.btnDXFToDB.TabIndex = 0;
            this.btnDXFToDB.Text = "DXF를 읽어 DB에 설비정보 저장";
            this.btnDXFToDB.UseVisualStyleBackColor = true;
            this.btnDXFToDB.Click += new System.EventHandler(this.btnDXFToDB_Click);
            // 
            // btnMakeBuildingZone
            // 
            this.btnMakeBuildingZone.Location = new System.Drawing.Point(12, 12);
            this.btnMakeBuildingZone.Name = "btnMakeBuildingZone";
            this.btnMakeBuildingZone.Size = new System.Drawing.Size(300, 31);
            this.btnMakeBuildingZone.TabIndex = 0;
            this.btnMakeBuildingZone.Text = "DXF로부터 Building 정보를 읽어 Zone 생성";
            this.btnMakeBuildingZone.UseVisualStyleBackColor = true;
            this.btnMakeBuildingZone.Click += new System.EventHandler(this.btnMakeBuildingZone_Click);
            // 
            // btnCSVToDB
            // 
            this.btnCSVToDB.Location = new System.Drawing.Point(12, 271);
            this.btnCSVToDB.Name = "btnCSVToDB";
            this.btnCSVToDB.Size = new System.Drawing.Size(300, 31);
            this.btnCSVToDB.TabIndex = 0;
            this.btnCSVToDB.Text = "CSV에서 RFID Tag 추출하여 DB에 설비정보 저장";
            this.btnCSVToDB.UseVisualStyleBackColor = true;
            this.btnCSVToDB.Click += new System.EventHandler(this.btnCSVToDB_Click);
            // 
            // btnFireEquipmentToDBFinal
            // 
            this.btnFireEquipmentToDBFinal.Location = new System.Drawing.Point(12, 321);
            this.btnFireEquipmentToDBFinal.Name = "btnFireEquipmentToDBFinal";
            this.btnFireEquipmentToDBFinal.Size = new System.Drawing.Size(300, 31);
            this.btnFireEquipmentToDBFinal.TabIndex = 0;
            this.btnFireEquipmentToDBFinal.Text = "설비 ID정보 File에서 읽어 DB에 최종 갱신";
            this.btnFireEquipmentToDBFinal.UseVisualStyleBackColor = true;
            this.btnFireEquipmentToDBFinal.Click += new System.EventHandler(this.btnFireEquipmentToDBFinal_Click);
            // 
            // btnMakeEquipmentZone
            // 
            this.btnMakeEquipmentZone.Location = new System.Drawing.Point(395, 12);
            this.btnMakeEquipmentZone.Name = "btnMakeEquipmentZone";
            this.btnMakeEquipmentZone.Size = new System.Drawing.Size(300, 31);
            this.btnMakeEquipmentZone.TabIndex = 0;
            this.btnMakeEquipmentZone.Text = "DXF로부터 설비 Zone 정보를 읽어 설비 Zone 생성";
            this.btnMakeEquipmentZone.UseVisualStyleBackColor = true;
            this.btnMakeEquipmentZone.Click += new System.EventHandler(this.btnMakeEquipmentZone_Click);
            // 
            // btnUpdateFireEquipmentTemp
            // 
            this.btnUpdateFireEquipmentTemp.Location = new System.Drawing.Point(395, 62);
            this.btnUpdateFireEquipmentTemp.Name = "btnUpdateFireEquipmentTemp";
            this.btnUpdateFireEquipmentTemp.Size = new System.Drawing.Size(300, 47);
            this.btnUpdateFireEquipmentTemp.TabIndex = 0;
            this.btnUpdateFireEquipmentTemp.Text = "FireEquipment로부터 도면 정보를 읽어 FireEquipmentTemp Table에 집어넣기";
            this.btnUpdateFireEquipmentTemp.UseVisualStyleBackColor = true;
            this.btnUpdateFireEquipmentTemp.Click += new System.EventHandler(this.btnUpdateFireEquipmentTemp_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(708, 364);
            this.Controls.Add(this.btnFireEquipmentToDBFinal);
            this.Controls.Add(this.btnCSVToDB);
            this.Controls.Add(this.btnDXFToDB);
            this.Controls.Add(this.btnBuildingDXF);
            this.Controls.Add(this.btnBuildingBoundary);
            this.Controls.Add(this.btnUpdateFireEquipmentTemp);
            this.Controls.Add(this.btnMakeEquipmentZone);
            this.Controls.Add(this.btnMakeBuildingZone);
            this.Controls.Add(this.btnZoneBoundary);
            this.Name = "FormMain";
            this.Text = "DXF Utility";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnZoneBoundary;
        private System.Windows.Forms.Button btnBuildingBoundary;
        private System.Windows.Forms.Button btnBuildingDXF;
        private System.Windows.Forms.Button btnDXFToDB;
        private System.Windows.Forms.Button btnMakeBuildingZone;
        private System.Windows.Forms.Button btnCSVToDB;
        private System.Windows.Forms.Button btnFireEquipmentToDBFinal;
        private System.Windows.Forms.Button btnMakeEquipmentZone;
        private System.Windows.Forms.Button btnUpdateFireEquipmentTemp;
    }
}

