namespace SDMS_Building.PopupDialog
{
    partial class FormMessageBox
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
            this.lblMsg = new System.Windows.Forms.Label();
            this.btnYes = new UnE.GUI.ImageButton();
            this.btnCancel = new UnE.GUI.ImageButton();
            this.btnClose = new UnE.GUI.ImageButton();
            ((System.ComponentModel.ISupportInitialize)(this.btnYes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMsg
            // 
            this.lblMsg.BackColor = System.Drawing.Color.Transparent;
            this.lblMsg.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblMsg.Location = new System.Drawing.Point(24, 28);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(322, 137);
            this.lblMsg.TabIndex = 12;
            this.lblMsg.Text = "message";
            this.lblMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMsg.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormMessageBox_MouseDown);
            this.lblMsg.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormMessageBox_MouseMove);
            this.lblMsg.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormMessageBox_MouseUp);
            // 
            // btnYes
            // 
            this.btnYes.ButtonText = "확인";
            this.btnYes.ImageClicked = global::SDMS_Building.Properties.Resources.btn_click;
            this.btnYes.ImageDisabled = null;
            this.btnYes.ImageMouseOver = global::SDMS_Building.Properties.Resources.btn_hover;
            this.btnYes.ImageNormal = global::SDMS_Building.Properties.Resources.btn_normal;
            this.btnYes.Location = new System.Drawing.Point(26, 177);
            this.btnYes.Name = "btnYes";
            this.btnYes.Owner = null;
            this.btnYes.Size = new System.Drawing.Size(150, 45);
            this.btnYes.TabIndex = 14;
            this.btnYes.TabStop = false;
            this.btnYes.Text = "확인";
            this.btnYes.TextColor = System.Drawing.Color.Black;
            this.btnYes.TextFont = new System.Drawing.Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnYes.ToolTipText = "";
            this.btnYes.UseToolTip = false;
            this.btnYes.WindowRateWidth = 1F;
            this.btnYes.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.ButtonText = "취소";
            this.btnCancel.ImageClicked = global::SDMS_Building.Properties.Resources.btn_click2;
            this.btnCancel.ImageDisabled = null;
            this.btnCancel.ImageMouseOver = global::SDMS_Building.Properties.Resources.btn_hover2;
            this.btnCancel.ImageNormal = global::SDMS_Building.Properties.Resources.btn_normal2;
            this.btnCancel.Location = new System.Drawing.Point(196, 177);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(150, 45);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.TabStop = false;
            this.btnCancel.Text = "취소";
            this.btnCancel.TextColor = System.Drawing.Color.Black;
            this.btnCancel.TextFont = new System.Drawing.Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ToolTipText = "";
            this.btnCancel.UseToolTip = false;
            this.btnCancel.WindowRateWidth = 1F;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnClose
            // 
            this.btnClose.ButtonText = "";
            this.btnClose.ImageClicked = global::SDMS_Building.Properties.Resources.close2_click;
            this.btnClose.ImageDisabled = null;
            this.btnClose.ImageMouseOver = global::SDMS_Building.Properties.Resources.close2_hover;
            this.btnClose.ImageNormal = global::SDMS_Building.Properties.Resources.close2_normal;
            this.btnClose.Location = new System.Drawing.Point(328, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(30, 30);
            this.btnClose.TabIndex = 17;
            this.btnClose.TabStop = false;
            this.btnClose.TextColor = System.Drawing.Color.Black;
            this.btnClose.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ToolTipText = "";
            this.btnClose.UseToolTip = false;
            this.btnClose.Visible = false;
            this.btnClose.WindowRateWidth = 1F;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FormMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(370, 250);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnYes);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblMsg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMessageBox";
            this.ShowInTaskbar = false;
            this.Text = "FormMessageBox";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(254)))), ((int)(((byte)(254)))));
            this.Load += new System.EventHandler(this.FormMessageBox_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormMessageBox_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormMessageBox_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormMessageBox_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.btnYes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private UnE.GUI.ImageButton btnYes;
        private UnE.GUI.ImageButton btnCancel;
        private System.Windows.Forms.Label lblMsg;
        private UnE.GUI.ImageButton btnClose;
    }
}