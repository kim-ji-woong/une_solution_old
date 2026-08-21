using Common.Model.History;
using System;
using System.Collections.Generic;

namespace Common.BLL
{
    public class LoadManager
    {
        private ProcessManager m_processManager = null;

        public LoadManager(ProcessManager processManager)
        {
            this.m_processManager = processManager;
        }
    }
}
