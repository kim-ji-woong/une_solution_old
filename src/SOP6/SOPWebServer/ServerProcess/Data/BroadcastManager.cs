using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgentFactory;
using DBUtility2;
using System.Collections;
using UnE.Spatial;

namespace ServerProcess.Data
{
    public class BroadcastManager : BaseBroadcastManager
    {
        public BroadcastManager(Factory factory)
            : base(factory)
        {
        }

        public override bool RunBroadcast(DirectDBManager dbMgr, string strMessage, int nRepeatCount, bool useSiren)
        {
            if (strMessage == null || strMessage.Length == 0)
                return false;

            DateTime timeStamp = DateTime.Now;
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", timeStamp.Year, timeStamp.Month, timeStamp.Day, timeStamp.Hour, timeStamp.Minute, timeStamp.Second);

            string szSQL = string.Format("INSERT INTO Broadcast (Text, UseSiren, PlayOption, RepeatCount, AddTime, SiteID) VALUES('{0}', {1}, {2}, {3},'{4}', {5})",
                strMessage, useSiren ? 1 : 0, 1, nRepeatCount, strTime, dbMgr.SiteID);

            if (dbMgr.GetResultData(szSQL) == null)
                return false;

            string szSQL2 = string.Format("INSERT INTO BroadcastHistory (Text, UseSiren, PlayOption, RepeatCount, HostInfo, AddTime,SiteID) VALUES('{0}', {1}, {2}, {3}, '{4}', '{5}', {6})",
                strMessage, useSiren ? 1 : 0, 1, nRepeatCount, "", strTime, dbMgr.SiteID);

            if (dbMgr.GetResultData(szSQL2) == null)
                return false;

            return true;
        }

