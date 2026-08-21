using System;
using System.Collections;
using System.Collections.Generic;
using TeamEditor.Model.Sop.Team;
using dnsData.Sensor;
using SDMS.Model.Spatial;
using SDMS.Model.Sensor;
using AgentFactory.BLL;
using TeamEditor.BLL;
using Common.Model;
using UnE.Geometry;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace SafetyServer.BLL.Process
{
    using Data.Models;
    using Data.Response;
    using Data.Request;

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

        public MessageResult SetLoginEvent(string strMemberID, bool login)
        {
            if (strMemberID == null || strMemberID.Length == 0)
            {
                return new MessageResult(false, "id is empty");
            }

            bool isNullable;

            string strCondition = string.Format("{0} = '{1}'",
                RegularMember.GetFieldName(RegularMember.Fields.MemberID, out isNullable),
                strMemberID);

            string strErrorMessage;
            List<RegularMember> members = m_mainManager.TeamDataManager.GetSelectManager().SelectRegularMembers(strCondition, out strErrorMessage);

            if (members == null)
                return new MessageResult(false, strErrorMessage);

            if (members.Count == 0)
            {
                InsertRegularMemberAsync(strMemberID, login);
                //return new MessageResult(false, string.Format("{0}에 해당하는 사용자 정보를 찾을수 없습니다.", strMemberID));
            }
            else
            {
                UpdateRegularMemberAsync(strMemberID, login, members[0]);
            }

            return new MessageResult(true, "");
        }

        private void InsertRegularMemberAsync(string strMemberID, bool login)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(strMemberID);
            arrDatas.Add(login);

            Thread t = new Thread(new ParameterizedThreadStart(InsertRegularMemberAsyncThread));
            t.Start(arrDatas);
        }

        private void InsertRegularMemberAsyncThread(object param)
        {
            ArrayList arrDatas = (ArrayList)param;
            string strMemberID = (string)arrDatas[0];
            bool login = (bool)arrDatas[1];

            string strMemberName, strTeamName, strJobLevel;
            bool loginStatus;

            if (RequestRegularMemberInfo(strMemberID, out strMemberName, out strTeamName, out strJobLevel, out loginStatus) == false)
                return;

            Regular team = GetRegularTeam(strTeamName);

            if (team == null)
                return;

            int nJobLevelID = GetJobLevelID(strJobLevel);

            if (nJobLevelID < 0)
                return;

            string strErrorMessage;
            RegisterRegularMember(strMemberID, team.ID, strMemberName, nJobLevelID, loginStatus, out strErrorMessage);
        }

        private void UpdateRegularMemberAsync(string strMemberID, bool login, RegularMember member)
        {
            string strMemberName, strTeamName, strJobLevel;
            bool loginStatus;

            if (RequestRegularMemberInfo(strMemberID, out strMemberName, out strTeamName, out strJobLevel, out loginStatus) == false)
                return;

            Regular team = GetRegularTeam(strTeamName);

            if (team == null)
                return;

            int nJobLevelID = GetJobLevelID(strJobLevel);

            if (nJobLevelID < 0)
                return;

            SetLoginStatus(member, loginStatus);
            member.JobLevelID = nJobLevelID;
            member.MemberName = strMemberName;
            member.RegularID = team.ID;

            string strErrorMessage;
            m_mainManager.TeamDataManager.GetUpdateManager().UpdateRegularMember(member, out strErrorMessage);
        }

        private bool RequestRegularMemberInfo(string strMemberID, out string strName, out string strTeamName, out string strPosition, out bool loginStatus)
        {
            strName = strTeamName = strPosition = null;
            loginStatus = false;

            NetvisionManager netvisionManager = new NetvisionManager();
            JObject json = netvisionManager.SendRequestUserInfo(strMemberID);

            if (json == null)
                return false;

            Dictionary<string, string> dicValues = new Dictionary<string, string>();
            JToken token = json.First;

            while (token != null)
            {
                string strPropertyName = token.Path;
                JToken data = json[strPropertyName];

                if (data.Type == JTokenType.String)
                {
                    string strValue = (string)data;
                    dicValues[strPropertyName.ToLower()] = strValue;
                }
                else if (data.Type == JTokenType.Null)
                {
                    dicValues[strPropertyName.ToLower()] = null;
                }
                else if (data.Type == JTokenType.Boolean)
                {
                    string strValue = ((bool)data).ToString();
                    dicValues[strPropertyName.ToLower()] = strValue;
                }

                token = token.Next;
            }

            string strLoginStatus;

            if (dicValues.TryGetValue("name", out strName) == false)
                return false;

            if (dicValues.TryGetValue("affiliation", out strTeamName) == false)
                return false;

            if (dicValues.TryGetValue("position", out strPosition) == false)
                return false;

            if (dicValues.TryGetValue("loginstatus", out strLoginStatus) == false)
                return false;

            if (strLoginStatus.ToLower() == "true")
                loginStatus = true;
            else
                loginStatus = false;

            return true;
        }

        public MessageResult UpdateUserPosition(UpdateUserPosition data)
        {
            string strErrorMessage = null;

            if (data.UserID == null)
            {
                return new MessageResult(false, "UserID is null");
            }

            bool isNullable;

            string strCondition = string.Format("{0} = '{1}'",
                RegularMember.GetFieldName(RegularMember.Fields.MemberID, out isNullable),
                data.UserID);

            List<RegularMember> members = m_mainManager.TeamDataManager.GetSelectManager().SelectRegularMembers(strCondition, out strErrorMessage);

            if (members == null)
                return new MessageResult(false, strErrorMessage);

            RegularMember member;

            if (members.Count == 0)
            {
                strErrorMessage = string.Format("No one in Database as id = {0}", data.UserID);
                return new MessageResult(false, strErrorMessage);
            }
            else
                member = members[0];

            if (data.FieldID == null)
            {
                strErrorMessage = string.Format("FieldID is null");
                return new MessageResult(false, strErrorMessage);
            }

            if (data.X == null)
            {
                strErrorMessage = string.Format("X is null");
                return new MessageResult(false, strErrorMessage);
            }

            if (data.Y == null)
            {
                strErrorMessage = string.Format("Y is null");
                return new MessageResult(false, strErrorMessage);
            }

            member.Email = string.Format("{0}, {1}, {2}", data.FieldID, data.X, data.Y);
            bool result = m_mainManager.TeamDataManager.GetUpdateManager().UpdateRegularMember(member, out strErrorMessage);

            if (result == false)
                return new MessageResult(false, strErrorMessage);

            return new MessageResult(true, "");
        }

        public ResponseUserPosition GetUserPosition(string strMemberID)
        {
            ResponseUserPosition response = new ResponseUserPosition();
            response.Success = false;
            response.ID = strMemberID;

            if (strMemberID == null || strMemberID.Length == 0)
            {
                response.Message = "id is empty";
                return response;
            }

            bool isNullable;

            string strCondition = string.Format("{0} = '{1}'",
                RegularMember.GetFieldName(RegularMember.Fields.MemberID, out isNullable),
                strMemberID);

            string strErrorMessage;
            List<RegularMember> members = m_mainManager.TeamDataManager.GetSelectManager().SelectRegularMembers(strCondition, out strErrorMessage);

            if (members == null)
            {
                response.Message = strErrorMessage;
                return response;
            }

            RegularMember member;

            if (members.Count == 0)
            {
                /*member = RegisterRegularMember(strMemberID, 1, "Unknown", null, false, out strErrorMessage);

                int memberID = m_mainManager.TeamDataManager.GetSelectManager().GetMaxID(RegularMember.GetTableName(), out strErrorMessage);

                if (strErrorMessage != null && strErrorMessage.Length > 0)
                {
                    response.Message = strErrorMessage;
                    return response;
                }*/

                response.Message = string.Format("No one in Database as id = {0}", strMemberID);
                return response;
            }
            else
                member = members[0];

            /*bool login = member.OfficePhoneNumber != null && member.OfficePhoneNumber == "1" ? true : false;

            if (login == false)
            {
                response.Message = "현재 로그인 되어있지 않습니다.";
                return response;
            }*/

            int nZoneID;
            float x, y;
            response.Success = true;

            if (GetUserPosition(member, out nZoneID, out x, out y) == false)
            {
                response.Message = string.Format("the user({0}) has no position", strMemberID);
                return response;
            }

            response.FieldID = nZoneID;
            response.X = x;
            response.Y = y;
            return response;
        }

        private Regular GetRegularTeam(string strTeamName)
        {
            // 최상위 팀을 부모로 둔다.
            int nParentTeamID = 1;

            Dictionary<Regular.Fields, object> dicConditions = new Dictionary<Regular.Fields, object>();
            dicConditions[Regular.Fields.TeamName] = strTeamName;
            dicConditions[Regular.Fields.ParentTeamID] = nParentTeamID;

            string strErrorMessage;
            List<Regular> teams = m_mainManager.TeamDataManager.GetSelectManager().SelectRegulars(dicConditions, out strErrorMessage);

            if (teams == null)
                return null;

            if (teams.Count > 0)
                return teams[0];

            int nTeamID = m_mainManager.TeamDataManager.GetSelectManager().GetMaxID(Regular.GetTableName(), out strErrorMessage);

            Regular team = new Regular();
            team.ID = nTeamID;
            team.ParentTeamID = nParentTeamID;
            team.TeamName = strTeamName;

            if (m_mainManager.TeamDataManager.GetCreateManager().AddRegular(team))
                return team;

            return null;
        }

        private int GetJobLevelID(string strJobLevelName)
        {
            string strPropertyName = "JobLevel";
            string strCondition = "PropertyName = '" + strPropertyName + "'";

            string strErrorMessage;
            List<Options> options = m_mainManager.TeamDataManager.GetSelectManager().SelectOptions(strCondition, out strErrorMessage);

            if (options == null)
                return -1;

            int maxPropertyID = -1;

            foreach (Options option in options)
            {
                if (option.PropertyValue == strJobLevelName)
                    return option.PropertyID;

                if (option.PropertyID > maxPropertyID)
                    maxPropertyID = option.PropertyID;
            }

            int id = m_mainManager.TeamDataManager.GetSelectManager().GetMaxID("SopTeamOptions", out strErrorMessage);

            Options newOption = new Options();
            newOption.ID = id;
            newOption.PropertyID = maxPropertyID + 1;
            newOption.PropertyName = strPropertyName;
            newOption.PropertyValue = strJobLevelName;

            if (m_mainManager.TeamDataManager.GetCreateManager().AddOptions(newOption))
                return newOption.PropertyID;

            return -1;
        }

        private RegularMember RegisterRegularMember(string strMemberID, int nTeamID, string strMemberName, int? jobLevelID, bool loginStatus, out string strErrorMessage)
        {
            strErrorMessage = null;
            int memberID = m_mainManager.TeamDataManager.GetSelectManager().GetMaxID(RegularMember.GetTableName(), out strErrorMessage);

            if (strErrorMessage != null && strErrorMessage.Length > 0)
            {
                return null;
            }

            RegularMember member = new RegularMember();
            member.ID = memberID;
            member.RegularID = 1;
            member.MemberName = strMemberName;
            member.MemberID = strMemberID;
            SetLoginStatus(member, loginStatus);
            member.JobLevelID = jobLevelID;

            if (m_mainManager.TeamDataManager.GetCreateManager().AddRegularMember(member) == false)
            {
                strErrorMessage = "DB Exception";
                return null;
            }

            return member;
        }

        public static bool GetUserPosition(RegularMember member, out int nZoneID, out float x, out float y)
        {
            // 외부영역 ID
            /*nZoneID = 12;
            x = y = 0;

            string strErrorMessage;
            List<Common.Model.Option.Options> options = m_mainManager.CommonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SDMS, "ZoneImageCoord(outdoor)", out strErrorMessage);

            if (options == null || options.Count == 0)
                return false;

            Common.Model.Option.Options option = options[0];

            if (option.PropertyValue == null)
                return false;

            string[] tokens = option.PropertyValue.Split(',');

            if (tokens.Length < 6)
                return false;

            Vertex2D vTL = null, vBL = null, vBR = null, vPos;

            if (GetRectangleVertex(options[0], out vTL, out vBL, out vBR) == false)
                return false;

            Dictionary<int, Vertex2D> dicMemberPositions = MakeRandomPositions(vTL, vBL, vBR);

            if (dicMemberPositions.TryGetValue(member.ID, out vPos))
            {
                x = (float)vPos.x;
                y = (float)vPos.y;
            }

            return true;*/

            nZoneID = -1;
            x = y = 0;

            if (member.Email == null)
                return false;

            string[] tokens = member.Email.Split(',');

            if (tokens.Length < 3)
                return false;

            if (int.TryParse(tokens[0].Trim(), out nZoneID) == false)
                return false;

            if (float.TryParse(tokens[1].Trim(), out x) == false ||
                float.TryParse(tokens[2].Trim(), out y) == false)
                return false;

            return true;
        }

        private Dictionary<int, Vertex2D> MakeRandomPositions(Vertex2D vTL, Vertex2D vBL, Vertex2D vBR)
        {
            double width = vBL.GetDistance(vBR);
            double height = vTL.GetDistance(vBL);

            double h = height / 11;
            double w = width / 11;
            Dictionary<int, Vertex2D> dicMemberPositions = new Dictionary<int, Vertex2D>();

            Random rand = new Random(0);

            for (int i = 1; i <= 10; i++)
            {
                Vertex2D vLeft = vTL + (vBL - vTL) * i * h / height;

                for (int j = 1; j <= 10; j++)
                {
                    Vertex2D vBottom = vBL + (vBR - vBL) * j * w / width;
                    Vertex2D vTarget = vBottom - vBL + vLeft;

                    int memberID = GetRandomNumber(rand, dicMemberPositions, 1, 100);
                    //int memberID = (i - 1) * 10 + j;
                    dicMemberPositions[memberID] = vTarget;
                }
            }

            return dicMemberPositions;
        }

        private int GetRandomNumber(Random rand, Dictionary<int, Vertex2D> dicMemberPositions, int min, int max)
        {
            int num = rand.Next(min, max);

            if (dicMemberPositions.ContainsKey(num) == false)
                return num;

            for (int i = num + 1; i <= max; i++)
            {
                if (dicMemberPositions.ContainsKey(i) == false)
                    return i;
            }

            for (int i = num - 1; i >= min; i--)
            {
                if (dicMemberPositions.ContainsKey(i) == false)
                    return i;
            }

            return min;
        }

        private bool GetRectangleVertex(Common.Model.Option.Options option, out Vertex2D vTL, out Vertex2D vBL, out Vertex2D vBR)
        {
            vTL = vBL = vBR = null;

            if (option.PropertyValue == null)
                return false;

            string[] tokens = option.PropertyValue.Split(',');

            if (tokens.Length < 6)
                return false;

            float x, y;

            if (float.TryParse(tokens[0].Trim(), out x) && float.TryParse(tokens[1].Trim(), out y))
                vTL = new Vertex2D(x, y);
            else
                return false;

            if (float.TryParse(tokens[2].Trim(), out x) && float.TryParse(tokens[3].Trim(), out y))
                vBL = new Vertex2D(x, y);
            else
                return false;

            if (float.TryParse(tokens[4].Trim(), out x) && float.TryParse(tokens[5].Trim(), out y))
                vBR = new Vertex2D(x, y);
            else
                return false;

            return true;
        }

        public static bool IsLoginStatus(RegularMember member)
        {
            if (member.OfficePhoneNumber == null || member.OfficePhoneNumber != "1")
                return false;

            return true;
        }

        private void SetLoginStatus(RegularMember member, bool loginStatus)
        {
            if (loginStatus)
                member.OfficePhoneNumber = "1";
            else
                member.OfficePhoneNumber = "0";
        }
    }
}
