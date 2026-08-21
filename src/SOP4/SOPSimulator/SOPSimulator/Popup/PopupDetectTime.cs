using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPMonitoringSystem.Popup
{
    public partial class PopupDetectTime : Form
    {
        private DateTime m_dtDetect;
        public DateTime DetectTime
        {
            get { return m_dtDetect; }
        }

        public PopupDetectTime(DateTime dtTime)
        {
            InitializeComponent();
            SetDate(dtTime);
        }

        private void SetDate(DateTime dtTime)
        {
            textBoxYear.Text = dtTime.Year.ToString();
            textBoxMonth.Text = dtTime.Month.ToString();
            textBoxDay.Text = dtTime.Day.ToString();
            textBoxHour.Text = dtTime.Hour.ToString();
            textBoxMinute.Text = dtTime.Minute.ToString();
            textBoxSecond.Text = dtTime.Second.ToString();

            monthCalendar1.SetDate(dtTime);
            ShowCalendar(false);
        }

        private void ShowCalendar(bool visible)
        {
            monthCalendar1.Visible = visible;

            if (visible)
            {
                this.Size = new Size(433, this.Size.Height);
            }
            else
            {
                this.Size = new Size(202, this.Size.Height);
            }
        }

        private void btnCalendar_Click(object sender, EventArgs e)
        {
            ShowCalendar(true);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!CheckTimeValidation())
                return;

            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private bool CheckTimeValidation()
        {
            if (!CheckYear())
                return false;

            if (!CheckMonth())
                return false;

            if (!CheckDay())
                return false;

            if (!CheckHour())
                return false;

            if (!CheckMinute())
                return false;

            if (!CheckSecond())
                return false;

            int year = int.Parse(textBoxYear.Text);
            int month = int.Parse(textBoxMonth.Text);
            int day = int.Parse(textBoxDay.Text);
            int hour = int.Parse(textBoxHour.Text);
            int minute = int.Parse(textBoxMinute.Text);
            int second = int.Parse(textBoxSecond.Text);

            m_dtDetect = new DateTime(year, month, day, hour, minute, second);
            return true;
        }

        private bool CheckYear()
        {
            if (!NullCheck(textBoxYear, "년도를"))
                return false;

            if (!CheckRange(textBoxYear, 1000, 3000, "년도가"))
                return false;

            return true;
        }

        private bool CheckMonth()
        {
            if (!NullCheck(textBoxMonth, "월을"))
                return false;

            if (!CheckRange(textBoxMonth, 1, 12, "월이"))
                return false;

            return true;
        }

        private bool CheckDay()
        {
            if (!NullCheck(textBoxDay, "일자를"))
                return false;

            int min, max;
            GetMinMaxDate(out min, out max);

            if (!CheckRange(textBoxDay, min, max, "일자가"))
                return false;

            return true;
        }

        private bool CheckHour()
        {
            if (!NullCheck(textBoxHour, "시간을"))
                return false;

            if (!CheckRange(textBoxHour, 0, 23, "시간이"))
                return false;

            return true;
        }

        private bool CheckMinute()
        {
            if (!NullCheck(textBoxMinute, "분을"))
                return false;

            if (!CheckRange(textBoxMinute, 0, 59, "분이"))
                return false;

            return true;
        }

        private bool CheckSecond()
        {
            if (!NullCheck(textBoxSecond, "초를"))
                return false;

            if (!CheckRange(textBoxSecond, 0, 59, "초가"))
                return false;

            return true;
        }

        private void GetMinMaxDate(out int min, out int max)
        {
            min = max = 1;

            int year = int.Parse(textBoxYear.Text);
            int month = int.Parse(textBoxMonth.Text);

            if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12)
                max = 31;
            else if (month == 2)
            {
                if (IsLeapYear(year))
                    max = 29;
                else
                    max = 28;
            }
            else
                max = 30;
        }

        // 윤년인가?
        private bool IsLeapYear(int nYear)
        {
            if (nYear % 4 == 0)			// 4년에 한번씩 윤년
            {
                if (nYear % 100 == 0)	// 100년마다 윤년 건너뜀
                {
                    if (nYear % 400 == 0)
                        return true;	// 그러나, 400년째는 윤년 인정
                    else
                        return false;
                }
                else
                    return true;
            }

            return false;
        }

        private bool CheckRange(TextBox textBox, int min, int max, string strTag)
        {
            int nTime;

            if (!int.TryParse(textBox.Text, out nTime))
            {
                MessageBox.Show(string.Format("{0} 정수 형태가 아닙니다.\r\n{1}에서 {2} 사이의 값으로 지정해주세요", strTag, min, max));
                textBox.Focus();
                return false;
            }

            if (nTime < min || nTime > max)
            {
                MessageBox.Show(string.Format("{0} 범위를 벗어났습니다.\r\n{1}에서 {2} 사이의 값으로 지정해주세요", strTag, min, max));
                textBox.Focus();
                return false;
            }

            return true;
        }

        private bool NullCheck(TextBox textBox, string strTag)
        {
            if (textBox.Text.Length == 0)
            {
                MessageBox.Show(strTag + " 입력하세요");
                textBox.Focus();
                return false;
            }

            return true;
        }

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            m_dtDetect = e.Start;

            textBoxYear.Text = m_dtDetect.Year.ToString();
            textBoxMonth.Text = m_dtDetect.Month.ToString();
            textBoxDay.Text = m_dtDetect.Day.ToString();

            ShowCalendar(false);
        }
    }
}
