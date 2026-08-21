using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.SOP.Workstate;

namespace SectionContents.Utility
{
    public class DecisionProcessButton : IComparable
    {
        public enum YesNo { No = 0, Yes, Unknown };

        private string m_strText = "";
        private ProcessButton m_btn = null;

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }

        public ProcessButton ProcessButton
        {
            get { return m_btn; }
            set { m_btn = value; }
        }

        public YesNo Decision
        {
            get
            {
                string strText = this.Text.ToLower();

                if (strText.Contains("(예)") || strText.Contains("(네)") || strText.Contains("(yes)"))
                    return YesNo.Yes;
                else if (strText.Contains("(아니오)") || strText.Contains("(no)"))
                    return YesNo.No;

                return YesNo.Unknown;
            }
        }

        public DecisionProcessButton()
        {
        }

        public DecisionProcessButton(string strText, ProcessButton btn)
        {
            m_strText = strText;
            m_btn = btn;
        }

        public int CompareTo(object obj)
        {
            DecisionProcessButton btn = (DecisionProcessButton)obj;
            string thisText = this.Text.ToLower();
            string btnText = btn.Text.ToLower();

            if (this.Text.Length == 0)
            {
                if (btn.Text.Length == 0)
                    return 0;
                else
                    return 1;
            }
            else if (this.Text.Contains("(예)") || this.Text.Contains("(네)") || thisText.Contains("(yes)"))
            {
                if (btn.Text.Contains("(예)") || btn.Text.Contains("(네)") || btnText.Contains("(yes)"))
                    return 0;
                else
                    return -1;
            }
            else if (this.Text.Contains("(아니오)") || thisText.Contains("(no)"))
            {
                if (btn.Text.Contains("(아니오)") || btnText.Contains("(no)"))
                    return 0;
                else if (btn.Text.Length == 0)
                    return -1;
                else
                    return 1;
            }

            int thisNumber, btnNumber;

            // 둘다 숫자일 경우 숫자로 비교한다.
            if (GetNumber(this.Text, out thisNumber) && GetNumber(btn.Text, out btnNumber))
            {
                return thisNumber.CompareTo(btnNumber);
            }

            return this.Text.CompareTo(btn.Text);
        }

        private bool GetNumber(string strText, out int num)
        {
            int len = strText.Length;
            num = -1;

            for (int i = 0; i < len; i++)
            {
                char ch = strText.ElementAt(i);

                if (ch >= '0' && ch <= '9')
                {
                    if (num < 0)
                        num = ch - '0';
                    else
                        num = num * 10 + ch - '0';
                }
                else
                    break;
            }

            return num >= 0;
        }

        public string TextToString
        {
            get { return string.Format("[{0}]로 분기", this.Text); ; }
        }

        public override string ToString()
        {
            return string.Format("[{0}]로 분기", this.Text);
        }
    }
}
