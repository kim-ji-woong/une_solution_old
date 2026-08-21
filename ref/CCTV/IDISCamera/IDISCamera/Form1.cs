using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IDISCamera
{
    public partial class Form1 : Form
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

        private FormCCTVList m_frmCCTVList = null;
        private CCTVInfo m_currentCCTV = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Test();
            axRASplus_WatSear1.initialize();
            axRASplus_WatSear1_DisconnectedWatch(null, null);

            axRASplus_WatSear1.setLayout(LAYOUT_1X1);
            axRASplus_WatSear1.setupOSD(false, false, false, false, false, false);
            // 접속 시도시 메시지박스가 나타나지 않도록 한다.
            axRASplus_WatSear1.setHiddenMessageBox(true);
            // Mouse 오른쪽 버튼 Click시 팝업메뉴가 나타나지 않도록 한다.
            axRASplus_WatSear1.setProperty(0, 0, 0, 0, "", "");

            m_frmCCTVList = new FormCCTVList(this);
            m_frmCCTVList.Show(this);
        }

        /*private void Test()
        {
            System.IO.StreamReader reader = new System.IO.StreamReader("C:\\Movie\\aaa.txt");
            System.IO.StreamWriter writer = new System.IO.StreamWriter("C:\\Movie\\bbb.txt");

            int nLine = 1;
            int ngCount = 0;

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine();
                int no = int.Parse(strLine);

                if (nLine < no)
                {
                    for (int i=nLine;i<no;i++)
                    {
                        writer.WriteLine(i.ToString() + "\tOK");
                    }
                    writer.WriteLine(no.ToString() + "\tNG"); ngCount++;
                }
                else if (nLine == no)
                {
                    writer.WriteLine(no.ToString() + "\tNG");
                    ngCount++;
                }

                nLine = no + 1;
            }

            reader.Close();
            writer.Close();
        }*/

        private void axRASplus_WatSear1_LayoutChanged(object sender, AxRASplus_WatSearLib._DRASplus_WatSearEvents_LayoutChangedEvent e)
        {
            if (e.layout != LAYOUT_1X1)
            {
                // Layout이 변경되지 못하도록 한다.
                axRASplus_WatSear1.setLayout(LAYOUT_1X1);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string strSiteName = "2동 외곽비상벨";
            string strIP = "192.168.2.98";
            string strUserID = "admin", strPW = "`cctv3112";
            short nPortNo = 8016;

            axRASplus_WatSear1.setCameraMap(0, 0, strSiteName, strIP, 0, strUserID, strPW, nPortNo, false, false, false, "", 0, 0);
            axRASplus_WatSear1.connect();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            axRASplus_WatSear1.disconnectAll();
        }

        private void axRASplus_WatSear1_ConnectedWatch(object sender, AxRASplus_WatSearLib._DRASplus_WatSearEvents_ConnectedWatchEvent e)
        {
            btnConnect.Enabled = false;
            btnDisconnect.Enabled = true;

            if (m_currentCCTV != null)
                System.Diagnostics.Trace.WriteLine(m_currentCCTV.IP + " is connected");
        }

        private void axRASplus_WatSear1_DisconnectedWatch(object sender, AxRASplus_WatSearLib._DRASplus_WatSearEvents_DisconnectedWatchEvent e)
        {
            btnConnect.Enabled = true;
            btnDisconnect.Enabled = false;

            if (m_currentCCTV != null)
                System.Diagnostics.Trace.WriteLine(m_currentCCTV.IP + " is not connected");
        }

        public void OnSelectCCTV(CCTVInfo cctv)
        {
            if (m_currentCCTV == cctv)
                return;

            axRASplus_WatSear1.disconnectAll();

            axRASplus_WatSear1.setCameraMap(0, 0, cctv.CameraName, cctv.IP, 0, cctv.UserID, cctv.PW, 8016, false, false, false, "", 0, 0);
            axRASplus_WatSear1.connect();
            bool isConnected = axRASplus_WatSear1.isConnected(0);

            m_currentCCTV = cctv;
        }

        private void axRASplus_WatSear1_CallbackEventLoaded(object sender, AxRASplus_WatSearLib._DRASplus_WatSearEvents_CallbackEventLoadedEvent e)
        {
            System.Diagnostics.Trace.WriteLine("axRASplus_WatSear1_CallbackEventLoaded");
            System.Diagnostics.Trace.WriteLine(e.ToString());
        }

        private void axRASplus_WatSear1_CameraStatusLoaded(object sender, AxRASplus_WatSearLib._DRASplus_WatSearEvents_CameraStatusLoadedEvent e)
        {
            System.Diagnostics.Trace.WriteLine("axRASplus_WatSear1_CameraStatusLoaded");
            System.Diagnostics.Trace.WriteLine(e.ToString());
        }

        private void axRASplus_WatSear1_CausesValidationChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("axRASplus_WatSear1_CausesValidationChanged");
        }

        private void axRASplus_WatSear1_EventLoaded(object sender, AxRASplus_WatSearLib._DRASplus_WatSearEvents_EventLoadedEvent e)
        {
            System.Diagnostics.Trace.WriteLine("axRASplus_WatSear1_EventLoaded");
            System.Diagnostics.Trace.WriteLine(e.ToString());
        }

        private void axRASplus_WatSear1_externalTangoInfo(object sender, AxRASplus_WatSearLib._DRASplus_WatSearEvents_externalTangoInfoEvent e)
        {
            System.Diagnostics.Trace.WriteLine("axRASplus_WatSear1_externalTangoInfo");
            System.Diagnostics.Trace.WriteLine(e.ToString());
        }

        private void axRASplus_WatSear1_FindingIDREventTime(object sender, AxRASplus_WatSearLib._DRASplus_WatSearEvents_FindingIDREventTimeEvent e)
        {
            System.Diagnostics.Trace.WriteLine("axRASplus_WatSear1_FindingIDREventTime");
            System.Diagnostics.Trace.WriteLine(e.ToString());
        }

        private void axRASplus_WatSear1_FrameLoaded(object sender, AxRASplus_WatSearLib._DRASplus_WatSearEvents_FrameLoadedEvent e)
        {
            System.Diagnostics.Trace.WriteLine("axRASplus_WatSear1_FrameLoaded");
            System.Diagnostics.Trace.WriteLine(e.ToString());
        }

        private void axRASplus_WatSear1_PlayEventLoaded(object sender, AxRASplus_WatSearLib._DRASplus_WatSearEvents_PlayEventLoadedEvent e)
        {
            System.Diagnostics.Trace.WriteLine("axRASplus_WatSear1_PlayEventLoaded");
            System.Diagnostics.Trace.WriteLine(e.ToString());
        }

        private void axRASplus_WatSear1_PluginMessage(object sender, AxRASplus_WatSearLib._DRASplus_WatSearEvents_PluginMessageEvent e)
        {
            System.Diagnostics.Trace.WriteLine("axRASplus_WatSear1_PluginMessage");
            System.Diagnostics.Trace.WriteLine(e.ToString());
        }

        private void axRASplus_WatSear1_RecvScreenSecureRawVideoFrame(object sender, AxRASplus_WatSearLib._DRASplus_WatSearEvents_RecvScreenSecureRawVideoFrameEvent e)
        {
            System.Diagnostics.Trace.WriteLine("axRASplus_WatSear1_RecvScreenSecureRawVideoFrame");
            System.Diagnostics.Trace.WriteLine(e.ToString());
        }

        private void axRASplus_WatSear1_SearchEventLoaded(object sender, AxRASplus_WatSearLib._DRASplus_WatSearEvents_SearchEventLoadedEvent e)
        {
            System.Diagnostics.Trace.WriteLine("axRASplus_WatSear1_SearchEventLoaded");
            System.Diagnostics.Trace.WriteLine(e.ToString());
        }

        private void axRASplus_WatSear1_SearchTextInLoaded(object sender, AxRASplus_WatSearLib._DRASplus_WatSearEvents_SearchTextInLoadedEvent e)
        {
            System.Diagnostics.Trace.WriteLine("axRASplus_WatSear1_SearchTextInLoaded");
            System.Diagnostics.Trace.WriteLine(e.ToString());
        }

        private void axRASplus_WatSear1_SegmentSpots(object sender, AxRASplus_WatSearLib._DRASplus_WatSearEvents_SegmentSpotsEvent e)
        {
            System.Diagnostics.Trace.WriteLine("axRASplus_WatSear1_SegmentSpots");
            System.Diagnostics.Trace.WriteLine(e.ToString());
        }

        private void axRASplus_WatSear1_SetNatType(object sender, AxRASplus_WatSearLib._DRASplus_WatSearEvents_SetNatTypeEvent e)
        {
            System.Diagnostics.Trace.WriteLine("axRASplus_WatSear1_SetNatType");
            System.Diagnostics.Trace.WriteLine(e.ToString());
        }

        private void axRASplus_WatSear1_StatusLoaded(object sender, AxRASplus_WatSearLib._DRASplus_WatSearEvents_StatusLoadedEvent e)
        {
            System.Diagnostics.Trace.WriteLine("axRASplus_WatSear1_StatusLoaded");
            System.Diagnostics.Trace.WriteLine(e.ToString());
        }

        private void axRASplus_WatSear1_TextInEventLoaded(object sender, AxRASplus_WatSearLib._DRASplus_WatSearEvents_TextInEventLoadedEvent e)
        {
            System.Diagnostics.Trace.WriteLine("axRASplus_WatSear1_TextInEventLoaded");
            System.Diagnostics.Trace.WriteLine(e.ToString());
        }

        private void axRASplus_WatSear1_WatchStatusLoadedIDR(object sender, AxRASplus_WatSearLib._DRASplus_WatSearEvents_WatchStatusLoadedIDREvent e)
        {
            System.Diagnostics.Trace.WriteLine("axRASplus_WatSear1_WatchStatusLoadedIDR");
            System.Diagnostics.Trace.WriteLine(e.ToString());
        }
    }

    public class IDISCameraControl : AxRASplus_WatSearLib.AxRASplus_WatSear
    {
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONDBLCLK = 0x0203;
        private const int WM_RBUTTONDOWN = 0x0204;

        public override ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return base.ContextMenuStrip;
            }
            set
            {
                base.ContextMenuStrip = value;
            }
        }

        public override ContextMenu ContextMenu
        {
            get
            {
                return base.ContextMenu;
            }
            set
            {
                base.ContextMenu = value;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN)
            {
                System.Diagnostics.Trace.WriteLine("LButtonDown");
            }
            else if (m.Msg == WM_RBUTTONDOWN)
            {
                int y = ((int)m.LParam >> 16);
                int x = ((int)m.LParam & 0xffff);
                System.Diagnostics.Trace.WriteLine("RButtonDown(" + x.ToString() + ", " + y.ToString() + ")");
            }
            else if (m.Msg == WM_LBUTTONDBLCLK)
            {
                int y = ((int)m.LParam >> 16);
                int x = ((int)m.LParam & 0xffff);
                System.Diagnostics.Trace.WriteLine("LButtonDoubleClick(" + x.ToString() + ", " + y.ToString() + ")");
            }

            base.WndProc(ref m);
        }
    }
}
