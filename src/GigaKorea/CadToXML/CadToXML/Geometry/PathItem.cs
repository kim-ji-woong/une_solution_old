using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;
using System.Collections;

namespace CadToXML
{
    public class PathItem
    {
        public enum DrawType { None = 0, Line, Arc, EArc };

        private Line2D m_line = null;
        private Arc2D m_arc = null;
        private EArc2D m_earc = null;

        // m_innerXXX : 교차점 계산에 의하여 잘려진 결과 선형
        // m_XXX : 원래 선형
        private Line2D m_innerLine = null;
        private Arc2D m_innerArc = null;
        private EArc2D m_innerEArc = null;
        // 교차점 계산결과 이 선형은 사용하지 않게될 경우 m_innerPass는 true가 된다.
        private bool m_innerPass = false;

        private DrawType m_drawType = DrawType.None;

        // PathItem은 Polygon의 일부분이 되는데, 이 객체가 Arc 또는 EArc 타입일 경우
        // 해당 곡선이 Polygon 안쪽을 향해 있으면 false, 바깥쪽을 향해 있으면 true를 리턴한다.
        private bool m_arcIsOutside = false;

        private Wall m_wall = null;

        public bool ArcIsOutside
        {
            get { return m_arcIsOutside; }
            set { m_arcIsOutside = value; }
        }

        public Wall Wall
        {
            get { return m_wall; }
            set { m_wall = value; }
        }

        public void SetLine(Line2D line, Vertex2D vBegin = null)
        {
            m_drawType = DrawType.Line;

            if (vBegin == null)
            {
                m_line = new Line2D(line.GetVertex(true), line.GetVertex(false));
            }
            else
            {
                Vertex2D v1 = line.GetVertex(true);
                Vertex2D v2 = line.GetVertex(false);

                double len1 = v1.GetDistance(vBegin);
                double len2 = v2.GetDistance(vBegin);

                if (len1 < len2)
                    m_line = new Line2D(v1, v2);
                else
                    m_line = new Line2D(v2, v1);
            }
        }

        public void SetArc(Arc2D arc, Vertex2D vBegin = null)
        {
            m_drawType = DrawType.Arc;

            if (vBegin == null)
            {
                m_arc = new Arc2D(arc.GetCenter(), arc.GetRadius(), arc.GetBeginAngle(), arc.GetAngle(), arc.IsClockWise());
            }
            else
            {
                Vertex2D v1 = arc.GetBeginVertex();
                Vertex2D v2 = arc.GetEndVertex();

                double len1 = v1.GetDistance(vBegin);
                double len2 = v2.GetDistance(vBegin);

                if (len1 < len2)
                    m_arc = new Arc2D(arc.GetCenter(), arc.GetRadius(), arc.GetBeginAngle(), arc.GetAngle(), arc.IsClockWise());
                else
                    m_arc = new Arc2D(arc.GetCenter(), arc.GetRadius(), arc.GetEndAngle(), arc.GetAngle(), !arc.IsClockWise());
            }
        }

        public void SetEArc(EArc2D earc, Vertex2D vBegin = null)
        {
            m_drawType = DrawType.EArc;

            if (vBegin == null)
            {
                m_earc = new EArc2D(earc.GetTL(), earc.GetBL(), earc.GetBR(), earc.GetBeginAngle(), earc.GetAngle(), earc.IsClockWise());
            }
            else
            {
                Vertex2D v1 = earc.GetBeginVertex();
                Vertex2D v2 = earc.GetEndVertex();

                double len1 = v1.GetDistance(vBegin);
                double len2 = v2.GetDistance(vBegin);

                if (len1 < len2)
                    m_earc = new EArc2D(earc.GetTL(), earc.GetBL(), earc.GetBR(), earc.GetBeginAngle(), earc.GetAngle(), earc.IsClockWise());
                else
                    m_earc = new EArc2D(earc.GetTL(), earc.GetBL(), earc.GetBR(), earc.GetEndAngle(), earc.GetAngle(), !earc.IsClockWise());
            }
        }

        public DrawType GetDrawType()
        {
            return m_drawType;
        }

