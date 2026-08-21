using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgentFactory;
using DBUtility2;
using System.Collections;
using libSOPPolicy;
using System.Collections.Concurrent;

namespace ServerProcess.Data.SOP
{
    /// <summary>
    /// 알람발생으로 인한 SOP를 관리한다.
    /// </summary>
    public class AlarmSOPManager
    {
        private Dictionary<AlarmData, string> m_dicAlarmSOPFullPath = new Dictionary<AlarmData, string>();
        private Dictionary<AlarmData, BaseSOPUser> m_dicAlarmSOPUser = new Dictionary<AlarmData, BaseSOPUser>();

        /// <summary>
        /// SOP 처리가 되지 않은 알람들에 대한 byte Array를 생성한다.
        /// </summary>
        /// <param name="alarms"></param>
        /// <param name="clients"></param>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public byte[] MakeAlarmDataList(List<AlarmData> alarms, List<Client.ClientData> clients, DirectDBManager dbMgr)
        {
            List<AlarmData> oldAlarms = m_dicAlarmSOPFullPath.Keys.ToList();

            foreach (AlarmData alarm in alarms)
            {
                oldAlarms.Remove(alarm);
            }

            // 종료된 알람정보를 지운다.
            foreach (AlarmData closedAlarm in oldAlarms)
            {
                m_dicAlarmSOPFullPath.Remove(closedAlarm);
                m_dicAlarmSOPUser.Remove(closedAlarm);
            }

            ArrayList arrDatas = null;

            foreach (AlarmData alarm in alarms)
            {
                if (alarm.SOPProcess != AlarmData.SOPProcessType.None)
                    continue;

                string strSOPFullPath = null;

                if (m_dicAlarmSOPFullPath.TryGetValue(alarm, out strSOPFullPath) == false)
                    strSOPFullPath = null;
                else if (strSOPFullPath != null && strSOPFullPath.Length == 0)
                    continue;

                // 이전에 SOP를 실행중이던 User가 접속중인가?
                BaseSOPUser user = GetAlarmSOPUser(alarm, clients);

                if (user == null)
                {
                    foreach (SOPClientData client in clients)
                    {
                        if (strSOPFullPath == null)
                        {
                            if (client.User == null)
                                continue;
                            else
                            {
                                int manualReportZoneID = -1; // 수동신고로 발생한 알람은 Zone ID를 가지고 있다;
                                if (!int.TryParse(alarm.ReactionHistoryParam1, out manualReportZoneID))
                                    manualReportZoneID = -1;

                                strSOPFullPath = client.User.GetLinkedSOPFullPath(alarm.SensorZoneID, alarm.SensorType, alarm.Tag, dbMgr, SensorZoneManager.Instance, manualReportZoneID, alarm.AlarmDepth);

                                m_dicAlarmSOPFullPath[alarm] = strSOPFullPath;
                            }
                        }

                        if (strSOPFullPath == null)
                            break;

                        if (client.User == null)
                            continue;

                        if (client.User.AbletoAccess(alarm.SensorZoneID, dbMgr.SiteID, dbMgr))
                        {
                            user = client.User;
                            break;
                        }
                    }
                }

                if (strSOPFullPath == null || user == null)
                    continue;

                if (arrDatas == null)
                    arrDatas = new ArrayList();

                int nZoneID, nEquipZoneID;
                GetZoneNEquipZoneIDFromAlarm(alarm, out nZoneID, out nEquipZoneID);

                arrDatas.Add((int)alarm.SensorType);
                arrDatas.Add(nEquipZoneID);
                arrDatas.Add(nZoneID);
                arrDatas.Add(alarm.TimeStamp.ToBinary());
                arrDatas.Add(alarm.SensorZoneID);
                arrDatas.Add(alarm.SensorZoneHistoryID);
                arrDatas.Add(user.ID);
                arrDatas.Add(strSOPFullPath);
                arrDatas.Add(alarm.Message);
            }

            if (arrDatas == null)
                return null;

            return SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
        }

        public byte[] MakeSOPDataList(ConcurrentQueue<SOPRequest> sopRequests, List<Client.ClientData> clients, DirectDBManager dbMgr)
        {
            ArrayList arrDatas = null;
            SOPRequest request;
            List<SOPRequest> unprocessedRequest = null;
            UnE.SOP.ISupervisor supervisor = null;

            while (sopRequests.TryDequeue(out request))
            {
                bool processed = false;

                foreach (SOPClientData client in clients)
                {
                    if (client.User == null)
                        continue;

                    if (client.User.AbletoAccess(request.SOPFullPath, dbMgr.SiteID, dbMgr))
                    {
                        string[] tokens = request.SOPFullPath.Split('/');

                        if (tokens.Count() == 3)
                        {
                            if (supervisor == null)
                                supervisor = UnE.SOP.SupervisorFactory.MakeInstance(dbMgr);

                            string strActionStepName = supervisor.GetActionStepName(request.SOPFullPath, (int)request.SensorType);
                            request.SOPFullPath += "/" + strActionStepName;
                        }

                        if (arrDatas == null)
                            arrDatas = new ArrayList();

                        arrDatas.Add(request.RealMode);
                        arrDatas.Add(request.SOPFullPath);
                        arrDatas.Add(client.User.ID);

                        int nParamCount = request.GetParameterCount();
                        arrDatas.Add(nParamCount);

                        for (int i = 0; i < nParamCount; i++)
                        {
                            arrDatas.Add(request.GetParameter(i));
                        }

                        processed = true;
                        break;
                    }
                }

                if (processed == false)
                {
                    if (unprocessedRequest == null)
                        unprocessedRequest = new List<SOPRequest>();

                    unprocessedRequest.Add(request);
                }
            }

            if (unprocessedRequest != null)
            {
                foreach (SOPRequest sop in unprocessedRequest)
                {
                    sopRequests.Enqueue(sop);
                }
            }

            if (arrDatas == null)
                return null;

            return SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
        }

        private BaseSOPUser GetAlarmSOPUser(AlarmData alarm, List<Client.ClientData> clients)
        {
            BaseSOPUser user;

            if (m_dicAlarmSOPUser.TryGetValue(alarm, out user))
            {
                foreach (SOPClientData client in clients)
                {
                    if (client.User == user)
                        return user;
                }
            }

            return null;
        }

        private void GetZoneNEquipZoneIDFromAlarm(AlarmData alarm, out int nZoneID, out int nEquipZoneID)
        {
            nZoneID = nEquipZoneID = -1;

            BaseSMSManager.SMSMessageType messageType = ServerProcess.Data.SMSManager.ReactionTypeToMessageType(alarm.Status, alarm.SensorType);

            if (alarm.SensorZoneID >= SOPWebServer.Header.ManualReportDefaultID)
            {
                // 수동신고
                nZoneID = -1;
                int.TryParse(alarm.ReactionHistoryParam1, out nZoneID);
            }
            else
            {
                SensorZoneGroup group = SensorZoneManager.Instance.GetSensorZoneGroup(alarm.SensorZoneID);

                if (group != null && group.EquipmentZone != null)
                {
                    nEquipZoneID = group.EquipmentZone.ID;

                    if (group.EquipmentZone.LinkedZoneList.Count > 0)
                    {
                        UnE.Spatial.Zone zone = (UnE.Spatial.Zone)group.EquipmentZone.LinkedZoneList[0];
                        nZoneID = zone.ID;
                    }
                }
            }
        }
    }
}
