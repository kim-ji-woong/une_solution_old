using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Web.Mvc;
using PersonalSOP.Models;
using PersonalSOP.Network;

namespace PersonalSOP.Controllers
{
    using Common;

    public class SOPBulletinController : Controller
    {
        /// <summary>
        /// ActionStepHistoryID, Index
        /// </summary>
        public static Dictionary<int, int> nCurrentIndex = new Dictionary<int, int>();
                
        // GET: SOPBulletin
        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult DisplayBulletin(int ash = -1)
        public ActionResult DisplayBulletin(string ash = "")
        {            
            //Session[ParameterManager.ActionStepHistoryID] = ash;            
            
            int nActionStepHistoryID, nUserID;
            ParameterManager.SetAccount(ash, "", Session, out nActionStepHistoryID, out nUserID);
            
            Session[ParameterManager.LastIndex] = -1;

            return View();
        }

        public ActionResult DisplayBulletin2()
        {
            int actionStepHistoryID = -1;
            object id = Session[ParameterManager.ActionStepHistoryID];

            if (id != null && id is int)
                actionStepHistoryID = (int)id;

            int lastIndex = -1;
            if (Session[ParameterManager.LastIndex] != null)
            {
                lastIndex = int.Parse(Session[ParameterManager.LastIndex].ToString());
            }
            
            int tempIndex = -1;
            if (nCurrentIndex.ContainsKey(actionStepHistoryID))
                tempIndex = nCurrentIndex[actionStepHistoryID];

            if (actionStepHistoryID <= 0)
                return new HttpStatusCodeResult(204);

            if (lastIndex > 0 && tempIndex <= lastIndex)
                return new HttpStatusCodeResult(204);

            List<Models.BulletinMessage> sopBulletins = new List<BulletinMessage>();

            StringBuilder sb = new StringBuilder();
            sb.Append("Select ID, TimeStamp, Title, Message, Image ");
            sb.Append("  From ActionStepHistoryMessage ");
            sb.AppendFormat("Where ActionStepHistoryID = {0} ", actionStepHistoryID);
            
            ArrayList arrResult = NetworkWebManager.Instance.DBMgr.GetResultData(sb.ToString());
            if (arrResult != null && arrResult.Count > 0)
            {
                for (int i = 0; i < arrResult.Count; i += 5)
                {
                    int nID = DBUtility2.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    DateTime dtTimeStamp = DBUtility2.WebDBManager.GetDateTimeField(arrResult[i + 1], DateTime.Now);
                    string strTitle = DBUtility2.WebDBManager.GetStringField(arrResult[i + 2], "");
                    string strMessage = DBUtility2.WebDBManager.GetStringField(arrResult[i + 3], "");
                    string strImagePath = DBUtility2.WebDBManager.GetStringField(arrResult[i + 4], "");

                    BulletinMessage sop = new BulletinMessage();
                    sop.ID = nID;
                    sop.Time = dtTimeStamp;
                    sop.Title = strTitle;
                    sop.Message = strMessage;
                    if (strImagePath.Length > 0)
                    {                        
                        //sop.ImagePath = NetworkWebManager.Instance.DBMgr.WebServerURL + ":5555" + strImagePath; // TODO: 포트 얻어오기
                        System.Net.Http.HttpRequestMessage msg = HttpContext.Items["MS_HttpRequestMessage"] as System.Net.Http.HttpRequestMessage;
                        sop.ImagePath = "http://" + msg.Headers.Host + strImagePath;
                    }

                    Session[ParameterManager.LastIndex] = nID;
                    
                    sopBulletins.Add(sop);
                }
            }
            
            return View(sopBulletins);
        }
    }
}