        public bool GetVertex(out Vertex2D vBegin, out Vertex2D vEnd, out Vertex2D vMiddle)
        {
            vBegin = vEnd = vMiddle = null;

            if (m_drawType == DrawType.Line)
            {
                if (m_line != null)
                {
                    vMiddle = null;
                    vBegin = m_line.GetVertex(true);
                    vEnd = m_line.GetVertex(false);
                    return true;
                }
            }
            else if (m_drawType == DrawType.Arc || m_drawType == DrawType.EArc)
            {
                EArc2D earc = m_earc;

                if (m_drawType == DrawType.Arc)
                    earc = m_arc;

                if (earc != null)
                {
                    vBegin = earc.GetBeginVertex();
                    vEnd = earc.GetEndVertex();

                    if (earc.GetVertex(earc.GetBeginAngle() + earc.GetAngle() / 2, out vMiddle) == false)
                        return false;
                }

                if (UnE.Geometry.Math.IsRightSideFromLine(vMiddle, vBegin, vEnd) == 1)
                    m_arcIsOutside = true;
                else
                    m_arcIsOutside = false;
            }

            return true;
        }

        // offset 만큼 이동시킨 거리에 객체의 복사본을 만들어 리턴한다.
        // isClockwise : 전체 Polygon의 진행방향이 시계방향인가?
        public PathItem Offset(double offset, bool isClockwise)
        {
            PathItem item = null;

            if (m_drawType == DrawType.Line)
            {
                if (m_line != null)
                {
                    if (isClockwise == false)
                        offset = -offset;

                    Vertex2D vBegin = UnE.Geometry.Math.GetRightVertex(m_line.GetVertex(true), m_line.GetVertex(false), -offset);
                    Vertex2D vEnd = UnE.Geometry.Math.GetRightVertex(m_line.GetVertex(false), m_line.GetVertex(true), offset);

                    item = new PathItem();
                    item.SetLine(new Line2D(vBegin, vEnd));
                }
            }
            else if (m_drawType == DrawType.Arc)
            {
                if (m_arc != null)
                {
                    Arc2D arc = m_arc.Offset(!m_arcIsOutside, offset);

                    if (arc != null)
                    {
                        item = new PathItem();
                        item.SetArc(arc);
                        item.m_arcIsOutside = m_arcIsOutside;
                    }
                }
            }
            else if (m_drawType == DrawType.EArc)
            {
                if (m_earc != null)
                {
                    EArc2D earc = m_earc.Offset(!m_arcIsOutside, offset);

                    if (earc != null)
                    {
                        item = new PathItem();
                        item.SetEArc(earc);
                        item.m_arcIsOutside = m_arcIsOutside;
                    }
                }
            }

            if (item != null)
                item.m_wall = m_wall;

            return item;
        }

