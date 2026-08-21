namespace ImageEditor
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelRight = new System.Windows.Forms.Panel();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.rbSelectCut = new UnE.GUI.RibbonButton();
            this.rbTransparent = new UnE.GUI.RibbonButton();
            this.rbReverse = new UnE.GUI.RibbonButton();
            this.rbAllSelect = new UnE.GUI.RibbonButton();
            this.rbRotate = new UnE.GUI.RibbonButton();
            this.rbSizeSetup = new UnE.GUI.RibbonButton();
            this.rbDelete = new UnE.GUI.RibbonButton();
            this.rbPaste = new UnE.GUI.RibbonButton();
            this.rbCut = new UnE.GUI.RibbonButton();
            this.rbCopy = new UnE.GUI.RibbonButton();
            this.파일ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mNewImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.이미지다른이름으로저장ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.끝내기ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.보기ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.확대ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.축소ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.격자ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.눈금자ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.panelMain.SuspendLayout();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.panelRight);
            this.panelMain.Controls.Add(this.panelLeft);
            this.panelMain.Controls.Add(this.panelTop);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 24);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1134, 648);
            this.panelMain.TabIndex = 1;
            // 
            // panelRight
            // 
            this.panelRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(334, 112);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(800, 536);
            this.panelRight.TabIndex = 0;
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 112);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(334, 536);
            this.panelLeft.TabIndex = 1;
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(71)))), ((int)(((byte)(86)))));
            this.panelTop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTop.Controls.Add(this.pictureBox1);
            this.panelTop.Controls.Add(this.rbSelectCut);
            this.panelTop.Controls.Add(this.rbTransparent);
            this.panelTop.Controls.Add(this.rbReverse);
            this.panelTop.Controls.Add(this.rbAllSelect);
            this.panelTop.Controls.Add(this.rbRotate);
            this.panelTop.Controls.Add(this.rbSizeSetup);
            this.panelTop.Controls.Add(this.rbDelete);
            this.panelTop.Controls.Add(this.rbPaste);
            this.panelTop.Controls.Add(this.rbCut);
            this.panelTop.Controls.Add(this.rbCopy);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1134, 112);
            this.panelTop.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ImageEditor.Properties.Resources.skin_line_img;
            this.pictureBox1.Location = new System.Drawing.Point(329, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(10, 95);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // rbSelectCut
            // 
            this.rbSelectCut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rbSelectCut.CheckButton = false;
            this.rbSelectCut.CheckedBkgndImage = global::ImageEditor.Properties.Resources.clicked배경;
            this.rbSelectCut.CheckedImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbSelectCut.ClickedBackgroundImage = global::ImageEditor.Properties.Resources.clicked배경;
            this.rbSelectCut.ClickedImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbSelectCut.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbSelectCut.DisabledBkgndImage = null;
            this.rbSelectCut.DisabledImage = global::ImageEditor.Properties.Resources.코딩가이드_disable;
            this.rbSelectCut.ID = -1;
            this.rbSelectCut.InitButtonWidth = 70;
            this.rbSelectCut.IsChecked = false;
            this.rbSelectCut.Location = new System.Drawing.Point(750, 16);
            this.rbSelectCut.MouseOverBkgndImage = global::ImageEditor.Properties.Resources.Ribon_mouse_over_background;
            this.rbSelectCut.MouseOverImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbSelectCut.Name = "rbSelectCut";
            this.rbSelectCut.NormalImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbSelectCut.Owner = null;
            this.rbSelectCut.Size = new System.Drawing.Size(70, 88);
            this.rbSelectCut.TabIndex = 1;
            this.rbSelectCut.Text = "자르기";
            this.rbSelectCut.TextLocation = new System.Drawing.Point(0, 0);
            this.rbSelectCut.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbSelectCut.ToolTipText = "자르기";
            this.rbSelectCut.UseCustomImageRect = true;
            this.rbSelectCut.UseTextLocation = false;
            this.rbSelectCut.UseVisualStyleBackColor = true;
            // 
            // rbTransparent
            // 
            this.rbTransparent.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rbTransparent.CheckButton = false;
            this.rbTransparent.CheckedBkgndImage = global::ImageEditor.Properties.Resources.clicked배경;
            this.rbTransparent.CheckedImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbTransparent.ClickedBackgroundImage = global::ImageEditor.Properties.Resources.clicked배경;
            this.rbTransparent.ClickedImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbTransparent.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbTransparent.DisabledBkgndImage = null;
            this.rbTransparent.DisabledImage = global::ImageEditor.Properties.Resources.코딩가이드_disable;
            this.rbTransparent.ID = -1;
            this.rbTransparent.InitButtonWidth = 70;
            this.rbTransparent.IsChecked = false;
            this.rbTransparent.Location = new System.Drawing.Point(670, 16);
            this.rbTransparent.MouseOverBkgndImage = global::ImageEditor.Properties.Resources.Ribon_mouse_over_background;
            this.rbTransparent.MouseOverImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbTransparent.Name = "rbTransparent";
            this.rbTransparent.NormalImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbTransparent.Owner = null;
            this.rbTransparent.Size = new System.Drawing.Size(85, 88);
            this.rbTransparent.TabIndex = 1;
            this.rbTransparent.Text = "선택영역 투명";
            this.rbTransparent.TextLocation = new System.Drawing.Point(0, 0);
            this.rbTransparent.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbTransparent.ToolTipText = "선택영역 투명";
            this.rbTransparent.UseCustomImageRect = true;
            this.rbTransparent.UseTextLocation = false;
            this.rbTransparent.UseVisualStyleBackColor = true;
            // 
            // rbReverse
            // 
            this.rbReverse.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rbReverse.CheckButton = false;
            this.rbReverse.CheckedBkgndImage = global::ImageEditor.Properties.Resources.clicked배경;
            this.rbReverse.CheckedImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbReverse.ClickedBackgroundImage = global::ImageEditor.Properties.Resources.clicked배경;
            this.rbReverse.ClickedImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbReverse.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbReverse.DisabledBkgndImage = null;
            this.rbReverse.DisabledImage = global::ImageEditor.Properties.Resources.코딩가이드_disable;
            this.rbReverse.ID = -1;
            this.rbReverse.InitButtonWidth = 70;
            this.rbReverse.IsChecked = false;
            this.rbReverse.Location = new System.Drawing.Point(590, 16);
            this.rbReverse.MouseOverBkgndImage = global::ImageEditor.Properties.Resources.Ribon_mouse_over_background;
            this.rbReverse.MouseOverImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbReverse.Name = "rbReverse";
            this.rbReverse.NormalImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbReverse.Owner = null;
            this.rbReverse.Size = new System.Drawing.Size(70, 88);
            this.rbReverse.TabIndex = 1;
            this.rbReverse.Text = "선택반전";
            this.rbReverse.TextLocation = new System.Drawing.Point(0, 0);
            this.rbReverse.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbReverse.ToolTipText = "선택반전";
            this.rbReverse.UseCustomImageRect = true;
            this.rbReverse.UseTextLocation = false;
            this.rbReverse.UseVisualStyleBackColor = true;
            // 
            // rbAllSelect
            // 
            this.rbAllSelect.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rbAllSelect.CheckButton = false;
            this.rbAllSelect.CheckedBkgndImage = global::ImageEditor.Properties.Resources.clicked배경;
            this.rbAllSelect.CheckedImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbAllSelect.ClickedBackgroundImage = global::ImageEditor.Properties.Resources.clicked배경;
            this.rbAllSelect.ClickedImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbAllSelect.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbAllSelect.DisabledBkgndImage = null;
            this.rbAllSelect.DisabledImage = global::ImageEditor.Properties.Resources.코딩가이드_disable;
            this.rbAllSelect.ID = -1;
            this.rbAllSelect.InitButtonWidth = 70;
            this.rbAllSelect.IsChecked = false;
            this.rbAllSelect.Location = new System.Drawing.Point(510, 16);
            this.rbAllSelect.MouseOverBkgndImage = global::ImageEditor.Properties.Resources.Ribon_mouse_over_background;
            this.rbAllSelect.MouseOverImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbAllSelect.Name = "rbAllSelect";
            this.rbAllSelect.NormalImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbAllSelect.Owner = null;
            this.rbAllSelect.Size = new System.Drawing.Size(70, 88);
            this.rbAllSelect.TabIndex = 1;
            this.rbAllSelect.Text = "모두선택";
            this.rbAllSelect.TextLocation = new System.Drawing.Point(0, 0);
            this.rbAllSelect.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbAllSelect.ToolTipText = "모두선택";
            this.rbAllSelect.UseCustomImageRect = true;
            this.rbAllSelect.UseTextLocation = false;
            this.rbAllSelect.UseVisualStyleBackColor = true;
            // 
            // rbRotate
            // 
            this.rbRotate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rbRotate.CheckButton = false;
            this.rbRotate.CheckedBkgndImage = global::ImageEditor.Properties.Resources.clicked배경;
            this.rbRotate.CheckedImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbRotate.ClickedBackgroundImage = global::ImageEditor.Properties.Resources.clicked배경;
            this.rbRotate.ClickedImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbRotate.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbRotate.DisabledBkgndImage = null;
            this.rbRotate.DisabledImage = global::ImageEditor.Properties.Resources.코딩가이드_disable;
            this.rbRotate.ID = -1;
            this.rbRotate.InitButtonWidth = 70;
            this.rbRotate.IsChecked = false;
            this.rbRotate.Location = new System.Drawing.Point(430, 16);
            this.rbRotate.MouseOverBkgndImage = global::ImageEditor.Properties.Resources.Ribon_mouse_over_background;
            this.rbRotate.MouseOverImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbRotate.Name = "rbRotate";
            this.rbRotate.NormalImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbRotate.Owner = null;
            this.rbRotate.Size = new System.Drawing.Size(70, 88);
            this.rbRotate.TabIndex = 1;
            this.rbRotate.Text = "회전";
            this.rbRotate.TextLocation = new System.Drawing.Point(0, 0);
            this.rbRotate.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbRotate.ToolTipText = "회전";
            this.rbRotate.UseCustomImageRect = true;
            this.rbRotate.UseTextLocation = false;
            this.rbRotate.UseVisualStyleBackColor = true;
            // 
            // rbSizeSetup
            // 
            this.rbSizeSetup.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rbSizeSetup.CheckButton = false;
            this.rbSizeSetup.CheckedBkgndImage = global::ImageEditor.Properties.Resources.clicked배경;
            this.rbSizeSetup.CheckedImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbSizeSetup.ClickedBackgroundImage = global::ImageEditor.Properties.Resources.clicked배경;
            this.rbSizeSetup.ClickedImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbSizeSetup.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbSizeSetup.DisabledBkgndImage = null;
            this.rbSizeSetup.DisabledImage = global::ImageEditor.Properties.Resources.코딩가이드_disable;
            this.rbSizeSetup.ID = -1;
            this.rbSizeSetup.InitButtonWidth = 70;
            this.rbSizeSetup.IsChecked = false;
            this.rbSizeSetup.Location = new System.Drawing.Point(350, 16);
            this.rbSizeSetup.MouseOverBkgndImage = global::ImageEditor.Properties.Resources.Ribon_mouse_over_background;
            this.rbSizeSetup.MouseOverImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbSizeSetup.Name = "rbSizeSetup";
            this.rbSizeSetup.NormalImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbSizeSetup.Owner = null;
            this.rbSizeSetup.Size = new System.Drawing.Size(70, 88);
            this.rbSizeSetup.TabIndex = 1;
            this.rbSizeSetup.Text = "크기조정";
            this.rbSizeSetup.TextLocation = new System.Drawing.Point(0, 0);
            this.rbSizeSetup.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbSizeSetup.ToolTipText = "크기조정";
            this.rbSizeSetup.UseCustomImageRect = true;
            this.rbSizeSetup.UseTextLocation = false;
            this.rbSizeSetup.UseVisualStyleBackColor = true;
            // 
            // rbDelete
            // 
            this.rbDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rbDelete.CheckButton = false;
            this.rbDelete.CheckedBkgndImage = global::ImageEditor.Properties.Resources.clicked배경;
            this.rbDelete.CheckedImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbDelete.ClickedBackgroundImage = global::ImageEditor.Properties.Resources.clicked배경;
            this.rbDelete.ClickedImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbDelete.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbDelete.DisabledBkgndImage = null;
            this.rbDelete.DisabledImage = global::ImageEditor.Properties.Resources.코딩가이드_disable;
            this.rbDelete.ID = -1;
            this.rbDelete.InitButtonWidth = 70;
            this.rbDelete.IsChecked = false;
            this.rbDelete.Location = new System.Drawing.Point(250, 16);
            this.rbDelete.MouseOverBkgndImage = global::ImageEditor.Properties.Resources.Ribon_mouse_over_background;
            this.rbDelete.MouseOverImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbDelete.Name = "rbDelete";
            this.rbDelete.NormalImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbDelete.Owner = null;
            this.rbDelete.Size = new System.Drawing.Size(70, 88);
            this.rbDelete.TabIndex = 1;
            this.rbDelete.Text = "삭제";
            this.rbDelete.TextLocation = new System.Drawing.Point(0, 0);
            this.rbDelete.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbDelete.ToolTipText = "삭제";
            this.rbDelete.UseCustomImageRect = true;
            this.rbDelete.UseTextLocation = false;
            this.rbDelete.UseVisualStyleBackColor = true;
            // 
            // rbPaste
            // 
            this.rbPaste.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rbPaste.CheckButton = false;
            this.rbPaste.CheckedBkgndImage = global::ImageEditor.Properties.Resources.clicked배경;
            this.rbPaste.CheckedImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbPaste.ClickedBackgroundImage = global::ImageEditor.Properties.Resources.clicked배경;
            this.rbPaste.ClickedImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbPaste.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbPaste.DisabledBkgndImage = null;
            this.rbPaste.DisabledImage = global::ImageEditor.Properties.Resources.코딩가이드_disable;
            this.rbPaste.ID = -1;
            this.rbPaste.InitButtonWidth = 70;
            this.rbPaste.IsChecked = false;
            this.rbPaste.Location = new System.Drawing.Point(170, 16);
            this.rbPaste.MouseOverBkgndImage = global::ImageEditor.Properties.Resources.Ribon_mouse_over_background;
            this.rbPaste.MouseOverImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbPaste.Name = "rbPaste";
            this.rbPaste.NormalImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbPaste.Owner = null;
            this.rbPaste.Size = new System.Drawing.Size(70, 88);
            this.rbPaste.TabIndex = 1;
            this.rbPaste.Text = "붙여넣기";
            this.rbPaste.TextLocation = new System.Drawing.Point(0, 0);
            this.rbPaste.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbPaste.ToolTipText = "붙여넣기";
            this.rbPaste.UseCustomImageRect = true;
            this.rbPaste.UseTextLocation = false;
            this.rbPaste.UseVisualStyleBackColor = true;
            // 
            // rbCut
            // 
            this.rbCut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rbCut.CheckButton = false;
            this.rbCut.CheckedBkgndImage = global::ImageEditor.Properties.Resources.clicked배경;
            this.rbCut.CheckedImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbCut.ClickedBackgroundImage = global::ImageEditor.Properties.Resources.clicked배경;
            this.rbCut.ClickedImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbCut.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbCut.DisabledBkgndImage = null;
            this.rbCut.DisabledImage = global::ImageEditor.Properties.Resources.코딩가이드_disable;
            this.rbCut.ID = -1;
            this.rbCut.InitButtonWidth = 70;
            this.rbCut.IsChecked = false;
            this.rbCut.Location = new System.Drawing.Point(90, 16);
            this.rbCut.MouseOverBkgndImage = global::ImageEditor.Properties.Resources.Ribon_mouse_over_background;
            this.rbCut.MouseOverImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbCut.Name = "rbCut";
            this.rbCut.NormalImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbCut.Owner = null;
            this.rbCut.Size = new System.Drawing.Size(70, 88);
            this.rbCut.TabIndex = 1;
            this.rbCut.Text = "잘라내기";
            this.rbCut.TextLocation = new System.Drawing.Point(0, 0);
            this.rbCut.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbCut.ToolTipText = "잘라내기";
            this.rbCut.UseCustomImageRect = true;
            this.rbCut.UseTextLocation = false;
            this.rbCut.UseVisualStyleBackColor = true;
            // 
            // rbCopy
            // 
            this.rbCopy.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rbCopy.CheckButton = false;
            this.rbCopy.CheckedBkgndImage = global::ImageEditor.Properties.Resources.clicked배경;
            this.rbCopy.CheckedImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbCopy.ClickedBackgroundImage = global::ImageEditor.Properties.Resources.clicked배경;
            this.rbCopy.ClickedImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbCopy.CustomImageRect = new System.Drawing.Rectangle(15, 8, 40, 40);
            this.rbCopy.DisabledBkgndImage = null;
            this.rbCopy.DisabledImage = global::ImageEditor.Properties.Resources.코딩가이드_disable;
            this.rbCopy.ID = -1;
            this.rbCopy.InitButtonWidth = 70;
            this.rbCopy.IsChecked = false;
            this.rbCopy.Location = new System.Drawing.Point(10, 16);
            this.rbCopy.MouseOverBkgndImage = global::ImageEditor.Properties.Resources.Ribon_mouse_over_background;
            this.rbCopy.MouseOverImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbCopy.Name = "rbCopy";
            this.rbCopy.NormalImage = global::ImageEditor.Properties.Resources.코딩가이드_normal;
            this.rbCopy.Owner = null;
            this.rbCopy.Size = new System.Drawing.Size(70, 88);
            this.rbCopy.TabIndex = 1;
            this.rbCopy.Text = "복사";
            this.rbCopy.TextLocation = new System.Drawing.Point(0, 0);
            this.rbCopy.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            this.rbCopy.ToolTipText = "복사";
            this.rbCopy.UseCustomImageRect = true;
            this.rbCopy.UseTextLocation = false;
            this.rbCopy.UseVisualStyleBackColor = true;
            // 
            // 파일ToolStripMenuItem
            // 
            this.파일ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mNewImageToolStripMenuItem,
            this.OpenImageToolStripMenuItem,
            this.SaveImageToolStripMenuItem,
            this.이미지다른이름으로저장ToolStripMenuItem,
            this.끝내기ToolStripMenuItem});
            this.파일ToolStripMenuItem.Name = "파일ToolStripMenuItem";
            this.파일ToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.파일ToolStripMenuItem.Text = "파일 (&F)";
            this.파일ToolStripMenuItem.Click += new System.EventHandler(this.보기ToolStripMenuItem_Click);
            // 
            // mNewImageToolStripMenuItem
            // 
            this.mNewImageToolStripMenuItem.Name = "mNewImageToolStripMenuItem";
            this.mNewImageToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.mNewImageToolStripMenuItem.Text = "새 이미지";
            this.mNewImageToolStripMenuItem.Click += new System.EventHandler(this.mNewImageToolStripMenuItem_Click);
            // 
            // OpenImageToolStripMenuItem
            // 
            this.OpenImageToolStripMenuItem.Name = "OpenImageToolStripMenuItem";
            this.OpenImageToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.OpenImageToolStripMenuItem.Text = "이미지 열기";
            this.OpenImageToolStripMenuItem.Click += new System.EventHandler(this.OpenImageToolStripMenuItem_Click);
            // 
            // SaveImageToolStripMenuItem
            // 
            this.SaveImageToolStripMenuItem.Name = "SaveImageToolStripMenuItem";
            this.SaveImageToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.SaveImageToolStripMenuItem.Text = "이미지 저장";
            this.SaveImageToolStripMenuItem.Click += new System.EventHandler(this.SaveImageToolStripMenuItem_Click);
            // 
            // 이미지다른이름으로저장ToolStripMenuItem
            // 
            this.이미지다른이름으로저장ToolStripMenuItem.Name = "이미지다른이름으로저장ToolStripMenuItem";
            this.이미지다른이름으로저장ToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.이미지다른이름으로저장ToolStripMenuItem.Text = "이미지 다른 이름으로 저장";
            this.이미지다른이름으로저장ToolStripMenuItem.Click += new System.EventHandler(this.이미지다른이름으로저장ToolStripMenuItem_Click);
            // 
            // 끝내기ToolStripMenuItem
            // 
            this.끝내기ToolStripMenuItem.Name = "끝내기ToolStripMenuItem";
            this.끝내기ToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.끝내기ToolStripMenuItem.Text = "끝내기";
            this.끝내기ToolStripMenuItem.Click += new System.EventHandler(this.끝내기ToolStripMenuItem_Click);
            // 
            // 보기ToolStripMenuItem
            // 
            this.보기ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.확대ToolStripMenuItem,
            this.축소ToolStripMenuItem,
            this.격자ToolStripMenuItem,
            this.눈금자ToolStripMenuItem});
            this.보기ToolStripMenuItem.Name = "보기ToolStripMenuItem";
            this.보기ToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.보기ToolStripMenuItem.Text = "보기 (&V)";
            // 
            // 확대ToolStripMenuItem
            // 
            this.확대ToolStripMenuItem.Name = "확대ToolStripMenuItem";
            this.확대ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.확대ToolStripMenuItem.Text = "확대";
            this.확대ToolStripMenuItem.Click += new System.EventHandler(this.확대ToolStripMenuItem_Click);
            // 
            // 축소ToolStripMenuItem
            // 
            this.축소ToolStripMenuItem.Name = "축소ToolStripMenuItem";
            this.축소ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.축소ToolStripMenuItem.Text = "축소";
            this.축소ToolStripMenuItem.Click += new System.EventHandler(this.축소ToolStripMenuItem_Click);
            // 
            // 격자ToolStripMenuItem
            // 
            this.격자ToolStripMenuItem.Name = "격자ToolStripMenuItem";
            this.격자ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.격자ToolStripMenuItem.Text = "격자";
            // 
            // 눈금자ToolStripMenuItem
            // 
            this.눈금자ToolStripMenuItem.Name = "눈금자ToolStripMenuItem";
            this.눈금자ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.눈금자ToolStripMenuItem.Text = "눈금자";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.파일ToolStripMenuItem,
            this.보기ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1134, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1134, 672);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "Form1";
            this.panelMain.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelTop;
        private UnE.GUI.RibbonButton rbCopy;
        private System.Windows.Forms.ToolStripMenuItem 파일ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mNewImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 이미지다른이름으로저장ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 끝내기ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 보기ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 확대ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 축소ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 격자ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 눈금자ToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private UnE.GUI.RibbonButton rbSelectCut;
        private UnE.GUI.RibbonButton rbTransparent;
        private UnE.GUI.RibbonButton rbReverse;
        private UnE.GUI.RibbonButton rbAllSelect;
        private UnE.GUI.RibbonButton rbRotate;
        private UnE.GUI.RibbonButton rbSizeSetup;
        private UnE.GUI.RibbonButton rbDelete;
        private UnE.GUI.RibbonButton rbPaste;
        private UnE.GUI.RibbonButton rbCut;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;


    }
}

