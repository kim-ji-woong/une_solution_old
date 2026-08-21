using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using DBUtility;

namespace SOPBulletin
{
    // SOP Data를 DB에 저장 및 불러오기 담당
    public class IOManager
    {
        public IOManager()
        {
        }

        private bool LoadProcessMission(WebDBManager dbMgr, int nProcessID, ArrayList arrMissionItems)
        {
            string strSQL = string.Format("Select ID, missionText from ProcessMission where ProcessID = {0}", nProcessID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

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

        private bool LoadProcess(WebDBManager dbMgr, int nStepMemberID, Dictionary<int, Sections.Section> dicSections, Sections.PanelSection panel, Dictionary<int, string> dicNormal, Dictionary<int, string> dicEmergency, Dictionary<int, string> dicUserDefined, Dictionary<int, string> dicExternal, Dictionary<int, string> dicRegular)
        {
            string strSQL = "select id, x, y, width, height, text, TeamList, ComponentID, ProcessTime, ProcessTimeType, useProcessTime, useMissionMessage";
            strSQL += ", onlyTeamLeader, CommanderMemberType, CommanderMemberID, CommanderDisplayText from Process where StepMemberID = " + nStepMemberID.ToString();

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

                Sections.SectionProcess section = new Sections.SectionProcess(panel, x, y);
                Sections.SectionDataProcess sectionData = (Sections.SectionDataProcess)section.Data;
                sectionData.ShowMessageBox = false;

                // ComponentID는 Type별로 중복될수 있으므로 ComponentType을 Int의 제일 첫번째 Byte를 ComponentType에 할당한다.
                int nKey = (((int)Sections.Section.ComponentType.PROCESS) << 24) | nID;
                dicSections[nKey] = section;

                section.RectSize = new SizeF(fWidth, fHeight);
                section.TextUP = strText;
                section.TextDown = GetTeamList(dbMgr, strTeamList, dicNormal, dicEmergency, dicUserDefined, dicExternal, dicRegular);
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
                sectionData.Commander = GetSectionCommander(dbMgr, nCommanderMemberType, nCommanderMemberID, strCommanderDisplayText);

                if (!LoadProcessMission(dbMgr, nID, sectionData.MissionItems))
                    return false;

                //if (!LoadCheckedItems(dbMgr, nID, sectionData.CheckedItems))
                //    return false;
            }

            return true;
        }

        private string GetTeamList(WebDBManager dbMgr, string strTeamList, Dictionary<int, string> dicNormal, Dictionary<int, string> dicEmergency, Dictionary<int, string> dicUserDefined, Dictionary<int, string> dicExternal, Dictionary<int, string> dicRegular)
        {
            int nBeginIndex = 0;
            int nLen = strTeamList.Length;

            string strTeamNameList = "";

            while (nBeginIndex < nLen)
            {
                int nDotIndex = strTeamList.IndexOf(',', nBeginIndex);
                if (nDotIndex < 0) break;

                if (!GetTeamName(dbMgr, ref strTeamNameList, strTeamList, nBeginIndex, nDotIndex, dicNormal, dicEmergency, dicUserDefined, dicExternal, dicRegular))
                    return "";

                nBeginIndex = nDotIndex + 1;
            }

            if (!GetTeamName(dbMgr, ref strTeamNameList, strTeamList, nBeginIndex, nLen, dicNormal, dicEmergency, dicUserDefined, dicExternal, dicRegular))
                return "";

            return strTeamNameList;
        }

        private bool GetTeamName(WebDBManager dbMgr, ref string strTeamNameList, string strTeamList, int nBeginIndex, int nEndIndex, Dictionary<int, string> dicNormal, Dictionary<int, string> dicEmergency, Dictionary<int, string> dicUserDefined, Dictionary<int, string> dicExternal, Dictionary<int, string> dicRegular)
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

            string strTeamName = null;
            int nTeamID;

            if (!int.TryParse(strTeamID, out nTeamID))
                return false;

            if (strTeamType == "0")
            {
                if (!dicNormal.TryGetValue(nTeamID, out strTeamName))
                {
                    strTeamName = GetTeamLeaderName(dbMgr, nTeamID, "TemporaryNormalTeam");
                    dicNormal[nTeamID] = strTeamName;
                }
            }
            else if (strTeamType == "1")
            {
                if (!dicEmergency.TryGetValue(nTeamID, out strTeamName))
                {
                    strTeamName = GetTeamLeaderName(dbMgr, nTeamID, "TemporaryEmergencyTeam");
                    dicEmergency[nTeamID] = strTeamName;
                }
            }
            else if (strTeamType == "2")
            {
                if (!dicExternal.TryGetValue(nTeamID, out strTeamName))
                {
                    strTeamName = GetTeamLeaderName(dbMgr, nTeamID, "ExternalTeam");
                    dicEmergency[nTeamID] = strTeamName;
                }
            }
            else if (strTeamType == "3")
            {
                if (!dicUserDefined.TryGetValue(nTeamID, out strTeamName))
                {
                    strTeamName = GetTeamLeaderName(dbMgr, nTeamID, "UserDefinedTeam");
                    dicUserDefined[nTeamID] = strTeamName;
                }
            }
            else if (strTeamType == "4")
            {
                if (!dicRegular.TryGetValue(nTeamID, out strTeamName))
                {
                    strTeamName = GetTeamLeaderName(dbMgr, nTeamID, "RegularTeam");

                    if (!strTeamName.EndsWith("장") && !strTeamName.EndsWith("본부"))
                        strTeamName += "장";

                    dicRegular[nTeamID] = strTeamName;
                }
            }
            else
                return false;

            if (strTeamName == null)
                return false;

            if (strTeamNameList.Length == 0)
                strTeamNameList = strTeamName;
            else
                strTeamNameList += ", " + strTeamName;

            return true;
        }

