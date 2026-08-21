using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnE.Geometry;
using System.Collections;
using System.Data.SqlClient;

namespace SimulationScenarioMaker
{
    public class DataManager
    {
        public enum SensorType { WORKER = 0, VEHICLE, EQUIPMENT };

        private DBManager m_dbMgrHSMS = null;
        private DBManager m_dbMgrHPublic = null;
        private DBManager m_dbMgrHWinmm = null;

        private SqlConnection m_dbConnectionHSMS = null;
        private SqlConnection m_dbConnectionHPublic = null;
        private SqlConnection m_dbConnectionHWinmm = null;

        private Vertex2D m_vBoundaryBL = new Vertex2D();
        private Vertex2D m_vBoundaryTR = new Vertex2D();

        private ArrayList m_arrWorkers = new ArrayList();
        private ArrayList m_arrEquipments = new ArrayList();
        private ArrayList m_arrVehicles = new ArrayList();

        // 총 시간(초)
        private int m_nRunningTime = 0;
        private ArrayList m_arrEvents = new ArrayList();
        // 반복횟수 : 이 값이 0이면 실행하지 않는다.
        //            이 값이 0보다 작으면 무한 반복한다.
        private int m_nRepeatCount = 1;

        // Type별 SensorEvents List
        private Dictionary<SensorType, ArrayList> m_dicSensorEvents = new Dictionary<SensorType, ArrayList>();

        private ArrayList m_workerNullEvents = new ArrayList();
        private ArrayList m_carNullEvents = new ArrayList();
        private ArrayList m_equipNullEvents = new ArrayList();

        // 총 시간(초)
        public int RunningTime
        {
            get { return m_nRunningTime; }
            set { m_nRunningTime = value; }
        }

        public ArrayList Events
        {
            get { return m_arrEvents; }
        }

        // 반복횟수 : 이 값이 0이면 실행하지 않는다.
        //            이 값이 0보다 작으면 무한 반복한다.
        public int RepeatCount
        {
            get { return m_nRepeatCount; }
            set { m_nRepeatCount = value; }
        }

        public ArrayList GetSensorEvents(SensorType type)
        {
            ArrayList sensorEvents = null;

            if (m_dicSensorEvents.ContainsKey(type))
            {
                sensorEvents = m_dicSensorEvents[type];
            }
            else
            {
                sensorEvents = new ArrayList();
                m_dicSensorEvents[type] = sensorEvents;
            }

            if (type == SensorType.WORKER)
                AddNullEvents(sensorEvents, m_workerNullEvents);
            else if (type == SensorType.VEHICLE)
                AddNullEvents(sensorEvents, m_carNullEvents);
            else if (type == SensorType.EQUIPMENT)
                AddNullEvents(sensorEvents, m_equipNullEvents);

            return sensorEvents;
        }

        private void AddNullEvents(ArrayList arrTargetEvents, ArrayList arrSourceEvents)
        {
            foreach (SensorEvents events in arrSourceEvents)
            {
                SensorEvents findEvents = FindSensorEvents(arrTargetEvents, events.SensorID);

                if (findEvents == null)
                    arrTargetEvents.Add(events);
            }
        }

        private ArrayList CloneList(ArrayList arrOrigin)
        {
            ArrayList arrClone = new ArrayList();

            foreach (object obj in arrOrigin)
            {
                arrClone.Add(obj);
            }

            return arrClone;
        }

