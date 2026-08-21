namespace BIMViewer
{
    partial class FormProperty
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
            this.labelShapeType = new System.Windows.Forms.Label();
            this.textBoxProperty = new System.Windows.Forms.TextBox();
            this.pnSpace = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.lblSpaceID = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblSpaceName = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblSpaceType = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cboSafetyFire = new System.Windows.Forms.ComboBox();
            this.pnWall = new System.Windows.Forms.TableLayoutPanel();
            this.lblWallHeight = new System.Windows.Forms.Label();
            this.lblWallMaterial = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblWallName = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblWallType = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.pnUser = new System.Windows.Forms.TableLayoutPanel();
            this.lblUserPoiHeight = new System.Windows.Forms.Label();
            this.lblUserPoiID = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblUserPoiName = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.btnClose = new UnE.GUI.ImageButton();
            this.pnDoor = new System.Windows.Forms.TableLayoutPanel();
            this.lblDoorID = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.lblDoorType = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.lblDoor = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.pnWindow = new System.Windows.Forms.TableLayoutPanel();
            this.lblWindowID = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.lblWindow = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.pnPOI = new System.Windows.Forms.TableLayoutPanel();
            this.lblPoiChannel = new System.Windows.Forms.Label();
            this.lblPoiAddress = new System.Windows.Forms.Label();
            this.lblPoiLoop = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.lblPoiRx = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.lblPoiHeight = new System.Windows.Forms.Label();
            this.lblPoiID = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.lblPoiType4 = new System.Windows.Forms.Label();
            this.lblPoiType3 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.lblPoiType2 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.lblPoiType1 = new System.Windows.Forms.Label();
            this.pnSpace.SuspendLayout();
            this.pnWall.SuspendLayout();
            this.pnUser.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.pnDoor.SuspendLayout();
            this.pnWindow.SuspendLayout();
            this.pnPOI.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelShapeType
            // 
            this.labelShapeType.AutoSize = true;
            this.labelShapeType.Font = new System.Drawing.Font("돋움", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelShapeType.Location = new System.Drawing.Point(12, 350);
            this.labelShapeType.Name = "labelShapeType";
            this.labelShapeType.Size = new System.Drawing.Size(42, 16);
            this.labelShapeType.TabIndex = 0;
            this.labelShapeType.Text = "타입";
            // 
            // textBoxProperty
            // 
            this.textBoxProperty.BackColor = System.Drawing.Color.White;
            this.textBoxProperty.Location = new System.Drawing.Point(748, 281);
            this.textBoxProperty.Multiline = true;
            this.textBoxProperty.Name = "textBoxProperty";
            this.textBoxProperty.ReadOnly = true;
            this.textBoxProperty.Size = new System.Drawing.Size(298, 164);
            this.textBoxProperty.TabIndex = 3;
            // 
            // pnSpace
            // 
            this.pnSpace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.pnSpace.ColumnCount = 2;
            this.pnSpace.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.683F));
            this.pnSpace.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.317F));
            this.pnSpace.Controls.Add(this.label6, 0, 3);
            this.pnSpace.Controls.Add(this.lblSpaceID, 0, 3);
            this.pnSpace.Controls.Add(this.label5, 0, 2);
            this.pnSpace.Controls.Add(this.lblSpaceName, 1, 1);
            this.pnSpace.Controls.Add(this.label3, 0, 1);
            this.pnSpace.Controls.Add(this.lblSpaceType, 1, 0);
            this.pnSpace.Controls.Add(this.label1, 0, 0);
            this.pnSpace.Controls.Add(this.cboSafetyFire, 1, 2);
            this.pnSpace.Location = new System.Drawing.Point(389, 34);
            this.pnSpace.Name = "pnSpace";
            this.pnSpace.RowCount = 4;
            this.pnSpace.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pnSpace.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pnSpace.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pnSpace.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pnSpace.Size = new System.Drawing.Size(347, 112);
            this.pnSpace.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(0, 84);
            this.label6.Margin = new System.Windows.Forms.Padding(0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(103, 28);
            this.label6.TabIndex = 6;
            this.label6.Text = "ID";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSpaceID
            // 
            this.lblSpaceID.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblSpaceID.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSpaceID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSpaceID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpaceID.Location = new System.Drawing.Point(103, 84);
            this.lblSpaceID.Margin = new System.Windows.Forms.Padding(0);
            this.lblSpaceID.Name = "lblSpaceID";
            this.lblSpaceID.Size = new System.Drawing.Size(244, 28);
            this.lblSpaceID.TabIndex = 5;
            this.lblSpaceID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(0, 56);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 28);
            this.label5.TabIndex = 4;
            this.label5.Text = "방화구획";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSpaceName
            // 
            this.lblSpaceName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblSpaceName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSpaceName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSpaceName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpaceName.Location = new System.Drawing.Point(103, 28);
            this.lblSpaceName.Margin = new System.Windows.Forms.Padding(0);
            this.lblSpaceName.Name = "lblSpaceName";
            this.lblSpaceName.Size = new System.Drawing.Size(244, 28);
            this.lblSpaceName.TabIndex = 3;
            this.lblSpaceName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(0, 28);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 28);
            this.label3.TabIndex = 2;
            this.label3.Text = "객체 명칭";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSpaceType
            // 
            this.lblSpaceType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblSpaceType.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSpaceType.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSpaceType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpaceType.Location = new System.Drawing.Point(103, 0);
            this.lblSpaceType.Margin = new System.Windows.Forms.Padding(0);
            this.lblSpaceType.Name = "lblSpaceType";
            this.lblSpaceType.Size = new System.Drawing.Size(244, 28);
            this.lblSpaceType.TabIndex = 1;
            this.lblSpaceType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 28);
            this.label1.TabIndex = 0;
            this.label1.Text = "객체 분류";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cboSafetyFire
            // 
            this.cboSafetyFire.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.cboSafetyFire.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSafetyFire.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboSafetyFire.FormattingEnabled = true;
            this.cboSafetyFire.Items.AddRange(new object[] {
            "아니오",
            "예"});
            this.cboSafetyFire.Location = new System.Drawing.Point(106, 61);
            this.cboSafetyFire.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.cboSafetyFire.Name = "cboSafetyFire";
            this.cboSafetyFire.Size = new System.Drawing.Size(77, 20);
            this.cboSafetyFire.TabIndex = 2;
            this.cboSafetyFire.SelectedIndexChanged += new System.EventHandler(this.cboSafetyFire_SelectedIndexChanged);
            // 
            // pnWall
            // 
            this.pnWall.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.pnWall.ColumnCount = 2;
            this.pnWall.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.683F));
            this.pnWall.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.317F));
            this.pnWall.Controls.Add(this.lblWallHeight, 1, 3);
            this.pnWall.Controls.Add(this.lblWallMaterial, 1, 2);
            this.pnWall.Controls.Add(this.label10, 0, 3);
            this.pnWall.Controls.Add(this.label4, 0, 2);
            this.pnWall.Controls.Add(this.lblWallName, 1, 1);
            this.pnWall.Controls.Add(this.label7, 0, 1);
            this.pnWall.Controls.Add(this.lblWallType, 1, 0);
            this.pnWall.Controls.Add(this.label9, 0, 0);
            this.pnWall.Location = new System.Drawing.Point(392, 198);
            this.pnWall.Name = "pnWall";
            this.pnWall.RowCount = 4;
            this.pnWall.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pnWall.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pnWall.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pnWall.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pnWall.Size = new System.Drawing.Size(347, 122);
            this.pnWall.TabIndex = 5;
            // 
            // lblWallHeight
            // 
            this.lblWallHeight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblWallHeight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblWallHeight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblWallHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWallHeight.Location = new System.Drawing.Point(103, 90);
            this.lblWallHeight.Margin = new System.Windows.Forms.Padding(0);
            this.lblWallHeight.Name = "lblWallHeight";
            this.lblWallHeight.Size = new System.Drawing.Size(244, 32);
            this.lblWallHeight.TabIndex = 7;
            this.lblWallHeight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblWallMaterial
            // 
            this.lblWallMaterial.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblWallMaterial.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblWallMaterial.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblWallMaterial.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWallMaterial.Location = new System.Drawing.Point(103, 60);
            this.lblWallMaterial.Margin = new System.Windows.Forms.Padding(0);
            this.lblWallMaterial.Name = "lblWallMaterial";
            this.lblWallMaterial.Size = new System.Drawing.Size(244, 30);
            this.lblWallMaterial.TabIndex = 6;
            this.lblWallMaterial.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(0, 90);
            this.label10.Margin = new System.Windows.Forms.Padding(0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(103, 32);
            this.label10.TabIndex = 5;
            this.label10.Text = "높이";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(0, 60);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 30);
            this.label4.TabIndex = 4;
            this.label4.Text = "재질";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblWallName
            // 
            this.lblWallName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblWallName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblWallName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblWallName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWallName.Location = new System.Drawing.Point(103, 30);
            this.lblWallName.Margin = new System.Windows.Forms.Padding(0);
            this.lblWallName.Name = "lblWallName";
            this.lblWallName.Size = new System.Drawing.Size(244, 30);
            this.lblWallName.TabIndex = 3;
            this.lblWallName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(0, 30);
            this.label7.Margin = new System.Windows.Forms.Padding(0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(103, 30);
            this.label7.TabIndex = 2;
            this.label7.Text = "객체 명칭";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblWallType
            // 
            this.lblWallType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblWallType.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblWallType.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblWallType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWallType.Location = new System.Drawing.Point(103, 0);
            this.lblWallType.Margin = new System.Windows.Forms.Padding(0);
            this.lblWallType.Name = "lblWallType";
            this.lblWallType.Size = new System.Drawing.Size(244, 30);
            this.lblWallType.TabIndex = 1;
            this.lblWallType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(0, 0);
            this.label9.Margin = new System.Windows.Forms.Padding(0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(103, 30);
            this.label9.TabIndex = 0;
            this.label9.Text = "객체 분류";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnUser
            // 
            this.pnUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.pnUser.ColumnCount = 2;
            this.pnUser.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.683F));
            this.pnUser.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.317F));
            this.pnUser.Controls.Add(this.lblUserPoiHeight, 1, 2);
            this.pnUser.Controls.Add(this.lblUserPoiID, 1, 1);
            this.pnUser.Controls.Add(this.label8, 0, 2);
            this.pnUser.Controls.Add(this.label11, 0, 1);
            this.pnUser.Controls.Add(this.lblUserPoiName, 1, 0);
            this.pnUser.Controls.Add(this.label14, 0, 0);
            this.pnUser.Location = new System.Drawing.Point(395, 412);
            this.pnUser.Name = "pnUser";
            this.pnUser.RowCount = 3;
            this.pnUser.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.pnUser.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.pnUser.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.pnUser.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.pnUser.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.pnUser.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.pnUser.Size = new System.Drawing.Size(347, 92);
            this.pnUser.TabIndex = 7;
            // 
            // lblUserPoiHeight
            // 
            this.lblUserPoiHeight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblUserPoiHeight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUserPoiHeight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblUserPoiHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserPoiHeight.Location = new System.Drawing.Point(103, 60);
            this.lblUserPoiHeight.Margin = new System.Windows.Forms.Padding(0);
            this.lblUserPoiHeight.Name = "lblUserPoiHeight";
            this.lblUserPoiHeight.Size = new System.Drawing.Size(244, 32);
            this.lblUserPoiHeight.TabIndex = 11;
            this.lblUserPoiHeight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblUserPoiID
            // 
            this.lblUserPoiID.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblUserPoiID.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUserPoiID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblUserPoiID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserPoiID.Location = new System.Drawing.Point(103, 30);
            this.lblUserPoiID.Margin = new System.Windows.Forms.Padding(0);
            this.lblUserPoiID.Name = "lblUserPoiID";
            this.lblUserPoiID.Size = new System.Drawing.Size(244, 30);
            this.lblUserPoiID.TabIndex = 10;
            this.lblUserPoiID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(0, 60);
            this.label8.Margin = new System.Windows.Forms.Padding(0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(103, 32);
            this.label8.TabIndex = 9;
            this.label8.Text = "POI Height";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(0, 30);
            this.label11.Margin = new System.Windows.Forms.Padding(0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(103, 30);
            this.label11.TabIndex = 8;
            this.label11.Text = "POI ID";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblUserPoiName
            // 
            this.lblUserPoiName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblUserPoiName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUserPoiName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblUserPoiName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserPoiName.Location = new System.Drawing.Point(103, 0);
            this.lblUserPoiName.Margin = new System.Windows.Forms.Padding(0);
            this.lblUserPoiName.Name = "lblUserPoiName";
            this.lblUserPoiName.Size = new System.Drawing.Size(244, 30);
            this.lblUserPoiName.TabIndex = 7;
            this.lblUserPoiName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label14
            // 
            this.label14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.Color.White;
            this.label14.Location = new System.Drawing.Point(0, 0);
            this.label14.Margin = new System.Windows.Forms.Padding(0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(103, 30);
            this.label14.TabIndex = 5;
            this.label14.Text = "사용자 POI";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ButtonText = "";
            this.btnClose.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.Image = global::BIMViewer.Properties.Resources.Windowclose__Base;
            this.btnClose.ImageClicked = global::BIMViewer.Properties.Resources.Windowclose_1st_MSover;
            this.btnClose.ImageDisabled = null;
            this.btnClose.ImageMouseOver = global::BIMViewer.Properties.Resources.Windowclose_1st_MSover;
            this.btnClose.ImageNormal = global::BIMViewer.Properties.Resources.Windowclose__Base;
            this.btnClose.Location = new System.Drawing.Point(342, 2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(23, 23);
            this.btnClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnClose.TabIndex = 13;
            this.btnClose.TabStop = false;
            this.btnClose.TextColor = System.Drawing.Color.Black;
            this.btnClose.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ToolTipText = "";
            this.btnClose.UseToolTip = false;
            this.btnClose.WindowRateWidth = 1F;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // pnDoor
            // 
            this.pnDoor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.pnDoor.ColumnCount = 2;
            this.pnDoor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.683F));
            this.pnDoor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.317F));
            this.pnDoor.Controls.Add(this.lblDoorID, 1, 2);
            this.pnDoor.Controls.Add(this.label23, 0, 2);
            this.pnDoor.Controls.Add(this.lblDoorType, 1, 1);
            this.pnDoor.Controls.Add(this.label26, 0, 1);
            this.pnDoor.Controls.Add(this.lblDoor, 1, 0);
            this.pnDoor.Controls.Add(this.label28, 0, 0);
            this.pnDoor.Location = new System.Drawing.Point(15, 369);
            this.pnDoor.Name = "pnDoor";
            this.pnDoor.RowCount = 3;
            this.pnDoor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pnDoor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pnDoor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pnDoor.Size = new System.Drawing.Size(347, 92);
            this.pnDoor.TabIndex = 14;
            // 
            // lblDoorID
            // 
            this.lblDoorID.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblDoorID.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblDoorID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDoorID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDoorID.Location = new System.Drawing.Point(103, 60);
            this.lblDoorID.Margin = new System.Windows.Forms.Padding(0);
            this.lblDoorID.Name = "lblDoorID";
            this.lblDoorID.Size = new System.Drawing.Size(244, 32);
            this.lblDoorID.TabIndex = 6;
            this.lblDoorID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label23
            // 
            this.label23.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label23.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.ForeColor = System.Drawing.Color.White;
            this.label23.Location = new System.Drawing.Point(0, 60);
            this.label23.Margin = new System.Windows.Forms.Padding(0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(103, 32);
            this.label23.TabIndex = 4;
            this.label23.Text = "ID";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDoorType
            // 
            this.lblDoorType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblDoorType.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblDoorType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDoorType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDoorType.Location = new System.Drawing.Point(103, 30);
            this.lblDoorType.Margin = new System.Windows.Forms.Padding(0);
            this.lblDoorType.Name = "lblDoorType";
            this.lblDoorType.Size = new System.Drawing.Size(244, 30);
            this.lblDoorType.TabIndex = 3;
            this.lblDoorType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label26
            // 
            this.label26.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label26.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.ForeColor = System.Drawing.Color.White;
            this.label26.Location = new System.Drawing.Point(0, 30);
            this.label26.Margin = new System.Windows.Forms.Padding(0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(103, 30);
            this.label26.TabIndex = 2;
            this.label26.Text = "문 종류";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDoor
            // 
            this.lblDoor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblDoor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblDoor.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDoor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDoor.Location = new System.Drawing.Point(103, 0);
            this.lblDoor.Margin = new System.Windows.Forms.Padding(0);
            this.lblDoor.Name = "lblDoor";
            this.lblDoor.Size = new System.Drawing.Size(244, 30);
            this.lblDoor.TabIndex = 1;
            this.lblDoor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label28
            // 
            this.label28.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label28.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label28.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.ForeColor = System.Drawing.Color.White;
            this.label28.Location = new System.Drawing.Point(0, 0);
            this.label28.Margin = new System.Windows.Forms.Padding(0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(103, 30);
            this.label28.TabIndex = 0;
            this.label28.Text = "객체 분류";
            this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnWindow
            // 
            this.pnWindow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.pnWindow.ColumnCount = 2;
            this.pnWindow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.683F));
            this.pnWindow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.317F));
            this.pnWindow.Controls.Add(this.lblWindowID, 1, 1);
            this.pnWindow.Controls.Add(this.label19, 0, 1);
            this.pnWindow.Controls.Add(this.lblWindow, 1, 0);
            this.pnWindow.Controls.Add(this.label25, 0, 0);
            this.pnWindow.Location = new System.Drawing.Point(392, 332);
            this.pnWindow.Name = "pnWindow";
            this.pnWindow.RowCount = 2;
            this.pnWindow.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pnWindow.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pnWindow.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.pnWindow.Size = new System.Drawing.Size(347, 63);
            this.pnWindow.TabIndex = 15;
            // 
            // lblWindowID
            // 
            this.lblWindowID.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblWindowID.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblWindowID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblWindowID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWindowID.Location = new System.Drawing.Point(103, 31);
            this.lblWindowID.Margin = new System.Windows.Forms.Padding(0);
            this.lblWindowID.Name = "lblWindowID";
            this.lblWindowID.Size = new System.Drawing.Size(244, 32);
            this.lblWindowID.TabIndex = 3;
            this.lblWindowID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label19
            // 
            this.label19.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label19.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.ForeColor = System.Drawing.Color.White;
            this.label19.Location = new System.Drawing.Point(0, 31);
            this.label19.Margin = new System.Windows.Forms.Padding(0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(103, 32);
            this.label19.TabIndex = 2;
            this.label19.Text = "ID";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblWindow
            // 
            this.lblWindow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblWindow.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblWindow.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblWindow.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWindow.Location = new System.Drawing.Point(103, 0);
            this.lblWindow.Margin = new System.Windows.Forms.Padding(0);
            this.lblWindow.Name = "lblWindow";
            this.lblWindow.Size = new System.Drawing.Size(244, 30);
            this.lblWindow.TabIndex = 1;
            this.lblWindow.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label25
            // 
            this.label25.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label25.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.ForeColor = System.Drawing.Color.White;
            this.label25.Location = new System.Drawing.Point(0, 0);
            this.label25.Margin = new System.Windows.Forms.Padding(0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(103, 31);
            this.label25.TabIndex = 0;
            this.label25.Text = "객체 분류";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnPOI
            // 
            this.pnPOI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.pnPOI.ColumnCount = 2;
            this.pnPOI.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.683F));
            this.pnPOI.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.317F));
            this.pnPOI.Controls.Add(this.lblPoiChannel, 1, 9);
            this.pnPOI.Controls.Add(this.lblPoiAddress, 1, 8);
            this.pnPOI.Controls.Add(this.lblPoiLoop, 1, 7);
            this.pnPOI.Controls.Add(this.label31, 0, 9);
            this.pnPOI.Controls.Add(this.label29, 0, 8);
            this.pnPOI.Controls.Add(this.label24, 0, 7);
            this.pnPOI.Controls.Add(this.lblPoiRx, 1, 6);
            this.pnPOI.Controls.Add(this.label2, 0, 6);
            this.pnPOI.Controls.Add(this.label20, 0, 0);
            this.pnPOI.Controls.Add(this.lblPoiHeight, 1, 5);
            this.pnPOI.Controls.Add(this.lblPoiID, 1, 4);
            this.pnPOI.Controls.Add(this.label22, 0, 5);
            this.pnPOI.Controls.Add(this.label21, 0, 4);
            this.pnPOI.Controls.Add(this.lblPoiType4, 1, 3);
            this.pnPOI.Controls.Add(this.lblPoiType3, 1, 2);
            this.pnPOI.Controls.Add(this.label15, 0, 3);
            this.pnPOI.Controls.Add(this.label16, 0, 2);
            this.pnPOI.Controls.Add(this.lblPoiType2, 1, 1);
            this.pnPOI.Controls.Add(this.label18, 0, 1);
            this.pnPOI.Controls.Add(this.lblPoiType1, 1, 0);
            this.pnPOI.Location = new System.Drawing.Point(12, 34);
            this.pnPOI.Name = "pnPOI";
            this.pnPOI.RowCount = 10;
            this.pnPOI.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.998817F));
            this.pnPOI.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.998814F));
            this.pnPOI.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.998814F));
            this.pnPOI.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.998814F));
            this.pnPOI.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.998814F));
            this.pnPOI.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.998814F));
            this.pnPOI.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.00081F));
            this.pnPOI.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0021F));
            this.pnPOI.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0021F));
            this.pnPOI.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0021F));
            this.pnPOI.Size = new System.Drawing.Size(347, 300);
            this.pnPOI.TabIndex = 16;
            // 
            // lblPoiChannel
            // 
            this.lblPoiChannel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblPoiChannel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPoiChannel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPoiChannel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPoiChannel.Location = new System.Drawing.Point(103, 264);
            this.lblPoiChannel.Margin = new System.Windows.Forms.Padding(0);
            this.lblPoiChannel.Name = "lblPoiChannel";
            this.lblPoiChannel.Size = new System.Drawing.Size(244, 36);
            this.lblPoiChannel.TabIndex = 19;
            this.lblPoiChannel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPoiAddress
            // 
            this.lblPoiAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblPoiAddress.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPoiAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPoiAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPoiAddress.Location = new System.Drawing.Point(103, 234);
            this.lblPoiAddress.Margin = new System.Windows.Forms.Padding(0);
            this.lblPoiAddress.Name = "lblPoiAddress";
            this.lblPoiAddress.Size = new System.Drawing.Size(244, 30);
            this.lblPoiAddress.TabIndex = 18;
            this.lblPoiAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPoiLoop
            // 
            this.lblPoiLoop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblPoiLoop.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPoiLoop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPoiLoop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPoiLoop.Location = new System.Drawing.Point(103, 204);
            this.lblPoiLoop.Margin = new System.Windows.Forms.Padding(0);
            this.lblPoiLoop.Name = "lblPoiLoop";
            this.lblPoiLoop.Size = new System.Drawing.Size(244, 30);
            this.lblPoiLoop.TabIndex = 17;
            this.lblPoiLoop.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label31
            // 
            this.label31.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label31.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label31.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.ForeColor = System.Drawing.Color.White;
            this.label31.Location = new System.Drawing.Point(0, 264);
            this.label31.Margin = new System.Windows.Forms.Padding(0);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(103, 36);
            this.label31.TabIndex = 16;
            this.label31.Text = "POI Channel";
            this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label29
            // 
            this.label29.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label29.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label29.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.ForeColor = System.Drawing.Color.White;
            this.label29.Location = new System.Drawing.Point(0, 234);
            this.label29.Margin = new System.Windows.Forms.Padding(0);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(103, 30);
            this.label29.TabIndex = 15;
            this.label29.Text = "POI Address";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label24
            // 
            this.label24.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label24.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.ForeColor = System.Drawing.Color.White;
            this.label24.Location = new System.Drawing.Point(0, 204);
            this.label24.Margin = new System.Windows.Forms.Padding(0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(103, 30);
            this.label24.TabIndex = 14;
            this.label24.Text = "POI Loop";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPoiRx
            // 
            this.lblPoiRx.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblPoiRx.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPoiRx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPoiRx.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPoiRx.Location = new System.Drawing.Point(103, 174);
            this.lblPoiRx.Margin = new System.Windows.Forms.Padding(0);
            this.lblPoiRx.Name = "lblPoiRx";
            this.lblPoiRx.Size = new System.Drawing.Size(244, 30);
            this.lblPoiRx.TabIndex = 13;
            this.lblPoiRx.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(0, 174);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 30);
            this.label2.TabIndex = 12;
            this.label2.Text = "POI Rx";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label20
            // 
            this.label20.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label20.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.ForeColor = System.Drawing.Color.White;
            this.label20.Location = new System.Drawing.Point(0, 0);
            this.label20.Margin = new System.Windows.Forms.Padding(0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(103, 29);
            this.label20.TabIndex = 0;
            this.label20.Text = "POI 대분류";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPoiHeight
            // 
            this.lblPoiHeight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblPoiHeight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPoiHeight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPoiHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPoiHeight.Location = new System.Drawing.Point(103, 145);
            this.lblPoiHeight.Margin = new System.Windows.Forms.Padding(0);
            this.lblPoiHeight.Name = "lblPoiHeight";
            this.lblPoiHeight.Size = new System.Drawing.Size(244, 29);
            this.lblPoiHeight.TabIndex = 11;
            this.lblPoiHeight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPoiID
            // 
            this.lblPoiID.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblPoiID.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPoiID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPoiID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPoiID.Location = new System.Drawing.Point(103, 116);
            this.lblPoiID.Margin = new System.Windows.Forms.Padding(0);
            this.lblPoiID.Name = "lblPoiID";
            this.lblPoiID.Size = new System.Drawing.Size(244, 29);
            this.lblPoiID.TabIndex = 10;
            this.lblPoiID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label22
            // 
            this.label22.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label22.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.ForeColor = System.Drawing.Color.White;
            this.label22.Location = new System.Drawing.Point(0, 145);
            this.label22.Margin = new System.Windows.Forms.Padding(0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(103, 29);
            this.label22.TabIndex = 9;
            this.label22.Text = "POI Height";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label21
            // 
            this.label21.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.ForeColor = System.Drawing.Color.White;
            this.label21.Location = new System.Drawing.Point(0, 116);
            this.label21.Margin = new System.Windows.Forms.Padding(0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(103, 29);
            this.label21.TabIndex = 8;
            this.label21.Text = "POI ID";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPoiType4
            // 
            this.lblPoiType4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblPoiType4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPoiType4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPoiType4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPoiType4.Location = new System.Drawing.Point(103, 87);
            this.lblPoiType4.Margin = new System.Windows.Forms.Padding(0);
            this.lblPoiType4.Name = "lblPoiType4";
            this.lblPoiType4.Size = new System.Drawing.Size(244, 29);
            this.lblPoiType4.TabIndex = 7;
            this.lblPoiType4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPoiType3
            // 
            this.lblPoiType3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblPoiType3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPoiType3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPoiType3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPoiType3.Location = new System.Drawing.Point(103, 58);
            this.lblPoiType3.Margin = new System.Windows.Forms.Padding(0);
            this.lblPoiType3.Name = "lblPoiType3";
            this.lblPoiType3.Size = new System.Drawing.Size(244, 29);
            this.lblPoiType3.TabIndex = 6;
            this.lblPoiType3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label15
            // 
            this.label15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.ForeColor = System.Drawing.Color.White;
            this.label15.Location = new System.Drawing.Point(0, 87);
            this.label15.Margin = new System.Windows.Forms.Padding(0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(103, 29);
            this.label15.TabIndex = 5;
            this.label15.Text = "POI 상세분류";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label16
            // 
            this.label16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.Color.White;
            this.label16.Location = new System.Drawing.Point(0, 58);
            this.label16.Margin = new System.Windows.Forms.Padding(0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(103, 29);
            this.label16.TabIndex = 4;
            this.label16.Text = "POI 소분류";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPoiType2
            // 
            this.lblPoiType2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblPoiType2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPoiType2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPoiType2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPoiType2.Location = new System.Drawing.Point(103, 29);
            this.lblPoiType2.Margin = new System.Windows.Forms.Padding(0);
            this.lblPoiType2.Name = "lblPoiType2";
            this.lblPoiType2.Size = new System.Drawing.Size(244, 29);
            this.lblPoiType2.TabIndex = 3;
            this.lblPoiType2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label18
            // 
            this.label18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(76)))));
            this.label18.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.ForeColor = System.Drawing.Color.White;
            this.label18.Location = new System.Drawing.Point(0, 29);
            this.label18.Margin = new System.Windows.Forms.Padding(0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(103, 29);
            this.label18.TabIndex = 2;
            this.label18.Text = "POI 중분류";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPoiType1
            // 
            this.lblPoiType1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.lblPoiType1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPoiType1.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPoiType1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPoiType1.Location = new System.Drawing.Point(103, 0);
            this.lblPoiType1.Margin = new System.Windows.Forms.Padding(0);
            this.lblPoiType1.Name = "lblPoiType1";
            this.lblPoiType1.Size = new System.Drawing.Size(244, 29);
            this.lblPoiType1.TabIndex = 1;
            this.lblPoiType1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormProperty
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::BIMViewer.Properties.Resources.background;
            this.ClientSize = new System.Drawing.Size(773, 454);
            this.Controls.Add(this.pnUser);
            this.Controls.Add(this.pnPOI);
            this.Controls.Add(this.pnWindow);
            this.Controls.Add(this.pnDoor);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.pnWall);
            this.Controls.Add(this.pnSpace);
            this.Controls.Add(this.textBoxProperty);
            this.Controls.Add(this.labelShapeType);
            this.Name = "FormProperty";
            this.Text = "속성";
            this.Controls.SetChildIndex(this.labelShapeType, 0);
            this.Controls.SetChildIndex(this.textBoxProperty, 0);
            this.Controls.SetChildIndex(this.pnSpace, 0);
            this.Controls.SetChildIndex(this.pnWall, 0);
            this.Controls.SetChildIndex(this.btnClose, 0);
            this.Controls.SetChildIndex(this.pnDoor, 0);
            this.Controls.SetChildIndex(this.pnWindow, 0);
            this.Controls.SetChildIndex(this.pnPOI, 0);
            this.Controls.SetChildIndex(this.pnUser, 0);
            this.pnSpace.ResumeLayout(false);
            this.pnWall.ResumeLayout(false);
            this.pnUser.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.pnDoor.ResumeLayout(false);
            this.pnWindow.ResumeLayout(false);
            this.pnPOI.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelShapeType;
        private System.Windows.Forms.TextBox textBoxProperty;
        private System.Windows.Forms.TableLayoutPanel pnSpace;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblSpaceName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblSpaceType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboSafetyFire;
        private System.Windows.Forms.TableLayoutPanel pnWall;
        private System.Windows.Forms.Label lblWallHeight;
        private System.Windows.Forms.Label lblWallMaterial;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblWallName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblWallType;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TableLayoutPanel pnUser;
        private System.Windows.Forms.Label lblUserPoiHeight;
        private System.Windows.Forms.Label lblUserPoiID;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblUserPoiName;
        private System.Windows.Forms.Label label14;
        private UnE.GUI.ImageButton btnClose;
        private System.Windows.Forms.TableLayoutPanel pnDoor;
        private System.Windows.Forms.Label lblDoorID;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label lblDoorType;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label lblDoor;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.TableLayoutPanel pnWindow;
        private System.Windows.Forms.Label lblWindowID;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label lblWindow;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblSpaceID;
        private System.Windows.Forms.TableLayoutPanel pnPOI;
        private System.Windows.Forms.Label lblPoiChannel;
        private System.Windows.Forms.Label lblPoiAddress;
        private System.Windows.Forms.Label lblPoiLoop;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label lblPoiRx;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label lblPoiHeight;
        private System.Windows.Forms.Label lblPoiID;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label lblPoiType4;
        private System.Windows.Forms.Label lblPoiType3;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lblPoiType2;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label lblPoiType1;
    }
}