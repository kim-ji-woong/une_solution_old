using Common.Model.History;
using SDMS.Model.Alarm;
using SOPSimulator.BLL.Models.Request;
using System;
using System.Collections.Generic;

namespace SOPSimulator.BLL
{
    public class UpdateManager
    {
        private ProcessManager m_processManager = null;

        public UpdateManager(ProcessManager processManager)
        {
            this.m_processManager = processManager;
        }
    }
}
