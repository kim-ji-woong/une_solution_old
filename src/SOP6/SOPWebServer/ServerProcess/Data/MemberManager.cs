using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Spatial;
using UnE.Sensor;
using DBUtility2;
using System.Collections;

namespace ServerProcess.Data
{
    public class MemberManager
    {
        private DataTeam m_teamRegularRoot = null;
        private List<DataTeam> m_listExternalRootTeams = new List<DataTeam>();
        private Dictionary<int, DataTeam> m_dicRegularTeams = new Dictionary<int, DataTeam>();
        private Dictionary<DataTeam, List<DataCompanyMember>> m_dicRegularTeamMembers = new Dictionary<DataTeam, List<DataCompanyMember>>();
        private Dictionary<int, DataCompanyMember> m_dicRegularMembers = new Dictionary<int, DataCompanyMember>();
        private Dictionary<int, DataTeam> m_dicExternalTeams = new Dictionary<int, DataTeam>();
        private Dictionary<DataTeam, List<DataExternalMember>> m_dicExternalTeamMembers = new Dictionary<DataTeam, List<DataExternalMember>>();
        private Dictionary<int, DataExternalMember> m_dicExternalMembers = new Dictionary<int, DataExternalMember>();
        private Dictionary<int, DataTeamControlRoom> m_dicControlRoomTeams = new Dictionary<int, DataTeamControlRoom>();

        // 시설물 타입별 발전소 전체 담당자(재난 탐지시)
        private Dictionary<IFacility.FacilityType, FacilityManagerGroup> m_dicEntireFacilityManagers = new Dictionary<IFacility.FacilityType, FacilityManagerGroup>();
        // 시설물 타입별 발전소 전체 담당자(재난 전파시)
        private Dictionary<IFacility.FacilityType, FacilityManagerGroup> m_dicEntireFacilityManagersReport = new Dictionary<IFacility.FacilityType, FacilityManagerGroup>();
        // 건물별 시설물 담당자(재난 탐지시)
        private Dictionary<IFacility.FacilityType, Dictionary<Building, FacilityManagerGroup>> m_dicBuildingFacilityManager = new Dictionary<IFacility.FacilityType, Dictionary<Building, FacilityManagerGroup>>();
        // 건물별 시설물 담당자(재난 전파시)
        private Dictionary<IFacility.FacilityType, Dictionary<Building, FacilityManagerGroup>> m_dicBuildingFacilityManagerReport = new Dictionary<IFacility.FacilityType, Dictionary<Building, FacilityManagerGroup>>();
        // 외부 Zone별 시설물 담당자(재난 탐지시)
        private Dictionary<IFacility.FacilityType, Dictionary<Zone, FacilityManagerGroup>> m_dicOutdoorFacilityManager = new Dictionary<IFacility.FacilityType, Dictionary<Zone, FacilityManagerGroup>>();
        // 외부 Zone별 시설물 담당자(재난 전파시)
        private Dictionary<IFacility.FacilityType, Dictionary<Zone, FacilityManagerGroup>> m_dicOutdoorFacilityManagerReport = new Dictionary<IFacility.FacilityType, Dictionary<Zone, FacilityManagerGroup>>();
        // EquipZone 별 시설물 담당자(재난 탐지시)
        private Dictionary<IFacility.FacilityType, Dictionary<int, FacilityManagerGroup>> m_dicEquipZoneFacilityManager = new Dictionary<IFacility.FacilityType, Dictionary<int, FacilityManagerGroup>>();
        // EquipZone 별 시설물 담당자(재난 전파시)
        private Dictionary<IFacility.FacilityType, Dictionary<int, FacilityManagerGroup>> m_dicEquipZoneFacilityManagerReport = new Dictionary<IFacility.FacilityType, Dictionary<int, FacilityManagerGroup>>();

        // 재난전파시 담당자를 따로 지정하여 사용하는가?
        // 이 값이 false이면 재난 전파시 전직원에게 문자메시지를 발송한다.
        // [2017-06-06] 김지웅
        private bool m_useReportFacilityManagers = false;

        private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        private static MemberManager m_instance = new MemberManager();

        // Team이나 직원정보, 담당자 정보를 바꾸거나 조회하는 중인가?
        private object m_memberCriticalSection = new object();

        public static MemberManager Instance
        {
            get { return m_instance; }
        }

        public bool UseReportFacilityManagers
        {
            get { return m_useReportFacilityManagers; }
        }

        public object MemberCriticalSection
        {
            get { return m_memberCriticalSection; }
        }

        private MemberManager()
        {
        }

        public void Initialize(DirectDBManager dbMgr)
        {
            m_teamRegularRoot = LoadRegularTeam(dbMgr, m_dicRegularTeams);
            m_listExternalRootTeams = LoadExternalTeam(dbMgr, m_dicExternalTeams);

            // SiteID를 고려하지 않은 전체 직원 리스트
            Dictionary<int, DataCompanyMember> members = new Dictionary<int, DataCompanyMember>();
            // SiteID를 고려하지 않은 전체 협력업체 직원 리스트
            Dictionary<int, DataExternalMember> externalMembers = new Dictionary<int, DataExternalMember>();

            LoadCompanyMember(dbMgr, members);
            LoadExternalMember(dbMgr, externalMembers);

            LoadRegularMemberList(dbMgr, m_dicRegularTeams, members);
            LoadExternalMemberList(dbMgr, m_dicExternalTeams, externalMembers);
            LoadControlRoomTeams(dbMgr, m_dicControlRoomTeams);
        }

        public void LoadFacilityManager(DirectDBManager dbMgr)
        {
            m_dicEntireFacilityManagers.Clear();
            m_dicBuildingFacilityManager.Clear();
            m_dicOutdoorFacilityManager.Clear();
            m_dicEquipZoneFacilityManager.Clear();

            m_dicEntireFacilityManagersReport.Clear();
            m_dicBuildingFacilityManagerReport.Clear();
            m_dicOutdoorFacilityManagerReport.Clear();
            m_dicEquipZoneFacilityManagerReport.Clear();

            m_useReportFacilityManagers = UseFacilityManagerType(dbMgr);

            LoadFacilityManager(dbMgr, true);
            LoadBuildingNOutdoorFacilityManager(dbMgr, true);
            LoadEquipZoneFacilityManager(dbMgr, true);

            if (m_useReportFacilityManagers)
            {
                LoadFacilityManager(dbMgr, false);
                LoadBuildingNOutdoorFacilityManager(dbMgr, false);
                LoadEquipZoneFacilityManager(dbMgr, false);
            }
        }

