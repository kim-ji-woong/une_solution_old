using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXFViewer;
using System.Windows.Forms;
using System.Collections;
using UnE.Geometry;

namespace CadToXML
{
    public class DXFManager
    {
        public static Project ReadFile(string strPath, out double dMoveX, out double dMoveY)
        {
            dMoveX = dMoveY = 0.0;

            DXFControl ctrl = new DXFControl();

            if (ctrl.OpenDXF(strPath) == false)
            {
                MessageBox.Show("DXF 파일을 열수 없습니다.\r\n다른 프로그램에서 독립적으로 파일을 사용중인지 확인하세요.");
                return null;
            }

            ctrl.MoveAll(ctrl.ObjectTL.x, ctrl.ObjectBR.y);
            dMoveX = ctrl.ObjectTL.x;
            dMoveY = ctrl.ObjectBR.y;

            Project project = new Project();

            if (ReadFloors(ctrl, project) == false)
            {
                Floor floor = new Floor();
                floor.ID = "L1";
                floor.Name = "1층";
                project.Floors.Add(floor);
            }

            //Floor prevFloor = null;
            //Dictionary<Space, Topology.Node> prevSpaceTopologyNodes = null;

            foreach (Floor floor in project.Floors)
            {
                if (ReadWalls(ctrl.Layers, floor))
                {
                    ReadSpaces(ctrl.Layers, floor);
                    ReadSpecialSpaces(ctrl.Layers, floor);
                    ReadAlertAreas(ctrl.Layers, floor);
                    ReadDoors(ctrl.Layers, floor);
                    ReadWindows(ctrl.Layers, floor);
                    ReadColumns(ctrl.Layers, floor);

                    // MovingPosition 만큼 이동
                    floor.Move();

                    //prevSpaceTopologyNodes = MakeTopology(floor, prevFloor, prevSpaceTopologyNodes);
                }

                SetWallIDs(floor);
                SetSpaceIDs(floor);
                SetAlertAreaIDs(floor);
                SetColumnIDs(floor);
                //SetTopologyIDs(floor);

                //prevFloor = floor;
            }

            return project;
        }

        private static void SetWallIDs(Floor floor)
        {
            int nDoorIndex = 1;
            int nWindowIndex = 1;

            for (int i=0;i<floor.Walls.Count;i++)
            {
                Wall wall = floor.Walls[i];
                wall.ID = string.Format("w{0}_{1}", i + 1, floor.ID);

                foreach (Door door in wall.Doors)
                {
                    door.ID = string.Format("d{0}_{1}", nDoorIndex++, floor.ID);
                }

                foreach (Window window in wall.Windows)
                {
                    window.ID = string.Format("win{0}_{1}", nWindowIndex++, floor.ID);
                }
            }
        }

        private static void SetSpaceIDs(Floor floor)
        {
            int nRoomID = 1;
            int nStairRoomID = 1;
            int nElevatorRoomID = 1;
            int nEscalatorRoomID = 1;

            for (int i = 0; i < floor.Spaces.Count; i++)
            {
                Space space = floor.Spaces[i];
                space.ID = string.Format("s{0}_{1}", i + 1, floor.ID);

                Property property = new Property();
                property.Name = "실종류";

                if (space.Type == Space.SpaceType.Normal)
                {
                    space.Name = string.Format("room{0}_{1}", nRoomID++, floor.ID);
                    property.Value = "일반실";
                }
                else if (space.Type == Space.SpaceType.StairRoom)
                {
                    space.Name = string.Format("계단실{0}_{1}", nStairRoomID++, floor.ID);
                    property.Value = "계단실";
                }
                else if (space.Type == Space.SpaceType.ElevatorRoom)
                {
                    space.Name = string.Format("엘리베이터{0}_{1}", nElevatorRoomID++, floor.ID);
                    property.Value = "엘리베이터";
                }
                else if (space.Type == Space.SpaceType.EscalatorRoom)
                {
                    space.Name = string.Format("에스컬레이터{0}_{1}", nEscalatorRoomID++, floor.ID);
                    property.Value = "에스컬레이터";
                }

                space.Properties.Add(property);
            }
        }

        private static void SetAlertAreaIDs(Floor floor)
        {
            int nAlertAreaID = 1;

            for (int i = 0; i < floor.AlertAreas.Count; i++)
            {
                AlertArea alertArea = floor.AlertAreas[i];
                alertArea.ID = string.Format("a{0}_{1}", i + 1, floor.ID);

                alertArea.Name = string.Format("alertArea{0}_{1}", nAlertAreaID++, floor.ID);
            }
        }

        private static void SetColumnIDs(Floor floor)
        {
            for (int i = 0; i < floor.Columns.Count; i++)
            {
                Column column = floor.Columns[i];
                column.ID = string.Format("c{0}_{1}", i + 1, floor.ID);
            }
        }

        public static void SetTopologyIDs(Floor floor)
        {
            int nNodeIndex = 1;
            
            for (int i = 0; i < floor.Topologies.Count; i++)
            {
                Topology topology = floor.Topologies[i];
                topology.ID = string.Format("t{0}_{1}", i + 1, floor.ID);

                foreach (Topology.Node node in topology.Nodes)
                {
                    node.ID = string.Format("tn{0}_{1}", nNodeIndex++, floor.ID);
                }
            }
        }

        private static bool ReadFloors(DXFControl ctrl, Project project)
        {
            UnE.Geometry.Vertex2D vFirstFloorPosition = null;

            foreach (Block block in ctrl.Blocks)
            {
                Floor floor = new Floor();
                floor.Name = block.Name;
                project.Floors.Add(floor);

                bool findFloorIndex = false;

                foreach (Shape shape in block.Shapes)
                {
                    Layer layer = shape.GetLayer();

                    if (layer != null && (string.Compare(layer.LayerName, "B_BasePoint", true) == 0 || string.Compare(layer.LayerName, "0_BasePoint", true) == 0))
                    {
                        if (shape is Point)
                        {
                            Point point = (Point)shape;
                            floor.MovingPosition = point.Vertex;
                        }
                    }
                    else if (layer != null && (string.Compare(layer.LayerName, "B_Text", true) == 0 || string.Compare(layer.LayerName, "0_Text", true) == 0))
                    {
                        if (shape is Text)
                        {
                            Text text = (Text)shape;

                            int nFloorIndex;

                            if (GetFloorIndex(text.Title, out nFloorIndex))
                            {
                                floor.Name = text.Title;
                                floor.FloorIndex = nFloorIndex;
                                block.Name = floor.Name;
                                findFloorIndex = true;
                            }
                        }
                    }
                }

                if (findFloorIndex == false)
                {
                    int nFloorIndex;

                    if (GetFloorIndex(floor.Name, out nFloorIndex))
                        floor.FloorIndex = nFloorIndex;
                }
                //ym0730.id두자리
                //floor.ID = floor.FloorIndex < 0 ? string.Format("B{0}F", -floor.FloorIndex) : string.Format("{0}F", floor.FloorIndex + 1);                       
               if (floor.FloorIndex < 0)
                    floor.ID = (-floor.FloorIndex) < 10 ? string.Format("B{0:00}F", (-floor.FloorIndex)) : string.Format("B{0}F", (-floor.FloorIndex));                                   
                else                                                   
                    floor.ID = (floor.FloorIndex + 1) < 10 ? string.Format("{0:00}F", (floor.FloorIndex + 1)) : string.Format("{0}F", (floor.FloorIndex + 1));                  
              
                if (floor.FloorIndex == 10000)
                    floor.ID = "Roof";

                if (floor.FloorIndex == 0)
                    vFirstFloorPosition = floor.MovingPosition;
            }

            project.Floors.Sort();

            if (vFirstFloorPosition == null)
            {
                // 1층이 없으면 첫번째 층을 기준으로 한다.
                if (project.Floors.Count > 0)
                    vFirstFloorPosition = project.Floors[0].MovingPosition;
            }

            foreach (Floor floor in project.Floors)
            {
                // 기준점을 0,0 으로 수정
                //floor.MovingPosition = floor.MovingPosition - vFirstFloorPosition; 
                floor.MovingPosition = floor.MovingPosition;

                // TODO: 디버깅 시에 좌표 파악을 위한 코드
                //floor.MovingPosition = new Vertex2D(0, 0);
            }

            return project.Floors.Count > 0;
        }

