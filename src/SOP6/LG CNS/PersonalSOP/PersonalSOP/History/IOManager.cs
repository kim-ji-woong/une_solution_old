using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DBUtility2;
using System.Collections;
using System.Drawing;

namespace PersonalSOP.History
{
    // SOP Data를 DB에 저장 및 불러오기 담당
    public class IOManager
    {
        private Dictionary<int, List<TemporaryMember>> m_dicTemporaryNormalMemberID = null;
        private Dictionary<int, List<TemporaryMember>> m_dicTemporaryEmergencyMemberID = null;

        public IOManager()
        {
        }

        private bool LoadProcessMission(WebDBManager dbMgr, int nProcessID, ArrayList arrMissionItems)
        {
            string strSQL = string.Format("Select ID, missionText from ProcessMission where ProcessID = {0}", nProcessID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strMissionText = WebDBManager.GetStringField(arrResult[i + 1], "");
                //int nTransmission = dbMgr.GetIntField(arrResult[i + 2].ToString(), -1);

                Sections.MissionItem item = new Sections.MissionItem();

                item.Mission = strMissionText;
                //item.Transmission = nTransmission;

                arrMissionItems.Add(item);
            }

            return true;
        }

        private bool LoadCheckedItems(WebDBManager dbMgr, int nProcessID, ArrayList arrCheckedItems)
        {
            string strSQL = string.Format("Select ID, Category, SubCategory, TaskName, TargetCount, Position from CheckTask where ProcessID = {0}", nProcessID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strCategory = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strSubCategory = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strTaskName = WebDBManager.GetStringField(arrResult[i + 3], "");
                int nTargetCount = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                string strPosition = WebDBManager.GetStringField(arrResult[i + 5], "");

                Sections.CheckedItem item = new Sections.CheckedItem();

                item.Category = strCategory;
                item.SubCategory = strSubCategory;
                item.Item = strTaskName;
                item.ItemCount = nTargetCount;
                item.Location = strPosition;

                arrCheckedItems.Add(item);
            }

            return true;
        }

