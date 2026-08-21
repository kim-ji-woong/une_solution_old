using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections;
using HSMS;

namespace HSMSServer2
{
    public class DataManager
    {
        private DBConn m_DBConnection = null;

        /*private ArrayList m_arDataZones = new ArrayList();
        public ArrayList DataZones
        {
            get { return m_arDataZones; }
            set { m_arDataZones = value; }
        }*/
        private ArrayList m_arrZoneGroup = new ArrayList();
        private ArrayList m_arrEquipGroup = new ArrayList();

        // SensorID, 작업자
        private Dictionary<string, DataWorker> m_dicSensorWorkers = new Dictionary<string, DataWorker>();
        //사번, 작업자데이터
        private Dictionary<string, DataWorker> m_dicWorkers = new Dictionary<string, DataWorker>();

        //DB데이터를 임시로 저장, 저장완료시 이 배열의 데이터를 DB데이터에 덮어 씌움/ 저장 취소시 DB데이터를 여기에 저장
        private ArrayList m_arrTempWorkers = new ArrayList();
        public ArrayList TempWorkers
        {
            get { return m_arrTempWorkers; }
            set { m_arrTempWorkers = value; }
        }

        private ArrayList m_arrDeleteWorkers = new ArrayList();
        public ArrayList DeleteWorkers
        {
            get { return m_arrDeleteWorkers; }
            set { m_arrDeleteWorkers = value; }
        }

        private Dictionary<int, GasSensor> m_dicGasSensor = new Dictionary<int, GasSensor>();

        //SensorID, 차량데이터
        private Dictionary<string, DataCar> m_dicSensorCars = new Dictionary<string, DataCar>();
        //장비코드, 차량데이터
        private Dictionary<string, DataCar> m_dicCars = new Dictionary<string, DataCar>();

        //SensorID, 설비데이터
        private Dictionary<string, DataEquip> m_dicSensorEquips = new Dictionary<string, DataEquip>();
        //설비코드, 설비데이터
        private Dictionary<string, DataEquip> m_dicEquips = new Dictionary<string, DataEquip>();

        private Dictionary<string, EquipmentRawData> m_dicEquipRawDatas = new Dictionary<string, EquipmentRawData>();
        public Dictionary<string, EquipmentRawData> DicEquipRawDatas
        {
            get { return m_dicEquipRawDatas; }
            set { m_dicEquipRawDatas = value; }
        }

        private ArrayList m_arManagers = new ArrayList();
        public System.Collections.ArrayList Managers
        {
            get { return m_arManagers; }
        }

        private ArrayList m_arrDetectIgnoreWorkers = null;
        public ArrayList DetectIgnoreWorkers
        {
            get { return m_arrDetectIgnoreWorkers; }
        }

        // 작업자와 차량이 상호 마주보고 가까워지는 경우 안전거리(m)
        private float m_fWorkerToCarDistanceBoth = 5.0f;
        // 작업자와 차량중 한쪽에서 다가서는 경우 안전거리(m)
        private float m_fWorkerToCarDistanceOneSide = 3.0f;
        // 작업자와 위험영역간 안전거리(m)
        //private float m_fWorkerToZoneDistance = 2.0f;
        // 작업자와 위험설비간 안전거리(m)
        //private float m_fWorkerToEquipDistance = 2.0f;
        // 일산화탄소의 위험 농도(ppm)
        private float m_fCOGasTolerance = 400.0f;
        // 메탄가스의 위험 농도(ppm)
        private float m_fMethaneTolerance = 5000.0f;

        // 작업자와 차량이 상호 마주보고 가까워지는 경우 안전거리(m)
        public float WorkerToCarDistanceBoth
        {
            get { return m_fWorkerToCarDistanceBoth; }
            set { m_fWorkerToCarDistanceBoth = value; }
        }

        // 작업자와 차량중 한쪽에서 다가서는 경우 안전거리(m)
        public float WorkerToCarDistanceOneSide
        {
            get { return m_fWorkerToCarDistanceOneSide; }
            set { m_fWorkerToCarDistanceOneSide = value; }
        }

        // 일산화탄소의 위험 농도(ppm)
        public float COGasTolerance
        {
            get { return m_fCOGasTolerance; }
            set { m_fCOGasTolerance = value; }
        }

        // 메탄가스의 위험 농도(ppm)
        public float MethaneTolerance
        {
            get { return m_fMethaneTolerance; }
            set { m_fMethaneTolerance = value; }
        }

        // 작업자와 위험영역간 안전거리(m)
        // Key : ZoneGroup Name
        private Dictionary<string, float> m_dicWorkerToZoneDistance = new Dictionary<string, float>();
        // 작업자와 위험설비간 안전거리(m)
        // Key : EquipGroup Name
        private Dictionary<string, float> m_dicWorkerToEquipDistance = new Dictionary<string, float>();

        // 작업자와 위험영역간 안전거리(m)
        /*public float WorkerToZoneDistance
        {
            get { return m_fWorkerToZoneDistance; }
            set { m_fWorkerToZoneDistance = value; }
        }*/

        // 작업자와 위험설비간 안전거리(m)
        /*public float WorkerToEquipDistance
        {
            get { return m_fWorkerToEquipDistance; }
            set { m_fWorkerToEquipDistance = value; }
        }*/

        // 위험상황시 SMS전송 여부
        private bool m_bMessageChecked;
        public bool MessageChecked
        {
            get { return m_bMessageChecked; }
            set { m_bMessageChecked = value; }
        }

        public DataManager(DBConn dbMgr)
        {
            m_DBConnection = dbMgr;
            //ReadDBData();
        }

        public void SetWorkerToZoneDistance(string strZoneGroupName, float fDistance)
        {
            if (strZoneGroupName == ZoneGroup.DefaultZoneGroup.ToString())
                strZoneGroupName = ZoneGroup.DefaultZoneGroup.GroupName;

            m_dicWorkerToZoneDistance[strZoneGroupName] = fDistance;
        }

