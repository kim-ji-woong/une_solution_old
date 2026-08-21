using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Collections;

// 07_과제수행\23_sop(남동발전선진통합방재시스템)\05_Data\06_공간DB자료관리\5.2차원_베이스도면\130221_전체배치도_정북(기준으로 작업)b.dxf 파일 사용

namespace DXFUtility
{
    public class BoundaryLoader
    {
        public static string MakeBoundaryString(DXFViewer.PolyLine pLine, UnE.Geometry.Vertex2D vMove)
        {
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

            for (int i = 0; i < nVertexSize; i++)
            {
                System.Drawing.PointF pt = pLine.GetVertex(i);
                UnE.Geometry.Vertex2D vertex = new UnE.Geometry.Vertex2D((pt.X - vMove.x) * dUnitFlag, (pt.Y - vMove.y) * dUnitFlag);

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
                    FormMain.Instance.Text = dlg.FileName;

                    if (ReadZoneBlock(dxf, "Zone", "Zone_name"))
                        MessageBox.Show("DXF 로딩 및 DB 데이터 삽입 완료");
                    else
                        MessageBox.Show("DB 데이터 삽입에 실패하였습니다.");
                }
            }
        }

        private bool ReadZoneBlock(DXFViewer.DXFControl dxf, string strBoundaryLayerName, string strTextLayerName)
        {
            string strSQL = "delete from Zone where BuildingID = -1 and SiteID = 1";
            if (FormMain.Instance.DBManager.GetResultData(strSQL, 0) == null)
                return false;

            int nMaxID = -1;
            // Zone Name, Zone
            Dictionary<string, Zone> dicZones = new Dictionary<string, Zone>();
            bool loadBoundary = false, loadText = false;

            UnE.Geometry.Vertex2D vMove = dxf.MovedVertex;

            foreach (DXFViewer.Layer layer in dxf.Layers)
            {
                if (layer.LayerName == strBoundaryLayerName)
                {
                    foreach (DXFViewer.Shape shape in layer.Shapes)
                    {
                        DXFViewer.Block block = shape.GetBlock();
                        if (block == null)
                            continue;

                        string strZoneName = block.Name;
                        DXFViewer.Shape.ShapeType shapeType = shape.GetShapeType();

                        if (shape.GetShapeType() == DXFViewer.Shape.ShapeType.POLYLINE)
                        {
                            DXFViewer.PolyLine pLine = (DXFViewer.PolyLine)shape;
                            MakeZone(strZoneName, pLine, dicZones, vMove, ref nMaxID);
                        }
                    }

                    loadBoundary = true;
                    if (loadText)
                        break;
                }
                else if (layer.LayerName == strTextLayerName)
                {
                    foreach (DXFViewer.Shape shape in layer.Shapes)
                    {
                        DXFViewer.Block block = shape.GetBlock();
                        if (block == null)
                            continue;

                        string strZoneName = block.Name;
                        DXFViewer.Shape.ShapeType shapeType = shape.GetShapeType();

                        if (shapeType == DXFViewer.Shape.ShapeType.TEXT)
                        {
                            DXFViewer.Text text = (DXFViewer.Text)shape;
                            MakeZone(strZoneName, text, dicZones, ref nMaxID);
                        }
                    }

                    loadText = true;
                    if (loadBoundary)
                        break;
                }
            }

            return InsertZones(dicZones);
        }

        private void MakeZone(string strZoneName, DXFViewer.Text text, Dictionary<string, Zone> dicZones, ref int nMaxID)
        {
            Zone zone = null;

            if (dicZones.ContainsKey(strZoneName))
                zone = dicZones[strZoneName];
            else
            {
                if (nMaxID < 0)
                {
                    string strSQL = string.Format("select max(id) from Zone");
                    ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

                    if (arrResult == null || arrResult.Count == 0)
                        return;

                    nMaxID = FormMain.Instance.DBManager.GetIntField(arrResult[0].ToString(), -1);
                }

                zone = new Zone();

                zone.BuildingID = -1;
                zone.FloorIndex = 0;
                zone.ID = ++nMaxID;
                zone.ZoneName = strZoneName;

                dicZones[strZoneName] = zone;
            }

            zone.TextCenter.x = text.Position.x;
            zone.TextCenter.y = text.Position.y;
        }

        private void MakeZone(string strZoneName, DXFViewer.PolyLine pLine, Dictionary<string, Zone> dicZones, UnE.Geometry.Vertex2D vMove, ref int nMaxID)
        {
            Zone zone = null;

            if (dicZones.ContainsKey(strZoneName))
                zone = dicZones[strZoneName];
            else
            {
                if (nMaxID < 0)
                {
                    string strSQL = string.Format("select max(id) from Zone");
                    ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

                    if (arrResult == null || arrResult.Count == 0)
                        return;

                    nMaxID = FormMain.Instance.DBManager.GetIntField(arrResult[0].ToString(), -1);
                }

                zone = new Zone();

                zone.BuildingID = -1;
                zone.FloorIndex = 0;
                zone.ID = ++nMaxID;
                zone.ZoneName = strZoneName;

                dicZones[strZoneName] = zone;
            }

            zone.BoundaryVertices = MakeBoundaryString(pLine, vMove);
        }

        private bool InsertZones(Dictionary<string, Zone> dicZones)
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;

            foreach (KeyValuePair<string, Zone> pair in dicZones)
            {
                Zone zone = pair.Value;

                string strTextCenter = string.Format("{0:f3}, {1:f3}", zone.TextCenter.x, zone.TextCenter.y);

                string strSQL = string.Format("Insert into Zone (id, ZoneName, SiteID, BuildingID, FloorIndex, Boundary, TextCenter) values ({0}, '{1}', 1, {2}, {3}, '{4}', '{5}')",
                    zone.ID, zone.ZoneName, zone.BuildingID, zone.FloorIndex, zone.BoundaryVertices, strTextCenter);

                if (dbMgr.GetResultData(strSQL, 0) == null)
                    return false;
            }

            return true;
        }
    }
}