        private static bool GetFloorIndex(string strFloorName, out int nFloorIndex)
        {
            nFloorIndex = 0;
            strFloorName = strFloorName.Trim();

            if (strFloorName.ToLower().StartsWith("roof"))
            {
                nFloorIndex = 10000;
                return true;
            }

            if (strFloorName.StartsWith("B"))
            {
                int nIndex = strFloorName.IndexOf('F');

                if (nIndex < 0)
                    return false;

                string strIndex = strFloorName.Substring(1, nIndex - 1);

                if (int.TryParse(strIndex, out nFloorIndex))
                    nFloorIndex = -nFloorIndex;
                else
                    return false;
            }
            else
            {
                int nIndex = strFloorName.IndexOf('F');

                if (nIndex < 0)
                    return false;

                string strIndex = strFloorName.Substring(0, nIndex);

                if (int.TryParse(strIndex, out nFloorIndex))
                    nFloorIndex--;
                else
                    return false;
            }

            return true;
        }

        private static int ReadSpecialSpaces(ArrayList layers, Floor floor)
        {
            int nSpaceCount = 0;

            foreach (Layer layer in layers)
            {
                if (string.Compare(layer.LayerName, "B_StairBoundary", true) == 0 || string.Compare(layer.LayerName, "0_StairBoundary", true) == 0)
                    nSpaceCount += ReadSpecialSpaces(layer, floor, Space.SpaceType.StairRoom);
                else if (string.Compare(layer.LayerName, "B_EscalBoundary", true) == 0 || string.Compare(layer.LayerName, "0_EscalBoundary", true) == 0)
                    nSpaceCount += ReadSpecialSpaces(layer, floor, Space.SpaceType.EscalatorRoom);
                else if (string.Compare(layer.LayerName, "B_ElevBoundary", true) == 0 || string.Compare(layer.LayerName, "0_ElevBoundary", true) == 0)
                    nSpaceCount += ReadSpecialSpaces(layer, floor, Space.SpaceType.ElevatorRoom);
            }

            return nSpaceCount;
        }

        private static int ReadSpecialSpaces(Layer layer, Floor floor, Space.SpaceType spaceType)
        {
            int nSpaceCount = 0;
            List<UnE.Geometry.Polygon> noSpacePolygons = new List<UnE.Geometry.Polygon>();

            foreach (Shape shape in layer.Shapes)
            {
                if (CheckFloor(shape, floor.Name))
                {
                    if (shape is PolyLine)
                    {
                        PolyLine polyline = (PolyLine)shape;

                        int nPrevCount = noSpacePolygons.Count;
                        Space space = FindSpace(polyline.GetPolygon(), floor.Spaces, floor.Walls, noSpacePolygons);
                        int nCurrentCount = noSpacePolygons.Count;

                        if (space == null)
                        {
                            if (nPrevCount == nCurrentCount)
                            {
                                string strError = string.Format("Error, Unknown Space : {0}, {1}", layer.LayerName, spaceType.ToString());
                                System.Diagnostics.Trace.WriteLine(strError);
                            }

                            continue;
                        }

                        space.Type = spaceType;
                        nSpaceCount++;
                    }
                }
            }

            foreach (UnE.Geometry.Polygon polygon in noSpacePolygons)
            {
                Space space = FindSpace(polygon, floor.Spaces);

                if (space == null)
                {
                    System.Diagnostics.Trace.WriteLine("Unknown Polygon");
                }
                else
                {
                    space.Type = spaceType;
                    nSpaceCount++;
                }
            }

            return nSpaceCount;
        }

        private static Space FindSpace(UnE.Geometry.Polygon polygon ,List<Space> spaces)
        {
            double dArea = polygon.GetArea();

            foreach (Space space in spaces)
            {
                UnE.Geometry.Polygon spacePolygon = space.GetPolygon();
                double spaceArea = spacePolygon.GetArea();

                if (System.Math.Abs(dArea - spaceArea) < 1.0)
                {
                    int i;
                    int nVertexCount = polygon.GetVertexCount();

                    for (i=0;i<nVertexCount;i++)
                    {
                        UnE.Geometry.Vertex2D vertex = polygon.GetVertex(i);
                        bool find = false;

                        int nVertexCount2 = spacePolygon.GetVertexCount();
                        int nBeginIndex = 0;

                        for (int j = 0; j < nVertexCount2; j++)
                        {
                            int nIndex1 = j + nBeginIndex;
                            int nIndex2 = j + nBeginIndex + 1;

                            if (nIndex1 >= nVertexCount2)
                                nIndex1 -= nVertexCount2;

                            if (nIndex2 >= nVertexCount2)
                                nIndex2 -= nVertexCount2;

                            UnE.Geometry.Vertex2D v1 = spacePolygon.GetVertex(nIndex1);
                            UnE.Geometry.Vertex2D v2 = spacePolygon.GetVertex(nIndex2);
                            UnE.Geometry.Line2D line = new UnE.Geometry.Line2D(v1, v2);

                            if (line.IsInclude(vertex))
                            {
                                find = true;
                                break;
                            }
                        }

                        if (find == false)
                            break;
                    }

                    if (i == nVertexCount)
                        return space;
                }
            }

            return null;
        }

        private static Space FindSpace(UnE.Geometry.Polygon polygon, List<Space> spaces, List<Wall> walls, List<UnE.Geometry.Polygon> noSpacePolygons)
        {
            Space space = PolygonToSpace(polygon, walls, noSpacePolygons);

            if (space == null)
                return null;

            foreach (Space _space in spaces)
            {
                if (IsSameSpace(space, _space))
                    return _space;
            }

            return null;
        }

        private static bool IsSameSpace(Space space1, Space space2)
        {
            if (space1.Walls.Count != space2.Walls.Count)
                return false;

            foreach (Wall wall1 in space1.Walls)
            {
                bool find = false;

                foreach (Wall wall2 in space2.Walls)
                {
                    if (wall1 == wall2)
                    {
                        find = true;
                        break;
                    }
                }

                if (find == false)
                    return false;
            }

            return true;
        }

        private static bool ReadSpaces(ArrayList layers, Floor floor)
        {
            List<Space> spaces = floor.Spaces;
            List<Wall> walls = floor.Walls;

            //Geometry.PolygonBuilder polygonBuilder = new Geometry.PolygonBuilder();
            UnE.Geometry.PolygonBuilder polygonBuilder = new UnE.Geometry.PolygonBuilder();

            foreach (Wall wall in walls)
            {
                if (wall.Type != Wall.WallType.NoSpace)
                    polygonBuilder.AddLine(wall.Begin, wall.End);
                else
                    System.Diagnostics.Trace.WriteLine(wall);
            }

            List<Line2D> lines = null;
            /*List<Arc2D> arcs = null;
            List<EArc2D> earcs = null;
            List<UnE.Geometry.Polygon> polygons = polygonBuilder.MakePolygon(out lines, out arcs, out earcs);*/
            List<UnE.Geometry.Polygon> polygons = polygonBuilder.MakePolygon(out lines); 

            if (polygons == null || lines == null)
                return false;

            List<Wall> newWalls = LinesToWalls(lines, walls);
            walls.Clear();
            walls.AddRange(newWalls);

            foreach (UnE.Geometry.Polygon polygon in polygons)
            {
                if (CheckPolygonValidation(polygon) == false)
                    continue;

                Space space = PolygonToSpace(polygon, walls);

                if (space != null)
                {
                    foreach (Wall wall in space.Walls)
                    {
                        // Topology 생성을 위하여 저장해 둔다.
                        // 문의 유무를 파악하여 나중에 다시 정리한다. 
                        wall.AddLinkedSpace(space);
                    }

                    spaces.Add(space);
                }
            }

            return true;
        }

        private static void RemovePolygonSameVertex(Polygon polygon)
        {
            int nVertexCount = polygon.GetVertexCount();

            if (nVertexCount < 3)
                return;

            if (polygon.GetVertex(0).GetDistance(polygon.GetVertex(nVertexCount - 1)) < UnE.Geometry.Math.HALF_TOLERANCE())
            {
                // 시작점과 끝점이 같다면...
                nVertexCount--;
            }

            if (nVertexCount < 3)
                return;

            List<int> removeIDs = new List<int>();
            Vertex2D vPrev = polygon.GetVertex(0);

            for (int i=1;i<nVertexCount;i++)
            {
                Vertex2D vCurrent = polygon.GetVertex(i);

                if (vPrev.GetDistance(vCurrent) < UnE.Geometry.Math.HALF_TOLERANCE())
                    removeIDs.Add(i);
                else
                    vPrev = vCurrent;
            }

            for (int i=removeIDs.Count-1;i>=0;i--)
            {
                polygon.RemoveVertex(removeIDs[i]);
            }
        }

