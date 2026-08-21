using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;

namespace ScenarioToDB
{
    // DB가 아닌 파일에서 읽고 쓴다.
    public class XMLManager
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

        private static string XML_VERSION = "V1.3";
        ///////////////////////////////////////

        private ArrayList m_arrActionSteps = new ArrayList();

        public bool IsNormal
        {
            get { return m_isNormal; }
        }

        private bool GetTeamInfo(ActionStep actionStep, out string strTeamName, out int nTeamID, out int nTeamType)
        {
            strTeamName = "";
            nTeamID = nTeamType = -1;

            if (actionStep.StepMemberList.Count == 0)
                return false;

            StepMember stepMember = (StepMember)actionStep.StepMemberList[0];
            strTeamName = stepMember.TeamName;
            nTeamID = stepMember.TeamID;
            nTeamType = stepMember.TeamType;

            return true;
        }

        private StepMember FindStepMember(ActionStep actionStep, int nTeamID, int nTeamType)
        {
            foreach (StepMember stepMember in actionStep.StepMemberList)
            {
                if (stepMember.TeamID == nTeamID && stepMember.TeamType == nTeamType)
                    return stepMember;
            }

            return null;
        }

		private bool Load(XmlTextReader reader)
		{
            m_arrActionSteps.Clear();

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

			return true;
		}

		public bool Load(System.IO.Stream stream)
		{
			XmlTextReader reader = InitReader(stream);
			return Load(reader);
		}

        public bool Load(string strPath)
        {
            XmlTextReader reader = InitReader(strPath);
            bool result = Load(reader);
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
            }

            if (nActionStepID < 0)
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

            if (!readStepMemberList)
            {
                m_strErrorMessage = string.Format("Line Number {0}, StepMemberList가 존재하지 않습니다.", reader.LineNumber);
                return false;
            }

