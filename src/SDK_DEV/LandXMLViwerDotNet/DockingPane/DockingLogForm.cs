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
	public partial class DockingLogForm : Form
	{

		private Font mFont = new Font("Gulim", 9, FontStyle.Regular);
		private string m_szLogPlaneText = "";
		private string m_szLogRichText = "";

		public DockingLogForm()
		{
			InitializeComponent();

			m_szLogRichText = "";

			AddPythonFunction();
		}

		public void AddPythonFunction()
		{
			ScriptProxy proxy = ScriptProxy.Instance;
			proxy.UserObject.ClearLog = new Func<bool>(ClearLog);
			proxy.UserObject.AddLog = new Func<string, bool>(AddLog);
		}

		private void DockingLogForm_SizeChanged(object sender, EventArgs e)
		{
			int width = this.Size.Width;
			int height = this.Size.Height;

			m_LogTextBox.SetBounds(0, 0, width, height);
		}

		public void OnChangeTheme(int nID)
		{
			switch (nID)
			{
				case ID.ID_OPTIONS_STYLEBLACK:
					{
					}
					break;
				case ID.ID_OPTIONS_STYLEBLUE:
					{
					}
					break;
				case ID.ID_OPTIONS_STYLEAQUA:
					{
					}
					break;
				case ID.ID_OPTIONS_STYLESILVER:
					{
					}
					break;
				case ID.ID_OPTIONS_STYLEOFFCIE2010BLUE:
					{
					}
					break;
				case ID.ID_OPTIONS_STYLEOFFICE2010SILVER:
					{
					}
					break;
				case ID.ID_OPTIONS_STYLEOFFCIE2010BLACK:
					{
					}
					break;
				case ID.ID_OPTIONS_STYLESCENIC:
					{

					}
					break;
				default:
					break;
			};
		}
		

		public void CopyAll()
		{
			m_LogTextBox.SelectAll();
			m_LogTextBox.Copy();
			m_LogTextBox.DeselectAll();
		}

		public bool ClearLog()
		{
			m_szLogRichText = "";;
			m_szLogPlaneText = "";
			m_LogTextBox.Clear();
			return true;
		}		     

		public bool AddLog(string szMessage)
		{
			m_szLogPlaneText += szMessage;
			m_szLogPlaneText += Environment.NewLine;
			
			Color c = Color.Black;
			switch (szMessage[0])
			{
				case 'e':
				case 'E':
					c = Color.Red;
					break;
				case 'w':
				case 'W':
					c = Color.Orange;
					break;
				case 'i':
				case 'I':
					c = Color.Green;
					break;
				case 'd':
				case 'D':
					c = Color.Blue;
					break;
				default:
					c = Color.Black;
					break;
			}

			m_LogTextBox.SelectionFont = mFont;
			m_LogTextBox.SelectionColor = c;
			m_LogTextBox.SelectedText = szMessage + Environment.NewLine;

			m_szLogRichText = m_LogTextBox.Rtf;
			return true;
		}

		private void copyAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CopyAll();
		}

   

		private void DockingLogForm_MouseDown(object sender, MouseEventArgs e)
		{
			
		}

		private void m_LogTextBox_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				m_ContextMenuStrip.Show(m_LogTextBox, e.X, e.Y);
			}
		}

		private void deleteAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ClearLog();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}


	}

}
