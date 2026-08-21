using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DXFViewer;
using System.Drawing;

namespace FireManagement
{
    public class DXFManager
    {
        private static Shape m_shapeFE = null;
        private static Shape m_shapeHD = null;
        private static Shape m_shapeFA = null;

        private static string m_strFireEquipmentZoneName = "소화설비영역";
        private static string m_strFireAlarmZoneName = "발신기영역";

        // 설비영역 텍스트를 표시할 것인가?
        private bool m_showEquipZoneText = false;
        public bool ShowEquipZoneText
        {
            get { return m_showEquipZoneText; }
            set { m_showEquipZoneText = value; }
        }

        // 현재 열려있는 DXF 도면에 대한 전체 설비들
        private ArrayList m_arrEquipments = new ArrayList();
        // 현재 열려있는 DXF 도면에 대한 전체 설비 점검 내용들
        private Dictionary<FireEquipment, FireEquipmentHistory> m_dicEquipmentHistory = new Dictionary<FireEquipment, FireEquipmentHistory>();
        // 삭제할 설비들
        private ArrayList m_arrRemoveEquipments = new ArrayList();

        private Shape m_shapeTemp = null;

        private void FixZoom()
        {
            DXFControl dxf = FormMain2.Instance.DXFControl;
            Size dxfSize = dxf.Size;
            double zoom;

            for (int i = 0; i < 45; i++)
            {
                UnE.Geometry.Vertex2D vCurrent = dxf.ScreenToGlobal(dxfSize.Width / 2, dxfSize.Height / 2);

                if (vCurrent != null)
                {
                    double dZoomValue = dxf.GetViewportWeight();

                    //if (e->Delta < 0)
                    {
                        dZoomValue *= 0.9;
                        if (dZoomValue < 0.0001)
                            dZoomValue = 0.0001;
                    }
                    /*else
                        dZoomValue /= 0.9;*/

                    dxf.Zoom(dZoomValue, vCurrent, true);
                    zoom = dZoomValue;
                }
            }
        }

        // nFECount : 소화기 개수
        // nHDCount : 소화전 개수
        // nFACount : 발신기 개수
        public bool LoadEquipment(string strDXFFilePath, Zone zone, out int nFECount, out int nHDCount, out int nFACount)
        {
            nFECount = nFACount = nHDCount = 0;
            Init();

            FormMain2 frmMain = FormMain2.Instance;
            DXFControl dxf = frmMain.DXFControl;

            if (!dxf.OpenDXF(strDXFFilePath))
                return false;

            //FixZoom();

            Layer layerFE = null, layerHD = null, layerFA = null;

            foreach (Layer layer in dxf.Layers)
            {
                if (layer.LayerName == "FE")
                {
                    //nFECount = LoadEquipment(layer, FireEquipment.EquipmentType.FE);
                    frmMain.SetEquipmentLayer(FireEquipment.EquipmentType.FE, layer);

                    // DXF로부터 읽는 대신 DB로부터 읽는다.
                    // DXF의 데이터는 모두 지운다.
                    layer.RemoveAll();
                    layerFE = layer;
                    layerFE.Frozen = false;
                }
                else if (layer.LayerName == "HD")
                {
                    //nHDCount = LoadEquipment(layer, FireEquipment.EquipmentType.HD);
                    frmMain.SetEquipmentLayer(FireEquipment.EquipmentType.HD, layer);

                    // DXF로부터 읽는 대신 DB로부터 읽는다.
                    // DXF의 데이터는 모두 지운다.
                    layer.RemoveAll();
                    layerHD = layer;
                    layerHD.Frozen = false;
                }
                else if (layer.LayerName == "FA")
                {
                    //nFACount = LoadEquipment(layer, FireEquipment.EquipmentType.FA);
                    frmMain.SetEquipmentLayer(FireEquipment.EquipmentType.FA, layer);

                    // DXF로부터 읽는 대신 DB로부터 읽는다.
                    // DXF의 데이터는 모두 지운다.
                    layer.RemoveAll();
                    layerFA = layer;
                    layerFA.Frozen = false;
                }

                // Text 객체들을 모두 제거
                RemoveText(layer);
            }

            CheckEquipmentLayer(dxf, "FE", ref layerFE);
            CheckEquipmentLayer(dxf, "HD", ref layerHD);
            CheckEquipmentLayer(dxf, "FA", ref layerFA);

            LoadEquipment(zone, layerFE, layerHD, layerFA);
            
            if (m_showEquipZoneText)
                LoadEquipmentZone(zone, dxf);

            return true;
        }

        private void CheckEquipmentLayer(DXFControl dxf, string strLayerName, ref Layer layer)
        {
            if (layer == null)
            {
                layer = new Layer(dxf);
                layer.LayerName = strLayerName;
                dxf.Layers.Add(layer);
            }
        }

        private bool LoadEquipment(Zone zone, Layer layerFE, Layer layerHD, Layer layerFA)
        {
            ArrayList arrEquipments = FormMain2.Instance.IOManager.GetEquipments(zone);
            if (arrEquipments == null)
                return false;

            MakeOriginShapes();

            UnE.Geometry.Vertex2D vMove = FormMain2.Instance.DXFControl.MovedVertex;
            float fUnitFlag = FormMain2.Instance.GetUnitFlag(UnitOfLength.METER);

            // 단위 변환은 FMF를 읽을때 이미 수행하였으므로 다시 할 필요없다.
            /*foreach (FireEquipment equip in arrEquipments)
            {
                if (equip.LinkedShape != null)
                {
                    if (equip.Type == FireEquipment.EquipmentType.FE)
                    {
                        equip.LinkedShape.GetLayer().Remove(equip.LinkedShape);
                        layerFE.Add(equip.LinkedShape);
                    }
                    else if (equip.Type == FireEquipment.EquipmentType.HD)
                    {
                        equip.LinkedShape.GetLayer().Remove(equip.LinkedShape);
                        layerHD.Add(equip.LinkedShape);
                    }
                    else if (equip.Type == FireEquipment.EquipmentType.FA)
                    {
                        equip.LinkedShape.GetLayer().Remove(equip.LinkedShape);
                        layerFA.Add(equip.LinkedShape);
                    }
                }
                equip.SetUnitFlag(fUnitFlag);
                //equip.Position = new PointF(equip.Position.X / fUnitFlag, equip.Position.Y / fUnitFlag);
            }*/
            /*foreach (FireEquipment equip in arrEquipments)
            {
                Shape shape = null;

                if (equip.Type == FireEquipment.EquipmentType.FE)
                {
                    shape = m_shapeFE.Clone();
                    layerFE.Add(shape);
                }
                else if (equip.Type == FireEquipment.EquipmentType.HD)
                {
                    shape = m_shapeHD.Clone();
                    layerHD.Add(shape);
                }
                else if (equip.Type == FireEquipment.EquipmentType.FA)
                {
                    shape = m_shapeFA.Clone();
                    layerFA.Add(shape);
                }
                else
                    continue;

                equip.Position = new PointF(equip.Position.X / fUnitFlag, equip.Position.Y / fUnitFlag);
                
                Hatch hatch = (Hatch)shape;
                hatch.Center = new PointF((float)(equip.Position.X + vMove.x), (float)(equip.Position.Y + vMove.y));

                //equip.Position = new PointF(hatch.Center.X - (float)vMove.x, hatch.Center.Y - (float)vMove.y);
                equip.LinkedShape = shape;
            }*/

            return true;
        }

