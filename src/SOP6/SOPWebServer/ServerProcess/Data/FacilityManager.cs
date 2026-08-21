using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Sensor;
using UnE.Spatial;
using DBUtility2;
using System.Collections;

namespace ServerProcess.Data
{
    public class FacilityManager
    {
        private int m_nID = -1;
        private int m_nMemberID = -1;
        private int m_nMemberType = -1;
        private UnE.Sensor.IFacility.FacilityType m_type = UnE.Sensor.IFacility.FacilityType.NONE;
        private int m_nLevelLimit = -1;
        // 이 값이 0보다 크면 ~급 및 그 상위직급만 해당
        //         0이면 ~급만 해당
        //         0보다 작으면 ~급 및 그 하위직급만 해당
        private int m_nUpperLimit = 0;
        private string m_strDescription = "";
        private object m_tag = null;
        private UnE.Spatial.Building m_building = null;
        private UnE.Spatial.Zone m_zone = null;
        private FacilityManagerGroup m_group = null;

        // FacilityType별 DB Table 이름
        private static Dictionary<UnE.Sensor.IFacility.FacilityType, string> m_dicFacilityTypeTable = new Dictionary<UnE.Sensor.IFacility.FacilityType, string>();

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int MemberID
        {
            get { return m_nMemberID; }
            set { m_nMemberID = value; }
        }

        // 0(CompanyMember), 1(RegularTeam), 2(ExternalCompanyMember), 3(ExternalCompanyTeam), 4(RegularCompany), 5(ExternalCompany), 6(당직자)
        public int MemberType
        {
            get { return m_nMemberType; }
            set { m_nMemberType = value; }
        }

        public UnE.Sensor.IFacility.FacilityType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public int LevelLimit
        {
            get { return m_nLevelLimit; }
            set { m_nLevelLimit = value; }
        }

