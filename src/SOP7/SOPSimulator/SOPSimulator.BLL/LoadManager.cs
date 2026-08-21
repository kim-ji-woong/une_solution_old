using Common.Model.History;
using SDMS.Model.History;
using SOPManager.Model.Sop.Category;
using SOPSimulator.BLL.Models.Request;
using SOPSimulator.BLL.Models.Response;
using System.Collections;
using System.Collections.Generic;

namespace SOPSimulator.BLL
{
    public class LoadManager
    {
        private ProcessManager m_processManager = null;

        public LoadManager(ProcessManager processManager)
        {
            this.m_processManager = processManager;            
        }

        public ResponseMonitor MonitorComponentHistory()
        {
            string strErrorMessage = null;

            string strCondition = "EndTime is null";
            List<ActionStepHistory> actionStepHistories = 
                m_processManager.CommonDataManager.GetSelectManager().SelectActionStepHistories(strCondition, out strErrorMessage);

            ResponseMonitor res = new ResponseMonitor();
            List<HistoryData> historyDatas = new List<HistoryData>();

            foreach (ActionStepHistory actionStepHistory in actionStepHistories)
            {
                strCondition = string.Format("ActionStepHistoryID={0}", actionStepHistory.ID);
                List<ComponentHistory> componentHistory =
                    m_processManager.CommonDataManager.GetSelectManager().SelectComponentHistories(strCondition, out strErrorMessage);

                strCondition = string.Format(
                    "ComponentHistoryID in (select id from SopHistoryComponent where ActionStepHistoryID={0})", actionStepHistory.ID);
                List<ComponentHistoryDetail> details =
                    m_processManager.CommonDataManager.GetSelectManager().SelectComponentHistoryDetails(strCondition, out strErrorMessage);

                ActionStep actionStep =
                    m_processManager.SopDataManager.GetSelectManager().SelectActionStep(actionStepHistory.ActionStepID, out strErrorMessage);

                Disaster disaster =
                    m_processManager.SopDataManager.GetSelectManager().SelectDisaster(actionStep.DisasterID, out strErrorMessage);

                HistoryData data = new HistoryData();
                data.ActionStepHistory = actionStepHistory;
                data.ComponentHistories = componentHistory;
                data.ComponentHistoryDetails = details;
                data.ActionStep = actionStep;
                data.Disaster = disaster;
                data.VersionID = disaster.VersionID;

                historyDatas.Add(data);
            }
            res.HistoryData = historyDatas;

            return res;
        }

        public ResponseSensorName RequestSensorName(int sensorZoneHistoryID)
        {
            ResponseSensorName res = new ResponseSensorName();
            string strErrorMessage = null;

            string strSensorName = "";

            SensorZoneHistory szh = m_processManager.SdmsManager.GetSelectManager().SelectSensorZoneHistory(sensorZoneHistoryID, out strErrorMessage);
            if (szh == null)
                return res;

            if (szh.AllSensorZoneIDs.Count == 0)
                return res;

            string strCondition = string.Format("ID in ({0})", string.Join(", ", szh.AllSensorZoneIDs));

            ArrayList arrResult = m_processManager.SdmsManager.GetSelectManager().JoinSensorZoneSensors(null, strCondition, out strErrorMessage);
            if (arrResult == null)
                return res;

            int resultCount = arrResult.Count;
            for (int i = 0; i < resultCount; i += 3)
            {
                if (arrResult[i] is int && arrResult[i + 1] is int && arrResult[i + 2] is string)
                {
                    int nSensorZoneID = (int)arrResult[i];
                    int nSensorType = (int)arrResult[i + 1];
                    string strSensorName2 = arrResult[i + 2].ToString();

                    if (strSensorName.Length > 0)
                        strSensorName += ", " + strSensorName2;
                    else
                        strSensorName = strSensorName2;
                }
                else if (arrResult[i] is int && arrResult[i + 1] is int && arrResult[i + 2] is null) // 수동신고
                {
                    if (strSensorName.Length > 0)
                        strSensorName += ", 수동신고";
                    else
                        strSensorName = "수동 신고";
                }
            }

            res.SensorName = strSensorName;

            return res;
        }
    }
}
