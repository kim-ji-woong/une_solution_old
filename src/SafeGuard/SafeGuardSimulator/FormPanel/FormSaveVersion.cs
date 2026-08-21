using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using DBUtility;

namespace SOPManager
{
    public partial class FormSaveVersion : Form
    {
        private string m_strDisasterCategory = "";
        private string m_strSubDisasterCategory = "";
        private string m_strDisasterDetail = "";
        private WebDBManager m_dbMgr = null;
        private int m_nSOPGenUserID = -1;

        private string m_strVersionName = "";
        private int m_nVersionID = -1;
        private string m_strDescription = "";

        private bool m_isRegular = true;
        private bool m_isNormal = true;

        private VersionInfo m_versionCurrent = null;
        private bool m_bNewVersionOnly = false;

        private int m_nSiteID = 1;
        public FormSaveVersion(WebDBManager dbMgr, int nSOPGenUserID, string strDisasterCategory, string strSubDisasterCategory, string strDisasterDetail, bool isRegular, bool isNormal, VersionInfo versionCurrent)
        {
            m_nSiteID = FormMain.Instance.SiteID;

            InitializeComponent();

            m_dbMgr = dbMgr;
            m_strDisasterCategory = strDisasterCategory;
            m_strSubDisasterCategory = strSubDisasterCategory;
            m_strDisasterDetail = strDisasterDetail;
            m_nSOPGenUserID = nSOPGenUserID;

            m_isRegular = isRegular;
            m_isNormal = isNormal;

            m_versionCurrent = versionCurrent;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void SetNewVersionOnly()
        {
            m_bNewVersionOnly = true;

            checkNewVersion.AutoCheck = false;
            checkNewVersion.Checked = true;
            checkNewVersion.CheckState = CheckState.Checked;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (checkNewVersion.Checked || m_bNewVersionOnly == true)
            {
                if (textVersion.Text == "")
                {
                    UnE.Utility.UMessageBox.Show("저장할 버전명을 입력하세요", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    m_strVersionName = textVersion.Text;
                    m_nVersionID = -1;
                    m_strDescription = textDescription.Text;
                }
            }
            else
            {
                int nSelectedCount = dataGridView.SelectedCells.Count;

                if (nSelectedCount == 0)
                {
                    UnE.Utility.UMessageBox.Show("저장할 버전을 선택하세요", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    DataGridViewRow row = dataGridView.Rows[dataGridView.SelectedCells[0].RowIndex];
                    int nOwnerID = (int)row.Cells[1].Tag;
                    if (nOwnerID != m_nSOPGenUserID)
                    {
                        UnE.Utility.UMessageBox.Show("선택한 버전은 현재 사용자가 만든 버전이 아닙니다.\r\n버전 수정은 작성자만 가능합니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    m_strVersionName = row.Cells[0].Value.ToString();
                    m_nVersionID = (int)row.Cells[0].Tag;

                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void FormSaveVersion_Load(object sender, EventArgs e)
        {
            // 전체 버전을 읽어오는 것이 아니라 특정 재난 유형에 대한 버전만 불러온다.
            // 즉, 재난별로 버전을 별도로 관리한다.
            // 예 : 자연재해/태풍/매미 V1.0, 자연재해/태풍/루사 V1.0
            string strFormat = "select disaster.ID, disaster.VersionID, version.VersionName, version.OwnerID, SOPGenUser.UserID, version.CreateTime, version.LastAccessTime, version.Description, version.isRegular, version.isNormal ";
            strFormat += "from Disaster, Version, SOPGenUser ";
            strFormat += "where SubDisasterID = (Select ID from SubDisasterCategory ";
            strFormat += "where DisasterID = (select ID from DisasterCategory where CategoryName = '{0}' and SiteID = {1}) and ";
            strFormat += "SubCategoryName = '{2}') and DisasterName = '{3}' and VersionID = version.ID and version.OwnerID = SOPGenUser.ID ";
            strFormat += "and version.isRegular = {4} and version.isNormal = {5} order by Version.CreateTime";

            string strSQL = string.Format(strFormat, m_strDisasterCategory, m_nSiteID, m_strSubDisasterCategory, m_strDisasterDetail, m_isRegular ? 1 : 0, m_isNormal ? 1 : 0);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            AddData(arrResult);
        }

        private void AddData(ArrayList arrData)
        {
            int nDataCount = arrData.Count;
            DateTime nullTime = new DateTime();
            string strLastVersionName = "";

            for (int i = 0; i < nDataCount - 9; i += 10)
            {
                int nDisasterID = WebDBManager.GetIntField(arrData[i].ToString(), 0);
                int nVersionID = WebDBManager.GetIntField(arrData[i + 1].ToString(), 0);
                string strVersionName = WebDBManager.GetStringField(arrData[i + 2], "");
                int nOwnerID = WebDBManager.GetIntField(arrData[i + 3].ToString(), 0);
                string strOwnerID = WebDBManager.GetStringField(arrData[i + 4], "");
                DateTime dtCreate = WebDBManager.GetDateTimeField(arrData[i + 5], nullTime);
                DateTime dtLastAccess = WebDBManager.GetDateTimeField(arrData[i + 6], nullTime);
                string strDescription = WebDBManager.GetStringField(arrData[i + 7], "");
                bool isRegular = WebDBManager.GetIntField(arrData[i + 8].ToString(), 0) == 0 ? false : true;
                bool isNormal = WebDBManager.GetIntField(arrData[i + 9].ToString(), 0) == 0 ? false : true;

                if (strDescription == "null")
                    strDescription = "";

                strLastVersionName = AddRow(nDisasterID, nVersionID, strVersionName, nOwnerID, strOwnerID, dtCreate, dtLastAccess, strDescription);
            }

            string strNewVersionName = NewVersionName(strLastVersionName);
            textVersion.Text = strNewVersionName;

            // 새로운 버전일 경우는 CheckBox를 check 상태로 둔다.
            if (dataGridView.Rows.Count == 0)
                checkNewVersion.Checked = true;

            checkNewVersion_CheckedChanged(null, null);
            SelectDefaultVersion();
        }

        private void SelectDefaultVersion()
        {
            dataGridView.ClearSelection();
            if (m_versionCurrent == null)
            {
                // 현재 버전이 없을 경우 제일 아래쪽 버전을 선택
                int nRowCount = dataGridView.Rows.Count;
                if (nRowCount == 0) return;

                dataGridView.Rows[nRowCount - 1].Cells[0].Selected = true;
            }
            else
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (row.Cells[0].Tag != null && (int)row.Cells[0].Tag == m_versionCurrent.VersionID)
                    {
                        row.Cells[0].Selected = true;
                        return;
                    }
                }
            }
        }

        private string AddRow(int nDisasterID, int nVersionID, string strVersionName, int nOwnerID, string strOwnerID, DateTime dtCreate, DateTime dtLastAccess, string strDescription)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.Tag = nDisasterID;

            DataGridViewCell cell = new DataGridViewTextBoxCell();
            cell.Value = strVersionName;
            cell.Tag = nVersionID;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strOwnerID;
            cell.Tag = nOwnerID;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = dtCreate.ToLongDateString() + " " + dtCreate.ToLongTimeString();
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = dtLastAccess.ToLongDateString() + " " + dtLastAccess.ToLongTimeString();
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strDescription;
            row.Cells.Add(cell);

            dataGridView.Rows.Add(row);

            return strVersionName;
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

        private void checkNewVersion_CheckedChanged(object sender, EventArgs e)
        {
            if (checkNewVersion.Checked)
                textVersion.Enabled = true;
            else
                textVersion.Enabled = false;
        }

        public string VersionName
        {
            get { return m_strVersionName; }
        }

        public int VersionID
        {
            get { return m_nVersionID; }
        }

        public string Description
        {
            get { return m_strDescription; }
        }
    }
}
