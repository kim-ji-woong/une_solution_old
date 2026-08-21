using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Data.SqlClient;

using HSMS;

namespace HSMSServer2
{   
    
    public class ERPDataChecker : IChangedDataChecker
    {
        private static ERPDataChecker m_Instance = null;
        public static ERPDataChecker Instance
        {
            get 
            {
                if (m_Instance == null)
                    m_Instance = new ERPDataChecker();
                return m_Instance; 
            }
        }

        private bool m_bReleaseThread = false;
        public bool ReleaseThread
        {
            get { return m_bReleaseThread; }
            set { m_bReleaseThread = value; }
        }

        private ConcurrentBag<IChangedData> m_arChangedData = new ConcurrentBag<IChangedData>();
        
        public ERPDataChecker()
        {
            ProxyHSMS.Checker = this;
            m_bReleaseThread = false;
        }

        public int GetChangedCount()
        {
            return m_arChangedData.Count;
        }

        public void AddChangedData(IChangedData data)
        {
            m_arChangedData.Add(data);
        }


        private void ProcessEquipData(IEnumerable<IChangedData> arDatas)
        {        
            DBConn dbMgr = NetworkServer.Instance.DBManager;
            if (dbMgr == null)
                return;
            foreach (IChangedData data in arDatas)
            {
                DataEquip equip = (DataEquip)data;                
            }
        }

        private void ProcessCarData(IEnumerable<IChangedData> arDatas)
        {                
            DBConn dbMgr = NetworkServer.Instance.DBManager;
            if (dbMgr == null)
                return;
            foreach (IChangedData data in arDatas)
            {
                DataCar car = (DataCar)data;

                AlarmManager alarmMgr = NetworkServer.Instance.AlarmManager;
                alarmMgr.RemoveAlarm(car, null);

                DBCarHelper.RemoveCar(dbMgr, car);
           
                ArrayList arrDatas = new ArrayList();
                arrDatas.Add((int)ChangeDataType.CAR);
                arrDatas.Add(EditData.REMOVE);  // delete 는 2번
                arrDatas.Add(car.ID);
                arrDatas.Add(car.Code);

                ServiceProvider provider = NetworkServer.Instance.ServiceProvider;
                byte[] bytes = ServiceProvider.MakeBytes(TCP_ID.CHANGE_DB_DATA, arrDatas);
                provider.SendClientData(bytes, ClientData.ClientType.HSMS_CLIENT, true);
            }
        }

        private void ProcessWorkerData(IEnumerable<IChangedData> arDatas)
        {
            DBConn dbMgr = NetworkServer.Instance.DBManager;
            if (dbMgr == null)
                return;
            foreach (IChangedData data in arDatas)
            {
                DataWorker worker = (DataWorker)data;

                // 현재 진행중인 Alarm에서 삭제한다.
                AlarmManager alarmMgr = NetworkServer.Instance.AlarmManager;
                alarmMgr.RemoveAlarm(worker, null);

                if( DBWorkerHelper.RemoveWorker(dbMgr, worker))
                {
                    ArrayList arrDatas = new ArrayList();
                    arrDatas.Add((int)ChangeDataType.WORKER);
                    arrDatas.Add(EditData.REMOVE);  // delete 는 2번
                    arrDatas.Add(worker.ID);
                    arrDatas.Add(worker.MemberID);

                    DataManager dbManger = NetworkServer.Instance.DataManager;
                    Manager mgr = dbManger.GetManager(worker.MemberID);
                    if (mgr != null)
                    {
                        dbManger.RemoveManager(mgr);
                    }

                    ServiceProvider provider = NetworkServer.Instance.ServiceProvider;
                    byte[] bytes = ServiceProvider.MakeBytes(TCP_ID.CHANGE_DB_DATA, arrDatas);
                    provider.SendClientData(bytes, ClientData.ClientType.HSMS_CLIENT, true);     
                }                          
            }
        }

