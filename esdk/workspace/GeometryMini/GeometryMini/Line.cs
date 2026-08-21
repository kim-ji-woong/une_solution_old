using System;
using System.Collections.Generic;
using System.Text;

namespace UnE.Geomini
{
    public class Line2D
    {
        // HALF_LINE_BEGIN_2_END : 시작점에서 끝점 방향으로 끝없이 이어진 반직선
        // HALF_LINE_END_2_BEGIN : 끝점에서 시작점 방향으로 끝없이 이어진 반직선
        public enum LineType { LINE = 0, HALF_LINE_BEGIN_2_END, HALF_LINE_END_2_BEGIN, SEGMENT, NO_LINE };

        private LineType m_lineType = LineType.SEGMENT;
        private Vertex2D m_vBegin = new Vertex2D();
        private Vertex2D m_vEnd = new Vertex2D();

        public Line2D()
        {
        }

        public Line2D(LineType type)
        {
            SetLineType(type);
        }

        public Line2D(Line2D rhs)
        {
            SetLineType(rhs.m_lineType);
            SetVertex(rhs.m_vBegin, true);
            SetVertex(rhs.m_vEnd, false);
        }

        public Line2D(Vertex2D vBegin, Vertex2D vEnd)
        {
            SetVertex(vBegin, true);
            SetVertex(vEnd, false);
        }

        public Line2D(Vertex2D vBegin, Vertex2D vEnd, LineType type)
        {
            SetLineType(type);
            SetVertex(vBegin, true);
            SetVertex(vEnd, false);
        }

        /*public static bool operator ==(Line2D line1, Line2D line2)
        {
            bool isNull1 = NullChecker.IsNull(line1);
            bool isNull2 = NullChecker.IsNull(line2);

            if (isNull1 && isNull2)
                return true;
            else if (isNull1 || isNull2)
                return false;

            if (line1.GetLineType() != line2.GetLineType())
                return false;
            else if (line1.GetVertex(true) != line2.GetVertex(true))
                return false;
            else if (line1.GetVertex(false) != line2.GetVertex(false))
                return false;

            return true;
        }

        public static bool operator !=(Line2D line1, Line2D line2)
        {
            return !(line1 == line2);
        }*/

        public LineType GetLineType()
        {
            return m_lineType;
        }

        public void SetLineType(LineType type)
        {
            m_lineType = type;
        }

        public void SetVertex(Vertex2D vertex, bool isBegin)
        {
            if (isBegin)
                m_vBegin.CopyFrom(vertex);
            else
                m_vEnd.CopyFrom(vertex);
        }

        // rVertex가 Line내에 포함되어 있는지 알려준다.
        public bool IsInclude(Vertex2D vertex)
        {
            double dLen = this.GetDistance(vertex, false);
            if (dLen <= Math.HALF_TOLERANCE())
                return true;

            return false;
        }

		// noLimit이 true이면 실제 LineType에 상관없이 무한히 뻗은 직선이라 가정하고 계산한다.
		// noLimit이 false이면 실제 LineType을 고려하여 가장 가까운 거리를 구한다.
		public double GetDistance(Vertex2D vertex, bool noLimit)
        {
            Vertex2D vBegin = this.GetVertex(true);
            Vertex2D vEnd = this.GetVertex(false);

            double a = vertex.GetDistance(vBegin);
            double b = vBegin.GetDistance(vEnd);
            double c = vertex.GetDistance(vEnd);

            if (a <= Math.COORD_TOLERANCE() || c <= Math.COORD_TOLERANCE())
                return 0.0;
            if (b <= Math.COORD_TOLERANCE())
                return a;

            double dCos = (a * a + b * b - c * c) / 2 / a / b;
            Vertex2D vertex2 = Math.GetLinearVertex(vBegin, vEnd, dCos * a);
            double dLen = vertex2.GetDistance(vertex);

            LineType type = this.GetLineType();

            if (noLimit || type == LineType.LINE)
                return dLen;

            double dAngle1 = Math.GetAngle(vertex, vBegin, vEnd);
            double dAngle2 = Math.GetAngle(vertex, vEnd, vBegin);

            if (dAngle1 <= Math.HALF_PI() && dAngle2 <= Math.HALF_PI())
                return dLen;

            if (type == LineType.HALF_LINE_BEGIN_2_END)
            {
                if (dAngle1 < Math.HALF_PI())
                    return dLen;
            }
            else if (type == LineType.HALF_LINE_END_2_BEGIN)
            {
                if (dAngle2 < Math.HALF_PI())
                    return dLen;
            }

            return a > c ? c : a;
        }
			
