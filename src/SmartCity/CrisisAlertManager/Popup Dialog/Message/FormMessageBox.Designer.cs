namespace CrisisAlertManager.Popup_Dialog.Message
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
            this.lbTitle = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnConfirm = new UnE.GUI.ImageButton();
            this.btnCancel = new UnE.GUI.ImageButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnConfirm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMsg
            // 
            this.lblMsg.BackColor = System.Drawing.Color.Transparent;
            this.lblMsg.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMsg.ForeColor = System.Drawing.Color.White;
            this.lblMsg.Location = new System.Drawing.Point(23, 85);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(322, 94);
            this.lblMsg.TabIndex = 17;
            this.lblMsg.Text = "message";
            this.lblMsg.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblMsg.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormMessageBox_MouseDown);
            this.lblMsg.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormMessageBox_MouseMove);
            this.lblMsg.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormMessageBox_MouseUp);
            // 
            // lbTitle
            // 
            this.lbTitle.BackColor = System.Drawing.Color.Transparent;
            this.lbTitle.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTitle.ForeColor = System.Drawing.Color.White;
            this.lbTitle.Location = new System.Drawing.Point(23, 16);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(322, 44);
            this.lbTitle.TabIndex = 18;
            this.lbTitle.Text = "title";
            this.lbTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormMessageBox_MouseDown);
            this.lbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormMessageBox_MouseMove);
            this.lbTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormMessageBox_MouseUp);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(124)))), ((int)(((byte)(164)))));
            this.pictureBox1.Location = new System.Drawing.Point(0, 68);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(370, 2);
            this.pictureBox1.TabIndex = 19;
            this.pictureBox1.TabStop = false;
            // 
            // btnConfirm
            // 
            this.btnConfirm.ButtonText = "";
            this.btnConfirm.ImageClicked = global::CrisisAlertManager.Properties.Resources.PopupConfirm_Click;
            this.btnConfirm.ImageDisabled = null;
            this.btnConfirm.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.PopupConfirm_Hover;
            this.btnConfirm.ImageNormal = global::CrisisAlertManager.Properties.Resources.PopupConfirm_Normal;
            this.btnConfirm.Location = new System.Drawing.Point(80, 179);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Owner = null;
            this.btnConfirm.Size = new System.Drawing.Size(100, 45);
            this.btnConfirm.TabIndex = 16;
            this.btnConfirm.TabStop = false;
            this.btnConfirm.TextColor = System.Drawing.Color.Black;
            this.btnConfirm.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnConfirm.ToolTipText = "";
            this.btnConfirm.UseToolTip = false;
            this.btnConfirm.WindowRateWidth = 1F;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.ButtonText = "";
            this.btnCancel.ImageClicked = global::CrisisAlertManager.Properties.Resources.PopupCancle_Click;
            this.btnCancel.ImageDisabled = null;
            this.btnCancel.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.PopupCancle_Hover;
            this.btnCancel.ImageNormal = global::CrisisAlertManager.Properties.Resources.PopupCancle_Normal;
            this.btnCancel.Location = new System.Drawing.Point(195, 179);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(100, 45);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.TabStop = false;
            this.btnCancel.TextColor = System.Drawing.Color.Black;
            this.btnCancel.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ToolTipText = "";
            this.btnCancel.UseToolTip = false;
            this.btnCancel.WindowRateWidth = 1F;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FormMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(102)))), ((int)(((byte)(147)))));
            this.ClientSize = new System.Drawing.Size(370, 250);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lbTitle);
            this.Controls.Add(this.lblMsg);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMessageBox";
            this.Text = "FormMessageBox";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormMessageBox_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormMessageBox_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormMessageBox_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnConfirm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private UnE.GUI.ImageButton btnConfirm;
        private UnE.GUI.ImageButton btnCancel;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}