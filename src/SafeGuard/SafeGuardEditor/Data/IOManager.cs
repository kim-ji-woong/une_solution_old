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

        public bool Load(FormMain frm, WebDBManager dbMgr,ArrayList arrActionSteps, string strCategoryName, string strSubCategoryName, string strDisasterName)
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
                ArrayList arrResult1 = dbMgr.GetResultData(szSQL, 0);
                if (arrResult1 == null || arrResult1.Count == 0)
                    return false;

                int nTopTeamID = WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
                if (nTopTeamID == -1)
                    return false;

                string strSQL = string.Format("sp_TeamList2 {0}", nTopTeamID);
                ArrayList arrResult = dbMgr.GetStoredProcedureData(strSQL, 0);
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
                strSQL += " WHERE SiteID = " + FormMain.Instance.SiteID.ToString();

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

        public static Dictionary<int, Sections.ExternalTeamData> ReadExternalTeamList(WebDBManager dbMgr)
        {
            //string strSQL = "SELECT id, TeamName, PhoneNumber, FaxNumber from ExternalTeam";
            // Edit by Skkim. 2015.01.09 , 여러 Site에서 사용할 수 있도록 SiteID를 지정
            string szText = "SELECT id, TeamName, PhoneNumber, FaxNumber, ParentTeamID FROM ExternalTeam WHERE SiteID = {0}";
            
            string strSQL = string.Format(szText, FormMain.Instance.SiteID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            Dictionary<int, Sections.ExternalTeamData> dicExternal = new Dictionary<int,Sections.ExternalTeamData>();

            if (arrResult == null)
                return dicExternal;

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
                                        ref Dictionary<int, string> dicRegular)
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
                team.TeamType = (Sections.SOPTeam.SOPTeamType)int.Parse(strTeamType);
                team.TeamName = strTeamName;

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
                                        ref Dictionary<int, string> dicRegular)
        {
            ArrayList arResult = new ArrayList();
            bool bResult = GetTeamName(dbMgr, ref arResult, ref strTeamNameList, strTeamList, nBeginIndex, nEndIndex,
                ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular);

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
                                            ref Dictionary<int, string> dicRegular)
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

        public static string GetSenderTeamList(WebDBManager dbMgr, 
                                            string strTeamList, 
                                            ref ArrayList arResult,
                                            ref Dictionary<int, string> dicNormal, 
                                            ref Dictionary<int, string> dicEmergency, 
                                            ref Dictionary<int, string> dicUserDefined, 
                                            ref Dictionary<int, Sections.ExternalTeamData> dicExternal, 
                                            ref Dictionary<int, string> dicRegular)
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

                if (!GetTeamName(dbMgr, ref arResult, ref strTeamNameList, strTeamList, nBeginIndex, nDotIndex, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular))
                    return "";

                nBeginIndex = nDotIndex + 1;
            }

            if (!GetTeamName(dbMgr, ref arResult, ref strTeamNameList, strTeamList, nBeginIndex, nLen, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular))
                return "";

            return strTeamNameList;
        }


        private bool LoadProcessMission(WebDBManager dbMgr, int nProcessID, ArrayList arrMissionItems)
        {
            string strSQL = string.Format("Select ID, missionText from ProcessMission2 where ProcessID = {0}", nProcessID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strMissionText = WebDBManager.GetStringField(arrResult[i + 1], "");
              
                Sections.MissionItem item = new Sections.MissionItem();

                item.TransmissionType = -1;
                item.Mission = strMissionText;
                item.Target = "";

                arrMissionItems.Add(item);
            }

            return true;
        }

        private bool LoadProcess(WebDBManager dbMgr, 
            Dictionary<int, Sections.Section> dicSections, 
            ArrayList arrSections, 
            Sections.PanelSectionEx panel, 
            StepMemberData data)

        {

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT id, x, y, width, height, text, TeamList, ComponentID, useMissionMessage ");
            sb.AppendFormat(" FROM Process2 where StepMemberID = {0}", data.ID.ToString());

            string szSQL = sb.ToString();
            ArrayList arrResult = dbMgr.GetResultData(szSQL, 0);
            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;
            
            for (int i = 0; i < nResultCount - 8; i+=9)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                float x = WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0.0f);
                float y = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
                float fWidth = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
                float fHeight = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
                string strText = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strTeamList = WebDBManager.GetStringField(arrResult[i + 6], "");
                string strComponentID = WebDBManager.GetStringField(arrResult[i + 7], "");               
                bool useMissionMessage = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 0) > 0 ? true : false;
                              

                Sections.SectionProcess section = new Sections.SectionProcess(panel, x, y);                               
               
                dicSections[nID] = section;
                arrSections.Add(section);
                panel.Sections.Add(section);
                
                section.RectSize = new SizeF(fWidth, fHeight);
                section.TextUP = strText;

                Sections.SectionData tempData = section.Data;
                section.TextDown = "";
                
                Sections.SectionDataProcess sectionData = (Sections.SectionDataProcess)tempData;               
                sectionData.ID = nID;
                sectionData.Title = strText;
                sectionData.ComponentID = strComponentID;
                
                Sections.ProcessingTime.Type type = Sections.ProcessingTime.Type.UNKNOWN;               

                sectionData.ProcessingTime.ProcessingType = type;
                sectionData.MissionTransfer = useMissionMessage;

                if (!LoadProcessMission(dbMgr, nID, sectionData.MissionItems))
                    return false;
            }
            return true;
        }
              
        private bool LoadAnnotation(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberData data)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID from Annotation2 where StepMemberID = "; 
            strSQL += data.ID.ToString();
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

        private bool LoadEndPoint(WebDBManager dbMgr, Dictionary<int, Sections.Section> dicSections, ArrayList arrSections, Sections.PanelSectionEx panel, StepMemberData data)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID, isBegin from EndPoint2 where StepMemberID = " + data.ID.ToString();
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

        private Dictionary<int, Sections.Section> GetSectionDictionary(int nSectionType,
            Dictionary<int, Sections.Section> dicProcessSections,
            Dictionary<int, Sections.Section> dicAnnotationSections,
            Dictionary<int, Sections.Section> dicEndPointSections)
        {
            switch (nSectionType)
            {
                case (int)Sections.Section.ComponentType.PROCESS:
                    return dicProcessSections;          

                case (int)Sections.Section.ComponentType.ANNOTATION:
                    return dicAnnotationSections;

                case (int)Sections.Section.ComponentType.ENDPOINT:
                    return dicEndPointSections;
            }
            return null;
        }

		private bool LoadArrow(WebDBManager dbMgr, 
            Dictionary<int, Sections.Section> dicProcessSections, 
            Dictionary<int, Sections.Section> dicAnnotationSections, 
            Dictionary<int, Sections.Section> dicEndPointSections,
            StepMemberData data)
        {
            string strSQL = "select ID, Text, BeginComponentID, BeginComponentPosition, EndComponentID, EndComponentPosition ";
            strSQL += "from Arrow2 where StepMemberID = " + data.ID.ToString();

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
                Dictionary<int, Sections.Section> dicBeginSection = 
                    GetSectionDictionary(nBeginType, 
                                        dicProcessSections, 
                                        dicAnnotationSections, 
                                        dicEndPointSections);

                // nBeginType, 즉 nBeginComponentID가 잘못 입력된 경우
                if (dicBeginSection == null)
                    return false;

                int nEndType = nEndComponentID >> 24;
                nEndComponentID = nEndComponentID & 0xffffff;
                Dictionary<int, Sections.Section> dicEndSection =
                    GetSectionDictionary(nEndType,
                                        dicProcessSections,
                                        dicAnnotationSections,
                                        dicEndPointSections); ;

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
            StepMemberData data, 
            ArrayList arrLink, 
            ArrayList arrSections)
        {
            // 화살표 연결을 위하여 Section 정보를 임시 저장
            // ComponentID, Section
            Dictionary<int, Sections.Section> dicProcessSections = new Dictionary<int,Sections.Section>();
            Dictionary<int, Sections.Section> dicAnnotationSections = new Dictionary<int, Sections.Section>();
            Dictionary<int, Sections.Section> dicEndPointSections = new Dictionary<int, Sections.Section>();      

            if (!LoadProcess(dbMgr, dicProcessSections, arrSections, panel, data))
                    return false;
            if (!LoadAnnotation(dbMgr, dicAnnotationSections, arrSections, panel, data))
                return false;
            if (!LoadEndPoint(dbMgr, dicEndPointSections, arrSections, panel, data))
                return false;          

			if (!LoadArrow(dbMgr, dicProcessSections, dicAnnotationSections, dicEndPointSections, data))
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
                    strTeamIDs = data.ID.ToString();
                else
                    strTeamIDs += ", " + data.ID.ToString();
            }

            if (strTeamIDs.Length == 0)
                return null;

            string strSQL = string.Format("select id, TeamName, ActionStepID from StepMember2 where ActionStepID in ({0}) and ID in ({1})", strActionStepIDs, strTeamIDs);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            if (nResultCount == 0)
                return null;

            Dictionary<int, ArrayList> dicStepMembers = new Dictionary<int, ArrayList>();

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nStepMemberID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string szTeamName =  WebDBManager.GetStringField(arrResult[i + 1].ToString(),"");             
                int nActionStepID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);

                StepMemberData data = new StepMemberData(szTeamName);
                data.ActionStepID = nActionStepID;
                data.ID = nStepMemberID;

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

        private StepMemberData FindStepMemberDataEx(Sections.PanelSectionEx panel, ArrayList arrStepMemberDataEx)
        {
            foreach (StepMemberData data in arrStepMemberDataEx)
            {
                if (data.ID == panel.TeamID && data.TeamName == panel.TeamName)
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

            string strSQL = string.Format("Select ID, StepName, DisasterID, Description from ActionStep2 where ID in ({0})", strIDs);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 1);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            if (nResultCount == 0)
                return null;

            ArrayList arrStepDatas = new ArrayList();

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strStepName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nDisasterID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                string strDesc = WebDBManager.GetStringField(arrResult[i + 3], "");

                Data_ActionStep data = new Data_ActionStep();

                data.ID = nID;
                data.StepName = strStepName;
                data.DisasterID = nDisasterID;

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
            
            // TeamID, Team Name
            Dictionary<int, string> dicNormal = null;
            Dictionary<int, string> dicEmergency = null;
            Dictionary<int, string> dicUserDefined = null;
            //Dictionary<int, Sections.ExternalTeamData> dicExternal = ReadExternalTeamList(dbMgr);
            Dictionary<int, string> dicRegular = null;

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
                    }
                }

                if (!dicStepMembers.ContainsKey(actionStep.ActionStepID))
                    continue;

                ArrayList arrStepMemberDataEx = dicStepMembers[actionStep.ActionStepID];

                TabPage tabPage = GetTabPage(actionStep.ActionStepName, pageLevel.GetTabPage());
                if (tabPage == null)
                    continue;
				
                ArrayList arrPanels = pageLevel.AddPane(arrTeams, tabPage);
				pageLevel.AddUsingTeam(arrTeams);

                // Link 객체는 다른 Panel의 객체와 연결되어야 하므로, DB로부터 모든 객체를 읽어들인 후에 해당 객체와 연결시킨다.
                // Link 객체는 같은 Step내의 객체들과만 연결된다.
                // arrSections는 Step내의 모든 Section 객체를 담게 되는데, Link 객체와 연결하기 위해서다.
                ArrayList arrLink = new ArrayList();
                ArrayList arrSections = new ArrayList();

                foreach (Sections.PanelSectionEx panel in arrPanels)
                {
                    StepMemberData data = FindStepMemberDataEx(panel, arrStepMemberDataEx);
                    if (data == null)
                        continue;

                    panel.ActionStepID = actionStep.ActionStepID;

                    if (!LoadPanelComponent(dbMgr, panel, data, arrLink, arrSections))
                        return false;
                }

                if (!SetLinkSections(arrLink, arrSections))
                    return false;
            }

            return true;
        }

        private int FindStepMemberTeamIndex(int nTeamID, string strTeamName, ArrayList arrTeams)
        {
            int nTeamCount = arrTeams.Count;

            for (int i = 0; i < nTeamCount; i++)
            {
                StepMemberData data = (StepMemberData)arrTeams[i];

                if (data.ID == nTeamID && data.TeamName == strTeamName)
                    return i;
            }
            return -1;
        }
       
		private ArrayList LoadBarPage(FormPageSOP pageLevel, ArrayList arrActionSteps, WebDBManager dbMgr)
        {
            if (arrActionSteps == null || arrActionSteps.Count == 0)
                return null;

            ActionStepInfo actionStep = (ActionStepInfo)arrActionSteps[0];
            string strSQL = string.Format("Select ID, TeamName from StepMember2 where ActionStepID = {0}", actionStep.ActionStepID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            int nResultCount = arrResult.Count;

            ArrayList arrTeams = new ArrayList();

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nStepMemberID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string szTeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
                StepMemberData data = new StepMemberData(szTeamName, nStepMemberID, actionStep.ActionStepID);
                arrTeams.Add(data);
            }
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
        public bool Save(FormMain frm, WebDBManager dbMgr,bool bSaveAs, string strDescription, out int nDisasterID)
        {
            nDisasterID = 0;

            string strCategory = SopDocManager.Instance.CategoryName;
			string strSubCategory = SopDocManager.Instance.SubCategoryName;
			string strDisaster = SopDocManager.Instance.DisasterName;

            Data_Disaster currentDisaster = null;
   
            foreach (Data_Disaster disaster in FormMain.Instance.DetailDisaster)
            {
                if (disaster.DisasterName == strDisaster && disaster.DisasterType == strCategory)
                {
                    if (disaster.ID > 0)
                    {
                        currentDisaster = disaster;
                        break;
                    }
                }
            }

            m_dicDeletedActionStep.Clear();
            SaveDeletingActionStepID();

            // Batch Job Start - Begin Transaction
            dbMgr.BeginBatch();

            // 현재 사용중인 버전은 삭제가 안되므로 업데이트 할 수 없다.
            if (bSaveAs == true)
            {
                if (!DeleteSOPVersion(dbMgr, currentDisaster.ID, true))
                {
                    return false;
                }
            }

            
            nDisasterID = AddDisaster(dbMgr);
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
                        Sections.SOPTeam.SOPTeamType nTeamType = panel.TeamType;

                        StepMemberData data = dicStepMember.Keys.ElementAt(0);

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
            GetComponentMaxID(dbMgr, "Arrow2", ref nArrowID, true);

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

                string strSQL = string.Format("insert into Arrow2 (ID, Text, BeginComponentID, BeginComponentPosition, EndComponentID, EndComponentPosition, StepMemberID) values ({0}, '{1}', {2}, {3}, {4}, {5}, {6})",
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
                    strIDs = string.Format("{0}({1})", team.TeamID, (int)team.TeamType);
                else
                    strIDs += string.Format(", {0}({1})", team.TeamID, (int)team.TeamType);
            }

            return strIDs;
        }

        public static string GetInternalTeamList(Sections.SectionInternal section)
        {
            Sections.SectionDataInternal data = (Sections.SectionDataInternal)section.Data;
            string strIDs = "";

            foreach (Sections.SOPTeam team in data.TeamList)
            {
                if (strIDs.Length == 0)
                    strIDs = string.Format("{0}({1})", team.TeamID, (int)team.TeamType);
                else
                    strIDs += string.Format(", {0}({1})", team.TeamID, (int)team.TeamType);
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
            GetComponentMaxID(dbMgr, "Process2", ref nProcessID, true);

            Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;
            string strTeamList = GetProcessTeamList(section);          

            StringBuilder sb = new StringBuilder();
            sb.Append("insert into Process2 (ID, x, y, width, height, text, TeamList, ComponentID, useMissionMessage, StepMemberID ) ");
            sb.AppendFormat(" values ({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', '{7}', {8}, {9} )",
                    ++nProcessID,
                    section.Position.X,
                    section.Position.Y,
                    section.RectSize.Width,
                    section.RectSize.Height,
                    ChangeSpecialCharacter(section.TextUP),
                    ChangeSpecialCharacter(strTeamList) ,
                    ChangeSpecialCharacter(data.ComponentID),
                    data.MissionTransfer ? 1 : 0,
                    nStepMemberID
                    );            

            string strSQL = sb.ToString();
            if (dbMgr.GetResultData(strSQL, 1) == null)
                return false;

            return AddProcessMission(dbMgr, nProcessID, data, ref nProcessMissionID);
        }

        private bool AddProcessMission(WebDBManager dbMgr, int nProcessID, Sections.SectionDataProcess data, ref int nProcessMissionID)
        {
            GetComponentMaxID(dbMgr, "ProcessMission2", ref nProcessMissionID, true);

            foreach (Sections.MissionItem mission in data.MissionItems)
            {
                string strSQL = string.Format("insert into ProcessMission2 (ID, missionText, ProcessID) values ({0}, '{1}', {2})",
                    ++nProcessMissionID, ChangeSpecialCharacter(mission.Mission), nProcessID);

                if (dbMgr.GetResultData(strSQL, 1) == null)
                    return false;
            }
            return true;
        }

        private bool AddAnnotation(WebDBManager dbMgr, int nStepMemberID, Sections.SectionAnnotation section, ref int nAnnotationID)
        {
            GetComponentMaxID(dbMgr, "Annotation2", ref nAnnotationID, true);
            Sections.SectionDataAnnotation data = (Sections.SectionDataAnnotation)section.Data;

            StringBuilder sb = new StringBuilder();
            sb.Append("insert into Annotation2 ( ID, x, y, width, height, text, ComponentID, StepMemberID) ");

            sb.AppendFormat(" values ( {0}, {1}, {2}, {3}, {4}, '{5}', '{6}', {7})",
                ++nAnnotationID, 
                section.Position.X,
                section.Position.Y,
                section.RectSize.Width,
                section.RectSize.Height,
                ChangeSpecialCharacter(section.Title),
                ChangeSpecialCharacter(data.ComponentID),
                nStepMemberID                        
                );

            string strSQL = sb.ToString();
            return dbMgr.GetResultData(strSQL, 1) == null ? false : true;
        }

        private bool AddEndPoint(WebDBManager dbMgr, int nStepMemberID, Sections.SectionEndPoint section, ref int nEndPointID)
        {

            GetComponentMaxID(dbMgr, "EndPoint2", ref nEndPointID, true);
           
            Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)section.Data;
            
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into EndPoint2 ( ID, x, y, width, height, text, ComponentID, isBegin, StepMemberID) ");
            sb.AppendFormat(" values ( {0}, {1}, {2}, {3}, {4}, '{5}', '{6}', {7}, {8} )",
                ++nEndPointID,  // 0
                section.Position.X, 
                section.Position.Y,  
                section.RectSize.Width, 
                section.RectSize.Height,
                ChangeSpecialCharacter(section.Title),// 5
                ChangeSpecialCharacter(data.ComponentID),
                data.IsBegin ? 1 : 0,
                nStepMemberID            
                );
            string strSQL = sb.ToString();
            return dbMgr.GetResultData(strSQL, 1) == null ? false : true;
        }

        // Return 값 : 새로 생성된 StepMember들의 ID List
        //             저장에 실패하면 null을 리턴
        private Dictionary<System.Windows.Forms.TabPage, Dictionary<StepMemberData, int>> AddStepMembers(FormMain frm, WebDBManager dbMgr, Dictionary<System.Windows.Forms.TabPage, int> dicActionStepIDs)
        {
			FormPageSOP pageLevel = frm.GetPageLevel();

            string strSQL = "Select max(id) from StepMember2";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 1);

            int nStepMemberID;

            if (arrResult == null || arrResult.Count == 0)
                nStepMemberID = 0;
            else
                nStepMemberID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);

            Type panelType = typeof(Sections.PanelSectionEx);
            Dictionary<System.Windows.Forms.TabPage, Dictionary<StepMemberData, int>> dicStepMembers = new Dictionary<System.Windows.Forms.TabPage, Dictionary<StepMemberData, int>>();
         
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
                        
                        if (nTeamID < 0)
                        {                           
                            return null;
                        }

                        strSQL = string.Format("insert into StepMember2 (ID, TeamName, ActionStepID) values ({0}, '{1}', {2})",
                            ++nStepMemberID, strTeamName, nActionStepID);

                        if (dbMgr.GetResultData(strSQL, 1) == null)
                            return null;
                       

                        StepMemberData data = new StepMemberData(strTeamName, nStepMemberID, nActionStepID);
                        data.ID = nStepMemberID;

                        dicStepMember[data] = nStepMemberID;
                    }
                }
                dicStepMembers[page] = dicStepMember;
            }
            return dicStepMembers;
        }
 

        // Return 값 : 새로 생성된 ActionStep들의 ID List
        //             저장에 실패하면 null을 리턴
        public Dictionary<System.Windows.Forms.TabPage, int> AddActionSteps(FormMain frm, WebDBManager dbMgr, int nDisasterID)
        {
			FormPageSOP pageLevel = frm.GetPageLevel();
            
            string strSQL = "Select max(id) from ActionStep2";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 1);

            int nActionStepID;

            if (arrResult == null || arrResult.Count == 0)
                nActionStepID = 0;
            else
                nActionStepID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);

            // TabPage별 ActionStepID
            Dictionary<System.Windows.Forms.TabPage, int> dicActionStepID = new Dictionary<System.Windows.Forms.TabPage, int>();

            // ActionStepID별 부모 TabPage
            Dictionary<int, TabPage> dicParentTabPage = new Dictionary<int, TabPage>();

            foreach (ActionStepTabPage page in pageLevel.TabControls.TabPages)
            {
                string strStepName = page.Text;
				Data_ActionStep opt = page.Data;
                if (opt == null)
                    continue;

                StringBuilder sb = new StringBuilder();
                sb.Append("insert into ActionStep2 (ID, StepName, DisasterID, Description )");
                sb.AppendFormat(" values ({0}, '{1}', {2},  NULL)",
                        ++nActionStepID,
                        strStepName,
                        nDisasterID);

                strSQL = sb.ToString();
                if (dbMgr.GetResultData(strSQL, 1) == null)
                    return null;

                dicActionStepID[page] = nActionStepID;
                opt.ID = nActionStepID;
                opt.DisasterID = nDisasterID;

                if (page.Tag != null)
                    dicParentTabPage[nActionStepID] = (TabPage)page.Tag;
            }
            return dicActionStepID;
        }

        // Return 값 : Disaster ID
        //             이 값이 0보다 작으면 실패
        private int AddDisaster(WebDBManager dbMgr)
        {            
			string strDisaster = SopDocManager.Instance.DisasterName;
			string strCategory = SopDocManager.Instance.CategoryName;
			string strDescription = SopDocManager.Instance.DisasterDescription;

            if (strDisaster == "" || strCategory == "")
                return -1;


            string strSQL = string.Format("select max(id) from Disaster2");
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 1);

            int nDisasterID;

            if (arrResult == null || arrResult.Count == 0)
                nDisasterID = 0;
            else
                nDisasterID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);

            strSQL = string.Format("INSERT INTO Disaster2 (ID, DisasterName, DisasterType, Description) VALUES ({0}, '{1}', '{2}', '{3}')",
                ++nDisasterID, ChangeSpecialCharacter(strDisaster), ChangeSpecialCharacter(strCategory), ChangeSpecialCharacter(strDescription));

            if (dbMgr.GetResultData(strSQL, 1) == null)
                return -1;

            return nDisasterID;
        }

    
				
		public bool IsMonitoringDiaster(WebDBManager dbMgr, string szDisasterName)
		{
			return false;
		}


        // 기존 버전을 삭제
        public bool DeleteSOPVersion(WebDBManager dbMgr, int nDisasterID,  bool transaction)
        {

            string strSQL = string.Format("select id from ActionStep2 where DisasterID in ({0})", nDisasterID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult.Count == 0)
            {
                if (!DeleteDisaster(dbMgr, nDisasterID, transaction))
                    return false;

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
                        
            strSQL = string.Format("select id from StepMember2 where ActionStepID in ({0})", strActionStepIDs);
            arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            if (arrResult.Count == 0)
            {

                if (!DeleteActionStep(dbMgr, nDisasterID, transaction))
                    return false;
                if (!DeleteDisaster(dbMgr, nDisasterID, transaction))
                    return false;
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
           
            if (!DeleteStepMember(dbMgr, strActionStepIDs, transaction))
                return false;
            if (!DeleteActionStep(dbMgr, nDisasterID, transaction))
                return false;
            if (!DeleteDisaster(dbMgr, nDisasterID, transaction))
                return false;

            return true;
        }


        private bool DeleteDisaster(WebDBManager dbMgr, int nVersionID, bool transaction)
        {
            string strSQL = "delete from Disaster2 where ID = " + nVersionID.ToString();
            return dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null ? false : true;
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

        private bool DeleteActionStep(WebDBManager dbMgr, int nDisasterID, bool transaction)
        {
            string strSQL = string.Format("delete from ActionStep2 where DisasterID in ({0})", nDisasterID);
            return dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null ? false : true;
        }

        private bool DeleteStepMember(WebDBManager dbMgr, string strActionStepIDs, bool transaction)
        {
            string strSQL = string.Format("delete from StepMember2 where ActionStepID in ({0})", strActionStepIDs);
            return dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null ? false : true;
        }

        private bool DeleteComponent(WebDBManager dbMgr, string strStepMemberIDs, bool transaction)
        {
            if (strStepMemberIDs == null || strStepMemberIDs == "")
                return false;

            string strSQL = string.Format("select id from Process2 where StepMemberID in ({0})", strStepMemberIDs);
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

            //if (strProcessIDs.Length > 0)
            //{
            //    strSQL = string.Format("delete from CheckTask where ProcessID in ({0})", strProcessIDs);
            //    if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
            //        return false;
            //}

            if (strProcessIDs.Length > 0)
            {
                strSQL = string.Format("delete from ProcessMission2 where ProcessID in ({0})", strProcessIDs);
                if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                    return false;
            }

            strSQL = string.Format("delete from Process2 where StepMemberID in ({0})", strStepMemberIDs);
            if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                return false;

            strSQL = string.Format("delete from Annotation2 where StepMemberID in ({0})", strStepMemberIDs);
            if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                return false;

            //strSQL = string.Format("delete from ExternalTransmission where StepMemberID in ({0})", strStepMemberIDs);
            //if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
            //    return false;

            //strSQL = string.Format("delete from InternalTransmission where StepMemberID in ({0})", strStepMemberIDs);
            //if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
            //    return false;

            //strSQL = string.Format("delete from Transmission where StepMemberID in ({0})", strStepMemberIDs);
            //if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
            //    return false;

            //strSQL = string.Format("delete from Decision where StepMemberID in ({0})", strStepMemberIDs);
            //if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
            //    return false;

            strSQL = string.Format("delete from EndPoint2 where StepMemberID in ({0})", strStepMemberIDs);
            if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                return false;

            //strSQL = string.Format("delete from Link where StepMemberID in ({0})", strStepMemberIDs);
            //if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
            //    return false;

            //strSQL = string.Format("delete from TransSOP where StepMemberID in ({0})", strStepMemberIDs);
            //if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
            //    return false;

            strSQL = string.Format("delete from Arrow2 where StepMemberID in ({0})", strStepMemberIDs);
            if (dbMgr.GetResultData(strSQL, transaction ? 1 : 0) == null)
                return false;

            return true;
        }
    }

    public class StepMemberData
    {
        private string m_strTeamName;
        private int m_nTeamID = -1;
        private int m_nActionStepID = -1;      

        public StepMemberData(string strTeamName, int nTeamID, int nActionID)
        {
            m_strTeamName = strTeamName;
            m_nTeamID = nTeamID;
            m_nActionStepID = nActionID;
        }

        public StepMemberData(string strTeamName)
        {
            m_strTeamName = strTeamName;
        }

        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }

        public int ActionStepID
        {
            get { return m_nActionStepID; }
            set { m_nActionStepID = value; }
        }


        public int ID
        {
            get { return m_nTeamID; }
            set { m_nTeamID = value; }
        }
    }    
}
