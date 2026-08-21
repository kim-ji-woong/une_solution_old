using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using HSMS;
using System.Data.SqlClient;

namespace HSMSServer2
{
    public class AlarmManager
    {
        public enum AlarmStatus
        {
            NONE = 0,
            ALARM_BEGINNING,
            ALARM_PROCESSING,
            ALARM_FINISH,
            ALARM_FINISH_BY_USER,   // 알람종료버튼 클릭에 의한 종료
            TYPE_COUNT
        };

        public enum AlarmIgnoreOption
        {
            NONE = 0,
            IGNORE_FOREVER,
            IGNORE_TIME,
            IGNORE_DISTANCE,
            IGNORE_TIME_N_DISTANCE_OR,
            IGNORE_TIME_N_DISTANCE_AND,
            TYPE_COUNT
        };

        public class IgnoreAlarmInfo
        {
            private DataWorker m_worker = null;
            private string m_strTargetSensorID = "";
            private DataZone m_targetZone = null;
            private DateTime m_timeBeginIgnore = new DateTime();
            private NetworkClient.ObjectType m_targetType = NetworkClient.ObjectType.NONE;

            public DataWorker Worker
            {
                get { return m_worker; }
                set { m_worker = value; }
            }

            public string TargetSensorID
            {
                get { return m_strTargetSensorID; }
                set { m_strTargetSensorID = value; }
            }

            public DataZone TargetZone
            {
                get { return m_targetZone; }
                set { m_targetZone = value; }
            }

            public DateTime BeginIgnoreTime
            {
                get { return m_timeBeginIgnore; }
                set { m_timeBeginIgnore = value; }
            }

            public NetworkClient.ObjectType TargetType
            {
                get { return m_targetType; }
                set { m_targetType = value; }
            }
        }

        // DataWorker, DangerState List
        private Dictionary<DataWorker, ArrayList> m_dicWorkerAlarms = new Dictionary<DataWorker, ArrayList>();
        // SensorID, Gas 농도
        private Dictionary<string, double> m_dicCoGasAlarms = new Dictionary<string, double>();
        // SensorID, Alarm History ID
        private Dictionary<string, int> m_dicCoGasAlarmHistoryIDs = new Dictionary<string, int>();
        // SensorID, Gas 농도
        private Dictionary<string, double> m_dicMethaneGasAlarms = new Dictionary<string, double>();
        // SensorID, Alarm History ID
        private Dictionary<string, int> m_dicMethaneGasAlarmHistoryIDs = new Dictionary<string, int>();
        private DataWorker m_worker4Gas = new DataWorker();

        #region 강제종료시킨 알람에 대한 무시 옵션
        private AlarmIgnoreOption m_ignoreOptionCar = AlarmIgnoreOption.NONE;
        private AlarmIgnoreOption m_ignoreOptionEquip = AlarmIgnoreOption.NONE;
        private AlarmIgnoreOption m_ignoreOptionZone = AlarmIgnoreOption.NONE;
        private int m_nIgnoreSecondsCar = 0;
        private int m_nIgnoreSecondsEquip = 0;
        private int m_nIgnoreSecondsZone = 0;
        private int m_nIgnoreMetersCar = 0;
        private int m_nIgnoreMetersEquip = 0;
        private int m_nIgnoreMetersZone = 0;

        public AlarmIgnoreOption IgnoreOptionCar
        {
            get { return m_ignoreOptionCar; }
            set { m_ignoreOptionCar = value; }
        }

        public AlarmIgnoreOption IgnoreOptionEquip
        {
            get { return m_ignoreOptionEquip; }
            set { m_ignoreOptionEquip = value; }
        }

        public AlarmIgnoreOption IgnoreOptionZone
        {
            get { return m_ignoreOptionZone; }
            set { m_ignoreOptionZone = value; }
        }

        // 강제종료시킨 알람을 얼마의 시간동안 무시할 것인가?
        // 단위 : 초
        public int IgnoreTimeCar
        {
            get { return m_nIgnoreSecondsCar; }
            set { m_nIgnoreSecondsCar = value; }
        }

        // 강제종료시킨 알람을 얼마의 시간동안 무시할 것인가?
        // 단위 : 초
        public int IgnoreTimeEquip
        {
            get { return m_nIgnoreSecondsEquip; }
            set { m_nIgnoreSecondsEquip = value; }
        }

        // 강제종료시킨 알람을 얼마의 시간동안 무시할 것인가?
        // 단위 : 초
        public int IgnoreTimeZone
        {
            get { return m_nIgnoreSecondsZone; }
            set { m_nIgnoreSecondsZone = value; }
        }

        // 강제종료시킨 알람을 얼마의 거리까지 무시할 것인가?
        // 단위 : meter
        public int IgnoreDistanceCar
        {
            get { return m_nIgnoreMetersCar; }
            set { m_nIgnoreMetersCar = value; }
        }

        // 강제종료시킨 알람을 얼마의 거리까지 무시할 것인가?
        // 단위 : meter
        public int IgnoreDistanceEquip
        {
            get { return m_nIgnoreMetersEquip; }
            set { m_nIgnoreMetersEquip = value; }
        }

        // 강제종료시킨 알람을 얼마의 거리까지 무시할 것인가?
        // 단위 : meter
        public int IgnoreDistanceZone
        {
            get { return m_nIgnoreMetersZone; }
            set { m_nIgnoreMetersZone = value; }
        }
        #endregion

        #region 작업자별 무시될 알람 정보
        // Value : IgnoreAlarmInfo List
        private Dictionary<DataWorker, ArrayList> m_dicIgnoreAlarmInfo = new Dictionary<DataWorker, ArrayList>();

        public void SetIgnoreAlarm(DataWorker worker, string strTargetSensorID, DataZone zoneTarget, DateTime dtBegin, NetworkClient.ObjectType targetType)
        {
            ArrayList arrIgnores = null;

            if (m_dicIgnoreAlarmInfo.ContainsKey(worker))
            {
                arrIgnores = m_dicIgnoreAlarmInfo[worker];

                foreach (IgnoreAlarmInfo info in arrIgnores)
                {
                    if (info.TargetSensorID == strTargetSensorID && info.TargetZone == zoneTarget && info.TargetType == targetType)
                    {
                        // 이미 적용중인 상태이다.
                        return;
                    }
                }
            }
            else
            {
                arrIgnores = new ArrayList();
                m_dicIgnoreAlarmInfo[worker] = arrIgnores;
            }

            IgnoreAlarmInfo alarmInfo = new IgnoreAlarmInfo();
            alarmInfo.Worker = worker;
            alarmInfo.TargetSensorID = strTargetSensorID;
            alarmInfo.TargetZone = zoneTarget;
            alarmInfo.BeginIgnoreTime = dtBegin;
            alarmInfo.TargetType = targetType;

            if (SetIgnoreAlarmDB(alarmInfo))
                arrIgnores.Add(alarmInfo);
        }

