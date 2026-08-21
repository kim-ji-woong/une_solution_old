using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections;

namespace HSMS
{
    public class DataManager
    {
        private DBConn m_DBConnection = null;
        public DBConn DBManager
        {
            get { return m_DBConnection; }
        }

        //private static DataManager m_Instance = null;
        //public static DataManager Instance
        //{
        //    get { return DataManager.m_Instance; }
        //    set { DataManager.m_Instance = value; }
        //}

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
        /*public Dictionary<string, DataWorker> DicWorkers
        {
            get { return m_dicWorkers; }
            set { m_dicWorkers = value; }
        }*/

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

        //SensorID, 차량데이터
        private Dictionary<string, DataCar> m_dicSensorCars = new Dictionary<string, DataCar>();
        //장비코드, 차량데이터
        private Dictionary<string, DataCar> m_dicCars = new Dictionary<string, DataCar>();
        /*public Dictionary<string, DataCar> DicCars
        {
            get { return m_dicCars; }
            set { m_dicCars = value; }
        }*/

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
        public ArrayList Managers
        {
            get { return m_arManagers; }
        }
        
        private ArrayList m_arrDetectIgnoreWorkers = null;
        public ArrayList DetectIgnoreWorkers
        {
            get { return m_arrDetectIgnoreWorkers; }
        }

        private string m_strCaller = "";
        private string m_strReceiver = "";

        public string Caller
        {
            get { return m_strCaller; }
        }

        public string Receiver
        {
            get { return m_strReceiver; }
        }

        // 작업자와 차량이 상호 마주보고 가까워지는 경우 안전거리(m)
        private float m_fWorkerToCarDistanceBoth = 5.0f;
        // 작업자와 차량중 한쪽에서 다가서는 경우 안전거리(m)
        private float m_fWorkerToCarDistanceOneSide = 3.0f;
        // 작업자와 위험영역간 안전거리(m)
        //private float m_fWorkerToZoneDistance = 2.0f;
        // 작업자와 위험설비간 안전거리(m)
        //private float m_fWorkerToEquipDistance = 2.0f;

        // 작업자와 위험영역간 안전거리(m)
        // Key : ZoneGroup Name
        private Dictionary<string, float> m_dicWorkerToZoneDistance = new Dictionary<string, float>();
        // 작업자와 위험설비간 안전거리(m)
        // Key : EquipGroup Name
        private Dictionary<string, float> m_dicWorkerToEquipDistance = new Dictionary<string, float>();

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

        private DateTime m_dtLastAccess;
        public System.DateTime LastAccess
        {
            get { return m_dtLastAccess; }
        }

        private bool m_bMessageChecked;
        public bool MessageChecked
        {
            get { return m_bMessageChecked; }
            set { m_bMessageChecked = value; }
        }

        public float COGasTolerance
        {
            get { return m_fCOGasTolerance; }
            set { m_fCOGasTolerance = value; }
        }

        public float MethaneTolerance
        {
            get { return m_fMethaneTolerance; }
            set { m_fMethaneTolerance = value; }
        }

        private Dictionary<int, APData> m_dicAP = new Dictionary<int, APData>();
        public Dictionary<int, APData> DicAPs
        {
            get { return m_dicAP; }
        }

        private Dictionary<int, GasSensor> m_dicGasSensor = new Dictionary<int, GasSensor>();
        public Dictionary<int, GasSensor> DicGasSensors
        {
            get { return m_dicGasSensor; }
        }

