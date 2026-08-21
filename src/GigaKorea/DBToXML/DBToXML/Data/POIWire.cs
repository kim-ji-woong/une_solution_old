using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;
using DBUtility2;
using System.Collections;

namespace DBToXML.Data
{
    public class POIWire
    {
        private string m_strID = "";
        private POI m_begin = null;
        private POI m_end = null;
        private POIType m_poiType = null;
        private string m_strLines = "";

        public const string POIWireIDTag = "pw";

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public POI BeginPOI
        {
            get { return m_begin; }
            set { m_begin = value; }
        }

        public POI EndPOI
        {
            get { return m_end; }
            set { m_end = value; }
        }

        public POIType POIType
        {
            get { return m_poiType; }
            set { m_poiType = value; }
        }

        public string Lines
        {
            get { return m_strLines; }
        }

        public static List<POIWire> ReadPOIWire(int nLevelID, Dictionary<int, POI> dicPOIs, Dictionary<int, POIType> dicPOITypes, WebDBManager dbMgr)
        {
            string strSQL = "Select ID, BeginPOI, EndPOI, POITypeID, Lines from POIWire where LevelID = " + nLevelID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            List<POIWire> wires = new List<POIWire>();

            if (arrResult == null)
                return wires;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> beginPOI = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> endPOI = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> poiTypeID = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                string strLines = WebDBManager.GetStringField(arrResult[i + 4]);

                if (id == null || beginPOI == null || endPOI == null || poiTypeID == null || strLines == null)
                    continue;

                POI begin, end;
                POIType poiType;

                if (dicPOIs.TryGetValue(beginPOI.Data, out begin) == false || dicPOIs.TryGetValue(endPOI.Data, out end) == false)
                    continue;

                if (dicPOITypes.TryGetValue(poiTypeID.Data, out poiType) == false)
                    continue;

                POIWire wire = new POIWire();

                wire.m_strID = POIWireIDTag + id.Data.ToString();
                wire.m_begin = begin;
                wire.m_end = end;
                wire.m_poiType = poiType;
                wire.m_strLines = strLines;

                wires.Add(wire);
            }

            return wires;
        }
    }
}
