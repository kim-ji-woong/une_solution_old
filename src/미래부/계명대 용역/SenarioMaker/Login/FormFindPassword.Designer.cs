namespace UnE.SenarioMaker
{
    partial class FormFindPassword
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
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cboAsk = new System.Windows.Forms.ComboBox();
            this.btnOK = new UnE.GUI.RibbonButton();
            this.button2 = new UnE.GUI.RibbonButton();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxPassword.Location = new System.Drawing.Point(247, 192);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(143, 27);
            this.textBoxPassword.TabIndex = 24;
            this.textBoxPassword.UseSystemPasswordChar = true;
            // 
            // textBoxID
            // 
            this.textBoxID.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxID.Location = new System.Drawing.Point(247, 118);
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.Size = new System.Drawing.Size(143, 27);
            this.textBoxID.TabIndex = 23;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.label3.Location = new System.Drawing.Point(149, 158);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 20);
            this.label3.TabIndex = 27;
            this.label3.Text = "등록한 질문";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.label2.Location = new System.Drawing.Point(172, 121);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 20);
            this.label2.TabIndex = 26;
            this.label2.Text = "아 이 디";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(159)))), ((int)(((byte)(164)))));
            this.label4.Location = new System.Drawing.Point(212, 196);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 20);
            this.label4.TabIndex = 27;
            this.label4.Text = "답";
            // 
            // cboAsk
            // 
            this.cboAsk.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAsk.FormattingEnabled = true;
            this.cboAsk.Items.AddRange(new object[] {
            "내 보물 1호는? ",
            "내가 가장 좋아하는 캐릭터는?",
            "가장 감명 깊게 읽은 책은?",
            "초등학교 때 짝꿍 이름은?",
            "어머니의 고향은?",
            "가장 무서웠던 선생님 이름은?",
            "가장 기억에 남는 장소는?",
            "내가 존경하는 인물은?",
            "다시 태어나면 되고 싶은 것은?",
            "초등학교 시절 나의 꿈은?",
            "우리집 애완동물의 이름은?",
            "나의 출신 초등학교는?"});
            this.cboAsk.Location = new System.Drawing.Point(247, 158);
            this.cboAsk.Name = "cboAsk";
            this.cboAsk.Size = new System.Drawing.Size(215, 20);
            this.cboAsk.TabIndex = 28;
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.Transparent;
            this.btnOK.BackgroundImage = global::UnE.SenarioMaker.Properties.Resources.btnLogin2;
            this.btnOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnOK.CheckButton = false;
            this.btnOK.CheckedBkgndImage = null;
            this.btnOK.CheckedImage = null;
            this.btnOK.ClickedBackgroundImage = null;
            this.btnOK.ClickedImage = null;
            this.btnOK.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnOK.DisabledBkgndImage = null;
            this.btnOK.DisabledImage = null;
            this.btnOK.ID = -1;
            this.btnOK.InitButtonWidth = 115;
            this.btnOK.IsChecked = false;
            this.btnOK.Location = new System.Drawing.Point(176, 245);
            this.btnOK.MouseOverBkgndImage = global::UnE.SenarioMaker.Properties.Resources.btnLogin2_over;
            this.btnOK.MouseOverImage = null;
            this.btnOK.Name = "btnOK";
            this.btnOK.NormalImage = null;
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(115, 33);
            this.btnOK.TabIndex = 29;
            this.btnOK.Text = "확인";
            this.btnOK.TextLocation = new System.Drawing.Point(0, 6);
            this.btnOK.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOK.ToolTipText = "확인";
            this.btnOK.UseCustomImageRect = false;
            this.btnOK.UseTextLocation = true;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Transparent;
            this.button2.BackgroundImage = global::UnE.SenarioMaker.Properties.Resources.btnLogin2;
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button2.CheckButton = false;
            this.button2.CheckedBkgndImage = null;
            this.button2.CheckedImage = null;
            this.button2.ClickedBackgroundImage = null;
            this.button2.ClickedImage = null;
            this.button2.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.button2.DisabledBkgndImage = null;
            this.button2.DisabledImage = null;
            this.button2.ID = -1;
            this.button2.InitButtonWidth = 115;
            this.button2.IsChecked = false;
            this.button2.Location = new System.Drawing.Point(301, 245);
            this.button2.MouseOverBkgndImage = global::UnE.SenarioMaker.Properties.Resources.btnLogin2_over;
            this.button2.MouseOverImage = null;
            this.button2.Name = "button2";
            this.button2.NormalImage = null;
            this.button2.Owner = null;
            this.button2.Size = new System.Drawing.Size(115, 33);
            this.button2.TabIndex = 30;
            this.button2.Text = "취소";
            this.button2.TextLocation = new System.Drawing.Point(0, 6);
            this.button2.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.button2.ToolTipText = "취소";
            this.button2.UseCustomImageRect = false;
            this.button2.UseTextLocation = true;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 17.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(211, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 31);
            this.label1.TabIndex = 21;
            this.label1.Text = "시나리오 생성기";
            // 
            // FormFindPassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::UnE.SenarioMaker.Properties.Resources.LoginMain_bg;
            this.ClientSize = new System.Drawing.Size(605, 335);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.cboAsk);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.textBoxID);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormFindPassword";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "FormFindPassword";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.TextBox textBoxID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboAsk;
        private GUI.RibbonButton btnOK;
        private GUI.RibbonButton button2;
        private System.Windows.Forms.Label label1;
    }
}