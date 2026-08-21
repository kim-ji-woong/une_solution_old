using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;
using System.Collections.Concurrent;

namespace SDMS_Building.Content
{
    public partial class TooltipCCTVCtrl : Form, IPOIPopup
    {
        public static Size CctvPopupSize = new Size();
        // Target과의 거리
        static private int m_nTargetSpaceX = 30;

        static private int m_nTargetSpaceY = 50;

        private int m_nOwnTargetSpaceX = -1;
        private int m_nOwnTargetSpaceY = -1;
        private int m_nTargetPOIX = 0;
        private int m_nTargetPOIY = 0;
        private Point m_ptOrigin = new Point();

        private ISensorTooltipOwner m_viewOwner = null;

        private static ConcurrentDictionary<TooltipCCTVCtrl, TooltipCCTVCtrl> m_dicCurrentTooltips = new ConcurrentDictionary<TooltipCCTVCtrl, TooltipCCTVCtrl>();
        public static ConcurrentDictionary<TooltipCCTVCtrl, TooltipCCTVCtrl> DicCurrentTooltips
        {
            get { return m_dicCurrentTooltips; }
        }

        public bool Connected
        {
            get { return cctvCtrl1 != null && cctvCtrl1.IsConnected; }
        }

        private CCTV m_cctv = null;

        public CCTV CCTV
        {
            get { return m_cctv; }
            set { m_cctv = value; }
        }

        public ISensor Sensor
        {
            get;
            set;
        }

        private bool m_bLayerVisible = true;

        public bool LayerVisible
        {
            get { return m_bLayerVisible; }
            set
            {
                m_bLayerVisible = value;
                if (m_bLayerVisible == false)
                {
                    Visible = false;
                }
                else
                {
                    if (this.Visible)
                    {
                        //base.Show();
                    }
                }
            }
        }

        private static int m_nCCTVCount = 0;
        protected int m_nID = -1;

        private UnE.Control.CCTVCtrl cctvCtrl1 = null;

        protected UnE.Control.CCTVCtrl CCTVCtrl
        {
            get { return cctvCtrl1; }
        }

        public static TooltipCCTVCtrl MakeInstance(ISensorTooltipOwner view, CCTV cctv)
        {            
            return new TooltipCCTVCtrl(view, cctv);
        }

        public TooltipCCTVCtrl(ISensorTooltipOwner view, CCTV cctv)
        {
            m_nID = ++m_nCCTVCount;

            InitializeComponent();

            cctvPanel.Size = new Size(this.Width, this.Height - pnTop.Height);
            cctvPanel.Location = new Point(0, pnTop.Height);

            TooltipCCTVCtrl.CctvPopupSize = this.Size;

            m_nOwnTargetSpaceX = m_nTargetSpaceX;
            m_nOwnTargetSpaceY = m_nTargetSpaceY;

            this.TopLevel = false;
            view.AddToolTipControl(this);
            this.BringToFront();

            m_viewOwner = view;
            m_cctv = cctv;

            base.Hide();
        }

        private void InitCCTVCtrl(CCTV cctv)
        {
            cctvCtrl1 = new UnE.Control.CCTVCtrl((UnE.Control.CCTVTypes)cctv.CCTVType);

            this.cctvCtrl1.BackColor = System.Drawing.Color.Black;
            this.cctvCtrl1.CCTVOwner = null;
            this.cctvCtrl1.Location = new System.Drawing.Point(0, 0);
            this.cctvCtrl1.Name = "cctvCtrl1";
            this.cctvCtrl1.Size = new System.Drawing.Size(279, 246);
            this.cctvCtrl1.Dock = DockStyle.Fill;

            this.cctvPanel.Controls.Add(cctvCtrl1);
        }

        private void TooltipCCTVCtrl_FormClosing(object sender, FormClosingEventArgs e)
        {
            Disconnect();

            e.Cancel = true;
            base.Hide();

            TooltipCCTVCtrl tooltip;
            m_dicCurrentTooltips.TryRemove(this, out tooltip);
            System.Diagnostics.Trace.WriteLine("FormClosing : " + m_nID.ToString());
        }

        public void Disconnect()
        {
            if (cctvCtrl1 != null)
            {
                this.cctvPanel.Controls.Remove(cctvCtrl1);
                cctvCtrl1.Disconnect();
                cctvCtrl1.Dispose();                
                cctvCtrl1 = null;
            }
        }

