using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisisAlertTester.Data
{
    public enum FacilityType { NONE = 0, FIRE_SENSOR = 1, FLOOD_SENSOR, HEAT_SENSOR, COLLAPSE_SENSOR };

    public class CommonString
    {
        public const string RiskLevel_Kor = "위기경보 단계";

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
        public const string FacilityType_Collapse = "Fire";
        public const string FacilityType_Collapse_Kor = "경사지 붕괴";
    }

}