        public bool GetWorkerToZoneDistance(string strZoneGroupName, out float fDistance)
        {
            if (strZoneGroupName == ZoneGroup.DefaultZoneGroup.ToString())
                strZoneGroupName = ZoneGroup.DefaultZoneGroup.GroupName;

            fDistance = 0.0f;

            if (m_dicWorkerToZoneDistance.ContainsKey(strZoneGroupName))
            {
                fDistance = m_dicWorkerToZoneDistance[strZoneGroupName];
                return true;
            }

            return false;
        }

        public void SetWorkerToEquipDistance(string strEquipGroupName, float fDistance)
        {
            if (strEquipGroupName == EquipmentGroup.DefaultEquipmentGroup.ToString())
                strEquipGroupName = EquipmentGroup.DefaultEquipmentGroup.GroupName;

            m_dicWorkerToEquipDistance[strEquipGroupName] = fDistance;
        }

        public bool GetWorkerToEquipDistance(string strEquipGroupName, out float fDistance)
        {
            if (strEquipGroupName == EquipmentGroup.DefaultEquipmentGroup.ToString())
                strEquipGroupName = EquipmentGroup.DefaultEquipmentGroup.GroupName;

            fDistance = 0.0f;

            if (m_dicWorkerToEquipDistance.ContainsKey(strEquipGroupName))
            {
                fDistance = m_dicWorkerToEquipDistance[strEquipGroupName];
                return true;
            }

            return false;
        }

        #region DataWorker 접근 함수들
        public void AddWorker(DataWorker worker)
        {
            m_dicWorkers[worker.MemberID] = worker;
            m_dicSensorWorkers[worker.Sensor] = worker;
        }

        // 사번으로 찾는다.
        public DataWorker FindWorker(string strID)
        {
            if (m_dicWorkers.ContainsKey(strID))
                return m_dicWorkers[strID];

            return null;
        }

        // SensorID로 찾는다.
        public DataWorker FindWorker2(string strSensorID)
        {
            if (m_dicSensorWorkers.ContainsKey(strSensorID))
                return m_dicSensorWorkers[strSensorID];

            return null;
        }

        // 사번으로 찾아서 삭제한다.
        public void RemoveWorker(string strID)
        {
            if (m_dicWorkers.ContainsKey(strID))
            {
                DataWorker worker = m_dicWorkers[strID];

                m_dicWorkers.Remove(strID);
                m_dicSensorWorkers.Remove(worker.Sensor);
            }
        }

        // SensorID로 찾아서 삭제한다.
        public void RemoveWorker2(string strSensorID)
        {
            if (m_dicSensorWorkers.ContainsKey(strSensorID))
            {
                DataWorker worker = m_dicSensorWorkers[strSensorID];

                m_dicWorkers.Remove(worker.MemberID);
                m_dicSensorWorkers.Remove(strSensorID);
            }
        }

        public void RemoveWorker(DataWorker worker)
        {
            if (m_dicWorkers.ContainsKey(worker.MemberID))
            {
                m_dicWorkers.Remove(worker.MemberID);
                m_dicSensorWorkers.Remove(worker.Sensor);
            }
        }

        public void ClearWorkers()
        {
            m_dicWorkers.Clear();
            m_dicSensorWorkers.Clear();
        }

        public int GetWorkerCount()
        {
            return m_dicWorkers.Count;
        }

        public DataWorker GetWorkerFromID(int nID)
        {
            foreach (KeyValuePair<string, DataWorker> w in m_dicWorkers)
            {
                if (w.Value.ID == nID)
                    return w.Value;
            }
            return null;
        }


        public DataWorker GetWorker(int nIndex)
        {
            if (nIndex >= GetWorkerCount())
                return null;

            KeyValuePair<string, DataWorker> pair = m_dicWorkers.ElementAt(nIndex);
            return pair.Value;
        }

        public ArrayList GetWorkers()
        {
            ArrayList arr = new ArrayList();
            //foreach (KeyValuePair<string, DataWorker> w in m_dicWorkers)
            //{
            //    arr.Add(w.Value);
            //}
            arr.AddRange(m_dicWorkers.Values);
            return arr;
        }

        #endregion

        #region DataCar 접근 함수들
        public void AddCar(DataCar car)
        {
            m_dicCars[car.Code] = car;
            m_dicSensorCars[car.Sensor] = car;
        }

        // 장비코드로 찾는다.
        public DataCar FindCar(string strCode)
        {
            if (m_dicCars.ContainsKey(strCode))
                return m_dicCars[strCode];

            return null;
        }

        // SensorID로 찾는다.
        public DataCar FindCar2(string strSensorID)
        {
            if (m_dicSensorCars.ContainsKey(strSensorID))
                return m_dicSensorCars[strSensorID];

            return null;
        }

        // 장비코드로 찾아서 삭제한다.
        public void RemoveCar(string strCode)
        {
            if (m_dicCars.ContainsKey(strCode))
            {
                DataCar car = m_dicCars[strCode];

                m_dicCars.Remove(strCode);
                m_dicSensorCars.Remove(car.Sensor);
            }
        }

        // SensorID로 찾아서 삭제한다.
        public void RemoveCar2(string strSensorID)
        {
            if (m_dicSensorCars.ContainsKey(strSensorID))
            {
                DataCar car = m_dicSensorCars[strSensorID];

                m_dicCars.Remove(car.Code);
                m_dicSensorCars.Remove(strSensorID);
            }
        }

        public void RemoveCar(DataCar car)
        {
            if (m_dicCars.ContainsKey(car.Code))
            {
                m_dicCars.Remove(car.Code);
                m_dicSensorCars.Remove(car.Sensor);
            }
        }

        public void ClearCars()
        {
            m_dicCars.Clear();
            m_dicSensorCars.Clear();
        }

        public int GetCarCount()
        {
            return m_dicCars.Count;
        }

        public DataCar GetCar(int nIndex)
        {
            if (nIndex >= GetCarCount())
                return null;

            KeyValuePair<string, DataCar> pair = m_dicCars.ElementAt(nIndex);
            return pair.Value;
        }

        public DataCar GetCarFromID(int nID)
        {
            foreach (KeyValuePair<string, DataCar> w in m_dicCars)
            {
                if (w.Value.ID == nID)
                    return w.Value;
            }
            return null;
        }
        #endregion

        #region DataEquip 접근 함수들
        public void AddEquip(DataEquip equip)
        {
            m_dicEquips[equip.Code] = equip;
            m_dicSensorEquips[equip.Sensor] = equip;
        }

