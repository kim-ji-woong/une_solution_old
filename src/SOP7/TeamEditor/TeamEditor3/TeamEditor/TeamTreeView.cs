using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using System.Collections;
using System.Drawing;

namespace TeamEditor
{
    public class TeamTreeView : TreeView
    {
        public class ValidateLabelEditEventArgs : System.ComponentModel.CancelEventArgs
        {
            public ValidateLabelEditEventArgs(string label)
            {
                this.label = label;
                this.Cancel = false;
            }

            private string label;
            public string Label
            {
                get { return label; }
                set { label = value; }
            }
        }

        public enum TeamType { REGULAR = 0, TEMPORARY_NORMAL, TEMPORARY_EMERGENCY, EXTERNAL };
        public enum DropDataType { TREE_NODE = 0, REGULAR_MEMBER, TEMPORARY_MEMBER, EXTERNAL_MEMBER, NONE };

        private WebDBManager m_dbMgr = null;
        private int m_nSiteID = -1;
        private TeamType m_teamType = TeamType.REGULAR;

        private DropDataType m_dropType = DropDataType.NONE;
        private object m_dropData = null;

        // Node being dragged
        private TreeNode dragNode = null;

        // Temporary drop node for selection
        private TreeNode tempDropNode = null;

        // Timer for scrolling
        private Timer timer = new Timer();

        public DropDataType DropType
        {
            get { return m_dropType; }
            set { m_dropType = value; }
        }

        public object DropData
        {
            get { return m_dropData; }
            set { m_dropData = value; }
        }

        public TeamTreeView()
        {
            InitializeComponent();

            timer.Interval = 200;
            timer.Tick += new EventHandler(timer_Tick);

            this.HideSelection = false;
            this.LabelEdit = false;
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);

        }


        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode tn = this.GetNodeAt(e.X, e.Y);
                if (tn != null)
                    this.SelectedNode = tn;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            TreeNode tn;
            
