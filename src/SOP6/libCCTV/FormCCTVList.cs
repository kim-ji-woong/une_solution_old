using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using DBUtility2;

namespace UnE.CCTV
{
	public partial class FormCCTVList : Form
	{
        public class NULL_CCTV_POI : POI
        {
            public override Facility.FacilityType Type
            {
                get { return Facility.FacilityType.CCTV; }
            }
        }

		private Dictionary<CCTV, Zone> m_dicCCTVs = new Dictionary<CCTV, Zone>();
        private Dictionary<int, ArrayList> m_dicPreset = new Dictionary<int, ArrayList>();

		private bool m_lastOutdoorOption;
		private bool m_lastIndoorOption;
		private string m_lastKey = "";

		private static bool m_showOutdoorCCTV = true;
		private static bool m_showIndoorCCTV = true;
		private static string m_keyPrev = "";

        private static POI NULL_POI = new NULL_CCTV_POI();

        private int m_nSiteID = 1;
        private int m_SelectedCCTV = -1;

		public FormCCTVList()
		{
            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

			InitializeComponent();

            SetDoubleBuffer(this.dataGridViewCCTVList, true);

            mTreeViewCCTV.Visible = false;
            mTreeViewCCTV.Nodes.Add("외부 영역", "외부 영역");
            RTSP_ONVIF.ONVIF_PTZ_Manager.Instance.GetPresetList(-1);
		}

