using System.ComponentModel.Design;

namespace HSMS
{
    partial class FormContent
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
            this.components = new System.ComponentModel.Container();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.contextMenuStripManualReport = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuAddDisasterPos = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuIndoor = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuManualCCTV = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.menuManualReport = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripBuilding = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStripManualReport.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "dae";
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // contextMenuStripManualReport
            // 
            this.contextMenuStripManualReport.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAddDisasterPos,
            this.toolStripSeparator2,
            this.menuIndoor,
            this.toolStripSeparator3,
            this.menuManualCCTV,
            this.toolStripSeparator4,
            this.menuManualReport});
            this.contextMenuStripManualReport.Name = "popupMenu";
            this.contextMenuStripManualReport.Size = new System.Drawing.Size(151, 110);
            // 
            // menuAddDisasterPos
            // 
            this.menuAddDisasterPos.Name = "menuAddDisasterPos";
            this.menuAddDisasterPos.Size = new System.Drawing.Size(150, 22);
            this.menuAddDisasterPos.Text = "재난위치 설정";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(147, 6);
            // 
            // menuIndoor
            // 
            this.menuIndoor.Name = "menuIndoor";
            this.menuIndoor.Size = new System.Drawing.Size(150, 22);
            this.menuIndoor.Text = "실내 보기";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(147, 6);
            // 
            // menuManualCCTV
            // 
            this.menuManualCCTV.Name = "menuManualCCTV";
            this.menuManualCCTV.Size = new System.Drawing.Size(150, 22);
            this.menuManualCCTV.Text = "CCTV 보기";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(147, 6);
            // 
            // menuManualReport
            // 
            this.menuManualReport.Name = "menuManualReport";
            this.menuManualReport.Size = new System.Drawing.Size(150, 22);
            this.menuManualReport.Text = "화재 신고";
            // 
            // contextMenuStripBuilding
            // 
            this.contextMenuStripBuilding.Name = "contextMenuStripBuilding";
            this.contextMenuStripBuilding.Size = new System.Drawing.Size(61, 4);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem8,
            this.toolStripSeparator1,
            this.toolStripMenuItem9,
            this.toolStripMenuItem10,
            this.toolStripMenuItem11,
            this.toolStripMenuItem12});
            this.contextMenuStrip1.Name = "popupMenu";
            this.contextMenuStrip1.Size = new System.Drawing.Size(141, 142);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(140, 22);
            this.toolStripMenuItem1.Text = "Select";
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(140, 22);
            this.toolStripMenuItem8.Text = "Clear Select";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(137, 6);
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Checked = true;
            this.toolStripMenuItem9.CheckOnClick = true;
            this.toolStripMenuItem9.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(140, 22);
            this.toolStripMenuItem9.Text = "Texture";
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.CheckOnClick = true;
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(140, 22);
            this.toolStripMenuItem10.Text = "HiddenLine";
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Size = new System.Drawing.Size(140, 22);
            this.toolStripMenuItem11.Text = "Add POI";
            // 
            // toolStripMenuItem12
            // 
            this.toolStripMenuItem12.Name = "toolStripMenuItem12";
            this.toolStripMenuItem12.Size = new System.Drawing.Size(140, 22);
            this.toolStripMenuItem12.Text = "Remove POI";
            
            // 
            // FormContent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1920, 1080);
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormContent";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormLayout_FormClosed);
            this.Load += new System.EventHandler(this.FormLayout_Load);
            this.Shown += new System.EventHandler(this.FormContent_Shown);
            this.SizeChanged += new System.EventHandler(this.FormContent_SizeChanged);
            this.Resize += new System.EventHandler(this.FormLayout_Resize);
            this.contextMenuStripManualReport.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripManualReport;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripBuilding;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem10;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem11;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem12;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		public System.Windows.Forms.ToolStripMenuItem menuIndoor;
		public System.Windows.Forms.ToolStripMenuItem menuManualReport;
		public System.Windows.Forms.ToolStripMenuItem menuManualCCTV;
        public System.Windows.Forms.ToolStripMenuItem menuAddDisasterPos;
        private System.Windows.Forms.Timer timer1;


    }
}

