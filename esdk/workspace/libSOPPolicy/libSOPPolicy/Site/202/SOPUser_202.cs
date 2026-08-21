using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;

namespace libSOPPolicy
{
    using Common;

    public class SOPUser_202 : BaseSOPUser
    {
        // 일반유저, 시청유저, 도청유저
        public enum UserType { DoUser = 0, CityUser, NormalUser, Unknown};

        private UserType m_userType = UserType.Unknown;
        private BuildingGroup m_buildingGroup = null;

        private static Dictionary<int, BuildingGroup> m_dicBuildingGroups = null;
        private static Dictionary<int, Building> m_dicBuildings = null;

        public SOPUser_202(WebDBManager dbMgr, int nSOPGenUserID)
        {
            LoadBuildingGroups(dbMgr, dbMgr.SiteID);
            LoadBuildings(dbMgr);
            SetUserType(dbMgr, nSOPGenUserID, dbMgr.SiteID);
        }

        public SOPUser_202(DirectDBManager dbMgr, int nSOPGenUserID)
        {
            dbMgr = dbMgr.Clone();

            if (dbMgr.Connect())
            {
                LoadBuildingGroups(dbMgr, dbMgr.SiteID);
                LoadBuildings(dbMgr);
                SetUserType(dbMgr, nSOPGenUserID, dbMgr.SiteID);

                dbMgr.Close();
            }
        }

        public UserType GetUserType()
        {
            return m_userType;
        }

        private void LoadBuildingGroups(object dbMgr, int nSiteID)
        {
            if (m_dicBuildingGroups != null)
                return;

            string strSQL = "Select ID, ParentID, GroupName, GroupType from BuildingGroup where SiteID = " + nSiteID.ToString();
            ArrayList arrResult = DBManager.GetResultData(strSQL, dbMgr);

            if (arrResult == null)
                return;

            // Key : Child ID
            // Value : Parent ID
            Dictionary<int, int> dicParents = new Dictionary<int, int>();
            m_dicBuildingGroups = new Dictionary<int, BuildingGroup>();

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-3;i+=4)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> parentID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                string strName = WebDBManager.GetStringField(arrResult[i + 2]);
                VariousData<int> groupType = WebDBManager.GetIntField(arrResult[i + 3].ToString());

                if (id == null || strName == null)
                    continue;

                BuildingGroup group = new BuildingGroup();
                group.ID = id.Data;
                group.GroupName = strName;

                if (groupType != null)
                    group.SetGroupType(BuildingGroup.ToGroupType(groupType.Data));

                m_dicBuildingGroups[group.ID] = group;

                if (parentID != null)
                    dicParents[group.ID] = parentID.Data;
            }

