using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Collections;

namespace DXFUtility
{
    public class BoundaryLoader
    {
        private static UnE.Geometry.Vertex2D GetMinCoord(DXFViewer.PolyLine pLine, UnE.Geometry.Vertex2D vMove, double dUnitFlag, UnE.Geometry.Vertex2D vBlockOrigin)
        {
            UnE.Geometry.Vertex2D vMin = new UnE.Geometry.Vertex2D();
            int nVertexSize = pLine.GetVertexSize();

            for (int i = 0; i < nVertexSize; i++)
            {
                System.Drawing.PointF pt = pLine.GetVertex(i);

                UnE.Geometry.Vertex2D vertex = new UnE.Geometry.Vertex2D((pt.X - vMove.x) * dUnitFlag, (pt.Y - vMove.y) * dUnitFlag);
                vertex = vertex + vBlockOrigin;

                if (i == 0)
                    vMin = vertex;
                else
                {
                    if (vMin.x > vertex.x)
                        vMin.x = vertex.x;
                    if (vMin.y > vertex.y)
                        vMin.y = vertex.y;
                }
            }

            return vMin;
        }

        public static string MakeBoundaryString(DXFViewer.PolyLine pLine, DXFViewer.Block block, UnE.Geometry.Vertex2D vMove, out UnE.Geometry.Vertex2D vCenter, bool moveCoords)
        {
            // 무게 중심
            vCenter = new UnE.Geometry.Vertex2D();

            int nVertexSize = pLine.GetVertexSize();
            string strBoundary = "";

            UnE.Geometry.Vertex2D vPrev = null;
            UnE.Geometry.Vertex2D vFirst = null;
            double dUnitFlag = 0.001;

            if (nVertexSize < 3)
                return strBoundary;

            System.Drawing.PointF ptBegin = pLine.GetVertex(0);
            System.Drawing.PointF ptLast = pLine.GetVertex(nVertexSize - 1);

            UnE.Geometry.Vertex2F vBegin = new UnE.Geometry.Vertex2F(ptBegin.X, ptBegin.Y);
            UnE.Geometry.Vertex2F vLast = new UnE.Geometry.Vertex2F(ptLast.X, ptLast.Y);

            if (vBegin.GetDistance(vLast) <= UnE.Geometry.Math.HALF_TOLERANCE())
            {
                nVertexSize--;
            }

            UnE.Geometry.Vertex2D vBlockOrigin = block == null ? new UnE.Geometry.Vertex2D(0, 0) : block.OriginVertex * dUnitFlag;
            UnE.Geometry.Vertex2D vMin;

            if (moveCoords)
                vMin = GetMinCoord(pLine, vMove, dUnitFlag, vBlockOrigin);
            else
                vMin = new UnE.Geometry.Vertex2D();

            for (int i = 0; i < nVertexSize; i++)
            {
                System.Drawing.PointF pt = pLine.GetVertex(i);
                
                UnE.Geometry.Vertex2D vertex = new UnE.Geometry.Vertex2D((pt.X - vMove.x) * dUnitFlag, (pt.Y - vMove.y) * dUnitFlag);
                vertex = vertex + vBlockOrigin - vMin;

                vCenter += vertex;

                if (i == 0)
                    vFirst = vertex;
                else if (i == nVertexSize - 1)
                {
                    if (vFirst.GetDistance(vertex) < 0.1)
                        break;
                }

                if (vPrev == null)
                {
                    vPrev = vertex;
                    strBoundary = string.Format("{0:f3}, {1:f3}", vertex.x, vertex.y);
                }
                else
                {
                    if (vertex.GetDistance(vPrev) <= UnE.Geometry.Math.HALF_TOLERANCE())
                        continue;

                    System.Drawing.PointF ptNext = i < nVertexSize - 1 ? pLine.GetVertex(i + 1) : pLine.GetVertex(0);
                    UnE.Geometry.Vertex2D vNext = new UnE.Geometry.Vertex2D(ptNext.X, ptNext.Y);

                    UnE.Geometry.Line2D line = new UnE.Geometry.Line2D(vPrev, vNext);

                    if (line.IsInclude(vertex))
                        continue;

                    vPrev = vertex;
                    strBoundary += string.Format(", {0:f3}, {1:f3}", vertex.x, vertex.y);
                }
            }

            vCenter /= nVertexSize;

            return strBoundary;
        }
    }

