using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace UnE.SenarioMaker
{
    // DB가 아닌 파일에서 읽고 쓴다.
    internal class XMLManager : IDisposable
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
        
        // Content Read/Write시 데이터의 Validation을 체크하는지 여부
        // 일반적으로 True, 디버깅, Snapshot은 False이어야 한다.
        private bool m_bCheckContent = true;
        public bool CheckContent
        {
            get { return m_bCheckContent; }
            set { m_bCheckContent = value; }
        }

        public void Dispose()
        {

        }
		
        private ArrayList m_arrActionSteps = new ArrayList();

        
        private bool LoadScreen()
        {
            bool bSuccess = false;
            
            Sections.SectionData.ClearIDList();

            // undo/redo 인경우
            if(m_bCheckContent == false)
            {
                TabPage selectedPage = null;
                ArrayList arPageList = FormMain.Instance.ContentForm.GetAllTabPages();
                TabControl tabCtrl = FormMain.Instance.ContentForm.GetTabContorl();

                foreach (ActionStep actionStepXML in m_arrActionSteps)
                {
                    UnE.SenarioMaker.ActionStep actionStep = SenarioManager.Instance.FindActionStep(actionStepXML.StepName, actionStepXML.ID);
                    // 없는 Page인 경우 새 페이지 추가
                    if( actionStep == null)
                    {
                        // Add
                        actionStep = AddNewActionStep(actionStepXML);
                    }
                    else
                    {
                        // 기존 페이지에 갱신
                        Sections.PanelSectionEx panel = actionStep.Panel;
                        panel.ClearSelection();
                        panel.ClearData();
                        AddPanels(actionStepXML, panel);
                        TabPage page = (TabPage)panel.Parent;
                        if( page != null)
                        {
                            arPageList.Remove(page);
                        }
                    }

                    if (actionStep != null)
                        bSuccess = true;

                    if (bSuccess == false)
                        return false;

                    if(actionStepXML.Selected)
                    {                        
                        Sections.PanelSectionEx panel = actionStep.Panel;
                        TabPage page = (TabPage)panel.Parent;
                        selectedPage = page;
                    }
                }

                tabCtrl.SelectedTab = null;
                // 없는 ActionStep에 대한 남아있는 Page는 삭제                
                foreach(TabPage page in arPageList)
                {                   
                    Sections.PanelSectionEx panel = (Sections.PanelSectionEx)page.Tag;
                    UnE.SenarioMaker.ActionStep actionStep = (UnE.SenarioMaker.ActionStep)panel.Tag;
                    SenarioManager.Instance.RemoveActionStep(actionStep);
                }

                // Select current tab
                if (selectedPage == null || !tabCtrl.TabPages.Contains(selectedPage))
                {
                    int nIdx = tabCtrl.TabPages.Count - 1;
                    if( nIdx >= 0 )
                    {
                        selectedPage = tabCtrl.TabPages[nIdx];
                    }                    
                }
                tabCtrl.SelectedTab = selectedPage; 
            }
            else
            {
                FormMain.Instance.ContentForm.SenarioTitle = this.VersionName;
                SenarioManager.Instance.VersionName = this.VersionName;

                SenarioManager.Instance.ActionStepList.Clear();
                // all tab close
                FormMain.Instance.ContentForm.ClearData();

                foreach (ActionStep actionStepXML in m_arrActionSteps)
                {
                    bSuccess = (AddNewActionStep(actionStepXML) != null);
                    //System.Diagnostics.Trace.WriteLine(bSuccess);
                }      
            }          
                 
			return bSuccess;
        }

        private UnE.SenarioMaker.ActionStep AddNewActionStep(ActionStep actionStepXML)
        {
            UnE.SenarioMaker.ActionStep actionStep = SenarioManager.Instance.AddActionStep(actionStepXML.StepName);
            actionStep.StepName = actionStepXML.StepName;
            actionStep.BeginTime = actionStepXML.BeginTime;
            actionStep.EndTime = actionStepXML.EndTime;
            actionStep.DisasterID = actionStepXML.DisasterID;
            actionStep.Iteration = actionStepXML.Iteration;
            actionStep.IterationType = actionStepXML.IterationType;
            actionStep.ParentStepID = actionStepXML.ParentStepID;
            actionStep.PeriodType = actionStepXML.PeriodType;
            actionStep.ProcessTime = actionStepXML.ProcessTime;
            actionStep.ProcessTimeType = actionStepXML.ProcessTimeType;
            actionStep.Selected = actionStepXML.Selected;
            actionStep.WeekdayOption = actionStepXML.WeekdayOption;

            Sections.PanelSectionEx panel = actionStep.Panel;
            bool bResult = AddPanels(actionStepXML, panel);
            if( bResult == true)
            {
                return actionStep;
            }
            return null;
        }

		private bool AddPanels(ActionStep actionStepXML, Sections.PanelSectionEx panel)
        {
            foreach (StepMember stepMember in actionStepXML.StepMemberList)
            {
                // Link된 Section을 알아내기 위하여 ID별 Section 객체 저장
                // 상위 4바이트(StepMember Index) + 하위 4바이트(Component ID), Section 객체
                Dictionary<long, Sections.Section> dicSections = new Dictionary<long, Sections.Section>();

                Dictionary<int, Sections.Section> dicCompSection = new Dictionary<int, Sections.Section>();

                long nStepMemberIndex = 1;

                Dictionary<int, string> dicNormal = null;
                Dictionary<int, string> dicEmergency = null;
                Dictionary<int, string> dicUserDefined = null;
                Dictionary<int, string> dicRegular = null;
                
                panel.DisasterType = m_strDisasterName;

                UnE.SenarioMaker.ActionStep actionStep = (UnE.SenarioMaker.ActionStep)panel.Tag;
                actionStep.TeamName = stepMember.TeamName;
                

                AddComponents(panel, stepMember, nStepMemberIndex, dicSections, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicRegular, ref dicCompSection);

                AddArrows(stepMember, (long)nStepMemberIndex, dicSections);  
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

        private bool AddComponents(Sections.PanelSectionEx panel, StepMember stepMember, long nStepMemberIndex, Dictionary<long, Sections.Section> dicSections, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined,  ref Dictionary<int, string> dicRegular, ref Dictionary<int, Sections.Section> dicCompSection)
        {
            foreach (Component component in stepMember.ComponentList)
            {
                ComponentProperty property = component.Property;
				Sections.Section section = ToSection(panel, component.X, component.Y, property, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicRegular, dicCompSection);

                if (section == null)
                    return false;

                if (section.GetComponentType() == Sections.Section.ComponentType.PROCESS)
                {
                    ((Sections.SectionProcess)section).TextUP = component.Text;
                }

                section.Data.ComponentID = component.ComponentID;
                section.Data.Title = component.Text;

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

        private Sections.Section ToSection(Sections.PanelSectionEx panel, float x, float y, ComponentProperty property, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined, ref Dictionary<int, string> dicRegular, Dictionary<int, Sections.Section> dicCompSection)
        {
            if (property.Type == Sections.Section.ComponentType.PROCESS)
            {
                return ToSectionProcess(panel, x, y, (PropertyProcess)property, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicRegular);
            }
            else if (property.Type == Sections.Section.ComponentType.DECISION)
            {
                Sections.SectionDecision section = new Sections.SectionDecision(panel, x, y);
                if(property.UserData.ContainsKey("expr"))
                {
                    string szExpr = (string)property.UserData["expr"];
                    if (szExpr != null && szExpr != "")
                    {
                        section.Data.Expression = szExpr;
                    }
                }
                if (property.UserData.ContainsKey("showExpr"))
                {
                    string szExpr = (string)property.UserData["showExpr"];
                    if (szExpr != null && szExpr != "" && szExpr.ToLower() == "true")
                    {
                        section.Data.ShowExpression = true;
                    }
                    else
                    {
                        section.Data.ShowExpression = false;
                    }
                }
                return section;
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
				//return ToSectionLink(panel, x, y, (PropertyLink)property, ref dicLinkSections);
            }
            else if (property.Type == Sections.Section.ComponentType.TRANSSOP)
            {
				//return ToSectionTransSOP(panel, x, y, (PropertyTransSOP)property);
            }
            else if (property.Type == Sections.Section.ComponentType.INTERNAL)
            {
				//return ToSectionInternal(panel, x, y, (PropertyInternal)property);
            }
            else if (property.Type == Sections.Section.ComponentType.EXTERNAL)
            {
				//return ToSectionExternal(panel, x, y, (PropertyExternal)property, ref dicExternal);
            }
            else if (property.Type == Sections.Section.ComponentType.TRANSMISSION)
            {
				//return ToSectionTransmission(panel, x, y, (PropertyTransmission)property, ref dicExternal);
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
            Sections.SectionTransmission section = new Sections.SectionTransmission(panel, x, y);
            Sections.SectionDataTransmission data = (Sections.SectionDataTransmission)section.Data;

            data.DataInternal.UsePopupMessage = property.Internal.UsePopupMessage;
            data.DataInternal.UseMobileApp = property.Internal.UseSMS;
            data.DataInternal.UseBroadcast = property.Internal.UseBroadcast;
            data.DataInternal.BroadcastMessage = property.Internal.BroadcastMessage;

            data.DataExternal.UseSMS = property.External.UseSMS;
            data.DataExternal.SMSMessage = property.External.SMSMessage;
            data.DataExternal.UseFax = property.External.UseFax;

            return section;
        }

        private Sections.SectionExternal ToSectionExternal(Sections.PanelSectionEx panel, float x, float y, PropertyExternal property, ref Dictionary<int, Sections.ExternalTeamData> dicExternal)
        {
            Sections.SectionExternal section = new Sections.SectionExternal(panel, x, y);
            Sections.SectionDataExternal data = (Sections.SectionDataExternal)section.Data;

            data.UseSMS = property.UseSMS;
            data.SMSMessage = property.SMSMessage;
            data.UseFax = property.UseFax;

            return section;
        }

        private Sections.SectionInternal ToSectionInternal(Sections.PanelSectionEx panel, float x, float y, PropertyInternal property)
        {
            Sections.SectionInternal section = new Sections.SectionInternal(panel, x, y);
            Sections.SectionDataInternal data = (Sections.SectionDataInternal)section.Data;

            data.UsePopupMessage = property.UsePopupMessage;
            data.UseMobileApp = property.UseSMS;
            data.UseBroadcast = property.UseBroadcast;
            data.BroadcastMessage = property.BroadcastMessage;

            return section;
        }

        private int GetLinkedActionStepID(PropertyTransSOP property)
        {
            //    string strFormat = "select id, DisasterID from ActionStep where StepName = '{0}' and DisasterID in ";
            //    strFormat += "(select id from Disaster where DisasterName = '{1}' and SubDisasterID in ";
            //    strFormat += "(select id from SubDisasterCategory where SubCategoryName = '{2}' and DisasterID in ";
            //    strFormat += "(select id from DisasterCategory where CategoryName = '{3}')";

            //    WebDBManager dbMgr = FormMain.Instance.DBManager;

            //    string strSQL = string.Format(strFormat, property.LinkedActionStepName, property.LinkedDisasterName, property.LinkedSubCategoryName, property.LinkedCategoryName);
            //    ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            //    if (arrResult == null)
            //        return -1;

            //    int nResultCount = arrResult.Count;
            //    if (nResultCount == 0)
            //        return -1;

            //      int nActionStepID = WebDBManager.GetIntField(arrResult[nResultCount - 2].ToString(), -1);
            //    int nDisasterID = WebDBManager.GetIntField(arrResult[nResultCount - 1].ToString(), -1);

            //    int nParentCount = property.ParentActionStepNameList.Count;
            //    if (nParentCount == 0)
            //        return nActionStepID;

            //    for (int i = nParentCount - 1; i >= 0; i--)
            //    {
            //        string strParentActionStepName = (string)property.ParentActionStepNameList[i];

            //        strSQL = string.Format("select id from ActionStep where DisasterID = {0} and StepName = {1} and ParentStepID = {2}",
            //            nDisasterID, strParentActionStepName, i == nParentCount - 1 ? "NULL" : nActionStepID.ToString());

            //        arrResult = dbMgr.GetResultData(strSQL, 0);
            //        if (arrResult == null || arrResult.Count == 0)
            //            return nActionStepID;

            //        nActionStepID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            //    }
            int nActionStepID = -1;
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

        private Sections.SectionProcess ToSectionProcess(Sections.PanelSectionEx panel, float x, float y, PropertyProcess property, ref Dictionary<int, string> dicNormal, ref Dictionary<int, string> dicEmergency, ref Dictionary<int, string> dicUserDefined,  ref Dictionary<int, string> dicRegular)
        {
            Sections.SectionProcess section = new Sections.SectionProcess(panel, x, y);
            Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;

            data.ProcessingTime.Time = property.ProcessTime;

            Sections.ProcessingTime.Type type = Sections.ProcessingTime.Type.UNKNOWN;
            if (!Sections.ProcessingTime.IntToType(property.ProcessTimeType, ref type))
                return null;

            data.ProcessingTime.ProcessingType = type;
            data.UseProcessingTime = property.UseProcessTime;
            data.MissionTransfer = property.UseMissionMessage;
            data.TransferTeamLeaderOnly = property.OnlyTeamLeader;

            /*foreach (string strMission in property.Missions)
            {
                Sections.MissionItem item = new Sections.MissionItem();
                item.Mission = strMission;
                data.MissionItems.Add(item);
            }*/
            foreach (Sections.MissionItem item in property.Missions)
            {
                data.MissionItems.Add(item);
            }
                       
            if (property.UserData.ContainsKey("expr"))
            {
                string szExpr = (string)property.UserData["expr"];
                if (szExpr != null && szExpr != "")
                {
                    data.Expression = szExpr;
                }
            }

            if (property.UserData.ContainsKey("showExpr"))
            {
                string szExpr = (string)property.UserData["showExpr"];
                if (szExpr != null && szExpr != "" && szExpr.ToLower() == "true")
                {
                    data.ShowExpression = true;
                }
                else
                {
                    data.ShowExpression = false;
                }
            }

            return section;
        }

    

        public bool Load(XmlTextReader reader)
        {
            if (reader == null)
                return false;

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
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Body", true) == 0)
                            {
                                if (!ReadBody(reader))
                                    return false;
                            }
                            else if(string.Compare(reader.Name, "Variables", true) == 0)
                            {
                                if (!ReadVariables(reader))
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
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                reader.Close();
                return false;
            }

            reader.Close();

            return LoadScreen();
        }

        public bool Load(System.IO.Stream stream)
        {
            XmlTextReader reader = InitReader(stream);
            return Load(reader);
        }

        private PopupTransparentMessage m_popupCheckValidXML = null;

        public bool Load(string strPath)
        {
            Cursor cursorPrev = FormMain.Instance.Cursor;
            FormMain.Instance.Cursor = Cursors.WaitCursor;

            XmlTextReader reader = InitReader(strPath);
            bool result = Load(reader);

            FormMain.Instance.Cursor = cursorPrev;
            return result;
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
                UnE.Utility.UMessageBox.Show(e.Message, "XML 유효성 검증 실패");
                m_strErrorMessage = e.Message;
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
            if (m_popupCheckValidXML != null)
            {
                if (m_popupCheckValidXML.Visible)
                {
                    m_popupCheckValidXML.Close();
                }
            }

            if(m_bCheckContent == true)
            {
                m_popupCheckValidXML = new PopupTransparentMessage();

                m_popupCheckValidXML.Text = "XML 유효성을 검토하고 있습니다.";
                m_popupCheckValidXML.Text2 = "이 작업은 몇분까지 걸릴수 있습니다.";
                m_popupCheckValidXML.UseMultiLine = true;

                m_popupCheckValidXML.Show();

                if (!SchemaValidationCheck(strPath))
                {
                    if (m_popupCheckValidXML.Visible)
                    {
                        m_popupCheckValidXML.Close();
                        m_popupCheckValidXML = null;
                    }

                    if (UnE.Utility.UMessageBox.Show("XML 스키마에 맞지 않는 데이터 파일입니다.\r\n계속 진행하시겠습니까?", "XML 스키마 오류", MessageBoxButtons.YesNo) == DialogResult.No)
                        return null;
                }

                if (m_popupCheckValidXML.Visible)
                {
                    m_popupCheckValidXML.Close();
                    m_popupCheckValidXML = null;
                }
            }
            
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
            //UnE.Utility.UMessageBox.Show(e.Message);
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

        private bool ReadVariables(XmlTextReader reader)
        {
            bool stop = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "UserVariable", true) == 0)
                        {
                            if (!ReadUserVariable(reader))
                                return false;
                        }
                        else if (string.Compare(reader.Name, "UserEnum", true) == 0)
                        {
                            if (!ReadEnumVariable(reader))
                                return false;
                        }
                        else
                            PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        if(string.Compare(reader.Name, "UserVariable",false) == 0 && string.Compare(reader.Name, "UserEnum", false) == 0)
                        {
                            stop = true;
                        }
                        
                        break;
                }
                if (stop)
                    break;
            }

            return true;
            //SenarioManager.Instance.UserVariables = reader.
        }


        private bool ReadUserVariable(XmlTextReader reader)
        {
            string strName = "";
            string strType = "";
            object objMaxValue = null;
            object objMinValue = null;
            object objValue = null;
            string strDesc = "";

            ArrayList arrVariables = new ArrayList();
            while(reader.MoveToNextAttribute())
            {
                if (string.Compare(reader.Name, "name", true) == 0)
                {
                    try
                    {
                        strName = reader.Value;
                    }
                    catch (Exception e)
                    {
                        m_strErrorMessage = string.Format("Line Number {0}, " + e.Message, reader.LineNumber);
                        return true;
                    }
                }

                else if (string.Compare(reader.Name, "type", true) == 0)
                {
                    try
                    {
                        strType = reader.Value;
                    }
                    catch (Exception e)
                    {
                        m_strErrorMessage = string.Format("Line Number {0}, " + e.Message, reader.LineNumber);
                        return true;
                    }
                }

                else if (string.Compare(reader.Name, "maxValue", true) == 0)
                {
                    try
                    {
                        objMaxValue = reader.Value;
                    }
                    catch (Exception e)
                    {
                        m_strErrorMessage = string.Format("Line Number {0}, " + e.Message, reader.LineNumber);
                        return true;
                    }
                }

                else if (string.Compare(reader.Name, "minValue", true) == 0)
                {
                    try
                    {
                        objMinValue = reader.Value;
                    }
                    catch (Exception e)
                    {
                        m_strErrorMessage = string.Format("Line Number {0}, " + e.Message, reader.LineNumber);
                        return true;
                    }
                }

                else if (string.Compare(reader.Name, "value", true) == 0)
                {
                    try
                    {
                        objValue = reader.Value;
                    }
                    catch (Exception e)
                    {
                        m_strErrorMessage = string.Format("Line Number {0}, " + e.Message, reader.LineNumber);
                        return true;
                    }
                }

                else if (string.Compare(reader.Name, "description", true) == 0)
                {
                    try
                    {
                        strDesc = reader.Value;
                    }
                    catch (Exception e)
                    {
                        m_strErrorMessage = string.Format("Line Number {0}, " + e.Message, reader.LineNumber);
                        return true;
                    }
                }
            }

            UserVariable var = new UserVariable(strName, strType, strDesc);
            var.MaxValue = objMaxValue;
            var.MinValue = objMinValue;
            var.Value = objValue;

            SenarioManager.Instance.UserVariables.AddVariable(var);

            FormMain.Instance.FormUserVariable.UpdateUserVariable();
            //UserVariable var = new UserVariable(szName, szType, szDesc);

            return true;
        }

        private bool ReadEnumVariable(XmlTextReader reader)
        {
            string strName = "";
            string strType = "";
            object objValue = null;
            string strDesc = "";

            ArrayList arrVariables = new ArrayList();
            while (reader.MoveToNextAttribute())
            {
                if (string.Compare(reader.Name, "name", true) == 0)
                {
                    try
                    {
                        strName = reader.Value;
                    }
                    catch (Exception e)
                    {
                        m_strErrorMessage = string.Format("Line Number {0}, " + e.Message, reader.LineNumber);
                        return true;
                    }
                }

                else if (string.Compare(reader.Name, "type", true) == 0)
                {
                    try
                    {
                        strType = reader.Value;
                    }
                    catch (Exception e)
                    {
                        m_strErrorMessage = string.Format("Line Number {0}, " + e.Message, reader.LineNumber);
                        return true;
                    }
                }

                else if (string.Compare(reader.Name, "value", true) == 0)
                {
                    try
                    {
                        objValue = reader.Value;
                    }
                    catch (Exception e)
                    {
                        m_strErrorMessage = string.Format("Line Number {0}, " + e.Message, reader.LineNumber);
                        return true;
                    }
                }

                else if (string.Compare(reader.Name, "description", true) == 0)
                {
                    try
                    {
                        strDesc = reader.Value;
                    }
                    catch (Exception e)
                    {
                        m_strErrorMessage = string.Format("Line Number {0}, " + e.Message, reader.LineNumber);
                        return true;
                    }
                }
            }

            Enums var = new Enums(strName, strType, objValue, strDesc);
            var.Value = objValue;

            SenarioManager.Instance.EnumList.AddVariable(var);

            FormMain.Instance.FormEnumeration.UpdateUserVariable();
            //UserVariable var = new UserVariable(szName, szType, szDesc);

            return true;
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

				if( m_bCheckContent == false)
				{				
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
            }

            if (m_bCheckContent == true && nActionStepID < 0)
            {
                m_strErrorMessage = string.Format("Line Number {0}, id가 없는 ActionStep Element가 존재합니다.", reader.LineNumber);
                return true;
            }

            bool stop = false, readStepName = false, readPeriodType = false, readWeekDayOption = false;
            bool readIteration = false, readIterationType = false, readProcessTime = false, readStepMemberList = false;

            string strStepName = "";
            int nPeriodType = -1, nWeekDayOption = -1, nIteration = -1, nIterationType = -1, nProcessTime = -1;

            ActionStep actionStep = new ActionStep();
            actionStep.ID = nActionStepID;

			if( m_bCheckContent == false)
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
                            else
                                readStepMemberList = true;
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

            if (!readStepMemberList && m_bCheckContent)
            {
                m_strErrorMessage = string.Format("Line Number {0}, StepMemberList가 존재하지 않습니다.", reader.LineNumber);
                return false;
            }

            m_arrActionSteps.Add(actionStep);
            return true;
        }

        private bool ReadParentStepID(XmlTextReader reader, ref int nParentStepID)
        {
            if (reader.IsEmptyElement && m_bCheckContent)
                return false;
            return ReadInt(reader, ref nParentStepID, "ParentStepID는", "ParentStepID가");
        }

        private bool ReadProcessTimeType(XmlTextReader reader, ref int nProcessTimeType)
        {
            if (reader.IsEmptyElement && m_bCheckContent)
                return false;
            return ReadInt(reader, ref nProcessTimeType, "ProcessTimeType은", "ProcessTimeType이");
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
            if (reader.IsEmptyElement && m_bCheckContent)
                return true;
            return ReadDateTime(reader, ref dtBegin, "BeginTime");
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
            //stepMember.TeamID = nTeamID;
            //stepMember.TeamType = nTeamType;
            stepMember.TeamName = strStepMemberName;

            bool stop = false;//, readComponentList = false, readArrowList = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "ComponentList", true) == 0)
                        {
                            if (!ReadComponentList(reader, stepMember))
                                return null;
                            //else
                            //    readComponentList = true;
                        }
                        else if (string.Compare(reader.Name, "ArrowList", true) == 0)
                        {
                            if (!ReadArrowList(reader, stepMember))
                                return null;
                            //else
                            //    readArrowList = true;
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

            //if (!readComponentList)
            //{
            //    m_strErrorMessage = string.Format("Line Number {0}, ComponentList가 존재하지 않습니다.", reader.LineNumber);
            //    return null;
            //}

            //if (!readArrowList)
            //{
            //    m_strErrorMessage = string.Format("Line Number {0}, ArrowList가 존재하지 않습니다.", reader.LineNumber);
            //    return null;
            //}

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
            bool stop = false;//, readBeginComponentID = false;
           // bool readBeginComponentPosition = false, readEndComponentID = false, readEndComponentPosition = false;

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
                            if (!ReadBeginComponentID(reader, ref nBeginComponentID))
                                return false;

                            //readBeginComponentID = true;
                            arrow.BeginComponentID = nBeginComponentID;
                        }
                        else if (string.Compare(reader.Name, "BeginComponentPosition", true) == 0)
                        {
                            if (!ReadBeginComponentPosition(reader, ref nBeginComponentPosition))
                                return false;

                            //readBeginComponentPosition = true;
                            arrow.BeginComponentPosition = nBeginComponentPosition;
                        }
                        else if (string.Compare(reader.Name, "EndComponentID", true) == 0)
                        {
                            if (!ReadEndComponentID(reader, ref nEndComponentID))
                                return false;

                            //readEndComponentID = true;
                            arrow.EndComponentID = nEndComponentID;
                        }
                        else if (string.Compare(reader.Name, "EndComponentPosition", true) == 0)
                        {
                            if (!ReadEndComponentPosition(reader, ref nEndComponentPosition))
                                return false;

                            //readEndComponentPosition = true;
                            arrow.EndComponentPosition = nEndComponentPosition;
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
            if (reader.IsEmptyElement && m_bCheckContent)
                return false;

            return ReadInt(reader, ref nBeginComponentID, "BeginComponentID는", "BeginComponentID가");
        }

        private bool ReadBeginComponentPosition(XmlTextReader reader, ref int nBeginComponentPosition)
        {
            if (reader.IsEmptyElement && m_bCheckContent)
                return false;

            return ReadInt(reader, ref nBeginComponentPosition, "BeginComponentPosition은", "BeginComponentPosition은");
        }

        private bool ReadEndComponentID(XmlTextReader reader, ref int nEndComponentID)
        {
            if (reader.IsEmptyElement && m_bCheckContent)
                return false;

            return ReadInt(reader, ref nEndComponentID, "EndComponentID는", "EndComponentID가");
        }

        private bool ReadEndComponentPosition(XmlTextReader reader, ref int nEndComponentPosition)
        {
            if (reader.IsEmptyElement && m_bCheckContent)
                return false;

            return ReadInt(reader, ref nEndComponentPosition, "EndComponentPosition은", "EndComponentPosition은");
        }

        private bool ReadComponentList(XmlTextReader reader, StepMember stepMember)
        {
            if (reader.IsEmptyElement && m_bCheckContent)
                return true;

            bool stop = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Component", true) == 0)
                        {
                            Component component = ReadComponent(reader);
                            if (component == null && m_bCheckContent)
                                return false;
							if (component != null)
                            	stepMember.ComponentList.Add(component);
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
                        else if (string.Compare(reader.Name, "ComponentID", true) == 0)
                        {
                            if (!ReadComponentID(reader, ref strComponentID))
                                return null;

                            readComponentID = true;
                            component.ComponentID = strComponentID;
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
            else if (nType == (int)Sections.Section.ComponentType.DECISION)
                property = ReadDecisionProperty(reader);
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
                                string[] sep = { "," };
                                string[] items = groupItem.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                                for (int i = 0; i < items.Length; i++)
                                {
                                    string szItem = items[i].Replace(",", "");
                                    int nItem = 0;
                                    if (int.TryParse(szItem, out nItem))
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
            bool stop = false, readUseSMS = false, readSMSText = false, readSMSExternalTeamIDList = false;
            bool readUseFax = false, readFaxExternalTeamIDList = false;

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
                            if (!ReadUseSMS(reader, ref useSMS) && m_bCheckContent )
                                return null;

                            readUseSMS = true;
                            property.UseSMS = useSMS;
                        }
                        else if (string.Compare(reader.Name, "SMSText", true) == 0)
                        {
                            if (ReadSMSText(reader, ref strSMSText))
                            {
                                readSMSText = true;
                                property.SMSMessage = strSMSText;
                            }
							else
                            {
                                if (!m_bCheckContent)
                                {
                                    if (strSMSText == "null")
                                        strSMSText = "";
                                    readSMSText = true;
                                    property.SMSMessage = strSMSText;
                                }
                            }							
                        }
                        else if (string.Compare(reader.Name, "SMSExternalTeamIDList", true) == 0)
                        {
                            if (ReadSMSExternalTeamIDList(reader, ref strSMSExternalTeamIDList))
                            {
                                readSMSExternalTeamIDList = true;
                                property.SMSReceivers = strSMSExternalTeamIDList;
                            }
                            else
                            {
                                if (!m_bCheckContent)
                                {
                                    if (strSMSExternalTeamIDList == "null")
                                        strSMSExternalTeamIDList = "";
                                    readSMSExternalTeamIDList = true;
                                    property.SMSReceivers = strSMSExternalTeamIDList;
                                }
                            }
                        }
                        else if (string.Compare(reader.Name, "useFax", true) == 0)
                        {
                            if (!ReadUseFax(reader, ref useFax) && m_bCheckContent)
                                return null;

                            readUseFax = true;
                            property.UseFax = useFax;
                        }
                        else if (string.Compare(reader.Name, "FaxExternalTeamIDList", true) == 0)
                        {
                            if (ReadFaxExternalTeamIDList(reader, ref strFaxExternalTeamIDList))
                            {
                                readFaxExternalTeamIDList = true;
                                property.FaxReceivers = strFaxExternalTeamIDList;
                            }
                            else
                            {
                                if(!m_bCheckContent)
                                {
                                    if (strFaxExternalTeamIDList == "null")
                                        strFaxExternalTeamIDList = "";
                                    readFaxExternalTeamIDList = true;
                                    property.FaxReceivers = strFaxExternalTeamIDList;
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


            if(m_bCheckContent == true)
            {
                if (!readUseSMS)
                {
                    m_strErrorMessage = string.Format("Line Number {0}, useSMS가 존재하지 않습니다.", reader.LineNumber);
                    return null;
                }
                else
                {
                    if (useSMS)
                    {
                        if (!readSMSText)
                        {
                            m_strErrorMessage = string.Format("Line Number {0}, useSMS가 true인데 SMSText가 존재하지 않습니다.", reader.LineNumber);
                            return null;
                        }

                        if (!readSMSExternalTeamIDList)
                        {
                            m_strErrorMessage = string.Format("Line Number {0}, useSMS가 true인데 SMSExternalTeamIDList가 존재하지 않습니다.", reader.LineNumber);
                            return null;
                        }
                    }
                }

                if (!readUseFax)
                {
                    m_strErrorMessage = string.Format("Line Number {0}, useFax가 존재하지 않습니다.", reader.LineNumber);
                    return null;
                }
                else
                {
                    if (useFax)
                    {
                        if (!readFaxExternalTeamIDList)
                        {
                            m_strErrorMessage = string.Format("Line Number {0}, useFax가 true인데 FaxExternalTeamIDList가 존재하지 않습니다.", reader.LineNumber);
                            return null;
                        }
                    }
                }
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
            if (reader.IsEmptyElement && m_bCheckContent)
                return false;
            return ReadBoolean(reader, ref useSMS, "useSMS가", "useSMS는");
        }

        private bool ReadSMSText(XmlTextReader reader, ref string strSMSText)
        {
            if (reader.IsEmptyElement && m_bCheckContent)
                return false;

            if (!ReadElementText(reader, ref strSMSText))
            {
                m_strErrorMessage = string.Format("Line Number {0}, SMSText가 비어있습니다.", reader.LineNumber);
                return false;
            }

            return true;
        }

        private bool ReadSMSExternalTeamIDList(XmlTextReader reader, ref string strSMSExternalTeamIDList)
        {
            if (reader.IsEmptyElement && m_bCheckContent)
                return false;

            if (!ReadElementText(reader, ref strSMSExternalTeamIDList))
            {
                m_strErrorMessage = string.Format("Line Number {0}, SMSExternalTeamIDList가 비어있습니다.", reader.LineNumber);
                return false;
            }

            return true;
        }

        private bool ReadUseFax(XmlTextReader reader, ref bool useFax)
        {
            if (reader.IsEmptyElement && m_bCheckContent)
                return false;

            return ReadBoolean(reader, ref useFax, "useFax가", "useFax는");
        }

        private bool ReadFaxExternalTeamIDList(XmlTextReader reader, ref string strFaxExternalTeamIDList)
        {
            if (reader.IsEmptyElement && m_bCheckContent)
                return false;

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
                        if (string.Compare(reader.Name, "usePopupMessage", true) == 0)
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
            bool stop = false, readCategoryName = false, readSubCategoryName = false, readDisasterName = false, readActionStepName = false;
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
                            if (!ReadCategoryName(reader, ref strCategoryName) && m_bCheckContent)
                                return null;

                            if (strCategoryName == "null")
                                strCategoryName = "";

                            readCategoryName = true;
                            property.LinkedCategoryName = strCategoryName;
                        }
                        else if (string.Compare(reader.Name, "SubCategoryName", true) == 0)
                        {
                            if (!ReadSubCategoryName(reader, ref strSubCategoryName) && m_bCheckContent)
                                return null;

                            if (strSubCategoryName == "null")
                                strSubCategoryName = "";

                            readSubCategoryName = true;
                            property.LinkedSubCategoryName = strSubCategoryName;
                        }
                        else if (string.Compare(reader.Name, "DisasterName", true) == 0)
                        {
                            if (!ReadDisasterName(reader, ref strDisasterName) && m_bCheckContent)
                                return null;

                            if (strDisasterName == "null")
                                strDisasterName = "";

                            readDisasterName = true;
                            property.LinkedDisasterName = strDisasterName;
                        }
                        else if (string.Compare(reader.Name, "ActionStepName", true) == 0)
                        {
                            if (!ReadActionStepName(reader, ref strActionStepName, "ActionStepName") && m_bCheckContent)
                                return null;

                            if (strActionStepName == "null")
                                strActionStepName = "";

                            readActionStepName = true;
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


            if( m_bCheckContent )
            {
                if (!readCategoryName)
                {
                    m_strErrorMessage = string.Format("Line Number {0}, CategoryName이 존재하지 않습니다.", reader.LineNumber);
                    return null;
                }

                if (!readSubCategoryName)
                {
                    m_strErrorMessage = string.Format("Line Number {0}, SubCategoryName이 존재하지 않습니다.", reader.LineNumber);
                    return null;
                }

                if (!readDisasterName)
                {
                    m_strErrorMessage = string.Format("Line Number {0}, DisasterName이 존재하지 않습니다.", reader.LineNumber);
                    return null;
                }

                if (!readActionStepName)
                {
                    m_strErrorMessage = string.Format("Line Number {0}, ActionStepName이 존재하지 않습니다.", reader.LineNumber);
                    return null;
                }
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
            bool stop = false, readLinkedComponentName = false, readLinkedStepMemberID = false, readLinkedComponentID = false;
            string strLinkedComponentName = "";
            int nLinkedStepMemberID = -1, nLinkedComponentID = -1;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "LinkedComponentName", true) == 0)
                        {
                            if (!ReadLinkedComponentName(reader, ref strLinkedComponentName) && m_bCheckContent)
                                return null;
                            
                            if (strLinkedComponentName == "null")
                                strLinkedComponentName = "";

                            readLinkedComponentName = true;
                        }
                        else if (string.Compare(reader.Name, "LinkedStepMemberIndex", true) == 0)
                        {
                            if (!ReadLinkedStepMemberID(reader, ref nLinkedStepMemberID) && m_bCheckContent)
                                return null;

                            readLinkedStepMemberID = true;
                        }
                        else if (string.Compare(reader.Name, "LinkedComponentID", true) == 0)
                        {
                            if (!ReadLinkedComponentID(reader, ref nLinkedComponentID) && m_bCheckContent)
                                return null;

                            readLinkedComponentID = true;
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

            if( m_bCheckContent)
            {
                if (!readLinkedComponentName)
                {
                    m_strErrorMessage = string.Format("Line Number {0}, LinkedComponentName이 존재하지 않습니다.", reader.LineNumber);
                    return null;
                }

                if (!readLinkedStepMemberID)
                {
                    m_strErrorMessage = string.Format("Line Number {0}, LinkedStepMemberIndex가 존재하지 않습니다.", reader.LineNumber);
                    return null;
                }

                if (!readLinkedComponentID)
                {
                    m_strErrorMessage = string.Format("Line Number {0}, LinkedComponentID가 존재하지 않습니다.", reader.LineNumber);
                    return null;
                }
            }           

            PropertyLink property = new PropertyLink();
            property.LinkedComponentName = strLinkedComponentName;
            property.LinkedStepMemberID = nLinkedStepMemberID;
            property.LinkedID = nLinkedComponentID;

            return property;
        }

        private bool ReadLinkedComponentName(XmlTextReader reader, ref string strLinkedComponentName)
        {
            if (reader.IsEmptyElement && m_bCheckContent)
                return false;

            if (!ReadElementText(reader, ref strLinkedComponentName) && m_bCheckContent)
            {
                m_strErrorMessage = string.Format("Line Number {0}, LinkedComponentName이 비어있습니다.", reader.LineNumber);
                return false;
            }
            else
            {
                ReadElementText(reader, ref strLinkedComponentName);
            }

            return true;
        }

        private bool ReadLinkedStepMemberID(XmlTextReader reader, ref int nLinkedStepMemberID)
        {
            if (reader.IsEmptyElement && m_bCheckContent)
                return false;

            return ReadInt(reader, ref nLinkedStepMemberID, "LinkedStepMemberIndex는", "LinkedStepMemberIndex가");
        }

        private bool ReadLinkedComponentID(XmlTextReader reader, ref int nLinkedComponentID)
        {
            if (reader.IsEmptyElement && m_bCheckContent)
                return false;

            return ReadInt(reader, ref nLinkedComponentID, "LinkedComponentID는", "LinkedComponentID가");
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
            if (reader.IsEmptyElement && m_bCheckContent)
                return false;
            return ReadBoolean(reader, ref isBegin, "isBegin이", "isBegin은");
        }

        private PropertyDecision ReadDecisionProperty(XmlTextReader reader)
        {
            bool stop = false;
            PropertyDecision property = new PropertyDecision();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:                        
                        if (string.Compare(reader.Name, "UserData", true) == 0)
                        {
                            ReadUserData(reader, property);
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
            return property;
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
                        else if (string.Compare(reader.Name, "UserData", true) == 0)
                        {
                            ReadUserData(reader, property);                            
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

        private bool ReadUserData(XmlTextReader reader, ComponentProperty property)
        {
            bool IsExpr = false;
            string szName = "";
            string szType = "";
            string szValue = "";
            while (reader.MoveToNextAttribute())
            {
                if (string.Compare(reader.Name, "name", true) == 0)
                {
                    string szTemp = reader.Value;

                    if (szTemp == "expr")
                    {
                        IsExpr = true;                        
                        szType = "expr";
                    }
                    szName = szTemp;
                }
                else if (string.Compare(reader.Name, "value", true) == 0)
                {
                    szValue = reader.Value;                 
                }
            }

            if (reader.IsEmptyElement)
                return true;

            string strExpr = "";
            if (IsExpr == true)
            {
                ReadElementText(reader, ref strExpr);

                if (szName != "" && szType == "expr")
                    property.AddUserData(szName, strExpr);
            }  
            else
            {
                property.AddUserData(szName, szValue);
            }
            return true;
        }

        private bool ReadMissionList(XmlTextReader reader, PropertyProcess property)
        {
            if (reader.IsEmptyElement)
                return true;

            bool stop = false;
            //string strMission = "";
            Sections.MissionItem mission = null;

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
                            /*if (ReadTeamList(reader, ref strMission))
                            {
                                property.Missions.Add(strMission);
                            }*/
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

        private Sections.MissionItem ReadMission(XmlTextReader reader)
        {
            Sections.MissionItem item = new Sections.MissionItem();
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

        public bool Save(SenarioManager manager, System.IO.Stream stream, string strVersionName, string strDescription = null)
        {
            XmlTextWriter writer = InitWriter(stream);
            return Save(writer, manager, strVersionName, strDescription);
        }

        public bool Save(SenarioManager manager, string strPath, string strVersionName, string strDescription = null)
        {
            XmlTextWriter writer = InitWriter(strPath);
            return Save(writer, manager, strVersionName, strDescription);
        }


        protected bool Save(XmlTextWriter writer, SenarioManager manager, string strVersionName, string strDescription = null)
        {
            writer.WriteStartElement("SOP");        // SOP 시작

            writer.WriteStartAttribute("xmlns:xsi");
            writer.WriteString("http://www.w3.org/2001/XMLSchema-instance");
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("xsi:noNamespaceSchemaLocation");
            writer.WriteString("http://unes.iptime.org:9808/SOP/XML/SOP.xsd");
            writer.WriteEndAttribute();

            if (!MakeHeader(writer, manager, strVersionName, strDescription))
                return false;

            if (!MakeBody(writer, manager))
                return false;
           
            if (!MakeVariables(writer))
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

        private bool MakeHeader(XmlTextWriter writer, SenarioManager manager, string strVersionName, string strDescription = null)
        {
            writer.WriteStartElement("Header"); // Header 시작

            //FormNewSOP pageDisaster = frm.GetPageDisaster();

            string strDisaster = manager.DisasterType;
            string strSubCategory = manager.SubCategory;
            string strCategory = manager.Category;


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
            //writer.WriteString(frm.GetPageDisaster().IsRegularMode() ? "1" : "0");
            writer.WriteString("0");
            writer.WriteFullEndElement();

            // 주간/야간 모드
            writer.WriteStartElement("Normal");
            //writer.WriteString(frm.GetPageDisaster().IsWeekMode() ? "1" : "0");
            writer.WriteString("1");
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

        private bool MakeBody(XmlTextWriter writer, SenarioManager manager)
        {
            writer.WriteStartElement("Body");   // Body 시작

            writer.WriteStartElement("ActionStepList"); // ActionStepList 시작

            ArrayList arActionStepList = manager.ActionStepList;
            foreach(UnE.SenarioMaker.ActionStep step in arActionStepList)
            {
                Sections.PanelSectionEx panel = step.Panel;
                if( panel != null)
                {
                    if (!MakeActionStep(writer, panel))
                        return false;
                }
            }          

            writer.WriteFullEndElement();       // ActionStepList 끝
            writer.WriteFullEndElement();       // Body 끝

            return true;
        }

        private bool MakeVariables(XmlTextWriter writer)
        {
            writer.WriteStartElement("Variables");

            IEnumerable<UserVariable> list =  SenarioManager.Instance.UserVariables.VarList;
            foreach(UserVariable var in list)
            {
                XMLWriter<UserVariable>.AddVariable(writer, var);
            }
            IEnumerable<Enums> list2 = SenarioManager.Instance.EnumList.VarList;
            foreach (Enums var in list2)
            {
                XMLWriter<Enums>.AddVariable(writer, var);
            }
            IEnumerable<Variable> list3 = SenarioManager.Instance.SystemVariables.VarList;
            foreach (Variable var in list3)
            {
                XMLWriter<Variable>.AddVariable(writer, var);
            }  
            writer.WriteFullEndElement(); //Variables 끝

            return true;
        }

        private bool MakeActionStep(XmlTextWriter writer, Sections.PanelSectionEx panel)
        {
            writer.WriteStartElement("ActionStep");     // ActionStep 시작

            UnE.SenarioMaker.ActionStep opt = (UnE.SenarioMaker.ActionStep)panel.Tag;
            if (opt == null)
                return false;
            //long nData = dicActionSteps[tabPage];

            long nData = opt.ID;
            long nParentID = nData >> 32;
            long nActionStepID = nData & 0xffffffff;

            writer.WriteStartAttribute("id");
            writer.WriteString(nActionStepID.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("selected");
            if( panel.CurrentSection == true)
                writer.WriteString("1");
            else
                writer.WriteString("0");
            writer.WriteEndAttribute();


            string strStepName = panel.StepName;
            writer.WriteStartElement("StepName");
            writer.WriteString(strStepName);
            writer.WriteFullEndElement();

         

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

            bool isSuccess = MakeStepMemberList(writer, panel);

            writer.WriteFullEndElement();       // ActionStep 끝
            return isSuccess;
        }

        private bool MakeStepMemberList(XmlTextWriter writer, Sections.PanelSectionEx panel)
        {
            writer.WriteStartElement("StepMemberList");     // StepMemberList 시작

            Type type = typeof(Sections.PanelSectionEx);
            int nStepMemberID = 1;

            // Arrow 및 Link 계산을 위한 Data
            // Section 별 StepMember ID(상위 4바이트) & Component ID(하위 4바이트)
            Dictionary<Sections.Section, long> dicSectionInfo = new Dictionary<Sections.Section, long>();
            
            if (panel.Collapse == false)
            {
                panel.CollapseAllGroup();
            }

            GetSectionInfo(panel, dicSectionInfo, nStepMemberID);
            if (!MakeStepMember(writer, panel, dicSectionInfo, nStepMemberID++))
                return false;

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
            writer.WriteString(panel.TeamType.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("name");
            writer.WriteString(panel.TeamName);
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

            writer.WriteStartElement("ComponentID");
            writer.WriteString(section.Data.ComponentID);
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
                case Sections.Section.ComponentType.DECISION:
                    isSuccess = MakeDecisionProperty(writer, (Sections.SectionDecision)section);
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

        private bool MakeTransSOPProperty(XmlTextWriter writer, Sections.SectionTransSOP section)
        {
            return true;
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

        private bool MakeDecisionProperty(XmlTextWriter writer, Sections.SectionDecision section)
        {
            Sections.SectionData data = (Sections.SectionData)section.Data;
            if (data.Expression != null && data.Expression != "")
            {
                writer.WriteStartElement("UserData");
                writer.WriteStartAttribute("name");
                writer.WriteString("expr");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString(data.Expression);
                writer.WriteEndAttribute();
                writer.WriteString(data.Expression);
                writer.WriteFullEndElement();
            }

            if (data.ShowExpression == true)
            {
                writer.WriteStartElement("UserData");
                writer.WriteStartAttribute("name");
                writer.WriteString("showExpr");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString(data.ShowExpression.ToString());
                writer.WriteEndAttribute();
                writer.WriteString(data.ShowExpression.ToString());
                writer.WriteFullEndElement();
            } 
            return true;
        }

        private bool MakeProcessProperty(XmlTextWriter writer, Sections.SectionProcess section)
        {
            //string strTeamList = IOManager.GetProcessTeamList(section);

            string strTeamList = "";

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

            if (data.Expression != null && data.Expression != "")
            {
                writer.WriteStartElement("UserData");
                writer.WriteStartAttribute("name");
                writer.WriteString("expr");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString(data.Expression);
                writer.WriteEndAttribute();
                writer.WriteString(data.Expression);
                writer.WriteFullEndElement();
            }

            //if( data.ShowExpression == true)
            {
                writer.WriteStartElement("UserData");
                writer.WriteStartAttribute("name");
                writer.WriteString("showExpr");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString(data.ShowExpression.ToString());
                writer.WriteEndAttribute();
                writer.WriteString(data.ShowExpression.ToString());
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
            if (data.LinkedSection == null && m_bCheckContent )
                return false;

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

      

        private bool MakeInternalProperty(XmlTextWriter writer, Sections.SectionInternal section)
        {
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

			if (strReceiver == "" && !m_bCheckContent)
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
				if(m_bCheckContent)
				{
                    writer.WriteString(data.SMSMessage);                   
				}
				else
				{
                    if (data.SMSMessage == "")
	                    writer.WriteString("null");
                    else
	                    writer.WriteString(data.SMSMessage);                    
				}
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
                //int stepMemberID = (int)(pair.Value >> 32);
                //if (stepMemberID != nStepMemberID)
                //    continue;

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

        public class ActionStep
        {
            // PeriodType : 기간 Type : 0(사용 안함), 1(날짜 옵션, n1월 n2일 ~ m1월 m2일까지), 2(시간 옵션, n1시 n2분 ~ m1월 m2일까지), 3(날짜 옵션 + 시간 옵션),
            //                                      11(고정 년도 사용 + 날짜 옵션), 12(고정 년도 사용 + 시간 옵션), 13(고정 년도 사용 + 날짜 옵션 + 시간 옵션)
            // WeekDayOption : 요일 옵션(bit 연산), bit : 1(일요일), 2(월요일), 4(화요일), 8(수요일), 16(목요일), 32(금요일), 64(토요일)
            // Iteration : 반복 회수
            // IterationType : 반복 회수 옵션 : 0(전체 기간중 몇회), 1(년중 몇회), 2(월중 몇회), 3(주중 몇회), 4(하루중 몇회), 5(시간당 몇회)
            // ProcessTimeType : 처리시간 옵션, 0(개월), 1(주), 2(일), 3(시간), 4(분)

            private int m_nID;
            private string m_strStepName;
            private int m_nPeriodType;
            private DateTime m_dtBeginTime;
            private DateTime m_dtEndTime;
            private int m_nWeekdayOption = 127;
            private int m_nIteration;
            private int m_nIterationType;
            private int m_nProcessTime;
            private int m_nProcessTimeType = 5;
            private int m_nDisasterID;
            private int m_nParentStepID = -1;

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }
            public string StepName
            {
                get { return m_strStepName; }
                set { m_strStepName = value; }
            }
            public int PeriodType
            {
                get { return m_nPeriodType; }
                set { m_nPeriodType = value; }
            }
            public DateTime BeginTime
            {
                get { return m_dtBeginTime; }
                set { m_dtBeginTime = value; }
            }
            public DateTime EndTime
            {
                get { return m_dtEndTime; }
                set { m_dtEndTime = value; }
            }
            public int WeekdayOption
            {
                get { return m_nWeekdayOption; }
                set { m_nWeekdayOption = value; }
            }
            public int Iteration
            {
                get { return m_nIteration; }
                set { m_nIteration = value; }
            }
            public int IterationType
            {
                get { return m_nIterationType; }
                set { m_nIterationType = value; }
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
            public int DisasterID
            {
                get { return m_nDisasterID; }
                set { m_nDisasterID = value; }
            }
            public int ParentStepID
            {
                get { return m_nParentStepID; }
                set { m_nParentStepID = value; }
            }

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

        class StepMember
        {
            private ArrayList m_arrComponent = new ArrayList();
            private ArrayList m_arrArrow = new ArrayList();
            private string m_strTeamName = "";

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

            protected Dictionary<string, object> m_arrUserData = new Dictionary<string, object>();
            public Dictionary<string, object> UserData
            {
                get { return m_arrUserData; }
                set { m_arrUserData = value; }
            }
            public void AddUserData(string szName, object ojb)
            {
                if (m_arrUserData.ContainsKey(szName))
                    return;

                m_arrUserData.Add(szName, ojb);
            }
        }

        class PropertyProcess : ComponentProperty
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

        class PropertyInternal : ComponentProperty
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

        class PropertyDecision : ComponentProperty
        {
        }
       
    }



}
