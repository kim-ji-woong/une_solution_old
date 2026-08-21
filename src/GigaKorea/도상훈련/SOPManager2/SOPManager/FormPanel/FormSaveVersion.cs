using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using DBUtility2;
using SOPManager.FormPanel;

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
        private List<int> m_usingLevelIDs = null;

        public List<int> UsingLevelIDs
        {
            get { return m_usingLevelIDs; }
        }

        private int m_nSiteID = 1;
        public FormSaveVersion(WebDBManager dbMgr, int nSOPGenUserID, string strDisasterCategory, string strSubDisasterCategory, string strDisasterDetail, bool isRegular, bool isNormal, VersionInfo versionCurrent)
        {
            m_nSiteID = FormMain.Instance.SiteID;

            InitializeComponent();

            dataGridView.CellPainting += dataGridView_CellPainting;

            m_dbMgr = dbMgr;
            m_strDisasterCategory = strDisasterCategory;
            m_strSubDisasterCategory = strSubDisasterCategory;
            m_strDisasterDetail = strDisasterDetail;
            m_nSOPGenUserID = nSOPGenUserID;

            m_isRegular = isRegular;
            m_isNormal = isNormal;

            m_versionCurrent = versionCurrent;
            UpdateNewVersionImage();

            UpdateControlSize();

            btnSetLevelDisaster.Visible = FormMain.Instance.LevelDisasterOption == LevelDisasterOption.Use;

            if (FormMain.Instance.LevelDisasterOption == LevelDisasterOption.Use)
            {
                // 이전 버전에서 사용했던 Level ID와 동일하게 설정한다.
                // 이전 버전이 없을 경우 모든 Level ID를 선택한다.
                m_usingLevelIDs = SetDefaultLevelIDs(m_versionCurrent, m_dbMgr);
            }
        }

        // 이전 버전에서 사용했던 Level ID와 동일하게 설정한다.
        // 이전 버전이 없을 경우 모든 Level ID를 선택한다.
        public static List<int> SetDefaultLevelIDs(VersionInfo version, WebDBManager dbMgr)
        {
            if (version == null)
                return ReadUserLevels();

            string strSQL = "Select d.ID ";
            strSQL += "from Disaster as d, Version as v ";
            strSQL += "where d.VersionID = v.ID and DisasterName = (";
            strSQL += string.Format("Select DisasterName from Disaster where VersionID = {0}) order by v.LastAccessTime desc", version.VersionID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            int nDisasterID = -1;

            for (int i=0;i<nResultCount;i++)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());

                if (id == null)
                    continue;

                nDisasterID = id.Data;
                break;
            }

            if (nDisasterID < 0)
                return null;

            List<int> ids = new List<int>();

            strSQL = "Select LevelID from SOPGenLevelDisaster where DisasterID = " + nDisasterID.ToString();
            arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount;i++)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());

                if (id == null)
                    continue;

                ids.Add(id.Data);
            }

            return ids;
        }

        private static List<int> ReadUserLevels()
        {
            string strSQL = "Select ID, LevelName from SOPGenLevel";
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            List<int> ids = new List<int>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strLevelName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (id == null || strLevelName == null)
                    continue;

                ids.Add(id.Data);
            }

            return ids;
        }

        void dataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridView gdv = sender as DataGridView;
            if (gdv == null) return;

            foreach (DataGridViewRow row in gdv.Rows)            
                row.MinimumHeight = gdv.RowTemplate.Height;            
        }

        public void UpdateControlSize()
        {
            Double[] dWindowRate = FormMain.Instance.GetCurWindowRate();
            double WindowRateWidth = dWindowRate[0];
            double WindowRateHeight = dWindowRate[1];

            this.Size = new System.Drawing.Size((int)(this.Size.Width * WindowRateWidth), (int)(this.Size.Height * WindowRateHeight));

            FormMain.Instance.UpdateWindowRate(dataGridView, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(picNewVersion, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(label6, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(label1, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(textVersion, WindowRateWidth, WindowRateHeight);

            FormMain.Instance.UpdateWindowRate(label2, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(textDescription, WindowRateWidth, WindowRateHeight);

            FormMain.Instance.UpdateWindowRate(btnSave, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(btnCancel, WindowRateWidth, WindowRateHeight);
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
            if (checkNewVersion.Checked/* && m_bNewVersionOnly == true*/)
            {
                if (textVersion.Text == "")
                {
                    UnE.Utility.UMessageBoxRibbon.Show("저장할 버전명을 입력하세요", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                    UnE.Utility.UMessageBoxRibbon.Show("저장할 버전을 선택하세요", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    DataGridViewRow row = dataGridView.Rows[dataGridView.SelectedCells[0].RowIndex];
                    int nOwnerID = (int)row.Cells[1].Tag;
                    if (nOwnerID != m_nSOPGenUserID)
                    {
                        UnE.Utility.UMessageBoxRibbon.Show("선택한 버전은 현재 사용자가 만든 버전이 아닙니다.\r\n버전 수정은 작성자만 가능합니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    m_strVersionName = row.Cells[0].Value.ToString();
                    m_nVersionID = (int)row.Cells[0].Tag;

                    using (IOManager ioMgr = new IOManager())
                    {
                        if (ioMgr.IsMonitoringSOPVersion(m_dbMgr, m_nVersionID, null))
                        {
                            UnE.Utility.UMessageBoxRibbon.Show("선택한 버전은 현재 사용중인 버전입니다.\r\n사용중인 버전으로 저장할 수 없습니다.\r\n다른 버전을 선택 하세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
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
            string strFormat = "select Disaster.ID, Disaster.VersionID, Version.VersionName, Version.OwnerID, SOPGenUser.UserID, Version.CreateTime, Version.LastAccessTime, Version.Description, Version.isRegular, Version.isNormal ";
            strFormat += "from Disaster, Version, SOPGenUser ";
            strFormat += "where SubDisasterID = (Select ID from SubDisasterCategory ";
            strFormat += "where DisasterID = (select ID from DisasterCategory where CategoryName = '{0}' and SiteID = {1}) and ";
            strFormat += "SubCategoryName = '{2}') and DisasterName = '{3}' and VersionID = Version.ID and Version.OwnerID = SOPGenUser.ID ";
            strFormat += "and Version.isRegular = {4} and Version.isNormal = {5} order by Version.CreateTime";

            string strSQL = string.Format(strFormat, m_strDisasterCategory, m_nSiteID, m_strSubDisasterCategory, m_strDisasterDetail, m_isRegular ? 1 : 0, m_isNormal ? 1 : 0);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

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
            UpdateNewVersionImage();

            if (checkNewVersion.Checked)
                textVersion.Enabled = true;
            else
                textVersion.Enabled = false;
        }

        private void NewVersionCheck_Click(object sender, EventArgs e)
        {
            checkNewVersion.Checked = !checkNewVersion.Checked;
        }

        private void UpdateNewVersionImage()
        {
            if (checkNewVersion.Checked == true)
            {
                this.picNewVersion.BackgroundImage = global::SOPManager.Properties.Resources.__COMMON_ckb_enable;
            }
            else
            {
                this.picNewVersion.BackgroundImage = global::SOPManager.Properties.Resources.__COMMON_ckb_disable;
            }
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

        private void btnSetLevelDisaster_Click(object sender, EventArgs e)
        {
            List<int> usingLevelIDs = m_usingLevelIDs;

            UnE.GUI.DialogFormFrameRibbon frm = new UnE.GUI.DialogFormFrameRibbon(new FormSOPGenLevels(ref usingLevelIDs));
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Text = "등급별 속성 지정";
            frm.Sizable = false;
            frm.PictureBoxTitleImage = global::SOPManager.Properties.Resources.열기_normal;
            frm.ShowDialog();

            m_usingLevelIDs = usingLevelIDs;
        }
    }
}
