using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace HSMS
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

        // Key : Sensor ID
        private Dictionary<string, DangerState> m_dicMethaneGasAlarms = new Dictionary<string, DangerState>();
        // Key : Sensor ID
        private Dictionary<string, DangerState> m_dicCoGasAlarms = new Dictionary<string, DangerState>();
        // DataWorker, DangerState List
        private Dictionary<DataWorker, ArrayList> m_dicWorkerAlarms = new Dictionary<DataWorker, ArrayList>();
        // AlarmHistory ID, Alarm
        private Dictionary<int, DangerState> m_dicAlarms = new Dictionary<int, DangerState>();
        // AlarmHistory ID, CCTV Image Path
        private Dictionary<int, string> m_dicAlarmCCTVImages = new Dictionary<int, string>();

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

        // 화면에 보여지고 있는 알람
        //private DangerState m_stateScreen = null;
        
        public void AddGasAlarm(string strSensorID, int nGasType, double dGas, int nAlarmHistoryID, int nAlarmProcessHistoryID, string strStatus, string strMessage)
        {
            string strGasName = "";
            Dictionary<string, DangerState> dicGasAlarms = null;

            if (nGasType == (int)SafetyChecker.DangerType.CO_GAS_ALARM)
            {
                strGasName = "일산화탄소";
                dicGasAlarms = m_dicCoGasAlarms;
            }
            else if (nGasType == (int)SafetyChecker.DangerType.METHANE_ALARM)
            {
                strGasName = "메탄가스";
                dicGasAlarms = m_dicMethaneGasAlarms;
            }
            else
            {
                return;
            }

            DangerState state;

            if (!dicGasAlarms.TryGetValue(strSensorID, out state))
            {
                state = new DangerState();
                state.EventTime = DateTime.Now;
                state.Type = (SafetyChecker.DangerType)nGasType;

                dicGasAlarms[strSensorID] = state;
            }

            state.Distance = dGas;
            state.AlarmHistoryID = nAlarmHistoryID;
            state.AlarmProcessHistoryID = nAlarmProcessHistoryID;

            string strSensorOwner = GetSensorOwner(strSensorID);

            if (strSensorOwner == null)
                return;

            FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                FormMain.Instance.SetAlarm(true, "", strStatus, strMessage, "", state);
                //string strAlarmMessage = string.Format("{0} 주변에서 {1:F1}ppm의 {2} 누출이 감지되었습니다.", strSensorOwner, dGas, strGasName);
                //FormMain.Instance.SetAlarm(true, "", strGasName + " 누출", strAlarmMessage, "", state);
            });
        }

        private string GetSensorOwner(string strSensorID)
        {
            string strSensorOwner = "";
            DataWorker worker = FormMain.Instance.DataMgr.FindWorker2(strSensorID);

            if (worker != null)
                strSensorOwner = worker.Name;
            else
            {
                DataCar car = FormMain.Instance.DataMgr.FindCar2(strSensorID);

                if (car != null)
                    strSensorOwner = car.Name;
                else
                {
                    DataEquip equip = FormMain.Instance.DataMgr.FindEquip2(strSensorID);

                    if (equip != null)
                        strSensorOwner = equip.Name;
                    else
                    {
                        GasSensor gasSensor = FormMain.Instance.DataMgr.FindGasSensor(strSensorID);

                        if (gasSensor != null)
                            strSensorOwner = gasSensor.SensorName;
                        else
                            return null;
                    }
                }
            }

            return strSensorOwner;
        }

        public void AddAlarm(DataWorker worker, DangerState state)
        {
            if (m_dicAlarms.ContainsKey(state.AlarmHistoryID))
            {
                // 같은 알람이 존재할 경우 상태값만 바꾼다.
                DangerState _state = m_dicAlarms[state.AlarmHistoryID];

                if (_state.Worker != state.Worker)
                {
                    RemoveAlarm(_state.Worker);
                }

                _state.TargetCar = state.TargetCar;
                _state.TargetEquipment = state.TargetEquipment;
                _state.TargetZone = state.TargetZone;
                _state.Type = state.Type;
                _state.Worker = state.Worker;
            }
            else
            {
                ArrayList arrStates = null;

                if (m_dicWorkerAlarms.ContainsKey(worker))
                {
                    arrStates = m_dicWorkerAlarms[worker];
                }
                else
                {
                    arrStates = new ArrayList();
                    m_dicWorkerAlarms[worker] = arrStates;
                }

                arrStates.Add(state);
                m_dicAlarms[state.AlarmHistoryID] = state;
            }
        }

        public void AddAlarmCCTV(int nAlarmHistoryID, string strCCTVImagePath)
        {
            m_dicAlarmCCTVImages[nAlarmHistoryID] = strCCTVImagePath;

            DBConn dbMgr = FormMain.Instance.DataMgr.DBManager;
            SqlConnection connection = dbMgr.Connect();

            string strSQL = "Update AlarmHistory set Description = '" + strCCTVImagePath + "' where ID = " + nAlarmHistoryID.ToString();
            dbMgr.ExecuteSQL(strSQL, connection);

            connection.Close();
        }

        // 저장되어 있지 않은 Alarm은 DB로부터 읽어온다.
        private bool ReadAlarmFromDB(int nAlarmHistoryID)
        {
            DBConn dbMgr = FormMain.Instance.DataMgr.DBManager;
            SqlConnection connection = dbMgr.Connect();

            string strSQL = "Select WorkerMemberID, TargetSensorID, TargetZoneID, AlarmType from AlarmHistory where ID = " + nAlarmHistoryID.ToString();
            SqlDataReader reader = dbMgr.ExecuteReader(strSQL, connection);

            if (reader.Read())
            {
                string strMemberID = (string)reader[0];
                string strTargetSensorID = reader.IsDBNull(1) ? "" : (string)reader[1];
                string strTargetZoneID = reader.IsDBNull(2) ? "" : ((int)reader[2]).ToString();
                int nType = (int)reader[3];

                reader.Close();
                connection.Close();

                if (nType <= (int)SafetyChecker.DangerType.NONE || nType >= (int)SafetyChecker.DangerType.TYPE_COUNT)
                    return false;

                DataWorker worker = FormMain.Instance.DataMgr.FindWorker(strMemberID);

                if (worker == null)
                    return false;

                SafetyChecker.DangerType type = (SafetyChecker.DangerType)nType;
                DangerState state = ClientProvider.MakeAlarmHistory(nAlarmHistoryID, worker.ID, strTargetSensorID, strTargetZoneID, type);

                if (state == null)
                    return false;

                AddAlarm(state.Worker, state);
                return true;
            }

            reader.Close();
            connection.Close();
            return false;
        }

        public void AddAlarmProcess(DangerState state, AlarmManager.AlarmStatus status, string strAlarmStatus, string strAlarmMessage, string strShortAlarmMessage, bool isCritical)
        {
            if (!m_dicAlarms.ContainsKey(state.AlarmHistoryID))
            {
                // 저장되어 있지 않은 Alarm은 DB로부터 읽어온다.
                if (!ReadAlarmFromDB(state.AlarmHistoryID))
                    return;

                if (!m_dicAlarms.ContainsKey(state.AlarmHistoryID))
                    return;
            }

            DangerState _state = m_dicAlarms[state.AlarmHistoryID];

            if (_state.Worker == null)
                return;

            // 이미 처리된 알람인가?
            if (_state.AlarmProcessHistoryID == state.AlarmProcessHistoryID)
                return;

            string szTeamName = "";
            if (_state.Worker.Team != null)
                szTeamName = _state.Worker.Team.Name;
            
            string strWorkerInfo = _state.Worker.Company.CompanyName + " " + szTeamName + " " + _state.Worker.Name;

            
            //_state.Critical = state.Critical;

            if (status == AlarmStatus.ALARM_BEGINNING)
            {
                _state.AlarmProcessHistoryID = state.AlarmProcessHistoryID;
                _state.Distance = state.Distance;
                _state.EventTime = state.EventTime;

                FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    FormMain.Instance.SetAlarm(isCritical, strWorkerInfo, strAlarmStatus, strAlarmMessage, strShortAlarmMessage, _state);
                });
            }
            else if (status == AlarmStatus.ALARM_PROCESSING)
            {
                if (NeedUpdate(state))
                {
                    _state.AlarmProcessHistoryID = state.AlarmProcessHistoryID;
                    _state.Distance = state.Distance;
                    _state.EventTime = state.EventTime;
                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        FormMain.Instance.SetAlarm(isCritical, strWorkerInfo, strAlarmStatus, strAlarmMessage, strShortAlarmMessage, _state);
                    });
                }
            }
            else if (status == AlarmStatus.ALARM_FINISH || status == AlarmStatus.ALARM_FINISH_BY_USER)
            {
                _state.AlarmProcessHistoryID = state.AlarmProcessHistoryID;
                _state.Distance = state.Distance;
                _state.EventTime = state.EventTime;

                FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    FormMain.Instance.RemoveAlarm(_state);
                });
            }
        }

        private bool NeedUpdate(DangerState state)
        {
            if (state == null)
                return false;

            DangerState currentAlarm = FormMain.Instance.CurrentAlarm;

            if (currentAlarm == null)
                return true;

            return state.AlarmProcessHistoryID != currentAlarm.AlarmProcessHistoryID;
        }

        public bool GetAlarmMessage(DangerState state, DataWorker worker, out string strWorkerInfo, out string strAlarmStatus, out string strAlarmMessage, out string strShortAlarmMessage, out bool isCritical)
        {
            strWorkerInfo = strAlarmStatus = strAlarmMessage = strShortAlarmMessage = "";
            isCritical = false;

            if (worker == null)
            {
                string strGasName = "";
                Dictionary<string, DangerState> dicGasAlarms = null;

                if (state.Type == SafetyChecker.DangerType.CO_GAS_ALARM)
                {
                    strGasName = "일산화탄소";
                    dicGasAlarms = m_dicCoGasAlarms;
                }
                else if (state.Type == SafetyChecker.DangerType.METHANE_ALARM)
                {
                    strGasName = "메탄가스";
                    dicGasAlarms = m_dicMethaneGasAlarms;
                }
                else
                    return false;

                string strSensorID = null;

                foreach (KeyValuePair<string, DangerState> pair in dicGasAlarms)
                {
                    if (pair.Value == state)
                    {
                        strSensorID = pair.Key;
                        break;
                    }
                }

                if (strSensorID == null)
                    return false;

                string strSensorOwner = GetSensorOwner(strSensorID);

                if (strSensorOwner == null)
                    return false;

                isCritical = true;
                strWorkerInfo = "";
                strAlarmStatus = strGasName + " 누출";
                strShortAlarmMessage = "";
                strAlarmMessage = string.Format("{0} 주변에서 {1:F1}ppm의 {2} 누출이 감지되었습니다.", strSensorOwner, state.Distance, strGasName);
                return true;
            }

            isCritical = state.Distance <= 0.0;

            string szTeamName = "";
            if (worker.Team != null)
                szTeamName = worker.Team.Name;

            strWorkerInfo = worker.Company.CompanyName + " " + szTeamName + " " + worker.Name;
            strAlarmStatus = strAlarmMessage = strShortAlarmMessage = "";

            if (state.TargetCar != null)
            {
                if (isCritical)
                {
                    strAlarmStatus = "차량 충돌";
                    strAlarmMessage = "작업자(" + worker.Name + ")가 " + state.TargetCar.Name + " 차량과 충돌하였습니다.";
                    strShortAlarmMessage = worker.Name + " ↔ " + state.TargetCar.Name + " 충돌";
                }
                else
                {
                    strAlarmStatus = "차량 접근중";

                    if (state.Type == SafetyChecker.DangerType.CAR_TO_WORKER)
                    {
                        strAlarmMessage = string.Format("{0} 차량이 작업자({1})에게 {2:F0}m 이내로 접근중입니다",
                                state.TargetCar.Name, worker.Name, state.Distance);
                        strShortAlarmMessage = state.TargetCar.Name + " → " + worker.Name + string.Format(" {0:F0}m 이내로 접근중", state.Distance);
                    }
                    else if (state.Type == SafetyChecker.DangerType.WORKER_TO_CAR)
                    {
                        strAlarmMessage = string.Format("작업자({0})가 {1} 차량에게 {2:F0}m 이내로 접근중입니다",
                                worker.Name, state.TargetCar.Name, state.Distance);
                        strShortAlarmMessage = worker.Name + " → " + state.TargetCar.Name + string.Format(" {0:F0}m 이내로 접근중", state.Distance);
                    }
                    else if (state.Type == SafetyChecker.DangerType.CAR_TO_WORKER_BOTH)
                    {
                        strAlarmMessage = string.Format("작업자({0})와 {1} 차량이 {2:F0}m 이내로 서로 접근중입니다",
                                worker.Name, state.TargetCar.Name, state.Distance);
                        strShortAlarmMessage = worker.Name + " ↔ " + state.TargetCar.Name + string.Format(" {0:F0}m 이내로 접근중", state.Distance);
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
                    strShortAlarmMessage = worker.Name + " → " + state.TargetEquipment.Name + " 설비영역 진입";
                }
                else
                {
                    strAlarmStatus = "설비영역 접근중";
                    //strAlarmMessage = "작업자(" + worker.Name + ")가 " + state.TargetEquipment.Name + " 설비영역으로 접근중입니다.";
                    strAlarmMessage = string.Format("작업자({0})가 {1} 설비영역의 {2:F0}m 이내로 접근중입니다.",
                        worker.Name, state.TargetEquipment.Name, state.Distance);
                    strShortAlarmMessage = worker.Name + " → " + state.TargetEquipment.Name + string.Format(" 설비영역 {0:F0} 이내로 접근중", state.Distance);
                }
            }
            else if (state.TargetZone != null)
            {
                if (isCritical)
                {
                    strAlarmStatus = "접근금지영역 진입";
                    strAlarmMessage = "작업자(" + worker.Name + ")가 " + state.TargetZone.ZoneName + " 영역으로 진입하였습니다.";
                    strShortAlarmMessage = worker.Name + " → " + state.TargetZone.ZoneName + " 영역 진입";
                }
                else
                {
                    strAlarmStatus = "접근금지영역 접근중";
                    //strAlarmMessage = "작업자(" + worker.Name + ")가 " + state.TargetZone.ZoneName + " 영역으로 접근중입니다.";
                    strAlarmMessage = string.Format("작업자({0})가 {1} 영역의 {2:F0}m 이내로 접근중입니다.",
                        worker.Name, state.TargetZone.ZoneName, state.Distance);
                    strShortAlarmMessage = worker.Name + " → " + state.TargetZone.ZoneName + string.Format(" 영역 {0:F0}m 이내로 접근중", state.Distance);
                }
            }
            else
                return false;

            return true;
        }

        public ArrayList FindAlarms(DataWorker worker)
        {
            if (m_dicWorkerAlarms.ContainsKey(worker))
                return m_dicWorkerAlarms[worker];

            return null;
        }

        public void RemoveAlarmAt(int nIndex)
        {
            if (nIndex < 0 || nIndex >= GetAlarmWorkerCount())
                return;

            KeyValuePair<DataWorker, ArrayList> pair = m_dicWorkerAlarms.ElementAt(nIndex);
            RemoveAlarm(pair.Key);
        }

        public void RemoveGasAlarm(string strSensorID, int nGasType)
        {
            Dictionary<string, DangerState> dicGasAlarms = null;

            if (nGasType == (int)SafetyChecker.DangerType.CO_GAS_ALARM)
            {
                dicGasAlarms = m_dicCoGasAlarms;
            }
            else if (nGasType == (int)SafetyChecker.DangerType.METHANE_ALARM)
            {
                dicGasAlarms = m_dicMethaneGasAlarms;
            }
            else
                return;

            DangerState state;

            if (!dicGasAlarms.TryGetValue(strSensorID, out state))
                return;

            FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                FormMain.Instance.RemoveAlarm(state);
            });
        }

        public bool RemoveGasAlarm(DangerState state, out string strSensorID, out int nGasType)
        {
            foreach (KeyValuePair<string, DangerState> pair in m_dicCoGasAlarms)
            {
                if (pair.Value == state)
                {
                    strSensorID = pair.Key;
                    nGasType = (int)SafetyChecker.DangerType.CO_GAS_ALARM;
                    m_dicCoGasAlarms.Remove(pair.Key);
                    return true;
                }
            }

            foreach (KeyValuePair<string, DangerState> pair in m_dicMethaneGasAlarms)
            {
                if (pair.Value == state)
                {
                    strSensorID = pair.Key;
                    nGasType = (int)SafetyChecker.DangerType.METHANE_ALARM;
                    m_dicMethaneGasAlarms.Remove(pair.Key);
                    return true;
                }
            }

            strSensorID = "";
            nGasType = 0;
            return false;
        }

        public void RemoveAlarm(DataCar car, DataEquip equip, DataZone zone)
        {
            ArrayList arrRemove = new ArrayList();
            
            foreach (KeyValuePair<DataWorker, ArrayList> pair in m_dicWorkerAlarms)
            {
                ArrayList arrStates = pair.Value;

                foreach (DangerState state in arrStates)
                {
                    if (state.TargetCar == car && state.TargetEquipment == equip && state.TargetZone == zone)
                    {
                        arrRemove.Add(state);
                        FormMain.Instance.RemoveAlarm(state);
                        m_dicAlarms.Remove(state.AlarmHistoryID);
                    }
                }

                foreach (DangerState state in arrRemove)
                {
                    arrStates.Remove(state);
                }
            }
        }

        public void RemoveAlarm(DataWorker worker)
        {
            if (!m_dicWorkerAlarms.ContainsKey(worker))
                return;

            ArrayList arrStates = m_dicWorkerAlarms[worker];

            //DangerState statePrev = m_stateScreen;

            foreach (DangerState state in arrStates)
            {
                FormMain.Instance.RemoveAlarm(state);
                m_dicAlarms.Remove(state.AlarmHistoryID);
            }

            m_dicWorkerAlarms.Remove(worker);

            /*if (statePrev != FormMain.Instance.CurrentAlarm)
            {
                m_stateScreen = FormMain.Instance.CurrentAlarm;
                FormMain.Instance.RefreshMessage();
            }*/
        }

        public void RemoveAlarm(DataWorker worker, DangerState state)
        {
            ArrayList arrStates = null;

            if (m_dicWorkerAlarms.ContainsKey(worker))
                arrStates = m_dicWorkerAlarms[worker];
            else
                return;

            foreach (DangerState _state in arrStates)
            {
                if (_state.AlarmHistoryID == state.AlarmHistoryID)
                {
                    arrStates.Remove(_state);
                    m_dicAlarms.Remove(_state.AlarmHistoryID);

                    FormMain.Instance.RemoveAlarm(_state);
                    //m_stateScreen = FormMain.Instance.CurrentAlarm;

                    return;
                }
            }
        }

        // 첫번째 DangerState 객체를 찾아내어 화면에 보여준다.
        private DangerState SetFirstAlarm()
        {
            DataWorker newWorker;
            DangerState state = GetFirstState(out newWorker);

            if (state != null)
            {
                bool isCritical;
                string strWorkerInfo, strAlarmStatus, strAlarmMessage, strShortMessage;

                if (GetAlarmMessage(state, newWorker, out strWorkerInfo, out strAlarmStatus, out strAlarmMessage, out strShortMessage, out isCritical))
                {
                    //m_stateScreen = state;

                    // 기존 알람 대신 새로운 알람을 화면에 표시한다.
                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        FormMain.Instance.SetAlarm(isCritical, strWorkerInfo, strAlarmStatus, strAlarmMessage, strShortMessage, state);
                    });

                    return state;
                }
            }

            return null;
        }

        private DangerState GetFirstState(out DataWorker worker)
        {
            worker = null;

            foreach (KeyValuePair<DataWorker, ArrayList> pair in m_dicWorkerAlarms)
            {
                foreach (DangerState state in pair.Value)
                {
                    worker = pair.Key;
                    return state;
                }
            }

            return null;
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

        public void Reload()
        {
            DataManager dataMgr = FormMain.Instance.DataMgr;

            ArrayList arrWorkers = (ArrayList)dataMgr.GetWorkers().Clone();
            int nWorkerCount = arrWorkers.Count;

            Dictionary<DataWorker, ArrayList> dicInsert = new Dictionary<DataWorker,ArrayList>();

            foreach (KeyValuePair<DataWorker, ArrayList> pair in m_dicWorkerAlarms)
            {
                for (int j=0;j<nWorkerCount;j++)
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