        // item1과 item2와의 교차점을 계산하여 그 결과를 item1과 item2에 각각 반영한다.
        // Return 값 : 1(계산 성공), 2(계산 성공하였으며, items 개수가 하나 증가함), 0(계산 실패)
        public static int CalcIntersection(PathItem item1, PathItem item2, List<PathItem> items, int nItem1Index)
        {
            int nIndex = nItem1Index;
            int nResult = 0;

            PathItem itemOrigin1 = item1;
            PathItem itemOrigin2 = item2;
            int nItem2Index = 0;

            while (item1 != null)
            {
                while (item1.m_innerPass)
                {
                    nIndex--;

                    if (nIndex < 0)
                        nIndex = items.Count - 1;

                    if (nIndex == nItem1Index)
                    {
                        System.Diagnostics.Trace.WriteLine("교차점을 찾을수 없음");
                        return 0;
                    }

                    item1 = items[nIndex];
                }

                if (item1.m_drawType == DrawType.Line)
                {
                    if (item2.m_drawType == DrawType.Line)
                        nResult = CalcIntersectionLineToLine(item1, item2);
                    else if (item2.m_drawType == DrawType.Arc || item2.m_drawType == DrawType.EArc)
                        nResult = CalcIntersectionLineToEArc(item1, item2);
                }
                else if (item1.m_drawType == DrawType.Arc || item1.m_drawType == DrawType.EArc)
                {
                    if (item2.m_drawType == DrawType.Line)
                        nResult = CalcIntersectionEArcToLine(item1, item2);
                    else if (item2.m_drawType == DrawType.Arc || item2.m_drawType == DrawType.EArc)
                        nResult = CalcIntersectionEArcToEArc(item1, item2);
                }

                if (nResult == 1)
                    break;
                else if (nResult == 0)
                    continue;
                else if (nResult == -1)
                {
                    if (item1.m_drawType == DrawType.Line && item2.m_drawType == DrawType.Line)
                    {
                        // 두 직선이 한점에서 만나면서 일직선을 이루어야 하는데, 벽체의 두께가 서로 달라서 평행하게 되어버린 경우
                        if (item1.m_innerLine == null)
                            item1.m_innerLine = new Line2D(item1.m_line);

                        int nIndex1 = items.IndexOf(item1);
                        int nIndex2 = items.IndexOf(item2);

                        if (nIndex1 < 0 || nIndex2 < 0)
                            return -1;

                        Vertex2D vTarget = null;

                        PathItem prevItem = nIndex1 == 0 ? items[items.Count - 1] : items[nIndex1 - 1];
                        Vertex2D vPrevBegin, vPrevEnd;

                        if (prevItem.GetInnerVertices(out vPrevBegin, out vPrevEnd))
                        {
                            if (item1.m_innerLine.IsInclude(vPrevBegin))
                                vTarget = vPrevBegin;
                            else if (item1.m_innerLine.IsInclude(vPrevEnd))
                                vTarget = vPrevEnd;
                            else
                                return -1;
                        }

                        // 두 벽체 사이에 임시 PathItem을 하나 끼워넣는다.
                        PathItem itemTemp = new PathItem();
                        itemTemp.SetLine(new Line2D(item1.m_line.GetVertex(false), item2.m_line.GetVertex(true)));
                        itemTemp.m_innerLine = new Line2D(itemTemp.m_line);
                        //items.Insert(items.Count - 1, itemTemp);

                        if (nIndex2 > nIndex1)
                            items.Insert(nIndex2, itemTemp);
                        else
                            items.Insert(nIndex1 + 1, itemTemp);

                        if (vTarget != null)
                        {
                            double dLen1 = itemTemp.m_line.GetDistance(item1.m_innerLine.GetVertex(true), false);
                            double dLen2 = itemTemp.m_line.GetDistance(item1.m_innerLine.GetVertex(false), false);

                            if (dLen1 < dLen2)
                                item1.m_innerLine.SetVertex(vTarget, false);
                            else
                                item1.m_innerLine.SetVertex(vTarget, true);
                        }

                        return 2;
                    }

                    System.Diagnostics.Trace.WriteLine("교차점을 찾을수 없음");
                    return 0;
                }
                else if (nResult == -2)
                {
                    if (itemOrigin2 == items[0])
                    {
                        do
                        {
                            nItem2Index++;

                            if (nItem2Index >= items.Count || items[nItem2Index] == itemOrigin1)
                            {
                                System.Diagnostics.Trace.WriteLine("교차점을 찾을수 없음");
                                return 0;
                            }

                            item2 = items[nItem2Index];
                        }
                        while (item2.m_innerPass == false);
                    }
                    else
                        break;
                }
            }

            return 1;
        }

