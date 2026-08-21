using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;

namespace DBToXML.Data
{
    public class Door
    {
        // 미닫이 : 옆으로 밀고 닫는문
        // 여닫이 : 앞뒤로 밀거나 당겨서 여는 문
        // 미닫이문, 한방향 외여닫이문, 양방향 외여닫이문, 한방향 쌍여닫이문, 양방향 쌍여닫이문
        public enum DoorType { Sliding = 0, Hinged, Hinged2, DoubleHinged, DoubleHinged2 };

        public const string DoorIDTag = "door";

        private string m_strID = "";
        private double m_x = 0.0;
        private double m_y = 0.0;
        private double m_dWidth = 0.0;
        private double m_dHeight = 0.0;
        private double m_dElevation = 0.0;
        private DoorType m_type = DoorType.Hinged;
        private UnE.Geometry.Vertex2D m_vHinge1 = null;
        private UnE.Geometry.Vertex2D m_vHinge2 = null;
        private List<Property> m_properties = new List<Property>();

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public double X
        {
            get { return m_x; }
            set { m_x = value; }
        }

        public double Y
        {
            get { return m_y; }
            set { m_y = value; }
        }

        public double Width
        {
            get { return m_dWidth; }
            set { m_dWidth = value; }
        }

        public double Height
        {
            get { return m_dHeight; }
            set { m_dHeight = value; }
        }

        public double Elevation
        {
            get { return m_dElevation; }
            set { m_dElevation = value; }
        }

        public UnE.Geometry.Vertex2D Hinge1
        {
            get { return m_vHinge1; }
            set { m_vHinge1 = value; }
        }

        public UnE.Geometry.Vertex2D Hinge2
        {
            get { return m_vHinge2; }
            set { m_vHinge2 = value; }
        }

        public DoorType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public static List<Door> ReadDoors(int nLevelID, Dictionary<int, Wall> dicWalls, WebDBManager dbMgr)
        {
            string strSQL = "Select ID, WallID, X, Y, Width, Height, Elevation, DoorType, Hinge1X, Hinge1Y, Hinge2X, Hinge2Y from Door where LevelID = " + nLevelID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            Dictionary<int, Door> dicDoors = new Dictionary<int, Door>();

            if (arrResult == null)
                return dicDoors.Values.ToList();

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 11; i += 12)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> wallID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<float> x = WebDBManager.GetFloatField(arrResult[i + 2].ToString());
                VariousData<float> y = WebDBManager.GetFloatField(arrResult[i + 3].ToString());
                VariousData<float> width = WebDBManager.GetFloatField(arrResult[i + 4].ToString());
                VariousData<float> height = WebDBManager.GetFloatField(arrResult[i + 5].ToString());
                VariousData<float> elevation = WebDBManager.GetFloatField(arrResult[i + 6].ToString());
                VariousData<int> doorType = WebDBManager.GetIntField(arrResult[i + 7].ToString());
                VariousData<float> hinge1X = WebDBManager.GetFloatField(arrResult[i + 8].ToString());
                VariousData<float> hinge1Y = WebDBManager.GetFloatField(arrResult[i + 9].ToString());
                VariousData<float> hinge2X = WebDBManager.GetFloatField(arrResult[i + 10].ToString());
                VariousData<float> hinge2Y = WebDBManager.GetFloatField(arrResult[i + 11].ToString());

                if (id == null || wallID == null || x == null || y == null || width == null || height == null || elevation == null || doorType == null)
                    continue;

                Wall wall;

                if (dicWalls.TryGetValue(wallID.Data, out wall) == false)
                    continue;

                Door door = new Door();

                door.m_strID = DoorIDTag + id.Data.ToString();
                door.X = x.Data;
                door.Y = y.Data;
                door.Width = width.Data;
                door.Height = height.Data;
                door.Elevation = elevation.Data;
                door.m_type = (DoorType)doorType.Data;

                if (hinge1X != null && hinge1Y != null)
                    door.Hinge1 = new UnE.Geometry.Vertex2D(hinge1X.Data, hinge1Y.Data);

                if (hinge2X != null && hinge2Y != null)
                    door.Hinge2 = new UnE.Geometry.Vertex2D(hinge2X.Data, hinge2Y.Data);

                dicDoors[id.Data] = door;

                wall.Doors.Add(door);

                //List<Property> properties = Property.ReadDB(dbMgr, "DoorProperties", "DoorProperty", "DoorID", id.Data);
                //door.m_properties = properties;
            }

            Dictionary<int, List<Property>> dicProperties = Property.ReadDB(dbMgr, "DoorProperties", "DoorProperty", "DoorID", "LevelID = " + nLevelID.ToString());

            foreach (KeyValuePair<int, List<Property>> pair in dicProperties)
            {
                Door door;

                if (dicDoors.TryGetValue(pair.Key, out door) == false)
                    continue;

                door.m_properties = pair.Value;
            }

            return dicDoors.Values.ToList();
        }

        public static List<Door> ReadDoor(int nWallID, WebDBManager dbMgr)
        {
            string strSQL = "Select ID, X, Y, Width, Height, Elevation, DoorType, Hinge1X, Hinge1Y, Hinge2X, Hinge2Y from Door where WallID = " + nWallID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            List<Door> doors = new List<Door>();

            if (arrResult == null)
                return doors;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-10;i+=11)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<float> x = WebDBManager.GetFloatField(arrResult[i + 1].ToString());
                VariousData<float> y = WebDBManager.GetFloatField(arrResult[i + 2].ToString());
                VariousData<float> width = WebDBManager.GetFloatField(arrResult[i + 3].ToString());
                VariousData<float> height = WebDBManager.GetFloatField(arrResult[i + 4].ToString());
                VariousData<float> elevation = WebDBManager.GetFloatField(arrResult[i + 5].ToString());
                VariousData<int> doorType = WebDBManager.GetIntField(arrResult[i + 6].ToString());
                VariousData<float> hinge1X = WebDBManager.GetFloatField(arrResult[i + 7].ToString());
                VariousData<float> hinge1Y = WebDBManager.GetFloatField(arrResult[i + 8].ToString());
                VariousData<float> hinge2X = WebDBManager.GetFloatField(arrResult[i + 9].ToString());
                VariousData<float> hinge2Y = WebDBManager.GetFloatField(arrResult[i + 10].ToString());

                if (id == null || x == null || y == null || width == null || height == null || elevation == null || doorType == null)
                    continue;

                Door door = new Door();

                door.m_strID = DoorIDTag + id.Data.ToString();
                door.X = x.Data;
                door.Y = y.Data;
                door.Width = width.Data;
                door.Height = height.Data;
                door.Elevation = elevation.Data;
                door.m_type = (DoorType)doorType.Data;

                if (hinge1X != null && hinge1Y != null)
                    door.Hinge1 = new UnE.Geometry.Vertex2D(hinge1X.Data, hinge1Y.Data);

                if (hinge2X != null && hinge2Y != null)
                    door.Hinge2 = new UnE.Geometry.Vertex2D(hinge2X.Data, hinge2Y.Data);

                doors.Add(door);

                List<Property> properties = Property.ReadDB(dbMgr, "DoorProperties", "DoorProperty", "DoorID", id.Data);
                door.m_properties = properties;
            }

            return doors;
        }
    }
}
