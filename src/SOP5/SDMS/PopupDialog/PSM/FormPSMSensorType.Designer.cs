namespace SDMS.PopupDialog
{
    partial class FormPSMSensorType
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxTypeName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblUserDefined = new System.Windows.Forms.Label();
            this.textBoxUserDefined = new System.Windows.Forms.TextBox();
            this.lblMonth = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCancel = new UnE.GUI.ImageButton();
            this.btnApply = new UnE.GUI.ImageButton();
            this.btnRemoveType = new UnE.GUI.ImageButton();
            this.cboLifeTime = new UnE.GUI.ImageComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnApply)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRemoveType)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(7, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "센서 타입명 :";
            // 
            // textBoxTypeName
            // 
            this.textBoxTypeName.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxTypeName.Location = new System.Drawing.Point(112, 40);
            this.textBoxTypeName.Name = "textBoxTypeName";
            this.textBoxTypeName.Size = new System.Drawing.Size(140, 27);
            this.textBoxTypeName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(7, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "사용기한     :";
            // 
            // lblUserDefined
            // 
            this.lblUserDefined.AutoSize = true;
            this.lblUserDefined.BackColor = System.Drawing.Color.Transparent;
            this.lblUserDefined.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblUserDefined.ForeColor = System.Drawing.Color.White;
            this.lblUserDefined.Location = new System.Drawing.Point(0, 105);
            this.lblUserDefined.Name = "lblUserDefined";
            this.lblUserDefined.Size = new System.Drawing.Size(273, 17);
            this.lblUserDefined.TabIndex = 3;
            this.lblUserDefined.Text = "사용기한(개월수)을 입력해 주세요.";
            this.lblUserDefined.Visible = false;
            // 
            // textBoxUserDefined
            // 
            this.textBoxUserDefined.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxUserDefined.Location = new System.Drawing.Point(143, 127);
            this.textBoxUserDefined.Name = "textBoxUserDefined";
            this.textBoxUserDefined.Size = new System.Drawing.Size(62, 27);
            this.textBoxUserDefined.TabIndex = 4;
            this.textBoxUserDefined.Visible = false;
            // 
            // lblMonth
            // 
            this.lblMonth.AutoSize = true;
            this.lblMonth.BackColor = System.Drawing.Color.Transparent;
            this.lblMonth.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblMonth.ForeColor = System.Drawing.Color.White;
            this.lblMonth.Location = new System.Drawing.Point(211, 132);
            this.lblMonth.Name = "lblMonth";
            this.lblMonth.Size = new System.Drawing.Size(42, 17);
            this.lblMonth.TabIndex = 5;
            this.lblMonth.Text = "개월";
            this.lblMonth.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.label3.Location = new System.Drawing.Point(8, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 18);
            this.label3.TabIndex = 8;
            this.label3.Text = "센서 타입 정의";
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.ButtonText = "";
            this.btnCancel.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ImageClicked = global::SDMS.Properties.Resources.BtnCancel_Click;
            this.btnCancel.ImageDisabled = null;
            this.btnCancel.ImageMouseOver = global::SDMS.Properties.Resources.BtnCancel_Click;
            this.btnCancel.ImageNormal = global::SDMS.Properties.Resources.BtnCancel_Default;
            this.btnCancel.Location = new System.Drawing.Point(200, 166);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(52, 28);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.TabStop = false;
            this.btnCancel.TextColor = System.Drawing.Color.Black;
            this.btnCancel.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ToolTipText = "";
            this.btnCancel.UseToolTip = false;
            this.btnCancel.WindowRateWidth = 1F;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnApply
            // 
            this.btnApply.BackColor = System.Drawing.Color.Transparent;
            this.btnApply.ButtonText = "";
            this.btnApply.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnApply.ImageClicked = global::SDMS.Properties.Resources.BtnOk_Click;
            this.btnApply.ImageDisabled = null;
            this.btnApply.ImageMouseOver = global::SDMS.Properties.Resources.BtnOk_Click;
            this.btnApply.ImageNormal = global::SDMS.Properties.Resources.BtnOk_Default;
            this.btnApply.Location = new System.Drawing.Point(142, 166);
            this.btnApply.Name = "btnApply";
            this.btnApply.Owner = null;
            this.btnApply.Size = new System.Drawing.Size(52, 28);
            this.btnApply.TabIndex = 16;
            this.btnApply.TabStop = false;
            this.btnApply.TextColor = System.Drawing.Color.Black;
            this.btnApply.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnApply.ToolTipText = "";
            this.btnApply.UseToolTip = false;
            this.btnApply.WindowRateWidth = 1F;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnRemoveType
            // 
            this.btnRemoveType.BackColor = System.Drawing.Color.Transparent;
            this.btnRemoveType.ButtonText = "";
            this.btnRemoveType.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRemoveType.ImageClicked = global::SDMS.Properties.Resources.BtnRemoveType_Click;
            this.btnRemoveType.ImageDisabled = null;
            this.btnRemoveType.ImageMouseOver = global::SDMS.Properties.Resources.BtnRemoveType_Click;
            this.btnRemoveType.ImageNormal = global::SDMS.Properties.Resources.BtnRemoveType_Default;
            this.btnRemoveType.Location = new System.Drawing.Point(12, 166);
            this.btnRemoveType.Name = "btnRemoveType";
            this.btnRemoveType.Owner = null;
            this.btnRemoveType.Size = new System.Drawing.Size(75, 28);
            this.btnRemoveType.TabIndex = 18;
            this.btnRemoveType.TabStop = false;
            this.btnRemoveType.TextColor = System.Drawing.Color.Black;
            this.btnRemoveType.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRemoveType.ToolTipText = "";
            this.btnRemoveType.UseToolTip = false;
            this.btnRemoveType.WindowRateWidth = 1F;
            this.btnRemoveType.Click += new System.EventHandler(this.btnRemoveType_Click);
            // 
            // cboLifeTime
            // 
            this.cboLifeTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLifeTime.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboLifeTime.FormattingEnabled = true;
            this.cboLifeTime.ImageClicked = global::SDMS.Properties.Resources.ComboBoxDropDownBtn_Click;
            this.cboLifeTime.ImageDisabled = null;
            this.cboLifeTime.ImageMouseOver = global::SDMS.Properties.Resources.ComboBoxDropDownBtn_Click;
            this.cboLifeTime.ImageNormal = global::SDMS.Properties.Resources.ComboBoxDropDownBtn3_Default;
            this.cboLifeTime.Items.AddRange(new object[] {
            "모든 탐지 값을 표시",
            "몇 분 동안 표시하지 않습니다",
            "몇 시간 동안 표시하지 않습니다",
            "몇 일 동안 표시하지 않습니다",
            "완전히 표시하지 않습니다"});
            this.cboLifeTime.Location = new System.Drawing.Point(112, 71);
            this.cboLifeTime.Name = "cboLifeTime";
            this.cboLifeTime.Owner = null;
            this.cboLifeTime.Size = new System.Drawing.Size(140, 25);
            this.cboLifeTime.TabIndex = 19;
            this.cboLifeTime.TextColor = System.Drawing.Color.Black;
            this.cboLifeTime.TextFont = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboLifeTime.SelectedIndexChanged += new System.EventHandler(this.cboLifeTime_SelectedIndexChanged);
            // 
            // FormPSMSensorType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SDMS.Properties.Resources.PSMDepartment_Background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(270, 212);
            this.Controls.Add(this.cboLifeTime);
            this.Controls.Add(this.btnRemoveType);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblMonth);
            this.Controls.Add(this.textBoxUserDefined);
            this.Controls.Add(this.lblUserDefined);
            this.Controls.Add(this.textBoxTypeName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FormPSMSensorType";
            this.Text = "센서타입 정의";
            this.Load += new System.EventHandler(this.FormPSMSensorType_Load);
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnApply)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRemoveType)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxTypeName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblUserDefined;
        private System.Windows.Forms.TextBox textBoxUserDefined;
        private System.Windows.Forms.Label lblMonth;
        private System.Windows.Forms.Label label3;
        private UnE.GUI.ImageButton btnCancel;
        private UnE.GUI.ImageButton btnApply;
        private UnE.GUI.ImageButton btnRemoveType;
        private UnE.GUI.ImageComboBox cboLifeTime;
    }
}