using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.SqlClient;

namespace HSMS
{
    public class EditIgnoreDetect : ChangedData
    {
        private log4net.ILog logger = null;


        /// <summary>
        /// 삭제할 DetectIgnoreWorker 데이터
        /// </summary>
        private ArrayList m_arDeleteIgnoreData = new ArrayList();

        /// <summary>
        /// 업데이트할 Ignore 필드 데이터
        /// </summary>
        private ArrayList m_arUpdateWorkerData = new ArrayList();
        private ArrayList m_arUpdateCarData = new ArrayList();
        private ArrayList m_arUpdateEquipData = new ArrayList();

        /// <summary>
        /// 새로 추가할 DetectIgnoreWorker 데이터
        /// </summary>
        private ArrayList m_arAddData = new ArrayList();

        /// <summary>
        /// 삭제할 데이터가 있는 지 여부
        /// </summary>
        private bool m_bIsDeleting = false;
        public bool IsDeleting
        {
            get { return m_bIsDeleting; }
            set { m_bIsDeleting = value; }
        }

        // Update시 Network으로 전송될 데이터를 저장할 공간
        protected ArrayList m_arrWorkerDatas = null;
        protected ArrayList m_arrVehicleDatas = null;
        protected ArrayList m_arrEquipDatas = null;

        public ArrayList WorkerDatas
        {
            get { return m_arrWorkerDatas; }
            set { m_arrWorkerDatas = value; }
        }

        public ArrayList CarDatas
        {
            get { return m_arrVehicleDatas; }
            set { m_arrVehicleDatas = value; }
        }

        public ArrayList EquipDatas
        {
            get { return m_arrEquipDatas; }
            set { m_arrEquipDatas = value; }
        }

        public EditIgnoreDetect()
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
        
        /// <summary>
        /// 삭제할 데이터 추가
        /// </summary>
        /// <param name="data">삭제할 데이터</param>
        public void AddDeleteIgnore(DetectIgnoreWorker data)
        {
            if( data == null)
                return;
            m_bIsDeleting = true;
            m_arDeleteIgnoreData.Add(data);
        }        

        /// <summary>
        /// DetectSensor필드가 변경된 업데이트할 Data Object
        /// </summary>
        /// <param name="data">DataCar or DataWorker or DataEquipment</param>
        public void AddUpdateData(object data)
        {
            if (data == null)
                return;

            if (data.GetType() == typeof(DataWorker))
            {
                DataWorker worker = (DataWorker)data;
                m_arUpdateWorkerData.Add(data);
            }
            else if (data.GetType() == typeof(DataCar))
            {
                DataCar car = (DataCar)data;
                m_arUpdateCarData.Add(data);
            }
            else if (data.GetType() == typeof(DataEquip))
            {
                DataEquip equip = (DataEquip)data;
                m_arUpdateEquipData.Add(data);
            }
        }

        /// <summary>
        /// 새로 추가되는 Ignore 데이터
        /// </summary>
        /// <param name="data"></param>
        public void AddIgnore(DetectIgnoreWorker data)
        {
            if( data == null)
                return;

            m_arAddData.Add(data);
        }

        
        private void CloseConnection(SqlConnection con)
        {
            try
            {
                if(con!= null)
                {
                    con.Close();
                }
            }
            catch (System.Exception)
            {                
            }
        }

