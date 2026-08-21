using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace DXFUtility
{
    public class DXFEquipZoneDBInput
    {
        private string m_strFolderPath = "";
        private WebDBManager m_dbMgr = null;
        private System.IO.StreamWriter m_writerInvalid = null;
        private System.IO.StreamWriter m_writerValid = null;

        public DXFEquipZoneDBInput(string strFolderPath, WebDBManager dbMgr)
        {
            m_strFolderPath = strFolderPath;
            m_dbMgr = dbMgr;
        }

        public bool Run()
        {
            m_writerInvalid = new System.IO.StreamWriter(Application.StartupPath + "\\Invalid.log", false, Encoding.UTF8);
            m_writerValid = new System.IO.StreamWriter(Application.StartupPath + "\\Valid.log", false, Encoding.UTF8);

            bool result = Run2();

            m_writerInvalid.Close();
            m_writerValid.Close();

            return result;
        }

        private bool Run2()
        {
            /*string strSQL = "delete from EquipmentZone";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;*/
            string strSQL;
            ArrayList arrResult;

            int nID = 0;
            int nLen = m_strFolderPath.Length;
            string[] arrFolders = System.IO.Directory.GetDirectories(m_strFolderPath);

            DXFViewer.DXFControl dxf = new DXFViewer.DXFControl();
            float fUnitFlag = DXFDBInput2.GetUnitFlag(DXFViewer.UnitOfLength.METER, dxf);

            // Key : EquipZone Name
            // Value : Linked Zone ID List
            Dictionary<string, ArrayList> dicZoneIDList = LoadEquipZones(ref nID);

            foreach (string strFolderPath in arrFolders)
            {
                string strFolderName = strFolderPath.Substring(nLen + 1);

                int nIndex = strFolderPath.IndexOf('_', nLen + 1);
                string strBuildingID = strFolderPath.Substring(nLen + 1, nIndex - (nLen + 1));

                strSQL = string.Format("select id from Building where BuildingID = '{0}'", strBuildingID);
                arrResult = m_dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null)
                    return false;

                if (arrResult.Count == 0)
                    continue;

                int nBuildingID = m_dbMgr.GetIntField(arrResult[0].ToString(), -1);

                string[] arrFiles = System.IO.Directory.GetFiles(strFolderPath);
                //int nAddFloor = 0;

                foreach (string strFilePath in arrFiles)
                {
                    string strAddFloorCondition = "is NULL";
                    int nDotIndex = strFilePath.LastIndexOf('.');
                    string strExt = strFilePath.Substring(nDotIndex + 1);

                    if (string.Compare(strExt, "dxf", true) != 0)
                        continue;

                    FormMain.Instance.Text = strFilePath;

                    nIndex = strFilePath.LastIndexOf('_');
                    string strFloor = strFilePath.Substring(nIndex + 1, nDotIndex - (nIndex + 1));

                    // 층표시가 되어있지 않는 경우
                    if (strFloor.Length > 3)
                        strFloor = "1";
                    else
                    {
                        nIndex = strFloor.IndexOf('M');
                        if (nIndex >= 0)
                        {
                            // 'M'은 무시한다.
                            //strAddFloorCondition = "= '0.5'";
                            //nAddFloor++;
                            strFloor = strFloor.Substring(0, nIndex);
                        }

                        nIndex = strFloor.IndexOf('.');
                        if (nIndex >= 0)
                        {
                            strAddFloorCondition = "= '0" + strFloor.Substring(nIndex) + "'";
                            //nAddFloor++;
                            strFloor = strFloor.Substring(0, nIndex);
                        }
                    }

                    int nFloorIndex;

                    /*if (arrFiles.Count() == 1)
                        nFloorIndex = 0;
                    else*/
                    {
                        if (strFloor.Contains('B'))
                            nFloorIndex = -(int.Parse(strFloor.Substring(1)));
                        else
                            nFloorIndex = int.Parse(strFloor) - 1;
                    }

                    nIndex = strFilePath.LastIndexOf('\\');
                    string strFileName = strFilePath.Substring(nIndex);

                    strSQL = string.Format("select id from Zone where BuildingID = {0} and FloorIndex = {1} and AddFloor {2}",
                        nBuildingID, nFloorIndex, strAddFloorCondition);
                    arrResult = m_dbMgr.GetResultData(strSQL, 0);

                    if (arrResult == null)
                        return false;

                    if (arrResult.Count == 0)
                        return false;

                    int nZoneID = m_dbMgr.GetIntField(arrResult[0].ToString(), -1);
                    if (nZoneID < 0)
                        return false;

                    if (dxf.OpenDXF(strFilePath))
                    {
                        if (!InsertDXFToDB(strFilePath, dxf, nZoneID, fUnitFlag, dicZoneIDList, ref nID))
                            return false;
                    }
                    else
                        return false;
                }
            }

            MessageBox.Show("DB 저장 끝");
            return true;
        }

        // Key : EquipZone Name
        // Value : Linked Zone ID List
        //         List의 처음에는 EquipmentZone의 ID를 넣는다.
        private Dictionary<string, ArrayList> LoadEquipZones(ref int nMaxID)
        {
            Dictionary<string, ArrayList> dicZoneIDList = new Dictionary<string, ArrayList>();

            WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strSQL = "select id, ZoneName, Boundary, LinkedZoneIDList, type, BroadcastName from EquipmentZone";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return dicZoneIDList;

            int nResultCount = arrResult.Count;
            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                int nID = dbMgr.GetIntField(arrResult[i].ToString(), -1);
                string strZoneName = dbMgr.GetStringField(arrResult[i + 1], "");
                string strBoundary = dbMgr.GetStringField(arrResult[i + 2], "");
                string strLinkedZones = dbMgr.GetStringField(arrResult[i + 3], "");
                int nType = dbMgr.GetIntField(arrResult[i + 4].ToString(), 0);
                string strBroadcastName = dbMgr.GetStringField(arrResult[i + 5], "");

                if (nID <= 0)
                    continue;

                if (nMaxID < nID)
                    nMaxID = nID;

                strLinkedZones = strLinkedZones.Trim();
                string[] szIds = strLinkedZones.Split(',');

                ArrayList arrZoneIDs = new ArrayList();
                arrZoneIDs.Add(nID);

                for (int j = 0; j < szIds.Length; j++)
                {
                    string szID = szIds[j];
                    int nZoneID = -1;
                    if (int.TryParse(szID, out nZoneID))
                    {
                        arrZoneIDs.Add(nZoneID);
                    }
                    else
                        return null;
                }

                dicZoneIDList[strZoneName] = arrZoneIDs;
            }

            return dicZoneIDList;
        }

        private bool InsertDXFToDB(string strFilePath, DXFViewer.DXFControl dxf, int nZoneID, float fUnitFlag, Dictionary<string, ArrayList> dicZoneIDList, ref int nID)
        {
            UnE.Geometry.Vertex2D vMove = dxf.MovedVertex;
            ArrayList arrBuildings = BuildingBoundaryLoader.ReadBuildings();

            string strLayer1 = "소화설비_zone";
            string strLayer2 = "발신기_zone";

            ArrayList arrEquipZoneName = new ArrayList();
            bool success = true;
            double dUnitFlag = 0.001;

            foreach (DXFViewer.Layer layer in dxf.Layers)
            {
                int nLayerType = -1;

                if (string.Compare(layer.LayerName, strLayer1, true) == 0)
                    nLayerType = 0;
                else if (string.Compare(layer.LayerName, strLayer2, true) == 0)
                    nLayerType = 1;

                if (nLayerType >= 0)
                {
                    // Key : BlockName, Value : DXFViewer.Shape List
                    Dictionary<string, ArrayList> dicShapeBlocks = new Dictionary<string, ArrayList>();

                    foreach (DXFViewer.Shape shape in layer.Shapes)
                    {
                        DXFViewer.Block block = shape.GetBlock();
                        if (block == null)
                            continue;

                        string strEquipZoneName = block.Name;
                        DXFViewer.Shape.ShapeType shapeType = shape.GetShapeType();

                        if (arrEquipZoneName.Contains(strEquipZoneName))
                            continue;

                        DXFViewer.Shape.ShapeType type = shape.GetShapeType();
                        DXFViewer.Shape shape2 = null;

                        if (type == DXFViewer.Shape.ShapeType.POLYLINE)
                        {
                            /*DXFViewer.PolyLine pLine = (DXFViewer.PolyLine)shape;

                            if (!InsertZone(pLine, vMove, nLayerType, nZoneID, strEquipZoneName, dicZoneIDList, ref nID, out success))
                                return false;
                            else
                            {
                                if (success)
                                    arrEquipZoneName.Add(strEquipZoneName);
                            }*/
                            DXFViewer.PolyLine pLine = (DXFViewer.PolyLine)shape;
                            DXFViewer.PolyLine pLine2 = new DXFViewer.PolyLine();

                            int nVertexCount = pLine.GetVertexSize();

                            UnE.Geometry.Vertex2D vBegin = null, vEnd = null;

                            System.Drawing.PointF ptBegin = pLine.GetVertex(0);
                            System.Drawing.PointF ptEnd = pLine.GetVertex(nVertexCount - 1);

                            vBegin = new UnE.Geometry.Vertex2D((ptBegin.X - vMove.x) * dUnitFlag, (ptBegin.Y - vMove.y) * dUnitFlag);
                            vEnd = new UnE.Geometry.Vertex2D((ptEnd.X - vMove.x) * dUnitFlag, (ptEnd.Y - vMove.y) * dUnitFlag);

                            if (vBegin.GetDistance(vEnd) < 0.1)
                                nVertexCount--;

                            pLine2.SetPointSize(nVertexCount);

                            for (int i = 0; i < nVertexCount; i++)
                            {
                                System.Drawing.PointF pt = pLine.GetVertex(i);
                                System.Drawing.PointF pt2 = new System.Drawing.PointF((float)((pt.X - vMove.x) * dUnitFlag), (float)((pt.Y - vMove.y) * dUnitFlag));
                                pLine2.UpdatePoint(i, pt2.X, pt2.Y);
                            }

                            /*if (vBegin.GetDistance(vEnd) < 0.1)
                                pLine2.SetPointSize(nVertexCount - 1);*/

                            shape2 = pLine2;
                        }
                        else if (type == DXFViewer.Shape.ShapeType.LINE)
                        {
                            DXFViewer.Line line = (DXFViewer.Line)shape;

                            UnE.Geometry.Vertex2D vBegin = new UnE.Geometry.Vertex2D((line.Begin.x - vMove.x) * dUnitFlag, (line.Begin.y - vMove.y) * dUnitFlag);
                            UnE.Geometry.Vertex2D vEnd = new UnE.Geometry.Vertex2D((line.End.x - vMove.x) * dUnitFlag, (line.End.y - vMove.y) * dUnitFlag);

                            DXFViewer.Line line2 = new DXFViewer.Line(vBegin, vEnd);
                            shape2 = line2;
                        }
                        else if (type == DXFViewer.Shape.ShapeType.ARC)
                        {
                            DXFViewer.Arc arc = (DXFViewer.Arc)shape;

                            UnE.Geometry.Vertex2D vCenter = new UnE.Geometry.Vertex2D((arc.Center.x - vMove.x) * dUnitFlag, (arc.Center.y - vMove.y) * dUnitFlag);

                            DXFViewer.Arc arc2 = new DXFViewer.Arc();

                            arc2.Center = vCenter;
                            arc2.Radius = arc.Radius * dUnitFlag;
                            arc2.IsCircle = arc.IsCircle;
                            arc2.ArcAngle = arc.ArcAngle;
                            arc2.BeginAngle = arc.BeginAngle;

                            DXFViewer.PolyLine pLine = ArcToPolyline(arc2);
                            shape2 = pLine;
                        }
                        else if (type == DXFViewer.Shape.ShapeType.EARC)
                        {
                            DXFViewer.EArc eArc = (DXFViewer.EArc)shape;

                            UnE.Geometry.Vertex2D vTL = new UnE.Geometry.Vertex2D((eArc.TopLeft.x - vMove.x) * dUnitFlag, (eArc.TopLeft.y - vMove.y) * dUnitFlag);
                            double dWidth = eArc.Width * dUnitFlag;
                            double dHeight = eArc.Height * dUnitFlag;

                            DXFViewer.EArc eArc2 = new DXFViewer.EArc();

                            eArc2.TopLeft = vTL;
                            eArc2.Width = dWidth;
                            eArc2.Height = dHeight;
                            eArc2.IsEllipse = eArc.IsEllipse;
                            eArc2.EArcAngle = eArc.EArcAngle;
                            eArc2.BeginAngle = eArc.BeginAngle;
                            eArc2.XAxisAngle = eArc.XAxisAngle;

                            DXFViewer.PolyLine pLine = EArcToPolyline(eArc2);
                            shape2 = pLine;
                        }
                        else
                            continue;

                        ArrayList arrShapes = null;

                        if (dicShapeBlocks.ContainsKey(strEquipZoneName))
                            arrShapes = dicShapeBlocks[strEquipZoneName];
                        else
                        {
                            arrShapes = new ArrayList();
                            dicShapeBlocks[strEquipZoneName] = arrShapes;
                        }

                        arrShapes.Add(shape2);
                    }

                    // Block이 PolyLine이 아닌 Shape으로 이루어져 있을 경우
                    ProcessOtherShapes(strFilePath, dicShapeBlocks, nLayerType, nZoneID, dicZoneIDList, ref nID);
                }
            }

            return true;
        }

        private bool ProcessOtherShapes(string strFilePath, Dictionary<string, ArrayList> dicShapeBlocks, int nType, int nZoneID, Dictionary<string, ArrayList> dicZoneIDList, ref int nID)
        {
            foreach (KeyValuePair<string, ArrayList> pair in dicShapeBlocks)
            {
                ArrayList arrShapes = pair.Value;

                int nShapeCount = arrShapes.Count - 1;
                DXFViewer.Shape shapePrev = (DXFViewer.Shape)arrShapes[0];

                UnE.Geometry.Vertex2D vPrev = null, vBegin = null, vEnd = null;
                string strBoundary = "";

                if (shapePrev.GetShapeType() == DXFViewer.Shape.ShapeType.LINE)
                {
                    DXFViewer.Line line = (DXFViewer.Line)shapePrev;
                    AddBoundary(ref strBoundary, line.Begin);
                    vPrev = AddShapeBoundary(shapePrev, false, ref strBoundary);
                }
                else
                {
                    DXFViewer.PolyLine pLine = (DXFViewer.PolyLine)shapePrev;
                    System.Drawing.PointF ptBegin = pLine.GetVertex(0);
                    AddBoundary(ref strBoundary, new UnE.Geometry.Vertex2D(ptBegin.X, ptBegin.Y));
                    vPrev = AddShapeBoundary(shapePrev, false, ref strBoundary);
                }

                arrShapes.Remove(shapePrev);

                while (nShapeCount > 0)
                {
                    int nOriginShapeCount = nShapeCount;

                    foreach (DXFViewer.Shape shape in arrShapes)
                    {
                        GetEndVertices(shape, out vBegin, out vEnd);

                        if (vPrev.GetDistance(vBegin) < 0.1)
                        {
                            AddShapeBoundary(shape, false, ref strBoundary);
                            vPrev = vEnd;
                            arrShapes.Remove(shape);
                            nShapeCount--;
                            break;
                        }
                        else if (vPrev.GetDistance(vEnd) < 0.1)
                        {
                            AddShapeBoundary(shape, true, ref strBoundary);
                            vPrev = vBegin;
                            arrShapes.Remove(shape);
                            nShapeCount--;
                            break;
                        }
                    }

                    if (nOriginShapeCount == nShapeCount)
                    {
                        m_writerInvalid.WriteLine(strFilePath + " : " + pair.Key + " is mismatch");
                        m_writerInvalid.Flush();
                        nShapeCount = -1;
                    }
                }

                if (nShapeCount < 0)
                    continue;

                if (!ProcessDB(strBoundary, nType, nZoneID, pair.Key, dicZoneIDList, ref nID))
                {
                    m_writerInvalid.WriteLine(strFilePath + " : " + pair.Key + " is db error");
                    m_writerInvalid.Flush();
                }
                else
                {
                    m_writerValid.WriteLine(pair.Key);
                    m_writerValid.Flush();
                }
            }

            return true;
        }

        private void GetEndVertices(DXFViewer.Shape shape, out UnE.Geometry.Vertex2D vBegin, out UnE.Geometry.Vertex2D vEnd)
        {
            if (shape.GetShapeType() == DXFViewer.Shape.ShapeType.LINE)
            {
                DXFViewer.Line line = (DXFViewer.Line)shape;
                vBegin = line.Begin;
                vEnd = line.End;
            }
            else
            {
                DXFViewer.PolyLine pLine = (DXFViewer.PolyLine)shape;
                int nVertexCount = pLine.GetVertexSize();

                System.Drawing.PointF ptBegin = pLine.GetVertex(0);
                System.Drawing.PointF ptEnd = pLine.GetVertex(nVertexCount - 1);

                vBegin = new UnE.Geometry.Vertex2D(ptBegin.X, ptBegin.Y);
                vEnd = new UnE.Geometry.Vertex2D(ptEnd.X, ptEnd.Y);
            }
        }

        private UnE.Geometry.Vertex2D AddShapeBoundary(DXFViewer.Shape shape, bool isBegin, ref string strBoundary)
        {
            if (shape.GetShapeType() == DXFViewer.Shape.ShapeType.LINE)
            {
                DXFViewer.Line line = (DXFViewer.Line)shape;

                if (isBegin)
                {
                    AddBoundary(ref strBoundary, line.Begin);
                    return line.Begin;
                }
                else
                {
                    AddBoundary(ref strBoundary, line.End);
                    return line.End;
                }
            }

            DXFViewer.PolyLine pLine = (DXFViewer.PolyLine)shape;
            int nVertexCount = pLine.GetVertexSize();

            if (isBegin)
            {
                for (int i = nVertexCount - 2; i >= 0; i--)
                {
                    System.Drawing.PointF pt = pLine.GetVertex(i);
                    AddBoundary(ref strBoundary, new UnE.Geometry.Vertex2D(pt.X, pt.Y));
                }

                System.Drawing.PointF ptBegin = pLine.GetVertex(0);
                return new UnE.Geometry.Vertex2D(ptBegin.X, ptBegin.Y);
            }

            for (int i = 1; i < nVertexCount; i++)
            {
                System.Drawing.PointF pt = pLine.GetVertex(i);
                AddBoundary(ref strBoundary, new UnE.Geometry.Vertex2D(pt.X, pt.Y));
            }

            System.Drawing.PointF ptEnd = pLine.GetVertex(nVertexCount - 1);
            return new UnE.Geometry.Vertex2D(ptEnd.X, ptEnd.Y);
        }

        /*private void ProcessOtherShapes(string strFilePath, Dictionary<string, ArrayList> dicShapeBlocks, int nType, int nZoneID, Dictionary<string, ArrayList> dicZoneIDList, ref int nID)
        {
            foreach (KeyValuePair<string, ArrayList> pair in dicShapeBlocks)
            {
                DXFViewer.Shape.ShapeType type;

                if (IsMultiTypeShape(pair.Value, out type))
                {
                    m_writerInvalid.WriteLine(strFilePath + " : Multi Type Error, " + pair.Key);
                    m_writerInvalid.Flush();
                }
                else if (type == DXFViewer.Shape.ShapeType.LINE)
                {
                    if (!ProcessLineType(pair.Key, pair.Value, nType, nZoneID, dicZoneIDList, ref nID))
                    {
                        m_writerInvalid.WriteLine(strFilePath + " : Line Type Error, " + pair.Key);
                        m_writerInvalid.Flush();
                    }
                    else
                    {
                        m_writerValid.WriteLine(pair.Key);
                        m_writerValid.Flush();
                    }
                }
                else if (type == DXFViewer.Shape.ShapeType.ARC)
                {
                    if (!ProcessArcType(pair.Key, pair.Value, nType, nZoneID, dicZoneIDList, ref nID))
                    {
                        m_writerInvalid.WriteLine(strFilePath + " : Arc Type Error, " + pair.Key);
                        m_writerInvalid.Flush();
                    }
                    else
                    {
                        m_writerValid.WriteLine(pair.Key);
                        m_writerValid.Flush();
                    }
                }
                else
                {
                    m_writerInvalid.WriteLine(strFilePath + " : " + type.ToString() + " Type Error, " + pair.Key);
                    m_writerInvalid.Flush();
                }
            }
        }*/

        private DXFViewer.PolyLine ArcToPolyline(DXFViewer.Arc arc)
        {
            int nVertexCount = (int)(20 * arc.ArcAngle / 360.0);
            if (nVertexCount < 5)
                nVertexCount = 5;

            double dTheta = UnE.Geometry.Math.DegToRad(arc.ArcAngle);
            double dBeginAngle = UnE.Geometry.Math.DegToRad(arc.BeginAngle);
            double delta = dTheta / nVertexCount;

            if (!arc.IsCircle)
                nVertexCount++;

            DXFViewer.PolyLine pLine = new DXFViewer.PolyLine();
            pLine.SetPointSize(nVertexCount);

            for (int i = 0; i < nVertexCount; i++)
            {
                double dAngle = dBeginAngle - delta * i;
                double x = arc.Center.x + arc.Radius * System.Math.Cos(dAngle);
                double y = arc.Center.y + arc.Radius * System.Math.Sin(dAngle);

                pLine.UpdatePoint(i, (float)x, (float)y);
            }

            return pLine;
        }

        private DXFViewer.PolyLine EArcToPolyline(DXFViewer.EArc eArc)
        {
            double a = eArc.Width / 2;
            double b = eArc.Height / 2;

            double dTheta = UnE.Geometry.Math.DegToRad(eArc.XAxisAngle);
            double x = eArc.TopLeft.x + a * System.Math.Cos(dTheta);
            double y = eArc.TopLeft.y + a * System.Math.Sin(dTheta);

            UnE.Geometry.Vertex2D vT = new UnE.Geometry.Vertex2D(x, y);
            UnE.Geometry.Vertex2D vL = UnE.Geometry.Math.GetRightVertex(eArc.TopLeft, vT, -b);
            UnE.Geometry.Vertex2D vCenter = vT - eArc.TopLeft + vL;
            UnE.Geometry.Vertex2D vR = vCenter * 2 - vL;
            UnE.Geometry.Vertex2D vTR = vR - vCenter + vT;

            int nVertexCount = (int)(20 * eArc.EArcAngle / 360.0);
            if (nVertexCount < 5)
                nVertexCount = 5;

            double delta = eArc.EArcAngle / nVertexCount;

            if (!eArc.IsEllipse)
                nVertexCount++;

            DXFViewer.PolyLine pLine = new DXFViewer.PolyLine();
            pLine.SetPointSize(nVertexCount);

            for (int i = 0; i < nVertexCount; i++)
            {
                double dAngle = eArc.BeginAngle + delta * i;
                UnE.Geometry.Vertex2D vertex = GetVertexFromEArc(vCenter, vT, vR, a, b, -dAngle);

                pLine.UpdatePoint(i, (float)vertex.x, (float)vertex.y);
            }

            return pLine;
        }

        private UnE.Geometry.Vertex2D GetVertexFromEArc(UnE.Geometry.Vertex2D vCenter, UnE.Geometry.Vertex2D vT, UnE.Geometry.Vertex2D vR, double a, double b, double dAngle)
        {
            while (dAngle < 0.0)
                dAngle += 360.0;

            while (dAngle > 360.0)
                dAngle -= 360.0;

	        if (dAngle == 0.0 || dAngle == 360.0) return vR;
	        else if (dAngle == 90.0) return  vT;
	        else if (dAngle == 180.0) return UnE.Geometry.Math.GetLinearVertex(vR, vCenter, a*2);
            else if (dAngle == 270.0) return UnE.Geometry.Math.GetLinearVertex(vT, vCenter, b * 2);

	        double dLengthX, dLengthY;

	        if (dAngle > 0.0 && dAngle < 90.0)
	        {
                double dTheta = UnE.Geometry.Math.DegToRad(dAngle);
		        double dTanData = System.Math.Tan(dTheta);

		        dLengthX = System.Math.Sqrt(1.0 / (1.0 / a / a + dTanData * dTanData / b / b));
                dLengthY = System.Math.Sqrt(1.0 / (1.0 / a / a / dTanData / dTanData + 1.0 / b / b));
	        }
	        else if (dAngle > 90.0 && dAngle < 180.0)
	        {
                double dTheta = UnE.Geometry.Math.DegToRad(180.0 - dAngle);
                double dTanData = System.Math.Tan(dTheta);

                dLengthX = -System.Math.Sqrt(1.0 / (1.0 / a / a + dTanData * dTanData / b / b));
                dLengthY = System.Math.Sqrt(1.0 / (1.0 / a / a / dTanData / dTanData + 1.0 / b / b));
	        }
	        else if (dAngle > 180.0 && dAngle < 270.0)
	        {
                double dTheta = UnE.Geometry.Math.DegToRad(dAngle - 180.0);
                double dTanData = System.Math.Tan(dTheta);

                dLengthX = -System.Math.Sqrt(1.0 / (1.0 / a / a + dTanData * dTanData / b / b));
                dLengthY = -System.Math.Sqrt(1.0 / (1.0 / a / a / dTanData / dTanData + 1.0 / b / b));
	        }
	        else
	        {
                double dTheta = UnE.Geometry.Math.DegToRad(360.0 - dAngle);
                double dTanData = System.Math.Tan(dTheta);

                dLengthX = System.Math.Sqrt(1.0 / (1.0 / a / a + dTanData * dTanData / b / b));
                dLengthY = -System.Math.Sqrt(1.0 / (1.0 / a / a / dTanData / dTanData + 1.0 / b / b));
	        }

            UnE.Geometry.Vertex2D vertex = new UnE.Geometry.Vertex2D();

            vertex.x = vCenter.x + dLengthX * (vR.x - vCenter.x) / a;
            vertex.x = vertex.x + dLengthY * (vT.x - vCenter.x) / b;

            vertex.y = vCenter.y + dLengthX * (vR.y - vCenter.y) / a;
            vertex.y = vertex.y + dLengthY * (vT.y - vCenter.y) / b;

            return vertex;
        }

        private bool ProcessArcType(string strEquipZoneName, ArrayList arrArc, int nType, int nZoneID, Dictionary<string, ArrayList> dicZoneIDList, ref int nID)
        {
            if (arrArc.Count != 1)
                return false;

            DXFViewer.Arc arc = (DXFViewer.Arc)arrArc[0];

            int nVertexCount = 20;
            double delta = System.Math.PI * 2 / nVertexCount;

            string strBoundary = "";

            for (int i = 0; i < nVertexCount; i++)
            {
                double dAngle = delta * i;
                double x = arc.Center.x + arc.Radius * System.Math.Cos(dAngle);
                double y = arc.Center.y + arc.Radius * System.Math.Sin(dAngle);

                AddBoundary(ref strBoundary, new UnE.Geometry.Vertex2D(x, y));
            }

            return ProcessDB(strBoundary, nType, nZoneID, strEquipZoneName, dicZoneIDList, ref nID);
        }

        private bool ProcessLineType(string strEquipZoneName, ArrayList arrLines, int nType, int nZoneID, Dictionary<string, ArrayList> dicZoneIDList, ref int nID)
        {
            int nLineCount = arrLines.Count - 1;
            DXFViewer.Line linePrev = (DXFViewer.Line)arrLines[0];

            arrLines.Remove(linePrev);
            UnE.Geometry.Vertex2D vPrev = linePrev.End;

            string strBoundary = "";
            AddBoundary(ref strBoundary, linePrev.Begin);
            AddBoundary(ref strBoundary, linePrev.End);

            while (nLineCount > 0)
            {
                int nOriginLineCount = nLineCount;

                for (int i = 0; i < nLineCount; i++)
                {
                    DXFViewer.Line line = (DXFViewer.Line)arrLines[i];

                    if (vPrev.GetDistance(line.Begin) < 0.001)
                    {
                        AddBoundary(ref strBoundary, line.End);
                        vPrev = line.End;
                        arrLines.RemoveAt(i);
                        nLineCount--;
                        break;
                    }
                    else if (vPrev.GetDistance(line.End) < 0.001)
                    {
                        AddBoundary(ref strBoundary, line.Begin);
                        vPrev = line.Begin;
                        arrLines.RemoveAt(i);
                        nLineCount--;
                        break;
                    }
                }

                if (nLineCount == nOriginLineCount)
                    return false;
            }

            return ProcessDB(strBoundary, nType, nZoneID, strEquipZoneName, dicZoneIDList, ref nID);
        }

        private void AddBoundary(ref string strBoundary, UnE.Geometry.Vertex2D vertex)
        {
            if (strBoundary.Length == 0)
                strBoundary = string.Format("{0:f3}, {1:f3}", vertex.x, vertex.y);
            else
                strBoundary += string.Format(", {0:f3}, {1:f3}", vertex.x, vertex.y);
        }

        private bool IsMultiTypeShape(ArrayList arrShapes, out DXFViewer.Shape.ShapeType shapeType)
        {
            DXFViewer.Shape prev = null;
            shapeType = DXFViewer.Shape.ShapeType.NONE;

            foreach (DXFViewer.Shape shape in arrShapes)
            {
                if (prev == null)
                    prev = shape;
                else
                {
                    if (shape.GetShapeType() != shape.GetShapeType())
                        return true;
                }
            }

            shapeType = prev.GetShapeType();
            return false;
        }

        private bool ProcessDB(string strBoundary, int nType, int nZoneID, string strEquipZoneName, Dictionary<string, ArrayList> dicZoneIDList, ref int nID)
        {
            ArrayList arrZoneIDList = null;
            string strSQL = "";

            ArrayList arrZoneIDList2 = GetZoneIDList(strEquipZoneName, nZoneID);

            if (dicZoneIDList.ContainsKey(strEquipZoneName))
            {
                arrZoneIDList = dicZoneIDList[strEquipZoneName];
                int nEquipZoneID = (int)arrZoneIDList[0];
                string strLinkedZoneIDs = GetZoneIDList(arrZoneIDList, arrZoneIDList2);

                strSQL = string.Format("Update EquipmentZone set LinkedZoneIDList = '{0}', Boundary = '{1}' where ID = {2}",
                    strLinkedZoneIDs, strBoundary, nEquipZoneID);
            }
            else
            {
                string strLinkedZoneIDs = GetZoneIDList(null, arrZoneIDList2);

                // '\''은 jsp 쿼리 처리시 문제가 될수 있으므로 (char)8로 바꿔서 DB에 저장한다.
                string strEquipZoneNameChanged = strEquipZoneName.Replace('\'', (char)8);

                strSQL = string.Format("Insert into EquipmentZone (ID, ZoneName, Boundary, LinkedZoneIDList, Type, BroadcastName, Description) values ({0}, '{1}', '{2}', '{3}', {4}, '{1}', NULL)",
                    ++nID, strEquipZoneNameChanged, strBoundary, strLinkedZoneIDs, nType);

                arrZoneIDList = new ArrayList();
                dicZoneIDList[strEquipZoneName] = arrZoneIDList;

                // EquipmentZone ID를 List의 제일 앞에 저장시킨다.
                arrZoneIDList.Add(nID);

                m_writerValid.Write("Insert ID " + nID.ToString() + ", ");
                m_writerValid.Flush();
            }

            if (!arrZoneIDList.Contains(nZoneID))
                arrZoneIDList.Add(nZoneID);

            WebDBManager dbMgr = FormMain.Instance.DBManager;
            bool result = dbMgr.GetResultData(strSQL, 0) != null;
            return result;
        }

        private bool InsertZone(DXFViewer.PolyLine pLine, UnE.Geometry.Vertex2D vMove, int nType, int nZoneID, string strEquipZoneName, Dictionary<string, ArrayList> dicZoneIDList, ref int nID, out bool success)
        {
            string strBoundary = ZoneBoundaryLoader.MakeBoundaryString(pLine, vMove);

            if (strBoundary.Length == 0)
            {
                success = false;
                return true;
            }
            else
                success = true;

            return ProcessDB(strBoundary, nType, nZoneID, strEquipZoneName, dicZoneIDList, ref nID);
            // '\''은 jsp 쿼리 처리시 문제가 될수 있으므로 (char)8로 바꿔서 DB에 저장한다.
            /*strEquipZoneName = strEquipZoneName.Replace('\'', (char)8);

            ArrayList arrZoneIDList = null;
            string strSQL = "";

            ArrayList arrZoneIDList2 = GetZoneIDList(strEquipZoneName, nZoneID);

            if (dicZoneIDList.ContainsKey(strEquipZoneName))
            {
                arrZoneIDList = dicZoneIDList[strEquipZoneName];
                int nEquipZoneID = (int)arrZoneIDList[0];
                string strLinkedZoneIDs = GetZoneIDList(arrZoneIDList, arrZoneIDList2);

                strSQL = string.Format("Update EquipmentZone set LinkedZoneIDList = '{0}' where ID = {1}",
                    strLinkedZoneIDs, nEquipZoneID);
            }
            else
            {
                string strLinkedZoneIDs = GetZoneIDList(null, arrZoneIDList2);

                strSQL = string.Format("Insert into EquipmentZone (ID, ZoneName, Boundary, LinkedZoneIDList, Type, BroadcastName, Description) values ({0}, '{1}', '{2}', '{3}', {4}, '{1}', NULL)",
                    ++nID, strEquipZoneName, strBoundary, strLinkedZoneIDs, nType);

                arrZoneIDList = new ArrayList();
                dicZoneIDList[strEquipZoneName] = arrZoneIDList;

                // EquipmentZone ID를 List의 제일 앞에 저장시킨다.
                arrZoneIDList.Add(nID);
            }

            arrZoneIDList.Add(nZoneID);

            return true;*/
            //bool result = m_dbMgr.GetResultData(strSQL, 0) != null;
            //return result;
        }

        private ArrayList GetZoneIDList(string strEquipZoneName, int nZoneID)
        {
            ArrayList arrZoneIDs = new ArrayList();
            TrimString(ref strEquipZoneName);

            int nLen = strEquipZoneName.Length;

            if (nLen >= 4)
            {
                char ch = strEquipZoneName.ElementAt(nLen - 1);

                if (ch == 'F')
                {
                    int nIndex1 = strEquipZoneName.LastIndexOf('-');
                    if (nIndex1 >= 0)
                    {
                        int nIndex2 = strEquipZoneName.LastIndexOf(' ');

                        if (nIndex2 >= 0 && nIndex2 < nIndex1)
                        {
                            string str1 = strEquipZoneName.Substring(nIndex2 + 1, nIndex1 - nIndex2 - 1);
                            string str2 = strEquipZoneName.Substring(nIndex1 + 1, nLen - nIndex1 - 2);

                            if (str1.Length > 0 && str2.Length > 0)
                            {
                                int nFloorIndex1, nFloorIndex2;

                                if (int.TryParse(str1, out nFloorIndex1) && int.TryParse(str2, out nFloorIndex2))
                                {
                                    if (nFloorIndex2 > nFloorIndex1)
                                    {
                                        string strSQL = string.Format("select id from Zone where BuildingID = (select BuildingID from Zone where id = {0}) and FloorIndex >= {1} and FloorIndex <= {2} and AddFloor is null",
                                            nZoneID, nFloorIndex1 - 1, nFloorIndex2 - 1);

                                        ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

                                        if (arrResult != null)
                                        {
                                            foreach (object obj in arrResult)
                                            {
                                                int nID = m_dbMgr.GetIntField(obj.ToString(), -1);

                                                if (nID < 0)
                                                    return null;

                                                arrZoneIDs.Add(nID);
                                            }

                                            if (!arrZoneIDs.Contains(nZoneID))
                                                arrZoneIDs.Add(nZoneID);

                                            arrZoneIDs.Sort();

                                            return arrZoneIDs;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            arrZoneIDs.Add(nZoneID);
            return arrZoneIDs;
        }

        private string GetZoneIDList(ArrayList arrZoneIDs, ArrayList arrZoneIDs2)
        {
            string strZoneIDs = "";

            if (arrZoneIDs != null)
            {
                int nIDCount = arrZoneIDs.Count;

                for (int i = 1; i < nIDCount; i++)
                {
                    int nZoneID = (int)arrZoneIDs[i];

                    if (strZoneIDs.Length == 0)
                        strZoneIDs = nZoneID.ToString();
                    else
                        strZoneIDs += ", " + nZoneID.ToString();
                }
            }

            foreach (int nZoneID in arrZoneIDs2)
            {
                if (arrZoneIDs != null && arrZoneIDs.Contains(nZoneID))
                    continue;

                if (strZoneIDs.Length == 0)
                    strZoneIDs = nZoneID.ToString();
                else
                    strZoneIDs += ", " + nZoneID.ToString();
            }

            return strZoneIDs;
        }

        private void TrimString(ref string str)
        {
            str = str.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
            str = str.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });
        }
    }
}