        // 설비코드 찾는다.
        public DataEquip FindEquip(string strCode)
        {
            if (m_dicEquips.ContainsKey(strCode))
                return m_dicEquips[strCode];

            return null;
        }

        // SensorID로 찾는다.
        public DataEquip FindEquip2(string strSensorID)
        {
            if (m_dicSensorEquips.ContainsKey(strSensorID))
                return m_dicSensorEquips[strSensorID];

            return null;
        }

        // 설비코드 찾아서 삭제한다.
        public void RemoveEquip(string strCode)
        {
            if (m_dicEquips.ContainsKey(strCode))
            {
                DataEquip equip = m_dicEquips[strCode];

                m_dicEquips.Remove(strCode);
                m_dicSensorEquips.Remove(equip.Sensor);
            }
        }

        // SensorID로 찾아서 삭제한다.
        public void RemoveEquip2(string strSensorID)
        {
            if (m_dicSensorEquips.ContainsKey(strSensorID))
            {
                DataEquip equip = m_dicSensorEquips[strSensorID];

                m_dicEquips.Remove(equip.Code);
                m_dicSensorEquips.Remove(strSensorID);
            }
        }

        public void RemoveEquip(DataEquip equip)
        {
            if (m_dicEquips.ContainsKey(equip.Code))
            {
                m_dicEquips.Remove(equip.Code);
                m_dicSensorEquips.Remove(equip.Sensor);
            }
        }

        public void ClearEquips()
        {
            m_dicEquips.Clear();
            m_dicSensorEquips.Clear();
        }

        public int GetEquipCount()
        {
            return m_dicEquips.Count;
        }
        public DataEquip GetEquipFromID(int nID)
        {
            foreach (KeyValuePair<string, DataEquip> w in m_dicEquips)
            {
                if (w.Value.ID == nID)
                    return w.Value;
            }
            return null;
        }
        public DataEquip GetEquip(int nIndex)
        {
            if (nIndex >= GetEquipCount())
                return null;

            KeyValuePair<string, DataEquip> pair = m_dicEquips.ElementAt(nIndex);
            return pair.Value;
        }
        #endregion

        public object FindSensorOwner(string strSensorID)
        {
            DataWorker worker = FindWorker2(strSensorID);

            if (worker != null)
                return worker;

            DataEquip equip = FindEquip2(strSensorID);

            if (equip != null)
                return equip;

            DataCar car = FindCar2(strSensorID);

            if (car != null)
                return car;

            return null;
        }

        //DB데이터 미리 복사
        public void SaveWorkerTempData()
        {
            m_arrTempWorkers.AddRange(m_dicWorkers.Values);
        }

        private void ClearAllData()
        {
            //m_arDataZones.Clear();
            m_arrZoneGroup.Clear();

            m_dicSensorWorkers.Clear();
            m_dicWorkers.Clear();
     
            m_arrTempWorkers.Clear();      
            m_arrDeleteWorkers.Clear();

            m_dicSensorCars.Clear();
            m_dicCars.Clear();

            m_dicSensorEquips.Clear();
            m_dicEquips.Clear();
            m_dicEquipRawDatas.Clear();

            m_arManagers.Clear();
            m_arrDetectIgnoreWorkers.Clear();
        }

        public void ReloadDBData()
        {
            ClearAllData();

            ERPManager.Instance.ReloadErpData();
            
            ReadDBData();
        }

        public void ReadDBData()
        {           

            LoadDataWorker();
            LoadDataCar();
            LoadDataEquip();
            LoadGasSensor();
            LoadOptions();

            LoadEquipRawData();
            LoadDataManager();

            LoadZone();
            m_arrDetectIgnoreWorkers = GetSensorIgnoreDatas();

            SaveWorkerTempData();
        }

        public void LoadDataWorker()
        {
            if (m_DBConnection == null)
                return;

            m_dicWorkers.Clear();

            //Erp에서 가져온 전체 worker데이터
            Dictionary<string, DataWorker> dicDataWorker = ERPManager.Instance.DicCompanyWorkers;
            int nSiteID = NetworkServer.Instance.SiteID;
            SqlConnection connect = m_DBConnection.Connect();

            string szSQL = string.Format("Select ID, MemberID, WorkerLevel, SiteID, SensorDetect, Description from Worker where SiteID = {0}", nSiteID);

            SqlDataReader rd = m_DBConnection.ExecuteReader(szSQL, connect);
            while (rd.Read())
            {
                int nWorkerID = Convert.ToInt32(rd[0].ToString().TrimEnd());
                //사원번호
                string strWorkerMemberID = rd[1].ToString().TrimEnd();
                //출입등급
                int nWorkerLevel = Convert.ToInt32(rd[2].ToString().TrimEnd());
                //int nSiteID = Convert.ToInt32(rd[3].ToString().TrimEnd());
                bool bDetectSensor = (bool)rd[4];


                DataWorker worker = null;
                bool bFindWorker = false;
                if (dicDataWorker.ContainsKey(strWorkerMemberID))
                {
                    worker = dicDataWorker[strWorkerMemberID];
                    bFindWorker = true;
                }
                

                if (worker == null)
                    worker = new DataWorker();

                worker.ID = nWorkerID;
                worker.MemberID = strWorkerMemberID;
                worker.EnterLevel = nWorkerLevel;
                worker.DBEnterLevel = nWorkerLevel;
                worker.SiteID = nSiteID;

                worker.SensorDetect = bDetectSensor;
                worker.DBSensorDetect = bDetectSensor;

                if (bFindWorker == true)
                {
                    AddWorker(worker);
                }                
                else
                {
                    // 삭제할 데이터로 추가
                    ProxyHSMS.Checker.AddChangedData(worker);
                }
                //m_dicWorkers[strWorkerID] = worker;
            }
            rd.Close();
            connect.Close();
            return;
        }

