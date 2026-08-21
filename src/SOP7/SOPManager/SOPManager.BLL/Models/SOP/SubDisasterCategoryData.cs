using System.Collections.Generic;
using SOPManager.Model.Sop.Category;

namespace SOPManager.BLL.Models.SOP
{
    public class SubDisasterCategoryData
    {
        private SubDisasterCategory m_subDisasterCategory = null;
        private List<VersionDisasterData> m_disasterDatas = new List<VersionDisasterData>();
        // 같은 이름을 가진 여러 버전의 Disaster가 있을수 있다.
        // List 대신 Dictionary를 사용하는 이유다.
        //private Dictionary<string, List<DisasterData>> m_dicDisasters = new Dictionary<string, List<DisasterData>>();
        //private List<DisasterData> m_disasters = new List<DisasterData>();

        public SubDisasterCategory SubDisasterCategory
        {
            get { return m_subDisasterCategory; }
            set { m_subDisasterCategory = value; }
        }

        public List<VersionDisasterData> DisasterDatas
        {
            get { return m_disasterDatas; }
        }
        /*public Dictionary<string, List<DisasterData>> Disasters
        {
            get { return m_dicDisasters; }
        }*/
    }
}
