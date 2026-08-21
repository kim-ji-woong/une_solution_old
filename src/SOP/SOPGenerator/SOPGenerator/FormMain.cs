using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using XtremeDockingPane;

namespace SOPGen
{
    public partial class FormMain : Form
    {
        private FormProcess m_frmProcess = null;
        private FormPaneLayer m_frmPaneLayer = null;
        private FormLogin m_frmLogin = null;
        private FormDocking m_frmDocking = null;
        private FormDockingMission m_frmMission = null;

        public Form m_DockingMission = new Form();
        public Form m_Docking = new Form();

        private Pane m_paneMission;
        private Pane m_paneCircum;

        //public DBManager m_dbMgr = null;
        public WebDBManager m_dbMgr = null;

        private VersionData m_currentVersion = null;

        // PINNED : Docking 바 고정된 상태
        // UNPINNED : Docking 바가 화면에서 사라진 상태
        // EXPANDED : Docking 바가 화면에는 보이지만 고정되지 않은 상태
        public enum DockingStatus { PINNED, UNPINNED, EXPANDED, UNKOWN };

        private DockingStatus m_statusMission = DockingStatus.UNPINNED;
        private DockingStatus m_statusCircum = DockingStatus.UNPINNED;

        private string m_strSkinFolder;

        private string m_strVersion = "V1.0";

        public string SkinFolder
        {
            get{return m_strSkinFolder;}
            set { m_strSkinFolder = value;}
        }

        private static FormMain m_Main = null;
        public static FormMain Instance
        {
            get
            {
                if (m_Main == null)
                {
                    m_Main = new FormMain();
                }

                return m_Main;
            }
        }

        public FormMain()
        {
            InitializeComponent();
            SkinFolder = StylesPath();
            Skin_Load();

            //if (m_dbMgr == null)
            //    m_dbMgr = new DBManager(this);

            if (m_dbMgr == null)
                m_dbMgr = new WebDBManager(this);


            m_frmLogin = new FormLogin(this);
            if (m_frmLogin.ShowDialog() != DialogResult.OK)
            {
                // LogIn 실패시 강제 종료
                Application.Exit();

                Application.ExitThread();
                //Environment.Exit(0);
                return;
            }

            m_frmPaneLayer = new FormPaneLayer(this);
            m_frmProcess = new FormProcess(this);
            m_frmMission = new FormDockingMission(this);
            m_frmDocking = new FormDocking(this);
            
            AddChild();
            CreateDockingPane();

            tabCtrlMain.TabPages.Remove(tabPage2);
        }

        public void Skin_Load()
        {
            axSkinFramework1.LoadSkin(m_strSkinFolder + "Vista.cjstyles", "");
            axSkinFramework1.ApplyWindow(this.Handle.ToInt32());
            this.BackColor = axSkinFramework1.GetColor(XtremeSkinFramework.XTPColorManagerColor.STDCOLOR_BTNFACE);
        }

        public string StylesPath()
        {
            string strExePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            System.IO.Directory.Exists(strExePath + "\\Styles\\");

            return strExePath + "\\Styles\\";
        }

        private void AddChild()
        {
            m_frmPaneLayer.TopLevel = false;
            m_frmPaneLayer.Parent = toolStripContainer1.ContentPanel;
            toolStripContainer1.ContentPanel.Controls.Add(m_frmPaneLayer);
            m_frmPaneLayer.Dock = DockStyle.Left;
            m_frmPaneLayer.Show();

            m_frmProcess.TopLevel = false;
            m_frmProcess.Parent = panelProcess;
            panelProcess.Controls.Add(m_frmProcess);
            m_frmProcess.Dock = DockStyle.Fill;
            m_frmProcess.Show();
        }