        // 이 값이 0보다 크면 ~급 및 그 상위직급만 해당
        //         0이면 ~급만 해당
        //         0보다 작으면 ~급 및 그 하위직급만 해당
        public int UpperLimit
        {
            get { return m_nUpperLimit; }
            set { m_nUpperLimit = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public object Tag
        {
            get { return m_tag; }
            set { m_tag = value; }
        }

        public UnE.Spatial.Building Building
        {
            get { return m_building; }
            set { m_building = value; }
        }

        public UnE.Spatial.Zone Zone
        {
            get { return m_zone; }
            set { m_zone = value; }
        }

        public FacilityManagerGroup Group
        {
            get { return m_group; }
            set { m_group = value; }
        }

        public FacilityManager Clone()
        {
            FacilityManager mgr = new FacilityManager();

            mgr.m_nID = this.m_nID;
            mgr.m_nMemberID = this.m_nMemberID;
            mgr.m_nMemberType = this.m_nMemberType;
            mgr.m_type = this.m_type;
            mgr.m_nLevelLimit = this.m_nLevelLimit;
            mgr.m_strDescription = this.m_strDescription;
            mgr.m_nUpperLimit = this.m_nUpperLimit;
            mgr.m_tag = this.m_tag;

            return mgr;
        }

        public void CopyFrom(FacilityManager mgr)
        {
            this.m_nID = mgr.m_nID;
            this.m_nMemberID = mgr.m_nMemberID;
            this.m_nMemberType = mgr.m_nMemberType;
            this.m_type = mgr.m_type;
            this.m_nLevelLimit = mgr.m_nLevelLimit;
            this.m_strDescription = mgr.m_strDescription;
            this.m_tag = mgr.m_tag;
        }

        public bool IsSame(FacilityManager mgr)
        {
            if (this.m_nID != mgr.m_nID)
                return false;

            if (this.m_nMemberID != mgr.m_nMemberID)
                return false;

            if (this.m_nMemberType != mgr.m_nMemberType)
                return false;

            if (this.m_type != mgr.m_type)
                return false;

            if (this.m_nLevelLimit != mgr.m_nLevelLimit)
                return false;

            if (this.m_strDescription != mgr.m_strDescription)
                return false;

            if (this.m_tag != mgr.m_tag)
                return false;

            return true;
        }

        public static void ReadFacilityTypes(DirectDBManager dbMgr)
        {
            string strSQL = "select ID, LinkedTableName from FacilityType";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTableName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (strTableName == null)
                    continue;

                SetFacilityTypeTable(nID, strTableName);
            }
        }

        // FacilityType별 DB Table 이름 지정
        public static void SetFacilityTypeTable(int nFacilityType, string strTableName)
        {
            UnE.Sensor.IFacility.FacilityType type = UnE.Sensor.IFacility.ToFacilityType(nFacilityType);

            if (type == UnE.Sensor.IFacility.FacilityType.NONE)
                return;

            SetFacilityTypeTable(type, strTableName);
        }

        // FacilityType별 DB Table 이름 지정
        public static void SetFacilityTypeTable(UnE.Sensor.IFacility.FacilityType type, string strTableName)
        {
            m_dicFacilityTypeTable[type] = strTableName;
        }

        public static string GetFacilityTypeTable(UnE.Sensor.IFacility.FacilityType type)
        {
            string strTableName;

            if (m_dicFacilityTypeTable.TryGetValue(type, out strTableName))
                return strTableName;

            return "";
        }

        private static FacilityManagerGroup GetFacilityManagerGroup(int nFacilityType, Dictionary<IFacility.FacilityType, FacilityManagerGroup> dicFacilityManagers)
        {
            FacilityManagerGroup group = null;

            if (nFacilityType >= 0 && nFacilityType <= 2)
            {
                IFacility.FacilityType typeFire = IFacility.FacilityType.FIRE_SENSOR;

                if (dicFacilityManagers.ContainsKey(typeFire))
                    group = dicFacilityManagers[typeFire];
                else
                {
                    group = new FacilityManagerGroup();
                    group.Type = typeFire;

                    dicFacilityManagers[typeFire] = group;
                    dicFacilityManagers[IFacility.FacilityType.COOLER_SENSOR] = group;
                    dicFacilityManagers[IFacility.FacilityType.PRESSURE_SENSOR] = group;
                }
            }
            else if (nFacilityType == 3)
            {
                IFacility.FacilityType type = IFacility.FacilityType.CCTV;

                if (dicFacilityManagers.ContainsKey(type))
                    group = dicFacilityManagers[type];
                else
                {
                    group = new FacilityManagerGroup();
                    group.Type = type;
                    dicFacilityManagers[type] = group;
                }
            }
            else if (nFacilityType >= 4 && nFacilityType <= 6)
            {
                IFacility.FacilityType typeFE = IFacility.FacilityType.FE;

                if (dicFacilityManagers.ContainsKey(typeFE))
                    group = dicFacilityManagers[typeFE];
                else
                {
                    group = new FacilityManagerGroup();
                    group.Type = typeFE;

                    dicFacilityManagers[typeFE] = group;
                    dicFacilityManagers[IFacility.FacilityType.HD] = group;
                    dicFacilityManagers[IFacility.FacilityType.FA] = group;
                }
            }
            else if (nFacilityType == 11)
            {
                IFacility.FacilityType type = IFacility.FacilityType.PSM_SENSOR;

                if (dicFacilityManagers.ContainsKey(type))
                    group = dicFacilityManagers[type];
                else
                {
                    group = new FacilityManagerGroup();
                    group.Type = type;
                    dicFacilityManagers[type] = group;
                }
            }
            else if (nFacilityType == (int)IFacility.FacilityType.Security_Sensor)
            {
                IFacility.FacilityType type = IFacility.FacilityType.Security_Sensor;

                if (dicFacilityManagers.ContainsKey(type))
                    group = dicFacilityManagers[type];
                else
                {
                    group = new FacilityManagerGroup();
                    group.Type = type;
                    dicFacilityManagers[type] = group;
                }
            }
            else if (nFacilityType == (int)IFacility.FacilityType.TEMPERATURE_HUMIDITY)
            {
                IFacility.FacilityType type = IFacility.FacilityType.TEMPERATURE_HUMIDITY;

                if (dicFacilityManagers.ContainsKey(type))
                    group = dicFacilityManagers[type];
                else
                {
                    group = new FacilityManagerGroup();
                    group.Type = type;
                    dicFacilityManagers[type] = group;
                }
            }

            return group;
        }
    }