        public void LoadGasSensor()
        {
            if (m_DBConnection == null)
                return;

            SqlConnection connect = m_DBConnection.Connect();
            string szSQL = "SELECT ID, SensorName, SensorID, X, Y, Z, Description FROM GasSensor";

            SqlDataReader rd = m_DBConnection.ExecuteReader(szSQL, connect);
            while (rd.Read())
            {
                int nID = Convert.ToInt32(rd[0]);
                string strSensorName = rd[1].ToString();
                string strSensorID = rd[2].ToString();
                float fX = float.Parse(rd[3].ToString());
                float fY = float.Parse(rd[4].ToString());
                float fZ = float.Parse(rd[5].ToString());
                string strDescription = rd[6].ToString();

                GasSensor sensor = new GasSensor(nID, strSensorName, fX, fY, fZ, strDescription);
                sensor.SensorID = strSensorID;
                m_dicGasSensor[nID] = sensor;
            }

            rd.Close();
            connect.Close();
        }

        public void LoadDataCar()
        {
            if (m_DBConnection == null)
                return;

            m_dicCars.Clear();
            m_dicSensorCars.Clear();

            //Erp에서 가져온 전체 worker데이터
            Dictionary<string, DataCar> dicDataCar = ERPManager.Instance.DicCompanyCars;

            SqlConnection connect = m_DBConnection.Connect();
            int nSiteID = NetworkServer.Instance.SiteID;
            string szSQL = string.Format("Select ID, CarNumber, SiteID, SensorDetect, Description from Car where SiteID = {0}", nSiteID);

            SqlDataReader rd = m_DBConnection.ExecuteReader(szSQL, connect);
            while (rd.Read())
            {
                int nCarID = Convert.ToInt32(rd[0].ToString().TrimEnd());
                //차 코드
                string strCarNumber = rd[1].ToString().TrimEnd();
                //int nSiteID = Convert.ToInt32(rd[2].ToString().TrimEnd());
                bool bDetectSensor = (bool)rd[3];

                bool bFindCar = false;
                DataCar car = null;
                if (dicDataCar.ContainsKey(strCarNumber))
                {
                    car = dicDataCar[strCarNumber];
                    bFindCar = true;
                }

                if (car == null)
                    car = new DataCar();

                car.ID = nCarID;
                car.SiteID = nSiteID;
                car.SensorDetect = bDetectSensor;
                car.DBSensorDetect = bDetectSensor;

                if (bFindCar == true)
                    AddCar(car);
                else
                    ProxyHSMS.Checker.AddChangedData(car);

            }
            rd.Close();
            connect.Close();
            return;
        }

        public void LoadEquipRawData()
        {
            if (m_DBConnection == null)
                return;

            m_dicEquipRawDatas.Clear();

            int nSiteID = NetworkServer.Instance.SiteID;
            SqlConnection connect = m_DBConnection.Connect();

            string szSQL = string.Format("Select ID, EquipName, Boundary, SiteID, TextCenter, SensorPos, SensorFinishPos, SensorDirVector from EquipRawData where SiteID = {0}", nSiteID);

            SqlDataReader rd = m_DBConnection.ExecuteReader(szSQL, connect);
            while (rd.Read())
            {
                int nID = Convert.ToInt32(rd[0].ToString().TrimEnd());
                string strEquipName = rd[1].ToString().TrimEnd();
                string strBoundary = rd[2].ToString().TrimEnd();
                //int nSiteID = Convert.ToInt32(rd[3].ToString().TrimEnd());
                string strTextCenter = rd[4].ToString().TrimEnd();
                //bool bDetectSensor = (bool)rd[4];
                string strSensorPos = rd[5] == null ? "" : rd[5].ToString().TrimEnd();
                string strSensorFinishPos = rd[6] == null ? "" : rd[6].ToString().TrimEnd();
                string strSensorDirVector = rd[7] == null ? "" : rd[7].ToString().TrimEnd();

                EquipmentRawData equipRawData = new EquipmentRawData();

                equipRawData.ID = nID;
                equipRawData.Name = strEquipName;
                equipRawData.Boundary = strBoundary;
                equipRawData.SiteID = nSiteID;
                equipRawData.TextCenter = strTextCenter;
                equipRawData.SensorPos = strSensorPos;
                equipRawData.SensorFinishPos = strSensorFinishPos;
                equipRawData.SensorDirVector = strSensorDirVector;

                if (!m_dicEquipRawDatas.ContainsKey(strEquipName))
                {
                    m_dicEquipRawDatas[strEquipName] = equipRawData;
                }
            }
            rd.Close();
            connect.Close();
            return;
        }

        public EquipmentGroup FindEquipmentGroup(string strEquipGroupName)
        {
            foreach (EquipmentGroup group in m_arrEquipGroup)
            {
                if (group.GroupName == strEquipGroupName || group.ToString() == strEquipGroupName)
                    return group;
            }

            return null;
        }

        public int GetEquipmentGroupCount()
        {
            return m_arrEquipGroup.Count;
        }

        public EquipmentGroup GetEquipmentGroup(int nIndex)
        {
            if (nIndex < 0 || nIndex > GetEquipmentGroupCount())
                return null;

            return (EquipmentGroup)m_arrEquipGroup[nIndex];
        }

        public void AddEquipmentGroup(EquipmentGroup group)
        {
            if (!m_arrEquipGroup.Contains(group))
                m_arrEquipGroup.Add(group);
        }

