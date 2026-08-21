using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using UnE.Sensor;
using System.Diagnostics;
using System.IO;

namespace SDMS_Building.Report
{
    public partial class uFormReport : UserControl
    {
        private static uFormReport m_instance = null;
        public static uFormReport Instance
        {
            get
            {
                return m_instance;
            }
        }

        private ReactionManager m_DetectMgr = new ReactionManager();

        private uFormReport_Pareto m_uFrmPareto = null;
        private uFormReport_Detect m_uFrmDetect = null;
        private uFormReport_NotOperation m_uFrmNotOperation = null;

        private HwpCtrlData m_hwpCtrl = null;
        internal HwpCtrlData HwpCtrl
        {
            get { return m_hwpCtrl; }
            set { m_hwpCtrl = value; }
        }

        private string m_strLogoFileName = "";
        public string StrLogoFileName
        {
            get { return m_strLogoFileName; }
        }

        public uFormReport()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            m_instance = this;

            

            m_hwpCtrl = new HwpCtrlData();
            m_hwpCtrl.SetRegistry();

            m_strLogoFileName = GetReportLogoFileName();

            rbtnPareto.IsChecked = true;

            rbtnPareto.Font = m_fontBold;
            rbtnDetect.Font = m_fontBold;
            rbtnNotOperation.Font = m_fontBold;

            rbtnPareto.SetTextLocation(0, 13);
            rbtnDetect.SetTextLocation(0, 13);
            rbtnNotOperation.SetTextLocation(0, 13);

            LoadDefineColumns();
        }

        private void uFormReport_Load(object sender, EventArgs e)
        {
            m_uFrmPareto = new uFormReport_Pareto(m_DetectMgr);
            m_uFrmPareto.Parent = pnMain;
            m_uFrmPareto.Dock = DockStyle.Fill;

            m_uFrmDetect = new uFormReport_Detect(m_DetectMgr);
            m_uFrmDetect.Parent = pnMain;
            m_uFrmDetect.Dock = DockStyle.Fill;
            m_uFrmDetect.Visible = false;

            m_uFrmNotOperation = new uFormReport_NotOperation(m_DetectMgr);
            m_uFrmNotOperation.Parent = pnMain;
            m_uFrmNotOperation.Dock = DockStyle.Fill;
            m_uFrmNotOperation.Visible = false;
        }

        private string GetReportLogoFileName()
        {
            string strSQL = "Select PropertyValue from OptionSdms where PropertyName='LogoFileName' and SiteID=" + UnE.SOP.ProxySOP.Instance.SiteID;
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0) return string.Empty;

            string logoName = DBUtility2.WebDBManager.GetStringField(arrResult[0].ToString(), string.Empty);