    public class FacilityManagerGroup
    {
        /*class ArrayListEx : ArrayList
        {
            private FacilityManagerGroup m_group = null;

            public ArrayListEx(FacilityManagerGroup group)
            {
                m_group = group;
            }

            public override int Add(object value)
            {
                if (value.GetType() == typeof(FacilityManager))
                {
                    FacilityManager mgr = (FacilityManager)value;
                    mgr.Group = m_group;
                }

                return base.Add(value);
            }

            public override void Remove(object obj)
            {
                base.Remove(obj);

                if (m_group != null && obj.GetType() == typeof(FacilityManager))
                {
                    FacilityManager mgr = (FacilityManager)obj;
                    mgr.Group = null;
                }
            }

            public override void RemoveAt(int index)
            {
                object obj = base[index];

                base.RemoveAt(index);

                if (m_group != null && obj.GetType() == typeof(FacilityManager))
                {
                    FacilityManager mgr = (FacilityManager)obj;
                    mgr.Group = null;
                }
            }
        }*/

        private UnE.Sensor.IFacility.FacilityType m_type = UnE.Sensor.IFacility.FacilityType.NONE;
        /*// Key : 정규조직
        // Value : 몇급 이상으로 설정할 것인가?
        //         이 값이 음수이면 모든 팀원
        private Dictionary<DataTeam, int> m_dicRegularTeams = new Dictionary<DataTeam, int>();*/
        private List<FacilityManager> m_arrRegularTeams = null;//new ArrayList();
        private List<FacilityManager> m_arrCompanyMembers = null;//new ArrayList();
        private List<FacilityManager> m_arrExternalTeams = null;//new ArrayList();
        private List<FacilityManager> m_arrExternalCompanyMembers = null;//new ArrayList();
        // 교대 근무자
        private List<FacilityManager> m_arrControlRoomMembers = null;

        // 특정 건물의 담당자일 경우 m_building이 값을 가진다.
        private UnE.Spatial.Building m_building = null;

        // 특정 외부영역의 담당자일 경우 m_zone이 값을 가진다.
        private UnE.Spatial.Zone m_zone = null;

        // 특정 Equip 존의 담당자일 경우 m_equipZone이 값을 가진다.
        private UnE.Spatial.EquipmentZone m_equipZone = null;

        public FacilityManagerGroup()
        {
            m_arrRegularTeams = new List<FacilityManager>();
            m_arrCompanyMembers = new List<FacilityManager>();
            m_arrExternalTeams = new List<FacilityManager>();
            m_arrExternalCompanyMembers = new List<FacilityManager>();
            m_arrControlRoomMembers = new List<FacilityManager>();
        }

        public IFacility.FacilityType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public List<FacilityManager> RegularTeams
        {
            get { return m_arrRegularTeams; }
        }

        /*// Key : 정규조직
        // Value : 몇급 이상으로 설정할 것인가?
        //         이 값이 음수이면 모든 팀원
        public Dictionary<DataTeam, int> RegularTeams
        {
            get { return m_dicRegularTeams; }
        }*/

        public List<FacilityManager> CompanyMembers
        {
            get { return m_arrCompanyMembers; }
        }

        public List<FacilityManager> ExternalTeams
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
        }

