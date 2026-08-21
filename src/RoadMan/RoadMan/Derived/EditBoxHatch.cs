using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXFViewer;
using UnE.Geometry;
using System.Drawing;

namespace RoadMan
{
    public class EditBoxHatch : Hatch
    {
        // Polygon을 형성하기 위한 최소 개수는 3개이며, 최대 4개까지 가질수 있다.
        private List<Vertex2D> m_editBoxVertices = new List<Vertex2D>();
        private Polygon m_polygon = null;

        // 0 : V0과 V1이 직접 연결되었는가?
        // 1 : V1과 V2가 직접 연결되었는가?
        // 2 : V2와 V3이 직접 연결되었는가?(Vertex 개수가 4개일 경우)
        //     V0과 V2가 직접 연결되었는가?(Vertex 개수가 3개일 경우)
        // 3 : V3과 V0이 직접 연결되었는가?(Vertex 개수가 4개일 경우에만 사용)
        private bool[] m_arrDirectLink = new bool[4] { false, false, false, true };

        /*// Polygon 외곽선을 따르지 않고 직접 연결된 두 점의 Index
        // 상위 2바이트 : 높은 Index
        // 하위 2바이트 : 낮은 Index
        private List<int> m_listDirectConnection = new List<int>();*/
        private bool m_dirPositive = true;
        private int m_nBeginIndex = 0, m_nEndIndex = 0;
        private ScheduleProperty m_linkedScheduleProperty = null;
        private bool m_visibleEditBox = true;

        public const int LIMIT_EDITBOX_COUNT = 4;

        public bool DirPos
        {
            get { return m_dirPositive; }
            set { m_dirPositive = value; }
        }

        public int BeginIndex
        {
            get { return m_nBeginIndex; }
            set { m_nBeginIndex = value; }
        }

        public int EndIndex
        {
            get { return m_nEndIndex; }
            set { m_nEndIndex = value; }
        }

        public ScheduleProperty LinkedScheduleProperty
        {
            get { return m_linkedScheduleProperty; }
            set { m_linkedScheduleProperty = value; }
        }

        public bool VisibleEditBox
        {
            get { return m_visibleEditBox; }
            set { m_visibleEditBox = value; }
        }

        public EditBoxHatch(Polygon polygon)
        {
            m_polygon = polygon;
            m_selectedShowingType = SelectedShowingType.NONE;
        }

        public bool GetDirectLink(int nIndex)
        {
            return m_arrDirectLink[nIndex];
        }

        public void SetDirectLink(int nIndex, bool isDirect)
        {
            m_arrDirectLink[nIndex] = isDirect;
        }

        public void AddEditBoxVertex(Vertex2D vertex)
        {
            FormEditSection.Instance.CurrentHatch = this;
            m_editBoxVertices.Add(vertex);

            int nVertexCount = m_editBoxVertices.Count;

            if (nVertexCount > 1)
            {
                if (FormEditSection.Instance.DirectPrev)
                    m_arrDirectLink[nVertexCount - 2] = true;
                else
                    m_arrDirectLink[nVertexCount - 2] = false;

                if (nVertexCount >= 3)
                {
                    if (FormEditSection.Instance.DirectNext)
                        m_arrDirectLink[nVertexCount - 1] = true;
                    else
                        m_arrDirectLink[nVertexCount - 1] = false;
                }
            }

            Calc();
        }

        public void AddEditBoxVertex2(Vertex2D vertex)
        {
            m_editBoxVertices.Add(vertex);
        }

        public int GetEditBoxVertexCount()
        {
            return m_editBoxVertices.Count;
        }

        public Vertex2D GetEditBoxVertex(int nIndex)
        {
            if (nIndex < 0 || nIndex >= GetEditBoxVertexCount())
                return null;

            return m_editBoxVertices[nIndex];
        }

        public void RemoveEditBoxVertex(int nIndex)
        {
            if (nIndex < 0 || nIndex >= GetEditBoxVertexCount())
                return;

            m_editBoxVertices.RemoveAt(nIndex);
            Calc();
        }

        public void UpdateEditBoxVertex(int nIndex, Vertex2D vertex)
        {
            if (vertex == null)
                return;

            if (nIndex < 0 || nIndex >= GetEditBoxVertexCount())
                return;

            m_editBoxVertices[nIndex].SetVertex(vertex.x, vertex.y);
            Calc();
        }

