using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using DBUtility2;
using UnE.Geometry;

namespace DBToXML.Data
{
    public class Wall
    {
        // 구조벽, 가벽, 커튼월, NoSpace
        // NoSpace는 문과 벽체만 만들고 공간 생성에는 참여하지 않는다.
        public enum WallType { Structure = 0, Fake, Partition, Handrail, NoSpace };
        public enum GridType { Line = 0, Arc, EArc };

        private string m_strID = "";
        private string m_strGridID = "";
        private double m_dThick = 0.0;
        private double m_dHeight = 0.0;
        private Vertex2D m_vBegin = null;
        private Vertex2D m_vEnd = null;
        private Line2D m_line = new Line2D();
        private Arc2D m_arc = new Arc2D();
        private EArc2D m_earc = new EArc2D();
        private WallType m_wallType = WallType.Structure;
        private Material m_material = null;
        private List<Door> m_doors = new List<Door>();
        private List<Window> m_windows = new List<Window>();
        private List<Property> m_properties = new List<Property>();

        public const string WallIDTag = "wall";
        public const string GridIDTag = "grid";

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public string GridID
        {
            get { return m_strGridID; }
        }

        public UnE.Geometry.Vertex2D Begin
        {
            get { return m_vBegin; }
        }

        public UnE.Geometry.Vertex2D End
        {
            get { return m_vEnd; }
        }

        public Line2D Line
        {
            get { return m_line; }
        }

        public Arc2D Arc
        {
            get { return m_arc; }
        }

        public EArc2D EArc
        {
            get { return m_earc; }
        }

        public WallType Type
        {
            get { return m_wallType; }
            set { m_wallType = value; }
        }

        public double Thick
        {
            get { return m_dThick; }
            set { m_dThick = value; }
        }

        public double Height
        {
            get { return m_dHeight; }
            set { m_dHeight = value; }
        }

        public Material Material
        {
            get { return m_material; }
            set { m_material = value; }
        }

        public List<Door> Doors
        {
            get { return m_doors; }
        }

        public List<Window> Windows
        {
            get { return m_windows; }
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public static Dictionary<int, Wall> ReadWall(Project project, int nLevelID, WebDBManager dbMgr)
        {
            string strSQL = "Select wall.ID, Thick, Height, ComponentID, grid.GridType, grid.BeginX, grid.BeginY, grid.EndX, grid.EndY, grid.ThirdX, grid.ThirdY, grid.BeginAngle, grid.Angle, grid.ClockWise, grid.ID ";
            strSQL += "from wall, grid where wall.GridID = grid.ID and wall.LevelID = " + nLevelID.ToString();

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            Dictionary<int, Wall> dicWalls = new Dictionary<int, Wall>();

            if (arrResult == null)
                return dicWalls;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-14;i+=15)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<float> thick = WebDBManager.GetFloatField(arrResult[i + 1].ToString());
                VariousData<float> height = WebDBManager.GetFloatField(arrResult[i + 2].ToString());
                VariousData<int> componentID = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<int> gridType = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                VariousData<float> beginX = WebDBManager.GetFloatField(arrResult[i + 5].ToString());
                VariousData<float> beginY = WebDBManager.GetFloatField(arrResult[i + 6].ToString());
                VariousData<float> endX = WebDBManager.GetFloatField(arrResult[i + 7].ToString());
                VariousData<float> endY = WebDBManager.GetFloatField(arrResult[i + 8].ToString());
                VariousData<float> thirdX = WebDBManager.GetFloatField(arrResult[i + 9].ToString());
                VariousData<float> thirdY = WebDBManager.GetFloatField(arrResult[i + 10].ToString());
                VariousData<float> beginAngle = WebDBManager.GetFloatField(arrResult[i + 11].ToString());
                VariousData<float> angle = WebDBManager.GetFloatField(arrResult[i + 12].ToString());
                VariousData<int> clockWise = WebDBManager.GetIntField(arrResult[i + 13].ToString());
                VariousData<int> gridID = WebDBManager.GetIntField(arrResult[i + 14].ToString());

                if (id == null || thick == null || height == null || componentID == null || gridType == null || beginX == null || beginY == null || endX == null || endY == null || gridID == null)
                    continue;

                Material material = project.GetMaterial(componentID.Data);

                if (material == null)
                {
                    material = Material.ReadMaterial(componentID.Data, dbMgr);

                    if (material != null)
                        project.SetMaterial(material, componentID.Data);
                    else
                        continue;
                }

                Wall wall = new Wall();

                if (gridType.Data == (int)GridType.Line)
                {
                    wall.m_line = MakeLine(beginX.Data, beginY.Data, endX.Data, endY.Data);
                    wall.m_vBegin = wall.m_line.GetVertex(true);
                    wall.m_vEnd = wall.m_line.GetVertex(false);
                }
                else if (gridType.Data == (int)GridType.Arc)
                {
                    wall.m_arc = MakeArc(beginX.Data, beginY.Data, thirdX, beginAngle, angle, clockWise);

                    if (wall.m_arc == null)
                        continue;

                    wall.m_vBegin = wall.m_arc.GetBeginVertex();
                    wall.m_vEnd = wall.m_arc.GetEndVertex();
                }
                else if (gridType.Data == (int)GridType.EArc)
                {
                    wall.m_earc = MakeEArc(beginX.Data, beginY.Data, endX.Data, endY.Data, thirdX, thirdY, beginAngle, angle, clockWise);

                    if (wall.m_earc == null)
                        continue;

                    wall.m_vBegin = wall.m_earc.GetBeginVertex();
                    wall.m_vEnd = wall.m_earc.GetEndVertex();
                }
                else
                    continue;

                wall.m_strID = WallIDTag + id.Data.ToString();
                wall.m_strGridID = GridIDTag + gridID.Data.ToString();
                wall.m_dThick = thick.Data;
                wall.m_dHeight = height.Data;
                wall.m_material = material;

                dicWalls[id.Data] = wall;

                //wall.m_doors = Door.ReadDoor(id.Data, dbMgr);
                //wall.m_windows = Window.ReadWindow(id.Data, dbMgr);

                //List<Property> properties = Property.ReadDB(dbMgr, "WallProperties", "WallProperty", "WallID", id.Data);
                //wall.m_properties = properties;
            }

