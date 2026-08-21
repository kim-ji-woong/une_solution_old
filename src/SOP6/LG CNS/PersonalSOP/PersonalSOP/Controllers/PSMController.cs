using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PersonalSOP.Controllers
{
    using Models;
    using Network;

    public class PSMController : Controller
    {
        public ActionResult Alarm()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Alarm(PSMAlarm alarm)
        {
            // 필수 입력값 확인
            //if (ModelState.IsValid)
            {
                if (alarm.TankName == null || alarm.TankName.Length > 0)
                {
                    string strSOPFullPath = AlarmController.GetPSMSOPFullPath(alarm.TankAlarmData);

                    if (strSOPFullPath != null)
                    {
                        //string strSOPFullPath = System.Configuration.ConfigurationManager.AppSettings["psm_sop"].ToString();
                        double dTemp;
                        if (double.TryParse(alarm.TankAlarmData, out dTemp) == true)
                        {
                            // 서버에 전송
                            NetworkWebManager.Instance.AddMessage(new PSMMessage(alarm.TankName, dTemp, strSOPFullPath, alarm.RealMode));
                        }
                    } 
                }
                                
                NetworkWebManager.Instance.AddLostArticle(alarm.DeadCount, alarm.InjuryCount, alarm.LostCount, alarm.TankAlarmData);
                return RedirectToAction("Alarm", "PSM");
            }

            //return View(alarm);
        }

        [HttpPost]
        public ActionResult LostAricle(PSMAlarm alarm)
        {
            NetworkWebManager.Instance.AddLostArticle(alarm.DeadCount, alarm.InjuryCount, alarm.LostCount, alarm.TankTemp);
            return RedirectToAction("Alarm", "PSM");
        }

        public ActionResult InitLostArticle()
        {
            NetworkWebManager.Instance.InitLostArticle();
            return RedirectToAction("Alarm", "PSM");
        }

        [HttpPost]
        public ActionResult Injury(PSMAlarm alarm)
        {
            // 필수 입력값 확인
            //if (ModelState.IsValid)
            {
                // 서버에 전송
                NetworkWebManager.Instance.AddInjuryMessage(alarm.InjuryLocation, alarm.InjuryData, false);
                System.Diagnostics.Trace.WriteLine("인명피해 : " + alarm.InjuryLocation);
                System.Diagnostics.Trace.WriteLine("사고현황 : " + alarm.InjuryData);
                return RedirectToAction("Alarm", "PSM");
            }

            //return View(alarm);
        }

        [HttpPost]
        public ActionResult LostArticle(PSMAlarm alarm)
        {
            // 필수 입력값 확인
            //if (ModelState.IsValid)
            {
                // 서버에 전송
                NetworkWebManager.Instance.AddInjuryMessage(alarm.InjuryLocation, alarm.InjuryData, false);
                System.Diagnostics.Trace.WriteLine("사망자 수 : " + alarm.DeadCount);
                System.Diagnostics.Trace.WriteLine("부상자 수 : " + alarm.InjuryCount);
                System.Diagnostics.Trace.WriteLine("실종자 수 : " + alarm.LostCount);
                return RedirectToAction("Alarm", "PSM");
            }

            //return View(alarm);
        }
    }
}
