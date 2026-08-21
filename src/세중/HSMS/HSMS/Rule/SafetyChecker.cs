using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnE.Geometry;
using System.Threading;

namespace HSMS
{
    public class SafetyChecker
    {
        public enum DangerType
        {
            NONE = 0,
            CAR_TO_WORKER,  // 차량이 작업자를 향해 접근
            WORKER_TO_CAR,  // 작업자가 차량을 향해 접근
            CAR_TO_WORKER_BOTH, // 차량과 작업자가 상호 접근
            WORKER_TO_EQUIP,    // 작업자가 위험설비를 향해 접근
            WORKER_TO_ZONE,     // 작업자가 위험존을 향해 접근
            CO_GAS_ALARM,       // CO Gas 누출
            METHANE_ALARM,      // 메탄가스 누출
            TYPE_COUNT
        };

        protected class SensorList
        {
            private object m_sensorOwner = null;
            private ArrayList m_arrList = new ArrayList();

            public object SensorOwner
            {
                get { return m_sensorOwner; }
                set { m_sensorOwner = value; }
            }

            public ArrayList List
            {
                get { return m_arrList; }
            }
        }

        // 이 값이 true이면 TimeLogCount 시간 만큼의 로그를 기록
        // 이 값이 false이면 MovingLogCount 개수 만큼의 로그를 기록
        private bool m_countTime = false;

        // 몇초 동안의 움직임 로그를 기록할 것인가?
        private int m_nTimeLogCount = 60;
        // 몇번의 움직임 로그를 기록할 것인가?
        private int m_nMovingLogCount = 60;

        // 이 값이 true이면 TimeLogCount 시간 만큼의 로그를 기록
        // 이 값이 false이면 MovingLogCount 개수 만큼의 로그를 기록
        public bool CountTime
        {
            get { return m_countTime; }
            set { m_countTime = value; }
        }

        // 몇초 동안의 움직임 로그를 기록할 것인가?
        public int TimeLogCount
        {
            get { return m_nTimeLogCount; }
            set { m_nTimeLogCount = value; }
        }

        // 몇번의 움직임 로그를 기록할 것인가?
        public int MovingLogCount
        {
            get { return m_nMovingLogCount; }
            set { m_nMovingLogCount = value; }
        }

        // SensorID, EventSensorData List
        private Dictionary<string, SensorList> m_dicSensorHistory = new Dictionary<string, SensorList>();
        private DataManager m_dataMgr = null;

        public SafetyChecker(DataManager dataMgr)
        {
            m_dataMgr = dataMgr;

            //Thread t = new Thread(new ThreadStart(CheckThread));
            //t.Start();
        }

        private DangerState FindState(ArrayList arrStates, DangerState state)
        {
            foreach (DangerState _state in arrStates)
            {
                if (_state.TargetCar == state.TargetCar &&
                    _state.TargetEquipment == state.TargetEquipment &&
                    _state.TargetZone == state.TargetZone &&
                    _state.Worker == state.Worker)
                    return _state;
            }

            return null;
        }

        /*private void CheckThread()
        {
            m_aliveThread = true;

            while (m_aliveThread)
            {
                ArrayList arrDangerStates = null;

                lock (this)
                {
                    arrDangerStates = CheckSafety();
                }

                // 알람 계산이 끝날때까지 화면이 갱신되지 않도록 한다.
                FormMain.Instance.LockMessage = true;

                AlarmManager alarmMgr = FormMain.Instance.AlarmManager;
                int nAlarmWorkerCount = alarmMgr.GetAlarmWorkerCount();

                for (int i = 0; i < nAlarmWorkerCount; i++)
                {
                    ArrayList arrStates = alarmMgr.GetAlarms(i);

                    if (arrStates == null)
                    {
                        alarmMgr.RemoveAlarmAt(i);
                        i--;
                        nAlarmWorkerCount--;
                        continue;
                    }

                    // 삭제된 알람을 찾아 제거한다.
                    ArrayList arrRemoveStates = new ArrayList();

                    foreach (DangerState state in arrStates)
                    {
                        if (FindState(arrDangerStates, state) == null)
                            arrRemoveStates.Add(state);
                    }

                    foreach (DangerState state in arrRemoveStates)
                    {
                        alarmMgr.RemoveAlarm(state.Worker, state);
                    }
                    ////////////////////////////////////////////////
                }

                if (arrDangerStates != null && arrDangerStates.Count > 0)
                {
                    foreach (DangerState state in arrDangerStates)
                    {
                        FormMain.Instance.AlarmManager.AddAlarm(state.Worker, state);
                    }
                }

                // 화면 갱신
                FormMain.Instance.RefreshMessage();

                Thread.Sleep(1000);
            }
        }*/

