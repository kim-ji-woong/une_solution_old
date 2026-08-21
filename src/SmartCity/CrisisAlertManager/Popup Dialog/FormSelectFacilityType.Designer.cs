namespace CrisisAlertManager.Popup_Dialog
{
    partial class FormSelectFacilityType
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.plCollapse = new System.Windows.Forms.Panel();
            this.plHeat = new System.Windows.Forms.Panel();
            this.plFlood = new System.Windows.Forms.Panel();
            this.plFire = new System.Windows.Forms.Panel();
            this.btnCollapse = new UnE.GUI.ImageButton();
            this.btnHeat = new UnE.GUI.ImageButton();
            this.btnFlood = new UnE.GUI.ImageButton();
            this.btnFire = new UnE.GUI.ImageButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnConfirm = new UnE.GUI.ImageButton();
            this.btnCancel = new UnE.GUI.ImageButton();
            ((System.ComponentModel.ISupportInitialize)(this.btnCollapse)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnHeat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFlood)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFire)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnConfirm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImage = global::CrisisAlertManager.Properties.Resources.SelectFacilityTypeTitle;
            this.panel1.Location = new System.Drawing.Point(21, 21);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(184, 23);
            this.panel1.TabIndex = 37;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form_MouseMove);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form_MouseUp);
            // 
            // plCollapse
            // 
            this.plCollapse.BackColor = System.Drawing.Color.Transparent;
            this.plCollapse.BackgroundImage = global::CrisisAlertManager.Properties.Resources.FacilityType_Collapse;
            this.plCollapse.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.plCollapse.Location = new System.Drawing.Point(264, 99);
            this.plCollapse.Name = "plCollapse";
            this.plCollapse.Size = new System.Drawing.Size(105, 25);
            this.plCollapse.TabIndex = 36;
            this.plCollapse.Click += new System.EventHandler(this.btnCollapse_Click);
            // 
            // plHeat
            // 
            this.plHeat.BackColor = System.Drawing.Color.Transparent;
            this.plHeat.BackgroundImage = global::CrisisAlertManager.Properties.Resources.FacilityType_Heat;
            this.plHeat.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.plHeat.Location = new System.Drawing.Point(186, 99);
            this.plHeat.Name = "plHeat";
            this.plHeat.Size = new System.Drawing.Size(45, 25);
            this.plHeat.TabIndex = 35;
            this.plHeat.Click += new System.EventHandler(this.btnHeat_Click);
            // 
            // plFlood
            // 
            this.plFlood.BackColor = System.Drawing.Color.Transparent;
            this.plFlood.BackgroundImage = global::CrisisAlertManager.Properties.Resources.FacilityType_Flood;
            this.plFlood.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.plFlood.Location = new System.Drawing.Point(110, 99);
            this.plFlood.Name = "plFlood";
            this.plFlood.Size = new System.Drawing.Size(45, 25);
            this.plFlood.TabIndex = 34;
            this.plFlood.Click += new System.EventHandler(this.btnFlood_Click);
            // 
            // plFire
            // 
            this.plFire.BackColor = System.Drawing.Color.Transparent;
            this.plFire.BackgroundImage = global::CrisisAlertManager.Properties.Resources.FacilityType_Fire;
            this.plFire.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.plFire.Location = new System.Drawing.Point(31, 99);
            this.plFire.Name = "plFire";
            this.plFire.Size = new System.Drawing.Size(45, 25);
            this.plFire.TabIndex = 33;
            this.plFire.Click += new System.EventHandler(this.btnFire_Click);
            // 
            // btnCollapse
            // 
            this.btnCollapse.ButtonText = "";
            this.btnCollapse.ImageClicked = global::CrisisAlertManager.Properties.Resources.Checkbox_Click;
            this.btnCollapse.ImageDisabled = null;
            this.btnCollapse.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.Checkbox_Normal;
            this.btnCollapse.ImageNormal = global::CrisisAlertManager.Properties.Resources.Checkbox_Normal;
            this.btnCollapse.Location = new System.Drawing.Point(239, 102);
            this.btnCollapse.Name = "btnCollapse";
            this.btnCollapse.Owner = null;
            this.btnCollapse.Size = new System.Drawing.Size(19, 19);
            this.btnCollapse.TabIndex = 32;
            this.btnCollapse.TabStop = false;
            this.btnCollapse.TextColor = System.Drawing.Color.Black;
            this.btnCollapse.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCollapse.ToolTipText = "";
            this.btnCollapse.UseToolTip = false;
            this.btnCollapse.WindowRateWidth = 1F;
            this.btnCollapse.Click += new System.EventHandler(this.btnCollapse_Click);
            // 
            // btnHeat
            // 
            this.btnHeat.ButtonText = "";
            this.btnHeat.ImageClicked = global::CrisisAlertManager.Properties.Resources.Checkbox_Click;
            this.btnHeat.ImageDisabled = null;
            this.btnHeat.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.Checkbox_Normal;
            this.btnHeat.ImageNormal = global::CrisisAlertManager.Properties.Resources.Checkbox_Normal;
            this.btnHeat.Location = new System.Drawing.Point(161, 102);
            this.btnHeat.Name = "btnHeat";
            this.btnHeat.Owner = null;
            this.btnHeat.Size = new System.Drawing.Size(19, 19);
            this.btnHeat.TabIndex = 31;
            this.btnHeat.TabStop = false;
            this.btnHeat.TextColor = System.Drawing.Color.Black;
            this.btnHeat.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnHeat.ToolTipText = "";
            this.btnHeat.UseToolTip = false;
            this.btnHeat.WindowRateWidth = 1F;
            this.btnHeat.Click += new System.EventHandler(this.btnHeat_Click);
            // 
            // btnFlood
            // 
            this.btnFlood.ButtonText = "";
            this.btnFlood.ImageClicked = global::CrisisAlertManager.Properties.Resources.Checkbox_Click;
            this.btnFlood.ImageDisabled = null;
            this.btnFlood.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.Checkbox_Normal;
            this.btnFlood.ImageNormal = global::CrisisAlertManager.Properties.Resources.Checkbox_Normal;
            this.btnFlood.Location = new System.Drawing.Point(84, 102);
            this.btnFlood.Name = "btnFlood";
            this.btnFlood.Owner = null;
            this.btnFlood.Size = new System.Drawing.Size(19, 19);
            this.btnFlood.TabIndex = 30;
            this.btnFlood.TabStop = false;
            this.btnFlood.TextColor = System.Drawing.Color.Black;
            this.btnFlood.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnFlood.ToolTipText = "";
            this.btnFlood.UseToolTip = false;
            this.btnFlood.WindowRateWidth = 1F;
            this.btnFlood.Click += new System.EventHandler(this.btnFlood_Click);
            // 
            // btnFire
            // 
            this.btnFire.ButtonText = "";
            this.btnFire.ImageClicked = global::CrisisAlertManager.Properties.Resources.Checkbox_Click;
            this.btnFire.ImageDisabled = null;
            this.btnFire.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.Checkbox_Normal;
            this.btnFire.ImageNormal = global::CrisisAlertManager.Properties.Resources.Checkbox_Normal;
            this.btnFire.Location = new System.Drawing.Point(6, 102);
            this.btnFire.Name = "btnFire";
            this.btnFire.Owner = null;
            this.btnFire.Size = new System.Drawing.Size(19, 19);
            this.btnFire.TabIndex = 29;
            this.btnFire.TabStop = false;
            this.btnFire.TextColor = System.Drawing.Color.Black;
            this.btnFire.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnFire.ToolTipText = "";
            this.btnFire.UseToolTip = false;
            this.btnFire.WindowRateWidth = 1F;
            this.btnFire.Click += new System.EventHandler(this.btnFire_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(93)))), ((int)(((byte)(128)))));
            this.pictureBox1.Location = new System.Drawing.Point(0, 68);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(370, 2);
            this.pictureBox1.TabIndex = 28;
            this.pictureBox1.TabStop = false;
            // 
            // btnConfirm
            // 
            this.btnConfirm.ButtonText = "";
            this.btnConfirm.ImageClicked = global::CrisisAlertManager.Properties.Resources.PopupConfirm_Click;
            this.btnConfirm.ImageDisabled = null;
            this.btnConfirm.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.PopupConfirm_Hover;
            this.btnConfirm.ImageNormal = global::CrisisAlertManager.Properties.Resources.PopupConfirm_Normal;
            this.btnConfirm.Location = new System.Drawing.Point(78, 172);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Owner = null;
            this.btnConfirm.Size = new System.Drawing.Size(100, 45);
            this.btnConfirm.TabIndex = 18;
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
            this.btnCancel.Location = new System.Drawing.Point(193, 172);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(100, 45);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.TabStop = false;
            this.btnCancel.TextColor = System.Drawing.Color.Black;
            this.btnCancel.TextFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.ToolTipText = "";
            this.btnCancel.UseToolTip = false;
            this.btnCancel.WindowRateWidth = 1F;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FormSelectFacilityType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(40)))), ((int)(((byte)(76)))));
            this.ClientSize = new System.Drawing.Size(370, 250);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.plCollapse);
            this.Controls.Add(this.plHeat);
            this.Controls.Add(this.plFlood);
            this.Controls.Add(this.plFire);
            this.Controls.Add(this.btnCollapse);
            this.Controls.Add(this.btnHeat);
            this.Controls.Add(this.btnFlood);
            this.Controls.Add(this.btnFire);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormSelectFacilityType";
            this.Text = "FormSelectFacilityType";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.btnCollapse)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnHeat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFlood)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFire)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnConfirm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private UnE.GUI.ImageButton btnConfirm;
        private UnE.GUI.ImageButton btnCancel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private UnE.GUI.ImageButton btnFire;
        private UnE.GUI.ImageButton btnFlood;
        private UnE.GUI.ImageButton btnHeat;
        private UnE.GUI.ImageButton btnCollapse;
        private System.Windows.Forms.Panel plFire;
        private System.Windows.Forms.Panel plFlood;
        private System.Windows.Forms.Panel plHeat;
        private System.Windows.Forms.Panel plCollapse;
        private System.Windows.Forms.Panel panel1;
    }
}