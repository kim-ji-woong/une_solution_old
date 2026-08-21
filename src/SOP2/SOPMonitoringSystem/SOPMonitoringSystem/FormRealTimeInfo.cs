using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace SOPMonitoringSystem
{
    public partial class FormRealTimeInfo : Form
    {
        public enum MessageType { OWN_MESSAGE = 0, RECV_MESSAGE, LOG_DATA, ETC };

        private Graphics m_gp;
        private Font m_font;
        private StringBuilder m_str;
        private Rectangle m_rect;
        private Label label = new Label();
        
        private string m_strRealTimeInfo;
        public string RealTimeInfo
        {
            get { return m_strRealTimeInfo; }
            set {
                m_strRealTimeInfo = value;
                SOPLog log = FormMain.Instance.LogFile;
                log.Write(string.Format("SetRealTimeInfo, value : {0}\n", value));
            }
        }
        
        [System.Runtime.InteropServices.DllImport("User32.dll", EntryPoint = "SetParent", ExactSpelling = false)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndParent);

        public FormRealTimeInfo(FormMain frmParent)
        {
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            InitializeComponent();
 
            SetParent(this.Handle, frmParent.Handle);

            Graphics gp = CreateGraphics();
            Font font = new Font("맑은고딕", 26, FontStyle.Bold);

            StringBuilder str = new StringBuilder(66);
            for (int i = 0; i < 66; i++)
                str.Append(' ');
            Rectangle rect = CreateRect(gp, str, font);

            label.Parent = this;
            label.Size = new Size(rect.Width, rect.Height);
            label.Location = new Point(30, 20);

            label.BackColor = Color.Transparent;
            label.BringToFront();
            label.BackColor = Color.Black;
            label.ForeColor = Color.Gold;
            
            
            m_gp = gp;
            m_font = font;
            m_str = str;
            m_rect = rect;

            StartTimer();
        }
        
        /*public void SetForeColor(bool isFlag)
        {
            if (isFlag)
                label.ForeColor = Color.Red;
            else
                label.ForeColor = Color.Gold;
        }*/

        public void SetForeColor(MessageType type)
        {
            if (type == MessageType.OWN_MESSAGE)
                label.ForeColor = Color.Red;
            else if (type == MessageType.RECV_MESSAGE)
                label.ForeColor = Color.LightGreen;
            else if (type == MessageType.LOG_DATA)
                label.ForeColor = Color.Gold;
        }

        public void StartTimer()
        {
            timer1.Start();
        }

        public void StopTimer()
        {
            if (m_gp != null)
                m_gp.Dispose();
            //timer1.Stop();
        }

        int m_nBufferLength = 40;
        int m_nMaxBufferLength = 44;
        bool bFirst = true;
        public void DrawMovingText()
        {
            if( bFirst == true)
            {
                StartTimer();
                bFirst = false;
            }
            m_str.Clear();
            if (m_strRealTimeInfo != null && m_strRealTimeInfo != "")
            {
                char[] temp = m_strRealTimeInfo.ToCharArray();

                if (temp.Length > m_nBufferLength)
                {
                    m_nBufferLength = temp.Length;
                    m_nMaxBufferLength = temp.Length + 4;
                    m_str = new StringBuilder(m_nMaxBufferLength);
                }

                if (m_nMaxBufferLength > temp.Length && m_nMaxBufferLength > 44)
                {
                    m_nBufferLength = 40;
                    m_nMaxBufferLength = 44;
                    m_str = new StringBuilder(m_nMaxBufferLength);
                }

                for (int i = 0; i < temp.Length; i++)
                {
                    m_str.Append(temp[i]);
                }

                for (int i = 0; i < 4; i++)
                {
                    m_str.Append(' ');
                }

                if (temp.Length < 40)
                {
                    int nSpace = 40 - temp.Length;
                    for (int i = 0; i < nSpace; i++)
                    {
                        m_str.Append(' ');
                    }
                }
            }
        }

        private Rectangle CreateRect(Graphics grfx, StringBuilder str, Font font)
        {
            int w = (int)grfx.MeasureString(str.ToString(), font).Width + 5; // +5 to allow last char to fit
            int h = (int)grfx.MeasureString(str.ToString(), font).Height;
            int x = 30;// (int)this.Width / 2 - w / 2;
            int y = 20; //(int)this.Height / 2 - h;

            return new Rectangle(x, y, /*w*/700, h);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                label.Font = m_font;
                label.Text = m_str.ToString();

                Refresh();

                // 텍스트 좌표를 변화시킨다. 
                if (m_str.Length >= 2 && (m_str[0] == ' ' && m_str[1] == ' '))
                {
                    m_str.Append(m_str[0]);
                    m_str.Remove(0, 1);
                    m_str.Append(m_str[0]);
                    m_str.Remove(0, 1);
                }
                else
                {
                    m_str.Append(m_str[0]);
                    m_str.Remove(0, 1);
                }

                // 15초 멈추면서 무빙효과
                //Thread.Sleep(300);
            }
            catch (Exception)
            {
            }
        }

        private void FormRealTimeInfo_Paint(object sender, PaintEventArgs e)
        {
            if (sender != this)
            {
                try
                {
                    Bitmap bitmap = new Bitmap(label.Size.Width, label.Size.Height);
                    this.DrawToBitmap(bitmap, new Rectangle(0, 0, label.Size.Width, label.Size.Height));
                    e.Graphics.DrawImage((Image)bitmap, 0, 0);
                }
                catch (Exception)
                {

                }
            }
        }

    }
}
