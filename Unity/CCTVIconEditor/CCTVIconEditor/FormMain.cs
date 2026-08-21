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

namespace CCTVIconEditor
{
    public partial class FormMain : Form
    {
        private const int ID_INDEX = 0;
        private const int NAME_INDEX = 1;
        private const int POS_INDEX = 2;
        private const int X_INDEX = 3;
        private const int Y_INDEX = 4;
        private const int Z_INDEX = 5;

        private WebDBManager m_dbMgr = null;
        private Dictionary<int, Zone> m_dicZones = new Dictionary<int, Zone>();
        private Dictionary<int, CCTV> m_dicCCTVs = new Dictionary<int, CCTV>();
        private Dictionary<Zone, List<CCTV>> m_dicZoneCCTVs = new Dictionary<Zone, List<CCTV>>();

        private FormUnity m_frm = null;
        private bool m_systemInput = false;

        private static FormMain m_instance = null;

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public FormMain()
        {
            m_instance = this;
            InitializeComponent();

            string strSiteID = System.Configuration.ConfigurationManager.AppSettings.Get("siteid");
            string strURL = System.Configuration.ConfigurationManager.AppSettings.Get("url");
            string strDB = System.Configuration.ConfigurationManager.AppSettings.Get("db");

            int nSiteID;

            if (int.TryParse(strSiteID, out nSiteID))
            {
                string[] tokens = strDB.Split(',');

                if (tokens.Count() == 2)
                {
                    string strType = tokens[0].Trim();
                    string strDBName = tokens[1].Trim();

                    m_dbMgr = new WebDBManager(nSiteID);
                    m_dbMgr.WebServerURL = strURL;
                    m_dbMgr.DatabaseName = strDBName;

                    if (strType == "0")
                        m_dbMgr.DatabaseType = WebDBManager.DBType.sqlserver;
                    else if (strType == "1")
                        m_dbMgr.DatabaseType = WebDBManager.DBType.mysql;

                    LoadZone();
                    LoadCCTV();

                    if (cboScenes.Items.Count > 0)
                        cboScenes.SelectedIndex = 0;
                }
            }
        }

        private void LoadZone()
        {
            string strSQL = "Select z.ID, z.ZoneName, zs.SceneName from Zone as z, ZoneScene as zs where z.ID = zs.ZoneID and SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-2;i+=3)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strZoneName = WebDBManager.GetStringField(arrResult[i + 1]);
                string strSceneName = WebDBManager.GetStringField(arrResult[i + 2]);

                if (id == null || strZoneName == null)
                    continue;

                Zone zone = new Zone();
                zone.ID = id.Data;
                zone.ZoneName = strZoneName;
                zone.SceneName = strSceneName;

                m_dicZones[id.Data] = zone;
                cboScenes.Items.Add(zone);
            }
        }

