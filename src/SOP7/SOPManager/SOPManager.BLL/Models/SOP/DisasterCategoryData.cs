using System.Collections.Generic;
using SOPManager.Model.Sop.Category;

namespace SOPManager.BLL.Models.SOP
{
    public class DisasterCategoryData
    {
        private DisasterCategory m_disasterCategory = null;
        private List<SubDisasterCategoryData> m_subDisasterCategories = new List<SubDisasterCategoryData>();

        public DisasterCategory DisasterCategory
        {
            get { return m_disasterCategory; }
            set { m_disasterCategory = value; }
        }

        public List<SubDisasterCategoryData> SubDisasterCategories
        {
            get { return m_subDisasterCategories; }
        }
    }
}
