using System;
using System.Collections.Generic;
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

namespace PreSafe
{

    public partial class FormMain : Form, IMenuCommandOwner, IRibbonButtonOwner, IronPython.ITextCommanderOwner
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
                return false;
            }
            return true;
        }
        public void CreaetPythonContext()
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
        
        private static FormMain m_instance = null;
        public static FormMain Instance
        {
            get { return m_instance; }
        }
        
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
        }
      
        private void InitForm()
        {
            FormContent contentForm = new FormContent();
            contentForm.TopLevel = false;
            contentForm.Dock = DockStyle.Fill;
            mSplitContent.Panel2.Controls.Add(contentForm);
            contentForm.Show();
        }

        private void SetCommandID()
        {
            mRBtnUndo.Owner = this;
            mRBtnRedo.Owner = this;
            mRBtnSystemVar.Owner = this;
            mRBtnUserVar.Owner = this;
            mRBtnEnum.Owner = this;
            mRBtnHelp.Owner = this;

            mRBtnUndo.ID = CommandID.EDIT_UNDO;            
            mRBtnRedo.ID = CommandID.EDIT_REDO;
            mRBtnSystemVar.ID = CommandID.FILE_SYSTEM_VAR_OPEN;
            mRBtnUserVar.ID = CommandID.FILE_USER_VAR_OPEN;
            mRBtnEnum.ID = CommandID.FILE_ENUM_OPEN;
            mRBtnHelp.ID = CommandID.VIEW_EXPR_HELP;

            mOpenSennarioMenuItem.CommandID = CommandID.FILE_SENARIO_OPEN;
            mSaveSenarioMenuItem.CommandID = CommandID.FILE_SENARIO_OPEN;
            mOpenUserVarMenuItem.CommandID = CommandID.FILE_USER_VAR_OPEN;
            mSaveUserVarMenuItem.CommandID = CommandID.FILE_USER_VAR_SAVE;
            mOpenEnumMenuItem.CommandID = CommandID.FILE_ENUM_OPEN;           
            saveEnumToolstripMenuItem.CommandID = CommandID.FILE_ENUM_SAVE;
            exitToolStripMenuItem.CommandID = CommandID.FILE_EXIT;
            viewExprToolStripMenuItem.CommandID = CommandID.VIEW_EXPR;
            viewTextToolStripMenuItem.CommandID = CommandID.VIEW_TEXT;
            compOptionToolStripMenuItem.CommandID = CommandID.VIEW_OPTION;
            leftPaneToolStripMenuItem.CommandID = CommandID.VIEW_LEFTPANE;
            leftPaneToolStripMenuItem.CheckOnClick = true;
        }       
           

        public void RunCommand(int nCommand)
        {
            MessageBox.Show(nCommand.ToString());
        }

        public void CheckedChanged(int nCommand, bool bChecked)
        {
            if( nCommand == CommandID.VIEW_LEFTPANE)
            {
                if (bChecked == true)
                    mSplitContent.Panel1Collapsed = false;
                else
                    mSplitContent.Panel1Collapsed = true;
            }
            else
            {
                MessageBox.Show(nCommand.ToString() + bChecked.ToString());
            }
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
            DisposePythonContext();
           
            statusClockTimer.Stop();
            statusClockTimer.Enabled = false;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            CreaetPythonContext();

            InitForm();
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
 
    }
}