        public ArrayList GetIgnoreAlarms(DataWorker worker)
        {
            if (m_dicIgnoreAlarmInfo.ContainsKey(worker))
                return m_dicIgnoreAlarmInfo[worker];

            return null;
        }

        public IgnoreAlarmInfo GetIgnoreAlarm(DataWorker worker, string strTargetSensorID, DataZone zoneTarget)
        {
            if (m_dicIgnoreAlarmInfo.ContainsKey(worker))
            {
                ArrayList arrIgnores = m_dicIgnoreAlarmInfo[worker];

                foreach (IgnoreAlarmInfo info in arrIgnores)
                {
                    if (info.TargetSensorID == strTargetSensorID && info.TargetZone == zoneTarget)
                    {
                        // 이미 적용중인 상태이다.
                        return info;
                    }
                }
            }

            return null;
        }

        public void RemoveIgnoreAlarms(DataWorker worker)
        {
            if (m_dicIgnoreAlarmInfo.ContainsKey(worker))
            {
                ArrayList arrIgnores = m_dicIgnoreAlarmInfo[worker];

                foreach (IgnoreAlarmInfo info in arrIgnores)
                {
                    RemoveIgnoreAlarmDB(info);
                }

                m_dicIgnoreAlarmInfo.Remove(worker);
            }
        }

        public void RemoveIgnoreAlarm(IgnoreAlarmInfo info)
        {
            if (m_dicIgnoreAlarmInfo.ContainsKey(info.Worker))
            {
                ArrayList arrIgnores = m_dicIgnoreAlarmInfo[info.Worker];
                RemoveIgnoreAlarmDB(info);
                arrIgnores.Remove(info);
            }
        }

        private void RemoveIgnoreAlarmDB(IgnoreAlarmInfo info)
        {
            string strTargetSensorID = info.TargetSensorID.Length == 0 ? "is NULL" : string.Format("= '{0}'", info.TargetSensorID);
            string strTargetZoneID = info.TargetZone == null ? "is NULL" : "= " + info.TargetZone.ID.ToString();

            int nSiteID = NetworkServer.Instance.SiteID;

            string strSQL = string.Format("Delete from IgnoreAlarm where WorkerID = {0} and TargetSensorID {1} and " +
                "TargetZoneID {2} and SiteID = {3}",
                info.Worker.ID, strTargetSensorID, strTargetZoneID, nSiteID);

            SqlConnection connection = NetworkServer.Instance.DBManager.Connect();
            NetworkServer.Instance.DBManager.ExecuteSQL(strSQL, connection);
            connection.Close();
        }

        private bool SetIgnoreAlarmDB(IgnoreAlarmInfo info)
        {
            if (info.Worker == null || info.Worker.ID <= 0)
                return false;

            if (info.TargetSensorID.Length == 0 && info.TargetZone == null)
                return false;

            if (info.TargetSensorID.Length == 0 && info.TargetZone.ID <= 0)
                return false;

            string strTargetSensorID = info.TargetSensorID.Length == 0 ? "NULL" : string.Format("'{0}'", info.TargetSensorID);
            string strTargetZoneID = info.TargetZone == null ? "NULL" : info.TargetZone.ID.ToString();
            string strTime = string.Format("'{0}-{1}-{2} {3}:{4}:{5}'", info.BeginIgnoreTime.Year, info.BeginIgnoreTime.Month, info.BeginIgnoreTime.Day, info.BeginIgnoreTime.Hour, info.BeginIgnoreTime.Minute, info.BeginIgnoreTime.Second);

            int nSiteID = NetworkServer.Instance.SiteID;

            string strSQL = string.Format("Insert into IgnoreAlarm (WorkerID, TargetSensorID, TargetZoneID, BeginTime, TargetType, SiteID, Description) values " +
                "({0}, {1}, {2}, {3}, {4}, {5}, NULL)",
                info.Worker.ID, strTargetSensorID, strTargetZoneID, strTime, (int)info.TargetType, nSiteID);

            SqlConnection connection = NetworkServer.Instance.DBManager.Connect();
            NetworkServer.Instance.DBManager.ExecuteSQL(strSQL, connection);
            connection.Close();

            return true;
        }

        // 화면에 보여지고 있는 알람
        //private DangerState m_stateScreen = null;

        private bool IsValidAlarm(double dDistance, DateTime dtTime, IgnoreAlarmInfo info, AlarmIgnoreOption option, int nDistance, int nTime)
        {
            if (option == AlarmIgnoreOption.NONE)
                return true;
            else if (option == AlarmIgnoreOption.IGNORE_FOREVER)
                return false;
            else if (option == AlarmIgnoreOption.IGNORE_DISTANCE)
                return dDistance > (double)nDistance;
            else if (option == AlarmIgnoreOption.IGNORE_TIME)
            {
                return CheckValidTime(info, dtTime, nTime);
            }
            else if (option == AlarmIgnoreOption.IGNORE_TIME_N_DISTANCE_AND)
            {
                if (dDistance <= (double)nDistance)
                    return false;

                return CheckValidTime(info, dtTime, nTime);
            }
            else if (option == AlarmIgnoreOption.IGNORE_TIME_N_DISTANCE_OR)
            {
                if (dDistance > (double)nDistance)
                    return true;

                return CheckValidTime(info, dtTime, nTime);
            }

            return true;
        }

        private bool CheckValidTime(IgnoreAlarmInfo info, DateTime dtTime, int nTime)
        {
            TimeSpan span = dtTime - info.BeginIgnoreTime;
            double dTotalSeconds = span.TotalSeconds;

            return dTotalSeconds > (double)nTime;
        }

        // 무시되어야 할 알람에 포함되어 있지 않은지 검사
        private bool IsValidAlarm(DangerState state)
        {
            if (m_dicIgnoreAlarmInfo.ContainsKey(state.Worker))
            {
                ArrayList arrIgnores = m_dicIgnoreAlarmInfo[state.Worker];

                foreach (IgnoreAlarmInfo info in arrIgnores)
                {
                    if (info.Worker == state.Worker)
                    {
                        IgnoreAlarmInfo targetInfo = null;
                        bool isValidAlarm = false;

                        if (info.TargetType == NetworkClient.ObjectType.VEHICLE && state.TargetCar != null)
                        {
                            if (info.TargetSensorID == state.TargetCar.Sensor)
                            {
                                targetInfo = info;
                                isValidAlarm = IsValidAlarm(state.Distance, state.EventTime, info, IgnoreOptionCar, IgnoreDistanceCar, IgnoreTimeCar);
                            }
                        }
                        else if (info.TargetType == NetworkClient.ObjectType.EQUIPMENT && state.TargetEquipment != null)
                        {
                            if (info.TargetSensorID == state.TargetEquipment.Sensor)
                            {
                                targetInfo = info;
                                isValidAlarm = IsValidAlarm(state.Distance, state.EventTime, info, IgnoreOptionEquip, IgnoreDistanceEquip, IgnoreTimeEquip);
                            }
                        }
                        else if (info.TargetType == NetworkClient.ObjectType.ZONE && state.TargetZone != null)
                        {
                            if (info.TargetZone == state.TargetZone)
                            {
                                targetInfo = info;
                                isValidAlarm = IsValidAlarm(state.Distance, state.EventTime, info, IgnoreOptionZone, IgnoreDistanceZone, IgnoreTimeZone);
                            }
                        }

                        if (targetInfo != null)
                        {
                            if (isValidAlarm)
                                RemoveIgnoreAlarm(info);

                            return isValidAlarm;
                        }
                    }
                }
            }

            return true;
        }