        private static bool CheckPolygonValidation(Polygon polygon)
        {
            RemovePolygonSameVertex(polygon);

            int nVertexCount = polygon.GetVertexCount();

            if (nVertexCount < 3)
                return false;

            int nVertexCount2 = nVertexCount;

            Vertex2D v1 = polygon.GetVertex(0);
            Vertex2D v2 = polygon.GetVertex(1);
            List<int> removeIndex = new List<int>();

            for (int i=2;i<nVertexCount;i++)
            {
                Vertex2D vertex = polygon.GetVertex(i);
                Line2D line = new Line2D(v1, v2, Line2D.LineType.LINE);

                if (line.IsInclude(vertex))
                {
                    Line2D line2 = new Line2D(v1, vertex, Line2D.LineType.SEGMENT);

                    if (line2.IsInclude(v2))
                    {
                        // 연속으로 두 개의 점이 일직선으로 배열된 경우
                        nVertexCount2--;
                        /*removeIndex.Add(i - 1);
                        v2 = vertex;
                        continue;*/
                    }
                    else
                    {
                        // v1에서 v2 방향으로 진행되었다가 다시 v1 방향으로 되돌아온 경우
                        return false;
                    }
                }

                v1 = v2;
                v2 = vertex;
            }

            int nRemoveCount = removeIndex.Count;

            for (int i=nRemoveCount-1;i>=0;i--)
            {
                int nIndex = removeIndex[i];
                polygon.RemoveVertex(nIndex);
            }

            if (nVertexCount2 - nRemoveCount < 3)
                return false;

            return true;
        }

        private static Space PolygonToSpace(UnE.Geometry.Polygon polygon, List<Wall> walls, List<UnE.Geometry.Polygon> noSpacePolygons = null)
        {
            int nVertexCount = polygon.GetVertexCount();

            if (nVertexCount < 3)
                return null;

            UnE.Geometry.Vertex2D vBegin = polygon.GetVertex(0);
            UnE.Geometry.Vertex2D vLast = polygon.GetVertex(nVertexCount - 1);

            if (IsSameVertex(vBegin, vLast) == false)
            {
                polygon.AddVertex(vBegin);
                nVertexCount++;
            }

            if (nVertexCount < 4)
                return null;

            Space space = new Space();
            UnE.Geometry.Vertex2D vPrev = vBegin;

            for (int i=1;i<nVertexCount;i++)
            {
                UnE.Geometry.Vertex2D vCurrent = polygon.GetVertex(i);

                if (vPrev.GetDistance(vCurrent) < 0.001)
                {
                    vPrev = vCurrent;
                    continue;
                }

                Wall wall = FindWall(vPrev, vCurrent, walls);

                if (wall == null)
                {
                    if (noSpacePolygons != null)
                    {
                        // 같은 모양의 공간이 존재할수 있으니 List에 일단 담아둔다.
                        noSpacePolygons.Add(polygon);
                        return null;
                    }

                    string strError = string.Format("Error, Unknown Polygon Line : ({0}, {1}) ~ ({2}, {3})", vPrev.x, vPrev.y, vCurrent.x, vCurrent.y);
                    System.Diagnostics.Trace.WriteLine(strError);
                    return null;
                }

                space.Walls.Add(wall);
                vPrev = vCurrent;
            }

            return space;
        }

        public static bool IsSameVertex(UnE.Geometry.Vertex2D v1, UnE.Geometry.Vertex2D v2)
        {
            if (v1.GetDistance(v2) <= 0.1)
                return true;

            return false;
        }

        private static List<Wall> LinesToWalls(List<UnE.Geometry.Line2D> lines, List<Wall> oldWalls)
        {
            double dTolerance = 0.1;
            List<Wall> newWalls = new List<Wall>();

            foreach (UnE.Geometry.Line2D line in lines)
            {
                Wall wall = FindWall(line, oldWalls);

                if (wall == null)
                {
                    string strError = string.Format("Error, Unknown Line : ({0}, {1}) ~ ({2}, {3})", line.GetVertex(true).x, line.GetVertex(true).y, line.GetVertex(false).x, line.GetVertex(false).y);
                    System.Diagnostics.Trace.WriteLine(strError);
                    continue;
                }

                if (line.GetVertex(true).GetDistance(line.GetVertex(false)) < dTolerance)
                    continue;

                Wall newWall = new Wall();

                newWall.Begin = line.GetVertex(true);
                newWall.End = line.GetVertex(false);
                newWall.Type = wall.Type;

                newWalls.Add(newWall);
            }

            return newWalls;
        }

        private static Wall FindWall(UnE.Geometry.Vertex2D v1, UnE.Geometry.Vertex2D v2, List<Wall> walls)
        {
            double dTolerance = 1.0;
            //double dTolerance = 0.001;

            foreach (Wall wall in walls)
            {
                if (wall.Line.GetDistance(v1, false) < dTolerance && wall.Line.GetDistance(v2, false) < dTolerance)
                    return wall;
                //if (wall.Line.IsInclude(v1) && wall.Line.IsInclude(v2))
                //    return wall;
            }

            return null;
        }

        private static Wall FindWall(UnE.Geometry.Line2D line, List<Wall> walls)
        {
            UnE.Geometry.Vertex2D vBegin = line.GetVertex(true);
            UnE.Geometry.Vertex2D vEnd = line.GetVertex(false);

            return FindWall(vBegin, vEnd, walls);
        }

        // isSwing : 양쪽으로 열리는가?
        private static Wall FindWall(PolyLine pLine, int nVertexCount, List<Wall> walls, out bool isSwing, out UnE.Geometry.Vertex2D vDoorBegin, out UnE.Geometry.Vertex2D vDoorEnd, out UnE.Geometry.Vertex2D vHinge1, out UnE.Geometry.Vertex2D vHinge2)
        {
            List<UnE.Geometry.Vertex2D> vertices = new List<UnE.Geometry.Vertex2D>();
            Polygon polygon = pLine.GetPolygon();

            for (int i=0;i<nVertexCount;i++)
            {
                Vertex2D vertex = polygon.GetVertex(i);
                //System.Drawing.PointF pt = pLine.GetVertex(i);
                //UnE.Geometry.Vertex2D vertex = new UnE.Geometry.Vertex2D(pt.X, pt.Y);
                vertices.Add(vertex);
            }

            double dTolerance = 0.1;

            List<UnE.Geometry.Vertex2D> doorPoints = new List<UnE.Geometry.Vertex2D>();
            UnE.Geometry.Vertex2D v1, v2;
            UnE.Geometry.Line2D.LineType resultType;
            Wall findWall = null;

            isSwing = false;
            vHinge1 = vHinge2 = null;
            vDoorBegin = vDoorEnd = null;

            foreach (Wall wall in walls)
            {
                for (int i=1;i<nVertexCount;i++)
                {
                    UnE.Geometry.Vertex2D vBegin = vertices[i - 1];
                    UnE.Geometry.Vertex2D vEnd = vertices[i];
                    UnE.Geometry.Line2D line = new UnE.Geometry.Line2D(vBegin, vEnd);

                    int nCount = wall.Line.IntersectLine(line, out v1, out v2, out resultType);

                    if (nCount == 1)
                        AddDoorPoints(v1, doorPoints, dTolerance);
                    else if (nCount == 2)
                    {
                        AddDoorPoints(v1, doorPoints, dTolerance);
                        AddDoorPoints(v2, doorPoints, dTolerance);
                    }
                }

                if (doorPoints.Count >= 2)
                {
                    findWall = wall;
                    break;
                }
                else
                    doorPoints.Clear(); 
            }

            if (findWall == null)
                return null;

            int nPointCount = doorPoints.Count;

            if (nPointCount >= 4)
            {
                System.Diagnostics.Trace.WriteLine("문의 교점이 너무 많습니다.");
                return null;
            }

            m_vDoorLineBegin = findWall.Begin;
            doorPoints.Sort(CompareDoorPoints);

            // 1 : 오른쪽, 2 : 왼쪽, 3 : 양쪽
            int nDirection = 0;
            bool nearBegin = false;

            Line2D line2 = new Line2D(doorPoints[0], doorPoints[nPointCount - 1]);

            for (int i=0;i<nVertexCount;i++)
            {
                UnE.Geometry.Vertex2D vertex = vertices[i];

                if (FindVertex(vertex, doorPoints, dTolerance))
                    continue;

                int nResult = UnE.Geometry.Math.IsRightSideFromLine(vertex, doorPoints[0], doorPoints[nPointCount - 1]);

                if (nResult >= 0)
                {
                    double dLength = line2.GetDistance(vertex, true);

                    // 벽체와의 거리가 1 이하이면 벽체위에 있는 점으로 간주한다.
                    if (dLength < 1.0)
                        nResult = -1;
                }

                if (nResult == 1)
                {
                    if (nDirection == 2)
                    {
                        nDirection = 3;
                        break;
                    }
                    else
                        nDirection = 1;
                }
                else if (nResult == 0)
                {
                    if (nDirection == 1)
                    {
                        nDirection = 3;
                        break;
                    }
                    else
                        nDirection = 2;
                }

                if (vertex.GetDistance(doorPoints[0]) < vertex.GetDistance(doorPoints[nPointCount - 1]))
                    nearBegin = true;
                else
                    nearBegin = false;
            }

            isSwing = nDirection == 3;

            if (nDirection == 3 || nDirection == 1)
            {
                if (nPointCount == 3)
                {
                    double len1 = doorPoints[0].GetDistance(doorPoints[1]);
                    double len2 = doorPoints[1].GetDistance(doorPoints[2]);

                    vHinge1 = UnE.Geometry.Math.GetRightVertex(doorPoints[0], doorPoints[2], len1);
                    vHinge2 = UnE.Geometry.Math.GetRightVertex(doorPoints[2], doorPoints[0], -len2);
                }
                else
                {
                    double len = doorPoints[0].GetDistance(doorPoints[1]);

                    if (nearBegin)
                        vHinge1 = UnE.Geometry.Math.GetRightVertex(doorPoints[0], doorPoints[1], len);
                    else
                        vHinge1 = UnE.Geometry.Math.GetRightVertex(doorPoints[1], doorPoints[0], -len);
                }
            }
            else
            {
                if (nPointCount == 3)
                {
                    double len1 = doorPoints[0].GetDistance(doorPoints[1]);
                    double len2 = doorPoints[1].GetDistance(doorPoints[2]);

                    vHinge1 = UnE.Geometry.Math.GetRightVertex(doorPoints[0], doorPoints[2], -len1);
                    vHinge2 = UnE.Geometry.Math.GetRightVertex(doorPoints[2], doorPoints[0], len2);
                }
                else
                {
                    double len = doorPoints[0].GetDistance(doorPoints[1]);

                    if (nearBegin)
                        vHinge1 = UnE.Geometry.Math.GetRightVertex(doorPoints[0], doorPoints[1], -len);
                    else
                        vHinge1 = UnE.Geometry.Math.GetRightVertex(doorPoints[1], doorPoints[0], len);
                }
            }

            vDoorBegin = doorPoints[0];
            vDoorEnd = doorPoints[nPointCount - 1];
            return findWall;
        }

