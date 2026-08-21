using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Net.Sockets;
using SOPMonitoringSystem;
using System.Windows.Forms;
using SDMS;
using UnE.SOP;
using DBUtility;

namespace SOPMonitoringSystem
{
    public class ClientProvider : ClientServiceProvider
    {
		public enum ClientType { ALL = 0, SDMS_CLIENT, SOP_SIMULATOR, SENSOR_SIMULATOR, UNKNOWN };

        public enum ReactionType
        {
            BEGIN_STATUS = 0,
            RUN_BROADCAST = 10,
            SEND_SMS = 11,
            MALFUNCTION = 21,
            NOTIFY_FIRE = 22,
            IGNORE_FIRE = 23,
            RUN_SOP = 30,
            RUN_N_CANCEL_SOP = 31,
            FINISH_SOP = 32,
            IGNORE_SOP = 33,
            ETC = 100
        }

        private NetworkManager m_mgr = null;
        private int m_nPingCount = 0;
        private byte[] m_arrReceived = null;
        // OnReceive()에서 전달받는 데이터(ReceivedData)가 아직 완결되지 않은 Packet일 경우 다음 OnReceive() 호출시 데이터를
        // 합치기 위한 임시 버퍼
        private byte[] m_arrTemp = null;

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
        }
        
        public override void OnReceiveData()
        {
            OnReceive(ReceivedData);
        }

        private bool OnReceive(byte[] bytes)
        {
            if (bytes != null)
            {
                m_isReadingProcess = true;

                m_arrReceived = bytes;

                if (m_arrTemp != null)
                {
                    int nReceivedCount = m_arrReceived.Length;
                    int nTempCount = m_arrTemp.Length;

                    byte[] arrBuffer = new byte[nReceivedCount + nTempCount];
                    Array.Copy(m_arrTemp, arrBuffer, nTempCount);
                    Array.Copy(m_arrReceived, 0, arrBuffer, nTempCount, nReceivedCount);

                    m_arrReceived = arrBuffer;
                    m_arrTemp = null;
                }

                int nBytesCount = m_arrReceived.Count();

                if (nBytesCount > 0)
                {
                    m_nPingCount = 0;

                    if (!CheckValidation(m_arrReceived))
                    {
                        m_arrTemp = m_arrReceived;
                        m_isReadingProcess = false;
                        return false;
                    }

                    m_mgr.RecvLog(m_arrReceived);

					//int nHeader = (int)BitConverter.ToInt16(m_arrReceived, 0);
                    short nHeader;
                    ArrayList arrDatas = ReadBytes(m_arrReceived, out nHeader);

                    if (arrDatas == null)
                        return false;

					if (nHeader == TCP_ID.ARE_YOU_THERE)
                    {
                        ProcessAreYouThere(arrDatas);
                        //SendData(TCP_ID.I_AM_HERE);
                    }
					else if (nHeader == TCP_ID.WHO_ARE_YOU)
                    {
                        //SendData(TCP_ID.WHO_I_AM, TCP_TYPE.INTEGER, BitConverter.GetBytes((int)ClientType.SOP_SIMULATOR));
                        SendWhoIAm();
                    }
					else if (nHeader == TCP_ID.FIRE_SENSOR_SIGNAL || nHeader == TCP_ID.FIRE_DETECT_TRAINNING)
                    {
                        ProcessFireSensorSignal(m_arrReceived);
                    }
					else if (nHeader == TCP_ID.SENSOR_REACTION_HISTORY_DATA)
                    {
                        ProcessSensorReactionHistory(m_arrReceived);
                    }
                    else if (nHeader == TCP_ID.CLEAR_DETECT_REPORT)
                    {
                        ProcessClearDetect(m_arrReceived);
                    }
                    else if (nHeader == TCP_ID.GIVE_CONTROL)
                    {
                        ObtainControl();
                    }
                    else if (nHeader == TCP_ID.TAKE_CONTROL)
                    {
                        LoseControl();
                    }
                    else if (nHeader == TCP_ID.GIVE_CONTROL_KEY)
                    {
                        ProcessGiveControlKey(arrDatas);
                    }
                    else if (nHeader == TCP_ID.REQUEST_CONTROL)
                    {
                        ProcessRequestControl(m_arrReceived);
                    }
                    else if (nHeader == TCP_ID.REJECT_REQUEST_CONTROL)
                    {
                        ProcessRejectRequestControl();
                    }
                    else if (nHeader == TCP_ID.CHANGE_CONFIG)
                    {
                        ProcessChangedConfig(arrDatas);
                        //ProcessChangeCompanyMember();
                    }
                    else if (nHeader == TCP_ID.SOP_SELECT_MISSION)
                    {
                        ProcessSelectMission(m_arrReceived);
                    }
                    else if (nHeader == TCP_ID.SOP_CURRENT_SELECT_MISSION)
                    {
                        ProcessCurrentSelectMission(m_arrReceived);
                    }
                    else if (nHeader == TCP_ID.IGNORE_SOP)
                    {
                        ProcessIgnoreSOP(arrDatas);
                    }
                    else if (nHeader == TCP_ID.CHAGNE_WORK_MEMBER)
                    {
                        NeedToUpdateWorkingMemberData();
                    }
                    else if (nHeader == TCP_ID.SOP_SIMULATOR_COMMAND)
                    {
                        ProcessSimulatorCommand(arrDatas);
                    }
                    else if (nHeader == TCP_ID.EARTHQUAKE_SENSOR_DETECT)
                    {
                        ProcessEarthquakeSensorDetect(arrDatas);
                        
                    }
                }
            }

            m_isReadingProcess = false;
            return true;
        }

#if SAFE_KOREA_YH_2017
        private void ProcessBuilingCollapseDetect()
        {
            // SDMS 지진 이벤트를 발생시킨다.
            SDMS.ScriptProxy.Instance.UserObject.SDMSShowBuildingCollapsed.Invoke("yhz85", "14호기 기계공작실");
        }
#endif

