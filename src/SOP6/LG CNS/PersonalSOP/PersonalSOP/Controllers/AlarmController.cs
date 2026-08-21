using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PersonalSOP.Controllers
{
    using Models;
    using Network;

    public class AlarmController : Controller
    {
        private const string LastAlarmTag = "LastAlarm";
        private const string FireAlarm = "Fire";
        private const string PSMAlarm = "PSM";

        // GET: Alarm
        public ActionResult Fire()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Fire(FirePSMAlarm alarm, string submit)
        {
            string strSOPFullPath = System.Configuration.ConfigurationManager.AppSettings["fire_sop"].ToString();
            Session[LastAlarmTag] = FireAlarm;

            // 서버에 전송
            NetworkWebManager.Instance.AddMessage(new FireMessage(alarm.Location, strSOPFullPath, alarm.RealMode));   
            return RedirectToAction("Fire", "Alarm");
        }

        [HttpPost]
        public ActionResult PSM(FirePSMAlarm alarm, string submit)
        {
            string strSOPFullPath = GetPSMSOPFullPath(alarm.TankAlarmData);

            if (strSOPFullPath == null)
                return RedirectToAction("Fire", "Alarm");

            //string strSOPFullPath = System.Configuration.ConfigurationManager.AppSettings["psm_sop"].ToString();
            Session[LastAlarmTag] = PSMAlarm;

            double dTemp;

            if (double.TryParse(alarm.TankAlarmData, out dTemp) == false)
                return RedirectToAction("Fire", "Alarm");

            // 서버에 전송
            NetworkWebManager.Instance.AddMessage(new PSMMessage(alarm.TankName, dTemp, strSOPFullPath, alarm.RealMode));
            return RedirectToAction("Fire", "Alarm");
        }

        [HttpPost]
        public ActionResult Injury(FirePSMAlarm alarm)
        {
            // 서버에 전송
            NetworkWebManager.Instance.AddInjuryMessage(alarm.Location, alarm.DisasterInfo, IsFireInjury());
            System.Diagnostics.Trace.WriteLine("인명피해 : " + alarm.Location);
            System.Diagnostics.Trace.WriteLine("사고현황 : " + alarm.DisasterInfo);
            return RedirectToAction("Fire", "Alarm");
        }

        public static string GetPSMSOPFullPath(string strTankAlarmData)
        {
            string strSOPPath = System.Configuration.ConfigurationManager.AppSettings["psm_sop"].ToString();
            string strTempOption = System.Configuration.ConfigurationManager.AppSettings["temperature_option"].ToString();

            if (strTempOption == null || strTempOption.Length == 0)
                return null;

            float fTemperature;

            if (strTankAlarmData == null || float.TryParse(strTankAlarmData.Trim(), out fTemperature) == false)
                return null;

            string[] tokens = strTempOption.Split(';');

            foreach (string strToken in tokens)
            {
                int nIndex1 = strToken.IndexOf(':');
                int nIndex2 = strToken.IndexOf('~');

                if (nIndex1 < 0 || nIndex2 < 0)
                    continue;

                string strActionStepName = strToken.Substring(0, nIndex1).Trim();

                string strBegin = strToken.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1).Trim();
                string strEnd = strToken.Substring(nIndex2 + 1).Trim();

                float fBegin = -1, fEnd = -1;

                if (strBegin.Length > 0)
                {
                    if (float.TryParse(strBegin, out fBegin) == false)
                        continue;
                }

                if (strEnd.Length > 0)
                {
                    if (float.TryParse(strEnd, out fEnd) == false)
                        continue;
                }

                if (fBegin < 0 && fEnd < 0)
                    continue;
                else if (fBegin < 0)
                {
                    if (fTemperature < fEnd)
                        return strSOPPath + "/" + strActionStepName;
                }
                else if (fEnd < 0)
                {
                    if (fTemperature >= fBegin)
                        return strSOPPath + "/" + strActionStepName;
                }
                else
                {
                    if (fTemperature >= fBegin && fTemperature < fEnd)
                        return strSOPPath + "/" + strActionStepName;
                }
            }

            return null;
        }

        private bool IsFireInjury()
        {
            object alarm = Session[LastAlarmTag];

            if (alarm != null && alarm is string)
            {
                string strAlarm = (string)alarm;

                if (strAlarm == FireAlarm)
                    return true;
            }

            return false;
        }
    }
}