        private SectionCommanderEx GetSectionCommander(WebDBManager dbMgr, int nCommanderMemberType, int nCommanderMemberID, string strDisplayText)
        {
            SectionCommanderEx commander = new SectionCommanderEx();

            if (nCommanderMemberType == -1)
                commander.IsDefaultCommander = true;
            else if (strDisplayText != "null")
                commander.DisplayText = strDisplayText;
            else
            {
                // 이 값이 NULL이면 발신자가 존재하지 않는다. -1이면 SOPGenUserCommander의 값을 따른다. 
                // (0 : 평일 비상 조직-TemporaryNormalTeam, 1 : 휴일 비상 조직-TemporaryEmergencyTeam, 2 : 외부 기관-ExternalTeam 또는 ExternalCompanyTeam, 
                // 3 : 사용자 정의 조직-UserDefinedTeam, 4 : 상시조직-RegularTeam, 5 : 평일 비상 조직 조직원, 6 : 휴일 비상 조직 조직원, 7 : 협력업체 직원, 8 : 정규직원)
                // 예 : 1(0), 1(3). 팀일 경우 해당 팀의 팀장으로 설정됨.
                if (nCommanderMemberType == 0)
                    commander.DisplayText = GetTeamLeaderName(dbMgr, nCommanderMemberID, "TemporaryNormalTeam");
                else if (nCommanderMemberType == 1)
                    commander.DisplayText = GetTeamLeaderName(dbMgr, nCommanderMemberID, "TemporaryEmergencyTeam");
                else if (nCommanderMemberType == 2)
                    commander.DisplayText = GetTeamLeaderName(dbMgr, nCommanderMemberID, "ExternalTeam");
                else if (nCommanderMemberType == 3)
                    commander.DisplayText = GetTeamLeaderName(dbMgr, nCommanderMemberID, "UserDefinedTeam");
                else if (nCommanderMemberType == 4)
                {
                    commander.DisplayText = GetTeamLeaderName(dbMgr, nCommanderMemberID, "RegularTeam");

                    if (!commander.DisplayText.EndsWith("장"))
                        commander.DisplayText += "장";
                }
                else if (nCommanderMemberType == 5 || nCommanderMemberType == 6)
                    commander.DisplayText = GetTemporaryMemberName(dbMgr, nCommanderMemberID);
                else if (nCommanderMemberType == 7)
                    commander.DisplayText = GetMemberName(dbMgr, nCommanderMemberID, "ExternalCompanyMember", "Name");
                else if (nCommanderMemberType == 8)
                    commander.DisplayText = GetMemberName(dbMgr, nCommanderMemberID, "CompanyMember", "MemberName");
                else
                    commander.DisplayText = "";
            }

            return commander;
        }

        private string GetMemberName(WebDBManager dbMgr, int nID, string strTableName, string strFieldName)
        {
            string strSQL = string.Format("Select {0} from {1} where ID = {2}", strFieldName, strTableName, nID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

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
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

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
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return "";

            string strTeamName = WebDBManager.GetStringField(arrResult[0], "null");

            if (strTeamName == "null")
                return "";

            return strTeamName + strAdd;
        }

        private bool LoadDecision(WebDBManager dbMgr, int nStepMemberID, Dictionary<int, Sections.Section> dicSections, Sections.PanelSection panel)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID from Decision where StepMemberID = " + nStepMemberID.ToString();
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
                sectionData.ShowMessageBox = false;

                // ComponentID는 Type별로 중복될수 있으므로 ComponentType을 Int의 제일 첫번째 Byte를 ComponentType에 할당한다.
                int nKey = (((int)Sections.Section.ComponentType.DECISION) << 24) | nID;
                dicSections[nKey] = section;

                section.RectSize = new SizeF(fWidth, fHeight);
                section.Title = strText;

                sectionData.ID = nID;
                sectionData.Title = strText;
                sectionData.ComponentID = strComponentID;
            }

