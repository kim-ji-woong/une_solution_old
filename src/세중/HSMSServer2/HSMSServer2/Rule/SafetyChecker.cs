using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnE.Geometry;
using System.Threading;
using HSMS;
using System.Data.SqlClient;

namespace HSMSServer2
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
            //private ArrayList m_arrList = new ArrayList();
            // 삽입과 삭제가 빈번하게 일어나므로 최적화를 위하여 LinkedList로 교체한다.
            // [2014/07/09] 김지웅
            private LinkedList<EventSensorData> m_arrList = new LinkedList<EventSensorData>();

            public object SensorOwner
            {
                get { return m_sensorOwner; }
                set { m_sensorOwner = value; }
            }

            public LinkedList<EventSensorData> List
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

        // 몇분 동안 신호가 발생하지 않는 센서는 무시할 것인가?
        // 이 값이 0보다 작으면 신호가 발생하지 않는 센서를 무시하지 않는다.
        private int m_nIgnoreSensorMinute = -1;
        private static int m_nDefIgnoreSensorMinute = -1;

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

        // 몇분 동안 신호가 발생하지 않는 센서는 무시할 것인가?
        // 이 값이 0보다 작으면 신호가 발생하지 않는 센서를 무시하지 않는다.
        public int IgnoreSensorMinute
        {
            get { return m_nIgnoreSensorMinute; }
            set { m_nIgnoreSensorMinute = value; }
        }

        public static int DefIgnoreSensorMinute
        {
            get { return m_nDefIgnoreSensorMinute; }
            set { m_nDefIgnoreSensorMinute = value; }
        }

        // SensorID, EventSensorData List
        private Dictionary<string, SensorList> m_dicSensorHistory = new Dictionary<string, SensorList>();
        private DataManager m_dataMgr = null;
        private bool m_aliveThread = false;

        private static SafetyChecker m_instance = null;
        public static SafetyChecker Instance
        {
            get { return m_instance; }
        }

        public SafetyChecker(DataManager dataMgr)
        {
            m_instance = this;
            m_dataMgr = dataMgr;

            m_nIgnoreSensorMinute = m_nDefIgnoreSensorMinute;

            // 이전에 발생했던 Sensor History들을 DB로부터 읽어온다.
            ReadDBSensorHistory();

            Thread t = new Thread(new ThreadStart(CheckThread));
            t.Start();
        }

        public void GetLastSensorDatas(ArrayList arrSensorDatas)
        {
            lock (this)
            {
                NetworkClient.ObjectType type = NetworkClient.ObjectType.NONE;
                int nObjectID = 0;

                Type typeWorker = typeof(DataWorker);
                Type typeCar = typeof(DataCar);
                Type typeEquip = typeof(DataEquip);

                foreach (KeyValuePair<string, SensorList> pair in m_dicSensorHistory)
                {
                    if (pair.Value.SensorOwner != null && pair.Value.List.Count > 0)
                    {
                        Type sensorType = pair.Value.SensorOwner.GetType();

                        if (sensorType == typeWorker)
                        {
                            DataWorker worker = (DataWorker)pair.Value.SensorOwner;
                            nObjectID = worker.ID;
                            type = NetworkClient.ObjectType.WORKER;
                        }
                        else if (sensorType == typeCar)
                        {
                            DataCar car = (DataCar)pair.Value.SensorOwner;
                            nObjectID = car.ID;
                            type = NetworkClient.ObjectType.VEHICLE;
                        }
                        else if (sensorType == typeEquip)
                        {
                            DataEquip equip = (DataEquip)pair.Value.SensorOwner;
                            nObjectID = equip.ID;
                            type = NetworkClient.ObjectType.EQUIPMENT;
                        }
                        else
                            continue;

                        EventSensorData data = pair.Value.List.Last.Value;

                        arrSensorDatas.Add((int)type);
                        arrSensorDatas.Add(nObjectID);
                        arrSensorDatas.Add(pair.Key);
                        arrSensorDatas.Add(data.X);
                        arrSensorDatas.Add(data.Y);
                    }
                }
            }
        }

        // 이전에 발생했던 Sensor History들을 DB로부터 읽어온다.
        private void ReadDBSensorHistory()
        {
            DBConn conn = NetworkServer.Instance.DBManager;
            SqlConnection connection = conn.Connect();

            string strSQL = "Select SensorID, Max(Time) from SensorHistory group by SensorID";
            SqlDataReader reader = conn.ExecuteReader(strSQL, connection);

            ArrayList arrSensors = new ArrayList();
            DateTime dtCurrent = DateTime.Now;

            while (reader.Read())
            {
                string strSensorID = (string)reader[0];
                DateTime dtLast = (DateTime)reader[1];

                TimeSpan span = dtCurrent - dtLast;

                if (m_nIgnoreSensorMinute < 0)
                    arrSensors.Add(strSensorID);
                else if (span.TotalMinutes < m_nIgnoreSensorMinute)
                    arrSensors.Add(strSensorID);
            }

            reader.Close();

            foreach (string strSensorID in arrSensors)
            {
                strSQL = MakeReadDBSensorHistoryQuery(dtCurrent, strSensorID);
                reader = conn.ExecuteReader(strSQL, connection);

                while (reader.Read())
                {
                    DateTime dtEvent = (DateTime)reader[0];
                    double x = (double)reader[1];
                    double y = (double)reader[2];

                    SensorList sensorList = null;

                    if (m_dicSensorHistory.ContainsKey(strSensorID))
                        sensorList = m_dicSensorHistory[strSensorID];
                    else
                    {
                        sensorList = new SensorList();
                        sensorList.SensorOwner = m_dataMgr.FindSensorOwner(strSensorID);
                        m_dicSensorHistory[strSensorID] = sensorList;
                    }

                    EventSensorData data = new EventSensorData(strSensorID, dtEvent, x, y);
                    sensorList.List.AddFirst(data);
                }

                reader.Close();
            }

            connection.Close();
        }

        private string MakeReadDBSensorHistoryQuery(DateTime dtCurrent, string strSensorID)
        {
            string strFormat = "";
            DateTime dt;

            if (CountTime)
            {
                if (m_nIgnoreSensorMinute < 0)
                    dt = dtCurrent.AddSeconds(-m_nTimeLogCount);
                else
                {
                    DateTime dt1 = dtCurrent.AddSeconds(-m_nTimeLogCount);
                    DateTime dt2 = dtCurrent.AddMinutes(-m_nIgnoreSensorMinute);
                    dt = dt1 > dt2 ? dt1 : dt2;
                }

                strFormat = "Select Time, X, Y from SensorHistory where SensorID = '{0}' ";
                strFormat += "and Time Between '{1}-{2}-{3} {4}:{5}:{6}' and getdate() order by Time desc";
            }
            else
            {
                dt = dtCurrent.AddMinutes(-m_nIgnoreSensorMinute);

                strFormat = "Select Top " + m_nMovingLogCount.ToString() + " Time, X, Y from SensorHistory where SensorID = '{0}' ";
                strFormat += "and Time Between '{1}-{2}-{3} {4}:{5}:{6}' and getdate() order by Time desc";
            }

            string strSQL = string.Format(strFormat, strSensorID, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
            return strSQL;
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

        private void CheckThread()
        {
            m_aliveThread = true;

            while (m_aliveThread && NetworkServer.Instance.ServiceProvider == null)
            {
                Thread.Sleep(1000);
            }

            while (m_aliveThread)
            {
                ArrayList arrDangerStates = null;
                //ArrayList arrBytes = new ArrayList();
                ArrayList arrBytes = null;

                lock (this)
                {
                    CheckIgnoreSensors();
                    arrDangerStates = CheckSafety();
                }

                AlarmManager alarmMgr = NetworkServer.Instance.AlarmManager;
                int nAlarmWorkerCount = alarmMgr.GetAlarmWorkerCount();

                for (int i = 0; i < nAlarmWorkerCount; i++)
                {
                    ArrayList arrStates = alarmMgr.GetAlarms(i);

                    if (arrStates == null)
                    {
                        alarmMgr.RemoveAlarmAt(i, arrBytes);
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
                        alarmMgr.RemoveAlarm(state.Worker, state, arrBytes);
                    }
                    ////////////////////////////////////////////////
                }

                if (arrDangerStates != null && arrDangerStates.Count > 0)
                {
                    foreach (DangerState state in arrDangerStates)
                    {
                        NetworkServer.Instance.AlarmManager.AddAlarm(state.Worker, state, arrBytes);
                    }
                }

                //SendAlarms(arrBytes);

                Thread.Sleep(1000);
            }
        }

        private int GetTotalBytesLength(ArrayList arrBytes)
        {
            int nBytesLength = 0;

            foreach (byte[] bytes in arrBytes)
            {
                nBytesLength += bytes.Length + 4;
            }

            /*// 첫번째 데이터는 TcpLib2에서 길이 바이트를 추가하므로 따로 4바이트를 붙이지 않는다.
            nBytesLength -= 4;*/
            return nBytesLength;
        }

        // 전체 Bytes를 묶어서 하나의 패킷으로 만들어 보낸다.
        private void SendAlarms(ArrayList arrBytes)
        {
            int nBytesLength = GetTotalBytesLength(arrBytes);

            if (nBytesLength <= 0)
                return;

            byte[] bytes = new byte[nBytesLength];

            int nIndex = 0;
            int nDataCount = arrBytes.Count;

            for (int i = 0; i < nDataCount; i++)
            {
                byte[] data = (byte[])arrBytes[i];

                byte[] bytesLength = BitConverter.GetBytes(data.Length);
                System.Buffer.BlockCopy(bytesLength, 0, bytes, nIndex, 4);
                nIndex += 4;

                System.Buffer.BlockCopy(data, 0, bytes, nIndex, data.Length);
                nIndex += data.Length;
            }

            NetworkServer.Instance.ServiceProvider.SendClientData_NoLengthBytes(bytes, ClientData.ClientType.HSMS_CLIENT, false);
            arrBytes.Clear();
        }

        public void ReleaseThread()
        {
            m_aliveThread = false;
        }

        private void RemoveHistory(LinkedList<EventSensorData> arrHistories, int nHistoryCount, DateTime dtCurrent)
        {
            LinkedListNode<EventSensorData> node = arrHistories.First;

            for (int i = 0; i < nHistoryCount; i++)
            {
                EventSensorData historyData = node.Value;
                TimeSpan span = dtCurrent - historyData.EventTime;

                if ((int)span.TotalSeconds < TimeLogCount)
                {
                    for (int j = 0; j < i; j++)
                        arrHistories.RemoveFirst();

                    break;
                }

                node = node.Next;
            }
        }

        public LinkedList<EventSensorData> AddSensorHistory(string strSensorID, EventSensorData data)
        {
            SensorList arrHistories = null;

            if (m_dicSensorHistory.ContainsKey(strSensorID))
            {
                arrHistories = m_dicSensorHistory[strSensorID];
            }
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
                arrHistories.List.AddLast(data);
            else
            {
               // EventSensorData lastData = arrHistories.List.Last.Value;
                //EventSensorData lastData = (EventSensorData)arrHistories.List[nHistoryCount - 1];
               // double dLen = System.Math.Sqrt((lastData.X - data.X) * (lastData.X - data.X) + (lastData.Y - data.Y) * (lastData.Y - data.Y));

                // 같은 좌표일 경우 List에 추가하지 않는다.
                //if (dLen > UnE.Geometry.Math.HALF_TOLERANCE())
                    arrHistories.List.AddLast(data);
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
                            arrHistories.List.RemoveFirst();
                    }
                }
            }

            return arrHistories.List;
        }

        public LinkedList<EventSensorData> FindSensorHistory(string strSensorID)
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

        public LinkedList<EventSensorData> GetSensorHistory(int nIndex)
        {
            if (nIndex < 0 || nIndex >= GetSensorHistoryCount())
                return null;

            KeyValuePair<string, SensorList> pair = m_dicSensorHistory.ElementAt(nIndex);
            return pair.Value.List;
        }

        // 일정시간동안 신호를 발생시키지 않은 센서는 없앤다.
        private void CheckIgnoreSensors()
        {
            if (m_nIgnoreSensorMinute < 0)
                return;

            ArrayList arrRemove = new ArrayList();
            DateTime dtNow = DateTime.Now;

            foreach (KeyValuePair<string, SensorList> pair in m_dicSensorHistory)
            {
                if (pair.Value.List.Count == 0)
                    arrRemove.Add(pair.Key);

                EventSensorData data = pair.Value.List.Last.Value;
                TimeSpan span = dtNow - data.EventTime;

                if (span.TotalMinutes >= m_nIgnoreSensorMinute)
                    arrRemove.Add(pair.Key);
            }

            if (arrRemove.Count == 0)
                return;

            foreach (string strSensor in arrRemove)
            {
                m_dicSensorHistory.Remove(strSensor);
            }

            byte[] bytes = ServiceProvider.MakeBytes(TCP_ID.REMOVE_SENSORS, arrRemove);
            NetworkServer.Instance.ServiceProvider.SendClientData(bytes, ClientData.ClientType.HSMS_CLIENT, false);
        }

        // Return 값 : DangerState List
        public ArrayList CheckSafety()
        {
            if (CountTime)
            {
                DateTime dtNow = DateTime.Now;

                foreach (KeyValuePair<string, SensorList> pair in m_dicSensorHistory)
                {
                    RemoveHistory(pair.Value.List, pair.Value.List.Count, dtNow);
                }
            }

            Type typeWorker = typeof(DataWorker);
            //Type typeEquip = typeof(DataEquip);
            //Type typeCar = typeof(DataCar);

            ArrayList arrDangerStates = new ArrayList();

            foreach (KeyValuePair<string, SensorList> pairWorker in m_dicSensorHistory)
            {
                if (pairWorker.Value.SensorOwner == null || pairWorker.Value.SensorOwner.GetType() != typeWorker)
                    continue;

                /*DataWorker worker = (DataWorker)pairWorker.Value.SensorOwner;

                // 센서가 달려있는 설비와 차량을 검사한다.
                foreach (KeyValuePair<string, SensorList> pairObject in m_dicSensorHistory)
                {
                    if (pairObject.Value.SensorOwner == null || pairObject.Value.SensorOwner == worker)
                        continue;

                    Type type = pairObject.Value.SensorOwner.GetType();
                    DangerState state = null;

                    if (type == typeCar)
                        state = CheckStateWorkerToCar(pairWorker.Value, pairObject.Value);
                    else if (type == typeEquip)
                        state = CheckStateWorkerToEquip(pairWorker.Value, (DataEquip)pairObject.Value.SensorOwner);
                    else
                        continue;
                }*/

                int nEquipCount = m_dataMgr.GetEquipCount();

                for (int i = 0; i < nEquipCount; i++)
                {
                    DataEquip equip = m_dataMgr.GetEquip(i);
                    DangerState state = null;

                    if (m_dicSensorHistory.ContainsKey(equip.Sensor))
                    {
                        SensorList sensorList = m_dicSensorHistory[equip.Sensor];

                        if (sensorList.SensorOwner == null)
                            sensorList.SensorOwner = equip;

                        state = CheckStateWorkerToEquip(pairWorker.Value, sensorList);
                    }

                    if (state != null)
                        arrDangerStates.Add(state);
                }

                int nCarCount = m_dataMgr.GetCarCount();

                for (int i = 0; i < nCarCount; i++)
                {
                    DataCar car = m_dataMgr.GetCar(i);
                    DangerState state = null;

                    if (m_dicSensorHistory.ContainsKey(car.Sensor))
                    {
                        SensorList sensorList = m_dicSensorHistory[car.Sensor];

                        if (sensorList.SensorOwner == null)
                            sensorList.SensorOwner = car;

                        state = CheckStateWorkerToCar(pairWorker.Value, sensorList);
                    }

                    if (state != null)
                        arrDangerStates.Add(state);
                }

                int nZoneGroupCount = m_dataMgr.GetZoneGroupCount();

                for (int i = 0; i < nZoneGroupCount;i++ )
                //foreach (DataZone zone in m_dataMgr.DataZones)
                {
                    ZoneGroup group = m_dataMgr.GetZoneGroup(i);

                    int nZoneCount = group.GetZoneCount();

                    for (int j = 0; j < nZoneCount; j++)
                    {
                        DataZone zone = group.GetZone(j);
                        DangerState state = CheckStateWorkerToZone(pairWorker.Value, zone);

                        if (state != null)
                            arrDangerStates.Add(state);
                    }
                }
            }

            return arrDangerStates;
        }

        private DangerState CheckStateWorkerToZone(SensorList workerSensorList, DataZone zone)
        {
            DataWorker worker = (DataWorker)workerSensorList.SensorOwner;

            if (!worker.SensorDetect)
                return null;

            int nPermitLevelCount = zone.GetPermitLevelCount();

            if (nPermitLevelCount == 0)
            {
                RemoveIgnoreAlarmZone(worker, zone);
                return null;
            }
            
            bool permittedZone = false;

            for (int i = 0; i < nPermitLevelCount; i++)
            {
                int nPermitLevel = zone.GetPermitLevel(i);

                if (nPermitLevel == 0)
                {
                    permittedZone = false;
                    break;
                }
                else if (worker.EnterLevel == nPermitLevel)
                    permittedZone = true;
            }

            // worker에게 출입이 허가된 영역인가?
            if (permittedZone)
            {
                RemoveIgnoreAlarmZone(worker, zone);
                return null;
            }

            if (zone.Boundary == null)
                return null;

            int nWCount = workerSensorList.List.Count;
            if (nWCount == 0)
            {
                RemoveIgnoreAlarmZone(worker, zone);
                return null;
            }

            EventSensorData dataWorkerLast = workerSensorList.List.Last.Value;
            //EventSensorData dataWorkerLast = (EventSensorData)workerSensorList.List[nWCount - 1];
            Vertex2D vWLast = new Vertex2D(dataWorkerLast.X, dataWorkerLast.Y);

            DangerState state = null;
            double dDistance = zone.Boundary.GetDistance(vWLast);

            float fWorkerToZoneDistance;
            if (!m_dataMgr.GetWorkerToZoneDistance(zone.ZoneGroup.GroupName, out fWorkerToZoneDistance))
            {
                m_dataMgr.GetWorkerToZoneDistance(ZoneGroup.DefaultZoneGroup.GroupName, out fWorkerToZoneDistance);
            }

            if (dDistance <= 0.0)
            {
                state = new DangerState();

                state.Worker = worker;
                state.TargetZone = zone;
                state.Type = DangerType.WORKER_TO_ZONE;
                state.Distance = 0.0;
                state.EventTime = dataWorkerLast.EventTime;
            }
            else if (dDistance <= fWorkerToZoneDistance)
            {
                state = new DangerState();

                state.Worker = worker;
                state.TargetZone = zone;
                state.Type = DangerType.WORKER_TO_ZONE;
                state.Distance = dDistance;
                state.EventTime = dataWorkerLast.EventTime;
            }
            else
            {
                AlarmManager.IgnoreAlarmInfo info = NetworkServer.Instance.AlarmManager.GetIgnoreAlarm(worker, "", zone);

                if (info != null)
                {
                    // 알람 무시조건을 확인후 조건이 충족되면 무시 목록에서 삭제한다.
                    NetworkServer.Instance.AlarmManager.CheckNRemoveIgnoreAlarm(info, dDistance, DateTime.Now, null, null, zone);
                }
            }

            return state;
        }

        private void RemoveIgnoreAlarmZone(DataWorker worker, DataZone zone)
        {
            AlarmManager.IgnoreAlarmInfo info = NetworkServer.Instance.AlarmManager.GetIgnoreAlarm(worker, "", zone);

            if (info != null)
                NetworkServer.Instance.AlarmManager.RemoveIgnoreAlarm(info);
        }

        private DangerState CheckStateWorkerToEquip(SensorList workerSensorList, SensorList equipSensorList)
        {
            if (equipSensorList == null || equipSensorList.SensorOwner == null)
                return null;

            int nECount = equipSensorList.List.Count;
            if (nECount == 0)
                return null;

            DataEquip equip = (DataEquip)equipSensorList.SensorOwner;

            if (equip.Boundary == null)
                return null;

            int nWCount = workerSensorList.List.Count;
            if (nWCount == 0)
                return null;

            DataWorker worker = (DataWorker)workerSensorList.SensorOwner;

            if (!worker.SensorDetect || !equip.SensorDetect)
                return null;

            if (NetworkServer.Instance.DataManager.FindIgnoreWorker(worker.ID, equip.ID, 2, worker.SiteID) != null)
                return null;

            EventSensorData dataEquipLast = equipSensorList.List.Last.Value;
            EventSensorData dataWorkerLast = workerSensorList.List.Last.Value;
            //EventSensorData dataWorkerLast = (EventSensorData)workerSensorList.List[nWCount - 1];
            Vertex2D vWLast = new Vertex2D(dataWorkerLast.X, dataWorkerLast.Y);
            Vertex2D vELast = new Vertex2D(dataEquipLast.X, dataEquipLast.Y);

            // 설비의 움직임만큼 작업자 좌표도 위치 이동시킨다
            //vWLast = vWLast - equip.OriginPosition - equip.Moved;

            DangerState state = null;
            //double dDistance = equip.Boundary.GetDistance(vWLast);
            double dDistance = equip.GetDistance(vWLast, vELast);

            float fWorkerToEquipDistance;
            if (!m_dataMgr.GetWorkerToEquipDistance(equip.EquipmentGroup.GroupName, out fWorkerToEquipDistance))
            {
                m_dataMgr.GetWorkerToEquipDistance(EquipmentGroup.DefaultEquipmentGroup.GroupName, out fWorkerToEquipDistance);
            }

            if (dDistance <= 0.0)
            {
                state = new DangerState();

                state.Worker = worker;
                state.TargetEquipment = equip;
                state.Type = DangerType.WORKER_TO_EQUIP;
                state.Distance = 0.0;
                state.EventTime = dataWorkerLast.EventTime;
            }
            else
            {
                if (dDistance <= fWorkerToEquipDistance)
                {
                    state = new DangerState();

                    state.Worker = worker;
                    state.TargetEquipment = equip;
                    state.Type = DangerType.WORKER_TO_EQUIP;
                    state.Distance = dDistance;
                    state.EventTime = dataWorkerLast.EventTime;
                }
            }

            if (state == null)
            {
                AlarmManager.IgnoreAlarmInfo info = NetworkServer.Instance.AlarmManager.GetIgnoreAlarm((DataWorker)workerSensorList.SensorOwner, equip.Sensor, null);

                if (info != null)
                {
                    // 알람 무시조건을 확인후 조건이 충족되면 무시 목록에서 삭제한다.
                    NetworkServer.Instance.AlarmManager.CheckNRemoveIgnoreAlarm(info, dDistance, DateTime.Now, null, equip, null);
                }
            }

            return state;
        }

        private DangerState CheckStateWorkerToCar(SensorList workerSensorList, SensorList carSensorList)
        {
            int nWCount = workerSensorList.List.Count;
            int nCCount = carSensorList.List.Count;

            if (nWCount == 0 || nCCount == 0)
                return null;

            DataWorker worker = (DataWorker)workerSensorList.SensorOwner;
            DataCar car = (DataCar)carSensorList.SensorOwner;

            if (!worker.SensorDetect || !car.SensorDetect)
                return null;

            if (NetworkServer.Instance.DataManager.FindIgnoreWorker(worker.ID, car.ID, 1, worker.SiteID) != null)
                return null;

            DangerType type = DangerType.NONE;

            EventSensorData dataWorkerLast = workerSensorList.List.Last.Value;
            EventSensorData dataCarLast = carSensorList.List.Last.Value;

            Vertex2D vWLast = new Vertex2D(dataWorkerLast.X, dataWorkerLast.Y);
            Vertex2D vCLast = new Vertex2D(dataCarLast.X, dataCarLast.Y);
            double dLen = -1.0;

            if (nWCount > 1 && nCCount > 1)
            {
                EventSensorData dataWorkerPrev = workerSensorList.List.Last.Previous.Value;
                EventSensorData dataCarPrev = carSensorList.List.Last.Previous.Value;

                Vertex2D vWPrev = new Vertex2D(dataWorkerPrev.X, dataWorkerPrev.Y);
                Vertex2D vCPrev = new Vertex2D(dataCarPrev.X, dataCarPrev.Y);

                type = CheckStateWorkerToCarDangerType(vWLast, vWPrev, vCLast, vCPrev, car);
                dLen = GetDistanceWorkerToCar(car, vCLast, vCPrev, vWLast);
            }
            else if (nWCount > 1)
            {
                dLen = vWLast.GetDistance(vCLast);

                if (dLen < m_dataMgr.WorkerToCarDistanceOneSide)
                    type = DangerType.WORKER_TO_CAR;
            }
            else if (nCCount > 1)
            {
                EventSensorData dataCarPrev = carSensorList.List.Last.Previous.Value;
                Vertex2D vCPrev = new Vertex2D(dataCarPrev.X, dataCarPrev.Y);

                dLen = GetDistanceWorkerToCar((DataCar)carSensorList.SensorOwner, vCLast, vCPrev, vWLast);
                //double dLen = vWLast.GetDistance(vCLast);

                if (dLen < m_dataMgr.WorkerToCarDistanceOneSide)
                    type = DangerType.CAR_TO_WORKER;
            }

            if (type == DangerType.NONE)
            {
                AlarmManager.IgnoreAlarmInfo info = NetworkServer.Instance.AlarmManager.GetIgnoreAlarm((DataWorker)workerSensorList.SensorOwner, car.Sensor, null);

                if (info != null)
                {
                    // 알람 무시조건을 확인후 조건이 충족되면 무시 목록에서 삭제한다.
                    NetworkServer.Instance.AlarmManager.CheckNRemoveIgnoreAlarm(info, dLen, DateTime.Now, car, null, null);
                }

                return null;
            }

            DangerState state = new DangerState();

            state.Worker = worker;
            state.Distance = dLen >= 0.0 ? dLen : vWLast.GetDistance(vCLast);
            state.TargetCar = (DataCar)carSensorList.SensorOwner;
            state.Type = type;
            state.EventTime = dataWorkerLast.EventTime;

            //System.Diagnostics.Trace.WriteLine("WorkerToCar Distance : " + state.Distance.ToString());

            return state;
        }

        private double GetDistanceWorkerToCar(DataCar car, Vertex2D vCCurrent, Vertex2D vCPrev, Vertex2D vWCurrent)
        {
            int nCarWidth = car.Width / 1000;
            int nCarLength = car.Length / 1000;

            Vertex2D vTail = UnE.Geometry.Math.GetLinearVertex(vCCurrent, vCPrev, nCarLength);

            Vertex2D vHR = UnE.Geometry.Math.GetRightVertex(vCCurrent, vCPrev, nCarWidth / 2);
            Vertex2D vHL = vCCurrent * 2 - vHR;
            Vertex2D vTR = vTail - vCCurrent + vHR;
            Vertex2D vTL = vTail - vCCurrent + vHL;

            Polygon polygon = new Polygon();

            polygon.AddVertex(vHR);
            polygon.AddVertex(vHL);
            polygon.AddVertex(vTL);
            polygon.AddVertex(vTR);

            double dLen = polygon.GetDistance(vWCurrent);

            if (dLen <= 0.0)
                return 0.0;

            return dLen;
        }

        private DangerType CheckStateWorkerToCarDangerType(Vertex2D vWCurrent, Vertex2D vWPrev, Vertex2D vCCurrent, Vertex2D vCPrev, DataCar car)
        {
            int nClosingType = GetClosingState(vWCurrent, vWPrev, vCCurrent, vCPrev);

            double dLen = GetDistanceWorkerToCar(car, vCCurrent, vCPrev, vWCurrent);
            //System.Diagnostics.Trace.WriteLine("WorkerToCar Distance : " + dLen.ToString() + ", Closing Type : " + nClosingType.ToString());

            if (dLen <= m_dataMgr.WorkerToCarDistanceOneSide)
            {
                if (nClosingType == 1)
                    return DangerType.WORKER_TO_CAR;
                else if (nClosingType == 2)
                    return DangerType.CAR_TO_WORKER;
                else if (nClosingType == 3)
                    return DangerType.CAR_TO_WORKER_BOTH;
                else
                    return DangerType.WORKER_TO_CAR;
            }
            else if (dLen <= m_dataMgr.WorkerToCarDistanceBoth)
            {
                if (nClosingType == 3)
                    return DangerType.CAR_TO_WORKER_BOTH;
            }

            return DangerType.NONE;
        }

        // 두 객체가 서로 가까워지는 형태를 확인한다.
        // Return 값 : 1(obj1이 obj2 쪽으로 다가가고 있다.)
        //             2(obj2가 obj1 쪽으로 다가가고 있다.)
        //             3(양쪽이 서로 가까워지고 있다.)
        //             4(양쪽이 서로 멀어지고 있다.)
        //             0(양쪽이 나란히 거리를 유지하며 이동하고 있다.)
        private int GetClosingState(Vertex2D v1Current, Vertex2D v1Prev, Vertex2D v2Current, Vertex2D v2Prev)
        {
            double dLen = v1Current.GetDistance(v2Current);

            Vertex2D v1Next = v1Current * 2 - v1Prev;
            Vertex2D v2Next = v2Current * 2 - v2Prev;

            Line2D line1 = new Line2D(v1Current, v1Next, Line2D.LineType.SEGMENT);
            Line2D line2 = new Line2D(v2Current, v2Next, Line2D.LineType.SEGMENT);

            Vertex2D vResult1, vResult2;
            Line2D.LineType resultLineType;

            int nResult = line1.IntersectLine(line2, out vResult1, out vResult2, out resultLineType);

            if (nResult == 0)
            {
                double dLen2 = v1Next.GetDistance(v2Next);

                if (dLen == dLen2)
                    return 0;
                else if (dLen < dLen2)
                    return 4;
                else// if (dLen > dLen2)
                {
                    double dLen11 = v1Current.GetDistance(v2Next);
                    double dLen22 = v2Current.GetDistance(v1Next);

                    if (dLen2 < dLen11)
                    {
                        if (dLen2 < dLen22)
                        {
                            // 양쪽이 서로 다가서는 경우
                            return 3;
                        }
                        else
                        {
                            // obj1이 obj2쪽으로 다가가고 있다.
                            return 1;
                        }
                    }
                    else if (dLen2 < dLen22)
                    {
                        // obj2가 obj1쪽으로 다가가고 있다.
                        return 2;
                    }
                }
            }
            else if (nResult == 1)
            {
                double dLen2 = v1Next.GetDistance(v2Next);

                double dLen11 = v1Current.GetDistance(v2Next);
                double dLen22 = v2Current.GetDistance(v1Next);

                if (dLen2 < dLen11)
                {
                    if (dLen2 < dLen22)
                    {
                        // 양쪽이 서로 다가서는 경우
                        return 3;
                    }
                    else
                    {
                        // obj1이 obj2쪽으로 다가가고 있다.
                        return 1;
                    }
                }
                else if (dLen2 < dLen22)
                {
                    // obj2가 obj1쪽으로 다가가고 있다.
                    return 2;
                }
            }
            else if (nResult == 2)
            {
                Vertex2D vector1 = v1Next - v1Current;
                Vertex2D vector2 = v2Next - v2Current;

                if (UnE.Geometry.Math.GetAngle(vector1, new Vertex2D(0, 0), vector2) < UnE.Geometry.Math.HALF_TOLERANCE())
                {
                    if (line1.IsInclude(v2Current))
                    {
                        // obj1이 obj2쪽으로 다가가고 있다.
                        return 1;
                    }
                    else if (line2.IsInclude(v1Current))
                    {
                        // obj2가 obj1쪽으로 다가가고 있다.
                        return 2;
                    }
                }

                // 양쪽이 서로 다가서는 경우
                return 3;
            }

            return 0;
        }

        public bool CheckAlarmValidation(DangerState state)
        {
            if (!m_dicSensorHistory.ContainsKey(state.Worker.Sensor))
                return false;
            
            SensorList workerSensorList = m_dicSensorHistory[state.Worker.Sensor];

            if (state.Type == DangerType.CAR_TO_WORKER ||
                state.Type == DangerType.WORKER_TO_CAR ||
                state.Type == DangerType.CAR_TO_WORKER_BOTH)
            {
                if (state.TargetCar == null)
                    return false;

                if (!m_dicSensorHistory.ContainsKey(state.TargetCar.Sensor))
                    return false;

                SensorList carSensorList = m_dicSensorHistory[state.TargetCar.Sensor];

                DangerState _state = CheckStateWorkerToCar(workerSensorList, carSensorList);

                if (_state != null)
                    return true;
            }
            else if (state.Type == DangerType.WORKER_TO_EQUIP)
            {
                if (state.TargetEquipment == null)
                    return false;

                SensorList equipSensorList = m_dicSensorHistory[state.TargetEquipment.Sensor];
                DangerState _state = CheckStateWorkerToEquip(workerSensorList, equipSensorList);

                if (_state != null)
                    return true;
            }
            else if (state.Type == DangerType.WORKER_TO_ZONE)
            {
                if (state.TargetZone == null)
                    return false;

                DangerState _state = CheckStateWorkerToZone(workerSensorList, state.TargetZone);

                if (_state != null)
                    return true;
            }

            return false;
        }
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
        private AlarmManager.AlarmStatus m_alarmStatus = AlarmManager.AlarmStatus.NONE;
        private string m_strAlarmStatus = "";
        private string m_strAlarmMessage = "";
        private string m_strShortAlarmMessage = "";
        private bool m_isCritical = false;

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

        public AlarmManager.AlarmStatus AlarmStatus
        {
            get { return m_alarmStatus; }
            set { m_alarmStatus = value; }
        }

        public string AlarmStatusMessage
        {
            get { return m_strAlarmStatus; }
            set { m_strAlarmStatus = value; }
        }

        public string AlarmMessage
        {
            get { return m_strAlarmMessage; }
            set { m_strAlarmMessage = value; }
        }

        public string ShortAlarmMessage
        {
            get { return m_strShortAlarmMessage; }
            set { m_strShortAlarmMessage = value; }
        }

        public bool IsCritical
        {
            get { return m_isCritical; }
            set { m_isCritical = value; }
        }

        public override string ToString()
        {
            if (m_worker == null)
                return "";

            string szTeamName = "";
            if (m_worker.Team != null)
                szTeamName = m_worker.Team.Name;

            return m_worker.Company.CompanyName + " " + szTeamName + " " + m_worker.Name;

        }
    }
}
