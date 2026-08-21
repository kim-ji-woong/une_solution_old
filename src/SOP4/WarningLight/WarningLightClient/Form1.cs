using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using DBUtility;

namespace WarningLightClient
{
    public struct USB_INPUT     // UIO 입력 패킷으로부터 데이타를 얻기 위한 구조체 
    {
        public int ProductID;   // 장치 ID 
        public Byte Status;     // 패킷 수신 상태값  0=입력 변화에 의한 수신, 1=데이타 재전송 요구에 의한 수신 
        public Byte Button;     // 입력 버턴값
        public Byte Output;     // USB 장치의 입출력 상태값
        public Byte Mask;       // 포트의 입출력 설정값. bit값이 '0'이면 출력, '1'이면 입력
    };

    public partial class Form1 : Form
    {
        /// <summary>
        /// uio.dll을 사용하기 위한 선언부입니다.
        /// </summary>
        /// 
        [DllImport("uio.dll")]
        private static extern int usb_io_init(int pID);
        [DllImport("uio.dll")]
        private static extern void set_usb_events(int hWnd);
        [DllImport("uio.dll")]
        private static extern void get_usb_input(int lParam, ref USB_INPUT uInput);
        [DllImport("uio.dll")]
        private static extern bool usb_io_output(int pID, int cmd, int io1, int io2, int io3, int io4);
        [DllImport("uio.dll")]
        private static extern bool usb_io_reset(int pID);
        [DllImport("uio.dll")]
        private static extern bool usb_in_request(int pID);
        /// 여기까지 uio.dll을 사용하기 위한 선언부
        /// 
        public Form1()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;

            this.ContextMenuStrip = null;

            notifyIcon1.Visible = true;
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
        }

        private void Iconize()
        {
            //this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
            this.notifyIcon1.Visible = true;
        }

        private void Normalize()
        {
            this.ShowInTaskbar = true;
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.notifyIcon1.Visible = false;

            this.BringToFront();
        }