        public void ReleaseThread()
        {
            //m_aliveThread = false;
        }

        private void RemoveHistory(ArrayList arrHistories, int nHistoryCount, DateTime dtCurrent)
        {
            for (int i = 0; i < nHistoryCount; i++)
            {
                EventSensorData historyData = (EventSensorData)arrHistories[i];
                TimeSpan span = dtCurrent - historyData.EventTime;

                if ((int)span.TotalSeconds < TimeLogCount)
                {
                    for (int j = 0; j < i; j++)
                        arrHistories.RemoveAt(0);

                    break;
                }
            }
        }

        public ArrayList AddSensorHistory(string strSensorID, EventSensorData data)
        {
            SensorList arrHistories = null;

            if (m_dicSensorHistory.ContainsKey(strSensorID))
                arrHistories = m_dicSensorHistory[strSensorID];
            else
            {
                arrHistories = new SensorList();
                m_dicSensorHistory[strSensorID] = arrHistories;

                if (m_dataMgr != null)
                {
                    DataWorker worker = m_dataMgr.FindWorker2(strSensorID);

                    if (worker != null)
                        arrHistories.SensorOwner = worker;
                    else
                    {
                        DataCar car = m_dataMgr.FindCar2(strSensorID);

                        if (car != null)
                            arrHistories.SensorOwner = car;
                        else
                        {
                            DataEquip equip = m_dataMgr.FindEquip2(strSensorID);

                            if (equip != null)
                                arrHistories.SensorOwner = equip;
                        }
                    }
                }
            }

            if (arrHistories.List.Count == 0)
                arrHistories.List.Add(data);
            else
            {
                EventSensorData lastData = (EventSensorData)arrHistories.List[arrHistories.List.Count - 1];
                double dLen = System.Math.Sqrt((lastData.X - data.X) * (lastData.X - data.X) + (lastData.Y - data.Y) * (lastData.Y - data.Y));

                // 같은 좌표일 경우 List에 추가하지 않는다.
                if (dLen > UnE.Geometry.Math.HALF_TOLERANCE())
                    arrHistories.List.Add(data);
            }

            int nHistoryCount = arrHistories.List.Count;

            if (nHistoryCount > 1)
            {
                // 일정 시간이 경과한 로그는 삭제한다.
                if (CountTime)
                {
                    RemoveHistory(arrHistories.List, nHistoryCount - 1, data.EventTime);
                }
                else
                {
                    // 일정 회수를 초과한 로그는 삭제한다.
                    if (nHistoryCount > MovingLogCount)
                    {
                        int nRemoveCount = nHistoryCount - MovingLogCount;

                        for (int i = 0; i < nRemoveCount; i++)
                            arrHistories.List.RemoveAt(0);
                    }
                }
            }

            return arrHistories.List;
        }

        public ArrayList FindSensorHistory(string strSensorID)
        {
            if (m_dicSensorHistory.ContainsKey(strSensorID))
                return m_dicSensorHistory[strSensorID].List;

            return null;
        }

        public void RemoveSensorHistory(string strSensorID)
        {
            m_dicSensorHistory.Remove(strSensorID);
        }

        public void RemoveAllSensorHistory()
        {
            m_dicSensorHistory.Clear();
        }

        public int GetSensorHistoryCount()
        {
            return m_dicSensorHistory.Count;
        }

        public ArrayList GetSensorHistory(int nIndex)
        {
            if (nIndex < 0 || nIndex >= GetSensorHistoryCount())
                return null;

            KeyValuePair<string, SensorList> pair = m_dicSensorHistory.ElementAt(nIndex);
            return pair.Value.List;
        }

        // Return 값 : DangerState List
        //public ArrayList CheckSafety()
        //{
        //    if (CountTime)
        //    {
        //        DateTime dtNow = DateTime.Now;

        //        foreach (KeyValuePair<string, SensorList> pair in m_dicSensorHistory)
        //        {
        //            RemoveHistory(pair.Value.List, pair.Value.List.Count, dtNow);
        //        }
        //    }

        //    Type typeWorker = typeof(DataWorker);
        //    //Type typeEquip = typeof(DataEquip);
        //    //Type typeCar = typeof(DataCar);

