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
    public partial class FormSpecialMessageHelpPSM : Form
    {
        public enum VariableType { PSMMaterial = 0, PSMDistanceM, PSMDistanceKM };

        // 유해화학물질
        private const string PSM_MATERIAL = "{PSMMaterial}";
        // 대피거리(미터)
        private const string PSM_DISTANCE_M = "{PSMDistanceM}";
        // 대피거리(킬로미터)
        private const string PSM_DISTANCE_KM = "{PSMDistanceKM}";

        public FormSpecialMessageHelpPSM()
        {
            InitializeComponent();
            WriteMessage();
            // Cursor 위치를 제일 처음으로 둔다.
            richTextBox1.Select(0, 0);
        }

        public static string GetVariableString(VariableType type)
        {
            if (type == VariableType.PSMMaterial)
                return PSM_MATERIAL;
            else if (type == VariableType.PSMDistanceM)
                return PSM_DISTANCE_M;
            else if (type == VariableType.PSMDistanceKM)
                return PSM_DISTANCE_KM;

            return "";
        }

        private void WriteMessage()
        {
            string strTitle = "[유해화학물질 입력 방법]\r\n";
            string strPSMMaterial = GetVariableString(VariableType.PSMMaterial);

            string strMessage = strPSMMaterial + " : 유해화학물질\r\n";
            string strMessage2 = FormSpecialMessageHelpClimate.IgnoreCaseVariableString(strPSMMaterial, "은") + "\r\n";//"• PSMMaterial은 대소문자 구분하지 않음\r\n";

            Font fontTitle = new Font("나눔스퀘어", 12.0f, FontStyle.Bold);
            Font fontNormal = new Font("나눔스퀘어", 11.0f);

            FormSpecialMessageHelpEarthquake.AppendText(richTextBox1, strTitle, fontTitle, 10);
            FormSpecialMessageHelpEarthquake.AppendText(richTextBox1, strMessage, fontNormal);
            FormSpecialMessageHelpEarthquake.AppendText(richTextBox1, strMessage2, fontNormal, 12);

            string strPSMDistanceM = GetVariableString(VariableType.PSMDistanceM);
            string strPSMDistanceKM = GetVariableString(VariableType.PSMDistanceKM);
            string strPureM = FormSpecialMessageHelpClimate.PureVariable(strPSMDistanceM);
            string strPureKM = FormSpecialMessageHelpClimate.PureVariable(strPSMDistanceKM);

            strTitle = "[유해화학물질 누출시 대피거리 입력 방법]\r\n";
            strMessage = strPSMDistanceM + " : 대피거리(미터)\r\n";
            strMessage += strPSMDistanceKM + " : 대피거리(킬로미터)\r\n";
            strMessage += "• " + strPureM + " 및 " + strPureKM + "은 대소문자 구분하지 않음";

            FormSpecialMessageHelpEarthquake.AppendText(richTextBox1, strTitle, fontTitle, 10);
            FormSpecialMessageHelpEarthquake.AppendText(richTextBox1, strMessage, fontNormal);
        }

        public static void GetParameters(List<SOPParameter> parameters, string strCagtegoryName, string strSubCategoryName)
        {
            if (strCagtegoryName == "유출사고")
            {
                SOPParameter param = new SOPParameter();
                param.VariableName = "PSMMaterial";
                param.Type = Sections.SectionDataDecision.VariableType.STRING;
                param.Description = "유해화학물질";
                parameters.Add(param);

                param = new SOPParameter();
                param.VariableName = "PSMDistanceM";
                param.Type = Sections.SectionDataDecision.VariableType.INTEGER;
                param.Description = "유해화학물질 누출시 대피거리(미터)";
                parameters.Add(param);

                param = new SOPParameter();
                param.VariableName = "PSMDistanceKM";
                param.Type = Sections.SectionDataDecision.VariableType.DOUBLE;
                param.Description = "유해화학물질 누출시 대피거리(킬로미터)";
                parameters.Add(param);
            }
        }
    }
}
