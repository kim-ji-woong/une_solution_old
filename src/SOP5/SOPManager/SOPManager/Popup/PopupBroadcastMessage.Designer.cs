namespace SOPManager
{
    partial class PopupBroadcastMessage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopupBroadcastMessage));
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.labelNote = new System.Windows.Forms.Label();
            this.textBox = new System.Windows.Forms.TextBox();
            this.picTextSend = new System.Windows.Forms.PictureBox();
            this.lblCommander = new System.Windows.Forms.Label();
            this.lblTextSend = new System.Windows.Forms.Label();
            this.textBoxCommander = new System.Windows.Forms.TextBox();
            this.picBroadcast = new System.Windows.Forms.PictureBox();
            this.btnSelectCommander = new UnE.GUI.RibbonButton();
            this.lblBroadcast = new System.Windows.Forms.Label();
            this.btnShowSpecialMessage = new UnE.GUI.RibbonButton();
            this.picAutoRun = new System.Windows.Forms.PictureBox();
            this.lblSelectTeam = new System.Windows.Forms.Label();
            this.lblAutoRun = new System.Windows.Forms.Label();
            this.btnPreview = new UnE.GUI.RibbonButton();
            this.txtSelectTeam = new System.Windows.Forms.TextBox();
            this.btnCancel = new UnE.GUI.RibbonButton();
            this.btnSelectTeam = new UnE.GUI.RibbonButton();
            this.btnTTS = new UnE.GUI.RibbonButton();
            this.lblHelpMessage = new System.Windows.Forms.Label();
            this.btnOK = new UnE.GUI.RibbonButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxAutoRun = new System.Windows.Forms.CheckBox();
            this.rbBtnBroadcast = new System.Windows.Forms.RadioButton();
            this.rbBtnMobile = new System.Windows.Forms.RadioButton();
            this.textMessage = new System.Windows.Forms.TextBox();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTextSend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBroadcast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAutoRun)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel2.Size = new System.Drawing.Size(858, 450);
            this.panel2.TabIndex = 17;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panel3.Controls.Add(this.labelNote);
            this.panel3.Controls.Add(this.textBox);
            this.panel3.Controls.Add(this.picTextSend);
            this.panel3.Controls.Add(this.lblCommander);
            this.panel3.Controls.Add(this.lblTextSend);
            this.panel3.Controls.Add(this.textBoxCommander);
            this.panel3.Controls.Add(this.picBroadcast);
            this.panel3.Controls.Add(this.btnSelectCommander);
            this.panel3.Controls.Add(this.lblBroadcast);
            this.panel3.Controls.Add(this.btnShowSpecialMessage);
            this.panel3.Controls.Add(this.picAutoRun);
            this.panel3.Controls.Add(this.lblSelectTeam);
            this.panel3.Controls.Add(this.lblAutoRun);
            this.panel3.Controls.Add(this.btnPreview);
            this.panel3.Controls.Add(this.txtSelectTeam);
            this.panel3.Controls.Add(this.btnCancel);
            this.panel3.Controls.Add(this.btnSelectTeam);
            this.panel3.Controls.Add(this.btnTTS);
            this.panel3.Controls.Add(this.lblHelpMessage);
            this.panel3.Controls.Add(this.btnOK);
            this.panel3.Controls.Add(this.groupBox1);
            this.panel3.Controls.Add(this.textMessage);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panel3.Location = new System.Drawing.Point(3, 4);
            this.panel3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(852, 442);
            this.panel3.TabIndex = 0;
            this.panel3.Paint += new System.Windows.Forms.PaintEventHandler(this.panel3_Paint);
            // 
            // labelNote
            // 
            this.labelNote.AutoSize = true;
            this.labelNote.Font = new System.Drawing.Font("나눔스퀘어", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelNote.ForeColor = System.Drawing.Color.White;
            this.labelNote.Location = new System.Drawing.Point(18, 12);
            this.labelNote.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.labelNote.Name = "labelNote";
            this.labelNote.Size = new System.Drawing.Size(56, 18);
            this.labelNote.TabIndex = 26;
            this.labelNote.Text = "제목 : ";
            // 
            // textBox
            // 
            this.textBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.textBox.Location = new System.Drawing.Point(80, 8);
            this.textBox.Margin = new System.Windows.Forms.Padding(0);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(760, 27);
            this.textBox.TabIndex = 27;
            // 
            // picTextSend
            // 
            this.picTextSend.BackgroundImage = global::SOPManager.Properties.Resources.@__SOPEDIT_Disable2;
            this.picTextSend.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picTextSend.Location = new System.Drawing.Point(15, 40);
            this.picTextSend.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.picTextSend.Name = "picTextSend";
            this.picTextSend.Size = new System.Drawing.Size(22, 22);
            this.picTextSend.TabIndex = 42;
            this.picTextSend.TabStop = false;
            this.picTextSend.Click += new System.EventHandler(this.MobileMessage_Click);
            // 
            // lblCommander
            // 
            this.lblCommander.AutoSize = true;
            this.lblCommander.Font = new System.Drawing.Font("나눔스퀘어", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblCommander.ForeColor = System.Drawing.Color.White;
            this.lblCommander.Location = new System.Drawing.Point(330, 382);
            this.lblCommander.Margin = new System.Windows.Forms.Padding(0, 9, 3, 0);
            this.lblCommander.Name = "lblCommander";
            this.lblCommander.Size = new System.Drawing.Size(67, 18);
            this.lblCommander.TabIndex = 31;
            this.lblCommander.Text = "발신자 :";
            this.lblCommander.Visible = false;
            // 
            // lblTextSend
            // 
            this.lblTextSend.AutoSize = true;
            this.lblTextSend.Font = new System.Drawing.Font("나눔스퀘어", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTextSend.ForeColor = System.Drawing.Color.White;
            this.lblTextSend.Location = new System.Drawing.Point(41, 43);
            this.lblTextSend.Margin = new System.Windows.Forms.Padding(3, 6, 6, 0);
            this.lblTextSend.Name = "lblTextSend";
            this.lblTextSend.Size = new System.Drawing.Size(72, 18);
            this.lblTextSend.TabIndex = 44;
            this.lblTextSend.Text = "문자발송";
            this.lblTextSend.Click += new System.EventHandler(this.MobileMessage_Click);
            // 
            // textBoxCommander
            // 
            this.textBoxCommander.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.textBoxCommander.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxCommander.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxCommander.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.textBoxCommander.Location = new System.Drawing.Point(410, 377);
            this.textBoxCommander.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.textBoxCommander.Name = "textBoxCommander";
            this.textBoxCommander.ReadOnly = true;
            this.textBoxCommander.Size = new System.Drawing.Size(221, 27);
            this.textBoxCommander.TabIndex = 32;
            this.textBoxCommander.Visible = false;
            // 
            // picBroadcast
            // 
            this.picBroadcast.BackgroundImage = global::SOPManager.Properties.Resources.@__SOPEDIT_Disable2;
            this.picBroadcast.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picBroadcast.Location = new System.Drawing.Point(120, 40);
            this.picBroadcast.Name = "picBroadcast";
            this.picBroadcast.Size = new System.Drawing.Size(22, 22);
            this.picBroadcast.TabIndex = 45;
            this.picBroadcast.TabStop = false;
            this.picBroadcast.Click += new System.EventHandler(this.BroadcastMessage_Click);
            // 
            // btnSelectCommander
            // 
            this.btnSelectCommander.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSelectCommander.CheckButton = false;
            this.btnSelectCommander.CheckedBkgndImage = null;
            this.btnSelectCommander.CheckedImage = null;
            this.btnSelectCommander.CheckedMouseOver = null;
            this.btnSelectCommander.ClickedBackgroundImage = null;
            this.btnSelectCommander.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_SelectClick;
            this.btnSelectCommander.CustomImageRect = new System.Drawing.Rectangle(0, 0, 64, 35);
            this.btnSelectCommander.DisabledBkgndImage = null;
            this.btnSelectCommander.DisabledImage = null;
            this.btnSelectCommander.ForeColorChecked = System.Drawing.Color.White;
            this.btnSelectCommander.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnSelectCommander.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnSelectCommander.ForeColorsByTypeUse = false;
            this.btnSelectCommander.ID = -1;
            this.btnSelectCommander.InitButtonWidth = 64;
            this.btnSelectCommander.IsChecked = false;
            this.btnSelectCommander.Location = new System.Drawing.Point(634, 373);
            this.btnSelectCommander.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.btnSelectCommander.MouseOverBkgndImage = null;
            this.btnSelectCommander.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_SelectClick;
            this.btnSelectCommander.Name = "btnSelectCommander";
            this.btnSelectCommander.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Select;
            this.btnSelectCommander.Owner = null;
            this.btnSelectCommander.Size = new System.Drawing.Size(64, 35);
            this.btnSelectCommander.TabIndex = 99;
            this.btnSelectCommander.TextLocation = new System.Drawing.Point(0, 0);
            this.btnSelectCommander.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnSelectCommander.ToolTipText = "";
            this.btnSelectCommander.UseCustomImageRect = true;
            this.btnSelectCommander.UseTextLocation = true;
            this.btnSelectCommander.UseVisualStyleBackColor = true;
            this.btnSelectCommander.Visible = false;
            this.btnSelectCommander.Click += new System.EventHandler(this.btnSelectCommander_Click);
            // 
            // lblBroadcast
            // 
            this.lblBroadcast.AutoSize = true;
            this.lblBroadcast.Font = new System.Drawing.Font("나눔스퀘어", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblBroadcast.ForeColor = System.Drawing.Color.White;
            this.lblBroadcast.Location = new System.Drawing.Point(146, 43);
            this.lblBroadcast.Margin = new System.Windows.Forms.Padding(3, 6, 6, 0);
            this.lblBroadcast.Name = "lblBroadcast";
            this.lblBroadcast.Size = new System.Drawing.Size(72, 18);
            this.lblBroadcast.TabIndex = 46;
            this.lblBroadcast.Text = "방송전파";
            this.lblBroadcast.Click += new System.EventHandler(this.BroadcastMessage_Click);
            // 
            // btnShowSpecialMessage
            // 
            this.btnShowSpecialMessage.CheckButton = false;
            this.btnShowSpecialMessage.CheckedBkgndImage = null;
            this.btnShowSpecialMessage.CheckedImage = null;
            this.btnShowSpecialMessage.CheckedMouseOver = null;
            this.btnShowSpecialMessage.ClickedBackgroundImage = null;
            this.btnShowSpecialMessage.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_SpecialcharoptionClick;
            this.btnShowSpecialMessage.CustomImageRect = new System.Drawing.Rectangle(0, 0, 110, 37);
            this.btnShowSpecialMessage.DisabledBkgndImage = null;
            this.btnShowSpecialMessage.DisabledImage = null;
            this.btnShowSpecialMessage.ForeColorChecked = System.Drawing.Color.White;
            this.btnShowSpecialMessage.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnShowSpecialMessage.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnShowSpecialMessage.ForeColorsByTypeUse = false;
            this.btnShowSpecialMessage.ID = -1;
            this.btnShowSpecialMessage.InitButtonWidth = 110;
            this.btnShowSpecialMessage.IsChecked = false;
            this.btnShowSpecialMessage.Location = new System.Drawing.Point(16, 373);
            this.btnShowSpecialMessage.Margin = new System.Windows.Forms.Padding(0);
            this.btnShowSpecialMessage.MouseOverBkgndImage = null;
            this.btnShowSpecialMessage.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_SpecialcharoptionClick;
            this.btnShowSpecialMessage.Name = "btnShowSpecialMessage";
            this.btnShowSpecialMessage.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Specialcharoption;
            this.btnShowSpecialMessage.Owner = null;
            this.btnShowSpecialMessage.Size = new System.Drawing.Size(110, 37);
            this.btnShowSpecialMessage.TabIndex = 41;
            this.btnShowSpecialMessage.TextLocation = new System.Drawing.Point(-3, 18);
            this.btnShowSpecialMessage.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnShowSpecialMessage.ToolTipText = "";
            this.btnShowSpecialMessage.UseCustomImageRect = true;
            this.btnShowSpecialMessage.UseTextLocation = true;
            this.btnShowSpecialMessage.UseVisualStyleBackColor = true;
            this.btnShowSpecialMessage.Click += new System.EventHandler(this.btnSpecialMessage_Click);
            // 
            // picAutoRun
            // 
            this.picAutoRun.BackgroundImage = global::SOPManager.Properties.Resources.@__COMMON_ckb_enable;
            this.picAutoRun.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picAutoRun.Location = new System.Drawing.Point(225, 41);
            this.picAutoRun.Name = "picAutoRun";
            this.picAutoRun.Size = new System.Drawing.Size(20, 20);
            this.picAutoRun.TabIndex = 97;
            this.picAutoRun.TabStop = false;
            this.picAutoRun.Click += new System.EventHandler(this.AutoRun_Click);
            // 
            // lblSelectTeam
            // 
            this.lblSelectTeam.AutoSize = true;
            this.lblSelectTeam.Font = new System.Drawing.Font("나눔스퀘어", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblSelectTeam.ForeColor = System.Drawing.Color.White;
            this.lblSelectTeam.Location = new System.Drawing.Point(18, 71);
            this.lblSelectTeam.Margin = new System.Windows.Forms.Padding(27, 9, 3, 0);
            this.lblSelectTeam.Name = "lblSelectTeam";
            this.lblSelectTeam.Size = new System.Drawing.Size(67, 18);
            this.lblSelectTeam.TabIndex = 28;
            this.lblSelectTeam.Text = "수신자 :";
            // 
            // lblAutoRun
            // 
            this.lblAutoRun.AutoSize = true;
            this.lblAutoRun.Font = new System.Drawing.Font("나눔스퀘어", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblAutoRun.ForeColor = System.Drawing.Color.White;
            this.lblAutoRun.Location = new System.Drawing.Point(249, 43);
            this.lblAutoRun.Margin = new System.Windows.Forms.Padding(3, 6, 6, 0);
            this.lblAutoRun.Name = "lblAutoRun";
            this.lblAutoRun.Size = new System.Drawing.Size(77, 18);
            this.lblAutoRun.TabIndex = 98;
            this.lblAutoRun.Text = "자동 실행";
            this.lblAutoRun.Click += new System.EventHandler(this.AutoRun_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.CheckButton = false;
            this.btnPreview.CheckedBkgndImage = null;
            this.btnPreview.CheckedImage = null;
            this.btnPreview.CheckedMouseOver = null;
            this.btnPreview.ClickedBackgroundImage = null;
            this.btnPreview.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_PreviewClick;
            this.btnPreview.CustomImageRect = new System.Drawing.Rectangle(0, 0, 100, 37);
            this.btnPreview.DisabledBkgndImage = null;
            this.btnPreview.DisabledImage = null;
            this.btnPreview.ForeColorChecked = System.Drawing.Color.White;
            this.btnPreview.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnPreview.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnPreview.ForeColorsByTypeUse = false;
            this.btnPreview.ID = -1;
            this.btnPreview.InitButtonWidth = 100;
            this.btnPreview.IsChecked = false;
            this.btnPreview.Location = new System.Drawing.Point(126, 373);
            this.btnPreview.Margin = new System.Windows.Forms.Padding(0);
            this.btnPreview.MouseOverBkgndImage = null;
            this.btnPreview.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_PreviewClick;
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Preview;
            this.btnPreview.Owner = null;
            this.btnPreview.Size = new System.Drawing.Size(100, 37);
            this.btnPreview.TabIndex = 40;
            this.btnPreview.TextLocation = new System.Drawing.Point(-3, 18);
            this.btnPreview.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnPreview.ToolTipText = "";
            this.btnPreview.UseCustomImageRect = true;
            this.btnPreview.UseTextLocation = true;
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            this.btnPreview.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RibbonBtn_MouseDown);
            this.btnPreview.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RibbonBtn_MouseUp);
            // 
            // txtSelectTeam
            // 
            this.txtSelectTeam.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.txtSelectTeam.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSelectTeam.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtSelectTeam.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.txtSelectTeam.Location = new System.Drawing.Point(98, 67);
            this.txtSelectTeam.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.txtSelectTeam.Name = "txtSelectTeam";
            this.txtSelectTeam.ReadOnly = true;
            this.txtSelectTeam.Size = new System.Drawing.Size(681, 27);
            this.txtSelectTeam.TabIndex = 29;
            // 
            // btnCancel
            // 
            this.btnCancel.CheckButton = false;
            this.btnCancel.CheckedBkgndImage = null;
            this.btnCancel.CheckedImage = null;
            this.btnCancel.CheckedMouseOver = null;
            this.btnCancel.ClickedBackgroundImage = null;
            this.btnCancel.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_CancelClick;
            this.btnCancel.CustomImageRect = new System.Drawing.Rectangle(0, 0, 69, 37);
            this.btnCancel.DisabledBkgndImage = null;
            this.btnCancel.DisabledImage = null;
            this.btnCancel.ForeColorChecked = System.Drawing.Color.White;
            this.btnCancel.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnCancel.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnCancel.ForeColorsByTypeUse = false;
            this.btnCancel.ID = -1;
            this.btnCancel.InitButtonWidth = 69;
            this.btnCancel.IsChecked = false;
            this.btnCancel.Location = new System.Drawing.Point(778, 371);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(0);
            this.btnCancel.MouseOverBkgndImage = null;
            this.btnCancel.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_CancelClick;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Cancel;
            this.btnCancel.Owner = null;
            this.btnCancel.Size = new System.Drawing.Size(69, 37);
            this.btnCancel.TabIndex = 39;
            this.btnCancel.TextLocation = new System.Drawing.Point(0, 0);
            this.btnCancel.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnCancel.ToolTipText = "";
            this.btnCancel.UseCustomImageRect = true;
            this.btnCancel.UseTextLocation = false;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnSelectTeam
            // 
            this.btnSelectTeam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSelectTeam.CheckButton = false;
            this.btnSelectTeam.CheckedBkgndImage = null;
            this.btnSelectTeam.CheckedImage = null;
            this.btnSelectTeam.CheckedMouseOver = null;
            this.btnSelectTeam.ClickedBackgroundImage = null;
            this.btnSelectTeam.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_SelectClick;
            this.btnSelectTeam.CustomImageRect = new System.Drawing.Rectangle(0, 0, 64, 35);
            this.btnSelectTeam.DisabledBkgndImage = null;
            this.btnSelectTeam.DisabledImage = null;
            this.btnSelectTeam.ForeColorChecked = System.Drawing.Color.White;
            this.btnSelectTeam.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnSelectTeam.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnSelectTeam.ForeColorsByTypeUse = false;
            this.btnSelectTeam.ID = -1;
            this.btnSelectTeam.InitButtonWidth = 64;
            this.btnSelectTeam.IsChecked = false;
            this.btnSelectTeam.Location = new System.Drawing.Point(782, 64);
            this.btnSelectTeam.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.btnSelectTeam.MouseOverBkgndImage = null;
            this.btnSelectTeam.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_SelectClick;
            this.btnSelectTeam.Name = "btnSelectTeam";
            this.btnSelectTeam.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Select;
            this.btnSelectTeam.Owner = null;
            this.btnSelectTeam.Size = new System.Drawing.Size(64, 35);
            this.btnSelectTeam.TabIndex = 100;
            this.btnSelectTeam.TextLocation = new System.Drawing.Point(0, 0);
            this.btnSelectTeam.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnSelectTeam.ToolTipText = "";
            this.btnSelectTeam.UseCustomImageRect = true;
            this.btnSelectTeam.UseTextLocation = true;
            this.btnSelectTeam.UseVisualStyleBackColor = true;
            this.btnSelectTeam.Click += new System.EventHandler(this.btnSelectTeam_Click);
            // 
            // btnTTS
            // 
            this.btnTTS.CheckButton = false;
            this.btnTTS.CheckedBkgndImage = null;
            this.btnTTS.CheckedImage = null;
            this.btnTTS.CheckedMouseOver = null;
            this.btnTTS.ClickedBackgroundImage = null;
            this.btnTTS.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_PreListenClick;
            this.btnTTS.CustomImageRect = new System.Drawing.Rectangle(0, 0, 100, 37);
            this.btnTTS.DisabledBkgndImage = null;
            this.btnTTS.DisabledImage = null;
            this.btnTTS.ForeColorChecked = System.Drawing.Color.White;
            this.btnTTS.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnTTS.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnTTS.ForeColorsByTypeUse = false;
            this.btnTTS.ID = -1;
            this.btnTTS.InitButtonWidth = 100;
            this.btnTTS.IsChecked = false;
            this.btnTTS.Location = new System.Drawing.Point(226, 373);
            this.btnTTS.Margin = new System.Windows.Forms.Padding(0);
            this.btnTTS.MouseOverBkgndImage = null;
            this.btnTTS.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_PreListenClick;
            this.btnTTS.Name = "btnTTS";
            this.btnTTS.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_PreListen;
            this.btnTTS.Owner = null;
            this.btnTTS.Size = new System.Drawing.Size(100, 37);
            this.btnTTS.TabIndex = 102;
            this.btnTTS.TextLocation = new System.Drawing.Point(-3, 18);
            this.btnTTS.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnTTS.ToolTipText = "";
            this.btnTTS.UseCustomImageRect = true;
            this.btnTTS.UseTextLocation = true;
            this.btnTTS.UseVisualStyleBackColor = true;
            this.btnTTS.Click += new System.EventHandler(this.btnTTS_Click);
            // 
            // lblHelpMessage
            // 
            this.lblHelpMessage.AutoSize = true;
            this.lblHelpMessage.BackColor = System.Drawing.Color.Transparent;
            this.lblHelpMessage.Font = new System.Drawing.Font("나눔스퀘어", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblHelpMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            this.lblHelpMessage.Location = new System.Drawing.Point(3, 419);
            this.lblHelpMessage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 0);
            this.lblHelpMessage.Name = "lblHelpMessage";
            this.lblHelpMessage.Size = new System.Drawing.Size(851, 17);
            this.lblHelpMessage.TabIndex = 1;
            this.lblHelpMessage.Text = "방송 또는 문자메시지를 이용하여 상황전파시 시나리오상에 정의되는 내용입니다. SOP 시스템에서 [시나리오]로 표현됩니다.\r\n";
            // 
            // btnOK
            // 
            this.btnOK.CheckButton = false;
            this.btnOK.CheckedBkgndImage = null;
            this.btnOK.CheckedImage = null;
            this.btnOK.CheckedMouseOver = null;
            this.btnOK.ClickedBackgroundImage = null;
            this.btnOK.ClickedImage = global::SOPManager.Properties.Resources.@__COMMON_OkClick;
            this.btnOK.CustomImageRect = new System.Drawing.Rectangle(0, 0, 69, 37);
            this.btnOK.DisabledBkgndImage = null;
            this.btnOK.DisabledImage = null;
            this.btnOK.ForeColorChecked = System.Drawing.Color.White;
            this.btnOK.ForeColorCheckedMouseOver = System.Drawing.Color.White;
            this.btnOK.ForeColorMouseOver = System.Drawing.Color.White;
            this.btnOK.ForeColorsByTypeUse = false;
            this.btnOK.ID = -1;
            this.btnOK.InitButtonWidth = 69;
            this.btnOK.IsChecked = false;
            this.btnOK.Location = new System.Drawing.Point(709, 371);
            this.btnOK.Margin = new System.Windows.Forms.Padding(0);
            this.btnOK.MouseOverBkgndImage = null;
            this.btnOK.MouseOverImage = global::SOPManager.Properties.Resources.@__COMMON_OkClick;
            this.btnOK.Name = "btnOK";
            this.btnOK.NormalImage = global::SOPManager.Properties.Resources.@__COMMON_Ok;
            this.btnOK.Owner = null;
            this.btnOK.Size = new System.Drawing.Size(69, 37);
            this.btnOK.TabIndex = 38;
            this.btnOK.TextLocation = new System.Drawing.Point(0, 0);
            this.btnOK.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.btnOK.ToolTipText = "";
            this.btnOK.UseCustomImageRect = true;
            this.btnOK.UseTextLocation = false;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.checkBoxAutoRun);
            this.groupBox1.Controls.Add(this.rbBtnBroadcast);
            this.groupBox1.Controls.Add(this.rbBtnMobile);
            this.groupBox1.Location = new System.Drawing.Point(874, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(370, 52);
            this.groupBox1.TabIndex = 34;
            this.groupBox1.TabStop = false;
            this.groupBox1.Visible = false;
            // 
            // checkBoxAutoRun
            // 
            this.checkBoxAutoRun.AutoSize = true;
            this.checkBoxAutoRun.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxAutoRun.ForeColor = System.Drawing.Color.White;
            this.checkBoxAutoRun.Location = new System.Drawing.Point(239, 17);
            this.checkBoxAutoRun.Name = "checkBoxAutoRun";
            this.checkBoxAutoRun.Size = new System.Drawing.Size(101, 20);
            this.checkBoxAutoRun.TabIndex = 38;
            this.checkBoxAutoRun.Text = "자동 실행";
            this.checkBoxAutoRun.UseVisualStyleBackColor = true;
            // 
            // rbBtnBroadcast
            // 
            this.rbBtnBroadcast.AutoSize = true;
            this.rbBtnBroadcast.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnBroadcast.ForeColor = System.Drawing.Color.White;
            this.rbBtnBroadcast.Location = new System.Drawing.Point(128, 18);
            this.rbBtnBroadcast.Name = "rbBtnBroadcast";
            this.rbBtnBroadcast.Size = new System.Drawing.Size(85, 19);
            this.rbBtnBroadcast.TabIndex = 36;
            this.rbBtnBroadcast.TabStop = true;
            this.rbBtnBroadcast.Text = "방송전파";
            this.rbBtnBroadcast.UseVisualStyleBackColor = true;
            this.rbBtnBroadcast.CheckedChanged += new System.EventHandler(this.rbBtnBroadcast_CheckedChanged);
            // 
            // rbBtnMobile
            // 
            this.rbBtnMobile.AutoSize = true;
            this.rbBtnMobile.Font = new System.Drawing.Font("굴림", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbBtnMobile.ForeColor = System.Drawing.Color.White;
            this.rbBtnMobile.Location = new System.Drawing.Point(15, 18);
            this.rbBtnMobile.Name = "rbBtnMobile";
            this.rbBtnMobile.Size = new System.Drawing.Size(85, 19);
            this.rbBtnMobile.TabIndex = 34;
            this.rbBtnMobile.TabStop = true;
            this.rbBtnMobile.Text = "문자발송";
            this.rbBtnMobile.UseVisualStyleBackColor = true;
            this.rbBtnMobile.CheckedChanged += new System.EventHandler(this.rbBtnMobile_CheckedChanged);
            // 
            // textMessage
            // 
            this.textMessage.Font = new System.Drawing.Font("굴림", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textMessage.Location = new System.Drawing.Point(18, 99);
            this.textMessage.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.textMessage.Multiline = true;
            this.textMessage.Name = "textMessage";
            this.textMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textMessage.Size = new System.Drawing.Size(824, 270);
            this.textMessage.TabIndex = 37;
            // 
            // PopupBroadcastMessage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.ClientSize = new System.Drawing.Size(858, 450);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(809, 450);
            this.Name = "PopupBroadcastMessage";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "전파내용 입력";
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTextSend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBroadcast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAutoRun)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lblHelpMessage;
        private System.Windows.Forms.Label labelNote;
        private System.Windows.Forms.Label lblCommander;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxCommander;
        private System.Windows.Forms.Label lblSelectTeam;
        private System.Windows.Forms.TextBox txtSelectTeam;
        private System.Windows.Forms.RadioButton rbBtnBroadcast;
        private System.Windows.Forms.RadioButton rbBtnMobile;
        private System.Windows.Forms.CheckBox checkBoxAutoRun;
        private UnE.GUI.RibbonButton btnCancel;
        private UnE.GUI.RibbonButton btnOK;
        private UnE.GUI.RibbonButton btnPreview;
        private UnE.GUI.RibbonButton btnShowSpecialMessage;
        private System.Windows.Forms.PictureBox picTextSend;
        private System.Windows.Forms.PictureBox picBroadcast;
        private System.Windows.Forms.Label lblBroadcast;
        private System.Windows.Forms.Label lblTextSend;
        private System.Windows.Forms.PictureBox picAutoRun;
        private System.Windows.Forms.Label lblAutoRun;
        private UnE.GUI.RibbonButton btnSelectCommander;
        private UnE.GUI.RibbonButton btnSelectTeam;
        private UnE.GUI.RibbonButton btnTTS;
        private System.Windows.Forms.TextBox textMessage;
    }
}