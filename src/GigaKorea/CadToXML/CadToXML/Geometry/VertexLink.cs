using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;

namespace CadToXML.Geometry
{
    //public class VertexLink
    //{
    //    public class Node
    //    {
    //        public enum NodeType { Normal = 0, EArcMiddle };

    //        private Vertex2D m_pos = null;
    //        private List<Node> m_likedNodes = new List<Node>();
    //        private NodeType m_nodeType = NodeType.Normal;

    //        public Vertex2D Position
    //        {
    //            get { return m_pos; }
    //            set { m_pos = value; }
    //        }

    //        public List<Node> LinkedNodes
    //        {
    //            get { return m_likedNodes; }
    //        }

    //        public Node()
    //        {
    //            m_pos = new Vertex2D();
    //        }

    //        public Node(Vertex2D vPos)
    //        {
    //            m_pos = vPos;
    //        }

    //        public Node(Vertex2D vPos, NodeType nodeType)
    //        {
    //            m_pos = vPos;
    //            m_nodeType = nodeType;
    //        }

    //        public void AddLink(Node node)
    //        {
    //            foreach (Node link in m_likedNodes)
    //            {
    //                if (link == node)
    //                    return;
    //            }

    //            m_likedNodes.Add(node);
    //        }

    //        public NodeType GetNodeType()
    //        {
    //            return m_nodeType;
    //        }

    //        public void SetNodeType(NodeType nodeType)
    //        {
    //            m_nodeType = nodeType;
    //        }
    //    }

    //    public class EArcNodeGroup
    //    {
    //        private Node m_earcBeginNode = null;
    //        private Node m_earcMiddleNode = null;
    //        private Node m_earcEndNode = null;
    //        private bool m_isClosed = false;

    //        public Node BeginNode
    //        {
    //            get { return m_earcBeginNode; }
    //            set { m_earcBeginNode = value; }
    //        }

    //        public Node MiddleNode
    //        {
    //            get { return m_earcMiddleNode; }
    //            set { m_earcMiddleNode = value; }
    //        }

    //        public Node EndNode
    //        {
    //            get { return m_earcEndNode; }
    //            set { m_earcEndNode = value; }
    //        }

    //        public bool IsClosed
    //        {
    //            get { return m_isClosed; }
    //            set { m_isClosed = value; }
    //        }

    //        public EArcNodeGroup()
    //        {
    //        }

    //        public EArcNodeGroup(EArc2D earc)
    //        {
    //            SetEArc(earc, null, null);
    //        }

    //        public EArcNodeGroup(EArc2D earc, Node node, bool isBeginNode)
    //        {
    //            if (isBeginNode)
    //                SetEArc(earc, node, null);
    //            else
    //                SetEArc(earc, null, node);
    //        }

    //        public EArcNodeGroup(EArc2D earc, Node beginNode, Node endNode)
    //        {
    //            SetEArc(earc, beginNode, endNode);
    //        }

    //        private bool SetEArc(EArc2D earc, Node beginNode, Node endNode)
    //        {
    //            Vertex2D vBegin = earc.GetBeginVertex();
    //            Vertex2D vEnd = earc.GetEndVertex();

    //            double dMiddleAngle = earc.IsClockWise() ? earc.GetBeginAngle() - earc.GetAngle() / 2 : earc.GetBeginAngle() + earc.GetAngle() / 2;

    //            Vertex2D vMiddle;

    //            if (earc.GetVertex(dMiddleAngle, out vMiddle))
    //            {
    //                m_earcBeginNode = beginNode == null ? new Node(vBegin) : beginNode;
    //                m_earcEndNode = endNode == null ? new Node(vEnd) : endNode;
    //                m_earcMiddleNode = new Node(vMiddle, Node.NodeType.EArcMiddle);

    //                m_earcBeginNode.AddLink(m_earcMiddleNode);
    //                m_earcEndNode.AddLink(m_earcMiddleNode);
    //                m_earcMiddleNode.AddLink(m_earcBeginNode);
    //                m_earcMiddleNode.AddLink(m_earcEndNode);
    //                m_isClosed = earc.IsClosed();

    //                return true;
    //            }

    //            return false;
    //        }
    //    }

    //    private List<Node> m_nodes = new List<Node>();
    //    private Dictionary<Line2D, KeyValuePair<Node, Node>> m_dicLineVertex = new Dictionary<Line2D, KeyValuePair<Node, Node>>();
    //    private Dictionary<EArc2D, EArcNodeGroup> m_dicEArcVertex = new Dictionary<EArc2D, EArcNodeGroup>();

    //    public List<Node> Nodes
    //    {
    //        get { return m_nodes; }
    //    }

    //    private string VertexToString(Vertex2D vertex)
    //    {
    //        long x = (long)(vertex.x + 0.5);
    //        long y = (long)(vertex.y + 0.5);
    //        return x.ToString() + "," + y.ToString();
    //    }

    //    /*private long VertexToLong(Vertex2D vertex)
    //    {
    //        long x = (long)(vertex.x + 0.5);
    //        long y = (long)(vertex.y + 0.5);

    //        long key = ((x << 32) | y);
    //        return key;
    //    }*/