		public Vertex2D GetVertex(bool isBegin)
        {
            return isBegin? m_vBegin : m_vEnd;
        }

        // rLine과 만나지 않으면 0을 리턴한다.
        // 교차점이 하나만 존재할 경우 v1에만 값이 담겨지며 1이 리턴된다.
        // 교차점이 두 개 존재할 경우 v1과 v2에 각각 값이 담겨진다.
        // 교차점이 두 개인 경우는 직선에 해당하기 때문에 resultType을 읽어 어떠한 형태의 직선인지 알아낼 수 있다.
        public int IntersectLine(Line2D line, ref Vertex2D v1, ref Vertex2D v2, ref LineType resultType)
        {
            v1 = new Vertex2D();
            v2 = new Vertex2D();
            resultType = LineType.NO_LINE;

            Vertex2D vBegin1 = this.GetVertex(true);
            Vertex2D vEnd1 = this.GetVertex(false);
            Vertex2D vBegin2 = line.GetVertex(true);
            Vertex2D vEnd2 = line.GetVertex(false);

            double dLen1 = vBegin1.GetDistance(vEnd1);
            double dLen2 = vBegin2.GetDistance(vEnd2);

            // this Line이 한 점일 경우
            if (dLen1 < Math.HALF_TOLERANCE())
            {
                if (dLen2 < Math.HALF_TOLERANCE())
                {
                    if (vBegin1.GetDistance(vBegin2) < Math.HALF_TOLERANCE())
                    {
                        v1.SetVertex(vBegin1.x, vBegin1.y);
                        return 1;
                    }
                }
                else
                {
                    double dLen3 = vBegin1.GetDistance(vBegin2);
                    double dLen4 = vBegin1.GetDistance(vEnd2);

                    if (dLen3 < Math.HALF_TOLERANCE() ||
                        dLen4 < Math.HALF_TOLERANCE())
                    {
                        v1.SetVertex(vBegin1.x, vBegin1.y);
                        return 1;
                    }

                    double dAngle = Math.GetAngle(vBegin2, vBegin1, vEnd2);

                    if (line.m_lineType == LineType.LINE)
                    {
                        if (dAngle < Math.HALF_TOLERANCE() ||
                            Math.PI() - dAngle < Math.HALF_TOLERANCE())
                        {
                            v1.SetVertex(vBegin1.x, vBegin1.y);
                            return 1;
                        }
                    }
                    else if (line.m_lineType == LineType.HALF_LINE_BEGIN_2_END)
                    {
                        if (Math.PI() - dAngle < Math.HALF_TOLERANCE())
                        {
                            v1.SetVertex(vBegin1.x, vBegin1.y);
                            return 1;
                        }
                        else if (dAngle < Math.HALF_TOLERANCE())
                        {
                            if (dLen4 < dLen3)
                            {
                                v1.SetVertex(vBegin1.x, vBegin1.y);
                                return 1;
                            }
                        }
                    }
                    else if (line.m_lineType == LineType.HALF_LINE_END_2_BEGIN)
                    {
                        if (Math.PI() - dAngle < Math.HALF_TOLERANCE())
                        {
                            v1.SetVertex(vBegin1.x, vBegin1.y);
                            return 1;
                        }
                        else if (dAngle < Math.HALF_TOLERANCE())
                        {
                            if (dLen3 < dLen4)
                            {
                                v1.SetVertex(vBegin1.x, vBegin1.y);
                                return 1;
                            }
                        }
                    }
                    else if (line.m_lineType == LineType.SEGMENT)
                    {
                        if (Math.PI() - dAngle < Math.HALF_TOLERANCE())
                        {
                            v1.SetVertex(vBegin1.x, vBegin1.y);
                            return 1;
                        }
                    }
                }

                return 0;
            }
            // rLine이 한 점일 경우
            else if (dLen2 < Math.HALF_TOLERANCE())
            {
                double dLen3 = vBegin2.GetDistance(vBegin1);
                double dLen4 = vBegin2.GetDistance(vEnd1);

                if (dLen3 < Math.HALF_TOLERANCE() ||
                    dLen4 < Math.HALF_TOLERANCE())
                {
                    v1.SetVertex(vBegin2.x, vBegin2.y);
                    return 1;
                }

                double dAngle = Math.GetAngle(vBegin1, vBegin2, vEnd1);

                if (this.m_lineType == LineType.LINE)
                {
                    if (dAngle < Math.HALF_TOLERANCE() ||
                        Math.PI() - dAngle < Math.HALF_TOLERANCE())
                    {
                        v1.SetVertex(vBegin2.x, vBegin2.y);
                        return 1;
                    }
                }
                else if (this.m_lineType == LineType.HALF_LINE_BEGIN_2_END)
                {
                    if (Math.PI() - dAngle < Math.HALF_TOLERANCE())
                    {
                        v1.SetVertex(vBegin2.x, vBegin2.y);
                        return 1;
                    }
                    else if (dAngle < Math.HALF_TOLERANCE())
                    {
                        if (dLen4 < dLen3)
                        {
                            v1.SetVertex(vBegin2.x, vBegin2.y);
                            return 1;
                        }
                    }
                }
                else if (this.m_lineType == LineType.HALF_LINE_END_2_BEGIN)
                {
                    if (Math.PI() - dAngle < Math.HALF_TOLERANCE())
                    {
                        v1.SetVertex(vBegin2.x, vBegin2.y);
                        return 1;
                    }
                    else if (dAngle < Math.HALF_TOLERANCE())
                    {
                        if (dLen3 < dLen4)
                        {
                            v1.SetVertex(vBegin2.x, vBegin2.y);
                            return 1;
                        }
                    }
                }
                else if (this.m_lineType == LineType.SEGMENT)
                {
                    if (Math.PI() - dAngle < Math.HALF_TOLERANCE())
                    {
                        v1.SetVertex(vBegin2.x, vBegin2.y);
                        return 1;
                    }
                }

                return 0;
            }

            // this Line의 방정식 y = (a1)x + b1
            // line의 방정식 y = (a2)x + b2
            // x = constant 형태의 직선일 경우
            // this Line의 x값 : c1
            // line의 x값 : c2
            double a1 = 0.0, a2 = 0.0, b1 = 0.0, b2 = 0.0, c1 = 0.0, c2 = 0.0;

            // x = const 형태의 방정식인가?
            bool bXEq1 = GetLineEquation(this, out a1, out b1, out c1);
            bool bXEq2 = GetLineEquation(this, out a2, out b2, out c2);

            if (bXEq1 && bXEq2)
            {
                if (System.Math.Abs(c1 - c2) > Math.HALF_TOLERANCE())
                    return 0;

                if (this.m_lineType == LineType.LINE)
                {
                    if (line.m_lineType == LineType.LINE)
                    {
                        v1 = vBegin1;
                        v2 = vEnd1;
                        resultType = LineType.LINE;
                        return 2;
                    }
                    else
                    {
                        v1 = vBegin2;
                        v2 = vEnd2;
                        resultType = line.m_lineType;
                        return 2;
                    }
                }
                else if (this.m_lineType == LineType.HALF_LINE_BEGIN_2_END || this.m_lineType == LineType.HALF_LINE_END_2_BEGIN)
                {
                    if (line.m_lineType == LineType.LINE)
                    {
                        v1 = vBegin1;
                        v2 = vEnd1;
                        resultType = this.m_lineType;
                        return 2;
                    }
                    else if (line.m_lineType == LineType.HALF_LINE_BEGIN_2_END || line.m_lineType == LineType.HALF_LINE_END_2_BEGIN)
                    {
                        return GetHalfLineResult(this, line, ref v1, ref v2, ref resultType);
                    }
                    else// if (line.m_lineType == LineType.SEGMENT)
                    {
                        return GetHalfLineSegmentResult(this, line, ref v1, ref v2, ref resultType);
                    }
                }
                else// if (this.m_lineType == LineType.SEGMENT)
                {
                    if (line.m_lineType == LineType.LINE)
                    {
                        v1 = vBegin1;
                        v2 = vEnd1;
                        resultType = this.m_lineType;
                        return 2;
                    }
                    else if (line.m_lineType == LineType.HALF_LINE_BEGIN_2_END || line.m_lineType == LineType.HALF_LINE_END_2_BEGIN)
                    {
                        return GetHalfLineSegmentResult(line, this, ref v1, ref v2, ref resultType);
                    }
                    else// if (line.m_lineType == LineType.SEGMENT)
                    {
                        return GetSegmentResult(this, line, ref v1, ref v2, ref resultType);
                    }
                }
            }
            else if (bXEq1)
            {
                v1.x = c1;
                v1.y = a2 * v1.x + b2;
            }
            else if (bXEq2)
            {
                v1.x = c2;
                v1.y = a1 * v1.x + b1;
            }
            else
            {
                // 기울기가 같을 경우
                if (System.Math.Abs(a1 - a2) <= Math.HALF_TOLERANCE())
                {
                    if (System.Math.Abs(b1 - b2) > Math.HALF_TOLERANCE())
                        return 0;

                    if (this.m_lineType == LineType.LINE)
                    {
                        if (line.m_lineType == LineType.LINE)
                        {
                            v1 = vBegin1;
                            v2 = vEnd1;
                            resultType = LineType.LINE;
                            return 2;
                        }
                        else
                        {
                            v1 = vBegin2;
                            v2 = vEnd2;
                            resultType = line.m_lineType;
                            return 2;
                        }
                    }
                    else if (this.m_lineType == LineType.HALF_LINE_BEGIN_2_END || this.m_lineType == LineType.HALF_LINE_END_2_BEGIN)
                    {
                        if (line.m_lineType == LineType.LINE)
                        {
                            v1 = vBegin1;
                            v2 = vEnd1;
                            resultType = this.m_lineType;
                            return 2;
                        }
                        else if (line.m_lineType == LineType.HALF_LINE_BEGIN_2_END || line.m_lineType == LineType.HALF_LINE_END_2_BEGIN)
                        {
                            return GetHalfLineResult(this, line, ref v1, ref v2, ref resultType);
                        }
                        else// if (line.m_lineType == LineType.SEGMENT)
                        {
                            return GetHalfLineSegmentResult(this, line, ref v1, ref v2, ref resultType);
                        }
                    }
                    else// if (this.m_lineType == LineType.SEGMENT)
                    {
                        if (line.m_lineType == LineType.LINE)
                        {
                            v1 = vBegin1;
                            v2 = vEnd1;
                            resultType = this.m_lineType;
                            return 2;
                        }
                        else if (line.m_lineType == LineType.HALF_LINE_BEGIN_2_END || line.m_lineType == LineType.HALF_LINE_END_2_BEGIN)
                        {
                            return GetHalfLineSegmentResult(line, this, ref v1, ref v2, ref resultType);
                        }
                        else// if (line.m_lineType == LineType.SEGMENT)
                        {
                            return GetSegmentResult(this, line, ref v1, ref v2, ref resultType);
                        }
                    }
                }
                else
                {
                    v1.x = (b2 - b1) / (a1 - a2);
                    v1.y = a1 * v1.x + b1;
                }
            }

            if (this.IsInclude(v1) && line.IsInclude(v1))
            {
                resultType = LineType.NO_LINE;
                return 1;
            }

            return 0;
        }

