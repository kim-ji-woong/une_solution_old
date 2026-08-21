using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PersonalSOP.Controllers
{
    using Models;
    using Network;

    public class FireController : Controller
    {
        // GET: Fire
        public ActionResult Alarm()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Alarm(FireAlarm alarm, string submit)
        {
            // 필수 입력값 확인
            //if (ModelState.IsValid)
            {
                if (alarm.Location != null && alarm.Location.Length > 0)
                {
                    string strSOPFullPath = System.Configuration.ConfigurationManager.AppSettings["fire_sop"].ToString();

                    // 서버에 전송
                    NetworkWebManager.Instance.AddMessage(new FireMessage(alarm.Location, strSOPFullPath, alarm.RealMode));
                }
                return RedirectToAction("Alarm", "Fire");
            }

            //return View(alarm);
        }

        [HttpPost]
        public ActionResult Injury(FireAlarm alarm)
        {
            // 필수 입력값 확인
            //if (ModelState.IsValid)
            {
                // 서버에 전송
                NetworkWebManager.Instance.AddInjuryMessage(alarm.Location, alarm.DisasterInfo, true);
                System.Diagnostics.Trace.WriteLine("인명피해 : " + alarm.Location);
                System.Diagnostics.Trace.WriteLine("사고현황 : " + alarm.DisasterInfo);
                return RedirectToAction("Alarm", "Fire");
            }

            //return View(alarm);
        }
    }
}