            Door.ReadDoors(nLevelID, dicWalls, dbMgr);
            Window.ReadWindows(nLevelID, dicWalls, dbMgr);

            Dictionary<int, List<Property>> dicProperties = Property.ReadDB(dbMgr, "WallProperties", "WallProperty", "WallID", "LevelID = " + nLevelID.ToString());

            foreach (KeyValuePair<int, List<Property>> pair in dicProperties)
            {
                Wall wall;

                if (dicWalls.TryGetValue(pair.Key, out wall) == false)
                    continue;

                wall.m_properties = pair.Value;
            }

            return dicWalls;
        }

        private static Line2D MakeLine(float fBeginX, float fBeginY, float fEndX, float fEndY)
        {
            Vertex2D vBegin = new Vertex2D(fBeginX, fBeginY);
            Vertex2D vEnd = new Vertex2D(fEndX, fEndY);
            return new Line2D(vBegin, vEnd);
        }

        private static Arc2D MakeArc(float fCenterX, float fCenterY, VariousData<float> radius, VariousData<float> beginAngle, VariousData<float> angle, VariousData<int> clockWise)
        {
            if (radius == null || beginAngle == null || angle == null || clockWise == null)
                return null;

            Vertex2D vCenter = new Vertex2D(fCenterX, fCenterY);
            return new Arc2D(vCenter, radius.Data, beginAngle.Data, angle.Data, clockWise.Data == 1);
        }

        private static EArc2D MakeEArc(float fTLX, float fTLY, float fBLX, float fBLY, VariousData<float> brX, VariousData<float> brY, VariousData<float> beginAngle, VariousData<float> angle, VariousData<int> clockWise)
        {
            if (brX == null || brY == null || beginAngle == null || angle == null || clockWise == null)
                return null;

            Vertex2D vTL = new Vertex2D(fTLX, fTLY);
            Vertex2D vBL = new Vertex2D(fBLX, fBLY);
            Vertex2D vBR = new Vertex2D(brX.Data, brY.Data);
            return new EArc2D(vTL, vBL, vBR, beginAngle.Data, angle.Data, clockWise.Data == 1);
        }
    }
}