    //    public void AddLine(Line2D line)
    //    {
    //        if (m_nodes.Count == 0)
    //        {
    //            AddLineNode(line);
    //        }
    //        else
    //        {
    //            AddLineToLine(line);
    //            AddLineToEArc(line);
    //            /*Vertex2D v1, v2;
    //            Line2D.LineType resultType;

    //            Dictionary<Line2D, Vertex2D> dicSplitLines = new Dictionary<Line2D, Vertex2D>();
    //            List<Line2D> removeLines = new List<Line2D>();

    //            // Key : 좌표의 소숫점 첫째자리에서 반올림 한 이후 정수로 변환하여 상위 4바이트는 x, 하위 4바이트는 y값으로 만든다.
    //            //       long 대신 string으로 대체한다.(값이 음수일때 처리가 어렵다.)
    //            Dictionary<string, Node> recycleNode = new Dictionary<string, Node>();
    //            //Dictionary<Vertex2D, Node> recycleNode = new Dictionary<Vertex2D, Node>();

    //            // line이 몇개의 버텍스로 쪼개어지는가를 기록하기위한 List
    //            List<Vertex2D> linearVertices = new List<Vertex2D>();
    //            linearVertices.Add(line.GetVertex(true));
    //            linearVertices.Add(line.GetVertex(false));

    //            foreach (KeyValuePair<Line2D, KeyValuePair<Node, Node>> pair in m_dicLineVertex)
    //            {
    //                int nResult = pair.Key.IntersectLine(line, out v1, out v2, out resultType);

    //                // 한점에서 만날 경우
    //                if (nResult == 1)
    //                {
    //                    bool sameBegin = IsSameVertex(v1, pair.Value.Key.Position);
    //                    bool sameEnd = IsSameVertex(v1, pair.Value.Value.Position);

    //                    if (sameBegin == false && sameEnd == false)
    //                    {
    //                        // pair.Key는 v1에 의하여 쪼개어진다.
    //                        dicSplitLines[pair.Key] = v1;
    //                    }
    //                    else if (sameBegin)
    //                    {
    //                        recycleNode[VertexToString(v1)] = pair.Value.Key;
    //                        //recycleNode[VertexToLong(v1)] = pair.Value.Key;
    //                    }
    //                    else if (sameEnd)
    //                    {
    //                        recycleNode[VertexToString(v1)] = pair.Value.Value;
    //                        //recycleNode[VertexToLong(v1)] = pair.Value.Value;
    //                    }

    //                    AddLinearVertex(linearVertices, v1);
    //                }
    //                // 두점에서 만날 경우
    //                else if (nResult == 2)
    //                {
    //                    // v1이 pair.Key 내에 포함되어 있는가?
    //                    bool v1Include = pair.Key.IsInclude(v1);
    //                    // v2가 pair.Key 내에 포함되어 있는가?
    //                    bool v2Include = pair.Key.IsInclude(v2);

    //                    if (v1Include && !v2Include)
    //                    {
    //                        if (v2.GetDistance(pair.Value.Key.Position) < v2.GetDistance(pair.Value.Value.Position))
    //                            SetLinearVertexBegin(linearVertices, pair.Value.Key.Position);
    //                        else
    //                            SetLinearVertexBegin(linearVertices, pair.Value.Value.Position);
    //                    }
    //                    else if (!v1Include && v2Include)
    //                    {
    //                        if (v1.GetDistance(pair.Value.Key.Position) < v1.GetDistance(pair.Value.Value.Position))
    //                            SetLinearVertexEnd(linearVertices, pair.Value.Key.Position);
    //                        else
    //                            SetLinearVertexEnd(linearVertices, pair.Value.Value.Position);
    //                    }
    //                    else// if (v1Include && v2Include)
    //                    {
    //                        if (pair.Key.GetVertex(true).GetDistance(pair.Key.GetVertex(false)) > line.GetVertex(true).GetDistance(line.GetVertex(false)))
    //                        {
    //                            // line이 pair.Key내에 완전히 포함되어 있는 경우
    //                            continue;
    //                        }
    //                        else
    //                        {
    //                            // pair.Key가 line내에 완전히 속해있는 경우
    //                            removeLines.Add(pair.Key);
    //                            AddLinearVertex(linearVertices, pair.Key.GetVertex(true));
    //                            AddLinearVertex(linearVertices, pair.Key.GetVertex(false));

    //                            recycleNode[VertexToString(pair.Key.GetVertex(true))] = pair.Value.Key;
    //                            recycleNode[VertexToString(pair.Key.GetVertex(false))] = pair.Value.Value;
    //                        }
    //                    }
    //                }
    //            }

    //            foreach (Line2D removeLine in removeLines)
    //            {
    //                m_dicLineVertex.Remove(removeLine);
    //            }

    //            Node prevNode = null;
    //            List<Node> linearNodes = new List<Node>();

    //            foreach (Vertex2D vPos in linearVertices)
    //            {
    //                Node node = FindRecycleNode(vPos, recycleNode);

    //                if (node == null)
    //                //if (recycleNode.TryGetValue(vPos, out node) == false)
    //                {
    //                    node = FindNode(vPos);