        private string GetEarthquakeBroadcastMessage(DirectDBManager dbMgr, AlarmData alarm, UnE.Earthquake.EarthquakeOption option)
        {
            string szText = "SELECT ID, ShelterName, ShelterType, ShelterIDType, ShelterID, Boundary, Description FROM Shelter where SiteID = {0}";
            string strSQL = string.Format(szText, dbMgr.SiteID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            UnE.Spatial.ZoneManager.Instance.LoadShelters(arrResult);
            Dictionary<int, UnE.Spatial.Shelter> dicShelters = UnE.Spatial.ZoneManager.Instance.GetShelters(UnE.Spatial.Shelter.ShelterTypes.Earthquake);

            string strShelterName = "대피소";

            if (dicShelters != null)
            {
                foreach (KeyValuePair<int, UnE.Spatial.Shelter> pair in dicShelters)
                {
                    strShelterName = pair.Value.ShelterName;
                    break;
                }
            }

            string strData = "";

            if (alarm.ReactionHistoryParam4.Length > 0)
            {
                for (int i=0;i<alarm.ReactionHistoryParam4.Length;i++)
                {
                    char ch = alarm.ReactionHistoryParam4.ElementAt(i);

                    if (ch >= '0' && ch <= '9')
                    {
                        strData = alarm.ReactionHistoryParam4.Substring(i);
                        break;
                    }
                }
            }

            string strMessage = option.BroadcastMessage;

            if (strData.Length > 0)
            {
                ReplaceString("{INTENS}", strData, ref strMessage);
                ReplaceString("{MAGNIT}", strData, ref strMessage);
            }

            ReplaceString("{SHELTER}", strShelterName, ref strMessage);
            return strMessage;
        }

        // string.Replace()는 대소문자를 엄격히 구별하여 사용하여야 한다.
        // 대소문자 구별없이 같은 기능을 수행한다.
        private void ReplaceString(string strSrc, string strTrg, ref string strMessage)
        {
            int nSrcLen = strSrc.Length;
            strSrc = strSrc.ToLower();

            string strLow = strMessage.ToLower();

            int nIndex = 0;

            do
            {
                nIndex = strLow.IndexOf(strSrc, nIndex);

                if (nIndex >= 0)
                {
                    strLow = strLow.Substring(0, nIndex) + strTrg + strLow.Substring(nIndex + nSrcLen);
                    strMessage = strMessage.Substring(0, nIndex) + strTrg + strMessage.Substring(nIndex + nSrcLen);
                }
            }
            while (nIndex >= 0);
        }

        // 상황에 맞는 방송문구를 만든다.
        // nRepeatCount : 0이면 한번만 방송한다. 0보다 크면 한번 이상 반복 방송한다.
        // Return 값 : 빈 문자열이면 방송하지 않는다.
        public override string GetBroadcastMessage(DirectDBManager dbMgr, AlarmData alarm, SituationType type, out int nRepeatCount, out bool useSiren)
        {
            nRepeatCount = 0;
            useSiren = false;

            if (type == SituationType.DETECT_EARTHQUAKE)
            {
                if (alarm.Tag != null && alarm.Tag is UnE.Earthquake.EarthquakeOption)
                {
                    nRepeatCount = 1;
                    useSiren = true;
                    UnE.Earthquake.EarthquakeOption option = (UnE.Earthquake.EarthquakeOption)alarm.Tag;
                    return GetEarthquakeBroadcastMessage(dbMgr, alarm, option);
                }
                else
                    return "";
            }

            string szText = "select id, UseBroadcast, Message, UseSiren, RepeatCount from SDMSBroadcastConfig where SituationType = {0} and SiteID = {1}";
            string strSQL = string.Format(szText, (int)type, dbMgr.SiteID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null || arrResult.Count < 5)
                return "";

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
            bool useBroadcast = WebDBManager.GetIntField(arrResult[1].ToString(), 0) == 0 ? false : true;
            string strMessage = WebDBManager.GetStringField(arrResult[2]);
            useSiren = WebDBManager.GetIntField(arrResult[3].ToString(), 0) == 0 ? false : true;
            VariousData<int> repeatCount = WebDBManager.GetIntField(arrResult[4].ToString());

            if (id == null || strMessage == null || repeatCount == null)
                return "";

            if (useBroadcast == false || strMessage.Length == 0)
                return "";

            nRepeatCount = repeatCount.Data;

            SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(alarm.SensorZoneID);

            if (sensorZone == null)
                return "";

            EquipmentZone equipZone = sensorZone.EquipZone;

            string strLocationName = "";

            if (equipZone != null)
            {
                if (equipZone != null)
                    strLocationName = equipZone.BroadcastName;
            }

            string strBroadcastMessage = "";

            if (type == SituationType.DETECT_FIRE || type == SituationType.REPORT_FIRE)
                strBroadcastMessage = GetBroadcastMessage(strMessage, strLocationName, alarm.TimeStamp, nRepeatCount);
            else if (type == SituationType.DETECT_PSM || type == SituationType.REPORT_PSM)
            {
                // 대피거리(미터)
                int nPSMDistance;
                string strPSMMaterialName = GetPSMInfo(dbMgr, alarm, sensorZone, out nPSMDistance);

                if (strPSMMaterialName.Length > 0)
                    strBroadcastMessage = GetPSMBroadcastMessage(strMessage, strLocationName, alarm.TimeStamp, strPSMMaterialName, nPSMDistance, nRepeatCount);
                else
                    strBroadcastMessage = strMessage;
            }

            return strBroadcastMessage;
        }

        // Return 값 : 유해화학물질 이름
        // nPSMDistance : 대피거리(미터)
        private static string GetPSMInfo(DirectDBManager dbMgr, AlarmData alarm, SensorZone sensorZone, out int nPSMDistance)
        {
            nPSMDistance = 0;
            
            UnE.PSM.PSMSensor sensor = PSMManager.Instance.GetSensor(sensorZone.LinkedSensorID);

            if (sensor == null)
                return "";

            UnE.PSM.PSMMaterial material = PSMManager.Instance.GetMaterial(sensor.MaterialType);
            //PSMMaterial material = sensor.GetLinkedMaterial();

            if (material == null)
                return "";

            if (sensor.LinkedTankList == null || sensor.LinkedTankList.Count == 0)
                return "";

            UnE.PSM.PSMTank tank = sensor.LinkedTankList[0];

            int nAlarmDepth = alarm.AlarmDepth;

            if (nAlarmDepth == 1)
                nPSMDistance = tank.EvacInitDistance;
            else if (nAlarmDepth == 2 || nAlarmDepth == 3)
            {
                if (IsDayLight(dbMgr, alarm.TimeStamp))
                    nPSMDistance = tank.EvacDayDistance;
                else
                    nPSMDistance = tank.EvacNightDistance;
            }

            return material.Name;
        }

        public static bool IsDayLight(DirectDBManager dbMgr, DateTime time)
        {
            string strSQL = "Select PropertyValue from OptionSOPSimulator where (PropertyName = 'WorkingBeginHour' or PropertyName = 'WorkingEndHour') and SiteID = " + dbMgr.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            string strBeginHour = null, strEndHour = null;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount; i++)
            {
                string strTime = WebDBManager.GetStringField(arrResult[i]);

                if (strTime != null && string.Compare(strTime, "WorkingBeginHour", true) == 0)
                    strBeginHour = strTime;
                else if (strTime != null && string.Compare(strTime, "WorkingEndHour", true) == 0)
                    strEndHour = strTime;
            }

            if (strBeginHour != null && strEndHour != null)
            {
                int nBeginHour = 0, nBeginMinute = 0, nEndHour = 0, nEndMinute = 0;

                if (GetWorkingHours(strBeginHour, ref nBeginHour, ref nBeginMinute) && GetWorkingHours(strEndHour, ref nEndHour, ref nEndMinute))
                {
                    if (time.Hour > nBeginHour)
                    {
                        if (time.Hour < nEndHour)
                            return true;
                        else if (time.Hour == nEndHour)
                            return time.Minute <= nEndMinute;
                    }
                    else if (time.Hour == nBeginHour)
                    {
                        if (time.Minute >= nBeginMinute)
                        {
                            if (time.Hour < nEndHour)
                                return true;
                            else if (time.Hour == nEndHour)
                                return time.Minute <= nEndMinute;
                        }
                    }
                }
            }

            return false;
        }

