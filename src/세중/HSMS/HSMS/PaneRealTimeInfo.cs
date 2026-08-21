using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace HSMS
{
    public partial class RealTimeInfoPane : Panel
    {
        public enum MessageType { OWN_MESSAGE = 0, RECV_MESSAGE, LOG_DATA, ETC };

        private Graphics m_Graphics = null;
        private Font m_font = null;
        private StringBuilder m_strDisplay = null;
        private Rectangle m_rect;
        private Label m_DisplayLabel = new Label();
        
        private int m_nBufferLength = 40;
        private int m_nMaxBufferLength = 44;
        private bool bFirst = true;
        
        private string m_strRealTimeInfo;
        public string RealTimeInfo
        {
            get { return m_strRealTimeInfo; }
            set 
            {
                m_strRealTimeInfo = value;                
            }
        }
		private Color m_TextColor;
		public System.Drawing.Color TextColor
		{
			get { return m_DisplayLabel.ForeColor; }
			set { 
				m_TextColor = value;
				m_DisplayLabel.ForeColor = m_TextColor;
			}
		}
        
        [System.Runtime.InteropServices.DllImport("User32.dll", EntryPoint = "SetParent", ExactSpelling = false)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndParent);

		public RealTimeInfoPane()
        {
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
			
            InitializeComponent();
			m_DisplayLabel.BackColor = Color.Transparent;
            //SetParent(this.Handle, frmParent.Handle);

            Graphics gp = CreateGraphics();

		
            m_DisplayLabel.Font = new Font("맑은 고딕",18,FontStyle.Bold);
            Font font = m_DisplayLabel.Font;

            StringBuilder str = new StringBuilder(34);
            for (int i = 0; i < 34; i++)
                str.Append(' ');
            Rectangle rect = CreateRect(gp, str, font);

			m_DisplayLabel.AutoSize = false;
            m_DisplayLabel.Parent = this;
            m_DisplayLabel.Size = new Size(rect.Width - 100, rect.Height);
            m_DisplayLabel.Location = new Point(90, 27);

            m_DisplayLabel.BackColor = Color.Transparent;
            m_DisplayLabel.BringToFront();

            //m_DisplayLabel.BackColor = BackColor;
            //m_DisplayLabel.BackColor = Color.Black;
            
            m_DisplayLabel.ForeColor = Color.White;
                      
            
            m_Graphics = gp;
            m_font = font;
            m_strDisplay = str;
            m_rect = rect;
            m_DisplayLabel.Font = m_font;

            //StartTimer();
        }        

        public void SetForeColor(MessageType type)
        {
            if (type == MessageType.OWN_MESSAGE)
                m_DisplayLabel.ForeColor = Color.Red;
            else if (type == MessageType.RECV_MESSAGE)
                m_DisplayLabel.ForeColor = Color.LightGreen;
            else if (type == MessageType.LOG_DATA)
                m_DisplayLabel.ForeColor = Color.Gold;
        }

        public void StartTimer()
        {
            m_TextScrollTimer.Enabled = true;
            m_TextScrollTimer.Start();
        }

        public void StopTimer()        
        {
            m_TextScrollTimer.Stop();
            m_TextScrollTimer.Enabled = false;

            if (m_Graphics != null)
                m_Graphics.Dispose();
        }


        public void DrawMovingText()
        {
            if( bFirst == true)
            {
                StartTimer();
                bFirst = false;
            }
            m_strDisplay.Clear();
            if (m_strRealTimeInfo != null && m_strRealTimeInfo != "")
            {
                char[] temp = m_strRealTimeInfo.ToCharArray();

                if (temp.Length > m_nBufferLength)
                {
                    m_nBufferLength = temp.Length;
                    m_nMaxBufferLength = temp.Length + 4;
                    m_strDisplay = new StringBuilder(m_nMaxBufferLength);
                }

                //if (m_nMaxBufferLength > temp.Length && m_nMaxBufferLength > 44)
                //{
                //    m_nBufferLength = 40;
                //    m_nMaxBufferLength = 44;
                //    m_strDisplay = new StringBuilder(m_nMaxBufferLength);
                //}

                for (int i = 0; i < temp.Length; i++)
                {
                    m_strDisplay.Append(temp[i]);
                }

                for (int i = 0; i < 4; i++)
                {
                    m_strDisplay.Append(' ');
                }

                if (temp.Length < m_nBufferLength)
                {
                    int nSpace = m_nBufferLength - temp.Length;
                    for (int i = 0; i < nSpace; i++)
                    {
                        m_strDisplay.Append(' ');
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

            return new Rectangle(x, y, /*w*/350, h);
        }

        private void TextScrollTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (m_strDisplay.Equals(""))
                    return;
                m_DisplayLabel.Text = m_strDisplay.ToString();
                Update();

                if (m_strDisplay.Length == 0)
                    return;
                // 텍스트 좌표를 변화시킨다. 
                if (m_strDisplay.Length >= 2 && (m_strDisplay[0] == ' ' && m_strDisplay[1] == ' '))
                {
                    m_strDisplay.Append(m_strDisplay[0]);
                    m_strDisplay.Remove(0, 1);
                    m_strDisplay.Append(m_strDisplay[0]);
                    m_strDisplay.Remove(0, 1);
                }
                else
                {
                    m_strDisplay.Append(m_strDisplay[0]);
                    m_strDisplay.Remove(0, 1);
                }

                // 15초 멈추면서 무빙효과 -> 여기서 sleep걸면 안됨(skkim)
                //Thread.Sleep(300);
            }
            catch (Exception)
            {
            }
        }

        private Bitmap m_ScrBitmap = null;
        private void FormRealTimeInfo_Paint(object sender, PaintEventArgs e)
        {
            if (sender != this)
            {
                try
                {
                    if (m_ScrBitmap == null)
                    {
                        m_ScrBitmap = new Bitmap(m_DisplayLabel.Size.Width, m_DisplayLabel.Size.Height);
                    }
                    this.DrawToBitmap(m_ScrBitmap, new Rectangle(0, 0, m_DisplayLabel.Size.Width, m_DisplayLabel.Size.Height));
                    e.Graphics.DrawImage((Image)m_ScrBitmap, 0, 0);
                }
                catch (Exception)
                {
                }
            }
        }

        private void FormRealTimeInfo_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (m_DisplayLabel.Size.Width > 0 && m_DisplayLabel.Size.Height > 0)
                    m_ScrBitmap = new Bitmap(m_DisplayLabel.Size.Width, m_DisplayLabel.Size.Height);
            }
            catch (Exception)
            {
            }
        }

        private void FormRealTimeInfo_Resize(object sender, EventArgs e)
        {
            if (this.Size.Width > 0 && this.Size.Height > 0)
            {
				m_DisplayLabel.Size = new Size(this.Size.Width - 100, m_DisplayLabel.Size.Height);
                if (m_Graphics != null)
                {
                    string str = "가";
                    Font font = m_DisplayLabel.Font;
                    int w = (int)m_Graphics.MeasureString(str.ToString(), font).Width;
                    int d = (int)(m_DisplayLabel.Width / w - 0.5);
                    int d2 = (int)(350 / w - 0.5);
                    int curLength = m_strDisplay.Length;
                    if (d > d2)
                    {
                        int nE = d - d2;
                        m_nBufferLength = 34 + nE * 2;
                        DrawMovingText();
                        
                    }
                    else
                    {
                        int nE = d2 - d;
                        if (curLength + nE > 34)
                        {
                            m_nBufferLength = 34 + nE * 2;
                            DrawMovingText();
                        }
                        else
                        {
                            m_nBufferLength = 34;
                            DrawMovingText();
                        }
                    }
                }
            }
        }

    }
}
