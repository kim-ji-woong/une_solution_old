using SOPManager.Model.Sop.Category;
using System.Collections.Generic;
using System.Reflection;

namespace SOPManager.BLL.Models.SOP
{
    public class VersionData : Model.Sop.Category.Version
    {
        private string m_strOwnerName = "";

        public VersionData()
        {
        }

        public VersionData(Version version)
        {
            foreach (var propSrc in typeof(Version).GetProperties())
            {
                PropertyInfo propTrg = this.GetType().GetProperty(propSrc.Name);

                if (propTrg == null)
                    continue;

                var value = propSrc.GetValue(version, null);
                propTrg.SetValue(this, value, null);
            }
        }

        public string Owner
        {
            get { return m_strOwnerName; }
            set { m_strOwnerName = value; }
        }
    }

    public class VersionDisasterData
    {
        private string m_strDisasterName = "";
        // 버전별로 정렬된 Disaster들
        private List<DisasterData> m_disasterDatas = new List<DisasterData>();

        public string DisasterName
        {
            get { return m_strDisasterName; }
            set { m_strDisasterName = value; }
        }

        public List<DisasterData> DisasterDatas
        {
            get { return m_disasterDatas; }
        }
    }
}