            foreach (KeyValuePair<int, int> pair in dicParents)
            {
                BuildingGroup child, parent;

                if (m_dicBuildingGroups.TryGetValue(pair.Key, out child) && m_dicBuildingGroups.TryGetValue(pair.Value, out parent))
                {
                    child.ParentGroup = parent;
                }
            }
        }

        private void LoadBuildings(object dbMgr)
        {
            if (m_dicBuildingGroups == null || m_dicBuildings != null)
                return;

            string strSQL = "Select ID, BuildingID, BuildingCode, BuildingName, BuildingGroupID from Building";
            ArrayList arrResult = DBManager.GetResultData(strSQL, dbMgr);

            if (arrResult == null)
                return;

            m_dicBuildings = new Dictionary<int, Building>();

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strBuildingID = WebDBManager.GetStringField(arrResult[i + 1]);
                string strBuildingCode = WebDBManager.GetStringField(arrResult[i + 2]);
                string strBuildingName = WebDBManager.GetStringField(arrResult[i + 3]);
                VariousData<int> buildingGroupID = WebDBManager.GetIntField(arrResult[i + 4].ToString());

                if (id == null || strBuildingID == null || strBuildingCode == null || strBuildingName == null || buildingGroupID == null)
                    continue;

                BuildingGroup group;

                if (m_dicBuildingGroups.TryGetValue(buildingGroupID.Data, out group) == false)
                    continue;

                Building building = new Building();

                building.ID = id.Data;
                building.BuildingID = strBuildingID;
                building.BuildingCode = strBuildingCode;
                building.BuildingName = strBuildingName;
                building.BuildingGroup = group;

                m_dicBuildings[building.ID] = building;
            }
        }

        private void SetUserType(object dbMgr, int nSOPGenUserID, int nSiteID)
        {
            ID = nSOPGenUserID;

            if (dbMgr == null)
                return;

            this.SiteID = nSiteID;

            string strSQL = "Select su.ID, su.UserID, su.NickName, su.UserLevel, sl.LevelName, sbg.BuildingGroupID ";
            strSQL += "from SOPGenUser as su, SOPGenLevel as sl, SOPGenUserBuildingGroup as sbg, BuildingGroup as bg ";
            strSQL += "where su.UserLevel = sl.ID and su.ID = sbg.UserID and sbg.BuildingGroupID = bg.ID and su.ID = " + nSOPGenUserID.ToString();

            ArrayList arrResult = DBManager.GetResultData(strSQL, dbMgr);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strUserID = WebDBManager.GetStringField(arrResult[i + 1]);
                string strUserNickName = WebDBManager.GetStringField(arrResult[i + 2]);
                VariousData<int> userLevel = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                string strLevelName = WebDBManager.GetStringField(arrResult[i + 4]);
                VariousData<int> buildingGroupID = WebDBManager.GetIntField(arrResult[i + 5].ToString());

                // 다른 Site와 달리 202에서는 buildingID가 필수
                if (id == null || strUserID == null || userLevel == null || strLevelName == null || strUserNickName == null || buildingGroupID == null)
                    continue;

                BuildingGroup buildingGroup;

                if (m_dicBuildingGroups.TryGetValue(buildingGroupID.Data, out buildingGroup) == false)
                    continue;

                UserID = strUserID;
                NickName = strUserNickName;
                m_buildingGroup = buildingGroup;

                if (strLevelName.Contains("시"))
                {
                    m_userType = UserType.CityUser;
                    //SetCity(dbMgr, m_building);
                }
                else if (strLevelName.Contains("도"))
                {
                    m_userType = UserType.DoUser;
                    //SetCity(dbMgr, m_building);
                }
                else
                    m_userType = UserType.NormalUser;

                break;
            }
        }

        /*private bool SetCity(WebDBManager dbMgr, Building building)
        {
            if (building == null)
                return false;

            string strSQL = "Select BuildingGroupID from Building where ID = " + building.ID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            VariousData<int> buildingGroupID = WebDBManager.GetIntField(arrResult[0].ToString());

            if (buildingGroupID == null)
                return false;

            BuildingGroup buildingGroup;

            if (m_dicBuildingGroups.TryGetValue(buildingGroupID.Data, out buildingGroup) == false)
                return false;

            if (buildingGroup.GetGroupType() == BuildingGroup.GroupType.City)
            {
                m_city = buildingGroup;
                return true;
            }

            BuildingGroup parent = buildingGroup.ParentGroup;

            while (parent != null)
            {
                if (parent.GetGroupType() == BuildingGroup.GroupType.City)
                {
                    m_city = buildingGroup;
                    return true;
                }

                parent = parent.ParentGroup;
            }

            return false;
        }*/

        // nSensorZoneID 신호가 탐지되었을때 이에 연결된 SOP를 이 SOPUser 계정으로 제어가 가능한지 여부를 알려준다.
        public override bool AbletoAccess(int nSensorZoneID, int nSiteID, WebDBManager dbMgr)
        {
            return _AbletoAccess(nSensorZoneID, nSiteID, dbMgr);
        }

        // nSensorZoneID 신호가 탐지되었을때 이에 연결된 SOP를 이 SOPUser 계정으로 제어가 가능한지 여부를 알려준다.
        public override bool AbletoAccess(int nSensorZoneID, int nSiteID, DirectDBManager dbMgr)
        {
            dbMgr = dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return false;

            bool result = _AbletoAccess(nSensorZoneID, nSiteID, dbMgr);
            dbMgr.Close();
            return result;
        }

        // nSensorZoneID 신호가 탐지되었을때 이에 연결된 SOP를 이 SOPUser 계정으로 제어가 가능한지 여부를 알려준다.
        private bool _AbletoAccess(int nSensorZoneID, int nSiteID, object dbMgr)
        {
            if (m_userType != UserType.NormalUser)
                return false;

            string strSQL = "Select EquipZoneID, Zone, ez.LinkedZoneIDList from SensorZone as sz, EquipmentZone as ez where sz.EquipZoneID = ez.ID and sz.ID = " + nSensorZoneID.ToString();
            ArrayList arrResult = DBManager.GetResultData(strSQL, dbMgr);

            if (arrResult == null || arrResult.Count < 3)
                return false;

            VariousData<int> equipZoneID = WebDBManager.GetIntField(arrResult[0].ToString());
            VariousData<int> zoneID = WebDBManager.GetIntField(arrResult[1].ToString());
            string strLinkedZoneIDs = WebDBManager.GetStringField(arrResult[2]);

            int nBuildingID = -1;

            if (zoneID != null)
                nBuildingID = GetBuildingID(zoneID.Data, dbMgr);
            else
            {
                if (strLinkedZoneIDs != null && strLinkedZoneIDs.Length > 0)
                {
                    string[] tokens = strLinkedZoneIDs.Split(',');

                    int nZoneID;

                    if (int.TryParse(tokens[0].Trim(), out nZoneID))
                    {
                        nBuildingID = GetBuildingID(nZoneID, dbMgr);
                    }
                }
            }

            if (nBuildingID < 0)
            {
                return false;
            }
            else
            {
                Building building;

                if (m_buildingGroup != null && m_dicBuildings.TryGetValue(nBuildingID, out building))
                {
                    // 건물이 달라도 건물그룹이 같으면 같은 SOP를 공유할 수 있다.(특정 Site에만 적용된다.)
                    if (m_buildingGroup == building.BuildingGroup)
                        return true;
                }
                /*Building building;

                if (m_building != null && m_dicBuildings.TryGetValue(nBuildingID, out building))
                {
                    // 건물이 달라도 건물그룹이 같으면 같은 SOP를 공유할 수 있다.(특정 Site에만 적용된다.)
                    if (m_building.BuildingGroup == building.BuildingGroup)
                        return true;
                }*/
            }

            return false;
        }

        private int GetBuildingID(int nZoneID, object dbMgr)
        {
            string strSQL = "Select BuildingID from Zone where ID = " + nZoneID.ToString();
            ArrayList arrResult = DBManager.GetResultData(strSQL, dbMgr);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> buildingID = WebDBManager.GetIntField(arrResult[0].ToString());

            if (buildingID == null)
                return -1;

            return buildingID.Data;
        }

        // strSOPFullPath 해당하는 SOP를 이 SOPUser 계정으로 제어가 가능한지 여부를 알려준다.
        public override bool AbletoAccess(string strSOPFullPath, int nSiteID, WebDBManager dbMgr)
        {
            return _AbletoAccess(strSOPFullPath, nSiteID, dbMgr);
        }

        // strSOPFullPath 해당하는 SOP를 이 SOPUser 계정으로 제어가 가능한지 여부를 알려준다.
        public override bool AbletoAccess(string strSOPFullPath, int nSiteID, DirectDBManager dbMgr)
        {
            dbMgr = dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return false;

            bool result = _AbletoAccess(strSOPFullPath, nSiteID, dbMgr);
            dbMgr.Close();
            return result;
        }

        // strSOPFullPath 해당하는 SOP를 이 SOPUser 계정으로 제어가 가능한지 여부를 알려준다.
        private bool _AbletoAccess(string strSOPFullPath, int nSiteID, object dbMgr)
        {
            if (m_userType != UserType.NormalUser)
                return false;

            SOPLoader loader = new SOPLoader();
            int nDisasterID;

            if (loader.GetLinkedSOP(strSOPFullPath, dbMgr, out nDisasterID) == false)
                return false;

            int nBuildingID = loader.GetDisasterBuilding(nDisasterID, dbMgr);

            if (nBuildingID < 0)
            {
                return false;
            }
            else
            {
                Building building;

                if (m_buildingGroup != null && m_dicBuildings.TryGetValue(nBuildingID, out building))
                {
                    // 건물이 달라도 건물그룹이 같으면 같은 SOP를 공유할 수 있다.(특정 Site에만 적용된다.)
                    if (m_buildingGroup == building.BuildingGroup)
                        return true;
                }
                /*Building building;

                if (m_building != null && m_dicBuildings.TryGetValue(nBuildingID, out building))
                {
                    // 건물이 달라도 건물그룹이 같으면 같은 SOP를 공유할 수 있다.(특정 Site에만 적용된다.)
                    if (m_building.BuildingGroup == building.BuildingGroup)
                        return true;
                }*/
            }

            return false;
        }

        // 탐지된 nSensorZoneID 신호가 실제 재난상황으로 전파되었다.
        // 이에 연결된 SOP를 이 SOPUser 계정으로 제어가 가능한지 여부를 알려준다.
        // 이 SOPUser 계정에 권한이 없으면 -2가 리턴된다.
        // 적당한 SOP가 존재하지 않으면 -1이 리턴된다.
        // 권한도 있고 SOP도 있으면 해당 Disaster ID가 리턴된다.
        public override int GetReportDisasterID(int nSensorZoneID, int nSiteID, WebDBManager dbMgr)
        {
            return _GetReportDisasterID(nSensorZoneID, nSiteID, dbMgr);
        }

        // 탐지된 nSensorZoneID 신호가 실제 재난상황으로 전파되었다.
        // 이에 연결된 SOP를 이 SOPUser 계정으로 제어가 가능한지 여부를 알려준다.
        // 이 SOPUser 계정에 권한이 없으면 -2가 리턴된다.
        // 적당한 SOP가 존재하지 않으면 -1이 리턴된다.
        // 권한도 있고 SOP도 있으면 해당 Disaster ID가 리턴된다.
        public override int GetReportDisasterID(int nSensorZoneID, int nSiteID, DirectDBManager dbMgr)
        {
            dbMgr = dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return -2;

            int nResult = _GetReportDisasterID(nSensorZoneID, nSiteID, dbMgr);
            dbMgr.Close();
            return nResult;
        }

        // 탐지된 nSensorZoneID 신호가 실제 재난상황으로 전파되었다.
        // 이에 연결된 SOP를 이 SOPUser 계정으로 제어가 가능한지 여부를 알려준다.
        // 이 SOPUser 계정에 권한이 없으면 -2가 리턴된다.
        // 적당한 SOP가 존재하지 않으면 -1이 리턴된다.
        // 권한도 있고 SOP도 있으면 해당 Disaster ID가 리턴된다.
        private int _GetReportDisasterID(int nSensorZoneID, int nSiteID, object dbMgr)
        {
            // 시, 도 사용자의 경우에만 재난신고에 대한 SOP가 작동한다.
            // 이 SOP는 다른 SOP 사용자들의 화면에는 나타나지 않고, 오직 시,도 사용자의 화면에만 나타난다.
            if (m_userType == UserType.NormalUser || m_userType == UserType.Unknown)
                return -2;

            if (m_buildingGroup == null)
                return -2;

            // 도 사용자는 항상 SOP 실행이 가능하다.
            if (m_userType == UserType.DoUser)
                return GetReportDisasterID(m_buildingGroup.GroupName, (int)m_userType, dbMgr);

            // 시 사용자는 자신의 도시내의 센서신호 SOP만 실행이 가능하다.
            string strSQL = "Select sz.ID, z.ZoneName, b.BuildingName, g.ID, g.GroupName, g.ParentID ";
            strSQL += "from SensorZone as sz, Zone as z, Building as b, BuildingGroup as g ";
            strSQL += "where sz.Zone = z.ID and z.BuildingID = b.ID and b.BuildingGroupID = g.ID and sz.ID = " + nSensorZoneID.ToString();

            ArrayList arrResult = DBManager.GetResultData(strSQL, dbMgr);

            if (arrResult == null || arrResult.Count < 6)
                return -2;

            VariousData<int> buildingGroupID = WebDBManager.GetIntField(arrResult[3].ToString());
            VariousData<int> parentBuildingGroupID = WebDBManager.GetIntField(arrResult[5].ToString());

            if (buildingGroupID == null)
                return -2;

            bool permit = false;

            if (buildingGroupID.Data == m_buildingGroup.ID)
                permit = true;
            else if (parentBuildingGroupID != null)
            {
                if (parentBuildingGroupID.Data == m_buildingGroup.ID)
                    permit = true;
                else
                    permit = FindBuildingGroup(parentBuildingGroupID.Data, m_buildingGroup.ID, dbMgr);
            }

            if (permit)
                return GetReportDisasterID(m_buildingGroup.GroupName, (int)m_userType, dbMgr);

            return -2;
        }

        private int GetReportDisasterID(string strAreaName, int nLevelID, object dbMgr)
        {
            string strSQL = "Select DisasterID from SOPGenLevelDisaster where LevelID = " + nLevelID.ToString();
            ArrayList arrResult = DBManager.GetResultData(strSQL, dbMgr);

            if (arrResult == null)
                return -1;

            string strDisasterIDs = "";

            foreach (object obj in arrResult)
            {
                VariousData<int> disasterID = WebDBManager.GetIntField(obj.ToString());

                if (disasterID == null)
                    continue;

                if (strDisasterIDs.Length == 0)
                    strDisasterIDs = disasterID.Data.ToString();
                else
                    strDisasterIDs += ", " + disasterID.Data.ToString();
            }

            if (strDisasterIDs.Length == 0)
                return -1;

            strSQL = "Select d.ID, d.DisasterName, sdc.ID, sdc.SubCategoryName ";
            strSQL += "from Disaster as d, SubDisasterCategory as sdc ";
            strSQL += string.Format("where d.SubDisasterID = sdc.ID and sdc.SubCategoryName = '{0}' and d.ID in ({1})", strAreaName, strDisasterIDs);

            arrResult = DBManager.GetResultData(strSQL, dbMgr);

            if (arrResult == null || arrResult.Count < 4)
                return -1;

            VariousData<int> _disasterID = WebDBManager.GetIntField(arrResult[0].ToString());

            if (_disasterID == null)
                return -1;

            return _disasterID.Data;
        }

        private bool FindBuildingGroup(int nSrcBuildingGroupID, int nTrgBuildingGroupID, object dbMgr)
        {
            string strSQL = "Select ParentID from BuildingGroup where ID = " + nSrcBuildingGroupID.ToString();
            ArrayList arrResult = DBManager.GetResultData(strSQL, dbMgr);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            VariousData<int> parentID = WebDBManager.GetIntField(arrResult[0].ToString());

            if (parentID == null)
                return false;

            if (parentID.Data == nTrgBuildingGroupID)
                return true;

            return FindBuildingGroup(parentID.Data, nTrgBuildingGroupID, dbMgr);
        }

        public override bool AbleToEditTools()
        {
            return true;
            //return m_userType >= UserType.BuildingAdmin;
        }

        // Site별로 계정등급은 다르게 설정되어 있을수 있다.
        public override int GetUserGrade()
        {
            return (int)m_userType;
        }
    }
}
