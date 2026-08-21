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

    public class AlarmEventController : ApiController
    {
        public const string FireOn = "3";
        public const string FireOff = "0";

        /// <summary>
        /// 센서신호가 감지되면 알람정보를 알려준다.
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

            FireAlarm alarm = MakeAlarm(nWebHistoryID, param.dvcCd, param.dvcStatus, param.evtId, param.evtType, param.mapCd, param.floorId, true, result);

            if (alarm != null)
                result.Success = true;

            return alarm;
        }

        public static FireAlarm MakeAlarm(int nWebHistoryID, string dvcCd, string dvcStatus, string evtId, string evtType, string mapCd, string floorId, bool checkValidation = false, ResponseResult result = null)
        {
            Building building = DataManager.Instance.GetBuilding(mapCd);

            if (building == null)
            {
                if (result != null)
                    result.ErrorMessage = string.Format("{0}는 알수 없는 Map Code입니다.", mapCd);
                return null;
            }

            Zone zone = DataManager.Instance.GetZone(building, floorId);

            if (zone == null)
            {
                if (result != null)
                    result.ErrorMessage = string.Format("{0}로부터 층정보를 알아낼수 없습니다.", floorId);
                return null;
            }

            FireAlarm alarm = null;

            if (dvcStatus == FireOn)
            {
                if (checkValidation)
                {
                    alarm = Network.NetworkWebManager.Instance.GetFireAlarm(evtId);

                    if (alarm != null)
                    {
                        if (result != null)
                            result.ErrorMessage = string.Format("{0}는 현재 진행중인 알람에 대한 이벤트 ID입니다.", evtId);
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
                    alarm = Network.NetworkWebManager.Instance.GetFireAlarm(nSensorZoneID, zone);

                    if (alarm != null)
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
            else if (dvcStatus == FireOff)
            {
                if (checkValidation)
                {
                    alarm = Network.NetworkWebManager.Instance.GetFireAlarm(zone);

                    if (alarm == null)
                    {
                        if (result != null)
                            result.ErrorMessage = string.Format("[{0}]에 대한 알람정보를 찾을수 없습니다.", zone.Name);
                        return null;
                    }
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
            else
            {
                result.ErrorMessage = string.Format("[{0}]는 알수 없는 상태값입니다.", dvcStatus);
                return null;
            }

            alarm.WebHistoryID = nWebHistoryID;
            alarm.EquipCode = dvcCd;
            alarm.EquipStatus = dvcStatus;
            alarm.EventID = evtId;
            alarm.EventType = evtType;
            alarm.Zone = zone;

            return alarm;
        }

        private int SaveFireAlarm(FireParams param, DateTime timeStamp)
        {
            int nPrevAlarmID = DataManager.Instance.GetMaxTableID(DataManager.FireAlarmHistoryTable);

            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", timeStamp.Year, timeStamp.Month, timeStamp.Day, timeStamp.Hour, timeStamp.Minute, timeStamp.Second);

            string strSQL = "Insert into " + DataManager.FireAlarmHistoryTable + " (ID, RecvTime, dvcCd, dvcStatus, evtId, evtTime, evtType, mapCd, floorId, SensorZoneHistoryID) ";
            strSQL += string.Format("Select isnull(max(id) + 1, 1), '{0}', '{1}', '{2}', '{3}', '{4}', '{5}','{6}', '{7}', NULL from " + DataManager.FireAlarmHistoryTable,
                strTime, param.dvcCd, param.dvcStatus, param.evtId, param.evtTime, param.evtType, param.mapCd, param.floorId);

            if (DataManager.Instance.DBManager.GetResultData(strSQL) == null)
                return -1;

            int nCurrentAlarmID = DataManager.Instance.GetMaxTableID(DataManager.FireAlarmHistoryTable);

            if (nCurrentAlarmID == nPrevAlarmID + 1)
                return nCurrentAlarmID;
            else
            {
                // 방금 삽입한 데이터의 ID를 알아내기 위하여 가장 최근에 생성된 5개의 데이터를 얻어와서 데이터를 비교한다.
                strSQL = "Select TOP 5 ID, RecvTime, dvcCd, dvcStatus, evtId, evtTime, evtType, mapCd, floorId, SensorZoneHistoryID from " + DataManager.FireAlarmHistoryTable + " order by ID desc";
                ArrayList arrResult = DataManager.Instance.DBManager.GetResultData(strSQL);

                if (arrResult == null)
                    return -1;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 9; i += 10)
                {
                    VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                    VariousData<DateTime> recvTime = WebDBManager.GetDateTimeField(arrResult[i + 1]);
                    string dvcCd = WebDBManager.GetStringField(arrResult[i + 2]);
                    string dvcStatus = WebDBManager.GetStringField(arrResult[i + 3]);
                    string evtId = WebDBManager.GetStringField(arrResult[i + 4]);
                    string evtTime = WebDBManager.GetStringField(arrResult[i + 5]);
                    string evtType = WebDBManager.GetStringField(arrResult[i + 6]);
                    string mapCd = WebDBManager.GetStringField(arrResult[i + 7]);
                    string floorId = WebDBManager.GetStringField(arrResult[i + 8]);
                    string strSensorZoneHistoryID = WebDBManager.GetStringField(arrResult[i + 9]);

                    if (id == null || recvTime == null || dvcCd == null || dvcStatus == null || evtId == null ||
                        evtTime == null || evtType == null || mapCd == null || floorId == null)
                        continue;

                    if (strSensorZoneHistoryID != null)
                        continue;

                    if (IsSameTime(recvTime.Data, timeStamp) && dvcCd == param.dvcCd && dvcStatus == param.dvcStatus &&
                        evtId == param.evtId && evtTime == param.evtTime && evtType == param.evtType &&
                        mapCd == param.mapCd && floorId == param.floorId)
                        return id.Data;
                }
            }

            return -1;
        }

        private bool IsSameTime(DateTime time1, DateTime time2)
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

            string strSQL = "Insert into " + DataManager.FireAlarmFailHistoryTable + " (ID, RecvTime, evtId, ErrorMessage, Description) ";
            strSQL += string.Format("Select isnull(max(id) + 1, 1), '{0}', '{1}', '{2}', NULL from " + DataManager.FireAlarmFailHistoryTable,
                strTime, param.evtId, strErrorMessage);

            return DataManager.Instance.DBManager.GetResultData(strSQL) != null;
        }
    }
}
