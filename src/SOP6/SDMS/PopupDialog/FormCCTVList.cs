using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using DBUtility2;
using UnE.Spatial;
using UnE.Sensor;


namespace SDMS
{
	public partial class FormCCTVList : Form
	{
        public class NULL_CCTV_POI : POI
        {
            public override IFacility.FacilityType Type
            {
                get { return IFacility.FacilityType.CCTV; }
            }
        }

		private Dictionary<CCTV, Zone> m_dicCCTVs = new Dictionary<CCTV, Zone>();

		private bool m_lastOutdoorOption;
		private bool m_lastIndoorOption;
		private string m_lastKey = "";

		private static bool m_showOutdoorCCTV = true;
		private static bool m_showIndoorCCTV = true;
		private static string m_keyPrev = "";

        private static POI NULL_POI = new NULL_CCTV_POI();

        private int m_nSiteID = 1;
		public FormCCTVList()
		{          

            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

			InitializeComponent();

            this.DoubleBuffered = true;

            FormMain.SetDoubleBuffer(dataGridViewCCTVList, true);

            mTreeViewCCTV.Visible = false;
            mTreeViewCCTV.Nodes.Add("외부 영역", "외부 영역");
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

                UnE.View.Content.IFormContent frmContent = FormMain.Instance.PageHome.ContentForm;

				frmContent.ZoomTarget(cctv.POI.X, cctv.POI.Y, cctv.POI.Z, cctv.POI.IsIndoor);
                frmContent.SelectPOILoadZone(cctv.POI, cctv.POI.IsIndoor);

				if (cctv.POI.IsIndoor)
				{
					//frmContent.IndoorView.Focus();
                    UnE.View.Content.IBaseView view = (UnE.View.Content.IBaseView)frmContent.IndoorView;
                    view.Refresh();
				}
				else
				{
                    UnE.View.Content.IBaseView view = (UnE.View.Content.IBaseView)frmContent.OutdoorView;
					//frmContent.OutdoorView.Focus();
                    view.Refresh();
				}

				FormMain.Instance.PageHome.OnPostPickPOI(cctv.POI);
				this.Focus();
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

                        UnE.View.Content.IFormContent frmContent = FormMain.Instance.PageHome.ContentForm;

                        frmContent.ZoomTarget(cctv.POI.X, cctv.POI.Y, cctv.POI.Z, cctv.POI.IsIndoor);
                        frmContent.SelectPOILoadZone(cctv.POI, cctv.POI.IsIndoor);

                        if (cctv.POI.IsIndoor)
                        {
                            //frmContent.IndoorView.Focus();

                            UnE.View.Content.IBaseView view = (UnE.View.Content.IBaseView)frmContent.IndoorView;
                            view.Refresh();
                        }
                        else
                        {
                            //frmContent.OutdoorView.Focus();

                            UnE.View.Content.IBaseView view = (UnE.View.Content.IBaseView)frmContent.OutdoorView;
                            view.Refresh();
                        }

                        FormMain.Instance.PageHome.OnPostPickPOI(cctv.POI);
                        this.Focus();
                    }
                }
            }
        }

        private void btnEmptyCCTV_Click(object sender, EventArgs e)
        {
            FormMain.Instance.PageHome.OnPostPickPOI(NULL_POI);
            this.Focus();
        }
	}
}