    //                    if (node == null)
    //                    {
    //                        node = new Node(vPos);
    //                        m_nodes.Add(node);
    //                    }
    //                }

    //                linearNodes.Add(node);

    //                if (prevNode != null)
    //                {
    //                    prevNode.AddLink(node);
    //                    node.AddLink(prevNode);

    //                    m_dicLineVertex[new Line2D(prevNode.Position, node.Position)] = new KeyValuePair<Node, Node>(prevNode, node);
    //                }

    //                prevNode = node;
    //            }

    //            foreach (KeyValuePair<Line2D, Vertex2D> pair in dicSplitLines)
    //            {
    //                SplitLine(pair.Key, pair.Value, linearNodes);
    //            }*/
    //        }
    //    }

    //    public void AddEArc(EArc2D earc)
    //    {
    //        if (m_nodes.Count == 0)
    //        {
    //            AddEArcNode(earc);
    //        }
    //        else
    //        {
    //            //AddEArcToLine(earc);
    //            //AddEArcToEArc(earc);
    //        }
    //    }

    //    private void AddLineToLine(Line2D line)
    //    {
    //        Vertex2D v1, v2;
    //        Line2D.LineType resultType;

    //        Dictionary<Line2D, Vertex2D> dicSplitLines = new Dictionary<Line2D, Vertex2D>();
    //        List<Line2D> removeLines = new List<Line2D>();

    //        // Key : 좌표의 소숫점 첫째자리에서 반올림 한 이후 정수로 변환하여 상위 4바이트는 x, 하위 4바이트는 y값으로 만든다.
    //        //       long 대신 string으로 대체한다.(값이 음수일때 처리가 어렵다.)
    //        Dictionary<string, Node> recycleNode = new Dictionary<string, Node>();
    //        //Dictionary<Vertex2D, Node> recycleNode = new Dictionary<Vertex2D, Node>();

    //        // line이 몇개의 버텍스로 쪼개어지는가를 기록하기위한 List
    //        List<Vertex2D> linearVertices = new List<Vertex2D>();
    //        linearVertices.Add(line.GetVertex(true));
    //        linearVertices.Add(line.GetVertex(false));

    //        foreach (KeyValuePair<Line2D, KeyValuePair<Node, Node>> pair in m_dicLineVertex)
    //        {
    //            int nResult = pair.Key.IntersectLine(line, out v1, out v2, out resultType);

    //            // 한점에서 만날 경우
    //            if (nResult == 1)
    //            {
    //                bool sameBegin = IsSameVertex(v1, pair.Value.Key.Position);
    //                bool sameEnd = IsSameVertex(v1, pair.Value.Value.Position);

    //                if (sameBegin == false && sameEnd == false)
    //                {
    //                    // pair.Key는 v1에 의하여 쪼개어진다.
    //                    dicSplitLines[pair.Key] = v1;
    //                }
    //                else if (sameBegin)
    //                {
    //                    recycleNode[VertexToString(v1)] = pair.Value.Key;
    //                    //recycleNode[VertexToLong(v1)] = pair.Value.Key;
    //                }
    //                else if (sameEnd)
    //                {
    //                    recycleNode[VertexToString(v1)] = pair.Value.Value;
    //                    //recycleNode[VertexToLong(v1)] = pair.Value.Value;
    //                }

    //                AddLinearVertex(linearVertices, v1);
    //            }
    //            // 두점에서 만날 경우
    //            else if (nResult == 2)
    //            {
    //                // v1이 pair.Key 내에 포함되어 있는가?
    //                bool v1Include = pair.Key.IsInclude(v1);
    //                // v2가 pair.Key 내에 포함되어 있는가?
    //                bool v2Include = pair.Key.IsInclude(v2);

    //                if (v1Include && !v2Include)
    //                {
    //                    if (v2.GetDistance(pair.Value.Key.Position) < v2.GetDistance(pair.Value.Value.Position))
    //                        SetLinearVertexBegin(linearVertices, pair.Value.Key.Position);
    //                    else
    //                        SetLinearVertexBegin(linearVertices, pair.Value.Value.Position);
    //                }
    //                else if (!v1Include && v2Include)
    //                {
    //                    if (v1.GetDistance(pair.Value.Key.Position) < v1.GetDistance(pair.Value.Value.Position))
    //                        SetLinearVertexEnd(linearVertices, pair.Value.Key.Position);
    //                    else
    //                        SetLinearVertexEnd(linearVertices, pair.Value.Value.Position);
    //                }
    //                else// if (v1Include && v2Include)
    //                {
    //                    if (pair.Key.GetVertex(true).GetDistance(pair.Key.GetVertex(false)) > line.GetVertex(true).GetDistance(line.GetVertex(false)))
    //                    {
    //                        // line이 pair.Key내에 완전히 포함되어 있는 경우
    //                        continue;
    //                    }
    //                    else
    //                    {
    //                        // pair.Key가 line내에 완전히 속해있는 경우
    //                        removeLines.Add(pair.Key);
    //                        AddLinearVertex(linearVertices, pair.Key.GetVertex(true));
    //                        AddLinearVertex(linearVertices, pair.Key.GetVertex(false));