    public class ZoneBoundaryLoader : BoundaryLoader
    {
        public ZoneBoundaryLoader()
        {

        }

        public void Run()
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "DXF Files|*.dxf|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "DXF 파일 열기";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DXFViewer.DXFControl dxf = new DXFViewer.DXFControl();

                bool isSuccess = dxf.OpenDXF(dlg.FileName);

                if (!isSuccess)
                {
                    string strError = "DXF 불러오기가 실패하였습니다.";
                    MessageBox.Show(strError);
                }
                else
                {
                    if (ReadZoneBlock(dxf))
                        MessageBox.Show("DXF 로딩 및 DB 데이터 삽입 완료");
                    else
                        MessageBox.Show("DB 데이터 삽입에 실패하였습니다.");
                }
            }
        }

        private bool ReadZoneBlock(DXFViewer.DXFControl dxf)
        {
            ArrayList arrLayerNames = new ArrayList();

            arrLayerNames.Add("ASSISTANCE ROOM");
            arrLayerNames.Add("CONTROL ROOM");
            arrLayerNames.Add("MOVING_EQUIP");
            arrLayerNames.Add("PLAN");

            string strSQL = "delete from Zone";
            if (FormMain.Instance.DBManager.GetResultData(strSQL, 0, "HSMS") == null)
                return false;

            strSQL = "delete from Equipment";
            if (FormMain.Instance.DBManager.GetResultData(strSQL, 0, "HSMS") == null)
                return false;

            int nEquipMaxID = 0, nZoneMaxID = 0;
            UnE.Geometry.Vertex2D vMove = dxf.MovedVertex;

            ArrayList arrEquipZones = new ArrayList();
            ArrayList arrZones = new ArrayList();

            foreach (DXFViewer.Layer layer in dxf.Layers)
            {
                if (arrLayerNames.Contains(layer.LayerName))
                {
                    if (layer.LayerName == "MOVING_EQUIP")
                    {
                        // Equipment
                        foreach (DXFViewer.Shape shape in layer.Shapes)
                        {
                            AddPolyLineZone(shape, layer.LayerName, arrEquipZones, null, vMove, ref nEquipMaxID, "Equipment");
                        }
                    }
                    else
                    {
                        // Zone
                        foreach (DXFViewer.Shape shape in layer.Shapes)
                        {
                            AddPolyLineZone(shape, layer.LayerName, arrZones, shape.GetBlock(), vMove, ref nZoneMaxID, "Zone");
                        }
                    }
                }
                else
                {
                    // Equipment
                    foreach (DXFViewer.Shape shape in layer.Shapes)
                    {
                        DXFViewer.Block block = shape.GetBlock();
                        if (block == null)
                            continue;

                        string strZoneName = block.Name;
                        AddPolyLineZone(shape, strZoneName, arrEquipZones, block, vMove, ref nEquipMaxID, "Equipment");
                    }
                }
            }

            bool result1 = InsertEquipments(arrEquipZones);
            bool result2 = InsertZones(arrZones);
            return result1 && result2;
        }

        private void AddPolyLineZone(DXFViewer.Shape shape, string strZoneName, ArrayList arrZones, DXFViewer.Block block, UnE.Geometry.Vertex2D vMove, ref int nMaxID, string strTableName)
        {
            DXFViewer.Shape.ShapeType shapeType = shape.GetShapeType();

            if (shape.GetShapeType() == DXFViewer.Shape.ShapeType.POLYLINE)
            {
                DXFViewer.PolyLine pLine = (DXFViewer.PolyLine)shape;
                Zone zone = MakeZone(strZoneName, pLine, block, vMove, ref nMaxID, strTableName);

                if (zone != null)
                    arrZones.Add(zone);
            }
        }

        private Zone MakeZone(string strZoneName, DXFViewer.PolyLine pLine, DXFViewer.Block block, UnE.Geometry.Vertex2D vMove, ref int nMaxID, string strTableName)
        {
            if (nMaxID <= 0)
            {
                string strSQL = string.Format("select max(id) from " + strTableName);
                ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0, "HSMS");

                if (arrResult == null || arrResult.Count == 0)
                    return null;

                nMaxID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0);
            }

            Zone zone = new Zone();

            zone.ID = ++nMaxID;
            zone.ZoneName = strZoneName;

            UnE.Geometry.Vertex2D vTextCenter;
            zone.Boundary = MakeBoundaryString(pLine, block, vMove, out vTextCenter, false);//strTableName == "Equipment");
            zone.TextCenter.SetVertex(vTextCenter.x, vTextCenter.y);

            return zone;
        }

        private bool InsertZones(ArrayList arrZones)
        {
            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            foreach (Zone zone in arrZones)
            {
                string strTextCenter = string.Format("{0:f3}, {1:f3}", zone.TextCenter.x, zone.TextCenter.y);
                string strDescription = zone.Description == null ? "NULL" : "'" + zone.Description + "'";
                string strPermitLevel = GetPermitLevelString(zone.PermitLevel);

                string strSQL = string.Format("Insert into Zone (id, ZoneName, Boundary, SiteID, PermitLevel, TextCenter, Description) values ({0}, '{1}', '{2}', {3}, {4}, '{5}', {6})",
                    zone.ID, zone.ZoneName, zone.Boundary, zone.SiteID, strPermitLevel, strTextCenter, strDescription);

                if (dbMgr.GetResultData(strSQL, 0, "HSMS") == null)
                    return false;
            }

            return true;
        }

        private bool InsertEquipments(ArrayList arrZones)
        {
            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            foreach (Zone zone in arrZones)
            {
                string strTextCenter = string.Format("{0:f3}, {1:f3}", zone.TextCenter.x, zone.TextCenter.y);
                string strDescription = zone.Description == null ? "NULL" : "'" + zone.Description + "'";

                string strSQL = string.Format("Insert into Equipment (id, EquipCode, MeshName, Boundary, SensorPos, SiteID, TextCenter, Description) values " + 
                    "({0}, '', '{1}', '{2}', NULL, {3}, '{4}', NULL)",
                    zone.ID, zone.ZoneName, zone.Boundary, zone.SiteID, strTextCenter, strDescription);

                if (dbMgr.GetResultData(strSQL, 0, "HSMS") == null)
                    return false;
            }

            return true;
        }

        private string GetPermitLevelString(ArrayList arrPermitLevels)
        {
            if (arrPermitLevels == null || arrPermitLevels.Count == 0)
                return "NULL";

            string strResult = "";

            foreach (int nLevel in arrPermitLevels)
            {
                if (strResult.Length == 0)
                    strResult = nLevel.ToString();
                else
                    strResult += ", " + nLevel.ToString();
            }

            return "'" + strResult + "'";
        }
    }

    public class Zone
    {
        private int m_nID = -1;
        private string m_strZoneName = "";
        private string m_strBoundary = "";
        private int m_nSiteID = 1;
        private ArrayList m_arrPermitLevel = new ArrayList();
        private UnE.Geometry.Vertex2D m_vTextCenter = new UnE.Geometry.Vertex2D();
        private string m_strDescription = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string ZoneName
        {
            get { return m_strZoneName; }
            set { m_strZoneName = value; }
        }

        public string Boundary
        {
            get { return m_strBoundary; }
            set { m_strBoundary = value; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public ArrayList PermitLevel
        {
            get { return m_arrPermitLevel; }
        }

        public UnE.Geometry.Vertex2D TextCenter
        {
            get { return m_vTextCenter; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }
    }
}
