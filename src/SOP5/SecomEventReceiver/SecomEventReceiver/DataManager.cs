using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility;
using System.Collections;
using System.Threading;

namespace SecomEventReceiver
{
    class DataManager
    {
        private WebDBManager m_dbMgr = null;
        private string m_strSecomDBName = "";
        // Key : Secom DB에 저장된 설비명(또는 공간명)
        // Value : Key에 해당하는 EquipmentZone ID
        //private Dictionary<string, int> m_dicSecomEqEquipZoneID = new Dictionary<string, int>();
        private int m_nSiteID = 1;

        //private string m_strLastDate = "";
        //private string m_strLastTime = "";
        //private int m_nLastID = -1;

        private bool stopWatching = false;

        // 마지막으로 지난 로그를 삭제한 날짜
        private int m_nLastLogYear = -1, m_nLastLogMonth = -1, m_nLastLogDay = -1;

        private static DataManager m_instance = null;

        public static DataManager Instance
        {
            get { return m_instance; }
        }

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
            set { m_dbMgr = value; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public DataManager()
        {
            m_instance = this;
            m_nSiteID = UnE.Util.UtilMethods.ReadSiteID();
            m_dbMgr = new WebDBManager(m_nSiteID);
            LoadDBOption();
            m_strSecomDBName = ReadSecomDBName();

            //CheckLastRead();
        }

        private bool LoadDBOption()
        {
            string strWebServerURL = m_dbMgr.LoadIni("webserver_url", "Server Connection Info").Trim();

            if (strWebServerURL.Length == 0)
                return false;

            string strServerIP = m_dbMgr.LoadIni("server_ip", "Server Connection Info").Trim();

            if (strServerIP.Length == 0)
                return false;

            string strServerPort = m_dbMgr.LoadIni("server_port", "Server Connection Info").Trim();

            if (strServerPort.Length == 0)
                return false;

            string strDBName = m_dbMgr.LoadIni("server_db", "Server Connection Info").Trim();

            if (strDBName.Length == 0)
                return false;

            if (strServerPort == "1433")
                m_dbMgr.DatabaseType = DBUtility.WebDBManager.DBType.sqlserver;
            else if (strServerPort == "3306")
                m_dbMgr.DatabaseType = DBUtility.WebDBManager.DBType.mysql;
            else
                return false;

            m_dbMgr.WebServerURL = strWebServerURL;
            m_dbMgr.DatabaseHost = strServerIP;
            m_dbMgr.DatabaseName = strDBName;
            return true;
        }

        // DB에서 가장 마지막에 기록된 데이터를 읽어, 그 이후부터 알람처리를 한다.
        // SecomEventeceiver가 꺼져있던 동안 발생한 알람은 무시한다.
        /*private void CheckLastRead()
        {
            string strSQL = "Select ADate, ATime, ID from Secom_Alarm order by ADate desc, ATime desc";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0, 1, m_strSecomDBName);

            if (arrResult == null || arrResult.Count < 3)
                return;

            string strDate = WebDBManager.GetStringField(arrResult[0]);
            string strTime = WebDBManager.GetStringField(arrResult[1]);
            VariousData<int> id = WebDBManager.GetIntField(arrResult[2].ToString());

            if (strDate == null || strTime == null || id == null)
                return;

            m_strLastDate = strDate;
            m_strLastTime = strTime;
            m_nLastID = id.Data;
        }*/

        private string ReadSecomDBName()
        {
            string strSQL = "Select PropertyValue from OptionSDMS where PropertyName = 'S1DBName' and SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return "";

            string strDBName = WebDBManager.GetStringField(arrResult[0]);

            if (strDBName == null)
                return "";

            return strDBName.Trim();
        }

        public void Run()
        {
            stopWatching = false;

            Thread t = new Thread(new ThreadStart(WatchAlarm));
            t.Start();
        }

        public void Stop()
        {
            stopWatching = true;
        }

        private void WatchAlarm()
        {
            while (stopWatching == false)
            {
                if (NetworkManager.IsReady)
                {
                    List<SecomEquipment> events = ReadEvents();

                    foreach (SecomEquipment newEvent in events)
                    {
                        int nData = newEvent.Status == SecomEquipment.AlarmStatus.FIRE || newEvent.Status == SecomEquipment.AlarmStatus.SECURITY ? 1 : 0;
                        NetworkManager.Instance.SendSensorData(newEvent.SensorZoneID, newEvent.SensorTagID, newEvent.SensorType, nData);
                        System.Diagnostics.Trace.WriteLine("NewEvent : " + newEvent.SensorTagID.ToString());
                    }
                }

                Thread.Sleep(1000);
            }
        }

        private List<SecomEquipment> ReadEvents()
        {
            List<SecomEquipment> events = new List<SecomEquipment>();

            string strFormat = "select ADate, Atime, ID, EqCode, Master, Local, Point, Loop, Content, EQname, State, BuildingID, FloorID, SectorID, PlaceName, PreState, CardNum from Secom_Alarm order by ADate, ATime";
            string strSQL = strFormat;//string.Format(strFormat, m_strLastDate, m_strLastTime);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0, m_strSecomDBName);

            if (arrResult == null)
                return events;

            int nResultCount = arrResult.Count;
            Dictionary<string, string> dicEquipStatus = new Dictionary<string, string>();

            for (int i = 0; i < nResultCount - 16; i += 17)
            {
                string strDate = WebDBManager.GetStringField(arrResult[i]);
                string strTime = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> eqCode = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<int> master = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                VariousData<int> local = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                VariousData<int> point = WebDBManager.GetIntField(arrResult[i + 6].ToString());
                VariousData<int> loop = WebDBManager.GetIntField(arrResult[i + 7].ToString());
                string strContent = WebDBManager.GetStringField(arrResult[i + 8].ToString());
                string strEqName = WebDBManager.GetStringField(arrResult[i + 9]);
                string strState = WebDBManager.GetStringField(arrResult[i + 10]);
                VariousData<int> buildingID = WebDBManager.GetIntField(arrResult[i + 11].ToString());
                VariousData<int> floorID = WebDBManager.GetIntField(arrResult[i + 12].ToString());
                VariousData<int> sectorID = WebDBManager.GetIntField(arrResult[i + 13].ToString());
                string strPlaceName = WebDBManager.GetStringField(arrResult[i + 14]);
                string strPreState = WebDBManager.GetStringField(arrResult[i + 15]);
                string strCardNum = WebDBManager.GetStringField(arrResult[i + 16]);

                if (strDate == null || strTime == null || strState == null || id == null ||
                    buildingID == null || floorID == null || sectorID == null || eqCode == null || master == null ||
                    local == null || point == null || loop == null || strContent == null)
                    continue;

                // 이미 읽었던 데이터인가?
                /*if (m_strLastDate == strDate && m_strLastTime == strTime && id.Data == m_nLastID)
                    continue;

                m_strLastDate = strDate;
                m_strLastTime = strTime;
                m_nLastID = id.Data;*/

                string strEquip = string.Format("{0}_{1}_{2}_{3}_{4}_{5}", 
                    eqCode.Data, master.Data, local.Data, point.Data, loop.Data, strContent);
                //string strEquip = string.Format("{0}_{1}_{2}_-{3}", buildingID.Data, floorID.Data, sectorID.Data, strEqName);
                dicEquipStatus[strEquip] = strState;

                // 한번 읽은 로그는 History Table로 옮긴후 지운다.
                MoveNDeleteLog(strDate, strTime, id.Data, eqCode.Data, master.Data, local.Data, point.Data, loop.Data, buildingID.Data, floorID.Data, sectorID.Data, strState, strContent, strEqName, strPlaceName, strPreState, strCardNum);
            }

            int nEqCode, nMaster, nLocal, nPoint, nLoop;

            foreach (KeyValuePair<string, string> pair in dicEquipStatus)
            {
                string[] tokens = pair.Key.Split('_');

                if (tokens.Count() < 6)
                    continue;

                if (int.TryParse(tokens[0].Trim(), out nEqCode) == false ||
                    int.TryParse(tokens[1].Trim(), out nMaster) == false ||
                    int.TryParse(tokens[2].Trim(), out nLocal) == false ||
                    int.TryParse(tokens[3].Trim(), out nPoint) == false ||
                    int.TryParse(tokens[4].Trim(), out nLoop) == false)
                    continue;

                string strContent = tokens[5].Trim();

                if (nLoop == 0 && pair.Value == "0000")
                {
                    // 대표코드로 한꺼번에 복구하는 경우
                    strSQL = "Select sl.SensorTagInfoID, sti.SensorType, sti.SensorZoneID, sl.PrevState, sl.SecomEqName, sl.Loop ";
                    strSQL += "FROM SecomEq_Sensor_Link as sl, SensorTagInfo as sti ";
                    strSQL += string.Format("where sl.SensorTagInfoID = sti.ID and sl.EqCode = {0} and sl.Master = {1} and sl.Local = {2} and sl.Point = {3} and sl.Content = '{4}'",
                        nEqCode, nMaster, nLocal, nPoint, strContent);

                    arrResult = m_dbMgr.GetResultData(strSQL, 0);

                    if (arrResult == null)
                        continue;

                    int nResultCount2 = arrResult.Count;

                    for (int j = 0; j < nResultCount2 - 5; j += 6)
                    {
                        VariousData<int> sensorTagID = WebDBManager.GetIntField(arrResult[j].ToString());
                        VariousData<int> sensorType = WebDBManager.GetIntField(arrResult[j + 1].ToString());
                        VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[j + 2].ToString());
                        string strPrevState = WebDBManager.GetStringField(arrResult[j + 3]);
                        string strEqName = WebDBManager.GetStringField(arrResult[j + 4], "");
                        VariousData<int> loop = WebDBManager.GetIntField(arrResult[j + 5].ToString());

                        if (sensorTagID == null || sensorType == null || sensorZoneID == null || loop == null)
                            continue;

                        string state = pair.Value;

                        if (state == strPrevState)
                            continue;

                        strFormat = "Update SecomEq_Sensor_Link set PrevState = '{0}' where EqCode = {1} and ";
                        strFormat += "Master = {2} and Local = {3} and Point = {4} and Loop = {5} and Content = '{6}' and SensorTagInfoID = {7}";
                        string strSQL2 = string.Format(strFormat, state, nEqCode, nMaster, nLocal, nPoint, loop.Data, strContent, sensorTagID.Data);

                        if (m_dbMgr.GetResultData(strSQL2, 0) == null)
                            continue;

                        SecomEquipment newEvent = new SecomEquipment();

                        newEvent.EqName = strEqName;
                        newEvent.SensorTagID = sensorTagID.Data;
                        newEvent.SensorZoneID = sensorZoneID.Data;
                        newEvent.SensorType = sensorType.Data;
                        newEvent.SetStatus(state);

                        events.Add(newEvent);
                    }
                }
                else
                {
                    strSQL = "Select sl.SensorTagInfoID, sti.SensorType, sti.SensorZoneID, sl.PrevState, sl.SecomEqName ";
                    strSQL += "FROM SecomEq_Sensor_Link as sl, SensorTagInfo as sti ";
                    strSQL += string.Format("where sl.SensorTagInfoID = sti.ID and sl.EqCode = {0} and sl.Master = {1} and sl.Local = {2} and sl.Point = {3} and sl.Loop = {4} and sl.Content = '{5}'",
                        nEqCode, nMaster, nLocal, nPoint, nLoop, strContent);

                    arrResult = m_dbMgr.GetResultData(strSQL, 0);

                    if (arrResult == null || arrResult.Count != 5)
                        continue;

                    VariousData<int> sensorTagID = WebDBManager.GetIntField(arrResult[0].ToString());
                    VariousData<int> sensorType = WebDBManager.GetIntField(arrResult[1].ToString());
                    VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[2].ToString());
                    string strPrevState = WebDBManager.GetStringField(arrResult[3]);
                    string strEqName = WebDBManager.GetStringField(arrResult[4], "");

                    if (sensorTagID == null || sensorType == null || sensorZoneID == null)
                        continue;

                    string state = pair.Value;

                    if (state == strPrevState)
                        continue;

                    strFormat = "Update SecomEq_Sensor_Link set PrevState = '{0}' where EqCode = {1} and ";
                    strFormat += "Master = {2} and Local = {3} and Point = {4} and Loop = {5} and Content = '{6}' and SensorTagInfoID = {7}";
                    string strSQL2 = string.Format(strFormat, state, nEqCode, nMaster, nLocal, nPoint, nLoop, strContent, sensorTagID.Data);

                    if (m_dbMgr.GetResultData(strSQL2, 0) == null)
                        continue;

                    SecomEquipment newEvent = new SecomEquipment();

                    newEvent.EqName = strEqName;
                    newEvent.SensorTagID = sensorTagID.Data;
                    newEvent.SensorZoneID = sensorZoneID.Data;
                    newEvent.SensorType = sensorType.Data;
                    newEvent.SetStatus(state);

                    events.Add(newEvent);
                }
            }
            /*int nBuildingID, nFloorID, nSectorID;

            foreach (KeyValuePair<string, string> pair in dicEquipStatus)
            {
                string[] tokens = pair.Key.Split('_');

                if (tokens.Count() != 4)
                    continue;

                if (int.TryParse(tokens[0].Trim(), out nBuildingID) == false ||
                    int.TryParse(tokens[1].Trim(), out nFloorID) == false ||
                    int.TryParse(tokens[2].Trim(), out nSectorID) == false)
                    continue;

                int nIndex = pair.Key.IndexOf('-');

                if (nIndex < 0)
                    continue;

                string strEqName = pair.Key.Substring(nIndex + 1);

                strSQL = "Select sl.SensorTagInfoID, sti.SensorType, sti.SensorZoneID, sl.PrevState ";
                strSQL += "FROM SecomEq_Sensor_Link as sl, SensorTagInfo as sti ";
                strSQL += string.Format("where sl.SensorTagInfoID = sti.ID and sl.SecomEqName = '{0}' and sl.BuildingID = {1} and si.FloorID = {2} and si.SectorID = {3}", strEqName, nBuildingID, nFloorID, nSectorID);

                arrResult = m_dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null || arrResult.Count != 4)
                    continue;

                VariousData<int> sensorTagID = WebDBManager.GetIntField(arrResult[0].ToString());
                VariousData<int> sensorType = WebDBManager.GetIntField(arrResult[1].ToString());
                VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[2].ToString());
                string strPrevState = WebDBManager.GetStringField(arrResult[3]);

                if (sensorTagID == null || sensorType == null || sensorZoneID == null || strPrevState == null)
                    continue;

                string state = pair.Value;

                if (state == strPrevState)
                    continue;

                strFormat = "Update SecomEq_EquipZone_Link set PrevState = '{0}' where SecomEqName = '{1}' and SensorTagInfoID = {2}";
                string strSQL2 = string.Format(strFormat, state, strEqName, sensorTagID.Data);

                if (m_dbMgr.GetResultData(strSQL2, 0) == null)
                    continue;

                SecomEquipment newEvent = new SecomEquipment();

                newEvent.EqName = strEqName;
                newEvent.SensorTagID = sensorTagID.Data;
                newEvent.SensorZoneID = sensorZoneID.Data;
                newEvent.SensorType = sensorType.Data;
                newEvent.SetStatus(state);

                events.Add(newEvent);
            }*/

