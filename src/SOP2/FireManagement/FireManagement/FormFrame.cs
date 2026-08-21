using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace FireManagement
{
    public class FormFrame : UnE.GUI.FormNoFrameSizable
    {
        private Button m_btn = new Button();
        private Panel m_topPanel = new Panel();

        private static FormFrame m_instance = null;
        public static FormFrame Instance
        {
            get { return m_instance; }
        }

        public FormFrame(Form frmMain)
            : base(frmMain)
        {


            m_instance = this;

            this.Load += new EventHandler(FormFrame_Load);

            
        }

        void FormFrame_Load(object sender, EventArgs e)
        {
            //this.WindowState = FormWindowState.Maximized;

            this.Size = new Size(1366, 768);

            this.TitleBarHeight = 30;
            this.TitleBarImage = global::FireManagement.Properties.Resources.FireManageMent_System;

            this.SystemButtonSize = new Size(41, 30);


            this.TitleTextFont = new Font("맑은 고딕", 18, FontStyle.Bold);
            this.TitleTextColor = System.Drawing.Color.FromArgb(237,234,234);
            //this.TitlePosition = FormMain2.Instance.PanelTitle.Size.Width / 2;
            //this.TitlePosition = 651;
            this.Text = "소방설비 관리 시스템";

            this.MinButtonImage = global::FireManagement.Properties.Resources.HideWindow_Normal;
            this.NormalButtonImage = global::FireManagement.Properties.Resources.NormalWindow_Normal;
            this.MaxButtonImage = global::FireManagement.Properties.Resources.MaxWindow_Normal;
            this.CloseButtonImage = global::FireManagement.Properties.Resources.CloseWindow_Normal;
            
            this.ResizeFrame();
        }

        protected override void btnMax_Click(object sender, EventArgs e)
        {
            base.btnMax_Click(sender, e);

            TitlePosition = (this.Width / 2) - (TitleTextWidth / 2);
        }
    }
}
