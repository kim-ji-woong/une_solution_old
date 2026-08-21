namespace SOPManager
{
	partial class FormToolBox
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
            this.components = new System.ComponentModel.Container();
            this.btnZoomIn = new UnE.GUI.ImageButton();
            this.btnZoomOut = new UnE.GUI.ImageButton();
            this.btnFitView = new UnE.GUI.ImageButton();
            this.btnResetView = new UnE.GUI.ImageButton();
            this.btnScreenShot = new UnE.GUI.ImageButton();
            this.btnPrint = new UnE.GUI.ImageButton();
            this.btnSaveHome = new UnE.GUI.ImageButton();
            this.btnHomeView = new UnE.GUI.ImageButton();
            this.tmrUpdateCmd = new System.Windows.Forms.Timer(this.components);
            this.btnSpaceX = new UnE.GUI.ImageButton();
            this.btnSpaceY = new UnE.GUI.ImageButton();
            this.btnArrangeY = new UnE.GUI.ImageButton();
            this.btnArrangeX = new UnE.GUI.ImageButton();
            this.btnMiddleY = new UnE.GUI.ImageButton();
            this.btnMiddleX = new UnE.GUI.ImageButton();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.btnZoomIn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnZoomOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFitView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnResetView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnScreenShot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPrint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSaveHome)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnHomeView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSpaceX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSpaceY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnArrangeY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnArrangeX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMiddleY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMiddleX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            this.SuspendLayout();
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.ButtonText = "";
            this.btnZoomIn.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnZoomIn.ImageClicked = global::SOPManager.Properties.Resources._36_ZoomIn_disable;
            this.btnZoomIn.ImageDisabled = global::SOPManager.Properties.Resources._36_ZoomIn_disable;
            this.btnZoomIn.ImageMouseOver = global::SOPManager.Properties.Resources._36_ZoomIn_Checked;
            this.btnZoomIn.ImageNormal = global::SOPManager.Properties.Resources._36_ZoomIn_normal;
            this.btnZoomIn.Location = new System.Drawing.Point(8, 78);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Owner = null;
            this.btnZoomIn.Size = new System.Drawing.Size(36, 36);
            this.btnZoomIn.TabIndex = 0;
            this.btnZoomIn.TabStop = false;
            this.btnZoomIn.TextColor = System.Drawing.Color.Black;
            this.btnZoomIn.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnZoomIn.ToolTipText = "Zoom In";
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.ButtonText = "";
            this.btnZoomOut.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnZoomOut.ImageClicked = global::SOPManager.Properties.Resources._36_ZoomOut_disable;
            this.btnZoomOut.ImageDisabled = global::SOPManager.Properties.Resources._36_ZoomOut_disable;
            this.btnZoomOut.ImageMouseOver = global::SOPManager.Properties.Resources._36_ZoomOut_Checked;
            this.btnZoomOut.ImageNormal = global::SOPManager.Properties.Resources._36_ZoomOut_normal;
            this.btnZoomOut.Location = new System.Drawing.Point(8, 117);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Owner = null;
            this.btnZoomOut.Size = new System.Drawing.Size(36, 36);
            this.btnZoomOut.TabIndex = 1;
            this.btnZoomOut.TabStop = false;
            this.btnZoomOut.TextColor = System.Drawing.Color.Black;
            this.btnZoomOut.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnZoomOut.ToolTipText = "Zoom Out";
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // btnFitView
            // 
            this.btnFitView.ButtonText = "";
            this.btnFitView.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnFitView.ImageClicked = global::SOPManager.Properties.Resources._36_FitView_normal;
            this.btnFitView.ImageDisabled = global::SOPManager.Properties.Resources._36_FitView_disable;
            this.btnFitView.ImageMouseOver = global::SOPManager.Properties.Resources._36_FitView_checked;
            this.btnFitView.ImageNormal = global::SOPManager.Properties.Resources._36_FitView_normal;
            this.btnFitView.Location = new System.Drawing.Point(8, 167);
            this.btnFitView.Name = "btnFitView";
            this.btnFitView.Owner = null;
            this.btnFitView.Size = new System.Drawing.Size(36, 36);
            this.btnFitView.TabIndex = 2;
            this.btnFitView.TabStop = false;
            this.btnFitView.TextColor = System.Drawing.Color.Black;
            this.btnFitView.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnFitView.ToolTipText = "한 화면에 보기";
            this.btnFitView.Click += new System.EventHandler(this.btnFitView_Click);
            // 
            // btnResetView
            // 
            this.btnResetView.ButtonText = "";
            this.btnResetView.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnResetView.ImageClicked = global::SOPManager.Properties.Resources._36_Reset_disable;
            this.btnResetView.ImageDisabled = global::SOPManager.Properties.Resources._36_Reset_disable;
            this.btnResetView.ImageMouseOver = global::SOPManager.Properties.Resources._36_Reset_Checked;
            this.btnResetView.ImageNormal = global::SOPManager.Properties.Resources._36_Reset_normal;
            this.btnResetView.Location = new System.Drawing.Point(8, 206);
            this.btnResetView.Name = "btnResetView";
            this.btnResetView.Owner = null;
            this.btnResetView.Size = new System.Drawing.Size(36, 36);
            this.btnResetView.TabIndex = 3;
            this.btnResetView.TabStop = false;
            this.btnResetView.TextColor = System.Drawing.Color.Black;
            this.btnResetView.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnResetView.ToolTipText = "화면 리셋";
            this.btnResetView.Click += new System.EventHandler(this.btnResetView_Click);
            // 
            // btnScreenShot
            // 
            this.btnScreenShot.ButtonText = "";
            this.btnScreenShot.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnScreenShot.ImageClicked = global::SOPManager.Properties.Resources._36_ScreenShot_disable;
            this.btnScreenShot.ImageDisabled = global::SOPManager.Properties.Resources._36_ScreenShot_disable;
            this.btnScreenShot.ImageMouseOver = global::SOPManager.Properties.Resources._36_ScreenShot_Checked;
            this.btnScreenShot.ImageNormal = global::SOPManager.Properties.Resources._36_ScreenShot_normal;
            this.btnScreenShot.Location = new System.Drawing.Point(8, 333);
            this.btnScreenShot.Name = "btnScreenShot";
            this.btnScreenShot.Owner = null;
            this.btnScreenShot.Size = new System.Drawing.Size(36, 36);
            this.btnScreenShot.TabIndex = 4;
            this.btnScreenShot.TabStop = false;
            this.btnScreenShot.TextColor = System.Drawing.Color.Black;
            this.btnScreenShot.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnScreenShot.ToolTipText = "클립보드에 저장";
            this.btnScreenShot.Click += new System.EventHandler(this.btnScreenShot_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.ButtonText = "";
            this.btnPrint.Enabled = false;
            this.btnPrint.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnPrint.ImageClicked = global::SOPManager.Properties.Resources._36_Print_disable;
            this.btnPrint.ImageDisabled = global::SOPManager.Properties.Resources._36_Print_disable;
            this.btnPrint.ImageMouseOver = global::SOPManager.Properties.Resources._36_Print_Checked;
            this.btnPrint.ImageNormal = global::SOPManager.Properties.Resources._36_Print_normal;
            this.btnPrint.Location = new System.Drawing.Point(8, 372);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Owner = null;
            this.btnPrint.Size = new System.Drawing.Size(36, 36);
            this.btnPrint.TabIndex = 5;
            this.btnPrint.TabStop = false;
            this.btnPrint.TextColor = System.Drawing.Color.Black;
            this.btnPrint.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnPrint.ToolTipText = "출력하기";
            // 
            // btnSaveHome
            // 
            this.btnSaveHome.ButtonText = "";
            this.btnSaveHome.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSaveHome.ImageClicked = global::SOPManager.Properties.Resources._36_Save_disable;
            this.btnSaveHome.ImageDisabled = global::SOPManager.Properties.Resources._36_Save_disable;
            this.btnSaveHome.ImageMouseOver = global::SOPManager.Properties.Resources._36_Save_Checked;
            this.btnSaveHome.ImageNormal = global::SOPManager.Properties.Resources._36_Save_normal;
            this.btnSaveHome.Location = new System.Drawing.Point(8, 245);
            this.btnSaveHome.Name = "btnSaveHome";
            this.btnSaveHome.Owner = null;
            this.btnSaveHome.Size = new System.Drawing.Size(36, 36);
            this.btnSaveHome.TabIndex = 6;
            this.btnSaveHome.TabStop = false;
            this.btnSaveHome.TextColor = System.Drawing.Color.Black;
            this.btnSaveHome.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSaveHome.ToolTipText = "홈뷰 저장하기";
            this.btnSaveHome.Click += new System.EventHandler(this.btnSaveHome_Click);
            // 
            // btnHomeView
            // 
            this.btnHomeView.ButtonText = "";
            this.btnHomeView.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnHomeView.ImageClicked = global::SOPManager.Properties.Resources._36_HomeView_disable;
            this.btnHomeView.ImageDisabled = global::SOPManager.Properties.Resources._36_HomeView_disable;
            this.btnHomeView.ImageMouseOver = global::SOPManager.Properties.Resources._36_HomeView_Checked;
            this.btnHomeView.ImageNormal = global::SOPManager.Properties.Resources._36_HomeView_normal;
            this.btnHomeView.Location = new System.Drawing.Point(8, 284);
            this.btnHomeView.Name = "btnHomeView";
            this.btnHomeView.Owner = null;
            this.btnHomeView.Size = new System.Drawing.Size(36, 36);
            this.btnHomeView.TabIndex = 7;
            this.btnHomeView.TabStop = false;
            this.btnHomeView.TextColor = System.Drawing.Color.Black;
            this.btnHomeView.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnHomeView.ToolTipText = "홈 뷰";
            this.btnHomeView.Click += new System.EventHandler(this.btnHomeView_Click);
            // 
            // tmrUpdateCmd
            // 
            this.tmrUpdateCmd.Interval = 500;
            this.tmrUpdateCmd.Tick += new System.EventHandler(this.tmrUpdateCmd_Tick);
            // 
            // btnSpaceX
            // 
            this.btnSpaceX.ButtonText = "";
            this.btnSpaceX.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSpaceX.ImageClicked = global::SOPManager.Properties.Resources._36_가로간격_disable;
            this.btnSpaceX.ImageDisabled = global::SOPManager.Properties.Resources._36_가로간격_disable;
            this.btnSpaceX.ImageMouseOver = global::SOPManager.Properties.Resources._36_가로간격_Checked;
            this.btnSpaceX.ImageNormal = global::SOPManager.Properties.Resources._36_가로간격_normal;
            this.btnSpaceX.Location = new System.Drawing.Point(8, 422);
            this.btnSpaceX.Name = "btnSpaceX";
            this.btnSpaceX.Owner = null;
            this.btnSpaceX.Size = new System.Drawing.Size(36, 36);
            this.btnSpaceX.TabIndex = 8;
            this.btnSpaceX.TabStop = false;
            this.btnSpaceX.TextColor = System.Drawing.Color.Black;
            this.btnSpaceX.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSpaceX.ToolTipText = "가로간격 일정하게";
            this.btnSpaceX.Click += new System.EventHandler(this.btnSpaceX_Click);
            // 
            // btnSpaceY
            // 
            this.btnSpaceY.ButtonText = "";
            this.btnSpaceY.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSpaceY.ImageClicked = global::SOPManager.Properties.Resources._36_세로간격_disable;
            this.btnSpaceY.ImageDisabled = global::SOPManager.Properties.Resources._36_세로간격_disable;
            this.btnSpaceY.ImageMouseOver = global::SOPManager.Properties.Resources._36_세로간격_Checked;
            this.btnSpaceY.ImageNormal = global::SOPManager.Properties.Resources._36_세로간격_normal;
            this.btnSpaceY.Location = new System.Drawing.Point(8, 461);
            this.btnSpaceY.Name = "btnSpaceY";
            this.btnSpaceY.Owner = null;
            this.btnSpaceY.Size = new System.Drawing.Size(36, 36);
            this.btnSpaceY.TabIndex = 9;
            this.btnSpaceY.TabStop = false;
            this.btnSpaceY.TextColor = System.Drawing.Color.Black;
            this.btnSpaceY.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSpaceY.ToolTipText = "세로간격 일정하게";
            this.btnSpaceY.Click += new System.EventHandler(this.btnSpaceY_Click);
            // 
            // btnArrangeY
            // 
            this.btnArrangeY.ButtonText = "";
            this.btnArrangeY.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnArrangeY.ImageClicked = global::SOPManager.Properties.Resources._36_가로높이_disable;
            this.btnArrangeY.ImageDisabled = global::SOPManager.Properties.Resources._36_가로높이_disable;
            this.btnArrangeY.ImageMouseOver = global::SOPManager.Properties.Resources._36_가로높이_Checked;
            this.btnArrangeY.ImageNormal = global::SOPManager.Properties.Resources._36_가로높이_normal;
            this.btnArrangeY.Location = new System.Drawing.Point(8, 500);
            this.btnArrangeY.Name = "btnArrangeY";
            this.btnArrangeY.Owner = null;
            this.btnArrangeY.Size = new System.Drawing.Size(36, 36);
            this.btnArrangeY.TabIndex = 11;
            this.btnArrangeY.TabStop = false;
            this.btnArrangeY.TextColor = System.Drawing.Color.Black;
            this.btnArrangeY.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnArrangeY.ToolTipText = "위 맞춤";
            this.btnArrangeY.Click += new System.EventHandler(this.btnArrangeY_Click);
            // 
            // btnArrangeX
            // 
            this.btnArrangeX.ButtonText = "";
            this.btnArrangeX.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnArrangeX.ImageClicked = global::SOPManager.Properties.Resources._36_세로위치_disable;
            this.btnArrangeX.ImageDisabled = global::SOPManager.Properties.Resources._36_세로위치_disable;
            this.btnArrangeX.ImageMouseOver = global::SOPManager.Properties.Resources._36_세로위치_Checked;
            this.btnArrangeX.ImageNormal = global::SOPManager.Properties.Resources._36_세로위치_normal;
            this.btnArrangeX.Location = new System.Drawing.Point(8, 539);
            this.btnArrangeX.Name = "btnArrangeX";
            this.btnArrangeX.Owner = null;
            this.btnArrangeX.Size = new System.Drawing.Size(36, 36);
            this.btnArrangeX.TabIndex = 12;
            this.btnArrangeX.TabStop = false;
            this.btnArrangeX.TextColor = System.Drawing.Color.Black;
            this.btnArrangeX.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnArrangeX.ToolTipText = "왼쪽 맞춤";
            this.btnArrangeX.Click += new System.EventHandler(this.btnArrangeX_Click);
            // 
            // btnMiddleY
            // 
            this.btnMiddleY.ButtonText = "";
            this.btnMiddleY.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnMiddleY.ImageClicked = global::SOPManager.Properties.Resources._36_세로중심_disable;
            this.btnMiddleY.ImageDisabled = global::SOPManager.Properties.Resources._36_세로중심_disable;
            this.btnMiddleY.ImageMouseOver = global::SOPManager.Properties.Resources._36_세로중심_Checked;
            this.btnMiddleY.ImageNormal = global::SOPManager.Properties.Resources._36_세로중심_normal;
            this.btnMiddleY.Location = new System.Drawing.Point(8, 618);
            this.btnMiddleY.Name = "btnMiddleY";
            this.btnMiddleY.Owner = null;
            this.btnMiddleY.Size = new System.Drawing.Size(36, 36);
            this.btnMiddleY.TabIndex = 13;
            this.btnMiddleY.TabStop = false;
            this.btnMiddleY.TextColor = System.Drawing.Color.Black;
            this.btnMiddleY.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnMiddleY.ToolTipText = "세로 중심 맞춤";
            this.btnMiddleY.Click += new System.EventHandler(this.btnMiddleY_Click);
            // 
            // btnMiddleX
            // 
            this.btnMiddleX.ButtonText = "";
            this.btnMiddleX.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnMiddleX.ImageClicked = global::SOPManager.Properties.Resources._36_가로중심_disable;
            this.btnMiddleX.ImageDisabled = global::SOPManager.Properties.Resources._36_가로중심_disable;
            this.btnMiddleX.ImageMouseOver = global::SOPManager.Properties.Resources._36_가로중심_Checked;
            this.btnMiddleX.ImageNormal = global::SOPManager.Properties.Resources._36_가로중심_normal;
            this.btnMiddleX.Location = new System.Drawing.Point(8, 578);
            this.btnMiddleX.Name = "btnMiddleX";
            this.btnMiddleX.Owner = null;
            this.btnMiddleX.Size = new System.Drawing.Size(36, 36);
            this.btnMiddleX.TabIndex = 14;
            this.btnMiddleX.TabStop = false;
            this.btnMiddleX.TextColor = System.Drawing.Color.Black;
            this.btnMiddleX.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnMiddleX.ToolTipText = "가로 중심 맞춤";
            this.btnMiddleX.Click += new System.EventHandler(this.btnMiddleX_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackgroundImage = global::SOPManager.Properties.Resources.skin_line_img;
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox2.Location = new System.Drawing.Point(8, 159);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(36, 2);
            this.pictureBox2.TabIndex = 16;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackgroundImage = global::SOPManager.Properties.Resources.skin_line_img;
            this.pictureBox5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox5.Location = new System.Drawing.Point(8, 325);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(36, 2);
            this.pictureBox5.TabIndex = 20;
            this.pictureBox5.TabStop = false;
            // 
            // pictureBox6
            // 
            this.pictureBox6.BackgroundImage = global::SOPManager.Properties.Resources.skin_line_img;
            this.pictureBox6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox6.Location = new System.Drawing.Point(8, 414);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(36, 2);
            this.pictureBox6.TabIndex = 21;
            this.pictureBox6.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(58, 73);
            this.panel1.TabIndex = 22;
            // 
            // FormToolBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.ClientSize = new System.Drawing.Size(58, 710);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox6);
            this.Controls.Add(this.pictureBox5);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.btnMiddleX);
            this.Controls.Add(this.btnMiddleY);
            this.Controls.Add(this.btnArrangeX);
            this.Controls.Add(this.btnArrangeY);
            this.Controls.Add(this.btnSpaceY);
            this.Controls.Add(this.btnSpaceX);
            this.Controls.Add(this.btnHomeView);
            this.Controls.Add(this.btnSaveHome);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnScreenShot);
            this.Controls.Add(this.btnResetView);
            this.Controls.Add(this.btnFitView);
            this.Controls.Add(this.btnZoomOut);
            this.Controls.Add(this.btnZoomIn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormToolBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormToolBox_FormClosing);
            this.Load += new System.EventHandler(this.FormToolBox_Load);
            ((System.ComponentModel.ISupportInitialize)(this.btnZoomIn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnZoomOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFitView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnResetView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnScreenShot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPrint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSaveHome)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnHomeView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSpaceX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSpaceY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnArrangeY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnArrangeX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMiddleY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMiddleX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private UnE.GUI.ImageButton btnZoomIn;
        private UnE.GUI.ImageButton btnZoomOut;
        private UnE.GUI.ImageButton btnFitView;
        private UnE.GUI.ImageButton btnResetView;
		private UnE.GUI.ImageButton btnScreenShot;
        private UnE.GUI.ImageButton btnPrint;
        private UnE.GUI.ImageButton btnSaveHome;
        private UnE.GUI.ImageButton btnHomeView;
		private System.Windows.Forms.Timer tmrUpdateCmd;
        private UnE.GUI.ImageButton btnSpaceX;
        private UnE.GUI.ImageButton btnSpaceY;
        private UnE.GUI.ImageButton btnArrangeY;
        private UnE.GUI.ImageButton btnArrangeX;
        private UnE.GUI.ImageButton btnMiddleY;
        private UnE.GUI.ImageButton btnMiddleX;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.Panel panel1;
	}
}