    //                        recycleNode[VertexToString(pair.Key.GetVertex(true))] = pair.Value.Key;
    //                        recycleNode[VertexToString(pair.Key.GetVertex(false))] = pair.Value.Value;
    //                    }
    //                }
    //                /*else
    //                {
    //                    // pair.Key가 line내에 완전히 속해있는 경우
    //                    removeLines.Add(pair.Key);
    //                    AddLinearVertex(linearVertices, pair.Key.GetVertex(true));
    //                    AddLinearVertex(linearVertices, pair.Key.GetVertex(false));

    //                    recycleNode[VertexToString(pair.Key.GetVertex(true))] = pair.Value.Key;
    //                    recycleNode[VertexToString(pair.Key.GetVertex(false))] = pair.Value.Value;
    //                    //recycleNode[VertexToLong(pair.Key.GetVertex(true))] = pair.Value.Key;
    //                    //recycleNode[VertexToLong(pair.Key.GetVertex(false))] = pair.Value.Value;
    //                }*/
    //            }
    //        }

    //        foreach (Line2D removeLine in removeLines)
    //        {
    //            m_dicLineVertex.Remove(removeLine);
    //        }

    //        Node prevNode = null;
    //        List<Node> linearNodes = new List<Node>();

    //        foreach (Vertex2D vPos in linearVertices)
    //        {
    //            Node node = FindRecycleNode(vPos, recycleNode);

    //            if (node == null)
    //            //if (recycleNode.TryGetValue(vPos, out node) == false)
    //            {
    //                node = FindNode(vPos);

    //                if (node == null)
    //                {
    //                    node = new Node(vPos);
    //                    m_nodes.Add(node);
    //                }
    //            }

    //            linearNodes.Add(node);

    //            if (prevNode != null)
    //            {
    //                prevNode.AddLink(node);
    //                node.AddLink(prevNode);

    //                m_dicLineVertex[new Line2D(prevNode.Position, node.Position)] = new KeyValuePair<Node, Node>(prevNode, node);
    //            }

    //            prevNode = node;
    //        }

    //        foreach (KeyValuePair<Line2D, Vertex2D> pair in dicSplitLines)
    //        {
    //            SplitLine(pair.Key, pair.Value, linearNodes);
    //        }
    //    }

    //    private void AddLineToEArc(Line2D line)
    //    {
    //        Vertex2D v1, v2;
            
    //        // EArc가 하나의 Vertex에 의하여 쪼개지는 경우
    //        Dictionary<EArc2D, Vertex2D> dicSplitEArcVertex = new Dictionary<EArc2D, Vertex2D>();
    //        // EArc가 두개의 Vertex에 의하여 쪼개지는 경우
    //        Dictionary<EArc2D, Line2D> dicSplitEArcLine = new Dictionary<EArc2D, Line2D>();
            
    //        // Key : 좌표의 소숫점 첫째자리에서 반올림 한 이후 정수로 변환하여 상위 4바이트는 x, 하위 4바이트는 y값으로 만든다.
    //        //       long 대신 string으로 대체한다.(값이 음수일때 처리가 어렵다.)
    //        Dictionary<string, Node> recycleNode = new Dictionary<string, Node>();

    //        // line이 몇개의 버텍스로 쪼개어지는가를 기록하기위한 List
    //        List<Vertex2D> linearVertices = new List<Vertex2D>();
    //        linearVertices.Add(line.GetVertex(true));
    //        linearVertices.Add(line.GetVertex(false));

    //        foreach (KeyValuePair<EArc2D, EArcNodeGroup> pair in m_dicEArcVertex)
    //        {
    //            int nResult = pair.Key.IntersectLine(line, out v1, out v2);

    //            EArcNodeGroup earcNodeGroup = pair.Value;

    //            // 한점에서 만날 경우
    //            if (nResult == 1)
    //            {
    //                bool sameBegin = IsSameVertex(v1, earcNodeGroup.BeginNode.Position);
    //                bool sameEnd = IsSameVertex(v1, earcNodeGroup.EndNode.Position);

    //                if (sameBegin == false && sameEnd == false)
    //                {
    //                    // pair.Key는 v1에 의하여 쪼개어진다.
    //                    dicSplitEArcVertex[pair.Key] = v1;
    //                }
    //                else if (sameBegin)
    //                {
    //                    recycleNode[VertexToString(v1)] = earcNodeGroup.BeginNode;
    //                }
    //                else if (sameEnd)
    //                {
    //                    recycleNode[VertexToString(v1)] = earcNodeGroup.EndNode;
    //                }

    //                AddLinearVertex(linearVertices, v1);
    //            }
    //            // 두점에서 만날 경우
    //            else if (nResult == 2)
    //            {
    //                // v1이 pair.Key의 시작점인가?
    //                bool v1IsBegin = IsSameVertex(v1, earcNodeGroup.BeginNode.Position);
    //                bool v1IsEnd = IsSameVertex(v1, earcNodeGroup.EndNode.Position);
    //                // v2가 pair.Key의 시작점인가?
    //                bool v2IsBegin = IsSameVertex(v2, earcNodeGroup.BeginNode.Position);
    //                bool v2IsEnd = IsSameVertex(v2, earcNodeGroup.EndNode.Position);

