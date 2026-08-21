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

namespace ConfirmScenario
{
    public partial class FormMain : Form
    {
        private bool m_isValidFile = false;
        private string m_strFilePath = "";

        private ArrayList m_arrActionSteps = null;
        private VariousData<bool> m_isDayLight = null;
        private string m_strChemistry = "";
        private string m_strPlace = "";
        private string m_strAccident = "";
        private string m_strWeather = "";
        private string m_strDamage = "";
        private VariousData<int> m_nCountOfDeath = null;
        private VariousData<int> m_nCountOfBuilding = null;
        // m
        private VariousData<double> m_dInitialDistance = null;
        private string m_strControl = "";
        private List<string> m_actions = new List<string>();
        // km
        private VariousData<double> m_dDistance = null;
        private List<string> m_patientItems = new List<string>();
        private string m_strMixedFactor = "";

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_DragDrop(object sender, DragEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("DragDrop");

            if (m_isValidFile)
            {
                LoadFile(m_strFilePath);
            }
        }

        private void FormMain_DragEnter(object sender, DragEventArgs e)
        {
            m_isValidFile = GetFileName(out m_strFilePath, e);

            if (m_isValidFile)
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;

            System.Diagnostics.Trace.WriteLine("DragEnter, filePath : " + m_strFilePath);
        }

        protected bool GetFileName(out string filename, DragEventArgs e)
        {
            bool ret = false;
            filename = String.Empty;

            string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if (fileList != null && fileList.Count() == 1)
            {
                string strExt = System.IO.Path.GetExtension(fileList[0]).ToLower();

                if (strExt == ".xml")
                {
                    filename = fileList[0];
                    ret = true;
                }
            }

            return ret;
        }

        private void LoadFile(string strPath)
        {
            ScenarioToDB.XMLManager mgr = new ScenarioToDB.XMLManager();
            bool result = mgr.Load(strPath);

            int nIndex1 = strPath.LastIndexOf('\\');
            int nIndex2 = strPath.LastIndexOf('.');

            m_arrActionSteps = null;
            m_actions.Clear();
            m_patientItems.Clear();

            if (result)
            {
                if (nIndex1 >= 0 && nIndex2 > nIndex1)
                {
                    string strFileName = strPath.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                    this.Text = strFileName;
                }
                else
                    this.Text = "시나리오 확인";

                m_arrActionSteps = mgr.ActionSteps;
                LoadData(mgr.IsNormal);
            }
            else
            {
                MessageBox.Show("Error 발생");
            }
        }

        private void LoadData(bool isDayLight)
        {
            m_isDayLight = new VariousData<bool>(isDayLight);
            m_strChemistry = "";
            m_strPlace = "";
            m_strAccident = "";
            m_strWeather = "";
            m_strDamage = "";
            m_nCountOfDeath = null;
            m_nCountOfBuilding = null;
            // m
            m_dInitialDistance = null;
            m_strControl = "";
            m_actions = new List<string>();
            // km
            m_dDistance = null;
            m_patientItems = new List<string>();
            m_strMixedFactor = "";

            //LoadTime();
            LoadChemistryNWeather();
            LoadPlaceDamageCount();
            LoadAccident();
            LoadInitialDistance();
            LoadControl();
            LoadActions();
            LoadDistance();
            LoadPatientItems();

            SetResult();
        }

        private void SetResult()
        {
            if (m_isDayLight == null)
                radioDay.Checked = radioNight.Checked = false;
            else if (m_isDayLight.Data)
                radioDay.Checked = true;
            else
                radioNight.Checked = true;

            cboMaterialName.Text = m_strChemistry;
            textBoxPlace.Text = m_strPlace;
            cboReason.Text = m_strAccident;
            cboWeather.Text = m_strWeather;
            textBoxMaterial.Text = m_strDamage;

            if (m_nCountOfDeath == null)
                textBoxCountOfDeath.Text = "";
            else
                textBoxCountOfDeath.Text = m_nCountOfDeath.Data.ToString();

            if (m_nCountOfBuilding == null)
                textBoxCountOfBuilding.Text = "";
            else
                textBoxCountOfBuilding.Text = m_nCountOfBuilding.Data.ToString();

            if (m_dInitialDistance == null)
                textBoxInitialDistance.Text = "";
            else
                textBoxInitialDistance.Text = string.Format("{0:F1}", m_dInitialDistance.Data);

            SetGrid(dataGridViewActionList, m_actions);
            SetGrid(dataGridViewPatient, m_patientItems);

            if (m_dDistance == null)
                textBoxDistance.Text = "";
            else
                textBoxDistance.Text = string.Format("{0:F1}", m_dDistance.Data);

            cboMixedFactor.Text = m_strMixedFactor;
        }

        private void SetGrid(DataGridView grid, List<string> items)
        {
            grid.Rows.Clear();

            foreach (string strItem in items)
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = strItem;
                row.Cells.Add(cell);
                grid.Rows.Add(row);
            }
        }

