namespace BIMViewer
{
    partial class uProgressForm
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnClose = new UnE.GUI.ImageButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.imgBtnClose = new UnE.GUI.ImageButton();
            this.label2 = new System.Windows.Forms.Label();
            this.lbStartTime = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgBtnClose)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.BackColor = System.Drawing.Color.Transparent;
            this.lblMessage.ForeColor = System.Drawing.Color.White;
            this.lblMessage.Location = new System.Drawing.Point(60, 62);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(38, 12);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "label1";
            // 
            // progressBar1
            // 
            this.progressBar1.ForeColor = System.Drawing.Color.DodgerBlue;
            this.progressBar1.Location = new System.Drawing.Point(62, 77);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(314, 23);
            this.progressBar1.TabIndex = 1;
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
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::BIMViewer.Properties.Resources.green_gradation_01;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.imgBtnClose);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(458, 28);
            this.panel1.TabIndex = 4;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Panel1_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Panel1_MouseMove);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(7, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "작업중";
            // 
            // imgBtnClose
            // 
            this.imgBtnClose.BackColor = System.Drawing.Color.Transparent;
            this.imgBtnClose.ButtonText = "";
            this.imgBtnClose.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.imgBtnClose.Image = global::BIMViewer.Properties.Resources.Windowclose__Base;
            this.imgBtnClose.ImageClicked = global::BIMViewer.Properties.Resources.Windowclose_1st_MSover;
            this.imgBtnClose.ImageDisabled = null;
            this.imgBtnClose.ImageMouseOver = global::BIMViewer.Properties.Resources.Windowclose_1st_MSover;
            this.imgBtnClose.ImageNormal = global::BIMViewer.Properties.Resources.Windowclose__Base;
            this.imgBtnClose.Location = new System.Drawing.Point(433, 2);
            this.imgBtnClose.Name = "imgBtnClose";
            this.imgBtnClose.Owner = null;
            this.imgBtnClose.Size = new System.Drawing.Size(23, 23);
            this.imgBtnClose.TabIndex = 1;
            this.imgBtnClose.TabStop = false;
            this.imgBtnClose.TextColor = System.Drawing.Color.Black;
            this.imgBtnClose.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.imgBtnClose.ToolTipText = "";
            this.imgBtnClose.UseToolTip = false;
            this.imgBtnClose.WindowRateWidth = 1F;
            this.imgBtnClose.Click += new System.EventHandler(this.ImgBtnClose_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(60, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "시작 시간: ";
            // 
            // lbStartTime
            // 
            this.lbStartTime.AutoSize = true;
            this.lbStartTime.BackColor = System.Drawing.Color.Transparent;
            this.lbStartTime.ForeColor = System.Drawing.Color.White;
            this.lbStartTime.Location = new System.Drawing.Point(123, 110);
            this.lbStartTime.Name = "lbStartTime";
            this.lbStartTime.Size = new System.Drawing.Size(11, 12);
            this.lbStartTime.TabIndex = 6;
            this.lbStartTime.Text = "0";
            // 
            // uProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::BIMViewer.Properties.Resources.background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(458, 147);
            this.Controls.Add(this.lbStartTime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.lblMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "uProgressForm";
            this.Text = "작업중";
            this.Load += new System.EventHandler(this.UProgressForm_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UProgressForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.UProgressForm_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgBtnClose)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.ProgressBar progressBar1;
        private UnE.GUI.ImageButton btnClose;
        private System.Windows.Forms.Panel panel1;
        private UnE.GUI.ImageButton imgBtnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbStartTime;
    }
}