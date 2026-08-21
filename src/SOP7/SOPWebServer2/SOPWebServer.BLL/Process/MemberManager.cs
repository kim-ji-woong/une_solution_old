using System.Collections.Generic;
using TeamEditor.Model.Sop.Team;
using dnsData.Sensor;
using SDMS.Model.Spatial;
using SDMS.Model.Sensor;
using AgentFactory.BLL;
using TeamEditor.BLL;
using Common.Model;

namespace SOPWebServer.BLL.Process
{
    using Models;

    public class MemberManager
    {
        private Regular m_teamRegularRoot = null;
        private Dictionary<int, Regular> m_dicRegularTeams = new Dictionary<int, Regular>();
        private Dictionary<Regular, List<RegularMember>> m_dicRegularTeamMembers = new Dictionary<Regular, List<RegularMember>>();
        private Dictionary<int, RegularMember> m_dicRegularMembers = new Dictionary<int, RegularMember>();

        // 시설물 타입별 전체 담당자(재난 탐지시)
        private Dictionary<Facility.FacilityType, FacilityManagerGroup> m_dicEntireFacilityManagers = new Dictionary<Facility.FacilityType, FacilityManagerGroup>();
        // 시설물 타입별 전체 담당자(재난 전파시)
        private Dictionary<Facility.FacilityType, FacilityManagerGroup> m_dicEntireFacilityManagersReport = new Dictionary<Facility.FacilityType, FacilityManagerGroup>();
        // 건물별 시설물 담당자(재난 탐지시)
        private Dictionary<Facility.FacilityType, Dictionary<Building, FacilityManagerGroup>> m_dicBuildingFacilityManager = new Dictionary<Facility.FacilityType, Dictionary<Building, FacilityManagerGroup>>();
        // 건물별 시설물 담당자(재난 전파시)
        private Dictionary<Facility.FacilityType, Dictionary<Building, FacilityManagerGroup>> m_dicBuildingFacilityManagerReport = new Dictionary<Facility.FacilityType, Dictionary<Building, FacilityManagerGroup>>();
        // 외부 Zone별 시설물 담당자(재난 탐지시)
        private Dictionary<Facility.FacilityType, Dictionary<Zone, FacilityManagerGroup>> m_dicOutdoorFacilityManager = new Dictionary<Facility.FacilityType, Dictionary<Zone, FacilityManagerGroup>>();
        // 외부 Zone별 시설물 담당자(재난 전파시)
        private Dictionary<Facility.FacilityType, Dictionary<Zone, FacilityManagerGroup>> m_dicOutdoorFacilityManagerReport = new Dictionary<Facility.FacilityType, Dictionary<Zone, FacilityManagerGroup>>();
        // EquipZone 별 시설물 담당자(재난 탐지시)
        private Dictionary<Facility.FacilityType, Dictionary<int, FacilityManagerGroup>> m_dicEquipZoneFacilityManager = new Dictionary<Facility.FacilityType, Dictionary<int, FacilityManagerGroup>>();
        // EquipZone 별 시설물 담당자(재난 전파시)
        private Dictionary<Facility.FacilityType, Dictionary<int, FacilityManagerGroup>> m_dicEquipZoneFacilityManagerReport = new Dictionary<Facility.FacilityType, Dictionary<int, FacilityManagerGroup>>();

        private MainManager m_mainManager = null;

        public MemberManager(MainManager mainManager)
        {
            m_mainManager = mainManager;
        }

        public void Initialize()
        {
            m_teamRegularRoot = LoadRegularTeam(m_dicRegularTeams);

            // SiteID를 고려하지 않은 전체 직원 리스트
            Dictionary<int, RegularMember> members = new Dictionary<int, RegularMember>();

            LoadRegularMember(members);

            LoadRegularMemberList(m_dicRegularTeams, members);
        }