        // 현재 직선에서 vertex 방향으로 dLen 만큼 떨어진 객체를 만들어 리턴한다.
        // LineType은 현재 직선과 동일하다.
        public Line2D Offset(Vertex2D vertex, double dLen)
        {
            Vertex2D vBegin = GetVertex(true);
            Vertex2D vEnd = GetVertex(false);

            Vertex2D vTarget = Math.GetNearestVertex(vertex, vBegin, vEnd, true);
            double distance = vTarget.GetDistance(vertex);

            if (distance <= Math.HALF_TOLERANCE())
                return new Line2D(vBegin, vEnd, GetLineType());

            Vertex2D vBegin2 = vBegin + (vertex - vTarget) * dLen / distance;
            Vertex2D vEnd2 = vEnd + (vertex - vTarget) * dLen / distance;

            return new Line2D(vBegin, vEnd, GetLineType());
        }

		// 현재 직선에서 오른쪽 방향으로(rightSide가 false이면 왼쪽 방향) dLen 만큼 떨어진 객체를 만들어 리턴한다.
		// LineType은 현재 직선과 동일하다.
		// 방향은 직선의 시작점과 끝점을 기준으로 판단한다.
		public Line2D Offset(bool rightSide, double dLen)
        {
            if (!rightSide)
                dLen = -dLen;

            Vertex2D vBegin = Math.GetRightVertex(m_vBegin, m_vEnd, dLen);
            Vertex2D vEnd = Math.GetRightVertex(m_vEnd, m_vBegin, -dLen);

            return new Line2D(vBegin, vEnd, m_lineType);
        }