        public void LoadDataEquip()
        {
            if (m_DBConnection == null)
                return;

            m_dicEquips.Clear();

            //Erp에서 가져온 전체 worker데이터
            Dictionary<string, DataEquip> dicEquip = ERPManager.Instance.DicEquips;

            SqlConnection connect = m_DBConnection.Connect();

            string szSQL = "Select ID, EquipCode, MeshName, EquipGroupName, Boundary, SensorPos, SensorFinishPos, SensorDirVector, SiteID, SensorDetect, TextCenter, Description from Equipment";

            EquipmentGroup group = null;

            SqlDataReader rd = m_DBConnection.ExecuteReader(szSQL, connect);
            while (rd.Read())
            {
                int nID = Convert.ToInt32(rd[0].ToString().TrimEnd());
                //설비 코드
                string strEquipCode = rd[1].ToString().TrimEnd();
                //설비이름
                string strMeshName = rd[2].ToString().TrimEnd();

                if (rd.IsDBNull(3))
                {
                    group = EquipmentGroup.DefaultEquipmentGroup;

                    if (FindEquipmentGroup(group.GroupName) == null)
                        m_arrEquipGroup.Add(group);
                }
                else
                {
                    string strEquipGroupName = rd[3].ToString().TrimEnd();
                    group = FindEquipmentGroup(strEquipGroupName);

                    if (group == null)
                    {
                        if (strEquipGroupName == EquipmentGroup.DefaultEquipmentGroup.GroupName ||
                            strEquipGroupName == EquipmentGroup.DefaultEquipmentGroup.ToString())
                        {
                            group = EquipmentGroup.DefaultEquipmentGroup;

                            if (FindEquipmentGroup(group.GroupName) == null)
                                m_arrEquipGroup.Add(group);
                        }
                        else
                        {
                            group = new EquipmentGroup(strEquipGroupName);
                            m_arrEquipGroup.Add(group);
                        }
                    }
                }

                string strBoundary = rd[4].ToString().TrimEnd();
                string strSensorPos = rd.IsDBNull(5) ? "" : rd[5].ToString().TrimEnd();
                string strSensorFinishPos = rd.IsDBNull(6) ? "" : rd[6].ToString().TrimEnd();
                string strSensorDirVector = rd.IsDBNull(7) ? "" : rd[7].ToString().TrimEnd();
                int nSiteID = Convert.ToInt32(rd[8].ToString().TrimEnd());

                UnE.Geometry.Polygon polygon = GetPolygon(strBoundary);

                if (polygon == null)
                    continue;

                UnE.Geometry.Vertex2D vEquipOrigin = ResetPolygonCoords(polygon);
                UnE.Geometry.Vertex2D vSensorPos = GetVertex(strSensorPos);
                UnE.Geometry.Vertex2D vSensorFinishPos = GetVertex(strSensorFinishPos);
                UnE.Geometry.Vertex2D vSensorDirVector = GetVertex(strSensorDirVector);
    
                bool bDetectSensor = (bool)rd[9];

                bool bFindEquip = false;
                DataEquip equip = null;
                if (dicEquip.ContainsKey(strEquipCode))
                {
                    equip = dicEquip[strEquipCode];
                    bFindEquip = true;
                }

                if (equip == null)
                    equip = new DataEquip();

                if (vSensorPos != null)
                    equip.SensorPosition = vSensorPos;

                if (vSensorFinishPos != null)
                    equip.SensorFinishPosition = vSensorFinishPos;

                if (vSensorDirVector != null)
                    equip.SensorDirVector = vSensorDirVector;

                equip.ID = nID;
                equip.Boundary = polygon;
                equip.OriginPosition = vEquipOrigin;
                equip.SiteID = nSiteID;
                equip.SensorDetect = bDetectSensor;
                equip.DBSensorDetect = bDetectSensor;

                equip.SetLiked3DEquipmentFromName();
                SetLinked3DEquipmentMovingArea(equip.Linked3DEquipment, vSensorPos, vSensorFinishPos);

                if (bFindEquip == true)
                    AddEquip(equip);
                else
                    ProxyHSMS.Checker.AddChangedData(equip);
                //m_dicEquips[strEquipCode] = equip;
            }
            rd.Close();
            connect.Close();
            return;
        }

        private void SetLinked3DEquipmentMovingArea(_3DEquipment equip3D, UnE.Geometry.Vertex2D vSensorPos, UnE.Geometry.Vertex2D vSensorFinishPos)
        {
            if (equip3D != null)
            {
                if (equip3D.GetType() == typeof(MovingEquip3D))
                {
                    if (vSensorPos.x < vSensorFinishPos.x)
                    {
                        equip3D.MinMovedX = new PrimitiveData<double>(vSensorPos.x);
                        equip3D.MaxMovedX = new PrimitiveData<double>(vSensorFinishPos.x);
                    }
                    else
                    {
                        equip3D.MinMovedX = new PrimitiveData<double>(vSensorFinishPos.x);
                        equip3D.MaxMovedX = new PrimitiveData<double>(vSensorPos.x);
                    }

                    if (vSensorPos.y < vSensorFinishPos.y)
                    {
                        equip3D.MinMovedY = new PrimitiveData<double>(vSensorPos.y);
                        equip3D.MaxMovedY = new PrimitiveData<double>(vSensorFinishPos.y);
                    }
                    else
                    {
                        equip3D.MinMovedY = new PrimitiveData<double>(vSensorFinishPos.y);
                        equip3D.MaxMovedY = new PrimitiveData<double>(vSensorPos.y);
                    }
                }
                else if (equip3D.GetType() == typeof(Crane3D))
                {
                    if (vSensorPos.x < vSensorFinishPos.x)
                    {
                        equip3D.MinMovedX = new PrimitiveData<double>(vSensorPos.x);
                        equip3D.MaxMovedX = new PrimitiveData<double>(vSensorFinishPos.x);
                    }
                    else
                    {
                        equip3D.MinMovedX = new PrimitiveData<double>(vSensorFinishPos.x);
                        equip3D.MaxMovedX = new PrimitiveData<double>(vSensorPos.x);
                    }
                }
            }
        }

        public UnE.Geometry.Vertex2D GetVertex(string strVertex)
        {
            string[] arrCoords = strVertex.Split(',');

            if (arrCoords.Count() != 2)
                return null;

            char[] arrTrims = new char[] { ' ', '\t', '\r', '\n' };
            
            arrCoords[0] = arrCoords[0].TrimStart(arrTrims);
            arrCoords[0] = arrCoords[0].TrimEnd(arrTrims);
            arrCoords[1] = arrCoords[1].TrimStart(arrTrims);
            arrCoords[1] = arrCoords[1].TrimEnd(arrTrims);

            double x, y;

            if (!double.TryParse(arrCoords[0], out x))
                return null;

            if (!double.TryParse(arrCoords[1], out y))
                return null;

            return new UnE.Geometry.Vertex2D(x, y);
        }

