using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using DXFViewer;

namespace DXFUtility
{
    public class DXFDBInput2
    {
        private string m_strFolderPath = "";
        private WebDBManager m_dbMgr = null;

        public DXFDBInput2(string strFolderPath, WebDBManager dbMgr)
        {
            m_strFolderPath = strFolderPath;
            m_dbMgr = dbMgr;
        }

        public bool Run()
        {
            string strSQL = "select max(id) from FireEquipment";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nMaxID = arrResult.Count == 0 ? 0 : m_dbMgr.GetIntField(arrResult[0].ToString(), 0);

            int nLen = m_strFolderPath.Length;
            string[] arrFolders = System.IO.Directory.GetDirectories(m_strFolderPath);

            DXFViewer.DXFControl dxf = new DXFViewer.DXFControl();
            float fUnitFlag = GetUnitFlag(UnitOfLength.METER, dxf);

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
                        if (!InsertDXFToDB(dxf, nZoneID, fUnitFlag, ref nMaxID))
                            return false;
                    }
                    else
                        return false;

                    /*string strFormat = "Insert into FireEquipment (ID, RFIDTag, EquipID, RFIDTagID, DxfObjID, EquipType, ZoneID, ";
                    strFormat += "x, y, z, Description) values ({0}, NULL, 
                    strSQL = string.Format(

                    strSQL = string.Format("Update Zone set DXFFileName = '{0}' where SiteID = 1 and BuildingID = {1} and FloorIndex = {2}",
                        strFolderName + strFileName, nBuildingID, nFloorIndex);

                    if (m_dbMgr.GetResultData(strSQL, 0) == null)
                        return false;*/
                }
            }
            
            MessageBox.Show("DB 저장 끝");
            return true;
        }

        private bool InsertDXFToDB(DXFViewer.DXFControl dxf, int nZoneID, float fUnitFlag, ref int nEquipID)
        {
            string strFormat = "Insert into FireEquipment (ID, RFIDTag, EquipID, RFIDTagID, DxfObjID, EquipType, ZoneID, ";
            strFormat += "x, y, z, Description) values ({0}, NULL, '{1}', NULL, '{2}', {3}, {4}, {5}, {6}, 0.0, NULL)";

            UnE.Geometry.Vertex2D vMove = dxf.MovedVertex;

            // 설비 Type : 소화기(1), 소화전(2), 발신기(3)
            int nEquipType = 0;

            foreach (DXFViewer.Layer layer in dxf.Layers)
            {
                if (layer.LayerName == "FE")
                    nEquipType = 1;
                else if (layer.LayerName == "HD")
                    nEquipType = 2;
                else if (layer.LayerName == "FA")
                    nEquipType = 3;
                else
                    continue;

                foreach (DXFViewer.Shape shape in layer.Shapes)
                {
                    Block block = shape.GetBlock();
                    if (block == null)
                        continue;

                    string strObjectID = block.Name;

                    if (shape.GetShapeType() == Shape.ShapeType.HATCH)
                    {
                        Hatch hatch = (Hatch)shape;

                        string strSQL = string.Format(strFormat, ++nEquipID, strObjectID, strObjectID, nEquipType,
                            nZoneID, fUnitFlag * (hatch.Center.X - vMove.x), fUnitFlag * (hatch.Center.Y - vMove.y));

                        if (m_dbMgr.GetResultData(strSQL, 0) == null)
                            return false;
                    }
                }
            }

            return true;
        }

        // DXFViewer의 단위계를 unitTrg으로 변환하기 위한 flag 값을 리턴한다.
        public static float GetUnitFlag(DXFViewer.UnitOfLength unitTrg, DXFViewer.DXFControl dxf)
        {
            if (dxf == null)
                return 1.0f;

            DXFViewer.UnitOfLength unitSrc = dxf.UnitOfLength;

            if (unitSrc == DXFViewer.UnitOfLength.INCH)
            {
                if (unitTrg == DXFViewer.UnitOfLength.INCH)
                    return 1.0f;
                else if (unitTrg == DXFViewer.UnitOfLength.FEET)
                    return 1.0f / 12;
                else if (unitTrg == DXFViewer.UnitOfLength.MILLIMETER)
                    return 25.4f;
                else if (unitTrg == DXFViewer.UnitOfLength.CENTIMETER)
                    return 2.54f;
                else if (unitTrg == DXFViewer.UnitOfLength.METER)
                    return 0.0254f;
            }
            else if (unitSrc == DXFViewer.UnitOfLength.FEET)
            {
                if (unitTrg == DXFViewer.UnitOfLength.INCH)
                    return 12.0f;
                else if (unitTrg == DXFViewer.UnitOfLength.FEET)
                    return 1.0f;
                else if (unitTrg == DXFViewer.UnitOfLength.MILLIMETER)
                    return 304.8f;
                else if (unitTrg == DXFViewer.UnitOfLength.CENTIMETER)
                    return 30.48f;
                else if (unitTrg == DXFViewer.UnitOfLength.METER)
                    return 0.3048f;
            }
            else if (unitSrc == DXFViewer.UnitOfLength.MILLIMETER)
            {
                if (unitTrg == DXFViewer.UnitOfLength.INCH)
                    return 1.0f / 25.4f;
                else if (unitTrg == DXFViewer.UnitOfLength.FEET)
                    return 1.0f / 25.4f / 12f;
                else if (unitTrg == DXFViewer.UnitOfLength.MILLIMETER)
                    return 1.0f;
                else if (unitTrg == DXFViewer.UnitOfLength.CENTIMETER)
                    return 0.1f;
                else if (unitTrg == DXFViewer.UnitOfLength.METER)
                    return 0.001f;
            }
            else if (unitSrc == DXFViewer.UnitOfLength.CENTIMETER)
            {
                if (unitTrg == DXFViewer.UnitOfLength.INCH)
                    return 1.0f / 2.54f;
                else if (unitTrg == DXFViewer.UnitOfLength.FEET)
                    return 1.0f / 2.54f / 12;
                else if (unitTrg == DXFViewer.UnitOfLength.MILLIMETER)
                    return 10;
                else if (unitTrg == DXFViewer.UnitOfLength.CENTIMETER)
                    return 1.0f;
                else if (unitTrg == DXFViewer.UnitOfLength.METER)
                    return 0.01f;
            }
            else if (unitSrc == DXFViewer.UnitOfLength.METER)
            {
                if (unitTrg == DXFViewer.UnitOfLength.INCH)
                    return 1.0f / 0.0254f;
                else if (unitTrg == DXFViewer.UnitOfLength.FEET)
                    return 1.0f / 0.0254f / 12;
                else if (unitTrg == DXFViewer.UnitOfLength.MILLIMETER)
                    return 1000.0f;
                else if (unitTrg == DXFViewer.UnitOfLength.CENTIMETER)
                    return 100.0f;
                else if (unitTrg == DXFViewer.UnitOfLength.METER)
                    return 1.0f;
            }

            return 1.0f;
        }
    }
}
