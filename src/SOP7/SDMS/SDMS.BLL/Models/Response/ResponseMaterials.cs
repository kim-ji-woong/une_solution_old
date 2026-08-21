using System.Collections.Generic;

namespace SDMS.BLL.Models.Response
{
    using SDMS.Model.Sensor;

    public class ResponseMaterials : MessageResult
    {
        private List<Material> m_materials = null;

        public List<Material> Materials
        {
            get { return m_materials; }
            set { m_materials = value; }
        }
    }
}