        public UnE.Geometry.Polygon GetPolygon(string strPolygon)
        {
            string[] arrCoords = strPolygon.Split(',');
            int nCoordCount = arrCoords.Length;

            if (nCoordCount % 2 != 0)
                return null;

            char[] arrTrims = new char[] { ' ', '\t', '\r', '\n' };

            for (int i = 0; i < nCoordCount; i++ )
            {
                arrCoords[i] = arrCoords[i].TrimStart(arrTrims);
                arrCoords[i] = arrCoords[i].TrimEnd(arrTrims);
            }

            double x, y;
            UnE.Geometry.Polygon polygon = new UnE.Geometry.Polygon();

            for (int i = 0; i < nCoordCount; i += 2)
            {
                if (!double.TryParse(arrCoords[i], out x))
                    return null;

                if (!double.TryParse(arrCoords[i + 1], out y))
                    return null;

                polygon.AddVertex(new UnE.Geometry.Vertex2D(x, y));
            }

            return polygon;
        }

        // polygon 좌표를 가장 작은값을 기준으로 위치 이동시킨다.
        public UnE.Geometry.Vertex2D ResetPolygonCoords(UnE.Geometry.Polygon polygon)
        {
            int nVertexCount = polygon.GetVertexCount();
            UnE.Geometry.Vertex2D vMin = new UnE.Geometry.Vertex2D();

            for (int i = 0; i < nVertexCount; i++)
            {
                UnE.Geometry.Vertex2D vertex = polygon.GetVertex(i);

                if (i == 0)
                    vMin.SetVertex(vertex.x, vertex.y);
                else
                {
                    if (vMin.x > vertex.x)
                        vMin.x = vertex.x;
                    if (vMin.y > vertex.y)
                        vMin.y = vertex.y;
                }
            }

            for (int i = 0; i < nVertexCount; i++)
            {
                UnE.Geometry.Vertex2D vertex = polygon.GetVertex(i);
                polygon.UpdateVertex(i, vertex - vMin);
            }

            return vMin;
        }

        public ZoneGroup FindZoneGroup(string strZoneGroupName)
        {
            foreach (ZoneGroup group in m_arrZoneGroup)
            {
                if (group.GroupName == strZoneGroupName)
                    return group;
            }

            return null;
        }

        public int GetZoneGroupCount()
        {
            return m_arrZoneGroup.Count;
        }

        public ZoneGroup GetZoneGroup(int nIndex)
        {
            if (nIndex < 0 || nIndex > GetZoneGroupCount())
                return null;

            return (ZoneGroup)m_arrZoneGroup[nIndex];
        }

        public void AddZoneGroup(ZoneGroup group)
        {
            if (!m_arrZoneGroup.Contains(group))
                m_arrZoneGroup.Add(group);
        }

        private void LoadZone()
        {
            if (m_DBConnection == null)
                return;

            //m_arDataZones.Clear();
            m_arrZoneGroup.Clear();

            SqlConnection connect = m_DBConnection.Connect();
            
            int nSiteID = NetworkServer.Instance.SiteID;

            string szSQL = "Select ID, ZoneName, ZoneGroupName, Boundary, PermitLevel, TextCenter, Description from Zone where SiteID="  + nSiteID;

            ZoneGroup group = null;

            SqlDataReader rd = m_DBConnection.ExecuteReader(szSQL, connect);
            while (rd.Read())
            {
                int nID = (int)rd[0];

                string szZoneName = rd[1].ToString().TrimEnd();
                string szBoundary = rd[3].ToString().TrimEnd();

                if (rd.IsDBNull(2))
                {
                    group = ZoneGroup.DefaultZoneGroup;

                    if (FindZoneGroup(group.GroupName) == null)
                        m_arrZoneGroup.Add(group);
                }
                else
                {
                    string strZoneGroupName = rd[2].ToString().TrimEnd();
                    group = FindZoneGroup(strZoneGroupName);

                    if (group == null)
                    {
                        if (strZoneGroupName == ZoneGroup.DefaultZoneGroup.GroupName ||
                            strZoneGroupName == ZoneGroup.DefaultZoneGroup.ToString())
                        {
                            group = ZoneGroup.DefaultZoneGroup;

                            if (FindZoneGroup(group.GroupName) == null)
                                m_arrZoneGroup.Add(group);
                        }
                        else
                        {
                            group = new ZoneGroup(strZoneGroupName);
                            m_arrZoneGroup.Add(group);
                        }
                    }
                }

                string szPermitLevel = rd[4] != null ? rd[4].ToString().TrimEnd() : "";

                string szTextCenter = rd[5] != null ? rd[5].ToString().TrimEnd() : "";
                string szDescription = rd[6] != null ? rd[6].ToString().TrimEnd() : "";

                DataZone zone = new DataZone(group);

                //////////////////////////////////////////////////////////////////////////
                 
                // boundary를 얻어 폴리곤을 구성한다.
                string[] bounds = szBoundary.Split(',');
                UnE.Geometry.Polygon polygon = new UnE.Geometry.Polygon();                     
                for (int i = 0; i < bounds.Length; i+= 2)
                {
                    float x = 0.0f;
                    float y = 0.0f;
                    if (float.TryParse(bounds[i], out x))
                    {
                        if (float.TryParse(bounds[i + 1], out y))
                        {
                            polygon.AddVertex(new UnE.Geometry.Vertex2D(x, y));
                        }
                    }
                    
                }
                polygon.CalcWeightCenter();

                zone.ID = nID;
                zone.Boundary = polygon;
                zone.ZoneName = szZoneName;
                string[] centers = szTextCenter.Split(',');
                for (int i = 0; i < centers.Length; i += 2)
                {
                    float x = 0.0f;
                    float y = 0.0f;
                    if (float.TryParse(centers[i], out x))
                    {
                        if (float.TryParse(centers[i + 1], out y))
                        {
                            zone.TextCenter = new UnE.Geometry.Vertex2D(x, y);
                        }
                    }
                }

                if (szPermitLevel != null && szPermitLevel != "")
                {
                    string[] permits = szPermitLevel.Split(',');
                    for (int i = 0; i < permits.Length; i++)
                    {
                        int nLevel = 0;
                        if (int.TryParse(permits[i], out nLevel))
                        {
                            zone.AddPermitLevel(nLevel);
                        }
                    }
                }

                //m_arDataZones.Add(zone);
            }

            rd.Close();
            connect.Close();

            return;
        }