        public void LoadFacilityManager()
        {
            m_dicEntireFacilityManagers.Clear();
            m_dicBuildingFacilityManager.Clear();
            m_dicOutdoorFacilityManager.Clear();
            m_dicEquipZoneFacilityManager.Clear();

            m_dicEntireFacilityManagersReport.Clear();
            m_dicBuildingFacilityManagerReport.Clear();
            m_dicOutdoorFacilityManagerReport.Clear();
            m_dicEquipZoneFacilityManagerReport.Clear();

            LoadFacilityManager(true);
            LoadBuildingNOutdoorFacilityManager(true);
            LoadEquipZoneFacilityManager(true);

            LoadFacilityManager(false);
            LoadBuildingNOutdoorFacilityManager(false);
            LoadEquipZoneFacilityManager(false);
        }

        private bool UseFacilityManagerType()
        {
            string strErrorMessage;
            List<Common.Model.Option.Options> options = m_mainManager.CommonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SDMS, "UseFacilityManagerType", out strErrorMessage);

            if (options == null || options.Count == 0)
                return false;

            string strValue = options[0].PropertyValue;

            if (strValue == null)
                return false;

            strValue = strValue.Trim();

            if (strValue == "1" || string.Compare(strValue, "true", true) == 0)
            {
                return true;
            }

            return false;
        }

        private void LoadEquipZoneFacilityManager(bool isDetectTime)
        {
            Dictionary<EquipZoneFacilityManager.Fields, object> dicConditions = new Dictionary<EquipZoneFacilityManager.Fields, object>();
            dicConditions[EquipZoneFacilityManager.Fields.DetectType] = isDetectTime ? (int)FacilityManager.DetectTypes.Detect : (int)FacilityManager.DetectTypes.Report;
            dicConditions[EquipZoneFacilityManager.Fields.SiteID] = m_mainManager.SDMSDataManager.SiteID;

            string strErrorMessage;
            List<EquipZoneFacilityManager> managers = m_mainManager.SDMSDataManager.GetSelectManager().SelectEquipZoneFacilityManagers(dicConditions, null, out strErrorMessage);

            if (managers == null)
                return;

            foreach (EquipZoneFacilityManager mgr in managers)
            {
                EquipmentZone equipZone = m_mainManager.SensorManager.GetEquipmentZone(mgr.EquipZoneID);

                if (equipZone == null)
                    continue;

                FacilityManagerGroup group = GetEquipZoneFacilityManagerGroup(mgr.FacilityType, equipZone, isDetectTime);

                if (group == null)
                    continue;

                AddFacilityManager(mgr.ID, mgr.MemberID, mgr.MemberType, mgr.FacilityType, mgr.DetectType, mgr.Description, group);
            }
        }

        private FacilityManagerGroup GetEquipZoneFacilityManagerGroup(int nFacilityType, EquipmentZone equipZone, bool isDetectTime)
        {
            Dictionary<Facility.FacilityType, Dictionary<int, FacilityManagerGroup>> dicEquipZoneFacilityManager = isDetectTime ? m_dicEquipZoneFacilityManager : m_dicEquipZoneFacilityManagerReport;
            FacilityManagerGroup group = null;

            Facility.FacilityType sensorType = (Facility.FacilityType)nFacilityType;

            if (BaseBroadcastManager.IsFireSensor(sensorType))
            {
                group = SetEquipZoneFacilityManagerGroup(dicEquipZoneFacilityManager, equipZone, Facility.FacilityType.FIRE_SENSOR);
            }
            else if (BaseBroadcastManager.IsSecuritySensor(sensorType))
            {
                group = SetEquipZoneFacilityManagerGroup(dicEquipZoneFacilityManager, equipZone, Facility.FacilityType.Security_Sensor);
            }
            else if (BaseBroadcastManager.IsPSMSensor(sensorType))
            {
                group = SetEquipZoneFacilityManagerGroup(dicEquipZoneFacilityManager, equipZone, Facility.FacilityType.PSM_SENSOR);
            }
            else if (BaseBroadcastManager.IsEarthquakeSensor(sensorType))
            {
                group = SetEquipZoneFacilityManagerGroup(dicEquipZoneFacilityManager, equipZone, Facility.FacilityType.Earthquake);
            }
            else if (BaseBroadcastManager.IsETCSensor(sensorType))
            {
                group = SetEquipZoneFacilityManagerGroup(dicEquipZoneFacilityManager, equipZone, Facility.FacilityType.ETC);
            }

            return group;
        }

