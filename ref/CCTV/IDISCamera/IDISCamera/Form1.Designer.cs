namespace IDISCamera
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panel1 = new System.Windows.Forms.Panel();
            this.axRASplus_WatSear1 = new IDISCamera.IDISCameraControl();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axRASplus_WatSear1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.axRASplus_WatSear1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(284, 223);
            this.panel1.TabIndex = 1;
            // 
            // axRASplus_WatSear1
            // 
            this.axRASplus_WatSear1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axRASplus_WatSear1.Enabled = true;
            this.axRASplus_WatSear1.Location = new System.Drawing.Point(0, 0);
            this.axRASplus_WatSear1.Name = "axRASplus_WatSear1";
            this.axRASplus_WatSear1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axRASplus_WatSear1.OcxState")));
            this.axRASplus_WatSear1.Size = new System.Drawing.Size(284, 223);
            this.axRASplus_WatSear1.TabIndex = 0;
            this.axRASplus_WatSear1.ConnectedWatch += new AxRASplus_WatSearLib._DRASplus_WatSearEvents_ConnectedWatchEventHandler(this.axRASplus_WatSear1_ConnectedWatch);
            this.axRASplus_WatSear1.DisconnectedWatch += new AxRASplus_WatSearLib._DRASplus_WatSearEvents_DisconnectedWatchEventHandler(this.axRASplus_WatSear1_DisconnectedWatch);
            this.axRASplus_WatSear1.FrameLoaded += new AxRASplus_WatSearLib._DRASplus_WatSearEvents_FrameLoadedEventHandler(this.axRASplus_WatSear1_FrameLoaded);
            this.axRASplus_WatSear1.LayoutChanged += new AxRASplus_WatSearLib._DRASplus_WatSearEvents_LayoutChangedEventHandler(this.axRASplus_WatSear1_LayoutChanged);
            this.axRASplus_WatSear1.EventLoaded += new AxRASplus_WatSearLib._DRASplus_WatSearEvents_EventLoadedEventHandler(this.axRASplus_WatSear1_EventLoaded);
            this.axRASplus_WatSear1.SearchEventLoaded += new AxRASplus_WatSearLib._DRASplus_WatSearEvents_SearchEventLoadedEventHandler(this.axRASplus_WatSear1_SearchEventLoaded);
            this.axRASplus_WatSear1.SegmentSpots += new AxRASplus_WatSearLib._DRASplus_WatSearEvents_SegmentSpotsEventHandler(this.axRASplus_WatSear1_SegmentSpots);
            this.axRASplus_WatSear1.StatusLoaded += new AxRASplus_WatSearLib._DRASplus_WatSearEvents_StatusLoadedEventHandler(this.axRASplus_WatSear1_StatusLoaded);
            this.axRASplus_WatSear1.FindingIDREventTime += new AxRASplus_WatSearLib._DRASplus_WatSearEvents_FindingIDREventTimeEventHandler(this.axRASplus_WatSear1_FindingIDREventTime);
            this.axRASplus_WatSear1.SearchTextInLoaded += new AxRASplus_WatSearLib._DRASplus_WatSearEvents_SearchTextInLoadedEventHandler(this.axRASplus_WatSear1_SearchTextInLoaded);
            this.axRASplus_WatSear1.WatchStatusLoadedIDR += new AxRASplus_WatSearLib._DRASplus_WatSearEvents_WatchStatusLoadedIDREventHandler(this.axRASplus_WatSear1_WatchStatusLoadedIDR);
            this.axRASplus_WatSear1.externalTangoInfo += new AxRASplus_WatSearLib._DRASplus_WatSearEvents_externalTangoInfoEventHandler(this.axRASplus_WatSear1_externalTangoInfo);
            this.axRASplus_WatSear1.CallbackEventLoaded += new AxRASplus_WatSearLib._DRASplus_WatSearEvents_CallbackEventLoadedEventHandler(this.axRASplus_WatSear1_CallbackEventLoaded);
            this.axRASplus_WatSear1.RecvScreenSecureRawVideoFrame += new AxRASplus_WatSearLib._DRASplus_WatSearEvents_RecvScreenSecureRawVideoFrameEventHandler(this.axRASplus_WatSear1_RecvScreenSecureRawVideoFrame);
            this.axRASplus_WatSear1.CameraStatusLoaded += new AxRASplus_WatSearLib._DRASplus_WatSearEvents_CameraStatusLoadedEventHandler(this.axRASplus_WatSear1_CameraStatusLoaded);
            this.axRASplus_WatSear1.TextInEventLoaded += new AxRASplus_WatSearLib._DRASplus_WatSearEvents_TextInEventLoadedEventHandler(this.axRASplus_WatSear1_TextInEventLoaded);
            this.axRASplus_WatSear1.PlayEventLoaded += new AxRASplus_WatSearLib._DRASplus_WatSearEvents_PlayEventLoadedEventHandler(this.axRASplus_WatSear1_PlayEventLoaded);
            this.axRASplus_WatSear1.PluginMessage += new AxRASplus_WatSearLib._DRASplus_WatSearEvents_PluginMessageEventHandler(this.axRASplus_WatSear1_PluginMessage);
            this.axRASplus_WatSear1.SetNatType += new AxRASplus_WatSearLib._DRASplus_WatSearEvents_SetNatTypeEventHandler(this.axRASplus_WatSear1_SetNatType);
            this.axRASplus_WatSear1.CausesValidationChanged += new System.EventHandler(this.axRASplus_WatSear1_CausesValidationChanged);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDisconnect.Location = new System.Drawing.Point(214, 229);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(58, 23);
            this.btnDisconnect.TabIndex = 2;
            this.btnDisconnect.Text = "중단";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Location = new System.Drawing.Point(150, 229);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(58, 23);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "접속";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axRASplus_WatSear1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private IDISCameraControl axRASplus_WatSear1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Button btnConnect;
    }
}

