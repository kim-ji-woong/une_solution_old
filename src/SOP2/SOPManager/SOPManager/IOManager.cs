using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using DBUtility;

namespace SOPManager
{
    // SOP Data를 DB에 저장 및 불러오기 담당
    public class IOManager
    {
        // 같은 버전으로 덮어쓰기 할 경우 저장에 앞서 먼저 버전을 삭제한다.
        // 이때 삭제된 ActionStep 정보를 기억시키기 위하여 TabPage별 삭제된 ActionStep 정보를 기억시킨다.
        private Dictionary<int, TabPage> m_dicDeletedActionStep = new Dictionary<int, TabPage>();

        public IOManager()
        {
        }

        public bool Load(FormMain frm, WebDBManager dbMgr, VersionInfo version, ArrayList arrActionSteps, string strCategoryName, string strSubCategoryName, string strDisasterName)
        {
            //for (int i = 1; i <= 60; i++)
            //{
            //    if (!DeleteSOPVersion(dbMgr, i, true, false))
            //    {
            //        int aa = 3; 
            //    }
            //}

            //return true;
            
            ClearSOP(frm);

			FormPageSOP pageLevel = frm.GetPageLevel();

            string strFullPath = LoadDisasterTree(frm, strCategoryName, strSubCategoryName, strDisasterName, arrActionSteps);
            pageLevel.GetPropertiesLevel().AddTitle(strFullPath);

            ArrayList arrTeams = LoadBarPage(pageLevel, arrActionSteps, dbMgr);
            if (arrTeams == null)
                return false;

            if (!LoadPane(dbMgr, pageLevel, arrActionSteps, arrTeams))
                return false;

            //m_pageLevel.GetBarLevelTree().AddTreeNode();
            //m_pageLevel.GetPropertiesLevel().AddTitle(strValue);
            //m_pageLevel.GetBarPage().SetDataGrid();
            //m_pageLevel.AddPane();
            //EnableControlDisaster(false);

            return true;
        }

        // dicTeamName : TeamID, TeamName
        public static bool ReadTeamList(WebDBManager dbMgr, string strTableName, Dictionary<int, string> dicTeamName)
        {
            string strSQL = "select id, TeamName from " + strTableName;
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                int nTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
                dicTeamName[nTeamID] = strTeamName;
            }

            return true;
        }

        public static Dictionary<int, Sections.ExternalTeamData> ReadExternalTeamList(WebDBManager dbMgr)
        {
            string strSQL = "select id, TeamName, PhoneNumber, FaxNumber from ExternalTeam";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            Dictionary<int, Sections.ExternalTeamData> dicExternal = new Dictionary<int,Sections.ExternalTeamData>();

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-3;i+=4)
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