        //    ArrayList arrDangerStates = new ArrayList();

        //    foreach (KeyValuePair<string, SensorList> pairWorker in m_dicSensorHistory)
        //    {
        //        if (pairWorker.Value.SensorOwner == null || pairWorker.Value.SensorOwner.GetType() != typeWorker)
        //            continue;

        //        /*DataWorker worker = (DataWorker)pairWorker.Value.SensorOwner;

        //        // 센서가 달려있는 설비와 차량을 검사한다.
        //        foreach (KeyValuePair<string, SensorList> pairObject in m_dicSensorHistory)
        //        {
        //            if (pairObject.Value.SensorOwner == null || pairObject.Value.SensorOwner == worker)
        //                continue;

        //            Type type = pairObject.Value.SensorOwner.GetType();
        //            DangerState state = null;

        //            if (type == typeCar)
        //                state = CheckStateWorkerToCar(pairWorker.Value, pairObject.Value);
        //            else if (type == typeEquip)
        //                state = CheckStateWorkerToEquip(pairWorker.Value, (DataEquip)pairObject.Value.SensorOwner);
        //            else
        //                continue;
        //        }*/

        //        int nEquipCount = m_dataMgr.GetEquipCount();

        //        for (int i = 0; i < nEquipCount; i++)
        //        {
        //            DataEquip equip = m_dataMgr.GetEquip(i);
        //            DangerState state = null;

        //            if (m_dicSensorHistory.ContainsKey(equip.Sensor))
        //            {
        //                //SensorList sensorList = m_dicSensorHistory[equip.Sensor];
        //                state = CheckStateWorkerToEquip(pairWorker.Value, equip);
        //            }

        //            if (state != null)
        //                arrDangerStates.Add(state);
        //        }

        //        int nCarCount = m_dataMgr.GetCarCount();

        //        for (int i = 0; i < nCarCount; i++)
        //        {
        //            DataCar car = m_dataMgr.GetCar(i);
        //            DangerState state = null;

        //            if (m_dicSensorHistory.ContainsKey(car.Sensor))
        //            {
        //                SensorList sensorList = m_dicSensorHistory[car.Sensor];
        //                state = CheckStateWorkerToCar(pairWorker.Value, sensorList);
        //            }

        //            if (state != null)
        //                arrDangerStates.Add(state);
        //        }

        //        foreach (DataZone zone in m_dataMgr.DataZones)
        //        {
        //            DangerState state = CheckStateWorkerToZone(pairWorker.Value, zone);

        //            if (state != null)
        //                arrDangerStates.Add(state);
        //        }
        //    }

        //    return arrDangerStates;
        //}

        //private DangerState CheckStateWorkerToZone(SensorList workerSensorList, DataZone zone)
        //{
        //    int nPermitLevelCount = zone.GetPermitLevelCount();

        //    if (nPermitLevelCount == 0)
        //        return null;
            
        //    bool permittedZone = false;
        //    DataWorker worker = (DataWorker)workerSensorList.SensorOwner;

        //    for (int i = 0; i < nPermitLevelCount; i++)
        //    {
        //        int nPermitLevel = zone.GetPermitLevel(i);

        //        if (nPermitLevel == 0)
        //        {
        //            permittedZone = false;
        //            break;
        //        }
        //        else if (worker.EnterLevel == nPermitLevel)
        //            permittedZone = true;
        //    }

        //    // worker에게 출입이 허가된 영역인가?
        //    if (permittedZone)
        //        return null;

        //    if (zone.Boundary == null)
        //        return null;

        //    int nWCount = workerSensorList.List.Count;
        //    if (nWCount == 0)
        //        return null;

        //    EventSensorData dataWorkerLast = (EventSensorData)workerSensorList.List[nWCount - 1];
        //    Vertex2D vWLast = new Vertex2D(dataWorkerLast.X, dataWorkerLast.Y);

        //    DangerState state = null;
        //    double dDistance = zone.Boundary.GetDistance(vWLast);

        //    if (dDistance <= 0.0)
        //    {
        //        state = new DangerState();

        //        state.Worker = worker;
        //        state.TargetZone = zone;
        //        state.Type = DangerType.WORKER_TO_ZONE;
        //        state.Distance = 0.0;
        //    }
        //    else if (dDistance <= m_dataMgr.WorkerToZoneDistance)
        //    {
        //        state = new DangerState();

