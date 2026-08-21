using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Data.SqlClient;
using BIMViewer.DB;
using UnE.Geometry;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace BIMViewer.BIM
{
    public class BIMManager
    {
        private List<Project> m_openedProjects = new List<Project>();
        private string m_strErrorMessage = "";
        private Dictionary<int, POIType> m_dicPOITypes = new Dictionary<int, POIType>();
        private List<Shapes.POITypeProperty> m_poiTypePropertyList = new List<Shapes.POITypeProperty>();
        private bool m_initPOITypes = false;

        public string ErrorMessage
        {
            get { return m_strErrorMessage; }
        }

        public Dictionary<int, POIType> POITypes
        {
            get { return m_dicPOITypes; }
        }

        //ym. Delete Project
        public void DeleteProject(Project project)
        {
            if (m_openedProjects.Contains(project))
                m_openedProjects.Remove(project);
        }

        public Project GetProject(string strXMLFile)
        {
            XMLManager mgr = new XMLManager();
            
            if (File.Exists(strXMLFile) == false)
                return null;
            
            Project project = mgr.ReadProject(strXMLFile, m_dicPOITypes);

            if (project != null)
                project.LocalFilePath = strXMLFile;

            return project;
        }

        public List<Project> GetProjectList(string strXMLFolder)
        {
            XMLManager mgr = new XMLManager();
            List<Project> projects = new List<Project>();

            if (Directory.Exists(strXMLFolder) == false)
                return projects;

            string[] files = Directory.GetFiles(strXMLFolder, "*.xml");
            
            foreach (string strFile in files)
            {
                BIM.Project project = mgr.ReadProject(strFile, m_dicPOITypes);

                if (project != null)
                {
                    project.LocalFilePath = strFile;
                    projects.Add(project);
                }
            }

            return projects;
        }

        public List<Shapes.POITypeProperty> POITypePropertys
        {
            get { return m_poiTypePropertyList; }
        }

        public List<Project> GetProjectList(_SqlConnection connection)
        {
            List<Project> results = null;
            string strSQL = "Select ID, Name, UnitOfLength, TimeStamp, Author from Project";

            try
            {
                _SqlDataReader reader = ReadQuery(strSQL, connection, null);

                if (reader != null)
                {
                    results = new List<Project>();

                    while (reader.Read())
                    {
                        Project project = new Project();

                        project.ID = reader.GetInt32(0);
                        project.Name = reader.GetString(1);
                        project.Unit = (Project.UnitOfLength)reader.GetInt32(2);
                        project.TimeStamp = reader.GetDateTime(3);

                        if (reader.IsDBNull(4) == false)
                            project.Author = reader.GetString(4);

                        results.Add(project);
                    }

                    reader.Close();

                    foreach (Project project in results)
                    {
                        LoadProjectComponent(project, connection);
                    }
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return results;
        }

        private bool LoadProjectComponent(Project project, _SqlConnection connection)
        {
            string strSQL = "Select ID, TypeName, ComponentName from Component where ProjectID = " + project.ID.ToString();

            try
            {
                _SqlDataReader reader = ReadQuery(strSQL, connection, null);

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        Component component = new Component();

                        component.ID = reader.GetInt32(0);
                        component.TypeName = reader.GetString(1);
                        component.ComponentName = reader.GetString(2);

                        project.AddComponent(component);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        public List<Level> LoadXMLProject(Project project)
        {
            if (project == null)
                return null;

            foreach (Project prj in m_openedProjects)
            {
                if (prj.ID == project.ID)
                    return null;
            }

            m_strErrorMessage = "";

            XMLManager mgr = new XMLManager();

            if (mgr.ReadLevels(project, m_dicPOITypes) == false)
            {
                m_strErrorMessage = mgr.ErrorMessage;
                return null;
            }

            if (m_initPOITypes == false)
            {
                Dictionary<int, POIType> dicPOITypes = new Dictionary<int, POIType>();

                foreach (KeyValuePair<int, POIType> pair in m_dicPOITypes)
                {
                    dicPOITypes[pair.Key] = pair.Value;
                }

                ReadPOIFile(dicPOITypes);
                m_initPOITypes = true;
            }

            return project.Levels;
        }

        public List<Level> LoadProject(Project project, _SqlConnection connection)
        {
            if (project == null)
                return null;

            foreach (Project prj in m_openedProjects)
            {
                if (prj.ID == project.ID)
                    return null;
            }

            m_strErrorMessage = "";
            string strSQL = "Select ID, Name, Elevation from Level where ProjectID = " + project.ID.ToString();

            try
            {
                _SqlDataReader reader = ReadQuery(strSQL, connection, null);

                if (reader != null)
                {
                    project.Levels.Clear();

                    while (reader.Read())
                    {
                        Level level = new Level();

                        level.ID = reader.GetInt32(0);
                        level.Name = reader.GetString(1);
                        level.Elevation = float.Parse(reader.GetValue(2).ToString());

                        project.Levels.Add(level);
                    }

                    reader.Close();
                    project.SortLevel();

                    /*Level firstLevel = project.GetFirstLevel();

                    if (firstLevel != null)
                    {

                    }*/
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return project.Levels;
        }

        public bool LoadXMLLevel(List<Shapes.Layer> layers, Level level, Project project)
        {
            if (level.CompleteLoading)
                return true;

            layers = level.SetLayers(layers);

            m_strErrorMessage = "";

            level.MakeShapes(layers, project.Unit);

            LoadPOIVisible(layers, level, project);
            SetPOIColor(layers);

            return true;
        }

        public bool LoadLevel(List<Shapes.Layer> layers, Level level, Project project, _SqlConnection connection)
        {
            if (level.CompleteLoading)
                return true;

            layers = level.SetLayers(layers);

            m_strErrorMessage = "";

            if (LoadWalls(layers, level, project, connection) == false)
                return false;
            if (LoadSpaces(layers, level, project, connection) == false)
                return false;
            if (LoadDoors(layers, level, project, connection) == false)
                return false;
            if (LoadWindows(layers, level, project, connection) == false)
                return false;
            if (LoadPOIs(layers, level, project, connection) == false)
                return false;
            if (LoadWires(layers, level, project, connection) == false)
                return false;

            level.MakeShapes(layers, project.Unit);

            LoadPOIVisible(layers, level, project);
            SetPOIColor(layers);

            return true;
        }

        // key : projectID_levelID
        private Dictionary<string, Dictionary<int, bool>> m_dicPOIVisible = new Dictionary<string, Dictionary<int, bool>>();
        public Dictionary<string, Dictionary<int, bool>> DicPOIVisible
        {
            get { return m_dicPOIVisible; }
            set { m_dicPOIVisible = value; }
        }
        private void LoadPOIVisible(List<Shapes.Layer> layers, Level level, Project project)
        {
            string key = project.Name + "_" + level.ID;
            if (m_dicPOIVisible.ContainsKey(key))
                m_dicPOIVisible.Remove(key);

            if (!Directory.Exists(Application.StartupPath + "\\POITypeVisible"))
                Directory.CreateDirectory(Application.StartupPath + "\\POITypeVisible");

            if (!File.Exists(Application.StartupPath + "\\POITypeVisible\\" + project.Name + "_" + level.ID + ".ini"))
                return;

            using (StreamReader sr = new StreamReader(Application.StartupPath + "\\POITypeVisible\\" + project.Name + "_" + level.ID + ".ini"))
            {
                while (!sr.EndOfStream)
                {
                    string strLine = sr.ReadLine();
                    if (strLine.Length == 0)
                        continue;

                    string[] property = strLine.Split(',');
                    if (property.Length != 2)
                        return;

                    int nPoiID = Convert.ToInt32(property[0]);
                    bool isVisible = (property[1] == "0") ? false : true;

                    if (!m_dicPOIVisible.ContainsKey(key))
                        m_dicPOIVisible.Add(key, new Dictionary<int, bool>());
                    m_dicPOIVisible[key].Add(nPoiID, isVisible);
                }
            }

            foreach (Shapes.Layer item in layers)
            {
                if (item is Shapes.POILayer)
                {
                    Shapes.POILayer poiLayer = item as Shapes.POILayer;
                    foreach (Shapes.Shape shape in poiLayer.Shapes)
                    {
                        if (shape is Shapes.POI)
                        {
                            Shapes.POI poi = shape as Shapes.POI;
                            poi.PoiType.POIVisible = GetPOIVisible(project.Name, level.ID, poi.PoiType.ID);
                        }
                    }
                }
            }
        }

        // key : projectID_levelID
        private Dictionary<int, Color> m_dicPOIColor = new Dictionary<int, Color>();
        public Dictionary<int, Color> DicPOIColor
        {
            get { return m_dicPOIColor; }
            set { m_dicPOIColor = value; }
        }

        public void LoadPOIColor()
        {
            m_dicPOIColor.Clear();

            if (!Directory.Exists(Application.StartupPath + "\\POITypeColor"))
                Directory.CreateDirectory(Application.StartupPath + "\\POITypeColor");

            if (!File.Exists(Application.StartupPath + "\\POITypeColor\\POITypeColor.ini"))
                return;

            using (StreamReader sr = new StreamReader(Application.StartupPath + "\\POITypeColor\\POITypeColor.ini"))
            {
                while (!sr.EndOfStream)
                {
                    string strLine = sr.ReadLine();
                    if (strLine.Length == 0)
                        continue;

                    string[] property = strLine.Split('z');
                    if (property.Length != 2)
                        return;

                    int nPoiID = Convert.ToInt32(property[0]);
                    Color color = GetColor(property[1]);

                    m_dicPOIColor[nPoiID] = color;
                }
            }           
        }

        public void SavePOITypeVisible()
        {
            foreach (KeyValuePair<string, Dictionary<int, bool>> item in m_dicPOIVisible)
            {
                using (StreamWriter sw = new StreamWriter(Application.StartupPath + "\\POITypeVisible\\" + item.Key + ".ini", false))
                {
                    foreach (KeyValuePair<int, bool> item2 in item.Value)
                    {
                        sw.WriteLine(item2.Key + "," + ((item2.Value) ? "1" : "0"));
                    }
                }
            }
        }

        public void SavePOITypeColor()
        { 
            using (StreamWriter sw = new StreamWriter(Application.StartupPath + "\\POITypeColor\\POITypeColor.ini", false))
            {
                foreach (KeyValuePair<int, Color> item in m_dicPOIColor)
                {
                    Color color = item.Value;
                    string strColor = color.R + "," + color.G + "," + color.B;
                    sw.WriteLine(item.Key + "z" + strColor);
                }
            }
        }

        private void SetPOIColor(List<Shapes.Layer> layers)
        {
            foreach (Shapes.Layer item in layers)
            {
                if (item is Shapes.POILayer)
                {
                    Shapes.POILayer poiLayer = item as Shapes.POILayer;
                    foreach (Shapes.Shape shape in poiLayer.Shapes)
                    {
                        if (shape is Shapes.POI)
                        {
                            Shapes.POI poi = shape as Shapes.POI;
                            if (m_dicPOIColor.ContainsKey(poi.PoiType.ID))
                                poi.FillColor = m_dicPOIColor[poi.PoiType.ID];
                        }
                    }
                }
            }
        }

        public bool GetPOIVisible(string projectName, int levelID, int poiTypeID)
        {
            string key = projectName + "_" + levelID;
            if (!m_dicPOIVisible.ContainsKey(key))
                return true;

            int tempPoiID = poiTypeID;
            Dictionary<int, bool> temp = m_dicPOIVisible[key];

            for (int i = 0; i < 4; i++)
            {
                if (i == 0 && !temp.ContainsKey(tempPoiID))
                    return true;

                if (!temp.ContainsKey(tempPoiID))
                    return true;

                bool visible = temp[tempPoiID];
                if (!visible)
                    return false;

                foreach (KeyValuePair<int, POIType> item in POITypes)
                {
                    if (item.Key == tempPoiID)
                    {
                        if (item.Value.Parent == null)
                            return visible;

                        tempPoiID = item.Value.Parent.ID;
                        if (tempPoiID == 0)
                            return visible;

                        break;
                    }
                }
            }

            return true;
        }

        public void SetPoiVisibleTrue(string projectName, int levelID, int poiID)
        {
            string key = projectName + "_" + levelID;
            if (!m_dicPOIVisible.ContainsKey(key))
                return;

            int tempPoiID = poiID;
            Dictionary<int, bool> temp = m_dicPOIVisible[key];

            for (int i = 0; i < 4; i++)
            {
                temp[tempPoiID] = true;
                foreach (KeyValuePair<int, POIType> item in POITypes)
                {
                    if (item.Key == tempPoiID)
                    {
                        if (item.Value.Parent == null)
                            break;
                        tempPoiID = item.Value.Parent.ID;
                        break;
                    }
                }
            }
        }

        private Dictionary<int, POIType> GetPOITypes(_SqlConnection connection)
        {
            if (m_dicPOITypes != null)
                return m_dicPOITypes;

            string strSQL = "Select ID, IsGroup, ParentID, Name, Code, IsUserDefined from POIType";

            try
            {
                _SqlDataReader reader = ReadQuery(strSQL, connection, null);
                m_dicPOITypes = new Dictionary<int, POIType>();
                
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        int nID = reader.GetInt32(0);
                        bool isGroup = reader.GetBoolean(1);
                        int nParentID = reader.IsDBNull(2) ? -1 : reader.GetInt32(2);
                        string strPOIName = reader.GetString(3);
                        string strCode = reader.IsDBNull(4) ? "" : reader.GetString(4);
                        bool isUserDefined = reader.GetBoolean(5);
                        POIType poiType = null;

                        if (isGroup == false)
                        {
                            poiType = new POIType();
                            poiType.ID = nID;
                            poiType.ParentID = nParentID;
                            poiType.Name = strPOIName;
                            poiType.UserDefined = isUserDefined;
                            poiType.Code = strCode;

                            if (m_dicPOIColor.ContainsKey(nID))
                                poiType.Color = m_dicPOIColor[nID];

                            m_dicPOITypes[nID] = poiType;
                        }
                    }

                    reader.Close();

                    POIType parent;

                    foreach (KeyValuePair<int, POIType> pair in m_dicPOITypes)
                    {
                        if (m_dicPOITypes.TryGetValue(pair.Value.ParentID, out parent))
                            pair.Value.Parent = parent;
                    }
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return null;
            }

            return m_dicPOITypes;
        }

        public void LoadPOITypes(_SqlConnection connection)
        {
            string strSQL = "Select ID, Name, IsUserDefined, Code, ParentID, IsGroup from POIType";

            try
            {
                Dictionary<int, POIType> dicPOITypes = new Dictionary<int, POIType>();
                _SqlDataReader reader = ReadQuery(strSQL, connection, null);

                if (m_dicPOITypes == null)
                    m_dicPOITypes = new Dictionary<int, POIType>();

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        int nID = reader.GetInt32(0);
                        string strPOIName = reader.GetString(1);
                        bool isUserDefined = reader.GetBoolean(2);
                        string strCode = reader.IsDBNull(3) ? "" : reader.GetString(3);
                        int nParentID = reader.IsDBNull(4) ? -1 : reader.GetInt32(4);
                        bool bIsGroup = reader.GetBoolean(5);

                        POIType poiType = new POIType();

                        poiType.ID = nID;
                        poiType.Name = strPOIName;
                        poiType.UserDefined = isUserDefined;
                        poiType.Code = strCode;
                        poiType.ParentID = nParentID;
                        poiType.IsGroup = bIsGroup;

                        if (m_dicPOIColor.ContainsKey(nID))
                            poiType.Color = m_dicPOIColor[nID];

                        m_dicPOITypes[nID] = poiType;
                        dicPOITypes[nID] = poiType;
                    }

                    reader.Close();

                    POIType parent;

                    foreach (KeyValuePair<int, POIType> pair in m_dicPOITypes)
                    {
                        if (m_dicPOITypes.TryGetValue(pair.Value.ParentID, out parent))
                            pair.Value.Parent = parent;
                    }
                }

                ReadPOIFile(dicPOITypes);
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return;
            }
        }

        public void LoadPOITypeProperty(_SqlConnection connection)
        {
            string strSQL = "Select POITypeID, PropertyName, PropertyValue, Description from POITypeProperty";

            try
            {                
                _SqlDataReader reader = ReadQuery(strSQL, connection, null);
                
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        int nPOITypeID = reader.GetInt32(0);
                        string strPropertyName = reader.GetString(1);
                        string nPropertyValue = reader.IsDBNull(2) ? "" : reader.GetString(2);
                        string strDescription = reader.IsDBNull(3) ? "" : reader.GetString(3);
                        
                        Shapes.POITypeProperty property = new Shapes.POITypeProperty();

                        property.POITypeID = nPOITypeID;
                        property.PropertyName = strPropertyName;
                        property.ProperetyValue = nPropertyValue;
                        property.Description = strDescription;

                        m_poiTypePropertyList.Add(property);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return;
            }
        }

        private bool ReadPOIFile(Dictionary<int, POIType> dicPOITypes)
        {
            int nIndex = System.Windows.Forms.Application.ExecutablePath.LastIndexOf('\\');

            if (nIndex < 0)
                return false;

            string strFolderPath = System.Windows.Forms.Application.ExecutablePath.Substring(0, nIndex);
            strFolderPath += "\\POI";

            if (Directory.Exists(strFolderPath) == false)
                return true;

            foreach (KeyValuePair<int, POIType> pair in dicPOITypes)
            {
                if (pair.Value.Code == null || pair.Value.Code.Length == 0)
                    continue;

                string strFilePath = strFolderPath + "\\" + pair.Value.Code + ".poi";

                if (File.Exists(strFilePath) == false)
                    continue;

                FileStream fs = new FileStream(strFilePath, FileMode.Open);
                BinaryReader reader = new BinaryReader(fs);

                POIType poiType = POIType.ReadPOI(reader);
                pair.Value.CopyFrom(poiType);

                reader.Close();
            }

            return true;
        }

        /*private bool ReadPOIFile(Dictionary<string, POIType> dicPOITypes)
        {
            int nIndex = System.Windows.Forms.Application.ExecutablePath.LastIndexOf('\\');

            if (nIndex < 0)
                return false;

            string strFilePath = System.Windows.Forms.Application.ExecutablePath.Substring(0, nIndex);
            strFilePath += "\\system.poi";

            if (File.Exists(strFilePath) == false)
                return false;

            FileStream fs = new FileStream(strFilePath, FileMode.Open);
            BinaryReader reader = new BinaryReader(fs);

            POIType poi;
            int nPOICount = reader.ReadInt32();

            for (int i=0;i<nPOICount;i++)
            {
                POIType poiType = POIType.ReadPOI(reader);

                if (poiType == null)
                {
                    reader.Close();
                    return false;
                }

                if (dicPOITypes.TryGetValue(poiType.Name, out poi))
                {
                    poi.CopyFrom(poiType);
                }
            }

            reader.Close();
            return true;
        }*/

        private POIType LoadPOIType(int nID, _SqlConnection connection)
        {
            string strSQL = "Select IsGroup, ParentID, Name, Code, IsUserDefined from POIType where ID = " + nID.ToString();
            POIType poiType = null;

            try
            {
                _SqlDataReader reader = ReadQuery(strSQL, connection, null);

                if (m_dicPOITypes == null)
                    m_dicPOITypes = new Dictionary<int, POIType>();

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        bool isGroup = reader.GetBoolean(0);
                        int nParentID = reader.IsDBNull(1) ? -1 : reader.GetInt32(1);
                        string strPOIName = reader.GetString(2);
                        string strCode = reader.IsDBNull(3) ? "" : reader.GetString(3);
                        bool isUserDefined = reader.GetBoolean(4);

                        if (isGroup == false)
                        {
                            poiType = new POIType();
                            poiType.ID = nID;
                            poiType.ParentID = nParentID;
                            poiType.Name = strPOIName;
                            poiType.UserDefined = isUserDefined;
                            poiType.Code = strCode;

                            POIType parent;

                            if (m_dicPOITypes.TryGetValue(nParentID, out parent))
                                poiType.Parent = parent;

                            if (m_dicPOIColor.ContainsKey(nID))
                                poiType.Color = m_dicPOIColor[nID];

                            m_dicPOITypes[nID] = poiType;
                        }
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return null;
            }

            return poiType;
        }

        private bool LoadPOIs(List<Shapes.Layer> layers, Level level, Project project, _SqlConnection connection)
        {
            string strSQL = "Select ID, TypeID, Name, X, Y from POI where LevelID = " + level.ID.ToString();
            m_dicPOITypes = GetPOITypes(connection);

            try
            {
                _SqlDataReader reader = ReadQuery(strSQL, connection, null);
                Dictionary<Shapes.POI, int> dicNoTypePOIs = new Dictionary<Shapes.POI, int>();

                if (reader != null)
                {
                    POIType poiType = null;

                    while (reader.Read())
                    {
                        int nID = reader.GetInt32(0);
                        int nTypeID = reader.GetInt32(1);
                        string strName = reader.GetString(2);
                        double x = reader.GetDouble(3);
                        double y = reader.GetDouble(4);

                        Shapes.POI poi = new Shapes.POI();
                        poi.ID = nID;
                        poi.Name = strName;
                        poi.Position = new Vertex2D(x, y);

                        if (m_dicPOITypes.TryGetValue(nTypeID, out poiType) == false)
                        {
                            dicNoTypePOIs[poi] = nTypeID;
                        }
                        else
                        {
                            poi.PoiType = poiType;
                            level.AddPOI(poi);
                        }
                    }

                    reader.Close();
                }

                foreach (KeyValuePair<Shapes.POI, int> pair in dicNoTypePOIs)
                {
                    POIType poiType = LoadPOIType(pair.Value, connection);

                    if (poiType != null)
                    {
                        pair.Key.PoiType = poiType;
                        level.AddPOI(pair.Key);
                    }
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool LoadWires(List<Shapes.Layer> layers, Level level, Project project, _SqlConnection connection)
        {
            string strSQL = "Select ID, BeginPOI, EndPOI, POITypeID, Lines From POIWire where LevelID = " + level.ID.ToString();
            
            try
            {
                _SqlDataReader reader = ReadQuery(strSQL, connection, null);
                
                if (reader != null)
                {
                    System.Resources.ResourceManager rm = Properties.Resources.ResourceManager;

                    while (reader.Read())
                    {
                        int nID = reader.GetInt32(0);
                        int nBeginPOI = reader.GetInt32(1);
                        int nEndPOI = reader.GetInt32(2);
                        int nPOITypeID = reader.GetInt32(3);
                        string strLines = reader.GetString(4);
                        
                        Shapes.Wire wire = new Shapes.Wire();                        
                        wire.ID = nID;
                        wire.BeginPOI = nBeginPOI;
                        wire.EndPOI = nEndPOI;
                        wire.POITypeID = nPOITypeID;
                        wire.Lines = strLines;
                        wire.LevelID = level.ID;

                        POIType poiType = null;
                        if (m_dicPOITypes.TryGetValue(nPOITypeID, out poiType))
                        {                            
                            Bitmap img = (Bitmap)rm.GetObject(poiType.Code);
                            if (img == null)
                                img = (Bitmap)rm.GetObject("empty");

                            wire.Icon = img;

                            Shapes.POI poi = new Shapes.POI();
                            poi.PoiType = poiType;
                            wire.POIIcon = poi;                            
                        }

                        string[] lines = strLines.Split(',');
                        for (int i = 0; i < lines.Length; i+=2)
                        {
                            double x = Convert.ToDouble(lines[i]);
                            double y = Convert.ToDouble(lines[i + 1]);
                            wire.Positions.Add(new Vertex2D(x, y));
                        }

                        wire.SetIconPosition();

                        level.AddWire(wire);
                    }

                    reader.Close();
                }                
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool LoadWindows(List<Shapes.Layer> layers, Level level, Project project, _SqlConnection connection)
        {
            string strSQL = "Select ID, WallID, X, Y, Width, Height, Elevation from Window where LevelID = " + level.ID.ToString();

            try
            {
                _SqlDataReader reader = ReadQuery(strSQL, connection, null);

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        int nID = reader.GetInt32(0);
                        int nWallID = reader.GetInt32(1);

                        Wall wall = level.FindWall(nWallID);

                        if (wall == null)
                            continue;

                        Window window = new Window();
                        window.ID = nID;

                        double x = reader.GetDouble(2);
                        double y = reader.GetDouble(3);
                        window.Position = new Vertex2D(x, y);

                        window.Width = (float)reader.GetDouble(4);
                        window.Height = (float)reader.GetDouble(5);
                        window.Elevation = (float)reader.GetDouble(6);

                        wall.AddWindow(window);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool LoadDoors(List<Shapes.Layer> layers, Level level, Project project, _SqlConnection connection)
        {
            string strSQL = "Select ID, WallID, X, Y, Width, Height, Elevation, DoorType, Hinge1X, Hinge1Y, Hinge2X, Hinge2Y from Door where LevelID = " + level.ID.ToString();

            try
            {
                _SqlDataReader reader = ReadQuery(strSQL, connection, null);

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        int nID = reader.GetInt32(0);
                        int nWallID = reader.GetInt32(1);

                        Wall wall = level.FindWall(nWallID);

                        if (wall == null)
                            continue;

                        Door door = new Door();
                        door.ID = nID;

                        double x = reader.GetDouble(2);
                        double y = reader.GetDouble(3);
                        door.Position = new Vertex2D(x, y);

                        door.Width = (float)reader.GetDouble(4);
                        door.Height = (float)reader.GetDouble(5);
                        door.Elevation = (float)reader.GetDouble(6);
                        door.SetDoorType(reader.GetInt32(7));

                        wall.AddDoor(door);

                        if (reader.IsDBNull(8) == false && reader.IsDBNull(9) == false)
                        {
                            double x1 = reader.GetDouble(8);
                            double y1 = reader.GetDouble(9);
                            door.Hinge1 = new Vertex2D(x1, y1);
                        }

                        if (reader.IsDBNull(10) == false && reader.IsDBNull(11) == false)
                        {
                            double x2 = reader.GetDouble(10);
                            double y2 = reader.GetDouble(11);
                            door.Hinge2 = new Vertex2D(x2, y2);
                        }
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool LoadSpaces(List<Shapes.Layer> layers, Level level, Project project, _SqlConnection connection)
        {
            string strSQL = "Select Space.ID, Space.Name, swl.WallID, swl.WallIndex from Space, SpaceWallLink as swl ";
            strSQL += "where Space.ID = swl.SpaceID and Space.LevelID = " + level.ID.ToString() + " order by Space.ID, swl.WallIndex";

            string strSpaceIDs = "";

            try
            {
                _SqlDataReader reader = ReadQuery(strSQL, connection, null);

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        int nID = reader.GetInt32(0);
                        string strName = reader.GetString(1);

                        Space space = level.FindSpace(nID);

                        if (space == null)
                        {
                            space = new Space();
                            space.ID = nID;
                            space.Name = strName;
                            space.Level = level;

                            level.AddSpace(space);
                        }

                        int nWallID = reader.GetInt32(2);

                        Wall wall = level.FindWall(nWallID);

                        if (wall == null)
                            continue;

                        space.AddWall(wall);
                        //space.Walls.Add(wall);

                        /*if (reader.IsDBNull(4) == false)
                        {
                            bool isSafetyFire = reader.GetBoolean(4);
                            space.SafetyFire = isSafetyFire;
                        }*/

                        if (strSpaceIDs.Length == 0)
                            strSpaceIDs = nID.ToString();
                        else
                            strSpaceIDs += ", " + nID.ToString();
                    }

                    reader.Close();

                    if (strSpaceIDs.Length > 0)
                    {
                        strSQL = "Select SpaceID, PropertyValue from SpaceProperty where PropertyName = '" + Space.SafetyFireTag + "' and SpaceID in (" + strSpaceIDs + ")";
                        reader = ReadQuery(strSQL, connection, null);

                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                int nID = reader.GetInt32(0);
                                string strValue = reader.GetString(1).Trim().ToLower();

                                Space space = level.FindSpace(nID);

                                if (space != null)
                                {
                                    space.SafetyFire = strValue == "1" || strValue == "true";
                                }
                            }

                            reader.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool LoadWalls(List<Shapes.Layer> layers, Level level, Project project, _SqlConnection connection)
        {
            string strSQL = "Select wall.ID, Thick, Height, ComponentID, grid.GridType, grid.BeginX, grid.BeginY, grid.EndX, grid.EndY, grid.ThirdX, grid.ThirdY, grid.BeginAngle, grid.Angle, grid.ClockWise ";
            strSQL += "from wall, grid where wall.GridID = grid.ID and wall.LevelID = " + level.ID.ToString();

            try
            {
                _SqlDataReader reader = ReadQuery(strSQL, connection, null);

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        Wall wall = new Wall();

                        wall.ID = reader.GetInt32(0);
                        wall.Thick = reader.GetDouble(1);
                        wall.Height = reader.GetDouble(2);
                        wall.Component = project.FindComponent(reader.GetInt32(3));
                        wall.SetGridType(reader.GetInt32(4));

                        if (wall.GetGridType() == Wall.GridType.Line)
                        {
                            double x1 = reader.GetDouble(5);
                            double y1 = reader.GetDouble(6);
                            double x2 = reader.GetDouble(7);
                            double y2 = reader.GetDouble(8);

                            Line2D line = new Line2D(new Vertex2D(x1, y1), new Vertex2D(x2, y2));
                            wall.Line = line;
                        }
                        else if (wall.GetGridType() == Wall.GridType.Arc)
                        {
                            if (reader.IsDBNull(9) || reader.IsDBNull(11) || reader.IsDBNull(12) || reader.IsDBNull(13))
                                continue;

                            double x = reader.GetDouble(5);
                            double y = reader.GetDouble(6);
                            double radius = reader.GetDouble(9);
                            double beginAngle = reader.GetDouble(11);
                            double arcAngle = reader.GetDouble(12);
                            bool isClockWise = reader.GetInt32(13) == 1;

                            Arc2D arc = new Arc2D(new Vertex2D(x, y), radius, beginAngle, arcAngle, isClockWise);
                            wall.Arc = arc;
                        }
                        else if (wall.GetGridType() == Wall.GridType.EArc)
                        {
                            if (reader.IsDBNull(9) || reader.IsDBNull(10) || reader.IsDBNull(11) || reader.IsDBNull(12) || reader.IsDBNull(13))
                                continue;

                            double x1 = reader.GetDouble(5);
                            double y1 = reader.GetDouble(6);
                            double x2 = reader.GetDouble(7);
                            double y2 = reader.GetDouble(8);
                            double x3 = reader.GetDouble(9);
                            double y3 = reader.GetDouble(10);
                            double beginAngle = reader.GetDouble(11);
                            double earcAngle = reader.GetDouble(12);
                            bool isClockWise = reader.GetInt32(13) == 1;

                            EArc2D earc = new EArc2D(new Vertex2D(x1, y1), new Vertex2D(x2, y2), new Vertex2D(x3, y3), beginAngle, earcAngle, isClockWise);
                            wall.EArc = earc;
                        }

                        level.AddWall(wall);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        public Color GetColor(string code)
        {
            Color color = Color.Red;

            if (code.Length == 0)
                return color;

            string[] strColorArr = code.Split(',');
            if (strColorArr.Length != 3)
                return color;

            int red = 255;
            int.TryParse(strColorArr[0], out red);

            int green = 255;
            int.TryParse(strColorArr[1], out green);

            int blue = 255;
            int.TryParse(strColorArr[2], out blue);

            return Color.FromArgb(red, green, blue);
        }

        public _SqlDataReader ReadQuery(string strSQL, _SqlConnection connection, _SqlTransaction transaction)
        {
            try
            {
                _SqlCommand cmd = new _SqlCommand(strSQL, connection, transaction);
                return cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return null;
        }

        public bool ExecuteQuery(string strSQL, _SqlConnection connection, _SqlTransaction transaction)
        {
            try
            {
                _SqlCommand cmd = new _SqlCommand(strSQL, connection, transaction);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }
    }
}
