using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;
using UnE.Geometry;

namespace DBToXML.Data
{
    public class POI
    {
        private string m_strID = "";
        private POIType m_poiType = null;
        private string m_strName = "";
        private Vertex2D m_vPos = null;
        private double m_dAngle = 0.0;
        private VariousData<double> m_height = null;
        private List<Property> m_properties = new List<Property>();

        public const string POIIDTag = "poi";

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public POIType POIType
        {
            get { return m_poiType; }
            set { m_poiType = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public Vertex2D Position
        {
            get { return m_vPos; }
            set { m_vPos = value; }
        }

        public double Angle
        {
            get { return m_dAngle; }
            set { m_dAngle = value; }
        }

        public VariousData<double> Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public static Dictionary<int, POI> ReadPOI(int nLevelID, Dictionary<int, POIType> dicPOITypes, WebDBManager dbMgr)
        {
            Dictionary<int, POI> dicPOIs = new Dictionary<int, POI>();

            string strSQL = "Select ID, TypeID, Name, X, Y, Angle, Height from POI where LevelID = " + nLevelID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return dicPOIs;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-6;i+=7)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> typeID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                string strName = WebDBManager.GetStringField(arrResult[i + 2]);
                VariousData<float> x = WebDBManager.GetFloatField(arrResult[i + 3].ToString());
                VariousData<float> y = WebDBManager.GetFloatField(arrResult[i + 4].ToString());
                VariousData<float> angle = WebDBManager.GetFloatField(arrResult[i + 5].ToString());
                VariousData<float> height = WebDBManager.GetFloatField(arrResult[i + 6].ToString());

                if (id == null || typeID == null || strName == null || x == null || y == null || angle == null)
                    continue;

                POIType poiType;

                if (dicPOITypes.TryGetValue(typeID.Data, out poiType) == false)
                    continue;

                POI poi = new POI();

                poi.m_strID = POIIDTag + id.Data.ToString();
                poi.POIType = poiType;
                poi.Name = strName;
                poi.Position = new Vertex2D(x.Data, y.Data);
                poi.Angle = angle.Data;
                poi.Height = height == null ? null : new VariousData<double>(height.Data);

                dicPOIs[id.Data] = poi;

                //List<Property> properties = Property.ReadDB(dbMgr, "POIProperties", "POIProperty", "POIID", id.Data);
                //poi.m_properties = properties;
            }

            Dictionary<int, List<Property>> dicProperties = Property.ReadDB(dbMgr, "POIProperties", "POIProperty", "POIID", "LevelID = " + nLevelID.ToString());

            foreach (KeyValuePair<int, List<Property>> pair in dicProperties)
            {
                POI poi;

                if (dicPOIs.TryGetValue(pair.Key, out poi) == false)
                    continue;

                poi.m_properties = pair.Value;
            }

            return dicPOIs;
        }
    }
}
