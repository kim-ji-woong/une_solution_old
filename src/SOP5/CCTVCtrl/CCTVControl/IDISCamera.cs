using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnE.Control.CCTVControl
{
    public interface IDISCameraOwner
    {
        void OnMouseDown(object sender, MouseButtons btn, int x, int y);
        void OnMouseDblClick(object sender, MouseButtons btn, int x, int y);
        void OnConnected(object sender);
        void OnDisconnected(object sender);
    }
#if _IDIS_
    public class IDISCameraControl : AxRASplus_WatSearLib.AxRASplus_WatSear
    {
        private const short LAYOUT_1X1 = 0;
        private const short LAYOUT_2X2 = 1;
        private const short LAYOUT_3X3 = 2;
        private const short LAYOUT_4X4 = 3;
        private const short LAYOUT_5X5 = 4;
        private const short LAYOUT_6X6 = 5;
        private const short LAYOUT_7X7 = 6;
        private const short LAYOUT_8X8 = 7;
        private const short LAYOUT_8X1 = 8;
        private const short LAYOUT_12X1 = 9;
        private const short LAYOUT_32X1 = 10;

        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONDBLCLK = 0x0203;
        private const int WM_RBUTTONDOWN = 0x0204;

        private IDISCameraOwner m_owner = null;

        public IDISCameraOwner Owner
        {
            get { return m_owner; }
            set { m_owner = value; }
        }

        public IDISCameraControl()
        {
            this.ConnectedWatch += new AxRASplus_WatSearLib._DRASplus_WatSearEvents_ConnectedWatchEventHandler(this.OnConnectedWatch);
            this.DisconnectedWatch += new AxRASplus_WatSearLib._DRASplus_WatSearEvents_DisconnectedWatchEventHandler(this.OnDisconnectedWatch);
            this.LayoutChanged += new AxRASplus_WatSearLib._DRASplus_WatSearEvents_LayoutChangedEventHandler(this.OnLayoutChanged);
        }

        private void OnConnectedWatch(object sender, AxRASplus_WatSearLib._DRASplus_WatSearEvents_ConnectedWatchEvent e)
        {
            if (m_owner != null)
                m_owner.OnConnected(this);
        }

        private void OnDisconnectedWatch(object sender, AxRASplus_WatSearLib._DRASplus_WatSearEvents_DisconnectedWatchEvent e)
        {
            if (m_owner != null)
                m_owner.OnDisconnected(this);
        }

        private void OnLayoutChanged(object sender, AxRASplus_WatSearLib._DRASplus_WatSearEvents_LayoutChangedEvent e)
        {
            if (e.layout != LAYOUT_1X1)
            {
                // Layout이 변경되지 못하도록 한다.
                this.setLayout(LAYOUT_1X1);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN)
            {
                MouseDown(MouseButtons.Left, (int)m.LParam);
            }
            else if (m.Msg == WM_RBUTTONDOWN)
            {
                MouseDown(MouseButtons.Right, (int)m.LParam);
            }
            else if (m.Msg == WM_LBUTTONDBLCLK)
            {
                MouseDblClick(MouseButtons.Left, (int)m.LParam);
            }

            base.WndProc(ref m);
        }

        private void MouseDown(MouseButtons btn, int lParam)
        {
            if (m_owner != null)
            {
                int x = (lParam & 0xffff);
                int y = (lParam >> 16);
                m_owner.OnMouseDown(this, btn, x, y);
            }
        }

        private void MouseDblClick(MouseButtons btn, int lParam)
        {
            if (m_owner != null)
            {
                int x = (lParam & 0xffff);
                int y = (lParam >> 16);
                m_owner.OnMouseDblClick(this, btn, x, y);
            }
        }
    }
#endif
}
