using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;

namespace CadToXML
{
    public partial class Floor
    {
        private class Coord2D
        {
            private class NullChecker
            {
                public static bool IsNull(Object obj)
                {
                    return obj == null;
                }
            }

            private long x = 0;
            private long y = 0;
            private int m_nHashCode = 0;

            public long X
            {
                get { return x; }
                set
                {
                    x = value;
                    SetHashCode();
                }
            }

            public long Y
            {
                get { return y; }
                set
                {
                    y = value;
                    SetHashCode();
                }
            }

            public Coord2D()
            {
                SetHashCode();
            }

            public Coord2D(long x, long y)
            {
                this.x = x;
                this.y = y;
                SetHashCode();
            }

            private void SetHashCode()
            {
                string str = x.ToString() + "_" + y.ToString();
                m_nHashCode = str.GetHashCode();
            }

            public static bool operator ==(Coord2D op1, Coord2D op2)
            {
                bool isNull1 = NullChecker.IsNull(op1);
                bool isNull2 = NullChecker.IsNull(op2);

                if (isNull1 == false && isNull2 == false)
                    return op1.Equals(op2);

                return false;
            }

            public static bool operator !=(Coord2D op1, Coord2D op2)
            {
                bool isNull1 = NullChecker.IsNull(op1);
                bool isNull2 = NullChecker.IsNull(op2);

                if (isNull1 == false && isNull2 == false)
                    return !op1.Equals(op2);

                return true;
            }

            public override bool Equals(object obj)
            {
                if (NullChecker.IsNull(obj))
                    return false;

                if (obj is Coord2D)
                {
                    Coord2D coord = (Coord2D)obj;

                    if (this.x == coord.x && this.y == coord.y)
                        return true;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return m_nHashCode;
            }
        }

        // 벽체들간의 연결관계를 기억시키기 위하여, 벽체의 끝점을 기준으로 끝점과 연결된 벽체들의 리스트를 저장한다.
        private Dictionary<Coord2D, List<Wall>> m_dicCoordWalls = new Dictionary<Coord2D, List<Wall>>();

        private bool CheckClosed(List<PathItem> items)
        {
            Vertex2D vBegin, vMiddle, vEnd;
            Vertex2D vPrev = null;
            bool first = true;

            foreach (PathItem item in items)
            {
                if (item.GetVertex(out vBegin, out vEnd, out vMiddle) == false)
                    return false;

                if (vPrev == null)
                    vPrev = vEnd;
                else
                {
                    if (vPrev.GetDistance(vBegin) <= UnE.Geometry.Math.HALF_TOLERANCE())
                        vPrev = vEnd;
                    else if (vPrev.GetDistance(vEnd) <= UnE.Geometry.Math.HALF_TOLERANCE())
                        vPrev = vBegin;
                    else
                    {
                        if (first)
                        {
                            Vertex2D _vBegin, _vEnd;
                            items[0].GetVertex(out _vBegin, out _vEnd, out vMiddle);

                            vPrev = _vBegin;

                            if (vPrev.GetDistance(vBegin) <= UnE.Geometry.Math.HALF_TOLERANCE())
                                vPrev = vEnd;
                            else if (vPrev.GetDistance(vEnd) <= UnE.Geometry.Math.HALF_TOLERANCE())
                                vPrev = vBegin;
                            else
                                return false;
                        }
                        else
                            return false;
                    }

                    first = false;
                }
            }

            return true;
        }

        public bool MakeShapes(Project.UnitOfLength unit)
        {
            int nWallCount = m_walls.Count;

            // 길이가 1 미만인 벽체는 삭제한다.
            for (int i=0;i<nWallCount;i++)
            {
                Wall wall = m_walls[i];

                Vertex2D vBegin = wall.GetBeginVertex();
                Vertex2D vEnd = wall.GetEndVertex();

                if (vBegin.GetDistance(vEnd) < 1.0)
                {
                    m_walls.RemoveAt(i);
                    nWallCount--;
                    i--;
                }
            }

            SetWallLink();

            List<Space> removeSpaces = new List<Space>();

            foreach (Space space in m_spaces)
            {
                if (space.MakeShape(unit) == false)
                {
                    removeSpaces.Add(space);
                    continue;
                }

                if (CheckClosed(space.Boundary) == false)
                    return false;
            }

            foreach (Space space in removeSpaces)
            {
                foreach (Wall wall in space.Walls)
                {
                    wall.RemoveSpace(space);
                }

                m_spaces.Remove(space);
            }

            // 공간에 속해있지 않은 외곽벽체들을 이용하여 Polygon을 생성한다.
            Wall.MakeOutsideWallLine(m_walls, unit);

            // 벽체영역 계산
            foreach (Wall wall in m_walls)
            {
                wall.MakeShape(this);

                if (wall.Boundary != null)
                {
                    if (CheckClosed(wall.Boundary) == false)
                        return false;
                    else if (wall.BoundaryPolygon != null)
                    {
                        Space.CheckPolygonValidation(wall.BoundaryPolygon);
                    }
                }
            }

            return true;
        }

        private void SetWallLink()
        {
            m_dicCoordWalls.Clear();

            foreach (Wall wall in m_walls)
            {
                Vertex2D vBegin = wall.GetBeginVertex();
                Vertex2D vEnd = wall.GetEndVertex();

                Coord2D cBegin = new Coord2D((long)vBegin.x, (long)vBegin.y);
                Coord2D cEnd = new Coord2D((long)vEnd.x, (long)vEnd.y);

                List<Wall> walls = null;

                if (m_dicCoordWalls.TryGetValue(cBegin, out walls) == false)
                {
                    walls = new List<Wall>();
                    m_dicCoordWalls[cBegin] = walls;
                }

                walls.Add(wall);

                if (m_dicCoordWalls.TryGetValue(cEnd, out walls) == false)
                {
                    walls = new List<Wall>();
                    m_dicCoordWalls[cEnd] = walls;
                }

                walls.Add(wall);
            }
        }

        public List<Wall> GetLinkedWall(Wall wall, bool isBegin)
        {
            if (wall == null)
                return null;

            Vertex2D vertex = isBegin ? wall.GetBeginVertex() : wall.GetEndVertex();
            Coord2D coord = new Coord2D((long)vertex.x, (long)vertex.y);

            List<Wall> walls = null;
            m_dicCoordWalls.TryGetValue(coord, out walls);
            return walls;
        }

        public List<Space> GetSpecialSpaces(List<Space.SpaceType> types)
        {
            List<Space> spaces = new List<Space>();

            foreach (Space space in m_spaces)
            {
                if (types.Contains(space.Type))
                    spaces.Add(space);
            }

            return spaces;
        }

        public List<Space> GetSpecialSpaces(Space.SpaceType type)
        {
            List<Space> spaces = new List<Space>();

            foreach (Space space in m_spaces)
            {
                if (space.Type == type)
                    spaces.Add(space);
            }

            return spaces;
        }
    }
}
