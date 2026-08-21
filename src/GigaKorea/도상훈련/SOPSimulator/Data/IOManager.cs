using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using DBUtility;

using UnE.SOP;
using UnE.SOP.Workstate;
using UnE.SOP.Sections;

namespace SOPMonitoringSystem
{
	// SOP Data를 DB에 저장 및 불러오기 담당
	public class IOManager
	{
        private Dictionary<int, List<TemporaryMember>> m_dicTemporaryNormalMemberID = null;
        private Dictionary<int, List<TemporaryMember>> m_dicTemporaryEmergencyMemberID = null;
		//private Dictionary<int, ArrayList> m_dicNormalRegularTeamID = null;
		//private Dictionary<int, ArrayList> m_dicEmergencyRegularTeamID = null;
		// ExternalTeam ID, TeamData
		private static Dictionary<int, Sections.ExternalTeamData> m_dicExternal = null;
        private static Sections.SOPTeam m_teamRegularRoot = null;

        // Key : 상위 4바이트 : Team Type, 하위 4바이트 : Member ID
        private static Dictionary<long, Sections.SectionCommander> m_dicSectionCommanders = new Dictionary<long, Sections.SectionCommander>();

        private Dictionary<int, ConfigData> m_dicUserDefinedConfig = new Dictionary<int, ConfigData>();

		public IOManager()
		{
		}

		public bool Load(FormSOP frm, WebDBManager dbMgr, VersionInfo version, ArrayList arrActionSteps, string strCategoryName, string strSubCategoryName, string strDisasterName)
		{
            m_dicUserDefinedConfig.Clear();
            ClearSectionCommanders();
			//ClearSOP(frm);

			frm.Cursor = Cursors.WaitCursor;

			PageBackstageSOP pageHome = frm.GetPageHome();

			ArrayList arrTeams = LoadBarPage(pageHome, arrActionSteps, dbMgr);
			if (arrTeams == null)
			{
				frm.Cursor = Cursors.Arrow;
				return false;
			}

			if (!LoadPane(dbMgr, pageHome, arrActionSteps, arrTeams))
			{
				frm.Cursor = Cursors.Arrow;
				return false;
			}

			frm.Cursor = Cursors.Arrow;
			return true;
		}


		char szDeli = (char)0x06;
		private string GetFirstActionStepFullPath(string strCategoryName, string strSubCategoryName, string strDisasterName, ArrayList arrActionSteps)
		{
			string strFullPath = strCategoryName + szDeli + strSubCategoryName + szDeli + strDisasterName;

			if (arrActionSteps.Count == 0)
				return strFullPath;

			ActionStepInfo actionStep = (ActionStepInfo)arrActionSteps[0];

			if (actionStep.ParentStepID < 0)
				return strFullPath + szDeli + actionStep.ActionStepName;

			return GetActionStepFullPath(strFullPath, actionStep.ParentStepID, arrActionSteps);
		}

		private string GetActionStepFullPath(string strPath, int nParentID, ArrayList arrActionSteps)
		{
			if (nParentID < 0)
				return strPath;

			foreach (ActionStepInfo actionStep in arrActionSteps)
			{
				if (actionStep.ActionStepID == nParentID)
				{
					strPath = actionStep.ActionStepName + szDeli + strPath;

					if (actionStep.ParentStepID < 0)
						return strPath;
					else
						return GetActionStepFullPath(strPath, actionStep.ParentStepID, arrActionSteps);
				}
			}

			return strPath;
		}

		// arrTeamID : 하부 조직을 포함하지 않는 팀 ID List
		// arrTeamGroupID : 하부 조직을 포함하는 팀 ID LIst
		/*private bool AddRegularTeamID(string strRegularTeamLink, int nBeginIndex, int nEndIndex, ArrayList arrTeamID, ArrayList arrTeamGroupID)
		{
			string strID = strRegularTeamLink.Substring(nBeginIndex, nEndIndex - nBeginIndex);
			strID = Utility.TrimString(strID);

			if (strID.Length == 0)
				return true;

			try
			{
				int nID = -1;
				int.TryParse(strID, out nID);
				//int nID = int.Parse(strID);

				if (nID > 0)
				{
					// 중복은 허용하지 않는다.
					if (!arrTeamGroupID.Contains(nID))
						arrTeamGroupID.Add(nID);
				}
				else
				{
					// 중복은 허용하지 않는다.
					if (!arrTeamID.Contains(-nID))
						arrTeamID.Add(-nID);
				}
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}

		private void ReadRegularTeamID(WebDBManager dbMgr, string strRegularTeamLink, ArrayList arrRegularTeamIDList)
		{
			// 하부 조직을 포함하지 않는 팀 ID List
			ArrayList arrTeamID = new ArrayList();
			// 하부 조직을 포함하는 팀 ID LIst
			ArrayList arrTeamGroupID = new ArrayList();

			int nLen = strRegularTeamLink.Length;
			if (nLen == 0)
				return;

			int nBeginIndex = 0;

			while (true)
			{
				int nCommaIndex = strRegularTeamLink.IndexOf(',', nBeginIndex);
				if (nCommaIndex < 0)
					break;

				if (!AddRegularTeamID(strRegularTeamLink, nBeginIndex, nCommaIndex, arrTeamID, arrTeamGroupID))
					return;

				nBeginIndex = nCommaIndex + 1;
			}

			if (!AddRegularTeamID(strRegularTeamLink, nBeginIndex, nLen, arrTeamID, arrTeamGroupID))
				return;

			foreach (int nTeamID in arrTeamGroupID)
			{
				string strSQL = "EXEC sp_teamList2 " + nTeamID.ToString();
				ArrayList arrResult = dbMgr.GetStoredProcedureData(strSQL, 0);

				if (arrResult == null)
					return;

				int nResultCount = arrResult.Count;

				for (int i = 0; i < nResultCount - 2; i += 3)
				{
					int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);

					if (!arrTeamID.Contains(nID))
						arrTeamID.Add(nID);
				}
			}

			foreach (int nTeamID in arrTeamID)
			{
				arrRegularTeamIDList.Add(nTeamID);
			}

			arrTeamID.Clear();
			arrTeamGroupID.Clear();
		}*/

        public static bool ReadTemporaryTeamMemberList(WebDBManager dbMgr, bool isNormal, bool includeChildTeams, int nTeamID/*, bool parentTeamID*/, List<TemporaryMember> members)
        {
            if (isNormal)
            {
                Data_NormalTeam team = DataManager.Instance.GetTemporaryNormalTeam(nTeamID);

                if (team == null)
                    return false;

                if (ReadTemporaryTeamMemberList(dbMgr, isNormal, nTeamID, members) == false)
                    return false;

                foreach (Data_NormalTeam childTeam in team.ChildTeams)
                {
                    if (ReadTemporaryTeamMemberList(dbMgr, isNormal, includeChildTeams, childTeam.ID, members) == false)
                        return false;
                }
            }
            else
            {
                Data_EmergencyTeam team = DataManager.Instance.GetTemporaryEmergencyTeam(nTeamID);

                if (team == null)
                    return false;

                if (ReadTemporaryTeamMemberList(dbMgr, isNormal, nTeamID, members) == false)
                    return false;

                foreach (Data_EmergencyTeam childTeam in team.ChildTeams)
                {
                    if (ReadTemporaryTeamMemberList(dbMgr, isNormal, includeChildTeams, childTeam.ID, members) == false)
                        return false;
                }
            }
            /*string strTableName = isNormal ? "TemporaryNormalTeam" : "TemporaryEmergencyTeam";
            string strFormat = "select team.ID, TeamName, link.MemberID, link.MemberType, link.IsTeamLeader, link.Role, link.MemberName from {0} as team, TemporaryMemberList as link ";
            strFormat += "where link.TemporaryTeamID = team.ID and link.IsNormal = {1} and team.SiteID = {2}";// and team.ID = {3}";

            if (parentTeamID)
                strFormat += " and team.ParentTeamID = {3}";
            else
                strFormat += " and team.ID = {3}";

            string strSQL = string.Format(strFormat, strTableName, isNormal ? 1 : 0, ProxySOP.Instance.SiteID, nTeamID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            TemporaryMember.MemberType memberType;
            TemporaryMember.RoleType roleType;

            int nResultCount = arrResult.Count;
            
            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                int _nTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nTeamLeader = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nRoleType = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 6].ToString(), "");

                bool _includeChildTeams = true;

                if (nMemberID < 0)
                {
                    nMemberID = -nMemberID;
                    _includeChildTeams = false;
                }

                if (_nTeamID < 0 || nMemberID < 0)
                    continue;

                if (!TemporaryMember.GetMemberType(nMemberType, out memberType))
                    continue;

                if (!TemporaryMember.GetRoleType(nRoleType, out roleType))
                {
                    roleType = TemporaryMember.RoleType.Unknown;
                    //continue;
                }

                if (strMemberName == "null")
                    strMemberName = "";

                TemporaryMember member = new TemporaryMember(_nTeamID, isNormal, nMemberID, nTeamLeader, memberType, roleType, strMemberName);
                members.Add(member);

                if (memberType == TemporaryMember.MemberType.ExternalCompanyTeam ||
                    memberType == TemporaryMember.MemberType.ExternalTeam ||
                    memberType == TemporaryMember.MemberType.RegularTeam)
                    member.IncludeChildTeams = _includeChildTeams;

                if (includeChildTeams)
                {
                    if (ReadTemporaryTeamMemberList(dbMgr, isNormal, includeChildTeams, _nTeamID, true, members) == false)
                        return false;
                }
            }*/

            return true;
        }

        private static bool ReadTemporaryTeamMemberList(WebDBManager dbMgr, bool isNormal, int nTeamID, List<TemporaryMember> members)
        {
            string strTableName = isNormal ? "TemporaryNormalTeam" : "TemporaryEmergencyTeam";
            string strFormat = "select team.ID, TeamName, link.MemberID, link.MemberType, link.IsTeamLeader, link.Role, link.MemberName from {0} as team, TemporaryMemberList as link ";
            strFormat += "where link.TemporaryTeamID = team.ID and link.IsNormal = {1} and team.SiteID = {2} and team.ID = {3}";

            string strSQL = string.Format(strFormat, strTableName, isNormal ? 1 : 0, ProxySOP.Instance.SiteID, nTeamID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            TemporaryMember.MemberType memberType;
            TemporaryMember.RoleType roleType;

            int nResultCount = arrResult.Count;
            
            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                int _nTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nTeamLeader = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nRoleType = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 6].ToString(), "");

                bool _includeChildTeams = true;

                if (nMemberID < 0)
                {
                    nMemberID = -nMemberID;
                    _includeChildTeams = false;
                }

                if (_nTeamID < 0 || nMemberID < 0)
                    continue;

                if (!TemporaryMember.GetMemberType(nMemberType, out memberType))
                    continue;

                if (!TemporaryMember.GetRoleType(nRoleType, out roleType))
                {
                    roleType = TemporaryMember.RoleType.Unknown;
                    //continue;
                }

                if (strMemberName == "null")
                    strMemberName = "";

                TemporaryMember member = new TemporaryMember(_nTeamID, isNormal, nMemberID, nTeamLeader, memberType, roleType, strMemberName);
                members.Add(member);

                if (memberType == TemporaryMember.MemberType.ExternalCompanyTeam ||
                    memberType == TemporaryMember.MemberType.ExternalTeam ||
                    memberType == TemporaryMember.MemberType.RegularTeam)
                    member.IncludeChildTeams = _includeChildTeams;
            }

