using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;

namespace SOPMonitoringSystem
{
    public class SectionEx : Section
    {
        public enum SectionType { SUBDISASTER_SECTION, PROCESS_SECTION, GROUP_SECTION, INVALID_SECTION };
        public enum PROCESS_STATUS { STARTED, COMPLETE, WAITING };
        private static Color[] m_arrStatusColor = new Color[] { Color.FromArgb(142, 180, 227), Color.FromArgb(252, 213, 181), Color.White };

        private SectionType m_sectionType = SectionType.INVALID_SECTION;
        private int m_nBeginHour = -1, m_nBeginMinute = -1, m_nProcessHour = -1, m_nProcessMinute = -1;
        private StepMemberData m_stepMemberData = null;
        private MemberofSection m_missionData = null;

        protected static Pen BOUNDARY_PEN = new Pen(Color.FromArgb(185, 255, 185), 1);
        protected static SolidBrush BOUNDARY_BRUSH = new SolidBrush(Color.FromArgb(100, 128, 128, 192));

        private PROCESS_STATUS m_status = PROCESS_STATUS.WAITING;
        private SolidBrush m_brush = null;

        private ArrayList m_arrProcessBoundary = new ArrayList();

        private bool m_isCurrentProcess = false;

        // GROUP_SECTION 속성을 가진 Section(업무 수행팀)의 임무 및 점검 항목들을 Message 형태로 보관
        // 팀 별 sms 메시지 전송때 이 메시지들을 보내게 됨
        private ArrayList m_arrMessages = new ArrayList();

        public SectionEx(Form frmParent)
            : base(frmParent)
        {
            m_brush = new SolidBrush(m_arrStatusColor[(int)m_status]);
        }

        public SectionEx(Form frmParent, int x, int y)
            : base(frmParent, x, y)
        {
            m_brush = new SolidBrush(m_arrStatusColor[(int)m_status]);
        }

        public SectionEx(Form frmParent, int x, int y, int width, int height)
            : base(frmParent, x, y, width, height)
        {
            m_brush = new SolidBrush(m_arrStatusColor[(int)m_status]);
        }

        public void SetCurrentProcess(bool isCurrentProcess)
        {
            m_isCurrentProcess = isCurrentProcess;
        }

        public void SetTime(int nHour, int nMinute, bool isBegin)
        {
            if (isBegin)
            {
                m_nBeginHour = nHour;
                m_nBeginMinute = nMinute;
            }
            else
            {
                m_nProcessHour = nHour;
                m_nProcessMinute = nMinute;
            }
        }

        public void GetTime(out int nHour, out int nMinute, bool isBegin)
        {
            if (isBegin)
            {
                nHour = m_nBeginHour;
                nMinute = m_nBeginMinute;
            }
            else
            {
                nHour = m_nProcessHour;
                nMinute = m_nProcessMinute;
            }
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

        protected override void DrawRectangle(Graphics g, int xLeft, int yTop, int xRight, int yBottom)
        {
            int nPointCount = m_arrProcessBoundary.Count;
            Point ptScroll = m_frmParent.AutoScrollPosition;

            if (nPointCount > 2)
            {
                if (m_isCurrentProcess)
                {
                    Point[] arrBoundary = new Point[nPointCount];
                    for (int i=0;i<nPointCount;i++)
                    {
                        Point ptBoundary = (Point)m_arrProcessBoundary[i];
                        arrBoundary[i].X = ptBoundary.X + ptScroll.X;
                        arrBoundary[i].Y = ptBoundary.Y + ptScroll.Y;
                    }

                    g.FillPolygon(BOUNDARY_BRUSH, arrBoundary);
                }
                else
                {
                    for (int i = 0; i < nPointCount; i++)
                    {
                        Point prev = i == 0 ? (Point)m_arrProcessBoundary[nPointCount - 1] : (Point)m_arrProcessBoundary[i - 1];
                        Point current = (Point)m_arrProcessBoundary[i];

                        prev.X += ptScroll.X;
                        prev.Y += ptScroll.Y;
                        current.X += ptScroll.X;
                        current.Y += ptScroll.Y;

                        g.DrawLine(BOUNDARY_PEN, prev, current);
                    }
                }
            }

            g.FillRectangle(m_brush, xLeft, yTop, xRight - xLeft, yBottom - yTop);
        }

        public SectionType Type
        {
            get { return m_sectionType; }
            set { m_sectionType = value; }
        }

        public StepMemberData StepMember
        {
            get { return m_stepMemberData; }
            set { m_stepMemberData = value; }
        }

        public MemberofSection MissionData
        {
            get { return m_missionData; }
            set
            {
                m_missionData = value;
                if (m_missionData != null)
                {
                    if (m_missionData.LinkedSection != this)
                        m_missionData.LinkedSection = this;
                }
            }
        }

        public PROCESS_STATUS Status
        {
            get { return m_status; }
            set
            {
                if (m_status == PROCESS_STATUS.WAITING && m_sectionType == SectionType.GROUP_SECTION)
                {
                    // Section의 상태가 대기상태에서 실행 또는 종료로 바뀌면, 임무가 실행되었다는 의미이므로
                    // 관련 팀들에게 문자 메시지를 보낸다.
                    if (value == PROCESS_STATUS.STARTED || value == PROCESS_STATUS.COMPLETE)
                    {
                        FormProcess frmProcess = (FormProcess)m_frmParent;
                        frmProcess.SendSMSMessage(this);
                    }
                }

                if (value == PROCESS_STATUS.COMPLETE && GetTextBox().Text == "종료")
                    m_status = value;
                m_status = value;
                m_brush.Color = m_arrStatusColor[(int)m_status];
                GetTextBox().BackColor = m_brush.Color;
            }
        }

        public ArrayList Boundary
        {
            get { return m_arrProcessBoundary; }
        }

        public ArrayList SMSMessages
        {
            get { return m_arrMessages; }
        }
    }

