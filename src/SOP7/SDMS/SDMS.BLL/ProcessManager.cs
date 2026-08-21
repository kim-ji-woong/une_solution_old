using System;
using System.Collections.Generic;
using System.Text;

namespace SDMS.BLL
{
    public class ProcessManager
    {
        private string m_strSOPWebServerURL = "";
        public string SOPWebServerURL
        {
            get { return m_strSOPWebServerURL; }
            set { m_strSOPWebServerURL = value; }
        }

        private string m_strStreamServerURL = "";
        public string StreamServerURL
        {
            get { return m_strStreamServerURL; }
            set { m_strStreamServerURL = value; }
        }

        private Common.IDAL.IDataManager m_commonDataManager = null;
        private SDMS.IDAL.IDataManager m_sdmsDataManager = null;
        private SOPManager.IDAL.IDataManager m_sopDataManager = null;
        private TeamEditor.IDAL.IDataManager m_teamDataManager = null;

        public Common.IDAL.IDataManager CommonDataManager
        {
            get { return m_commonDataManager; }
        }

        public SOPManager.IDAL.IDataManager SopDataManager
        {
            get { return m_sopDataManager; }
        }

        public TeamEditor.IDAL.IDataManager TeamDataManager
        {
            get { return m_teamDataManager; }
        }

        public SDMS.IDAL.IDataManager SdmsDataManager
        {
            get { return m_sdmsDataManager; }
        }

        private LoadManager m_loadManager = null;
        private SaveManager m_saveManager = null;
        private AlarmManager m_alarmManager = null;
        private ExcelManager m_excelManager = null;

        public ProcessManager(Common.IDAL.IDataManager commonDataManager, SDMS.IDAL.IDataManager sdmsDataManager, SOPManager.IDAL.IDataManager sopDataManager, TeamEditor.IDAL.IDataManager teamDataManager)
        {
            this.m_commonDataManager = commonDataManager;
            this.m_sdmsDataManager = sdmsDataManager;
            this.m_sopDataManager = sopDataManager;
            this.m_teamDataManager = teamDataManager;

            m_loadManager = new LoadManager(m_sdmsDataManager, this);
            m_saveManager = new SaveManager(m_sdmsDataManager, this);
            m_alarmManager = new AlarmManager(m_sdmsDataManager, this);
            m_excelManager = new ExcelManager(m_sdmsDataManager, this);
        }

        public LoadManager GetLoadManager()
        {
            return m_loadManager;
        }

        public SaveManager GetSaveManager()
        {
            return m_saveManager;
        }

        public AlarmManager GetAlarmManager()
        {
            return m_alarmManager;
        }

        public ExcelManager GetExcelManager()
        {
            return m_excelManager;
        }
    }
}
