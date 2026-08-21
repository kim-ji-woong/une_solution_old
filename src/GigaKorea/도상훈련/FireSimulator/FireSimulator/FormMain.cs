using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Concurrent;
using DBUtility2;
using FireSimulator.Data;
using XMLWebServiceManager;

namespace FireSimulator
{
    public partial class FormMain : Form
    {
        //private const string XML_VERSION = "1.3";
        public const string MINIMUM_VERSION = "1.5";
        private const string HISTORY_FILE = "history.dat";

        private string m_strXMLFile = null;
        private Project m_project = null;
        private Level m_selectedLevel = null;
        private Space m_selectedSpace = null;

        private Level m_selectOutLevel = null;
        private Space m_selectOutSpace = null;

        private OutbreakData m_selectOutbreak = null;

        private ConcurrentDictionary<Alarm, Alarm> m_alarms = new ConcurrentDictionary<Alarm, Alarm>();
        private ConcurrentDictionary<Alarm, Alarm> m_outAlarms = new ConcurrentDictionary<Alarm, Alarm>();

        private NetworkServer m_netServer = null;
        private OutbreakManager m_outMgr = null;

        WebServiceManager m_webManager = null;
        private string m_strID = null;
        private string m_strPW = null;

        // 다운로드 xml 경로
        private string m_strFilePath = null;
        public string FilePath
        {
            set { m_strFilePath = value; }
        }

        private static FormMain m_instance = null;

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        private int m_nSiteID = 1;
        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        private WebDBManager m_dbMgr = null;
        private WebDBManager m_dbSub1Mgr = null;
        private WebDBManager m_dbSub2Mgr = null;

        private int m_nUpdateDBCount = 0;

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
            set { m_dbMgr = value; }
        }

        private NetworkWebManager m_NetMgr = null;
        public NetworkWebManager NetManager
        {
            get { return m_NetMgr; }
        }

        private NetworkWebManager m_NetSub1Mgr = null;
        private NetworkWebManager m_NetSub2Mgr = null;

        private NetworkWebManager m_OutMgr = null;