    //                if ((v1IsBegin || v1IsEnd) && (v2IsBegin || v2IsEnd))
    //                {
    //                    if (v1IsBegin)
    //                    {
    //                        recycleNode[VertexToString(v1)] = earcNodeGroup.BeginNode;
    //                        recycleNode[VertexToString(v2)] = earcNodeGroup.EndNode;
    //                    }
    //                    else
    //                    {
    //                        recycleNode[VertexToString(v1)] = earcNodeGroup.EndNode;
    //                        recycleNode[VertexToString(v2)] = earcNodeGroup.BeginNode;
    //                    }
    //                }
    //                else if (v1IsBegin || v1IsEnd)
    //                {
    //                    // pair.Key는 v1에 의하여 쪼개어진다.
    //                    dicSplitEArcVertex[pair.Key] = v1;

    //                    if (v1IsBegin)
    //                    {
    //                        recycleNode[VertexToString(v1)] = earcNodeGroup.BeginNode;
    //                    }
    //                    else
    //                    {
    //                        recycleNode[VertexToString(v1)] = earcNodeGroup.EndNode;
    //                    }
    //                }
    //                else if (v2IsBegin || v2IsEnd)
    //                {
    //                    // pair.Key는 v2에 의하여 쪼개어진다.
    //                    dicSplitEArcVertex[pair.Key] = v2;

    //                    if (v2IsBegin)
    //                    {
    //                        recycleNode[VertexToString(v2)] = earcNodeGroup.BeginNode;
    //                    }
    //                    else
    //                    {
    //                        recycleNode[VertexToString(v2)] = earcNodeGroup.EndNode;
    //                    }
    //                }
    //                else
    //                {
    //                    // pair.Key가 v1과 v2에 의하여 세조각으로 쪼개어진다.
    //                    Line2D splitLine = new Line2D(v1, v2);
    //                    dicSplitEArcLine[pair.Key] = splitLine;
    //                }

    //                AddLinearVertex(linearVertices, v1);
    //                AddLinearVertex(linearVertices, v2);
    //            }
    //        }

    //        Node prevNode = null;
    //        List<Node> linearNodes = new List<Node>();

    //        foreach (Vertex2D vPos in linearVertices)
    //        {
    //            Node node = FindRecycleNode(vPos, recycleNode);

    //            if (node == null)
    //            {
    //                node = FindNode(vPos);

    //                if (node == null)
    //                {
    //                    node = new Node(vPos);
    //                    m_nodes.Add(node);
    //                }
    //            }

    //            linearNodes.Add(node);

    //            if (prevNode != null)
    //            {
    //                prevNode.AddLink(node);
    //                node.AddLink(prevNode);

    //                m_dicLineVertex[new Line2D(prevNode.Position, node.Position)] = new KeyValuePair<Node, Node>(prevNode, node);
    //            }

    //            prevNode = node;
    //        }

    //        foreach (KeyValuePair<EArc2D, Vertex2D> pair in dicSplitEArcVertex)
    //        {
    //            SplitEArc(pair.Key, pair.Value, linearNodes);
    //        }

    //        foreach (KeyValuePair<EArc2D, Line2D> pair in dicSplitEArcLine)
    //        {
    //            SplitEArc(pair.Key, pair.Value.GetVertex(true), pair.Value.GetVertex(false), linearNodes);
    //        }
    //    }

    //    private Node FindRecycleNode(Vertex2D vPos, Dictionary<string, Node> recycleNodes)
    //    {
    //        string key = VertexToString(vPos);
    //        //long key = VertexToLong(vPos);
    //        Node node;

    //        if (recycleNodes.TryGetValue(key, out node))
    //            return node;

    //        return null;
    //    }
    //    /*private Node FindRecycleNode(Vertex2D vPos, Dictionary<Vertex2D, Node> recycleNodes)
    //    {
    //        foreach (KeyValuePair<Vertex2D, Node> pair in recycleNodes)
    //        {
    //            if (IsSameVertex(vPos, pair.Key))
    //                return pair.Value;
    //        }

    //        return null;
    //    }*/

    //    private void SplitEArc(EArc2D earc, Vertex2D vertex, List<Node> linearNodes)
    //    {
    //        EArcNodeGroup nodeGroup;

    //        if (m_dicEArcVertex.TryGetValue(earc, out nodeGroup))
    //        {
    //            double angle = GetEArcAngle(earc, vertex);
    //            double dBeginEArcAngle, dEndEArcAngle;
    //            GetEArcSplitAngle(earc, angle, out dBeginEArcAngle, out dEndEArcAngle);

    //            EArc2D earc1 = new EArc2D(earc.GetTL(), earc.GetBL(), earc.GetBR(), earc.GetBeginAngle(), dBeginEArcAngle, earc.IsClockWise());
    //            EArc2D earc2 = new EArc2D(earc.GetTL(), earc.GetBL(), earc.GetBR(), angle, dEndEArcAngle, earc.IsClockWise());

