namespace FireManagement
{
    partial class FormAddEquip2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAddEquip2));
            this.label1 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnFireAlarm = new UnE.GUI.RibbonButton();
            this.btnFirePlug = new UnE.GUI.RibbonButton();
            this.btnFireExtingusher = new UnE.GUI.RibbonButton();
            this.pictureBoxCircle03 = new UnE.GUI.TextPictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.btnComplete = new System.Windows.Forms.Button();
            this.checkContinueAdd = new System.Windows.Forms.CheckBox();
            this.checkBoxUseScren = new System.Windows.Forms.CheckBox();
            this.textBoxY = new System.Windows.Forms.TextBox();
            this.textBoxX = new System.Windows.Forms.TextBox();
            this.textBoxLocationName = new System.Windows.Forms.TextBox();
            this.textBoxEquipID = new System.Windows.Forms.TextBox();
            this.textBoxRFID = new System.Windows.Forms.TextBox();
            this.textBoxRFIDTagID = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.radioManual = new System.Windows.Forms.RadioButton();
            this.radioRFID = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBoxCircle02 = new UnE.GUI.TextPictureBox();
            this.pictureBoxCircle01 = new UnE.GUI.TextPictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCircle03)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCircle02)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCircle01)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(2)))), ((int)(((byte)(2)))));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 40);
            this.label1.TabIndex = 0;
            this.label1.Text = "설비 추가";
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnClose.BackgroundImage")));
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(349, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(70, 68);
            this.btnClose.TabIndex = 24;
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnFireAlarm
            // 
            this.btnFireAlarm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFireAlarm.CheckedBkgndImage = ((System.Drawing.Image)(resources.GetObject("btnFireAlarm.CheckedBkgndImage")));
            this.btnFireAlarm.CheckedImage = ((System.Drawing.Image)(resources.GetObject("btnFireAlarm.CheckedImage")));
            this.btnFireAlarm.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnFireAlarm.DisabledBkgndImage = null;
            this.btnFireAlarm.DisabledImage = ((System.Drawing.Image)(resources.GetObject("btnFireAlarm.DisabledImage")));
            this.btnFireAlarm.ID = -1;
            this.btnFireAlarm.InitButtonWidth = 70;
            this.btnFireAlarm.IsChecked = false;
            this.btnFireAlarm.Location = new System.Drawing.Point(207, 64);
            this.btnFireAlarm.MouseOverBkgndImage = null;
            this.btnFireAlarm.Name = "btnFireAlarm";
            this.btnFireAlarm.NormalImage = ((System.Drawing.Image)(resources.GetObject("btnFireAlarm.NormalImage")));
            this.btnFireAlarm.Owner = null;
            this.btnFireAlarm.Size = new System.Drawing.Size(70, 76);
            this.btnFireAlarm.TabIndex = 23;
            this.btnFireAlarm.TextLocation = new System.Drawing.Point(0, 0);
            this.btnFireAlarm.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFireAlarm.UseCustomImageRect = false;
            this.btnFireAlarm.UseTextLocation = false;
            this.btnFireAlarm.UseVisualStyleBackColor = true;
            this.btnFireAlarm.Click += new System.EventHandler(this.btnFireAlarm_Click);
            // 
            // btnFirePlug
            // 
            this.btnFirePlug.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFirePlug.CheckedBkgndImage = ((System.Drawing.Image)(resources.GetObject("btnFirePlug.CheckedBkgndImage")));
            this.btnFirePlug.CheckedImage = ((System.Drawing.Image)(resources.GetObject("btnFirePlug.CheckedImage")));
            this.btnFirePlug.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnFirePlug.DisabledBkgndImage = null;
            this.btnFirePlug.DisabledImage = ((System.Drawing.Image)(resources.GetObject("btnFirePlug.DisabledImage")));
            this.btnFirePlug.ID = -1;
            this.btnFirePlug.InitButtonWidth = 70;
            this.btnFirePlug.IsChecked = false;
            this.btnFirePlug.Location = new System.Drawing.Point(119, 64);
            this.btnFirePlug.MouseOverBkgndImage = null;
            this.btnFirePlug.Name = "btnFirePlug";
            this.btnFirePlug.NormalImage = ((System.Drawing.Image)(resources.GetObject("btnFirePlug.NormalImage")));
            this.btnFirePlug.Owner = null;
            this.btnFirePlug.Size = new System.Drawing.Size(70, 76);
            this.btnFirePlug.TabIndex = 22;
            this.btnFirePlug.TextLocation = new System.Drawing.Point(0, 0);
            this.btnFirePlug.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFirePlug.UseCustomImageRect = false;
            this.btnFirePlug.UseTextLocation = false;
            this.btnFirePlug.UseVisualStyleBackColor = true;
            this.btnFirePlug.Click += new System.EventHandler(this.btnFirePlug_Click);
            // 
            // btnFireExtingusher
            // 
            this.btnFireExtingusher.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFireExtingusher.CheckedBkgndImage = ((System.Drawing.Image)(resources.GetObject("btnFireExtingusher.CheckedBkgndImage")));
            this.btnFireExtingusher.CheckedImage = ((System.Drawing.Image)(resources.GetObject("btnFireExtingusher.CheckedImage")));
            this.btnFireExtingusher.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnFireExtingusher.DisabledBkgndImage = null;
            this.btnFireExtingusher.DisabledImage = ((System.Drawing.Image)(resources.GetObject("btnFireExtingusher.DisabledImage")));
            this.btnFireExtingusher.ID = -1;
            this.btnFireExtingusher.InitButtonWidth = 70;
            this.btnFireExtingusher.IsChecked = true;
            this.btnFireExtingusher.Location = new System.Drawing.Point(33, 64);
            this.btnFireExtingusher.MouseOverBkgndImage = null;
            this.btnFireExtingusher.Name = "btnFireExtingusher";
            this.btnFireExtingusher.NormalImage = ((System.Drawing.Image)(resources.GetObject("btnFireExtingusher.NormalImage")));
            this.btnFireExtingusher.Owner = null;
            this.btnFireExtingusher.Size = new System.Drawing.Size(70, 76);
            this.btnFireExtingusher.TabIndex = 21;
            this.btnFireExtingusher.TextLocation = new System.Drawing.Point(0, 0);
            this.btnFireExtingusher.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnFireExtingusher.UseCustomImageRect = false;
            this.btnFireExtingusher.UseTextLocation = false;
            this.btnFireExtingusher.UseVisualStyleBackColor = true;
            this.btnFireExtingusher.Click += new System.EventHandler(this.btnFireExtingusher_Click);
            // 
            // pictureBoxCircle03
            // 
            this.pictureBoxCircle03.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCircle03.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxCircle03.BackgroundImage")));
            this.pictureBoxCircle03.Location = new System.Drawing.Point(297, 594);
            this.pictureBoxCircle03.Name = "pictureBoxCircle03";
            this.pictureBoxCircle03.Size = new System.Drawing.Size(18, 18);
            this.pictureBoxCircle03.TabIndex = 20;
            this.pictureBoxCircle03.TabStop = false;
            this.pictureBoxCircle03.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.btnComplete);
            this.panel1.Controls.Add(this.checkContinueAdd);
            this.panel1.Controls.Add(this.checkBoxUseScren);
            this.panel1.Controls.Add(this.textBoxY);
            this.panel1.Controls.Add(this.textBoxX);
            this.panel1.Controls.Add(this.textBoxLocationName);
            this.panel1.Controls.Add(this.textBoxEquipID);
            this.panel1.Controls.Add(this.textBoxRFID);
            this.panel1.Controls.Add(this.textBoxRFIDTagID);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.radioManual);
            this.panel1.Controls.Add(this.radioRFID);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(15, 140);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(406, 456);
            this.panel1.TabIndex = 17;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.Control;
            this.button1.BackgroundImage = global::FireManagement.Properties.Resources.Docking_nomal_Button;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(132)))), ((int)(((byte)(132)))));
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("맑은 고딕", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button1.Location = new System.Drawing.Point(33, 365);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(165, 38);
            this.button1.TabIndex = 33;
            this.button1.Text = "취소";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnComplete
            // 
            this.btnComplete.BackColor = System.Drawing.SystemColors.Control;
            this.btnComplete.BackgroundImage = global::FireManagement.Properties.Resources.Docking_nomal_Button;
            this.btnComplete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnComplete.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(132)))), ((int)(((byte)(132)))));
            this.btnComplete.FlatAppearance.BorderSize = 0;
            this.btnComplete.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.btnComplete.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnComplete.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnComplete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnComplete.Font = new System.Drawing.Font("맑은 고딕", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnComplete.Location = new System.Drawing.Point(210, 365);
            this.btnComplete.Name = "btnComplete";
            this.btnComplete.Size = new System.Drawing.Size(165, 38);
            this.btnComplete.TabIndex = 33;
            this.btnComplete.Text = "추가";
            this.btnComplete.UseVisualStyleBackColor = false;
            this.btnComplete.Click += new System.EventHandler(this.buttonComplete_Click);
            // 
            // checkContinueAdd
            // 
            this.checkContinueAdd.AutoSize = true;
            this.checkContinueAdd.BackColor = System.Drawing.Color.Transparent;
            this.checkContinueAdd.Font = new System.Drawing.Font("맑은 고딕", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkContinueAdd.ForeColor = System.Drawing.Color.Black;
            this.checkContinueAdd.Location = new System.Drawing.Point(39, 405);
            this.checkContinueAdd.Name = "checkContinueAdd";
            this.checkContinueAdd.Size = new System.Drawing.Size(103, 27);
            this.checkContinueAdd.TabIndex = 29;
            this.checkContinueAdd.Text = "연속 추가";
            this.checkContinueAdd.UseVisualStyleBackColor = false;
            // 
            // checkBoxUseScren
            // 
            this.checkBoxUseScren.AutoSize = true;
            this.checkBoxUseScren.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxUseScren.Font = new System.Drawing.Font("맑은 고딕", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxUseScren.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.checkBoxUseScren.Location = new System.Drawing.Point(204, 329);
            this.checkBoxUseScren.Name = "checkBoxUseScren";
            this.checkBoxUseScren.Size = new System.Drawing.Size(177, 27);
            this.checkBoxUseScren.TabIndex = 29;
            this.checkBoxUseScren.Text = "화면에서 위치 지정";
            this.checkBoxUseScren.UseVisualStyleBackColor = false;
            this.checkBoxUseScren.CheckedChanged += new System.EventHandler(this.checkBoxUseScren_CheckedChanged_1);
            // 
            // textBoxY
            // 
            this.textBoxY.BackColor = System.Drawing.Color.DarkGray;
            this.textBoxY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxY.Font = new System.Drawing.Font("맑은 고딕", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxY.Location = new System.Drawing.Point(143, 286);
            this.textBoxY.Name = "textBoxY";
            this.textBoxY.Size = new System.Drawing.Size(238, 30);
            this.textBoxY.TabIndex = 27;
            // 
            // textBoxX
            // 
            this.textBoxX.BackColor = System.Drawing.Color.DarkGray;
            this.textBoxX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxX.Font = new System.Drawing.Font("맑은 고딕", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxX.Location = new System.Drawing.Point(143, 243);
            this.textBoxX.Name = "textBoxX";
            this.textBoxX.Size = new System.Drawing.Size(238, 30);
            this.textBoxX.TabIndex = 27;
            // 
            // textBoxLocationName
            // 
            this.textBoxLocationName.BackColor = System.Drawing.Color.DarkGray;
            this.textBoxLocationName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxLocationName.Font = new System.Drawing.Font("맑은 고딕", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxLocationName.Location = new System.Drawing.Point(119, 201);
            this.textBoxLocationName.Name = "textBoxLocationName";
            this.textBoxLocationName.Size = new System.Drawing.Size(262, 30);
            this.textBoxLocationName.TabIndex = 27;
            // 
            // textBoxEquipID
            // 
            this.textBoxEquipID.BackColor = System.Drawing.Color.DarkGray;
            this.textBoxEquipID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxEquipID.Font = new System.Drawing.Font("맑은 고딕", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxEquipID.Location = new System.Drawing.Point(119, 142);
            this.textBoxEquipID.Name = "textBoxEquipID";
            this.textBoxEquipID.Size = new System.Drawing.Size(262, 30);
            this.textBoxEquipID.TabIndex = 27;
            // 
            // textBoxRFID
            // 
            this.textBoxRFID.BackColor = System.Drawing.Color.DarkGray;
            this.textBoxRFID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxRFID.Font = new System.Drawing.Font("맑은 고딕", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxRFID.Location = new System.Drawing.Point(120, 63);
            this.textBoxRFID.Name = "textBoxRFID";
            this.textBoxRFID.Size = new System.Drawing.Size(262, 30);
            this.textBoxRFID.TabIndex = 27;
            // 
            // textBoxRFIDTagID
            // 
            this.textBoxRFIDTagID.BackColor = System.Drawing.Color.DarkGray;
            this.textBoxRFIDTagID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxRFIDTagID.Font = new System.Drawing.Font("맑은 고딕", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxRFIDTagID.Location = new System.Drawing.Point(119, 102);
            this.textBoxRFIDTagID.Name = "textBoxRFIDTagID";
            this.textBoxRFIDTagID.Size = new System.Drawing.Size(262, 30);
            this.textBoxRFIDTagID.TabIndex = 27;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("맑은 고딕", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(115, 289);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(20, 23);
            this.label8.TabIndex = 26;
            this.label8.Text = "Y";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("맑은 고딕", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(116, 243);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(21, 23);
            this.label7.TabIndex = 25;
            this.label7.Text = "X";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(43, 204);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 23);
            this.label5.TabIndex = 23;
            this.label5.Text = "위치명";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(29, 145);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 23);
            this.label4.TabIndex = 22;
            this.label4.Text = "관리번호";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(29, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 23);
            this.label3.TabIndex = 21;
            this.label3.Text = "Tag 이름";
            // 
            // radioManual
            // 
            this.radioManual.AutoSize = true;
            this.radioManual.BackColor = System.Drawing.Color.Transparent;
            this.radioManual.Font = new System.Drawing.Font("맑은 고딕", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioManual.ForeColor = System.Drawing.Color.Black;
            this.radioManual.Location = new System.Drawing.Point(285, 29);
            this.radioManual.Name = "radioManual";
            this.radioManual.Size = new System.Drawing.Size(96, 27);
            this.radioManual.TabIndex = 20;
            this.radioManual.TabStop = true;
            this.radioManual.Text = "수동입력";
            this.radioManual.UseVisualStyleBackColor = false;
            this.radioManual.CheckedChanged += new System.EventHandler(this.radioManual_CheckedChanged);
            // 
            // radioRFID
            // 
            this.radioRFID.AutoSize = true;
            this.radioRFID.BackColor = System.Drawing.Color.Transparent;
            this.radioRFID.Font = new System.Drawing.Font("맑은 고딕", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioRFID.ForeColor = System.Drawing.Color.Black;
            this.radioRFID.Location = new System.Drawing.Point(119, 29);
            this.radioRFID.Name = "radioRFID";
            this.radioRFID.Size = new System.Drawing.Size(128, 27);
            this.radioRFID.TabIndex = 19;
            this.radioRFID.TabStop = true;
            this.radioRFID.Text = "RFID Reader";
            this.radioRFID.UseVisualStyleBackColor = false;
            this.radioRFID.CheckedChanged += new System.EventHandler(this.radioRFID_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Font = new System.Drawing.Font("맑은 고딕", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(43, 54);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 23);
            this.label9.TabIndex = 18;
            this.label9.Text = "(필수)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(43, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 23);
            this.label2.TabIndex = 18;
            this.label2.Text = "RFID";
            // 
            // pictureBoxCircle02
            // 
            this.pictureBoxCircle02.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCircle02.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxCircle02.BackgroundImage")));
            this.pictureBoxCircle02.Location = new System.Drawing.Point(203, 594);
            this.pictureBoxCircle02.Name = "pictureBoxCircle02";
            this.pictureBoxCircle02.Size = new System.Drawing.Size(18, 18);
            this.pictureBoxCircle02.TabIndex = 19;
            this.pictureBoxCircle02.TabStop = false;
            this.pictureBoxCircle02.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // pictureBoxCircle01
            // 
            this.pictureBoxCircle01.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCircle01.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxCircle01.BackgroundImage")));
            this.pictureBoxCircle01.Location = new System.Drawing.Point(108, 594);
            this.pictureBoxCircle01.Name = "pictureBoxCircle01";
            this.pictureBoxCircle01.Size = new System.Drawing.Size(18, 18);
            this.pictureBoxCircle01.TabIndex = 18;
            this.pictureBoxCircle01.TabStop = false;
            this.pictureBoxCircle01.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // FormAddEquip2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(132)))), ((int)(((byte)(132)))));
            this.ClientSize = new System.Drawing.Size(423, 625);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnFireAlarm);
            this.Controls.Add(this.btnFirePlug);
            this.Controls.Add(this.btnFireExtingusher);
            this.Controls.Add(this.pictureBoxCircle03);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBoxCircle02);
            this.Controls.Add(this.pictureBoxCircle01);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormAddEquip2";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "FormAddEquip2";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormAddEquip2_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCircle03)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCircle02)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCircle01)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioManual;
        private System.Windows.Forms.RadioButton radioRFID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxLocationName;
        private System.Windows.Forms.TextBox textBoxEquipID;
        private System.Windows.Forms.TextBox textBoxRFIDTagID;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBoxUseScren;
        private System.Windows.Forms.TextBox textBoxY;
        private System.Windows.Forms.TextBox textBoxRFID;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox checkContinueAdd;
        private System.Windows.Forms.TextBox textBoxX;
        private UnE.GUI.TextPictureBox pictureBoxCircle03;
        private UnE.GUI.TextPictureBox pictureBoxCircle02;
        private UnE.GUI.TextPictureBox pictureBoxCircle01;
        private UnE.GUI.RibbonButton btnFireAlarm;
        private UnE.GUI.RibbonButton btnFirePlug;
        private UnE.GUI.RibbonButton btnFireExtingusher;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnComplete;
    }
}