using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPGen
{
    public class SectionTimeTextBox : ZBobb.AlphaBlendTextBox
    {
        private SectionTimeText m_section = null;
        private bool m_showTag = true;
        private string m_strTag = "";

        public SectionTimeTextBox(SectionTimeText section)
        {
            InitializeComponent();
            //SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            m_section = section;
        }

        public void SetTag(string strTag)
        {
            m_strTag = strTag;
        }

        private void InitializeComponent()
        {
            this.KeyDown += new KeyEventHandler(this.OnKeyDown); ;
        }

        public static bool TextToTime(string strTime, string strTag, out int nHour, out int nMinute)
        {
            nHour = nMinute = 0;
            string strText = strTime;

            if (strTag.Length > 0)
            {
                if (strText.StartsWith(strTag))
                    strText = strText.Substring(strTag.Length);
            }

            int nIndex = strText.IndexOf(':');

            if (nIndex < 0)
            {
                System.Windows.Forms.MessageBox.Show("문자열 가운데 :가 존재하지 않습니다. 시간과 분의 구분자 :가 존재하여야 합니다.");
                return false;
            }

            string strHour = strText.Substring(0, nIndex);
            string strMinute = strText.Substring(nIndex + 1);

            if (strHour.Length > 0 && strHour[strHour.Length - 1] == 'h')
                strHour = strHour.Substring(0, strHour.Length - 1);
            if (strMinute.Length > 0 && strMinute[strMinute.Length - 1] == 'm')
                strMinute = strMinute.Substring(0, strMinute.Length - 1);

            if (strHour.Length == 0 || strMinute.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("시간과 분이 명확히 입력되어야 합니다.\r\n시간 : 0 또는 그 이상의 숫자, 분 : 0에서 59 사이의 숫자");
                return false;
            }

            try
            {
                nHour = Int32.Parse(strHour);
                nMinute = Int32.Parse(strMinute);
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("입력한 문자열 가운데 숫자로 변환할 수 없는 값이 존재합니다. :를 구분자로 하여 시간과 분을 숫자로 입력하여야 합니다.\r\n시간 : 0 또는 그 이상의 숫자, 분 : 0에서 59 사이의 숫자");
                return false;
            }

            return true;
        }

        protected bool TextValidCheck(out int nHour, out int nMinute)
        {
            return TextToTime(this.Text, m_strTag, out nHour, out nMinute);
        }

        protected void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int nHour, nMinute;

                if (!TextValidCheck(out nHour, out nMinute))
                {
                    this.Text = m_section.GetTimeString(this, false);
                    return;
                }

                int nHour2, nMinute2;
                m_section.GetTime(this, out nHour2, out nMinute2);

                if (nHour != nHour2 || nMinute != nMinute2)
                {
                    if (!m_section.ChangeTime(this, ref nHour, ref nMinute))
                        return;
                    m_section.SetTime(this, nHour, nMinute);
                }

                this.Text = m_section.GetTimeString(this, false);

                m_section.Select(false, null);
                m_section.GetParent().Refresh();

                m_showTag = true;
                //m_section.GetParent().Invalidate(m_section.InvalidateRectArea, true);
                //m_section.GetParent().Update();
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            if (m_showTag)
            {
                this.Text = m_section.GetTimeString(this, true);
                m_showTag = false;
            }

            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (!m_showTag)
            {
                int nHour, nMinute;

                if (!TextValidCheck(out nHour, out nMinute))
                {
                    this.Text = m_section.GetTimeString(this, false);
                    m_showTag = true;
                    return;
                }

                int nHour2, nMinute2;
                m_section.GetTime(this, out nHour2, out nMinute2);

                if (nHour != nHour2 || nMinute != nMinute2)
                {
                    if (!m_section.ChangeTime(this, ref nHour, ref nMinute))
                        return;
                    m_section.SetTime(this, nHour, nMinute);
                }

                this.Text = m_section.GetTimeString(this, false);
                m_showTag = true;
            }

            base.OnLostFocus(e);
        }
    }
}