        private void LoadCCTV()
        {
            string strSQL = "Select ID, CameraName, ZoneID, X, Y, Z from CCTV";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            Zone zone;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strCameraName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> zoneID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<float> x = WebDBManager.GetFloatField(arrResult[i + 3].ToString());
                VariousData<float> y = WebDBManager.GetFloatField(arrResult[i + 4].ToString());
                VariousData<float> z = WebDBManager.GetFloatField(arrResult[i + 5].ToString());

                if (id == null || strCameraName == null || zoneID == null || x == null || y == null || z == null)
                    continue;

                if (m_dicZones.TryGetValue(zoneID.Data, out zone))
                {
                    CCTV cctv = new CCTV();
                    cctv.ID = id.Data;
                    cctv.Name = strCameraName;
                    cctv.Zone = zone;
                    cctv.X = x.Data;
                    cctv.Y = y.Data;
                    cctv.Z = z.Data;

                    m_dicCCTVs[cctv.ID] = cctv;

                    List<CCTV> cctvs = null;

                    if (m_dicZoneCCTVs.TryGetValue(zone, out cctvs) == false)
                    {
                        cctvs = new List<CCTV>();
                        m_dicZoneCCTVs[zone] = cctvs;
                    }

                    cctvs.Add(cctv);
                }
            }
        }

        private void cboScenes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboScenes.SelectedItem == null)
                return;

            Zone zone = (Zone)cboScenes.SelectedItem;

            gridCCTV.Rows.Clear();

            List<CCTV> cctvs = null;
            
            if (m_dicZoneCCTVs.TryGetValue(zone, out cctvs))
            {
                foreach (CCTV cctv in cctvs)
                {
                    AddCCTVGrid(cctv);
                }
            }
        }

        private void AddCCTVGrid(CCTV cctv)
        {
            int nRowIndex = gridCCTV.Rows.Add();

            if (nRowIndex < 0)
                return;

            DataGridViewRow row = gridCCTV.Rows[nRowIndex];
            row.Tag = cctv;

            row.Cells[ID_INDEX].Value = cctv.ID;
            row.Cells[NAME_INDEX].Value = cctv.Name;
            row.Cells[POS_INDEX].Value = cctv.Zone.ZoneName;
            row.Cells[X_INDEX].Value = GetFloatString(cctv.X);
            row.Cells[Y_INDEX].Value = GetFloatString(cctv.Y);
            row.Cells[Z_INDEX].Value = GetFloatString(cctv.Z);
        }

        private string GetFloatString(float value)
        {
            return string.Format("{0:F1}", value);
        }

        private void btnRunUnity_Click(object sender, EventArgs e)
        {
            m_frm = new FormUnity();
            m_frm.Show();
        }

        private void btnChangeScene_Click(object sender, EventArgs e)
        {
            if (cboScenes.SelectedIndex < 0 || m_frm == null)
                return;

            Zone zone = (Zone)cboScenes.Items[cboScenes.SelectedIndex];
            m_frm.SelectScene(zone.SceneName);

            List<CCTV> cctvs;

            if (m_dicZoneCCTVs.TryGetValue(zone, out cctvs))
            {
                m_frm.AddPOIFile(cctvs);
            }
        }

        public void OnPOIMoved(int nID, float x, float y, float z)
        {
            m_systemInput = true;

            foreach (DataGridViewRow row in gridCCTV.Rows)
            {
                CCTV cctv = (CCTV)row.Tag;

                if (cctv == null)
                    continue;

                if (cctv.ID == nID)
                {
                    OnCCTVMoved(row, cctv, x, y, z);
                    break;
                }
            }

            m_systemInput = false;
        }

        private void OnCCTVMoved(DataGridViewRow row, CCTV cctv, float x, float y, float z)
        {
            string strSQL = string.Format("Update CCTV set X = {0}, Z = {1} where ID = {2}", x, z, cctv.ID);

            if (m_dbMgr.GetResultData(strSQL) != null)
            {
                row.Cells[X_INDEX].Value = GetFloatString(x);
                //row.Cells[Y_INDEX].Value = GetFloatString(x);
                row.Cells[Z_INDEX].Value = GetFloatString(x);
                row.Cells[0].Selected = true;

                cctv.X = x;
                cctv.Z = z;
            }
        }

        private void checkBoxEdit_CheckedChanged(object sender, EventArgs e)
        {
            colX.ReadOnly = colY.ReadOnly = colZ.ReadOnly = !checkBoxEdit.Checked;
            btnSave.Enabled = checkBoxEdit.Checked;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (m_dbMgr == null)
                return;

            foreach (DataGridViewRow row in gridCCTV.Rows)
            {
                if (row.IsNewRow)
                    continue;

                CCTV cctv = (CCTV)row.Tag;
                float x, y, z;

                if (GetValue(row, X_INDEX, out x) == false)
                    continue;
                if (GetValue(row, Y_INDEX, out y) == false)
                    continue;
                if (GetValue(row, Z_INDEX, out z) == false)
                    continue;

                string strSQL = string.Format("Update CCTV set X = {0}, Y = {1}, Z = {2} where ID = {3}", x, y, z, cctv.ID);

                if (m_dbMgr.GetResultData(strSQL) == null)
                {
                    MessageBox.Show("DB와의 접속이 끊어졌습니다.");
                    return;
                }
            }

            MessageBox.Show("변경된 내용이 저장되었습니다.");
        }

        private bool GetValue(DataGridViewRow row, int nColumnIndex, out float value)
        {
            value = 0.0f;

            if (row.Cells[nColumnIndex].Value == null)
                return false;

            if (float.TryParse(row.Cells[nColumnIndex].Value.ToString().Trim(), out value))
                return true;

            return false;
        }

        private void gridCCTV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (m_systemInput)
                return;

            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = gridCCTV.Rows[e.RowIndex];

            if (row.IsNewRow)
                return;

            CCTV cctv = (CCTV)row.Tag;

            if (m_frm != null)
                m_frm.SelectCCTV(cctv);
        }
    }
}