        // 알람 무시조건을 확인후 조건이 충족되면 무시 목록에서 삭제한다.
        public void CheckNRemoveIgnoreAlarm(IgnoreAlarmInfo info, double dDistance, DateTime dtTime, DataCar car, DataEquip equip, DataZone zone)
        {
            bool isValidAlarm = false;

            if (info.TargetType == NetworkClient.ObjectType.VEHICLE && car != null)
            {
                if (info.TargetSensorID == car.Sensor)
                {
                    isValidAlarm = IsValidAlarm(dDistance, dtTime, info, IgnoreOptionCar, IgnoreDistanceCar, IgnoreTimeCar);
                }
            }
            else if (info.TargetType == NetworkClient.ObjectType.EQUIPMENT && equip != null)
            {
                if (info.TargetSensorID == equip.Sensor)
                {
                    isValidAlarm = IsValidAlarm(dDistance, dtTime, info, IgnoreOptionEquip, IgnoreDistanceEquip, IgnoreTimeEquip);
                }
            }
            else if (info.TargetType == NetworkClient.ObjectType.ZONE && zone != null)
            {
                if (info.TargetZone == zone)
                {
                    isValidAlarm = IsValidAlarm(dDistance, dtTime, info, IgnoreOptionZone, IgnoreDistanceZone, IgnoreTimeZone);
                }
            }

            if (isValidAlarm)
                RemoveIgnoreAlarm(info);
        }
        #endregion

        public void AddAlarm(DataWorker worker, DangerState state, ArrayList arrBytes, bool noInsertDB = false)
        {
            if (!IsValidAlarm(state))
                return;

            ArrayList arrStates = null;
            bool isCritical;
            string strWorkerInfo, strAlarmStatus, strAlarmMessage;

            if (m_dicWorkerAlarms.ContainsKey(worker))
            {
                arrStates = m_dicWorkerAlarms[worker];

                foreach (DangerState _state in arrStates)
                {
                    if (_state.TargetCar == state.TargetCar &&
                        _state.TargetEquipment == state.TargetEquipment &&
                        _state.TargetZone == state.TargetZone)
                    {
                        if (System.Math.Abs(_state.Distance - state.Distance) > 0.001 ||
                            _state.Type != state.Type)
                        {
                            // 동일한 알람이 존재하면 상태값만 바꿔준다.
                            _state.Distance = state.Distance;
                            _state.Type = state.Type;
                            _state.EventTime = state.EventTime;

                            if (GetAlarmMessage(_state, worker, out strWorkerInfo, out strAlarmStatus, out strAlarmMessage, out isCritical))
                            {
                                _state.AlarmStatus = AlarmStatus.ALARM_PROCESSING;
                                _state.AlarmStatusMessage = strAlarmStatus;
                                _state.AlarmMessage = strAlarmMessage;
                                _state.ShortAlarmMessage = MakeShortAlarmMessage(worker, _state.TargetCar, _state.TargetEquipment, _state.TargetZone, state.Type, state.Distance, isCritical);
                                _state.IsCritical = isCritical;

                                UpdateAlarmHistory(_state, worker, strAlarmStatus, strAlarmMessage, _state.ShortAlarmMessage, isCritical, arrBytes);
                            }
                        }

                        return;
                    }
                }
            }
            else
            {
                arrStates = new ArrayList();
                m_dicWorkerAlarms[worker] = arrStates;
            }

            arrStates.Add(state);

            if (!noInsertDB)
            {
                if (GetAlarmMessage(state, worker, out strWorkerInfo, out strAlarmStatus, out strAlarmMessage, out isCritical))
                {
                    state.AlarmStatus = AlarmStatus.ALARM_BEGINNING;
                    state.AlarmStatusMessage = strAlarmStatus;
                    state.AlarmMessage = strAlarmMessage;
                    state.ShortAlarmMessage = MakeShortAlarmMessage(worker, state.TargetCar, state.TargetEquipment, state.TargetZone, state.Type, state.Distance, isCritical);
                    state.IsCritical = isCritical;

                    InsertAlarmHistory(state, worker, strAlarmStatus, strAlarmMessage, state.ShortAlarmMessage, isCritical, arrBytes);
                }
            }
        }

        public void AddGasAlarm(string strSensorID, double dGas, SafetyChecker.DangerType alarmType, int nAlarmHistoryID)
        {
            if (alarmType == SafetyChecker.DangerType.CO_GAS_ALARM)
            {
                m_dicCoGasAlarms[strSensorID] = dGas;
                m_dicCoGasAlarmHistoryIDs[strSensorID] = nAlarmHistoryID;
            }
            else if (alarmType == SafetyChecker.DangerType.METHANE_ALARM)
            {
                m_dicMethaneGasAlarms[strSensorID] = dGas;
                m_dicCoGasAlarmHistoryIDs[strSensorID] = nAlarmHistoryID;
            }
        }

