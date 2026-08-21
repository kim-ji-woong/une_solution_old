using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SOPGen
{
    public partial class FormTeam : Form
    {
        private bool m_isRegular = true;
        //private DBManager m_dbMgr = null;
        public WebDBManager m_dbMgr = null;

        private TeamData m_selectedTeamData = null;
        private static FormTeam m_frmRegularInstance = null;
        private static FormTeam m_frmTemporaryInstance = null;

        public static FormTeam Instance(bool isRegular = true)
        {
            if (isRegular)
            {
                if (m_frmRegularInstance == null)
                {
                    m_frmRegularInstance = new FormTeam(FormMain.Instance.m_dbMgr, true);
                }

                return m_frmRegularInstance;
            }

            if (m_frmTemporaryInstance == null)
            {
                m_frmTemporaryInstance = new FormTeam(FormMain.Instance.m_dbMgr, false);
            }

            return m_frmTemporaryInstance;
        }

        //private FormTeam(DBManager dbMgr, bool isRegular = true)
        private FormTeam(WebDBManager dbMgr, bool isRegular = true)
        {
            m_dbMgr = dbMgr;
            m_isRegular = isRegular;

            InitializeComponent();
        }

        private void FormTeam_Load(object sender, EventArgs e)
        {
            if (m_dbMgr == null)
                return;

            treeViewTeam.Nodes.Clear();

            this.Text = m_isRegular ? "상시 조직도" : "비상상황 조직도";

            ArrayList arrFields = new ArrayList();
            ArrayList arrValues = new ArrayList();

            arrFields.Add("@teamName");
            arrFields.Add("@isRegular");

            //arrValues.Add("");
            //arrValues.Add(m_isRegular ? "1" : "0");
            arrValues.Add("''");
            arrValues.Add(m_isRegular ? "'1'" : "'0'");

            ArrayList arrResult;
            m_dbMgr.RunStoredProcedure("sp_TeamList", arrFields, arrValues, 0, out arrResult);

            if (arrResult == null)
                return;

            string strTeamName;
            int nTeamID, nParentTeamID;

            ArrayList arrTeamID = new ArrayList();

            for (int i = 0; i < arrResult.Count - 2; i = i + 3)
            {
                nTeamID = m_dbMgr.GetIntField(arrResult[i].ToString(), 0);
                strTeamName = m_dbMgr.GetStringField(arrResult[i+1].ToString(), "");
                nParentTeamID = m_dbMgr.GetIntField(arrResult[i+2].ToString(), 0);
                
                arrTeamID.Add(nTeamID);
                AddTeamTreeData(nTeamID, strTeamName, nParentTeamID);
            }

            AddTeamMember(arrTeamID);
        }

        private void AddTeamMember(ArrayList arrTeamID)
        {
            int nIDCount = arrTeamID.Count;
            if (nIDCount == 0) return;

            string strTeamField = m_isRegular ? "RegularTeamID" : "TemporaryTeamID";
            string strSQL = string.Format("select id, MemberName, {0} from CompanyMember where {0} in ({1}", strTeamField, (int)arrTeamID[0]);

            for (int i=1;i<nIDCount;i++)
            {
                strSQL += string.Format(", {0}", arrTeamID[i]);
            }

            strSQL += ") order by " + strTeamField;

            //System.Data.SqlClient.SqlDataReader reader;
            //m_dbMgr.ReadDB(strSQL, null, out reader);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            //if (reader == null)
            //    return;

            int nMemberID, nTeamID;
            string strMemberName;
            DataTreeNode<int, TeamData.DataType> node = null;
            int nPrevTeamID = -1;

            for (int i = 0; i < arrResult.Count - 2; i = i + 3)
            {
                nMemberID = m_dbMgr.GetIntField(arrResult[i].ToString(), 0);
                strMemberName = m_dbMgr.GetStringField(arrResult[i + 1].ToString(), "");
                nTeamID = m_dbMgr.GetIntField(arrResult[i + 2].ToString(), 0);
                
                if (nPrevTeamID != nTeamID)
               {
                    node = FindTree(null, nTeamID, TeamData.DataType.TeamData);
                    if (node == null) break;

                    DataTreeNode<int, TeamData.DataType> MemberData = new DataTreeNode<int, TeamData.DataType>();
                    MemberData.Text = strMemberName;
                    MemberData.Data = nMemberID;
                    MemberData.Type = TeamData.DataType.MemberData;

                    node.Nodes.Add(MemberData);
                }
            }
        }

        private DataTreeNode<int, TeamData.DataType> FindTree(DataTreeNode<int, TeamData.DataType> parentNode, int nTeamID, TeamData.DataType type)
        {
            TreeNodeCollection nodes = parentNode == null ? treeViewTeam.Nodes : parentNode.Nodes;
            int nCount = nodes.Count;

            for (int i = 0; i < nCount; i++)
            {
                DataTreeNode<int, TeamData.DataType> node = (DataTreeNode<int, TeamData.DataType>)nodes[i];
                if (node.Data == nTeamID && node.Type == type)
                    return node;
            
                node = FindTree(node, nTeamID, type);
                if (node != null) return node;
            }

            return null;
        }

        private void AddTeamTreeData(int nTeamID, string strTeamName, int nParentTeamID)
        {
            DataTreeNode<int, TeamData.DataType> node = new DataTreeNode<int, TeamData.DataType>();
            node.Text = strTeamName;
            node.Data = nTeamID;
            node.Type = TeamData.DataType.TeamData;

            if (nParentTeamID == 0)
            {
                treeViewTeam.Nodes.Add(node);
            }
            else
            {
                DataTreeNode<int, TeamData.DataType> parentNode = FindTree(null, nParentTeamID, TeamData.DataType.TeamData);

                if (parentNode == null)
                {
                    MessageBox.Show(string.Format("조직 Tree에서 TeamID가 {0}인 데이터를 찾을 수 없습니다.", nParentTeamID));
                    return;
                }

                parentNode.Nodes.Add(node);
            }
        }

        private string GetFullPathName(TreeNode node)
        {
            string strFullPath = node.Text;
            
            while (node.Parent != null)
            {
                node = node.Parent;
                strFullPath = node.Text + "/" + strFullPath;
            }

            return strFullPath;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            DataTreeNode<int, TeamData.DataType> node = (DataTreeNode<int, TeamData.DataType>)treeViewTeam.SelectedNode;

            if (node == null)
            {
                MessageBox.Show("Tree에서 선택이 되어있지 않습니다.");
                return;
            }

            m_selectedTeamData = new TeamData();

            m_selectedTeamData.Name = node.Text;
            m_selectedTeamData.FullName = GetFullPathName(node);
            m_selectedTeamData.ID = node.Data;
            m_selectedTeamData.Type = node.Type;

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            m_selectedTeamData = null;
            Close();
        }

        public TeamData GetSelectedTeamData()
        {
            return m_selectedTeamData;
        }

        // strNodeName에 해당하는 노드 정보를 검색하여 arrTeamData에 담는다.
        // 해당 이름을 가진 노드의 개수를 리턴한다.
        public int FindItem(string strItemName, ref ArrayList arrTeamData)
        {
            return FindTree(strItemName, null, ref arrTeamData);
        }

        // nID에 해당하는 TeamData 객체를 얻어온다.
        // isMember가 true이면 nID는 멤버의 ID이며, false이면 TeamID를 의미한다.
        public bool FindItem(int nID, bool isMember, out TeamData data)
        {
            data = null;
            return FindTree(nID, isMember ? TeamData.DataType.MemberData : TeamData.DataType.TeamData, null, ref data);
        }

        private bool FindTree(int nID, TeamData.DataType type, DataTreeNode<int, TeamData.DataType> parentNode, ref TeamData data)
        {
            TreeNodeCollection nodes = parentNode == null ? treeViewTeam.Nodes : parentNode.Nodes;
            int nCount = nodes.Count;

            if (parentNode == null && nodes.Count == 0)
            {
                FormTeam_Load(null, null);
                nCount = nodes.Count;
            }

            for (int i = 0; i < nCount; i++)
            {
                DataTreeNode<int, TeamData.DataType> node = (DataTreeNode<int, TeamData.DataType>)nodes[i];

                if (node.Type == type && node.Data == nID)
                {
                    data = new TeamData();

                    data.Name = node.Text;
                    data.FullName = GetFullPathName(node);
                    data.ID = node.Data;
                    data.Type = node.Type;

                    return true;
                }

                if (FindTree(nID, type, node, ref data))
                    return true;
            }

            return false;
        }

        private int FindTree(string strName, TreeNode parentNode, ref ArrayList arrTeamData)
        {
            TreeNodeCollection nodes = parentNode == null ? treeViewTeam.Nodes : parentNode.Nodes;
            int nCount = nodes.Count;

            if (parentNode == null && nodes.Count == 0)
            {
                FormTeam_Load(null, null);
                nCount = nodes.Count;
            }

            int nDataCount = 0;

            for (int i = 0; i < nCount; i++)
            {
                DataTreeNode<int, TeamData.DataType> node = (DataTreeNode<int, TeamData.DataType>)nodes[i];

                if (node.Text == strName)
                {
                    TeamData data = new TeamData();

                    data.Name = strName;
                    data.FullName = GetFullPathName(node);
                    data.ID = node.Data;
                    data.Type = node.Type;

                    arrTeamData.Add(data);
                    nDataCount++;
                }

                nDataCount += FindTree(strName, node, ref arrTeamData);
            }

            return nDataCount;
        }

        private void treeViewTeam_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextCheckMenu.Show(this, new Point(e.X, e.Y));
            }
        }

        private void VisibleCheckBoxMenu_Click(object sender, EventArgs e)
        {
            VisibleCheckBoxMenu.Checked = !VisibleCheckBoxMenu.Checked;
            if (VisibleCheckBoxMenu.Checked)
            {
                treeViewTeam.CheckBoxes = true;
            }
            else
            {
                treeViewTeam.CheckBoxes = false;
            }
        }
    }

    public class TeamData
    {
        public enum DataType { TeamData, MemberData };

        private string m_strName = "";
        private string m_strFullName = "";
        private int m_nID = 0;
        private DataType m_dataType = DataType.TeamData;

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public string FullName
        {
            get { return m_strFullName; }
            set { m_strFullName = value; }
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public DataType Type
        {
            get { return m_dataType; }
            set { m_dataType = value; }
        }
    }

    public class DataTreeNode<T, S> : TreeNode
    {
        private T m_data;
        private S m_type;

        public T Data
        {
            get { return m_data; }
            set { m_data = value; }
        }

        public S Type
        {
            get { return m_type; }
            set { m_type = value; }
        }
    }
}