        protected virtual void LoadCamera()
        {
            lblTitle.Text = string.Format("{0}. {1}", CCTV.ID, CCTV.AccessKey);

            //cctvCtrl1.ChangeType(GetCCTVType(CCTV.CCTVType));
            cctvCtrl1.AddProperty("MediaType", "rtp-tcp");
            cctvCtrl1.AddProperty("Channel", CCTV.Channel.ToString());
            cctvCtrl1.AddProperty("Stream", CCTV.Stream.ToString());
            cctvCtrl1.AddProperty("HttpPort", CCTV.HttpPort.ToString());
            cctvCtrl1.AddProperty("IPAddress", CCTV.IPAddress);
            cctvCtrl1.AddProperty("Port", CCTV.PortNo.ToString());
            cctvCtrl1.AddProperty("UserName", CCTV.UserName);
            cctvCtrl1.AddProperty("Password", CCTV.Password);
            //cctvCtrl1.AddProperty("ReversePTZ", CCTV.ReversePTZ.ToString());
            cctvCtrl1.AddProperty("AccessKey", CCTV.AccessKey.ToString());
            cctvCtrl1.AddProperty("URL", CCTV.URL.ToString());

            cctvCtrl1.Connect();
        }

        // xTarget, yTarget : Target POI의 좌표
        public void Show(int xTarget, int yTarget)
        {
            try
            {
                this.Location = new Point(xTarget, yTarget);

                InitCCTVCtrl(m_cctv);

                if (m_cctv != null)
                    lblTitle.Text = String.Format("{0} - {1}", m_cctv.ID, m_cctv.AccessKey);

                m_nTargetPOIX = xTarget;
                m_nTargetPOIY = yTarget;
                m_ptOrigin = this.Location;

                int x = xTarget + m_nOwnTargetSpaceX;
                int y = yTarget - m_nOwnTargetSpaceY;

                //if (CCTVCtrl.IsConnected == false)
                //    LoadCamera();

                
                this.Show();

                if (CCTVCtrl.IsConnected == false)
                    LoadCamera();

                this.BringToFront();

                if (this.IsDisposed == false)
                    m_dicCurrentTooltips[this] = this;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("TooltipCCTVCtrl.Show() Error : " + e.Message);
            }
        }

        public void Hide(bool absolutely)
        {
            if (IsDisposed == true)
                return;

            Disconnect();
            base.Hide();

            TooltipCCTVCtrl tooltip;
            m_dicCurrentTooltips.TryRemove(this, out tooltip);
        }

        public void MoveTarget(int xTarget, int yTarget)
        {
            m_nTargetPOIX = xTarget;
            m_nTargetPOIY = yTarget;
            m_ptOrigin = this.Location;

            int x = xTarget + m_nOwnTargetSpaceX;
            int y = yTarget - m_nOwnTargetSpaceY;

            this.Location = new Point(x, y);
        }

        public bool IsVisible()
        {
            if (m_bLayerVisible == true && this.Visible == true)
                return true;
            return this.Visible;
        }

        public static void CloseAll()
        {
            List< TooltipCCTVCtrl> tooltips = m_dicCurrentTooltips.Values.ToList();

            foreach (TooltipCCTVCtrl tooltip in tooltips)
            {
                tooltip.Hide(true);
            }

            m_dicCurrentTooltips.Clear();
        }

        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();
        private bool m_isClicked = false;
        private Point m_ptFrmOrigin = new Point();

        private void pnTop_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = Control.MousePosition;
                m_ptFrmOrigin = this.Location;
            }

            m_isClicked = true;
        }

        private void pnTop_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_isClicked)
                return;

            if (!m_bLeftMouseDown)
                return;

            Point ptScreen = Control.MousePosition;

            int dx = ptScreen.X - m_ptMove.X;
            int dy = ptScreen.Y - m_ptMove.Y;

            if (dx == 0 && dy == 0)
                return;

            Point ptCur = this.Location;
            this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
            m_ptMove.X += dx;
            m_ptMove.Y += dy;
        }

        private void pnTop_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;

            m_isClicked = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TooltipCCTVCtrl_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
    }
}
