using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Collections;
using System.Data.SqlClient;
using HSMS;

namespace HSMSServer2
{
    public class ClientDataHSMS : ClientData
    {        
        private ProcessLogin m_ProcessLogin = null;
        public ClientDataHSMS(ServiceProvider provider)
        {
            m_provider = provider;
            Type = ClientType.HSMS_CLIENT;
            m_ProcessLogin = new ProcessLogin(provider);
        }

        private string GetLoginID(byte[] bytes, out int nUserID)
        {
            nUserID = -1;
            string strUserID = "";

            ArrayList arrDatas = ServiceProvider.ReadBytes(bytes);

            if (arrDatas == null)
                return strUserID;

            if (arrDatas.Count >= 2)
            {
                try
                {
                    nUserID = (int)arrDatas[0];
                    strUserID = (string)arrDatas[1];
                }
                catch (Exception)
                {
                }
            }

            /*int nLen = bytes.Length;

            if (nLen > 4)
            {
                int nDataLength = BitConverter.ToInt32(bytes, 1);

                if (nDataLength < 0)
                    return null;
                else if (nDataLength > 0)
                {
                    if (nLen < 5 + nDataLength)
                        return null;

                    string strData = Encoding.UTF8.GetString(bytes, 5, nDataLength);
                    return strData;
                }
            }*/

            return strUserID;
        }

        // OnAccept() 이후 WhoIAm을 받은 뒤 처리해야 할 로직
        protected override bool ProcessFirstConnection(ConnectionState state)
        {
            int nUserID;
            string strLoginID = GetLoginID(ReceivedData, out nUserID);

            if (m_ProcessLogin != null && nUserID > 0)
            {
                if (m_ProcessLogin.ProcessFirstConnection(state, nUserID, strLoginID))
                    ProcessFirstLogin(state);
            }

            m_bLoginUser = true;

            return true;
        }

        // 사용자 로그인 이후에 전송할 데이터
        private bool ProcessFirstLogin(ConnectionState state)
        {
            m_bLoginUser = true;
            //string strLoginID = GetLoginID();

            //string szUserID = NetworkServer.Instance.GetLoginUserID(state);
            //if (szUserID != null)
             //   return false;

            //if (strLoginID != null)
            //    NetworkServer.Instance.SetLoginUserID(state, strLoginID);

            ArrayList arrLastSensorDatas = new ArrayList();
            SafetyChecker.Instance.GetLastSensorDatas(arrLastSensorDatas);

            if (arrLastSensorDatas.Count > 0)
            {
                byte[] bytes = ServiceProvider.MakeBytes(TCP_ID.SENSOR_DATA_LIST, arrLastSensorDatas);
                NetworkServer.Instance.ServiceProvider.Send(bytes, 0, bytes.Length, state);
            }

            // 현재 진행중인 알람 리스트를 전송한다.
            Dictionary<object, UnE.Geometry.Vertex2D> dicSensorDatas = SendAlarmProcessHistoryList(state);

            // 현재 진행중인 알람 리스트와 관련된 센서들의 마지막 좌표 위치를 전송한다.
            /*if (dicSensorDatas != null)
                SendSensorDatas(dicSensorDatas, state);*/

            // 마지막 DB읽은 시간을 전송한다.
            SendLastDBAccess(state);



            return true;
        }

        private string GetLoginID()
        {
            byte[] bytesParameter = ReceivedData;

            if (bytesParameter.Length < 5)
                return null;

            if (bytesParameter[0] != TCP_TYPE.STRING)
                return null;

            int nDataLength = BitConverter.ToInt32(bytesParameter, 1);

            if (bytesParameter.Length < 5 + nDataLength)
                return null;

            string strLoginID = Encoding.UTF8.GetString(bytesParameter, 5, nDataLength);
            return strLoginID;
        }

