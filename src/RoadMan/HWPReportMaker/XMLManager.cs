using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HWPReportMaker
{
    public class XMLManager
    {
        private string m_strErrorMessage = "";
        private string m_strXMLVer = "";

        public string ErrorMessage
        {
            get { return m_strErrorMessage; }
        }

        public Project ReadXML(string strPath)
        {
            m_strErrorMessage = "";

            XmlTextReader reader = null;
            bool stop = false;
            Project prj = null;

            try
            {
                reader = new XmlTextReader(strPath);

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "RoadManReport", true) != 0)
                            {
                                m_strErrorMessage = "다른 형식의 파일입니다.";
                                stop = true;
                            }
                            else
                            {
                                prj = ReadRoadManReport(reader);

                                if (prj == null)
                                    stop = true;
                            }

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
                return null;
            }

            reader.Close();
            return prj;
        }

        private Project ReadRoadManReport(XmlTextReader reader)
        {
            if (reader == null)
                return null;

            bool stop = false;
            Project prj = new Project();
            
            try
            {
                System.Windows.Forms.TabPage page = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "ver", true) == 0)
                    {
                        m_strXMLVer = reader.Value.ToString();
                    }
                }

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "RegionName", true) == 0)
                            {
                                string strRegionName = "";

                                if (ReadText(reader, ref strRegionName, true))
                                    prj.RegionName = strRegionName;
                                else
                                    return null;
                            }
                            else if (string.Compare(reader.Name, "ProcessList", true) == 0)
                            {
                                if (!ReadProcessList(reader, prj.ProcessList))
                                    return null;
                            }
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
            }

            return prj;
        }

        private bool ReadProcessList(XmlTextReader reader, List<Process> processList)
        {
            if (reader.IsEmptyElement)
                return true;

            bool stop = false;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Process", true) == 0)
                            {
                                Process process = ReadProcess(reader);

                                if (process == null)
                                    return false;
                                else
                                    processList.Add(process);
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
            }

            return true;
        }

        private Process ReadProcess(XmlTextReader reader)
        {
            bool stop = false;

            string strData = "";
            Process process = new Process();

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "ProcessName", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                process.ProcessName = strData;
                            }
                            else if (string.Compare(reader.Name, "ProcessDescription", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                process.Description = strData;
                            }
                            else if (string.Compare(reader.Name, "StreetList", true) == 0)
                            {
                                if (!ReadStreetList(reader, process))
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
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return process;
        }

        private bool ReadStreetList(XmlTextReader reader, Process process)
        {
            if (reader.IsEmptyElement)
                return true;

            List<Street> streetList = process.StreetList;

            bool stop = false;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Street", true) == 0)
                            {
                                Street street = ReadStreet(reader);

                                if (street == null)
                                    return false;
                                else
                                {
                                    street.Process = process;
                                    streetList.Add(street);
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
            }

            return true;
        }

        private Street ReadStreet(XmlTextReader reader)
        {
            bool stop = false;

            string strData = "";
            Street street = new Street();

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "StreetName", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                street.StreetName = strData;
                            }
                            else if (string.Compare(reader.Name, "CategoryName", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                street.CategoryName = strData;
                            }
                            else if (string.Compare(reader.Name, "SubCategoryName", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                street.SubCategoryName = strData;
                            }
                            else if (string.Compare(reader.Name, "ScheduleArea", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                if (strData.Length > 0)
                                {
                                    int nData;
                                    if (!int.TryParse(strData, out nData))
                                        return null;
                                    else
                                        street.ScheduleArea = new VariousData<int>(nData);
                                }
                            }
                            else if (string.Compare(reader.Name, "CompleteArea", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                if (strData.Length > 0)
                                {
                                    int nData;
                                    if (!int.TryParse(strData, out nData))
                                        return null;
                                    else
                                        street.CompleteArea = new VariousData<int>(nData);
                                }
                            }
                            else if (string.Compare(reader.Name, "ScheduleCost", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                if (strData.Length > 0)
                                {
                                    long nData;
                                    if (!long.TryParse(strData, out nData))
                                        return null;
                                    else
                                        street.ScheduleCost = new VariousData<long>(nData);
                                }
                            }
                            else if (string.Compare(reader.Name, "ResultCost", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                if (strData.Length > 0)
                                {
                                    long nData;
                                    if (!long.TryParse(strData, out nData))
                                        return null;
                                    else
                                        street.ResultCost = new VariousData<long>(nData);
                                }
                            }
                            else if (string.Compare(reader.Name, "FirstDate", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                if (strData.Length > 0)
                                {
                                    DateTime dt;
                                    if (!DateTime.TryParse(strData, out dt))
                                        return null;
                                    else
                                        street.FirstDate = new VariousData<DateTime>(dt);
                                }
                            }
                            else if (string.Compare(reader.Name, "FinalDate", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                if (strData.Length > 0)
                                {
                                    DateTime dt;
                                    if (!DateTime.TryParse(strData, out dt))
                                        return null;
                                    else
                                        street.FinalDate = new VariousData<DateTime>(dt);
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
            }

            return street;
        }

        private bool ReadText(XmlTextReader reader, ref string strText, bool allowEmpty = false)
        {
            if (reader.IsEmptyElement)
            {
                strText = "";
                return allowEmpty;
            }

            if (!ReadElementText(reader, ref strText))
                strText = "";

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
    }
}
