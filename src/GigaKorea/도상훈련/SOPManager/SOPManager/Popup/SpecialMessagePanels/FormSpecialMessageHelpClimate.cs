using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPManager.Popup.SpecialMessagePanels
{
    public partial class FormSpecialMessageHelpClimate : Form
    {
        public enum VariableType { SNOW_DEPTH = 0 };

        // 적설량(cm)
        private const string SNOW_DEPTH = "{snow_depth}";
        
        public FormSpecialMessageHelpClimate()
        {
            InitializeComponent();
            WriteMessage();
            // Cursor 위치를 제일 처음으로 둔다.
            richTextBox1.Select(0, 0);
        }

        public static string GetVariableString(VariableType type)
        {
            if (type == VariableType.SNOW_DEPTH)
                return SNOW_DEPTH;

            return "";
        }

        public static string PureVariable(string strVariable)
        {
            if (strVariable.StartsWith("{") && strVariable.EndsWith("}"))
                return strVariable.Substring(1, strVariable.Length - 2).Trim();

            return strVariable;
        }

        public static string IgnoreCaseVariableString(string strVariable, string strAdd)
        {
            string strPureVariable = PureVariable(strVariable);
            return "• " + strPureVariable + strAdd + " 대소문자 구분하지 않음";
        }

        private void WriteMessage()
        {
            string strTitle = "[기후정보 입력 방법]\r\n";

            string strSnowDepth = GetVariableString(VariableType.SNOW_DEPTH);

            string strMessage = strSnowDepth + " : 적설량(cm)\r\n";
            strMessage += IgnoreCaseVariableString(strSnowDepth, "는");

            FormSpecialMessageHelpEarthquake.AppendText(richTextBox1, strTitle, new Font("나눔스퀘어", 12.0f, FontStyle.Bold), 10);
            FormSpecialMessageHelpEarthquake.AppendText(richTextBox1, strMessage, new Font("나눔스퀘어", 11.0f));
        }

        public static void GetParameters(List<SOPParameter> parameters, string strCategoryName, string strSubCategoryName)
        {
            if (strCategoryName == "폭설" || strSubCategoryName == "폭설")
            {
                SOPParameter param = new SOPParameter();
                param.VariableName = "snow_depth";
                param.Type = Sections.SectionDataDecision.VariableType.DOUBLE;
                param.Description = "적설량(cm)";
                parameters.Add(param);
            }
        }
    }
}
