using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Text;

namespace SOPManager.BLL
{
    using Models.SOP;
    using Model.Sop.Category;
    using Model.Sop.Component;
    using Model.Sop.Config;
    using IDAL;

    public class XMLManager
    {
        private const string XMLVersion = "V1.0";

        public static string Save(IDataManager dataManager, SOPData sopData, Dictionary<ActionStepData, bool> dicActiveActionSteps, out string strXMLFileName, out string strErrorMessage)
        {
            strErrorMessage = null;
            strXMLFileName = "";

            try
            {
                using (var stream = new MemoryStream())
                {
                    using (var writer = new XmlTextWriter(stream, new UTF8Encoding(false)))
                    {
                        writer.Formatting = Formatting.Indented;

                        writer.WriteStartDocument();
                        WriteSopData(writer, dataManager, sopData, dicActiveActionSteps, ref strXMLFileName, ref strErrorMessage);
                        writer.WriteEndDocument();
                    }

                    if (strErrorMessage != null)
                        return null;

                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }
            catch (Exception e)
            {
                strErrorMessage = e.Message;
            }

            return null;
        }

        private static bool WriteSopData(XmlTextWriter writer, IDataManager dataManager, SOPData sopData, Dictionary<ActionStepData, bool> dicActiveActionSteps, ref string strXMLFileName, ref string strErrorMessage)
        {
            try
            {
                writer.WriteStartElement("SOP");

                if (WriteHeader(writer, sopData, ref strXMLFileName, ref strErrorMessage) == false)
                    return false;
                if (WriteBody(writer, dataManager, sopData, dicActiveActionSteps, ref strErrorMessage) == false)
                    return false;

                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private static bool WriteHeader(XmlTextWriter writer, SOPData sopData, ref string strXMLFileName, ref string strErrorMessage)
        {
            writer.WriteStartElement("Header");

            writer.WriteStartElement("XMLVersion");
            writer.WriteString(XMLVersion);
            writer.WriteFullEndElement();

            if (sopData.DisasterCategory == null || sopData.DisasterCategory.CategoryName.Length == 0)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Value("noDisasterCategory");
                return false;
            }

            writer.WriteStartElement("Category");
            writer.WriteString(sopData.DisasterCategory.CategoryName);
            writer.WriteFullEndElement();

            if (sopData.SubDisasterCategory == null || sopData.SubDisasterCategory.SubCategoryName.Length == 0)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Value("noSubDisasterCategory");
                return false;
            }

            writer.WriteStartElement("SubCategory");
            writer.WriteString(sopData.SubDisasterCategory.SubCategoryName);
            writer.WriteFullEndElement();

            if (sopData.Disaster == null || sopData.Disaster.DisasterName.Length == 0)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Value("noDisaster");
                return false;
            }

            writer.WriteStartElement("Disaster");
            writer.WriteString(sopData.Disaster.DisasterName);
            writer.WriteFullEndElement();

            strXMLFileName = sopData.Disaster.DisasterName + ".sop";

            if (sopData.Version == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Value("failSaveVersion");
                return false;
            }

            writer.WriteStartElement("Normal");
            writer.WriteString(sopData.Version.IsNormal ? "1" : "0");
            writer.WriteFullEndElement();

            writer.WriteStartElement("SOPVersion");

            if (sopData.Version.Description != null)
            {
                string strDescription = sopData.Version.Description.Trim();

                if (strDescription.Length > 0)
                {
                    writer.WriteStartAttribute("description");
                    writer.WriteString(strDescription);
                    writer.WriteEndAttribute();
                }
            }
            writer.WriteString(sopData.Version.VersionName);
            writer.WriteFullEndElement();

            writer.WriteFullEndElement();
            return true;
        }

        private static bool WriteBody(XmlTextWriter writer, IDataManager dataManager, SOPData sopData, Dictionary<ActionStepData, bool> dicActiveActionSteps, ref string strErrorMessage)
        {
            writer.WriteStartElement("Body");

            if (WriteActionStepList(writer, dataManager, sopData.ActionStepDatas, dicActiveActionSteps, ref strErrorMessage) == false)
                return false;

            writer.WriteFullEndElement();
            return true;
        }

        private static bool WriteActionStepList(XmlTextWriter writer, IDataManager dataManager, List<ActionStepData> actionStepDatas, Dictionary<ActionStepData, bool> dicActiveActionSteps, ref string strErrorMessage)
        {
            writer.WriteStartElement("ActionStepList");

            int nActionStepCount = actionStepDatas.Count;

            for (int i = 0; i < nActionStepCount; i++)
            {
                ActionStepData actionStepData = actionStepDatas[i];

                if (dicActiveActionSteps.ContainsKey(actionStepData) == false)
                    continue;

                int nOriginalID = actionStepData.ActionStep.ID;
                actionStepData.ActionStep.ID = i + 1;

                if (WriteActionStep(writer, dataManager, actionStepData, ref strErrorMessage) == false)
                {
                    actionStepData.ActionStep.ID = nOriginalID;
                    return false;
                }

                actionStepData.ActionStep.ID = nOriginalID;
            }

            writer.WriteFullEndElement();
            return true;
        }

