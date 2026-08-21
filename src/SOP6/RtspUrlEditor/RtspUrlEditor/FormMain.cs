using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using System.Collections;

namespace RtspUrlEditor
{
    public partial class FormMain : Form
    {
        private const int NO_INDEX = 0;
        private const int NAME_INDEX = 1;
        private const int ZONE_INDEX = 2;
        private const int URL_INDEX = 3;

        private WebDBManager m_dbMgr = null;
        private DataManager m_dataMgr = new DataManager();

        private static FormMain m_instance = null;

        private FormZone m_frmZone = null;
        private List<CCTV> m_removeCCTVs = new List<CCTV>();

        public bool EditMode
        {
            get { return checkBoxEdit.Checked; }
        }

        public DataManager DataManager
        {
            get { return m_dataMgr; }
        }

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public FormMain()
        {
            m_instance = this;

            InitializeComponent();
            ReadSiteID();
        }

        private void ReadSiteID()
        {
            int nSiteID;
            Utility util = new Utility();
            string szSection = "Server Connection Info";
            string szText = util.getinivalue(szSection, "siteid");

            if (!int.TryParse(szText, out nSiteID))
            {
                nSiteID = 1;
            }

            m_dbMgr = new WebDBManager(nSiteID);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            if (m_dbMgr != null && m_dataMgr.ReadDatas(m_dbMgr))
            {
                AddGrid();
            }
        }

        private void AddGrid()
        {
            foreach (CCTV cctv in m_dataMgr.CCTVs)
            {
                int nRowIndex = gridCCTV.Rows.Add();

                if (nRowIndex < 0)
                    return;

                DataGridViewRow row = gridCCTV.Rows[nRowIndex];

                row.Cells[NO_INDEX].Value = cctv.ID;
                row.Cells[NAME_INDEX].Value = cctv.CCTVName;
                row.Cells[ZONE_INDEX].Value = cctv.Zone;
                row.Cells[URL_INDEX].Value = cctv.URL;

                row.Tag = cctv;
            }
        }

        private void checkBoxEdit_CheckedChanged(object sender, EventArgs e)
        {
            colName.ReadOnly = colURL.ReadOnly = !checkBoxEdit.Checked;
            btnAdd.Enabled = btnDelete.Enabled = btnSave.Enabled = btnZone.Enabled = btnEquipZoneCCTV.Enabled = checkBoxEdit.Checked;
        }

        private void btnZone_Click(object sender, EventArgs e)
        {
            if (m_frmZone == null || m_frmZone.IsDisposed)
            {
                m_frmZone = new FormZone(m_dataMgr.BuildingGroups, m_dataMgr.OutdoorZones);
            }

            m_frmZone.Show(this);
        }

