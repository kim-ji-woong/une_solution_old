using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Sensor;
using UnE.Geometry;
using System.IO;

namespace IconEditor
{
    public class POIManager
    {
        private Vertex2D m_vC1 = null, m_vC2 = null, m_vC3 = null;
        private Vertex2D m_v1 = null, m_v2 = null, m_v3 = null;
        private double m_dCLen, m_dLen;
        private float m_fElevation;
        private string m_strFileName = "";
        private bool m_isRightSide = false;

        public bool RightSide
        {
            get { return m_isRightSide; }
            set { m_isRightSide = value; }
        }

        public bool HasData
        {
            get { return m_vC1 != null; }
        }

        public string FileName
        {
            get { return m_strFileName; }
        }

        public List<POI> LoadPOI(POI poi1, POI poi2, float x1, float y1, float x2, float y2, string strFileName, float fElevation, out List<int> poiIDs, out List<string> poiTypes, out List<bool> poiVisibles)
        {
            CalcBasic(poi1, poi2, x1, y1, x2, y2);
            List<POI> pois = ReadPOI(strFileName, fElevation);

            poiIDs = new List<int>();
            poiTypes = new List<string>();
            poiVisibles = new List<bool>();

            foreach (POI poi in pois)
            {
                poiIDs.Add(-1);
                poiTypes.Add("CCTV");
                poiVisibles.Add(true);
            }

            m_fElevation = fElevation;
            m_strFileName = strFileName;

            //strPOIFile = MakePOIFile(pois);
            return pois;
        }

        public List<POI> ReloadPOI(POI poi1, POI poi2, float x1, float y1, float x2, float y2, out List<int> poiIDs, out List<string> poiTypes, out List<bool> poiVisibles)
        {
            CalcBasic(poi1, poi2, x1, y1, x2, y2);
            List<POI> pois = ReadPOI(m_strFileName, m_fElevation);

            poiIDs = new List<int>();
            poiTypes = new List<string>();
            poiVisibles = new List<bool>();

            foreach (POI poi in pois)
            {
                poiIDs.Add(-1);
                poiTypes.Add("CCTV");
                poiVisibles.Add(true);
            }

            return pois;
        }

        public void Init()
        {
            m_vC1 = m_vC2 = m_vC3 = null;
            m_v1 = m_v2 = m_v3 = null;
        }

        private string MakePOIFile(List<POI> pois)
        {
            string strPOIFile = "AddPOI.txt";
            StreamWriter writer = new StreamWriter(strPOIFile, false, Encoding.UTF8);

            int nIndex = 1;

            foreach (POIData poi in pois)
            {
                string strLog = string.Format("{0},{1},{2},CCTV_5103_{3}", poi.X, poi.Y, poi.Z, nIndex++);
                writer.WriteLine(strLog);
            }

            writer.Close();
            return strPOIFile;
        }

        private List<POI> ReadPOI(string strFileName, float fElevation)
        {
            List<POI> pois = new List<POI>();

            double x, y;
            int nID = 3;
            StreamReader reader = new StreamReader(strFileName, Encoding.UTF8);

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                string[] tokens = strLine.Split('\t');

                if (tokens.Count() != 4)
                    continue;

                if (double.TryParse(tokens[2].Trim(), out x) == false || double.TryParse(tokens[3].Trim(), out y) == false)
                    continue;

                string strSensorType = tokens[0].Trim();
                string strSensorName = tokens[1].Trim();

                if (strSensorType.Length == 0 || strSensorName.Length == 0)
                    continue;

                Vertex2D vPos = GetPOIVertex(new Vertex2D(x, y));

                POIData poiData = new POIData();
                poiData.X = (float)vPos.x;
                poiData.Y = fElevation;
                poiData.Z = (float)vPos.y;
                poiData.SensorName = strSensorName;
                poiData.SensorType = strSensorType;
                poiData.ID = poiData.Facility.ID = nID++;
                //poiData.Popup = new SelectionManager((ISensor)poiData.Facility);

                pois.Add(poiData);
            }

            reader.Close();
            return pois;
        }

        private Vertex2D GetPOIVertex(Vertex2D vCad)
        {
            Vertex2D vC13 = UnE.Geometry.Math.GetNearestVertex(vCad, m_vC1, m_vC3, true);
            Vertex2D vC23 = UnE.Geometry.Math.GetNearestVertex(vCad, m_vC2, m_vC3, true);

            double dCLen1 = GetLengthFromLine(vC13, m_vC1, m_vC3);//m_vC1.GetDistance(vC13);
            double dCLen2 = GetLengthFromLine(vC23, m_vC2, m_vC3);//m_vC2.GetDistance(vC23);

            double dLen1 = m_dLen * dCLen1 / m_dCLen;
            double dLen2 = m_dLen * dCLen2 / m_dCLen;

            Vertex2D v13 = UnE.Geometry.Math.GetLinearVertex(m_v1, m_v3, dLen1);
            Vertex2D v23 = UnE.Geometry.Math.GetLinearVertex(m_v2, m_v3, dLen2);

            return v23 - m_v3 + v13;
        }