        private FacilityManagerGroup SetEquipZoneFacilityManagerGroup(Dictionary<Facility.FacilityType, Dictionary<int, FacilityManagerGroup>> dicEquipZoneFacilityManager, EquipmentZone equipZone, Facility.FacilityType sensorType)
        {
            FacilityManagerGroup group = null;

            if (dicEquipZoneFacilityManager.ContainsKey(sensorType))
            {
                Dictionary<int, FacilityManagerGroup> dicManagers = dicEquipZoneFacilityManager[sensorType];

                if (dicManagers.ContainsKey(equipZone.ID))
                    group = dicManagers[equipZone.ID];
                else
                {
                    group = new FacilityManagerGroup();
                    group.EquipZone = equipZone;

                    dicManagers[equipZone.ID] = group;
                }
            }
            else
            {
                Dictionary<int, FacilityManagerGroup> dicManagers = new Dictionary<int, FacilityManagerGroup>();
                group = new FacilityManagerGroup();
                group.EquipZone = equipZone;
                dicManagers[equipZone.ID] = group;

                dicEquipZoneFacilityManager[sensorType] = dicManagers;
            }

            return group;
        }

        private void LoadBuildingNOutdoorFacilityManager(bool isDetectTime)
        {
            Dictionary<BuildingFacilityManager.Fields, object> dicConditions = new Dictionary<BuildingFacilityManager.Fields, object>();
            dicConditions[BuildingFacilityManager.Fields.DetectType] = isDetectTime ? (int)FacilityManager.DetectTypes.Detect : (int)FacilityManager.DetectTypes.Report;
            dicConditions[BuildingFacilityManager.Fields.SiteID] = m_mainManager.SDMSDataManager.SiteID;

            string strErrorMessage;
            List<BuildingFacilityManager> managers = m_mainManager.SDMSDataManager.GetSelectManager().SelectBuildingFacilityManagers(dicConditions, null, out strErrorMessage);

            if (managers == null)
                return;

            FacilityManagerGroup group = null;

            foreach (BuildingFacilityManager mgr in managers)
            {
                if (mgr.BuildingID > 0)
                {
                    Building building = m_mainManager.SensorManager.GetBuilding(mgr.BuildingID);

                    if (building == null)
                        continue;

                    group = GetBuildingFacilityManagerGroup(mgr.FacilityType, building, isDetectTime);
                }
                else if (mgr.BuildingID < 0)
                {
                    Zone zone = m_mainManager.SensorManager.GetZone(-mgr.BuildingID);

                    if (zone == null)
                        continue;

                    group = GetOutdoorFacilityManagerGroup(mgr.FacilityType, zone, isDetectTime);
                }
                else
                    continue;

                if (group == null)
                    continue;

                AddFacilityManager(mgr.ID, mgr.MemberID, mgr.MemberType, mgr.FacilityType, mgr.DetectType, mgr.Description, group);
            }
        }

