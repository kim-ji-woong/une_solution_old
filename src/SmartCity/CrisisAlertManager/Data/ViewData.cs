using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisisAlertManager.Data
{
    public enum ContentOwnerTab { CRISIS_TAB = 0, REPORT_TAB, GROUP_TAB, ALARM_TAB, MANUAL_TAB };

    public enum FacilityType {NONE = 0, FIRE_SENSOR = 1, FLOOD_SENSOR, HEAT_SENSOR, COLLAPSE_SENSOR };
    public enum MessageType { MESSAGE = 0};

    public enum RiskLevel { Normal = 0, Attention, Caution, Alert, Serious }

    //public enum AfterFire { };


    public class CommonString
    {
        public const string RiskLevel_Kor = "위기경보 단계";
        public const string RiskLevel_Occur = "위기경보 발생";
        public const string RiskLevel_End = "상황종료";

        public const string RiskLevel_Normal = "Normal";
        public const string RiskLevel_Normal_Kor = "평시";
        public const string RiskLevel_Attention = "Attention";
        public const string RiskLevel_Attention_Kor = "관심";
        public const string RiskLevel_Caution = "Caution";
        public const string RiskLevel_Caution_Kor = "주의";
        public const string RiskLevel_Alert = "Alert";
        public const string RiskLevel_Alert_Kor = "경계";
        public const string RiskLevel_Serious = "Serious";
        public const string RiskLevel_Serious_Kor = "심각";

        public const string FacilityType_Fire = "Fire";
        public const string FacilityType_Fire_Kor = "화재";
        public const string FacilityType_Flood = "Flood";
        public const string FacilityType_Flood_Kor = "홍수";
        public const string FacilityType_Heat = "Heat";
        public const string FacilityType_Heat_Kor = "폭염";
        public const string FacilityType_Collapse = "Collapse";
        public const string FacilityType_Collapse_Kor = "경사지 붕괴";

        public const string FireSensor_AfterFire = "AfterFire";
        public const string FireSensor_AfterFire_Kor = "화재발생 2시간 경과";
        public const string FireSensor_AlarmPeriod = "AlarmPeriod";
        public const string FireSensor_AlarmPeriod_Kor = "화재 경보 발령 기간";
        public const string FireSensor_Weak = "Weak";
        public const string FireSensor_Weak_Kor = "화재 취약시기";
        public const string FireSensor_InitReact = "InitReact";
        public const string FireSensor_InitReact_Kor = "초기대응 실패 여부";

        public const string HeatSensor_MeasPeriod = "MeasPeriod";
        public const string HeatSensor_MeasPeriod_Kor = "폭염 대책기간 발령";
        public const string HeatSensor_PreliminaryDate = "PreliminaryDate";
        public const string HeatSensor_PreliminaryDate_Kor = "폭염 예비특보 발령";
        public const string HeatSensor_AdvisoryDate = "AdvisoryDate";
        public const string HeatSensor_AdvisoryDate_Kor = "폭염 주의보 발령";
        public const string HeatSensor_AlertDate = "AlertDate";
        public const string HeatSensor_AlertDate_Kor = "폭염 경보 발령";

        public const string Demander_Kor = "요구조자";
        public const string DeathTollr_Kor = "사망자";

        public const string Group_Select = "선택";

        public const string Report_Data_Kor = "데이터 변동 이력";
        public const string Report_Alert_Kor = "위기경보 변동 이력";
        public const string Report_SMS_Kor = "메시지 전송 이력";

        public const string Report_No = "No";
        public const string Report_Type = "유형";
        public const string Report_Time = "일시";
        public const string Report_DataName = "변동데이터 명";
        public const string Report_OriginData = "기존데이터";
        public const string Report_NewData = "변동데이터";
        public const string Report_Message = "전송된 메시지";
        public const string Report_Manager = "담당자";

        static public string GetRiskDataName (string strOldData, string strNewData)
        {
            string strRet = RiskLevel_Kor;

            if (strOldData == RiskLevel_Normal_Kor)
                strRet = RiskLevel_Occur;
            else if (strNewData == RiskLevel_Normal_Kor)
                strRet = RiskLevel_End;

            return strRet;
        }

        static public int GetRiskLevelIndex(string strRiskLevel)
        {
            int nRet = 0;

            if (strRiskLevel == RiskLevel_Normal)
                nRet = (int)RiskLevel.Normal;
            else if (strRiskLevel == RiskLevel_Attention)
                nRet = (int)RiskLevel.Attention;
            else if (strRiskLevel == RiskLevel_Caution)
                nRet = (int)RiskLevel.Caution;
            else if (strRiskLevel == RiskLevel_Alert)
                nRet = (int)RiskLevel.Alert;
            else if (strRiskLevel == RiskLevel_Serious)
                nRet = (int)RiskLevel.Serious;

            return nRet;
        }

        static public string GetRiskLevelString(int nRiskLevel)
        {
            string strRet = RiskLevel_Normal;

            if (nRiskLevel == (int)RiskLevel.Normal)
                strRet = RiskLevel_Normal;
            else if (nRiskLevel == (int)RiskLevel.Attention)
                strRet = RiskLevel_Attention;
            else if (nRiskLevel == (int)RiskLevel.Caution)
                strRet = RiskLevel_Caution;
            else if (nRiskLevel == (int)RiskLevel.Alert)
                strRet = RiskLevel_Alert;
            else if (nRiskLevel == (int)RiskLevel.Serious)
                strRet = RiskLevel_Serious;

            return strRet;
        }

        static public string GetRiskLevelKorToEng(string strRiskLevel)
        {
            string strRet = RiskLevel_Normal;

            if (strRiskLevel == RiskLevel_Normal_Kor)
                strRet = RiskLevel_Normal;
            else if (strRiskLevel == RiskLevel_Attention_Kor)
                strRet = RiskLevel_Attention;
            else if (strRiskLevel == RiskLevel_Caution_Kor)
                strRet = RiskLevel_Caution;
            else if (strRiskLevel == RiskLevel_Alert_Kor)
                strRet = RiskLevel_Alert;
            else if (strRiskLevel == RiskLevel_Serious_Kor)
                strRet = RiskLevel_Serious;

            return strRet;
        }
    }







}