        // 이벤트 순서대로 정렬시켜 계산
        public void CalcByEvent()
        {
            m_arrEvents.Clear();

            // EventTime, SensorData List
            Dictionary<int, ArrayList> dicEventSensors = new Dictionary<int,ArrayList>();

            foreach (KeyValuePair<SensorType, ArrayList> pair in m_dicSensorEvents)
            {
                foreach (SensorEvents events in pair.Value)
                {
                    foreach (SensorEvents.SensorEvent sensorEvent in events.Events)
                    {
                        ArrayList arrSensorDatas = null;
                        
                        if (dicEventSensors.ContainsKey(sensorEvent.EventTime))
                            arrSensorDatas = dicEventSensors[sensorEvent.EventTime];
                        else
                        {
                            arrSensorDatas = new ArrayList();
                            dicEventSensors[sensorEvent.EventTime] = arrSensorDatas;
                        }

                        SensorData sensorData = new SensorData();

                        sensorData.SensorID = events.SensorID;
                        sensorData.X = sensorEvent.X;
                        sensorData.Y = sensorEvent.Y;

                        arrSensorDatas.Add(sensorData);
                    }
                }
            }

            foreach (KeyValuePair<int, ArrayList> pair in dicEventSensors)
            {
                EventData data = new EventData();

                data.EventTime = pair.Key;
                data.Sensors = pair.Value;

                m_arrEvents.Add(data);
            }

            m_arrEvents.Sort();
        }

        // 센서 위주로 정렬시켜 계산
        public void CalcBySensor()
        {
            ArrayList arrWorkers = CloneList(m_arrWorkers);
            ArrayList arrVehicles = CloneList(m_arrVehicles);
            ArrayList arrEquipments = CloneList(m_arrEquipments);

            m_dicSensorEvents.Clear();

            SensorType type = SensorType.WORKER;

            #region XML에서 읽은 Event Data
            foreach (EventData data in m_arrEvents)
            {
                foreach (SensorData sensor in data.Sensors)
                {
                    HSMS.DataWorker worker = FindWorker(sensor.SensorID);

                    if (worker != null)
                    {
                        type = SensorType.WORKER;
                        arrWorkers.Remove(worker);
                    }
                    else
                    {
                        HSMS.DataCar car = FindVehicle(sensor.SensorID);

                        if (car != null)
                        {
                            type = SensorType.VEHICLE;
                            arrVehicles.Remove(car);
                        }
                        else
                        {
                            HSMS.DataEquip equip = FindEquipment(sensor.SensorID);

                            if (equip != null)
                            {
                                type = SensorType.EQUIPMENT;
                                arrEquipments.Remove(equip);
                            }
                            else
                                continue;
                        }
                    }

                    ArrayList arrSensorEvents = GetSensorEvents(type);

                    if (arrSensorEvents == null)
                    {
                        arrSensorEvents = new ArrayList();
                        m_dicSensorEvents[type] = arrSensorEvents;
                    }

                    SensorEvents events = FindSensorEvents(arrSensorEvents, sensor.SensorID);

                    if (events == null)
                    {
                        events = new SensorEvents();
                        events.SensorID = sensor.SensorID;
                        events.SensorType = type;

                        arrSensorEvents.Add(events);
                    }

                    SensorEvents.SensorEvent sensorEvent = new SensorEvents.SensorEvent();

                    sensorEvent.EventTime = data.EventTime;
                    sensorEvent.X = sensor.X;
                    sensorEvent.Y = sensor.Y;

                    events.Events.Add(sensorEvent);
                }
            }
            #endregion

            foreach (KeyValuePair<SensorType, ArrayList> pair in m_dicSensorEvents)
            {
                pair.Value.Sort();
            }

            #region DB에서 읽은 Sensor 정보를 이용하여 빈 데이터를 만들어 놓는다.
            AddSensorEvents(arrWorkers, SensorType.WORKER);
            AddSensorEvents(arrVehicles, SensorType.VEHICLE);
            AddSensorEvents(arrEquipments, SensorType.EQUIPMENT);
            #endregion
        }

        private void AddSensorEvents(ArrayList arrSensors, SensorType type)
        {
            ArrayList arrSensorEvents = null;

            if (!m_dicSensorEvents.ContainsKey(type))
            {
                arrSensorEvents = new ArrayList();
                m_dicSensorEvents[type] = arrSensorEvents;
            }
            else
                arrSensorEvents = m_dicSensorEvents[type];

            foreach (object obj in arrSensors)
            {
                SensorEvents events = new SensorEvents();

                if (type == SensorType.WORKER)
                {
                    HSMS.DataWorker worker = (HSMS.DataWorker)obj;
                    events.SensorID = worker.Sensor;
                }
                else if (type == SensorType.VEHICLE)
                {
                    HSMS.DataCar car = (HSMS.DataCar)obj;
                    events.SensorID = car.Sensor;
                }
                else if (type == SensorType.EQUIPMENT)
                {
                    HSMS.DataEquip equip = (HSMS.DataEquip)obj;
                    events.SensorID = equip.Sensor;
                }
                else
                    continue;

                events.SensorType = type;

                if (events.SensorID.Length > 0)
                    arrSensorEvents.Add(events);
            }
        }

