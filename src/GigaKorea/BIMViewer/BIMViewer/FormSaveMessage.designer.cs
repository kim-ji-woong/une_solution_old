namespace BIMViewer.uControl
{
    partial class uSaveMessage
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
            this.lblMessage = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnClose = new UnE.GUI.ImageButton();
            this.btnOK = new UnE.GUI.RibbonButton();
            this.btnCancel = new UnE.GUI.RibbonButton();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.BackColor = System.Drawing.Color.Transparent;
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblMessage.ForeColor = System.Drawing.Color.White;
            this.lblMessage.Location = new System.Drawing.Point(63, 33);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(374, 61);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "label1";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::BIMViewer.Properties.Resources.popup_title;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(505, 28);
            this.panel1.TabIndex = 3;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Panel1_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Panel1_MouseMove);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "저장";
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ButtonText = "";
            this.btnClose.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.Image = global::BIMViewer.Properties.Resources.Windowclose__Base;
            this.btnClose.ImageClicked = global::BIMViewer.Properties.Resources.Windowclose_1st_MSover;
            this.btnClose.ImageDisabled = null;
            this.btnClose.ImageMouseOver = global::BIMViewer.Properties.Resources.Windowclose_1st_MSover;
            this.btnClose.ImageNormal = global::BIMViewer.Properties.Resources.Windowclose__Base;
            this.btnClose.Location = new System.Drawing.Point(480, 2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(23, 23);
            this.btnClose.TabIndex = 0;
            this.btnClose.TabStop = false;
            this.btnClose.TextColor = System.Drawing.Color.Black;
            this.btnClose.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ToolTipText = "";
            this.btnClose.UseToolTip = false;
            this.btnClose.WindowRateWidth = 1F;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.Transparent;
            this.btnOK.BackgroundImage = global::BIMViewer.Properties.Resources.btnBackground;
            this.btnOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnOK.CheckButton = false;
            this.btnOK.CheckedBkgndImage = null;
            this.btnOK.CheckedImage = null;
            this.btnOK.CheckedMouseOver = null;
            this.btnOK.ClickedBackgroundImage = null;
            this.btnOK.ClickedImage = null;
            this.btnOK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOK.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.DisabledBkgndImage = null;
            this.btnOK.DisabledImage = null;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOK.ForeColor = System.Drawing.Color.White;
            this.btnOK.ForeColorChecked = System.Drawing.Color.White;
            this.btnOK.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnOK.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnOK.ForeColorsByTypeUse = false;
            this.btnOK.ID = -1;
            this.btnOK.InitButtonWidth = 60;
            this.btnOK.IsChecked = false;
            this.btnOK.Location = new System.Drawing.Point(190, 109);
            this.btnOK.MouseOverBkgndImage = null;
            this.btnOK.MouseOverImage = null;
            this.btnOK.Name = "btnOK";
            this.btnOK.NormalImage = null;
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(60, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "예";
            this.btnOK.TextLocation = new System.Drawing.Point(1, 3);
            this.btnOK.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOK.ToolTipText = "예";
            this.btnOK.UseCustomImageRect = false;
            this.btnOK.UseTextLocation = true;
            this.btnOK.UseVisualStyleBackColor = false;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.BackgroundImage = global::BIMViewer.Properties.Resources.btnBackground;
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCancel.CheckButton = false;
            this.btnCancel.CheckedBkgndImage = null;
            this.btnCancel.CheckedImage = null;
            this.btnCancel.CheckedMouseOver = null;
            this.btnCancel.ClickedBackgroundImage = null;
            this.btnCancel.ClickedImage = null;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnCancel.DisabledBkgndImage = null;
            this.btnCancel.DisabledImage = null;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.ForeColorChecked = System.Drawing.Color.White;
            this.btnCancel.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnCancel.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnCancel.ForeColorsByTypeUse = false;
            this.btnCancel.ID = -1;
            this.btnCancel.InitButtonWidth = 60;
            this.btnCancel.IsChecked = false;
            this.btnCancel.Location = new System.Drawing.Point(256, 109);
            this.btnCancel.MouseOverBkgndImage = null;
            this.btnCancel.MouseOverImage = null;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.NormalImage = null;
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(60, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "아니오";
            this.btnCancel.TextLocation = new System.Drawing.Point(1, 3);
            this.btnCancel.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCancel.ToolTipText = "아니오";
            this.btnCancel.UseCustomImageRect = false;
            this.btnCancel.UseTextLocation = true;
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // uSaveMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.BackgroundImage = global::BIMViewer.Properties.Resources.background;
            this.ClientSize = new System.Drawing.Size(505, 144);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "uSaveMessage";
            this.ShowIcon = false;
            this.Text = "uSaveAs";
            this.Load += new System.EventHandler(this.uSaveMessageLoad);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.USaveMessage_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.USaveMessage_MouseMove);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Panel panel1;
        private UnE.GUI.ImageButton btnClose;
        private System.Windows.Forms.Label label1;
        private UnE.GUI.RibbonButton btnOK;
        private UnE.GUI.RibbonButton btnCancel;
    }
}