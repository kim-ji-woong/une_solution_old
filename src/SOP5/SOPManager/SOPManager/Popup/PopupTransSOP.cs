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
    public partial class PopupTransSOP : Form
    {
		private ArrayList m_arrCategory = new ArrayList();
		private ArrayList m_arrSub = new ArrayList();
		private ArrayList m_arrDetail = new ArrayList();
		private ArrayList m_arrActionStep = new ArrayList();

		private int m_nRegular = 0;     // 0이면 비등록 모드
		private bool m_isNormal = true; // 평일 모드 인가?
		private bool m_isNormalOrigin = true;   // 초기값 저장

		private Sections.SectionTransSOP m_section = null;
		public Sections.SectionTransSOP Section
		{
			get { return m_section; }
			set 
			{
				m_section = value;		
				if( m_section != null)
					Init();
			}
		}

		private string m_szTitle = "";
		public string Title
		{
			get { return m_szTitle; }
		}

		private string m_szFullPath = "";
		public string FullPath
		{
			get { return m_szFullPath; }
		}

		private int m_nLinkedActionStepID = -1;
		public int LinkedActionStepID
		{
			get { return m_nLinkedActionStepID; }
		}

        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();

        public PopupTransSOP()
        {
            InitializeComponent();

            // Regular : true : 등록모드 false : 미등록모드

            string strRegular = "미등록모드";

			bool isRegular = SopDocManager.Instance.RegularMode;
			m_isNormal = SopDocManager.Instance.WeekMode;

            m_isNormalOrigin = m_isNormal;

            if (isRegular)
            {
                m_nRegular = 1;
                strRegular = "등록모드";
            }

            this.Text += string.Format("({0})", strRegular);
            
            ribbonButton1.Font = new System.Drawing.Font(Program.prgFont, 12f, FontStyle.Bold);
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

        private void Init()
        {
            int nLinkedActionStepID = -1;
            bool isLinkedNormal = !m_isNormal;

            if (m_section != null)
            {
                Sections.SectionDataTransSOP data = (Sections.SectionDataTransSOP)m_section.Data;
                nLinkedActionStepID = data.LinkedActionStepID;

                if (nLinkedActionStepID >= 0)
                {
                    string strSQL = string.Format("select Version.isNormal from ActionStep, Disaster, Version where ActionStep.DisasterID = Disaster.ID and Disaster.VersionID = Version.ID and ActionStep.ID = {0}",
                        nLinkedActionStepID);

                    WebDBManager dbMgr = FormMain.Instance.DBManager;
                    ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

                    if (arrResult == null || arrResult.Count < 1)
                        return;

					isLinkedNormal = WebDBManager.GetIntField(arrResult[0].ToString(), 0) == 0 ? false : true;
                }
            }

            if (isLinkedNormal)
            {
                radioNormal.Checked = true;
                radioAbnormal.Checked = false;
            }
            else
            {
                radioAbnormal.Checked = true;
                radioNormal.Checked = false;
            }

            m_arrCategory = FormMain.Instance.DisasterCategory;
            m_arrSub = FormMain.Instance.SubDisasterCategory;
            m_arrDetail = FormMain.Instance.DetailDisasterCategory;
            m_arrActionStep = FormMain.Instance.ActionStep;

            AddTreeNode(nLinkedActionStepID);


			SetRadioImage();
			radioAbnormal.Visible = false;
			radioNormal.Visible = false;
        }
		       
		private Data_Disaster FindDisaster(int nVersionID, ArrayList arrDisaster)
        {
            foreach (Data_Disaster data in arrDisaster)
            {
                if (data.VersionID == nVersionID)
                    return data;
            }
            return null;
        }

        // 최근의 평일, 야간 및 휴일 버전만 ArrayList에 담아 리턴
        private ArrayList GetLastVersion()
        {
            bool isWeekday = radioNormal.Checked;
            VersionInfo versionCurrent = isWeekday == m_isNormalOrigin ? FormMain.Instance.CurrentVersion : null;
            
            ArrayList arrVersion = new ArrayList();
            ArrayList arrDisasterVersion = new ArrayList();
            ArrayList arrSOPVersion = FormMain.Instance.SOPVersion;

			string strDetail = SopDocManager.Instance.DisasterName;

            foreach (Data_Disaster detail in m_arrDetail)
            {
                if (strDetail == detail.DisasterName)
                {
                    arrDisasterVersion.Add(detail);
                }
            }

            Data_Version trgVersion = null;
            int nNormal = isWeekday ? 1 : 0;

            DateTime dtWeekday = new DateTime();

            foreach (Data_Version data in arrSOPVersion)
            {
                if ((versionCurrent != null && versionCurrent.VersionID == data.ID) ||
                    (versionCurrent == null && FindDisaster(data.ID, arrDisasterVersion) != null))
                {
                    if (data.Regular == m_nRegular && data.Normal == nNormal)
                    {
                        if (dtWeekday < data.CreateTime)
                        {
                            trgVersion = data;
                            dtWeekday = data.CreateTime;
                        }
                    }
                }
            }

            if (trgVersion != null)
                arrVersion.Add(trgVersion);

            return arrVersion;
        }

        //// 최근의 평일, 야간 및 휴일 버전만 ArrayList에 담아 리턴
        //private ArrayList GetLastVersion()
        //{
        //    bool isWeekday = false;
        //    bool isWeekend = false;

        //    ArrayList arrVersion = new ArrayList();
        //    ArrayList arrDisasterVersion = new ArrayList();
        //    ArrayList arrSOPVersion = FormMain.Instance.SOPVersion;

        //    string strDetail = FormMain.Instance.GetPageDisaster().SelectedDetailCategory;
        //    foreach (Data_Disaster detail in m_arrDetail)
        //    {
        //        if (strDetail == detail.DisasterName)
        //        {
        //            arrDisasterVersion.Add(detail);
        //        }
        //    }

        //    Data_Version Weekend = null;
        //    Data_Version Weekday = null;

        //    DateTime dtWeekday = new DateTime();
        //    DateTime dtWeekend = new DateTime();
        //    foreach (Data_Disaster dataDetail in arrDisasterVersion)
        //    {
        //        foreach (Data_Version data in arrSOPVersion)
        //        {
        //            if (dataDetail.VersionID == data.ID)
        //            {
        //                if (data.Regular == m_nRegular)
        //                {
        //                    if (data.Normal == 1)
        //                    {
        //                        if (dtWeekday < data.CreateTime)
        //                        {
        //                            Weekday = data;
        //                            dtWeekday = data.CreateTime;
        //                            isWeekday = true;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (dtWeekend < data.CreateTime)
        //                        {
        //                            Weekend = data;
        //                            dtWeekend = data.CreateTime;
        //                            isWeekend = true;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    if (isWeekday)
        //        arrVersion.Add(Weekday);
        //    if (isWeekend)
        //        arrVersion.Add(Weekend);

        //    return arrVersion;
        //}

        private void AddTreeNode(int nLinkedActionStepID = -1)
        {
            ArrayList arrVersion = GetLastVersion();

            string strCategory = SopDocManager.Instance.CategoryName;
			string strSubCategory = SopDocManager.Instance.SubCategoryName;
			string strDetailCategory = SopDocManager.Instance.DisasterName;

            int nCategoryID = GetCategoryID(strCategory);
            int nSubCategoryID = GetSubCategoryID(strSubCategory);

            // Normal : 평일(1), 휴일 및 야간(0)
            int nWeekday = m_isNormal ? 1 : 0;

            treeView.Nodes.Clear();

            TabPage tabCurrent = null;
            
            if (m_isNormal == m_isNormalOrigin)
                tabCurrent = FormMain.Instance.GetPageLevel().TabControls.SelectedTab;

            foreach (Data_Version data in arrVersion)
            {
                if (nWeekday == data.Normal)
                {
                    foreach (Data_Disaster detail in m_arrDetail)
                    {
                        if (data.ID == detail.VersionID && strDetailCategory == detail.DisasterName)
                        {
                            TreeNode parent = treeView.Nodes.Add(strCategory);
                            parent.Tag = nCategoryID;
                            TreeNode secound = parent.Nodes.Add(strSubCategory);
                            secound.Tag = nSubCategoryID;

                            TreeNode third = secound.Nodes.Add(detail.DisasterName);
                            third.Tag = detail.ID;

                            foreach (Data_ActionStep step in m_arrActionStep)
                            {
                                if ((int)third.Tag == step.DisasterID)
                                {
                                    TreeNode node = third.Nodes.Add(step.StepName);
                                    node.Tag = step.ID;

                                    if (tabCurrent != null && tabCurrent.Text == step.StepName)
                                        node.ForeColor = Color.Red;

                                    if (step.ID == nLinkedActionStepID)
                                        treeView.SelectedNode = node;
                                }
                            }
                            break;
                        }
                    }
                }
            }

            treeView.ExpandAll();
        }

        /*private void AddTreeNode()
        {
            ArrayList arrVersion = GetLastVersion();

            string strCategory = FormMain.Instance.GetPageDisaster().SelectedCategory;
            string strSubCategory = FormMain.Instance.GetPageDisaster().SelectedSubCategory;
            string strDetailCategory = FormMain.Instance.GetPageDisaster().SelectedDetailCategory;
            int nCategoryID = GetCategoryID(strCategory);
            int nSubCategoryID = GetSubCategoryID(strSubCategory);

            bool isWeek = FormMain.Instance.GetPageDisaster().IsWeekMode();

            treeView.Nodes.Clear();

            // Normal : true : 평일 false : 휴일 및 야간
            int nWeekday = 0;

            if (isWeek)
                nWeekday = 1; // 평일

            foreach (Data_Version data in arrVersion)
            {
                if (nWeekday != data.Normal)
                {
                    foreach (Data_Disaster detail in m_arrDetail)
                    {
                        if (data.ID == detail.VersionID && strDetailCategory == detail.DisasterName)
                        {
                            TreeNode parent = treeView.Nodes.Add(strCategory);
                            parent.Tag = nCategoryID;
                            TreeNode secound = parent.Nodes.Add(strSubCategory);
                            secound.Tag = nSubCategoryID;

                            TreeNode third = secound.Nodes.Add(detail.DisasterName);
                            third.Tag = detail.ID;
                            
                            foreach (Data_ActionStep step in m_arrActionStep)
                            {
                                if ((int)third.Tag == step.DisasterID)
                                {
                                    TreeNode node = third.Nodes.Add(step.StepName);
                                    node.Tag = step.ID;
                                }
                            }
                            break;
                        }
                    }
                }
            }

            treeView.ExpandAll();
        }*/

        private TreeNode FindNode(string strValue, TreeNodeCollection parentNodes = null)
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

        private int GetCategoryID(string strCategory)
        {
            foreach (Data_DisasterCategory data in m_arrCategory)
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
            foreach (Data_SubDisasterCategory data in m_arrSub)
            {
                if (data.CategoryName == strSubCategory)
                {
                    return data.ID;
                }
            }
            return 0;
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            int nID = (int)e.Node.Tag;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                if (treeView.SelectedNode.Level < 3)
                {
                    MessageBox.Show("SOP 상세 대응 단계를 선택하여 주십시오.");
                    return;
                }

                if (treeView.SelectedNode.ForeColor == Color.Red)
                {
                    MessageBox.Show("전환하려는 SOP와 현재의 SOP가 같습니다.\r\n다른 SOP를 선택하여 주십시오.");
                    return;
                }

                bool isWeek = radioNormal.Checked;//FormMain.Instance.GetPageDisaster().IsWeekMode();

                string strValue = "평일";
                if (!isWeek)
                    strValue = "야간 및 휴일";

                TreeNode node = treeView.SelectedNode;
                if (node != null)
                {
                    m_szTitle = strValue + " / " + node.Text;
                    m_nLinkedActionStepID = (int)node.Tag;
                    m_szFullPath = node.FullPath;
                    treeView.SelectedNode = null;
                }
                	
            }
            else
            {
                m_szTitle = "";
                m_nLinkedActionStepID = -1;
                m_szFullPath = "";               	
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }		


        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void radioNormal_CheckedChanged(object sender, EventArgs e)
        {
            if (m_isNormal)
                return;

            m_isNormal = true;
            AddTreeNode();
        }

        private void radioAbnormal_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_isNormal)
                return;

            m_isNormal = false;
            AddTreeNode();
        }

        private void PopupTransSOP_MouseDown(object sender, MouseEventArgs e)
        {
            m_bLeftMouseDown = true;
            m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
        }

        private void PopupTransSOP_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {
                        Point ptCur = this.Location;
                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void PopupTransSOP_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void SetRadioImage()
        {
            if (radioNormal.Checked == true)
            {
                rdPictureBox1.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Enable2;
            }
            else
            {
                rdPictureBox1.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Disable2;
            }

            if (radioAbnormal.Checked == true)
            {
                rdPictureBox2.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Enable2;
            }
            else
            {
                rdPictureBox2.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Disable2;
            }
        }

        private void rdPictureBox1_Click(object sender, EventArgs e)
        {
            if (radioNormal.Checked == false)
            {
                radioNormal.Checked = !radioNormal.Checked;
                SetRadioImage();
            }
        }

        private void rdPictureBox2_Click(object sender, EventArgs e)
        {
            if (radioAbnormal.Checked == false)
            {
                radioAbnormal.Checked = !radioAbnormal.Checked;
                SetRadioImage();
            }
        }

        private void rdLabel1_Click(object sender, EventArgs e)
        {
            rdPictureBox1_Click(sender, e);
        }

        private void rdLabel2_Click(object sender, EventArgs e)
        {
            rdPictureBox2_Click(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            treeView.SelectedNode = null;
        }

    }
}
