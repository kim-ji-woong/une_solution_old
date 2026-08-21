using System.Collections;
using System.Collections.Generic;
using SDMS.IDAL;
using TeamEditor.Model.Sop.Team;

namespace SafetyServer.BLL
{
    using Data.Spatial;
    using Data.Response;
    using Data.Models;

    public class SpatialManager
    {
        private IDataManager m_dataManager = null;
        private TeamEditor.IDAL.IDataManager m_teamDataManager = null;
        private Common.IDAL.IDataManager m_commonDataManager = null;

        public SpatialManager(IDataManager dataManager, TeamEditor.IDAL.IDataManager teamDataManager, Common.IDAL.IDataManager commonDataManager)
        {
            m_dataManager = dataManager;
            m_teamDataManager = teamDataManager;
            m_commonDataManager = commonDataManager;
        }

        public ResponseSpatialInfo GetSpatialInfo()
        {
            ResponseSpatialInfo response = new ResponseSpatialInfo();

            if (m_dataManager == null)
                return response;

            string strErrorMessage;
            ArrayList arrResult = m_dataManager.GetSelectManager().JoinBuildingGroupBuildingZone(null, null, null, null, out strErrorMessage);

            if (arrResult == null)
                return response;

            int nResultCount = arrResult.Count;

            Dictionary<int, BuildingGroupData> dicBuildingGroupDatas = new Dictionary<int, BuildingGroupData>();
            Dictionary<int, BuildingData> dicBuildingDatas = new Dictionary<int, BuildingData>();
            Dictionary<int, ZoneData> dicZoneDatas = new Dictionary<int, ZoneData>();

            BuildingGroupData buildingGroupData;
            BuildingData buildingData;
            ZoneData zoneData;

            for (int i=0;i<nResultCount-2;i+=3)
            {
                if (arrResult[i] != null && arrResult[i + 1] != null && arrResult[i + 2] != null &&
                    arrResult[i] is SDMS.Model.Spatial.BuildingGroup &&
                    arrResult[i + 1] is SDMS.Model.Spatial.Building &&
                    arrResult[i + 2] is SDMS.Model.Spatial.Zone)
                {
                    SDMS.Model.Spatial.BuildingGroup buildingGroup = (SDMS.Model.Spatial.BuildingGroup)arrResult[i];
                    SDMS.Model.Spatial.Building building = (SDMS.Model.Spatial.Building)arrResult[i + 1];
                    SDMS.Model.Spatial.Zone zone = (SDMS.Model.Spatial.Zone)arrResult[i + 2];

                    if (dicBuildingGroupDatas.TryGetValue(buildingGroup.ID, out buildingGroupData) == false)
                    {
                        buildingGroupData = new BuildingGroupData();
                        buildingGroupData.ID = buildingGroup.ID;
                        buildingGroupData.Name = buildingGroup.GroupName;

                        dicBuildingGroupDatas[buildingGroup.ID] = buildingGroupData;
                    }

                    if (dicBuildingDatas.TryGetValue(building.ID, out buildingData) == false)
                    {
                        buildingData = new BuildingData();
                        buildingData.ID = building.ID;
                        buildingData.Name = building.BuildingName;

                        dicBuildingDatas[building.ID] = buildingData;
                        buildingGroupData.Buildings.Add(buildingData);
                    }

                    if (dicZoneDatas.TryGetValue(zone.ID, out zoneData) == false)
                    {
                        if (zone.BuildingID != null && zone.FloorIndex == null)
                        {
                            // 건물 전체를 의미한다.
                            continue;
                        }
                        
                        zoneData = new ZoneData();
                        zoneData.ID = zone.ID;
                        zoneData.Name = zone.ZoneName;
                        zoneData.FloorIndex = zone.FloorIndex;

                        dicZoneDatas[zone.ID] = zoneData;
                        buildingData.Fields.Add(zoneData);
                    }
                }
            }

            Dictionary<SDMS.Model.Spatial.Zone.Fields, object> dicConditions = new Dictionary<SDMS.Model.Spatial.Zone.Fields, object>();
            dicConditions[SDMS.Model.Spatial.Zone.Fields.BuildingID] = null;

            List<SDMS.Model.Spatial.Zone> outdoorZones = m_dataManager.GetSelectManager().SelectZones(dicConditions, null, out strErrorMessage);

            if (outdoorZones == null)
                return response;

            foreach (KeyValuePair<int, BuildingGroupData> pair in dicBuildingGroupDatas)
            {
                response.BuildingGroups.Add(pair.Value);
            }

            foreach (SDMS.Model.Spatial.Zone zone in outdoorZones)
            {
                if (zone.ID >= dnsSopID.Header.ManualReportDefaultID)
                {
                    // 특수한 용도를 위하여 만들어진 Zone이다.
                    continue;
                }

                ZoneData _zoneData = new ZoneData();
                _zoneData.ID = zone.ID;
                _zoneData.Name = zone.ZoneName;
                _zoneData.FloorIndex = zone.FloorIndex;

                response.OutdoorFields.Add(_zoneData);
            }

            return response;
        }

