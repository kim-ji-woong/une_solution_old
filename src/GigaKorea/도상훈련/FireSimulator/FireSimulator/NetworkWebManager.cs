using DBUtility2;
using SOPWebClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FireSimulator
{
    public class NetworkWebManager : IPostMan
    {
        private PostBox m_postBox = null;
        private WebDBManager m_dbMgr = null;
        private bool m_shutdownThread = false;
        private bool m_isConnected = false;
        private int m_nPort = -1;
        private DateTime m_dtLastSendMessage = new DateTime();

        private int m_nClientType = SOPWebServer.ClientType.FIRE_SENSOR_SERVER;
        private int m_nClientSubType = SOPWebServer.ClientSubType.SIMULATOR;

        public bool IsConnected
        {
            get { return m_isConnected; }
        }

        public NetworkWebManager(WebDBManager dbMgr, int nClientType, int nClientSubType)
        {
            m_dbMgr = dbMgr;
            //InitLog();

            m_nClientType = nClientType;
            m_nClientSubType = nClientSubType;

            int nPort = ReadServerPort();
            SetPostBox(nPort);

            Thread t = new Thread(new ThreadStart(ConnectionThread));
            t.Start();
        }

        private void ConnectionThread()
        {
            int nPrevMonth = DateTime.Now.Month;

            while (m_shutdownThread == false)
            {
                if (m_isConnected == false)
                {
                    int nPort = ReadServerPort();

                    if (m_nPort != nPort)
                        SetPostBox(nPort);

                    if (m_postBox != null)
                    {
                        if (m_postBox.Connect(m_nClientType, m_nClientSubType))
                        {
                            m_isConnected = true;
                        }
                    }
                }
                else
                {
                    TimeSpan span = DateTime.Now - m_dtLastSendMessage;

                    // 마지막 메시지를 보낸 이후 3초 이상 지났는지 확인한다.
                    if (span.TotalSeconds > 3.0)
                    {
                        // 접속이 유지되고 있는지 확인한다.
                        SendMessage(SOPWebServer.Header.ARE_YOU_THERE, null);
                    }
                }

                Thread.Sleep(1000);
            }
        }

        public void ReleaseThread()
        {
            m_shutdownThread = true;
        }


        public bool SendMessage(int header, byte[] messages)
        {
            if (m_postBox == null || m_isConnected == false)
            {
                m_isConnected = false;
            }
            else
            {
                SendLog(header, messages);

                bool closeConnection;
                bool result = m_postBox.SendMessage(header, messages, out closeConnection);

                if (closeConnection)
                {
                    WriteLog(m_postBox.ErrorMessage);
                    m_isConnected = false;
                }
                else
                    m_dtLastSendMessage = DateTime.Now;

                return result;
            }

            return false;
        }

        private void WriteLog(object str)
        {
            if (ConnectionLogEx.Instance.IsOpened)
                ConnectionLogEx.Instance.Write(str);
        }

        private void WriteLineLog(object str)
        {
            if (ConnectionLogEx.Instance.IsOpened)
                ConnectionLogEx.Instance.WriteLine(str);
        }

        private void SendLog(int header, byte[] bytes)
        {
            MessageLog(header, bytes, "SendMessage");
        }

        private void MessageLog(int header, byte[] bytes, string strMessageTag)
        {
            if (!ConnectionLogEx.Instance.IsOpened)
                return;

            if (header != SOPWebServer.Header.ARE_YOU_THERE)
            {
                string strLog = "";

                if (bytes == null)
                {
                    strLog = string.Format(strMessageTag + " : Header({0}), Length(0)", header);
                }
                else
                {
                    strLog = string.Format(strMessageTag + " : Header({0}), Length({1})", header, bytes.Length);
                    string strBytes = "";

                    foreach (byte b in bytes)
                    {
                        if (strBytes.Length == 0)
                            strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                        else
                            strBytes += string.Format(" {0:X2}", (int)b);
                    }

                    strLog += strBytes;
                }

                WriteLineLog(strLog);
            }
        }

        private int ReadServerPort()
        {
            string strSQL = "Select Port from SensorServerPort where Name = '" + SOPWebServer.ServerPort.SOP_WEB_SERVER + "' and SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> port = WebDBManager.GetIntField(arrResult[0].ToString());

            if (port == null)
                return -1;

            return port.Data;
        }

        private void SetPostBox(int nPort)
        {
            if (nPort > 0)
            {
                m_postBox = new PostBox();
                m_postBox.WebServerURL = m_dbMgr.WebServerURL;
                m_postBox.PostMan = this;

                m_nPort = nPort;
            }
        }

        public void OnMessage(int header, byte[] messages)
        {
            /*
            if (FormMain.Instance.Closing)
              return;

            ArrayList arrDatas = messages == null ? null : SOPWebServer.BinaryHelper.ReadBytes(messages);

            if (header == SOPWebServer.Header.CLOSE_CONNECTION)
            {
                m_isConnected = false;
            }
            else if (header == SOPWebServer.Header.ACCEPT_LOGIN)
            {
                ProcessAcceptLogin(arrDatas);
            }
            else if (header == SOPWebServer.Header.REJECT_LOGIN)
            {
                ProcessRejectLogin(arrDatas);
            }
            else if (header == SOPWebServer.Header.CHECK_LOGIN)
            {
                ProcessCheckLogin();
            }
            else if (header == SOPWebServer.Header.LOGOUT_USER)
            {
                ProcessLogoutUser();
            }
            else if (header == SOPWebServer.Header.JOIN_USER)
            {
                ProcessJoinUser(arrDatas);
            }
            else if (header == SOPWebServer.Header.CHANGE_SOPGENUSER_COMMANDER)
            {
                ProcessChangeSOPGenUserCommander(arrDatas);
            }
            else if (header == SOPWebServer.Header.CHANGE_PASSWORD || header == SOPWebServer.Header.SET_PASSWORD)
            {
                ProcessChangePassword(arrDatas);
            }
            else if (header == SOPWebServer.Header.CHANGE_NICKNAME)
            {
                ProcessChangeNickName(arrDatas);
            }
            else if (header == SOPWebServer.Header.END_RESTORE)
            {
                LoginManager.Instance.OnEndRestore();
            }
            else if (header == SOPWebServer.Header.SERVER_COMMAND)
            {
                ProcessServerCommand(arrDatas);
            }
            else if (header == SOPWebServer.Header.INTERNAL_MESSAGE)
            {
                // SOP Server가 다른 곳에서 전송된 InternalMessage를 대신 전달해 주는 경우
                ProcessInternalMessage(arrDatas, messages);
            }

            RecvLog(header, messages);
            */
        }

        //public bool SendSensorData(int nSensorZoneID, int nSensorTagInfoID, int nSensorType, int nData, string szBuilding, string szTag)
        public bool SendSensorData(Alarm alarm, Project project)
        {
            // SOP서버로 연결된 Provider로 전송
            if (m_isConnected == false)
                return false;

            string strLevelName = alarm.Level.Name;
            string strSpaceName = alarm.Space.Name;

            int nSensor = 0;    // (int)Facility.FacilityType.FIRE_SENSOR;
            int nSensorTagInfoID = 0;
            int nSensorZoneID = 0;
            int nData = 1;      // 1: 알람 발생, 0: 알람 해제

            bool bRet = ReadSensorData(strSpaceName, out nSensorZoneID, out nSensorTagInfoID);

            if (!bRet)
                return false;

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nSensor);
            arrDatas.Add(nSensorTagInfoID);
            arrDatas.Add(nSensorZoneID);
            arrDatas.Add(nData);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return SendMessage(SOPWebServer.Header.SENSOR_DATA_TEST, bytes);


            //int nSensor = -1;
            //Facility.FacilityType sensorType = Facility.ToFacilityType(nSensorType);

            //switch (sensorType)
            //{
            //    case Facility.FacilityType.FIRE_SENSOR:
            //    case Facility.FacilityType.FireSensor_TypeA:
            //    case Facility.FacilityType.FireSensor_TypeB:
            //    case Facility.FacilityType.FireSensor_GasEmission:
            //    case Facility.FacilityType.FireSensor_ManualControl:
            //        nSensor = (int)Facility.FacilityType.FIRE_SENSOR;
            //        break;

            //    case Facility.FacilityType.FireSensor_SiemensType:
            //    case Facility.FacilityType.FireSensor_AnalogSmokeType:
            //    case Facility.FacilityType.PSM_SENSOR:
            //        nSensor = (int)sensorType;
            //        break;
            //}

            //if (nSensor == -1)
            //    return false;

            //if (nSensorZoneID < 0)
            //    return false;


        }

        public bool SendClearData(Alarm alarm, Project project)
        {
            // SOP서버로 연결된 Provider로 전송
            if (m_isConnected == false)
                return false;

            string strLevelName = alarm.Level.Name;
            string strSpaceName = alarm.Space.Name;

            int nSensor = 0;    // (int)Facility.FacilityType.FIRE_SENSOR;
            int nSensorTagInfoID = 0;
            int nSensorZoneID = 0;
            int nData = 0;      // 1: 알람 발생, 0: 알람 해제

            bool bRet = ReadSensorData(strSpaceName, out nSensorZoneID, out nSensorTagInfoID);

            if (!bRet)
                return false;

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nSensor);
            arrDatas.Add(nSensorTagInfoID);
            arrDatas.Add(nSensorZoneID);
            arrDatas.Add(nData);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return SendMessage(SOPWebServer.Header.SENSOR_DATA_TEST, bytes);
        }

        public bool SendOutbreakData(int nActionStepHistoryID, int nProcessID)
        {
            // SOP서버로 연결된 Provider로 전송
            if (m_isConnected == false)
                return false;

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nActionStepHistoryID);
            arrDatas.Add("Process");
            arrDatas.Add(nProcessID);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return SendMessage(SOPWebServer.Header.SELECT_SOP_COMPONENT, bytes);
        }

        private bool ReadSensorData(string strSpaceName, out int nSensorZoneID, out int nSensorTagInfoID)
        {
            nSensorZoneID = 0;
            nSensorTagInfoID = 0;

            string strSQL = "Select sz.ID, st.ID from EquipmentZone as ez, SensorZone as sz, SensorTagInfo as st where ez.ZoneName = '" + strSpaceName + "' AND ez.ID = sz.EquipZoneID AND ez.ID = st.EquipZoneID";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            int nCount = arrResult.Count;

            for (int i = 0; i < nCount - 1; i += 2)
            {
                nSensorZoneID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                nSensorTagInfoID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
            }

            return true;
        }
    }

    

    public class ConnectionLogEx : TcpLib2.ConnectionLog
    {
        private log4net.ILog logger = null;
        private static ConnectionLogEx m_instance2 = new ConnectionLogEx();

        public static ConnectionLogEx Instance
        {
            get { return m_instance2; }
        }

        public static bool MakeInstance()
        {
            /*if (m_instance == null)
				m_instance = new ConnectionLogEx();

			ConnectionLogEx instance = (ConnectionLogEx)m_instance;*/
            m_instance2.logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            m_instance2.m_isOpened = true;
            return m_instance2.m_isOpened;
        }

        public override bool Write(object obj, bool writeTime = true)
        {
            if (obj.GetType() == typeof(Exception))
            {
                Exception e = (Exception)obj;
                if (logger != null)
                    logger.Debug(e.Message, e);
            }
            else
            {
                if (logger != null)
                    logger.DebugFormat("{0}", obj.ToString());
            }
            return true;
        }

        public override bool WriteLine(object obj, bool writeTime = true)
        {
            if (obj.GetType() == typeof(Exception))
            {
                Exception e = (Exception)obj;
                if (logger != null)
                    logger.Debug(e.Message, e);
            }
            else
            {
                if (logger != null)
                    logger.Debug(obj.ToString());
            }
            return true;
        }
    }
}