        private void ProcessEarthquakeSensorDetect(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount < 8)
                return;

            if (arrDatas[0] is int && arrDatas[1] is float && arrDatas[2] is int && arrDatas[3] is int && arrDatas[4] is string && arrDatas[5] is long && arrDatas[6] is bool && arrDatas[7] is string)
            {
                int nSensorID = (int)arrDatas[0];
                float fMagnitude = (float)arrDatas[1];
                int nIntensity = (int)arrDatas[2];
                int nAlarmLevel = (int)arrDatas[3];
                string strPosition = (string)arrDatas[4];
                DBUtility.VariousData<DateTime> time = (long)arrDatas[5] == 0 ? null : new DBUtility.VariousData<DateTime>(DateTime.FromBinary((long)arrDatas[5]));
                bool runSOP = (bool)arrDatas[6];
                string strSOPPath = (string)arrDatas[7];

                FormSOP.Instance.ProxyMessenger.OpenSOP_Earthquake(strSOPPath, time == null ? DateTime.Now : time.Data, -1, nIntensity, fMagnitude, strPosition);

                // SDMS 지진 이벤트를 발생시킨다.
                /*SDMS.ScriptProxy.Instance.UserObject.SDMSEarthquakeEvent.Invoke(nIntensity, fMagnitude, strPosition, false);

                

                // SDMS 지진 이벤트가 끝나기를 기다린다.
                while (SDMS.ScriptProxy.Instance.UserObject.SDMSEarthquakeEventIsFinished() == false)
                {
                    System.Threading.Thread.Sleep(100);
                }

                int nActionStepID = GetEarthquakeLinkedActionStepID();

                if (nActionStepID < 0)
                    return;

                //if (CheckPrevEarthquakeSOP(fMagnitude, nIntensity, nActionStepID) == false)
                //{
                //    // 기존에 더 큰 지진세기로 진행중인 SOP가 존재한다.
                //    return;
                //}

                if (FormSOP.Instance.HasControl == false)
                    return;

                FormSOP.Instance.Invoke((MethodInvoker)delegate
                {
                    TreeNode node = SOPScenarioManager.Instance.GetBarLevelTree().FindActionStepNode(nActionStepID);

                    if (node == null)
                        return;

                    UnE.SOP.Workstate.WorkflowOptionEarthquake option = new UnE.SOP.Workstate.WorkflowOptionEarthquake();

                    if (nIntensity >= 0)
                    {
                        option.Intensity = nIntensity;
                        option.Mode = UnE.SOP.Workstate.WorkflowOptionEarthquake.PowerMode.Intensity;
                    }

                    if (fMagnitude >= 0.0f)
                    {
                        option.Magnitude = fMagnitude;
                        option.Mode = UnE.SOP.Workstate.WorkflowOptionEarthquake.PowerMode.Magnitude;
                    }

                    if (strPosition.Length > 0)
                    {
                        option.PositionName = strPosition;
                        option.HasPosition = true;
                    }

                    if (time != null)
                        option.DetectTime = time;
                    else
                        option.DetectTime = new DBUtility.VariousData<DateTime>(DateTime.Now);

                    SOPScenarioManager.Instance.GetBarLevelTree().SelectNode(node);
                    FormSOP.Instance.RunWorkflow(option);
                });*/
            }
        }

        // 기존에 같은 SOP가 이미 진행되고 있을 경우 해당 SOP의 진도가 규모를 측정하여
        // 새로운 값보다 더 높은 값인지 검사한다.
        // 더 높거나 같은 값으로 이미 진행중인 SOP가 있을 경우 false를 리턴한다.
        // 기존에 진행되고 있는 SOP가 없을 경우 true를 리턴한다.
        // 기존에 진행되고 있는 SOP가 더 낮을 경우 기존 SOP를 강제 종료시키고 true를 리턴한다.
        /*private bool CheckPrevEarthquakeSOP(float fMagnitude, int nIntensity, int nActionStepID)
        {
            bool isReal = true;
            HistoryDisasterNoPosition noPos = UnE.SOP.History.HistoryManager.Instance.FindHistoryDisasterNoPosition(nActionStepID, true);

            if (noPos == null)
            {
                noPos = UnE.SOP.History.HistoryManager.Instance.FindHistoryDisasterNoPosition(nActionStepID, false);
                isReal = false;
            }

            if (noPos == null)
                return true;

            int nIndex1 = noPos.DisasterOptions.IndexOf('[');
            int nIndex2 = noPos.DisasterOptions.LastIndexOf(']');

            if (nIndex1 < 0 || nIndex2 <= nIndex1)
                return true;

            int nIndex3 = noPos.DisasterOptions.IndexOf(':', nIndex1 + 1);

            if (nIndex3 < 0)
                return true;

            int nIndex4 = noPos.DisasterOptions.IndexOf('/', nIndex3 + 1);

            if (nIndex4 < 0)
                return true;

            string strDisasterName = noPos.DisasterOptions.Substring(nIndex1 + 1, nIndex3 - nIndex1 - 1).Trim();
            string strTagName = noPos.DisasterOptions.Substring(nIndex3 + 1, nIndex4 - nIndex3 - 1).Trim();
            string strTagValue = noPos.DisasterOptions.Substring(nIndex4 + 1, nIndex2 - nIndex4 - 1).Trim();

            if (strTagName == UnE.SOP.Workstate.WorkflowOptionEarthquake.PowerMode.Intensity.ToString())
            {
                int prevIntensity = 0;

                if (int.TryParse(strTagValue, out prevIntensity))
                {
                    // 기존에 실행되고 있는 SOP의 진도가 더 크거나 같다.
                    if (prevIntensity >= nIntensity)
                        return false;
                }
            }
            else if (strTagName == UnE.SOP.Workstate.WorkflowOptionEarthquake.PowerMode.Magnitude.ToString())
            {
                float prevMagnitude = 0.0f;

                if (float.TryParse(strTagValue, out prevMagnitude))
                {
                    // 기존에 실행되고 있는 SOP의 규모가 더 크거나 같다.
                    if (prevMagnitude >= fMagnitude)
                        return false;
                }
            }

            // 기존의 SOP 강제종료 시키기
            UnE.SOP.Workstate.WorkFlow workFlow = UnE.SOP.Workstate.WorkFlowManager.Instance.Get(nActionStepID, isReal);

            if (workFlow == null)
                return false;

            workFlow.Done(DateTime.Now);
            return true;
        }*/

        private int GetEarthquakeLinkedActionStepID()
        {
            DBUtility.WebDBManager dbMgr = FormSOP.Instance.DBManager;

            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = 'EarthquakeLinkedSOP' and SiteID = " + ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            string strFullPath = DBUtility.WebDBManager.GetStringField(arrResult[0]);

            if (strFullPath == null || strFullPath.Length == 0)
                return -1;

            string[] tokens = strFullPath.Split('/');

            if (tokens.Count() < 3)
                return -1;

            string strCategoryName = tokens[0].Trim();
            string strSubCategoryName = tokens[1].Trim();
            string strDisasterName = tokens[2].Trim();

            string strFormat = "select d.ID, d.VersionID from Disaster as d, SubDisasterCategory as sdc, DisasterCategory as dc ";
            strFormat += "where dc.CategoryName = '{0}' and sdc.SubCategoryName = '{1}' and d.DisasterName = '{2}' ";
            strFormat += "and d.SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID and dc.SiteID = {3}";

            strSQL = string.Format(strFormat, strCategoryName, strSubCategoryName, strDisasterName, ProxySOP.Instance.SiteID);
            arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return -1;

            int nResultCount = arrResult.Count;
            int nDisasterID = -1, nVersionID = -1;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                DBUtility.VariousData<int> disasterID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString());
                DBUtility.VariousData<int> versionID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (disasterID == null || versionID == null)
                    continue;

                if (versionID.Data > nVersionID)
                {
                    nDisasterID = disasterID.Data;
                    nVersionID = versionID.Data;
                }
            }

            if (nDisasterID < 0)
                return -1;

            strSQL = "select id, StepName from ActionStep where DisasterID = " + nDisasterID.ToString();
            arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return -1;

            int nActionStepID = -1;
            nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                DBUtility.VariousData<int> id = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString());
                string strStepName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1].ToString());

                if (id == null || strStepName == null)
                    continue;

                if (strStepName == "대응")
                {
                    nActionStepID = id.Data;
                    break;
                }
                else if (nActionStepID < 0)
                    nActionStepID = id.Data;
            }

            return nActionStepID;
        }

        private void ProcessSimulatorCommand(ArrayList arrDatas)
        {
            if (arrDatas.Count == 0 || (arrDatas[0] is byte) == false)
                return;

            byte command = (byte)arrDatas[0];

            if (command == SOPSimulatorCommandType.RESET_USER_DEFINED_TEAM_NAMES)
            {
                if (arrDatas.Count == 2 && (arrDatas[1] is int))
                {
                    int nActionStepHistoryID = (int)arrDatas[1];
                    FormSOP.Instance.GetPageHome().SOPTeamMemberManager.ResetUserDefinedTeamNames(nActionStepHistoryID);
                }
            }
        }

        private void ProcessAreYouThere(ArrayList arrDatas)
        {
            arrDatas.Clear();
            arrDatas.Add(FormSOP.Instance.HasControl);

            byte[] bytes = MakeBytes(TCP_ID.I_AM_HERE, arrDatas);

            m_mgr.Send(bytes, this);
        }

        private void ProcessChangedConfig(ArrayList arrDatas)
        {
            if (arrDatas == null)
                return;

            if (arrDatas.Count < 3)
                return;

            try
            {
                byte byteClientType = (byte)arrDatas[0];
                string strPropertyName = (string)arrDatas[1];
                string strPropertyValue = (string)arrDatas[2];

                if (byteClientType == TCP_CLIENT.SDMS_CLIENT && strPropertyName == SOP.SDMSConfig.PropertyName)
                {
                    int nConfigValue;

                    if (int.TryParse(strPropertyValue, out nConfigValue))
                    {
                        if (((nConfigValue & (int)SOP.SDMSConfig.ConfigType.COMPANY_MEMBER) == (int)SOP.SDMSConfig.ConfigType.COMPANY_MEMBER) ||
                            ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.REGULAR_TEAM) == (int)SOP.SDMSConfig.ConfigType.REGULAR_TEAM) ||
                            ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.TEMPORARY_MEMBER) == (int)SOP.SDMSConfig.ConfigType.TEMPORARY_MEMBER) ||
                            ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.TEMPARARY_NORMAL_TEAM) == (int)SOP.SDMSConfig.ConfigType.TEMPARARY_NORMAL_TEAM) ||
                            ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.TEMPARAY_EMERGENCY_TEAM) == (int)SOP.SDMSConfig.ConfigType.TEMPARAY_EMERGENCY_TEAM) ||
                            ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.EXTERNAL_MEMBER) == (int)SOP.SDMSConfig.ConfigType.EXTERNAL_MEMBER) ||
                            ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.EXTERNAL_TEAM) == (int)SOP.SDMSConfig.ConfigType.EXTERNAL_TEAM))
                            ProcessChangeCompanyMember();

                        if (((nConfigValue & (int)SOP.SDMSConfig.ConfigType.EXTERNAL_MEMBER) == (int)SOP.SDMSConfig.ConfigType.EXTERNAL_MEMBER) ||
                            ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.EXTERNAL_TEAM) == (int)SOP.SDMSConfig.ConfigType.EXTERNAL_TEAM))
                            ProcessChangeExternalMember();
                    }
                }
                else if (byteClientType == TCP_CLIENT.SOP_SIMULATOR)
                {
                    if (strPropertyName == SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.USE_BROADCAST))
                    {
                        int nValue;

                        if (int.TryParse(strPropertyValue, out nValue))
                        {
                            FormSOP.Instance.UseBroadcast = nValue == 0 ? false : true;
                        }
                    }
                    else if (strPropertyName == SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.USE_SMS))
                    {
                        int nValue;

                        if (int.TryParse(strPropertyValue, out nValue))
                        {
                            FormSOP.Instance.SMSOn = nValue == 0 ? false : true;
                        }
                    }
                    else if (strPropertyName == SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.SMS_TO_EXTERNAL_MEMBER))
                    {
                        int nValue;

                        if (int.TryParse(strPropertyValue, out nValue))
                        {
                            FormSOP.Instance.SmsExternalCompanyMemberOn = nValue == 0 ? false : true;
                        }
                    }
                    else if (strPropertyName == SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.WORKING_BEGIN_HOUR))
                    {
                        int nHour, nMinute;

                        if (ParseTime(strPropertyValue, out nHour, out nMinute))
                        {
                            PageBackstageOption pageOption = FormSOP.Instance.GetPageOption();
                            pageOption.BeginHour = nHour;
                            pageOption.BeginMinute = nMinute;
                        }
                    }
                    else if (strPropertyName == SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.WORKING_END_HOUR))
                    {
                        int nHour, nMinute;

                        if (ParseTime(strPropertyValue, out nHour, out nMinute))
                        {
                            PageBackstageOption pageOption = FormSOP.Instance.GetPageOption();
                            pageOption.EndHour = nHour;
                            pageOption.EndMinute = nMinute;
                        }
                    }
                }
            }
            catch (Exception e)
            {
				ConnectionLogEx.Instance.WriteLine(e.StackTrace);
            }
        }

        private bool ParseTime(string strValue, out int nHour, out int nMinute)
        {
            nHour = nMinute = 0;
            int nIndex = strValue.IndexOf(':');

            if (nIndex <= 0 || nIndex == strValue.Length - 1)
                return false;

            string strHour = strValue.Substring(0, nIndex);
            string strMinute = strValue.Substring(nIndex + 1);

            if (!int.TryParse(strHour, out nHour))
                return false;

            if (!int.TryParse(strMinute, out nMinute))
                return false;

            return true;
        }

        private void ProcessIgnoreSOP(ArrayList arrDatas)
        {
            if (arrDatas.Count < 1)
                return;

            int nSensorHistoryID = (int)arrDatas[0];

            m_mgr.RemoveSensorHistory(nSensorHistoryID);
        }

        private void ProcessGiveControlKey(ArrayList arrDatas)
        {
            if (arrDatas.Count < 1)
                return;

            int nSOPGenUserID = (int)arrDatas[0];

            // nSOPGenUserID가 현재 로그인된 UserID와 동일하면 제어권을 획득한다.
            if (ProxySOP.Instance.SOPGenUserID == nSOPGenUserID)
                ObtainControl();
            else
                LoseControl();
        }

        private void ProcessCurrentSelectMission(byte[] bytes)
        {
            int nActionStepHistory, nReal, nCompHistory;
            string strRowIndex;
            int nIndex = 6;

            if (!GetChunkDatai(bytes, ref nIndex, out nActionStepHistory))
                return;

            if (!GetChunkDatai(bytes, ref nIndex, out nReal))
                return;

            if (!GetChunkDatai(bytes, ref nIndex, out nCompHistory))
                return;

            if (!GetChunkDatas(bytes, ref nIndex, out strRowIndex))
                return;

            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                if (FormSOP.Instance.HasControl == false)
                {
                    PageBackstageSOP page = FormSOP.Instance.GetPageHome();
                    if (page != null && page.Visible == true)
                    {
                        page.OnCurrentSelectedMission(nActionStepHistory, nReal, nCompHistory, strRowIndex);
                    }
                }
            });
        }

        private void ProcessSelectMission(byte[] bytes)
        {
            //int nActionStepHistory = BitConverter.ToInt32(bytes, 11);
            //int nReal = BitConverter.ToInt32(bytes, 20);
            //int nCompHistory = BitConverter.ToInt32(bytes, 29);
            //string strRowIndex = BitConverter.ToString(bytes, 38);


            int nActionStepHistory, nReal, nComponentID;
            string strRowIndex;
            int nIndex = 6;

            if (!GetChunkDatai(bytes, ref nIndex, out nActionStepHistory))
                return;

            if (!GetChunkDatai(bytes, ref nIndex, out nReal))
                return;

            if (!GetChunkDatai(bytes, ref nIndex, out nComponentID))
                return;

            if (!GetChunkDatas(bytes, ref nIndex, out strRowIndex))
                return;

            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                if (FormSOP.Instance.HasControl == false)
                {
                    PageBackstageSOP page = FormSOP.Instance.GetPageHome();
                    if (page != null && page.Visible == true)
                    {
                        //page.OnSelectMission(nActionStepHistory, nReal, nComponentID, strRowIndex);
                        page.OnCurrentSelectedMission(nActionStepHistory, nReal, nComponentID, strRowIndex);
                    }
                }
            });
        }

		private void ProcessChangeCompanyMember()
		{
            FormSOP.Instance.SOPManager.LoadRegularMember();
		}

        private void ProcessChangeExternalMember()
        {
            FormSOP.Instance.SOPManager.LoadExternalCompany();
        }

        private void ProcessRejectRequestControl()
        {
            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                FormSOP.Instance.CloseRequestProgress();
            });
        }

        private bool GetChunkDatai(byte[] bytes, ref int nIndex, out int nData)
        {
            nData = 0;

            if (bytes.Length < nIndex + 9)
                return false;

            if (bytes[nIndex] != TCP_TYPE.INTEGER)
                return false;

            int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

            if (nDataLength != 4)
                return false;

            nData = BitConverter.ToInt32(bytes, nIndex + 5);
            nIndex += 9;

            return true;
        }

        private bool GetChunkDatas(byte[] bytes, ref int nIndex, out string strData)
        {
            strData = "";

            if (bytes.Length < nIndex + 5)
                return false;

            if (bytes[nIndex] != TCP_TYPE.STRING)
                return false;

            int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

            if (nDataLength < 0)
                return false;

            if (nDataLength == 0)
                return true;

            if (bytes.Length < nIndex + 5 + nDataLength)
                return false;

            byte[] bytesBlock = new byte[nDataLength];
            System.Buffer.BlockCopy(bytes, nIndex + 5, bytesBlock, 0, nDataLength);
            strData = Encoding.UTF8.GetString(bytesBlock, 0, nDataLength);

            nIndex += 5 + nDataLength;

            return true;
        }

        private void ProcessRequestControl(byte[] bytes)
        {
            if (!FormSOP.Instance.HasControl)
                return;

            //int nUserID;
            string strUserID;
            string strUserName, strUserNickName, strIP;
            int nIndex = 6;

            //if (!GetChunkDatai(bytes, ref nIndex, out nUserID))
            if (!GetChunkDatas(bytes, ref nIndex, out strUserID))
                return;

            if (!GetChunkDatas(bytes, ref nIndex, out strUserName))
                return;

            if (!GetChunkDatas(bytes, ref nIndex, out strUserNickName))
                return;

            if (!GetChunkDatas(bytes, ref nIndex, out strIP))
                return;

            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                FormSOP.Instance.ShowRequestControl(strUserID, strUserName, strUserNickName, strIP);
            });
        }

        private void LoseControl()
        {
            

            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                try
                {
                     FormSOP.Instance.SetControl(false);
                     ScriptProxy.Instance.UserObject.SupervisorSOPLostControlAuthority.Invoke();

                }
                catch(Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine("LoseControl");
                    System.Diagnostics.Trace.WriteLine("Exception : " + ex.Message);
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                } 
            });

            SendData((short)TCP_ID.CONFIRM_TAKE_CONTROL);
        }

        private void ObtainControl()
        {

            SendData((short)TCP_ID.CONFIRM_GIVE_CONTROL);

            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                try
                {
                    FormSOP.Instance.CloseRequestProgress();
                    FormSOP.Instance.SetControl(true);
                    
                    
                    ScriptProxy.Instance.UserObject.SupervisorSOPObtainControlAuthority.Invoke();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine("LoseControl");
                    System.Diagnostics.Trace.WriteLine("Exception : " + ex.Message);
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                }  
            });

        }

        private void SendWhoIAm()
        {
            byte[] clientTypeBytes = MakeBytes((int)ClientType.SOP_SIMULATOR);
            byte[] userIDBytes = MakeBytes(ProxySOP.Instance.SOPGenUserID);
            byte[] userLevelBytes = MakeBytes(ProxySOP.Instance.SOPUserLevel);
            byte[] userRealNameBytes = MakeBytes(ProxySOP.Instance.SOPUserName);

            // WhoIAm과 함께 제어권 소유 여부를 서버에 알리는 이유는
            // 네트웍 오류로 인하여 잠시 접속이 끊어졌다 재개되는 경우
            // 제어권 박탈과 부여가 일어나는 현상을 방지하기 위해서다.
            // [2013/11/01] 김지웅
            byte[] hasControlBytes = MakeBytes(FormSOP.Instance.HasControl);

            int nChunkCount = 5;
            byte[] chunkCountBytes = BitConverter.GetBytes(nChunkCount);

            int nLen = chunkCountBytes.Length + clientTypeBytes.Length + userIDBytes.Length + 
                userLevelBytes.Length + userRealNameBytes.Length + hasControlBytes.Length + 2;

            byte[] bytes = new byte[nLen];

            bytes[0] = TCP_ID.WHO_I_AM;
            bytes[1] = 0;

            int nIndex = 2;

            CopyBlock(chunkCountBytes, 0, bytes, ref nIndex, chunkCountBytes.Length);
            CopyBlock(clientTypeBytes, 0, bytes, ref nIndex, clientTypeBytes.Length);
            CopyBlock(userIDBytes, 0, bytes, ref nIndex, userIDBytes.Length);
            CopyBlock(userLevelBytes, 0, bytes, ref nIndex, userLevelBytes.Length);
            CopyBlock(userRealNameBytes, 0, bytes, ref nIndex, userRealNameBytes.Length);
            CopyBlock(hasControlBytes, 0, bytes, ref nIndex, hasControlBytes.Length);

            if (this.IsClientDisposed == false)
                m_mgr.Send(bytes, this);
        }

        private void NeedToUpdateWorkingMemberData()
        {
            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                FormSOP.Instance.NeedToUpdateWorkingMemberData();
            });
        }


        public static void CopyBlock(byte[] srcBytes, int nSrcIndex, byte[] trgBytes, ref int nTrgIndex, int nCopyLength)
        {
            System.Buffer.BlockCopy(srcBytes, nSrcIndex, trgBytes, nTrgIndex, nCopyLength);
            nTrgIndex += nCopyLength;
        }
        private void ProcessClearDetect(byte[] bytes)
        {
            int nSensorHistoryID = BitConverter.ToInt32(bytes, 11);
            FireDetectSignal signal = m_mgr.FindDetectSignal(nSensorHistoryID);
            if (signal != null)
                m_mgr.RemoveDetectSignal(signal);

            SOPMonitoringSystem.Popup.PopupSensorOn popup = SOPMonitoringSystem.Popup.PopupSensorOn.Instance;




            if (popup.Visible == true)
            {
                if (popup.SensorHistoryID == nSensorHistoryID)
                {
                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        popup.Visible = false;
                        m_mgr.ShowDetectSignal();
                    });
                    
                }
                
            }
        }

        private void ProcessSensorReactionHistory(byte[] bytes)
        {
            int nSensorReactionHistoryID = BitConverter.ToInt32(bytes, 11);
            int nSensorHistoryID = BitConverter.ToInt32(bytes, 20);
            int nReactionType = BitConverter.ToInt32(bytes, 29);

               if (nReactionType == (int)ReactionType.RUN_SOP ||
                nReactionType == (int)ReactionType.IGNORE_SOP)
            {
                FireDetectSignal signal = m_mgr.FindDetectSignal(nSensorHistoryID);

                if (signal != null)
                    m_mgr.RemoveDetectSignal(signal);
            }
        }

        private void ProcessFireSensorSignal(byte[] bytes)
        {
            int nSensorID = BitConverter.ToInt32(bytes, 11);
            int nSensorHistoryID = BitConverter.ToInt32(bytes, 20);
            int nEquipZoneID = BitConverter.ToInt32(bytes,29);
            DateTime detectTime = DateTime.FromBinary(BitConverter.ToInt64(bytes, 38));
            float x = BitConverter.ToSingle(bytes, 51);
            float y = BitConverter.ToSingle(bytes, 60);
            float z = BitConverter.ToSingle(bytes, 69);
			int bReal = BitConverter.ToInt32(bytes, 78);

			// 수동신고의 경우 sensor id가 0일 수 있다
            if (nSensorHistoryID < 0)
                return;

            FireDetectSignal signal = m_mgr.FindDetectSignal(nSensorHistoryID);

            if (signal != null)
                return;

            signal = new FireDetectSignal(nSensorID, nSensorHistoryID, nEquipZoneID, detectTime, x, y, z);
			signal.RealMode = (bReal == 1 ? false : true);
            m_mgr.AddDetectSignal(signal);

            SOPMonitoringSystem.Popup.PopupSensorOn popup = SOPMonitoringSystem.Popup.PopupSensorOn.Instance;

            if (popup.Visible == false)
            {
                if (SDMS.FormMain.Instance.UsePopupSensorOn)
                {
                    FormSOP.Instance.Invoke((MethodInvoker)delegate
                    {
                        SOPMonitoringSystem.Popup.PopupSensorOn.PopUpForm(FormSOP.Instance.DBManager, signal, FormSOP.Instance.HasControl);
                    });
                }
            }
        }

		private bool CheckValidation(byte[] bytes)
		{
			int length = bytes.Length;
			if (length < 6)
				return false;

			int nChunkCount = (int)BitConverter.ToInt16(bytes, 2);
			int nIndex = 6;

			for (int i = 0; i < nChunkCount; i++)
			{
				if (length < nIndex + 5)
					return false;

				int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

				if (length < nIndex + 5 + nDataLength)
					return false;

				nIndex += 5 + nDataLength;
			}

			if (length > nIndex)
			{
				byte[] bytes1 = new byte[nIndex];
				byte[] bytes2 = new byte[length - nIndex];

				Array.Copy(bytes, bytes1, nIndex);
				Array.Copy(bytes, nIndex, bytes2, 0, length - nIndex);

				OnReceive(bytes1);

				if (!OnReceive(bytes2))
					return false;

				m_arrReceived = null;
				return false;
			}

			return true;
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

			if (Client != null && this.Client.Client != null)
			{
				if (this.Client.Client.Connected == true)
					m_mgr.Send(bytes, this);
			}			
		}

		public void SendData(short header, byte dataHeader, byte[] datas)
		{
			if (header < 0)
				return;

			if (datas.Length >= 10000)
				return;
			if (datas == null || datas.Length == 0)
				return;

			byte[] sndData = new byte[datas.Length + 11];

			byte[] nHader = BitConverter.GetBytes(header);
			byte[] nCount = BitConverter.GetBytes(1);

			// SET MESSAGE HeADER
			sndData[0] = nHader[0];
			sndData[1] = nHader[1];

			// SET DATA COUNT
			sndData[2] = nCount[0];
			sndData[3] = nCount[1];
			sndData[4] = nCount[2];
			sndData[5] = nCount[3];

			// SET DATA TYPE
			sndData[6] = dataHeader;

			// SET DATA LENGTH
			byte[] lengthData = BitConverter.GetBytes(datas.Length);
			for (int i = 0; i < 4; i++)
			{
				if (lengthData.Length > i)
				{
					sndData[7 + i] = lengthData[i];
				}
			}

			// SET DATA
			for (int i = 0; i < datas.Length; i++)
			{
				sndData[i + 11] = datas[i];
			}

			if (this.IsClientDisposed == false)
				m_mgr.Send(sndData, this);
		}
				
        public static byte[] MakeBytes(int data)
        {
            int nDataLength = sizeof(int);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.INTEGER;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Count();

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

            int nCount = lengthBytes.Count();

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

            int nCount = lengthBytes.Count();

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

            int nCount = lengthBytes.Count();

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
            //byte[] dataBytes = new byte[data.Length * sizeof(char)];
            //System.Buffer.BlockCopy(data.ToCharArray(), 0, dataBytes, 0, dataBytes.Length);
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

        public static byte[] MakeBytes(bool data)
        {
            int nDataLength = sizeof(bool);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.BOOLEAN;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Count();

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(short data)
        {
            int nDataLength = sizeof(short);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.SHORT;

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

        public static byte[] MakeBytes(byte data)
        {
            int nDataLength = sizeof(byte);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.BYTE;

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
                else
                    return null;
            }

            return arrResult;
        }

        public override void OnDropConnection()
        {
            m_mgr.OnDropConnection();
            m_arrTemp = null;
        }

        public new void Close()
        {
            base.Close();
            m_arrTemp = null;
        }

        public void SendResetUserDefinedTeamNames(int nActionStepHistoryID)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(SOPSimulatorCommandType.RESET_USER_DEFINED_TEAM_NAMES);
            arrDatas.Add(nActionStepHistoryID);

            byte[] bytes = MakeBytes(TCP_ID.SOP_SIMULATOR_COMMAND, arrDatas);
            m_mgr.Send(bytes, this);
        }
    }
}
