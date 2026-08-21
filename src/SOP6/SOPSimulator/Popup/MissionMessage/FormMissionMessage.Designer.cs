namespace SOPMonitoringSystem.Popup.MissionMessage
{
    partial class FormMissionMessage
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
            this.labelMessageType = new System.Windows.Forms.Label();
            this.cboDisaster = new System.Windows.Forms.ComboBox();
            this.checkBoxSiren = new System.Windows.Forms.CheckBox();
            this.labelBroadcastCount = new System.Windows.Forms.Label();
            this.btnExecute = new System.Windows.Forms.Button();
            this.checkBoxComplete = new System.Windows.Forms.CheckBox();
            this.textBoxMessage = new System.Windows.Forms.RichTextBox();
            this.btnShowSpecialMessageOption = new System.Windows.Forms.Button();
            this.labelSender = new System.Windows.Forms.Label();
            this.textBoxSender = new System.Windows.Forms.TextBox();
            this.labelReceiver = new System.Windows.Forms.Label();
            this.textBoxReceiver = new System.Windows.Forms.TextBox();
            this.cboBroadcastCount = new SOPMonitoringSystem.DisabledComboBox();
            this.SuspendLayout();
            // 
            // labelMessageType
            // 
            this.labelMessageType.AutoSize = true;
            this.labelMessageType.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelMessageType.Location = new System.Drawing.Point(4, 64);
            this.labelMessageType.Name = "labelMessageType";
            this.labelMessageType.Size = new System.Drawing.Size(106, 21);
            this.labelMessageType.TabIndex = 48;
            this.labelMessageType.Text = "메시지 타입 :";
            this.labelMessageType.Visible = false;
            // 
            // cboDisaster
            // 
            this.cboDisaster.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDisaster.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboDisaster.FormattingEnabled = true;
            this.cboDisaster.Items.AddRange(new object[] {
            "화재, 유출사고(비상상황)",
            "기타(시스템제공)",
            "사용자 입력",
            "시나리오"});
            this.cboDisaster.Location = new System.Drawing.Point(114, 62);
            this.cboDisaster.Name = "cboDisaster";
            this.cboDisaster.Size = new System.Drawing.Size(361, 29);
            this.cboDisaster.TabIndex = 47;
            this.cboDisaster.Visible = false;
            this.cboDisaster.SelectedIndexChanged += new System.EventHandler(this.comboDisaster_SelectedIndexChanged);
            // 
            // checkBoxSiren
            // 
            this.checkBoxSiren.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxSiren.AutoSize = true;
            this.checkBoxSiren.Checked = true;
            this.checkBoxSiren.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSiren.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxSiren.Location = new System.Drawing.Point(657, 6);
            this.checkBoxSiren.Name = "checkBoxSiren";
            this.checkBoxSiren.Size = new System.Drawing.Size(115, 25);
            this.checkBoxSiren.TabIndex = 55;
            this.checkBoxSiren.Text = "사이렌 사용";
            this.checkBoxSiren.UseVisualStyleBackColor = true;
            this.checkBoxSiren.CheckedChanged += new System.EventHandler(this.checkBoxSiren_CheckedChanged);
            // 
            // labelBroadcastCount
            // 
            this.labelBroadcastCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelBroadcastCount.AutoSize = true;
            this.labelBroadcastCount.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelBroadcastCount.Location = new System.Drawing.Point(483, 7);
            this.labelBroadcastCount.Name = "labelBroadcastCount";
            this.labelBroadcastCount.Size = new System.Drawing.Size(84, 21);
            this.labelBroadcastCount.TabIndex = 48;
            this.labelBroadcastCount.Text = "방송횟수 :";
            // 
            // btnExecute
            // 
            this.btnExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecute.Enabled = false;
            this.btnExecute.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnExecute.Location = new System.Drawing.Point(903, 4);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 29);
            this.btnExecute.TabIndex = 57;
            this.btnExecute.Text = "실행";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // checkBoxComplete
            // 
            this.checkBoxComplete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxComplete.AutoSize = true;
            this.checkBoxComplete.Enabled = false;
            this.checkBoxComplete.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxComplete.Location = new System.Drawing.Point(984, 7);
            this.checkBoxComplete.Name = "checkBoxComplete";
            this.checkBoxComplete.Size = new System.Drawing.Size(61, 25);
            this.checkBoxComplete.TabIndex = 58;
            this.checkBoxComplete.Text = "완료";
            this.checkBoxComplete.UseVisualStyleBackColor = true;
            this.checkBoxComplete.CheckedChanged += new System.EventHandler(this.checkBoxComplete_CheckedChanged);
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxMessage.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxMessage.Location = new System.Drawing.Point(0, 39);
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.textBoxMessage.Size = new System.Drawing.Size(1057, 183);
            this.textBoxMessage.TabIndex = 59;
            this.textBoxMessage.Text = "";
            this.textBoxMessage.Click += new System.EventHandler(this.textBoxMessage_Click);
            this.textBoxMessage.TextChanged += new System.EventHandler(this.textBoxMessage_TextChanged);
            // 
            // btnShowSpecialMessageOption
            // 
            this.btnShowSpecialMessageOption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShowSpecialMessageOption.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnShowSpecialMessageOption.Location = new System.Drawing.Point(779, 4);
            this.btnShowSpecialMessageOption.Name = "btnShowSpecialMessageOption";
            this.btnShowSpecialMessageOption.Size = new System.Drawing.Size(120, 29);
            this.btnShowSpecialMessageOption.TabIndex = 57;
            this.btnShowSpecialMessageOption.Text = "특수문자 옵션";
            this.btnShowSpecialMessageOption.UseVisualStyleBackColor = true;
            this.btnShowSpecialMessageOption.Click += new System.EventHandler(this.btnShowSpecialMessageOption_Click);
            // 
            // labelSender
            // 
            this.labelSender.AutoSize = true;
            this.labelSender.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelSender.Location = new System.Drawing.Point(4, 7);
            this.labelSender.Name = "labelSender";
            this.labelSender.Size = new System.Drawing.Size(68, 21);
            this.labelSender.TabIndex = 48;
            this.labelSender.Text = "발신자 :";
            // 
            // textBoxSender
            // 
            this.textBoxSender.BackColor = System.Drawing.Color.White;
            this.textBoxSender.Location = new System.Drawing.Point(75, 7);
            this.textBoxSender.Name = "textBoxSender";
            this.textBoxSender.ReadOnly = true;
            this.textBoxSender.Size = new System.Drawing.Size(160, 21);
            this.textBoxSender.TabIndex = 60;
            // 
            // labelReceiver
            // 
            this.labelReceiver.AutoSize = true;
            this.labelReceiver.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelReceiver.Location = new System.Drawing.Point(240, 7);
            this.labelReceiver.Name = "labelReceiver";
            this.labelReceiver.Size = new System.Drawing.Size(68, 21);
            this.labelReceiver.TabIndex = 48;
            this.labelReceiver.Text = "수신자 :";
            // 
            // textBoxReceiver
            // 
            this.textBoxReceiver.BackColor = System.Drawing.Color.White;
            this.textBoxReceiver.Location = new System.Drawing.Point(311, 7);
            this.textBoxReceiver.Name = "textBoxReceiver";
            this.textBoxReceiver.ReadOnly = true;
            this.textBoxReceiver.Size = new System.Drawing.Size(160, 21);
            this.textBoxReceiver.TabIndex = 60;
            // 
            // cboBroadcastCount
            // 
            this.cboBroadcastCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboBroadcastCount.CanVisible = true;
            this.cboBroadcastCount.Disabled = false;
            this.cboBroadcastCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBroadcastCount.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboBroadcastCount.FormattingEnabled = true;
            this.cboBroadcastCount.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.cboBroadcastCount.Location = new System.Drawing.Point(573, 5);
            this.cboBroadcastCount.Name = "cboBroadcastCount";
            this.cboBroadcastCount.Size = new System.Drawing.Size(65, 29);
            this.cboBroadcastCount.TabIndex = 47;
            this.cboBroadcastCount.SelectedIndexChanged += new System.EventHandler(this.cboBroadcast_SelectedIndexChanged);
            // 
            // FormMissionMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1057, 222);
            this.Controls.Add(this.textBoxReceiver);
            this.Controls.Add(this.textBoxSender);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.checkBoxComplete);
            this.Controls.Add(this.btnShowSpecialMessageOption);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.checkBoxSiren);
            this.Controls.Add(this.labelReceiver);
            this.Controls.Add(this.labelSender);
            this.Controls.Add(this.labelBroadcastCount);
            this.Controls.Add(this.labelMessageType);
            this.Controls.Add(this.cboBroadcastCount);
            this.Controls.Add(this.cboDisaster);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMissionMessage";
            this.ShowInTaskbar = false;
            this.Text = "FormMessageFire";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelMessageType;
        private System.Windows.Forms.ComboBox cboDisaster;
        private System.Windows.Forms.CheckBox checkBoxSiren;
        private DisabledComboBox cboBroadcastCount;
        private System.Windows.Forms.Label labelBroadcastCount;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.CheckBox checkBoxComplete;
        private System.Windows.Forms.RichTextBox textBoxMessage;
        private System.Windows.Forms.Button btnShowSpecialMessageOption;
        private System.Windows.Forms.Label labelSender;
        private System.Windows.Forms.TextBox textBoxSender;
        private System.Windows.Forms.Label labelReceiver;
        private System.Windows.Forms.TextBox textBoxReceiver;
    }
}