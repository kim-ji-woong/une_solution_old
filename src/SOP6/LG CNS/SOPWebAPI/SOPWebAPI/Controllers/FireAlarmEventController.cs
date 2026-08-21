using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DBUtility2;
using System.Collections;

namespace SOPWebAPI.Controllers
{
    using Models;

    public class FireAlarmEventController : ApiController
    {
        /// <summary>
        /// 화재신호가 감지되면 알람정보를 알려준다.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public HttpResponseMessage Post(FireParams param)
        {
            DateTime dtNow = DateTime.Now;
            ResponseResult result;
            FireAlarm alarm = MakeAlarm(param, dtNow, out result);

            if (alarm != null)
            {
                // 아직 SOPWebServer에 접속되지 않은 상태일수도 있으니 일단 Queue에 넣는다.
                Network.NetworkWebManager.Instance.AddAlarm(alarm);
                //Network.NetworkWebManager.Instance.SendFireSensorEvent(alarm);
            }
            else
                SaveFail(param, result.ErrorMessage, dtNow);

            var response = Request.CreateResponse<ResponseResult>(HttpStatusCode.Created, result);
            string uri = Url.Link(WebApiConfig.DEFAULT_ROUTE_NAME, new { id = "OK" });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        private FireAlarm MakeAlarm(FireParams param, DateTime timeStamp, out ResponseResult result)
        {
            // Alarm Log
            int nWebHistoryID = SaveFireAlarm(param, timeStamp);

            result = new ResponseResult();
            result.Success = false;
            result.ErrorMessage = "";

            FireAlarm alarm = MakeAlarm(nWebHistoryID, param.alarmID, param.alarmLevel, param.alarmPosition, param.alarmTime, param.onOff == 1, true, result);
            
            if (alarm != null)
                result.Success = true;

            return alarm;
        }

        public static FireAlarm MakeAlarm(int nWebHistoryID, string alarmID, int alarmLevel, string alarmPosition, string alarmTime, bool onOff, bool checkValidation = false, ResponseResult result = null)
        {
            Zone zone = DataManager.Instance.GetZone(alarmPosition);

            if (zone == null)
            {
                if (result != null)
                    result.ErrorMessage = string.Format("{0}는 알려지지 않은 영역 정보입니다.", alarmPosition);
                return null;
            }

            FireAlarm alarm = null;

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

                alarm = new FireAlarm();

                alarm.SensorTagID = nSensorTagID;
                alarm.SensorZoneID = nSensorZoneID;
                alarm.IsAlarmOn = true;
            }
            else
            {
                if (checkValidation)
                {
                    Alarm _alarm = Network.NetworkWebManager.Instance.GetAlarm(alarmID);

                    if (_alarm == null || (_alarm is PSMAlarm))
                    {
                        if (result != null)
                            result.ErrorMessage = string.Format("[{0}]에 대한 알람정보를 찾을수 없습니다.", alarmID);
                        return null;
                    }
                    else
                        alarm = (FireAlarm)_alarm;
                }
                else
                {
                    int nSensorTagID, nSensorZoneID;

                    if (Network.NetworkWebManager.Instance.GetSensorInfo(zone, out nSensorTagID, out nSensorZoneID) == false)
                    {
                        return null;
                    }

                    alarm = new FireAlarm();
                    alarm.SensorTagID = nSensorTagID;
                    alarm.SensorZoneID = nSensorZoneID;
                }

                alarm.IsAlarmOn = false;
            }

            alarm.WebHistoryID = nWebHistoryID;
            alarm.AlarmID = alarmID;
            alarm.Zone = zone;

            return alarm;
        }

