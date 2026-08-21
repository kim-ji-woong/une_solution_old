using System.Collections.Generic;

namespace SOPManager.BLL.Models.Response
{
    using SOP;

    public class ResponseStepMemberData : MessageResult
    {
        private StepMemberData m_stepMemberData = null;

        public StepMemberData StepMemberData
        {
            get { return m_stepMemberData; }
            set { m_stepMemberData = value; }
        }
    }

    public class ResponseActionStepDatas : MessageResult
    {
        private List<ActionStepData> m_actionStepDatas = new List<ActionStepData>();

        public List<ActionStepData> ActionStepDatas
        {
            get { return m_actionStepDatas; }
        }
    }
}