        public void CopyFrom(FacilityManagerGroup group)
        {
            m_type = group.m_type;

            m_arrRegularTeams.Clear();
            foreach (FacilityManager mgr in group.m_arrRegularTeams)
            {
                m_arrRegularTeams.Add(mgr.Clone());
            }

            m_arrCompanyMembers.Clear();
            foreach (FacilityManager mgr in group.m_arrCompanyMembers)
            {
                m_arrCompanyMembers.Add(mgr.Clone());
            }

            m_arrExternalTeams.Clear();
            foreach (FacilityManager mgr in group.m_arrExternalTeams)
            {
                m_arrExternalTeams.Add(mgr.Clone());
            }

            m_arrExternalCompanyMembers.Clear();
            foreach (FacilityManager mgr in group.m_arrExternalCompanyMembers)
            {
                m_arrExternalCompanyMembers.Add(mgr.Clone());
            }

            m_arrControlRoomMembers.Clear();
            foreach (FacilityManager mgr in group.m_arrControlRoomMembers)
            {
                m_arrControlRoomMembers.Add(mgr.Clone());
            }
        }

        protected bool IsSameList(List<FacilityManager> managers1, List<FacilityManager> managers2)
        {
            if (managers1.Count != managers2.Count)
                return false;

            foreach (FacilityManager mgr in managers1)
            {
                bool find = false;

                foreach (FacilityManager mgr2 in managers2)
                {
                    if (mgr.IsSame(mgr2))
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

            if (!IsSameList(m_arrExternalTeams, group.m_arrExternalTeams))
                return false;

            if (!IsSameList(m_arrExternalCompanyMembers, group.m_arrExternalCompanyMembers))
                return false;

            if (!IsSameList(m_arrControlRoomMembers, group.m_arrControlRoomMembers))
                return false;

            return true;
        }

        private FacilityManager Contains(List<FacilityManager> arrManagers, FacilityManager mgr)
        {
            foreach (FacilityManager manager in arrManagers)
            {
                if (manager.MemberType == mgr.MemberType &&
                    manager.Type == mgr.Type &&
                    manager.Tag == mgr.Tag)
                    return manager;
            }

            return null;
        }

        public void AddManager(FacilityManager mgr)
        {
            if (mgr.MemberType == 0)
            {
                FacilityManager manager = Contains(CompanyMembers, mgr);

                if (manager == null)
                    CompanyMembers.Add(mgr);
                else
                    manager.CopyFrom(mgr);
            }
            else if (mgr.MemberType == 1 || mgr.MemberType == 4)
            {
                FacilityManager manager = Contains(RegularTeams, mgr);

                if (manager == null)
                    RegularTeams.Add(mgr);
                else
                    manager.CopyFrom(mgr);
            }
            else if (mgr.MemberType == 2)
            {
                FacilityManager manager = Contains(ExternalCompanyMembers, mgr);

                if (manager == null)
                    ExternalCompanyMembers.Add(mgr);
                else
                    manager.CopyFrom(mgr);
            }
            else if (mgr.MemberType == 3 || mgr.MemberType == 5)
            {
                FacilityManager manager = Contains(ExternalTeams, mgr);

                if (manager == null)
                    ExternalTeams.Add(mgr);
                else
                    manager.CopyFrom(mgr);
            }
            else if (mgr.MemberType == 7)
            {
                FacilityManager manager = Contains(ControlRoomMembers, mgr);

                if (manager == null)
                    ControlRoomMembers.Add(mgr);
                else
                    manager.CopyFrom(mgr);
            }
        }

        public bool IsEmpty()
        {
            if (m_arrRegularTeams.Count > 0)
                return false;

            if (m_arrCompanyMembers.Count > 0)
                return false;

            if (m_arrExternalTeams.Count > 0)
                return false;

            if (m_arrExternalCompanyMembers.Count > 0)
                return false;

            if (m_arrControlRoomMembers.Count > 0)
                return false;

            return true;
        }

        public UnE.Spatial.Building Building
        {
            get { return m_building; }
            set { m_building = value; }
        }

        public UnE.Spatial.Zone Zone
        {
            get { return m_zone; }
            set { m_zone = value; }
        }

        public UnE.Spatial.EquipmentZone EquipZone
        {
            get { return m_equipZone; }
            set { m_equipZone = value; }
        }
    }
}