        private void 시작ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private bool m_bExitProgram = false;
        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("이 프로그램을 종료하시면 경광등이 꺼집니다. \n종료하시겠습니까?", "종료알림", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                // Save info
                m_bExitProgram = true;
                this.Close();
            }
        }

        private void 열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                Iconize();
            }
            else
            {
                Normalize();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            set_usb_events(this.Handle.ToInt32());  // USB로부터 입력 패킷이 수신 되었을 때 WM_INPUT 이벤트가 발생하로록 설정
            button1_Click(null, null);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_bExitProgram == false)
            {
                e.Cancel = true;
                Iconize();
                return;
            }
            OffWarnLight();
         

            this.notifyIcon1.Visible = false;


        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Iconize();
            }
        }

        private void 감시중ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
            {
                timer1.Stop();
                timer1.Enabled = false;
            }
            OffWarnLight();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
            {
                timer1.Stop();
                timer1.Enabled = false;
            }
            
            timer1.Interval = 800;
            timer1.Enabled = true;
            timer1.Start();
        }



        private void button2_Click(object sender, EventArgs e)
        {
            usb_io_output(0x261, 0, 1, 0, 0, 0);
            System.Threading.Thread.Sleep(1000);
            usb_io_output(0x261, 0, -1, 0, 0, 0);
        }

        private void OffWarnLight()
        {
            BuzzarBlink(0x261, 0, 5, 5);
            WarnLight(0x261, 0, 2, 5, 5);
            WarnLight(0x261, 0, 3, 5, 5);
            WarnLight(0x261, 0, 4, 5, 5);
        }

        private bool m_bRingBuzzur = false;
        private bool BuzzarBlink(int nID, int nOnOff, int nHiLow, int nTime)
        {
            bool result = false;
            if (nOnOff == 1)
            {
                int blink = nHiLow * 16 + nTime;
                result = usb_io_output(nID, blink, 1, 0, 0, 0);
                m_bRingBuzzur = result;
            }
            else
            {
                result = usb_io_output(nID, 0, -1, 0, 0, 0);
                m_bRingBuzzur = result;
            }
            return result;
        }

        private bool WarnLight(int nID, int nOnOff, int p2, int nTime, int nHiLow)
        {
            bool result = false;
            if (nOnOff == 1)
            {
                int blink = nHiLow * 16 + nTime;
                result = usb_io_output(nID, blink, p2, 0, 0, 0);
                m_bRingBuzzur = result;
            }
            else
            {
                result = usb_io_output(nID, 0, -p2, 0, 0, 0);
                m_bRingBuzzur = result;
            }
            return result;
        }

        private bool WarnLightNoBlink(int nID, int nOnOff, int p2, int nTime, int nHiLow)
        {
            bool result = false;
            if (nOnOff == 1)
            {
                int blink = nHiLow * 16 + nTime;
                result = usb_io_output(nID, 0, p2, 0, 0, 0);
                m_bRingBuzzur = result;
            }
            else
            {
                result = usb_io_output(nID, 0, -p2, 0, 0, 0);
                m_bRingBuzzur = result;
            }
            return result;
        }

        DBUtility.WebDBManager dbMgr = new DBUtility.WebDBManager(3);
        private void timer1_Tick(object sender, EventArgs e)
        {
            string szSQL = "SELECT ID, CH1, CH2, CH3, CH4, CH5, Time, HiLow, Description FROM WarningLight";
            ArrayList arResult = dbMgr.GetResultData(szSQL, 0);
            if (arResult != null && arResult.Count > 0)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arResult[0].ToString(), -1);
                int nCh1 = DBUtility.WebDBManager.GetIntField(arResult[1].ToString(), -1);
                int nCh2 = DBUtility.WebDBManager.GetIntField(arResult[2].ToString(), -1);
                int nCh3 = DBUtility.WebDBManager.GetIntField(arResult[3].ToString(), -1);
                int nCh4 = DBUtility.WebDBManager.GetIntField(arResult[4].ToString(), -1);
                int nCh5 = DBUtility.WebDBManager.GetIntField(arResult[5].ToString(), -1);

                int nTime = DBUtility.WebDBManager.GetIntField(arResult[6].ToString(), 5);
                int nHiLow = DBUtility.WebDBManager.GetIntField(arResult[7].ToString(), 5);

                BuzzarBlink(nID, nCh1, nTime, nHiLow);

                if (nCh2 == 1)
                {
                    WarnLight(nID, 1, 2, nTime, nHiLow);
                }
                else if (nCh3 == 1)
                {
                    WarnLight(nID, 1, 3, nTime, nHiLow);
                }
                else if (nCh4 == 1)
                {
                    WarnLightNoBlink(nID, 1, 4, nTime, nHiLow);
                }
                else
                {
                    WarnLight(nID, 0, 2, nTime, nHiLow);
                    WarnLight(nID, 0, 3, nTime, nHiLow);
                    WarnLight(nID, 0, 4, nTime, nHiLow);

                }
            }
        }

    }

    public class MessageFilter : System.Windows.Forms.IMessageFilter
    {
        [DllImport("uio.dll")]
        private static extern void get_usb_input(int lParam, ref USB_INPUT uInput);

        const int WM_CREATE = 1;
        const int WM_INPUT = 255;
        const int WM_WM_DEVICECHANGE = 537;

        static int cnt;

        USB_INPUT uInput = new USB_INPUT();
        Form1 frm = null;

        public Form1 UIO_FORM
        {
            set
            {
                frm = value;
            }
            get
            {
                return frm;
            }
        }

        public bool PreFilterMessage(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_CREATE:
                    break;

                case WM_INPUT:              // USB로부터 입력 패킷이 수신 되었을 때 발생하는 이벤트 
                    get_usb_input(m.LParam.ToInt32(), ref uInput);                    
                    break;

                case WM_WM_DEVICECHANGE:    // USB 장치가 연결되거나 분리될 때 발생하는 이벤트 
                    break;
            }
            return false;
        }
    }
}
