using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using DBUtility2;

namespace SOPManager
{
    // SOP Data를 DB에 저장 및 불러오기 담당
    public class IOManager : IDisposable
    {
        // 같은 버전으로 덮어쓰기 할 경우 저장에 앞서 먼저 버전을 삭제한다.
        // 이때 삭제된 ActionStep 정보를 기억시키기 위하여 TabPage별 삭제된 ActionStep 정보를 기억시킨다.
        private Dictionary<int, TabPage> m_dicDeletedActionStep = new Dictionary<int, TabPage>();

        private int m_nSiteID = 1;
        public IOManager()
        {
            m_nSiteID = FormMain.Instance.SiteID;
        }

		public void Dispose()
		{ }

        public bool Load(FormMain frm, WebDBManager dbMgr, VersionInfo version, ArrayList arrActionSteps, string strCategoryName, string strSubCategoryName, string strDisasterName)
        {
            ClearSOP(frm);

			FormPageSOP pageLevel = frm.GetPageLevel();

            string strFullPath = LoadDisasterTree(frm, strCategoryName, strSubCategoryName, strDisasterName, arrActionSteps);
			pageLevel.GetPropertiesLevel().SetTitleText(strFullPath);

            ArrayList arrTeams = LoadBarPage(pageLevel, arrActionSteps, dbMgr);
            if (arrTeams == null)
                return false;

            if (!LoadPane(dbMgr, pageLevel, arrActionSteps, arrTeams))
                return false;

            return true;
        }

        // dicTeamName : TeamID, TeamName
        public static bool ReadTeamList(WebDBManager dbMgr, string strTableName, Dictionary<int, string> dicTeamName)
        {
            if (strTableName == "RegularTeam")
            {
                // Site별로 사용할 수 있도록 수정 , Edit by skkim 2015.01.09
                // SiteID로 본부 아이디를 가져온다.
                string szSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", FormMain.Instance.SiteID);
                ArrayList arrResult1 = dbMgr.GetResultData(szSQL);
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
                foreach (Data_ControlRoom item in FormMain.Instance.ControlRoom)
	            {
                    if (!dicTeamName.ContainsKey(item.ID))
                        dicTeamName.Add(item.ID, item.TeamName);
                    else
                        dicTeamName[item.ID] = item.TeamName;
	            }
            }
            else
            {
                string strSQL = "select id, TeamName from " + strTableName;
                strSQL += " WHERE SiteID = " + FormMain.Instance.SiteID.ToString();

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

        public static Dictionary<int, Sections.ExternalTeamData> ReadExternalTeamList(WebDBManager dbMgr)
        {
            //string strSQL = "SELECT id, TeamName, PhoneNumber, FaxNumber from ExternalTeam";
            // Edit by Skkim. 2015.01.09 , 여러 Site에서 사용할 수 있도록 SiteID를 지정
            string szText = "SELECT id, TeamName, PhoneNumber, FaxNumber, ParentTeamID FROM ExternalTeam WHERE SiteID = {0}";
            
            string strSQL = string.Format(szText, FormMain.Instance.SiteID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            Dictionary<int, Sections.ExternalTeamData> dicExternal = new Dictionary<int,Sections.ExternalTeamData>();

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-4;i+=5)
            {
                int nTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 2].ToString(), "");
                string strFaxNumber = WebDBManager.GetStringField(arrResult[i + 3].ToString(), "");
                
                int ParentID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);

                if (strPhoneNumber == null || strPhoneNumber == "null")
                    strPhoneNumber = "";
                if (strFaxNumber == null || strFaxNumber == "null")
                    strFaxNumber = "";

                Sections.ExternalTeamData data = new Sections.ExternalTeamData();
                data.TeamID = nTeamID;
                data.TeamName = strTeamName;
                data.PhoneNumber = strPhoneNumber;
                data.FaxNumber = strFaxNumber;
                data.ParentTeamID = ParentID;

                dicExternal[nTeamID] = data;
            }

            return dicExternal;
        }

        public static bool GetTeamName(WebDBManager dbMgr, 
                                        ref ArrayList arResult, 
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

                    if (nTeamID < 0)
                        nTeamID = -nTeamID;

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

            try
            {
                bool includeChildTeams = true;
                int nTeamID = int.Parse(strTeamID);

                if (nTeamID < 0)
                {
                    nTeamID = -nTeamID;
                    includeChildTeams = false;
                }

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
                team.TeamType = (Sections.SOPTeam.SOPTeamType)int.Parse(strTeamType);
                team.TeamName = strTeamName;

                if (team.TeamType == Sections.SOPTeam.SOPTeamType.Regular || team.TeamType == Sections.SOPTeam.SOPTeamType.External ||
                    team.TeamType == Sections.SOPTeam.SOPTeamType.Normal || team.TeamType == Sections.SOPTeam.SOPTeamType.Holiday)
                    team.IncludeChildTeams = includeChildTeams;

                if( arResult != null)
                {
                    arResult.Add(team);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool GetTeamName(WebDBManager dbMgr, 
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
            ArrayList arResult = new ArrayList();
            bool bResult = GetTeamName(dbMgr, ref arResult, ref strTeamNameList, strTeamList, nBeginIndex, nEndIndex,
                ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom);

            if( bResult == true && arResult.Count > 0)
            {
                if ((typeof(Sections.SectionDataProcess)).IsAssignableFrom(sectionData.GetType()))
                {
                    ((Sections.SectionDataProcess)sectionData).TeamList.AddRange(arResult);
                }
                if ((typeof(Sections.SectionDataInternal)).IsAssignableFrom(sectionData.GetType()))
                {
                    ((Sections.SectionDataInternal)sectionData).TeamList.AddRange(arResult);
                }
            }
            return bResult;
        }

        // TeamID(TeamType), ... 형태로 되어 있는 strTeamList를 분석하여 Team 이름들을 얻어온다.
        // ex) 1(0), 1(2), 2(3), 5(0)
        public static string GetTeamList(WebDBManager dbMgr, 
                                            string strTeamList, 
                                            ref Sections.SectionData sectionData, 
                                            ref Dictionary<int, string> dicNormal, 
                                            ref Dictionary<int, string> dicEmergency,
                                            ref Dictionary<int, string> dicUserDefined, 
                                            ref Dictionary<int, Sections.ExternalTeamData> dicExternal, 
                                            ref Dictionary<int, string> dicRegular,
                                            ref Dictionary<int, string> dicControlRoom)
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

        public static string GetSenderTeamList(WebDBManager dbMgr, 
                                            string strTeamList, 
                                            ref ArrayList arResult,
                                            ref Dictionary<int, string> dicNormal, 
                                            ref Dictionary<int, string> dicEmergency, 
                                            ref Dictionary<int, string> dicUserDefined, 
                                            ref Dictionary<int, Sections.ExternalTeamData> dicExternal, 
                                            ref Dictionary<int, string> dicRegular,
                                            ref Dictionary<int, string> dicControlRoom)
        {
            if (arResult == null)
                arResult = new ArrayList();

            int nBeginIndex = 0;
            int nLen = strTeamList.Length;

            string strTeamNameList = "";

            while (nBeginIndex < nLen)
            {
                int nDotIndex = strTeamList.IndexOf(',', nBeginIndex);
                if (nDotIndex < 0) break;

                if (!GetTeamName(dbMgr, ref arResult, ref strTeamNameList, strTeamList, nBeginIndex, nDotIndex, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom))
                    return "";

                nBeginIndex = nDotIndex + 1;
            }

            if (!GetTeamName(dbMgr, ref arResult, ref strTeamNameList, strTeamList, nBeginIndex, nLen, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom))
                return "";

            return strTeamNameList;
        }


        private bool LoadProcessMission(WebDBManager dbMgr, int nProcessID, ArrayList arrMissionItems)
        {
            string strSQL = string.Format("Select ID, missionText, TransmissionType, missionTarget, CommanderDisplayText,CommanderMemberType, CommanderMemberID from ProcessMission where ProcessID = {0}", nProcessID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strMissionText = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nTransmissionType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                string strTarget = WebDBManager.GetStringField(arrResult[i + 3], "");

                string strCommanderDisplayText = WebDBManager.GetStringField(arrResult[i + 4], "");
                int nCommanderMemberType = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                int nCommanderMemberID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);

                if (strCommanderDisplayText == null || strCommanderDisplayText == "null")
                    strCommanderDisplayText = "";

                Sections.MissionItem item = new Sections.MissionItem();

                item.TransmissionType = nTransmissionType;
                item.Mission = strMissionText;
                item.Target = strTarget;                
                //item.Transmission = nTransmission;
                Sections.SectionCommander commander = LoadCommanderTeamMember(dbMgr, nCommanderMemberType, nCommanderMemberID, strCommanderDisplayText);
                item.Commander = commander;

                arrMissionItems.Add(item);
            }

            return true;
        }

        private bool LoadProcess(WebDBManager dbMgr, 
            Dictionary<int, Sections.Section> dicSections, 
            ArrayList arrSections, 
            Sections.PanelSectionEx panel, 
            StepMemberDataEx data, 
            ref Dictionary<int, string> dicNormal, 
            ref Dictionary<int, string> dicEmergency, 
            ref Dictionary<int, string> dicUserDefined, 
            ref Dictionary<int, Sections.ExternalTeamData> dicExternal, 
            ref Dictionary<int, string> dicRegular,
            ref Dictionary<int, string> dicControlRoom)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT id, x, y, width, height, text, TeamList, ComponentID, ProcessTime, ");
            sb.Append(" ProcessTimeType, useProcessTime, useMissionMessage, onlyTeamLeader, ");
            sb.Append(" CommanderMemberType, CommanderMemberID, CommanderDisplayText, ");
            sb.Append(" valign, halign, FontName,FontStyle,FontSize,LineSpace,FontColor, AutoRun ");            
            sb.AppendFormat(" FROM Process where StepMemberID = {0}", data.StepMemberID.ToString());

            string szSQL = sb.ToString();
            ArrayList arrResult = dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                return false;

            string strProcessIDs = "";
            int nResultCount = arrResult.Count;
            
            for (int i = 0; i < nResultCount - 23; i+=24)
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
                int nCommanderMemberType = WebDBManager.GetIntField(arrResult[i + 13].ToString(), -2);
                int nCommanderMemberID = WebDBManager.GetIntField(arrResult[i + 14].ToString(), -2);
                string strCommanderDisplayText = WebDBManager.GetStringField(arrResult[i + 15], "");
                
                // 영흥 요구사항 (장진환)으로 추가됨 - skkim 2015-07-27
                // Default Valign - Center
                int nValign = WebDBManager.GetIntField(arrResult[i + 16].ToString(), 2);
                // Default Haling - Left
                int nHalign = WebDBManager.GetIntField(arrResult[i + 17].ToString(), 2);
                

                if (strCommanderDisplayText == "null")
                    strCommanderDisplayText = "";

                Sections.SectionProcess section = new Sections.SectionProcess(panel, x, y);

                string szFontName = WebDBManager.GetStringField(arrResult[i + 18], "");
                if (!(szFontName == null || szFontName == "" || szFontName == "null"))
                {
                    // Set Font
                    int fontStyle = WebDBManager.GetIntField(arrResult[i + 19].ToString(), 0);
                    float fontSize = WebDBManager.GetFloatField(arrResult[i + 20].ToString(), 0.0f);
                    float lineSpace = WebDBManager.GetFloatField(arrResult[i + 21].ToString(), 0.0f);
                    int fontColor = WebDBManager.GetIntField(arrResult[i + 22].ToString(), 0);

                    section.Data.LineSpace = lineSpace;

                    System.Drawing.Font font = new Font(szFontName, fontSize, (FontStyle)fontStyle);
                    section.TextFont = font;

                    Color color = Color.FromArgb(fontColor);

                    section.TextColor = color;
                }

                bool autoRun = WebDBManager.GetIntField(arrResult[i + 23].ToString(), 0) == 0 ? false : true;
               
                dicSections[nID] = section;
                arrSections.Add(section);
                panel.Sections.Add(section);
                
                section.RectSize = new SizeF(fWidth, fHeight);
                section.TextUP = strText;

                Sections.SectionData tempData = section.Data;
                section.TextDown = GetTeamList(dbMgr, strTeamList, ref tempData, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom);
                
                Sections.SectionDataProcess sectionData = (Sections.SectionDataProcess)tempData;               
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

                if (!LoadProcessMission(dbMgr, nID, sectionData.MissionItems))
                    return false;

                if (strProcessIDs.Length == 0)
                    strProcessIDs = nID.ToString();
                else
                    strProcessIDs += ", " + nID.ToString();

                Sections.SectionCommander commander = LoadCommanderTeamMember(dbMgr, nCommanderMemberType, nCommanderMemberID, strCommanderDisplayText);
                sectionData.Commander = commander;
                sectionData.AutoRun = autoRun;
            }
            return true;
        }

