
namespace BroadcastServer
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.cboCommandType = new System.Windows.Forms.ComboBox();
            this.cboMaterialType = new System.Windows.Forms.ComboBox();
            this.cboAlarmLevel = new System.Windows.Forms.ComboBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.labelConnection = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cboCommandType
            // 
            this.cboCommandType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCommandType.FormattingEnabled = true;
            this.cboCommandType.Items.AddRange(new object[] {
            "누출",
            "화재"});
            this.cboCommandType.Location = new System.Drawing.Point(92, 26);
            this.cboCommandType.Name = "cboCommandType";
            this.cboCommandType.Size = new System.Drawing.Size(121, 20);
            this.cboCommandType.TabIndex = 0;
            this.cboCommandType.SelectedIndexChanged += new System.EventHandler(this.cboCommandType_SelectedIndexChanged);
            // 
            // cboMaterialType
            // 
            this.cboMaterialType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMaterialType.FormattingEnabled = true;
            this.cboMaterialType.Items.AddRange(new object[] {
            "불산",
            "염산",
            "Co",
            "Co2",
            "Tvoc",
            "O2"});
            this.cboMaterialType.Location = new System.Drawing.Point(92, 52);
            this.cboMaterialType.Name = "cboMaterialType";
            this.cboMaterialType.Size = new System.Drawing.Size(121, 20);
            this.cboMaterialType.TabIndex = 0;
            // 
            // cboAlarmLevel
            // 
            this.cboAlarmLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAlarmLevel.FormattingEnabled = true;
            this.cboAlarmLevel.Items.AddRange(new object[] {
            "알람해제",
            "주의 알람",
            "경계 알람",
            "심각 알람"});
            this.cboAlarmLevel.Location = new System.Drawing.Point(92, 78);
            this.cboAlarmLevel.Name = "cboAlarmLevel";
            this.cboAlarmLevel.Size = new System.Drawing.Size(121, 20);
            this.cboAlarmLevel.TabIndex = 0;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(138, 156);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "전송";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // labelConnection
            // 
            this.labelConnection.AutoSize = true;
            this.labelConnection.Location = new System.Drawing.Point(43, 117);
            this.labelConnection.Name = "labelConnection";
            this.labelConnection.Size = new System.Drawing.Size(133, 12);
            this.labelConnection.TabIndex = 2;
            this.labelConnection.Text = "접속된 클라이언트 없음";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(265, 209);
            this.Controls.Add(this.labelConnection);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.cboAlarmLevel);
            this.Controls.Add(this.cboMaterialType);
            this.Controls.Add(this.cboCommandType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormMain";
            this.Text = "FormMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboCommandType;
        private System.Windows.Forms.ComboBox cboMaterialType;
        private System.Windows.Forms.ComboBox cboAlarmLevel;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Label labelConnection;
    }
}