        public void Calc()
        {
            int nVertexCount = m_editBoxVertices.Count;

            if (nVertexCount <= 2 || m_polygon == null)
                return;

            if (nVertexCount == 3)
            {
                if (CalcPolygonDirection())
                    CalcPoints();
                else
                    SetPointSize(0);
            }
            else
                CalcPoints();
        }

        private void CalcPoints()
        {
            int nVertexCount = m_editBoxVertices.Count;

            if (nVertexCount < 2)
                return;

            List<Vertex2D> listVertices = new List<Vertex2D>();

            int nPrevIndex = 0;
            Vertex2D prev = m_editBoxVertices[nPrevIndex];

            listVertices.Add(prev);
            int nBeginIndex = m_nBeginIndex;

            for (int i=1;i<=nVertexCount;i++)
            {
                Vertex2D vertex = i == nVertexCount ? m_editBoxVertices[0] : m_editBoxVertices[i];

                if (m_arrDirectLink[i - 1])
                {
                    listVertices.Add(vertex);
                    nBeginIndex = GetBeginIndex(nBeginIndex, vertex, prev);
                }
                else
                {
                    if (nBeginIndex < 0)
                        return;

                    nBeginIndex = CalcPoints(nBeginIndex, vertex, prev, listVertices);
                }

                nPrevIndex = i;
                prev = vertex;
            }

            /*for (int i=1;i<nVertexCount;i++)
            {
                int nIndex = GetConnectedIndex(nPrevIndex, i);
                Vertex2D vertex = m_editBoxVertices[i];

                if (m_listDirectConnection.Contains(nIndex))
                {
                    listVertices.Add(vertex);
                    nBeginIndex = GetBeginIndex(nBeginIndex, vertex, prev);
                }
                else
                {
                    nBeginIndex = CalcPoints(nBeginIndex, vertex, prev, listVertices);
                }

                nPrevIndex = i;
                prev = vertex;
            }*/

            Vertex2D vCurrent = m_editBoxVertices[0];

            if (vCurrent.GetDistance(prev) > UnE.Geometry.Math.HALF_TOLERANCE())
            {
                listVertices.Add(vCurrent);
            }

            int nPointCount = listVertices.Count;
            SetPointSize(nPointCount);

            for (int i=0;i<nPointCount;i++)
            {
                Vertex2D vertex = listVertices[i];
                UpdatePoint(i, (float)vertex.x, (float)vertex.y);
            }
        }

        private int GetBeginIndex(int nBeginIndex, Vertex2D vertex, Vertex2D vLast)
        {
            int nVertexCount = m_polygon.GetVertexCount();
            Vertex2D vPrev = vLast;

            if (m_dirPositive)
            {
                for (int i=nBeginIndex;;)
                {
                    Vertex2D vCurrent = m_polygon.GetVertex(i);
                    Line2D line = new Line2D(vPrev, vCurrent);

                    if (line.IsInclude(vertex))
                        return i;

                    i++;
                    vPrev = vCurrent;

                    if (i >= nVertexCount)
                        i = 0;

                    if (i == nBeginIndex)
                        break;
                }
            }
            else
            {
                for (int i=nBeginIndex;;)
                {
                    Vertex2D vCurrent = m_polygon.GetVertex(i);
                    Line2D line = new Line2D(vPrev, vCurrent);

                    if (line.IsInclude(vertex))
                        return i;

                    i--;
                    vPrev = vCurrent;

                    if (i < 0)
                        i = nVertexCount - 1;

                    if (i == nBeginIndex)
                        break;
                }
            }

            return -1;
        }

        private int CalcPoints(int nBeginIndex, Vertex2D vertex, Vertex2D vLast, List<Vertex2D> listVertices)
        {
            int nVertexCount = m_polygon.GetVertexCount();
            Vertex2D vPrev = vLast;

            for (int i=nBeginIndex;;)
            {
                Vertex2D vCurrent = m_polygon.GetVertex(i);
                Line2D line = new Line2D(vPrev, vCurrent);

                if (line.IsInclude(vertex))
                {
                    if (vPrev.GetDistance(vCurrent) > UnE.Geometry.Math.HALF_TOLERANCE())
                    {
                        listVertices.Add(vertex);
                    }

                    return i;
                }
                else
                {
                    if (vPrev.GetDistance(vCurrent) > UnE.Geometry.Math.HALF_TOLERANCE())
                    {
                        listVertices.Add(vCurrent);
                    }
                }

                if (m_dirPositive)
                {
                    i++;

                    if (i >= nVertexCount)
                        i = 0;
                }
                else
                {
                    i--;

                    if (i < 0)
                        i = nVertexCount - 1;
                }

                vPrev = vCurrent;

                if (i == nBeginIndex)
                    break;
            }

            return -1;
        }

