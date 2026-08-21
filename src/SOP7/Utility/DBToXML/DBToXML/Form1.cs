using SensorMaker.BLL.Models.Data.Sensor;
using SensorMaker.BLL.Models.Request;
using SensorMaker.BLL.Models.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static dnsData.Sensor.Facility;

namespace DBToXML
{
    public partial class Form1 : Form
    {
        private SensorMaker.BLL.ProcessManager m_processManager = null;

        private int m_nSiteID = -1;
        private bool m_bReady = false;

        public Form1()
        {
            InitializeComponent();

            cbDBType.SelectedIndex = 0;
        }

        private void btnConnet_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtWebServerURL.TextLength == 0)
                    throw new ApplicationException("URL을 입력하세요");

                if (txtDBName.TextLength == 0)
                    throw new ApplicationException("DB Name을 입력하세요");

                if (txtSiteID.TextLength == 0)
                    throw new ApplicationException("Site ID를 입력하세요");

                int nSiteID;
                if (!int.TryParse(txtSiteID.Text, out nSiteID))
                    throw new ApplicationException("Site ID는 숫자로 입력하세요");

                m_nSiteID = nSiteID;

                SDMS.IDAL.IDataManager iSdmsDataManager = new SDMS.DAL.DataManager(txtDBName.Text, cbDBType.SelectedIndex, m_nSiteID, txtWebServerURL.Text);
                TeamEditor.IDAL.IDataManager iTeamDataManaer = new TeamEditor.DAL.DataManager();
                Common.IDAL.IDataManager iCommonDataManager = new Common.DAL.DataManager(txtDBName.Text, cbDBType.SelectedIndex, m_nSiteID, txtWebServerURL.Text);
                SOPManager.IDAL.IDataManager iSopDataManager = new SOPManager.DAL.DataManager(txtDBName.Text, cbDBType.SelectedIndex, m_nSiteID, txtWebServerURL.Text);
                m_processManager = new SensorMaker.BLL.ProcessManager(iTeamDataManaer, iCommonDataManager, iSdmsDataManager, iSopDataManager);
                
                bool bConnect = CheckConnectDB();
                if (bConnect)
                {
                    lblConnectDB.Text = "연결되었습니다.";
                    lblConnectDB.ForeColor = Color.Green;
                    m_bReady = true;
                }
                else
                {
                    lblConnectDB.Text = "연결 실패했습니다.";
                    lblConnectDB.ForeColor = Color.Red;
                    m_bReady = false;
                }

            }
            catch (Exception ex)
            {
                lblConnectDB.Text = ex.Message;
                lblConnectDB.ForeColor = Color.Red;
                m_bReady = false;
            }
        }

        private bool CheckConnectDB()
        {            
            string strErrorMessage;
            SDMS.Model.Spatial.Building building = m_processManager.SdmsDataManager.GetSelectManager().SelectBuilding(1, out strErrorMessage);
            if (building == null)
                return false;

            return true;
        }

        private void btnDBToXML_Click(object sender, EventArgs e)
        {
            try
            {
                if (!m_bReady)
                    throw new ApplicationException("연결된 DB가 없네요");

                string strSavePath = "";

                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Title = "XML 저장";
                dlg.DefaultExt = "xml";
                dlg.Filter = "*XML files|*.xml";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    strSavePath = dlg.FileName;
                }

                List<int> siteIDs = new List<int>();
                siteIDs.Add(m_nSiteID);

                SensorMaker.BLL.LoadManager loadManager = m_processManager.GetLoadManager();                
                if (!loadManager.LoadSpatial())
                    MessageBox.Show("LoadSpatial 실패");

                ResponseSensorList res2 = loadManager.GetSensorList();
                if (!res2.Success)
                    MessageBox.Show("Sensor 정보 읽기 실패\r\n" + res2.Message);

                List<SensorMaker.BLL.Models.Basic.SensorType> sensorTypes = loadManager.GetSensorTypes();
                // SensorSubType 넣기
                SetSensorTypes(sensorTypes, res2.FireSensors);

                ResponseGltfDataList resGltfDataList = loadManager.RequestGltfModelList(siteIDs);

                RequestSaveXML req = new RequestSaveXML();
                req.TestBuildingGroupData = loadManager.GetBuildingGroups(siteIDs);
                req.TestBuildingData = loadManager.GetBuildings();
                req.TestZoneData = loadManager.GetZones();
                req.TestEquipmentZoneData = loadManager.GetEquipmentZones();
                req.OutdoorZones = loadManager.GetOutdoorZones(siteIDs);
                req.FireSensors = res2.FireSensors;
                req.PSMSensors = res2.PSMSensors;
                req.EtcSensors = res2.EtcSensors;
                req.Cctvs = res2.Cctvs;
                req.SensorTypes = sensorTypes;
                req.Models = resGltfDataList.Models;
                req.GltfOption = resGltfDataList.GltfOption;

                ResponseSaveXML resSave = m_processManager.GetXmlManager().SaveXML(req);
                resSave.XDocument.Save(strSavePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SetSensorTypes(List<SensorMaker.BLL.Models.Basic.SensorType> sensorTypes, List<FireSensor> fireSensors)
        {
            SensorMaker.BLL.Models.Basic.SensorType fireSensorType = null;
            foreach (SensorMaker.BLL.Models.Basic.SensorType type in sensorTypes)
            {
                if (type.ID == (int)FacilityType.FIRE_SENSOR)
                {
                    fireSensorType = type;
                    break;
                }
            }

            if (fireSensorType == null)
                return;

            IEnumerable<int?> ids = fireSensors.Select(p => p.SensorSubType).Distinct();

            foreach (int? item in ids)
            {
                int nID = item == null ? -1 : (int)item;
                SensorMaker.BLL.Models.Basic.SensorSubType subType = new SensorMaker.BLL.Models.Basic.SensorSubType();
                subType.ID = nID;
                subType.Name = dnsData.Sensor.Facility.GetFacilitySubTypeString((FacilitySubType)nID);

                fireSensorType.SubType.Add(subType);
            }
        }

        private void btnReadXML_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                ResponseOpenXML res = m_processManager.GetXmlManager().OpenXML(dlg.FileName);                
            }
        }
    }
}
