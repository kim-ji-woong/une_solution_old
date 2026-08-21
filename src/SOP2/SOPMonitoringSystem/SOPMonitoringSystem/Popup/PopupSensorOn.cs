using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using SOPDisasterSystem;

namespace SOPMonitoringSystem.Popup
{
    public partial class PopupSensorOn : Form, IFormDisasterOwner
    {
        private static PopupSensorOn m_instance = null;
        public static PopupSensorOn Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new PopupSensorOn(FormMain.Instance.DBManager);

                return m_instance;
            }
        }
        // true : 실제모드 , false : 훈련모드
        //private bool bRealMode = false;

        private SDMS.FireDetectSignal m_detectSignal = null;
        public SDMS.FireDetectSignal DetectSignal
        {
            get { return m_detectSignal; }
            set { m_detectSignal = value; }
        }

        private bool m_isTreeMode = false;
        private SOPMonitoringSystem.FormDisaster m_frmDisaster = null;

        private static ArrayList m_arrIgnoreIDs = new ArrayList();

        private WebDBManager m_dbMgr = null;
        private string m_strIDs = "";
		
        //private bool m_isRegular = true;
        //private bool m_isNormal = true;

        private bool IsRegularMode
        {
            get { return true; }
        }

        private bool IsNormalMode
        {
            get { return radioNormal.Checked; }
        }

        private bool IsRealMode
        {
            get { return m_detectSignal.RealMode; }
        }

        public int ActionStepID
        {
            get { return m_detectSignal == null ? -1 : m_detectSignal.ActionStepID; }
        }

		private bool m_bHideForm = false;

		private bool m_bHasContorl = false;
		public bool HasContorl
		{
			get { return m_bHasContorl; }
			set { m_bHasContorl = value; }
		}

		public int EquipZoneID
		{
			get { return m_detectSignal == null ? -1 : m_detectSignal.EquipZoneID; }
		}

		public int SensorID
		{
			get { return m_detectSignal == null ? -1 : m_detectSignal.SensorID; }
		}

		public System.Windows.Forms.TextBox DetectZone
		{
			get { return textBox1; }
            set { textBox1 = value; }
		}
		public System.Windows.Forms.Label DetectTime
		{
			get { return mLabelDetectTime; }
			set { mLabelDetectTime = value; }
		}

        //private int m_nSensorHistoryID = -1;
        public int SensorHistoryID
        {
            get { return m_detectSignal == null ? -1 : m_detectSignal.SensorHistoryID; }
            //set { m_nSensorHistoryID = value; }
        }

        private SOPLoader m_sopLoader = null;
        private TreeNode m_prevSelectedNode = null;

        public PopupSensorOn(WebDBManager dbMgr)
        {
            InitializeComponent();
            m_dbMgr = dbMgr;

            m_sopLoader = new SOPLoader(treeView);

            if (!m_isTreeMode)
            {
                m_frmDisaster = new FormDisaster(this);

                m_frmDisaster.TopLevel = false;
                this.Controls.Add(m_frmDisaster);

                m_frmDisaster.Size = treeView.Size;
                m_frmDisaster.Location = treeView.Location;
                m_frmDisaster.Show();

                treeView.Size = new Size(10, 10);
                treeView.Location = new Point(-100, 0);
            }
        }

		//public static void PopUpForm(WebDBManager dbMgr, int nSensorID, int nSensorHistoryID, int nZoneID, DateTime detectTime, bool bHasControl)
        public static void PopUpForm(WebDBManager dbMgr, SDMS.FireDetectSignal signal, bool bHasControl)
		{
			SOPDisasterSystem.EquipmentZone equipZone = SOPDisasterSystem.DataManager.Instance.GetEquipZone(signal.EquipZoneID);
			if( m_instance == null)
				m_instance = new PopupSensorOn(dbMgr);

			
			if (equipZone == null && signal.SensorID != 0)
                return;

			// 수동 신고
			if (signal.SensorID == 0)
			{
				Zone realzone = SOPDisasterSystem.DataManager.Instance.GetZone(signal.EquipZoneID);
				m_instance.DetectZone.Text = realzone.BroadcastName;
			}
			else
			{
				m_instance.DetectZone.Text = equipZone.BroadcastName;
			}
			m_instance.DetectTime.Text = signal.DetectTime.ToString();

			if( m_instance.EquipZoneID != signal.EquipZoneID || m_instance.SensorID != signal.SensorID)
				m_instance.m_bHideForm = false;

            m_instance.DetectSignal = signal;

			m_instance.HasContorl = bHasControl;

            m_instance.m_bHideForm = !bHasControl;

			m_instance.InitForm();

            if (m_instance.m_bHideForm != true)
            {
                m_instance.ShowForm();
            }

            string strDisasterFullPath = FormMain.Instance.GetPageHome().GetQuickSOPFullPath(ID.ID_SOP_FIRE);

            // 표준화재 SOP Link
            if (strDisasterFullPath != null)
            {
                TreeNode node = FindDisasterNode(strDisasterFullPath, m_instance.treeView);

                if (node != null)
                {
                    m_instance.Focus();
                    m_instance.treeView.SelectedNode = node;
                    m_instance.treeView.Focus();
                }
            }
        }

        static private TreeNode FindDisasterNode(string strDisasterFullPath, TreeView tree)
        {
            int nIndex1 = strDisasterFullPath.IndexOf('/');
            if (nIndex1 < 0) return null;

            int nIndex2 = strDisasterFullPath.IndexOf('/', nIndex1 + 1);
            if (nIndex2 < 0) return null;

            TreeNode node = FindNode(strDisasterFullPath.Substring(0, nIndex1), tree.Nodes);
            if (node == null) return null;

            node = FindNode(strDisasterFullPath.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1), node.Nodes);
            if (node == null) return null;

            return FindNode(strDisasterFullPath.Substring(nIndex2 + 1), node.Nodes);
        }

        static private TreeNode FindNode(string strValue, TreeNodeCollection parentNodes)
        {
            TreeNodeCollection nodes = parentNodes;

            foreach (TreeNode node in nodes)
            {
                if (strValue == node.Text)
                    return node;
                TreeNode result = FindNode(strValue, node.Nodes);
                if (result != null)
                    return result;
            }

            return null;
        }

        public void ShowForm()
        {
            timer1.Start();
            base.Show();
        }

        public void HideForm()
        {
            timer1.Stop();
            Visible = false;
        }

		private void InitForm()
		{
            EnableForm(FormMain.Instance.HasControl);
		}

        private void EnableForm(bool enabled)
        {
            if (enabled)
            {
				btnHideForm.Visible = false;
				btnOK.Visible = true;
				btnCancel.Visible = true;

				treeView.Enabled = true;
				radioAbnormal.Enabled = true;
				radioNormal.Enabled = true;

				
				//radioButton1.Enabled = true;
				//radioButton2.Enabled = true;
			}
			else
			{
				btnHideForm.Visible = true;
				btnOK.Visible = false;
				btnCancel.Visible = false;

				treeView.Enabled = false;
				radioAbnormal.Enabled = false;
				radioNormal.Enabled = false;

				//radioButton1.Enabled = false;
				//radioButton2.Enabled = false;
			}

			if (DetectSignal.RealMode == true)
			{
				radioButton1.Checked = true;
				radioButton2.Checked = false;
			}
			else
			{
				radioButton1.Checked = false;
				radioButton2.Checked = true;
			}
        }
        private void PopupSensorOn_Load(object sender, EventArgs e)
        {
            //InitGrid();
            bool isNormal = SOPLoader.IsNormal(DateTime.Now);

            if (isNormal)
                radioNormal.Checked = true;
            else
                radioAbnormal.Checked = true;

			//radioButton1.Checked = true;
        }

        private void InitGrid()
        {
            string strSQL = "select equip.ID, equip.EquipID, equip.EquipType, Zone.ZoneName, equip.Description ";
            strSQL += "from FireEquipment as equip, Zone where equip.ZoneID = Zone.ID and equip.ID in ";
            strSQL += m_strIDs;

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;
            dataGridView.Rows.Clear();

            string[] strType = new string[3] {"소화기", "소화전", "발신기"};
            int nIndex = 0;

            for (int i=0;i<nResultCount - 4;i+=5)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strEquipID = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nEquipType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                string strZoneName = WebDBManager.GetStringField(arrResult[i + 3], "");
                string strDescription = WebDBManager.GetStringField(arrResult[i + 4], "");

                if (nID <= 0 || nEquipType < 1 || nEquipType > 3)
                    continue;

                DataGridViewRow row = new DataGridViewRow();

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = (++nIndex).ToString();
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = strEquipID;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = strType[nEquipType - 1];
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = strZoneName;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = strDescription;
                row.Cells.Add(cell);

                dataGridView.Rows.Add(row);
            }
        }

		/*private void InitTree(SOPManager sopMgr, bool isNormal)
		{
			m_isNormal = isNormal;
			treeView.Nodes.Clear();

			Dictionary<string, DisasterInfo> dicSOP = sopMgr.GetSOPDictionary(true, m_isNormal);
			Dictionary<int, VersionInfo> dicVersion = sopMgr.GetVersionDictionary(true, m_isNormal);

			foreach (KeyValuePair<string, DisasterInfo> pair in dicSOP)
			{
				string strFullPath = pair.Key;
                
				int nIndex1 = strFullPath.IndexOf((char)0x06);
				int nIndex2 = strFullPath.LastIndexOf((char)0x06);
				if (nIndex1 < 0 || nIndex2 < 0) continue;

				string strCategoryName = strFullPath.Substring(0, nIndex1);
				string strSubCategoryName = strFullPath.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
				string strDisasterName = strFullPath.Substring(nIndex2 + 1);

				// 화재만 얻어온다.
				if (strSubCategoryName != "화재")
					continue;

				TreeNode nodeCategory = FindNode(strCategoryName, treeView.Nodes);

				if (nodeCategory == null)
					nodeCategory = treeView.Nodes.Add(strCategoryName);

				TreeNode nodeSubCategory = FindNode(strSubCategoryName, nodeCategory.Nodes);

				if (nodeSubCategory == null)
					nodeSubCategory = nodeCategory.Nodes.Add(strSubCategoryName);

				TreeNode nodeDisaster = FindNode(strDisasterName, nodeSubCategory.Nodes);

				if (nodeDisaster == null)
					nodeDisaster = nodeSubCategory.Nodes.Add(strDisasterName);

				if (nodeDisaster.Tag == null)
					AddActionStep(nodeDisaster, pair.Value, dicVersion);
			}

			treeView.ExpandAll();
		}

		public TreeNode FindNode(string strValue, TreeNodeCollection parentNodes = null)
		{
			TreeNodeCollection nodes = parentNodes == null ? treeView.Nodes : parentNodes;

			foreach (TreeNode node in nodes)
			{
				if (strValue == node.Text)
					return node;
				TreeNode result = FindNode(strValue, node.Nodes);
				if (result != null)
					return result;
			}

			return null;
		}

		public TreeNode FindNode(int nTag, TreeNodeCollection nodes = null)
		{
			if (nodes == null)
				nodes = treeView.Nodes;

			foreach (TreeNode node in nodes)
			{
				if (node.Tag != null && (int)node.Tag == nTag)
					return node;

				TreeNode result = FindNode(nTag, node.Nodes);
				if (result != null)
					return result;
			}

			return null;
		}

		private void AddActionStep(TreeNode nodeDisaster, DisasterInfo disaster, Dictionary<int, VersionInfo> dicVersion)
		{
			ArrayList arrActionSteps = disaster.ActionSteps;
			int nDisasterID = disaster.DisasterID;

			if (arrActionSteps == null)
			{
				nodeDisaster.Tag = 0;
				return;
			}

			nodeDisaster.Tag = nDisasterID;
			AddActionStep(nodeDisaster, arrActionSteps);
		}

		private void InsertArray(ArrayList arrSrc, ArrayList arrTrg)
		{
			foreach (object obj in arrSrc)
			{
				arrTrg.Add(obj);
			}
		}

		private void AddActionStep(TreeNode node, ArrayList arrActionSteps)
		{
			ArrayList arrChildActionStep = new ArrayList();

			ArrayList _arrActionSteps = new ArrayList();
			InsertArray(arrActionSteps, _arrActionSteps);

			while (_arrActionSteps.Count > 0)
			{
				arrChildActionStep.Clear();

				int nChildCount = arrChildActionStep.Count;

				foreach (ActionStepInfo actionStep in _arrActionSteps)
				{
					if (actionStep.ParentStepID == -1)
					{
						TreeNode nodeStep = node.Nodes.Add(actionStep.ActionStepName);
						nodeStep.Tag = actionStep.ActionStepID;
					}
					else
					{
						TreeNode nodeParent = FindNode(actionStep.ParentStepID, node.Nodes);

						if (nodeParent != null)
						{
							TreeNode nodeStep = nodeParent.Nodes.Add(actionStep.ActionStepName);
							nodeStep.Tag = actionStep.ActionStepID;
						}
						else
							arrChildActionStep.Add(actionStep);
					}
				}

				if (nChildCount == arrChildActionStep.Count)
					break;

				_arrActionSteps.Clear();

				// 부모 단계가 존재하는 ActionStep들
				InsertArray(arrChildActionStep, _arrActionSteps);
			}
		}*/


		private void radioModeCheckedChanged(object sender, EventArgs e)
		{
			/*if (sender == radioButton1)
			{
				bRealMode = true;
			}
			else
			{
				bRealMode = false;
			}*/
		}

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == radioNormal)
            {
                //InitTree(FormMain.Instance.SOPManager, true);
                m_sopLoader.LoadTree(FormMain.Instance.SOPManager, IsRegularMode, true, "화재");
            }
            else
            {
                //InitTree(FormMain.Instance.SOPManager, false);
                m_sopLoader.LoadTree(FormMain.Instance.SOPManager, IsRegularMode, false, "화재");
            }

            m_frmDisaster.LoadSOP(treeView.Nodes, "화재", "화재");

            string strDisasterFullPath = FormMain.Instance.GetPageHome().GetQuickSOPFullPath(ID.ID_SOP_FIRE);

            // 표준화재 SOP Link
            if (strDisasterFullPath != null)
            {
                TreeNode node = FindDisasterNode(strDisasterFullPath, m_instance.treeView);

                if (node != null)
                {
                    m_frmDisaster.SelectSOP(node);
                }
            }
        }
        
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (m_detectSignal == null)
                return;

            TreeNode node = null;

            if (treeView.SelectedNode != null)
            {
                if (treeView.SelectedNode.Level < 2)
                {
                    MessageBox.Show("실행시킬 SOP를 선택하여 주십시오.");
                    return;
                }
                else
                {
                    // SOP 아래의 단계가 선택되어 있지 않으면 첫번째 단계를 선택한다.
                    if (treeView.SelectedNode.Level == 2)
                    {
                        if (treeView.SelectedNode.Nodes.Count == 0)
                            return;

                        treeView.SelectedNode = treeView.SelectedNode.Nodes[0];
                    }
                    
                    node = treeView.SelectedNode;
                }
            }
            else
            {
                MessageBox.Show("실행시킬 SOP를 선택하여 주십시오.");
                return;
            }

            m_detectSignal.ActionStepID = (int)node.Tag;

			FormMain frmMain = FormMain.Instance;

            frmMain.ChangeMode(IsRealMode, m_instance.IsRegularMode, m_instance.IsNormalMode);
			frmMain.VirtualMode(!IsRealMode);
			BarLevelTree tree = frmMain.GetPageHome().GetDockScenario().GetBarLevelTree();
			node = tree.FindActionStepNode(m_instance.ActionStepID);
			if (node == null)
				return;

			tree.TreeView.SelectedNode = node;


			if (m_detectSignal.SensorID == 0)
			{
				Zone zone = DataManager.Instance.GetZone(this.EquipZoneID);
				if (zone == null)
					return;
                m_detectSignal.PositionName = textBox1.Text;
				if (!frmMain.PlayWithDisasterPosition(zone.ID, this.SensorID, m_detectSignal.SensorHistoryID))
				{
					// 이미 실행중인 경우
					DockingLeftScenario scenario = frmMain.GetPageHome().GetDockScenario();

					int nRowIndex = scenario.FindRowIndex(m_instance.ActionStepID, IsRealMode);
					if (nRowIndex >= 0)
						scenario.SelectRow(nRowIndex);
				}
			}
			else
			{
				SOPDisasterSystem.EquipmentZone equipZone = SOPDisasterSystem.DataManager.Instance.GetEquipZone(this.EquipZoneID);
				if (equipZone == null || equipZone.LinkedZoneList.Count == 0)
					return;
				SOPDisasterSystem.Zone zone = (SOPDisasterSystem.Zone)equipZone.LinkedZoneList[0];
                m_detectSignal.PositionName = textBox1.Text;
				if (!frmMain.PlayWithDisasterPosition(zone.ID, this.SensorID, m_detectSignal.SensorHistoryID))
				{
					// 이미 실행중인 경우
					DockingLeftScenario scenario = frmMain.GetPageHome().GetDockScenario();

					int nRowIndex = scenario.FindRowIndex(m_instance.ActionStepID, IsRealMode);
					if (nRowIndex >= 0)
						scenario.SelectRow(nRowIndex);
				}
			}
           
			ClearSensorInfo(this.SensorHistoryID);

            HideForm();

            // Send SOP
            // ActionStepHistoryID는 HistoryManager의 Thread에서 생성되므로 Thread를 생성시켜
            // ActionStepHistoryID가 생성될때까지 기다린다.
            Thread t = new Thread(new ThreadStart(SendRunSOPThread));
            t.Start();

        }
        
        private void SendRunSOPThread()
        {
            int nSensorHistoryID = m_instance.SensorHistoryID;
            int nActionStepID = m_instance.ActionStepID;

            // nActionStepID에 대한 기존 History가 남아있을지 모르기 때문에
            // 새로운 HistoryID가 생성될때까지 잠시 대기
            Thread.Sleep(1500);

            while (true)
            {
                int nHistoryID = FormMain.Instance.SOPManager.GetActionStepHistoryID(nActionStepID, IsRealMode);

                if (nHistoryID > 0)
                {
                    m_instance.DetectSignal.ActionStepHistoryID = nHistoryID;
                    FormMain.Instance.NetworkManager.SendRunSOP(nSensorHistoryID, nHistoryID);
                    break;
                }
				
				// 종료되는 시점인지 확인
				if (FormMain.Instance.CloseThread == true)
				{
					break;
				}
                
				Thread.Sleep(1000);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            FormMain.Instance.NetworkManager.SendIgnoreSOP(this.SensorHistoryID);

			ClearSensorInfo(this.SensorHistoryID);
            HideForm();
        }
       
		private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{

		}

		public void ClearSensorInfo(int nSensorHistoryID)
		{
			string szSQL = "DELETE FROM FireSensorSignal where SensorHistoryID = " + nSensorHistoryID.ToString();
			m_dbMgr.GetResultData(szSQL, 0);
		}

        private void PopupSensorOn_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FormMain.Instance.HasControl)
                btnCancel_Click(null, null);
            else
                btnHideForm_Click(null, null);
            // 종료 이벤트 취소해야 dispose가 되지않는다.
			e.Cancel = true;
        }

        private void btnHideForm_Click(object sender, EventArgs e)
        {
            //this.Visible = false;
            HideForm();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            EnableForm(FormMain.Instance.HasControl);
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = treeView.SelectedNode;

            if (m_prevSelectedNode == node)
                goto RETURN_FALSE;

            m_prevSelectedNode = node;
            if (node == null) goto RETURN_FALSE;

            FormMain frmMain = FormMain.Instance;
            string strSOPInfo = "";

            if (!m_sopLoader.GetSOPVersionInfo(node, frmMain.SOPManager, IsRegularMode, IsNormalMode, ref strSOPInfo))
                goto RETURN_FALSE;

            rTextBoxSOPInfo.Text = strSOPInfo;
            return;

        RETURN_FALSE:
            rTextBoxSOPInfo.Text = "";
        }

        public void OnTreeViewClicked(TreeNode node, bool noSelect)
        {
            if (node == null)
                return;

            if (!noSelect)
                treeView.SelectedNode = node;
        }

        public void EnableButton(bool isPrevButton, bool enabled)
        {
        }
    }

    public class SOPLoader
    {
        private TreeView m_tree = null;

        public SOPLoader(TreeView tree)
        {
            m_tree = tree;
        }

        public void LoadTree(SOPManager sopMgr, bool isRegular, bool isNormal, string strTargetSubCategoryName = "")
        {
            m_tree.Nodes.Clear();

            Dictionary<string, DisasterInfo> dicSOP = sopMgr.GetSOPDictionary(isRegular, isNormal);
            Dictionary<int, VersionInfo> dicVersion = sopMgr.GetVersionDictionary(isRegular, isNormal);

            foreach (KeyValuePair<string, DisasterInfo> pair in dicSOP)
            {
                string strFullPath = pair.Key;

				int nIndex1 = strFullPath.IndexOf((char)0x06);
				int nIndex2 = strFullPath.LastIndexOf((char)0x06);
                if (nIndex1 < 0 || nIndex2 < 0) continue;

                string strCategoryName = strFullPath.Substring(0, nIndex1);
                string strSubCategoryName = strFullPath.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                string strDisasterName = strFullPath.Substring(nIndex2 + 1);

                if (strTargetSubCategoryName.Length > 0)
                {
                    if (strSubCategoryName != strTargetSubCategoryName)
                        continue;
                }

                TreeNode nodeCategory = FindNode(strCategoryName, m_tree.Nodes);

                if (nodeCategory == null)
                    nodeCategory = m_tree.Nodes.Add(strCategoryName);

                TreeNode nodeSubCategory = FindNode(strSubCategoryName, nodeCategory.Nodes);

                if (nodeSubCategory == null)
                    nodeSubCategory = nodeCategory.Nodes.Add(strSubCategoryName);

                TreeNode nodeDisaster = FindNode(strDisasterName, nodeSubCategory.Nodes);

                if (nodeDisaster == null)
                    nodeDisaster = nodeSubCategory.Nodes.Add(strDisasterName);

                if (nodeDisaster.Tag == null)
                    AddActionStep(nodeDisaster, pair.Value, dicVersion);
            }

            m_tree.ExpandAll();
        }

        private void AddActionStep(TreeNode nodeDisaster, DisasterInfo disaster, Dictionary<int, VersionInfo> dicVersion)
        {
            ArrayList arrActionSteps = disaster.ActionSteps;
            int nDisasterID = disaster.DisasterID;

            if (arrActionSteps == null)
            {
                nodeDisaster.Tag = 0;
                return;
            }

            nodeDisaster.Tag = nDisasterID;
            AddActionStep(nodeDisaster, arrActionSteps);
        }

        private void AddActionStep(TreeNode node, ArrayList arrActionSteps)
        {
            ArrayList arrChildActionStep = new ArrayList();

            ArrayList _arrActionSteps = new ArrayList();
            InsertArray(arrActionSteps, _arrActionSteps);

            while (_arrActionSteps.Count > 0)
            {
                arrChildActionStep.Clear();

                int nChildCount = arrChildActionStep.Count;

                foreach (ActionStepInfo actionStep in _arrActionSteps)
                {
                    if (actionStep.ParentStepID == -1)
                    {
                        TreeNode nodeStep = node.Nodes.Add(actionStep.ActionStepName);
                        nodeStep.Tag = actionStep.ActionStepID;
                    }
                    else
                    {
                        TreeNode nodeParent = FindNode(actionStep.ParentStepID, node.Nodes);

                        if (nodeParent != null)
                        {
                            TreeNode nodeStep = nodeParent.Nodes.Add(actionStep.ActionStepName);
                            nodeStep.Tag = actionStep.ActionStepID;
                        }
                        else
                            arrChildActionStep.Add(actionStep);
                    }
                }

                if (nChildCount == arrChildActionStep.Count)
                    break;

                _arrActionSteps.Clear();

                // 부모 단계가 존재하는 ActionStep들
                InsertArray(arrChildActionStep, _arrActionSteps);
            }
        }

        private void InsertArray(ArrayList arrSrc, ArrayList arrTrg)
        {
            foreach (object obj in arrSrc)
            {
                arrTrg.Add(obj);
            }
        }

        public TreeNode FindNode(string strValue, TreeNodeCollection parentNodes = null)
        {
            TreeNodeCollection nodes = parentNodes == null ? m_tree.Nodes : parentNodes;

            foreach (TreeNode node in nodes)
            {
                if (strValue == node.Text)
                    return node;
                TreeNode result = FindNode(strValue, node.Nodes);
                if (result != null)
                    return result;
            }

            return null;
        }

        public TreeNode FindNode(int nTag, TreeNodeCollection nodes = null)
        {
            if (nodes == null)
                nodes = m_tree.Nodes;

            foreach (TreeNode node in nodes)
            {
                if (node.Tag != null && (int)node.Tag == nTag)
                    return node;

                TreeNode result = FindNode(nTag, node.Nodes);
                if (result != null)
                    return result;
            }

            return null;
        }

        public bool GetSOPVersionInfo(TreeNode node, SOPManager sopMgr, bool isRegular, bool isNormal, ref string strSOPInfo)
        {
            // Disaster
            if (node.Level < 2)
                return false;
            else
                node = Get2LevelNode(node, node.Level);

			string strFullPath = node.FullPath.Replace('\\', (char)0x06);
            ArrayList arrDisasterList = sopMgr.GetSOPDisasterList(strFullPath, isRegular, isNormal);

            if (arrDisasterList == null)
                return false;

            int nCount = arrDisasterList.Count;
            if (nCount == 0)
                return false;

            // 첫번째 버전
            DisasterInfo disasterBegin = (DisasterInfo)arrDisasterList[0];
            // 마지막 버전
            DisasterInfo disasterLast = (DisasterInfo)arrDisasterList[nCount - 1];

            Dictionary<int, VersionInfo> dicDisasterVersion = sopMgr.GetVersionDictionary(isRegular, isNormal);

            if (!dicDisasterVersion.ContainsKey(disasterBegin.DisasterID) ||
                !dicDisasterVersion.ContainsKey(disasterLast.DisasterID))
                return false;

            VersionInfo versionBegin = dicDisasterVersion[disasterBegin.DisasterID];
            VersionInfo versionLast = dicDisasterVersion[disasterLast.DisasterID];
            strSOPInfo = GetSOPInfoString(versionBegin, versionLast);

            //int nActionStepID = (int)node.Tag;
            //versionInfo = FormMain.Instance.SOPManager.GetActionStepVersionInfo(nActionStepID);

            return true;
        }

        private TreeNode Get2LevelNode(TreeNode node, int nNodeLevel)
        {
            for (int i = nNodeLevel - 2; i > 0; i--)
            {
                node = node.Parent;
            }

            return node;
        }

        public string GetSOPInfoString(VersionInfo versionBegin, VersionInfo versionLast)
        {
            string strSOPInfo = "버전명 : " + versionLast.VersionName;
            strSOPInfo += "\r\n작성자 : " + versionLast.UserName;
            strSOPInfo += "\r\n생성일자 : " + ToDateString(versionBegin.BeginTime);
            strSOPInfo += "\r\n수정일자 : " + ToDateString(versionLast.EndTime);
            strSOPInfo += "\r\n부가설명 : " + versionLast.Description;

            return strSOPInfo;
        }

        private string ToDateString(DateTime dt)
        {
            return string.Format("{0}년 {1}월 {2}일", dt.Year, dt.Month, dt.Day);
        }

        public static bool IsNormal(DateTime time)
        {
            if (time.DayOfWeek == DayOfWeek.Saturday ||
                time.DayOfWeek == DayOfWeek.Sunday)
                return false;

            PageBackstageOption opt = FormMain.Instance.GetPageOption();

            int nBeginHour = opt.BeginHour;
            int nBeginMinute = opt.BeginMinute;
            int nEndHour = opt.EndHour;
            int nEndMinute = opt.EndMinute;

            if (time.Hour > nBeginHour)
            {
                if (time.Hour < nEndHour)
                    return true;
                else if (time.Hour == nEndHour)
                    return time.Minute <= nEndMinute;
            }
            else if (time.Hour == nBeginHour)
            {
                if (time.Minute >= nBeginMinute)
                {
                    if(time.Hour < nEndHour)
                        return true;
                    else if (time.Hour == nEndHour)
                        return time.Minute <= nEndMinute;
                }
            }

            return false;
        }
    }
}