        //        state.Worker = worker;
        //        state.TargetZone = zone;
        //        state.Type = DangerType.WORKER_TO_ZONE;
        //        state.Distance = dDistance;
        //    }

        //    return state;
        //}

        //private DangerState CheckStateWorkerToEquip(SensorList workerSensorList, DataEquip equip)
        //{
        //    if (equip.Boundary == null)
        //        return null;

        //    int nWCount = workerSensorList.List.Count;
        //    if (nWCount == 0)
        //        return null;

        //    EventSensorData dataWorkerLast = (EventSensorData)workerSensorList.List[nWCount - 1];
        //    Vertex2D vWLast = new Vertex2D(dataWorkerLast.X, dataWorkerLast.Y);

        //    // 설비의 움직임만큼 작업자 좌표도 위치 이동시킨다
        //    vWLast = vWLast - equip.OriginPosition - equip.Moved;

        //    DangerState state = null;
        //    double dDistance = equip.Boundary.GetDistance(vWLast);

        //    if (dDistance <= 0.0)
        //    {
        //        state = new DangerState();

        //        state.Worker = (DataWorker)workerSensorList.SensorOwner;
        //        state.TargetEquipment = equip;
        //        state.Type = DangerType.WORKER_TO_EQUIP;
        //        state.Distance = 0.0;
        //    }
        //    else
        //    {
        //        if (dDistance <= m_dataMgr.WorkerToEquipDistance)
        //        {
        //            state = new DangerState();

        //            state.Worker = (DataWorker)workerSensorList.SensorOwner;
        //            state.TargetEquipment = equip;
        //            state.Type = DangerType.WORKER_TO_EQUIP;
        //            state.Distance = dDistance;
        //        }
        //    }

        //    return state;
        //}

        //private DangerState CheckStateWorkerToCar(SensorList workerSensorList, SensorList carSensorList)
        //{
        //    int nWCount = workerSensorList.List.Count;
        //    int nCCount = carSensorList.List.Count;

        //    if (nWCount == 0 || nCCount == 0)
        //        return null;

        //    DangerType type = DangerType.NONE;

        //    EventSensorData dataWorkerLast = (EventSensorData)workerSensorList.List[nWCount - 1];
        //    EventSensorData dataCarLast = (EventSensorData)carSensorList.List[nCCount - 1];

        //    Vertex2D vWLast = new Vertex2D(dataWorkerLast.X, dataWorkerLast.Y);
        //    Vertex2D vCLast = new Vertex2D(dataCarLast.X, dataCarLast.Y);
        //    double dLen = -1.0;

        //    if (nWCount > 1 && nCCount > 1)
        //    {
        //        EventSensorData dataWorkerPrev = (EventSensorData)workerSensorList.List[nWCount - 2];
        //        EventSensorData dataCarPrev = (EventSensorData)carSensorList.List[nCCount - 2];

        //        Vertex2D vWPrev = new Vertex2D(dataWorkerPrev.X, dataWorkerPrev.Y);
        //        Vertex2D vCPrev = new Vertex2D(dataCarPrev.X, dataCarPrev.Y);

        //        type = CheckStateWorkerToCarDangerType(vWLast, vWPrev, vCLast, vCPrev, (DataCar)carSensorList.SensorOwner);
        //        dLen = GetDistanceWorkerToCar((DataCar)carSensorList.SensorOwner, vCLast, vCPrev, vWLast);
        //    }
        //    else if (nWCount > 1)
        //    {
        //        dLen = vWLast.GetDistance(vCLast);

        //        if (dLen < m_dataMgr.WorkerToCarDistanceOneSide)
        //            type = DangerType.WORKER_TO_CAR;
        //    }
        //    else if (nCCount > 1)
        //    {
        //        EventSensorData dataCarPrev = (EventSensorData)carSensorList.List[nCCount - 2];
        //        Vertex2D vCPrev = new Vertex2D(dataCarPrev.X, dataCarPrev.Y);

        //        dLen = GetDistanceWorkerToCar((DataCar)carSensorList.SensorOwner, vCLast, vCPrev, vWLast);
        //        //double dLen = vWLast.GetDistance(vCLast);

        //        if (dLen < m_dataMgr.WorkerToCarDistanceOneSide)
        //            type = DangerType.CAR_TO_WORKER;
        //    }

        //    if (type == DangerType.NONE)
        //        return null;

        //    DangerState state = new DangerState();