            m_arrActionSteps.Add(actionStep);
            return true;
        }

        private bool ReadParentStepID(XmlTextReader reader, ref int nParentStepID)
        {
            if (reader.IsEmptyElement)
                return false;
            return ReadInt(reader, ref nParentStepID, "ParentStepID는", "ParentStepID가");
        }

        private bool ReadProcessTimeType(XmlTextReader reader, ref int nProcessTimeType)
        {
            if (reader.IsEmptyElement)
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
            if (reader.IsEmptyElement)
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
            stepMember.TeamID = nTeamID;
            stepMember.TeamType = nTeamType;
            stepMember.TeamName = strStepMemberName;

            bool stop = false, readComponentList = false, readArrowList = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "ComponentList", true) == 0)
                        {
                            if (!ReadComponentList(reader, stepMember))
                                return null;
                            else
                                readComponentList = true;
                        }
                        else if (string.Compare(reader.Name, "ArrowList", true) == 0)
                        {
                            if (!ReadArrowList(reader, stepMember))
                                return null;
                            else
                                readArrowList = true;
                        }
                        else
                            PassElement(reader);

                        break;

                    case XmlNodeType.EndElement:
						if (string.Compare(reader.Name, "StepMember", true) == 0)
							stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            if (!readComponentList)
            {
                m_strErrorMessage = string.Format("Line Number {0}, ComponentList가 존재하지 않습니다.", reader.LineNumber);
                return null;
            }

            if (!readArrowList)
            {
                m_strErrorMessage = string.Format("Line Number {0}, ArrowList가 존재하지 않습니다.", reader.LineNumber);
                return null;
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
                            if (!ReadArrow(reader, stepMember.ArrowList, stepMember.ComponentList))
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

            return true;
        }

        private Component GetComponent(int nID, ArrayList arrComponents)
        {
            foreach (Component component in arrComponents)
            {
                if (component.ID == nID)
                    return component;
            }

            return null;
        }

        private bool ReadArrow(XmlTextReader reader, ArrayList arrArrows, ArrayList arrComponents)
        {
            bool stop = false, readBeginComponentID = false;
            bool readBeginComponentPosition = false, readEndComponentID = false, readEndComponentPosition = false;

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

                            readBeginComponentID = true;
                            arrow.BeginComponent = GetComponent(nBeginComponentID, arrComponents);
                        }
                        else if (string.Compare(reader.Name, "BeginComponentPosition", true) == 0)
                        {
                            if (!ReadBeginComponentPosition(reader, ref nBeginComponentPosition))
                                return false;

                            readBeginComponentPosition = true;
                            arrow.BeginComponentPosition = nBeginComponentPosition;
                        }
                        else if (string.Compare(reader.Name, "EndComponentID", true) == 0)
                        {
                            if (!ReadEndComponentID(reader, ref nEndComponentID))
                                return false;

                            readEndComponentID = true;
                            arrow.EndComponent = GetComponent(nEndComponentID, arrComponents);
                        }
                        else if (string.Compare(reader.Name, "EndComponentPosition", true) == 0)
                        {
                            if (!ReadEndComponentPosition(reader, ref nEndComponentPosition))
                                return false;

                            readEndComponentPosition = true;
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

            if (!readBeginComponentID)
            {
                m_strErrorMessage = string.Format("Line Number {0}, BeginComponentID가 존재하지 않습니다.", reader.LineNumber);
                return false;
            }

            if (!readBeginComponentPosition)
            {
                m_strErrorMessage = string.Format("Line Number {0}, BeginComponentPosition이 존재하지 않습니다.", reader.LineNumber);
                return false;
            }

            if (!readEndComponentID)
            {
                m_strErrorMessage = string.Format("Line Number {0}, EndComponentID가 존재하지 않습니다.", reader.LineNumber);
                return false;
            }

            if (!readEndComponentPosition)
            {
                m_strErrorMessage = string.Format("Line Number {0}, EndComponentPosition이 존재하지 않습니다.", reader.LineNumber);
                return false;
            }

            arrArrows.Add(arrow);
            return true;
        }

        private bool ReadBeginComponentID(XmlTextReader reader, ref int nBeginComponentID)
        {
            if (reader.IsEmptyElement)
                return false;

            return ReadInt(reader, ref nBeginComponentID, "BeginComponentID는", "BeginComponentID가");
        }

        private bool ReadBeginComponentPosition(XmlTextReader reader, ref int nBeginComponentPosition)
        {
            if (reader.IsEmptyElement)
                return false;

            return ReadInt(reader, ref nBeginComponentPosition, "BeginComponentPosition은", "BeginComponentPosition은");
        }

        private bool ReadEndComponentID(XmlTextReader reader, ref int nEndComponentID)
        {
            if (reader.IsEmptyElement)
                return false;

            return ReadInt(reader, ref nEndComponentID, "EndComponentID는", "EndComponentID가");
        }

        private bool ReadEndComponentPosition(XmlTextReader reader, ref int nEndComponentPosition)
        {
            if (reader.IsEmptyElement)
                return false;

            return ReadInt(reader, ref nEndComponentPosition, "EndComponentPosition은", "EndComponentPosition은");
        }

        private bool ReadComponentList(XmlTextReader reader, StepMember stepMember)
        {
            if (reader.IsEmptyElement)
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
                            if (component == null)
                                return false;

                            stepMember.ComponentList.Add(component);
                        }
						else if(string.Compare(reader.Name, "Viewport", true) == 0)
                        {
                            Viewport viewport = ReadViewport(reader);						
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
					case XmlNodeType.EndElement:
						stop = true;
						break;
				}

				if (stop)
					break;
			}
			return viewport;
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
            bool stop = false, readUseSMS = false, readSMSText = false, readSMSExternalTeamIDList = false;
            bool readUseFax = false, readFaxExternalTeamIDList = false;

            PropertyExternal property = new PropertyExternal();

            bool useSMS = false, useFax = false;
            string strSMSText = "", strSMSExternalTeamIDList = "", strFaxExternalTeamIDList = "";

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "useSMS", true) == 0)
                        {
                            if (!ReadUseSMS(reader, ref useSMS))
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
                        }
                        else if (string.Compare(reader.Name, "SMSExternalTeamIDList", true) == 0)
                        {
                            if (ReadSMSExternalTeamIDList(reader, ref strSMSExternalTeamIDList))
                            {
                                readSMSExternalTeamIDList = true;
                                property.SMSReceivers = strSMSExternalTeamIDList;
                            }
                        }
                        else if (string.Compare(reader.Name, "useFax", true) == 0)
                        {
                            if (!ReadUseFax(reader, ref useFax))
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
            if (reader.IsEmptyElement)
                return false;
            return ReadBoolean(reader, ref useSMS, "useSMS가", "useSMS는");
        }

        private bool ReadSMSText(XmlTextReader reader, ref string strSMSText)
        {
            if (reader.IsEmptyElement)
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
            if (reader.IsEmptyElement)
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
            if (reader.IsEmptyElement)
                return false;

            return ReadBoolean(reader, ref useFax, "useFax가", "useFax는");
        }

        private bool ReadFaxExternalTeamIDList(XmlTextReader reader, ref string strFaxExternalTeamIDList)
        {
            if (reader.IsEmptyElement)
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
                            if (!ReadCategoryName(reader, ref strCategoryName))
                                return null;

                            readCategoryName = true;
                            property.LinkedCategoryName = strCategoryName;
                        }
                        else if (string.Compare(reader.Name, "SubCategoryName", true) == 0)
                        {
                            if (!ReadSubCategoryName(reader, ref strSubCategoryName))
                                return null;

                            readSubCategoryName = true;
                            property.LinkedSubCategoryName = strSubCategoryName;
                        }
                        else if (string.Compare(reader.Name, "DisasterName", true) == 0)
                        {
                            if (!ReadDisasterName(reader, ref strDisasterName))
                                return null;

                            readDisasterName = true;
                            property.LinkedDisasterName = strDisasterName;
                        }
                        else if (string.Compare(reader.Name, "ActionStepName", true) == 0)
                        {
                            if (!ReadActionStepName(reader, ref strActionStepName, "ActionStepName"))
                                return null;

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
                            if (!ReadLinkedComponentName(reader, ref strLinkedComponentName))
                                return null;

                            readLinkedComponentName = true;
                        }
                        else if (string.Compare(reader.Name, "LinkedStepMemberIndex", true) == 0)
                        {
                            if (!ReadLinkedStepMemberID(reader, ref nLinkedStepMemberID))
                                return null;

                            readLinkedStepMemberID = true;
                        }
                        else if (string.Compare(reader.Name, "LinkedComponentID", true) == 0)
                        {
                            if (!ReadLinkedComponentID(reader, ref nLinkedComponentID))
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

            PropertyLink property = new PropertyLink();
            property.LinkedComponentName = strLinkedComponentName;
            property.LinkedStepMemberID = nLinkedStepMemberID;
            property.LinkedID = nLinkedComponentID;

            return property;
        }

        private bool ReadLinkedComponentName(XmlTextReader reader, ref string strLinkedComponentName)
        {
            if (reader.IsEmptyElement)
                return false;

            if (!ReadElementText(reader, ref strLinkedComponentName))
            {
                m_strErrorMessage = string.Format("Line Number {0}, LinkedComponentName이 비어있습니다.", reader.LineNumber);
                return false;
            }

            return true;
        }

        private bool ReadLinkedStepMemberID(XmlTextReader reader, ref int nLinkedStepMemberID)
        {
            if (reader.IsEmptyElement)
                return false;

            return ReadInt(reader, ref nLinkedStepMemberID, "LinkedStepMemberIndex는", "LinkedStepMemberIndex가");
        }

        private bool ReadLinkedComponentID(XmlTextReader reader, ref int nLinkedComponentID)
        {
            if (reader.IsEmptyElement)
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
            if (reader.IsEmptyElement)
                return false;
            return ReadBoolean(reader, ref isBegin, "isBegin이", "isBegin은");
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
    }

    public class ActionStep : Data_ActionStep
    {
        private ArrayList m_arrStepMember = new ArrayList();

        public ArrayList StepMemberList
        {
            get { return m_arrStepMember; }
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

    public class StepMember : StepMemberDataEx
    {
        private ArrayList m_arrComponent = new ArrayList();
        private ArrayList m_arrArrow = new ArrayList();

        private Viewport m_viewport = null;
        public Viewport Viewport
        {
            get { return m_viewport; }
            set { m_viewport = value; }
        }

        private string m_strTeamName = "";

        public StepMember()
            : base(-1, -1, -1)
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
    }




    public class Arrow
    {
        private string m_strText = "";
        private Component m_beginComponent = null;
        private int m_nBeginComponentPosition = -1;
        private Component m_endComponent = null;
        private int m_nEndComponentPosition = -1;

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }

        public Component BeginComponent
        {
            get { return m_beginComponent; }
            set { m_beginComponent = value; }
        }

        public int BeginComponentPosition
        {
            get { return m_nBeginComponentPosition; }
            set { m_nBeginComponentPosition = value; }
        }

        public Component EndComponent
        {
            get { return m_endComponent; }
            set { m_endComponent = value; }
        }

        public int EndComponentPosition
        {
            get { return m_nEndComponentPosition; }
            set { m_nEndComponentPosition = value; }
        }
    }

    public class Component
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

    public class ComponentProperty
    {
        protected Sections.Section.ComponentType m_nType = Sections.Section.ComponentType.NONE;

        public Sections.Section.ComponentType Type
        {
            get { return m_nType; }
            set { m_nType = value; }
        }
    }

    public class PropertyProcess : ComponentProperty
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

    public class PropertyEndPoint : ComponentProperty
    {
        private bool m_isBegin = true;

        public bool IsBegin
        {
            get { return m_isBegin; }
            set { m_isBegin = value; }
        }
    }

    public class PropertyLink : ComponentProperty
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

    public class PropertyTransSOP : ComponentProperty
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

    public class PropertyInternal : ComponentProperty
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

    public class PropertyExternal : ComponentProperty
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

    public class PropertyTransmission : ComponentProperty
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

    public class PropertyGroup : ComponentProperty
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

    public class Viewport
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