        private bool UseFacilityManagerType(DirectDBManager dbMgr)
        {
            string strSQL = "Select PropertyValue from OptionSDMS where PropertyName = 'UseFacilityManagerType' and SiteID = " + dbMgr.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            string strValue = WebDBManager.GetStringField(arrResult[0]);

            if (strValue == null)
                return false;

            strValue = strValue.Trim();

            if (strValue == "1" || string.Compare(strValue, "true", true) == 0)
            {
                return true;
            }

            return false;
        }

        private void LoadEquipZoneFacilityManager(DirectDBManager dbMgr, bool isDetectTime)
        {
            string strTableName = isDetectTime ? "EquipZoneFacilityManager" : "EquipZoneFacilityManagerReport";
            string szText = "select id, MemberID, MemberType, FacilityType, LevelLimit, EquipZoneID, UpperLimit, Description " +
                      " from {1} WHERE SiteID = {0} order by FacilityType";
            string strSQL = string.Format(szText, dbMgr.SiteID, strTableName);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;
            for (int i = 0; i < nResultCount - 7; i += 8)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nFacilityType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nLevelLimit = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nEquipZoneID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0);
                int nUseUpper = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);
                string strDescription = WebDBManager.GetStringField(arrResult[i + 7], "");

                if (nID < 0 || nMemberID < 0)
                    continue;

                if (nEquipZoneID == 0)
                    continue;

                FacilityManagerGroup group = null;

                if (nEquipZoneID > 0)
                {
                    if (!ZoneManager.Instance.DicEquipZones.ContainsKey(nEquipZoneID))
                        continue;

                    EquipmentZone zone = ZoneManager.Instance.DicEquipZones[nEquipZoneID];
                    group = GetEquipZoneFacilityManagerGroup(nFacilityType, zone, isDetectTime);
                }

                if (group == null)
                    continue;

