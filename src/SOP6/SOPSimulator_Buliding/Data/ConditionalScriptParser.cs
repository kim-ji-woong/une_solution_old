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
            // 원래 '<'나 '>'은 '='보다 왼쪽에 위치해야 하지만
            // 개발자가 아닌 일반인들의 사용을 고려할때 엄격한 규칙을 요구하긴 힘들다.
            strStatement = strStatement.Replace("=<", "<=");
            strStatement = strStatement.Replace("=>", ">=");

            // 포함구문에 대한 처리
            ContainsToLike(ref strStatement);

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

        public static void ContainsToLike(ref string strStatement)
        {
            string strLower = strStatement.ToLower();
            string strTag = "contains";

            int nIndex = strLower.IndexOf(strTag);

            while (nIndex >= 0)
            {
                int nIndex1 = strStatement.IndexOf('\'', nIndex);

                if (nIndex1 < 0)
                    break;

                int nIndex2 = strStatement.IndexOf('\'', nIndex1 + 1);

                if (nIndex2 < 0)
                    break;

                string strAfter = strStatement.Substring(nIndex2 + 1).Trim();

                strStatement = strStatement.Substring(0, nIndex) + "like " + "'%" + strStatement.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1) + "%'";
                strStatement += " " + strAfter;
                strLower = strStatement.ToLower();

                if (strLower.Length <= nIndex2 + 1)
                    break;

                nIndex = strLower.IndexOf(strTag, nIndex2 + 1);
            }
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

                // 변수명은 모두 소문자로 만든다.
                // 변수명과 중괄호 사이에 빈칸은 없앤다.
                string strVariable = strToken.Substring(0, nIndex1);
                ReshapeVariable(ref strVariable);

                string strType = strToken.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                dicVariableTypes[strVariable] = SectionDataDecision.ToVariableType(strType);
            }

            return dicVariableTypes;
        }

        // 1. 변수 앞뒤에 빈칸이 있을 경우 없앤다.
        //    { abc } < 3 => {abc} < 3
        // 2. 변수를 모두 소문자로 바꾼다.
        //    {Cafe유엔이} Contains '유엔' => {cafe유엔이} Contains '유엔'
        public static string ReshapeExpression(string strExpression)
        {
            string strNewExpression = "";
            int nIndex = strExpression.IndexOf('{');

            if (nIndex > 0)
                strNewExpression = strExpression.Substring(0, nIndex);

            if (nIndex >= 0)
            {
                while (true)
                {
                    int nIndex2 = strExpression.IndexOf('}', nIndex + 1);

                    if (nIndex2 < 0)
                    {
                        strNewExpression += strExpression.Substring(nIndex);
                        break;
                    }

                    string strVariableName = strExpression.Substring(nIndex + 1, nIndex2 - nIndex - 1).Trim();
                    string strVariableNameLow = strVariableName.ToLower();
                    strNewExpression += "{" + strVariableNameLow + "}";

                    nIndex = strExpression.IndexOf('{', nIndex2 + 1);

                    if (nIndex < 0)
                    {
                        if (nIndex2 + 1 < strExpression.Length)
                            strNewExpression += strExpression.Substring(nIndex2 + 1);
                        break;
                    }
                    else
                        strNewExpression += strExpression.Substring(nIndex2 + 1, nIndex - nIndex2 - 1);
                }
            }
            else
                strNewExpression = strExpression;

            return strNewExpression;
        }

        // 1. 변수 앞뒤에 빈칸이 있을 경우 없앤다.
        //    { abc } => {abc}
        // 2. 변수를 모두 소문자로 바꾼다.
        //    { Cafe유엔이} => {cafe유엔이}
        private static void ReshapeVariable(ref string strVariable)
        {
            strVariable = strVariable.Substring(1, strVariable.Length - 2).Trim();
            strVariable = "{" + strVariable.ToLower() + "}";
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
                {
                    data.ExpressionOrigin = strAutoRunScript;
                    data.Expression = strAutoRunScript;
                }
            }
            else
            {
                //string strScript = ReshapeExpression(strAutoRunScript);
                Dictionary<string, Sections.SectionDataDecision.VariableType> dicVariableTypes = DecisionDataHelper.GetVariableTypes(strVariableTypes);

                // 유효성 검사
                // 모든 변수에 대하여 검사
                /*foreach (KeyValuePair<string, Sections.SectionDataDecision.VariableType> pair in dicVariableTypes)
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

                if (strError.Length == 0)*/

                string strNewAutoRunScript = IsValidExpression(strAutoRunScript, dicVariableTypes, out strError);
                if (strError.Length == 0)                
                {
                    foreach (KeyValuePair<string, Sections.SectionDataDecision.VariableType> pair in dicVariableTypes)
                    {
                        data.VariableTypes[pair.Key] = pair.Value;
                    }

                    data.ExpressionOrigin = strNewAutoRunScript;
                    data.Expression = strNewAutoRunScript;
                }
            }
        }

        // 수식의 유효성 검사
        // dicVariableType의 Key는 모두 소문자이어야 한다.
        public static string IsValidExpression(string strExpression, Dictionary<string, Sections.SectionDataDecision.VariableType> dicVariableTypes, out string strError)
        {
            string strScript = ReshapeExpression(strExpression);

            strScript = IsValidExpression2(strExpression, dicVariableTypes);

            string strNewScript = strScript;

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

            ChangeBooleanType(ref strScript);

            strError = "";
            ConditionalScriptParser.Execute(strScript, out strError);

            return strNewScript;
        }

        private static void ChangeBooleanType(ref string str)
        {
            string[] keys = new string[] { "참", "true", "거짓", "false" };
            string[] values = new string[] { "1", "1", "0", "0" };
            string strLower = str.ToLower();

            for (int i = 0; i < keys.Count(); i++)
            {
                int nBeginIndex = 0;

                while (nBeginIndex < str.Length)
                {
                    int nIndex = FindExpressionWordIndex(strLower, keys[i], nBeginIndex);

                    if (nIndex >= 0)
                    {
                        str = str.Substring(0, nIndex) + values[i] + str.Substring(nIndex + keys[i].Length);
                        strLower = str.ToLower();
                        nBeginIndex = nIndex + keys[i].Length;
                    }
                    else
                        break;
                }
            }
        }

        private static int FindExpressionWordIndex(string str, string strWord, int nBeginIndex)
        {
            int nIndex = str.IndexOf(strWord, nBeginIndex);

            if (nIndex < 0)
                return -1;

            char chBegin = (char)0;
            char chEnd = (char)0;
            int nWordLen = strWord.Length;

            if (nIndex > 0)
                chBegin = str.ElementAt(nIndex - 1);

            if (nIndex + nWordLen < str.Length)
                chEnd = str.ElementAt(nIndex + nWordLen);

            if (!CheckExpressionCharacter(chBegin) || !CheckExpressionCharacter(chEnd))
                return -1;

            return nIndex;
        }

        private static bool CheckExpressionCharacter(char ch)
        {
            if (ch == (char)0 || ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n' || ch == '<' ||
                ch == '>' || ch == '=' || ch == '*' || ch == '/' || ch == '+' || ch == '-' || ch == '(' || ch == ')')
                return true;

            return false;
        }

        /// <summary>
        /// 수식 유효성 검사2 (수식에 대한 값이 있는지 여부 검사)
        /// ex : {몇명} = 5 and {갇혔는가} -> {갇혔는가} 수식에 대한 값이 없으므로 채워준다
        /// </summary>
        /// <returns></returns>
        private static string IsValidExpression2(string strExpression, Dictionary<string, Sections.SectionDataDecision.VariableType> dicVariableTypes)
        {
            strExpression = strExpression.ToLower();
            foreach (KeyValuePair<string, SectionDataDecision.VariableType> item in dicVariableTypes)
            {
                if (item.Value != SectionDataDecision.VariableType.BOOLEAN)
                    continue;

                if (!strExpression.Contains(item.Key))
                    continue;

                int nBeginIndex = 0;

                while (true)
                {
                    int nIndex = FindExpressionWordIndex(strExpression, item.Key, nBeginIndex);
                    if (nIndex >= 0)
                    {
                        bool bEquals = true;
                        if (strExpression.Length < nIndex + item.Key.Length + 1 || strExpression.Substring(nIndex + item.Key.Length).Trim().Substring(0, 1) != "=")
                            bEquals = false;

                        if (!bEquals)
                        {
                            strExpression = strExpression.Insert(nIndex + item.Key.Length, " = true ");
                            nBeginIndex += (item.Key + " = true ").Length;
                        }
                        else
                        {
                            nBeginIndex += item.Key.Length;
                        }
                    }
                    else
                    {

                        break;
                    }
                }
            }

            return strExpression;
        } 
    }
}
