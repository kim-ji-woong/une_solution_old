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
        // 기존에 존재하는 버전을 덮어쓰는 것인가?
        //private bool m_isUpdateVersion = false;

        private bool m_isRegular = true;
        private bool m_isNormal = true;

        private VersionInfo m_versionCurrent = null;

        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();

        public FormSaveVersion(WebDBManager dbMgr, int nSOPGenUserID, string strDisasterCategory, string strSubDisasterCategory, string strDisasterDetail, bool isRegular, bool isNormal, VersionInfo versionCurrent)
        {
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (checkNewVersion.Checked)
            {
                if (textVersion.Text == "")
                {
                    MessageBox.Show("저장할 버전명을 입력하세요");
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
                    MessageBox.Show("저장할 버전을 선택하세요");
                    return;
                }
                else
                {
                    DataGridViewRow row = dataGridView.Rows[dataGridView.SelectedCells[0].RowIndex];

                    int nOwnerID = (int)row.Cells[1].Tag;

                    if (nOwnerID != m_nSOPGenUserID)
                    {
                        MessageBox.Show("선택한 버전은 현재 사용자가 만든 버전이 아닙니다.\r\n버전 수정은 작성자만 가능합니다.");
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
            strFormat += "where DisasterID = (select ID from DisasterCategory where CategoryName = '{0}') and ";
            strFormat += "SubCategoryName = '{1}') and DisasterName = '{2}' and VersionID = version.ID and version.OwnerID = SOPGenUser.ID ";
            strFormat += "and version.isRegular = {3} and version.isNormal = {4} order by Version.CreateTime";

            string strSQL = string.Format(strFormat, m_strDisasterCategory, m_strSubDisasterCategory, m_strDisasterDetail, m_isRegular ? 1 : 0, m_isNormal ? 1 : 0);
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

        private void FormSaveVersion_MouseDown(object sender, MouseEventArgs e)
        {
            m_bLeftMouseDown = true;
            m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
        }

        private void FormSaveVersion_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {
                        Point ptCur = this.Location;
                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void FormSaveVersion_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void label3_MouseDown(object sender, MouseEventArgs e)
        {
            FormSaveVersion_MouseDown(sender, e);
        }

        private void label3_MouseMove(object sender, MouseEventArgs e)
        {
            FormSaveVersion_MouseMove(sender, e);
        }

        private void label3_MouseUp(object sender, MouseEventArgs e)
        {
            FormSaveVersion_MouseUp(sender, e);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            FormSaveVersion_MouseDown(sender, e);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            FormSaveVersion_MouseMove(sender, e);
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            FormSaveVersion_MouseUp(sender, e);
        }
    }
}
