using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SoilMan
{
    public class QuadTree
    {
        public const int TL = 0;
        public const int BL = 1;
        public const int BR = 2;
        public const int TR = 3;

        private const int DEFAULT_DEPTH_LEVEL = 4;

        private QuadNode[] m_nodes = null;

        public QuadNode[] Nodes
        {
            get { return m_nodes; }
        }

        public QuadTree()
        {
        }

        public QuadTree(float fTLx, float fTLy, float fBRx, float fBRy, int nDepth = DEFAULT_DEPTH_LEVEL)
        {
            MakeNode(fTLx, fTLy, fBRx, fBRy, nDepth);
        }

        public void MakeNode(float fTLx, float fTLy, float fBRx, float fBRy, int nDepth = DEFAULT_DEPTH_LEVEL)
        {
            m_nodes = null;
            GC.Collect();

            if (nDepth > 0)
            {
                m_nodes = QuadNode.MakeNodes(fTLx, fTLy, fBRx, fBRy);

                if (m_nodes != null)
                {
                    MakeNodes(m_nodes[TL], 1, nDepth);
                    MakeNodes(m_nodes[BL], 1, nDepth);
                    MakeNodes(m_nodes[BR], 1, nDepth);
                    MakeNodes(m_nodes[TR], 1, nDepth);
                }
            }
        }

        private void MakeNodes(QuadNode node, int nCurrentDepth, int nLimitDepth)
        {
            if (nCurrentDepth == nLimitDepth)
                return;

            QuadNode[] nodes = node.MakeChildren();

            if (nodes != null)
            {
                MakeNodes(nodes[TL], nCurrentDepth + 1, nLimitDepth);
                MakeNodes(nodes[BL], nCurrentDepth + 1, nLimitDepth);
                MakeNodes(nodes[BR], nCurrentDepth + 1, nLimitDepth);
                MakeNodes(nodes[TR], nCurrentDepth + 1, nLimitDepth);
            }
        }

        public void AddData(IQuadData data, int nIndex)
        {
            if (m_nodes != null)
            {
                m_nodes[TL].AddData(data, nIndex);
                m_nodes[BL].AddData(data, nIndex);
                m_nodes[BR].AddData(data, nIndex);
                m_nodes[TR].AddData(data, nIndex);
            }
        }

        // (x, y)에 해당하는 가장 하위 노드를 얻어온다.
        public QuadNode GetNode(float x, float y)
        {
            if (m_nodes == null)
                return null;

            QuadNode node = m_nodes[TL].GetNode(x, y);

            if (node != null)
                return node;

            node = m_nodes[BL].GetNode(x, y);

            if (node != null)
                return node;

            node = m_nodes[BR].GetNode(x, y);

            if (node != null)
                return node;

            node = m_nodes[TR].GetNode(x, y);

            if (node != null)
                return node;

            return null;
        }

        // [TL ~ BR] 사각영역에 존재하는 가장 하위 노드들의 리스트를 얻어온다.
        public List<QuadNode> GetNodes(float fTLx, float fTLy, float fBRx, float fBRy)
        {
            if (m_nodes == null)
                return null;

            RectangleF rect;

            if (fTLy > fBRy)
                rect = new RectangleF(fTLx, fBRy, fBRx - fTLx, fTLy - fBRy);
            else
                rect = new RectangleF(fTLx, fTLy, fBRx - fTLx, fBRy - fTLy);

            List<QuadNode> nodes = new List<QuadNode>();

            m_nodes[TL].GetNodes(rect, nodes);
            m_nodes[BL].GetNodes(rect, nodes);
            m_nodes[BR].GetNodes(rect, nodes);
            m_nodes[TR].GetNodes(rect, nodes);

            return nodes;
        }
    }

    public class QuadNode
    {
        private RectangleF m_boundary;
        private float m_fTLx = 0.0f, m_fTLy = 0.0f, m_fBRx = 0.0f, m_fBRy = 0.0f;
        // Data를 직접 저장하지 않고 Data Index만 저장한다.
        private List<int> m_datas = new List<int>();
        private QuadNode[] m_nodes = null;

        public List<int> Datas
        {
            get { return m_datas; }
        }

        public QuadNode[] Nodes
        {
            get { return m_nodes; }
        }

        public float Top
        {
            get { return m_fTLy; }
        }

        public float Left
        {
            get { return m_fTLx; }
        }

        public float Bottom
        {
            get { return m_fBRy; }
        }

        public float Right
        {
            get { return m_fBRx; }
        }

        public QuadNode()
        {
        }

        public QuadNode(float fTLx, float fTLy, float fBRx, float fBRy)
        {
            SetBoundary(fTLx, fTLy, fBRx, fBRy);
        }

        public void SetBoundary(float fTLx, float fTLy, float fBRx, float fBRy)
        {
            if (fTLy > fBRy)
            {
                m_fTLy = fTLy;
                m_fBRy = fBRy;
                m_boundary = new RectangleF(fTLx, fBRy, fBRx - fTLx, fTLy - fBRy);
            }
            else
            {
                m_fTLy = fBRy;
                m_fBRy = fTLy;
                m_boundary = new RectangleF(fTLx, fTLy, fBRx - fTLx, fBRy - fTLy);
            }

            m_fTLx = fTLx;
            m_fBRx = fBRx;
        }

        public static QuadNode[] MakeNodes(float fTLx, float fTLy, float fBRx, float fBRy)
        {
            if ((fBRx - fTLx) == 0.0f || (fTLy - fBRy) == 0.0f)
                return null;

            QuadNode[] nodes = new QuadNode[4];

            float fMidX = (fTLx + fBRx) / 2;
            float fMidY = (fTLy + fBRy) / 2;

            nodes[QuadTree.TL] = new QuadNode(fTLx, fTLy, fMidX, fMidY);
            nodes[QuadTree.BL] = new QuadNode(fTLx, fMidY, fMidX, fBRy);
            nodes[QuadTree.BR] = new QuadNode(fMidX, fMidY, fBRx, fBRy);
            nodes[QuadTree.TR] = new QuadNode(fMidX, fTLy, fBRx, fMidY);

            return nodes;
        }

        public QuadNode[] MakeChildren()
        {
            m_nodes = MakeNodes(m_fTLx, m_fTLy, m_fBRx, m_fBRy);
            return m_nodes;
        }

        public QuadNode GetChild(int nIndex)
        {
            if (nIndex < 0 || nIndex >= 4)
                return null;

            if (m_nodes == null)
                return null;

            return m_nodes[nIndex];
        }

        // 가장 마지막 Depth의 노드에 data가 저장된다.
        public bool AddData(IQuadData data, int nIndex)
        {
            RectangleF rect = data.GetBoundaryRectangle();

            if (!m_boundary.IntersectsWith(rect))
                return false;

            if (m_nodes == null)
            {
                // Data를 직접 저장하지 않고 Index를 저장한다.
                m_datas.Add(nIndex);
                //m_datas.Add(data);
            }
            else
            {
                m_nodes[QuadTree.TL].AddData(data, nIndex);
                m_nodes[QuadTree.BL].AddData(data, nIndex);
                m_nodes[QuadTree.BR].AddData(data, nIndex);
                m_nodes[QuadTree.TR].AddData(data, nIndex);
            }

            return true;
        }

        // (x, y)에 해당하는 가장 하위 노드를 얻어온다.
        public QuadNode GetNode(float x, float y)
        {
            if (x < m_fTLx || x > m_fBRx)
                return null;

            if (m_fTLy > m_fBRy)
            {
                if (y > m_fTLy || y < m_fBRy)
                    return null;
            }
            else
            {
                if (y > m_fBRy || y < m_fTLy)
                    return null;
            }

            if (m_nodes == null)
                return this;

            QuadNode node = m_nodes[QuadTree.TL].GetNode(x, y);

            if (node != null)
                return node;

            node = m_nodes[QuadTree.BL].GetNode(x, y);

            if (node != null)
                return node;

            node = m_nodes[QuadTree.BR].GetNode(x, y);

            if (node != null)
                return node;

            node = m_nodes[QuadTree.TR].GetNode(x, y);

            if (node != null)
                return node;

            // 여기까지 진행되면 에러
            // 노드가 잘못 만들어졌음
            return this;
        }

        // rect와 겹치는 영역에 존재하는 가장 하위 노드들의 리스트를 얻어온다.
        public void GetNodes(RectangleF rect, List<QuadNode> nodes)
        {
            if (!m_boundary.IntersectsWith(rect))
                return;

            if (m_nodes == null)
                nodes.Add(this);
            else
            {
                m_nodes[QuadTree.TL].GetNodes(rect, nodes);
                m_nodes[QuadTree.BL].GetNodes(rect, nodes);
                m_nodes[QuadTree.BR].GetNodes(rect, nodes);
                m_nodes[QuadTree.TR].GetNodes(rect, nodes);
            }
        }
    }

    public interface IQuadData
    {
        RectangleF GetBoundaryRectangle();
    }
}
