using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RestoreService;
using DBUtility;

namespace WinRestore
{
	public partial class Form1 : Form
	{
		private NetworkManager m_Network = null;
		private WebDBManager m_dbMgr = null;

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			m_dbMgr = new WebDBManager();
			m_Network = new NetworkManager(m_dbMgr);
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			m_Network.ReleaseThread();

			if (m_Network.RestoreThread != null)
			{
				m_Network.RestoreThread.Join();
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			m_Network.BeginRestore();
		}
	}
}
