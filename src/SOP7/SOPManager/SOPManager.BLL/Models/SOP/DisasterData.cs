using System.Collections.Generic;
using SOPManager.Model.Sop.Category;

namespace SOPManager.BLL.Models.SOP
{
    public class DisasterData
    {
        private Disaster m_disaster = null;
        private List<ActionStepData> m_actionSteps = new List<ActionStepData>();
        private Version m_version = null;
        // Version 작성자의 ID
        private string m_strUserID = "";

        public Disaster Disaster
        {
            get { return m_disaster; }
            set { m_disaster = value; }
        }

        public List<ActionStepData> ActionSteps
        {
            get { return m_actionSteps; }
        }

        public Version Version
        {
            get { return m_version; }
            set { m_version = value; }
        }

        // Version 작성자의 ID
        public string Owner
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }
    }
}
