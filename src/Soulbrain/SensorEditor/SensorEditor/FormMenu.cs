using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Configuration;
using SDMS.DAL;
using SDMS.Model.Spatial;
using SDMS.Model.Sensor;
using SDMS.Model.CCTV;
using UnE.Geometry;

namespace SensorEditor
{
    public partial class FormMenu : Form
    {
        private const int Fire_Index = 0;
        private const int CCTV_Index = 1;

        private DataManager m_dataManager = null;
        private FormMain m_frmMain = null;

        private Dictionary<int, BuildingGroup> m_dicBuildingGroups = new Dictionary<int, BuildingGroup>();
        private Dictionary<int, Building> m_dicBuildings = new Dictionary<int, Building>();
        private Dictionary<int, Zone> m_dicZones = new Dictionary<int, Zone>();
        private Dictionary<int, Fire> m_dicFireSensors = new Dictionary<int, Fire>();
        private Dictionary<int, CCTV> m_dicCCTVs = new Dictionary<int, CCTV>();

        private List<BuildingGroup> m_buildingGroups = new List<BuildingGroup>();
        private List<Building> m_buildings = new List<Building>();
        private List<Zone> m_zones = new List<Zone>();
        private List<Fire> m_fireSensors = new List<Fire>();
        private List<CCTV> m_cctvs = new List<CCTV>();

        private bool m_systemInput = false;

        public FormMenu(FormMain frmMain)
        {
            InitializeComponent();
            m_frmMain = frmMain;
            SetDBManager();
        }

        private bool SetDBManager()
        {
            string strSiteID = ConfigurationManager.AppSettings["siteid"].ToString();
            string strDBName = ConfigurationManager.AppSettings["dbName"].ToString();
            string strDBType = ConfigurationManager.AppSettings["dbType"].ToString();
            string strWebServerURL = ConfigurationManager.AppSettings["webServerURL"].ToString();

            int nSiteID, nDBType;

            if (int.TryParse(strSiteID, out nSiteID) == false || int.TryParse(strDBType, out nDBType) == false)
                return false;

            m_dataManager = new DataManager(strDBName, nDBType, nSiteID, strWebServerURL);
            return true;
        }

        private void FormMenu_Load(object sender, EventArgs e)
        {
            if (m_dataManager != null)
            {
                LoadBuildingGroups();
                LoadBuildings();
                LoadZones();
                LoadFireSensors();
                LoadCCTVs();

                m_systemInput = true;
                cboSensorType.SelectedIndex = Fire_Index;
                m_systemInput = false;

                if (cboBuildingGroup.Items.Count > 0)
                {
                    cboBuildingGroup.SelectedIndex = 0;
                }
            }
        }

        private void LoadBuildingGroups()
        {
            m_dicBuildingGroups.Clear();
            m_buildingGroups.Clear();
            cboBuildingGroup.Items.Clear();

            string strErrorMessage;
            List<BuildingGroup> buildingGroups = m_dataManager.GetSelectManager().SelectBuildingGroups(null, null, out strErrorMessage);

            if (buildingGroups != null)
            {
                foreach (BuildingGroup buildingGroup in buildingGroups)
                {
                    m_dicBuildingGroups[buildingGroup.ID] = buildingGroup;
                }
            
                m_buildingGroups.AddRange(buildingGroups);

                foreach (BuildingGroup buildingGroup in buildingGroups)
                {
                    cboBuildingGroup.Items.Add(buildingGroup.DisplayText);
                }
            }
        }

        private void LoadBuildings()
        {
            m_dicBuildings.Clear();

            string strErrorMessage;
            List<Building> buildings = m_dataManager.GetSelectManager().SelectBuildings(null, null, out strErrorMessage);

            if (buildings != null)
            {
                foreach (Building building in buildings)
                {
                    m_dicBuildings[building.ID] = building;
                }
            }
        }

        private void LoadZones()
        {
            m_dicZones.Clear();

            string strErrorMessage;
            List<Zone> zones = m_dataManager.GetSelectManager().SelectZones(null, null, out strErrorMessage);

            if (zones != null)
            {
                foreach (Zone zone in zones)
                {
                    m_dicZones[zone.ID] = zone;
                }
            }
        }

