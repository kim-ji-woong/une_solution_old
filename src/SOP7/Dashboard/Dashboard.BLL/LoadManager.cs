using Dashboard.BLL.Models.Response;
using Dashboard.IDAL;
using Dashboard.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dashboard.BLL
{
    public class LoadManager
    {
        private IDataManager m_dataManager = null;
        private ProcessManager m_processManager = null;

        public LoadManager(IDataManager dataManager, ProcessManager processManager)
        {
            this.m_dataManager = dataManager;
            this.m_processManager = processManager;
        }

        public ResponseCurrentWorkPermit GetCurrentWorkPermits()
        {
            ResponseCurrentWorkPermit result = new ResponseCurrentWorkPermit();

            Dictionary<CurrentWorkPermit.Fields, object> dicConditions = new Dictionary<CurrentWorkPermit.Fields, object>();
            string strAdditionalConditions = null;
            string strErrorMessage = null;

            List<CurrentWorkPermit> currentWorkPermits = m_dataManager.GetSelectManager().SelectCurrentWorkPermits(dicConditions, strAdditionalConditions, out strErrorMessage);
            
            if (currentWorkPermits == null)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }

            result.Success = true;
            result.CurrentWorkPermits = currentWorkPermits;
            return result;
        }
    }
}
