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

        private double m_WindowWidthRate = 1d;
        public double WindowWidthRate
        {
            get { return m_WindowWidthRate; }
            set { m_WindowWidthRate = value; }
        }

        private double m_WindowHeightRate = 1d;
        public double WindowHeightRate
        {
            get { return m_WindowHeightRate; }
            set { m_WindowHeightRate = value; }
        }

        public FormSpecialMessageHelpLocation()
        {
            InitializeComponent();
            WriteMessage();
            // Cursor 위치를 제일 처음으로 둔다.
            richTextBox1.Select(0, 0);
        }

        public void UpdateControl()
        {
            UpdateWindowRate(this, WindowWidthRate, WindowHeightRate);
            UpdateWindowRate(richTextBox1, WindowWidthRate, WindowHeightRate);
        }

        public void UpdateWindowRate(Control ctl, double pWindowRateWidth, double pWindowRateHeight, String pFontFamily = "맑은 고딕")
        {
            if (ctl is Form || ctl.GetType().Name == "Form")
            {
                ctl.Size = new System.Drawing.Size((int)(ctl.Size.Width * pWindowRateWidth), (int)(ctl.Size.Height * pWindowRateHeight));
            }
            else if (ctl is RichTextBox || ctl.GetType().Name == "RichTextBox")
            {
                double fLabelFontSize = ctl.Font.Size * pWindowRateWidth;
                ctl.Font = new Font(pFontFamily, (float)fLabelFontSize, ctl.Font.Style);
                ctl.Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
            }
            else
            {
                return;
            }

            ctl.Location = new Point((int)(ctl.Location.X * pWindowRateWidth), (int)(ctl.Location.Y * pWindowRateHeight));
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

        public static void GetParameters(List<SOPParameter> parameters, string strCagtegoryName, string strSubCategoryName)
        {
            bool location = false;

            if (strCagtegoryName == "자연재해")
            {
                if (strSubCategoryName == "지진" || strSubCategoryName == "폭설" ||
                    strSubCategoryName == "침수")
                    location = true;
            }
            else if (strCagtegoryName == "화재")
            {
                location = true;
            }
            else if (strCagtegoryName == "유출사고")
            {
                location = true;
            }
            else if (strCagtegoryName == "테러")
            {
                location = true;
            }
            else if (strCagtegoryName.Contains("인명구조"))
            {
                location = true;
            }
            else if (strCagtegoryName.Contains("기타"))
            {
                location = true;
            }
            else if (strCagtegoryName.Contains("폭발"))
            {
                location = true;
            }

            if (location)
            {
                SOPParameter param = new SOPParameter();
                param.VariableName = "location";
                param.Type = Sections.SectionDataDecision.VariableType.STRING;
                param.Description = "재난 발생 위치";

                parameters.Add(param);
            }
        }
    }
}