        private static bool WriteActionStep(XmlTextWriter writer, IDataManager dataManager, ActionStepData actionStepData, ref string strErrorMessage)
        {
            ActionStep actionStep = actionStepData.ActionStep;

            writer.WriteStartElement("ActionStep");

            writer.WriteStartAttribute("id");
            writer.WriteString(actionStep.ID.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartElement("StepName");
            writer.WriteString(actionStep.StepName);
            writer.WriteFullEndElement();

            if (actionStep.UserDefinedConfigID != null)
            {
                writer.WriteStartElement("UserDefinedConfigID");
                writer.WriteString(((int)actionStep.UserDefinedConfigID).ToString());
                writer.WriteFullEndElement();
            }

            if (WriteStepMemberList(writer, dataManager, actionStepData.StepMemberDatas, ref strErrorMessage) == false)
                return false;

            writer.WriteFullEndElement();
            return true;
        }

        private static bool WriteStepMemberList(XmlTextWriter writer, IDataManager dataManager, List<StepMemberData> stepMemberDatas, ref string strErrorMessage)
        {
            writer.WriteStartElement("StepMemberList");

            int nStepMemberCount = stepMemberDatas.Count;

            for (int i = 0; i < nStepMemberCount; i++)
            {
                StepMemberData stepMemberData = stepMemberDatas[i];

                int nOriginalID = stepMemberData.StepMember.ID;
                stepMemberData.StepMember.ID = i + 1;

                if (WriteStepMember(writer, dataManager, stepMemberData, ref strErrorMessage) == false)
                {
                    stepMemberData.StepMember.ID = nOriginalID;
                    return false;
                }

                stepMemberData.StepMember.ID = nOriginalID;
            }

            writer.WriteFullEndElement();
            return true;
        }

        private static bool WriteStepMember(XmlTextWriter writer, IDataManager dataManager, StepMemberData stepMemberData, ref string strErrorMessage)
        {
            StepMember stepMember = stepMemberData.StepMember;
            writer.WriteStartElement("StepMember");

            writer.WriteStartAttribute("id");
            writer.WriteString(stepMember.ID.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("teamType");
            writer.WriteString(stepMember.TeamType.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("name");
            writer.WriteString(stepMemberData.StepMemberName);
            writer.WriteEndAttribute();

            if (WriteGrid(writer, stepMemberData, ref strErrorMessage) == false)
                return false;

            // Key : 상위 4바이트(ColumnIndex), 하위 4바이트(RowIndex)
            Dictionary<long, int> dicSectionIDs = new Dictionary<long, int>();

            if (WriteComponentList(writer, dataManager, stepMemberData.Sections, dicSectionIDs, ref strErrorMessage) == false)
                return false;

            if (WriteArrowList(writer, stepMemberData.Arrows, dicSectionIDs, ref strErrorMessage) == false)
                return false;

            writer.WriteFullEndElement();
            return true;
        }

        private static bool WriteGrid(XmlTextWriter writer, StepMemberData stepMemberData, ref string strErrorMessage)
        {
            writer.WriteStartElement("Grid");

            writer.WriteStartElement("Columns");
            int nColumnCount = stepMemberData.GridColumnWidth.Count;

            for (int i = 0; i < nColumnCount; i++)
            {
                int nColumnWidth = stepMemberData.GridColumnWidth[i];

                writer.WriteStartElement("Column");

                writer.WriteStartAttribute("index");
                writer.WriteString(i.ToString());
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("width");
                writer.WriteString(nColumnWidth.ToString());
                writer.WriteEndAttribute();

                writer.WriteFullEndElement();
            }

            writer.WriteFullEndElement();

            writer.WriteStartElement("Rows");
            int nRowCount = stepMemberData.GridRowHeight.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                int nRowHeight = stepMemberData.GridRowHeight[i];

                writer.WriteStartElement("Row");

                writer.WriteStartAttribute("index");
                writer.WriteString(i.ToString());
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("height");
                writer.WriteString(nRowHeight.ToString());
                writer.WriteEndAttribute();

                writer.WriteFullEndElement();
            }

            writer.WriteFullEndElement();

            writer.WriteFullEndElement();
            return true;
        }

        // dicSectionIDs.Key : 상위 4바이트(ColumnIndex), 하위 4바이트(RowIndex)
        private static bool WriteComponentList(XmlTextWriter writer, IDataManager dataManager, List<SectionData> sections, Dictionary<long, int> dicSectionIDs, ref string strErrorMessage)
        {
            List<ExternalProgram> externalPrograms = dataManager.GetSelectManager().SelectExternalPrograms("", out strErrorMessage);

            if (externalPrograms == null)
                return false;

            Dictionary<int, ExternalProgram> dicExternalPrograms = new Dictionary<int, ExternalProgram>();

            foreach (ExternalProgram program in externalPrograms)
            {
                dicExternalPrograms[program.ID] = program;
            }

            writer.WriteStartElement("ComponentList");
            int nSectionCount = sections.Count;

            for (int i = 0; i < nSectionCount; i++)
            {
                if (WriteComponent(writer, dicExternalPrograms, sections[i], i, dicSectionIDs, ref strErrorMessage) == false)
                    return false;
            }

            writer.WriteFullEndElement();
            return true;
        }

        // dicSectionIDs.Key : 상위 4바이트(ColumnIndex), 하위 4바이트(RowIndex)
        private static bool WriteComponent(XmlTextWriter writer, Dictionary<int, ExternalProgram> dicExternalPrograms, SectionData sectionData, int id, Dictionary<long, int> dicSectionIDs, ref string strErrorMessage)
        {
            writer.WriteStartElement("Component");

            writer.WriteStartAttribute("id");
            writer.WriteString(id.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartElement("ColumnIndex");
            writer.WriteString(sectionData.GridColumnIndex.ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("RowIndex");
            writer.WriteString(sectionData.GridRowIndex.ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("Width");
            writer.WriteString(GetFloatString(sectionData.Width));
            writer.WriteFullEndElement();

            writer.WriteStartElement("Height");
            writer.WriteString(GetFloatString(sectionData.Height));
            writer.WriteFullEndElement();

            writer.WriteStartElement("Text");
            writer.WriteString(sectionData.Text);
            writer.WriteFullEndElement();

            writer.WriteStartElement("ComponentID");
            writer.WriteString(sectionData.ComponentID);
            writer.WriteFullEndElement();

            if (WriteComponentProperty(writer, dicExternalPrograms, sectionData, ref strErrorMessage) == false)
                return false;

            long key = ((((long)sectionData.GridColumnIndex) << 32) | (long)sectionData.GridRowIndex);
            dicSectionIDs[key] = id;

            writer.WriteFullEndElement();
            return true;
        }

        private static bool WriteComponentProperty(XmlTextWriter writer, Dictionary<int, ExternalProgram> dicExternalPrograms, SectionData sectionData, ref string strErrorMessage)
        {
            writer.WriteStartElement("Property");

            writer.WriteStartAttribute("type");
            writer.WriteString(sectionData.ComponentType.ToString());
            writer.WriteEndAttribute();

            if (sectionData.ComponentType == (int)Section.SectionType.Process)
            {
                if (WriteProcessProperty(writer, dicExternalPrograms, sectionData, ref strErrorMessage) == false)
                    return false;
            }
            else if (sectionData.ComponentType == (int)Section.SectionType.Annotation)
            {
                if (WriteAnnotationProperty(writer, sectionData, ref strErrorMessage) == false)
                    return false;
            }
            else if (sectionData.ComponentType == (int)Section.SectionType.Decision)
            {
                if (WriteDecisionProperty(writer, sectionData, ref strErrorMessage) == false)
                    return false;
            }
            else if (sectionData.ComponentType == (int)Section.SectionType.Endpoint)
            {
                if (WriteEndpointProperty(writer, sectionData, ref strErrorMessage) == false)
                    return false;
            }
            else if (sectionData.ComponentType == (int)Section.SectionType.Internal)
            {
                if (WriteInternalProperty(writer, sectionData, ref strErrorMessage) == false)
                    return false;
            }

            writer.WriteFullEndElement();
            return true;
        }

        private static bool WriteProcessProperty(XmlTextWriter writer, Dictionary<int, ExternalProgram> dicExternalPrograms, SectionData sectionData, ref string strErrorMessage)
        {
            string strTeamList = SOPManager.DAL.CreateManager.MakeTeamListString(sectionData.Receivers);

            writer.WriteStartElement("TeamList");
            writer.WriteString(strTeamList);
            writer.WriteFullEndElement();

            if (sectionData.OnlyTeamLeader != null)
            {
                writer.WriteStartElement("OnlyTeamLeader");
                writer.WriteString(((bool)sectionData.OnlyTeamLeader).ToString());
                writer.WriteFullEndElement();
            }

            if (sectionData.AutoRun == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Value("noAutoRunInProcess");
                return false;
            }

            writer.WriteStartElement("AutoRun");
            writer.WriteString(((bool)sectionData.AutoRun).ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("MissionList");

            if (sectionData.Missions != null)
            {
                foreach (ProcessMissionData data in sectionData.Missions)
                {
                    if (data.MissionType == ProcessMissionData.MissionDataType.Normal)
                    {
                        ProcessMission mission = ProcessMissionDataSorter.ToMission(data);

                        writer.WriteStartElement("Mission");
                        writer.WriteString(mission.MissionText);
                        writer.WriteFullEndElement();
                    }
                    else if (data.MissionType == ProcessMissionData.MissionDataType.External)
                    {
                        ProcessExternalMission externalMission = ProcessMissionDataSorter.ToExternalMission(data);

                        ExternalProgram program;

                        if (dicExternalPrograms.TryGetValue(externalMission.ProgramID, out program) == false)
                        {
                            strErrorMessage = string.Format("존재하지 않는 외부 프로그램 ID({0})입니다.", externalMission.ProgramID);
                            return false;
                        }

                        writer.WriteStartElement("MissionExternal");

                        writer.WriteStartElement("Program");

                        writer.WriteStartAttribute("id");
                        writer.WriteString(externalMission.ProgramID.ToString());
                        writer.WriteEndAttribute();

                        writer.WriteStartElement("Exe");
                        writer.WriteString(program.ExeName);
                        writer.WriteFullEndElement();

                        if (program.InstallPath != null)
                        {
                            writer.WriteStartElement("InstallPath");
                            writer.WriteString(program.InstallPath);
                            writer.WriteFullEndElement();
                        }

                        writer.WriteStartElement("Description");
                        writer.WriteString(program.Description);
                        writer.WriteFullEndElement();

                        // Program
                        writer.WriteFullEndElement();

                        writer.WriteStartElement("Parameters");

                        int nParameterCount = data.Parameters.Count;

                        // 첫번째 Parameter는 무시한다.
                        for (int i = 1; i < nParameterCount; i++)
                        {
                            string strParameter = data.Parameters[i];

                            writer.WriteStartElement("Param");
                            writer.WriteString(strParameter);
                            writer.WriteFullEndElement();
                        }

                        // Parameters
                        writer.WriteFullEndElement();

                        // MissionExternal
                        writer.WriteFullEndElement();
                    }
                }
            }

            // MissionList
            writer.WriteFullEndElement();

            return true;
        }

        private static bool WriteAnnotationProperty(XmlTextWriter writer, SectionData sectionData, ref string strErrorMessage)
        {
            return true;
        }

        private static bool WriteDecisionProperty(XmlTextWriter writer, SectionData sectionData, ref string strErrorMessage)
        {
            if (sectionData.TeamType != null)
            {
                writer.WriteStartElement("TeamType");
                writer.WriteString(((int)sectionData.TeamType).ToString());
                writer.WriteFullEndElement();

                writer.WriteStartElement("TeamName");
                writer.WriteString(sectionData.TeamName);
                writer.WriteFullEndElement();
            }

            if (sectionData.AutoRunScript != null)
            {
                writer.WriteStartElement("AutoRunScript");
                writer.WriteString(sectionData.AutoRunScript);
                writer.WriteFullEndElement();
            }

            if (sectionData.AutoRunScriptVariableTypes != null)
            {
                writer.WriteStartElement("AutoRunScriptVariableTypes");
                writer.WriteString(sectionData.AutoRunScriptVariableTypes);
                writer.WriteFullEndElement();
            }

            return true;
        }

        private static bool WriteEndpointProperty(XmlTextWriter writer, SectionData sectionData, ref string strErrorMessage)
        {
            if (sectionData.IsBegin == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Value("noIsBeginInEndpoint");
                return false;
            }

            writer.WriteStartElement("IsBegin");
            writer.WriteString(((bool)sectionData.IsBegin).ToString());
            writer.WriteFullEndElement();

            return true;
        }

        private static bool WriteInternalProperty(XmlTextWriter writer, SectionData sectionData, ref string strErrorMessage)
        {
            if (sectionData.IsSMS == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Value("noUseSMSInInternal");
                return false;
            }

            if (sectionData.IsBroadcast == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Value("noUseBroadcastInInternal");
                return false;
            }

            writer.WriteStartElement("UseSMS");
            writer.WriteString(((bool)sectionData.IsSMS).ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("UseBroadcast");
            writer.WriteString(((bool)sectionData.IsBroadcast).ToString());
            writer.WriteFullEndElement();

            if (sectionData.IsEmail != null)
            {
                writer.WriteStartElement("UseEmail");
                writer.WriteString(((bool)sectionData.IsEmail).ToString());
                writer.WriteFullEndElement();
            }

            if (sectionData.Message == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Value("noMessageInInternal");
                return false;
            }
            else
            {
                writer.WriteStartElement("Message");
                writer.WriteString(sectionData.Message);
                writer.WriteFullEndElement();
            }

            string strTeamList = SOPManager.DAL.CreateManager.MakeTeamListString(sectionData.Receivers);

            /*foreach (string strReceiver in sectionData.Receivers)
            {
                if (strTeamList.Length == 0)
                    strTeamList = strReceiver;
                else
                    strTeamList += "\t" + strReceiver;
            }*/

            writer.WriteStartElement("TeamList");
            writer.WriteString(strTeamList);
            writer.WriteFullEndElement();

            if (sectionData.OnlyTeamLeader != null)
            {
                writer.WriteStartElement("OnlyTeamLeader");
                writer.WriteString(((bool)sectionData.OnlyTeamLeader).ToString());
                writer.WriteFullEndElement();
            }

            if (sectionData.AutoRun == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Value("noAutoRunInInternal");
                return false;
            }

            writer.WriteStartElement("AutoRun");
            writer.WriteString(((bool)sectionData.AutoRun).ToString());
            writer.WriteFullEndElement();

            if (sectionData.UseSiren != null)
            {
                writer.WriteStartElement("UseSiren");
                writer.WriteString(((bool)sectionData.UseSiren).ToString());
                writer.WriteFullEndElement();
            }

            return true;
        }

        // dicSectionIDs.Key : 상위 4바이트(ColumnIndex), 하위 4바이트(RowIndex)
        private static bool WriteArrowList(XmlTextWriter writer, List<ArrowData> arrows, Dictionary<long, int> dicSectionIDs, ref string strErrorMessage)
        {
            writer.WriteStartElement("ArrowList");
            int nArrowCount = arrows.Count;

            for (int i = 0; i < nArrowCount; i++)
            {
                if (WriteArrow(writer, arrows[i], dicSectionIDs, ref strErrorMessage) == false)
                    return false;
            }

            writer.WriteFullEndElement();
            return true;
        }

        // dicSectionIDs.Key : 상위 4바이트(ColumnIndex), 하위 4바이트(RowIndex)
        private static bool WriteArrow(XmlTextWriter writer, ArrowData arrow, Dictionary<long, int> dicSectionIDs, ref string strErrorMessage)
        {
            writer.WriteStartElement("Arrow");

            if (arrow.Text != null)
            {
                string strText = arrow.Text.Trim();

                if (strText.Length > 0)
                {
                    writer.WriteStartElement("Text");
                    writer.WriteString(strText);
                    writer.WriteFullEndElement();
                }
            }

            int nBeginComponentID, nEndComponentID;
            long beginKey = ((((long)arrow.BeginComponentColumnIndex) << 32) | (long)arrow.BeginComponentRowIndex);

            if (dicSectionIDs.TryGetValue(beginKey, out nBeginComponentID) == false)
                return false;

            long endKey = ((((long)arrow.EndComponentColumnIndex) << 32) | (long)arrow.EndComponentRowIndex);

            if (dicSectionIDs.TryGetValue(endKey, out nEndComponentID) == false)
                return false;

            writer.WriteStartElement("BeginComponentID");
            writer.WriteString(nBeginComponentID.ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("BeginComponentPosition");
            writer.WriteString(arrow.BeginComponentPosition.ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("EndComponentID");
            writer.WriteString(nEndComponentID.ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("EndComponentPosition");
            writer.WriteString(arrow.EndComponentPosition.ToString());
            writer.WriteFullEndElement();

            writer.WriteFullEndElement();
            return true;
        }

        private static string GetFloatString(float data)
        {
            return string.Format("{0:F1}", data);
        }

        public static SOPData OpenXML(string strXML, int nSiteID, out string strErrorMessage)
        {
            XElement xml = XElement.Parse(strXML);

            XElement xSOP = xml.Name == "SOP" ? xml : null;

            if (xSOP == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noSOPTagInXML");
                return null;
            }

            return ReadSOP(xSOP, nSiteID, out strErrorMessage);
        }

        private static SOPData ReadSOP(XElement xSOP, int nSiteID, out string strErrorMessage)
        {
            XElement xHeader = FindElement(xSOP, "Header");
            XElement xBody = FindElement(xSOP, "Body");

            if (xHeader == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noHeaderTagInXML");
                return null;
            }

            if (xBody == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noBodyTagInXML");
                return null;
            }

            DisasterCategory dc;
            SubDisasterCategory sdc;
            Disaster disaster;
            Version version;

            if (ReadHeader(xHeader, nSiteID, out dc, out sdc, out disaster, out version, out strErrorMessage) == false)
                return null;

            List<ActionStepData> actionStepDatas = ReadBody(xBody, nSiteID, out strErrorMessage);

            if (actionStepDatas == null)
                return null;

            SOPData sopData = new SOPData();

            sopData.DisasterCategory = dc;
            sopData.SubDisasterCategory = sdc;
            sopData.Disaster = disaster;
            sopData.ActionStepDatas.AddRange(actionStepDatas);
            sopData.Version = version;

            return sopData;
        }

        private static bool ReadHeader(XElement xHeader, int nSiteID, out DisasterCategory dc, out SubDisasterCategory sdc, out Disaster disaster, out Version version, out string strErrorMessage)
        {
            dc = null;
            sdc = null;
            disaster = null;
            version = null;
            strErrorMessage = null;

            XElement xXMLVersion = FindElement(xHeader, "XMLVersion");
            XElement xCategory = FindElement(xHeader, "Category");
            XElement xSubCategory = FindElement(xHeader, "SubCategory");
            XElement xDisaster = FindElement(xHeader, "Disaster");
            XElement xNormal = FindElement(xHeader, "Normal");
            XElement xSopVersion = FindElement(xHeader, "SOPVersion");

            if (xXMLVersion == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noXMLVersionTagInHeader");
                return false;
            }

            if (xCategory == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noCategoryTagInHeader");
                return false;
            }

            if (xSubCategory == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noSubCategoryTagInHeader");
                return false;
            }

            if (xDisaster == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noDisasterTagInHeader");
                return false;
            }

            if (xNormal == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noNormalTagInHeader");
                return false;
            }

            if (xSopVersion == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noSOPVersionTagInHeader");
                return false;
            }

            XAttribute attrDescription = FindAttribute(xSopVersion, "description");

            dc = new DisasterCategory();
            dc.CategoryName = xCategory.Value;
            dc.SiteID = nSiteID;

            sdc = new SubDisasterCategory();
            sdc.SubCategoryName = xSubCategory.Value;

            disaster = new Disaster();
            disaster.DisasterName = xDisaster.Value;

            bool? isNormal = GetBoolean(xNormal.Value);

            if (isNormal == null)
            {
                strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("onlyBooleanTag1"), xNormal.Name, xNormal.Value);
                return false;
            }

            version = new Version();
            version.CreateTime = DateTime.Now;
            version.IsNormal = (bool)isNormal;
            version.LastAccessTime = version.CreateTime;
            version.SiteID = nSiteID;
            version.VersionName = xSopVersion.Value;

            if (attrDescription != null)
                version.Description = attrDescription.Value;

            return true;
        }

        private static List<ActionStepData> ReadBody(XElement xHeader, int nSiteID, out string strErrorMessage)
        {
            strErrorMessage = null;

            XElement xActionStepList = FindElement(xHeader, "ActionStepList");
            
            if (xActionStepList == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noActionStepListTagInBody");
                return null;
            }

            List<ActionStepData> actionStepDatas = new List<ActionStepData>();

            foreach (XElement element in xActionStepList.Elements())
            {
                if (element.Name == "ActionStep")
                {
                    ActionStepData actionStepData = ReadActionStep(element, nSiteID, out strErrorMessage);

                    if (actionStepData == null)
                        return null;
                    else
                    {
                        if (IsValidActionStep(actionStepData))
                            actionStepDatas.Add(actionStepData);
                    }
                }
            }

            if (actionStepDatas.Count == 0)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Value("noValidActionStep");
                return null;
            }

            return actionStepDatas;
        }

        private static bool IsValidActionStep(ActionStepData actionStepData)
        {
            if (actionStepData.StepMemberDatas.Count == 0)
                return false;

            bool begin = false, end = false;

            foreach (StepMemberData stepMemberData in actionStepData.StepMemberDatas)
            {
                foreach (SectionData sectionData in stepMemberData.Sections)
                {
                    if (sectionData.ComponentType == (int)Section.SectionType.Endpoint)
                    {
                        if (sectionData.IsBegin != null)
                        {
                            if ((bool)sectionData.IsBegin)
                                begin = true;
                            else
                                end = true;

                            if (begin && end)
                                break;
                        }
                    }
                }

                if (begin && end)
                    break;
            }

            return begin && end;
        }

        private static ActionStepData ReadActionStep(XElement xActionStep, int nSiteID, out string strErrorMessage)
        {
            strErrorMessage = null;

            ActionStepData actionStepData = new ActionStepData();
            ActionStep actionStep = new ActionStep();
            actionStepData.ActionStep = actionStep;

            foreach (XElement element in xActionStep.Elements())
            {
                if (element.Name == "StepName")
                {
                    actionStep.StepName = element.Value;
                    actionStepData.StepName = element.Value;
                }
                else if (element.Name == "UserDefinedConfigID")
                {
                    // 나중에 구현
                }
                else if (element.Name == "StepMemberList")
                {
                    if (ReadStepMemberList(element, actionStepData.StepMemberDatas, nSiteID, out strErrorMessage) == false)
                        return null;
                }
            }

            return actionStepData;
        }

        private static bool ReadStepMemberList(XElement xStepMemberList, List<StepMemberData> stepMemberDatas, int nSiteID, out string strErrorMessage)
        {
            strErrorMessage = null;
            
            foreach (XElement element in xStepMemberList.Elements())
            {
                if (element.Name == "StepMember")
                {
                    StepMemberData stepMemberData = ReadStepMember(element, nSiteID, out strErrorMessage);

                    if (stepMemberData == null)
                        return false;
                    else
                    {
                        if (stepMemberData.GridColumnWidth.Count > 0 &&
                            stepMemberData.GridRowHeight.Count > 0 &&
                            stepMemberData.Sections.Count > 0)
                            stepMemberDatas.Add(stepMemberData);
                    }
                }
            }

            return true;
        }

        private static StepMemberData ReadStepMember(XElement xStepMember, int nSiteID, out string strErrorMessage)
        {
            strErrorMessage = null;

            StepMemberData stepMemberData = new StepMemberData();
            stepMemberData.StepMember = new StepMember();

            int? teamType = null;
            string strTeamName = null;

            foreach (XAttribute attr in xStepMember.Attributes())
            {
                if (attr.Name == "teamType")
                {
                    int nTeamType;

                    if (int.TryParse(attr.Value.Trim(), out nTeamType))
                        teamType = nTeamType;
                    else
                    {
                        strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("onlyIntegerInTeamType"), attr.Value);
                        return null;
                    }
                }
                else if (attr.Name == "name")
                {
                    strTeamName = attr.Value;
                }
            }

            if (teamType == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noTeamTypeAttrInStepMember");
                return null;
            }

            if (strTeamName == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noNameAttrInStepMember");
                return null;
            }

            stepMemberData.StepMember.TeamType = (int)teamType;
            stepMemberData.StepMemberName = strTeamName;

            Dictionary<int, SectionData> dicSections = new Dictionary<int, SectionData>();

            foreach (XElement element in xStepMember.Elements())
            {
                if (element.Name == "Grid")
                {
                    if (ReadGrid(element, stepMemberData.GridColumnWidth, stepMemberData.GridRowHeight, out strErrorMessage) == false)
                        return null;
                }
                else if (element.Name == "ComponentList")
                {
                    if (ReadSections(element, stepMemberData.Sections, dicSections, stepMemberData.GridColumnWidth, stepMemberData.GridRowHeight, out strErrorMessage) == false)
                        return null;
                }
                else if (element.Name == "ArrowList")
                {
                    if (ReadArrows(element, stepMemberData.Arrows, dicSections, out strErrorMessage) == false)
                        return null;
                }
            }

            return stepMemberData;
        }

        private static bool ReadGrid(XElement xGrid, List<int> gridColumnWidth, List<int> gridRowHeight, out string strErrorMessage)
        {
            strErrorMessage = null;

            foreach (XElement element in xGrid.Elements())
            {
                if (element.Name == "Columns")
                {
                    if (ReadColumn(element, gridColumnWidth, out strErrorMessage) == false)
                        return false;
                }
                else if (element.Name == "Rows")
                {
                    if (ReadRow(element, gridRowHeight, out strErrorMessage) == false)
                        return false;
                }
            }

            return true;
        }

        private static bool ReadColumn(XElement xColumns, List<int> gridColumnWidth, out string strErrorMessage)
        {
            strErrorMessage = null;
            int data;

            foreach (XElement element in xColumns.Elements())
            {
                if (element.Name == "Column")
                {
                    foreach (XAttribute attr in element.Attributes())
                    {
                        if (attr.Name == "width")
                        {
                            if (int.TryParse(attr.Value.Trim(), out data) == false)
                            {
                                strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("onlyIntegerInWidth"), attr.Value);
                                return false;
                            }
                            else
                                gridColumnWidth.Add(data);
                        }
                    }
                }
            }

            return true;
        }

        private static bool ReadRow(XElement xRows, List<int> gridRowHeight, out string strErrorMessage)
        {
            strErrorMessage = null;
            int data;

            foreach (XElement element in xRows.Elements())
            {
                if (element.Name == "Row")
                {
                    foreach (XAttribute attr in element.Attributes())
                    {
                        if (attr.Name == "height")
                        {
                            if (int.TryParse(attr.Value.Trim(), out data) == false)
                            {
                                strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("onlyIntegerInHeight"), attr.Value);
                                return false;
                            }
                            else
                                gridRowHeight.Add(data);
                        }
                    }
                }
            }

            return true;
        }

        // dicSectionData.Key : SectionData ID
        private static bool ReadSections(XElement xSections, List<SectionData> sectionDatas, Dictionary<int, SectionData> dicSections, List<int> gridColumnWidth, List<int> gridRowHeight, out string strErrorMessage)
        {
            strErrorMessage = null;

            foreach (XElement element in xSections.Elements())
            {
                if (element.Name == "Component")
                {
                    SectionData sectionData = ReadSection(element, gridColumnWidth, gridRowHeight, out strErrorMessage);

                    if (sectionData == null)
                        return false;
                    else
                    {
                        sectionDatas.Add(sectionData);
                        dicSections[sectionData.ID] = sectionData;
                    }
                }
            }

            return true;
        }

        private static SectionData ReadSection(XElement xSection, List<int> gridColumnWidth, List<int> gridRowHeight, out string strErrorMessage)
        {
            strErrorMessage = null;

            int nColumnCount = gridColumnWidth.Count;
            int nRowCount = gridRowHeight.Count;

            SectionData sectionData = new SectionData();
            bool readID = false;

            foreach (XAttribute attr in xSection.Attributes())
            {
                if (attr.Name == "id")
                {
                    int? id = GetInteger(attr.Value.Trim());

                    if (id == null)
                    {
                        strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("onlyIntegerAttr2"), attr.Name, attr.Value);
                        return null;
                    }
                    else
                    {
                        sectionData.ID = (int)id;
                        readID = true;
                    }
                }
            }

            if (readID == false)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noIDAttrInComponent");
                return null;
            }

            foreach (XElement element in xSection.Elements())
            {
                if (element.Name == "ColumnIndex")
                {
                    int? columnIndex = GetInteger(element.Value.Trim());

                    if (columnIndex == null)
                    {
                        strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("onlyIntegerTag2"), element.Name, element.Value);
                        return null;
                    }

                    if (columnIndex < 0 || columnIndex >= nColumnCount)
                    {
                        strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("noDefinedTag"), element.Name, element.Value);
                        return null;
                    }

                    sectionData.GridColumnIndex = (int)columnIndex;
                }
                else if (element.Name == "RowIndex")
                {
                    int? rowIndex = GetInteger(element.Value.Trim());

                    if (rowIndex == null)
                    {
                        strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("onlyIntegerTag2"), element.Name, element.Value);
                        return null;
                    }

                    if (rowIndex < 0 || rowIndex >= nRowCount)
                    {
                        strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("noDefinedTag"), element.Name, element.Value);
                        return null;
                    }

                    sectionData.GridRowIndex = (int)rowIndex;
                }
                else if (element.Name == "Width")
                {
                    float? width = GetFloat(element.Value.Trim());

                    if (width == null)
                    {
                        strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("onlyFloatTag2"), element.Name, element.Value);
                        return null;
                    }

                    sectionData.Width = (float)width;
                }
                else if (element.Name == "Height")
                {
                    float? height = GetFloat(element.Value.Trim());

                    if (height == null)
                    {
                        strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("onlyFloatTag2"), element.Name, element.Value);
                        return null;
                    }