        public static void SetDoubleBuffer(DataGridView gvView, bool bEnabled)
        {
            Type dgvType1 = gvView.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(gvView, bEnabled, null);
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
            //string strSQL = "Select id, CameraName, zoneID from CCTV";
            string szText = "SELECT cctv.id, cctv.CameraName, cctv.zoneID FROM CCTV AS cctv " +
                            " INNER JOIN zone AS z ON z.ID = cctv.ZoneID AND z.SiteID = {0} " +
                            " WHERE cctv.LOD > -1 "+
                            " ORDER BY cctv.ID";            

            string strSQL = string.Format(szText, m_nSiteID);

			ArrayList arrResult = dbMgr.GetResultData(strSQL);
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
                {
                    AddGridRow(cctv, zone);
                    AddTreeNode(cctv, zone);
                }

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

        private void AddTreeNode(CCTV cctv, Zone zone)
        {
            if( zone.Building != null)
            { 
                Building building = zone.Building;
                BuildingGroup group = building.BuildingGroup;
                if( group != null)
                {
                    TreeNode groupNode = null;
                    TreeNode [] nodes = mTreeViewCCTV.Nodes.Find(group.BuildingGroupName, false);
                    if(nodes == null || nodes.Length == 0)
                    {
                        groupNode = mTreeViewCCTV.Nodes.Add(group.BuildingGroupName, group.BuildingGroupName);
                    }
                    else
                    {
                        groupNode = nodes[0];
                    }

                    TreeNode buildingNode = null;
                    TreeNode[] bnodes = groupNode.Nodes.Find(building.BuildingName, false);
                    if (bnodes == null || bnodes.Length == 0)
                    {
                        buildingNode = groupNode.Nodes.Add(building.BuildingName, building.BuildingName);
                    }
                    else
                    {
                        buildingNode = bnodes[0];
                    }

                    TreeNode zoneNode = null;
                    TreeNode[] znodes = buildingNode.Nodes.Find(zone.DisplayText, false);
                    if (znodes == null || znodes.Length == 0)
                    {
                        zoneNode = buildingNode.Nodes.Add(zone.DisplayText, zone.DisplayText);
                    }
                    else
                    {
                        zoneNode = znodes[0];
                    }
                    zoneNode.Tag = zone;

                    TreeNode cctvNode = zoneNode.Nodes.Add(cctv.AccessKey);
                    cctvNode.Tag = cctv;                    
                }                
            }
            else
            {              
                TreeNode groupNode = null;
                TreeNode[] nodes = mTreeViewCCTV.Nodes.Find("외부 영역", false);
                if (nodes == null || nodes.Length == 0)
                {
                    groupNode = mTreeViewCCTV.Nodes.Add("외부 영역","외부 영역");
                }
                else
                {
                    groupNode = nodes[0];
                }

                //TreeNode buildingNode = null;
                //TreeNode[] bnodes = groupNode.Nodes.Find(building.BuildingName, false);
                //if (bnodes == null || bnodes.Length == 0)
                //{
                //    buildingNode = groupNode.Nodes.Add(building.BuildingName, building.BuildingName);
                //}
                //else
                //{
                //    buildingNode = bnodes[0];
                //}

                TreeNode zoneNode = null;
                TreeNode[] znodes = groupNode.Nodes.Find(zone.DisplayText, false);
                if (znodes == null || znodes.Length == 0)
                {
                    zoneNode = groupNode.Nodes.Add(zone.DisplayText, zone.DisplayText);
                }
                else
                {
                    zoneNode = znodes[0];
                }
                zoneNode.Tag = zone;

                TreeNode cctvNode = zoneNode.Nodes.Add(cctv.AccessKey);
                cctvNode.Tag = cctv;
                        
            }
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
            
            ArrayList PresetList = RTSP_ONVIF.ONVIF_PTZ_Manager.Instance.GetPresetList(cctv.ID);
            if (PresetList != null)
                m_dicPreset.Add(cctv.ID, PresetList);

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
			//FormMain.Instance.CCTVList = null;
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

        /*private void dataGridViewCCTVList_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;

            if (e.RowIndex < 0) return;

            Int32 icctvID = Int32.Parse(dataGridViewCCTVList.Rows[e.RowIndex].Cells[0].Value.ToString());

            if (m_dicPreset.ContainsKey(icctvID) == true)
            {
                ArrayList PresetList = m_dicPreset[icctvID];
                if (PresetList != null)
                {
                    if (cboPreset.Items.Count > 0)
                        cboPreset.Items.Clear();

                    for (Int32 index = 0; index < PresetList.Count; index++)
                        cboPreset.Items.Add(PresetList[index].ToString());

                    if (PresetList.Count > 0)
                        cboPreset.SelectedItem = PresetList[0].ToString();

                    cboPreset.Visible = true;
                    btnPresetMove.Visible = true;
                    m_SelectedCCTV = icctvID;
                }
            }
            else
            {
                cboPreset.Visible = false;
                btnPresetMove.Visible = false;
                m_SelectedCCTV = - 1;
            }

            if (e.ColumnIndex <= 2)
            {
                DoDragDrop(UnE.SOP.ProxySOP.Instance.SiteID + "," + dataGridViewCCTVList.Rows[e.RowIndex].Cells[0].Value, DragDropEffects.Copy);
            }
            else if (e.ColumnIndex == 3)
            {
                #region ComboBox 바로 펼치기

                DataGridViewRow dataGridViewRow = dataGridViewCCTVList.Rows[e.RowIndex];
                DataGridViewCell dataGridViewCell = dataGridViewRow.Cells[e.ColumnIndex];
                if (dataGridViewCell is DataGridViewComboBoxCell)
                {
                    dataGridViewCCTVList.CurrentCell = dataGridViewCell;
                    dataGridViewCCTVList.BeginEdit(true);

                    DataGridViewComboBoxEditingControl comboboxEdit = (DataGridViewComboBoxEditingControl)this.dataGridViewCCTVList.EditingControl;

                    if (comboboxEdit != null)
                    {
                        comboboxEdit.DroppedDown = true;
                    }
                }

                #endregion
            }
            else if (e.ColumnIndex == 4)
            {
                if (dataGridViewCCTVList.Rows[e.RowIndex].Cells[3].Value == null) return;

                String strPreset = dataGridViewCCTVList.Rows[e.RowIndex].Cells[3].EditedFormattedValue.ToString();//dataGridViewCCTVList.Rows[e.RowIndex].Cells[3].Value.ToString();
                if (strPreset.Length == 0) return;                

                Int32 iReturn = RTSP_ONVIF.ONVIF_PTZ_Manager.Instance.GoPreset(icctvID, strPreset);
                if( iReturn != 220)
                {
                    UnE.Utility.UMessageBox.Show("Preset 이동에 실패하였습니다.", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }*/

		private void dataGridViewCCTVList_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
		{
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (e.RowIndex < 0)
                    return;

                //if (e.ColumnIndex == 1 || e.ColumnIndex == 2)
                {
                    CCTV cctv = (CCTV)dataGridViewCCTVList.Rows[e.RowIndex].Tag;

                    if (cctv == null || cctv.POI == null)
                        return;

                    //Pipe
                    FormMain.Instance.PipeServer.Send("ZoomTarget("
                        + cctv.POI.X.ToString() + ","
                        + cctv.POI.Y.ToString() + ","
                        + cctv.POI.Z.ToString() + ","
                        + cctv.POI.IsIndoor.ToString() + ")");

                    FormMain.Instance.PipeServer.Send("SelectPOILoadZone(" + cctv.POI.ID + "," + cctv.POI.IsIndoor.ToString() + ")");
                    if (cctv.POI.IsIndoor)
                    {
                        FormMain.Instance.PipeServer.Send("IndoorRefresh");
                        //frmContent.IndoorView.Focus();
                        //frmContent.IndoorView.Refresh();
                    }
                    else
                    {
                        FormMain.Instance.PipeServer.Send("OutdoorRefresh");
                    }

                    //FormMain.Instance.PageHome.OnPostPickPOI(cctv.POI);

                    FormMain.Instance.SetCCTV(cctv);
                    this.Focus();
                }
            }
		}

		private void btnFind_Click(object sender, EventArgs e)
		{
			if (m_lastOutdoorOption == checkBoxShowOutdoor.Checked &&
				m_lastIndoorOption == checkBoxShowIndoor.Checked &&
				m_lastKey == textBoxDictionary.Text)
				return;

			dataGridViewCCTVList.Rows.Clear();
            mTreeViewCCTV.Nodes.Clear();
            if (checkBoxShowOutdoor.Checked == true)
            {
                mTreeViewCCTV.Nodes.Add("외부 영역", "외부 영역");
            }

			foreach (KeyValuePair<CCTV, Zone> pair in m_dicCCTVs)
			{
                if (CheckCondition(pair.Key, pair.Value, textBoxDictionary.Text, checkBoxShowOutdoor.Checked, checkBoxShowIndoor.Checked))
                {
                    AddGridRow(pair.Key, pair.Value);
                    AddTreeNode(pair.Key, pair.Value);
                }
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

        private void btnChangeView_Click(object sender, EventArgs e)
        {
            if( btnChangeView.Text == "그룹별 보기")
            {
                dataGridViewCCTVList.Visible = false;
                mTreeViewCCTV.Visible = true;
                mTreeViewCCTV.Dock = DockStyle.Bottom;
                mTreeViewCCTV.Size = dataGridViewCCTVList.Size;
                btnChangeView.Text = "목록 보기";
            }
            else
            {
                dataGridViewCCTVList.Visible = true;
                dataGridViewCCTVList.Dock = DockStyle.Bottom;
                mTreeViewCCTV.Visible = false;
                //dataGridViewCCTVList.Size = mTreeViewCCTV.Size;
                btnChangeView.Text = "그룹별 보기";
            }
        }

        private void mTreeViewCCTV_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            if( node != null)
            {
                if( node.Tag != null)
                {
                    object tag = node.Tag;
                    if( tag.GetType() == typeof(CCTV))
                    {
                        CCTV cctv = (CCTV)tag;
                        if (cctv == null || cctv.POI == null)
                            return;

						//Pipe
                        FormMain.Instance.PipeServer.Send("ZoomTarget("
                           + cctv.POI.X.ToString() + ","
                           + cctv.POI.Y.ToString() + ","
                           + cctv.POI.Z.ToString() + ","
                           + cctv.POI.IsIndoor.ToString() + ")");

                        FormMain.Instance.PipeServer.Send("SelectPOILoadZone(" + cctv.POI.ID + "," + cctv.POI.IsIndoor.ToString() + ")");

                        if (cctv.POI.IsIndoor)
                        {
                            FormMain.Instance.PipeServer.Send("IndoorRefresh");
                        }
                        else
                        {
                            FormMain.Instance.PipeServer.Send("OutdoorRefresh");
                        }

                        FormMain.Instance.SetCCTV(cctv);
                        this.Focus();
                    }
                }
            }
        }

        private void btnEmptyCCTV_Click(object sender, EventArgs e)
        {
            FormMain.Instance.SetCCTV(null);
            this.Focus();
        }

        private void cboPreset_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void btnPresetMove_Click(object sender, EventArgs e)
        {
            if (m_SelectedCCTV == -1 || cboPreset.SelectedItem.ToString() == "") return;

            Int32 iReturn = RTSP_ONVIF.ONVIF_PTZ_Manager.Instance.GoPreset(m_SelectedCCTV, cboPreset.SelectedItem.ToString());
            if (iReturn != 220)
            {
                MessageBox.Show("Preset 이동에 실패하였습니다.", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
	}
}