		// v1과 v2를 지나는 직선을 기준으로 현재의 직선과 대칭되는 객체를 만들어 리턴한다.
		// LineType은 현재 직선과 동일하다.
		// v1과 v2가 동일한 좌표일 경우 false를 리턴한다.
		public bool Mirror(Vertex2D v1, Vertex2D v2, out Line2D result)
        {
            result = null;

            if (v1.GetDistance(v2) <= Math.HALF_TOLERANCE())
                return false;

            Vertex2D _v1 = Math.GetNearestVertex(m_vBegin, v1, v2, true);
            Vertex2D _v2 = Math.GetNearestVertex(m_vEnd, v1, v2, true);

            Vertex2D vBegin = _v1 * 2 - m_vBegin;
            Vertex2D vEnd = _v2 * 2 - m_vEnd;

            result = new Line2D(vBegin, vEnd, m_lineType);
            return true;
        }

        // 겹쳐진 구간이 있는 두 선분에 대한 결과
        private static int GetSegmentResult(Line2D line1, Line2D line2, ref Vertex2D v1, ref Vertex2D v2, ref LineType resultType)
        {
            Vertex2D vBegin1 = line1.GetVertex(true);
            Vertex2D vEnd1 = line1.GetVertex(false);
            Vertex2D vBegin2 = line2.GetVertex(true);
            Vertex2D vEnd2 = line2.GetVertex(false);

            bool include1 = line1.IsInclude(vBegin2);
            bool include2 = line1.IsInclude(vEnd2);
            bool include3 = line2.IsInclude(vBegin1);
            bool include4 = line2.IsInclude(vEnd1);

            if (include1 && include2)
            {
                v1 = vBegin2;
                v2 = vBegin2;
                resultType = LineType.SEGMENT;
                return 2;
            }
            else if (include1)
            {
                if (include3 && include4)
                {
                    v1 = vBegin1;
                    v2 = vEnd1;
                    resultType = LineType.SEGMENT;
                    return 2;
                }
                else if (include3)
                {
                    v1 = vBegin2;
                    v2 = vBegin1;
                    resultType = LineType.SEGMENT;

                    if (v1.GetDistance(v2) <= Geomini.Math.HALF_TOLERANCE())
                    {
                        resultType = LineType.NO_LINE;
                        return 1;
                    }

                    return 2;
                }
                else// if (include4)
                {
                    v1 = vBegin2;
                    v2 = vEnd1;
                    resultType = LineType.SEGMENT;

                    if (v1.GetDistance(v2) <= Geomini.Math.HALF_TOLERANCE())
                    {
                        resultType = LineType.NO_LINE;
                        return 1;
                    }

                    return 2;
                }
            }
            else if (include2)
            {
                if (include3 && include4)
                {
                    v1 = vBegin1;
                    v2 = vEnd1;
                    resultType = LineType.SEGMENT;
                    return 2;
                }
                else if (include3)
                {
                    v1 = vEnd2;
                    v2 = vBegin1;
                    resultType = LineType.SEGMENT;

                    if (v1.GetDistance(v2) <= Geomini.Math.HALF_TOLERANCE())
                    {
                        resultType = LineType.NO_LINE;
                        return 1;
                    }

                    return 2;
                }
                else// if (include4)
                {
                    v1 = vEnd2;
                    v2 = vEnd1;
                    resultType = LineType.SEGMENT;

                    if (v1.GetDistance(v2) <= Geomini.Math.HALF_TOLERANCE())
                    {
                        resultType = LineType.NO_LINE;
                        return 1;
                    }

                    return 2;
                }
            }
            else if (include3 && include4)
            {
                v1 = vBegin1;
                v2 = vEnd1;
                resultType = LineType.SEGMENT;
                return 2;
            }

            resultType = LineType.NO_LINE;
            return 0;
        }