    public class StepMemberData
    {
        private int m_nID = -1;
        private int m_nActionStepID = -1;
        private string m_strMemberName = "";
        private int m_nMemberType = -1; // 1(상시조직), 2(비상조직), 3(팀원)
        private int m_nMemberID = -1;

        public StepMemberData()
        {
        }

        public StepMemberData(int nID, int nMemberID, int nActionStepID, string strMemberName, int nMemberType)
        {
            m_nID = nID;
            m_nMemberID = nMemberID;
            m_nActionStepID = nActionStepID;
            m_strMemberName = strMemberName;
            m_nMemberType = nMemberType;
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int MemberID
        {
            get { return m_nMemberID; }
            set { m_nMemberID = value; }
        }

        public int ActionStepID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string MemberName
        {
            get { return m_strMemberName; }
            set { m_strMemberName = value; }
        }

        public int MemberType
        {
            get { return m_nMemberType; }
            set { m_nMemberType = value; }
        }
    }

    public class MemberofSection
    {
        public class MissionofSection
        {
            string m_strDivision;
            string m_strTaskName;
            string m_strReport;
            string m_strDescription;
            string m_strLocation = "";

            private ArrayList m_arrCheckItems = new ArrayList();

            public string Division
            {
                get { return m_strDivision; }
                set { m_strDivision = value; }
            }

            public string TaskName
            {
                get { return m_strTaskName; }
                set { m_strTaskName = value; }
            }

            public string Report
            {
                get { return m_strReport; }
                set { m_strReport = value; }
            }

            public string Description
            {
                get { return m_strDescription; }
                set { m_strDescription = value; }
            }

            public string Location
            {
                get { return m_strLocation; }
                set { m_strLocation = value; }
            }

            public ArrayList CheckItems
            {
                get { return m_arrCheckItems; }
                set { m_arrCheckItems = value; }
            }
        }

        public class CheckofMission
        {
            string m_strCategory;
            string m_strSubCategory;
            string m_strTaskName;
            string m_strCount;
            string m_strDescription;
            string m_strLocation = "";

            public string Category
            {
                get { return m_strCategory; }
                set { m_strCategory = value; }
            }

            public string SubCategory
            {
                get { return m_strSubCategory; }
                set { m_strSubCategory = value; }
            }

            public string TaskName
            {
                get { return m_strTaskName; }
                set { m_strTaskName = value; }
            }

            public string Count
            {
                get { return m_strCount; }
                set { m_strCount = value; }
            }

            public string Description
            {
                get { return m_strDescription; }
                set { m_strDescription = value; }
            }

            public string Location
            {
                get { return m_strLocation; }
                set { m_strLocation = value; }
            }
        }

        string m_strMember;
        string m_strCellphone1;
        string m_strCellphone2;
        string m_strCellphone3;
        string m_strTelephone1;
        string m_strTelephone2;
        string m_strTelephone3;
        string m_strMessengerID;
        SectionEx m_linkedSection = null;

        private ArrayList m_arrMissions = new ArrayList();

        public string Member
        {
            get { return m_strMember; }
            set { m_strMember = value; }
        }

        public string CellPhone1
        {
            get { return m_strCellphone1; }
            set { m_strCellphone1 = value; }
        }

        public string CellPhone2
        {
            get { return m_strCellphone2; }
            set { m_strCellphone2 = value; }
        }

        public string CellPhone3
        {
            get { return m_strCellphone3; }
            set { m_strCellphone3 = value; }
        }

        public string Telephone1
        {
            get { return m_strTelephone1; }
            set { m_strTelephone1 = value; }
        }

        public string Telephone2
        {
            get { return m_strTelephone2; }
            set { m_strTelephone2 = value; }
        }

        public string Telephone3
        {
            get { return m_strTelephone3; }
            set { m_strTelephone3 = value; }
        }

        public string MessengerID
        {
            get { return m_strMessengerID; }
            set { m_strMessengerID = value; }
        }

        public ArrayList Missions
        {
            get { return m_arrMissions; }
            set { m_arrMissions = value; }
        }

        public SectionEx LinkedSection
        {
            get { return m_linkedSection; }
            set
            {
                m_linkedSection = value;
                if (m_linkedSection != null)
                {
                    if (m_linkedSection.MissionData != this)
                        m_linkedSection.MissionData = this;
                }
            }
        }
    }
}