        public FacilityManagerGroup GetOutdoorFacilityManagerGroup(int nFacilityType, Zone zone, bool isDetectTime)
        {
            Dictionary<Facility.FacilityType, Dictionary<Zone, FacilityManagerGroup>> dicOutdoorFacilityManager = isDetectTime ? m_dicOutdoorFacilityManager : m_dicOutdoorFacilityManagerReport;
            FacilityManagerGroup group = null;

            Facility.FacilityType sensorType = (Facility.FacilityType)nFacilityType;

            if (BaseBroadcastManager.IsFireSensor(sensorType))
            {
                group = SetOutdoorFacilityManagerGroup(dicOutdoorFacilityManager, zone, Facility.FacilityType.FIRE_SENSOR);
            }
            else if (BaseBroadcastManager.IsSecuritySensor(sensorType))
            {
                group = SetOutdoorFacilityManagerGroup(dicOutdoorFacilityManager, zone, Facility.FacilityType.Security_Sensor);
            }
            else if (BaseBroadcastManager.IsPSMSensor(sensorType))
            {
                group = SetOutdoorFacilityManagerGroup(dicOutdoorFacilityManager, zone, Facility.FacilityType.PSM_SENSOR);
            }
            else if (BaseBroadcastManager.IsEarthquakeSensor(sensorType))
            {
                group = SetOutdoorFacilityManagerGroup(dicOutdoorFacilityManager, zone, Facility.FacilityType.Earthquake);
            }
            else if (BaseBroadcastManager.IsETCSensor(sensorType))
            {
                group = SetOutdoorFacilityManagerGroup(dicOutdoorFacilityManager, zone, Facility.FacilityType.ETC);
            }

            return group;
        }

        private FacilityManagerGroup SetOutdoorFacilityManagerGroup(Dictionary<Facility.FacilityType, Dictionary<Zone, FacilityManagerGroup>> dicOutdoorFacilityManager, Zone zone, Facility.FacilityType sensorType)
        {
            FacilityManagerGroup group = null;

            if (dicOutdoorFacilityManager.ContainsKey(sensorType))
            {
                Dictionary<Zone, FacilityManagerGroup> dicManagers = dicOutdoorFacilityManager[sensorType];

                if (dicManagers.ContainsKey(zone))
                    group = dicManagers[zone];
                else
                {
                    group = new FacilityManagerGroup();
                    group.Zone = zone;

                    dicManagers[zone] = group;
                }
            }
            else
            {
                Dictionary<Zone, FacilityManagerGroup> dicManagers = new Dictionary<Zone, FacilityManagerGroup>();
                group = new FacilityManagerGroup();
                group.Zone = zone;
                dicManagers[zone] = group;

                dicOutdoorFacilityManager[sensorType] = dicManagers;
            }

            return group;
        }

        // 시설물 타입별 전체 담당자 얻어오기
        public FacilityManagerGroup GetEntireFacilityManagerGroup(Facility.FacilityType type, bool isDetectTime, bool alwaysGet = false)
        {
            Dictionary<Facility.FacilityType, FacilityManagerGroup> dicFacilityManagers = isDetectTime ? m_dicEntireFacilityManagers : m_dicEntireFacilityManagersReport;

            if (dicFacilityManagers.ContainsKey(type))
                return dicFacilityManagers[type];

            if (alwaysGet)
            {
                FacilityManagerGroup group = new FacilityManagerGroup();
                group.Type = type;

                if (BaseBroadcastManager.IsFireSensor(type))
                {
                    dicFacilityManagers[Facility.FacilityType.FIRE_SENSOR] = group;
                }
                else if (BaseBroadcastManager.IsSecuritySensor(type))
                {
                    dicFacilityManagers[Facility.FacilityType.Security_Sensor] = group;
                }
                else if (BaseBroadcastManager.IsPSMSensor(type))
                {
                    dicFacilityManagers[Facility.FacilityType.PSM_SENSOR] = group;
                }
                else if (BaseBroadcastManager.IsEarthquakeSensor(type))
                {
                    dicFacilityManagers[Facility.FacilityType.Earthquake] = group;
                }
                else if (BaseBroadcastManager.IsETCSensor(type))
                {
                    dicFacilityManagers[Facility.FacilityType.ETC] = group;
                }

                return group;
            }

            return null;
        }

