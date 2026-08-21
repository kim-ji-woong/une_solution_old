using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnE.Sensor;

namespace AlarmButtonSimulator
{
    public class XMLManager
    {
        private string m_strFileName = "config.xml";

        private FacilityManagerGroup m_facilityManagerGroup = null;
        private SelectedSensor m_sensor = null;
        private ButtonOption[] m_btnOptions = new ButtonOption[3];
        private string m_strSMSMessage = "";
        private string m_strBroadcastMessage = "";
        private string m_strSMSCaller = "";
        private string m_strDefaultSMSCaller = "07088983203";
        private bool m_useBroadcastSiren = false;
        private bool m_isSimulationMode = false;

        public bool IsSimulationMode
        {
            get { return m_isSimulationMode; }
            set { m_isSimulationMode = value; }
        }

        public bool BroadcastSiren
        {
            get { return m_useBroadcastSiren; }
        }

        public string SMSCaller
        {
            get { return m_strSMSCaller; }
        }

        public FacilityManagerGroup FacilityManagerGroup
        {
            get { return m_facilityManagerGroup; }
        }

        public SelectedSensor SelectedSensor
        {
            get { return m_sensor; }
        }

        public string SMSMessage
        {
            get { return m_strSMSMessage; }
        }

        public string BroadcastMessage
        {
            get { return m_strBroadcastMessage; }
        }

        public ButtonOption GetButtonOption(int nIndex)
        {
            return m_btnOptions[nIndex];
        }

        public bool Write(FacilityManagerGroup group, Circuit sensor, bool useSMS1, bool useBroadcast1, bool useSMS2, bool useBroadcast2, bool useSMS3, bool useBroadcast3, string strSMSMessage, string strBroadcastMessage, bool useBroadcastSiren)
        {
            string strPath = GetFilePath();

            if (strPath.Length == 0)
                return false;

            XmlTextWriter writer = new XmlTextWriter(strPath, Encoding.UTF8);

            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();

            return Save(writer, group, sensor, useSMS1, useBroadcast1, useSMS2, useBroadcast2, useSMS3, useBroadcast3, strSMSMessage, strBroadcastMessage, useBroadcastSiren);
        }

        private bool Save(XmlTextWriter writer, FacilityManagerGroup group, Circuit sensor, bool useSMS1, bool useBroadcast1, bool useSMS2, bool useBroadcast2, bool useSMS3, bool useBroadcast3, string strSMSMessage, string strBroadcastMessage, bool useBroadcastSiren)
        {
            writer.WriteStartElement("AlarmButton");

            SaveElement(writer, "SimulationMode", m_isSimulationMode ? "1" : "0");
            SaveElement(writer, "SMSCaller", m_strDefaultSMSCaller);

            if (group != null)
            {
                if (SaveFacilityManagerGroup(writer, group) == false)
                {
                    writer.Close();
                    return false;
                }
            }

            if (sensor != null)
            {
                if (SaveSelectedSensor(writer, sensor) == false)
                {
                    writer.Close();
                    return false;
                }
            }

            if (SaveButtonOptions(writer, useSMS1, useBroadcast1, useSMS2, useBroadcast2, useSMS3, useBroadcast3) == false)
            {
                writer.Close();
                return false;
            }

            if (SaveMessage(writer, "SMSMessage", strSMSMessage) == false ||
                SaveMessage(writer, "BroadcastMessage", strBroadcastMessage) == false)
            {
                writer.Close();
                return false;
            }

            SaveElement(writer, "BroadcastSiren", useBroadcastSiren ? "1" : "0");

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Close();

            return true;
        }

        private bool SaveMessage(XmlTextWriter writer, string strElementName, string strMessage)
        {
            writer.WriteStartElement(strElementName);

            string[] lines = strMessage.Split('\n');

            foreach (string strLine in lines)
            {
                SaveElement(writer, "Line", strLine.Trim());
            }

            writer.WriteFullEndElement();
            return true;
        }
    
