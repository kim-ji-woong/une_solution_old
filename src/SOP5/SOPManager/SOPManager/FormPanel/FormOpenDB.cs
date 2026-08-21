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
	public partial class FormOpenDB : Form
	{
		private WebDBManager m_dbMgr = null;
		// FullPath(Category/SubCategory/Disaster), DisasterInfo List
		private Dictionary<string, ArrayList> m_dicSOPRegularNormal = new Dictionary<string, ArrayList>();
		private Dictionary<string, ArrayList> m_dicSOPRegularEmergency = new Dictionary<string, ArrayList>();
		private Dictionary<string, ArrayList> m_dicSOPNonRegularNormal = new Dictionary<string, ArrayList>();
		private Dictionary<string, ArrayList> m_dicSOPNonRegularEmergency = new Dictionary<string, ArrayList>();

		// DisasterID, VersionInfo
		private Dictionary<int, VersionInfo> m_dicVersionRegularNormal = new Dictionary<int, VersionInfo>();
		private Dictionary<int, VersionInfo> m_dicVersionRegularEmergency = new Dictionary<int, VersionInfo>();
		private Dictionary<int, VersionInfo> m_dicVersionNonRegularNormal = new Dictionary<int, VersionInfo>();
		private Dictionary<int, VersionInfo> m_dicVersionNonRegularEmergency = new Dictionary<int, VersionInfo>();

		// VersionID, VersionInfo
		private Dictionary<int, VersionInfo> m_dicVersionInfo = new Dictionary<int, VersionInfo>();

		private TreeNode m_prevSelectedNode = null;
		private int m_nPrevSelectedRow = -1;
		private bool m_isPrevRegular = true;
		private bool m_isPrevNormal = true;

		private string m_selectedCategoryName = "";
		private string m_selectedSubCategoryName = "";
		private string m_selectedDisasterName = "";
		private ArrayList m_arrSelectedActionSteps = null;
		private VersionInfo m_selectedVersion = null;

        // 재난 Tree의 Node별 구분자
        private char m_chDelimeter = (char)6;
        private string m_strDelimeter = "";

		public string CategoryName
		{
			get { return m_selectedCategoryName; }
		}

		public string SubCategoryName
		{
			get { return m_selectedSubCategoryName; }
		}

		public string DisasterName
		{
			get { return m_selectedDisasterName; }
		}

		public ArrayList ActionSteps
		{
			get { return m_arrSelectedActionSteps; }
		}

		public VersionInfo Version
		{
			get { return m_selectedVersion; }
		}

		public bool IsRegular
		{
			get { return m_isPrevRegular; }
		}

		public bool IsNormal
		{
			get { return m_isPrevNormal; }
		}
        
		public FormOpenDB()
		{
            m_strDelimeter = m_chDelimeter.ToString();
			m_dbMgr = FormMain.Instance.DBManager;

			InitializeComponent();

			radioRegular.Checked = true;
			radioNormal.Checked = true;
			SetRadioImage();

			TopLevel = false;
			StartPosition = FormStartPosition.Manual;
			ShowInTaskbar = false;
			//BackColor = Color.FromArgb(227, 226, 226);

			InitTree();
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
		
		public void InitTree()
		{
			ImageList arImageList = new ImageList();
            List<Bitmap> imageList = SOPCategory.GetSubCategoryIamgeList();

            foreach (Bitmap bmp in imageList)
            {
                arImageList.Images.Add(bmp);
            }
			/*arImageList.Images.Add(global::SOPManager.Properties.Resources.btnEtc_User);
			arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_typoon);
			arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_earthquake);
			arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_snowfall);
			arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_flooding);
			arImageList.Images.Add(global::SOPManager.Properties.Resources.btnEtc_User);
			arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_fire);
			arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_fire);
			arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_spill);
			arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_spill);
			arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_spill);
			arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_spill);
			arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_terror);
			arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_volcano);
			arImageList.Images.Add( global::SOPManager.Properties.Resources.btnEtc_User);
			arImageList.Images.Add( global::SOPManager.Properties.Resources.btn_sub_terror);
			arImageList.Images.Add( global::SOPManager.Properties.Resources.btn_sub_terror);
			arImageList.Images.Add( global::SOPManager.Properties.Resources.btn_sub_terror);
			arImageList.Images.Add( global::SOPManager.Properties.Resources.btn_sub_terror);
			arImageList.Images.Add( global::SOPManager.Properties.Resources.btn_sub_terror);
			arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_strongwind);*/
			treeViewSOP.ImageList = arImageList;
			LoadVersion();
			LoadSOP();

			ResetTree();

            UpdateControlSize();
		}

        public void UpdateControlSize()
        {
            Double[] dWindowRate = FormMain.Instance.GetCurWindowRate();
            double WindowRateWidth = dWindowRate[0];
            double WindowRateHeight = dWindowRate[1];

            this.Size = new System.Drawing.Size((int)(this.Size.Width * WindowRateWidth), (int)(this.Size.Height * WindowRateHeight));

            ImageList arImageList = new ImageList();
            List<Bitmap> imageList = SOPCategory.GetSubCategoryIamgeList();
            arImageList.ImageSize = new System.Drawing.Size((int)((double)arImageList.ImageSize.Width * WindowRateWidth), (int)((double)arImageList.ImageSize.Height * WindowRateHeight));
            foreach (Bitmap bmp in imageList)            
                arImageList.Images.Add(bmp);            

            treeViewSOP.ImageList = arImageList;


            FormMain.Instance.UpdateWindowRate(flowLayoutPanel1, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(label1, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(rdPictureBox3, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(rdLabel3, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(rdPictureBox4, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(rdLabel4, WindowRateWidth, WindowRateHeight);

            FormMain.Instance.UpdateWindowRate(treeViewSOP, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(dataGridViewVersion, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(textSOPInfo, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(btnOpenSOP2, WindowRateWidth, WindowRateHeight);
    
            //foreach (Control ctl in this.Controls)
            //{
            //    HaveControl(ctl, WindowRateWidth, WindowRateHeight);
            //}
        }

        private void HaveControl(Control pctl, double WindowRateWidth, double WindowRateHeight)
        {
            foreach (Control ctl in pctl.Controls)
            {
                if (ctl.Controls.Count > 0)
                    HaveControl(ctl, WindowRateWidth, WindowRateHeight);

                FormMain.Instance.UpdateWindowRate(ctl, WindowRateWidth, WindowRateHeight);
            }
        }

		private object[] m_arSubCategorys = 
		{
			"SOP상황", "태풍",	"지진", "폭설", "침수", "일반재해", "화재","산불", 	"오염", "누출","유출","암모니아", 
			"테러",	"폭발", "119상황","무장", "괴선박", "폭탄", "침입", "폭약", "자연재해", "방범"
		};

		/*private int SetSubCategoryImage(string strValue)
		{
			for (int i = 0; i < m_arSubCategorys.Length; i++)
			{
				if (strValue == (string)m_arSubCategorys[i] || strValue.Contains((string)m_arSubCategorys[i]))
					return i;
			}
			return 0;
		}*/

        private object[] m_arCategorys = 
		{
			"화재", "자연재해", "방범", "오염", "누출", "유출", "암모니아", "수소",
			"테러",	"폭발", "무장", "괴선박", "폭탄", "침입", "폭약", "일반재해","기타", "SOP상황"
		};

        private int GetSOPTreeIndex(string strValue)
        {
            for (int i = 0; i < m_arCategorys.Length; i++)
            {
                if (strValue == (string)m_arCategorys[i] || strValue.Contains((string)m_arCategorys[i]))
                    return i;
            }
            return 0;
        }

        public void SelectNode(int nDepth, int nTag)
        {
            SelectNode(nDepth, nTag, treeViewSOP.Nodes);
        }

		public bool SelectNode(int nDepth, string szFullPath)
		{
			return SelectNode(nDepth, szFullPath, treeViewSOP.Nodes);
		}

		private string GetNodePath(TreeNode node)
		{
			if (node == null)
				return "";
			if (node.Parent == null)
				return "";
			if (node.Parent.Parent == null)
				return "";
			
			string szCategoryName = node.Parent.Parent.Text;
			string szSubCategoryName = node.Parent.Text;
			string szDisasterName = node.Text;

			string strFullPath = szCategoryName + m_strDelimeter + szSubCategoryName + m_strDelimeter + szDisasterName;
			return strFullPath;
		}

		private bool SelectNode(int nDepth, string szFullPath, TreeNodeCollection nodes)
		{
			foreach (TreeNode node in nodes)
			{
				string szPath = GetNodePath(node);
				if (szPath == szFullPath)
				{
					treeViewSOP.SelectedNode = node;
					
					return true;
				}				
				else
				{
					bool bResult = SelectNode(nDepth, szFullPath, node.Nodes);
					if (bResult == true)
						return true;
				}
			}
			return false;
		}

        private void SelectNode(int nDepth, int nTag, TreeNodeCollection nodes)
        {
            nDepth--;

            if (nDepth < 0)
                return;

            foreach (TreeNode node in nodes)
            {
                if (nDepth == 0)
                {
                    if (node.Tag == null)
                        continue;

                    if ((int)node.Tag == nTag)
                    {
                        treeViewSOP.SelectedNode = node;
                        return;
                    }
                }
                else
                {
                    SelectNode(nDepth, nTag, node.Nodes);
                }
            }
        }

		#region Image Check 박스 처리 영역
		private void SetRadioImage()
		{
			if (radioRegular.Checked == true)
			{
				rdPictureBox1.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_black;
			}
			else
			{
				rdPictureBox1.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_gray;
			}

			if (radioButton2.Checked == true)
			{
				rdPictureBox2.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_black;
			}
			else
			{
				rdPictureBox2.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_gray;
			}

			if (radioNormal.Checked == true)
			{
				rdPictureBox3.Image = global::SOPManager.Properties.Resources.__SOPEDIT_Enable2;
			}
			else
			{
                rdPictureBox3.Image = global::SOPManager.Properties.Resources.__SOPEDIT_Disable2;
			}

			if (radioButton4.Checked == true)
			{
                rdPictureBox4.Image = global::SOPManager.Properties.Resources.__SOPEDIT_Enable2;
			}
			else
			{
                rdPictureBox4.Image = global::SOPManager.Properties.Resources.__SOPEDIT_Disable2;
			}
		}

		private void rdPictureBox1_Click(object sender, EventArgs e)
		{
			rdLabel1_Click(sender, e);
		}

		private void rdPictureBox2_Click(object sender, EventArgs e)
		{
			rdLabel2_Click(sender, e);
		}

		private void rdPictureBox3_Click(object sender, EventArgs e)
		{
			rdLabel3_Click(sender, e);
		}

		private void rdPictureBox4_Click(object sender, EventArgs e)
		{
			rdLabel4_Click(sender, e);
		}

		private void rdLabel1_Click(object sender, EventArgs e)
		{
			if (radioRegular.Checked == false)
			{
				radioRegular.Checked = !radioRegular.Checked;
				SetRadioImage();
			}
		}

		private void rdLabel2_Click(object sender, EventArgs e)
		{
			if (radioButton2.Checked == false)
			{
				radioButton2.Checked = !radioButton2.Checked;
				SetRadioImage();
			}
		}

		private void rdLabel3_Click(object sender, EventArgs e)
		{
			if (radioNormal.Checked == false)
			{
				radioNormal.Checked = !radioNormal.Checked;
				SetRadioImage();
			}
		}

		private void rdLabel4_Click(object sender, EventArgs e)
		{
			if (radioButton4.Checked == false)
			{
				radioButton4.Checked = !radioButton4.Checked;
				SetRadioImage();
			}
		}
		#endregion

        public void SelectChangePage(int nPagenum)
        {
			if (nPagenum == ID.ID_FILE_OPEN)
			{
				label1.Text = "전체 SOP";

                //btnOpenSOP2.Text = "시나리오 열기 >";
                //btnOpenSOP2.NormalImage = global::SOPManager.Properties.Resources.__COMMON_ScenarioOpen;
                //btnOpenSOP2.MouseOverImage = global::SOPManager.Properties.Resources.__COMMON_ScenarioOpenClick;
                //btnOpenSOP2.ClickedImage = global::SOPManager.Properties.Resources.__COMMON_ScenarioOpenClick;
			}
			else
			{
				label1.Text = "삭제할 SOP 선택";

                //btnOpenSOP2.Text = "시나리오 삭제 >";
                btnOpenSOP2.ImageNormal = global::SOPManager.Properties.Resources.DeleteSOP;
                btnOpenSOP2.ImageMouseOver = global::SOPManager.Properties.Resources.DeleteSOP_Click;
                btnOpenSOP2.ImageClicked = global::SOPManager.Properties.Resources.DeleteSOP_Click;
            }
        }

		private void dataGridViewVersion_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			OnSelectedGrid(e.RowIndex);
		}

		private void radioRegular_CheckedChanged(object sender, EventArgs e)
		{
			if (radioRegular.Checked == m_isPrevRegular)
				return;

			m_isPrevRegular = radioRegular.Checked;

			ResetTree();

			int nRowCount = dataGridViewVersion.RowCount;

			for (int i = 0; i < nRowCount; i++)
				dataGridViewVersion.Rows.RemoveAt(0);
			
			SetVersionInfo(null);
		}

		private void radioNormal_CheckedChanged(object sender, EventArgs e)
		{
			if (radioNormal.Checked == m_isPrevNormal)
				return;

			m_isPrevNormal = radioNormal.Checked;

			ResetTree();

			int nRowCount = dataGridViewVersion.RowCount;

			for (int i = 0; i < nRowCount; i++)
				dataGridViewVersion.Rows.RemoveAt(0);
			
			SetVersionInfo(null);
		}

		private bool m_bButtonProcess = false;
		private void btnOpenSOP_Click(object sender, EventArgs e)
		{
            if (m_bButtonProcess == false)
            {
                m_bButtonProcess = true;

                if (label1.Text.Contains("삭제"))
                    OnClickDeleteSOP();
                else
                    OnClickOpenSOP(); 
							
                m_bButtonProcess = false;
            }
		}

        private VersionInfo GetSelectedVersion()
        {
            if (m_prevSelectedNode != null)
            {
                m_selectedCategoryName = m_prevSelectedNode.Parent.Parent.Text;
                m_selectedSubCategoryName = m_prevSelectedNode.Parent.Text;
                m_selectedDisasterName = m_prevSelectedNode.Text;

                Dictionary<string, ArrayList> dicSOP = GetSOPDictionary(radioRegular.Checked, radioNormal.Checked);
                Dictionary<int, VersionInfo> dicVersion = GetVersionDictionary(radioRegular.Checked, radioNormal.Checked);

                string strFullPath = m_selectedCategoryName + m_strDelimeter + m_selectedSubCategoryName + m_strDelimeter + m_selectedDisasterName;
                ArrayList arrDisasters = null;

                if (dicSOP.ContainsKey(strFullPath))
                    arrDisasters = dicSOP[strFullPath];

                m_selectedVersion = null;
                m_arrSelectedActionSteps = null;

                if (dataGridViewVersion.SelectedCells.Count == 0)
                {
					UnE.Utility.UMessageBoxRibbon.Show("SOP 버전을 선택해 주세요", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    m_bButtonProcess = false;
                    return null;
                }

                VersionInfo info = (VersionInfo)dataGridViewVersion.Rows[dataGridViewVersion.SelectedCells[0].RowIndex].Tag;
                int nVersionID = info.VersionID;
                m_arrSelectedActionSteps = GetActionSteps(arrDisasters, nVersionID);

                if (m_dicVersionInfo.ContainsKey(nVersionID))
                    return m_dicVersionInfo[nVersionID];
            }

            return null;
        }

        private void OnClickOpenSOP()
        {
            m_selectedVersion = GetSelectedVersion();

            if (m_selectedVersion != null)
                OpenSOP();

			if (this.ParentForm != null)
			{
				ParentForm.Close();
				
			}
        }

        private void OnClickDeleteSOP()
        {
			string szPreSelectedNodePath = GetNodePath(m_prevSelectedNode);
            m_selectedVersion = GetSelectedVersion();

            if (m_selectedVersion != null)
            {
				IOManager mgr = new IOManager();
				int nVersionID = m_selectedVersion.VersionID;
				
				if(mgr.IsMonitoringSOPVersion(m_dbMgr, nVersionID, false))
				{
					string szMsg2 = "아래의 SOP가 사용 중 이어서 삭제가 취소됩니다.\n모니터링 시스템에서 중지 후 삭제 해 주세요\n사용 중인 SOP : {0} ({1})";
					string szMsg1 = string.Format(szMsg2, m_selectedDisasterName, m_selectedVersion.VersionName);

					UnE.Utility.UMessageBoxRibbon.Show(this, szMsg1, "삭제 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				string szText = m_selectedDisasterName + "(" + m_selectedVersion.VersionName + ")" +
                    "를 정말 삭제하시겠습니까?\r\n삭제된 버전은 되돌릴 수 없습니다.";

				if (UnE.Utility.UMessageBoxRibbon.Show(this, szText, "경고", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (m_dbMgr.BeginBatch())
                    {
                        // DeleteSOPVersion() 내부에서 BatchCommit() 또는 BatchRollback()이 수행됨
                        if (mgr.DeleteSOPVersion(m_dbMgr, m_selectedVersion.VersionID, true, true))
                        {
                            #region 열려있는 버전이면 닫기
                            string szCategoryName = m_prevSelectedNode.Parent.Parent.Text;
                            string szSubCategoryName = m_prevSelectedNode.Parent.Text;
                            string szDisasterName = m_prevSelectedNode.Text;

                            bool bEqualCategoryName = SopDocManager.Instance.CategoryName == szCategoryName;
                            bool bEqualSubCategoryName = SopDocManager.Instance.SubCategoryName == szSubCategoryName;
                            bool bEqualDisasterName = SopDocManager.Instance.DisasterName == szDisasterName;
                            bool bEqualVersionID = (FormMain.Instance.CurrentVersion != null && FormMain.Instance.CurrentVersion.VersionID == nVersionID);

                            if (bEqualCategoryName && bEqualSubCategoryName && bEqualDisasterName && bEqualVersionID)
                            {
                                FormMain.Instance.CloseSOP();
                            }
                            #endregion

                            InitTree();

                            if (szPreSelectedNodePath != "")
                            {
                                if (!SelectNode(2, szPreSelectedNodePath))
                                {
                                    dataGridViewVersion.ClearSelection();
                                    dataGridViewVersion.Rows.Clear();
                                    SetVersionInfo(null);
                                }
                            }

                            if (SelectNode(0, szPreSelectedNodePath))
                            {
                                // 다른 버전이 있는 경우					
                                TreeNode node = treeViewSOP.SelectedNode;
                                treeViewSOP.SelectedNode = node;
                            }
                            else
                            {
                                // 더이상 같은 이름의 재난이 없는 경우
                                if (m_prevSelectedNode != null)
                                {
                                    TreeNode node = m_prevSelectedNode.Parent;
                                    if (node != null)
                                    {
                                        // 현재 사용중인 재난 타입이 아니면
                                        if (node.Text != SopDocManager.Instance.DisasterName)
                                        {
                                            // 재난 타입 삭제
                                            try
                                            {
                                                int nSubID = (int)node.Tag;
                                                string szSQL1 = string.Format("delete from DisasterType where name= '{0}' and SubDisasterID = {1}", m_selectedDisasterName, nSubID);

                                                m_dbMgr.GetResultData(szSQL1, 0);
                                            }
                                            catch (Exception)
                                            {
                                            }
                                        }
                                    }
                                }
                            }

                            FormMain.Instance.SaveLastAccessedSOPTime(DateTime.Now);
                        }
                    }
                }
            }
        }
	 
        private TreeNode FindTreeNode(string strText, TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Text == strText)
                    return node;
            }
            return null;
        }

        private void ResetTree()
        {
            m_prevSelectedNode = null;
            treeViewSOP.Nodes.Clear();

            Dictionary<string, ArrayList> dicSOP = GetSOPDictionary(radioRegular.Checked, radioNormal.Checked);

            // DisasterID, Version
            Dictionary<int, VersionInfo> dicVersion = GetVersionDictionary(radioRegular.Checked, radioNormal.Checked);
            
            foreach (KeyValuePair<string, ArrayList> pair in dicSOP)
            {
                string strFullPath = pair.Key;

                int nIndex1 = strFullPath.IndexOf(m_chDelimeter);
                int nIndex2 = strFullPath.LastIndexOf(m_chDelimeter);
                if (nIndex1 < 0 || nIndex2 < 0) continue;

                string strCategoryName = strFullPath.Substring(0, nIndex1);
                string strSubCategoryName = strFullPath.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                string strDisasterName = strFullPath.Substring(nIndex2 + 1);


				int nCategoryID = GetCategoryID(strCategoryName);
				int nSubCategoryID = GetSubCategoryID(strSubCategoryName);

                TreeNode nodeCategory = FindTreeNode(strCategoryName, treeViewSOP.Nodes);

				if (nodeCategory == null)
				{
                    int nIdx = SOPCategory.GetSubCategoryImageIndex(strCategoryName, strCategoryName);
					//int nIdx = SetSubCategoryImage(strCategoryName);
                    int nTreeIdx = GetSOPTreeIndex(strCategoryName);
                    nodeCategory = treeViewSOP.Nodes.Insert(nTreeIdx, strCategoryName, strCategoryName, nIdx, nIdx);
					nodeCategory.Tag = nCategoryID;
				}

                TreeNode nodeSubCategory = FindTreeNode(strSubCategoryName, nodeCategory.Nodes);
                if (nodeSubCategory == null)
				{
                    int nIdx = SOPCategory.GetSubCategoryImageIndex(strCategoryName, strSubCategoryName);
					//int nIdx = SetSubCategoryImage(strSubCategoryName);
					nodeSubCategory = nodeCategory.Nodes.Add(strSubCategoryName, strSubCategoryName, nIdx, nIdx);
					nodeSubCategory.Tag = nSubCategoryID;
				}                   

                TreeNode nodeDisaster = FindTreeNode(strDisasterName, nodeSubCategory.Nodes);
				if (nodeDisaster == null)
				{
                    int nIdx = SOPCategory.GetSubCategoryImageIndex(strCategoryName, strSubCategoryName);
                    //int nIdx = SetSubCategoryImage(strSubCategoryName);
					nodeDisaster = nodeSubCategory.Nodes.Add(strDisasterName, strDisasterName, nIdx, nIdx);
				}

                if (nodeDisaster.Tag == null)
                    AddActionStep(nodeDisaster, pair.Value, dicVersion);
            }

            treeViewSOP.ExpandAll();

        }

        private void InsertArray(ArrayList arrSrc, ArrayList arrTrg)
        {
            foreach (object obj in arrSrc)
            {
                arrTrg.Add(obj);
            }
        }

        private void AddActionStep(TreeNode nodeDisaster, ArrayList arrDisaster, Dictionary<int, VersionInfo> dicVersion)
        {
            DateTime dtMaxBegin = new DateTime();
            ArrayList arrActionSteps = null;
            int nDisasterID = -1;

            int nDisasterCount = arrDisaster.Count;

            for (int i = 0; i < nDisasterCount; i++)
            {
                DisasterInfo disaster = (DisasterInfo)arrDisaster[i];
                
                if (!dicVersion.ContainsKey(disaster.DisasterID))
                    continue;

                VersionInfo version = dicVersion[disaster.DisasterID];

                if (i == 0)
                {
                    dtMaxBegin = version.BeginTime;
                    arrActionSteps = disaster.ActionSteps;
                    nDisasterID = disaster.DisasterID;
                }
                else
                {
                    if (dtMaxBegin < version.BeginTime)
                    {
                        dtMaxBegin = version.BeginTime;
                        arrActionSteps = disaster.ActionSteps;
                        nDisasterID = disaster.DisasterID;
                    }
                }
            }

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
                        TreeNode nodeStep = node.Nodes.Add(actionStep.ActionStepName, actionStep.ActionStepName, node.ImageIndex, node.SelectedImageIndex);
                        nodeStep.Tag = actionStep.ActionStepID;
                    }
                    else
                    {
                        TreeNode nodeParent = FindNode(actionStep.ParentStepID, node);
                        if (nodeParent != null)
                        {

                            TreeNode nodeStep = nodeParent.Nodes.Add(actionStep.ActionStepName, actionStep.ActionStepName, node.ImageIndex, node.SelectedImageIndex);
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

        private TreeNode FindNode(int nTag, TreeNode nodeParent)
        {
            if (nodeParent == null)
                return null;

            foreach (TreeNode node in nodeParent.Nodes)
            {
                if (node.Tag != null && (int)node.Tag == nTag)
                    return node;

                TreeNode result = FindNode(nTag, node);
                if (result != null)
                    return result;
            }

            return null;
        }

        private void LoadVersion()
        {
            m_dicVersionRegularNormal.Clear();
            m_dicVersionRegularEmergency.Clear();
            m_dicVersionNonRegularNormal.Clear();
            m_dicVersionNonRegularEmergency.Clear();
            m_dicVersionInfo.Clear();
            
            // Site별로 사용할 수 있도록 Query 수정 , Edit by skkim 2015.01.08
            string szText = "SELECT v.ID, v.VersionName, v.isRegular, v.isNormal, s.MemberID, s.UserID, v.CreateTime, v.LastAccessTime, v.Description, d.ID ";
            szText += "FROM Version as v, SOPGenUser as s, Disaster as d ";
            szText += string.Format("WHERE v.SiteID = {0} AND v.OwnerID = s.ID AND v.ID = d.VersionID ORDER BY v.CreateTime", FormMain.Instance.SiteID);

            string strSQL = string.Format(szText, FormMain.Instance.SiteID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            DateTime dtDefault = new DateTime();

            int nCount = arrResult.Count;

            for (int i = 0; i < nCount - 9; i += 10)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				string strVersionName = WebDBManager.GetStringField(arrResult[i + 1], "");
				bool isRegular = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0) == 0 ? false : true;
				bool isNormal = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0) == 0 ? false : true;
                int nCompanyMemberID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                string strUserID = WebDBManager.GetStringField(arrResult[i + 5], "");
				//string strMemberName = WebDBManager.GetStringField(arrResult[i + 4], "");
				DateTime dtBegin = WebDBManager.GetDateTimeField(arrResult[i + 6], dtDefault);
				DateTime dtEnd = WebDBManager.GetDateTimeField(arrResult[i + 7], dtDefault);
				string strDesc = WebDBManager.GetStringField(arrResult[i + 8], "");
				int nDisasterID = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 0);

                string strMemberName = GetCompanyMemberName(nCompanyMemberID, 0, strUserID);

                if (strMemberName == null)
                    continue;

                VersionInfo version = new VersionInfo();

                version.VersionID = nID;
                version.VersionName = strVersionName;
                version.UserName = strMemberName;
                version.BeginTime = dtBegin;
                version.EndTime = dtEnd;
                version.Description = strDesc;

                Dictionary<int, VersionInfo> dicVersion = GetVersionDictionary(isRegular, isNormal);
                dicVersion[nDisasterID] = version;

                m_dicVersionInfo[nID] = version;
            }
        }

        private string GetCompanyMemberName(int nCompanyMemberID, int nTransaction, string strUserID)
        {
            if (nCompanyMemberID < 0)
                return strUserID;// "직원정보 없음";

            string strSQL = "Select MemberName from CompanyMember where ID = " + nCompanyMemberID.ToString();
            ArrayList arrResult = nTransaction != 0 ? m_dbMgr.GetBatchData(strSQL) : m_dbMgr.GetResultData(strSQL, 0);// m_dbMgr.GetResultData(strSQL, nTransaction);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            return WebDBManager.GetStringField(arrResult[0], "");
        }

        private void SortDisasterArray(Dictionary<string, ArrayList> dicSOP)
        {
            DisasterCompare cmp = new DisasterCompare();

            foreach (KeyValuePair<string, ArrayList> pair in dicSOP)
            {
                ArrayList arrDisasters = pair.Value;
                arrDisasters.Sort(cmp);
            }
        }

        private void LoadSOP()
        {
            m_dicSOPRegularNormal.Clear();
            m_dicSOPRegularEmergency.Clear();
            m_dicSOPNonRegularNormal.Clear();
            m_dicSOPNonRegularEmergency.Clear();

            //string strSQL = "select disaster.id, disaster.DisasterName, sc.SubCategoryName, dc.CategoryName, disaster.VersionID, Version.isRegular, Version.isNormal from disaster, SubDisasterCategory as sc, DisasterCategory as dc, Version ";
            //strSQL += "where disaster.SubDisasterID = sc.id and sc.DisasterID = dc.id and disaster.VersionID = Version.ID order by DisasterName";

            // Site별로 사용할 수 있도록 Query 수정 , Edit by skkim 2015.01.08
            string szText = "SELECT d.id, d.DisasterName, sc.SubCategoryName, dc.CategoryName, d.VersionID, v.isRegular, v.isNormal " +
                            " FROM disaster as d, SubDisasterCategory as sc, DisasterCategory as dc, Version as v " +
                            " WHERE dc.SiteID = {0} AND  d.SubDisasterID = sc.id AND sc.DisasterID = dc.id AND d.VersionID = v.ID ORDER BY DisasterName";

            string strSQL = string.Format(szText, FormMain.Instance.SiteID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null) return;

            int nCount = arrResult.Count;
            if (nCount == 0) return;

            string strDisasterIDs = "";
            Dictionary<int, DisasterInfo> dicDisaster = new Dictionary<int, DisasterInfo>();

            for (int i = 0; i < nCount - 6; i += 7)
            {
				int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				string strDisasterName = WebDBManager.GetStringField(arrResult[i + 1], "");
				string strSubCategoryName = WebDBManager.GetStringField(arrResult[i + 2], "");
				string strCategoryName = WebDBManager.GetStringField(arrResult[i + 3], "");
				int nVersionID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
				bool isRegular = WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0) == 0 ? false : true;
				bool isNormal = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0) == 0 ? false : true;

                string strFullPath = strCategoryName + m_strDelimeter + strSubCategoryName + m_strDelimeter + strDisasterName;
                DisasterInfo disaster = new DisasterInfo();
                dicDisaster[nID] = disaster;

                ArrayList arrDisasters = null;
                Dictionary<string, ArrayList> dicSOP = GetSOPDictionary(isRegular, isNormal);

                if (dicSOP.ContainsKey(strFullPath))
                    arrDisasters = dicSOP[strFullPath];
                else
                {
                    arrDisasters = new ArrayList();
                    dicSOP[strFullPath] = arrDisasters;
                }

                arrDisasters.Add(disaster);

                disaster.DisasterID = nID;
                disaster.VersionID = nVersionID;

                if (strDisasterIDs.Length == 0)
                    strDisasterIDs = nID.ToString();
                else
                    strDisasterIDs += ", " + nID.ToString();
            }

            if (strDisasterIDs.Length == 0)
                return;

			//테이블 추가	
            strSQL = string.Format("select ID, StepName, DisasterID, ParentStepID, UserDefinedConfigID from ActionStep where DisasterID in ({0})", strDisasterIDs);
            //strSQL = string.Format("select ID, StepName, DisasterID, ParentStepID from ActionStep where DisasterID in ({0})", strDisasterIDs);
            arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null) return;

            nCount = arrResult.Count;

            for (int i = 0; i < nCount - 4; i += 5)
            //for (int i = 0; i < nCount - 3; i += 4)
            {
				int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				string strStepName = WebDBManager.GetStringField(arrResult[i + 1], "");
				int nDisasterID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
				int nParentStepID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                //
                VariousData<int> userDefinedConfigID = WebDBManager.GetIntField(arrResult[i + 4].ToString());

                if (!dicDisaster.ContainsKey(nDisasterID))
                    continue;

                ActionStepInfo actionStep = new ActionStepInfo();
                actionStep.ActionStepID = nID;
                actionStep.ActionStepName = strStepName;
                actionStep.ParentStepID = nParentStepID;

                if (userDefinedConfigID != null)
                    actionStep.UserDefinedConfig = LoadUserDefinedConfig(userDefinedConfigID.Data);

                DisasterInfo disaster = dicDisaster[nDisasterID];
                disaster.ActionSteps.Add(actionStep);
            }

            DisasterCompare.m_dicVersion = GetVersionDictionary(radioRegular.Checked, radioNormal.Checked);

            SortDisasterArray(m_dicSOPRegularNormal);
            SortDisasterArray(m_dicSOPRegularEmergency);
            SortDisasterArray(m_dicSOPNonRegularNormal);
            SortDisasterArray(m_dicSOPNonRegularEmergency);
        }

        private ConfigData LoadUserDefinedConfig(int nID)
        {
            string strSQL = "Select ConfigName, VariableName, VariableType, uv.Description ";
            strSQL += "from UserDefinedConfigVariable as uv, UserDefinedConfig as uc ";
            strSQL += "where ConfigID = " + nID.ToString() + " and uc.ID = uv.ConfigID order by No";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            ConfigData data = new ConfigData(ConfigData.ConfigType.FILE);
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-3;i+=4)
            {
                string strConfigName = WebDBManager.GetStringField(arrResult[i]);
                string strVariableName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> variableType = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                string strDescription = WebDBManager.GetStringField(arrResult[i + 3]);

                if (strConfigName == null || strVariableName == null)
                    continue;

                data.Text = strConfigName;

                Sections.SectionDataDecision.VariableType type = Sections.SectionDataDecision.ToVariableType(variableType.Data);

                if (type == Sections.SectionDataDecision.VariableType.UNKNOWN)
                    continue;

                SOPParameter param = new SOPParameter();
                param.VariableName = strVariableName;
                param.Type = type;
                param.Description = strDescription != null ? strDescription : "";

                data.Variables.Add(param);
            }

            return data;
        }

        private ArrayList GetActionSteps(ArrayList arrDisasters, int nVersionID)
        {
            if (arrDisasters == null)
                return null;

            foreach (DisasterInfo disaster in arrDisasters)
            {
                if (disaster.VersionID == nVersionID)
                    return disaster.ActionSteps;
            }

            return null;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (m_prevSelectedNode != null)
            {
                m_selectedCategoryName = m_prevSelectedNode.Parent.Parent.Text;
                m_selectedSubCategoryName = m_prevSelectedNode.Parent.Text;
                m_selectedDisasterName = m_prevSelectedNode.Text;

                Dictionary<string, ArrayList> dicSOP = GetSOPDictionary(radioRegular.Checked, radioNormal.Checked);
                Dictionary<int, VersionInfo> dicVersion = GetVersionDictionary(radioRegular.Checked, radioNormal.Checked);

                string strFullPath = m_selectedCategoryName + m_strDelimeter + m_selectedSubCategoryName + m_strDelimeter + m_selectedDisasterName;
                ArrayList arrDisasters = null;

                if (dicSOP.ContainsKey(strFullPath))
                    arrDisasters = dicSOP[strFullPath];

                m_selectedVersion = null;
                m_arrSelectedActionSteps = null;

                if (dataGridViewVersion.SelectedCells.Count == 0)
                {
					UnE.Utility.UMessageBoxRibbon.Show("SOP 버전을 선택해 주세요", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                int nVersionID = (int)dataGridViewVersion.Rows[dataGridViewVersion.SelectedCells[0].RowIndex].Tag;
                m_arrSelectedActionSteps = GetActionSteps(arrDisasters, nVersionID);

                if (m_dicVersionInfo.ContainsKey(nVersionID))
                    m_selectedVersion = m_dicVersionInfo[nVersionID];
                else
                    m_selectedVersion = null;
            
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
				UnE.Utility.UMessageBoxRibbon.Show("Tree에서 재난 상세 정의를 선택하세요\r\nTree에서 세 번째 단계", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // DisasterID, Version
        private Dictionary<int, VersionInfo> GetVersionDictionary(bool isRegular, bool isNormal)
        {
            if (isRegular)
            {
                if (isNormal)
                    return m_dicVersionRegularNormal;
                else
                    return m_dicVersionRegularEmergency;
            }
            else
            {
                if (isNormal)
                    return m_dicVersionNonRegularNormal;
            }

            return m_dicVersionNonRegularEmergency;
        }

        private Dictionary<string, ArrayList> GetSOPDictionary(bool isRegular, bool isNormal)
        {
            if (isRegular)
            {
                if (isNormal)
                    return m_dicSOPRegularNormal;
                else
                    return m_dicSOPRegularEmergency;
            }
            else
            {
                if (isNormal)
                    return m_dicSOPNonRegularNormal;
            }

            return m_dicSOPNonRegularEmergency;
        }

        private TreeNode Get2LevelNode(TreeNode node, int nNodeLevel)
        {
            for (int i = nNodeLevel - 2; i > 0; i--)
            {
                node = node.Parent;
            }

            return node;
        }

        private void treeViewSOP_AfterSelect(object sender, TreeViewEventArgs e)
		{
			TreeNode node = treeViewSOP.SelectedNode;
			if (node == null) return;

			// Disaster
			if (node.Level < 2)
				return;
			else
				node = Get2LevelNode(node, node.Level);

			if (m_prevSelectedNode == node)
				return;

			m_prevSelectedNode = node;

            string strFullPath = node.Parent.Parent.Text + m_strDelimeter + node.Parent.Text + m_strDelimeter + node.Text;
			Dictionary<string, ArrayList> dicSOP = GetSOPDictionary(radioRegular.Checked, radioNormal.Checked);

			if (!dicSOP.ContainsKey(strFullPath))
				return;

			ArrayList arrDisasters = dicSOP[strFullPath];
			ResetGrid(strFullPath, arrDisasters);
		}

        private void ResetGrid(string strFullPath, ArrayList arrDisasters)
        {
            m_nPrevSelectedRow = -1;

            int nRowCount = dataGridViewVersion.RowCount;

            for (int i = 0; i < nRowCount; i++)
            {
                dataGridViewVersion.Rows.RemoveAt(0);
            }

            // DisasterID, Version
            Dictionary<int, VersionInfo> dicVersion = GetVersionDictionary(radioRegular.Checked, radioNormal.Checked);
            dataGridViewVersion.Tag = strFullPath;

            // 생성 날짜순으로 정렬
            DisasterCompare.m_dicVersion = dicVersion;
            arrDisasters.Sort(new DisasterCompare());

            foreach (DisasterInfo disaster in arrDisasters)
            {
                if (!dicVersion.ContainsKey(disaster.DisasterID))
                    continue;

                VersionInfo version = dicVersion[disaster.DisasterID];

                DataGridViewRow row = new DataGridViewRow();                

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                
				cell.Value = version.VersionName + " " + version.UserName;
                row.Cells.Add(cell);

				row.Height = 40;

                row.Tag = version;
                dataGridViewVersion.Rows.Add(row);
            }

            dataGridViewVersion.ClearSelection();
            nRowCount = dataGridViewVersion.RowCount;

			if (nRowCount >= 1)
			{
				dataGridViewVersion.ClearSelection();

				dataGridViewVersion.Rows[0].Selected = true;
				OnSelectedGrid(0);
			}
			else
			{
				SetVersionInfo(null);
			}
        }

		private void SetVersionInfo(VersionInfo info)
		{
			textSOPInfo.Clear();

			if (info != null)
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("");
				sb.AppendLine("  버전명 : " + info.VersionName);
				sb.AppendLine("  작성자 : " + info.UserName);
				sb.AppendLine("  생성일자 : " + info.BeginTime.ToLongDateString() + " " + info.BeginTime.ToLongTimeString());
				sb.AppendLine("  수정일자 : " + info.EndTime.ToLongDateString() + " " + info.EndTime.ToLongTimeString());
				sb.AppendLine("  부가설명 : " + info.Description);
				textSOPInfo.Text = sb.ToString();
			}			
		}

        private void OnSelectedGrid(int nSelectedRowIndex)
        {
            if (nSelectedRowIndex < 0)
                return;

            if (m_nPrevSelectedRow == nSelectedRowIndex)
                return;

            if (m_prevSelectedNode == null)
                return;

            m_nPrevSelectedRow = nSelectedRowIndex;
			VersionInfo info = (VersionInfo)dataGridViewVersion.Rows[nSelectedRowIndex].Tag;
            int nVersionID = info.VersionID;

            string strFullPath = (string)dataGridViewVersion.Tag;
            Dictionary<string, ArrayList> dicSOP = GetSOPDictionary(radioRegular.Checked, radioNormal.Checked);

            if (!dicSOP.ContainsKey(strFullPath))
                return;

            ArrayList arrDisasters = dicSOP[strFullPath];

            foreach (DisasterInfo disaster in arrDisasters)
            {
                if (disaster.VersionID == nVersionID)
                {
                    m_prevSelectedNode.Nodes.Clear();

                    AddActionStep(m_prevSelectedNode, disaster.ActionSteps);

                    treeViewSOP.SelectedNode = m_prevSelectedNode;
                    m_prevSelectedNode.ExpandAll();
                    break;
                }
            }
			SetVersionInfo(info);
        }
		     

		private void OpenSOP()
		{
			FormMain.Instance.UseWaitCursor = true;
			Cursor.Current = Cursors.WaitCursor;
			FormMain.Instance.OpenSOP(this);
			Cursor.Current = Cursors.Default;
			FormMain.Instance.UseWaitCursor = false;
		}

		private void dataGridViewVersion_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
		}

		private int GetCategoryID(string strCategory)
		{
			foreach (Data_DisasterCategory data in FormMain.Instance.DisasterCategory)
			{
				if (data.CategoryName == strCategory)
				{
					return data.ID;
				}
			}
			return 0;
		}

		private int GetSubCategoryID(string strSubCategory)
		{
			foreach (Data_SubDisasterCategory data in FormMain.Instance.SubDisasterCategory)
			{
				if (data.CategoryName == strSubCategory)
				{
					return data.ID;
				}
			}
			return 0;
		}

        private void treeViewSOP_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            btnOpenSOP_Click(null, null);
        }

    }

    public class DisasterInfo
    {
        private int m_nDisasterID = -1;
        private int m_nVersionID = -1;
        private ArrayList m_arrActionSteps = new ArrayList();

        public int DisasterID
        {
            get { return m_nDisasterID; }
            set { m_nDisasterID = value; }
        }

        public int VersionID
        {
            get { return m_nVersionID; }
            set { m_nVersionID = value; }
        }

        public ArrayList ActionSteps
        {
            get { return m_arrActionSteps; }
        }
    }

    public class ActionStepInfo
    {
        private int m_nActionStepID = -1;
        private string m_strActionStepName = "";
        private int m_nParentStepID = -1;
        private ConfigData m_userDefinedConfig = null;

        public int ActionStepID
        {
            get { return m_nActionStepID; }
            set { m_nActionStepID = value; }
        }

        public string ActionStepName
        {
            get { return m_strActionStepName; }
            set { m_strActionStepName = value; }
        }

        public int ParentStepID
        {
            get { return m_nParentStepID; }
            set { m_nParentStepID = value; }
        }

        public ConfigData UserDefinedConfig
        {
            get { return m_userDefinedConfig; }
            set { m_userDefinedConfig = value; }
        }
    }

    public class VersionInfo
    {
        private int m_nVersionID = -1;
        private string m_strVersionName = "";
        private string m_strUserName = "";
        private DateTime m_dtBegin;
        private DateTime m_dtEnd;
        private string m_strDescription = "";
        
        public int VersionID
        {
            get { return m_nVersionID; }
            set { m_nVersionID = value; }
        }

        public string VersionName
        {
            get { return m_strVersionName; }
            set { m_strVersionName = value; }
        }

        public string UserName
        {
            get { return m_strUserName; }
            set { m_strUserName = value; }
        }

        public DateTime BeginTime
        {
            get { return m_dtBegin; }
            set { m_dtBegin = value; }
        }

        public DateTime EndTime
        {
            get { return m_dtEnd; }
            set { m_dtEnd = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }
    }

    public class DisasterCompare : IComparer
    {
        public static Dictionary<int, VersionInfo> m_dicVersion = null;

        int IComparer.Compare(Object obj1, Object obj2)
        {
            if (m_dicVersion == null)
                return 0;

            DisasterInfo disaster1 = (DisasterInfo)obj1;
            DisasterInfo disaster2 = (DisasterInfo)obj2;

            if (!m_dicVersion.ContainsKey(disaster1.DisasterID))
                return 0;
            if (!m_dicVersion.ContainsKey(disaster2.DisasterID))
                return 0;

            VersionInfo version1 = m_dicVersion[disaster1.DisasterID];
            VersionInfo version2 = m_dicVersion[disaster2.DisasterID];

            if (version1.BeginTime == version2.BeginTime)
                return 0;

            return version1.BeginTime > version2.BeginTime ? -1 : 1;
        }

	}
}