        private static bool FindVertex(UnE.Geometry.Vertex2D vertex, List<UnE.Geometry.Vertex2D> vertices, double dTolerance)
        {
            foreach (UnE.Geometry.Vertex2D v in vertices)
            {
                if (vertex.GetDistance(v) <= dTolerance)
                    return true;
            }

            return false;
        }

        private static UnE.Geometry.Vertex2D m_vDoorLineBegin = null;

        private static int CompareDoorPoints(UnE.Geometry.Vertex2D v1, UnE.Geometry.Vertex2D v2)
        {
            double len1 = m_vDoorLineBegin.GetDistance(v1);
            double len2 = m_vDoorLineBegin.GetDistance(v2);
            return len1.CompareTo(len2);
        }

        private static void AddDoorPoints(UnE.Geometry.Vertex2D vertex, List<UnE.Geometry.Vertex2D> doorPoints, double dTolerance)
        {
            foreach (UnE.Geometry.Vertex2D point in doorPoints)
            {
                if (point.GetDistance(vertex) <= dTolerance)
                    return;
            }

            doorPoints.Add(vertex);
        }

        private static bool ReadWalls(ArrayList layers, Floor floor)
        {
            foreach (Layer layer in layers)
            {
                if (string.Compare(layer.LayerName, "B_Wall_S", true) == 0 || string.Compare(layer.LayerName, "0_Wall_S", true) == 0)
                    ReadWalls(layer, floor.Walls, floor.Name, Wall.WallType.Structure);
                else if (string.Compare(layer.LayerName, "B_Wall_P", true) == 0 || string.Compare(layer.LayerName, "0_Wall_P", true) == 0)
                    ReadWalls(layer, floor.Walls, floor.Name, Wall.WallType.Partition);
                else if (string.Compare(layer.LayerName, "B_Wall_F", true) == 0 || string.Compare(layer.LayerName, "0_Wall_F", true) == 0)
                    ReadWalls(layer, floor.Walls, floor.Name, Wall.WallType.Fake);
                else if (string.Compare(layer.LayerName, "B_Wall_C", true) == 0 || string.Compare(layer.LayerName, "0_Wall_C", true) == 0)
                    ReadWalls(layer, floor.Walls, floor.Name, Wall.WallType.CurtainWall);
                else if (string.Compare(layer.LayerName, "B_Wall_N", true) == 0 || string.Compare(layer.LayerName, "0_Wall_N", true) == 0)
                    ReadWalls(layer, floor.Walls, floor.Name, Wall.WallType.NoSpace);
                else if (string.Compare(layer.LayerName, "B_Handrail", true) == 0 || string.Compare(layer.LayerName, "0_Handrail", true) == 0)
                    ReadWalls(layer, floor.Walls, floor.Name, Wall.WallType.Handrail);
            }

            return floor.Walls.Count > 0;
        }

        private static bool ReadAlertAreas(ArrayList layers, Floor floor)
        {
            foreach (Layer layer in layers)
            {
                if (string.Compare(layer.LayerName, "B_AlertArea", true) == 0)
                    ReadAlertAreas(layer, floor.AlertAreas, floor.Name);

            }

            return floor.AlertAreas.Count > 0;
        }

        private static bool ReadColumns(ArrayList layers, Floor floor)
        {
            foreach (Layer layer in layers)
            {
                if (string.Compare(layer.LayerName, "B_Col_S", true) == 0)
                    ReadColumns(layer, floor.Columns, floor.Name);
            }

            return floor.Columns.Count > 0;
        }

        private static bool ReadWalls(Layer layer, List<Wall> walls, string strFloorName, Wall.WallType wallType)
        {
            double dTolerance = 0.1;

            foreach (Shape shape in layer.Shapes)
            {
                if (CheckFloor(shape, strFloorName))
                {
                    if (shape is Line)
                    {
                        Line line = (Line)shape;

                        if (line.Begin.GetDistance(line.End) < dTolerance)
                            continue;

                        // 같은 타입, 같은 선형의 벽체가 이미 존재하면 중복으로 추가하지 않는다.
                        long nBegin = Wall.VertexToLong(line.Begin);
                        long nEnd = Wall.VertexToLong(line.End);
                        bool isSame = false;

                        foreach (Wall _wall in walls)
                        {
                            if (_wall.IsSame(line.Begin, line.End, nBegin, nEnd, wallType))
                            {
                                isSame = true;
                                break;
                            }
                        }

                        if (isSame)
                            continue;

                        Wall wall = new Wall();
                        wall.Begin = line.Begin;
                        wall.End = line.End;
                        wall.Type = wallType;

                        walls.Add(wall);
                    }
                    else if (shape is Arc)
                    {
                        Arc arc = (Arc)shape;

                        if (arc.Radius < dTolerance || System.Math.Abs(arc.ArcAngle) < dTolerance)
                            continue;

                        double dBeginAngle = InflateAngle(arc.BeginAngle);
                        double dArcAngle = arc.ArcAngle;
                        bool isClockwise = true;

                        if (dArcAngle < 0.0)
                        {
                            dArcAngle = -dArcAngle;
                            isClockwise = false;
                        }

                        Arc2D arc2D = new Arc2D(arc.Center, arc.Radius, UnE.Geometry.Math.DegToRad(dBeginAngle), UnE.Geometry.Math.DegToRad(dArcAngle), isClockwise);

                        // 같은 타입, 같은 선형의 벽체가 이미 존재하면 중복으로 추가하지 않는다.
                        long nCenter = Wall.VertexToLong(arc.Center);
                        bool isSame = false;

                        foreach (Wall _wall in walls)
                        {
                            if (_wall.IsSame(arc2D, wallType))
                            {
                                isSame = true;
                                break;
                            }
                        }

                        if (isSame)
                            continue;

                        Wall wall = new Wall();
                        wall.Arc = arc2D;
                        wall.Type = wallType;

                        walls.Add(wall);
                    }
                }
            }

            return true;
        }

        private static double InflateAngle(double degree)
        {
            if (degree >= 360.0)
            {
                int nCount = (int)(degree / 360);
                degree = degree - nCount * 360;
            }
            else if (degree < 0.0)
            {
                int nCount = (int)(degree / 360);
                degree = degree - (nCount - 1) * 360;

                if (degree >= 360.0)
                    degree -= 360.0;
            }

            return degree;
        }

        private static bool CheckFloor(Shape shape, string strFloorName)
        {
            Block block = shape.GetBlock();

            if (block == null)
                return false;

            return block.Name == strFloorName;
        }

        private static int ReadWindows(ArrayList layers, Floor floor)
        {
            foreach (Layer layer in layers)
            {
                if (string.Compare(layer.LayerName, "B_Win", true) == 0 || string.Compare(layer.LayerName, "0_Win", true) == 0)
                    return ReadWindows(layer, floor.Walls, floor.Name);
            }

            return 0;
        }