                    sectionData.Height = (float)height;
                }
                else if (element.Name == "Text")
                {
                    sectionData.Text = element.Value;
                }
                else if (element.Name == "ComponentID")
                {
                    sectionData.ComponentID = element.Value;
                }
                else if (element.Name == "Property")
                {
                    if (ReadSectionProperty(element, sectionData, out strErrorMessage) == false)
                        return null;
                }
            }

            return sectionData;
        }

        private static bool ReadSectionProperty(XElement xProperty, SectionData sectionData, out string strErrorMessage)
        {
            strErrorMessage = null;

            int? type = null;

            foreach (XAttribute attr in xProperty.Attributes())
            {
                if (attr.Name == "type")
                {
                    type = GetInteger(attr.Value.Trim());

                    if (type == null)
                    {
                        strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("onlyIntegerAttr1"), attr.Name, attr.Value);
                        return false;
                    }
                }
            }

            if (type == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noTypeAttrInProperty");
                return false;
            }
            else
                sectionData.ComponentType = (int)type;

            if (sectionData.ComponentType == (int)Section.SectionType.Process)
            {
                if (ReadProcessProperty(xProperty, sectionData, out strErrorMessage) == false)
                    return false;
            }
            else if (sectionData.ComponentType == (int)Section.SectionType.Annotation)
            {
                if (ReadAnnotationProperty(xProperty, sectionData, out strErrorMessage) == false)
                    return false;
            }
            else if (sectionData.ComponentType == (int)Section.SectionType.Decision)
            {
                if (ReadDecisionProperty(xProperty, sectionData, out strErrorMessage) == false)
                    return false;
            }
            else if (sectionData.ComponentType == (int)Section.SectionType.Endpoint)
            {
                if (ReadEndpointProperty(xProperty, sectionData, out strErrorMessage) == false)
                    return false;
            }
            else if (sectionData.ComponentType == (int)Section.SectionType.Internal)
            {
                if (ReadInternalProperty(xProperty, sectionData, out strErrorMessage) == false)
                    return false;
            }

            return true;
        }

        private static bool ReadInternalProperty(XElement xProperty, SectionData sectionData, out string strErrorMessage)
        {
            strErrorMessage = null;
            bool readTeamList = false, readAutoRun = false;

            foreach (XElement element in xProperty.Elements())
            {
                if (element.Name == "UseSMS")
                {
                    bool? useSMS = GetBoolean(element.Value.Trim());

                    if (useSMS == null)
                    {
                        strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("onlyBooleanTag2"), element.Name, element.Value);
                        return false;
                    }

                    sectionData.IsSMS = useSMS;
                }
                else if (element.Name == "UseBroadcast")
                {
                    bool? useBroadcast = GetBoolean(element.Value.Trim());

                    if (useBroadcast == null)
                    {
                        strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("onlyBooleanTag2"), element.Name, element.Value);
                        return false;
                    }

                    sectionData.IsBroadcast = useBroadcast;
                }
                else if (element.Name == "UseEmail")
                {
                    bool? useEmail = GetBoolean(element.Value.Trim());

                    if (useEmail == null)
                    {
                        strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("onlyBooleanTag1"), element.Name, element.Value);
                        return false;
                    }

                    sectionData.IsEmail = useEmail;
                }
                else if (element.Name == "Message")
                {
                    sectionData.Message = element.Value;
                }
                else if (element.Name == "TeamList")
                {
                    sectionData.Receivers = ReadTeamList(element, out strErrorMessage);
                    readTeamList = true;
                }
                else if (element.Name == "AutoRun")
                {
                    bool? autoRun = GetBoolean(element.Value.Trim());

                    if (autoRun == null)
                    {
                        strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("onlyBooleanTag2"), element.Name, element.Value);
                        return false;
                    }

                    sectionData.AutoRun = (bool)autoRun;
                    readAutoRun = true;
                }
                else if (element.Name == "UseSiren")
                {
                    bool? useSiren = GetBoolean(element.Value.Trim());

                    if (useSiren == null)
                    {
                        strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("onlyBooleanTag1"), element.Name, element.Value);
                        return false;
                    }

                    sectionData.UseSiren = useSiren;
                }
            }

            if (sectionData.IsSMS == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noIsBeginTagInEndpointProperty");
                return false;
            }

            if (sectionData.IsBroadcast == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noUseBroadcastTagInInternalProperty");
                return false;
            }

            if (readTeamList == false)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noTeamListTagInInternalProperty");
                return false;
            }

            if (readAutoRun == false)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noAutoRunTagInInternalProperty");
                return false;
            }

            return true;
        }

        private static bool ReadEndpointProperty(XElement xProperty, SectionData sectionData, out string strErrorMessage)
        {
            strErrorMessage = null;

            foreach (XElement element in xProperty.Elements())
            {
                if (element.Name == "IsBegin")
                {
                    bool? isBegin = GetBoolean(element.Value.Trim());

                    if (isBegin == null)
                    {
                        strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("onlyBooleanTag1"), element.Name, element.Value);
                        return false;
                    }

                    sectionData.IsBegin = isBegin;
                }
            }

            if (sectionData.IsBegin == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noIsBeginTagInEndpointProperty");
                return false;
            }

            return true;
        }

        private static bool ReadDecisionProperty(XElement xProperty, SectionData sectionData, out string strErrorMessage)
        {
            strErrorMessage = null;
            
            foreach (XElement element in xProperty.Elements())
            {
                if (element.Name == "TeamType")
                {
                    int? teamType = GetInteger(element.Value.Trim());

                    if (teamType == null)
                    {
                        strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("onlyIntegerTag1"), element.Name, element.Value);
                        return false;
                    }

                    sectionData.TeamType = teamType;
                }
                else if (element.Name == "TeamName")
                {
                    sectionData.TeamName = element.Value;
                }
                else if (element.Name == "AutoRunScript")
                {
                    sectionData.AutoRunScript = element.Value;
                }
                else if (element.Name == "AutoRunScriptVariableTypes")
                {
                    sectionData.AutoRunScriptVariableTypes = element.Value;
                }
            }

            return true;
        }

        private static bool ReadAnnotationProperty(XElement xProperty, SectionData sectionData, out string strErrorMessage)
        {
            strErrorMessage = null;
            return true;
        }

        private static bool ReadProcessProperty(XElement xProperty, SectionData sectionData, out string strErrorMessage)
        {
            strErrorMessage = null;
            bool readTeamList = false, readAutoRun = false, readMissionList = false;

            foreach (XElement element in xProperty.Elements())
            {
                if (element.Name == "TeamList")
                {
                    sectionData.Receivers = ReadTeamList(element, out strErrorMessage);
                    readTeamList = true;
                }
                else if (element.Name == "OnlyTeamLeader")
                {
                    bool? onlyTeamLeader = GetBoolean(element.Value.Trim());

                    if (onlyTeamLeader == null)
                    {
                        strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("onlyBooleanTag2"), element.Name, element.Value);
                        return false;
                    }

                    sectionData.OnlyTeamLeader = (bool)onlyTeamLeader;
                }
                else if (element.Name == "AutoRun")
                {
                    bool? autoRun = GetBoolean(element.Value.Trim());

                    if (autoRun == null)
                    {
                        strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("onlyBooleanTag2"), element.Name, element.Value);
                        return false;
                    }

                    sectionData.AutoRun = (bool)autoRun;
                    readAutoRun = true;
                }
                else if (element.Name == "MissionList")
                {
                    List<ProcessMissionData> missions = ReadProcessMissions(element, sectionData.ID, out strErrorMessage);

                    if (missions == null)
                        return false;
                    else
                        sectionData.Missions = missions;

                    readMissionList = true;
                }
            }

            if (readTeamList == false)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noTeamListTagInProcessProperty");
                return false;
            }

            if (readAutoRun == false)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noAutoRunTagInProcessProperty");
                return false;
            }

            if (readMissionList == false)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noMissionListTagInProcessProperty");
                return false;
            }

            return true;
        }

        private static List<Receiver> ReadTeamList(XElement xTeamList, out string strErrorMessage)
        {
            strErrorMessage = null;

            List<Receiver> receivers = new List<Receiver>();
            string[] tokens = xTeamList.Value.Trim().Split(',');

            int nTeamID, nTeamType;

            foreach (string strToken in tokens)
            {
                string strReceiver = strToken.Trim();

                if (strReceiver.Length > 0)
                {
                    int nIndex1 = strReceiver.IndexOf('(');
                    int nIndex2 = strReceiver.IndexOf(')');

                    if (nIndex1 < 0 || nIndex2 <= nIndex1)
                        continue;

                    string strID = strReceiver.Substring(0, nIndex1).Trim();
                    string strType = strReceiver.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1).Trim();

                    if (int.TryParse(strID, out nTeamID) && int.TryParse(strType, out nTeamType))
                    {
                        receivers.Add(new Receiver(nTeamType, nTeamID));
                    }
                }
            }

            return receivers;
        }

        private static List<ProcessMissionData> ReadProcessMissions(XElement xMissions, int nProcessID, out string strErrorMessage)
        {
            strErrorMessage = null;

            int nOrderIndex = 1;
            List<ProcessMissionData> missions = new List<ProcessMissionData>();

            foreach (XElement element in xMissions.Elements())
            {
                if (element.Name == "Mission")
                {
                    string strProcessMission = element.Value.Trim();

                    if (strProcessMission.Length > 0)
                    {
                        ProcessMission mission = new ProcessMission();
                        mission.MissionText = strProcessMission;
                        mission.ProcessID = nProcessID;

                        ProcessMissionData data = ProcessMissionDataSorter.ToMissionData(mission);

                        if (data != null)
                            missions.Add(data);
                    }

                    nOrderIndex++;
                }
                else if (element.Name == "MissionExternal")
                {
                    int nProgramID;
                    string strProgramName;
                    List<ProcessExternalMission> externalMissions = ReadProcessExternalMissions(element, nOrderIndex++, out nProgramID, out strProgramName, out strErrorMessage);

                    if (externalMissions == null)
                        return null;

                    ProcessMissionData data = new ProcessMissionData();

                    data.MissionType = ProcessMissionData.MissionDataType.External;
                    data.ProcessID = nProcessID;
                    data.ProgramID = nProgramID;
                    data.ProgramName = strProgramName;
                    data.OrderIndex = nOrderIndex - 1;

                    data.Parameters = new List<string>();
                    data.Parameters.Add(null);

                    missions.Add(data);

                    foreach (ProcessExternalMission externalMission in externalMissions)
                    {
                        data.Parameters.Add(externalMission.Value);
                    }
                }
            }

            return missions;
        }

        private static List<ProcessExternalMission> ReadProcessExternalMissions(XElement xMission, int nOrderIndex, out int nProgramID, out string strProgramName, out string strErrorMessage)
        {
            strErrorMessage = null;
            nProgramID = -1;
            strProgramName = null;

            int? programID = null;

            List<ProcessExternalMission> externalMissions = null;

            foreach (XElement element in xMission.Elements())
            {
                if (element.Name == "Program")
                {
                    programID = ReadExternalProgram(element, out strProgramName, out strErrorMessage);

                    if (programID == null)
                        return null;
                }
                else if (element.Name == "Parameters")
                {
                    externalMissions = ReadProcessExternalMissionParameters(element, out strErrorMessage);

                    if (externalMissions == null)
                        return null;
                }
            }

            if (programID == null || strProgramName == null)
            {
                strErrorMessage = "MissionExternal에 Program이 존재하지 않습니다.";
                return null;
            }
            else
                nProgramID = (int)programID;

            if (externalMissions == null)
            {
                strErrorMessage = "MissionExternal에 Parameters가 존재하지 않습니다.";
                return null;
            }

            if (nProgramID < 0)
            {
                strErrorMessage = "MissionExternal에 Program이 존재하지 않습니다.";
                return null;
            }

            foreach (ProcessExternalMission externalMission in externalMissions)
            {
                externalMission.ProgramID = (int)nProgramID;
                externalMission.OrderIndex = nOrderIndex;
            }

            return externalMissions;
        }

        private static List<ProcessExternalMission> ReadProcessExternalMissionParameters(XElement xParameters, out string strErrorMessage)
        {
            strErrorMessage = null;

            int nIndex = 0;
            List<ProcessExternalMission> externalMissions = new List<ProcessExternalMission>();

            foreach (XElement element in xParameters.Elements())
            {
                if (element.Name == "Param")
                {
                    ProcessExternalMission externalMission = new ProcessExternalMission();
                    externalMission.ParameterIndex = nIndex++;
                    externalMission.Value = element.Value.Trim();

                    externalMissions.Add(externalMission);
                }
            }

            return externalMissions;
        }

        private static int? ReadExternalProgram(XElement xProgram, out string strProgramName, out string strErrorMessage)
        {
            strErrorMessage = null;
            strProgramName = null;

            int? nProgramID = null;

            foreach (XAttribute attr in xProgram.Attributes())
            {
                if (attr.Name == "id")
                {
                    int programID;

                    if (int.TryParse(attr.Value.Trim(), out programID))
                        nProgramID = programID;
                }
            }

            if (nProgramID == null)
            {
                strErrorMessage = "Program Element에 속성 id가 존재하지 않습니다.";
                return null;
            }

            string strExeName = null;
            string strDescription = null;

            foreach (XElement element in xProgram.Elements())
            {
                if (element.Name == "Exe")
                {
                    strExeName = element.Value.Trim();
                }
                else if (element.Name == "Description")
                {
                    strDescription = element.Value.Trim();
                }
            }

            if (strDescription != null)
                strProgramName = strDescription;
            else if (strExeName != null)
                strProgramName = strExeName;
            else
            {
                strErrorMessage = "외부 프로그램의 실행파일 정보가 존재하지 않습니다.";
                return null;
            }

            return nProgramID;
        }

        private static bool ReadArrows(XElement xArrows, List<ArrowData> arrows, Dictionary<int, SectionData> dicSections, out string strErrorMessage)
        {
            strErrorMessage = null;

            foreach (XElement element in xArrows.Elements())
            {
                if (element.Name == "Arrow")
                {
                    ArrowData arrowData = ReadArrow(element, dicSections, out strErrorMessage);

                    if (arrowData == null)
                        return false;
                    else
                        arrows.Add(arrowData);
                }
            }

            return true;
        }

        private static ArrowData ReadArrow(XElement xArrow, Dictionary<int, SectionData> dicSections, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strText = null;
            int? beginComponentID = null, endComponentID = null;
            int? beginComponentPosition = null, endComponentPosition = null;

            foreach (XElement element in xArrow.Elements())
            {
                if (element.Name == "Text")
                {
                    strText = element.Value;
                }
                else if (element.Name == "BeginComponentID")
                {
                    beginComponentID = GetInteger(element.Value);

                    if (beginComponentID == null)
                    {
                        strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("onlyIntegerTag2"), element.Name, element.Value);
                        return null;
                    }
                }
                else if (element.Name == "BeginComponentPosition")
                {
                    beginComponentPosition = GetInteger(element.Value);

                    if (beginComponentPosition == null)
                    {
                        strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("onlyIntegerTag1"), element.Name, element.Value);
                        return null;
                    }
                }
                else if (element.Name == "EndComponentID")
                {
                    endComponentID = GetInteger(element.Value);

                    if (endComponentID == null)
                    {
                        strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("onlyIntegerTag2"), element.Name, element.Value);
                        return null;
                    }
                }
                else if (element.Name == "EndComponentPosition")
                {
                    endComponentPosition = GetInteger(element.Value);

                    if (endComponentPosition == null)
                    {
                        strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("onlyIntegerTag1"), element.Name, element.Value);
                        return null;
                    }
                }
            }

            if (beginComponentID == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noBeginComponentIDInArrow");
                return null;
            }

            if (beginComponentPosition == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noBeginComponentPositionInArrow");
                return null;
            }

            if (endComponentID == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noEndComponentIDInArrow");
                return null;
            }

            if (endComponentPosition == null)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Get("xml").Value("noEndComponentPositionInArrow");
                return null;
            }

            SectionData beginSection, endSection;

            if (dicSections.TryGetValue((int)beginComponentID, out beginSection) == false)
            {
                strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("noDefinedTag"), "BeginComponentID", (int)beginComponentID);
                return null;
            }

            if (dicSections.TryGetValue((int)endComponentID, out endSection) == false)
            {
                strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Get("xml").Value("noDefinedTag"), "EndComponentID", (int)endComponentID);
                return null;
            }

            ArrowData arrowData = new ArrowData();

            arrowData.BeginComponentID = beginSection.ID;
            arrowData.BeginComponentPosition = (int)beginComponentPosition;
            arrowData.BeginComponentColumnIndex = beginSection.GridColumnIndex;
            arrowData.BeginComponentRowIndex = beginSection.GridRowIndex;

            arrowData.EndComponentID = endSection.ID;
            arrowData.EndComponentPosition = (int)endComponentPosition;
            arrowData.EndComponentColumnIndex = endSection.GridColumnIndex;
            arrowData.EndComponentRowIndex = endSection.GridRowIndex;

            if (strText != null)
                arrowData.Text = strText;

            return arrowData;
        }

        private static XElement FindElement(XElement node, string strNodeName)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name == strNodeName)
                    return element;
            }

            return null;
        }

        private static XAttribute FindAttribute(XElement node, string strAttrName)
        {
            foreach (XAttribute attr in node.Attributes())
            {
                if (attr.Name == strAttrName)
                    return attr;
            }

            return null;
        }

        private static bool? GetBoolean(string strValue)
        {
            if (strValue == "1")
                return true;
            else if (strValue == "0")
                return false;

            string strLower = strValue.ToLower();

            if (string.Compare(strLower, "true") == 0)
                return true;
            else if (string.Compare(strLower, "false") == 0)
                return false;

            return null;
        }

        private static int? GetInteger(string strValue)
        {
            int value;

            if (int.TryParse(strValue, out value))
                return value;

            return null;
        }

        private static float? GetFloat(string strValue)
        {
            float value;

            if (float.TryParse(strValue, out value))
                return value;

            return null;
        }
    }
}
