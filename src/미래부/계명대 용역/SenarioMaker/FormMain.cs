using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.GUI;
using IronPython;
using System.Xml;
using System.IO;

namespace UnE.SenarioMaker
{

    internal partial class FormMain : Form, IMenuCommandOwner, IRibbonButtonOwner, ITextCommanderOwner, ISOPTreeNodeSelection
    {
        #region Status Bar Key 상태
        [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true, CallingConvention=CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode); 
        public void OnIdle(object sender, EventArgs e)
        {
            // Update the panels when the program is idle.
            bool CapsLock = (((ushort)GetKeyState(0x14 /*VK_CAPITAL*/)) & 0x01) == 0x01;
            bool NumLock = (((ushort)GetKeyState(0x90 /*VK_NUMLOCK*/)) & 0x01) == 0x01;
            bool Hangul = (((ushort)GetKeyState(0x15 /*VK_NUMLOCK*/)) & 0x01) == 0x01;

            if (mStatsCaps != null)
            {
                mStatsCaps.Text = CapsLock ? "CAP" : "";
            }
            if (mStatusNum != null)
            {
                mStatusNum.Text = NumLock ? "NUM" : "";
            }           
            if (mStatusHanguel != null)
            {
                mStatusHanguel.Text = Hangul == false ? "영문" : "한글";
            }
        }
        #endregion

        #region Python ==========================================================================
        private TextCommander commander = null;
        public TextCommander Commander
        {
            get { return commander; }
            set { commander = value; }
        }
        public void AddPythonFunction()
        {
            ScriptProxy proxy = ScriptProxy.Instance;
            proxy.UserObject.MainForm = this;
            proxy.UserObject.Script = ScriptProxy.Instance;

        }

        private bool m_bWriteConsole = false;
        public void SetConsoleLog(bool bOnOff)
        {
            m_bWriteConsole = bOnOff;

            if (bOnOff == true)
            {
                if (tmrUpdate == null)
                {
                    ConsoleDebugger.Instance.EnableLogger = true;
                    tmrUpdate = new System.Timers.Timer();

                    tmrUpdate.Elapsed += new System.Timers.ElapsedEventHandler(tmrUpdate_Tick);
                    tmrUpdate.Interval = 1000;
                    tmrUpdate.Enabled = false;
                }

                if (tmrUpdate.Enabled == false)
                {

                    _logger = ScriptProxy.Instance.Logger;
                    tmrUpdate.Enabled = true;
                    tmrUpdate.Start();
                }
            }
            else
            {
                ConsoleDebugger.Instance.EnableLogger = false;
                _logger = null;
                tmrUpdate.Enabled = false;
                tmrUpdate.Stop();
            }
        }

        private System.Timers.Timer tmrUpdate = null;
        private PythonLogger _logger = null;
        private void tmrUpdate_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_logger == null)
                return;

            List<PythonLogger.Entry> entries = _logger.GetAll();
            foreach (var entry in entries)
            {
                string szLog = entry.ToString();
                if (szLog.Contains("Fault"))
                {
                    ConsoleDebugger.WriteLine(entry.ToString(), ConsoleColor.Red);
                }
                else
                    ConsoleDebugger.WriteLine(entry.ToString(), ConsoleColor.Green);
            }

