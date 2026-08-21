using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace DXFUtility
{
    public class BuildingBoundaryLoader : BoundaryLoader
    {
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

                    if (ReadBuildingBlock(dxf, "BLOCK", dlg.FileName))
                        MessageBox.Show("DXF 로딩 및 DB 데이터 삽입 완료");
                    else
                        MessageBox.Show("DB 데이터 삽입에 실패하였습니다.");
                }
            }
        }

        public void Run2(string strFolder)
        {
            string[] arrFolders = System.IO.Directory.GetDirectories(strFolder);

            foreach (string strFolderPath in arrFolders)
            {
                string[] arrFiles = System.IO.Directory.GetFiles(strFolderPath);

                foreach (string strFilePath in arrFiles)
                {
                    int nDotIndex = strFilePath.LastIndexOf('.');
                    string strExt = strFilePath.Substring(nDotIndex + 1);

                    if (string.Compare(strExt, "dxf", true) != 0)
                        continue;

                    DXFViewer.DXFControl dxf = new DXFViewer.DXFControl();

                    bool isSuccess = dxf.OpenDXF(strFilePath);

                    if (!isSuccess)
                    {
                        string strError = "DXF 불러오기가 실패하였습니다.";
                        MessageBox.Show(strError);
                    }
                    else
                    {
                        FormMain.Instance.Text = strFilePath;

                        if (ReadBuildingBlock(dxf, "BLOCK", strFilePath))
                            continue;//MessageBox.Show("DXF 로딩 및 DB 데이터 삽입 완료");
                        else
                            MessageBox.Show("DB 데이터 삽입에 실패하였습니다.");
                    }
                }
            }
        }

        public bool Run3(string strFilePath)
        {
            DXFViewer.DXFControl dxf = new DXFViewer.DXFControl();

            bool isSuccess = dxf.OpenDXF(strFilePath);

            if (!isSuccess)
            {
                string strError = "DXF 불러오기가 실패하였습니다.";
                MessageBox.Show(strError);
                return false;
            }
            else
            {
                FormMain.Instance.Text = strFilePath;

                if (!ReadBuildingBlock(dxf, "BLOCK", strFilePath))
                    return false;

                if (!ReadOutdoorZone(dxf, "Zone"))
                    return false;

                MessageBox.Show("DB 데이터 Update 성공");
            }

            return true;
        }

        private bool ReadOutdoorZone(DXFViewer.DXFControl dxf, string strBoundaryLayerName)
        {
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
                            if (!UpdateZone2(strZoneName, pLine, vMove))
                                return false;
                        }
                    }

                    break;
                }
            }

            return true;
        }

        private bool ReadBuildingBlock(DXFViewer.DXFControl dxf, string strBoundaryLayerName, string strFilePath)
        {
            int nSlashIndex = strFilePath.LastIndexOf('\\');
            if (nSlashIndex < 0)
                return false;

            nSlashIndex = strFilePath.LastIndexOf('\\', nSlashIndex - 1);
            if (nSlashIndex < 0)
                return false;

            string strDXFFileName = strFilePath.Substring(nSlashIndex + 1);

            UnE.Geometry.Vertex2D vMove = dxf.MovedVertex;
            ArrayList arrBuildings = ReadBuildings();

            foreach (DXFViewer.Layer layer in dxf.Layers)
            {
                if (layer.LayerName == strBoundaryLayerName)
                {
                    foreach (DXFViewer.Shape shape in layer.Shapes)
                    {
                        DXFViewer.Block block = shape.GetBlock();
                        if (block == null)
                            continue;

                        string strBuildingID = block.Name;
                        DXFViewer.Shape.ShapeType shapeType = shape.GetShapeType();

                        if (shape.GetShapeType() == DXFViewer.Shape.ShapeType.POLYLINE)
                        {
                            DXFViewer.PolyLine pLine = (DXFViewer.PolyLine)shape;
                            if (!UpdateZone(strDXFFileName, pLine, vMove))
                                return false;
                            /*if (!UpdateZone(strBuildingID, pLine, arrBuildings, vMove))
                                return false;*/
                        }
                    }

                    break;
                }
            }

            return true;
        }

        private bool UpdateZone(string strDXFFileName, DXFViewer.PolyLine pLine, UnE.Geometry.Vertex2D vMove)
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;
            string strBoundary = MakeBoundaryString(pLine, vMove);

            string strSQL = string.Format("Update Zone set Boundary = '{0}' where DXFFileName = '{1}'", strBoundary, strDXFFileName);
            if (dbMgr.GetResultData(strSQL, 0) == null)
                return false;

            return true;
        }

        private bool InsertZone(string strZoneName, string strBoundary)
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strSQL = string.Format("Select max(id) from Zone");
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            int nID = dbMgr.GetIntField(arrResult[0].ToString(), 0) + 1;

            strSQL = string.Format("Insert into Zone (ID, ZoneName, SiteID, BuildingID, FloorIndex, AddFloor, Boundary" +
                ", DXFFIleName, DXFAccessedTime, _3DFileName, _3DAccessedTime, TextCenter, BroadcastName) values " +
                "({0}, '{1}', 1, -1, 0, NULL, '{2}', NULL, NULL, NULL, NULL, '', '{1}')",
                nID, strZoneName, strBoundary);

            return dbMgr.GetResultData(strSQL, 0) != null;
        }

        private bool UpdateZone2(string strZoneName, DXFViewer.PolyLine pLine, UnE.Geometry.Vertex2D vMove)
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;
            string strBoundary = MakeBoundaryString(pLine, vMove);

            string strSQL = string.Format("Select id From Zone Where ZoneName = '{0}'", strZoneName);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
            {
                return InsertZone(strZoneName, strBoundary);
                //return false;
            }

            int nZoneID = dbMgr.GetIntField(arrResult[0].ToString(), -1);

            if (nZoneID < 0)
                return false;

            strSQL = string.Format("Update Zone set Boundary = '{0}' where ID = {1}", strBoundary, nZoneID);
            if (dbMgr.GetResultData(strSQL, 0) == null)
                return false;

            return true;
        }

        private bool UpdateZone(string strBuildingID, DXFViewer.PolyLine pLine, ArrayList arrBuildings, UnE.Geometry.Vertex2D vMove)
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;
            string strBoundary = MakeBoundaryString(pLine, vMove);

            foreach (Building building in arrBuildings)
            {
                if (building.BuildingID == strBuildingID)
                {
                    string strSQL = string.Format("Update Zone set Boundary = '{0}' where BuildingID = {1}", strBoundary, building.ID);
                    if (dbMgr.GetResultData(strSQL, 0) == null)
                        return false;

                    break;
                }
            }

            return true;
        }

        public static ArrayList ReadBuildings()
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strSQL = "select id, BuildingID from Building";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            ArrayList arrBuildings = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = dbMgr.GetIntField(arrResult[i].ToString(), -1);
                string strBuildingID = dbMgr.GetStringField(arrResult[i + 1], "");

                Building building = new Building();

                building.ID = nID;
                building.BuildingID = strBuildingID;

                arrBuildings.Add(building);
            }

            return arrBuildings;
        }
    }
}