        public DataManager()
        {
            m_DBConnection = new DBConn("HSMS");
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

        public void ReloadDBData()
        {
            ConnectionLogEx.Instance.WriteLine("Clear All Data");
            ClearAllData();

            ConnectionLogEx.Instance.WriteLine("Reload ERP Data");
            ERPManager.Instance.ReloadErpData();
            
            ConnectionLogEx.Instance.WriteLine("Reload HSMS Data");
            ReadDBData();

            ApplyCCTVData();
            ApplyAPData();

            m_dtLastAccess = DateTime.Now;
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

        #region DataWorker 접근 함수들
        public void AddWorker(DataWorker worker)
        {
            m_dicWorkers[worker.MemberID] = worker;
            m_dicSensorWorkers[worker.Sensor] = worker;
        }

        public DataWorker FindWorker(int nID)
        {
            foreach (KeyValuePair<string, DataWorker> pair in m_dicWorkers)
            {
                if (pair.Value.ID == nID)
                    return pair.Value;
            }

            return null;
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

        public ArrayList GetCars()
        {
            ArrayList arr = new ArrayList();

            arr.AddRange(m_dicCars.Values);
            return arr;
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

            if (equip.EquipmentGroup != null)
            {
                if (!m_arrEquipGroup.Contains(equip.EquipmentGroup))
                    AddEquipmentGroup(equip.EquipmentGroup);
            }
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

        public GasSensor FindGasSensor(string strSensorID)
        {
            foreach (KeyValuePair<int, GasSensor> pair in m_dicGasSensor)
            {
                if (pair.Value.SensorID == strSensorID)
                    return pair.Value;
            }

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

        public ArrayList GetEquips()
        {
            ArrayList arr = new ArrayList();

            arr.AddRange(m_dicEquips.Values);
            return arr;
        }
        #endregion

        public DataZone FindZone(int nZoneID)
        {
            int nZoneGroupCount = m_arrZoneGroup.Count;

            for (int i = 0; i < nZoneGroupCount;i++ )
            {
                ZoneGroup group = GetZoneGroup(i);
                int nZoneCount = group.GetZoneCount();

                for (int j=0;j<nZoneCount;j++)
                {
                    DataZone zone = group.GetZone(j);

                    if (zone.ID == nZoneID)
                        return zone;
                }
            }
            /*foreach (DataZone zone in m_arDataZones)
            {
                if (zone.ID == nZoneID)
                    return zone;
            }*/

            return null;
        }

        public DataZone FindZone(string strZoneName)
        {
            int nZoneGroupCount = m_arrZoneGroup.Count;

            for (int i = 0; i < nZoneGroupCount; i++)
            {
                ZoneGroup group = GetZoneGroup(i);
                int nZoneCount = group.GetZoneCount();

                for (int j = 0; j < nZoneCount; j++)
                {
                    DataZone zone = group.GetZone(j);

                    if (zone.ZoneName == strZoneName)
                        return zone;
                }
            }
            /*foreach (DataZone zone in m_arDataZones)
            {
                if (zone.ZoneName == strZoneName)
                    return zone;
            }*/

            return null;
        }

        //DB데이터 미리 복사
        public void SaveWorkerTempData()
        {
            foreach (KeyValuePair<string, DataWorker> w in m_dicWorkers)
            {
                m_arrTempWorkers.Add(w.Value);
            }
            //m_arrTempWorkers.AddRange(m_dicWorkers.Values);
        }

        public void ReadDBData()
        {  
            LoadDataWorker();
            LoadDataCar();
            LoadDataEquip();
            LoadEquipRawData();
            LoadCCTV();
            LoadAP();
            LoadGasSensor();
            LoadOptions();

            LoadDataManager();

            LoadZone();
            m_arrDetectIgnoreWorkers = LoadSensorIgnoreDatas();
            SaveWorkerTempData();

            m_dtLastAccess = DateTime.Now;
        }

        public void LoadDataWorker()
        {
            if (m_DBConnection == null)
                return;

            m_dicWorkers.Clear();

            //Erp에서 가져온 전체 worker데이터
            Dictionary<string, DataWorker> dicDataWorker = ERPManager.Instance.DicCompanyWorkers;
            int nSiteID = FormMain.Instance.SiteID;
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
                if (dicDataWorker.ContainsKey(strWorkerMemberID))
                {
                    worker = dicDataWorker[strWorkerMemberID];
                }

                if (worker == null)
                    continue;

                worker.ID = nWorkerID;
                worker.MemberID = strWorkerMemberID;
                worker.EnterLevel = nWorkerLevel;
                worker.DBEnterLevel = nWorkerLevel;
                worker.SiteID = nSiteID;

                worker.SensorDetect = bDetectSensor;
                worker.DBSensorDetect = bDetectSensor;

                AddWorker(worker);
            }
            rd.Close();
            connect.Close();
            return;
        }


        public void LoadDataCar()
        {
            if (m_DBConnection == null)
                return;

            m_dicCars.Clear();

            //Erp에서 가져온 전체 worker데이터
            Dictionary<string, DataCar> dicDataCar = ERPManager.Instance.DicCompanyCars;

            SqlConnection connect = m_DBConnection.Connect();
            int nSiteID = FormMain.Instance.SiteID;
            string szSQL = string.Format("Select ID, CarNumber, SiteID, SensorDetect, Description from Car where SiteID = {0}", nSiteID);

            SqlDataReader rd = m_DBConnection.ExecuteReader(szSQL, connect);
            while (rd.Read())
            {
                int nCarID = Convert.ToInt32(rd[0].ToString().TrimEnd());
                //차 코드
                string strCarNumber = rd[1].ToString().TrimEnd();
                //int nSiteID = Convert.ToInt32(rd[2].ToString().TrimEnd());
                bool bDetectSensor = (bool)rd[3];

                DataCar car = null;
                if (dicDataCar.ContainsKey(strCarNumber))
                {
                    car = dicDataCar[strCarNumber];
                }

                if (car == null)
                    continue;

                car.ID = nCarID;
                car.SiteID = nSiteID;
                car.SensorDetect = bDetectSensor;
                car.DBSensorDetect = bDetectSensor;

                AddCar(car);
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

                DataEquip equip = null;
                if (dicEquip.ContainsKey(strEquipCode))
                {
                    equip = dicEquip[strEquipCode];
                }
                else
                {
                    // ERP DB가 변경되었다. 변경점을 저장
                    // 삭제할 Equip 저장
                }

                if (equip == null)
                    continue;

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
                equip.EquipmentGroup = group;

                AddEquip(equip);
                //m_dicEquips[strEquipCode] = equip;
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

            int nSiteID = FormMain.Instance.SiteID;
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

            SqlConnection connect = m_DBConnection.Connect();

            //m_arDataZones.Clear();
            m_arrZoneGroup.Clear();

            int nSiteID = FormMain.Instance.SiteID;


            string szSQL = "Select ID, ZoneName, ZoneGroupName, Boundary, PermitLevel, TextCenter, Description from Zone where SiteID=" + nSiteID;

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
            int nSiteID = FormMain.Instance.SiteID;
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
                            FormMain.Instance.AlarmManager.IgnoreOptionCar = (AlarmManager.AlarmIgnoreOption)nOption;
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
                            FormMain.Instance.AlarmManager.IgnoreOptionEquip = (AlarmManager.AlarmIgnoreOption)nOption;
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
                            FormMain.Instance.AlarmManager.IgnoreOptionZone = (AlarmManager.AlarmIgnoreOption)nOption;
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
                            FormMain.Instance.AlarmManager.IgnoreDistanceCar = nDistance;
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
                            FormMain.Instance.AlarmManager.IgnoreDistanceEquip = nDistance;
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
                            FormMain.Instance.AlarmManager.IgnoreDistanceZone = nDistance;
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
                            FormMain.Instance.AlarmManager.IgnoreTimeCar = nTime;
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
                            FormMain.Instance.AlarmManager.IgnoreTimeEquip = nTime;
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
                            FormMain.Instance.AlarmManager.IgnoreTimeZone = nTime;
                        }
                    }
                }
                else if (string.Compare(strItemName, "CCTVCaptureFolder", true) == 0)
                {
                    string strCapturePath = strItemValue.Replace("[APP_START_PATH]", System.Windows.Forms.Application.StartupPath);

                    if (strCapturePath.Substring(strCapturePath.Length - 1) != @"\")
                        strCapturePath += @"\";

                    CCTVManager.Instance.CapturePath = strCapturePath;
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
                else if (string.Compare(strItemName, "SMSCaller", true) == 0)
                    m_strCaller = strItemValue.Trim();
                else if (string.Compare(strItemName, "SMSReceiver", true) == 0)
                    m_strReceiver = strItemValue.Trim();
            }

            rd.Close();
            connect.Close();
            return;
        }

        public ArrayList GetSensorIgnoreDatas()
        {
            return m_arrDetectIgnoreWorkers;
        }

        public ArrayList LoadSensorIgnoreDatas()
        {
            if (m_DBConnection == null)
                return null;

            SqlConnection connect = m_DBConnection.Connect();

            int nSiteID = FormMain.Instance.SiteID;

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

        private void LoadCCTV()
        {
            if (m_DBConnection == null)
                return;

            SqlConnection connect = m_DBConnection.Connect();
            int nSiteID = FormMain.Instance.SiteID;
            string szSQL = "SELECT ID, CameraName, IPAddr, ControlPort, VideoPort, AudioTransmitPort, AudioReceivePort, X, Y, Z, LOD, HTTPPort, Type, Stream, Channel, UserID";
            szSQL += ", Password, URL, ChipVersion, Description FROM CCTV ";

            SqlDataReader rd = m_DBConnection.ExecuteReader(szSQL, connect);
            if (rd == null)
                return;
            while (rd.Read())
            {
                int nID = Convert.ToInt32(rd[0]);
                string strCameraName = rd[1].ToString();
                string strIPAddr = rd[2].ToString();
                int nControlPort = Convert.ToInt32(rd[3]);
                int nVideoPort = Convert.ToInt32(rd[4]);
                int nAudioTransmitPort = Convert.ToInt32(rd[5]);
                int nAudioReceivePort = Convert.ToInt32(rd[6]);
                float fX = float.Parse(rd[7].ToString());
                float fY = float.Parse(rd[8].ToString());
                float fZ = float.Parse(rd[9].ToString());
                bool bIsInDoor = Convert.ToBoolean(rd[10]);
                int nHTTPPort = Convert.ToInt32(rd[11]);
                string strType = rd[12].ToString();
                int nStream = Convert.ToInt32(rd[13]);
                int nChannel = Convert.ToInt32(rd[14]);
                string strUserID = rd[15].ToString();
                string strPassword = rd[16].ToString();
                string strURL = rd[17].ToString();
                int nChipVersion = Convert.ToInt32(rd[18]);
                string strDescription = rd[19].ToString();

                CCTVViewer.CCTV cctv = new CCTVViewer.CCTV(
                        nID, strCameraName, strIPAddr, nControlPort, nVideoPort, nAudioTransmitPort, nAudioReceivePort,
                        fX, fY, fZ, bIsInDoor, nHTTPPort, strType, nStream, nChannel, strUserID, strPassword,
                        strURL, nChipVersion, strDescription);

                CCTVManager.Instance.AddCCTV(cctv);
            }

            CCTVManager.Instance.EndCCTVLoad();

            rd.Close();
            connect.Close();

            return;
        }

        private void LoadAP()
        {
            if (m_DBConnection == null)
                return;

            SqlConnection connect = m_DBConnection.Connect();
            string szSQL = "SELECT ID, AP_Name, X, Y, Z, Description FROM AP";

            SqlDataReader rd = m_DBConnection.ExecuteReader(szSQL, connect);
            while (rd.Read())
            {
                int nID = Convert.ToInt32(rd[0]);
                string strAPName = rd[1].ToString();
                float fX = float.Parse(rd[2].ToString());
                float fY = float.Parse(rd[3].ToString());
                float fZ = float.Parse(rd[4].ToString());
                string strDescription = rd[5].ToString();

                APData ap = new APData(nID, strAPName, fX, fY, fZ, strDescription);
                m_dicAP[nID] = ap;
            }

            rd.Close();
            connect.Close();

            return;
        }

        private void LoadGasSensor()
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

        private void ApplyCCTVData()
        {
            CCTVManager.Instance.ReConnectCCTV();
            PageBackstageHome.Instance.ContentView.CreateCCTVs();
        }

        private void ApplyAPData()
        {
            PageBackstageHome.Instance.ContentView.CreateAPs();
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

            int nSiteID = FormMain.Instance.SiteID;

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

        public DetectIgnoreWorker FindIgnoreWorker(int nWorkerID, int nIgnoreObjectID, int nIgnoreObjectType, int nSiteID)
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

    public partial class Crane3D : _3DEquipment
    {
        private Core.Crane m_crane = null;
        public Core.Crane Crane
        {
            get { return m_crane; }
            set { m_crane = value; }
        }

        public Crane3D(Core.Crane crane)
        {
            m_crane = crane;

            MinMovedY = new PrimitiveData<double>(-9.0);
            MaxMovedY = new PrimitiveData<double>(9.0);
        }

        public override void SetPosition(float x, float y, float z)
        {
            if (m_crane != null)
            {
                if (!m_crane.GetVisible())
                    m_crane.OnVisible(true);

                float fDistance = (float)Equipment.GetMovingDistance(x, y);

                if (MinMovedX != null && MaxMovedX != null)
                {
                    if (fDistance < MinMovedX.Data)
                        fDistance = (float)MinMovedX.Data;
                    else if (fDistance > MaxMovedX.Data)
                        fDistance = (float)MaxMovedX.Data;
                }

                if (MinMovedY != null && MaxMovedY != null)
                {
                    if (y < MinMovedY.Data)
                        y = (float)MinMovedY.Data;
                    else if (y > MaxMovedY.Data)
                        y = (float)MaxMovedY.Data;
                }

                m_crane.SetLocation(fDistance);
                m_crane.SetHookLocation(y);
                //m_crane.SetLocation(x);
                //m_crane.SetHookLocation(y);

                System.Diagnostics.Trace.WriteLine("Crane SetPosition : " + fDistance.ToString() + ", HookPosition : " + y.ToString());
            }
        }

        public override void Select()
        {
            if (m_crane != null)
            {
                m_crane.Select();
            }
        }

        public override void Unselect()
        {
            if (m_crane != null)
            {
                m_crane.ClearSelect();
            }
        }

        public override object GetLinkedObject()
        {
            return m_crane;
        }
    }

    public partial class MovingEquip3D : _3DEquipment
    {
        private Core.MovingEquipment m_movingEquip = null;
        public Core.MovingEquipment MovingEquipment
        {
            get { return m_movingEquip; }
            set { m_movingEquip = value; }
        }

        public MovingEquip3D(Core.MovingEquipment movingEquip)
        {
            m_movingEquip = movingEquip;
        }

        public override void SetPosition(float x, float y, float z)
        {
            if (m_movingEquip == null || Equipment == null)
                return;

            if (!m_movingEquip.GetVisible())
                m_movingEquip.OnVisible(true);

            float fDistance = (float)Equipment.GetMovingDistance(x, y);
            double distance = fDistance * Equipment.SensorDirVector.y;

            if (MinMovedY != null && MaxMovedY != null)
            {
                if (distance < MinMovedY.Data)
                    distance = MinMovedY.Data;
                else if (distance > MaxMovedY.Data)
                    distance = MaxMovedY.Data;

                if (Equipment.SensorDirVector.y != 0.0)
                    fDistance = (float)(distance / Equipment.SensorDirVector.y);
            }

            m_movingEquip.SetLocation(fDistance);
            System.Diagnostics.Trace.WriteLine("MovingEquip SetPosition : " + fDistance.ToString());
        }

        public override void Select()
        {
            if (m_movingEquip != null)
            {
                m_movingEquip.Select();
            }
        }

        public override void Unselect()
        {
            if (m_movingEquip != null)
            {
                m_movingEquip.ClearSelect();
            }
        }

        public override object GetLinkedObject()
        {
            return m_movingEquip;
        }
    }
}