        private void LoadFireSensors()
        {
            m_dicFireSensors.Clear();

            string strErrorMessage;
            List<Fire> fireSensors = m_dataManager.GetSelectManager().SelectFireSensors(null, null, out strErrorMessage);

            if (fireSensors != null)
            {
                foreach (Fire sensor in fireSensors)
                {
                    m_dicFireSensors[sensor.ID] = sensor;
                }
            }
        }

        private void LoadCCTVs()
        {
            m_dicCCTVs.Clear();

            string strErrorMessage;
            List<CCTV> cctvs = m_dataManager.GetSelectManager().SelectCCTVs(null, null, out strErrorMessage);

            if (cctvs != null)
            {
                foreach (CCTV cctv in cctvs)
                {
                    m_dicCCTVs[cctv.ID] = cctv;
                }
            }
        }

        private void cboBuildingGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboBuildingGroup.SelectedIndex < 0)
            {
                cboZone.Items.Clear();
                cboBuilding.Items.Clear();
                return;
            }

            BuildingGroup buildingGroup = m_buildingGroups[cboBuildingGroup.SelectedIndex];
            m_buildings.Clear();
            cboBuilding.Items.Clear();

            foreach (KeyValuePair<int, Building> pair in m_dicBuildings)
            {
                if (pair.Value.BuildingGroupID == buildingGroup.ID)
                {
                    m_buildings.Add(pair.Value);
                    cboBuilding.Items.Add(pair.Value.DisplayText);
                }
            }

