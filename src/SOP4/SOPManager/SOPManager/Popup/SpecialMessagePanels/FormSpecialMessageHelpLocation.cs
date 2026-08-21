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
    public partial class FormSpecialMessageHelpLocation : Form
    {
        public enum VariableType { Location = 0 };

        // 재난 발생 장소
        private const string LOCATION = "{location}";

        public FormSpecialMessageHelpLocation()
        {
            InitializeComponent();
            WriteMessage();
            // Cursor 위치를 제일 처음으로 둔다.
            richTextBox1.Select(0, 0);
        }

        public static string GetVariableString(VariableType type)
        {
            if (type == VariableType.Location)
                return LOCATION;

            return "";
        }

        private void WriteMessage()
        {
            string strTitle = "[재난 발생 장소 입력 방법]\r\n";

            string strLocation = GetVariableString(VariableType.Location);
            
            string strMessage = strLocation + " : 재난 발생 장소\r\n";
            strMessage += FormSpecialMessageHelpClimate.IgnoreCaseVariableString(strLocation, "은");

            FormSpecialMessageHelpEarthquake.AppendText(richTextBox1, strTitle, new Font("맑은 고딕", 12.0f, FontStyle.Bold), 10);
            FormSpecialMessageHelpEarthquake.AppendText(richTextBox1, strMessage, new Font("맑은 고딕", 11.0f));
        }
    }
}