            return events;
        }

        // 읽어들인 알람로그를 Secom_Alarm_History에 옮기고, Secom_Alarm에서는 지운다.
        private bool MoveNDeleteLog(string strDate, string strTime, int nID, int nEqCode, int nMaster, int nLocal, int nPoint, int nLoop, int nBuildingID, int nFloorID, int nSectorID, string strState, string strContent, string strEqName, string strPlaceName, string strPreState, string strCardNum)
        {
            DateTime dtNow = DateTime.Now;
            string strTimeStamp = string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}.{6:000}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second, dtNow.Millisecond);

            SetData(ref strEqName);
            SetData(ref strPlaceName);
            SetData(ref strPreState);
            SetData(ref strCardNum);

            string strFormat = "Insert into Secom_Alarm_History (TimeStamp, ADate, ATime, ID, EqCode, Master, Local, Point, Loop, EqName, BuildingID, FloorID, SectorID, PlaceName, State, Content, PreState, CardNum) values ";
            strFormat += "('{0}', '{1}', '{2}', {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, '{14}', '{15}', {16}, {17})";
            string strSQL = string.Format(strFormat, strTimeStamp, strDate, strTime, nID, nEqCode, nMaster, nLocal, nPoint, nLoop, strEqName,
                nBuildingID, nFloorID, nSectorID, strPlaceName, strState, strContent, strPreState, strCardNum);

            if (m_dbMgr.GetResultData(strSQL, 0, m_strSecomDBName) == null)
                return false;

            strFormat = "Delete from Secom_Alarm where ADate = '{0}' and ATime = '{1}' and ID = {2} and EqCode = {3} and Master = {4} and Local = {5} and ";
            strFormat += "Point = {6} and Loop = {7} and State = '{8}' and Content = '{9}'";
            strSQL = string.Format(strFormat, strDate, strTime, nID, nEqCode, nMaster, nLocal, nPoint, nLoop, strState, strContent);

            return m_dbMgr.GetResultData(strSQL, 0, m_strSecomDBName) != null;
        }

        private void SetData(ref string strData)
        {
            if (strData == null)
                strData = "NULL";
            else
                strData = "'" + strData + "'";
        }

        // 한달이 지난 로그는 삭제한다.
        public void CheckOldLog()
        {
            DateTime dtNow = DateTime.Now;

            if (dtNow.Year == m_nLastLogYear && dtNow.Month == m_nLastLogMonth && dtNow.Day == m_nLastLogDay)
                return;

            m_nLastLogYear = dtNow.Year;
            m_nLastLogMonth = dtNow.Month;
            m_nLastLogDay = dtNow.Day;

            string strTime = string.Format("{0}{1:00}{2:00}000000.000", dtNow.Year, dtNow.Month, dtNow.Day);

            string strSQL = "Delete from Secom_Alarm_History where TimeStamp < '" + strTime + "'";
            DataManager.Instance.DBManager.GetResultData(strSQL, 0, m_strSecomDBName);
        }

        /*private bool ReadLinkedEquipZone()
        {
            string strSQL = "Select SecomEqName, EquipZoneID from SecomEq_EquipZone_Link";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
            {
                throw new Exception("SecomEq_EquipZone_Link Table을 확인할 수 없습니다.");
            }

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                string strEqName = WebDBManager.GetStringField(arrResult[i]);
                VariousData<int> equipZoneID = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (strEqName == null || strEqName.Length == 0 || equipZoneID == null)
                    continue;

                m_dicSecomEqEquipZoneID[strEqName] = equipZoneID.Data;
            }

            return true;
        }*/
    }

    class SecomEquipment
    {
        public enum AlarmStatus { NONE, FIRE, SECURITY, UNKNOWN };

        private string m_strEqName = "";
        private int m_nSensorTagID = -1;
        private int m_nSensorZoneID = -1;
        private int m_nSensorType = -1;
        private AlarmStatus m_status = AlarmStatus.UNKNOWN;

        public string EqName
        {
            get { return m_strEqName; }
            set { m_strEqName = value; }
        }

        public int SensorTagID
        {
            get { return m_nSensorTagID; }
            set { m_nSensorTagID = value; }
        }

        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        public int SensorType
        {
            get { return m_nSensorType; }
            set { m_nSensorType = value; }
        }

        public AlarmStatus Status
        {
            get { return m_status; }
        }

        public void SetStatus(string strState)
        {
            if (strState == "0000")
                m_status = AlarmStatus.NONE;
            else if (strState == "2000")
                m_status = AlarmStatus.FIRE;
            else if (strState == "2100")
                m_status = AlarmStatus.SECURITY;
            else
                m_status = AlarmStatus.UNKNOWN;
        }
    }
}
