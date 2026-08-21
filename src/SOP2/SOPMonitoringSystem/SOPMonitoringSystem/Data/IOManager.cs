using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using DBUtility;

namespace SOPMonitoringSystem
{
    // SOP Data를 DB에 저장 및 불러오기 담당
    public class IOManager
    {
        private Dictionary<int, ArrayList> m_dicNormalRegularTeamID = null;
        private Dictionary<int, ArrayList> m_dicEmergencyRegularTeamID = null;
        // ExternalTeam ID, TeamData
        private static Dictionary<int, Sections.ExternalTeamData> m_dicExternal = null;

        public IOManager()
        {
        }

        public bool Load(FormMain frm, WebDBManager dbMgr, VersionInfo version, ArrayList arrActionSteps, string strCategoryName, string strSubCategoryName, string strDisasterName)
        {
            ClearSOP(frm);

            //Application.UseWaitCursor = true;
            frm.Cursor = Cursors.WaitCursor;

            PageBackstageHome pageHome = frm.GetPageHome();

            //string strFullPath = LoadDisasterTree(frm, strCategoryName, strSubCategoryName, strDisasterName, arrActionSteps);
            /*string strFullPath = GetFirstActionStepFullPath(strCategoryName, strSubCategoryName, strDisasterName, arrActionSteps);
            pageHome.GetDockPropertiesLevel().AddTitle(strFullPath);*/

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
        private bool AddRegularTeamID(string strRegularTeamLink, int nBeginIndex, int nEndIndex, ArrayList arrTeamID, ArrayList arrTeamGroupID)
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
        }

        // dicTeamName : TeamID, TeamName
        private bool ReadTeamList(WebDBManager dbMgr, string strTableName, Dictionary<int, string> dicTeamName, ref Dictionary<int, ArrayList> dicRegularTeamID)
        {
            string strSQL = "select id, TeamName, RegularTeamLink from " + strTableName;
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

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
            }

            return true;
        }

        // dicTeamName : TeamID, TeamName
        private bool ReadTeamList(WebDBManager dbMgr, string strTableName, Dictionary<int, string> dicTeamName)
        {
            string strSQL = "select id, TeamName from " + strTableName;
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");

                //if (strTeamName == "상황반장")
                //{
                 //   int a = 3;
                //}
                
                dicTeamName[nTeamID] = strTeamName;
            }