        //Create DockingPane
        private void CreateDockingPane()
        {
            m_paneMission = axDocking.CreatePane(0, m_frmMission.Size.Width, this.Size.Height, DockingDirection.DockRightOf);
            m_paneMission.Title = "임무관리";

            m_paneCircum = axDocking.CreatePane(1, m_frmDocking.Size.Width, this.Size.Height, DockingDirection.DockRightOf, null);
            m_paneCircum.Title = "상황전파";
            m_paneCircum.Hide();

            m_DockingMission = new FormDockingMission(this);
            m_frmMission = (FormDockingMission)m_DockingMission;

            m_Docking = new FormDocking(this);
            m_frmDocking = (FormDocking)m_Docking;
        }

        private void OnBtnProcessAdd(object sender, EventArgs e)
        {
            m_frmProcess.AddProcess();
            //Invalidate(true);
            //Update();
        }

        private void tsBtnProccessEdit_Click(object sender, EventArgs e)
        {
            m_frmProcess.OnMenuRenameProcess(null, null);
        }

        private void tsBtnProccessDel_Click(object sender, EventArgs e)
        {
            m_frmProcess.OnMenuDeleteProcess(null, null);
        }

        private void OnBtnGroupAdd(object sender, EventArgs e)
        {
            m_frmProcess.AddGroup(null);
        }

        private void tsBtnGroupDel_Click(object sender, EventArgs e)
        {
            m_frmProcess.OnMenuDeleteGroup(null, null);
        }

        public FormPaneLayer GetPaneLayer()
        {
            return m_frmPaneLayer;
        }

        public FormProcess GetProcess()
        {
            return m_frmProcess;
        }

        public FormDockingMission GetMission()
        {
            return m_frmMission;
        }
        
        public FormDocking GetDocking()
        {
            return m_frmDocking;
        }
 
        private void tsBtnGroupEdit_Click(object sender, EventArgs e)
        {
            m_frmProcess.OnMenuRenameGroup(null, null);
            //GetDocking().GetCircumstances().Show();
            //FormTeam.Instance().Show();
        }

        private void axDocking_AttachPaneEvent(object sender, AxXtremeDockingPane._DDockingPaneEvents_AttachPaneEvent e)
        {
            int nIndex = e.item.Id;

            if (nIndex == 0)
                e.item.Handle = m_DockingMission.Handle.ToInt32();
            else if(nIndex == 1)
                e.item.Handle = m_Docking.Handle.ToInt32();
        }

        private void axDocking_Action(object sender, AxXtremeDockingPane._DDockingPaneEvents_ActionEvent e)
        {
            System.Diagnostics.Debug.WriteLine("Action = " + e.action + ", Title = " + e.pane.Title);

            if (e.action == DockingPaneAction.PaneActionSplitterResizing)
            {
                e.cancel = true;
                return;
            }

            if (e.pane.Title == "상황전파")
            {
                if (e.action == DockingPaneAction.PaneActionExpanded)
                {
                    if (m_statusMission == DockingStatus.PINNED)
                    {
                        m_statusMission = DockingStatus.UNPINNED;
                        axDocking.FindPane(0).Hidden = true;
                        ResizePane(2);
                    }
                    m_statusCircum = DockingStatus.EXPANDED;
                }
                else if (e.action == DockingPaneAction.PaneActionPinned)
                    m_statusCircum = DockingStatus.PINNED;
                else if (e.action == DockingPaneAction.PaneActionUnpinned)
                    m_statusCircum = DockingStatus.UNPINNED;
            }
            else if (e.pane.Title == "임무관리")
            {
                if (e.action == DockingPaneAction.PaneActionExpanded)
                {
                    if (m_statusMission == DockingStatus.PINNED)
                    {
                        m_statusCircum = DockingStatus.UNPINNED;
                        axDocking.FindPane(1).Hidden = true;
                        ResizePane(2);
                    }
                    m_statusMission = DockingStatus.EXPANDED;
                }
                else if (e.action == DockingPaneAction.PaneActionPinned)
                    m_statusMission = DockingStatus.PINNED;
                else if (e.action == DockingPaneAction.PaneActionUnpinned)
                    m_statusMission = DockingStatus.UNPINNED;
            }

            // close 막음
            if (DockingPaneAction.PaneActionClosed == e.action)
                e.pane.Closed = false;

            // floating 막음
            if (DockingPaneAction.PaneActionFloated == e.action)
                e.pane.Floating = false;

            if(DockingPaneAction.PaneActionPinned == e.action)
            {
                switch (e.pane.Id)
                {
                    case 0:
                        {
                            SelectGroup();
                        }
                        break;
                    case 1:
                        {
                            SelectProcess();
                        }
                        break;
                    default:
                        break;
                }
            }
            else if (DockingPaneAction.PaneActionUnpinned == e.action)
            {
                if(axDocking.FindPane(0).Hidden && axDocking.FindPane(1).Hidden)
                    ResizePane(2);
            }
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            //m_dbMgr.CloseConnection();
            Application.Exit();
        }

