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
    public partial class FormVersion : Form
    {
        private ArrayList m_arrVersionData = new ArrayList();
        //private string m_strNewVersionName = "";
        //private string m_strNewVersionDescription = "";
        private VersionData m_newVersionData = null;
        private bool m_isNewVersion = true;

        private ArrayList m_arrAllVersions = new ArrayList();

        public FormVersion()
        {
            InitializeComponent();
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

        private void buttonSave_Click(object sender, EventArgs e)
        {
            //string strNewVersion = "";

            if (checkBoxNewVersion.Checked)
            {
                string strNewVersion = textBoxNewVersion.Text;
                strNewVersion = strNewVersion.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
                strNewVersion = strNewVersion.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });

                if (strNewVersion.Length == 0)
                {
                    MessageBox.Show("버전명을 입력하세요");
                    return;
                }

                VersionData existVersion = FindVersion(strNewVersion);

                if (existVersion != null)
                {
                    MessageBox.Show(string.Format("이미 존재하는 버전명입니다.\r\n{0}, 작성자({1}), 생성일자({2}), 수정일자({3}), 부가설명({4})", 
                        strNewVersion, existVersion.Owner, existVersion.CreateTime.ToString(), existVersion.LastAccessTime.ToString(), existVersion.Description), "버전 중복");
                    return;
                }

                DateTime dtNow = DateTime.Now;
                m_newVersionData = new VersionData(strNewVersion, null, dtNow, dtNow, textBoxDescription.Text);

                m_isNewVersion = true;
            }
            else
            {
                if (m_arrVersionData.Count == 0)
                {
                    MessageBox.Show("버전명을 입력하세요");
                    return;
                }

                if (versionDataGrid.SelectedCells.Count == 0)
                {
                    MessageBox.Show("Grid에서 저장할 버전을 선택해 주세요");
                    return;
                }

                int nRowIndex = versionDataGrid.SelectedCells[0].RowIndex;
                m_newVersionData = (VersionData)m_arrVersionData[nRowIndex];

                m_isNewVersion = false;
                //strNewVersion = versionDataGrid.Rows[nRowIndex].Cells[0].Value.ToString();
                //m_strNewVersionDescription = versionDataGrid.Rows[nRowIndex].Cells[4].Value.ToString();
            }

            //m_strNewVersionName = strNewVersion;

            DialogResult = DialogResult.OK;
            Close();
        }

        //public void GetNewVersion(out string strNewVersionName, out string strNewVersionDescription, out bool isNewVersion)
        public VersionData GetNewVersion(out bool isNewVersion)
        {
            //strNewVersionName = m_strNewVersionName;
            //strNewVersionDescription = m_strNewVersionDescription;
            isNewVersion = m_isNewVersion;
            return m_newVersionData;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void checkBoxNewVersion_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxNewVersion.Checked)
            {
                textBoxNewVersion.ReadOnly = false;
                textBoxDescription.ReadOnly = false;
            }
            else
            {
                textBoxNewVersion.ReadOnly = true;
                textBoxDescription.ReadOnly = true;
            }
        }

        private void FormVersion_Load(object sender, EventArgs e)
        {
            string strLastVersionName = "";

            foreach (VersionData data in m_arrVersionData)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                
                cell.Value = data.VersionName;
                gridRow.Cells.Add(cell);
                strLastVersionName = data.VersionName;

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

            if (m_arrVersionData.Count == 0)
            {
                checkBoxNewVersion.Checked = true;
                checkBoxNewVersion.Enabled = false;
                textBoxNewVersion.Text = "V1.0";
            }
            else
            {
                textBoxNewVersion.ReadOnly = true;
                textBoxDescription.ReadOnly = true;

                string strNewVersionName = NewVersionName(strLastVersionName);
                textBoxNewVersion.Text = strNewVersionName;
            }
        }

        private string NewVersionName(string strLastVersionName)
        {
            double num = 0.0;
            bool isDot = false;
            int nCount = 0, nCount2 = 0;

            string strHeader = "";
            int nLen = strLastVersionName.Length;

            for (int i = nLen - 1; i >= 0; i--)
            {
                char ch = strLastVersionName[i];

                if (char.IsDigit(ch))
                {
                    num += Math.Pow(10.0, nCount) * int.Parse(ch.ToString());
                    nCount++;
                }
                else if (ch == '.')
                {
                    if (isDot)
                    {
                        strHeader = strLastVersionName.Substring(0, i + 1);
                        break;
                    }
                    else
                        isDot = true;

                    if (nCount == 0)
                    {
                        strHeader = strLastVersionName.Substring(0, i + 1);
                        break;
                    }

                    num = num / Math.Pow(10.0, nCount);

                    // 소수점 아래 자리수
                    nCount2 = nCount;
                    nCount = 0;
                }
                else
                {
                    strHeader = strLastVersionName.Substring(0, i + 1);
                    break;
                }
            }

            if (nCount == 0 && nCount2 == 0)
                return "V1.0";

            if (nCount2 == 0)
                return string.Format("{0}{1}", strHeader, num + 1);

            string strFormat = "{0}{1:F" + nCount2.ToString() + "}";
            return string.Format(strFormat, strHeader, num + 1.0 / Math.Pow(10.0, nCount2));
        }
    }

    public class VersionData
    {
        private string m_strVersionName = "";
        private string m_strOwner = "";
        private DateTime m_dtCreate;
        private DateTime m_dtLastAccess;
        private string m_strDescription = "";

        public VersionData(string strVersionName, string strOwner, DateTime dtCreate, DateTime dtLastAccess, string strDescription)
        {
            m_strVersionName = strVersionName;
            m_strOwner = strOwner;
            m_dtCreate = dtCreate;
            m_dtLastAccess = dtLastAccess;
            m_strDescription = strDescription;
        }

        public string VersionName
        {
            get { return m_strVersionName; }
            set { m_strVersionName = value; }
        }

        public string Owner
        {
            get { return m_strOwner; }
            set { m_strOwner = value; }
        }

        public DateTime CreateTime
        {
            get { return m_dtCreate; }
            set { m_dtCreate = value; }
        }

        public DateTime LastAccessTime
        {
            get { return m_dtLastAccess; }
            set { m_dtLastAccess = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }
    }
}