        private void LoadOptions()
        {
            if (m_DBConnection == null)
                return;

            SqlConnection connect = m_DBConnection.Connect();
            int nSiteID = NetworkServer.Instance.SiteID;
            string szSQL = "Select ID, ItemName, ItemValue from Options where SiteID = " + nSiteID.ToString();

            SqlDataReader rd = m_DBConnection.ExecuteReader(szSQL, connect);
            while (rd.Read())
            {
                int nID = (int)rd[0];
                string strItemName = rd[1].ToString().TrimEnd();
                string strItemValue = rd[2].ToString().TrimEnd();
                if (strItemValue == "1")
                    m_bMessageChecked = true;
                else
                    m_bMessageChecked = false;

                if (string.Compare(strItemName, "WorkerToCarDistanceBoth", true) == 0)
                {
                    int nDistance;

                    if (int.TryParse(strItemValue, out nDistance))
                        m_fWorkerToCarDistanceBoth = nDistance / 1000.0f;
                }
                else if (string.Compare(strItemName, "WorkerToCarDistanceOneSide", true) == 0)
                {
                    int nDistance;

                    if (int.TryParse(strItemValue, out nDistance))
                        m_fWorkerToCarDistanceOneSide = nDistance / 1000.0f;
                }
                else if (string.Compare(strItemName, "WorkerToZoneDistance", true) == 0)
                {
                    int nIndex = strItemValue.IndexOf('_');

                    if (nIndex >= 0)
                    {
                        string strZoneGroupName = strItemValue.Substring(0, nIndex);
                        string strDistance = strItemValue.Substring(nIndex + 1);

                        int nDistance;

                        if (int.TryParse(strDistance, out nDistance))
                        {
                            SetWorkerToZoneDistance(strZoneGroupName, nDistance / 1000.0f);

                            if (FindZoneGroup(strZoneGroupName) == null)
                            {
                                ZoneGroup group = new ZoneGroup(strZoneGroupName);
                                AddZoneGroup(group);
                            }
                        }
                    }
                    /*int nDistance;

                    if (int.TryParse(strItemValue, out nDistance))
                        m_fWorkerToZoneDistance = nDistance / 1000.0f;*/
                }
                else if (string.Compare(strItemName, "WorkerToEquipDistance", true) == 0)
                {
                    int nIndex = strItemValue.IndexOf('_');

                    if (nIndex >= 0)
                    {
                        string strEquipGroupName = strItemValue.Substring(0, nIndex);
                        string strDistance = strItemValue.Substring(nIndex + 1);

                        int nDistance;

                        if (int.TryParse(strDistance, out nDistance))
                        {
                            SetWorkerToEquipDistance(strEquipGroupName, nDistance / 1000.0f);

                            if (FindEquipmentGroup(strEquipGroupName) == null)
                            {
                                EquipmentGroup group = new EquipmentGroup(strEquipGroupName);
                                AddEquipmentGroup(group);
                            }
                        }
                    }
                    /*int nDistance;

                    if (int.TryParse(strItemValue, out nDistance))
                        m_fWorkerToEquipDistance = nDistance / 1000.0f;*/
                }
                else if (string.Compare(strItemName, "AlarmIgnoreOptionCar", true) == 0)
                {
                    int nOption;

                    if (int.TryParse(strItemValue, out nOption))
                    {
                        if (nOption >= (int)AlarmManager.AlarmIgnoreOption.NONE && nOption < (int)AlarmManager.AlarmIgnoreOption.TYPE_COUNT)
                        {
                            NetworkServer.Instance.AlarmManager.IgnoreOptionCar = (AlarmManager.AlarmIgnoreOption)nOption;
                        }
                    }
                }
                else if (string.Compare(strItemName, "AlarmIgnoreOptionEquip", true) == 0)
                {
                    int nOption;

                    if (int.TryParse(strItemValue, out nOption))
                    {
                        if (nOption >= (int)AlarmManager.AlarmIgnoreOption.NONE && nOption < (int)AlarmManager.AlarmIgnoreOption.TYPE_COUNT)
                        {
                            NetworkServer.Instance.AlarmManager.IgnoreOptionEquip = (AlarmManager.AlarmIgnoreOption)nOption;
                        }
                    }
                }
                else if (string.Compare(strItemName, "AlarmIgnoreOptionZone", true) == 0)
                {
                    int nOption;

                    if (int.TryParse(strItemValue, out nOption))
                    {
                        if (nOption >= (int)AlarmManager.AlarmIgnoreOption.NONE && nOption < (int)AlarmManager.AlarmIgnoreOption.TYPE_COUNT)
                        {
                            NetworkServer.Instance.AlarmManager.IgnoreOptionZone = (AlarmManager.AlarmIgnoreOption)nOption;
                        }
                    }
                }
                else if (string.Compare(strItemName, "AlarmIgnoreDistanceCar", true) == 0)
                {
                    int nDistance;

                    if (int.TryParse(strItemValue, out nDistance))
                    {
                        if (nDistance >= 0)
                        {
                            NetworkServer.Instance.AlarmManager.IgnoreDistanceCar = nDistance;
                        }
                    }
                }
                else if (string.Compare(strItemName, "AlarmIgnoreDistanceEquip", true) == 0)
                {
                    int nDistance;

                    if (int.TryParse(strItemValue, out nDistance))
                    {
                        if (nDistance >= 0)
                        {
                            NetworkServer.Instance.AlarmManager.IgnoreDistanceEquip = nDistance;
                        }
                    }
                }
                else if (string.Compare(strItemName, "AlarmIgnoreDistanceZone", true) == 0)
                {
                    int nDistance;

                    if (int.TryParse(strItemValue, out nDistance))
                    {
                        if (nDistance >= 0)
                        {
                            NetworkServer.Instance.AlarmManager.IgnoreDistanceZone = nDistance;
                        }
                    }
                }
                else if (string.Compare(strItemName, "AlarmIgnoreTimeCar", true) == 0)
                {
                    int nTime;

                    if (int.TryParse(strItemValue, out nTime))
                    {
                        if (nTime >= 0)
                        {
                            NetworkServer.Instance.AlarmManager.IgnoreTimeCar = nTime;
                        }
                    }
                }
                else if (string.Compare(strItemName, "AlarmIgnoreTimeEquip", true) == 0)
                {
                    int nTime;

                    if (int.TryParse(strItemValue, out nTime))
                    {
                        if (nTime >= 0)
                        {
                            NetworkServer.Instance.AlarmManager.IgnoreTimeEquip = nTime;
                        }
                    }
                }
                else if (string.Compare(strItemName, "AlarmIgnoreTimeZone", true) == 0)
                {
                    int nTime;

                    if (int.TryParse(strItemValue, out nTime))
                    {
                        if (nTime >= 0)
                        {
                            NetworkServer.Instance.AlarmManager.IgnoreTimeZone = nTime;
                        }
                    }
                }
                else if (string.Compare(strItemName, "SensorIgnoreTime", true) == 0)
                {
                    int nTime;

                    if (int.TryParse(strItemValue, out nTime))
                    {
                        if (SafetyChecker.Instance == null)
                            SafetyChecker.DefIgnoreSensorMinute = nTime;
                        else
                            SafetyChecker.Instance.IgnoreSensorMinute = nTime;
                    }
                }
                else if (string.Compare(strItemName, "COGasTolerance", true) == 0)
                {
                    float fTolerance;

                    if (float.TryParse(strItemValue, out fTolerance))
                    {
                        m_fCOGasTolerance = fTolerance;
                    }
                }
                else if (string.Compare(strItemName, "MethaneTolerance", true) == 0)
                {
                    float fTolerance;

                    if (float.TryParse(strItemValue, out fTolerance))
                    {
                        m_fMethaneTolerance = fTolerance;
                    }
                }
            }

            rd.Close();
            connect.Close();
            return;
        }