            return true;
        }

        private bool LoadEndPoint(WebDBManager dbMgr, int nStepMemberID, Dictionary<int, Sections.Section> dicSections, Sections.PanelSection panel)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID, isBegin from EndPoint where StepMemberID = " + nStepMemberID.ToString();
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
                sectionData.ShowMessageBox = false;

                // ComponentID는 Type별로 중복될수 있으므로 ComponentType을 Int의 제일 첫번째 Byte를 ComponentType에 할당한다.
                int nKey = (((int)Sections.Section.ComponentType.ENDPOINT) << 24) | nID;
                dicSections[nKey] = section;

                section.RectSize = new SizeF(fWidth, fHeight);
                section.Title = strText;

                sectionData.ID = nID;
                sectionData.Title = strText;
                sectionData.ComponentID = strComponentID;
                sectionData.IsBegin = isBegin;
            }

            return true;
        }

        private bool LoadTransSOP(WebDBManager dbMgr, int nStepMemberID, Dictionary<int, Sections.Section> dicSections, Sections.PanelSection panel)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID, LinkedActionStepID, Description from TransSOP where StepMemberID = " + nStepMemberID.ToString();
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
                sectionData.ShowMessageBox = false;

                // ComponentID는 Type별로 중복될수 있으므로 ComponentType을 Int의 제일 첫번째 Byte를 ComponentType에 할당한다.
                int nKey = (((int)Sections.Section.ComponentType.TRANSSOP) << 24) | nID;
                dicSections[nKey] = section;

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

        private bool LoadInternal(WebDBManager dbMgr, int nStepMemberID, Dictionary<int, Sections.Section> dicSections, Sections.PanelSection panel, Dictionary<int, string> dicNormal, Dictionary<int, string> dicEmergency, Dictionary<int, string> dicUserDefined, Dictionary<int, string> dicExternal, Dictionary<int, string> dicRegular)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID, usePopupMessage, useMobileApp, useBroadcast, TeamList, CommanderMemberType, CommanderMemberID, CommanderDisplayText from InternalTransmission where StepMemberID = " + nStepMemberID.ToString();
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
                bool usePopupMessage = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0) == 0 ? false : true;
                bool useMobileApp = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 0) == 0 ? false : true;
                bool useBroadcast = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 0) == 0 ? false : true;
                string strTeamList = WebDBManager.GetStringField(arrResult[i + 10]);
                int nCommanderMemberType = WebDBManager.GetIntField(arrResult[i + 11].ToString(), -1);
                int nCommanderMemberID = WebDBManager.GetIntField(arrResult[i + 12].ToString(), -1);
                string strCommanderDisplayText = WebDBManager.GetStringField(arrResult[i + 13], "null");

                Sections.SectionInternal section = new Sections.SectionInternal(panel, x, y);
                Sections.SectionDataInternal sectionData = (Sections.SectionDataInternal)section.Data;
                sectionData.ShowMessageBox = false;

                // ComponentID는 Type별로 중복될수 있으므로 ComponentType을 Int의 제일 첫번째 Byte를 ComponentType에 할당한다.
                int nKey = (((int)Sections.Section.ComponentType.INTERNAL) << 24) | nID;
                dicSections[nKey] = section;
                
                section.RectSize = new SizeF(fWidth, fHeight);
                section.Title = strText;

                sectionData.ID = nID;
                sectionData.Title = strText;
                sectionData.ComponentID = strComponentID;
                sectionData.UsePopupMessage = usePopupMessage;
                sectionData.UseMobileApp = useMobileApp;
                sectionData.UseBroadcast = useBroadcast;
                sectionData.Commander = GetSectionCommander(dbMgr, nCommanderMemberType, nCommanderMemberID, strCommanderDisplayText);

                if (!useBroadcast && strTeamList != null)
                {
                    string strTeamListName = GetTeamList(dbMgr, strTeamList, dicNormal, dicEmergency, dicUserDefined, dicExternal, dicRegular);
                    sectionData.TeamList.Add(strTeamListName);
                }
            }

            return true;
        }

        private bool LoadExternal(WebDBManager dbMgr, int nStepMemberID, Dictionary<int, Sections.Section> dicSections, Sections.PanelSection panel)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID, useSMS, SMSText, SMSExternalTeamIDList, useEFax, FaxExternalTeamIDList from ExternalTransmission where StepMemberID = " + nStepMemberID.ToString();
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
                sectionData.ShowMessageBox = false;

                // ComponentID는 Type별로 중복될수 있으므로 ComponentType을 Int의 제일 첫번째 Byte를 ComponentType에 할당한다.
                int nKey = (((int)Sections.Section.ComponentType.EXTERNAL) << 24) | nID;
                dicSections[nKey] = section;

                section.RectSize = new SizeF(fWidth, fHeight);
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

        private bool LoadTransmission(WebDBManager dbMgr, int nStepMemberID, Dictionary<int, Sections.Section> dicSections, Sections.PanelSection panel)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID, useInternalPopupMessage, useInternalMobileApp, useInternalBroadcast, "
                + "useExternalSMS, externalSMSText, SMSExternalTeamIDList, useExternalFax, FaxExternalTeamIDList from Transmission where StepMemberID = " + nStepMemberID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 14; i += 15)
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

                Sections.SectionTransmission section = new Sections.SectionTransmission(panel, x, y);
                Sections.SectionDataTransmission sectionData = (Sections.SectionDataTransmission)section.Data;
                sectionData.ShowMessageBox = false;

                // ComponentID는 Type별로 중복될수 있으므로 ComponentType을 Int의 제일 첫번째 Byte를 ComponentType에 할당한다.
                int nKey = (((int)Sections.Section.ComponentType.TRANSMISSION) << 24) | nID;
                dicSections[nKey] = section;

                section.RectSize = new SizeF(fWidth, fHeight);
                section.Title = strText;

                sectionData.ID = nID;
                sectionData.Title = strText;
                sectionData.ComponentID = strComponentID;

                sectionData.DataInternal.UsePopupMessage = useInternalPopupMessage;
                sectionData.DataInternal.UseMobileApp = useInternalMobileApp;
                sectionData.DataInternal.UseBroadcast = useInternalBroadcast;

                sectionData.DataExternal.UseSMS = useExternalSMS;
                sectionData.DataExternal.UseFax = useExternalFax;
                sectionData.DataExternal.SMSMessage = strExternalSMSText;
            }

            return true;
        }

        public bool LoadSections(WebDBManager dbMgr, int nStepMemberID, Dictionary<int, Sections.Section> dicSections, Sections.PanelSection panel)
        {
            Dictionary<int, string> dicNormal = new Dictionary<int,string>();
            Dictionary<int, string> dicEmergency = new Dictionary<int,string>();
            Dictionary<int, string> dicUserDefined = new Dictionary<int,string>();
            Dictionary<int, string> dicExternal = new Dictionary<int,string>();
            Dictionary<int, string> dicRegular = new Dictionary<int, string>();

            if (!LoadProcess(dbMgr, nStepMemberID, dicSections, panel, dicNormal, dicEmergency, dicUserDefined, dicExternal, dicRegular))
                return false;
            if (!LoadDecision(dbMgr, nStepMemberID, dicSections, panel))
                return false;
            if (!LoadEndPoint(dbMgr, nStepMemberID, dicSections, panel))
                return false;
            if (!LoadTransSOP(dbMgr, nStepMemberID, dicSections, panel))
                return false;
            if (!LoadInternal(dbMgr, nStepMemberID, dicSections, panel, dicNormal, dicEmergency, dicUserDefined, dicExternal, dicRegular))
                return false;
            if (!LoadExternal(dbMgr, nStepMemberID, dicSections, panel))
                return false;
            if (!LoadTransmission(dbMgr, nStepMemberID, dicSections, panel))
                return false;

            if (!LoadArrow(dbMgr, nStepMemberID, dicSections))
                return false;

            SetSectionNumber(dicSections);
            return true;
        }

        private void SetSectionNumber(Dictionary<int, Sections.Section> dicSections)
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
                        section.Data.SectionNumber = number++;
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
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

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

        public static void GetTransmissionCheckedNotify(Sections.SectionTransmission section, out int nCheckedNotify1, out int nCheckedNotify2)
        {
            nCheckedNotify1 = 0;
            nCheckedNotify2 = 0;
            Sections.SectionDataTransmission data = (Sections.SectionDataTransmission)section.Data;
            if (data == null)
                return;

            if (data.DataInternal.UsePopupMessage)
                nCheckedNotify1 |= 1;

            if (data.DataInternal.UseMobileApp)
                nCheckedNotify1 |= 2;

            if (data.DataInternal.UseBroadcast)
                nCheckedNotify1 |= 4;

            int nIdx = 3;
            int nBit = 0;
            if (data.DataExternal.UseSMS)
            {
                foreach (Sections.ExternalTeamData exTeam in data.DataExternal.SMSReceivers)
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
            if (data.DataExternal.UseFax)
            {
                foreach (Sections.ExternalTeamData exTeam in data.DataExternal.FaxReceivers)
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

            if (data.DataExternal.UseSMS)
                nCheckedNotify1 |= (1 << 31);

            if (data.DataExternal.UseFax)
                nCheckedNotify2 |= (1 << 31);
        }
    }
}