        private void ReadAppVersion()
        {
            try
            {
                System.IO.StreamReader reader = new System.IO.StreamReader("svnGenInfo.txt", Encoding.Default);

                string strLine = reader.ReadLine();
                
                if (strLine != null)
                {
                    int nLen = strLine.Length;
                    int nFirstIndex = -1, nSecondIndex = -1;

                    for (int i = 0; i < nLen; i++)
                    {
                        char ch = strLine.ElementAt(i);

                        if (ch < '0' || ch > '9')
                        {
                            if (nFirstIndex < 0)
                                nFirstIndex = i;
                            else
                            {
                                nSecondIndex = i;
                                break;
                            }
                        }
                    }

                    if (nFirstIndex < 0)
                    {
                        m_strVersion += "." + strLine;
                    }
                    else if (nSecondIndex < 0)
                    {
                        m_strVersion += "." + strLine.Substring(0, nFirstIndex);
                    }
                    else
                    {
                        m_strVersion += "." + strLine.Substring(nFirstIndex + 1, nSecondIndex - nFirstIndex - 1);
                    }
                }

                reader.Close();

                this.Text += " " + m_strVersion;
            }
            catch (System.IO.FileNotFoundException e)
            {
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Point pt = new Point();
            pt.X = m_frmPaneLayer.Location.X + m_frmPaneLayer.Size.Width;

            Size sz = new Size();
            sz.Width = toolStripContainer1.ContentPanel.Width - m_frmMission.Size.Width - m_frmPaneLayer.Size.Width;
            sz.Height = toolStripContainer1.ContentPanel.Height;

            panelProcess.Location = new System.Drawing.Point(pt.X, 0);
            ResizePane(0);

            if (!SelectVersion(true))
            {
                // 강제 종료
                Application.Exit();

                Application.ExitThread();
                Environment.Exit(0);
            }

            m_statusMission = DockingStatus.PINNED;
            ReadAppVersion();
        }

        // isBegining : 폼이 로딩되는 순간인가?
        private bool SelectVersion(bool isBegining)
        {
            //System.Data.SqlClient.SqlDataReader reader;
            ArrayList arrFields = new ArrayList();
            ArrayList arrValues = new ArrayList();

            arrFields.Add("@sopGenUser");
            arrFields.Add("@onlyUserData");

            //arrValues.Add(m_frmLogin.GetLoginID());
            //arrValues.Add("0");
            arrValues.Add("'" + m_frmLogin.GetLoginID() + "'");
            arrValues.Add("'0'");

            // @sopGenUser
            // ID, VersionName, Owner, CreateTime, LastAccessTime, Description
            ArrayList arrResult;
            if (isBegining)
            {
                // 가장 최근에 만들어진 버전을 불러오기
                //m_dbMgr.RunStoredProcedure("sp_LatestVersion", arrFields, arrValues, null, out reader);
                m_dbMgr.RunStoredProcedure("sp_LatestVersion", arrFields, arrValues, 0, out arrResult);
            }
            else
            {
                // 전체 버전 불러오기
                string strSQL = "select Version.ID, VersionName, SOPGenUser.UserID, Version.CreateTime, Version.LastAccessTime, Version.Description from Version, SOPGenUser where Version.OwnerID = SOPGenUser.ID";
                //m_dbMgr.ReadDB(strSQL, null, out reader);
                arrResult = m_Main.m_dbMgr.GetResultData(strSQL, 0);
            }

            int nVersionID = -1;
            string strVersionName = "", strDesc = "", strOwner = "";
            DateTime dtCreate, dtLastAccess, dtDefault = new DateTime();
            FormVersionHistory frm = new FormVersionHistory(!isBegining);

            for (int i = 0; i < arrResult.Count - 5; i = i + 6)
            {
                nVersionID = m_dbMgr.GetIntField(arrResult[i].ToString(), 0);
                strVersionName = m_dbMgr.GetStringField(arrResult[i+1].ToString(), "");
                strOwner = m_dbMgr.GetStringField(arrResult[i + 2].ToString(), "");
                dtCreate = m_dbMgr.GetDateTimeField(arrResult[i + 3].ToString(), dtDefault);
                dtLastAccess = m_dbMgr.GetDateTimeField(arrResult[i + 4].ToString(), dtDefault);
                strDesc = m_dbMgr.GetStringField(arrResult[i + 5].ToString(), "");

                frm.AddVersionData(strVersionName, strOwner, dtCreate, dtLastAccess, strDesc);
            }

            if (nVersionID >= 0)
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    bool isNewBegin = frm.IsNewBegin();

                    if (isNewBegin)
                    {
                        if (!isBegining)
                        {
                            tsBtnNewSOP_Click(null, null);
                        }
                    }
                    else
                    {
                        VersionData data = frm.GetVersionData();
                        if (data == null)
                        {
                            if (isBegining)
                            {
                                MessageBox.Show("버전을 불러오는데 실패하였습니다.\r\n프로그램을 종료합니다.");
                                return false;
                            }
                            else
                                return false;
                        }

                        if (!isBegining)
                            GetPaneLayer().NewSOP();

                        m_currentVersion = data;

                        if (m_dbMgr.LoadSOP(data.VersionName))
                        {
                            m_frmProcess.AfterLoadSOP();
                            //m_frmProcess.AutoAlign();
                        }
                        else
                        {
                            if (isBegining)
                            {
                                MessageBox.Show("버전을 불러오는데 실패하였습니다.\r\n프로그램을 종료합니다.");
                                return false;
                            }
                            else
                                return false;
                        }                            
                    }
                }
                else
                {
                    if (isBegining)
                    {
                        MessageBox.Show("불러올 버전을 선택하지 않았습니다.\r\n프로그램을 종료합니다.");
                        return false;
                    }
                    else
                        return false;
                }
            }