        // 겹쳐진 구간이 있는 반직선과 선분에 대한 결과
        private static int GetHalfLineSegmentResult(Line2D halfLine, Line2D segment, ref Vertex2D v1, ref Vertex2D v2, ref LineType resultType)
        {
            Vertex2D vBegin1 = halfLine.GetVertex(true);
            Vertex2D vEnd1 = halfLine.GetVertex(false);
            Vertex2D vBegin2 = segment.GetVertex(true);
            Vertex2D vEnd2 = segment.GetVertex(false);

            bool include1 = halfLine.IsInclude(vBegin2);
            bool include2 = halfLine.IsInclude(vEnd2);
            bool include3 = segment.IsInclude(vBegin1);
            bool include4 = segment.IsInclude(vEnd1);

            if (include1 && include2)
            {
                v1 = vBegin2;
                v2 = vBegin2;
                resultType = LineType.SEGMENT;
                return 2;
            }
            else if (include1)
            {
                if (include3 && include4)
                {
                    double dLen1 = vBegin2.GetDistance(vBegin1);
                    double dLen2 = vBegin2.GetDistance(vEnd1);

                    if (dLen1 > dLen2)
                        v2 = vBegin1;
                    else
                        v2 = vEnd1;

                    v1 = vBegin2;
                    resultType = LineType.SEGMENT;
                    return 2;
                }
                else if (include3)
                {
                    v1 = vBegin2;
                    v2 = vBegin1;
                    resultType = LineType.SEGMENT;

                    if (v1.GetDistance(v2) <= Geomini.Math.HALF_TOLERANCE())
                    {
                        resultType = LineType.NO_LINE;
                        return 1;
                    }

                    return 2;
                }
                else// if (include4)
                {
                    v1 = vBegin2;
                    v2 = vEnd1;
                    resultType = LineType.SEGMENT;

                    if (v1.GetDistance(v2) <= Geomini.Math.HALF_TOLERANCE())
                    {
                        resultType = LineType.NO_LINE;
                        return 1;
                    }

                    return 2;
                }
            }
            else if (include2)
            {

                if (include3 && include4)
                {
                    double dLen1 = vEnd2.GetDistance(vBegin1);
                    double dLen2 = vEnd2.GetDistance(vEnd1);

                    if (dLen1 > dLen2)
                        v2 = vBegin1;
                    else
                        v2 = vEnd1;

                    v1 = vEnd2;
                    resultType = LineType.SEGMENT;
                    return 2;
                }
                else if (include3)
                {
                    v1 = vEnd2;
                    v2 = vBegin1;
                    resultType = LineType.SEGMENT;

                    if (v1.GetDistance(v2) <= Geomini.Math.HALF_TOLERANCE())
                    {
                        resultType = LineType.NO_LINE;
                        return 1;
                    }

                    return 2;
                }
                else// if (include4)
                {
                    v1 = vEnd2;
                    v2 = vEnd1;
                    resultType = LineType.SEGMENT;

                    if (v1.GetDistance(v2) <= Geomini.Math.HALF_TOLERANCE())
                    {
                        resultType = LineType.NO_LINE;
                        return 1;
                    }

                    return 2;
                }
            }

            resultType = LineType.NO_LINE;
            return 0;
        }

