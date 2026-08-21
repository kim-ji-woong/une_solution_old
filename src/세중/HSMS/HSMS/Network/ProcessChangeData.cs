using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HSMS
{
    public class ProcessChangeData
    {
        // HSMS 데이터가 업데이트 되는 경우
        public const int UPDATE = 1;
        // HSMS 데이터가 삭제 되는 경우
        public const int DELETE = 2;
        // HSMS 데이터가 추가 되는 경우
        public const int INSERT = 3;
        // ERP 데이터가 변경되어 데이터가 삭제되는 경우
        public const int REMOVE = 4;

        public static void ProcessChangeZoneGroup(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount < 4)
                return;

            DataManager dataMgr = FormMain.Instance.DataMgr;

            for (int i=2;i<nDataCount-1;i+=2)
            {
                int nZoneID = (int)arrDatas[i];
                string strGroupName = (string)arrDatas[i + 1];

                ZoneGroup group = dataMgr.FindZoneGroup(strGroupName);

                if (group == null)
                {
                    if (strGroupName == ZoneGroup.DefaultZoneGroup.GroupName ||
                        strGroupName == ZoneGroup.DefaultZoneGroup.ToString())
                        group = ZoneGroup.DefaultZoneGroup;
                    else
                    {
                        group = new ZoneGroup(strGroupName);
                        dataMgr.AddZoneGroup(group);
                    }
                }

                DataZone zone = dataMgr.FindZone(nZoneID);

                if (zone != null)
                    zone.ZoneGroup = group;
            }
        }

        public static void ProcessChangeZone(ArrayList arrDatas)
        {
            int nChangeType = (int)arrDatas[1];

            DataManager dataMgr = FormMain.Instance.DataMgr;

            // EnterLevel Update
            if (nChangeType == ProcessChangeData.UPDATE)
            {
                int nZoneID = (int)arrDatas[2];
                string szPermitLevel = (string)arrDatas[3];

                int nZoneGroupCount = dataMgr.GetZoneGroupCount();

                for (int i = 0; i < nZoneGroupCount; i++)
                {
                    ZoneGroup group = dataMgr.GetZoneGroup(i);
                    int nZoneCount = group.GetZoneCount();

                    for (int j = 0; j < nZoneCount; j++)
                    //ArrayList arZone = dataMgr.DataZones;
                    //foreach (DataZone zone in arZone)
                    {
                        DataZone zone = group.GetZone(j);

                        if (zone.ID == nZoneID)
                        {
                            zone.RemoveAllPermitLevels();
                            if (szPermitLevel != null && szPermitLevel != "")
                            {
                                string[] permits = szPermitLevel.Split(',');
                                for (int k = 0; k < permits.Length; k++)
                                {
                                    int nLevel = 0;
                                    if (int.TryParse(permits[k], out nLevel))
                                    {
                                        zone.AddPermitLevel(nLevel);
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }

        public static void ProcessChangeWorker(ArrayList arrDatas)
        {
            int nChangeType = (int)arrDatas[1];

            DataManager dataMgr = FormMain.Instance.DataMgr;

            // EnterLevel Update
            if (nChangeType == ProcessChangeData.UPDATE)
            {
                int nTargetWorkerID = (int)arrDatas[2];
                int nChangeLevel = (int)arrDatas[3];
                DataWorker worker = dataMgr.GetWorkerFromID(nTargetWorkerID);
                if (worker != null)
                {
                    if (worker.DBEnterLevel != nChangeLevel)
                    {
                        worker.EnterLevel = nChangeLevel;
                        worker.DBEnterLevel = nChangeLevel;
                    }
                }
            }
            // Delete Worker
            else if (nChangeType == ProcessChangeData.DELETE)
            {
                int nTargetWorkerID = (int)arrDatas[2];
                DataWorker worker = dataMgr.GetWorkerFromID(nTargetWorkerID);
                if (worker != null)
                {
                    // DataWorker를 삭제
                    dataMgr.RemoveWorker(worker);

                    // SensorWorker를 삭제
                    PageBackstageHome.Instance.Invoke((MethodInvoker)delegate
                    {
                        FormContent contetnView = PageBackstageHome.Instance.ContentView;
                        contetnView.RemoveWorker(worker);
                    });
                    // 재사용을 위해 ID초기화
                    worker.ID = -1;
                    worker.EnterLevel = -1;
                    worker.SensorDetect = true;
                    worker.SensorWorker = null;
                }
            }
            else if (nChangeType == ProcessChangeData.REMOVE)
            {
                int nTargetWorkerID = (int)arrDatas[2];
                DataWorker worker = dataMgr.GetWorkerFromID(nTargetWorkerID);
                if (worker != null)
                {
                    // DataWorker를 삭제
                    dataMgr.RemoveWorker(worker);

                    // SensorWorker를 삭제
                    PageBackstageHome.Instance.Invoke((MethodInvoker)delegate
                    {
                        FormContent contetnView = PageBackstageHome.Instance.ContentView;
                        contetnView.RemoveWorker(worker);
                    });

                    // 해당 아이디의 Manager를 삭제한다.
                    Manager mgr = dataMgr.GetManager(worker.MemberID);
                    if (mgr != null)
                    {
                        dataMgr.RemoveManager(mgr);
                    }

                    // ERP데이터에서 삭제한다.
                    ERPManager.Instance.DicCompanyWorkers.Remove(worker.MemberID);                    
                }
            }
            // Add Worker
            else if (nChangeType == ProcessChangeData.INSERT)
            {
                int nWorkerID = (int)arrDatas[2];
                string szMemberID = (string)arrDatas[3];
                int nEnterLevel = (int)arrDatas[4];
                int nSiteID = FormMain.Instance.SiteID;
                bool bIgnore = (bool)arrDatas[6];
                Dictionary<string, DataWorker> dicWorkers = ERPManager.Instance.DicCompanyWorkers;
                if (dicWorkers.ContainsKey(szMemberID))
                {
                    DataWorker worker = dicWorkers[szMemberID];
                    if (worker != null)
                    {
                        worker.ID = nWorkerID;
                        worker.SiteID = nSiteID;
                        worker.SensorDetect = bIgnore;
                        worker.EnterLevel = nEnterLevel;
                        dataMgr.AddWorker(worker);

                        // SensorWorker를 새로 생성
                        PageBackstageHome.Instance.Invoke((MethodInvoker)delegate
                        {
                            FormContent contetnView = PageBackstageHome.Instance.ContentView;
                            contetnView.AddWorker(worker);
                            if (worker.SensorWorker != null)
                            {
                                worker.SensorWorker.OnVisible(false);
                            }

                        });
                    }
                }
            }
        }

        /// <summary>
        /// 서버로 부터 전송받은 데이터로 차량을 수정/삭제/추가하는 함수
        /// </summary>
        /// <param name="arrDatas">추가/삭제/업데이트용 데이터</param>
        public static void ProcessChangeCar(ArrayList arrDatas)
        {
            int nChangeType = (int)arrDatas[1];

            DataManager dataMgr = FormMain.Instance.DataMgr;

            // Update
            if (nChangeType == ProcessChangeData.UPDATE)
            {
            }
            // Delete Car
            else if (nChangeType == ProcessChangeData.DELETE)
            {
                int nTargetCarID = (int)arrDatas[2];
                string szCarCode = (string)arrDatas[3];
                DataCar car = dataMgr.FindCar(szCarCode);
                if (car != null)
                {
                    dataMgr.RemoveCar(car);

                    PageBackstageHome.Instance.Invoke((MethodInvoker)delegate
                    {
                        FormContent contetnView = PageBackstageHome.Instance.ContentView;
                        contetnView.RemoveVehicle(car);
                    });

                    car.ID = -1;
                    car.SensorDetect = true;
                }
            }
            // delete ERP data
            else if( nChangeType == ProcessChangeData.REMOVE)
            {
                int nTargetCarID = (int)arrDatas[2];
                string szCarCode = (string)arrDatas[3];
                DataCar car = dataMgr.FindCar(szCarCode);
                //DataCar car = dataMgr.GetCarFromID(nTargetCarID);                
                if (car != null)
                {
                    dataMgr.RemoveCar(car);

                    PageBackstageHome.Instance.Invoke((MethodInvoker)delegate
                    {
                        FormContent contetnView = PageBackstageHome.Instance.ContentView;
                        contetnView.RemoveVehicle(car);
                    });

                    car.ID = -1;
                    car.SensorDetect = true;
                    ERPManager.Instance.DicCompanyCars.Remove(car.Code);
                }
            }
            // Add Car
            else if (nChangeType == ProcessChangeData.INSERT)
            {
                int nID = (int)arrDatas[2];
                string szCarNum = (string)arrDatas[3];
                int nSiteID = (int)arrDatas[4];
                bool bIgnore = (bool)arrDatas[5];

                Dictionary<string, DataCar> dicCars = ERPManager.Instance.DicCompanyCars;
                if (dicCars.ContainsKey(szCarNum))
                {
                    DataCar car = dicCars[szCarNum];
                    if (car != null)
                    {
                        car.ID = nID;
                        car.SiteID = FormMain.Instance.SiteID;
                        car.SensorDetect = bIgnore;
                        dataMgr.AddCar(car);

                        PageBackstageHome.Instance.Invoke((MethodInvoker)delegate
                        {
                            FormContent contetnView = PageBackstageHome.Instance.ContentView;
                            contetnView.AddVehicle(car);
                            if (car.SensorVehicle != null)
                            {
                                car.SensorVehicle.OnVisible(false);
                            }
                        });
                    }
                }
            }
        }

        /*public static void ProcessChangeEquip(ArrayList arrDatas)
        {
            int nChangeType = (int)arrDatas[1];

            DataManager dataMgr = FormMain.Instance.DataMgr;

            // Delete Equip
            if (nChangeType == ProcessChangeData.DELETE)
            {
                int nTargetEquip = (int)arrDatas[2];
                DataEquip equip = dataMgr.GetEquipFromID(nTargetEquip);
                if (equip != null)
                {
                    dataMgr.RemoveEquip(equip);

                    equip.SensorDetect = true;
                    equip.ID = -1;
                    equip.Boundary = null;
                    equip.SensorPosition = null;
                    equip.SensorFinishPosition = null;
                    equip.SensorDirVector = null;
                    equip.OriginPosition = null;
                }
            }
            else if (nChangeType == ProcessChangeData.INSERT)
            {
                int nID = (int)arrDatas[2];
                string szEquipName = (string)arrDatas[3];
                string szBoundary = (string)arrDatas[4];
                string szSensorPos = (string)arrDatas[5];
                string szSensorFinishPos = (string)arrDatas[6];
                string szTextCenter = (string)arrDatas[7];
                int nSiteID = FormMain.Instance.SiteID;

                Dictionary<string, DataEquip> dicEquip = ERPManager.Instance.DicEquips;
                if (dicEquip.ContainsKey(szEquipName))
                {
                    DataEquip equip = dicEquip[szEquipName];
                    if (equip != null)
                    {
                        equip.ID = nID;
                        equip.SiteID = nSiteID;

                        UnE.Geometry.Polygon polygon = dataMgr.GetPolygon(szBoundary);
                        if (polygon != null)
                        {
                            UnE.Geometry.Vertex2D vEquipOrigin = dataMgr.ResetPolygonCoords(polygon);
                            UnE.Geometry.Vertex2D vSensorPos = dataMgr.GetVertex(szSensorPos);
                            UnE.Geometry.Vertex2D vSensorFinishPos = dataMgr.GetVertex(szSensorFinishPos);
                            equip.Boundary = polygon;

                            if (vSensorPos != null)
                                equip.SensorPosition = vSensorPos;

                            if (vSensorFinishPos != null)
                                equip.SensorFinishPosition = vSensorFinishPos;

                            equip.Boundary = polygon;
                            equip.OriginPosition = vEquipOrigin;
                        }
                        dataMgr.AddEquip(equip);
                    }
                }
            }
        }*/

        public static void ProcessChangeSMSConfige(ArrayList arrDatas)
        {
            int nChangeType = (int)arrDatas[1];
            bool bChecked = (bool)arrDatas[2];
            
            DataManager dataMgr = FormMain.Instance.DataMgr;
            
            if (bChecked == true)
            {
                dataMgr.MessageChecked = true;
            }
            else
            {
                dataMgr.MessageChecked = false;
            }
        }

        public static void ProcessChangeIgnreToWorkerList(ArrayList arrDatas)
        {
            DataManager dataMgr = FormMain.Instance.DataMgr;
            for(int i = 1 ; i < arrDatas.Count ; i += 5)
            {
                int nChangeType = (int)arrDatas[i];
                int nWorkerID = (int)arrDatas[i+1];
                int nIgnoreObjectID = (int)arrDatas[i+2];
                int nObjectType = (int)arrDatas[i+3];
                int nSiteID = (int)arrDatas[i+4];

                if (nChangeType == ProcessChangeData.DELETE)
                {
                    foreach (HSMS.DetectIgnoreWorker data in dataMgr.DetectIgnoreWorkers)
                    {
                        if (data.WorkerID == nWorkerID && data.IgnoreObjectID == nIgnoreObjectID &&
                            data.IgnoreObjectType == nObjectType && data.SiteID == nSiteID)
                        {
                            dataMgr.DetectIgnoreWorkers.Remove(data);
                            break;
                        }
                    }
                }
                else if (nChangeType == ProcessChangeData.INSERT)
                {
                    HSMS.DataWorker worker = dataMgr.GetWorkerFromID(nWorkerID);
                    if (worker != null)
                    {
                        HSMS.DetectIgnoreWorker data = new HSMS.DetectIgnoreWorker();

                        data.Worker = worker;
                        data.WorkerID = nWorkerID;
                        data.IgnoreObjectID = nIgnoreObjectID;
                        data.IgnoreObjectType = nObjectType;
                        data.SiteID = nSiteID;

                        dataMgr.DetectIgnoreWorkers.Add(data);
                    }
                }                
            }            
        }

        public static void ProcessChnageManager(ArrayList arrDatas)
        {
            int nChangeType = (int)arrDatas[1];

            DataManager dataMgr = FormMain.Instance.DataMgr;

            if (nChangeType == ProcessChangeData.UPDATE)
            {
            }

            else if (nChangeType == ProcessChangeData.DELETE)
            {
                string szMemberID = (string)arrDatas[2];
                int nSiteID = (int)arrDatas[3];

                Manager mgr = dataMgr.GetManager(szMemberID);
                if (mgr != null)
                {
                    dataMgr.RemoveManager(mgr);                    
                }
            }

            else if (nChangeType == ProcessChangeData.INSERT)
            {
                int nID = (int)arrDatas[2];
                string szMemberID = (string)arrDatas[3];
                int nSiteID = (int)arrDatas[4];

                Manager mgr = dataMgr.GetManager(szMemberID);
                if (mgr == null)
                {

                    Dictionary<string, DataWorker> workers = ERPManager.Instance.DicCompanyWorkers;
                    if (workers.ContainsKey(szMemberID))
                    {
                        DataWorker worker = workers[szMemberID];
                        if (worker != null)
                        {
                            Manager newMgr = new Manager();
                            newMgr.ID = nID;
                            newMgr.MemberID = szMemberID;
                            newMgr.SiteID = nSiteID;
                            newMgr.Worker = worker;
                            dataMgr.AddManager(newMgr);    
                        }                       
                    }                                       
                }
            }
        }


        public static void ProcessAlarmIgnoreOptions(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            for (int i = 1; i < nDataCount; i += 4)
            {
                ProcessAlarmIgnoreOption(arrDatas, i);
            }
        }

        private static bool ProcessAlarmIgnoreOption(ArrayList arrDatas, int nIndex)
        {
            int nDataCount = arrDatas.Count;

            if (nIndex + 4 > nDataCount)
                return false;

            try
            {
                int nType = (int)arrDatas[nIndex];
                int nOption = (int)arrDatas[nIndex + 1];
                int nDistance = (int)arrDatas[nIndex + 2];
                int nTime = (int)arrDatas[nIndex + 3];

                if (nOption < (int)AlarmManager.AlarmIgnoreOption.NONE || nOption >= (int)AlarmManager.AlarmIgnoreOption.TYPE_COUNT)
                    return false;

                AlarmManager alarmMgr = FormMain.Instance.AlarmManager;

                if (nType == (int)ClientProvider.ObjectType.VEHICLE)
                {
                    alarmMgr.IgnoreOptionCar = (AlarmManager.AlarmIgnoreOption)nOption;
                    alarmMgr.IgnoreDistanceCar = nDistance;
                    alarmMgr.IgnoreTimeCar = nTime;
                }
                else if (nType == (int)ClientProvider.ObjectType.EQUIPMENT)
                {
                    alarmMgr.IgnoreOptionEquip = (AlarmManager.AlarmIgnoreOption)nOption;
                    alarmMgr.IgnoreDistanceEquip = nDistance;
                    alarmMgr.IgnoreTimeEquip = nTime;
                }
                else if (nType == (int)ClientProvider.ObjectType.ZONE)
                {
                    alarmMgr.IgnoreOptionZone = (AlarmManager.AlarmIgnoreOption)nOption;
                    alarmMgr.IgnoreDistanceZone = nDistance;
                    alarmMgr.IgnoreTimeZone = nTime;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
