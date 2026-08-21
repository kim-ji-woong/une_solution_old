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
    public partial class FormSpecialMessageHelpAlarm : Form
    {
        public enum VariableType { ALARM_MESSAGE = 0 };

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

        public FormSpecialMessageHelpAlarm()
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

        private void WriteMessage()
        {
            Font fontNormal = new System.Drawing.Font("맑은 고딕", 11.0f);

            string strTitle = "[알람과 관련된 정보]\r\n";
            FormSpecialMessageHelpEarthquake.AppendText(richTextBox1, strTitle, new Font("맑은 고딕", 12.0f, FontStyle.Bold), 10);

            string strMessage = GetVariableString(VariableType.ALARM_MESSAGE) + " : 알람관련 메시지\r\n";
            FormSpecialMessageHelpEarthquake.AppendText(richTextBox1, strMessage, fontNormal);
        }

        public static string GetVariableString(VariableType type)
        {
            if (type == VariableType.ALARM_MESSAGE)
                return "{AlarmMessage}";

            return "";
        }

        public static void GetParameters(List<SOPParameter> parameters)
        {
        }
    }
}