        /*private void CalcPoints()
        {
            int nVertexCount = m_polygon.GetVertexCount();
            List<Vertex2D> listVertices = new List<Vertex2D>();

            Vertex2D prev = m_editBoxVertices[0];
            Vertex2D vEnd = m_editBoxVertices[nVertexCount - 1];

            listVertices.Add(prev);
            
            if (m_dirPositive)
            {
                for (int i=m_nBeginIndex;;i++)
                {
                    if (i >= nVertexCount)
                        i = 0;

                    Vertex2D vertex = m_editBoxVertices[i];

                    if (i == m_nEndIndex)
                    {
                        if (prev.GetDistance(vEnd) > UnE.Geometry.Math.HALF_TOLERANCE())
                            listVertices.Add(vEnd);

                        break;
                    }
                    else if (prev.GetDistance(vertex) > UnE.Geometry.Math.HALF_TOLERANCE())
                        listVertices.Add(vertex);

                    prev = vertex;
                }
            }
            else
            {
                for (int i = m_nBeginIndex; ; i--)
                {
                    if (i < 0)
                        i = nVertexCount - 1;

                    Vertex2D vertex = m_editBoxVertices[i];

                    if (i == m_nEndIndex)
                    {
                        if (prev.GetDistance(vEnd) > UnE.Geometry.Math.HALF_TOLERANCE())
                            listVertices.Add(vEnd);

                        break;
                    }
                    else if (prev.GetDistance(vertex) > UnE.Geometry.Math.HALF_TOLERANCE())
                        listVertices.Add(vertex);

                    prev = vertex;
                }
            }
        }*/

        /*private int GetConnectedIndex(int nVertexIndex1, int nVertexIndex2)
        {
            int nIndex = 0;

            if (nVertexIndex1 > nVertexIndex2)
            {
                nIndex = nVertexIndex1 << 16;
                nIndex |= nVertexIndex2;
            }
            else
            {
                nIndex = nVertexIndex2 << 16;
                nIndex |= nVertexIndex1;
            }

            return nIndex;
        }*/

        // m_editBoxVertices의 처음 세 점을 사용하여 Polygon 방향을 결정한다.
        private bool CalcPolygonDirection()
        {
            int nV1BeginIndex = 0, nV1EndIndex = 0;
            int nV3BeginIndex = 0, nV3EndIndex = 0;

            int nVertexCount = m_polygon.GetVertexCount();
            List<int> arrFind = new List<int>();

            Vertex2D v1 = m_editBoxVertices[0];
            Vertex2D v2 = m_editBoxVertices[1];
            Vertex2D v3 = m_editBoxVertices[2];
            bool isComplete = false;

            for (int i=0;i<nVertexCount;i++)
            {
                int nIndex2 = i == nVertexCount - 1 ? 0 : i + 1;
                Vertex2D vBegin = m_polygon.GetVertex(i);
                Vertex2D vEnd = m_polygon.GetVertex(nIndex2);

                Line2D line = new Line2D(vBegin, vEnd);

                double dLen1 = -1.0, dLen2 = -1.0, dLen3 = -1.0;

                if (!arrFind.Contains(1))
                {
                    if (line.IsInclude(v1))
                    {
                        dLen1 = vBegin.GetDistance(v1);
                        nV1BeginIndex = i;
                        nV1EndIndex = nIndex2;
                    }
                }

                if (!arrFind.Contains(2))
                {
                    if (line.IsInclude(v2))
                        dLen2 = vBegin.GetDistance(v2);
                }

                if (!arrFind.Contains(3))
                {
                    if (line.IsInclude(v3))
                    {
                        dLen3 = vBegin.GetDistance(v3);
                        nV3BeginIndex = i;
                        nV3EndIndex = nIndex2;
                    }
                }

                if (dLen1 >= 0.0 && dLen2 >= 0.0 && dLen3 >= 0)
                {
                    if (dLen1 < dLen2)
                    {
                        if (dLen1 < dLen3)
                        {
                            arrFind.Add(1);

                            if (dLen2 < dLen3)
                            {
                                arrFind.Add(2);
                                arrFind.Add(3);
                            }
                            else
                            {
                                arrFind.Add(3);
                                arrFind.Add(2);
                            }
                        }
                        else
                        {
                            arrFind.Add(3);
                            arrFind.Add(1);
                            arrFind.Add(2);
                        }
                    }
                    else
                    {
                        if (dLen2 < dLen3)
                        {
                            arrFind.Add(2);

                            if (dLen1 < dLen3)
                            {
                                arrFind.Add(1);
                                arrFind.Add(3);
                            }
                            else
                            {
                                arrFind.Add(3);
                                arrFind.Add(1);
                            }
                        }
                        else
                        {
                            arrFind.Add(3);
                            arrFind.Add(2);
                            arrFind.Add(1);
                        }
                    }
                }
                else if (dLen1 >= 0.0 && dLen2 >= 0.0)
                {
                    if (dLen1 < dLen2)
                    {
                        arrFind.Add(1);
                        arrFind.Add(2);
                    }
                    else
                    {
                        arrFind.Add(2);
                        arrFind.Add(1);
                    }
                }
                else if (dLen1 >= 0.0 && dLen3 >= 0.0)
                {
                    if (dLen1 < dLen3)
                    {
                        arrFind.Add(1);
                        arrFind.Add(3);
                    }
                    else
                    {
                        arrFind.Add(3);
                        arrFind.Add(1);
                    }
                }
                else if (dLen2 >= 0.0 && dLen3 >= 0.0)
                {
                    if (dLen2 < dLen3)
                    {
                        arrFind.Add(2);
                        arrFind.Add(3);
                    }
                    else
                    {
                        arrFind.Add(3);
                        arrFind.Add(2);
                    }
                }
                else if (dLen1 >= 0.0)
                    arrFind.Add(1);
                else if (dLen2 >= 0.0)
                    arrFind.Add(2);
                else if (dLen3 >= 0.0)
                    arrFind.Add(3);
                else
                    continue;

                if (arrFind.Contains(1) && arrFind.Contains(2) && arrFind.Contains(3))
                {
                    isComplete = true;
                    break;
                }
            }

            if (!isComplete)
                return false;

            if ((arrFind[0] < arrFind[1] && arrFind[1] - arrFind[0] == 1) || (arrFind[0] > arrFind[1] && arrFind[0] - arrFind[1] == 2))
            {
                // Positive
                m_dirPositive = true;
                m_nBeginIndex = nV1EndIndex;
                m_nEndIndex = nV3EndIndex;
                return true;
            }

            // Negative
            m_dirPositive = false;
            m_nBeginIndex = nV1BeginIndex;
            m_nEndIndex = nV3BeginIndex;
            return true;
        }