        public override bool Update(DBConn conn)
        {
            if (m_arrDatas == null)
                return false;

            int nSiteID = FormMain.Instance.SiteID;

            if (m_bIsDeleting == true)
            {
                // Delete Data              
                foreach (DetectIgnoreWorker data in m_arDeleteIgnoreData)
                {
                    m_arrDatas.Add((int)ChangedData.DELETE);
                    m_arrDatas.Add(data.Worker.ID);
                    m_arrDatas.Add(data.IgnoreObjectID);
                    m_arrDatas.Add(data.IgnoreObjectType);
                    m_arrDatas.Add(nSiteID);                    
                }
            }
            
            // ADD DATA           
            //bFirst = true;            
            foreach (DetectIgnoreWorker data in m_arAddData)
            {
                if (FormMain.Instance.DataMgr.FindIgnoreWorker(data.WorkerID, data.IgnoreObjectID, data.IgnoreObjectType, data.SiteID) == null)
                {
                    m_arrDatas.Add((int)ChangedData.INSERT);
                    m_arrDatas.Add(data.Worker.ID);
                    m_arrDatas.Add(data.IgnoreObjectID);
                    m_arrDatas.Add(data.IgnoreObjectType);
                    m_arrDatas.Add(nSiteID);
                }
            }

            if (m_arrWorkerDatas != null)
            {
                foreach (DataWorker data in m_arUpdateWorkerData)
                {
                    m_arrWorkerDatas.Add((int)ChangedData.UPDATE);
                    m_arrWorkerDatas.Add(data.ID);
                    m_arrWorkerDatas.Add(data.MemberID);
                    m_arrWorkerDatas.Add(data.EnterLevel);
                    m_arrWorkerDatas.Add(nSiteID);

                    // Update 함수가 호출되었으므로 data.SensorDetect 값이 바뀌어야 한다.
                    // 따라서, 반대의 값으로 서버에 전송한다.
                    // 이 값을 즉시 바꾸지 않는 이유는 서버에 전송이 실패할 경우 서버와 클라이언트간 데이터 불일치가 생길수 있기 때문이다.
                    // [2014/6/20] 김지웅
                    m_arrWorkerDatas.Add(!data.SensorDetect);                    
                }
            }

            if (m_arrVehicleDatas != null)
            {
                foreach (DataCar data in m_arUpdateCarData)
                {
                    m_arrVehicleDatas.Add((int)ChangedData.UPDATE);
                    m_arrVehicleDatas.Add(data.ID);
                    m_arrVehicleDatas.Add(data.Number);
                    m_arrVehicleDatas.Add(nSiteID);

                    // Update 함수가 호출되었으므로 data.SensorDetect 값이 바뀌어야 한다.
                    // 따라서, 반대의 값으로 서버에 전송한다.
                    // 이 값을 즉시 바꾸지 않는 이유는 서버에 전송이 실패할 경우 서버와 클라이언트간 데이터 불일치가 생길수 있기 때문이다.
                    // [2014/6/20] 김지웅
                    m_arrVehicleDatas.Add(!data.SensorDetect);                    
                }
            }

            if (m_arrEquipDatas != null)
            {
                foreach (DataEquip data in m_arUpdateEquipData)
                {
                    m_arrEquipDatas.Add((int)ChangedData.UPDATE);
                    m_arrEquipDatas.Add(data.ID);
                    m_arrEquipDatas.Add(data.Code);
                    m_arrEquipDatas.Add(nSiteID);

                    // Update 함수가 호출되었으므로 data.SensorDetect 값이 바뀌어야 한다.
                    // 따라서, 반대의 값으로 서버에 전송한다.
                    // 이 값을 즉시 바꾸지 않는 이유는 서버에 전송이 실패할 경우 서버와 클라이언트간 데이터 불일치가 생길수 있기 때문이다.
                    // [2014/6/20] 김지웅
                    m_arrEquipDatas.Add(!data.SensorDetect);                    
                }
            }            
            return true;
        }

        /// <summary>
        /// 현재 데이터를 Manager에 더한다.
        /// </summary>
        /// <param name="mgr">IChangedDataManager</param>
        public override void AddToManager(IChangedDataManager mgr)
        {
            ArrayList arChangeData = mgr.GetDataList();
            foreach (object data in arChangeData)
            {
                if (data.GetType() == typeof(EditIgnoreDetect))
                {
                    EditIgnoreDetect other = (EditIgnoreDetect)data;
                    other.MergeData(this);
                    return;
                }
            }          
            mgr.SomethingChanged(this);
        }        

