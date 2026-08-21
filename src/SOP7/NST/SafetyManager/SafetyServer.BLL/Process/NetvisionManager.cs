using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.IO;
using dnsData.Sensor;
using Newtonsoft.Json.Linq;
using dnsData.Alarm;
using TeamEditor.Model.Sop.Team;
using SDMS.Model.History;
using SDMS.Model.Sensor;

namespace SafetyServer.BLL.Process
{
    public class NetvisionManager
    {
        public enum DisasterType { fire };

        private static string BaseURL = "";

        public static void SetBaseURL(string strURL)
        {
            if (strURL.EndsWith("/"))
                BaseURL = strURL.Substring(0, strURL.Length - 1);
            else
                BaseURL = strURL;
        }

        public void SendAlarmAsync(Facility.FacilityType sensorType, bool isAlarm, int? buildingID, int? fieldID, DateTime dtEvent, int alarmLevel, string strMessage, string strMemberID, AlarmData alarm, MainManager mainManager)
        {
            if (BaseURL == null || BaseURL.Length == 0)
                return;

            string strAlarmType = GetAlarmType(sensorType);

            if (strAlarmType == null)
                return;

            ArrayList arrDatas = new ArrayList();

            if (alarm.IsManual)
            {
                arrDatas.Add(strAlarmType);
                arrDatas.Add(isAlarm);
                arrDatas.Add(buildingID);
                arrDatas.Add(fieldID);
                arrDatas.Add(dtEvent);
                //arrDatas.Add(alarmLevel);
                arrDatas.Add(strMessage);

                RegularMember member = GetManualAlarmReporter(alarm, mainManager);

                if (member != null)
                    arrDatas.Add(member.MemberID);

                Thread t = new Thread(new ParameterizedThreadStart(SendManualAlarm));
                t.Start(arrDatas);
            }
            else
            {
                if (Facility.IsFireSensorType(sensorType))
                {
                    arrDatas.Add(strAlarmType);
                    arrDatas.Add(isAlarm);
                    arrDatas.Add(buildingID);
                    arrDatas.Add(fieldID);
                    arrDatas.Add(dtEvent);
                    arrDatas.Add(alarmLevel);
                    arrDatas.Add(strMessage);

                    Thread t = new Thread(new ParameterizedThreadStart(SendAlarm));
                    t.Start(arrDatas);
                }
                else if (!isAlarm && !Facility.IsFireSensorType(sensorType))
                {
                    if (alarm.Tag != null && alarm.Tag is bool)
                    {
                        bool signalFromSystem = (bool)alarm.Tag;

                        // 외부에서 받은 신호는 다른곳에서 처리하도록 한다.
                        if (signalFromSystem)
                            return;
                    }

                    SendResponseFromUserClearAlarm(alarm, mainManager);
                }
            }
        }

        private bool SendResponseFromUserClearAlarm(AlarmData alarm, MainManager mainManager)
        {
            string strErrorMessage;
            Dictionary<SensorReactionHistory.Fields, object> dicConditions = new Dictionary<SensorReactionHistory.Fields, object>();

            dicConditions[SensorReactionHistory.Fields.SensorZoneHistoryID] = alarm.SensorZoneHistoryID;
            dicConditions[SensorReactionHistory.Fields.ReactionType] = (int)SensorReactionHistory.ReactionTypes.BEGIN_STATUS;

            List<SensorReactionHistory> histories = mainManager.SDMSDataManager.GetSelectManager().SelectSensorReactionHistories(dicConditions, null, out strErrorMessage);

            if (histories == null)
                return false;

            string strCameraID = "";

            if (histories.Count > 0)
            {
                strCameraID = histories[0].Param4;
            }
            else
                return false;

            Dictionary<SensorZone.Fields, object> dicCondition1 = new Dictionary<SensorZone.Fields, object>();
            dicCondition1[SensorZone.Fields.ID] = alarm.SensorZoneID;

            ArrayList arrResult = mainManager.SDMSDataManager.GetSelectManager().JoinSensorZoneETCSensor(dicCondition1, null, null, out strErrorMessage);

            if (arrResult == null || arrResult.Count < 2)
                return false;

            ETC sensor = (ETC)arrResult[1];

            if (sensor.Department == null)
                return false;

            int nRegularMemberID;

            if (int.TryParse(sensor.Department, out nRegularMemberID) == false)
                return false;

            RegularMember member = mainManager.TeamDataManager.GetSelectManager().SelectRegularMember(nRegularMemberID, out strErrorMessage);

            if (member == null)
                return false;

            ArrayList arrDatas = new ArrayList();

            if (sensor.Name.ToLower().Contains("areaalarm"))
            {
                SendAreaAlarmAsync(strCameraID, member.MemberID, DateTime.Now, 0, alarm.Message);
                return true;
            }
            else if (sensor.Name.ToLower().Contains("noequipment"))
            {
                SendNoEquipmentAlarmAsync(strCameraID, member.MemberID, DateTime.Now, true, true, true, 0, alarm.Message);
                return true;
            }

            return false;
        }