        public bool CanAdd(Vertex2D vertex)
        {
            int nEditBoxCount = m_editBoxVertices.Count;

            if (nEditBoxCount <= 2)
                return true;

            if (nEditBoxCount >= 4)
                return false;

            int nBoundaryCount = Polygon.GetVertexCount();

            if (nBoundaryCount == 0)
                return true;

            Vertex2D vPrev = Polygon.GetVertex(0);
            Vertex2D vEnd = Polygon.GetVertex(nBoundaryCount - 1);

            int nVertexCount = m_polygon.GetVertexCount();
            
            for (int i=m_nBeginIndex;;)
            {
                Vertex2D vCurrent = m_polygon.GetVertex(i);
                Line2D line = new Line2D(vPrev, vCurrent);

                if (i == m_nEndIndex)
                {
                    if (line.IsInclude(vertex))
                    {
                        double dLen1 = vPrev.GetDistance(vertex);
                        double dLen2 = vPrev.GetDistance(vEnd);

                        return dLen1 > dLen2;
                    }

                    break;
                }
                else if (line.IsInclude(vertex))
                    return false;

                vPrev = vCurrent;

                if (i == nVertexCount - 1)
                    i = 0;
                else
                    i++;
            }

            return true;
        }

        public override bool Draw(System.Drawing.Graphics g, bool bDrawText)
        {
            if (Polygon != null && Polygon.GetVertexCount() >= 3)
            {
                if (!base.Draw(g, bDrawText))
                    return false;
            }

            if (m_visibleEditBox)
            {
                foreach (Vertex2D vertex in m_editBoxVertices)
                {
                    m_editBox.Draw(g, (float)vertex.x, (float)vertex.y);
                }
            }

            return true;
        }

        public void SetFullPolygon()
        {
            if (m_polygon == null)
                return;

            int nVertexCount = m_polygon.GetVertexCount();
            if (nVertexCount < 3)
                return;

            base.SetPointSize(nVertexCount);

            for (int i=0;i<nVertexCount;i++)
            {
                Vertex2D vertex = m_polygon.GetVertex(i);
                base.UpdatePoint(i, (float)vertex.x, (float)vertex.y);
            }
        }
    }
}