        /// <summary>
        /// 다른 EditIgnoreDetect와 모든 데이터를 합친다.
        /// </summary>
        /// <param name="rhs">다른 EditIgnoreDetect</param>
        protected void MergeData(EditIgnoreDetect rhs)
        {
            this.m_bIsDeleting = rhs.m_bIsDeleting;

            ArrayList arTemp = new ArrayList();

            // 원본에 없고 새로운 데이터에 있는 경우 더한다.
            foreach (DetectIgnoreWorker data in rhs.m_arDeleteIgnoreData)
            {
                string szValue = data.ToString();
                bool bExistData = false;
                foreach (DetectIgnoreWorker orgData in m_arDeleteIgnoreData)
                {
                    string szValue2 = orgData.ToString();
                    if (szValue == szValue2)
                    {
                        bExistData = true;
                        break;
                    }
                }
                if (bExistData == false)
                {
                    arTemp.Add(data);
                }               
            }

            // 원본에 있고 새로운 데이터에 있는 경우 더한다.
            foreach (DetectIgnoreWorker data in m_arDeleteIgnoreData)
            {
                string szValue = data.ToString();
                bool bExistData = false;
                foreach (DetectIgnoreWorker orgData in rhs.m_arDeleteIgnoreData)
                {
                    string szValue2 = orgData.ToString();
                    if (szValue == szValue2)
                    {
                        bExistData = true;
                        break;
                    }
                }

                if (bExistData == false)
                {
                    // 새로운 데이터에 없는 경우 없앤다.
                }
                else
                {
                    // 새 데이터에 있는 경우 더한다.
                    arTemp.Add(data);
                }
            }
            m_arDeleteIgnoreData.Clear();
            m_arDeleteIgnoreData.AddRange(arTemp);

            arTemp.Clear();
            // 원본에 없고 새로운 데이터에 있는 경우 더한다.
            foreach (DetectIgnoreWorker data in rhs.m_arAddData)
            {
                string szValue = data.ToString();
                bool bExistData = false;
                foreach (DetectIgnoreWorker orgData in m_arAddData)
                {
                    string szValue2 = orgData.ToString();
                    if (szValue == szValue2)
                    {
                        bExistData = true;
                        break;
                    }
                }

                if (bExistData == false)
                {
                    arTemp.Add(data);
                }
            }

            // 원본에 있고 새로운 데이터에 있는 경우 더한다.
            foreach (DetectIgnoreWorker data in m_arAddData)
            {
                string szValue = data.ToString();
                bool bExistData = false;
                foreach (DetectIgnoreWorker orgData in rhs.m_arAddData)
                {
                    string szValue2 = orgData.ToString();
                    if (szValue == szValue2)
                    {
                        bExistData = true;
                        break;
                    }
                }

                if (bExistData == false)
                {
                    // 새로운 데이터에 없는 경우 없앤다.
                }
                else
                {
                    // 새 데이터에 있는 경우 더한다.
                    arTemp.Add(data);
                }
            }
            m_arAddData.Clear();
            m_arAddData.AddRange(arTemp);

            // dictionary를 이용해 중복 데이터를 제거한다.
            Dictionary<int, DataCar> carUpdates = new Dictionary<int, DataCar>();
            Dictionary<int, DataWorker> workerUpdates = new Dictionary<int, DataWorker>();
            Dictionary<int, DataEquip> euqipUpdates = new Dictionary<int, DataEquip>();

            foreach (object data in rhs.m_arUpdateWorkerData)
            {
                if (data.GetType() == typeof(DataWorker))
                {
                    DataWorker worker = (DataWorker)data;
                    if (!workerUpdates.ContainsKey(worker.ID))
                        workerUpdates.Add(worker.ID, worker);
                }
            }

            foreach (object data in m_arUpdateWorkerData)
            {
                if (data.GetType() == typeof(DataWorker))
                {
                    DataWorker worker = (DataWorker)data;
                    if (!workerUpdates.ContainsKey(worker.ID))
                        workerUpdates.Add(worker.ID, worker);
                }
            }
            m_arUpdateWorkerData.Clear();
            m_arUpdateWorkerData.AddRange(workerUpdates.Values);


            foreach (object data in rhs.m_arUpdateCarData)
            {
                if (data.GetType() == typeof(DataCar))
                {
                    DataCar car = (DataCar)data;
                    if (!carUpdates.ContainsKey(car.ID))
                        carUpdates.Add(car.ID, car);
                }
            }

            foreach (object data in m_arUpdateCarData)
            {
                if (data.GetType() == typeof(DataCar))
                {
                    DataCar car = (DataCar)data;
                    if (!carUpdates.ContainsKey(car.ID))
                        carUpdates.Add(car.ID, car);
                }
            } 
            m_arUpdateCarData.Clear();
            m_arUpdateCarData.AddRange(carUpdates.Values);

            foreach (object data in rhs.m_arUpdateEquipData)
            {
                if (data.GetType() == typeof(DataEquip))
                {
                    DataEquip equip = (DataEquip)data;
                    if (!euqipUpdates.ContainsKey(equip.ID))
                        euqipUpdates.Add(equip.ID, equip);
                }
            }

            foreach (object data in m_arUpdateEquipData)
            {
                if (data.GetType() == typeof(DataEquip))
                {
                    DataEquip equip = (DataEquip)data;
                    if (!euqipUpdates.ContainsKey(equip.ID))
                        euqipUpdates.Add(equip.ID, equip);
                }
            }

            m_arUpdateEquipData.Clear();            
            m_arUpdateEquipData.AddRange(euqipUpdates.Values);
        }
    }
}