            if (m_buildings.Count > 0)
            {
                cboBuilding.SelectedIndex = 0;
            }
        }

        private void cboBuilding_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboBuilding.SelectedIndex < 0)
            {
                cboZone.Items.Clear();
                return;
            }

            Building building = m_buildings[cboBuilding.SelectedIndex];
            m_zones.Clear();
            cboZone.Items.Clear();

            foreach (KeyValuePair<int, Zone> pair in m_dicZones)
            {
                if (pair.Value.BuildingID != null && pair.Value.BuildingID == building.ID)
                {
                    m_zones.Add(pair.Value);
                    cboZone.Items.Add(pair.Value.DisplayText);
                }
            }

            if (cboZone.Items.Count > 0)
            {
                cboZone.SelectedIndex = 0;
            }
        }

        private void cboZone_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboZone.SelectedIndex < 0)
            {
                cboSensor.Items.Clear();
                return;
            }

            Zone zone = m_zones[cboZone.SelectedIndex];
            m_fireSensors.Clear();
            m_cctvs.Clear();
            cboSensor.Items.Clear();

            if (cboSensorType.SelectedIndex == Fire_Index)
            {
                foreach (KeyValuePair<int, Fire> pair in m_dicFireSensors)
                {
                    if (pair.Value.ZoneID == zone.ID)
                    {
                        m_fireSensors.Add(pair.Value);
                        cboSensor.Items.Add(pair.Value.Name);
                    }
                }
            }
            else if (cboSensorType.SelectedIndex == CCTV_Index)
            {
                foreach (KeyValuePair<int, CCTV> pair in m_dicCCTVs)
                {
                    if (pair.Value.ZoneID == zone.ID)
                    {
                        m_cctvs.Add(pair.Value);
                        cboSensor.Items.Add(pair.Value.CameraName);
                    }
                }
            }

            if (cboSensor.Items.Count > 0)
            {
                cboSensor.SelectedIndex = 0;
            }
        }

        private void cboSensorType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_systemInput)
                return;

            if (cboZone.SelectedIndex < 0)
                return;

            Zone zone = m_zones[cboZone.SelectedIndex];
            m_fireSensors.Clear();
            m_cctvs.Clear();
            cboSensor.Items.Clear();

            if (cboSensorType.SelectedIndex == Fire_Index)
            {
                foreach (KeyValuePair<int, Fire> pair in m_dicFireSensors)
                {
                    if (pair.Value.ZoneID == zone.ID)
                    {
                        m_fireSensors.Add(pair.Value);
                        cboSensor.Items.Add(pair.Value.Name);
                    }
                }
            }
            else if (cboSensorType.SelectedIndex == CCTV_Index)
            {
                foreach (KeyValuePair<int, CCTV> pair in m_dicCCTVs)
                {
                    if (pair.Value.ZoneID == zone.ID)
                    {
                        m_cctvs.Add(pair.Value);
                        cboSensor.Items.Add(pair.Value.CameraName);
                    }
                }
            }

            if (cboSensor.Items.Count > 0)
            {
                cboSensor.SelectedIndex = 0;
            }
        }

        private void cboSensor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSensorType.SelectedIndex < 0 || cboSensor.SelectedIndex < 0)
                return;

            if (cboSensorType.SelectedIndex == Fire_Index)
            {
                Fire sensor = m_fireSensors[cboSensor.SelectedIndex];

                textBoxX.Text = string.Format("{0:F2}", sensor.X);
                textBoxZ.Text = string.Format("{0:F2}", sensor.Z);

                if (sensor.X != null && sensor.Z != null)
                    m_frmMain.ShowSensor((float)sensor.X, (float)sensor.Z);
            }
            else if (cboSensorType.SelectedIndex == CCTV_Index)
            {
                CCTV cctv = m_cctvs[cboSensor.SelectedIndex];

                textBoxX.Text = string.Format("{0:F2}", cctv.X);
                textBoxZ.Text = string.Format("{0:F2}", cctv.Z);

                if (cctv.X != null && cctv.Z != null)
                    m_frmMain.ShowSensor((float)cctv.X, (float)cctv.Z);
            }
        }

        public void SetSensorLocation(Vertex2D vPos)
        {
            textBoxX.Text = string.Format("{0:F2}", vPos.x);
            textBoxZ.Text = string.Format("{0:F2}", vPos.y);
        }

        private void btnSaveDB_Click(object sender, EventArgs e)
        {
            if (cboSensorType.SelectedIndex < 0 || cboSensor.SelectedIndex < 0)
                return;

            string strX = textBoxX.Text.Trim();
            string strZ = textBoxZ.Text.Trim();

            if (strX.Length == 0)
            {
                textBoxX.Focus();
                MessageBox.Show("좌표를 입력하세요.");
                return;
            }

            if (strZ.Length == 0)
            {
                textBoxZ.Focus();
                MessageBox.Show("좌표를 입력하세요.");
                return;
            }

            double x, z;

            if (double.TryParse(strX, out x) == false)
            {
                textBoxX.Focus();
                MessageBox.Show("숫자만 입력가능합니다.");
                return;
            }

            if (double.TryParse(strZ, out z) == false)
            {
                textBoxZ.Focus();
                MessageBox.Show("숫자만 입력가능합니다.");
                return;
            }

            string strErrorMessage;

            if (cboSensorType.SelectedIndex == Fire_Index)
            {
                Fire sensor = m_fireSensors[cboSensor.SelectedIndex];

                Dictionary<Fire.Fields, object> dicSets = new Dictionary<Fire.Fields, object>();
                dicSets[Fire.Fields.X] = (float)x;
                dicSets[Fire.Fields.Z] = (float)z;

                Dictionary<Fire.Fields, object> dicConditions = new Dictionary<Fire.Fields, object>();
                dicConditions[Fire.Fields.ID] = sensor.ID;

                if (m_dataManager.GetUpdateManager().UpdateFireSensor(dicSets, dicConditions, "", out strErrorMessage))
                {
                    sensor.X = (float)x;
                    sensor.Z = (float)z;
                }
            }
            else if (cboSensorType.SelectedIndex == CCTV_Index)
            {
                CCTV cctv = m_cctvs[cboSensor.SelectedIndex];

                Dictionary<CCTV.Fields, object> dicSets = new Dictionary<CCTV.Fields, object>();
                dicSets[CCTV.Fields.X] = (float)x;
                dicSets[CCTV.Fields.Z] = (float)z;

                Dictionary<CCTV.Fields, object> dicConditions = new Dictionary<CCTV.Fields, object>();
                dicConditions[CCTV.Fields.ID] = cctv.ID;

                if (m_dataManager.GetUpdateManager().UpdateCCTV(dicSets, dicConditions, "", out strErrorMessage))
                {
                    cctv.X = (float)x;
                    cctv.Z = (float)z;
                }
            }
        }
    }
}
