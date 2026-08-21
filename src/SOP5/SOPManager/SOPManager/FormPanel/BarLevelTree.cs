using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SOPManager
{
	public partial class BarLevelTree : Form
	{
        ImageList arImageList;

		public BarLevelTree()
		{
			InitializeComponent();

			//ImageList arImageList = new ImageList();
            arImageList = new ImageList();
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
			arImageList.Images.Add(global::SOPManager.Properties.Resources.btnEtc_User);
			arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_terror);
			arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_terror);
			arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_terror);
			arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_terror);
			arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_terror);
			arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_strongwind);
            arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_security);
            arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_security);
            arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_security);
            arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_security);
            arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_security);
            arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_security);
            arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_security);
            arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_security);
            arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_security);
            arImageList.Images.Add(global::SOPManager.Properties.Resources.btn_sub_security);*/
			this.treeView.ImageList = arImageList;
		}

        /*private object[] m_arSubCategorys =
		{
			"인명구조 및 의료지원", "SOP상황", "자연재해", "태풍",	"자연재해", "지진", "자연재해", "폭설", "자연재해", "침수", "기타", "일반재해", "화재", "화재", "화재", "산불", 	"유출사고", "오염", "유출사고", "누출", "유출사고", "유출", "유출사고", "암모니아",
			"테러", "테러",	"테러", "폭발", "인명구조 및 의료지원", "119상황", "테러", "무장", "테러", "괴선박", "테러", "폭탄", "테러", "침입", "테러", "폭약", "자연재해", "자연재해",
            "방범", "침입", "방범", "배회", "방범", "쓰러짐", "방범", "도난", "방범", "방치", "방범", "가상펜스", "방범", "내부비상벨", "방범", "외부비상벨", "방범", "기타", "방범", "방범"
		};

        private int SetSubCategoryImage(string strCategoryName, string strSubCategoryName)
        {
            for (int i = 0; i < m_arSubCategorys.Length-1; i+=2)
            {
                if (strCategoryName == (string)m_arSubCategorys[i])
                {
                    if (strSubCategoryName == (string)m_arSubCategorys[i + 1] || strSubCategoryName.Contains((string)m_arSubCategorys[i + 1]))
                        return i / 2;
                }
            }
            return 0;
        }

		private object[] m_arSubCategorys =
		{
			"SOP상황", "태풍",	"지진", "폭설", "침수", "일반재해", "화재","산불", 	"오염", "누출","유출","암모니아",
			"테러",	"폭발", "119상황","무장", "괴선박", "폭탄", "침입", 	"폭약", "자연재해"
		};

		private int SetSubCategoryImage(string strValue)
		{
			for (int i = 0; i < m_arSubCategorys.Length; i++)
			{
				if (strValue == (string)m_arSubCategorys[i] || strValue.Contains((string)m_arSubCategorys[i]))
					return i;
			}
			return 0;
		}*/

        public void event_WinRateChanged()
        {
            double dWindowRateWidth = FormMain.Instance.WindowWidthRate;
            double dWindowRateHeight = FormMain.Instance.WindowHeightRate;

            double fLabelFontSize = label1.Font.Size * dWindowRateWidth;

            label1.Font = new Font(Program.prgFont, (float)fLabelFontSize, FontStyle.Bold);
            label2.Font = new Font(Program.prgFont, (float)fLabelFontSize, FontStyle.Bold);

            panelTop.Size = new Size(panelTop.Size.Width, (int)((float)panelTop.Size.Height * dWindowRateHeight));     
                        
            arImageList.Images.Clear();

            arImageList.ImageSize = new System.Drawing.Size((int)((double)arImageList.ImageSize.Width * dWindowRateWidth)
                                                                                      , (int)((double)arImageList.ImageSize.Height * dWindowRateHeight));

            arImageList.Images.AddRange(SOPCategory.GetSubCategoryIamgeList().ToArray());

            treeView.Location = new Point(0, panelTop.Size.Height);
            fLabelFontSize = treeView.Font.Size * dWindowRateWidth;
            treeView.Font = new Font(Program.prgFont, (float)fLabelFontSize, FontStyle.Bold);
            //treeView.Indent = (int)((float)treeView.Indent * dWindowRateHeight);
            //treeView.ItemHeight = (int)((float)treeView.ItemHeight * dWindowRateHeight);
            treeView.ImageList = arImageList;
  
            FormMain.Instance.UpdateWindowRate(treeContextMenu, dWindowRateWidth, dWindowRateHeight);
            FormMain.Instance.UpdateWindowRate(subCategoryContextMenu, dWindowRateWidth, dWindowRateHeight);
            FormMain.Instance.UpdateWindowRate(disasterContextMenu, dWindowRateWidth, dWindowRateHeight);
            FormMain.Instance.UpdateWindowRate(levelContextMenu, dWindowRateWidth, dWindowRateHeight);
        }

		public void ClearTree()
		{
			treeView.Nodes.Clear();
		}

		public bool CheckPathChildNode(TreeNode currentNode, string szTargetValue)
		{
			// 이미 자식 path에 포함되어 있는지 검사
			TreeNode node = FindNode(szTargetValue, currentNode.Nodes);
			if (node != null)
				return false;

			return true;
			// 부모노드에 포함되어 있는지 검사
			//return CheckPathParent(currentNode, szTargetValue);
		}

		private bool CheckPathParent(TreeNode currentNode, string szValue)
		{
			TreeNode parentNode = currentNode.Parent;
			if (parentNode.Parent == null || parentNode.Level <= 3)
				return true;

			if (parentNode.Text == szValue)
				return false;

			return CheckPathParent(parentNode, szValue);
		}

		public bool SetChildNode(TreeNode parentNode, TreeNode childNode)
		{
			if (parentNode == null || childNode == null)
				return false;

			if (!CheckPathChildNode(childNode, parentNode.Text))
				return false;

			TreeNode node = null;
			int nStep = 0;
			try
			{
				node = childNode.Parent;
				if (node != null)
				{
					node.Nodes.Remove(childNode);
				}
				nStep = 1;

				if (!parentNode.Nodes.Contains(childNode))
				{
					parentNode.Nodes.Add(childNode);
				}
				nStep = 2;
			}
			catch (System.Exception)
			{
			}
			if (nStep == 2)
			{
				parentNode.ExpandAll();
				return true;
			}

			if (nStep == 1)
			{
				childNode = node;
			}
			return false;
		}

		// Return 값 : strLevelName에 해당하는 노드를 리턴
		public TreeNode AddTreeNode(string strCategory = null, string strSubCategory = null, string strDetailCategory = null, string strLevelName = null)
		{
			if (strCategory == null)
				strCategory = SopDocManager.Instance.CategoryName;
			if (strSubCategory == null)
				strSubCategory = SopDocManager.Instance.SubCategoryName;
			if (strDetailCategory == null)
				strDetailCategory = SopDocManager.Instance.DisasterName;
			if (strLevelName == null)
				strLevelName = FormMain.Instance.GetPageLevel().GetTabPageName();

			if (strCategory == "" || strSubCategory == "" || strDetailCategory == "")
				return null;

			TreeNode child = FindNode(strCategory, treeView.Nodes);
			if (child == null)
			{
                int nIdx = SOPCategory.GetSubCategoryImageIndex(strCategory, strSubCategory);
                //int nIdx = SetSubCategoryImage(strCategory, strCategory);

				UnE.Controls.SOPNode node = new UnE.Controls.SOPNode(strCategory);
				node.TypeText = "Category";
				node.ImageIndex = nIdx;
				node.SelectedImageIndex = nIdx;

				treeView.Nodes.Add(node);
				child = node;
			}

			TreeNode second = FindNode(strSubCategory, child.Nodes);
			if (second == null)
			{
                int nIdx = SOPCategory.GetSubCategoryImageIndex(strCategory, strSubCategory);
                //int nIdx = SetSubCategoryImage(strCategory, strSubCategory);

				UnE.Controls.SOPNode node = new UnE.Controls.SOPNode(strSubCategory);
				node.TypeText = "SubCategory";
				node.ImageIndex = nIdx;
				node.SelectedImageIndex = nIdx;

				child.Nodes.Add(node);
				second = node;
			}

			TreeNode detail = FindNode(strDetailCategory, second.Nodes);
			if (detail == null)
			{
                int nIdx = SOPCategory.GetSubCategoryImageIndex(strCategory, strSubCategory);
                //int nIdx = SetSubCategoryImage(strCategory, strSubCategory);

				UnE.Controls.SOPNode node = new UnE.Controls.SOPNode(strDetailCategory);
				node.TypeText = "Disaster";
				node.ImageIndex = nIdx;
				node.SelectedImageIndex = nIdx;

				second.Nodes.Add(node);
				detail = node;
			}

			if (strLevelName == "")
			{
				treeView.ExpandAll();
				return null;
			}

			TreeNode level = FindNode(strLevelName, detail.Nodes);
			if (level == null)
			{
                int nIdx = SOPCategory.GetSubCategoryImageIndex(strCategory, strSubCategory);
                //int nIdx = SetSubCategoryImage(strCategory, strSubCategory);

				UnE.Controls.SOPNode node = new UnE.Controls.SOPNode(strLevelName);
                node.TypeText = "Level";
                node.ImageIndex = nIdx;
				node.SelectedImageIndex = nIdx;;
                int nIndex = GetActionStepInsertIndex(node.Text, detail.Nodes);
                detail.Nodes.Insert(nIndex, node);
				//detail.Nodes.Add(node);
				level = node;
			}

			treeView.ExpandAll();

			SelectNode(level);

			ArrayList arList = FormMain.Instance.GetPageLevel().GetTabPage();
			foreach (TabPage page in arList)
			{
				if (page.Text == level.Text)
				{
					TabControl control = (TabControl)page.Parent;
					if (control != null)
					{
						control.SelectedTab = null;
						control.SelectedTab = page;
					}
				}
			}
			return level;
		}

        private int GetActionStepInsertIndex(string strNewStepName, TreeNodeCollection nodes)
        {
            List<string> actionStepNames = new List<string>();

            foreach (TreeNode node in nodes)
            {
                actionStepNames.Add(node.Text);
            }

            return Data_ActionStep.GetActionStepIndex(strNewStepName, actionStepNames);
        }

		public TreeNode FindNode(string strValue, TreeNodeCollection parentNodes = null, int nSearchLevel = 0)
		{
			TreeNodeCollection nodes = parentNodes == null ? treeView.Nodes : parentNodes;

			foreach (TreeNode node in nodes)
			{
				if (node.Level >= nSearchLevel)
				{
					if (strValue == node.Text)
						return node;
				}
				
				TreeNode result = FindNode(strValue, node.Nodes);
				if (result != null)
					return result;
			}

			return null;
		}

		public void RemoveTreeNode(string strValue)
		{
			TreeNode node = FindNode(strValue, treeView.Nodes);
			if (node != null)
			{
				TreeNode node2 = FindNode(strValue, node.Nodes);
				if (node2 == null)
					node.Nodes.Remove(node);
			}
		}

		public void ChangeLevelText(string strValue)
		{
			TreeNode node = FindNode(FormMain.Instance.GetPageLevel().OldTabPageText, null);
			if (node != null)
				node.Text = strValue;
		}

		public void SelectNode(TreeNode node)
		{
			if (node == null)
				return;

			if (treeView.SelectedNode != null)
				treeView.SelectedNode.ForeColor = Color.Black;

			treeView.SelectedNode = node;
			node.ForeColor = Color.Red;
		}

		private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Action != TreeViewAction.Unknown)
			{
				TreeNode node = treeView.SelectedNode;
				if (node == null) return;

				node.ForeColor = Color.Red;

				ArrayList arList = FormMain.Instance.GetPageLevel().GetTabPage();
				foreach (TabPage page in arList)
				{
					if (page.Text == node.Text)
					{
						TabControl control = (TabControl)page.Parent;
						control.SelectedTab = page;
					}
				}
			}
		}

		private void treeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
		{
			TreeNode node = treeView.SelectedNode;
			if (node == null)
				return;
			node.ForeColor = Color.Black;
		}

		public bool ExistNode()
		{
			if (treeView.Nodes.Count > 0)
				return true;
			return false;
		}

		
		private void toolStripMenuItem1_Click(object sender, EventArgs e)
		{
            //FormDisaster disasterForm = new FormDisaster();
            FormNewSOP3 disasterForm = new FormNewSOP3(false);
            UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(disasterForm);

            frame.StartPosition = FormStartPosition.CenterScreen;
            frame.Text = "재난 관리";
            frame.ShowMaxButton = false;
            frame.ShowMinButton = false;
            frame.Sizable = false;
            if (frame.ShowDialog(FormMain.Instance) == DialogResult.OK)
			{
                SopDocManager.Instance.CategoryName = disasterForm.SelectedCategory;
                SopDocManager.Instance.SubCategoryName = disasterForm.SelectedSubCategory;
                SopDocManager.Instance.DisasterName = disasterForm.SelectedDetailCategory;

				TreeNode node = treeView.SelectedNode;
				if (node != null)
				{
                    foreach (TreeNode cnode in node.Nodes[0].Nodes[0].Nodes)
                    {
                        cnode.ImageIndex = SOPCategory.GetSubCategoryImageIndex(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                        cnode.SelectedImageIndex = SOPCategory.GetSubCategoryImageIndex(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                        //cnode.ImageIndex = SetSubCategoryImage(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                        //cnode.SelectedImageIndex = SetSubCategoryImage(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                    }

					node.Nodes[0].Nodes[0].Text = SopDocManager.Instance.DisasterName;
					node.Nodes[0].Nodes[0].ToolTipText = SopDocManager.Instance.DisasterName;
                    node.Nodes[0].Nodes[0].ImageIndex = SOPCategory.GetSubCategoryImageIndex(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                    node.Nodes[0].Nodes[0].SelectedImageIndex = SOPCategory.GetSubCategoryImageIndex(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);	
                    //node.Nodes[0].Nodes[0].ImageIndex = SetSubCategoryImage(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                    //node.Nodes[0].Nodes[0].SelectedImageIndex = SetSubCategoryImage(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);	

					node.Nodes[0].Text = SopDocManager.Instance.SubCategoryName;
					node.Nodes[0].ToolTipText = SopDocManager.Instance.SubCategoryName;
                    node.Nodes[0].ImageIndex = SOPCategory.GetSubCategoryImageIndex(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                    node.Nodes[0].SelectedImageIndex = SOPCategory.GetSubCategoryImageIndex(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);		
                    //node.Nodes[0].ImageIndex = SetSubCategoryImage(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                    //node.Nodes[0].SelectedImageIndex = SetSubCategoryImage(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);		

					node.Text = SopDocManager.Instance.CategoryName;
					node.ToolTipText = SopDocManager.Instance.CategoryName;
                    node.ImageIndex = SOPCategory.GetSubCategoryImageIndex(SopDocManager.Instance.CategoryName, SopDocManager.Instance.CategoryName);
                    node.SelectedImageIndex = SOPCategory.GetSubCategoryImageIndex(SopDocManager.Instance.CategoryName, SopDocManager.Instance.CategoryName);
                    //node.ImageIndex = SetSubCategoryImage(SopDocManager.Instance.CategoryName, SopDocManager.Instance.CategoryName);
                    //node.SelectedImageIndex = SetSubCategoryImage(SopDocManager.Instance.CategoryName, SopDocManager.Instance.CategoryName);

					FormMain.Instance.RefreshLevelProperties();
				}
			}
		}

		private void toolStripMenuItem2_Click(object sender, EventArgs e)
		{
			//FormDisaster disasterForm = new FormDisaster();
            FormNewSOP3 disasterForm = new FormNewSOP3(false);
            UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(disasterForm);
            frame.StartPosition = FormStartPosition.CenterScreen;
			frame.ShowMaxButton = false;
			frame.ShowMinButton = false;
			frame.Sizable = true;
			if (frame.ShowDialog(FormMain.Instance) == DialogResult.OK)
			{
				SopDocManager.Instance.CategoryName = disasterForm.SelectedCategory;
				SopDocManager.Instance.SubCategoryName = disasterForm.SelectedSubCategory;
				SopDocManager.Instance.DisasterName = disasterForm.SelectedDetailCategory;

				TreeNode node = treeView.SelectedNode;
				if (node != null)
				{

                    foreach (TreeNode cnode in node.Nodes[0].Nodes)
                    {
                        cnode.ImageIndex = SOPCategory.GetSubCategoryImageIndex(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                        cnode.SelectedImageIndex = SOPCategory.GetSubCategoryImageIndex(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                        //cnode.ImageIndex = SetSubCategoryImage(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                        //cnode.SelectedImageIndex = SetSubCategoryImage(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                    }

					node.Nodes[0].Text = SopDocManager.Instance.DisasterName;
					node.Nodes[0].ToolTipText = SopDocManager.Instance.DisasterName;
                    node.Nodes[0].ImageIndex = SOPCategory.GetSubCategoryImageIndex(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                    node.Nodes[0].SelectedImageIndex = SOPCategory.GetSubCategoryImageIndex(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                    //node.Nodes[0].ImageIndex = SetSubCategoryImage(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                    //node.Nodes[0].SelectedImageIndex = SetSubCategoryImage(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);

					node.Text = SopDocManager.Instance.SubCategoryName;
					node.ToolTipText = SopDocManager.Instance.SubCategoryName;
                    node.ImageIndex = SOPCategory.GetSubCategoryImageIndex(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                    node.SelectedImageIndex = SOPCategory.GetSubCategoryImageIndex(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);		
                    //node.ImageIndex = SetSubCategoryImage(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                    //node.SelectedImageIndex = SetSubCategoryImage(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);		

					node.Parent.Text = SopDocManager.Instance.CategoryName;
					node.Parent.ToolTipText = SopDocManager.Instance.CategoryName;
                    node.Parent.ImageIndex = SOPCategory.GetSubCategoryImageIndex(SopDocManager.Instance.CategoryName, SopDocManager.Instance.CategoryName);
                    node.Parent.SelectedImageIndex = SOPCategory.GetSubCategoryImageIndex(SopDocManager.Instance.CategoryName, SopDocManager.Instance.CategoryName);
                    //node.Parent.ImageIndex = SetSubCategoryImage(SopDocManager.Instance.CategoryName, SopDocManager.Instance.CategoryName);
                    //node.Parent.SelectedImageIndex = SetSubCategoryImage(SopDocManager.Instance.CategoryName, SopDocManager.Instance.CategoryName);

					FormMain.Instance.RefreshLevelProperties();
				}
			}
		}

		public void ChangeDisasterName(string szOrgName, string szNewName)
		{
			TreeNode node = FindNode(szOrgName, null);
			if (node != null)
			{
				UnE.Controls.SOPNode snode = (UnE.Controls.SOPNode)node;
				if (snode.TypeText == "Disaster")
				{
					snode.Text = szNewName;
					snode.ToolTipText = szNewName;
                    snode.ImageIndex = node.Parent.ImageIndex;// SetSubCategoryImage(szNewName);
                    snode.SelectedImageIndex = node.Parent.SelectedImageIndex;// SetSubCategoryImage(szNewName);
				}
			}

		}

        public bool ChangeActionStepName(string szOrgName, string szNewName)
        {
            TreeNode node = FindNode(szOrgName, null);
            TreeNode node2 = FindNode(szNewName, null);
            if (node2 != null)
                return false;

            if (node != null)
            {
                UnE.Controls.SOPNode snode = (UnE.Controls.SOPNode)node;
                if (snode.TypeText == "Level")
                {
                    snode.Text = szNewName;
                    snode.ToolTipText = szNewName;
                    snode.ImageIndex = SOPCategory.GetSubCategoryImageIndex(node.Parent.Text, szNewName);
                    snode.SelectedImageIndex = SOPCategory.GetSubCategoryImageIndex(node.Parent.Text, szNewName);
                    //snode.ImageIndex = SetSubCategoryImage(node2.Parent.Text, szNewName);
                    //snode.SelectedImageIndex = SetSubCategoryImage(node2.Parent.Text, szNewName);
                    return true;
                }
            }
            return false;
        }

		public void ChangeSubCategoryName(string szOrgName, string szNewName)
		{
			TreeNode node = FindNode(szOrgName, null, 2);
			if (node != null)
			{
				UnE.Controls.SOPNode snode = (UnE.Controls.SOPNode)node;
				if (snode.TypeText == "SubCategory")
				{
					snode.Text = szNewName;
					snode.ToolTipText = szNewName;
                    snode.ImageIndex = SOPCategory.GetSubCategoryImageIndex(node.Parent.Text, szNewName);
                    snode.SelectedImageIndex = SOPCategory.GetSubCategoryImageIndex(node.Parent.Text, szNewName);
                    //snode.ImageIndex = SetSubCategoryImage(node.Parent.Text, szNewName);
                    //snode.SelectedImageIndex = SetSubCategoryImage(node.Parent.Text, szNewName);
				}
			}
		}

        public void ChangeCategoryName(string szOrgName, string szNewName)
        {
            TreeNode node = FindNode(szOrgName, null, 0);
            if (node != null)
            {
                UnE.Controls.SOPNode snode = (UnE.Controls.SOPNode)node;
                if (snode.TypeText == "Category")
                {
                    snode.Text = szNewName;
                    snode.ToolTipText = szNewName;
                    snode.ImageIndex = SOPCategory.GetSubCategoryImageIndex(node.Text, szNewName);
                    snode.SelectedImageIndex = SOPCategory.GetSubCategoryImageIndex(node.Text, szNewName);
                    //snode.ImageIndex = SetSubCategoryImage(node.Parent.Text, szNewName);
                    //snode.SelectedImageIndex = SetSubCategoryImage(node.Parent.Text, szNewName);
                }
            }
        }


		private void changeDisasterMenuItem_Click(object sender, EventArgs e)
		{
			//FormDisaster disasterForm = new FormDisaster();
            FormNewSOP3 disasterForm = new FormNewSOP3(false);
            UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(disasterForm);
            frame.StartPosition = FormStartPosition.CenterScreen;
			frame.ShowMaxButton = false;
			frame.ShowMinButton = false;
			frame.Sizable = true;
			if( frame.ShowDialog(FormMain.Instance) == DialogResult.OK)
			{
				SopDocManager.Instance.CategoryName = disasterForm.SelectedCategory;
				SopDocManager.Instance.SubCategoryName = disasterForm.SelectedSubCategory;
				SopDocManager.Instance.DisasterName = disasterForm.SelectedDetailCategory;
				
				TreeNode node = treeView.SelectedNode;
				if( node != null)
				{

                    foreach(TreeNode cnode in node.Nodes)
                    {
                        cnode.ImageIndex = SOPCategory.GetSubCategoryImageIndex(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                        cnode.SelectedImageIndex = SOPCategory.GetSubCategoryImageIndex(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                        //cnode.ImageIndex = SetSubCategoryImage(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                        //cnode.SelectedImageIndex = SetSubCategoryImage(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);	
                    }

					node.Text = SopDocManager.Instance.DisasterName;
					node.ToolTipText = SopDocManager.Instance.DisasterName;
                    node.ImageIndex = SOPCategory.GetSubCategoryImageIndex(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                    node.SelectedImageIndex = SOPCategory.GetSubCategoryImageIndex(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                    //node.ImageIndex = SetSubCategoryImage(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                    //node.SelectedImageIndex = SetSubCategoryImage(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);					

					node.Parent.Text = SopDocManager.Instance.SubCategoryName;
					node.Parent.ToolTipText = SopDocManager.Instance.SubCategoryName;
                    node.Parent.ImageIndex = SOPCategory.GetSubCategoryImageIndex(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                    node.Parent.SelectedImageIndex = SOPCategory.GetSubCategoryImageIndex(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                    //node.Parent.ImageIndex = SetSubCategoryImage(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);
                    //node.Parent.SelectedImageIndex = SetSubCategoryImage(SopDocManager.Instance.CategoryName, SopDocManager.Instance.SubCategoryName);

					node.Parent.Parent.Text = SopDocManager.Instance.CategoryName;
					node.Parent.Parent.ToolTipText = SopDocManager.Instance.CategoryName;
                    node.Parent.Parent.ImageIndex = SOPCategory.GetSubCategoryImageIndex(SopDocManager.Instance.CategoryName, SopDocManager.Instance.CategoryName);
                    node.Parent.Parent.SelectedImageIndex = SOPCategory.GetSubCategoryImageIndex(SopDocManager.Instance.CategoryName, SopDocManager.Instance.CategoryName);
                    //node.Parent.Parent.ImageIndex = SetSubCategoryImage(SopDocManager.Instance.CategoryName, SopDocManager.Instance.CategoryName);
                    //node.Parent.Parent.SelectedImageIndex = SetSubCategoryImage(SopDocManager.Instance.CategoryName, SopDocManager.Instance.CategoryName);

					FormMain.Instance.RefreshLevelProperties();

                    FormPageSOP pageLevel = FormMain.Instance.GetPageLevel();

                    string szPath = SopDocManager.Instance.GetLevelPath();
                    pageLevel.GetPropertiesLevel().SetTitleText(szPath);
                    FormMain.Instance.SetTitleText(szPath);
				}
			}
		}

		private void treeView_MouseUp(object sender, MouseEventArgs e)
		{
			if( e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				TreeNode node = treeView.GetNodeAt(e.X, e.Y);
				if (node != null)
				{
					treeView.SelectedNode = node;
					UnE.Controls.SOPNode snode = (UnE.Controls.SOPNode)node;
					if (snode.TypeText == "Disaster")
					{
						disasterContextMenu.Show(treeView, e.Location);
					}
					else if (snode.TypeText == "SubCategory")
					{
						this.subCategoryContextMenu.Show(treeView, e.Location);
					}
					else if (snode.TypeText == "Category")
					{
						this.treeContextMenu.Show(treeView, e.Location);
					}
					else if( snode.TypeText == "Level")
					{
						this.levelContextMenu.Show(treeView, e.Location);
						levelContextMenu.Tag = node;
					}
				}
			}
		}

		private void addLevelMenuItem_Click(object sender, EventArgs e)
		{
			FormMain.Instance.AddLevel();
		}

		private void deleteLevelMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode node = (TreeNode)levelContextMenu.Tag;
			if( node != null)
			{
				FormMain.Instance.RemoveLevel(node.Text);
			}
			
		}

		private void levelContextMenu_Opening(object sender, CancelEventArgs e)
		{
			TreeNode node = treeView.SelectedNode;
			if( node != null)
			{
				string szValue = node.Text;
				if(leveMenuItem1.Text.Contains(szValue))
				{
					leveMenuItem1.Enabled = false;
				}
				else
				{
					leveMenuItem1.Enabled = true;
				}
				if (leveMenuItem2.Text.Contains(szValue))
				{
					leveMenuItem2.Enabled = false;
				}
				else
				{
					leveMenuItem2.Enabled = true;
				}
				if (leveMenuItem3.Text.Contains(szValue))
				{
					leveMenuItem3.Enabled = false;
				}
				else
				{
					leveMenuItem3.Enabled = true;
				}
				if (leveMenuItem4.Text.Contains(szValue))
				{
					leveMenuItem4.Enabled = false;
				}
				else
				{
					leveMenuItem4.Enabled = true;
				}
			}
		}

        public void SetWeekEnd(bool bWeek)
        {
            if (bWeek == true)
            {
                label2.Text = "주간 모드";
            }
            else
            {
                label2.Text = "야간 모드";
            }
            panelTop.Invalidate();

        }

        private void label2_Click(object sender, EventArgs e)
        {
           
        }

        private void label2_DoubleClick(object sender, EventArgs e)
        {
            //bool bWeek = SopDocManager.Instance.WeekMode;
            //SopDocManager.Instance.WeekMode = !bWeek;
        }

        public string GetCurrentCategoryName()
        {
            if (treeView.Nodes.Count == 0)
                return "";

            return treeView.Nodes[0].Text;
        }

        public string GetCurrentSubCategoryName()
        {
            if (treeView.Nodes.Count == 0)
                return "";

            TreeNode node = treeView.Nodes[0];

            if (node.Nodes.Count == 0)
                return "";

            return node.Nodes[0].Text;
        }
	}
}