        // Return 값 : 1(계산 성공)
        //             0(계산결과 item1은 사용하지 않게됨)
        //            -1(계산 실패)
        //            -2(계산결과 item2를 사용하지 않게됨)
        private static int CalcIntersectionEArcToEArc(PathItem item1, PathItem item2)
        {
            EArc2D earcItem1 = item1.m_earc;
            EArc2D earcItem2 = item2.m_earc;

            if (item1.m_drawType == DrawType.Arc)
            {
                if (item1.m_innerArc != null)
                    earcItem1 = item1.m_innerArc;
                else
                    earcItem1 = item1.m_arc;
            }
            else
            {
                if (item1.m_innerEArc != null)
                    earcItem1 = item1.m_innerEArc;
            }

            if (item2.m_drawType == DrawType.Arc)
            {
                if (item2.m_innerArc != null)
                    earcItem2 = item2.m_innerArc;
                else
                    earcItem2 = item2.m_arc;
            }
            else
            {
                if (item2.m_innerEArc != null)
                    earcItem2 = item2.m_innerEArc;
            }

            if (earcItem1 == null || earcItem2 == null)
                return -1;

            ArrayList arrVertices, arrEArcs;
            int nResult = earcItem1.IntersectEArc(earcItem2, out arrVertices, out arrEArcs);

            List<Vertex2D> vertices = new List<Vertex2D>();

            if (nResult == 0)
            {
                EArc2D earc1 = null, earc2 = null;

                if (item1.m_drawType == DrawType.Arc)
                    earc1 = new Arc2D(earcItem1.GetCenter(), ((Arc2D)earcItem1).GetRadius(), 0.0, UnE.Geometry.Math._2PI(), true);
                else if (item1.m_drawType == DrawType.EArc)
                    earc1 = new EArc2D(earcItem1.GetTL(), earcItem1.GetBL(), earcItem1.GetBR(), 0.0, UnE.Geometry.Math._2PI(), true);

                if (item2.m_drawType == DrawType.Arc)
                    earc2 = new Arc2D(earcItem2.GetCenter(), ((Arc2D)earcItem2).GetRadius(), 0.0, UnE.Geometry.Math._2PI(), true);
                else if (item2.m_drawType == DrawType.EArc)
                    earc2 = new EArc2D(earcItem2.GetTL(), earcItem2.GetBL(), earcItem2.GetBR(), 0.0, UnE.Geometry.Math._2PI(), true);

                nResult = earc1.IntersectEArc(earc2, out arrVertices, out arrEArcs);

                if (nResult == 0)
                {
                    item1.m_innerPass = true;
                    return 0;
                }
                else
                    AddEArcVertices(vertices, arrVertices, arrEArcs);
            }
            else
                AddEArcVertices(vertices, arrVertices, arrEArcs);

            Vertex2D vNear = GetNearVertex(earcItem1, vertices);

            if (vNear == null)
                return -1;

            EArc2D innerEArc1, innerEArc2;

            if (IsValidEArcVertex(earcItem1, vNear, true, out innerEArc1) == false)
            {
                item1.m_innerPass = true;
                return 0;
            }

            if (IsValidEArcVertex(earcItem2, vNear, false, out innerEArc2) == false)
            {
                item2.m_innerPass = true;
                return -2;
            }

            if (item1.m_drawType == DrawType.Arc)
            {
                item1.m_innerArc = (Arc2D)innerEArc1;
            }
            else if (item1.m_drawType == DrawType.EArc)
            {
                item1.m_innerEArc = innerEArc1;
            }

            if (item2.m_drawType == DrawType.Arc)
            {
                item2.m_innerArc = (Arc2D)innerEArc2;
            }
            else if (item2.m_drawType == DrawType.EArc)
            {
                item2.m_innerEArc = innerEArc2;
            }

            return 1;
        }

        private static void AddEArcVertices(List<Vertex2D> vertices, ArrayList arrVertices, ArrayList arrEArcs)
        {
            foreach (Vertex2D vertex in arrVertices)
            {
                vertices.Add(vertex);
            }

            foreach (EArc2D earc in arrEArcs)
            {
                vertices.Add(earc.GetBeginVertex());
                vertices.Add(earc.GetEndVertex());
            }
        }

