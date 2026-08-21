using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sections;

namespace SOPMonitoringSystem
{
    public class ConditionalScriptParser
    {
        // 조건문을 실행하여 그 결과를 알려줍니다.
        public static bool Execute(string strStatement, out string strError)
        {
            strError = "";

            // == => =
            // || => or
            // && => and
            // != => <>
            // ! => not
            strStatement = strStatement.Replace("&&", "and");
            strStatement = strStatement.Replace("||", "or");
            strStatement = strStatement.Replace("!=", "<>");
            strStatement = strStatement.Replace("==", "=");
            strStatement = strStatement.Replace("!", "not ");

            try
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                object result = dt.Compute(strStatement, "");

                if (result != null && result is bool)
                    return (bool)result;
            }
            catch (Exception e)
            {
                strError = e.Message;
            }

            return false;
        }
    }

    public class DecisionDataHelper
    {
        public static Dictionary<string, SectionDataDecision.VariableType> GetVariableTypes(string strVariableTypes)
        {
            Dictionary<string, SectionDataDecision.VariableType> dicVariableTypes = new Dictionary<string, SectionDataDecision.VariableType>();
            string[] tokens = strVariableTypes.Split(';');

            foreach (string strToken in tokens)
            {
                int nIndex1 = strToken.IndexOf('(');
                int nIndex2 = strToken.IndexOf(')');

                if (nIndex1 < 0 || nIndex2 < nIndex1)
                    continue;

                string strVariable = strToken.Substring(0, nIndex1);
                string strType = strToken.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                dicVariableTypes[strVariable] = SectionDataDecision.ToVariableType(strType);
            }

            return dicVariableTypes;
        }

        // strAutoRunScript와 strVariableTypes의 유효성을 검사하여 data에 그 값을 할당한다.
        public static void SetDecisionExpression(Sections.SectionDataDecision data, string strAutoRunScript, string strVariableTypes)
        {
            if (strAutoRunScript == null || strAutoRunScript.Length == 0)
                return;

            string strError = "";

            if (strVariableTypes == null || strVariableTypes.Length == 0)
            {
                // 유효성 검사
                // 변수가 없음
                ConditionalScriptParser.Execute(strAutoRunScript, out strError);

                if (strError.Length == 0)
                    data.Expression = strAutoRunScript;
            }
            else
            {
                string strScript = strAutoRunScript;
                Dictionary<string, Sections.SectionDataDecision.VariableType> dicVariableTypes = DecisionDataHelper.GetVariableTypes(strVariableTypes);

                // 유효성 검사
                // 모든 변수에 대하여 검사
                foreach (KeyValuePair<string, Sections.SectionDataDecision.VariableType> pair in dicVariableTypes)
                {
                    if (pair.Value == Sections.SectionDataDecision.VariableType.BOOLEAN)
                        strScript = strScript.Replace(pair.Key, "true");
                    else if (pair.Value == Sections.SectionDataDecision.VariableType.DOUBLE)
                        strScript = strScript.Replace(pair.Key, "1.0");
                    else if (pair.Value == Sections.SectionDataDecision.VariableType.INTEGER)
                        strScript = strScript.Replace(pair.Key, "1");
                    else if (pair.Value == Sections.SectionDataDecision.VariableType.STRING)
                        strScript = strScript.Replace(pair.Key, "'a'");
                }

                ConditionalScriptParser.Execute(strScript, out strError);

                if (strError.Length == 0)
                {
                    data.Expression = strAutoRunScript;

                    foreach (KeyValuePair<string, Sections.SectionDataDecision.VariableType> pair in dicVariableTypes)
                    {
                        data.VariableTypes[pair.Key] = pair.Value;
                    }
                }
            }
        }
    }
}
