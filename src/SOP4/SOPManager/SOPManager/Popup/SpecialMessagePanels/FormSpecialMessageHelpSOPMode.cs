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
    public partial class FormSpecialMessageHelpSOPMode : Form
    {
        public enum VariableType { SOPMode = 0, SOPFullMode };

        private const string SOP_MODE = "{SOPMode}";
        private const string SOP_FULL_MODE = "{SOPFullMode}";

        public FormSpecialMessageHelpSOPMode()
        {
            InitializeComponent();
            WriteMessage();
            // Cursor 위치를 제일 처음으로 둔다.
            richTextBox1.Select(0, 0);
        }

        public static string GetVariableString(VariableType type)
        {
            if (type == VariableType.SOPMode)
                return SOP_MODE;
            else if (type == VariableType.SOPFullMode)
                return SOP_FULL_MODE;

            return "";
        }

        private void WriteMessage()
        {
            string strTitle = "[SOP 모드 입력 방법]\r\n";
            string strSOPMode = GetVariableString(VariableType.SOPMode);
            string strSOPFullMode = GetVariableString(VariableType.SOPFullMode);

            string strMessage = strSOPMode + " : 실제상황이면 [실제], 훈련상황이면 [훈련]으로 표시\r\n";
            string strMessage2 = FormSpecialMessageHelpClimate.IgnoreCaseVariableString(strSOPMode, "는") + "\r\n";//"• SOPMode는 대소문자 구분하지 않음\r\n";

            Font fontTitle = new Font("맑은 고딕", 12.0f, FontStyle.Bold);
            Font fontNormal = new Font("맑은 고딕", 11.0f);

            FormSpecialMessageHelpEarthquake.AppendText(richTextBox1, strTitle, fontTitle, 10);
            FormSpecialMessageHelpEarthquake.AppendText(richTextBox1, strMessage, fontNormal);
            FormSpecialMessageHelpEarthquake.AppendText(richTextBox1, strMessage2, fontNormal, 12);

            strTitle = "[SOP FULL 모드 입력 방법]\r\n";
            strMessage = strSOPFullMode + " : 실제상황이면 [실제상황], 훈련상황이면 [훈련상황]으로 표시\r\n";
            strMessage += FormSpecialMessageHelpClimate.IgnoreCaseVariableString(strSOPFullMode, "는");//"• SOPFullMode는 대소문자 구분하지 않음";

            FormSpecialMessageHelpEarthquake.AppendText(richTextBox1, strTitle, fontTitle, 10);
            FormSpecialMessageHelpEarthquake.AppendText(richTextBox1, strMessage, fontNormal);
        }
    }
}
