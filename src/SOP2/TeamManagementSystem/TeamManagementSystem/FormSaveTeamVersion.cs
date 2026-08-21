using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace TeamManagementSystem
{
    public partial class FormSaveTeamVersion : Form
    {
        private FormMain m_Main = null;
        private string m_strVersionName = "";
        private string m_strDescription = "";
        private int m_nNewVersionID = 1;

        public FormSaveTeamVersion(FormMain frmMain)
        {
            InitializeComponent();
            m_Main = frmMain;
        }

        private void FormSaveTeamVersion_Load(object sender, EventArgs e)
        {
            dataGridViewVersion.ReadOnly = true;
            string strLastVersionName = GetVersionInfo();

            string strDefaultVersionName = FormMain.NewVersionName(strLastVersionName);
            textBoxVersionName.Text = strDefaultVersionName;
        }

        private string GetVersionInfo()
        {
            string strLastVersionName = "";

            if (m_Main == null)
                return strLastVersionName;

            ArrayList arrVersion = m_Main.TeamVersion;
            if (arrVersion == null)
                return strLastVersionName;

            int nMaxVersionID = 0;

            dataGridViewVersion.Rows.Clear();
            foreach (Data_TeamVersion data in arrVersion)
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewCell cell = new DataGridViewTextBoxCell();

                cell.Value = data.VersionID;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = data.VersionName;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = data.UserName;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = data.CreateTime;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                if (data.Description == "null")
                    cell.Value = "";
                else
                    cell.Value = data.Description;
                row.Cells.Add(cell);

                dataGridViewVersion.Rows.Add(row);

                if (nMaxVersionID < data.VersionID)
                {
                    nMaxVersionID = data.VersionID;
                    strLastVersionName = data.VersionName;
                }
            }

            m_nNewVersionID = nMaxVersionID + 1;
            return strLastVersionName;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            m_strVersionName = textBoxVersionName.Text;
            m_strDescription = textBoxDescription.Text;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        public string VersionName
        {
            get { return m_strVersionName; }
            set { m_strVersionName = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public int VersionID
        {
            get { return m_nNewVersionID; }
            set { m_nNewVersionID = value; }
        }
    }
}