        public FacilityManagerGroup GetBuildingFacilityManagerGroup(int nFacilityType, Building building, bool isDetectTime)
        {
            Dictionary<Facility.FacilityType, Dictionary<Building, FacilityManagerGroup>> dicBuildingFacilityManagers = isDetectTime ? m_dicBuildingFacilityManager : m_dicBuildingFacilityManagerReport;
            FacilityManagerGroup group = null;

            Facility.FacilityType sensorType = (Facility.FacilityType)nFacilityType;

            if (BaseBroadcastManager.IsFireSensor(sensorType))
            {
                group = SetBuildingFacilityManagerGroup(dicBuildingFacilityManagers, building, Facility.FacilityType.FIRE_SENSOR);
            }
            else if (BaseBroadcastManager.IsSecuritySensor(sensorType))
            {
                group = SetBuildingFacilityManagerGroup(dicBuildingFacilityManagers, building, Facility.FacilityType.Security_Sensor);
            }
            else if (BaseBroadcastManager.IsPSMSensor(sensorType))
            {
                group = SetBuildingFacilityManagerGroup(dicBuildingFacilityManagers, building, Facility.FacilityType.PSM_SENSOR);
            }
            else if (BaseBroadcastManager.IsEarthquakeSensor(sensorType))
            {
                group = SetBuildingFacilityManagerGroup(dicBuildingFacilityManagers, building, Facility.FacilityType.Earthquake);
            }
            else if (BaseBroadcastManager.IsETCSensor(sensorType))
            {
                group = SetBuildingFacilityManagerGroup(dicBuildingFacilityManagers, building, Facility.FacilityType.ETC);
            }

            return group;
        }

        private FacilityManagerGroup SetBuildingFacilityManagerGroup(Dictionary<Facility.FacilityType, Dictionary<Building, FacilityManagerGroup>> dicBuildingFacilityManager, Building building, Facility.FacilityType sensorType)
        {
            FacilityManagerGroup group = null;

            if (dicBuildingFacilityManager.ContainsKey(sensorType))
            {
                Dictionary<Building, FacilityManagerGroup> dicManagers = dicBuildingFacilityManager[sensorType];

                if (dicManagers.ContainsKey(building))
                    group = dicManagers[building];
                else
                {
                    group = new FacilityManagerGroup();
                    group.Building = building;

                    dicManagers[building] = group;
                }
            }
            else
            {
                Dictionary<Building, FacilityManagerGroup> dicManagers = new Dictionary<Building, FacilityManagerGroup>();
                group = new FacilityManagerGroup();
                group.Building = building;
                dicManagers[building] = group;

                dicBuildingFacilityManager[sensorType] = dicManagers;
            }

            return group;
        }

        private void LoadFacilityManager(bool isDetectTime)
        {
            Dictionary<FacilityManager.Fields, object> dicConditions = new Dictionary<FacilityManager.Fields, object>();
            dicConditions[FacilityManager.Fields.DetectType] = isDetectTime ? (int)FacilityManager.DetectTypes.Detect : (int)FacilityManager.DetectTypes.Report;
            dicConditions[FacilityManager.Fields.SiteID] = m_mainManager.SDMSDataManager.SiteID;

            string strErrorMessage;
            List<FacilityManager> managers = m_mainManager.SDMSDataManager.GetSelectManager().SelectFacilityManagers(dicConditions, null, out strErrorMessage);

            if (managers == null)
                return;

            Dictionary<Facility.FacilityType, FacilityManagerGroup> dicEntireFacilityManagers = isDetectTime ? m_dicEntireFacilityManagers : m_dicEntireFacilityManagersReport;

            foreach (FacilityManager mgr in managers)
            {
                FacilityManagerGroup group = GetFacilityManagerGroup(mgr.FacilityType, dicEntireFacilityManagers);
                if (group == null)
                    continue;

                AddFacilityManager(mgr.ID, mgr.MemberID, mgr.MemberType, mgr.FacilityType, mgr.DetectType, mgr.Description, group);
            }
        }