        private bool SaveButtonOptions(XmlTextWriter writer, bool useSMS1, bool useBroadcast1, bool useSMS2, bool useBroadcast2, bool useSMS3, bool useBroadcast3)
        {
            writer.WriteStartElement("ButtonOptions");

            if (SaveButtonOption(writer, 1, useSMS1, useBroadcast1) == false ||
                SaveButtonOption(writer, 2, useSMS2, useBroadcast2) == false ||
                SaveButtonOption(writer, 3, useSMS3, useBroadcast3) == false)
            {
                return false;
            }

            writer.WriteFullEndElement();
            return true;
        }

        private bool SaveButtonOption(XmlTextWriter writer, int nIndex, bool useSMS, bool useBroadcast)
        {
            writer.WriteStartElement("Button");

            writer.WriteStartAttribute("index");
            writer.WriteString(nIndex.ToString());
            writer.WriteEndAttribute();

            SaveElement(writer, "SMS", useSMS ? "1" : "0");
            SaveElement(writer, "Broadcast", useBroadcast ? "1" : "0");

            writer.WriteFullEndElement();
            return true;
        }

        private bool SaveSelectedSensor(XmlTextWriter writer, Circuit sensor)
        {
            writer.WriteStartElement("SelectedSensor");

            SaveElement(writer, "ID", sensor.ID.ToString());
            SaveElement(writer, "ReceiverID", sensor.ReciverID.ToString());
            SaveElement(writer, "TagNo", sensor.TagNum.ToString());

            writer.WriteFullEndElement();
            return true;
        }

        private bool SaveFacilityManagerGroup(XmlTextWriter writer, FacilityManagerGroup group)
        {
            writer.WriteStartElement("FacilityManagerGroup");

            foreach (FacilityManager mgr in group.CompanyMembers)
            {
                if (SaveFacilityManager(writer, mgr) == false)
                    return false;
            }

            foreach (FacilityManager mgr in group.ExternalCompanyMembers)
            {
                if (SaveFacilityManager(writer, mgr) == false)
                    return false;
            }

            foreach (FacilityManager mgr in group.RegularTeams)
            {
                if (SaveFacilityManager(writer, mgr) == false)
                    return false;
            }

            foreach (FacilityManager mgr in group.ExternalTeams)
            {
                if (SaveFacilityManager(writer, mgr) == false)
                    return false;
            }

            foreach (FacilityManager mgr in group.ControlRoomMembers)
            {
                if (SaveFacilityManager(writer, mgr) == false)
                    return false;
            }

            writer.WriteFullEndElement();
            return true;
        }

        private bool SaveFacilityManager(XmlTextWriter writer, FacilityManager mgr)
        {
            writer.WriteStartElement("FacilityManager");

            SaveElement(writer, "MemberID", mgr.MemberID.ToString());
            SaveElement(writer, "MemberType", mgr.MemberType.ToString());
            SaveElement(writer, "FacilityType", ((int)mgr.Type).ToString());
            SaveElement(writer, "LevelLimit", mgr.LevelLimit.ToString());
            SaveElement(writer, "UpperLimit", mgr.UpperLimit.ToString());

            writer.WriteFullEndElement();
            return true;
        }

        private void SaveElement(XmlTextWriter writer, string strElementName, string strElementValue)
        {
            writer.WriteStartElement(strElementName);
            writer.WriteString(strElementValue);
            writer.WriteFullEndElement();
        }

        public bool Read()
        {
            string strPath = GetFilePath();

            if (strPath.Length == 0)
                return false;

            XmlTextReader reader = new XmlTextReader(strPath);
            return Load(reader);
        }

