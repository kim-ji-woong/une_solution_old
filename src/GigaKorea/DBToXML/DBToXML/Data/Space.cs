using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;

namespace DBToXML.Data
{
    public class Space
    {
        private string m_strID = "";
        private string m_strSpaceName = "";
        private List<Wall> m_walls = new List<Wall>();
        private List<Property> m_properties = new List<Property>();

        public const string SpaceIDTag = "space";

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public string Name
        {
            get { return m_strSpaceName; }
            set { m_strSpaceName = value; }
        }

        public List<Wall> Walls
        {
            get { return m_walls; }
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public static List<Space> ReadSpace(int nLevelID, Dictionary<int, Wall> dicWalls, WebDBManager dbMgr)
        {
            Dictionary<int, Space> dicSpaces = new Dictionary<int, Space>();
            string strSQL = "Select ID, Name from Space where LevelID = " + nLevelID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return dicSpaces.Values.ToList();

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (id == null || strName == null)
                    continue;

                Space space = new Space();

                space.m_strID = SpaceIDTag + id.Data.ToString();
                space.Name = strName;

                dicSpaces[id.Data] = space;

                //List<Property> properties = Property.ReadDB(dbMgr, "SpaceProperties", "SpaceProperty", "SpaceID", id.Data);
                //space.m_properties = properties;
            }

            Dictionary<int, List<Property>> dicProperties = Property.ReadDB(dbMgr, "SpaceProperties", "SpaceProperty", "SpaceID", "LevelID = " + nLevelID.ToString());

            foreach (KeyValuePair<int, List<Property>> pair in dicProperties)
            {
                Space space;

                if (dicSpaces.TryGetValue(pair.Key, out space) == false)
                    continue;

                space.m_properties = pair.Value;
            }

            strSQL = "Select SpaceID, WallID, WallIndex from SpaceWallLink where LevelID = " + nLevelID.ToString() + " order by WallIndex";
            arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return dicSpaces.Values.ToList();

            nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-2;i+=3)
            {
                VariousData<int> spaceID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> wallID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> wallIndex = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                if (spaceID == null || wallID == null || wallIndex == null)
                    continue;

                Space space;
                Wall wall;

                if (dicSpaces.TryGetValue(spaceID.Data, out space) == false)
                    continue;

                if (dicWalls.TryGetValue(wallID.Data, out wall) == false)
                    continue;

                space.m_walls.Add(wall);
            }

            return dicSpaces.Values.ToList();
        }
    }
}