        private void Init()
        {
            m_arrEquipments.Clear();
            m_dicEquipmentHistory.Clear();
        }

        // Text 객체들을 모두 제거한다.
        private void RemoveText(Layer layer)
        {
            ArrayList arrRemove = new ArrayList();

            foreach (Shape shape in layer.Shapes)
            {
                if (shape.GetShapeType() == Shape.ShapeType.TEXT)
                    arrRemove.Add(shape);
            }

            foreach (Shape shape in arrRemove)
            {
                layer.Shapes.Remove(shape);
            }
        }

        private int LoadEquipment(Layer layer, FireEquipment.EquipmentType type)
        {
            UnE.Geometry.Vertex2D vMove = FormMain2.Instance.DXFControl.MovedVertex;
            int nCount = 0;

            ArrayList arrRemove = new ArrayList();

            IOManager ioMgr = FormMain2.Instance.IOManager;
            Zone zoneCurrent = FormMain2.Instance.CurrentZone;

            foreach (Shape shape in layer.Shapes)
            {
                Block block = shape.GetBlock();
                if (block == null)
                    continue;

                string strObjectID = block.Name;

                if (shape.GetShapeType() == Shape.ShapeType.HATCH)
                {
                    Hatch hatch = (Hatch)shape;

                    FireEquipment equip = ioMgr.FindEquipment(strObjectID, zoneCurrent);

                    if (equip == null)
                    {
                        equip = new FireEquipment();

                        equip.DXFObjID = strObjectID;
                        equip.EquipID = strObjectID;
                        equip.Position = new PointF(hatch.Center.X - (float)vMove.x, hatch.Center.Y - (float)vMove.y);
                        equip.Type = type;
                        equip.Zone = zoneCurrent;
                    }
                    else
                    {
                        equip.Position = new PointF(hatch.Center.X - (float)vMove.x, hatch.Center.Y - (float)vMove.y);
                    }

                    if (!m_arrEquipments.Contains(equip))
                    {
                        m_arrEquipments.Add(equip);
                        nCount++;
                    }

                    arrRemove.Add(shape);
                }
            }

            // 소방설비 객체들은 FireManagement에서 다시 그려줄 예정이므로 원본 객체들은 지운다.
            foreach (Shape shape in arrRemove)
            {
                layer.Shapes.Remove(shape);
            }

            return nCount;
        }

        public bool SetEquipmentZone(Zone zone)
        {
            ArrayList arrInsert = new ArrayList();
            Dictionary<string, FireEquipment> dicFireEquipment = new Dictionary<string, FireEquipment>();
            string strDxfObjIDs = "";
            
            foreach (FireEquipment equip in m_arrEquipments)
            {
                equip.Zone = zone;

                if (strDxfObjIDs.Length == 0)
                    strDxfObjIDs = "'" + equip.DXFObjID + "'";
                else
                    strDxfObjIDs += ", '" + equip.DXFObjID + "'";

                dicFireEquipment[equip.DXFObjID] = equip;
                arrInsert.Add(equip);
            }

            if (strDxfObjIDs.Length == 0)
                return true;

            if (!UpdateEquipmentToDB(strDxfObjIDs, dicFireEquipment, arrInsert, zone))
                return false;

            return InsertEquipmentToDB(arrInsert);
        }

        private bool InsertEquipmentToDB(ArrayList arrInsert)
        {
            float fUnitFlag = FormMain2.Instance.UnitFlag;
            WebDBManager dbMgr = FormMain2.Instance.DBManager;

            string strSQL = "select max(id) from FireEquipment";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            strSQL = "";
            int nMaxID = arrResult.Count == 0 ? 0 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0);

            UnE.Geometry.Vertex2D vMove = FormMain2.Instance.DXFControl.MovedVertex;

            string strFormat = "Insert into FireEquipment (ID, RFIDTag, EquipID, RFIDTagID, DxfObjID, EquipType, ZoneID, X, Y, Z, Description) ";
            strFormat += "values ({0}, NULL, '{1}', NULL, '{2}', {3}, {4}, {5}, {6}, 0, '{7}')";

            foreach (FireEquipment equip in arrInsert)
            {
                if (strSQL.Length == 0)
                    strSQL = string.Format(strFormat, ++nMaxID, equip.EquipID, equip.DXFObjID, (int)equip.Type, equip.Zone.ID, (equip.Position.X - vMove.x) * fUnitFlag, (equip.Position.Y - vMove.y) * fUnitFlag, equip.Description);
                else
                    strSQL += ";" + string.Format(strFormat, ++nMaxID, equip.EquipID, equip.DXFObjID, (int)equip.Type, equip.Zone.ID, (equip.Position.X - vMove.x) * fUnitFlag, (equip.Position.Y - vMove.y) * fUnitFlag, equip.Description);
            }

            if (strSQL.Length == 0)
                return true;