        //    state.Worker = (DataWorker)workerSensorList.SensorOwner;
        //    state.Distance = dLen >= 0.0 ? dLen : vWLast.GetDistance(vCLast);
        //    state.TargetCar = (DataCar)carSensorList.SensorOwner;
        //    state.Type = type;

        //    //System.Diagnostics.Trace.WriteLine("WorkerToCar Distance : " + state.Distance.ToString());

        //    return state;
        //}

        //private double GetDistanceWorkerToCar(DataCar car, Vertex2D vCCurrent, Vertex2D vCPrev, Vertex2D vWCurrent)
        //{
        //    int nCarWidth = car.Width / 1000;
        //    int nCarLength = car.Length / 1000;

        //    Vertex2D vTail = UnE.Geometry.Math.GetLinearVertex(vCCurrent, vCPrev, nCarLength);

        //    Vertex2D vHR = UnE.Geometry.Math.GetRightVertex(vCCurrent, vCPrev, nCarWidth / 2);
        //    Vertex2D vHL = vCCurrent * 2 - vHR;
        //    Vertex2D vTR = vTail - vCCurrent + vHR;
        //    Vertex2D vTL = vTail - vCCurrent + vHL;

        //    Polygon polygon = new Polygon();

        //    polygon.AddVertex(vHR);
        //    polygon.AddVertex(vHL);
        //    polygon.AddVertex(vTL);
        //    polygon.AddVertex(vTR);

        //    double dLen = polygon.GetDistance(vWCurrent);

        //    if (dLen <= 0.0)
        //        return 0.0;

        //    return dLen;
        //}

        //private DangerType CheckStateWorkerToCarDangerType(Vertex2D vWCurrent, Vertex2D vWPrev, Vertex2D vCCurrent, Vertex2D vCPrev, DataCar car)
        //{
        //    int nClosingType = GetClosingState(vWCurrent, vWPrev, vCCurrent, vCPrev);

        //    double dLen = GetDistanceWorkerToCar(car, vCCurrent, vCPrev, vWCurrent);
        //    //System.Diagnostics.Trace.WriteLine("WorkerToCar Distance : " + dLen.ToString() + ", Closing Type : " + nClosingType.ToString());

        //    if (dLen <= m_dataMgr.WorkerToCarDistanceOneSide)
        //    {
        //        if (nClosingType == 1)
        //            return DangerType.WORKER_TO_CAR;
        //        else if (nClosingType == 2)
        //            return DangerType.CAR_TO_WORKER;
        //        else if (nClosingType == 3)
        //            return DangerType.CAR_TO_WORKER_BOTH;
        //        else
        //            return DangerType.WORKER_TO_CAR;
        //    }
        //    else if (dLen <= m_dataMgr.WorkerToCarDistanceBoth)
        //    {
        //        if (nClosingType == 3)
        //            return DangerType.CAR_TO_WORKER_BOTH;
        //    }

        //    return DangerType.NONE;
        //}

        //// 두 객체가 서로 가까워지는 형태를 확인한다.
        //// Return 값 : 1(obj1이 obj2 쪽으로 다가가고 있다.)
        ////             2(obj2가 obj1 쪽으로 다가가고 있다.)
        ////             3(양쪽이 서로 가까워지고 있다.)
        ////             4(양쪽이 서로 멀어지고 있다.)
        ////             0(양쪽이 나란히 거리를 유지하며 이동하고 있다.)
        //private int GetClosingState(Vertex2D v1Current, Vertex2D v1Prev, Vertex2D v2Current, Vertex2D v2Prev)
        //{
        //    double dLen = v1Current.GetDistance(v2Current);

        //    Vertex2D v1Next = v1Current * 2 - v1Prev;
        //    Vertex2D v2Next = v2Current * 2 - v2Prev;

        //    Line2D line1 = new Line2D(v1Current, v1Next, Line2D.LineType.SEGMENT);
        //    Line2D line2 = new Line2D(v2Current, v2Next, Line2D.LineType.SEGMENT);

        //    Vertex2D vResult1, vResult2;
        //    Line2D.LineType resultLineType;

        //    int nResult = line1.IntersectLine(line2, out vResult1, out vResult2, out resultLineType);

        //    if (nResult == 0)
        //    {
        //        double dLen2 = v1Next.GetDistance(v2Next);

        //        if (dLen == dLen2)
        //            return 0;
        //        else if (dLen < dLen2)
        //            return 4;
        //        else// if (dLen > dLen2)
        //        {
        //            double dLen11 = v1Current.GetDistance(v2Next);
        //            double dLen22 = v2Current.GetDistance(v1Next);

