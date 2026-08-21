using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using System.Windows.Forms;
using IronPython;

namespace HSMS
{
    public class ClientProvider : TcpLib2.ClientServiceProvider
    {
        public enum ObjectType
        {
            NONE = 0,
            VEHICLE,
            EQUIPMENT,
            ZONE,
            WORKER,
            TYPE_COUNT
        };

        private NetworkManager m_mgr = null;
        private int m_nPingCount = 0;

        // 현재 OnReceive()에서 받은 데이터를 처리중인가?
        private bool m_isReadingProcess = false;

        public bool IsReadingProcess
        {
            get { return m_isReadingProcess; }
        }

        public int PingCount
        {
            get { return m_nPingCount; }
            set { m_nPingCount = value; }
        }

        public ClientProvider(NetworkManager mgr)
        {
            m_mgr = mgr;
            this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
            //this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            ScriptProxy proxy = ScriptProxy.Instance;
            proxy.UserObject.ClientProvider = this;
            
        }


        public override void OnReceiveData()
        {
            if (ReceivedData != null)
            {
                m_isReadingProcess = true;

                try
                {
                    _OnReceive(ReceivedData, false);
                }
                catch (Exception)
                {
                }
            }

            m_isReadingProcess = false;
        }

        private bool _OnReceive(byte[] bytes, bool checkValidation)
        {
            ArrayList arrDatas;

            int nHeader = GetHeader(bytes, out arrDatas);

            if (nHeader < 0)
                return false;
            else if (nHeader == 0)
                return true;

            bool bResult = OnReceive(bytes, nHeader, arrDatas);
            return bResult;
        }

        private bool OnReceive(byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            if (nHeader == TCP_ID.ARE_YOU_THERE)
                SendData(TCP_ID.I_AM_HERE);
            else if (nHeader == TCP_ID.WHO_ARE_YOU)
                SendWhoIam();
            else if (nHeader == TCP_ID.SENSOR_DATA)
                ProcessSensorData(arrDatas);
            else if (nHeader == TCP_ID.SENSOR_DATA_LIST)
                ProcessSensorDataList(arrDatas);
            else if (nHeader == TCP_ID.ALARM_HISTORY)
                ProcessAlarmHistory(arrDatas);
            else if (nHeader == TCP_ID.ALARM_PROCESS_HISTORY)
                ProcessAlarmProcessHistory(arrDatas);
            else if (nHeader == TCP_ID.ALARM_PROCESS_HISTORY_LIST)
                ProcessAlarmProcessHistoryList(arrDatas);
            else if (nHeader == TCP_ID.CHANGE_DB_DATA)
            {
                ProcessChangeDBData(arrDatas);
            }
            else if (nHeader == TCP_ID.CHANGE_DB_DATA_LIST)
            {
                ProcessChangeDBDataList(arrDatas);
            }
            else if (nHeader == TCP_ID.CHANGE_DB_TIME)
            {
                ProcessLastAccessTime(arrDatas);
            }
            else if (nHeader == TCP_ID.ACCEPT_LOGIN)
            {
                if (arrDatas.Count >= 3)
                {
                    int nUserID = (int)arrDatas[0];
                    string szUserID = (string)arrDatas[1];
                    int nUserLevel = (int)arrDatas[2];

                    LoginManager.Instance.OnAcceptLogin(nUserID, szUserID, nUserLevel);
                }
            }
            else if (nHeader == TCP_ID.REJECT_LOGIN)
            {
                int nRejectType = (int)arrDatas[0];
                LoginManager.Instance.OnRejectLogin(nRejectType);
            }
            else if (nHeader == TCP_ID.CHECK_LOGIN)
            {
                LoginManager.Instance.OnCheckLogin();
            }
            else if (nHeader == TCP_ID.LOGOUT_USER)
            {
                LoginManager.Instance.OnLogout();
            }
            else if (nHeader == TCP_ID.JOIN_USER)
            {
                int nUserID = (int)arrDatas[0];
                //int nGenUserID = BitConverter.ToInt32(bytes, 11);
                LoginManager.Instance.OnJoinUser(nUserID);
            }
            else if (nHeader == TCP_ID.CHNAGE_PASSWORD)// || nHeader == TCP_ID.SET_PASSWORD)
            {
                int nSuccess = BitConverter.ToInt32(bytes, 11);
                LoginManager.Instance.OnChangePassword(nSuccess);
            }
            /*else if( nHeader == TCP_ID.REQUEST_CODE)
            {
                string szCode = (string)arrDatas[0];
                LoginManager.Instance.OnResultCode(szCode);
            }*/
            else if (nHeader == TCP_ID.DELETE_USER)
            {
                if (arrDatas.Count >= 2)
                {
                    int nCode = (int)arrDatas[0];
                    string strUserID = (string)arrDatas[1];
                    LoginManager.Instance.OnDeleteUser(nCode, strUserID);
                }
            }
            else if (nHeader == TCP_ID.REMOVE_SENSORS)
                ProcessRemoveSensors(arrDatas);
            else if (nHeader == TCP_ID.GAS_ALARM)
                ProcessGasAlarm(arrDatas);
            else if (nHeader == TCP_ID.FINISH_GAS_ALARM)
                ProcessFinishGasAlarm(arrDatas);
            else
                return false;

            return true;
        }
        