        private void AddFacilityManager(int nID, int nMemberID, int nMemberType, int nFacilityType, int nDetectType, string strDescription, FacilityManagerGroup group)
        {
            FacilityManagerEx mgr = new FacilityManagerEx();
            mgr.ID = nID;
            mgr.MemberID = nMemberID;
            mgr.MemberType = nMemberType;
            mgr.FacilityType = nFacilityType;
            mgr.DetectType = nDetectType;
            mgr.Description = strDescription;
            mgr.SiteID = m_mainManager.SDMSDataManager.SiteID;

            if (nMemberType == (int)TemporaryMemberData.MemberType.RegularMember)
            {
                if (!m_dicRegularMembers.ContainsKey(nMemberID))
                    return;

                RegularMember member = m_dicRegularMembers[nMemberID];
                mgr.Tag = member;
                group.CompanyMembers.Add(mgr);
            }
            else if (nMemberType == (int)TemporaryMemberData.MemberType.RegularTeam)
            {
                if (!m_dicRegularTeams.ContainsKey(nMemberID))
                    return;

                Regular team = m_dicRegularTeams[nMemberID];
                mgr.Tag = team;
                group.RegularTeams.Add(mgr);
            }
        }

        private FacilityManagerGroup GetFacilityManagerGroup(int nFacilityType, Dictionary<Facility.FacilityType, FacilityManagerGroup> dicFacilityManagers)
        {
            FacilityManagerGroup group = null;
            Facility.FacilityType sensorType = (Facility.FacilityType)nFacilityType;

            if (BaseBroadcastManager.IsFireSensor((Facility.FacilityType)nFacilityType))
            {
                group = SetFacilityManagerGroup(dicFacilityManagers, Facility.FacilityType.FIRE_SENSOR);
            }
            else if (BaseBroadcastManager.IsSecuritySensor(sensorType))
            {
                group = SetFacilityManagerGroup(dicFacilityManagers, Facility.FacilityType.Security_Sensor);
            }
            else if (BaseBroadcastManager.IsPSMSensor(sensorType))
            {
                group = SetFacilityManagerGroup(dicFacilityManagers, Facility.FacilityType.PSM_SENSOR);
            }
            else if (BaseBroadcastManager.IsEarthquakeSensor(sensorType))
            {
                group = SetFacilityManagerGroup(dicFacilityManagers, Facility.FacilityType.Earthquake);
            }
            else if (BaseBroadcastManager.IsETCSensor(sensorType))
            {
                group = SetFacilityManagerGroup(dicFacilityManagers, Facility.FacilityType.ETC);
            }

            return group;
        }

        private FacilityManagerGroup SetFacilityManagerGroup(Dictionary<Facility.FacilityType, FacilityManagerGroup> dicFacilityManagers, Facility.FacilityType sensorType)
        {
            FacilityManagerGroup group = null;

            if (dicFacilityManagers.ContainsKey(sensorType))
                group = dicFacilityManagers[sensorType];
            else
            {
                group = new FacilityManagerGroup();
                group.Type = sensorType;
                dicFacilityManagers[sensorType] = group;
            }

            return group;
        }

        private bool LoadRegularMemberList(Dictionary<int, Regular> dicTeams, Dictionary<int, RegularMember> dicMembers)
        {
            Regular team;
            List<RegularMember> regularMembers = null;

            foreach (KeyValuePair<int, RegularMember> pair in dicMembers)
            {
                if (dicTeams.TryGetValue(pair.Value.RegularID, out team) == false)
                    continue;

                if (m_dicRegularTeamMembers.TryGetValue(team, out regularMembers) == false)
                {
                    regularMembers = new List<RegularMember>();
                    m_dicRegularTeamMembers[team] = regularMembers;
                }

                regularMembers.Add(pair.Value);
                m_dicRegularMembers[pair.Value.ID] = pair.Value;
            }

            foreach (KeyValuePair<Regular, List<RegularMember>> pair in m_dicRegularTeamMembers)
            {
                pair.Value.Sort();
            }

            return true;
        }