        private bool Load(XmlTextReader reader)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "AlarmButton", true) == 0)
                        {
                            bool result = LoadAlarmButton(reader);
                            reader.Close();
                            return result;
                        }
                        break;
                }
            }

            reader.Close();
            return false;
        }

        private bool LoadAlarmButton(XmlTextReader reader)
        {
            bool stop = false;
            m_facilityManagerGroup = null;
            m_sensor = null;

            m_strSMSCaller = m_strDefaultSMSCaller;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "SimulationMode", true) == 0)
                        {
                            ReadBoolean(reader, ref m_isSimulationMode);
                        }
                        else if (string.Compare(reader.Name, "SMSCaller", true) == 0)
                        {
                            ReadElementText(reader, ref m_strSMSCaller);
                        }
                        else if (string.Compare(reader.Name, "FacilityManagerGroup", true) == 0)
                        {
                            m_facilityManagerGroup = LoadFacilityManagerGroup(reader);
                        }
                        else if (string.Compare(reader.Name, "SelectedSensor", true) == 0)
                        {
                            m_sensor = LoadSelectedSensor(reader);
                        }
                        else if (string.Compare(reader.Name, "ButtonOptions", true) == 0)
                        {
                            if (LoadButtonOptions(reader) == false)
                                return false;
                        }
                        else if (string.Compare(reader.Name, "SMSMessage", true) == 0)
                        {
                            if (LoadMessage(reader, ref m_strSMSMessage) == false)
                                return false;
                        }
                        else if (string.Compare(reader.Name, "BroadcastMessage", true) == 0)
                        {
                            if (LoadMessage(reader, ref m_strBroadcastMessage) == false)
                                return false;
                        }
                        else if (string.Compare(reader.Name, "BroadcastSiren", true) == 0)
                        {
                            ReadBoolean(reader, ref m_useBroadcastSiren);
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

        private bool LoadMessage(XmlTextReader reader, ref string strMessage)
        {
            bool stop = false;
            strMessage = "";

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Line", true) == 0)
                        {
                            string strLine = "";

                            if (ReadElementText(reader, ref strLine) == false)
                                return false;

                            if (strMessage.Length == 0)
                                strMessage = strLine;
                            else
                                strMessage += "\r\n" + strLine;
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

        private bool LoadButtonOptions(XmlTextReader reader)
        {
            bool stop = false;

            for (int i = 0; i < m_btnOptions.Count(); i++)
            {
                m_btnOptions[i] = null;
            }
            
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Button", true) == 0)
                        {
                            if (LoadButtonOption(reader) == null)
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

            foreach (ButtonOption option in m_btnOptions)
            {
                if (option == null)
                    return false;
            }

            return true;
        }

        private ButtonOption LoadButtonOption(XmlTextReader reader)
        {
            bool stop = false;
            int nIndex = -1;

            while (reader.MoveToNextAttribute())
            {
                if (string.Compare(reader.Name, "index", true) == 0)
                {
                    if (int.TryParse(reader.Value.Trim(), out nIndex) == false)
                        return null;
                }
            }

            if (nIndex < 0)
                return null;

            DBUtility.VariousData<bool> useSMS = null;
            DBUtility.VariousData<bool> useBroadcast = null;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "SMS", true) == 0)
                        {
                            bool _useSMS = false;

                            if (ReadBoolean(reader, ref _useSMS) == false)
                                return null;

                            useSMS = new DBUtility.VariousData<bool>(_useSMS);
                        }
                        else if (string.Compare(reader.Name, "Broadcast", true) == 0)
                        {
                            bool _useBroadcast = false;

                            if (ReadBoolean(reader, ref _useBroadcast) == false)
                                return null;

                            useBroadcast = new DBUtility.VariousData<bool>(_useBroadcast);
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

            if (useSMS == null || useBroadcast == null)
                return null;

            if (nIndex <= 0 || nIndex > m_btnOptions.Count())
                return null;

            ButtonOption option = new ButtonOption();
            option.UseSMS = useSMS.Data;
            option.UseBroadcast = useBroadcast.Data;
            m_btnOptions[nIndex - 1] = option;

            return option;
        }

        private SelectedSensor LoadSelectedSensor(XmlTextReader reader)
        {
            bool stop = false;
            int nID = -1, nReceiverID = -1, nTagNo = -1;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "ID", true) == 0)
                        {
                            if (ReadInt(reader, ref nID) == false)
                                return null;
                        }
                        else if (string.Compare(reader.Name, "ReceiverID", true) == 0)
                        {
                            if (ReadInt(reader, ref nReceiverID) == false)
                                return null;
                        }
                        else if (string.Compare(reader.Name, "TagNo", true) == 0)
                        {
                            if (ReadInt(reader, ref nTagNo) == false)
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

            if (nID < 0 || nReceiverID < 0 || nTagNo < 0)
                return null;

            SelectedSensor sensor = new SelectedSensor();
            sensor.ID = nID;
            sensor.ReceiverID = nReceiverID;
            sensor.TagNo = nTagNo;
            return sensor;
        }

        private FacilityManagerGroup LoadFacilityManagerGroup(XmlTextReader reader)
        {
            bool stop = false;
            FacilityManagerGroup group = new FacilityManagerGroup();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "FacilityManager", true) == 0)
                        {
                            FacilityManager mgr = LoadFacilityManager(reader);

                            if (mgr == null)
                                return null;

                            group.AddManager(mgr);
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

            return group;
        }

        private FacilityManager LoadFacilityManager(XmlTextReader reader)
        {
            bool stop = false;
            int nMemberID = -1, nMemberType = -1, nFacilityType = -1;   // 필수요소
            int nLevelLimit = -1, nUpperLimit = -1;                     // 선택요소

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "MemberID", true) == 0)
                        {
                            if (ReadInt(reader, ref nMemberID) == false)
                                return null;
                        }
                        else if (string.Compare(reader.Name, "MemberType", true) == 0)
                        {
                            if (ReadInt(reader, ref nMemberType) == false)
                                return null;
                        }
                        else if (string.Compare(reader.Name, "FacilityType", true) == 0)
                        {
                            if (ReadInt(reader, ref nFacilityType) == false)
                                return null;
                        }
                        else if (string.Compare(reader.Name, "LevelLimit", true) == 0)
                        {
                            if (ReadInt(reader, ref nLevelLimit) == false)
                                return null;
                        }
                        else if (string.Compare(reader.Name, "UpperLimit", true) == 0)
                        {
                            if (ReadInt(reader, ref nUpperLimit) == false)
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

            if (nMemberID < 0 || nMemberType < 0 || nFacilityType < 0)
                return null;

            FacilityManager mgr = new FacilityManager();
            mgr.MemberID = nMemberID;
            mgr.MemberType = nMemberType;
            mgr.Type = IFacility.ToFacilityType(nFacilityType);
            mgr.UpperLimit = nUpperLimit;
            mgr.LevelLimit = nLevelLimit;

            if (nMemberType == 0)
            {
                DataCompanyMember member = DataManager.Instance.GetCompanyMember(nMemberID);
                mgr.Tag = member;
            }
            else if (nMemberType == 1 || nMemberType == 4)
            {
                DataTeam team = DataManager.Instance.GetRegularTeam(nMemberID);
                mgr.Tag = team;
            }
            else if (nMemberType == 2)
            {
                DataExternalMember member = DataManager.Instance.GetExternalMember(nMemberID);
                mgr.Tag = member;
            }
            else if (nMemberType == 3 || nMemberType == 5)
            {
                DataTeam team = DataManager.Instance.GetExternalTeam(nMemberID);
                mgr.Tag = team;
            }
            else if (nMemberType == 7)
            {
                DataTeamControlRoom team = DataManager.Instance.GetControlRoomTeam(nMemberID);
                mgr.Tag = team;
            }

            return mgr;
        }

        private bool ReadInt(XmlTextReader reader, ref int nData)
        {
            string strText = "";

            if (ReadElementText(reader, ref strText))
            {
                return int.TryParse(strText, out nData);
            }

            return false;
        }

        private bool ReadBoolean(XmlTextReader reader, ref bool bData)
        {
            string strText = "";

            if (!ReadElementText(reader, ref strText))
            {
                return false;
            }

            if (string.Compare(strText, "true", true) == 0)
                bData = true;
            else if (string.Compare(strText, "false", true) == 0)
                bData = false;
            else
                bData = int.Parse(strText) == 0 ? false : true;

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

        private string GetFilePath()
        {
            int nIndex = System.Windows.Forms.Application.ExecutablePath.LastIndexOf('\\');

            if (nIndex < 0)
                return "";

            string strPath = System.Windows.Forms.Application.ExecutablePath.Substring(0, nIndex + 1);
            strPath += m_strFileName;
            return strPath;
        }
    }

    public class SelectedSensor
    {
        private int m_nID = -1;
        private int m_nReceiverID = -1;
        private int m_nTagNo = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int ReceiverID
        {
            get { return m_nReceiverID; }
            set { m_nReceiverID = value; }
        }

        public int TagNo
        {
            get { return m_nTagNo; }
            set { m_nTagNo = value; }
        }
    }

    public class ButtonOption
    {
        private bool m_useSMS = false;
        private bool m_useBroadcast = false;

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
    }
}
