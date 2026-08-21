using DBUtility;
using KpxPipeMonitoring.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Net;
using KpxPipeMonitoring.Popups;

namespace KpxPipeMonitoring
{
    public partial class MainForm : Form
    {
        private static MainForm m_instance = null;
        public static MainForm Instance
        {
            get { return m_instance; }
        } 

        public enum PageKind { PIPE, TANK }

        bool isDual = false;
        System.Windows.Forms.Timer timer = null;
        public int PagingType = Convert.ToInt32(Settings.Default.PagingType);
        public int SiteID = Convert.ToInt32(Settings.Default.SiteId);
        public int nUserID = Convert.ToInt32(Settings.Default.UserId);
        public Excel.Application excelApp = null;
        public DBUtility.WebDBManager dbMgr;
        public CommonFunction commonFunction = null;
        public bool isSound = true;
        public int refreshSec = 8; //짝수만 가능
        public int curSec = 0;
        public bool isChgPublicMsg = true; //공지사항이 변경됐는지 여부
        private string publicMsg = ""; //공지사항 문구 
        public DateTime SystemNow;
        private string strIniFilePath = "";
        private string strVersion = "";
        private string strUpdateFileVersion = "";
        private DateTime dtLastVersionChkTime;
        private int nVersionChkCycle = 60;
        MainForm_Pipe fPipe = null;
        MainForm_Tank fTank = null;  

        public List<CommonFunction.PipeInfo> pipeInfo = new List<CommonFunction.PipeInfo>();
        public List<CommonFunction.TankInfo> tankInfo = new List<CommonFunction.TankInfo>();
        public List<CommonFunction.AlarmTankOptionInfo> alarmTankOptionInfo = new List<CommonFunction.AlarmTankOptionInfo>();
        public List<CommonFunction.AlarmPipeOptionInfo> alarmPipeOptionInfo = new List<CommonFunction.AlarmPipeOptionInfo>();
        public List<CommonFunction.AllAlarm> newAlarmInfo = new List<CommonFunction.AllAlarm>();
         
        public MainForm()
        { 
            this.DoubleBuffered = true;
            m_instance = this;
            dbMgr = new WebDBManager(SiteID);
            dbMgr.DatabaseHost = "127.0.0.1";
            commonFunction = new CommonFunction();

            InitializeComponent();
            
            this.SystemNow = commonFunction.GetDateTimeNow();
            SetSystemLog("System on");

            strIniFilePath = Application.StartupPath + "\\SystemInfo.ini";
            nVersionChkCycle = Convert.ToInt32(GetValue("Version CheckTime", "VersionCheckTime"));
            Microsoft.Win32.RegistryKey rkHKCR = Microsoft.Win32.Registry.ClassesRoot;
            Microsoft.Win32.RegistryKey rkExcelKey = rkHKCR.OpenSubKey(REGISTRY_EXCEL_KEY);
            bExcelInstalled = (rkExcelKey == null ? false : true);
            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ThreadStart(ExcelNew));
            th.Start();

            DisplayPipe();
            DisplayTank();
            DisplayAlarm();
            DisplayOptions();
            fPipe = new MainForm_Pipe();
            fPipe.Size = new Size(1920, 1080);
            fPipe.StartPosition = FormStartPosition.Manual;

            fTank = new MainForm_Tank();
            fTank.Size = new Size(1920, 1080);
            fTank.StartPosition = FormStartPosition.Manual;