        private bool LoadProcess(WebDBManager dbMgr, int nStepMemberID, Dictionary<int, Sections.Section> dicSections, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined, ref Dictionary<int, Sections.ExternalTeamData> dicExternal, ref Dictionary<int, string> dicRegular, ref Dictionary<int, string> dicControlRoom)
        {
            string strSQL = "select id, x, y, width, height, text, TeamList, ComponentID, ProcessTime, ProcessTimeType, useProcessTime, useMissionMessage";
            strSQL += ", onlyTeamLeader, CommanderMemberType, CommanderMemberID, CommanderDisplayText from Process where StepMemberID = " + nStepMemberID.ToString();

            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 15; i += 16)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                float x = WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0.0f);
                float y = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
                float fWidth = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
                float fHeight = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
                string strText = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strTeamList = WebDBManager.GetStringField(arrResult[i + 6], "");
                string strComponentID = WebDBManager.GetStringField(arrResult[i + 7], "");
                int nProcessTime = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 0);
                int nProcessTimeType = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 0);
                bool useProcessTime = WebDBManager.GetIntField(arrResult[i + 10].ToString(), 0) > 0 ? true : false;
                bool useMissionMessage = WebDBManager.GetIntField(arrResult[i + 11].ToString(), 0) > 0 ? true : false;
                bool onlyTeamLeader = WebDBManager.GetIntField(arrResult[i + 12].ToString(), 0) > 0 ? true : false;
                int nCommanderMemberType = WebDBManager.GetIntField(arrResult[i + 13].ToString(), -1);
                int nCommanderMemberID = WebDBManager.GetIntField(arrResult[i + 14].ToString(), -1);
                string strCommanderDisplayText = WebDBManager.GetStringField(arrResult[i + 15], "null");

                Sections.SectionProcess section = new Sections.SectionProcess();
                Sections.SectionDataProcess sectionData = (Sections.SectionDataProcess)section.Data;
                sectionData.ShowMessageBox = false;

                // ComponentID는 Type별로 중복될수 있으므로 ComponentType을 Int의 제일 첫번째 Byte를 ComponentType에 할당한다.
                int nKey = (((int)Sections.Section.ComponentType.PROCESS) << 24) | nID;
                dicSections[nKey] = section;

                section.TextUP = strText;
                section.TextDown = GetTeamList(dbMgr, strTeamList, sectionData, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom);
                //section.TextDown = GetTeamList(dbMgr, strTeamList, ref sectionData, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular);

                sectionData.ID = nID;
                sectionData.Title = strText;
                sectionData.ComponentID = strComponentID;
                sectionData.ProcessingTime.Time = nProcessTime;

                Sections.ProcessingTime.Type type = Sections.ProcessingTime.Type.UNKNOWN;
                if (!Sections.ProcessingTime.IntToType(nProcessTimeType, ref type))
                    return false;

                sectionData.ProcessingTime.ProcessingType = type;
                sectionData.UseProcessingTime = useProcessTime;
                sectionData.MissionTransfer = useMissionMessage;
                sectionData.TransferTeamLeaderOnly = onlyTeamLeader;

                if (!LoadProcessMission(dbMgr, nID, sectionData.MissionItems))
                    return false;

                //if (!LoadCheckedItems(dbMgr, nID, sectionData.CheckedItems))
                //    return false;
            }

            return true;
        }

        private string GetTeamList(WebDBManager dbMgr, string strTeamList, Models.SectionData sectionData, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined, ref Dictionary<int, Sections.ExternalTeamData> dicExternal, ref Dictionary<int, string> dicRegular, ref Dictionary<int, string> dicControlRoom)
        {
            int nBeginIndex = 0;
            int nLen = strTeamList.Length;

            string strTeamNameList = "";

            while (nBeginIndex < nLen)
            {
                int nDotIndex = strTeamList.IndexOf(',', nBeginIndex);
                if (nDotIndex < 0) break;

                if (!GetTeamName(dbMgr, ref sectionData, ref strTeamNameList, strTeamList, nBeginIndex, nDotIndex, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom))
                    return "";

                nBeginIndex = nDotIndex + 1;
            }

            if (!GetTeamName(dbMgr, ref sectionData, ref strTeamNameList, strTeamList, nBeginIndex, nLen, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom))
                return "";

            return strTeamNameList;
        }

        private bool GetTeamName(WebDBManager dbMgr,
            ref Models.SectionData sectionData,
            ref string strTeamNameList,
            string strTeamList,
            int nBeginIndex,
            int nEndIndex,
            ref Dictionary<int, string> dicNormal,
            ref Dictionary<int, string> dicEmergency,
            ref Dictionary<int, string> dicUserDefined,
            ref Dictionary<int, Sections.ExternalTeamData> dicExternal,
            ref Dictionary<int, string> dicRegular,
            ref Dictionary<int, string> dicControlRoom)
        {
            string strToken = strTeamList.Substring(nBeginIndex, nEndIndex - nBeginIndex);

            int nIndex1 = strTeamList.IndexOf('(', nBeginIndex);
            int nIndex2 = strTeamList.IndexOf(')', nBeginIndex);

            if (nIndex1 < 0 || nIndex2 < 0)
                return false;

            string strTeamID = strTeamList.Substring(nBeginIndex, nIndex1 - nBeginIndex);
            string strTeamType = strTeamList.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);

            strTeamID = strTeamID.Trim();
            strTeamType = strTeamType.Trim();

            // TeamID, TeamName
            Dictionary<int, string> dicTeamName = null;
            // TeamID, RegularTeamID List
            //Dictionary<int, ArrayList> dicRegualrTeamID = null;
            string strTeamName = null;
            ArrayList arrLinkedMembers = new ArrayList();

            bool includeChildTeams = true;
            int nTeamID;

            if (!int.TryParse(strTeamID, out nTeamID))
                return false;

            if (nTeamID < 0)
            {
                nTeamID = -nTeamID;
                includeChildTeams = false;
            }

            if (strTeamType == "0")
            {
                if (dicNormal == null || m_dicTemporaryNormalMemberID == null)
                {
                    if (dicNormal == null)
                        dicNormal = new Dictionary<int, string>();

                    if (m_dicTemporaryNormalMemberID == null)
                        m_dicTemporaryNormalMemberID = new Dictionary<int, List<TemporaryMember>>();

                    ReadTeamList(dbMgr, "TemporaryNormalTeam", true, dicNormal, ref m_dicTemporaryNormalMemberID);
                }

                dicTeamName = dicNormal;

                if (m_dicTemporaryNormalMemberID == null)
                    m_dicTemporaryNormalMemberID = new Dictionary<int, List<TemporaryMember>>();
                else
                {
                    List<TemporaryMember> members;

                    if (m_dicTemporaryNormalMemberID.TryGetValue(nTeamID, out members))
                    {
                        arrLinkedMembers.AddRange(members);
                    }
                }
                //dicRegualrTeamID = m_dicNormalRegularTeamID;
            }
            else if (strTeamType == "1")
            {
                if (dicEmergency == null || m_dicTemporaryEmergencyMemberID == null)
                {
                    if (dicEmergency == null)
                        dicEmergency = new Dictionary<int, string>();

                    if (m_dicTemporaryEmergencyMemberID == null)
                        m_dicTemporaryEmergencyMemberID = new Dictionary<int, List<TemporaryMember>>();

                    ReadTeamList(dbMgr, "TemporaryEmergencyTeam", false, dicEmergency, ref m_dicTemporaryEmergencyMemberID);
                }

                dicTeamName = dicEmergency;

                if (m_dicTemporaryEmergencyMemberID == null)
                    m_dicTemporaryEmergencyMemberID = new Dictionary<int, List<TemporaryMember>>();
                else
                {
                    List<TemporaryMember> members;

                    if (m_dicTemporaryEmergencyMemberID.TryGetValue(nTeamID, out members))
                    {
                        arrLinkedMembers.AddRange(members);
                    }
                }
                //dicRegualrTeamID = m_dicEmergencyRegularTeamID;
            }
            else if (strTeamType == "2")
            {
                if (!dicExternal.ContainsKey(nTeamID))
                    return false;

                strTeamName = dicExternal[nTeamID].TeamName;
            }
            else if (strTeamType == "3")
            {
                if (dicUserDefined == null)
                {
                    dicUserDefined = new Dictionary<int, string>();
                    ReadTeamList(dbMgr, "UserDefinedTeam", dicUserDefined);
                }

                dicTeamName = dicUserDefined;
            }
            else if (strTeamType == "4")
            {
                if (dicRegular == null)
                {
                    dicRegular = new Dictionary<int, string>();
                    ReadTeamList(dbMgr, "RegularTeam", dicRegular);
                }

                dicTeamName = dicRegular;
            }
            else if (strTeamType == "10")
            {
                if (dicControlRoom == null)
                {
                    dicControlRoom = new Dictionary<int, string>();
                    ReadTeamList(dbMgr, "ControlRoom", dicControlRoom);
                }

                dicTeamName = dicControlRoom;
            }
            else
                return false;

            if (strTeamName == null)
            {
                if (!dicTeamName.ContainsKey(nTeamID))
                    return false;

                strTeamName = dicTeamName[nTeamID];
            }

            if (strTeamNameList.Length == 0)
                strTeamNameList = strTeamName;
            else
                strTeamNameList += ", " + strTeamName;

            int nLevelNo = GetLevelNumber(dbMgr, nTeamID, strTeamType);
            Sections.SOPTeam team = new Sections.SOPTeam();

            team.TeamID = nTeamID;
            team.TeamType = (Sections.SOPTeam.SOPTeamType)int.Parse(strTeamType);
            team.TeamName = strTeamName;
            team.LevelNo = nLevelNo;
            team.LinkedMembers = arrLinkedMembers;

            if (team.TeamType == Sections.SOPTeam.SOPTeamType.Regular || team.TeamType == Sections.SOPTeam.SOPTeamType.External ||
                team.TeamType == Sections.SOPTeam.SOPTeamType.Normal || team.TeamType == Sections.SOPTeam.SOPTeamType.Holiday)
                team.IncludeChildTeams = includeChildTeams;

            if (sectionData is Sections.SectionDataProcess)
            {
                ((Sections.SectionDataProcess)sectionData).TeamList.Add(team);
            }
            else if (sectionData is Sections.SectionDataInternal)
            {
                ((Sections.SectionDataInternal)sectionData).TeamList.Add(team);
            }

            return true;
        }

        private int GetLevelNumber(WebDBManager dbMgr, int nTeamID, string strTeamType)
        {
            int nLevelNo = -1;
            string strSQL = "";

            if (strTeamType == "0")
            {
                //strSQL = "select ID, TeamName, LevelNo from TemporaryNormalTeam where ID = " + nTeamID.ToString();
                strSQL = "select team.ID, TeamName, link.MemberID from TemporaryNormalTeam as team, TemporaryMemberList as link where team.ID = link.TemporaryTeamID and link.IsNormal = 1 and link.MemberType = 6 and team.ID = {0} and SiteID = {1}";
                strSQL = string.Format(strSQL, nTeamID, dbMgr.SiteID);
            }
            else if (strTeamType == "1")
            {
                //strSQL = "select ID, TeamName, LevelNo from TemporaryEmergencyTeam where ID = " + nTeamID.ToString();
                strSQL = "select team.ID, TeamName, link.MemberID from TemporaryEmergencyTeam as team, TemporaryMemberList as link where team.ID = link.TemporaryTeamID and link.IsNormal = 0 and link.MemberType = 6 and team.ID = {0} and SiteID = {1}";
                strSQL = string.Format(strSQL, nTeamID, dbMgr.SiteID);
            }
            else
                return nLevelNo;

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return -1;

            int nResultCount = arrResult.Count;
            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                nLevelNo = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
            }

            return nLevelNo;
        }

        // dicTeamName : TeamID, TeamName
        private bool ReadTeamList(WebDBManager dbMgr, string strTableName, bool isNormal, Dictionary<int, string> dicTeamName, ref Dictionary<int, List<TemporaryMember>> dicTemporaryMembers)
        {
            string strFormat = "select team.ID, TeamName, link.MemberID, link.MemberType, link.IsTeamLeader, link.Role, link.MemberName from {0} as team, TemporaryMemberList as link ";
            strFormat += "where link.TemporaryTeamID = team.ID and link.IsNormal = {1} and team.SiteID = {2}";

            string strSQL = string.Format(strFormat, strTableName, isNormal ? 1 : 0, dbMgr.SiteID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            List<TemporaryMember> members;
            TemporaryMember.MemberType memberType;
            TemporaryMember.RoleType roleType;

            int nResultCount = arrResult.Count;

            List<int> teamIDs = new List<int>();

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                int nTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nTeamLeader = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nRoleType = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 6].ToString(), "");

                if (nTeamID < 0 || nMemberID < 0)
                    continue;

                if (!TemporaryMember.GetMemberType(nMemberType, out memberType))
                    continue;

                if (!TemporaryMember.GetRoleType(nRoleType, out roleType))
                    roleType = TemporaryMember.RoleType.Unknown;

                if (strMemberName == "null")
                    strMemberName = "";

                if (!teamIDs.Contains(nTeamID))
                    teamIDs.Add(nTeamID);

                dicTeamName[nTeamID] = strTeamName;

                if (!dicTemporaryMembers.TryGetValue(nTeamID, out members))
                {
                    members = new List<TemporaryMember>();
                    dicTemporaryMembers[nTeamID] = members;
                }

                TemporaryMember member = new TemporaryMember(nTeamID, isNormal, nMemberID, nTeamLeader, memberType, roleType, strMemberName);
                members.Add(member);
            }

            strSQL = "select ID, TeamName from TemporaryNormalTeam where SiteID = " + dbMgr.SiteID.ToString();

            if (teamIDs.Count > 0)
            {
                strSQL += " and ID not in (";

                string strIDs = "";

                foreach (int nID in teamIDs)
                {
                    if (strIDs.Length == 0)
                        strIDs = nID.ToString();
                    else
                        strIDs += ", " + nID.ToString();
                }

                strSQL += strIDs + ")";
            }

            arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");

                dicTeamName[nTeamID] = strTeamName;
            }

            return true;
        }

        // dicTeamName : TeamID, TeamName
        private bool ReadTeamList(WebDBManager dbMgr, string strTableName, Dictionary<int, string> dicTeamName)
        {
            if (strTableName == "RegularTeam")
            {
                // Site별로 사용할 수 있도록 수정 , Edit by skkim 2015.01.09
                // SiteID로 본부 아이디를 가져온다.
                string szSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", dbMgr.SiteID);
                ArrayList arrResult1 = dbMgr.GetResultData(szSQL);
                if (arrResult1 == null || arrResult1.Count == 0)
                    return false;

                int nTopTeamID = WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
                if (nTopTeamID == -1)
                    return false;

                ArrayList arrResult = ExecuteTeamList(dbMgr, nTopTeamID);

                for (int i = 0; i < arrResult.Count - 2; i += 3)
                {
                    int nTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                    string strTeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
                    dicTeamName[nTeamID] = strTeamName;
                }
            }
            else
            {
                string strSQL = "select id, TeamName from " + strTableName;
                strSQL += " WHERE SiteID = " + dbMgr.SiteID.ToString();

                ArrayList arrResult = dbMgr.GetResultData(strSQL);
                if (arrResult == null)
                    return false;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 1; i += 2)
                {
                    int nTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                    string strTeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
                    dicTeamName[nTeamID] = strTeamName;
                }
            }
            return true;
        }

        public static ArrayList ExecuteTeamList(WebDBManager dbMgr, int nRootTeamID, string strTableName = "RegularTeam")
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

        private string GetMemberName(WebDBManager dbMgr, int nID, string strTableName, string strFieldName)
        {
            string strSQL = string.Format("Select {0} from {1} where ID = {2}", strFieldName, strTableName, nID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return "";

            string strMemberName = WebDBManager.GetStringField(arrResult[0], "null");

            if (strMemberName == "null")
                strMemberName = "";

            return strMemberName;
        }

        private string GetTemporaryMemberName(WebDBManager dbMgr, int nID)
        {
            string strSQL = "Select MemberName from TemporaryMemberList where ID = " + nID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return "";

            string strMemberName = WebDBManager.GetStringField(arrResult[0], "null");

            if (strMemberName == "null")
                strMemberName = "";

            return strMemberName;
        }

        private string GetTeamLeaderName(WebDBManager dbMgr, int nTeamID, string strTableName, string strAdd = "")
        {
            string strSQL = "Select TeamName from " + strTableName + " where ID = " + nTeamID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return "";

            string strTeamName = WebDBManager.GetStringField(arrResult[0], "null");

            if (strTeamName == "null")
                return "";

            return strTeamName + strAdd;
        }

        private bool LoadDecision(WebDBManager dbMgr, int nStepMemberID, Dictionary<int, Sections.Section> dicSections)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID from Decision where StepMemberID = " + nStepMemberID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                float x = WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0.0f);
                float y = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
                float fWidth = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
                float fHeight = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
                string strText = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strComponentID = WebDBManager.GetStringField(arrResult[i + 6], "");

                Sections.SectionDecision section = new Sections.SectionDecision();
                Sections.SectionDataDecision sectionData = (Sections.SectionDataDecision)section.Data;
                sectionData.ShowMessageBox = false;

                // ComponentID는 Type별로 중복될수 있으므로 ComponentType을 Int의 제일 첫번째 Byte를 ComponentType에 할당한다.
                int nKey = (((int)Sections.Section.ComponentType.DECISION) << 24) | nID;
                dicSections[nKey] = section;

                section.Title = strText;

                sectionData.ID = nID;
                sectionData.Title = strText;
                sectionData.ComponentID = strComponentID;
            }

            return true;
        }

        private bool LoadEndPoint(WebDBManager dbMgr, int nStepMemberID, Dictionary<int, Sections.Section> dicSections)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID, isBegin from EndPoint where StepMemberID = " + nStepMemberID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 7; i += 8)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                float x = WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0.0f);
                float y = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
                float fWidth = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
                float fHeight = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
                string strText = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strComponentID = WebDBManager.GetStringField(arrResult[i + 6], "");
                bool isBegin = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0) == 0 ? false : true;

                Sections.SectionEndPoint section = new Sections.SectionEndPoint();
                Sections.SectionDataEndPoint sectionData = (Sections.SectionDataEndPoint)section.Data;
                sectionData.ShowMessageBox = false;

                // ComponentID는 Type별로 중복될수 있으므로 ComponentType을 Int의 제일 첫번째 Byte를 ComponentType에 할당한다.
                int nKey = (((int)Sections.Section.ComponentType.ENDPOINT) << 24) | nID;
                dicSections[nKey] = section;

                section.Title = strText;

                sectionData.ID = nID;
                sectionData.Title = strText;
                sectionData.ComponentID = strComponentID;
                sectionData.IsBegin = isBegin;
            }

            return true;
        }

        private bool LoadInternal(WebDBManager dbMgr, int nStepMemberID, Dictionary<int, Sections.Section> dicSections, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined, ref Dictionary<int, Sections.ExternalTeamData> dicExternal, ref Dictionary<int, string> dicRegular, ref Dictionary<int, string> dicControlRoom)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID, usePopupMessage, useMobileApp, useBroadcast, TeamList, CommanderMemberType, CommanderMemberID, CommanderDisplayText from InternalTransmission where StepMemberID = " + nStepMemberID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 13; i += 14)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                float x = WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0.0f);
                float y = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
                float fWidth = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
                float fHeight = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
                string strText = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strComponentID = WebDBManager.GetStringField(arrResult[i + 6], "");
                bool usePopupMessage = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0) == 0 ? false : true;
                bool useMobileApp = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 0) == 0 ? false : true;
                bool useBroadcast = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 0) == 0 ? false : true;
                string strTeamList = WebDBManager.GetStringField(arrResult[i + 10]);
                int nCommanderMemberType = WebDBManager.GetIntField(arrResult[i + 11].ToString(), -1);
                int nCommanderMemberID = WebDBManager.GetIntField(arrResult[i + 12].ToString(), -1);
                string strCommanderDisplayText = WebDBManager.GetStringField(arrResult[i + 13], "null");

                Sections.SectionInternal section = new Sections.SectionInternal();
                Sections.SectionDataInternal sectionData = (Sections.SectionDataInternal)section.Data;
                sectionData.ShowMessageBox = false;

                // ComponentID는 Type별로 중복될수 있으므로 ComponentType을 Int의 제일 첫번째 Byte를 ComponentType에 할당한다.
                int nKey = (((int)Sections.Section.ComponentType.INTERNAL) << 24) | nID;
                dicSections[nKey] = section;

                section.Title = strText;

                sectionData.ID = nID;
                sectionData.Title = strText;
                sectionData.ComponentID = strComponentID;
                sectionData.UsePopupMessage = usePopupMessage;
                sectionData.UseMobileApp = useMobileApp;
                sectionData.UseBroadcast = useBroadcast;

                if (!useBroadcast && strTeamList != null)
                {
                    string strTeamListName = GetTeamList(dbMgr, strTeamList, sectionData, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom);
                    sectionData.TeamList.Add(strTeamListName);
                }
            }

            return true;
        }

        private bool LoadExternal(WebDBManager dbMgr, int nStepMemberID, Dictionary<int, Sections.Section> dicSections)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID, useSMS, SMSText, SMSExternalTeamIDList, useEFax, FaxExternalTeamIDList from ExternalTransmission where StepMemberID = " + nStepMemberID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 11; i += 12)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                float x = WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0.0f);
                float y = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
                float fWidth = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
                float fHeight = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
                string strText = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strComponentID = WebDBManager.GetStringField(arrResult[i + 6], "");
                bool useSMS = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0) == 0 ? false : true;
                string strSMSText = WebDBManager.GetStringField(arrResult[i + 8], "");
                string strSMSExternalTeamIDList = WebDBManager.GetStringField(arrResult[i + 9], "");
                bool useEFax = WebDBManager.GetIntField(arrResult[i + 10].ToString(), 0) == 0 ? false : true;
                string strFaxExternalTeamIDList = WebDBManager.GetStringField(arrResult[i + 11], "");

                Sections.SectionExternal section = new Sections.SectionExternal();
                Sections.SectionDataExternal sectionData = (Sections.SectionDataExternal)section.Data;
                sectionData.ShowMessageBox = false;

                // ComponentID는 Type별로 중복될수 있으므로 ComponentType을 Int의 제일 첫번째 Byte를 ComponentType에 할당한다.
                int nKey = (((int)Sections.Section.ComponentType.EXTERNAL) << 24) | nID;
                dicSections[nKey] = section;

                section.Title = strText;

                sectionData.ID = nID;
                sectionData.Title = strText;
                sectionData.ComponentID = strComponentID;
                sectionData.UseSMS = useSMS;
                sectionData.UseFax = useEFax;
                sectionData.SMSMessage = strSMSText;
            }

            return true;
        }

        public bool LoadSections(WebDBManager dbMgr, int nStepMemberID, Dictionary<int, Sections.Section> dicSections, int nActionStepID)
        {
            Dictionary<int, string> dicNormal = null;
            Dictionary<int, string> dicEmergency = null;
            Dictionary<int, string> dicUserDefined = null;
            Dictionary<int, Sections.ExternalTeamData> dicExternal = null;
            Dictionary<int, string> dicRegular = null;
            Dictionary<int, string> dicControlRoom = null;

            if (!LoadProcess(dbMgr, nStepMemberID, dicSections, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom))
                return false;
            if (!LoadDecision(dbMgr, nStepMemberID, dicSections))
                return false;
            if (!LoadEndPoint(dbMgr, nStepMemberID, dicSections))
                return false;
            if (!LoadInternal(dbMgr, nStepMemberID, dicSections, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom))
                return false;
            if (!LoadExternal(dbMgr, nStepMemberID, dicSections))
                return false;

            if (!LoadArrow(dbMgr, nStepMemberID, dicSections))
                return false;

            SetSectionNumber(dicSections, nActionStepID);
            return true;
        }

        private void SetSectionNumber(Dictionary<int, Sections.Section> dicSections, int nActionStepID)
        {
            Sections.SectionEndPoint sectionBegin = null;

            foreach (KeyValuePair<int, Sections.Section> pair in dicSections)
            {
                Sections.Section section = pair.Value;

                if (section.GetComponentType() == Sections.Section.ComponentType.ENDPOINT)
                {
                    Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)section.Data;

                    if (data.IsBegin)
                    {
                        sectionBegin = (Sections.SectionEndPoint)section;
                        break;
                    }
                }
            }

            if (sectionBegin == null)
                return;

            List<Sections.Section> allSections = new List<Sections.Section>();
            Dictionary<int, List<Sections.Section>> dicDepthSections = new Dictionary<int, List<Sections.Section>>();
            int depth = 1, number = 1;

            SetSectionNumber(sectionBegin, allSections, dicDepthSections, depth);

            for (int i = depth; ; i++)
            {
                List<Sections.Section> sections = null;

                if (!dicDepthSections.TryGetValue(i, out sections))
                    break;

                foreach (Sections.Section section in sections)
                {
                    if (section.Data.SectionNumber < 0)
                    {
                        section.Data.SectionNumber = number++;
                        SOPHistoryManager.Instance.SetSectionNumber(nActionStepID, (int)section.GetComponentType(), section.Data.ID, section.Data.SectionNumber);
                    }
                }
            }
        }

        private void SetSectionNumber(Sections.Section section, List<Sections.Section> allSections, Dictionary<int, List<Sections.Section>> dicDepthSections, int depth)
        {
            List<Sections.Section> depthSections = null;

            if (!dicDepthSections.TryGetValue(depth, out depthSections))
            {
                depthSections = new List<Sections.Section>();
                dicDepthSections[depth] = depthSections;
            }

            foreach (Sections.Arrow arrow in section.Arrows)
            {
                if (arrow.BeginLink != section || arrow.EndLink == null)
                    continue;

                Sections.Section.ComponentType type = arrow.EndLink.GetComponentType();

                // 시작을 제외한 종료 Section들은 번호를 가지도록 수정
                if (/*type == Sections.Section.ComponentType.ENDPOINT ||*/
                    type == Sections.Section.ComponentType.ANNOTATION ||
                    type == Sections.Section.ComponentType.LINK)
                    continue;

                if (allSections.Contains(arrow.EndLink))
                    continue;

                allSections.Add(arrow.EndLink);
                depthSections.Add(arrow.EndLink);

                SetSectionNumber(arrow.EndLink, allSections, dicDepthSections, depth + 1);
            }
        }

        private bool LoadArrow(WebDBManager dbMgr, int nStepMemberID, Dictionary<int, Sections.Section> dicSections)
        {
            string strSQL = "select ID, BeginComponentID, BeginComponentPosition, EndComponentID, EndComponentPosition from Arrow where StepMemberID = " + nStepMemberID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            Sections.Section sectionBegin, sectionEnd;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                int nBeginComponentID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
                int nBeginComponentPosition = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                int nEndComponentID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0);
                int nEndComponentPosition = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);

                if (!dicSections.TryGetValue(nBeginComponentID, out sectionBegin) || !dicSections.TryGetValue(nEndComponentID, out sectionEnd))
                    continue;

                Sections.Arrow arrow = new Sections.Arrow();

                arrow.BeginLink = sectionBegin;
                arrow.EndLink = sectionEnd;

                Sections.Arrow.ArrowPosition posBegin, posEnd;

                if (!Sections.Arrow.IntToArrowPosition(nBeginComponentPosition, out posBegin))
                    return false;
                if (!Sections.Arrow.IntToArrowPosition(nEndComponentPosition, out posEnd))
                    return false;

                arrow.BeginPosition = posBegin;
                arrow.EndPosition = posEnd;

                sectionBegin.AddArrow(arrow);
                sectionEnd.AddArrow(arrow);
            }

            return true;
        }

        public static void GetProcessCheckedNotify(Sections.SectionProcess section, out int nCheckedNotify1, out int nCheckedNotify2)
        {
            nCheckedNotify1 = 0;
            nCheckedNotify2 = 0;

            Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;
            if (data == null)
                return;

            int nMissionCount = (int)data.MissionItems.Count;

            for (int i = 0; i < nMissionCount; i++)
            {
                int nSMSFlag = 0;
                int nBroadcastFlag = 0;// 1 << i;

                nCheckedNotify1 |= nSMSFlag;
                nCheckedNotify2 |= nBroadcastFlag;
            }
        }

        public static void GetInternalCheckedNotify(Sections.SectionInternal section, out int nCheckedNotify1)
        {
            nCheckedNotify1 = 0;

            Sections.SectionDataInternal data = (Sections.SectionDataInternal)section.Data;
            if (data == null)
                return;

            if (data.UsePopupMessage)
                nCheckedNotify1 |= 1;

            if (data.UseMobileApp)
                nCheckedNotify1 |= 2;

            if (data.UseBroadcast)
                nCheckedNotify1 |= 4;
        }

        public static void GetExternalCheckedNotify(Sections.SectionExternal section, out int nCheckedNotify1, out int nCheckedNotify2)
        {
            nCheckedNotify1 = 0;
            nCheckedNotify2 = 0;
            Sections.SectionDataExternal data = (Sections.SectionDataExternal)section.Data;
            if (data == null)
                return;

            int nIdx = 0;
            int nBit = 0;
            if (data.UseSMS)
            {
                foreach (Sections.ExternalTeamData exTeam in data.SMSReceivers)
                {
                    nBit = 1 << nIdx;
                    nCheckedNotify1 |= nBit;
                    nIdx++;
                    if (nIdx == 16)
                        break;
                }
            }
            else
            {
                nCheckedNotify1 = 0;
            }

            nIdx = 0;
            if (data.UseFax)
            {
                foreach (Sections.ExternalTeamData exTeam in data.FaxReceivers)
                {
                    nBit = 1 << nIdx;
                    nCheckedNotify2 |= nBit;
                    nIdx++;
                    if (nIdx == 16)
                        break;
                }
            }
            else
            {
                nCheckedNotify2 = 0;
            }

            if (data.UseSMS)
                nCheckedNotify1 |= (1 << 31);

            if (data.UseFax)
                nCheckedNotify2 |= (1 << 31);
        }
    }

    // TemporaryMemberList의 데이터를 표현
    public class TemporaryMember
    {
        public enum MemberType
        {
            RegularTeam = 0,
            CompanyMember,
            ExternalCompanyTeam,    // 외부 협력사의 팀
            ExternalTeam,           // 외부 협력사
            ExternalCompanyMember,
            UserDefinedTeam,
            JobLevel,               // 직급, 1이면 1직급, 2면 2직급
            Unknown
        }

        //역할 : 0(정), 1(부), 2(팀장), 3(일반)
        public enum RoleType { Main = 0, Sub, TeamLeader, General, Unknown };

        private int m_nTemporaryTeamID = -1;
        private bool m_isNormal = true;
        private int m_nMemberID = -1;
        // 1이면 팀장, 0이면 팀원이며 0보다 작으면 null 값이다.
        private int m_nTeamLeader = -1;
        private MemberType m_memberType = MemberType.Unknown;
        private RoleType m_roleType = RoleType.Unknown;
        private string m_strMemberName = "";
        // 하위팀을 포함하는가?
        private bool m_includeChildTeams = true;

        public int TemporaryTeamID
        {
            get { return m_nTemporaryTeamID; }
            set { m_nTemporaryTeamID = value; }
        }

        public bool IsNormal
        {
            get { return m_isNormal; }
            set { m_isNormal = value; }
        }

        public int MemberID
        {
            get { return m_nMemberID; }
            set { m_nMemberID = value; }
        }

        // 1이면 팀장, 0이면 팀원이며 0보다 작으면 null 값이다.
        public int TeamLeader
        {
            get { return m_nTeamLeader; }
            set { m_nTeamLeader = value; }
        }

        public MemberType _MemberType
        {
            get { return m_memberType; }
            set { m_memberType = value; }
        }

        public RoleType _RoleType
        {
            get { return m_roleType; }
            set { m_roleType = value; }
        }

        public string MemberName
        {
            get { return m_strMemberName; }
            set { m_strMemberName = value; }
        }

        public bool IncludeChildTeams
        {
            get { return m_includeChildTeams; }
            set { m_includeChildTeams = value; }
        }

        public TemporaryMember()
        {
        }

        public TemporaryMember(int nTemporaryTeamID, bool isNormal, int nMemberID, int nTeamLeader, MemberType memberType, RoleType roleType, string strMemberName)
        {
            m_nTemporaryTeamID = nTemporaryTeamID;
            m_isNormal = isNormal;
            m_nMemberID = nMemberID;
            m_nTeamLeader = nTeamLeader;
            m_memberType = memberType;
            m_roleType = roleType;
            m_strMemberName = strMemberName;
        }

        public static bool GetMemberType(int nMemberType, out MemberType memberType)
        {
            if (nMemberType < 0 || nMemberType >= (int)MemberType.Unknown)
            {
                memberType = MemberType.Unknown;
                return false;
            }

            memberType = (MemberType)nMemberType;
            return true;
        }

        public static bool GetRoleType(int nRoleType, out RoleType roleType)
        {
            if (nRoleType < 0 || nRoleType >= (int)RoleType.Unknown)
            {
                roleType = RoleType.Unknown;
                return false;
            }

            roleType = (RoleType)nRoleType;
            return true;
        }

        public static string GetRoleTypeString(RoleType roleType)
        {
            if (roleType == RoleType.Main)
                return "정";
            else if (roleType == RoleType.Sub)
                return "부";

            return "";
        }
    }
}