        // Return 값 : 1(계산 성공)
        //             0(계산결과 item1은 사용하지 않게됨)
        //            -1(계산 실패)
        //            -2(계산결과 item2를 사용하지 않게됨)
        private static int CalcIntersectionEArcToLine(PathItem item1, PathItem item2)
        {
            EArc2D earc = item1.m_earc;

            if (item1.m_drawType == DrawType.Arc)
            {
                if (item1.m_innerArc != null)
                    earc = item1.m_innerArc;
                else
                    earc = item1.m_arc;
            }
            else
            {
                if (item1.m_innerEArc != null)
                    earc = item1.m_innerEArc;
            }

            if (earc == null)
                return -1;

            Line2D line = item2.m_line;

            if (item2.m_innerLine != null)
                line = item2.m_innerLine;

            if (line == null)
                return -1;

            Vertex2D v1, v2;
            int nResult = earc.IntersectLine(line, out v1, out v2);

            if (nResult == 2)
            {
                v1 = GetNearVertex(earc, v1, v2);
            }
            else if (nResult == 0)
            {
                // 두 직선이 만나지 않을 경우 직선과 타원을 연장시킨다.
                Line2D line2 = new Line2D(line.GetVertex(true), line.GetVertex(false), Line2D.LineType.HALF_LINE_END_2_BEGIN);
                EArc2D earc1 = null;

                if (item1.m_drawType == DrawType.Arc)
                    earc1 = new Arc2D(earc.GetCenter(), ((Arc2D)earc).GetRadius(), 0.0, UnE.Geometry.Math._2PI(), true);
                else if (item1.m_drawType == DrawType.EArc)
                    earc1 = new EArc2D(earc.GetTL(), earc.GetBL(), earc.GetBR(), 0.0, UnE.Geometry.Math._2PI(), true);

                nResult = line2.IntersectEArc(earc1, out v1, out v2);

                if (nResult == 0)
                {
                    item1.m_innerPass = true;
                    return 0;
                }
                else if (nResult == 2)
                {
                    double len1 = line.GetVertex(false).GetDistance(v1);
                    double len2 = line.GetVertex(false).GetDistance(v2);

                    if (len2 < len1)
                        v1 = v2;
                }
            }

            EArc2D innerEArc;

            if (IsValidEArcVertex(earc, v1, true, out innerEArc) == false)
            {
                item1.m_innerPass = true;
                return 0;
            }

            if (item1.m_drawType == DrawType.Arc)
            {
                item1.m_innerArc = (Arc2D)innerEArc;
            }
            else if (item1.m_drawType == DrawType.EArc)
            {
                item1.m_innerEArc = innerEArc;
            }

            item2.m_innerLine = new Line2D(v1, line.GetVertex(false));
            return 1;
        }

        // earc 위의 두점 v1과 v2가 있다.
        // 이 가운데 earc의 시작점과 더 가까운 점을 찾아 리턴한다.
        private static Vertex2D GetNearVertex(EArc2D earc, Vertex2D v1, Vertex2D v2)
        {
            double dAngle1 = GetEArcAngle(earc, v1);
            double dAngle2 = GetEArcAngle(earc, v2);
            double dBeginAngle = GetEArcAngle(earc, earc.GetBeginVertex());

            double dEArcAngle1 = GetEArcAngle(dBeginAngle, dAngle1, earc.IsClockWise());
            double dEArcAngle2 = GetEArcAngle(dBeginAngle, dAngle2, earc.IsClockWise());
            return dEArcAngle1 < dEArcAngle2 ? v1 : v2;
        }

        // vertices 요소 가운데 earc의 시작점과 가장 가까운 점을 찾아 리턴한다.
        // 이 가운데 earc의 시작점과 더 가까운 점을 찾아 리턴한다.
        private static Vertex2D GetNearVertex(EArc2D earc, List<Vertex2D> vertices)
        {
            double dMinAngle = -1.0;
            Vertex2D vNear = null;

            double dBeginAngle = GetEArcAngle(earc, earc.GetBeginVertex());

            foreach (Vertex2D vertex in vertices)
            {
                double dAngle = GetEArcAngle(earc, vertex);
                double dEArcAngle = GetEArcAngle(dBeginAngle, dAngle, earc.IsClockWise());

                if (vNear == null || dEArcAngle < dMinAngle)
                {
                    vNear = vertex;
                    dMinAngle = dEArcAngle;
                }
            }

            return vNear;
        }

        private static double GetEArcAngle(double dBeginAngle, double dEndAngle, bool isClockwise)
        {
            if (isClockwise)
            {
                if (dEndAngle < dBeginAngle)
                    return dBeginAngle - dEndAngle;
                else
                    return UnE.Geometry.Math._2PI() - (dEndAngle - dBeginAngle);
            }
            else
            {
                if (dEndAngle > dBeginAngle)
                    return dEndAngle - dBeginAngle;
                else
                    return UnE.Geometry.Math._2PI() - (dBeginAngle - dEndAngle);
            }
        }