        //            if (dLen2 < dLen11)
        //            {
        //                if (dLen2 < dLen22)
        //                {
        //                    // 양쪽이 서로 다가서는 경우
        //                    return 3;
        //                }
        //                else
        //                {
        //                    // obj1이 obj2쪽으로 다가가고 있다.
        //                    return 1;
        //                }
        //            }
        //            else if (dLen2 < dLen22)
        //            {
        //                // obj2가 obj1쪽으로 다가가고 있다.
        //                return 2;
        //            }
        //        }
        //    }
        //    else if (nResult == 1)
        //    {
        //        double dLen2 = v1Next.GetDistance(v2Next);

        //        double dLen11 = v1Current.GetDistance(v2Next);
        //        double dLen22 = v2Current.GetDistance(v1Next);

        //        if (dLen2 < dLen11)
        //        {
        //            if (dLen2 < dLen22)
        //            {
        //                // 양쪽이 서로 다가서는 경우
        //                return 3;
        //            }
        //            else
        //            {
        //                // obj1이 obj2쪽으로 다가가고 있다.
        //                return 1;
        //            }
        //        }
        //        else if (dLen2 < dLen22)
        //        {
        //            // obj2가 obj1쪽으로 다가가고 있다.
        //            return 2;
        //        }
        //    }
        //    else if (nResult == 2)
        //    {
        //        Vertex2D vector1 = v1Next - v1Current;
        //        Vertex2D vector2 = v2Next - v2Current;

        //        if (UnE.Geometry.Math.GetAngle(vector1, new Vertex2D(0, 0), vector2) < UnE.Geometry.Math.HALF_TOLERANCE())
        //        {
        //            if (line1.IsInclude(v2Current))
        //            {
        //                // obj1이 obj2쪽으로 다가가고 있다.
        //                return 1;
        //            }
        //            else if (line2.IsInclude(v1Current))
        //            {
        //                // obj2가 obj1쪽으로 다가가고 있다.
        //                return 2;
        //            }
        //        }

        //        // 양쪽이 서로 다가서는 경우
        //        return 3;
        //    }

        //    return 0;
        //}
    }

    public class DangerState : Object
    {
        private int m_nAlarmHistoryID = -1;
        private int m_nAlarmProcessHistoryID = -1;
        private SafetyChecker.DangerType m_type = SafetyChecker.DangerType.NONE;
        private double m_dDistance = 0.0;
        private DataEquip m_equipTarget = null;
        private DataCar m_carTarget = null;
        private DataZone m_zoneTarget = null;
        private DataWorker m_worker = null;
        private DateTime m_time = new DateTime();

        private bool m_bCritical = false;
        public bool Critical
        {
            get { return m_bCritical; }
            set { m_bCritical = value; }
        }

        public int AlarmHistoryID
        {
            get { return m_nAlarmHistoryID; }
            set { m_nAlarmHistoryID = value; }
        }

        // AlarmProcessHistory ID
        public int AlarmProcessHistoryID
        {
            get { return m_nAlarmProcessHistoryID; }
            set { m_nAlarmProcessHistoryID = value; }
        }

        public SafetyChecker.DangerType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public double Distance
        {
            get { return m_dDistance; }
            set { m_dDistance = value; }
        }

        public DataWorker Worker
        {
            get { return m_worker; }
            set { m_worker = value; }
        }

        public DataEquip TargetEquipment
        {
            get { return m_equipTarget; }
            set { m_equipTarget = value; }
        }

        public DataCar TargetCar
        {
            get { return m_carTarget; }
            set { m_carTarget = value; }
        }

        public DataZone TargetZone
        {
            get { return m_zoneTarget; }
            set { m_zoneTarget = value; }
        }

        public DateTime EventTime
        {
            get { return m_time; }
            set { m_time = value; }
        }

        public override string ToString()
        {
            if (m_worker == null)
            {
                if (m_type == SafetyChecker.DangerType.CO_GAS_ALARM)
                    return "일산화탄소 누출";
                else if (m_type == SafetyChecker.DangerType.METHANE_ALARM)
                    return "메탄가스 누출";

                return "";
            }

            string szTeamName = "";
            if (m_worker.Team != null)
                szTeamName = m_worker.Team.Name;

            return m_worker.Company.CompanyName + " " + szTeamName + " " + m_worker.Name;
        }
    }
}