        private void AddGasAlarmHistory(string strSensorID, double dGas, SafetyChecker.DangerType alarmType, Dictionary<string, double> dicGasAlarms, Dictionary<string, int> dicGasAlarmHistoryIDs, out int nAlarmHistoryID, out int nAlarmProcessHistoryID, out string strStatus, out string strMessage)
        {
            string strGasName = alarmType == SafetyChecker.DangerType.CO_GAS_ALARM ? "일산화탄소" : "메탄가스";
            string strSensorOwner = GetGasSensorOwner(strSensorID);
            strMessage = string.Format("{0} 주변에서 {1:F1}ppm의 {2} 누출이 감지되었습니다.", strSensorOwner, dGas, strGasName);
            strStatus = strGasName + " 누출";

            int nProcessType = 1;   // 알람 발생

            if (!dicGasAlarmHistoryIDs.TryGetValue(strSensorID, out nAlarmHistoryID))
            {
                nAlarmHistoryID = DBHelper.FindMaxID(NetworkServer.Instance.DBManager, "AlarmHistory");

                string strSQL = string.Format("Insert into AlarmHistory (ID, WorkerMemberID, TargetSensorID, TargetZoneID, AlarmType, Done, SiteID, Description) values ({0}, '', '{1}', NULL, {2}, 0, {3}, NULL)",
                    nAlarmHistoryID, strSensorID, (int)alarmType, NetworkServer.Instance.SiteID);

                ExecuteQuery(NetworkServer.Instance.DBManager, strSQL);
            }
            else
                nProcessType = 2;   // 알람 진행중

            dicGasAlarmHistoryIDs[strSensorID] = nAlarmHistoryID;

            nAlarmProcessHistoryID = DBHelper.FindMaxID(NetworkServer.Instance.DBManager, "AlarmProcessHistory");

            string strFormat = "Insert into AlarmProcessHistory (ID, AlarmHistoryID, Time, ProcessType, Distance, Status, Message, IsCritical, Description) ";
            strFormat += "values ({0}, {1}, '{2}', {3}, {4}, '{5}', '{6}', 0, NULL)";

            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
            string strSQL2 = string.Format(strFormat, nAlarmProcessHistoryID, nAlarmHistoryID, strTime, nProcessType, (float)dGas, strStatus, strMessage);

            ExecuteQuery(NetworkServer.Instance.DBManager, strSQL2);
            dicGasAlarms[strSensorID] = dGas;
        }

        private int RemoveGasAlarmHistory(string strSensorID, Dictionary<string, double> dicGasAlarms, Dictionary<string, int> dicGasAlarmHistoryIDs)
        {
            int nAlarmHistoryID;

            if (!dicGasAlarmHistoryIDs.TryGetValue(strSensorID, out nAlarmHistoryID))
                return -1;

            string strSQL = "Update AlarmHistory set Done = 1 where ID = " + nAlarmHistoryID.ToString();
            ExecuteQuery(NetworkServer.Instance.DBManager, strSQL);

            dicGasAlarms.Remove(strSensorID);
            dicGasAlarmHistoryIDs.Remove(strSensorID);
            return nAlarmHistoryID;
        }

        private string GetGasSensorOwner(string strSensorID)
        {
            string strSensorOwner = "";
            GasSensor gasSensor = NetworkServer.Instance.DataManager.FindGasSensor(strSensorID);

            if (gasSensor != null)
                strSensorOwner = gasSensor.SensorName;

            return strSensorOwner;
        }

        private void ExecuteQuery(DBConn conn, string strSQL)
        {
            SqlConnection connection = NetworkServer.Instance.DBManager.Connect();
            NetworkServer.Instance.DBManager.ExecuteSQL(strSQL, connection);
            connection.Close();
        }

        public void CheckGasData(string strSensorID, double dCoGas, double dMethaneGas)
        {
            int nAlarmHistoryID, nAlarmProcessHistoryID;
            string strStatus, strMessage;

            if (dCoGas >= NetworkServer.Instance.DataManager.COGasTolerance)
            {
                double dCo;

                if (m_dicCoGasAlarms.TryGetValue(strSensorID, out dCo))
                {
                    if (System.Math.Abs(dCo - dCoGas) <= 0.001)
                        return;

                    m_dicCoGasAlarms[strSensorID] = dCoGas;
                }

                AddGasAlarmHistory(strSensorID, dCoGas, SafetyChecker.DangerType.CO_GAS_ALARM, m_dicCoGasAlarms, m_dicCoGasAlarmHistoryIDs, out nAlarmHistoryID, out nAlarmProcessHistoryID, out strStatus, out strMessage);
                //m_dicCoGasAlarms[strSensorID] = dCoGas;
                SendGasAlarm(strSensorID, (int)SafetyChecker.DangerType.CO_GAS_ALARM, dCoGas, nAlarmHistoryID, nAlarmProcessHistoryID, strStatus, strMessage);
            }
            else
            {
                if (m_dicCoGasAlarms.ContainsKey(strSensorID))
                {
                    nAlarmHistoryID = RemoveGasAlarmHistory(strSensorID, m_dicCoGasAlarms, m_dicCoGasAlarmHistoryIDs);
                    //m_dicCoGasAlarms.Remove(strSensorID);
                    SendFinishGasAlarm(strSensorID, (int)SafetyChecker.DangerType.CO_GAS_ALARM, nAlarmHistoryID);
                }
            }

            if (dMethaneGas >= NetworkServer.Instance.DataManager.MethaneTolerance)
            {
                double dMethane;

                if (m_dicMethaneGasAlarms.TryGetValue(strSensorID, out dMethane))
                {
                    if (System.Math.Abs(dMethane - dMethaneGas) <= 0.001)
                        return;

                    m_dicMethaneGasAlarms[strSensorID] = dMethaneGas;
                }

                AddGasAlarmHistory(strSensorID, dMethaneGas, SafetyChecker.DangerType.METHANE_ALARM, m_dicMethaneGasAlarms, m_dicMethaneGasAlarmHistoryIDs, out nAlarmHistoryID, out nAlarmProcessHistoryID, out strStatus, out strMessage);
                //m_dicMethaneGasAlarms[strSensorID] = dMethaneGas;
                SendGasAlarm(strSensorID, (int)SafetyChecker.DangerType.METHANE_ALARM, dMethaneGas, nAlarmHistoryID, nAlarmProcessHistoryID, strStatus, strMessage);
            }
            else
            {
                if (m_dicMethaneGasAlarms.ContainsKey(strSensorID))
                {
                    nAlarmHistoryID = RemoveGasAlarmHistory(strSensorID, m_dicMethaneGasAlarms, m_dicMethaneGasAlarmHistoryIDs);
                    //m_dicMethaneGasAlarms.Remove(strSensorID);
                    SendFinishGasAlarm(strSensorID, (int)SafetyChecker.DangerType.METHANE_ALARM, nAlarmHistoryID);
                }
            }
        }

        private void SendFinishGasAlarm(string strSensorID, int nGasType, int nAlarmHistoryID)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(strSensorID);
            arrDatas.Add(nGasType);
            arrDatas.Add(nAlarmHistoryID);

            byte[] bytes = ServiceProvider.MakeBytes((short)TCP_ID.FINISH_GAS_ALARM, arrDatas);
            NetworkServer.Instance.ServiceProvider.SendClientData(bytes, ClientData.ClientType.HSMS_CLIENT, false);
        }

        private void SendGasAlarm(string strSensorID, int nGasType, double dGas, int nAlarmHistoryID, int nAlarmProcessHistoryID, string strStatus, string strMessage)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(strSensorID);
            arrDatas.Add(nGasType);
            arrDatas.Add(dGas);
            arrDatas.Add(nAlarmHistoryID);
            arrDatas.Add(nAlarmProcessHistoryID);
            arrDatas.Add(strStatus);
            arrDatas.Add(strMessage);

