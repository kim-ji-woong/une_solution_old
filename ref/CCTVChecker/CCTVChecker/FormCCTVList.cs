using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DBUtility;
using System.Collections;

namespace CCTVChecker
{
    public partial class FormCCTVList : Form
    {
        private Dictionary<CCTV, Zone> m_dicCCTVs = new Dictionary<CCTV, Zone>();

        private bool m_lastOutdoorOption;
        private bool m_lastIndoorOption;
        private string m_lastKey = "";

        private static bool m_showOutdoorCCTV = true;
        private static bool m_showIndoorCCTV = true;
        private static string m_keyPrev = "";

        public FormCCTVList()
        {
            InitializeComponent();
        }

        private void FormCCTVList_Load(object sender, EventArgs e)
        {
            checkBoxShowIndoor.Checked = m_showIndoorCCTV;
            checkBoxShowOutdoor.Checked = m_showOutdoorCCTV;
            textBoxDictionary.Text = m_keyPrev;

            m_lastOutdoorOption = checkBoxShowOutdoor.Checked;
            m_lastIndoorOption = checkBoxShowIndoor.Checked;
            m_lastKey = textBoxDictionary.Text;

            WebDBManager dbMgr = FormMain.Instance.DBManager;
            string strSQL = "Select id, CameraName, zoneID from CCTV";

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strCameraName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nZoneID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                
                Zone zone = ZoneManager.Instance.GetZone(nZoneID);

                if (zone == null)
                    continue;

                CCTV cctv = CCTVManager.Instance.GetCCTV(nID);
                if (cctv == null)
                    continue;

                m_dicCCTVs[cctv] = zone;

                if (CheckCondition(cctv, zone, textBoxDictionary.Text, checkBoxShowOutdoor.Checked, checkBoxShowIndoor.Checked))
                    AddGridRow(cctv, zone);

                textBoxDictionary.AutoCompleteCustomSource.Add(strCameraName);
            }
        }

        private bool CheckCondition(CCTV cctv, Zone zone, string strKey, bool showOutdoor, bool showIndoor)
        {
            if (cctv.POI == null)
            {
                if (showOutdoor && showIndoor)
                    return true;
                else
                    return false;
            }

            if (showOutdoor && !showIndoor)
            {
                if (cctv.POI.IsIndoor)
                    return false;
            }
            else if (!showOutdoor && showIndoor)
            {
                if (!cctv.POI.IsIndoor)
                    return false;
            }
            else if (!showOutdoor && !showIndoor)
                return false;

            if (strKey.Length > 0)
            {
                if (cctv.AccessKey.IndexOf(strKey, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    return true;

                string strPosition = GetCCTVPositionName(cctv, zone);
                if (strPosition.IndexOf(strKey, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    return true;

                return false;
            }

            return true;
        }

        private void AddGridRow(CCTV cctv, Zone zone)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = cctv.ID;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = cctv.AccessKey;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = GetCCTVPositionName(cctv, zone);
            row.Cells.Add(cell);

            row.Tag = cctv;
            dataGridViewCCTVList.Rows.Add(row);
        }

        private string GetCCTVPositionName(CCTV cctv, Zone zone)
        {
            if (cctv.POI == null)
                return zone.ZoneName;

            return cctv.POI.IsIndoor ? zone.ZoneName + "(실내)" : zone.ZoneName + "(외부)";
        }

        private void FormCCTVList_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormMain.Instance.CCTVList = null;
        }

        public void SelectCCTV(int nCCTVID)
        {
            foreach (DataGridViewRow row in dataGridViewCCTVList.Rows)
            {
                if ((int)row.Cells[0].Value == nCCTVID)
                {
                    row.Cells[0].Selected = true;
                    return;
                }
            }
        }

        private void dataGridViewCCTVList_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (e.RowIndex < 0)
                    return;

                CCTV cctv = (CCTV)dataGridViewCCTVList.Rows[e.RowIndex].Tag;

                if (cctv == null || cctv.POI == null)
                    return;

                FormMain.Instance.MultiCCTV.SetCCTV(cctv);
                this.Focus();

                /*FormContent frmContent = FormMain.Instance.PageHome.ContentForm;

                frmContent.ZoomTarget(cctv.POI.X, cctv.POI.Y, cctv.POI.Z, cctv.POI.IsIndoor);
                frmContent.SelectPOI(cctv.POI, cctv.POI.IsIndoor);

                if (cctv.POI.IsIndoor)
                {
                    //frmContent.IndoorView.Focus();
                    frmContent.IndoorView.Refresh();
                }
                else
                {
                    //frmContent.OutdoorView.Focus();
                    frmContent.OutdoorView.Refresh();
                }

                FormMain.Instance.PageHome.OnPostPickPOI(cctv.POI);
                this.Focus();*/
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (m_lastOutdoorOption == checkBoxShowOutdoor.Checked &&
                m_lastIndoorOption == checkBoxShowIndoor.Checked &&
                m_lastKey == textBoxDictionary.Text)
                return;

            dataGridViewCCTVList.Rows.Clear();

            foreach (KeyValuePair<CCTV, Zone> pair in m_dicCCTVs)
            {
                if (CheckCondition(pair.Key, pair.Value, textBoxDictionary.Text, checkBoxShowOutdoor.Checked, checkBoxShowIndoor.Checked))
                    AddGridRow(pair.Key, pair.Value);
            }

            m_lastOutdoorOption = checkBoxShowOutdoor.Checked;
            m_lastIndoorOption = checkBoxShowIndoor.Checked;
            m_lastKey = textBoxDictionary.Text;

            if (!textBoxDictionary.AutoCompleteCustomSource.Contains(m_lastKey))
                textBoxDictionary.AutoCompleteCustomSource.Add(m_lastKey);
        }

        private void checkBoxShowOutdoor_CheckedChanged(object sender, EventArgs e)
        {
            m_showOutdoorCCTV = checkBoxShowOutdoor.Checked;
            btnFind_Click(null, null);
        }

        private void checkBoxShowIndoor_CheckedChanged(object sender, EventArgs e)
        {
            m_showIndoorCCTV = checkBoxShowIndoor.Checked;
            btnFind_Click(null, null);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFind_Click(null, null);
            }
        }
    }
}