        private bool LoadRegularMember(Dictionary<int, RegularMember> members)
        {
            string strErrorMessage;
            List<RegularMember> regularMembers = m_mainManager.TeamDataManager.GetSelectManager().SelectRegularMembers(out strErrorMessage);

            if (regularMembers == null)
                return false;

            foreach (RegularMember member in regularMembers)
            {
                members[member.ID] = member;
            }

            return true;
        }

        private string ValidPhoneNumber(string strPhoneNumber)
        {
            string strResult = "";
            int nLen = strPhoneNumber.Length;

            // 공백문자나 '-' 등의 기호를 제거한다.
            for (int i = 0; i < nLen; i++)
            {
                char ch = strPhoneNumber[i];

                if (ch != ' ' && ch != '\t' && ch != '-')
                    strResult += ch;
            }

            int nLen2 = strResult.Length;

            // 숫자 이외의 기호가 들어있으면 잘못된 전화번호다.
            for (int i = 0; i < nLen2; i++)
            {
                char ch = strResult[i];

                if (ch < '0' || ch > '9')
                    return "";
            }

            return strResult;
        }

        // dicTeams : ID별 Team
        private Regular LoadRegularTeam(Dictionary<int, Regular> dicTeams)
        {
            // Site별로 사용할 수 있도록 수정 , Edit by skkim 2015.01.14
            // SiteID로 본부 아이디를 가져온다.
            string strErrorMessage;
            Site site = m_mainManager.CommonDataManager.GetSelectManager().SelectSite(m_mainManager.CommonDataManager.SiteID, out strErrorMessage);

            if (site == null || site.TeamID == null)
                return null;

            Regular teamRoot = ExecuteTeamList(m_mainManager.TeamDataManager, (int)site.TeamID, dicTeams);
            return teamRoot;
        }

        // Return값 : RootTeam
        public static Regular ExecuteTeamList(TeamEditor.IDAL.IDataManager dataManager, int nRootTeamID, Dictionary<int, Regular> dicTeams)
        {
            string strErrorMessage;
            List<Regular> teams = dataManager.GetSelectManager().SelectRegulars(out strErrorMessage);

            if (teams == null)
                return null;

            Dictionary<int, Regular> dicAllTeams = new Dictionary<int, Regular>();
            Dictionary<Regular, List<Regular>> dicChildTeams = new Dictionary<Regular, List<Regular>>();

            foreach (Regular team in teams)
            {
                dicAllTeams[team.ID] = team;
                dicChildTeams[team] = new List<Regular>();
            }

            Regular parentTeam, rootTeam = null;
            List<Regular> teamList;

            foreach (Regular team in teams)
            {
                if (team.ID == nRootTeamID)
                    rootTeam = team;

                if (team.ParentTeamID != null)
                {
                    if (dicAllTeams.TryGetValue((int)team.ParentTeamID, out parentTeam) == false)
                        continue;

                    if (dicChildTeams.TryGetValue(parentTeam, out teamList) == false)
                        continue;

                    teamList.Add(team);
                }
            }

            if (rootTeam == null)
                return null;

            SetChildTeams(rootTeam, dicTeams, dicChildTeams);
            return rootTeam;
        }

        private static void SetChildTeams(Regular teamParent, Dictionary<int, Regular> dicTeams, Dictionary<Regular, List<Regular>> dicChildTeams)
        {
            dicTeams[teamParent.ID] = teamParent;

            List<Regular> childTeams = null;

            if (dicChildTeams.TryGetValue(teamParent, out childTeams) == false)
                return;

            foreach (Regular childTeam in childTeams)
            {
                SetChildTeams(childTeam, dicTeams, dicChildTeams);
            }
        }