        private static int ReadWindows(Layer layer, List<Wall> walls, string strFloorName)
        {
            int nWindowCount = 0;

            foreach (Shape shape in layer.Shapes)
            {
                if (CheckFloor(shape, strFloorName))
                {
                    if (shape is Line)
                    {
                        Line line = (Line)shape;
                        Wall wall = FindWall(line.Begin, line.End, walls);

                        if (wall == null)
                        {
                            string strError = string.Format("Error, Unknown Window Line : ({0}, {1}) ~ ({2}, {3})", line.Begin.x, line.Begin.y, line.End.x, line.End.y);
                            System.Diagnostics.Trace.WriteLine(strError);
                            continue;
                        }

                        Window window = new Window();

                        window.Width = line.Begin.GetDistance(line.End);
                        window.X = (line.Begin.x + line.End.x) / 2;
                        window.Y = (line.Begin.y + line.End.y) / 2;

                        wall.Windows.Add(window);
                        nWindowCount++;
                    }
                }
            }

            return nWindowCount;
        }

        private static int ReadDoors(ArrayList layers, Floor floor)
        {
            int nDoorCount = 0;

            foreach (Layer layer in layers)
            {
                if (string.Compare(layer.LayerName, "B_Door_P", true) == 0 || string.Compare(layer.LayerName, "0_Door_P", true) == 0)
                    nDoorCount += ReadDoors(layer, floor.Walls, floor.Name, Door.DoorType.Hinged);
                else if (string.Compare(layer.LayerName, "B_Door_S", true) == 0 || string.Compare(layer.LayerName, "0_Door_S", true) == 0)
                    nDoorCount += ReadDoors(layer, floor.Walls, floor.Name, Door.DoorType.Sliding);
                else if (string.Compare(layer.LayerName, "B_Door_T", true) == 0 || string.Compare(layer.LayerName, "0_Door_T", true) == 0)
                    nDoorCount += ReadDoors(layer, floor.Walls, floor.Name, Door.DoorType.Hinged);
                else if (string.Compare(layer.LayerName, "B_EscalDoor", true) == 0 || string.Compare(layer.LayerName, "0_EscalDoor", true) == 0)
                    nDoorCount += ReadDoors(layer, floor.Walls, floor.Name, Door.DoorType.Sliding);
                else if (string.Compare(layer.LayerName, "B_StairDoor", true) == 0 || string.Compare(layer.LayerName, "0_StairDoor", true) == 0)
                    nDoorCount += ReadDoors(layer, floor.Walls, floor.Name, Door.DoorType.Sliding);
                else if (string.Compare(layer.LayerName, "B_ElevDoor", true) == 0 || string.Compare(layer.LayerName, "0_ElevDoor", true) == 0)
                    nDoorCount += ReadDoors(layer, floor.Walls, floor.Name, Door.DoorType.Sliding);
            }

            return nDoorCount;
        }

        private static int ReadDoors(Layer layer, List<Wall> walls, string strFloorName, Door.DoorType doorType)
        {
            int nDoorCount = 0;

            foreach (Shape shape in layer.Shapes)
            {
                if (CheckFloor(shape, strFloorName))
                {
                    if (shape is Line && doorType == Door.DoorType.Sliding)
                    {
                        Line line = (Line)shape;

                        if (ReadSlidingDoors(line.Begin, line.End, walls))
                            nDoorCount++;
                    }
                    else if (shape is PolyLine)
                    {
                        PolyLine pLine = (PolyLine)shape;
                        int nVertexCount = pLine.GetVertexSize();

                        if (doorType == Door.DoorType.Sliding)
                        {
                            if (nVertexCount < 2)
                                continue;

                            System.Drawing.PointF ptBegin = pLine.GetVertex(0);
                            System.Drawing.PointF ptEnd = pLine.GetVertex(nVertexCount - 1);
                            UnE.Geometry.Vertex2D vBegin = new UnE.Geometry.Vertex2D(ptBegin.X, ptBegin.Y);
                            UnE.Geometry.Vertex2D vEnd = new UnE.Geometry.Vertex2D(ptEnd.X, ptEnd.Y);

                            if (ReadSlidingDoors(vBegin, vEnd, walls))
                                nDoorCount++;
                        }
                        else if (doorType == Door.DoorType.Hinged)
                        {
                            if (ReadHingeDoors(pLine, walls))
                                nDoorCount++;
                        }
                        /*else if (doorType == Door.DoorType.DoubleHinged)
                        {
                            if (ReadHingeDoors(pLine, walls))
                                nDoorCount++;
                        }*/
                    }
                }
            }

            return nDoorCount;
        }

        private static bool ReadHingeDoors(PolyLine pLine, List<Wall> walls)
        {
            int nVertexCount = pLine.GetVertexSize();

            if (nVertexCount < 3)
                return false;

            UnE.Geometry.Vertex2D vHinge1, vHinge2;
            UnE.Geometry.Vertex2D vDoorBegin, vDoorEnd;
            bool isSwing = false;
            Wall wall = FindWall(pLine, nVertexCount, walls, out isSwing, out vDoorBegin, out vDoorEnd, out vHinge1, out vHinge2);

            if (wall == null)
                return false;

            Door door = new Door();

            if (isSwing)
            {
                if (vHinge2 == null)
                    door.Type = Door.DoorType.Hinged2;
                else
                    door.Type = Door.DoorType.DoubleHinged2;
            }
            else
            {
                if (vHinge2 == null)
                    door.Type = Door.DoorType.Hinged;
                else
                    door.Type = Door.DoorType.DoubleHinged;
            }

            door.Width = vDoorBegin.GetDistance(vDoorEnd);
            door.X = (vDoorBegin.x + vDoorEnd.x) / 2;
            door.Y = (vDoorBegin.y + vDoorEnd.y) / 2;
            door.Hinge1 = vHinge1;
            door.Hinge2 = vHinge2;

            //if (doorType == Door.DoorType.Hinged)
            //    door.Direction = GetDoorDirection(wall);

            wall.Doors.Add(door);
            return true;
        }

        private static bool ReadSlidingDoors(UnE.Geometry.Vertex2D vBegin, UnE.Geometry.Vertex2D vEnd, List<Wall> walls)
        {
            Wall wall = FindWall(vBegin, vEnd, walls);

            if (wall == null)
            {
                string strError = string.Format("Error, Unknown Door Line : ({0}, {1}) ~ ({2}, {3})", vBegin.x, vBegin.y, vEnd.x, vEnd.y);
                System.Diagnostics.Trace.WriteLine(strError);
                return false;
            }

            Door door = new Door();

            door.Type = Door.DoorType.Sliding;
            door.Width = vBegin.GetDistance(vEnd);
            door.X = (vBegin.x + vEnd.x) / 2;
            door.Y = (vBegin.y + vEnd.y) / 2;

            //if (doorType == Door.DoorType.Hinged)
            //    door.Direction = GetDoorDirection(wall);

            wall.Doors.Add(door);
            return true;
        }

        // 미닫이 문에 대한 Direction(Degree)
        private static double GetDoorDirection(Wall wall)
        {
            UnE.Geometry.Vertex2D vRight = UnE.Geometry.Math.GetRightVertex(wall.End, wall.Begin, 100.0);

            UnE.Geometry.Vertex2D vCenter = new UnE.Geometry.Vertex2D(0.0, 0.0);
            UnE.Geometry.Vertex2D vDir = vRight - wall.End;

            if (vDir.y == 0.0)
                return 0.0;

            UnE.Geometry.Vertex2D vR = new UnE.Geometry.Vertex2D(100.0, 0.0);

            double theta = UnE.Geometry.Math.GetAngle(vDir, vCenter, vR);

            if (vDir.y < 0)
                theta = UnE.Geometry.Math._2PI() - theta;

            return UnE.Geometry.Math.RadToDeg(theta);
        }

