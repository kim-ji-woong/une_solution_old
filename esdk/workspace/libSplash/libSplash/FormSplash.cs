using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Collections;

namespace libSplash
{
    public partial class FormSplash : Form, GifPictureBoxOwner
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, ref libSplash.COPYDATASTRUCT lParam);

        private static FormSplash m_instance = null;
        public static FormSplash Instance
        {
            get { return m_instance; }
        }

        private List<TextData> m_textDatas = new List<TextData>();

        public Image SplashImage
        {
            get { return picBackground.Image; }
            set
            {
                picBackground.Image = value;

                if (value != null)
                {
                    this.Size = value.Size;
                }
            }
        }

        public List<TextData> TextDatas
        {
            get { return m_textDatas; }
        }

        public Font ProgressTextFont
        {
            get { return lblProgress.Font; }
            set { lblProgress.Font = value; }
        }

        public Point ProgressTextLocation
        {
            get { return lblProgress.Location; }
            set { lblProgress.Location = value; }
        }

        public Color ProgressTextColor
        {
            get { return lblProgress.ForeColor; }
            set { lblProgress.ForeColor = value; }
        }

        public string ProgressText
        {
            get { return lblProgress.Text; }
            set { lblProgress.Text = value; }
        }

        private IntPtr m_callWindowHandle = IntPtr.Zero;
        private Point m_callerLocation = new Point();
        private int m_callerProcessID = 0;

        private bool m_optGIF = false;

        public FormSplash(IntPtr callWindowHandle, int nCallerProcessID, string strInitFilePath)
        //public FormSplash(int nServerPort, string strInitFilePath)
        {
            InitializeComponent();
            m_instance = this;

            m_callerProcessID = nCallerProcessID;

            if (callWindowHandle != IntPtr.Zero)
            {
                m_callWindowHandle = callWindowHandle;

                /*string strMessage = "SplashHandle";
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strMessage);

                COPYDATASTRUCT cds = new COPYDATASTRUCT();
                cds.dwData = this.Handle;
                cds.lpData = "SplashHandle";
                cds.cbData = bytes.Length + 1;

                SendMessage(callWindowHandle, libSplash.Message.WM_COPYDATA, 0, ref cds);*/
                string strFileName = string.Format(Message.SPLASH_HANLDE_FILE_NAME_FORMAT, nCallerProcessID);

                StreamWriter writer = new StreamWriter(strFileName, false, Encoding.UTF8);
                writer.Write(this.Handle.ToInt64());
                writer.Close();
            }
            //m_nServerPort = nServerPort;

            picBackground.Image = global::libSplash.Properties.Resources.background;
            picBackground.Owner = this;

            lblProgress.Text = "";
            lblProgress.BackColor = Color.Transparent;
            lblProgress.Parent = picBackground;

            if (strInitFilePath != null)
            {
                ReadFile(strInitFilePath);
            }
        }

        private void ReadFile(string strPath)
        {
            StreamReader reader = new StreamReader(strPath, Encoding.UTF8);

            TextData data = null;
            string currentSection = "";

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                if (strLine.StartsWith("["))
                {
                    currentSection = strLine.ToLower();

                    if (currentSection == "[splash]")
                    {
                        data = new TextData();
                        m_textDatas.Add(data);
                        currentSection = strLine.ToLower();
                    }

                    continue;
                }

                int nIndex = strLine.IndexOf(':');

                if (nIndex < 0)
                    continue;

                string strProperty = strLine.Substring(0, nIndex).Trim().ToLower();
                string strValue = strLine.Substring(nIndex + 1).Trim();

                if (currentSection == "[common]")
                {
                    if (strProperty == "image")
                    {
                        if (ReadImage(strValue) == false)
                            continue;
                    }
                    else if (strProperty == "callerlocation")
                    {
                        if (ReadCallerLocation(strValue) == false)
                            continue;
                    }
                    else if (strProperty == "ref")
                    {
                        if (ReadRef(strValue) == false)
                            continue;
                    }
                    else if (strProperty == "fix")
                    {
                        if (ReadFix(strValue) == false)
                            continue;
                    }
                }
                else if (currentSection == "[splash]")
                {
                    if (data == null)
                        continue;

                    if (strProperty == "rectangle")
                    {
                        if (ReadRectangle(data, strValue) == false)
                            continue;
                    }
                    else if (strProperty == "font")
                    {
                        if (ReadFont(data, strValue) == false)
                            continue;
                    }
                    else if (strProperty == "text")
                    {
                        if (ReadText(data, strValue) == false)
                            continue;
                    }
                }
            }

            reader.Close();

            // 사용이 끝난 파일은 삭제한다.
            File.Delete(strPath);
        }

        private bool ReadFix(string strValue)
        {
            string[] tokens = strValue.Split(',');

            if (tokens.Count() != 3)
                return false;

            int r, g, b;

            if (int.TryParse(tokens[0].Trim(), out r) == false)
                return false;

            if (int.TryParse(tokens[1].Trim(), out g) == false)
                return false;

            if (int.TryParse(tokens[2].Trim(), out b) == false)
                return false;

            picBackground.SetFixTextColor(Color.FromArgb(r, g, b));
            return true;
        }

        private bool ReadRef(string strValue)
        {
            string[] tokens = strValue.Split(',');

            if (tokens.Count() != 2)
                return false;

            int x, y;

            if (int.TryParse(tokens[0].Trim(), out x) == false)
                return false;

            if (int.TryParse(tokens[1].Trim(), out y) == false)
                return false;

            picBackground.SetRefTextColor(x, y);
            return true;
        }

        private bool ReadCallerLocation(string strValue)
        {
            string[] tokens = strValue.Split(',');

            if (tokens.Count() != 2)
                return false;

            int x, y;
            
            if (int.TryParse(tokens[0].Trim(), out x) == false)
                return false;

            if (int.TryParse(tokens[1].Trim(), out y) == false)
                return false;

            m_callerLocation.X = x;
            m_callerLocation.Y = y;
            return true;
        }

        private bool ReadImage(string strValue)
        {
            try
            {
                Image image = Image.FromFile(strValue);
                SplashImage = image;
                picBackground.UseGIF = true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        private bool ReadText(TextData data, string strValue)
        {
            data.Text = strValue;
            return true;
        }

        private bool ReadFont(TextData data, string strValue)
        {
            string[] tokens = strValue.Split(',');
            int nTokenCount = tokens.Count();

            float fontHeight;
            int fontStyle;
            string strFontName = "";

            if (nTokenCount == 1)
            {
                if (float.TryParse(tokens[0].Trim(), out fontHeight) == false)
                    return false;

                data.Font = new Font(data.Font.Name, fontHeight, data.Font.Style);
            }
            else if (nTokenCount == 2)
            {
                if (float.TryParse(tokens[0].Trim(), out fontHeight) == false)
                    return false;

                strFontName = tokens[1].Trim();

                data.Font = new Font(strFontName, fontHeight, data.Font.Style);
            }
            else if (nTokenCount >= 3)
            {
                if (float.TryParse(tokens[0].Trim(), out fontHeight) == false)
                    return false;

                strFontName = tokens[1].Trim();

                if (int.TryParse(tokens[2].Trim(), out fontStyle) == false)
                    return false;

                data.Font = new Font(strFontName, fontHeight, (FontStyle)fontStyle);
            }

            return true;
        }

        private bool ReadRectangle(TextData data, string strValue)
        {
            string[] tokens = strValue.Split(',');

            if (tokens.Count() != 4)
                return false;

            int x, y, width, height;
            string strWidth = tokens[2].Trim();

            if (int.TryParse(tokens[0].Trim(), out x) == false)
                return false;

            if (int.TryParse(tokens[1].Trim(), out y) == false)
                return false;

            if (int.TryParse(tokens[3].Trim(), out height) == false)
                return false;

            if (strWidth.Length > 0)
            {
                if (int.TryParse(strWidth, out width) == false)
                    return false;

                data.Rectangle = new Rectangle(x, y, width, height);
            }
            else
                data.Rectangle = new Rectangle(x, y, picBackground.Size.Width - x, height);

            return true;
        }

        public void OnPostPaint(Graphics g, Color color)
        {
            foreach (TextData text in m_textDatas)
            {
                text.OnPaint(g, color);
                //text.OnPaint(g, nDrawingCount, nFrameCount);
            }
        }

        /*public void OnPostPaint(Graphics g, int nDrawingCount, int nFrameCount)
        {
            foreach (TextData text in m_textDatas)
            {
                text.OnPoint(g, nDrawingCount, nFrameCount);
            }
        }*/

        public void UpdateProgressText()
        {
            lblProgress.Update();
        }

        public new void Show(IWin32Window owner)
        {
            Point pt = new Point();

            if (owner != null && owner is Control)
            {
                pt = ((Control)owner).Location;
            }

            SetCenterLocation(pt);
            /*Rectangle rect = new Rectangle();

            if (Screen.AllScreens.Length == 0)
            {
                rect = Screen.AllScreens[0].WorkingArea;
            }
            else
            {
                Screen trgScreen = null;

                if (owner != null && owner is Control)
                {
                    Control ownerCtrl = (Control)owner;
                    Point pt = ownerCtrl.Location;

                    foreach (Screen screen in Screen.AllScreens)
                    {
                        if (pt.X >= screen.WorkingArea.Left && pt.X <= screen.WorkingArea.Right &&
                            pt.Y >= screen.WorkingArea.Top && pt.Y <= screen.WorkingArea.Bottom)
                        {
                            trgScreen = screen;
                            break;
                        }
                    }
                }

                if (trgScreen != null)
                    rect = trgScreen.WorkingArea;
                else
                    rect = Screen.AllScreens[0].WorkingArea;
            }

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(rect.Left + (rect.Width - this.Width) / 2, rect.Top + (rect.Height - this.Height) / 2);*/
            base.Show(owner);
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == libSplash.Message.WM_COPYDATA)
            {
                COPYDATASTRUCT cds = (COPYDATASTRUCT)m.GetLParam(typeof(COPYDATASTRUCT));
                OnCopyData(cds);
                return;
            }

            base.WndProc(ref m);
        }

        private void OnCopyData(COPYDATASTRUCT cds)
        {
            int header = cds.dwData.ToInt32();

            if (header == libSplash.Message.SPLASH_CLOSE)
                this.Close();
            else if (header == libSplash.Message.SPLASH_MESSAGE)
            {
                ProgressText = cds.lpData;
            }
        }

        private void FormSplash_Load(object sender, EventArgs e)
        {
            SetCenterLocation(m_callerLocation);

            if (picBackground.UseGIF == false)
                picBackground.Run();

            timer1.Start();
        }

        private void SetCenterLocation(Point pt)
        {
            Rectangle rect = new Rectangle();

            if (Screen.AllScreens.Length == 0)
            {
                rect = Screen.AllScreens[0].WorkingArea;
            }
            else
            {
                Screen trgScreen = null;

                foreach (Screen screen in Screen.AllScreens)
                {
                    if (pt.X >= screen.WorkingArea.Left && pt.X <= screen.WorkingArea.Right &&
                        pt.Y >= screen.WorkingArea.Top && pt.Y <= screen.WorkingArea.Bottom)
                    {
                        trgScreen = screen;
                        break;
                    }
                }

                if (trgScreen != null)
                    rect = trgScreen.WorkingArea;
                else
                    rect = Screen.AllScreens[0].WorkingArea;
            }

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(rect.Left + (rect.Width - this.Width) / 2, rect.Top + (rect.Height - this.Height) / 2);
        }

        // Splash Window를 호출한 process가 비정상적으로 종료되지 않았는지 검사하기 위한 Timer
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_callerProcessID == 0)
                return;

            Process process = null;

            try
            {
                process = Process.GetProcessById(m_callerProcessID);
            }
            catch (Exception)
            {
                process = null;
            }

            if (process == null)
            {
                timer1.Stop();
                this.Close();
            }
        }

        /*public void OnReceive(short header, ArrayList arrDatas)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (header == libSplash.Message.SPLASH_CLOSE)
                    this.Close();
                else if (header == libSplash.Message.SPLASH_MESSAGE)
                {
                    if (arrDatas != null && arrDatas.Count > 0 && arrDatas[0] is string)
                    {
                        string strMessage = (string)arrDatas[0];
                        ProgressText = strMessage;
                    }
                }
            });
        }

        public void OnDropConnection()
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.Close();
            });
        }*/
    }
}