            Screen[] sc = Screen.AllScreens;
            if (sc.Length > 1)
            {
                isDual = true;

                switch ((PageKind)Enum.Parse(typeof(PageKind), Settings.Default.MainView.ToUpper()))
                {
                    case PageKind.PIPE:
                        fPipe.Location = new System.Drawing.Point(sc[0].Bounds.Location.X, sc[0].Bounds.Location.Y);
                        fTank.Location = new System.Drawing.Point(sc[1].Bounds.Location.X, sc[1].Bounds.Location.Y);
                        break;
                    case PageKind.TANK:
                        fTank.Location = new System.Drawing.Point(sc[0].Bounds.Location.X, sc[0].Bounds.Location.Y);
                        fPipe.Location = new System.Drawing.Point(sc[1].Bounds.Location.X, sc[1].Bounds.Location.Y);
                        break;
                }

                fPipe.Show();
                fTank.Show();
            }
            else
            {
                isDual = false;

                switch ((PageKind)Enum.Parse(typeof(PageKind), Settings.Default.MainView.ToUpper()))
                {
                    case PageKind.PIPE:
                        fTank.Show();
                        fPipe.Show();
                        break;
                    case PageKind.TANK:
                        fPipe.Show();
                        fTank.Show();
                        break;
                }
            }

            this.timer = new Timer();
            this.timer.Interval = 1000;
            this.timer.Tick += timer_Tick;
            this.timer.Start();

            this.FormClosing += MainForm_FormClosing;
            this.FormClosed += MainForm_FormClosed;
            SetSystemLog("System load");
        }