        private SensorEvents FindSensorEvents(ArrayList arrSensorEvents, string strSensorID)
        {
            foreach (SensorEvents events in arrSensorEvents)
            {
                if (events.SensorID == strSensorID)
                    return events;
            }

            return null;
        }

        public DataManager()
        {
            m_dbMgrHSMS = new DBManager("HSMS");
            m_dbMgrHPublic = new DBManager("hpublic00");
            m_dbMgrHWinmm = new DBManager("hwinmm");

            m_dbConnectionHSMS = m_dbMgrHSMS.Connect();
            m_dbConnectionHPublic = m_dbMgrHPublic.Connect();
            m_dbConnectionHWinmm = m_dbMgrHWinmm.Connect();

            ReadBoundary();
            ReadWorkers();
            ReadVehicles();
            ReadEquipments();

            m_dbConnectionHSMS.Close();
            m_dbConnectionHPublic.Close();
            m_dbConnectionHWinmm.Close();
        }

        private void ReadBoundary()
        {
            string strSQL = "Select Boundary from Zone where ZoneName = 'PLAN' and SiteID = 1";
            SqlDataReader reader = m_dbMgrHSMS.ExecuteReader(strSQL, m_dbConnectionHSMS);

            while (reader.Read())
            {
                string strBoundary = (string)reader[0];
                ReadBoundary(strBoundary);
            }

            reader.Close();
        }

        private bool ReadBoundary(string strBoundary)
        {
            string[] arrCoord = strBoundary.Split(',');

            bool isFirst = true;
            bool xTime = true;
            double x = 0.0, coord;

            foreach (string strCoord in arrCoord)
            {
                string str = strCoord.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
                str = str.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });

                if (!double.TryParse(str, out coord))
                    return false;

                if (xTime)
                    x = coord;
                else
                {
                    if (isFirst)
                    {
                        m_vBoundaryBL.x = x;
                        m_vBoundaryBL.y = coord;
                        m_vBoundaryTR.x = x;
                        m_vBoundaryTR.y = coord;

                        isFirst = false;
                    }
                    else
                    {
                        if (m_vBoundaryBL.x > x)
                            m_vBoundaryBL.x = x;
                        if (m_vBoundaryBL.y > coord)
                            m_vBoundaryBL.y = coord;
                        if (m_vBoundaryTR.x < x)
                            m_vBoundaryTR.x = x;
                        if (m_vBoundaryTR.y < coord)
                            m_vBoundaryTR.y = coord;
                    }
                }

                xTime = !xTime;
            }

