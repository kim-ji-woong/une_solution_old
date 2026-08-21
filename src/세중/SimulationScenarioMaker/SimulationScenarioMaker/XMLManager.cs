using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SimulationScenarioMaker
{
    public class XMLManager
    {
        private string m_strDefFilePath = "";
        private string m_strErrorMessage = "";
        private DataManager m_dataMgr = null;

        public string FilePath
        {
            get { return m_strDefFilePath; }
            set { m_strDefFilePath = value; }
        }

        public string ErrorMessage
        {
            get { return m_strErrorMessage; }
            set { m_strErrorMessage = value; }
        }

        public XMLManager()
        {
            string strPath = System.Windows.Forms.Application.ExecutablePath;
            int nIndex = strPath.LastIndexOf('\\');

            if (nIndex >= 0)
                strPath = strPath.Substring(0, nIndex);
            else
                strPath = ".\\";

            m_strDefFilePath = strPath + "\\Simulation.xml";
        }

        public bool ReadXML(string strPath, DataManager dataMgr)
        {
            dataMgr.Events.Clear();
            dataMgr.RunningTime = 0;
            dataMgr.RepeatCount = 1;
            m_dataMgr = dataMgr;

            m_strErrorMessage = "";

            XmlTextReader reader = new XmlTextReader(strPath);
            bool stop = false;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Simulation", true) == 0)
                            {
                                if (!ReadSimulation(reader))
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
            dataMgr.CalcBySensor();

            return true;
        }

        private bool ReadSimulation(XmlTextReader reader)
        {
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

            return true;
        }

        private bool ReadBody(XmlTextReader reader)
        {
            bool stop = false;
            
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Event", true) == 0)
                        {
                            if (!ReadEvent(reader))
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

        private bool ReadEvent(XmlTextReader reader)
        {
            bool stop = false;
            int hour = -1, minute = -1, second = -1;

            while (reader.MoveToNextAttribute())
            {
                if (string.Compare(reader.Name, "time", true) == 0)
                {
                    string[] arrTimes = reader.Value.Split(':');

                    if (arrTimes.Count() != 3)
                        return false;

                    if (!int.TryParse(arrTimes[0], out hour))
                        return false;

                    if (!int.TryParse(arrTimes[1], out minute))
                        return false;

                    if (!int.TryParse(arrTimes[2], out second))
                        return false;
                }
            }

            if (hour < 0 || minute < 0 || second < 0)
            {
                m_strErrorMessage = "형식에 맞지 않는 Event@time이 존재합니다.";
                return false;
            }

            EventData data = new EventData();
            data.EventTime = hour * 3600 + minute * 60 + second;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Sensor", true) == 0)
                        {
                            SensorData sensor = ReadSensor(reader);

                            if (sensor == null)
                                return false;
                            else
                                data.Sensors.Add(sensor);
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

            m_dataMgr.Events.Add(data);

            return true;
        }

        private SensorData ReadSensor(XmlTextReader reader)
        {
            bool stop = false, readID = false, readX = false, readY = false;
            string strID = "";
            double x = 0.0, y = 0.0;
            
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "ID", true) == 0)
                        {
                            if (!ReadElementText(reader, ref strID))
                                return null;
                            else
                                readID = true;
                        }
                        else if (string.Compare(reader.Name, "X", true) == 0)
                        {
                            string strX = "";
                            if (!ReadElementText(reader, ref strX))
                                return null;
                            else
                            {
                                readX = double.TryParse(strX, out x);
                            }
                        }
                        else if (string.Compare(reader.Name, "Y", true) == 0)
                        {
                            string strY = "";
                            if (!ReadElementText(reader, ref strY))
                                return null;
                            else
                            {
                                readY = double.TryParse(strY, out y);
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

            if (!readID)
            {
                m_strErrorMessage = "Sensor/ID를 찾을수 없습니다.";
                return null;
            }

            if (!readX)
            {
                m_strErrorMessage = "Sensor/X를 찾을수 없습니다.";
                return null;
            }

            if (!readY)
            {
                m_strErrorMessage = "Sensor/Y를 찾을수 없습니다.";
                return null;
            }

            SensorData sensor = new SensorData();

            sensor.SensorID = strID;
            sensor.X = x;
            sensor.Y = y;

            return sensor;
        }

        private bool ReadHeader(XmlTextReader reader)
        {
            bool stop = false, readRepeatCount = false, readRunningTime = false;
            int nRepeatCount = -1, nRunningTime = -1;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "RepeatCount", true) == 0)
                        {
                            if (!ReadInt(reader, ref nRepeatCount, "RepeatCount는", "RepeatCount가"))
                                return false;

                            readRepeatCount = true;
                        }
                        else if (string.Compare(reader.Name, "RunningTime", true) == 0)
                        {
                            nRunningTime = ReadRunningTime(reader);

                            if (nRunningTime < 0)
                                return false;

                            readRunningTime = true;
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

            if (readRepeatCount == false)
                m_strErrorMessage = "RepeatCount 정보를 찾을 수 없습니다.";
            else if (readRunningTime == false)
            {
                if (m_strErrorMessage.Length == 0)
                    m_strErrorMessage = "RunningTime 정보를 찾을 수 없습니다.";
            }
            else
            {
                m_dataMgr.RepeatCount = nRepeatCount;
                m_dataMgr.RunningTime = nRunningTime;
            }

            return readRepeatCount && readRunningTime;
        }

        // Return 값 : 초
        private int ReadRunningTime(XmlTextReader reader)
        {
            if (reader.IsEmptyElement)
                return -1;

            bool stop = false;
            int nSecond = -1;
            int hour = 0, minute = 0, second = 0;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Hour", true) == 0)
                        {
                            if (!ReadInt(reader, ref hour, "Hour는", "Hour가"))
                                return -1;
                            else
                            {
                                if (nSecond < 0)
                                    nSecond = 0;

                                nSecond += hour * 3600;
                            }
                        }
                        else if (string.Compare(reader.Name, "Minute", true) == 0)
                        {
                            if (!ReadInt(reader, ref minute, "Minute은", "Minute이"))
                                return -1;
                            else
                            {
                                if (nSecond < 0)
                                    nSecond = 0;

                                nSecond += minute * 60;
                            }
                        }
                        else if (string.Compare(reader.Name, "Second", true) == 0)
                        {
                            if (!ReadInt(reader, ref second, "Second는", "Second가"))
                                return -1;
                            else
                            {
                                if (nSecond < 0)
                                    nSecond = 0;

                                nSecond += second;
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

            if (nSecond < 0)
                m_strErrorMessage = "RunningTime에 시간정보가 없습니다.";

            return nSecond;
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

        public bool SaveXML(string strPath, DataManager dataMgr)
        {
            XmlTextWriter writer = InitWriter(strPath);
            return SaveXML(writer, dataMgr);
        }

        private bool SaveXML(XmlTextWriter writer, DataManager dataMgr)
        {
            writer.WriteStartElement("Simulation");

            if (!MakeHeader(writer, dataMgr))
                return false;

            if (!MakeBody(writer, dataMgr))
                return false;

            writer.WriteEndElement();

            writer.WriteEndDocument();
            writer.Close();

            return true;
        }

        private bool MakeBody(XmlTextWriter writer, DataManager dataMgr)
        {
            writer.WriteStartElement("Body"); // Body 시작

            foreach (EventData data in dataMgr.Events)
            {
                writer.WriteStartElement("Event");

                int nHour = data.EventTime / 3600;
                int nMin = (data.EventTime - nHour * 3600) / 60;
                int nSec = data.EventTime - nHour * 3600 - nMin * 60;

                string strEventTime = string.Format("{0}:{1}:{2}", nHour, nMin, nSec);

                writer.WriteStartAttribute("time");
                writer.WriteString(strEventTime);
                writer.WriteEndAttribute();

                foreach (SensorData sensorData in data.Sensors)
                {
                    writer.WriteStartElement("Sensor");

                    writer.WriteStartElement("ID");
                    writer.WriteString(sensorData.SensorID);
                    writer.WriteEndElement();

                    writer.WriteStartElement("X");
                    writer.WriteString(string.Format("{0:F3}", sensorData.X));
                    writer.WriteEndElement();

                    writer.WriteStartElement("Y");
                    writer.WriteString(string.Format("{0:F3}", sensorData.Y));
                    writer.WriteEndElement();

                    writer.WriteEndElement();   // Sensor 끝
                }

                writer.WriteEndElement();   // Event 끝
            }

            writer.WriteEndElement();   // Body 끝
            return true;
        }

        private bool MakeHeader(XmlTextWriter writer, DataManager dataMgr)
        {
            writer.WriteStartElement("Header"); // Header 시작

            writer.WriteStartElement("RepeatCount");
            writer.WriteString(dataMgr.RepeatCount.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("RunningTime");

            int nHour = dataMgr.RunningTime / 3600;
            int nMin = (dataMgr.RunningTime - nHour * 3600) / 60;
            int nSec = dataMgr.RunningTime - nHour * 3600 - nMin * 60;

            writer.WriteStartElement("Hour");
            writer.WriteString(nHour.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Minute");
            writer.WriteString(nMin.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Second");
            writer.WriteString(nSec.ToString());
            writer.WriteEndElement();

            writer.WriteEndElement();   // RunningTime 끝
            writer.WriteEndElement();   // Header 끝
            return true;
        }

        private XmlTextWriter InitWriter(string strPath)
        {
            XmlTextWriter writer = new XmlTextWriter(strPath, Encoding.UTF8);

            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();

            return writer;
        }
    }
}