        /*private void SendSensorDatas(Dictionary<object, UnE.Geometry.Vertex2D>  dicSensorDatas, ConnectionState state)
        {
            ArrayList arrDatas = new ArrayList();

            Type typeWorker = typeof(DataWorker);
            Type typeCar = typeof(DataCar);
            Type typeEquip = typeof(DataEquip);

            foreach (KeyValuePair<object, UnE.Geometry.Vertex2D> pair in dicSensorDatas)
            {
                if (pair.Key.GetType() == typeWorker)
                {
                    DataWorker worker = (DataWorker)pair.Key;

                    arrDatas.Add((int)NetworkClient.ObjectType.WORKER);
                    arrDatas.Add(worker.ID);
                    arrDatas.Add(worker.Sensor);
                    arrDatas.Add(pair.Value.x);
                    arrDatas.Add(pair.Value.y);
                }
            }

            byte[] bytes = ServiceProvider.MakeBytes(TCP_ID.SENSOR_DATA_LIST, arrDatas);
            m_provider.Send(bytes, 0, bytes.Length, state);
        }*/

        private Dictionary<object, UnE.Geometry.Vertex2D> SendAlarmProcessHistoryList(ConnectionState state)
        {
            AlarmManager alarmMgr = NetworkServer.Instance.AlarmManager;
            int nWorkerCount = alarmMgr.GetAlarmWorkerCount();

            Dictionary<object, UnE.Geometry.Vertex2D> dicSensorDatas = new Dictionary<object, UnE.Geometry.Vertex2D>();
            ArrayList arrDatas = new ArrayList();

            for (int i = 0; i < nWorkerCount; i++)
            {
                try
                {
                    ArrayList arrStates = alarmMgr.GetAlarms(i);

                    foreach (DangerState _state in arrStates)
                    {
                        arrDatas.Add(_state.AlarmProcessHistoryID);
                        arrDatas.Add(_state.AlarmHistoryID);
                        arrDatas.Add(_state.EventTime);
                        arrDatas.Add((int)_state.AlarmStatus);
                        arrDatas.Add(_state.Distance);
                        arrDatas.Add(_state.AlarmStatusMessage);
                        arrDatas.Add(_state.AlarmMessage);
                        arrDatas.Add(_state.ShortAlarmMessage);
                        arrDatas.Add(_state.IsCritical);

                        AddSensorData(_state, dicSensorDatas);
                    }
                }
                catch (Exception)
                {

                }
            }

            if (arrDatas.Count > 0)
            {
                byte[] bytes = ServiceProvider.MakeBytes(TCP_ID.ALARM_PROCESS_HISTORY_LIST, arrDatas);
                m_provider.Send(bytes, 0, bytes.Length, state);
            }

            return dicSensorDatas;
        }

        private void AddSensorData(DangerState state, Dictionary<object, UnE.Geometry.Vertex2D> dicSensorDatas)
        {
            AddSensorData(state.Worker, state.Worker.Sensor, dicSensorDatas);
            
            object obj = null;
            string strSensorID = "";

            if (state.TargetCar != null)
            {
                obj = state.TargetCar;
                strSensorID = state.TargetCar.Sensor;
            }
            else if (state.TargetEquipment != null)
            {
                obj = state.TargetEquipment;
                strSensorID = state.TargetEquipment.Sensor;
            }

            if (obj != null && strSensorID.Length > 0)
            {
                AddSensorData(obj, strSensorID, dicSensorDatas);
            }
        }

        private void AddSensorData(object obj, string strSensorID, Dictionary<object, UnE.Geometry.Vertex2D> dicSensorDatas)
        {
            LinkedList<EventSensorData> arrEventDatas = SafetyChecker.Instance.FindSensorHistory(strSensorID);

            if (arrEventDatas != null)
            {
                int nDataCount = arrEventDatas.Count;

                if (nDataCount > 0)
                {
                    EventSensorData data = arrEventDatas.Last.Value;
                    dicSensorDatas[obj] = new UnE.Geometry.Vertex2D(data.X, data.Y);
                }
            }
        }

        /// <summary>
        /// 마지막으로 DB가 없데이트 된 시간을 전송
        /// </summary>
        /// <param name="state"></param>
        private void SendLastDBAccess(ConnectionState state)
        {
            ArrayList arDatas = new ArrayList();
            arDatas.Add(ProxyHSMS.LastDBAccess);

            byte[] bytes = ServiceProvider.MakeBytes(TCP_ID.CHANGE_DB_TIME, arDatas);
            m_provider.Send(bytes, 0, bytes.Length, state, true);
        }

