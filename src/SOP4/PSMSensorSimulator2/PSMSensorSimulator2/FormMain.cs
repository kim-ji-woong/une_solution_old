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
using DBUtility;
using SensorTester;

namespace PSMSensorSimulator2
{
    public partial class FormMain : Form
    {
        private int m_nSiteID = 1;
        private WebDBManager m_dbMgr = null;
        private DataManagerEx m_dataMgr = null;
        private List<PSMSensor> m_allSensors = null;

        public FormMain()
        {
            m_nSiteID = LoadSiteID();
            m_dbMgr = new WebDBManager(m_nSiteID);

            InitializeComponent();

            m_dataMgr = new DataManagerEx(m_dbMgr, m_nSiteID);
            m_allSensors = m_dataMgr.GetAllSensors();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private int LoadSiteID()
        {
            DBUtility.Utility ini = new DBUtility.Utility();
            string strSiteID = ini.getinivalue("Server Connection Info", "siteid");

            int nSiteID = 1;

            if (strSiteID.Length > 0)
            {
                int.TryParse(strSiteID, out nSiteID);
            }

            return nSiteID;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Key : SensorZone ID
            // Value : AlarmDepth
            Dictionary<int, int> dicSensorZoneIDs = LoadAlarms();
            List<PSMSensor> currentSensors = UpdateGrid(dicSensorZoneIDs);
            Simulate(currentSensors, dicSensorZoneIDs);
        }

        private void Simulate(List<PSMSensor> sensors, Dictionary<int, int> dicSensorZoneIDs)
        {
            if (sensors == null)
                return;

            foreach (PSMSensor sensor in m_allSensors)
            {
                if (radioAllSensors.Checked)
                {
                    if (Simulate(sensor, dicSensorZoneIDs) == false)
                        break;
                }
                else
                {
                    if (sensors.Contains(sensor))
                    {
                        if (Simulate(sensor, dicSensorZoneIDs) == false)
                            break;
                    }
                    else
                    {
                        if (Initialize(sensor) == false)
                            break;
                    }
                }
            }

            sensors.Clear();
        }

        private bool Initialize(PSMSensor sensor)
        {
            if (sensor.CurrentAlarmLevel == 0 && sensor.CurrentValue == 0.0f)
                return true;

            string strSQL = "Update PSMSensor set CurrentLevel = 0, CurrentData = 0 where ID = " + sensor.ID.ToString();

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return false;

            sensor.CurrentAlarmLevel = 0;
            sensor.CurrentValue = 0.0f;

            DataGridViewRow row = GetRow(sensor);

            if (row != null)
            {
                row.Cells[4].Value = "-";
                row.Cells[3].Value = string.Format("0.0 {0}", sensor.UOM);
            }

            return true;
        }

        private bool Simulate(PSMSensor sensor, Dictionary<int, int> dicSensorZoneIDs)
        {
            int min = 0, max = 0;
            int nAlarmDepth = 0;

            if (dicSensorZoneIDs.TryGetValue(sensor.SensorZoneID, out nAlarmDepth) == false)
                nAlarmDepth = 0;

            if (nAlarmDepth == 0)
                max = (int)(sensor.Alarm1Value * 1000) - 1;
            else if (nAlarmDepth == 1)
            {
                min = (int)(sensor.Alarm1Value * 1000);
                max = (int)(sensor.Alarm2Value * 1000) - 1;
            }
            else if (nAlarmDepth == 2)
            {
                min = (int)(sensor.Alarm2Value * 1000);
                max = (int)(sensor.Alarm3Value * 1000) - 1;
            }
            else if (nAlarmDepth == 3)
            {
                min = (int)(sensor.Alarm3Value * 1000);
                max = (int)(sensor.Alarm3Value * 2000) - 1;
            }
            else
                return false;

            DateTime dtNow = DateTime.Now;
            int nHour = dtNow.Hour * 3600 * 1000;
            int nMin = dtNow.Minute * 60 * 1000;
            int nSec = dtNow.Second * 1000;
            int nMilliSecond = dtNow.Millisecond;

            Random rand = new Random(nHour + nMin + nSec + nMilliSecond);
            int value = rand.Next(min, max);

            float fSensorValue = value / 1000.0f;

            int nAlarmBit = nAlarmDepth == 0 ? 0 : (1 << (nAlarmDepth - 1));
            
            string strSQL = string.Format("Update PSMSensor set CurrentLevel = {0}, CurrentData = {1} where ID = {2}",
                nAlarmBit, fSensorValue, sensor.ID);

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return false;

            sensor.CurrentAlarmLevel = nAlarmDepth;
            sensor.CurrentValue = fSensorValue;

            DataGridViewRow row = GetRow(sensor);

            if (row != null)
            {
                if (nAlarmDepth == 0)
                    row.Cells[4].Value = "-";
                else
                    row.Cells[4].Value = nAlarmDepth.ToString() + " 단계";

                row.Cells[3].Value = string.Format("{0:F1} {1}", fSensorValue, sensor.UOM);
            }

            return true;
        }

        private DataGridViewRow GetRow(PSMSensor sensor)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Tag == sensor)
                    return row;
            }

