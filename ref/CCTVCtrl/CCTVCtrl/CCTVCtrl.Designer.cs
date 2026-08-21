namespace UnE.Control
{
    partial class CCTVCtrl
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

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CCTVCtrl));
            this.axxpressStrm1 = new AxxpressStrmLib.AxxpressStrm();
            this.axNVSViewerCtrl1 = new AxNVSVIEWERCTRLLib.AxNVSViewerCtrl();
            this.axAxVCA1 = new AxAXVCALib.AxAxVCA();
            this.axipropsapiCtrl1 = new AxIPROPSAPILib.AxipropsapiCtrl();
            this.axTVSLiveControl1 = new AxTVSLib.AxTVSLiveControl();
            this.axTechWinLib1 = new Axwebviewer_activexplugin_libLib.Axwebviewer_activexplugin_lib();
            this.axAxisMediaControl1 = new AxAXISMEDIACONTROLLib.AxAxisMediaControl();
            ((System.ComponentModel.ISupportInitialize)(this.axxpressStrm1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axNVSViewerCtrl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axAxVCA1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axipropsapiCtrl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axTVSLiveControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axTechWinLib1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axAxisMediaControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // axxpressStrm1
            // 
            this.axxpressStrm1.Enabled = true;
            this.axxpressStrm1.Location = new System.Drawing.Point(113, 127);
            this.axxpressStrm1.Name = "axxpressStrm1";
            this.axxpressStrm1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axxpressStrm1.OcxState")));
            this.axxpressStrm1.Size = new System.Drawing.Size(75, 55);
            this.axxpressStrm1.TabIndex = 0;
            this.axxpressStrm1.Notify += new AxxpressStrmLib._DxpressStrmEvents_NotifyEventHandler(this.axxpressStrm1_Notify);
            this.axxpressStrm1.EventSignal += new AxxpressStrmLib._DxpressStrmEvents_EventSignalEventHandler(this.axxpressStrm1_EventSignal);
            this.axxpressStrm1.StatusChanged += new System.EventHandler(this.axxpressStrm1_StatusChanged);
            this.axxpressStrm1.Enter += new System.EventHandler(this.axxpressStrm1_Enter);
            this.axxpressStrm1.Leave += new System.EventHandler(this.axxpressStrm1_Leave);
            this.axxpressStrm1.MouseCaptureChanged += new System.EventHandler(this.axxpressStrm1_MouseCaptureChanged);
            // 
            // axNVSViewerCtrl1
            // 
            this.axNVSViewerCtrl1.Enabled = true;
            this.axNVSViewerCtrl1.Location = new System.Drawing.Point(371, 144);
            this.axNVSViewerCtrl1.Name = "axNVSViewerCtrl1";
            this.axNVSViewerCtrl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axNVSViewerCtrl1.OcxState")));
            this.axNVSViewerCtrl1.Size = new System.Drawing.Size(50, 49);
            this.axNVSViewerCtrl1.TabIndex = 2;
            // 
            // axAxVCA1
            // 
            this.axAxVCA1.Enabled = true;
            this.axAxVCA1.Location = new System.Drawing.Point(229, 259);
            this.axAxVCA1.Name = "axAxVCA1";
            this.axAxVCA1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axAxVCA1.OcxState")));
            this.axAxVCA1.Size = new System.Drawing.Size(71, 52);
            this.axAxVCA1.TabIndex = 3;
            // 
            // axipropsapiCtrl1
            // 
            this.axipropsapiCtrl1.Enabled = true;
            this.axipropsapiCtrl1.Location = new System.Drawing.Point(371, 199);
            this.axipropsapiCtrl1.Name = "axipropsapiCtrl1";
            this.axipropsapiCtrl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axipropsapiCtrl1.OcxState")));
            this.axipropsapiCtrl1.Size = new System.Drawing.Size(192, 192);
            this.axipropsapiCtrl1.TabIndex = 4;
            // 
            // axTVSLiveControl1
            // 
            this.axTVSLiveControl1.Enabled = true;
            this.axTVSLiveControl1.Location = new System.Drawing.Point(-4, 199);
            this.axTVSLiveControl1.Name = "axTVSLiveControl1";
            this.axTVSLiveControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axTVSLiveControl1.OcxState")));
            this.axTVSLiveControl1.Size = new System.Drawing.Size(192, 192);
            this.axTVSLiveControl1.TabIndex = 5;
            // 
            // axTechWinLib1
            // 
            this.axTechWinLib1.Enabled = true;
            this.axTechWinLib1.Location = new System.Drawing.Point(83, 24);
            this.axTechWinLib1.Name = "axTechWinLib1";
            this.axTechWinLib1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axTechWinLib1.OcxState")));
            this.axTechWinLib1.Size = new System.Drawing.Size(121, 47);
            this.axTechWinLib1.TabIndex = 6;
            // 
            // axAxisMediaControl1
            // 
            this.axAxisMediaControl1.Enabled = true;
            this.axAxisMediaControl1.Location = new System.Drawing.Point(235, 155);
            this.axAxisMediaControl1.Name = "axAxisMediaControl1";
            this.axAxisMediaControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axAxisMediaControl1.OcxState")));
            this.axAxisMediaControl1.Size = new System.Drawing.Size(93, 69);
            this.axAxisMediaControl1.TabIndex = 7;
            // 
            // CCTVCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.axAxisMediaControl1);
            this.Controls.Add(this.axTechWinLib1);
            this.Controls.Add(this.axTVSLiveControl1);
            this.Controls.Add(this.axipropsapiCtrl1);
            this.Controls.Add(this.axAxVCA1);
            this.Controls.Add(this.axNVSViewerCtrl1);
            this.Controls.Add(this.axxpressStrm1);
            this.Name = "CCTVCtrl";
            this.Size = new System.Drawing.Size(584, 417);
            ((System.ComponentModel.ISupportInitialize)(this.axxpressStrm1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axNVSViewerCtrl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axAxVCA1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axipropsapiCtrl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axTVSLiveControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axTechWinLib1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axAxisMediaControl1)).EndInit();
            this.ResumeLayout(false);

        }


        #endregion

        private AxxpressStrmLib.AxxpressStrm axxpressStrm1;
        private AxNVSVIEWERCTRLLib.AxNVSViewerCtrl axNVSViewerCtrl1;
        private AxAXVCALib.AxAxVCA axAxVCA1;
        private AxIPROPSAPILib.AxipropsapiCtrl axipropsapiCtrl1;
        private AxTVSLib.AxTVSLiveControl axTVSLiveControl1;
        private Axwebviewer_activexplugin_libLib.Axwebviewer_activexplugin_lib axTechWinLib1;
        private AxAXISMEDIACONTROLLib.AxAxisMediaControl axAxisMediaControl1;
    }
}
