using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;

namespace BuildingSMS
{
    public class SMSManager
    {
        public static bool GetReceivers(WebDBManager dbMgr, bool isNormal, List<string> teamTags, Building building, List<int> floorList, out string strReceiverDescription, out List<string> phoneNumbers)
        {
            Dictionary<string, string> dicPhoneNumbers = new Dictionary<string, string>();
            TeamSMS.TeamManager teamMgr = new TeamSMS.TeamManager();

            string strTeamNames = "";
            string strFloors = "";

            foreach (int nFloorIndex in floorList)
            {
                string strFloorName = nFloorIndex < 0 ? string.Format("지하{0}층", -nFloorIndex) : string.Format("{0}층", nFloorIndex + 1);

                if (strFloors.Length == 0)
                    strFloors = strFloorName;
                else
                    strFloors += ", " + strFloorName;

                foreach (string strTeamTag in teamTags)
                {
                    string strTeamName = strTeamTag.Replace("b#", building.BuildingName);
                    strTeamName = strTeamName.Replace("f#", strFloorName);

                    List<string> numbers = teamMgr.GetTeamPhoneNumbers(strTeamName, isNormal, false, dbMgr);

                    if (numbers != null)
                    {
                        foreach (string strPhoneNumber in numbers)
                        {
                            if (strPhoneNumber.Length > 0)
                                dicPhoneNumbers[strPhoneNumber] = strPhoneNumber;
                        }
                    }

                    //if (strTeamNames.Length == 0)
                    //    strTeamNames = strTeamName + "\r\n";
                    //else
                        strTeamNames += "∙ " + strTeamName + "\r\n";
                }
            }

            phoneNumbers = new List<string>();

            if (strTeamNames.Length == 0)
            {
                strReceiverDescription = "[수신자]\r\n총 0명";
                return true;
            }

            foreach (KeyValuePair<string, string> pair in dicPhoneNumbers)
            {
                phoneNumbers.Add(pair.Value);
            }

            strReceiverDescription = string.Format("[수신자]\r\n총 {0}명\r\n\r\n[담당층]\r\n{1}\r\n\r\n[담당팀]\r\n{2}", phoneNumbers.Count, strFloors, strTeamNames);
            return true;
        }

        public static bool SendSMS(List<string> phoneNumbers, string strMessage, int nSiteID)
        {
            return Network.NetworkManager.Instance.SendSMS(GetSender(), phoneNumbers, strMessage);
        }

        private static string GetSender()
        {
            return "";
        }
    }
}
