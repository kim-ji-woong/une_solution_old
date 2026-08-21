using System.Collections.Generic;

namespace TeamEditor.BLL.Models
{
    public class TreeView
    {        
        private List<TreeNode> m_children = null;
        public List<TreeNode> Children
        {
            get { return m_children; }
            set { m_children = value; }
        }

        public void ExpandAll()
        {
            // treeview node 다 펼쳐지는 기능 구현하기
        }
    }

    public class TeamTreeView
    {
        public enum TeamType { REGULAR = 0, TEMPORARY_NORMAL, TEMPORARY_EMERGENCY, EXTERNAL };
    }
}
