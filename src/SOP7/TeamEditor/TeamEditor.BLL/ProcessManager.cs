using System;
using System.Collections.Generic;
using System.Text;

namespace TeamEditor.BLL
{
    public class ProcessManager
    {
        private LoadManager m_loadManager = null;
        private SaveManager m_saveManager = null;
        
        private TeamEditor.IDAL.IDataManager m_teamDataManager = null;
        private Common.IDAL.IDataManager m_commonDataManager = null;
        private SOPManager.IDAL.IDataManager m_sopDataManager = null;
        private SDMS.IDAL.IDataManager m_sdmsDataManager = null;

        public TeamEditor.IDAL.IDataManager TeamDataManager
        {
            get { return m_teamDataManager; }
        }

        public Common.IDAL.IDataManager CommonDataManager
        {
            get { return m_commonDataManager; }
        }

        public ProcessManager(Common.IDAL.IDataManager commonDataManager, TeamEditor.IDAL.IDataManager teamDataManager, SOPManager.IDAL.IDataManager sopDataManager, SDMS.IDAL.IDataManager sdmsDataManager)
        {
            m_commonDataManager = commonDataManager;
            m_teamDataManager = teamDataManager;
            m_sopDataManager = sopDataManager;
            m_sdmsDataManager = sdmsDataManager;

            m_loadManager = new LoadManager(m_teamDataManager);
            m_saveManager = new SaveManager(m_teamDataManager, m_sopDataManager, m_sdmsDataManager);
        }

        public LoadManager GetLoadManager()
        {
            return m_loadManager;
        }

        public SaveManager GetSaveManager()
        {
            return m_saveManager;
        }
    }
}
