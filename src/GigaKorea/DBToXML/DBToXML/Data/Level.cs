using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;

namespace DBToXML.Data
{
    public class Level
    {
        private string m_strID = "";
        private string m_strFloorName = "";
        private double m_dElevation = 0.0;
        private List<Wall> m_walls = new List<Wall>();
        private List<Space> m_spaces = new List<Space>();
        private List<Topology> m_topologies = new List<Topology>();
        private List<POI> m_pois = new List<POI>();
        private List<POIWire> m_wires = new List<POIWire>();
        private List<Property> m_properties = new List<Property>();

        public const string LevelIDTag = "level";

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public string Name
        {
            get { return m_strFloorName; }
            set { m_strFloorName = value; }
        }

        public double Elevation
        {
            get { return m_dElevation; }
            set { m_dElevation = value; }
        }

        public List<Wall> Walls
        {
            get { return m_walls; }
        }

        public List<Space> Spaces
        {
            get { return m_spaces; }
        }

        public List<Topology> Topologies
        {
            get { return m_topologies; }
        }

        public List<POI> POIs
        {
            get { return m_pois; }
        }

        public List<POIWire> Wires
        {
            get { return m_wires; }
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public static List<Level> ReadLevel(Project project, WebDBManager dbMgr)
        {
            List<Level> levels = new List<Level>();

            string strSQL = "Select ID, Name, Elevation from Level where ProjectID = " + project.ID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return levels;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-2;i+=3)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<float> elevation = WebDBManager.GetFloatField(arrResult[i + 2].ToString());

                if (id == null || strName == null || elevation == null)
                    continue;

                Level level = new Level();

                level.m_strID = LevelIDTag + id.Data.ToString();
                level.m_strFloorName = strName;
                level.m_dElevation = elevation.Data;

                levels.Add(level);

                Dictionary<int, Wall> dicWalls = Wall.ReadWall(project, id.Data, dbMgr);
                List<Space> spaces = Space.ReadSpace(id.Data, dicWalls, dbMgr);
                List<Topology> topologies = Topology.ReadTopology(id.Data, dbMgr);
                Dictionary<int, POI> dicPois = POI.ReadPOI(id.Data, project.POITypes, dbMgr);
                List<POIWire> wires = POIWire.ReadPOIWire(id.Data, dicPois, project.POITypes, dbMgr);

                level.m_walls = dicWalls.Values.ToList(); ;
                level.m_spaces = spaces;
                level.m_topologies = topologies;
                level.m_pois = dicPois.Values.ToList();
                level.m_wires = wires;

                List<Property> properties = Property.ReadDB(dbMgr, "LevelProperties", "LevelProperty", "LevelID", id.Data);
                level.m_properties = properties;
            }

            return levels;
        }
    }
}