        // 수동신고한 사람
        private RegularMember GetManualAlarmReporter(AlarmData alarm, MainManager mainManager)
        {
            if (alarm.ReactionHistoryParam3 != null && alarm.ReactionHistoryParam3.Length > 0)
            {
                int id;

                if (int.TryParse(alarm.ReactionHistoryParam3.Trim(), out id))
                {
                    string strErrorMessage;
                    return mainManager.TeamDataManager.GetSelectManager().SelectRegularMember(id, out strErrorMessage);
                }
            }

            return null;
        }

        private string GetString(string str)
        {
            if (str == null)
                return "null";

            return str;
        }

        public void SendAreaAlarmAsync(string strCameraID, string strUserID, DateTime dtEvent, int alarmLevel, string strMessage)
        {
            if (BaseURL == null || BaseURL.Length == 0)
                return;

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(strCameraID);
            arrDatas.Add(strUserID);
            arrDatas.Add(dtEvent);
            arrDatas.Add(alarmLevel);
            arrDatas.Add(strMessage);

            Thread t = new Thread(new ParameterizedThreadStart(SendAreaAlarm));
            t.Start(arrDatas);
        }

        public void SendNoEquipmentAlarmAsync(string strCameraID, string strUserID, DateTime dtEvent, bool helmet, bool shoes, bool belt, int alarmLevel, string strMessage)
        {
            if (BaseURL == null || BaseURL.Length == 0)
                return;

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(strCameraID);
            arrDatas.Add(strUserID);
            arrDatas.Add(dtEvent);
            arrDatas.Add(helmet);
            arrDatas.Add(shoes);
            arrDatas.Add(belt);
            arrDatas.Add(alarmLevel);
            arrDatas.Add(strMessage);

            Thread t = new Thread(new ParameterizedThreadStart(SendNoEquipmentAlarm));
            t.Start(arrDatas);
        }

        private void SendNoEquipmentAlarm(object param)
        {
            ArrayList arrDatas = (ArrayList)param;

            string strCameraID = (string)arrDatas[0];
            string strUserID = (string)arrDatas[1];
            DateTime dtEvent = (DateTime)arrDatas[2];
            bool helmet = (bool)arrDatas[3];
            bool shoes = (bool)arrDatas[4];
            bool belt = (bool)arrDatas[5];
            int alarmLevel = (int)arrDatas[6];
            string strMessage = (string)arrDatas[7];

            string strURL = BaseURL + "/missing_equipment_report";
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtEvent.Year, dtEvent.Month, dtEvent.Day, dtEvent.Hour, dtEvent.Minute, dtEvent.Second);

            JObject jsonInput = new JObject();

            jsonInput.Add("cameraID", strCameraID);
            jsonInput.Add("userID", strUserID);
            jsonInput.Add("time", strTime);
            jsonInput.Add("helmet", helmet);
            jsonInput.Add("shoes", shoes);
            jsonInput.Add("belt", belt);
            jsonInput.Add("level", alarmLevel);
            //jsonInput.Add("level", alarmLevel.ToString());
            jsonInput.Add("notifications", strMessage);

            string strJson = jsonInput.ToString();
            SendPost(strJson, strURL);
        }

        private void SendAreaAlarm(object param)
        {
            ArrayList arrDatas = (ArrayList)param;

            string strCameraID = (string)arrDatas[0];
            string strUserID = (string)arrDatas[1];
            DateTime dtEvent = (DateTime)arrDatas[2];
            int alarmLevel = (int)arrDatas[3];
            string strMessage = (string)arrDatas[4];

            string strURL = BaseURL + "/area_risk_alarm";
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtEvent.Year, dtEvent.Month, dtEvent.Day, dtEvent.Hour, dtEvent.Minute, dtEvent.Second);

            JObject jsonInput = new JObject();

            jsonInput.Add("cameraID", strCameraID);
            jsonInput.Add("userID", strUserID);
            jsonInput.Add("time", strTime);
            jsonInput.Add("level", alarmLevel);
            //jsonInput.Add("level", alarmLevel.ToString());
            jsonInput.Add("notifications", strMessage);

