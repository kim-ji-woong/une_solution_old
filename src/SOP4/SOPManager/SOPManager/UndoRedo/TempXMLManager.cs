
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using DBUtility;

namespace SOPManager
{
    // DB가 아닌 파일에서 읽고 쓴다.
	// Undo/Redo 시 사용된다. XmlManager와는 Check, UI셋팅이 다른다. 
	// 상태에 따른 부가정보도 들어 있다. 
    public class TempXMLManager
    {
        private string m_strErrorMessage = "";

        // Header
        private string m_strCategoryName = "";
        private string m_strSubCategoryName = "";
        private string m_strDisasterName = "";
        private string m_strVersionName = "";
        private string m_strDescription = "";
        private bool m_isRegular = false;   // 등록 모드인가?
        private bool m_isNormal = false;    // 주간 모드인가?

        private static string XML_VERSION = "V1.4";
        ///////////////////////////////////////

        private ArrayList m_arrActionSteps = new ArrayList();

        private bool LoadScreen(FormMain frm)
        {
			FormPageSOP pageLevel = FormMain.Instance.GetPageLevel();

			ClearSOP(frm);
			if (m_arrActionSteps.Count > 0)
			{
				string strFullPath = LoadDisasterTree(frm, m_strCategoryName, m_strSubCategoryName, m_strDisasterName, m_arrActionSteps);
				pageLevel.GetPropertiesLevel().SetTitleText(strFullPath);

				LoadBarPage(frm, m_arrActionSteps);
			}
			

			bool bSuccess = false;
			if (LoadTabPages(frm))
			{
				// -> UI 에 저장된 값은 SopDocManager로 이동함.
				// TempXML에서는 FormNewSOP를 셋팅하는 경우는 NewSOP인 경우임, 2014.10.31 skkim
				// New SOP
				
				SopDocManager.Instance.CategoryName = m_strCategoryName;
				SopDocManager.Instance.SubCategoryName = m_strSubCategoryName;
				SopDocManager.Instance.DisasterName = m_strDisasterName;
				SopDocManager.Instance.RegularMode = m_isRegular;
				SopDocManager.Instance.WeekMode = m_isNormal;

				bSuccess = true;
			}
						
			pageLevel.LevelTabSelected();

			if (m_arrActionSteps.Count == 0)
			{
				pageLevel.GetBarPage().ClearGrid();
				BarLevelTree tree = pageLevel.GetBarLevelTree();
				tree.AddTreeNode(m_strCategoryName, m_strSubCategoryName, m_strDisasterName);
				
			}

			// 프로퍼티창 초기화
			pageLevel.OnClearComponentProperties();

			return bSuccess;
        }

        // Return 값 : Tree에서 선택된 단계의 전체 경로
        private string LoadDisasterTree(FormMain frm, string strCategoryName, string strSubCategoryName, string strDisasterName, ArrayList arrActionSteps)
        {
			FormPageSOP pageLevel = frm.GetPageLevel();
            BarLevelTree tree = pageLevel.GetBarLevelTree();

            if (arrActionSteps == null)
                return "";

            // ActionStepID, TreeNode
            Dictionary<int, TreeNode> dicTreeNode = new Dictionary<int, TreeNode>();
            ArrayList arrChildSteps = new ArrayList();

            foreach (ActionStep actionStep in arrActionSteps)
            {
                if (actionStep.ParentStepID <= 0)
                {
                    TreeNode node = tree.AddTreeNode(strCategoryName, strSubCategoryName, strDisasterName, actionStep.StepName);
                    dicTreeNode[actionStep.ID] = node;
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

        private void LoadBarPage(FormMain frm, ArrayList arrActionSteps)
        {
            if (arrActionSteps == null || arrActionSteps.Count == 0)
                return;

            ArrayList arrTeams = new ArrayList();

            //0(평일 비상 조직, TemporaryNormalTeam), 1(휴일 비상 조직, TemporaryEmergencyTeam), 2(외부 기관, ExternalTeam), 3(사용자 정의 조직, UserDefinedTeam), 4(정규 조직, RegularTeam)
            //foreach (ActionStep actionStep in arrActionSteps)
            ActionStep actionStep = (ActionStep)arrActionSteps[0];
            {              
                foreach (StepMember stepMember in actionStep.StepMemberList)
                {
                    StepMemberData data = new StepMemberData(stepMember.TeamName, stepMember.TeamID, stepMember.TeamType);
                    arrTeams.Add(data);
                }
            }

			frm.GetPageLevel().GetBarPage().SetDataGrid(arrTeams);
        }

        // 부모가 있는 단계들...
        private void LoadChildActionStepTree(Dictionary<int, TreeNode> dicTreeNode, ArrayList arrChildSteps)
        {
            while (arrChildSteps.Count > 0)
            {
                ArrayList arrRemove = new ArrayList();

                foreach (ActionStep actionStep in arrChildSteps)
                {
                    if (dicTreeNode.ContainsKey(actionStep.ParentStepID))
                    {
                        TreeNode node = dicTreeNode[actionStep.ParentStepID];
                        node = node.Nodes.Add(actionStep.StepName);
                        dicTreeNode[actionStep.ID] = node;
                        arrRemove.Add(actionStep);
                    }
                }

                foreach (ActionStep actionStep in arrRemove)
                {
                    arrChildSteps.Remove(actionStep);
                }
            }
        }

        private bool LoadTabPages(FormMain frm)
        {
			FormPageSOP pageLevel = frm.GetPageLevel();
            int nOldTabCount = pageLevel.GetTabPage().Count;			

			for (int i = 0; i < nOldTabCount; i++)
			{				
				pageLevel.TabControls.Controls[0].Visible = false;
			}


            // TabPage 간의 부모, 자식 관계를 저장하기 위한 Dictionary
            // ActionStep ID, TabPage
            Dictionary<int, TabPage> dicTabPages = new Dictionary<int, TabPage>();

            // ActionStep별 TabPage 생성
            foreach (ActionStep actionStep in m_arrActionSteps)
            {
                TabPage tabPage = pageLevel.AddTabPage(actionStep);
				Control c = (Control)tabPage;
				c.Visible = false;
                if (tabPage == null)
                {
                    m_strErrorMessage = "TabPage 생성에 실패하였습니다.";
                    return false;
                }
	            dicTabPages[actionStep.ID] = tabPage;
            }

            // TeamID, Team Name
            Dictionary<int, string> dicNormal = null;
            Dictionary<int, string> dicEmergency = null;
            Dictionary<int, string> dicUserDefined = null;
            Dictionary<int, Sections.ExternalTeamData> dicExternal = IOManager.ReadExternalTeamList(FormMain.Instance.DBManager);
            Dictionary<int, string> dicRegular = null;
            Dictionary<int, string> dicControlRoom = null;

            // TabPage별 부모 Tab 설정
            foreach (ActionStep actionStep in m_arrActionSteps)
            {
                TabPage tabPage = dicTabPages[actionStep.ID];

                if (actionStep.ParentStepID >= 0)
                {
                    if (!dicTabPages.ContainsKey(actionStep.ParentStepID))
                    {
                        m_strErrorMessage = string.Format("ActionStep id=\"{0}\"에서 존재하지 않는 ParentStepID {1}을 참조합니다.", actionStep.ID, actionStep.ParentStepID);
                        return false;
                    }

                    tabPage.Tag = dicTabPages[actionStep.ParentStepID];
                }

                if (!AddPanels(tabPage, actionStep, pageLevel, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom))
                    return false;
            }

            // 기존에 남아있던 Tab은 삭제
            for (int i = 0; i < nOldTabCount; i++)
            {
                ArrayList arTabPages = pageLevel.GetTabPage();
                TabPage page = (TabPage)arTabPages[0];
                pageLevel.GetTabPage().RemoveAt(0);
                pageLevel.TabControls.Controls.RemoveAt(0);
                page.Dispose();
            }

			foreach (Control control in pageLevel.TabControls.Controls)
			{
				control.Visible = true;
			}

			foreach (ActionStep actionStep in m_arrActionSteps)
			{
				TabPage tabPage = dicTabPages[actionStep.ID];		
				if (actionStep.Selected == true)
				{
					pageLevel.TabControls.SelectedTab = tabPage;
				}
			}

            return true;
        }

        private StepMember FindStepMember(ActionStep actionStep, int nTeamID, Sections.SOPTeam.SOPTeamType nTeamType)
        {
            foreach (StepMember stepMember in actionStep.StepMemberList)
            {
                if (stepMember.TeamID == nTeamID && stepMember.TeamType == nTeamType)
                    return stepMember;
            }

            return null;
        }

        private bool AddPanels(TabPage tabPage, ActionStep actionStep, FormPageSOP pageLevel, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined, ref Dictionary<int, Sections.ExternalTeamData> dicExternal, ref Dictionary<int, string> dicRegular, ref Dictionary<int, string> dicControlRoom)
        {
            ArrayList arrStepMemberDatas = new ArrayList();

            foreach (StepMember stepMember in actionStep.StepMemberList)
            {
                SOPManager.StepMemberData data = new StepMemberData(stepMember.TeamName, stepMember.TeamID, stepMember.TeamType);
                arrStepMemberDatas.Add(data);
            }

            ArrayList arrPanels = pageLevel.AddPane(arrStepMemberDatas, tabPage);
            if (arrPanels == null)
                return false;

            // Link된 Section을 알아내기 위하여 ID별 Section 객체 저장
            // 상위 4바이트(StepMember Index) + 하위 4바이트(Component ID), Section 객체
            Dictionary<long, Sections.Section> dicSections = new Dictionary<long, Sections.Section>();
            Dictionary<PropertyLink, Sections.SectionLink> dicLinkSections = new Dictionary<PropertyLink,Sections.SectionLink>();

			Dictionary<int, Sections.Section> dicCompSection = new Dictionary<int, Sections.Section>();


            long nStepMemberIndex = 1;

            foreach (Sections.PanelSectionEx panel in arrPanels)
            {
                StepMember stepMember = FindStepMember(actionStep, panel.TeamID, panel.TeamType);
                if (stepMember == null)
                    return false;

				AddComponents(panel, stepMember, nStepMemberIndex, dicSections, dicLinkSections, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicCompSection, ref dicControlRoom);
                AddArrows(stepMember, (long)nStepMemberIndex, dicSections);

				PointF ptOrigin = new PointF(stepMember.Viewport.OriginX, stepMember.Viewport.OriginY);
				PointF ptCurrent = new PointF(stepMember.Viewport.CurrentX, stepMember.Viewport.CurrentY);
				PointF fScale = new PointF(stepMember.Viewport.Scale, stepMember.Viewport.PrevScale);

				panel.IsModified = stepMember.Modify;

				panel.SetViewport(ptOrigin, ptCurrent, fScale);
				panel.Refresh();

                nStepMemberIndex++;
            }

            // Link된 Section 설정
            foreach (KeyValuePair<PropertyLink, Sections.SectionLink> pair in dicLinkSections)
            {
				long id = (((long)pair.Key.LinkedStepMemberID) << 32) | (uint)pair.Key.LinkedID;

                if (!dicSections.ContainsKey(id))
					continue;

                Sections.Section section = dicSections[id];
                Sections.SectionDataLink data = (Sections.SectionDataLink)pair.Value.Data;
                data.LinkedSection = section;
            }

            return true;
        }

        // dicSections : 상위 4바이트(StepMember Index) + 하위 4바이트(Component ID), Section 객체
        private bool AddArrows(StepMember stepMember, long nStepMemberIndex, Dictionary<long, Sections.Section> dicSections)
        {
            foreach (Arrow arrow in stepMember.ArrowList)
            {
                if (arrow.BeginComponentID < 0 || arrow.EndComponentID < 0)
                    continue;

				long idBegin = (nStepMemberIndex << 32) | (uint)arrow.BeginComponentID;
                long idEnd = (nStepMemberIndex << 32) | (uint)arrow.EndComponentID;

                if (!dicSections.ContainsKey(idBegin))
                    continue;

                if (!dicSections.ContainsKey(idEnd))
                    continue;

                Sections.Section sectionBegin = dicSections[idBegin];
                Sections.Section sectionEnd = dicSections[idEnd];
								
                Sections.Arrow sectionArrow = new Sections.Arrow();

                sectionArrow.BeginLink = sectionBegin;
                sectionArrow.EndLink = sectionEnd;
                sectionArrow.BeginPosition = (Sections.Arrow.ArrowPosition)arrow.BeginComponentPosition;
                sectionArrow.EndPosition = (Sections.Arrow.ArrowPosition)arrow.EndComponentPosition;
                sectionArrow.Text = arrow.Text;

                sectionBegin.AddArrow(sectionArrow);
				sectionEnd.AddArrow(sectionArrow);
                sectionArrow.CalcArrowLine();

				if (sectionBegin.GroupMember == true || sectionEnd.GroupMember == true)
				{
					sectionArrow.Visible = false;
				}
            }

            return true;
        }

        private bool AddComponents(Sections.PanelSectionEx panel, StepMember stepMember, long nStepMemberIndex, Dictionary<long, Sections.Section> dicSections, Dictionary<PropertyLink, Sections.SectionLink> dicLinkSections, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined, ref Dictionary<int, Sections.ExternalTeamData> dicExternal, ref Dictionary<int, string> dicRegular, ref Dictionary<int, Sections.Section> dicCompSection, ref Dictionary<int, string> dicControlRoom)
        {
            foreach (Component component in stepMember.ComponentList)
            {
                ComponentProperty property = component.Property;
				Sections.Section section = ToSection(panel, component.X, component.Y, property, dicLinkSections, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, dicCompSection, ref dicControlRoom);

                if (section == null)
                    return false;

                if (component.FontName != "")
                {
                    Font font = new Font(component.FontName, component.FontSize, (FontStyle)component.FontStyle);
                    section.TextFont = font;
                    section.TextColor = Color.FromArgb(component.TextColor);
                    section.Data.LineSpace = component.LineSpace;
                }


                if (section.GetComponentType() == Sections.Section.ComponentType.PROCESS)
                {
                    ((Sections.SectionProcess)section).TextUP = component.Text;
                }

                section.Data.ComponentID = component.ComponentID;
                section.Data.Title = component.Text;

                section.Data.TextHorizontalAlign = (Sections.SectionData.TextHAlign)component.HAlign;
                section.Data.TextVerticalAlign = (Sections.SectionData.TextVAlign)component.VAlign;

                section.Title = component.Text;
                //section.Position = new System.Drawing.PointF(component.X, component.Y);
                section.RectSize = new System.Drawing.SizeF(component.Width, component.Height);

				if (section.GetComponentType() == Sections.Section.ComponentType.GROUP)
				{
					PropertyGroup propertyGroup = (PropertyGroup)property;
					Sections.SectionDataGroup data = (Sections.SectionDataGroup)section.Data;
					foreach (int nItem in propertyGroup.GroupItems)
					{
						if (dicCompSection.ContainsKey(nItem))
						{
							Sections.Section sectionComp = dicCompSection[nItem];
							data.AddGroupMember(sectionComp);
						}						
						((Sections.SectionGroup)section).UpdateGroupRegion();	
					}
				}

				long id = (nStepMemberIndex << 32) | (uint)component.ID;
                dicSections[id] = section;
				dicCompSection[component.ID] = section;
                panel.Sections.Add(section);
            }
			panel.Refresh();
            return true;
        }

        private Sections.Section ToSection(Sections.PanelSectionEx panel, float x, float y, ComponentProperty property, Dictionary<PropertyLink, Sections.SectionLink> dicLinkSections, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined, ref Dictionary<int, Sections.ExternalTeamData> dicExternal, ref Dictionary<int, string> dicRegular, Dictionary<int, Sections.Section> dicCompSection, ref Dictionary<int, string> dicControlRoom)
        {
            if (property.Type == Sections.Section.ComponentType.PROCESS)
            {
                return ToSectionProcess(panel,x, y, (PropertyProcess)property, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom);
            }
            else if (property.Type == Sections.Section.ComponentType.DECISION)
            {
                return new Sections.SectionDecision(panel, x, y );
            }
            else if (property.Type == Sections.Section.ComponentType.ANNOTATION)
            {
				return new Sections.SectionAnnotation(panel, x, y);
            }
            else if (property.Type == Sections.Section.ComponentType.ENDPOINT)
            {
                return ToSectionEndPoint(panel, x, y, (PropertyEndPoint)property);
            }
            else if (property.Type == Sections.Section.ComponentType.LINK)
            {
				return ToSectionLink(panel, x, y, (PropertyLink)property, ref dicLinkSections);
            }
            else if (property.Type == Sections.Section.ComponentType.TRANSSOP)
            {
				return ToSectionTransSOP(panel, x, y, (PropertyTransSOP)property);
            }
            else if (property.Type == Sections.Section.ComponentType.INTERNAL)
            {
                return ToSectionInternal(panel, x, y, (PropertyInternal)property, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom);
            }
            else if (property.Type == Sections.Section.ComponentType.EXTERNAL)
            {
				return ToSectionExternal(panel, x, y, (PropertyExternal)property, ref dicExternal);
            }
            else if (property.Type == Sections.Section.ComponentType.TRANSMISSION)
            {
				return ToSectionTransmission(panel, x, y, (PropertyTransmission)property, ref dicExternal);
            }
			else if (property.Type == Sections.Section.ComponentType.GROUP)
			{
				return ToSectionGroup(panel, x, y, (PropertyGroup)property, dicCompSection);
			}

            return null;
        }

		private Sections.SectionGroup ToSectionGroup(Sections.PanelSectionEx panel, float x, float y, PropertyGroup property, Dictionary<int, Sections.Section> dicSections)
		{
			Sections.SectionGroup section = new Sections.SectionGroup(panel, x, y);
			
			return section;			
		}

		private Sections.SectionTransmission ToSectionTransmission(Sections.PanelSectionEx panel, float x, float y, PropertyTransmission property, ref Dictionary<int, Sections.ExternalTeamData> dicExternal)
        {
            Sections.SectionTransmission section = new Sections.SectionTransmission(panel,x, y);
            Sections.SectionDataTransmission data = (Sections.SectionDataTransmission)section.Data;

            data.DataInternal.UsePopupMessage = property.Internal.UsePopupMessage;
            data.DataInternal.UseMobileApp = property.Internal.UseSMS;
            data.DataInternal.UseBroadcast = property.Internal.UseBroadcast;
            data.DataInternal.BroadcastMessage = property.Internal.BroadcastMessage;

            data.DataExternal.UseSMS = property.External.UseSMS;
            data.DataExternal.SMSMessage = property.External.SMSMessage;
            data.DataExternal.UseFax = property.External.UseFax;

            if (!IOManager.GetExternalTeamList(property.External.SMSReceivers, data.DataExternal.SMSReceivers, dicExternal))
                return null;
            if (!IOManager.GetExternalTeamList(property.External.FaxReceivers, data.DataExternal.FaxReceivers, dicExternal))
                return null;

            return section;
        }

		private Sections.SectionExternal ToSectionExternal(Sections.PanelSectionEx panel, float x, float y, PropertyExternal property, ref Dictionary<int, Sections.ExternalTeamData> dicExternal)
        {
            Sections.SectionExternal section = new Sections.SectionExternal(panel, x, y);
            Sections.SectionDataExternal data = (Sections.SectionDataExternal)section.Data;
                      
            data.UseSMS = property.UseSMS;
            data.SMSMessage = property.SMSMessage;
            data.UseFax = property.UseFax;

            if (!IOManager.GetExternalTeamList(property.SMSReceivers, data.SMSReceivers, dicExternal))
                return null;
            if (!IOManager.GetExternalTeamList(property.FaxReceivers, data.FaxReceivers, dicExternal))
                return null;

            return section;
        }

        private Sections.SectionInternal ToSectionInternal(Sections.PanelSectionEx panel, float x, float y, PropertyInternal property, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined, ref Dictionary<int, Sections.ExternalTeamData> dicExternal, ref Dictionary<int, string> dicRegular, ref Dictionary<int, string> dicControlRoom)
        {
            Sections.SectionInternal section = new Sections.SectionInternal(panel, x, y);

            Sections.SectionData sectionData = section.Data;
            Sections.SectionDataInternal data = (Sections.SectionDataInternal)section.Data;

            IOManager.GetTeamList(FormMain.Instance.DBManager, property.TeamList, ref sectionData, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom);


            data.UsePopupMessage = property.UsePopupMessage;
            data.UseMobileApp = property.UseSMS;
            data.UseBroadcast = property.UseBroadcast;
            data.BroadcastMessage = property.BroadcastMessage;

            Sections.SOPTeam team = FindSOPTeam(property, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular);
            if (team != null)
            {
                data.Commander.Team = team;
            }
            data.Commander.DisplayText = property.CommanderDisplayText;
            data.Commander.IsTeamMember = property.CommanderIsTeamMember;
            data.Commander.TeamMemberID = property.CommanderTeamMemberID;

            return section;
        }

        private int GetLinkedActionStepID(PropertyTransSOP property)
        {
            string strFormat = "select id, DisasterID from ActionStep where StepName = '{0}' and DisasterID in ";
	        strFormat += "(select id from Disaster where DisasterName = '{1}' and SubDisasterID in ";
		    strFormat += "(select id from SubDisasterCategory where SubCategoryName = '{2}' and DisasterID in ";
			strFormat += "(select id from DisasterCategory where CategoryName = '{3}' and SiteID = {4})";

            WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strSQL = string.Format(strFormat, property.LinkedActionStepName, property.LinkedDisasterName, property.LinkedSubCategoryName, property.LinkedCategoryName, FormMain.Instance.SiteID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return -1;

            int nResultCount = arrResult.Count;
            if (nResultCount == 0)
                return -1;

			int nActionStepID = WebDBManager.GetIntField(arrResult[nResultCount - 2].ToString(), -1);
			int nDisasterID = WebDBManager.GetIntField(arrResult[nResultCount - 1].ToString(), -1);

            int nParentCount = property.ParentActionStepNameList.Count;
            if (nParentCount == 0)
                return nActionStepID;

            for (int i=nParentCount-1;i>=0;i--)
            {
                string strParentActionStepName = (string)property.ParentActionStepNameList[i];

                strSQL = string.Format("select id from ActionStep where DisasterID = {0} and StepName = {1} and ParentStepID = {2}",
                    nDisasterID, strParentActionStepName, i == nParentCount - 1 ? "NULL" : nActionStepID.ToString());

                arrResult = dbMgr.GetResultData(strSQL, 0);
                if (arrResult == null || arrResult.Count == 0)
                    return nActionStepID;

				nActionStepID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            }

            return nActionStepID;
		}

		private Sections.SectionTransSOP ToSectionTransSOP(Sections.PanelSectionEx panel, float x, float y, PropertyTransSOP property)
        {
            Sections.SectionTransSOP section = new Sections.SectionTransSOP(panel, x, y);
            Sections.SectionDataTransSOP data = (Sections.SectionDataTransSOP)section.Data;

            data.LinkedActionStepID = GetLinkedActionStepID(property);

            return section;
        }

		private Sections.SectionLink ToSectionLink(Sections.PanelSectionEx panel, float x, float y, PropertyLink property, ref Dictionary<PropertyLink, Sections.SectionLink> dicLinkSections)
        {
            Sections.SectionLink section = new Sections.SectionLink(panel, x, y);
            dicLinkSections[property] = section;
            return section;
        }

		private Sections.SectionEndPoint ToSectionEndPoint(Sections.PanelSectionEx panel, float x, float y, PropertyEndPoint property)
        {
            Sections.SectionEndPoint section = new Sections.SectionEndPoint(panel, x, y);
            Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)section.Data;
            data.IsBegin = property.IsBegin;
            return section;
        }

        private Sections.SectionProcess ToSectionProcess(Sections.PanelSectionEx panel, float x, float y, PropertyProcess property, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined, ref Dictionary<int, Sections.ExternalTeamData> dicExternal, ref Dictionary<int, string> dicRegular, ref Dictionary<int, string> dicControlRoom)
        {
            Sections.SectionProcess section = new Sections.SectionProcess(panel, x, y);
            Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;
            Sections.SectionData sectionData = section.Data;

            section.TextDown = IOManager.GetTeamList(FormMain.Instance.DBManager, property.TeamList, ref sectionData, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular, ref dicControlRoom);

            data.ProcessingTime.Time = property.ProcessTime;

            Sections.ProcessingTime.Type type = Sections.ProcessingTime.Type.UNKNOWN;
            if (!Sections.ProcessingTime.IntToType(property.ProcessTimeType, ref type))
                return null;

            data.ProcessingTime.ProcessingType = type;
            data.UseProcessingTime = property.UseProcessTime;
            data.MissionTransfer = property.UseMissionMessage;
            data.TransferTeamLeaderOnly = property.OnlyTeamLeader;

            foreach (PropertyMissionItem prop in property.Missions)
            {
                Sections.MissionItem item = new Sections.MissionItem();
                item.Commander = new Sections.SectionCommander();
                Sections.SOPTeam team2 = FindSOPTeam(prop, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular);
                if (team2 != null)
                {
                    item.Commander.Team = team2;
                }
                item.Commander.DisplayText = prop.CommanderDisplayText;
                item.Commander.IsTeamMember = prop.CommanderIsTeamMember;
                item.Commander.TeamMemberID = prop.CommanderTeamMemberID;

                item.Mission = prop.Mission;
                item.Target = prop.Target;
                item.TransmissionType = prop.TransmissionType;

                data.MissionItems.Add(item);
            }

            Sections.SOPTeam team = FindSOPTeam(property, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular);
            if( team != null)
            {
                data.Commander.Team = team;
            }
            data.Commander.DisplayText = property.CommanderDisplayText;
            data.Commander.IsTeamMember = property.CommanderIsTeamMember;
            data.Commander.TeamMemberID = property.CommanderTeamMemberID;

            return section;
        }

        private Sections.SOPTeam FindSOPTeam(ICommanderOwner owner,
                                            ref Dictionary<int, string> dicNormal, 
                                            ref Dictionary<int, string> dicEmergency,
                                            ref Dictionary<int, string> dicUserDefined, 
                                            ref Dictionary<int, Sections.ExternalTeamData> dicExternal, 
                                            ref Dictionary<int, string> dicRegular)
        {

            Sections.SOPTeam.SOPTeamType teamType = (Sections.SOPTeam.SOPTeamType)owner.CommanderTeamType;
            int nTeamID = owner.CommanderTeamID;
            string szTeamName = owner.CommanderTeamName;
            int nMemberID = owner.CommanderTeamMemberID;

            Sections.SOPTeam newTeam = new Sections.SOPTeam();
            newTeam.TeamID = nTeamID;
            newTeam.TeamName = szTeamName;
            newTeam.TeamType = teamType;


            return newTeam;
        }

        private ArrayList GetStepMembers()
        {
            if (m_arrActionSteps.Count == 0)
                return null;

            ActionStep actionStep = (ActionStep)m_arrActionSteps[0];
            return actionStep.StepMemberList;
        }

        private void ClearSOP(FormMain frm)
        {
			FormPageSOP pageLevel = frm.GetPageLevel();
            BarLevelTree tree = pageLevel.GetBarLevelTree();

            tree.ClearTree();

            Sections.SectionData.ClearIDList();
        }

		private bool Load(FormMain frm, XmlTextReader reader)
		{
			if (reader == null)
				return false;


			bool bResult = true;
			bool stop = false;

			try
			{
				while (reader.Read())
				{
					switch (reader.NodeType)
					{
						case XmlNodeType.Element:
							if (string.Compare(reader.Name, "Header", true) == 0)
							{
								if (!ReadHeader(reader))
								{
									bResult = false;
									break;
								}
							}
							else if (string.Compare(reader.Name, "Body", true) == 0)
							{
								if (!ReadBody(reader))
								{
									bResult = false;
									break;
								}
							}
							else
								PassElement(reader);
							break;

						case XmlNodeType.EndElement:
							stop = true;
							break;
					}

					if (stop)
						break;
				}
			}
			catch (Exception e)
			{
				m_strErrorMessage = e.Message;
				reader.Close();
				bResult = false;
			}

			if( bResult == true)
				reader.Close();


			return LoadScreen(frm);
		}

		public bool Load(FormMain frm, System.IO.Stream stream)
		{
			XmlTextReader reader = InitReader(stream);
			return Load(frm, reader);
		}

        public bool Load(FormMain frm, string strPath)
        {
            XmlTextReader reader = InitReader(strPath);
			return Load(frm, reader);
        }

        private bool GetXMLSchemaLocation(string strPath, ref string strSchemaLocation)
        {
            XmlTextReader reader = null;
            bool stop = false;

            try
            {
                reader = new XmlTextReader(strPath);

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "SOP", true) != 0)
                            {
                                reader.Close();
                                return false;
                            }
                            else
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    if (string.Compare(reader.Name, "xsi:noNamespaceSchemaLocation", true) == 0 ||
                                        string.Compare(reader.Name, "xsi:schemaLocation", true) == 0)
                                    {
                                        strSchemaLocation = reader.Value;
                                        reader.Close();
                                        return true;
                                    }
                                }
                            }

                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception)
            {
                reader.Close();
            }

