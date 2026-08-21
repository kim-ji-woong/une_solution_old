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
            this.btnConfirm = new UnE.GUI.ImageButton();
            this.btnCancel = new UnE.GUI.ImageButton();
            this.lblMsg = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.btnConfirm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            this.SuspendLayout();
            // 
            // btnConfirm
            // 
            this.btnConfirm.ButtonText = "";
            this.btnConfirm.ImageClicked = global::SDMS_Building.Properties.Resources.ok_click;
            this.btnConfirm.ImageDisabled = null;
            this.btnConfirm.ImageMouseOver = global::SDMS_Building.Properties.Resources.ok_hover;
            this.btnConfirm.ImageNormal = global::SDMS_Building.Properties.Resources.ok_normal;
            this.btnConfirm.Location = new System.Drawing.Point(26, 177);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Owner = null;
            this.btnConfirm.Size = new System.Drawing.Size(150, 45);
            this.btnConfirm.TabIndex = 14;
            this.btnConfirm.TabStop = false;
            this.btnConfirm.TextColor = System.Drawing.Color.Black;
            this.btnConfirm.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnConfirm.ToolTipText = "";
            this.btnConfirm.UseToolTip = false;
            this.btnConfirm.WindowRateWidth = 1F;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.ButtonText = "";
            this.btnCancel.ImageClicked = global::SDMS_Building.Properties.Resources.cancel_Click;
            this.btnCancel.ImageDisabled = null;
            this.btnCancel.ImageMouseOver = global::SDMS_Building.Properties.Resources.cancel_Hover;
            this.btnCancel.ImageNormal = global::SDMS_Building.Properties.Resources.cancel_Normal;
            this.btnCancel.Location = new System.Drawing.Point(196, 177);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(150, 45);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.TabStop = false;
            this.btnCancel.TextColor = System.Drawing.Color.Black;
            this.btnCancel.TextFont = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ToolTipText = "";
            this.btnCancel.UseToolTip = false;
            this.btnCancel.WindowRateWidth = 1F;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
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
            // FormMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(370, 250);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblMsg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMessageBox";
            this.ShowInTaskbar = false;
            this.Text = "FormMessageBox";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(254)))), ((int)(((byte)(254)))));
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormMessageBox_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormMessageBox_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormMessageBox_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.btnConfirm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private UnE.GUI.ImageButton btnConfirm;
        private UnE.GUI.ImageButton btnCancel;
        private System.Windows.Forms.Label lblMsg;
    }
}