            return true;
        }

        public void OnSelectedSOP(int nDepth, string strSOPFullName, TreeNode node)
        {
            if (m_frmProcess != null)
            {
                m_frmProcess.OnSelectedSOP(nDepth, strSOPFullName, node);
            }
        }

        public void OnChangedSOP(int nDepth, string strSOPFullName, TreeNode node)
        {
            if (m_frmProcess != null)
            {
                m_frmProcess.OnChangedSOP(nDepth, strSOPFullName, node);
            }
        }

        public void OnRemovedSOP(TreeNode node)
        {
            if (m_frmProcess != null)
            {
                m_frmProcess.OnRemovedSOP(node);
            }
        }

        private void tsBtnSave_Click(object sender, EventArgs e)
        {
            m_frmMission.SaveMission();

            ArrayList arrValid = m_frmPaneLayer.ValidCheck();
            string strTemp = "";
            
            if (arrValid.Count != 0)
            {
                foreach (string strPath in arrValid)
                {
                    strTemp = strTemp + strPath + "\r\n";
                }
                string strValue = strTemp + "트리에 공백이 있어 저장할 수 없습니다.";
                MessageBox.Show(strValue);
            }
            else
            {
                if (m_frmProcess != null && m_dbMgr != null)
                {
                    m_dbMgr.SaveSOP(m_frmProcess, m_frmLogin.GetLoginID());
                }
            }
        }

