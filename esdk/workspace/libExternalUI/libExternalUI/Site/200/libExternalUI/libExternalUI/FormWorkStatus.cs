using DBUtility2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace libExternalUI
{
    public partial class FormWorkStatus : Form
    {
        public enum DBType { sqlserver = 0, mysql, TypeCount };
        private DirectDBManager m_DBManager;

        private string m_strAccessSetPath = "AccessSet.txt";
        private string m_strAccessSetTempPath = "AccessSet_Temp.txt";
        private string m_strConfigPath = "config.ini";

        private System.Windows.Forms.Timer m_timer;

        private List<int> m_listAccessFloor = new List<int>();

        private bool m_bDBConnect = false;

        // Form Move 를 위한 Panel Top 클릭 체크
        private bool m_bLeftMouseDown = false;
        // Form Move 를 위한 Panel Top 클릭 점
        private Point m_ptMove;

        public FormWorkStatus()
        {
            InitializeComponent();

            InitDBSet();
            ReadAccessCount();
            SetWorkStatus();
            InitTimer();
        }

        private void InitDBSet()
        {
            string strWebServerURL = UIManager.Instance.GetWebServerURL();
            string strWebServerIP = strWebServerURL.Replace("http://", "").Trim();
            //string strID = "event_user";
            string strID = "sa";
            //string strPW = "event1234!";
            string strPW = "9449966Ab";
            string strDBName = "SH_ExportEvent";

            m_DBManager = DirectDBManager.MakeInstance((DBUtility2.DirectDBManager.DBType)DBType.sqlserver, strWebServerIP, strID, strPW, strDBName);

            if (m_DBManager.Connect())
            {
                m_bDBConnect = true;
                m_DBManager.Close();
            }
        }

        private void InitTimer()
        {
            m_timer = new System.Windows.Forms.Timer();
            m_timer.Interval = 1000;

            m_timer.Tick += (s, e) =>
            {
                ReadAccessCount();
                SetWorkStatus();
                CleanDB();
            };

            m_timer.Start();
        }

        private void ReadAccessCount()
        {
            if (m_bDBConnect == false)
                return;

            if (m_DBManager.Connect() == true)
            {
                string strLimitDate = LoadLimitDate();
                List<int> listFloorInOut = new List<int>();

                // 2층 입실 카운터
                string strSQL = "Select ID from Access_Alarm where LACode = '304-48-1-1' AND Status LIKE '%-4200' AND ATime > '" + strLimitDate + "'";
                ArrayList arrResult = m_DBManager.GetResultData(strSQL);
                listFloorInOut.Add(arrResult.Count);

                // 2층 퇴실 카운터
                strSQL = "Select ID from Access_Alarm where LACode = '304-48-1-2' AND Status LIKE '%-4201' AND ATime > '" + strLimitDate + "'";
                arrResult = m_DBManager.GetResultData(strSQL);
                listFloorInOut.Add(arrResult.Count);

                // 3층 입실 카운터
                strSQL = "Select ID from Access_Alarm where LACode = '304-39-1-1' AND Status LIKE '%-4200' AND ATime > '" + strLimitDate + "'";
                arrResult = m_DBManager.GetResultData(strSQL);
                listFloorInOut.Add(arrResult.Count);

                // 3층 퇴실 카운터
                strSQL = "Select ID from Access_Alarm where LACode = '304-39-1-2' AND Status LIKE '%-4201' AND ATime > '" + strLimitDate + "'";
                arrResult = m_DBManager.GetResultData(strSQL);
                listFloorInOut.Add(arrResult.Count);

                // 4층 입실 카운터
                strSQL = "Select ID from Access_Alarm where LACode = '304-28-1-1' AND Status LIKE '%-4200' AND ATime > '" + strLimitDate + "'";
                arrResult = m_DBManager.GetResultData(strSQL);
                listFloorInOut.Add(arrResult.Count);

                // 4층 퇴실 카운터
                strSQL = "Select ID from Access_Alarm where LACode = '304-28-1-2' AND Status LIKE '%-4201' AND ATime > '" + strLimitDate + "'";
                arrResult = m_DBManager.GetResultData(strSQL);
                listFloorInOut.Add(arrResult.Count);

                // 5층 입실 카운터
                strSQL = "Select ID from Access_Alarm where LACode = '304-19-1-1' AND Status LIKE '%-4200' AND ATime > '" + strLimitDate + "'";
                arrResult = m_DBManager.GetResultData(strSQL);
                listFloorInOut.Add(arrResult.Count);

                // 5층 퇴실 카운터
                strSQL = "Select ID from Access_Alarm where LACode = '304-19-1-2' AND Status LIKE '%-4201' AND ATime > '" + strLimitDate + "'";
                arrResult = m_DBManager.GetResultData(strSQL);
                listFloorInOut.Add(arrResult.Count);

                // 6층 입실 카운터
                strSQL = "Select ID from Access_Alarm where LACode = '304-7-1-1' AND Status LIKE '%-4200' AND ATime > '" + strLimitDate + "'";
                arrResult = m_DBManager.GetResultData(strSQL);
                listFloorInOut.Add(arrResult.Count);

                // 6층 퇴실 카운터
                strSQL = "Select ID from Access_Alarm where LACode = '304-7-1-2' AND Status LIKE '%-4201' AND ATime > '" + strLimitDate + "'";
                arrResult = m_DBManager.GetResultData(strSQL);
                listFloorInOut.Add(arrResult.Count);

                m_DBManager.Close();
                m_listAccessFloor.Clear();

                for (int i = 0; i < listFloorInOut.Count; i+=2)
                {
                    int nCount = 0;
                    nCount = listFloorInOut[i] - listFloorInOut[i + 1];

                    m_listAccessFloor.Add(nCount);
                }
            }
        }

        private string LoadLimitDate()
        {
            string retDate = DateTime.Now.ToString("yyyyMMdd") + "000000";
            char sp = ':';
            string[] spStrings = null;

            try
            {
                if (File.Exists(m_strAccessSetPath))
                {
                    StreamReader reader = new StreamReader(m_strAccessSetPath, Encoding.Default);

                    while (reader.EndOfStream == false)
                    {
                        string strLine = reader.ReadLine().Trim();
                        spStrings = strLine.Split(sp);

                        if (spStrings[0].Trim() == "AccessDate")
                        {
                            string strDate = spStrings[1].Trim();

                            int nTemp = 0;

                            if (int.TryParse(strDate, out nTemp) == true || strDate.Length == 14)
                            {
                                retDate = strDate;
                            }
                        }
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {

            }

            return retDate;
        }

        private void SaveLimitDate()
        {
            try
            {
                char sp = ':';
                string[] spStrings = null;

                StreamReader reader = new StreamReader(m_strAccessSetPath, Encoding.Default);
                StreamWriter writer = new StreamWriter(m_strAccessSetTempPath, false, Encoding.Default);

                while (reader.EndOfStream == false)
                {
                    string strLine = reader.ReadLine().Trim();
                    spStrings = strLine.Split(sp);

                    string strTitle = spStrings[0].Trim();
                    string strValue = spStrings[1].Trim();

                    if (strTitle == "AccessDate")
                    {
                        strLine = "AccessDate : " + DateTime.Now.ToString("yyyyMMddHHmmss");
                    }

                    writer.WriteLine(strLine);
                }

                reader.Close();
                writer.Close();

                FileInfo file = new FileInfo(m_strAccessSetTempPath);

                if (file.Exists)
                {
                    file.CopyTo(m_strAccessSetPath, true);
                    file.Delete();
                }
            }
            catch
            {

            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            UIManager.Instance.SetExternalFormStatus();
            this.Hide();
        }

        private void SetWorkStatus()
        {
            int i = 0;
            int nTitleSize = 50;
            int nHeightSize = 50;

            if (m_listAccessFloor.Count == 0 || m_listAccessFloor == null)
                return;

            //if (m_listAccessFloor[0] != 0)
            //{
            lbFloor2.Text = m_listAccessFloor[0].ToString();
            //plFloor2.Location = new System.Drawing.Point(0, (i * nHeightSize) + nTitleSize);
            //plFloor2.BringToFront();
            //i++;
            //}

            //if (m_listAccessFloor[1] != 0)
            //{
            lbFloor3.Text = m_listAccessFloor[1].ToString();
            //plFloor3.Location = new System.Drawing.Point(0, (i * nHeightSize) + nTitleSize);
            //plFloor3.BringToFront();
            //i++;
            //}

            //if (m_listAccessFloor[2] != 0)
            //{
            lbFloor4.Text = m_listAccessFloor[2].ToString();
            //plFloor4.Location = new System.Drawing.Point(0, (i * nHeightSize) + nTitleSize);
            //plFloor4.BringToFront();
            //i++;
            //}

            //if (m_listAccessFloor[3] != 0)
            //{
            lbFloor5.Text = m_listAccessFloor[3].ToString();
            //plFloor5.Location = new System.Drawing.Point(0, (i * nHeightSize) + nTitleSize);
            //plFloor5.BringToFront();
            //i++;
            //}

            //if (m_listAccessFloor[4] != 0)
            //{
            lbFloor6.Text = m_listAccessFloor[4].ToString();
            //plFloor6.Location = new System.Drawing.Point(0, (i * nHeightSize) + nTitleSize);
            //plFloor6.BringToFront();
            //i++;
            //}

            //this.Height = nTitleSize + (i * nHeightSize);
            //UIManager.Instance.OnResize();

        }

        public bool CheckAccessFloor()
        {
            bool bRet = false;
            
            foreach (int row in m_listAccessFloor)
            {
                if (row != 0)
                    bRet = true;
            }

            return bRet;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("출입 인원을 초기화 시키겠습니까?", "출입인원", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                SaveLimitDate();
                //this.Hide();
                //UIManager.Instance.SetExternalFormStatus();
            }
        }

        private void CleanDB()
        {
            if (m_bDBConnect == false)
                return;

            int nDate = LoadCleanDate();

            if (nDate == 0 || nDate < 0)
                return;

            nDate = -nDate;

            string strCleanDate = DateTime.Now.AddDays(nDate).ToString("yyyyMMdd");

            if (m_DBManager.Connect() == true)
            {
                string strSQL = "DELETE FROM Access_Alarm WHERE ATime < '" + strCleanDate + "000000'";
                ArrayList arrResult = m_DBManager.GetResultData(strSQL);

                m_DBManager.Close();
            }
        }

        private int LoadCleanDate()
        {
            Dictionary<string, string> dicRetDate = new Dictionary<string, string>();
            int nRet = 0;

            try
            {
                char sp = '=';
                string[] spStrings = null;

                if (File.Exists(m_strConfigPath))
                {
                    StreamReader reader = new StreamReader(m_strConfigPath, Encoding.Default);

                    while (reader.EndOfStream == false)
                    {
                        string strLine = reader.ReadLine();

                        if (strLine.Length == 0)
                            continue;

                        spStrings = strLine.Split(sp);

                        string strTitle = spStrings[0].Trim();

                        if (strTitle == "access_clean")
                        {
                            if (int.TryParse(spStrings[1].Trim(), out nRet) == true)
                            {
                                break;
                            }
                        }
                    }

                    reader.Close();
                }
            }
            catch
            {

            }

            return nRet;
        }

        private void plTitleba_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = plTitleba.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void plTitleba_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point ptCur = this.Location;

                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {

                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void plTitleba_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_bLeftMouseDown = false;
        }
    }
}
