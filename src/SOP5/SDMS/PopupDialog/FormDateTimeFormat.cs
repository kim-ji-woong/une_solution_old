using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SDMS.PopupDialog
{
    public partial class FormDateTimeFormat : Form
    {
        public static class ReportDateTimeFormat
        {
            private static string m_strSectionName = "Report DateTime Format";

            private static string m_strFormatUserDefineMin = string.Empty;
            private static string m_strFormatUserDefineHour = string.Empty;
            private static string m_strFormatUserDefineDay = string.Empty;
            private static string m_strFormatUserDefineWeek = string.Empty;
            private static string m_strFormatUserDefineMonth = string.Empty;
            private static string m_strFormatUserDefineYear = string.Empty;

            public static string FormatUserDefineMin { get { return m_strFormatUserDefineMin; } set { m_strFormatUserDefineMin = value; } }
            public static string FormatUserDefineHour { get { return m_strFormatUserDefineHour; } set { m_strFormatUserDefineHour = value; } }
            public static string FormatUserDefineDay { get { return m_strFormatUserDefineDay; } set { m_strFormatUserDefineDay = value; } }
            public static string FormatUserDefineWeek { get { return m_strFormatUserDefineWeek; } set { m_strFormatUserDefineWeek = value; } }
            public static string FormatUserDefineMonth { get { return m_strFormatUserDefineMonth; } set { m_strFormatUserDefineMonth = value; } }
            public static string FormatUserDefineYear { get { return m_strFormatUserDefineYear; } set { m_strFormatUserDefineYear = value; } }


            private static string m_strFormatMin = null;
            public static string FormatMin
            {
                get
                {
                    if (m_strFormatMin == null)
                    {
                        if (String.IsNullOrWhiteSpace(FormMain.Instance.DBManager.LoadIni("Min_Format", m_strSectionName)))
                            FormMain.Instance.DBManager.SaveIni("Min_Format", "{Y}-{M}-{D} {H}시 {MIN}분", m_strSectionName);
                            
                        m_strFormatMin = FormMain.Instance.DBManager.LoadIni("Min_Format", m_strSectionName);
                    }

                    return m_strFormatMin;
                }
                set
                {
                    m_strFormatMin = value;
                    FormMain.Instance.DBManager.SaveIni("Min_Format", m_strFormatMin, m_strSectionName);
                }
            }

            private static string m_strFormatHour = null;
            public static string FormatHour
            {
                get
                {
                    if (m_strFormatHour == null)
                    {
                        if (String.IsNullOrWhiteSpace(FormMain.Instance.DBManager.LoadIni("Hour_Format", m_strSectionName)))
                            FormMain.Instance.DBManager.SaveIni("Hour_Format", "{Y}-{M}-{D} {H}시", m_strSectionName);
                            
                        m_strFormatHour = FormMain.Instance.DBManager.LoadIni("Hour_Format", m_strSectionName);
                    }

                    return m_strFormatHour;
                }
                set
                {
                    m_strFormatHour = value;
                    FormMain.Instance.DBManager.SaveIni("Hour_Format", m_strFormatHour, m_strSectionName);
                }
            }

            private static string m_strFormatDay = null;
            public static string FormatDay
            {
                get
                {
                    if (m_strFormatDay == null)
                    {
                        if (String.IsNullOrWhiteSpace(FormMain.Instance.DBManager.LoadIni("Day_Format", m_strSectionName)))
                            FormMain.Instance.DBManager.SaveIni("Day_Format", "{Y}-{M}-{D}", m_strSectionName);

                        m_strFormatDay = FormMain.Instance.DBManager.LoadIni("Day_Format", m_strSectionName);
                    }

                    return m_strFormatDay;
                }
                set
                {
                    m_strFormatDay = value;
                    FormMain.Instance.DBManager.SaveIni("Day_Format", m_strFormatDay, m_strSectionName);
                }
            }

            private static string m_strFormatWeek = null;
            public static string FormatWeek
            {
                get
                {
                    if (m_strFormatWeek == null)
                    {
                        if (String.IsNullOrWhiteSpace(FormMain.Instance.DBManager.LoadIni("Week_Format", m_strSectionName)))
                            FormMain.Instance.DBManager.SaveIni("Week_Format", "{Y}년도 {W}주차", m_strSectionName);
                            
                        m_strFormatWeek = FormMain.Instance.DBManager.LoadIni("Week_Format", m_strSectionName);
                    }

                    return m_strFormatWeek;
                }
                set
                {
                    m_strFormatWeek = value;
                    FormMain.Instance.DBManager.SaveIni("Week_Format", m_strFormatWeek, m_strSectionName);
                }
            }

            private static string m_strFormatMonth = null;
            public static string FormatMonth
            {
                get
                {
                    if (m_strFormatMonth == null)
                    {
                        if (String.IsNullOrWhiteSpace(FormMain.Instance.DBManager.LoadIni("Month_Format", m_strSectionName)))
                            FormMain.Instance.DBManager.SaveIni("Month_Format", "{Y}년 {M}월", m_strSectionName);
                            
                        m_strFormatMonth = FormMain.Instance.DBManager.LoadIni("Month_Format", m_strSectionName);
                    }

                    return m_strFormatMonth;
                }
                set
                {
                    m_strFormatMonth = value;
                    FormMain.Instance.DBManager.SaveIni("Month_Format", m_strFormatMonth, m_strSectionName);
                }
            }

            private static string m_strFormatYear = null;
            public static string FormatYear
            {
                get
                {
                    if (m_strFormatYear == null)
                    {
                        if (String.IsNullOrWhiteSpace(FormMain.Instance.DBManager.LoadIni("Year_Format", m_strSectionName)))
                            FormMain.Instance.DBManager.SaveIni("Year_Format", "{Y}년도", m_strSectionName);
                            
                        m_strFormatYear = FormMain.Instance.DBManager.LoadIni("Year_Format", m_strSectionName);
                    }

                    return m_strFormatYear;
                }
                set
                {
                    m_strFormatYear = value;
                    FormMain.Instance.DBManager.SaveIni("Year_Format", m_strFormatYear, m_strSectionName);
                }
            }


            public static string GetDateTimeParsing(DateTime dt, int nUnit, string strFormat = null)
            {
                string strParse = string.Empty;

                if (String.IsNullOrWhiteSpace(strFormat) == false)
                {
                    strParse = strFormat.Trim();
                }
                else
                {
                    switch (nUnit)
                    {
                        case 0:
                            strParse = FormatMin.Trim();
                            break;
                        case 1:
                            strParse = FormatHour.Trim();
                            break;
                        case 2:
                            strParse = FormatDay.Trim();
                            break;
                        case 3:
                            strParse = FormatWeek.Trim();
                            break;
                        case 4:
                            strParse = FormatMonth.Trim();
                            break;
                        case 5:
                            strParse = FormatYear.Trim();
                            break;
                    }
                }

                strParse =
                strParse
                .Replace("{Y}", String.Format("{0:yyyy}", dt))
                .Replace("{M}", nUnit > 4 ? "" : String.Format("{0:MM}", dt))
                .Replace("{W}", nUnit > 3 ? "" : System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dt, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Sunday).ToString())
                .Replace("{D}", nUnit > 2 ? "" : String.Format("{0:dd}", dt))
                .Replace("{H}", nUnit > 1 ? "" : String.Format("{0:HH}", dt))
                .Replace("{MIN}", nUnit > 0 ? "" : String.Format("{0:mm}", dt));

                return strParse;
            }

        }

        private DateTime m_dtNow = DateTime.Now;
        private string m_strUserDefine = "사용자정의";

        private Dictionary<ComboBox, TextBox> m_dicDateFormatControl = new Dictionary<ComboBox, TextBox>();

        public FormDateTimeFormat()
        {
            InitializeComponent();

            this.Load += FormDateTimeFormat_Load;

            cboMin.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            cboHour.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            cboDay.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            cboWeek.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            cboMonth.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            cboYear.SelectedIndexChanged += ComboBox_SelectedIndexChanged;

            txtMin.TextChanged += (s, e) => { PrintFormat(cboMin, txtMin, lblMin, 0); };
            txtHour.TextChanged += (s, e) => { PrintFormat(cboHour, txtHour, lblHour, 1); };
            txtDay.TextChanged += (s, e) => { PrintFormat(cboDay, txtDay, lblDay, 2); };
            txtWeek.TextChanged += (s, e) => { PrintFormat(cboWeek, txtWeek, lblWeek, 3); };
            txtMonth.TextChanged += (s, e) => { PrintFormat(cboMonth, txtMonth, lblMonth, 4); };
            txtYear.TextChanged += (s, e) => { PrintFormat(cboYear, txtYear, lblYear, 5); };

            btnSave.Click += btnSave_Click;

            m_dicDateFormatControl.Add(cboMin, txtMin);
            m_dicDateFormatControl.Add(cboHour, txtHour);
            m_dicDateFormatControl.Add(cboDay, txtDay);
            m_dicDateFormatControl.Add(cboWeek, txtWeek);
            m_dicDateFormatControl.Add(cboMonth, txtMonth);
            m_dicDateFormatControl.Add(cboYear, txtYear);
        }

        private void FormDateTimeFormat_Load(object sender, EventArgs e)
        {
            SetComboBoxItem();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ReportDateTimeFormat.FormatMin = txtMin.Text;
            ReportDateTimeFormat.FormatHour = txtHour.Text;
            ReportDateTimeFormat.FormatDay = txtDay.Text;
            ReportDateTimeFormat.FormatWeek = txtWeek.Text;
            ReportDateTimeFormat.FormatMonth = txtMonth.Text;
            ReportDateTimeFormat.FormatYear = txtYear.Text;

            this.Owner.Visible = false;
            FormMain.Instance.PageHome.OnTranslucentFormClosing();
            FormMain.Instance.Activate();
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbo = (sender as ComboBox);

            if (cbo.Text == m_strUserDefine)
            {
                m_dicDateFormatControl[cbo].Text = GetFormatUserDefine(cbo);
                m_dicDateFormatControl[cbo].ReadOnly = false;
                m_dicDateFormatControl[cbo].TabStop = true;
            }
            else
            {
                m_dicDateFormatControl[cbo].Text = cbo.Text;
                m_dicDateFormatControl[cbo].ReadOnly = true;
                m_dicDateFormatControl[cbo].TabStop = false;
            }
        }


        private void SetComboBoxItem()
        {
            cboMin.Items.Add(m_strUserDefine);
            cboMin.Items.Add("{Y}-{M}-{D} {H}시 {MIN}분");
            cboMin.Items.Add("{M}-{D} {H}시 {MIN}분");
            cboMin.Items.Add("{M}-{D} {H}:{MIN}");
            cboMin.Items.Add("{M}월 {D}일 {H}시 {MIN}분");
            cboMin.Items.Add("{M}월 {D}일 {H}:{MIN}");
            cboMin.Items.Add("{D}일 {H}시 {MIN}분");
            cboMin.Items.Add("{D}일 {H}:{MIN}");
            cboMin.Items.Add("{H}:{MIN}");
            cboMin.Items.Add("{H}시 {MIN}분");

            cboHour.Items.Add(m_strUserDefine);
            cboHour.Items.Add("{Y}-{M}-{D} {H}시");
            cboHour.Items.Add("{M}-{D} {H}시");
            cboHour.Items.Add("{M}월 {D}일 {H}시");
            cboHour.Items.Add("{D}일 {H}시");
            cboHour.Items.Add("{H}시");

            cboDay.Items.Add(m_strUserDefine);
            cboDay.Items.Add("{Y}-{M}-{D}");
            cboDay.Items.Add("{M}-{D}");
            cboDay.Items.Add("{Y}년 {M}월 {D}일");
            cboDay.Items.Add("{M}월 {D}일");
            cboDay.Items.Add("{D}일");

            cboWeek.Items.Add(m_strUserDefine);
            cboWeek.Items.Add("{Y}년도 {W}주차");
            cboWeek.Items.Add("{Y}년 {W}주차");
            cboWeek.Items.Add("{W}주차");

            cboMonth.Items.Add(m_strUserDefine);
            cboMonth.Items.Add("{Y}년 {M}월");
            cboMonth.Items.Add("{Y}-{M}");
            cboMonth.Items.Add("{M}월");

            cboYear.Items.Add(m_strUserDefine);
            cboYear.Items.Add("{Y}년도");
            cboYear.Items.Add("{Y}년");
            cboYear.Items.Add("{Y}");


            foreach (object item in cboMin.Items)
            {
                if (item.ToString() == ReportDateTimeFormat.FormatMin)
                {
                    cboMin.SelectedItem = item;
                    break;
                }
            }

            foreach (object item in cboHour.Items)
            {
                if (item.ToString() == ReportDateTimeFormat.FormatHour)
                {
                    cboHour.SelectedItem = item;
                    break;
                }
            }

            foreach (object item in cboDay.Items)
            {
                if (item.ToString() == ReportDateTimeFormat.FormatDay)
                {
                    cboDay.SelectedItem = item;
                    break;
                }
            }

            foreach (object item in cboWeek.Items)
            {
                if (item.ToString() == ReportDateTimeFormat.FormatWeek)
                {
                    cboWeek.SelectedItem = item;
                    break;
                }
            }

            foreach (object item in cboMonth.Items)
            {
                if (item.ToString() == ReportDateTimeFormat.FormatMonth)
                {
                    cboMonth.SelectedItem = item;
                    break;
                }
            }
            
            foreach (object item in cboYear.Items)
            {
                if (item.ToString() == ReportDateTimeFormat.FormatYear)
                {
                    cboYear.SelectedItem = item;
                    break;
                }
            }


            if (cboMin.SelectedItem == null)
            {
                cboMin.SelectedIndex = 0;
                txtMin.Text = ReportDateTimeFormat.FormatMin;
            }

            if (cboHour.SelectedItem == null)
            {
                cboHour.SelectedIndex = 0;
                txtHour.Text = ReportDateTimeFormat.FormatHour;
            }

            if (cboDay.SelectedItem == null)
            {
                cboDay.SelectedIndex = 0;
                txtDay.Text = ReportDateTimeFormat.FormatDay;
            }

            if (cboWeek.SelectedItem == null)
            {
                cboWeek.SelectedIndex = 0;
                txtWeek.Text = ReportDateTimeFormat.FormatWeek;
            }

            if (cboMonth.SelectedItem == null)
            {
                cboMonth.SelectedIndex = 0;
                txtMonth.Text = ReportDateTimeFormat.FormatMonth;
            }

            if (cboYear.SelectedItem == null)
            {
                cboYear.SelectedIndex = 0;
                txtYear.Text = ReportDateTimeFormat.FormatYear;
            }


        }

        private string GetFormatUserDefine(ComboBox cbo)
        {
            string strRtn = string.Empty;

            if (cbo == cboMin)
                strRtn = ReportDateTimeFormat.FormatUserDefineMin;
            else if (cbo == cboHour)
                strRtn = ReportDateTimeFormat.FormatUserDefineHour;
            else if (cbo == cboDay)
                strRtn = ReportDateTimeFormat.FormatUserDefineDay;
            else if (cbo == cboWeek)
                strRtn = ReportDateTimeFormat.FormatUserDefineWeek;
            else if (cbo == cboMonth)
                strRtn = ReportDateTimeFormat.FormatUserDefineMonth;
            else if (cbo == cboYear)
                strRtn = ReportDateTimeFormat.FormatUserDefineYear;

            return strRtn;
        }


        private void PrintFormat(ComboBox cbo, TextBox txt, Label lbl, int nUnit)
        {
            if (cbo.Text == m_strUserDefine)
            {
                switch (nUnit)
                {
                    case 0:
                        ReportDateTimeFormat.FormatUserDefineMin = txt.Text;
                        break;
                    case 1:
                        ReportDateTimeFormat.FormatUserDefineHour = txt.Text;
                        break;
                    case 2:
                        ReportDateTimeFormat.FormatUserDefineDay = txt.Text;
                        break;
                    case 3:
                        ReportDateTimeFormat.FormatUserDefineWeek = txt.Text;
                        break;
                    case 4:
                        ReportDateTimeFormat.FormatUserDefineMonth = txt.Text;
                        break;
                    case 5:
                        ReportDateTimeFormat.FormatUserDefineYear = txt.Text;
                        break;

                }
                
            }

            lbl.Text = ReportDateTimeFormat.GetDateTimeParsing(m_dtNow, nUnit, txt.Text.Trim());
        }

    }
}
