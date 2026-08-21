namespace ImageEditor
{
    partial class FormImageToolBar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormImageToolBar));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.cboLineThick = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cboTextFont = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cboTextSize = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnUnderline = new UnE.GUI.RibbonButton();
            this.btnLean = new UnE.GUI.RibbonButton();
            this.btnStrong = new UnE.GUI.RibbonButton();
            this.rbText = new UnE.GUI.RibbonButton();
            this.rbDrawCurve = new UnE.GUI.RibbonButton();
            this.rbDrawStraightLine = new UnE.GUI.RibbonButton();
            this.rbRotate = new UnE.GUI.RibbonButton();
            this.rbTranslate = new UnE.GUI.RibbonButton();
            this.rbZoomOut = new UnE.GUI.RibbonButton();
            this.rbZoomIn = new UnE.GUI.RibbonButton();
            this.rbLineColor = new UnE.GUI.RibbonButton();
            this.rbSelectColor = new UnE.GUI.RibbonButton();
            this.rbSelectArea = new UnE.GUI.RibbonButton();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.rbText);
            this.panel1.Controls.Add(this.rbDrawCurve);
            this.panel1.Controls.Add(this.pictureBox3);
            this.panel1.Controls.Add(this.rbDrawStraightLine);
            this.panel1.Controls.Add(this.rbRotate);
            this.panel1.Controls.Add(this.rbTranslate);
            this.panel1.Controls.Add(this.rbZoomOut);
            this.panel1.Controls.Add(this.rbZoomIn);
            this.panel1.Controls.Add(this.rbLineColor);
            this.panel1.Controls.Add(this.rbSelectColor);
            this.panel1.Controls.Add(this.rbSelectArea);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(1, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(872, 106);
            this.panel1.TabIndex = 4;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
            this.panel1.Resize += new System.EventHandler(this.panel1_Resize);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.cboLineThick);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Location = new System.Drawing.Point(761, 1);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(106, 98);
            this.panel3.TabIndex = 48;
            this.panel3.VisibleChanged += new System.EventHandler(this.panel3_VisibleChanged);
            // 
            // cboLineThick
            // 
            this.cboLineThick.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLineThick.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboLineThick.FormattingEnabled = true;
            this.cboLineThick.Items.AddRange(new object[] {
            "1px",
            "3px",
            "5px",
            "7px",
            "9px",
            "11px"});
            this.cboLineThick.Location = new System.Drawing.Point(10, 32);
            this.cboLineThick.Name = "cboLineThick";
            this.cboLineThick.Size = new System.Drawing.Size(83, 23);
            this.cboLineThick.TabIndex = 47;
            this.cboLineThick.SelectedIndexChanged += new System.EventHandler(this.cboLineThick_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(26, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 20);
            this.label4.TabIndex = 46;
            this.label4.Text = "선 굵기";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::ImageEditor.Properties.Resources.skin_line_img;
            this.pictureBox3.Location = new System.Drawing.Point(562, -1);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(10, 121);
            this.pictureBox3.TabIndex = 3;
            this.pictureBox3.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(439, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 20);
            this.label2.TabIndex = 46;
            this.label2.Text = "도형 및 글자";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(227, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 20);
            this.label1.TabIndex = 46;
            this.label1.Text = "선택영역 편집";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(17, 78);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(129, 20);
            this.label8.TabIndex = 46;
            this.label8.Text = "선택 및 색상 편집";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::ImageEditor.Properties.Resources.skin_line_img;
            this.pictureBox2.Location = new System.Drawing.Point(393, 1);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(10, 121);
            this.pictureBox2.TabIndex = 3;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ImageEditor.Properties.Resources.skin_line_img;
            this.pictureBox1.Location = new System.Drawing.Point(163, 1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(10, 109);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // cboTextFont
            // 
            this.cboTextFont.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTextFont.FormattingEnabled = true;
            this.cboTextFont.Location = new System.Drawing.Point(16, 8);
            this.cboTextFont.Name = "cboTextFont";
            this.cboTextFont.Size = new System.Drawing.Size(164, 20);
            this.cboTextFont.TabIndex = 4;
            this.cboTextFont.SelectedIndexChanged += new System.EventHandler(this.cboTextFont_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnUnderline);
            this.panel2.Controls.Add(this.btnLean);
            this.panel2.Controls.Add(this.btnStrong);
            this.panel2.Controls.Add(this.cboTextSize);
            this.panel2.Controls.Add(this.cboTextFont);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Location = new System.Drawing.Point(570, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(187, 98);
            this.panel2.TabIndex = 5;
            this.panel2.VisibleChanged += new System.EventHandler(this.panel2_VisibleChanged);
            // 
            // cboTextSize
            // 
            this.cboTextSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTextSize.FormattingEnabled = true;
            this.cboTextSize.Items.AddRange(new object[] {
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "16",
            "18",
            "20",
            "22",
            "24",
            "26",
            "28"});
            this.cboTextSize.Location = new System.Drawing.Point(16, 41);
            this.cboTextSize.Name = "cboTextSize";
            this.cboTextSize.Size = new System.Drawing.Size(69, 20);
            this.cboTextSize.TabIndex = 4;
            this.cboTextSize.SelectedIndexChanged += new System.EventHandler(this.cboTextSize_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(76, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 20);
            this.label3.TabIndex = 46;
            this.label3.Text = "글꼴";
            // 
            // btnUnderline
            // 
            this.btnUnderline.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnUnderline.CheckButton = false;
            this.btnUnderline.CheckedBkgndImage = ((System.Drawing.Image)(resources.GetObject("btnUnderline.CheckedBkgndImage")));
            this.btnUnderline.CheckedImage = null;
            this.btnUnderline.ClickedBackgroundImage = null;
            this.btnUnderline.ClickedImage = null;
            this.btnUnderline.CustomImageRect = new System.Drawing.Rectangle(3, 3, 23, 23);
            this.btnUnderline.DisabledBkgndImage = null;
            this.btnUnderline.DisabledImage = null;
            this.btnUnderline.ID = -1;
            this.btnUnderline.InitButtonWidth = 30;
            this.btnUnderline.IsChecked = false;
            this.btnUnderline.Location = new System.Drawing.Point(150, 34);
            this.btnUnderline.MouseOverBkgndImage = global::ImageEditor.Properties.Resources.over배경;
            this.btnUnderline.MouseOverImage = null;
            this.btnUnderline.Name = "btnUnderline";
            this.btnUnderline.NormalImage = global::ImageEditor.Properties.Resources.밑줄;
            this.btnUnderline.Owner = null;
            this.btnUnderline.Size = new System.Drawing.Size(30, 30);
            this.btnUnderline.TabIndex = 2;
            this.btnUnderline.TextLocation = new System.Drawing.Point(0, 0);
            this.btnUnderline.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.btnUnderline.ToolTipText = "";
            this.btnUnderline.UseCustomImageRect = true;
            this.btnUnderline.UseTextLocation = false;
            this.btnUnderline.UseVisualStyleBackColor = true;
            // 
            // btnLean
            // 
            this.btnLean.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnLean.CheckButton = false;
            this.btnLean.CheckedBkgndImage = ((System.Drawing.Image)(resources.GetObject("btnLean.CheckedBkgndImage")));
            this.btnLean.CheckedImage = null;
            this.btnLean.ClickedBackgroundImage = null;
            this.btnLean.ClickedImage = null;
            this.btnLean.CustomImageRect = new System.Drawing.Rectangle(3, 3, 23, 23);
            this.btnLean.DisabledBkgndImage = null;
            this.btnLean.DisabledImage = null;
            this.btnLean.ID = -1;
            this.btnLean.InitButtonWidth = 30;
            this.btnLean.IsChecked = false;
            this.btnLean.Location = new System.Drawing.Point(121, 34);
            this.btnLean.MouseOverBkgndImage = global::ImageEditor.Properties.Resources.over배경;
            this.btnLean.MouseOverImage = null;
            this.btnLean.Name = "btnLean";
            this.btnLean.NormalImage = global::ImageEditor.Properties.Resources.기울임꼴;
            this.btnLean.Owner = null;
            this.btnLean.Size = new System.Drawing.Size(30, 30);
            this.btnLean.TabIndex = 2;
            this.btnLean.TextLocation = new System.Drawing.Point(0, 0);
            this.btnLean.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.btnLean.ToolTipText = "";
            this.btnLean.UseCustomImageRect = true;
            this.btnLean.UseTextLocation = false;
            this.btnLean.UseVisualStyleBackColor = true;
            // 
            // btnStrong
            // 
            this.btnStrong.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnStrong.CheckButton = false;
            this.btnStrong.CheckedBkgndImage = ((System.Drawing.Image)(resources.GetObject("btnStrong.CheckedBkgndImage")));
            this.btnStrong.CheckedImage = null;
            this.btnStrong.ClickedBackgroundImage = null;
            this.btnStrong.ClickedImage = null;
            this.btnStrong.CustomImageRect = new System.Drawing.Rectangle(3, 3, 23, 23);
            this.btnStrong.DisabledBkgndImage = null;
            this.btnStrong.DisabledImage = null;
            this.btnStrong.ID = -1;
            this.btnStrong.InitButtonWidth = 30;
            this.btnStrong.IsChecked = false;
            this.btnStrong.Location = new System.Drawing.Point(92, 34);
            this.btnStrong.MouseOverBkgndImage = global::ImageEditor.Properties.Resources.over배경;
            this.btnStrong.MouseOverImage = null;
            this.btnStrong.Name = "btnStrong";
            this.btnStrong.NormalImage = global::ImageEditor.Properties.Resources.굵게;
            this.btnStrong.Owner = null;
            this.btnStrong.Size = new System.Drawing.Size(30, 30);
            this.btnStrong.TabIndex = 2;
            this.btnStrong.TextLocation = new System.Drawing.Point(0, 0);
            this.btnStrong.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.btnStrong.ToolTipText = "";
            this.btnStrong.UseCustomImageRect = true;
            this.btnStrong.UseTextLocation = false;
            this.btnStrong.UseVisualStyleBackColor = true;
            // 
            // rbText
            // 
            this.rbText.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rbText.CheckButton = false;
            this.rbText.CheckedBkgndImage = ((System.Drawing.Image)(resources.GetObject("rbText.CheckedBkgndImage")));
            this.rbText.CheckedImage = null;
            this.rbText.ClickedBackgroundImage = null;
            this.rbText.ClickedImage = null;
            this.rbText.CustomImageRect = new System.Drawing.Rectangle(3, 3, 39, 39);
            this.rbText.DisabledBkgndImage = null;
            this.rbText.DisabledImage = null;
            this.rbText.ID = -1;
            this.rbText.InitButtonWidth = 45;
            this.rbText.IsChecked = false;
            this.rbText.Location = new System.Drawing.Point(511, 8);
            this.rbText.MouseOverBkgndImage = global::ImageEditor.Properties.Resources.over배경;
            this.rbText.MouseOverImage = null;
            this.rbText.Name = "rbText";
            this.rbText.NormalImage = global::ImageEditor.Properties.Resources.아이콘9;
            this.rbText.Owner = null;
            this.rbText.Size = new System.Drawing.Size(45, 45);
            this.rbText.TabIndex = 2;
            this.rbText.TextLocation = new System.Drawing.Point(0, 0);
            this.rbText.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbText.ToolTipText = "";
            this.rbText.UseCustomImageRect = true;
            this.rbText.UseTextLocation = false;
            this.rbText.UseVisualStyleBackColor = true;
            // 
            // rbDrawCurve
            // 
            this.rbDrawCurve.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rbDrawCurve.CheckButton = false;
            this.rbDrawCurve.CheckedBkgndImage = ((System.Drawing.Image)(resources.GetObject("rbDrawCurve.CheckedBkgndImage")));
            this.rbDrawCurve.CheckedImage = null;
            this.rbDrawCurve.ClickedBackgroundImage = null;
            this.rbDrawCurve.ClickedImage = null;
            this.rbDrawCurve.CustomImageRect = new System.Drawing.Rectangle(3, 3, 39, 39);
            this.rbDrawCurve.DisabledBkgndImage = null;
            this.rbDrawCurve.DisabledImage = null;
            this.rbDrawCurve.ID = -1;
            this.rbDrawCurve.InitButtonWidth = 45;
            this.rbDrawCurve.IsChecked = false;
            this.rbDrawCurve.Location = new System.Drawing.Point(460, 8);
            this.rbDrawCurve.MouseOverBkgndImage = global::ImageEditor.Properties.Resources.over배경;
            this.rbDrawCurve.MouseOverImage = null;
            this.rbDrawCurve.Name = "rbDrawCurve";
            this.rbDrawCurve.NormalImage = global::ImageEditor.Properties.Resources.아이콘8;
            this.rbDrawCurve.Owner = null;
            this.rbDrawCurve.Size = new System.Drawing.Size(45, 45);
            this.rbDrawCurve.TabIndex = 2;
            this.rbDrawCurve.TextLocation = new System.Drawing.Point(0, 0);
            this.rbDrawCurve.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbDrawCurve.ToolTipText = "";
            this.rbDrawCurve.UseCustomImageRect = true;
            this.rbDrawCurve.UseTextLocation = false;
            this.rbDrawCurve.UseVisualStyleBackColor = true;
            // 
            // rbDrawStraightLine
            // 
            this.rbDrawStraightLine.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rbDrawStraightLine.CheckButton = false;
            this.rbDrawStraightLine.CheckedBkgndImage = ((System.Drawing.Image)(resources.GetObject("rbDrawStraightLine.CheckedBkgndImage")));
            this.rbDrawStraightLine.CheckedImage = null;
            this.rbDrawStraightLine.ClickedBackgroundImage = null;
            this.rbDrawStraightLine.ClickedImage = null;
            this.rbDrawStraightLine.CustomImageRect = new System.Drawing.Rectangle(3, 3, 39, 39);
            this.rbDrawStraightLine.DisabledBkgndImage = null;
            this.rbDrawStraightLine.DisabledImage = null;
            this.rbDrawStraightLine.ID = -1;
            this.rbDrawStraightLine.InitButtonWidth = 45;
            this.rbDrawStraightLine.IsChecked = false;
            this.rbDrawStraightLine.Location = new System.Drawing.Point(409, 8);
            this.rbDrawStraightLine.MouseOverBkgndImage = global::ImageEditor.Properties.Resources.over배경;
            this.rbDrawStraightLine.MouseOverImage = null;
            this.rbDrawStraightLine.Name = "rbDrawStraightLine";
            this.rbDrawStraightLine.NormalImage = global::ImageEditor.Properties.Resources.아이콘7;
            this.rbDrawStraightLine.Owner = null;
            this.rbDrawStraightLine.Size = new System.Drawing.Size(45, 45);
            this.rbDrawStraightLine.TabIndex = 2;
            this.rbDrawStraightLine.TextLocation = new System.Drawing.Point(0, 0);
            this.rbDrawStraightLine.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbDrawStraightLine.ToolTipText = "";
            this.rbDrawStraightLine.UseCustomImageRect = true;
            this.rbDrawStraightLine.UseTextLocation = false;
            this.rbDrawStraightLine.UseVisualStyleBackColor = true;
            // 
            // rbRotate
            // 
            this.rbRotate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rbRotate.CheckButton = false;
            this.rbRotate.CheckedBkgndImage = ((System.Drawing.Image)(resources.GetObject("rbRotate.CheckedBkgndImage")));
            this.rbRotate.CheckedImage = null;
            this.rbRotate.ClickedBackgroundImage = null;
            this.rbRotate.ClickedImage = null;
            this.rbRotate.CustomImageRect = new System.Drawing.Rectangle(3, 3, 39, 39);
            this.rbRotate.DisabledBkgndImage = null;
            this.rbRotate.DisabledImage = global::ImageEditor.Properties.Resources.disable_회전;
            this.rbRotate.ID = -1;
            this.rbRotate.InitButtonWidth = 45;
            this.rbRotate.IsChecked = false;
            this.rbRotate.Location = new System.Drawing.Point(338, 8);
            this.rbRotate.MouseOverBkgndImage = global::ImageEditor.Properties.Resources.over배경;
            this.rbRotate.MouseOverImage = null;
            this.rbRotate.Name = "rbRotate";
            this.rbRotate.NormalImage = global::ImageEditor.Properties.Resources.아이콘6;
            this.rbRotate.Owner = null;
            this.rbRotate.Size = new System.Drawing.Size(45, 45);
            this.rbRotate.TabIndex = 2;
            this.rbRotate.TextLocation = new System.Drawing.Point(0, 0);
            this.rbRotate.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbRotate.ToolTipText = "";
            this.rbRotate.UseCustomImageRect = true;
            this.rbRotate.UseTextLocation = false;
            this.rbRotate.UseVisualStyleBackColor = true;
            // 
            // rbTranslate
            // 
            this.rbTranslate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rbTranslate.CheckButton = false;
            this.rbTranslate.CheckedBkgndImage = ((System.Drawing.Image)(resources.GetObject("rbTranslate.CheckedBkgndImage")));
            this.rbTranslate.CheckedImage = null;
            this.rbTranslate.ClickedBackgroundImage = null;
            this.rbTranslate.ClickedImage = null;
            this.rbTranslate.CustomImageRect = new System.Drawing.Rectangle(3, 3, 39, 39);
            this.rbTranslate.DisabledBkgndImage = null;
            this.rbTranslate.DisabledImage = global::ImageEditor.Properties.Resources.disable_이동;
            this.rbTranslate.ID = -1;
            this.rbTranslate.InitButtonWidth = 45;
            this.rbTranslate.IsChecked = false;
            this.rbTranslate.Location = new System.Drawing.Point(284, 8);
            this.rbTranslate.MouseOverBkgndImage = global::ImageEditor.Properties.Resources.over배경;
            this.rbTranslate.MouseOverImage = null;
            this.rbTranslate.Name = "rbTranslate";
            this.rbTranslate.NormalImage = global::ImageEditor.Properties.Resources.이동;
            this.rbTranslate.Owner = null;
            this.rbTranslate.Size = new System.Drawing.Size(45, 45);
            this.rbTranslate.TabIndex = 2;
            this.rbTranslate.TextLocation = new System.Drawing.Point(0, 0);
            this.rbTranslate.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbTranslate.ToolTipText = "";
            this.rbTranslate.UseCustomImageRect = true;
            this.rbTranslate.UseTextLocation = false;
            this.rbTranslate.UseVisualStyleBackColor = true;
            this.rbTranslate.Click += new System.EventHandler(this.rbTranslate_Click);
            // 
            // rbZoomOut
            // 
            this.rbZoomOut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rbZoomOut.CheckButton = false;
            this.rbZoomOut.CheckedBkgndImage = ((System.Drawing.Image)(resources.GetObject("rbZoomOut.CheckedBkgndImage")));
            this.rbZoomOut.CheckedImage = null;
            this.rbZoomOut.ClickedBackgroundImage = null;
            this.rbZoomOut.ClickedImage = null;
            this.rbZoomOut.CustomImageRect = new System.Drawing.Rectangle(3, 3, 39, 39);
            this.rbZoomOut.DisabledBkgndImage = null;
            this.rbZoomOut.DisabledImage = global::ImageEditor.Properties.Resources.disable_축소;
            this.rbZoomOut.ID = -1;
            this.rbZoomOut.InitButtonWidth = 45;
            this.rbZoomOut.IsChecked = false;
            this.rbZoomOut.Location = new System.Drawing.Point(231, 8);
            this.rbZoomOut.MouseOverBkgndImage = global::ImageEditor.Properties.Resources.over배경;
            this.rbZoomOut.MouseOverImage = null;
            this.rbZoomOut.Name = "rbZoomOut";
            this.rbZoomOut.NormalImage = global::ImageEditor.Properties.Resources.아이콘5;
            this.rbZoomOut.Owner = null;
            this.rbZoomOut.Size = new System.Drawing.Size(45, 45);
            this.rbZoomOut.TabIndex = 2;
            this.rbZoomOut.TextLocation = new System.Drawing.Point(0, 0);
            this.rbZoomOut.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbZoomOut.ToolTipText = "";
            this.rbZoomOut.UseCustomImageRect = true;
            this.rbZoomOut.UseTextLocation = false;
            this.rbZoomOut.UseVisualStyleBackColor = true;
            // 
            // rbZoomIn
            // 
            this.rbZoomIn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rbZoomIn.CheckButton = false;
            this.rbZoomIn.CheckedBkgndImage = ((System.Drawing.Image)(resources.GetObject("rbZoomIn.CheckedBkgndImage")));
            this.rbZoomIn.CheckedImage = null;
            this.rbZoomIn.ClickedBackgroundImage = null;
            this.rbZoomIn.ClickedImage = null;
            this.rbZoomIn.CustomImageRect = new System.Drawing.Rectangle(3, 3, 39, 39);
            this.rbZoomIn.DisabledBkgndImage = null;
            this.rbZoomIn.DisabledImage = ((System.Drawing.Image)(resources.GetObject("rbZoomIn.DisabledImage")));
            this.rbZoomIn.ID = -1;
            this.rbZoomIn.InitButtonWidth = 45;
            this.rbZoomIn.IsChecked = false;
            this.rbZoomIn.Location = new System.Drawing.Point(179, 8);
            this.rbZoomIn.MouseOverBkgndImage = global::ImageEditor.Properties.Resources.over배경;
            this.rbZoomIn.MouseOverImage = null;
            this.rbZoomIn.Name = "rbZoomIn";
            this.rbZoomIn.NormalImage = global::ImageEditor.Properties.Resources.아이콘4;
            this.rbZoomIn.Owner = null;
            this.rbZoomIn.Size = new System.Drawing.Size(45, 45);
            this.rbZoomIn.TabIndex = 2;
            this.rbZoomIn.TextLocation = new System.Drawing.Point(0, 0);
            this.rbZoomIn.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbZoomIn.ToolTipText = "";
            this.rbZoomIn.UseCustomImageRect = true;
            this.rbZoomIn.UseTextLocation = false;
            this.rbZoomIn.UseVisualStyleBackColor = true;
            // 
            // rbLineColor
            // 
            this.rbLineColor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rbLineColor.CheckButton = false;
            this.rbLineColor.CheckedBkgndImage = global::ImageEditor.Properties.Resources._44;
            this.rbLineColor.CheckedImage = null;
            this.rbLineColor.ClickedBackgroundImage = null;
            this.rbLineColor.ClickedImage = null;
            this.rbLineColor.CustomImageRect = new System.Drawing.Rectangle(3, 3, 39, 39);
            this.rbLineColor.DisabledBkgndImage = null;
            this.rbLineColor.DisabledImage = null;
            this.rbLineColor.ID = -1;
            this.rbLineColor.InitButtonWidth = 45;
            this.rbLineColor.IsChecked = false;
            this.rbLineColor.Location = new System.Drawing.Point(112, 8);
            this.rbLineColor.MouseOverBkgndImage = global::ImageEditor.Properties.Resources.over배경;
            this.rbLineColor.MouseOverImage = null;
            this.rbLineColor.Name = "rbLineColor";
            this.rbLineColor.NormalImage = global::ImageEditor.Properties.Resources.아이콘2;
            this.rbLineColor.Owner = null;
            this.rbLineColor.Size = new System.Drawing.Size(45, 45);
            this.rbLineColor.TabIndex = 2;
            this.rbLineColor.TextLocation = new System.Drawing.Point(0, 0);
            this.rbLineColor.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbLineColor.ToolTipText = "";
            this.rbLineColor.UseCustomImageRect = true;
            this.rbLineColor.UseTextLocation = false;
            this.rbLineColor.UseVisualStyleBackColor = true;
            // 
            // rbSelectColor
            // 
            this.rbSelectColor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rbSelectColor.CheckButton = false;
            this.rbSelectColor.CheckedBkgndImage = global::ImageEditor.Properties.Resources._44;
            this.rbSelectColor.CheckedImage = null;
            this.rbSelectColor.ClickedBackgroundImage = null;
            this.rbSelectColor.ClickedImage = null;
            this.rbSelectColor.CustomImageRect = new System.Drawing.Rectangle(3, 3, 39, 39);
            this.rbSelectColor.DisabledBkgndImage = null;
            this.rbSelectColor.DisabledImage = null;
            this.rbSelectColor.ID = -1;
            this.rbSelectColor.InitButtonWidth = 45;
            this.rbSelectColor.IsChecked = false;
            this.rbSelectColor.Location = new System.Drawing.Point(61, 8);
            this.rbSelectColor.MouseOverBkgndImage = global::ImageEditor.Properties.Resources.over배경;
            this.rbSelectColor.MouseOverImage = null;
            this.rbSelectColor.Name = "rbSelectColor";
            this.rbSelectColor.NormalImage = global::ImageEditor.Properties.Resources.아이콘3;
            this.rbSelectColor.Owner = null;
            this.rbSelectColor.Size = new System.Drawing.Size(45, 45);
            this.rbSelectColor.TabIndex = 2;
            this.rbSelectColor.TextLocation = new System.Drawing.Point(0, 0);
            this.rbSelectColor.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbSelectColor.ToolTipText = "";
            this.rbSelectColor.UseCustomImageRect = true;
            this.rbSelectColor.UseTextLocation = false;
            this.rbSelectColor.UseVisualStyleBackColor = true;
            // 
            // rbSelectArea
            // 
            this.rbSelectArea.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rbSelectArea.CheckButton = false;
            this.rbSelectArea.CheckedBkgndImage = global::ImageEditor.Properties.Resources._44;
            this.rbSelectArea.CheckedImage = null;
            this.rbSelectArea.ClickedBackgroundImage = null;
            this.rbSelectArea.ClickedImage = null;
            this.rbSelectArea.CustomImageRect = new System.Drawing.Rectangle(3, 3, 39, 39);
            this.rbSelectArea.DisabledBkgndImage = null;
            this.rbSelectArea.DisabledImage = null;
            this.rbSelectArea.ID = -1;
            this.rbSelectArea.InitButtonWidth = 45;
            this.rbSelectArea.IsChecked = false;
            this.rbSelectArea.Location = new System.Drawing.Point(10, 8);
            this.rbSelectArea.MouseOverBkgndImage = global::ImageEditor.Properties.Resources.over배경;
            this.rbSelectArea.MouseOverImage = null;
            this.rbSelectArea.Name = "rbSelectArea";
            this.rbSelectArea.NormalImage = global::ImageEditor.Properties.Resources.아이콘1;
            this.rbSelectArea.Owner = null;
            this.rbSelectArea.Size = new System.Drawing.Size(45, 45);
            this.rbSelectArea.TabIndex = 2;
            this.rbSelectArea.TextLocation = new System.Drawing.Point(0, 0);
            this.rbSelectArea.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbSelectArea.ToolTipText = "";
            this.rbSelectArea.UseCustomImageRect = true;
            this.rbSelectArea.UseTextLocation = false;
            this.rbSelectArea.UseVisualStyleBackColor = true;
            // 
            // FormImageToolBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.ClientSize = new System.Drawing.Size(878, 116);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormImageToolBar";
            this.Text = "FormImageToolBar";
            this.Load += new System.EventHandler(this.FormImageToolBar_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormImageToolBar_MouseUp);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private UnE.GUI.RibbonButton rbSelectArea;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox cboTextFont;
        private System.Windows.Forms.Label label8;
        private UnE.GUI.RibbonButton rbLineColor;
        private UnE.GUI.RibbonButton rbSelectColor;
        private UnE.GUI.RibbonButton rbZoomIn;
        private UnE.GUI.RibbonButton rbZoomOut;
        private System.Windows.Forms.Label label1;
        private UnE.GUI.RibbonButton rbRotate;
        private System.Windows.Forms.PictureBox pictureBox2;
        private UnE.GUI.RibbonButton rbText;
        private UnE.GUI.RibbonButton rbDrawCurve;
        private UnE.GUI.RibbonButton rbDrawStraightLine;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Panel panel2;
        private UnE.GUI.RibbonButton btnStrong;
        private System.Windows.Forms.ComboBox cboTextSize;
        private System.Windows.Forms.Label label3;
        private UnE.GUI.RibbonButton btnUnderline;
        private UnE.GUI.RibbonButton btnLean;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label4;
        private UnE.GUI.RibbonButton rbTranslate;
        private System.Windows.Forms.ComboBox cboLineThick;
    }
}