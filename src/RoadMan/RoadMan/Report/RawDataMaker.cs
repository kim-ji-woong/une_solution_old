using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RoadMan
{
    public class RawDataMaker
    {
        private PanelDXFViewer m_panel = null;
        private string m_strVer = "1.0";
        private string m_strErrorMessage = "";

        public string ErrorMessage
        {
            get { return m_strErrorMessage; }
        }

        public RawDataMaker(PanelDXFViewer panel)
        {
            m_panel = panel;
        }

        public bool SaveFile(int nIndex, string strFolderPath)
        {
            if (m_panel == null)
                return false;

            string strRegionName = "";

            if (m_panel.RegionName.Length > 0)
                strRegionName = m_panel.RegionName;
            else
                strRegionName = FormMain.Instance.GetProjectName(m_panel.DXFFilePath);

            string strFilePath = strFolderPath + "\\" + nIndex.ToString() + "_" + strRegionName + ".xml";

            XmlTextWriter writer = null;

            try
            {
                writer = new XmlTextWriter(strFilePath, Encoding.UTF8);

                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();

                writer.WriteStartElement("RoadManReport");

                writer.WriteStartAttribute("ver");
                writer.WriteString(m_strVer);
                writer.WriteEndAttribute();

                writer.WriteStartElement("RegionName");
                writer.WriteString(strRegionName);
                writer.WriteEndElement();
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            if (!SaveProcessList(writer))
            {
                writer.Close();
                return false;
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();

            return true;
        }

        private bool SaveProcessList(XmlTextWriter writer)
        {
            writer.WriteStartElement("ProcessList");

            List<ProcessResult> results = m_panel.ProcessResultForm.ProcessResults;

            foreach (ProcessResult result in results)
            {
                if (!SaveProcess(writer, result))
                    return false;
            }

            writer.WriteEndElement();
            return true;
        }

        private bool SaveProcess(XmlTextWriter writer, ProcessResult result)
        {
            ProcessSchedule schedule = result.ProcessSchedule;

            if (schedule == null)
                return false;

            if (schedule.Properties.Count == 0)
                return true;

            writer.WriteStartElement("Process");

            writer.WriteStartElement("ProcessName");
            writer.WriteString(schedule.ScheduleName);
            writer.WriteEndElement();

            writer.WriteStartElement("ProcessDescription");
            writer.WriteString(schedule.Description);
            writer.WriteEndElement();

            if (!SaveStreetList(writer, result))
                return false;

            writer.WriteEndElement();
            return true;
        }

        private bool SaveStreetList(XmlTextWriter writer, ProcessResult result)
        {
            writer.WriteStartElement("StreetList");

            foreach (ResultProperty prop in result.ResultProperties)
            {
                if (!SaveStreet(writer, prop))
                    return false;
            }

            writer.WriteEndElement();
            return true;
        }

        private bool SaveStreet(XmlTextWriter writer, ResultProperty prop)
        {
            ScheduleProperty scheduleProp = prop.ScheduleProperty;

            if (scheduleProp == null)
                return false;

            writer.WriteStartElement("Street");

            writer.WriteStartElement("StreetName");
            writer.WriteString(scheduleProp.StreetName);
            writer.WriteEndElement();

            writer.WriteStartElement("CategoryName");
            writer.WriteString(scheduleProp.Category);
            writer.WriteEndElement();

            writer.WriteStartElement("SubCategoryName");
            writer.WriteString(scheduleProp.SubCategory);
            writer.WriteEndElement();

            if (scheduleProp.Area != null)
            {
                writer.WriteStartElement("ScheduleArea");
                writer.WriteString(scheduleProp.Area.Data.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("CompleteArea");
                writer.WriteString(prop.TotalArea.ToString());
                writer.WriteEndElement();
            }

            long nScheduleCost = scheduleProp.GetTotalCost();

            if (nScheduleCost > 0)
            {
                writer.WriteStartElement("ScheduleCost");
                writer.WriteString(nScheduleCost.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("ResultCost");
                writer.WriteString(prop.TotalCost.ToString());
                writer.WriteEndElement();
            }

            if (scheduleProp.FirstDate != null)
            {
                writer.WriteStartElement("FirstDate");
                writer.WriteString(scheduleProp.FirstDate.Data.ToShortDateString());
                writer.WriteEndElement();
            }

            if (scheduleProp.FinalDate != null)
            {
                writer.WriteStartElement("FinalDate");
                writer.WriteString(scheduleProp.FinalDate.Data.ToShortDateString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            return true;
        }
    }
}
