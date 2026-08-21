using System.Collections.Generic;

namespace SOPManager.BLL.Models.Response
{
    using Model.Sop.Config;

    public class ResponseExternalProgram : MessageResult
    {
        private List<ExternalProgramData> m_programs = new List<ExternalProgramData>();

        public List<ExternalProgramData> Programs
        {
            get { return m_programs; }
        }
    }

    public class ExternalProgramData
    {
        private ExternalProgram m_program = null;
        private List<ExternalProgramParameter> m_parameters = new List<ExternalProgramParameter>();

        public ExternalProgram Program
        {
            get { return m_program; }
            set { m_program = value; }
        }

        public List<ExternalProgramParameter> Parameters
        {
            get { return m_parameters; }
        }
    }
}
