using History.BLL.Models.Request;
using SDMS.Model.History;
using System;
using System.Collections.Generic;
using System.Text;

namespace History.BLL
{
    public class SaveManager
    {
        private ProcessManager m_processManager = null;

        public SaveManager(ProcessManager processManager)
        {
            this.m_processManager = processManager;
        }

        public bool UpdateAlarmMemo(RequestUpdateAlarmMemo req)
        {
            if (req.SensorZoneHistoryID == -1)
                return false;

            Dictionary<SensorZoneHistory.Fields, object> dicSets = new Dictionary<SensorZoneHistory.Fields, object>();
            dicSets.Add(SensorZoneHistory.Fields.Memo, req.Memo);

            Dictionary<SensorZoneHistory.Fields, object> dicConditions = new Dictionary<SensorZoneHistory.Fields, object>();
            dicConditions.Add(SensorZoneHistory.Fields.ID, req.SensorZoneHistoryID);

            string strErrorMessage = null;
            if (!m_processManager.SdmsDataManager.GetUpdateManager().UpdateSensorZoneHistory(dicSets, dicConditions, null, out strErrorMessage))
            {
                System.Diagnostics.Trace.WriteLine("[ERROR] UpdateAlarmMemo : " + strErrorMessage);
                return false;
            }

            return true;
        }
    }
}