        public void OnSelectZone(Zone zone)
        {
            if (EditMode)
            {
                if (gridCCTV.SelectedCells.Count == 0)
                    return;

                int nRowIndex = gridCCTV.SelectedCells[0].RowIndex;
                DataGridViewRow row = gridCCTV.Rows[nRowIndex];

                row.Cells[ZONE_INDEX].Value = zone;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int nSaveCount = 0;

            foreach (DataGridViewRow row in gridCCTV.Rows)
            {
                if (row.Tag == null)
                    continue;

                CCTV cctv = (CCTV)row.Tag;

                string strCameraName = row.Cells[NAME_INDEX].Value.ToString().Trim();
                Zone zone = (Zone)row.Cells[ZONE_INDEX].Value;
                string strURL = row.Cells[URL_INDEX].Value.ToString().Trim();

                if (cctv.CCTVName == strCameraName && cctv.Zone == zone && cctv.URL == strURL)
                    continue;

                if (UpdateCCTV(cctv, strCameraName, zone, strURL) == false)
                {
                    row.Selected = true;
                    MessageBox.Show("저장이 실패하였습니다.");
                    return;
                }

                nSaveCount++;
            }

            foreach (CCTV cctv in m_removeCCTVs)
            {
                if (DeleteCCTV(cctv) == false)
                {
                    MessageBox.Show("저장이 실패하였습니다.");
                    return;
                }
                else
                    nSaveCount++;
            }

            m_removeCCTVs.Clear();

            if (nSaveCount > 0)
                MessageBox.Show(string.Format("총 {0}개의 CCTV 정보가 변경되었습니다.", nSaveCount));
            else
                MessageBox.Show("변경된 데이터가 없습니다.");
        }

        private bool DeleteCCTV(CCTV cctv)
        {
            if (cctv.NewCCTV)
                return true;

            string strSQL = "Delete from CCTV where ID = " + cctv.ID.ToString();

            if (m_dbMgr.GetResultData(strSQL) != null)
            {
                m_dataMgr.DeleteCCTV(cctv);
                return true;
            }

            return false;
        }

        private bool UpdateCCTV(CCTV cctv, string strCameraName, Zone zone, string strURL)
        {
            if (strCameraName.Length == 0)
            {
                MessageBox.Show("CCTV 이름을 입력해야 합니다.");
                return false;
            }

            if (zone == null)
            {
                MessageBox.Show("Zone이 설정되지 않았습니다.");
                return false;
            }

            if (strURL.Length == 0)
            {
                MessageBox.Show("URL이 설정되지 않았습니다.");
                return false;
            }

            if (cctv.NewCCTV)
                return InsertCCTV(cctv, strCameraName, zone, strURL);

            int nZoneID = zone == null ? -1 : zone.ID;
            string strSQL = string.Format("Update CCTV set CameraName = '{0}', ZoneID = {1}, URL = '{2}' where ID = {3}", strCameraName, nZoneID, strURL, cctv.ID);

            if (m_dbMgr.GetResultData(strSQL) != null)
            {
                cctv.CCTVName = strCameraName;
                cctv.URL = strURL;
                cctv.Zone = zone;
                return true;
            }

            return false;
        }

        private bool InsertCCTV(CCTV cctv, string strCameraName, Zone zone, string strURL)
        {
            int nZoneID = zone == null ? -1 : zone.ID;
            int nIndoor = 0;

            if (zone != null)
            {
                if (zone.BuildingID < 0)
                    nIndoor = 0;
                else
                    nIndoor = 1;
            }

            string strSQL = "Insert into CCTV (ID, CameraName, IPAddr, Port, PositionName, X, Y, Z, ZoneID, IsIndoor, LOD, Description, HTTPPort, Type, Stream, Channel, UserID, Password, URL, ReversePTZ, BigURL, SmallURL) values (";
            strSQL += string.Format("{0}, '{1}', '', 554, '{1}', 0, 0, 0, {2}, {3}, 0, NULL, NULL, 'RTSP', NULL, NULL, NULL, NULL, '{4}', NULL, NULL, NULL)", cctv.ID, strCameraName, nZoneID, nIndoor, strURL);

            if (m_dbMgr.GetResultData(strSQL) == null)
                return false;

            cctv.CCTVName = strCameraName;
            cctv.Zone = zone;
            cctv.URL = strURL;
            cctv.NewCCTV = false;

            m_dataMgr.AddCCTV(cctv);
            return true;
        }

        private int GetMaxCCTVID()
        {
            string strSQL = "Select max(ID) from CCTV";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return -1;

            if (arrResult.Count == 0)
                return 0;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            if (id == null)
                return 0;

            int nRowCount = gridCCTV.Rows.Count;

            for (int i=nRowCount-1;i>=0;i--)
            {
                DataGridViewRow row = gridCCTV.Rows[i];

                if (row.IsNewRow)
                    continue;

                if (row.Cells[NO_INDEX].Value == null)
                    continue;

                int nCCTVID = (int)row.Cells[NO_INDEX].Value;

                if (nCCTVID > id.Data)
                {
                    // 추가된 CCTV가 이미 있으면 그 CCTV ID를 사용한다.
                    id.Data = nCCTVID;
                }

                break;
            }

            return id.Data;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int nRowIndex = gridCCTV.Rows.Add();
            DataGridViewRow row = gridCCTV.Rows[nRowIndex];

            int nID = GetMaxCCTVID() + 1;

            if (nID == 0)
            {
                gridCCTV.Rows.RemoveAt(nRowIndex);
                return;
            }

            row.Cells[NO_INDEX].Value = nID;
            row.Cells[NAME_INDEX].Value = "";
            row.Cells[URL_INDEX].Value = "";

            CCTV cctv = new CCTV();
            cctv.ID = nID;
            cctv.NewCCTV = true;

            row.Tag = cctv;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (gridCCTV.SelectedCells.Count == 0)
                return;

            int nRowIndex = gridCCTV.SelectedCells[0].RowIndex;
            CCTV cctv = (CCTV)gridCCTV.Rows[nRowIndex].Tag;

            gridCCTV.Rows.RemoveAt(nRowIndex);
            m_removeCCTVs.Add(cctv);
        }

        private void btnEquipZoneCCTV_Click(object sender, EventArgs e)
        {
            FormEquipZoneCCTV frm = new FormEquipZoneCCTV();
            frm.ShowDialog(this);
        }
    }
}