                dicExternal[nTeamID] = data;
            }

            return dicExternal;
        }

        public static bool GetTeamName(WebDBManager dbMgr, ref Sections.SectionDataProcess sectionData, ref string strTeamNameList, string strTeamList, int nBeginIndex, int nEndIndex, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined, ref Dictionary<int, Sections.ExternalTeamData> dicExternal, ref Dictionary<int, string> dicRegular)
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

            Dictionary<int, string> dicTeamName = null;
            string strTeamName = null;

            if (strTeamType == "0")
            {
                if (dicNormal == null)
                {
                    dicNormal = new Dictionary<int, string>();
                    ReadTeamList(dbMgr, "TemporaryNormalTeam", dicNormal);
                }

                dicTeamName = dicNormal;
            }
            else if (strTeamType == "1")
            {
                if (dicEmergency == null)
                {
                    dicEmergency = new Dictionary<int, string>();
                    ReadTeamList(dbMgr, "TemporaryEmergencyTeam", dicEmergency);
                }

                dicTeamName = dicEmergency;
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

                Sections.SOPTeam team = new Sections.SOPTeam();

                team.TeamID = nTeamID;
                team.TeamType = int.Parse(strTeamType);
                team.TeamName = strTeamName;

                sectionData.TeamList.Add(team);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        // TeamID(TeamType), ... 형태로 되어 있는 strTeamList를 분석하여 Team 이름들을 얻어온다.
        // ex) 1(0), 1(2), 2(3), 5(0)
        public static string GetTeamList(WebDBManager dbMgr, string strTeamList, ref Sections.SectionDataProcess sectionData, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined, ref Dictionary<int, Sections.ExternalTeamData> dicExternal, ref Dictionary<int, string> dicRegular)
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
                int nTransmissionType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                string strTarget = WebDBManager.GetStringField(arrResult[i + 3], "");
                Sections.MissionItem item = new Sections.MissionItem();

                item.TransmissionType = nTransmissionType;
                item.Mission = strMissionText;
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

        private bool LoadProcess(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberDataEx data, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined, ref Dictionary<int, Sections.ExternalTeamData> dicExternal, ref Dictionary<int, string> dicRegular)
        {
            string strSQL = "select id, x, y, width, height, text, TeamList, ComponentID, ProcessTime, ProcessTimeType, useProcessTime, useMissionMessage";
            strSQL += ", onlyTeamLeader from Process where StepMemberID = " + data.StepMemberID.ToString();

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;
            
            for (int i = 0; i < nResultCount - 12; i+=13)
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

        private bool LoadDecision(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberDataEx data)
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

                section.RectSize = new SizeF(fWidth, fHeight);
                section.Title = strText;

                sectionData.ID = nID;
                sectionData.Title = strText;
                sectionData.ComponentID = strComponentID;
            }

            return true;
        }

        private bool LoadAnnotation(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberDataEx data)
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

                section.RectSize = new SizeF(fWidth, fHeight);
                section.Title = strText;

                sectionData.ID = nID;
                sectionData.Title = strText;
                sectionData.ComponentID = strComponentID;
            }

            return true;
        }

        private bool LoadEndPoint(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberDataEx data)
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

                section.RectSize = new SizeF(fWidth, fHeight);
                section.Title = strText;

                sectionData.ID = nID;
                sectionData.Title = strText;
                sectionData.ComponentID = strComponentID;
                sectionData.IsBegin = isBegin;
            }

            return true;
        }

        // arrLink : Link 객체는 다른 Panel의 객체와 연결되어야 하므로, DB로부터 모든 객체를 읽어들인 후에 해당 객체와 연결시킨다.
        private bool LoadLink(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, ArrayList arrLink, Sections.PanelSectionEx panel, StepMemberDataEx data)
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

                section.RectSize = new SizeF(fWidth, fHeight);
                section.Title = strText;

                sectionData.ID = nID;
                // sectionData의 Title은 strText이지만 링크된 Section 객체의 이름을 기억해 놓기 위하여 임시로 strLinkedComponentID를 집어넣는다.
                sectionData.Title = strLinkedComponentID;
                sectionData.ComponentID = strComponentID;
            }

            return true;
        }

        private bool LoadTransSOP(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberDataEx data)
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

        private bool LoadInternal(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberDataEx data)
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
                {
                    szMessage = "";
                }

                Sections.SectionInternal section = new Sections.SectionInternal(panel, x, y);
                Sections.SectionDataInternal sectionData = (Sections.SectionDataInternal)section.Data;
                dicSections[nID] = section;
                arrSections.Add(section);
                panel.Sections.Add(section);

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

        public static bool GetExternalTeam(string strTeamList, ArrayList arrExternalTeamList, Dictionary<int, Sections.ExternalTeamData> dicExternal, int nBeginIndex, int nEndIndex)
        {
            if (strTeamList.Length == 0)
                return true;

			if (strTeamList == "null")
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
        public static bool GetExternalTeamList(string strTeamList, ArrayList arrExternalTeamList, Dictionary<int, Sections.ExternalTeamData> dicExternal)
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

        private bool LoadExternal(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberDataEx data, Dictionary<int, Sections.ExternalTeamData> dicExternal)
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

        private bool LoadTransmission(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberDataEx data, Dictionary<int, Sections.ExternalTeamData> dicExternal)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID, useInternalPopupMessage, useInternalMobileApp, useInternalBroadcast, "
                + "useExternalSMS, externalSMSText, SMSExternalTeamIDList, useExternalFax, FaxExternalTeamIDList, InternalBroadcastMessage from Transmission where StepMemberID = " + data.StepMemberID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

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
				string strComponentID = WebDBManager.GetStringField(arrResult[i + 6], "");

				bool useInternalPopupMessage = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0) == 0 ? false : true;
				bool useInternalMobileApp = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 0) == 0 ? false : true;
				bool useInternalBroadcast = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 0) == 0 ? false : true;

				bool useExternalSMS = WebDBManager.GetIntField(arrResult[i + 10].ToString(), 0) == 0 ? false : true;
				string strExternalSMSText = WebDBManager.GetStringField(arrResult[i + 11], "");
				string strSMSExternalTeamIDList = WebDBManager.GetStringField(arrResult[i + 12], "");
				bool useExternalFax = WebDBManager.GetIntField(arrResult[i + 13].ToString(), 0) == 0 ? false : true;
				string strFaxExternalTeamIDList = WebDBManager.GetStringField(arrResult[i + 14], "");

				string strMessage = WebDBManager.GetStringField(arrResult[i + 15], "");
                if (strMessage == null || strMessage.Equals("null"))
                {
                    strMessage = "";
                }

                Sections.SectionTransmission section = new Sections.SectionTransmission(panel, x, y);
                Sections.SectionDataTransmission sectionData = (Sections.SectionDataTransmission)section.Data;
                dicSections[nID] = section;
                arrSections.Add(section);
                panel.Sections.Add(section);

                section.RectSize = new SizeF(fWidth, fHeight);
                section.Title = strText;

                sectionData.ID = nID;
                sectionData.Title = strText;
                sectionData.ComponentID = strComponentID;

                sectionData.DataInternal.UsePopupMessage = useInternalPopupMessage;
                sectionData.DataInternal.UseMobileApp = useInternalMobileApp;
                sectionData.DataInternal.UseBroadcast = useInternalBroadcast;
                sectionData.DataInternal.BroadcastMessage = strMessage;

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

       

		private bool LoadGroup(WebDBManager dbMgr, 
								Dictionary<int, Sections.Section> dicSections, 
								ArrayList arrSections, 
								Sections.PanelSectionEx panel,
								Dictionary<int, Sections.Section> dicProcessSections, 
								Dictionary<int, Sections.Section> dicDecisionSections, 
								Dictionary<int, Sections.Section> dicAnnotationSections,								
								Dictionary<int, Sections.Section> dicEndPointSections,
								Dictionary<int, Sections.Section> dicLinkSections,
								Dictionary<int, Sections.Section> dicTransSOPSections,
								Dictionary<int, Sections.Section> dicInternalSections,
								Dictionary<int, Sections.Section> dicExternalSections,	
								Dictionary<int, Sections.Section> dicTransmissionSections,
								StepMemberDataEx data)
		{
			string szSQL = "SELECT ID, x, y, width, height, text, ComponentID, RegionX, RegionY, RegionWidth, RegionHeight FROM SectionGroup where StepMemberID = " + data.StepMemberID.ToString();
			ArrayList arrResult = dbMgr.GetResultData(szSQL, 0);

			if (arrResult == null)
				return true;

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

				float rx = WebDBManager.GetFloatField(arrResult[i + 7].ToString(), 0.0f);
				float ry = WebDBManager.GetFloatField(arrResult[i + 8].ToString(), 0.0f);
				float rwidth = WebDBManager.GetFloatField(arrResult[i + 9].ToString(), 0.0f);
				float rheight = WebDBManager.GetFloatField(arrResult[i + 10].ToString(), 0.0f);

				Sections.SectionGroup section = new Sections.SectionGroup(panel, x, y);
				Sections.SectionDataGroup sectionData = (Sections.SectionDataGroup)section.Data;
				dicSections[nID] = section;
				

				section.RectSize = new SizeF(fWidth, fHeight);
				section.Title = strText;
				RectangleF rect = new RectangleF();
				rect.Location = new PointF(rx, ry);
				rect.Size = new SizeF(rwidth, rheight);
				section.GroupRegion = rect;
				section.UpdateGroupRegion();	

				sectionData.ID = nID;
				sectionData.Title = strText;
				sectionData.ComponentID = strComponentID;
				

				string szSql2 = "SELECT CID, type, ComponentID FROM GroupComponent WHERE GroupID = " + nID.ToString();
				ArrayList arrResultComp = dbMgr.GetResultData(szSql2, 0);

				if (arrResultComp == null)
					return false;

				int nResultCountComp = arrResultComp.Count;
				for (int j = 0; j < nResultCountComp - 2; j += 3)
				{
					int nCompID = WebDBManager.GetIntField(arrResultComp[j].ToString(), 0);
					int nCompType = WebDBManager.GetIntField(arrResultComp[j + 1].ToString(), 0);
					string szCompID = WebDBManager.GetStringField(arrResultComp[j + 2], "");

					Dictionary<int, Sections.Section> dicCompSection = GetSectionDictionary(nCompType, dicProcessSections, dicDecisionSections, dicAnnotationSections, dicEndPointSections, dicLinkSections, dicTransSOPSections, dicInternalSections, dicExternalSections, dicTransmissionSections, dicSections);

					Sections.Section sectionComp = dicCompSection[nCompID];
					if (sectionComp != null)
					{
						sectionData.AddGroupMember(sectionComp);
					}
				}

				arrSections.Add(section);
				panel.Sections.Add(section);

			}
			return true;
		}

		private Dictionary<int, Sections.Section> GetSectionDictionary(int nSectionType, Dictionary<int, Sections.Section> dicProcessSections, Dictionary<int, Sections.Section> dicDecisionSections, Dictionary<int, Sections.Section> dicAnnotationSections, Dictionary<int, Sections.Section> dicEndPointSections, Dictionary<int, Sections.Section> dicLinkSections, Dictionary<int, Sections.Section> dicTransSOPSections, Dictionary<int, Sections.Section> dicInternalSections, Dictionary<int, Sections.Section> dicExternalSections, Dictionary<int, Sections.Section> dicTransmissionSections, Dictionary<int, Sections.Section> dicGroupSections)
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

				case (int)Sections.Section.ComponentType.GROUP:
					return dicGroupSections;
			}

			return null;
		}

		private bool LoadArrow(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicProcessSections, Dictionary<int, Sections.Section> dicDecisionSections, Dictionary<int, Sections.Section> dicAnnotationSections, Dictionary<int, Sections.Section> dicEndPointSections, Dictionary<int, Sections.Section> dicLinkSections, Dictionary<int, Sections.Section> dicTransSOPSections, Dictionary<int, Sections.Section> dicInternalSections, Dictionary<int, Sections.Section> dicExternalSections, Dictionary<int, Sections.Section> dicTransmissionSections, Dictionary<int, Sections.Section> dicGroupSections, StepMemberDataEx data)
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
                Dictionary<int, Sections.Section> dicBeginSection = GetSectionDictionary(nBeginType, dicProcessSections, dicDecisionSections, dicAnnotationSections, dicEndPointSections, dicLinkSections, dicTransSOPSections, dicInternalSections, dicExternalSections, dicTransmissionSections, dicGroupSections);

                // nBeginType, 즉 nBeginComponentID가 잘못 입력된 경우
                if (dicBeginSection == null)
                    return false;

                int nEndType = nEndComponentID >> 24;
                nEndComponentID = nEndComponentID & 0xffffff;
				Dictionary<int, Sections.Section> dicEndSection = GetSectionDictionary(nEndType, dicProcessSections, dicDecisionSections, dicAnnotationSections, dicEndPointSections, dicLinkSections, dicTransSOPSections, dicInternalSections, dicExternalSections, dicTransmissionSections, dicGroupSections);

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
            }

            return true;
        }

        private bool LoadPanelComponent(WebDBManager dbMgr, Sections.PanelSectionEx panel, StepMemberDataEx data, ArrayList arrLink, ArrayList arrSections, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined, ref Dictionary<int, Sections.ExternalTeamData> dicExternal, ref Dictionary<int, string> dicRegular)
        {
            // 화살표 연결을 위하여 Section 정보를 임시 저장
            // ComponentID, Section
            Dictionary<int, Sections.Section> dicProcessSections = new Dictionary<int,Sections.Section>();
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

			Dictionary<int, Sections.Section> dicGroupSections = new Dictionary<int, Sections.Section>();

			if (!LoadGroup(dbMgr, dicGroupSections, arrSections, panel, dicProcessSections, dicDecisionSections, dicAnnotationSections, dicEndPointSections, dicLinkSections, dicTransSOPSections, dicInternalSections, dicExternalSections, dicTransmissionSections, data))
                return false;


			if (!LoadArrow(dbMgr, dicProcessSections, dicDecisionSections, dicAnnotationSections, dicEndPointSections, dicLinkSections, dicTransSOPSections, dicInternalSections, dicExternalSections, dicTransmissionSections, dicGroupSections, data))
                return false;
            
            return true;
        }

        // Return 값 : ActionStepID, StepMemberDataEx List
        private Dictionary<int, ArrayList> LoadStepMemberDataEx(WebDBManager dbMgr, ArrayList arrActionSteps, ArrayList arrTeams)
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

                StepMemberDataEx data = new StepMemberDataEx(nTeamID, nTeamType, nStepMemberID);

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

        private StepMemberDataEx FindStepMemberDataEx(Sections.PanelSectionEx panel, ArrayList arrStepMemberDataEx)
        {
            foreach (StepMemberDataEx data in arrStepMemberDataEx)
            {
                if (data.TeamID == panel.TeamID && data.TeamType == panel.TeamType)
                    return data;
            }

            return null;
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

        private TabPage GetTabPage(string strTabPageName, ArrayList arrTabPages)
        {
            int nPageCount = arrTabPages.Count;

            for (int i=nPageCount - 1;i>=0;i--)
            {
                TabPage page = (TabPage)arrTabPages[i];

                if (page.Text == strTabPageName)
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

		private bool LoadPane(WebDBManager dbMgr, FormPageSOP pageLevel, ArrayList arrActionSteps, ArrayList arrTeams)
        {
            Dictionary<int, ArrayList> dicStepMembers = LoadStepMemberDataEx(dbMgr, arrActionSteps, arrTeams);
            if (dicStepMembers == null)
                return false;

            ArrayList arrStepDatas = LoadActionSteps(dbMgr, arrActionSteps);
            if (arrStepDatas == null)
                return false;

            // ActionStepID, TabPage
            Dictionary<int, TabPage> dicActionStep = new Dictionary<int, TabPage>();

            foreach (Data_ActionStep data in arrStepDatas)
            {
                TabPage page = pageLevel.AddTabPage(data);
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
                        FormMain.Instance.GetPageLevel().GetPropertiesLevel().GetLevelProperties(pageCurrent);
                    }
                }

                if (!dicStepMembers.ContainsKey(actionStep.ActionStepID))
                    continue;

                ArrayList arrStepMemberDataEx = dicStepMembers[actionStep.ActionStepID];

                TabPage tabPage = GetTabPage(actionStep.ActionStepName, pageLevel.GetTabPage());
                if (tabPage == null)
                    continue;

                //pageLevel.AddTabPage(actionStep);
				
                ArrayList arrPanels = pageLevel.AddPane(arrTeams, tabPage);
				pageLevel.AddUsingTeam(arrTeams);

                // Link 객체는 다른 Panel의 객체와 연결되어야 하므로, DB로부터 모든 객체를 읽어들인 후에 해당 객체와 연결시킨다.
                // Link 객체는 같은 Step내의 객체들과만 연결된다.
                // arrSections는 Step내의 모든 Section 객체를 담게 되는데, Link 객체와 연결하기 위해서다.
                ArrayList arrLink = new ArrayList();
                ArrayList arrSections = new ArrayList();

                foreach (Sections.PanelSectionEx panel in arrPanels)
                {
                    StepMemberDataEx data = FindStepMemberDataEx(panel, arrStepMemberDataEx);
                    if (data == null)
                        continue;

                    panel.ActionStepID = actionStep.ActionStepID;

                    if (!LoadPanelComponent(dbMgr, panel, data, arrLink, arrSections, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular))
                        return false;
                }

                if (!SetLinkSections(arrLink, arrSections))
                    return false;
            }

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

                //arrTeamNames.Add(strTeamName);

                int nIndex = FindStepMemberTeamIndex(nTeamID, nTeamType, arrTeams);

                if (nIndex >= 0)
                {
                    StepMemberData data = new StepMemberData(strTeamName, nTeamID, nTeamType);
                    arrTeams[nIndex] = data;
                }
                else
                    return;

                /*for (int j = 0; j < nStepMemberCount; j++)
                {
                    StepMemberData data = (StepMemberData)arrStepMembers[j];

                    if (data.TeamID == nTeamID)
                    {
                        arrStepMembers[j] = new StepMemberData(strTeamName, data.TeamID, data.TeamType);
                        break;
                    }
                }*/
            }
        }

		private ArrayList LoadBarPage(FormPageSOP pageLevel, ArrayList arrActionSteps, WebDBManager dbMgr)
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
                else if (nTeamType == 4)    // 정규 조직
                    arrRegular.Add(data);
            }

            GetStepMemberTeamName(arrNormal, "TemporaryNormalTeam", 0, arrTeams, dbMgr);
            GetStepMemberTeamName(arrEmergency, "TemporaryEmergencyTeam", 1, arrTeams, dbMgr);
            GetStepMemberTeamName(arrExternal, "ExternalTeam", 2, arrTeams, dbMgr);
            GetStepMemberTeamName(arrUserDefined, "UserDefinedTeam", 3, arrTeams, dbMgr);
            GetStepMemberTeamName(arrRegular, "RegularTeam", 4, arrTeams, dbMgr);



			pageLevel.GetBarPage().SetDataGrid(arrTeams);
            return arrTeams;
        }

        // 부모가 있는 단계들...
        private void LoadChildActionStepTree(Dictionary<int, TreeNode> dicTreeNode, ArrayList arrChildSteps)
        {
            while (arrChildSteps.Count > 0)
            {
                ArrayList arrRemove = new ArrayList();

                foreach (ActionStepInfo actionStep in arrChildSteps)
                {
                    if (dicTreeNode.ContainsKey(actionStep.ParentStepID))
                    {
                        TreeNode node = dicTreeNode[actionStep.ParentStepID];
                        node = node.Nodes.Add(actionStep.ActionStepName);
                        dicTreeNode[actionStep.ActionStepID] = node;
                        arrRemove.Add(actionStep);
                    }
                }

                foreach (ActionStepInfo actionStep in arrRemove)
                {
                    arrChildSteps.Remove(actionStep);
                }
            }
        }

        // Return 값 : Tree에서 선택된 단계의 전체 경로
        private string LoadDisasterTree(FormMain frm, string strCategoryName, string strSubCategoryName, string strDisasterName, ArrayList arrActionSteps)
        {
			FormPageSOP pageLevel = frm.GetPageLevel();
            BarLevelTree tree = pageLevel.GetBarLevelTree();

            if (arrActionSteps == null || arrActionSteps.Count == 0)
                return "";

            // ActionStepID, TreeNode
            Dictionary<int, TreeNode> dicTreeNode = new Dictionary<int, TreeNode>();
            ArrayList arrChildSteps = new ArrayList();

            foreach (ActionStepInfo actionStep in arrActionSteps)
            {
                if (actionStep.ParentStepID <= 0)
                {
                    TreeNode node = tree.AddTreeNode(strCategoryName, strSubCategoryName, strDisasterName, actionStep.ActionStepName);
                    dicTreeNode[actionStep.ActionStepID] = node;
                }
                else
                    arrChildSteps.Add(actionStep);
            }

            // 부모가 있는 단계들은 별도로 입력
            LoadChildActionStepTree(dicTreeNode, arrChildSteps);

            TreeNode nodeCategory = tree.FindNode(strCategoryName);
            if (nodeCategory == null)
                return "";

            TreeNode nodeSubCategory = tree.FindNode(strSubCategoryName, nodeCategory.Nodes);
            if (nodeSubCategory == null)
                return "";

            TreeNode nodeDisaster = tree.FindNode(strDisasterName, nodeSubCategory.Nodes);
            if (nodeDisaster == null)
                return "";

            if (nodeDisaster.Nodes.Count > 0)
            {
                tree.SelectNode(nodeDisaster.Nodes[0]);
                string strFullPath = nodeDisaster.Nodes[0].FullPath;
                return strFullPath.Replace('\\', '/');
            }

            return "";
        }

        private void ClearSOP(FormMain frm)
        {
			FormPageSOP pageLevel = frm.GetPageLevel();
            BarLevelTree tree = pageLevel.GetBarLevelTree();

            tree.ClearTree();
        }
        
        // nVersionID : nVersionID가 0보다 크면 기존 버전을 덮어쓴다.
        public bool Save(FormMain frm, WebDBManager dbMgr, string strVersionName, int nVersionID, int nSOPGenUserID, string strDescription, ref VersionInfo rVersion, out int nDisasterID)
        {
            nDisasterID = 0;

            m_dicDeletedActionStep.Clear();
            SaveDeletingActionStepID();
            
            // Batch Job Start - Begin Transaction
            dbMgr.BeginBatch();

            if (nVersionID > 0)
                DeleteSOPVersion(dbMgr, nVersionID, false, true);

            if (nVersionID > 0)
                UpdateVersion(dbMgr, nVersionID, ref rVersion);
            else
                nVersionID = SaveVersion(frm, dbMgr, strVersionName, nVersionID, nSOPGenUserID, strDescription, ref rVersion);

            if (nVersionID < 0)
            {
                // Rollback
                dbMgr.BatchRollback();
                return false;
            }

            nDisasterID = AddDisaster(frm, dbMgr, nVersionID);
            if (nDisasterID < 0)
            {
                // Rollback
				dbMgr.BatchRollback();
                return false;
            }

            Dictionary<System.Windows.Forms.TabPage, int> dicActionStepIDs = AddActionSteps(frm, dbMgr, nDisasterID);
            if (dicActionStepIDs == null)
            {
                // Rollback
				dbMgr.BatchRollback();
                return false;
            }

            Dictionary<System.Windows.Forms.TabPage, Dictionary<StepMemberData, int>> dicStepMemberIDs = AddStepMembers(frm, dbMgr, dicActionStepIDs);
            if (dicStepMemberIDs == null)
            {
                // Rollback
				dbMgr.BatchRollback();
                return false;
            }

            if (!AddComponents(frm, dbMgr, dicStepMemberIDs, dicActionStepIDs))
            {
                // Rollback
				dbMgr.BatchRollback();
                return false;
            }
            // Batch Job end - Commit
			dbMgr.BatchCommit();

            return true;
        }

        private bool AddComponents(FormMain frm, WebDBManager dbMgr, Dictionary<System.Windows.Forms.TabPage, Dictionary<StepMemberData, int>> dicStepMemberIDs, Dictionary<System.Windows.Forms.TabPage, int> dicActionStepIDs)
        {
            Type panelType = typeof(Sections.PanelSectionEx);
            int nProcessID = -1, nDecisionID = -1, nAnnotationID = -1, nEndPointID = -1, nLinkID = -1, nTransSOP = -1, nInternalID = -1, nExternalID = -1, nTransmissionID = -1, nProcessMissionID = -1;
            int nArrowID = -1;
			int nGroupID = -1;
            foreach (KeyValuePair<System.Windows.Forms.TabPage, Dictionary<StepMemberData, int>> pair in dicStepMemberIDs)
            {
                System.Windows.Forms.TabPage page = pair.Key;
                Dictionary<StepMemberData, int> dicStepMember = pair.Value;

                foreach (System.Windows.Forms.Control control in page.Controls)
                {
                    if (control.GetType() == panelType)
                    {
                        Sections.PanelSectionEx panel = (Sections.PanelSectionEx)control;

						panel.CollapseAllGroup();

                        string strTeamName = panel.TeamName;
                        int nTeamID = panel.TeamID;
                        int nTeamType = panel.TeamType;

                        StepMemberData data = new StepMemberData(strTeamName, nTeamID, nTeamType);

                        if (!dicStepMember.ContainsKey(data))
                            continue;

                        int nStepMemberID = dicStepMember[data];
                        Dictionary<Sections.Section, int> dicComponentID = new Dictionary<Sections.Section, int>();

                        // Component 저장
						ArrayList arGroupList = new ArrayList();
                        foreach (Sections.Section section in panel.Sections)
                        {
                            Sections.Section.ComponentType type = section.GetComponentType();

                            if (type == Sections.Section.ComponentType.PROCESS)
                            {
                                if (!AddProcess(dbMgr, nStepMemberID, (Sections.SectionProcess)section, ref nProcessID, ref nProcessMissionID))
                                    return false;
                                else
                                    dicComponentID[section] = nProcessID;
                            }
                            else if (type == Sections.Section.ComponentType.DECISION)
                            {
                                if (!AddDecision(dbMgr, nStepMemberID, (Sections.SectionDecision)section, ref nDecisionID))
                                    return false;
                                else
                                    dicComponentID[section] = nDecisionID;
                            }
                            else if (type == Sections.Section.ComponentType.ANNOTATION)
                            {
                                if (!AddAnnotation(dbMgr, nStepMemberID, (Sections.SectionAnnotation)section, ref nAnnotationID))
                                    return false;
                                else
                                    dicComponentID[section] = nAnnotationID;
                            }
                            else if (type == Sections.Section.ComponentType.ENDPOINT)
                            {
                                if (!AddEndPoint(dbMgr, nStepMemberID, (Sections.SectionEndPoint)section, ref nEndPointID))
                                    return false;
                                else
                                    dicComponentID[section] = nEndPointID;
                            }
                            else if (type == Sections.Section.ComponentType.LINK)
                            {
                                if (!AddLink(dbMgr, nStepMemberID, (Sections.SectionLink)section, ref nLinkID))
                                    return false;
                                else
                                    dicComponentID[section] = nLinkID;
                            }
                            else if (type == Sections.Section.ComponentType.TRANSSOP)
                            {
                                if (!AddTransSOP(dbMgr, nStepMemberID, (Sections.SectionTransSOP)section, dicActionStepIDs, ref nTransSOP))
                                    return false;
                                else
                                    dicComponentID[section] = nTransSOP;
                            }
                            else if (type == Sections.Section.ComponentType.INTERNAL)
                            {
                                if (!AddInternal(dbMgr, nStepMemberID, (Sections.SectionInternal)section, ref nInternalID))
                                    return false;
                                else
                                    dicComponentID[section] = nInternalID;
                            }
                            else if (type == Sections.Section.ComponentType.EXTERNAL)
                            {
                                if (!AddExternal(dbMgr, nStepMemberID, (Sections.SectionExternal)section, ref nExternalID))
                                    return false;
                                else
                                    dicComponentID[section] = nExternalID;
                            }
                            else if (type == Sections.Section.ComponentType.TRANSMISSION)
                            {
                                if (!AddTransmission(dbMgr, nStepMemberID, (Sections.SectionTransmission)section, ref nTransmissionID))
                                    return false;
                                else
                                    dicComponentID[section] = nTransmissionID;
                            }
							else if (type == Sections.Section.ComponentType.GROUP)
							{
								arGroupList.Add(section);								
							}
                        }

						foreach (Sections.SectionGroup group in arGroupList)
						{
							if (!AddGroup(dbMgr, nStepMemberID, (Sections.SectionGroup)group, ref nGroupID))
								return false;
							else
								dicComponentID[group] = nGroupID;

							AddGroupComponent(dbMgr, nStepMemberID, group, nGroupID, dicComponentID);
						}

                        // Component와 연결된 화살표 저장
                        foreach (Sections.Section section in panel.Sections)
                        {
                            if (!AddArrow(dbMgr, nStepMemberID, section, dicComponentID, ref nArrowID))
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        private void GetComponentMaxID(WebDBManager dbMgr, string strComponentTableName, ref int nComponentID, bool transaction)
        {
            if (nComponentID < 0)
            {
                string strSQL = "Select max(id) from " + strComponentTableName;
                ArrayList arrResult = dbMgr.GetResultData(strSQL, transaction ? 1 : 0);

                if (arrResult == null || arrResult.Count == 0)
                    nComponentID = 0;
                else
                    nComponentID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);
            }
        }

        private bool AddArrow(WebDBManager dbMgr, int nStepMemberID, Sections.Section section, Dictionary<Sections.Section, int> dicComponentID, ref int nArrowID)
        {
            GetComponentMaxID(dbMgr, "Arrow", ref nArrowID, true);

            foreach (Sections.Arrow arrow in section.Arrows)
            {
                if (arrow.BeginLink != section || !dicComponentID.ContainsKey(section))
                    continue;

                if (arrow.EndLink == null || !dicComponentID.ContainsKey(arrow.EndLink))
                    continue;

                // nBeginID, nEndID : 화살표와 링크된 Section 정보
                //                    Component + Component Type 정보(처음 1Byte는 Type 정보, 뒤 3Byte는 ComponentID)로 구성
                //                    Type(0 : Process, 1 : Decision, 2 : Annotation, 3 : EndPoint, 4 : Link, 5 : TransSOP, 6 : Internal, 7 : External)
                int nBeginID = dicComponentID[section] | ((int)section.GetComponentType() << 24);
                int nEndID = dicComponentID[arrow.EndLink] | ((int)arrow.EndLink.GetComponentType() << 24);

                string strSQL = string.Format("insert into Arrow (ID, Text, BeginComponentID, BeginComponentPosition, EndComponentID, EndComponentPosition, StepMemberID) values ({0}, '{1}', {2}, {3}, {4}, {5}, {6})",
                    ++nArrowID, ChangeSpecialCharacter(arrow.Text), nBeginID, (int)arrow.BeginPosition, nEndID, (int)arrow.EndPosition, nStepMemberID);

                if (dbMgr.GetResultData(strSQL, 1) == null)
                    return false;
            }

            return true;
        }

        public static string GetProcessTeamList(Sections.SectionProcess section)
        {
            Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;
            string strIDs = "";

            foreach (Sections.SOPTeam team in data.TeamList)
            {
                if (strIDs.Length == 0)
                    strIDs = string.Format("{0}({1})", team.TeamID, team.TeamType);
                else
                    strIDs += string.Format(", {0}({1})", team.TeamID, team.TeamType);
            }

            return strIDs;
        }

        // strText에 따옴표(')가 있을 경우 DB에서 인식할 수 있도록 ('')로 치환시킨다.
        private string ChangeSpecialCharacter(string strText)
        {
            return strText.Replace("'", "''");
        }

        private bool AddProcess(WebDBManager dbMgr, int nStepMemberID, Sections.SectionProcess section, ref int nProcessID, ref int nProcessMissionID)
        {
            GetComponentMaxID(dbMgr, "Process", ref nProcessID, true);

            Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;

            Sections.ProcessingTime.Type type = data.ProcessingTime.ProcessingType;
            int nProcessType = (int)type;

            string strTeamList = GetProcessTeamList(section);

            string strSQL = string.Format("insert into Process (ID, x, y, width, height, text, TeamList, ComponentID, ProcessTime, ProcessTimeType, useProcessTime, useMissionMessage, onlyTeamLeader, StepMemberID) values ({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', '{7}', {8}, {9}, {10}, {11}, {12}, {13})",
                ++nProcessID,
                section.Position.X,
                section.Position.Y,
                section.RectSize.Width,
                section.RectSize.Height,
                ChangeSpecialCharacter(section.TextUP),
                ChangeSpecialCharacter(strTeamList),
                ChangeSpecialCharacter(data.ComponentID),
                data.ProcessingTime.Time, 
                (int)data.ProcessingTime.ProcessingType, 
                data.UseProcessingTime ? 1 : 0, 
                data.MissionTransfer ? 1 : 0,
                data.TransferTeamLeaderOnly ? 1 : 0,
                nStepMemberID);

            if (dbMgr.GetResultData(strSQL, 1) == null)
                return false;

            return AddProcessMission(dbMgr, nProcessID, data, ref nProcessMissionID);
        }

        private bool AddProcessMission(WebDBManager dbMgr, int nProcessID, Sections.SectionDataProcess data, ref int nProcessMissionID)
        {
            GetComponentMaxID(dbMgr, "ProcessMission", ref nProcessMissionID, true);

            foreach (Sections.MissionItem mission in data.MissionItems)
            {
                string strSQL = string.Format("insert into ProcessMission (ID, missionText, ProcessID, TransmissionType, missionTarget) values ({0}, '{1}', {2}, {3}, '{4}')",
                    ++nProcessMissionID, ChangeSpecialCharacter(mission.Mission), nProcessID, mission.TransmissionType, mission.Target);

                if (dbMgr.GetResultData(strSQL, 1) == null)
                    return false;
            }

            return true;
        }
        
        private bool AddCheckItem(WebDBManager dbMgr, int nProcessID, Sections.SectionDataProcess data, ref int nCheckItemID)
        {
            GetComponentMaxID(dbMgr, "CheckTask", ref nCheckItemID, true);

            foreach (Sections.CheckedItem item in data.CheckedItems)
            {
                string strSQL = string.Format("insert into CheckTask (ID, ProcessID, Category, SubCategory, TaskName, TargetCount, Position) values ({0}, {1}, '{2}', '{3}', '{4}', {5}, '{6}')",
                    ++nCheckItemID, nProcessID, ChangeSpecialCharacter(item.Category), ChangeSpecialCharacter(item.SubCategory), ChangeSpecialCharacter(item.Item), item.ItemCount, ChangeSpecialCharacter(item.Location));

                if (dbMgr.GetResultData(strSQL, 1) == null)
                    return false;
            }

            return true;
        }

        private bool AddDecision(WebDBManager dbMgr, int nStepMemberID, Sections.SectionDecision section, ref int nDecisionID)
        {
            GetComponentMaxID(dbMgr, "Decision", ref nDecisionID, true);
            Sections.SectionDataDecision data = (Sections.SectionDataDecision)section.Data;

            string strSQL = string.Format("insert into Decision (ID, x, y, width, height, text, ComponentID, StepMemberID) values ({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', {7})",
                ++nDecisionID, section.Position.X, section.Position.Y, section.RectSize.Width, section.RectSize.Height, ChangeSpecialCharacter(section.Title), ChangeSpecialCharacter(data.ComponentID), nStepMemberID);

            return dbMgr.GetResultData(strSQL, 1) == null ? false : true;
        }

        private bool AddAnnotation(WebDBManager dbMgr, int nStepMemberID, Sections.SectionAnnotation section, ref int nAnnotationID)
        {
            GetComponentMaxID(dbMgr, "Annotation", ref nAnnotationID, true);
            Sections.SectionDataAnnotation data = (Sections.SectionDataAnnotation)section.Data;

            string strSQL = string.Format("insert into Annotation (ID, x, y, width, height, text, ComponentID, StepMemberID) values ({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', {7})",
                ++nAnnotationID, section.Position.X, section.Position.Y, section.RectSize.Width, section.RectSize.Height, ChangeSpecialCharacter(section.Title), ChangeSpecialCharacter(data.ComponentID), nStepMemberID);

            return dbMgr.GetResultData(strSQL, 1) == null ? false : true;
        }

        private bool AddEndPoint(WebDBManager dbMgr, int nStepMemberID, Sections.SectionEndPoint section, ref int nEndPointID)
        {
            GetComponentMaxID(dbMgr, "EndPoint", ref nEndPointID, true);
            Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)section.Data;

            string strSQL = string.Format("insert into EndPoint (ID, x, y, width, height, text, ComponentID, isBegin, StepMemberID) values ({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', {7}, {8})",
                ++nEndPointID, section.Position.X, section.Position.Y, section.RectSize.Width, section.RectSize.Height, ChangeSpecialCharacter(section.Title), ChangeSpecialCharacter(data.ComponentID), data.IsBegin ? 1 : 0, nStepMemberID);

            return dbMgr.GetResultData(strSQL, 1) == null ? false : true;
        }

        private bool AddLink(WebDBManager dbMgr, int nStepMemberID, Sections.SectionLink section, ref int nLinkID)
        {
            GetComponentMaxID(dbMgr, "Link", ref nLinkID, true);
            Sections.SectionDataLink data = (Sections.SectionDataLink)section.Data;

            string strSQL = string.Format("insert into Link (ID, x, y, width, height, text, ComponentID, LinkedComponentID, StepMemberID) values ({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', '{7}', {8})",
                ++nLinkID, section.Position.X, section.Position.Y, section.RectSize.Width, section.RectSize.Height, ChangeSpecialCharacter(section.Title), ChangeSpecialCharacter(data.ComponentID), ChangeSpecialCharacter(data.LinkedSection.Data.ComponentID), nStepMemberID);

            return dbMgr.GetResultData(strSQL, 1) == null ? false : true;
        }

        // TransSOP의 Data는 이미 삭제된 ActionStep ID를 갖고 있을수 있기 때문에 이를 보정해준다.
        private void ChangeTransSOPLinkedActionStep(Sections.SectionDataTransSOP data, Dictionary<System.Windows.Forms.TabPage, int> dicActionStepIDs)
        {
            if (data.LinkedActionStepID < 0)
                return;

            // 바뀌기 이전의 ActionStep ID : m_dicDeletedActionStep
            if (m_dicDeletedActionStep.ContainsKey(data.LinkedActionStepID))
            {
                TabPage page = m_dicDeletedActionStep[data.LinkedActionStepID];

                // 바뀐 이후의 ActionStep ID : dicActionStepIDs
                if (dicActionStepIDs.ContainsKey(page))
                {
                    data.LinkedActionStepID = dicActionStepIDs[page];
                }
            }
        }

        private bool AddTransSOP(WebDBManager dbMgr, int nStepMemberID, Sections.SectionTransSOP section, Dictionary<System.Windows.Forms.TabPage, int> dicActionStepIDs, ref int nTransSOPID)
        {
            GetComponentMaxID(dbMgr, "TransSOP", ref nTransSOPID, true);
            Sections.SectionDataTransSOP data = (Sections.SectionDataTransSOP)section.Data;

            // m_dicDeletedActionStep를 이용하여 Data의 LinkedActionStep 변환
            ChangeTransSOPLinkedActionStep(data, dicActionStepIDs);

            string strSQL = string.Format("insert into TransSOP (ID, x, y, width, height, text, ComponentID, StepMemberID, LinkedActionStepID, Description) values ({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', {7}, {8}, '{9}')",
                ++nTransSOPID, section.Position.X, section.Position.Y, section.RectSize.Width, section.RectSize.Height, ChangeSpecialCharacter(section.Title), ChangeSpecialCharacter(data.ComponentID), nStepMemberID, data.LinkedActionStepID, ChangeSpecialCharacter(data.Description));

            return dbMgr.GetResultData(strSQL, 1) == null ? false : true;
        }

        private bool AddInternal(WebDBManager dbMgr, int nStepMemberID, Sections.SectionInternal section, ref int nInternalID)
        {
            GetComponentMaxID(dbMgr, "InternalTransmission", ref nInternalID, true);
            Sections.SectionDataInternal data = (Sections.SectionDataInternal)section.Data;

            string strSQL = string.Format("insert into InternalTransmission (ID, x, y, width, height, text, ComponentID, usePopupMessage, useMobileApp, useBroadcast, StepMemberID, BroadcastMessage ) values ({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', {7}, {8}, {9}, {10}, '{11}')",
                ++nInternalID, section.Position.X, section.Position.Y, section.RectSize.Width, section.RectSize.Height, ChangeSpecialCharacter(section.Title), ChangeSpecialCharacter(data.ComponentID),
                data.UsePopupMessage ? 1 : 0, data.UseMobileApp ? 1 : 0, data.UseBroadcast ? 1 : 0, nStepMemberID, data.BroadcastMessage);

            return dbMgr.GetResultData(strSQL, 1) == null ? false : true;
        }

        private void GetSMSData(Sections.SectionDataExternal data, ref string strSMSText, ref string strSMSReceiver)
        {
            if (data.UseSMS)
            {
                strSMSText = "'" + data.SMSMessage + "'";
                strSMSReceiver = GetExternalTeamString(data.SMSReceivers);

                /*foreach (Sections.ExternalTeamData teamData in data.SMSReceivers)
                {
                    if (strSMSReceiver.Length == 0)
                        strSMSReceiver = "'" + teamData.TeamID.ToString();
                    else
                        strSMSReceiver += ", " + teamData.TeamID.ToString();
                }*/

                if (strSMSReceiver.Length == 0)
                    strSMSReceiver = "''";
                else
                    strSMSReceiver += "'";
            }
        }

        private void GetSMSData(Sections.SectionDataTransmission data, ref string strSMSText, ref string strSMSReceiver)
        {
            if (data.DataExternal.UseSMS)
            {
                strSMSText = "'" + data.DataExternal.SMSMessage + "'";
                strSMSReceiver = GetExternalTeamString(data.DataExternal.SMSReceivers);

                if (strSMSReceiver.Length == 0)
                    strSMSReceiver = "''";
                else
                    strSMSReceiver += "'";
            }
        }

        public static string GetExternalTeamString(ArrayList arrReceivers)
        {
            string strReceiver = "";

            foreach (Sections.ExternalTeamData teamData in arrReceivers)
            {
                if (strReceiver.Length == 0)
                    strReceiver = "'" + teamData.TeamID.ToString();
                else
                    strReceiver += ", " + teamData.TeamID.ToString();
            }

            return strReceiver;
        }

        private void GetFaxData(Sections.SectionDataExternal data, ref string strFaxReceiver)
        {
            if (data.UseFax)
            {
                strFaxReceiver = GetExternalTeamString(data.FaxReceivers);

                /*foreach (Sections.ExternalTeamData teamData in data.FaxReceivers)
                {
                    if (strFaxReceiver.Length == 0)
                        strFaxReceiver = "'" + teamData.TeamID.ToString();
                    else
                        strFaxReceiver += ", " + teamData.TeamID.ToString();
                }*/

                if (strFaxReceiver.Length == 0)
                    strFaxReceiver = "''";
                else
                    strFaxReceiver += "'";
            }
        }

        private void GetFaxData(Sections.SectionDataTransmission data, ref string strFaxReceiver)
        {
            if (data.DataExternal.UseFax)
            {
                strFaxReceiver = GetExternalTeamString(data.DataExternal.FaxReceivers);

                if (strFaxReceiver.Length == 0)
                    strFaxReceiver = "''";
                else
                    strFaxReceiver += "'";
            }
        }

        private bool AddExternal(WebDBManager dbMgr, int nStepMemberID, Sections.SectionExternal section, ref int nExternalID)
        {
            GetComponentMaxID(dbMgr, "ExternalTransmission", ref nExternalID, true);
            Sections.SectionDataExternal data = (Sections.SectionDataExternal)section.Data;

            string strSMSText = "''", strSMSReceivers = "''", strFaxReceivers = "''";
            GetSMSData(data, ref strSMSText, ref strSMSReceivers);
            GetFaxData(data, ref strFaxReceivers);

            string strChangeSMSText = "'" + ChangeSpecialCharacter(strSMSText.Substring(1, strSMSText.Length - 2)) + "'";
            string strChangeSMSReceivers = "'" + ChangeSpecialCharacter(strSMSReceivers.Substring(1, strSMSReceivers.Length - 2)) + "'";
            string strChangeFaxReceivers = "'" + ChangeSpecialCharacter(strFaxReceivers.Substring(1, strFaxReceivers.Length - 2)) + "'";

            string strSQL = string.Format("insert into ExternalTransmission (ID, x, y, width, height, text, ComponentID, useSMS, SMSText, SMSExternalTeamIDList, useEFax, FaxExternalTeamIDList, StepMemberID) values ({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', {7}, {8}, {9}, {10}, {11}, {12})",
                ++nExternalID, section.Position.X, section.Position.Y, section.RectSize.Width, section.RectSize.Height, ChangeSpecialCharacter(section.Title), ChangeSpecialCharacter(data.ComponentID),
                data.UseSMS ? 1 : 0, strChangeSMSText, strChangeSMSReceivers, data.UseFax ? 1 : 0, strChangeFaxReceivers, nStepMemberID);

            return dbMgr.GetResultData(strSQL, 1) == null ? false : true;
        }

        private bool AddTransmission(WebDBManager dbMgr, int nStepMemberID, Sections.SectionTransmission section, ref int nTransmissionID)
        {
            GetComponentMaxID(dbMgr, "Transmission", ref nTransmissionID, true);
            Sections.SectionDataTransmission data = (Sections.SectionDataTransmission)section.Data;

            string strSMSText = "''", strSMSReceivers = "''", strFaxReceivers = "''";
            GetSMSData(data, ref strSMSText, ref strSMSReceivers);
            GetFaxData(data, ref strFaxReceivers);

            string strChangeSMSText = "'" + ChangeSpecialCharacter(strSMSText.Substring(1, strSMSText.Length - 2)) + "'";
            string strChangeSMSReceivers = "'" + ChangeSpecialCharacter(strSMSReceivers.Substring(1, strSMSReceivers.Length - 2)) + "'";
            string strChangeFaxReceivers = "'" + ChangeSpecialCharacter(strFaxReceivers.Substring(1, strFaxReceivers.Length - 2)) + "'";

            string strBroadcastMessage = "'" + ChangeSpecialCharacter(data.DataInternal.BroadcastMessage) + "'";

            string strSQL = string.Format("insert into Transmission (ID, x, y, width, height, text, ComponentID, useInternalPopupMessage, useInternalMobileApp, useInternalBroadcast, "
                + "useExternalSMS, externalSMSText, SMSExternalTeamIDList, useExternalFax, FaxExternalTeamIDList, StepMemberID, InternalBroadcastMessage) values ({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', {7}, {8}, {9}, "
                + "{10}, {11}, {12}, {13}, {14}, {15}, {16})",
                ++nTransmissionID, section.Position.X, section.Position.Y, section.RectSize.Width, section.RectSize.Height, ChangeSpecialCharacter(section.Title), ChangeSpecialCharacter(data.ComponentID),
                data.DataInternal.UsePopupMessage ? 1 : 0, data.DataInternal.UseMobileApp ? 1 : 0, data.DataInternal.UseBroadcast ? 1 : 0,
                data.DataExternal.UseSMS ? 1 : 0, strChangeSMSText, strChangeSMSReceivers, data.DataExternal.UseFax ? 1 : 0, strChangeFaxReceivers, nStepMemberID, strBroadcastMessage);

            return dbMgr.GetResultData(strSQL, 1) == null ? false : true;
        }

		private bool AddGroup(WebDBManager dbMgr, int nStepMemberID, Sections.SectionGroup section, ref int nGroupID)
		{
			GetComponentMaxID(dbMgr, "SectionGroup", ref nGroupID, true);
			Sections.SectionDataGroup data = (Sections.SectionDataGroup)section.Data;
			
			
			StringBuilder sb = new StringBuilder();
			
			float x = section.Position.X;
			float y = section.Position.Y;
			float width = section.RectSize.Width;
			float height = section.RectSize.Height;
			float rx = section.GroupRegion.Location.X;
			float ry = section.GroupRegion.Location.Y;
			float rwidth = section.RectSize.Width;
			float rHeight = section.RectSize.Height;

			sb.AppendFormat("insert into SectionGroup (ID, x, y, width, height, text, ComponentID, StepMemberID, RegionX, RegionY, RegionWidth, RegionHeight) values ({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', {7}, {8}, {9}, {10}, {11})",
			++nGroupID, x, y, width, height, ChangeSpecialCharacter(section.Title), ChangeSpecialCharacter(data.ComponentID), nStepMemberID,  rx, ry, rwidth, rHeight);
			
			string strSQL = sb.ToString();
			return dbMgr.GetResultData(strSQL, 1) == null ? false : true;
		}

		private bool AddGroupComponent(WebDBManager dbMgr, int nStepMemberID, Sections.SectionGroup group, int nGroupID, Dictionary<Sections.Section, int> dicComponentID)
		{
			Sections.SectionDataGroup data = (Sections.SectionDataGroup)group.Data;
			StringBuilder sb = new StringBuilder();
			foreach (Sections.Section section in data.GroupItems)
			{
				int nCompID = dicComponentID[section];
				int nType = (int)(section.GetComponentType());
				string szCompID = section.Data.ComponentID;

				sb.AppendFormat("insert into GroupComponent ( GroupID, CID, type, ComponentID ) values ({0}, {1}, {2}, '{3}');"
					, nGroupID
					, nCompID
					, nType
					, szCompID);
				sb.AppendLine("");
			}
			string szSQL = sb.ToString();
			if (szSQL == "")
				return false;
			ArrayList arResult = dbMgr.GetResultData(szSQL, 1);
			if (arResult == null)
				return false;
			return true;
		}

        // Return 값 : 새로 생성된 StepMember들의 ID List
        //             저장에 실패하면 null을 리턴
        private Dictionary<System.Windows.Forms.TabPage, Dictionary<StepMemberData, int>> AddStepMembers(FormMain frm, WebDBManager dbMgr, Dictionary<System.Windows.Forms.TabPage, int> dicActionStepIDs)
        {
			FormPageSOP pageLevel = frm.GetPageLevel();

            string strSQL = "Select max(id) from StepMember";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 1);

            int nStepMemberID;

            if (arrResult == null || arrResult.Count == 0)
                nStepMemberID = 0;
            else
                nStepMemberID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);

            Type panelType = typeof(Sections.PanelSectionEx);
            Dictionary<System.Windows.Forms.TabPage, Dictionary<StepMemberData, int>> dicStepMembers = new Dictionary<System.Windows.Forms.TabPage, Dictionary<StepMemberData, int>>();

            // TeamType : 0(평일 비상 조직, TemporaryNormalTeam), 1(휴일 비상 조직, TemporaryEmergencyTeam), 2(외부 기관, ExternalTeam), 3(사용자 정의 조직, UserDefinedTeam), 4(정규 조직, RegularTeam)
            foreach (KeyValuePair<System.Windows.Forms.TabPage, int> pair in dicActionStepIDs)
            {
                System.Windows.Forms.TabPage page = pair.Key;
                int nActionStepID = pair.Value;

                Dictionary<StepMemberData, int> dicStepMember = new Dictionary<StepMemberData, int>();

                foreach (System.Windows.Forms.Control control in page.Controls)
                {
                    if (control.GetType() == panelType)
                    {
                        Sections.PanelSectionEx panel = (Sections.PanelSectionEx)control;

                        panel.ActionStepID = nActionStepID;

                        int nTeamID = panel.TeamID;
                        string strTeamName = panel.TeamName;
                        int nTeamType = panel.TeamType;

                        if (nTeamID < 0)
                        {
                            if (nTeamType == 3)
                                nTeamID = AddUserDefinedTeam(dbMgr, strTeamName, true);
                            else if (nTeamType == 2)
                                nTeamID = AddExternalTeam(dbMgr, strTeamName, true);

                            if (nTeamID < 0)
                                return null;
                        }

                        strSQL = string.Format("insert into StepMember (ID, TeamID, TeamType, ActionStepID) values ({0}, {1}, {2}, {3})",
                            ++nStepMemberID, nTeamID, nTeamType, nActionStepID);

                        if (dbMgr.GetResultData(strSQL, 1) == null)
                            return null;

                        dicStepMember[new StepMemberData(strTeamName, nTeamID, nTeamType)] = nStepMemberID;
                    }
                }

                dicStepMembers[page] = dicStepMember;
            }

            return dicStepMembers;
        }

        public int AddUserDefinedTeam(WebDBManager dbMgr, string strTeamName, bool transaction)
        {
            string strSQL = "select max(id) from UserDefinedTeam";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, transaction ? 1 : 0);

            int nTeamID;

            if (arrResult == null || arrResult.Count == 0)
                nTeamID = 0;
            else
                nTeamID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);

            strSQL = string.Format("Insert into UserDefinedTeam (ID, TeamName, PhoneNumber, FaxNumber) values ({0}, '{1}', '0000000', NULL)",
                ++nTeamID, ChangeSpecialCharacter(strTeamName));

            return dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null ? -1 : nTeamID;
        }

		public bool DeleteUserDefinedTeam(WebDBManager dbMgr, string strTeamName, bool transaction)
		{
			string strSQL = string.Format("delete from UserDefinedTeam where TeamName = '{0}'", ChangeSpecialCharacter(strTeamName));

			dbMgr.GetResultData(strSQL, 0);

			string strSQL2 = string.Format("select * from UserDefinedTeam where TeamName = '{0}'", ChangeSpecialCharacter(strTeamName));
			ArrayList arResult = dbMgr.GetResultData(strSQL2, 0);

			if (arResult == null || arResult.Count == 0)
			{
				return true;
			}
			return false;
		}

        public int AddExternalTeam(WebDBManager dbMgr, string strTeamName, bool transaction)
        {
            string strSQL = "select max(id) from ExternalTeam";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, transaction ? 1 : 0);

            int nTeamID;

            if (arrResult == null || arrResult.Count == 0)
                nTeamID = 0;
            else
                nTeamID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);

            strSQL = string.Format("Insert into ExternalTeam (ID, TeamName, PhoneNumber, FaxNumber) values ({0}, '{1}', '0000000', NULL)",
                ++nTeamID, ChangeSpecialCharacter(strTeamName));

            return dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null ? -1 : nTeamID;
        }

		public bool DeleteExternalTeam(WebDBManager dbMgr, string strTeamName, bool transaction)
		{
			string strSQL = string.Format("delete from ExternalTeam where TeamName = '{0}'", ChangeSpecialCharacter(strTeamName));

			dbMgr.GetResultData(strSQL, 0);

			string strSQL2 = string.Format("select * from ExternalTeam where TeamName = '{0}'", ChangeSpecialCharacter(strTeamName));
			ArrayList arResult = dbMgr.GetResultData(strSQL2, 0);

			if (arResult == null || arResult.Count == 0)
			{
				return true;
			}
			return false;
		}

        // Return 값 : 새로 생성된 ActionStep들의 ID List
        //             저장에 실패하면 null을 리턴
        public Dictionary<System.Windows.Forms.TabPage, int> AddActionSteps(FormMain frm, WebDBManager dbMgr, int nDisasterID)
        {
			FormPageSOP pageLevel = frm.GetPageLevel();
            
            string strSQL = "Select max(id) from ActionStep";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 1);

            int nActionStepID;

            if (arrResult == null || arrResult.Count == 0)
                nActionStepID = 0;
            else
                nActionStepID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);

            string strBeginTime, strEndTime;

            // TabPage별 ActionStepID
            Dictionary<System.Windows.Forms.TabPage, int> dicActionStepID = new Dictionary<System.Windows.Forms.TabPage, int>();

            // ActionStepID별 부모 TabPage
            Dictionary<int, TabPage> dicParentTabPage = new Dictionary<int, TabPage>();

            foreach (System.Windows.Forms.TabPage page in pageLevel.TabControls.TabPages)
            {
                string strStepName = page.Text;
                Data_ActionStep opt = pageLevel.GetActionStepOption(page);
                if (opt == null) continue;

                if (opt.PeriodType == 0)    // 기간 사용 안함
                {
                    strBeginTime = "NULL";
                    strEndTime  = "NULL";
                }
                else
                {
                    strBeginTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", opt.BeginTime.ToShortDateString(), opt.BeginTime.Hour, opt.BeginTime.Minute, opt.BeginTime.Second);
                    strEndTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", opt.EndTime.ToShortDateString(), opt.EndTime.Hour, opt.EndTime.Minute, opt.EndTime.Second);
                }

                strSQL = string.Format("insert into ActionStep (ID, StepName, PeriodType, BeginTime, EndTime, WeekDayOption, Iteration, IterationType, ProcessTime, ProcessTimeType, DisasterID, ParentStepID) values ({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, NULL)",
                    ++nActionStepID, strStepName, opt.PeriodType, strBeginTime, strEndTime, opt.WeekdayOption, opt.Iteration, opt.IterationType, opt.ProcessTime, opt.ProcessTimeType, nDisasterID/*, opt.ParentStepID < 0 ? "NULL" : opt.ParentStepID.ToString()*/);

                if (dbMgr.GetResultData(strSQL, 1) == null)
                    return null;

                dicActionStepID[page] = nActionStepID;

                if (page.Tag != null)
                    dicParentTabPage[nActionStepID] = (TabPage)page.Tag;
            }

            // 부모 단계를 다시 입력시킨다.
            foreach (KeyValuePair<int, TabPage> pair in dicParentTabPage)
            {
                int nID = pair.Key;
                TabPage pageParent = pair.Value;

                if (!dicActionStepID.ContainsKey(pageParent))
                    return null;

                int nParentID = dicActionStepID[pageParent];

                strSQL = string.Format("Update ActionStep set ParentStepID = {0} where id = {1}", nParentID, nID);
                if (dbMgr.GetResultData(strSQL, 1) == null)
                    return null;
            }

            return dicActionStepID;
        }

        // Return 값 : Disaster ID
        //             이 값이 0보다 작으면 실패
        private int AddDisaster(FormMain frm, WebDBManager dbMgr, int nVersionID)
        {
            FormNewSOP pageDisaster = frm.GetPageDisaster();

            string strDisaster = pageDisaster.SelectedDetailCategory;
            string strSubDisaster = pageDisaster.SelectedSubCategory;
            string strCategory = pageDisaster.SelectedCategory;

            if (strDisaster == "" || strSubDisaster == "" || strCategory == "")
                return -1;

            string strSQL = string.Format("Select id from SubDisasterCategory where SubCategoryName = '{0}' and DisasterID = (select id from DisasterCategory where CategoryName = '{1}')",
                strSubDisaster, strCategory);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 1);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nSubCategoryID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);
            if (nSubCategoryID == 0)
                return -1;

            strSQL = string.Format("select max(id) from Disaster");
            arrResult = dbMgr.GetResultData(strSQL, 1);

            int nDisasterID;

            if (arrResult == null || arrResult.Count == 0)
                nDisasterID = 0;
            else
                nDisasterID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);

            strSQL = string.Format("INSERT INTO Disaster(ID, DisasterName, SubDisasterID, VersionID, Description) VALUES ({0}, '{1}', {2}, {3}, '{4}')",
                ++nDisasterID, ChangeSpecialCharacter(strDisaster), nSubCategoryID, nVersionID, ChangeSpecialCharacter(pageDisaster.DisasterDescription));

            if (dbMgr.GetResultData(strSQL, 1) == null)
                return -1;

            return nDisasterID;
        }

        private bool UpdateVersion(WebDBManager dbMgr, int nVersionID, ref VersionInfo rVersion)
        {
            DateTime dtCurrent = DateTime.Now;
            string strSQL = string.Format("update Version set LastAccessTime = '{0} {1:00}:{2:00}:{3:00}' where id = {4}", 
                dtCurrent.ToShortDateString(), dtCurrent.Hour, dtCurrent.Minute, dtCurrent.Second, nVersionID);

            if (dbMgr.GetResultData(strSQL, 1) == null)
                return false;

            strSQL = string.Format("select CreateTime, LastAccessTime, VersionName, Description from Version where id = {0}", nVersionID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 1);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            if (nResultCount < 4)
                return false;

            DateTime dtDefault = new DateTime();
            
            rVersion.BeginTime = WebDBManager.GetDateTimeField(arrResult[0], dtDefault);
            rVersion.EndTime = WebDBManager.GetDateTimeField(arrResult[1], dtDefault);
            rVersion.VersionName = WebDBManager.GetStringField(arrResult[2], "");
            rVersion.Description = WebDBManager.GetStringField(arrResult[3], "");
            rVersion.VersionID = nVersionID;

            return true;
        }

        // Return 값 : 저장된 Version의 ID
        //             저장에 실패하며 -1을 리턴
        private int SaveVersion(FormMain frm, WebDBManager dbMgr, string strVersionName, int nVersionID, int nSOPGenUserID, string strDescription, ref VersionInfo rVersion)
        {
            string strSQL;
            ArrayList arrResult;

            if (nVersionID <= 0)
            {
                strSQL = "select max(id) from version";
                arrResult = dbMgr.GetResultData(strSQL, 1);

                if (arrResult == null || arrResult.Count == 0)
                    nVersionID = 0;
                else
                    nVersionID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);
            }

            FormNewSOP pageDisaster = frm.GetPageDisaster();
            DateTime dtCurrent = DateTime.Now;

            int nRegular = pageDisaster.IsRegularMode() ? 1 : 0;
            int nNormal = pageDisaster.IsWeekMode() ? 1 : 0;
            string strCurrentTime = dtCurrent.ToShortDateString() + string.Format(" {0:00}:{1:00}:{2:00}", dtCurrent.Hour, dtCurrent.Minute, dtCurrent.Second);

            strSQL = string.Format("INSERT INTO Version(ID, isRegular, isNormal, CreateTime, LastAccessTime, VersionName, OwnerID, Description) VALUES ({0}, {1}, {2}, '{3}', '{4}', '{5}', {6}, '{7}')",
                ++nVersionID, nRegular, nNormal, strCurrentTime, strCurrentTime, ChangeSpecialCharacter(strVersionName), nSOPGenUserID, ChangeSpecialCharacter(strDescription));

            if (dbMgr.GetResultData(strSQL, 1) == null)
                return -1;

            rVersion.BeginTime = dtCurrent;
            rVersion.Description = strDescription;
            rVersion.EndTime = dtCurrent;
            rVersion.VersionID = nVersionID;
            rVersion.VersionName = strVersionName;

            return nVersionID;
        }

        // 기존 버전을 삭제
        public bool DeleteSOPVersion(WebDBManager dbMgr, int nVersionID, bool deleteVersion, bool transaction)
        {
            string strSQL = "select id from Disaster where VersionID = " + nVersionID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            if (arrResult.Count == 0)
            {
                if (deleteVersion)
                    return DeleteVersion(dbMgr, nVersionID, transaction);
                return true;
            }

            string strDisasterIDs = "";

            foreach (object obj in arrResult)
            {
                string strID = obj.ToString();

                if (strDisasterIDs.Length == 0)
                    strDisasterIDs = strID;
                else
                    strDisasterIDs += ", " + strID;
            }

            strSQL = string.Format("select id from ActionStep where DisasterID in ({0})", strDisasterIDs);
            arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult.Count == 0)
            {
                if (!DeleteDisaster(dbMgr, nVersionID, transaction))
                    return false;

                if (deleteVersion)
                    return DeleteVersion(dbMgr, nVersionID, transaction);
                return true;
            }

            string strActionStepIDs = "";

            foreach (object obj in arrResult)
            {
                if (strActionStepIDs.Length == 0)
                    strActionStepIDs = obj.ToString();
                else
                    strActionStepIDs += ", " + obj.ToString();
            }

            strSQL = string.Format("select id from StepMember where ActionStepID in ({0})", strActionStepIDs);
            arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            if (arrResult.Count == 0)
            {
                if (!DeleteActionStepHistory(dbMgr, strActionStepIDs, transaction))
                    return false;
                if (!DeleteActionStep(dbMgr, strDisasterIDs, transaction))
                    return false;
                if (!DeleteDisaster(dbMgr, nVersionID, transaction))
                    return false;
                if (deleteVersion)
                    return DeleteVersion(dbMgr, nVersionID, transaction);
                return true;
            }

            string strStepMemberIDs = "";

            foreach (object obj in arrResult)
            {
                if (strStepMemberIDs.Length == 0)
                    strStepMemberIDs = obj.ToString();
                else
                    strStepMemberIDs += ", " + obj.ToString();
            }

            if (strStepMemberIDs.Length > 0)
            {
                if (!DeleteComponent(dbMgr, strStepMemberIDs, transaction))
                    return false;
            }

            if (!DeleteActionStepHistory(dbMgr, strActionStepIDs, transaction))
                return false;
            if (!DeleteStepMember(dbMgr, strActionStepIDs, transaction))
                return false;
            if (!DeleteActionStep(dbMgr, strDisasterIDs, transaction))
                return false;
            if (!DeleteDisaster(dbMgr, nVersionID, transaction))
                return false;
            if (deleteVersion)
                return DeleteVersion(dbMgr, nVersionID, transaction);

            return true;
        }

        private bool DeleteVersion(WebDBManager dbMgr, int nVersionID, bool transaction)
        {
            string strSQL = "delete from Version where id = " + nVersionID.ToString();
            return dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null ? false : true;
        }

        private bool DeleteDisaster(WebDBManager dbMgr, int nVersionID, bool transaction)
        {
            string strSQL = "delete from Disaster where VersionID = " + nVersionID.ToString();
            return dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null ? false : true;
        }

        private bool DeleteActionStepHistory(WebDBManager dbMgr, string strActionStepIDs, bool transaction)
        {
            string strSQL = string.Format("select id from ActionStepHistory where ActionStepID in ({0})", strActionStepIDs);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, transaction ? 1 : 0);

            if (arrResult == null)
                return false;

            string strActionStepHistoryIDs = "";
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount; i++)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);

                if (strActionStepHistoryIDs.Length == 0)
                    strActionStepHistoryIDs = nID.ToString();
                else
                    strActionStepHistoryIDs += ", " + nID.ToString();
            }

            if (strActionStepHistoryIDs.Length == 0)
                return true;

            //strSQL = string.Format("delete from ComponentHistory where ActionStepID in ({0})", strActionStepIDs);
            strSQL = string.Format("delete from ComponentHistory where ActionStepHistoryID in ({0})", strActionStepHistoryIDs);
            if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                return false;

            strSQL = string.Format("delete from Message where ActionStepHistoryID in ({0})", strActionStepHistoryIDs);
            if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                return false;

            strSQL = string.Format("delete from ActionStepHistory where ID in ({0})", strActionStepHistoryIDs);
            if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                return false;

            return true;
        }

        // ActionStep ID별 TabPage를 얻어온다.
        private Dictionary<int, TabPage> GetTabPageActionStepList()
        {
			FormPageSOP pageLevel = FormMain.Instance.GetPageLevel();

            // ActionStepID별 TabPage
            Dictionary<int, TabPage> dicActionStepID = new Dictionary<int, TabPage>();
            Type type = typeof(Sections.PanelSectionEx);

            foreach (System.Windows.Forms.TabPage page in pageLevel.TabControls.TabPages)
            {
                foreach (Control ctrl in page.Controls)
                {
                    if (ctrl.GetType() == type)
                    {
                        Sections.PanelSectionEx panel = (Sections.PanelSectionEx)ctrl;
                        dicActionStepID[panel.ActionStepID] = page;
                        break;
                    }
                }
            }

            return dicActionStepID;
        }

        // TransSOP가 존재할 경우 이미 지워진 ActionStep ID를 가지고 있을 수 있으므로
        // 이를 보정하기 위하여 삭제된 Tab별 ActionStep ID를 기억시킨다.
        private void SaveDeletingActionStepID()
        {
			FormPageSOP pageLevel = FormMain.Instance.GetPageLevel();
            Type type = typeof(Sections.PanelSectionEx);

            foreach (System.Windows.Forms.TabPage page in pageLevel.TabControls.TabPages)
            {
                foreach (Control ctrl in page.Controls)
                {
                    if (ctrl.GetType() == type)
                    {
                        Sections.PanelSectionEx panel = (Sections.PanelSectionEx)ctrl;
                        if (panel.ActionStepID < 0)
                            break;

                        m_dicDeletedActionStep[panel.ActionStepID] = page;
                        break;
                    }
                }
            }
        }

        private bool DeleteActionStep(WebDBManager dbMgr, string strDisasterIDs, bool transaction)
        {
            string strSQL = string.Format("delete from Message where ActionStepID in (select id from ActionStep where DisasterID in ({0}))", strDisasterIDs);
            if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                return false;

            strSQL = string.Format("delete from ActionStep where DisasterID in ({0})", strDisasterIDs);
            return dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null ? false : true;
        }

        private bool DeleteStepMember(WebDBManager dbMgr, string strActionStepIDs, bool transaction)
        {
            string strSQL = string.Format("delete from StepMember where ActionStepID in ({0})", strActionStepIDs);
            return dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null ? false : true;
        }

        private bool DeleteComponent(WebDBManager dbMgr, string strStepMemberIDs, bool transaction)
        {
            string strSQL = string.Format("select id from Process where StepMemberID in ({0})", strStepMemberIDs);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, transaction ? 1 : 0);

            if (arrResult == null)
                return false;

            string strProcessIDs = "";

            foreach (object obj in arrResult)
            {
                if (strProcessIDs.Length == 0)
                    strProcessIDs = obj.ToString();
                else
                    strProcessIDs += ", " + obj.ToString();
            }

            if (strProcessIDs.Length > 0)
            {
                strSQL = string.Format("delete from CheckTask where ProcessID in ({0})", strProcessIDs);
                if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                    return false;
            }

            if (strProcessIDs.Length > 0)
            {
                strSQL = string.Format("delete from ProcessMission where ProcessID in ({0})", strProcessIDs);
                if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                    return false;
            }

            strSQL = string.Format("delete from Process where StepMemberID in ({0})", strStepMemberIDs);
            if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                return false;

            strSQL = string.Format("delete from Annotation where StepMemberID in ({0})", strStepMemberIDs);
            if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                return false;

            strSQL = string.Format("delete from ExternalTransmission where StepMemberID in ({0})", strStepMemberIDs);
            if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                return false;

            strSQL = string.Format("delete from InternalTransmission where StepMemberID in ({0})", strStepMemberIDs);
            if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                return false;

            strSQL = string.Format("delete from Transmission where StepMemberID in ({0})", strStepMemberIDs);
            if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                return false;

            strSQL = string.Format("delete from Decision where StepMemberID in ({0})", strStepMemberIDs);
            if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                return false;

            strSQL = string.Format("delete from EndPoint where StepMemberID in ({0})", strStepMemberIDs);
            if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                return false;

            strSQL = string.Format("delete from Link where StepMemberID in ({0})", strStepMemberIDs);
            if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                return false;

            strSQL = string.Format("delete from TransSOP where StepMemberID in ({0})", strStepMemberIDs);
            if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                return false;

            strSQL = string.Format("delete from Arrow where StepMemberID in ({0})", strStepMemberIDs);
            if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                return false;

            return true;
        }
    }

    public struct StepMemberData
    {
        private string m_strTeamName;
        private int m_nTeamID;
        private int m_nTeamType;

        public StepMemberData(string strTeamName, int nTeamID, int nTeamType)
        {
            m_strTeamName = strTeamName;
            m_nTeamID = nTeamID;
            m_nTeamType = nTeamType;
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
    }

    public class StepMemberDataEx
    {
        private int m_nTeamID = -1;
        private int m_nTeamType = -1;
        private int m_nStepMemberID = -1;

        public StepMemberDataEx(int nTeamID, int nTeamType, int nStepMemberID)
        {
            m_nTeamID = nTeamID;
            m_nTeamType = nTeamType;
            m_nStepMemberID = nStepMemberID;
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
    }
}
