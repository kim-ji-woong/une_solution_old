using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPManager
{
    public partial class PopupProcessTerm : Form
    {
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();

        public PopupProcessTerm()
        {
            InitializeComponent();

            InitYear();
            InitDate();
            InitTime();

            rdoUnUse.Checked = true;
            rdoUse.Visible = false;
            rdoUnUse.Visible = false;
            
            SetRadioImage();
        }

        private void InitYear()
        {
            for(int i = 2000; i < 3000; i++)
            {
                cboBeginYear.Items.Add(i);
                cboEndYear.Items.Add(i);
            }

            cboBeginYear.Text = DateTime.Now.Year.ToString();
            cboEndYear.Text = DateTime.Now.Year.ToString();
        }

        private void InitDate()
        {
            for(int i = 1; i <= 12; i++)
            {
                cboBeginMonth.Items.Add(i);
                cboEndMonth.Items.Add(i);
            }
            
            for(int i = 1; i<=31; i++)
            {
                cboBeginDay.Items.Add(i);
                cboEndDay.Items.Add(i);
            }

            cboBeginMonth.SelectedIndex = 0;
            cboEndMonth.SelectedIndex = 0;
            cboBeginDay.SelectedIndex = 0;
            cboEndDay.SelectedIndex = 0;
        }
        
        private void InitTime()
        {
            for(int i=0; i<=23; i++)
            {
                cboBeginHour.Items.Add(i);
                cboEndHour.Items.Add(i);
            }

            for (int i = 0; i <= 59; i++)
            {
                cboBeginMinute.Items.Add(i);
                cboEndMinute.Items.Add(i);
            }

            cboBeginHour.SelectedIndex = 0;
            cboEndHour.SelectedIndex = 0;
            cboBeginMinute.SelectedIndex = 0;
            cboEndMinute.SelectedIndex = 0;
        }

        public void SetTerm(string strValue)
        {
            PropertiesLevel propertiesLevel = FormMain.Instance.GetPageLevel().GetPropertiesLevel();
            int nType = propertiesLevel.PeriodType;
            CheckOption(nType);
            SelectDateTime(strValue, nType);
            SelectWeekDay(propertiesLevel.WeekDayOPtion);
        }

        private void rdoUse_CheckedChanged(object sender, EventArgs e)
        {
            EnabledOption();
        }

        private void EnabledOption()
        {
            groupYear.Enabled = groupDate.Enabled = groupTime.Enabled = groupWeek.Enabled = rdoUse.Checked;
            checkYear.Enabled = checkDate.Enabled = checkTime.Enabled = rdoUse.Checked;

            if (rdoUse.Checked)
            {
                groupYear.Enabled = checkYear.Checked;
                groupDate.Enabled = checkDate.Checked;
                groupTime.Enabled = checkTime.Checked;
                groupWeek.Enabled = rdoUse.Checked;
            }
        }

        private void checkOption_CheckedChanged(object sender, EventArgs e)
        {
            EnabledOption();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(rdoUse.Checked)
            {
                if (checkDate.Checked == false && checkTime.Checked == false)
                {
                    MessageBox.Show("날짜 또는 시간 옵션이 체크되어 있지 않습니다.");
                    return;
                }
            }

            PropertiesLevel propertiesLevel = FormMain.Instance.GetPageLevel().GetPropertiesLevel();

            int nType = GetPeriodType();
            string[] strOption = GetCheckOption();
            DateTime dtBeginTime = Convert.ToDateTime(strOption[0]);
            DateTime dtEndTime = Convert.ToDateTime(strOption[1]);

            propertiesLevel.PeriodType = nType;
            propertiesLevel.BeginTime = dtBeginTime;
            propertiesLevel.EndTime = dtEndTime;
            propertiesLevel.WeekDayOPtion = GetWeekOption();
            propertiesLevel.Term = GetTerm(dtBeginTime, dtEndTime, nType);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private string GetTerm(DateTime dtBegin, DateTime dtEnd, int nType)
        {
            string strValue = "";
            string strBegin = "";
            string strEnd = "";

            if (!rdoUse.Checked)
                strValue = rdoUnUse.Text;
            else
            {
                switch (nType)
                {
                    case 1:
                        strBegin = dtBegin.Month.ToString() + "/" + dtBegin.Day.ToString();
                        strEnd = dtEnd.Month.ToString() + "/" + dtEnd.Day.ToString();
                        break;
                    case 2:
                        strBegin = dtBegin.Hour.ToString() + ":" + dtBegin.Minute.ToString();
                        strEnd = dtEnd.Hour.ToString() + ":" + dtEnd.Minute.ToString();
                        break;
                    case 3:
                        strBegin = dtBegin.Month.ToString() + "/" + dtBegin.Day.ToString() + " " + dtBegin.Hour.ToString() + ":" + dtBegin.Minute.ToString();
                        strEnd = dtEnd.Month.ToString() + "/" + dtEnd.Day.ToString() + " " + dtEnd.Hour.ToString() + ":" + dtEnd.Minute.ToString();
                        break;
                    case 11:
                        strBegin = dtBegin.Year.ToString() + "/" + dtBegin.Month.ToString() + "/" + dtBegin.Day.ToString();
                        strEnd = dtEnd.Year.ToString() + "/" + dtEnd.Month.ToString() + "/" + dtEnd.Day.ToString();
                        break;
                    case 12:
                        strBegin = dtBegin.Year.ToString() + " " + dtBegin.Hour.ToString() + ":" + dtBegin.Minute.ToString();
                        strEnd = dtEnd.Year.ToString() + " " + dtEnd.Hour.ToString() + ":" + dtEnd.Minute.ToString();
                        break;
                    case 13:
                        strBegin = dtBegin.Year.ToString() + "/" + dtBegin.Month.ToString() + "/" + dtBegin.Day.ToString() + " " + dtBegin.Hour.ToString() + ":" + dtBegin.Minute.ToString();
                        strEnd = dtEnd.Year.ToString() + "/" + dtEnd.Month.ToString() + "/" + dtEnd.Day.ToString() + " " + dtEnd.Hour.ToString() + ":" + dtEnd.Minute.ToString();
                        break;
                }
                strValue = strBegin + " ~ " + strEnd;
            }

            return strValue;
        }

        private int GetPeriodType()
        {
            int nPeriodType = 0;
            if (rdoUse.Checked)
            {
                if (checkDate.Checked)
                    nPeriodType = 1;

                if (checkTime.Checked)
                    nPeriodType = 2;

                if (checkDate.Checked && checkTime.Checked)
                    nPeriodType = 3;

                if (checkYear.Checked)
                    nPeriodType += 10;
            }

            return nPeriodType;
        }

        private string[] GetCheckOption()
        {
            int[] nBeginDate = { 0, 0 };
            int[] nEndDate = { 0, 0 };
            int[] nBeginTime = { 0, 0 };
            int[] nEndTime = { 0, 0 };

            string stBeginYear = "";
            string strEndYear = "";

            stBeginYear = GetBeginYear();
            strEndYear = GetEndYear();
            
            nBeginDate = GetBeginDate();
            nEndDate = GetEndDate();
            
            nBeginTime = GetBeginTime();
            nEndTime = GetEndTime();           

            string[] strOption = {"", ""};
            strOption[0] = stBeginYear + "/" + nBeginDate[0].ToString() + "/" + nBeginDate[1].ToString() + " " + nBeginTime[0].ToString() + ":" + nBeginTime[1].ToString() + ":00";
            strOption[1] = strEndYear + "/" + nEndDate[0].ToString() + "/" + nEndDate[1].ToString() + " " + nEndTime[0].ToString() + ":" + nEndTime[1].ToString() + ":00";

            return strOption;
        }

        private string GetBeginYear()
        {
            return cboBeginYear.Text;
        }

        private string GetEndYear()
        {
            return cboEndYear.Text;
        }

        private int[] GetBeginDate()
        {
            int[] nBeginDate = { 0, 0 };

            nBeginDate[0] = cboBeginMonth.SelectedIndex + 1;
            nBeginDate[1] = cboBeginDay.SelectedIndex + 1;

            return nBeginDate;
        }

        private int[] GetEndDate()
        {
            int[] nEndDate = { 0, 0 };

            nEndDate[0] = cboEndMonth.SelectedIndex + 1;
            nEndDate[1] = cboEndDay.SelectedIndex + 1;

            return nEndDate;
        }

        private int[] GetBeginTime()
        {
            int[] nBeginTime = { 0, 0 };

            nBeginTime[0] = cboBeginHour.SelectedIndex;
            nBeginTime[1] = cboBeginMinute.SelectedIndex;

            return nBeginTime;
        }

        private int[] GetEndTime()
        {
            int[] nEndTime = { 0, 0 };

            nEndTime[0] = cboEndHour.SelectedIndex;
            nEndTime[1] = cboEndMinute.SelectedIndex;

            return nEndTime;
        }

        private int GetWeekOption()
        {
            int nWeek = 0;

            if (checkSun.Checked)
            {
                nWeek |= 1;
            }
            if(checkMon.Checked)
            {
                nWeek |= 2;
            }
            if (checkTue.Checked)
            {
                nWeek |= 4;
            }
            if (checkWed.Checked)
            {
                nWeek |= 8;
            }
            if (checkThu.Checked)
            {
                nWeek |= 16;
            }
            if (checkFri.Checked)
            {
                nWeek |= 32;
            }
            if (checkSat.Checked)
            {
                nWeek |= 64;
            }

            return nWeek;
        }

        private void CheckOption(int nType)
        {
            if (nType == 0)
            {
                rdoUse.Checked = false;
                EnabledOption();
            }
            else
            {
                rdoUse.Checked = true;
            }

            if (nType == 1)
            {
                checkDate.Checked = true;
            }
            else if (nType == 2)
            {
                checkTime.Checked = true;
            }
            else if (nType == 3)
            {
                checkDate.Checked = true;
                checkTime.Checked = true;
            }
            else if (nType == 11)
            {
                checkYear.Checked = true;
                checkDate.Checked = true;
            }
            else if (nType == 12)
            {
                checkYear.Checked = true;
                checkTime.Checked = true;
            }
            else if (nType == 13)
            {
                checkYear.Checked = true;
                checkDate.Checked = true;
                checkTime.Checked = true;
            }
        }

        // nType 1: 1/1 ~ 1/1 nType 2: 0:0 ~ 0:0 nType 3: 1/1 0:0 ~ 1/1 0:0
        // nTYpe 11: 2012/1/1 ~ 2012/1/1 nType 12: 2012 0:0 ~ 2012 0:0 nType 13: 2012/1/1 0:0 ~ 2012/1/1 0:0
        private void SelectDateTime(string strDateTime, int nType)
        {
            if (strDateTime == null) return;
            if (strDateTime == "사용안함") nType = 0;

            string[] str = strDateTime.Split(new char[] { ' ' });

            switch(nType)
            {
                case 1:
                    OnDate(str);
                    break;
                case 2:
                    OnTime(str);
                    break;
                case 3:
                    OnDateTime(str);
                    break;
                case 11:
                     OnDate2(str);
                    break;
                case 12:
                    OnTime2(str);
                    break;
                case 13:
                    OnDateTime2(str);
                    break;
            }
        }

        private void OnDate(string[] str)
        {
            string[] strBeginDate = { "", "" };
            string[] strEndDate = { "", "" };

            strBeginDate = str[0].Split(new char[] { '/' });
            strEndDate = str[2].Split(new char[] { '/' });

            cboBeginMonth.SelectedIndex = int.Parse(strBeginDate[0]) - 1;
            cboBeginDay.SelectedIndex = int.Parse(strBeginDate[1]) - 1;

            cboEndMonth.SelectedIndex = int.Parse(strEndDate[0]) - 1;
            cboEndDay.SelectedIndex = int.Parse(strEndDate[1]) - 1;
        }

        private void OnTime(string[] str)
        {
            string[] strBeginTime = { "", "" };
            string[] strEndTime = { "", "" };

            strBeginTime = str[0].Split(new char[] { ':' });
            strEndTime = str[2].Split(new char[] { ':' });

            cboBeginHour.SelectedIndex = int.Parse(strBeginTime[0]);
            cboBeginMinute.SelectedIndex = int.Parse(strBeginTime[1]);

            cboEndHour.SelectedIndex = int.Parse(strEndTime[0]);
            cboEndMinute.SelectedIndex = int.Parse(strEndTime[1]);
        }

        private void OnDateTime(string[] str)
        {
            string[] strBeginDate = { "", "" };
            string[] strEndDate = { "", "" };

            string[] strBeginTime = { "", "" };
            string[] strEndTime = { "", "" };

            strBeginDate = str[0].Split(new char[] { '/' });
            strEndDate = str[3].Split(new char[] { '/' });

            cboBeginMonth.SelectedIndex = int.Parse(strBeginDate[0]) - 1;
            cboBeginDay.SelectedIndex = int.Parse(strBeginDate[1]) - 1;

            cboEndMonth.SelectedIndex = int.Parse(strEndDate[0]) - 1;
            cboEndDay.SelectedIndex = int.Parse(strEndDate[1]) - 1;

            strBeginTime = str[1].Split(new char[] { ':' });
            strEndTime = str[4].Split(new char[] { ':' });

            cboBeginHour.SelectedIndex = int.Parse(strBeginTime[0]);
            cboBeginMinute.SelectedIndex = int.Parse(strBeginTime[1]);

            cboEndHour.SelectedIndex = int.Parse(strEndTime[0]);
            cboEndMinute.SelectedIndex = int.Parse(strEndTime[1]);
        }

        private void OnDate2(string[] str)
        {
            int nBeginYear = 0;
            int nEndYear = 0;

            string[] strBeginDate = { "", "" };
            string[] strEndDate = { "", "" };

            strBeginDate = str[0].Split(new char[] { '/' });
            strEndDate = str[2].Split(new char[] { '/' });

            nBeginYear = int.Parse(strBeginDate[0]) - 2000;
            cboBeginYear.SelectedIndex = nBeginYear;
            cboBeginMonth.SelectedIndex = int.Parse(strBeginDate[1]) - 1;
            cboBeginDay.SelectedIndex = int.Parse(strBeginDate[2]) - 1;

            nEndYear = int.Parse(strEndDate[0]) - 2000;
            cboEndYear.SelectedIndex = nEndYear;
            cboEndMonth.SelectedIndex = int.Parse(strEndDate[1]) - 1;
            cboEndDay.SelectedIndex = int.Parse(strEndDate[2]) - 1;
        }

        private void OnTime2(string[] str)
        {
            int nBeginYear = 0;
            int nEndYear = 0;

            string[] strBeginTime = { "", "" };
            string[] strEndTime = { "", "" };

            strBeginTime = str[1].Split(new char[] { ':' });
            strEndTime = str[4].Split(new char[] { ':' });

            nBeginYear = int.Parse(str[0]) - 2000;
            cboBeginYear.SelectedIndex = nBeginYear;
            cboBeginHour.SelectedIndex = int.Parse(strBeginTime[0]);
            cboBeginMinute.SelectedIndex = int.Parse(strBeginTime[1]);

            nEndYear = int.Parse(str[3]) - 2000;
            cboEndYear.SelectedIndex = nEndYear;
            cboEndHour.SelectedIndex = int.Parse(strEndTime[0]);
            cboEndMinute.SelectedIndex = int.Parse(strEndTime[1]);
        }

        private void OnDateTime2(string[] str)
        {
            int nBeginYear = 0;
            int nEndYear = 0;

            string[] strBeginDate = { "", "" };
            string[] strEndDate = { "", "" };
            string[] strBeginTime = { "", "" };
            string[] strEndTime = { "", "" };

            strBeginDate = str[0].Split(new char[] { '/' });
            strBeginTime = str[1].Split(new char[] { ':' });
            strEndDate = str[3].Split(new char[] { '/' });
            strEndTime = str[4].Split(new char[] { ':' });

            nBeginYear = int.Parse(strBeginDate[0]) - 2000;
            cboBeginYear.SelectedIndex = nBeginYear;
            cboBeginMonth.SelectedIndex = int.Parse(strBeginDate[1]) - 1;
            cboBeginDay.SelectedIndex = int.Parse(strBeginDate[2]) - 1;
            cboBeginHour.SelectedIndex = int.Parse(strBeginTime[0]);
            cboBeginMinute.SelectedIndex = int.Parse(strBeginTime[1]);

            nEndYear = int.Parse(strEndDate[0]) - 2000;
            cboEndYear.SelectedIndex = nEndYear;
            cboEndMonth.SelectedIndex = int.Parse(strEndDate[1]) - 1;
            cboEndDay.SelectedIndex = int.Parse(strEndDate[2]) - 1;
            cboEndHour.SelectedIndex = int.Parse(strEndTime[0]);
            cboEndMinute.SelectedIndex = int.Parse(strEndTime[1]);
        }

        private void SelectWeekDay(int nWeekDayOPtion)
        {
            checkSun.Checked = (nWeekDayOPtion & 1) == 1;
            checkMon.Checked = (nWeekDayOPtion & 2) == 2;
            checkTue.Checked = (nWeekDayOPtion & 4) == 4;
            checkWed.Checked = (nWeekDayOPtion & 8) == 8;
            checkThu.Checked = (nWeekDayOPtion & 16) == 16;
            checkFri.Checked = (nWeekDayOPtion & 32) == 32;
            checkSat.Checked = (nWeekDayOPtion & 64) == 64;   
        }



        private void SetRadioImage()
        {
            if (rdoUse.Checked == true)
            {
                rdPictureBox1.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_black;
            }
            else
            {
                rdPictureBox1.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_gray;
            }

            if (rdoUnUse.Checked == true)
            {
                rdPictureBox2.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_black;
            }
            else
            {
                rdPictureBox2.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_gray;
            }

        }

        private void rdPictureBox1_Click(object sender, EventArgs e)
        {
            rdLabel1_Click(sender, e);
        }

        private void rdPictureBox2_Click(object sender, EventArgs e)
        {
            rdLabel2_Click(sender, e);
        }

        private void rdLabel1_Click(object sender, EventArgs e)
        {
            if (rdoUse.Checked == false)
            {
                rdoUse.Checked = !rdoUse.Checked;
                SetRadioImage();
            }
        }

        private void rdLabel2_Click(object sender, EventArgs e)
        {
            if (rdoUnUse.Checked == false)
            {
                rdoUnUse.Checked = !rdoUnUse.Checked;
                SetRadioImage();
            }
        }

        private void PopupProcessTerm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void PopupProcessTerm_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {
                        Point ptCur = this.Location;
                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void PopupProcessTerm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        }
    }
}
