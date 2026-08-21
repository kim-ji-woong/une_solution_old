using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PersonalSOP.Controllers
{
    using Models;
    using History;

    public class SOPHistoryController : Controller
    {
        private const string ProcessingTag = "StatusProcessing";
        private const string ActionStepHistoryIDTag = "ActionStepHistoryID";
        private const string ComponentHistoryIDTag = "ComponentHistoryID";

        // GET: SOPHistory
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Index2()
        {
            if (IsProcessing())
                return new HttpStatusCodeResult(204);

            SetProcessing(true);

            int nCurrentActionStepHistoryID;
            int nLastComponentHistoryID;
            GetHistory(out nCurrentActionStepHistoryID, out nLastComponentHistoryID);

            string strElapsedTime = "";
            SOPHistory history = ReadNewHistories(ref nCurrentActionStepHistoryID, ref nLastComponentHistoryID, ref strElapsedTime);
            SetHistory(nCurrentActionStepHistoryID, nLastComponentHistoryID);

            SetProcessing(false);

            if (history == null)
            {
                this.ControllerContext.HttpContext.Response.Write("[ET]" + strElapsedTime);

                HttpStatusCodeResult result = new HttpStatusCodeResult(System.Net.HttpStatusCode.PartialContent);
                result.ExecuteResult(this.ControllerContext);
                return result;
                //return new HttpStatusCodeResult(204);
            }
            else
                SortHistory(history);

            return View(history);
        }

        private void SortHistory(SOPHistory history)
        {
            List<SOPHistoryData> datas = history.HistoryDatas;
            datas.Sort();

            int nDataCount = datas.Count;

            for (int i=0;i<nDataCount;i++)
            {
                SOPHistoryData data = datas[i];
                data.No = i + 1;
            }

            history.SortedHistoryDatas.Clear();
            history.SortedHistoryDatas.AddRange(datas);
        }

        private void SetHistory(int actionStepHistoryID, int componentHistoryID)
        {
            Session[ActionStepHistoryIDTag] = actionStepHistoryID;
            Session[ComponentHistoryIDTag] = componentHistoryID;
        }

        private void GetHistory(out int actionStepHistoryID, out int componentHistoryID)
        {
            object _actionStepHistoryID = Session[ActionStepHistoryIDTag];
            object _componentHistoryID = Session[ComponentHistoryIDTag];

            if (_actionStepHistoryID == null)
                actionStepHistoryID = -1;
            else
                actionStepHistoryID = (int)_actionStepHistoryID;

            if (_componentHistoryID == null)
                componentHistoryID = -1;
            else
                componentHistoryID = (int)_componentHistoryID;
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

        private SOPHistory ReadNewHistories(ref int nCurrentActionStepHistoryID, ref int nLastComponentHistoryID, ref string strElapsedTime)
        {
            SOPHistory history = SOPHistoryManager.Instance.SOPHistory.Clone();
            strElapsedTime = history.ElapsedTime;

            if (history.ActionStepHistory == null)
            {
                nCurrentActionStepHistoryID = -1;
                nLastComponentHistoryID = -1;
                return null;
            }
            else if (history.ActionStepHistory.ActionStepHistoryID != nCurrentActionStepHistoryID)
            {
                nCurrentActionStepHistoryID = history.ActionStepHistory.ActionStepHistoryID;
                nLastComponentHistoryID = -1;

                List<SOPHistoryData> datas = history.HistoryDatas;

                if (datas.Count > 0)
                {
                    ComponentHistory componentHistory = datas.Last().ComponentHistory;

                    if (componentHistory != null)
                        nLastComponentHistoryID = componentHistory.ComponentHistoryID;
                }

                return history;
            }
            else
            {
                int nLastID = -1;

                List<SOPHistoryData> datas = history.HistoryDatas;

                if (datas.Count > 0)
                {
                    ComponentHistory componentHistory = datas.Last().ComponentHistory;

                    if (componentHistory != null)
                        nLastID = componentHistory.ComponentHistoryID;
                }

                if (nLastComponentHistoryID != nLastID)
                {
                    nLastComponentHistoryID = nLastID;
                    return history;
                }
            }

            return null;
        }
    }
}