        private static int ReadColumns(Layer layer, List<Column> columns, string strFloorName)
        {
            int nColumnCount = 0;

            foreach (Shape shape in layer.Shapes)
            {
                if (CheckFloor(shape, strFloorName))
                {
                    if (shape is Arc)
                    {
                        Arc arc = (Arc)shape;

                        if (arc.IsCircle)
                        {
                            CircleColumn column = new CircleColumn();
                            column.Center = arc.Center;
                            column.Radius = arc.Radius;
                            columns.Add(column);

                            nColumnCount++;
                        }
                    }
                    else if (shape is PolyLine)
                    {
                        PolyLine pLine = (PolyLine)shape;
                        int nVertexCount = pLine.GetVertexSize();

                        System.Drawing.PointF ptFirst = pLine.GetVertex(0);
                        System.Drawing.PointF ptLast = pLine.GetVertex(nVertexCount - 1);
                        Vertex2D vFirst = new Vertex2D(ptFirst.X, ptFirst.Y);
                        Vertex2D vLast = new Vertex2D(ptLast.X, ptLast.Y);

                        bool isSame = vFirst.GetDistance(vLast) <= UnE.Geometry.Math.HALF_TOLERANCE();

                        // 사각형일 경우
                        if ((nVertexCount == 4 && isSame == false) || (nVertexCount == 5 && isSame))
                        {
                            RectColumn column = new RectColumn();

                            System.Drawing.PointF ptSecond = pLine.GetVertex(1);
                            System.Drawing.PointF ptThird = pLine.GetVertex(2);

                            column.TopLeft = vFirst;
                            column.BottomLeft = new Vertex2D(ptSecond.X, ptSecond.Y);
                            column.BottomRight = new Vertex2D(ptThird.X, ptThird.Y);

                            columns.Add(column);

                            nColumnCount++;
                        }
                    }
                }
            }

            return nColumnCount;
        }

        private static int ReadAlertAreas(Layer layer, List<AlertArea> alertAreas, string strFloorName)
        {
            int nAlertAreaCount = 0;
            double dTolerance = 0.1;

            foreach (Shape shape in layer.Shapes)
            {
                if (CheckFloor(shape, strFloorName))
                {
                    // Arc, EArc도 추후 구현 필요
                    if (shape is PolyLine)
                    {
                        AlertArea area = new AlertArea();

                        PolyLine pLine = (PolyLine)shape;
                        int nVertexCount = pLine.GetVertexSize();

                        System.Drawing.PointF ptFirst = pLine.GetVertex(0);
                        System.Drawing.PointF ptLast = pLine.GetVertex(nVertexCount - 1);
                        Vertex2D vFirst = new Vertex2D(ptFirst.X, ptFirst.Y);
                        Vertex2D vLast = new Vertex2D(ptLast.X, ptLast.Y);

                        // 시작점과 끝점이 동일하다면
                        if (vFirst.GetDistance(vLast) <= UnE.Geometry.Math.HALF_TOLERANCE())
                            nVertexCount--;

                        for (int i = 0; i < nVertexCount; i++)
                        {
                            System.Drawing.PointF point = pLine.GetVertex(i);
                            System.Drawing.PointF ptNext = pLine.GetVertex(i + 1);
                            Vertex2D vPoint = new Vertex2D(point.X, point.Y);
                            Vertex2D vNext = new Vertex2D(ptNext.X, ptNext.Y);

                            PathItem item = new PathItem();
                            item.SetLine(new Line2D(vPoint, vNext));

                            area.AddLineBoundary(item);
                            nAlertAreaCount++;

                            // PolyLine에 중복좌표가 있는 경우
                            if (vFirst.GetDistance(vNext) <= UnE.Geometry.Math.HALF_TOLERANCE())
                                break;
                        }

                        if (area.Boundary.Count != 0)
                            alertAreas.Add(area);
                    }
                    else if (shape is Arc)
                    {
                        //AlertArea area = new AlertArea();
                        //Arc arc = (Arc)shape;

                        //if (arc.Radius < dTolerance || System.Math.Abs(arc.ArcAngle) < dTolerance)
                        //    continue;

                        //double dBeginAngle = InflateAngle(arc.BeginAngle);
                        //double dArcAngle = arc.ArcAngle;
                        //bool isClockwise = true;

                        //if (dArcAngle < 0.0)
                        //{
                        //    dArcAngle = -dArcAngle;
                        //    isClockwise = false;
                        //}

                        //Arc2D arc2D = new Arc2D(arc.Center, arc.Radius, UnE.Geometry.Math.DegToRad(dBeginAngle), UnE.Geometry.Math.DegToRad(dArcAngle), isClockwise);
                        
                        //PathItem item = new PathItem();
                        //item.SetArc(arc2D);

                        //area.AddLineBoundary(item);
                        //nAlertAreaCount++;

                        //alertAreas.Add(area);
                    }
                }
            }

            return nAlertAreaCount;
        }

        // 같은층 내에서는 공간과 문의 연결관계를 생성한다.
        // 연결된 층의 경우 계단 또는 엘리베이터나 에스컬레이터를 통한 연결관계도 생성한다.
        public static Dictionary<Space, Topology.Node> MakeTopology(Floor floor, Floor belowFloor, Dictionary<Space, Topology.Node> belowSpaceTopologyNodes)
        {
            Dictionary<Space, List<Door>> dicSpaceDoorLink = new Dictionary<Space, List<Door>>();
            Dictionary<Space, Topology.Node> dicSpaceTopologyNodes = new Dictionary<Space, Topology.Node>();
            Dictionary<Door, Topology.Node> dicDoorTopologyNodes = new Dictionary<Door, Topology.Node>();

            foreach (Wall wall in floor.Walls)
            {
                AddDoorLink(wall.Doors, dicDoorTopologyNodes);
                AddSpaceLink(wall.LinkedSpace1, wall.Doors, dicSpaceDoorLink, dicSpaceTopologyNodes, dicDoorTopologyNodes);
                AddSpaceLink(wall.LinkedSpace2, wall.Doors, dicSpaceDoorLink, dicSpaceTopologyNodes, dicDoorTopologyNodes);
            }

            while (dicSpaceDoorLink.Count > 0)
            {
                Space space = dicSpaceDoorLink.ElementAt(0).Key;

                Topology topology = new Topology();
                floor.Topologies.Add(topology);

                SetTopology(topology, space, dicSpaceDoorLink, dicSpaceTopologyNodes, dicDoorTopologyNodes);
            }

            // 층간의 연결노드 생성
            if (belowFloor != null)
            {
                List<Space> belowElevatorRooms = belowFloor.GetSpecialSpaces(Space.SpaceType.ElevatorRoom);
                List<Space> elevatorRooms = floor.GetSpecialSpaces(Space.SpaceType.ElevatorRoom);

                if (belowElevatorRooms.Count > 0 && elevatorRooms.Count > 0)
                    MakeTopology(elevatorRooms, belowElevatorRooms, dicSpaceTopologyNodes, belowSpaceTopologyNodes);

                List<Space.SpaceType> types = new List<Space.SpaceType>();
                types.Add(Space.SpaceType.StairRoom);
                types.Add(Space.SpaceType.EscalatorRoom);

                List<Space> belowRooms = belowFloor.GetSpecialSpaces(types);
                List<Space> rooms = floor.GetSpecialSpaces(types);

                if (belowRooms.Count > 0 && rooms.Count > 0)
                    MakeTopology(rooms, belowRooms, dicSpaceTopologyNodes, belowSpaceTopologyNodes);
            }

            return dicSpaceTopologyNodes;
        }

        // 인접한 층간에 연결된 공간들이 있는지 검사한다.
        private static void MakeTopology(List<Space> spaces, List<Space> belowSpaces, Dictionary<Space, Topology.Node> dicSpaceTopologyNodes, Dictionary<Space, Topology.Node> belowSpaceTopologyNodes)
        {
            Dictionary<Space, Polygon> dicSpacePolygon = new Dictionary<Space, Polygon>();
            Dictionary<Space, Polygon> dicBelowSpacePolygon = new Dictionary<Space, Polygon>();
            Dictionary<Space, double> dicBelowSpaceArea = new Dictionary<Space, double>();

            foreach (Space space in belowSpaces)
            {
                Polygon polygon = new Polygon();

                foreach (Wall wall in space.Walls)
                {
                    polygon.AddVertex(wall.Begin);
                }

                dicBelowSpacePolygon[space] = polygon;

                double area = polygon.GetArea();
                dicBelowSpaceArea[space] = area;
            }

            Polygon belowPolygon = null;
            double dBelowArea = 0.0;
            List<List<Vertex2D>> results = new List<List<Vertex2D>>();
            ClipperLib.Clipper clipper = new ClipperLib.Clipper();

            Topology.Node spaceNode = null;
            Topology.Node belowNode = null;

            foreach (Space space in spaces)
            {
                Polygon polygon = new Polygon();

                foreach (Wall wall in space.Walls)
                {
                    polygon.AddVertex(wall.Begin);
                }

                double dArea = polygon.GetArea();

                if (dicSpaceTopologyNodes.TryGetValue(space, out spaceNode) == false)
                    continue;

                foreach (Space belowSpace in belowSpaces)
                {
                    if (dicBelowSpacePolygon.TryGetValue(belowSpace, out belowPolygon) == false)
                        continue;

                    if (dicBelowSpaceArea.TryGetValue(belowSpace, out dBelowArea) == false)
                        continue;

                    if (belowSpaceTopologyNodes.TryGetValue(belowSpace, out belowNode) == false)
                        continue;

                    clipper.Clear();
                    results.Clear();

                    clipper.AddPolygon(polygon.GetVertexList(), ClipperLib.PolyType.ptClip);
                    clipper.AddPolygon(belowPolygon.GetVertexList(), ClipperLib.PolyType.ptSubject);

                    if (clipper.Execute(ClipperLib.ClipType.ctIntersection, results) && results.Count > 0)
                    {
                        // 교차되는 영역이 적어도 원본 영역의 10분의 1 이상이어야 한다.
                        double dResultArea = GetPolygonArea(results[0]) * 10;

                        if (dResultArea >= dArea || dResultArea >= dBelowArea)
                        {
                            if (spaceNode.LinkedNodes.Contains(belowNode) == false)
                            {
                                spaceNode.LinkedNodes.Add(belowNode);
                                belowNode.LinkedNodes.Add(spaceNode);
                            }
                        }
                    }
                }
            }
        }