            return dbMgr.GetResultData(strSQL, 0) != null;
        }

        // 기존에 존재하는 데이터를 Update 시킨다.
        private bool UpdateEquipmentToDB(string strDxfObjIDs, Dictionary<string, FireEquipment> dicFireEquipment, ArrayList arrInsert, Zone zone)
        {
            float fUnitFlag = FormMain2.Instance.UnitFlag;
            WebDBManager dbMgr = FormMain2.Instance.DBManager;

            string strSQL = string.Format("select id, DxfObjID from FireEquipment where ZoneID = {0} and DxfObjID in ({1})", zone.ID, strDxfObjIDs);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            UnE.Geometry.Vertex2D vMove = FormMain2.Instance.DXFControl.MovedVertex;

            strSQL = "";
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strDxfObjID = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], "");

                if (!dicFireEquipment.ContainsKey(strDxfObjID))
                    continue;

                FireEquipment equip = dicFireEquipment[strDxfObjID];
                equip.ID = nID;

                if (strSQL.Length == 0)
                    strSQL = string.Format("Update FireEquipment set EquipType = {0}, ZoneID = {1}, X = {2}, Y = {3} where id = {4}",
                        (int)equip.Type, equip.Zone.ID, (equip.Position.X + vMove.x) * fUnitFlag, (equip.Position.Y + vMove.y) * fUnitFlag, nID);
                else
                    strSQL += string.Format(";Update FireEquipment set EquipType = {0}, ZoneID = {1}, X = {2}, Y = {3} where id = {4}",
                        (int)equip.Type, equip.Zone.ID, (equip.Position.X + vMove.x) * fUnitFlag, (equip.Position.Y + vMove.y) * fUnitFlag, nID);

                int nIndex = arrInsert.IndexOf(equip);

                if (nIndex >= 0)
                    arrInsert.RemoveAt(nIndex);
            }

            if (strSQL.Length > 0)
                return dbMgr.GetResultData(strSQL, 0) != null;

            return true;
        }

        private Layer FindLayer(string strLayerName, DXFControl dxf)
        {
            foreach (Layer layer in dxf.Layers)
            {
                if (layer.LayerName == strLayerName)
                    return layer;
            }

            return null;
        }

        public void ClearTeampShape()
        {
            FormMain2 frmMain = FormMain2.Instance;
            DXFControl dxf = frmMain.DXFControl;

            Layer tempLayer = FindLayer("temp", dxf);
            if (tempLayer == null)
                return;

            if (tempLayer.Shapes.Count > 0)
            {
                tempLayer.RemoveAll();
                dxf.Refresh();
            }
        }

        public Shape MakeTempShape(FireEquipment.EquipmentType type, float x, float y)
        {
            FormMain2 frmMain = FormMain2.Instance;
            DXFControl dxf = frmMain.DXFControl;

            if (dxf.Layers.Count == 0)
                return null;

            Layer tempLayer = FindLayer("temp", dxf);

            if (tempLayer == null)
            {
                Layer firstLayer = (Layer)dxf.Layers[0];
                tempLayer = new Layer(firstLayer.Owner);
                tempLayer.LayerName = "temp";
                dxf.Layers.Add(tempLayer);
            }

            string strTypeName = FireEquipment.GetTypeName(type);

            if (m_shapeTemp != null)
            {
                if ((string)m_shapeTemp.Tag != strTypeName)
                {
                    tempLayer.Remove(m_shapeTemp);
                    m_shapeTemp = NewEquipmentShape(type, tempLayer);
                }
            }
            else
                m_shapeTemp = NewEquipmentShape(type, tempLayer);
        
            if (m_shapeTemp == null)
                return null;

            UnE.Geometry.Vertex2D vMove = FormMain2.Instance.DXFControl.MovedVertex;
            m_shapeTemp.Move(x + vMove.x, y + vMove.y);

            return m_shapeTemp;
        }

        public Shape AddEquipmentObjectToDXF(FireEquipment equip)
        {
            /*FormMain frmMain = FormMain2.Instance;
            DXFViewer.Layer layer = frmMain.GetEquipmentLayer(equip.Type);

            if (layer == null)
                return;

            MakeOriginShapes();

            UnE.Geometry.Vertex2D vMove = frmMain.DXFControl.MovedVertex;
            Shape shape = null;

            if (equip.Type == FireEquipment.EquipmentType.FE)
                shape = m_shapeFE.Clone();
            else if (equip.Type == FireEquipment.EquipmentType.HD)
                shape = m_shapeHD.Clone();
            else if (equip.Type == FireEquipment.EquipmentType.FA)
                shape = m_shapeFA.Clone();
            else
                return;

            shape.Selectable = true;*/
            ClearTeampShape();

            Shape shape = NewEquipmentShape(equip.Type);
            if (shape == null)
                return null;

            shape.ID = equip.ID;
            equip.LinkedShape = shape;

            UnE.Geometry.Vertex2D vMove = FormMain2.Instance.DXFControl.MovedVertex;
            shape.Move(equip.Position.X + vMove.x, equip.Position.Y + vMove.y);
            //layer.Add(shape);
            return shape;
        }

        private static void MakeOriginShapes()
        {
            if (m_shapeFE != null)
                return;

            // 소화기
            DXFViewer.Hatch hatchFE = new DXFViewer.Hatch();
            
            int nPointSize = 30;
            hatchFE.SetPointSize(nPointSize);

            double dAngleFE = System.Math.PI * 2 / nPointSize;
            double dRadiusFE = 1250 / 2;

            for (int i = 0; i < nPointSize; i++)
            {
                double x = -dRadiusFE * System.Math.Sin(dAngleFE * i);
                double y = dRadiusFE * System.Math.Cos(dAngleFE * i);
                hatchFE.UpdatePoint(i, (float)x, (float)y);
            }

            hatchFE.SetOwnColor(Color.LightGreen);
            hatchFE.SetColorOption(Shape.ControlType.BYOWN);
            
            m_shapeFE = hatchFE;

            // 소화전
            DXFViewer.Hatch hatchHD = new DXFViewer.Hatch();

            hatchHD.SetPointSize(4);
            hatchHD.UpdatePoint(0, -1800, -900);
            hatchHD.UpdatePoint(1, 1800, -900);
            hatchHD.UpdatePoint(2, 1800, 900);
            hatchHD.UpdatePoint(3, -1800, 900);

            hatchHD.SetOwnColor(Color.Brown);
            hatchHD.SetColorOption(Shape.ControlType.BYOWN);

            m_shapeHD = hatchHD;

            // 발신기
            DXFViewer.Hatch hatchFA = new DXFViewer.Hatch();

            double dTriangleSize = 2500;
            double root3 = System.Math.Pow(3, 0.5);

            hatchFA.SetPointSize(3);
            hatchFA.UpdatePoint(0, 0, (float)(dTriangleSize / root3));
            hatchFA.UpdatePoint(1, (float)(-dTriangleSize / 2), (float)(-dTriangleSize / 2 / root3));
            hatchFA.UpdatePoint(2, (float)(dTriangleSize / 2), (float)(-dTriangleSize / 2 / root3));
            /*double dRadiusFA = 2000;
            // 별을 그리기 위한 원의 5등분 각도
            double dAngleFA = UnE.Geometry.Math.DegToRad(72.0);

            UnE.Geometry.Vertex2D v1 = new UnE.Geometry.Vertex2D(0.0, dRadiusFA);
            UnE.Geometry.Vertex2D v2 = new UnE.Geometry.Vertex2D(-dRadiusFA * System.Math.Sin(dAngleFA), dRadiusFA * System.Math.Cos(dAngleFA));
            UnE.Geometry.Vertex2D v3 = new UnE.Geometry.Vertex2D(-dRadiusFA * System.Math.Sin(dAngleFA * 2), dRadiusFA * System.Math.Cos(dAngleFA * 2));
            UnE.Geometry.Vertex2D v4 = new UnE.Geometry.Vertex2D(-dRadiusFA * System.Math.Sin(dAngleFA * 3), dRadiusFA * System.Math.Cos(dAngleFA * 3));
            UnE.Geometry.Vertex2D v5 = new UnE.Geometry.Vertex2D(-dRadiusFA * System.Math.Sin(dAngleFA * 4), dRadiusFA * System.Math.Cos(dAngleFA * 4));

            UnE.Geometry.Line2D line1 = new UnE.Geometry.Line2D(v1, v3, UnE.Geometry.Line2D.LineType.SEGMENT);
            UnE.Geometry.Line2D line2 = new UnE.Geometry.Line2D(v2, v4, UnE.Geometry.Line2D.LineType.SEGMENT);
            UnE.Geometry.Line2D line3 = new UnE.Geometry.Line2D(v3, v5, UnE.Geometry.Line2D.LineType.SEGMENT);
            UnE.Geometry.Line2D line4 = new UnE.Geometry.Line2D(v4, v1, UnE.Geometry.Line2D.LineType.SEGMENT);
            UnE.Geometry.Line2D line5 = new UnE.Geometry.Line2D(v5, v2, UnE.Geometry.Line2D.LineType.SEGMENT);

            UnE.Geometry.Vertex2D vInner1, vInner2, vInner3, vInner4, vInner5, vTemp;
            UnE.Geometry.Line2D.LineType resultType;

            line1.IntersectLine(line5, out vInner1, out vTemp, out resultType);
            line2.IntersectLine(line1, out vInner2, out vTemp, out resultType);
            line3.IntersectLine(line2, out vInner3, out vTemp, out resultType);
            line4.IntersectLine(line3, out vInner4, out vTemp, out resultType);
            line5.IntersectLine(line4, out vInner5, out vTemp, out resultType);

            DXFViewer.Hatch hatchFA = new DXFViewer.Hatch();

            hatchFA.SetPointSize(10);

            hatchFA.UpdatePoint(0, (float)v1.x, (float)v1.y);
            hatchFA.UpdatePoint(1, (float)vInner1.x, (float)vInner1.y);
            hatchFA.UpdatePoint(2, (float)v2.x, (float)v2.y);
            hatchFA.UpdatePoint(3, (float)vInner2.x, (float)vInner2.y);
            hatchFA.UpdatePoint(4, (float)v3.x, (float)v3.y);
            hatchFA.UpdatePoint(5, (float)vInner3.x, (float)vInner3.y);
            hatchFA.UpdatePoint(6, (float)v4.x, (float)v4.y);
            hatchFA.UpdatePoint(7, (float)vInner4.x, (float)vInner4.y);
            hatchFA.UpdatePoint(8, (float)v5.x, (float)v5.y);
            hatchFA.UpdatePoint(9, (float)vInner5.x, (float)vInner5.y);*/

            hatchFA.SetOwnColor(Color.Yellow);
            hatchFA.SetColorOption(Shape.ControlType.BYOWN);

            m_shapeFA = hatchFA;
        }

        private DXFViewer.Shape NewEquipmentShape(FireEquipment.EquipmentType type, DXFViewer.Layer layer = null)
        {
            FormMain2 frmMain = FormMain2.Instance;
            //DXFViewer.Layer layer = frmMain.GetEquipmentLayer(type);

            if (layer == null)
                layer = frmMain.GetEquipmentLayer(type);

            if (layer != null && !frmMain.DXFControl.Layers.Contains(layer))
                layer = null;
            
            if (layer == null)
            {
                layer = new Layer(frmMain.DXFControl);

                if (type == FireEquipment.EquipmentType.FE)
                    layer.LayerName = "FE";
                else if (type == FireEquipment.EquipmentType.HD)
                    layer.LayerName = "HD";
                else if (type == FireEquipment.EquipmentType.FA)
                    layer.LayerName = "FA";
                else
                    return null;

                frmMain.SetEquipmentLayer(type, layer);
                frmMain.DXFControl.Layers.Add(layer);
            }

            MakeOriginShapes();

            //UnE.Geometry.Vertex2D vMove = frmMain.DXFControl.MovedVertex;
            Shape shape = null;

            if (type == FireEquipment.EquipmentType.FE)
                shape = m_shapeFE.Clone();
            else if (type == FireEquipment.EquipmentType.HD)
                shape = m_shapeHD.Clone();
            else if (type == FireEquipment.EquipmentType.FA)
                shape = m_shapeFA.Clone();
            else
                return null;

            shape.Selectable = true;
            layer.Add(shape);

            return shape;
        }

        public FireEquipment FindEquipment(string strRFID, FireEquipment exceptEquip = null)
        {
            foreach (FireEquipment equip in m_arrEquipments)
            {
                if (equip == exceptEquip)
                    continue;

                if (equip.RFIDTag == strRFID)
                    return equip;
            }

            return null;
        }

        public FireEquipment FindEquipment(int nEquipmentID)
        {
            foreach (FireEquipment equip in m_arrEquipments)
            {
                if (equip.ID == nEquipmentID)
                    return equip;
            }

            return null;
        }

        public void LoadZoneEquipments(Zone zone)
        {
            ArrayList arrDBEquipments = FormMain2.Instance.IOManager.GetEquipments(zone);
            if (arrDBEquipments == null || arrDBEquipments.Count == 0)
                return;

            // DxfObjID별 Equipment
            Dictionary<string, FireEquipment> dicEquipments = new Dictionary<string, FireEquipment>();

            foreach (FireEquipment equip in m_arrEquipments)
            {
                dicEquipments[equip.DXFObjID] = equip;
            }

            foreach (FireEquipment equipSrc in arrDBEquipments)
            {
                if (dicEquipments.ContainsKey(equipSrc.DXFObjID))
                {
                    // DB 데이터 가운데 DXF 데이터와 일치하는것이 있는지 검사한다.
                    FireEquipment equipTrg = dicEquipments[equipSrc.DXFObjID];

                    equipTrg.ID = equipSrc.ID;
                    equipTrg.Description = equipSrc.Description;
                    equipTrg.EquipID = equipSrc.EquipID;
                    equipTrg.RFIDTag = equipSrc.RFIDTag;
                    equipTrg.RFIDTagID = equipSrc.RFIDTagID;
                    equipTrg.Type = equipSrc.Type;
                    equipTrg.Zone = equipSrc.Zone;
                }
                else
                {
                    // DB 데이터 가운데 DXF 데이터와 일치하지 않는것은 새로 추가해 준다.
                    m_arrEquipments.Add(equipSrc);
                }
            }
        }

        private bool UpdateEquipments(Zone zone, ArrayList arrZoneEquipIDs, out ArrayList arrRemovedEquipIDs, out ArrayList arrNewEquipments)
        {
            arrNewEquipments = new ArrayList();
            arrRemovedEquipIDs = new ArrayList();

            foreach (object equipID in arrZoneEquipIDs)
            {
                int nID = WebDBManager.GetIntField(equipID.ToString(), -1);

                if (nID < 0)
                    return false;

                arrRemovedEquipIDs.Add(nID);
            }

            ArrayList arrEquipments = FormMain2.Instance.IOManager.GetEquipments(zone);
            if (arrEquipments == null)
                return false;

            string strFormatUpdate = "Update FireEquipment set RFIDTag = {0}, EquipID = '{1}', RFIDTagID = '{2}', DxfObjID = '{3}', EquipType = {4}, ZoneID = {5}, X = {6}, Y = {7}, Description = '{8}' where ID = {9}";

            float fUnitFlag = FormMain2.Instance.GetUnitFlag(UnitOfLength.METER);

            //foreach (FireEquipment equip in m_arrEquipments)
            foreach (FireEquipment equip in arrEquipments)
            {
                string strRFIDTagID = equip.RFIDTagID;
                string strDescription = equip.Description;

                CheckQuotation(ref strRFIDTagID);
                CheckQuotation(ref strDescription);

                if (equip.ID > 0 && arrRemovedEquipIDs.Contains(equip.ID))
                {
                    arrRemovedEquipIDs.Remove(equip.ID);

                    string strSQL = string.Format(strFormatUpdate, equip.RFIDTag.Length == 0 ? "NULL" : "'" + equip.RFIDTag + "'", equip.EquipID, strRFIDTagID, equip.DXFObjID, (int)equip.Type,
                        zone.ID, (double)(equip.Position.X * fUnitFlag), (double)(equip.Position.Y * fUnitFlag), strDescription, equip.ID);

                    if (FormMain2.Instance.DBManager.GetBatchData(strSQL) == null)
                        return false;

                    /*if (strUpdateQuery.Length == 0)
                        strUpdateQuery = strSQL;
                    else
                        strUpdateQuery += ";" + strSQL;*/
                }
                else
                    arrNewEquipments.Add(equip);
            }

            return true;
        }

        private bool UpdateEquipments()
        {
            //string strUpdateQuery = "";
            string strFormatUpdate = "Update FireEquipment set RFIDTag = {0}, EquipID = '{1}', RFIDTagID = '{2}', DxfObjID = '{3}', EquipType = {4}, ZoneID = {5}, X = {6}, Y = {7}, Description = '{8}' where ID = {9}";

            float fUnitFlag = FormMain2.Instance.GetUnitFlag(UnitOfLength.METER);

            foreach (FireEquipment equip in m_arrEquipments)
            {
                if (equip.Zone == null)
                    equip.Zone = FormMain2.Instance.CurrentZone;

                string strRFIDTagID = equip.RFIDTagID;
                string strDescription = equip.Description;

                CheckQuotation(ref strRFIDTagID);
                CheckQuotation(ref strDescription);

                if (equip.ID > 0)
                {
                    string strSQL = string.Format(strFormatUpdate, equip.RFIDTag.Length == 0 ? "NULL" : "'" + equip.RFIDTag + "'", equip.EquipID, strRFIDTagID, equip.DXFObjID, (int)equip.Type,
                        equip.Zone.ID, (double)(equip.Position.X * fUnitFlag), (double)(equip.Position.Y * fUnitFlag), strDescription, equip.ID);

                    if (FormMain2.Instance.DBManager.GetBatchData(strSQL) == null)
                        return false;

                    /*if (strUpdateQuery.Length == 0)
                        strUpdateQuery = strSQL;
                    else
                        strUpdateQuery += ";" + strSQL;*/
                }
            }

            return true;
            /*if (strUpdateQuery.Length == 0)
                return true;

            return FormMain2.Instance.DBManager.GetResultData(strUpdateQuery, 0) != null;*/
        }

        // 문자열의 가운데에 ['] 가 있으면 ['']으로 바꿔준다.
        private void CheckQuotation(ref string strField)
        {
            int nBeginIndex = 0;
            int nIndex = strField.IndexOf('\'', nBeginIndex);

            ArrayList arrQuotation = new ArrayList();

            while (nIndex >= 0)
            {
                arrQuotation.Add(nIndex);
                nBeginIndex = nIndex + 1;
                nIndex = strField.IndexOf('\'', nBeginIndex);
            }

            int nArrSize = arrQuotation.Count;

            for (int i = nArrSize - 1; i >= 0; i--)
            {
                int nQuotationIndex = (int)arrQuotation[i];
                strField = strField.Insert(nQuotationIndex, "'");
            }
        }

        private bool RemoveEquipments(ArrayList arrRemovedEquipIDs)
        {
            string strIDs = "";

            foreach (int nEquipID in arrRemovedEquipIDs)
            {
                if (strIDs.Length == 0)
                    strIDs = nEquipID.ToString();
                else
                    strIDs += ", " + nEquipID.ToString();
            }

            if (strIDs.Length == 0)
                return true;

            strIDs = "(" + strIDs + ")";

            string strSQL = "delete from FireEquipmentHistory where FireEquipmentID in " + strIDs;
            if (FormMain2.Instance.DBManager.GetBatchData(strSQL) == null)
                return false;

            strSQL = "delete from FireEquipmentSignal where EquipmentID in " + strIDs;
            if (FormMain2.Instance.DBManager.GetBatchData(strSQL) == null)
                return false;

            strSQL = "delete from FireEquipmentGroup where linkedEquipID in " + strIDs;
            if (FormMain2.Instance.DBManager.GetBatchData(strSQL) == null)
                return false;

            strSQL = "delete from FireEquipment where ID in " + strIDs;
            if (FormMain2.Instance.DBManager.GetBatchData(strSQL) == null)
                return false;

            m_arrRemoveEquipments.Clear();
            return true;
        }

        private bool RemoveEquipments()
        {
            string strIDs = "";

            foreach (FireEquipment equip in m_arrRemoveEquipments)
            {
                if (strIDs.Length == 0)
                    strIDs = equip.ID.ToString();
                else
                    strIDs += ", " + equip.ID.ToString();
            }

            if (strIDs.Length == 0)
                return true;

            strIDs = "(" + strIDs + ")";

            string strSQL = "delete from FireEquipmentHistory where FireEquipmentID in " + strIDs;
            //if (FormMain2.Instance.DBManager.GetResultData(strSQL, 0) == null)
            if (FormMain2.Instance.DBManager.GetBatchData(strSQL) == null)
                return false;

            strSQL = "delete from FireEquipmentSignal where EquipmentID in " + strIDs;
            //if (FormMain2.Instance.DBManager.GetResultData(strSQL, 0) == null)
            if (FormMain2.Instance.DBManager.GetBatchData(strSQL) == null)
                return false;

            strSQL = "delete from FireEquipment where ID in " + strIDs;
            //if (FormMain2.Instance.DBManager.GetResultData(strSQL, 0) == null)
            if (FormMain2.Instance.DBManager.GetBatchData(strSQL) == null)
                return false;

            m_arrRemoveEquipments.Clear();
            return true;
        }

        private bool InsertEquipments(Zone zone, Dictionary<int, Zone> dicAllEquipIDs, ArrayList arrNewEquipments, ref int nMaxID)
        {
            string strFormatInsert = "Insert into FireEquipment (ID, RFIDTag, EquipID, RFIDTagID, DxfObjID, EquipType, ZoneID, X, Y, Z, Description) ";
            strFormatInsert += "values ({0}, {1}, '{2}', '{3}', '{4}', {5}, {6}, {7}, {8}, 0, '{9}')";

            float fUnitFlag = FormMain2.Instance.GetUnitFlag(UnitOfLength.METER);
            int nEquipID = -1;
            
            foreach (FireEquipment equip in arrNewEquipments)
            {
                if (equip.ID > 0 && !dicAllEquipIDs.ContainsKey(equip.ID))
                    nEquipID = equip.ID;
                else
                    nEquipID = ++nMaxID;

                string strSQL = string.Format(strFormatInsert, nEquipID, equip.RFIDTag.Length == 0 ? "NULL" : "'" + equip.RFIDTag + "'", equip.EquipID, equip.RFIDTagID, equip.DXFObjID,
                    (int)equip.Type, zone.ID, (double)(equip.Position.X * fUnitFlag), (double)(equip.Position.Y * fUnitFlag), equip.Description);

                if (FormMain2.Instance.DBManager.GetBatchData(strSQL) == null)
                    return false;

                equip.ID = nEquipID;
            }

            return true;
        }

        private bool InsertEquipments(int nMaxID)
        {
            string strFormatInsert = "Insert into FireEquipment (ID, RFIDTag, EquipID, RFIDTagID, DxfObjID, EquipType, ZoneID, X, Y, Z, Description) ";
            strFormatInsert += "values ({0}, {1}, '{2}', '{3}', '{4}', {5}, {6}, {7}, {8}, 0, '{9}')";

            float fUnitFlag = FormMain2.Instance.GetUnitFlag(UnitOfLength.METER);

            foreach (FireEquipment equip in m_arrEquipments)
            {
                if (equip.Zone == null)
                    continue;

                if (equip.ID < 0)
                {
                    string strSQL = string.Format(strFormatInsert, ++nMaxID, equip.RFIDTag.Length == 0 ? "NULL" : "'" + equip.RFIDTag + "'", equip.EquipID, equip.RFIDTagID, equip.DXFObjID,
                        (int)equip.Type, equip.Zone.ID, (double)(equip.Position.X * fUnitFlag), (double)(equip.Position.Y * fUnitFlag), equip.Description);

                    //if (FormMain2.Instance.DBManager.GetResultData(strSQL, 0) == null)
                    if (FormMain2.Instance.DBManager.GetBatchData(strSQL) == null)
                        return false;

                    equip.ID = nMaxID;
                }
            }

            return true;
        }

        private int GetMaxID(string strTableName)
        {
            WebDBManager dbMgr = FormMain2.Instance.DBManager;

            string strSQL = "select max(id) from " + strTableName;
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);
            if (arrResult == null)
                return -1;

            if (arrResult.Count == 0)
                return 0;

            return DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }

        private bool InsertEquipmentHistory(Zone zone, ref int nMaxID)
        {
            IOManager ioMgr = FormMain2.Instance.IOManager;

            ArrayList arrEquipments = ioMgr.GetEquipments(zone);
            if (arrEquipments == null)
                return false;

            string strFormatInsert = "Insert into FireEquipmentHistory (ID, FireEquipmentID, SOPGenUserID, Time, Status, CheckersOpinion, Description) ";
            strFormatInsert += "values ({0}, {1}, {2}, '{3} {4}:{5}:{6}', {7}, '{8}', '{9}')";

            int nUserID = FormMain2.Instance.SOPGenUserID;

            foreach (FireEquipment equip in arrEquipments)
            {
                ArrayList arrEquipHistory = ioMgr.FindEquipmentHistoryList(equip.ID);
                if (arrEquipHistory == null)
                    continue;

                foreach (FireEquipmentHistory history in arrEquipHistory)
                {
                    // 이미 DB에 기록되었는지 검사
                    if (history.ID > 0)
                        continue;

                    if (history.EquipmentID < 0)
                        continue;

                    string strSQL = string.Format(strFormatInsert, ++nMaxID, history.EquipmentID, nUserID, history.Time.ToShortDateString(),
                    history.Time.Hour, history.Time.Minute, history.Time.Second, (int)history.Status, history.CheckersOpinion, history.Description);

                    if (FormMain2.Instance.DBManager.GetBatchData(strSQL) == null)
                        return false;

                    history.ID = nMaxID;
                }
            }

            return true;
        }

        private bool InsertEquipmentHistory(int nMaxID)
        {
            string strFormatInsert = "Insert into FireEquipmentHistory (ID, FireEquipmentID, SOPGenUserID, Time, Status, CheckersOpinion, Description) ";
            strFormatInsert += "values ({0}, {1}, {2}, '{3} {4}:{5}:{6}', {7}, '{8}', '{9}')";

            int nUserID = FormMain2.Instance.SOPGenUserID;

            /*Dictionary<int, ArrayList> dicEquipmentHistory = FormMain2.Instance.IOManager.EquipmentHistory;

            foreach (KeyValuePair<int, ArrayList> pair in dicEquipmentHistory)
            {
                if (pair.Key < 0)
                    continue;

                ArrayList arrHistory = pair.Value;

                foreach (FireEquipmentHistory history in arrHistory)
                {
                    // 이미 DB에 기록되었는지 검사
                    if (history.ID > 0)
                        continue;

                    if (history.EquipmentID < 0)
                        continue;

                    string strSQL = string.Format(strFormatInsert, ++nMaxID, history.EquipmentID, nUserID, history.Time.ToShortDateString(),
                    history.Time.Hour, history.Time.Minute, history.Time.Second, (int)history.Status, history.CheckersOpinion, history.Description);

                    if (FormMain2.Instance.DBManager.GetResultData(strSQL, 0) == null)
                        return false;

                    history.ID = nMaxID;
                }
            }*/

            foreach (KeyValuePair<FireEquipment, FireEquipmentHistory> pair in m_dicEquipmentHistory)
            {
                FireEquipmentHistory history = pair.Value;

                // 이미 DB에 기록되었는지 검사
                if (history.ID > 0)
                    continue;

                if (history.EquipmentID < 0)
                    continue;

                string strSQL = string.Format(strFormatInsert, ++nMaxID, history.EquipmentID, nUserID, history.Time.ToShortDateString(),
                    history.Time.Hour, history.Time.Minute, history.Time.Second, (int)history.Status, history.CheckersOpinion, history.Description);

                //if (FormMain2.Instance.DBManager.GetResultData(strSQL, 0) == null)
                if (FormMain2.Instance.DBManager.GetBatchData(strSQL) == null)
                    return false;

                history.ID = nMaxID;
            }

            return true;
        }

        private ArrayList GetZoneEquipIDs(Zone zone)
        {
            WebDBManager dbMgr = FormMain2.Instance.DBManager;
            string strSQL = "Select id from FireEquipment where ZoneID = " + zone.ID.ToString();

            return dbMgr.GetBatchData(strSQL);
        }

        // dicAllEquipIDs : EquipID, Zone
        // dicZoneEquipIDs : Zone별 EquipID List
        private bool GetAllEquipIDs(Dictionary<int, Zone> dicAllEquipIDs, Dictionary<Zone, ArrayList> dicZoneEquipIDs)
        {
            WebDBManager dbMgr = FormMain2.Instance.DBManager;
            string strSQL = "Select id, ZoneID from FireEquipment";

            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return false;

            IOManager ioMgr = FormMain2.Instance.IOManager;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i+=2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nZoneID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);

                if (nID < 0 || nZoneID < 0)
                    continue;

                Zone zone = ioMgr.FindZone(nZoneID);

                if (zone == null)
                    continue;

                dicAllEquipIDs[nID] = zone;

                if (dicZoneEquipIDs.ContainsKey(zone))
                {
                    ArrayList arrEquipIDs = dicZoneEquipIDs[zone];
                    arrEquipIDs.Add(nID);
                }
                else
                {
                    ArrayList arrEquipIDs = new ArrayList();
                    arrEquipIDs.Add(nID);
                    dicZoneEquipIDs[zone] = arrEquipIDs;
                }
            }

            return true;
        }

        public bool SaveToDB()
        {
            FormMain2 frmMain = FormMain2.Instance;
            IOManager ioMgr = frmMain.IOManager;

            if (frmMain.CurrentZone != null)
                ioMgr.CompareZoneEquipmentsToDB(frmMain.CurrentZone);

            WebDBManager dbMgr = frmMain.DBManager;
            dbMgr.BeginBatch();

            Dictionary<int, Zone> dicAllEquipIDs = new Dictionary<int,Zone>();
            Dictionary<Zone, ArrayList> dicZoneEquipIDs = new Dictionary<Zone, ArrayList>();

            if (!GetAllEquipIDs(dicAllEquipIDs, dicZoneEquipIDs))
                goto BATCH_FAIL;

            int nMaxID = GetMaxID("FireEquipment");
            int nMaxHistoryID = GetMaxID("FireEquipmentHistory");

            if (nMaxID < 0 || nMaxHistoryID < 0)
                goto BATCH_FAIL;

            foreach (Zone zone in ioMgr.ChangedZones)
            {
                if (!dicZoneEquipIDs.ContainsKey(zone))
                {
                    ArrayList arrNewEquips = ioMgr.GetEquipments(zone);

                    if (!InsertEquipments(zone, dicAllEquipIDs, arrNewEquips, ref nMaxID))
                        goto BATCH_FAIL;

                    continue;
                }
                /*if (!dicZoneEquipIDs.ContainsKey(zone))
                    goto BATCH_FAIL;*/

                ArrayList arrRemovedEquipIDs = null, arrNewEquipments = null;
                //ArrayList arrZoneEquipIDs = GetZoneEquipIDs(zone);
                ArrayList arrZoneEquipIDs = dicZoneEquipIDs[zone];

                if (!UpdateEquipments(zone, arrZoneEquipIDs, out arrRemovedEquipIDs, out arrNewEquipments))
                    goto BATCH_FAIL;

                if (!RemoveEquipments(arrRemovedEquipIDs))
                    goto BATCH_FAIL;

                if (!InsertEquipments(zone, dicAllEquipIDs, arrNewEquipments, ref nMaxID))
                    goto BATCH_FAIL;

                if (!InsertEquipmentHistory(zone, ref nMaxHistoryID))
                    goto BATCH_FAIL;
            }

            /*if (!UpdateEquipments())
                goto BATCH_FAIL;

            if (!RemoveEquipments())
                goto BATCH_FAIL;

            int nMaxID = GetMaxID("FireEquipment");
            if (nMaxID < 0)
                goto BATCH_FAIL;

            if (!InsertEquipments(nMaxID))
                goto BATCH_FAIL;

            nMaxID = GetMaxID("FireEquipmentHistory");

            if (!InsertEquipmentHistory(nMaxID))
                goto BATCH_FAIL;*/

            dbMgr.BatchCommit();

            ioMgr.ChangedZones.Clear();

            return true;

        BATCH_FAIL:
            dbMgr.BatchRollback();
            return false;
        }

        public void DeleteEquipment(FireEquipment equip)
        {
            m_arrEquipments.Remove(equip);

            if (m_dicEquipmentHistory.ContainsKey(equip))
                m_dicEquipmentHistory.Remove(equip);

            if (equip.ID > 0 && !m_arrRemoveEquipments.Contains(equip))
                m_arrRemoveEquipments.Add(equip);
        }

        private void LoadEquipmentZone(Zone zone, DXFControl dxf)
        {
            ArrayList arrEquipZones = FormMain2.Instance.IOManager.GetEquipmentZoneList(zone);

            if (arrEquipZones == null)
                return;

            Layer layerSensor = GetLayer(EquipmentZone.EquipZoneType.SENSOR_TYPE, dxf);
            Layer layerFA = GetLayer(EquipmentZone.EquipZoneType.FA_TYPE, dxf);

            if (layerSensor == null)
            {
                layerSensor = new Layer(dxf);
                layerSensor.LayerName = m_strFireEquipmentZoneName;
                dxf.Layers.Add(layerSensor);
            }

            if (layerFA == null)
            {
                layerFA = new Layer(dxf);
                layerFA.LayerName = m_strFireAlarmZoneName;
                dxf.Layers.Add(layerFA);
            }

            layerSensor.RemoveAll();
            layerFA.RemoveAll();

            layerSensor.LineColor = Color.Green;
            layerFA.LineColor = Color.Red;

            UnE.Geometry.Vertex2D vMove = FormMain2.Instance.DXFControl.MovedVertex;
            float fUnitFlag = FormMain2.Instance.GetUnitFlag(UnitOfLength.METER);

            UnE.Geometry.Vertex2D vCenter = null;

            foreach (EquipmentZone equipZone in arrEquipZones)
            {
                bool isHide = equipZone.NotShowingZone.Contains(zone);

                if (isHide)
                    continue;

                if (!equipZone.ZoneTextCenter.ContainsKey(zone))
                    vCenter = equipZone.Polygon.CalcWeightCenter();
                else
                    vCenter = equipZone.ZoneTextCenter[zone];

                vCenter.SetVertex(vCenter.x / fUnitFlag + vMove.x, vCenter.y / fUnitFlag + vMove.y);

                Text text = new Text();

                text.HorizontalAlignment = StringAlignment.Center;
                text.VerticalAlignment = StringAlignment.Center;
                text.Title = equipZone.ZoneName;
                text.SetPosition(vCenter);
                text.Font = new Font(text.Font.FontFamily, 640.0f);
                text.Tag = equipZone;

                if (equipZone.ZoneType == EquipmentZone.EquipZoneType.SENSOR_TYPE)
                {
                    layerSensor.Add(text);
                }
                else if (equipZone.ZoneType == EquipmentZone.EquipZoneType.FA_TYPE)
                {
                    layerFA.Add(text);
                }
            }
        }

        static public Layer GetLayer(EquipmentZone.EquipZoneType type, DXFControl dxf)
        {
            string strLayerName = "";

            if (type == EquipmentZone.EquipZoneType.SENSOR_TYPE)    // 소화설비 영역
                strLayerName = m_strFireEquipmentZoneName;
            else if (type == EquipmentZone.EquipZoneType.FA_TYPE)   // 발신기 영역
                strLayerName = m_strFireAlarmZoneName;
            else
                return null;

            foreach (Layer layer in dxf.Layers)
            {
                if (layer.LayerName == strLayerName)
                    return layer;
            }

            return null;
        }

        public Dictionary<FireEquipment, FireEquipmentHistory> EquipmentHistory
        {
            get { return m_dicEquipmentHistory; }
        }

        public ArrayList Equipments
        {
            get { return m_arrEquipments; }
        }
    }
}
