namespace FireManagement
{
    partial class FormCheckEquip3
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCheckEquip3));
            this.label1 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxOpinion = new System.Windows.Forms.TextBox();
            this.textBoxRFIDTagID = new System.Windows.Forms.TextBox();
            this.textBoxLastCheckedTime = new System.Windows.Forms.TextBox();
            this.textBoxEquipID = new System.Windows.Forms.TextBox();
            this.textBoxEquipType = new System.Windows.Forms.TextBox();
            this.textBoxRFID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.radioManual = new System.Windows.Forms.RadioButton();
            this.radioRFID = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnEquipState = new UnE.GUI.RibbonButton();
            this.pictureBoxGroup = new FireManagement.TextPictureBoxEx();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGroup)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(1, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 40);
            this.label1.TabIndex = 0;
            this.label1.Text = "설비점검";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label13.Location = new System.Drawing.Point(18, 360);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(74, 21);
            this.label13.TabIndex = 50;
            this.label13.Text = "점검의견";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.Location = new System.Drawing.Point(15, 141);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(76, 21);
            this.label8.TabIndex = 44;
            this.label8.Text = "Tag 이름";
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.Location = new System.Drawing.Point(18, 307);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 50);
            this.label5.TabIndex = 40;
            this.label5.Text = "마지막 점검시간";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.Location = new System.Drawing.Point(15, 225);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(74, 21);
            this.label7.TabIndex = 41;
            this.label7.Text = "관리번호";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(15, 270);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 21);
            this.label4.TabIndex = 43;
            this.label4.Text = "설비상태";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(15, 184);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 21);
            this.label3.TabIndex = 42;
            this.label3.Text = "설비종류";
            // 
            // textBoxOpinion
            // 
            this.textBoxOpinion.BackColor = System.Drawing.Color.DarkGray;
            this.textBoxOpinion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxOpinion.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxOpinion.Location = new System.Drawing.Point(103, 362);
            this.textBoxOpinion.Multiline = true;
            this.textBoxOpinion.Name = "textBoxOpinion";
            this.textBoxOpinion.Size = new System.Drawing.Size(296, 72);
            this.textBoxOpinion.TabIndex = 51;
            // 
            // textBoxRFIDTagID
            // 
            this.textBoxRFIDTagID.BackColor = System.Drawing.Color.DarkGray;
            this.textBoxRFIDTagID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxRFIDTagID.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxRFIDTagID.Location = new System.Drawing.Point(103, 139);
            this.textBoxRFIDTagID.Name = "textBoxRFIDTagID";
            this.textBoxRFIDTagID.ReadOnly = true;
            this.textBoxRFIDTagID.Size = new System.Drawing.Size(296, 29);
            this.textBoxRFIDTagID.TabIndex = 39;
            // 
            // textBoxLastCheckedTime
            // 
            this.textBoxLastCheckedTime.BackColor = System.Drawing.Color.DarkGray;
            this.textBoxLastCheckedTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxLastCheckedTime.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxLastCheckedTime.Location = new System.Drawing.Point(102, 314);
            this.textBoxLastCheckedTime.Name = "textBoxLastCheckedTime";
            this.textBoxLastCheckedTime.ReadOnly = true;
            this.textBoxLastCheckedTime.Size = new System.Drawing.Size(296, 29);
            this.textBoxLastCheckedTime.TabIndex = 46;
            // 
            // textBoxEquipID
            // 
            this.textBoxEquipID.BackColor = System.Drawing.Color.DarkGray;
            this.textBoxEquipID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxEquipID.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxEquipID.Location = new System.Drawing.Point(103, 225);
            this.textBoxEquipID.Name = "textBoxEquipID";
            this.textBoxEquipID.ReadOnly = true;
            this.textBoxEquipID.Size = new System.Drawing.Size(296, 29);
            this.textBoxEquipID.TabIndex = 47;
            // 
            // textBoxEquipType
            // 
            this.textBoxEquipType.BackColor = System.Drawing.Color.DarkGray;
            this.textBoxEquipType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxEquipType.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxEquipType.Location = new System.Drawing.Point(103, 181);
            this.textBoxEquipType.Name = "textBoxEquipType";
            this.textBoxEquipType.ReadOnly = true;
            this.textBoxEquipType.Size = new System.Drawing.Size(296, 29);
            this.textBoxEquipType.TabIndex = 45;
            // 
            // textBoxRFID
            // 
            this.textBoxRFID.BackColor = System.Drawing.Color.DarkGray;
            this.textBoxRFID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxRFID.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxRFID.Location = new System.Drawing.Point(103, 98);
            this.textBoxRFID.Name = "textBoxRFID";
            this.textBoxRFID.ReadOnly = true;
            this.textBoxRFID.Size = new System.Drawing.Size(296, 29);
            this.textBoxRFID.TabIndex = 38;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(18, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 21);
            this.label2.TabIndex = 37;
            this.label2.Text = "RFID";
            // 
            // radioManual
            // 
            this.radioManual.AutoSize = true;
            this.radioManual.BackColor = System.Drawing.Color.Transparent;
            this.radioManual.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioManual.Location = new System.Drawing.Point(254, 64);
            this.radioManual.Name = "radioManual";
            this.radioManual.Size = new System.Drawing.Size(98, 25);
            this.radioManual.TabIndex = 56;
            this.radioManual.TabStop = true;
            this.radioManual.Text = "화면 선택";
            this.radioManual.UseVisualStyleBackColor = false;
            this.radioManual.Visible = false;
            // 
            // radioRFID
            // 
            this.radioRFID.AutoSize = true;
            this.radioRFID.BackColor = System.Drawing.Color.Transparent;
            this.radioRFID.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.radioRFID.Location = new System.Drawing.Point(105, 64);
            this.radioRFID.Name = "radioRFID";
            this.radioRFID.Size = new System.Drawing.Size(122, 25);
            this.radioRFID.TabIndex = 55;
            this.radioRFID.TabStop = true;
            this.radioRFID.Text = "RFID Reader";
            this.radioRFID.UseVisualStyleBackColor = false;
            this.radioRFID.Visible = false;
            this.radioRFID.CheckedChanged += new System.EventHandler(this.radioRFID_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.Location = new System.Drawing.Point(16, 65);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 21);
            this.label6.TabIndex = 54;
            this.label6.Text = "설비선택";
            this.label6.Visible = false;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(132)))), ((int)(((byte)(132)))));
            this.button3.BackgroundImage = global::FireManagement.Properties.Resources.DockingBtn_header;
            this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button3.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(132)))), ((int)(((byte)(132)))));
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("맑은 고딕", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button3.Location = new System.Drawing.Point(22, 466);
            this.button3.Name = "button3";
            this.button3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.button3.Size = new System.Drawing.Size(12, 56);
            this.button3.TabIndex = 58;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.buttonComplete_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(132)))), ((int)(((byte)(132)))));
            this.button2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button2.BackgroundImage")));
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button2.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(132)))), ((int)(((byte)(132)))));
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("맑은 고딕", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button2.Location = new System.Drawing.Point(389, 466);
            this.button2.Name = "button2";
            this.button2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.button2.Size = new System.Drawing.Size(10, 55);
            this.button2.TabIndex = 58;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.buttonComplete_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(132)))), ((int)(((byte)(132)))));
            this.button1.BackgroundImage = global::FireManagement.Properties.Resources.DockingBtn_Body;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(132)))), ((int)(((byte)(132)))));
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("맑은 고딕", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button1.Location = new System.Drawing.Point(32, 467);
            this.button1.Name = "button1";
            this.button1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.button1.Size = new System.Drawing.Size(360, 56);
            this.button1.TabIndex = 58;
            this.button1.Text = "저장";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.buttonComplete_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(101, 264);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(7, 37);
            this.pictureBox1.TabIndex = 57;
            this.pictureBox1.TabStop = false;
            // 
            // btnEquipState
            // 
            this.btnEquipState.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnEquipState.BackgroundImage")));
            this.btnEquipState.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnEquipState.CheckedBkgndImage = null;
            this.btnEquipState.CheckedImage = null;
            this.btnEquipState.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnEquipState.DisabledBkgndImage = null;
            this.btnEquipState.DisabledImage = null;
            this.btnEquipState.ID = -1;
            this.btnEquipState.InitButtonWidth = 40;
            this.btnEquipState.IsChecked = false;
            this.btnEquipState.Location = new System.Drawing.Point(358, 267);
            this.btnEquipState.MouseOverBkgndImage = null;
            this.btnEquipState.Name = "btnEquipState";
            this.btnEquipState.NormalImage = null;
            this.btnEquipState.Owner = null;
            this.btnEquipState.Size = new System.Drawing.Size(40, 32);
            this.btnEquipState.TabIndex = 53;
            this.btnEquipState.TextLocation = new System.Drawing.Point(0, 0);
            this.btnEquipState.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnEquipState.UseCustomImageRect = false;
            this.btnEquipState.UseTextLocation = false;
            this.btnEquipState.UseVisualStyleBackColor = true;
            this.btnEquipState.Click += new System.EventHandler(this.btnEquipState_Click);
            // 
            // pictureBoxGroup
            // 
            this.pictureBoxGroup.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxGroup.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxGroup.BackgroundImage")));
            this.pictureBoxGroup.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxGroup.Location = new System.Drawing.Point(105, 264);
            this.pictureBoxGroup.Name = "pictureBoxGroup";
            this.pictureBoxGroup.Size = new System.Drawing.Size(255, 37);
            this.pictureBoxGroup.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxGroup.TabIndex = 52;
            this.pictureBoxGroup.TabStop = false;
            this.pictureBoxGroup.TextColor = System.Drawing.Color.Black;
            // 
            // FormCheckEquip3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(132)))), ((int)(((byte)(132)))));
            this.ClientSize = new System.Drawing.Size(428, 561);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.radioManual);
            this.Controls.Add(this.radioRFID);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnEquipState);
            this.Controls.Add(this.pictureBoxGroup);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxOpinion);
            this.Controls.Add(this.textBoxRFIDTagID);
            this.Controls.Add(this.textBoxLastCheckedTime);
            this.Controls.Add(this.textBoxEquipID);
            this.Controls.Add(this.textBoxEquipType);
            this.Controls.Add(this.textBoxRFID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormCheckEquip3";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "FormCheckEquip";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGroup)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxOpinion;
        private System.Windows.Forms.TextBox textBoxRFIDTagID;
        private System.Windows.Forms.TextBox textBoxLastCheckedTime;
        private System.Windows.Forms.TextBox textBoxEquipID;
        private System.Windows.Forms.TextBox textBoxEquipType;
        private System.Windows.Forms.TextBox textBoxRFID;
        private System.Windows.Forms.Label label2;
        private UnE.GUI.RibbonButton btnEquipState;
        private TextPictureBoxEx pictureBoxGroup;
        private System.Windows.Forms.RadioButton radioManual;
        private System.Windows.Forms.RadioButton radioRFID;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;

    }
}