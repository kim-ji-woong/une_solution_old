using System;
using System.Collections.Generic;
using System.Text;

namespace SOPManager.BLL.Models.Response
{
    using SOP;

    public class ResponseDisasterCategories : MessageResult
    {
        private List<DisasterCategoryData> m_disasterCategoryDatas = new List<DisasterCategoryData>();

        public List<DisasterCategoryData> DisasterCategoryDatas
        {
            get { return m_disasterCategoryDatas; }
        }
    }
}
