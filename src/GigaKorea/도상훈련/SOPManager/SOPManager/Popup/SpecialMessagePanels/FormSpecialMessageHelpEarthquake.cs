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
    public partial class FormSpecialMessageHelpEarthquake : Form
    {
        public enum VariableType { Magnitude = 0, Intensity, Epicenter };

        // 규모
        private const string EARTHQUAKE_MAGNITUDE = "{earthq_magnit}";
        // 진도
        private const string EARTHQUAKE_INTENSITY = "{earthq_intens}";
        // 진앙
        private const string EARTHQUAKE_EPICENTER = "{earthq_epicenter}";

        public FormSpecialMessageHelpEarthquake()
        {
            InitializeComponent();

            WriteMessage();
            // Cursor 위치를 제일 처음으로 둔다.
            richTextBox1.Select(0, 0);
        }

        public static string GetVariableString(VariableType type)
        {
            if (type == VariableType.Magnitude)
                return EARTHQUAKE_MAGNITUDE;
            else if (type == VariableType.Intensity)
                return EARTHQUAKE_INTENSITY;
            else if (type == VariableType.Epicenter)
                return EARTHQUAKE_EPICENTER;

            return "";
        }

        private void WriteMessage()
        {
            string strTitle = "[지진정보 입력 방법]\r\n";

            string strMessage = GetVariableString(VariableType.Magnitude) + " : 지진 규모\r\n";
            strMessage += GetVariableString(VariableType.Intensity) + " : 지진 진도\r\n";
            strMessage += GetVariableString(VariableType.Epicenter) + " : 진앙지";

            AppendText(richTextBox1, strTitle, new Font("나눔스퀘어", 12.0f, FontStyle.Bold), 10);
            AppendText(richTextBox1, strMessage, new Font("나눔스퀘어", 11.0f));
        }

        public static void AppendText(RichTextBox rtb, string strText, Font font, int nRowHeight = 0)
        {
            int nPos = 0;

            if (rtb.Tag != null && rtb.Tag is int)
            {
                nPos = (int)rtb.Tag - 1;

                if (nPos < 0)
                    nPos = 0;
            }

            rtb.AppendText(strText);
            rtb.SelectionStart = nPos;
            rtb.SelectionLength = strText.Length;
            rtb.SelectionFont = font;
            rtb.SelectionCharOffset = nRowHeight;

            nPos += strText.Length;
            rtb.Tag = nPos;
        }

        public static void GetParameters(List<SOPParameter> parameters, string strCategoryName, string strSubCategoryName)
        {
            if (strCategoryName == "지진" || strSubCategoryName == "지진")
            {
                SOPParameter param = new SOPParameter();
                param.VariableName = "earthq_magnit";
                param.Type = Sections.SectionDataDecision.VariableType.DOUBLE;
                param.Description = "지진 규모";
                parameters.Add(param);

                param = new SOPParameter();
                param.VariableName = "earthq_intens";
                param.Type = Sections.SectionDataDecision.VariableType.INTEGER;
                param.Description = "지진 진도";
                parameters.Add(param);

                param = new SOPParameter();
                param.VariableName = "earthq_epicenter";
                param.Type = Sections.SectionDataDecision.VariableType.STRING;
                param.Description = "지진 진앙지";
                parameters.Add(param);
            }
        }
    }
}
