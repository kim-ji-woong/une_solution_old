using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Data.SqlClient;

namespace SOPMonitoringSystem
{
    public partial class FormProcess : Form
    {
//         private Dictionary<int, MemberofSection> m_dicMissions = new Dictionary<int,MemberofSection>();
//         private Dictionary<int, MemberofSection.MissionofSection> m_dicTask = new Dictionary<int, MemberofSection.MissionofSection>();

        //private Dictionary<SOPData, ArrayList> m_dicSections = new Dictionary<SOPData, ArrayList>();
        private Dictionary<SOPData, SectionViewData> m_dicSections = new Dictionary<SOPData, SectionViewData>();
        private Size m_sizeNormal;
        private SectionEx m_sectionSelected = null;
        private SOPData m_currentSOP = null;
        private FormMain m_frmMain = null;
        private int m_nRightEnd = 0;
        
        private int m_nTickCount = 0;
        private DateTime m_dateTime = new DateTime();
        private ElapsedTime m_elapsedTime= new ElapsedTime();

        private bool m_requestRefresh = false;
        private SectionViewData m_requestComplete = null;

        public const int RECT_UP = 10;

        private static Point FIRST_POINT = new Point(150, 50);

        private ArrayList m_arrPlayingProcess = new ArrayList();

        // RegularTeamID, 팀원 리스트(ArrayList<MemberInfo>)
        private Dictionary<int, ArrayList> m_dicTeamInfo = new Dictionary<int, ArrayList>();
        private Dictionary<int, MemberInfo> m_dicMemberInfo = new Dictionary<int, MemberInfo>();

        ezSMSComponent.ISMS m_sms = new ezSMSComponent.SMS();
        private bool m_useSMS = false;

        //private ArrayList m_arrAdd = new ArrayList();

        public FormProcess(FormMain frmMain)
        {
            InitializeComponent();

            m_frmMain = frmMain;
            tsSOPControlMenu_ImageLoad();

            timer1.Start();
        }

        private void FormProcess_Load(object sender, EventArgs e)
        {
            Section temp = new Section(this);
            m_sizeNormal = temp.RectSize;
            temp.Hide();

            this.SetStyle(ControlStyles.DoubleBuffer | /*ControlStyles.UserPaint |*/ ControlStyles.AllPaintingInWmPaint, true);
            FIRST_POINT.Y = tsSOPControlMenu.Height + 30;

            label4Scroll.Text = "";

            LoadCompanyMember();

            try
            {
                m_sms.ServiceCode = "020026C9FCC7C39E41A88C2CF52D00D7BAA6";
                ezSMSComponent.LoginInfo login = m_sms.Login("121.254.175.25", 4545, "unes", "unes0101");
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                MessageBox.Show("SMS Component 초기화에 실패하였습니다.\r\n프로그램을 다시 실행하여 주세요.");
                Application.Exit();
            }

            string strSMSOn = m_frmMain.GetDBManager().LoadIni("sms_on");
            m_useSMS = strSMSOn == "1";
        }

        // 팀별 인원수를 미리 읽어둔다.
        private void LoadCompanyMember()
        {
            WebDBManager dbMgr = m_frmMain.GetDBManager();

            string strSQL = "Select ID, MemberName, RegularTeamID from CompanyMember order by RegularTeamID";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-2;i+=3)
            {
                int nID = dbMgr.GetIntField(arrResult[i].ToString(), 0);
                string strMemberName = dbMgr.GetStringField(arrResult[i + 1].ToString(), "");
                int nTeamID = dbMgr.GetIntField(arrResult[i + 2].ToString(), 0);

                MemberInfo member = new MemberInfo(nID, strMemberName, "", nTeamID);

                if (nTeamID > 0 && strMemberName.Length > 0 && nTeamID > 0)
                {
                    if (m_dicTeamInfo.ContainsKey(nTeamID))
                        m_dicTeamInfo[nTeamID].Add(member);
                    else
                    {
                        ArrayList arrTeamMember = new ArrayList();
                        arrTeamMember.Add(member);
                        m_dicTeamInfo[nTeamID] = arrTeamMember;
                    }

                    m_dicMemberInfo[nID] = member;
                }
            }
        }

        private void tsSOPControlMenu_ImageLoad()
        {
            //Bitmap bmpSOPControlMenu = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.toolbar_SOPcontrol);
            //ImageList SOPControlMenuList = new ImageList();
            //SOPControlMenuList.ImageSize = new Size(16, 16);
            //SOPControlMenuList.Images.AddStrip(bmpSOPControlMenu);
            Bitmap bmpSOPControlMenu = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.toolbar_SOPcontrol2);
            ImageList SOPControlMenuList = new ImageList();
            SOPControlMenuList.ImageSize = new Size(32, 32);
            SOPControlMenuList.Images.AddStrip(bmpSOPControlMenu);

            tsSOPControlMenu.ImageList = SOPControlMenuList;

            tsbtnPlay.ImageIndex = 0;
            tsbtnPause.ImageIndex = 1;
            tsbtnStop.ImageIndex = 2;
            tsbtnRestart.ImageIndex = 3;

