using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using DBUtility;

namespace PSMSensorSimulator2
{
    public class DataManagerEx : SensorTester.DataManager
    {
        // Key : PSMSensor ID
        private Dictionary<int, PSMSensor> m_dicPSMSensors = new Dictionary<int, PSMSensor>();
        // Key : SensorZone ID
        private Dictionary<int, PSMSensor> m_dicPSMSensorsFromSensorZone = new Dictionary<int, PSMSensor>();
        private WebDBManager m_dbMgr = null;
        private int m_nSiteID = -1;

        public DataManagerEx(WebDBManager dbMgr, int nSiteID)
            : base(dbMgr, nSiteID)
        {
            m_dbMgr = dbMgr;
            m_nSiteID = nSiteID;

            LoadPSMSensors();
        }

        private void LoadPSMSensors()
        {
            m_dicPSMSensors.Clear();
            m_dicPSMSensorsFromSensorZone.Clear();

            Dictionary<int, string> dicTankLocationName = LoadTankLocationNames();
            Dictionary<int, int> dicPSMSensorZones = LoadPSMSensorZones();

            if (dicTankLocationName == null || dicPSMSensorZones == null)
                return;

            string strSQL = "Select ps.ID, SensorName, LimitLevel1, LimitLevel2, LimitLevel3, TankIDList, EquipZoneID, pm.MaterialName, pm.UOM, ps.CurrentData, ps.CurrentLevel ";
            strSQL += "from PSMSensor as ps, PSMMaterial as pm where ps.MaterialType = pm.ID";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            string strLocationName = "";
            int nSensorZoneID = -1;
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-10;i+=11)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strSensorName = WebDBManager.GetStringField(arrResult[i + 1]);
                float fAlarm1 = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), -1.0f);
                float fAlarm2 = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), -1.0f);
                float fAlarm3 = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), -1.0f);
                string strTankIDs = WebDBManager.GetStringField(arrResult[i + 5]);
                int nEquipZoneID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                string strMaterialName = WebDBManager.GetStringField(arrResult[i + 7]);
                string strUOM = WebDBManager.GetStringField(arrResult[i + 8]);
                float fSensorValue = WebDBManager.GetFloatField(arrResult[i + 9].ToString(), -1.0f);
                int nAlarmDepth = WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);

                VariousData<int> tankID = GetFirstID(strTankIDs);

                if (tankID == null)
                    continue;

                if (!dicTankLocationName.TryGetValue(tankID.Data, out strLocationName))
                    continue;

                if (!dicPSMSensorZones.TryGetValue(nID, out nSensorZoneID))
                    continue;

                PSMSensor sensor = new PSMSensor();

                sensor.ID = nID;
                sensor.SensorName = strSensorName;
                sensor.Alarm1Value = fAlarm1;
                sensor.Alarm2Value = fAlarm2;
                sensor.Alarm3Value = fAlarm3;
                sensor.EquipZoneID = nEquipZoneID;
                sensor.LocationName = strLocationName;
                sensor.MaterialName = strMaterialName;
                sensor.UOM = strUOM;
                sensor.SensorZoneID = nSensorZoneID;
                sensor.CurrentValue = fSensorValue;
                sensor.CurrentAlarmLevel = nAlarmDepth;

                m_dicPSMSensors[nID] = sensor;
                m_dicPSMSensorsFromSensorZone[nSensorZoneID] = sensor;
            }
        }

        // Key : Origin SensorID
        // Value : SensorZone ID
        private Dictionary<int, int> LoadPSMSensorZones()
        {
            string strSQL = "Select ID, OrgSensorID from SensorZone where Type = 11";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            Dictionary<int, int> dicPSMSensorZones = new Dictionary<int, int>();
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                int nSensorZoneID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nOrgSensorID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);

                dicPSMSensorZones[nOrgSensorID] = nSensorZoneID;
            }

            return dicPSMSensorZones;
        }

        private VariousData<int> GetFirstID(string strIDs)
        {
            if (strIDs == null)
                return null;

            string[] tokens = strIDs.Split(',');

            if (tokens.Count() == 0)
                return null;

            int nID;

            if (!int.TryParse(tokens[0].Trim(), out nID))
                return null;

            return new VariousData<int>(nID);
        }

        private Dictionary<int, string> LoadTankLocationNames()
        {
            string strSQL = "Select ID, LocationName from PSMTank";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            Dictionary<int, string> dicTankLocationNames = new Dictionary<int, string>();
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strLocationName = WebDBManager.GetStringField(arrResult[i + 1]);

                dicTankLocationNames[nID] = strLocationName;
            }

            return dicTankLocationNames;
        }

        public PSMSensor GetPSMSensor(int nSensorID)
        {
            PSMSensor sensor;

            if (m_dicPSMSensors.TryGetValue(nSensorID, out sensor))
                return sensor;

            return null;
        }

        public PSMSensor GetPSMSensorFromSensorZone(int nSensorZoneID)
        {
            PSMSensor sensor;

            if (m_dicPSMSensorsFromSensorZone.TryGetValue(nSensorZoneID, out sensor))
                return sensor;

            return null;
        }

        public List<PSMSensor> GetAllSensors()
        {
            return m_dicPSMSensors.Values.ToList();
        }
    }

    public class PSMSensor
    {
        private int m_nID = -1;
        private string m_strSensorName = "";
        private float m_fAlarm1Value = -1.0f;
        private float m_fAlarm2Value = -1.0f;
        private float m_fAlarm3Value = -1.0f;
        private string m_strLocationName = "";
        private string m_strMaterialName = "";
        private string m_strUOM = "";
        // 현재 알람 단계
        private int m_nCurrentAlarmLevel = 0;
        // 현재 센서 수치
        private float m_fCurrentValue = 0.0f;
        private int m_nEquipZoneID = -1;
        private int m_nSensorZoneID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string SensorName
        {
            get { return m_strSensorName; }
            set { m_strSensorName = value; }
        }

        public float Alarm1Value
        {
            get { return m_fAlarm1Value; }
            set { m_fAlarm1Value = value; }
        }

        public float Alarm2Value
        {
            get { return m_fAlarm2Value; }
            set { m_fAlarm2Value = value; }
        }

        public float Alarm3Value
        {
            get { return m_fAlarm3Value; }
            set { m_fAlarm3Value = value; }
        }

        public string LocationName
        {
            get { return m_strLocationName; }
            set { m_strLocationName = value; }
        }

        public string MaterialName
        {
            get { return m_strMaterialName; }
            set { m_strMaterialName = value; }
        }

        public string UOM
        {
            get { return m_strUOM; }
            set { m_strUOM = value; }
        }

        // 현재 알람 단계
        public int CurrentAlarmLevel
        {
            get { return m_nCurrentAlarmLevel; }
            set { m_nCurrentAlarmLevel = value; }
        }

        // 현재 센서 수치
        public float CurrentValue
        {
            get { return m_fCurrentValue; }
            set { m_fCurrentValue = value; }
        }

        public int EquipZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }

        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }
    }
}