    //            Node newNode = GetLinearNode(vertex, linearNodes);
    //            EArcNodeGroup group1 = new EArcNodeGroup(earc1, nodeGroup.BeginNode, newNode);
    //            EArcNodeGroup group2 = new EArcNodeGroup(earc2, newNode, nodeGroup.EndNode);

    //            m_dicEArcVertex[earc1] = group1;
    //            m_dicEArcVertex[earc2] = group2;

    //            m_nodes.Add(group1.MiddleNode);
    //            m_nodes.Add(group2.MiddleNode);

    //            foreach (Node node in nodeGroup.MiddleNode.LinkedNodes)
    //            {
    //                node.LinkedNodes.Remove(nodeGroup.BeginNode);
    //            }

    //            m_nodes.Remove(nodeGroup.MiddleNode);
    //            m_dicEArcVertex.Remove(earc);
    //        }
    //    }

    //    private void SplitEArc(EArc2D earc, Vertex2D v1, Vertex2D v2, List<Node> linearNodes)
    //    {
    //        EArcNodeGroup nodeGroup;

    //        if (m_dicEArcVertex.TryGetValue(earc, out nodeGroup))
    //        {
    //            double angle1 = GetEArcAngle(earc, v1);
    //            double angle2 = GetEArcAngle(earc, v2);

    //            double dBeginEArcAngle1, dEndEArcAngle1, dBeginEArcAngle2, dEndEArcAngle2;
    //            GetEArcSplitAngle(earc, angle1, out dBeginEArcAngle1, out dEndEArcAngle1);
    //            GetEArcSplitAngle(earc, angle2, out dBeginEArcAngle2, out dEndEArcAngle2);

    //            if (System.Math.Abs(dBeginEArcAngle1) < System.Math.Abs(dBeginEArcAngle2))
    //            {
    //                SplitEArc(earc, v1, v2, angle1, angle2, dBeginEArcAngle1, earc.GetAngle() - dBeginEArcAngle1 - dEndEArcAngle2, dEndEArcAngle2, linearNodes);
    //            }
    //            else
    //            {
    //                SplitEArc(earc, v2, v1, angle2, angle1, dBeginEArcAngle2, earc.GetAngle() - dBeginEArcAngle2 - dEndEArcAngle1, dEndEArcAngle1, linearNodes);
    //            }
    //        }
    //    }

    //    private void SplitEArc(EArc2D earc, Vertex2D v1, Vertex2D v2, double angle1, double angle2, double dEArcAngle1, double dEArcAngle2, double dEArcAngle3, List<Node> linearNodes)
    //    {
    //        EArcNodeGroup nodeGroup;

    //        if (m_dicEArcVertex.TryGetValue(earc, out nodeGroup))
    //        {
    //            EArc2D earc1 = new EArc2D(earc.GetTL(), earc.GetBL(), earc.GetBR(), earc.GetBeginAngle(), dEArcAngle1, earc.IsClockWise());
    //            EArc2D earc2 = new EArc2D(earc.GetTL(), earc.GetBL(), earc.GetBR(), angle1, dEArcAngle2, earc.IsClockWise());
    //            EArc2D earc3 = new EArc2D(earc.GetTL(), earc.GetBL(), earc.GetBR(), angle2, dEArcAngle3, earc.IsClockWise());

    //            Node newNode1 = GetLinearNode(v1, linearNodes);
    //            Node newNode2 = GetLinearNode(v2, linearNodes);
    //            EArcNodeGroup group1 = new EArcNodeGroup(earc1, newNode1, false);
    //            EArcNodeGroup group2 = new EArcNodeGroup(earc2, newNode1, newNode2);
    //            EArcNodeGroup group3 = new EArcNodeGroup(earc3, newNode2, true);

    //            m_dicEArcVertex[earc1] = group1;
    //            m_dicEArcVertex[earc2] = group2;
    //            m_dicEArcVertex[earc3] = group3;

    //            m_nodes.Add(group1.MiddleNode);
    //            m_nodes.Add(group2.MiddleNode);
    //            m_nodes.Add(group3.MiddleNode);

    //            foreach (Node node in nodeGroup.MiddleNode.LinkedNodes)
    //            {
    //                node.LinkedNodes.Remove(nodeGroup.BeginNode);
    //            }

    //            m_nodes.Remove(nodeGroup.MiddleNode);
    //            m_dicEArcVertex.Remove(earc);
    //        }
    //    }

    //    private static double GetEArcAngle(EArc2D earc, Vertex2D vertex)
    //    {
    //        Vertex2D vCenter = earc.GetCenter();
    //        Vertex2D vRight = new Vertex2D(vCenter.x + 100, vCenter.y);
    //        double dAngle = UnE.Geometry.Math.GetAngle(vertex, vCenter, vRight);

    //        if (vertex.y < vCenter.y)
    //            dAngle = UnE.Geometry.Math._2PI() - dAngle;

    //        return dAngle;
    //    }