        void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
             
        }

        void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SetSystemLog("System off");
        } 
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Visible = false;
            ShowInTaskbar = false;
            Opacity = 0; 
        } 
          
        #region 타이머 이벤트 

        void timer_Tick(object sender, EventArgs e)
        {            
            curSec++;

            this.SystemNow = commonFunction.GetDateTimeNow();
            SetTime();

            TimeSpan gg = SystemNow - dtLastVersionChkTime;
            if (gg.TotalMinutes >= nVersionChkCycle)
            {
                VersionCheck();
            }

            if (curSec == refreshSec / 2)
            {
                DisplayAlarm();

                DisplayPipe();
                fPipe.RefreshPipe();

                DisplayTank();
                fTank.RefreshTank();
            }
            else if (curSec == refreshSec)
            {
                DisplayAlarm();

                DisplayPipe();
                fPipe.RefreshPipe();

                DisplayTank();
                fTank.RefreshTank();

                fPipe.RefreshChart();
                DisplayButtonStatus();
                DisplayOptions();
                curSec = 0;
            }

            fPipe.UpdateBeginTimeWorkPipe();
            fTank.UpdateBeginTimeWorkTank();

            // 사운드 버튼 체크
            fPipe.SetSound();
            fTank.SetSound();

            // 공지사항 변경 체크
            if (isChgPublicMsg)
            {
                this.publicMsg = Popups.EnvironmentPop2.LoadPublicMessage();
                isChgPublicMsg = false;
            }
            fPipe.DisplayNotice(this.publicMsg);
            fTank.DisplayNotice(this.publicMsg); 
        }  
        #endregion

        #region 함체버튼 조회
        public void DisplayButtonStatus()
        {
            try
            {
                string strSQL = "SELECT PropertyValue FROM Options WHERE PropertyName = 'ButtonStatus'";
                ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(strSQL, 0);
                if (arrResult == null || arrResult.Count == 0)
                {
                    fPipe.pictureBox_buttonStatus.Visible = false;
                }
                else
                {
                    //0일때 함체 버튼 눌러져 있음
                    int nStatus = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
                    if (nStatus == 0)
                    {
                        fPipe.pictureBox_buttonStatus.Visible = true;
                    }
                    else
                    {
                        fPipe.pictureBox_buttonStatus.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                this.SetSystemLog("[ERROR] DisplayButtonStatus() / " + ex.Message);
            }
        } 
        #endregion

        #region 배관, 탱크, 옵션, 알람 조회
        public void DisplayOptions()
        {
            try
            {
                alarmTankOptionInfo.Clear();
                alarmPipeOptionInfo.Clear();

                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT t.id as TankID, concat(t.Name, ' ', Type) as TankName, StableBeginWorkM, AlarmInterval, AlarmIntervalUse ");
                sb.Append("     , TankStableRatio, TankStableAbsolute, TankStableType, TankStableCTime, TankStableCTimeUse ");
                sb.Append("  FROM Tank t INNER JOIN AlarmOptions ao ON t.ID = ao.TankID ");
                sb.Append(" ORDER BY TankName ");

                ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
                if (arrResult == null) return;

                for (int i = 0; i < arrResult.Count; i += 10)
                {
                    int nTankID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strTankName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);

                    int nStableBeginWorkM = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                    int nAlarmInterval = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                    int nAlarmIntervalUse = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);

                    double nTankStableRatio = (arrResult[i + 5].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 5]);
                    double nTankStableAbsolute = (arrResult[i + 6].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 6]);
                    int nTankStableType = DBUtility.WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);
                    int nTankStableCTime = DBUtility.WebDBManager.GetIntField(arrResult[i + 8].ToString(), -1);
                    int nTankStableCTimeUse = DBUtility.WebDBManager.GetIntField(arrResult[i + 9].ToString(), -1);

                    alarmTankOptionInfo.Add(new CommonFunction.AlarmTankOptionInfo(nTankID, strTankName, nStableBeginWorkM, nAlarmInterval, nAlarmIntervalUse, nTankStableRatio, nTankStableAbsolute, nTankStableType, nTankStableCTime, nTankStableCTimeUse));
                }

                sb = new StringBuilder();
                sb.Append("SELECT p.id as PipeID, concat(p.Name, ' ', Type) as PipeName ");
                sb.Append("     , PipeStableRatio, PipeStableAbsolute, PipeStableType, PipeStableCTime, PipeStableCTimeUse ");
                sb.Append("  FROM Pipe as p INNER JOIN AlarmPipeOptions ao ON p.ID = ao.PipeID ");
                sb.Append(" ORDER BY PipeName ");

                arrResult = MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
                if (arrResult == null) return;

                for (int i = 0; i < arrResult.Count; i += 7)
                {
                    int nPipeID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strPipeName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);

                    double nPipeStableRatio = (arrResult[i + 2].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 2]);
                    double nPipeStableAbsolute = (arrResult[i + 3].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 3]);
                    int nPipeStableType = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                    int nPipeStableCTime = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                    int nPipeStableCTimeUse = DBUtility.WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);

                    alarmPipeOptionInfo.Add(new CommonFunction.AlarmPipeOptionInfo(nPipeID, strPipeName, nPipeStableRatio, nPipeStableAbsolute, nPipeStableType, nPipeStableCTime, nPipeStableCTimeUse));
                }
            }
            catch (Exception ex)
            {
                this.SetSystemLog("[ERROR] DisplayOptions() / " + ex.Message);
            }
        } 

        private void DisplayPipe()
        {
            try
            {
                pipeInfo.Clear();

                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT p.ID as PipeID, Name, Type, StandardPressure, StandardFlow, lwh.TankID, Pressure ");
                sb.Append("  FROM Pipe as p LEFT OUTER JOIN (select * from lastworkhistory where endtime is null) as lwh ON p.id=lwh.pipeid ");
                sb.Append(" ORDER BY Name ");
                ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
                if (arrResult == null) return;
                
                Dictionary<int, KpxPipeMonitoring.CommonFunction.PipeInfo> dicPipeName = new Dictionary<int, KpxPipeMonitoring.CommonFunction.PipeInfo>();
                pipeInfo.Clear();
                for (int i = 0; i < arrResult.Count; i += 7)
                {
                    int nPipeID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strPipeName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);
                    string strPipeType = DBUtility.WebDBManager.GetStringField(arrResult[i + 2]);
                    double nStandardPressure = (arrResult[i + 3].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 3]);
                    double nStandardFlow = (arrResult[i + 4].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 4]);
                    int nConnectTankID = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                    double nPressure = (arrResult[i + 6].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 6]);

                    pipeInfo.Add(new CommonFunction.PipeInfo(nPipeID, strPipeName, strPipeType, nStandardPressure, nStandardFlow, nConnectTankID, nPressure));
                }
            }
            catch (Exception ex)
            {
                this.SetSystemLog("[ERROR] DisplayPipe() / " + ex.Message);
            }
        }
                
        private void DisplayTank()
        {
            try
            {
                tankInfo.Clear();

                Dictionary<int, List<int>> dicConnectPipeIDs = commonFunction.ReturnConnectPipeIDs();

                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT Name, LiquidType, Density, Temperature, Mass, Level, Flow, Type, HighLevel, Capacity, MinTemp, MaxTemp, t.ID ");
                sb.Append("     , (select count(tankid) from LastWorkHistory where endtime is null and tankid=t.id) as iswork  ");
                //sb.Append("     , ifnull(lwh.PipeID, ifnull(lwh.AnotherLink, -1)) as connectPipeID, CASE WHEN lwh.AnotherLink = -100 THEN 'PO' WHEN lwh.AnotherLink = -200 THEN '황산' ELSE (select name from pipe where id=lwh.pipeid) END as connectPipeName ");
                sb.Append("     , (select count(*) from disconnectedtank dt where dt.tankid=t.id) disconnected, LeakLevel, LevelTime ");
                sb.Append("     , OrgHighLevel, OrgMinTemp, OrgMaxTemp, lwh.StandardFlow, IsLeakStatus, IsLeakMonitoring ");
                sb.Append("  FROM Tank as t  ");
                sb.Append("  LEFT OUTER JOIN (select distinct lwh.tankid, standardflow from lastworkhistory as lwh  ");
                sb.Append("  INNER JOIN (select tankid, max(StandardFlowUpdateTime) as StandardFlowUpdateTime from lastworkhistory where endtime is null group by tankid) as lwh2 ");
                sb.Append("  ON lwh.tankid = lwh2.tankid and lwh.StandardFlowUpdateTime=lwh2.StandardFlowUpdateTime) as lwh ON t.id=lwh.tankid ");
                sb.Append(" ORDER BY Name ");

                ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
                if (arrResult == null) return;

                for (int i = 0; i < arrResult.Count; i += 23)
                {
                    string strTankName = WebDBManager.GetStringField(arrResult[i]);
                    string liquidType = WebDBManager.GetStringField(arrResult[i + 1]);
                    if (liquidType == "N-BUTANOL") liquidType = "BUTANOL";
                    else if (liquidType == "메틸렌클로라이드") liquidType = "MC";
                    double nDensity = (arrResult[i + 2].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 2]);
                    double nTemp = (arrResult[i + 3].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 3]);
                    double nMass = (arrResult[i + 4].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 4]);
                    double nCurLevel = (arrResult[i + 5].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 5]);
                    double nFlow = (arrResult[i + 6].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 6]);
                    string strType = WebDBManager.GetStringField(arrResult[i + 7]);
                    double nHighLevel = (arrResult[i + 8].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 8]);
                    int nCapacity = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 0);
                    double nMinTemp = (arrResult[i + 10].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 10]);
                    double nMaxTemp = (arrResult[i + 11].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 11]);
                    int nTankID = WebDBManager.GetIntField(arrResult[i + 12].ToString(), -1);
                    int nIsWorkTankID = WebDBManager.GetIntField(arrResult[i + 13].ToString(), -1); // 작업중이라면 Tank ID가 들어감 
                    bool bDisconnected = (DBUtility.WebDBManager.GetIntField(arrResult[i + 14].ToString(), -1) == 1) ? true : false;
                    double nLeakLevel = (arrResult[i + 15].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 15].ToString());
                    int nLeakTime = WebDBManager.GetIntField(arrResult[i + 16].ToString(), -1);

                    double nOrgHighLevel = (arrResult[i + 17].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 17]);
                    double nOrgMinTemp = (arrResult[i + 18].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 18]);
                    double nOrgMaxTemp = (arrResult[i + 19].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 19]);

                    double nStandardFlow = (arrResult[i + 20].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 20]);
                    bool bIsLeakStatus = (DBUtility.WebDBManager.GetIntField(arrResult[i + 21].ToString(), -1) == 1) ? true : false;
                    bool bIsLeakMonitoring = (DBUtility.WebDBManager.GetIntField(arrResult[i + 22].ToString(), -1) == 1) ? true : false; 
                    bool isWork = false;
                    if (nIsWorkTankID > 0)
                        isWork = true;

                    List<int> nConnectPipeIDs = new List<int>();
                    List<string> nConnectPipeNames = new List<string>();
                    if (dicConnectPipeIDs != null || dicConnectPipeIDs.Count > 0)
                    {
                        if (dicConnectPipeIDs.ContainsKey(nTankID) && dicConnectPipeIDs[nTankID].Count > 0)
                        {
                            nConnectPipeIDs = dicConnectPipeIDs[nTankID];
                            if (nConnectPipeIDs[0] == -100)
                                nConnectPipeNames.Add("PO");
                            else if (nConnectPipeIDs[0] == -200)
                                nConnectPipeNames.Add("황산");
                            else
                            {
                                foreach (int pipeid in nConnectPipeIDs)
                                {
                                    foreach (CommonFunction.PipeInfo pipeinfo in MainForm.Instance.pipeInfo)
                                    {
                                        if (pipeinfo.nPipeID == pipeid)
                                        {
                                            nConnectPipeNames.Add(pipeinfo.strPipeName);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    tankInfo.Add(new CommonFunction.TankInfo(nTankID, strTankName, liquidType, nDensity, nTemp, nMass, nCurLevel, nFlow, strType, nHighLevel, nCapacity, nMinTemp, nMaxTemp, nOrgHighLevel, nOrgMinTemp, nOrgMaxTemp, isWork, nConnectPipeIDs, nConnectPipeNames, bDisconnected, nLeakLevel, nLeakTime, nStandardFlow, bIsLeakStatus, bIsLeakMonitoring));
                }
            }
            catch (Exception ex)
            {
                this.SetSystemLog("[ERROR] DisplayTank() / " + ex.Message);
            }
        }

        private void DisplayAlarm()
        {
            try
            {
                newAlarmInfo.Clear();

                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT arh.TankID, arh.PipeID, ah.ID as AlarmHistoryID, BeginTime, AlarmType ");
                sb.Append("     , (select description from alarmType as at where at.id=ah.alarmType) as AlarmDescription ");
                sb.Append("     , (select UserName from user as u where u.id=ah.AlarmTerminator) as AlarmTerminator ");
                sb.Append("     , StandardValue, StandardRange, RealValue, alarmOccurType, alarmComment ");
                sb.Append("  FROM AlarmRecentHistory as arh  ");
                sb.Append(" INNER JOIN AlarmHistory as ah ON (arh.AlarmHistoryID1=ah.id OR ");
                sb.Append("                                   arh.AlarmHistoryID2=ah.id OR ");
                sb.Append("                                   arh.AlarmHistoryID3=ah.id OR ");
                sb.Append("                                   arh.AlarmHistoryID4=ah.id)  ");
                sb.Append("                              AND (arh.PipeID=ah.PipeID OR arh.PipeID = -1 OR arh.PipeID IS NULL) ");
                sb.Append("                              AND arh.TankID=ah.TankID AND ah.Endtime IS NULL ");
                sb.Append(" UNION ALL ");
                sb.Append("SELECT t.ID, -1, tlh.ID, BeginTime, -1, '황산누출' ");
                sb.Append("     , (select UserName from user as u where u.id=tlh.AlarmTerminator) as AlarmTerminator  ");
                sb.Append("     , 0,0,0,0,'' ");
                sb.Append("  FROM Tank as t INNER JOIN TankLeak as tl ON t.id=tl.tankID ");
                sb.Append("                 INNER JOIN TankLeakHistory as tlh ON tl.HistoryID=tlh.ID ");
                sb.Append(" WHERE t.LiquidType='황산' ");
                sb.Append("   AND tlh.EndTime IS NULL ");

                ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
                if (arrResult == null) return;

                for (int i = 0; i < arrResult.Count; i += 12)
                {
                    int nTankID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    int nPipeID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                    int nAlarmHistoryID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                    VariousData<DateTime> dtBeginTime = WebDBManager.GetDateTimeField(arrResult[i + 3]);
                    int nAlarmType = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                    string strAlarmDescription = WebDBManager.GetStringField(arrResult[i + 5]);
                    string strAlarmTerminator = WebDBManager.GetStringField(arrResult[i + 6]);
                    double nStandardValue = (arrResult[i + 7].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 7]);
                    double nStandardRange = (arrResult[i + 8].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 8]);
                    double nRealValue = (arrResult[i + 9].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 9]);
                    int nOccurType = WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);
                    string strComment = WebDBManager.GetStringField(arrResult[i + 11]);

                    if (nAlarmHistoryID > 0)
                        newAlarmInfo.Add(new CommonFunction.AllAlarm(nTankID, nPipeID, nAlarmHistoryID, dtBeginTime.Data, nAlarmType, strAlarmDescription, strAlarmTerminator, nStandardValue, nStandardRange, nRealValue, nOccurType, strComment));
                }
            }
            catch (Exception ex)
            {
                this.SetSystemLog("[ERROR] DisplayAlarm() / " + ex.Message);
            }
        } 
        #endregion 

        private void VersionCheck()
        {
            try
            {
                this.dtLastVersionChkTime = this.SystemNow;

                ArrayList arrList = MainForm.Instance.dbMgr.GetResultData("SELECT PropertyName, PropertyValue FROM Options WHERE PropertyName IN ('ClientVersion', 'UpdateFile', 'UpdateFileVersion')", 0);
                if (arrList == null || arrList.Count == 0)
                    return;

                bool versionUp = false;
                string strNewVersion = "";
                bool updateUp = false;
                string strNewUpdateFileVersion = "";
                string[] updateFiles = null;
                for (int i = 0; i < arrList.Count; i += 2)
                {
                    string strPropertyName = DBUtility.WebDBManager.GetStringField(arrList[i]);
                    string strPropertyValue = DBUtility.WebDBManager.GetStringField(arrList[i + 1]);

                    if (strPropertyName == "ClientVersion")
                    {
                        strVersion = GetValue("Version Info", "Version");
                        if (strVersion != strPropertyValue)
                        {
                            versionUp = true;
                            strNewVersion = strPropertyValue;
                        } 
                    }
                    else if (strPropertyName == "UpdateFile")
                    {
                        if (strPropertyValue.Trim().Length > 0)
                        {
                            updateFiles = strPropertyValue.Split('/');
                        }
                    }
                    else if (strPropertyName == "UpdateFileVersion")
                    {
                        strUpdateFileVersion = GetValue("UpdateFile Version Info", "UpdateFileVersion");
                        if (strUpdateFileVersion != strPropertyValue)
                        {
                            updateUp = true;
                            strNewUpdateFileVersion = strPropertyValue;
                        }
                    }
                }
                
                if (versionUp || updateUp)
                {
                    AutoUpdate auto = new AutoUpdate();
                    auto.StartPosition = FormStartPosition.CenterParent;

                    if (auto.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                    {
                        string strFileName = "", strFolder = "";

                        if (updateUp)
                        { 
                            foreach (string updateFile in updateFiles)
                            {
                                if (!DownloadFile(updateFile)) 
                                    return; 
                            }

                            strFileName = "KpxMonitoring.exe";
                            strFolder = Application.StartupPath;

                            strUpdateFileVersion = strNewUpdateFileVersion;
                            SetValue("UpdateFile Version Info", "UpdateFileVersion", strNewUpdateFileVersion);
                        }

                        if (versionUp)
                        {    
                            if (!DownloadSelf(ref strFileName, ref strFolder))
                                return;

                            SetSystemLog("[INFO] VersionCheck : client new version download ok");

                            strVersion = strNewVersion;
                            SetValue("Version Info", "Version", strNewVersion);
                        }
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                        startInfo.FileName = strFileName;
                        startInfo.WorkingDirectory = strFolder;
                        startInfo.ErrorDialog = true;
                        startInfo.Arguments = System.Diagnostics.Process.GetCurrentProcess().Id.ToString();

                        try
                        {
                            System.Diagnostics.Process.Start(startInfo);
                            SetSystemLog("[INFO] VersionCheck : client Start ok");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Trace.WriteLine(ex.Message);
                            SetSystemLog("[ERROR] VersionCheck : " + ex.Message);
                            return;
                        } 
                        
                        timer.Stop();
                        timer.Dispose();
                        this.Dispose();
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                this.SetSystemLog("[ERROR] VersionCheck / " + ex.Message);
            }
        } 
        private bool DownloadSelf(ref string strDownloadFileName, ref string strDownloadFolderPath)
        {
            string strURL = "http://unes.iptime.org:10091/SOP/KPX/";

            if (strURL.Length == 0)
                return false;
             
            string strPath = System.Windows.Forms.Application.ExecutablePath;

            int nIndex = strPath.LastIndexOf('\\');
            string strFolder = strPath.Substring(0, nIndex + 1);

            int nIndex2 = strPath.LastIndexOf('.');
            string strLocalFileName = strPath.Substring(nIndex + 1, nIndex2 - nIndex - 1) + "_temp.exe";// +strPath.Substring(nIndex2);
            string strFilePath = strFolder + strLocalFileName;

            strURL += strPath.Substring(nIndex + 1);
            strURL = strURL.Replace(".EXE", ".exe");

            strDownloadFileName = strLocalFileName;
            strDownloadFolderPath = strFolder;

            try
            {
                if (System.IO.File.Exists(strFilePath))
                    System.IO.File.Delete(strFilePath);

                System.Net.WebClient web = new System.Net.WebClient();

                Uri uri = new Uri(strURL);

                CredentialCache credentials = new CredentialCache();
                NetworkCredential netCredential = new NetworkCredential("sop", "sop");
                credentials.Add(uri, "Basic", netCredential);
                web.Credentials = new NetworkCredential("sop", "sop");
                 
                web.DownloadFile(strURL, strFilePath);
                SetSystemLog("[INFO] DownloadSelf : strURL / " + strURL + " , strFilePath / " + strFilePath);
            }
            catch (Exception ex)
            {
                this.SetSystemLog("[ERROR] DownloadSelf(ref string strDownloadFileName, ref string strDownloadFolderPath) : " + ex.Message);
                return false;
            }

            return true;
        }
        private bool DownloadFile(string strDownloadFileName)
        {
            string strURL = "http://unes.iptime.org:10091/SOP/KPX/";

            if (strURL.Length == 0)
                return false;

            string strPath = System.Windows.Forms.Application.StartupPath + "\\";

            string[] strDownloadFileName2 = strDownloadFileName.Split('.');
            string strLocalFileName = "";//strDownloadFileName2[0] + "_temp." + strDownloadFileName2[1];
            for (int i = 0; i < strDownloadFileName2.Length; i++)
            {
                if (i == 0)
                    strLocalFileName = strDownloadFileName2[i] + "_temp";
                else
                    strLocalFileName += "." + strDownloadFileName2[i];
            }

            if (strLocalFileName.Length == 0) 
                return false;

            string strFilePath = strPath + strLocalFileName;

            strURL += strDownloadFileName;
            strURL = strURL.Replace(".EXE", ".exe"); 

            try
            {
                if (System.IO.File.Exists(strFilePath))
                    System.IO.File.Delete(strFilePath);

                System.Net.WebClient web = new System.Net.WebClient();

                Uri uri = new Uri(strURL);

                CredentialCache credentials = new CredentialCache();
                NetworkCredential netCredential = new NetworkCredential("sop", "sop");
                credentials.Add(uri, "Basic", netCredential);
                web.Credentials = new NetworkCredential("sop", "sop");
                 
                web.DownloadFile(strURL, strFilePath);

                SetSystemLog("[INFO] DownloadFile : strURL / " + strURL + " , strFilePath / " + strFilePath);
            }
            catch (Exception ex)
            {
                this.SetSystemLog("[ERROR] DownloadFile : " + ex.Message);
                return false;
            }

            return true;
        }

        #region 시간조회
        private void SetTime()
        { 
            string week = string.Empty;
            switch (SystemNow.DayOfWeek)
            {
                case DayOfWeek.Monday: week = "월"; break;
                case DayOfWeek.Tuesday: week = "화"; break;
                case DayOfWeek.Wednesday: week = "수"; break;
                case DayOfWeek.Thursday: week = "목"; break;
                case DayOfWeek.Friday: week = "금"; break;
                case DayOfWeek.Saturday: week = "토"; break;
                case DayOfWeek.Sunday: week = "일"; break;
            }

            fPipe.SetTime(SystemNow.ToString("yyyy-MM-dd") + "(" + week + ") " + SystemNow.ToString("HH:mm:ss"));
            fTank.SetTime(SystemNow.ToString("yyyy-MM-dd") + "(" + week + ") " + SystemNow.ToString("HH:mm:ss"));
        } 
        #endregion 

        #region 로그
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public string SetValue(string section, string key, string value)
        {
            StringBuilder temp = new StringBuilder(1024);
            string strPath = strIniFilePath;
            WritePrivateProfileString(section, key, value, strPath);
            return temp.ToString();
        }

        public string GetValue(string section, string key)
        {
            StringBuilder temp = new StringBuilder();
            string strPath = strIniFilePath;
            int nResult = GetPrivateProfileString(section, key, "", temp, 32, strPath); 
            return temp.ToString();
        }

        public void SetSystemLog(string content)
        {
            string filePath = @"D:\Tomcat 7.0\webapps\ROOT\SOP\KPX\SoundBtn.log";
            string dirPath = @"D:\Tomcat 7.0\webapps\ROOT\SOP\KPX";

            DirectoryInfo di = new DirectoryInfo(dirPath);
            FileInfo fi = new FileInfo(filePath);
             
            try
            {
                if (!di.Exists) Directory.CreateDirectory(dirPath);
                if (!fi.Exists)
                {
                    using (StreamWriter sw = new StreamWriter(filePath))
                    {
                        sw.WriteLine("[Monitoring System " + this.SystemNow.ToString("yyyy-MM-dd HH:mm:ss") + "]    " + content);
                        sw.Close();
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(filePath))
                    {
                        sw.WriteLine("[Monitoring System " + this.SystemNow.ToString("yyyy-MM-dd HH:mm:ss") + "]    " + content);
                        sw.Close();
                    }
                }
            }
            catch (Exception)
            {

            }
        } 
        #endregion

        #region Buffer
        public void SetDoubleBuffer(Panel panel, bool bEnabled)
        {
            Type dgvType1 = panel.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(panel, bEnabled, null);
        }

        public void SetDoubleBuffer(DataGridView gvView, bool bEnabled)
        {
            Type dgvType1 = gvView.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(gvView, bEnabled, null);
        } 
        #endregion          

        #region 엑셀
        private const string REGISTRY_EXCEL_KEY = @"Excel.Application";
        public static bool bExcelInstalled = false;
        void ExcelNew()
        {
            try
            {
                if (bExcelInstalled)
                    excelApp = new Excel.Application();
            }
            catch (Exception)
            {                 
            }
        } 
        #endregion

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e); 
            if (bExcelInstalled)
            {
                if (excelApp != null)
                    excelApp.Quit();
                try
                {
                    if (excelApp != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    excelApp = null;
                }
                catch (Exception)
                {
                    excelApp = null;
                }
                finally
                {
                    GC.Collect();
                }
            }
        }
    }
}
