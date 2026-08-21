using System;
using System.Collections.Generic;
using System.Text;
using UnE.Geometry;

namespace XMLWebServiceManager.Shapes
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

        private BIM.Wall m_wall = null;

        public bool ArcIsOutside
        {
            get { return m_arcIsOutside; }
            set { m_arcIsOutside = value; }
        }

        public BIM.Wall Wall
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


        public EArc2D GetEArc()
        {
            if (m_drawType == DrawType.Arc)
                return m_arc;
            else if (m_drawType == DrawType.EArc)
                return m_earc;

            return null;
        }

    }
}