        private static double GetPolygonArea(List<Vertex2D> vertices)
        {
            Polygon polygon = new Polygon();

            foreach (Vertex2D vertex in vertices)
            {
                polygon.AddVertex(vertex);
            }

            return polygon.GetArea();
        }

        private static void SetTopology(Topology topology, Space space, Dictionary<Space, List<Door>> dicSpaceDoorLink, Dictionary<Space, Topology.Node> dicSpaceTopologyNodes, Dictionary<Door, Topology.Node> dicDoorTopologyNodes)
        {
            List<Door> doors = null;

            if (dicSpaceDoorLink.TryGetValue(space, out doors) == false)
                return;

            dicSpaceDoorLink.Remove(space);

            Topology.Node spaceNode = null, doorNode = null;

            if (dicSpaceTopologyNodes.TryGetValue(space, out spaceNode) == false)
                return;

            if (topology.Nodes.Contains(spaceNode) == false)
                topology.Nodes.Add(spaceNode);

            foreach (Door door in doors)
            {
                if (dicDoorTopologyNodes.TryGetValue(door, out doorNode) == false)
                    continue;

                if (topology.Nodes.Contains(doorNode) == false)
                {
                    topology.Nodes.Add(doorNode);

                    foreach (Topology.Node node in doorNode.LinkedNodes)
                    {
                        if (node.Owner != null && node.Owner is Space)
                        {
                            SetTopology(topology, (Space)node.Owner, dicSpaceDoorLink, dicSpaceTopologyNodes, dicDoorTopologyNodes);
                        }
                    }
                }
            }
        }

        // 문 Node는 무시하고 공간들간의 연결관계만 생성한다.
        /*private static void MakeTopology(Floor floor)
        {
            Dictionary<Space, List<Space>> dicSpaceLink = new Dictionary<Space, List<Space>>();
            Dictionary<Space, Topology.Node> dicTopologyNodes = new Dictionary<Space, Topology.Node>();

            foreach (Wall wall in floor.Walls)
            {
                // 문이 있는 곳만 연결 링크를 만든다.
                if (wall.LinkedSpace1 != null && wall.LinkedSpace2 != null)
                {
                    if (wall.Doors.Count > 0)
                    {
                        AddSpaceLink(wall.LinkedSpace1, wall.LinkedSpace2, dicSpaceLink, dicTopologyNodes);
                        continue;
                    }
                }

                wall.LinkedSpace1 = null;
                wall.LinkedSpace2 = null;
            }

            foreach (KeyValuePair<Space, List<Space>> pair in dicSpaceLink)
            {
                Topology.Node node = null;

                if (dicTopologyNodes.TryGetValue(pair.Key, out node) == false)
                {
                    string strError = string.Format("Error, Unknown Space");
                    System.Diagnostics.Trace.WriteLine(strError);
                    continue;
                }

                foreach (Space space in pair.Value)
                {
                    Topology.Node link = null;

                    if (dicTopologyNodes.TryGetValue(space, out link) == false)
                    {
                        string strError = string.Format("Error, Unknown Space");
                        System.Diagnostics.Trace.WriteLine(strError);
                        continue;
                    }

                    node.LinkedNodes.Add(link);
                }
            }

            while (dicSpaceLink.Count > 0)
            {
                List<Space> spaces = new List<Space>();

                Space space = dicSpaceLink.ElementAt(0).Key;
                AddLinkedSpace(space, spaces, dicSpaceLink);

                Topology topology = new Topology();

                foreach (Space _space in spaces)
                {
                    Topology.Node node = null;

                    if (dicTopologyNodes.TryGetValue(_space, out node) == false)
                    {
                        string strError = string.Format("Error, Unknown Space");
                        System.Diagnostics.Trace.WriteLine(strError);
                    }
                    else
                        topology.Nodes.Add(node);

                    dicSpaceLink.Remove(_space);
                }

                floor.Topologies.Add(topology);
            }
        }*/

        private static void AddDoorLink(List<Door> doors, Dictionary<Door, Topology.Node> dicDoorTopologyNodes)
        {
            foreach (Door door in doors)
            {
                Topology.Node node = new Topology.Node();

                node.Owner = door;
                node.Type = Topology.Node.NodeType.Door;
                node.X = door.X;
                node.Y = door.Y;

                dicDoorTopologyNodes[door] = node;
            }
        }

        private static void AddSpaceLink(Space space, List<Door> doors, Dictionary<Space, List<Door>> dicSpaceDoorLink, Dictionary<Space, Topology.Node> dicSpaceTopologyNodes, Dictionary<Door, Topology.Node> dicDoorTopologyNodes)
        {
            if (space == null)
                return;

            Topology.Node spaceNode = null, doorNode = null;

            if (dicSpaceTopologyNodes.TryGetValue(space, out spaceNode) == false)
            {
                spaceNode = SpaceToTopologyNode(space);

                spaceNode.Owner = space;
                spaceNode.Type = Topology.Node.NodeType.Space;
                dicSpaceTopologyNodes[space] = spaceNode;
            }

            List<Door> spaceDoors = null;

            if (dicSpaceDoorLink.TryGetValue(space, out spaceDoors) == false)
            {
                spaceDoors = new List<Door>();
                dicSpaceDoorLink[space] = spaceDoors;
            }

            foreach (Door door in doors)
            {
                if (dicDoorTopologyNodes.TryGetValue(door, out doorNode))
                {
                    if (spaceDoors.Contains(door) == false)
                    {
                        spaceDoors.Add(door);
                        spaceNode.LinkedNodes.Add(doorNode);
                        doorNode.LinkedNodes.Add(spaceNode);
                    }
                }
            }
        }

        private static void AddLinkedSpace(Space space, List<Space> spaces, Dictionary<Space, List<Space>> dicSpaceLink)
        {
            if (spaces.Contains(space))
                return;

            spaces.Add(space);

            List<Space> spaceList = null;

            if (dicSpaceLink.TryGetValue(space, out spaceList))
            {
                foreach (Space _space in spaceList)
                {
                    AddLinkedSpace(_space, spaces, dicSpaceLink);
                }
            }
        }

        private static void AddSpaceLink(Space space1, Space space2, Dictionary<Space, List<Space>> dicSpaceLink, Dictionary<Space, Topology.Node> dicTopologyNodes)
        {
            List<Space> spaceList = null;

            if (dicSpaceLink.TryGetValue(space1, out spaceList) == false)
            {
                spaceList = new List<Space>();
                dicSpaceLink[space1] = spaceList;
            }

            if (spaceList.Contains(space2) == false)
                spaceList.Add(space2);

            if (dicSpaceLink.TryGetValue(space2, out spaceList) == false)
            {
                spaceList = new List<Space>();
                dicSpaceLink[space2] = spaceList;
            }

            if (spaceList.Contains(space1) == false)
                spaceList.Add(space1);

            if (dicTopologyNodes.ContainsKey(space1) == false)
            {
                Topology.Node node = SpaceToTopologyNode(space1);
                dicTopologyNodes[space1] = node;
            }

            if (dicTopologyNodes.ContainsKey(space2) == false)
            {
                Topology.Node node = SpaceToTopologyNode(space2);
                dicTopologyNodes[space2] = node;
            }
        }