        // Return 값 : 1(계산 성공)
        //             0(계산결과 item1은 사용하지 않게됨)
        //            -1(계산 실패)
        //            -2(계산결과 item2를 사용하지 않게됨)
        private static int CalcIntersectionLineToEArc(PathItem item1, PathItem item2)
        {
            EArc2D earc = item2.m_earc;

            if (item2.m_drawType == DrawType.Arc)
            {
                if (item2.m_innerArc != null)
                    earc = item2.m_innerArc;
                else
                    earc = item2.m_arc;
            }
            else
            {
                if (item2.m_innerEArc != null)
                    earc = item2.m_innerEArc;
            }

            if (earc == null)
                return -1;

            Line2D line = item1.m_line;

            if (item1.m_innerLine != null)
                line = item1.m_innerLine;

            Vertex2D v1, v2;
            int nResult = line.IntersectEArc(earc, out v1, out v2);

            if (nResult == 2)
            {
                double len1 = line.GetVertex(true).GetDistance(v1);
                double len2 = line.GetVertex(true).GetDistance(v2);

                if (len2 < len1)
                    v1 = v2;
            }
            else if (nResult == 0)
            {
                // 두 직선이 만나지 않을 경우 직선과 타원을 연장시킨다.
                Line2D line1 = new Line2D(line.GetVertex(true), line.GetVertex(false), Line2D.LineType.HALF_LINE_BEGIN_2_END);
                EArc2D earc2 = null;

                if (item2.m_drawType == DrawType.Arc)
                    earc2 = new Arc2D(earc.GetCenter(), ((Arc2D)earc).GetRadius(), 0.0, UnE.Geometry.Math._2PI(), true);
                else if (item2.m_drawType == DrawType.EArc)
                    earc2 = new EArc2D(earc.GetTL(), earc.GetBL(), earc.GetBR(), 0.0, UnE.Geometry.Math._2PI(), true);

                nResult = line1.IntersectEArc(earc2, out v1, out v2);

                if (nResult == 0)
                {
                    item1.m_innerPass = true;
                    return 0;
                }
                else if (nResult == 2)
                {
                    double len1 = line.GetVertex(true).GetDistance(v1);
                    double len2 = line.GetVertex(true).GetDistance(v2);

                    if (len2 < len1)
                        v1 = v2;
                }
            }

            EArc2D innerEArc;

            if (IsValidEArcVertex(earc, v1, false, out innerEArc) == false)
            {
                item2.m_innerPass = true;
                return -2;
            }

            if (item1.m_innerLine == null)
            {
                item1.m_innerLine = new Line2D(line.GetVertex(true), v1);
            }
            else
            {
                double len1 = line.GetVertex(true).GetDistance(item1.m_innerLine.GetVertex(false));
                double len2 = line.GetVertex(true).GetDistance(v1);

                if (len1 < len2)
                {
                    item1.m_innerLine.SetVertex(item1.m_innerLine.GetVertex(false), true);
                    item1.m_innerLine.SetVertex(v1, false);
                }
                else
                {
                    item1.m_innerPass = true;
                    return 0;
                }
            }

            if (item2.m_drawType == DrawType.Arc)
                item2.m_innerArc = (Arc2D)innerEArc;
            else if (item2.m_drawType == DrawType.EArc)
                item2.m_innerEArc = innerEArc;

            return 1;
        }

