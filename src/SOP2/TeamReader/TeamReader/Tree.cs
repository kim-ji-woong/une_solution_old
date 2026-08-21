using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace TeamReader
{
    public class Tree<T>
    {
        public class Node
        {
            private T m_data = default(T);
            private Node m_parent = null;
            private List<Node> m_arrChildren = new List<Node>();
            
            public Node()
            {
            }

            public Node(T data)
            {
                m_data = data;
            }

            public bool Contains(Node node)
            {
                return m_arrChildren.Contains(node);
            }

            public void Add(Node node)
            {
                m_arrChildren.Add(node);
                node.Parent = this;
            }

            public void Remove(Node node)
            {
                m_arrChildren.Remove(node);
                node.Parent = null;
            }

            public bool RemoveAt(int nIndex)
            {
                if (m_arrChildren.Count >= nIndex)
                    return false;

                m_arrChildren[nIndex].Parent = null;
                m_arrChildren.RemoveAt(nIndex);
                return true;
            }

            public bool Insert(int nIndex, Node node)
            {
                if (m_arrChildren.Count == nIndex)
                {
                    m_arrChildren.Add(node);
                    node.Parent = this;
                    return true;
                }

                if (m_arrChildren.Count > nIndex)
                    return false;

                m_arrChildren.Insert(nIndex, node);
                node.Parent = this;
                return true;
            }

            public void Clear()
            {
                foreach (Node node in m_arrChildren)
                    node.Parent = null;

                m_arrChildren.Clear();
            }

            public T Data
            {
                get { return m_data; }
                set { m_data = value; }
            }

            public Node Parent
            {
                get { return m_parent; }
                set { m_parent = value; }
            }

            public List<Node> Children
            {
                get { return m_arrChildren; }
            }
        }

        private Node m_root = new Node();

        // node가 null이면 root 노드에서부터 찾는다.
        //        null이 아니면 node에서부터 찾는다.
        public Node Find(T data, Node node = null)
        {
            if (node == null)
                node = m_root;

            if (data.Equals(node.Data))
                return node;

            foreach (Node child in node.Children)
            {
                Node result = Find(data, child);
                if (result != null)
                    return result;
            }

            return null;
        }

        public Node Root
        {
            get { return m_root; }
        }
    }
}