        // EquipZone별 시설물 담당자 얻어오기
        public FacilityManagerGroup GetEquipZoneFacilityManagerGroup(Facility.FacilityType type, EquipmentZone zone, bool isDetectTime, bool alwaysGet = false)
        {
            Dictionary<Facility.FacilityType, Dictionary<int, FacilityManagerGroup>> dicEquipZoneFacilityManager = isDetectTime ? m_dicEquipZoneFacilityManager : m_dicEquipZoneFacilityManagerReport;
            if (zone == null)
                return null;

            if (dicEquipZoneFacilityManager.ContainsKey(type))
            {
                Dictionary<int, FacilityManagerGroup> dicManagers = dicEquipZoneFacilityManager[type];

                if (dicManagers.ContainsKey(zone.ID))
                    return dicManagers[zone.ID];

                if (alwaysGet)
                {
                    FacilityManagerGroup group = new FacilityManagerGroup();
                    group.Type = type;
                    group.EquipZone = zone;

                    dicManagers[zone.ID] = group;
                    return group;
                }
            }

            if (alwaysGet)
            {
                Dictionary<int, FacilityManagerGroup> dicManagers = new Dictionary<int, FacilityManagerGroup>();

                if (BaseBroadcastManager.IsFireSensor(type))
                {
                    dicEquipZoneFacilityManager[Facility.FacilityType.FIRE_SENSOR] = dicManagers;
                }
                else if (BaseBroadcastManager.IsSecuritySensor(type))
                {
                    dicEquipZoneFacilityManager[Facility.FacilityType.Security_Sensor] = dicManagers;
                }
                else if (BaseBroadcastManager.IsPSMSensor(type))
                {
                    dicEquipZoneFacilityManager[Facility.FacilityType.PSM_SENSOR] = dicManagers;
                }
                else if (BaseBroadcastManager.IsEarthquakeSensor(type))
                {
                    dicEquipZoneFacilityManager[Facility.FacilityType.Earthquake] = dicManagers;
                }
                else if (BaseBroadcastManager.IsETCSensor(type))
                {
                    dicEquipZoneFacilityManager[Facility.FacilityType.ETC] = dicManagers;
                }

                FacilityManagerGroup group = new FacilityManagerGroup();
                group.Type = type;
                group.EquipZone = zone;

                dicManagers[zone.ID] = group;
                return group;
            }

            return null;
        }

        public List<RegularMember> GetRegularTeamMembers(Regular team)
        {
            List<RegularMember> members;

            if (m_dicRegularTeamMembers.TryGetValue(team, out members))
                return members;

            return null;
        }

        public RegularMember GetRegularMember(int nMemberID)
        {
            RegularMember member;

            if (m_dicRegularMembers.TryGetValue(nMemberID, out member))
                return member;

            return null;
        }

        public ICollection<RegularMember> GetAllRegularMember()
        {
            return m_dicRegularMembers.Values;
        }

        public void AddAllRegularMemberPhoneNumbers(Dictionary<string, string> dicPhoneNumbers, Dictionary<int, int> dicRegularMemberIDs)
        {
            foreach (KeyValuePair<int, RegularMember> pair in m_dicRegularMembers)
            {
                dicPhoneNumbers[pair.Value.PhoneNumber] = pair.Value.PhoneNumber;
                dicRegularMemberIDs[pair.Key] = pair.Key;
            }
        }

        public void ReloadRegularMembers()
        {
            m_dicRegularTeams.Clear();
            m_dicRegularTeamMembers.Clear();
            m_dicRegularMembers.Clear();

            m_teamRegularRoot = LoadRegularTeam(m_dicRegularTeams);

            // SiteID를 고려하지 않은 전체 직원 리스트
            Dictionary<int, RegularMember> members = new Dictionary<int, RegularMember>();

            LoadRegularMember(members);
            LoadRegularMemberList(m_dicRegularTeams, members);
        }
    }
}