        private static bool IsValidEArcVertex(EArc2D earc, Vertex2D vertex, bool inverse, out EArc2D result)
        {
            result = null;

            double dAngle = GetEArcAngle(earc, vertex);
            Vertex2D vBegin = earc.GetBeginVertex();
            Vertex2D vEnd = earc.GetEndVertex();

            // vertex가 earc내에 속해 있는가?
            if (earc.CheckValidAngle(dAngle))
            {
                if (inverse)
                {
                    if (vertex.GetDistance(vBegin) <= 0.1)
                        return false;
                }
                else
                {
                    if (vertex.GetDistance(vEnd) <= 0.1)
                        return false;
                }

                double dBeginAngle = GetEArcAngle(earc, vBegin);
                double dEndAngle = GetEArcAngle(earc, vEnd);
                double dEArcAngle = 0.0;

                if (inverse)
                {
                    dEArcAngle = GetEArcAngle(dBeginAngle, dAngle, earc.IsClockWise());
                }
                else
                {
                    dEArcAngle = GetEArcAngle(dEndAngle, dAngle, earc.IsClockWise());
                    /*if (dAngle > dBeginAngle)
                    {
                        if (earc.IsClockWise())
                            dEArcAngle = dAngle - dEndAngle;
                        else
                        {
                            if (dEndAngle > dAngle)
                                dEArcAngle = dEndAngle - dAngle;
                            else
                                dEArcAngle = UnE.Geometry.Math._2PI() - (dAngle - dEndAngle);
                        }
                    }
                    else
                    {
                        if (earc.IsClockWise())
                        {
                            if (dEndAngle < dAngle)
                                dEArcAngle = dAngle - dEndAngle;
                            else
                                dEArcAngle = UnE.Geometry.Math._2PI() - (dEndAngle - dAngle);
                        }
                        else
                            dEArcAngle = dEndAngle - dAngle;
                    }*/
                }

                if (inverse)
                {
                    if (earc is Arc2D)
                        result = new Arc2D(earc.GetCenter(), ((Arc2D)earc).GetRadius(), dBeginAngle, dEArcAngle, earc.IsClockWise());
                    else
                        result = new EArc2D(earc.GetTL(), earc.GetBL(), earc.GetBR(), dBeginAngle, dEArcAngle, earc.IsClockWise());
                }
                else
                {
                    if (earc is Arc2D)
                        result = new Arc2D(earc.GetCenter(), ((Arc2D)earc).GetRadius(), dAngle, dEArcAngle, earc.IsClockWise());
                    else
                        result = new EArc2D(earc.GetTL(), earc.GetBL(), earc.GetBR(), dAngle, dEArcAngle, earc.IsClockWise());
                }

                return true;
            }

            if (inverse)
            {
                // vertex가 earc의 연장선에 있다면 earc의 시작점 보다는 끝점에 더 가까워야 한다.
                return vBegin.GetDistance(vertex) > vEnd.GetDistance(vertex);
            }
            //else
            // vertex가 earc의 연장선에 있다면 earc의 끝점 보다는 시작점에 더 가까워야 한다.
            return vBegin.GetDistance(vertex) < vEnd.GetDistance(vertex);
        }

        public static double GetEArcAngle(EArc2D earc, Vertex2D vertex)
        {
            Vertex2D vCenter = earc.GetCenter();
            Vertex2D vRight = new Vertex2D(vCenter.x + earc.GetBR().GetDistance(earc.GetBL()), vCenter.y);

            double dAngle = 0.0;

            if (vertex.y < vCenter.y)
                dAngle = UnE.Geometry.Math._2PI() - UnE.Geometry.Math.GetAngle(vertex, vCenter, vRight);
            else
                dAngle = UnE.Geometry.Math.GetAngle(vertex, vCenter, vRight);

            return dAngle;
        }

        // Return 값 : 1(계산 성공)
        //             0(계산결과 item1은 사용하지 않게됨)
        //            -1(계산 실패)
        private static int CalcIntersectionLineToLine(PathItem item1, PathItem item2)
        {
            Line2D itemLine1 = item1.m_line;
            Line2D itemLine2 = item2.m_line;

            if (item1.m_innerLine != null)
                itemLine1 = item1.m_innerLine;

            if (item2.m_innerLine != null)
                itemLine2 = item2.m_innerLine;

            if (itemLine1 == null || itemLine2 == null)
                return -1;

            Vertex2D v1, v2;
            Line2D.LineType lineType;
            int nResult = itemLine1.IntersectLine(itemLine2, out v1, out v2, out lineType);

            if (nResult == 2)
            {
                System.Diagnostics.Trace.WriteLine("Error");
                return -1;
            }
            else if (nResult == 0)
            {
                // 두 직선이 만나지 않을 경우 각각의 직선을 연장시켜 만나는 점을 찾는다.
                Line2D line1 = new Line2D(itemLine1.GetVertex(true), itemLine1.GetVertex(false), Line2D.LineType.HALF_LINE_BEGIN_2_END);
                Line2D line2 = new Line2D(itemLine2.GetVertex(true), itemLine2.GetVertex(false), Line2D.LineType.HALF_LINE_END_2_BEGIN);

                nResult = line1.IntersectLine(line2, out v1, out v2, out lineType);

                if (nResult == 0)
                {
                    System.Diagnostics.Trace.WriteLine("Error");
                    return -1;
                }
            }

            if (item1.m_innerLine == null)
            {
                item1.m_innerLine = new Line2D(itemLine1.GetVertex(true), v1);
            }
            else
            {
                item1.m_innerLine.SetVertex(v1, false);
                /*double len1 = itemLine1.GetVertex(true).GetDistance(item1.m_innerLine.GetVertex(false));
                double len2 = itemLine1.GetVertex(true).GetDistance(v1);

                if (len1 < len2)
                {
                    item1.m_innerLine.SetVertex(item1.m_innerLine.GetVertex(false), true);
                    item1.m_innerLine.SetVertex(v1, false);
                }
                else
                {
                    item1.m_innerPass = true;
                    return 0;
                }*/
            }

            item2.m_innerLine = new Line2D(v1, itemLine2.GetVertex(false));
            return 1;
        }

