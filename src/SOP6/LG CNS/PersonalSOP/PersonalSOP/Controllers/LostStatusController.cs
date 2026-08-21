using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DBUtility2;
using System.Collections;

namespace PersonalSOP.Controllers
{
    using Models;

    public class LostStatusController : Controller
    {
        private const string ProcessingTag = "StatusProcessing";
        private const string CurrentStatusTag = "CurrentStatus";
        private const string CurrentArticleNoTag = "CurrentArticleNo";
        private const string InjuryStatusTag = "InjuryStatus";

        // GET: LostAll
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Index2()
        {
            if (IsProcessing())
                return new HttpStatusCodeResult(204);

            SetProcessing(true);

            try
            {
                LostStatus status = null;
                string strInjuryStatus, strCurrentInjuryStatus;
                GetStatus(out status, out strInjuryStatus);

                LostStatus.State state = ReadNewArticles(ref status, out strCurrentInjuryStatus);
                SetStatus(status, strCurrentInjuryStatus);

                SetProcessing(false);

                if (state == LostStatus.State.NoChanged || status == null)
                {
                    if (strInjuryStatus == strCurrentInjuryStatus || strCurrentInjuryStatus.Length == 0)
                        return new HttpStatusCodeResult(204);
                    else
                    {
                        this.ControllerContext.HttpContext.Response.Write("[CS]" + strCurrentInjuryStatus);

                        HttpStatusCodeResult result = new HttpStatusCodeResult(System.Net.HttpStatusCode.PartialContent);
                        result.ExecuteResult(this.ControllerContext);
                        return result;
                    }
                }
                else if (state == LostStatus.State.Initialized)
                    status.Articles.Clear();

                return View(status);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                SetProcessing(false);
            }

            return new HttpStatusCodeResult(204);
        }

        private void SetStatus(LostStatus status/*, int nArticleNo*/, string strInjuryStatus)
        {
            Session[CurrentStatusTag] = status;
            //Session[CurrentArticleNoTag] = nArticleNo;
            Session[InjuryStatusTag] = strInjuryStatus;
        }

        private void GetStatus(out LostStatus status, /*out int nArticleNo,*/ out string strInjuryStatus)
        {
            object _status = Session[CurrentStatusTag];
            //object articleNo = Session[CurrentArticleNoTag];
            object injuryStatus = Session[InjuryStatusTag];

            if (_status == null)
                status = null;
            else
                status = (LostStatus)_status;

            /*if (articleNo == null)
                nArticleNo = 0;
            else
                nArticleNo = (int)articleNo;*/

            if (injuryStatus == null)
                strInjuryStatus = "";
            else
                strInjuryStatus = (string)injuryStatus;
        }

        private bool IsProcessing()
        {
            object processing = Session[ProcessingTag];

            if (processing == null)
                return false;

            return (bool)processing;
        }

        private void SetProcessing(bool processing)
        {
            Session[ProcessingTag] = processing;
        }

        private LostStatus.State ReadNewArticles(ref LostStatus status, out string strCurrentStatus)
        {
            WebDBManager dbMgr = Network.NetworkWebManager.Instance.DBMgr;
            return LostStatus.ReadNewData(dbMgr, ref status, out strCurrentStatus);
        }
    }
}