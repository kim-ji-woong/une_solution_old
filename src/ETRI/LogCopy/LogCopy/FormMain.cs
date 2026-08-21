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

namespace LogCopy
{
    public partial class FormMain : Form
    {
        private int m_nYear = -1, m_nMonth = -1, m_nDay = -1;
        private List<string> m_lines = new List<string>();
        private int m_nPrevHour = -1, m_nPrevMinute = -1, m_nPrevSecond = -1, m_nPrevMillsecond = -1;

        public FormMain()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime dtNow = DateTime.Now;

            if (dtNow.Hour == 1 && dtNow.Minute == 0 && dtNow.Second == 0)
            {
                // 1주일 이상 경과된 로그 파일은 삭제한다.
                DateTime dtRemoveDate = dtNow.AddDays(-7.0);
                RemoveLogs(dtRemoveDate.Year, dtRemoveDate.Month, dtRemoveDate.Day);
            }

            StreamWriter writer = null;

            // 날짜가 바뀌면 리스트를 초기화한다.
            if (dtNow.Year != m_nYear || dtNow.Month != m_nMonth || dtNow.Day != m_nDay)
            {
                m_lines.Clear();

                m_nPrevHour = -1;
                m_nPrevMinute = -1;
                m_nPrevSecond = -1;
                m_nPrevMillsecond = -1;
            }

            string strFolder = Application.StartupPath;
            string strFileName = strFolder + string.Format("\\App_Data\\log_{0}_{1:00}_{2}.txt", dtNow.Year, dtNow.Month, dtNow.Day);

            if (!File.Exists(strFileName))
                return;

            try
            {
                DateTime dt = File.GetLastWriteTime(strFileName);

                // 이전에 읽었던 파일과 같은 시간대이면 파일이 변하지 않은것으로 간주한다.
                if (dt.Hour == m_nPrevHour && dt.Minute == m_nPrevMinute && dt.Second == m_nPrevSecond && dt.Millisecond == m_nPrevMillsecond)
                    return;

                m_nPrevHour = dt.Hour;
                m_nPrevMinute = dt.Minute;
                m_nPrevSecond = dt.Second;
                m_nPrevMillsecond = dt.Millisecond;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return;
            }

            StreamReader reader = new StreamReader(strFileName, Encoding.UTF8);

            int nLineCount = 0;
            int nPrevCount = m_lines.Count;

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine();

                if (nLineCount++ < nPrevCount)
                    continue;

                if (writer == null)
                {
                    if (dtNow.Year != m_nYear || dtNow.Month != m_nMonth || dtNow.Day != m_nDay)
                    {
                        writer = new StreamWriter("log.txt", false, Encoding.UTF8);

                        m_nYear = dtNow.Year;
                        m_nMonth = dtNow.Month;
                        m_nDay = dtNow.Day;
                    }
                    else
                    {
                        writer = new StreamWriter("log.txt", true, Encoding.UTF8);
                    }
                }

                writer.WriteLine(strLine);
                writer.Flush();

                m_lines.Add(strLine);
            }

            reader.Close();

            if (writer != null)
                writer.Close();
        }

        // 입력된 날짜 및 그 이전에 생성된 로그파일들은 모두 삭제한다.
        private void RemoveLogs(int nYear, int nMonth, int nDay)
        {
            string strFolder = Application.StartupPath + "\\App_Data";
            string[] files = Directory.GetFiles(strFolder);

            int year, month, day;
            string strBeginTag = "log_", strEndTag = ".txt";
            int nFolderLength = strFolder.Length;

            foreach (string strFile in files)
            {
                string strFileName = strFile.Substring(nFolderLength + 1).ToLower();

                if (strFileName.StartsWith(strBeginTag) && strFileName.EndsWith(strEndTag))
                {
                    string strDate = strFileName.Substring(strBeginTag.Length, strFileName.Length - strBeginTag.Length - strEndTag.Length);
                    string[] tokens = strDate.Split('_');

                    if (tokens.Count() != 3)
                        continue;

                    if (!int.TryParse(tokens[0], out year) || !int.TryParse(tokens[1], out month) || !int.TryParse(tokens[2], out day))
                        continue;

                    if (year > nYear)
                        continue;
                    else if (year == nYear)
                    {
                        if (month > nMonth)
                            continue;
                        else if (month == nMonth)
                        {
                            if (day > nDay)
                                continue;
                        }
                    }

                    File.Delete(strFile);
                }
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }
    }
}
