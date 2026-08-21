using System;
using System.Collections.Generic;
using AgentFactory.BLL;
using SDMS.Model.Broadcast;
using SDMS.Model.Sensor;
using SDMS.Model.Spatial;
using dnsData.Alarm;
using dnsData.Sensor;
using dnsData.Script;
using Common.Model;

namespace SOPWebServer.BLL.Process
{
    public class BroadcastManager : BaseBroadcastManager
    {
        private MainManager m_mainManager = null;

        public BroadcastManager(Factory factory, MainManager mainManager)
            : base(factory)
        {
            m_mainManager = mainManager;
            factory.BroadcastManager = this;
        }

        public override bool RunBroadcast(string strMessage, int nRepeatCount, bool useSiren)
        {
            if (strMessage == null || strMessage.Length == 0)
                return false;

            Broadcast broadcast = m_mainManager.SDMSDataManager.GetCreateManager().CreateBroadcast(strMessage, useSiren, (int)Broadcast.PlayType.PLAY, nRepeatCount, DateTime.Now, m_mainManager.SDMSDataManager.SiteID);
            return broadcast != null;
        }

        private string GetEarthquakeBroadcastMessage(AlarmData alarm, EarthquakeOption option)
        {
            bool isNullable;
            string strCondition = Shelter.GetFieldName(Shelter.Fields.SiteID, out isNullable);

            string strErrorMessage;
            List<Shelter> shelters = m_mainManager.CommonDataManager.GetSelectManager().SelectShelters(strCondition, out strErrorMessage);

            string strShelterName = "대피소";

            if (shelters != null)
            {
                foreach (Shelter shelter in shelters)
                {
                    if (shelter.ShelterType == (int)Shelter.ShelterTypes.Earthquake)
                    {
                        strShelterName = shelter.ShelterName;
                        break;
                    }
                }
            }

            string strData = "";

            // 지진일 경우 Param4는 지진 세기를 나타낸다.
            if (alarm.ReactionHistoryParam4.Length > 0)
            {
                for (int i = 0; i < alarm.ReactionHistoryParam4.Length; i++)
                {
                    char ch = alarm.ReactionHistoryParam4[i];

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
        public override string GetBroadcastMessage(AlarmData alarm, SDMS.Model.Config.Broadcast.SituationTypes type, out int nRepeatCount, out bool useSiren)
        {
            nRepeatCount = 0;
            useSiren = false;

            if (type == SDMS.Model.Config.Broadcast.SituationTypes.DETECT_EARTHQUAKE)
            {
                if (alarm.Tag != null && alarm.Tag is EarthquakeOption)
                {
                    nRepeatCount = 1;
                    useSiren = true;
                    EarthquakeOption option = (EarthquakeOption)alarm.Tag;
                    return GetEarthquakeBroadcastMessage(alarm, option);
                }
                else
                    return "";
            }

            Dictionary<SDMS.Model.Config.Broadcast.Fields, object> dicConditions = new Dictionary<SDMS.Model.Config.Broadcast.Fields, object>();
            dicConditions[SDMS.Model.Config.Broadcast.Fields.SituationType] = (int)type;
            dicConditions[SDMS.Model.Config.Broadcast.Fields.SiteID] = m_mainManager.SDMSDataManager.SiteID;

            string strErrorMessage;
            List<SDMS.Model.Config.Broadcast> configs = m_mainManager.SDMSDataManager.GetSelectManager().SelectBroadcastConfigs(dicConditions, "", out strErrorMessage);

            if (configs == null || configs.Count == 0)
                return "";

            SDMS.Model.Config.Broadcast config = configs[0];

            if (config.UseBroadcast == false || config.Message.Length == 0)
                return "";

            nRepeatCount = config.RepeatCount;
            useSiren = config.UseSiren;

            SensorZone sensorZone = m_mainManager.SensorManager.GetSensorZone(alarm.SensorZoneID);

            if (sensorZone == null)
                return "";

            EquipmentZone equipZone = m_mainManager.SensorManager.GetEquipmentZone(sensorZone.EquipZoneID);

            string strLocationName = "";

            if (equipZone != null)
            {
                if (equipZone != null)
                    strLocationName = equipZone.BroadcastText;
            }

            string strBroadcastMessage = "";

            if (type == SDMS.Model.Config.Broadcast.SituationTypes.DETECT_FIRE || type == SDMS.Model.Config.Broadcast.SituationTypes.REPORT_FIRE)
                strBroadcastMessage = GetBroadcastMessage(config.Message, strLocationName, alarm.TimeStamp, nRepeatCount);
            else if (type == SDMS.Model.Config.Broadcast.SituationTypes.DETECT_PSM || type == SDMS.Model.Config.Broadcast.SituationTypes.REPORT_PSM)
            {
                // 대피거리(미터)
                //int nPSMDistance;
                //string strPSMMaterialName = GetPSMInfo(m_mainManager.SDMSDataManager, m_mainManager.CommonDataManager, alarm, sensorZone, out nPSMDistance);
                string strPSMMaterialName = GetPSMInfo(m_mainManager.SDMSDataManager, m_mainManager.CommonDataManager, alarm, sensorZone);

                if (strPSMMaterialName.Length > 0)
                {
                    //strBroadcastMessage = GetPSMBroadcastMessage(config.Message, strLocationName, alarm.TimeStamp, strPSMMaterialName, nPSMDistance, nRepeatCount);
                    strBroadcastMessage = GetPSMBroadcastMessage(config.Message, strLocationName, alarm.TimeStamp, strPSMMaterialName, nRepeatCount);
                }
                else
                    strBroadcastMessage = config.Message;
            }

            return strBroadcastMessage;
        }

        // Return 값 : 유해화학물질 이름
        // nPSMDistance : 대피거리(미터)
        //private static string GetPSMInfo(SDMS.IDAL.IDataManager sdmsDataManager, Common.IDAL.IDataManager commonDataManager, AlarmData alarm, SensorZone sensorZone, out int nPSMDistance)
        //{
        //    string materialName = Facility.GetNFacilityTypeString(sensorZone.SensorType);
        //    nPSMDistance = 0;

        //    string strErrorMessage;
        //    Dictionary<Material.Fields, object> dicCondition = new Dictionary<Material.Fields, object>();
        //    dicCondition.Add(Material.Fields.FacilityTypeID, sensorZone.SensorType);
        //    List<Material> materials = sdmsDataManager.GetSelectManager().SelectPSMMaterials(dicCondition, "", out strErrorMessage);

        //    if (materials != null || materials.Count > 0)
        //    {
        //        Material material = materials[0];

        //        int nAlarmDepth = alarm.AlarmDepth;

        //        if (nAlarmDepth == 1 && material.EvacInitDistance != null)
        //            nPSMDistance = (int)material.EvacInitDistance;
        //        else if (nAlarmDepth == 2 || nAlarmDepth == 3)
        //        {
        //            if (IsDayLight(commonDataManager, alarm.TimeStamp))
        //            {
        //                if (material.EvacDayDistance != null)
        //                    nPSMDistance = (int)material.EvacDayDistance;
        //            }
        //            else
        //            {
        //                if (material.EvacNightDistance != null)
        //                    nPSMDistance = (int)material.EvacNightDistance;
        //            }
        //        }
        //    }

        //    return materialName;
        //}
        private static string GetPSMInfo(SDMS.IDAL.IDataManager sdmsDataManager, Common.IDAL.IDataManager commonDataManager, AlarmData alarm, SensorZone sensorZone)
        {
            string materialName = "";

            string strErrorMessage;
            string strAdditionalConditions = string.Format("{0}.{1} = {2}", PSM.TableName, PSM.Fields.ID, sensorZone.OrgSensorID);
            System.Collections.ArrayList arrDatas = sdmsDataManager.GetSelectManager().JoinPSMSensorMaterial(strAdditionalConditions, out strErrorMessage);

            if (arrDatas != null)
            {
                int nDataCount = arrDatas.Count;

                for (int i = 0; i < nDataCount - 1; i += 2)
                {
                    if (arrDatas[i] is PSM && arrDatas[i + 1] is Material)
                    {
                        PSM psm = (PSM)arrDatas[i];
                        Material material = (Material)arrDatas[i + 1];

                        materialName = material.MaterialName;

                        break;
                    }
                }
            }

            return materialName;
        }

        public static bool IsDayLight(Common.IDAL.IDataManager dataManager, DateTime time)
        {
            string strErrorMessage;
            List<Common.Model.Option.Options> beginOptions = dataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SOPSimulator, "WorkingBeginHour", out strErrorMessage);
            List<Common.Model.Option.Options> endOptions = dataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SOPSimulator, "WorkingEndHour", out strErrorMessage);

            if (beginOptions == null || endOptions == null ||
                beginOptions.Count == 0 || endOptions.Count == 0 ||
                beginOptions[0].PropertyValue == null ||
                endOptions[0].PropertyValue == null)
                return false;

            string strBeginHour = beginOptions[0].PropertyValue.Trim();
            string strEndHour = endOptions[0].PropertyValue.Trim();

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
        //private static string GetPSMBroadcastMessage(string strOriginMessage, string strLocation, DateTime time, string strPSMMaterialName, int nPSMDistance, int nRepeatCount)
        private static string GetPSMBroadcastMessage(string strOriginMessage, string strLocation, DateTime time, string strPSMMaterialName, int nRepeatCount)
        {
            string szBroadcastMessage;
            string strRepeatMessage = GetMessage(strOriginMessage, "<<", ">>", out szBroadcastMessage);

            for (int j = 0; j < nRepeatCount; j++)
            {
                szBroadcastMessage += "...\n다시한번 알려드립니다...";
                szBroadcastMessage += strRepeatMessage;
            }

            //szBroadcastMessage = ParseSpecialMessage(strOriginMessage, time, strLocation, strPSMMaterialName, nPSMDistance);
            szBroadcastMessage = ParseSpecialMessage(strOriginMessage, time, strLocation, strPSMMaterialName);
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
            dnsData.Script.SOP.DataParameter param = new dnsData.Script.SOP.DataParameter(strOriginMessage, time, strLocation);
            return dnsData.Script.SOP.Parse(param);
        }

        private static string ParseSpecialMessage(string strOriginMessage, DateTime time, string strLocation, string strPSMMaterialName)
        {
            dnsData.Script.SOP.DataParameter param = new dnsData.Script.SOP.DataParameter(strOriginMessage, time, strLocation);
            param.AddData("PSMMaterialType", strPSMMaterialName);

            return dnsData.Script.SOP.Parse(param);
        }

        private static string ParseSpecialMessage(string strOriginMessage, DateTime time, string strLocation, string strPSMMaterialName, int nPSMDistance)
        {
            dnsData.Script.SOP.DataParameter param = new dnsData.Script.SOP.DataParameter(strOriginMessage, time, strLocation);
            param.AddData("PSMMaterialType", strPSMMaterialName);
            param.AddData("PSMDistance", nPSMDistance);

            return dnsData.Script.SOP.Parse(param);
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