        // 겹쳐진 구간이 있는 두개의 반직선에 대한 결과
        private static int GetHalfLineResult(Line2D line1, Line2D line2, ref Vertex2D v1, ref Vertex2D v2, ref LineType resultType)
        {
            Vertex2D vBegin1 = line1.GetVertex(true);
            Vertex2D vEnd1 = line1.GetVertex(false);
            Vertex2D vBegin2 = line2.GetVertex(true);
            Vertex2D vEnd2 = line2.GetVertex(false);

            Vertex2D vStart1, vFinish1, vStart2, vFinish2;

            if (line1.m_lineType == LineType.HALF_LINE_BEGIN_2_END)
            {
                vStart1 = vBegin1;
                vFinish1 = vEnd1;
            }
            else
            {
                vStart1 = vEnd1;
                vFinish1 = vBegin1;
            }

            if (line2.m_lineType == LineType.HALF_LINE_BEGIN_2_END)
            {
                vStart2 = vBegin2;
                vFinish2 = vEnd2;
            }
            else
            {
                vStart2 = vEnd2;
                vFinish2 = vBegin2;
            }

            bool include1 = line1.IsInclude(vStart2);
            bool include2 = line2.IsInclude(vStart1);

            if (include1 == false && include2 == false)
            {
                resultType = LineType.NO_LINE;
                return 0;
            }
            else if (include1 && include2)
            {
                Vertex2D vOther1 = vStart1 * 2 - vFinish1;

                if (line2.IsInclude(vOther1))
                {
                    v1 = vStart1;
                    v2 = vStart2;

                    if (v1.GetDistance(v2) <= Math.HALF_TOLERANCE())
                    {
                        resultType = LineType.NO_LINE;
                        return 1;
                    }
                    else
                    {
                        resultType = LineType.SEGMENT;
                        return 2;
                    }
                }
                else
                {
                    v1 = vStart1;
                    v2 = vFinish1;
                    resultType = line1.m_lineType;
                    return 2;
                }
            }
            else if (include1)
            {
                v1 = vStart2;
                v2 = vFinish2;
                resultType = line2.m_lineType;
                return 2;
            }

            v1 = vStart1;
            v2 = vFinish1;
            resultType = line1.m_lineType;
            return 2;
        }

