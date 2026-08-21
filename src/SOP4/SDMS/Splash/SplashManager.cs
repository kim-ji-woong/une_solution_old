using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility;
using System.Collections;
using System.IO;

namespace SDMS
{
    public class SplashManager
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, ref libSplash.COPYDATASTRUCT lParam);

        private class SplashData
        {
            private int x = 0, y = 0, height = 0;
            private DBUtility.VariousData<int> width = null;
            private float m_fontSize = 22.0f;
            private string m_strText = "";

            public int X
            {
                get { return x; }
                set { x = value; }
            }

            public int Y
            {
                get { return y; }
                set { y = value; }
            }

            public DBUtility.VariousData<int> Width
            {
                get { return width; }
                set { width = value; }
            }

            public int Height
            {
                get { return height; }
                set { height = value; }
            }

            public float FontSize
            {
                get { return m_fontSize; }
                set { m_fontSize = value; }
            }

            public string Text
            {
                get { return m_strText; }
                set { m_strText = value; }
            }

            public bool Parse(string strData)
            {
                string[] tokens = strData.Split(',');
                int nTokenCount = tokens.Count();

                if (nTokenCount < 6)
                    return false;

                if (nTokenCount == 6)
                {
                    if (int.TryParse(tokens[0].Trim(), out x) == false || int.TryParse(tokens[1].Trim(), out y) == false
                        || int.TryParse(tokens[3].Trim(), out height) == false)
                        return false;

                    if (float.TryParse(tokens[4].Trim(), out m_fontSize) == false)
                        return false;

                    int nWidth;

                    if (int.TryParse(tokens[2].Trim(), out nWidth))
                    {
                        width = new DBUtility.VariousData<int>(nWidth);
                    }

                    m_strText = tokens[5].Trim();
                }
                else
                {
                    int nBeginIndex = 0;

                    for (int i = 0; i < 5; i++)
                    {
                        int nIndex = strData.IndexOf(',', nBeginIndex);

                        string strToken = strData.Substring(nBeginIndex, nIndex - nBeginIndex).Trim();
                        nBeginIndex = nIndex + 1;

                        if (i == 0)
                        {
                            if (int.TryParse(strToken, out x) == false)
                                return false;
                        }
                        else if (i == 1)
                        {
                            if (int.TryParse(strToken, out y) == false)
                                return false;
                        }
                        else if (i == 2)
                        {
                            int nWidth;

                            if (int.TryParse(strToken, out nWidth) == true)
                                width = new DBUtility.VariousData<int>(nWidth);
                        }
                        else if (i == 3)
                        {
                            if (int.TryParse(strToken, out height) == false)
                                return false;
                        }
                        else if (i == 4)
                        {
                            if (float.TryParse(strToken, out m_fontSize) == false)
                                return false;
                        }
                    }

                    m_strText = strData.Substring(nBeginIndex).Trim();
                }

                return true;
            }
        }

        private List<SplashData> m_splashDatas = null;
        private IntPtr m_splashHandle = IntPtr.Zero;
        private string m_strTextColorOption = "";

        private int m_nSplashMessageHeader = -1;
        private string m_strSplashMessage = "";
        private int m_nTotalSplashMilliSeconds = 78000;
        private int m_nNextMilliSeconds = 0;

        private string m_strTimeLogFileName = "SplashTimeLog.dat";

        // Splash Handle이 생성되기 전에 만들어진 메시지
        // Header + "_" + NextMilliSeconds + "_" + Message
        private string m_strUnprocessedMessage = "";
        // 이전 데이터
        private Dictionary<string, int> m_dicTimeLogs = new Dictionary<string, int>();
        // 현재 데이터
        private Dictionary<string, int> m_dicCurrentTimeLogs = new Dictionary<string, int>();
        private string m_strCurrentKey = "";
        private DateTime m_dtInit = new DateTime();

        public IntPtr SplashHandle
        {
            get { return m_splashHandle; }
            set { m_splashHandle = value; }
        }

        public SplashManager(WebDBManager dbMgr, int nSiteID)
        {
            m_splashDatas = ReadSplashOption(ref m_strTextColorOption);
            LoadTimeLogFile();
        }

        private void LoadTimeLogFile()
        {
            if (File.Exists(m_strTimeLogFileName) == false)
                return;

            StreamReader reader = new StreamReader(m_strTimeLogFileName, Encoding.UTF8);

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                int nIndex1 = strLine.IndexOf(',');

                if (nIndex1 < 0)
                    continue;

                int nTime;

                if (strLine.StartsWith("Total"))
                {
                    string strTotalTime = strLine.Substring(nIndex1 + 1).Trim();

                    if (int.TryParse(strTotalTime, out nTime) == false)
                        continue;

                    m_nTotalSplashMilliSeconds = nTime;
                    continue;
                }

                int nIndex2 = strLine.IndexOf(',', nIndex1 + 1);

                if (nIndex2 < 0)
                    continue;

                string strHeader = strLine.Substring(0, nIndex1).Trim();
                string strNextMilliSeconds = strLine.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1).Trim();
                string strMessage = strLine.Substring(nIndex2 + 1).Trim();

                if (int.TryParse(strNextMilliSeconds, out nTime) == false)
                    continue;

                string strKey = strHeader + "_" + strMessage;
                m_dicTimeLogs[strKey] = nTime;
            }

            reader.Close();
        }

        private void SaveTimeLogFile()
        {
            if (m_dicCurrentTimeLogs.Count == 0)
                return;

            int nMax = 0;
            StreamWriter writer = new StreamWriter(m_strTimeLogFileName, false, Encoding.UTF8);

            foreach (KeyValuePair<string, int> pair in m_dicCurrentTimeLogs)
            {
                int nIndex = pair.Key.IndexOf('_');

                if (nIndex < 0)
                    continue;

                if (nMax < pair.Value)
                    nMax = pair.Value;

                string strHeader = pair.Key.Substring(0, nIndex);
                string strMessage = pair.Key.Substring(nIndex + 1);

                writer.WriteLine(strHeader + "," + pair.Value.ToString() + "," + strMessage);
            }

            writer.WriteLine("Total," + nMax.ToString());
            writer.Close();
        }

        private List<SplashData> ReadSplashOption(ref string strTextColorOption)
        {
            List<SplashData> splashDatas = new List<SplashData>();

            string strSQL = "Select PropertyValue from OptionSDMS where PropertyName = 'SplashText' and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount; i++)
            {
                string strPropertyValue = DBUtility.WebDBManager.GetStringField(arrResult[i]);

                if (strPropertyValue == null)
                    continue;

                string[] tokens = strPropertyValue.Split(';');

                foreach (string strToken in tokens)
                {
                    SplashData data = new SplashData();

                    if (data.Parse(strToken) == false)
                        return null;

                    splashDatas.Add(data);
                }
            }

            strSQL = "Select PropertyValue from OptionSDMS where PropertyName = 'SplashTextColor' and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();

            arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount; i++)
            {
                strTextColorOption = DBUtility.WebDBManager.GetStringField(arrResult[i]);
            }

            return splashDatas;
        }

        public void RunSplash()
        {
            if (m_splashDatas == null)
                return;

            string strPath = SetSplashText(m_strTextColorOption);

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = "libSplash.exe";
            startInfo.ErrorDialog = true;
            startInfo.Arguments = FormMain.Instance.Handle.ToInt64().ToString() + " " + System.Diagnostics.Process.GetCurrentProcess().Id.ToString() + " \"" + strPath + "\"";

            System.Diagnostics.Process process;
            try
            {
                process = System.Diagnostics.Process.Start(startInfo);
            }
            catch (Exception)
            {
                //System.Windows.Forms.MessageBox.Show(ex.Message);
            }

            System.Threading.Thread t = new System.Threading.Thread(SplashMessageThread);
            t.Start();
        }

        public void SendSplashMessage(string strMessage, int nHeader, int nNextMilliSeconds)
        {
            if (m_splashHandle == IntPtr.Zero)
            {
                m_strUnprocessedMessage = nHeader.ToString() + "_" + nNextMilliSeconds.ToString() + "_" + strMessage;
                return;
            }

            m_strSplashMessage = strMessage;
            m_nSplashMessageHeader = nHeader;

            string strKey = nHeader.ToString() + "_" + strMessage;
            int nTime;

            if (m_strCurrentKey.Length > 0)
            {
                // 경과시간
                int nCurrentMilliSeconds = (int)(DateTime.Now - m_dtInit).TotalMilliseconds;
                m_dicCurrentTimeLogs[m_strCurrentKey] = nCurrentMilliSeconds;
            }

            m_strCurrentKey = strKey;

            // 기존에 측정한 데이터가 있으면 기존값을 사용한다.
            if (m_dicTimeLogs.TryGetValue(strKey, out nTime))
                m_nNextMilliSeconds = nTime;
            else
                m_nNextMilliSeconds = nNextMilliSeconds;
        }

        public void SendSplashMessage(string strMessage, int nHeader)
        {
            if (m_splashHandle == IntPtr.Zero)
                return;

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strMessage);

            libSplash.COPYDATASTRUCT cds = new libSplash.COPYDATASTRUCT();

            cds.dwData = (IntPtr)nHeader;
            cds.cbData = bytes.Length + 1;
            cds.lpData = strMessage;

            SendMessage(m_splashHandle, libSplash.Message.WM_COPYDATA, 0, ref cds);

            if (nHeader == libSplash.Message.SPLASH_CLOSE)
            {
                if (m_strCurrentKey.Length > 0)
                {
                    // 경과시간
                    int nCurrentMilliSeconds = (int)(DateTime.Now - m_dtInit).TotalMilliseconds;
                    m_dicCurrentTimeLogs[m_strCurrentKey] = nCurrentMilliSeconds;
                }

                SaveTimeLogFile();
            }
        }

        private void ProcessUnprocessedMessage()
        {
            int nIndex1 = m_strUnprocessedMessage.IndexOf('_');

            if (nIndex1 < 0)
                return;

            int nIndex2 = m_strUnprocessedMessage.IndexOf('_', nIndex1 + 1);

            if (nIndex2 < 0)
                return;

            string strHeader = m_strUnprocessedMessage.Substring(0, nIndex1).Trim();
            string strTime = m_strUnprocessedMessage.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1).Trim();
            string strMessage = m_strUnprocessedMessage.Substring(nIndex2 + 1).Trim();

            int nHeader, nTime;
            
            if (int.TryParse(strHeader, out nHeader) == false || int.TryParse(strTime, out nTime) == false)
                return;

            SendSplashMessage(strMessage, nHeader, nTime);
            m_strUnprocessedMessage = "";
        }

        private bool ReadSplashHandle()
        {
            string strFileName = string.Format(libSplash.Message.SPLASH_HANLDE_FILE_NAME_FORMAT, System.Diagnostics.Process.GetCurrentProcess().Id);

            if (CanReadFile(strFileName) == false)
            //if (File.Exists(strFileName) == false)
                return false;

            StreamReader reader = new StreamReader(strFileName, Encoding.UTF8);
            string strHandle = reader.ReadLine().Trim();
            reader.Close();

            if (strHandle == null || strHandle.Length == 0)
                return false;

            File.Delete(strFileName);

            long splashHandle;

            if (long.TryParse(strHandle, out splashHandle) == false)
                return false;

            m_splashHandle = (IntPtr)splashHandle;
            return true;
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

        private void SplashMessageThread()
        {
            DateTime dtInit = DateTime.Now;
            m_dtInit = dtInit;

            int nCurrentMilliSeconds = 0, nSleepMilliSeconds = 100;
            string strPrevMessage = "";

            while (FormMain.Instance.Exit == false)
            {
                if (m_splashHandle == IntPtr.Zero)
                {
                    ReadSplashHandle();
                }

                if (m_strUnprocessedMessage.Length > 0 && m_splashHandle != IntPtr.Zero)
                {
                    ProcessUnprocessedMessage();
                }

                if (m_nSplashMessageHeader == libSplash.Message.SPLASH_CLOSE)
                {
                    SendSplashMessage(m_strSplashMessage, m_nSplashMessageHeader);
                    break;
                }

                // 경과시간
                nCurrentMilliSeconds = (int)(DateTime.Now - dtInit).TotalMilliseconds;

                if (nSleepMilliSeconds >= m_nNextMilliSeconds)
                {
                    if (strPrevMessage == m_strSplashMessage)
                        continue;
                }

                double percent = nCurrentMilliSeconds * 100.0 / m_nTotalSplashMilliSeconds;

                if (percent > 100.0)
                    percent = 100;

                string strPercent = string.Format(" ({0:F1}%)", percent);
                SendSplashMessage(m_strSplashMessage + strPercent, m_nSplashMessageHeader);
                strPrevMessage = m_strSplashMessage;

                System.Threading.Thread.Sleep(nSleepMilliSeconds);
            }
        }

        private string SetSplashText(string strSplashTextColorOption)
        {
            int nIndex = System.Windows.Forms.Application.ExecutablePath.LastIndexOf('\\');
            string strPath = nIndex < 0 ? System.Windows.Forms.Application.ExecutablePath + "\\splash.ini" : System.Windows.Forms.Application.ExecutablePath.Substring(0, nIndex) + "\\splash.ini";

            System.IO.StreamWriter writer = new System.IO.StreamWriter(strPath, false, System.Text.Encoding.UTF8);

            writer.WriteLine("[Common]");
            writer.WriteLine("CallerLocation : " + FormMain.Instance.Location.X.ToString() + "," + FormMain.Instance.Location.Y.ToString());

            if (strSplashTextColorOption != null)
                writer.WriteLine(strSplashTextColorOption);

            foreach (SplashData data in m_splashDatas)
            {
                writer.WriteLine("[Splash]");
                writer.WriteLine("TEXT : " + data.Text);

                if (data.Width != null)
                    writer.WriteLine("Rectangle : " + data.X.ToString() + "," + data.Y.ToString() + "," + data.Width.Data.ToString() + "," + data.Height.ToString());
                else
                    writer.WriteLine("Rectangle : " + data.X.ToString() + "," + data.Y.ToString() + ",," + data.Height.ToString());

                writer.WriteLine("Font : " + data.FontSize.ToString());
                /*libSplash.TextData text = new libSplash.TextData();
                text.Text = data.Text;
                text.Font = new Font(text.Font.FontFamily, data.FontSize);

                if (data.Width != null)
                    text.Rectangle = new Rectangle(data.X, data.Y, data.Width.Data, data.Height);
                else
                    text.Rectangle = new Rectangle(data.X, data.Y, m_frmSplash.Size.Width - data.X, data.Height);

                m_frmSplash.TextDatas.Add(text);*/
            }

            writer.Close();

            return strPath;
        }
    }
}