            if (e.Button == MouseButtons.Left)
            {
                tn = this.SelectedNode;
                if (tn == this.GetNodeAt(e.X, e.Y))
                {
                    if (wasDoubleClick)
                        wasDoubleClick = false;
                    else
                    {
                        TriggerLabelEdit = true;
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                FormMain.Instance.OnTreeViewMouseUp(this, e);
            }

            base.OnMouseUp(e);
        }

        private const int WM_TIMER = 0x0113;
        private bool TriggerLabelEdit = false;

        protected override void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
        {
            if (!FormMain.Instance.IsEditMode)
            {
                e.CancelEdit = true;
                return;
            }
        }

        protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
        {
            this.LabelEdit = false;
            e.CancelEdit = true;
            if (e.Label == null)
            {
                return;
            }
            ValidateLabelEditEventArgs ea = new ValidateLabelEditEventArgs(e.Label);
            OnValidateLabelEdit(ea);
            if (ea.Cancel == true)
            {
                this.LabelEdit = true;
                e.Node.BeginEdit();
            }
            else
                base.OnAfterLabelEdit(e);
        }

        public void BeginEdit()
        {
            StartLabelEdit();
        }

        protected override void OnNotifyMessage(Message m)
        {
            if (TriggerLabelEdit)
                if (m.Msg == WM_TIMER)
                {
                    TriggerLabelEdit = false;
                    StartLabelEdit();
                }
            base.OnNotifyMessage(m);
        }

        public void StartLabelEdit()
        {
            TreeNode tn = this.SelectedNode;
            //viewedLabel = tn.Text;

            NodeLabelEditEventArgs e = new NodeLabelEditEventArgs(tn);
            base.OnBeforeLabelEdit(e);

            this.LabelEdit = true;
            tn.BeginEdit();
        }


        protected override void OnClick(EventArgs e)
        {
            TriggerLabelEdit = false;
            base.OnClick(e);
        }

        private bool wasDoubleClick = false;
        protected override void OnDoubleClick(EventArgs e)
        {
            wasDoubleClick = true;
            base.OnDoubleClick(e);
        }

        public event ValidateLabelEditEventHandler ValidateLabelEdit;

        protected virtual void OnValidateLabelEdit(ValidateLabelEditEventArgs e)
        {
            ValidateLabelEdit(this, e);
        }

        public delegate void ValidateLabelEditEventHandler(object sender, ValidateLabelEditEventArgs e);

        public TeamType GetTeamType()
        {
            return m_teamType;
        }

        public bool LoadData(WebDBManager dbMgr, int nSiteID, TeamType type)
        {
            m_dbMgr = dbMgr;
            m_nSiteID = nSiteID;
            m_teamType = type;

            this.Nodes.Clear();

            if (type == TeamType.REGULAR && LoadRegularTeam())
            {
                FormMain.Instance.SetRegularTeamComboItems();
                return DataManager.LoadCompanyMember(dbMgr);
            }
            else if (type == TeamType.TEMPORARY_NORMAL && LoadNormalTeam())
                return DataManager.LoadNormalMember(dbMgr);
            else if (type == TeamType.TEMPORARY_EMERGENCY && LoadEmergencyTeam())
                return DataManager.LoadEmergencyMember(dbMgr);
            else if (type == TeamType.EXTERNAL && LoadExternalCompanyTeam())
            {
                FormMain.Instance.SetExternalTeamComboItems();
                return DataManager.LoadExternalCompanyMember(dbMgr);
            }

            return false;
        }

        private TreeNode CreateExternalCompanyTeamNode(int nParentTeamID, Dictionary<ExternalTeam, int> teams)
        {
            TreeNode rtnNode = null;

            foreach (KeyValuePair<ExternalTeam, int> item in from items in teams
                                                             where items.Key.TeamID == nParentTeamID
                                                             select items
                                                            )
            {

                ExternalTeam parentTeam = DataManager.GetExternalTeam(item.Value);
                TreeNode parentNode = FindNode(this.Nodes, parentTeam);

                if (parentNode == null)
                {
                    parentNode = CreateExternalCompanyTeamNode(item.Value, teams);
                }

                TreeNode node = FindNode(this.Nodes, parentTeam).Nodes.Add(item.Key.TeamName);
                node.Tag = item.Key;

                item.Key.ParentTeam = parentTeam;

                rtnNode = node;
            }

            return rtnNode;
        }

        private bool LoadExternalCompanyTeam()
        {
            string strIFNull = m_dbMgr.DatabaseType == WebDBManager.DBType.sqlserver ? "ISNULL" : "IFNULL";

            string strSQL = String.Format("SELECT ID, TeamName, PhoneNumber, FaxNumber, ParentTeamID FROM ExternalTeam WHERE SiteID = {0} ORDER BY {1}(ParentTeamID, 0) ASC ", m_nSiteID, strIFNull);
            ArrayList arrResults = m_dbMgr.GetResultData(strSQL);

            if (arrResults == null)
                return false;

            int nResultCount = arrResults.Count;

            Dictionary<ExternalTeam, int> dicTeamList = new Dictionary<ExternalTeam, int>();

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                int nID = WebDBManager.GetIntField(arrResults[i].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResults[i + 1], "");
                string strPhoneNumber = WebDBManager.GetStringField(arrResults[i + 2].ToString(), "");
                string strFaxNumber = WebDBManager.GetStringField(arrResults[i + 3].ToString(), null);
                int nParentTeamID = WebDBManager.GetIntField(arrResults[i + 4].ToString(), -1);

                if (nID < 0)
                    continue;

                ExternalTeam team = new ExternalTeam();

                team.TeamID = nID;
                team.TeamName = strTeamName;
                team.PhoneNumber = strPhoneNumber;

                if (strFaxNumber != null && strFaxNumber != "null")
                    team.FaxNumber = strFaxNumber;

                DataManager.AddTeam(team);

                if (nParentTeamID < 0)
                {
                    TreeNode node = this.Nodes.Add(strTeamName);
                    node.Tag = team;
                }
                else
                {
                    dicTeamList.Add(team, nParentTeamID);

                    //TreeNode node = FindNode(this.Nodes, DataManager.GetExternalTeam(nParentTeamID)).Nodes.Add(strTeamName);
                    //node.Tag = team;

                    //team.ParentTeam = DataManager.GetExternalTeam(nParentTeamID);
                }
            }

            foreach (KeyValuePair<ExternalTeam, int> item in dicTeamList)
            {
                ExternalTeam parentTeam = DataManager.GetExternalTeam(item.Value);
                TreeNode parentNode = FindNode(this.Nodes, parentTeam);

                if (parentNode == null)
                {
                    parentNode = CreateExternalCompanyTeamNode(item.Value, dicTeamList);
                }

                TreeNode node = FindNode(this.Nodes, parentTeam).Nodes.Add(item.Key.TeamName);
                node.Tag = item.Key;

                item.Key.ParentTeam = parentTeam;
            }
            


            //if (strCompanyIDs.Length == 0)
            //    return false;
            //else
            //    strCompanyIDs = "(" + strCompanyIDs + ")";

            //strSQL = "Select ID, TeamName, ParentTeamID, CompanyID from ExternalCompanyTeam where CompanyID in " + strCompanyIDs;
            //arrResults = m_dbMgr.GetResultData(strSQL);

            //if (arrResults == null)
            //    return false;

            //nResultCount = arrResults.Count;
            //Dictionary<ExternalCompanyTeam, int> dicParentIDs = new Dictionary<ExternalCompanyTeam, int>();

            //for (int i = 0; i < nResultCount - 3; i += 4)
            //{
            //    int nID = WebDBManager.GetIntField(arrResults[i].ToString(), -1);
            //    string strTeamName = WebDBManager.GetStringField(arrResults[i + 1], "");
            //    int nParentTeamID = WebDBManager.GetIntField(arrResults[i + 2].ToString(), -1);
            //    int nCompanyID = WebDBManager.GetIntField(arrResults[i + 3].ToString(), -1);

            //    if (nID < 0)
            //        continue;

            //    ExternalCompanyTeam team = new ExternalCompanyTeam();

            //    team.TeamID = nID;
            //    team.TeamName = strTeamName;
            //    team.Company = DataManager.GetExternalTeam(nCompanyID);

            //    if (team.Company == null)
            //        continue;

            //    DataManager.AddTeam(team);
            //    dicParentIDs[team] = nParentTeamID;
            //}

            //foreach (KeyValuePair<ExternalCompanyTeam, int> pair in dicParentIDs)
            //{
            //    pair.Key.ParentTeam = DataManager.GetExternalCompanyTeam(pair.Value);
            //}

            //foreach (KeyValuePair<ExternalCompanyTeam, int> pair in dicParentIDs)
            //{
            //    ExternalCompanyTeam team = pair.Key;

            //    if (team.Company == null)
            //        continue;

            //    TreeNode companyNode = FindNode(this.Nodes, team.Company);

            //    if (companyNode == null)
            //        continue;

            //    TreeNode teamNode = FindNode(companyNode.Nodes, team);

            //    if (teamNode == null)
            //    {
            //        List<ExternalCompanyTeam> teams = new List<ExternalCompanyTeam>();
            //        teams.Add(team);

            //        TreeNodeCollection nodes = companyNode.Nodes;

            //        while (team.ParentTeam != null)
            //        {
            //            TreeNode node = FindNode(companyNode.Nodes, team.ParentTeam);

            //            if (node != null)
            //            {
            //                nodes = node.Nodes;
            //                break;
            //            }

            //            teams.Add(team.ParentTeam);
            //            team = team.ParentTeam;
            //        }

            //        int nTeamCount = teams.Count;

            //        for (int i = nTeamCount - 1; i >= 0; i--)
            //        {
            //            team = teams[i];
            //            TreeNode node = nodes.Add(team.TeamName);
            //            node.Tag = team;
            //            nodes = node.Nodes;
            //        }
            //    }
            //}

            //dicParentIDs.Clear();
            this.ExpandAll();

            return true;
        }

