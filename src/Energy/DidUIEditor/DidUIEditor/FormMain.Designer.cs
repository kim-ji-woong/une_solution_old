namespace DidUIEditor
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
            AnimatorNS.Animation animation4 = new AnimatorNS.Animation();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.btnSave = new UnE.GUI.ImageButton();
            this.btnEmergency = new UnE.GUI.ImageButton();
            this.btnNormal = new UnE.GUI.ImageButton();
            this.pnMain = new System.Windows.Forms.Panel();
            this.txtPageTime = new System.Windows.Forms.TextBox();
            this.tabControl1 = new AnimatorNS.TabControlEx();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.pnUI = new System.Windows.Forms.Panel();
            this.btnPageSetting = new UnE.GUI.ImageButton();
            this.btnNewPage = new UnE.GUI.ImageButton();
            this.btnAddMedia = new UnE.GUI.ImageButton();
            this.label1 = new System.Windows.Forms.Label();
            this.btnTopSort = new UnE.GUI.ImageButton();
            this.label2 = new System.Windows.Forms.Label();
            this.btnLeftSort = new UnE.GUI.ImageButton();
            this.pnTop = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.btnSave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEmergency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNormal)).BeginInit();
            this.pnMain.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnPageSetting)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAddMedia)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnTopSort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLeftSort)).BeginInit();
            this.pnTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.ButtonText = "";
            this.btnSave.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSave.ImageClicked = global::DidUIEditor.Properties.Resources.btnSave_Click;
            this.btnSave.ImageDisabled = null;
            this.btnSave.ImageMouseOver = global::DidUIEditor.Properties.Resources.btnSave_Click;
            this.btnSave.ImageNormal = global::DidUIEditor.Properties.Resources.btnSave_Default;
            this.btnSave.Location = new System.Drawing.Point(884, 43);
            this.btnSave.Name = "btnSave";
            this.btnSave.Owner = null;
            this.btnSave.Size = new System.Drawing.Size(100, 32);
            this.btnSave.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnSave.TabIndex = 13;
            this.btnSave.TabStop = false;
            this.btnSave.TextColor = System.Drawing.Color.Black;
            this.btnSave.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSave.ToolTipText = "";
            this.btnSave.UseToolTip = false;
            this.btnSave.WindowRateWidth = 1F;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnEmergency
            // 
            this.btnEmergency.BackColor = System.Drawing.Color.Transparent;
            this.btnEmergency.ButtonText = "";
            this.btnEmergency.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnEmergency.ImageClicked = global::DidUIEditor.Properties.Resources.btnEmergency_Click;
            this.btnEmergency.ImageDisabled = null;
            this.btnEmergency.ImageMouseOver = global::DidUIEditor.Properties.Resources.btnEmergency_Click;
            this.btnEmergency.ImageNormal = global::DidUIEditor.Properties.Resources.btnEmergency_Default;
            this.btnEmergency.Location = new System.Drawing.Point(136, 43);
            this.btnEmergency.Name = "btnEmergency";
            this.btnEmergency.Owner = null;
            this.btnEmergency.Size = new System.Drawing.Size(120, 32);
            this.btnEmergency.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnEmergency.TabIndex = 14;
            this.btnEmergency.TabStop = false;
            this.btnEmergency.TextColor = System.Drawing.Color.Black;
            this.btnEmergency.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnEmergency.ToolTipText = "";
            this.btnEmergency.UseToolTip = false;
            this.btnEmergency.Visible = false;
            this.btnEmergency.WindowRateWidth = 1F;
            this.btnEmergency.Click += new System.EventHandler(this.btnEmergency_Click);
            // 
            // btnNormal
            // 
            this.btnNormal.BackColor = System.Drawing.Color.Transparent;
            this.btnNormal.ButtonText = "";
            this.btnNormal.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnNormal.ImageClicked = global::DidUIEditor.Properties.Resources.btnNormal_Click;
            this.btnNormal.ImageDisabled = null;
            this.btnNormal.ImageMouseOver = global::DidUIEditor.Properties.Resources.btnNormal_Click;
            this.btnNormal.ImageNormal = global::DidUIEditor.Properties.Resources.btnNormal_Click;
            this.btnNormal.Location = new System.Drawing.Point(10, 43);
            this.btnNormal.Name = "btnNormal";
            this.btnNormal.Owner = null;
            this.btnNormal.Size = new System.Drawing.Size(120, 32);
            this.btnNormal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnNormal.TabIndex = 15;
            this.btnNormal.TabStop = false;
            this.btnNormal.TextColor = System.Drawing.Color.Black;
            this.btnNormal.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnNormal.ToolTipText = "";
            this.btnNormal.UseToolTip = false;
            this.btnNormal.WindowRateWidth = 1F;
            this.btnNormal.Click += new System.EventHandler(this.btnNormal_Click);
            // 
            // pnMain
            // 
            this.pnMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(50)))), ((int)(((byte)(69)))));
            this.pnMain.Controls.Add(this.txtPageTime);
            this.pnMain.Controls.Add(this.tabControl1);
            this.pnMain.Controls.Add(this.btnPageSetting);
            this.pnMain.Controls.Add(this.btnNewPage);
            this.pnMain.Controls.Add(this.btnAddMedia);
            this.pnMain.Controls.Add(this.label1);
            this.pnMain.Controls.Add(this.btnTopSort);
            this.pnMain.Controls.Add(this.label2);
            this.pnMain.Controls.Add(this.btnLeftSort);
            this.pnMain.Location = new System.Drawing.Point(0, 106);
            this.pnMain.Name = "pnMain";
            this.pnMain.Size = new System.Drawing.Size(994, 625);
            this.pnMain.TabIndex = 3;
            // 
            // txtPageTime
            // 
            this.txtPageTime.Location = new System.Drawing.Point(666, 16);
            this.txtPageTime.Name = "txtPageTime";
            this.txtPageTime.Size = new System.Drawing.Size(84, 21);
            this.txtPageTime.TabIndex = 19;
            // 
            // tabControl1
            // 
            animation4.AnimateOnlyDifferences = false;
            animation4.BlindCoeff = ((System.Drawing.PointF)(resources.GetObject("animation4.BlindCoeff")));
            animation4.LeafCoeff = 0F;
            animation4.MaxTime = 1F;
            animation4.MinTime = 0F;
            animation4.MosaicCoeff = ((System.Drawing.PointF)(resources.GetObject("animation4.MosaicCoeff")));
            animation4.MosaicShift = ((System.Drawing.PointF)(resources.GetObject("animation4.MosaicShift")));
            animation4.MosaicSize = 0;
            animation4.Padding = new System.Windows.Forms.Padding(0);
            animation4.RotateCoeff = 0F;
            animation4.RotateLimit = 0F;
            animation4.ScaleCoeff = ((System.Drawing.PointF)(resources.GetObject("animation4.ScaleCoeff")));
            animation4.SlideCoeff = ((System.Drawing.PointF)(resources.GetObject("animation4.SlideCoeff")));
            animation4.TimeCoeff = 1F;
            animation4.TransparencyCoeff = 0F;
            this.tabControl1.Animation = animation4;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(10, 49);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(972, 573);
            this.tabControl1.TabIndex = 18;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.pnUI);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(964, 547);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // pnUI
            // 
            this.pnUI.BackColor = System.Drawing.Color.White;
            this.pnUI.Location = new System.Drawing.Point(2, 4);
            this.pnUI.Name = "pnUI";
            this.pnUI.Size = new System.Drawing.Size(960, 540);
            this.pnUI.TabIndex = 1;
            // 
            // btnPageSetting
            // 
            this.btnPageSetting.BackColor = System.Drawing.Color.Transparent;
            this.btnPageSetting.ButtonText = "";
            this.btnPageSetting.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnPageSetting.ImageClicked = global::DidUIEditor.Properties.Resources.btnPageSet_Click;
            this.btnPageSetting.ImageDisabled = null;
            this.btnPageSetting.ImageMouseOver = global::DidUIEditor.Properties.Resources.btnPageSet_Click;
            this.btnPageSetting.ImageNormal = global::DidUIEditor.Properties.Resources.btnPageSet_Default;
            this.btnPageSetting.Location = new System.Drawing.Point(884, 12);
            this.btnPageSetting.Name = "btnPageSetting";
            this.btnPageSetting.Owner = null;
            this.btnPageSetting.Size = new System.Drawing.Size(100, 30);
            this.btnPageSetting.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnPageSetting.TabIndex = 17;
            this.btnPageSetting.TabStop = false;
            this.btnPageSetting.TextColor = System.Drawing.Color.Black;
            this.btnPageSetting.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnPageSetting.ToolTipText = "";
            this.btnPageSetting.UseToolTip = false;
            this.btnPageSetting.WindowRateWidth = 1F;
            this.btnPageSetting.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnNewPage
            // 
            this.btnNewPage.BackColor = System.Drawing.Color.Transparent;
            this.btnNewPage.ButtonText = "";
            this.btnNewPage.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnNewPage.ImageClicked = global::DidUIEditor.Properties.Resources.btnNewPage_Click;
            this.btnNewPage.ImageDisabled = null;
            this.btnNewPage.ImageMouseOver = global::DidUIEditor.Properties.Resources.btnNewPage_Click;
            this.btnNewPage.ImageNormal = global::DidUIEditor.Properties.Resources.btnNewPage_Default;
            this.btnNewPage.Location = new System.Drawing.Point(778, 12);
            this.btnNewPage.Name = "btnNewPage";
            this.btnNewPage.Owner = null;
            this.btnNewPage.Size = new System.Drawing.Size(100, 30);
            this.btnNewPage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnNewPage.TabIndex = 16;
            this.btnNewPage.TabStop = false;
            this.btnNewPage.TextColor = System.Drawing.Color.Black;
            this.btnNewPage.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnNewPage.ToolTipText = "";
            this.btnNewPage.UseToolTip = false;
            this.btnNewPage.WindowRateWidth = 1F;
            this.btnNewPage.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnAddMedia
            // 
            this.btnAddMedia.ButtonText = "";
            this.btnAddMedia.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnAddMedia.ImageClicked = global::DidUIEditor.Properties.Resources.btnAddMedia_Click;
            this.btnAddMedia.ImageDisabled = null;
            this.btnAddMedia.ImageMouseOver = global::DidUIEditor.Properties.Resources.btnAddMedia_Click;
            this.btnAddMedia.ImageNormal = global::DidUIEditor.Properties.Resources.btnAddMedia_Default;
            this.btnAddMedia.Location = new System.Drawing.Point(10, 12);
            this.btnAddMedia.Name = "btnAddMedia";
            this.btnAddMedia.Owner = null;
            this.btnAddMedia.Size = new System.Drawing.Size(70, 30);
            this.btnAddMedia.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnAddMedia.TabIndex = 12;
            this.btnAddMedia.TabStop = false;
            this.btnAddMedia.TextColor = System.Drawing.Color.Black;
            this.btnAddMedia.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnAddMedia.ToolTipText = "";
            this.btnAddMedia.UseToolTip = false;
            this.btnAddMedia.WindowRateWidth = 1F;
            this.btnAddMedia.Click += new System.EventHandler(this.btnAddMedia_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("나눔바른고딕", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(597, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "유지시간 : ";
            // 
            // btnTopSort
            // 
            this.btnTopSort.ButtonText = "";
            this.btnTopSort.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnTopSort.ImageClicked = global::DidUIEditor.Properties.Resources.btnTopOrder_Click;
            this.btnTopSort.ImageDisabled = null;
            this.btnTopSort.ImageMouseOver = global::DidUIEditor.Properties.Resources.btnTopOrder_Click;
            this.btnTopSort.ImageNormal = global::DidUIEditor.Properties.Resources.btnTopOrder_Default;
            this.btnTopSort.Location = new System.Drawing.Point(132, 12);
            this.btnTopSort.Name = "btnTopSort";
            this.btnTopSort.Owner = null;
            this.btnTopSort.Size = new System.Drawing.Size(40, 30);
            this.btnTopSort.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnTopSort.TabIndex = 11;
            this.btnTopSort.TabStop = false;
            this.btnTopSort.TextColor = System.Drawing.Color.Black;
            this.btnTopSort.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnTopSort.ToolTipText = "";
            this.btnTopSort.UseToolTip = false;
            this.btnTopSort.WindowRateWidth = 1F;
            this.btnTopSort.Click += new System.EventHandler(this.btnTopSort_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("나눔바른고딕", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(751, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "초";
            // 
            // btnLeftSort
            // 
            this.btnLeftSort.ButtonText = "";
            this.btnLeftSort.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnLeftSort.ImageClicked = global::DidUIEditor.Properties.Resources.btnLeftOrder_Click;
            this.btnLeftSort.ImageDisabled = null;
            this.btnLeftSort.ImageMouseOver = global::DidUIEditor.Properties.Resources.btnLeftOrder_Click;
            this.btnLeftSort.ImageNormal = global::DidUIEditor.Properties.Resources.btnLeftOrder_Default;
            this.btnLeftSort.Location = new System.Drawing.Point(86, 12);
            this.btnLeftSort.Name = "btnLeftSort";
            this.btnLeftSort.Owner = null;
            this.btnLeftSort.Size = new System.Drawing.Size(40, 30);
            this.btnLeftSort.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnLeftSort.TabIndex = 10;
            this.btnLeftSort.TabStop = false;
            this.btnLeftSort.TextColor = System.Drawing.Color.Black;
            this.btnLeftSort.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnLeftSort.ToolTipText = "";
            this.btnLeftSort.UseToolTip = false;
            this.btnLeftSort.WindowRateWidth = 1F;
            this.btnLeftSort.Click += new System.EventHandler(this.btnLeftSort_Click);
            // 
            // pnTop
            // 
            this.pnTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(218)))), ((int)(((byte)(218)))));
            this.pnTop.Controls.Add(this.label3);
            this.pnTop.Controls.Add(this.btnClose);
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(994, 30);
            this.pnTop.TabIndex = 16;
            this.pnTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnTop_MouseDown);
            this.pnTop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnTop_MouseMove);
            this.pnTop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnTop_MouseUp);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(9, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 19);
            this.label3.TabIndex = 1;
            this.label3.Text = "DID 편집기";
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Image = global::DidUIEditor.Properties.Resources.close;
            this.btnClose.Location = new System.Drawing.Point(970, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(14, 14);
            this.btnClose.TabIndex = 0;
            this.btnClose.TabStop = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::DidUIEditor.Properties.Resources.bg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(994, 741);
            this.Controls.Add(this.pnTop);
            this.Controls.Add(this.btnNormal);
            this.Controls.Add(this.btnEmergency);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.pnMain);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "FormMain";
            this.Text = "DID 편집";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormMain_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormMain_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.btnSave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEmergency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNormal)).EndInit();
            this.pnMain.ResumeLayout(false);
            this.pnMain.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnPageSetting)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNewPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAddMedia)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnTopSort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLeftSort)).EndInit();
            this.pnTop.ResumeLayout(false);
            this.pnTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private UnE.GUI.ImageButton btnSave;
        private UnE.GUI.ImageButton btnEmergency;
        private UnE.GUI.ImageButton btnNormal;
        private System.Windows.Forms.Panel pnMain;
        private UnE.GUI.ImageButton btnAddMedia;
        private UnE.GUI.ImageButton btnTopSort;
        private UnE.GUI.ImageButton btnLeftSort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private UnE.GUI.ImageButton btnNewPage;
        private UnE.GUI.ImageButton btnPageSetting;
        private System.Windows.Forms.Panel pnTop;
        private System.Windows.Forms.PictureBox btnClose;
        private System.Windows.Forms.Label label3;
        private AnimatorNS.TabControlEx tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Panel pnUI;
        private System.Windows.Forms.TextBox txtPageTime;
    }
}

