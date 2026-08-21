using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XtremeCommandBars;


namespace UBMLViewer
{
    public partial class DockingCmdForm : Form
    {

        public System.Windows.Forms.ListBox HistoryList
        {
            get { return m_HistoryListBox; }
        }

        public System.Windows.Forms.TextBox CommandEdit
        {
            get { return m_CmdTextBox; }
        }

        private string szCmdStr = "";
        private bool bLogProcess = false;
        public DockingCmdForm()
        {
            InitializeComponent();
            
            m_CmdTextBox.Text = szCmdStr;
            m_LoggerTimer.Enabled = true;
            m_LoggerTimer.Start();
        }

        private void LoggerTimer_Tick(object sender, EventArgs e)
        {
            if (bLogProcess == false)
            {
                bLogProcess = true;
                PythonLogger _logger = ScriptProxy.Instance.Logger;
                List<PythonLogger.Entry> entries = _logger.GetAll();
                foreach (var entry in entries)
                {
                    m_HistoryListBox.Items.Insert(0, entry);
                }

                bLogProcess = false;
            }            
        }

        private void DockingLogForm_SizeChanged(object sender, EventArgs e)
        {
            int width = this.Size.Width;
            int height = this.Size.Height;

            m_HistoryListBox.SetBounds(0, 0, width, height - 12);
            m_CmdTextBox.SetBounds(0, height - 20, width, 12);
        }

        private void CommandText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                string cmdStr = m_CmdTextBox.Text;
                ScriptProxy.Instance.Call(cmdStr);
                m_CmdTextBox.Text = szCmdStr;
                m_CmdTextBox.SelectionStart = szCmdStr.Length;
                m_CmdTextBox.SelectionLength = 0;
            }
        }

        private void DockingCmdForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_LoggerTimer.Stop();
        }

        public bool SetCmdTxtEntryFunction()
        {
            if (m_HistoryListBox != null && m_HistoryListBox.Visible == true)
            {
                PythonLogger.Entry entry = (PythonLogger.Entry)m_HistoryListBox.SelectedItem;
                if (entry != null && entry.Tag != null)
                {
                    string szText = (string)entry.Tag;
                    m_CmdTextBox.Text = szText;
                    return true;
                }
            }   
            return false;
        }

        private void HistoryListBox_DoubleClick(object sender, EventArgs e)
        {
            SetCmdTxtEntryFunction();
        }

        private void m_HistoryListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
        }

        private void m_HistoryListBox_SelectedValueChanged(object sender, EventArgs e)
        {
        }

       
    }
}
