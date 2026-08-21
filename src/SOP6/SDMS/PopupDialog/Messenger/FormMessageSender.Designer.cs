namespace SDMS.PopupDialog
{
    partial class FormMessageSender
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMessageSender));
            this.rtbBody = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxSenderName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.btnCancel = new UnE.GUI.ImageButton();
            this.btnOK = new UnE.GUI.ImageButton();
            this.btnShowReceiveForm = new UnE.GUI.ImageButton();
            this.btnBold = new UnE.GUI.ImageButton();
            this.btnItalic = new UnE.GUI.ImageButton();
            this.btnUnderline = new UnE.GUI.ImageButton();
            this.btnStrikeout = new UnE.GUI.ImageButton();
            this.btnFont = new UnE.GUI.ImageButton();
            this.btnColor = new UnE.GUI.ImageButton();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnShowReceiveForm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnBold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnItalic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnUnderline)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnStrikeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFont)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnColor)).BeginInit();
            this.SuspendLayout();
            // 
            // rtbBody
            // 
            this.rtbBody.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rtbBody.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rtbBody.Location = new System.Drawing.Point(14, 154);
            this.rtbBody.Name = "rtbBody";
            this.rtbBody.Size = new System.Drawing.Size(382, 203);
            this.rtbBody.TabIndex = 7;
            this.rtbBody.Text = "";
            this.rtbBody.SelectionChanged += new System.EventHandler(this.rtbBody_SelectionChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(10, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(385, 16);
            this.label1.TabIndex = 10;
            this.label1.Text = "스마트 재난관리 시스템을 사용하는 전체 사용자에게";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(10, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(183, 16);
            this.label2.TabIndex = 10;
            this.label2.Text = "알림 메시지를 보냅니다.";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(10, 366);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 18);
            this.label3.TabIndex = 11;
            this.label3.Text = "작성자 :";
            // 
            // textBoxSenderName
            // 
            this.textBoxSenderName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxSenderName.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxSenderName.Location = new System.Drawing.Point(78, 363);
            this.textBoxSenderName.Name = "textBoxSenderName";
            this.textBoxSenderName.Size = new System.Drawing.Size(122, 27);
            this.textBoxSenderName.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(10, 126);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 18);
            this.label4.TabIndex = 13;
            this.label4.Text = "제목 :";
            // 
            // textBoxTitle
            // 
            this.textBoxTitle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxTitle.Font = new System.Drawing.Font("굴림", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxTitle.Location = new System.Drawing.Point(67, 123);
            this.textBoxTitle.Name = "textBoxTitle";
            this.textBoxTitle.Size = new System.Drawing.Size(329, 27);
            this.textBoxTitle.TabIndex = 6;
            // 
            // btnCancel
            // 
            this.btnCancel.ButtonText = "";
            this.btnCancel.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ImageClicked = global::SDMS.Properties.Resources.MessageSender_Cancel_Click;
            this.btnCancel.ImageDisabled = null;
            this.btnCancel.ImageMouseOver = global::SDMS.Properties.Resources.MessageSender_Cancel_Click;
            this.btnCancel.ImageNormal = global::SDMS.Properties.Resources.MessageSender_Cancel_Default;
            this.btnCancel.Location = new System.Drawing.Point(344, 363);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(52, 27);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.TabStop = false;
            this.btnCancel.TextColor = System.Drawing.Color.Black;
            this.btnCancel.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ToolTipText = "";
            this.btnCancel.UseToolTip = false;
            this.btnCancel.WindowRateWidth = 1F;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.ButtonText = "";
            this.btnOK.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.ImageClicked = global::SDMS.Properties.Resources.MessageSender_Send_Click;
            this.btnOK.ImageDisabled = null;
            this.btnOK.ImageMouseOver = global::SDMS.Properties.Resources.MessageSender_Send_Click;
            this.btnOK.ImageNormal = global::SDMS.Properties.Resources.MessageSender_Send_Default;
            this.btnOK.Location = new System.Drawing.Point(286, 363);
            this.btnOK.Name = "btnOK";
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(52, 27);
            this.btnOK.TabIndex = 15;
            this.btnOK.TabStop = false;
            this.btnOK.TextColor = System.Drawing.Color.Black;
            this.btnOK.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOK.ToolTipText = "";
            this.btnOK.UseToolTip = false;
            this.btnOK.WindowRateWidth = 1F;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnShowReceiveForm
            // 
            this.btnShowReceiveForm.ButtonText = "";
            this.btnShowReceiveForm.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnShowReceiveForm.ImageClicked = global::SDMS.Properties.Resources.MessageSender_ViewMsg_Click;
            this.btnShowReceiveForm.ImageDisabled = null;
            this.btnShowReceiveForm.ImageMouseOver = global::SDMS.Properties.Resources.MessageSender_ViewMsg_Click;
            this.btnShowReceiveForm.ImageNormal = global::SDMS.Properties.Resources.MessageSender_ViewMsg_Default;
            this.btnShowReceiveForm.Location = new System.Drawing.Point(296, 91);
            this.btnShowReceiveForm.Name = "btnShowReceiveForm";
            this.btnShowReceiveForm.Owner = null;
            this.btnShowReceiveForm.Size = new System.Drawing.Size(100, 26);
            this.btnShowReceiveForm.TabIndex = 16;
            this.btnShowReceiveForm.TabStop = false;
            this.btnShowReceiveForm.TextColor = System.Drawing.Color.Black;
            this.btnShowReceiveForm.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnShowReceiveForm.ToolTipText = "";
            this.btnShowReceiveForm.UseToolTip = false;
            this.btnShowReceiveForm.WindowRateWidth = 1F;
            this.btnShowReceiveForm.Click += new System.EventHandler(this.btnShowReceiveForm_Click);
            // 
            // btnBold
            // 
            this.btnBold.ButtonText = "";
            this.btnBold.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnBold.ImageClicked = global::SDMS.Properties.Resources.MessageSender_Bold_Click;
            this.btnBold.ImageDisabled = null;
            this.btnBold.ImageMouseOver = global::SDMS.Properties.Resources.MessageSender_Bold_Click;
            this.btnBold.ImageNormal = global::SDMS.Properties.Resources.MessageSender_Bold;
            this.btnBold.Location = new System.Drawing.Point(14, 91);
            this.btnBold.Name = "btnBold";
            this.btnBold.Owner = null;
            this.btnBold.Size = new System.Drawing.Size(26, 26);
            this.btnBold.TabIndex = 18;
            this.btnBold.TabStop = false;
            this.btnBold.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(45)))), ((int)(((byte)(40)))));
            this.btnBold.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnBold.ToolTipText = "";
            this.btnBold.UseToolTip = false;
            this.btnBold.WindowRateWidth = 1F;
            this.btnBold.Click += new System.EventHandler(this.btnBold_Click);
            // 
            // btnItalic
            // 
            this.btnItalic.ButtonText = "";
            this.btnItalic.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnItalic.ImageClicked = global::SDMS.Properties.Resources.MessageSender_Italic_Click;
            this.btnItalic.ImageDisabled = null;
            this.btnItalic.ImageMouseOver = global::SDMS.Properties.Resources.MessageSender_Italic_Click;
            this.btnItalic.ImageNormal = global::SDMS.Properties.Resources.MessageSender_Italic;
            this.btnItalic.Location = new System.Drawing.Point(46, 91);
            this.btnItalic.Name = "btnItalic";
            this.btnItalic.Owner = null;
            this.btnItalic.Size = new System.Drawing.Size(26, 26);
            this.btnItalic.TabIndex = 19;
            this.btnItalic.TabStop = false;
            this.btnItalic.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(45)))), ((int)(((byte)(40)))));
            this.btnItalic.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnItalic.ToolTipText = "";
            this.btnItalic.UseToolTip = false;
            this.btnItalic.WindowRateWidth = 1F;
            this.btnItalic.Click += new System.EventHandler(this.btnItalic_Click);
            // 
            // btnUnderline
            // 
            this.btnUnderline.ButtonText = "";
            this.btnUnderline.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnUnderline.ImageClicked = global::SDMS.Properties.Resources.MessageSender_Underline_Click;
            this.btnUnderline.ImageDisabled = null;
            this.btnUnderline.ImageMouseOver = global::SDMS.Properties.Resources.MessageSender_Underline_Click;
            this.btnUnderline.ImageNormal = global::SDMS.Properties.Resources.MessageSender_Underline;
            this.btnUnderline.Location = new System.Drawing.Point(78, 91);
            this.btnUnderline.Name = "btnUnderline";
            this.btnUnderline.Owner = null;
            this.btnUnderline.Size = new System.Drawing.Size(26, 26);
            this.btnUnderline.TabIndex = 20;
            this.btnUnderline.TabStop = false;
            this.btnUnderline.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(45)))), ((int)(((byte)(40)))));
            this.btnUnderline.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnUnderline.ToolTipText = "";
            this.btnUnderline.UseToolTip = false;
            this.btnUnderline.WindowRateWidth = 1F;
            this.btnUnderline.Click += new System.EventHandler(this.btnUnderline_Click);
            // 
            // btnStrikeout
            // 
            this.btnStrikeout.ButtonText = "";
            this.btnStrikeout.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnStrikeout.ImageClicked = global::SDMS.Properties.Resources.MessageSender_Strikout_Click;
            this.btnStrikeout.ImageDisabled = null;
            this.btnStrikeout.ImageMouseOver = global::SDMS.Properties.Resources.MessageSender_Strikout_Click;
            this.btnStrikeout.ImageNormal = global::SDMS.Properties.Resources.MessageSender_Strikout;
            this.btnStrikeout.Location = new System.Drawing.Point(110, 91);
            this.btnStrikeout.Name = "btnStrikeout";
            this.btnStrikeout.Owner = null;
            this.btnStrikeout.Size = new System.Drawing.Size(26, 26);
            this.btnStrikeout.TabIndex = 21;
            this.btnStrikeout.TabStop = false;
            this.btnStrikeout.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(45)))), ((int)(((byte)(40)))));
            this.btnStrikeout.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnStrikeout.ToolTipText = "";
            this.btnStrikeout.UseToolTip = false;
            this.btnStrikeout.WindowRateWidth = 1F;
            this.btnStrikeout.Click += new System.EventHandler(this.btnStrikeout_Click);
            // 
            // btnFont
            // 
            this.btnFont.ButtonText = "";
            this.btnFont.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnFont.ImageClicked = global::SDMS.Properties.Resources.MessageSender_Font_Click;
            this.btnFont.ImageDisabled = null;
            this.btnFont.ImageMouseOver = global::SDMS.Properties.Resources.MessageSender_Font_Click;
            this.btnFont.ImageNormal = global::SDMS.Properties.Resources.MessageSender_Font;
            this.btnFont.Location = new System.Drawing.Point(174, 91);
            this.btnFont.Name = "btnFont";
            this.btnFont.Owner = null;
            this.btnFont.Size = new System.Drawing.Size(26, 26);
            this.btnFont.TabIndex = 22;
            this.btnFont.TabStop = false;
            this.btnFont.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(45)))), ((int)(((byte)(40)))));
            this.btnFont.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnFont.ToolTipText = "";
            this.btnFont.UseToolTip = false;
            this.btnFont.WindowRateWidth = 1F;
            this.btnFont.Click += new System.EventHandler(this.btnFont_Click);
            // 
            // btnColor
            // 
            this.btnColor.ButtonText = "";
            this.btnColor.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnColor.ImageClicked = global::SDMS.Properties.Resources.MessageSender_Color_Click;
            this.btnColor.ImageDisabled = null;
            this.btnColor.ImageMouseOver = global::SDMS.Properties.Resources.MessageSender_Color_Click;
            this.btnColor.ImageNormal = global::SDMS.Properties.Resources.MessageSender_Color;
            this.btnColor.Location = new System.Drawing.Point(142, 91);
            this.btnColor.Name = "btnColor";
            this.btnColor.Owner = null;
            this.btnColor.Size = new System.Drawing.Size(26, 26);
            this.btnColor.TabIndex = 23;
            this.btnColor.TabStop = false;
            this.btnColor.TextColor = System.Drawing.Color.Black;
            this.btnColor.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnColor.ToolTipText = "";
            this.btnColor.UseToolTip = false;
            this.btnColor.WindowRateWidth = 1F;
            this.btnColor.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // FormMessageSender
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SDMS.Properties.Resources.MessageSenderBackground;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(408, 400);
            this.Controls.Add(this.btnColor);
            this.Controls.Add(this.btnFont);
            this.Controls.Add(this.btnStrikeout);
            this.Controls.Add(this.btnUnderline);
            this.Controls.Add(this.btnItalic);
            this.Controls.Add(this.btnBold);
            this.Controls.Add(this.btnShowReceiveForm);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.textBoxTitle);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxSenderName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rtbBody);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMessageSender";
            this.Text = "알림 메시지 작성";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMessageSender_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnShowReceiveForm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnBold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnItalic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnUnderline)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnStrikeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFont)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnColor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbBody;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxSenderName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxTitle;
        private UnE.GUI.ImageButton btnCancel;
        private UnE.GUI.ImageButton btnOK;
        private UnE.GUI.ImageButton btnShowReceiveForm;
        private UnE.GUI.ImageButton btnBold;
        private UnE.GUI.ImageButton btnItalic;
        private UnE.GUI.ImageButton btnUnderline;
        private UnE.GUI.ImageButton btnStrikeout;
        private UnE.GUI.ImageButton btnFont;
        private UnE.GUI.ImageButton btnColor;
    }
}