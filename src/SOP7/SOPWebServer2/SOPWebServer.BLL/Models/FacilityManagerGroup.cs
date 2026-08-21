using System.Collections.Generic;
using dnsData.Sensor;
using SDMS.Model.Spatial;
using SDMS.Model.Sensor;
using TeamEditor.BLL;

namespace SOPWebServer.BLL.Models
{
    public class FacilityManagerEx : FacilityManager
    {
        private object m_tag = null;
        private FacilityManagerGroup m_group = null;
        // 이 값이 null이 아니면 BuildingFacilityManager
        private Building m_building = null;
        // 이 값이 null이 아니면 EquipZoneFacilityManager
        private EquipmentZone m_equipZone = null;
        // 이 값이 null이 아니면 OutdoorFacilityManager
        private Zone m_zone = null;

        public object Tag
        {
            get { return m_tag; }
            set { m_tag = value; }
        }

        // 이 값이 null이 아니면 BuildingFacilityManager
        public Building Building
        {
            get { return m_building; }
            set { m_building = value; }
        }

        // 이 값이 null이 아니면 EquipZoneFacilityManager
        public EquipmentZone EquipZone
        {
            get { return m_equipZone; }
            set { m_equipZone = value; }
        }

        // 이 값이 null이 아니면 OutdoorFacilityManager
        public Zone Zone
        {
            get { return m_zone; }
            set { m_zone = value; }
        }

        public FacilityManagerGroup Group
        {
            get { return m_group; }
            set { m_group = value; }
        }
    }

    public class FacilityManagerGroup
    {
        private Facility.FacilityType m_type = Facility.FacilityType.NONE;
        /*// Key : 정규조직
        // Value : 몇급 이상으로 설정할 것인가?
        //         이 값이 음수이면 모든 팀원
        private Dictionary<DataTeam, int> m_dicRegularTeams = new Dictionary<DataTeam, int>();*/
        private List<FacilityManagerEx> m_arrRegularTeams = null;//new ArrayList();
        private List<FacilityManagerEx> m_arrCompanyMembers = null;//new ArrayList();
        //private List<FacilityManager> m_arrExternalTeams = null;//new ArrayList();
        //private List<FacilityManager> m_arrExternalCompanyMembers = null;//new ArrayList();
        // 교대 근무자
        //private List<FacilityManager> m_arrControlRoomMembers = null;

        // 특정 건물의 담당자일 경우 m_building이 값을 가진다.
        private Building m_building = null;

        // 특정 외부영역의 담당자일 경우 m_zone이 값을 가진다.
        private Zone m_zone = null;

        // 특정 Equip 존의 담당자일 경우 m_equipZone이 값을 가진다.
        private EquipmentZone m_equipZone = null;

        public FacilityManagerGroup()
        {
            m_arrRegularTeams = new List<FacilityManagerEx>();
            m_arrCompanyMembers = new List<FacilityManagerEx>();
            //m_arrExternalTeams = new List<FacilityManager>();
            //m_arrExternalCompanyMembers = new List<FacilityManager>();
            //m_arrControlRoomMembers = new List<FacilityManager>();
        }

        public Facility.FacilityType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public List<FacilityManagerEx> RegularTeams
        {
            get { return m_arrRegularTeams; }
        }

        public List<FacilityManagerEx> CompanyMembers
        {
            get { return m_arrCompanyMembers; }
        }

        /*public List<FacilityManager> ExternalTeams
        {
            get { return m_arrExternalTeams; }
        }

        public List<FacilityManager> ExternalCompanyMembers
        {
            get { return m_arrExternalCompanyMembers; }
        }

        public List<FacilityManager> ControlRoomMembers
        {
            get { return m_arrControlRoomMembers; }
        }*/

        public Building Building
        {
            get { return m_building; }
            set { m_building = value; }
        }

        public Zone Zone
        {
            get { return m_zone; }
            set { m_zone = value; }
        }

        public EquipmentZone EquipZone
        {
            get { return m_equipZone; }
            set { m_equipZone = value; }
        }

