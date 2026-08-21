using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoilMan.ShapeFile
{
    public class ShapeFileLoader : libShapeFile.IFileEventListener
    {
        private DXFViewer.DXFControl m_dxfControl = null;

        public ShapeFileLoader(DXFViewer.DXFControl ctrl)
        {
            m_dxfControl = ctrl;
        }

        public void BeginReadFile(string szPath, string szType, int nCount)
        {
            FormMain.Instance.DXFControl_BeginRead(szPath, szType, nCount);
        }

        public void ReadEntity(string szName, int nCount)
        {
            FormMain.Instance.DXFControl_ReadEntity(szName, nCount);
        }

        public void EndReadFile(string szPath, string szType)
        {
            FormMain.Instance.DXFControl_EndRead(szPath, szType);
        }

        public bool OpenUSHFile(string strPath, ref DXFViewer.Layer prevLayer, ref UnE.Geometry.Vertex2D prevMovedVertex, ref UnE.Geometry.Vertex2D vTL, ref UnE.Geometry.Vertex2D vBR, out libShapeFile.ShapeInfo shapeInfo)
        {
            if (prevLayer != null)
            {
                m_dxfControl.Layers.Remove(prevLayer);
                prevLayer = null;
            }

            shapeInfo = null;
            libShapeFile.USHReader reader = new libShapeFile.USHReader();
            List<libShapeFile.Shape> shapes = reader.Read(strPath, out shapeInfo);

            //vTL = vBR = prevMovedVertex = null;

            if (shapes == null)
                return false;

            if (vTL == null)
                vTL = new UnE.Geometry.Vertex2D(reader.TopLeft.x, reader.TopLeft.y);
            else
            {
                if (vTL.x > reader.TopLeft.x)
                    vTL.x = reader.TopLeft.x;

                if (vTL.y < reader.TopLeft.y)
                    vTL.y = reader.TopLeft.y;
            }

            if (vBR == null)
                vBR = new UnE.Geometry.Vertex2D(reader.BottomRight.x, reader.BottomRight.y);
            else
            {
                if (vBR.x < reader.BottomRight.x)
                    vBR.x = reader.BottomRight.x;

                if (vBR.y > reader.BottomRight.y)
                    vBR.y = reader.BottomRight.y;
            }

            Drawing.IShapeAttrib attr = (Drawing.IShapeAttrib)FormMain.Instance;
            UnE.Geometry.Vertex2D vOrigin = new UnE.Geometry.Vertex2D();

            if (prevMovedVertex == null)
            {
               // System.Drawing.Rectangle rect = System.Windows.Forms.Screen.FromControl(FormMain.Instance).Bounds;
                //int nScaleSize = rect.Width > rect.Height ? rect.Width : rect.Height;

                //UnE.Geometry.Vertex2D vCenter = null;
                UnE.Geometry.Vertex2D vScreenCenter = new UnE.Geometry.Vertex2D(FormMain.Instance.Size.Width / 2, FormMain.Instance.Size.Height / 2);
                ////double dScale = GetScale(loader.TopLeft, loader.BottomRight, nScaleSize, out vCenter);

                prevLayer = LoadShapes(shapes, vOrigin, shapeInfo, attr);
                //SetViewport(new UnE.Geometry.Vertex2D(), vBR - vTL);
                
                prevMovedVertex = new UnE.Geometry.Vertex2D(-vTL.x, -vTL.y);
                m_dxfControl.MoveAll(prevMovedVertex.x, prevMovedVertex.y);
            }
            else
            {
                prevLayer = LoadShapes(shapes, vOrigin, shapeInfo, attr);

                if (prevLayer != null)
                    prevLayer.MoveAll(prevMovedVertex.x, prevMovedVertex.y);

                //SetViewport(vTL + prevMovedVertex, vBR + prevMovedVertex);
            }

            FormMain.Instance.TimeLog("Prev MakeQuadTreeNode");
            FormMain.Instance.QuadTree.MakeNode((float)reader.TopLeft.x, (float)reader.TopLeft.y, (float)reader.BottomRight.x, (float)reader.BottomRight.y);
            AddToQuadTree(FormMain.Instance.QuadTree);
            FormMain.Instance.TimeLog("After MakeQuadTreeNode");

            return true;
        }

        public bool OpenFile(string strPath, ref DXFViewer.Layer prevLayer, ref UnE.Geometry.Vertex2D prevMovedVertex, ref UnE.Geometry.Vertex2D vTL, ref UnE.Geometry.Vertex2D vBR, out libShapeFile.ShapeInfo shapeInfo)
        //public bool OpenFile(string strPath, ref DXFViewer.Layer prevLayer, out UnE.Geometry.Vertex2D prevMovedVertex, out UnE.Geometry.Vertex2D vTL, out UnE.Geometry.Vertex2D vBR, out libShapeFile.ShapeInfo shapeInfo)
        {
            if (prevLayer != null)
            {
                m_dxfControl.Layers.Remove(prevLayer);
                prevLayer = null;
            }

            shapeInfo = null;
            libShapeFile.FileLoader loader = new libShapeFile.FileLoader(this);
            List<libShapeFile.Shape> shapes = loader.LoadFile(strPath, out shapeInfo);

            //vTL = vBR = prevMovedVertex = null;

            if (shapes == null)
                return false;

            if (vTL == null)
                vTL = loader.TopLeft;
            else
            {
                if (vTL.x > loader.TopLeft.x)
                    vTL.x = loader.TopLeft.x;

                if (vTL.y < loader.TopLeft.y)
                    vTL.y = loader.TopLeft.y;
            }

            if (vBR == null)
                vBR = loader.BottomRight;
            else
            {
                if (vBR.x < loader.BottomRight.x)
                    vBR.x = loader.BottomRight.x;

                if (vBR.y > loader.BottomRight.y)
                    vBR.y = loader.BottomRight.y;
            }

            Drawing.IShapeAttrib attr = (Drawing.IShapeAttrib)FormMain.Instance;
            UnE.Geometry.Vertex2D vOrigin = new UnE.Geometry.Vertex2D();
            
            if (prevMovedVertex == null)
            {
                //System.Drawing.Rectangle rect = System.Windows.Forms.Screen.FromControl(FormMain.Instance).Bounds;
                //int nScaleSize = rect.Width > rect.Height ? rect.Width : rect.Height;
               //UnE.Geometry.Vertex2D vCenter = null;
                //UnE.Geometry.Vertex2D vScreenCenter = new UnE.Geometry.Vertex2D(FormMain.Instance.Size.Width / 2, FormMain.Instance.Size.Height / 2);
                //double dScale = GetScale(loader.TopLeft, loader.BottomRight, nScaleSize, out vCenter);

                prevLayer = LoadShapes(shapes, vOrigin, shapeInfo, attr);
                
                //SetViewport(new UnE.Geometry.Vertex2D(), vBR - vTL);

                prevMovedVertex = m_dxfControl.MovedVertex;
                m_dxfControl.MoveAll(prevMovedVertex.x, prevMovedVertex.y);
            }
            else
            {
                prevLayer = LoadShapes(shapes, vOrigin, shapeInfo, attr);

                if (prevLayer != null)
                    prevLayer.MoveAll(prevMovedVertex.x, prevMovedVertex.y);

                //SetViewport(vTL + prevMovedVertex, vBR + prevMovedVertex);
            }

            FormMain.Instance.TimeLog("Prev MakeQuadTreeNode");
            FormMain.Instance.QuadTree.MakeNode((float)loader.TopLeft.x, (float)loader.TopLeft.y, (float)loader.BottomRight.x, (float)loader.BottomRight.y);
            AddToQuadTree(FormMain.Instance.QuadTree);
            FormMain.Instance.TimeLog("After MakeQuadTreeNode");

            return true;
        }

        private void AddToQuadTree(QuadTree quadTree)
        {
            DXFViewer.Layer layer = FindLayer("Polygon");

            if (layer == null)
                return;

            int nShapeCount = layer.Shapes.Count;


            
            int nEntCount = 0;
            for (int i = 0; i < nShapeCount;i++ )
            //foreach (DXFViewer.Shape shape in layer.Shapes)
            {
                DXFViewer.Shape shape = (DXFViewer.Shape)layer.Shapes[i];

                if (shape is Drawing.Polygon)
                {
                    
                    quadTree.AddData((IQuadData)shape, i);
                    //ReadEntity("Add Quad Node", nEntCount);
                    nEntCount++;
                }
                else if (shape is Drawing.PolygonList)
                {
                    Drawing.PolygonList polygonList = (Drawing.PolygonList)shape;
                   
                    List<Drawing.Polygon> polyList = polygonList.GetPolygons(null);
                    int nPolygonCount = polyList.Count;
                    BeginReadFile("Create QuadTree", "QUD", nPolygonCount);
                    nEntCount = 0;
                    foreach( Drawing.Polygon polygon in polyList )
                    {
                        quadTree.AddData(polygon, nEntCount);
                        ReadEntity("Add Quad Node", nEntCount);
                        nEntCount++;
                    }
                    EndReadFile("Create QuadTree", "QUD");
                }
               

            }
           
        }

        /*private double GetScale(UnE.Geometry.Vertex2D vTL, UnE.Geometry.Vertex2D vBR, int nScaleSize, out UnE.Geometry.Vertex2D vCenter)
        {
            vCenter = (vTL + vBR) / 2;

            double dWidth = vBR.x - vTL.x;
            double dHeight = vTL.y - vBR.y;
            double dBig = dWidth > dHeight ? dWidth : dHeight;

            if (dBig <= UnE.Geometry.Math.HALF_TOLERANCE())
                return 1.0;

            double dScale = nScaleSize / dBig;
            return dScale;
        }*/

        private void SetViewport(UnE.Geometry.Vertex2D vTL, UnE.Geometry.Vertex2D vBR)
        {
            
            UnE.Geometry.Vertex2D vObjectCenter = (vTL + vBR) / 2;
            m_dxfControl.SetViewportCenter(vObjectCenter);

            double weight1 = m_dxfControl.Size.Width * 0.85 / ((vObjectCenter.x - vTL.x) * 2);
            double weight2 = m_dxfControl.Size.Height * 0.85 / ((vObjectCenter.y - vBR.y) * 2);
            double dViewportWeight = weight1 < weight2 ? weight1 : weight2;

            m_dxfControl.Zoom(dViewportWeight, vObjectCenter, false);
            
        }

        // x, y를 화면 중앙에 오도록 한다.
        private void MakeCenter(double x, double y)
        {
            int nCenterX = m_dxfControl.Size.Width / 2;
            int nCenterY = m_dxfControl.Size.Height / 2;

            UnE.Geometry.Vertex2D vOriginCenter = m_dxfControl.GetViewportCenter();

            System.Drawing.Point pt = m_dxfControl.GlobalToScreen(new UnE.Geometry.Vertex2D(x, y));

            int nMoveX = pt.X - nCenterX;
            int nMoveY = pt.Y - nCenterY;

            UnE.Geometry.Vertex2D vCenter = new UnE.Geometry.Vertex2D(vOriginCenter.x - nMoveX, vOriginCenter.y - nMoveY);
            m_dxfControl.SetViewportCenter(vCenter);
        }

        private DXFViewer.Layer LoadShapes(List<libShapeFile.Shape> shapes, UnE.Geometry.Vertex2D vCenter, libShapeFile.ShapeInfo shapeInfo, Drawing.IShapeAttrib attr)
        {
            Type typePolyLine = typeof(libShapeFile.PolyLine);
            Type typePoint = typeof(libShapeFile.Point);
            Type typePolygon = typeof(libShapeFile.Polygon);
            Type typeMultiPoint = typeof(libShapeFile.MultiPoint);

            Drawing.PolygonList polygonList = new Drawing.PolygonList();
            polygonList.SetAttrib(attr);

            foreach (libShapeFile.Shape shape in shapes)
            {
                Type type = shape.GetType();

                if (type == typePoint)
                    AddPoint((libShapeFile.Point)shape, vCenter, shapeInfo, attr);
                else if (type == typePolyLine)
                    AddPolyLine((libShapeFile.PolyLine)shape, vCenter, shapeInfo, attr);
                else if (type == typePolygon)
                    AddPolygon((libShapeFile.Polygon)shape, vCenter, shapeInfo, attr, polygonList);
                else if (type == typeMultiPoint)
                    AddMultiPoint((libShapeFile.MultiPoint)shape, vCenter, shapeInfo, attr);
            }

            DXFViewer.Layer layer = SetFirstLast();
            return layer;
        }

        private DXFViewer.Layer SetFirstLast(string strLayerName)
        {
            DXFViewer.Layer layer = FindLayer(strLayerName);

            if (layer != null)
            {
                int nShapeCount = layer.Shapes.Count;

                if (nShapeCount > 0)
                {
                    Drawing.PointShape shapeFirst = (Drawing.PointShape)layer.Shapes[0];
                    Drawing.PointShape shapeLast = (Drawing.PointShape)layer.Shapes[nShapeCount - 1];

                    shapeFirst.FirstElement = true;
                    shapeLast.LastElement = true;
                }
            }

            return layer;
        }

        private DXFViewer.Layer SetFirstLast()
        {
            DXFViewer.Layer layer = SetFirstLast("Point");

            if (layer != null)
                return layer;

            layer = SetFirstLast("PolyLine");

            if (layer != null)
                return layer;

            layer = SetFirstLast("Polygon");

            if (layer != null)
                return layer;

            layer = SetFirstLast("MultiPoint");

            if (layer != null)
                return layer;

            return null;
        }

        private DXFViewer.Layer FindLayer(string strLayerName)
        {
            foreach (DXFViewer.Layer layer in m_dxfControl.Layers)
            {
                if (layer.LayerName == strLayerName)
                    return layer;
            }

            return null;
        }

        private void AddPoint(libShapeFile.Point point, UnE.Geometry.Vertex2D vCenter, libShapeFile.ShapeInfo shapeInfo, Drawing.IShapeAttrib attr)
        {
            DXFViewer.Layer layer = FindLayer("Point");

            if (layer == null)
            {
                layer = new Drawing.ShapeLayer(m_dxfControl);
                layer.LayerName = "Point";

                int nIndex = FormMain.Instance.GetLayerInsertIndex(DockingForm.FormLayer.LayerType.지적도);

                if (nIndex >= 0)
                    m_dxfControl.Layers.Insert(nIndex, layer);
                else
                    m_dxfControl.Layers.Add(layer);
            }

            UnE.Geometry.Vertex2D vCoord = new UnE.Geometry.Vertex2D(point.Vertex.x, point.Vertex.y);
            //UnE.Geometry.Vertex2D vCoord = Drawing.BoundingShape.ScaleTransfer(point.Vertex.x, point.Vertex.y, 1.0, vCenter);
            Drawing.Point point2 = new Drawing.Point(vCoord.x, vCoord.y);
            point2.ID = point.ID;
            point2.SetAttrib(attr);
            layer.Add(point2);

            point2.ShapeInfo = shapeInfo;
        }

        private void AddPolyLine(libShapeFile.PolyLine polyLine, UnE.Geometry.Vertex2D vCenter, libShapeFile.ShapeInfo shapeInfo, Drawing.IShapeAttrib attr)
        {
            DXFViewer.Layer layer = FindLayer("PolyLine");

            if (layer == null)
            {
                layer = new Drawing.ShapeLayer(m_dxfControl);
                layer.LayerName = "PolyLine";

                int nIndex = FormMain.Instance.GetLayerInsertIndex(DockingForm.FormLayer.LayerType.지적도);

                if (nIndex >= 0)
                    m_dxfControl.Layers.Insert(nIndex, layer);
                else
                    m_dxfControl.Layers.Add(layer);
            }

            Drawing.PolyLine polyLine2 = new Drawing.PolyLine();
            int nSubLineCount = polyLine.SubPolyLineCount;

            UnE.Geometry.Vertex2F _vCenter = new UnE.Geometry.Vertex2F((float)vCenter.x, (float)vCenter.y);

            for (int i = 0; i < nSubLineCount; i++)
            {
                List<UnE.Geometry.Vertex2F> vertices = polyLine.GetSubPolyLine(i);
                polyLine2.AddVertices(vertices, 1.0, _vCenter);
            }

            UnE.Geometry.Vertex2D vMin = new UnE.Geometry.Vertex2D(polyLine.MinX, polyLine.MinY);
            UnE.Geometry.Vertex2D vMax = new UnE.Geometry.Vertex2D(polyLine.MaxX, polyLine.MaxY);
            //UnE.Geometry.Vertex2D vMin = Drawing.BoundingShape.ScaleTransfer(polyLine.MinX, polyLine.MinY, 1.0, vCenter);
            //UnE.Geometry.Vertex2D vMax = Drawing.BoundingShape.ScaleTransfer(polyLine.MaxX, polyLine.MaxY, 1.0, vCenter);

            //polyLine2.GenerateLevelPolyline();     
            

            polyLine2.SetBounding(vMin.x, vMax.x, vMin.y, vMax.y);
            polyLine2.ID = polyLine.ID;
            polyLine2.SetAttrib(attr);
            layer.Add(polyLine2);

            polyLine2.ShapeInfo = shapeInfo;
        }

        private void AddPolygon(libShapeFile.Polygon polygon, UnE.Geometry.Vertex2D vCenter, libShapeFile.ShapeInfo shapeInfo, Drawing.IShapeAttrib attr, Drawing.PolygonList polygonList)
        {
            DXFViewer.Layer layer = FindLayer("Polygon");

            if (layer == null)
            {
                layer = new Drawing.ShapeLayer(m_dxfControl);
                layer.LayerName = "Polygon";

                int nIndex = FormMain.Instance.GetLayerInsertIndex(DockingForm.FormLayer.LayerType.지적도);

                if (nIndex >= 0)
                    m_dxfControl.Layers.Insert(nIndex, layer);
                else
                    m_dxfControl.Layers.Add(layer);

                FormMain.Instance.ShapeFilePolygonLayer = layer;
            }

            Drawing.Polygon polygon2 = new Drawing.Polygon();
            int nSubPolygonCount = polygon.SubPolygonCount;

            for (int i = 0; i < nSubPolygonCount; i++)
            {
                List<UnE.Geometry.Vertex2F> vertices = polygon.GetSubPolygon(i);
                polygon2.AddVertices(vertices, 1.0, vCenter);
            }
                        
            //polygon2.GenerateLevelPolygon();            

            UnE.Geometry.Vertex2D vMin = new UnE.Geometry.Vertex2D(polygon.MinX, polygon.MinY);
            UnE.Geometry.Vertex2D vMax = new UnE.Geometry.Vertex2D(polygon.MaxX, polygon.MaxY);
            //UnE.Geometry.Vertex2D vMin = Drawing.BoundingShape.ScaleTransfer(polygon.MinX, polygon.MinY, 1.0, vCenter);
            //UnE.Geometry.Vertex2D vMax = Drawing.BoundingShape.ScaleTransfer(polygon.MaxX, polygon.MaxY, 1.0, vCenter);

           

            polygon2.SetBounding(vMin.x, vMax.x, vMin.y, vMax.y);
            polygon2.ID = polygon.ID;
            polygon2.SetAttrib(attr);



            if (layer.Shapes.Count == 0)
                layer.Add(polygonList);

            polygonList.AddPolygon(polygon2);
            //layer.Add(polygon2);

            polygon2.ShapeInfo = shapeInfo;
        }

        private void AddMultiPoint(libShapeFile.MultiPoint multiPoint, UnE.Geometry.Vertex2D vCenter, libShapeFile.ShapeInfo shapeInfo, Drawing.IShapeAttrib attr)
        {
            DXFViewer.Layer layer = FindLayer("MultiPoint");

            if (layer == null)
            {
                layer = new Drawing.ShapeLayer(m_dxfControl);
                layer.LayerName = "MultiPoint";

                int nIndex = FormMain.Instance.GetLayerInsertIndex(DockingForm.FormLayer.LayerType.지적도);

                if (nIndex >= 0)
                    m_dxfControl.Layers.Insert(nIndex, layer);
                else
                    m_dxfControl.Layers.Add(layer);
            }

            Drawing.MultiPoint multiPoint2 = new Drawing.MultiPoint();
            int nVertexCount = multiPoint.GetVertexCount();

            UnE.Geometry.Vertex2F _vCenter = new UnE.Geometry.Vertex2F((float)vCenter.x, (float)vCenter.y);

            foreach (UnE.Geometry.Vertex2F vertex in multiPoint.Vertices)
            //for (int i = 0; i < nVertexCount; i++)
            {
                //UnE.Geometry.Vertex2F vertex = multiPoint.GetVertex(i);
                multiPoint2.AddVertex(vertex);
                /*UnE.Geometry.Vertex2F vCoord = Drawing.BoundingShape.ScaleTransfer(vertex.x, vertex.y, 1.0, _vCenter);
                multiPoint2.AddVertex(vCoord);*/
            }

            UnE.Geometry.Vertex2D vMin = new UnE.Geometry.Vertex2D(multiPoint.MinX, multiPoint.MinY);
            UnE.Geometry.Vertex2D vMax = new UnE.Geometry.Vertex2D(multiPoint.MaxX, multiPoint.MaxY);
            //UnE.Geometry.Vertex2D vMin = Drawing.BoundingShape.ScaleTransfer(multiPoint.MinX, multiPoint.MinY, 1.0, vCenter);
            //UnE.Geometry.Vertex2D vMax = Drawing.BoundingShape.ScaleTransfer(multiPoint.MaxX, multiPoint.MaxY, 1.0, vCenter);

            multiPoint2.SetBounding(vMin.x, vMax.x, vMin.y, vMax.y);
            multiPoint2.ID = multiPoint.ID;
            multiPoint2.SetAttrib(attr);
            layer.Add(multiPoint2);

            multiPoint2.ShapeInfo = shapeInfo;
        }
    }
}
