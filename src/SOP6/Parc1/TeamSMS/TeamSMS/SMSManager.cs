using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace TeamSMS
{
    public class SMSManager
    {
        private const string m_strFloorManagerTag = "_#층담당자";

        private WebDBManager m_dbMgr = null;
        private string m_strLocation = "";
        private int m_nActionStepHistoryID = -1;
        private bool m_isNormal = true;
        private Network.NetworkManager m_netMgr = null;

        public bool SendSMS(string[] args)
        {
            if (args == null)
                return false;
            
            VariousData<bool> includeChildTeam = null;
            int nArgumentCount = args.Count();

            if (nArgumentCount < 2)
                return false;

            int nSiteID;

            if (ReadConfig("siteid", out nSiteID) == false)
                nSiteID = 201;

            m_dbMgr = new WebDBManager(nSiteID);
            m_netMgr = new Network.NetworkManager(m_dbMgr);
            
            m_nActionStepHistoryID = GetActionStepHistoryID();
            if (m_nActionStepHistoryID > 0)
            {
                bool isNormal;

                if (GetActionStepInfo(out isNormal))
                    m_isNormal = isNormal;
            }
            
            if (args[1].Trim() == "1")
                includeChildTeam = new VariousData<bool>(true);
            else if (args[1].Trim() == "0")
                includeChildTeam = new VariousData<bool>(false);

            string strTeamName = args[0].Trim();
            bool result = true;
            
            if (nArgumentCount == 2)
                result = SendSMS(strTeamName, args[1].Trim());
            else if (nArgumentCount == 3)
                result = SendSMS(strTeamName, includeChildTeam, args[2].Trim());
            else if (nArgumentCount == 4)
                result = SendSMS(strTeamName, args[1].Trim(), args[2].Trim(), args[3].Trim());
            else if (nArgumentCount == 5)
            {
                if (includeChildTeam == null)
                    result = SendSMS(strTeamName, args[1].Trim(), args[2].Trim(), args[3].Trim(), args[4].Trim());
                else
                    result = SendSMS(strTeamName, includeChildTeam, args[2].Trim(), args[3].Trim(), args[4].Trim());
            }
            else if (nArgumentCount == 6)
                result = SendSMS(strTeamName, includeChildTeam, args[2].Trim(), args[3].Trim(), args[4].Trim(), args[5].Trim());
            
            return result;
        }
        private int GetActionStepHistoryID()
        {
            System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
            string strFileName = process.ProcessName + ".aid";
            
            if (File.Exists(strFileName))
            {
                StreamReader reader = new StreamReader(strFileName, Encoding.UTF8);
                string strActionStepID = reader.ReadLine().Trim();
                reader.Close();

                int nActionStepHistoryID;

                if (int.TryParse(strActionStepID, out nActionStepHistoryID))
                    return nActionStepHistoryID;

                File.Delete(strFileName);
            }
            return -1;
        }

        private bool ReadConfig(string strName, out int value)
        {
            string strValue = System.Configuration.ConfigurationManager.AppSettings[strName].ToString().Trim();
            return int.TryParse(strValue, out value);
        }

        private bool SendMessage(MessageContent message)
        {
            return m_netMgr.SendSMS(message.Caller, message.PhoneNumbers, message.Message);
        }

        private bool SendMessage(List<MessageContent> messages)
        {
            if (messages.Count == 0)
                return false;

            bool result = true;

            foreach (MessageContent message in messages)
            {
                if (m_netMgr.SendSMS(message.Caller, message.PhoneNumbers, message.Message) == false)
                    result = false;
            }

            return result;
        }

        private bool SendSMS(string strTeamName, string strMessage)
        {
            MessageContent message = GetSMSInfo(strTeamName, strMessage, false);

            if (message == null)
                return false;

            return SendMessage(message);
        }

        private bool SendSMS(string strTeamName, VariousData<bool> includeChildTeam, string strMessage)
        {
            if (includeChildTeam == null)
                return false;

            MessageContent message = GetSMSInfo(strTeamName, strMessage, includeChildTeam.Data);

            if (message == null)
                return false;

            return SendMessage(message);
        }

        private string ParseLocation(string strLocation)
        {
            if (strLocation.StartsWith("'"))
                strLocation = strLocation.Substring(1);

            if (strLocation.EndsWith("'"))
                strLocation = strLocation.Substring(0, strLocation.Length - 1);

            return strLocation;
        }

        private bool SendSMS(string strTeamNameHeader, string strTeamNameOption, string strLocation, string strMessage)
        {
            m_strLocation = ParseLocation(strLocation);
            List<MessageContent> messages = GetSMSInfo(strTeamNameHeader, strTeamNameOption, m_strFloorManagerTag, strLocation, strMessage, false);

            if (messages == null)
                return false;

            return SendMessage(messages);
        }

        private bool SendSMS(string strTeamNameHeader, VariousData<bool> includeChildTeam, string strTeamNameOption, string strLocation, string strMessage)
        {
            m_strLocation = ParseLocation(strLocation);

            if (includeChildTeam == null)
                return false;

            List<MessageContent> messages = GetSMSInfo(strTeamNameHeader, strTeamNameOption, m_strFloorManagerTag, strLocation, strMessage, includeChildTeam.Data);

            if (messages == null)
                return false;

            return SendMessage(messages);
        }

        private bool SendSMS(string strTeamNameHeader, string strTeamNameOption, string strFloorManagerTag, string strLocation, string strMessage)
        {
            m_strLocation = ParseLocation(strLocation);

            List<MessageContent> messages = GetSMSInfo(strTeamNameHeader, strTeamNameOption, strFloorManagerTag, strLocation, strMessage, false);

            if (messages == null)
                return false;

            return SendMessage(messages);
        }

        private bool SendSMS(string strTeamNameHeader, VariousData<bool> includeChildTeam, string strTeamNameOption, string strFloorManagerTag, string strLocation, string strMessage)
        {
            m_strLocation = ParseLocation(strLocation);
            List<MessageContent> messages = GetSMSInfo(strTeamNameHeader, strTeamNameOption, strFloorManagerTag, strLocation, strMessage, includeChildTeam.Data);

            if (messages == null)
                return false;

            return SendMessage(messages);
        }

        private bool GetFloorIndex(string strLocation, out int nFloorIndex)
        {
            nFloorIndex = 0;
            bool underground = false;

            if (strLocation.Contains("지하"))
                underground = true;

            int nIndex = strLocation.LastIndexOf("층");

            if (nIndex < 0)
                return false;

            int num = 0;
            int nTimes = 1;

            for (int i=nIndex-1;i>=0;i--)
            {
                char ch = strLocation.ElementAt(i);

                if (ch >= '0' && ch <= '9')
                {
                    num += nTimes * (int)(ch - '0');
                    nTimes *= 10;
                }
                else
                    break;
            }

            if (underground)
                nFloorIndex = -num;
            else
                nFloorIndex = num - 1;

            return true;
        }

        private MessageContent GetSMSInfo(string strTeamName, string strMessage, bool includeChildTeams)
        {
            string strSender = GetSender();
            List<string> phoneNumbers = GetPhoneNumbers(strTeamName, includeChildTeams);

            if (phoneNumbers == null || phoneNumbers.Count == 0)
                return null;

            MessageContent message = new MessageContent();

            message.Caller = strSender;
            message.PhoneNumbers.AddRange(phoneNumbers);
            message.Message = strMessage;

            return message;
        }

        private List<MessageContent> GetSMSInfo(string strTeamNameHeader, string strTeamNameOption, string strFloorManagerTag, string strLocation, string strMessage, bool includeChildTeams)
        {
            string strTag = "{location}";
            string strLower = strMessage.ToLower();

            int nIndex = strLower.IndexOf(strTag);

            while (nIndex >= 0)
            {
                if (nIndex == 0)
                    strMessage = strLocation + strMessage.Substring(strTag.Length);
                else
                    strMessage = strMessage.Substring(0, nIndex) + strLocation + strMessage.Substring(nIndex + strTag.Length);

                strLower = strMessage.ToLower();
                nIndex = strLower.IndexOf(strTag);
            }

            string strSender = GetSender();

            int nFloorIndex;

            if (GetFloorIndex(strLocation, out nFloorIndex) == false)
                return null;

            Building building = ZoneManager.GetBuilding(strLocation);

            if (building == null)
                return null;

            List<string> teamNameOptions = GetTeamNameOptions(strTeamNameOption, building, nFloorIndex, strFloorManagerTag);

            if (teamNameOptions == null || teamNameOptions.Count == 0)
                return null;

            List<MessageContent> messages = new List<MessageContent>();

            foreach (string option in teamNameOptions)
            {
                string strTeamName = strTeamNameHeader + option;
                List<string> phoneNumbers = GetPhoneNumbers(strTeamName, includeChildTeams);

                if (phoneNumbers == null || phoneNumbers.Count == 0)
                    return null;

                MessageContent message = new MessageContent();

                message.Caller = strSender;
                message.PhoneNumbers.AddRange(phoneNumbers);
                message.Message = strMessage;

                messages.Add(message);
            }

            return messages;
        }

        private List<string> GetTeamNameOptions(string strTeamNameOption, Building building, int nFloorIndex, string strFloorManagerTag)
        {
            List<string> teamNameOptions = new List<string>();
            string[] tokens = strTeamNameOption.Split(',');

            int nFloorIndex1, nFloorIndex2;

            foreach (string strToken in tokens)
            {
                int nIndex = strToken.IndexOf('-');

                if (nIndex >= 0)
                {
                    string str1 = strToken.Substring(0, nIndex).Trim();
                    string str2 = strToken.Substring(nIndex + 1).Trim();

                    // 빈문자열일 경우 building의 최하위층까지 선택한다.
                    if (str1.Length == 0)
                        nFloorIndex1 = building.MinFloor;
                    else
                    {
                        if (GetFloorIndex(str1, nFloorIndex, out nFloorIndex1) == false)
                            return null;
                    }

                    // 빈문자열일 경우 building의 최상위층까지 선택한다.
                    if (str2.Length == 0)
                        nFloorIndex2 = building.MaxFloor;
                    else
                    {
                        if (GetFloorIndex(str2, nFloorIndex, out nFloorIndex2) == false)
                            return null;
                    }

                    /*if (GetFloorIndex(str1, nFloorIndex, out nFloorIndex1) == false ||
                        GetFloorIndex(str2, nFloorIndex, out nFloorIndex2) == false)
                        return null;*/

                    AddTeamNameOption(building, nFloorIndex1, nFloorIndex2, strFloorManagerTag, teamNameOptions);
                }
                else
                {
                    if (GetFloorIndex(strToken.Trim(), nFloorIndex, out nFloorIndex1) == false)
                        return null;

                    AddTeamNameOption(building, nFloorIndex1, strFloorManagerTag, teamNameOptions);
                }
            }

            return teamNameOptions;
        }

        private void AddTeamNameOption(Building building, int nFloorIndex, string strFloorManagerTag, List<string> teamNameOptions)
        {
            if (nFloorIndex < building.MinFloor || nFloorIndex > building.MaxFloor)
                return;

            string strFloor = nFloorIndex < 0 ? string.Format("지하 {0}", -nFloorIndex) : (nFloorIndex + 1).ToString();
            strFloor = strFloorManagerTag.Replace("#", strFloor);
            teamNameOptions.Add(strFloor);
        }

        private void AddTeamNameOption(Building building, int nFloorIndex1, int nFloorIndex2, string strFloorManagerTag, List<string> teamNameOptions)
        {
            int n1, n2;

            if (nFloorIndex1 < nFloorIndex2)
            {
                n1 = nFloorIndex1;
                n2 = nFloorIndex2;
            }
            else
            {
                n1 = nFloorIndex2;
                n2 = nFloorIndex1;
            }

            string strFloor;

            for (int i=n1;i<=n2;i++)
            {
                if (i < building.MinFloor || i > building.MaxFloor)
                    continue;

                if (i < 0)
                    strFloor = string.Format("지하 {0}", -i);
                else
                    strFloor = (i + 1).ToString();

                strFloor = strFloorManagerTag.Replace("#", strFloor);
                teamNameOptions.Add(strFloor);
            }
        }

        private bool GetFloorIndex(string strTag, int nFloorIndex, out int nResultFloorIndex)
        {
            nResultFloorIndex = 0;

            if (strTag.StartsWith("{") == false || strTag.EndsWith("}") == false)
                return false;

            strTag = strTag.Substring(1, strTag.Length - 2);

            int nAdd = 0;
            int nIndex = strTag.LastIndexOf('+');

            if (nIndex > 0)
            {
                if (int.TryParse(strTag.Substring(nIndex + 1).Trim(), out nAdd) == false)
                    return false;
            }
            else
            {
                nIndex = strTag.LastIndexOf('-');

                if (nIndex > 0)
                {
                    if (int.TryParse(strTag.Substring(nIndex + 1).Trim(), out nAdd) == false)
                        return false;
                    else
                        nAdd = -nAdd;
                }
            }

            if (nIndex > 0)
                strTag = strTag.Substring(0, nIndex).Trim();

            if (strTag.ToLower() == "currentfloor")
            {
                nResultFloorIndex = nFloorIndex + nAdd;
                return true;
            }

            return false;
        }

        private string GetSender()
        {
            return "";
        }

        private List<string> GetPhoneNumbers(string strTeamName, bool includeChildTeams)
        {
            TeamManager mgr = new TeamManager();
            return mgr.GetTeamPhoneNumbers(strTeamName, m_isNormal, includeChildTeams, m_dbMgr);
        }

        private bool GetActionStepInfo(out bool isNormal)
        {
            isNormal = true;

            string strSQL = "Select ash.ID, step.ID, ash.RealMode, v.isNormal from ActionStepHistory as ash, ActionStep as step, Disaster as d, Version as v ";
            strSQL += "where ash.ActionStepID = step.ID and step.DisasterID = d.ID and d.VersionID = v.ID and ash.ID = " + m_nActionStepHistoryID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 4)
                return false;

            VariousData<int> normal = WebDBManager.GetIntField(arrResult[3].ToString());

            if (normal != null)
            {
                isNormal = normal.Data == 1;
                return true;
            }

            return false;
        }

        private class MessageContent
        {
            private string m_strCaller = "";
            private List<string> m_phoneNumbers = new List<string>();
            private string m_strMessage = "";

            public string Caller
            {
                get { return m_strCaller; }
                set { m_strCaller = value; }
            }

            public List<string> PhoneNumbers
            {
                get { return m_phoneNumbers; }
            }

            public string Message
            {
                get { return m_strMessage; }
                set { m_strMessage = value; }
            }
        }
    }
}