        /// <summary>
        /// Client로부터 데이터를 전송 받았을때 처리하는 함수
        /// </summary>
        /// <param name="state">전송 Client</param>
        /// <param name="bytes">전송받은 데이터  ,bytes는 length byte가 제거되었음</param>
        /// <param name="nHeader">데이터 헤더</param>
        /// <param name="arrDatas">데이터 ArrayList</param>
        /// <returns></returns>
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            // 로그인된 유저만 DB 데이터 리스트를 받는다.
            if (m_bLoginUser == true)
            {
                if (nHeader == TCP_ID.CHANGE_DB_DATA)
                {
                    byte[] editBytes = ProcessChangeDBData(state, arrDatas, bytes);
                    SendChangeDBData(state, editBytes);
                }
                else if (nHeader == TCP_ID.CHANGE_DB_DATA_LIST)
                {
                    byte[] editBytes = ProcessChangeDBDataList(state, arrDatas, bytes);
                    SendChangeDBDataList(state, editBytes);
                }
                else if (nHeader == TCP_ID.FINISH_ALARM)
                {
                    ProcessFinishAlarm(state, arrDatas);
                }
                else if (nHeader == TCP_ID.FINISH_GAS_ALARM)
                {
                    ProcessFinishGasAlarm(state, arrDatas);
                }
            }

            if (nHeader >= TCP_ID.LOGIN_USER && nHeader <= TCP_ID.DELETE_USER)
            {
                m_ProcessLogin.ProcessLoginData(state, nHeader, arrDatas);
            }

            if (nHeader == TCP_ID.LOGIN_USER)
            {
                int nID = -1;
                int nUserLevel = -1;
                string szUserID = "";
                int nSiteID;

                LoginUserResult result = m_ProcessLogin.LoginUser(arrDatas, out nID, out szUserID, out nUserLevel, out nSiteID);
                
                if (result == LoginUserResult.SUCCESS)
                {
                    if (!LoginManager.Instance.IsValidUser(nID, nSiteID))
                    {
                        m_ProcessLogin.SendRejectLogin(state, LoginUserResult.INVALID_ID);
                        return true;
                    }

                    if (LoginManager.Instance.IsLoginUser(szUserID))
                    {
                        // 중복 로그인 경우
                        m_ProcessLogin.SendRejectLogin(state, LoginUserResult.DUPLICATE_LOGIN);
                        return true;
                    }

                    m_ProcessLogin.SendAcceptLogin(state, nID, szUserID, nUserLevel);

                    LoginInfo login = LoginManager.Instance.FindLoginUser(nID);

                    LoginManager.Instance.AddUser(state, login);

                    

                    ProcessFirstLogin(state);
                }
                else
                {
                    // id, pass가 다른 경우
                    m_ProcessLogin.SendRejectLogin(state, result);
                }
            }
            else if (nHeader == TCP_ID.LOGOUT_USER)
            {
                m_bLoginUser = false;  
                // 로그인 해제
                LoginManager.Instance.RemoveClient(state);
                m_ProcessLogin.SendLogout(state);
            }

            return true;
        }

        private void ProcessFinishGasAlarm(ConnectionState state, ArrayList arrDatas)
        {
            if (arrDatas.Count != 2)
                return;

            if ((arrDatas[0] is string) && (arrDatas[1] is int))
            {
                string strSensorID = (string)arrDatas[0];
                int nGasType = (int)arrDatas[1];

                NetworkServer.Instance.AlarmManager.RemoveGasAlarm(strSensorID, nGasType);
            }
        }

        private void ProcessFinishAlarm(ConnectionState state, ArrayList arrDatas)
        {
            if (arrDatas.Count == 0)
                return;

            int nAlarmHistoryID = (int)arrDatas[0];

            AlarmManager alarmMgr = NetworkServer.Instance.AlarmManager;

            string strLoginUserID = NetworkServer.Instance.GetLoginUserID(state);
            DangerState dangerState = alarmMgr.RemoveAlarm(nAlarmHistoryID, strLoginUserID);

            if (dangerState != null)
            {
                string strTargetSensorID = "";
                NetworkClient.ObjectType targetType = NetworkClient.ObjectType.ZONE;

                if (dangerState.TargetCar != null)
                {
                    strTargetSensorID = dangerState.TargetCar.Sensor;
                    targetType = NetworkClient.ObjectType.VEHICLE;
                }
                else if (dangerState.TargetEquipment != null)
                {
                    strTargetSensorID = dangerState.TargetEquipment.Sensor;
                    targetType = NetworkClient.ObjectType.EQUIPMENT;
                }
                else if (dangerState.TargetZone == null)
                    return;

                alarmMgr.SetIgnoreAlarm(dangerState.Worker, strTargetSensorID, dangerState.TargetZone, dangerState.EventTime, targetType);
            }
        }

