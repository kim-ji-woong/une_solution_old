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
using System.Runtime.InteropServices;
using System.Collections;

namespace SplashSample
{
    public partial class FormMain : Form
    {
        private int m_nTimeLimit = 0;
        private int m_nTimeCount = 0;

        private IntPtr m_splashHandle = IntPtr.Zero;
        
        public FormMain()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            string strSeconds = textBoxSeconds.Text.Trim();

            if (strSeconds.Length == 0)
            {
                textBoxSeconds.Focus();
                MessageBox.Show("지속시간을 입력하세요.");
                return;
            }

            int nSeconds;

            if (int.TryParse(strSeconds, out nSeconds) == false || nSeconds <= 0)
            {
                textBoxSeconds.Focus();
                MessageBox.Show("지속시간은 0보다 큰 정수로 입력해야 합니다.");
                return;
            }

            m_nTimeCount = 0;
            m_nTimeLimit = nSeconds;

            RunSplash();
            /*m_frmSplash = new FormSplash();
            SetSplashText();
            m_frmSplash.Show(this);*/

            timer1.Start();
        }

        private void RunSplash()
        {
            //m_splashServer = new TcpLib2.TcpServer(m_splashProvider, libSplash.Message.PORT);
            //m_splashServer.Start();

            string strPath = MakeSplashFile();

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = "libSplash.exe";
            //startInfo.WorkingDirectory = @"..\..\..\..\bin\Common12";
            startInfo.ErrorDialog = true;
            startInfo.Arguments = this.Handle.ToInt64().ToString() + " " + System.Diagnostics.Process.GetCurrentProcess().Id.ToString() + " \"" + strPath + "\"";

            System.Diagnostics.Process process;
            try
            {
                process = System.Diagnostics.Process.Start(startInfo);
            }
            catch (Exception)
            {
                //System.Windows.Forms.MessageBox.Show(ex.Message);
            }

            System.Threading.Thread t = new System.Threading.Thread(ReadSplashHandleThread);
            t.Start();
        }

        private void ReadSplashHandleThread()
        {
            string strFileName = string.Format(libSplash.Message.SPLASH_HANLDE_FILE_NAME_FORMAT, System.Diagnostics.Process.GetCurrentProcess().Id);

            while (CanReadFile(strFileName) == false)
            {
                System.Threading.Thread.Sleep(50);
            }

            StreamReader reader = new StreamReader(strFileName, Encoding.UTF8);
            string strHandle = reader.ReadLine().Trim();
            reader.Close();

            if (strHandle == null || strHandle.Length == 0)
                return;

            File.Delete(strFileName);

            long splashHandle;

            if (long.TryParse(strHandle, out splashHandle) == false)
                return;

            m_splashHandle = (IntPtr)splashHandle;
        }

        private bool CanReadFile(string strFilePath)
        {
            FileInfo file = new FileInfo(strFilePath);
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return false;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return true;
        }

        private string MakeSplashFile()
        {
            int nIndex = Application.ExecutablePath.LastIndexOf('\\');
            string strPath = nIndex < 0 ? Application.ExecutablePath + "\\splash.ini" : Application.ExecutablePath.Substring(0, nIndex) + "\\splash.ini";

            string strGIFFolder = nIndex < 0 ? Application.ExecutablePath + "\\" : Application.ExecutablePath.Substring(0, nIndex) + "\\";

            StreamWriter writer = new StreamWriter(strPath, false, Encoding.UTF8);

            writer.WriteLine("[Common]");
            writer.WriteLine("CallerLocation : " + this.Location.X.ToString() + "," + this.Location.Y.ToString());
            writer.WriteLine("REF : 119, 123");

            if (radioOrange.Checked)
            {
                writer.WriteLine("Image : " + strGIFFolder + "주황색.gif");
            }
            else if (radioGreen.Checked)
            {
                writer.WriteLine("Image : " + strGIFFolder + "엑셀-색.gif");
            }
            else if (radioBlue.Checked)
            {
                writer.WriteLine("Image : " + strGIFFolder + "밝은-파랑.gif");
            }
            else if (radioDarkBlue.Checked)
            {
                writer.WriteLine("Image : " + strGIFFolder + "남색.gif");
            }

            if (radioEDisaster.Checked)
            {
                writer.WriteLine("[Splash]");
                writer.WriteLine("TEXT : e-재난 시스템");
                writer.WriteLine("Rectangle : 178,94,,40");
                writer.WriteLine("Font : 22.0");
            }
            else
            {
                writer.WriteLine("[Splash]");
                writer.WriteLine("TEXT : SMART");
                writer.WriteLine("Rectangle : 178,84,,40");
                writer.WriteLine("Font : 22.0");

                writer.WriteLine("[Splash]");
                writer.WriteLine("TEXT : 재난관리 시스템");
                writer.WriteLine("Rectangle : 178,124,,40");
                writer.WriteLine("Font : 18.0, 맑은 고딕, " + ((int)FontStyle.Bold).ToString());
            }

            writer.Close();
            return strPath;
        }