            return logoName;
        }


        private Font m_fontBold = new System.Drawing.Font("나눔바른고딕", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));

        private void rbtnPareto_Click(object sender, EventArgs e)
        {
            if (rbtnPareto.IsChecked)
                return;

            rbtnPareto.IsChecked = true;
            rbtnDetect.IsChecked = false;
            rbtnNotOperation.IsChecked = false;

            rbtnPareto.Refresh();
            rbtnDetect.Refresh();
            rbtnNotOperation.Refresh();

            m_uFrmPareto.Visible = true;
            m_uFrmDetect.Visible = false;
            m_uFrmNotOperation.Visible = false;
        }

        private void rbtnDetect_Click(object sender, EventArgs e)
        {
            if (rbtnDetect.IsChecked)
                return;

            rbtnPareto.IsChecked = false;
            rbtnDetect.IsChecked = true;
            rbtnNotOperation.IsChecked = false;

            rbtnPareto.Refresh();
            rbtnDetect.Refresh();
            rbtnNotOperation.Refresh();

            m_uFrmPareto.Visible = false;
            m_uFrmDetect.Visible = true;
            m_uFrmNotOperation.Visible = false;
        }

        private void rbtnNotOperation_Click(object sender, EventArgs e)
        {
            if (rbtnNotOperation.IsChecked)
                return;

            rbtnPareto.IsChecked = false;
            rbtnDetect.IsChecked = false;
            rbtnNotOperation.IsChecked = true;

            rbtnPareto.Refresh();
            rbtnDetect.Refresh();
            rbtnNotOperation.Refresh();

            m_uFrmPareto.Visible = false;
            m_uFrmDetect.Visible = false;
            m_uFrmNotOperation.Visible = true;
        }

        private void LoadDefineColumns()
        {
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData("Select ID, ColumnName, Description From DefineReportColumn");
            if (arrResult == null || arrResult.Count == 0)
                return;

            m_defineColumns = new List<DefineColumns>();

            for (int i = 0; i < arrResult.Count; i+=3)
            {
                int nID = DBUtility2.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strColumnName = DBUtility2.WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
                string strDesc = DBUtility2.WebDBManager.GetStringField(arrResult[i + 2].ToString(), "");

                DefineColumns column = new DefineColumns();
                column.ColumnID = nID;
                column.ColumnName = strColumnName;
                column.Description = strDesc;
                m_defineColumns.Add(column);
            }

            List<int> useLists = new List<int>();
            useLists.Add((int)IFacility.FacilityType.FIRE_SENSOR);
            if (UnE.SOP.ProxySOP.Instance.UsePSM)
                useLists.Add((int)IFacility.FacilityType.PSM_SENSOR);
            if (UnE.SOP.ProxySOP.Instance.UseDoor)
                useLists.Add((int)IFacility.FacilityType.DOOR);
            if (UnE.SOP.ProxySOP.Instance.UseEarthquake)
                useLists.Add((int)IFacility.FacilityType.Earthquake);
            if (UnE.SOP.ProxySOP.Instance.UseFirewall)
                useLists.Add((int)IFacility.FacilityType.FIREWALL);
            if (UnE.SOP.ProxySOP.Instance.UseBlackout)
                useLists.Add((int)IFacility.FacilityType.BLACKOUT);
            if (UnE.SOP.ProxySOP.Instance.UseStrongWind)
                useLists.Add((int)IFacility.FacilityType.STRONG_WIND);
            if (UnE.SOP.ProxySOP.Instance.UseTerror)
                useLists.Add((int)IFacility.FacilityType.TERROR);
            if (UnE.SOP.ProxySOP.Instance.UseSubmergency)
                useLists.Add((int)IFacility.FacilityType.SUBMERGENCY);

            string strUseList = string.Format("{0}", string.Join(", ", useLists));
            
            StringBuilder sb = new StringBuilder();
            sb.Append("Select ID, FacilityTypeID, ReportType, DefineReportColumnID, HeaderText, ColumnWidthRatio ");
            sb.Append("  From DefineReportColumnsByFacilityType ");
            sb.AppendFormat(" Where FacilityTypeID IN ({0}) ", strUseList);

            arrResult = FormMain.Instance.DBManager.GetResultData(sb.ToString());
            if (arrResult == null || arrResult.Count == 0)
                return;
            
            for (int i = 0; i < arrResult.Count; i += 6)
            {
                int nID = DBUtility2.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nFacilityTypeID = DBUtility2.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nReportTypeID = DBUtility2.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nColumnID = DBUtility2.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                string strHeaderText = DBUtility2.WebDBManager.GetStringField(arrResult[i + 4].ToString(), "");
                int nWidthRatio = DBUtility2.WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);

                TypeColumns column = new TypeColumns();
                foreach (DefineColumns item in m_defineColumns)
                {
                    if (item.ColumnID == nColumnID)
                    {
                        column.DefineColumn = item;
                        break;
                    }
                }
                column.HeaderText = strHeaderText;
                column.ColumnWidthRatio = nWidthRatio;

                ReportType reportType = (ReportType)nReportTypeID;

                if (!m_dicDefineColumns.ContainsKey(IFacility.ToFacilityType(nFacilityTypeID)))
                    m_dicDefineColumns.Add(IFacility.ToFacilityType(nFacilityTypeID), new Dictionary<ReportType, List<TypeColumns>>());

                if (!m_dicDefineColumns[IFacility.ToFacilityType(nFacilityTypeID)].ContainsKey(reportType))
                    m_dicDefineColumns[IFacility.ToFacilityType(nFacilityTypeID)].Add(reportType, new List<TypeColumns>());

                m_dicDefineColumns[IFacility.ToFacilityType(nFacilityTypeID)][reportType].Add(column);
            }
        }

        private List<DefineColumns> m_defineColumns = null;
        private Dictionary<IFacility.FacilityType, Dictionary<ReportType, List<TypeColumns>>> m_dicDefineColumns = new Dictionary<IFacility.FacilityType, Dictionary<ReportType, List<TypeColumns>>>();
        public Dictionary<IFacility.FacilityType, Dictionary<ReportType, List<TypeColumns>>> DicDefineColumns
        {
            get { return m_dicDefineColumns; }
        }

        private string m_strHWPPath = null;
        public bool IsHwpSetup()
        {
            bool isHwpSetup = false;
            isHwpSetup = m_hwpCtrl.GetRegistry(ref m_strHWPPath);
            
            return isHwpSetup;
        }

        public string GetHWPFilePath(string strDocType, bool isHwpSetup)
        {
            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("__{0}{1:00}{2:00}_{3:00}{4:00}{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

            try
            {
                string strFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (strFolderPath != null && strFolderPath.Length > 0)
                {
                    if (!System.IO.Directory.Exists(strFolderPath + "\\리포트"))
                        System.IO.Directory.CreateDirectory(strFolderPath + "\\리포트");
                    
                    if (!isHwpSetup)
                    {
                        string temp = strTime.Replace("__", "");
                        return strFolderPath + "\\리포트\\" + temp + "\\" + strDocType + strTime;
                    }

                    return strFolderPath + "\\리포트\\" + strDocType + strTime;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("SDMS_Building.Report.uFormReport.GetHWPFilePath(string, bool) : " + ex.Message);
            }
            
            return null;
        }

        public void RunHWP(string strFilePath)
        {
            string strHmlFilePath = strFilePath + ".hml";
            // 대용량인 경우 파일에 번호가 붙는다. ex) 화재탐지분석_날짜_1
            // 1번 파일을 열어준다.
            if (!File.Exists(strHmlFilePath))
            {
                int nIndex = strFilePath.LastIndexOf(@"\");
                string filePath = strFilePath.Substring(0, nIndex);
                foreach (string item in System.IO.Directory.GetFiles(filePath))
                {
                    if (item.Contains(strFilePath))
                    {
                        if (item == strFilePath + "_1.hml")
                        {
                            strHmlFilePath = item;
                            break;
                        }
                    }
                }
            }

            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.Arguments = strHmlFilePath;
            info.FileName = m_strHWPPath;

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo = info;

            process.Start();
        }

        #region DateTime 형식
        private static string m_strSectionName = "Report DateTime Format";

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
        #endregion

        public void Display(IFacility.FacilityType alarmType)
        {
            if (m_uFrmPareto != null && m_uFrmPareto.CurFacilityType == alarmType)
            {
                m_uFrmPareto.btnSearch_Click(null, null);
            }

            if (m_uFrmNotOperation != null && m_uFrmNotOperation.CurFacilityType == alarmType)
            {
                m_uFrmNotOperation.btnSearch_Click(null, null);
            }

            if (m_uFrmDetect != null && m_uFrmDetect.CurFacilityType == alarmType)
            {
                m_uFrmDetect.btnSearch_Click(null, null);
            }
        }
    }

    public enum ReportMode
    {
        DetectFireAnalyze = 0,
        DetectFire = 1,
        ProcessFire = 2,
        ActionFire = 3,
        SMSFire = 4,
        DetectPSMAnalyze = 5,
        DetectPSM = 6,
        ProcessPSM = 7,
        ActionPSM = 8,
        SMSPSM = 9,
        //침입
        DetectIntrusionAnalyze = 10,
        DetectIntrusion = 11,
        ProcessIntrusion = 12,
        ActionIntrusion = 13,
        SMSIntrusion = 14,
        DisasterPrevention = 15, // 방재장비
        //지진
        DetectEarthquake = 16,
        ActionEarthquake = 17,
        // 온도/습도
        DetectTHAnalyze = 18,
        DetectTH = 19,
        ActionTH = 20
    }

    public enum ReportType
    {
        Pareto_Sensor = 0 /*탐지분석 - 센서별*/,
        Pareto_EquipZone = 10 /*탐지분석 - 위치별*/,
        Detect = 1 /*탐지이력*/,
        NotOperation = 2 /*처리이력*/
    }

    public class DefineColumns
    {
        public int ColumnID { get; set; }
        public string ColumnName { get; set; }
        public string Description { get; set; }
    }

    public class TypeColumns
    {
        public DefineColumns DefineColumn { get; set; }        
        public string HeaderText { get; set; }
        public int ColumnWidthRatio { get; set; }
    }
}