                AddFacilityManager(dbMgr, nID, nMemberID, nMemberType, nFacilityType, nLevelLimit, nUseUpper, strDescription, group);
            }
        }

        private FacilityManagerGroup GetEquipZoneFacilityManagerGroup(int nFacilityType, EquipmentZone zone, bool isDetectTime)
        {
            Dictionary<IFacility.FacilityType, Dictionary<int, FacilityManagerGroup>> dicEquipZoneFacilityManager = isDetectTime ? m_dicEquipZoneFacilityManager : m_dicEquipZoneFacilityManagerReport;
            FacilityManagerGroup group = null;

            if (nFacilityType >= 0 && nFacilityType <= 2)
            {
                IFacility.FacilityType typeFire = IFacility.FacilityType.FIRE_SENSOR;

                if (dicEquipZoneFacilityManager.ContainsKey(typeFire))
                {
                    Dictionary<int, FacilityManagerGroup> dicManagers = dicEquipZoneFacilityManager[typeFire];

                    if (dicManagers.ContainsKey(zone.ID))
                        group = dicManagers[zone.ID];
                    else
                    {
                        group = new FacilityManagerGroup();
                        group.EquipZone = zone;

                        dicManagers[zone.ID] = group;
                    }
                }
                else
                {
                    Dictionary<int, FacilityManagerGroup> dicManagers = new Dictionary<int, FacilityManagerGroup>();
                    group = new FacilityManagerGroup();
                    group.EquipZone = zone;
                    dicManagers[zone.ID] = group;

                    dicEquipZoneFacilityManager[typeFire] = dicManagers;
                    dicEquipZoneFacilityManager[IFacility.FacilityType.COOLER_SENSOR] = dicManagers;
                    dicEquipZoneFacilityManager[IFacility.FacilityType.PRESSURE_SENSOR] = dicManagers;
                }
            }
            else if (nFacilityType == 3)
            {
                IFacility.FacilityType type = IFacility.FacilityType.CCTV;

                if (dicEquipZoneFacilityManager.ContainsKey(type))
                {
                    Dictionary<int, FacilityManagerGroup> dicManagers = dicEquipZoneFacilityManager[type];

                    if (dicManagers.ContainsKey(zone.ID))
                        group = dicManagers[zone.ID];
                    else
                    {
                        group = new FacilityManagerGroup();
                        group.EquipZone = zone;

                        dicManagers[zone.ID] = group;
                    }
                }
                else
                {
                    Dictionary<int, FacilityManagerGroup> dicManagers = new Dictionary<int, FacilityManagerGroup>();
                    group = new FacilityManagerGroup();
                    group.EquipZone = zone;
                    dicManagers[zone.ID] = group;

                    dicEquipZoneFacilityManager[type] = dicManagers;
                }
            }
            else if (nFacilityType >= 4 && nFacilityType <= 6)
            {
                IFacility.FacilityType typeFE = IFacility.FacilityType.FE;

                if (dicEquipZoneFacilityManager.ContainsKey(typeFE))
                {
                    Dictionary<int, FacilityManagerGroup> dicManagers = dicEquipZoneFacilityManager[typeFE];

                    if (dicManagers.ContainsKey(zone.ID))
                        group = dicManagers[zone.ID];
                    else
                    {
                        group = new FacilityManagerGroup();
                        group.EquipZone = zone;

                        dicManagers[zone.ID] = group;
                    }
                }
                else
                {
                    Dictionary<int, FacilityManagerGroup> dicManagers = new Dictionary<int, FacilityManagerGroup>();
                    group = new FacilityManagerGroup();
                    group.EquipZone = zone;
                    dicManagers[zone.ID] = group;

                    dicEquipZoneFacilityManager[typeFE] = dicManagers;
                    dicEquipZoneFacilityManager[IFacility.FacilityType.HD] = dicManagers;
                    dicEquipZoneFacilityManager[IFacility.FacilityType.FA] = dicManagers;
                }
            }
            else if (nFacilityType == 11)
            {
                IFacility.FacilityType type = IFacility.FacilityType.PSM_SENSOR;

                if (dicEquipZoneFacilityManager.ContainsKey(type))
                {
                    Dictionary<int, FacilityManagerGroup> dicManagers = dicEquipZoneFacilityManager[type];

                    if (dicManagers.ContainsKey(zone.ID))
                        group = dicManagers[zone.ID];
                    else
                    {
                        group = new FacilityManagerGroup();
                        group.EquipZone = zone;

                        dicManagers[zone.ID] = group;
                    }
                }
                else
                {
                    Dictionary<int, FacilityManagerGroup> dicManagers = new Dictionary<int, FacilityManagerGroup>();
                    group = new FacilityManagerGroup();
                    group.EquipZone = zone;
                    dicManagers[zone.ID] = group;

                    dicEquipZoneFacilityManager[type] = dicManagers;
                }
            }
            else if (nFacilityType == (int)IFacility.FacilityType.Security_Sensor)
            {
                IFacility.FacilityType type = IFacility.FacilityType.Security_Sensor;

                if (dicEquipZoneFacilityManager.ContainsKey(type))
                {
                    Dictionary<int, FacilityManagerGroup> dicManagers = dicEquipZoneFacilityManager[type];

                    if (dicManagers.ContainsKey(zone.ID))
                        group = dicManagers[zone.ID];
                    else
                    {
                        group = new FacilityManagerGroup();
                        group.EquipZone = zone;

                        dicManagers[zone.ID] = group;
                    }
                }
                else
                {
                    Dictionary<int, FacilityManagerGroup> dicManagers = new Dictionary<int, FacilityManagerGroup>();
                    group = new FacilityManagerGroup();
                    group.EquipZone = zone;
                    dicManagers[zone.ID] = group;

                    dicEquipZoneFacilityManager[type] = dicManagers;
                }
            }

            return group;
        }

        private void LoadBuildingNOutdoorFacilityManager(DirectDBManager dbMgr, bool isDetectTime)
        {
            string strTableName = isDetectTime ? "BuildingFacilityManager" : "BuildingFacilityManagerReport";
            string szText = "SELECT id, MemberID, MemberType, FacilityType, LevelLimit, BuildingID, Description, UpperLimit " +
                     " FROM {1} WHERE SiteID = {0} order by FacilityType";

            string strSQL = string.Format(szText, dbMgr.SiteID, strTableName);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 7; i += 8)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nFacilityType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nLevelLimit = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nBuildingID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0);
                string strDescription = WebDBManager.GetStringField(arrResult[i + 6], "");
                int nUpperLimit = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);

                if (nID < 0 || nMemberID < 0)
                    continue;

                if (nBuildingID == 0)
                    continue;

                FacilityManagerGroup group = null;

                if (nBuildingID > 0)
                {
                    if (!ZoneManager.Instance.DicBuildings.ContainsKey(nBuildingID))
                        continue;

                    Building building = ZoneManager.Instance.DicBuildings[nBuildingID];
                    group = GetBuildingFacilityManagerGroup(nFacilityType, building, isDetectTime);
                }
                else if (nBuildingID < 0)
                {
                    Zone zone = ZoneManager.Instance.GetZone(-nBuildingID);

                    if (zone == null)
                        continue;

                    group = GetOutdoorFacilityManagerGroup(nFacilityType, zone, isDetectTime);
                }

                if (group == null)
                    continue;

                AddFacilityManager(dbMgr, nID, nMemberID, nMemberType, nFacilityType, nLevelLimit, nUpperLimit, strDescription, group);
            }
        }

        public FacilityManagerGroup GetOutdoorFacilityManagerGroup(int nFacilityType, Zone zone, bool isDetectTime)
        {
            Dictionary<IFacility.FacilityType, Dictionary<Zone, FacilityManagerGroup>> dicOutdoorFacilityManager = isDetectTime ? m_dicOutdoorFacilityManager : m_dicOutdoorFacilityManagerReport;
            FacilityManagerGroup group = null;

            if (nFacilityType >= 0 && nFacilityType <= 2)
            {
                IFacility.FacilityType typeFire = IFacility.FacilityType.FIRE_SENSOR;

                if (dicOutdoorFacilityManager.ContainsKey(typeFire))
                {
                    Dictionary<Zone, FacilityManagerGroup> dicManagers = dicOutdoorFacilityManager[typeFire];

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

                    dicOutdoorFacilityManager[typeFire] = dicManagers;
                    dicOutdoorFacilityManager[IFacility.FacilityType.COOLER_SENSOR] = dicManagers;
                    dicOutdoorFacilityManager[IFacility.FacilityType.PRESSURE_SENSOR] = dicManagers;
                }
            }
            else if (nFacilityType == 3)
            {
                IFacility.FacilityType type = IFacility.FacilityType.CCTV;

                if (dicOutdoorFacilityManager.ContainsKey(type))
                {
                    Dictionary<Zone, FacilityManagerGroup> dicManagers = dicOutdoorFacilityManager[type];

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

                    dicOutdoorFacilityManager[type] = dicManagers;
                }
            }
            else if (nFacilityType >= 4 && nFacilityType <= 6)
            {
                IFacility.FacilityType typeFE = IFacility.FacilityType.FE;

                if (dicOutdoorFacilityManager.ContainsKey(typeFE))
                {
                    Dictionary<Zone, FacilityManagerGroup> dicManagers = dicOutdoorFacilityManager[typeFE];

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

                    dicOutdoorFacilityManager[typeFE] = dicManagers;
                    dicOutdoorFacilityManager[IFacility.FacilityType.HD] = dicManagers;
                    dicOutdoorFacilityManager[IFacility.FacilityType.FA] = dicManagers;
                }
            }
            else if (nFacilityType == (int)IFacility.FacilityType.PSM_SENSOR)
            {
                IFacility.FacilityType type = IFacility.FacilityType.PSM_SENSOR;

                if (dicOutdoorFacilityManager.ContainsKey(type))
                {
                    Dictionary<Zone, FacilityManagerGroup> dicManagers = dicOutdoorFacilityManager[type];

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

                    dicOutdoorFacilityManager[type] = dicManagers;
                }
            }
            else if (nFacilityType == (int)IFacility.FacilityType.Security_Sensor)
            {
                IFacility.FacilityType type = IFacility.FacilityType.Security_Sensor;

                if (dicOutdoorFacilityManager.ContainsKey(type))
                {
                    Dictionary<Zone, FacilityManagerGroup> dicManagers = dicOutdoorFacilityManager[type];

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

                    dicOutdoorFacilityManager[type] = dicManagers;
                }
            }

            return group;
        }

        // 시설물 타입별 발전소 전체 담당자 얻어오기
        public FacilityManagerGroup GetEntireFacilityManagerGroup(IFacility.FacilityType type, bool isDetectTime, bool alwaysGet = false)
        {
            Dictionary<IFacility.FacilityType, FacilityManagerGroup> dicFacilityManagers = isDetectTime ? m_dicEntireFacilityManagers : m_dicEntireFacilityManagersReport;

            if (dicFacilityManagers.ContainsKey(type))
                return dicFacilityManagers[type];

            if (alwaysGet)
            {
                FacilityManagerGroup group = new FacilityManagerGroup();
                group.Type = type;

                if (type == IFacility.FacilityType.FIRE_SENSOR ||
                    type == IFacility.FacilityType.COOLER_SENSOR ||
                    type == IFacility.FacilityType.PRESSURE_SENSOR)
                {
                    dicFacilityManagers[IFacility.FacilityType.FIRE_SENSOR] = group;
                    dicFacilityManagers[IFacility.FacilityType.COOLER_SENSOR] = group;
                    dicFacilityManagers[IFacility.FacilityType.PRESSURE_SENSOR] = group;
                }
                else if (type == IFacility.FacilityType.FE ||
                    type == IFacility.FacilityType.HD ||
                    type == IFacility.FacilityType.FA)
                {
                    dicFacilityManagers[IFacility.FacilityType.FE] = group;
                    dicFacilityManagers[IFacility.FacilityType.HD] = group;
                    dicFacilityManagers[IFacility.FacilityType.FA] = group;
                }
                else
                    dicFacilityManagers[type] = group;

                return group;
            }

            return null;
        }

        public FacilityManagerGroup GetBuildingFacilityManagerGroup(int nFacilityType, Building building, bool isDetectTime)
        {
            Dictionary<IFacility.FacilityType, Dictionary<Building, FacilityManagerGroup>> dicBuildingFacilityManagers = isDetectTime ? m_dicBuildingFacilityManager : m_dicBuildingFacilityManagerReport;
            FacilityManagerGroup group = null;

            if (nFacilityType >= 0 && nFacilityType <= 2)
            {
                IFacility.FacilityType typeFire = IFacility.FacilityType.FIRE_SENSOR;

                if (dicBuildingFacilityManagers.ContainsKey(typeFire))
                {
                    Dictionary<Building, FacilityManagerGroup> dicManagers = dicBuildingFacilityManagers[typeFire];

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

                    dicBuildingFacilityManagers[typeFire] = dicManagers;
                    dicBuildingFacilityManagers[IFacility.FacilityType.COOLER_SENSOR] = dicManagers;
                    dicBuildingFacilityManagers[IFacility.FacilityType.PRESSURE_SENSOR] = dicManagers;
                }
            }
            else if (nFacilityType == 3)
            {
                IFacility.FacilityType type = IFacility.FacilityType.CCTV;

                if (dicBuildingFacilityManagers.ContainsKey(type))
                {
                    Dictionary<Building, FacilityManagerGroup> dicManagers = dicBuildingFacilityManagers[type];

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

                    dicBuildingFacilityManagers[type] = dicManagers;
                }
            }
            else if (nFacilityType >= 4 && nFacilityType <= 6)
            {
                IFacility.FacilityType typeFE = IFacility.FacilityType.FE;

                if (dicBuildingFacilityManagers.ContainsKey(typeFE))
                {
                    Dictionary<Building, FacilityManagerGroup> dicManagers = dicBuildingFacilityManagers[typeFE];

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

                    dicBuildingFacilityManagers[typeFE] = dicManagers;
                    dicBuildingFacilityManagers[IFacility.FacilityType.HD] = dicManagers;
                    dicBuildingFacilityManagers[IFacility.FacilityType.FA] = dicManagers;
                }
            }
            else if (nFacilityType == 11)
            {
                IFacility.FacilityType type = IFacility.FacilityType.PSM_SENSOR;

                if (dicBuildingFacilityManagers.ContainsKey(type))
                {
                    Dictionary<Building, FacilityManagerGroup> dicManagers = dicBuildingFacilityManagers[type];

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

                    dicBuildingFacilityManagers[type] = dicManagers;
                }
            }

            else if (nFacilityType == (int)IFacility.FacilityType.Security_Sensor)
            {
                IFacility.FacilityType type = IFacility.FacilityType.Security_Sensor;

                if (dicBuildingFacilityManagers.ContainsKey(type))
                {
                    Dictionary<Building, FacilityManagerGroup> dicManagers = dicBuildingFacilityManagers[type];

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

                    dicBuildingFacilityManagers[type] = dicManagers;
                }
            }

            return group;
        }

        private void LoadFacilityManager(DirectDBManager dbMgr, bool isDetectTime)
        {
            string strTableName = isDetectTime ? "FacilityManager" : "FacilityManagerReport";
            Dictionary<IFacility.FacilityType, FacilityManagerGroup> dicEntireFacilityManagers = isDetectTime ? m_dicEntireFacilityManagers : m_dicEntireFacilityManagersReport;

            string szText = "SELECT id, MemberID, MemberType, FacilityType, LevelLimit, Description, UpperLimit FROM {1} WHERE SiteID = {0} order by FacilityType";
            string strSQL = string.Format(szText, dbMgr.SiteID, strTableName);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nFacilityType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nLevelLimit = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                string strDescription = WebDBManager.GetStringField(arrResult[i + 5], "");
                int nUppderLimit = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);

                if (nID < 0 || nMemberID < 0)
                    continue;

                FacilityManagerGroup group = GetFacilityManagerGroup(nFacilityType, dicEntireFacilityManagers);
                if (group == null)
                    continue;

                AddFacilityManager(dbMgr, nID, nMemberID, nMemberType, nFacilityType, nLevelLimit, nUppderLimit, strDescription, group);
            }
        }

        private void AddFacilityManager(DirectDBManager dbMgr, int nID, int nMemberID, int nMemberType, int nFacilityType, int nLevelLimit, int nUpperLimit, string strDescription, FacilityManagerGroup group)
        {
            FacilityManager mgr = new FacilityManager();
            mgr.ID = nID;
            mgr.MemberID = nMemberID;
            mgr.MemberType = nMemberType;
            mgr.Type = IFacility.ToFacilityType(nFacilityType);
            mgr.LevelLimit = nLevelLimit;
            mgr.UpperLimit = nUpperLimit;
            mgr.Description = strDescription;

            if (nMemberType == 0)
            {
                if (!m_dicRegularMembers.ContainsKey(nMemberID))
                    return;

                DataCompanyMember member = m_dicRegularMembers[nMemberID];
                mgr.Tag = member;
                group.CompanyMembers.Add(mgr);
            }
            else if (nMemberType == 1)
            {
                if (!m_dicRegularTeams.ContainsKey(nMemberID))
                    return;

                DataTeam team = m_dicRegularTeams[nMemberID];
                mgr.Tag = team;
                group.RegularTeams.Add(mgr);
            }
            else if (nMemberType == 2)
            {
                if (!m_dicExternalMembers.ContainsKey(nMemberID))
                    return;

                DataExternalMember member = m_dicExternalMembers[nMemberID];
                mgr.Tag = member;
                group.ExternalCompanyMembers.Add(mgr);
            }
            else if (nMemberType == 3)
            {
                if (!m_dicExternalTeams.ContainsKey(nMemberID))
                    return;

                DataTeam team = m_dicExternalTeams[nMemberID];
                mgr.Tag = team;
                group.ExternalTeams.Add(mgr);
            }
            else if (nMemberType == 4)
            {
                DataTeam team = GetCompany(m_dicRegularTeams);
                if (team == null)
                    return;

                mgr.Tag = team;
                group.RegularTeams.Add(mgr);
            }
            else if (nMemberType == 5)
            {
                DataTeam team = GetCompany(m_listExternalRootTeams, nMemberID);
                if (team == null)
                    return;

                mgr.Tag = team;
                group.ExternalTeams.Add(mgr);
            }
            else if (nMemberType == 7)
            {
                DataTeamControlRoom team = GetControlRoomTeam(dbMgr, nMemberID);
                mgr.Tag = team;
                group.ControlRoomMembers.Add(mgr);
            }
        }

        private DataTeamControlRoom GetControlRoomTeam(DirectDBManager dbMgr, int nMemberID)
        {
            DataTeamControlRoom team;

            if (m_dicControlRoomTeams.TryGetValue(nMemberID, out team))
                return team;

            int nRoomTypeID, nControlRoomID, nPositionID;
            DataTeamControlRoom.GetParams(nMemberID, out nRoomTypeID, out nControlRoomID, out nPositionID);

            string strSQL = string.Format("select cr.LocationName, crt.TypeName from ControlRoom as cr, ControlRoomType as crt where cr.RoomType = crt.ID and crt.SiteID = {0} and cr.RoomType = {1} and cr.ID = {2}",
                dbMgr.SiteID, nRoomTypeID, nControlRoomID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count != 2)
                return null;

            string strLocationName = WebDBManager.GetStringField(arrResult[0]);
            string strTypeName = WebDBManager.GetStringField(arrResult[1]);

            if (strLocationName == null || strTypeName == null)
                return null;

            DataTeamControlRoom teamParent;
            int nParentTeamID = DataTeamControlRoom.MakeID(nRoomTypeID, nControlRoomID, 0);

            if (!m_dicControlRoomTeams.TryGetValue(nParentTeamID, out teamParent))
            {
                teamParent = new DataTeamControlRoom();
                teamParent.ID = nParentTeamID;

                if (strLocationName == strTypeName)
                    teamParent.TeamName = strLocationName;
                else
                    teamParent.TeamName = strLocationName + " " + strTypeName;

                teamParent.ParentTeam = GetRootControlRoomTeam();
                m_dicControlRoomTeams[nParentTeamID] = teamParent;
            }

            if (nPositionID == 0)
                return teamParent;

            strSQL = "Select JobName from ControlTeamJobPosition where ID = " + nPositionID.ToString();
            arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            string strJobName = WebDBManager.GetStringField(arrResult[0]);

            if (strJobName == null)
                return null;

            team = new DataTeamControlRoom();
            team.ID = nMemberID;
            team.TeamName = strJobName;
            team.ParentTeam = teamParent;

            m_dicControlRoomTeams[team.ID] = team;
            return team;
        }

        private DataTeam GetCompany(List<DataTeam> arrCompanies, int nCompanyID)
        {
            foreach (DataTeam team in arrCompanies)
            {
                if (team.ID == nCompanyID)
                    return team;
            }

            return null;
        }

        private DataTeam GetCompany(Dictionary<int, DataTeam> dicTeams)
        {
            foreach (KeyValuePair<int, DataTeam> pair in dicTeams)
            {
                if (pair.Value.IsCompany)
                    return pair.Value;
            }

            return null;
        }

        private FacilityManagerGroup GetFacilityManagerGroup(int nFacilityType, Dictionary<IFacility.FacilityType, FacilityManagerGroup> dicFacilityManagers)
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
            else// if (nFacilityType == 11)
            {
                IFacility.FacilityType type = IFacility.ToFacilityType(nFacilityType);
                //IFacility.FacilityType type = IFacility.FacilityType.PSM_SENSOR;

                if (dicFacilityManagers.ContainsKey(type))
                    group = dicFacilityManagers[type];
                else
                {
                    group = new FacilityManagerGroup();
                    group.Type = type;
                    dicFacilityManagers[type] = group;
                }
            }
            /*else if (nFacilityType == (int)IFacility.FacilityType.Security_Sensor)
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
            }*/

            return group;
        }

        private bool LoadControlRoomTeams(DirectDBManager dbMgr, Dictionary<int, DataTeamControlRoom> dicTeams)
        {
            dicTeams.Clear();

            string strSQL = "select cr.ID, cr.RoomType, cr.LocationName, crt.TypeName from ControlRoom as cr, ControlRoomType as crt ";
            strSQL += "where cr.RoomType = crt.ID and crt.SiteID = " + dbMgr.SiteID.ToString() + " order by cr.RoomType";

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            DataTeamControlRoom teamRoot = GetRootControlRoomTeam(dicTeams);

            List<int> controlRoomIDs = new List<int>();
            List<int> roomTypeIDs = new List<int>();
            string strRoomTypeIDs = "";

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nControlRoomID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nRoomTypeID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                string strLocationName = WebDBManager.GetStringField(arrResult[i + 2]);
                string strRoomType = WebDBManager.GetStringField(arrResult[i + 3]);

                if (nControlRoomID < 0 || nRoomTypeID < 0 || strLocationName == null || strRoomType == null)
                    continue;

                int nID = DataTeamControlRoom.MakeID(nRoomTypeID, nControlRoomID, 0);

                DataTeamControlRoom team = new DataTeamControlRoom();
                team.ID = nID;
                team.TeamName = strLocationName + " " + strRoomType;
                team.ParentTeam = teamRoot;

                dicTeams[nID] = team;

                if (!roomTypeIDs.Contains(nRoomTypeID))
                {
                    roomTypeIDs.Add(nRoomTypeID);

                    if (strRoomTypeIDs.Length == 0)
                        strRoomTypeIDs = nRoomTypeID.ToString();
                    else
                        strRoomTypeIDs += ", " + nRoomTypeID.ToString();
                }

                if (!controlRoomIDs.Contains(nControlRoomID))
                    controlRoomIDs.Add(nControlRoomID);
            }

            if (roomTypeIDs.Count == 0)
                return true;

            strSQL = string.Format("Select ID, JobName, RoomType from ControlTeamJobPosition where RoomType in ({0})", strRoomTypeIDs);
            arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nPositionID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strJobName = WebDBManager.GetStringField(arrResult[i + 1]);
                int nRoomTypeID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                if (nPositionID < 0 || nRoomTypeID < 0 || strJobName == null)
                    continue;

                foreach (int nControlRoomID in controlRoomIDs)
                {
                    int nID = DataTeamControlRoom.MakeID(nRoomTypeID, nControlRoomID, nPositionID);

                    DataTeamControlRoom team = new DataTeamControlRoom();
                    team.TeamName = strJobName;
                    team.ID = nID;

                    int nParentTeamID = DataTeamControlRoom.MakeID(nRoomTypeID, nControlRoomID, 0);
                    DataTeamControlRoom parentTeam;

                    if (m_dicControlRoomTeams.TryGetValue(nParentTeamID, out parentTeam))
                        team.ParentTeam = parentTeam;

                    dicTeams[nID] = team;
                }
            }

            return true;
        }

        private DataTeamControlRoom GetRootControlRoomTeam(Dictionary<int, DataTeamControlRoom> dicTeams = null)
        {
            if (dicTeams == null)
                dicTeams = m_dicControlRoomTeams;

            DataTeamControlRoom team;
            int nID = DataTeamControlRoom.MakeID(0, 0, 0);

            if (!dicTeams.TryGetValue(nID, out team))
            {
                team = new DataTeamControlRoom();
                team.ID = nID;

                dicTeams[nID] = team;
            }

            return team;
        }

        private bool LoadExternalMemberList(DirectDBManager dbMgr, Dictionary<int, DataTeam> dicTeams, Dictionary<int, DataExternalMember> dicExternalMembers)
        {
            Dictionary<int, string> dicJobLevels = new Dictionary<int, string>();
            Dictionary<int, string> dicJobPositions = new Dictionary<int, string>();

            if (!LoadExternalJobLevel(dbMgr, dicJobLevels))
                return false;

            if (!LoadExternalJobPosition(dbMgr, dicJobPositions))
                return false;

            string strSQL = "select ExternalCompanyTeamID, ExternalCompanyMemberID, JobLevelID, JobPositionID ";
            strSQL += "from ExternalMemberList as eml, ExternalTeam as et ";
            strSQL += "where eml.ExternalCompanyTeamID = et.ID and et.SiteID = " + dbMgr.SiteID.ToString();

            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null)
                return false;

            int nCount = arrResult.Count;
            if (nCount == 0)
                return true;

            DataTeam team;
            DataExternalMember member;
            string strJobLevel, strJobPosition;

            for (int i = 0; i < nCount - 3; i += 4)
            {
                int nTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nJobLevelID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nJobPositionID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);

                // SiteID가 m_nSiteID와 다른 협력업체 직원은 여기서 걸러진다.
                if (!dicTeams.TryGetValue(nTeamID, out team))
                    continue;

                if (!dicExternalMembers.TryGetValue(nMemberID, out member))
                    continue;

                if (nJobLevelID > 0 && dicJobLevels.TryGetValue(nJobLevelID, out strJobLevel))
                    member.JobLevel = strJobLevel;

                if (nJobPositionID > 0 && dicJobPositions.TryGetValue(nJobPositionID, out strJobPosition))
                    member.JobPosition = strJobPosition;

                List<DataExternalMember> arrMembers = null;

                if (m_dicExternalTeamMembers.TryGetValue(team, out arrMembers))
                    arrMembers = m_dicExternalTeamMembers[team];
                else
                {
                    arrMembers = new List<DataExternalMember>();
                    m_dicExternalTeamMembers[team] = arrMembers;
                }

                //member.TeamLeaders[team] = isTeamLeader;

                // dicExternalMembers에는 SiteID가 다른 협력업체 직원들도 포함되어 있는데, m_nSiteID에 해당하는 협력업체 직원들만
                // m_dicExternalMembers에 담는다.
                m_dicExternalMembers[member.ID] = member;
                arrMembers.Add(member);
            }

            return true;
        }

        private bool LoadExternalJobLevel(DirectDBManager dbMgr, Dictionary<int, string> dicJobLevels)
        {
            string strSQL = "select ID, LevelName from ExternalJobLevel";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strLevelName = WebDBManager.GetStringField(arrResult[i + 1], "");

                dicJobLevels[nID] = strLevelName;
            }

            return true;
        }

        private bool LoadExternalJobPosition(DirectDBManager dbMgr, Dictionary<int, string> dicJobPositions)
        {
            string strSQL = "select ID, PositionName from ExternalJobPosition";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strPositionName = WebDBManager.GetStringField(arrResult[i + 1], "");

                dicJobPositions[nID] = strPositionName;
            }

            return true;
        }

        private bool LoadRegularMemberList(DirectDBManager dbMgr, Dictionary<int, DataTeam> dicTeams, Dictionary<int, DataCompanyMember> dicMembers)
        {
            string strSQL = "select RegularTeamID, CompanyMemberID, PositionID from RegularMemberList";

            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null)
                return false;

            int nCount = arrResult.Count;
            if (nCount == 0)
                return true;

            DataTeam team;
            DataCompanyMember member;

            for (int i = 0; i < nCount - 2; i += 3)
            {
                int nTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nPositionID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                // SiteID가 m_nSiteID와 다른 직원은 여기서 걸러진다.
                if (!dicTeams.TryGetValue(nTeamID, out team))
                    continue;

                if (!dicMembers.TryGetValue(nMemberID, out member))
                    continue;

                List<DataCompanyMember> arrMembers = null;

                if (!m_dicRegularTeamMembers.TryGetValue(team, out arrMembers))
                {
                    arrMembers = new List<DataCompanyMember>();
                    m_dicRegularTeamMembers[team] = arrMembers;
                }

                arrMembers.Add(member);
                member.TeamPositions[team] = nPositionID;

                // dicMembers에는 SiteID가 다른 직원들도 포함되어 있는데, m_nSiteID에 해당하는 직원들만
                // m_dicRegularMembers에 담는다.
                m_dicRegularMembers[member.ID] = member;
            }

            foreach (KeyValuePair<DataTeam, List<DataCompanyMember>> pair in m_dicRegularTeamMembers)
            {
                pair.Value.Sort();
            }

            return true;
        }

        private bool LoadExternalMember(DirectDBManager dbMgr, Dictionary<int, DataExternalMember> externalMembers)
        {
            string szSQL = "SELECT ID, Name, PhoneNumber FROM ExternalCompanyMember";
            ArrayList arrResult = dbMgr.GetResultData(szSQL);

            if (arrResult == null)
                return false;

            int nCount = arrResult.Count;
            if (nCount == 0)
                return true;

            for (int i = 0; i < nCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 1], "");
                string szPhoneNumber = WebDBManager.GetStringField(arrResult[i + 2].ToString(), "");

                if (szPhoneNumber == null)
                    szPhoneNumber = "";
                else
                    szPhoneNumber = AES256Cipher.AES_decrypt(szPhoneNumber, key);

                szPhoneNumber = ValidPhoneNumber(szPhoneNumber);

                DataExternalMember data = new DataExternalMember();
                data.ID = nID;
                data.Name = strMemberName;
                data.PhoneNumber = szPhoneNumber;
                
                externalMembers[data.ID] = data;
            }

            return true;
        }

        private bool LoadCompanyMember(DirectDBManager dbMgr, Dictionary<int, DataCompanyMember> members)
        {
            // site ID구분이 없더라도 dicTeams에 RegularTeamID 가 없는 경우 저장되지 않는다.
            // 전체 인원이 많아지는 경우 ReqularTeam에 SiteID를 참조하는 방향을 고려해볼것. skkim 2015.01.14
            string strSQL = "select ID, MemberName, LevelID, MemberID, OfficePhoneNumber, PhoneNumber from CompanyMember";
            
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            for (int i = 0; i < nCount - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nLevelID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                string strMemberID = WebDBManager.GetStringField(arrResult[i + 3], "");
                string strOfficePhoneNumber = WebDBManager.GetStringField(arrResult[i + 4], "");
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 5], "");

                if (nLevelID < 0)
                {
                    // nLevelID가 0보다 작은 직원은 삭제된 직원이다.
                    continue;
                }

                if (string.Compare(strPhoneNumber, "null", true) == 0 || strPhoneNumber == "")
                    strPhoneNumber = "";
                else
                    strPhoneNumber = AES256Cipher.AES_decrypt(strPhoneNumber, key);

                strPhoneNumber = ValidPhoneNumber(strPhoneNumber);

                if (string.Compare(strOfficePhoneNumber, "null", true) == 0)
                    strOfficePhoneNumber = "";

                DataCompanyMember data = new DataCompanyMember();
                data.ID = nID;
                data.MemberName = strMemberName;
                //data.Team = team;
                data.LevelID = nLevelID;
                //data.PositionID = nPositionID;
                data.MemberID = strMemberID;
                data.OfficePhoneNumber = strOfficePhoneNumber;
                data.PhoneNumber = strPhoneNumber;

                members[nID] = data;
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

        private List<DataTeam> LoadExternalTeam(DirectDBManager dbMgr, Dictionary<int, DataTeam> dicTeams)
        {
            List<DataTeam> arrExternalRootTeams = new List<DataTeam>();
            string szText2 = "SELECT et.ID, et.TeamName, et.ParentTeamID " +
                             " FROM ExternalTeam as et WHERE et.SiteID = {0} ";

            string szSQL = string.Format(szText2, dbMgr.SiteID);

            ArrayList arrResult = dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                return null;

            // 자신의 Team, 부모 팀의 ID
            Dictionary<DataTeam, int> dicParentID = new Dictionary<DataTeam, int>();

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string szTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                
                DataTeam data = new DataTeam();
                data.ID = nID;
                data.TeamName = szTeamName;
                data.External = true;

                if (nParentTeamID == -1)
                {
                    data.IsCompany = true;
                    data.CompanyName = szTeamName;

                    if (!arrExternalRootTeams.Contains(data))
                    {
                        arrExternalRootTeams.Add(data);
                    }
                }
                else
                {
                    dicParentID[data] = nParentTeamID;
                }

                dicTeams[nID] = data;
            }

            foreach (KeyValuePair<DataTeam, int> pair in dicParentID)
            {
                if (pair.Key.ParentTeam != null)
                    continue;

                if (!dicTeams.ContainsKey(pair.Value))
                    continue;

                DataTeam teamParent = dicTeams[pair.Value];
                pair.Key.ParentTeam = teamParent;
                pair.Key.CompanyName = teamParent.CompanyName;
            }

            return arrExternalRootTeams;
        }

        // dicTeams : ID별 Team
        private DataTeam LoadRegularTeam(DirectDBManager dbMgr, Dictionary<int, DataTeam> dicTeams)
        {
            // Site별로 사용할 수 있도록 수정 , Edit by skkim 2015.01.14
            // SiteID로 본부 아이디를 가져온다.
            string szSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", dbMgr.SiteID);
            ArrayList arrResult1 = dbMgr.GetResultData(szSQL);
            if (arrResult1 == null || arrResult1.Count == 0)
                return null;

            int nTeamID = WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
            if (nTeamID == -1)
                return null;

            ArrayList arrResult = ExecuteTeamList(dbMgr, nTeamID);
            if (arrResult == null)
                return null;

            // 자신의 Team, 부모 팀의 ID
            Dictionary<DataTeam, int> dicParentID = new Dictionary<DataTeam, int>();
            int nCount = arrResult.Count;

            for (int i = 0; i < nCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string szTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                DataTeam data = new DataTeam();
                data.ID = nID;
                data.TeamName = szTeamName;
                data.External = false;

                dicTeams[nID] = data;
                dicParentID[data] = nParentTeamID;
            }

            DataTeam teamRoot = null;
            foreach (KeyValuePair<DataTeam, int> pair in dicParentID)
            {
                if (pair.Value < 0)
                {
                    teamRoot = pair.Key;
                    teamRoot.IsCompany = true;
                    continue;
                }

                if (!dicTeams.ContainsKey(pair.Value))
                    continue;

                DataTeam teamParent = dicTeams[pair.Value];
                pair.Key.ParentTeam = teamParent;
            }

            return teamRoot;
        }

        public static ArrayList ExecuteTeamList(DirectDBManager dbMgr, int nRootTeamID, string strTableName = "RegularTeam")
        {
            string strSQL = "Select ID, TeamName, ParentTeamID from " + strTableName + " order by ParentTeamID, ID";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            if (nRootTeamID == 0)
                return arrResult;

            int nResultCount = arrResult.Count;

            ArrayList arrNewResult = new ArrayList();
            Dictionary<int, int> dicParentID = new Dictionary<int, int>();

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                VariousData<int> parentID = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                if (dicParentID.Count == 0)
                {
                    if (nID == nRootTeamID)
                    {
                        dicParentID[nID] = nID;

                        arrNewResult.Add(arrResult[i]);
                        arrNewResult.Add(arrResult[i + 1]);
                        arrNewResult.Add(arrResult[i + 2]);
                    }
                }
                else
                {
                    if (parentID == null)
                        continue;

                    if (dicParentID.ContainsKey(parentID.Data))
                    {
                        dicParentID[nID] = nID;

                        arrNewResult.Add(arrResult[i]);
                        arrNewResult.Add(arrResult[i + 1]);
                        arrNewResult.Add(arrResult[i + 2]);
                    }
                }
            }

            return arrNewResult;
        }

        // EquipZone별 시설물 담당자 얻어오기
        public FacilityManagerGroup GetEquipZoneFacilityManagerGroup(IFacility.FacilityType type, EquipmentZone zone, bool isDetectTime, bool alwaysGet = false)
        {
            Dictionary<IFacility.FacilityType, Dictionary<int, FacilityManagerGroup>> dicEquipZoneFacilityManager = isDetectTime ? m_dicEquipZoneFacilityManager : m_dicEquipZoneFacilityManagerReport;
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

                if (type == IFacility.FacilityType.FIRE_SENSOR ||
                    type == IFacility.FacilityType.COOLER_SENSOR ||
                    type == IFacility.FacilityType.PRESSURE_SENSOR)
                {
                    dicEquipZoneFacilityManager[IFacility.FacilityType.FIRE_SENSOR] = dicManagers;
                    dicEquipZoneFacilityManager[IFacility.FacilityType.COOLER_SENSOR] = dicManagers;
                    dicEquipZoneFacilityManager[IFacility.FacilityType.PRESSURE_SENSOR] = dicManagers;
                }
                else if (type == IFacility.FacilityType.FE ||
                    type == IFacility.FacilityType.HD ||
                    type == IFacility.FacilityType.FA)
                {
                    dicEquipZoneFacilityManager[IFacility.FacilityType.FE] = dicManagers;
                    dicEquipZoneFacilityManager[IFacility.FacilityType.HD] = dicManagers;
                    dicEquipZoneFacilityManager[IFacility.FacilityType.FA] = dicManagers;
                }
                else
                    dicEquipZoneFacilityManager[type] = dicManagers;

                FacilityManagerGroup group = new FacilityManagerGroup();
                group.Type = type;
                group.EquipZone = zone;

                dicManagers[zone.ID] = group;
                return group;
            }

            return null;
        }

        public List<DataCompanyMember> GetRegularTeamMembers(DataTeam team)
        {
            if (team.External)
                return null;

            List<DataCompanyMember> members;

            if (m_dicRegularTeamMembers.TryGetValue(team, out members))
                return members;

            return null;
        }

        public List<DataExternalMember> GetExternalTeamMembers(DataTeam team)
        {
            if (team.External)
            {
                List<DataExternalMember> members;

                if (m_dicExternalTeamMembers.TryGetValue(team, out members))
                    return members;
            }

            return null;
        }

        public DataCompanyMember GetRegularMember(int nMemberID)
        {
            DataCompanyMember member;

            if (m_dicRegularMembers.TryGetValue(nMemberID, out member))
                return member;

            return null;
        }

        public List<DataCompanyMember> GetAllRegularMember()
        {
            return m_dicRegularMembers.Values.ToList();
        }

        public List<DataExternalMember> GetAllExternalMember()
        {
            return m_dicExternalMembers.Values.ToList();
        }

        public DataExternalMember GetExternalMember(int nMemberID)
        {
            DataExternalMember member;

            if (m_dicExternalMembers.TryGetValue(nMemberID, out member))
                return member;

            return null;
        }

        public void AddAllCompanyMemberPhoneNumbers(Dictionary<string, string> dicPhoneNumbers, Dictionary<int, int> dicRegularMemberIDs)
        {
            foreach (KeyValuePair<int, DataCompanyMember> pair in m_dicRegularMembers)
            {
                dicPhoneNumbers[pair.Value.PhoneNumber] = pair.Value.PhoneNumber;
                dicRegularMemberIDs[pair.Key] = pair.Key;
            }
        }

        // 암호화된 데이터를 복호화시킨다.
        public static string Convert(string strEnc)
        {
            return AES256Cipher.AES_decrypt(strEnc, key);
        }

        public void ReloadRegularMembers(DirectDBManager dbMgr)
        {
            m_dicRegularTeams.Clear();
            m_dicRegularTeamMembers.Clear();
            m_dicRegularMembers.Clear();

            m_teamRegularRoot = LoadRegularTeam(dbMgr, m_dicRegularTeams);

            // SiteID를 고려하지 않은 전체 직원 리스트
            Dictionary<int, DataCompanyMember> members = new Dictionary<int, DataCompanyMember>();

            LoadCompanyMember(dbMgr, members);
            LoadRegularMemberList(dbMgr, m_dicRegularTeams, members);
        }

        public void ReloadExternalMembers(DirectDBManager dbMgr)
        {
            m_dicExternalTeams.Clear();
            m_dicExternalTeamMembers.Clear();
            m_dicExternalMembers.Clear();

            m_listExternalRootTeams = LoadExternalTeam(dbMgr, m_dicExternalTeams);

            // SiteID를 고려하지 않은 전체 협력업체 직원 리스트
            Dictionary<int, DataExternalMember> externalMembers = new Dictionary<int, DataExternalMember>();

            LoadExternalMember(dbMgr, externalMembers);
            LoadExternalMemberList(dbMgr, m_dicExternalTeams, externalMembers);
        }

        public void ReloadControlRoomTeams(DirectDBManager dbMgr)
        {
            m_dicControlRoomTeams.Clear();
            LoadControlRoomTeams(dbMgr, m_dicControlRoomTeams);
        }
    }
}