            reader.Close();
            return false;
        }

        private bool SchemaValidationCheck(string strPath)
        {
            XmlReader reader = null;

            try
            {
                string strSchemaLocation = "";
                if (!GetXMLSchemaLocation(strPath, ref strSchemaLocation))
                    return true;

                XmlReaderSettings settings = new XmlReaderSettings();

                try
                {
                    settings.Schemas.Add("", strSchemaLocation);
                }
                catch (Exception)
                {
                    // 유효하지 않은 스키마 경로
                    return true;
                }

                settings.ValidationType = ValidationType.Schema;
                settings.ValidationEventHandler += new System.Xml.Schema.ValidationEventHandler(settings_ValidationEventHandler);
                
                reader = XmlReader.Create(strPath, settings);

                while (reader.Read())
                {
                }
            }
            catch (Exception e)
            {
                reader.Close();
                MessageBox.Show(e.Message, "XML 유효성 검증 실패");
                //m_strErrorMessage = e.Message;
                return false;
            }

            reader.Close();
            return true;
        }

		private XmlTextReader InitReader(System.IO.Stream strem)
		{
			
			XmlTextReader reader = null;

			try
			{
				reader = new XmlTextReader(strem);

				while (reader.Read())
				{
					switch (reader.NodeType)
					{
						case XmlNodeType.Element:
							if (string.Compare(reader.Name, "SOP", true) != 0)
							{
								m_strErrorMessage = "SOP XML이 아닙니다.";
								reader.Close();
								return null;
							}
							return reader;
					}
				}
			}
			catch (Exception e)
			{
				m_strErrorMessage = e.Message;
				reader.Close();
				return null;
			}

			reader.Close();
			return reader;
		}