        public void Visiable(bool bValue)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.Visible = bValue;
            });
        }

        public FormMain(bool bChk = false)
        {
            m_instance = this;
            InitializeComponent();
            //ReadHistory();

            //m_nSiteID = LoadSiteID();
            if (ReadConfig("MainSiteID", out m_nSiteID) == false)
                m_nSiteID = 300;

            int nClient1ID = -1;
            int nClient2ID = -1;

            if (ReadConfig("Client1SiteID", out nClient1ID) == false)
                nClient1ID = 301;
            if (ReadConfig("Client2SiteID", out nClient2ID) == false)
                nClient2ID = 302;

            m_netServer = new NetworkServer();

            m_dbMgr = new WebDBManager(m_nSiteID);
            m_dbSub1Mgr = new WebDBManager(nClient1ID);
            m_dbSub2Mgr = new WebDBManager(nClient2ID);

            m_NetMgr = new NetworkWebManager(m_dbMgr, SOPWebServer.ClientType.FIRE_SENSOR_SERVER, SOPWebServer.ClientSubType.SIMULATOR);
            m_OutMgr = new NetworkWebManager(m_dbMgr, SOPWebServer.ClientType.SOP_SIMULATOR, SOPWebServer.ClientSubType.SIMULATOR);

            m_NetSub1Mgr = new NetworkWebManager(m_dbSub1Mgr, SOPWebServer.ClientType.FIRE_SENSOR_SERVER, SOPWebServer.ClientSubType.SIMULATOR);
            m_NetSub2Mgr = new NetworkWebManager(m_dbSub2Mgr, SOPWebServer.ClientType.FIRE_SENSOR_SERVER, SOPWebServer.ClientSubType.SIMULATOR);

            if (bChk)
                this.Visible = true;



            // XML 다운로드 셋팅
            string strID = System.Configuration.ConfigurationManager.AppSettings.Get("SampleProject_ID");
            if (strID == null || strID.Length == 0)
                strID = "user_spatial";

            string strPW = System.Configuration.ConfigurationManager.AppSettings.Get("SampleProject_PW");
            if (strPW == null || strPW.Length == 0)
                strPW = "spatial1234";

            m_strID = strID;
            m_strPW = strPW;

            m_webManager = new WebServiceManager();

        }

        private bool ReadConfig(string strName, out int value)
        {
            string strValue = System.Configuration.ConfigurationManager.AppSettings[strName].ToString().Trim();
            return int.TryParse(strValue, out value);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //if (m_strXMLFile == null)
            //{
                //string strFolder = Application.StartupPath;
                //string[] files = Directory.GetFiles(strFolder, "*.xml");

                //foreach (string strXML in files)
                //{
                //    Project project = ReadXML(strXML);

                //    if (project != null)
                //    {
                //        SetProject(project);
                //        labelBuildingName.Visible = /*labelFloor.Visible = labelSpace.Visible =*/ true;
                //        m_strXMLFile = strXML;
                //        btnFire.Enabled = false;
                //        break;
                //    }
                //}

                string strTEMP = Environment.GetEnvironmentVariable("TEMP");
                if (strTEMP == null || strTEMP.Length == 0)
                {
                    MessageBox.Show("환경변수 TEMP 를 찾을 수 없습니다. 관리자에게 문의해주세요.");
                    return;
                }

                string strFileName = strTEMP + "\\다부처건물.xml";

                Project project = ReadXML(strFileName);

                if (project != null)
                {
                    m_nUpdateDBCount = 0;
                    //btnOpenXML.Enabled = false;
                    RunUpdateDBThread(project, strFileName);
                }
            //}

            cmbOutbreak.ValueMember = "Text";
            m_outMgr = new OutbreakManager(m_dbMgr);
        }

        private void SetProject(Project project)
        {
            labelBuildingName.Text = project.Name;
            //labelFloor.Text = labelSpace.Text = "";

            treeSpace.Nodes.Clear();
            treeOutbreak.Nodes.Clear();

            foreach (Level level in project.Levels)
            {
                TreeNode node = treeSpace.Nodes.Add(level.Name);
                TreeNode OutNode = treeOutbreak.Nodes.Add(level.Name);

                node.Tag = level;
                OutNode.Tag = level;

                foreach (Space space in level.Spaces)
                {
                    TreeNode spaceNode = node.Nodes.Add(space.Name);
                    TreeNode OutSpaceNode = OutNode.Nodes.Add(space.Name);

                    spaceNode.Tag = space;
                    OutSpaceNode.Tag = space;
                }
            }

            m_project = project;
        }

        private Project ReadXML(string strPath)
        {
            if (File.Exists(strPath) == false)
                return null;

            StreamReader reader = new StreamReader(strPath, Encoding.UTF8);
            string strXML = reader.ReadToEnd();
            reader.Close();

            XElement xml = XElement.Parse(strXML);

            if (xml.Name != "IndoorModelFile")
                return null;

            XAttribute attr = xml.Attribute("version");
            
            string strMiniVersion = MINIMUM_VERSION;
            double dMiniVersion = Convert.ToDouble(strMiniVersion);

            double dVersion = 0;

            // v1.5 이상 읽을 수 있다.
            //if (attr == null || attr.Value != XML_VERSION)
            //    return null;
            if (attr != null)
                dVersion = double.Parse(attr.Value);

            if (dMiniVersion > dVersion)
                return null;

            return Project.Read(xml);
        }

        private void treeSpace_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;

            if (e.Node.Level == 0)
            {
                //labelFloor.Text = labelSpace.Text = "";
                m_selectedLevel = null;
                m_selectedSpace = null;
                btnFire.Enabled = false;
                return;
            }
            else
            {
                Level level = (Level)e.Node.Parent.Tag;
                Space space = (Space)e.Node.Tag;

                //labelFloor.Text = level.Name;
                //labelSpace.Text = space.Name;

                m_selectedLevel = level;
                m_selectedSpace = space;
                btnFire.Enabled = true;
            }
        }

        private void AddAlarm(Alarm alarm)
        {
            m_alarms[alarm] = alarm;
            //WriteHistory();
        }

        private void AddOutAlarm(Alarm alarm)
        {
            m_outAlarms[alarm] = alarm;
        }

        private void RemoveAlarm(Alarm alarm)
        {
            Alarm _alarm;

            if (m_alarms.TryRemove(alarm, out _alarm))
            {
                foreach (DataGridViewRow row in gridFire.Rows)
                {
                    if (row.Tag == _alarm)
                    {
                        gridFire.Rows.Remove(row);
                        Reorder();
                        break;
                    }
                }
            }
        }

        private void RemoveOutAlarm(Alarm alarm)
        {
            Alarm _alarm;

            if (m_outAlarms.TryRemove(alarm, out _alarm))
            {
                foreach (DataGridViewRow row in gridOutbreak.Rows)
                {
                    if (row.Tag == _alarm)
                    {
                        gridOutbreak.Rows.Remove(row);
                        ReOutorder();
                        break;
                    }
                }
            }
        }

        private void Reorder()
        {
            foreach (DataGridViewRow row in gridFire.Rows)
            {
                row.Cells[0].Value = row.Index + 1;
            }
        }

        private void ReOutorder()
        {
            foreach (DataGridViewRow row in gridOutbreak.Rows)
            {
                row.Cells[0].Value = row.Index + 1;
            }
        }

        private void ReadHistory()
        {
            if (File.Exists(HISTORY_FILE) == false)
                return;

            StreamReader reader = new StreamReader(HISTORY_FILE, Encoding.UTF8);

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                if (m_strXMLFile == null)
                {
                    m_strXMLFile = strLine;

                    Project project = ReadXML(m_strXMLFile);

                    if (project != null)
                    {
                        SetProject(project);
                        labelBuildingName.Visible = true;
                        btnFire.Enabled = false;
                    }

                    continue;
                }

                string[] tokens = strLine.Split(',');

                if (tokens.Count() != 3)
                    return;

                string strLevelID = tokens[0].Trim();
                string strSpaceID = tokens[1].Trim();
                string strTime = tokens[2].Trim();
                DateTime time = DateTime.FromBinary(long.Parse(strTime));

                Alarm alarm = MakeAlarm(strLevelID, strSpaceID, time);

                if (alarm != null)
                {
                    AddGrid(alarm);
                    m_alarms[alarm] = alarm;
                }
            }

            reader.Close();
        }

        private void WriteHistory()
        {
            if (m_strXMLFile == null)
                return;

            StreamWriter writer = new StreamWriter(HISTORY_FILE, false, Encoding.UTF8);

            writer.WriteLine(m_strXMLFile);

            foreach (KeyValuePair<Alarm, Alarm> pair in m_alarms)
            {
                Alarm alarm = pair.Value;

                if (alarm.Level == null || alarm.Space == null)
                    continue;

                writer.Write(alarm.Level.ID);
                writer.Write(", " + alarm.Space.ID);
                writer.WriteLine(", " + alarm.TimeStamp.ToBinary().ToString());
            }

            writer.Close();
        }

        private Alarm MakeAlarm(string strLevelID, string strSpaceID, DateTime time)
        {
            if (m_project == null)
                return null;

            foreach (Level level in m_project.Levels)
            {
                if (level.ID == strLevelID)
                {
                    foreach (Space space in level.Spaces)
                    {
                        if (space.ID == strSpaceID)
                        {
                            Alarm alarm = new Alarm();

                            alarm.TimeStamp = time;
                            alarm.Level = level;
                            alarm.Space = space;

                            return alarm;
                        }
                    }

                    return null;
                }
            }

            return null;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
#if TRAY_ICON
            e.Cancel = true;
            this.Visible = false;
#else
            m_NetMgr.ReleaseThread();
            m_OutMgr.ReleaseThread();

            if (m_NetSub1Mgr != null)
                m_NetSub1Mgr.ReleaseThread();
            if (m_NetSub2Mgr != null)
                m_NetSub2Mgr.ReleaseThread();

            if (m_outMgr != null)
                m_outMgr.Shoutdown();
#endif
        }

        public void FormClosed()
        {
            m_NetMgr.ReleaseThread();
            m_OutMgr.ReleaseThread();
            
            if (m_NetSub1Mgr != null)
                m_NetSub1Mgr.ReleaseThread();
            if (m_NetSub2Mgr != null)
                m_NetSub2Mgr.ReleaseThread();

            if (m_outMgr != null)
                m_outMgr.Shoutdown();

            //WriteHistory();
            this.Dispose();
        }

        private void btnOpenXML_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "XML 파일 (*.xml)|*.xml|모든 파일 (*.*)|*.*";
            dlg.Title = "프로젝트 파일 열기";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Project project = ReadXML(dlg.FileName);

                if (project != null)
                {
                    m_nUpdateDBCount = 0;
                    //btnOpenXML.Enabled = false;
                    RunUpdateDBThread(project, dlg.FileName);

                    /*SetProject(project);
                    labelBuildingName.Visible = true;
                    m_strXMLFile = dlg.FileName;
                    m_alarms.Clear();
                    gridFire.Rows.Clear();
                    btnFire.Enabled = false;
                    return;*/
                }
            }
        }

        private void CompleteUpdateDB(Project project, string strFileName)
        {
            if (project == null)
            {
                //btnOpenXML.Enabled = true;
                return;
            }

            m_nUpdateDBCount++;

            if (m_nUpdateDBCount >= 3)
            {
                SetProject(project);
                labelBuildingName.Visible = true;
                m_strXMLFile = strFileName;
                m_alarms.Clear();
                gridFire.Rows.Clear();
                btnFire.Enabled = false;
                //btnOpenXML.Enabled = true;
            }
        }

        private void RunUpdateDBThread(Project project, string strFileName)
        {
            m_NetMgr.SendMessage(SOPWebServer.Header.RESET_MAX_ACTIONSTEP_HISTORY_ID, null);
            m_NetSub1Mgr.SendMessage(SOPWebServer.Header.RESET_MAX_ACTIONSTEP_HISTORY_ID, null);
            m_NetSub2Mgr.SendMessage(SOPWebServer.Header.RESET_MAX_ACTIONSTEP_HISTORY_ID, null);

            ArrayList arrDatas1 = new ArrayList();
            arrDatas1.Add(m_dbMgr);
            arrDatas1.Add(project);
            arrDatas1.Add(strFileName);

            Thread t1 = new Thread(new ParameterizedThreadStart(UpdateDBThread));
            t1.Start(arrDatas1);

            ArrayList arrDatas2 = new ArrayList();
            arrDatas2.Add(m_dbSub1Mgr);
            arrDatas2.Add(project);
            arrDatas2.Add(strFileName);

            Thread t2 = new Thread(new ParameterizedThreadStart(UpdateDBThread));
            t2.Start(arrDatas2);

            ArrayList arrDatas3 = new ArrayList();
            arrDatas3.Add(m_dbSub2Mgr);
            arrDatas3.Add(project);
            arrDatas3.Add(strFileName);

            Thread t3 = new Thread(new ParameterizedThreadStart(UpdateDBThread));
            t3.Start(arrDatas3);
        }

        private void UpdateDBThread(object param)
        {
            ArrayList arrDatas = (ArrayList)param;
            WebDBManager dbMgr = (WebDBManager)arrDatas[0];
            Project project = (Project)arrDatas[1];
            string strFileName = (string)arrDatas[2];

            string strErrorMessage;

            if (DataManager.UpdateDB(dbMgr, project, out strErrorMessage) == false)
            {
                MessageBox.Show(strErrorMessage);

                this.Invoke((MethodInvoker)delegate
                {
                    this.CompleteUpdateDB(null, null);
                });

                return;
            }

            this.Invoke((MethodInvoker)delegate
            {
                this.CompleteUpdateDB(project, strFileName);
            });
        }

        private void btnFire_Click(object sender, EventArgs e)
        {
            if (m_project == null || m_selectedLevel == null || m_selectedSpace == null)
                return;

            if (FindAlarm(m_selectedLevel, m_selectedSpace) != null)
            {
                MessageBox.Show("이미 존재하는 알람입니다.");
                return;
            }

            Alarm alarm = new Alarm();
            alarm.Level = m_selectedLevel;
            alarm.Space = m_selectedSpace;
            alarm.TimeStamp = DateTime.Now;

            SendAlarm(alarm, m_project);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (gridFire.SelectedCells.Count == 0)
            {
                MessageBox.Show("종료시킬 화재를 Grid에서 선택하세요.");
            }
            else
            {
                DataGridViewRow row = gridFire.Rows[gridFire.SelectedCells[0].RowIndex];
                Alarm alarm = (Alarm)row.Tag;

                if (alarm != null && m_project != null)
                {
                    // SOP WebServer 알람 송신
                    if (!m_NetMgr.SendClearData(alarm, m_project) && m_NetMgr.IsConnected == true)
                        return;
                    if (!m_NetSub1Mgr.SendClearData(alarm, m_project) && m_NetSub1Mgr.IsConnected == true)
                        return;
                    if (!m_NetSub2Mgr.SendClearData(alarm, m_project) && m_NetSub2Mgr.IsConnected == true)
                        return;

                    RemoveAlarm(alarm);
                    m_netServer.SendClear(alarm, m_project, TCP_ID.CLEAR_FIRE);
                }
            }
        }

        private void AddGrid(Alarm alarm)
        {
            string strTime = string.Format("{0:00}:{1:00}:{2:00}", alarm.TimeStamp.Hour, alarm.TimeStamp.Minute, alarm.TimeStamp.Second);
            //string strLocation = alarm.Level.ID + " " + alarm.Space.ID;
            string strLocation = alarm.Level.Name + " " + alarm.Space.Name;

            int nRowIndex = gridFire.Rows.Add();

            if (nRowIndex < 0)
                return;

            DataGridViewRow row = gridFire.Rows[nRowIndex];
            row.Cells[0].Value = nRowIndex + 1;
            row.Cells[1].Value = strTime;
            row.Cells[2].Value = strLocation;
            row.Tag = alarm;
        }

        private void AddOutGrid(Alarm alarm)
        {
            string strTime = string.Format("{0:00}:{1:00}:{2:00}", alarm.TimeStamp.Hour, alarm.TimeStamp.Minute, alarm.TimeStamp.Second);
            string strLocation = alarm.Level.ID + " " + alarm.Space.ID;

            int nRowIndex = gridOutbreak.Rows.Add();

            if (nRowIndex < 0)
                return;

            DataGridViewRow row = gridOutbreak.Rows[nRowIndex];
            row.Cells[0].Value = nRowIndex + 1;
            row.Cells[1].Value = strTime;
            row.Cells[2].Value = strLocation;
            row.Tag = alarm;
        }

        private void SendAlarm(Alarm alarm, Project project)
        {
            // SOP WebServer 화재 알람 송신 >> 3개의 웹서버로 보내야함!!
            if (!m_NetMgr.SendSensorData(alarm, project) && m_NetMgr.IsConnected == true)
                return;
            if (!m_NetSub1Mgr.SendSensorData(alarm, project) && m_NetSub1Mgr.IsConnected == true)
                return;
            if (!m_NetSub2Mgr.SendSensorData(alarm, project) && m_NetSub2Mgr.IsConnected == true)
                return;

            AddGrid(alarm);
            AddAlarm(alarm);
            m_netServer.SendAlarm(alarm, project, TCP_ID.REPORT_FIRE);
        }

        private void SendOutAlarm(Alarm alarm, Project project)
        {
            if (cmbOutbreak.SelectedItem == null)
                return;

            OutbreakData outbreak = (OutbreakData)cmbOutbreak.SelectedItem;

            int nActionStepHistoryID = outbreak.ActionStepHistoryID;
            int nProcessID = outbreak.ProcessID;

            if (!m_OutMgr.SendOutbreakData(nActionStepHistoryID, nProcessID) && m_OutMgr.IsConnected == true)
                return;

            AddOutGrid(alarm);
            AddOutAlarm(alarm);
            m_netServer.SendAlarm(alarm, project, TCP_ID.REPORT_OUTBREAK);
        }

        private Alarm FindAlarm(Level level, Space space)
        {
            foreach (KeyValuePair<Alarm, Alarm> pair in m_alarms)
            {
                Alarm alarm = pair.Value;

                if (alarm.Level == level && alarm.Space == space)
                    return alarm;
            }

            return null;
        }

        private Alarm FindOutAlarm(Level level, Space space)
        {
            foreach (KeyValuePair<Alarm, Alarm> pair in m_outAlarms)
            {
                Alarm alarm = pair.Value;

                if (alarm.Level == level && alarm.Space == space)
                    return alarm;
            }

            return null;
        }

        public List<Alarm> GetAlarms()
        {
            return m_alarms.Values.ToList();
        }

        public Project GetProject()
        {
            return m_project;
        }

        public void UpdateClient(int nClientCount)
        {
            this.Invoke((MethodInvoker)delegate
            {
                labelClientCount.Text = nClientCount.ToString();
            });
        }

        private void btnOutbreak_Click(object sender, EventArgs e)
        {
            if (m_project == null || m_selectOutLevel == null || m_selectOutSpace == null)
                return;

            if (FindOutAlarm(m_selectOutLevel, m_selectOutSpace) != null)
            {
                MessageBox.Show("이미 존재하는 알람입니다.");
                return;
            }

            Alarm alarm = new Alarm();
            alarm.Level = m_selectOutLevel;
            alarm.Space = m_selectOutSpace;
            alarm.TimeStamp = DateTime.Now;

            SendOutAlarm(alarm, m_project);
        }

        private void treeOutbreak_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;

            if (e.Node.Level == 0)
            {
                m_selectOutLevel = null;
                m_selectOutSpace = null;
                btnOutbreak.Enabled = false;
                return;
            }
            else
            {
                Level level = (Level)e.Node.Parent.Tag;
                Space space = (Space)e.Node.Tag;

                m_selectOutLevel = level;
                m_selectOutSpace = space;

                if (cmbOutbreak.SelectedItem != null)
                    btnOutbreak.Enabled = true;
            }
        }

        private void btnClearOutbreak_Click(object sender, EventArgs e)
        {
            if (gridOutbreak.SelectedCells.Count == 0)
            {
                MessageBox.Show("종료시킬 돌발상황을 Grid에서 선택하세요.");
            }
            else
            {
                DataGridViewRow row = gridOutbreak.Rows[gridOutbreak.SelectedCells[0].RowIndex];
                Alarm alarm = (Alarm)row.Tag;

                if (alarm != null && m_project != null)
                {
                    RemoveOutAlarm(alarm);
                    m_netServer.SendClear(alarm, m_project, TCP_ID.CLEAR_OUTBREAK);
                }
            }
        }

        public void ShowOutbreakComboList(Dictionary<int, OutbreakData> dicOutbreak)
        {
            if (dicOutbreak == null)
                return;

            this.Invoke((MethodInvoker)delegate
            {
                cmbOutbreak.Items.Clear();

                foreach (KeyValuePair<int, OutbreakData> pair in dicOutbreak)
                {
                    OutbreakData outbreak = pair.Value;
                    cmbOutbreak.Items.Add(outbreak);
                }

                // TODO: 새롭게 불러올때 기존 값 유지
                //if (cmbOutbreak.Items.Count > 0)
                //{
                //    if (m_selectOutbreak != null)
                //    {
                //        for (int i = 0; i < cmbOutbreak.Items.Count; i++)
                //        {
                //            OutbreakData data = (OutbreakData)cmbOutbreak.Items[i];

                //            if (data.ProcessID == m_selectOutbreak.ProcessID)
                //                cmbOutbreak.SelectedIndex = i;
                //        }
                //    }
                //}

            });

              

        }

        public void ClearOutbreakComboList()
        {
            this.Invoke((MethodInvoker)delegate
            {
                cmbOutbreak.Items.Clear();
                btnOutbreak.Enabled = false;
            });
        }

        private void cmbOutbreak_SelectedValueChanged(object sender, EventArgs e)
        {
            // TODO: 선택했을때 값 교체
            if (m_project == null || m_selectOutLevel == null || m_selectOutSpace == null || cmbOutbreak.SelectedItem == null)
                btnOutbreak.Enabled = false;
            else
                btnOutbreak.Enabled = true;
        }

        private void btnDownXML_Click(object sender, EventArgs e)
        {
            //m_exeMgr.Run(ExecuteManager.APP_TYPE.SAMPLE_PROJECT);

            string strResultMessage = "";
            m_strFilePath = null;

            if (m_webManager.Login(m_strID, m_strPW, ref strResultMessage) == false)
            {   // 로그인 실패
                MessageBox.Show("Login Failed!");
                this.Close();
            }
            else
            {
                FormDownloadXML formDownloadXML = new FormDownloadXML(m_webManager);
                formDownloadXML.Owner = this;

                DialogResult result = formDownloadXML.ShowDialog();
                

                if (m_strFilePath != null)
                {   // XML 다운로드 성공하였을 경우
                    Project project = ReadXML(m_strFilePath);

                    //if (project != null)
                    //{
                    //    SetProject(project);
                    //    labelBuildingName.Visible = true;
                    //    //m_strXMLFile = dlg.FileName;
                    //    m_alarms.Clear();
                    //    gridFire.Rows.Clear();
                    //    btnFire.Enabled = false;
                    //    return;
                    //}
                    if (project != null)
                    {
                        m_nUpdateDBCount = 0;
                        //btnOpenXML.Enabled = false;
                        RunUpdateDBThread(project, m_strFilePath);
                    }
                }
            }

        }
    }
}