        public void CopyFrom(FacilityManagerGroup group)
        {
            m_type = group.m_type;

            m_arrRegularTeams.Clear();
            foreach (FacilityManagerEx mgr in group.m_arrRegularTeams)
            {
                m_arrRegularTeams.Add(CopyFrom(mgr));
            }

            m_arrCompanyMembers.Clear();
            foreach (FacilityManagerEx mgr in group.m_arrCompanyMembers)
            {
                m_arrCompanyMembers.Add(CopyFrom(mgr));
            }

            /*m_arrExternalTeams.Clear();
            foreach (FacilityManager mgr in group.m_arrExternalTeams)
            {
                m_arrExternalTeams.Add(CopyFrom(mgr));
            }

            m_arrExternalCompanyMembers.Clear();
            foreach (FacilityManager mgr in group.m_arrExternalCompanyMembers)
            {
                m_arrExternalCompanyMembers.Add(CopyFrom(mgr));
            }

            m_arrControlRoomMembers.Clear();
            foreach (FacilityManager mgr in group.m_arrControlRoomMembers)
            {
                m_arrControlRoomMembers.Add(CopyFrom(mgr));
            }*/
        }

        private FacilityManagerEx CopyFrom(FacilityManagerEx manager)
        {
            FacilityManagerEx mgr = new FacilityManagerEx();

            mgr.ID = manager.ID;
            mgr.MemberID = manager.MemberID;
            mgr.MemberType = manager.MemberType;
            mgr.FacilityType = manager.FacilityType;
            mgr.SiteID = manager.SiteID;
            mgr.Description = manager.Description;
            mgr.Tag = manager.Tag;

            return mgr;
        }

        protected bool IsSameList(List<FacilityManagerEx> managers1, List<FacilityManagerEx> managers2)
        {
            if (managers1.Count != managers2.Count)
                return false;

            foreach (FacilityManagerEx mgr in managers1)
            {
                bool find = false;

                foreach (FacilityManagerEx mgr2 in managers2)
                {
                    if (IsSame(mgr, mgr2))
                    {
                        find = true;
                        break;
                    }
                }

                if (!find)
                    return false;
            }

            return true;
        }

        private bool IsSame(FacilityManagerEx mgr1, FacilityManagerEx mgr2)
        {
            if (mgr1.ID != mgr2.ID)
                return false;

            if (mgr1.MemberID != mgr2.MemberID)
                return false;

            if (mgr1.MemberType != mgr2.MemberType)
                return false;

            if (mgr1.FacilityType != mgr2.FacilityType)
                return false;

            if (mgr1.SiteID != mgr2.SiteID)
                return false;

            if (mgr1.Tag != mgr2.Tag)
                return false;

            return true;
        }

        public bool IsSame(FacilityManagerGroup group)
        {
            if (group == null)
                return false;

            if (m_type != group.m_type)
                return false;

            if (!IsSameList(m_arrRegularTeams, group.m_arrRegularTeams))
                return false;

            if (!IsSameList(m_arrCompanyMembers, group.m_arrCompanyMembers))
                return false;

            /*if (!IsSameList(m_arrExternalTeams, group.m_arrExternalTeams))
                return false;

            if (!IsSameList(m_arrExternalCompanyMembers, group.m_arrExternalCompanyMembers))
                return false;

            if (!IsSameList(m_arrControlRoomMembers, group.m_arrControlRoomMembers))
                return false;*/

            return true;
        }

        private FacilityManagerEx Contains(List<FacilityManagerEx> arrManagers, FacilityManagerEx mgr)
        {
            foreach (FacilityManagerEx manager in arrManagers)
            {
                if (manager.MemberType == mgr.MemberType &&
                    manager.FacilityType == mgr.FacilityType &&
                    manager.Tag == mgr.Tag)
                    return manager;
            }

            return null;
        }

        public void AddManager(FacilityManagerEx mgr)
        {
            if (mgr.MemberType == (int)TemporaryMemberData.MemberType.RegularTeam)
            {
                FacilityManagerEx manager = Contains(CompanyMembers, mgr);

                if (manager == null)
                    CompanyMembers.Add(mgr);
                else
                    manager = CopyFrom(mgr);
            }
            else if (mgr.MemberType == (int)TemporaryMemberData.MemberType.RegularMember)
            {
                FacilityManagerEx manager = Contains(RegularTeams, mgr);

                if (manager == null)
                    RegularTeams.Add(mgr);
                else
                    manager = CopyFrom(mgr);
            }
        }

        public bool IsEmpty()
        {
            if (m_arrRegularTeams.Count > 0)
                return false;

            if (m_arrCompanyMembers.Count > 0)
                return false;

            /*if (m_arrExternalTeams.Count > 0)
                return false;

            if (m_arrExternalCompanyMembers.Count > 0)
                return false;

            if (m_arrControlRoomMembers.Count > 0)
                return false;*/

            return true;
        }
    }
}