        public void TestReload()
        {
            FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                FormMain.Instance.Reload();
            });

            ConnectionLogEx.Instance.WriteLine("Delete 3D Object");
            DataManager dataMgr = FormMain.Instance.DataMgr;
            PageBackstageHome.Instance.Invoke((MethodInvoker)delegate
            {
                FormContent contetnView = PageBackstageHome.Instance.ContentView;

                int nCount = dataMgr.GetCarCount();
                for (int j = 0; j < nCount; j++)
                {
                    DataCar car = dataMgr.GetCar(j);
                    contetnView.RemoveVehicle(car);
                }

                nCount = dataMgr.GetWorkerCount();
                for (int j = 0; j < nCount; j++)
                {
                    DataWorker car = dataMgr.GetWorker(j);
                    contetnView.RemoveWorker(car);
                }
            });

            ConnectionLogEx.Instance.WriteLine("DataBase Reloading");

            dataMgr.ReloadDBData();


            ConnectionLogEx.Instance.WriteLine("AlarmManager Reloading");
            AlarmManager alarmMgr = FormMain.Instance.AlarmManager;
            if (alarmMgr != null)
            {
                alarmMgr.Reload();
            }

            FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                FormMain.Instance.MakeSensorOwner();
            });
        }

        private void ProcessLastAccessTime(ArrayList arDatas)
        {
            DateTime dbAccessTime = (DateTime)arDatas[0];

            FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                DataManager dataManager = FormMain.Instance.DataMgr;
                TimeSpan span = dbAccessTime - dataManager.LastAccess;
                if (span.TotalMinutes > 0)
                {                   
                    // ToDo: UI초기값으로 설정

                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        FormMain.Instance.Reload();
                    });

                    ConnectionLogEx.Instance.WriteLine("Delete 3D Object");
                    DataManager dataMgr = FormMain.Instance.DataMgr;
                    PageBackstageHome.Instance.Invoke((MethodInvoker)delegate
                    {
                        FormContent contetnView = PageBackstageHome.Instance.ContentView;

                        int nCount = dataMgr.GetCarCount();
                        for (int j = 0; j < nCount; j++)
                        {
                            DataCar car = dataMgr.GetCar(j);
                            contetnView.RemoveVehicle(car);
                        }

                        nCount = dataMgr.GetWorkerCount();
                        for (int j = 0; j < nCount; j++)
                        {
                            DataWorker car = dataMgr.GetWorker(j);
                            contetnView.RemoveWorker(car);
                        }                            
                    });

                    ConnectionLogEx.Instance.WriteLine("DataBase Reloading");
                   
                    dataMgr.ReloadDBData();


                    ConnectionLogEx.Instance.WriteLine("AlarmManager Reloading");                    
                    AlarmManager alarmMgr = FormMain.Instance.AlarmManager;
                    if (alarmMgr != null)
                    {
                        alarmMgr.Reload();
                    }

                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        FormMain.Instance.MakeSensorOwner();
                    });
                    

                }
            }); 
        }

        private void ProcessChangeDBData(ArrayList arrDatas)
        {
            ChangeDataType nType = (ChangeDataType)arrDatas[0];

            switch (nType)
            {
                case ChangeDataType.WORKER:
                    ProcessChangeData.ProcessChangeWorker(arrDatas);
                    break;
                case ChangeDataType.CAR:
                    ProcessChangeData.ProcessChangeCar(arrDatas);
                    break;
                /*case ChangeDataType.EQUIP:
                    ProcessChangeData.ProcessChangeEquip(arrDatas);
                    break;*/
                case ChangeDataType.SMSCONFIG:
                    ProcessChangeData.ProcessChangeSMSConfige(arrDatas);
                    break;
                case ChangeDataType.ALARM_IGNORE_OPTIONS:
                    ProcessChangeData.ProcessAlarmIgnoreOptions(arrDatas);
                    break;
                case ChangeDataType.MANAGER:
                    ProcessChangeData.ProcessChnageManager(arrDatas);
                    break;
                case ChangeDataType.ZONELEVEL:
                    ProcessChangeData.ProcessChangeZone(arrDatas);
                    break;
                case ChangeDataType.CHANGE_ZONE_GROUP:
                    ProcessChangeData.ProcessChangeZoneGroup(arrDatas);
                    break;
            }
        }

        private void ProcessChangeDBDataList(ArrayList arrDatas)
        {
            ChangeDataType nType = (ChangeDataType)arrDatas[0];

            switch (nType)
            {
                case ChangeDataType.IGNORE_SENSORS_TO_WORKER:
                    ProcessChangeData.ProcessChangeIgnreToWorkerList(arrDatas);                     
                    break; 
                case ChangeDataType.WORKER:
                    EditWorker.ProcessChangeDataList(arrDatas);
                    break;
                case ChangeDataType.CAR:
                    EditCar.ProcessChangeDataList(arrDatas);
                    break;
                case ChangeDataType.EQUIP:
                    EditEquipment.ProcessChangeDataList(arrDatas);
                    break;
                case ChangeDataType.ALARM_DISTANCE:
                    EditEquipment.ProcessChangeAlarmDistance(arrDatas);
                    break;
                case ChangeDataType.EDIT_EQUIPMENT:
                    EditEquipment.ProcessChangeDataList2(arrDatas);
                    break;
            }
        }

        private void ProcessAlarmIgnoreOptions(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            for (int i = 1; i < nDataCount; i += 4)
            {
                ProcessAlarmIgnoreOption(arrDatas, i);
            }
        }

        private bool ProcessAlarmIgnoreOption(ArrayList arrDatas, int nIndex)
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

        private void ProcessAlarmProcessHistoryList(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount % 9 != 0)
                return;

            for (int i = 0; i < nDataCount; i += 9)
            {
                ProcessAlarmProcessHistory(arrDatas, i);
            }
        }

        private bool GetAlarmProcessHistoryData(ArrayList arrDatas, int nIndex, out int nAlarmHistoryID, out int nAlarmProcessHistoryID, out DateTime dtTime, out AlarmManager.AlarmStatus status, out double distance, out string strAlarmStatus, out string strAlarmMessage, out string strShortAlarmMessage, out bool isCritical)
        {
            nAlarmProcessHistoryID = nAlarmHistoryID = -1;
            dtTime = new DateTime();
            status = AlarmManager.AlarmStatus.NONE;
            strAlarmStatus = strAlarmMessage = strShortAlarmMessage = "";
            isCritical = false;
            distance = 0.0;

            int nDataCount = arrDatas.Count;

            if (nDataCount - nIndex < 8)
                return false;

            try
            {
                nAlarmProcessHistoryID = (int)arrDatas[nIndex];
                nAlarmHistoryID = (int)arrDatas[nIndex + 1];
                dtTime = (DateTime)arrDatas[nIndex + 2];
                int nStatus = (int)arrDatas[nIndex + 3];
                distance = (double)arrDatas[nIndex + 4];
                strAlarmStatus = (string)arrDatas[nIndex + 5];
                strAlarmMessage = (string)arrDatas[nIndex + 6];
                strShortAlarmMessage = (string)arrDatas[nIndex + 7];
                isCritical = (bool)arrDatas[nIndex + 8];

                if (nStatus <= (int)AlarmManager.AlarmStatus.NONE || nStatus >= (int)AlarmManager.AlarmStatus.TYPE_COUNT)
                    return false;

                status = (AlarmManager.AlarmStatus)nStatus;

                if (nAlarmHistoryID <= 0 || nAlarmProcessHistoryID <= 0)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private DangerState MakeAlarmProcessHistory(int nAlarmHistoryID, int nAlarmProcessHistoryID, DateTime dtTime, double distance, bool isCritical)
        {
            if (nAlarmHistoryID <= 0 || nAlarmProcessHistoryID <= 0)
                return null;

            DangerState state = new DangerState();

            state.AlarmHistoryID = nAlarmHistoryID;
            state.AlarmProcessHistoryID = nAlarmProcessHistoryID;
            state.Distance = distance;
            state.EventTime = dtTime;
            state.Critical = isCritical;

            return state;
        }

        private void ProcessAlarmProcessHistory(ArrayList arrDatas, int nIndex = 0)
        {
            int nAlarmProcessHistoryID, nAlarmHistoryID;
            DateTime dtTime;
            AlarmManager.AlarmStatus status;
            string strAlarmStatus, strAlarmMessage, strShortAlarmMessage;
            bool isCritical;
            double distance;

            if (!GetAlarmProcessHistoryData(arrDatas, nIndex, out nAlarmHistoryID, out nAlarmProcessHistoryID, out dtTime, out status, out distance, out strAlarmStatus, out strAlarmMessage, out strShortAlarmMessage, out isCritical))
                return;

            DangerState state = MakeAlarmProcessHistory(nAlarmHistoryID, nAlarmProcessHistoryID, dtTime, distance, isCritical);

            if (state == null)
                return;

            FormMain.Instance.AlarmManager.AddAlarmProcess(state, status, strAlarmStatus, strAlarmMessage, strShortAlarmMessage, isCritical);
        }

        private bool GetAlarmHistoryData(ArrayList arrDatas, out int nAlarmHistoryID, out int nWorkerID, out string strTargetSensorID, out string strTargetZoneID, out SafetyChecker.DangerType type)
        {
            nAlarmHistoryID = nWorkerID = -1;
            strTargetSensorID = strTargetZoneID = "";
            type = SafetyChecker.DangerType.NONE;

            int nDataCount = arrDatas.Count;

            if (nDataCount != 5)
                return false;

            try
            {
                nAlarmHistoryID = (int)arrDatas[0];
                nWorkerID = (int)arrDatas[1];
                strTargetSensorID = (string)arrDatas[2];
                strTargetZoneID = (string)arrDatas[3];
                int nType = (int)arrDatas[4];

                if (string.Compare(strTargetSensorID, "NULL", true) == 0)
                    strTargetSensorID = "";

                if (string.Compare(strTargetZoneID, "NULL", true) == 0)
                    strTargetZoneID = "";

                if (nType <= (int)SafetyChecker.DangerType.NONE || nType >= (int)SafetyChecker.DangerType.TYPE_COUNT)
                    return false;

                type = (SafetyChecker.DangerType)nType;

                if (nAlarmHistoryID <= 0 || nWorkerID <= 0)
                    return false;

                if (strTargetSensorID.Length == 0 && strTargetZoneID.Length == 0)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static DangerState MakeAlarmHistory(int nAlarmHistoryID, int nWorkerID, string strTargetSensorID, string strTargetZoneID, SafetyChecker.DangerType type)
        {
            DataWorker worker = FormMain.Instance.DataMgr.FindWorker(nWorkerID);

            if (worker == null)
                return null;

            DangerState state = null;

            if (strTargetSensorID.Length > 0)
            {
                if (type == SafetyChecker.DangerType.CAR_TO_WORKER ||
                    type == SafetyChecker.DangerType.CAR_TO_WORKER_BOTH ||
                    type == SafetyChecker.DangerType.WORKER_TO_CAR)
                {
                    DataCar car = FormMain.Instance.DataMgr.FindCar2(strTargetSensorID);

                    if (car == null)
                        return null;

                    state = new DangerState();
                    state.TargetCar = car;
                }
                else if (type == SafetyChecker.DangerType.WORKER_TO_EQUIP)
                {
                    DataEquip equip = FormMain.Instance.DataMgr.FindEquip2(strTargetSensorID);

                    if (equip == null)
                        return null;

                    state = new DangerState();
                    state.TargetEquipment = equip;
                }
                else
                    return null;
            }
            else if (strTargetZoneID.Length > 0)
            {
                if (type == SafetyChecker.DangerType.WORKER_TO_ZONE)
                {
                    int nZoneID;

                    if (!int.TryParse(strTargetZoneID, out nZoneID))
                        return null;

                    DataZone zone = FormMain.Instance.DataMgr.FindZone(nZoneID);

                    if (zone == null)
                        return null;

                    state = new DangerState();
                    state.TargetZone = zone;
                }
                else
                    return null;
            }
            else
                return null;

            state.Type = type;
            state.AlarmHistoryID = nAlarmHistoryID;
            state.Worker = FormMain.Instance.DataMgr.FindWorker(nWorkerID);

            return state;
        }

        private void ProcessAlarmHistory(ArrayList arrDatas)
        {
            int nAlarmHistoryID, nWorkerID;
            string strTargetSensorID, strTargetZoneID;
            SafetyChecker.DangerType type;

            if (!GetAlarmHistoryData(arrDatas, out nAlarmHistoryID, out nWorkerID, out strTargetSensorID, out strTargetZoneID, out type))
                return;

            DangerState state = MakeAlarmHistory(nAlarmHistoryID, nWorkerID, strTargetSensorID, strTargetZoneID, type);

            if (state == null)
                return;

            FormVirtualCCTV cctv = FormMain.Instance.Get3DView().GetCurrentCCTV();

            if (cctv != null)
            {
                DateTime time = DateTime.Now;
                string strFileName = string.Format("{0}{1:00}{2:00}_{3:00}{4:00}{5:00}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
                string strFullPath = cctv.CaptureCCTVImage(strFileName);

                if (strFullPath == null)
                    System.Diagnostics.Trace.WriteLine("CCTV Capturue 실패");
                else
                {
                    System.Diagnostics.Trace.WriteLine("CCTV Capture 성공 : " + strFullPath);
                    FormMain.Instance.AlarmManager.AddAlarmCCTV(state.AlarmHistoryID, strFullPath);
                }
            }

            FormMain.Instance.AlarmManager.AddAlarm(state.Worker, state);
        }

        private void ProcessFinishGasAlarm(ArrayList arrDatas)
        {
            if (arrDatas.Count != 3)
                return;

            if ((arrDatas[0] is string) && (arrDatas[1] is int) && (arrDatas[2] is int))
            {
                string strSensorID = (string)arrDatas[0];
                int nGasType = (int)arrDatas[1];
                int nAlarmHistoryID = (int)arrDatas[2];

                FormMain.Instance.AlarmManager.RemoveGasAlarm(strSensorID, nGasType);
            }
        }

        private void ProcessGasAlarm(ArrayList arrDatas)
        {
            if (arrDatas.Count != 7)
                return;

            if ((arrDatas[0] is string) && (arrDatas[1] is int) && (arrDatas[2] is double) && (arrDatas[3] is int) && (arrDatas[4] is int) && (arrDatas[5] is string) && (arrDatas[6] is string))
            {
                string strSensorID = (string)arrDatas[0];
                int nGasType = (int)arrDatas[1];
                double dGas = (double)arrDatas[2];
                int nAlarmHistoryID = (int)arrDatas[3];
                int nAlarmProcessHistoryID = (int)arrDatas[4];
                string strStatus = (string)arrDatas[5];
                string strMessage = (string)arrDatas[6];

                FormMain.Instance.AlarmManager.AddGasAlarm(strSensorID, nGasType, dGas, nAlarmHistoryID, nAlarmProcessHistoryID, strStatus, strMessage);
            }
        }

        private void ProcessSensorDataList(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            for (int i = 0; i < nDataCount; i += 5)
            {
                ProcessSensorData(arrDatas, i);
            }
        }

        private void ProcessSensorData(ArrayList arrDatas, int nIndex = 0)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount < nIndex + 5)
            //if (nDataCount != 5)
                return;

            ObjectType type = ObjectType.NONE;
            int nObjectID = -1;
            string strSensorID = "";
            double x = 0.0, y = 0.0;

            try
            {
                int nType = (int)arrDatas[0 + nIndex];
                nObjectID = (int)arrDatas[1 + nIndex];
                strSensorID = (string)arrDatas[2 + nIndex];
                x = (double)arrDatas[3 + nIndex];
                y = (double)arrDatas[4 + nIndex];

                if (nType <= (int)ObjectType.NONE || nType >= (int)ObjectType.TYPE_COUNT)
                    return;

                type = (ObjectType)nType;
            }
            catch (Exception)
            {
                return;
            }

            FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                     FormMain.Instance.OnReceiveSensorLocation(strSensorID, x, y);
                }
                );
           
        }

        private void ProcessRemoveSensors(ArrayList arrSensors)
        {
            FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                foreach (string strSensorID in arrSensors)
                {
                    FormMain.Instance.RemoveSensor(strSensorID);
                }
            });
        }

        protected int GetHeader(byte[] bytes, out ArrayList arrDatas)
        {
            arrDatas = null;
            
            int nBytesCount = bytes.Count();

            if (nBytesCount > 0)
            {
                this.PingCount = 0;

                short nHeader;
                arrDatas = ReadBytes(bytes, out nHeader);

                if (m_mgr != null)
                    m_mgr.RecvLog(bytes);

                return nHeader;
            }

            return 0;
        }

        private static bool ReadType(byte[] bytes, int nBytesLength, ref int nIndex, int nTotalLength, out bool isNullData)
        {
            isNullData = false;

            if (nBytesLength < nIndex + 5)
                return false;

            int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

            if (nDataLength < 0)
                return false;
            else if (nDataLength > 0)
            {
                if (nBytesLength < nIndex + nTotalLength)
                    return false;

                nIndex += nTotalLength;
            }
            else
            {
                isNullData = true;
                nIndex += 5;
            }

            return true;
        }

        public static ArrayList ReadBytes(byte[] bytes, out short nHeader)
        {
            nHeader = 0;

            int nLength = bytes.Length;

            if (nLength < 6)
                return null;

            nHeader = BitConverter.ToInt16(bytes, 0);
            int nChunkCount = BitConverter.ToInt32(bytes, 2);

            ArrayList arrResult = new ArrayList();
            int nIndex = 6;
            bool isNullData;

            for (int i = 0; i < nChunkCount; i++)
            {
                if (nLength <= nIndex)
                    return null;

                byte type = bytes[nIndex];

                if (type == TCP_TYPE.INTEGER)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 9, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        int nData = BitConverter.ToInt32(bytes, nIndex - 4);
                        arrResult.Add(nData);
                    }
                }
                else if (type == TCP_TYPE.FLOAT)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 9, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        float fData = BitConverter.ToSingle(bytes, nIndex - 4);
                        arrResult.Add(fData);
                    }
                }
                else if (type == TCP_TYPE.DOUBLE)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 13, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        double dData = BitConverter.ToDouble(bytes, nIndex - 8);
                        arrResult.Add(dData);
                    }
                }
                else if (type == TCP_TYPE.LONG)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 13, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        long lData = BitConverter.ToInt64(bytes, nIndex - 8);
                        arrResult.Add(lData);
                    }
                }
                else if (type == TCP_TYPE.BOOLEAN)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 6, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        bool bData = BitConverter.ToBoolean(bytes, nIndex - 1);
                        arrResult.Add(bData);
                    }
                }
                else if (type == TCP_TYPE.SHORT)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 7, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        short sData = BitConverter.ToInt16(bytes, nIndex - 2);
                        arrResult.Add(sData);
                    }
                }
                else if (type == TCP_TYPE.BYTE)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 6, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        byte data = bytes[nIndex - 1];
                        arrResult.Add(data);
                    }
                }
                else if (type == TCP_TYPE.STRING)
                {
                    if (nLength < nIndex + 5)
                        return null;

                    int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

                    if (nDataLength < 0)
                        return null;
                    else if (nDataLength > 0)
                    {
                        if (nLength < nIndex + 5 + nDataLength)
                            return null;

                        string strData = Encoding.UTF8.GetString(bytes, nIndex + 5, nDataLength);
                        arrResult.Add(strData);

                        nIndex += 5 + nDataLength;
                    }
                    else
                    {
                        arrResult.Add("");
                        nIndex += 5;
                    }
                }
                else if (type == TCP_TYPE.DATETIME)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 14, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        int year = BitConverter.ToInt16(bytes, nIndex - 9);
                        int month = bytes[nIndex - 7];
                        int day = bytes[nIndex - 6];
                        int hour = bytes[nIndex - 5];
                        int min = bytes[nIndex - 4];
                        int sec = bytes[nIndex - 3];
                        int millisec = bytes[nIndex - 2];

                        DateTime dtTime = new DateTime(year, month, day, hour, min, sec, millisec);
                        arrResult.Add(dtTime);
                    }
                }
                else
                    return null;
            }

            return arrResult;
        }

        private void SendWhoIam()
        {
            // WHO_I_AM 전달시 자신의 로그인 ID를 같이 전송한다.
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add((int)TCP_CLIENT.HSMS_CLIENT);

            if (LoginManager.Instance != null)
            {
                arrDatas.Add(LoginManager.Instance.LoginID);
                arrDatas.Add(LoginManager.Instance.LoginUserID);
            }
            else
            {
                arrDatas.Add(0);
                arrDatas.Add("");
            }

            byte[] bytes = MakeBytes(TCP_ID.WHO_I_AM, arrDatas);

            /*byte[] bytes = new byte[15];
            byte[] dataBytes = MakeBytes((int)TCP_CLIENT.HSMS_CLIENT);

            byte[] nHeader = BitConverter.GetBytes((short)TCP_ID.WHO_I_AM);
            byte[] nCount = BitConverter.GetBytes(1);

            // SET MESSAGE HeADER
            bytes[0] = nHeader[0];
            bytes[1] = nHeader[1];

            // SET DATA COUNT
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            System.Buffer.BlockCopy(dataBytes, 0, bytes, 6, dataBytes.Length);*/

            m_mgr.Send(bytes, this);
        }

        // header 1 Byte로만 이루어진 데이터
        public void SendData(short header)
        {
            byte[] bytes = new byte[6];

            byte[] nHader = BitConverter.GetBytes(header);
            byte[] nCount = BitConverter.GetBytes(0);

            bytes[0] = nHader[0];
            bytes[1] = nHader[1];

            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            if (this.Client.Client.Connected == true)
                m_mgr.Send(bytes, this);
        }

        public static byte[] MakeBytes(int data)
        {
            int nDataLength = sizeof(int);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.INTEGER;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(long data)
        {
            int nDataLength = sizeof(long);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.LONG;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(float data)
        {
            int nDataLength = sizeof(float);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.FLOAT;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(double data)
        {
            int nDataLength = sizeof(double);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.DOUBLE;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(string data)
        {
            UTF8Encoding enc = new UTF8Encoding();
            byte[] datas = enc.GetBytes(data);

            int nDataLength = datas.Length;

            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.STRING;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = datas[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(DateTime data)
        {
            int nDataLength = 9;
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.DATETIME;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] bytesYear = BitConverter.GetBytes((short)data.Year);
            byte[] bytesMilliSecond = BitConverter.GetBytes((short)data.Millisecond);

            bytes[nCount + 1] = bytesYear[0];
            bytes[nCount + 2] = bytesYear[1];
            bytes[nCount + 3] = (byte)data.Month;
            bytes[nCount + 4] = (byte)data.Day;
            bytes[nCount + 5] = (byte)data.Hour;
            bytes[nCount + 6] = (byte)data.Minute;
            bytes[nCount + 7] = (byte)data.Second;
            bytes[nCount + 8] = bytesMilliSecond[0];
            bytes[nCount + 9] = bytesMilliSecond[1];

            return bytes;
        }

        public static byte[] MakeBytes(bool data)
        {
            int nDataLength = sizeof(bool);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.BOOLEAN;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(short nHeader, ArrayList arrDatas)
        {
            int nChunkCount = arrDatas == null ? 0 : arrDatas.Count;

            ArrayList arrBytes = new ArrayList();
            int nBytesCount = 0;

            for (int i = 0; i < nChunkCount; i++)
            {
                object data = arrDatas[i];
                Type type = data.GetType();
                byte[] bytes = null;

                if (type == typeof(int))
                    bytes = MakeBytes((int)data);
                else if (type == typeof(long))
                    bytes = MakeBytes((long)data);
                else if (type == typeof(float))
                    bytes = MakeBytes((float)data);
                else if (type == typeof(bool))
                    bytes = MakeBytes((bool)data);
                else if (type == typeof(double))
                    bytes = MakeBytes((double)data);
                else if (type == typeof(short))
                    bytes = MakeBytes((short)data);
                else if (type == typeof(byte))
                    bytes = MakeBytes((byte)data);
                else if (type == typeof(string))
                    bytes = MakeBytes((string)data);
                else if (type == typeof(DateTime))
                    bytes = MakeBytes((DateTime)data);
                else
                    return null;

                nBytesCount += bytes.Length;
                arrBytes.Add(bytes);
            }

            byte[] _bytes = new byte[6 + nBytesCount];
            byte[] headerBytes = BitConverter.GetBytes(nHeader);
            byte[] lengthBytes = BitConverter.GetBytes(nChunkCount);

            _bytes[0] = headerBytes[0];
            _bytes[1] = headerBytes[1];
            _bytes[2] = lengthBytes[0];
            _bytes[3] = lengthBytes[1];
            _bytes[4] = lengthBytes[2];
            _bytes[5] = lengthBytes[3];

            int nIndex = 6;

            foreach (byte[] bytes in arrBytes)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    _bytes[nIndex + i] = bytes[i];
                }

                nIndex += bytes.Length;
            }

            return _bytes;
        }

        public override void OnDropConnection()
        {
            m_mgr.OnDropConnection();
        }

        public int Send_NoLengthByte(byte[] buffer, int offset, int size)
        {
            if (Client != null)
            {
                SocketError nErrCode = SocketError.Success;
                int nSendSize = 0;

                nSendSize = Client.Client.Send(buffer, 0, size, SocketFlags.None, out nErrCode);

                if (nErrCode == SocketError.Success)
                    return nSendSize;
            }

            return -1;
        }

        public void SendChangeAlarmIgnoreOptions(ArrayList arrDatas)
        {
            byte[] bytes = MakeBytes(TCP_ID.CHANGE_DB_DATA, arrDatas);
            Send(bytes, 0, bytes.Length);
        }

        public void SendFinishGasAlarm(string strSensorID, int nGasType)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(strSensorID);
            arrDatas.Add(nGasType);

            byte[] bytes = MakeBytes(TCP_ID.FINISH_GAS_ALARM, arrDatas);
            Send(bytes, 0, bytes.Length);
        }

        public void SendFinishAlarm(DangerState state)
        {
            if (state.AlarmHistoryID <= 0)
                return;

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(state.AlarmHistoryID);

            byte[] bytes = MakeBytes(TCP_ID.FINISH_ALARM, arrDatas);
            Send(bytes, 0, bytes.Length);
        }
        //////////////////////////////////////////////////////////////////
        // LOGIN FUNCTION
        public bool SendLogout(string szID)
        {
            if (this.IsConnected == false)
            {
                return false;
            }

            byte[] dataBytes = MakeBytes(szID);
            byte[] bytes = new byte[dataBytes.Length + 6];

            byte[] nHader = BitConverter.GetBytes((short)TCP_ID.LOGOUT_USER);
            byte[] nCount = BitConverter.GetBytes(1);

            // SET MESSAGE HeADER
            bytes[0] = nHader[0];
            bytes[1] = nHader[1];

            // SET DATA COUNT
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            System.Buffer.BlockCopy(dataBytes, 0, bytes, 6, dataBytes.Length);

            m_mgr.Send(bytes, this);

            return true;
        }

        public bool SendCheckUser(int nID, string szID)
        {
            if (this.IsConnected == false)
            {
                return false;
            }

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nID);
            arrDatas.Add(szID);
            arrDatas.Add(FormMain.Instance.SiteID);
            arrDatas.Add(LoginManager.Instance.LoginState);

            byte[] bytes = MakeBytes(TCP_ID.CHECK_LOGIN, arrDatas);

            /*byte[] dataBytes = MakeBytes(szID);
            byte[] bytes = new byte[dataBytes.Length + 6];

            byte[] nHader = BitConverter.GetBytes((short)TCP_ID.CHECK_LOGIN);
            byte[] nCount = BitConverter.GetBytes(1);

            // SET MESSAGE HeADER
            bytes[0] = nHader[0];
            bytes[1] = nHader[1];

            // SET DATA COUNT
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            System.Buffer.BlockCopy(dataBytes, 0, bytes, 6, dataBytes.Length);*/

            m_mgr.Send(bytes, this);

            return true;
        }

        public bool SendLoginUser(string szID, string szPass, string szCode)
        {
            if (this.IsConnected == false)
            {
                return false;
            }

            byte[] dataBytes = MakeBytes(szID);
            byte[] dataBytes2 = MakeBytes(szPass);
            byte[] dataBytes3 = MakeBytes(szCode);

            byte[] bytes = new byte[dataBytes.Length + dataBytes2.Length + dataBytes3.Length + 6];

            byte[] nHader = BitConverter.GetBytes((short)TCP_ID.LOGIN_USER);
            byte[] nCount = BitConverter.GetBytes(3);

            // SET MESSAGE HeADER
            bytes[0] = nHader[0];
            bytes[1] = nHader[1];

            // SET DATA COUNT
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            System.Buffer.BlockCopy(dataBytes, 0, bytes, 6, dataBytes.Length);
            System.Buffer.BlockCopy(dataBytes2, 0, bytes, 6 + dataBytes.Length, dataBytes2.Length);
            System.Buffer.BlockCopy(dataBytes3, 0, bytes, 6 + dataBytes.Length + dataBytes2.Length, dataBytes3.Length);

            if (m_mgr.Send(bytes, this) < 0)
            {
                return false;
            }

            return true;
        }

        public bool SendRegisterUser(string szMemberID, string szPass, int nUserLevel, ArrayList arrMacAddrList, UnE.KeyValidator.CertOption option)
        {
            if (this.IsConnected == false)
            {
                return false;
            }

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(szMemberID);
            arrDatas.Add(szPass);
            arrDatas.Add(nUserLevel);
            arrDatas.Add((int)option);
            arrDatas.Add(FormMain.Instance.SiteID);

            foreach (string strMacAddr in arrMacAddrList)
            {
                arrDatas.Add(strMacAddr);
            }

            byte[] bytes = MakeBytes((short)TCP_ID.JOIN_USER, arrDatas);

            /*byte[] dataBytes = MakeBytes(szMemberID);
            byte[] dataBytes1 = MakeBytes(szPass);
            byte[] dataBytes2 = MakeBytes(szRegisterCode);
            byte[] dataBytes3 = MakeBytes(szCode);
            byte[] dataBytes4 = MakeBytes(nUserLevel);

            byte[] bytes = new byte[dataBytes.Length + dataBytes1.Length + dataBytes2.Length + dataBytes3.Length + dataBytes4.Length + 6];

            byte[] nHader = BitConverter.GetBytes((short)TCP_ID.JOIN_USER);
            byte[] nCount = BitConverter.GetBytes(5);

            // SET MESSAGE HeADER
            bytes[0] = nHader[0];
            bytes[1] = nHader[1];

            // SET DATA COUNT
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            int dataLength = 6;
            System.Buffer.BlockCopy(dataBytes, 0, bytes, dataLength, dataBytes.Length);
            dataLength += dataBytes.Length;
            System.Buffer.BlockCopy(dataBytes1, 0, bytes, dataLength, dataBytes1.Length);
            dataLength += dataBytes1.Length;
            System.Buffer.BlockCopy(dataBytes2, 0, bytes, dataLength, dataBytes2.Length);
            dataLength += dataBytes2.Length;
            System.Buffer.BlockCopy(dataBytes3, 0, bytes, dataLength, dataBytes3.Length);
            dataLength += dataBytes3.Length;
            System.Buffer.BlockCopy(dataBytes4, 0, bytes, dataLength, dataBytes4.Length);*/

            m_mgr.Send(bytes, this);
            return true;
        }

        public bool SendChangePassword(string szUserID, string strCertCode, string strMacAddrList, string szNewPass)
        {
            if (this.IsConnected == false)
            {
                return false;
            }
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(szUserID);
            arrDatas.Add(strCertCode);
            arrDatas.Add(strMacAddrList);
            arrDatas.Add(szNewPass);

            byte[] bytes = MakeBytes((short)TCP_ID.CHNAGE_PASSWORD, arrDatas);
            /*byte[] dataBytes = MakeBytes(szUserID);
            byte[] dataBytes1 = MakeBytes(szPass);
            byte[] dataBytes2 = MakeBytes(szNewPass);

            byte[] bytes = new byte[dataBytes.Length + dataBytes1.Length + dataBytes2.Length + 6];

            byte[] nHader = BitConverter.GetBytes((short)TCP_ID.CHNAGE_PASSWORD);
            byte[] nCount = BitConverter.GetBytes(3);

            // SET MESSAGE HeADER
            bytes[0] = nHader[0];
            bytes[1] = nHader[1];

            // SET DATA COUNT
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            System.Buffer.BlockCopy(dataBytes, 0, bytes, 6, dataBytes.Length);
            System.Buffer.BlockCopy(dataBytes1, 0, bytes, 6 + dataBytes.Length, dataBytes1.Length);
            System.Buffer.BlockCopy(dataBytes2, 0, bytes, 6 + dataBytes.Length + dataBytes1.Length, dataBytes2.Length);*/

            m_mgr.Send(bytes, this);
            return true;
        }

        /*public bool SendRequestCode(string szUserID, string szPass)
        {
            if (this.IsConnected == false)
            {
                return false;
            }
            byte[] dataBytes = MakeBytes(szUserID);
            byte[] dataBytes1 = MakeBytes(szPass);

            byte[] bytes = new byte[dataBytes.Length + dataBytes1.Length + 6];

            byte[] nHader = BitConverter.GetBytes((short)TCP_ID.REQUEST_CODE);
            byte[] nCount = BitConverter.GetBytes(2);

            // SET MESSAGE HeADER
            bytes[0] = nHader[0];
            bytes[1] = nHader[1];

            // SET DATA COUNT
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            System.Buffer.BlockCopy(dataBytes, 0, bytes, 6, dataBytes.Length);
            System.Buffer.BlockCopy(dataBytes1, 0, bytes, 6 + dataBytes.Length, dataBytes1.Length);

            m_mgr.Send(bytes, this);
            return true;
        }*/

        /*public bool SendSetPassword(string szGenUserID, string szNewPass)
        {
            if (this.IsConnected == false)
            {
                return false;
            }
            byte[] dataBytes = MakeBytes(szGenUserID);
            byte[] dataBytes2 = MakeBytes(szNewPass);

            byte[] bytes = new byte[dataBytes.Length + dataBytes2.Length + 6];

            byte[] nHader = BitConverter.GetBytes((short)TCP_ID.SET_PASSWORD);
            byte[] nCount = BitConverter.GetBytes(2);

            // SET MESSAGE HeADER
            bytes[0] = nHader[0];
            bytes[1] = nHader[1];

            // SET DATA COUNT
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            System.Buffer.BlockCopy(dataBytes, 0, bytes, 6, dataBytes.Length);
            System.Buffer.BlockCopy(dataBytes2, 0, bytes, 6 + dataBytes.Length, dataBytes2.Length);

            m_mgr.Send(bytes, this);
            return true;
        }*/

        public bool SendDeleteUser(string szUserID, string szPass)
        {
            if (this.IsConnected == false)
            {
                return false;
            }

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(szUserID);
            arrDatas.Add(szPass);
            arrDatas.Add(FormMain.Instance.SiteID);

            byte[] bytes = MakeBytes(TCP_ID.DELETE_USER, arrDatas);

            /*byte[] dataBytes = MakeBytes(szUserID);
            byte[] dataBytes2 = MakeBytes(szPass);

            byte[] bytes = new byte[dataBytes.Length + dataBytes2.Length + 6];

            byte[] nHader = BitConverter.GetBytes((short)TCP_ID.DELETE_USER);
            byte[] nCount = BitConverter.GetBytes(2);

            // SET MESSAGE HeADER
            bytes[0] = nHader[0];
            bytes[1] = nHader[1];

            // SET DATA COUNT
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            System.Buffer.BlockCopy(dataBytes, 0, bytes, 6, dataBytes.Length);
            System.Buffer.BlockCopy(dataBytes2, 0, bytes, 6 + dataBytes.Length, dataBytes2.Length);*/

            m_mgr.Send(bytes, this);
            return true;
        }
    }    
}