        public ArrayList GetSensorIgnoreDatas()
        {

            if (m_DBConnection == null)
                return null;

            //m_arrDetectIgnoreWorkers.Clear();

            SqlConnection connect = m_DBConnection.Connect();

            int nSiteID = NetworkServer.Instance.SiteID;

            ArrayList arIgnoreDatas = new ArrayList();
            

            string szSQL = string.Format("Select WorkerID, ObjectID, ObjectType, Description from IgnoreSensorsToWorker where SiteID = {0}", nSiteID);

            try
            {
                SqlDataReader rd = m_DBConnection.ExecuteReader(szSQL, connect);
                while (rd.Read())
                {
                    int nWorkerID = (int)rd[0];
                    int nObjID = (int)rd[1];
                    int nObjType = (int)rd[2];

                    DataWorker worker = GetWorkerFromID(nWorkerID);
                    if (worker == null)
                        continue;

                    DetectIgnoreWorker data = new DetectIgnoreWorker();
                    data.IgnoreObjectID = nObjID;
                    data.IgnoreObjectType = nObjType;
                    data.WorkerID = nWorkerID;
                    data.SiteID = nSiteID;

                    data.Worker = worker;

                    arIgnoreDatas.Add(data);
                }
                rd.Close();
                connect.Close();
            }
            catch (System.Exception)
            {            	
            }           
            return arIgnoreDatas;
        }

        public DataZone FindZone(int nZoneID)
        {
            foreach (ZoneGroup group in m_arrZoneGroup)
            {
                int nZoneCount = group.GetZoneCount();

                for (int i=0;i<nZoneCount;i++)
                {
                    DataZone zone = group.GetZone(i);
                    if (zone.ID == nZoneID)
                        return zone;
                }
            }
            /*foreach (DataZone zone in DataZones)
            {
                if (zone.ID == nZoneID)
                    return zone;
            }*/

            return null;
        }

        public GasSensor FindGasSensor(string strSensorID)
        {
            foreach (KeyValuePair<int, GasSensor> pair in m_dicGasSensor)
            {
                if (pair.Value.SensorID == strSensorID)
                    return pair.Value;
            }

            return null;
        }

        public void AddManager(Manager mgr)
        {
            if (!m_arManagers.Contains(mgr))
                m_arManagers.Add(mgr);
        }
        public void RemoveManager(Manager mgr)
        {
            if (m_arManagers.Contains(mgr))
                m_arManagers.Remove(mgr);
        }

        public Manager GetManager(string szMemberID)
        {
            foreach (Manager mgr in m_arManagers)
            {
                if (mgr.MemberID == szMemberID)
                {
                    return mgr;
                }
            }
            return null;
        }
        public Manager GetManager(int nID)
        {
            foreach (Manager mgr in m_arManagers)
            {
                if (mgr.ID == nID)
                {
                    return mgr;
                }
            }
            return null;
        }

        private void LoadDataManager()
        {
            if (m_DBConnection == null)
                return;

            int nSiteID = NetworkServer.Instance.SiteID;

            SqlConnection connect = m_DBConnection.Connect();

            m_arManagers.Clear();

            string szSQL = string.Format("Select ID, MemberID, Description from Manager where SiteID = {0}", nSiteID);

            try
            {

                SqlDataReader rd = m_DBConnection.ExecuteReader(szSQL, connect);
                while (rd.Read())
                {
                    int nID = (int)rd[0];
                    string szMemberID = rd[1].ToString().TrimEnd();

                    Manager mgr = new Manager();
                    mgr.ID = nID;
                    mgr.MemberID = szMemberID;

                    Dictionary<string, DataWorker> workers = ERPManager.Instance.DicCompanyWorkers;
                    if (workers.ContainsKey(szMemberID))
                    {
                        mgr.Worker = workers[szMemberID];
                    }

                    m_arManagers.Add(mgr);
                }
                rd.Close();
                connect.Close();
            }
            catch (Exception)
            {
            }
        }

        public HSMS.DetectIgnoreWorker FindIgnoreWorker(int nWorkerID, int nIgnoreObjectID, int nIgnoreObjectType, int nSiteID)
        {
            if (m_arrDetectIgnoreWorkers == null)
                return null;

            foreach (HSMS.DetectIgnoreWorker ignore in m_arrDetectIgnoreWorkers)
            {
                if (ignore.WorkerID == nWorkerID && ignore.IgnoreObjectID == nIgnoreObjectID &&
                    ignore.IgnoreObjectType == nIgnoreObjectType && ignore.SiteID == nSiteID)
                    return ignore;
            }

            return null;
        }
    }
}