        private void LoadPatientItems()
        {
            m_patientItems.Clear();
            ScenarioToDB.Component component = GetComponent(22);

            if (component == null)
                return;

            ScenarioToDB.PropertyProcess prop = (ScenarioToDB.PropertyProcess)component.Property;

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
            ScenarioToDB.Component component = GetComponent(6);

            if (component == null)
                return;

            ScenarioToDB.PropertyProcess prop = (ScenarioToDB.PropertyProcess)component.Property;

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

                        double distance;

                        if (double.TryParse(strDistance, out distance))
                        {
                            m_dDistance = new VariousData<double>(distance);
                            //m_dDistance = 0.0;
                        }
                    }
                }
            }
        }

        private void LoadActions()
        {
            m_actions.Clear();
            ScenarioToDB.Component component = GetComponent(16);

            if (component == null)
                return;

            ScenarioToDB.PropertyProcess prop = (ScenarioToDB.PropertyProcess)component.Property;

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
            ScenarioToDB.Component component = GetComponent(15);

            if (component == null)
                return;

            ScenarioToDB.PropertyProcess prop = (ScenarioToDB.PropertyProcess)component.Property;

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
            ScenarioToDB.Component component = GetComponent(3);

            if (component == null)
                return;

            ScenarioToDB.PropertyProcess prop = (ScenarioToDB.PropertyProcess)component.Property;

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

                        double dInitialDistance;

                        if (double.TryParse(strInitial, out dInitialDistance))
                        {
                            m_dInitialDistance = new VariousData<double>(dInitialDistance);
                            //m_dInitialDistance = 0.0;
                        }
                    }
                }
            }
        }

        private void LoadAccident()
        {
            ScenarioToDB.Component component = GetComponent(32);

            if (component == null)
                return;

            string strTarget = " 진압";
            ScenarioToDB.PropertyProcess prop = (ScenarioToDB.PropertyProcess)component.Property;

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
            ScenarioToDB.Component component = GetComponent(9);

            if (component == null)
                return;

            ScenarioToDB.PropertyProcess prop = (ScenarioToDB.PropertyProcess)component.Property;

            int nCount = prop.Missions.Count;

            for (int i = 0; i < nCount; i++)
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

                        int nCountOfDeath;

                        if (int.TryParse(strCountOfDeath, out nCountOfDeath))
                        {
                            m_nCountOfDeath = new VariousData<int>(nCountOfDeath);
                            //m_nCountOfDeath = 0;
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

                        int nCountOfBuilding;

                        if (int.TryParse(strCountOfBuilding, out nCountOfBuilding))
                        {
                            m_nCountOfBuilding = new VariousData<int>(nCountOfBuilding);
                            //m_nCountOfBuilding = 0;
                        }
                    }
                }
            }
        }

        private void LoadChemistryNWeather()
        {
            ScenarioToDB.Component component = GetComponent(7);

            if (component == null)
                return;

            string strTarget = "누출물질: ";
            ScenarioToDB.PropertyProcess prop = (ScenarioToDB.PropertyProcess)component.Property;

            foreach (Sections.MissionItem item in prop.Missions)
            {
                int nIndex = item.Mission.IndexOf(strTarget);

                if (nIndex >= 0)
                {
                    string strChemistry = item.Mission.Substring(nIndex + strTarget.Length);
                    m_strChemistry = strChemistry;

                    /*if (m_strChemistry.Contains("플루오르"))
                        m_strChemistry = "플루오르화수소";
                    else */if (m_strChemistry.Contains("황산"))
                        m_strChemistry = "황산";
                    else
                        return;
                }
                else if (!item.Mission.Contains("에서"))
                {
                    m_strWeather = item.Mission;
                }
            }
        }

        private void LoadTime()
        {
            ScenarioToDB.Component component = GetComponent(8);

            if (component == null)
                return;

            if (component.Text == "10:00")
                m_isDayLight = new VariousData<bool>(true);
            else
                m_isDayLight = new VariousData<bool>(false);
        }

        private ScenarioToDB.Component GetComponent(int nComponentID)
        {
            foreach (ScenarioToDB.ActionStep actionStep in m_arrActionSteps)
            {
                foreach (ScenarioToDB.StepMember stepMember in actionStep.StepMemberList)
                {
                    foreach (ScenarioToDB.Component component in stepMember.ComponentList)
                    {
                        if (component.ID == nComponentID)
                            return component;
                    }
                }
            }

            return null;
        }

        private void cboMaterialName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMaterialName.SelectedItem == null)
                return;

            string strSelectedItem = cboMaterialName.SelectedItem.ToString();

            if (strSelectedItem == "벤젠")
                SetMixedFactor("열", "산화성물질", "없음");
            else if (strSelectedItem == "산화질소")
                SetMixedFactor("물", "가연성물질", "없음");
            else if (strSelectedItem == "암모니아")
                SetMixedFactor("물", "산", "열", "없음");
            else if (strSelectedItem == "염소")
                SetMixedFactor("물", "열", "없음");
            else if (strSelectedItem == "황산" || strSelectedItem == "질산")
                SetMixedFactor("물", "열", "가연성물질", "없음");
            else
                SetMixedFactor("물", "열", "없음");
        }

        private void SetMixedFactor(string strFactor1, string strFactor2, string strFactor3, string strFactor4 = null)
        {
            cboMixedFactor.Items.Clear();

            cboMixedFactor.Items.Add(strFactor1);
            cboMixedFactor.Items.Add(strFactor2);
            cboMixedFactor.Items.Add(strFactor3);

            if (strFactor4 != null)
                cboMixedFactor.Items.Add(strFactor4);
        }
    }

    // struct와 같이 null이 허용되지 않는 데이터를 위한 Wrapper 클래스
    public class VariousData<DataType>
    {
        private DataType data;

        public DataType Data
        {
            get { return data; }
            set { data = value; }
        }

        public VariousData()
        {
        }

        public VariousData(DataType data)
        {
            this.data = data;
        }
    }
}
