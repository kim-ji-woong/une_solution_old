using Common.Model.History;
using SDMS.Model.Alarm;
using SOPSimulator.BLL.Models.Request;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SOPSimulator.BLL
{
    public class CreateManager
    {
        private ProcessManager m_processManager = null;

        public CreateManager(ProcessManager processManager)
        {
            this.m_processManager = processManager;
        }

        public int ExcuteSOP(RequestExcuteSOP data)
        {
            DateTime beginTime = Convert.ToDateTime(data.BeginTime);

            ActionStepHistory history = m_processManager.CommonDataManager.GetCreateManager().CreateActionStepHistory(
                data.ActionStepID, beginTime, data.RealMode == 1, null, null, null, DateTime.Now, data.Position, data.LastAccessedUserID,
                null, null, data.SensorZoneHistoryID, null);

            if (history == null)
                return -1;

            // 센서 신호로 실행된 SOP라면 SensorZoneHistoryID가 있다.
            // 해당 SOP가 실행중임을 DB에 기록한다
            if (data.SensorZoneHistoryID != null && data.SensorZoneHistoryID > 0) 
            {
                string strErrorMessage = null;

                Dictionary<CurrentAlarm.Fields, object> dicSets = new Dictionary<CurrentAlarm.Fields, object>();
                dicSets.Add(CurrentAlarm.Fields.SopStatus, 1); // SOP 실행 상태 (-1: SOP 시작 하기전, 0: SOP 실행 요청, 1: SOP 실행중, 2: SOP종료)

                Dictionary<CurrentAlarm.Fields, object> dicConditions = new Dictionary<CurrentAlarm.Fields, object>();
                dicConditions.Add(CurrentAlarm.Fields.SensorZoneHistoryID, data.SensorZoneHistoryID);
                bool updateSopStatus =
                    m_processManager.SdmsManager.GetUpdateManager().UpdateCurrentAlarm(dicSets, dicConditions, "", out strErrorMessage); 
            }

            return history.ID;
        }

        public ComponentHistory ProgressSOP(RequestProgressSOP data)
        {
            try
            {                
                // PROCESS(0), DECISION(1), ANNOTATION(2), ENDPOINT(3), LINK(4), TRANSSOP(5), INTERNAL(6), EXTERNAL(7), NONE(8)
                if (data.ComponentType == 0)
                {
                    return ProgressProcess(data);
                }
                else if (data.ComponentType == 1)
                {
                    return ProgressDecision(data);
                }
                else if (data.ComponentType == 3)
                {
                    return ProgressEndPoint(data);
                }
                else if (data.ComponentType == 6)
                {
                    return ProgressInternal(data);
                }

                return null;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return null;
            }
        }

        private string GetStringStatus(int nStatus)
        {
            //NORMAL = 1, RUN = 2, DONE = 3, INPUT = 4, SKIP = 5
            if (nStatus == 1)
                return "대기";
            else if (nStatus == 2)
                return "실행중";
            else if (nStatus == 3)
                return "완료";
            return "";
        }

        public ComponentHistory ProgressEndPoint(RequestProgressSOP data)
        {
            string strDescription = string.Format("EndPoint_{0}_{1}", data.Text, GetStringStatus(data.Status)); 
            
            string strTask = null;
            ComponentHistory history = m_processManager.CommonDataManager.GetCreateManager().CreateComponentHistory(
                data.ActionStepHistoryID, data.ComponentID, data.ComponentType, DateTime.Now, data.Status, strTask, null, null
                , data.AccessedUserID, null, null, null, null, strDescription);

            return history;
        }

        public ComponentHistory ProgressProcess(RequestProgressSOP data)
        {
            string strDescription = string.Format("Process_{0}_{1}", data.Text, GetStringStatus(data.Status));
            string strTask = null;
            ComponentHistory history = m_processManager.CommonDataManager.GetCreateManager().CreateComponentHistory(
                data.ActionStepHistoryID, data.ComponentID, data.ComponentType, DateTime.Now, data.Status, strTask, null, null
                , data.AccessedUserID, null, null, null, null, strDescription);

            return history;
        }

        public ComponentHistory ProgressDecision(RequestProgressSOP data)
        {
            string strDescription = string.Format("Decision_{0}_{1}", data.Text, GetStringStatus(data.Status));
            string strTask = null;
            ComponentHistory history = m_processManager.CommonDataManager.GetCreateManager().CreateComponentHistory(
                data.ActionStepHistoryID, data.ComponentID, data.ComponentType, DateTime.Now, data.Status, strTask, null, null
                , data.AccessedUserID, null, null, null, null, strDescription);

            return history;
        }

        public ComponentHistory ProgressInternal(RequestProgressSOP data)
        {
            string strDescription = string.Format("Internal_{0}_{1}", data.Text, GetStringStatus(data.Status));
            string strTask = null;
            ComponentHistory history = m_processManager.CommonDataManager.GetCreateManager().CreateComponentHistory(
                data.ActionStepHistoryID, data.ComponentID, data.ComponentType, DateTime.Now, data.Status, strTask, null, null
                , data.AccessedUserID, null, null, null, null, strDescription);

            return history;
        }

        // ComponentHistory가 DB에 입력되었지만 아직 ComponentHistoryDetail이 생성되지 않은 상태임을 표시한다.
        // SOPWebServer에서 ComponentHistoryDetail이 생성되기 전에 ComponentHistory만 읽어가지 않도록 표시를 해둔다.
        private int SetNoDetailStatus(int nStatus)
        {
            return nStatus | 0x100;
        }

        // ComponentHistoryDetail이 생성되었거나 처리가 끝난 ComponentHistory 상태로 만든다.
        private int SetDetailStatus(int nStatus)
        {
            if ((nStatus & 0x100) == 0x100)
                return nStatus - 0x100;

            return nStatus;
        }
    }
}