        private void tsBtnLoad_Click(object sender, EventArgs e)
        {
            //m_Main.GetPaneLayer().NewSOP();
            SelectVersion(false);
        }

        public void SelectProcess()
        {
            axDocking.FindPane(0).Hidden = true;
            axDocking.FindPane(1).Hidden = false;
            ResizePane(1);
        }
        
        public void SelectGroup()
        {
            axDocking.FindPane(0).Hidden = false;
            axDocking.FindPane(1).Hidden = true;
            ResizePane(0);
        }

        public void HideExpandedPane()
        {
            bool resize = false;

            if (m_statusMission == DockingStatus.EXPANDED)
            {
                axDocking.FindPane(0).Hidden = true;
                resize = true;
            }

            if (m_statusCircum == DockingStatus.EXPANDED)
            {
                axDocking.FindPane(1).Hidden = true;
                resize = true;
            }

            if (resize)
                ResizePane(2);
        }

        public void ResizePane(int nPane)
        {
            Size sz = toolStripContainer1.ContentPanel.Size;
            switch (nPane)
            {
                case 0:
                    sz.Width = toolStripContainer1.ContentPanel.Width - m_frmPaneLayer.Size.Width - m_frmMission.Size.Width - 23;
                    break;
                case 1:
                    sz.Width = toolStripContainer1.ContentPanel.Width - m_frmPaneLayer.Size.Width - m_frmDocking.Size.Width - 23;
                    break;
                case 2:
                    sz.Width = toolStripContainer1.ContentPanel.Width - m_frmPaneLayer.Size.Width - 23;
                    break;
            }

            panelProcess.Size = sz;
        }
        
        public bool NumberCheck(string strValue)
        {
            if (strValue.Length == 0) return true;

            ArrayList arrValue = new ArrayList();
            arrValue.Add("010");
            arrValue.Add("011");
            arrValue.Add("016");
            arrValue.Add("017");
            arrValue.Add("018");
            arrValue.Add("019");

            bool isCheck = false;
            foreach (string strArr in arrValue)
            {
                if (strValue == strArr)
                {
                    isCheck = true;
                    break;
                }
            }

            return isCheck;
        }

        public bool AreaCodeCheck(string strValue)
        {
            if (strValue.Length == 0) return true;

            ArrayList arrValue = new ArrayList();
            arrValue.Add("02");
            arrValue.Add("031");
            arrValue.Add("032");
            arrValue.Add("033");
            arrValue.Add("041");
            arrValue.Add("042");
            arrValue.Add("043");
            arrValue.Add("044");
            arrValue.Add("051");
            arrValue.Add("052");
            arrValue.Add("053");
            arrValue.Add("054");
            arrValue.Add("061");
            arrValue.Add("062");
            arrValue.Add("063");
            arrValue.Add("064");
            arrValue.Add("060");
            arrValue.Add("070");
            arrValue.Add("080");
            arrValue.Add("010");
            arrValue.Add("011");
            arrValue.Add("016");
            arrValue.Add("017");
            arrValue.Add("018");
            arrValue.Add("019");

            bool isCheck = false;
            foreach (string strArr in arrValue)
            {
                if (strValue == strArr)
                {
                    isCheck = true;
                    break;
                }
            }

            return isCheck;
        }

        private void tsBtnNewSOP_Click(object sender, EventArgs e)
        {
            m_currentVersion = null;

            m_frmProcess.NewSOP();
            m_frmPaneLayer.NewSOP();
            m_frmMission.NewSOP();
            m_frmDocking.NewSOP();
            m_frmProcess.Refresh();
        }
    }
}