        private TreeNode FindNode(TreeNodeCollection nodes, object tagData)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag == tagData)
                    return node;

                TreeNode childNode = FindNode(node.Nodes, tagData);

                if (childNode != null)
                    return childNode;
            }

            return null;
        }

        private bool LoadNormalTeam()
        {
            string strSQL = "select ID, TeamName, ParentTeamID from TemporaryNormalTeam where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResults = m_dbMgr.GetResultData(strSQL);

            if (arrResults == null)
                return false;

            int nResultCount = arrResults.Count;

            if (nResultCount == 0)
                return false;

            Dictionary<int, int> dicParentTeamID = new Dictionary<int, int>();
            Dictionary<int, TreeNode> dicTreeNodes = new Dictionary<int, TreeNode>();

            for (int i=0;i<nResultCount-2;i+=3)
            {
                int nID = WebDBManager.GetIntField(arrResults[i].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResults[i + 1], "");
                int nParentTeamID = WebDBManager.GetIntField(arrResults[i + 2].ToString(), -1);

                if (nID < 0)
                    continue;

                TemporaryNormalTeam team = new TemporaryNormalTeam();
                team.TeamID = nID;
                team.TeamName = strTeamName;
                
                TreeNode node = new TreeNode(strTeamName);
                node.Tag = team;

                if (nParentTeamID < 0)
                    this.Nodes.Add(node);
                else
                    dicParentTeamID[nID] = nParentTeamID;

                DataManager.AddTeam(team);
                dicTreeNodes[nID] = node;

                DataManager.AddTeam(team);
            }

            foreach (KeyValuePair<int, int> pair in dicParentTeamID)
            {
                TemporaryNormalTeam team = DataManager.GetTemporaryNormalTeam(pair.Key);
                TemporaryNormalTeam teamParent = DataManager.GetTemporaryNormalTeam(pair.Value);

                TreeNode node, nodeParent;

                if (team == null || !dicTreeNodes.TryGetValue(pair.Key, out node))
                    continue;

                if (teamParent == null || !dicTreeNodes.TryGetValue(pair.Value, out nodeParent))
                    continue;

                team.ParentTeam = teamParent;
                nodeParent.Nodes.Add(node);
            }

            this.ExpandAll();
            DataManager.SetTemporaryRootTeams(true);
            return true;
        }

        private bool LoadEmergencyTeam()
        {
            string strSQL = "select ID, TeamName, ParentTeamID from TemporaryEmergencyTeam where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResults = m_dbMgr.GetResultData(strSQL);

            if (arrResults == null)
                return false;

            int nResultCount = arrResults.Count;

            if (nResultCount == 0)
                return false;

            Dictionary<int, int> dicParentTeamID = new Dictionary<int, int>();
            Dictionary<int, TreeNode> dicTreeNodes = new Dictionary<int, TreeNode>();

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResults[i].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResults[i + 1], "");
                int nParentTeamID = WebDBManager.GetIntField(arrResults[i + 2].ToString(), -1);

                if (nID < 0)
                    continue;

                TemporaryEmergencyTeam team = new TemporaryEmergencyTeam();
                team.TeamID = nID;
                team.TeamName = strTeamName;

                TreeNode node = new TreeNode(strTeamName);
                node.Tag = team;

                if (nParentTeamID < 0)
                    this.Nodes.Add(node);
                else
                    dicParentTeamID[nID] = nParentTeamID;

                DataManager.AddTeam(team);
                dicTreeNodes[nID] = node;

                DataManager.AddTeam(team);
            }

            foreach (KeyValuePair<int, int> pair in dicParentTeamID)
            {
                TemporaryEmergencyTeam team = DataManager.GetTemporaryEmergencyTeam(pair.Key);
                TemporaryEmergencyTeam teamParent = DataManager.GetTemporaryEmergencyTeam(pair.Value);

                TreeNode node, nodeParent;

                if (team == null || !dicTreeNodes.TryGetValue(pair.Key, out node))
                    continue;

                if (teamParent == null || !dicTreeNodes.TryGetValue(pair.Value, out nodeParent))
                    continue;

                team.ParentTeam = teamParent;
                nodeParent.Nodes.Add(node);
            }

            this.ExpandAll();
            DataManager.SetTemporaryRootTeams(false);
            return true;
        }

        private bool LoadRegularTeam()
        {
            string strSQL = "select TeamID from Site where ID = " + m_nSiteID.ToString();
            ArrayList arrResults = m_dbMgr.GetResultData(strSQL);

            if (arrResults == null || arrResults.Count == 0)
                return false;

            int nRootTeamID = WebDBManager.GetIntField(arrResults[0].ToString(), -1);

            arrResults = ExecuteTeamList(m_dbMgr, nRootTeamID);
            //strSQL = "exec sp_TeamList2 " + nRootTeamID.ToString();
            //arrResults = m_dbMgr.GetStoredProcedureData(strSQL, 0);

            if (arrResults == null)
                return false;

            int nResultCount = arrResults.Count;

            if (nResultCount == 0)
                return false;

            DataManager.ClearRegularTeams();

            Dictionary<int, TreeNode> dicTreeNodes = new Dictionary<int, TreeNode>();
            Dictionary<int, int> dicMissingParentTeamID = new Dictionary<int, int>();

            for (int i=0;i<nResultCount-2;i+=3)
            {
                int nTeamID = WebDBManager.GetIntField(arrResults[i].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResults[i + 1], "");
                int nParentTeamID = WebDBManager.GetIntField(arrResults[i + 2].ToString(), -1);

                if (nTeamID < 0)
                    continue;

                RegularTeam team = new RegularTeam();
                team.TeamID = nTeamID;
                team.TeamName = strTeamName;

                TreeNode node = new TreeNode(strTeamName, 10, 10);
                node.Tag = team;
                //node.Tag = nTeamID;

                TreeNode nodeParent = null;
                
                if (nParentTeamID > 0)
                {
                    RegularTeam teamParent = DataManager.GetRegularTeam(nParentTeamID);

                    if (teamParent != null)
                    {
                        team.ParentTeam = teamParent;

                        if (!dicTreeNodes.TryGetValue(nParentTeamID, out nodeParent))
                            continue;
                    }
                }

                if (nodeParent == null && i == 0)
                {
                    node.ImageIndex = 0; 
                    node.SelectedImageIndex = 0;
                    this.Nodes.Add(node);
                }
                else if (nParentTeamID < 0)
                    continue;
                else
                    dicMissingParentTeamID[nTeamID] = nParentTeamID;

                DataManager.AddTeam(team);
                dicTreeNodes[nTeamID] = node;
            }

            // 부모노드가 할당되지 않은 Team들
            foreach (KeyValuePair<int, int> pair in dicMissingParentTeamID)
            {
                RegularTeam team = DataManager.GetRegularTeam(pair.Key);
                RegularTeam teamParent = DataManager.GetRegularTeam(pair.Value);

                TreeNode node, nodeParent;

                if (team == null || !dicTreeNodes.TryGetValue(pair.Key, out node))
                    continue;

                if (teamParent == null || !dicTreeNodes.TryGetValue(pair.Value, out nodeParent))
                    continue;

                team.ParentTeam = teamParent;
                nodeParent.Nodes.Add(node);
            }

            this.ExpandAll();
            return true;
        }

        public static ArrayList ExecuteTeamList(WebDBManager dbMgr, int nRootTeamID, string strTableName = "RegularTeam")
        {
            string strSQL = "Select ID, TeamName, ParentTeamID from " + strTableName + " order by ParentTeamID, ID";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            if (nRootTeamID == 0)
                return arrResult;

            int nResultCount = arrResult.Count;

            ArrayList arrNewResult = new ArrayList();
            Dictionary<int, int> dicParentID = new Dictionary<int, int>();

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                VariousData<int> parentID = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                if (dicParentID.Count == 0)
                {
                    if (nID == nRootTeamID)
                    {
                        dicParentID[nID] = nID;

                        arrNewResult.Add(arrResult[i]);
                        arrNewResult.Add(arrResult[i + 1]);
                        arrNewResult.Add(arrResult[i + 2]);
                    }
                }
                else
                {
                    if (parentID == null)
                        continue;

                    if (dicParentID.ContainsKey(parentID.Data))
                    {
                        dicParentID[nID] = nID;

                        arrNewResult.Add(arrResult[i]);
                        arrNewResult.Add(arrResult[i + 1]);
                        arrNewResult.Add(arrResult[i + 2]);
                    }
                }
            }

            return arrNewResult;
        }

        public void LoadRegularTeam(Tree<RegularTeam> tree)
        {
            if (tree == null || tree.RootNode.Data == null)
                return;

            RegularTeam team = tree.RootNode.Data;
            TreeNode node = team.TeamID < 0 ? null : FindTeamNode(team.TeamID, this.Nodes);

            if (node == null)
            {
                node = new TreeNode(team.TeamName, 0, 0);
                node.Tag = team;
                this.Nodes.Add(node);
            }

            foreach (Tree<RegularTeam>.Node child in tree.RootNode.Children)
            {
                LoadRegularTeam(child, node.Nodes);
            }

            this.ExpandAll();
        }

        private TreeNode FindTeamNode(int nTeamID, TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag != null && (node.Tag is Team))
                {
                    Team team = (Team)node.Tag;

                    if (team.TeamID == nTeamID)
                        return node;
                }
            }

            return null;
        }

        private void LoadRegularTeam(Tree<RegularTeam>.Node node, TreeNodeCollection nodes)
        {
            if (node.Data == null)
                return;

            RegularTeam team = node.Data;
            TreeNode newNode = team.TeamID < 0 ? null : FindTeamNode(team.TeamID, nodes);

            if (newNode == null)
            {
                newNode = new TreeNode(team.TeamName, 10, 10);
                newNode.Tag = team;
                nodes.Add(newNode);
            }

            foreach (Tree<RegularTeam>.Node child in node.Children)
            {
                LoadRegularTeam(child, newNode.Nodes);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TeamTreeView
            // 
            this.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TeamTreeView_AfterSelect);
            this.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.TeamTreeView_ItemDrag);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.TeamTreeView_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.TeamTreeView_DragEnter);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.TeamTreeView_DragOver);
            this.DragLeave += new System.EventHandler(this.TeamTreeView_DragLeave);
            this.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.TeamTreeView_GiveFeedback);
            this.ResumeLayout(false);

        }

        private void TeamTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (m_teamType == TeamType.REGULAR)
            {
                if (SelectedNode == null || SelectedNode.Tag == null || SelectedNode.Tag.GetType() != typeof(RegularTeam))
                    FormMain.Instance.SelectRegularTeam(null);
                else
                {
                    RegularTeam team = (RegularTeam)SelectedNode.Tag;
                    //RegularTeam team = DataManager.GetRegularTeam((int)SelectedNode.Tag);
                    FormMain.Instance.SelectRegularTeam(team);
                }
            }
            else if (m_teamType == TeamType.TEMPORARY_NORMAL)
            {
                if (SelectedNode == null || SelectedNode.Tag == null || SelectedNode.Tag.GetType() != typeof(TemporaryNormalTeam))
                    FormMain.Instance.SelectTemporaryTeam(null, true);
                else
                {
                    TemporaryNormalTeam team = (TemporaryNormalTeam)SelectedNode.Tag;
                    FormMain.Instance.SelectTemporaryTeam(team, true);
                }
            }
            else if (m_teamType == TeamType.TEMPORARY_EMERGENCY)
            {
                if (SelectedNode == null || SelectedNode.Tag == null || SelectedNode.Tag.GetType() != typeof(TemporaryEmergencyTeam))
                    FormMain.Instance.SelectTemporaryTeam(null, false);
                else
                {
                    TemporaryEmergencyTeam team = (TemporaryEmergencyTeam)SelectedNode.Tag;
                    FormMain.Instance.SelectTemporaryTeam(team, false);
                }
            }
            else if (m_teamType == TeamType.EXTERNAL)
            {
                if (SelectedNode == null || SelectedNode.Tag == null || (SelectedNode.Tag is ExternalTeam) == false)
                {
                    FormMain.Instance.SelectExternalCompanyTeam(null);
                }
                else
                {
                    Team team = (Team)SelectedNode.Tag;
                    FormMain.Instance.SelectExternalCompanyTeam(team);
                }
            }
        }

        private void TeamTreeView_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
        {
            // Get drag node and select it
            this.dragNode = (TreeNode)e.Item;
            this.SelectedNode = this.dragNode;

            Bitmap bmp = MakeDragBitmap(this.dragNode.Text, this.dragNode);

            if (bmp == null)
                return;
            /*// Reset image list used for drag image
            FormMain.Instance.ImageListDrag.Images.Clear();
            FormMain.Instance.ImageListDrag.ImageSize = new Size(this.dragNode.Bounds.Size.Width + this.Indent, this.dragNode.Bounds.Height);

            // Create new bitmap
            // This bitmap will contain the tree node image to be dragged
            Bitmap bmp = new Bitmap(this.dragNode.Bounds.Width + this.Indent, this.dragNode.Bounds.Height);

            // Get graphics from bitmap
            Graphics gfx = Graphics.FromImage(bmp);

            // Draw node label into bitmap
            gfx.DrawString(this.dragNode.Text,
                this.Font,
                new SolidBrush(this.ForeColor),
                (float)this.Indent, 1.0f);

            // Add bitmap to imagelist
            FormMain.Instance.ImageListDrag.Images.Add(bmp);*/

            BeginDragDrop(bmp, DragDropEffects.Move, DropDataType.TREE_NODE/*, this.dragNode*/);
            /*// Get mouse position in client coordinates
            Point p = this.PointToClient(Control.MousePosition);

            // Compute delta between mouse position and node bounds
            int dx = p.X + this.Indent - this.dragNode.Bounds.Left;
            int dy = p.Y - this.dragNode.Bounds.Top;

            // Begin dragging image
            if (DragHelper.ImageList_BeginDrag(FormMain.Instance.ImageListDrag.Handle, 0, dx, dy))
            {
                this.DropType = DropDataType.TREE_NODE;

                // Begin dragging
                this.DoDragDrop(bmp, DragDropEffects.Move);
                // End dragging image
                DragHelper.ImageList_EndDrag();
            }*/
        }

        public void BeginDragDrop(Bitmap bmp, DragDropEffects effects, DropDataType type, TreeNode node = null)
        {
            // Get mouse position in client coordinates
            Point p = this.PointToClient(Control.MousePosition);

            // Compute delta between mouse position and node bounds
            int dx = node == null ? this.Indent : p.X + this.Indent - node.Bounds.Left;
            int dy = node == null ? this.Indent : p.Y - node.Bounds.Top;

            // Begin dragging image
            if (DragHelper.ImageList_BeginDrag(FormMain.Instance.ImageListDrag.Handle, 0, dx, dy))
            {
                this.DropType = type;

                // Begin dragging
                this.DoDragDrop(bmp, effects);
                // End dragging image
                DragHelper.ImageList_EndDrag();
            }
        }

        public Bitmap MakeDragBitmap(string str, TreeNode boundNode = null)
        {
            if (boundNode == null)
            {
                if (this.Nodes.Count == 0)
                    return null;

                boundNode = this.Nodes[0];
            }

            // Reset image list used for drag image
            FormMain.Instance.ImageListDrag.Images.Clear();
            FormMain.Instance.ImageListDrag.ImageSize = new Size(boundNode.Bounds.Size.Width + this.Indent, boundNode.Bounds.Height);

            // Create new bitmap
            // This bitmap will contain the tree node image to be dragged
            Bitmap bmp = new Bitmap(boundNode.Bounds.Width + this.Indent, boundNode.Bounds.Height);

            // Get graphics from bitmap
            Graphics gfx = Graphics.FromImage(bmp);

            // Draw node label into bitmap
            gfx.DrawString(str,
                this.Font,
                new SolidBrush(this.ForeColor),
                (float)this.Indent, 1.0f);

            // Add bitmap to imagelist
            FormMain.Instance.ImageListDrag.Images.Add(bmp);

            return bmp;
        }

        private void TeamTreeView_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            // Compute drag position and move image
            Point formP = this.PointToClient(new Point(e.X, e.Y));
            DragHelper.ImageList_DragMove(formP.X - this.Left, formP.Y - this.Top);

            // Get actual drop node
            TreeNode dropNode = this.GetNodeAt(this.PointToClient(new Point(e.X, e.Y)));
            if (dropNode == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            e.Effect = DragDropEffects.Move;

            // if mouse is on a new node select it
            if (this.tempDropNode != dropNode)
            {
                DragHelper.ImageList_DragShowNolock(false);
                this.SelectedNode = dropNode;
                DragHelper.ImageList_DragShowNolock(true);
                tempDropNode = dropNode;
            }

            // Avoid that drop node is child of drag node 
            TreeNode tmpNode = dropNode;
            while (tmpNode.Parent != null)
            {
                if (tmpNode.Parent == this.dragNode) e.Effect = DragDropEffects.None;
                tmpNode = tmpNode.Parent;
            }
        }

        private void TeamTreeView_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            // Unlock updates
            DragHelper.ImageList_DragLeave(this.Handle);

            // Get drop node
            TreeNode dropNode = this.GetNodeAt(this.PointToClient(new Point(e.X, e.Y)));

            if (m_dropType == DropDataType.REGULAR_MEMBER)
            {
                if (m_dropData == null)
                    return;

                if (m_dropData is TeamEditor.BLL.WinForms.Command.CommandMoveRegularMembers)
                {
                    OnDropRegularMembers((TeamEditor.BLL.WinForms.Command.CommandMoveRegularMembers)m_dropData, dropNode);
                }

                return;
            }
            else if (m_dropType == DropDataType.TEMPORARY_MEMBER)
            {
                if (m_dropData == null)
                    return;

                if (m_dropData is TeamEditor.BLL.WinForms.Command.CommandMoveTemporaryMembers)
                {
                    OnDropTemporaryMembers((TeamEditor.BLL.WinForms.Command.CommandMoveTemporaryMembers)m_dropData, dropNode);
                }

                return;
            }
            
            // If drop node isn't equal to drag node, add drag node as child of drop node
            if (this.dragNode != dropNode)
            {
                TreeNode srcNode = this.dragNode;
                TreeNode nodeSrcParent = srcNode.Parent;

                /*// Remove drag node from parent
                if (this.dragNode.Parent == null)
                {
                    this.Nodes.Remove(this.dragNode);
                }
                else
                {
                    this.dragNode.Parent.Nodes.Remove(this.dragNode);
                }

                // Add drag node to drop node
                dropNode.Nodes.Add(this.dragNode);
                dropNode.ExpandAll();*/

                // Set drag node to null
                this.dragNode = null;

                // Disable scroll timer
                this.timer.Enabled = false;

                FormMain.Instance.OnDropNode(this, nodeSrcParent, srcNode, dropNode);
            }
        }

        private void OnDropTemporaryMembers(TeamEditor.BLL.WinForms.Command.CommandMoveTemporaryMembers cmd, TreeNode dropNode)
        {
            // Disable scroll timer
            this.timer.Enabled = false;

            FormMain.Instance.OnDropTemporaryMembers(cmd, dropNode);
        }

        private void OnDropRegularMembers(TeamEditor.BLL.WinForms.Command.CommandMoveRegularMembers cmd, TreeNode dropNode)
        {
            // Disable scroll timer
            this.timer.Enabled = false;

            FormMain.Instance.OnDropRegularMembers(cmd, dropNode);
        }

        private void TeamTreeView_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            DragHelper.ImageList_DragEnter(this.Handle, e.X - this.Left,
                e.Y - this.Top);

            // Enable timer for scrolling dragged item
            this.timer.Enabled = true;
        }

        private void TeamTreeView_DragLeave(object sender, System.EventArgs e)
        {
            DragHelper.ImageList_DragLeave(this.Handle);

            // Disable timer for scrolling dragged item
            this.timer.Enabled = false;
        }

        private void TeamTreeView_GiveFeedback(object sender, System.Windows.Forms.GiveFeedbackEventArgs e)
        {
            if (e.Effect == DragDropEffects.Move)
            {
                // Show pointer cursor while dragging
                e.UseDefaultCursors = false;
                this.Cursor = Cursors.Default;
            }
            else
                e.UseDefaultCursors = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            // get node at mouse position
            Point pt = PointToClient(Control.MousePosition);
            TreeNode node = this.GetNodeAt(pt);

            if (node == null) return;

            // if mouse is near to the top, scroll up
            if (pt.Y < 30)
            {
                // set actual node to the upper one
                if (node.PrevVisibleNode != null)
                {
                    node = node.PrevVisibleNode;

                    // hide drag image
                    DragHelper.ImageList_DragShowNolock(false);
                    // scroll and refresh
                    node.EnsureVisible();
                    this.Refresh();
                    // show drag image
                    DragHelper.ImageList_DragShowNolock(true);

                }
            }
            // if mouse is near to the bottom, scroll down
            else if (pt.Y > this.Size.Height - 30)
            {
                if (node.NextVisibleNode != null)
                {
                    node = node.NextVisibleNode;

                    DragHelper.ImageList_DragShowNolock(false);
                    node.EnsureVisible();
                    this.Refresh();
                    DragHelper.ImageList_DragShowNolock(true);
                }
            }
        }
    }
}
