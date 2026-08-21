using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SOPWebAPI.Controllers
{
    using Models;

    public class CheckAlarmController : ApiController
    {
        /// <summary>
        /// 탐지된 알람정보가 실제 재난신호인지 여부를 알려준다.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public HttpResponseMessage Post(CheckParams param)
        {
            DateTime dtNow = DateTime.Now;
            ResponseResult result;
            FireAlarm alarm = GetAlarm(param, dtNow, out result);

            if (alarm != null)
                Network.NetworkWebManager.Instance.SendFireAlarmResult(alarm, param.isReal == 1, param.description);
            else
                SaveFail(param, result.ErrorMessage, dtNow);

            var response = Request.CreateResponse<ResponseResult>(HttpStatusCode.Created, result);
            string uri = Url.Link(WebApiConfig.DEFAULT_ROUTE_NAME, new { id = "OK" });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        private FireAlarm GetAlarm(CheckParams param, DateTime timeStamp, out ResponseResult result)
        {
            // Alarm Log
            SaveCheckAlarm(param, timeStamp);

            result = new ResponseResult();
            result.Success = false;
            result.ErrorMessage = "";

            FireAlarm alarm = Network.NetworkWebManager.Instance.GetFireAlarm(param.evtId);

            if (alarm == null)
            {
                result.ErrorMessage = string.Format("{0}는 이미 종료된 알람에 대한 ID이거나, 알수없는 이벤트 ID입니다.", param.evtId);
                return null;
            }

            result.Success = true;
            return alarm;
        }

        private bool SaveCheckAlarm(CheckParams param, DateTime timeStamp)
        {
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", timeStamp.Year, timeStamp.Month, timeStamp.Day, timeStamp.Hour, timeStamp.Minute, timeStamp.Second);

            string strReal = param.isReal == 1 ? "실제화재" : "오작동";

            string strSQL = "Insert into " + DataManager.FireAlarmHistoryTable + " (ID, RecvTime, dvcCd, dvcStatus, evtId, evtTime, evtType, mapCd, floorId, SensorZoneHistoryID) ";
            strSQL += string.Format("Select isnull(max(id) + 1, 1), '{0}', 'CheckAlarm', '{1}', '{2}', '', '','', '', NULL from " + DataManager.FireAlarmHistoryTable,
                strTime, strReal, param.evtId);

            return DataManager.Instance.DBManager.GetResultData(strSQL) != null;
        }

        private bool SaveFail(CheckParams param, string strErrorMessage, DateTime timeStamp)
        {
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", timeStamp.Year, timeStamp.Month, timeStamp.Day, timeStamp.Hour, timeStamp.Minute, timeStamp.Second);

            string strSQL = "Insert into " + DataManager.FireAlarmFailHistoryTable + " (ID, RecvTime, evtId, ErrorMessage, Description) ";
            strSQL += string.Format("Select isnull(max(id) + 1, 1), '{0}', '{1}', '{2}', 'CheckAlarm' from " + DataManager.FireAlarmFailHistoryTable,
                strTime, param.evtId, strErrorMessage);

            return DataManager.Instance.DBManager.GetResultData(strSQL) != null;
        }
    }
}
