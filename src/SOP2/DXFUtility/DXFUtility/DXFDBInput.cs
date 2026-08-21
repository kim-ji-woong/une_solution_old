using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace DXFUtility
{
    public class DXFDBInput
    {
        private string m_strFolderPath = "";
        private WebDBManager m_dbMgr = null;

        public DXFDBInput(string strFolderPath, WebDBManager dbMgr)
        {
            m_strFolderPath = strFolderPath;
            m_dbMgr = dbMgr;
        }

        public bool Run()
        {
            int nLen = m_strFolderPath.Length;
            string[] arrFolders = System.IO.Directory.GetDirectories(m_strFolderPath);

            foreach (string strFolderPath in arrFolders)
            {
                string strFolderName = strFolderPath.Substring(nLen + 1);

                int nIndex = strFolderPath.IndexOf('_', nLen + 1);
                string strBuildingID = strFolderPath.Substring(nLen + 1, nIndex - (nLen + 1));

                string strSQL = string.Format("select id from Building where BuildingID = '{0}'", strBuildingID);
                ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

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

                    strSQL = string.Format("Update Zone set DXFFileName = '{0}' where SiteID = 1 and BuildingID = {1} and FloorIndex = {2} and AddFloor {3}",
                        strFolderName + strFileName, nBuildingID, nFloorIndex, strAddFloorCondition);

                    if (m_dbMgr.GetResultData(strSQL, 0) == null)
                        return false;
                }
            }

            MessageBox.Show("DXF 경로 추출후 DB 입력작업 완료");
            return true;
        }
    }
}
