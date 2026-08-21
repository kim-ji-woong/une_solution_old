using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sections
{
    public class SectionDataDecision : SectionData
    {
        public enum VariableType { UNKNOWN = 0, BOOLEAN, DOUBLE, INTEGER, STRING };

        // Default 문자열을 사용하여 작성된 ID 개수
        protected static Dictionary<string, int> DEFAULT_ID_COUNT = new Dictionary<string, int>();

        // Key : 변수명
        // Value : 변수 Type
        protected Dictionary<string, VariableType> m_dicVariableTypes = new Dictionary<string, VariableType>();
        public Dictionary<string, VariableType> VariableTypes
        {
            get { return m_dicVariableTypes; }
        }

        public static void ClearIDCount()
        {
            DEFAULT_ID_COUNT.Clear();
        }

        public override void SetDefaultID(string strStepName, string strTeamName)
        {
            MakeDefaultID(strStepName, strTeamName, DEFAULT_ID_COUNT, "Decision");
        }

        protected override void AddDefaultID(string strTag, int nTagCount)
        {
            DEFAULT_ID_COUNT[strTag] = nTagCount;
        }

        // nTagCount가 strTag에 대한 최대값이면 최대값을 1만큼 낮춰준다.
        protected override void RemoveMaxDefaultCount(string strTag, int nTagCount)
        {
            if (DEFAULT_ID_COUNT.ContainsKey(strTag))
            {
                if (DEFAULT_ID_COUNT[strTag] == nTagCount)
                    DEFAULT_ID_COUNT[strTag] = nTagCount - 1;
            }
        }

        public static VariableType ToVariableType(string strType)
        {
            strType = strType.ToLower();

            if (strType == "int" || strType == "integer" || strType == "정수")
                return Sections.SectionDataDecision.VariableType.INTEGER;
            else if (strType == "double" || strType == "실수")
                return Sections.SectionDataDecision.VariableType.DOUBLE;
            else if (strType == "string" || strType == "문자열")
                return Sections.SectionDataDecision.VariableType.STRING;
            else if (strType == "bool" || strType == "boolean" || strType == "참/거짓")
                return Sections.SectionDataDecision.VariableType.BOOLEAN;

            return VariableType.UNKNOWN;
        }

        public static VariableType ToVariableType(int nType)
        {
            foreach (VariableType type in Enum.GetValues(typeof(VariableType)))
            {
                if (nType == (int)type)
                    return type;
            }

            return VariableType.UNKNOWN;
        }

        public static string GetVariableTypeName(Sections.SectionDataDecision.VariableType type, bool isKoreanWord = true)
        {
            switch (type)
            {
                case Sections.SectionDataDecision.VariableType.INTEGER:
                    return isKoreanWord ? "정수" : "integer";

                case Sections.SectionDataDecision.VariableType.DOUBLE:
                    return isKoreanWord ? "실수" : "double";

                case Sections.SectionDataDecision.VariableType.STRING:
                    return isKoreanWord ? "문자열" : "string";

                case Sections.SectionDataDecision.VariableType.BOOLEAN:
                    return isKoreanWord ? "참/거짓" : "boolean";
            }

            return "";
        }
    }
}