            return true;
        }

		// dicTeamName : TeamID, TeamName
        private bool ReadTeamList(WebDBManager dbMgr, string strTableName, bool isNormal, Dictionary<int, string> dicTeamName, ref Dictionary<int, List<TemporaryMember>> dicTemporaryMembers)
		{
			//string strSQL = "select id, TeamName, RegularTeamLink from " + strTableName;
			//ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

			//if (arrResult == null)
			//	return false;

            string strFormat = "select team.ID, TeamName, link.MemberID, link.MemberType, link.IsTeamLeader, link.Role, link.MemberName from {0} as team, TemporaryMemberList as link ";
            strFormat += "where link.TemporaryTeamID = team.ID and link.IsNormal = {1} and team.SiteID = {2}";

            string strSQL = string.Format(strFormat, strTableName, isNormal ? 1 : 0, ProxySOP.Instance.SiteID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

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
                    continue;

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

            strSQL = "select ID, TeamName from TemporaryNormalTeam where SiteID = " + ProxySOP.Instance.SiteID.ToString();

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

            arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");

                dicTeamName[nTeamID] = strTeamName;
            }

            /*string szSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", ProxySOP.Instance.SiteID);
            ArrayList arrResult1 = dbMgr.GetResultData(szSQL, 0);
            if (arrResult1 == null || arrResult1.Count == 0)
                return false;

            int nTopTeamID = WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
            if (nTopTeamID == -1)
                return false;

            string strSQL = string.Format("sp_TeamList2 {0}", nTopTeamID);
            ArrayList arrResult = dbMgr.GetStoredProcedureData(strSQL, 0);
            int nResultCount = arrResult.Count;
            for (int i = 0; i < nResultCount - 2; i += 3)
			{
				int nTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				string strTeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
				string strRegularTeamLink = WebDBManager.GetStringField(arrResult[i + 2].ToString(), "");

				dicTeamName[nTeamID] = strTeamName;

				ArrayList arrRegularTeamIDList = null;
				
				if (dicRegularTeamID.ContainsKey(nTeamID))
					arrRegularTeamIDList = dicRegularTeamID[nTeamID];
				else
				{
					arrRegularTeamIDList = new ArrayList();
					dicRegularTeamID[nTeamID] = arrRegularTeamIDList;
				}

				ReadRegularTeamID(dbMgr, strRegularTeamLink, arrRegularTeamIDList);
			}*/
			return true;
		}

		// dicTeamName : TeamID, TeamName
		private bool ReadTeamList(WebDBManager dbMgr, string strTableName, Dictionary<int, string> dicTeamName)
		{
            if (strTableName == "RegularTeam")
            {
                // Site별로 사용할 수 있도록 수정 , Edit by skkim 2015.01.09
                // SiteID로 본부 아이디를 가져온다.
                string szSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", ProxySOP.Instance.SiteID);
                ArrayList arrResult1 = dbMgr.GetResultData(szSQL, 0);
                if (arrResult1 == null || arrResult1.Count == 0)
                    return false;

                int nTopTeamID = WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
                if (nTopTeamID == -1)
                    return false;

                ArrayList arrResult = ExecuteTeamList(dbMgr, nTopTeamID);
                //string strSQL = string.Format("sp_TeamList2 {0}", nTopTeamID);
                //ArrayList arrResult = dbMgr.GetStoredProcedureData(strSQL, 0);
                for (int i = 0; i < arrResult.Count - 2; i += 3)
                {
                    int nTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                    string strTeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
                    dicTeamName[nTeamID] = strTeamName;
                }
            }
            else if (strTableName == "ControlRoom")
            {
                Dictionary<int, Data_ControlRoom> dicControlRoom = FormSOP.Instance.SOPManager.ControlRoom;

                foreach (KeyValuePair<int, Data_ControlRoom> item in dicControlRoom)
                {                    
                    if (!dicTeamName.ContainsKey(item.Value.ID))
                        dicTeamName.Add(item.Value.ID, item.Value.TeamName);
                    else
                        dicTeamName[item.Value.ID] = item.Value.TeamName;
                }
            }
            else
            {
                string strSQL = "select id, TeamName from " + strTableName;
                strSQL += " WHERE SiteID = " + ProxySOP.Instance.SiteID.ToString();

                ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
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
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            if (nRootTeamID == 0)
                return arrResult;

            int nResultCount = arrResult.Count;

            ArrayList arrNewResult = new ArrayList();
            Dictionary<int, int> dicParentID = new Dictionary<int, int>();

            for (int i=0;i<nResultCount-2;i+=3)
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

		public static Dictionary<int, Sections.ExternalTeamData> ReadExternalTeamList(WebDBManager dbMgr)
		{
			if (m_dicExternal != null)
				return m_dicExternal;
            			
            //string strSQL = "SELECT id, TeamName, PhoneNumber, FaxNumber from ExternalTeam";
            // Edit by Skkim. 2015.01.09 , 여러 Site에서 사용할 수 있도록 SiteID를 지정
            string szText = "SELECT id, TeamName, PhoneNumber, FaxNumber FROM ExternalTeam WHERE SiteID = {0}";
            string strSQL = string.Format(szText, ProxySOP.Instance.SiteID);
			
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null)
				return null;

			m_dicExternal = new Dictionary<int, Sections.ExternalTeamData>();

			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 3; i += 4)
			{
				int nTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				string strTeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
				string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 2].ToString(), "");
				string strFaxNumber = WebDBManager.GetStringField(arrResult[i + 3].ToString(), "");

				Sections.ExternalTeamData data = new Sections.ExternalTeamData();
				data.TeamID = nTeamID;
				data.TeamName = strTeamName;
				data.PhoneNumber = strPhoneNumber;
				data.FaxNumber = strFaxNumber;

				m_dicExternal[nTeamID] = data;
			}
			return m_dicExternal;
		}

		private bool GetTeamName(WebDBManager dbMgr, 
            ref Sections.SectionData sectionData, 
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

			strTeamID = Utility.TrimString(strTeamID);
			strTeamType = Utility.TrimString(strTeamType);

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
				if (dicNormal == null)
				{
					dicNormal = new Dictionary<int, string>();
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
				if (dicEmergency == null)
				{
					dicEmergency = new Dictionary<int, string>();
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
                strSQL = string.Format(strSQL, nTeamID, ProxySOP.Instance.SiteID);
            }
            else if (strTeamType == "1")
            {
                //strSQL = "select ID, TeamName, LevelNo from TemporaryEmergencyTeam where ID = " + nTeamID.ToString();
                strSQL = "select team.ID, TeamName, link.MemberID from TemporaryEmergencyTeam as team, TemporaryMemberList as link where team.ID = link.TemporaryTeamID and link.IsNormal = 0 and link.MemberType = 6 and team.ID = {0} and SiteID = {1}";
                strSQL = string.Format(strSQL, nTeamID, ProxySOP.Instance.SiteID);
            }
            else
                return nLevelNo;

			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

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

		// TeamID(TeamType), ... 형태로 되어 있는 strTeamList를 분석하여 Team 이름들을 얻어온다.
		// ex) 1(0), 1(2), 2(3), 5(0)
		private string GetTeamList(WebDBManager dbMgr, string strTeamList, ref Sections.SectionData sectionData, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined, ref Dictionary<int, Sections.ExternalTeamData> dicExternal, ref Dictionary<int, string> dicRegular, ref Dictionary<int, string> dicControlRoom)
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

        // dicSectionMissionItems : Key(Section ID), Value(SectionData.MissionItems)
        private bool LoadProcessMission(WebDBManager dbMgr, string strSectionIDs, Dictionary<int, ArrayList> dicSectionMissionItems)
        {
            if (strSectionIDs.Length == 0)
                return true;

            string strSQL = string.Format("Select ID, missionText, TransmissionType, missionTarget, CommanderDisplayText, CommanderMemberType, CommanderMemberID, ProcessID from ProcessMission where ProcessID in ({0})", strSectionIDs);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;
            Sections.MissionItem prevItem = null;

            for (int i = 0; i < nResultCount - 7; i += 8)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strMissionText = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nTransmission = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 2);
                string strTarget = WebDBManager.GetStringField(arrResult[i + 3], "");
                string strCommanderDisplayText = WebDBManager.GetStringField(arrResult[i + 4], "");
                int nCommanderMemberType = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                int nCommanderMemberID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                int nProcessID = WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);

                //string strLower = strMissionText.Trim().ToLower();

                Sections.MissionItem item = MissionItemExternal.IsExternalMissionText(strMissionText) ? new MissionItemExternal(strMissionText) : new Sections.MissionItem();
                //Sections.MissionItem item = strLower.StartsWith("#exec") ? new MissionItemExternal(strMissionText) : new Sections.MissionItem();

                if (prevItem != null && prevItem is MissionItemExternal)
                {
                    string strDescription;

                    if (MissionItemExternal.IsExternalMissionDescriptionText(strMissionText, out strDescription))
                    {
                        MissionItemExternal _item = (MissionItemExternal)prevItem;
                        _item.Description = strDescription;
                        prevItem = null;
                        continue;
                    }
                }

                prevItem = item;

                item.Mission = strMissionText;
                item.TransmissionType = nTransmission;

                if (strTarget == null || strTarget.Equals("null"))
                {
                    strTarget = "";
                }
                item.Target = strTarget;
                //item.Transmission = nTransmission;

                Sections.SectionCommander commander = LoadCommanderTeamMember(dbMgr, nCommanderMemberType, nCommanderMemberID, strCommanderDisplayText);
                item.Commander = commander;

                ArrayList arrMissionItems = null;

                if (dicSectionMissionItems.TryGetValue(nProcessID, out arrMissionItems))
                {
                    arrMissionItems.Add(item);
                }
            }

            return true;
        }

		private bool LoadProcessMission(WebDBManager dbMgr, int nProcessID, ArrayList arrMissionItems)
		{
			string strSQL = string.Format("Select ID, missionText, TransmissionType, missionTarget, CommanderDisplayText, CommanderMemberType, CommanderMemberID from ProcessMission where ProcessID = {0}", nProcessID);
			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null)
				return false;

			int nResultCount = arrResult.Count;
            Sections.MissionItem prevItem = null;

            for (int i = 0; i < nResultCount - 6; i += 7)
			{
				int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				string strMissionText = WebDBManager.GetStringField(arrResult[i + 1], "");
				int nTransmission = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 2);
				string strTarget = WebDBManager.GetStringField(arrResult[i + 3], "");
                string strCommanderDisplayText = WebDBManager.GetStringField(arrResult[i + 4], "");
                int nCommanderMemberType = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                int nCommanderMemberID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);

                //string strLower = strMissionText.Trim().ToLower();

                Sections.MissionItem item = MissionItemExternal.IsExternalMissionText(strMissionText) ? new MissionItemExternal(strMissionText) : new Sections.MissionItem();
                //Sections.MissionItem item = strLower.StartsWith("#exec") ? new MissionItemExternal(strMissionText) : new Sections.MissionItem();

                if (prevItem != null && prevItem is MissionItemExternal)
                {
                    string strDescription;

                    if (MissionItemExternal.IsExternalMissionDescriptionText(strMissionText, out strDescription))
                    {
                        MissionItemExternal _item = (MissionItemExternal)prevItem;
                        _item.Description = strDescription;
                        prevItem = null;
                        continue;
                    }
                }

                prevItem = item;

                item.Mission = strMissionText;
				item.TransmissionType = nTransmission;

				if (strTarget == null || strTarget.Equals("null"))
				{
					strTarget = "";
				}
				item.Target = strTarget;
				//item.Transmission = nTransmission;

                Sections.SectionCommander commander = LoadCommanderTeamMember(dbMgr, nCommanderMemberType, nCommanderMemberID, strCommanderDisplayText);
                item.Commander = commander;

				arrMissionItems.Add(item);
			}

			return true;
		}

        //private bool LoadCheckedItems(WebDBManager dbMgr, int nProcessID, ArrayList arrCheckedItems)
        //{
        //    string strSQL = string.Format("Select ID, Category, SubCategory, TaskName, TargetCount, Position from CheckTask where ProcessID = {0}", nProcessID);
        //    ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

        //    if (arrResult == null)
        //        return false;

        //    int nResultCount = arrResult.Count;

        //    for (int i = 0; i < nResultCount - 5; i += 6)
        //    {
        //        int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
        //        string strCategory = WebDBManager.GetStringField(arrResult[i + 1], "");
        //        string strSubCategory = WebDBManager.GetStringField(arrResult[i + 2], "");
        //        string strTaskName = WebDBManager.GetStringField(arrResult[i + 3], "");
        //        int nTargetCount = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
        //        string strPosition = WebDBManager.GetStringField(arrResult[i + 5], "");

        //        Sections.CheckedItem item = new Sections.CheckedItem();

        //        item.Category = strCategory;
        //        item.SubCategory = strSubCategory;
        //        item.Item = strTaskName;
        //        item.ItemCount = nTargetCount;
        //        item.Location = strPosition;

        //        arrCheckedItems.Add(item);
        //    }

        //    return true;
        //}

        private bool LoadProcess(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberData data, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined, ref Dictionary<int, Sections.ExternalTeamData> dicExternal, ref Dictionary<int, string> dicRegular, ref Dictionary<int, string> dicControlRoom)
		{
			string strSQL = "select id, x, y, width, height, text, TeamList, ComponentID, ProcessTime, ProcessTimeType, useProcessTime, useMissionMessage";
            strSQL += ", onlyTeamLeader, CommanderMemberType, CommanderMemberID, CommanderDisplayText, valign, halign, AutoRun from Process where StepMemberID = " + data.StepMemberID.ToString();

			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
			if (arrResult == null)
				return false;

			int nResultCount = arrResult.Count;

            string strSectionIDs = "";
            Dictionary<int, ArrayList> dicSectionMissionItems = new Dictionary<int, ArrayList>();

			for (int i = 0; i < nResultCount - 18; i += 19)
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
                string strCommanderDisplayText = WebDBManager.GetStringField(arrResult[i + 15], "");

                if (strCommanderDisplayText == "null")
                    strCommanderDisplayText = "";

                // Default Valign - Center
                int nValign = WebDBManager.GetIntField(arrResult[i + 16].ToString(), 2);
                // Default Haling - Left
                int nHalign = WebDBManager.GetIntField(arrResult[i + 17].ToString(), 2);
                bool autoRun = WebDBManager.GetIntField(arrResult[i + 18].ToString(), 0) == 0 ? false : true;

				Sections.SectionProcess section = new Sections.SectionProcess(panel, x, y);
				Sections.SectionDataProcess sectionData = (Sections.SectionDataProcess)section.Data;
                Sections.SectionData tempData = section.Data;
				dicSections[nID] = section;
				arrSections.Add(section);

				panel.Sections.Add(section);
				panel.SetComponentID(section, nID);

				section.RectSize = new SizeF(fWidth, fHeight);
				section.TextUP = strText;
                section.TextDown = GetTeamList(dbMgr, strTeamList, ref tempData, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom);

				sectionData.ID = nID;
				sectionData.Title = strText;
				sectionData.ComponentID = strComponentID;
				sectionData.ProcessingTime.Time = nProcessTime;

                sectionData.TextVerticalAlign = (Sections.SectionData.TextVAlign)nValign;
                sectionData.TextHorizontalAlign = (Sections.SectionData.TextHAlign)nHalign;

				Sections.ProcessingTime.Type type = Sections.ProcessingTime.Type.UNKNOWN;
				if (!Sections.ProcessingTime.IntToType(nProcessTimeType, ref type))
					return false;

				sectionData.ProcessingTime.ProcessingType = type;
				sectionData.UseProcessingTime = useProcessTime;
				sectionData.MissionTransfer = useMissionMessage;
				sectionData.TransferTeamLeaderOnly = onlyTeamLeader;
                sectionData.AutoRun = autoRun;

                if (strSectionIDs.Length == 0)
                    strSectionIDs = nID.ToString();
                else
                    strSectionIDs += "," + nID.ToString();

                dicSectionMissionItems[nID] = sectionData.MissionItems;

                // 속도 개선을 위하여 Section마다 LoadProcessMission을 호출하여 각각 Query를 진행하지 않고
                // 한꺼번에 모든 ProcessMission Query를 호출하도록 한다.
                // [2017/10/11] 김지웅
				//if (!LoadProcessMission(dbMgr, nID, sectionData.MissionItems))
				//	return false;

                Sections.SectionCommander commander = LoadCommanderTeamMember(dbMgr, nCommanderMemberType, nCommanderMemberID, strCommanderDisplayText);
                sectionData.Commander = commander;

				//if (!LoadCheckedItems(dbMgr, nID, sectionData.CheckedItems))
				//	return false;
			}

            if (!LoadProcessMission(dbMgr, strSectionIDs, dicSectionMissionItems))
            	return false;

			return true;
		}

        public static Sections.SOPTeam LoadRegularRootTeam(WebDBManager dbMgr)
        {
            if (m_teamRegularRoot != null)
                return m_teamRegularRoot;

            string strSQL = "Select r.ID, r.TeamName from RegularTeam as r, Site where r.ID = Site.TeamID and Site.ID = " + ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count != 2)
                return null;

            int nTeamID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            string strTeamName = WebDBManager.GetStringField(arrResult[1], "");

            if (nTeamID < 0 || strTeamName.Length == 0 || strTeamName == "null")
                return null;

            Sections.SOPTeam team = new Sections.SOPTeam();
            team.TeamID = nTeamID;
            team.TeamName = strTeamName;
            team.TeamType = Sections.SOPTeam.SOPTeamType.Regular;

            m_teamRegularRoot = team;
            return team;
        }

        // TeamID(TeamType), ... 형태로 되어 있는 strTeamList를 분석하여 Team 이름들을 얻어온다.
        // ex) 1(0), 1(2), 2(3), 5(0)
        public static List<Sections.SOPTeam> LoadTeamList(WebDBManager dbMgr, string strTeamList)
        {
            List<Sections.SOPTeam> teams = new List<Sections.SOPTeam>();

            if (strTeamList.Length == 0)
            {
                Sections.SOPTeam team = LoadRegularRootTeam(dbMgr);

                if (team != null)
                    teams.Add(team);

                return teams;
            }

            string[] arrTokens = strTeamList.Split(',');

            foreach (string strToken in arrTokens)
            {
                int nIndex1 = strToken.IndexOf('(');
                int nIndex2 = strToken.IndexOf(')');

                if (nIndex1 > 0 && nIndex2 > nIndex1 + 1)
                {
                    string strID = strToken.Substring(0, nIndex1).Trim();
                    string strType = strToken.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);

                    int nID, nType;

                    if (int.TryParse(strID, out nID) && int.TryParse(strType, out nType))
                    {
                        bool includeChildTeams = true;

                        if (nID < 0)
                        {
                            nID = -nID;
                            includeChildTeams = false;
                        }

                        Sections.SOPTeam team = LoadSOPTeam(dbMgr, nID, nType);

                        if (team != null)
                        {
                            teams.Add(team);

                            if (team.TeamType == Sections.SOPTeam.SOPTeamType.Normal || team.TeamType == Sections.SOPTeam.SOPTeamType.Holiday ||
                                team.TeamType == Sections.SOPTeam.SOPTeamType.External || team.TeamType == Sections.SOPTeam.SOPTeamType.Regular)
                                team.IncludeChildTeams = includeChildTeams;
                        }
                    }
                }
            }

            return teams;
        }

        private static Sections.SOPTeam LoadSOPTeam(WebDBManager dbMgr, int nTeamID, int nTeamType)
        {
            Sections.SOPTeam team = null;

            if (nTeamType == (int)Sections.SOPTeam.SOPTeamType.Normal || nTeamType == (int)Sections.SOPTeam.SOPTeamType.Holiday)
			{
				bool isNormal = nTeamType == (int)Sections.SOPTeam.SOPTeamType.Normal;
                string strTableName = nTeamType == (int)Sections.SOPTeam.SOPTeamType.Normal ? "TemporaryNormalTeam" : "TemporaryEmergencyTeam";

                string strSQL = string.Format("Select TeamName from {0} where ID = {1}", strTableName, nTeamID);
                ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

                if (arrResult != null && arrResult.Count == 1)
                {
                    string strTeamName = WebDBManager.GetStringField(arrResult[0], "");

                    if (strTeamName != "" && strTeamName != "null")
                    {
                        team = new Sections.SOPTeam();

                        team.TeamType = (Sections.SOPTeam.SOPTeamType)nTeamType;
                        team.TeamID = nTeamID;
                        team.TeamName = strTeamName;
                    }
                }
			}
			else if (nTeamType == (int)Sections.SOPTeam.SOPTeamType.External)
			{
                Data_ExternalTeam external = FormSOP.Instance.SOPManager.GetExternalTeam(nTeamID);

                if (external != null)
                {
                    team = new Sections.SOPTeam();

                    team.TeamType = (Sections.SOPTeam.SOPTeamType)nTeamType;
                    team.TeamID = nTeamID;
                    team.TeamName = external.TeamName;
                }
			}
            else if (nTeamType == (int)Sections.SOPTeam.SOPTeamType.UserDefined)
            {
                Data_ExternalTeam userDefined = FormSOP.Instance.SOPManager.GetUserDefinedTeam(nTeamID);

                if (userDefined != null)
                {
                    team = new Sections.SOPTeam();

                    team.TeamType = (Sections.SOPTeam.SOPTeamType)nTeamType;
                    team.TeamID = nTeamID;
                    team.TeamName = userDefined.TeamName;
                }
            }
            else if (nTeamType == (int)Sections.SOPTeam.SOPTeamType.Regular)
            {
                Data_RegularTeam regular = FormSOP.Instance.SOPManager.GetRegularTeam(nTeamID);

                if (regular != null)
                {
                    team = new Sections.SOPTeam();

                    team.TeamType = (Sections.SOPTeam.SOPTeamType)nTeamType;
                    team.TeamID = nTeamID;
                    team.TeamName = regular.TeamName;
                }
            }
            else if (nTeamType == (int)Sections.SOPTeam.SOPTeamType.ControlRoom)
            {
                Data_ControlRoom regular = FormSOP.Instance.SOPManager.GetControlRoom(nTeamID);

                if (regular != null)
                {
                    team = new Sections.SOPTeam();

                    team.TeamType = (Sections.SOPTeam.SOPTeamType)nTeamType;
                    team.TeamID = nTeamID;
                    team.TeamName = regular.TeamName;
                }
            }

            return team;
        }

        public static void ClearSectionCommanders()
        {
            m_dicSectionCommanders.Clear();
        }

        public static Sections.SectionCommander LoadCommanderTeamMember(WebDBManager dbMgr, int nTeamType, int nMemberID, string strDisplayText)
        {
            string strSQL = "";
            Sections.SectionCommander commander = null;

            long key = (((long)nTeamType) << 32) | ((long)nMemberID);

            if (m_dicSectionCommanders.TryGetValue(key, out commander))
                return commander;

            if (nTeamType == -1)
            {
                // Default Option
                commander = new Sections.SectionCommander();

                if (strDisplayText.Length > 0)
                    commander.DisplayText = strDisplayText;
            }
            else if (nTeamType >= (int)Sections.SOPTeam.SOPTeamType.Normal && nTeamType <= (int)Sections.SOPTeam.SOPTeamType.Regular)
            {
                if (nTeamType == (int)Sections.SOPTeam.SOPTeamType.Normal)
                {
                    strSQL = string.Format("Select ID, TeamName from TemporaryNormalTeam where ID in ({0}) and SiteID = {1}",
                        nMemberID, ProxySOP.Instance.SiteID);
                }
                else if (nTeamType == (int)Sections.SOPTeam.SOPTeamType.Holiday)
                {
                    strSQL = string.Format("Select ID, TeamName from TemporaryEmergencyTeam where ID in ({0}) and SiteID = {1}",
                        nMemberID, ProxySOP.Instance.SiteID);
                }
                else if (nTeamType == (int)Sections.SOPTeam.SOPTeamType.External)
                {
                    strSQL = string.Format("Select ID, TeamName from ExternalTeam where ID in ({0})",
                        nMemberID, ProxySOP.Instance.SiteID);
                }
                else if (nTeamType == (int)Sections.SOPTeam.SOPTeamType.UserDefined)
                {
                    strSQL = string.Format("Select ID, TeamName from UserDefinedTeam where ID in ({0})",
                        nMemberID, ProxySOP.Instance.SiteID);
                }
                else if (nTeamType == (int)Sections.SOPTeam.SOPTeamType.Regular)
                {
                    strSQL = string.Format("Select ID, TeamName from RegularTeam where ID in ({0})",
                        nMemberID, ProxySOP.Instance.SiteID);
                }
                else
                    return null;

                ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null || arrResult.Count != 2)
                    return null;

                int nID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResult[1], "");

                commander = new Sections.SectionCommander();

                commander.Team = new Sections.SOPTeam();
                commander.Team.TeamType = (Sections.SOPTeam.SOPTeamType)nTeamType;
                commander.Team.TeamID = nID;
                commander.Team.TeamName = strTeamName;
                commander.IsTeamMember = false;
                commander.TeamMemberID = -1;

                if (strDisplayText.Length > 0)
                    commander.DisplayText = strDisplayText;
            }
            else if (nTeamType == (int)Sections.SOPTeam.SOPTeamType.ControlRoom)
            { 
                Data_ControlRoom item = FormSOP.Instance.SOPManager.GetControlRoom(nMemberID);
                
                //Data_ControlRoom regular = FormSOP.Instance.SOPManager.GetRegularTeam(nTeamID);
                //foreach (Data_ControlRoom item in FormMain.Instance.ControlRoom)
                //{
                    
                //    if (item.ID == nMemberID)
                //    {
                        commander = new Sections.SectionCommander();

                        commander.Team = new Sections.SOPTeam();
                        commander.Team.TeamType = (Sections.SOPTeam.SOPTeamType)nTeamType;
                        commander.Team.TeamID = item.ID;
                        commander.Team.TeamName = item.TeamName;
                        commander.IsTeamMember = false;
                        commander.TeamMemberID = -1;

                        if (strDisplayText.Length > 0)
                            commander.DisplayText = strDisplayText;
                //        break;
                //    }
                //}
            }
            else
            {
                if (nTeamType == 5 || nTeamType == 6)
                {
                    return LoadTemporaryMemberListCommander(dbMgr, nMemberID, strDisplayText);
                }
                else if (nTeamType == 7)
                {
                    strSQL = string.Format("Select ID, Name from ExternalCompanyMember where ID in ({0})", nMemberID);
                }
                else if (nTeamType == 8)
                {
                    strSQL = string.Format("Select ID, MemberName from CompanyMember where ID in ({0})", nMemberID);
                }
                else if (nTeamType == 9)
                {
                    return LoadDutyMemberListCommander(dbMgr, strDisplayText);
                } 
                else
                    return null;

                ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null || arrResult.Count != 2)
                    return null;

                int nID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
                string strMemberName = WebDBManager.GetStringField(arrResult[1], "");

                commander = new Sections.SectionCommander();

                commander.Team = new Sections.SOPTeam();

                if (nTeamType == 8)
                    commander.Team.TeamType = Sections.SOPTeam.SOPTeamType.Regular;
                else
                    commander.Team.TeamType = (Sections.SOPTeam.SOPTeamType)(nTeamType - 5);

                commander.Team.TeamID = -1;
                commander.Team.TeamName = "";
                commander.IsTeamMember = true;
                commander.TeamMemberID = nID;

                if (strDisplayText.Length > 0)
                    commander.DisplayText = strDisplayText;
            }

            return commander;
        }

        private static Sections.SectionCommander LoadDutyMemberListCommander(WebDBManager dbMgr, string strDisplayText)
        {
            string strSQL = "select ctm.MemberType, ctm.MemberID, ctm.JobPosition ";
            strSQL += "from ControlRoomType as crt, ControlRoom as cr, ControlTeamMembers as ctm ";
            strSQL += "where crt.TypeName = '당직실' and cr.RoomType = crt.ID and ctm.RoomID = cr.ID and crt.SiteID = " + ProxySOP.Instance.SiteID.ToString(); 

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            int nIndex = -1, nJobPosition = -1;

            for (int i=0;i<nResultCount-2;i+=3)
            {
                DBUtility.VariousData<int> jobPosition = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                if (jobPosition == null)
                    continue;

                if (nJobPosition < 0 || nJobPosition > jobPosition.Data)
                {
                    nJobPosition = jobPosition.Data;
                    nIndex = i;
                }
            }

            if (nIndex < 0)
                return null;

            DBUtility.VariousData<int> memberType = WebDBManager.GetIntField(arrResult[nIndex].ToString());
            DBUtility.VariousData<int> memberID = WebDBManager.GetIntField(arrResult[nIndex + 1].ToString());

            if (memberType == null)
                return null;

            Sections.SectionCommander commander = new Sections.SectionCommander();
            commander.Team = new Sections.SOPTeam();

            if (memberType.Data == 1)
            {
                if (memberID != null)
                {
                    Data_CompanyMember member = FormSOP.Instance.SOPManager.GetRegularCompanyMember(memberID.Data);

                    if (member == null)
                        return null;

                    commander.Team.TeamType = Sections.SOPTeam.SOPTeamType.Regular;
                    commander.IsTeamMember = true;
                    commander.TeamMemberID = member.ID;

                    if (member.TeamPositions.Count > 0)
                        commander.Team.TeamID = member.TeamPositions.ElementAt(0).Key.ID;

                    if (strDisplayText.Length > 0)
                        commander.DisplayText = strDisplayText;
                    else
                        commander.DisplayText = member.MemberName;
                }
                else
                {
                    commander.Team.TeamType = Sections.SOPTeam.SOPTeamType.Regular;
                    commander.IsTeamMember = true;

                    if (strDisplayText.Length > 0)
                        commander.DisplayText = strDisplayText;
                }

                return commander;
            }
            else if (memberType.Data == 4)
            {
                if (memberID != null)
                {
                    foreach (ExternalCompanyMember member in FormSOP.Instance.SOPManager.ExternalCompanyMembers)
                    {
                        if (member.ID == memberID.Data)
                        {
                            commander.Team.TeamType = Sections.SOPTeam.SOPTeamType.External;
                            commander.IsTeamMember = true;
                            commander.TeamMemberID = member.ID;

                            if (member.Teams.Count > 0)
                                commander.Team.TeamID = member.Teams[0].ID;

                            if (strDisplayText.Length > 0)
                                commander.DisplayText = strDisplayText;
                            else
                                commander.DisplayText = member.MemberName;

                            return commander;
                        }
                    }
                }
                else
                {
                    commander.Team.TeamType = Sections.SOPTeam.SOPTeamType.External;
                    commander.IsTeamMember = true;

                    if (strDisplayText.Length > 0)
                        commander.DisplayText = strDisplayText;

                    return commander;
                }
            }
            
            return null;
        }

        private static Sections.SectionCommander LoadTemporaryMemberListCommander(WebDBManager dbMgr, int nMemberID, string strDisplayText)
        {
            TemporaryMember member = FormSOP.Instance.SOPManager.GetTemporaryMember(nMemberID);

            if (member == null)
                return null;

            Sections.SectionCommander commander = new Sections.SectionCommander();

            commander.Team = new Sections.SOPTeam();

            commander.Team.TeamType = member.IsNormal ? Sections.SOPTeam.SOPTeamType.Normal : Sections.SOPTeam.SOPTeamType.Holiday;
            commander.Team.TeamID = member.TemporaryTeamID;
            commander.IsTeamMember = member.TeamLeader != 1;
            commander.TeamMemberID = nMemberID;

            if (strDisplayText.Length > 0)
                commander.DisplayText = strDisplayText;
            else
                commander.DisplayText = member.MemberName;

            return commander;
        }

		private bool LoadDecision(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberData data)
		{
            string strSQL = "select id, x, y, width, height, text, ComponentID, valign, halign, autoRunScript, autoRunScriptVariableTypes from Decision where StepMemberID = " + data.StepMemberID.ToString();
			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null)
				return false;

			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 10; i += 11)
			{
				int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				float x = WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0.0f);
				float y = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
				float fWidth = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
				float fHeight = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
				string strText = WebDBManager.GetStringField(arrResult[i + 5], "");
				string strComponentID = WebDBManager.GetStringField(arrResult[i + 6], "");

                // Default Valign - Center
                int nValign = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 2);
                // Default Haling - Left
                int nHalign = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 2);

                string strAutoRunScript = WebDBManager.GetStringField(arrResult[i + 9]);
                string strVariableTypes = WebDBManager.GetStringField(arrResult[i + 10]);

				Sections.SectionDecision section = new Sections.SectionDecision(panel, x, y);
				Sections.SectionDataDecision sectionData = (Sections.SectionDataDecision)section.Data;
				dicSections[nID] = section;
				arrSections.Add(section);

				panel.Sections.Add(section);
				panel.SetComponentID(section, nID);

				section.RectSize = new SizeF(fWidth, fHeight);
				section.Title = strText;

				sectionData.ID = nID;
				sectionData.Title = strText;
				sectionData.ComponentID = strComponentID;

                sectionData.TextVerticalAlign = (Sections.SectionData.TextVAlign)nValign;
                sectionData.TextHorizontalAlign = (Sections.SectionData.TextHAlign)nHalign;
                DecisionDataHelper.SetDecisionExpression(sectionData, strAutoRunScript, strVariableTypes);
			}

			return true;
		}

		private bool LoadAnnotation(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberData data)
		{
            string strSQL = "select id, x, y, width, height, text, ComponentID, valign, halign from Annotation where StepMemberID = " + data.StepMemberID.ToString();
			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null)
				return false;

			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 8; i += 9)
			{
				int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				float x = WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0.0f);
				float y = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
				float fWidth = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
				float fHeight = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
				string strText = WebDBManager.GetStringField(arrResult[i + 5], "");
				string strComponentID = WebDBManager.GetStringField(arrResult[i + 6], "");

                // Default Valign - Center
                int nValign = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 2);
                // Default Haling - Left
                int nHalign = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 2);

				Sections.SectionAnnotation section = new Sections.SectionAnnotation(panel, x, y);
				Sections.SectionDataAnnotation sectionData = (Sections.SectionDataAnnotation)section.Data;
				dicSections[nID] = section;
				arrSections.Add(section);

				panel.Sections.Add(section);
				panel.SetComponentID(section, nID);

				section.RectSize = new SizeF(fWidth, fHeight);
				section.Title = strText;

				sectionData.ID = nID;
				sectionData.Title = strText;
				sectionData.ComponentID = strComponentID;

                sectionData.TextVerticalAlign = (Sections.SectionData.TextVAlign)nValign;
                sectionData.TextHorizontalAlign = (Sections.SectionData.TextHAlign)nHalign;
			}

			return true;
		}

		private bool LoadEndPoint(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberData data)
		{
            string strSQL = "select id, x, y, width, height, text, ComponentID, isBegin, valign, halign from EndPoint where StepMemberID = " + data.StepMemberID.ToString();
			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null)
				return false;

			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 9; i += 10)
			{
				int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				float x = WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0.0f);
				float y = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
				float fWidth = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
				float fHeight = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
				string strText = WebDBManager.GetStringField(arrResult[i + 5], "");
				string strComponentID = WebDBManager.GetStringField(arrResult[i + 6], "");
				bool isBegin = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0) == 0 ? false : true;

                // Default Valign - Center
                int nValign = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 2);
                // Default Haling - Left
                int nHalign = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 2);

				Sections.SectionEndPoint section = new Sections.SectionEndPoint(panel, x, y);
				Sections.SectionDataEndPoint sectionData = (Sections.SectionDataEndPoint)section.Data;
				dicSections[nID] = section;
				arrSections.Add(section);
				
				panel.Sections.Add(section);
				panel.SetComponentID(section, nID);

				section.RectSize = new SizeF(fWidth, fHeight);
				section.Title = strText;

				sectionData.ID = nID;
				sectionData.Title = strText;
				sectionData.ComponentID = strComponentID;
				sectionData.IsBegin = isBegin;

                sectionData.TextVerticalAlign = (Sections.SectionData.TextVAlign)nValign;
                sectionData.TextHorizontalAlign = (Sections.SectionData.TextHAlign)nHalign;


                if (sectionData.IsBegin)
                {
                    Sections.ButtonEndPoint btn = new Sections.ButtonEndPoint(panel, x, y);

                    btn.RectSize = new SizeF(fWidth, fHeight);
                    btn.Section = section;
                    btn.Title = strText;
                    btn.ButtonClicked += FormSOP.Instance.SectionButtonClicked;
                    panel.Buttons.Add(btn);
                }
                else
                {
                    Sections.ButtonEndPoint btn = new Sections.ButtonEndPoint(panel, x, y);
                    btn.Data = sectionData;
                    btn.Section = section;
                    btn.RectSize = new SizeF(fWidth, fHeight);
                    btn.Title = strText;
                    btn.ButtonClicked += FormSOP.Instance.SectionButtonClicked;
                    panel.Buttons.Add(btn);
                }

				// 종료 Section은 화살표 없이 ProcessButton을 추가한다.
				if (!sectionData.IsBegin)
					SetProcessButton(section);
                else
                    SetProcessButton(section);
			}

			return true;
		}

		// arrLink : Link 객체는 다른 Panel의 객체와 연결되어야 하므로, DB로부터 모든 객체를 읽어들인 후에 해당 객체와 연결시킨다.
		private bool LoadLink(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, ArrayList arrLink, Sections.PanelSectionEx panel, StepMemberData data)
		{
            string strSQL = "select id, x, y, width, height, text, ComponentID, LinkedComponentID, valign, halign from Link where StepMemberID = " + data.StepMemberID.ToString();
			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null)
				return false;

			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 9; i += 10)
			{
				int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				float x = WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0.0f);
				float y = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
				float fWidth = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
				float fHeight = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
				string strText = WebDBManager.GetStringField(arrResult[i + 5], "");
				string strComponentID = WebDBManager.GetStringField(arrResult[i + 6], "");
				string strLinkedComponentID = WebDBManager.GetStringField(arrResult[i + 7], "");
                // Default Valign - Center
                int nValign = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 2);
                // Default Haling - Left
                int nHalign = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 2);

				Sections.SectionLink section = new Sections.SectionLink(panel, x, y);
				Sections.SectionDataLink sectionData = (Sections.SectionDataLink)section.Data;
				dicSections[nID] = section;
				arrLink.Add(section);
				arrSections.Add(section);

				panel.Sections.Add(section);
				panel.SetComponentID(section, nID);

				section.RectSize = new SizeF(fWidth, fHeight);
				section.Title = strText;

				sectionData.ID = nID;
				// sectionData의 Title은 strText이지만 링크된 Section 객체의 이름을 기억해 놓기 위하여 임시로 strLinkedComponentID를 집어넣는다.
				sectionData.Title = strLinkedComponentID;
				sectionData.ComponentID = strComponentID;

                sectionData.TextVerticalAlign = (Sections.SectionData.TextVAlign)nValign;
                sectionData.TextHorizontalAlign = (Sections.SectionData.TextHAlign)nHalign;
			}

			return true;
		}

		private bool LoadTransSOP(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberData data)
		{
            string strSQL = "select id, x, y, width, height, text, ComponentID, LinkedActionStepID, Description, valign, halign from TransSOP where StepMemberID = " + data.StepMemberID.ToString();
			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null)
				return false;

			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 10; i += 11)
			{
				int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				float x = WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0.0f);
				float y = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
				float fWidth = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
				float fHeight = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
				string strText = WebDBManager.GetStringField(arrResult[i + 5], "");
				string strComponentID = WebDBManager.GetStringField(arrResult[i + 6], "");
				int nLinkedActionStepID = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
				string strDescription = WebDBManager.GetStringField(arrResult[i + 8], "");
                // Default Valign - Center
                int nValign = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 2);
                // Default Haling - Left
                int nHalign = WebDBManager.GetIntField(arrResult[i + 10].ToString(), 2);

				Sections.SectionTransSOP section = new Sections.SectionTransSOP(panel, x, y);
				Sections.SectionDataTransSOP sectionData = (Sections.SectionDataTransSOP)section.Data;
				dicSections[nID] = section;
				arrSections.Add(section);

				panel.Sections.Add(section);
				panel.SetComponentID(section, nID);

				section.RectSize = new SizeF(fWidth, fHeight);
				section.Title = strText;

				sectionData.ID = nID;
				sectionData.Title = strText;
				sectionData.ComponentID = strComponentID;
				sectionData.LinkedActionStepID = nLinkedActionStepID;
				sectionData.Description = strDescription;

                sectionData.TextVerticalAlign = (Sections.SectionData.TextVAlign)nValign;
                sectionData.TextHorizontalAlign = (Sections.SectionData.TextHAlign)nHalign;
			}

			return true;
		}

        private bool LoadInternal(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberData data, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined, ref Dictionary<int, Sections.ExternalTeamData> dicExternal, ref Dictionary<int, string> dicRegular, ref Dictionary<int, string> dicControlRoom)
		{
            string strSQL = "select id, x, y, width, height, text, ComponentID, usePopupMessage, useMobileApp, useBroadcast, BroadcastMessage, valign, halign, TeamList, onlyTeamLeader, CommanderMemberType, CommanderMemberID, CommanderDisplayText, AutoRun from InternalTransmission where StepMemberID = " + data.StepMemberID.ToString();
			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null)
				return false;

			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 18; i += 19)
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
				string szMessage = WebDBManager.GetStringField(arrResult[i + 10], "");
				if (szMessage == null || szMessage.Equals("null"))
					szMessage = "";

                // Default Valign - Center
                int nValign = WebDBManager.GetIntField(arrResult[i + 11].ToString(), 2);
                // Default Haling - Left
                int nHalign = WebDBManager.GetIntField(arrResult[i + 12].ToString(), 2);
                string szTeamList = WebDBManager.GetStringField(arrResult[i + 13], "");

                bool bOnlyTeamLeader = WebDBManager.GetIntField(arrResult[i + 14].ToString(), 0) == 0 ? false : true;

                int nCommanderMemberType = WebDBManager.GetIntField(arrResult[i + 15].ToString(), -2);
                int nCommanderMemberID = WebDBManager.GetIntField(arrResult[i + 16].ToString(), -2);
                string szCommanderDisplayText = WebDBManager.GetStringField(arrResult[i + 17], "");
                bool autoRun = WebDBManager.GetIntField(arrResult[i + 18].ToString(), 0) == 0 ? false : true;

                if (szCommanderDisplayText == null || szCommanderDisplayText == "null")
                    szCommanderDisplayText = "";
				
				Sections.SectionInternal section = new Sections.SectionInternal(panel, x, y);
                Sections.SectionData tempData = section.Data;
                if (szTeamList != null && szTeamList != "" && szTeamList != "null")
                {
                    GetTeamList(dbMgr, szTeamList, ref tempData, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom);
                }

				Sections.SectionDataInternal sectionData = (Sections.SectionDataInternal)section.Data;

                string[] arrHeadTitle = { "(문자)", "(문자전파)", "(방송)", "(방송전파)" };
                string strSMS = "(문자)", strBroadcast = "(방송)";

                foreach (string strHeadTitle in arrHeadTitle)
                {
                    if (strText.StartsWith(strHeadTitle))
                    {
                        strText = strText.Replace(strHeadTitle, "").Trim();
                        break;
                    }
                }

                if (useBroadcast)
                    strText = String.Format("{0}{1}", strBroadcast, strText);
                else if (useMobileApp)
                    strText = String.Format("{0}{1}", strSMS, strText);

				dicSections[nID] = section;
				arrSections.Add(section);
				
				panel.Sections.Add(section);
				panel.SetComponentID(section, nID);

				section.RectSize = new SizeF(fWidth, fHeight);
				section.Title = strText;

				sectionData.ID = nID;
				sectionData.Title = strText;
				sectionData.ComponentID = strComponentID;
				sectionData.UsePopupMessage = usePopupMessage;
				sectionData.UseMobileApp = useMobileApp;
				sectionData.UseBroadcast = useBroadcast;
				sectionData.BroadcastMessage = szMessage;

                sectionData.TextVerticalAlign = (Sections.SectionData.TextVAlign)nValign;
                sectionData.TextHorizontalAlign = (Sections.SectionData.TextHAlign)nHalign;
                sectionData.AutoRun = autoRun;

                Sections.SectionCommander commander = LoadCommanderTeamMember(dbMgr, nCommanderMemberType, nCommanderMemberID, szCommanderDisplayText);
                sectionData.Commander = commander;
			}

			return true;
		}

		private bool GetExternalTeam(string strTeamList, ArrayList arrExternalTeamList, Dictionary<int, Sections.ExternalTeamData> dicExternal, int nBeginIndex, int nEndIndex)
		{
			if (strTeamList.Length == 0)
				return true;

			string strTeamID = strTeamList.Substring(nBeginIndex, nEndIndex - nBeginIndex);
			strTeamID = Utility.TrimString(strTeamID);

			try
			{
				int nTeamID = int.Parse(strTeamID);

				if (!dicExternal.ContainsKey(nTeamID))
				{
					// 존재하지 않는 외부기관의 ID
					return false;
				}

				arrExternalTeamList.Add(dicExternal[nTeamID]);
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}

		// TeamID, ... 형태로 되어 있는 strTeamList를 분석하여 ExternalTeamData 객체로 만든 다음 arrExternalTeamList에 넣는다.
		// ex) 1, 1, 2, 5
		private bool GetExternalTeamList(string strTeamList, ArrayList arrExternalTeamList, Dictionary<int, Sections.ExternalTeamData> dicExternal)
		{
			int nBeginIndex = 0;
			int nLen = strTeamList.Length;

			while (nBeginIndex < nLen)
			{
				int nDotIndex = strTeamList.IndexOf(',', nBeginIndex);
				if (nDotIndex < 0) break;

				if (!GetExternalTeam(strTeamList, arrExternalTeamList, dicExternal, nBeginIndex, nDotIndex))
					return false;

				nBeginIndex = nDotIndex + 1;
			}

			if (!GetExternalTeam(strTeamList, arrExternalTeamList, dicExternal, nBeginIndex, nLen))
				return false;

			return true;
		}

		private bool LoadExternal(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberData data, Dictionary<int, Sections.ExternalTeamData> dicExternal)
		{
            string strSQL = "select id, x, y, width, height, text, ComponentID, useSMS, SMSText, SMSExternalTeamIDList, useEFax, FaxExternalTeamIDList, valign, halign from ExternalTransmission where StepMemberID = " + data.StepMemberID.ToString();
			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

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
				bool useSMS = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0) == 0 ? false : true;
				string strSMSText = WebDBManager.GetStringField(arrResult[i + 8], "");
				string strSMSExternalTeamIDList = WebDBManager.GetStringField(arrResult[i + 9], "");
				bool useEFax = WebDBManager.GetIntField(arrResult[i + 10].ToString(), 0) == 0 ? false : true;
				string strFaxExternalTeamIDList = WebDBManager.GetStringField(arrResult[i + 11], "");

                // Default Valign - Center
                int nValign = WebDBManager.GetIntField(arrResult[i + 12].ToString(), 2);
                // Default Haling - Left
                int nHalign = WebDBManager.GetIntField(arrResult[i + 13].ToString(), 2);

				Sections.SectionExternal section = new Sections.SectionExternal(panel, x, y);
				Sections.SectionDataExternal sectionData = (Sections.SectionDataExternal)section.Data;
				dicSections[nID] = section;
				arrSections.Add(section);

				panel.Sections.Add(section);
				panel.SetComponentID(section, nID);

				section.RectSize = new SizeF(fWidth, fHeight);
				section.Title = strText;

				sectionData.ID = nID;
				sectionData.Title = strText;
				sectionData.ComponentID = strComponentID;
				sectionData.UseSMS = useSMS;
				sectionData.UseFax = useEFax;
				sectionData.SMSMessage = strSMSText;

                sectionData.TextVerticalAlign = (Sections.SectionData.TextVAlign)nValign;
                sectionData.TextHorizontalAlign = (Sections.SectionData.TextHAlign)nHalign;

				if (!GetExternalTeamList(strSMSExternalTeamIDList, sectionData.SMSReceivers, dicExternal))
					return false;
				if (!GetExternalTeamList(strFaxExternalTeamIDList, sectionData.FaxReceivers, dicExternal))
					return false;
			}

			return true;
		}

		private bool LoadTransmission(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberData data, Dictionary<int, Sections.ExternalTeamData> dicExternal)
		{
			string strSQL = "select id, x, y, width, height, text, ComponentID, useInternalPopupMessage, useInternalMobileApp, useInternalBroadcast, "
                + "useExternalSMS, externalSMSText, SMSExternalTeamIDList, useExternalFax, FaxExternalTeamIDList, InternalBroadcastMessage, valign, halign from Transmission where StepMemberID = " + data.StepMemberID.ToString();
			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null)
				return false;

			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 17;  i += 18)
			{
				int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				float x = WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0.0f);
				float y = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
				float fWidth = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
				float fHeight = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
				string strText = WebDBManager.GetStringField(arrResult[i + 5], "");
				string strComponentID = WebDBManager.GetStringField(arrResult[i + 6], "");

				bool useInternalPopupMessage = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0) == 0 ? false : true;
				bool useInternalMobileApp = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 0) == 0 ? false : true;
				bool useInternalBroadcast = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 0) == 0 ? false : true;

				bool useExternalSMS = WebDBManager.GetIntField(arrResult[i + 10].ToString(), 0) == 0 ? false : true;
				string strExternalSMSText = WebDBManager.GetStringField(arrResult[i + 11], "");
				string strSMSExternalTeamIDList = WebDBManager.GetStringField(arrResult[i + 12], "");
				bool useExternalFax = WebDBManager.GetIntField(arrResult[i + 13].ToString(), 0) == 0 ? false : true;
				string strFaxExternalTeamIDList = WebDBManager.GetStringField(arrResult[i + 14], "");
				string szMessage = WebDBManager.GetStringField(arrResult[i + 15], "");
				if (szMessage == null || szMessage.Equals("null"))
					szMessage = "";

                // Default Valign - Center
                int nValign = WebDBManager.GetIntField(arrResult[i + 16].ToString(), 2);
                // Default Haling - Left
                int nHalign = WebDBManager.GetIntField(arrResult[i + 17].ToString(), 2);

				Sections.SectionTransmission section = new Sections.SectionTransmission(panel, x, y);
				Sections.SectionDataTransmission sectionData = (Sections.SectionDataTransmission)section.Data;
				dicSections[nID] = section;
				arrSections.Add(section);
				panel.Sections.Add(section);

				panel.SetComponentID(section, nID);

				section.RectSize = new SizeF(fWidth, fHeight);
				section.Title = strText;

				sectionData.ID = nID;
				sectionData.Title = strText;
				sectionData.ComponentID = strComponentID;

				sectionData.DataInternal.UsePopupMessage = useInternalPopupMessage;
				sectionData.DataInternal.UseMobileApp = useInternalMobileApp;
				sectionData.DataInternal.UseBroadcast = useInternalBroadcast;
				sectionData.DataInternal.BroadcastMessage = szMessage;

				sectionData.DataExternal.UseSMS = useExternalSMS;
				sectionData.DataExternal.UseFax = useExternalFax;
				sectionData.DataExternal.SMSMessage = strExternalSMSText;

                sectionData.TextVerticalAlign = (Sections.SectionData.TextVAlign)nValign;
                sectionData.TextHorizontalAlign = (Sections.SectionData.TextHAlign)nHalign;

				if (!GetExternalTeamList(strSMSExternalTeamIDList, sectionData.DataExternal.SMSReceivers, dicExternal))
					return false;
				if (!GetExternalTeamList(strFaxExternalTeamIDList, sectionData.DataExternal.FaxReceivers, dicExternal))
					return false;
			}

			return true;
		}

		private Dictionary<int, Sections.Section> GetSectionDictionary(int nSectionType, Dictionary<int, Sections.Section> dicProcessSections, Dictionary<int, Sections.Section> dicDecisionSections, Dictionary<int, Sections.Section> dicAnnotationSections, Dictionary<int, Sections.Section> dicEndPointSections, Dictionary<int, Sections.Section> dicLinkSections, Dictionary<int, Sections.Section> dicTransSOPSections, Dictionary<int, Sections.Section> dicInternalSections, Dictionary<int, Sections.Section> dicExternalSections, Dictionary<int, Sections.Section> dicTransmissionSections)
		{
			switch (nSectionType)
			{
				case (int)Sections.Section.ComponentType.PROCESS:
					return dicProcessSections;

				case (int)Sections.Section.ComponentType.DECISION:
					return dicDecisionSections;

				case (int)Sections.Section.ComponentType.ANNOTATION:
					return dicAnnotationSections;

				case (int)Sections.Section.ComponentType.ENDPOINT:
					return dicEndPointSections;

				case (int)Sections.Section.ComponentType.LINK:
					return dicLinkSections;

				case (int)Sections.Section.ComponentType.TRANSSOP:
					return dicTransSOPSections;

				case (int)Sections.Section.ComponentType.INTERNAL:
					return dicInternalSections;

				case (int)Sections.Section.ComponentType.EXTERNAL:
					return dicExternalSections;

				case (int)Sections.Section.ComponentType.TRANSMISSION:
					return dicTransmissionSections;
			}

			return null;
		}

		private bool LoadArrow(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicProcessSections, Dictionary<int, Sections.Section> dicDecisionSections, Dictionary<int, Sections.Section> dicAnnotationSections, Dictionary<int, Sections.Section> dicEndPointSections, Dictionary<int, Sections.Section> dicLinkSections, Dictionary<int, Sections.Section> dicTransSOPSections, Dictionary<int, Sections.Section> dicInternalSections, Dictionary<int, Sections.Section> dicExternalSections, Dictionary<int, Sections.Section> dicTransmissionSections, StepMemberData data)
		{
			string strSQL = "select ID, Text, BeginComponentID, BeginComponentPosition, EndComponentID, EndComponentPosition from Arrow where StepMemberID = " + data.StepMemberID.ToString();
			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null)
				return false;

			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 5; i += 6)
			{
				int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				string strText = WebDBManager.GetStringField(arrResult[i + 1], "");
				int nBeginComponentID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
				int nBeginComponentPosition = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0);
				int nEndComponentID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
				int nEndComponentPosition = WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0);

				int nBeginType = nBeginComponentID >> 24;
				nBeginComponentID = nBeginComponentID & 0xffffff;
				Dictionary<int, Sections.Section> dicBeginSection = GetSectionDictionary(nBeginType, dicProcessSections, dicDecisionSections, dicAnnotationSections, dicEndPointSections, dicLinkSections, dicTransSOPSections, dicInternalSections, dicExternalSections, dicTransmissionSections);

				// nBeginType, 즉 nBeginComponentID가 잘못 입력된 경우
				if (dicBeginSection == null)
					return false;

				int nEndType = nEndComponentID >> 24;
				nEndComponentID = nEndComponentID & 0xffffff;
				Dictionary<int, Sections.Section> dicEndSection = GetSectionDictionary(nEndType, dicProcessSections, dicDecisionSections, dicAnnotationSections, dicEndPointSections, dicLinkSections, dicTransSOPSections, dicInternalSections, dicExternalSections, dicTransmissionSections);

				// nEndType, 즉 nEndComponentID가 잘못 입력된 경우
				if (dicEndSection == null)
					return false;

				// 존재하지 않는 Section과 연결되어 있는 경우
				if (!dicBeginSection.ContainsKey(nBeginComponentID))
					return false;
				if (!dicEndSection.ContainsKey(nEndComponentID))
					return false;

				Sections.Section sectionBegin = dicBeginSection[nBeginComponentID];
				Sections.Section sectionEnd = dicEndSection[nEndComponentID];

				Sections.Arrow arrow = new Sections.Arrow();

				arrow.BeginLink = sectionBegin;
				arrow.EndLink = sectionEnd;
				arrow.Text = strText;

				Sections.Arrow.ArrowPosition posBegin, posEnd;

				if (!Sections.Arrow.IntToArrowPosition(nBeginComponentPosition, out posBegin))
					return false;
				if (!Sections.Arrow.IntToArrowPosition(nEndComponentPosition, out posEnd))
					return false;

				arrow.BeginPosition = posBegin;
				arrow.EndPosition = posEnd;

				sectionBegin.AddArrow(arrow);
				sectionEnd.AddArrow(arrow);

				arrow.CalcArrowLine();

				SetProcessButton(arrow);
			}

			return true;
		}

		// 종료 Section에 한해서만 화살표 없이 Section 바닥에 ProcessButton을 추가한다.
		private void SetProcessButton(Sections.SectionEndPoint sectionEnd)
		{
			Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)sectionEnd.Data;

            bool isStart = false;
            if (data.IsBegin)
                isStart = true;

            ProcessRectButtonManager mgr = null;

            if (sectionEnd.GetSectionPainter(0) == null)
			{
				mgr = new ProcessRectButtonManager();
                sectionEnd.AddSectionPainter(mgr);

				ProcessButtonRect btn = new ProcessButtonRect(FormSOP.Instance, false );
                btn.SetStartButtn(isStart);

				btn.Position = Sections.Arrow.ArrowPosition.BOTTOM;
                btn.Status = ProcessButtonRect.ButtonStatus.WAIT;

				mgr.Section = sectionEnd;
				mgr.SetButton(btn);
			}
			else
			{
                mgr = (ProcessRectButtonManager)sectionEnd.GetSectionPainter(0);
                ProcessButtonRect btn =(ProcessButtonRect) mgr.FindButton();

				if (btn != null)
					return;

                btn = new ProcessButtonRect(FormSOP.Instance, false);
                btn.SetStartButtn(isStart);
				btn.Position = Sections.Arrow.ArrowPosition.BOTTOM;
                btn.Status = ProcessButtonRect.ButtonStatus.WAIT;

				mgr.Section = sectionEnd;
				mgr.SetButton(btn);
			}
		}
        private void SetConfirmButton(Sections.Section section, bool bNotEnd = true)
        {

            Sections.Section.ComponentType type = section.GetComponentType();
            // 주석에는 ProcessButton을 붙이지 않는다.
            if (type == Sections.Section.ComponentType.ANNOTATION ||
                type == Sections.Section.ComponentType.ANNOTATION)
                return;

            bool isRect = true;
            if (type == Sections.Section.ComponentType.DECISION || type == Sections.Section.ComponentType.ENDPOINT)
                return;

            bool bProcess = true;
            if (type == Sections.Section.ComponentType.INTERNAL || type == Sections.Section.ComponentType.EXTERNAL
                        || type == Sections.Section.ComponentType.TRANSMISSION)
            {
                bProcess = false;
            }

            ConfirmButtonManager mgr = null;

            if (section.GetSectionPainter(1) == null)
            {
                mgr = new ConfirmButtonManager();
                section.AddSectionPainter(mgr);
                GetConfirmButton(section, mgr, isRect, bProcess);
            }
            else
            {
                if (isRect == false)
                {
                    mgr = (ConfirmButtonManager)section.GetSectionPainter(1);
                    ConfirmButton btn = mgr.FindButton();
                    if (btn != null)
                    {
                        //if (!btn.Data.Arrows.Contains(arrow))
                        //	btn.Data.Arrows.Add(arrow);
                        return;
                    }
                    GetConfirmButton(section, mgr, isRect, bProcess);
                }
            }
        }
		private void SetProcessButton(Sections.Section section, bool bNotEnd = true)
		{
            Sections.Section.ComponentType type = section.GetComponentType();
			// 주석에는 ProcessButton을 붙이지 않는다.
            if (type== Sections.Section.ComponentType.ANNOTATION ||
                type== Sections.Section.ComponentType.ANNOTATION)
				return;
            
            bool isRect = true;
            if (section.GetComponentType() == Sections.Section.ComponentType.DECISION)
                return;

            bool bProcess = true;
            if (type == Sections.Section.ComponentType.INTERNAL || type == Sections.Section.ComponentType.EXTERNAL
                        || type == Sections.Section.ComponentType.TRANSMISSION)
            {
                bProcess = false;
            }

            ProcessRectButtonManager mgr = null;

            if (section.GetSectionPainter(0) == null)
			{
                mgr = new ProcessRectButtonManager();
                section.AddSectionPainter(mgr);
                GetProcessButton(section, mgr, isRect, bProcess);
			}
			else
			{
                if (isRect == false)
                {
                    mgr = (ProcessRectButtonManager)section.GetSectionPainter(0);
                    ProcessButton btn = mgr.FindButton();
                    if (btn != null)
                    {
                        //if (!btn.Data.Arrows.Contains(arrow))
                        //	btn.Data.Arrows.Add(arrow);
                        return;
                    }
                    GetProcessButton(section, mgr, isRect, bProcess);
                }                
			}
		}

        private ConfirmButton GetConfirmButton(Sections.Section section, ConfirmButtonManager mgr, bool bRect = false, bool bProcess = true)
        {
            ConfirmButton btn = null;

            btn = new ConfirmButton(FormSOP.Instance, bProcess);

            btn.Status = ConfirmButton.ButtonStatus.WAIT;

            mgr.Section = section;
            mgr.SetButton(btn);

            return btn;
        }


		private ProcessButton GetProcessButton(Sections.Section section, ProcessRectButtonManager mgr, bool bRect = false, bool bProcess = true)
		{
            ProcessButton btn = null;
            if( bRect == true)
            {
                btn = new ProcessButtonRect(FormSOP.Instance, bProcess);
            }
            else
            {
                btn = new ProcessButton(FormSOP.Instance);
            }
			
            //btn.Position = arrow.BeginPosition;

            //if (!btn.Data.Arrows.Contains(arrow))
            //    btn.Data.Arrows.Add(arrow);

			btn.Status = ProcessButton.ButtonStatus.WAIT;

            mgr.Section = section;
			mgr.SetButton( btn);

			return btn;
		}

        private void SetProcessButton(Sections.Arrow arrow)
        {
            // 주석에는 ProcessButton을 붙이지 않는다.
            if (arrow.BeginLink.GetComponentType() == Sections.Section.ComponentType.ANNOTATION ||
                arrow.EndLink.GetComponentType() == Sections.Section.ComponentType.ANNOTATION)
                return;


            bool isRect = true;
            if (arrow.BeginLink.GetComponentType() == Sections.Section.ComponentType.DECISION)
                isRect = false;

            if( isRect == true)
                return;

            ProcessButtonManager mgr = null;

            if (arrow.BeginLink.GetSectionPainter(0) == null)
            {
                mgr = new ProcessButtonManager();

                arrow.BeginLink.AddSectionPainter( mgr);

                GetProcessButton(arrow, mgr);
            }
            else
            {

                mgr = (ProcessButtonManager)arrow.BeginLink.GetSectionPainter(0);
                ProcessButton btn = mgr.FindButton(arrow.BeginPosition);

                if (btn != null)
                {
                    if (!btn.Data.Arrows.Contains(arrow))
                    	btn.Data.Arrows.Add(arrow);
                    return;
                }

                GetProcessButton(arrow, mgr);
                

            }
        }

        private ProcessButton GetProcessButton(Sections.Arrow arrow, ProcessButtonManager mgr)
        {
            ProcessButton btn = null;
            
            btn = new ProcessButton(FormSOP.Instance);
            

            btn.Position = arrow.BeginPosition;

            if (!btn.Data.Arrows.Contains(arrow))
                btn.Data.Arrows.Add(arrow);

            btn.Status = ProcessButton.ButtonStatus.WAIT;

            mgr.Section = arrow.BeginLink;
            mgr.SetButton(arrow.BeginPosition, btn);

            return btn;
        }

        private bool LoadPanelComponent(WebDBManager dbMgr, Sections.PanelSectionEx panel, StepMemberData data, ArrayList arrLink, ArrayList arrSections, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined, ref Dictionary<int, Sections.ExternalTeamData> dicExternal, ref Dictionary<int, string> dicRegular, ref Dictionary<int, string> dicControlRoom)
		{
			// 화살표 연결을 위하여 Section 정보를 임시 저장
			// ComponentID, Section
			Dictionary<int, Sections.Section> dicProcessSections = new Dictionary<int, Sections.Section>();
			Dictionary<int, Sections.Section> dicDecisionSections = new Dictionary<int, Sections.Section>();
			Dictionary<int, Sections.Section> dicAnnotationSections = new Dictionary<int, Sections.Section>();
			Dictionary<int, Sections.Section> dicEndPointSections = new Dictionary<int, Sections.Section>();
			Dictionary<int, Sections.Section> dicLinkSections = new Dictionary<int, Sections.Section>();
			Dictionary<int, Sections.Section> dicTransSOPSections = new Dictionary<int, Sections.Section>();
			Dictionary<int, Sections.Section> dicInternalSections = new Dictionary<int, Sections.Section>();
			Dictionary<int, Sections.Section> dicExternalSections = new Dictionary<int, Sections.Section>();
			Dictionary<int, Sections.Section> dicTransmissionSections = new Dictionary<int, Sections.Section>();

			if (!LoadProcess(dbMgr, dicProcessSections, arrSections, panel, data, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom))
				return false;
			if (!LoadDecision(dbMgr, dicDecisionSections, arrSections, panel, data))
				return false;
			if (!LoadAnnotation(dbMgr, dicAnnotationSections, arrSections, panel, data))
				return false;
			if (!LoadEndPoint(dbMgr, dicEndPointSections, arrSections, panel, data))
				return false;
			if (!LoadLink(dbMgr, dicLinkSections, arrSections, arrLink, panel, data))
				return false;
			if (!LoadTransSOP(dbMgr, dicTransSOPSections, arrSections, panel, data))
				return false;
            if (!LoadInternal(dbMgr, dicInternalSections, arrSections, panel, data, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom))
				return false;
			if (!LoadExternal(dbMgr, dicExternalSections, arrSections, panel, data, dicExternal))
				return false;
			if (!LoadTransmission(dbMgr, dicTransmissionSections, arrSections, panel, data, dicExternal))
				return false;

			if (!LoadArrow(dbMgr, dicProcessSections, dicDecisionSections, dicAnnotationSections, dicEndPointSections, dicLinkSections, dicTransSOPSections, dicInternalSections, dicExternalSections, dicTransmissionSections, data))
				return false;
            
            foreach(Sections.Section section in arrSections)
            {
                SetProcessButton(section);
                SetConfirmButton(section);
            }

            SetSectionNumber(arrSections, panel);

			return true;
		}

        private void SetSectionNumber(ArrayList arrSections, Sections.PanelSectionEx panel)
        {
            Sections.SectionEndPoint sectionBegin = null;

            foreach (Sections.Section section in arrSections)
            {
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
            Dictionary<int, List<Sections.Section>> dicDepthSections = new Dictionary<int,List<Sections.Section>>();
            int depth = 1, number = 1;

            SetSectionNumber(sectionBegin, allSections, dicDepthSections, depth);

            for (int i=depth;;i++)
            {
                List<Sections.Section> sections = null;

                if (!dicDepthSections.TryGetValue(i, out sections))
                    break;

                foreach (Sections.Section section in sections)
                {
                    if (section.Data.SectionNumber < 0)
                        section.Data.SectionNumber = number++;
                }
            }

            panel.VisibleSectionNumber = true;
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

		// Return 값 : ActionStepID, StepMemberData List
		private Dictionary<int, ArrayList> LoadStepMemberData(WebDBManager dbMgr, ArrayList arrActionSteps, ArrayList arrTeams)
		{
			string strActionStepIDs = "";

			foreach (ActionStepInfo actionStep in arrActionSteps)
			{
				if (strActionStepIDs.Length == 0)
					strActionStepIDs = actionStep.ActionStepID.ToString();
				else
					strActionStepIDs += ", " + actionStep.ActionStepID.ToString();
			}

			if (strActionStepIDs.Length == 0)
				return null;

			string strTeamIDs = "";

			foreach (StepMemberData data in arrTeams)
			{
				if (strTeamIDs.Length == 0)
					strTeamIDs = data.TeamID.ToString();
				else
					strTeamIDs += ", " + data.TeamID.ToString();
			}

			if (strTeamIDs.Length == 0)
				return null;

			string strSQL = string.Format("select id, TeamID, TeamType, ActionStepID from StepMember where ActionStepID in ({0}) and TeamID in ({1})", strActionStepIDs, strTeamIDs);
			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
			if (arrResult == null)
				return null;

			int nResultCount = arrResult.Count;
			if (nResultCount == 0)
				return null;

			Dictionary<int, ArrayList> dicStepMembers = new Dictionary<int, ArrayList>();

			for (int i = 0; i < nResultCount - 3; i += 4)
			{
				int nStepMemberID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				int nTeamID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
				int nTeamType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
				int nActionStepID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0);

				//StepMemberDataEx data = new StepMemberDataEx(nTeamID, nTeamType, nStepMemberID);
				StepMemberData data = new StepMemberData();
				data.TeamID = nTeamID;
				data.TeamType = nTeamType;
				data.StepMemberID = nStepMemberID;

				ArrayList arrStepMembers = null;

				if (dicStepMembers.ContainsKey(nActionStepID))
					arrStepMembers = dicStepMembers[nActionStepID];
				else
				{
					arrStepMembers = new ArrayList();
					dicStepMembers[nActionStepID] = arrStepMembers;
				}

				arrStepMembers.Add(data);
			}

			return dicStepMembers;
		}

		private StepMemberData FindStepMemberData(Sections.PanelSectionEx panel, ArrayList arrStepMemberData, out bool isSuccess)
		{
			foreach (StepMemberData data in arrStepMemberData)
			{
				if (data.TeamID == panel.TeamID && data.TeamType == panel.TeamType)
				{
					isSuccess = true;
					return data;
				}
			}

			isSuccess = false;
			return new StepMemberData();
		}

		private Sections.Section FindSection(string strComponentID, ArrayList arrSections)
		{
			foreach (Sections.Section section in arrSections)
			{
				if (section.Data.ComponentID == strComponentID)
					return section;
			}

			return null;
		}

		private bool SetLinkSections(ArrayList arrLink, ArrayList arrSections)
		{
			foreach (Sections.SectionLink link in arrLink)
			{
				Sections.SectionDataLink dataLink = (Sections.SectionDataLink)link.Data;
				string strLinkedComponentID = dataLink.Title;

				Sections.Section sectionLinked = FindSection(strLinkedComponentID, arrSections);

				if (sectionLinked == null)
				{
					// 존재하지 않는 Link
					return false;
				}

				dataLink.LinkedSection = sectionLinked;
				dataLink.Title = link.Title;
			}

			return true;
		}

		private TabPage GetTabPage(int nActionID, ArrayList arrTabPages)
		{
			int nPageCount = arrTabPages.Count;

			for (int i = nPageCount - 1; i >= 0; i--)
			{
				SectionTabPage page = (SectionTabPage)arrTabPages[i];

				if (page.ActionStepID == nActionID)
					return page;
			}

			return null;
		}        

		private ArrayList LoadActionSteps(WebDBManager dbMgr, ArrayList arrActionSteps)
		{
			string strIDs = "";

			foreach (ActionStepInfo actionStep in arrActionSteps)
			{
				if (strIDs.Length == 0)
					strIDs = actionStep.ActionStepID.ToString();
				else
					strIDs += ", " + actionStep.ActionStepID.ToString();
			}

            string strSQL = string.Format("Select ID, StepName, PeriodType, BeginTime, EndTime, WeekDayOption, Iteration, IterationType, ProcessTime, ProcessTimeType, ParentStepID, UserDefinedConfigID from ActionStep where ID in ({0})", strIDs);
			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null)
				return null;

			int nResultCount = arrResult.Count;
			if (nResultCount == 0)
				return null;

			DateTime dtDefault = new DateTime();
			ArrayList arrStepDatas = new ArrayList();

			for (int i = 0; i < nResultCount - 11; i += 12)
			{
				int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				string strStepName = WebDBManager.GetStringField(arrResult[i + 1], "");
				int nPeriodType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
				DateTime dtBegin = WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
				DateTime dtEnd = WebDBManager.GetDateTimeField(arrResult[i + 4], dtDefault);
				int nWeekdayOption = WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0);
				int nIteration = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);
				int nIterationType = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
				int nProcessTime = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 0);
				int nProcessTimeType = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 0);
				int nParentStepID = WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);
                VariousData<int> userDefinedConfigID = WebDBManager.GetIntField(arrResult[i + 11].ToString());

				Data_ActionStep data = new Data_ActionStep();

				data.ID = nID;
				data.StepName = strStepName;
				data.PeriodType = nPeriodType;
				data.BeginTime = dtBegin;
				data.EndTime = dtEnd;
				data.WeekdayOption = nWeekdayOption;
				data.Iteration = nIteration;
				data.IterationType = nIterationType;
				data.ProcessTime = nProcessTime;
				data.ProcessTimeType = nProcessTimeType;
				data.ParentStepID = nParentStepID;

                if (userDefinedConfigID != null)
                    data.UserDefinedConfig = LoadUserDefinedConfig(dbMgr, userDefinedConfigID.Data);

				arrStepDatas.Add(data);
			}

			return arrStepDatas;
		}

        private ConfigData LoadUserDefinedConfig(WebDBManager dbMgr, int nID)
        {
            ConfigData data = null;

            if (m_dicUserDefinedConfig.TryGetValue(nID, out data))
                return data;

            // Variable이 없는 ConfigData는 의미가 없으므로 하나의 쿼리만 사용한다.
            string strSQL = "Select uc.ConfigName, uv.VariableName, uv.VariableType, uv.Description ";
            strSQL += "from UserDefinedConfig as uc, UserDefinedConfigVariable as uv ";
            strSQL += "where uc.ID = " + nID.ToString() + " and uc.ID = uv.ConfigID order by uv.No";

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            if (nResultCount == 0)
                return null;

            data = new ConfigData();

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                string strConfigName = WebDBManager.GetStringField(arrResult[i]);
                string strVariableName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> type = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                string strDescription = WebDBManager.GetStringField(arrResult[i + 3]);

                if (strConfigName == null || strVariableName == null || type == null)
                    continue;

                Sections.SectionDataDecision.VariableType variableType = Sections.SectionDataDecision.ToVariableType(type.Data);

                if (variableType == Sections.SectionDataDecision.VariableType.UNKNOWN)
                    continue;

                if (strDescription == null)
                    strDescription = "";

                data.Text = strConfigName;

                SOPParameter param = new SOPParameter();
                param.VariableName = strVariableName;
                param.Type = variableType;
                param.Description = strDescription;

                data.Variables.Add(param);
            }

            if (data.Variables.Count == 0)
                return null;

            m_dicUserDefinedConfig[nID] = data;
            return data;
        }

		private bool LoadPane(WebDBManager dbMgr, PageBackstageSOP pageHome, ArrayList arrActionSteps, ArrayList arrTeams)
		{
			Dictionary<int, ArrayList> dicStepMembers = LoadStepMemberData(dbMgr, arrActionSteps, arrTeams);
			if (dicStepMembers == null)
				return false;

			ArrayList arrStepDatas = LoadActionSteps(dbMgr, arrActionSteps);
			if (arrStepDatas == null)
				return false;

			// ActionStepID, TabPage
			Dictionary<int, TabPage> dicActionStep = new Dictionary<int, TabPage>();

			foreach (Data_ActionStep data in arrStepDatas)
			{
				TabPage page = pageHome.AddTabPage(data);
				dicActionStep[data.ID] = page;
			}

			/*foreach (ActionStepInfo actionStep in arrActionSteps)
			{
				TabPage page = pageLevel.AddTabPage(actionStep);
				dicActionStep[actionStep.ActionStepID] = page;
			}*/

			// TeamID, Team Name
			Dictionary<int, string> dicNormal = null;
			Dictionary<int, string> dicEmergency = null;
			Dictionary<int, string> dicUserDefined = null;
			Dictionary<int, Sections.ExternalTeamData> dicExternal = ReadExternalTeamList(dbMgr);
			Dictionary<int, string> dicRegular = null;
            Dictionary<int, string> dicControlRoom = null;
			
			foreach (ActionStepInfo actionStep in arrActionSteps)
			{
				if (actionStep.ParentStepID > 0)
				{
					TabPage pageCurrent = dicActionStep[actionStep.ActionStepID];

					if (dicActionStep.ContainsKey(actionStep.ParentStepID))
					{
						TabPage pageParent = dicActionStep[actionStep.ParentStepID];
						// 부모 단계가 존재할 경우 Tag에 부모 단계를 넣는다.
						pageCurrent.Tag = pageParent;
						//pageHome.GetDockPropertiesLevel().GetLevelProperties(pageCurrent);
					}
				}

				if (!dicStepMembers.ContainsKey(actionStep.ActionStepID))
					continue;

				ArrayList arrStepMemberData = dicStepMembers[actionStep.ActionStepID];

				TabPage tabPage = GetTabPage(actionStep.ActionStepID, pageHome.GetTabPage());
				if (tabPage == null)
					continue;


                string szDisasterName = GetDisasterName(actionStep.ActionStepID);
				//pageLevel.AddTabPage(actionStep);
				SectionTabPage page = (SectionTabPage)tabPage;
                
                if( page.CreateNew == true)
                {
                    int i = 0;
                    i++;
                }

				if (page.CreateNew == true)
				{
					ArrayList arrPanels = pageHome.AddPane(arrTeams, actionStep.ActionStepID, tabPage);

                    bool isNormal, isRegular;
                    string strAdd = "";

                    if (FormSOP.Instance.SOPManager.GetDisaster(actionStep.DisasterID, out isNormal, out isRegular) != null)
                    //if (FormSOP.Instance.SOPManager.GetDisasterInfo(actionStep.DisasterID, out isNormal, out isRegular))
                    {
                        if (!isNormal)
                            strAdd = "(야간)";
                    }

                    foreach (Sections.PanelSectionEx pane in arrPanels)
                    {
                        pane.AddPanelTitle(szDisasterName + strAdd);
                        pane.ShowSectionButton(FormSOP.Instance.ShowSectionBtn);
                    }

					if (!LoadNewPanelComponent(dbMgr, arrPanels, arrStepMemberData, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom))
						return false;

                    pageHome.CreateComponentContentsSet(page);
                    //pageHome.CreatePreviewComponentContents(page);
                    page.CreateNew = false;
				  
				}
				else
				{
					page.ReSizePanel();
					foreach (Control control in  page.Controls)
					{
						if (control.GetType() == typeof(Sections.PanelSectionEx))
						{
                            Sections.PanelSectionEx pane = (Sections.PanelSectionEx)control;
                            pane.ShowSectionButton(FormSOP.Instance.ShowSectionBtn);
							FormSOP.Instance.GetPageHome().PanelArray.Add(control);
						}
					}

				}

                page.CreateNew = false;
			}

			return true;
		}

        /*public static void TraceLog(DateTime dtPrev, string strLog)
        {
            TimeSpan span = DateTime.Now - dtPrev;
            System.Diagnostics.Trace.WriteLine(strLog + " : " + span.TotalSeconds.ToString() + "초");
        }*/

        public string GetDisasterName(int nActionStepID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT dis.DisasterName FROM ActionStep AS step ");
            sb.Append(" INNER JOIN Disaster AS dis ON dis.ID = step.DisasterID ");
            sb.AppendFormat(" WHERE step.ID = {0}", nActionStepID);

            string szSQL = sb.ToString();

            ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(szSQL, 0);
            if (arrResult == null || arrResult.Count == 0)
                return "";

            string strDisName = WebDBManager.GetStringField(arrResult[0], "");
            return strDisName;
        }

        public bool LoadNewPanelComponent(WebDBManager dbMgr, ArrayList arrPanels, ArrayList arrStepMemberData, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined, ref Dictionary<int, Sections.ExternalTeamData> dicExternal, ref Dictionary<int, string> dicRegular, ref Dictionary<int, string> dicControlRoom)
		{
			// Link 객체는 다른 Panel의 객체와 연결되어야 하므로, DB로부터 모든 객체를 읽어들인 후에 해당 객체와 연결시킨다.
			// Link 객체는 같은 Step내의 객체들과만 연결된다.
			// arrSections는 Step내의 모든 Section 객체를 담게 되는데, Link 객체와 연결하기 위해서다.
			ArrayList arrLink = new ArrayList();
			ArrayList arrSections = new ArrayList();

			foreach (Sections.PanelSectionEx panel in arrPanels)
			{
				bool isSuccess;
				StepMemberData data = FindStepMemberData(panel, arrStepMemberData, out isSuccess);
				if (!isSuccess)
					continue;
				
				if (!LoadPanelComponent(dbMgr, panel, data, arrLink, arrSections, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom))
					return false;
			}
            
			if (!SetLinkSections(arrLink, arrSections))
				return false;

			return true;
		}

		private int FindStepMemberTeamIndex(int nTeamID, int nTeamType, ArrayList arrTeams)
		{
			int nTeamCount = arrTeams.Count;

			for (int i = 0; i < nTeamCount; i++)
			{
				StepMemberData data = (StepMemberData)arrTeams[i];

				if (data.TeamID == nTeamID && data.TeamType == nTeamType)
					return i;
			}

			return -1;
		}

		private void GetStepMemberTeamName(ArrayList arrStepMembers, string strTableName, int nTeamType, ArrayList arrTeams, WebDBManager dbMgr)
		{
			string strTeamIDs = "";

			foreach (StepMemberData data in arrStepMembers)
			{
				if (strTeamIDs.Length == 0)
					strTeamIDs = data.TeamID.ToString();
				else
					strTeamIDs += ", " + data.TeamID.ToString();
			}

			if (strTeamIDs.Length == 0)
				return;

			string strSQL = string.Format("select ID, TeamName from {0} where ID in ({1})", strTableName, strTeamIDs);
			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

			int nResultCount = arrResult.Count;
			int nStepMemberCount = arrStepMembers.Count;

			for (int i = 0; i < nResultCount - 1; i += 2)
			{
				int nTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				string strTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");

				int nIndex = FindStepMemberTeamIndex(nTeamID, nTeamType, arrTeams);

				if (nIndex >= 0)
				{
					StepMemberData data = new StepMemberData(strTeamName, nTeamID, nTeamType);
					arrTeams[nIndex] = data;
				}
				else
					return;
			}
		}

		private ArrayList LoadBarPage(PageBackstageSOP pageHome, ArrayList arrActionSteps, WebDBManager dbMgr)
		{
			if (arrActionSteps == null || arrActionSteps.Count == 0)
				return null;

			ActionStepInfo actionStep = (ActionStepInfo)arrActionSteps[0];
			string strSQL = string.Format("Select ID, TeamID, TeamType from StepMember where ActionStepID = {0}", actionStep.ActionStepID);

			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
			int nResultCount = arrResult.Count;

			ArrayList arrNormal = new ArrayList();
			ArrayList arrEmergency = new ArrayList();
			ArrayList arrExternal = new ArrayList();
			ArrayList arrUserDefined = new ArrayList();
			ArrayList arrRegular = new ArrayList();

			ArrayList arrTeams = new ArrayList();

			for (int i = 0; i < nResultCount - 2; i += 3)
			{
				int nStepMemberID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				int nTeamID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
				int nTeamType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);

				StepMemberData data = new StepMemberData("", nTeamID, nTeamType);
				arrTeams.Add(data);

				if (nTeamType == 0)
					arrNormal.Add(data);    // 평일 비상 조직
				else if (nTeamType == 1)
					arrEmergency.Add(data); // 야간 및 휴일 비상 조직
				else if (nTeamType == 2)
					arrExternal.Add(data);  // 외부 조직
				else if (nTeamType == 3)
					arrUserDefined.Add(data);   // 사용자 정의 조직
				else if (nTeamType == 4)
					arrRegular.Add(data);
			}

			GetStepMemberTeamName(arrNormal, "TemporaryNormalTeam", 0, arrTeams, dbMgr);
			GetStepMemberTeamName(arrEmergency, "TemporaryEmergencyTeam", 1, arrTeams, dbMgr);
			GetStepMemberTeamName(arrExternal, "ExternalTeam", 2, arrTeams, dbMgr);
			GetStepMemberTeamName(arrUserDefined, "UserDefinedTeam", 3, arrTeams, dbMgr);
			GetStepMemberTeamName(arrRegular, "RegularTeam", 4, arrTeams, dbMgr);

			ArrayList arrTeamNames = new ArrayList();

			foreach (StepMemberData stepMemberData in arrTeams)
			{
				if (stepMemberData.TeamName == "")
					return null;

				arrTeamNames.Add(stepMemberData.TeamName);
			}			
			return arrTeams;
		}
	}

	public struct StepMemberData
	{
		private string m_strTeamName;
		private int m_nTeamID;
		private int m_nTeamType;
		private int m_nStepMemberID;
		private int m_nLevelNo;
		
		public StepMemberData(string strTeamName, int nTeamID, int nTeamType)
		{
			m_strTeamName = strTeamName;
			m_nTeamID = nTeamID;
			m_nTeamType = nTeamType;
			m_nStepMemberID = -1;
			m_nLevelNo = -1;
		}

		

		public StepMemberData(string strTeamName, int nTeamID, int nTeamType, int nStepMemberID, int nLevelNo)
		{
			m_strTeamName = strTeamName;
			m_nTeamID = nTeamID;
			m_nTeamType = nTeamType;
			m_nStepMemberID = nStepMemberID;
			m_nLevelNo = nLevelNo;
		}

		public string TeamName
		{
			get { return m_strTeamName; }
			set { m_strTeamName = value; }
		}

		public int TeamID
		{
			get { return m_nTeamID; }
			set { m_nTeamID = value; }
		}

		public int TeamType
		{
			get { return m_nTeamType; }
			set { m_nTeamType = value; }
		}

		public int StepMemberID
		{
			get { return m_nStepMemberID; }
			set { m_nStepMemberID = value; }
		}

		public int LevelNo
		{
			get { return m_nLevelNo; }
			set { m_nLevelNo = value; }
		}
	}

}