    //    // earc가 dAngle 지점을 기준으로 둘로 쪼개어질 경우
    //    // 시작점 쪽의 BeginEArc와 끝점 쪽의 EndEArc가 생기게 되는데
    //    // 각각의 EArcAngle을 구한다.
    //    private static void GetEArcSplitAngle(EArc2D earc, double dAngle, out double dBeginEArcAngle, out double dEndEArcAngle)
    //    {
    //        double dBeginAngle = earc.GetBeginAngle();
    //        double dEndAngle = earc.GetEndAngle();
    //        double dEArcAngle = earc.GetAngle();

    //        if (earc.IsClockWise())
    //        {
    //            if (dEArcAngle > 0.0)
    //            {
    //                if (dBeginAngle > dEndAngle)
    //                {
    //                    dBeginEArcAngle = dBeginAngle - dAngle;
    //                }
    //                else
    //                {
    //                    if (dAngle >= 0.0)
    //                        dBeginEArcAngle = dBeginAngle - dAngle;
    //                    else
    //                        dBeginEArcAngle = dBeginAngle + (UnE.Geometry.Math._2PI() - dAngle);
    //                }
    //            }
    //            else
    //            {
    //                if (dEndAngle > dBeginAngle)
    //                {
    //                    dBeginEArcAngle = dBeginAngle - dAngle;
    //                }
    //                else
    //                {
    //                    if (dAngle >= 0.0)
    //                        dBeginEArcAngle = dBeginAngle - UnE.Geometry.Math._2PI() - dAngle;
    //                    else
    //                        dBeginEArcAngle = dBeginAngle - dAngle;
    //                }
    //            }
    //        }
    //        else
    //        {
    //            if (dEArcAngle > 0.0)
    //            {
    //                if (dEndAngle > dBeginAngle)
    //                {
    //                    dBeginEArcAngle = dAngle - dBeginAngle;
    //                }
    //                else
    //                {
    //                    if (dAngle >= 0.0)
    //                        dBeginEArcAngle = dAngle + (UnE.Geometry.Math._2PI() - dBeginAngle);
    //                    else
    //                        dBeginEArcAngle = dAngle - dBeginAngle;
    //                }
    //            }
    //            else
    //            {
    //                if (dBeginAngle > dEndAngle)
    //                {
    //                    dBeginEArcAngle = dAngle - dBeginAngle;
    //                }
    //                else
    //                {
    //                    if (dAngle >= 0.0)
    //                        dBeginEArcAngle = dAngle - dBeginAngle;
    //                    else
    //                        dBeginEArcAngle = dAngle - UnE.Geometry.Math._2PI() - dBeginAngle;
    //                }
    //            }
    //        }

    //        dEndEArcAngle = dEArcAngle - dBeginEArcAngle;
    //    }

    //    private void SplitLine(Line2D line, Vertex2D vertex, List<Node> linearNodes)
    //    {
    //        KeyValuePair<Node, Node> pair;

    //        if (m_dicLineVertex.TryGetValue(line, out pair))
    //        {
    //            Line2D lineBegin = new Line2D(pair.Key.Position, vertex);
    //            Line2D lineEnd = new Line2D(vertex, pair.Value.Position);
    //            Node newNode = GetLinearNode(vertex, linearNodes);

    //            if (newNode != null)
    //            {
    //                pair.Key.AddLink(newNode);
    //                pair.Value.AddLink(newNode);
    //                newNode.AddLink(pair.Key);
    //                newNode.AddLink(pair.Value);

    //                m_dicLineVertex[lineBegin] = new KeyValuePair<Node, Node>(pair.Key, newNode);
    //                m_dicLineVertex[lineEnd] = new KeyValuePair<Node, Node>(newNode, pair.Value);

    //                if (newNode != pair.Key && newNode != pair.Value)
    //                {
    //                    pair.Key.LinkedNodes.Remove(pair.Value);
    //                    pair.Value.LinkedNodes.Remove(pair.Key);
    //                }
    //            }

    //            m_dicLineVertex.Remove(line);
    //        }
    //    }

    //    private Node GetLinearNode(Vertex2D vertex, List<Node> linearNodes)
    //    {
    //        foreach (Node node in linearNodes)
    //        {
    //            if (IsSameVertex(vertex, node.Position))
    //            {
    //                return node;
    //            }
    //        }

    //        return null;
    //    }

    //    // vNew보다 앞에 있는 Vertex들을 모두 지우고, vNew를 시작점으로 한다.
    //    private void SetLinearVertexBegin(List<Vertex2D> linearVertices, Vertex2D vNew)
    //    {
    //        int nVertexCount = linearVertices.Count;
    //        double dPrevLen = 0.0;

    //        for (int i = 0; i < nVertexCount; i++)
    //        {
    //            Vertex2D vertex = linearVertices[i];

    //            double dLen = vertex.GetDistance(vNew);

    //            if (IsSame(dLen))
    //            {
    //                for (int j = 0; j < i; j++)
    //                    linearVertices.RemoveAt(0);

    //                return;
    //            }

    //            if (i == 0)
    //                dPrevLen = dLen;
    //            else
    //            {
    //                double dLen2 = vertex.GetDistance(linearVertices[i - 1]);