            return xTime && !isFirst;
        }

        private void TrimString(ref string str)
        {
            str = str.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
            str = str.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });
        }

        private bool ReadEquipments()
        {
            string strEquipCode = GetFieldLink("EquipCode");
            string strEquipSensor = GetFieldLink("EquipSensor");

            if (strEquipCode == null || strEquipSensor == null)
                return false;

            string[] arrField = strEquipCode.Split('.');

            if (arrField.Count() != 3)
                return false;

            string strEquipCodeFieldName = arrField[2];

            string strSQL = "Select id, EquipCode, MeshName from Equipment";
            SqlDataReader reader = m_dbMgrHSMS.ExecuteReader(strSQL, m_dbConnectionHSMS);

            while (reader.Read())
            {
                int nID = (int)reader[0];
                string strEquipCode2 = (string)reader[1];
                string strMeshName = (string)reader[2];

                TrimString(ref strEquipCode2);
                TrimString(ref strMeshName);

                string strSensorID = GetLinkData(strEquipCodeFieldName, strEquipCode2, strEquipSensor);

                if (strSensorID == null)
                    continue;

                TrimString(ref strSensorID);

                HSMS.DataEquip equip = new HSMS.DataEquip();

                equip.ID = nID;
                equip.Code = strEquipCode2;
                equip.Name = strMeshName;
                equip.Sensor = strSensorID;

                m_arrEquipments.Add(equip);

                SensorEvents events = new SensorEvents();
                events.SensorID = strSensorID;
                events.SensorType = SensorType.EQUIPMENT;

                m_equipNullEvents.Add(events);
            }

            reader.Close();
            return true;
        }

        private bool ReadVehicles()
        {
            string strCarCode = GetFieldLink("CarCode");
            string strCarSensor = GetFieldLink("CarSensor");

            if (strCarCode == null || strCarSensor == null)
                return false;

            string[] arrField = strCarCode.Split('.');

            if (arrField.Count() != 3)
                return false;

            string strCarCodeFieldName = arrField[2];

            string strSQL = "Select id, CarNumber from Car";
            SqlDataReader reader = m_dbMgrHSMS.ExecuteReader(strSQL, m_dbConnectionHSMS);

            while (reader.Read())
            {
                int nID = (int)reader[0];
                string strCarCode2 = (string)reader[1];

                TrimString(ref strCarCode2);

                string strSensorID = GetLinkData(strCarCodeFieldName, strCarCode2, strCarSensor);

                if (strSensorID == null)
                    continue;

                TrimString(ref strSensorID);

                TrimString(ref strCarCode2);

                HSMS.DataCar car = new HSMS.DataCar();

                car.Number = strCarCode2;
                car.Sensor = strSensorID;

                m_arrVehicles.Add(car);

                SensorEvents events = new SensorEvents();
                events.SensorID = strSensorID;
                events.SensorType = SensorType.VEHICLE;

                m_carNullEvents.Add(events);
            }

            reader.Close();
            return true;
        }

        private bool ReadWorkers()
        {
            string strWorkerSensorLink = GetFieldLink("WorkerSensor");
            string strWorkerMemberID = GetFieldLink("WorkerCompanyID");

            if (strWorkerMemberID == null || strWorkerSensorLink == null)
                return false;

            string[] arrField = strWorkerMemberID.Split('.');

            if (arrField.Count() != 3)
                return false;

            string strMemberIDFieldName = arrField[2];

            string strSQL = "Select id, MemberID, WorkerLevel from Worker";
            SqlDataReader reader = m_dbMgrHSMS.ExecuteReader(strSQL, m_dbConnectionHSMS);

            while (reader.Read())
            {
                int nID = (int)reader[0];
                string strMemberID = (string)reader[1];
                int nWorkerLevel = (int)reader[2];

                TrimString(ref strMemberID);

                string strSensorID = GetLinkData(strMemberIDFieldName, strMemberID, strWorkerSensorLink);

                if (strSensorID == null)
                    continue;

                TrimString(ref strSensorID);

                HSMS.DataWorker worker = new HSMS.DataWorker();

                worker.ID = nID;
                worker.MemberID = strMemberID;
                worker.EnterLevel = nWorkerLevel;
                worker.Sensor = strSensorID;

                m_arrWorkers.Add(worker);

                SensorEvents events = new SensorEvents();
                events.SensorID = strSensorID;
                events.SensorType = SensorType.WORKER;

                m_workerNullEvents.Add(events);
            }

            reader.Close();
            return true;
        }

        private string GetFieldLink(string strItemName)
        {
            string strSQL = "Select ItemValue from FieldLink where ItemName = '" + strItemName + "'";
            SqlDataReader reader = m_dbMgrHSMS.ExecuteReader(strSQL, m_dbConnectionHSMS);

            string strValue = null;

            while (reader.Read())
            {
                if (reader.IsDBNull(0))
                    strValue = "";
                else
                    strValue = (string)reader[0];
            }

            reader.Close();
            return strValue;
        }

        private string GetLinkData(string strKeyFieldName, string strKey, string strField)
        {
            string[] arrField = strField.Split('.');

            if (arrField.Count() != 3)
                return null;

            DBManager dbMgr = null;
            SqlConnection dbConnection = null;

            if (string.Compare(arrField[0], "hpublic00", true) == 0)
            {
                dbMgr = m_dbMgrHPublic;
                dbConnection = m_dbConnectionHPublic;
            }
            else if (string.Compare(arrField[0], "HSMS", true) == 0)
            {
                dbMgr = m_dbMgrHSMS;
                dbConnection = m_dbConnectionHSMS;
            }
            else if (string.Compare(arrField[0], "hwinmm", true) == 0)
            {
                dbMgr = m_dbMgrHWinmm;
                dbConnection = m_dbConnectionHWinmm;
            }
            else
                return null;

            string strSQL = string.Format("Select {0} from {1} where {2} = '{3}'",
                arrField[2], arrField[1], strKeyFieldName, strKey);

            string strValue = null;
            SqlDataReader reader = dbMgr.ExecuteReader(strSQL, dbConnection);

            while (reader.Read())
            {
                if (reader.IsDBNull(0))
                    strValue = "";
                else
                    strValue = (string)reader[0];
            }

            reader.Close();
            return strValue;
        }

        private HSMS.DataWorker FindWorker(string strSensorID)
        {
            foreach (HSMS.DataWorker worker in m_arrWorkers)
            {
                if (worker.Sensor == strSensorID)
                    return worker;
            }

            return null;
        }

        private HSMS.DataCar FindVehicle(string strSensorID)
        {
            foreach (HSMS.DataCar car in m_arrVehicles)
            {
                if (car.Sensor == strSensorID)
                    return car;
            }

            return null;
        }

        private HSMS.DataEquip FindEquipment(string strSensorID)
        {
            foreach (HSMS.DataEquip equip in m_arrEquipments)
            {
                if (equip.Sensor == strSensorID)
                    return equip;
            }

            return null;
        }
    }

    public class EventData : IComparable
    {
        // (초)
        private int m_nEventTime = 0;
        private ArrayList m_arrSensors = new ArrayList();

        public int EventTime
        {
            get { return m_nEventTime; }
            set { m_nEventTime = value; }
        }

        public ArrayList Sensors
        {
            get { return m_arrSensors; }
            set { m_arrSensors = value; }
        }

        public int CompareTo(object obj)
        {
            EventData data = (EventData)obj;

            if (this.m_nEventTime > data.m_nEventTime)
                return 1;
            else if (this.m_nEventTime < data.m_nEventTime)
                return -1;
            //else
            return 0;
        }
    }

    public class SensorData
    {
        private double x = 0.0, y = 0.0;
        private string m_strSensorID = "";

        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        public string SensorID
        {
            get { return m_strSensorID; }
            set { m_strSensorID = value; }
        }
    }

    public class SensorEvents : Object, IComparable
    {
        public class SensorEvent : IComparable
        {
            // 초
            private int m_nEventTime = 0;
            private double x = 0.0;
            private double y = 0.0;

            // 초
            public int EventTime
            {
                get { return m_nEventTime; }
                set { m_nEventTime = value; }
            }

            public double X
            {
                get { return x; }
                set { x = value; }
            }

            public double Y
            {
                get { return y; }
                set { y = value; }
            }

            public int CompareTo(object obj)
            {
                SensorEvent sEvent = (SensorEvent)obj;

                if (this.m_nEventTime > sEvent.m_nEventTime)
                    return 1;
                else if (this.m_nEventTime < sEvent.m_nEventTime)
                    return -1;
                //else
                return 0;
            }
        }

        private DataManager.SensorType m_type = DataManager.SensorType.WORKER;
        private string m_strSensorID = "";
        private ArrayList m_arrEvents = new ArrayList();

        public DataManager.SensorType SensorType
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public string SensorID
        {
            get { return m_strSensorID; }
            set { m_strSensorID = value; }
        }

        public ArrayList Events
        {
            get { return m_arrEvents; }
        }

        public override string ToString()
        {
            return m_strSensorID;
        }

        public int CompareTo(object obj)
        {
            SensorEvents events = (SensorEvents)obj;
            return string.Compare(m_strSensorID, events.m_strSensorID);
        }
    }
}