            return true;
        }

        public static Dictionary<int, Sections.ExternalTeamData> ReadExternalTeamList(WebDBManager dbMgr)
        {
            if (m_dicExternal != null)
                return m_dicExternal;

            string strSQL = "select id, TeamName, PhoneNumber, FaxNumber from ExternalTeam";
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

        private bool GetTeamName(WebDBManager dbMgr, ref Sections.SectionDataProcess sectionData, ref string strTeamNameList, string strTeamList, int nBeginIndex, int nEndIndex, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined, ref Dictionary<int, Sections.ExternalTeamData> dicExternal, ref Dictionary<int, string> dicRegular)
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
            Dictionary<int, ArrayList> dicRegualrTeamID = null;
            string strTeamName = null;

            if (strTeamType == "0")
            {
                if (dicNormal == null)
                {
                    dicNormal = new Dictionary<int, string>();
                    m_dicNormalRegularTeamID = new Dictionary<int, ArrayList>();
                    ReadTeamList(dbMgr, "TemporaryNormalTeam", dicNormal, ref m_dicNormalRegularTeamID);
                }

                dicTeamName = dicNormal;
                dicRegualrTeamID = m_dicNormalRegularTeamID;
            }
            else if (strTeamType == "1")
            {
                if (dicEmergency == null)
                {
                    dicEmergency = new Dictionary<int, string>();
                    m_dicEmergencyRegularTeamID = new Dictionary<int, ArrayList>();
                    ReadTeamList(dbMgr, "TemporaryEmergencyTeam", dicEmergency, ref m_dicEmergencyRegularTeamID);
                }

                dicTeamName = dicEmergency;
                dicRegualrTeamID = m_dicEmergencyRegularTeamID;
            }
            else if (strTeamType == "2")
            {
                try
                {
                    int nTeamID = int.Parse(strTeamID);

                    if (!dicExternal.ContainsKey(nTeamID))
                        return false;

                    strTeamName = dicExternal[nTeamID].TeamName;
                }
                catch (Exception)
                {
                    return false;
                }
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
            else
                return false;

            try
            {
                int nTeamID = int.Parse(strTeamID);

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

                ArrayList arrRegularTeamIDList = null;

                if (dicRegualrTeamID != null && dicRegualrTeamID.ContainsKey(nTeamID))
                {
                    arrRegularTeamIDList = dicRegualrTeamID[nTeamID];
                }

                int nLevelNo = GetLevelNumber(dbMgr, nTeamID, strTeamType);
                Sections.SOPTeam team = new Sections.SOPTeam();

                team.TeamID = nTeamID;
                team.TeamType = int.Parse(strTeamType);
                team.TeamName = strTeamName;
                team.LevelNo = nLevelNo;
                team.RegularTeamIDList = arrRegularTeamIDList;

                sectionData.TeamList.Add(team);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private int GetLevelNumber(WebDBManager dbMgr, int nTeamID, string strTeamType)
        {
            int nLevelNo = -1;
            string strSQL = "";

            if (strTeamType == "0")
                strSQL = "select ID, TeamName, LevelNo from TemporaryNormalTeam where ID = " + nTeamID.ToString();
            else if (strTeamType == "1")
                strSQL = "select ID, TeamName, LevelNo from TemporaryEmergencyTeam where ID = " + nTeamID.ToString();
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
        private string GetTeamList(WebDBManager dbMgr, string strTeamList, ref Sections.SectionDataProcess sectionData, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined, ref Dictionary<int, Sections.ExternalTeamData> dicExternal, ref Dictionary<int, string> dicRegular)
        {
            int nBeginIndex = 0;
            int nLen = strTeamList.Length;

            string strTeamNameList = "";

            while (nBeginIndex < nLen)
            {
                int nDotIndex = strTeamList.IndexOf(',', nBeginIndex);
                if (nDotIndex < 0) break;

                if (!GetTeamName(dbMgr, ref sectionData, ref strTeamNameList, strTeamList, nBeginIndex, nDotIndex, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular))
                    return "";

                nBeginIndex = nDotIndex + 1;
            }

            if (!GetTeamName(dbMgr, ref sectionData, ref strTeamNameList, strTeamList, nBeginIndex, nLen, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular))
                return "";

            return strTeamNameList;
        }

        private bool LoadProcessMission(WebDBManager dbMgr, int nProcessID, ArrayList arrMissionItems)
        {
            string strSQL = string.Format("Select ID, missionText, TransmissionType, missionTarget from ProcessMission where ProcessID = {0}", nProcessID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strMissionText = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nTransmission = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 2);
                string strTarget = WebDBManager.GetStringField(arrResult[i + 3], "");

                Sections.MissionItem item = new Sections.MissionItem();

                item.Mission = strMissionText;
                item.TransmissionType = nTransmission;

                if (strTarget == null || strTarget.Equals("null"))
                {
                    strTarget = "";
                }
                item.Target = strTarget;
                //item.Transmission = nTransmission;

                arrMissionItems.Add(item);
            }

            return true;
        }

        private bool LoadCheckedItems(WebDBManager dbMgr, int nProcessID, ArrayList arrCheckedItems)
        {
            string strSQL = string.Format("Select ID, Category, SubCategory, TaskName, TargetCount, Position from CheckTask where ProcessID = {0}", nProcessID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

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

        private bool LoadProcess(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberData data, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined, ref Dictionary<int, Sections.ExternalTeamData> dicExternal, ref Dictionary<int, string> dicRegular)
        {
            string strSQL = "select id, x, y, width, height, text, TeamList, ComponentID, ProcessTime, ProcessTimeType, useProcessTime, useMissionMessage";
            strSQL += ", onlyTeamLeader from Process where StepMemberID = " + data.StepMemberID.ToString();

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 12; i += 13)
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

                Sections.SectionProcess section = new Sections.SectionProcess(panel, x, y);
                Sections.SectionDataProcess sectionData = (Sections.SectionDataProcess)section.Data;
                dicSections[nID] = section;
                arrSections.Add(section);

                panel.Sections.Add(section);
                panel.SetComponentID(section, nID);

                section.RectSize = new SizeF(fWidth, fHeight);
                section.TextUP = strText;
                section.TextDown = GetTeamList(dbMgr, strTeamList, ref sectionData, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular);

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

                if (!LoadCheckedItems(dbMgr, nID, sectionData.CheckedItems))
                    return false;
            }

            return true;
        }

        private bool LoadDecision(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberData data)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID from Decision where StepMemberID = " + data.StepMemberID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

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
            }

            return true;
        }

        private bool LoadAnnotation(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberData data)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID from Annotation where StepMemberID = " + data.StepMemberID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

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
            }

            return true;
        }

        private bool LoadEndPoint(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberData data)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID, isBegin from EndPoint where StepMemberID = " + data.StepMemberID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

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

                // 종료 Section은 화살표 없이 ProcessButton을 추가한다.
                if (!sectionData.IsBegin)
                    SetProcessButton(section);
            }

            return true;
        }

        // arrLink : Link 객체는 다른 Panel의 객체와 연결되어야 하므로, DB로부터 모든 객체를 읽어들인 후에 해당 객체와 연결시킨다.
        private bool LoadLink(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, ArrayList arrLink, Sections.PanelSectionEx panel, StepMemberData data)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID, LinkedComponentID from Link where StepMemberID = " + data.StepMemberID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

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
                string strLinkedComponentID = WebDBManager.GetStringField(arrResult[i + 7], "");

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
            }

            return true;
        }

        private bool LoadTransSOP(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberData data)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID, LinkedActionStepID, Description from TransSOP where StepMemberID = " + data.StepMemberID.ToString();
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
                int nLinkedActionStepID = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
                string strDescription = WebDBManager.GetStringField(arrResult[i + 8], "");

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
            }

            return true;
        }

        private bool LoadInternal(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberData data)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID, usePopupMessage, useMobileApp, useBroadcast, BroadcastMessage from InternalTransmission where StepMemberID = " + data.StepMemberID.ToString();
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
                bool usePopupMessage = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0) == 0 ? false : true;
                bool useMobileApp = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 0) == 0 ? false : true;
                bool useBroadcast = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 0) == 0 ? false : true;
                string szMessage = WebDBManager.GetStringField(arrResult[i + 10], "");
                if (szMessage == null || szMessage.Equals("null"))
                    szMessage = "";
                
                Sections.SectionInternal section = new Sections.SectionInternal(panel, x, y);
                Sections.SectionDataInternal sectionData = (Sections.SectionDataInternal)section.Data;
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
            string strSQL = "select id, x, y, width, height, text, ComponentID, useSMS, SMSText, SMSExternalTeamIDList, useEFax, FaxExternalTeamIDList from ExternalTransmission where StepMemberID = " + data.StepMemberID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

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
                + "useExternalSMS, externalSMSText, SMSExternalTeamIDList, useExternalFax, FaxExternalTeamIDList, InternalBroadcastMessage from Transmission where StepMemberID = " + data.StepMemberID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 15;  i += 16)
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
            if (data.IsBegin) return;

            ProcessButtonManager mgr = null;

            if (sectionEnd.AdditionalPainter == null)
            {
                mgr = new ProcessButtonManager();
                sectionEnd.AdditionalPainter = mgr;

                ProcessButton btn = new ProcessButton();

                btn.Position = Sections.Arrow.ArrowPosition.BOTTOM;
                btn.Status = ProcessButton.ButtonStatus.WAIT;

                mgr.Section = sectionEnd;
                mgr.SetButton(btn.Position, btn);
            }
            else
            {
                mgr = (ProcessButtonManager)sectionEnd.AdditionalPainter;
                ProcessButton btn = mgr.FindButton(Sections.Arrow.ArrowPosition.BOTTOM);

                if (btn != null)
                    return;

                btn = new ProcessButton();

                btn.Position = Sections.Arrow.ArrowPosition.BOTTOM;
                btn.Status = ProcessButton.ButtonStatus.WAIT;

                mgr.Section = sectionEnd;
                mgr.SetButton(btn.Position, btn);
            }
        }

        private void SetProcessButton(Sections.Arrow arrow)
        {
            // 주석에는 ProcessButton을 붙이지 않는다.
            if (arrow.BeginLink.GetComponentType() == Sections.Section.ComponentType.ANNOTATION ||
                arrow.EndLink.GetComponentType() == Sections.Section.ComponentType.ANNOTATION)
                return;

            ProcessButtonManager mgr = null;

            if (arrow.BeginLink.AdditionalPainter == null)
            {
                mgr = new ProcessButtonManager();
                arrow.BeginLink.AdditionalPainter = mgr;

                GetProcessButton(arrow, mgr);
            }
            else
            {
                mgr = (ProcessButtonManager)arrow.BeginLink.AdditionalPainter;
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
            ProcessButton btn = new ProcessButton();

            btn.Position = arrow.BeginPosition;

            if (!btn.Data.Arrows.Contains(arrow))
                btn.Data.Arrows.Add(arrow);

            btn.Status = ProcessButton.ButtonStatus.WAIT;

            mgr.Section = arrow.BeginLink;
            mgr.SetButton(arrow.BeginPosition, btn);

            return btn;
        }

        private bool LoadPanelComponent(WebDBManager dbMgr, Sections.PanelSectionEx panel, StepMemberData data, ArrayList arrLink, ArrayList arrSections, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined, ref Dictionary<int, Sections.ExternalTeamData> dicExternal, ref Dictionary<int, string> dicRegular)
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

            if (!LoadProcess(dbMgr, dicProcessSections, arrSections, panel, data, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular))
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
            if (!LoadInternal(dbMgr, dicInternalSections, arrSections, panel, data))
                return false;
            if (!LoadExternal(dbMgr, dicExternalSections, arrSections, panel, data, dicExternal))
                return false;
            if (!LoadTransmission(dbMgr, dicTransmissionSections, arrSections, panel, data, dicExternal))
                return false;

            if (!LoadArrow(dbMgr, dicProcessSections, dicDecisionSections, dicAnnotationSections, dicEndPointSections, dicLinkSections, dicTransSOPSections, dicInternalSections, dicExternalSections, dicTransmissionSections, data))
                return false;

            return true;
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
                Sections.SectionTabPage page = (Sections.SectionTabPage)arrTabPages[i];

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

            string strSQL = string.Format("Select ID, StepName, PeriodType, BeginTime, EndTime, WeekDayOption, Iteration, IterationType, ProcessTime, ProcessTimeType, ParentStepID from ActionStep where ID in ({0})", strIDs);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 1);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            if (nResultCount == 0)
                return null;

            DateTime dtDefault = new DateTime();
            ArrayList arrStepDatas = new ArrayList();

            for (int i = 0; i < nResultCount - 10; i += 11)
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

                arrStepDatas.Add(data);
            }

            return arrStepDatas;
        }

        private bool LoadPane(WebDBManager dbMgr, PageBackstageHome pageHome, ArrayList arrActionSteps, ArrayList arrTeams)
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
                        pageHome.GetDockPropertiesLevel().GetLevelProperties(pageCurrent);
                    }
                }

                if (!dicStepMembers.ContainsKey(actionStep.ActionStepID))
                    continue;

                ArrayList arrStepMemberData = dicStepMembers[actionStep.ActionStepID];

                TabPage tabPage = GetTabPage(actionStep.ActionStepID, pageHome.GetTabPage());
                if (tabPage == null)
                    continue;

                //pageLevel.AddTabPage(actionStep);
                Sections.SectionTabPage page = (Sections.SectionTabPage)tabPage;

                if (page.CreateNew == true)
                {
                    ArrayList arrPanels = pageHome.AddPane(arrTeams, actionStep.ActionStepID, tabPage);

                    if (!LoadNewPanelComponent(dbMgr, arrPanels, arrStepMemberData, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular))
                        return false;
                    /*// Link 객체는 다른 Panel의 객체와 연결되어야 하므로, DB로부터 모든 객체를 읽어들인 후에 해당 객체와 연결시킨다.
                    // Link 객체는 같은 Step내의 객체들과만 연결된다.
                    // arrSections는 Step내의 모든 Section 객체를 담게 되는데, Link 객체와 연결하기 위해서다.
                    ArrayList arrLink = new ArrayList();
                    ArrayList arrSections = new ArrayList();

                    foreach (Sections.PanelSectionEx panel in arrPanels)
                    {
                        StepMemberDataEx data = FindStepMemberDataEx(panel, arrStepMemberDataEx);
                        if (data == null)
                            continue;

                        if (!LoadPanelComponent(dbMgr, panel, data, arrLink, arrSections, ref dicNormal, ref dicEmergency, ref dicUserDefined, dicExternal))
                            return false;
                    }
                    if (!SetLinkSections(arrLink, arrSections))
                        return false;*/
                }
                else
                {
                    foreach (Control control in  page.Controls)
                    {
                        if (control.GetType() == typeof(Sections.PanelSectionEx))
                        {
                            FormMain.Instance.GetPageHome().PanelArray.Add(control);
                        }
                    }
                   
                    
                }
            }

            return true;
        }

        public bool LoadNewPanelComponent(WebDBManager dbMgr, ArrayList arrPanels, ArrayList arrStepMemberData, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined, ref Dictionary<int, Sections.ExternalTeamData> dicExternal, ref Dictionary<int, string> dicRegular)
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
                
                if (!LoadPanelComponent(dbMgr, panel, data, arrLink, arrSections, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular))
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

        private ArrayList LoadBarPage(PageBackstageHome pageHome, ArrayList arrActionSteps, WebDBManager dbMgr)
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

            //pageHome.GetDockScenario().GetBarPage().SetDataGrid(arrTeamNames);
            return arrTeams;
        }

        private void ClearSOP(FormMain frm)
        {
            //PageBackstageHome pageHome = frm.GetPageHome();
        }
    }

    public struct StepMemberData
    {
        private string m_strTeamName;
        private int m_nTeamID;
        private int m_nTeamType;
        private int m_nStepMemberID;
        private int m_nLevelNo;
        
        //public StepMemberData()
        //{
        //    m_nTeamID = -1;
        //    m_strTeamName = "";
        //    m_nTeamType = -1;
        //    m_nStepMemberID = -1;
        //}

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

    /*public class StepMemberDataEx
    {
        private int m_nTeamID = -1;
        private string m_strTeamName = "";
        private int m_nTeamType = -1;
        private int m_nStepMemberID = -1;

        public StepMemberDataEx(int nTeamID, string strTeamName, int nTeamType, int nStepMemberID)
        {
            m_nTeamID = nTeamID;
            m_strTeamName = strTeamName;
            m_nTeamType = nTeamType;
            m_nStepMemberID = nStepMemberID;
        }

        public int TeamID
        {
            get { return m_nTeamID; }
            set { m_nTeamID = value; }
        }

        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
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
    }*/
}