        private XmlTextReader InitReader(string strPath)
        {
            //if (!SchemaValidationCheck(strPath))
            //{
            //    if (MessageBox.Show("XML 스키마에 맞지 않는 데이터 파일입니다.\r\n계속 진행하시겠습니까?", "XML 스키마 오류", MessageBoxButtons.YesNo) == DialogResult.No)
             //       return null;
           // }

            XmlTextReader reader = null;
            
            try
            {
                reader = new XmlTextReader(strPath);
                
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "SOP", true) != 0)
                            {
                                m_strErrorMessage = "SOP XML이 아닙니다.";
                                reader.Close();
                                return null;
                            }
                            return reader;
                    }
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                reader.Close();
                return null;
            }

            reader.Close();
            return reader;
        }

        void settings_ValidationEventHandler(object sender, System.Xml.Schema.ValidationEventArgs e)
        {
            throw new Exception(string.Format("Line Number {0}, {1}", e.Exception.LineNumber, e.Message));
            //MessageBox.Show(e.Message);
            //throw new NotImplementedException();
        }

        private bool ReadHeader(XmlTextReader reader)
        {
            bool stop = false, readCategory = false, readSubCategory = false, readDisaster = false, readSOPVersion = false;
            bool readRegular = false, readNormal = false;
            
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Category", true) == 0)
                        {
                            if (!ReadCategory(reader))
                                return false;
                            else
                                readCategory = true;
                        }
                        else if (string.Compare(reader.Name, "SubCategory", true) == 0)
                        {
                            if (!ReadSubCategory(reader))
                                return false;
                            else
                                readSubCategory = true;
                        }
                        else if (string.Compare(reader.Name, "Disaster", true) == 0)
                        {
                            if (!ReadDisaster(reader))
                                return false;
                            else
                                readDisaster = true;
                        }
                        else if (string.Compare(reader.Name, "Regular", true) == 0)
                        {
                            if (!ReadRegular(reader))
                                return false;
                            else
                                readRegular = true;
                        }
                        else if (string.Compare(reader.Name, "Normal", true) == 0)
                        {
                            if (!ReadNormal(reader))
                                return false;
                            else
                                readNormal = true;
                        }
                        else if (string.Compare(reader.Name, "SOPVersion", true) == 0)
                        {
                            if (!ReadSOPVersion(reader))
                                return false;
                            else
                                readSOPVersion = true;
                        }
                        else
                            PassElement(reader);

                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            if (!readCategory)
                m_strErrorMessage = "Category 정보를 찾을 수 없습니다.";
            else if (!readSubCategory)
                m_strErrorMessage = "SubCategory 정보를 찾을 수 없습니다.";
            else if (!readDisaster)
                m_strErrorMessage = "Disaster 정보를 찾을 수 없습니다.";
            else if (!readRegular)
                m_strErrorMessage = "Regular 정보를 찾을 수 없습니다.";
            else if (!readNormal)
                m_strErrorMessage = "Normal 정보를 찾을 수 없습니다.";
            else if (!readSOPVersion)
                m_strErrorMessage = "SOPVersion 정보를 찾을 수 없습니다.";

            return readCategory && readSubCategory && readDisaster && readRegular && readNormal && readSOPVersion;
        }

        private void PassElement(XmlTextReader reader)
        {
            if (reader.IsEmptyElement)
                return;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        return;
                }
            }
        }

        private bool ReadRegular(XmlTextReader reader)
        {
            return ReadBoolean(reader, ref m_isRegular, "Regular가", "Regular는");
        }

        private bool ReadNormal(XmlTextReader reader)
        {
            return ReadBoolean(reader, ref m_isNormal, "Normal이", "Normal은");
        }

        private bool ReadCategory(XmlTextReader reader)
        {
            if (reader.IsEmptyElement)
                return false;

            bool stop = false, readCategory = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Text:
                        m_strCategoryName = reader.Value;
                        readCategory = true;
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            if (!readCategory)
                m_strErrorMessage = "Category 이름을 찾을수 없습니다.";
            return readCategory;
        }

        private bool ReadSubCategory(XmlTextReader reader)
        {
            if (reader.IsEmptyElement)
                return false;

            bool stop = false, readSubCategory = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Text:
                        m_strSubCategoryName = reader.Value;
                        readSubCategory = true;
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            if (!readSubCategory)
                m_strErrorMessage = "SubCategory 이름을 찾을수 없습니다.";
            return readSubCategory;
        }

        private bool ReadDisaster(XmlTextReader reader)
        {
            if (reader.IsEmptyElement)
                return false;

            bool stop = false, readDisaster = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Text:
                        m_strDisasterName = reader.Value;
                        readDisaster = true;
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            if (!readDisaster)
                m_strErrorMessage = "Disaster 이름을 찾을수 없습니다.";
            return readDisaster;
        }

        private bool ReadSOPVersion(XmlTextReader reader)
        {
            if (reader.IsEmptyElement)
                return false;

            bool stop = false, readVersion = false;

            while (reader.MoveToNextAttribute())
            {
                if (string.Compare(reader.Name, "description", true) == 0)
                {
                    m_strDescription = reader.Value;
                }
            }

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Text:
                        m_strVersionName = reader.Value;
                        readVersion = true;
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            if (!readVersion)
                m_strErrorMessage = "Version 이름을 찾을수 없습니다.";

            return readVersion;
        }

        private bool ReadBody(XmlTextReader reader)
        {
            bool stop = false, readActionStepList = false;

             while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "ActionStepList", true) == 0)
                        {
                            if (!ReadActionStepList(reader))
                                return false;

                            readActionStepList = true;
                        }
                        else
                            PassElement(reader);

                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            if (!readActionStepList)
                m_strErrorMessage = "ActionStepList 정보를 찾을 수 없습니다.";

            return readActionStepList;
        }

        private bool ReadActionStepList(XmlTextReader reader)
        {
            bool stop = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "ActionStep", true) == 0)
                        {
                            if (!ReadActionStep(reader))
                                return false;
                        }
                        else
                            PassElement(reader);

                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            if (m_arrActionSteps.Count == 0)
            {
                m_strErrorMessage = "ActionStep 데이터가 존재하지 않습니다.";
                return false;
            }

            return true;
        }

        private bool ReadActionStep(XmlTextReader reader)
        {
            int nActionStepID = -1;
			bool bSelected = false;
            while (reader.MoveToNextAttribute())
            {
                if (string.Compare(reader.Name, "id", true) == 0)
                {
                    try
                    {
                        nActionStepID = int.Parse(reader.Value);
                    }
                    catch (Exception)
                    {
                        m_strErrorMessage = string.Format("Line Number {0}, 정수 형태로 변환할 수 없는 ActionStep id=\"{1}\"가 존재합니다.",
                            reader.LineNumber, reader.Value);
                        return true;
                    }
                }
				if (string.Compare(reader.Name, "selected", true) == 0)
				{
					try
					{
						int nSelect = int.Parse(reader.Value);
						bSelected = ((nSelect == 1) ? true : false);
					}
					catch (Exception)
					{
						m_strErrorMessage = string.Format("Line Number {0}, 정수 형태로 변환할 수 없는 ActionStep id=\"{1}\"가 존재합니다.",
							reader.LineNumber, reader.Value);
						return true;
					}
				}
            }

            //if (nActionStepID < 0)
            //{
            //    m_strErrorMessage = string.Format("Line Number {0}, id가 없는 ActionStep Element가 존재합니다.", reader.LineNumber);
            //    return true;
            //}

            bool stop = false, readStepName = false, readPeriodType = false, readWeekDayOption = false;
            bool readIteration = false, readIterationType = false, readProcessTime = false;

            string strStepName = "";
            int nPeriodType = -1, nWeekDayOption = -1, nIteration = -1, nIterationType = -1, nProcessTime = -1;
            
            ActionStep actionStep = new ActionStep();
            actionStep.ID = nActionStepID;
			actionStep.Selected = bSelected;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "StepName", true) == 0)
                        {
                            if (!ReadStepName(reader, ref strStepName))
                                return false;
                            
                            readStepName = true;
                            actionStep.StepName = strStepName;
                        }
                        else if (string.Compare(reader.Name, "PeriodType", true) == 0)
                        {
                            if (!ReadPeriodType(reader, ref nPeriodType))
                                return false;
                            
                            readPeriodType = true;
                            actionStep.PeriodType = nPeriodType;
                        }
                        else if (string.Compare(reader.Name, "BeginTime", true) == 0)
                        {
                            DateTime dtBegin = new DateTime();
                            if (!ReadBeginTime(reader, ref dtBegin))
                                return false;
                            else
                                actionStep.BeginTime = dtBegin;
                        }
                        else if (string.Compare(reader.Name, "EndTime", true) == 0)
                        {
                            DateTime dtEnd = new DateTime();
                            if (!ReadEndTime(reader, ref dtEnd))
                                return false;
                            else
                                actionStep.EndTime = dtEnd;
                        }
                        else if (string.Compare(reader.Name, "WeekDayOption", true) == 0)
                        {
                            if (!ReadWeekDayOption(reader, ref nWeekDayOption))
                                return false;
                            
                            readWeekDayOption = true;
                            actionStep.WeekdayOption = nWeekDayOption;
                        }
                        else if (string.Compare(reader.Name, "Iteration", true) == 0)
                        {
                            if (!ReadIteration(reader, ref nIteration))
                                return false;
                            
                            readIteration = true;
                            actionStep.Iteration = nIteration;
                        }
                        else if (string.Compare(reader.Name, "IterationType", true) == 0)
                        {
                            if (!ReadIterationType(reader, ref nIterationType))
                                return false;
                            
                            readIterationType = true;
                            actionStep.IterationType = nIterationType;
                        }
                        else if (string.Compare(reader.Name, "ProcessTime", true) == 0)
                        {
                            if (!ReadProcessTime(reader, ref nProcessTime))
                                return false;
                            
                            readProcessTime = true;
                            actionStep.ProcessTime = nProcessTime;
                        }
                        else if (string.Compare(reader.Name, "ProcessTimeType", true) == 0)
                        {
                            int nProcessTimeType = -1;

                            if (ReadProcessTimeType(reader, ref nProcessTimeType))
                                actionStep.ProcessTimeType = nProcessTimeType;
                        }
                        else if (string.Compare(reader.Name, "ParentStepID", true) == 0)
                        {
                            int nParentStepID = -1;

                            if (ReadParentStepID(reader, ref nParentStepID))
                                actionStep.ParentStepID = nParentStepID;
                        }
                        else if (string.Compare(reader.Name, "StepMemberList", true) == 0)
                        {
                            if (!ReadStepMemberList(reader, actionStep.StepMemberList))
                                return false;
                        }
                        else
                            PassElement(reader);

                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            if (!readStepName)
            {
                m_strErrorMessage = string.Format("Line Number {0}, StepName이 존재하지 않습니다.", reader.LineNumber);
                return false;
            }

            if (!readPeriodType)
            {
                m_strErrorMessage = string.Format("Line Number {0}, PeriodType이 존재하지 않습니다.", reader.LineNumber);
                return false;
            }

            if (!readWeekDayOption)
            {
                m_strErrorMessage = string.Format("Line Number {0}, WeekDayOption이 존재하지 않습니다.", reader.LineNumber);
                return false;
            }

            if (!readIteration)
            {
                m_strErrorMessage = string.Format("Line Number {0}, Iteration이 존재하지 않습니다.", reader.LineNumber);
                return false;
            }

            if (!readIterationType)
            {
                m_strErrorMessage = string.Format("Line Number {0}, IterationType이 존재하지 않습니다.", reader.LineNumber);
                return false;
            }

            if (!readProcessTime)
            {
                m_strErrorMessage = string.Format("Line Number {0}, ProcessTime이 존재하지 않습니다.", reader.LineNumber);
                return false;
            }
            else if (nProcessTime >= 0 && nProcessTime <= 4 && actionStep.ProcessTimeType < 0)
            {
                m_strErrorMessage = string.Format("Line Number {0}, ProcessTime이 {1}인데 ProcessTimeType이 존재하지 않습니다.", reader.LineNumber, nProcessTime);
                return false;
            }

            //if (!readStepMemberList)
            //{
            //    m_strErrorMessage = string.Format("Line Number {0}, StepMemberList가 존재하지 않습니다.", reader.LineNumber);
            //    return false;
            //}

            m_arrActionSteps.Add(actionStep);
            return true;
        }

        private bool ReadParentStepID(XmlTextReader reader, ref int nParentStepID)
        {
            ReadInt(reader, ref nParentStepID, "ParentStepID는", "ParentStepID가");
			return true;
        }

        private bool ReadProcessTimeType(XmlTextReader reader, ref int nProcessTimeType)
        {
            ReadInt(reader, ref nProcessTimeType, "ProcessTimeType은", "ProcessTimeType이");
			return true;
        }

        private bool ReadVAlign(XmlTextReader reader, ref int nVAlign)
        {
            ReadInt(reader, ref nVAlign, "VAlign은", "VAlign이");
            return true;
        }

        private bool ReadHAlign(XmlTextReader reader, ref int nHAlign)
        {
            ReadInt(reader, ref nHAlign, "HAlign은", "HAlign이");
            return true;
        }
        private bool ReadFontStyle(XmlTextReader reader, ref int nFontStyle)
        {
            ReadInt(reader, ref nFontStyle, "FontStyle은", "FontStyle이");
            return true;
        }

        private bool ReadTextColor(XmlTextReader reader, ref int nFontStyle)
        {
            ReadInt(reader, ref nFontStyle, "FontColor는", "FontColor가");
            return true;
        }

        private bool ReadInt(XmlTextReader reader, ref int nData, string strMessage1, string strMessage2)
        {
            string strText = "";

            if (ReadElementText(reader, ref strText))
            {
                try
                {
                    nData = int.Parse(strText);
                }
                catch (Exception)
                {
                    m_strErrorMessage = string.Format("Line Number {0}, {1} 정수 형태이어야만 합니다.", reader.LineNumber, strMessage1);
                    return false;
                }
            }
            else
            {
                m_strErrorMessage = string.Format("Line Number {0}, {1} 비어있습니다.", reader.LineNumber, strMessage2);
                return false;
            }

            return true;
        }

        private bool ReadBeginTime(XmlTextReader reader, ref DateTime dtBegin)
        {
            ReadDateTime(reader, ref dtBegin, "BeginTime");
			return true;
        }

        private bool ReadEndTime(XmlTextReader reader, ref DateTime dtEnd)
        {
            if (reader.IsEmptyElement)
                return true;
            return ReadDateTime(reader, ref dtEnd, "EndTime");
        }

        private bool ReadDateTime(XmlTextReader reader, ref DateTime dt, string strItemName)
        {
            string strText = "";

            if (ReadElementText(reader, ref strText))
            {
                try
                {
                    dt = Convert.ToDateTime(strText);
                }
                catch (Exception)
                {
                    m_strErrorMessage = string.Format("Line Number {0}, {1}은 날짜/시간 Type의 데이터가 아닙니다.", reader.LineNumber, strItemName);
                    return false;
                }
            }
            else
            {
                m_strErrorMessage = string.Format("Line Number {0}, {1}이 비어있습니다.", reader.LineNumber, strItemName);
                return false;
            }

            return true;
        }

        private bool ReadStepMemberList(XmlTextReader reader, ArrayList arrStepMemberList)
        {
            bool stop = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "StepMember", true) == 0)
                        {
                            StepMember stepMember = ReadStepMember(reader);
                            if (stepMember == null)
                                return false;

                            arrStepMemberList.Add(stepMember);
                        }
                        else
                            PassElement(reader);

                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            if (arrStepMemberList.Count == 0)
            {
                m_strErrorMessage = string.Format("Line Number {0}, StepMember가 존재하지 않는 StepMemberList가 있습니다.", reader.LineNumber);
                return false;
            }

            return true;
        }

        private StepMember ReadStepMember(XmlTextReader reader)
        {
            int nTeamID = -1;
            int nTeamType = -1;
			int nOrgTeamID = -1;
            string strStepMemberName = "";
			bool bModify = false;

            while (reader.MoveToNextAttribute())
            {
                if (string.Compare(reader.Name, "id", true) == 0)
                {
                    try
                    {
                        nTeamID = int.Parse(reader.Value);
                    }
                    catch (Exception)
                    {
                        m_strErrorMessage = string.Format("Line Number {0}, StepMember id는 정수이어야 합니다.", reader.LineNumber);
                        return null;
                    }
                }
                else if (string.Compare(reader.Name, "type", true) == 0)
                {
                    try
                    {
                        nTeamType = int.Parse(reader.Value);
                    }
                    catch (Exception)
                    {
                        m_strErrorMessage = string.Format("Line Number {0}, StepMember type은 정수이어야 합니다.", reader.LineNumber);
                        return null;
                    }
                }
				else if (string.Compare(reader.Name, "teamid", true) == 0)
				{
					try
					{
						nOrgTeamID = int.Parse(reader.Value);
					}
					catch (Exception)
					{
						m_strErrorMessage = string.Format("Line Number {0}, StepMember OriginalIDs는 정수이어야 합니다.", reader.LineNumber);
						return null;
					}
				}
				else if (string.Compare(reader.Name, "modify", true) == 0)
				{
					
					if(reader.Value.ToLower() == "true")
					{
						bModify = true;
					}
					else
					{
						bModify = false;
					}
					
					
				}
                else if (string.Compare(reader.Name, "name", true) == 0)
                {
                    strStepMemberName = reader.Value;
                }
            }

            if (nTeamID < 0)
            {
                m_strErrorMessage = string.Format("Line Number {0}, StepMember id가 존재하지 않습니다.", reader.LineNumber);
                return null;
            }

            if (nTeamType < 0)
            {
                m_strErrorMessage = string.Format("Line Number {0}, StepMember type이 존재하지 않습니다.", reader.LineNumber);
                return null;
            }

            if (strStepMemberName.Length == 0)
            {
                m_strErrorMessage = string.Format("Line Number {0}, StepMember name이 존재하지 않습니다.", reader.LineNumber);
                return null;
            }

            StepMember stepMember = new StepMember();
			stepMember.TeamID = nOrgTeamID;
            stepMember.TeamType = (Sections.SOPTeam.SOPTeamType)nTeamType;
            stepMember.TeamName = strStepMemberName;
			stepMember.StepMemberID = nTeamID;
			stepMember.Modify = bModify;
            bool stop = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "ComponentList", true) == 0)
                        {
                            if (!ReadComponentList(reader, stepMember))
                                return null;                            
                        }
                        else if (string.Compare(reader.Name, "ArrowList", true) == 0)
                        {
                            if (!ReadArrowList(reader, stepMember))
                                return null;                           
                        }
                        else
                            PassElement(reader);

                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return stepMember;
        }

        private bool ReadArrowList(XmlTextReader reader, StepMember stepMember)
        {
            if (reader.IsEmptyElement)
                return true;

            bool stop = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Arrow", true) == 0)
                        {
							ReadArrow(reader, stepMember.ArrowList);                             
                        }
                        else
                            PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return true;
        }

        private bool ReadArrow(XmlTextReader reader, ArrayList arrArrows)
        {
            bool stop = false;
           
            string strText = "";
            int nBeginComponentID = -1, nBeginComponentPosition = -1;
            int nEndComponentID = -1, nEndComponentPosition = -1;
            Arrow arrow = new Arrow();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Text", true) == 0)
                        {
                            if (ReadText(reader, ref strText))
                                arrow.Text = strText;
                        }
                        else if (string.Compare(reader.Name, "BeginComponentID", true) == 0)
                        {
							if (ReadBeginComponentID(reader, ref nBeginComponentID))
							{								
								arrow.BeginComponentID = nBeginComponentID;
							}
                        }
                        else if (string.Compare(reader.Name, "BeginComponentPosition", true) == 0)
                        {
							if (ReadBeginComponentPosition(reader, ref nBeginComponentPosition))
							{								
								arrow.BeginComponentPosition = nBeginComponentPosition;
							}                           
                        }
                        else if (string.Compare(reader.Name, "EndComponentID", true) == 0)
                        {
							if (ReadEndComponentID(reader, ref nEndComponentID))
							{								
								arrow.EndComponentID = nEndComponentID;
							}
                        }
                        else if (string.Compare(reader.Name, "EndComponentPosition", true) == 0)
                        {
							if (ReadEndComponentPosition(reader, ref nEndComponentPosition))
							{								
								arrow.EndComponentPosition = nEndComponentPosition;
							}
                        }
                        else
                            PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }		

            arrArrows.Add(arrow);
            return true;
        }

        private bool ReadBeginComponentID(XmlTextReader reader, ref int nBeginComponentID)
        {
            ReadInt(reader, ref nBeginComponentID, "BeginComponentID는", "BeginComponentID가");
			return true;
        }

        private bool ReadBeginComponentPosition(XmlTextReader reader, ref int nBeginComponentPosition)
        {
            ReadInt(reader, ref nBeginComponentPosition, "BeginComponentPosition은", "BeginComponentPosition은");
			return true;
        }

        private bool ReadEndComponentID(XmlTextReader reader, ref int nEndComponentID)
        {
            ReadInt(reader, ref nEndComponentID, "EndComponentID는", "EndComponentID가");
			return true;
        }

        private bool ReadEndComponentPosition(XmlTextReader reader, ref int nEndComponentPosition)
		{
            ReadInt(reader, ref nEndComponentPosition, "EndComponentPosition은", "EndComponentPosition은");
			return true;
        }

		private Viewport ReadViewport(XmlTextReader reader)
		{
			Viewport viewport = new Viewport();

			while (reader.MoveToNextAttribute())
			{
				string szValue = reader.Value;
				if (string.Compare(reader.Name, "OriginX", true) == 0)
				{
					float fValue;
					if (float.TryParse(szValue, out fValue))
						viewport.OriginX = fValue;
				}
				else if (string.Compare(reader.Name, "OriginY", true) == 0)
				{
					float fValue;
					if (float.TryParse(szValue, out fValue))
						viewport.OriginY = fValue;
				}
				else if (string.Compare(reader.Name, "CurrentX", true) == 0)
				{
					float fValue;
					if (float.TryParse(szValue, out fValue))
						viewport.CurrentX = fValue;
				}
				else if (string.Compare(reader.Name, "CurrentY", true) == 0)
				{
					float fValue;
					if (float.TryParse(szValue, out fValue))
						viewport.CurrentY = fValue;
				}				
				else if (string.Compare(reader.Name, "ScaleX", true) == 0)
				{
					float fValue;
					if (float.TryParse(szValue, out fValue))
						viewport.Scale = fValue;
				}
				else if (string.Compare(reader.Name, "ScaleY", true) == 0)
				{
					float fValue;
					if (float.TryParse(szValue, out fValue))
						viewport.PrevScale = fValue;
				}
			}
			bool stop = false;
			while (reader.Read())
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:
						PassElement(reader);
						break;

					case XmlNodeType.EndElement:
						stop = true;
						break;
				}

				if (stop)
					break;
			}
			return viewport;
		}

        private bool ReadComponentList(XmlTextReader reader, StepMember stepMember)
        {
            bool stop = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Component", true) == 0)
                        {
                            Component component = ReadComponent(reader);
                            if (component != null)
								stepMember.ComponentList.Add(component);
                        }
						else if (string.Compare(reader.Name, "Viewport", true) == 0)
						{
							Viewport viewport = ReadViewport(reader);
							if (viewport == null)
								return false;

							stepMember.Viewport = viewport;
						}
                        else
                            PassElement(reader);
                        break;
					

                    case XmlNodeType.EndElement:
						if (string.Compare(reader.Name, "ComponentList", true) == 0)
							stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return true;
        }

        private Component ReadComponent(XmlTextReader reader)
        {
            int nID = -1;
            
            while (reader.MoveToNextAttribute())
            {
                if (string.Compare(reader.Name, "id", true) == 0)
                {
                    try
                    {
                        nID = int.Parse(reader.Value);
                    }
                    catch (Exception)
                    {
                        m_strErrorMessage = string.Format("Line Number {0}, Component id는 정수이어야 합니다.", reader.LineNumber);
                        return null;
                    }
                }
            }

            bool stop = false, readX = false, readY = false, readWidth = false, readHeight = false;
            bool readComponentID = false, readProperty = false;

            float x = 0.0f, y = 0.0f, fWidth = 0.0f, fHeight = 0.0f;
            string strText = "", strComponentID = "";

            Component component = new Component();
            component.ID = nID;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "X", true) == 0)
                        {
                            if (!ReadX(reader, ref x))
                                return null;
                            
                            readX = true;
                            component.X = x;
                        }
                        else if (string.Compare(reader.Name, "Y", true) == 0)
                        {
                            if (!ReadY(reader, ref y))
                                return null;
                                
                            readY = true;
                            component.Y = y;
                        }
                        else if (string.Compare(reader.Name, "Width", true) == 0)
                        {
                            if (!ReadWidth(reader, ref fWidth))
                                return null;
                            
                            readWidth = true;
                            component.Width = fWidth;
                        }
                        else if (string.Compare(reader.Name, "Height", true) == 0)
                        {
                            if (!ReadHeight(reader, ref fHeight))
                                return null;
                            
                            readHeight = true;
                            component.Height = fHeight;
                        }
                        else if (string.Compare(reader.Name, "Text", true) == 0)
                        {
                            if (ReadText(reader, ref strText))
                            {
                                component.Text = strText;
                            }
                        }
                        else if (string.Compare(reader.Name, "VAlign", true) == 0)
                        {
                            int vAling = 0;
                            if (ReadVAlign(reader, ref vAling))
                            {
                                component.VAlign = vAling;
                            }
                        }
                        else if (string.Compare(reader.Name, "HAlign", true) == 0)
                        {
                            int hAling = 0;
                            if (ReadHAlign(reader, ref hAling))
                            {
                                component.HAlign = hAling;
                            }
                        }
                        else if (string.Compare(reader.Name, "ComponentID", true) == 0)
                        {
                            if (!ReadComponentID(reader, ref strComponentID))
                                return null;
                                                            
                            readComponentID = true;
                            component.ComponentID = strComponentID;
                        }
                        else if (string.Compare(reader.Name, "FontName", true) == 0)
                        {
                            string szFontName = "";
                            if (!ReadFontName(reader, ref szFontName))
                                return null;                            
                            component.FontName = szFontName;
                        }
                        else if (string.Compare(reader.Name, "FontStyle", true) == 0)
                        {
                            int nFontStyle = 0;
                            if (!ReadFontStyle(reader, ref nFontStyle))
                            {
                                return null;
                            }
                            component.FontStyle = nFontStyle;
                        }
                        else if (string.Compare(reader.Name, "FontColor", true) == 0)
                        {
                            int nFontColor = 0;
                            if (!ReadTextColor(reader, ref nFontColor))
                            {
                                return null;
                            }
                            component.TextColor = nFontColor;
                        }
                        else if (string.Compare(reader.Name, "FontSize", true) == 0)
                        {
                            float fFontSize = 0.0f;
                            if (!ReadFontSize(reader, ref fFontSize))
                            {
                                return null;
                            }
                            component.FontSize = fFontSize;
                        }
                        else if (string.Compare(reader.Name, "LineSpace", true) == 0)
                        {
                            float fLineSpace = 0.0f;
                            if (!ReadLineSpace(reader, ref fLineSpace))
                            {
                                return null;
                            }
                            component.LineSpace = fLineSpace;
                        }
                        else if (string.Compare(reader.Name, "Property", true) == 0)
                        {
                            component.Property = ReadProperty(reader);
                            if (component.Property == null)
                                return null;

                            readProperty = true;
                        }
                        else
                            PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
						if (string.Compare(reader.Name, "Component", true) == 0)
							stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            if (!readX)
            {
                m_strErrorMessage = string.Format("Line Number {0}, X가 존재하지 않습니다.", reader.LineNumber);
                return null;
            }

            if (!readY)
            {
                m_strErrorMessage = string.Format("Line Number {0}, Y가 존재하지 않습니다.", reader.LineNumber);
                return null;
            }

            if (!readWidth)
            {
                m_strErrorMessage = string.Format("Line Number {0}, Width가 존재하지 않습니다.", reader.LineNumber);
                return null;
            }

            if (!readHeight)
            {
                m_strErrorMessage = string.Format("Line Number {0}, Height가 존재하지 않습니다.", reader.LineNumber);
                return null;
            }

            if (!readComponentID)
            {
                m_strErrorMessage = string.Format("Line Number {0}, ComponentID가 존재하지 않습니다.", reader.LineNumber);
                return null;
            }

            if (!readProperty)
            {
                m_strErrorMessage = string.Format("Line Number {0}, Property가 존재하지 않습니다.", reader.LineNumber);
                return null;
            }

            return component;
        }

        private ComponentProperty ReadProperty(XmlTextReader reader)
        {
            int nType = -1;

            while (reader.MoveToNextAttribute())
            {
                if (string.Compare(reader.Name, "type", true) == 0)
                {
                    try
                    {
                        nType = int.Parse(reader.Value);
                    }
                    catch (Exception)
                    {
                        m_strErrorMessage = string.Format("Line Number {0}, Property type은 정수이어야 합니다.", reader.LineNumber);
                        return null;
                    }
                }
            }

            ComponentProperty property = null;

			if (nType == (int)Sections.Section.ComponentType.PROCESS)
				property = ReadProcessProperty(reader);
			else if (nType == (int)Sections.Section.ComponentType.ENDPOINT)
				property = ReadEndPointProperty(reader);
			else if (nType == (int)Sections.Section.ComponentType.LINK)
				property = ReadLinkProperty(reader);
			else if (nType == (int)Sections.Section.ComponentType.TRANSSOP)
				property = ReadTransSOPProperty(reader);
			else if (nType == (int)Sections.Section.ComponentType.INTERNAL)
				property = ReadInternalProperty(reader);
			else if (nType == (int)Sections.Section.ComponentType.EXTERNAL)
				property = ReadExternalProperty(reader);
			else if (nType == (int)Sections.Section.ComponentType.TRANSMISSION)
				property = ReadTransmissionProperty(reader);
			else if (nType == (int)Sections.Section.ComponentType.GROUP)
				property = ReadGroupProperty(reader);
			else
				property = new ComponentProperty();

            if (property == null)
                return null;

            property.Type = (Sections.Section.ComponentType)nType;
            return property;
        }
		private PropertyGroup ReadGroupProperty(XmlTextReader reader)
		{
			PropertyGroup property = new PropertyGroup();
			bool stop = false;
			while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "GroupItems", true) == 0)
                        {
							string groupItem = "";
							if (ReadText(reader, ref groupItem))
							{
								string [] sep = {","};
								string [] items = groupItem.Split(sep, StringSplitOptions.RemoveEmptyEntries);
								for(int i = 0 ; i < items.Length ; i++)
								{
									string szItem = items[i].Replace(",", "");
									int nItem = 0;
									if( int.TryParse(szItem, out nItem))
										property.AddItem(nItem);
								}								
							}
                        }                        
                        else
                            PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

			return property;
		}
        private PropertyExternal ReadExternalProperty(XmlTextReader reader)
        {
            bool stop = false;

            PropertyExternal property = new PropertyExternal();

            bool useSMS = false, useFax = false;
            string strSMSText = "", strSMSExternalTeamIDList = " ", strFaxExternalTeamIDList = " ";

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "useSMS", true) == 0)
                        {
							ReadUseSMS(reader, ref useSMS);
                           
                            //readUseSMS = true;
                            property.UseSMS = useSMS;
                        }
                        else if (string.Compare(reader.Name, "SMSText", true) == 0)
                        {
							ReadSMSText(reader, ref strSMSText);


							if (strSMSText == "null")
								strSMSText = "";
                           // readSMSText = true;
                            property.SMSMessage = strSMSText;
                            
                        }
                        else if (string.Compare(reader.Name, "SMSExternalTeamIDList", true) == 0)
                        {
                            ReadSMSExternalTeamIDList(reader, ref strSMSExternalTeamIDList);
							if (strSMSExternalTeamIDList == "null")
								strSMSExternalTeamIDList = "";
                            //readSMSExternalTeamIDList = true;
                            property.SMSReceivers = strSMSExternalTeamIDList;
                            
                        }
                        else if (string.Compare(reader.Name, "useFax", true) == 0)
                        {
							ReadUseFax(reader, ref useFax);                      

                            //readUseFax = true;
                            property.UseFax = useFax;
                        }
                        else if (string.Compare(reader.Name, "FaxExternalTeamIDList", true) == 0)
                        {
							ReadFaxExternalTeamIDList(reader, ref strFaxExternalTeamIDList);
							if (strFaxExternalTeamIDList == "null")
								strFaxExternalTeamIDList = "";
                            //readFaxExternalTeamIDList = true;
							property.FaxReceivers = strFaxExternalTeamIDList;
                            
                        }
                        else
                            PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }  

            return property;
        }

        private PropertyTransmission ReadTransmissionProperty(XmlTextReader reader)
        {
            bool stop = false, readInternal = false, readExternal = false;
            PropertyTransmission property = new PropertyTransmission();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Internal", true) == 0)
                        {
                            property.Internal = ReadInternalProperty(reader);
                            if (property.Internal == null)
                                return null;

                            readInternal = true;
                        }
                        else if (string.Compare(reader.Name, "External", true) == 0)
                        {
                            property.External = ReadExternalProperty(reader);
                            if (property.External == null)
                                return null;

                            readExternal = true;
                        }
                        else
                            PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            if (!readInternal)
            {
                m_strErrorMessage = string.Format("Line Number {0}, TRANSMISSION property에 Internal이 존재하지 않습니다.", reader.LineNumber);
                return null;
            }

            if (!readExternal)
            {
                m_strErrorMessage = string.Format("Line Number {0}, TRANSMISSION property에 External이 존재하지 않습니다.", reader.LineNumber);
                return null;
            }
            
            return property;
        }

        private bool ReadUseSMS(XmlTextReader reader, ref bool useSMS)
        {
            return ReadBoolean(reader, ref useSMS, "useSMS가", "useSMS는");
        }

        private bool ReadSMSText(XmlTextReader reader, ref string strSMSText)
        {
            if (!ReadElementText(reader, ref strSMSText))
            {
                m_strErrorMessage = string.Format("Line Number {0}, SMSText가 비어있습니다.", reader.LineNumber);
                return false;
            }

            return true;
        }

        private bool ReadSMSExternalTeamIDList(XmlTextReader reader, ref string strSMSExternalTeamIDList)
        {
            if (!ReadElementText(reader, ref strSMSExternalTeamIDList))
            {
                m_strErrorMessage = string.Format("Line Number {0}, SMSExternalTeamIDList가 비어있습니다.", reader.LineNumber);
                return false;
            }

            return true;
        }

        private bool ReadUseFax(XmlTextReader reader, ref bool useFax)
        {
            if (reader.IsEmptyElement)
                return false;

            return ReadBoolean(reader, ref useFax, "useFax가", "useFax는");
        }

        private bool ReadFaxExternalTeamIDList(XmlTextReader reader, ref string strFaxExternalTeamIDList)
        {
            if (!ReadElementText(reader, ref strFaxExternalTeamIDList))
            {
                m_strErrorMessage = string.Format("Line Number {0}, FaxExternalTeamIDList가 비어있습니다.", reader.LineNumber);
                return false;
            }

            return true;
        }

        private PropertyInternal ReadInternalProperty(XmlTextReader reader)
        {
            bool stop = false, readUsePopupMessage = false, readUseSMS = false, readUseBroadcast = false;
            PropertyInternal property = new PropertyInternal();

            bool usePopupMessage = false, useSMS = false, useBroadcast = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:

                        if (string.Compare(reader.Name, "TeamList", true) == 0)
                        {
                            string strTeamList = "";
                            if (ReadTeamList(reader, ref strTeamList))
                            {
                                property.TeamList = strTeamList;
                            }
                        }

                        else if (string.Compare(reader.Name, "usePopupMessage", true) == 0)
                        {
                            if (!ReadUsePopupMessage(reader, ref usePopupMessage))
                                return null;

                            readUsePopupMessage = true;
                            property.UsePopupMessage = usePopupMessage;
                        }
                        else if (string.Compare(reader.Name, "useMobileApp", true) == 0)
                        {
                            if (!ReadUseMobileApp(reader, ref useSMS))
                                return null;

                            readUseSMS = true;
                            property.UseSMS = useSMS;
                        }
                        else if (string.Compare(reader.Name, "useBroadcast", true) == 0)
                        {
                            if (!ReadUseBroadcast(reader, ref useBroadcast))
                                return null;

                            readUseBroadcast = true;
                            property.UseBroadcast = useBroadcast;
                        }
                        else if (string.Compare(reader.Name, "broadcastMessage", true) == 0)
                        {
                            string strBroadcastMessage = "";
                            if (!ReadBroadcastMessage(reader, ref strBroadcastMessage))
                                return null;

                            property.BroadcastMessage = strBroadcastMessage;
                        }
                        // Internal Section의 Commnader 처리 추가 - skkim 2015-07-31
                        else if (string.Compare(reader.Name, "CommanderDisplayText", true) == 0)
                        {
                            string szDisplayText = "";
                            if (!ReadBroadcastMessage(reader, ref szDisplayText))
                                return null;

                            property.CommanderDisplayText = szDisplayText;
                        }
                        else if (string.Compare(reader.Name, "CommanderTeamID", true) == 0)
                        {
                            int nTeamID = -1;
                            if (!ReadInt(reader, ref nTeamID, "Commander의 팀ID가", "Commander의 팀ID는"))
                                return null;

                            property.CommanderTeamID = nTeamID;
                        }
                        else if (string.Compare(reader.Name, "CommanderTeamName", true) == 0)
                        {
                            string szTeamName = "";
                            if (!ReadBroadcastMessage(reader, ref szTeamName))
                                return null;

                            property.CommanderTeamName = szTeamName;
                        }
                        else if (string.Compare(reader.Name, "CommanderTeamType", true) == 0)
                        {
                            int nTeamType = -1;
                            if (!ReadInt(reader, ref nTeamType,"Commander의 팀종류가", "Commander의 팀종류는"))
                                return null;

                            property.CommanderTeamType = nTeamType;
                        }
                        else if (string.Compare(reader.Name, "CommanderIsTeamMember", true) == 0)
                        {
                            bool bIsMember = false;
                            if (!ReadBoolean(reader, ref bIsMember, "Commander의 팀원여부가", "Commander의 팀원여부는"))
                                return null;

                            property.CommanderIsTeamMember = bIsMember;
                        }
                        else if (string.Compare(reader.Name, "CommanderTeamMemberID", true) == 0)
                        {
                            int nMemberID = -1;
                            if (!ReadInt(reader, ref nMemberID, "Commander의 팀ID가", "Commander의 팀ID는"))
                                return null;

                            property.CommanderTeamMemberID = nMemberID;
                        }
                        else
                            PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            if (!readUsePopupMessage)
            {
                m_strErrorMessage = string.Format("Line Number {0}, usePopupMessage가 존재하지 않습니다.", reader.LineNumber);
                return null;
            }

            if (!readUseSMS)
            {
                m_strErrorMessage = string.Format("Line Number {0}, useMobileApp이 존재하지 않습니다.", reader.LineNumber);
                return null;
            }

            if (!readUseBroadcast)
            {
                m_strErrorMessage = string.Format("Line Number {0}, useBroadcast가 존재하지 않습니다.", reader.LineNumber);
                return null;
            }

            return property;
        }

        private bool ReadUsePopupMessage(XmlTextReader reader, ref bool usePopupMessage)
        {
            if (reader.IsEmptyElement)
                return false;
            return ReadBoolean(reader, ref usePopupMessage, "usePopupMessage가", "usePopupMessage는");
        }

        private bool ReadUseMobileApp(XmlTextReader reader, ref bool useSMS)
        {
            if (reader.IsEmptyElement)
                return false;
            return ReadBoolean(reader, ref useSMS, "useMobileApp이", "useMobileApp은");
        }

        private bool ReadUseBroadcast(XmlTextReader reader, ref bool useBroadcast)
        {
            if (reader.IsEmptyElement)
                return false;
            return ReadBoolean(reader, ref useBroadcast, "useBroadcast가", "useBroadcast는");
        }

        private bool ReadBroadcastMessage(XmlTextReader reader, ref string strBroadcastmessage)
        {
            if (reader.IsEmptyElement)
                return true;

            return ReadText(reader, ref strBroadcastmessage);
        }

        private PropertyTransSOP ReadTransSOPProperty(XmlTextReader reader)
        {
            bool stop = false, readLinkedActionStep = false;
            PropertyTransSOP property = null;
            
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "LinkedActionStep", true) == 0)
                        {
                            property = ReadLinkedActionStep(reader);
                            if (property == null)
                                return null;

                            readLinkedActionStep = true;
                        }
                        else
                            PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            if (!readLinkedActionStep || property == null)
            {
                m_strErrorMessage = string.Format("Line Number {0}, LinkedActionStep이 존재하지 않습니다.", reader.LineNumber);
                return null;
            }

            return property;
        }

        private PropertyTransSOP ReadLinkedActionStep(XmlTextReader reader)
        {
            bool stop = false;//, readCategoryName = false, readSubCategoryName = false, readDisasterName = false, readActionStepName = false;
            PropertyTransSOP property = new PropertyTransSOP();

            string strCategoryName = "", strSubCategoryName = "", strDisasterName = "";
            string strActionStepName = "";

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "CategoryName", true) == 0)
                        {
							ReadCategoryName(reader, ref strCategoryName);
                            //readCategoryName = true;
							if (strCategoryName == "null")
								strCategoryName = "";
                            property.LinkedCategoryName = strCategoryName;
                        }
                        else if (string.Compare(reader.Name, "SubCategoryName", true) == 0)
                        {
							ReadSubCategoryName(reader, ref strSubCategoryName);
							if (strSubCategoryName == "null")
								strSubCategoryName = "";

                            //readSubCategoryName = true;
                            property.LinkedSubCategoryName = strSubCategoryName;
                        }
                        else if (string.Compare(reader.Name, "DisasterName", true) == 0)
                        {
							ReadDisasterName(reader, ref strDisasterName);
							if (strDisasterName == "null")
								strDisasterName = "";

                            //readDisasterName = true;
                            property.LinkedDisasterName = strDisasterName;
                        }
                        else if (string.Compare(reader.Name, "ActionStepName", true) == 0)
                        {
							ReadActionStepName(reader, ref strActionStepName, "ActionStepName");
							if (strActionStepName == "null")
								strActionStepName = "";

                            //readActionStepName = true;
                            property.LinkedActionStepName = strActionStepName;
                        }
                        else if (string.Compare(reader.Name, "ParentActionStepName", true) == 0)
                        {
                            string strParentActionStepName = "";

                            if (ReadActionStepName(reader, ref strParentActionStepName, "ParentActionStepName"))
                                property.ParentActionStepNameList.Add(strParentActionStepName);
                        }
                        else
                            PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return property;
        }

        private bool ReadCategoryName(XmlTextReader reader, ref string strCategoryName)
        {
            if (reader.IsEmptyElement)
                return false;

            if (!ReadElementText(reader, ref strCategoryName))
            {
                m_strErrorMessage = string.Format("Line Number {0}, CategoryName이 비어있습니다.", reader.LineNumber);
                return false;
            }

            return true;
        }

        private bool ReadSubCategoryName(XmlTextReader reader, ref string strSubCategoryName)
        {
            if (reader.IsEmptyElement)
                return false;

            if (!ReadElementText(reader, ref strSubCategoryName))
            {
                m_strErrorMessage = string.Format("Line Number {0}, SubCategoryName이 비어있습니다.", reader.LineNumber);
                return false;
            }

            return true;
        }

        private bool ReadDisasterName(XmlTextReader reader, ref string strDisasterName)
        {
            if (reader.IsEmptyElement)
                return false;

            if (!ReadElementText(reader, ref strDisasterName))
            {
                m_strErrorMessage = string.Format("Line Number {0}, DisasterName이 비어있습니다.", reader.LineNumber);
                return false;
            }

            return true;
        }

        private bool ReadActionStepName(XmlTextReader reader, ref string strActionStepName, string strElementName)
        {
            if (reader.IsEmptyElement)
                return false;

            if (!ReadElementText(reader, ref strActionStepName))
            {
                m_strErrorMessage = string.Format("Line Number {0}, {1}이 비어있습니다.", reader.LineNumber, strElementName);
                return false;
            }

            return true;
        }

        private PropertyLink ReadLinkProperty(XmlTextReader reader)
        {
            bool stop = false;
            string strLinkedComponentName = "";
            int nLinkedStepMemberID = -1, nLinkedComponentID = -1;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "LinkedComponentName", true) == 0)
                        {
							ReadLinkedComponentName(reader, ref strLinkedComponentName);
							if (strLinkedComponentName == "null")
								strLinkedComponentName = "";
                            //readLinkedComponentName = true;
                        }
                        else if (string.Compare(reader.Name, "LinkedStepMemberIndex", true) == 0)
                        {
							ReadLinkedStepMemberID(reader, ref nLinkedStepMemberID);
                            //readLinkedStepMemberID = true;
                        }
                        else if (string.Compare(reader.Name, "LinkedComponentID", true) == 0)
                        {
							ReadLinkedComponentID(reader, ref nLinkedComponentID);
                            //readLinkedComponentID = true;
                        }
                        else
                            PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            PropertyLink property = new PropertyLink();
            property.LinkedComponentName = strLinkedComponentName;
            property.LinkedStepMemberID = nLinkedStepMemberID;
            property.LinkedID = nLinkedComponentID;

            return property;
        }

        private bool ReadLinkedComponentName(XmlTextReader reader, ref string strLinkedComponentName)
        {
			ReadElementText(reader, ref strLinkedComponentName);
            return true;
        }

        private bool ReadLinkedStepMemberID(XmlTextReader reader, ref int nLinkedStepMemberID)
        {
            ReadInt(reader, ref nLinkedStepMemberID, "LinkedStepMemberIndex는", "LinkedStepMemberIndex가");
			return true;
        }

        private bool ReadLinkedComponentID(XmlTextReader reader, ref int nLinkedComponentID)
        {
			ReadInt(reader, ref nLinkedComponentID, "LinkedComponentID는", "LinkedComponentID가");
			return true;
        }

        private PropertyEndPoint ReadEndPointProperty(XmlTextReader reader)
        {
            bool stop = false, readIsBegin = false;
            bool isBegin = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "isBegin", true) == 0)
                        {
                            if (!ReadIsBegin(reader, ref isBegin))
                                return null;

                            readIsBegin = true;
                        }
                        else
                            PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            if (!readIsBegin)
            {
                m_strErrorMessage = string.Format("Line Number {0}, isBegin이 존재하지 않습니다.", reader.LineNumber);
                return null;
            }

            PropertyEndPoint property = new PropertyEndPoint();
            property.IsBegin = isBegin;

            return property;
        }

        private bool ReadIsBegin(XmlTextReader reader, ref bool isBegin)
        {
            ReadBoolean(reader, ref isBegin, "isBegin이", "isBegin은");
			return true;
        }

        private PropertyProcess ReadProcessProperty(XmlTextReader reader)
        {
            bool stop = false, readProcessTime = false, readProcessTimeType = false;
            bool readUseProcessTime = false, readUseMissionMessage = false;

            string strTeamList = "";
            int nProcessTime = -1;
            int nProcessTimeType = -1;
            bool useProcessTime = false;
            bool useMissionMessage = false;
            PropertyProcess property = new PropertyProcess();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "TeamList", true) == 0)
                        {
                            if (ReadTeamList(reader, ref strTeamList))
                            {
                                property.TeamList = strTeamList;
                            }
                        }
                        else if (string.Compare(reader.Name, "ProcessTime", true) == 0)
                        {
                            if (!ReadProcessTime(reader, ref nProcessTime))
                                return null;

                            readProcessTime = true;
                            property.ProcessTime = nProcessTime;
                        }
                        else if (string.Compare(reader.Name, "ProcessTimeType", true) == 0)
                        {
                            if (!ReadProcessTimeType(reader, ref nProcessTimeType))
                                return null;

                            readProcessTimeType = true;
                            property.ProcessTimeType = nProcessTimeType;
                        }
                        else if (string.Compare(reader.Name, "useProcessTime", true) == 0)
                        {
                            if (!ReadUseProcessTime(reader, ref useProcessTime))
                                return null;

                            readUseProcessTime = true;
                            property.UseProcessTime = useProcessTime;
                        }
                        else if (string.Compare(reader.Name, "useMissionMessage", true) == 0)
                        {
                            if (!ReadUseMissionMessage(reader, ref useMissionMessage))
                                return null;

                            readUseMissionMessage = true;
                            property.UseMissionMessage = useMissionMessage;
                        }
                        else if (string.Compare(reader.Name, "onlyTeamLeader", true) == 0)
                        {
                            bool isOnlyTeamLeader = false;
                            if (ReadOnlyTeamLeader(reader, ref isOnlyTeamLeader))
                                property.OnlyTeamLeader = isOnlyTeamLeader;
                        }
                        else if (string.Compare(reader.Name, "MissionList", true) == 0)
                        {
                            if (!ReadMissionList(reader, property))
                                return null;
                        }
                        // Internal Section의 Commnader 처리 추가 - skkim 2015-07-31
                        else if (string.Compare(reader.Name, "CommanderDisplayText", true) == 0)
                        {
                            string szDisplayText = "";
                            if (!ReadBroadcastMessage(reader, ref szDisplayText))
                                return null;

                            property.CommanderDisplayText = szDisplayText;
                        }
                        else if (string.Compare(reader.Name, "CommanderTeamID", true) == 0)
                        {
                            int nTeamID = -1;
                            if (!ReadInt(reader, ref nTeamID, "Commander의 팀ID가", "Commander의 팀ID는"))
                                return null;

                            property.CommanderTeamID = nTeamID;
                        }
                        else if (string.Compare(reader.Name, "CommanderTeamName", true) == 0)
                        {
                            string szTeamName = "";
                            if (!ReadBroadcastMessage(reader, ref szTeamName))
                                return null;

                            property.CommanderTeamName = szTeamName;
                        }
                        else if (string.Compare(reader.Name, "CommanderTeamType", true) == 0)
                        {
                            int nTeamType = -1;
                            if (!ReadInt(reader, ref nTeamType, "Commander의 팀종류가", "Commander의 팀종류는"))
                                return null;

                            property.CommanderTeamType = nTeamType;
                        }
                        else if (string.Compare(reader.Name, "CommanderIsTeamMember", true) == 0)
                        {
                            bool bIsMember = false;
                            if (!ReadBoolean(reader, ref bIsMember, "Commander의 팀원여부가", "Commander의 팀원여부는"))
                                return null;

                            property.CommanderIsTeamMember = bIsMember;
                        }
                        else if (string.Compare(reader.Name, "CommanderTeamMemberID", true) == 0)
                        {
                            int nMemberID = -1;
                            if (!ReadInt(reader, ref nMemberID, "Commander의 팀ID가", "Commander의 팀ID는"))
                                return null;

                            property.CommanderTeamMemberID = nMemberID;
                        }
                        else
                            PassElement(reader);

                        break;

                    case XmlNodeType.EndElement:
						if (string.Compare(reader.Name, "Property", true) == 0)
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            if (!readProcessTime)
            {
                m_strErrorMessage = string.Format("Line Number {0}, ProcessTime이 존재하지 않습니다.", reader.LineNumber);
                return null;
            }

            if (!readProcessTimeType)
            {
                m_strErrorMessage = string.Format("Line Number {0}, ProcessTimeType이 존재하지 않습니다.", reader.LineNumber);
                return null;
            }

            if (!readUseProcessTime)
            {
                m_strErrorMessage = string.Format("Line Number {0}, useProcessTime이 존재하지 않습니다.", reader.LineNumber);
                return null;
            }

            if (!readUseMissionMessage)
            {
                m_strErrorMessage = string.Format("Line Number {0}, useMissionMessage가 존재하지 않습니다.", reader.LineNumber);
                return null;
            }

            return property;
        }

        private bool ReadMissionList(XmlTextReader reader, PropertyProcess property)
        {
            if (reader.IsEmptyElement)
                return true;

            bool stop = false;
            //string strMission = "";
            PropertyMissionItem mission = null;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Mission", true) == 0)
                        {
                            mission = ReadMission(reader);
                            if (mission != null)
                                property.Missions.Add(mission);

                        }
                        else
                            PassElement(reader);

                        break;

                    case XmlNodeType.EndElement:
						if (string.Compare(reader.Name, "MissionList", true) == 0)
							stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return true;
        }

        private PropertyMissionItem ReadMission(XmlTextReader reader)
        {
           PropertyMissionItem item = new PropertyMissionItem();
            // Default : 기타
            item.TransmissionType = 3;
            

            while (reader.MoveToNextAttribute())
            {
                if (string.Compare(reader.Name, "transmissionType", true) == 0)
                {
                    try
                    {
                        item.TransmissionType = int.Parse(reader.Value);
                    }
                    catch (Exception)
                    {
                        m_strErrorMessage = string.Format("Line Number {0}, transmissionType은 정수이어야 합니다.", reader.LineNumber);
                        return null;
                    }
                }
                else if (string.Compare(reader.Name, "target", true) == 0)
                {
                    item.Target = reader.Value;
                }

                // MissionCommander 처리 추가 - skkim 2015-11-24
                else if (string.Compare(reader.Name, "CommanderDisplayText", true) == 0)
                {
                    item.CommanderDisplayText = reader.Value;
                }
                else if (string.Compare(reader.Name, "CommanderTeamID", true) == 0)
                {
                    item.CommanderTeamID = int.Parse(reader.Value);
                }
                else if (string.Compare(reader.Name, "CommanderTeamName", true) == 0)
                {
                    item.CommanderTeamName = reader.Value;
                }
                else if (string.Compare(reader.Name, "CommanderTeamType", true) == 0)
                {
                    item.CommanderTeamType = int.Parse(reader.Value);
                }
                else if (string.Compare(reader.Name, "CommanderIsTeamMember", true) == 0)
                {
                    item.CommanderIsTeamMember = bool.Parse(reader.Value);
                }
                else if (string.Compare(reader.Name, "CommanderTeamMemberID", true) == 0)
                {
                    item.CommanderTeamMemberID = int.Parse(reader.Value);
                }
            }

            if (reader.IsEmptyElement)
                return null;

            string strMission = "";
            ReadElementText(reader, ref strMission);

            item.Mission = strMission;
            
            return item;
        }

        /*private bool ReadMission(XmlTextReader reader, ref string strMission)
        {
            if (reader.IsEmptyElement)
                return false;

            if (!ReadElementText(reader, ref strMission))
            {
                strMission = "";
            }

            return true;
        }*/

        private bool ReadTeamList(XmlTextReader reader, ref string strTeamList)
        {
            if (reader.IsEmptyElement)
                return false;

            if (!ReadElementText(reader, ref strTeamList))
            {
                strTeamList = "";
            }

            return true;
        }

        private bool ReadUseProcessTime(XmlTextReader reader, ref bool useProcessTime)
        {
            if (reader.IsEmptyElement)
                return false;
            return ReadBoolean(reader, ref useProcessTime, "useProcessTime이", "useProcessTime은");
        }

        private bool ReadUseMissionMessage(XmlTextReader reader, ref bool useMissionMessage)
        {
            if (reader.IsEmptyElement)
                return false;
            return ReadBoolean(reader, ref useMissionMessage, "useMissionMessage가", "useMissionMessage는");
        }

        private bool ReadOnlyTeamLeader(XmlTextReader reader, ref bool onlyTeamLeader)
        {
            if (reader.IsEmptyElement)
                return false;
            return ReadBoolean(reader, ref onlyTeamLeader, "onlyTeamLeader가", "onlyTeamLeader는");
        }

        private bool ReadBoolean(XmlTextReader reader, ref bool bData, string strMessage1, string strMessage2)
        {
            string strText = "";

            if (!ReadElementText(reader, ref strText))
            {
                m_strErrorMessage = string.Format("Line Number {0}, {1} 비어있습니다.", reader.LineNumber, strMessage1);
                return false;
            }

            try
            {
                if (string.Compare(strText, "true", true) == 0)
                    bData = true;
                else if (string.Compare(strText, "false", true) == 0)
                    bData = false;
                else
                    bData = int.Parse(strText) == 0 ? false : true;
            }
            catch (Exception)
            {
                m_strErrorMessage = string.Format("Line Number {0}, {1} true, false, 0 또는 1로 표현되어야만 합니다.", reader.LineNumber, strMessage2);
                return false;
            }

            return true;
        }

        private bool ReadText(XmlTextReader reader, ref string strText)
        {
            if (reader.IsEmptyElement)
                return false;

            if (!ReadElementText(reader, ref strText))
                strText = "";

            return true;
        }

        private bool ReadX(XmlTextReader reader, ref float x)
        {
            if (reader.IsEmptyElement)
                return false;
            return ReadFloat(reader, ref x, "Component의 X", "Component의 X는");
        }

        private bool ReadY(XmlTextReader reader, ref float y)
        {
            if (reader.IsEmptyElement)
                return false;
            return ReadFloat(reader, ref y, "Component의 Y", "Component의 Y는");
        }

        private bool ReadWidth(XmlTextReader reader, ref float fWidth)
        {
            if (reader.IsEmptyElement)
                return false;
            return ReadFloat(reader, ref fWidth, "Component의 Width", "Component의 Width는");
        }

        private bool ReadHeight(XmlTextReader reader, ref float fHeight)
        {
            if (reader.IsEmptyElement)
                return false;
            return ReadFloat(reader, ref fHeight, "Component의 Height", "Component의 Height는");
        }

        private bool ReadComponentID(XmlTextReader reader, ref string strComponentID)
        {
            if (reader.IsEmptyElement)
                return false;

            if (!ReadElementText(reader, ref strComponentID))
            {
                m_strErrorMessage = string.Format("Line Number {0}, ComponentID 값이 비어있습니다.", reader.LineNumber);
                return false;
            }

            return true;
        }

        private bool ReadFontName(XmlTextReader reader, ref string strFontName)
        {
            if (reader.IsEmptyElement)
                return false;

            if (!ReadElementText(reader, ref strFontName))
            {
                m_strErrorMessage = string.Format("Line Number {0}, FontName 값이 비어있습니다.", reader.LineNumber);
                return false;
            }

            return true;
        }

        private bool ReadFontSize(XmlTextReader reader, ref float fHeight)
        {
            if (reader.IsEmptyElement)
                return false;
            return ReadFloat(reader, ref fHeight, "Component의 FontSize", "Component의 FontSize는");
        }

        private bool ReadLineSpace(XmlTextReader reader, ref float fHeight)
        {
            if (reader.IsEmptyElement)
                return false;
            return ReadFloat(reader, ref fHeight, "Component의 LineSpace", "Component의 LineSpace는");
        }
        

        private bool ReadFloat(XmlTextReader reader, ref float fData, string strMessage1, string strMessage2)
        {
            string strText = "";

            if (!ReadElementText(reader, ref strText))
            {
                m_strErrorMessage = string.Format("Line Number {0}, {1} 값이 비어있습니다.", reader.LineNumber, strMessage1);
                return false;
            }

            try
            {
                fData = float.Parse(strText);
            }
            catch (Exception)
            {
                m_strErrorMessage = string.Format("Line Number {0}, {1} 실수형이어야만 합니다.", reader.LineNumber, strMessage2);
                return false;
            }

            return true;
        }

        private bool ReadProcessTime(XmlTextReader reader, ref int nProcessTime)
        {
            if (reader.IsEmptyElement)
                return false;

            string strText = "";

            if (!ReadElementText(reader, ref strText))
            {
                m_strErrorMessage = string.Format("Line Number {0}, ProcessTime에 내용이 없습니다.", reader.LineNumber);
                return false;
            }

            try
            {
                nProcessTime = int.Parse(strText);
            }
            catch (Exception)
            {
                m_strErrorMessage = string.Format("Line Number {0}, ProcessTime은 정수 형태이어야만 합니다.", reader.LineNumber);
                return false;
            }

            return true;
        }

        private bool ReadIterationType(XmlTextReader reader, ref int nIterationType)
        {
            if (reader.IsEmptyElement)
                return false;

            string strText = "";

            if (!ReadElementText(reader, ref strText))
            {
                m_strErrorMessage = string.Format("Line Number {0}, IterationType에 내용이 없습니다.", reader.LineNumber);
                return false;
            }

            try
            {
                nIterationType = int.Parse(strText);
            }
            catch (Exception)
            {
                m_strErrorMessage = string.Format("Line Number {0}, IterationType은 정수 형태이어야만 합니다.", reader.LineNumber);
                return false;
            }

            return true;
        }

        private bool ReadIteration(XmlTextReader reader, ref int nIteration)
        {
            if (reader.IsEmptyElement)
                return false;

            string strText = "";

            if (!ReadElementText(reader, ref strText))
            {
                m_strErrorMessage = string.Format("Line Number {0}, Iteration에 내용이 없습니다.", reader.LineNumber);
                return false;
            }

            try
            {
                nIteration = int.Parse(strText);
            }
            catch (Exception)
            {
                m_strErrorMessage = string.Format("Line Number {0}, Iteration은 정수 형태이어야만 합니다.", reader.LineNumber);
                return false;
            }

            return true;
        }

        private bool ReadWeekDayOption(XmlTextReader reader, ref int nWeekDayOption)
        {
            if (reader.IsEmptyElement)
                return false;

            string strText = "";

            if (!ReadElementText(reader, ref strText))
            {
                m_strErrorMessage = string.Format("Line Number {0}, WeekDayOption에 내용이 없습니다.", reader.LineNumber);
                return false;
            }

            try
            {
                nWeekDayOption = int.Parse(strText);
            }
            catch (Exception)
            {
                m_strErrorMessage = string.Format("Line Number {0}, WeekDayOption은 정수 형태이어야만 합니다.", reader.LineNumber);
                return false;
            }

            return true;
        }

        private bool ReadPeriodType(XmlTextReader reader, ref int nPeriodType)
        {
            if (reader.IsEmptyElement)
                return false;

            string strText = "";

            if (!ReadElementText(reader, ref strText))
            {
                m_strErrorMessage = string.Format("Line Number {0}, PeriodType에 내용이 없습니다.", reader.LineNumber);
                return false;
            }

            try
            {
                nPeriodType = int.Parse(strText);
            }
            catch (Exception)
            {
                m_strErrorMessage = string.Format("Line Number {0}, PeriodType은 정수 형태이어야만 합니다.", reader.LineNumber);
                return false;
            }

            return true;
        }

        private bool ReadStepName(XmlTextReader reader, ref string strStepName)
        {
            if (reader.IsEmptyElement)
                return false;

            if (!ReadElementText(reader, ref strStepName))
            {
                m_strErrorMessage = string.Format("Line Number {0}, StepName에 내용이 없습니다.", reader.LineNumber);
                return false;
            }

            return true;
        }

        private bool ReadElementText(XmlTextReader reader, ref string strText)
        {
            bool stop = false, readText = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Text:
                        strText = reader.Value;
                        readText = true;
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return readText;
        }

		public bool Save(FormMain frm, System.IO.Stream stream, string strVersionName, string strDescription = null)
		{
			XmlTextWriter writer = InitWriter(stream);
			return Save(writer, frm, strVersionName, strDescription);
		}		

        public bool Save(FormMain frm, string strPath, string strVersionName, string strDescription = null)
        {
            XmlTextWriter writer = InitWriter(strPath);            
            return Save(writer, frm, strVersionName, strDescription);
        }

		private bool Save(XmlTextWriter writer, FormMain frm, string strVersionName, string strDescription = null)
		{
			writer.WriteStartElement("SOP");        // SOP 시작

			writer.WriteStartAttribute("xmlns:xsi");
			writer.WriteString("http://www.w3.org/2001/XMLSchema-instance");
			writer.WriteEndAttribute();

			writer.WriteStartAttribute("xsi:noNamespaceSchemaLocation");
			writer.WriteString("http://unes.iptime.org:9808/SOP/XML/SOP.xsd");
			writer.WriteEndAttribute();

			if (!MakeHeader(writer, strVersionName, strDescription))
				return false;

			if (!MakeBody(writer, frm))
				return false;

			writer.WriteFullEndElement();       // SOP 끝

			writer.WriteEndDocument();
			writer.Close();

			return true;
		}

		private XmlTextWriter InitWriter(System.IO.Stream stream)
		{
			XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);

			writer.Formatting = Formatting.Indented;
			writer.WriteStartDocument();

			return writer;
		}

        private XmlTextWriter InitWriter(string strPath)
        {
            XmlTextWriter writer = new XmlTextWriter(strPath, Encoding.UTF8);

            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();

            return writer;
        }

        private bool MakeHeader(XmlTextWriter writer, string strVersionName, string strDescription = null)
        {
            writer.WriteStartElement("Header"); // Header 시작          

            string strDisaster = SopDocManager.Instance.DisasterName;
            string strSubCategory = SopDocManager.Instance.SubCategoryName;
			string strCategory = SopDocManager.Instance.CategoryName;

			bool bRegular = SopDocManager.Instance.RegularMode;
			bool bWeek = SopDocManager.Instance.WeekMode;

            writer.WriteStartElement("XMLVersion");
            writer.WriteString(XML_VERSION);
            writer.WriteFullEndElement();

            writer.WriteStartElement("Category");
            writer.WriteString(strCategory);
            writer.WriteFullEndElement();

            writer.WriteStartElement("SubCategory");
            writer.WriteString(strSubCategory);
            writer.WriteFullEndElement();

            writer.WriteStartElement("Disaster");
            writer.WriteString(strDisaster);
            writer.WriteFullEndElement();

            // 등록/미등록 모드
            writer.WriteStartElement("Regular");
			writer.WriteString(bRegular ? "1" : "0");
            writer.WriteFullEndElement();

            // 주간/야간 모드
            writer.WriteStartElement("Normal");
			writer.WriteString(bWeek ? "1" : "0");
            writer.WriteFullEndElement();

            writer.WriteStartElement("SOPVersion");
            
            if (strDescription != null)
            {
                writer.WriteStartAttribute("description");
                writer.WriteString(strDescription);
                writer.WriteEndAttribute();
            }

            writer.WriteString(strVersionName);
            writer.WriteFullEndElement();

            writer.WriteFullEndElement();   // Header 끝
            return true;
        }

        private bool MakeBody(XmlTextWriter writer, FormMain frm)
        {
            writer.WriteStartElement("Body");   // Body 시작

            writer.WriteStartElement("ActionStepList"); // ActionStepList 시작

			FormPageSOP pageLevel = frm.GetPageLevel();

            Dictionary<TabPage, long> dicActionSteps = GetActionStepIDs(pageLevel);
            if (dicActionSteps == null)
                return false;

            foreach (TabPage page in pageLevel.TabControls.TabPages)
            {
                if (!MakeActionStep(writer, page, pageLevel, dicActionSteps))
                    return false;
            }

            writer.WriteFullEndElement();       // ActionStepList 끝
            writer.WriteFullEndElement();       // Body 끝

            return true;
        }

        // Return값 : TabPage별 ActionStep ID(하위 4바이트) & Parent Step ID(상위 4바이트)
		private Dictionary<TabPage, long> GetActionStepIDs(FormPageSOP pageLevel)
        {
            Dictionary<TabPage, long> dicActionSteps = new Dictionary<TabPage, long>();
            int nActionStepID = 1;

            // ActioinStep ID 설정
            foreach (TabPage page in pageLevel.TabControls.TabPages)
            {
                dicActionSteps[page] = nActionStepID++;
            }

            // Parent Step ID 설정
            foreach (TabPage page in pageLevel.TabControls.TabPages)
            {
                if (page.Tag != null)
                {
                    if (!dicActionSteps.ContainsKey((TabPage)page.Tag))
                        return null;

                    long nParentID = dicActionSteps[(TabPage)page.Tag];
					dicActionSteps[page] = (nParentID << 32) | dicActionSteps[page];
                }
            }

            return dicActionSteps;
        }

        private bool MakeActionStep(XmlTextWriter writer, TabPage tabPage, FormPageSOP pageLevel, Dictionary<TabPage, long> dicActionSteps)
        {
            writer.WriteStartElement("ActionStep");     // ActionStep 시작

            long nData = dicActionSteps[tabPage];
            long nParentID = nData >> 32;
            long nActionStepID = nData & 0xffffffff;

            writer.WriteStartAttribute("id");
            writer.WriteString(nActionStepID.ToString());
            writer.WriteEndAttribute();

			TabPage page = pageLevel.GetCurrentTabPage();
			if (page != null && tabPage == page)
			{
				writer.WriteStartAttribute("selected");
				writer.WriteString("1");
				writer.WriteEndAttribute();
			}
			else
			{
				writer.WriteStartAttribute("selected");
				writer.WriteString("0");
				writer.WriteEndAttribute();
			}

            writer.WriteStartElement("StepName");
            writer.WriteString(tabPage.Text);
            writer.WriteFullEndElement();

			ActionStepTabPage apage = (ActionStepTabPage)tabPage;
            //Data_ActionStep opt = pageLevel.GetActionStepOption(tabPage);
			Data_ActionStep opt = apage.Data;
            if (opt == null)
				return false;

            writer.WriteStartElement("PeriodType");
            writer.WriteString(opt.PeriodType.ToString());
            writer.WriteFullEndElement();

            if (opt.PeriodType != 0)
            {
                string strBeginTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", opt.BeginTime.ToShortDateString(), opt.BeginTime.Hour, opt.BeginTime.Minute, opt.BeginTime.Second);
                string strEndTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", opt.EndTime.ToShortDateString(), opt.EndTime.Hour, opt.EndTime.Minute, opt.EndTime.Second);

                writer.WriteStartElement("BeginTime");
                writer.WriteString(strBeginTime);
                writer.WriteFullEndElement();

                writer.WriteStartElement("EndTime");
                writer.WriteString(strBeginTime);
                writer.WriteFullEndElement();
            }

            writer.WriteStartElement("WeekDayOption");
            writer.WriteString(opt.WeekdayOption.ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("Iteration");
            writer.WriteString(opt.Iteration.ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("IterationType");
            writer.WriteString(opt.IterationType.ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("ProcessTime");
            writer.WriteString(opt.ProcessTime.ToString());
            writer.WriteFullEndElement();

            if (opt.ProcessTimeType >= 0 && opt.ProcessTimeType <= 4)
            {
                writer.WriteStartElement("ProcessTimeType");
                writer.WriteString(opt.ProcessTimeType.ToString());
                writer.WriteFullEndElement();
            }

            if (nParentID > 0)
            {
                writer.WriteStartElement("ParentStepID");
                writer.WriteString(nParentID.ToString());
                writer.WriteFullEndElement();
            }

            bool isSuccess = MakeStepMemberList(writer, tabPage);

            writer.WriteFullEndElement();       // ActionStep 끝
            return isSuccess;
        }

        private bool MakeStepMemberList(XmlTextWriter writer, TabPage tabPage)
        {
            writer.WriteStartElement("StepMemberList");     // StepMemberList 시작

            Type type = typeof(Sections.PanelSectionEx);           

            // Arrow 및 Link 계산을 위한 Data
            // Section 별 StepMember ID(상위 4바이트) & Component ID(하위 4바이트)
            Dictionary<Sections.Section, long> dicSectionInfo = new Dictionary<Sections.Section, long>();

			int nStempMemberID = 1;
            foreach (Control ctrl in tabPage.Controls)
            {
                if (ctrl.GetType() == type)
                {

					Sections.PanelSectionEx panel = (Sections.PanelSectionEx)ctrl; 
					if (panel.Collapse == false)
					{
						panel.CollapseAllGroup();
					}
					GetSectionInfo(panel, dicSectionInfo, nStempMemberID++);
                }
            }
			nStempMemberID = 1;
            foreach (Control ctrl in tabPage.Controls)
            {
                if (ctrl.GetType() == type)
                {
                    Sections.PanelSectionEx panel = (Sections.PanelSectionEx)ctrl;

					if (!MakeStepMember(writer, panel, dicSectionInfo, nStempMemberID++))
                        return false;
                }
            }

            writer.WriteFullEndElement();   // StepMemberList 끝
            return true;
        }

        private void GetSectionInfo(Sections.PanelSectionEx panel, Dictionary<Sections.Section, long> dicSectionInfo, long nStepMemberID)
        {
            long nComponentID = 1;

            foreach (Sections.Section section in panel.Sections)
            {
                long nSectionInfo = (nStepMemberID << 32) | nComponentID++;
                dicSectionInfo[section] = nSectionInfo;
            }
        }

        // dicSectionInfo : Section 별 StepMember ID(상위 4바이트) & Component ID(하위 4바이트)
        private bool MakeStepMember(XmlTextWriter writer, Sections.PanelSectionEx panel, Dictionary<Sections.Section, long> dicSectionInfo, int nStepMemberID)
        {
            writer.WriteStartElement("StepMember");     // StepMember 시작

            writer.WriteStartAttribute("id");
            writer.WriteString(nStepMemberID.ToString());
            writer.WriteEndAttribute();
			
            writer.WriteStartAttribute("type");
            writer.WriteString(((int)panel.TeamType).ToString());
            writer.WriteEndAttribute();
			
            writer.WriteStartAttribute("name");
			writer.WriteString(panel.TeamName);
            writer.WriteEndAttribute();

			writer.WriteStartAttribute("teamid");
			writer.WriteString(panel.TeamID.ToString());
			writer.WriteEndAttribute();

			writer.WriteStartAttribute("modify");
			writer.WriteString(panel.IsModified.ToString());
			writer.WriteEndAttribute();

            if (MakeComponentList(writer, panel, dicSectionInfo, nStepMemberID))
            {
                if (!MakeArrowList(writer, dicSectionInfo, nStepMemberID))
                    return false;

                writer.WriteFullEndElement();       // StepMember 끝
                return true;
            }

            return false;
        }

        // dicSectionInfo : Section 별 StepMember ID(상위 4바이트) & Component ID(하위 4바이트)
        private bool MakeComponentList(XmlTextWriter writer, Sections.PanelSectionEx panel, Dictionary<Sections.Section, long> dicSectionInfo, int nStepMemberID)
        {
            writer.WriteStartElement("ComponentList");      // ComponentList 시작

			PointF ptOrgin;
			PointF ptTrans, ptScale;
			panel.GetViewport(out ptOrgin, out ptTrans, out ptScale);

			writer.WriteStartElement("Viewport");
			writer.WriteAttributeString("OriginX", ptOrgin.X.ToString());
			writer.WriteAttributeString("OriginY", ptOrgin.Y.ToString());
			writer.WriteAttributeString("CurrentX", ptTrans.X.ToString());
			writer.WriteAttributeString("CurrentY", ptTrans.Y.ToString());
			writer.WriteAttributeString("ScaleX", ptScale.X.ToString());
			writer.WriteAttributeString("ScaleY", ptScale.Y.ToString());
			writer.WriteFullEndElement();

            foreach (Sections.Section section in panel.Sections)
            {
                if (!MakeComponent(writer, section, dicSectionInfo))
                    return false;
            }

            writer.WriteFullEndElement();       // Component List 끝
            return true;
        }

        // dicSectionInfo : Section 별 StepMember ID(상위 4바이트) & Component ID(하위 4바이트)
        private bool MakeComponent(XmlTextWriter writer, Sections.Section section, Dictionary<Sections.Section, long> dicSectionInfo)
        {
            if (!dicSectionInfo.ContainsKey(section))
                return false;

            Sections.SectionData data = section.Data;
            if (data == null)
                return false;

            long nSectionInfo = dicSectionInfo[section];
            long nComponentID = nSectionInfo & 0xffffffff;						

            writer.WriteStartElement("Component");      // Component 시작

            writer.WriteStartAttribute("id");
            writer.WriteString(nComponentID.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartElement("X");
            writer.WriteString(section.Position.X.ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("Y");
            writer.WriteString(section.Position.Y.ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("Width");
            writer.WriteString(section.RectSize.Width.ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("Height");
            writer.WriteString(section.RectSize.Height.ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("Text");

            if (section.GetComponentType() == Sections.Section.ComponentType.PROCESS)
                writer.WriteString(((Sections.SectionProcess)section).TextUP);
            else
                writer.WriteString(section.Title);
            writer.WriteFullEndElement();


            writer.WriteStartElement("VAlign");
            int valign = (int)data.TextVerticalAlign;
            writer.WriteString(valign.ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("HAlign");
            int halign = (int)data.TextHorizontalAlign;
            writer.WriteString(halign.ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("ComponentID");
            writer.WriteString(section.Data.ComponentID);
            writer.WriteFullEndElement();

            writer.WriteStartElement("FontName");
            writer.WriteString(section.TextFont.Name);
            writer.WriteFullEndElement();

            writer.WriteStartElement("FontSize");
            writer.WriteString(section.TextFont.Size.ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("FontStyle");
            writer.WriteString(((int)section.TextFont.Style).ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("LineSpace");
            writer.WriteString(section.Data.LineSpace.ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("FontColor");
            writer.WriteString((section.TextColor.ToArgb()).ToString());
            writer.WriteFullEndElement();


            Sections.Section.ComponentType type = section.GetComponentType();

            writer.WriteStartElement("Property");   // Property 시작

            writer.WriteStartAttribute("type");
            writer.WriteString(((int)type).ToString());
            writer.WriteEndAttribute();

            bool isSuccess = true;

            switch (type)
            {
                case Sections.Section.ComponentType.PROCESS:
                    isSuccess = MakeProcessProperty(writer, (Sections.SectionProcess)section);
                    break;

                case Sections.Section.ComponentType.ENDPOINT:
                    isSuccess = MakeEndPointProperty(writer, (Sections.SectionEndPoint)section);
                    break;

                case Sections.Section.ComponentType.LINK:
                    isSuccess = MakeLinkProperty(writer, (Sections.SectionLink)section, dicSectionInfo);
                    break;

                case Sections.Section.ComponentType.TRANSSOP:
                    isSuccess = MakeTransSOPProperty(writer, (Sections.SectionTransSOP)section);
                    break;

                case Sections.Section.ComponentType.INTERNAL:
                    isSuccess = MakeInternalProperty(writer, (Sections.SectionInternal)section);
                    break;

                case Sections.Section.ComponentType.EXTERNAL:
                    isSuccess = MakeExternalProperty(writer, (Sections.SectionExternal)section);
                    break;

                case Sections.Section.ComponentType.TRANSMISSION:
                    isSuccess = MakeTransmissionProperty(writer, (Sections.SectionTransmission)section);
                    break;

				case Sections.Section.ComponentType.GROUP:
					isSuccess = MakeGroupProperty(writer, (Sections.SectionGroup)section, dicSectionInfo);
					break;

            }

            writer.WriteFullEndElement();   // Property 끝
            writer.WriteFullEndElement();   // Component 끝

            return isSuccess;
        }

		private bool MakeGroupProperty(XmlTextWriter writer, Sections.SectionGroup section, Dictionary<Sections.Section, long> dicSectionInfo)
		{			
			Sections.SectionDataGroup data = (Sections.SectionDataGroup)section.Data;
			StringBuilder sb = new StringBuilder();		
			foreach (Sections.Section comp in data.GroupItems)
			{
				if (sb.Length != 0)
					sb.Append(",");
				long nSectionInfo = dicSectionInfo[comp];
				long nComponentID = nSectionInfo & 0xffffffff;
				sb.Append(nComponentID.ToString());
			}

			writer.WriteStartElement("GroupItems");
			writer.WriteString(sb.ToString());
			writer.WriteFullEndElement();

			//writer.WriteFullEndElement();

			return true;
		}

        private bool MakeProcessProperty(XmlTextWriter writer, Sections.SectionProcess section)
        {
            string strTeamList = IOManager.GetProcessTeamList(section);

            writer.WriteStartElement("TeamList");
            writer.WriteString(strTeamList);
            writer.WriteFullEndElement();

            Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;

            writer.WriteStartElement("ProcessTime");
            writer.WriteString(data.ProcessingTime.Time.ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("ProcessTimeType");
            writer.WriteString(((int)data.ProcessingTime.ProcessingType).ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("useProcessTime");
            writer.WriteString(data.UseProcessingTime ? "1" : "0");
            writer.WriteFullEndElement();

            writer.WriteStartElement("useMissionMessage");
            writer.WriteString(data.MissionTransfer ? "1" : "0");
            writer.WriteFullEndElement();

            if (data.TransferTeamLeaderOnly)
            {
                writer.WriteStartElement("onlyTeamLeader");
                writer.WriteString("1");
                writer.WriteFullEndElement();
            }

            writer.WriteStartElement("MissionList");
            
            if (!MakeProcessMissionList(writer, data))
                return false;

            writer.WriteFullEndElement();



            Sections.SectionCommander commander = data.Commander;
            if (commander != null)
            {
                writer.WriteStartElement("CommanderDisplayText");
                writer.WriteString(commander.DisplayText);
                writer.WriteFullEndElement();

                writer.WriteStartElement("CommanderTeamID");
                writer.WriteString(commander.Team == null ? "-1" : commander.Team.TeamID.ToString());
                writer.WriteFullEndElement();

                writer.WriteStartElement("CommanderTeamName");
                writer.WriteString(commander.Team == null ? "" : commander.Team.TeamName);
                writer.WriteFullEndElement();

                writer.WriteStartElement("CommanderTeamType");
                writer.WriteString(commander.Team == null ? "-1" : ((int)commander.Team.TeamType).ToString());
                writer.WriteFullEndElement();

                writer.WriteStartElement("CommanderIsTeamMember");
                writer.WriteString(commander.IsTeamMember.ToString());
                writer.WriteFullEndElement();

                writer.WriteStartElement("CommanderTeamMemberID");
                writer.WriteString(commander.TeamMemberID.ToString());
                writer.WriteFullEndElement();
            }

            return true;
        }

        private bool MakeProcessMissionList(XmlTextWriter writer, Sections.SectionDataProcess data)
        {
            foreach (Sections.MissionItem mission in data.MissionItems)
            {
                writer.WriteStartElement("Mission");

                writer.WriteStartAttribute("transmissionType");
                writer.WriteString(mission.TransmissionType.ToString());
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("target");
                writer.WriteString(mission.Target);
                writer.WriteEndAttribute();

                Sections.SectionCommander commander = mission.Commander;
                if (commander != null)
                {
                    writer.WriteStartAttribute("CommanderDisplayText");
                    writer.WriteString(commander.DisplayText);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("CommanderTeamID");
                    writer.WriteString(commander.Team == null ? "-1" : commander.Team.TeamID.ToString());
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("CommanderTeamName");
                    writer.WriteString(commander.Team == null ? "" : commander.Team.TeamName);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("CommanderTeamType");
                    writer.WriteString(commander.Team == null ? "-1" : ((int)commander.Team.TeamType).ToString());
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("CommanderIsTeamMember");
                    writer.WriteString(commander.IsTeamMember.ToString());
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("CommanderTeamMemberID");
                    writer.WriteString(commander.TeamMemberID.ToString());
                    writer.WriteEndAttribute();
                }

                writer.WriteString(mission.Mission);
                writer.WriteFullEndElement();
            }

            return true;
        }

        private bool MakeEndPointProperty(XmlTextWriter writer, Sections.SectionEndPoint section)
        {
            Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)section.Data;

            writer.WriteStartElement("isBegin");
            writer.WriteString(data.IsBegin ? "1" : "0");
            writer.WriteFullEndElement();

            return true;
        }

        // dicSectionInfo : Section 별 StepMember ID(상위 4바이트) & Component ID(하위 4바이트)
        private bool MakeLinkProperty(XmlTextWriter writer, Sections.SectionLink section, Dictionary<Sections.Section, long> dicSectionInfo)
        {
            Sections.SectionDataLink data = (Sections.SectionDataLink)section.Data;
			if (data.LinkedSection == null)
			{
				writer.WriteStartElement("LinkedComponentName");
				writer.WriteString("null");
				writer.WriteFullEndElement();

				writer.WriteStartElement("LinkedStepMemberIndex");
				writer.WriteString("-1");
				writer.WriteFullEndElement();

				writer.WriteStartElement("LinkedComponentID");
				writer.WriteString("-1");
				writer.WriteFullEndElement();
				return true;
			}

            if (!dicSectionInfo.ContainsKey(data.LinkedSection))
                return false;

            long nSectionInfo = dicSectionInfo[data.LinkedSection];
            long nLinkedStepMemberID = nSectionInfo >> 32;
            long nLinkedComponentID = nSectionInfo & 0xffffffff;

            writer.WriteStartElement("LinkedComponentName");
            writer.WriteString(data.LinkedSection.Data.ComponentID);
            writer.WriteFullEndElement();

            writer.WriteStartElement("LinkedStepMemberIndex");
            writer.WriteString(nLinkedStepMemberID.ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("LinkedComponentID");
            writer.WriteString(nLinkedComponentID.ToString());
            writer.WriteFullEndElement();

            return true;
        }

        private bool MakeTransSOPProperty(XmlTextWriter writer, Sections.SectionTransSOP section)
        {
            Sections.SectionDataTransSOP data = (Sections.SectionDataTransSOP)section.Data;

			string strCategoryName = "null";
			string strSubCategoryName = "null";
			string strDisasterName = "null";
			string strStepName = "";
			int nParentStepID = -1;
			ArrayList arrParentStepName = new ArrayList();

			if (data.LinkedActionStepID > 0)
			{
				WebDBManager dbMgr = FormMain.Instance.DBManager;

				string strSQL = "select dc.CategoryName, sc.SubCategoryName, Disaster.DisasterName, step.StepName, step.ParentStepID ";
				strSQL += "from ActionStep as step, Disaster, SubDisasterCategory as sc, DisasterCategory as dc ";
				strSQL += "where step.DisasterID = Disaster.ID and Disaster.SubDisasterID = sc.ID and sc.DisasterID = dc.ID and step.ID = " + data.LinkedActionStepID.ToString();

				ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
				if (arrResult == null || arrResult.Count != 5)
					return false;

				strCategoryName = WebDBManager.GetStringField(arrResult[0], "");
				strSubCategoryName = WebDBManager.GetStringField(arrResult[1], "");
				strDisasterName = WebDBManager.GetStringField(arrResult[2], "");
				strStepName = WebDBManager.GetStringField(arrResult[3], "");
				nParentStepID = WebDBManager.GetIntField(arrResult[4].ToString(), -1);
				
				while (nParentStepID > 0)
				{
					strSQL = "select ParentStepID, StepName from ActionStep where id = " + nParentStepID.ToString();
					arrResult = dbMgr.GetResultData(strSQL, 0);

					if (arrResult == null || arrResult.Count != 2)
						return false;

					nParentStepID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
					string strParentStepName = WebDBManager.GetStringField(arrResult[1], "");

					arrParentStepName.Add(strParentStepName);
				}
			}           

            writer.WriteStartElement("LinkedActionStep");       // LinkedActionStep 시작

            writer.WriteStartElement("CategoryName");
            writer.WriteString(strCategoryName);
            writer.WriteFullEndElement();

            writer.WriteStartElement("SubCategoryName");
            writer.WriteString(strSubCategoryName);
            writer.WriteFullEndElement();

            writer.WriteStartElement("DisasterName");
            writer.WriteString(strDisasterName);
            writer.WriteFullEndElement();

            writer.WriteStartElement("ActionStepName");
            writer.WriteString(strStepName);
            writer.WriteFullEndElement();

            foreach (string strParentStepName in arrParentStepName)
            {
                writer.WriteStartElement("ParentActionStepName");
                writer.WriteString(strParentStepName);
                writer.WriteFullEndElement();
            }

            writer.WriteFullEndElement();       // LInkedActionStep 끝
            return true;
        }

        private bool MakeInternalProperty(XmlTextWriter writer, Sections.SectionInternal section)
        {

            string strTeamList = IOManager.GetInternalTeamList(section);

            writer.WriteStartElement("TeamList");
            writer.WriteString(strTeamList);
            writer.WriteFullEndElement();

            Sections.SectionDataInternal data = (Sections.SectionDataInternal)section.Data;

            writer.WriteStartElement("usePopupMessage");
            writer.WriteString(data.UsePopupMessage ? "1" : "0");
            writer.WriteFullEndElement();

            writer.WriteStartElement("useMobileApp");
            writer.WriteString(data.UseMobileApp ? "1" : "0");
            writer.WriteFullEndElement();

            writer.WriteStartElement("useBroadcast");
            writer.WriteString(data.UseBroadcast ? "1" : "0");
            writer.WriteFullEndElement();

            writer.WriteStartElement("broadcastMessage");
            writer.WriteString(data.BroadcastMessage);
            writer.WriteFullEndElement();

            Sections.SectionCommander commander = data.Commander;
            if (commander!= null)
            {                           
                writer.WriteStartElement("CommanderDisplayText");
                writer.WriteString(commander.DisplayText);
                writer.WriteFullEndElement();
                
                writer.WriteStartElement("CommanderTeamID");
                writer.WriteString(commander.Team == null? "-1" : commander.Team.TeamID.ToString());
                writer.WriteFullEndElement();

                writer.WriteStartElement("CommanderTeamName");
                writer.WriteString(commander.Team == null? "" : commander.Team.TeamName);
                writer.WriteFullEndElement();

                writer.WriteStartElement("CommanderTeamType");
                writer.WriteString(commander.Team == null ? "-1" : ((int)commander.Team.TeamType).ToString());
                writer.WriteFullEndElement();

                writer.WriteStartElement("CommanderIsTeamMember");
                writer.WriteString(commander.IsTeamMember.ToString());
                writer.WriteFullEndElement();

                writer.WriteStartElement("CommanderTeamMemberID");
                writer.WriteString(commander.TeamMemberID.ToString());
                writer.WriteFullEndElement();
            }
            return true;
        }

        private string GetExternalTeamString(ArrayList arrReceivers)
        {
            string strReceiver = "";

            foreach (Sections.ExternalTeamData teamData in arrReceivers)
            {
                if (strReceiver.Length == 0)
                    strReceiver = string.Format("{0}", teamData.TeamID.ToString());
                else
                    strReceiver += string.Format(", {0}", teamData.TeamID.ToString());
            }
			if (strReceiver == "")
				strReceiver = "null";
            return strReceiver;
        }

        private bool MakeExternalProperty(XmlTextWriter writer, Sections.SectionExternal section)
        {
            Sections.SectionDataExternal data = (Sections.SectionDataExternal)section.Data;

            writer.WriteStartElement("useSMS");
            writer.WriteString(data.UseSMS ? "1" : "0");
            writer.WriteFullEndElement();

            if (data.UseSMS)
            {
                writer.WriteStartElement("SMSText");

				if (data.SMSMessage == "")
					writer.WriteString("null");
				else
					writer.WriteString(data.SMSMessage);
                writer.WriteFullEndElement();

                writer.WriteStartElement("SMSExternalTeamIDList");
                writer.WriteString(GetExternalTeamString(data.SMSReceivers));
                writer.WriteFullEndElement();
            }

            writer.WriteStartElement("useFax");
            writer.WriteString(data.UseFax ? "1" : "0");
            writer.WriteFullEndElement();

            if (data.UseFax)
            {
                writer.WriteStartElement("FaxExternalTeamIDList");
                writer.WriteString(GetExternalTeamString(data.FaxReceivers));
                writer.WriteFullEndElement();
            }

            return true;
        }

        private bool MakeTransmissionProperty(XmlTextWriter writer, Sections.SectionTransmission section)
        {
            Sections.SectionDataTransmission data = (Sections.SectionDataTransmission)section.Data;

            writer.WriteStartElement("Internal");

            writer.WriteStartElement("usePopupMessage");
            writer.WriteString(data.DataInternal.UsePopupMessage ? "1" : "0");
            writer.WriteFullEndElement();

            writer.WriteStartElement("useMobileApp");
            writer.WriteString(data.DataInternal.UseMobileApp ? "1" : "0");
            writer.WriteFullEndElement();

            writer.WriteStartElement("useBroadcast");
            writer.WriteString(data.DataInternal.UseBroadcast ? "1" : "0");
            writer.WriteFullEndElement();

            writer.WriteStartElement("broadcastMessage");
            writer.WriteString(data.DataInternal.BroadcastMessage);
            writer.WriteFullEndElement();

            writer.WriteFullEndElement();       // End Internal

            writer.WriteStartElement("External");

            writer.WriteStartElement("useSMS");
            writer.WriteString(data.DataExternal.UseSMS ? "1" : "0");
            writer.WriteFullEndElement();

            if (data.DataExternal.UseSMS)
            {
                writer.WriteStartElement("SMSText");
                writer.WriteString(data.DataExternal.SMSMessage);
                writer.WriteFullEndElement();

                writer.WriteStartElement("SMSExternalTeamIDList");
                writer.WriteString(GetExternalTeamString(data.DataExternal.SMSReceivers));
                writer.WriteFullEndElement();
            }

            writer.WriteStartElement("useFax");
            writer.WriteString(data.DataExternal.UseFax ? "1" : "0");
            writer.WriteFullEndElement();

            if (data.DataExternal.UseFax)
            {
                writer.WriteStartElement("FaxExternalTeamIDList");
                writer.WriteString(GetExternalTeamString(data.DataExternal.FaxReceivers));
                writer.WriteFullEndElement();
            }

            writer.WriteFullEndElement();   // End External

            return true;
        }

        // dicSectionInfo : Section 별 StepMember ID(상위 4바이트) & Component ID(하위 4바이트)
        private bool MakeArrowList(XmlTextWriter writer, Dictionary<Sections.Section, long> dicSectionInfo, int nStepMemberID)
        {
            writer.WriteStartElement("ArrowList");      // ArrowList 시작

            foreach (KeyValuePair<Sections.Section, long> pair in dicSectionInfo)
            {
                int stepMemberID = (int)(pair.Value >> 32);
                if (stepMemberID != nStepMemberID)
                    continue;

                Sections.Section section = pair.Key;

                foreach (Sections.Arrow arrow in section.Arrows)
                {
                    if (arrow.BeginLink == section)
                    {
                        if (!MakeArrow(writer, arrow, dicSectionInfo))
                            return false;
                    }
                }
            }

            writer.WriteFullEndElement();       // ArrowList 끝
            return true;
        }

        // dicSectionInfo : Section 별 StepMember ID(상위 4바이트) & Component ID(하위 4바이트)
        private int GetComponentID(Sections.Section section, Dictionary<Sections.Section, long> dicSectionInfo)
        {
            if (!dicSectionInfo.ContainsKey(section))
                return -1;

            long nSectionInfo = dicSectionInfo[section];
            return (int)(nSectionInfo & 0xffffffff);
        }

        // dicSectionInfo : Section 별 StepMember ID(상위 4바이트) & Component ID(하위 4바이트)
        private bool MakeArrow(XmlTextWriter writer, Sections.Arrow arrow, Dictionary<Sections.Section, long> dicSectionInfo)
        {
            if (arrow.BeginLink == null || arrow.EndLink == null)
                return false;

            writer.WriteStartElement("Arrow");      // Arrow 시작

            int nBeginComponentID = GetComponentID(arrow.BeginLink, dicSectionInfo);
            int nEndComponentID = GetComponentID(arrow.EndLink, dicSectionInfo);

            writer.WriteStartElement("Text");
            writer.WriteString(arrow.Text);
            writer.WriteFullEndElement();

            writer.WriteStartElement("BeginComponentID");
            writer.WriteString(nBeginComponentID.ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("BeginComponentPosition");
            writer.WriteString(((int)arrow.BeginPosition).ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("EndComponentID");
            writer.WriteString(nEndComponentID.ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("EndComponentPosition");
            writer.WriteString(((int)arrow.EndPosition).ToString());
            writer.WriteFullEndElement();

            writer.WriteFullEndElement();       // Arrow 끝
            return true;
        }

        public string ErrorMessage
        {
            get { return m_strErrorMessage; }
        }

        public string CategoryName
        {
            get { return m_strCategoryName; }
        }

        public string SubCategoryName
        {
            get { return m_strSubCategoryName; }
        }

        public string DisasterName
        {
            get { return m_strDisasterName; }
        }

        public string VersionName
        {
            get { return m_strVersionName; }
        }

        public string Description
        {
            get { return m_strDescription; }
        }

        public ArrayList ActionSteps
        {
            get { return m_arrActionSteps; }
        }

		class ActionStep : Data_ActionStep
		{
			private ArrayList m_arrStepMember = new ArrayList();

			public ArrayList StepMemberList
			{
				get { return m_arrStepMember; }
			}

			public bool m_bSelected = false;
			public bool Selected
			{
				get { return m_bSelected; }
				set { m_bSelected = value; }
			}
		}

		class StepMember : SOPManager.StepMemberDataEx
		{
			private ArrayList m_arrComponent = new ArrayList();
			private ArrayList m_arrArrow = new ArrayList();
			private string m_strTeamName = "";
			private Viewport m_viewport = new Viewport();
			public Viewport Viewport
			{
				get { return m_viewport; }
				set { m_viewport = value; }
			}
			public StepMember()
                : base(-1, Sections.SOPTeam.SOPTeamType.None, -1)
			{
			}

			public ArrayList ComponentList
			{
				get { return m_arrComponent; }
			}

			public ArrayList ArrowList
			{
				get { return m_arrArrow; }
			}

			public string TeamName
			{
				get { return m_strTeamName; }
				set { m_strTeamName = value; }
			}

			private bool m_bModify = false;
			public bool Modify
			{
				get { return m_bModify; }
				set { m_bModify = value; }
			}

		}

		class Arrow
		{
			private string m_strText = "";
			private int m_nBeginComponentID = -1;
			private int m_nBeginComponentPosition = -1;
			private int m_nEndComponentID = -1;
			private int m_nEndComponentPosition = -1;

			public string Text
			{
				get { return m_strText; }
				set { m_strText = value; }
			}

			public int BeginComponentID
			{
				get { return m_nBeginComponentID; }
				set { m_nBeginComponentID = value; }
			}

			public int BeginComponentPosition
			{
				get { return m_nBeginComponentPosition; }
				set { m_nBeginComponentPosition = value; }
			}

			public int EndComponentID
			{
				get { return m_nEndComponentID; }
				set { m_nEndComponentID = value; }
			}

			public int EndComponentPosition
			{
				get { return m_nEndComponentPosition; }
				set { m_nEndComponentPosition = value; }
			}
		}

		class Component
		{
			private int m_nID = -1;
			private string m_strComponentID = "";
			private float x = 0.0f, y = 0.0f;
			private float m_fWidth = 0.0f;
			private float m_fHeight = 0.0f;
			private string m_strText = "";
			private ComponentProperty m_property = null;

            private int m_nVAlign = 0;
            private int m_nHAlign = 0;

            public int VAlign
            {
                get { return m_nVAlign; }
                set { m_nVAlign = value; }
            }
            
            public int HAlign
            {
                get { return m_nHAlign; }
                set { m_nHAlign = value; }
            }

			public int ID
			{
				get { return m_nID; }
				set { m_nID = value; }
			}

			public string ComponentID
			{
				get { return m_strComponentID; }
				set { m_strComponentID = value; }
			}

			public float X
			{
				get { return x; }
				set { x = value; }
			}

			public float Y
			{
				get { return y; }
				set { y = value; }
			}

			public float Width
			{
				get { return m_fWidth; }
				set { m_fWidth = value; }
			}

			public float Height
			{
				get { return m_fHeight; }
				set { m_fHeight = value; }
			}

			public string Text
			{
				get { return m_strText; }
				set { m_strText = value; }
			}

            private string m_szFontName = "";
            public string FontName
            {
                get { return m_szFontName; }
                set { m_szFontName = value; }
            }

            private float m_fFontSize = 0.0f;
            public float FontSize
            {
                get { return m_fFontSize; }
                set { m_fFontSize = value; }
            }

            private float m_fLineSpace = 0.0f;
            public float LineSpace
            {
                get { return m_fLineSpace; }
                set { m_fLineSpace = value; }
            }

            private int m_nFontStyle = 0;
            public int FontStyle
            {
                get { return m_nFontStyle; }
                set { m_nFontStyle = value; }
            }

            private int m_nTextColor = 0;
            public int TextColor
            {
                get { return m_nTextColor; }
                set { m_nTextColor = value; }
            }

			public ComponentProperty Property
			{
				get { return m_property; }
				set { m_property = value; }
			}
		}

		class ComponentProperty
		{
			protected Sections.Section.ComponentType m_nType = Sections.Section.ComponentType.NONE;

			public Sections.Section.ComponentType Type
			{
				get { return m_nType; }
				set { m_nType = value; }
			}
		}

        interface ICommanderOwner
        {
            string CommanderDisplayText
            {
                get;
                set;
            }

            int CommanderTeamID
            {
                get;
                set;
            }

            string CommanderTeamName
            {
                get;
                set;
            }

            int CommanderTeamType
            {
                get;
                set;
            }

            bool CommanderIsTeamMember
            {
                get;
                set;
            }

            int CommanderTeamMemberID
            {
                get;
                set;
            }
        }

        class PropertyProcess : ComponentProperty, ICommanderOwner
		{
			private string m_strTeamList = "";
			private int m_nProcessTime = -1;
			private int m_nProcessTimeType = -1;
			private bool m_useProcessTime = false;
			private bool m_useMissionMessage = false;
			private bool m_onlyTeamLeader = false;
			private ArrayList m_arrMissionList = new ArrayList();

			public string TeamList
			{
				get { return m_strTeamList; }
				set { m_strTeamList = value; }
			}

			public int ProcessTime
			{
				get { return m_nProcessTime; }
				set { m_nProcessTime = value; }
			}

			public int ProcessTimeType
			{
				get { return m_nProcessTimeType; }
				set { m_nProcessTimeType = value; }
			}

			public bool UseProcessTime
			{
				get { return m_useProcessTime; }
				set { m_useProcessTime = value; }
			}

			public bool UseMissionMessage
			{
				get { return m_useMissionMessage; }
				set { m_useMissionMessage = value; }
			}

			public bool OnlyTeamLeader
			{
				get { return m_onlyTeamLeader; }
				set { m_onlyTeamLeader = value; }
			}

			public ArrayList Missions
			{
				get { return m_arrMissionList; }
			}
            private string m_CommanderDisplayText = "";
            public string CommanderDisplayText
            {
                get { return m_CommanderDisplayText; }
                set { m_CommanderDisplayText = value; }
            }

            private int m_CommanderTeamID = -1;
            public int CommanderTeamID
            {
                get { return m_CommanderTeamID; }
                set { m_CommanderTeamID = value; }
            }

            private string m_CommanderTeamName = "SOP 제어권 가진곳의 책임자";
            public string CommanderTeamName
            {
                get { return m_CommanderTeamName; }
                set { m_CommanderTeamName = value; }
            }

            private int m_CommanderTeamType = -1;
            public int CommanderTeamType
            {
                get { return m_CommanderTeamType; }
                set { m_CommanderTeamType = value; }
            }

            private bool m_CommanderIsTeamMember = false;
            public bool CommanderIsTeamMember
            {
                get { return m_CommanderIsTeamMember; }
                set { m_CommanderIsTeamMember = value; }
            }

            private int m_CommanderTeamMemberID = -1;
            public int CommanderTeamMemberID
            {
                get { return m_CommanderTeamMemberID; }
                set { m_CommanderTeamMemberID = value; }
            }

		}

		class PropertyEndPoint : ComponentProperty
		{
			private bool m_isBegin = true;

			public bool IsBegin
			{
				get { return m_isBegin; }
				set { m_isBegin = value; }
			}
		}

		class PropertyLink : ComponentProperty
		{
			private string m_strLinkedComponentName = "";
			private int m_nLinkedStepMemberID = -1;
			private int m_nLinkedID = -1;

			public string LinkedComponentName
			{
				get { return m_strLinkedComponentName; }
				set { m_strLinkedComponentName = value; }
			}

			public int LinkedStepMemberID
			{
				get { return m_nLinkedStepMemberID; }
				set { m_nLinkedStepMemberID = value; }
			}

			public int LinkedID
			{
				get { return m_nLinkedID; }
				set { m_nLinkedID = value; }
			}
		}

		class PropertyTransSOP : ComponentProperty
		{
			private string m_strLinkedCategoryName = "";
			private string m_strLinkedSubCategoryName = "";
			private string m_strLinkedDisasterName = "";
			private string m_strLinkedActionStepName = "";
			private ArrayList m_arrParentActionStepName = new ArrayList();

			public string LinkedCategoryName
			{
				get { return m_strLinkedCategoryName; }
				set { m_strLinkedCategoryName = value; }
			}

			public string LinkedSubCategoryName
			{
				get { return m_strLinkedSubCategoryName; }
				set { m_strLinkedSubCategoryName = value; }
			}

			public string LinkedDisasterName
			{
				get { return m_strLinkedDisasterName; }
				set { m_strLinkedDisasterName = value; }
			}

			public string LinkedActionStepName
			{
				get { return m_strLinkedActionStepName; }
				set { m_strLinkedActionStepName = value; }
			}

			public ArrayList ParentActionStepNameList
			{
				get { return m_arrParentActionStepName; }
			}
		}

        class PropertyInternal : ComponentProperty, ICommanderOwner
		{
			private bool m_usePopupMessage = false;
			private bool m_useSMS = false;
			private bool m_useBroadcast = false;
			private string m_strBroadcastMessage = "";

			public bool UsePopupMessage
			{
				get { return m_usePopupMessage; }
				set { m_usePopupMessage = value; }
			}

            private string m_szTeamList = "";
            public string TeamList
            {
                get { return m_szTeamList; }
                set { m_szTeamList = value; }
            }

			public bool UseSMS
			{
				get { return m_useSMS; }
				set { m_useSMS = value; }
			}

			public bool UseBroadcast
			{
				get { return m_useBroadcast; }
				set { m_useBroadcast = value; }
			}

			public string BroadcastMessage
			{
				get { return m_strBroadcastMessage; }
				set { m_strBroadcastMessage = value; }
			}


            private string m_CommanderDisplayText = "";
            public string CommanderDisplayText
            {
                get { return m_CommanderDisplayText; }
                set { m_CommanderDisplayText = value; }
            }

            private int m_CommanderTeamID = -1;
            public int CommanderTeamID
            {
                get { return m_CommanderTeamID; }
                set { m_CommanderTeamID = value; }
            }

            private string m_CommanderTeamName = "SOP 제어권 가진곳의 책임자";
            public string CommanderTeamName
            {
                get { return m_CommanderTeamName; }
                set { m_CommanderTeamName = value; }
            }

            private int m_CommanderTeamType = -1;
            public int CommanderTeamType
            {
                get { return m_CommanderTeamType; }
                set { m_CommanderTeamType = value; }
            }

            private bool m_CommanderIsTeamMember = false;
            public bool CommanderIsTeamMember
            {
                get { return m_CommanderIsTeamMember; }
                set { m_CommanderIsTeamMember = value; }
            }

            private int m_CommanderTeamMemberID = -1;
            public int CommanderTeamMemberID
            {
                get { return m_CommanderTeamMemberID; }
                set { m_CommanderTeamMemberID = value; }
            }

		}

		class PropertyExternal : ComponentProperty
		{
			private bool m_useSMS = false;
			private string m_strSMSMessage = "";
			private string m_strSMSReceivers = "";
			private bool m_useFax = false;
			private string m_strFaxReceivers = "";

			public bool UseSMS
			{
				get { return m_useSMS; }
				set { m_useSMS = value; }
			}

			public string SMSMessage
			{
				get { return m_strSMSMessage; }
				set { m_strSMSMessage = value; }
			}

			public string SMSReceivers
			{
				get { return m_strSMSReceivers; }
				set { m_strSMSReceivers = value; }
			}

			public bool UseFax
			{
				get { return m_useFax; }
				set { m_useFax = value; }
			}

			public string FaxReceivers
			{
				get { return m_strFaxReceivers; }
				set { m_strFaxReceivers = value; }
			}
		}

		class PropertyTransmission : ComponentProperty
		{
			private PropertyInternal m_dataInternal = null;
			private PropertyExternal m_dataExternal = null;

			public PropertyInternal Internal
			{
				get { return m_dataInternal; }
				set { m_dataInternal = value; }
			}

			public PropertyExternal External
			{
				get { return m_dataExternal; }
				set { m_dataExternal = value; }
			}
		}

		class PropertyGroup : ComponentProperty
		{
			private ArrayList m_GroupItems = new ArrayList();
			public System.Collections.ArrayList GroupItems
			{
				get { return m_GroupItems; }
				set { m_GroupItems = value; }
			}
			public void AddItem(int nItem)
			{
				if (!m_GroupItems.Contains(nItem))
					m_GroupItems.Add(nItem);
			}
		}

        class PropertyMissionItem : ICommanderOwner
        {
            private int m_nTransmissionType = 2;

            private string m_strMission;
            private ArrayList m_arrCheckItem = null;
            private bool bCheck = true;
            private string m_strTarget = "";

            private int nCommanderTeamMemberID = -1;
            public int CommanderTeamMemberID
            {
                get { return nCommanderTeamMemberID; }
                set { nCommanderTeamMemberID = value; }
            }

            private int nCommanderMemberType = -1;
            public int CommanderMemberType
            {
                get { return nCommanderMemberType; }
                set { nCommanderMemberType = value; }
            }

            private int nCommanderTeamID = -1;
            public int CommanderTeamID
            {
                get { return nCommanderTeamID; }
                set { nCommanderTeamID = value; }
            }

            private string szCommanderTeamName = "";
            public string CommanderTeamName
            {
                get { return szCommanderTeamName; }
                set { szCommanderTeamName = value; }
            }

            private string nCommanderDisplayText = "";
            public string CommanderDisplayText
            {
                get { return nCommanderDisplayText; }
                set { nCommanderDisplayText = value; }
            }

            private int nCommanderTeamType = -1;
            public int CommanderTeamType
            {
                get { return nCommanderTeamType; }
                set { nCommanderTeamType = value; }
            }

            private bool bCommanderIsTeamMember = false;
            public bool CommanderIsTeamMember
            {
                get { return bCommanderIsTeamMember; }
                set { bCommanderIsTeamMember = value; }
            }

            public string Target
            {
                get { return m_strTarget; }
                set { m_strTarget = value; }
            }
           
            public int TransmissionType
            {
                get { return m_nTransmissionType; }
                set { m_nTransmissionType = value; }
            }

            public string Mission
            {
                get { return m_strMission; }
                set { m_strMission = value; }
            }

            public ArrayList ArrCheckItem
            {
                get { return m_arrCheckItem; }
                set { m_arrCheckItem = value; }
            }

            public bool CheckItem
            {
                get { return bCheck; }
                set { bCheck = value; }
            }
        }

		class Viewport
		{
			private float m_originX = 0.0f;
			public float OriginX
			{
				get { return m_originX; }
				set { m_originX = value; }
			}

			private float m_originY = 0.0f;
			public float OriginY
			{
				get { return m_originY; }
				set { m_originY = value; }
			}

			private float m_currentX = 0;
			public float CurrentX
			{
				get { return m_currentX; }
				set { m_currentX = value; }
			}

			private float m_currentY = 0;
			public float CurrentY
			{
				get { return m_currentY; }
				set { m_currentY = value; }
			}

			private float fScale = 1.0f;
			public float Scale
			{
				get { return fScale; }
				set { fScale = value; }
			}

			private float fPrevScale = 1.0f;
			public float PrevScale
			{
				get { return fPrevScale; }
				set { fPrevScale = value; }
			}
		}
	}
}
