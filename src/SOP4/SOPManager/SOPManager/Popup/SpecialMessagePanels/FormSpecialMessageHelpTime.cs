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
    public partial class FormSpecialMessageHelpTime : Form
    {
        public enum VariableType { Time = 0 };

        private const string TIME = "{time}";

        public FormSpecialMessageHelpTime()
        {
            InitializeComponent();
            WriteMessage();
            // Cursor 위치를 제일 처음으로 둔다.
            richTextBox1.Select(0, 0);
        }

        public static string GetVariableString(VariableType type)
        {
            if (type == VariableType.Time)
                return TIME;
            
            return "";
        }

        private void WriteMessage()
        {
            Font fontNormal = new System.Drawing.Font("맑은 고딕", 11.0f);

            string strTitle = "[재난 발생 시각 입력 방법]\r\n";
            FormSpecialMessageHelpEarthquake.AppendText(richTextBox1, strTitle, new Font("맑은 고딕", 12.0f, FontStyle.Bold), 10);

            string strMessage = GetVariableString(VariableType.Time) + " : 재난 발생 시각\r\n";
            strMessage += "{time:option} : 재난 발생 시각을 어떤 단위로 표시할 것인가를 결정\r\n";
            strMessage += "option : Y(년도), M(월), D(일), h(시간), m(분), s(초)\r\n";
            strMessage += "예)\r\n";
            strMessage += "   {time:m} : 재난 발생 시각을 분만 표시\r\n";
            strMessage += "   {time:hm} : 재난 발생 시각을 시간과 분만 표시\r\n";
            FormSpecialMessageHelpEarthquake.AppendText(richTextBox1, strMessage, fontNormal);

            richTextBox1.Tag = (int)richTextBox1.Tag - 2;
            strMessage = "   {time:Dh} : 재난 발생 시각을 날짜와 시간만 표시\r\n";
            FormSpecialMessageHelpEarthquake.AppendText(richTextBox1, strMessage, fontNormal, 12);

            richTextBox1.Tag = (int)richTextBox1.Tag - 3;
            strMessage = "재난 발생 시각으로부터 경과된 시간은 '+'  또는 '-' 기호로 표시\r\n";
            strMessage += "예)\r\n";
            strMessage += "   {time:hm + 1h} : 재난 발생 시각으로부터 1시간 뒤\r\n";
            FormSpecialMessageHelpEarthquake.AppendText(richTextBox1, strMessage, fontNormal);

            richTextBox1.Tag = (int)richTextBox1.Tag - 2;
            strMessage = "   {time:hm + 1h + 2m + 3s} : 재난 발생 시각으로부터 1시간 2분 3초 뒤\r\n";
            FormSpecialMessageHelpEarthquake.AppendText(richTextBox1, strMessage, fontNormal, 12);

            richTextBox1.Tag = (int)richTextBox1.Tag - 1;
            strMessage = "• time은 대소문자 구분하지 않음\r\n";
            strMessage += "• option 문자들은 'time:' 이후에 붙여서 쓸것\r\n";
            strMessage += "• '+' 기호는 띄워쓰거나 붙여쓰기 모두 가능함";
            FormSpecialMessageHelpEarthquake.AppendText(richTextBox1, strMessage, fontNormal);
        }
    }
}
