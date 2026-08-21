using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnE.Geometry;
using System.Data.SqlClient;
using System.Collections;

namespace HSMSServer
{
    public class DataManager
    {
        public enum SensorType { WORKER = 0, VEHICLE, EQUIPMENT };

        private HSMS.DBConn m_dbMgrHSMS = null;
        private HSMS.DBConn m_dbMgrHPublic = null;
        private HSMS.DBConn m_dbMgrHWinmm = null;

        private SqlConnection m_dbConnectionHSMS = null;
        private SqlConnection m_dbConnectionHPublic = null;
        private SqlConnection m_dbConnectionHWinmm = null;
        
        private Vertex2D m_vBoundaryBL = new Vertex2D();
        private Vertex2D m_vBoundaryTR = new Vertex2D();

        private ArrayList m_arrWorkers = new ArrayList();
        private ArrayList m_arrEquipments = new ArrayList();
        private ArrayList m_arrVehicles = new ArrayList();

        private int m_nRunningTime = 0;
        private int m_nRepeatCount = 1;
        private ArrayList m_arrEvents = new ArrayList();

        public int RunningTime
        {
            get { return m_nRunningTime; }
            set { m_nRunningTime = value; }
        }

        public int RepeatCount
        {
            get { return m_nRepeatCount; }
            set { m_nRepeatCount = value; }
        }

        public ArrayList Events
        {
            get { return m_arrEvents; }
        }

        public DataManager()
        {
            m_dbMgrHSMS = new HSMS.DBConn("HSMS");
            m_dbMgrHPublic = new HSMS.DBConn("hpublic00");
            m_dbMgrHWinmm = new HSMS.DBConn("hwinmm");

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

                DataEquipment equip = new DataEquipment();

                equip.EquipCode = strEquipCode2;
                equip.MeshName = strMeshName;
                equip.Sensor = strSensorID;

                m_arrEquipments.Add(equip);
            }

            reader.Close();
            return true;
        }

        private bool ReadVehicles()
        {
            string strCarNumber = GetFieldLink("CarNumber");
            string strCarSensor = GetFieldLink("CarSensor");

            if (strCarNumber == null || strCarSensor == null)
                return false;

            string[] arrField = strCarNumber.Split('.');

            if (arrField.Count() != 3)
                return false;

            string strCarNumberFieldName = arrField[2];

            string strSQL = "Select id, CarNumber from Car";
            SqlDataReader reader = m_dbMgrHSMS.ExecuteReader(strSQL, m_dbConnectionHSMS);

            while (reader.Read())
            {
                int nID = (int)reader[0];
                string strCarNumber2 = (string)reader[1];

                TrimString(ref strCarNumber2);

                string strSensorID = GetLinkData(strCarNumberFieldName, strCarNumber2, strCarSensor);

                if (strSensorID == null)
                    continue;

                TrimString(ref strSensorID);

                TrimString(ref strCarNumber2);

                HSMS.DataCar car = new HSMS.DataCar();

                car.Number = strCarNumber2;
                car.Sensor = strSensorID;

                m_arrVehicles.Add(car);
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

            HSMS.DBConn dbMgr = null;
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

        public LocInfo GetRandomLocInfo(SensorType type)
        {
            string strSensorID = "";

            if (type == SensorType.WORKER)
            {
                HSMS.DataWorker worker = (HSMS.DataWorker)GetRandomSensorOwner(m_arrWorkers);

                if (worker == null)
                    return null;

                strSensorID = worker.Sensor;
            }
            else if (type == SensorType.VEHICLE)
            {
                HSMS.DataCar car = (HSMS.DataCar)GetRandomSensorOwner(m_arrVehicles);

                if (car == null)
                    return null;

                strSensorID = car.Sensor;
            }
            else if (type == SensorType.EQUIPMENT)
            {
                DataEquipment equip = (DataEquipment)GetRandomSensorOwner(m_arrEquipments);

                if (equip == null)
                    return null;

                strSensorID = equip.Sensor;
            }

            if (strSensorID == "")
                return null;

            double x, y;
            GetRandomCoord(out x, out y);

            LocInfo loc = new LocInfo();

            loc.DeviceID = strSensorID;
            loc.X = x;
            loc.Y = y;

            return loc;
        }

        private void GetRandomCoord(out double x, out double y)
        {
            double width = m_vBoundaryTR.x - m_vBoundaryBL.x;
            double height = m_vBoundaryTR.y - m_vBoundaryBL.y;

            DateTime dtNow = DateTime.Now;

            Random rand = new Random(dtNow.Minute * 60 * 1000 + dtNow.Second * 1000 + dtNow.Millisecond);
            int nIndexX = rand.Next(101);
            int nIndexY = rand.Next(101);

            x = m_vBoundaryBL.x + width / 100 * nIndexX;
            y = m_vBoundaryBL.y + height / 100 * nIndexY;
        }

        private object GetRandomSensorOwner(ArrayList arrOwners)
        {
            int nCount = arrOwners.Count;

            if (nCount == 0)
                return null;

            if (nCount == 1)
                return arrOwners[0];

            DateTime dtNow = DateTime.Now;

            Random rand = new Random(dtNow.Minute * 60 * 1000 + dtNow.Second * 1000 + dtNow.Millisecond);
            int nIndex = rand.Next(nCount);

            return arrOwners[nIndex];
        }

        public bool ProcessSysAlarm(string strSensorID, string strAlarmCode)
        {
            if (strSensorID == "ALL")
                return true;

            HSMS.DataWorker worker = FindWorker(strSensorID);

            if (worker != null)
            {
                return true;
            }

            HSMS.DataCar car = FindVehicle(strSensorID);

            if (car != null)
            {
                return true;
            }

            DataEquipment equip = FindEquipment(strSensorID);

            if (equip != null)
            {
                return true;
            }

            return false;
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

        private DataEquipment FindEquipment(string strSensorID)
        {
            foreach (DataEquipment equip in m_arrEquipments)
            {
                if (equip.Sensor == strSensorID)
                    return equip;
            }

            return null;
        }
    }

    public class DataEquipment
    {
        private string m_strEquipCode = "";
        private string m_strMeshName = "";
        private string m_strSensorID = "";

        public string EquipCode
        {
            get { return m_strEquipCode; }
            set { m_strEquipCode = value; }
        }

        public string MeshName
        {
            get { return m_strMeshName; }
            set { m_strMeshName = value; }
        }

        public string Sensor
        {
            get { return m_strSensorID; }
            set { m_strSensorID = value; }
        }
    }

    public class LocInfo
    {
        private string m_strDeviceID = "";
        private double x = 0.0;
        private double y = 0.0;
        private double m_dLatitude = 0.0;
        private double m_dLongitude = 0.0;
        private double m_dMethanGas = 0.0;
        private double m_dCoGas = 0.0;

        public string DeviceID
        {
            get { return m_strDeviceID; }
            set { m_strDeviceID = value; }
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

        public double Latitude
        {
            get { return m_dLatitude; }
            set { m_dLatitude = value; }
        }

        public double Longitude
        {
            get { return m_dLongitude; }
            set { m_dLongitude = value; }
        }

        public double MethanGas
        {
            get { return m_dMethanGas; }
            set { m_dMethanGas = value; }
        }

        public double CoGas
        {
            get { return m_dCoGas; }
            set { m_dCoGas = value; }
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
}