        private void SendChangeDBData(ConnectionState state, byte[] bytes)
        {
            if (m_provider != null && bytes != null && bytes.Length > 0)
                m_provider.SendClientData(bytes, ClientType.HSMS_CLIENT, true);
        }

        private void SendChangeDBDataList(ConnectionState state, byte[] bytes)
        {
            if (m_provider != null && bytes != null && bytes.Length > 0)
                m_provider.SendClientData(bytes, ClientType.HSMS_CLIENT, true);
        }
               
        private byte[] ProcessChangeDBData(ConnectionState state, ArrayList arrDatas, byte[] bytes)
        {
            ChangeDataType nType = (ChangeDataType)arrDatas[0];

            byte[] result = bytes;
            switch (nType)
            {
                case ChangeDataType.WORKER:
                    result = EditWorker.ProcessChangeWorker(state, arrDatas, bytes);
                    NetworkServer.Instance.AlarmManager.PostProcessChangeWorker();
                    break;
                case ChangeDataType.CAR:
                    result = EditCar.ProcessChangeCar(state, arrDatas, bytes);
                    NetworkServer.Instance.AlarmManager.PostProcessChangeCar();
                    break;
                
                /*case ChangeDataType.EQUIP:
                    result = EditEquip.ProcessChangeEquip(state, arrDatas, bytes);
                    NetworkServer.Instance.AlarmManager.PostProcessChangeEquip();
                    break;*/
                case ChangeDataType.SENSORDETECT:
                    result = EditIgnoreDetect.ProcessChangeIgnoreDetect(state, arrDatas, bytes);
                    break;
                case ChangeDataType.SMSCONFIG:
                    result = EditSMSConfig.ProcessChangeConfige(state, arrDatas, bytes);
                    break;
                case ChangeDataType.ALARM_IGNORE_OPTIONS:
                    EditAlarm.ProcessAlarmIgnoreOptions(state, arrDatas);
                    break;
                case ChangeDataType.ZONELEVEL:
                    result = EditZone.ProcessChangeZone(state, arrDatas, bytes);
                    NetworkServer.Instance.AlarmManager.PostProcessChangeZone();
                    break;
                case ChangeDataType.MANAGER:
                    result = EditManager.ProcessChangeManager(state, arrDatas, bytes);
                    break;
                case ChangeDataType.CHANGE_ZONE_GROUP:
                    EditZone.ProcessChangeZoneGroup(arrDatas, bytes);
                    break;
            }
            return result;
        }

        private byte[] ProcessChangeDBDataList(ConnectionState state, ArrayList arrDatas, byte[] bytes)
        {
            if (arrDatas.Count == 0)
                return null;

            bool isChanged = false;
            ChangeDataType nType = (ChangeDataType)arrDatas[0];

            switch (nType)
            {
                case ChangeDataType.IGNORE_SENSORS_TO_WORKER:
                    if (EditIgnoreSensorsToWorker.ProcessChangeDataList(arrDatas))
                        isChanged = true;
                    break; 
                case ChangeDataType.WORKER:
                    if (EditWorker.ProcessChangeDataList(arrDatas))
                        isChanged = true;
                    break;

                case ChangeDataType.CAR:
                    if (EditCar.ProcessChangeDataList(arrDatas))
                        isChanged = true;
                    break;

                case ChangeDataType.EQUIP:
                    if (EditEquip.ProcessChangeDataList(arrDatas))
                        isChanged = true;
                    break;

                case ChangeDataType.ALARM_DISTANCE:
                    if (EditEquip.ProcessChangeAlarmDistance(arrDatas))
                        isChanged = true;
                    break;

                case ChangeDataType.EDIT_EQUIPMENT:
                    if (EditEquip.ProcessChangeDataList2(arrDatas))
                        isChanged = true;
                    break;
            }

            if (isChanged)
               bytes = ServiceProvider.MakeBytes(TCP_ID.CHANGE_DB_DATA_LIST, arrDatas);

            return bytes;
        }
    }
}
