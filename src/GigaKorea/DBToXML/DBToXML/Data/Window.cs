using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;

namespace DBToXML.Data
{
    public class Window
    {
        private string m_strID = "";
        private double m_x = 0.0;
        private double m_y = 0.0;
        private double m_dWidth = 0.0;
        private double m_dHeight = 0.0;
        private double m_dElevation = 0.0;
        private List<Property> m_properties = new List<Property>();

        public const string WindowIDTag = "window";

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

        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public static List<Window> ReadWindows(int nLevelID, Dictionary<int, Wall> dicWalls, WebDBManager dbMgr)
        {
            string strSQL = "Select ID, WallID, X, Y, Width, Height, Elevation from Window where LevelID = " + nLevelID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            Dictionary<int, Window> dicWindows = new Dictionary<int, Window>();

            if (arrResult == null)
                return dicWindows.Values.ToList();

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> wallID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<float> x = WebDBManager.GetFloatField(arrResult[i + 2].ToString());
                VariousData<float> y = WebDBManager.GetFloatField(arrResult[i + 3].ToString());
                VariousData<float> width = WebDBManager.GetFloatField(arrResult[i + 4].ToString());
                VariousData<float> height = WebDBManager.GetFloatField(arrResult[i + 5].ToString());
                VariousData<float> elevation = WebDBManager.GetFloatField(arrResult[i + 6].ToString());

                if (id == null || wallID == null || x == null || y == null || width == null || height == null || elevation == null)
                    continue;

                Wall wall;

                if (dicWalls.TryGetValue(wallID.Data, out wall) == false)
                    continue;

                Window window = new Window();

                window.m_strID = WindowIDTag + id.Data.ToString();
                window.X = x.Data;
                window.Y = y.Data;
                window.Width = width.Data;
                window.Height = height.Data;
                window.Elevation = elevation.Data;

                dicWindows[id.Data] = window;
                wall.Windows.Add(window);

                //List<Property> properties = Property.ReadDB(dbMgr, "WindowProperties", "WindowProperty", "WindowID", id.Data);
                //window.m_properties = properties;
            }

            Dictionary<int, List<Property>> dicProperties = Property.ReadDB(dbMgr, "WindowProperties", "WindowProperty", "WindowID", "LevelID = " + nLevelID.ToString());

            foreach (KeyValuePair<int, List<Property>> pair in dicProperties)
            {
                Window window;

                if (dicWindows.TryGetValue(pair.Key, out window) == false)
                    continue;

                window.m_properties = pair.Value;
            }

            return dicWindows.Values.ToList();
        }

        public static List<Window> ReadWindow(int nWallID, WebDBManager dbMgr)
        {
            string strSQL = "Select ID, X, Y, Width, Height, Elevation from Window where WallID = " + nWallID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            List<Window> windows = new List<Window>();

            if (arrResult == null)
                return windows;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<float> x = WebDBManager.GetFloatField(arrResult[i + 1].ToString());
                VariousData<float> y = WebDBManager.GetFloatField(arrResult[i + 2].ToString());
                VariousData<float> width = WebDBManager.GetFloatField(arrResult[i + 3].ToString());
                VariousData<float> height = WebDBManager.GetFloatField(arrResult[i + 4].ToString());
                VariousData<float> elevation = WebDBManager.GetFloatField(arrResult[i + 5].ToString());
                
                if (id == null || x == null || y == null || width == null || height == null || elevation == null)
                    continue;

                Window window = new Window();

                window.m_strID = WindowIDTag + id.Data.ToString();
                window.X = x.Data;
                window.Y = y.Data;
                window.Width = width.Data;
                window.Height = height.Data;
                window.Elevation = elevation.Data;
                
                windows.Add(window);

                List<Property> properties = Property.ReadDB(dbMgr, "WindowProperties", "WindowProperty", "WindowID", id.Data);
                window.m_properties = properties;
            }

            return windows;
        }
    }
}
