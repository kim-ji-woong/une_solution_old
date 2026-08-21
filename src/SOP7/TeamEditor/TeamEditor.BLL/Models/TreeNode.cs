using System.Collections.Generic;

namespace TeamEditor.BLL.Models
{
    public class TreeNode
    {
        private int m_nID = -1;
        private string m_strText = "";
        private object m_tag = null;
        private TreeNode m_parent = null;
        private List<TreeNode> m_childrens = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strText; }
            set { m_strText = value; }
        }
        public object Tag
        {
            get { return m_tag; }
            set { m_tag = value; }
        }
        public TreeNode Parent
        {
            get { return m_parent; }
            set { m_parent = value; }
        }
        
        public List<TreeNode> Children
        {
            get { return m_childrens; }
            set { m_childrens = value; }
        }

        public void ExpandAll()
        {
            // treeview node 다 펼쳐지는 기능 구현하기
        }
    }
}