        private static Topology.Node SpaceToTopologyNode(Space space)
        {
            Polygon polygon = space.GetPolygon();

            if (polygon == null)
                return null;

            /*UnE.Geometry.Polygon polygon = new UnE.Geometry.Polygon();
            Vertex2D vPrev = null;
            Wall firstWall = null;

            foreach (Wall wall in space.Walls)
            {
                if (firstWall == null)
                {
                    firstWall = wall;
                    continue;
                }
                else
                {
                    Vertex2D vBegin = wall.GetBeginVertex();
                    Vertex2D vEnd = wall.GetEndVertex();

                    if (polygon.GetVertexCount() == 0)
                    {
                        Vertex2D vPrevBegin = firstWall.GetBeginVertex();
                        Vertex2D vPrevEnd = firstWall.GetEndVertex();
                        int nVertexResult = GetNearestWallVertex(vPrevBegin, vPrevEnd, vBegin, vEnd);

                        if (nVertexResult == 0) // EndToBegin
                        {
                            polygon.AddVertex(vPrevBegin);
                            polygon.AddVertex(vBegin);
                            vPrev = vEnd;
                        }
                        else if (nVertexResult == 1)    // EndToEnd
                        {
                            polygon.AddVertex(vPrevBegin);
                            polygon.AddVertex(vEnd);
                            vPrev = vBegin;
                        }
                        else if (nVertexResult == 2)    // BeginToBegin
                        {
                            polygon.AddVertex(vPrevEnd);
                            polygon.AddVertex(vBegin);
                            vPrev = vEnd;
                        }
                        else                            // BeginToEnd
                        {
                            polygon.AddVertex(vPrevEnd);
                            polygon.AddVertex(vEnd);
                            vPrev = vBegin; ;
                        }
                    }
                    else
                    {
                        double dBeginLength = vBegin.GetDistance(vPrev);

                        if (dBeginLength < UnE.Geometry.Math.HALF_TOLERANCE())
                        {
                            vPrev = vEnd;
                            polygon.AddVertex(vBegin);
                        }
                        else
                        {
                            double dEndLength = vEnd.GetDistance(vPrev);

                            if (dEndLength < UnE.Geometry.Math.HALF_TOLERANCE())
                            {
                                vPrev = vBegin;
                                polygon.AddVertex(vEnd);
                            }
                            else if (dBeginLength < dEndLength)
                            {
                                vPrev = vEnd;
                                polygon.AddVertex(vBegin);
                            }
                            else
                            {
                                vPrev = vBegin;
                                polygon.AddVertex(vEnd);
                            }
                        }
                    }
                }
            }*/

            UnE.Geometry.Vertex2D vCenter = polygon.CalcWeightCenter();

            int nResult = polygon.HitTest(vCenter);

            if (nResult == 0)
            {
                // vCenter가 Polygon 외부에 위치할 경우
                vCenter = GetInsideVertex(vCenter, polygon);
            }
            else if (nResult < 0)
            {
                // vCenter가 Polygon 경계에 위치할 경우
                vCenter = GetInsideVertexFromEdge(vCenter, polygon);
            }

            Topology.Node node = new Topology.Node();
            node.X = vCenter.x;
            node.Y = vCenter.y;

            return node;
        }

        // Return 값
        // 0 : EndToBegin
        // 1 : EndToEnd
        // 2 : BeginToBegin
        // 3 : BeginToEnd
        /*private static int GetNearestWallVertex(Vertex2D vPrevBegin, Vertex2D vPrevEnd, Vertex2D vBegin, Vertex2D vEnd)
        {
            double dEndToBegin = vPrevEnd.GetDistance(vBegin);

            if (dEndToBegin < UnE.Geometry.Math.HALF_TOLERANCE())
            {
                return 0;
            }
            else
            {
                double dEndToEnd = vPrevEnd.GetDistance(vEnd);

                if (dEndToEnd < UnE.Geometry.Math.HALF_TOLERANCE())
                {
                    return 1;
                }
                else
                {
                    double dBeginToBegin = vPrevBegin.GetDistance(vBegin);

                    if (dBeginToBegin < UnE.Geometry.Math.HALF_TOLERANCE())
                    {
                        return 2;
                    }
                    else
                    {
                        double dBeginToEnd = vPrevBegin.GetDistance(vEnd);

                        if (dBeginToEnd < UnE.Geometry.Math.HALF_TOLERANCE())
                        {
                            return 3;
                        }
                        else
                        {
                            if (dEndToBegin < dEndToEnd)
                            {
                                if (dEndToBegin < dBeginToBegin)
                                {
                                    if (dEndToBegin < dBeginToEnd)
                                        return 0;
                                    else
                                        return 3;
                                }
                                else
                                {
                                    if (dBeginToBegin < dBeginToEnd)
                                        return 2;
                                    else
                                        return 3;
                                }
                            }
                            else
                            {
                                if (dEndToEnd < dBeginToBegin)
                                {
                                    if (dEndToEnd < dBeginToEnd)
                                        return 1;
                                    else
                                        return 3;
                                }
                                else
                                {
                                    if (dBeginToBegin < dBeginToEnd)
                                        return 2;
                                    else
                                        return 3;
                                }
                            }
                        }
                    }
                }
            }

            return 0;
        }*/

        private static UnE.Geometry.Vertex2D GetInsideVertexFromEdge(UnE.Geometry.Vertex2D vEdge, UnE.Geometry.Polygon polygon)
        {
            int nVertexCount = polygon.GetVertexCount();
            UnE.Geometry.Vertex2D vPrev = polygon.GetVertex(nVertexCount - 1);

            for (int i = 0; i < nVertexCount; i++)
            {
                UnE.Geometry.Vertex2D vCurrent = polygon.GetVertex(i);
                UnE.Geometry.Line2D line = new UnE.Geometry.Line2D(vPrev, vCurrent);

                if (line.IsInclude(vEdge))
                {
                    UnE.Geometry.Vertex2D vertex = null;

                    if (IsSameVertex(vEdge, vPrev))
                        vertex = UnE.Geometry.Math.GetRightVertex(vEdge, vCurrent, 0.5);
                    else
                        vertex = UnE.Geometry.Math.GetRightVertex(vEdge, vPrev, 0.5);

                    int nResult = polygon.HitTest(vertex);

                    if (nResult == 0)
                        return GetInsideVertex(vertex, vEdge, polygon);
                    else if (nResult > 0)
                    {
                        UnE.Geometry.Vertex2D vOutside = vEdge * 2 - vertex;
                        return GetInsideVertex(vertex, vEdge, polygon);
                    }
                    else
                    {
                        // 계산 불가
                        return vEdge;
                    }
                }

                vPrev = vCurrent;
            }

            return vEdge;
        }

        private static UnE.Geometry.Vertex2D GetInsideVertex(UnE.Geometry.Vertex2D vOutside, UnE.Geometry.Polygon polygon)
        {
            UnE.Geometry.Vertex2D vEdge = null;
            polygon.GetDistanceNVertex(vOutside, out vEdge);

            if (vEdge == null)
            {
                // 계산 포기
                return vOutside;
            }

            return GetInsideVertex(vOutside, vEdge, polygon);
        }

        private static UnE.Geometry.Vertex2D GetInsideVertex(UnE.Geometry.Vertex2D vOutside, UnE.Geometry.Vertex2D vEdge, UnE.Geometry.Polygon polygon)
        {
            UnE.Geometry.Vertex2D vOther = vEdge * 2 - vOutside;
            UnE.Geometry.Line2D line = new UnE.Geometry.Line2D(vEdge, vOther, UnE.Geometry.Line2D.LineType.HALF_LINE_BEGIN_2_END);

            int nVertexCount = polygon.GetVertexCount();
            UnE.Geometry.Vertex2D vPrev = polygon.GetVertex(nVertexCount - 1);

            UnE.Geometry.Vertex2D v1, v2;
            UnE.Geometry.Line2D.LineType lineType;

            UnE.Geometry.Vertex2D vNear = null;
            double distance = 0.0;

            for (int i = 0; i < nVertexCount; i++)
            {
                UnE.Geometry.Vertex2D vCurrent = polygon.GetVertex(i);
                UnE.Geometry.Line2D line2 = new UnE.Geometry.Line2D(vPrev, vCurrent);

                int nResult = line2.IntersectLine(line, out v1, out v2, out lineType);

                if (nResult == 1)
                {
                    if (IsSameVertex(vEdge, v1) == false)
                    {
                        double len = vEdge.GetDistance(v1);

                        if (vNear == null || distance > len)
                        {
                            vNear = v1;
                            distance = len;
                        }
                    }
                }
                else if (nResult == 2)
                {
                    double len1 = vEdge.GetDistance(v1);
                    double len2 = vEdge.GetDistance(v2);

                    if (len1 < len2)
                    {
                        if (IsSameVertex(vEdge, v1) == false)
                        {
                            if (vNear == null || distance > len1)
                            {
                                vNear = v1;
                                distance = len1;
                            }
                        }
                    }
                    else
                    {
                        if (IsSameVertex(vEdge, v2) == false)
                        {
                            if (vNear == null || distance > len2)
                            {
                                vNear = v2;
                                distance = len2;
                            }
                        }
                    }
                }

                vPrev = vCurrent;
            }

            if (vNear != null)
                return (vEdge + vNear) / 2;

            // 계산 불가
            return vEdge;
        }
    }
}
