using System.Windows.Forms;

namespace UnE.Controls
{
    public class SOPNode : TreeNode
    {
        public SOPNode(string szNodeName)
            : base(szNodeName)
        { }

        protected string m_szTypeName = "";
        public virtual string TypeText
        {
            get { return m_szTypeName; }
            set { m_szTypeName = value; }
        }
    }
}
