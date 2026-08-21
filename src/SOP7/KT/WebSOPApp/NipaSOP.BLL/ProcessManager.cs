using System;

namespace NipaSOP.BLL
{
    using IDAL;
    using Models.Request;
    using Models.Response;

    public class ProcessManager
    {
        private string m_strSOPWebServerURL = "";
        public string SOPWebServerURL
        {
            get { return m_strSOPWebServerURL; }
            set { m_strSOPWebServerURL = value; }
        }

        private IDataManager m_dataManager = null;
        private SOPManager.IDAL.IDataManager m_sopDataManager = null;
        private Common.IDAL.IDataManager m_commonDataManager = null;
        private TeamEditor.IDAL.IDataManager m_teamDataManager = null;
        private SDMS.IDAL.IDataManager m_sdmsDataManager = null;
        
        public IDataManager DataManager
        {
            get { return m_dataManager; }
        }

        public SOPManager.IDAL.IDataManager SOPDataManager
        {
            get { return m_sopDataManager; }
        }

        public Common.IDAL.IDataManager CommonDataManager
        {
            get { return m_commonDataManager; }
        }

        public TeamEditor.IDAL.IDataManager TeamDataManager
        {
            get { return m_teamDataManager; }
        }

        public SDMS.IDAL.IDataManager SDMSDataManager
        {
            get { return m_sdmsDataManager; }
        }

        public ProcessManager(IDataManager dataManager, SOPManager.IDAL.IDataManager sopDataManager, Common.IDAL.IDataManager commonDataManager, TeamEditor.IDAL.IDataManager teamDataManager, SDMS.IDAL.IDataManager sdmsDataManager)
        {
            m_dataManager = dataManager;
            m_sopDataManager = sopDataManager;
            m_commonDataManager = commonDataManager;
            m_teamDataManager = teamDataManager;
            m_sdmsDataManager = sdmsDataManager;
        }

        public ResponseStartInfo SetStartInfo(StartInfo info)
        {
            ResponseStartInfo response = new ResponseStartInfo();

            if (this.m_dataManager == null)
            {
                response.Success = false;
                response.Message = "DB에 접속할 수 없습니다.";
            }
            else
            {
                DateTime dtNow = DateTime.Now;
                DateTime dtOld = dtNow.AddHours(-1);
                bool isNullable;

                string strAdditionalConditions = string.Format("{0} < '{1}-{2:00}-{3:00} {4:00}:{5:00}:{6:00}'",
                    Model.Sop.StartInfo.GetFieldName(Model.Sop.StartInfo.Fields.TimeStamp, out isNullable),
                    dtOld.Year, dtOld.Month, dtOld.Day, dtOld.Hour, dtOld.Minute, dtOld.Second);

                string strErrorMessage;
                
                // 한시간 이전에 만들어진 데이터는 삭제한다.
                if (m_dataManager.GetDeleteManager().DeleteStartInfo(null, strAdditionalConditions, out strErrorMessage) == false)
                {
                    response.Success = false;
                    response.Message = strErrorMessage;
                    return response;
                }

                Model.Sop.StartInfo startInfo = m_dataManager.GetCreateManager().CreateStartInfo(DateTime.Now, info.AccessMode, info.AccessToken, info.ServiceType, info.FacilityID, true);

                if (startInfo == null)
                {
                    response.Success = false;
                    response.Message = m_dataManager.GetCreateManager().GetErrorMessage();
                }
                else
                {
                    response.Success = true;
                    response.BeginCode = startInfo.ID;
                }
            }

            return response;
        }

        public ResponseRunSOP RunSOP(int nBeginCode)
        {
            SOPRunManager runManager = new SOPRunManager("");
            return runManager.RunSOP(nBeginCode, this);
        }
    }
}