            return null;
        }

        // Return 값 : 현재 알람이 발생한(Grid에 표시되고 있는) PSMSensor List
        private List<PSMSensor> UpdateGrid(Dictionary<int, int> dicSensorZoneIDs)
        {
            if (dicSensorZoneIDs == null)
                return null;

            List<int> insertSensorZoneIDs = dicSensorZoneIDs.Keys.ToList();
            List<DataGridViewRow> removeRows = new List<DataGridViewRow>();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow || row.Tag == null)
                    continue;

                PSMSensor sensor = (PSMSensor)row.Tag;

                if (dicSensorZoneIDs.ContainsKey(sensor.SensorZoneID))
                    insertSensorZoneIDs.Remove(sensor.SensorZoneID);
                else
                    removeRows.Add(row);
            }

            foreach (DataGridViewRow row in removeRows)
            {
                dataGridView1.Rows.Remove(row);
            }

            List<PSMSensor> currentSensors = new List<PSMSensor>();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells[0].Value = row.Index + 1;
                currentSensors.Add((PSMSensor)row.Tag);
            }

            foreach (int nSensorZoneID in insertSensorZoneIDs)
            {
                SensorTag sensor = m_dataMgr.GetSensorTagBySensorZoneID(nSensorZoneID);

                if (sensor == null || sensor.SensorZone == null || sensor.TagType != SensorTag.SensorType.PSM센서)
                    continue;

                PSMSensor psmSensor = m_dataMgr.GetPSMSensorFromSensorZone(sensor.SensorZone.ID);

                if (psmSensor == null)
                    continue;

                DataGridViewRow row = MakeNewRow(dataGridView1);

                row.Cells[0].Value = row.Index + 1;
                row.Cells[1].Value = psmSensor.LocationName;
                row.Cells[2].Value = psmSensor.MaterialName;
                row.Tag = psmSensor;

                currentSensors.Add(psmSensor);
            }

            insertSensorZoneIDs.Clear();
            return currentSensors;
        }

        // Key : SensorZone ID
        // Value : AlarmDepth
        private Dictionary<int, int> LoadAlarms()
        {
            string szText = "SELECT srh.id, srh.SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, srh.Param1, srh.Param2, srh.Param3, srh.Param4, srh.Param5, szh.SensorID ";
            szText += "FROM SensorReactionHistory as srh, SensorZoneHistory as szh, SensorZone as sz, EquipmentZone as ez ";
            szText += "WHERE SensorHistoryID in (";
            szText += "         SELECT srh2.SensorHistoryID ";
            szText += "         FROM SensorReactionHistory as srh2, SensorZoneHistory as szh2 ";
            szText += "         WHERE szh2.Id = srh2.SensorHistoryID and srh2.ReactionType in ( 0, 60, 62) ) ";
            szText += "     AND SensorHistoryID not in (";
            szText += "         SELECT srh3.SensorHistoryID ";
            szText += "         FROM SensorReactionHistory as srh3, SensorZoneHistory as szh3 ";
            szText += "         WHERE szh3.Id = srh3.SensorHistoryID and srh3.ReactionType in (21, 23, 33, 50, 70)) ";
            szText += "     AND srh.SensorHistoryID = szh.ID ";
            szText += "     AND szh.SensorID = sz.ID ";
            szText += "     AND sz.EquipZoneID = ez.ID ";
            szText += "     AND ez.SiteID = {0} ";
            szText += "     AND ( srh.Time between DATEADD(hour,-24,getdate()) and GETDATE()) ";
            szText += "     ORDER BY srh.Time, szh.SensorID";

            string strSQL = string.Format(szText, m_nSiteID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            DateTime dtDefault = new DateTime();

            SensorReactionLog log = new SensorReactionLog();
            bool isSuccess;
            int nSensorID = -1;

            ArrayList arrTimeHistory = new ArrayList();

            //SortedList<int, int> keyExistList = new SortedList<int, int>();

            Dictionary<int, int> dicSensorZoneIDs = new Dictionary<int, int>();
            //List<int> sensorZoneIDs = new List<int>();

            for (int i = 0; i < nResultCount - 10; i += 11)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nHistoryID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nReactionType = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                DateTime time = DBUtility.WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
                string strMessage = DBUtility.WebDBManager.GetStringField(arrResult[i + 4], "");
                string strParam1 = DBUtility.WebDBManager.GetStringField(arrResult[i + 5], "");
                string strParam2 = DBUtility.WebDBManager.GetStringField(arrResult[i + 6], "");
                string strParam3 = DBUtility.WebDBManager.GetStringField(arrResult[i + 7], "");
                string strParam4 = DBUtility.WebDBManager.GetStringField(arrResult[i + 8], "");
                string strParam5 = DBUtility.WebDBManager.GetStringField(arrResult[i + 9], "");

                nSensorID = DBUtility.WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);

                if (nReactionType == (int)SensorReactionLog.ReactionType.BEGIN_PSM_STATUS || nReactionType == (int)SensorReactionLog.ReactionType.CHANGE_PSM_ALARM_DEPTH)
                {
                    nSensorID = DBUtility.WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);
                }

                if (nID < 0 || nHistoryID < 0)
                    continue;

                /*string szHashKey = nHistoryID.ToString() + "_-_" + nReactionType.ToString() + "_-_" + strMessage;
                int nHash = szHashKey.GetHashCode();
                if (keyExistList.ContainsKey(nHash))
                    continue;

                keyExistList.Add(nHash, nHash);*/


                SensorReactionLog.ReactionType type = SensorReactionLog.ToReactionType(nReactionType, out isSuccess);

                // 방송정보와 sms송신 로그는 보내지 않는다.
                if (type == SensorReactionLog.ReactionType.SEND_SMS || type == SensorReactionLog.ReactionType.RUN_BROADCAST)
                    continue;

                if (!isSuccess)
                    continue;

                // 화학물질 센서는 통합처리되므로 data가 같은 SensorZone이므로 각기 SensorZone의 Data를 확인하도록 한다.
                // skkim 2016-02-26 
                string szText2 = "SELECT Data FROM SensorZone WHERE ID = {0}";
                string szSQL2 = string.Format(szText2, nSensorID);
                ArrayList arrResult2 = m_dbMgr.GetResultData(szSQL2, 0);
                if (arrResult2 == null || arrResult2.Count == 0)
                    continue;

                int nSensorData = DBUtility.WebDBManager.GetIntField(arrResult2[0].ToString(), -1);
                if (nSensorData == 1 || nSensorData == 21 || nSensorData == 22 || nSensorData == 23)
                {
                    int nAlarmDepth = 0;
                    int.TryParse(strParam5, out nAlarmDepth);

                    dicSensorZoneIDs[nSensorID] = nAlarmDepth;

                    /*if (!sensorZoneIDs.Contains(nSensorID))
                        sensorZoneIDs.Add(nSensorID);*/
                }
            }

            return dicSensorZoneIDs;
            //return sensorZoneIDs;
        }

        public static DataGridViewRow MakeNewRow(DataGridView grid)
        {
            int nRowIndex = grid.Rows.Add();

            if (nRowIndex < 0)
                return null;

            return grid.Rows[nRowIndex];
        }
    }
}
