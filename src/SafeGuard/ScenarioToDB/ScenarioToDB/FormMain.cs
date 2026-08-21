using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.IO;

namespace ScenarioToDB
{
    public partial class FormMain : Form
    {
        private ArrayList m_arrActionSteps = null;
        private bool m_isDayLight = true;
        private string m_strChemistry = "";
        private string m_strPlace = "";
        private string m_strAccident = "";
        private string m_strWeather = "";
        private string m_strDamage = "";
        private int m_nCountOfDeath = 0;
        private int m_nCountOfBuilding = 0;
        // m
        private double m_dInitialDistance = 0.0;
        private string m_strControl = "";
        private List<string> m_actions = new List<string>();
        // km
        private double m_dDistance = 0.0;
        private List<string> m_patientItems = new List<string>();
        private string m_strMixedFactor = "";

        private int m_nCurrentProcessIndex = 0;
        private int m_nTotalProcessCount = 0;

        WebDBManagerEx m_dbMgr = new WebDBManagerEx();

        public FormMain()
        {
            //m_dbMgr.WebServerURL = "http://192.168.0.195:8080/SOP";
            m_dbMgr.DatabaseName = "SafeGuard";
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            labelProcess.Visible = false;
        }

        private void LoadData()
        {
            //LoadTime();
            LoadChemistryNWeather();
            LoadPlaceDamageCount();
            LoadAccident();
            LoadInitialDistance();
            LoadControl();
            LoadActions();
            LoadDistance();
            LoadPatientItems();
        }

        private void LoadPatientItems()
        {
            m_patientItems.Clear();
            Component component = GetComponent(22);

            if (component == null)
                return;

            PropertyProcess prop = (PropertyProcess)component.Property;

            int nCount = prop.Missions.Count;

            for (int i = 0; i < nCount; i++)
            {
                Sections.MissionItem item = (Sections.MissionItem)prop.Missions[i];

                if (i >= 3)
                {
                    m_patientItems.Add(item.Mission);
                }
            }
        }

        private void LoadDistance()
        {
            Component component = GetComponent(6);

            if (component == null)
                return;

            PropertyProcess prop = (PropertyProcess)component.Property;

            int nCount = prop.Missions.Count;

            for (int i = 0; i < nCount; i++)
            {
                Sections.MissionItem item = (Sections.MissionItem)prop.Missions[i];

                if (i == 0)
                {
                    string strTarget1 = "주민대피 반경 ";
                    string strTarget2 = "km";

                    int nIndex1 = item.Mission.IndexOf(strTarget1);
                    int nIndex2 = item.Mission.IndexOf(strTarget2);

                    if (nIndex1 >= 0 && nIndex2 > nIndex1)
                    {
                        nIndex1 = nIndex1 + strTarget1.Length;
                        string strDistance = item.Mission.Substring(nIndex1, nIndex2 - nIndex1);
                        //m_dDistance = double.Parse(strDistance);

                        if (!double.TryParse(strDistance, out m_dDistance))
                            m_dDistance = 0.0;
                    }
                }
            }
        }

        private void LoadActions()
        {
            m_actions.Clear();
            Component component = GetComponent(16);

            if (component == null)
                return;

            PropertyProcess prop = (PropertyProcess)component.Property;

            int nCount = prop.Missions.Count;

            for (int i = 0; i < nCount; i++)
            {
                Sections.MissionItem item = (Sections.MissionItem)prop.Missions[i];

                if (i >= 8)
                {
                    m_actions.Add(item.Mission);
                }
            }
        }

        private void LoadControl()
        {
            Component component = GetComponent(15);

            if (component == null)
                return;

            PropertyProcess prop = (PropertyProcess)component.Property;

            int nCount = prop.Missions.Count;

            for (int i = 0; i < nCount; i++)
            {
                Sections.MissionItem item = (Sections.MissionItem)prop.Missions[i];

                if (i == 0)
                {
                    string strTarget = "지휘 체계 확립 ";

                    int nIndex = item.Mission.IndexOf(strTarget);

                    if (nIndex >= 0)
                    {
                        string strControl = item.Mission.Substring(nIndex + strTarget.Length);
                        m_strControl = strControl;
                    }
                }
            }
        }