            tsbtnFullScreen.ImageIndex = 4;
            tsbtnZoomIn.ImageIndex = 5;
            tsbtnZoomOut.ImageIndex = 6;
            tsbtnPan.ImageIndex = 7;
        }

        public ArrayList GetCurrentSections()
        {
            if (m_currentSOP == null)
                return null;

            if (!m_dicSections.ContainsKey(m_currentSOP))
                return null;

            return m_dicSections[m_currentSOP].Sections;
        }

        public SectionViewData GetCurrentSectionViewData()
        {
            if (m_currentSOP == null)
                return null;

            if (!m_dicSections.ContainsKey(m_currentSOP))
                return null;

            return m_dicSections[m_currentSOP];
        }

        public void ClearSOP()
        {
            foreach (KeyValuePair<SOPData, SectionViewData> pair in m_dicSections)
            {
                ArrayList arrSections = pair.Value.Sections;

                foreach (Section section in arrSections)
                {
                    section.Hide();
                }

                arrSections.Clear();
            }

            m_dicSections.Clear();
        }

        private bool ReadSubDisaster(WebDBManager dbMgr, string strSQL, ref int nParentID, out string strSubDisasterName)
        {
            strSubDisasterName = null;
            if (dbMgr == null) return false;

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult.Count >= 2)
            {
                nParentID = dbMgr.GetIntField(arrResult[0].ToString(), 0);
                strSubDisasterName = dbMgr.GetStringField(arrResult[1].ToString(), "");
            }
            else
            {
                return false;
            }

            return true;
        }

        public SectionEx AddSection(string strSectionName, SectionEx.SectionType sectionType, SectionEx sectionParent, bool autoAlign = true)
        {
            ArrayList arrSections = GetCurrentSections();
            if (arrSections == null) return null;

            SectionEx section = new SectionEx(this);
            section.Type = sectionType;

            if (sectionParent == null)
            {
                section.GetTextBox().Text = strSectionName;
                arrSections.Add(section);

                int nSectionCount = arrSections.Count;

                if (nSectionCount == 0)
                {
                    section.Position = FIRST_POINT;
                    Refresh();
                }
                else
                {
                    SectionEx lastSection = (SectionEx)arrSections[nSectionCount - 1];
                    lastSection.SetNext(section);

                    if (autoAlign) AutoAlign();
                }
            }
            else
            {
                section.GetTextBox().Text = strSectionName;

                sectionParent.AddChild(section);

                if (sectionType == SectionEx.SectionType.PROCESS_SECTION)
                {
                    ArrayList arrChilds = section.GetChildSections();
                    int nChildCount = arrChilds.Count;

                    if (nChildCount > 1)
                    {
                        SectionEx sectionPrev = (SectionEx)arrChilds[nChildCount - 2];
                        sectionPrev.SetNext(section);
                    }
                }
                else if (sectionType == SectionEx.SectionType.GROUP_SECTION)
                {
                    section.MissionData = new MemberofSection();
                }

                if (autoAlign) AutoAlign();
            }

            return section;
        }

        // Section의 가장 오른쪽 아래 모서리 Position
        protected Point GetRightPosition(Point ptBR = new Point(), ArrayList arrSections = null)
        {
            if (arrSections == null)
                arrSections = GetCurrentSections();

            if (arrSections == null)
                return ptBR;

            foreach (SectionEx section in arrSections)
            {
                ArrayList arrBoundary = section.Boundary;

                if (arrBoundary != null)
                {
                    foreach (Point pt in arrBoundary)
                    {
                        if (ptBR.X < pt.X) ptBR.X = pt.X;
                        if (ptBR.Y < pt.Y) ptBR.Y = pt.Y;
                    }
                }

                ptBR = GetRightPosition(ptBR, section.GetChildSections());
            }

            return ptBR;
        }

        // Return 값 : 최종 SubDisaster Section
        private SectionEx LoadSubDisaster(SOPData data, int nVersionID, out int nSubDisasterID)
        {
            nSubDisasterID = -1;

            string strDisaster = data.FullName;
            int nIndex = strDisaster.IndexOf('/');
            if (nIndex < 0) return null;

            string strSubDisaster = strDisaster.Substring(nIndex + 1);
            strDisaster = strDisaster.Substring(0, nIndex);

            int nBeginIndex = 0;
            nIndex = strSubDisaster.IndexOf('/');
            int nParentID = -1;
            string strSQL = "";

            string strSubDisasterName;
            SectionEx sectionParent = null;

            while (nIndex >= 0)
            {
                if (nParentID >= 0)
                    strSQL = string.Format("select ID, SubCategoryName from SubDisasterCategory where ParentSubCategoryID = {0} and SubCategoryName = '{1}' and VersionID = {2}", nParentID, strSubDisaster.Substring(nBeginIndex, nIndex - nBeginIndex), nVersionID);
                else
                    strSQL = string.Format("select ID, SubCategoryName from SubDisasterCategory where SubCategoryName = '{0}' and VersionID = {1} and DisasterID = (select id from DisasterCategory where CategoryName = '{2}')", strSubDisaster.Substring(nBeginIndex, nIndex - nBeginIndex), nVersionID, strDisaster);

                nBeginIndex = nIndex + 1;

                if (!ReadSubDisaster(m_frmMain.GetDBManager(), strSQL, ref nParentID, out strSubDisasterName))
                    return null;

                sectionParent = AddSection(strSubDisasterName, SectionEx.SectionType.SUBDISASTER_SECTION, sectionParent, false);
                nIndex = strSubDisaster.IndexOf('/', nBeginIndex);
            }

            if (nParentID >= 0)
                strSQL = string.Format("select ID, SubCategoryName from SubDisasterCategory where ParentSubCategoryID = {0} and SubCategoryName = '{1}' and VersionID = {2}", nParentID, strSubDisaster.Substring(nBeginIndex), nVersionID);
            else
                strSQL = string.Format("select ID, SubCategoryName from SubDisasterCategory where SubCategoryName = '{0}' and VersionID = {1} and DisasterID = (select id from DisasterCategory where CategoryName = '{2}')", strSubDisaster.Substring(nBeginIndex), nVersionID, strDisaster);

            if (!ReadSubDisaster(m_frmMain.GetDBManager(), strSQL, ref nParentID, out strSubDisasterName))
                return null;

            nSubDisasterID = nParentID;
            return AddSection(strSubDisasterName, SectionEx.SectionType.SUBDISASTER_SECTION, sectionParent, false);
        }

        // dicProcess : Key(ActionStepID), Value(Section)
        private bool LoadActionStep(SectionEx sectionParent, int nSubDisasterID, int nVersionID, out Dictionary<int, SectionEx> dicProcess)
        {
            dicProcess = new Dictionary<int,SectionEx>();

            string strSQL = string.Format("select * from ActionStep where SubDisasterID = {0} and VersionID = {1}", nSubDisasterID, nVersionID);

            WebDBManager dbMgr = m_frmMain.GetDBManager();

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            for (int i = 0; i < arrResult.Count - 5; i = i + 6)
            {
                int nID = dbMgr.GetIntField(arrResult[i].ToString(), 0);
                string strStepName = dbMgr.GetStringField(arrResult[i + 1].ToString(), "");
                string strBeginTime = dbMgr.GetStringField(arrResult[i + 2].ToString(), "");
                string strProcessTime = dbMgr.GetStringField(arrResult[i + 3].ToString(), "");

                SectionEx section = AddSection(strStepName, SectionEx.SectionType.PROCESS_SECTION, sectionParent, false);

                if (section == null)
                {
                    MessageBox.Show(string.Format("{0} 이름의 Process를 만들수 없습니다.", strStepName));
                    return false;
                }

                int nHour, nMinute;
                if (!SectionEx.TextToTime(strBeginTime, "", out nHour, out nMinute))
                    return false;

                section.SetTime(nHour, nMinute, true);

                if (!SectionEx.TextToTime(strProcessTime, "", out nHour, out nMinute))
                    return false;

                section.SetTime(nHour, nMinute, false);
                dicProcess[nID] = section;
            }

            return true;
        }

        private bool ReadStepMember(string strSQL, Dictionary<int, SectionEx> dicProcess, Dictionary<int, SectionEx> dicGroupSections)
        {
            WebDBManager dbMgr = m_frmMain.GetDBManager();

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            for (int i = 0; i < arrResult.Count - 6; i = i + 7)
            {
                int nID = dbMgr.GetIntField(arrResult[i].ToString(), 0);
                int nActionStepID = dbMgr.GetIntField(arrResult[i + 1].ToString(), 0);
                string strMemberName = dbMgr.GetStringField(arrResult[i + 2].ToString(), "");
                int nMemberType = dbMgr.GetIntField(arrResult[i + 3].ToString(), 0);
                string strBeginTime = dbMgr.GetStringField(arrResult[i + 4], "");
                string strProcessTime = dbMgr.GetStringField(arrResult[i + 5], "");
                int nMemberID = dbMgr.GetIntField(arrResult[i + 6].ToString(), 0);

                if (!dicProcess.ContainsKey(nActionStepID))
                {
                    return false;
                }

                SectionEx section = AddSection(strMemberName, SectionEx.SectionType.GROUP_SECTION, dicProcess[nActionStepID], false);
                if (section == null)
                {
                    return false;
                }

                int nHour, nMinute;
                if (!SectionEx.TextToTime(strBeginTime, "", out nHour, out nMinute))
                    return false;

                section.SetTime(nHour, nMinute, true);

                if (!SectionEx.TextToTime(strProcessTime, "", out nHour, out nMinute))
                    return false;

                section.SetTime(nHour, nMinute, false);

                StepMemberData data = new StepMemberData(nID, nMemberID, nActionStepID, strMemberName, nMemberType);
                section.StepMember = data;

                dicGroupSections[nID] = section;
            }

            return true;
        }

        private bool LoadStepMember(Dictionary<int, SectionEx> dicProcess, out Dictionary<int, SectionEx> dicGroupSections)
        {
            dicGroupSections = new Dictionary<int, SectionEx>();

            if (dicProcess.Count == 0)
                return false;

            string strCondition = "(";
            bool isFirst = true;

            foreach (KeyValuePair<int, SectionEx> pair in dicProcess)
            {
                int nID = pair.Key;

                if (isFirst)
                {
                    strCondition += nID.ToString();
                    isFirst = false;
                }
                else
                    strCondition += ", " + nID.ToString();
            }

            strCondition += ")";

            // MemberType이 1이면 상시조직
            //string strSQL = string.Format("select StepMember.MemberID, ActionStepID, RegularTeam.TeamName, MemberType, BeginTime, ProcessTime from StepMember, RegularTeam where StepMember.MemberType = 1 and RegularTeam.id = StepMember.MemberID and ActionStepID in {0} order by ActionStepID", strCondition);
            string strSQL = string.Format("select StepMember.ID, ActionStepID, RegularTeam.TeamName, MemberType, BeginTime, ProcessTime, StepMember.MemberID from StepMember, RegularTeam where StepMember.MemberType = 1 and RegularTeam.id = StepMember.MemberID and ActionStepID in {0} order by ActionStepID", strCondition);
            if (!ReadStepMember(strSQL, dicProcess, dicGroupSections))
                return false;

            // MemberType이 2이면 비상조직
            strSQL = string.Format("select StepMember.ID, ActionStepID, TemporaryNormalTeam.TeamName, MemberType, BeginTime, ProcessTime, StepMember.MemberID from StepMember, TemporaryNormalTeam where StepMember.MemberType = 2 and TemporaryNormalTeam.id = StepMember.MemberID and ActionStepID in {0} order by ActionStepID", strCondition);
            if (!ReadStepMember(strSQL, dicProcess, dicGroupSections))
                return false;

            // MemberType이 3이면 상시조직의 팀원
            strSQL = string.Format("select StepMember.ID, ActionStepID, CompanyMember.MemberName, MemberType, BeginTime, ProcessTime, StepMember.MemberID from StepMember, CompanyMember where StepMember.MemberType = 2 and CompanyMember.id = StepMember.MemberID and ActionStepID in {0} order by ActionStepID", strCondition);
            if (!ReadStepMember(strSQL, dicProcess, dicGroupSections))
                return false;

            return true;
        }

        private bool LoadMission(Dictionary<int, SectionEx> dicGroupSections, out Dictionary<int, MemberofSection> dicMissions, out string strMissionCondition)
        {
            strMissionCondition = "";
            dicMissions = new Dictionary<int, MemberofSection>();

            if (dicGroupSections.Count == 0)
                return true;

            string strCondition = "(";
            bool isFirst = true;

            foreach (KeyValuePair<int, SectionEx> pair in dicGroupSections)
            {
                StepMemberData data = pair.Value.StepMember;
                if (data == null) return false;

                if (isFirst)
                {
                    strCondition += data.ID.ToString();
                    isFirst = false;
                }
                else
                    strCondition += ", " + data.ID.ToString();
            }

            strCondition += ")";

            string strSQL = string.Format("select * from MissionInfo where StepMemberID in {0} order by StepMemberID", strCondition);

            WebDBManager dbMgr = m_frmMain.GetDBManager();

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            strMissionCondition = "(";
            isFirst = true;

            for (int i=0;i<arrResult.Count - 8;i += 9)
            {
                int nID = dbMgr.GetIntField(arrResult[i].ToString(), 0);
                int nStepMemberID = dbMgr.GetIntField(arrResult[i + 1].ToString(), 0);
                string strCellPhone1 = dbMgr.GetStringField(arrResult[i + 2].ToString(), "");
                string strCellPhone2 = dbMgr.GetStringField(arrResult[i + 3].ToString(), "");
                string strCellPhone3 = dbMgr.GetStringField(arrResult[i + 4].ToString(), "");
                string strPhone1 = dbMgr.GetStringField(arrResult[i + 5].ToString(), "");
                string strPhone2 = dbMgr.GetStringField(arrResult[i + 6].ToString(), "");
                string strPhone3 = dbMgr.GetStringField(arrResult[i + 7].ToString(), "");
                string strMessangerID = dbMgr.GetStringField(arrResult[i + 8].ToString(), "");

                SectionEx section = dicGroupSections[nStepMemberID];
                MemberofSection mission = section.MissionData;

                mission.CellPhone1 = strCellPhone1;
                mission.CellPhone2 = strCellPhone2;
                mission.CellPhone3 = strCellPhone3;
                mission.Telephone1 = strPhone1;
                mission.Telephone2 = strPhone2;
                mission.Telephone3 = strPhone3;
                mission.MessengerID = strMessangerID;

                if (isFirst)
                {
                    strMissionCondition += nID.ToString();
                    isFirst = false;
                }
                else
                    strMissionCondition += ", " + nID.ToString();

                dicMissions[nID] = mission;
            }

            //m_dicMissions = dicMissions;
            strMissionCondition += ")";

            return true;
        }

        private bool LoadCheckTask(Dictionary<int, MemberofSection.MissionofSection> dicTask, string strTaskCondition)
        {
            string strSQL = string.Format("select * from CheckTask where TaskID in {0} order by TaskID", strTaskCondition);

            WebDBManager dbMgr = m_frmMain.GetDBManager();

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            for (int i=0;i<arrResult.Count - 6;i+=7)
            {
                int nTaskID = dbMgr.GetIntField(arrResult[i + 1].ToString(), 0);
                string strSubCategory = dbMgr.GetStringField(arrResult[i + 2].ToString(), "");
                string strTaskName = dbMgr.GetStringField(arrResult[i + 3].ToString(), "");
                string strDescription = dbMgr.GetStringField(arrResult[i + 4].ToString(), "");
                int nTargetCount = dbMgr.GetIntField(arrResult[i + 5].ToString(), 0);

                if (!dicTask.ContainsKey(nTaskID))
                    return false;

                MemberofSection.MissionofSection task = dicTask[nTaskID];
                MemberofSection.CheckofMission checkTask = new MemberofSection.CheckofMission();

                checkTask.Category = task.Division;
                checkTask.Count = nTargetCount.ToString();
                checkTask.Description = strDescription;
                checkTask.SubCategory = strSubCategory;
                checkTask.TaskName = strTaskName;

                task.CheckItems.Add(checkTask);
            }
            //m_dicTask = dicTask;
            return true;
        }

        private bool LoadTask(Dictionary<int, MemberofSection> dicMissions, string strMissionCondition)
        {
            if (strMissionCondition == "")
                return true;

            string strSQL = string.Format("select * from Task where MissionInfoID in {0} order by MissionInfoID", strMissionCondition);

            WebDBManager dbMgr = m_frmMain.GetDBManager();

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            Dictionary<int, MemberofSection.MissionofSection> dicTask = new Dictionary<int, MemberofSection.MissionofSection>();
            string strCondition = "(";
            bool isFirst = true;

            for (int i=0;i<arrResult.Count - 4;i+=5)
            {
                int nID = dbMgr.GetIntField(arrResult[i].ToString(), 0);
                int nMissionInfoID = dbMgr.GetIntField(arrResult[i + 1].ToString(), 0);
                string strCategory = dbMgr.GetStringField(arrResult[i + 2], "");
                string strTaskName = dbMgr.GetStringField(arrResult[i + 3], "");
                string strDescription = dbMgr.GetStringField(arrResult[i + 4], "");

                if (!dicMissions.ContainsKey(nMissionInfoID))
                {
                    return false;
                }

                MemberofSection mission = dicMissions[nMissionInfoID];
                MemberofSection.MissionofSection task = new MemberofSection.MissionofSection();

                task.Division = strCategory;
                task.TaskName = strTaskName;
                task.Description = strDescription;

                mission.Missions.Add(task);
                dicTask[nID] = task;

                if (isFirst)
                {
                    strCondition += nID.ToString();
                    isFirst = false;
                }
                else
                    strCondition += ", " + nID.ToString();
            }

            strCondition += ")";

            if (dicTask.Count > 0)
            {
                if (!LoadCheckTask(dicTask, strCondition))
                    return false;
            }

            return true;
        }

        // strSubDisaster : SubDisaster의 계층 관계는 '/'로 구분하여 표기된다.
        //                  예) 태풍/예방
        // isNormalTime : 평일 근무시간에 발생한 SOP 인가?
        //public bool LoadSOP(string strDisaster, string strSubDisaster, int nVersionID, bool isNormalTime)
        public bool LoadSOP(SOPData data, int nVersionID, bool isNormalTime)
        {
            if (m_frmMain == null) return false;

            SOPData prevSOPData = m_currentSOP;
            m_currentSOP = data;

            int nSubDisasterID;
            SectionEx sectionLastSubDisaster = LoadSubDisaster(data, nVersionID, out nSubDisasterID);
            if (sectionLastSubDisaster == null)
            {
                m_currentSOP = prevSOPData;
                return false;
            }

            Dictionary<int, SectionEx> dicProcess;
            if (!LoadActionStep(sectionLastSubDisaster, nSubDisasterID, nVersionID, out dicProcess))
            {
                m_currentSOP = prevSOPData;
                return false;
            }

            Dictionary<int, SectionEx> dicGroupSections;
            if (!LoadStepMember(dicProcess, out dicGroupSections))
            {
                m_currentSOP = prevSOPData;
                return false;
            }

            string strMissionCondition;
            Dictionary<int, MemberofSection> dicMissions;
            if (!LoadMission(dicGroupSections, out dicMissions, out strMissionCondition))
            {
                m_currentSOP = prevSOPData;
                return false;
            }

            if (!LoadTask(dicMissions, strMissionCondition))
            {
                m_currentSOP = prevSOPData;
                return false;
            }

            int nTimeLineEndPos = AutoAlign();

            Point ptScroll = GetRightPosition();
            label4Scroll.Location = ptScroll;

            if (prevSOPData != null)
                ShowSections(m_dicSections[prevSOPData].Sections, false);

            SetTimeLine(m_dicSections[m_currentSOP], nTimeLineEndPos);
            
            return true;
        }

        private void SetTimeLine(SectionViewData data, int nTimeLineEndPos)
        {
            if (data == null) return;

            ArrayList arrSections = data.Sections;
            TimeLine timeLine = data.TimeLineNormal;
            timeLine.TimeLineEndPos = nTimeLineEndPos;

            _SetTimeLine(arrSections, timeLine);
        }

        private void _SetTimeLine(ArrayList arrSections, TimeLine timeLine)
        {
            if (arrSections == null)
                return;

            int nHour, nMin;

            foreach (SectionEx section in arrSections)
            {
                if (section.Type == SectionEx.SectionType.PROCESS_SECTION)
                {
                    section.GetTime(out nHour, out nMin, true);
                    timeLine.AddTime(nHour, nMin, section.Position.Y);
                }
                else
                    _SetTimeLine(section.GetChildSections(), timeLine);
            }
        }

        // Return 값 : AutoAlign이 끝난후 다음에 배치할 Section의 Y좌표
        public int AutoAlign()
        {
            ArrayList arrSections = GetCurrentSections();
            if (arrSections == null) return 0;

            if (arrSections.Count == 0)
                return 0;

            if (m_currentSOP != null)
                m_dicSections[m_currentSOP].ProcessSections.Clear();

            m_nRightEnd = 0;

            SectionEx sectionFirst = (SectionEx)arrSections[0];
            sectionFirst.Position = FIRST_POINT;
            Point pt = sectionFirst.Position;

            // 스크롤 영역 계산
            Point ptScroll = this.AutoScrollPosition;
            pt.X = pt.X - ptScroll.X;
            pt.Y = pt.Y - ptScroll.Y;
            /////////////////////////

            _AutoAlign(arrSections, ref pt, ptScroll);

            Refresh();
            return pt.Y;
        }

        public void _AutoAlign(ArrayList arrSections, ref Point pt, Point ptScroll)
        {
            foreach (SectionEx section in arrSections)
            {
                section.Position = pt;
                //section.SetInterpolation(ptScroll.X, ptScroll.Y);
                int x = pt.X + section.GetDiffText(true) - section.GetTextBox().Left;
                int y = pt.Y + section.GetDiffText(false) - section.GetTextBox().Top;
                section.SetInterpolation(x == 0 ? ptScroll.X : x, y == 0 ? ptScroll.Y : y);

                Point ptChild = new Point(pt.X + section.RectSize.Width + m_sizeNormal.Width, pt.Y);

                int nRightEnd = pt.X + section.RectSize.Width + m_sizeNormal.Width / 2;
                if (m_nRightEnd < nRightEnd) m_nRightEnd = nRightEnd;

                ArrayList childList = section.GetChildSections();

                if (childList.Count > 0)
                {
                    _AutoAlign(childList, ref ptChild, ptScroll);
                    pt.Y = ptChild.Y;
                }
                else
                {
                    pt.Y = pt.Y + section.RectSize.Height + m_sizeNormal.Height;
                }

                if (section.Type == SectionEx.SectionType.PROCESS_SECTION)
                {
                    CalcProcessArea(section, ptChild.X + section.RectSize.Width + m_sizeNormal.Width / 2, pt.Y);
                }
            }
        }

        private void CalcProcessArea(SectionEx section, int right, int bottom)
        {
            ArrayList arrBoundary = section.Boundary;

            int left = section.Position.X;
            int top = section.Position.Y;

            arrBoundary.Add(new Point(FIRST_POINT.X, top - RECT_UP));
            arrBoundary.Add(new Point(right, top - RECT_UP));
            arrBoundary.Add(new Point(right, bottom - RECT_UP - 7));
            arrBoundary.Add(new Point(FIRST_POINT.X, bottom - RECT_UP - 7));
            arrBoundary.Add(new Point(FIRST_POINT.X - 30, (top + bottom) / 2 - 10));

            if (m_currentSOP != null)
                m_dicSections[m_currentSOP].ProcessSections.Add(section);
        }

        private void DrawTitleImage(Graphics g)
        {
            Size sizeClient = this.ClientSize;
            //Image img = Image.FromFile("SOP_poster.jpg");
            Image img = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.SOP_poster);

            sizeClient.Height -= tsSOPControlMenu.Size.Height;

            int x, y;
            int nWidth, nHeight;

            if (sizeClient.Width * img.Height < img.Width * sizeClient.Height)  // Width에 맞춘다.
            {
                if (sizeClient.Width >= img.Width)
                {
                    x = (sizeClient.Width - img.Width) / 2;
                    y = (sizeClient.Height - img.Height) / 2;
                    nWidth = img.Width;
                    nHeight = img.Height;
                }
                else
                {
                    nWidth = (int)(sizeClient.Width * 0.8);
                    nHeight = img.Height * nWidth / img.Width;
                    x = (sizeClient.Width - nWidth) / 2;
                    y = (sizeClient.Height - nHeight) / 2;
                }
            }
            else
            {
                if (sizeClient.Height >= img.Height)
                {
                    x = (sizeClient.Width - img.Width) / 2;
                    y = (sizeClient.Height - img.Height) / 2;
                    nWidth = img.Width;
                    nHeight = img.Height;
                }
                else
                {
                    nHeight = (int)(sizeClient.Height * 0.8);
                    nWidth = img.Width * nHeight / img.Height;
                    x = (sizeClient.Width - nWidth) / 2;
                    y = (sizeClient.Height - nHeight) / 2;
                }
            }

            g.DrawImage(img, new Rectangle(x, y + tsSOPControlMenu.Size.Height, nWidth, nHeight));

            img.Dispose();
        }
        
        private void FormProcess_Paint(object sender, PaintEventArgs e)
        {
            SectionViewData data = GetCurrentSectionViewData();
            if (data == null)
            {
                DrawTitleImage(e.Graphics);
                return;
            }

            data.TimeLineNormal.Draw(e.Graphics);

            ArrayList arrSections = data.Sections;

            if (arrSections != null)
            {
                foreach (SectionEx section in arrSections)
                {
                    section.Draw(e.Graphics);
                }
            }
        }

        private SOPData FindSOPData(TreeNode node)
        {
            foreach (KeyValuePair<SOPData, SectionViewData> sop in m_dicSections)
            {
                if (sop.Key.Node == node)
                    return sop.Key;
            }

            return null;
        }

        private void SetStartTime(SectionViewData data)
        {
            toolStripLabel1.Text = m_dateTime.ToString(GetTime());
            //timer1.Start();
            m_nTickCount = 0;
            //m_elapsedTime.StartTime = m_dateTime;

            data.ElapsedTimeData.StartTime = FlexTimer.Now;//DateTime.Now;
            m_frmMain.GetProgress().GetStartTime(data.ElapsedTimeData.StartTime);
        }

        private void tsbtnPlay_Click(object sender, EventArgs e)
        {
            FormLeftDisaster frmDisaster = m_frmMain.GetDisaster();
            if (frmDisaster == null) return;

            TreeNode node = frmDisaster.GetSelectedNode();
            if (node == null) return;

            SOPData data = FindSOPData(node);

            if (data == null)
            {
                return;
            }

            SectionViewData viewData = m_dicSections[data];
            SetStartTime(viewData);
            //m_frmMain.GetScenario().AddGridRowScenario(m_currentSOP.FullName);

            ArrayList arrCurrentSections = viewData.Sections;
            if (arrCurrentSections.Count > 0)
            {
                ArrayList arrSections = GetCurrentSections();
                if (arrSections != null && arrSections.Count > 0)
                    ShowSections(arrSections, false);

                m_currentSOP = data;
                ShowSections(arrCurrentSections, true);
                m_frmMain.GetProcess().Refresh();
                m_frmMain.GetScenario().AddGridRowScenario(data.FullName);
            }
            else
            {
                if (LoadSOP(data, frmDisaster.GetVersionID(), true))
                {
                    SetSMSMessages();
                    //tsbtnPlay.Enabled = false;
                    m_frmMain.GetScenario().AddGridRowScenario(data.FullName);
                }
                else
                    return;
            }

            SetProcesButtonState(TimeLine.PROCESS_STATUS.STARTED);
            viewData.TimeLineNormal.Start(m_currentSOP);
            m_frmMain.GetSOPLog().SetCurrentSOP(m_currentSOP);

            string strMsg = m_currentSOP.FullName + " Process가 진행중입니다.";

            //ezSMSComponent.ISMS sms = new ezSMSComponent.SMS();
            //sms.ServiceCode = "020026C9FCC7C39E41A88C2CF52D00D7BAA6";
            //ezSMSComponent.LoginInfo login = sms.Login("121.254.175.25", 4545, "unes", "unes01");

            /*ezSMSComponent.Receivers receiver = m_sms.CreateReceivers();
            SendSMS(m_currentSOP.FullName, m_sms, receiver);*/

            /*receiver.AddDirect("07077095975", strMsg, ezSMSComponent.EZSMS_SENDMODE.EZSMS_DIRECT, FlexTimer.Now);
            //receiver.AddDirect("01023156964", strMsg, ezSMSComponent.EZSMS_SENDMODE.EZSMS_DIRECT, DateTime.Now);
            //receiver.AddDirect("01020104562", strMsg, ezSMSComponent.EZSMS_SENDMODE.EZSMS_DIRECT, DateTime.Now);
            //receiver.AddDirect("01032325710", strMsg, ezSMSComponent.EZSMS_SENDMODE.EZSMS_DIRECT, DateTime.Now);

            ezSMSComponent.SendResults results = sms.SendSMS("027144133", receiver);

            foreach (ezSMSComponent.SendResult result in results)
            {
                if (result.Result != ezSMSComponent.EZSMS_RESULT.EZSMS_SUCCEEDED)
                {
                    MessageBox.Show("메시지 전송에 실패하였습니다.");
                }
            }*/
        }

        public void SendSMSMessage(SectionEx section)
        {
            if (!m_useSMS)
                return;

            ArrayList arrMessages = section.SMSMessages;
            if (arrMessages.Count == 0)
                return;

            MemberofSection data = section.MissionData;

            int nCellPhoneFirst, nCellPhoneMiddle, nCellPhoneLast;

            string strPhone1 = Utility.TrimString(data.CellPhone1);
            string strPhone2 = Utility.TrimString(data.CellPhone2);
            string strPhone3 = Utility.TrimString(data.CellPhone3);

            try
            {
                nCellPhoneFirst = int.Parse(strPhone1);
                nCellPhoneMiddle = int.Parse(strPhone2);
                nCellPhoneLast = int.Parse(strPhone3);
            }
            catch (Exception)
            {
                return;
            }

            string strPhoneNumber = strPhone1 + strPhone2 + strPhone3;

            ezSMSComponent.Receivers receiver = m_sms.CreateReceivers();

            foreach (string strMessage in arrMessages)
            {
                receiver.AddDirect(strPhoneNumber, strMessage, ezSMSComponent.EZSMS_SENDMODE.EZSMS_DIRECT, FlexTimer.Now);
            }

            ezSMSComponent.SendResults results = m_sms.SendSMS("027144133", receiver);
        }

        private void SetSMSMessages()
        {
            ArrayList arrSections = GetCurrentSections();
            if (arrSections == null)
                return;

            foreach (SectionEx section in arrSections)
            {
                SMSMessageFactory.MakeMessageList(section);
            }
        }

        // 중간시연을(2012/10/12) 위한 임시 코드
        /*private void SendSMS(string strDisaster, ezSMSComponent.ISMS sms, ezSMSComponent.Receivers receiver)
        {
            string strMsg = "", strMsg2 = "";

            if (strDisaster == "자연재해/태풍대응/예방")
            {
                strMsg = "바람";
                strMsg2 = "태풍활동중";
            }
            else if (strDisaster == "자연재해/태풍대응/대비")
            {
                strMsg = "바람";
                strMsg2 = "태풍활동중";
            }
            else if (strDisaster == "자연재해/태풍대응/대응")
            {
                strMsg = "바람";
                strMsg2 = "태풍활동중";
            }
            else if (strDisaster == "자연재해/태풍대응/복구")
            {
                strMsg = "바람";
                strMsg2 = "태풍활동중";
            }
            else if (strDisaster == "화재/야간터빈/대응")
            {
                strMsg = "화재";
                strMsg2 = "화재임무";
            }
            else if (strDisaster == "야간 및 휴일 비상상황/재난경보/화재")
            {
                strMsg = "화재";
                strMsg2 = "화재발생";
            }
            else if (strDisaster == "야간 및 휴일 비상상황/재난경보/바람")
            {
                strMsg = "바람";
                strMsg2 = "태풍활동중";
            }
            else if (strDisaster == "야간 및 휴일 비상상황/재난경보/폭발")
            {
                strMsg = "폭발";
            }
            else if (strDisaster == "야간 및 휴일 비상상황/재난경보/오염")
            {
                strMsg = "오염";
                strMsg2 = "오염중";
            }
            else if (strDisaster == "야간 및 휴일 비상상황/재난경보/침수")
            {
                strMsg = "침수";
            }
            else if (strDisaster == "야간 및 휴일 비상상황/재난경보/테러")
            {
                strMsg = "테러";
            }
            else if (strDisaster == "야간 및 휴일 비상상황/재난경보/지진")
            {
                strMsg = "지진";
            }
            else if (strDisaster == "야간 및 휴일 비상상황/재난경보/기타")
            {
                strMsg = "기타";
            }
            else
                return;

            if (strMsg == "")
                return;

            receiver.AddDirect("01093595295", strMsg, ezSMSComponent.EZSMS_SENDMODE.EZSMS_DIRECT, FlexTimer.Now);
            receiver.AddDirect("01024036676", strMsg, ezSMSComponent.EZSMS_SENDMODE.EZSMS_DIRECT, FlexTimer.Now);

            if (strMsg2 != "")
            {
                receiver.AddDirect("01093595295", strMsg2, ezSMSComponent.EZSMS_SENDMODE.EZSMS_DIRECT, FlexTimer.Now);
                receiver.AddDirect("01024036676", strMsg2, ezSMSComponent.EZSMS_SENDMODE.EZSMS_DIRECT, FlexTimer.Now);
            }

            ezSMSComponent.SendResults results = sms.SendSMS("027144133", receiver);
        }*/

        private void tsbtnPause_Click(object sender, EventArgs e)
        {
            if (m_currentSOP != null)
            {
                m_dicSections[m_currentSOP].TimeLineNormal.Pause();
                //tsbtnPause.Enabled = false;
                //tsbtnPlay.Enabled = true;
                SetProcesButtonState(TimeLine.PROCESS_STATUS.PAUSED);
            }
        }

        private void CompleteProcess(SectionViewData data)
        {
            foreach (KeyValuePair<SOPData, SectionViewData> pair in m_dicSections)
            {
                if (pair.Value == data)
                {
                    m_frmMain.GetScenario().DeleteGridRowScenario(pair.Key.FullName);
                    //tsbtnPlay.Enabled = true;
                    SetProcesButtonState(TimeLine.PROCESS_STATUS.COMPLETE);
                    return;
                }
            }
        }

        // Return 값 : Grid에서 특정 행 삭제후 다음에 자동으로 선택된 행에 대한 SOPData
        private SOPData ResetCurrentSOP(int nDeletedRowIndex, DataGridView dataGrid)
        {
            dataGrid.ClearSelection();

            if (nDeletedRowIndex >= 0)
            {
                int nRowCount = dataGrid.Rows.Count;

                if (nRowCount > nDeletedRowIndex)
                {
                    dataGrid.Rows[nDeletedRowIndex].Selected = true;
                    return FindSOPData(m_frmMain.GetDisaster().FindNode((string)dataGrid.Rows[nDeletedRowIndex].Cells[0].Value));
                }
                else if (nRowCount > 0)
                {
                    if (nRowCount > nDeletedRowIndex - 1)
                    {
                        dataGrid.Rows[nDeletedRowIndex - 1].Selected = true;
                        return FindSOPData(m_frmMain.GetDisaster().FindNode((string)dataGrid.Rows[nDeletedRowIndex - 1].Cells[0].Value));
                    }
                 }
            }

            return null;
        }

        private void tsbtnStop_Click(object sender, EventArgs e)
        {
           // timer1.Stop();
            FormLeftScenario frmScenario = m_frmMain.GetScenario();

            DataGridView dataGrid = frmScenario.GetGridView();
            //foreach (DataGridViewRow row in dataGrid.SelectedRows)
            //{
            //    string strPath = row.Cells[0].Value.ToString();
            //}
            

            FormLeftDisaster frmDisaster = m_frmMain.GetDisaster();
            if (frmDisaster == null) return;

            TreeNode node = frmDisaster.GetSelectedNode();
            if (node == null) return;

            SOPData data = FindSOPData(node);
            if (data != null)
            {
                ArrayList arrSections = m_dicSections[data].Sections;
                ShowSections(arrSections, false);

                string strFullPath;
                int nDepth = frmDisaster.GetNodeText(node, out strFullPath);

                int nDeletedRowIndex = frmScenario.DeleteGridRowScenario(strFullPath);

//                 DataGridViewSelectedCellCollection cells = m_frmMain.GetScenario().GetSelectedCells();
//                 if (cells == null) return;
                /*foreach (DataGridViewRow row in dataGrid.SelectedRows)
                {
                    node = frmDisaster.FindNode((string)row.Cells[0].Value);
                }

                m_currentSOP = FindSOPData(node);*/
                // 이전 SOP 중지
                if (m_currentSOP != null)
                {
                    SectionViewData viewData = m_dicSections[m_currentSOP];
                    ShowSections(viewData.Sections, false);
                    viewData.TimeLineNormal.Stop();
                }

                m_currentSOP = ResetCurrentSOP(nDeletedRowIndex, dataGrid);

                // 새로운 SOP로 전환
                if (m_currentSOP != null)
                {
                    SectionViewData viewData = m_dicSections[m_currentSOP];
                    ShowSections(viewData.Sections, true);
                }
                else
                {
                    m_frmMain.GetDisaster().ClearSelection();
                }

                m_frmMain.GetSOPLog().SetCurrentSOP(m_currentSOP);
                Refresh();
            }
        }

        private void tsbtnRestart_Click(object sender, EventArgs e)
        {
            if (m_currentSOP != null)
            {
                SetProcesButtonState(TimeLine.PROCESS_STATUS.STARTED);
                m_dicSections[m_currentSOP].TimeLineNormal.Restart();
                Refresh();
            }
        }

        public void SetCurrentNode(TreeNode node, bool refreshView)
        {
            SOPData prev = m_currentSOP;

            if (node == null)
                m_currentSOP = null;
            else
            {
                SOPData data = FindSOPData(node);

                if (data == null)
                    m_currentSOP = null;
                else
                    m_currentSOP = data;
            }

            if (prev == m_currentSOP)
                return;
            else
            {
                if (prev != null)
                    ShowSections(m_dicSections[prev].Sections, false);

                if (m_currentSOP != null)
                    ShowSections(m_dicSections[m_currentSOP].Sections, true);
            }

            if (m_currentSOP != null)
            {
                SetProcesButtonState(m_dicSections[m_currentSOP].TimeLineNormal.Status);
            }

            m_frmMain.GetSOPLog().SetCurrentSOP(m_currentSOP);

            if (refreshView)
                Refresh();
        }

        public TimeLine.PROCESS_STATUS GetSOPStatus(TreeNode node)
        {
            SOPData data = FindSOPData(node);
            if (data == null)
                return TimeLine.PROCESS_STATUS.NO_PROCESS;

            if (!m_dicSections.ContainsKey(data))
                return TimeLine.PROCESS_STATUS.NO_PROCESS;

            return m_dicSections[data].TimeLineNormal.Status;
        }

        public void RemoveSOP(TreeNode node)
        {
            SOPData data = FindSOPData(node);
            if (data == null) return;

            if (!m_dicSections.ContainsKey(data))
                return;

            if (m_currentSOP == data)
                m_currentSOP = null;

            SectionViewData sectionView = m_dicSections[data];
            ShowSections(sectionView.Sections, false);

            m_dicSections.Remove(data);
        }

        private void ShowSections(ArrayList arrSections, bool isShow)
        {
            if (isShow)
            {
                foreach (SectionEx section in arrSections)
                {
                    section.Show();
                }
            }
            else
            {
                foreach (SectionEx section in arrSections)
                {
                    section.Hide();
                }
            }
        }

        private void SetProcesButtonState(TimeLine.PROCESS_STATUS status = TimeLine.PROCESS_STATUS.NO_WORKED)
        {
            switch (status)
            {
                case TimeLine.PROCESS_STATUS.COMPLETE:
                case TimeLine.PROCESS_STATUS.NO_WORKED:
                    tsbtnPlay.Enabled = true;
                    tsbtnPause.Enabled = false;
                    tsbtnStop.Enabled = false;
                    tsbtnRestart.Enabled = false;
                    break;

                case TimeLine.PROCESS_STATUS.STARTED:
                    tsbtnPlay.Enabled = false;
                    tsbtnPause.Enabled = true;
                    tsbtnStop.Enabled = true;
                    tsbtnRestart.Enabled = true;
                    break;

                case TimeLine.PROCESS_STATUS.PAUSED:
                    tsbtnPlay.Enabled = true;
                    tsbtnPause.Enabled = false;
                    tsbtnStop.Enabled = true;
                    tsbtnRestart.Enabled = true;
                    break;

                case TimeLine.PROCESS_STATUS.NO_PROCESS:
                    tsbtnPlay.Enabled = false;
                    tsbtnPause.Enabled = false;
                    tsbtnStop.Enabled = false;
                    tsbtnRestart.Enabled = false;
                    break;
            }
        }

        // Tree에서 선택된 SOP 항목
        public void OnSelectedSOP(int nDepth, string strSOPFullName, TreeNode node)
        {
            if (nDepth >= 3)
            {
                int nActionStepCount = 0;

                try
                {
                    if (node.Tag != null)
                    {
                        nActionStepCount = (int)node.Tag;
                    }
                }
                catch (Exception)
                {
                }

                SOPData data = FindSOPData(node);

                if (data == null)
                {
                    data = new SOPData(nDepth, strSOPFullName, node);
                    //m_dicSections[data] = new ArrayList();
                    m_dicSections[data] = new SectionViewData(this);
                    SetProcesButtonState(nActionStepCount == 0 ? TimeLine.PROCESS_STATUS.NO_PROCESS : TimeLine.PROCESS_STATUS.NO_WORKED);
                }
                else
                {
                    if (nActionStepCount == 0)
                        SetProcesButtonState(TimeLine.PROCESS_STATUS.NO_PROCESS);      // 실행할 Process가 없음
                    else
                    {
                        if (m_frmMain.GetScenario().IndexOf(strSOPFullName) >= 0)
                            SetProcesButtonState(TimeLine.PROCESS_STATUS.STARTED);      // 이미 실행중인 Process
                        else
                            SetProcesButtonState(TimeLine.PROCESS_STATUS.NO_WORKED);    // 아직 실행되지 않은 Process
                    }
                }

                data.Depth = nDepth;
                data.FullName = strSOPFullName;
            }
        }

        // Tree에서 선택된 SOP 항목
        //public void OnSelectedSOP(int nDepth, string strSOPFullName, TreeNode node)
        //{
        //    if (nDepth >= 3)
        //    {
        //        tsbtnPlay.Enabled = true;
        //        if (m_currentSOP != null)
        //        {
        //            if (m_currentSOP.Node == node)
        //            {
        //                //m_nSelectedDepth = nDepth;
        //                return;
        //            }

        //            //ShowSections(m_dicSections[m_currentSOP], false);
        //        }

        //        m_currentSOP = FindSOPData(node);

        //        if (m_currentSOP == null)
        //        {
        //            m_currentSOP = new SOPData(nDepth, strSOPFullName, node);
        //            m_dicSections[m_currentSOP] = new ArrayList();
        //        }
        //        else
        //        {
        //            //ShowSections(m_dicSections[m_currentSOP], true);
        //        }

        //        m_currentSOP.Depth = nDepth;
        //        m_currentSOP.FullName = strSOPFullName;

        //        m_sectionSelected = null;

        //        Refresh();
        //    }

        //    //m_nSelectedDepth = nDepth;
        //}

        public string GetTime()
        {
            string strTime = FlexTimer.Now.ToString("HH:mm:ss");
            return strTime;
        }

        private void AddMissionInfo(SectionEx section, ref int nTotalMissionCount, ref int nProcessedMissionCount, ref int nProcessingMissionCount)
        {
            if (section == null)
                return;

            if (section.Type == SectionEx.SectionType.GROUP_SECTION)
            {
                int nMissionCount = section.MissionData.Missions.Count;

                if (section.Status == SectionEx.PROCESS_STATUS.COMPLETE)
                    nProcessedMissionCount += nMissionCount;
                else if (section.Status == SectionEx.PROCESS_STATUS.STARTED)
                    nProcessingMissionCount += nMissionCount;

                nTotalMissionCount += nMissionCount;
            }
            else
            {
                ArrayList arrChilds = section.GetChildSections();
                if (arrChilds == null) return;

                foreach (SectionEx child in arrChilds)
                {
                    AddMissionInfo(child, ref nTotalMissionCount, ref nProcessedMissionCount, ref nProcessingMissionCount);
                }
            }
        }

        private void SendMissionInfo()
        {
            int nTotalMissionCount = 0;
            int nProcessedMissionCount = 0;
            int nProcessingMissionCount = 0;

            if (m_currentSOP == null)
                return;

            if (!m_dicSections.ContainsKey(m_currentSOP))
                return;

            SectionViewData data = m_dicSections[m_currentSOP];
            ArrayList arrSections = data.Sections;

            foreach (SectionEx section in arrSections)
            {
                AddMissionInfo(section, ref nTotalMissionCount, ref nProcessedMissionCount, ref nProcessingMissionCount);
            }

            m_frmMain.GetProgress().SetMissionInfo(nTotalMissionCount, nProcessedMissionCount, nProcessingMissionCount);
        }

        //private void AddMemberInfo(SectionEx section, Dictionary<int, int> dicTeamID, ArrayList arrMemberID)
        private void AddMemberInfo(SectionEx section, Dictionary<int, MemberInfo> dicAllMembers, Dictionary<int, MemberInfo> dicProcessingMembers)
        {
            if (section.Type == SectionEx.SectionType.GROUP_SECTION)
            {
                StepMemberData data = section.StepMember;

                if (data.MemberType == 3)       // 팀원
                {
                    if (!m_dicMemberInfo.ContainsKey(data.MemberID))
                        return;

                    MemberInfo member = m_dicMemberInfo[data.MemberID];
                    dicAllMembers[data.MemberID] = member;

                    if (section.Status == SectionEx.PROCESS_STATUS.STARTED)
                        dicProcessingMembers[data.MemberID] = member;
                }
                else if (data.MemberType == 1)  // 상시 조직
                {
                    if (!m_dicTeamInfo.ContainsKey(data.MemberID))
                        return;

                    bool isProcessing = section.Status == SectionEx.PROCESS_STATUS.STARTED;
                    ArrayList arrTeamInfo = m_dicTeamInfo[data.MemberID];

                    foreach (MemberInfo member in arrTeamInfo)
                    {
                        if (isProcessing)
                            dicProcessingMembers[member.ID] = member;

                        dicAllMembers[member.ID] = member;
                    }
                }
            }
            else
            {
                ArrayList arrChilds = section.GetChildSections();

                foreach (SectionEx child in arrChilds)
                {
                    AddMemberInfo(child, dicAllMembers, dicProcessingMembers);
                }
            }
        }

        private void SendMemberInfo()
        {
            if (m_currentSOP == null)
            {
                m_frmMain.GetPersonnel().SetMemberInfo(m_dicMemberInfo, null, null);
                return;
            }

            if (!m_dicSections.ContainsKey(m_currentSOP))
            {
                m_frmMain.GetPersonnel().SetMemberInfo(m_dicMemberInfo, null, null);
                return;
            }

            SectionViewData data = m_dicSections[m_currentSOP];
            ArrayList arrSections = data.Sections;
            
            Dictionary<int, MemberInfo> dicResource = new Dictionary<int, MemberInfo>();
            Dictionary<int, MemberInfo> dicProcessing = new Dictionary<int, MemberInfo>();
            Dictionary<int, MemberInfo> dicNoProcessing = new Dictionary<int, MemberInfo>();
            
            foreach (SectionEx section in arrSections)
            {
                AddMemberInfo(section, dicResource, dicProcessing);
            }

            foreach (KeyValuePair<int, MemberInfo> val in dicResource)
            {
                if (!dicProcessing.ContainsKey(val.Key))
                    dicNoProcessing[val.Key] = val.Value;
            }

            m_frmMain.GetPersonnel().SetMemberInfo(m_dicMemberInfo, dicProcessing, dicNoProcessing);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string strCurrentTime = GetTime();
            m_nTickCount++;

//             TimeSpan ts = new TimeSpan(0, 0, m_nTickCount);
//             toolStripLabel2.Text = ts.ToString();
            // SOP 진행 현황/SOP 진행 시간 정보
            m_frmMain.GetProgress().SetCurrentTime(strCurrentTime);

            // SOP 진행현황/SOP 임무 수행 정보
            SendMissionInfo();

            // SOP 요원 현황
            SendMemberInfo();

            if (m_currentSOP != null)
            {
                TimeSpan tsTemp = m_dicSections[m_currentSOP].ElapsedTimeData.TimePasses;
                TimeSpan ts = new TimeSpan(tsTemp.Hours, tsTemp.Minutes, tsTemp.Seconds);
                m_frmMain.GetProgress().GetElapsedTime(ts);
            }

            if (m_requestRefresh)
            {
                m_requestRefresh = false;
                Refresh();
            }

            if (m_requestComplete != null)
            {
                //CompleteProcess(m_requestComplete);
                m_requestComplete = null;
            }

            TimeLine currentTimeLine = m_currentSOP == null ? null : m_dicSections[m_currentSOP].TimeLineNormal;

            // 현재 진행중인 Process 수행
            ArrayList arrRemove = new ArrayList();
            int nPlayingCount = m_arrPlayingProcess.Count;

            for (int i=0;i<nPlayingCount;i++)
            {
                PlayingData play = (PlayingData)m_arrPlayingProcess[i];
                TimeLine tl = play.Time;
                SOPData data = play.Data;

                ArrayList arrChangedTask = m_frmMain.GetSOPLog().GetTaskArray(data);
                int nIndex = arrChangedTask == null ? 0 : arrChangedTask.Count;

                if (tl.Process(arrChangedTask))
                    arrRemove.Add(i);

                if (arrChangedTask != null && arrChangedTask.Count > nIndex)
                    m_frmMain.GetSOPLog().AddTask(arrChangedTask, nIndex);
            }

            // Stop 시킨 Process 제거
            int nRemoveCount = arrRemove.Count;

            for (int i = nRemoveCount - 1; i >= 0; i--)
            {
                m_arrPlayingProcess.RemoveAt((int)arrRemove[i]);
            }
        }

        public bool RequestRefresh
        {
            set { m_requestRefresh = value; }
        }

        private void FormProcess_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //m_clickedLButton = true;
                //m_ptClicked.X = e.X;
                //m_ptClicked.Y = e.Y;

                if (m_sectionSelected != null && m_sectionSelected.GetChangeSizeOption() != EditBox.BoxPosition.NO_SELECT)
                {
                    m_sectionSelected.SetChangeSizeOriginPoint(e.X, e.Y);
                }
                else
                {
                    SelectSection(e.X, e.Y);
                    //if (SelectSection(e.X, e.Y))
                    //    m_ptSelected = m_sectionSelected.Position;
                    //else
                    //    m_Main.HideExpandedPane();
                }
            }
        }

        private bool SelectSection(int x, int y)
        {
            ArrayList arrSections = GetCurrentSections();
            if (arrSections == null) return false;

            foreach (SectionEx section in arrSections)
            {
                SectionEx secsionSelected = (SectionEx)section.Select(x, y);

                if (secsionSelected != null)
                {
                    if (m_sectionSelected != null)
                    {
                        if (m_sectionSelected != secsionSelected)
                        {
                            m_sectionSelected.Select(false, null);
                            Refresh();
                        }
                        else
                        {
                            // 선택된 상태에서 다시 선택되었음을 알린다.
                            // 텍스트 편집이나 기타 기능을 수행할 수 있다.
                            m_sectionSelected.DoubleSelect(true);
                        }
                    }

                    secsionSelected.Select(true, null);
                    m_sectionSelected = secsionSelected;
                    m_frmMain.GetMission().SetMissionData();

                    if (m_sectionSelected.Type == SectionEx.SectionType.PROCESS_SECTION) //프로세스 선택시
                    {
                        m_frmMain.GetMission().VisiblePanel(1);
                        m_frmMain.GetMission().SetCurrentProcess(m_sectionSelected);
                    }
                    else if (m_sectionSelected.Type == SectionEx.SectionType.GROUP_SECTION) // 그룹 선택시
                    {
                        m_frmMain.GetMission().VisiblePanel(2);
                    }

                    Refresh();
                    return true;
                }
            }

            if (m_sectionSelected != null)
            {
                m_sectionSelected.Select(false, null);//m_arrSection);
                Refresh();
                m_sectionSelected = null;
            }

            return false;
        }

        public void StopTimer()
        {
            timer1.Stop();
        }

        public void StopThread()
        {
            if (m_currentSOP != null)
                m_dicSections[m_currentSOP].TimeLineNormal.Stop();
        }

        private void FormProcess_Scroll(object sender, ScrollEventArgs e)
        {
            this.Refresh();
        }

        private void FormProcess_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        //public void GetMissionInfo(TreeNode node, 

        public SectionViewData RequestComplete
        {
            get { return m_requestComplete; }
            set { m_requestComplete = value; }
        }

        public MemberofSection GetMissionData()
        {
            return m_sectionSelected.MissionData;
        }

        public ArrayList PlayingList
        {
            get { return m_arrPlayingProcess; }
        }
    }

    public class ElapsedTime
    {
        //protected TimeSpan m_ts;
        protected DateTime m_dateTime;
        private TimeMode m_mode = TimeMode.NORMAL;

        private TimeSpan m_lastTS = new TimeSpan();

        public enum TimeMode { NORMAL, STOPED, PAUSED, RESUMED };

        public TimeSpan TimePasses
        {
            get
            {
                if (m_mode == TimeMode.NORMAL)
                {
                    DateTime now = FlexTimer.Now;//DateTime.Now;
                    TimeSpan ts = now - m_dateTime;
                    m_lastTS = ts;
                    return ts;
                }
                else if (m_mode == TimeMode.RESUMED)
                {
                    m_mode = TimeMode.NORMAL;
                    DateTime now2 = FlexTimer.Now;//DateTime.Now;
                    m_dateTime = now2 - m_lastTS;
                    TimeSpan t1 = now2 - m_dateTime;
                    return t1;
                }

                return m_lastTS;
            }
            //get { return m_ts; }
            //set { m_ts = value; }
        }

        public DateTime  StartTime
        {
            get { return m_dateTime; }
            set
            {
                if (m_mode != TimeMode.PAUSED)
                    m_dateTime = value;
            }
        }

        public TimeMode Mode
        {
            get { return m_mode; }
            set { m_mode = value; }
        }
    }

    public class SOPData
    {
        protected int m_nDepth;
        protected string m_strFullName;
        protected string m_strName;
        protected TreeNode m_node;

        public SOPData(TreeNode node)
        {
            m_nDepth = -1;
            m_strName = m_strFullName = "";
            m_node = node;
        }

        public SOPData(int nDepth, string strFullName, TreeNode node)
        {
            Depth = nDepth;
            FullName = strFullName;
            m_node = node;
        }

        public int Depth
        {
            get { return m_nDepth; }
            set { m_nDepth = value; }
        }

        public string FullName
        {
            get { return m_strFullName; }
            set
            {
                int nIndex = value.LastIndexOf('/');

                if (nIndex >= 0)
                    m_strName = value.Substring(nIndex + 1);
                else
                    m_strName = value;

                m_strFullName = value;
            }
        }

        public string Name
        {
            get { return m_strName; }
        }

        public TreeNode Node
        {
            get { return m_node; }
        }
    }

    public class TimeLine
    {
        class TimeInfo
        {
            private string m_strTime = "00:00";
            private int m_nHour = 0;
            private int m_nMinute = 0;
            private int m_nPosition = 0;

            public int Hour
            {
                get { return m_nHour; }
                set
                {
                    m_nHour = value;
                    m_strTime = string.Format("{0:00}:{1:00}", m_nHour, m_nMinute);
                }
            }

            public int Minute
            {
                get { return m_nMinute; }
                set
                {
                    m_nMinute = value;
                    m_strTime = string.Format("{0:00}:{1:00}", m_nHour, m_nMinute);
                }
            }

            public string Time
            {
                get { return m_strTime; }
            }

            public int Position
            {
                get { return m_nPosition; }
                set { m_nPosition = value; }
            }
        }

        private ArrayList m_arrTimes = new ArrayList();
        private Color m_colVerticalLine = Color.FromArgb(183, 222, 232);
        private Color m_colHorizontalLine = Color.FromArgb(75, 172, 198);
        private int m_nVerticalLineThick = 10;
        private int m_nHorizontalLineThick = 3;
        private int m_nHorizontalLineWidth = 40;
        private Point m_ptLineBegin = new Point(70, 0);
        private int m_nTimeLineEndPos = 0;

        private SectionViewData m_parent = null;
        //private bool m_isThreading = false;
        private int m_nPrevHour = -1, m_nPrevMinute = -1;
        //private System.Threading.Thread m_thread = null;

        private SolidBrush m_brushVertical = null;
        private SolidBrush m_brushHorizontal = null;
        private PROCESS_STATUS m_status = PROCESS_STATUS.NO_WORKED;

        // 자식이 없는 Process Section이 시간이 경과할 경우
        // true이면 무조건 Complete
        // false이면 시간에 따라 상태 결정
        private static bool NO_CHILD_OPT = true;
        // 완료시간이 아직 지나지 않았으나 자식 노드가 모두 완료될 경우
        // true이면 부모도 완료시킴
        // false이면 시간에 따라 상태 결정
        private static bool ALL_CHILD_COMPLETE_OPT = true;

        private static Font TEXT_FONT = new Font("맑은고딕", 9);

        public enum PROCESS_STATUS { STARTED, COMPLETE, PAUSED, NO_WORKED, NO_PROCESS };

        public TimeLine()
        {
            m_brushVertical = new SolidBrush(m_colVerticalLine);
            m_brushHorizontal = new SolidBrush(m_colHorizontalLine);
        }

        public void AddTime(int nHour, int nMin, int nPosition)
        {
            TimeInfo time = new TimeInfo();

            time.Hour = nHour;
            time.Minute = nMin;
            time.Position = nPosition;

            m_arrTimes.Add(time);
        }

        public void ChangePosition(int nIndex, int nPosition)
        {
            int nCount = m_arrTimes.Count;
            if (nCount <= nIndex) return;

            TimeInfo time = (TimeInfo)m_arrTimes[nIndex];
            time.Position = nPosition;
        }

        public void Draw(Graphics g)
        {
            Point ptScroll = Parent.GetProcess().AutoScrollPosition;

            // 장축 Time Line
            g.FillRectangle(m_brushVertical, m_ptLineBegin.X - m_nVerticalLineThick / 2 + ptScroll.X, m_ptLineBegin.Y, m_nVerticalLineThick, m_nTimeLineEndPos - m_ptLineBegin.Y);

            int xBeginLine = m_ptLineBegin.X - m_nHorizontalLineWidth / 2;
            int xBeginText = xBeginLine - 40;

            foreach (TimeInfo time in m_arrTimes)
            {
                g.FillRectangle(m_brushHorizontal, xBeginLine + ptScroll.X, time.Position - m_nHorizontalLineThick / 2 - FormProcess.RECT_UP, m_nHorizontalLineWidth, m_nHorizontalLineThick);
                g.DrawString(time.Time, TEXT_FONT, Brushes.Black, xBeginText + ptScroll.X, time.Position - TEXT_FONT.Height / 2 - FormProcess.RECT_UP);
            }
        }

        public void Start(SOPData data)
        {
            ArrayList arrList = m_parent.GetProcess().PlayingList;
            if (!arrList.Contains(this))
                arrList.Add(new PlayingData(data, this));

            if (m_status == PROCESS_STATUS.PAUSED)
                m_parent.ElapsedTimeData.Mode = ElapsedTime.TimeMode.RESUMED;
            else
                m_parent.ElapsedTimeData.Mode = ElapsedTime.TimeMode.NORMAL;

            m_status = PROCESS_STATUS.STARTED;

            /*if (m_isThreading)
                return;

            m_thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ProcessThread));

            m_parent.ElapsedTimeData.Mode = ElapsedTime.TimeMode.NORMAL;
            m_status = PROCESS_STATUS.STARTED;
            m_thread.Start(this);*/
        }

        public void Stop()
        {
            m_parent.ElapsedTimeData.Mode = ElapsedTime.TimeMode.STOPED;
            m_status = PROCESS_STATUS.NO_WORKED;
            m_parent.ElapsedTimeData.StartTime = FlexTimer.Now;//DateTime.Now;

            m_nPrevHour = m_nPrevMinute = -1;
            SetSectionStatus(m_parent.Sections, SectionEx.PROCESS_STATUS.WAITING);

            /*m_parent.ElapsedTimeData.Mode = ElapsedTime.TimeMode.STOPED;
            m_isThreading = false;
            m_thread = null;*/
        }

        public void Pause()
        {
            m_parent.ElapsedTimeData.Mode = ElapsedTime.TimeMode.PAUSED;
            m_status = PROCESS_STATUS.PAUSED;
        }

        public void Restart()
        {
            m_status = PROCESS_STATUS.STARTED;
            m_parent.ElapsedTimeData.Mode = ElapsedTime.TimeMode.NORMAL;
            m_parent.ElapsedTimeData.StartTime = FlexTimer.Now;//DateTime.Now;

            m_nPrevHour = m_nPrevMinute = -1;
            SetSectionStatus(m_parent.Sections, SectionEx.PROCESS_STATUS.WAITING);
        }

        private void SetSectionStatus(ArrayList arrSections, SectionEx.PROCESS_STATUS status)
        {
            if (arrSections == null) return;

            foreach (SectionEx section in arrSections)
            {
                SetSectionStatus(section.GetChildSections(), status);
                section.Status = status;
            }
        }

        /*private static void ProcessThread(object arg)
        {
            TimeLine timeLine = (TimeLine)arg;
            if (timeLine == null) return;

            timeLine.m_isThreading = true;

            while (timeLine.m_isThreading)
            {
                System.Threading.Thread.Sleep(500);

                if (timeLine.m_status == PROCESS_STATUS.PAUSED)
                    continue;
                else if (timeLine.m_status == PROCESS_STATUS.COMPLETE)
                {
                    timeLine.ChangeAllSections(SectionEx.PROCESS_STATUS.COMPLETE);
                    break;
                }
                else if (timeLine.m_status == PROCESS_STATUS.NO_WORKED)
                {
                    timeLine.ChangeAllSections(SectionEx.PROCESS_STATUS.WAITING);
                    break;
                }
                else// if (timeLine.m_status == PROCESS_STATUS.STARTED)
                {
                    TimeSpan t = timeLine.m_parent.ElapsedTimeData.TimePasses;
                    timeLine.Process(t.Hours, t.Minutes);
                }
            }

            timeLine.m_isThreading = false;
        }*/

        // Return값 : 프로세스가 종료되었는가?(NO_WORKED 상태인가?)
        public bool Process(ArrayList arrChangedTask)
        {
            switch (m_status)
            {
                case PROCESS_STATUS.PAUSED:
                    return false;

                case PROCESS_STATUS.COMPLETE:
                    return false;

                case PROCESS_STATUS.NO_WORKED:
                    return true;

                case PROCESS_STATUS.STARTED:
                    TimeSpan t = m_parent.ElapsedTimeData.TimePasses;
                    Process(t.Hours, t.Minutes, arrChangedTask);
                    break;

                default:
                    return true;
            }

            return m_status == PROCESS_STATUS.NO_WORKED;
        }

        private void CalcCurrentProcessSection(HMTime time)
        {
            ArrayList arrSections = Parent.ProcessSections;

            foreach (SectionEx section in arrSections)
            {
                int nBeginHour, nBeginMin;
                int nProcessHour, nProcessMin;

                section.GetTime(out nBeginHour, out nBeginMin, true);
                section.GetTime(out nProcessHour, out nProcessMin, false);

                int nMin = nBeginMin + nProcessMin;
                int nHour = nBeginHour + nProcessHour;

                if (nMin >= 60)
                {
                    nHour++;
                    nMin -= 60;
                }

                HMTime beginTime = new HMTime(nBeginHour, nBeginMin);
                HMTime endTime = new HMTime(nHour, nMin);

                if (time >= beginTime && time < endTime)
                    section.SetCurrentProcess(true);
                else
                    section.SetCurrentProcess(false);
            }
        }

        private void Process(int nHour, int nMin, ArrayList arrChangedTask)
        {
            if (m_nPrevHour == nHour && m_nPrevMinute == nMin)
                return;

            ArrayList arrSections = m_parent.Sections;
            HMTime time = new HMTime(nHour, nMin);
            bool allCompleted = true;

            if (ProcessSections(null, time, arrChangedTask, ref allCompleted))
            {
                CalcCurrentProcessSection(time);
                //m_parent.GetProcess().Refresh();
                m_parent.GetProcess().RequestRefresh = true;
            }

            if (allCompleted)
            {
                m_status = PROCESS_STATUS.COMPLETE;
                //m_isThreading = false;
                //m_thread = null;

                //Parent.GetProcess().CompleteProcess(m_parent);
                m_parent.GetProcess().RequestComplete = m_parent;
                m_parent.ElapsedTimeData.Mode = ElapsedTime.TimeMode.PAUSED;
            }

            m_nPrevHour = nHour;
            m_nPrevMinute = nMin;
        }

        // Return 값 : 변동사항이 있는가?
        private bool CalcProcess(SectionEx section, HMTime currentTime, ArrayList arrChildStatus, ArrayList arrChangedTask, ref bool allCompleted)
        {
            int nBeginHour, nBeginMin;
            int nProcessHour, nProcessMin;

            section.GetTime(out nBeginHour, out nBeginMin, true);
            section.GetTime(out nProcessHour, out nProcessMin, false);

            int nMin = nBeginMin + nProcessMin;
            int nHour = nBeginHour + nProcessHour;

            if (nMin >= 60)
            {
                nHour++;
                nMin -= 60;
            }

            HMTime beginTime = new HMTime(nBeginHour, nBeginMin);
            HMTime endTime = new HMTime(nHour, nMin);

            SectionEx.PROCESS_STATUS prevStatus = section.Status;

            if (arrChildStatus == null || arrChildStatus.Count == 0)
            {
                if (section.Type == SectionEx.SectionType.PROCESS_SECTION)
                {
                    if (NO_CHILD_OPT)
                    {
                        if (beginTime <= currentTime)
                        {
                            section.Status = SectionEx.PROCESS_STATUS.COMPLETE;
                            if (prevStatus != section.Status)
                                SaveTask(section, true, arrChangedTask);
                            return true;
                        }
                        else
                            section.Status = SectionEx.PROCESS_STATUS.WAITING;
                    }
                    else
                    {
                        if (currentTime < beginTime)
                            section.Status = SectionEx.PROCESS_STATUS.WAITING;
                        else if (currentTime < endTime)
                            section.Status = SectionEx.PROCESS_STATUS.STARTED;
                        else
                            section.Status = SectionEx.PROCESS_STATUS.COMPLETE;
                    }
                }
                else if (section.Type == SectionEx.SectionType.GROUP_SECTION)
                {
                    if (currentTime < beginTime)
                        section.Status = SectionEx.PROCESS_STATUS.WAITING;
                    else if (currentTime < endTime)
                        section.Status = SectionEx.PROCESS_STATUS.STARTED;
                    else
                        section.Status = SectionEx.PROCESS_STATUS.COMPLETE;
                }

                allCompleted = section.Status == SectionEx.PROCESS_STATUS.COMPLETE;

                if (prevStatus != section.Status)
                    SaveTask(section, beginTime == endTime, arrChangedTask);

                return prevStatus != section.Status;
            }

            int nStatusCount = 0;

            foreach (SectionEx.PROCESS_STATUS status in arrChildStatus)
            {
                if (status == SectionEx.PROCESS_STATUS.STARTED)
                {
                    allCompleted = false;
                    nStatusCount++;
                }
                else if (status == SectionEx.PROCESS_STATUS.COMPLETE)
                    nStatusCount += 2;
                else
                    allCompleted = false;
            }

            if (nStatusCount == arrChildStatus.Count * 2)
            {
                if (section.Type == SectionEx.SectionType.PROCESS_SECTION)
                {
                    if (ALL_CHILD_COMPLETE_OPT)
                        section.Status = SectionEx.PROCESS_STATUS.COMPLETE;
                    else
                    {
                        if (endTime <= currentTime)
                            section.Status = SectionEx.PROCESS_STATUS.COMPLETE;
                        else
                            section.Status = SectionEx.PROCESS_STATUS.STARTED;
                    }
                }
                else
                    section.Status = SectionEx.PROCESS_STATUS.COMPLETE;
            }
            else if (nStatusCount > 0)
                section.Status = SectionEx.PROCESS_STATUS.STARTED;
            else
                section.Status = SectionEx.PROCESS_STATUS.WAITING;

            if (prevStatus != section.Status)
                SaveTask(section, beginTime == endTime, arrChangedTask);

            return prevStatus != section.Status;
        }

        private void SaveTask(SectionEx section, bool isDual, string strProcessTime, string strProcessName, string strMemberName, string strTaskName, ArrayList arrChangedTask)
        {
            Task task = new Task();

            task.ProcessTime = strProcessTime;
            task.ProcessName = strProcessName;
            task.MemberName = strMemberName;
            task.TaskName = strTaskName;
            task.Section = section;

            if (isDual)
            {
                task.Status = "시작";
                arrChangedTask.Add(task);

                task = new Task();

                task.ProcessTime = strProcessTime;
                task.ProcessName = section.GetTextBox().Text;
                task.MemberName = strMemberName;
                task.TaskName = strTaskName;
                task.Status = "종료";
                task.Section = section;

                arrChangedTask.Add(task);
            }
            else
            {
                if (section.Status == SectionEx.PROCESS_STATUS.COMPLETE)
                    task.Status = "종료";
                else if (section.Status == SectionEx.PROCESS_STATUS.STARTED)
                    task.Status = "시작";

                arrChangedTask.Add(task);
            }
        }

        private void SaveTask(SectionEx section, bool isDual, ArrayList arrChangedTask)
        {
            if (arrChangedTask == null) return;

            if (section.Type == SectionEx.SectionType.PROCESS_SECTION)
            {
                TimeSpan tsTemp = Parent.ElapsedTimeData.TimePasses;
                TimeSpan ts = new TimeSpan(tsTemp.Hours, tsTemp.Minutes, tsTemp.Seconds);
                SaveTask(section, isDual, ts.ToString(), section.GetTextBox().Text, "", "", arrChangedTask);
            }
            else if (section.Type == SectionEx.SectionType.GROUP_SECTION)
            {
                TimeSpan tsTemp = Parent.ElapsedTimeData.TimePasses;
                TimeSpan ts = new TimeSpan(tsTemp.Hours, tsTemp.Minutes, tsTemp.Seconds);
                string strProcessTime = ts.ToString();

                string strProcessName = section.GetParentSection().GetTextBox().Text;
                string strMemberName = section.GetTextBox().Text;

                int nMissionCount = section.MissionData.Missions.Count;

                for (int i = 0; i < nMissionCount; i++)
                {
                    MemberofSection.MissionofSection mission = (MemberofSection.MissionofSection)section.MissionData.Missions[i];
                    SaveTask(section, isDual, strProcessTime, strProcessName, strMemberName, mission.TaskName, arrChangedTask);
                }

                if (nMissionCount == 0)
                {
                    SaveTask(section, isDual, strProcessTime, strProcessName, strMemberName, "", arrChangedTask);
                }
            }
        }

        // Return 값 : 변동사항이 있는가?
        private bool ProcessSections(SectionEx sectionParent, HMTime currentTime, ArrayList arrChangedTask, ref bool allCompleted, ArrayList arrStatus = null)
        {
            ArrayList arrSections = sectionParent == null ? m_parent.Sections : sectionParent.GetChildSections();
            ArrayList arrChildStatus = new ArrayList();
            bool isChanged = false;

            foreach (SectionEx section in arrSections)
            {
                bool childAllCompleted = true;

                if (ProcessSections(section, currentTime, arrChangedTask, ref childAllCompleted, arrChildStatus))
                    isChanged = true;

                if (!childAllCompleted)
                    allCompleted = false;
            }

            if (sectionParent != null)
            {
                if (CalcProcess(sectionParent, currentTime, arrChildStatus, arrChangedTask, ref allCompleted))
                    isChanged = true;
                
                if (arrStatus != null)
                    arrStatus.Add(sectionParent.Status);

                return isChanged;
            }

            return isChanged;
        }

        private void ChangeAllSections(SectionEx.PROCESS_STATUS status)
        {
            ArrayList arrSections = m_parent.Sections;
            _ChangeAllSections(arrSections, status);
            //m_parent.GetProcess().Refresh();
            m_parent.GetProcess().RequestRefresh = true;
        }

        private void _ChangeAllSections(ArrayList arrSections, SectionEx.PROCESS_STATUS status)
        {
            if (arrSections == null) return;

            foreach (SectionEx section in arrSections)
            {
                section.Status = status;
                _ChangeAllSections(section.GetChildSections(), status);
            }
        }

        public int TimeLineEndPos
        {
            get { return m_nTimeLineEndPos; }
            set { m_nTimeLineEndPos = value; }
        }

        public Point LineBegin
        {
            get { return m_ptLineBegin; }
            set { m_ptLineBegin = value; }
        }

        public SectionViewData Parent
        {
            get { return m_parent; }
            set { m_parent = value; }
        }

        public PROCESS_STATUS Status
        {
            get { return m_status; }
        }
    }

    public class HMTime
    {
        private int m_nHour = -1, m_nMin = -1;

        public HMTime(int nHour, int nMin)
        {
            m_nHour = nHour;
            m_nMin = nMin;
        }

        public static bool operator <(HMTime t1, HMTime t2)
        {
            if (t1.Hour < t2.Hour)
                return true;
            else if (t1.Hour == t2.Hour)
            {
                return t1.Minute < t2.Minute;
            }

            return false;
        }

        public static bool operator >(HMTime t1, HMTime t2)
        {
            return t2 < t1;
        }

        public static bool operator <=(HMTime t1, HMTime t2)
        {
            return (t1 == t2) || (t1 < t2);
        }

        public static bool operator >=(HMTime t1, HMTime t2)
        {
            return (t2 == t1) || (t2 < t1);
        }

        public static bool operator ==(HMTime t1, HMTime t2)
        {
            return t1.Hour == t2.Hour && t1.Minute == t2.Minute;
        }

        public static bool operator !=(HMTime t1, HMTime t2)
        {
            return !(t1 == t2);
        }

        public int Hour
        {
            get { return m_nHour; }
            set { m_nHour = value; }
        }

        public int Minute
        {
            get { return m_nMin; }
            set { m_nMin = value; }
        }
    }

    public class SectionViewData
    {
        private ArrayList m_arrSections = new ArrayList();
        // 평상시 TimeLine
        private TimeLine m_timeLineNormal = new TimeLine();
        private ElapsedTime m_elapsedTime = new ElapsedTime();
        private FormProcess m_frmProcess = null;
        private ArrayList m_arrProcessSections = new ArrayList();
        
        public SectionViewData(FormProcess frmProcess)
        {
            m_timeLineNormal.Parent = this;
            m_frmProcess = frmProcess;
        }

        public FormProcess GetProcess()
        {
            return m_frmProcess;
        }

        public ArrayList Sections
        {
            get { return m_arrSections; }
        }

        public TimeLine TimeLineNormal
        {
            get { return m_timeLineNormal; }
        }

        public ElapsedTime ElapsedTimeData
        {
            get { return m_elapsedTime; }
            set { m_elapsedTime = value; }
        }

        public ArrayList ProcessSections
        {
            get { return m_arrProcessSections; }
        }
    }

    public class PlayingData
    {
        private SOPData m_data = null;
        private TimeLine m_timeLine = null;

        public PlayingData(SOPData data, TimeLine tl)
        {
            m_data = data;
            m_timeLine = tl;
        }

        public SOPData Data
        {
            get { return m_data; }
        }

        public TimeLine Time
        {
            get { return m_timeLine; }
        }
    }

    public class MemberInfo
    {
        private int m_nID = -1;
        private string m_strName = "";
        // '-' 이나 공백 없이 모두 붙여서 표기
        // ex) "01012345678"
        private string m_strPhoneNumber = "";
        private int m_nRegularTeamID = -1;

        public MemberInfo()
        {
        }

        public MemberInfo(int nID, string strName, string strPhoneNumber, int nRegularTeamID)
        {
            m_nID = nID;
            m_strName = strName;
            m_strPhoneNumber = strPhoneNumber;
            m_nRegularTeamID = nRegularTeamID;
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }

        public int TeamID
        {
            get { return m_nRegularTeamID; }
            set { m_nRegularTeamID = value; }
        }
    }
}
