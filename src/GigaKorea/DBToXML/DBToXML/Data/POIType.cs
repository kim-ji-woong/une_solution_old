using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;

namespace DBToXML.Data
{
    public class POIType
    {
        private string m_strID = "";
        private bool m_isGroup = true;
        private POIType m_parent = null;
        private List<POIType> m_childTypes = new List<POIType>();
        private string m_strName = "";
        private string m_strCode = null;
        private bool m_isUserDefined = false;
        private VariousData<float> m_defaultHeight = null;
        private List<Property> m_properties = new List<Property>();

        public const string POITypeIDTag = "pt";

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public bool IsGroup
        {
            get { return m_isGroup; }
            set { m_isGroup = value; }
        }

        public POIType Parent
        {
            get { return m_parent; }
            set
            {
                m_parent = value;

                if (m_parent != null)
                {
                    if (m_parent.m_childTypes.Contains(this) == false)
                        m_parent.m_childTypes.Add(this);
                }
            }
        }

        public List<POIType> ChildTypes
        {
            get { return m_childTypes; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public string Code
        {
            get { return m_strCode; }
            set { m_strCode = value; }
        }

        public bool IsUserDefined
        {
            get { return m_isUserDefined; }
            set { m_isUserDefined = value; }
        }

        public VariousData<float> DefaultHeight
        {
            get { return m_defaultHeight; }
            set { m_defaultHeight = value; }
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public static Dictionary<int, POIType> ReadPOIType(WebDBManager dbMgr)
        {
            Dictionary<int, POIType> dicPOITypes = new Dictionary<int, POIType>();
            Dictionary<POIType, int> dicParentIDs = new Dictionary<POIType, int>();

            string strSQL = "Select ID, IsGroup, ParentID, Name, Code, IsUserDefined, DefaultHeight from POIType";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return dicPOITypes;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-6;i+=7)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> isGroup = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> parentID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                string strName = WebDBManager.GetStringField(arrResult[i + 3]);
                string strCode = WebDBManager.GetStringField(arrResult[i + 4]);
                VariousData<int> isUserDefined = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                VariousData<float> defaultHeight = WebDBManager.GetFloatField(arrResult[i + 6].ToString());

                if (id == null || isGroup == null || strName == null || isUserDefined == null)
                    continue;

                POIType poiType = new POIType();

                poiType.m_strID = POITypeIDTag + id.Data.ToString();
                poiType.m_isGroup = isGroup.Data == 1;
                poiType.m_strName = strName;
                poiType.m_strCode = strCode;
                poiType.m_isUserDefined = isUserDefined.Data == 1;
                poiType.m_defaultHeight = defaultHeight;

                if (parentID != null)
                    dicParentIDs[poiType] = parentID.Data;

                dicPOITypes[id.Data] = poiType;

                List<Property> properties = Property.ReadDB(dbMgr, "POITypeProperties", "POITypeProperty", "POITypeID", id.Data);
                poiType.m_properties = properties;
            }

            int nParentID;

            foreach (KeyValuePair<int, POIType> pair in dicPOITypes)
            {
                if (dicParentIDs.TryGetValue(pair.Value, out nParentID))
                {
                    POIType parent;

                    if (dicPOITypes.TryGetValue(nParentID, out parent))
                        pair.Value.Parent = parent;
                }
            }

            return dicPOITypes;
        }
    }
}
