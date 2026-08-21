using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Collections;
using DBUtility2;

namespace SOPWebAPI.Controllers
{
    using Models;

    public class PSMAlarmEventController : ApiController
    {
        /// <summary>
        /// 유해물질센서 신호가 감지되면 알람정보를 알려준다.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public HttpResponseMessage Post(PSMParams param)
        {
            DateTime dtNow = DateTime.Now;
            ResponseResult result;
            PSMAlarm alarm = MakeAlarm(param, dtNow, out result);

            if (alarm != null)
            {
                // 아직 SOPWebServer에 접속되지 않은 상태일수도 있으니 일단 Queue에 넣는다.
                Network.NetworkWebManager.Instance.AddAlarm(alarm);
                //Network.NetworkWebManager.Instance.SendPSMSensorEvent(alarm);
            }
            else
                SaveFail(param, result.ErrorMessage, dtNow);

            var response = Request.CreateResponse<ResponseResult>(HttpStatusCode.Created, result);
            string uri = Url.Link(WebApiConfig.DEFAULT_ROUTE_NAME, new { id = "OK" });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        private PSMAlarm MakeAlarm(PSMParams param, DateTime timeStamp, out ResponseResult result)
        {
            // Alarm Log
            int nWebHistoryID = SavePSMAlarm(param, timeStamp);

            result = new ResponseResult();
            result.Success = false;
            result.ErrorMessage = "";

            PSMAlarm alarm = MakeAlarm(nWebHistoryID, param.alarmID, Convert.ToInt32(param.alarmLevel), param.alarmName, param.alarmTime, param.alarmState == "1", param.alarmValue, true, result);

            if (alarm != null)
                result.Success = true;

            return alarm;
        }

        public static PSMAlarm MakeAlarm(int nWebHistoryID, string alarmID, int alarmLevel, string alarmPosition, string alarmTime, bool onOff, string strAlarmData, bool checkValidation = false, ResponseResult result = null)
        {
            Zone zone = DataManager.Instance.GetZone(alarmPosition);

            if (zone == null)
            {
                if (result != null)
                    result.ErrorMessage = string.Format("{0}는 알려지지 않은 영역 정보입니다.", alarmPosition);
                return null;
            }

            PSMAlarm alarm = null;

            if (onOff)
            {
                if (checkValidation)
                {
                    Alarm _alarm = Network.NetworkWebManager.Instance.GetAlarm(alarmID);

                    if (_alarm != null)
                    {
                        if (result != null)
                            result.ErrorMessage = string.Format("{0}는 현재 진행중인 알람에 대한 이벤트 ID입니다.", alarmID);
                        return null;
                    }
                }

                int nSensorTagID, nSensorZoneID;

                if (Network.NetworkWebManager.Instance.GetSensorInfo(zone, out nSensorTagID, out nSensorZoneID) == false)
                {
                    if (result != null)
                        result.ErrorMessage = string.Format("DB로부터 [{0}]에 대한 센서정보를 읽어오지 못하였습니다.", zone.Name);
                    return null;
                }

                if (checkValidation)
                {
                    Alarm _alarm = Network.NetworkWebManager.Instance.GetAlarm(nSensorZoneID);

                    if (_alarm != null)
                    {
                        if (result != null)
                            result.ErrorMessage = string.Format("[{0}]에 대한 알람이 이미 발생되어 있습니다.", zone.Name);
                        return null;
                    }
                }

                alarm = new PSMAlarm();

                alarm.SensorTagID = nSensorTagID;
                alarm.SensorZoneID = nSensorZoneID;
                alarm.IsAlarmOn = true;
            }
            else
            {
                if (checkValidation)
                {
                    Alarm _alarm = Network.NetworkWebManager.Instance.GetAlarm(alarmID);

                    if (_alarm == null || (_alarm is FireAlarm))
                    {
                        if (result != null)
                            result.ErrorMessage = string.Format("[{0}]에 대한 알람정보를 찾을수 없습니다.", alarmID);
                        return null;
                    }
                    else
                        alarm = (PSMAlarm)_alarm;
                }
                else
                {
                    int nSensorTagID, nSensorZoneID;

                    if (Network.NetworkWebManager.Instance.GetSensorInfo(zone, out nSensorTagID, out nSensorZoneID) == false)
                    {
                        return null;
                    }

                    alarm = new PSMAlarm();
                    alarm.SensorTagID = nSensorTagID;
                    alarm.SensorZoneID = nSensorZoneID;
                }

                alarm.IsAlarmOn = false;
            }

            alarm.WebHistoryID = nWebHistoryID;
            alarm.AlarmID = alarmID;
            alarm.Zone = zone;
            alarm.AlarmMessage = strAlarmData;

            return alarm;
        }

        private int SavePSMAlarm(PSMParams param, DateTime timeStamp)
        {
            int nPrevAlarmID = DataManager.Instance.GetMaxTableID(DataManager.AlarmHistoryTable);

            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", timeStamp.Year, timeStamp.Month, timeStamp.Day, timeStamp.Hour, timeStamp.Minute, timeStamp.Second);

            string strSQL = "Insert into " + DataManager.AlarmHistoryTable + " (ID, RecvTime, alarmID, alarmPosition, alarmTime, alarmLevel, onOff, SensorZoneHistoryID, alarmData, alarmName, alarmMsg, alarmLocation, tagId, deviceID, deviceName) ";
            strSQL += string.Format("Select isnull(max(id) + 1, 1), '{0}', '{1}', '{2}', '{3}', {4}, {5}, NULL, '{6}','{7}','{8}','{9}','{10}','{11}','{12}' from " + DataManager.AlarmHistoryTable,
                strTime, param.alarmID, param.alarmName, param.alarmTime, param.alarmLevel, param.alarmState, param.alarmValue, param.alarmName, param.alarmMsg, param.alarmLocation, param.tagID, param.deviceID, param.deviceID);

            if (DataManager.Instance.DBManager.GetResultData(strSQL) == null)
                return -1;

            int nCurrentAlarmID = DataManager.Instance.GetMaxTableID(DataManager.AlarmHistoryTable);

            if (nCurrentAlarmID == nPrevAlarmID + 1)
                return nCurrentAlarmID;
            else
            {
                // 방금 삽입한 데이터의 ID를 알아내기 위하여 가장 최근에 생성된 5개의 데이터를 얻어와서 데이터를 비교한다.
                strSQL = "Select TOP 5 ID, RecvTime, alarmID, alarmPosition, alarmTime, alarmLevel, onOff, SensorZoneHistoryID, alarmData from " + DataManager.AlarmHistoryTable + " order by ID desc";
                ArrayList arrResult = DataManager.Instance.DBManager.GetResultData(strSQL);

                if (arrResult == null)
                    return -1;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 8; i += 9)
                {
                    VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                    VariousData<DateTime> recvTime = WebDBManager.GetDateTimeField(arrResult[i + 1]);
                    string alarmID = WebDBManager.GetStringField(arrResult[i + 2]);
                    string alarmPos = WebDBManager.GetStringField(arrResult[i + 3]);
                    string alarmTime = WebDBManager.GetStringField(arrResult[i + 4]);
                    VariousData<int> alarmLevel = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                    VariousData<int> onOff = WebDBManager.GetIntField(arrResult[i + 6].ToString());
                    VariousData<int> sensorZoneHistoryID = WebDBManager.GetIntField(arrResult[i + 7].ToString());
                    string alarmData = WebDBManager.GetStringField(arrResult[i + 8]);

                    if (id == null || recvTime == null || alarmID == null || alarmPos == null || alarmTime == null ||
                        alarmLevel == null || onOff == null || alarmData == null)
                        continue;

                    if (sensorZoneHistoryID != null)
                        continue;

                    if (FireAlarmEventController.IsSameTime(recvTime.Data, timeStamp) && alarmID == param.alarmID && alarmPos == param.alarmName &&
                        alarmTime == param.alarmTime && alarmLevel.Data.ToString() == param.alarmLevel.ToString() && onOff.Data.ToString() == param.alarmState.ToString() &&
                        alarmData == param.alarmValue)
                        return id.Data;
                }
            }

            return -1;
        }

        private bool SaveFail(PSMParams param, string strErrorMessage, DateTime timeStamp)
        {
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", timeStamp.Year, timeStamp.Month, timeStamp.Day, timeStamp.Hour, timeStamp.Minute, timeStamp.Second);

            string strSQL = "Insert into " + DataManager.AlarmFailHistoryTable + " (ID, RecvTime, alarmID, ErrorMessage, Description) ";
            strSQL += string.Format("Select isnull(max(id) + 1, 1), '{0}', '{1}', '{2}', NULL from " + DataManager.AlarmFailHistoryTable,
                strTime, param.alarmID, strErrorMessage);

            return DataManager.Instance.DBManager.GetResultData(strSQL) != null;
        }
    }
}
