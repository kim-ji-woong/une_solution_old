namespace SDMS_Building.Edit
{
    partial class uFormEdit
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

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.eleFloor = new System.Windows.Forms.Integration.ElementHost();
            this.eleBuilding = new System.Windows.Forms.Integration.ElementHost();
            this.eleType = new System.Windows.Forms.Integration.ElementHost();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.eleShapes = new System.Windows.Forms.Integration.ElementHost();
            this.textBoxShapeName = new System.Windows.Forms.TextBox();
            this.btnAdd = new UnE.GUI.ImageButton();
            this.btnRemove = new UnE.GUI.ImageButton();
            this.textBoxShapeX = new System.Windows.Forms.TextBox();
            this.textBoxShapeY = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnConfirm = new UnE.GUI.ImageButton();
            this.textBoxURL = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnApp = new UnE.GUI.ImageButton();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.panelImage = new SDMS_Building.Edit.ImagePanel();
            ((System.ComponentModel.ISupportInitialize)(this.btnAdd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRemove)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnConfirm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnApp)).BeginInit();
            this.SuspendLayout();
            // 
            // eleFloor
            // 
            this.eleFloor.BackColor = System.Drawing.Color.White;
            this.eleFloor.Location = new System.Drawing.Point(291, 10);
            this.eleFloor.Name = "eleFloor";
            this.eleFloor.Size = new System.Drawing.Size(120, 30);
            this.eleFloor.TabIndex = 30;
            this.eleFloor.Text = "elementHost2";
            this.eleFloor.Child = null;
            // 
            // eleBuilding
            // 
            this.eleBuilding.BackColor = System.Drawing.Color.White;
            this.eleBuilding.Location = new System.Drawing.Point(165, 10);
            this.eleBuilding.Name = "eleBuilding";
            this.eleBuilding.Size = new System.Drawing.Size(120, 30);
            this.eleBuilding.TabIndex = 29;
            this.eleBuilding.Text = "elementHost1";
            this.eleBuilding.Child = null;
            // 
            // eleType
            // 
            this.eleType.BackColor = System.Drawing.Color.White;
            this.eleType.Location = new System.Drawing.Point(40, 10);
            this.eleType.Name = "eleType";
            this.eleType.Size = new System.Drawing.Size(120, 30);
            this.eleType.TabIndex = 27;
            this.eleType.Text = "elementHost1";
            this.eleType.Child = null;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(3, 15);
            this.label2.MinimumSize = new System.Drawing.Size(36, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 28);
            this.label2.TabIndex = 26;
            this.label2.Text = "유형";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(502, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 19);
            this.label1.TabIndex = 31;
            this.label1.Text = "이름";
            // 
            // eleShapes
            // 
            this.eleShapes.BackColor = System.Drawing.Color.White;
            this.eleShapes.Location = new System.Drawing.Point(636, 9);
            this.eleShapes.Name = "eleShapes";
            this.eleShapes.Size = new System.Drawing.Size(144, 30);
            this.eleShapes.TabIndex = 32;
            this.eleShapes.Text = "elementHost1";
            this.eleShapes.Child = null;
            // 
            // textBoxShapeName
            // 
            this.textBoxShapeName.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxShapeName.Location = new System.Drawing.Point(538, 12);
            this.textBoxShapeName.Name = "textBoxShapeName";
            this.textBoxShapeName.Size = new System.Drawing.Size(90, 26);
            this.textBoxShapeName.TabIndex = 33;
            // 
            // btnAdd
            // 
            this.btnAdd.ButtonText = "";
            this.btnAdd.ImageClicked = global::SDMS_Building.Properties.Resources.add_click;
            this.btnAdd.ImageDisabled = null;
            this.btnAdd.ImageMouseOver = global::SDMS_Building.Properties.Resources.add_hover;
            this.btnAdd.ImageNormal = global::SDMS_Building.Properties.Resources.add_normal;
            this.btnAdd.Location = new System.Drawing.Point(787, 13);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Owner = null;
            this.btnAdd.Size = new System.Drawing.Size(80, 26);
            this.btnAdd.TabIndex = 35;
            this.btnAdd.TabStop = false;
            this.btnAdd.TextColor = System.Drawing.Color.White;
            this.btnAdd.TextFont = new System.Drawing.Font("나눔바른고딕", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnAdd.ToolTipText = "";
            this.btnAdd.UseToolTip = false;
            this.btnAdd.WindowRateWidth = 1F;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.ButtonText = "";
            this.btnRemove.ImageClicked = global::SDMS_Building.Properties.Resources.delete_click;
            this.btnRemove.ImageDisabled = null;
            this.btnRemove.ImageMouseOver = global::SDMS_Building.Properties.Resources.delete_hover;
            this.btnRemove.ImageNormal = global::SDMS_Building.Properties.Resources.delete_normal;
            this.btnRemove.Location = new System.Drawing.Point(874, 13);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Owner = null;
            this.btnRemove.Size = new System.Drawing.Size(80, 26);
            this.btnRemove.TabIndex = 34;
            this.btnRemove.TabStop = false;
            this.btnRemove.TextColor = System.Drawing.Color.Black;
            this.btnRemove.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRemove.ToolTipText = "";
            this.btnRemove.UseToolTip = false;
            this.btnRemove.WindowRateWidth = 1F;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // textBoxShapeX
            // 
            this.textBoxShapeX.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxShapeX.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxShapeX.Location = new System.Drawing.Point(1002, 11);
            this.textBoxShapeX.Name = "textBoxShapeX";
            this.textBoxShapeX.ReadOnly = true;
            this.textBoxShapeX.Size = new System.Drawing.Size(60, 26);
            this.textBoxShapeX.TabIndex = 36;
            // 
            // textBoxShapeY
            // 
            this.textBoxShapeY.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxShapeY.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxShapeY.Location = new System.Drawing.Point(1085, 11);
            this.textBoxShapeY.Name = "textBoxShapeY";
            this.textBoxShapeY.ReadOnly = true;
            this.textBoxShapeY.Size = new System.Drawing.Size(60, 26);
            this.textBoxShapeY.TabIndex = 37;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(982, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 19);
            this.label4.TabIndex = 38;
            this.label4.Text = "X";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(1066, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(19, 19);
            this.label5.TabIndex = 39;
            this.label5.Text = "Y";
            // 
            // btnConfirm
            // 
            this.btnConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirm.ButtonText = "";
            this.btnConfirm.ImageClicked = global::SDMS_Building.Properties.Resources.editSave_click;
            this.btnConfirm.ImageDisabled = null;
            this.btnConfirm.ImageMouseOver = global::SDMS_Building.Properties.Resources.editSave_hover;
            this.btnConfirm.ImageNormal = global::SDMS_Building.Properties.Resources.editSave_normal;
            this.btnConfirm.Location = new System.Drawing.Point(1498, 12);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Owner = null;
            this.btnConfirm.Size = new System.Drawing.Size(80, 26);
            this.btnConfirm.TabIndex = 40;
            this.btnConfirm.TabStop = false;
            this.btnConfirm.TextColor = System.Drawing.Color.White;
            this.btnConfirm.TextFont = new System.Drawing.Font("나눔바른고딕", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnConfirm.ToolTipText = "";
            this.btnConfirm.UseToolTip = false;
            this.btnConfirm.WindowRateWidth = 1F;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // textBoxURL
            // 
            this.textBoxURL.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxURL.Location = new System.Drawing.Point(1182, 12);
            this.textBoxURL.Name = "textBoxURL";
            this.textBoxURL.Size = new System.Drawing.Size(196, 26);
            this.textBoxURL.TabIndex = 42;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(1146, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 19);
            this.label6.TabIndex = 41;
            this.label6.Text = "URL";
            // 
            // btnApp
            // 
            this.btnApp.ButtonText = "";
            this.btnApp.ImageClicked = global::SDMS_Building.Properties.Resources.app_click;
            this.btnApp.ImageDisabled = null;
            this.btnApp.ImageMouseOver = global::SDMS_Building.Properties.Resources.app_hover;
            this.btnApp.ImageNormal = global::SDMS_Building.Properties.Resources.app_normal;
            this.btnApp.Location = new System.Drawing.Point(1387, 12);
            this.btnApp.Name = "btnApp";
            this.btnApp.Owner = null;
            this.btnApp.Size = new System.Drawing.Size(80, 26);
            this.btnApp.TabIndex = 43;
            this.btnApp.TabStop = false;
            this.btnApp.TextColor = System.Drawing.Color.White;
            this.btnApp.TextFont = new System.Drawing.Font("나눔바른고딕", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnApp.ToolTipText = "";
            this.btnApp.UseToolTip = false;
            this.btnApp.WindowRateWidth = 1F;
            this.btnApp.Click += new System.EventHandler(this.btnApp_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(428, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 19);
            this.label3.TabIndex = 45;
            this.label3.Text = "번호";
            // 
            // textBoxID
            // 
            this.textBoxID.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxID.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBoxID.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxID.Location = new System.Drawing.Point(462, 12);
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.ReadOnly = true;
            this.textBoxID.Size = new System.Drawing.Size(40, 26);
            this.textBoxID.TabIndex = 46;
            // 
            // panelImage
            // 
            this.panelImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelImage.BackColor = System.Drawing.Color.Black;
            this.panelImage.Location = new System.Drawing.Point(0, 54);
            this.panelImage.Name = "panelImage";
            this.panelImage.Owner = null;
            this.panelImage.Size = new System.Drawing.Size(1600, 498);
            this.panelImage.TabIndex = 0;
            // 
            // uFormEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(90)))), ((int)(((byte)(140)))));
            this.Controls.Add(this.textBoxID);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnApp);
            this.Controls.Add(this.textBoxURL);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxShapeY);
            this.Controls.Add(this.textBoxShapeX);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.textBoxShapeName);
            this.Controls.Add(this.eleShapes);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.eleFloor);
            this.Controls.Add(this.eleBuilding);
            this.Controls.Add(this.eleType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panelImage);
            this.Name = "uFormEdit";
            this.Size = new System.Drawing.Size(1600, 552);
            ((System.ComponentModel.ISupportInitialize)(this.btnAdd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRemove)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnConfirm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnApp)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ImagePanel panelImage;
        private System.Windows.Forms.Integration.ElementHost eleFloor;
        private System.Windows.Forms.Integration.ElementHost eleBuilding;
        private System.Windows.Forms.Integration.ElementHost eleType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Integration.ElementHost eleShapes;
        private System.Windows.Forms.TextBox textBoxShapeName;
        private UnE.GUI.ImageButton btnAdd;
        private UnE.GUI.ImageButton btnRemove;
        private System.Windows.Forms.TextBox textBoxShapeX;
        private System.Windows.Forms.TextBox textBoxShapeY;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private UnE.GUI.ImageButton btnConfirm;
        private System.Windows.Forms.TextBox textBoxURL;
        private System.Windows.Forms.Label label6;
        private UnE.GUI.ImageButton btnApp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxID;
    }
}
