using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SafetyServer.BLL;
using SafetyServer.BLL.Data.Request;
using SafetyServer.BLL.Data.Response;
using SDMS.IDAL;
using dnsSopID;

namespace SafetyManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SafetyController : ControllerBase
    {
        private MainManager m_mainManager = null;

        public SafetyController(IDataManager dataManager, Common.IDAL.IDataManager commonDataManager, TeamEditor.IDAL.IDataManager teamDataManager)
        {
            m_mainManager = new MainManager(dataManager, commonDataManager, teamDataManager);
        }

        [HttpPost]
        public IActionResult RequestData([FromBody] RequestData data)
        {
            if (data == null)
                return BadRequest();

            if (data.RequestSpatialInfo != null)
                return RequestSpatialInfo();
            else if (data.ReportManualAlarm != null)
                return ReportManualAlarm(data.ReportManualAlarm);
            else if (data.LoginEvent != null)
                return LoginEvent(data.LoginEvent);
            else if (data.RequestUserPosition != null)
                return RequestUserPosition(data.RequestUserPosition.ID);
            else if (data.ReportAreaAlarm != null)
                return ReportAreaAlarm(data.ReportAreaAlarm);
            else if (data.ReportNoEquipmentAlarm != null)
                return ReportNoEquipmentAlarm(data.ReportNoEquipmentAlarm);
            else if (data.RequestFieldUserPosition != null)
                return RequestFieldUserPosition(data.RequestFieldUserPosition);
            else if (data.UpdateUserPosition != null)
                return UpdateUserPosition(data.UpdateUserPosition);

            return NotFound();
        }

        private IActionResult RequestSpatialInfo()
        {
            Logger.Instance.Write("Request from Client : RequestSpatialInfo");
            ResponseSpatialInfo result = m_mainManager.GetSpatialManager().GetSpatialInfo();
            return Ok(result);
        }

        public IActionResult ReportManualAlarm(RequestReportManualAlarm request)
        {
            string strLog = string.Format("accident_type : {0}, buildingID : {1}, fieldID : {2}, notifications : {3}, reporterID : {4}",
                request.accident_type.ToString(),
                request.BuildingID == null ? "null" : request.BuildingID.ToString(),
                request.FieldID == null ? "null" : request.FieldID.ToString(),
                request.Notifications == null ? "null" : request.Notifications,
                request.ReporterID == null ? "null" : request.ReporterID);

            Logger.Instance.Write("Request from Client : RequestReportManualAlarm, " + strLog);
            dnsData.Sensor.Facility.FacilityType sensorType = request.accident_type != null && request.accident_type.Fire ? dnsData.Sensor.Facility.FacilityType.FIRE_SENSOR : dnsData.Sensor.Facility.FacilityType.NONE;
            MessageResult result = m_mainManager.SensorManager.ProcessManualReport(sensorType, request.ReporterID, request.BuildingID, request.FieldID, request.Notifications);
            
            return Ok(result);
        }

        public IActionResult LoginEvent(LoginEvent request)
        {
            string strLog = string.Format("id : {0}, login : {1}",
                request.ID,
                request.Login);

            Logger.Instance.Write("Request from Client : LoginEvent, " + strLog);
            MessageResult result = m_mainManager.MemberManager.SetLoginEvent(request.ID, request.Login);
            return Ok(result);
        }

        public IActionResult RequestUserPosition(string strMemberID)
        {
            string strLog = string.Format("MemberID : {0}",
                strMemberID);

            Logger.Instance.Write("Request from Client : RequestUserPosition, " + strLog);

            ResponseUserPosition result = m_mainManager.MemberManager.GetUserPosition(strMemberID);
            return Ok(result);
        }

        public IActionResult UpdateUserPosition(UpdateUserPosition data)
        {
            string strLog = "";

            if (data.UserID == null)
                strLog = "UserID : null";
            else
                strLog = "UserID : " + data.UserID;

            if (data.FieldID == null)
                strLog += "\r\nFieldID : null";
            else
                strLog += "\r\nFieldID : " + data.FieldID;

            if (data.X == null)
                strLog += "\r\nX : null";
            else
                strLog += "\r\nX : " + data.Y;

            if (data.Y == null)
                strLog += "\r\nY : null";
            else
                strLog += "\r\nY : " + data.Y;

            Logger.Instance.Write("Request from Client : UpdateUserPosition, " + strLog);

            MessageResult result = m_mainManager.MemberManager.UpdateUserPosition(data);
            return Ok(result);
        }

        public IActionResult ReportAreaAlarm(ReportAreaAlarm alarm)
        {
            string strLog = string.Format("cameraID : {0}, level : {1}, notifications : {2}, time : {3}, userID : {4}",
                alarm.CameraID == null ? "null" : alarm.CameraID,
                alarm.Level,
                alarm.Notifications == null ? "null" : alarm.Notifications,
                alarm.Time == null ? "null" : alarm.Time,
                alarm.UserID);

            Logger.Instance.Write("Request from Client : ReportAreaAlarm, " + strLog);

            DateTime time;

            if (alarm.Time == null || alarm.Time.Length == 0)
                time = DateTime.Now;
            else
            {
                try
                {
                    time = Convert.ToDateTime(alarm.Time.Trim());
                }
                catch (Exception)
                {
                    time = DateTime.Now;
                }
            }

            MessageResult result = m_mainManager.SensorManager.ProcessAreaAlarm(alarm.UserID, alarm.CameraID, time, alarm.Level, alarm.Notifications);
            return Ok(result);
        }

        public IActionResult ReportNoEquipmentAlarm(ReportNoEquipmentAlarm alarm)
        {
            string strLog = string.Format("belt: {0}, cameraID : {1}, helmet : {2}, level : {3}, notifications : {4}, shoes : {5}, time : {6}, userID : {7}",
                alarm.Belt,
                alarm.CameraID == null ? "null" : alarm.CameraID,
                alarm.Helmet,
                alarm.Level,
                alarm.Notifications == null ? "null" : alarm.Notifications,
                alarm.Shoes,
                alarm.Time == null ? "null" : alarm.Time,
                alarm.UserID);

            Logger.Instance.Write("Request from Client : ReportNoEquipmentAlarm, " + strLog);

            DateTime time;

            if (alarm.Time == null || alarm.Time.Length == 0)
                time = DateTime.Now;
            else
            {
                try
                {
                    time = Convert.ToDateTime(alarm.Time.Trim());
                }
                catch (Exception)
                {
                    time = DateTime.Now;
                }
            }

            MessageResult result = m_mainManager.SensorManager.ProcessNoEquipmentAlarm(alarm.UserID, alarm.CameraID, time, alarm.Helmet, alarm.Shoes, alarm.Belt, alarm.Level, alarm.Notifications);
            return Ok(result);
        }

        public IActionResult RequestFieldUserPosition(RequestFieldUserPosition request)
        {
            string strLog = string.Format("fieldID: {0}, userIDs : {1}",
                request.FieldID == null ? "null" : request.FieldID.ToString(),
                ListToString(request.UserIDs));

            Logger.Instance.Write("Request from Client : RequestFieldUserPosition, " + strLog);

            ResponseFieldUserPosition response = m_mainManager.GetSpatialManager().GetFieldUserPosition(request.FieldID, request.UserIDs);
            return Ok(response);
        }

        public static string ListToString(List<string> datas)
        {
            if (datas == null)
                return "null";

            string str = "";

            foreach (string strData in datas)
            {
                if (str.Length == 0)
                    str = strData;
                else
                    str += ", " + strData;
            }

            return str;
        }
    }
}