        // y = ax + b
        // x = constant 형태의 직선일 경우 => x = c
        // Return 값 : true이면 x = constant
        private static bool GetLineEquation(Line2D line, out double a, out double b, out double c)
        {
            c = 0.0;

            Vertex2D vBegin = line.GetVertex(true);
            Vertex2D vEnd = line.GetVertex(false);
            bool xEq = false;

            if (System.Math.Abs(vBegin.x - vEnd.x) <= Math.HALF_TOLERANCE())
            {
                a = b = 0.0;
                c = vBegin.x;
                xEq = true;
            }
            else if (System.Math.Abs(vBegin.y - vEnd.y) <= Math.HALF_TOLERANCE())
            {
                a = 0.0;
                b = vBegin.y;
            }
            else
            {
                a = (vEnd.y - vBegin.y) / (vEnd.x - vBegin.x);
                b = vEnd.y - (vEnd.y - vBegin.y) * vEnd.x / (vEnd.x - vBegin.x);
            }

            return xEq;
        }
    }

    public class Line3D
    {
        // HALF_LINE_BEGIN_2_END : 시작점에서 끝점 방향으로 끝없이 이어진 반직선
        // HALF_LINE_END_2_BEGIN : 끝점에서 시작점 방향으로 끝없이 이어진 반직선
        public enum LineType { LINE = 0, HALF_LINE_BEGIN_2_END, HALF_LINE_END_2_BEGIN, SEGMENT, NO_LINE };

        private LineType m_lineType = LineType.SEGMENT;
        private Vertex3D m_vBegin = new Vertex3D();
        private Vertex3D m_vEnd = new Vertex3D();

        public Line3D()
        {
        }

        public Line3D(LineType type)
        {
            SetLineType(type);
        }

        public Line3D(Line3D rhs)
        {
            SetLineType(rhs.m_lineType);
            SetVertex(rhs.m_vBegin, true);
            SetVertex(rhs.m_vEnd, false);
        }

        public Line3D(Vertex3D vBegin, Vertex3D vEnd)
        {
            SetVertex(vBegin, true);
            SetVertex(vEnd, false);
        }

        public Line3D(Vertex3D vBegin, Vertex3D vEnd, LineType type)
        {
            SetLineType(type);
            SetVertex(vBegin, true);
            SetVertex(vEnd, false);
        }

        /*public static bool operator ==(Line3D line1, Line3D line2)
        {
            bool isNull1 = NullChecker.IsNull(line1);
            bool isNull2 = NullChecker.IsNull(line2);

            if (isNull1 && isNull2)
                return true;
            else if (isNull1 || isNull2)
                return false;

            if (line1.GetLineType() != line2.GetLineType())
                return false;
            else if (line1.GetVertex(true) != line2.GetVertex(true))
                return false;
            else if (line1.GetVertex(false) != line2.GetVertex(false))
                return false;

            return true;
        }

        public static bool operator !=(Line3D line1, Line3D line2)
        {
            return !(line1 == line2);
        }*/

