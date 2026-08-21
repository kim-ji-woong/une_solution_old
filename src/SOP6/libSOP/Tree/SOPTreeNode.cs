using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnE
{
    namespace SOP
    {

        namespace Tree
        {
            public enum TreeNodeType 
            {
                OTHER_NODE = 0,
                CATEGORY_NODE = 1,
                SUBCATEGOY_NODE = 2,
                DISASTER_NODE = 4,
                ACTIONSTEP_NODE = 8               
            }



            public class SOPTreeNode : TreeNode
            {
                private TreeNodeType m_nType = TreeNodeType.OTHER_NODE;
                public TreeNodeType TreeNodeType
                {
                    get { return m_nType; }
                    set { m_nType = value; }
                }

                private int m_nLinkedID = 0;
                public int LinkedID
                {
                    get { return m_nLinkedID; }
                    set { m_nLinkedID = value; }
                }

                private int m_nActionStepID = -1;
                public int ActionStepID
                {
                    get { return m_nActionStepID; }
                    set { m_nActionStepID = value; }
                }

                private int m_nDisasterID = -1;
                public int DisasterID
                {
                    get { return m_nDisasterID; }
                    set { m_nDisasterID = value; }
                }

                public SOPTreeNode()
                    : base()
                {                     
                }

                public SOPTreeNode(string text)
                    : base(text)
                {
                }

           
            }
        }
    }
}