        private int SaveFireAlarm(FireParams param, DateTime timeStamp)
        {
            int nPrevAlarmID = DataManager.Instance.GetMaxTableID(DataManager.AlarmHistoryTable);

            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", timeStamp.Year, timeStamp.Month, timeStamp.Day, timeStamp.Hour, timeStamp.Minute, timeStamp.Second);

            string strSQL = "Insert into " + DataManager.AlarmHistoryTable + " (ID, RecvTime, alarmID, alarmPosition, alarmTime, alarmLevel, onOff, SensorZoneHistoryID, alarmData) ";
            strSQL += string.Format("Select isnull(max(id) + 1, 1), '{0}', '{1}', '{2}', '{3}', {4}, {5}, NULL, NULL from " + DataManager.AlarmHistoryTable,
                strTime, param.alarmID, param.alarmPosition, param.alarmTime, param.alarmLevel, param.onOff);

            if (DataManager.Instance.DBManager.GetResultData(strSQL) == null)
                return -1;

            int nCurrentAlarmID = DataManager.Instance.GetMaxTableID(DataManager.AlarmHistoryTable);

            if (nCurrentAlarmID == nPrevAlarmID + 1)
                return nCurrentAlarmID;
            else
            {
                // 방금 삽입한 데이터의 ID를 알아내기 위하여 가장 최근에 생성된 5개의 데이터를 얻어와서 데이터를 비교한다.
                strSQL = "Select TOP 5 ID, RecvTime, alarmID, alarmPosition, alarmTime, alarmLevel, onOff, SensorZoneHistoryID from " + DataManager.AlarmHistoryTable + " order by ID desc";
                ArrayList arrResult = DataManager.Instance.DBManager.GetResultData(strSQL);

                if (arrResult == null)
                    return -1;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 9; i += 10)
                {
                    VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                    VariousData<DateTime> recvTime = WebDBManager.GetDateTimeField(arrResult[i + 1]);
                    string alarmID = WebDBManager.GetStringField(arrResult[i + 2]);
                    string alarmPos = WebDBManager.GetStringField(arrResult[i + 3]);
                    string alarmTime = WebDBManager.GetStringField(arrResult[i + 4]);
                    VariousData<int> alarmLevel = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                    VariousData<int> onOff = WebDBManager.GetIntField(arrResult[i + 6].ToString());
                    VariousData<int> sensorZoneHistoryID = WebDBManager.GetIntField(arrResult[i + 7].ToString());

                    if (id == null || recvTime == null || alarmID == null || alarmPos == null || alarmTime == null ||
                        alarmLevel == null || onOff == null)
                        continue;

                    if (sensorZoneHistoryID != null)
                        continue;

                    if (IsSameTime(recvTime.Data, timeStamp) && alarmID == param.alarmID && alarmPos == param.alarmPosition &&
                        alarmTime == param.alarmTime && alarmLevel.Data == param.alarmLevel && onOff.Data == param.onOff)
                        return id.Data;
                }
            }

            return -1;
        }

        public static bool IsSameTime(DateTime time1, DateTime time2)
        {
            if (time1.Year != time2.Year)
                return false;
            if (time1.Month != time2.Month)
                return false;
            if (time1.Day != time2.Day)
                return false;

            if (time1.Hour != time2.Hour)
                return false;
            if (time1.Minute != time2.Minute)
                return false;
            if (time1.Second != time2.Second)
                return false;

            return true;
        }

        private bool SaveFail(FireParams param, string strErrorMessage, DateTime timeStamp)
        {
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", timeStamp.Year, timeStamp.Month, timeStamp.Day, timeStamp.Hour, timeStamp.Minute, timeStamp.Second);

            string strSQL = "Insert into " + DataManager.AlarmFailHistoryTable + " (ID, RecvTime, alarmID, ErrorMessage, Description) ";
            strSQL += string.Format("Select isnull(max(id) + 1, 1), '{0}', '{1}', '{2}', NULL from " + DataManager.AlarmFailHistoryTable,
                strTime, param.alarmID, strErrorMessage);

            return DataManager.Instance.DBManager.GetResultData(strSQL) != null;
        }
    }
}