        //private void SetSplashText()
        //{
        //    //TextData textUp = new TextData(" E-재난", 198, 84, m_frmSplash.Size.Width - 198, 40);
        //    //TextData textDown = new TextData("SYSTEM", 198, 114, m_frmSplash.Size.Width - 198, 40);
        //    TextData textUp = new TextData("SMART", 198, 84, m_frmSplash.Size.Width - 198, 40);
        //    TextData textDown = new TextData("재난관리 시스템", 198, 114, m_frmSplash.Size.Width - 198, 40);
        //    textDown.Font = new System.Drawing.Font("맑은 고딕", 18.0f, FontStyle.Bold);
        //    textDown.Rectangle = new Rectangle(198, 124, m_frmSplash.Size.Width - 198, 40);

        //    m_frmSplash.TextDatas.Clear();
        //    m_frmSplash.TextDatas.Add(textUp);
        //    m_frmSplash.TextDatas.Add(textDown);
        //    /*TextData textUp = new TextData("SMART", 198, 74, m_frmSplash.Size.Width - 198, 40);
        //    textUp.Font = new System.Drawing.Font("맑은 고딕", 20.0f, FontStyle.Bold);
        //    TextData textDown = new TextData("재난관리", 198, 114, m_frmSplash.Size.Width - 198, 40);
        //    textDown.Font = new System.Drawing.Font("맑은 고딕", 16.0f, FontStyle.Bold);
        //    textDown.Rectangle = new Rectangle(198, 108, m_frmSplash.Size.Width - 198, 40);

        //    TextData textDown2 = new TextData("시스템", 198, 144, m_frmSplash.Size.Width - 198, 40);
        //    textDown2.Font = new System.Drawing.Font("맑은 고딕", 16.0f, FontStyle.Bold);
        //    textDown2.Rectangle = new Rectangle(198, 135, m_frmSplash.Size.Width - 198, 40);

        //    m_frmSplash.TextDatas.Clear();
        //    m_frmSplash.TextDatas.Add(textUp);
        //    m_frmSplash.TextDatas.Add(textDown);
        //    m_frmSplash.TextDatas.Add(textDown2);*/
        //}

        private void timer1_Tick(object sender, EventArgs e)
        {
            ++m_nTimeCount;

            if (m_nTimeCount < m_nTimeLimit)
            {
                SendSplashMessage(string.Format("{0} / {1} 처리중...", m_nTimeCount, m_nTimeLimit));
                //m_frmSplash.ProgressText = string.Format("{0} / {1} 처리중...", m_nTimeCount, m_nTimeLimit);
                //m_frmSplash.UpdateProgressText();
            }
            else if (m_nTimeCount == m_nTimeLimit)
            {
                SendSplashMessage("처리완료");
                //m_frmSplash.ProgressText = string.Format("처리완료");
                //m_frmSplash.UpdateProgressText();
            }
            else
            {
                timer1.Stop();
                SendSplashMessage("", libSplash.Message.SPLASH_CLOSE);
                //m_frmSplash.Close();
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, ref libSplash.COPYDATASTRUCT lParam);

        /*protected override void WndProc(ref Message m)
        {
            if (m.Msg == libSplash.Message.WM_COPYDATA)
            {
                libSplash.COPYDATASTRUCT cds = (libSplash.COPYDATASTRUCT)m.GetLParam(typeof(libSplash.COPYDATASTRUCT));

                if (cds.lpData.ToLower() == "splashhandle")
                    m_splashHandle = cds.dwData;

                return;
            }

            base.WndProc(ref m);
        }*/

        private void SendSplashMessage(string strMessage, int nCode = libSplash.Message.SPLASH_MESSAGE)
        {
            if (m_splashHandle == IntPtr.Zero)
                return;

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strMessage);

            libSplash.COPYDATASTRUCT cds = new libSplash.COPYDATASTRUCT();

            cds.dwData = (IntPtr)nCode;
            cds.cbData = bytes.Length + 1;
            cds.lpData = strMessage;

            SendMessage(m_splashHandle, libSplash.Message.WM_COPYDATA, 0, ref cds);
        }
    }
}