        private void LoadInitialDistance()
        {
            Component component = GetComponent(3);

            if (component == null)
                return;

            PropertyProcess prop = (PropertyProcess)component.Property;

            int nCount = prop.Missions.Count;

            for (int i = 0; i < nCount; i++)
            {
                Sections.MissionItem item = (Sections.MissionItem)prop.Missions[i];
                
                if (i == 4)
                {
                    string strTarget1 = "  - 초기이격거리 : ";
                    string strTarget2 = "m";

                    int nIndex1 = item.Mission.IndexOf(strTarget1);
                    int nIndex2 = item.Mission.IndexOf(strTarget2);

                    if (nIndex1 >= 0 && nIndex2 > nIndex1)
                    {
                        nIndex1 = nIndex1 + strTarget1.Length;
                        string strInitial = item.Mission.Substring(nIndex1, nIndex2 - nIndex1);
                        //m_dInitialDistance = double.Parse(strInitial);

                        if (!double.TryParse(strInitial, out m_dInitialDistance))
                            m_dInitialDistance = 0.0;
                    }
                }
            }
        }

        private void LoadAccident()
        {
            Component component = GetComponent(32);

            if (component == null)
                return;

            string strTarget = " 진압";
            PropertyProcess prop = (PropertyProcess)component.Property;

            foreach (Sections.MissionItem item in prop.Missions)
            {
                int nIndex = item.Mission.IndexOf(strTarget);

                if (nIndex >= 0)
                {
                    string strAccident = item.Mission.Substring(0, nIndex);
                    m_strAccident = strAccident;
                    break;
                }
            }
        }

        private void LoadPlaceDamageCount()
        {
            Component component = GetComponent(9);

            if (component == null)
                return;

            PropertyProcess prop = (PropertyProcess)component.Property;

            int nCount = prop.Missions.Count;

            for (int i = 0; i < nCount;i++ )
            {
                Sections.MissionItem item = (Sections.MissionItem)prop.Missions[i];

                if (i == 0)
                {
                    string strTarget = "에서";
                    int nIndex = item.Mission.IndexOf(strTarget);

                    if (nIndex >= 0)
                    {
                        string strPlace = item.Mission.Substring(0, nIndex);
                        m_strPlace = strPlace;
                    }
                }
                else if (i == 1)
                {
                    string strTarget = "반응물질 : ";
                    int nIndex = item.Mission.IndexOf(strTarget);

                    if (nIndex >= 0)
                    {
                        string strMixedFactor = item.Mission.Substring(nIndex + strTarget.Length);
                        m_strMixedFactor = strMixedFactor;
                    }
                }
                else if (i == 2)
                {
                    string strTarget = "생성";
                    int nIndex = item.Mission.IndexOf(strTarget);

                    if (nIndex >= 0)
                    {
                        string strDamage = item.Mission.Substring(0, nIndex);
                        m_strDamage = strDamage;
                    }
                }
                else if (i == 3)
                {
                    string strTarget1 = "에서 ";
                    string strTarget2 = "명의";

                    int nIndex1 = item.Mission.IndexOf(strTarget1);
                    int nIndex2 = item.Mission.IndexOf(strTarget2);

                    if (nIndex1 >= 0 && nIndex2 > nIndex1)
                    {
                        nIndex1 = nIndex1 + strTarget1.Length;
                        string strCountOfDeath = item.Mission.Substring(nIndex1, nIndex2 - nIndex1);
                        //m_nCountOfDeath = int.Parse(strCountOfDeath);

                        if (!int.TryParse(strCountOfDeath, out m_nCountOfDeath))
                        {
                            m_nCountOfDeath = 0;
                        }
                    }
                }
                else if (i == 4)
                {
                    string strTarget1 = "에서 ";
                    string strTarget2 = "채";

                    int nIndex1 = item.Mission.IndexOf(strTarget1);
                    int nIndex2 = item.Mission.IndexOf(strTarget2);

                    if (nIndex1 >= 0 && nIndex2 > nIndex1)
                    {
                        nIndex1 = nIndex1 + strTarget1.Length;
                        string strCountOfBuilding = item.Mission.Substring(nIndex1, nIndex2 - nIndex1);
                        //m_nCountOfBuilding = int.Parse(strCountOfBuilding);

                        if (!int.TryParse(strCountOfBuilding, out m_nCountOfBuilding))
                        {
                            m_nCountOfBuilding = 0;
                        }
                    }
                }
            }
        }