            string strJson = jsonInput.ToString();
            SendPost(strJson, strURL);
        }

        private void SendManualAlarm(object param)
        {
            ArrayList arrDatas = (ArrayList)param;

            string strAlarmType = (string)arrDatas[0];
            bool isAlarm = (bool)arrDatas[1];
            int? buildingID = (int?)arrDatas[2];
            int? fieldID = (int?)arrDatas[3];
            DateTime dtEvent = (DateTime)arrDatas[4];
            string strMessage = (string)arrDatas[5];

            string strReporterID = arrDatas.Count > 6 ? (string)arrDatas[6] : "";

            string strURL = BaseURL + "/broadcast_alarm";

            string strBuildingID = buildingID == null ? null : buildingID.ToString();
            string strFieldID = fieldID == null ? null : fieldID.ToString();
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtEvent.Year, dtEvent.Month, dtEvent.Day, dtEvent.Hour, dtEvent.Minute, dtEvent.Second);
            //string strAlarmStatus = isAlarm ? "true" : "false";

            JObject jsonInput = new JObject();

            JObject jsonAlarmType = new JObject();
            jsonAlarmType.Add(strAlarmType, true);

            jsonInput.Add("reporterID", strReporterID);
            jsonInput.Add("accident_type", jsonAlarmType);
            jsonInput.Add("buildingID", buildingID);
            jsonInput.Add("fieldID", fieldID);
            //jsonInput.Add("buildingID", strBuildingID);
            //jsonInput.Add("fieldID", strFieldID);
            jsonInput.Add("time", strTime);
            jsonInput.Add("status", isAlarm);
            //jsonInput.Add("status", strAlarmStatus);
            //jsonInput.Add("level", alarmLevel);
            //jsonInput.Add("level", alarmLevel.ToString());
            jsonInput.Add("notifications", strMessage);

            string strJson = jsonInput.ToString();
            
            SendPost(strJson, strURL);
        }

        private void SendAlarm(object param)
        {
            ArrayList arrDatas = (ArrayList)param;

            string strAlarmType = (string)arrDatas[0];
            bool isAlarm = (bool)arrDatas[1];
            int? buildingID = (int?)arrDatas[2];
            int? fieldID = (int?)arrDatas[3];
            DateTime dtEvent = (DateTime)arrDatas[4];
            int alarmLevel = (int)arrDatas[5];
            string strMessage = (string)arrDatas[6];

            string strURL = BaseURL + "/broadcast_alarm";

            string strBuildingID = buildingID == null ? null : buildingID.ToString();
            string strFieldID = fieldID == null ? null : fieldID.ToString();
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtEvent.Year, dtEvent.Month, dtEvent.Day, dtEvent.Hour, dtEvent.Minute, dtEvent.Second);
            //string strAlarmStatus = isAlarm ? "true" : "false";

            JObject jsonInput = new JObject();

            JObject jsonAlarmType = new JObject();
            jsonAlarmType.Add(strAlarmType, true);

            jsonInput.Add("accident_type", jsonAlarmType);
            jsonInput.Add("buildingID", buildingID);
            jsonInput.Add("fieldID", fieldID);
            //jsonInput.Add("buildingID", strBuildingID);
            //jsonInput.Add("fieldID", strFieldID);
            jsonInput.Add("time", strTime);
            jsonInput.Add("status", isAlarm);
            //jsonInput.Add("status", strAlarmStatus);
            jsonInput.Add("level", alarmLevel);
            //jsonInput.Add("level", alarmLevel.ToString());
            jsonInput.Add("notifications", strMessage);

            string strJson = jsonInput.ToString();
            /*string strJson = "\"accident_type\":{\"" + strAlarmType + "\": true}, ";
            strJson += "\"buildingID\": " + strBuildingID + ", ";
            strJson += "\"fieldID\": " + strFieldID + ", ";
            strJson += "\"time\": \"" + strTime + "\", ";
            strJson += "\"status\": " + strAlarmStatus + ", ";
            strJson += "\"level\": " + alarmLevel.ToString() + ", ";
            strJson += "\"notifications\": \"" + strMessage + "\"";*/

            SendPost(strJson, strURL);
        }

        public JObject SendRequestUserInfo(string strMemberID)
        {
            JObject jsonInput = new JObject();
            jsonInput.Add("id", strMemberID);

            string strJson = jsonInput.ToString();
            string strURL = BaseURL + "/user/info";

            return SendPost(strJson, strURL);
        }

        private JObject SendPost(string strJson, string strURL)
        {
            JObject json = null;
            string resResult = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strURL));
            request.Method = "POST";
            request.ContentType = "application/json";
            //request.ContentLength = strJson.Length;

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strJson);
            //System.Diagnostics.Trace.WriteLine("length : " + bytes.Length);
            request.ContentLength = bytes.Length + 3;

            try
            {
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
                writer.Write(strJson);
                writer.Close();

                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                resResult = readerPost.ReadToEnd();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();
                Logger.Instance.Write("Send Response : " + strURL + ", " + strJson);
                System.Diagnostics.Trace.WriteLine("Success : " + resResult);

                json = JObject.Parse(resResult);
                return json;
            }
            catch (System.Net.WebException ex)
            {
                Logger.Instance.Write("Send Response Fail : " + strURL + ", " + ex.Message);
                System.Diagnostics.Trace.WriteLine("Fail : " + ex.Message);
            }

            return null;
        }

        private string GetAlarmType(Facility.FacilityType sensorType)
        {
            if (sensorType == Facility.FacilityType.FIRE_SENSOR)
                return "fire";
            else if (sensorType == Facility.FacilityType.ETC)
                return "etc";

            return null;
        }
    }
}
