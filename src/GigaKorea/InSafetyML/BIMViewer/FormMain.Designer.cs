namespace BIMViewer
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.panelToolbar = new System.Windows.Forms.Panel();
            this.panelProperty = new System.Windows.Forms.Panel();
            this.rbtnProperty = new UnE.GUI.RibbonButton();
            this.pnlProperty = new System.Windows.Forms.Panel();
            this.rbtnPropertyDone = new UnE.GUI.RibbonButton();
            this.panelBuilding = new System.Windows.Forms.Panel();
            this.rbtnBuildingDone = new UnE.GUI.RibbonButton();
            this.pnlBuilding = new System.Windows.Forms.Panel();
            this.rbtnBuilding = new UnE.GUI.RibbonButton();
            this.pnlSave = new System.Windows.Forms.Panel();
            this.panelLine = new System.Windows.Forms.Panel();
            this.rbtnAddLine = new UnE.GUI.RibbonButton();
            this.cbLineList = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.rbtnDoneLine = new UnE.GUI.RibbonButton();
            this.rbtnDeleteLine = new UnE.GUI.RibbonButton();
            this.rbtnMoveLine = new UnE.GUI.RibbonButton();
            this.panelPOI = new System.Windows.Forms.Panel();
            this.rbtnAdd = new UnE.GUI.RibbonButton();
            this.cbPOIList = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.rbtnDone = new UnE.GUI.RibbonButton();
            this.rbtnDelete = new UnE.GUI.RibbonButton();
            this.rbtnMove = new UnE.GUI.RibbonButton();
            this.rbtnLine = new UnE.GUI.RibbonButton();
            this.rbtnEdit = new UnE.GUI.RibbonButton();
            this.rbtnLayer = new UnE.GUI.RibbonButton();
            this.rbtnPOI = new UnE.GUI.RibbonButton();
            this.rbtnUpload = new UnE.GUI.RibbonButton();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelTitle = new System.Windows.Forms.Panel();
            this.tbLayoutPanel_top = new System.Windows.Forms.TableLayoutPanel();
            this.rbtnSave = new UnE.GUI.RibbonButton();
            this.rbtnOpen = new UnE.GUI.RibbonButton();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.imageButton1 = new UnE.GUI.ImageButton();
            this.clblLogin = new UnE.Controls.ColorLabel();
            this.colorLabel2 = new UnE.Controls.ColorLabel();
            this.ribbonButton1 = new UnE.GUI.RibbonButton();
            this.btnMin = new UnE.GUI.ImageButton();
            this.btnClose = new UnE.GUI.ImageButton();
            this.btnMax = new UnE.GUI.ImageButton();
            this.rbtnDownload = new UnE.GUI.RibbonButton();
            this.rbtnFormLayer = new UnE.GUI.RibbonButton();
            this.splitContainerLeft = new System.Windows.Forms.SplitContainer();
            this.splitContainerLeft2 = new System.Windows.Forms.SplitContainer();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelToolbar.SuspendLayout();
            this.panelProperty.SuspendLayout();
            this.panelBuilding.SuspendLayout();
            this.panelLine.SuspendLayout();
            this.panelPOI.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelTitle.SuspendLayout();
            this.tbLayoutPanel_top.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageButton1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLeft)).BeginInit();
            this.splitContainerLeft.Panel2.SuspendLayout();
            this.splitContainerLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLeft2)).BeginInit();
            this.splitContainerLeft2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelToolbar
            // 
            this.panelToolbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(27)))), ((int)(((byte)(43)))));
            this.panelToolbar.Controls.Add(this.panelProperty);
            this.panelToolbar.Controls.Add(this.panelBuilding);
            this.panelToolbar.Controls.Add(this.pnlSave);
            this.panelToolbar.Controls.Add(this.panelLine);
            this.panelToolbar.Controls.Add(this.panelPOI);
            this.panelToolbar.Controls.Add(this.rbtnLine);
            this.panelToolbar.Controls.Add(this.rbtnEdit);
            this.panelToolbar.Controls.Add(this.rbtnLayer);
            this.panelToolbar.Controls.Add(this.rbtnPOI);
            this.panelToolbar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelToolbar.Location = new System.Drawing.Point(0, 35);
            this.panelToolbar.Name = "panelToolbar";
            this.panelToolbar.Padding = new System.Windows.Forms.Padding(10);
            this.panelToolbar.Size = new System.Drawing.Size(1190, 93);
            this.panelToolbar.TabIndex = 0;
            // 
            // panelProperty
            // 
            this.panelProperty.BackColor = System.Drawing.Color.Gainsboro;
            this.panelProperty.Controls.Add(this.rbtnProperty);
            this.panelProperty.Controls.Add(this.pnlProperty);
            this.panelProperty.Controls.Add(this.rbtnPropertyDone);
            this.panelProperty.Location = new System.Drawing.Point(433, 22);
            this.panelProperty.Name = "panelProperty";
            this.panelProperty.Size = new System.Drawing.Size(438, 90);
            this.panelProperty.TabIndex = 24;
            // 
            // rbtnProperty
            // 
            this.rbtnProperty.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(27)))), ((int)(((byte)(43)))));
            this.rbtnProperty.CheckButton = false;
            this.rbtnProperty.CheckedBkgndImage = global::BIMViewer.Properties.Resources.clicked_background;
            this.rbtnProperty.CheckedImage = global::BIMViewer.Properties.Resources.property_MSclicked;
            this.rbtnProperty.CheckedMouseOver = global::BIMViewer.Properties.Resources.property_2nd_MSover;
            this.rbtnProperty.ClickedBackgroundImage = null;
            this.rbtnProperty.ClickedImage = global::BIMViewer.Properties.Resources.property_MSclicked;
            this.rbtnProperty.CustomImageRect = new System.Drawing.Rectangle(0, 0, 70, 90);
            this.rbtnProperty.DisabledBkgndImage = null;
            this.rbtnProperty.DisabledImage = null;
            this.rbtnProperty.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.rbtnProperty.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.rbtnProperty.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnProperty.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.rbtnProperty.ForeColorsByTypeUse = true;
            this.rbtnProperty.ID = -1;
            this.rbtnProperty.InitButtonWidth = 70;
            this.rbtnProperty.IsChecked = false;
            this.rbtnProperty.Location = new System.Drawing.Point(0, 0);
            this.rbtnProperty.Margin = new System.Windows.Forms.Padding(0);
            this.rbtnProperty.MouseOverBkgndImage = global::BIMViewer.Properties.Resources.mouse_over_background;
            this.rbtnProperty.MouseOverImage = global::BIMViewer.Properties.Resources.property_1st_MSover;
            this.rbtnProperty.Name = "rbtnProperty";
            this.rbtnProperty.NormalImage = global::BIMViewer.Properties.Resources.property_base;
            this.rbtnProperty.Owner = null;
            this.rbtnProperty.Size = new System.Drawing.Size(70, 90);
            this.rbtnProperty.TabIndex = 20;
            this.rbtnProperty.Text = "Property";
            this.rbtnProperty.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rbtnProperty.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnProperty.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbtnProperty.ToolTipText = "Property";
            this.rbtnProperty.UseCustomImageRect = true;
            this.rbtnProperty.UseTextLocation = false;
            this.rbtnProperty.UseVisualStyleBackColor = false;
            this.rbtnProperty.Click += new System.EventHandler(this.rbtnProperty_Click);
            // 
            // pnlProperty
            // 
            this.pnlProperty.BackColor = System.Drawing.Color.Gainsboro;
            this.pnlProperty.Location = new System.Drawing.Point(70, 0);
            this.pnlProperty.Name = "pnlProperty";
            this.pnlProperty.Size = new System.Drawing.Size(258, 90);
            this.pnlProperty.TabIndex = 23;
            // 
            // rbtnPropertyDone
            // 
            this.rbtnPropertyDone.BackColor = System.Drawing.Color.Transparent;
            this.rbtnPropertyDone.CheckButton = false;
            this.rbtnPropertyDone.CheckedBkgndImage = global::BIMViewer.Properties.Resources.clicked_background;
            this.rbtnPropertyDone.CheckedImage = global::BIMViewer.Properties.Resources.Done_MSover;
            this.rbtnPropertyDone.CheckedMouseOver = global::BIMViewer.Properties.Resources.Done_MSover;
            this.rbtnPropertyDone.ClickedBackgroundImage = null;
            this.rbtnPropertyDone.ClickedImage = global::BIMViewer.Properties.Resources.Done_MSover;
            this.rbtnPropertyDone.CustomImageRect = new System.Drawing.Rectangle(0, 0, 70, 90);
            this.rbtnPropertyDone.DisabledBkgndImage = null;
            this.rbtnPropertyDone.DisabledImage = null;
            this.rbtnPropertyDone.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(87)))), ((int)(((byte)(35)))));
            this.rbtnPropertyDone.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(87)))), ((int)(((byte)(35)))));
            this.rbtnPropertyDone.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(87)))), ((int)(((byte)(35)))));
            this.rbtnPropertyDone.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnPropertyDone.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(87)))), ((int)(((byte)(35)))));
            this.rbtnPropertyDone.ForeColorsByTypeUse = true;
            this.rbtnPropertyDone.ID = -1;
            this.rbtnPropertyDone.InitButtonWidth = 70;
            this.rbtnPropertyDone.IsChecked = false;
            this.rbtnPropertyDone.Location = new System.Drawing.Point(340, 0);
            this.rbtnPropertyDone.Margin = new System.Windows.Forms.Padding(0);
            this.rbtnPropertyDone.MouseOverBkgndImage = global::BIMViewer.Properties.Resources.mouse_over_background;
            this.rbtnPropertyDone.MouseOverImage = global::BIMViewer.Properties.Resources.Done_MSover;
            this.rbtnPropertyDone.Name = "rbtnPropertyDone";
            this.rbtnPropertyDone.NormalImage = global::BIMViewer.Properties.Resources.Done_base;
            this.rbtnPropertyDone.Owner = null;
            this.rbtnPropertyDone.Size = new System.Drawing.Size(70, 90);
            this.rbtnPropertyDone.TabIndex = 36;
            this.rbtnPropertyDone.Text = "\r\nDone";
            this.rbtnPropertyDone.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rbtnPropertyDone.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnPropertyDone.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbtnPropertyDone.ToolTipText = "\r\nDone";
            this.rbtnPropertyDone.UseCustomImageRect = true;
            this.rbtnPropertyDone.UseTextLocation = false;
            this.rbtnPropertyDone.UseVisualStyleBackColor = false;
            this.rbtnPropertyDone.Click += new System.EventHandler(this.RbtnPropertyDone_Click);
            // 
            // panelBuilding
            // 
            this.panelBuilding.BackColor = System.Drawing.Color.Gainsboro;
            this.panelBuilding.Controls.Add(this.rbtnBuildingDone);
            this.panelBuilding.Controls.Add(this.pnlBuilding);
            this.panelBuilding.Controls.Add(this.rbtnBuilding);
            this.panelBuilding.Location = new System.Drawing.Point(0, 0);
            this.panelBuilding.Name = "panelBuilding";
            this.panelBuilding.Size = new System.Drawing.Size(481, 90);
            this.panelBuilding.TabIndex = 24;
            // 
            // rbtnBuildingDone
            // 
            this.rbtnBuildingDone.BackColor = System.Drawing.Color.Transparent;
            this.rbtnBuildingDone.CheckButton = false;
            this.rbtnBuildingDone.CheckedBkgndImage = global::BIMViewer.Properties.Resources.clicked_background;
            this.rbtnBuildingDone.CheckedImage = global::BIMViewer.Properties.Resources.Done_MSover;
            this.rbtnBuildingDone.CheckedMouseOver = global::BIMViewer.Properties.Resources.Done_MSover;
            this.rbtnBuildingDone.ClickedBackgroundImage = null;
            this.rbtnBuildingDone.ClickedImage = global::BIMViewer.Properties.Resources.Done_MSover;
            this.rbtnBuildingDone.CustomImageRect = new System.Drawing.Rectangle(0, 0, 70, 90);
            this.rbtnBuildingDone.DisabledBkgndImage = null;
            this.rbtnBuildingDone.DisabledImage = null;
            this.rbtnBuildingDone.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(87)))), ((int)(((byte)(35)))));
            this.rbtnBuildingDone.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(87)))), ((int)(((byte)(35)))));
            this.rbtnBuildingDone.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(87)))), ((int)(((byte)(35)))));
            this.rbtnBuildingDone.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnBuildingDone.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(87)))), ((int)(((byte)(35)))));
            this.rbtnBuildingDone.ForeColorsByTypeUse = true;
            this.rbtnBuildingDone.ID = -1;
            this.rbtnBuildingDone.InitButtonWidth = 70;
            this.rbtnBuildingDone.IsChecked = false;
            this.rbtnBuildingDone.Location = new System.Drawing.Point(331, 3);
            this.rbtnBuildingDone.Margin = new System.Windows.Forms.Padding(0);
            this.rbtnBuildingDone.MouseOverBkgndImage = global::BIMViewer.Properties.Resources.mouse_over_background;
            this.rbtnBuildingDone.MouseOverImage = global::BIMViewer.Properties.Resources.Done_MSover;
            this.rbtnBuildingDone.Name = "rbtnBuildingDone";
            this.rbtnBuildingDone.NormalImage = global::BIMViewer.Properties.Resources.Done_base;
            this.rbtnBuildingDone.Owner = null;
            this.rbtnBuildingDone.Size = new System.Drawing.Size(70, 90);
            this.rbtnBuildingDone.TabIndex = 37;
            this.rbtnBuildingDone.Text = "\r\nDone";
            this.rbtnBuildingDone.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rbtnBuildingDone.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnBuildingDone.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbtnBuildingDone.ToolTipText = "\r\nDone";
            this.rbtnBuildingDone.UseCustomImageRect = true;
            this.rbtnBuildingDone.UseTextLocation = false;
            this.rbtnBuildingDone.UseVisualStyleBackColor = false;
            this.rbtnBuildingDone.Click += new System.EventHandler(this.RbtnBuildingDone_Click);
            // 
            // pnlBuilding
            // 
            this.pnlBuilding.BackColor = System.Drawing.Color.Gainsboro;
            this.pnlBuilding.Location = new System.Drawing.Point(70, 0);
            this.pnlBuilding.Name = "pnlBuilding";
            this.pnlBuilding.Size = new System.Drawing.Size(258, 90);
            this.pnlBuilding.TabIndex = 24;
            // 
            // rbtnBuilding
            // 
            this.rbtnBuilding.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(27)))), ((int)(((byte)(43)))));
            this.rbtnBuilding.CheckButton = false;
            this.rbtnBuilding.CheckedBkgndImage = global::BIMViewer.Properties.Resources.clicked_background;
            this.rbtnBuilding.CheckedImage = global::BIMViewer.Properties.Resources.property_MSclicked;
            this.rbtnBuilding.CheckedMouseOver = global::BIMViewer.Properties.Resources.property_2nd_MSover;
            this.rbtnBuilding.ClickedBackgroundImage = null;
            this.rbtnBuilding.ClickedImage = global::BIMViewer.Properties.Resources.property_MSclicked;
            this.rbtnBuilding.CustomImageRect = new System.Drawing.Rectangle(0, 0, 70, 90);
            this.rbtnBuilding.DisabledBkgndImage = null;
            this.rbtnBuilding.DisabledImage = null;
            this.rbtnBuilding.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.rbtnBuilding.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.rbtnBuilding.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnBuilding.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.rbtnBuilding.ForeColorsByTypeUse = true;
            this.rbtnBuilding.ID = -1;
            this.rbtnBuilding.InitButtonWidth = 70;
            this.rbtnBuilding.IsChecked = false;
            this.rbtnBuilding.Location = new System.Drawing.Point(0, 0);
            this.rbtnBuilding.Margin = new System.Windows.Forms.Padding(0);
            this.rbtnBuilding.MouseOverBkgndImage = global::BIMViewer.Properties.Resources.mouse_over_background;
            this.rbtnBuilding.MouseOverImage = global::BIMViewer.Properties.Resources.property_1st_MSover;
            this.rbtnBuilding.Name = "rbtnBuilding";
            this.rbtnBuilding.NormalImage = global::BIMViewer.Properties.Resources.property_base;
            this.rbtnBuilding.Owner = null;
            this.rbtnBuilding.Size = new System.Drawing.Size(70, 90);
            this.rbtnBuilding.TabIndex = 38;
            this.rbtnBuilding.Text = "Building";
            this.rbtnBuilding.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rbtnBuilding.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnBuilding.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbtnBuilding.ToolTipText = "Building";
            this.rbtnBuilding.UseCustomImageRect = true;
            this.rbtnBuilding.UseTextLocation = false;
            this.rbtnBuilding.UseVisualStyleBackColor = false;
            this.rbtnBuilding.Click += new System.EventHandler(this.rbtnBuilding_Click);
            // 
            // pnlSave
            // 
            this.pnlSave.BackColor = System.Drawing.Color.Silver;
            this.pnlSave.Location = new System.Drawing.Point(515, 14);
            this.pnlSave.Name = "pnlSave";
            this.pnlSave.Size = new System.Drawing.Size(64, 63);
            this.pnlSave.TabIndex = 37;
            // 
            // panelLine
            // 
            this.panelLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.panelLine.Controls.Add(this.rbtnAddLine);
            this.panelLine.Controls.Add(this.cbLineList);
            this.panelLine.Controls.Add(this.label6);
            this.panelLine.Controls.Add(this.label8);
            this.panelLine.Controls.Add(this.label10);
            this.panelLine.Controls.Add(this.rbtnDoneLine);
            this.panelLine.Controls.Add(this.rbtnDeleteLine);
            this.panelLine.Controls.Add(this.rbtnMoveLine);
            this.panelLine.Location = new System.Drawing.Point(683, 28);
            this.panelLine.Name = "panelLine";
            this.panelLine.Size = new System.Drawing.Size(473, 72);
            this.panelLine.TabIndex = 19;
            // 
            // rbtnAddLine
            // 
            this.rbtnAddLine.BackColor = System.Drawing.Color.Transparent;
            this.rbtnAddLine.CheckButton = false;
            this.rbtnAddLine.CheckedBkgndImage = global::BIMViewer.Properties.Resources.clicked_background;
            this.rbtnAddLine.CheckedImage = global::BIMViewer.Properties.Resources.Add_MSclicked;
            this.rbtnAddLine.CheckedMouseOver = global::BIMViewer.Properties.Resources.Add_1st_MSover;
            this.rbtnAddLine.ClickedBackgroundImage = null;
            this.rbtnAddLine.ClickedImage = global::BIMViewer.Properties.Resources.Add_MSclicked;
            this.rbtnAddLine.CustomImageRect = new System.Drawing.Rectangle(0, 0, 70, 90);
            this.rbtnAddLine.DisabledBkgndImage = null;
            this.rbtnAddLine.DisabledImage = null;
            this.rbtnAddLine.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnAddLine.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnAddLine.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnAddLine.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnAddLine.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnAddLine.ForeColorsByTypeUse = true;
            this.rbtnAddLine.ID = -1;
            this.rbtnAddLine.InitButtonWidth = 70;
            this.rbtnAddLine.IsChecked = false;
            this.rbtnAddLine.Location = new System.Drawing.Point(146, 4);
            this.rbtnAddLine.Margin = new System.Windows.Forms.Padding(0);
            this.rbtnAddLine.MouseOverBkgndImage = global::BIMViewer.Properties.Resources.mouse_over_background;
            this.rbtnAddLine.MouseOverImage = global::BIMViewer.Properties.Resources.Add_1st_MSover;
            this.rbtnAddLine.Name = "rbtnAddLine";
            this.rbtnAddLine.NormalImage = global::BIMViewer.Properties.Resources.Add_base;
            this.rbtnAddLine.Owner = null;
            this.rbtnAddLine.Size = new System.Drawing.Size(70, 58);
            this.rbtnAddLine.TabIndex = 22;
            this.rbtnAddLine.Text = "Add";
            this.rbtnAddLine.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rbtnAddLine.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnAddLine.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbtnAddLine.ToolTipText = "Add";
            this.rbtnAddLine.UseCustomImageRect = true;
            this.rbtnAddLine.UseTextLocation = false;
            this.rbtnAddLine.UseVisualStyleBackColor = false;
            this.rbtnAddLine.Click += new System.EventHandler(this.rbtnAddLine_Click);
            // 
            // cbLineList
            // 
            this.cbLineList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(36)))), ((int)(((byte)(39)))));
            this.cbLineList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbLineList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbLineList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbLineList.ForeColor = System.Drawing.Color.White;
            this.cbLineList.FormattingEnabled = true;
            this.cbLineList.Location = new System.Drawing.Point(53, 35);
            this.cbLineList.Name = "cbLineList";
            this.cbLineList.Size = new System.Drawing.Size(69, 22);
            this.cbLineList.TabIndex = 21;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(22)))), ((int)(((byte)(65)))));
            this.label6.Location = new System.Drawing.Point(20, 34);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(21, 15);
            this.label6.TabIndex = 18;
            this.label6.Text = "1F";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(22)))), ((int)(((byte)(65)))));
            this.label8.Location = new System.Drawing.Point(14, 12);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 15);
            this.label8.TabIndex = 16;
            this.label8.Text = "Floor";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(50, 15);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(68, 15);
            this.label10.TabIndex = 11;
            this.label10.Text = "Line Name";
            // 
            // rbtnDoneLine
            // 
            this.rbtnDoneLine.BackColor = System.Drawing.Color.Transparent;
            this.rbtnDoneLine.CheckButton = false;
            this.rbtnDoneLine.CheckedBkgndImage = global::BIMViewer.Properties.Resources.clicked_background;
            this.rbtnDoneLine.CheckedImage = global::BIMViewer.Properties.Resources.Done_MSover;
            this.rbtnDoneLine.CheckedMouseOver = global::BIMViewer.Properties.Resources.Done_MSover;
            this.rbtnDoneLine.ClickedBackgroundImage = null;
            this.rbtnDoneLine.ClickedImage = global::BIMViewer.Properties.Resources.Done_MSover;
            this.rbtnDoneLine.CustomImageRect = new System.Drawing.Rectangle(0, 0, 70, 90);
            this.rbtnDoneLine.DisabledBkgndImage = null;
            this.rbtnDoneLine.DisabledImage = null;
            this.rbtnDoneLine.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(87)))), ((int)(((byte)(35)))));
            this.rbtnDoneLine.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(87)))), ((int)(((byte)(35)))));
            this.rbtnDoneLine.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(87)))), ((int)(((byte)(35)))));
            this.rbtnDoneLine.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnDoneLine.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(87)))), ((int)(((byte)(35)))));
            this.rbtnDoneLine.ForeColorsByTypeUse = true;
            this.rbtnDoneLine.ID = -1;
            this.rbtnDoneLine.InitButtonWidth = 70;
            this.rbtnDoneLine.IsChecked = false;
            this.rbtnDoneLine.Location = new System.Drawing.Point(385, 1);
            this.rbtnDoneLine.Margin = new System.Windows.Forms.Padding(0);
            this.rbtnDoneLine.MouseOverBkgndImage = global::BIMViewer.Properties.Resources.mouse_over_background;
            this.rbtnDoneLine.MouseOverImage = global::BIMViewer.Properties.Resources.Done_MSover;
            this.rbtnDoneLine.Name = "rbtnDoneLine";
            this.rbtnDoneLine.NormalImage = global::BIMViewer.Properties.Resources.Done_base;
            this.rbtnDoneLine.Owner = null;
            this.rbtnDoneLine.Size = new System.Drawing.Size(70, 61);
            this.rbtnDoneLine.TabIndex = 5;
            this.rbtnDoneLine.Text = "Done";
            this.rbtnDoneLine.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rbtnDoneLine.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnDoneLine.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbtnDoneLine.ToolTipText = "Done";
            this.rbtnDoneLine.UseCustomImageRect = true;
            this.rbtnDoneLine.UseTextLocation = false;
            this.rbtnDoneLine.UseVisualStyleBackColor = false;
            this.rbtnDoneLine.Click += new System.EventHandler(this.rbtnDoneLine_Click);
            // 
            // rbtnDeleteLine
            // 
            this.rbtnDeleteLine.BackColor = System.Drawing.Color.Transparent;
            this.rbtnDeleteLine.CheckButton = false;
            this.rbtnDeleteLine.CheckedBkgndImage = global::BIMViewer.Properties.Resources.clicked_background;
            this.rbtnDeleteLine.CheckedImage = global::BIMViewer.Properties.Resources.Delete_MSclicked;
            this.rbtnDeleteLine.CheckedMouseOver = global::BIMViewer.Properties.Resources.Delete_1st_MSover;
            this.rbtnDeleteLine.ClickedBackgroundImage = null;
            this.rbtnDeleteLine.ClickedImage = global::BIMViewer.Properties.Resources.Delete_MSclicked;
            this.rbtnDeleteLine.CustomImageRect = new System.Drawing.Rectangle(0, 0, 70, 90);
            this.rbtnDeleteLine.DisabledBkgndImage = null;
            this.rbtnDeleteLine.DisabledImage = null;
            this.rbtnDeleteLine.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnDeleteLine.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnDeleteLine.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnDeleteLine.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnDeleteLine.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnDeleteLine.ForeColorsByTypeUse = true;
            this.rbtnDeleteLine.ID = -1;
            this.rbtnDeleteLine.InitButtonWidth = 70;
            this.rbtnDeleteLine.IsChecked = false;
            this.rbtnDeleteLine.Location = new System.Drawing.Point(302, 5);
            this.rbtnDeleteLine.Margin = new System.Windows.Forms.Padding(0);
            this.rbtnDeleteLine.MouseOverBkgndImage = global::BIMViewer.Properties.Resources.mouse_over_background;
            this.rbtnDeleteLine.MouseOverImage = global::BIMViewer.Properties.Resources.Delete_1st_MSover;
            this.rbtnDeleteLine.Name = "rbtnDeleteLine";
            this.rbtnDeleteLine.NormalImage = global::BIMViewer.Properties.Resources.Delete_base;
            this.rbtnDeleteLine.Owner = null;
            this.rbtnDeleteLine.Size = new System.Drawing.Size(70, 57);
            this.rbtnDeleteLine.TabIndex = 6;
            this.rbtnDeleteLine.Text = "Delete";
            this.rbtnDeleteLine.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rbtnDeleteLine.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnDeleteLine.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbtnDeleteLine.ToolTipText = "Delete";
            this.rbtnDeleteLine.UseCustomImageRect = true;
            this.rbtnDeleteLine.UseTextLocation = false;
            this.rbtnDeleteLine.UseVisualStyleBackColor = false;
            this.rbtnDeleteLine.Click += new System.EventHandler(this.rbtnDeleteLine_Click);
            // 
            // rbtnMoveLine
            // 
            this.rbtnMoveLine.BackColor = System.Drawing.Color.Transparent;
            this.rbtnMoveLine.CheckButton = false;
            this.rbtnMoveLine.CheckedBkgndImage = global::BIMViewer.Properties.Resources.clicked_background;
            this.rbtnMoveLine.CheckedImage = global::BIMViewer.Properties.Resources.Move_MSclicked;
            this.rbtnMoveLine.CheckedMouseOver = global::BIMViewer.Properties.Resources.Move_1st_MSover;
            this.rbtnMoveLine.ClickedBackgroundImage = null;
            this.rbtnMoveLine.ClickedImage = global::BIMViewer.Properties.Resources.Move_MSclicked;
            this.rbtnMoveLine.CustomImageRect = new System.Drawing.Rectangle(0, 0, 70, 90);
            this.rbtnMoveLine.DisabledBkgndImage = null;
            this.rbtnMoveLine.DisabledImage = null;
            this.rbtnMoveLine.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnMoveLine.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnMoveLine.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnMoveLine.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnMoveLine.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnMoveLine.ForeColorsByTypeUse = true;
            this.rbtnMoveLine.ID = -1;
            this.rbtnMoveLine.InitButtonWidth = 70;
            this.rbtnMoveLine.IsChecked = false;
            this.rbtnMoveLine.Location = new System.Drawing.Point(232, 3);
            this.rbtnMoveLine.Margin = new System.Windows.Forms.Padding(0);
            this.rbtnMoveLine.MouseOverBkgndImage = global::BIMViewer.Properties.Resources.mouse_over_background;
            this.rbtnMoveLine.MouseOverImage = global::BIMViewer.Properties.Resources.Move_1st_MSover;
            this.rbtnMoveLine.Name = "rbtnMoveLine";
            this.rbtnMoveLine.NormalImage = global::BIMViewer.Properties.Resources.Move_base;
            this.rbtnMoveLine.Owner = null;
            this.rbtnMoveLine.Size = new System.Drawing.Size(70, 56);
            this.rbtnMoveLine.TabIndex = 7;
            this.rbtnMoveLine.Text = "Move";
            this.rbtnMoveLine.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rbtnMoveLine.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnMoveLine.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbtnMoveLine.ToolTipText = "Move";
            this.rbtnMoveLine.UseCustomImageRect = true;
            this.rbtnMoveLine.UseTextLocation = false;
            this.rbtnMoveLine.UseVisualStyleBackColor = false;
            this.rbtnMoveLine.Click += new System.EventHandler(this.rbtnMoveLine_Click);
            // 
            // panelPOI
            // 
            this.panelPOI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.panelPOI.Controls.Add(this.rbtnAdd);
            this.panelPOI.Controls.Add(this.cbPOIList);
            this.panelPOI.Controls.Add(this.label5);
            this.panelPOI.Controls.Add(this.label4);
            this.panelPOI.Controls.Add(this.label3);
            this.panelPOI.Controls.Add(this.textBox1);
            this.panelPOI.Controls.Add(this.label2);
            this.panelPOI.Controls.Add(this.label1);
            this.panelPOI.Controls.Add(this.rbtnDone);
            this.panelPOI.Controls.Add(this.rbtnDelete);
            this.panelPOI.Controls.Add(this.rbtnMove);
            this.panelPOI.Location = new System.Drawing.Point(698, 3);
            this.panelPOI.Name = "panelPOI";
            this.panelPOI.Size = new System.Drawing.Size(492, 80);
            this.panelPOI.TabIndex = 8;
            // 
            // rbtnAdd
            // 
            this.rbtnAdd.BackColor = System.Drawing.Color.Transparent;
            this.rbtnAdd.CheckButton = false;
            this.rbtnAdd.CheckedBkgndImage = global::BIMViewer.Properties.Resources.clicked_background;
            this.rbtnAdd.CheckedImage = global::BIMViewer.Properties.Resources.Add_MSclicked;
            this.rbtnAdd.CheckedMouseOver = global::BIMViewer.Properties.Resources.Add_1st_MSover;
            this.rbtnAdd.ClickedBackgroundImage = null;
            this.rbtnAdd.ClickedImage = global::BIMViewer.Properties.Resources.Add_MSclicked;
            this.rbtnAdd.CustomImageRect = new System.Drawing.Rectangle(0, 0, 70, 90);
            this.rbtnAdd.DisabledBkgndImage = null;
            this.rbtnAdd.DisabledImage = null;
            this.rbtnAdd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnAdd.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnAdd.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnAdd.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnAdd.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnAdd.ForeColorsByTypeUse = true;
            this.rbtnAdd.ID = -1;
            this.rbtnAdd.InitButtonWidth = 70;
            this.rbtnAdd.IsChecked = false;
            this.rbtnAdd.Location = new System.Drawing.Point(238, 0);
            this.rbtnAdd.Margin = new System.Windows.Forms.Padding(0);
            this.rbtnAdd.MouseOverBkgndImage = global::BIMViewer.Properties.Resources.mouse_over_background;
            this.rbtnAdd.MouseOverImage = global::BIMViewer.Properties.Resources.Add_1st_MSover;
            this.rbtnAdd.Name = "rbtnAdd";
            this.rbtnAdd.NormalImage = global::BIMViewer.Properties.Resources.Add_base;
            this.rbtnAdd.Owner = null;
            this.rbtnAdd.Size = new System.Drawing.Size(70, 74);
            this.rbtnAdd.TabIndex = 20;
            this.rbtnAdd.Text = "Add";
            this.rbtnAdd.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rbtnAdd.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnAdd.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbtnAdd.ToolTipText = "Add";
            this.rbtnAdd.UseCustomImageRect = true;
            this.rbtnAdd.UseTextLocation = false;
            this.rbtnAdd.UseVisualStyleBackColor = false;
            this.rbtnAdd.Click += new System.EventHandler(this.rbtnAdd_Click);
            // 
            // cbPOIList
            // 
            this.cbPOIList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(36)))), ((int)(((byte)(39)))));
            this.cbPOIList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbPOIList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbPOIList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbPOIList.ForeColor = System.Drawing.Color.White;
            this.cbPOIList.FormattingEnabled = true;
            this.cbPOIList.Location = new System.Drawing.Point(78, 37);
            this.cbPOIList.Name = "cbPOIList";
            this.cbPOIList.Size = new System.Drawing.Size(71, 22);
            this.cbPOIList.TabIndex = 19;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(22)))), ((int)(((byte)(65)))));
            this.label5.Location = new System.Drawing.Point(47, 38);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(21, 15);
            this.label5.TabIndex = 18;
            this.label5.Text = "1F";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(215, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 15);
            this.label4.TabIndex = 17;
            this.label4.Text = "cm";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(22)))), ((int)(((byte)(65)))));
            this.label3.Location = new System.Drawing.Point(33, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 15);
            this.label3.TabIndex = 16;
            this.label3.Text = "Floor";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(36)))), ((int)(((byte)(39)))));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(158, 43);
            this.textBox1.Margin = new System.Windows.Forms.Padding(0);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(54, 17);
            this.textBox1.TabIndex = 14;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(155, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 15);
            this.label2.TabIndex = 13;
            this.label2.Text = "Height";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(75, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 15);
            this.label1.TabIndex = 11;
            this.label1.Text = "POI Name";
            // 
            // rbtnDone
            // 
            this.rbtnDone.BackColor = System.Drawing.Color.Transparent;
            this.rbtnDone.CheckButton = false;
            this.rbtnDone.CheckedBkgndImage = global::BIMViewer.Properties.Resources.clicked_background;
            this.rbtnDone.CheckedImage = global::BIMViewer.Properties.Resources.Done_MSover;
            this.rbtnDone.CheckedMouseOver = global::BIMViewer.Properties.Resources.Done_MSover;
            this.rbtnDone.ClickedBackgroundImage = null;
            this.rbtnDone.ClickedImage = global::BIMViewer.Properties.Resources.Done_MSover;
            this.rbtnDone.CustomImageRect = new System.Drawing.Rectangle(0, 0, 70, 90);
            this.rbtnDone.DisabledBkgndImage = null;
            this.rbtnDone.DisabledImage = null;
            this.rbtnDone.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(87)))), ((int)(((byte)(35)))));
            this.rbtnDone.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(87)))), ((int)(((byte)(35)))));
            this.rbtnDone.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(87)))), ((int)(((byte)(35)))));
            this.rbtnDone.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnDone.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(87)))), ((int)(((byte)(35)))));
            this.rbtnDone.ForeColorsByTypeUse = true;
            this.rbtnDone.ID = -1;
            this.rbtnDone.InitButtonWidth = 70;
            this.rbtnDone.IsChecked = false;
            this.rbtnDone.Location = new System.Drawing.Point(422, 5);
            this.rbtnDone.Margin = new System.Windows.Forms.Padding(0);
            this.rbtnDone.MouseOverBkgndImage = global::BIMViewer.Properties.Resources.mouse_over_background;
            this.rbtnDone.MouseOverImage = global::BIMViewer.Properties.Resources.Done_MSover;
            this.rbtnDone.Name = "rbtnDone";
            this.rbtnDone.NormalImage = global::BIMViewer.Properties.Resources.Done_base;
            this.rbtnDone.Owner = null;
            this.rbtnDone.Size = new System.Drawing.Size(70, 75);
            this.rbtnDone.TabIndex = 5;
            this.rbtnDone.Text = "Done";
            this.rbtnDone.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rbtnDone.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnDone.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbtnDone.ToolTipText = "Done";
            this.rbtnDone.UseCustomImageRect = true;
            this.rbtnDone.UseTextLocation = false;
            this.rbtnDone.UseVisualStyleBackColor = false;
            this.rbtnDone.Click += new System.EventHandler(this.rbtnDone_Click);
            // 
            // rbtnDelete
            // 
            this.rbtnDelete.BackColor = System.Drawing.Color.Transparent;
            this.rbtnDelete.CheckButton = false;
            this.rbtnDelete.CheckedBkgndImage = global::BIMViewer.Properties.Resources.clicked_background;
            this.rbtnDelete.CheckedImage = global::BIMViewer.Properties.Resources.Delete_MSclicked;
            this.rbtnDelete.CheckedMouseOver = global::BIMViewer.Properties.Resources.Delete_1st_MSover;
            this.rbtnDelete.ClickedBackgroundImage = null;
            this.rbtnDelete.ClickedImage = global::BIMViewer.Properties.Resources.Delete_MSclicked;
            this.rbtnDelete.CustomImageRect = new System.Drawing.Rectangle(0, 0, 70, 90);
            this.rbtnDelete.DisabledBkgndImage = null;
            this.rbtnDelete.DisabledImage = null;
            this.rbtnDelete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnDelete.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnDelete.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnDelete.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnDelete.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnDelete.ForeColorsByTypeUse = true;
            this.rbtnDelete.ID = -1;
            this.rbtnDelete.InitButtonWidth = 70;
            this.rbtnDelete.IsChecked = false;
            this.rbtnDelete.Location = new System.Drawing.Point(356, 4);
            this.rbtnDelete.Margin = new System.Windows.Forms.Padding(0);
            this.rbtnDelete.MouseOverBkgndImage = global::BIMViewer.Properties.Resources.mouse_over_background;
            this.rbtnDelete.MouseOverImage = global::BIMViewer.Properties.Resources.Delete_1st_MSover;
            this.rbtnDelete.Name = "rbtnDelete";
            this.rbtnDelete.NormalImage = global::BIMViewer.Properties.Resources.Delete_base;
            this.rbtnDelete.Owner = null;
            this.rbtnDelete.Size = new System.Drawing.Size(70, 71);
            this.rbtnDelete.TabIndex = 6;
            this.rbtnDelete.Text = "Delete";
            this.rbtnDelete.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rbtnDelete.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnDelete.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbtnDelete.ToolTipText = "Delete";
            this.rbtnDelete.UseCustomImageRect = true;
            this.rbtnDelete.UseTextLocation = false;
            this.rbtnDelete.UseVisualStyleBackColor = false;
            this.rbtnDelete.Click += new System.EventHandler(this.rbtnDelete_Click);
            // 
            // rbtnMove
            // 
            this.rbtnMove.BackColor = System.Drawing.Color.Transparent;
            this.rbtnMove.CheckButton = false;
            this.rbtnMove.CheckedBkgndImage = global::BIMViewer.Properties.Resources.clicked_background;
            this.rbtnMove.CheckedImage = global::BIMViewer.Properties.Resources.Move_MSclicked;
            this.rbtnMove.CheckedMouseOver = global::BIMViewer.Properties.Resources.Move_1st_MSover;
            this.rbtnMove.ClickedBackgroundImage = null;
            this.rbtnMove.ClickedImage = global::BIMViewer.Properties.Resources.Move_MSclicked;
            this.rbtnMove.CustomImageRect = new System.Drawing.Rectangle(0, 0, 70, 90);
            this.rbtnMove.DisabledBkgndImage = null;
            this.rbtnMove.DisabledImage = null;
            this.rbtnMove.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnMove.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnMove.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnMove.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnMove.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnMove.ForeColorsByTypeUse = true;
            this.rbtnMove.ID = -1;
            this.rbtnMove.InitButtonWidth = 70;
            this.rbtnMove.IsChecked = false;
            this.rbtnMove.Location = new System.Drawing.Point(295, 3);
            this.rbtnMove.Margin = new System.Windows.Forms.Padding(0);
            this.rbtnMove.MouseOverBkgndImage = global::BIMViewer.Properties.Resources.mouse_over_background;
            this.rbtnMove.MouseOverImage = global::BIMViewer.Properties.Resources.Move_1st_MSover;
            this.rbtnMove.Name = "rbtnMove";
            this.rbtnMove.NormalImage = global::BIMViewer.Properties.Resources.Move_base;
            this.rbtnMove.Owner = null;
            this.rbtnMove.Size = new System.Drawing.Size(70, 73);
            this.rbtnMove.TabIndex = 7;
            this.rbtnMove.Text = "Move";
            this.rbtnMove.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rbtnMove.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnMove.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbtnMove.ToolTipText = "Move";
            this.rbtnMove.UseCustomImageRect = true;
            this.rbtnMove.UseTextLocation = false;
            this.rbtnMove.UseVisualStyleBackColor = false;
            this.rbtnMove.Click += new System.EventHandler(this.rbtnMove_Click);
            // 
            // rbtnLine
            // 
            this.rbtnLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(27)))), ((int)(((byte)(43)))));
            this.rbtnLine.CheckButton = false;
            this.rbtnLine.CheckedBkgndImage = global::BIMViewer.Properties.Resources.clicked_background;
            this.rbtnLine.CheckedImage = global::BIMViewer.Properties.Resources.Line_MSclicked;
            this.rbtnLine.CheckedMouseOver = global::BIMViewer.Properties.Resources.Line_2nd_MSover;
            this.rbtnLine.ClickedBackgroundImage = null;
            this.rbtnLine.ClickedImage = global::BIMViewer.Properties.Resources.Line_MSclicked;
            this.rbtnLine.CustomImageRect = new System.Drawing.Rectangle(0, 0, 70, 90);
            this.rbtnLine.DisabledBkgndImage = null;
            this.rbtnLine.DisabledImage = global::BIMViewer.Properties.Resources.Line_None;
            this.rbtnLine.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnLine.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnLine.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnLine.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(104)))), ((int)(((byte)(147)))));
            this.rbtnLine.ForeColorsByTypeUse = true;
            this.rbtnLine.ID = -1;
            this.rbtnLine.InitButtonWidth = 70;
            this.rbtnLine.IsChecked = false;
            this.rbtnLine.Location = new System.Drawing.Point(661, 33);
            this.rbtnLine.Margin = new System.Windows.Forms.Padding(0);
            this.rbtnLine.MouseOverBkgndImage = global::BIMViewer.Properties.Resources.mouse_over_background;
            this.rbtnLine.MouseOverImage = global::BIMViewer.Properties.Resources.Line_1st_MSover;
            this.rbtnLine.Name = "rbtnLine";
            this.rbtnLine.NormalImage = global::BIMViewer.Properties.Resources.Line_base;
            this.rbtnLine.Owner = null;
            this.rbtnLine.Size = new System.Drawing.Size(70, 54);
            this.rbtnLine.TabIndex = 7;
            this.rbtnLine.Text = "LINE";
            this.rbtnLine.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rbtnLine.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnLine.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbtnLine.ToolTipText = "LINE";
            this.rbtnLine.UseCustomImageRect = true;
            this.rbtnLine.UseTextLocation = false;
            this.rbtnLine.UseVisualStyleBackColor = false;
            this.rbtnLine.Click += new System.EventHandler(this.rbtnLine_Click);
            // 
            // rbtnEdit
            // 
            this.rbtnEdit.BackColor = System.Drawing.Color.Transparent;
            this.rbtnEdit.CheckButton = false;
            this.rbtnEdit.CheckedBkgndImage = global::BIMViewer.Properties.Resources.clicked_background;
            this.rbtnEdit.CheckedImage = global::BIMViewer.Properties.Resources.Modify_MSclicked;
            this.rbtnEdit.CheckedMouseOver = global::BIMViewer.Properties.Resources.Modify_2nd_MSover;
            this.rbtnEdit.ClickedBackgroundImage = null;
            this.rbtnEdit.ClickedImage = global::BIMViewer.Properties.Resources.Modify_MSclicked;
            this.rbtnEdit.CustomImageRect = new System.Drawing.Rectangle(0, 0, 70, 90);
            this.rbtnEdit.DisabledBkgndImage = null;
            this.rbtnEdit.DisabledImage = null;
            this.rbtnEdit.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnEdit.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnEdit.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnEdit.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(104)))), ((int)(((byte)(147)))));
            this.rbtnEdit.ForeColorsByTypeUse = true;
            this.rbtnEdit.ID = -1;
            this.rbtnEdit.InitButtonWidth = 70;
            this.rbtnEdit.IsChecked = false;
            this.rbtnEdit.Location = new System.Drawing.Point(558, 43);
            this.rbtnEdit.Margin = new System.Windows.Forms.Padding(0);
            this.rbtnEdit.MouseOverBkgndImage = global::BIMViewer.Properties.Resources.mouse_over_background;
            this.rbtnEdit.MouseOverImage = global::BIMViewer.Properties.Resources.Modify_1st_MSover;
            this.rbtnEdit.Name = "rbtnEdit";
            this.rbtnEdit.NormalImage = global::BIMViewer.Properties.Resources.Modify_base;
            this.rbtnEdit.Owner = null;
            this.rbtnEdit.Size = new System.Drawing.Size(70, 50);
            this.rbtnEdit.TabIndex = 4;
            this.rbtnEdit.Text = "Modify";
            this.rbtnEdit.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rbtnEdit.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnEdit.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbtnEdit.ToolTipText = "Modify";
            this.rbtnEdit.UseCustomImageRect = true;
            this.rbtnEdit.UseTextLocation = false;
            this.rbtnEdit.UseVisualStyleBackColor = false;
            this.rbtnEdit.Click += new System.EventHandler(this.rbtnEdit_Click);
            // 
            // rbtnLayer
            // 
            this.rbtnLayer.BackColor = System.Drawing.Color.Transparent;
            this.rbtnLayer.CheckButton = false;
            this.rbtnLayer.CheckedBkgndImage = global::BIMViewer.Properties.Resources.clicked_background;
            this.rbtnLayer.CheckedImage = global::BIMViewer.Properties.Resources.POI_Layer_MSclicked;
            this.rbtnLayer.CheckedMouseOver = global::BIMViewer.Properties.Resources.POI_Layer_2nd_MSover;
            this.rbtnLayer.ClickedBackgroundImage = null;
            this.rbtnLayer.ClickedImage = global::BIMViewer.Properties.Resources.POI_Layer_MSclicked;
            this.rbtnLayer.CustomImageRect = new System.Drawing.Rectangle(0, 0, 70, 90);
            this.rbtnLayer.DisabledBkgndImage = null;
            this.rbtnLayer.DisabledImage = null;
            this.rbtnLayer.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnLayer.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnLayer.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnLayer.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(104)))), ((int)(((byte)(147)))));
            this.rbtnLayer.ForeColorsByTypeUse = true;
            this.rbtnLayer.ID = -1;
            this.rbtnLayer.InitButtonWidth = 70;
            this.rbtnLayer.IsChecked = false;
            this.rbtnLayer.Location = new System.Drawing.Point(576, 27);
            this.rbtnLayer.Margin = new System.Windows.Forms.Padding(0);
            this.rbtnLayer.MouseOverBkgndImage = global::BIMViewer.Properties.Resources.mouse_over_background;
            this.rbtnLayer.MouseOverImage = global::BIMViewer.Properties.Resources.POI_Layer_1st_MSover;
            this.rbtnLayer.Name = "rbtnLayer";
            this.rbtnLayer.NormalImage = global::BIMViewer.Properties.Resources.POI_Layer_base;
            this.rbtnLayer.Owner = null;
            this.rbtnLayer.Size = new System.Drawing.Size(70, 50);
            this.rbtnLayer.TabIndex = 2;
            this.rbtnLayer.Text = "POI Layer";
            this.rbtnLayer.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rbtnLayer.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnLayer.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbtnLayer.ToolTipText = "POI Layer";
            this.rbtnLayer.UseCustomImageRect = true;
            this.rbtnLayer.UseTextLocation = false;
            this.rbtnLayer.UseVisualStyleBackColor = false;
            this.rbtnLayer.Click += new System.EventHandler(this.rbtnLayer_Click);
            // 
            // rbtnPOI
            // 
            this.rbtnPOI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(27)))), ((int)(((byte)(43)))));
            this.rbtnPOI.CheckButton = false;
            this.rbtnPOI.CheckedBkgndImage = global::BIMViewer.Properties.Resources.clicked_background;
            this.rbtnPOI.CheckedImage = global::BIMViewer.Properties.Resources.POI_MSclicked;
            this.rbtnPOI.CheckedMouseOver = global::BIMViewer.Properties.Resources.POI_2nd_MSover;
            this.rbtnPOI.ClickedBackgroundImage = null;
            this.rbtnPOI.ClickedImage = global::BIMViewer.Properties.Resources.POI_MSclicked;
            this.rbtnPOI.CustomImageRect = new System.Drawing.Rectangle(0, 0, 70, 90);
            this.rbtnPOI.DisabledBkgndImage = null;
            this.rbtnPOI.DisabledImage = global::BIMViewer.Properties.Resources.POI_None;
            this.rbtnPOI.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnPOI.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(36)))), ((int)(((byte)(107)))));
            this.rbtnPOI.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnPOI.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(104)))), ((int)(((byte)(147)))));
            this.rbtnPOI.ForeColorsByTypeUse = true;
            this.rbtnPOI.ID = -1;
            this.rbtnPOI.InitButtonWidth = 70;
            this.rbtnPOI.IsChecked = false;
            this.rbtnPOI.Location = new System.Drawing.Point(609, 14);
            this.rbtnPOI.Margin = new System.Windows.Forms.Padding(0);
            this.rbtnPOI.MouseOverBkgndImage = global::BIMViewer.Properties.Resources.mouse_over_background;
            this.rbtnPOI.MouseOverImage = global::BIMViewer.Properties.Resources.POI_1st_MSover;
            this.rbtnPOI.Name = "rbtnPOI";
            this.rbtnPOI.NormalImage = global::BIMViewer.Properties.Resources.POI_base;
            this.rbtnPOI.Owner = null;
            this.rbtnPOI.Size = new System.Drawing.Size(70, 57);
            this.rbtnPOI.TabIndex = 3;
            this.rbtnPOI.Text = "POI";
            this.rbtnPOI.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rbtnPOI.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnPOI.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbtnPOI.ToolTipText = "POI";
            this.rbtnPOI.UseCustomImageRect = true;
            this.rbtnPOI.UseTextLocation = false;
            this.rbtnPOI.UseVisualStyleBackColor = false;
            this.rbtnPOI.Click += new System.EventHandler(this.rbtnPOI_Click);
            // 
            // rbtnUpload
            // 
            this.rbtnUpload.BackColor = System.Drawing.Color.Transparent;
            this.rbtnUpload.CheckButton = false;
            this.rbtnUpload.CheckedBkgndImage = null;
            this.rbtnUpload.CheckedImage = null;
            this.rbtnUpload.CheckedMouseOver = null;
            this.rbtnUpload.ClickedBackgroundImage = null;
            this.rbtnUpload.ClickedImage = null;
            this.rbtnUpload.CustomImageRect = new System.Drawing.Rectangle(4, 4, 60, 60);
            this.rbtnUpload.DisabledBkgndImage = null;
            this.rbtnUpload.DisabledImage = null;
            this.rbtnUpload.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbtnUpload.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.rbtnUpload.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.rbtnUpload.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnUpload.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.rbtnUpload.ForeColorsByTypeUse = true;
            this.rbtnUpload.ID = -1;
            this.rbtnUpload.InitButtonWidth = 70;
            this.rbtnUpload.IsChecked = false;
            this.rbtnUpload.Location = new System.Drawing.Point(189, 10);
            this.rbtnUpload.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.rbtnUpload.MouseOverBkgndImage = null;
            this.rbtnUpload.MouseOverImage = null;
            this.rbtnUpload.Name = "rbtnUpload";
            this.rbtnUpload.NormalImage = null;
            this.rbtnUpload.Owner = null;
            this.rbtnUpload.Size = new System.Drawing.Size(75, 25);
            this.rbtnUpload.TabIndex = 2;
            this.rbtnUpload.Text = "UPLOAD";
            this.rbtnUpload.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnUpload.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbtnUpload.ToolTipText = "UPLOAD";
            this.rbtnUpload.UseCustomImageRect = true;
            this.rbtnUpload.UseTextLocation = false;
            this.rbtnUpload.UseVisualStyleBackColor = false;
            this.rbtnUpload.Click += new System.EventHandler(this.rbtnUpload_Click);
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.panelTop.BackgroundImage = global::BIMViewer.Properties.Resources.background;
            this.panelTop.Controls.Add(this.panelToolbar);
            this.panelTop.Controls.Add(this.panelTitle);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1190, 128);
            this.panelTop.TabIndex = 3;
            // 
            // panelTitle
            // 
            this.panelTitle.BackColor = System.Drawing.Color.Transparent;
            this.panelTitle.BackgroundImage = global::BIMViewer.Properties.Resources.background;
            this.panelTitle.Controls.Add(this.tbLayoutPanel_top);
            this.panelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTitle.Location = new System.Drawing.Point(0, 0);
            this.panelTitle.Name = "panelTitle";
            this.panelTitle.Size = new System.Drawing.Size(1190, 35);
            this.panelTitle.TabIndex = 4;
            this.panelTitle.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panelTitle_MouseDoubleClick);
            this.panelTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTitle_MouseDown);
            this.panelTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelTitle_MouseMove);
            this.panelTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelTitle_MouseUp);
            // 
            // tbLayoutPanel_top
            // 
            this.tbLayoutPanel_top.ColumnCount = 14;
            this.tbLayoutPanel_top.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.tbLayoutPanel_top.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tbLayoutPanel_top.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tbLayoutPanel_top.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tbLayoutPanel_top.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tbLayoutPanel_top.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tbLayoutPanel_top.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 73F));
            this.tbLayoutPanel_top.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tbLayoutPanel_top.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 66F));
            this.tbLayoutPanel_top.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tbLayoutPanel_top.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.tbLayoutPanel_top.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tbLayoutPanel_top.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tbLayoutPanel_top.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tbLayoutPanel_top.Controls.Add(this.rbtnSave, 2, 0);
            this.tbLayoutPanel_top.Controls.Add(this.rbtnOpen, 1, 0);
            this.tbLayoutPanel_top.Controls.Add(this.pictureBox3, 0, 0);
            this.tbLayoutPanel_top.Controls.Add(this.rbtnUpload, 3, 0);
            this.tbLayoutPanel_top.Controls.Add(this.imageButton1, 10, 0);
            this.tbLayoutPanel_top.Controls.Add(this.clblLogin, 9, 0);
            this.tbLayoutPanel_top.Controls.Add(this.colorLabel2, 8, 0);
            this.tbLayoutPanel_top.Controls.Add(this.ribbonButton1, 6, 0);
            this.tbLayoutPanel_top.Controls.Add(this.btnMin, 11, 0);
            this.tbLayoutPanel_top.Controls.Add(this.btnClose, 13, 0);
            this.tbLayoutPanel_top.Controls.Add(this.btnMax, 12, 0);
            this.tbLayoutPanel_top.Controls.Add(this.rbtnDownload, 4, 0);
            this.tbLayoutPanel_top.Controls.Add(this.rbtnFormLayer, 7, 0);
            this.tbLayoutPanel_top.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLayoutPanel_top.Location = new System.Drawing.Point(0, 0);
            this.tbLayoutPanel_top.Name = "tbLayoutPanel_top";
            this.tbLayoutPanel_top.RowCount = 1;
            this.tbLayoutPanel_top.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tbLayoutPanel_top.Size = new System.Drawing.Size(1190, 35);
            this.tbLayoutPanel_top.TabIndex = 3;
            this.tbLayoutPanel_top.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panelTitle_MouseDoubleClick);
            this.tbLayoutPanel_top.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTitle_MouseDown);
            this.tbLayoutPanel_top.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelTitle_MouseMove);
            this.tbLayoutPanel_top.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelTitle_MouseUp);
            // 
            // rbtnSave
            // 
            this.rbtnSave.BackColor = System.Drawing.Color.Transparent;
            this.rbtnSave.CheckButton = false;
            this.rbtnSave.CheckedBkgndImage = null;
            this.rbtnSave.CheckedImage = null;
            this.rbtnSave.CheckedMouseOver = null;
            this.rbtnSave.ClickedBackgroundImage = null;
            this.rbtnSave.ClickedImage = null;
            this.rbtnSave.CustomImageRect = new System.Drawing.Rectangle(4, 4, 60, 60);
            this.rbtnSave.DisabledBkgndImage = null;
            this.rbtnSave.DisabledImage = null;
            this.rbtnSave.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbtnSave.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.rbtnSave.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.rbtnSave.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnSave.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.rbtnSave.ForeColorsByTypeUse = true;
            this.rbtnSave.ID = -1;
            this.rbtnSave.InitButtonWidth = 70;
            this.rbtnSave.IsChecked = false;
            this.rbtnSave.Location = new System.Drawing.Point(114, 10);
            this.rbtnSave.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.rbtnSave.MouseOverBkgndImage = null;
            this.rbtnSave.MouseOverImage = null;
            this.rbtnSave.Name = "rbtnSave";
            this.rbtnSave.NormalImage = null;
            this.rbtnSave.Owner = null;
            this.rbtnSave.Size = new System.Drawing.Size(75, 25);
            this.rbtnSave.TabIndex = 9;
            this.rbtnSave.Text = "SAVE";
            this.rbtnSave.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnSave.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbtnSave.ToolTipText = "SAVE";
            this.rbtnSave.UseCustomImageRect = true;
            this.rbtnSave.UseTextLocation = false;
            this.rbtnSave.UseVisualStyleBackColor = false;
            this.rbtnSave.Click += new System.EventHandler(this.rbtnSave_Click);
            // 
            // rbtnOpen
            // 
            this.rbtnOpen.BackColor = System.Drawing.Color.Transparent;
            this.rbtnOpen.CheckButton = false;
            this.rbtnOpen.CheckedBkgndImage = null;
            this.rbtnOpen.CheckedImage = null;
            this.rbtnOpen.CheckedMouseOver = null;
            this.rbtnOpen.ClickedBackgroundImage = null;
            this.rbtnOpen.ClickedImage = null;
            this.rbtnOpen.CustomImageRect = new System.Drawing.Rectangle(4, 4, 60, 60);
            this.rbtnOpen.DisabledBkgndImage = null;
            this.rbtnOpen.DisabledImage = null;
            this.rbtnOpen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbtnOpen.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.rbtnOpen.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.rbtnOpen.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnOpen.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.rbtnOpen.ForeColorsByTypeUse = true;
            this.rbtnOpen.ID = -1;
            this.rbtnOpen.InitButtonWidth = 70;
            this.rbtnOpen.IsChecked = false;
            this.rbtnOpen.Location = new System.Drawing.Point(39, 10);
            this.rbtnOpen.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.rbtnOpen.MouseOverBkgndImage = null;
            this.rbtnOpen.MouseOverImage = null;
            this.rbtnOpen.Name = "rbtnOpen";
            this.rbtnOpen.NormalImage = null;
            this.rbtnOpen.Owner = null;
            this.rbtnOpen.Size = new System.Drawing.Size(75, 25);
            this.rbtnOpen.TabIndex = 8;
            this.rbtnOpen.Text = "OPEN";
            this.rbtnOpen.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnOpen.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbtnOpen.ToolTipText = "OPEN";
            this.rbtnOpen.UseCustomImageRect = true;
            this.rbtnOpen.UseTextLocation = false;
            this.rbtnOpen.UseVisualStyleBackColor = false;
            this.rbtnOpen.Click += new System.EventHandler(this.rbtnOpen_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox3.Image = global::BIMViewer.Properties.Resources.공간정보_변환_로고;
            this.pictureBox3.Location = new System.Drawing.Point(3, 3);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(33, 29);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 1;
            this.pictureBox3.TabStop = false;
            // 
            // imageButton1
            // 
            this.imageButton1.ButtonText = "";
            this.imageButton1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.imageButton1.ImageClicked = global::BIMViewer.Properties.Resources.Setting_MSclicked;
            this.imageButton1.ImageDisabled = null;
            this.imageButton1.ImageMouseOver = global::BIMViewer.Properties.Resources.Setting_MSover;
            this.imageButton1.ImageNormal = global::BIMViewer.Properties.Resources.Setting_base;
            this.imageButton1.Location = new System.Drawing.Point(1054, 0);
            this.imageButton1.Margin = new System.Windows.Forms.Padding(0);
            this.imageButton1.Name = "imageButton1";
            this.imageButton1.Owner = null;
            this.imageButton1.Size = new System.Drawing.Size(43, 35);
            this.imageButton1.TabIndex = 2;
            this.imageButton1.TabStop = false;
            this.imageButton1.TextColor = System.Drawing.Color.Black;
            this.imageButton1.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.imageButton1.ToolTipText = "";
            this.imageButton1.UseToolTip = false;
            this.imageButton1.WindowRateWidth = 1F;
            // 
            // clblLogin
            // 
            this.clblLogin.AutoSize = true;
            this.clblLogin.ColorClicked = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.clblLogin.ColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.clblLogin.ColorNomal = System.Drawing.Color.White;
            this.clblLogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clblLogin.ForeColor = System.Drawing.Color.White;
            this.clblLogin.Location = new System.Drawing.Point(982, 7);
            this.clblLogin.Margin = new System.Windows.Forms.Padding(3, 7, 3, 0);
            this.clblLogin.Name = "clblLogin";
            this.clblLogin.Size = new System.Drawing.Size(51, 17);
            this.clblLogin.TabIndex = 5;
            this.clblLogin.Text = "LOGIN";
            this.clblLogin.Click += new System.EventHandler(this.ClblLogin_Click);
            // 
            // colorLabel2
            // 
            this.colorLabel2.AutoSize = true;
            this.colorLabel2.ColorClicked = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.colorLabel2.ColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.colorLabel2.ColorNomal = System.Drawing.Color.White;
            this.colorLabel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.colorLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colorLabel2.ForeColor = System.Drawing.Color.White;
            this.colorLabel2.Location = new System.Drawing.Point(916, 9);
            this.colorLabel2.Margin = new System.Windows.Forms.Padding(3, 9, 3, 0);
            this.colorLabel2.Name = "colorLabel2";
            this.colorLabel2.Size = new System.Drawing.Size(60, 26);
            this.colorLabel2.TabIndex = 6;
            this.colorLabel2.Text = "유엔이";
            // 
            // ribbonButton1
            // 
            this.ribbonButton1.BackColor = System.Drawing.Color.Transparent;
            this.ribbonButton1.CheckButton = false;
            this.ribbonButton1.CheckedBkgndImage = global::BIMViewer.Properties.Resources.clicked_background;
            this.ribbonButton1.CheckedImage = null;
            this.ribbonButton1.CheckedMouseOver = null;
            this.ribbonButton1.ClickedBackgroundImage = null;
            this.ribbonButton1.ClickedImage = null;
            this.ribbonButton1.CustomImageRect = new System.Drawing.Rectangle(4, 4, 60, 60);
            this.ribbonButton1.DisabledBkgndImage = null;
            this.ribbonButton1.DisabledImage = null;
            this.ribbonButton1.ForeColorChecked = System.Drawing.Color.White;
            this.ribbonButton1.ForeColorCheckedMouseOver = System.Drawing.Color.Black;
            this.ribbonButton1.ForeColorDisabled = System.Drawing.Color.White;
            this.ribbonButton1.ForeColorMouseOver = System.Drawing.Color.White;
            this.ribbonButton1.ForeColorsByTypeUse = false;
            this.ribbonButton1.ID = -1;
            this.ribbonButton1.InitButtonWidth = 70;
            this.ribbonButton1.IsChecked = false;
            this.ribbonButton1.Location = new System.Drawing.Point(773, 3);
            this.ribbonButton1.MouseOverBkgndImage = global::BIMViewer.Properties.Resources.mouse_over_background;
            this.ribbonButton1.MouseOverImage = null;
            this.ribbonButton1.Name = "ribbonButton1";
            this.ribbonButton1.NormalImage = null;
            this.ribbonButton1.Owner = null;
            this.ribbonButton1.Size = new System.Drawing.Size(67, 25);
            this.ribbonButton1.TabIndex = 7;
            this.ribbonButton1.Text = "FormPOI";
            this.ribbonButton1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.ribbonButton1.TextLocation = new System.Drawing.Point(0, 0);
            this.ribbonButton1.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.ribbonButton1.ToolTipText = "FormPOI";
            this.ribbonButton1.UseCustomImageRect = true;
            this.ribbonButton1.UseTextLocation = false;
            this.ribbonButton1.UseVisualStyleBackColor = false;
            this.ribbonButton1.Visible = false;
            this.ribbonButton1.Click += new System.EventHandler(this.ribbonButton1_Click);
            // 
            // btnMin
            // 
            this.btnMin.ButtonText = "";
            this.btnMin.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnMin.Image = global::BIMViewer.Properties.Resources.Windowminimized__Base;
            this.btnMin.ImageClicked = global::BIMViewer.Properties.Resources.Windowminimized_1st_MSover;
            this.btnMin.ImageDisabled = null;
            this.btnMin.ImageMouseOver = global::BIMViewer.Properties.Resources.Windowminimized_1st_MSover;
            this.btnMin.ImageNormal = global::BIMViewer.Properties.Resources.Windowminimized__Base;
            this.btnMin.Location = new System.Drawing.Point(1103, 3);
            this.btnMin.Name = "btnMin";
            this.btnMin.Owner = null;
            this.btnMin.Size = new System.Drawing.Size(23, 23);
            this.btnMin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnMin.TabIndex = 11;
            this.btnMin.TabStop = false;
            this.btnMin.TextColor = System.Drawing.Color.Black;
            this.btnMin.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnMin.ToolTipText = "";
            this.btnMin.UseToolTip = false;
            this.btnMin.WindowRateWidth = 1F;
            this.btnMin.Click += new System.EventHandler(this.btnMin_Click);
            // 
            // btnClose
            // 
            this.btnClose.ButtonText = "";
            this.btnClose.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.Image = global::BIMViewer.Properties.Resources.Windowclose__Base;
            this.btnClose.ImageClicked = global::BIMViewer.Properties.Resources.Windowclose_1st_MSover;
            this.btnClose.ImageDisabled = null;
            this.btnClose.ImageMouseOver = global::BIMViewer.Properties.Resources.Windowclose_1st_MSover;
            this.btnClose.ImageNormal = global::BIMViewer.Properties.Resources.Windowclose__Base;
            this.btnClose.Location = new System.Drawing.Point(1163, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Owner = null;
            this.btnClose.Size = new System.Drawing.Size(23, 23);
            this.btnClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnClose.TabIndex = 12;
            this.btnClose.TabStop = false;
            this.btnClose.TextColor = System.Drawing.Color.Black;
            this.btnClose.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnClose.ToolTipText = "";
            this.btnClose.UseToolTip = false;
            this.btnClose.WindowRateWidth = 1F;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnMax
            // 
            this.btnMax.ButtonText = "";
            this.btnMax.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnMax.Image = global::BIMViewer.Properties.Resources.Windowmulti_Base;
            this.btnMax.ImageClicked = global::BIMViewer.Properties.Resources.Windowmulti_1st_MSover;
            this.btnMax.ImageDisabled = null;
            this.btnMax.ImageMouseOver = global::BIMViewer.Properties.Resources.Windowmulti_1st_MSover;
            this.btnMax.ImageNormal = global::BIMViewer.Properties.Resources.Windowmulti_Base;
            this.btnMax.Location = new System.Drawing.Point(1133, 3);
            this.btnMax.Name = "btnMax";
            this.btnMax.Owner = null;
            this.btnMax.Size = new System.Drawing.Size(23, 23);
            this.btnMax.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnMax.TabIndex = 13;
            this.btnMax.TabStop = false;
            this.btnMax.TextColor = System.Drawing.Color.Black;
            this.btnMax.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnMax.ToolTipText = "";
            this.btnMax.UseToolTip = false;
            this.btnMax.WindowRateWidth = 1F;
            this.btnMax.Click += new System.EventHandler(this.btnMax_Click);
            // 
            // rbtnDownload
            // 
            this.rbtnDownload.CheckButton = false;
            this.rbtnDownload.CheckedBkgndImage = null;
            this.rbtnDownload.CheckedImage = null;
            this.rbtnDownload.CheckedMouseOver = null;
            this.rbtnDownload.ClickedBackgroundImage = null;
            this.rbtnDownload.ClickedImage = null;
            this.rbtnDownload.CustomImageRect = new System.Drawing.Rectangle(0, 0, 60, 60);
            this.rbtnDownload.DisabledBkgndImage = null;
            this.rbtnDownload.DisabledImage = null;
            this.rbtnDownload.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbtnDownload.ForeColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.rbtnDownload.ForeColorCheckedMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.rbtnDownload.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnDownload.ForeColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.rbtnDownload.ForeColorsByTypeUse = true;
            this.rbtnDownload.ID = -1;
            this.rbtnDownload.InitButtonWidth = 60;
            this.rbtnDownload.IsChecked = false;
            this.rbtnDownload.Location = new System.Drawing.Point(264, 10);
            this.rbtnDownload.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.rbtnDownload.MouseOverBkgndImage = null;
            this.rbtnDownload.MouseOverImage = null;
            this.rbtnDownload.Name = "rbtnDownload";
            this.rbtnDownload.NormalImage = null;
            this.rbtnDownload.Owner = null;
            this.rbtnDownload.Size = new System.Drawing.Size(100, 25);
            this.rbtnDownload.TabIndex = 14;
            this.rbtnDownload.Text = "DOWNLOAD";
            this.rbtnDownload.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnDownload.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbtnDownload.ToolTipText = "DOWNLOAD";
            this.rbtnDownload.UseCustomImageRect = true;
            this.rbtnDownload.UseTextLocation = false;
            this.rbtnDownload.UseVisualStyleBackColor = true;
            this.rbtnDownload.Click += new System.EventHandler(this.rbtnDownload_Click);
            // 
            // rbtnFormLayer
            // 
            this.rbtnFormLayer.BackColor = System.Drawing.Color.Transparent;
            this.rbtnFormLayer.CheckButton = false;
            this.rbtnFormLayer.CheckedBkgndImage = global::BIMViewer.Properties.Resources.clicked_background;
            this.rbtnFormLayer.CheckedImage = null;
            this.rbtnFormLayer.CheckedMouseOver = null;
            this.rbtnFormLayer.ClickedBackgroundImage = null;
            this.rbtnFormLayer.ClickedImage = null;
            this.rbtnFormLayer.CustomImageRect = new System.Drawing.Rectangle(4, 4, 60, 60);
            this.rbtnFormLayer.DisabledBkgndImage = null;
            this.rbtnFormLayer.DisabledImage = null;
            this.rbtnFormLayer.ForeColorChecked = System.Drawing.Color.White;
            this.rbtnFormLayer.ForeColorCheckedMouseOver = System.Drawing.Color.Black;
            this.rbtnFormLayer.ForeColorDisabled = System.Drawing.Color.White;
            this.rbtnFormLayer.ForeColorMouseOver = System.Drawing.Color.White;
            this.rbtnFormLayer.ForeColorsByTypeUse = false;
            this.rbtnFormLayer.ID = -1;
            this.rbtnFormLayer.InitButtonWidth = 70;
            this.rbtnFormLayer.IsChecked = false;
            this.rbtnFormLayer.Location = new System.Drawing.Point(846, 3);
            this.rbtnFormLayer.MouseOverBkgndImage = global::BIMViewer.Properties.Resources.mouse_over_background;
            this.rbtnFormLayer.MouseOverImage = null;
            this.rbtnFormLayer.Name = "rbtnFormLayer";
            this.rbtnFormLayer.NormalImage = null;
            this.rbtnFormLayer.Owner = null;
            this.rbtnFormLayer.Size = new System.Drawing.Size(64, 29);
            this.rbtnFormLayer.TabIndex = 10;
            this.rbtnFormLayer.Text = "LAYER";
            this.rbtnFormLayer.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rbtnFormLayer.TextLocation = new System.Drawing.Point(0, 0);
            this.rbtnFormLayer.TextPos = UnE.GUI.RibbonButton.TextPosition.NONE;
            this.rbtnFormLayer.ToolTipText = "LAYER";
            this.rbtnFormLayer.UseCustomImageRect = true;
            this.rbtnFormLayer.UseTextLocation = false;
            this.rbtnFormLayer.UseVisualStyleBackColor = false;
            this.rbtnFormLayer.Click += new System.EventHandler(this.rbtnFormLayer_Click);
            // 
            // splitContainerLeft
            // 
            this.splitContainerLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerLeft.Location = new System.Drawing.Point(0, 0);
            this.splitContainerLeft.Name = "splitContainerLeft";
            this.splitContainerLeft.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerLeft.Panel1
            // 
            this.splitContainerLeft.Panel1.BackColor = System.Drawing.SystemColors.ButtonFace;
            // 
            // splitContainerLeft.Panel2
            // 
            this.splitContainerLeft.Panel2.Controls.Add(this.splitContainerLeft2);
            this.splitContainerLeft.Size = new System.Drawing.Size(276, 426);
            this.splitContainerLeft.SplitterDistance = 92;
            this.splitContainerLeft.SplitterWidth = 1;
            this.splitContainerLeft.TabIndex = 0;
            // 
            // splitContainerLeft2
            // 
            this.splitContainerLeft2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.splitContainerLeft2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerLeft2.Location = new System.Drawing.Point(0, 0);
            this.splitContainerLeft2.Name = "splitContainerLeft2";
            this.splitContainerLeft2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainerLeft2.Size = new System.Drawing.Size(276, 333);
            this.splitContainerLeft2.SplitterDistance = 101;
            this.splitContainerLeft2.SplitterWidth = 1;
            this.splitContainerLeft2.TabIndex = 0;
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 128);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.splitContainerMain.Panel1.Controls.Add(this.splitContainerLeft);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.BackColor = System.Drawing.Color.Black;
            this.splitContainerMain.Panel2.Controls.Add(this.tabControl1);
            this.splitContainerMain.Size = new System.Drawing.Size(1190, 426);
            this.splitContainerMain.SplitterDistance = 276;
            this.splitContainerMain.SplitterWidth = 1;
            this.splitContainerMain.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(913, 426);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.Visible = false;
            this.tabControl1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControl1_DrawItem);
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            this.tabControl1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tabControl1_MouseClick);
            this.tabControl1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.tabControl1_MouseDoubleClick);
            this.tabControl1.MouseLeave += new System.EventHandler(this.tabControl1_MouseLeave);
            this.tabControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tabControl1_MouseMove);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImage = global::BIMViewer.Properties.Resources.background;
            this.panel1.Controls.Add(this.splitContainerMain);
            this.panel1.Controls.Add(this.panelTop);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1190, 554);
            this.panel1.TabIndex = 8;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::BIMViewer.Properties.Resources.background;
            this.ClientSize = new System.Drawing.Size(1196, 557);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Name = "FormMain";
            this.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.Text = "InsafetyML Manager";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResizeBegin += new System.EventHandler(this.FormMain_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.FormMain_ResizeEnd);
            this.ClientSizeChanged += new System.EventHandler(this.FormMain_ClientSizeChanged);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            this.panelToolbar.ResumeLayout(false);
            this.panelProperty.ResumeLayout(false);
            this.panelBuilding.ResumeLayout(false);
            this.panelLine.ResumeLayout(false);
            this.panelLine.PerformLayout();
            this.panelPOI.ResumeLayout(false);
            this.panelPOI.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.panelTitle.ResumeLayout(false);
            this.tbLayoutPanel_top.ResumeLayout(false);
            this.tbLayoutPanel_top.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageButton1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMax)).EndInit();
            this.splitContainerLeft.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLeft)).EndInit();
            this.splitContainerLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLeft2)).EndInit();
            this.splitContainerLeft2.ResumeLayout(false);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelToolbar;
        private UnE.GUI.RibbonButton rbtnLayer;
        private UnE.GUI.RibbonButton rbtnUpload;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelTitle;
        private System.Windows.Forms.SplitContainer splitContainerLeft;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox3;
        private UnE.GUI.ImageButton imageButton1;
        private System.Windows.Forms.TableLayoutPanel tbLayoutPanel_top;
        private UnE.Controls.ColorLabel clblLogin;
        private UnE.Controls.ColorLabel colorLabel2;
        private UnE.GUI.RibbonButton rbtnPOI;
        private UnE.GUI.RibbonButton rbtnEdit;
        private UnE.GUI.RibbonButton rbtnDone;
        private UnE.GUI.RibbonButton rbtnDelete;
        private UnE.GUI.RibbonButton rbtnMove;
        private UnE.GUI.RibbonButton rbtnLine;
        private System.Windows.Forms.Panel panelPOI;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panelLine;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private UnE.GUI.RibbonButton rbtnDoneLine;
        private UnE.GUI.RibbonButton rbtnDeleteLine;
        private UnE.GUI.RibbonButton rbtnMoveLine;
        private UnE.GUI.RibbonButton ribbonButton1;
        private System.Windows.Forms.ComboBox cbPOIList;
        private UnE.GUI.RibbonButton rbtnSave;
        private UnE.GUI.RibbonButton rbtnOpen;
        private System.Windows.Forms.ComboBox cbLineList;
        private UnE.GUI.RibbonButton rbtnAdd;
        private UnE.GUI.RibbonButton rbtnAddLine;
        private UnE.GUI.RibbonButton rbtnProperty;
        private System.Windows.Forms.SplitContainer splitContainerLeft2;
        private UnE.GUI.RibbonButton rbtnFormLayer;
        private UnE.GUI.ImageButton btnMin;
        private UnE.GUI.ImageButton btnClose;
        private UnE.GUI.ImageButton btnMax;
        private System.Windows.Forms.TabControl tabControl1;
        private UnE.GUI.RibbonButton rbtnDownload;
        private System.Windows.Forms.Panel pnlProperty;
        private UnE.GUI.RibbonButton rbtnPropertyDone;
        private System.Windows.Forms.Panel pnlSave;
        private UnE.GUI.RibbonButton rbtnBuilding;
        private System.Windows.Forms.Panel panelBuilding;
        private System.Windows.Forms.Panel panelProperty;
        private System.Windows.Forms.Panel pnlBuilding;
        private UnE.GUI.RibbonButton rbtnBuildingDone;
    }
}