            byte[] bytes = ServiceProvider.MakeBytes((short)TCP_ID.GAS_ALARM, arrDatas);
            NetworkServer.Instance.ServiceProvider.SendClientData(bytes, ClientData.ClientType.HSMS_CLIENT, false);
        }

        public static int GetMaxID(string strTableName, SqlConnection connection = null)
        {
            DBConn dbMgr = NetworkServer.Instance.DBManager;
            
            SqlConnection connection2 = connection == null ? dbMgr.Connect() : connection;

            string strSQL = "Select max(ID) from " + strTableName;
            SqlDataReader reader = dbMgr.ExecuteReader(strSQL, connection2);

            int nID = 0;

            if (reader.Read())
            {
                if (!reader.IsDBNull(0))
                    nID = (int)reader[0];
            }

            reader.Close();

            if (connection == null)
                connection2.Close();

            return nID;
        }

        private bool GetTargetInfo(DangerState state, out string strSensorID, out string strZoneID)
        {
            strSensorID = "NULL";
            strZoneID = "NULL";

            if (state.TargetCar != null)
            {
                strSensorID = state.TargetCar.Sensor;
                return true;
            }
            else if (state.TargetEquipment != null)
            {
                strSensorID = state.TargetEquipment.Sensor;
                return true;
            }
            else if (state.TargetZone != null)
            {
                strZoneID = state.TargetZone.ID.ToString();
                return true;
            }

            return false;
        }

        private void InsertAlarmHistory(DangerState state, DataWorker worker, string strAlarmStatus, string strAlarmMessage, string strShortAlarmMessage, bool isCritical, ArrayList arrBytes)
        {
            DBConn dbMgr = NetworkServer.Instance.DBManager;
            SqlConnection connection = dbMgr.Connect();

            string strTargetSensorID, strTargetZoneID;
            int nID = GetMaxID("AlarmHistory", connection) + 1;

            if (!GetTargetInfo(state, out strTargetSensorID, out strTargetZoneID))
            {
                connection.Close();
                return;
            }

            string strSQL = string.Format("Insert into AlarmHistory (ID, WorkerMemberID, TargetSensorID, TargetZoneID, AlarmType, Done, SiteID, Description) values ({0}, '{1}', {2}, {3}, {4}, 0, {5}, NULL)",
                nID, worker.MemberID,
                strTargetSensorID == "NULL" ? strTargetSensorID : "'" + strTargetSensorID + "'",
                strTargetZoneID,
                (int)state.Type,
                NetworkServer.Instance.SiteID);

            dbMgr.ExecuteSQL(strSQL, connection);

            int nProcessID = GetMaxID("AlarmProcessHistory", connection) + 1;

            DateTime dtTime = state.EventTime;
            string strTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", dtTime.Year, dtTime.Month, dtTime.Day, dtTime.Hour, dtTime.Minute, dtTime.Second);

            strSQL = string.Format("Insert into AlarmProcessHistory (ID, AlarmHistoryID, Time, ProcessType, Distance, Status, Message, isCritical, Description) values ({0}, {1}, '{2}', {3}, {4:F2}, '{5}', '{6}', {7}, NULL)",
                nProcessID, nID, strTime, (int)AlarmStatus.ALARM_BEGINNING, state.Distance, strAlarmStatus, strAlarmMessage, isCritical ? 1 : 0);

            dbMgr.ExecuteSQL(strSQL, connection);
            connection.Close();

            state.AlarmHistoryID = nID;
            state.AlarmProcessHistoryID = nProcessID;

            byte[] bytes1 = MakeAlarmHistoryBytes(nID, worker.ID, strTargetSensorID, strTargetZoneID, state.Type);
            //arrBytes.Add(bytes1);
            NetworkServer.Instance.ServiceProvider.SendClientData(bytes1, ClientData.ClientType.HSMS_CLIENT, false);

            byte[] bytes2 = MakeAlarmProcessHistoryBytes(nID, nProcessID, dtTime, AlarmStatus.ALARM_BEGINNING, state.Distance, strAlarmStatus, strAlarmMessage, strShortAlarmMessage, isCritical);
            //arrBytes.Add(bytes2);
            NetworkServer.Instance.ServiceProvider.SendClientData(bytes2, ClientData.ClientType.HSMS_CLIENT, false);
        } 

        private byte[] MakeAlarmHistoryBytes(int nAlarmHistoryID, int nWorkerID, string strTargetSensorID, string strTargetZoneID, SafetyChecker.DangerType type)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nAlarmHistoryID);
            arrDatas.Add(nWorkerID);
            arrDatas.Add(strTargetSensorID);
            arrDatas.Add(strTargetZoneID);
            arrDatas.Add((int)type);

            return ServiceProvider.MakeBytes(TCP_ID.ALARM_HISTORY, arrDatas);
        }

        private byte[] MakeAlarmProcessHistoryBytes(int nAlarmHistoryID, int nAlarmProcessHistoryID, DateTime dtTime, AlarmStatus status, double distance, string strAlarmStatus, string strAlarmMessage, string strShortAlarmMessage, bool isCritical)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nAlarmProcessHistoryID);
            arrDatas.Add(nAlarmHistoryID);
            arrDatas.Add(dtTime);
            arrDatas.Add((int)status);
            arrDatas.Add(distance);
            arrDatas.Add(strAlarmStatus);
            arrDatas.Add(strAlarmMessage);
            arrDatas.Add(strShortAlarmMessage);
            arrDatas.Add(isCritical);

            return ServiceProvider.MakeBytes(TCP_ID.ALARM_PROCESS_HISTORY, arrDatas);
        }

        private void UpdateAlarmHistory(DangerState state, DataWorker worker, string strAlarmStatus, string strAlarmMessage, string strShortAlarmMessage, bool isCritical, ArrayList arrBytes)
        {
            if (state.AlarmProcessHistoryID <= 0)
                return;

            DBConn dbMgr = NetworkServer.Instance.DBManager;
            SqlConnection connection = dbMgr.Connect();

            int nID = GetMaxID("AlarmProcessHistory", connection) + 1;

            DateTime dtTime = state.EventTime;
            string strTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", dtTime.Year, dtTime.Month, dtTime.Day, dtTime.Hour, dtTime.Minute, dtTime.Second);

            string strSQL = string.Format("Insert into AlarmProcessHistory (ID, AlarmHistoryID, Time, ProcessType, Distance, Status, Message, isCritical, Description) values ({0}, {1}, '{2}', {3}, {4:F2}, '{5}', '{6}', {7}, NULL)",
                nID, state.AlarmHistoryID, strTime, (int)AlarmStatus.ALARM_PROCESSING, state.Distance, strAlarmStatus, strAlarmMessage, isCritical ? 1 : 0);

            dbMgr.ExecuteSQL(strSQL, connection);
            connection.Close();

            state.AlarmProcessHistoryID = nID;

            byte[] bytes = MakeAlarmProcessHistoryBytes(state.AlarmHistoryID, nID, dtTime, AlarmStatus.ALARM_PROCESSING, state.Distance, strAlarmStatus, strAlarmMessage, strShortAlarmMessage, isCritical);
            //arrBytes.Add(bytes);
            NetworkServer.Instance.ServiceProvider.SendClientData(bytes, ClientData.ClientType.HSMS_CLIENT, false);
        }

        public bool GetAlarmMessage(DangerState state, DataWorker worker, out string strWorkerInfo, out string strAlarmStatus, out string strAlarmMessage, out bool isCritical)
        {
            isCritical = state.Distance <= 0.0;

            string szTeamName = "";
            if (worker.Team != null)
                szTeamName = worker.Team.Name;

            strWorkerInfo = worker.Company.CompanyName + " " + szTeamName + " " + worker.Name;

            strAlarmStatus = strAlarmMessage = "";

            if (state.TargetCar != null)
            {
                if (isCritical)
                {
                    strAlarmStatus = "차량 충돌";
                    strAlarmMessage = "작업자(" + worker.Name + ")가 " + state.TargetCar.Name + " 차량과 충돌하였습니다.";
                }
                else
                {
                    strAlarmStatus = "차량 접근중";

                    if (state.Type == SafetyChecker.DangerType.CAR_TO_WORKER)
                    {
                        strAlarmMessage = string.Format("{0} 차량이 작업자({1})에게 {2:F0}m 이내로 접근중입니다",
                                state.TargetCar.Name, worker.Name, state.Distance);
                    }
                    else if (state.Type == SafetyChecker.DangerType.WORKER_TO_CAR)
                    {
                        strAlarmMessage = string.Format("작업자({0})가 {1} 차량에게 {2:F0}m 이내로 접근중입니다",
                                worker.Name, state.TargetCar.Name, state.Distance);
                    }
                    else if (state.Type == SafetyChecker.DangerType.CAR_TO_WORKER_BOTH)
                    {
                        strAlarmMessage = string.Format("작업자({0})와 {1} 차량이 {2:F0}m 이내로 서로 접근중입니다",
                                worker.Name, state.TargetCar.Name, state.Distance);
                    }
                    else
                        return false;
                }
            }
            else if (state.TargetEquipment != null)
            {
                if (isCritical)
                {
                    strAlarmStatus = "설비영역 진입";
                    strAlarmMessage = "작업자(" + worker.Name + ")가 " + state.TargetEquipment.Name + " 설비영역으로 진입하였습니다.";
                }
                else
                {
                    strAlarmStatus = "설비영역 접근중";
                    //strAlarmMessage = "작업자(" + worker.Name + ")가 " + state.TargetEquipment.Name + " 설비영역으로 접근중입니다.";
                    strAlarmMessage = string.Format("작업자({0})가 {1} 설비영역의 {2:F0}m 이내로 접근중입니다.",
                        worker.Name, state.TargetEquipment.Name, state.Distance);
                }
            }
            else if (state.TargetZone != null)
            {
                if (isCritical)
                {
                    strAlarmStatus = "접근금지영역 진입";
                    strAlarmMessage = "작업자(" + worker.Name + ")가 " + state.TargetZone.ZoneName + " 영역으로 진입하였습니다.";
                }
                else
                {
                    strAlarmStatus = "접근금지영역 접근중";
                    //strAlarmMessage = "작업자(" + worker.Name + ")가 " + state.TargetZone.ZoneName + " 영역으로 접근중입니다.";
                    strAlarmMessage = string.Format("작업자({0})가 {1} 영역의 {2:F0}m 이내로 접근중입니다.",
                        worker.Name, state.TargetZone.ZoneName, state.Distance);
                }
            }
            else
                return false;

            return true;
        }

        public static string MakeShortAlarmMessage(DataWorker worker, DataCar car, DataEquip equip, DataZone zone, SafetyChecker.DangerType type, double distance, bool isCritical)
        {
            string strShortAlarmMessage = "";

            if (type == SafetyChecker.DangerType.CAR_TO_WORKER)
            {
                if (car != null)
                {
                    if (isCritical)
                        strShortAlarmMessage = worker.Name + " <-> " + car.Name + " 충돌";
                    else
                        strShortAlarmMessage = car.Name + " -> " + worker.Name + string.Format(" {0:F0}m 이내로 접근중", distance);
                }
            }
            else if (type == SafetyChecker.DangerType.CAR_TO_WORKER_BOTH)
            {
                if (car != null)
                {
                    if (isCritical)
                        strShortAlarmMessage = worker.Name + " <-> " + car.Name + " 충돌";
                    else
                        strShortAlarmMessage = worker.Name + " <-> " + car.Name + string.Format(" {0:F0}m 이내로 접근중", distance);
                }
            }
            else if (type == SafetyChecker.DangerType.WORKER_TO_CAR)
            {
                if (car != null)
                {
                    if (isCritical)
                        strShortAlarmMessage = worker.Name + " <-> " + car.Name + " 충돌";
                    else
                        strShortAlarmMessage = worker.Name + " -> " + car.Name + string.Format(" {0:F0}m 이내로 접근중", distance);
                }
            }
            else if (type == SafetyChecker.DangerType.WORKER_TO_EQUIP)
            {
                if (equip != null)
                {
                    if (isCritical)
                        strShortAlarmMessage = worker.Name + " -> " + equip.Name + " 설비영역 진입";
                    else
                        strShortAlarmMessage = worker.Name + " -> " + equip.Name + string.Format(" 설비영역 {0:F0} 이내로 접근중", distance);
                }
            }
            else if (type == SafetyChecker.DangerType.WORKER_TO_ZONE)
            {
                if (zone != null)
                {
                    if (isCritical)
                        strShortAlarmMessage = worker.Name + " -> " + zone.ZoneName + " 영역 진입";
                    else
                        strShortAlarmMessage = worker.Name + " -> " + zone.ZoneName + string.Format(" 영역 {0:F0}m 이내로 접근중", distance);
                }
            }

            return strShortAlarmMessage;
        }

        public ArrayList FindAlarms(DataWorker worker)
        {
            if (m_dicWorkerAlarms.ContainsKey(worker))
                return m_dicWorkerAlarms[worker];

            return null;
        }

        private void DeleteAlarm(DangerState state, ArrayList arrBytes, string strLoginUserID = null)
        {
            if (state.AlarmHistoryID <= 0)
                return;

            state.AlarmStatus = strLoginUserID == null ? AlarmStatus.ALARM_FINISH : AlarmStatus.ALARM_FINISH_BY_USER;
            state.AlarmStatusMessage = "";
            state.AlarmMessage = "";
            state.ShortAlarmMessage = "";

            DateTime dtTime = state.EventTime;

            int nProcessID = DeleteAlarmDB(state, dtTime, strLoginUserID);
            byte[] bytes = MakeAlarmProcessHistoryBytes(state.AlarmHistoryID, nProcessID, dtTime, state.AlarmStatus, state.Distance, "", "", "", false);

            NetworkServer.Instance.ServiceProvider.SendClientData(bytes, ClientData.ClientType.HSMS_CLIENT, false);
            //arrBytes.Add(bytes);
        }

        private int DeleteAlarmDB(DangerState state, DateTime dtTime, string strLoginUserID)
        {
            DBConn dbMgr = NetworkServer.Instance.DBManager;
            SqlConnection connection = dbMgr.Connect();

            int nID = GetMaxID("AlarmProcessHistory", connection) + 1;
            string strTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", dtTime.Year, dtTime.Month, dtTime.Day, dtTime.Hour, dtTime.Minute, dtTime.Second);

            string strDesc = strLoginUserID == null ? "NULL" : string.Format("'{0}'", strLoginUserID);

            string strSQL = string.Format("Insert into AlarmProcessHistory (ID, AlarmHistoryID, Time, ProcessType, Distance, Status, Message, isCritical, Description) values ({0}, {1}, '{2}', {3}, {4:F2}, '{5}', '{6}', {7}, {8})",
                nID, state.AlarmHistoryID, strTime, (int)state.AlarmStatus, state.Distance, "", "", 0, strDesc);

            dbMgr.ExecuteSQL(strSQL, connection);

            strSQL = "Update AlarmHistory set Done = 1 where id = " + state.AlarmHistoryID.ToString();
            dbMgr.ExecuteSQL(strSQL, connection);

            connection.Close();
            return nID;
        }

        public void RemoveAlarmAt(int nIndex, ArrayList arrBytes)
        {
            if (nIndex < 0 || nIndex >= GetAlarmWorkerCount())
                return;

            KeyValuePair<DataWorker, ArrayList> pair = m_dicWorkerAlarms.ElementAt(nIndex);
            RemoveAlarm(pair.Key, arrBytes);
        }

        public bool RemoveGasAlarm(string strSensorID, int nGasType)
        {
            Dictionary<string, double> dicGasAlarms = null;

            if (nGasType == (int)SafetyChecker.DangerType.CO_GAS_ALARM)
                dicGasAlarms = m_dicCoGasAlarms;
            else if (nGasType == (int)SafetyChecker.DangerType.METHANE_ALARM)
                dicGasAlarms = m_dicMethaneGasAlarms;
            else
                return false;

            if (dicGasAlarms.ContainsKey(strSensorID))
            {
                dicGasAlarms.Remove(strSensorID);
                return true;
            }

            return false;
        }

        public DangerState RemoveAlarm(int nAlarmHistoryID, string strLoginUserID)
        {
            foreach (KeyValuePair<DataWorker, ArrayList> pair in m_dicWorkerAlarms)
            {
                foreach (DangerState state in pair.Value)
                {
                    if (state.AlarmHistoryID == nAlarmHistoryID)
                    {
                        state.EventTime = DateTime.Now;
                        pair.Value.Remove(state);
                        DeleteAlarm(state, null, strLoginUserID);
                        return state;
                    }
                }
            }

            return null;
        }

        public void RemoveAlarm(DataCar car, ArrayList arrBytes)
        {
            foreach (KeyValuePair<DataWorker, ArrayList> pair in m_dicWorkerAlarms)
            {
                ArrayList arrAlarmHistoryIDs = new ArrayList();
                ArrayList arrRemove = new ArrayList();

                foreach (DangerState state in pair.Value)
                {
                    if (state.TargetCar == car)
                        arrRemove.Add(state);
                }

                foreach (DangerState state in arrRemove)
                {
                    pair.Value.Remove(state);

                    // 같은 AlarmHistoryID에 대하여 중복된 처리를 하지 않도록 한다.
                    if (arrAlarmHistoryIDs.Contains(state.AlarmHistoryID))
                        continue;

                    DeleteAlarm(state, arrBytes);
                    arrAlarmHistoryIDs.Add(state.AlarmHistoryID);
                }
            }
        }

        public void RemoveAlarm(DataEquip equip, ArrayList arrBytes)
        {
            foreach (KeyValuePair<DataWorker, ArrayList> pair in m_dicWorkerAlarms)
            {
                ArrayList arrAlarmHistoryIDs = new ArrayList();
                ArrayList arrRemove = new ArrayList();

                foreach (DangerState state in pair.Value)
                {
                    if (state.TargetEquipment == equip)
                        arrRemove.Add(state);
                }

                foreach (DangerState state in arrRemove)
                {
                    pair.Value.Remove(state);

                    // 같은 AlarmHistoryID에 대하여 중복된 처리를 하지 않도록 한다.
                    if (arrAlarmHistoryIDs.Contains(state.AlarmHistoryID))
                        continue;

                    DeleteAlarm(state, arrBytes);
                    arrAlarmHistoryIDs.Add(state.AlarmHistoryID);
                }
            }
        }

        public void RemoveAlarm(DataWorker worker, ArrayList arrBytes)
        {
            if (m_dicWorkerAlarms.ContainsKey(worker))
            {
                ArrayList arrAlarmHistoryIDs = new ArrayList();
                ArrayList arrStates = m_dicWorkerAlarms[worker];

                foreach (DangerState state in arrStates)
                {
                    // 같은 AlarmHistoryID에 대하여 중복된 처리를 하지 않도록 한다.
                    if (arrAlarmHistoryIDs.Contains(state.AlarmHistoryID))
                        continue;

                    DeleteAlarm(state, arrBytes);
                    arrAlarmHistoryIDs.Add(state.AlarmHistoryID);
                }

                m_dicWorkerAlarms.Remove(worker);
            }
        }

        public void RemoveAlarm(DataWorker worker, DangerState state, ArrayList arrBytes)
        {
            ArrayList arrStates = null;

            if (m_dicWorkerAlarms.ContainsKey(worker))
                arrStates = m_dicWorkerAlarms[worker];
            else
                return;

            foreach (DangerState _state in arrStates)
            {
                if (_state.TargetCar == state.TargetCar &&
                    _state.TargetEquipment == state.TargetEquipment &&
                    _state.TargetZone == state.TargetZone)
                {
                    DeleteAlarm(state, arrBytes);
                    arrStates.Remove(_state);
                    return;
                }
            }
        }

        public int GetAlarmWorkerCount()
        {
            return m_dicWorkerAlarms.Count;
        }

        public ArrayList GetAlarms(int nIndex)
        {
            if (nIndex < 0 || nIndex >= GetAlarmWorkerCount())
                return null;

            KeyValuePair<DataWorker, ArrayList> pair = m_dicWorkerAlarms.ElementAt(nIndex);
            return pair.Value;
        }

        public void LoadDB()
        {
            int nSiteID = NetworkServer.Instance.SiteID;
            string strSQL = "Select WorkerID, TargetSensorID, TargetZoneID, BeginTime, TargetType from IgnoreAlarm where SiteID = " + nSiteID;

            DBConn dbMgr = NetworkServer.Instance.DBManager;
            SqlConnection connection = dbMgr.Connect();

            SqlDataReader reader = dbMgr.ExecuteReader(strSQL, connection);

            while (reader.Read())
            {
                int nWorkerID = (int)reader[0];
                string strTargetSensorID = reader.IsDBNull(1) ? "" : (string)reader[1];
                int nTargetZoneID = reader.IsDBNull(2) ? 0 : (int)reader[2];
                DateTime dtBegin = (DateTime)reader[3];
                int nType = (int)reader[4];

                DataManager dataMgr = NetworkServer.Instance.DataManager;

                DataWorker worker = dataMgr.GetWorkerFromID(nWorkerID);

                if (worker == null)
                    continue;

                DataZone zoneTarget = null;
                bool isValidData = false;                

                if (nType == (int)NetworkClient.ObjectType.VEHICLE && strTargetSensorID.Length > 0)
                {
                    DataCar car = dataMgr.FindCar2(strTargetSensorID);
                    isValidData = car != null;
                }
                else if (nType == (int)NetworkClient.ObjectType.EQUIPMENT && strTargetSensorID.Length > 0)
                {
                    DataEquip equip = dataMgr.FindEquip2(strTargetSensorID);
                    isValidData = equip != null;
                }
                else if (nType == (int)NetworkClient.ObjectType.ZONE && nTargetZoneID > 0)
                {
                    DataZone zone = dataMgr.FindZone(nTargetZoneID);

                    if (zone != null)
                    {
                        zoneTarget = zone;
                        isValidData = true;
                    }
                    /*foreach (DataZone zone in dataMgr.DataZones)
                    {
                        if (zone.ID == nTargetZoneID)
                        {
                            zoneTarget = zone;
                            isValidData = true;
                            break;
                        }
                    }*/
                }

                if (!isValidData)
                    continue;

                IgnoreAlarmInfo info = new IgnoreAlarmInfo();

                info.BeginIgnoreTime = dtBegin;
                info.TargetSensorID = strTargetSensorID;
                info.TargetType = (NetworkClient.ObjectType)nType;
                info.TargetZone = zoneTarget;
                info.Worker = worker;

                if (m_dicIgnoreAlarmInfo.ContainsKey(worker))
                {
                    ArrayList arrIgnores = m_dicIgnoreAlarmInfo[worker];
                    arrIgnores.Add(info);
                }
                else
                {
                    ArrayList arrIgnores = new ArrayList();
                    arrIgnores.Add(info);
                    m_dicIgnoreAlarmInfo[worker] = arrIgnores;
                }
            }
        }

        public void PostProcessChangeWorker()
        {
            DataManager dataMgr = NetworkServer.Instance.DataManager;
            ArrayList arrRemove = new ArrayList();

            foreach (KeyValuePair<DataWorker, ArrayList> pair in m_dicIgnoreAlarmInfo)
            {
                if (dataMgr.GetWorkerFromID(pair.Key.ID) == null)
                {
                    arrRemove.Add(pair.Key);
                }
            }

            foreach (DataWorker worker in arrRemove)
            {
                RemoveIgnoreAlarms(worker);
            }
        }

        public void PostProcessChangeCar()
        {
            DataManager dataMgr = NetworkServer.Instance.DataManager;
            ArrayList arrRemove = new ArrayList();

            foreach (KeyValuePair<DataWorker, ArrayList> pair in m_dicIgnoreAlarmInfo)
            {
                foreach (IgnoreAlarmInfo info in pair.Value)
                {
                    if (info.TargetType == NetworkClient.ObjectType.VEHICLE)
                    {
                        if (dataMgr.FindCar2(info.TargetSensorID) == null)
                        {
                            arrRemove.Add(info);
                        }
                    }
                }
            }

            foreach (IgnoreAlarmInfo info in arrRemove)
            {
                RemoveIgnoreAlarm(info);
            }
        }

        public void PostProcessChangeEquip()
        {
            DataManager dataMgr = NetworkServer.Instance.DataManager;
            ArrayList arrRemove = new ArrayList();

            foreach (KeyValuePair<DataWorker, ArrayList> pair in m_dicIgnoreAlarmInfo)
            {
                foreach (IgnoreAlarmInfo info in pair.Value)
                {
                    if (info.TargetType == NetworkClient.ObjectType.EQUIPMENT)
                    {
                        if (dataMgr.FindEquip2(info.TargetSensorID) == null)
                        {
                            arrRemove.Add(info);
                        }
                    }
                }
            }

            foreach (IgnoreAlarmInfo info in arrRemove)
            {
                RemoveIgnoreAlarm(info);
            }
        }

        public void PostProcessChangeZone()
        {
            DataManager dataMgr = NetworkServer.Instance.DataManager;
            ArrayList arrRemove = new ArrayList();

            foreach (KeyValuePair<DataWorker, ArrayList> pair in m_dicIgnoreAlarmInfo)
            {
                foreach (IgnoreAlarmInfo info in pair.Value)
                {
                    if (info.TargetType == NetworkClient.ObjectType.ZONE && info.TargetZone != null)
                    {
                        if (dataMgr.FindZone(info.TargetZone.ID) == null)
                        {
                            arrRemove.Add(info);
                        }
                    }
                }
            }

            foreach (IgnoreAlarmInfo info in arrRemove)
            {
                RemoveIgnoreAlarm(info);
            }
        }

        public void Reload()
        {
            DataManager dataMgr = NetworkServer.Instance.DataManager;

            ArrayList arrWorkers = (ArrayList)dataMgr.GetWorkers().Clone();
            int nWorkerCount = arrWorkers.Count;

            Dictionary<DataWorker, ArrayList> dicInsert = new Dictionary<DataWorker, ArrayList>();

            foreach (KeyValuePair<DataWorker, ArrayList> pair in m_dicWorkerAlarms)
            {
                for (int j = 0; j < nWorkerCount; j++)
                {
                    DataWorker worker = (DataWorker)arrWorkers[j];

                    if (pair.Key.ID == worker.ID)
                    {
                        dicInsert[worker] = pair.Value;
                        arrWorkers.RemoveAt(j);
                        nWorkerCount--;
                        break;
                    }
                }
            }

            m_dicWorkerAlarms.Clear();

            foreach (KeyValuePair<DataWorker, ArrayList> pair in dicInsert)
            {
                m_dicWorkerAlarms[pair.Key] = pair.Value;
            }
        }
    }
}