        private Sections.SectionCommander LoadCommanderTeamMember(WebDBManager dbMgr, int nTeamType, int nMemberID, string strDisplayText)
        {
            string strSQL = "";
            Sections.SectionCommander commander = null;

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
                        nMemberID, m_nSiteID);
                }
                else if (nTeamType == (int)Sections.SOPTeam.SOPTeamType.Holiday)
                {
                    strSQL = string.Format("Select ID, TeamName from TemporaryEmergencyTeam where ID in ({0}) and SiteID = {1}",
                        nMemberID, m_nSiteID);
                }
                else if (nTeamType == (int)Sections.SOPTeam.SOPTeamType.External)
                {
                    //if(nMemberID < m_nSiteID * 1000)
                    //{
                    //    strSQL = string.Format("Select ID, TeamName from ExternalCompanyTeam where ID in ({0})",
                    //    nMemberID, m_nSiteID);
                    //}
                    //else
                    {
                        strSQL = string.Format("Select ID, TeamName from ExternalTeam where ID in ({0})",
                        nMemberID, m_nSiteID);
                    }                    
                }
                else if (nTeamType == (int)Sections.SOPTeam.SOPTeamType.UserDefined)
                {
                    strSQL = string.Format("Select ID, TeamName from UserDefinedTeam where ID in ({0})",
                        nMemberID, m_nSiteID);
                }
                else if (nTeamType == (int)Sections.SOPTeam.SOPTeamType.Regular)
                {
                    strSQL = string.Format("Select ID, TeamName from RegularTeam where ID in ({0})",
                        nMemberID, m_nSiteID);
                } 
                else
                    return null;

                ArrayList arrResult = dbMgr.GetResultData(strSQL);

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
                foreach (Data_ControlRoom item in FormMain.Instance.ControlRoom)
                {
                    if (item.ID == nMemberID)
                    {
                        commander = new Sections.SectionCommander();

                        commander.Team = new Sections.SOPTeam();
                        commander.Team.TeamType = (Sections.SOPTeam.SOPTeamType)nTeamType;
                        commander.Team.TeamID = item.ID;
                        commander.Team.TeamName = item.TeamName;
                        commander.IsTeamMember = false;
                        commander.TeamMemberID = -1;

                        if (strDisplayText.Length > 0)
                            commander.DisplayText = strDisplayText;
                        break;
                    }
                }
            }
            else
            {
                if (nTeamType == 5 || nTeamType == 6)
                {
                    strSQL = string.Format("Select ID, MemberName from TemporaryMemberList where ID in ({0})", nMemberID);
                }
                else if (nTeamType == 7)
                {
                    strSQL = string.Format("Select ID, Name from ExternalCompanyMember where ID in ({0})", nMemberID);
                }
                else if (nTeamType == 8)
                {
                    strSQL = string.Format("Select ID, MemberName from CompanyMember where ID in ({0})", nMemberID);
                } 
                else
                    return null;

                ArrayList arrResult = dbMgr.GetResultData(strSQL);

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

        private bool LoadDecision(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberDataEx data)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID, valign, halign "+
                ",FontName, FontStyle, FontSize, LineSpace, FontColor, autoRunScript, autoRunScriptVariableTypes " +
                " from Decision where StepMemberID = " + data.StepMemberID.ToString();

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
                string strComponentID = WebDBManager.GetStringField(arrResult[i + 6], "");

                // 영흥 요구사항 (장진환)으로 추가됨 - skkim 2015-07-27
                // Default Valign - Center
                int nValign = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 2);
                // Default Haling - Left
                int nHalign = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 2);

                Sections.SectionDecision section = new Sections.SectionDecision(panel, x, y);
                Sections.SectionDataDecision sectionData = (Sections.SectionDataDecision)section.Data;


                string szFontName = WebDBManager.GetStringField(arrResult[i + 9], "");
                if (!(szFontName == null || szFontName == "" || szFontName == "null"))
                {
                    // Set Font
                    int fontStyle = WebDBManager.GetIntField(arrResult[i + 10].ToString(), 0);
                    float fontSize = WebDBManager.GetFloatField(arrResult[i + 11].ToString(), 0.0f);
                    float lineSpace = WebDBManager.GetFloatField(arrResult[i + 12].ToString(), 0.0f);
                    int fontColor = WebDBManager.GetIntField(arrResult[i + 13].ToString(), 0);

                    section.Data.LineSpace = lineSpace;

                    System.Drawing.Font font = new Font(szFontName, fontSize, (FontStyle)fontStyle);
                    section.TextFont = font;

                    Color color = Color.FromArgb(fontColor);

                    section.TextColor = color;
                }

                string strAutoRunScript = WebDBManager.GetStringField(arrResult[i + 14]);
                string strVariableTypes = WebDBManager.GetStringField(arrResult[i + 15]);
                
                dicSections[nID] = section;
                arrSections.Add(section);
                panel.Sections.Add(section);

                section.RectSize = new SizeF(fWidth, fHeight);
                section.Title = strText;

                sectionData.ID = nID;
                sectionData.Title = strText;
                sectionData.ComponentID = strComponentID;

                sectionData.TextVerticalAlign = (Sections.SectionData.TextVAlign)nValign;
                sectionData.TextHorizontalAlign = (Sections.SectionData.TextHAlign)nHalign;

                if (strAutoRunScript != null)
                {
                    sectionData.Expression = strAutoRunScript;

                    if (strVariableTypes != null)
                    {
                        PropertiesDecision.SetVariableTypes(sectionData, strVariableTypes);
                    }
                }
            }
            return true;
        }

        private bool LoadAnnotation(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberDataEx data)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID, valign, halign " +
                            ",FontName, FontStyle, FontSize, LineSpace, FontColor " +
                            " from Annotation where StepMemberID = ";
                
                
            strSQL += data.StepMemberID.ToString();
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

                // 영흥 요구사항 (장진환)으로 추가됨 - skkim 2015-07-27
                // Default Valign - Center
                int nValign = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 2);
                // Default Haling - Left
                int nHalign = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 2);


                Sections.SectionAnnotation section = new Sections.SectionAnnotation(panel, x, y);
                Sections.SectionDataAnnotation sectionData = (Sections.SectionDataAnnotation)section.Data;
                
                string szFontName = WebDBManager.GetStringField(arrResult[i + 9], "");
                if(!(szFontName == null || szFontName == "" || szFontName == "null"))
                {
                    // Set Font
                    int fontStyle = WebDBManager.GetIntField(arrResult[i + 10].ToString(), 0);
                    float fontSize = WebDBManager.GetFloatField(arrResult[i + 11].ToString(), 0.0f);
                    float lineSpace = WebDBManager.GetFloatField(arrResult[i + 12].ToString(), 0.0f);
                    int fontColor = WebDBManager.GetIntField(arrResult[i + 13].ToString(), 0);

                    sectionData.LineSpace = lineSpace;

                    System.Drawing.Font font = new Font(szFontName, fontSize, (FontStyle)fontStyle);
                    section.TextFont = font;

                    Color color = Color.FromArgb(fontColor);
                    
                    section.TextColor = color;
                }

                
               
                dicSections[nID] = section;
                arrSections.Add(section);
                panel.Sections.Add(section);

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

        private bool LoadEndPoint(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberDataEx data)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID, isBegin, valign, halign " +
                ",FontName, FontStyle, FontSize, LineSpace, FontColor " +
                " from EndPoint where StepMemberID = " + data.StepMemberID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

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
                bool isBegin = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0) == 0 ? false : true;

                // 영흥 요구사항 (장진환)으로 추가됨 - skkim 2015-07-27
                // Default Valign - Center
                int nValign = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 2);
                // Default Haling - Left
                int nHalign = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 2);

                Sections.SectionEndPoint section = new Sections.SectionEndPoint(panel, x, y);
                Sections.SectionDataEndPoint sectionData = (Sections.SectionDataEndPoint)section.Data;
                
                string szFontName = WebDBManager.GetStringField(arrResult[i + 10], "");
                if (!(szFontName == null || szFontName == "" || szFontName == "null"))
                {
                    // Set Font
                    int fontStyle = WebDBManager.GetIntField(arrResult[i + 11].ToString(), 0);
                    float fontSize = WebDBManager.GetFloatField(arrResult[i + 12].ToString(), 0.0f);
                    float lineSpace = WebDBManager.GetFloatField(arrResult[i + 13].ToString(), 0.0f);
                    int fontColor = WebDBManager.GetIntField(arrResult[i + 14].ToString(), 0);

                    sectionData.LineSpace = lineSpace;
                    System.Drawing.Font font = new Font(szFontName, fontSize, (FontStyle)fontStyle);
                    section.TextFont = font;
                    Color color = Color.FromArgb(fontColor);
                    section.TextColor = color;
                }

                dicSections[nID] = section;
                arrSections.Add(section);
                panel.Sections.Add(section);

                section.RectSize = new SizeF(fWidth, fHeight);
                section.Title = strText;

                sectionData.ID = nID;
                sectionData.Title = strText;
                sectionData.ComponentID = strComponentID;
                sectionData.IsBegin = isBegin;

                sectionData.TextVerticalAlign = (Sections.SectionData.TextVAlign)nValign;
                sectionData.TextHorizontalAlign = (Sections.SectionData.TextHAlign)nHalign;
            }

            return true;
        }

        // arrLink : Link 객체는 다른 Panel의 객체와 연결되어야 하므로, DB로부터 모든 객체를 읽어들인 후에 해당 객체와 연결시킨다.
        private bool LoadLink(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, ArrayList arrLink, Sections.PanelSectionEx panel, StepMemberDataEx data)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID, LinkedComponentID, valign, halign " +
                ",FontName, FontStyle, FontSize, LineSpace, FontColor " +
                " from Link where StepMemberID = " + data.StepMemberID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

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
				string strLinkedComponentID = WebDBManager.GetStringField(arrResult[i + 7], "");

                // 영흥 요구사항 (장진환)으로 추가됨 - skkim 2015-07-27
                // Default Valign - Center
                int nValign = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 2);
                // Default Haling - Left
                int nHalign = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 2);

                Sections.SectionLink section = new Sections.SectionLink(panel, x, y);
                Sections.SectionDataLink sectionData = (Sections.SectionDataLink)section.Data;

                string szFontName = WebDBManager.GetStringField(arrResult[i + 10], "");
                if (!(szFontName == null || szFontName == "" || szFontName == "null"))
                {
                    // Set Font
                    int fontStyle = WebDBManager.GetIntField(arrResult[i + 11].ToString(), 0);
                    float fontSize = WebDBManager.GetFloatField(arrResult[i + 12].ToString(), 0.0f);
                    float lineSpace = WebDBManager.GetFloatField(arrResult[i + 13].ToString(), 0.0f);
                    int fontColor = WebDBManager.GetIntField(arrResult[i + 14].ToString(), 0);

                    sectionData.LineSpace = lineSpace;
                    System.Drawing.Font font = new Font(szFontName, fontSize, (FontStyle)fontStyle);
                    section.TextFont = font;
                    Color color = Color.FromArgb(fontColor);
                    section.TextColor = color;
                }

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

                sectionData.TextVerticalAlign = (Sections.SectionData.TextVAlign)nValign;
                sectionData.TextHorizontalAlign = (Sections.SectionData.TextHAlign)nHalign;
            }

            return true;
        }

        private bool LoadTransSOP(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberDataEx data)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID, LinkedActionStepID, Description, valign, halign "+
                ",FontName, FontStyle, FontSize, LineSpace, FontColor " +
                " from TransSOP where StepMemberID = " + data.StepMemberID.ToString();
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
				string strComponentID = WebDBManager.GetStringField(arrResult[i + 6], "");
				int nLinkedActionStepID = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
				string strDescription = WebDBManager.GetStringField(arrResult[i + 8], "");

                // 영흥 요구사항 (장진환)으로 추가됨 - skkim 2015-07-27
                // Default Valign - Center
                int nValign = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 2);
                // Default Haling - Left
                int nHalign = WebDBManager.GetIntField(arrResult[i + 10].ToString(), 2);


                Sections.SectionTransSOP section = new Sections.SectionTransSOP(panel, x, y);
                Sections.SectionDataTransSOP sectionData = (Sections.SectionDataTransSOP)section.Data;

                string szFontName = WebDBManager.GetStringField(arrResult[i + 10], "");
                if (!(szFontName == null || szFontName == "" || szFontName == "null"))
                {
                    // Set Font
                    int fontStyle = WebDBManager.GetIntField(arrResult[i + 11].ToString(), 0);
                    float fontSize = WebDBManager.GetFloatField(arrResult[i + 12].ToString(), 0.0f);
                    float lineSpace = WebDBManager.GetFloatField(arrResult[i + 13].ToString(), 0.0f);
                    int fontColor = WebDBManager.GetIntField(arrResult[i + 14].ToString(), 0);

                    sectionData.LineSpace = lineSpace;
                    System.Drawing.Font font = new Font(szFontName, fontSize, (FontStyle)fontStyle);
                    section.TextFont = font;
                    Color color = Color.FromArgb(fontColor);
                    section.TextColor = color;
                }

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

                sectionData.TextVerticalAlign = (Sections.SectionData.TextVAlign)nValign;
                sectionData.TextHorizontalAlign = (Sections.SectionData.TextHAlign)nHalign;
            }
            return true;
        }
               
        private bool LoadInternal(WebDBManager dbMgr, 
            Dictionary<int, Sections.Section> dicSections, 
            ArrayList arrSections,
            Sections.PanelSectionEx panel, 
            StepMemberDataEx data,            
            ref Dictionary<int, string> dicNormal, 
            ref Dictionary<int, string> dicEmergency, 
            ref Dictionary<int, string> dicUserDefined, 
            ref Dictionary<int, Sections.ExternalTeamData> dicExternal,
            ref Dictionary<int, string> dicRegular,
            ref Dictionary<int, string> dicControlRoom)
        { 
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT id, x, y, width, height, text, ComponentID, usePopupMessage, useMobileApp, useBroadcast, BroadcastMessage ");
            sb.Append(" ,TeamList, onlyTeamLeader, CommanderMemberType, CommanderMemberID, CommanderDisplayText, valign, halign ");
            sb.Append(" ,FontName, FontStyle, FontSize, LineSpace, FontColor, AutoRun ");
            sb.AppendFormat(" FROM InternalTransmission WHERE StepMemberID = {0}", data.StepMemberID.ToString());
            
            string szSQL = sb.ToString();
            ArrayList arrResult = dbMgr.GetResultData(szSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 23; i += 24)
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
                string szTeamList = WebDBManager.GetStringField(arrResult[i + 11], "");
               
                bool bOnlyTeamLeader = WebDBManager.GetIntField(arrResult[i + 12].ToString(), 0) == 0 ? false : true;

                int nCommanderMemberType = WebDBManager.GetIntField(arrResult[i + 13].ToString(), -2);
                int nCommanderMemberID = WebDBManager.GetIntField(arrResult[i + 14].ToString(), -2);
                string szCommanderDisplayText = WebDBManager.GetStringField(arrResult[i + 15], "");

                if (szCommanderDisplayText == null || szCommanderDisplayText == "null")
                    szCommanderDisplayText = "";

                // 영흥 요구사항 (장진환)으로 추가됨 - skkim 2015-07-27
                // Default Valign - Center
                int nValign = WebDBManager.GetIntField(arrResult[i + 16].ToString(), 2);
                // Default Haling - Left
                int nHalign = WebDBManager.GetIntField(arrResult[i + 17].ToString(), 2);

                Sections.SectionInternal section = new Sections.SectionInternal(panel, x, y);
                Sections.SectionData tempData = section.Data;
                
                string szFontName = WebDBManager.GetStringField(arrResult[i + 18], "");
                if (!(szFontName == null || szFontName == "" || szFontName == "null"))
                {
                    // Set Font
                    int fontStyle = WebDBManager.GetIntField(arrResult[i + 19].ToString(), 0);
                    float fontSize = WebDBManager.GetFloatField(arrResult[i + 20].ToString(), 0.0f);
                    float lineSpace = WebDBManager.GetFloatField(arrResult[i + 21].ToString(), 0.0f);
                    int fontColor = WebDBManager.GetIntField(arrResult[i + 22].ToString(), 0);

                    tempData.LineSpace = lineSpace;
                    System.Drawing.Font font = new Font(szFontName, fontSize, (FontStyle)fontStyle);
                    section.TextFont = font;
                    Color color = Color.FromArgb(fontColor);
                    section.TextColor = color;
                }

                bool autoRun = WebDBManager.GetIntField(arrResult[i + 23].ToString(), 0) == 0 ? false : true;
                
                if( szTeamList != null && szTeamList != "" && szTeamList != "null")
                {
                    GetTeamList(dbMgr, szTeamList, ref tempData, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom);
                }
                
                Sections.SectionDataInternal sectionData = (Sections.SectionDataInternal)tempData;
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
                sectionData.TransferTeamLeaderOnly = bOnlyTeamLeader;

                sectionData.TextVerticalAlign = (Sections.SectionData.TextVAlign)nValign;
                sectionData.TextHorizontalAlign = (Sections.SectionData.TextHAlign)nHalign;

                Sections.SectionCommander commander = LoadCommanderTeamMember(dbMgr, nCommanderMemberType, nCommanderMemberID, szCommanderDisplayText);
                sectionData.Commander = commander;
                sectionData.AutoRun = autoRun;
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

        private bool LoadExternal(WebDBManager dbMgr, 
            Dictionary<int, Sections.Section> dicSections, 
            ArrayList arrSections, 
            Sections.PanelSectionEx panel, 
            StepMemberDataEx data, 
            Dictionary<int, Sections.ExternalTeamData> dicExternal)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select id, x, y, width, height, text, ComponentID, useSMS, SMSText, ");
            sb.Append(" SMSExternalTeamIDList, useEFax, FaxExternalTeamIDList, valign, halign ");
            sb.Append(" ,FontName, FontStyle, FontSize, LineSpace, FontColor ");
            sb.AppendFormat("FROM ExternalTransmission where StepMemberID = {0}", data.StepMemberID.ToString());

            string strSQL = sb.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

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
				bool useSMS = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0) == 0 ? false : true;
				string strSMSText = WebDBManager.GetStringField(arrResult[i + 8], "");
				string strSMSExternalTeamIDList = WebDBManager.GetStringField(arrResult[i + 9], "");
				bool useEFax = WebDBManager.GetIntField(arrResult[i + 10].ToString(), 0) == 0 ? false : true;
				string strFaxExternalTeamIDList = WebDBManager.GetStringField(arrResult[i + 11], "");


                // 영흥 요구사항 (장진환)으로 추가됨 - skkim 2015-07-27
                // Default Valign - Center
                int nValign = WebDBManager.GetIntField(arrResult[i + 12].ToString(), 2);
                // Default Haling - Left
                int nHalign = WebDBManager.GetIntField(arrResult[i + 13].ToString(), 2);

                Sections.SectionExternal section = new Sections.SectionExternal(panel, x, y);
                Sections.SectionDataExternal sectionData = (Sections.SectionDataExternal)section.Data;

                string szFontName = WebDBManager.GetStringField(arrResult[i + 14], "");
                if (!(szFontName == null || szFontName == "" || szFontName == "null"))
                {
                    // Set Font
                    int fontStyle = WebDBManager.GetIntField(arrResult[i + 15].ToString(), 0);
                    float fontSize = WebDBManager.GetFloatField(arrResult[i + 16].ToString(), 0.0f);
                    float lineSpace = WebDBManager.GetFloatField(arrResult[i + 17].ToString(), 0.0f);
                    int fontColor = WebDBManager.GetIntField(arrResult[i + 18].ToString(), 0);

                    sectionData.LineSpace = lineSpace;
                    System.Drawing.Font font = new Font(szFontName, fontSize, (FontStyle)fontStyle);
                    section.TextFont = font;
                    Color color = Color.FromArgb(fontColor);
                    section.TextColor = color;
                }

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

                sectionData.TextVerticalAlign = (Sections.SectionData.TextVAlign)nValign;
                sectionData.TextHorizontalAlign = (Sections.SectionData.TextHAlign)nHalign;

                if (!GetExternalTeamList(strSMSExternalTeamIDList, sectionData.SMSReceivers, dicExternal))
                    return false;
                if (!GetExternalTeamList(strFaxExternalTeamIDList, sectionData.FaxReceivers, dicExternal))
                    return false;
            }
            return true;
        }

        private bool LoadTransmission(WebDBManager dbMgr, 
            Dictionary<int, Sections.Section> dicSections, 
            ArrayList arrSections, 
            Sections.PanelSectionEx panel, 
            StepMemberDataEx data, 
            Dictionary<int, Sections.ExternalTeamData> dicExternal)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select id, x, y, width, height, text, ComponentID, useInternalPopupMessage, ");
            sb.Append(" useInternalMobileApp, useInternalBroadcast, useExternalSMS, externalSMSText, ");
            sb.Append(" SMSExternalTeamIDList, useExternalFax, FaxExternalTeamIDList, InternalBroadcastMessage, valign, halign ");
            sb.Append(" ,FontName, FontStyle, FontSize, LineSpace, FontColor ");
            sb.AppendFormat(" from Transmission where StepMemberID = {0}", data.StepMemberID.ToString());
            
            string strSQL = sb.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 22; i += 23)
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
                
                // 영흥 요구사항 (장진환)으로 추가됨 - skkim 2015-07-27
                // Default Valign - Center
                int nValign = WebDBManager.GetIntField(arrResult[i + 16].ToString(), 2);
                // Default Haling - Left
                int nHalign = WebDBManager.GetIntField(arrResult[i + 17].ToString(), 2);

                Sections.SectionTransmission section = new Sections.SectionTransmission(panel, x, y);
                Sections.SectionDataTransmission sectionData = (Sections.SectionDataTransmission)section.Data;

                string szFontName = WebDBManager.GetStringField(arrResult[i + 18], "");
                if (!(szFontName == null || szFontName == "" || szFontName == "null"))
                {
                    // Set Font
                    int fontStyle = WebDBManager.GetIntField(arrResult[i + 19].ToString(), 0);
                    float fontSize = WebDBManager.GetFloatField(arrResult[i + 20].ToString(), 0.0f);
                    float lineSpace = WebDBManager.GetFloatField(arrResult[i + 21].ToString(), 0.0f);
                    int fontColor = WebDBManager.GetIntField(arrResult[i + 22].ToString(), 0);

                    sectionData.LineSpace = lineSpace;
                    System.Drawing.Font font = new Font(szFontName, fontSize, (FontStyle)fontStyle);
                    section.TextFont = font;
                    Color color = Color.FromArgb(fontColor);
                    section.TextColor = color;
                }


                dicSections[nID] = section;
                arrSections.Add(section);
                panel.Sections.Add(section);

                section.RectSize = new SizeF(fWidth, fHeight);
                section.Title = strText;

                sectionData.ID = nID;
                sectionData.Title = strText;
                sectionData.ComponentID = strComponentID;

                sectionData.TextVerticalAlign = (Sections.SectionData.TextVAlign)nValign;
                sectionData.TextHorizontalAlign = (Sections.SectionData.TextHAlign)nHalign;

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

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ID, x, y, width, height, text, ComponentID, RegionX, RegionY, RegionWidth, RegionHeight ");
            sb.Append(" RegionWidth, RegionHeight, valign, halign ");
            sb.Append(" ,FontName, FontStyle, FontSize, LineSpace, FontColor ");
            sb.AppendFormat(" FROM SectionGroup where StepMemberID = {0}", data.StepMemberID.ToString());

            string szSQL = sb.ToString();           
			ArrayList arrResult = dbMgr.GetResultData(szSQL);

			if (arrResult == null)
				return true;

			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 17; i += 18)
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
                
                // 영흥 요구사항 (장진환)으로 추가됨 - skkim 2015-07-27
                // Default Valign - Center
                int nValign = WebDBManager.GetIntField(arrResult[i + 11].ToString(), 2);
                // Default Haling - Left
                int nHalign = WebDBManager.GetIntField(arrResult[i + 12].ToString(), 2);

				Sections.SectionGroup section = new Sections.SectionGroup(panel, x, y);
				Sections.SectionDataGroup sectionData = (Sections.SectionDataGroup)section.Data;
				dicSections[nID] = section;

                string szFontName = WebDBManager.GetStringField(arrResult[i + 13], "");
                if (!(szFontName == null || szFontName == "" || szFontName == "null"))
                {
                    // Set Font
                    int fontStyle = WebDBManager.GetIntField(arrResult[i + 14].ToString(), 0);
                    float fontSize = WebDBManager.GetFloatField(arrResult[i + 15].ToString(), 0.0f);
                    float lineSpace = WebDBManager.GetFloatField(arrResult[i + 16].ToString(), 0.0f);
                    int fontColor = WebDBManager.GetIntField(arrResult[i + 17].ToString(), 0);

                    sectionData.LineSpace = lineSpace;
                    System.Drawing.Font font = new Font(szFontName, fontSize, (FontStyle)fontStyle);
                    section.TextFont = font;
                    Color color = Color.FromArgb(fontColor);
                    section.TextColor = color;
                }

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

                sectionData.TextVerticalAlign = (Sections.SectionData.TextVAlign)nValign;
                sectionData.TextHorizontalAlign = (Sections.SectionData.TextHAlign)nHalign;

				string szSql2 = "SELECT CID, type, ComponentID FROM GroupComponent WHERE GroupID = " + nID.ToString();
				ArrayList arrResultComp = dbMgr.GetResultData(szSql2);

				if (arrResultComp == null)
					return false;

				int nResultCountComp = arrResultComp.Count;
				for (int j = 0; j < nResultCountComp - 2; j += 3)
				{
					int nCompID = WebDBManager.GetIntField(arrResultComp[j].ToString(), 0);
					int nCompType = WebDBManager.GetIntField(arrResultComp[j + 1].ToString(), 0);
					string szCompID = WebDBManager.GetStringField(arrResultComp[j + 2], "");

					Dictionary<int, Sections.Section> dicCompSection = 
                        GetSectionDictionary(nCompType, 
                                            dicProcessSections, 
                                            dicDecisionSections, 
                                            dicAnnotationSections, 
                                            dicEndPointSections, 
                                            dicLinkSections, 
                                            dicTransSOPSections, 
                                            dicInternalSections, 
                                            dicExternalSections, 
                                            dicTransmissionSections, 
                                            dicSections);

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

		private Dictionary<int, Sections.Section> GetSectionDictionary(int nSectionType, 
            Dictionary<int, Sections.Section> dicProcessSections, 
            Dictionary<int, Sections.Section> dicDecisionSections, 
            Dictionary<int, Sections.Section> dicAnnotationSections,
            Dictionary<int, Sections.Section> dicEndPointSections,
            Dictionary<int, Sections.Section> dicLinkSections, 
            Dictionary<int, Sections.Section> dicTransSOPSections, 
            Dictionary<int, Sections.Section> dicInternalSections, 
            Dictionary<int, Sections.Section> dicExternalSections,
            Dictionary<int, Sections.Section> dicTransmissionSections,
            Dictionary<int, Sections.Section> dicGroupSections)
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

		private bool LoadArrow(WebDBManager dbMgr, 
            Dictionary<int, Sections.Section> dicProcessSections, 
            Dictionary<int, Sections.Section> dicDecisionSections, 
            Dictionary<int, Sections.Section> dicAnnotationSections, 
            Dictionary<int, Sections.Section> dicEndPointSections, 
            Dictionary<int, Sections.Section> dicLinkSections, 
            Dictionary<int, Sections.Section> dicTransSOPSections, 
            Dictionary<int, Sections.Section> dicInternalSections, 
            Dictionary<int, Sections.Section> dicExternalSections, 
            Dictionary<int, Sections.Section> dicTransmissionSections, 
            Dictionary<int, Sections.Section> dicGroupSections, 
            StepMemberDataEx data)
        {
            string strSQL = "select ID, Text, BeginComponentID, BeginComponentPosition, EndComponentID, EndComponentPosition ";
            strSQL += "from Arrow where StepMemberID = " + data.StepMemberID.ToString();

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

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
                Dictionary<int, Sections.Section> dicBeginSection = 
                    GetSectionDictionary(nBeginType, 
                                        dicProcessSections, 
                                        dicDecisionSections, 
                                        dicAnnotationSections, 
                                        dicEndPointSections, 
                                        dicLinkSections, 
                                        dicTransSOPSections, 
                                        dicInternalSections, 
                                        dicExternalSections, 
                                        dicTransmissionSections, 
                                        dicGroupSections);

                // nBeginType, 즉 nBeginComponentID가 잘못 입력된 경우
                if (dicBeginSection == null)
                    return false;

                int nEndType = nEndComponentID >> 24;
                nEndComponentID = nEndComponentID & 0xffffff;
				Dictionary<int, Sections.Section> dicEndSection = GetSectionDictionary(
                                        nEndType, 
                                        dicProcessSections, 
                                        dicDecisionSections, 
                                        dicAnnotationSections,
                                        dicEndPointSections,
                                        dicLinkSections,
                                        dicTransSOPSections, 
                                        dicInternalSections,
                                        dicExternalSections, 
                                        dicTransmissionSections, 
                                        dicGroupSections);

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

        private bool LoadPanelComponent(WebDBManager dbMgr, 
            Sections.PanelSectionEx panel, 
            StepMemberDataEx data, 
            ArrayList arrLink, 
            ArrayList arrSections, 
            ref Dictionary<int, string> dicNormal, 
            ref Dictionary<int, string> dicEmergency, 
            ref Dictionary<int, string> dicUserDefined, 
            ref Dictionary<int, Sections.ExternalTeamData> dicExternal, 
            ref Dictionary<int, string> dicRegular,
            ref Dictionary<int, string> dicControlRoom)
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
            //if (!LoadInternal(dbMgr, dicInternalSections, arrSections, panel, data))
            //    return false;

            if (!LoadInternal(dbMgr, dicInternalSections, arrSections, panel, data, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom))
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
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
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
                Sections.SOPTeam.SOPTeamType nTeamType = (Sections.SOPTeam.SOPTeamType)WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
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
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

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
            Dictionary<int, string> dicControlRoom = null;

            FormPanel.BarConfig barConfig = pageLevel.GetBarConfig();
            ActionStepInfo currentActionStep = null;

            foreach (ActionStepInfo actionStep in arrActionSteps)
            {
                if (actionStep.ParentStepID > 0)
                {
                    ActionStepTabPage pageCurrent = (ActionStepTabPage)dicActionStep[actionStep.ActionStepID];

                    if (dicActionStep.ContainsKey(actionStep.ParentStepID))
                    {
                        TabPage pageParent = dicActionStep[actionStep.ParentStepID];
                        // 부모 단계가 존재할 경우 Tag에 부모 단계를 넣는다.
                        pageCurrent.Tag = pageParent;                        
						
						//FormLevelProperties form = FormMain.Instance.GetPageLevel().GetPropertiesLevel();
						//form.LevelProperties.SetData(pageCurrent.Data);
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

                    if (!LoadPanelComponent(dbMgr, panel, data, arrLink, arrSections, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom))
                        return false;
                }

                if (!SetLinkSections(arrLink, arrSections))
                    return false;

                currentActionStep = actionStep;
            }

            if (currentActionStep != null)
                barConfig.SetConfig(currentActionStep.UserDefinedConfig);

            return true;
        }

        private int FindStepMemberTeamIndex(int nTeamID, Sections.SOPTeam.SOPTeamType nTeamType, ArrayList arrTeams)
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

        private void GetStepMemberTeamName(ArrayList arrStepMembers, string strTableName, Sections.SOPTeam.SOPTeamType nTeamType, ArrayList arrTeams, WebDBManager dbMgr)
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
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

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

		private ArrayList LoadBarPage(FormPageSOP pageLevel, ArrayList arrActionSteps, WebDBManager dbMgr)
        {
            if (arrActionSteps == null || arrActionSteps.Count == 0)
                return null;

            ActionStepInfo actionStep = (ActionStepInfo)arrActionSteps[0];
            string strSQL = string.Format("Select ID, TeamID, TeamType from StepMember where ActionStepID = {0}", actionStep.ActionStepID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);
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
                Sections.SOPTeam.SOPTeamType nTeamType = (Sections.SOPTeam.SOPTeamType)WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);

                StepMemberData data = new StepMemberData("", nTeamID, nTeamType);
                arrTeams.Add(data);

                if (nTeamType == Sections.SOPTeam.SOPTeamType.Normal)
                    arrNormal.Add(data);    // 평일 비상 조직
                else if (nTeamType == Sections.SOPTeam.SOPTeamType.Holiday)
                    arrEmergency.Add(data); // 야간 및 휴일 비상 조직
                else if (nTeamType == Sections.SOPTeam.SOPTeamType.External)
                    arrExternal.Add(data);  // 외부 조직
                else if (nTeamType == Sections.SOPTeam.SOPTeamType.UserDefined)
                    arrUserDefined.Add(data);   // 사용자 정의 조직
                else if (nTeamType == Sections.SOPTeam.SOPTeamType.Regular)    // 정규 조직
                    arrRegular.Add(data);
            }

            GetStepMemberTeamName(arrNormal, "TemporaryNormalTeam", Sections.SOPTeam.SOPTeamType.Normal, arrTeams, dbMgr);
            GetStepMemberTeamName(arrEmergency, "TemporaryEmergencyTeam", Sections.SOPTeam.SOPTeamType.Holiday, arrTeams, dbMgr);
            GetStepMemberTeamName(arrExternal, "ExternalTeam", Sections.SOPTeam.SOPTeamType.External, arrTeams, dbMgr);
            GetStepMemberTeamName(arrUserDefined, "UserDefinedTeam", Sections.SOPTeam.SOPTeamType.UserDefined, arrTeams, dbMgr);
            GetStepMemberTeamName(arrRegular, "RegularTeam", Sections.SOPTeam.SOPTeamType.Regular, arrTeams, dbMgr);

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
       
        // 수식에서 사용된 변수가 SOP에서 사용하는 Config와 동일한지 여부를 판단
        // strUserDefinedConfigName : SOP에서 사용하는 Config 이름
        public static bool CheckDecisionExpression(FormMain frm, out string strUserDefinedConfigName)
        {
            FormPageSOP pageLevel = frm.GetPageLevel();
            FormPanel.BarConfig barConfig = pageLevel.GetBarConfig();

            BarLevelTree tree = frm.GetPageLevel().GetBarLevelTree();
            List<SOPParameter> systemParameters = PopupSpecialMessage.GetSystemParameters(tree.GetCurrentCategoryName(), tree.GetCurrentSubCategoryName());

            ConfigData config = null;
            List<SOPParameter> parameters = barConfig.GetCurrentVariables(out config);

            if (config == null)
                strUserDefinedConfigName = "";
            else
                strUserDefinedConfigName = config.Text;

            if (parameters == null)
                parameters = systemParameters;
            else if (systemParameters != null)
                parameters.AddRange(systemParameters);

            Dictionary<string, Sections.SectionDataDecision.VariableType> dicVariableTypes = ParametersToVariableTypes(parameters);
            Sections.SectionDataDecision.VariableType type;

            foreach (ActionStepTabPage page in pageLevel.TabControls.TabPages)
            {
                foreach (System.Windows.Forms.Control control in page.Controls)
                {
                    if (control is Sections.PanelSectionEx)
                    {
                        Sections.PanelSectionEx panel = (Sections.PanelSectionEx)control;

                        foreach (Sections.Section section in panel.Sections)
                        {
                            if (section is Sections.SectionDecision)
                            {
                                Sections.SectionDataDecision data = (Sections.SectionDataDecision)section.Data;

                                if (data.Expression.Length > 0)
                                {
                                    foreach (KeyValuePair<string, Sections.SectionDataDecision.VariableType> pair in data.VariableTypes)
                                    {
                                        if (dicVariableTypes.TryGetValue(pair.Key.ToLower(), out type) == false || type != pair.Value)
                                        {
                                            page.Select();
                                            panel.SelectSection(section);
                                            panel.ShowEdit(section);
                                            MessageBox.Show(pair.Key + "는 정의되지 않은 변수입니다.\r\n현재 선택되어진 사용자 정의 설정을 살펴보시기 바랍니다.");
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        private static Dictionary<string, Sections.SectionDataDecision.VariableType> ParametersToVariableTypes(List<SOPParameter> parameters)
        {
            Dictionary<string, Sections.SectionDataDecision.VariableType> dicVariableTypes = new Dictionary<string, Sections.SectionDataDecision.VariableType>();

            if (parameters == null)
                return dicVariableTypes;

            Sections.SectionDataDecision.VariableType type;

            foreach (SOPParameter param in parameters)
            {
                string strVariableLower = "{" + param.VariableName.ToLower() + "}";

                if (dicVariableTypes.TryGetValue(strVariableLower, out type) == false)
                    dicVariableTypes[strVariableLower] = param.Type;
            }

            return dicVariableTypes;
        }

        // nVersionID의 버전을 신규 ID로 바꾼다.
        // 신규 ID를 리턴한다.
        private int ChangeSOPVersion(WebDBManager dbMgr, int nSOPGenUserID, int nVersionID, int nNewID)
        {
            UnE.SOP.RollbackManager rollback = new UnE.SOP.RollbackManager();
            //if (dbMgr.BeginBatch() == false)
            //    return -1;

            string strSQL = "Select max(ID) from Version";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            //ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
            {
                //dbMgr.BatchRollback();
                return -1;
            }

            // 기존 버전이 있으므로 Count가 0일수 없다.
            if (arrResult.Count == 0)
            {
                //dbMgr.BatchRollback();
                return -1;
            }

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            if (id == null)
            {
                //dbMgr.BatchRollback();
                return -1;
            }

            int nTempID = id.Data + 1;

            if (nNewID < 0)
            {
                nNewID = nTempID;
                nTempID++;
            }

            // 1. 임시 버전 만들기
            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

            string strSQL2 = "Insert into Version (ID, isRegular, isNormal, CreateTime, LastAccessTime, VersionName, OwnerID, SiteID, Description) values ";
            strSQL2 += string.Format("({0}, 1, 1, '{1}', '{1}', 'temp', {2}, {3}, NULL)", nTempID, strTime, nSOPGenUserID, dbMgr.SiteID);

            if (dbMgr.GetResultData(strSQL2) == null)
            //if (dbMgr.GetBatchData(strSQL2) == null)
            {
                rollback.Rollback(dbMgr);
                //dbMgr.BatchRollback();
                return -1;
            }
            else
                rollback.AddData(new UnE.SOP.RollbackData("Delete from Version where ID = " + nTempID));

            // 2. 기존버전에 연결된 Disaster를 임시버전에 연결하기
            strSQL2 = string.Format("Update Disaster set VersionID = {0} where VersionID = {1}", nTempID, nVersionID);

            if (dbMgr.GetResultData(strSQL2) == null)
            //if (dbMgr.GetBatchData(strSQL2) == null)
            {
                rollback.Rollback(dbMgr);
                //dbMgr.BatchRollback();
                return -1;
            }
            else
                rollback.AddData(new UnE.SOP.RollbackData(string.Format("Update Disaster set VersionID = {0} where VersionID = {1}", nVersionID, nTempID)));

            // 3. 기존버전의 ID 변경하기
            strSQL2 = string.Format("Update Version set ID = {0} where ID = {1}", nNewID, nVersionID);

            if (dbMgr.GetResultData(strSQL2) == null)
            //if (dbMgr.GetBatchData(strSQL2) == null)
            {
                rollback.Rollback(dbMgr);
                //dbMgr.BatchRollback();
                return -1;
            }
            else
                rollback.AddData(new UnE.SOP.RollbackData(string.Format("Update Version set ID = {0} where ID = {1}", nVersionID, nNewID)));

            // 4. Disaster를 원래 버전에 연결하기
            strSQL2 = string.Format("Update Disaster set VersionID = {0} where VersionID = {1}", nNewID, nTempID);

            if (dbMgr.GetResultData(strSQL2) == null)
            //if (dbMgr.GetBatchData(strSQL2) == null)
            {
                rollback.Rollback(dbMgr);
                //dbMgr.BatchRollback();
                return -1;
            }
            else
                rollback.AddData(new UnE.SOP.RollbackData(string.Format("Update Disaster set VersionID = {0} where VersionID = {1}", nTempID, nNewID)));

            // 5. 임시버전 삭제하기
            strSQL2 = string.Format("Delete Version where ID = {0}", nTempID);

            UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();

            if (rollbackData.AddInsertRollback(dbMgr, "Select ID, isRegular, isNormal, CreateTime, LastAccessTime, VersionName, OwnerID, Description, SiteID from Version where ID = " + nTempID, 0, 0, 0, 1, 1, 1, 0, 1, 0) == false)
            {
                rollback.Rollback(dbMgr);
                return -1;
            }

            if (dbMgr.GetResultData(strSQL2) == null)
            //if (dbMgr.GetBatchData(strSQL2) == null)
            {
                rollback.Rollback(dbMgr);
                //dbMgr.BatchRollback();
                return -1;
            }
            else
                rollback.AddData(rollbackData);

            /*if (dbMgr.BatchCommit() == false)
            {
                dbMgr.BatchRollback();
                return -1;
            }*/

            return nNewID;
        }

        // 트랜잭션을 사용하지 않는 버전(일부 구간에서 부분적으로 사용)
        // 1. 기존버전의 ID를 신규 ID로 바꾼다.(실패하면 사용중인 버전이므로 여기서 로직을 종료한다.)
        // 2. 기존버전의 ID로 신규 버전을 만든다.(실패하면 저장해둔 기존 버전을 Rollback 한다.)
        // 3. 신규버전의 저장에 성공하면 기존버전은삭제한다.
        public bool Save(FormMain frm, WebDBManager dbMgr, string strVersionName, int nVersionID, int nSOPGenUserID, string strDescription, List<int> usingLevelIDs, ref VersionInfo rVersion, out int nDisasterID)
        {
            // 사용자 정의 설정이 수정되었으면 DB에 저장한다.
            FormPanel.BarConfig barConfig = frm.GetPageLevel().GetBarConfig();
            barConfig.CheckChangedData();

            nDisasterID = 0;

            // 수식에서 사용된 변수가 SOP에서 사용하는 Config와 동일한지 여부를 판단
            string strUserDefinedConfigName;

            if (CheckDecisionExpression(frm, out strUserDefinedConfigName) == false)
                return false;

            m_dicDeletedActionStep.Clear();
            SaveDeletingActionStepID();

            int nTempVersionID = -1;
            int nOldVersionID = nVersionID;

            if (nVersionID > 0)
            {
                // 현재 사용중인 버전은 삭제가 안되므로 업데이트 할 수 없다.
                nTempVersionID = ChangeSOPVersion(dbMgr, nSOPGenUserID, nVersionID, -1);

                if (nTempVersionID < 0)
                    return false;
            }

            nVersionID = SaveVersion(frm, dbMgr, strVersionName, nVersionID, nSOPGenUserID, strDescription, ref rVersion, null);

            if (nVersionID < 0)
            {
                // Rollback
                if (nTempVersionID > 0 && nOldVersionID > 0)
                    ChangeSOPVersion(dbMgr, nSOPGenUserID, nTempVersionID, nOldVersionID);
                return false;
            }

            nDisasterID = AddDisaster(dbMgr, nVersionID, null);
            if (nDisasterID < 0)
            {
                // Rollback
                if (DeleteSOPVersion(dbMgr, nVersionID, true, null))
                {
                    if (nTempVersionID > 0 && nOldVersionID >0)
                        ChangeSOPVersion(dbMgr, nSOPGenUserID, nTempVersionID, nOldVersionID);
                }

                return false;
            }

            if (usingLevelIDs != null)
                AddDisasterLevelIDs(dbMgr, nDisasterID, usingLevelIDs);

            Dictionary<System.Windows.Forms.TabPage, int> dicActionStepIDs = AddActionSteps(frm, dbMgr, nDisasterID, strUserDefinedConfigName, null);
            if (dicActionStepIDs == null)
            {
                // Rollback
                if (DeleteSOPVersion(dbMgr, nVersionID, true, null))
                {
                    if (nTempVersionID > 0 && nOldVersionID > 0)
                        ChangeSOPVersion(dbMgr, nSOPGenUserID, nTempVersionID, nOldVersionID);
                }

                return false;
            }

            Dictionary<System.Windows.Forms.TabPage, Dictionary<StepMemberData, int>> dicStepMemberIDs = AddStepMembers(frm, dbMgr, dicActionStepIDs, null);
            if (dicStepMemberIDs == null)
            {
                // Rollback
                if (DeleteSOPVersion(dbMgr, nVersionID, true, null))
                {
                    if (nTempVersionID > 0 && nOldVersionID > 0)
                        ChangeSOPVersion(dbMgr, nSOPGenUserID, nTempVersionID, nOldVersionID);
                }

                return false;
            }

            if (!AddComponents(frm, dbMgr, dicStepMemberIDs, dicActionStepIDs, null))
            {
                // Rollback
                if (DeleteSOPVersion(dbMgr, nVersionID, true, null))
                {
                    if (nTempVersionID > 0 && nOldVersionID > 0)
                        ChangeSOPVersion(dbMgr, nSOPGenUserID, nTempVersionID, nOldVersionID);
                }

                return false;
            }

            // StepMember의 TeamID와 TeamType 변경
            VariousData<int> _teamID = null, _teamType = null;

            if (GetStepMemberTeamData(dbMgr, out _teamID, out _teamType))
            {
                foreach (KeyValuePair<System.Windows.Forms.TabPage, Dictionary<StepMemberData, int>> pair in dicStepMemberIDs)
                {
                    foreach (KeyValuePair<StepMemberData, int> pair2 in pair.Value)
                    {
                        int nStepMemberID = pair2.Value;

                        string strSQL = string.Format("Update StepMember set TeamID = {0}, TeamType = {1} where ID = {2}",
                            _teamID.Data, _teamType.Data, nStepMemberID);

                        if (dbMgr.GetResultData(strSQL) == null)
                        {
                            // Rollback
                            if (DeleteSOPVersion(dbMgr, nVersionID, true, null))
                            {
                                if (nTempVersionID > 0 && nOldVersionID > 0)
                                    ChangeSOPVersion(dbMgr, nSOPGenUserID, nTempVersionID, nOldVersionID);
                            }

                            return false;
                        }
                    }
                }
            }

            // 임시버전을 삭제한다.
            if (nTempVersionID > 0)
            {
                UnE.SOP.RollbackManager rollback = new UnE.SOP.RollbackManager();

                //if (dbMgr.BeginBatch() == false)
                //    return false;

                if (DeleteSOPVersion(dbMgr, nTempVersionID, true, rollback, true) == false)
                {
                    rollback.Rollback(dbMgr);
                    //dbMgr.BatchRollback();
                    return false;
                }

                /*if (dbMgr.BatchCommit() == false)
                {
                    dbMgr.BatchRollback();
                    return false;
                }*/
            }

            return true;
        }

        private void AddDisasterLevelIDs(WebDBManager dbMgr, int nDisasterID, List<int> usingLevelIDs)
        {
            string strSQL = "Delete from SOPGenLevelDisaster where DisasterID = " + nDisasterID.ToString() + " and SiteID = " + dbMgr.SiteID.ToString();

            if (dbMgr.GetResultData(strSQL) == null)
                return;

            foreach (int nLevelID in usingLevelIDs)
            {
                strSQL = "Insert into SOPGenLevelDisaster (LevelID, DisasterID, SiteID) values (";
                strSQL += string.Format("{0}, {1}, {2})", nLevelID, nDisasterID, dbMgr.SiteID);

                if (dbMgr.GetResultData(strSQL) == null)
                    return;
            }
        }

        // 트랜잭션 사용하는 버전
        // nVersionID : nVersionID가 0보다 크면 기존 버전을 덮어쓴다.
        /*public bool Save(FormMain frm, WebDBManager dbMgr, string strVersionName, int nVersionID, int nSOPGenUserID, string strDescription, ref VersionInfo rVersion, out int nDisasterID)
        {
            // 사용자 정의 설정이 수정되었으면 DB에 저장한다.
            FormPanel.BarConfig barConfig = frm.GetPageLevel().GetBarConfig();
            barConfig.CheckChangedData();

            nDisasterID = 0;

            // 수식에서 사용된 변수가 SOP에서 사용하는 Config와 동일한지 여부를 판단
            string strUserDefinedConfigName;

            if (CheckDecisionExpression(frm, out strUserDefinedConfigName) == false)
                return false;

            m_dicDeletedActionStep.Clear();
            SaveDeletingActionStepID();

            // Batch Job Start - Begin Transaction
            dbMgr.BeginBatch();

			if (nVersionID > 0)
			{
				// 현재 사용중인 버전은 삭제가 안되므로 업데이트 할 수 없다.
				if(!DeleteSOPVersion(dbMgr, nVersionID, false, true, true))
				{
                    dbMgr.BatchRollback();
					return false;
				}
			}

            if (nVersionID > 0)
                UpdateVersion(dbMgr, nVersionID, strDescription, ref rVersion);
            else
                nVersionID = SaveVersion(frm, dbMgr, strVersionName, nVersionID, nSOPGenUserID, strDescription, ref rVersion, true);

            if (nVersionID < 0)
            {
                // Rollback
                dbMgr.BatchRollback();
                return false;
            }			

            nDisasterID = AddDisaster(dbMgr, nVersionID, true);
            if (nDisasterID < 0)
            {
                // Rollback
				dbMgr.BatchRollback();
                return false;
            }

            Dictionary<System.Windows.Forms.TabPage, int> dicActionStepIDs = AddActionSteps(frm, dbMgr, nDisasterID, strUserDefinedConfigName, true);
            if (dicActionStepIDs == null)
            {
                // Rollback
				dbMgr.BatchRollback();
                return false;
            }

            Dictionary<System.Windows.Forms.TabPage, Dictionary<StepMemberData, int>> dicStepMemberIDs = AddStepMembers(frm, dbMgr, dicActionStepIDs, true);
            if (dicStepMemberIDs == null)
            {
                // Rollback
				dbMgr.BatchRollback();
                return false;
            }

            if (!AddComponents(frm, dbMgr, dicStepMemberIDs, dicActionStepIDs, true))
            {
                // Rollback
				dbMgr.BatchRollback();
                return false;
            }

            // StepMember의 TeamID와 TeamType 변경
            VariousData<int> _teamID = null, _teamType = null;
            
            if (GetStepMemberTeamData(dbMgr, true, out _teamID, out _teamType))
            {
                foreach (KeyValuePair<System.Windows.Forms.TabPage, Dictionary<StepMemberData, int>> pair in dicStepMemberIDs)
                {
                    foreach (KeyValuePair<StepMemberData, int> pair2 in pair.Value)
                    {
                        int nStepMemberID = pair2.Value;

                        string strSQL = string.Format("Update StepMember set TeamID = {0}, TeamType = {1} where ID = {2}",
                            _teamID.Data, _teamType.Data, nStepMemberID);

                        if (dbMgr.GetBatchData(strSQL) == null)
                        {
                            dbMgr.BatchRollback();
                            return false;
                        }
                    }
                }
            }
            /////////////////////////////////////////////////////////////////

            // Batch Job end - Commit
			dbMgr.BatchCommit();

            return true;
        }*/

        private bool AddComponents(FormMain frm, WebDBManager dbMgr, Dictionary<System.Windows.Forms.TabPage, Dictionary<StepMemberData, int>> dicStepMemberIDs, Dictionary<System.Windows.Forms.TabPage, int> dicActionStepIDs, UnE.SOP.RollbackManager rollback)
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
                        Sections.SOPTeam.SOPTeamType nTeamType = panel.TeamType;

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
                                if (!AddProcess(dbMgr, nStepMemberID, (Sections.SectionProcess)section, ref nProcessID, ref nProcessMissionID, rollback))
                                    return false;
                                else
                                    dicComponentID[section] = nProcessID;
                            }
                            else if (type == Sections.Section.ComponentType.DECISION)
                            {
                                if (!AddDecision(dbMgr, nStepMemberID, (Sections.SectionDecision)section, ref nDecisionID, rollback))
                                    return false;
                                else
                                    dicComponentID[section] = nDecisionID;
                            }
                            else if (type == Sections.Section.ComponentType.ANNOTATION)
                            {
                                if (!AddAnnotation(dbMgr, nStepMemberID, (Sections.SectionAnnotation)section, ref nAnnotationID, rollback))
                                    return false;
                                else
                                    dicComponentID[section] = nAnnotationID;
                            }
                            else if (type == Sections.Section.ComponentType.ENDPOINT)
                            {
                                if (!AddEndPoint(dbMgr, nStepMemberID, (Sections.SectionEndPoint)section, ref nEndPointID, rollback))
                                    return false;
                                else
                                    dicComponentID[section] = nEndPointID;
                            }
                            else if (type == Sections.Section.ComponentType.LINK)
                            {
                                if (!AddLink(dbMgr, nStepMemberID, (Sections.SectionLink)section, ref nLinkID, rollback))
                                    return false;
                                else
                                    dicComponentID[section] = nLinkID;
                            }
                            else if (type == Sections.Section.ComponentType.TRANSSOP)
                            {
                                if (!AddTransSOP(dbMgr, nStepMemberID, (Sections.SectionTransSOP)section, dicActionStepIDs, ref nTransSOP, rollback))
                                    return false;
                                else
                                    dicComponentID[section] = nTransSOP;
                            }
                            else if (type == Sections.Section.ComponentType.INTERNAL)
                            {
                                if (!AddInternal(dbMgr, nStepMemberID, (Sections.SectionInternal)section, ref nInternalID, rollback))
                                    return false;
                                else
                                    dicComponentID[section] = nInternalID;
                            }
                            else if (type == Sections.Section.ComponentType.EXTERNAL)
                            {
                                if (!AddExternal(dbMgr, nStepMemberID, (Sections.SectionExternal)section, ref nExternalID, rollback))
                                    return false;
                                else
                                    dicComponentID[section] = nExternalID;
                            }
                            else if (type == Sections.Section.ComponentType.TRANSMISSION)
                            {
                                if (!AddTransmission(dbMgr, nStepMemberID, (Sections.SectionTransmission)section, ref nTransmissionID, rollback))
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
							if (!AddGroup(dbMgr, nStepMemberID, (Sections.SectionGroup)group, ref nGroupID, rollback))
								return false;
							else
								dicComponentID[group] = nGroupID;

							AddGroupComponent(dbMgr, nStepMemberID, group, nGroupID, dicComponentID, rollback);
						}

                        // Component와 연결된 화살표 저장
                        foreach (Sections.Section section in panel.Sections)
                        {
                            if (!AddArrow(dbMgr, nStepMemberID, section, dicComponentID, ref nArrowID, rollback))
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        private void GetComponentMaxID(WebDBManager dbMgr, string strComponentTableName, ref int nComponentID)
        {
            if (nComponentID < 0)
            {
                string strSQL = "Select max(id) from " + strComponentTableName;
                ArrayList arrResult = dbMgr.GetResultData(strSQL);
                //ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

                if (arrResult == null || arrResult.Count == 0)
                    nComponentID = 0;
                else
                    nComponentID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);
            }
        }

        private bool AddArrow(WebDBManager dbMgr, int nStepMemberID, Sections.Section section, Dictionary<Sections.Section, int> dicComponentID, ref int nArrowID, UnE.SOP.RollbackManager rollback)
        {
            GetComponentMaxID(dbMgr, "Arrow", ref nArrowID);

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

                if (rollback != null)
                {
                    UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData("Delete from Arrow where ID = " + nArrowID);

                    if (dbMgr.GetResultData(strSQL) != null)
                        rollback.AddData(rollbackData);
                    else
                        return false;
                }
                else
                {
                    if (dbMgr.GetResultData(strSQL) == null)
                        return false;
                }

                /*ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

                if (arrResult == null)
                    return false;*/
            }

            return true;
        }

        public static string GetProcessTeamList(Sections.SectionProcess section)
        {
            Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;
            string strIDs = "";

            foreach (Sections.SOPTeam team in data.TeamList)
            {
                int nTeamID = team.IncludeChildTeams ? team.TeamID : -team.TeamID;

                if (strIDs.Length == 0)
                    strIDs = string.Format("{0}({1})", nTeamID, (int)team.TeamType);
                else
                    strIDs += string.Format(", {0}({1})", nTeamID, (int)team.TeamType);
            }

            return strIDs;
        }

        public static string GetInternalTeamList(Sections.SectionInternal section)
        {
            Sections.SectionDataInternal data = (Sections.SectionDataInternal)section.Data;
            string strIDs = "";

            foreach (Sections.SOPTeam team in data.TeamList)
            {
                int nTeamID = team.TeamID;

                if (team.IncludeChildTeams == false)
                {
                    if (team.TeamType == Sections.SOPTeam.SOPTeamType.Normal || team.TeamType == Sections.SOPTeam.SOPTeamType.Holiday ||
                        team.TeamType == Sections.SOPTeam.SOPTeamType.External || team.TeamType == Sections.SOPTeam.SOPTeamType.Regular)
                        nTeamID = -nTeamID;
                }
                //int nTeamID = team.IncludeChildTeams ? team.TeamID : -team.TeamID;

                if (strIDs.Length == 0)
                    strIDs = string.Format("{0}({1})", nTeamID, (int)team.TeamType);
                else
                    strIDs += string.Format(", {0}({1})", nTeamID, (int)team.TeamType);
            }

            return strIDs;
        }

        // strText에 따옴표(')가 있을 경우 DB에서 인식할 수 있도록 ('')로 치환시킨다.
        private string ChangeSpecialCharacter(string strText)
        {
            return strText.Replace("'", "''");
        }

        private bool AddProcess(WebDBManager dbMgr, int nStepMemberID, Sections.SectionProcess section, ref int nProcessID, ref int nProcessMissionID, UnE.SOP.RollbackManager rollback)
        {
            GetComponentMaxID(dbMgr, "Process", ref nProcessID);

            Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;

            Sections.ProcessingTime.Type type = data.ProcessingTime.ProcessingType;
            int nProcessType = (int)type;

            string strTeamList = GetProcessTeamList(section);
            string strCommanderMemberType = "NULL", strCommanderMemberID = "NULL", strCommanderDisplayText = "NULL";

            if (data.Commander != null)
            {
                if (data.Commander.Team == null)
                {
                    strCommanderMemberType = "-1";
                    strCommanderDisplayText = "'" + data.Commander.DisplayText + "'";
                }
                else
                { 
                    strCommanderMemberType = ((int)data.Commander.Team.TeamType).ToString();
                    strCommanderMemberID = data.Commander.Team.TeamID.ToString();
                    strCommanderDisplayText = "'" + data.Commander.DisplayText + "'";
                }
            }

            ChangeComponentID(data, nProcessID + 1);

            StringBuilder sb = new StringBuilder();
            sb.Append("insert into Process (ID, x, y, width, height, text, TeamList, valign, halign, ");
            sb.Append(" ComponentID, ProcessTime, ProcessTimeType, useProcessTime, useMissionMessage, onlyTeamLeader, ");
            sb.Append(" StepMemberID, CommanderMemberType, CommanderMemberID, CommanderDisplayText, ");
            sb.Append(" FontName, FontStyle, FontSize, LineSpace, FontColor, AutoRun  ) ");
            sb.AppendFormat(" values ({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', {7}, {8}, ",
                    ++nProcessID,
                    section.Position.X,
                    section.Position.Y,
                    section.RectSize.Width,
                    section.RectSize.Height,
                    ChangeSpecialCharacter(section.TextUP),
                    ChangeSpecialCharacter(strTeamList),
                    (int)data.TextVerticalAlign,
                    (int)data.TextHorizontalAlign
            );
            
            sb.AppendFormat(" '{0}', {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9},'{10}',{11},{12},{13},{14}, {15})",
                    ChangeSpecialCharacter(data.ComponentID),
                    data.ProcessingTime.Time,
                    (int)data.ProcessingTime.ProcessingType,
                    data.UseProcessingTime ? 1 : 0,
                    data.MissionTransfer ? 1 : 0,
                    data.TransferTeamLeaderOnly ? 1 : 0,
                    nStepMemberID,
                    strCommanderMemberType,
                    strCommanderMemberID,
                    strCommanderDisplayText,
                    section.TextFont.Name,
                    (int)section.TextFont.Style,
                    section.TextFont.Size,
                    data.LineSpace,
                    (int)section.TextColor.ToArgb(),
                    data.AutoRun ? 1 : 0
            );

            string strSQL = sb.ToString();
            
            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData("Delete from Process where ID = " + nProcessID);

                if (dbMgr.GetResultData(strSQL) != null)
                    rollback.AddData(rollbackData);
                else
                    return false;
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) == null)
                    return false;
            }

            /*ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;*/

            return AddProcessMission(dbMgr, nProcessID, data, ref nProcessMissionID, rollback);
        }

        private void ChangeComponentID(Sections.SectionData data, int nID)
        {
            int nIndex = data.ComponentID.LastIndexOf('_');

            if (nIndex >= 0)
            {
                string strComponentID = data.ComponentID.Substring(0, nIndex + 1) + nID.ToString();
                data.ComponentID = strComponentID;
            }
            else
            {
                string strComponentID = data.ComponentID + "_" + nID.ToString();
                data.ComponentID = strComponentID;
            }
        }

        private bool AddProcessMission(WebDBManager dbMgr, int nProcessID, Sections.SectionDataProcess data, ref int nProcessMissionID, UnE.SOP.RollbackManager rollback)
        {
            GetComponentMaxID(dbMgr, "ProcessMission", ref nProcessMissionID);

            foreach (Sections.MissionItem mission in data.MissionItems)
            {
                string strCommanderMemberType = "NULL", strCommanderMemberID = "NULL", strCommanderDisplayText = "NULL";

                if (mission.Commander != null)
                {
                    if (mission.Commander.Team == null)
                    {
                        strCommanderMemberType = "-1";
                        strCommanderDisplayText = "'" + mission.Commander.DisplayText + "'";
                    }
                    else
                    {
                        strCommanderMemberType = ((int)mission.Commander.Team.TeamType).ToString();
                        strCommanderMemberID = mission.Commander.Team.TeamID.ToString();
                        strCommanderDisplayText = "'" + mission.Commander.DisplayText + "'";
                    }
                }

                string strSQL = string.Format("insert into ProcessMission (ID, missionText, ProcessID, TransmissionType, missionTarget, CommanderDisplayText, CommanderMemberType, CommanderMemberID  ) values ({0}, '{1}', {2}, {3}, '{4}', {5},{6},{7})",
                    ++nProcessMissionID, ChangeSpecialCharacter(mission.Mission), nProcessID, mission.TransmissionType, mission.Target, strCommanderDisplayText, strCommanderMemberType, strCommanderMemberID);

                if (rollback != null)
                {
                    UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData("Delete from ProcessMission where ID = " + nProcessMissionID);

                    if (dbMgr.GetResultData(strSQL) != null)
                        rollback.AddData(rollbackData);
                    else
                        return false;
                }
                else
                {
                    if (dbMgr.GetResultData(strSQL) == null)
                        return false;
                }

                /*ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

                if (arrResult == null)
                    return false;*/
            }

            return true;
        }

        private bool AddDecision(WebDBManager dbMgr, int nStepMemberID, Sections.SectionDecision section, ref int nDecisionID, UnE.SOP.RollbackManager rollback)
        {
            GetComponentMaxID(dbMgr, "Decision", ref nDecisionID);
            Sections.SectionDataDecision data = (Sections.SectionDataDecision)section.Data;

            string strAutoRunScript = data.Expression.Length == 0 ? "NULL" : "'" + ChangeSpecialCharacter(data.Expression) + "'";
            string strVariableTypes = strAutoRunScript == null ? "NULL" : "'" + PropertiesDecision.ToVariableTypeString(data) + "'";

            if (strVariableTypes.Length == 2)
                strVariableTypes = "NULL";

            ChangeComponentID(data, nDecisionID + 1);

            StringBuilder sb = new StringBuilder();
            sb.Append("insert into Decision (ID, x, y, width, height, text, ComponentID, StepMemberID, valign, halign, ");
            sb.Append(" FontName, FontStyle, FontSize, LineSpace, FontColor, autoRunScript, autoRunScriptVariableTypes) ");
            sb.AppendFormat(" values ({0},{1},{2},{3},{4},'{5}','{6}',{7},{8},{9},'{10}',{11},{12},{13},{14}, {15}, {16} )",
                ++nDecisionID, 
                section.Position.X, 
                section.Position.Y, 
                section.RectSize.Width, 
                section.RectSize.Height, 
                ChangeSpecialCharacter(section.Title), 
                ChangeSpecialCharacter(data.ComponentID), 
                nStepMemberID,
                (int)data.TextVerticalAlign,
                (int)data.TextHorizontalAlign,
                section.TextFont.Name,
                (int)section.TextFont.Style,
                section.TextFont.Size,
                data.LineSpace,
                (int)section.TextColor.ToArgb(),
                strAutoRunScript,
                strVariableTypes
                );
      
            string strSQL = sb.ToString();

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData("Delete from Decision where ID = " + nDecisionID);

                if (dbMgr.GetResultData(strSQL) != null)
                    rollback.AddData(rollbackData);
                else
                    return false;
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) == null)
                    return false;
            }

            //ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);
            //return arrResult != null;
            return true;
        }

        private bool AddAnnotation(WebDBManager dbMgr, int nStepMemberID, Sections.SectionAnnotation section, ref int nAnnotationID, UnE.SOP.RollbackManager rollback)
        {
            GetComponentMaxID(dbMgr, "Annotation", ref nAnnotationID);
            Sections.SectionDataAnnotation data = (Sections.SectionDataAnnotation)section.Data;

            ChangeComponentID(data, nAnnotationID + 1);

            StringBuilder sb = new StringBuilder();
            sb.Append("insert into Annotation ( ID, x, y, width, height, text, ComponentID, StepMemberID, valign, halign, ");
            sb.Append(" FontName, FontStyle, FontSize, LineSpace, FontColor  ) ");
            sb.AppendFormat(" values ( {0}, {1}, {2}, {3}, {4}, '{5}', '{6}', {7}, {8}, {9}, '{10}',{11},{12},{13},{14})",
                ++nAnnotationID, 
                section.Position.X,
                section.Position.Y,
                section.RectSize.Width,
                section.RectSize.Height,
                ChangeSpecialCharacter(section.Title),
                ChangeSpecialCharacter(data.ComponentID),
                nStepMemberID,
                (int)data.TextVerticalAlign,
                (int)data.TextHorizontalAlign,
                section.TextFont.Name,
                (int)section.TextFont.Style,
                section.TextFont.Size,
                data.LineSpace,
                (int)section.TextColor.ToArgb()                
                );

            string strSQL = sb.ToString();

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData("Delete from Annotation where ID = " + nAnnotationID);

                if (dbMgr.GetResultData(strSQL) != null)
                    rollback.AddData(rollbackData);
                else
                    return false;
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) == null)
                    return false;
            }

            //ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);
            //return arrResult != null;
            return true;
        }

        private bool AddEndPoint(WebDBManager dbMgr, int nStepMemberID, Sections.SectionEndPoint section, ref int nEndPointID, UnE.SOP.RollbackManager rollback)
        {
            GetComponentMaxID(dbMgr, "EndPoint", ref nEndPointID);
            Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)section.Data;

            ChangeComponentID(data, nEndPointID + 1);

            StringBuilder sb = new StringBuilder();
            sb.Append("insert into EndPoint ( ID, x, y, width, height, text, ComponentID, isBegin, StepMemberID, valign, halign, ");
            sb.Append(" FontName, FontStyle, FontSize, LineSpace, FontColor  ) ");
            sb.AppendFormat(" values ( {0}, {1}, {2}, {3}, {4}, '{5}', '{6}', {7}, {8}, {9}, {10},'{11}',{12},{13},{14},{15} )",
                ++nEndPointID,  // 0
                section.Position.X, 
                section.Position.Y,  
                section.RectSize.Width, 
                section.RectSize.Height,
                ChangeSpecialCharacter(section.Title),// 5
                ChangeSpecialCharacter(data.ComponentID),
                data.IsBegin ? 1 : 0,
                nStepMemberID,
                (int)data.TextVerticalAlign, 
                (int)data.TextHorizontalAlign, // 10
                section.TextFont.Name,
                (int)section.TextFont.Style,
                section.TextFont.Size,
                data.LineSpace,
                (int)section.TextColor.ToArgb()   // 15
                );

            string strSQL = sb.ToString();

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData("Delete from EndPoint where ID = " + nEndPointID);

                if (dbMgr.GetResultData(strSQL) != null)
                    rollback.AddData(rollbackData);
                else
                    return false;
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) == null)
                    return false;
            }

            //ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);
            //return arrResult != null;
            return true;
        }

        private bool AddLink(WebDBManager dbMgr, int nStepMemberID, Sections.SectionLink section, ref int nLinkID, UnE.SOP.RollbackManager rollback)
        {
            GetComponentMaxID(dbMgr, "Link", ref nLinkID);
            Sections.SectionDataLink data = (Sections.SectionDataLink)section.Data;

            ChangeComponentID(data, nLinkID + 1);

            StringBuilder sb = new StringBuilder();
            sb.Append("insert into Link ( ID, x, y, width, height, text, ComponentID, LinkedComponentID, StepMemberID, valign, halign, ");
            sb.Append(" FontName, FontStyle, FontSize, LineSpace, FontColor  ) ");
            sb.AppendFormat(" values ( {0}, {1}, {2}, {3}, {4}, '{5}', '{6}', '{7}', {8}, {9}, {10},'{11}',{12},{13},{14},{15} )",
                ++nLinkID,  // 0
                section.Position.X,
                section.Position.Y,
                section.RectSize.Width,
                section.RectSize.Height,
                ChangeSpecialCharacter(section.Title),// 5
                ChangeSpecialCharacter(data.ComponentID),
                ChangeSpecialCharacter(data.LinkedSection.Data.ComponentID),
                nStepMemberID,
                (int)data.TextVerticalAlign, 
                (int)data.TextHorizontalAlign,
                section.TextFont.Name,
                (int)section.TextFont.Style,
                section.TextFont.Size,
                data.LineSpace,
                (int)section.TextColor.ToArgb()
                ); // 10

            string strSQL = sb.ToString();

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData("Delete from Link where ID = " + nLinkID);

                if (dbMgr.GetResultData(strSQL) != null)
                    rollback.AddData(rollbackData);
                else
                    return false;
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) == null)
                    return false;
            }

            //ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);
            //return arrResult != null;
            return true;
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

        private bool AddTransSOP(WebDBManager dbMgr, int nStepMemberID, Sections.SectionTransSOP section, Dictionary<System.Windows.Forms.TabPage, int> dicActionStepIDs, ref int nTransSOPID, UnE.SOP.RollbackManager rollback)
        {
            GetComponentMaxID(dbMgr, "TransSOP", ref nTransSOPID);
            Sections.SectionDataTransSOP data = (Sections.SectionDataTransSOP)section.Data;

            // m_dicDeletedActionStep를 이용하여 Data의 LinkedActionStep 변환
            ChangeTransSOPLinkedActionStep(data, dicActionStepIDs);

            ChangeComponentID(data, nTransSOPID + 1);

            StringBuilder sb = new StringBuilder();
            sb.Append("insert into TransSOP (ID, x, y, width, height, text, ComponentID, StepMemberID, LinkedActionStepID, Description, valign, halign, ");
            sb.Append(" FontName, FontStyle, FontSize, LineSpace, FontColor  ) ");
            sb.AppendFormat(" values ({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', {7}, {8}, '{9}', {10}, {11} , '{12}',{13},{14},{15},{16})",
                 ++nTransSOPID,
                 section.Position.X, 
                 section.Position.Y,
                 section.RectSize.Width, 
                 section.RectSize.Height,
                 ChangeSpecialCharacter(section.Title),
                 ChangeSpecialCharacter(data.ComponentID),                 
                 nStepMemberID, 
                 data.LinkedActionStepID, 
                 ChangeSpecialCharacter(data.Description),
                 (int)data.TextVerticalAlign, 
                 (int)data.TextHorizontalAlign,
                 section.TextFont.Name,
                (int)section.TextFont.Style,
                section.TextFont.Size,
                data.LineSpace,
                (int)section.TextColor.ToArgb()
                 );

            string strSQL = sb.ToString();

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData("Delete from TransSOP where ID = " + nTransSOPID);

                if (dbMgr.GetResultData(strSQL) != null)
                    rollback.AddData(rollbackData);
                else
                    return false;
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) == null)
                    return false;
            }

            //ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);
            //return arrResult != null;
            return true;
        }

        private bool AddInternal(WebDBManager dbMgr, int nStepMemberID, Sections.SectionInternal section, ref int nInternalID, UnE.SOP.RollbackManager rollback)
        {
            GetComponentMaxID(dbMgr, "InternalTransmission", ref nInternalID);
            Sections.SectionDataInternal data = (Sections.SectionDataInternal)section.Data;


            string szTeamList = GetInternalTeamList(section);
            Sections.SectionCommander commander = data.Commander;
            string strCommanderMemberType = "NULL", strCommanderMemberID = "NULL", strCommanderDisplayText = "NULL";

            if (commander != null)
            {
                if (commander.Team == null)
                {
                    strCommanderMemberType = "-1";
                    strCommanderDisplayText = "'" + data.Commander.DisplayText + "'";
                }
                else
                {
                    strCommanderMemberType = ((int)data.Commander.Team.TeamType).ToString();
                    strCommanderMemberID = data.Commander.Team.TeamID.ToString();
                    strCommanderDisplayText = "'" + data.Commander.DisplayText + "'";
                }
            }

            ChangeComponentID(data, nInternalID + 1);

            StringBuilder sb = new StringBuilder();
            sb.Append("insert into InternalTransmission (ID, x, y, width, height, text, valign, halign, ");
            sb.Append(" ComponentID, usePopupMessage, useMobileApp, useBroadcast, StepMemberID, BroadcastMessage,");
            sb.Append(" TeamList, onlyTeamLeader, CommanderMemberType, CommanderMemberID, CommanderDisplayText, ");
            sb.Append(" FontName, FontStyle, FontSize, LineSpace, FontColor, AutoRun ");

            sb.AppendFormat(") values ({0}, {1}, {2}, {3}, {4}, '{5}', {6}, {7}, ",
                ++nInternalID, 
                section.Position.X,
                section.Position.Y, 
                section.RectSize.Width,
                section.RectSize.Height,
                ChangeSpecialCharacter(section.Title),
                (int)data.TextVerticalAlign,
                (int)data.TextHorizontalAlign);
                        
            sb.AppendFormat("'{0}', {1}, {2}, {3}, {4}, '{5}',",
                ChangeSpecialCharacter(data.ComponentID), 
                data.UsePopupMessage ? 1 : 0, 
                data.UseMobileApp ? 1 : 0, 
                data.UseBroadcast ? 1 : 0, 
                nStepMemberID,
                data.BroadcastMessage);

            sb.AppendFormat("'{0}', {1}, {2}, {3}, {4},",
                ChangeSpecialCharacter(szTeamList),
                data.TransferTeamLeaderOnly ? 1: 0,
                strCommanderMemberType,
                strCommanderMemberID, 
                strCommanderDisplayText
                );

            sb.AppendFormat("'{0}', {1}, {2}, {3}, {4}, {5}",
               section.TextFont.Name,
                (int)section.TextFont.Style,
                section.TextFont.Size,
                data.LineSpace,
                (int)section.TextColor.ToArgb(),
                data.AutoRun ? 1 : 0
               );

            sb.Append(")");

            string strSQL = sb.ToString();

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData("Delete from InternalTransmission where ID = " + nInternalID);

                if (dbMgr.GetResultData(strSQL) != null)
                    rollback.AddData(rollbackData);
                else
                    return false;
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) == null)
                    return false;
            }

            //ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);
            //return arrResult != null;
            return true;
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

        private bool AddExternal(WebDBManager dbMgr, int nStepMemberID, Sections.SectionExternal section, ref int nExternalID, UnE.SOP.RollbackManager rollback)
        {
            GetComponentMaxID(dbMgr, "ExternalTransmission", ref nExternalID);
            Sections.SectionDataExternal data = (Sections.SectionDataExternal)section.Data;

            string strSMSText = "''", strSMSReceivers = "''", strFaxReceivers = "''";
            GetSMSData(data, ref strSMSText, ref strSMSReceivers);
            GetFaxData(data, ref strFaxReceivers);

            string strChangeSMSText = "'" + ChangeSpecialCharacter(strSMSText.Substring(1, strSMSText.Length - 2)) + "'";
            string strChangeSMSReceivers = "'" + ChangeSpecialCharacter(strSMSReceivers.Substring(1, strSMSReceivers.Length - 2)) + "'";
            string strChangeFaxReceivers = "'" + ChangeSpecialCharacter(strFaxReceivers.Substring(1, strFaxReceivers.Length - 2)) + "'";

            ChangeComponentID(data, nExternalID + 1);

            StringBuilder sb = new StringBuilder();
            sb.Append("insert into ExternalTransmission (ID, x, y, width, height, text, ComponentID, useSMS,");
            sb.Append(" SMSText, SMSExternalTeamIDList, useEFax, FaxExternalTeamIDList, StepMemberID, valign, halign, ");
            sb.Append(" FontName, FontStyle, FontSize, LineSpace, FontColor ) ");
            sb.AppendFormat(" values ({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14},'{15}', {16}, {17}, {18}, {19} )",
                ++nExternalID, // 0
                section.Position.X, 
                section.Position.Y,
                section.RectSize.Width, 
                section.RectSize.Height, 
                ChangeSpecialCharacter(section.Title),  // 5
                ChangeSpecialCharacter(data.ComponentID),
                data.UseSMS ? 1 : 0, 
                strChangeSMSText, 
                strChangeSMSReceivers, 
                data.UseFax ? 1 : 0,    // 10
                strChangeFaxReceivers, 
                nStepMemberID,
                (int)data.TextVerticalAlign,
                (int)data.TextHorizontalAlign,
                section.TextFont.Name,
                (int)section.TextFont.Style,
                section.TextFont.Size,
                data.LineSpace,
                (int)section.TextColor.ToArgb()
                
                );

            string strSQL = sb.ToString();

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData("Delete from ExternalTransmission where ID = " + nExternalID);

                if (dbMgr.GetResultData(strSQL) != null)
                    rollback.AddData(rollbackData);
                else
                    return false;
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) == null)
                    return false;
            }

            //ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);
            //return arrResult != null;
            return true;
        }

        private bool AddTransmission(WebDBManager dbMgr, int nStepMemberID, Sections.SectionTransmission section, ref int nTransmissionID, UnE.SOP.RollbackManager rollback)
        {
            GetComponentMaxID(dbMgr, "Transmission", ref nTransmissionID);
            Sections.SectionDataTransmission data = (Sections.SectionDataTransmission)section.Data;

            string strSMSText = "''", strSMSReceivers = "''", strFaxReceivers = "''";
            GetSMSData(data, ref strSMSText, ref strSMSReceivers);
            GetFaxData(data, ref strFaxReceivers);

            string strChangeSMSText = "'" + ChangeSpecialCharacter(strSMSText.Substring(1, strSMSText.Length - 2)) + "'";
            string strChangeSMSReceivers = "'" + ChangeSpecialCharacter(strSMSReceivers.Substring(1, strSMSReceivers.Length - 2)) + "'";
            string strChangeFaxReceivers = "'" + ChangeSpecialCharacter(strFaxReceivers.Substring(1, strFaxReceivers.Length - 2)) + "'";

            string strBroadcastMessage = "'" + ChangeSpecialCharacter(data.DataInternal.BroadcastMessage) + "'";

            ChangeComponentID(data, nTransmissionID + 1);

            StringBuilder sb = new StringBuilder();
            sb.Append("insert into Transmission (ID, x, y, width, height, text, ComponentID, useInternalPopupMessage, ");
            sb.Append(" useInternalMobileApp, useInternalBroadcast, useExternalSMS, externalSMSText, SMSExternalTeamIDList, ");
            sb.Append(" useExternalFax, FaxExternalTeamIDList, StepMemberID, InternalBroadcastMessage, valign, halign, ");
            sb.Append(" FontName, FontStyle, FontSize, LineSpace, FontColor ) ");
            sb.AppendFormat(" values ({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, '{19}', {20}, {21}, {22}, {23} )",
                    ++nTransmissionID, // 0
                    section.Position.X, 
                    section.Position.Y,
                    section.RectSize.Width,
                    section.RectSize.Height,
                    ChangeSpecialCharacter(section.Title),  //5
                    ChangeSpecialCharacter(data.ComponentID),
                    data.DataInternal.UsePopupMessage ? 1 : 0, 
                    data.DataInternal.UseMobileApp ? 1 : 0,
                    data.DataInternal.UseBroadcast ? 1 : 0,
                    data.DataExternal.UseSMS ? 1 : 0,  // 10
                    strChangeSMSText, 
                    strChangeSMSReceivers, 
                    data.DataExternal.UseFax ? 1 : 0, 
                    strChangeFaxReceivers, 
                    nStepMemberID,        //15
                    strBroadcastMessage,
                    (int)data.TextVerticalAlign,
                    (int)data.TextHorizontalAlign,
                    section.TextFont.Name,
                    (int)section.TextFont.Style, // 20
                    section.TextFont.Size,
                    data.LineSpace,
                    (int)section.TextColor.ToArgb()
                    
                    );
            
            string strSQL = sb.ToString();

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData("Delete from Transmission where ID = " + nTransmissionID);

                if (dbMgr.GetResultData(strSQL) != null)
                    rollback.AddData(rollbackData);
                else
                    return false;
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) == null)
                    return false;
            }

            //ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);
            //return arrResult != null;
            return true;
        }

		private bool AddGroup(WebDBManager dbMgr, int nStepMemberID, Sections.SectionGroup section, ref int nGroupID, UnE.SOP.RollbackManager rollback)
		{
			GetComponentMaxID(dbMgr, "SectionGroup", ref nGroupID);
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

            sb.Append("insert into SectionGroup (ID, x, y, width, height, text, ComponentID, StepMemberID, RegionX, RegionY, RegionWidth, RegionHeight, valign, halign, ");
            sb.Append(" FontName, FontStyle, FontSize, LineSpace, FontColor ) ");
            sb.AppendFormat(" values ({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', {7}, {8}, {9}, {10}, {11}, {12}, {13},'{14}', {15}, {16}, {17}, {18} )",
			            ++nGroupID,   //0
                        x, 
                        y,
                        width,
                        height, 
                        ChangeSpecialCharacter(section.Title),  //5
                        ChangeSpecialCharacter(data.ComponentID), 
                        nStepMemberID, 
                        rx,
                        ry,
                        rwidth,   //10
                        rHeight,
                        (int)data.TextVerticalAlign,
                        (int)data.TextHorizontalAlign,
                         section.TextFont.Name,
                    (int)section.TextFont.Style,
                    section.TextFont.Size,
                    data.LineSpace,
                    (int)section.TextColor.ToArgb()
                        );
			
			string strSQL = sb.ToString();

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData("Delete from SectionGroup where ID = " + nGroupID);

                if (dbMgr.GetResultData(strSQL) != null)
                    rollback.AddData(rollbackData);
                else
                    return false;
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) == null)
                    return false;
            }

            //ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);
            //return arrResult != null;
            return true;
        }

		private bool AddGroupComponent(WebDBManager dbMgr, int nStepMemberID, Sections.SectionGroup group, int nGroupID, Dictionary<Sections.Section, int> dicComponentID, UnE.SOP.RollbackManager rollback)
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

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData("Delete from GroupComponent where GroupID = " + nGroupID);

                if (dbMgr.GetResultData(szSQL) != null)
                    rollback.AddData(rollbackData);
                else
                    return false;
            }
            else
            {
                if (dbMgr.GetResultData(szSQL) == null)
                    return false;
            }

            /*ArrayList arResult = transaction ? dbMgr.GetBatchData(szSQL) : dbMgr.GetResultData(szSQL);
            if (arResult == null)
				return false;*/
			return true;
		}

        private bool GetStepMemberTeamData(WebDBManager dbMgr, out DBUtility2.VariousData<int> nTeamID, out VariousData<int> nTeamType)
        {
            nTeamID = nTeamType = null;

            string strSQL = "select rt.ID, rt.TeamName from Site, RegularTeam as rt where Site.TeamID = rt.ID";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            //ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count != 2)
                return false;

            nTeamID = WebDBManager.GetIntField(arrResult[0].ToString());
            nTeamType = new VariousData<int>((int)Sections.SOPTeam.SOPTeamType.Regular);
            return nTeamID != null;
        }

        // Return 값 : 새로 생성된 StepMember들의 ID List
        //             저장에 실패하면 null을 리턴
        private Dictionary<System.Windows.Forms.TabPage, Dictionary<StepMemberData, int>> AddStepMembers(FormMain frm, WebDBManager dbMgr, Dictionary<System.Windows.Forms.TabPage, int> dicActionStepIDs, UnE.SOP.RollbackManager rollback)
        {
			FormPageSOP pageLevel = frm.GetPageLevel();

            string strSQL = "Select max(id) from StepMember";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            //ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

            int nStepMemberID;

            if (arrResult == null || arrResult.Count == 0)
                nStepMemberID = 0;
            else
                nStepMemberID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);

            Type panelType = typeof(Sections.PanelSectionEx);
            Dictionary<System.Windows.Forms.TabPage, Dictionary<StepMemberData, int>> dicStepMembers = new Dictionary<System.Windows.Forms.TabPage, Dictionary<StepMemberData, int>>();

            // TeamType : 0(평일 비상 조직, TemporaryNormalTeam), 1(휴일 비상 조직, TemporaryEmergencyTeam), 2(외부 기관, ExternalTeam), 3(사용자 정의 조직, UserDefinedTeam), 4(정규 조직, RegularTeam), 10(교대 근무자, ControlRoom)
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
                        Sections.SOPTeam.SOPTeamType nTeamType = panel.TeamType;

                        if (nTeamID < 0)
                        {
                            if (nTeamType == Sections.SOPTeam.SOPTeamType.UserDefined)
                                nTeamID = AddUserDefinedTeam(dbMgr, strTeamName, rollback);
                            else if (nTeamType == Sections.SOPTeam.SOPTeamType.External)
                                nTeamID = AddExternalTeam(dbMgr, strTeamName, rollback);

                            if (nTeamID < 0)
                                return null;
                        }
                        strSQL = string.Format("insert into StepMember (ID, TeamID, TeamType, ActionStepID) values ({0}, {1}, {2}, {3})",
                            ++nStepMemberID, nTeamID, (int)nTeamType, nActionStepID);

                        if (rollback != null)
                        {
                            UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData("Delete from StepMember where ID = " + nStepMemberID);

                            if (dbMgr.GetResultData(strSQL) != null)
                                rollback.AddData(rollbackData);
                            else
                                return null;
                        }
                        else
                        {
                            if (dbMgr.GetResultData(strSQL) == null)
                                return null;
                        }

                        /*arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

                        if (arrResult == null)
                            return null;*/

                        dicStepMember[new StepMemberData(strTeamName, nTeamID, nTeamType)] = nStepMemberID;
                    }
                }

                dicStepMembers[page] = dicStepMember;
            }

            return dicStepMembers;
        }

        public int AddUserDefinedTeam(WebDBManager dbMgr, string strTeamName, UnE.SOP.RollbackManager rollback)
        {
            string strSQL = "select max(id) from UserDefinedTeam";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            //ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

            int nTeamID;

            if (arrResult == null || arrResult.Count == 0)
                nTeamID = 0;
            else
                nTeamID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);

            // Site별로 사용할 수 있도록 수정 , Edit by skkim 2015.01.09
            strSQL = string.Format("Insert into UserDefinedTeam (ID, TeamName, PhoneNumber, FaxNumber, SiteID) values ({0}, '{1}', '0000000', '', {2})",
                ++nTeamID, ChangeSpecialCharacter(strTeamName), m_nSiteID);

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData("Delete from UserDefinedTeam where ID = " + nTeamID);

                if (dbMgr.GetResultData(strSQL) != null)
                    rollback.AddData(rollbackData);
                else
                    return -1;
                
                return nTeamID;
                //return dbMgr.GetBatchData(strSQL) == null ? -1 : nTeamID;
            }

            return dbMgr.GetResultData(strSQL) == null ? -1 : nTeamID;
            //return dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null ? -1 : nTeamID;
        }

		public bool DeleteUserDefinedTeam(WebDBManager dbMgr, string strTeamName, UnE.SOP.RollbackManager rollback)
		{
            string strSQL3 = string.Format("select ID from UserDefinedTeam where TeamName = '{0}' and SiteID = {1}", ChangeSpecialCharacter(strTeamName), m_nSiteID);
            ArrayList arResult = dbMgr.GetResultData(strSQL3);
            //ArrayList arResult = transaction ? dbMgr.GetBatchData(strSQL3) : dbMgr.GetResultData(strSQL3);

            if (arResult == null || arResult.Count == 0)
                return false;

            int nID = WebDBManager.GetIntField(arResult[0].ToString(), 0);
            if (nID < 0)
                return false;

            // ActionStepUsingUserDefinedTeam에서 먼저 삭제 , Edit by skkim 2015.09.03
            if (DeleteActionStepUsingTeam(dbMgr, nID, 3, rollback) == false)
                return false;

            // Site별로 사용할 수 있도록 수정 , Edit by skkim 2015.01.09
            string strCondition = string.Format("where TeamName = '{0}' and SiteID = {1}", ChangeSpecialCharacter(strTeamName), m_nSiteID);
            string strSQL = "delete from UserDefinedTeam " + strCondition;

            UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();

            if (rollbackData.AddInsertRollback(dbMgr, "Select ID, TeamName, PhoneNumber, FaxNumber, SiteID from UserDefinedTeam " + strCondition, 0, 1, 1, 1, 0) == false)
                return false;

            if (dbMgr.GetResultData(strSQL) == null)
                return false;
            else
                rollback.AddData(rollbackData);

            /*arResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);
            if (arResult == null)
                return false;*/

            return true;

            /*string strSQL2 = string.Format("select * from UserDefinedTeam where TeamName = '{0}' and SiteID = {1}", ChangeSpecialCharacter(strTeamName), m_nSiteID);
			arResult = dbMgr.GetResultData(strSQL2);

			if (arResult == null || arResult.Count == 0)
			{
				return true;
			}
			return false;*/
		}

        public static bool DeleteActionStepUsingTeam(WebDBManager dbMgr, string strTeamIDs, int nTeamType)
        {
            string strSQL = string.Format("delete from ActionStepUsingTeam where TeamID in ({0}) and TeamType = {1}", strTeamIDs, nTeamType);
            return dbMgr.GetResultData(strSQL) != null;
        }

        public static bool DeleteActionStepUsingTeam(WebDBManager dbMgr, int nTeamID, int nTeamType, UnE.SOP.RollbackManager rollback)
        {
            string strCondition = string.Format("where TeamID = {0} and TeamType = {1}", nTeamID, nTeamType);
            string strSQL = "delete from ActionStepUsingTeam " + strCondition;

            UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();

            if (rollbackData.AddInsertRollback(dbMgr, "Select ID, ActionStepHistoryID, TeamType, TeamID, PhoneNumber, UserName, Role, JobName, AllMembers from ActionStepUsingTeam " + strCondition, 0, 0, 0, 0, 1, 1, 1, 1, 0) == false)
                return false;

            if (dbMgr.GetResultData(strSQL) == null)
                return false;
            else
                rollback.AddData(rollbackData);

            //ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);
            //return arrResult != null;
            return true;
        }

        public int AddExternalTeam(WebDBManager dbMgr, string strTeamName, UnE.SOP.RollbackManager rollback)
        {
            string strSQL = "select max(id) from ExternalTeam";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            //ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

            int nTeamID;

            if (arrResult == null || arrResult.Count == 0)
                nTeamID = 0;
            else
                nTeamID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);
            
            // Site별로 사용할 수 있도록 수정 , Edit by skkim 2015.01.09
            strSQL = string.Format("Insert into ExternalTeam (ID, TeamName, PhoneNumber, FaxNumber, SiteID) values ({0}, '{1}', '0000000', '', {2})",
                ++nTeamID, ChangeSpecialCharacter(strTeamName), m_nSiteID);

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData("Delete from ExternalTeam where ID = " + nTeamID);

                if (dbMgr.GetResultData(strSQL) != null)
                    rollback.AddData(rollbackData);
                else
                    return -1;

                return nTeamID;
                //return dbMgr.GetBatchData(strSQL) == null ? -1 : nTeamID;
            }

            return dbMgr.GetResultData(strSQL) == null ? -1 : nTeamID;
            //return dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null ? -1 : nTeamID;
        }

		public bool DeleteExternalTeam(WebDBManager dbMgr, string strTeamName, bool transaction)
		{
            // Site별로 사용할 수 있도록 수정 , Edit by skkim 2015.01.09
            string strSQL = string.Format("Select ID from ExternalTeam where TeamName = '{0}' and SiteID = {1}", ChangeSpecialCharacter(strTeamName), m_nSiteID);

			ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            string strIDs = "";

            foreach (object result in arrResult)
            {
                VariousData<int> id = WebDBManager.GetIntField(result.ToString());

                if (id != null)
                {
                    if (strIDs.Length == 0)
                        strIDs = id.Data.ToString();
                    else
                        strIDs += ", " + id.Data.ToString();
                }
            }

            if (strIDs.Length > 0)
            {
                if (DeleteActionStepUsingTeam(dbMgr, strIDs, 2) == false)
                    return false;

                strSQL = "Delete from ExternalTeam where ID in (" + strIDs + ")";

                if (dbMgr.GetResultData(strSQL) == null)
                    return false;
            }

            return true;
            // Site별로 사용할 수 있도록 수정 , Edit by skkim 2015.01.09
            /*string strSQL2 = string.Format("select * from ExternalTeam where TeamName = '{0}' and SiteID = {1}", ChangeSpecialCharacter(strTeamName), m_nSiteID);
			ArrayList arResult = dbMgr.GetResultData(strSQL2);

			if (arResult == null || arResult.Count == 0)
			{
				return true;
			}
			return false;*/
		}

        // Return 값 : 새로 생성된 ActionStep들의 ID List
        //             저장에 실패하면 null을 리턴
        public Dictionary<System.Windows.Forms.TabPage, int> AddActionSteps(FormMain frm, WebDBManager dbMgr, int nDisasterID, string strUserDefinedConfigName, UnE.SOP.RollbackManager rollback)
        {
			FormPageSOP pageLevel = frm.GetPageLevel();
            
            string strSQL = "Select max(id) from ActionStep";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            //ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

            int nActionStepID;

            if (arrResult == null || arrResult.Count == 0)
                nActionStepID = 0;
            else
                nActionStepID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);

            string strUserDefinedConfigID = "NULL";

            if (strUserDefinedConfigName != null || strUserDefinedConfigName.Length > 0)
            {
                strSQL = "Select ID from UserDefinedConfig where lower(ConfigName) = '" + strUserDefinedConfigName.ToLower() + "'";
                arrResult = dbMgr.GetResultData(strSQL);
                //arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

                if (arrResult != null && arrResult.Count > 0)
                {
                    VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

                    if (id != null)
                        strUserDefinedConfigID = id.Data.ToString();
                }
                else
                {
                    ConfigData config = null;
                    List<SOPParameter> variables = pageLevel.GetBarConfig().GetCurrentVariables(out config);

                    if (variables != null && config != null)
                    {
                        int nID = AddUserDefinedConfig(dbMgr, config, variables, rollback);

                        if (nID < 0)
                            return null;

                        strUserDefinedConfigID = nID.ToString();
                    }
                }
            }

            string strBeginTime, strEndTime;

            // TabPage별 ActionStepID
            Dictionary<System.Windows.Forms.TabPage, int> dicActionStepID = new Dictionary<System.Windows.Forms.TabPage, int>();

            // ActionStepID별 부모 TabPage
            Dictionary<int, TabPage> dicParentTabPage = new Dictionary<int, TabPage>();

            foreach (ActionStepTabPage page in pageLevel.TabControls.TabPages)
            {
                string strStepName = page.Text;				

				Data_ActionStep opt = page.Data;
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

                StringBuilder sb = new StringBuilder();
                sb.Append("insert into ActionStep (ID, StepName, PeriodType, BeginTime, EndTime, WeekDayOption, ");
                sb.Append(" Iteration, IterationType, ProcessTime, ProcessTimeType, DisasterID, ParentStepID, UserDefinedConfigID) ");
                sb.AppendFormat(" values ({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, NULL, {11})",
                     ++nActionStepID,
                     strStepName,
                     opt.PeriodType,
                     strBeginTime,
                     strEndTime, 
                     opt.WeekdayOption, 
                     opt.Iteration, 
                     opt.IterationType,
                     opt.ProcessTime, 
                     opt.ProcessTimeType,
                     nDisasterID,
                     strUserDefinedConfigID);

                strSQL = sb.ToString();

                if (rollback != null)
                {
                    UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData("Delete from ActionStep where GroupID = " + nActionStepID);

                    if (dbMgr.GetResultData(strSQL) != null)
                        rollback.AddData(rollbackData);
                    else
                        return null;
                }
                else
                {
                    if (dbMgr.GetResultData(strSQL) == null)
                        return null;
                }

                /*arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

                if (arrResult == null)
                    return null;*/

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

                string strCondition = string.Format("where id = {0}", nID);
                strSQL = string.Format("Update ActionStep set ParentStepID = {0} {1}", nParentID, strCondition);

                if (rollback != null)
                {
                    UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();

                    if (rollbackData.AddUpdateRollback(dbMgr, "Select ParentStepID from ActionStep " + strCondition, 0) == false)
                        return null;

                    if (dbMgr.GetResultData(strSQL) != null)
                        rollback.AddData(rollbackData);
                    else
                        return null;
                }
                else
                {
                    if (dbMgr.GetResultData(strSQL) == null)
                        return null;
                }

                /*arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

                if (arrResult == null)
                    return null;*/
            }

            return dicActionStepID;
        }

        private int AddUserDefinedConfig(WebDBManager dbMgr, ConfigData config, List<SOPParameter> variables, UnE.SOP.RollbackManager rollback)
        {
            if (config == null)
                return -1;

            int nID = FormMain.Instance.GetMaxTableID("UserDefinedConfig") + 1;
            string strSQL = string.Format("Insert into UserDefinedConfig (ID, ConfigName, Description) values ({0}, '{1}', NULL)", nID, config.Text);

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData("Delete from UserDefinedConfig where ID = " + nID);

                if (dbMgr.GetResultData(strSQL) != null)
                    rollback.AddData(rollbackData);
                else
                    return -1;
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) == null)
                    return -1;
            }

            /*ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return -1;*/

            for (int i=0;i<variables.Count;i++)
            {
                SOPParameter param = variables[i];
                string strDescription = param.Description == null ? "''" : "'" + param.Description + "'";

                strSQL = string.Format("Insert into UserDefinedConfigVariable (ConfigID, No, VariableName, VariableType, Description) values ({0}, {1}, '{2}', {3}, {4})",
                    nID, i + 1, param.VariableName, (int)param.Type, strDescription);

                if (rollback != null)
                {
                    UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData(string.Format("Delete from UserDefinedConfigVariable where ConfigID = {0} and No = {1}", nID, i + 1));

                    if (dbMgr.GetResultData(strSQL) != null)
                        rollback.AddData(rollbackData);
                    else
                        return -1;
                }
                else
                {
                    if (dbMgr.GetResultData(strSQL) == null)
                        return -1;
                }

                /*arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

                if (arrResult == null)
                    return -1;*/
            }

            config.ID = nID;
            return nID;
        }

        // Return 값 : Disaster ID
        //             이 값이 0보다 작으면 실패
        private int AddDisaster(WebDBManager dbMgr, int nVersionID, UnE.SOP.RollbackManager rollback)
        {
			string strDisaster = SopDocManager.Instance.DisasterName;
			string strSubDisaster = SopDocManager.Instance.SubCategoryName;
			string strCategory = SopDocManager.Instance.CategoryName;
			string strDescription = SopDocManager.Instance.DisasterDescription;

            if (strDisaster == "" || strSubDisaster == "" || strCategory == "")
                return -1;

            // Site별로 사용할 수 있도록 수정 , Edit by skkim 2015.01.09
            string strSQL = string.Format("Select id from SubDisasterCategory where SubCategoryName = '{0}' and DisasterID in (select id from DisasterCategory where CategoryName = '{1}' and SiteID = {2})",
                strSubDisaster, strCategory, FormMain.Instance.SiteID );
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            //ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nSubCategoryID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);
            if (nSubCategoryID == 0)
                return -1;

            strSQL = string.Format("select max(id) from Disaster");
            arrResult = dbMgr.GetResultData(strSQL);
            //arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

            int nDisasterID;

            if (arrResult == null || arrResult.Count == 0)
                nDisasterID = 0;
            else
                nDisasterID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);

            strSQL = string.Format("INSERT INTO Disaster(ID, DisasterName, SubDisasterID, VersionID, Description) VALUES ({0}, '{1}', {2}, {3}, '{4}')",
				++nDisasterID, ChangeSpecialCharacter(strDisaster), nSubCategoryID, nVersionID, ChangeSpecialCharacter(strDescription));

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData("Delete from Disaster where ID = " + nDisasterID);

                if (dbMgr.GetResultData(strSQL) == null)
                    return -1;
                else
                    rollback.AddData(rollbackData);
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) == null)
                    return -1;
            }


            /*arrResult = transaction? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return -1;*/

            return nDisasterID;
        }

        /*private bool UpdateVersion(WebDBManager dbMgr, int nVersionID, string strDescription, ref VersionInfo rVersion)
        {
            DateTime dtCurrent = DateTime.Now;
            string strSQL = string.Format("update Version set LastAccessTime = '{0} {1:00}:{2:00}:{3:00}', Description ='{5}' where id = {4}",
                dtCurrent.ToShortDateString(), dtCurrent.Hour, dtCurrent.Minute, dtCurrent.Second, nVersionID, strDescription);

            if (dbMgr.GetBatchData(strSQL) == null)
                return false;

            strSQL = string.Format("select CreateTime, LastAccessTime, VersionName, Description from Version where id = {0}", nVersionID);
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

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
        }*/

        // Return 값 : 저장된 Version의 ID
        //             저장에 실패하며 -1을 리턴
        private int SaveVersion(FormMain frm, WebDBManager dbMgr, string strVersionName, int nVersionID, int nSOPGenUserID, string strDescription, ref VersionInfo rVersion, UnE.SOP.RollbackManager rollback)
        {
            string strSQL;
            ArrayList arrResult;

            if (nVersionID <= 0)
            {
                strSQL = "select max(id) from version";
                arrResult = dbMgr.GetResultData(strSQL);
                //arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

                if (arrResult == null)
                    return -1;

                if (arrResult.Count == 0)
                    nVersionID = 1;
                else
                    nVersionID = WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;
            }
              
            DateTime dtCurrent = DateTime.Now;
            int nRegular = SopDocManager.Instance.RegularMode ? 1 : 0;
			int nNormal = SopDocManager.Instance.WeekMode ? 1 : 0;

            string strCurrentTime = dtCurrent.ToShortDateString() + string.Format(" {0:00}:{1:00}:{2:00}", dtCurrent.Hour, dtCurrent.Minute, dtCurrent.Second);
            
            // Site별로 사용할 수 있도록 수정 , Edit by skkim 2015.01.09
            strSQL = string.Format("INSERT INTO Version (ID, isRegular, isNormal, CreateTime, LastAccessTime, VersionName, OwnerID, SiteID, Description) VALUES ({0}, {1}, {2}, '{3}', '{4}', '{5}', {6}, {7}, '{8}')",
                nVersionID, nRegular, nNormal, strCurrentTime, strCurrentTime, ChangeSpecialCharacter(strVersionName), nSOPGenUserID, m_nSiteID, ChangeSpecialCharacter(strDescription));

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData("Delete from Version where ID = " + nVersionID);

                if (dbMgr.GetResultData(strSQL) == null)
                    return -1;
                else
                    rollback.AddData(rollbackData);
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) == null)
                    return -1;
            }

            /*ArrayList arResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

            if (arResult == null)// || arResult.Count == 0)
                return -1;*/

            rVersion.BeginTime = dtCurrent;
            rVersion.Description = strDescription;
            rVersion.EndTime = dtCurrent;
            rVersion.VersionID = nVersionID;
            rVersion.VersionName = strVersionName;

            return nVersionID;
        }
				
		public bool IsMonitoringDiaster(WebDBManager dbMgr, string szDisasterName)
		{
			if (szDisasterName == null || szDisasterName == "")
				return false;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT dis.ID FROM Disaster as dis ");
            sb.Append(" INNER JOIN ActionStep as step ON step.DisasterID = dis.ID ");
            sb.Append(" INNER JOIN ActionStepHistory as ash ON step.ID = ash.ActionStepID AND ash.EndTime is null AND ash.CancelTime is null ");
            sb.Append(" INNER JOIN SubDisasterCategory as sdc ON dis.SubDisasterID = sdc.ID ");
            sb.Append(" INNER JOIN DisasterCategory as dc ON sdc.DisasterID = dc.ID and dc.SiteID = {0} ");
            sb.Append(" WHERE dis.DisasterName = '{1}'");

            string szSQL = string.Format(sb.ToString(),  m_nSiteID, szDisasterName);

			ArrayList arrResult = dbMgr.GetResultData(szSQL);
			if (arrResult == null)
				return false;

			if (arrResult.Count == 0)
				return false;

			return true;
		}

		// 해당 버전의 SOP가 현재 모니터링에서 사용중인 여부
		public bool IsMonitoringSOPVersion(WebDBManager dbMgr, int nVersionID, UnE.SOP.RollbackManager rollback)
		{
			if (nVersionID < 0)
				return false;

            string szText = "SELECT dis.ID, step.ID as ActionStepID FROM Disaster as dis, ActionStep as step, ActionStepHistory as ash " +
                            " WHERE dis.VersionID = {0} and step.DisasterID = dis.ID and step.ID = ash.ActionStepID and ash.EndTime is null and ash.CancelTime is null";
            string strSQL = string.Format(szText, nVersionID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            //ArrayList arrResult = bTransaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);
            if (arrResult == null)
				return false;
			if (arrResult.Count == 0)
				return false;

			
            string szText2 = "SELECT ID FROM Disaster WHERE versionID = {0} and id not in (select dis.ID FROM Disaster as dis, ActionStep as step, ActionStepHistory as ash " +
                             " WHERE step.DisasterID = dis.ID and step.ID = ash.ActionStepID and ( ash.EndTime is null and ash.CancelTime is null ))";
            string strSQL2 = string.Format(szText2, nVersionID);
            ArrayList arrResult2 = dbMgr.GetResultData(strSQL2);
            //ArrayList arrResult2 = bTransaction ? dbMgr.GetBatchData(strSQL2) : dbMgr.GetResultData(strSQL2);
            if (arrResult2 == null)
				return true;

			if (arrResult2.Count == 0)
				return true;

			return false;
		}

        // 기존 버전을 삭제
        public bool DeleteSOPVersion(WebDBManager dbMgr, int nVersionID, bool deleteVersion, UnE.SOP.RollbackManager rollback, bool noCommit = false)
        {
			// 기존에 실행중인 버전은 삭제 되지 않도록 검사하여 id를 가져오지 못하도록 한다.
			// 수정 : 2014-11-13 skkim
			// 모니터링 중인 SOP의 처리방법
			// - 삭제 : 삭제하지 못하도록 막는다.
			// - 저장 : 반드시 새버전으로 저장하도록 한다.
            if (IsMonitoringSOPVersion(dbMgr, nVersionID, rollback))
			{
				return false;
			}
			
			string strSQL = "select id from Disaster where VersionID = " + nVersionID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            //ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            if (arrResult.Count == 0)
            {
                if (deleteVersion)
                    return DeleteVersion(dbMgr, nVersionID, rollback);
                return true;
            }

            string strDisasterIDs = "";

            foreach (object obj in arrResult)
            {
                string strID = WebDBManager.GetStringField(obj);
                //string strID = obj.ToString();

                if (strID == null)
                    continue;

                if (strDisasterIDs.Length == 0)
                    strDisasterIDs = strID;
                else
                    strDisasterIDs += ", " + strID;
            }

            strSQL = string.Format("select id from ActionStep where DisasterID in ({0})", strDisasterIDs);
            arrResult = dbMgr.GetResultData(strSQL);
            //arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

            if (arrResult.Count == 0)
            {
                if (!DeleteDisaster(dbMgr, nVersionID, rollback))
                    return false;

                if (deleteVersion)
                    return DeleteVersion(dbMgr, nVersionID, rollback);
                return true;
            }

            string strActionStepIDs = "";

            foreach (object obj in arrResult)
            {
                string strActionStepID = WebDBManager.GetStringField(obj);

                if (strActionStepID == null)
                    continue;

                if (strActionStepIDs.Length == 0)
                    strActionStepIDs = strActionStepID;
                else
                    strActionStepIDs += ", " + strActionStepID;
            }
                        
            strSQL = string.Format("select id from StepMember where ActionStepID in ({0})", strActionStepIDs);
            arrResult = dbMgr.GetResultData(strSQL);
            //arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            if (arrResult.Count == 0)
            {
                if (!DeleteActionStepHistory(dbMgr, strActionStepIDs, rollback))
                    return false;
                if (!DeleteActionStep(dbMgr, strDisasterIDs, rollback))
                    return false;
                if (!DeleteDisaster(dbMgr, nVersionID, rollback))
                    return false;
                if (deleteVersion)
                    return DeleteVersion(dbMgr, nVersionID, rollback);

                FormMain.Instance.SendDeletedActionStepIDs(strActionStepIDs);
                return true;
            }

            string strStepMemberIDs = "";

            foreach (object obj in arrResult)
            {
                string strStepMemberID = WebDBManager.GetStringField(obj);

                if (strStepMemberID == null)
                    continue;

                if (strStepMemberIDs.Length == 0)
                    strStepMemberIDs = strStepMemberID;
                else
                    strStepMemberIDs += ", " + strStepMemberID;
            }

            /*if (transaction == true)
            {
                dbMgr.BeginBatch();
            }*/

            if (strStepMemberIDs.Length > 0)
            {
                if (!DeleteComponent(dbMgr, strStepMemberIDs, rollback))
                {
                    /*if (noCommit == false && transaction)
                        dbMgr.BatchRollback();*/
                    return false;
                }
            }
            if (!DeleteActionStepHistory(dbMgr, strActionStepIDs, rollback))
            {
                //if (noCommit == false && transaction)
                //    dbMgr.BatchRollback();
                return false;
            }
            if (!DeleteStepMember(dbMgr, strActionStepIDs, rollback))
            {
                //if (noCommit == false && transaction)
                //    dbMgr.BatchRollback();
                return false;
            }
            if (!DeleteActionStep(dbMgr, strDisasterIDs, rollback))
            {
                //if (noCommit == false && transaction)
                //    dbMgr.BatchRollback();
                return false;
            }
            if (!DeleteDisaster(dbMgr, nVersionID, rollback))
            {
                //if (noCommit == false && transaction)
                //    dbMgr.BatchRollback();
                return false;
            }
            if (deleteVersion)
            {
                if (!DeleteVersion(dbMgr, nVersionID, rollback))
                {
                    //if (noCommit == false && transaction)
                    //    dbMgr.BatchRollback();
                    return false;
                }
            }

            /*if (transaction == true)
            {
                if (noCommit == false)
                    dbMgr.BatchCommit();
            }*/

            FormMain.Instance.SendDeletedActionStepIDs(strActionStepIDs);            
            return true;
        }

        private bool DeleteVersion(WebDBManager dbMgr, int nVersionID, UnE.SOP.RollbackManager rollback)
        {
            string strSQL = "delete from Version where id = " + nVersionID.ToString();

            if (rollback != null)
            //if (transaction)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();

                if (rollbackData.AddInsertRollback(dbMgr, "Select ID, isRegular, isNormal, CreateTime, LastAccessTime, VersionName, OwnerID, Description, SiteID from Version where ID = " + nVersionID, 0, 0, 0, 1, 1, 1, 0, 1, 0) == false)
                    return false;

                if (dbMgr.GetResultData(strSQL) != null)
                {
                    rollback.AddData(rollbackData);
                    return true;
                }
                else
                    return false;
                //return dbMgr.GetBatchData(strSQL) == null ? false : true;
            }

            return dbMgr.GetResultData(strSQL) == null ? false : true;
        }

        private bool DeleteDisaster(WebDBManager dbMgr, int nVersionID, UnE.SOP.RollbackManager rollback)
        {
            DeleteDisasterOwner(dbMgr, nVersionID, rollback);
            DeleteVersionLevelDisaster(dbMgr, nVersionID, rollback);

            string strSQL = "delete from Disaster where VersionID = " + nVersionID.ToString();

            if (rollback != null)
            //if (transaction)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();

                if (rollbackData.AddInsertRollback(dbMgr, "Select ID, DisasterName, SubDisasterID, VersionID, Description from Disaster where VersionID = " + nVersionID, 0, 1, 0, 0, 1) == false)
                    return false;

                if (dbMgr.GetResultData(strSQL) != null)
                {
                    rollback.AddData(rollbackData);
                    return true;
                }
                else
                    return false;

                //return dbMgr.GetBatchData(strSQL) == null ? false : true;
            }

            return dbMgr.GetResultData(strSQL) == null ? false : true;
        }

        // Site에 따라서 SOPGenLevelDisaster Table이 없는 경우도 있다.
        // DB 쿼리가 실패하여도 상관하지 않는다.
        private void DeleteVersionLevelDisaster(WebDBManager dbMgr, int nVersionID, UnE.SOP.RollbackManager rollback)
        {
            if (FormMain.Instance.LevelDisasterOption == LevelDisasterOption.Use)
            {
                string strSQL = "Select LevelID, DisasterID from SOPGenLevelDisaster where DisasterID in (";
                strSQL += "Select d.ID from Disaster as d, Version as v ";
                strSQL += string.Format("where d.VersionID = v.ID and v.ID = {0})", nVersionID);

                ArrayList arrResult = dbMgr.GetResultData(strSQL);

                if (arrResult == null)
                    return;

                int nResultCount = arrResult.Count;
                int nDisasterID = -1;
                List<UnE.SOP.RollbackData> insertRollbackDatas = new List<UnE.SOP.RollbackData>();

                for (int i=0;i<nResultCount-1;i+=2)
                {
                    VariousData<int> levelID = WebDBManager.GetIntField(arrResult[i].ToString());
                    VariousData<int> disasterID = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                    if (levelID == null || disasterID == null)
                        continue;

                    nDisasterID = disasterID.Data;

                    string strInsert = "Insert into SOPGenLevelDisaster (LevelID, DisasterID) values (";
                    strInsert += string.Format("{0}, {1})", levelID.Data, disasterID.Data);

                    UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData(strInsert);
                    insertRollbackDatas.Add(rollbackData);
                }

                if (nDisasterID < 0)
                    return;

                strSQL = "Delete from SOPGenLevelDisaster where DisasterID = " + nDisasterID.ToString();

                if (dbMgr.GetResultData(strSQL) != null)
                {
                    if (rollback != null)
                    {
                        foreach (UnE.SOP.RollbackData data in insertRollbackDatas)
                        {
                            rollback.AddData(data);
                        }
                    }
                }
            }
        }

        // Site에 따라서 DisasterOwner Table이 없는 경우도 있다.
        // DB 쿼리가 실패하여도 상관하지 않는다.
        private void DeleteDisasterOwner(WebDBManager dbMgr, int nVersionID, UnE.SOP.RollbackManager rollback)
        {
            string strCondition = string.Format("where DisasterID in (Select ID from Disaster where VersionID = {0})", nVersionID);
            string strSQL = "Delete from DisasterOwner " + strCondition;

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();

                if (rollbackData.AddInsertRollback(dbMgr, "Select DisasterID, BuildingID from DisasterOwner " + strCondition, 0, 0) == false)
                    return;

                if (dbMgr.GetResultData(strSQL) != null)
                {
                    rollback.AddData(rollbackData);
                }
                //dbMgr.GetBatchData(strSQL);
            }
            else
                dbMgr.GetResultData(strSQL);
        }

        private bool DeleteActionStepHistory(WebDBManager dbMgr, string strActionStepIDs, UnE.SOP.RollbackManager rollback)
        {
            string strSQL = string.Format("select id from ActionStepHistory where ActionStepID in ({0})", strActionStepIDs);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            //ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

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

            strSQL = string.Format("select id from ComponentHistory where ActionStepHistoryID in ({0})", strActionStepHistoryIDs);
            arrResult = dbMgr.GetResultData(strSQL);
            //arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            string strComponentHistoryIDs = "";
            nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount; i++)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);

                if (strComponentHistoryIDs.Length == 0)
                    strComponentHistoryIDs = nID.ToString();
                else
                    strComponentHistoryIDs += ", " + nID.ToString();
            }
            
            if (strComponentHistoryIDs != null && strComponentHistoryIDs != "")
            {
                string strCondition = string.Format("where ComponentHistoryID in ({0})", strComponentHistoryIDs);
                strSQL = "delete from ComponentHistoryDetail " + strCondition;

                if (rollback != null)
                {
                    UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();

                    if (rollbackData.AddInsertRollback(dbMgr, "Select ID, ComponentHistoryID, DataIndex, Datai, Dataf, Datas, Time from ComponentHistoryDetail " + strCondition, 0, 0, 0, 0, 0, 1, 1) == false)
                        return false;

                    if (dbMgr.GetResultData(strSQL) != null)
                    {
                        rollback.AddData(rollbackData);
                    }
                    else
                        return false;
                    //if (dbMgr.GetBatchData(strSQL) == null)
                    //    return false;
                }
                else
                {
                    if (dbMgr.GetResultData(strSQL) == null)
                        return false;
                }

                //if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                //    return false;
            }

            if (strActionStepHistoryIDs != null && strActionStepHistoryIDs != "")
            {
                string strCondition = string.Format("where ActionStepHistoryID in ({0})", strActionStepHistoryIDs);
                strSQL = "delete from ComponentHistory " + strCondition;

                if (rollback != null)
                {
                    UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();
                    string query = "Select ID, ActionStepHistoryID, ComponentID, ComponentType, Time, Status, Task, CompleteCount, ShowBoard, AccessedUserID, CheckedNotify1, CheckedNotify2, Description, CheckedRun, CheckedComplete from ComponentHistory ";

                    if (rollbackData.AddInsertRollback(dbMgr, query + strCondition, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0) == false)
                        return false;

                    if (dbMgr.GetResultData(strSQL) != null)
                    {
                        rollback.AddData(rollbackData);
                    }
                    else
                        return false;
                    //if (dbMgr.GetBatchData(strSQL) == null)
                    //    return false;
                }
                else
                {
                    if (dbMgr.GetResultData(strSQL) == null)
                        return false;
                }
                //if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                //    return false;

                // Delete UsingUserDefinedTeam 추가
                strSQL = string.Format("delete from ActionStepUsingTeam where ActionStepHistoryID in ({0})", strActionStepHistoryIDs);

                if (rollback != null)
                {
                    UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();
                    string query = "Select ID, ActionStepHistoryID, TeamType, TeamID, PhoneNumber, UserName, Role, JobName, AllMembers from ActionStepUsingTeam ";
                    strCondition = string.Format("where ActionStepHistoryID in ({0})", strActionStepHistoryIDs);

                    if (rollbackData.AddInsertRollback(dbMgr, query + strCondition, 0, 0, 0, 0, 1, 1, 1, 1, 0) == false)
                        return false;

                    if (dbMgr.GetResultData(strSQL) != null)
                    {
                        rollback.AddData(rollbackData);
                    }
                    else
                        return false;
                    //if (dbMgr.GetBatchData(strSQL) == null)
                    //    return false;
                }
                else
                {
                    if (dbMgr.GetResultData(strSQL) == null)
                        return false;
                }
                //if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                //    return false;
                
                strSQL = string.Format("delete from Message where ActionStepHistoryID in ({0})", strActionStepHistoryIDs);

                if (rollback != null)
                {
                    UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();
                    string query = "Select ID, SendTime, Message, MemberID, ActionStepID, ActionStepHistoryID from Message ";
                    strCondition = string.Format("where ActionStepHistoryID in ({0})", strActionStepHistoryIDs);

                    if (rollbackData.AddInsertRollback(dbMgr, query + strCondition, 0, 1, 1, 0, 0, 0) == false)
                        return false;

                    if (dbMgr.GetResultData(strSQL) != null)
                    {
                        rollback.AddData(rollbackData);
                    }
                    else
                        return false;
                    //if (dbMgr.GetBatchData(strSQL) == null)
                    //    return false;
                }
                else
                {
                    if (dbMgr.GetResultData(strSQL) == null)
                        return false;
                }
                //if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                //    return false;

                strSQL = string.Format("delete from ActionStepAutoClose where ActionStepHistoryID in ({0})", strActionStepHistoryIDs);

                if (rollback != null)
                {
                    UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();
                    string query = "Select ID, ActionStepHistoryID, ActionStepID, UseCloseNoInput, UseCloseSensorReset, UseCloseSensorResetWaitTime, InputWaitTime, SensorResetWaitTime, BeginTime, SensorZoneID, SensorZoneHistoryID, Description from ActionStepAutoClose ";
                    strCondition = string.Format("where ActionStepHistoryID in ({0})", strActionStepHistoryIDs);

                    if (rollbackData.AddInsertRollback(dbMgr, query + strCondition, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1) == false)
                        return false;

                    if (dbMgr.GetResultData(strSQL) != null)
                    {
                        rollback.AddData(rollbackData);
                    }
                    else
                        return false;
                    //if (dbMgr.GetBatchData(strSQL) == null)
                    //    return false;
                }
                else
                {
                    if (dbMgr.GetResultData(strSQL) == null)
                        return false;
                }
                //if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                //    return false;

                strSQL = string.Format("delete from ActionStepHistory where ID in ({0})", strActionStepHistoryIDs);

                if (rollback != null)
                {
                    UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();
                    string query = "Select ID, ActionStepID, RealMode, BeginTime, EndTime, CancelTime, PausedTime, DetectTime, Position, LastAccessedUserID, Description, SelectedComponentID, SelectedComponentType, StartOption, DisasterOption, SensorZoneHistoryID from ActionStepHistory ";
                    strCondition = string.Format("where ID in ({0})", strActionStepHistoryIDs);

                    if (rollbackData.AddInsertRollback(dbMgr, query + strCondition, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 0, 1, 0) == false)
                        return false;

                    if (dbMgr.GetResultData(strSQL) != null)
                    {
                        rollback.AddData(rollbackData);
                    }
                    else
                        return false;
                    //if (dbMgr.GetBatchData(strSQL) == null)
                    //    return false;
                }
                else
                {
                    if (dbMgr.GetResultData(strSQL) == null)
                        return false;
                }
                //if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                //    return false;
            }    
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

        private bool DeleteActionStep(WebDBManager dbMgr, string strDisasterIDs, UnE.SOP.RollbackManager rollback)
        {
            string strCondition = string.Format("where ActionStepID in (select id from ActionStep where DisasterID in ({0}))", strDisasterIDs);
            string strSQL = "delete from Message " + strCondition;

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();
                string query = "Select ID, SendTime, Message, MemberID, ActionStepID, ActionStepHistoryID from Message ";

                if (rollbackData.AddInsertRollback(dbMgr, query + strCondition, 0, 1, 1, 0, 0, 0) == false)
                    return false;

                if (dbMgr.GetResultData(strSQL) != null)
                {
                    rollback.AddData(rollbackData);
                }
                else
                    return false;
                //if (dbMgr.GetBatchData(strSQL) == null)
                //    return false;
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) == null)
                    return false;
            }
            //if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
            //    return false;

            strSQL = string.Format("delete from ActionStep where DisasterID in ({0})", strDisasterIDs);

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();
                string query = "Select ID, StepName, PeriodType, BeginTime, EndTime, WeekDayOption, Iteration, IterationType, ProcessTime, ProcessTimeType, DisasterID, ParentStepID, UserDefinedConfigID from ActionStep ";
                strCondition = string.Format("where DisasterID in ({0})", strDisasterIDs);

                if (rollbackData.AddInsertRollback(dbMgr, query + strCondition, 0, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0) == false)
                    return false;

                if (dbMgr.GetResultData(strSQL) != null)
                {
                    rollback.AddData(rollbackData);
                    return true;
                }
                else
                    return false;
                //return dbMgr.GetBatchData(strSQL) == null ? false : true;
            }

            return dbMgr.GetResultData(strSQL) == null ? false : true;
            //return dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null ? false : true;
        }

        private bool DeleteStepMember(WebDBManager dbMgr, string strActionStepIDs, UnE.SOP.RollbackManager rollback)
        {
            string strCondition = string.Format("where ActionStepID in ({0})", strActionStepIDs);
            string strSQL = "delete from StepMember " + strCondition;

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();

                if (rollbackData.AddInsertRollback(dbMgr, "Select ID, TeamID, TeamType, ActionStepID from StepMember " + strCondition, 0, 0, 0, 0) == false)
                    return false;

                if (dbMgr.GetResultData(strSQL) != null)
                {
                    rollback.AddData(rollbackData);
                }
                else
                    return false;
                //return dbMgr.GetBatchData(strSQL) == null ? false : true;
            }

            return dbMgr.GetResultData(strSQL) == null ? false : true;
            //return dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null ? false : true;
        }

        private bool DeleteComponent(WebDBManager dbMgr, string strStepMemberIDs, UnE.SOP.RollbackManager rollback)
        {
            string strSQL = string.Format("select id from Process where StepMemberID in ({0})", strStepMemberIDs);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            //ArrayList arrResult = transaction ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            string strProcessIDs = "";

            foreach (object obj in arrResult)
            {
                string strID = WebDBManager.GetStringField(obj);

                if (strID == null)
                    continue;

                if (strProcessIDs.Length == 0)
                    strProcessIDs = strID;
                else
                    strProcessIDs += ", " + strID;
            }

            //if (strProcessIDs.Length > 0)
            //{
            //    strSQL = string.Format("delete from CheckTask where ProcessID in ({0})", strProcessIDs);
            //    if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
            //        return false;
            //}

            if (strProcessIDs.Length > 0)
            {
                string strCondition2 = string.Format("where ProcessID in ({0})", strProcessIDs);
                strSQL = "delete from ProcessMission " + strCondition2;

                if (rollback != null)
                {
                    UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();
                    string query = "Select ID, missionText, processID, TransmissionType, missionTarget, CommanderDisplayText, CommanderMemberType, CommanderMemberID from ProcessMission ";

                    if (rollbackData.AddInsertRollback(dbMgr, query + strCondition2, 0, 1, 0, 0, 1, 1, 0, 0) == false)
                        return false;

                    if (dbMgr.GetResultData(strSQL) != null)
                    {
                        rollback.AddData(rollbackData);
                    }
                    else
                        return false;
                    //if (dbMgr.GetBatchData(strSQL) == null)
                    //    return false;
                }
                else
                {
                    if (dbMgr.GetResultData(strSQL) == null)
                        return false;
                }
                //if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                //    return false;
            }

            string strCondition = string.Format("where StepMemberID in ({0})", strStepMemberIDs);
            strSQL = "delete from Process " + strCondition;

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();
                string query = "Select ID, x, y, width, height, text, TeamList, ComponentID, ProcessTime, ProcessTimeType, useProcessTime, useMissionMessage, onlyTeamLeader, StepMemberID, CommanderMemberType, CommanderMemberID, CommanderDisplayText, valign, halign, FontName, FontStyle, FontSize, LineSpace, FontColor, AutoRun from Process ";

                if (rollbackData.AddInsertRollback(dbMgr, query + strCondition, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0) == false)
                    return false;

                if (dbMgr.GetResultData(strSQL) != null)
                {
                    rollback.AddData(rollbackData);
                }
                else
                    return false;
                //if (dbMgr.GetBatchData(strSQL) == null)
                //    return false;
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) == null)
                    return false;
            }
            //if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
            //    return false;

            strSQL = "delete from Annotation " + strCondition;

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();
                string query = "Select ID, x, y, width, height, text, ComponentID, StepMemberID, valign, halign, FontName, FontStyle, FontSize, LineSpace, FontColor from Annotation ";

                if (rollbackData.AddInsertRollback(dbMgr, query + strCondition, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0) == false)
                    return false;

                if (dbMgr.GetResultData(strSQL) != null)
                {
                    rollback.AddData(rollbackData);
                }
                else
                    return false;
                //if (dbMgr.GetBatchData(strSQL) == null)
                //    return false;
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) == null)
                    return false;
            }
            //if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
            //    return false;

            strSQL = "delete from ExternalTransmission " + strCondition;

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();
                string query = "Select ID, x, y, width, height, text, ComponentID, useSMS, SMSText, SMSExternalTeamIDList, useEFax, FaxExternalTeamIDList, StepMemberID, valign, halign, FontName, FontStyle, FontSize, LineSpace, FontColor from ExternalTransmission ";

                if (rollbackData.AddInsertRollback(dbMgr, query + strCondition, 0, 0, 0, 0, 0, 1, 1, 0, 1, 1, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0) == false)
                    return false;

                if (dbMgr.GetResultData(strSQL) != null)
                {
                    rollback.AddData(rollbackData);
                }
                else
                    return false;
                //if (dbMgr.GetBatchData(strSQL) == null)
                //    return false;
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) == null)
                    return false;
            }
            //if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
            //    return false;

            strSQL = "delete from InternalTransmission " + strCondition;

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();
                string query = "Select ID, x, y, width, height, text, ComponentID, usePopupMessage, useMobileApp, useBroadcast, StepMemberID, BroadcastMessage, TeamList, onlyTeamLeader, CommanderMemberType, CommanderMemberID, CommanderDisplayText, valign, halign, FontName, FontStyle, FontSize, LineSpace, FontColor, AutoRun from InternalTransmission ";

                if (rollbackData.AddInsertRollback(dbMgr, query + strCondition, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0) == false)
                    return false;

                if (dbMgr.GetResultData(strSQL) != null)
                {
                    rollback.AddData(rollbackData);
                }
                else
                    return false;
                //if (dbMgr.GetBatchData(strSQL) == null)
                //    return false;
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) == null)
                    return false;
            }
            //if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
            //    return false;

            strSQL = "delete from Transmission " + strCondition;

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();
                string query = "Select ID, x, y, width, height, text, ComponentID, useInternalPopupMessage, useInternalMobileApp, useInternalBroadcast, useExternalSMS, externalSMSText, SMSExternalTeamIDList, useExternalFax, FaxExternalTeamIDList, StepMemberID, InternalBroadcastMessage, valign, halign, FontName, FontStyle, FontSize, LineSpace, FontColor from Transmission ";

                if (rollbackData.AddInsertRollback(dbMgr, query + strCondition, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 0, 0) == false)
                    return false;

                if (dbMgr.GetResultData(strSQL) != null)
                {
                    rollback.AddData(rollbackData);
                }
                else
                    return false;
                //if (dbMgr.GetBatchData(strSQL) == null)
                //    return false;
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) == null)
                    return false;
            }
            //if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
            //    return false;

            strSQL = "delete from Decision " + strCondition;
            
            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();
                string query = "Select ID, x, y, width, height, text, ComponentID, StepMemberID, valign, halign, FontName, FontStyle, FontSize, LineSpace, FontColor, autoRunScript, autoRunScriptVariableTypes from Decision ";

                if (rollbackData.AddInsertRollback(dbMgr, query + strCondition, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1) == false)
                    return false;

                if (dbMgr.GetResultData(strSQL) != null)
                {
                    rollback.AddData(rollbackData);
                }
                else
                    return false;
                //if (dbMgr.GetBatchData(strSQL) == null)
                //    return false;
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) == null)
                    return false;
            }
            //if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
            //    return false;

            strSQL = "delete from EndPoint " + strCondition;

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();
                string query = "Select ID, x, y, width, height, text, ComponentID, IsBegin, StepMemberID, valign, halign, FontName, FontStyle, FontSize, LineSpace, FontColor from EndPoint ";

                if (rollbackData.AddInsertRollback(dbMgr, query + strCondition, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0) == false)
                    return false;

                if (dbMgr.GetResultData(strSQL) != null)
                {
                    rollback.AddData(rollbackData);
                }
                else
                    return false;
                //if (dbMgr.GetBatchData(strSQL) == null)
                //    return false;
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) == null)
                    return false;
            }
            //if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
            //    return false;

            strSQL = "delete from Link " + strCondition;

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();
                string query = "Select ID, x, y, width, height, text, ComponentID, LinkedComponentID, StepMemberID, valign, halign, FontName, FontStyle, FontSize, LineSpace, FontColor from Link ";

                if (rollbackData.AddInsertRollback(dbMgr, query + strCondition, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0) == false)
                    return false;

                if (dbMgr.GetResultData(strSQL) != null)
                {
                    rollback.AddData(rollbackData);
                }
                else
                    return false;
                //if (dbMgr.GetBatchData(strSQL) == null)
                //    return false;
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) == null)
                    return false;
            }
            //if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
            //    return false;

            strSQL = "delete from TransSOP " + strCondition;

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();
                string query = "Select ID, x, y, width, height, text, ComponentID, StepMemberID, LinkedActionStepID, Description, valign, halign, FontName, FontStyle, FontSize, LineSpace, FontColor from TransSOP ";

                if (rollbackData.AddInsertRollback(dbMgr, query + strCondition, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0) == false)
                    return false;

                if (dbMgr.GetResultData(strSQL) != null)
                {
                    rollback.AddData(rollbackData);
                }
                else
                    return false;
                //if (dbMgr.GetBatchData(strSQL) == null)
                //    return false;
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) == null)
                    return false;
            }
            //if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
            //    return false;

            strSQL = "delete from Arrow " + strCondition;

            if (rollback != null)
            {
                UnE.SOP.RollbackData rollbackData = new UnE.SOP.RollbackData();
                string query = "Select ID, text, BeginComponentID, BeginComponentPosition, EndComponentID, EndComponentPosition, StepMemberID from Arrow ";

                if (rollbackData.AddInsertRollback(dbMgr, query + strCondition, 0, 1, 0, 0, 0, 0, 0) == false)
                    return false;

                if (dbMgr.GetResultData(strSQL) != null)
                {
                    rollback.AddData(rollbackData);
                }
                else
                    return false;
                //if (dbMgr.GetBatchData(strSQL) == null)
                //    return false;
            }
            else
            {
                if (dbMgr.GetResultData(strSQL) == null)
                    return false;
            }
            //if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
            //    return false;

            return true;
        }
    }

    public struct StepMemberData
    {
        private string m_strTeamName;
        private int m_nTeamID;
        private Sections.SOPTeam.SOPTeamType m_nTeamType;

        public StepMemberData(string strTeamName, int nTeamID, Sections.SOPTeam.SOPTeamType nTeamType)
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

        public Sections.SOPTeam.SOPTeamType TeamType
        {
            get { return m_nTeamType; }
            set { m_nTeamType = value; }
        }
    }

    public class StepMemberDataEx
    {
        private int m_nTeamID = -1;
        private Sections.SOPTeam.SOPTeamType m_nTeamType = Sections.SOPTeam.SOPTeamType.None;
        private int m_nStepMemberID = -1;

        public StepMemberDataEx(int nTeamID, Sections.SOPTeam.SOPTeamType nTeamType, int nStepMemberID)
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

        public Sections.SOPTeam.SOPTeamType TeamType
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
