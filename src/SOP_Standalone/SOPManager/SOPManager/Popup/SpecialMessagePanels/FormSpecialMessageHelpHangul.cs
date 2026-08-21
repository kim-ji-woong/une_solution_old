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
    public partial class FormSpecialMessageHelpHangul : Form
    {
        public enum VariableType { 은_는 = 0, 는_은,  이_가, 가_이, 을_를, 를_을, 과_와, 와_과 };

        public FormSpecialMessageHelpHangul()
        {
            InitializeComponent();

            WriteMessage();
            // Cursor 위치를 제일 처음으로 둔다.
            richTextBox1.Select(0, 0);
        }

        public static string GetVariableString(VariableType type)
        {
            if (type == VariableType.은_는)
                return "{은는}";
            else if (type == VariableType.는_은)
                return "{는은}";
            else if (type == VariableType.이_가)
                return "{이가}";
            else if (type == VariableType.가_이)
                return "{가이}";
            else if (type == VariableType.을_를)
                return "{을를}";
            else if (type == VariableType.를_을)
                return "{를을}";
            else if (type == VariableType.과_와)
                return "{과와}";
            else if (type == VariableType.와_과)
                return "{와과}";

            return "";
        }

        private void WriteMessage()
        {
            Font fontNormal = new System.Drawing.Font("나눔스퀘어", 11.0f);

            string strTitle = "[한글 받침유무에 따른 조사 입력 방법]\r\n";
            FormSpecialMessageHelpEarthquake.AppendText(richTextBox1, strTitle, new Font("나눔스퀘어", 12.0f, FontStyle.Bold), 10);

            string strMessage = GetVariableString(VariableType.은_는) + " : 바로 앞 글자가 한글이면서 받침이 있으면 '은', 없으면 '는'으로 표시\r\n           • 한글이 아닐 경우에는 '은'으로 표시\r\n";
            strMessage += GetVariableString(VariableType.는_은) + " : 바로 앞 글자가 한글이면서 받침이 있으면 '은', 없으면 '는'으로 표시\r\n           • 한글이 아닐 경우에는 '는'으로 표시\r\n\r\n";
            
            strMessage += GetVariableString(VariableType.이_가) + " : 바로 앞 글자가 한글이면서 받침이 있으면 '이', 없으면 '가'으로 표시\r\n           • 한글이 아닐 경우에는 '이'으로 표시\r\n";
            strMessage += GetVariableString(VariableType.가_이) + " : 바로 앞 글자가 한글이면서 받침이 있으면 '이', 없으면 '가'으로 표시\r\n           • 한글이 아닐 경우에는 '가'으로 표시\r\n\r\n";

            strMessage += GetVariableString(VariableType.을_를) + " : 바로 앞 글자가 한글이면서 받침이 있으면 '을', 없으면 '를'으로 표시\r\n           • 한글이 아닐 경우에는 '을'으로 표시\r\n";
            strMessage += GetVariableString(VariableType.를_을) + " : 바로 앞 글자가 한글이면서 받침이 있으면 '을', 없으면 '를'으로 표시\r\n           • 한글이 아닐 경우에는 '를'으로 표시\r\n\r\n";

            strMessage += GetVariableString(VariableType.과_와) + " : 바로 앞 글자가 한글이면서 받침이 있으면 '과', 없으면 '와'으로 표시\r\n           • 한글이 아닐 경우에는 '과'으로 표시\r\n";
            strMessage += GetVariableString(VariableType.와_과) + " : 바로 앞 글자가 한글이면서 받침이 있으면 '과', 없으면 '와'으로 표시\r\n           • 한글이 아닐 경우에는 '와'으로 표시";
            FormSpecialMessageHelpEarthquake.AppendText(richTextBox1, strMessage, fontNormal);
        }

        public static void GetParameters(List<SOPParameter> parameters)
        {
        }
    }
}
