using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;

namespace CadToXML.Geometry
{
    //public class PolygonBuilder
    //{
    //    private List<Line2D> m_lines = new List<Line2D>();
    //    private List<Arc2D> m_arcs = new List<Arc2D>();
    //    private List<EArc2D> m_earcs = new List<EArc2D>();

    //    /*public List<Line2D> Lines
    //    {
    //        get { return m_lines; }
    //    }*/

    //    public void AddLine(Vertex2D vBegin, Vertex2D vEnd)
    //    {
    //        Line2D line = new Line2D(vBegin, vEnd);
    //        m_lines.Add(line);
    //    }

    //    public void AddArc(Arc2D arc)
    //    {
    //        m_arcs.Add(arc);
    //    }

    //    public void AddEArc(EArc2D earc)
    //    {
    //        m_earcs.Add(earc);
    //    }

    //    // Polygon 생성 과정에서 기존에 입력된 Line들이 쪼개어지게 되는데
    //    // 최종적으로 쪼개어진 모든 Line들의 집합을 lines, arcs, earcs에 담는다.
    //    public List<Polygon> MakePolygon(out List<Line2D> lines, out List<Arc2D> arcs, out List<EArc2D> earcs)
    //    {
    //        lines = null;
    //        arcs = null;
    //        earcs = null;

    //        List<Polygon> polygons = new List<Polygon>();

    //        if (m_lines.Count <= 2 && m_arcs.Count == 0 && m_earcs.Count == 0)
    //            return polygons;

    //        VertexLink link = new VertexLink();

    //        foreach (Line2D line in m_lines)
    //        {
    //            link.AddLine(line);
    //        }

    //        /*foreach (Arc2D arc in m_arcs)
    //        {
    //            link.AddEArc(arc);
    //        }

    //        foreach (EArc2D earc in m_earcs)
    //        {
    //            link.AddEArc(earc);
    //        }*/

    //        lines = link.GetLines();
    //        link.RemoveSingleNodes();
    //        MakePolygon(link.Nodes, polygons);

    //        return polygons;
    //    }

    //    private void MakePolygon(List<VertexLink.Node> nodes, List<Polygon> polygons)
    //    {
    //        List<List<VertexLink.Node>> polygonNodesList = new List<List<VertexLink.Node>>();

    //        foreach (VertexLink.Node node in nodes)
    //        {
    //            MakePolygon(node, polygonNodesList);
    //        }

    //        foreach (List<VertexLink.Node> polygonNodes in polygonNodesList)
    //        {
    //            Polygon polygon = new Polygon();

    //            foreach (VertexLink.Node node in polygonNodes)
    //            {
    //                polygon.AddVertex(node.Position);
    //            }

    //            polygons.Add(polygon);
    //        }
    //    }

    //    private void MakePolygon(VertexLink.Node node, List<List<VertexLink.Node>> polygonNodesList)
    //    {
    //        int nLinkCount = node.LinkedNodes.Count;

    //        if (nLinkCount == 0)
    //            return;

    //        for (int i = 0; i < nLinkCount; i++)
    //        {
    //            VertexLink.Node link = node.LinkedNodes[i];
    //            List<VertexLink.Node> polygonNodes = MakePolygon(node, link);

    //            if (polygonNodes != null)
    //            {
    //                if (CheckDuplicate(polygonNodes, polygonNodesList))
    //                    polygonNodesList.Add(polygonNodes);
    //            }
    //        }
    //    }

    //    private bool CheckDuplicate(List<VertexLink.Node> polygonNodes, List<List<VertexLink.Node>> polygonNodesList)
    //    {
    //        foreach (List<VertexLink.Node> nodeList in polygonNodesList)
    //        {
    //            if (IsSamePolygonNodeList(polygonNodes, nodeList))
    //                return false;
    //        }

    //        return true;
    //    }

    //    private bool IsSamePolygonNodeList(List<VertexLink.Node> nodes1, List<VertexLink.Node> nodes2)
    //    {
    //        if (nodes1.Count != nodes2.Count)
    //            return false;

    //        int nBeginIndex = -1;
    //        VertexLink.Node firstNode = nodes1[0];

    //        for (int i = 0; i < nodes2.Count; i++)
    //        {
    //            if (firstNode == nodes2[i])
    //            {
    //                nBeginIndex = i;
    //                break;
    //            }
    //        }

    //        if (nBeginIndex < 0)
    //            return false;

    //        for (int i = 1, j = nBeginIndex + 1; i < nodes1.Count; i++, j++)
    //        {
    //            if (j >= nodes2.Count)
    //                j = 0;

    //            VertexLink.Node node1 = nodes1[i];
    //            VertexLink.Node node2 = nodes2[j];

    //            if (node1 != node2)
    //                return false;
    //        }

    //        return true;
    //    }

    //    private List<VertexLink.Node> MakePolygon(VertexLink.Node begin, VertexLink.Node next)
    //    {
    //        List<VertexLink.Node> nodes = new List<VertexLink.Node>();
    //        nodes.Add(begin);
    //        nodes.Add(next);

    //        VertexLink.Node prev = begin;
    //        VertexLink.Node node = next;

    //        double dTotalAngle = 0.0, dAngle = 0.0;
    //        next = GetNextNode(node, prev, out dAngle);

    //        while (next != null)
    //        {
    //            dTotalAngle += dAngle;

    //            if (next == begin)
    //            {
    //                // Polygon 회전방향이 반시계 방향이면 무시한다.
    //                if (nodes.Count < 3 || dTotalAngle < 0.0)
    //                    return null;
    //                else
    //                    return nodes;
    //            }
    //            else
    //            {
    //                /*int nIndex = nodes.IndexOf(next);

    //                if (nIndex <= 0)
    //                    nodes.Add(next);
    //                else
    //                {
    //                    // 이미 존재하는 노드의 연속(두개의 노드가 동일해야 하며 순서도 같을 경우)을 만날 경우 Polygond은 성립할 수 없다.
    //                    if (node == nodes[nIndex - 1])
    //                        return null;
    //                    else
    //                        nodes.Add(next);
    //                }*/

    //                // 이미 존재하는 노드를 만날 경우 Polygon은 성립할 수 없다.
    //                if (nodes.Contains(next))
    //                    return null;
    //                else
    //                    nodes.Add(next);
    //            }

    //            prev = node;
    //            node = next;
    //            next = GetNextNode(node, prev, out dAngle);
    //        }

    //        return null;
    //    }

    //    // node에 연결된 링크 가운데 prev와 node를 잇는 직선경로에서 가장 오른쪽에 있는 노드를 리턴한다.
    //    // dAngle : 라디안
    //    private VertexLink.Node GetNextNode(VertexLink.Node node, VertexLink.Node prev, out double dAngle)
    //    {
    //        dAngle = 0.0;
    //        double theta = 0.0;
    //        VertexLink.Node right = null;

    //        foreach (VertexLink.Node link in node.LinkedNodes)
    //        {
    //            if (link == prev)
    //                continue;

    //            if (UnE.Geometry.Math.IsRightSideFromLine(link.Position, node.Position, prev.Position) != 0)
    //            {
    //                theta = UnE.Geometry.Math.PI() - UnE.Geometry.Math.GetAngle(link.Position, node.Position, prev.Position);
    //            }
    //            else
    //            {
    //                theta = UnE.Geometry.Math.GetAngle(link.Position, node.Position, prev.Position) - UnE.Geometry.Math.PI();
    //            }

    //            if (right == null || theta > dAngle)
    //            {
    //                dAngle = theta;
    //                right = link;
    //            }
    //        }

    //        return right;
    //    }
    //}
}