        // vertex가 v1과 v2를 잇는 직선위에 존재한다.
        // v1에서 출발하여 v2 방향으로 얼만큼 떨어진 거리에 vertex가 위치하는지 알려준다.
        private double GetLengthFromLine(Vertex2D vertex, Vertex2D v1, Vertex2D v2)
        {
            double dLen1 = v1.GetDistance(vertex);
            double dLen2 = v2.GetDistance(vertex);

            if (dLen1 > dLen2)
                return dLen1;
            else if (dLen2 > v1.GetDistance(v2))
                return -dLen1;

            return dLen1;
        }

        // POI3는 방향을 표시하기 위해서 있다.
        private void CalcBasic(POI poi1, POI poi2, float x1, float y1, float x2, float y2)
        {
            #region CAD좌표
            Vertex2D vC1 = new Vertex2D(x1, y1);
            Vertex2D vC2 = new Vertex2D(x2, y2);
            Vertex2D vC3 = new Vertex2D(vC1.x, vC2.y);

            double dCLen1 = vC1.GetDistance(vC3);
            double dAngle = UnE.Geometry.Math.GetAngle(vC2, vC1, vC3);
            double dCLen2 = dCLen1 * System.Math.Cos(dAngle);

            Vertex2D vCTemp = UnE.Geometry.Math.GetLinearVertex(vC1, vC2, dCLen2);

            double dCLen3 = vCTemp.GetDistance(vC3);
            double dCLen12 = vC1.GetDistance(vC2);

            bool isRightSide = UnE.Geometry.Math.IsRightSideFromLine(vC3, vC1, vC2) != 0;

            if (m_isRightSide)
                isRightSide = !isRightSide;
            #endregion

            #region 3D좌표
            Vertex2D v1 = new Vertex2D(poi1.X, poi1.Z);
            Vertex2D v2 = new Vertex2D(poi2.X, poi2.Z);

            double dLen12 = v1.GetDistance(v2);
            double dLen2 = dLen12 * dCLen2 / dCLen12;
            double dLen3 = dLen12 * dCLen3 / dCLen12;

            Vertex2D vTemp = UnE.Geometry.Math.GetLinearVertex(v1, v2, dLen2);
            Vertex2D v3 = UnE.Geometry.Math.GetRightVertex(vTemp, v1, isRightSide ? -dLen3 : dLen3);
            #endregion

            m_vC1 = vC1;
            m_vC2 = vC2;
            m_vC3 = vC3;
            m_dCLen = dCLen12;

            m_v1 = v1;
            m_v2 = v2;
            m_v3 = v3;
            m_dLen = dLen12;
        }

        public List<POI> Make2POIs(float x1, float y1, float x2, float y2, float fElevation, out List<int> poiIDs, out List<string> poiTypes, out List<bool> poiVisibles)
        {
            List<POI> pois = new List<POI>();
            poiIDs = new List<int>();
            poiTypes = new List<string>();
            poiVisibles = new List<bool>();

            POIData poiData = new POIData();
            poiData.X = x1;
            poiData.Y = fElevation;
            poiData.Z = y1;
            poiData.SensorName = "P1";
            poiData.SensorType = "CCTV";
            poiData.ID = poiData.Facility.ID = 1;
            poiData.Popup = new SelectionManager(poiData.Facility);

            poiIDs.Add(-1);
            poiTypes.Add("CCTV");
            poiVisibles.Add(true);
            pois.Add(poiData);

            poiData = new POIData();
            poiData.X = x2;
            poiData.Y = fElevation;
            poiData.Z = y2;
            poiData.SensorName = "P2";
            poiData.SensorType = "CCTV";
            poiData.ID = poiData.Facility.ID = 2;
            poiData.Popup = new SelectionManager(poiData.Facility);

            poiIDs.Add(-1);
            poiTypes.Add("CCTV");
            poiVisibles.Add(true);
            pois.Add(poiData);
            return pois;
        }
    }

    public class POIData : UnE.Sensor.POI
    {
        private string m_strSensorType = "";
        private string m_strSensorName = "";

        public string SensorType
        {
            get { return m_strSensorType; }
            set { m_strSensorType = value; }
        }

        public string SensorName
        {
            get { return m_strSensorName; }
            set { m_strSensorName = value; }
        }

        public POIData()
        {
            this.Facility = new CCTV();
        }
    }

    public class SelectionManager : IPOIPopup
    {
        private ISensor m_sensor = null;
        private IFacility m_facility = null;

        public SelectionManager(IFacility facility)
        {
            m_facility = facility;
        }

        public void Show(int xTarget, int yTarget)
        {
            System.Diagnostics.Trace.WriteLine("Select POI");
        }

        public void Hide(bool absolutely)
        {
        }

        public void Hide()
        {
        }

        public void MoveTarget(int xTarget, int yTarget)
        {
        }

        public bool IsVisible()
        {
            return true;
        }

        public void Close()
        {
        }

        public bool LayerVisible
        {
            get { return true; }
            set { }
        }

        public IntPtr Handle
        {
            get { return FormMain.Instance.Handle; }
        }

        public UnE.Sensor.ISensor Sensor
        {
            get { return m_sensor; }
            set { m_sensor = value; }
        }
    }
}