        public void InnerToCenter()
        {
            if (m_drawType == DrawType.Line)
            {
                m_line = m_innerLine;
                m_innerLine = null;
            }
            else if (m_drawType == DrawType.Arc)
            {
                m_arc = m_innerArc;
                m_innerArc = null;
            }
            else if (m_drawType == DrawType.EArc)
            {
                m_earc = m_innerEArc;
                m_innerEArc = null;
            }
        }

        public EArc2D GetEArc()
        {
            if (m_drawType == DrawType.Arc)
                return m_arc;
            else if (m_drawType == DrawType.EArc)
                return m_earc;

            return null;
        }

        public void CheckBoundary(double x, double y, ref Vertex2D vTL, ref Vertex2D vBR)
        {
            if (m_drawType == DrawType.Line)
            {
                Vertex2D vBegin = m_line.GetVertex(true);
                Vertex2D vEnd = m_line.GetVertex(false);

                SetBoundary(vBegin.x + x, vBegin.y + y, ref vTL, ref vBR);
                SetBoundary(vBegin.x + x, vBegin.y + y, ref vTL, ref vBR);
            }
            else if (m_drawType == DrawType.Arc || m_drawType == DrawType.EArc)
            {
                EArc2D arc = m_drawType == DrawType.Arc ? m_arc : m_earc;

                Vertex2D _vTL = m_arc.GetTL();
                Vertex2D _vBL = m_arc.GetBL();
                Vertex2D _vBR = m_arc.GetBR();

                SetBoundary(_vTL.x + x, _vTL.y + y, ref vTL, ref vBR);
                SetBoundary(_vBL.x + x, _vBL.y + y, ref vTL, ref vBR);
                SetBoundary(_vBR.x + x, _vBR.y + y, ref vTL, ref vBR);
            }
        }

        private void SetBoundary(double x, double y, ref Vertex2D vTL, ref Vertex2D vBR)
        {
            if (vTL == null)
            {
                vTL = new Vertex2D(x, y);
                vBR = new Vertex2D(x, y);
            }
            else
            {
                if (vTL.x > x)
                    vTL.x = x;
                if (vTL.y < y)
                    vTL.y = y;
                if (vBR.x < x)
                    vBR.x = x;
                if (vBR.y > y)
                    vBR.y = y;
            }
        }

        private bool GetInnerVertices(out Vertex2D vBegin, out Vertex2D vEnd)
        {
            vBegin = vEnd = null;
            Vertex2D v1 = null, v2 = null;

            if (m_innerLine != null)
            {
                v1 = m_innerLine.GetVertex(true);
                v2 = m_innerLine.GetVertex(false);
            }
            else if (m_innerArc != null)
            {
                v1 = m_innerArc.GetBeginVertex();
                v2 = m_innerArc.GetEndVertex();
            }
            else if (m_innerEArc != null)
            {
                v1 = m_innerEArc.GetBeginVertex();
                v2 = m_innerEArc.GetEndVertex();
            }
            else
                return false;

            vBegin = new Vertex2D(v1.x, v1.y);
            vEnd = new Vertex2D(v2.x, v2.y);
            return true;
        }
    }
}
