using System.Collections.Generic;
using SOPManager.Model.Sop.Category;

namespace SOPManager.BLL.Models.Response
{
    using SOP;

    public class ResponseDisasterVersions : MessageResult
    {
        private List<VersionData> m_versions = new List<VersionData>();
        private VersionData m_currentVersion = null;
        
        public List<VersionData> Versions
        {
            get { return m_versions; }
        }

        public VersionData CurrentVersion
        {
            get { return m_currentVersion; }
            set { m_currentVersion = value; }
        }
    }
}
