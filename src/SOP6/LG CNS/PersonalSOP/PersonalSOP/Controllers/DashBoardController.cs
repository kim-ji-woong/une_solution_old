using DBUtility2;
using PersonalSOP.Common;
using PersonalSOP.History;
using PersonalSOP.Models;
using PersonalSOP.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PersonalSOP.Controllers
{
    public class DashBoardController : Controller
    {
        private const string ProcessingTag = "StatusProcessing";
        private const string ActionStepHistoryIDTag = "ActionStepHistoryID";
        private const string ComponentHistoryIDTag = "ComponentHistoryID";

        private const string CurrentStatusTag = "CurrentStatus";
        private const string CurrentArticleNoTag = "CurrentArticleNo";
        private const string InjuryStatusTag = "InjuryStatus";

        // GET: DashBoard
        public ActionResult Index()
        {
            // 초기화
            Session[ActionStepHistoryIDTag] = -1;
            Session[ComponentHistoryIDTag] = -1;
            Session[ParameterManager.LastIndex] = -1;
            return View();
        }

        private SOPHistory CompareSOPHistory()
        {
            SOPHistory sessionHistory = (SOPHistory)Session[_SOPHistory];
            SOPHistory managerHistory = SOPHistoryManager.Instance.SOPHistory;

            if (sessionHistory == null && managerHistory == null)
                return null;

            if (sessionHistory == null)
            {
                sessionHistory = new SOPHistory();
                
                foreach (SOPHistoryData data in managerHistory.HistoryDatas)
                {
                    sessionHistory.AddHistoryData(data);
                }

                Session[_SOPHistory] = sessionHistory;
                return sessionHistory;
            }
            else
            {
                if (managerHistory == null || managerHistory.HistoryDatas.Count == 0)
                {
                    sessionHistory = new SOPHistory();
                    Session[_SOPHistory] = sessionHistory;
                    return sessionHistory;
                }
                else
                {
                    if (sessionHistory.HistoryDatas.Count == managerHistory.HistoryDatas.Count)
                        return null;

                    sessionHistory = new SOPHistory();

                    foreach (SOPHistoryData data in managerHistory.HistoryDatas)
                    {
                        sessionHistory.AddHistoryData(data);
                    }

                    Session[_SOPHistory] = sessionHistory;
                }
            }

            return sessionHistory;
        }

        public ActionResult Index2()
        {
            bool isRefresh = false;
            if (Session[ActionStepHistoryIDTag] != null && Convert.ToInt32(Session[ActionStepHistoryIDTag]) == -1)
                isRefresh = true;

            string returnTag = "";
            bool return204 = false;
            bool return206 = false;

            // TK
            if (IsProcessing())
                return204 = true;

            SetProcessing(true);

            int nCurrentActionStepHistoryID;
            int nLastComponentHistoryID;
            GetHistory(out nCurrentActionStepHistoryID, out nLastComponentHistoryID);

            string strElapsedTime = "";
            Dashboard dashboard = new Dashboard();
            /*dashboard.SOPHistory = */ReadNewHistories(ref nCurrentActionStepHistoryID, ref nLastComponentHistoryID, ref strElapsedTime);
            dashboard.SOPHistory = CompareSOPHistory();
            SetHistory(nCurrentActionStepHistoryID, nLastComponentHistoryID);

            SetProcessing(false);
            
            if (dashboard.SOPHistory == null)
            {
                returnTag = "[ET]" + strElapsedTime;
                return206 = true;
            }
            else
                SortHistory(dashboard.SOPHistory);

            if (dashboard.SOPHistory != null)
                Session[_SOPHistory] = dashboard.SOPHistory;
            
            // 종합상황
            bool bUpdateLostStatus = true;
            if (IsProcessing())
                return204 = true;

            SetProcessing(true);

            int nBeginHistoryMessageID = -1;

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
                    {
                        return204 = true;
                        bUpdateLostStatus = false;
                    }
                    else
                    {
                        returnTag += "[CS]" + strCurrentInjuryStatus;
                        return206 = true;
                        bUpdateLostStatus = false;
                        //this.ControllerContext.HttpContext.Response.Write("[CS]" + strCurrentInjuryStatus);

                        //HttpStatusCodeResult result = new HttpStatusCodeResult(System.Net.HttpStatusCode.PartialContent);
                        //result.ExecuteResult(this.ControllerContext);
                        //return result;
                    }
                }
                else if (state == LostStatus.State.Initialized)
                {
                    status.Articles.Clear();
                }
                
                dashboard.LostStatus = status;
                if (status != null)
                {
                    Session[_LostStatus] = status;
                    nBeginHistoryMessageID = status.BeginHistoryMessageID;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                SetProcessing(false);
            }

            
            // 현장상황
            bool bUpdateSopBulletin = true;
            int actionStepHistoryID = nCurrentActionStepHistoryID;
            //object id = Session[ParameterManager.ActionStepHistoryID];

            //if (id != null && id is int)
            //    actionStepHistoryID = (int)id;

            int lastIndex = -1;
            if (Session[ParameterManager.LastIndex] != null)
            {
                lastIndex = int.Parse(Session[ParameterManager.LastIndex].ToString());
            }

            int tempIndex = -1;
            if (SOPBulletinController.nCurrentIndex.ContainsKey(actionStepHistoryID))
                tempIndex = SOPBulletinController.nCurrentIndex[actionStepHistoryID];

            if (actionStepHistoryID <= 0)
                bUpdateSopBulletin = false;

            if (lastIndex > 0 && tempIndex <= lastIndex)
                bUpdateSopBulletin = false;

            if (tempIndex == -1 && lastIndex == -1)
                bUpdateSopBulletin = false;

            List<Models.BulletinMessage> sopBulletins = null;

            //int lastActionID = NetworkWebManager.Instance.GetMaxTableID("ActionStepHistory");

            StringBuilder sb = new StringBuilder();
            sb.Append("Select ID, TimeStamp, Title, Message, Image ");
            sb.Append("  From ActionStepHistoryMessage ");
            //sb.AppendFormat("Where ActionStepHistoryID = {0} ", actionStepHistoryID);
            //sb.AppendFormat("  And ID >= {0}", nBeginHistoryMessageID);

            ArrayList arrResult = NetworkWebManager.Instance.DBMgr.GetResultData(sb.ToString());
            if (arrResult != null && arrResult.Count > 0)
            {
                int arrResultCount = arrResult.Count;
                for (int i = 0; i < arrResultCount; i += 5)
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
                        System.Net.Http.HttpRequestMessage msg = HttpContext.Items["MS_HttpRequestMessage"] as System.Net.Http.HttpRequestMessage;
                        sop.ImagePath = "http://" + msg.Headers.Host + strImagePath;
                    }

                    Session[ParameterManager.LastIndex] = nID;

                    if (sopBulletins == null)
                        sopBulletins = new List<BulletinMessage>();

                    //if (nBeginHistoryMessageID <= nID)
                        sopBulletins.Add(sop);
                }
            }
            dashboard.BulletinMessages = sopBulletins;

            if (sopBulletins != null)
                sopBulletins.Reverse();
            
            if (sopBulletins != null)
            {
                object obj = Session[_BulletinMessages];

                if (obj != null)
                {
                    List<Models.BulletinMessage> orgDatas = obj as List<Models.BulletinMessage>;
                    if (orgDatas.Count != sopBulletins.Count)
                    {
                        bUpdateSopBulletin = true;
                        Session[_BulletinMessages] = sopBulletins;
                    }
                }
                else
                {
                    bUpdateSopBulletin = true;
                    Session[_BulletinMessages] = sopBulletins;
                }
            }

            // 새로운 알람이 들어와서 이전 상황기록이 모두 지워져야할때
            if (Session[_BulletinMessages] != null && sopBulletins == null)
            {
                bUpdateSopBulletin = true;
                Session[_BulletinMessages] = null;
            }

            #region 한 개의 뷰라도 업데이트된 내역이 있다면 뷰를 다 채워야 한다
            int resultCount = 0;
            if (dashboard.SOPHistory != null)
                resultCount++;
            if (bUpdateSopBulletin)
                resultCount++;
            if (bUpdateLostStatus)
                resultCount++;

            if (isRefresh)
                resultCount++;

            if (resultCount > 0)
            {
                if (dashboard.SOPHistory == null)
                {
                    if (Session[_SOPHistory] != null)
                        dashboard.SOPHistory = Session[_SOPHistory] as SOPHistory;
                    else
                        dashboard.SOPHistory = new SOPHistory();
                }
                if (dashboard.BulletinMessages == null)
                {
                    if (Session[_BulletinMessages] != null)
                        dashboard.BulletinMessages = Session[_BulletinMessages] as List<BulletinMessage>;
                    else
                        dashboard.BulletinMessages = new List<BulletinMessage>();
                }
                if (dashboard.LostStatus == null)
                {
                    if (Session[_LostStatus] != null)
                        dashboard.LostStatus = Session[_LostStatus] as LostStatus;
                    else
                        dashboard.LostStatus = new LostStatus();
                }
            }
            #endregion

            if (resultCount == 0)
            {
                if (return206)
                {
                    if (returnTag.Length > 0)
                        this.ControllerContext.HttpContext.Response.Write(returnTag);

                    HttpStatusCodeResult result = new HttpStatusCodeResult(System.Net.HttpStatusCode.PartialContent);
                    result.ExecuteResult(this.ControllerContext);
                    return result;
                }

                if (return204 && returnTag.Length == 0)
                    return new HttpStatusCodeResult(204);
            }

            SetSOPInfo(dashboard);
            return View(dashboard);
        }

        private void SetSOPInfo(Dashboard dashboard)
        {
            if (dashboard.SOPHistory.SOPInfo.Length == 0)
            {
                foreach (SOPHistoryData data in dashboard.SOPHistory.HistoryDatas)
                {
                    if (data.ComponentHistory.ActionStepHistory != null)
                    {
                        dashboard.SOPHistory.SOPInfo = data.ComponentHistory.ActionStepHistory.Position;
                        dashboard.SOPHistory.BeginTime = data.ComponentHistory.ActionStepHistory.BeginTime.m_time;
                    }

                    break;
                }
            }
        }

        private const string _SOPHistory = "_SOPHistory";
        private const string _BulletinMessages = "_BulletinMessages";
        private const string _LostStatus = "_LostStatus";

        private void SortHistory(SOPHistory history)
        {
            List<SOPHistoryData> datas = history.HistoryDatas;
            //datas = datas.OrderByDescending(p => p.No).ToList();

            //List<SOPHistoryData> datas2 = history.HistoryDatas;
            datas.Sort();

            int nDataCount = datas.Count;

            for (int i = 0; i < nDataCount; i++)
            {
                SOPHistoryData data = datas[i];
                data.No = i + 1;
            }
            //for (int i = nDataCount; i == 1; i--)
            //{
            //    SOPHistoryData data = datas[i];
            //    data.No = i + 1;
            //}

            history.SortedHistoryDatas.Clear();
            //datas.Reverse();
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

        private LostStatus.State ReadNewArticles(ref LostStatus status, out string strCurrentStatus)
        {
            WebDBManager dbMgr = Network.NetworkWebManager.Instance.DBMgr;
            return LostStatus.ReadNewData(dbMgr, ref status, out strCurrentStatus);
        }
    }

}