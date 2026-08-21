namespace CrisisAlertManager.Popup_Dialog.Alarm
{
    partial class FormAlertAlarm
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
            this.plState = new System.Windows.Forms.Panel();
            this.pbState = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.plCollapseText = new System.Windows.Forms.Panel();
            this.plFloodText = new System.Windows.Forms.Panel();
            this.plHeatText = new System.Windows.Forms.Panel();
            this.plFireText = new System.Windows.Forms.Panel();
            this.plAddress = new System.Windows.Forms.Panel();
            this.lbAddress = new System.Windows.Forms.Label();
            this.btnClose = new UnE.GUI.ImageButton();
            this.plTop = new System.Windows.Forms.Panel();
            this.plAlarm = new System.Windows.Forms.Panel();
            this.lbAlarm = new System.Windows.Forms.Label();
            this.plState.SuspendLayout();
            this.panel2.SuspendLayout();
            this.plFloodText.SuspendLayout();
            this.plAddress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.plTop.SuspendLayout();
            this.plAlarm.SuspendLayout();
            this.SuspendLayout();
            // 
            // plState
            // 
            this.plState.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(74)))), ((int)(((byte)(127)))));
            this.plState.Controls.Add(this.pbState);
            this.plState.Location = new System.Drawing.Point(0, 45);
            this.plState.Name = "plState";
            this.plState.Size = new System.Drawing.Size(270, 205);
            this.plState.TabIndex = 1;
            this.plState.Click += new System.EventHandler(this.pbState_Click);
            // 
            // pbState
            // 
            this.pbState.BackgroundImage = global::CrisisAlertManager.Properties.Resources.Normal_new;
            this.pbState.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbState.Location = new System.Drawing.Point(45, 12);
            this.pbState.Name = "pbState";
            this.pbState.Size = new System.Drawing.Size(180, 180);
            this.pbState.TabIndex = 0;
            this.pbState.Click += new System.EventHandler(this.pbState_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(194)))), ((int)(((byte)(208)))));
            this.panel2.Controls.Add(this.plCollapseText);
            this.panel2.Controls.Add(this.plFloodText);
            this.panel2.Controls.Add(this.plFireText);
            this.panel2.Controls.Add(this.plAddress);
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Location = new System.Drawing.Point(0, 250);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(270, 70);
            this.panel2.TabIndex = 2;
            // 
            // plCollapseText
            // 
            this.plCollapseText.BackColor = System.Drawing.Color.Transparent;
            this.plCollapseText.BackgroundImage = global::CrisisAlertManager.Properties.Resources.CollapseAlarmText;
            this.plCollapseText.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.plCollapseText.Location = new System.Drawing.Point(67, 10);
            this.plCollapseText.Name = "plCollapseText";
            this.plCollapseText.Size = new System.Drawing.Size(142, 12);
            this.plCollapseText.TabIndex = 62;
            // 
            // plFloodText
            // 
            this.plFloodText.BackColor = System.Drawing.Color.Transparent;
            this.plFloodText.BackgroundImage = global::CrisisAlertManager.Properties.Resources.FloodAlarmText;
            this.plFloodText.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.plFloodText.Controls.Add(this.plHeatText);
            this.plFloodText.Location = new System.Drawing.Point(79, 10);
            this.plFloodText.Name = "plFloodText";
            this.plFloodText.Size = new System.Drawing.Size(106, 12);
            this.plFloodText.TabIndex = 60;
            this.plFloodText.Visible = false;
            // 
            // plHeatText
            // 
            this.plHeatText.BackColor = System.Drawing.Color.Transparent;
            this.plHeatText.BackgroundImage = global::CrisisAlertManager.Properties.Resources.HeatAlarmText;
            this.plHeatText.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.plHeatText.Location = new System.Drawing.Point(0, 0);
            this.plHeatText.Name = "plHeatText";
            this.plHeatText.Size = new System.Drawing.Size(106, 12);
            this.plHeatText.TabIndex = 61;
            this.plHeatText.Visible = false;
            // 
            // plFireText
            // 
            this.plFireText.BackColor = System.Drawing.Color.Transparent;
            this.plFireText.BackgroundImage = global::CrisisAlertManager.Properties.Resources.FireAlarmText;
            this.plFireText.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.plFireText.Location = new System.Drawing.Point(79, 10);
            this.plFireText.Name = "plFireText";
            this.plFireText.Size = new System.Drawing.Size(106, 12);
            this.plFireText.TabIndex = 59;
            this.plFireText.Visible = false;
            // 
            // plAddress
            // 
            this.plAddress.BackColor = System.Drawing.Color.Transparent;
            this.plAddress.BackgroundImage = global::CrisisAlertManager.Properties.Resources.AlarmAddressPanel;
            this.plAddress.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.plAddress.Controls.Add(this.lbAddress);
            this.plAddress.Location = new System.Drawing.Point(25, 30);
            this.plAddress.Name = "plAddress";
            this.plAddress.Size = new System.Drawing.Size(220, 30);
            this.plAddress.TabIndex = 58;
            // 
            // lbAddress
            // 
            this.lbAddress.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbAddress.Location = new System.Drawing.Point(0, 3);
            this.lbAddress.Name = "lbAddress";
            this.lbAddress.Size = new System.Drawing.Size(220, 23);
            this.lbAddress.TabIndex = 0;
            this.lbAddress.Text = "감지 센서 주소";
            this.lbAddress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnClose
            // 
            this.btnClose.ButtonText = "";
            this.btnClose.ImageClicked = global::CrisisAlertManager.Properties.Resources.btnClose_Selected;
            this.btnClose.ImageDisabled = null;
            this.btnClose.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.btnClose_MouseOver;
            this.btnClose.ImageNormal = global::CrisisAlertManager.Properties.Resources.btnClose_Normal;
            this.btnClose.Location = new System.Drawing.Point(244, 5);
            this.btnClose.Name = "btnClose";
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(20, 20);
            this.btnClose.TabIndex = 57;
            this.btnClose.TabStop = false;
            this.btnClose.TextColor = System.Drawing.Color.Black;
            this.btnClose.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ToolTipText = "";
            this.btnClose.UseToolTip = false;
            this.btnClose.WindowRateWidth = 1F;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // plTop
            // 
            this.plTop.BackgroundImage = global::CrisisAlertManager.Properties.Resources.FireAlarmTop;
            this.plTop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.plTop.Controls.Add(this.plAlarm);
            this.plTop.Location = new System.Drawing.Point(0, 0);
            this.plTop.Name = "plTop";
            this.plTop.Size = new System.Drawing.Size(270, 45);
            this.plTop.TabIndex = 0;
            // 
            // plAlarm
            // 
            this.plAlarm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.plAlarm.BackColor = System.Drawing.Color.Transparent;
            this.plAlarm.BackgroundImage = global::CrisisAlertManager.Properties.Resources.AlarmNumber;
            this.plAlarm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.plAlarm.Controls.Add(this.lbAlarm);
            this.plAlarm.Location = new System.Drawing.Point(223, 0);
            this.plAlarm.Name = "plAlarm";
            this.plAlarm.Size = new System.Drawing.Size(40, 40);
            this.plAlarm.TabIndex = 0;
            this.plAlarm.Click += new System.EventHandler(this.plAlarm_Click);
            // 
            // lbAlarm
            // 
            this.lbAlarm.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbAlarm.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(173)))), ((int)(((byte)(5)))), ((int)(((byte)(23)))));
            this.lbAlarm.Location = new System.Drawing.Point(16, 7);
            this.lbAlarm.Name = "lbAlarm";
            this.lbAlarm.Size = new System.Drawing.Size(23, 12);
            this.lbAlarm.TabIndex = 0;
            this.lbAlarm.Text = "0";
            this.lbAlarm.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // FormAlertAlarm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(270, 320);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.plState);
            this.Controls.Add(this.plTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormAlertAlarm";
            this.Text = "FormAlertAlarm";
            this.plState.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.plFloodText.ResumeLayout(false);
            this.plAddress.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.plTop.ResumeLayout(false);
            this.plAlarm.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel plTop;
        private System.Windows.Forms.Panel plState;
        private System.Windows.Forms.Panel pbState;
        private System.Windows.Forms.Panel panel2;
        private UnE.GUI.ImageButton btnClose;
        private System.Windows.Forms.Panel plAddress;
        private System.Windows.Forms.Label lbAddress;
        private System.Windows.Forms.Panel plFloodText;
        private System.Windows.Forms.Panel plFireText;
        private System.Windows.Forms.Panel plHeatText;
        private System.Windows.Forms.Panel plCollapseText;
        private System.Windows.Forms.Panel plAlarm;
        private System.Windows.Forms.Label lbAlarm;
    }
}