        public LineType GetLineType()
        {
            return m_lineType;
        }

        public void SetLineType(LineType type)
        {
            m_lineType = type;
        }

        public void SetVertex(Vertex3D vertex, bool isBegin)
        {
            if (isBegin)
                m_vBegin.CopyFrom(vertex);
            else
                m_vEnd.CopyFrom(vertex);
        }

        // rVertex가 Line내에 포함되어 있는지 알려준다.
        public bool IsInclude(Vertex3D vertex)
        {
            double dLen = this.GetDistance(vertex, false);
            if (dLen <= Math.HALF_TOLERANCE())
                return true;

            return false;
        }

        // noLimit이 true이면 실제 LineType에 상관없이 무한히 뻗은 직선이라 가정하고 계산한다.
        // noLimit이 false이면 실제 LineType을 고려하여 가장 가까운 거리를 구한다.
        public double GetDistance(Vertex3D vertex, bool noLimit)
        {
            Vertex3D vBegin = this.GetVertex(true);
            Vertex3D vEnd = this.GetVertex(false);

            double a = vertex.GetDistance(vBegin);
            double b = vBegin.GetDistance(vEnd);
            double c = vertex.GetDistance(vEnd);

            if (a <= Math.COORD_TOLERANCE() || c <= Math.COORD_TOLERANCE())
                return 0.0;
            if (b <= Math.COORD_TOLERANCE())
                return a;

            double dCos = (a * a + b * b - c * c) / 2 / a / b;
            Vertex3D vertex2 = Math.GetLinearVertex(vBegin, vEnd, dCos * a);
            double dLen = vertex2.GetDistance(vertex);

            LineType type = this.GetLineType();

            if (noLimit || type == LineType.LINE)
                return dLen;

            double dAngle1 = Math.GetAngle(vertex, vBegin, vEnd);
            double dAngle2 = Math.GetAngle(vertex, vEnd, vBegin);

            if (dAngle1 <= Math.HALF_PI() && dAngle2 <= Math.HALF_PI())
                return dLen;

            if (type == LineType.HALF_LINE_BEGIN_2_END)
            {
                if (dAngle1 < Math.HALF_PI())
                    return dLen;
            }
            else if (type == LineType.HALF_LINE_END_2_BEGIN)
            {
                if (dAngle2 < Math.HALF_PI())
                    return dLen;
            }

            return a > c ? c : a;
        }

        public Vertex3D GetVertex(bool isBegin)
        {
            return isBegin ? m_vBegin : m_vEnd;
        }

        // 현재 직선에서 vertex 방향으로 dLen 만큼 떨어진 객체를 만들어 리턴한다.
        // LineType은 현재 직선과 동일하다.
        public Line3D Offset(Vertex3D vertex, double dLen)
        {
            Vertex3D vBegin = GetVertex(true);
            Vertex3D vEnd = GetVertex(false);

            Vertex3D vTarget = Math.GetNearestVertex(vertex, vBegin, vEnd, true);
            double distance = vTarget.GetDistance(vertex);

            if (distance <= Math.HALF_TOLERANCE())
                return new Line3D(vBegin, vEnd, GetLineType());

            Vertex3D vBegin2 = vBegin + (vertex - vTarget) * dLen / distance;
            Vertex3D vEnd2 = vEnd + (vertex - vTarget) * dLen / distance;

            return new Line3D(vBegin, vEnd, GetLineType());
        }

        // v1과 v2를 지나는 직선을 기준으로 현재의 직선과 대칭되는 객체를 만들어 리턴한다.
        // LineType은 현재 직선과 동일하다.
        // v1과 v2가 동일한 좌표일 경우 false를 리턴한다.
        public bool Mirror(Vertex3D v1, Vertex3D v2, out Line3D result)
        {
            result = null;

            if (v1.GetDistance(v2) <= Math.HALF_TOLERANCE())
                return false;

            Vertex3D _v1 = Math.GetNearestVertex(m_vBegin, v1, v2, true);
            Vertex3D _v2 = Math.GetNearestVertex(m_vEnd, v1, v2, true);

            Vertex3D vBegin = _v1 * 2 - m_vBegin;
            Vertex3D vEnd = _v2 * 2 - m_vEnd;

            result = new Line3D(vBegin, vEnd, m_lineType);
            return true;
        }
    }
}