    //                if (dPrevLen < dLen2)
    //                {
    //                    for (int j = 0; j < i; j++)
    //                        linearVertices.RemoveAt(0);

    //                    linearVertices.Insert(0, vNew);
    //                    return;
    //                }
    //                else
    //                    dPrevLen = dLen;
    //            }
    //        }
    //    }

    //    // vNew보다 뒤에 있는 Vertex들을 모두 지우고, vNew를 끝점으로 한다.
    //    private void SetLinearVertexEnd(List<Vertex2D> linearVertices, Vertex2D vNew)
    //    {
    //        int nVertexCount = linearVertices.Count;
    //        double dPrevLen = 0.0;

    //        for (int i = nVertexCount - 1; i >= 0; i--)
    //        {
    //            Vertex2D vertex = linearVertices[i];

    //            double dLen = vertex.GetDistance(vNew);

    //            if (IsSame(dLen))
    //            {
    //                for (int j = i + 1; j < nVertexCount; j++)
    //                    linearVertices.RemoveAt(i + 1);

    //                return;
    //            }

    //            if (i == nVertexCount - 1)
    //                dPrevLen = dLen;
    //            else
    //            {
    //                double dLen2 = vertex.GetDistance(linearVertices[i + 1]);

    //                if (dPrevLen < dLen2)
    //                {
    //                    for (int j = i + 1; j < nVertexCount; j++)
    //                        linearVertices.RemoveAt(i + 1);

    //                    linearVertices.Add(vNew);
    //                    return;
    //                }
    //                else
    //                    dPrevLen = dLen;
    //            }
    //        }
    //    }

    //    // vNew를 linearVertices에 시작점과 가까운 순으로 정렬하여 삽입한다.
    //    private void AddLinearVertex(List<Vertex2D> linearVertices, Vertex2D vNew)
    //    {
    //        int nVertexCount = linearVertices.Count;
    //        double dPrevLen = 0.0;

    //        for (int i = 0; i < nVertexCount; i++)
    //        {
    //            Vertex2D vertex = linearVertices[i];

    //            double dLen = vertex.GetDistance(vNew);

    //            if (IsSame(dLen))
    //                return;

    //            if (i == 0)
    //                dPrevLen = dLen;
    //            else
    //            {
    //                double dLen2 = vertex.GetDistance(linearVertices[i - 1]);

    //                if (dPrevLen < dLen2)
    //                {
    //                    linearVertices.Insert(i, vNew);
    //                    return;
    //                }
    //                else
    //                    dPrevLen = dLen;
    //            }
    //        }
    //    }

    //    private bool IsSameVertex(Vertex2D v1, Vertex2D v2)
    //    {
    //        double len = v1.GetDistance(v2);
    //        return IsSame(len);
    //    }

    //    private bool IsSame(double len)
    //    {
    //        return len < 0.1;
    //    }

    //    private Node FindNode(Vertex2D vPos)
    //    {
    //        foreach (Node node in m_nodes)
    //        {
    //            if (node.Position.GetDistance(vPos) < 0.1)
    //                return node;
    //        }

    //        return null;
    //    }

    //    private void AddLineNode(Line2D line)
    //    {
    //        Node begin = new Node(line.GetVertex(true));
    //        Node end = new Node(line.GetVertex(false));

    //        begin.LinkedNodes.Add(end);
    //        end.LinkedNodes.Add(begin);

    //        m_nodes.Add(begin);
    //        m_nodes.Add(end);
    //        m_dicLineVertex[line] = new KeyValuePair<Node, Node>(begin, end);
    //    }

    //    private void AddEArcNode(EArc2D earc)
    //    {
    //        EArcNodeGroup nodeGroup = new EArcNodeGroup(earc);
            
    //        m_nodes.Add(nodeGroup.BeginNode);
    //        m_nodes.Add(nodeGroup.EndNode);
    //        m_nodes.Add(nodeGroup.MiddleNode);
    //        m_dicEArcVertex[earc] = nodeGroup;
    //    }

    //    public List<Line2D> GetLines()
    //    {
    //        return m_dicLineVertex.Keys.ToList();
    //    }

    //    // 다른 노드와 연결되지 않은 노드들을 모두 없앤다.
    //    public void RemoveSingleNodes()
    //    {
    //        List<Node> singleNodes = new List<Node>();

    //        foreach (Node node in m_nodes)
    //        {
    //            if (node.LinkedNodes.Count <= 1)
    //                singleNodes.Add(node);
    //        }

    //        foreach (Node node in singleNodes)
    //        {
    //            m_nodes.Remove(node);
    //            RemoveSingleNode(node);
    //        }
    //    }

    //    private void RemoveSingleNode(Node node)
    //    {
    //        for (int i = 0; i < node.LinkedNodes.Count; i++)
    //        //foreach (Node link in node.LinkedNodes)
    //        {
    //            Node link = node.LinkedNodes[i];

    //            link.LinkedNodes.Remove(node);

    //            if (link.LinkedNodes.Count <= 1)
    //            {
    //                m_nodes.Remove(link);
    //                RemoveSingleNode(link);
    //            }
    //        }

    //        node.LinkedNodes.Clear();
    //    }
    //}
}