        private static bool GetWorkingHours(string strWorkingHours, ref int nHour, ref int nMinute)
        {
            int nIndex = strWorkingHours.IndexOf(':');

            if (nIndex < 0)
                return false;

            string strHour = strWorkingHours.Substring(0, nIndex);
            string strMinute = strWorkingHours.Substring(nIndex + 1);

            if (!int.TryParse(strHour, out nHour))
                return false;

            if (!int.TryParse(strMinute, out nMinute))
                return false;

            if (nHour < 0 || nHour > 23)
                return false;

            if (nMinute < 0 || nMinute > 59)
                return false;

            return true;
        }

        // nPSMDistance : 대피거리(미터)
        private static string GetPSMBroadcastMessage(string strOriginMessage, string strLocation, DateTime time, string strPSMMaterialName, int nPSMDistance, int nRepeatCount)
        {
            string szBroadcastMessage;
            string strRepeatMessage = GetMessage(strOriginMessage, "<<", ">>", out szBroadcastMessage);

            for (int j = 0; j < nRepeatCount; j++)
            {
                szBroadcastMessage += "...\n다시한번 알려드립니다...";
                szBroadcastMessage += strRepeatMessage;
            }

            szBroadcastMessage = ParseSpecialMessage(strOriginMessage, time, strLocation, strPSMMaterialName, nPSMDistance);
            return szBroadcastMessage;
        }

        private static string GetBroadcastMessage(string strOriginMessage, string strLocation, DateTime time, int nRepeatCount)
        {
            string szBroadcastMessage;
            string strRepeatMessage = GetMessage(strOriginMessage, "<<", ">>", out szBroadcastMessage);

            for (int j = 0; j < nRepeatCount; j++)
            {
                szBroadcastMessage += "...\n다시한번 알려드립니다...";
                szBroadcastMessage += strRepeatMessage;
            }

            szBroadcastMessage = ParseSpecialMessage(strOriginMessage, time, strLocation);
            //szBroadcastMessage = szBroadcastMessage.Replace("●", strLocation);
            return szBroadcastMessage;
        }

        private static string ParseSpecialMessage(string strOriginMessage, DateTime time, string strLocation)
        {
            UnE.SOP.Utility.SOPSimulatorScript.DataParameter param = new UnE.SOP.Utility.SOPSimulatorScript.DataParameter(strOriginMessage, time, strLocation);
            return UnE.SOP.Utility.SOPSimulatorScript.Parse(param);
        }

        private static string ParseSpecialMessage(string strOriginMessage, DateTime time, string strLocation, string strPSMMaterialName, int nPSMDistance)
        {
            UnE.SOP.Utility.SOPSimulatorScript.DataParameter param = new UnE.SOP.Utility.SOPSimulatorScript.DataParameter(strOriginMessage, time, strLocation);
            param.PSMMaterialType = strPSMMaterialName;
            param.PSMDistance = nPSMDistance;

            return UnE.SOP.Utility.SOPSimulatorScript.Parse(param);
        }

        // strBeginTag와 strEndTag로 둘러쌓인 부분을 제거한 문자열을 리턴한다.
        // strFullMessage : strBeginTag와 strEndTag를 포함한 문자열
        private static string GetMessage(string strOriginMessage, string strBeginTag, string strEndTag, out string strFullMessage)
        {
            int nLen = strOriginMessage.Length;
            int nIndex = 0;

            string strMessage = "";
            strFullMessage = "";
            int nBeginTagLength = strBeginTag.Length;
            int nEndTagLength = strEndTag.Length;

            while (nIndex < nLen)
            {
                int nIndex1 = strOriginMessage.IndexOf(strBeginTag, nIndex);

                if (nIndex1 < 0)
                {
                    strFullMessage += strOriginMessage.Substring(nIndex);
                    strMessage += strOriginMessage.Substring(nIndex);
                    break;
                }

                int len = nIndex1 - nIndex;

                if (len > 0)
                {
                    strFullMessage += strOriginMessage.Substring(nIndex, len);
                    strMessage += strOriginMessage.Substring(nIndex, len);
                }

                int nIndex2 = strOriginMessage.IndexOf(strEndTag, nIndex1 + nBeginTagLength);

                if (nIndex2 < 0)
                {
                    strFullMessage += strOriginMessage.Substring(nIndex);
                    strMessage += strOriginMessage.Substring(nIndex1);
                    break;
                }

                len = nIndex2 - (nIndex1 + nBeginTagLength);

                if (len > 0)
                    strFullMessage += strOriginMessage.Substring(nIndex1 + nBeginTagLength, len);

                nIndex = nIndex2 + nEndTagLength;
            }

            return strMessage;
        }
    }
}