        public ResponseFieldUserPosition GetFieldUserPosition(int? fieldID, List<string> userIDs)
        {
            ResponseFieldUserPosition response = new ResponseFieldUserPosition();
            response.Success = false;

            bool isNullable, useFieldID = false;
            string strErrorMessage;
            string strConditions = "";

            Dictionary<string, string> dicUserIDs = new Dictionary<string, string>();

            if (userIDs != null && userIDs.Count > 0)
            {
                strConditions = string.Format("{0} in (", RegularMember.GetFieldName(RegularMember.Fields.MemberID, out isNullable));

                int nUserCount = userIDs.Count;

                for (int i=0;i<nUserCount;i++)
                {
                    string strUser = userIDs[i];
                    dicUserIDs[strUser] = strUser;

                    if (i == 0)
                        strConditions += "'" + strUser + "'";
                    else
                        strConditions += ",'" + strUser + "'";
                }

                if (nUserCount == 0)
                {
                    response.Success = true;
                    return response;
                }

                strConditions += ")";
            }
            else if (fieldID != null)
            {
                useFieldID = true;
                strConditions = string.Format("{0} like '{1}%'", RegularMember.GetFieldName(RegularMember.Fields.Email, out isNullable), (int)fieldID);
            }
            else
            {
                response.Message = "filedID is empty, userIDs is empty";
                return response;
            }

            List<RegularMember> members = m_teamDataManager.GetSelectManager().SelectRegularMembers(strConditions, out strErrorMessage);

            if (members == null)
            {
                response.Message = strErrorMessage;
                return response;
            }

            int nZoneID;
            float x, y;

            foreach (RegularMember member in members)
            {
                if (member.Email == null)
                    continue;

                string[] tokens = member.Email.Split(',');

                if (tokens.Length != 3)
                    continue;

                if (int.TryParse(tokens[0].Trim(), out nZoneID) &&
                    float.TryParse(tokens[1].Trim(), out x) &&
                    float.TryParse(tokens[2].Trim(), out y))
                {
                    if (useFieldID && nZoneID != (int)fieldID)
                        continue;

                    ResponseFieldUserPosition.UserPosition position = new ResponseFieldUserPosition.UserPosition();

                    position.ID = member.MemberID;
                    position.X = x;
                    position.Y = y;
                    position.FieldID = nZoneID;

                    response.UserPositions.Add(position);

                    dicUserIDs.Remove(member.MemberID);
                }
            }

            foreach (KeyValuePair<string, string> pair in dicUserIDs)
            {
                // 쿼리에서 처리되지 못한 사용자들...
                ResponseFieldUserPosition.UserPosition position = new ResponseFieldUserPosition.UserPosition();

                position.ID = pair.Key;
                position.X = null;
                position.Y = null;
                position.FieldID = null;

                response.UserPositions.Add(position);
            }

            response.Success = true;
            return response;
        }

        public string[] GetZoneImageCoord(int zoneID)
        {
            string strErrorMessage;
            List<Common.Model.Option.Options> options = m_commonDataManager.GetSelectManager().SelectOptions(Common.Model.Option.Options.OptionTarget.SDMS, out strErrorMessage);

            if (options == null)
            {
                if (strErrorMessage != null)
                    return new string[] { strErrorMessage };
            }
            else
            {
                string strTrg = zoneID >= 0 ? "(" + zoneID.ToString() + ")" : "(outdoor)";

                foreach (Common.Model.Option.Options option in options)
                {
                    if (option.PropertyName.ToLower().StartsWith("zoneimagecoord"))
                    {
                        if (option.PropertyName.EndsWith(strTrg))
                        {
                            string[] tokens = option.PropertyValue.Split(',');

                            if (tokens.Length >= 6)
                            {
                                string strTL = tokens[0] + "," + tokens[1];
                                string strBL = tokens[2] + "," + tokens[3];
                                string strBR = tokens[4] + "," + tokens[5];
                                return new string[] { strTL, strBL, strBR };
                            }
                        }
                    }
                }
            }

            return new string[] { "Unknown field id" };
        }

        public ResponseMobieUserList GetMobileUserList()
        {
            string strErrorMessage;
            List<RegularMember> members = m_teamDataManager.GetSelectManager().SelectRegularMembers(out strErrorMessage);

            if (members == null)
                return new ResponseMobieUserList(false, strErrorMessage);

            List<Regular> teams = m_teamDataManager.GetSelectManager().SelectRegulars(out strErrorMessage);

            if (teams == null)
                return new ResponseMobieUserList(false, strErrorMessage);

            Dictionary<int, Regular> dicTeams = new Dictionary<int, Regular>();

            foreach (Regular team in teams)
            {
                dicTeams[team.ID] = team;
            }

            List<Options> options = m_teamDataManager.GetSelectManager().SelectOptions("PropertyName = 'JobLevel'", out strErrorMessage);

            if (options == null)
                return new ResponseMobieUserList(false, strErrorMessage);

            Dictionary<int, string> dicJobLevels = new Dictionary<int, string>();

            foreach (Options option in options)
            {
                dicJobLevels[option.PropertyID] = option.PropertyValue;
            }

            ResponseMobieUserList users = new ResponseMobieUserList(true, "");

            foreach (RegularMember member in members)
            {
                Regular team;
                string strJobLevelName;
                int nZoneID;
                float x, y;
                
                if (dicTeams.TryGetValue(member.RegularID, out team))
                {
                    MobileUser user = new MobileUser();
                    user.ID = member.ID;
                    user.LoginStatus = Process.MemberManager.IsLoginStatus(member);
                    user.MemberID = member.MemberID;
                    user.Name = member.MemberName;
                    user.TeamName = team.TeamName;
                    
                    if (Process.MemberManager.GetUserPosition(member, out nZoneID, out x, out y))
                    {
                        user.ZoneID = nZoneID;
                        user.X = x;
                        user.Y = y;
                    }

                    if (member.JobLevelID != null && dicJobLevels.TryGetValue((int)member.JobLevelID, out strJobLevelName))
                        user.JobLevelName = strJobLevelName;

                    users.UserList.Add(user);
                }
            }

            return users;
        }
    }
}
