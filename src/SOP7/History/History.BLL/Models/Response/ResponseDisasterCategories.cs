using SOPManager.Model.Sop.Category;
using System;
using System.Collections.Generic;
using System.Text;

namespace History.BLL.Models.Response
{
    public class ResponseDisasterCategories
    {
        private List<DisasterCategory> m_disasterCategories = new List<DisasterCategory>();
        public List<DisasterCategory> DisasterCategories
        {
            get { return m_disasterCategories; }
            set { m_disasterCategories = value; }
        }
    }
}
