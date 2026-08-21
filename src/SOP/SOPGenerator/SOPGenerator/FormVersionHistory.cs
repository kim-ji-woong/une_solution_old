using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SOPGen
{
    public partial class FormVersionHistory : Form
    {
        private ArrayList m_arrVersionData = new ArrayList();
        private VersionData m_versionData = null;
        private bool m_isNewBegin = true;
        private bool m_hideNewBegin = false;

        private string m_strSkinFolder;
        private ArrayList m_arrAllVersions = new ArrayList();

        public FormVersionHistory(bool hideNewBegin = false)
        {
            m_hideNewBegin = hideNewBegin;
            InitializeComponent();

            m_strSkinFolder = StylesPath();
            Skin_Load();
        }

        public void AddVersionData(string strVersionName, string strOwner, DateTime dtCreate, DateTime dtLastAccess, string strDescription)
        {
            VersionData data = new VersionData(strVersionName, strOwner, dtCreate, dtLastAccess, strDescription);
            m_arrVersionData.Add(data);
        }

        public void ClearVersionData()
        {
            m_arrVersionData.Clear();
        }

        public void AddAllVersions(string strVersionName, string strOwner, DateTime dtCreate, DateTime dtLastAccess, string strDescription)
        {
            VersionData data = new VersionData(strVersionName, strOwner, dtCreate, dtLastAccess, strDescription);
            m_arrAllVersions.Add(data);
        }

        private VersionData FindVersion(string strVersionName)
        {
            foreach (VersionData data in m_arrAllVersions)
            {
                if (data.VersionName == strVersionName)
                    return data;
            }

            return null;
        }

        private void FormVersionHistory_Load(object sender, EventArgs e)
        {
            foreach (VersionData data in m_arrVersionData)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = new DataGridViewTextBoxCell();

                cell.Value = data.VersionName;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = data.Owner;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = data.CreateTime.ToString();
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = data.LastAccessTime.ToString();
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = data.Description;
                gridRow.Cells.Add(cell);

                versionDataGrid.Rows.Add(gridRow);
            }

            if (m_hideNewBegin)
                checkBoxNewBegin.Hide();
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            m_isNewBegin = checkBoxNewBegin.Checked;

            if (!m_isNewBegin)
            {
                if (versionDataGrid.SelectedCells.Count == 0)
                {
                    MessageBox.Show("Grid에서 불러올 버전을 선택하거나, [빈 화면으로 시작] 옵션을 Check해 주시기 바랍니다.");
                    return;
                }

                int nRowIndex = versionDataGrid.SelectedCells[0].RowIndex;
                m_versionData = (VersionData)m_arrVersionData[nRowIndex];
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        public bool IsNewBegin()
        {
            return m_isNewBegin;
        }

        public VersionData GetVersionData()
        {
            return m_versionData;
        }

        private void versionDataGrid_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int nRowIndex = e.RowIndex;
            int nColIndex = e.ColumnIndex;

            m_versionData = (VersionData)m_arrVersionData[nRowIndex];
            m_isNewBegin = false;

            DialogResult = DialogResult.OK;
            Close();
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
    }
}
