namespace Common.DAL
{
    using IDAL;
    using Model;
    using Model.History;
    using Model.Option;
    using System;
    using dnsDBUtil;
    using System.Collections;
    using System.Reflection;
    using System.Collections.Generic;
    using UnE.Geometry;

    public class CreateManager : QueryManager, ICreate
    {
        private string m_strErrorMessage = null;
        private DataManager m_dataManager = null;
        //private WebDBManager m_dbManager = null;

        private const int FindCountLimit = 100;

        public CreateManager(DataManager dataManager)
        {
            m_dataManager = dataManager;
            m_dbManager = m_dataManager.GetDBManager() as WebDBManager;
        }

        // Option
        public Options CreateOption(Options.OptionTarget eTargetName, string strPropertyName, string strPropertyValue, int nSiteID, string strDescription = null)
        {
            string tableName = string.Format("Option{0}", eTargetName.ToString());
            //string query = "";
            //ArrayList res = null;
            Options ret = null;

            if (eTargetName != Options.OptionTarget.NOT_DEFINED)
            {
                Dictionary<Options.Fields, object> dicFieldDatas = new Dictionary<Options.Fields, object>();
                dicFieldDatas[Options.Fields.PropertyName] = strPropertyName;
                dicFieldDatas[Options.Fields.PropertyValue] = strPropertyValue;
                dicFieldDatas[Options.Fields.SiteID] = nSiteID;
                dicFieldDatas[Options.Fields.Description] = strDescription;

                string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                tableName,
                GetFieldNames<Options.Fields>(),
                GetFieldValues(dicFieldDatas));

                ArrayList arrResult = m_dbManager.GetResultData(strSQL);

                if (arrResult != null)
                {
                    bool isNullable;
                    string strCondition = string.Format("order by {0} desc", Options.GetFieldName(Options.Fields.ID, out isNullable));

                    string strErrorMessage;
                    // 가장 마지막에 삽입된 객체를 얻어온다.
                    List<Options> options = m_dataManager.GetSelectManager().SelectOptions(eTargetName, strCondition, 1, out strErrorMessage);

                    if (options == null || options.Count == 0)
                    {
                        m_strErrorMessage = strErrorMessage;
                        return null;
                    }

                    if (IsSameOption(options[0], strPropertyName, strPropertyValue, nSiteID, strDescription))
                        return options[0];

                    return GetOption(eTargetName, tableName, strPropertyName, strPropertyValue, nSiteID, strDescription, options[0].ID, 2, FindCountLimit, out m_strErrorMessage);
                }
                else
                {
                    m_strErrorMessage = m_dbManager.LastErrorMessage;
                }
            }
            else
            {
                // Not Defined
                m_strErrorMessage = "TargetName Not Defined";
            }

            return ret;
        }