        protected void ProcessChangedData()
        {
            if (m_arChangedData.IsEmpty == true)
                return;
            
            var deleteWorkers = from data in m_arChangedData
                                where data.GetChangedDataType() == ChangeDataType.WORKER
                                select data;

            if (deleteWorkers != null && deleteWorkers.Count<IChangedData>() > 0)
            {
                ProcessWorkerData(deleteWorkers);
            }


            var deleteCars = from dataCar in m_arChangedData
                                where dataCar.GetChangedDataType() == ChangeDataType.CAR
                                select dataCar;
            if (deleteCars != null && deleteCars.Count<IChangedData>() > 0)
            {
                ProcessCarData(deleteCars);
            }

            var deleteEquip = from dataEquip in m_arChangedData
                              where dataEquip.GetChangedDataType() == ChangeDataType.EQUIP
                              select dataEquip;

            if (deleteEquip != null && deleteEquip.Count<IChangedData>() > 0)
            {
                ProcessEquipData(deleteEquip);
            }



            m_arChangedData = new ConcurrentBag<IChangedData>(); 
        } 

        public void BeginCheck()
        {
            DataManager dataMgr = NetworkServer.Instance.DataManager;
            
            dataMgr.ReadDBData();

            DateTime dt = DateTime.Now;
            string szValue = RegistryUtil.ReadRegValue("Server Info", "LastDBAccess");
            if (szValue == null || szValue == "")
            {
                RegistryUtil.WriteRegValue("Server Info", "LastDBAccess", dt.ToString());
            }
            else
            {
                dt = Convert.ToDateTime(szValue);
            }
            ProxyHSMS.LastDBAccess = dt;       
            RunThread();
        }

        private void RunThread()
        {
            Thread t = new Thread(CheckErpDB);
            t.Start(this);
        }

        private void CheckErpDB(object param)
        {
            ERPDataChecker checker = (ERPDataChecker)param;
            
            while (!checker.m_bReleaseThread)
            {
                DateTime dtTime = DateTime.Now;
                if (dtTime.Hour >= 23 && dtTime.Hour < 24)
                {
                    try
                    {
                        // 센서 데이터 수신을 DB 로딩동안 중지한다.
                        ClientProvider.DatabaseReloading = true;

                        // 현재 처리중인 센서 데이터 처리를 기다린다.
                        Thread.Sleep(1000);

                        // ERP와 HSMS 데이터를 다시 읽는다. 
                        // 다시 읽는중에 ERP에 없어진 데이터가 있는경우 Proxy를 통해 AddChangedData가 호출된다.
                        DataManager dataMgr = NetworkServer.Instance.DataManager;
                        dataMgr.ReloadDBData();

                        // 변경된 데이터가 있으면 DB에 시간을 기록하고 삭제 처리한다.
                        if (GetChangedCount() > 0)
                        {
                            ProxyHSMS.LastDBAccess = dtTime;
                            // DB 갱신 시간을 기록해둔다.
                            RegistryUtil.WriteRegValue("Server Info", "LastDBAccess", dtTime.ToString());
                            
                            // 변경될 데이터를 삭제 처리하고 클라이언트에 알린다.
                            checker.ProcessChangedData();
                        }

                        AlarmManager alarmMgr = NetworkServer.Instance.AlarmManager;
                        if (alarmMgr!= null)
                        {
                            alarmMgr.Reload();
                        }

                        ClientProvider.DatabaseReloading = false;
                    }
                    catch (System.Exception)
                    {
                        ClientProvider.DatabaseReloading = false;
                        checker.m_bReleaseThread = true;
                    }
                }

                for (int i = 0; i < 3600; i++)
                {
                    Thread.Sleep(500);
                    if (checker.m_bReleaseThread == true)
                        break;
                }
                try
                {
                    GC.Collect();
                }
                catch(Exception)
                { }
            }
        }        
    } 
}