            if (m_bWriteConsole == true && entries.Count > 0)
                ConsoleDebugger.Write(">> ", ConsoleColor.Red);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!base.ProcessCmdKey(ref msg, keyData))
            {
                if (keyData.Equals(Keys.Control | Keys.Insert))
                {
                    if (commander != null && commander.IsInit == true)
                    {
                        ConsoleDebugger.Instance.ShowConsole(true);
                        return true;
                    }
                }
                else if (keyData.Equals(Keys.Control | Keys.Z))
                {
                    Undo();
                }
                else if (keyData.Equals(Keys.Control | Keys.Y))
                {
                    Redo();
                }
                else if(keyData.Equals(Keys.F1))
                {
                    if (m_FormFrameHelp == null)
                        CreateHelpForm();
                    m_FormFrameHelp.Visible = true;
                }
                return false;
            }
            return true;
        }
        public void CreatePythonContext()
        {
            commander = new TextCommander(this);
            // Init Text commander
            commander.InitCommander();

            // Create Console and Begin Input Thread
            if (commander.BeginCommnander())
            {
                // Create Python Context
                AddPythonFunction();
            }
        }
        public void DisposePythonContext()
        {
            if (commander != null)
            {
                if (tmrUpdate != null)
                {
                    tmrUpdate.Stop();
                    tmrUpdate.Enabled = false;
                }

                commander.StopCommander();
                commander.Dispose();
            }
        }
        #endregion//Python ==========================================================================


        ///////////////////////////////////////////////////////////////////////////
        private static FormMain m_instance = null;
        public static FormMain Instance
        {
            get { return m_instance; }
        }

        private ArrayList m_arRibbonButtons = new ArrayList();

        private FormContent contentForm = null;
        public FormContent ContentForm
        {
            get { return contentForm; }
        }

        private FormSelectComponet componentForm = null;
        public FormSelectComponet ComponentForm
        {
            get { return componentForm; }
        }

        private FormScriptSetup m_FormRunScript = null;
        private FormEnumeration m_formEnumeration = null;

        internal FormEnumeration FormEnumeration
        {
            get { return m_formEnumeration; }
            set { m_formEnumeration = value; }
        }
        private FormSystemVariable m_formSystemVariable = null;
        private FormUserVariable m_formUserVariable = null;
        internal FormUserVariable FormUserVariable
        {
            get { return m_formUserVariable; }
            set { m_formUserVariable = value; }
        }

        private FormTreeStep m_formTreeStep = null;
        internal FormTreeStep TreeForm
        {
            get { return m_formTreeStep; }
            set { m_formTreeStep = value; }
        }

        private FormProperties m_formProperties = null;
        public FormProperties PropertiesForm
        {
            get { return m_formProperties; }
        }

        private PopupFormHelp m_formhelp = null;
        public PopupFormHelp Formhelp
        {
            get { return m_formhelp; }
            set { m_formhelp = value; }
        }

        ///////////////////////////////////////////////////////////////////////////

        public FormMain()
        {       
            m_instance = this;

            InitializeComponent();
                       
            SetCommandID();

            Color backColor = Color.FromArgb(75, 71, 86);
            Color textColor = Color.White;

            CustomMenuHelper helper = new CustomMenuHelper(this);
            helper.MakeCustomLookMenu(mMainMenuStrip, backColor, textColor);

            Application.Idle += new System.EventHandler(OnIdle);

            m_formhelp = new PopupFormHelp();
        }

        private void InitForm()
        {
            m_formEnumeration = new FormEnumeration();
            m_formSystemVariable = new FormSystemVariable();
            m_formUserVariable = new FormUserVariable();

            m_FormRunScript = new FormScriptSetup();
            m_FormRunScript.Dock = DockStyle.Fill;
            m_FormRunScript.Visible = false;

            m_formEnumeration.Dock = DockStyle.Fill;
            m_formSystemVariable.Dock = DockStyle.Fill;
            m_formUserVariable.Dock = DockStyle.Fill;
            
            mSplitContent.Panel2.Controls.Add(m_FormRunScript);
            mSplitContent.Panel2.Controls.Add(m_formEnumeration);
            mSplitContent.Panel2.Controls.Add(m_formSystemVariable);
            mSplitContent.Panel2.Controls.Add(m_formUserVariable);

            contentForm = new FormContent();
            contentForm.TopLevel = false;
            contentForm.Dock = DockStyle.Fill;
            mSplitContent.Panel2.Controls.Add(contentForm);

            SenarioManager.Instance.SectionPageOwner = contentForm;
            contentForm.Show();
        }  

        private void InitLeftForm()
        {
            componentForm = new FormSelectComponet();
            componentForm.TopLevel = false;
            componentForm.Dock = DockStyle.Fill;
            mLeftUpSplit.Panel1.Controls.Add(componentForm);           
            componentForm.Show();         

            m_formTreeStep = new FormTreeStep();
            m_formTreeStep.Visible = true;
            m_formTreeStep.Dock = DockStyle.Fill;
            mLeftDownSplit.Panel1.Controls.Add(m_formTreeStep);
            m_formTreeStep.SOPTreeNodeSelectionOwner = this;
            m_formTreeStep.Show();

            m_formProperties = new FormProperties();
            m_formProperties.TopLevel = false;
            m_formProperties.Dock = DockStyle.Fill;
            mLeftDownSplit.Panel2.Controls.Add(m_formProperties);
            m_formProperties.Show();
        }
        
        private void SetCommandID()
        {
            mRBtnUndo.Owner = this;
            mRBtnRedo.Owner = this;
            mRBtnSystemVar.Owner = this;           
            mRBtnUserVar.Owner = this;
            mRBtnEnum.Owner = this;
            mRBtnHelp.Owner = this;

			mRbtnSimulation.Owner = this;
			mRBtnVerify.Owner = this;

            mRBtnUndo.CheckButton = false;
            mRBtnRedo.CheckButton = false;
            mRBtnSystemVar.CheckButton = true;
            mRBtnUserVar.CheckButton = true;
            mRBtnEnum.CheckButton = true;
            mRBtnHelp.CheckButton = true;

			mRbtnSimulation.CheckButton = false;
			mRBtnVerify.CheckButton = false;

            m_arRibbonButtons.Add(mRBtnUndo);
            m_arRibbonButtons.Add(mRBtnRedo);
            m_arRibbonButtons.Add(mRBtnSystemVar);
            m_arRibbonButtons.Add(mRBtnUserVar);
            m_arRibbonButtons.Add(mRBtnEnum);
            m_arRibbonButtons.Add(mRBtnHelp);

			m_arRibbonButtons.Add(mRbtnSimulation);
			m_arRibbonButtons.Add(mRBtnVerify);

            mRBtnUndo.ID = CommandID.EDIT_UNDO;            
            mRBtnRedo.ID = CommandID.EDIT_REDO;
            mRBtnSystemVar.ID = CommandID.FILE_SYSTEM_VAR_OPEN;
            mRBtnUserVar.ID = CommandID.FILE_USER_VAR_OPEN;
            mRBtnEnum.ID = CommandID.FILE_ENUM_OPEN;
            mRBtnHelp.ID = CommandID.VIEW_EXPR_HELP;

			mRbtnSimulation.ID = CommandID.SENARIO_SIMULATION;
			mRBtnVerify.ID = CommandID.SENARIO_VERIFY;

            mSaveAsMenuItem.CommandID = CommandID.FILE_SENARIO_SAVEAS;
            mNewSennarioMenuItem.CommandID = CommandID.FILE_SENARIO_NEW;
            mOpenSennarioMenuItem.CommandID = CommandID.FILE_SENARIO_OPEN;
            mSaveSenarioMenuItem.CommandID = CommandID.FILE_SENARIO_SAVE;
            mOpenUserVarMenuItem.CommandID = CommandID.FILE_USER_VAR_OPEN;
            mSaveUserVarMenuItem.CommandID = CommandID.FILE_USER_VAR_SAVE;
            mOpenEnumMenuItem.CommandID = CommandID.FILE_ENUM_OPEN;           
            saveEnumToolstripMenuItem.CommandID = CommandID.FILE_ENUM_SAVE;
            exitToolStripMenuItem.CommandID = CommandID.FILE_EXIT;
            
            viewExprToolStripMenuItem.CommandID = CommandID.VIEW_EXPR;
            viewExprToolStripMenuItem.CheckOnClick = true;
            viewTextToolStripMenuItem.CommandID = CommandID.VIEW_TEXT;
            viewTextToolStripMenuItem.CheckOnClick = true;
            compOptionToolStripMenuItem.CommandID = CommandID.VIEW_OPTION;
            compOptionToolStripMenuItem.CheckOnClick = true;
            leftPaneToolStripMenuItem.CommandID = CommandID.VIEW_LEFTPANE;
            leftPaneToolStripMenuItem.CheckOnClick = true;

            //viewTextToolStripMenuItem.Checked = true;
        }        

        public ToolStripStatusLabel GetStatusLabel()
        {
            return mStatusWork;
        }        

        public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
        {           
        }   

        public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
        {
            RibbonButton rbtn = (RibbonButton)sender;
            int nCmdID = rbtn.ID;
            if (rbtn.CheckButton)
            {
                bool bChecked = !rbtn.IsChecked;
                CheckedChanged(nCmdID, bChecked);
            }
            else
            {
                RunCommand(nCmdID);
            }
        }
        
        private void statusClockTimer_Tick(object sender, EventArgs e)
        {
            DateTime dtNow = DateTime.Now;
            if(mStatusClock != null)
            {
                mStatusClock.Text = dtNow.ToLongDateString() + " " + dtNow.ToLongTimeString();
            }
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            mUpdateTimer.Enabled = false;
            mUpdateTimer.Stop();

            DisposePythonContext();
           
            statusClockTimer.Stop();
            statusClockTimer.Enabled = false;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            CreatePythonContext();
            InitForm();

            InitLeftForm();

            mUpdateTimer.Enabled = true;
            mUpdateTimer.Start();

            SenarioManager.Instance.NewSenario("새시나리오", 1);

            string szCategory = SenarioManager.Instance.Category;
            string szSubCategory = SenarioManager.Instance.SubCategory;
            string szDisasterType = SenarioManager.Instance.DisasterType;
            ArrayList ar = SenarioManager.Instance.ActionStepList;

            m_formTreeStep.SetTreeView(szCategory, szSubCategory, szDisasterType, ar);


			//FormContentToolBar toolBar = new FormContentToolBar();
			//toolBar.TopMost = true;
			//toolBar.Show();
        }        

        private void FormMain_Shown(object sender, EventArgs e)
        {
            statusClockTimer_Tick(null, null);

            statusClockTimer.Interval = 1000;
            statusClockTimer.Enabled = true;
            statusClockTimer.Start();

            this.Focus();
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (contentForm.CheckModify())
            {
                DialogResult result = UnE.Utility.UMessageBox.Show("변경된 사항이 있습니다. 현재 시나리오를 저장하시겠습니까?", "저장 확인", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (FormMain.Instance.SaveSenarioFile())
                    {
                        DialogResult = DialogResult.None;
                    }
                    else
                    {
                        DialogResult = DialogResult.Cancel;
                        e.Cancel = true;
                        return;
                    }
                }
                else if (result == DialogResult.Cancel)
                {
                    DialogResult = DialogResult.Cancel;
                    e.Cancel = true;
                    return;
                }
            }
            DialogResult = DialogResult.None;
        }

        private void mUpdateTimer_Tick(object sender, EventArgs e)
        {
            foreach (RibbonButton rb in m_arRibbonButtons)
            {
                OnRibbonButtonUpdate(rb, rb.ID);
            }

            leftPaneToolStripMenuItem.Checked = !mSplitContent.Panel1Collapsed;

            if (contentForm.ContentOption == FormContent.ShowOption.Expression)
            {
                viewExprToolStripMenuItem.Checked = true;
                viewTextToolStripMenuItem.Checked = false;
                compOptionToolStripMenuItem.Checked = false;
            }
            else if(contentForm.ContentOption == FormContent.ShowOption.Text)
            {
                viewExprToolStripMenuItem.Checked = false;
                viewTextToolStripMenuItem.Checked = true;
                compOptionToolStripMenuItem.Checked = false;
            }
            else
            {
                viewExprToolStripMenuItem.Checked = false;
                viewTextToolStripMenuItem.Checked = false;
                compOptionToolStripMenuItem.Checked = true;
            }

            m_formTreeStep.SetEnabled = contentForm.Visible;
            componentForm.SetEnabled = contentForm.Visible;
            m_formProperties.SetEnabled = contentForm.Visible;
        }
        //////////////////////////////////////////////////////////////////////////////

        private void OnRibbonButtonUpdate(RibbonButton btn, int nID)
        {
            switch (nID)
            {
                case CommandID.EDIT_UNDO:
                    {
                        if (contentForm.Visible && UndoRedoManager.Instance.UndoCount > 0)
                            btn.Enabled = true;
                        else
                            btn.Enabled = false;
                    }
                    break;
                case CommandID.EDIT_REDO:
                    {
                        if (contentForm.Visible && UndoRedoManager.Instance.RedoCount > 0)
                            btn.Enabled = true;
                        else
                            btn.Enabled = false;
                    }
                    break;
                case CommandID.FILE_SYSTEM_VAR_OPEN:
                    btn.IsChecked = m_formSystemVariable.Visible;
                    break;
                case CommandID.FILE_USER_VAR_OPEN:
                    btn.IsChecked = m_formUserVariable.Visible;
               
                    break;
                case CommandID.FILE_ENUM_OPEN:
                    btn.IsChecked = m_formEnumeration.Visible;
                    break;
                case CommandID.VIEW_EXPR_HELP:
                    if (m_FormFrameHelp == null)
                    {
                        btn.IsChecked = false;
                    }
                    else
                        btn.IsChecked = m_FormFrameHelp.Visible;                    
                    break;

				case CommandID.SENARIO_VERIFY:
					if (contentForm.Visible)
						btn.Enabled = true;
					else
						btn.Enabled = false;
					break;
				case CommandID.SENARIO_SIMULATION:
					if (contentForm.Visible)
						btn.Enabled = true;
					else
						btn.Enabled = false;
					break;
            }
            btn.Refresh();
        }

        public void RunCommand(int nCommand)
        {
            switch (nCommand)
            {
                case CommandID.EDIT_REDO:
                    Redo();
                    break;
                case CommandID.EDIT_UNDO:
                    Undo();
                    break;
                case CommandID.FILE_SENARIO_SAVE:
                    {
                        SaveSenarioFile();
                    }
                    break;
                case CommandID.FILE_SENARIO_NEW:
                    {
                        NewSenario();
                    }
                    break;
                case CommandID.FILE_SENARIO_SAVEAS:
                    {
                        SaveAsSenarioFile();
                    }
                    break;
                case CommandID.FILE_SYSTEM_VAR_OPEN:
                    {
                        m_formSystemVariable.BringToFront();
                        m_formSystemVariable.Visible = true;
                        m_formUserVariable.Visible = false;
                        m_formEnumeration.Visible = false;  
                    }
                    break;
                case CommandID.FILE_USER_VAR_OPEN:
                    {
                        OpenUserVariableFile();
                    }
                    break;
                case CommandID.FILE_USER_VAR_SAVE:
                    {
                        SaveUserVariableFile();
                    }
                    break;
                case CommandID.FILE_ENUM_OPEN:
                    {
                        OpenUserEnumFile();                        
                    }
                    break;
                case CommandID.FILE_ENUM_SAVE:
                    {
                        SaveUserEnumFile();
                    }
                    break;
                case CommandID.FILE_SENARIO_OPEN:
                    {
                        OpenSenarioFile();
                    }
                    break;
                case CommandID.FILE_EXIT:
                    {
                        FormFrame.Instance.Close();
                    }
                    break;
				case CommandID.SENARIO_VERIFY:
					{
						using (SOPChecker sopChecker = new SOPChecker(false))
						{
							sopChecker.OnChecFailed += this.SectionExprCheckFailed;
							if (sopChecker.CheckExpression(SenarioManager.Instance, false))
							{
								UnE.Utility.UMessageBox.Show("시나리오 검증이 완료되었습니다.", "시나리오 검증", MessageBoxButtons.OK, MessageBoxIcon.Information);
							}
						}						
					}
					break;
				case CommandID.SENARIO_SIMULATION:
					if (m_FormRunScript.Visible == false)
					{
						m_FormRunScript.Visible = true;
					} 
					break;              
            }
        }


        public void CheckedChanged(int nCommand, bool bChecked)
        {
            switch (nCommand)
            {
                case CommandID.VIEW_EXPR:
                    {
                        if( bChecked == true)
                            contentForm.ContentOption = FormContent.ShowOption.Expression;
                    }
                    break;
                case CommandID.VIEW_OPTION:
                    {
                        if (bChecked == true)
                            contentForm.ContentOption = FormContent.ShowOption.Component;
                    }
                    break;
                case CommandID.VIEW_TEXT:
                    {
                        if (bChecked == true)
                            contentForm.ContentOption = FormContent.ShowOption.Text;
                    }
                    break;
                case CommandID.VIEW_LEFTPANE:
                    mSplitContent.Panel1Collapsed = !bChecked;
                    break;
                case CommandID.FILE_SYSTEM_VAR_OPEN:
                    {
                        if (bChecked == false)
                        {
                            m_formSystemVariable.ClearSelection();
                            SetHelpPage(Properties.Resources.Drawing);     
                        }
                        else
                        {
                            SetHelpPage(Properties.Resources.System);
                        }
                        
                        m_formSystemVariable.BringToFront();
                        m_formSystemVariable.Visible = bChecked;
                        contentForm.Visible = !bChecked;
                        m_formUserVariable.Visible = false;
                        m_formEnumeration.Visible = false;
                        
                       
                    }
                    break;
                case CommandID.FILE_USER_VAR_OPEN:
                    {
                        if (bChecked == false)
                        {
                            m_formUserVariable.ClearSelection();
                            SetHelpPage(Properties.Resources.Drawing);     
                        }
                        else
                        {
                            SetHelpPage(Properties.Resources.UserVariable);
                        }
                        
                        m_formUserVariable.BringToFront();
                        m_formUserVariable.Visible = bChecked;

                        contentForm.Visible = !bChecked;
                        m_formSystemVariable.Visible = false;
                        m_formEnumeration.Visible = false;
                        
                    }
                    break;
                case CommandID.FILE_ENUM_OPEN:
                    {
                        if (bChecked == false)
                        {
                            m_formEnumeration.ClearSelection();
                            SetHelpPage(Properties.Resources.Drawing);     
                        }
                        else
                        {
                            SetHelpPage(Properties.Resources.Enums);  
                        }
                      
                        m_formEnumeration.BringToFront();
                        m_formEnumeration.Visible = bChecked;
                        contentForm.Visible = !bChecked;
                        m_formSystemVariable.Visible = false;
                        m_formUserVariable.Visible = false;
                        
                    }
                    break;
                case CommandID.VIEW_EXPR_HELP:
                    {
                        if (m_FormFrameHelp == null)
                            CreateHelpForm();
                        else
                            m_FormFrameHelp.Visible = !m_FormFrameHelp.Visible;
                    }
                    break;
            }
        }

        private FormFrameHelp m_FormFrameHelp = null;
        private void CreateHelpForm()
        {
            if (m_FormFrameHelp == null)
            {
                m_FormFrameHelp = new FormFrameHelp(m_formhelp);
                m_FormFrameHelp.StartPosition = FormStartPosition.Manual;
                m_FormFrameHelp.Size = new Size(440, 730);

                Point p = this.PointToScreen(mPaneRibbonToolBar.Location);
                int nX = p.X + mPaneRibbonToolBar.Size.Width - m_FormFrameHelp.Size.Width;
                int nY = p.Y + mPaneRibbonToolBar.Size.Height + mMainMenuStrip.Size.Height + FormFrame.Instance.TitleBarHeight;
                m_FormFrameHelp.Location = new Point(nX - 10, nY + 70);
                m_FormFrameHelp.Show(this);
            }
        }

        public void CloseFromFrameHelp()
        {
            m_FormFrameHelp.Close();
            m_FormFrameHelp = null;
        }

        private bool SaveXML(string strFileName, out string szError)
        {
            szError = "";
            using (XMLManager mgr = new XMLManager())
            {
                string strVersionName = Path.GetFileNameWithoutExtension(strFileName);             

                SenarioManager manager = SenarioManager.Instance;
                manager.VersionName = strVersionName;
                if (!mgr.Save(manager, strFileName, strVersionName))
                {
                    szError = mgr.ErrorMessage;
                    
                    return false;
                }

                //LoadTreeView("PreSafe", "SOP", panel.DisasterType, panel.StepName);
            }
            return true;
        }

        public bool OpenUserVariableFile()
        {
            mOpenFileDialog.Title = "사용자 변수 열기";
            mOpenFileDialog.Filter = "사용자 변수 파일|*.xml";
            mOpenFileDialog.FileName = "";

            if (mOpenFileDialog.ShowDialog(FormFrame.Instance) == DialogResult.OK)
            {
                string strPath = mOpenFileDialog.FileName;

                XMLReader<UserVariable> reader = new XMLReader<UserVariable>();
                reader.ReadXML(strPath);
                SenarioManager.Instance.UserVariables = reader.Variables;

                m_formUserVariable.BringToFront();
                m_formUserVariable.Visible = true;
                m_formSystemVariable.Visible = false;
                m_formEnumeration.Visible = false;

                m_formUserVariable.UpdateUserVariable();

                SetStatusText("사용자 변수 파일이 로드 되었습니다.");
                return true;
            }

            return false;
        }

        public bool SaveUserVariableFile()
        {
            mSaveFileDialog.Title = "사용자 변수 저장";
            mSaveFileDialog.Filter = "사용자 변수 파일|*.xml";
            mSaveFileDialog.FileName = "";
            if (mSaveFileDialog.ShowDialog(FormFrame.Instance) == DialogResult.OK)
            {
                string strSavePath = mSaveFileDialog.FileName;

                //XML파일 Save
                XMLWriter<UserVariable> writer = new XMLWriter<UserVariable>();
                writer.Variables = SenarioManager.Instance.UserVariables;
                writer.SaveXML(strSavePath);
                return true;
            }
            return false;
        }

        public bool OpenUserEnumFile()
        {
            mOpenFileDialog.Title = "사용자 ENUM 열기";
            mOpenFileDialog.Filter = "사용자 ENUM 파일|*.xml";
            mOpenFileDialog.FileName = "";
            if (mOpenFileDialog.ShowDialog(FormFrame.Instance) == DialogResult.OK)
            {
                string strPath = mOpenFileDialog.FileName;

                XMLReader<Enums> reader = new XMLReader<Enums>();
                reader.ReadXML(strPath);
                SenarioManager.Instance.EnumList = reader.Variables;

                m_formEnumeration.BringToFront();
                m_formEnumeration.Visible = true;
                m_formSystemVariable.Visible = false;
                m_formUserVariable.Visible = false;

                m_formEnumeration.UpdateUserVariable();

                return true;
            }
            return false;
        }

        public bool SaveUserEnumFile()
        {
            mSaveFileDialog.Title = "사용자 ENUM 저장";
            mSaveFileDialog.Filter = "사용자 ENUM 파일|*.xml";
            mSaveFileDialog.FileName = "";
            if (mSaveFileDialog.ShowDialog(FormFrame.Instance) == DialogResult.OK)
            {
                string strSavePath = mSaveFileDialog.FileName;

                XMLWriter<Enums> writer = new XMLWriter<Enums>();
                writer.Variables = SenarioManager.Instance.EnumList;
                writer.SaveXML(strSavePath);
                
                return true;                          
            }
            return false;
        }

        public void NewSenario()
        { 
            if (contentForm.InitSectionPanel())
            {               
                FormNewScenario formNewScenario = new FormNewScenario();
                formNewScenario.StartPosition = FormStartPosition.CenterParent;
                if (formNewScenario.ShowDialog(this) == DialogResult.OK)
                {
                    string szSenarioName = formNewScenario.SenarioName;
                    int nSenarioType = formNewScenario.SenarioType;
                    
                    UndoRedoManager.Instance.Reset();

                    ClearSelection();
                   
                    contentForm.ClearData();
                    contentForm.SenarioTitle = szSenarioName;
                    contentForm.SenarioType = nSenarioType;
                    
                    SenarioManager.Instance.NewSenario(szSenarioName, nSenarioType);

                    // Set Tree
                    string szCategory = SenarioManager.Instance.Category;
                    string szSubCategory = SenarioManager.Instance.SubCategory;
                    string szDisasterType = SenarioManager.Instance.DisasterType;
                    ArrayList ar = SenarioManager.Instance.ActionStepList;
                    m_formTreeStep.SetTreeView(szCategory, szSubCategory, szDisasterType, ar);
                            
                }
            }
        }

        public bool OpenSenarioFile()
        {
            if (contentForm.InitSectionPanel())
            {
                mOpenFileDialog.Title = "시나리오 파일 열기";
                mOpenFileDialog.Filter = "시나리오 파일|*.xml";
                mOpenFileDialog.FileName = "";
                if (mOpenFileDialog.ShowDialog(FormFrame.Instance) == DialogResult.OK)
                {
                    string szOpenFilePath = mOpenFileDialog.FileName;

                    contentForm.ClearData();
                    UndoRedoManager.Instance.Reset();

                    //XML파일 Load
                    XMLManager mgr = new XMLManager();                  
                    if (mgr.Load(szOpenFilePath))
                    {
                        SenarioManager.Instance.SenarioFilePath = szOpenFilePath;
                        ClearSelection();
                        contentForm.ClearModify();

                        ArrayList arActionStep = SenarioManager.Instance.ActionStepList;
                        SenarioManager manager = SenarioManager.Instance;

                        string szFileName = Path.GetFileNameWithoutExtension(szOpenFilePath);
                        contentForm.SenarioTitle = szFileName;

                        foreach (ActionStep actionStep in arActionStep)
                        {
                            actionStep.StepName = contentForm.SenarioTitle;
                        }

                        this.m_formTreeStep.SetTreeView(manager.Category, manager.SubCategory, manager.DisasterType, arActionStep);


						//contentForm.ContentOption = FormContent.ShowOption.Text;
						contentForm.ContentOption = FormContent.ShowOption.Component;

                        return true;
                    }
                    else
                    {
                        string strError = mgr.ErrorMessage.Length == 0 ? "XML 불러오기가 실패하였습니다." : mgr.ErrorMessage;
                        SetStatusText(strError);
                        UnE.Utility.UMessageBox.Show(strError);
                     
                    }
                }
            }
                        
            return false;
        }

        public void SectionExprCheckFailed(SectionScriptFailEventArg e)
        {
            Sections.PanelSectionEx panel = (Sections.PanelSectionEx)e.Panel;
            Sections.Section section = e.Section;

            ActionStep actionStep = (ActionStep)panel.Tag;
            if (actionStep != null)
            {
                contentForm.ShowActionStep(actionStep);
                panel.ZoomSection(section);
            }   
            string szMessage = string.Format("수식오류가 발생하였습니다.\r\n확인후 저장하십시오.\r\n자세한 오류 사항 : {0}", e.Excpetion.Message);
            UnE.Utility.UMessageBox.Show(szMessage, "수식 검증 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);

            using (PopupNote form = new PopupNote(true))
            {
                if (section.GetComponentType() == Sections.Section.ComponentType.DECISION)
                {
                    Sections.SectionDataDecision data = (Sections.SectionDataDecision)section.Data;
                    form.Text = data.Expression;
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        data.Expression = form.Text;
                    }
                }
                if (section.GetComponentType() == Sections.Section.ComponentType.PROCESS)
                {
                    Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;
                    form.Text = data.Expression;
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        data.Expression = form.Text;
                    }
                }
                
            }
        }

        public bool SaveAsSenarioFile()
        {
            using (XMLManager mgr = new XMLManager())
            {
                using (SOPChecker checker = new SOPChecker(true))
                {
                    checker.OnChecFailed += SectionExprCheckFailed;
                    SenarioManager manager = SenarioManager.Instance;
                    if (!checker.CheckSOP(manager))
                    {
                        return false;
                    }
                }

                string szSaveFilePath = "";
                if (SenarioManager.Instance.SenarioFilePath != "")
                {
                    mSaveFileDialog.Title = "다른 이름으로 저장";
                    mSaveFileDialog.Filter = "시나리오 파일|*.xml";
                    mSaveFileDialog.FileName = "";
                    if (mSaveFileDialog.ShowDialog(FormFrame.Instance) == DialogResult.OK)
                    {
                        szSaveFilePath = mSaveFileDialog.FileName;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return SaveSenarioFile();
                }

                //XML파일 Save
                string szError = "";
                if (!SaveXML(szSaveFilePath, out szError))
                {
                    UnE.Utility.UMessageBox.Show(szError);
                    return false;
                }

                SenarioManager.Instance.SenarioFilePath = szSaveFilePath;

                string szFileName = Path.GetFileNameWithoutExtension(szSaveFilePath);
                FormMain.Instance.contentForm.SenarioTitle = szFileName;
                m_formTreeStep.SetActionStepName(FormMain.Instance.contentForm.SenarioTitle);

                ClearSelection();
                contentForm.ClearModify();
                UndoRedoManager.Instance.Reset();

                SetStatusText("저장이 완료되었습니다.");
                return true;
            }
        }
        public bool SaveSenarioFile()
        {
            using (XMLManager mgr = new XMLManager())
            {
                using(SOPChecker checker = new SOPChecker(true))
                {
                    checker.OnChecFailed += SectionExprCheckFailed;
                    SenarioManager manager = SenarioManager.Instance;
                    if (!checker.CheckSOP(manager))

                    {
                        return false;
                    }
                }                

                string szSaveFilePath = "";
                if (SenarioManager.Instance.SenarioFilePath == "")
                {
                    mSaveFileDialog.Title = "시나리오 파일 저장";
                    mSaveFileDialog.Filter = "시나리오 파일|*.xml";
                    mSaveFileDialog.FileName = "";
                    if (mSaveFileDialog.ShowDialog(FormFrame.Instance) == DialogResult.OK)
                    {
                        szSaveFilePath = mSaveFileDialog.FileName;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    szSaveFilePath = SenarioManager.Instance.SenarioFilePath;
                }

                //파일명
                string szFileName = Path.GetFileNameWithoutExtension(szSaveFilePath);
                //시나리오 이름
                string strSenarioName = FormMain.Instance.contentForm.SenarioTitle;
                if (String.Compare(strSenarioName, "새시나리오", true) == 0)
                {
                    FormMain.Instance.contentForm.SenarioTitle = szFileName;
                }

                string szError = "";
                if (!SaveXML(szSaveFilePath, out szError))
                {
                    UnE.Utility.UMessageBox.Show(szError);
                    FormMain.Instance.contentForm.SenarioTitle = strSenarioName;
                    return false;
                }

                SenarioManager.Instance.SenarioFilePath = szSaveFilePath;
                m_formTreeStep.SetActionStepName(FormMain.Instance.contentForm.SenarioTitle);

                ClearSelection();
                contentForm.ClearModify();

                UndoRedoManager.Instance.Reset();

                //m_formTreeStep.LoadTreeView(szFileName);

                SetStatusText("저장이 완료되었습니다.");
                return true;                
            }
        }

        private void ClearSelection()
        {
            m_formProperties.ClearSelection();
            contentForm.ClearSelectionComponent();
            componentForm.ClearSelection();
        }

		public void UpdateContentView()
		{
			contentForm.ContentOption = contentForm.ContentOption;
		}

        public void Undo()
        {
            UndoRedoManager.Instance.Undo();
            contentForm.RefreshContent();
			contentForm.ContentOption = contentForm.ContentOption;
            m_formProperties.ClearSelection();
        }

        public void Redo()
        {
            UndoRedoManager.Instance.Redo();
            contentForm.RefreshContent();
			contentForm.ContentOption = contentForm.ContentOption;
            m_formProperties.ClearSelection();
        }

        public void SetStatusText(string szText)
        {
            mStatusWork.Text = szText;
        }


        public void SetHelpPage(string szResourceName)
        {
            if (m_FormFrameHelp != null)
            {
                m_formhelp.SetPageLoad(szResourceName);
            }
        }

        public void OnCategoryNodeSelection(CategoryNode node)
        {
            SetStatusText("시나리오 대분류 : " + node.Text);
        }
        public void OnSubCategoryNodeSelection(SubCategoryNode node)
        {
            SetStatusText("시나리오 소분류 : " + node.Text);
        }
        public void OnDisasterNodeSelection(DisasterNode node)
        {
            SetStatusText("시나리오 유형 : " + node.DisasterName);
        }

        public void OnActionStepNodeSelection(ActionStepNode node)
        {
            SetStatusText("시나리오 : " + node.ActionStepName);
        }

        public void OnActionStepNodeDoubleClicked(ActionStepNode node)
        {
            if( node == null)
                return;

            //ActionStep 
            if( contentForm != null)
            {
                contentForm.ShowActionStep(node.ActionStep);
            }
        }


        public void OnChangeDisasterType(DisasterNode node)
        {
            UndoRedoManager.Instance.SaveSnapshot("시나리오 유형 변경");

            string szDisasterType = node.Text;
            SenarioManager.Instance.DisasterType = szDisasterType;
        }

        private void mCheckValidationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(SOPChecker sopChecker = new SOPChecker(false))
            {
                sopChecker.OnChecFailed += this.SectionExprCheckFailed;
                if( sopChecker.CheckExpression(SenarioManager.Instance, false))
                {
                    UnE.Utility.UMessageBox.Show("시나리오 검증이 완료되었습니다.", "시나리오 검증", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

        }

        private void SimulationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_FormRunScript.Visible == false)
            {
                m_FormRunScript.Visible = true;
            }    
        }      
          
    }
}