        private void LoadChemistryNWeather()
        {
            Component component = GetComponent(7);

            if (component == null)
                return;

            string strTarget = "누출물질: ";
            PropertyProcess prop = (PropertyProcess)component.Property;

            foreach (Sections.MissionItem item in prop.Missions)
            {
                int nIndex = item.Mission.IndexOf(strTarget);

                if (nIndex >= 0)
                {
                    string strChemistry = item.Mission.Substring(nIndex + strTarget.Length);
                    m_strChemistry = strChemistry;

                    /*if (m_strChemistry.Contains("플루오르"))
                        m_strChemistry = "플루오르화수소";
                    else if (m_strChemistry.Contains("황산"))
                        m_strChemistry = "황산";
                    else*/
                        return;
                }
                else if (!item.Mission.Contains("에서"))
                {
                    m_strWeather = item.Mission;
                }
            }
        }

        //private void LoadTime()
        //{
        //    Component component = GetComponent(8);

        //    if (component == null)
        //        return;

        //    if (component.Text == "10:00")
        //        m_isDayLight = true;
        //    else
        //        m_isDayLight = false;
        //}

        private Component GetComponent(int nComponentID)
        {
            foreach (ActionStep actionStep in m_arrActionSteps)
            {
                foreach (StepMember stepMember in actionStep.StepMemberList)
                {
                    foreach (Component component in stepMember.ComponentList)
                    {
                        if (component.ID == nComponentID)
                            return component;
                    }
                }
            }

            return null;
        }

        private bool SaveDB(string strPath)
        {
            if (!m_dbMgr.InsertDisaster(m_strChemistry, m_strAccident, m_strMixedFactor, m_strWeather, m_isDayLight, m_dDistance, m_strDamage))
                return false;

            return m_dbMgr.InsertScenario(strPath, m_arrActionSteps);
        }

        private void btnLoadFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxFolder.Text = dlg.SelectedPath;
            }
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            if (textBoxFolder.Text.Length == 0)
                MessageBox.Show("시나리오 XML이 존재하는 폴더의 경로를 입력하세요.");
            else
            {
                string[] arrFiles = Directory.GetFiles(textBoxFolder.Text);
                List<string> xmlFiles = new List<string>();

                foreach (string strFilePath in arrFiles)
                {
                    int nIndex1 = strFilePath.LastIndexOf('\\');
                    string strFileName = strFilePath.Substring(nIndex1 + 1);

                    if (strFileName.StartsWith("~"))
                        continue;

                    int nIndex = strFilePath.LastIndexOf('.');

                    if (nIndex < 0)
                        continue;

                    string strExt = strFilePath.Substring(nIndex + 1);

                    if (string.Compare(strExt, "xml", StringComparison.CurrentCultureIgnoreCase) == 0)
                        xmlFiles.Add(strFilePath);
                }

                m_nCurrentProcessIndex = 0;
                m_nTotalProcessCount = xmlFiles.Count;

                if (m_nTotalProcessCount == 0)
                    return;

                btnProcess.Enabled = false;
                labelProcess.Text = string.Format("{0} / {1}", m_nCurrentProcessIndex, m_nTotalProcessCount);
                labelProcess.Visible = true;

                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ProcessSaveDB));
				t.Start(xmlFiles);
            }
        }

        private void ProcessSaveDB(object param)
        {
            List<string> xmlFiles = (List<string>)param;

            foreach (string strFilePath in xmlFiles)
            {
                XMLManager mgr = new XMLManager();
                bool result = mgr.Load(strFilePath);

                m_arrActionSteps = null;
                m_actions.Clear();
                m_patientItems.Clear();

                if (result)
                {
                    m_arrActionSteps = mgr.ActionSteps;
                    LoadData();
                    m_isDayLight = mgr.IsNormal;
                }
                else
                {
                    SetStatus(labelProcess.Text + ", Error 발생");
                    break;
                }

                if (!SaveDB(strFilePath))
                {
                    SetStatus(labelProcess.Text + ", Error 발생");
                    break;
                }
                else
                {
                    m_nCurrentProcessIndex++;
                    SetStatus();
                }
            }

            this.Invoke((MethodInvoker)delegate
            {
                btnProcess.Enabled = true;
            });
        }

        private void SetStatus()
        {
            this.Invoke((MethodInvoker)delegate
            {
                labelProcess.Text = string.Format("{0} / {1}", m_nCurrentProcessIndex, m_nTotalProcessCount);
            });
        }

        private void SetStatus(string strStatus)
        {
            this.Invoke((MethodInvoker)delegate
            {
                labelProcess.Text = strStatus;
            });
        }
    }
}