        private Options GetOption(Options.OptionTarget eTargetName, string tableName, string strPropertyName, string strPropertyValue, int nSiteID, string strDescription, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", Options.GetFieldName(Options.Fields.ID, out isNullable), id);

            List<Options> options = m_dataManager.GetSelectManager().SelectOptions(eTargetName, strCondition, nCount, out strErrorMessage);

            if (options == null)
                return null;

            foreach (Options option in options)
            {
                if (IsSameOption(option, strPropertyName, strPropertyValue, nSiteID, strDescription))
                    return option;

                if (option.ID < id)
                    id = option.ID;
            }

            if (nCount < nLimit)
                return GetOption(eTargetName, tableName, strPropertyName, strPropertyValue, nSiteID, strDescription, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(tableName);
            return null;
        }

        private bool IsSameOption(Options option, string strPropertyName, string strPropertyValue, int nSiteID, string strDescription)
        {
            if (option.PropertyName == strPropertyName &&
                option.PropertyValue == strPropertyValue &&
                option.SiteID == nSiteID &&
                option.Description == strDescription)
                return true;

            return false;
        }

        // History
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nActionStepID"></param>
        /// <param name="dtBegin"></param>
        /// <param name="realMode">실제상황인가?</param>
        /// <param name="dtEnd"></param>
        /// <param name="dtLastAccessed"></param>
        /// <param name="dtDetectEnd"></param>
        /// <param name="dtDetect"></param>
        /// <param name="strPosition"></param>
        /// <param name="nLastAccesseduserID"></param>
        /// <param name="nStartOption">SOP시작 옵션 : 0:None 1:SMS 2:Broadcast 4:Reserve1 8:Reserve2</param>
        /// <param name="strDisasterOption"></param>
        /// <param name="nSensorZoneHistoryID"></param>
        /// <param name="strDescription"></param>
        /// <returns></returns>
        public ActionStepHistory CreateActionStepHistory(int nActionStepID, DateTime dtBegin, bool? realMode = null, DateTime? dtEnd = null, DateTime? dtLastAccessed = null, DateTime? dtDetectEnd = null, DateTime? dtDetect = null, string strPosition = null, int? nLastAccesseduserID = null, int? nStartOption = null, string strDisasterOption = null, int? nSensorZoneHistoryID = null, string strDescription = null)
        {
            Dictionary<ActionStepHistory.Fields, object> dicFieldDatas = new Dictionary<ActionStepHistory.Fields, object>();
            dicFieldDatas[ActionStepHistory.Fields.ActionStepID] = nActionStepID; 
            dicFieldDatas[ActionStepHistory.Fields.RealMode] = realMode;
            dicFieldDatas[ActionStepHistory.Fields.BeginTime] = dtBegin;
            dicFieldDatas[ActionStepHistory.Fields.EndTime] = dtEnd;
            dicFieldDatas[ActionStepHistory.Fields.LastAccessedTime] = dtLastAccessed;
            dicFieldDatas[ActionStepHistory.Fields.DetectEndTime] = dtDetectEnd;
            dicFieldDatas[ActionStepHistory.Fields.DetectTime] = dtDetect;
            dicFieldDatas[ActionStepHistory.Fields.Position] = strPosition;
            dicFieldDatas[ActionStepHistory.Fields.LastAccessedUserID] = nLastAccesseduserID;
            dicFieldDatas[ActionStepHistory.Fields.StartOption] = nStartOption;
            dicFieldDatas[ActionStepHistory.Fields.DisasterOption] = strDisasterOption;
            dicFieldDatas[ActionStepHistory.Fields.SensorZoneHistoryID] = nSensorZoneHistoryID;
            dicFieldDatas[ActionStepHistory.Fields.Description] = strDescription;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                ActionStepHistory.TableName,
                GetFieldNames<ActionStepHistory.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", ActionStepHistory.GetFieldName(ActionStepHistory.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<ActionStepHistory> histories = m_dataManager.GetSelectManager().SelectActionStepHistories(null, strCondition, 1, out strErrorMessage);

                if (histories == null || histories.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameActionStepHistory(histories[0], nActionStepID, dtBegin, realMode, dtEnd, dtLastAccessed, dtDetectEnd, dtDetect, strPosition, nLastAccesseduserID, nStartOption, strDisasterOption,  nSensorZoneHistoryID, strDescription))
                    return histories[0];

                return GetActionStepHistory(nActionStepID, dtBegin, realMode, dtEnd, dtLastAccessed, dtDetectEnd, dtDetect, strPosition, nLastAccesseduserID, nStartOption, strDisasterOption, nSensorZoneHistoryID, strDescription, histories[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private ActionStepHistory GetActionStepHistory(int nActionStepID, DateTime dtBegin, bool? realMode, DateTime? dtEnd, DateTime? dtLastAccessed, DateTime? dtDetectEnd, DateTime? dtDetect, string strPosition, int? nLastAccesseduserID, int? nStartOption, string strDisasterOption, int? nSensorZoneHistoryID, string strDescription, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", ActionStepHistory.GetFieldName(ActionStepHistory.Fields.ID, out isNullable), id);

            List<ActionStepHistory> histories = m_dataManager.GetSelectManager().SelectActionStepHistories(null, strCondition, nCount, out strErrorMessage);

            if (histories == null)
                return null;

            foreach (ActionStepHistory history in histories)
            {
                if (IsSameActionStepHistory(history, nActionStepID, dtBegin, realMode, dtEnd, dtLastAccessed, dtDetectEnd, dtDetect, strPosition, nLastAccesseduserID, nStartOption, strDisasterOption, nSensorZoneHistoryID, strDescription))
                    return history;

                if (history.ID < id)
                    id = history.ID;
            }

            if (nCount < nLimit)
                return GetActionStepHistory(nActionStepID, dtBegin, realMode, dtEnd, dtLastAccessed, dtDetectEnd, dtDetect, strPosition, nLastAccesseduserID, nStartOption, strDisasterOption, nSensorZoneHistoryID, strDescription, id, nCount * 2, FindCountLimit, out m_strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(ActionStepHistory.TableName);
            return null;
        }

        private bool IsSameActionStepHistory(ActionStepHistory history, int nActionStepID, DateTime dtBegin, bool? realMode, DateTime? dtEnd, DateTime? dtLastAccessed, DateTime? dtDetectEnd, DateTime? dtDetect, string strPosition, int? nLastAccesseduserID, int? nStartOption, string strDisasterOption, int? nSensorZoneHistoryID, string strDescription)
        {
            if (history.ActionStepID == nActionStepID &&
                IsSameDateTime2(history.BeginTime, dtBegin) &&
                history.RealMode == realMode &&
                IsSameDateTime(history.EndTime, dtEnd) &&
                IsSameDateTime(history.LastAccessedTime, dtLastAccessed) &&
                IsSameDateTime(history.DetectEndTime, dtDetectEnd) &&
                IsSameDateTime(history.DetectTime, dtDetect) &&
                history.Position == strPosition &&
                history.LastAccessedUserID == nLastAccesseduserID &&
                history.StartOption == nStartOption &&
                history.DisasterOption == strDisasterOption &&
                history.SensorZoneHistoryID == nSensorZoneHistoryID &&
                history.Description == strDescription)
                return true;

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nActionStepHistoryID"></param>
        /// <param name="nComponentID"></param>
        /// <param name="nComponentType">PROCESS(0), DECISION(1), ANNOTATION(2), ENDPOINT(3), LINK(4), TRANSSOP(5), INTERNAL(6), EXTERNAL(7), NONE(8)</param>
        /// <param name="dtTime"></param>
        /// <param name="nStatus">
        /// 하위 2바이트(실행상태) : 대기상태(1), 실행중(2), 완료(3), 입력대기상태(4), 건너뜀 상태(5)
        /// 상위 2바이트(실행방향, bit flag 조합) : 위쪽(1), 오른쪽(2), 아래쪽(4), 왼쪽(8)
        /// </param>
        /// <param name="strTask"></param>
        /// <param name="nCompleteCount"></param>
        /// <param name="showBoard"></param>
        /// <param name="nAccessedUserID"></param>
        /// <param name="nCheckedNotify1"></param>
        /// <param name="nCheckedNotify2"></param>
        /// <param name="nCheckedRun"></param>
        /// <param name="nCheckedComplete"></param>
        /// <param name="strDescription"></param>
        /// <returns></returns>
        public ComponentHistory CreateComponentHistory(int nActionStepHistoryID, int nComponentID, int nComponentType, DateTime dtTime, int nStatus, string strTask = null, int? nCompleteCount = null, bool? showBoard = null, int? nAccessedUserID = null, int? nCheckedNotify1 = null, int? nCheckedNotify2 = null, int? nCheckedRun = null, int? nCheckedComplete = null, string strDescription = null)
        {
            Dictionary<ComponentHistory.Fields, object> dicFieldDatas = new Dictionary<ComponentHistory.Fields, object>();
            dicFieldDatas[ComponentHistory.Fields.ActionStepHistoryID] = nActionStepHistoryID;
            dicFieldDatas[ComponentHistory.Fields.ComponentID] = nComponentID;
            dicFieldDatas[ComponentHistory.Fields.ComponentType] = nComponentType;
            dicFieldDatas[ComponentHistory.Fields.Time] = dtTime;
            dicFieldDatas[ComponentHistory.Fields.Status] = nStatus;
            dicFieldDatas[ComponentHistory.Fields.Task] = strTask;
            dicFieldDatas[ComponentHistory.Fields.CompleteCount] = nCompleteCount;
            dicFieldDatas[ComponentHistory.Fields.ShowBoard] = showBoard;
            dicFieldDatas[ComponentHistory.Fields.AccessedUserID] = nAccessedUserID;
            dicFieldDatas[ComponentHistory.Fields.CheckedNotify1] = nCheckedNotify1;
            dicFieldDatas[ComponentHistory.Fields.CheckedNotify2] = nCheckedNotify2;
            dicFieldDatas[ComponentHistory.Fields.CheckedRun] = nCheckedRun;
            dicFieldDatas[ComponentHistory.Fields.CheckedComplete] = nCheckedComplete;
            dicFieldDatas[ComponentHistory.Fields.Description] = strDescription;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                ComponentHistory.TableName,
                GetFieldNames<ComponentHistory.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", ComponentHistory.GetFieldName(ComponentHistory.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<ComponentHistory> histories = m_dataManager.GetSelectManager().SelectComponentHistories(null, strCondition, 1, out strErrorMessage);

                if (histories == null || histories.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameComponentHistory(histories[0], nActionStepHistoryID, nComponentID, nComponentType, dtTime, nStatus, strTask, nCompleteCount, showBoard, nAccessedUserID, nCheckedNotify1, nCheckedNotify2, nCheckedRun, nCheckedComplete, strDescription))
                    return histories[0];

                return GetComponentHistory(nActionStepHistoryID, nComponentID, nComponentType, dtTime, nStatus, strTask, nCompleteCount, showBoard, nAccessedUserID, nCheckedNotify1, nCheckedNotify2, nCheckedRun, nCheckedComplete, strDescription, histories[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private ComponentHistory GetComponentHistory(int nActionStepHistoryID, int nComponentID, int nComponentType, DateTime dtTime, int nStatus, string strTask, int? nCompleteCount, bool? showBoard, int? nAccessedUserID, int? nCheckedNotify1, int? nCheckedNotify2, int? nCheckedRun, int? nCheckedComplete, string strDescription, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", ComponentHistory.GetFieldName(ComponentHistory.Fields.ID, out isNullable), id);

            List<ComponentHistory> histories = m_dataManager.GetSelectManager().SelectComponentHistories(null, strCondition, nCount, out strErrorMessage);

            if (histories == null)
                return null;

            foreach (ComponentHistory history in histories)
            {
                if (IsSameComponentHistory(history, nActionStepHistoryID, nComponentID, nComponentType, dtTime, nStatus, strTask, nCompleteCount, showBoard, nAccessedUserID, nCheckedNotify1, nCheckedNotify2, nCheckedRun, nCheckedComplete, strDescription))
                    return history;

                if (history.ID < id)
                    id = history.ID;
            }

            if (nCount < nLimit)
                return GetComponentHistory(nActionStepHistoryID, nComponentID, nComponentType, dtTime, nStatus, strTask, nCompleteCount, showBoard, nAccessedUserID, nCheckedNotify1, nCheckedNotify2, nCheckedRun, nCheckedComplete, strDescription, id, nCount * 2, FindCountLimit, out m_strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(ComponentHistory.TableName);
            return null;
        }

        private bool IsSameComponentHistory(ComponentHistory history, int nActionStepHistoryID, int nComponentID, int nComponentType, DateTime dtTime, int nStatus, string strTask, int? nCompleteCount, bool? showBoard, int? nAccessedUserID, int? nCheckedNotify1, int? nCheckedNotify2, int? nCheckedRun, int? nCheckedComplete, string strDescription)
        {
            if (history.ActionStepHistoryID == nActionStepHistoryID &&
                history.ComponentID == nComponentID &&
                history.ComponentType == nComponentType &&
                IsSameDateTime2(history.Time, dtTime) &&
                history.Status == nStatus &&
                history.Task == strTask &&
                history.CompleteCount == nCompleteCount &&
                history.ShowBoard == showBoard &&
                history.AccessedUserID == nAccessedUserID &&
                history.CheckedNotify1 == nCheckedNotify1 &&
                history.CheckedNotify2 == nCheckedNotify2 &&
                history.CheckedRun == nCheckedRun &&
                history.CheckedComplete == nCheckedComplete &&
                history.Description == strDescription)
                return true;

            return false;
        }

        public ComponentHistoryDetail CreateComponentHistoryDetail(int nComponentHistoryID, int nDataIndex, int? nData = null, float? fData = null, string strData = null, DateTime? dtTime = null)
        {
            Dictionary<ComponentHistoryDetail.Fields, object> dicFieldDatas = new Dictionary<ComponentHistoryDetail.Fields, object>();
            dicFieldDatas[ComponentHistoryDetail.Fields.ComponentHistoryID] = nComponentHistoryID;
            dicFieldDatas[ComponentHistoryDetail.Fields.DataIndex] = nDataIndex;
            dicFieldDatas[ComponentHistoryDetail.Fields.Datai] = nData;
            dicFieldDatas[ComponentHistoryDetail.Fields.Dataf] = fData;
            dicFieldDatas[ComponentHistoryDetail.Fields.Datas] = strData;
            dicFieldDatas[ComponentHistoryDetail.Fields.Time] = dtTime;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                ComponentHistoryDetail.TableName,
                GetFieldNames<ComponentHistoryDetail.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", ComponentHistoryDetail.GetFieldName(ComponentHistoryDetail.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<ComponentHistoryDetail> datas = m_dataManager.GetSelectManager().SelectComponentHistoryDetails(strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameComponentHistoryDetail(datas[0], nComponentHistoryID, nDataIndex, nData, fData, strData, dtTime))
                    return datas[0];

                return GetComponentHistoryDetail (nComponentHistoryID, nDataIndex, nData, fData, strData, dtTime, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private ComponentHistoryDetail GetComponentHistoryDetail(int nComponentHistoryID, int nDataIndex, int? nData, float? fData, string strData, DateTime? dtTime, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", ComponentHistoryDetail.GetFieldName(ComponentHistoryDetail.Fields.ID, out isNullable), id);

            List<ComponentHistoryDetail> datas = m_dataManager.GetSelectManager().SelectComponentHistoryDetails(strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (ComponentHistoryDetail data in datas)
            {
                if (IsSameComponentHistoryDetail(data, nComponentHistoryID, nDataIndex, nData, fData, strData, dtTime))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetComponentHistoryDetail(nComponentHistoryID, nDataIndex, nData, fData, strData, dtTime, id, nCount * 2, FindCountLimit, out m_strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(ComponentHistoryDetail.TableName);
            return null;
        }

        private bool IsSameComponentHistoryDetail(ComponentHistoryDetail detail, int nComponentHistoryID, int nDataIndex, int? nData, float? fData, string strData, DateTime? dtTime)
        {
            if (detail.ComponentHistoryID == nComponentHistoryID &&
                detail.DataIndex == nDataIndex &&
                detail.Datai == nData &&
                IsSameFloatData(detail.Dataf, fData) &&
                detail.Datas == strData &&
                IsSameDateTime(detail.Time, dtTime))
                return true;

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nActionStepHistoryID"></param>
        /// <param name="nActionStepID"></param>
        /// <param name="nUseCloseNoInput">입력이 없을때 SOP 자동 종료 사용여부</param>
        /// <param name="nUseCloseSensorReset">센서 리셋 신호시 SOP자동 종료 사용여부</param>
        /// <param name="nUseCloseSensorResetWaitTime">센서 신호시 몇분뒤 자동 종료 사용여부</param>
        /// <param name="nInputWaitTime">입력 대기시간 (초)</param>
        /// <param name="nSensorResetWaitTime">센서 리셋 후 대기 시간 (초)</param>
        /// <param name="dtBegin">SOP 시작 시간</param>
        /// <param name="nSensorZoneID"></param>
        /// <param name="nSensorZoneHistoryID"></param>
        /// <param name="strDescription"></param>
        /// <returns></returns>
        public ActionStepAutoClose CreateActionStepAutoClose(int nActionStepHistoryID, int? nActionStepID = null, int? nUseCloseNoInput = null, int? nUseCloseSensorReset = null, int? nUseCloseSensorResetWaitTime = null, int? nInputWaitTime = null, int? nSensorResetWaitTime = null, DateTime? dtBegin = null, int? nSensorZoneID = null, int? nSensorZoneHistoryID = null, string strDescription = null)
        {
            Dictionary<ActionStepAutoClose.Fields, object> dicFieldDatas = new Dictionary<ActionStepAutoClose.Fields, object>();
            dicFieldDatas[ActionStepAutoClose.Fields.ActionStepHistoryID] = nActionStepHistoryID;
            dicFieldDatas[ActionStepAutoClose.Fields.ActionStepID] = nActionStepID;
            dicFieldDatas[ActionStepAutoClose.Fields.UseCloseNoInput] = nUseCloseNoInput;
            dicFieldDatas[ActionStepAutoClose.Fields.UseCloseSensorReset] = nUseCloseSensorReset;
            dicFieldDatas[ActionStepAutoClose.Fields.UseCloseSensorResetWaitTime] = nUseCloseSensorResetWaitTime;
            dicFieldDatas[ActionStepAutoClose.Fields.InputWaitTime] = nInputWaitTime;
            dicFieldDatas[ActionStepAutoClose.Fields.SensorResetWaitTime] = nSensorResetWaitTime;
            dicFieldDatas[ActionStepAutoClose.Fields.BeginTime] = dtBegin;
            dicFieldDatas[ActionStepAutoClose.Fields.SensorZoneID] = nSensorZoneID;
            dicFieldDatas[ActionStepAutoClose.Fields.SensorZoneHistoryID] = nSensorZoneHistoryID;
            dicFieldDatas[ActionStepAutoClose.Fields.Description] = strDescription;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                ActionStepAutoClose.TableName,
                GetFieldNames<ActionStepAutoClose.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", ActionStepAutoClose.GetFieldName(ActionStepAutoClose.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<ActionStepAutoClose> datas = m_dataManager.GetSelectManager().SelectActionStepAutoCloses(strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameActionStepAutoClose(datas[0], nActionStepHistoryID, nActionStepID, nUseCloseNoInput, nUseCloseSensorReset, nUseCloseSensorResetWaitTime, nInputWaitTime, nSensorResetWaitTime, dtBegin, nSensorZoneID, nSensorZoneHistoryID, strDescription))
                    return datas[0];

                return GetActionStepAutoClose(nActionStepHistoryID, nActionStepID, nUseCloseNoInput, nUseCloseSensorReset, nUseCloseSensorResetWaitTime, nInputWaitTime, nSensorResetWaitTime, dtBegin, nSensorZoneID, nSensorZoneHistoryID, strDescription, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private ActionStepAutoClose GetActionStepAutoClose(int nActionStepHistoryID, int? nActionStepID, int? nUseCloseNoInput, int? nUseCloseSensorReset, int? nUseCloseSensorResetWaitTime, int? nInputWaitTime, int? nSensorResetWaitTime, DateTime? dtBegin, int? nSensorZoneID, int? nSensorZoneHistoryID, string strDescription, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", ActionStepAutoClose.GetFieldName(ActionStepAutoClose.Fields.ID, out isNullable), id);

            List<ActionStepAutoClose> datas = m_dataManager.GetSelectManager().SelectActionStepAutoCloses(strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (ActionStepAutoClose data in datas)
            {
                if (IsSameActionStepAutoClose(data, nActionStepHistoryID, nActionStepID, nUseCloseNoInput, nUseCloseSensorReset, nUseCloseSensorResetWaitTime, nInputWaitTime, nSensorResetWaitTime, dtBegin, nSensorZoneID, nSensorZoneHistoryID, strDescription))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetActionStepAutoClose(nActionStepHistoryID, nActionStepID, nUseCloseNoInput, nUseCloseSensorReset, nUseCloseSensorResetWaitTime, nInputWaitTime, nSensorResetWaitTime, dtBegin, nSensorZoneID, nSensorZoneHistoryID, strDescription, id, nCount * 2, FindCountLimit, out m_strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(ActionStepAutoClose.TableName);
            return null;
        }

        private bool IsSameActionStepAutoClose(ActionStepAutoClose data, int nActionStepHistoryID, int? nActionStepID, int? nUseCloseNoInput, int? nUseCloseSensorReset, int? nUseCloseSensorResetWaitTime, int? nInputWaitTime, int? nSensorResetWaitTime, DateTime? dtBegin, int? nSensorZoneID, int? nSensorZoneHistoryID, string strDescription)
        {
            if (data.ActionStepHistoryID == nActionStepHistoryID &&
                data.ActionStepID == nActionStepID &&
                data.UseCloseNoInput == nUseCloseNoInput &&
                data.UseCloseSensorReset == nUseCloseSensorReset &&
                data.UseCloseSensorResetWaitTime == nUseCloseSensorResetWaitTime &&
                data.InputWaitTime == nInputWaitTime &&
                data.SensorResetWaitTime == nSensorResetWaitTime &&
                IsSameDateTime(data.BeginTime, dtBegin) &&
                data.SensorZoneID == nSensorZoneID &&
                data.SensorZoneHistoryID == nSensorZoneHistoryID &&
                data.Description == strDescription)
                return true;

            return false;
        }

        public Shelter CreateShelter(string strShelterName, int nShelterType, int nShelterIDType, int? nShelterID, List<Polygon> boundary, int nSiteID, string strDescription)
        {
            Dictionary<Shelter.Fields, object> dicFieldDatas = new Dictionary<Shelter.Fields, object>();
            dicFieldDatas[Shelter.Fields.ShelterName] = strShelterName;
            dicFieldDatas[Shelter.Fields.ShelterType] = nShelterType;
            dicFieldDatas[Shelter.Fields.ShelterIDType] = nShelterIDType;
            dicFieldDatas[Shelter.Fields.ShelterID] = nShelterID;
            dicFieldDatas[Shelter.Fields.Boundary] = boundary == null ? null : ShelterBoundaryToString(boundary);
            dicFieldDatas[Shelter.Fields.SiteID] = nSiteID;
            dicFieldDatas[Shelter.Fields.Description] = strDescription;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Shelter.TableName,
                GetFieldNames<Shelter.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", Shelter.GetFieldName(Shelter.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Shelter> datas = m_dataManager.GetSelectManager().SelectShelters(strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameShelter(datas[0], strShelterName, nShelterType, nShelterIDType, nShelterID, boundary, nSiteID, strDescription))
                    return datas[0];

                return GetShelter(strShelterName, nShelterType, nShelterIDType, nShelterID, boundary, nSiteID, strDescription, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Shelter GetShelter(string strShelterName, int nShelterType, int nShelterIDType, int? nShelterID, List<Polygon> boundary, int nSiteID, string strDescription, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", Shelter.GetFieldName(Shelter.Fields.ID, out isNullable), id);

            List<Shelter> datas = m_dataManager.GetSelectManager().SelectShelters(strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Shelter data in datas)
            {
                if (IsSameShelter(data, strShelterName, nShelterType, nShelterIDType, nShelterID,  boundary, nSiteID, strDescription))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetShelter(strShelterName, nShelterType, nShelterIDType, nShelterID, boundary, nSiteID, strDescription, id, nCount * 2, FindCountLimit, out m_strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(Shelter.TableName);
            return null;
        }

        private bool IsSameShelter(Shelter data, string strShelterName, int nShelterType, int nShelterIDType, int? nShelterID, List<Polygon> boundary, int nSiteID, string strDescription)
        {
            if (data.ShelterName == strShelterName &&
                data.ShelterType == nShelterType &&
                data.ShelterIDType == nShelterIDType &&
                data.ShelterID == nShelterID &&
                data.Boundary == boundary &&
                data.SiteID == nSiteID &&
                data.Description == strDescription)
                return true;

            return false;
        }

        public Site CreateSite(string strSiteName, int nTeamID)
        {
            Dictionary<Site.Fields, object> dicFieldDatas = new Dictionary<Site.Fields, object>();
            dicFieldDatas[Site.Fields.SiteName] = strSiteName;
            dicFieldDatas[Site.Fields.TeamID] = nTeamID;
            
            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Site.TableName,
                GetFieldNames<Site.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", Site.GetFieldName(Site.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Site> datas = m_dataManager.GetSelectManager().SelectSites(strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameSite(datas[0], strSiteName, nTeamID))
                    return datas[0];

                return GetSite(strSiteName, nTeamID, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Site GetSite(string strSiteName, int nTeamID, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", Site.GetFieldName(Site.Fields.ID, out isNullable), id);

            List<Site> datas = m_dataManager.GetSelectManager().SelectSites(strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Site data in datas)
            {
                if (IsSameSite(data, strSiteName, nTeamID))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetSite(strSiteName, nTeamID, id, nCount * 2, FindCountLimit, out m_strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(Site.TableName);
            return null;
        }

        private bool IsSameSite(Site data, string strSiteName, int nTeamID)
        {
            if (data.SiteName == strSiteName &&
                data.TeamID == nTeamID)
                return true;

            return false;
        }

        public string GetErrorMessage()
        {
            return m_strErrorMessage;
        }

        public bool CreateUserHistory(int nUserID, int nTargetType, int nActionType, string strHistoryContent)
        {
            Dictionary<UserHistory.Fields, object> dicFieldDatas = new Dictionary<UserHistory.Fields, object>();
            dicFieldDatas[UserHistory.Fields.Time] = DateTime.Now;
            dicFieldDatas[UserHistory.Fields.UserID] = nUserID;
            dicFieldDatas[UserHistory.Fields.TargetType] = nTargetType;
            dicFieldDatas[UserHistory.Fields.ActionType] = nActionType;
            dicFieldDatas[UserHistory.Fields.HistoryContent] = strHistoryContent;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                UserHistory.TableName,
                GetFieldNames<UserHistory.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        private string GetInsertErrorMessage(string tableName)
        {
            return string.Format("{0} 테이블의 데이터 삽입에 실패하였습니다.", tableName);
        }

        private bool IsSameFloatData(float? data1, float? data2)
        {
            if (data1 == null && data2 == null)
                return true;

            if (data1 != null && data2 != null)
            {
                if (System.Math.Abs((float)data1 - (float)data2) < UnE.Geometry.Math.HALF_TOLERANCE())
                    return true;
            }

            return false;
        }

        private bool IsSameDateTime(DateTime? time1, DateTime? time2)
        {
            if (time1 == null && time2 == null)
                return true;

            if (time1 != null && time2 != null)
            {
                if (((DateTime)time1).ToString("yyyyMMddHHmmss") == ((DateTime)time2).ToString("yyyyMMddHHmmss"))
                    return true;
            }

            return false;
        }

        private bool IsSameDateTime2(DateTime time1, DateTime time2)
        {
            if (time1.ToString("yyyyMMddHHmmss") == time2.ToString("yyyyMMddHHmmss"))
                return true;

            return false;
        }
    }
}
