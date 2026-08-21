using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using DBUtility;
using UnE.Spatial;
using UnE.Sensor;
using UnE.View.Content;
using System.Drawing;
using SDMS.Help;


namespace SDMS.PopupDialog
{
    public partial class FormCCTVList : PopupFormBase, IChildControl
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

        public static int nOrgFrmWidth = 626;

        private ManualManager m_manualManager = null;

		public FormCCTVList()
		{    
            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

			InitializeComponent();

            this.DoubleBuffered = true;

            FormMain.SetDoubleBuffer(dataGridViewCCTVList, true);

            InitCtrlSize(this);
            btnChangeView.ImageNormal = imgViewGroupDefault;
            btnChangeView.ImageClicked = imgViewGroupClick;
            btnChangeView.ImageMouseOver = imgViewGroupClick;
 
            mTreeViewCCTV.Visible = false;
            mTreeViewCCTV.Nodes.Add("외부 영역", "외부 영역");

            //dataGridViewCCTVList.MouseWheel += dataGridViewCCTVList_MouseWheel;
            this.MouseWheel += dataGridViewCCTVList_MouseWheel; 
            this.GotFocus += FormCCTVList_GotFocus;

            m_manualManager = new ManualManager(this);
            SetManualID();
		}

        public void SetSize(int height)
        {
            SetChildCtrlResize(this, 626, height);
            FormMain.Instance.CustomizeGridView(dataGridViewCCTVList);

            float sizePer = 1f;
            if (FormMain.Instance.Resolution == Resolution.FullHD)
            {
                sizePer = 0.5f;
            }
            else if (FormMain.Instance.Resolution == Resolution.Other)
            {
                sizePer = 0.75f;
            }

            mTreeViewCCTV.Font = new System.Drawing.Font(Program.prgFont, 18F * sizePer, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129))); 
        } 

        void dataGridViewCCTVList_MouseWheel(object sender, MouseEventArgs e)
        {
            int currentIndex = this.dataGridViewCCTVList.FirstDisplayedScrollingRowIndex;
            int scrollLines = SystemInformation.MouseWheelScrollLines;

            if (e.Delta > 0)
            {
                this.dataGridViewCCTVList.FirstDisplayedScrollingRowIndex
                    = Math.Max(0, currentIndex - scrollLines);
            }
            else if (e.Delta < 0)
            {
                this.dataGridViewCCTVList.FirstDisplayedScrollingRowIndex
                    = currentIndex + scrollLines;
            }
        }

        private bool m_bLoadData = false;

        public void LoadData()
        {
            if (m_bLoadData == true)
                return;

            textBoxDictionary.Text = m_keyPrev;
            m_lastKey = textBoxDictionary.Text;

            WebDBManager dbMgr = FormMain.Instance.DBManager;

            string szText = "SELECT cctv.id, cctv.CameraName, cctv.zoneID FROM CCTV AS cctv " +
                            " INNER JOIN zone AS z ON z.ID = cctv.ZoneID AND z.SiteID = {0} " +
                            " ORDER BY cctv.ID";

            string strSQL = string.Format(szText, m_nSiteID);

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

                if (CheckCondition(cctv, zone, textBoxDictionary.Text, true, false))
                {
                    AddGridRow(cctv, zone);
                    AddTreeNode(cctv, zone);
                }
                textBoxDictionary.AutoCompleteCustomSource.Add(strCameraName);
            }
            m_bLoadData = true;
        }

		private void FormCCTVList_Load(object sender, EventArgs e)
		{
            LoadData();
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

        private void dataGridViewCCTVList_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (e.RowIndex < 0)
                    return;

                CCTV cctv = (CCTV)dataGridViewCCTVList.Rows[e.RowIndex].Tag;

                if (cctv == null || cctv.POI == null)
                    return;

                IFormContent frmContent = FormMain.Instance.PageHome.ContentForm;

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
                    view.Refresh();
                }

                FormMain.Instance.PageHome.OnPostPickPOI(cctv.POI);

                dataGridViewCCTVList.Focus();

            }
            //this.paneSearch.Focus();
            this.Focus();
            this.Activate();
            SetFocusUser();
        }

		private void dataGridViewCCTVList_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
		{            
		}

		private void btnFind_Click(object sender, EventArgs e)
		{
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;

            //if (m_lastOutdoorOption == checkBoxShowOutdoor.Checked &&
            //    m_lastIndoorOption == checkBoxShowIndoor.Checked &&
            //    m_lastKey == textBoxDictionary.Text)
            //    return;

			dataGridViewCCTVList.Rows.Clear();
            mTreeViewCCTV.Nodes.Clear();
            //if (checkBoxShowOutdoor.Checked == true)
            //{
                mTreeViewCCTV.Nodes.Add("외부 영역", "외부 영역");
            //}


			foreach (KeyValuePair<CCTV, Zone> pair in m_dicCCTVs)
			{
                if (CheckCondition(pair.Key, pair.Value, textBoxDictionary.Text, true,false))
                {
                    AddGridRow(pair.Key, pair.Value);
                    AddTreeNode(pair.Key, pair.Value);
                }
			}

            mTreeViewCCTV.ExpandAll();

			//m_lastOutdoorOption = checkBoxShowOutdoor.Checked;
			//m_lastIndoorOption = checkBoxShowIndoor.Checked;
			m_lastKey = textBoxDictionary.Text;

			if (!textBoxDictionary.AutoCompleteCustomSource.Contains(m_lastKey))
				textBoxDictionary.AutoCompleteCustomSource.Add(m_lastKey);
		}

		private void checkBoxShowOutdoor_CheckedChanged(object sender, EventArgs e)
		{
			//m_showOutdoorCCTV = checkBoxShowOutdoor.Checked;
			btnFind_Click(null, null);
		}

		private void checkBoxShowIndoor_CheckedChanged(object sender, EventArgs e)
		{
			//m_showIndoorCCTV = checkBoxShowIndoor.Checked;
			btnFind_Click(null, null);
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				btnFind_Click(null, null);
			}
		}

        private Image imgViewGroupDefault = global::SDMS.Properties.Resources.CCTVList_ChangeViewGroup_Default;
        private Image imgViewGroupClick = global::SDMS.Properties.Resources.CCTVList_ChangeViewGroup_Click;
        private Image imgViewListDefault = global::SDMS.Properties.Resources.CCTVList_ChangeViewList_Default;
        private Image imgViewListClick = global::SDMS.Properties.Resources.CCTVList_ChangeViewList_Click;

        private void btnChangeView_Click(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;

            if (btnChangeView.ImageNormal == imgViewGroupDefault)
            {
                btnChangeView.ImageNormal = imgViewListDefault;
                btnChangeView.ImageClicked = imgViewListClick;
                btnChangeView.ImageMouseOver = imgViewListClick;

                dataGridViewCCTVList.Visible = false;
                mTreeViewCCTV.Visible = true;
                mTreeViewCCTV.Location = dataGridViewCCTVList.Location;
                mTreeViewCCTV.Anchor = dataGridViewCCTVList.Anchor;
                mTreeViewCCTV.Size = dataGridViewCCTVList.Size;
                //btnChangeView.Text = "목록 보기";
            }
            else
            {
                btnChangeView.ImageNormal = imgViewGroupDefault;
                btnChangeView.ImageClicked = imgViewGroupClick;
                btnChangeView.ImageMouseOver = imgViewGroupClick;

                dataGridViewCCTVList.Visible = true;
                mTreeViewCCTV.Visible = false;                
                //dataGridViewCCTVList.Size = mTreeViewCCTV.Size;
                //btnChangeView.Text = "그룹별 보기";
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

                        IFormContent frmContent = FormMain.Instance.PageHome.ContentForm;

                        frmContent.ZoomTarget(cctv.POI.X, cctv.POI.Y, cctv.POI.Z, cctv.POI.IsIndoor);
                        frmContent.SelectPOILoadZone(cctv.POI, cctv.POI.IsIndoor);

                        if (cctv.POI.IsIndoor)
                        {
                           
                            UnE.View.Content.IBaseView view = (UnE.View.Content.IBaseView)frmContent.IndoorView;
                            //view.Focus();
                            view.Refresh();
                        }
                        else
                        {
                            UnE.View.Content.IBaseView view = (UnE.View.Content.IBaseView)frmContent.OutdoorView;
                            //view.Focus();
                            view.Refresh();
                        }

                        FormMain.Instance.PageHome.OnPostPickPOI(cctv.POI);
                        
                    }
                }
            }
            
            this.Focus();
            this.Activate();
            //SetFocusUser();
        }

        private void btnEmptyCCTV_Click(object sender, EventArgs e)
        {
            FormMain.Instance.PageHome.OnPostPickPOI(NULL_POI);
            this.Focus();
        }


        public void OnRemoved(Control c)
        {

        }

        public void OnAdded(Control c)
        {

        }
        
        private Timer t = new Timer();
        public void SetFocusUser()
        {
            if (t.Enabled == true)
                t.Enabled = false;
            t = new Timer();
            t.Interval = 500;
            t.Tick += t_Tick;
            t.Enabled = true;
        }

        private int m_nCount = 0;
        void t_Tick(object sender, EventArgs e)
        {
            if (m_nCount == 3)
            {
                t.Enabled = false;
                m_nCount = -1;
            }           
            OnMouseLButtonClick();
            m_nCount++;
        }

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        private static int WM_LBUTTONDOWN = 0x201; //Left mousebutton down
        private static int WM_LBUTTONUP = 0x202; //Left mousebutton up
        private static int WM_LBUTTONDBLCLK = 0x203; //Left mousebutton doubleclick
        private static int WM_RBUTTONDOWN = 0x204; //Right mousebutton down
        private static int WM_RBUTTONUP = 0x205;  //Right mousebutton up
        private static int WM_RBUTTONDBLCLK = 0x206; //Right mousebutton doubleclick


        public void OnMouseLButtonClick()
        {
            IntPtr pt = MakeLParam(1, 1);
            if( this.IsDisposed == false && this.IsHandleCreated == true)
            {
                SendMessage(this.Handle, WM_RBUTTONDOWN, IntPtr.Zero, pt);
                SendMessage(this.Handle, WM_RBUTTONUP, IntPtr.Zero, pt);

            }
          
            //IsSelected = !IsSelected;
        }

        public IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
        }

        private void paneSearch_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void paneSearch_MouseUp(object sender, MouseEventArgs e)
        {
            int i = 0;
            i++;
        }

        private void FormCCTVList_Activated(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("CCTVList Activate");
        }

        private void FormCCTVList_Deactivate(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("CCTVList Deactivate");
        }

        private void FormCCTVList_Enter(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("CCTVList Get Focus");
        }

        private void FormCCTVList_Leave(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("CCTVList Lost Focus");
        }

        void FormCCTVList_GotFocus(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("CCTVList Get Focus");
        }

        private void FormCCTVList_MouseEnter(object sender, EventArgs e)
        {
            this.Focus();
        }

        private void textBoxDictionary_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFind_Click(null, null);
            }
        }

        private void SetManualID()
        {
            m_manualManager.Handle = this.Handle;

            m_manualManager.Clear();
            m_manualManager.SetID(this, "Toolbar_CCTV");            
            m_manualManager.SetID(dataGridViewCCTVList, "Toolbar_CCTV");
            m_manualManager.SetID(btnFind, "Toolbar_CCTV_Search");
            m_manualManager.SetID(btnChangeView, "Toolbar_CCTV_Mode");
            m_manualManager.SetID(mTreeViewCCTV, "Toolbar_CCTV_Mode");
            m_manualManager.ProcessEvent();
        } 
	}
}