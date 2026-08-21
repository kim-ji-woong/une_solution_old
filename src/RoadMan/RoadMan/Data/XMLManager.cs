using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Drawing;
using System.IO;

namespace RoadMan
{
    public class XMLManager
    {
        public enum FileOption { NO_PASSWORD = 0, PASSWORD_SAVE_ONLY, PASSWORD_READ_WRITE, TYPE_COUNT };

        private string m_strErrorMessage = "";
        private DXFViewer.DXFControl m_dxfCtrl = null;
        private PanelDXFViewer m_panel = null;

        // 현재 시스템이 사용하는 표준문서 Version
        private string m_strVer = "1.07";

        // LoadProject(...)를 통하여 읽은 문서의 버전
        private string m_strXMLVer = "";
        
        public string ErrorMessage
        {
            get { return m_strErrorMessage; }
        }

        public string StandardVersion
        {
            get { return m_strVer; }
        }

        public string XMLVersion
        {
            get { return m_strXMLVer; }
        }

        public void NewProject()
        {
            m_strErrorMessage = "";
        }

        // strFolderPath가 존재하면 해당 폴더의 파일 및 Sub 폴더를 모두 지운다.
        // strFolderPath가 존재하지 않으면 생성한다.
        private void CreateFolder(string strFolderPath)
        {
            if (Directory.Exists(strFolderPath))
            {
                string[] arrFiles = Directory.GetFiles(strFolderPath);

                foreach (string strFile in arrFiles)
                {
                    File.Delete(strFile);
                }

                string[] arrFolders = Directory.GetDirectories(strFolderPath);

                foreach (string strFolder in arrFolders)
                {
                    DeleteFolder(strFolder);
                }
            }
            else
                Directory.CreateDirectory(strFolderPath);
        }

        private void DeleteFolder(string strFolderPath)
        {
            string[] arrFiles = Directory.GetFiles(strFolderPath);

            foreach (string strFile in arrFiles)
            {
                File.Delete(strFile);
            }

            string[] arrFolders = Directory.GetDirectories(strFolderPath);

            foreach (string strFolder in arrFolders)
            {
                DeleteFolder(strFolder);
            }

            Directory.Delete(strFolderPath);
        }

        private void MakeBackupFile(string strPath)
        {
            if (!File.Exists(strPath))
                return;

            int nDotIndex = strPath.LastIndexOf('.');
            string strPathHeader = strPath.Substring(0, nDotIndex + 1);

            string strSrcName = "", strTrgName = "";
            int nBackupFileCount = Options.Instance.BackupCount;

            for (int i = nBackupFileCount - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    strSrcName = strPath;
                    strTrgName = strPathHeader + "bak";
                }
                else if (i == 1)
                {
                    strSrcName = strPathHeader + "bak";
                    strTrgName = string.Format("{0}bak{1:00}", strPathHeader, i);
                }
                else
                {
                    strSrcName = string.Format("{0}bak{1:00}", strPathHeader, i - 1);
                    strTrgName = string.Format("{0}bak{1:00}", strPathHeader, i);
                }

                if (File.Exists(strSrcName))
                    File.Copy(strSrcName, strTrgName, true);
            }
        }
        //private void BackUpFile(string strPath)
        //{
        //    //현재 파일의 경로
        //    //string strCurrentPath = CurrentPath;

        //    //처음 저장하는 프로젝트는 백업 할 필요 X
        //    //if(strCurrentPath == "")
        //   // {
        //    //    return;
        //   // }

        //    //없는 파일은 백업x(작업도중 삭제됐다거나..)
        //   // if(!File.Exists(strCurrentPath))
        //  //  {
        //  //      return;
        //  //  }

        //    //다른이름으로 저장할 때 파일을 덮어씌울경우,..
        //    if (!File.Exists(strPath))
        //    {
        //        return;
        //    }

        //    //경로(파일명 제외)
        //    string strPathOnly = Path.GetDirectoryName(strPath);

        //    //파일명(확장자 포함)
        //    string strFullFileNmaeOnly = Path.GetFileName(strPath);

        //    //파일명
        //    string strFileNameOnly = strFullFileNmaeOnly.Substring(0, strFullFileNmaeOnly.LastIndexOf('.'));

        //    //확장자
        //    string strExt = strFullFileNmaeOnly.Substring(strFullFileNmaeOnly.LastIndexOf('.'));


        //    bool isAllExist = false;

        //    int nOptionCount = Options.Instance.BackupCount;
        //    string[] Paths = new string[nOptionCount + 1];

        //    Paths[0] = strPath;
        //    Paths[1] = strPathOnly + "\\" + strFileNameOnly + ".bak";

        //    if(Paths[0] == Paths[1])
        //    {
        //        Paths[1] = strPathOnly + "\\" + strFileNameOnly + "_0.bak";
        //    }

        //    for (int i = 1; i < nOptionCount; i++)
        //    {
        //        Paths[i + 1] = strPathOnly + "\\" + strFileNameOnly + "_" + i + ".bak";
        //    }

        //    for (int i = 0; i < nOptionCount + 1; i++)
        //    {
        //        if (i == 0)
        //        {
        //            string file = Paths[1];
        //            //파일명.bak 파일이 이미 있는지 검사
        //            if (!File.Exists(file))
        //            {
        //                //없으면 파일명.bak 이름으로 백업파일을 만든다.
        //                File.Copy(strPath, file, true);

        //                isAllExist = false;
        //                break;
        //            }
        //        }
        //        else
        //        {
        //            string file = Paths[i];
        //            //파일명_i.bak 파일이 있는지 검사
        //            if (!File.Exists(file))
        //            {
        //                //없으면 파일명_i.bak 이름으로 백업파일을 만든다.
        //                //
        //                for (int j = i; j > 0; j--)
        //                {
        //                    File.Copy(Paths[j - 1], Paths[j], true);
        //                }

        //                isAllExist = false;
        //                break;
        //            }
        //        }

        //        isAllExist = true;
        //    }

        //    //백업 파일이 다 찼을경우
        //    if (isAllExist)
        //    {
        //        for (int j = nOptionCount; j > 0; j--)
        //        {
        //            File.Copy(Paths[j - 1], Paths[j], true);
        //        }
        //    }
        //}

        // 임시 폴더에 저장된 파일을 압축 및 암호화하여 사용자가 원래 입력한 경로에 복사해 넣는다.
        private bool PostSave(string strTempPath, string strPath, FileOption option, string strEncryptKey)
        {
			//return true;

            int nIndex = strTempPath.LastIndexOf('\\');

            if (nIndex < 0)
                return false;

            string strTempFolder = strTempPath.Substring(0, nIndex);
            string strTempZipFolder = strTempFolder + "Temp";
            string strTempZipPath = strTempZipFolder + "\\RoadManTemp.zip";

            //CreateFolder(strTempFolder);
            CreateFolder(strTempZipFolder);
            
            System.IO.Compression.ZipFile.CreateFromDirectory(strTempFolder, strTempZipPath);

            FileStream file = new FileStream(strTempZipPath, FileMode.Open);

            if (file.Length == 0)
            {
                file.Close();

                DeleteFolder(strTempZipFolder);
                DeleteFolder(strTempFolder);
                return false;
            }

            byte[] arrBinary = new byte[file.Length];
            file.Read(arrBinary, 0, (int)file.Length);
            file.Close();

            try
            {
                //BackUpFile(strPath);
                MakeBackupFile(strPath);

                file = new FileStream(strPath, FileMode.Create);
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            if (option == FileOption.NO_PASSWORD || strEncryptKey.Length == 0)
            {
                byte[] bytes = new byte[1] { (byte)FileOption.NO_PASSWORD };
                file.Write(bytes, 0, bytes.Length);
            }
            else
            {
                byte[] bytes = new byte[1] { (byte)option };

                file.Write(bytes, 0, bytes.Length);
                byte[] bytesKey = MakeKey(strEncryptKey);
                file.Write(bytesKey, 0, bytesKey.Length);
            }

            file.Write(arrBinary, 0, arrBinary.Length);
            file.Close();

            DeleteFolder(strTempZipFolder);
            DeleteFolder(strTempFolder);
            return true;
        }

        public static byte[] MakeKey(string strKey)
        {
            int hash = strKey.GetHashCode();
            return BitConverter.GetBytes(hash);
        }

        public bool SaveProject(System.Windows.Forms.TabControl.TabPageCollection tabPages, string strPath, FileOption option, string strPassword)
        {
            // XML 파일은 일단 임시경로에 저장한다.
            DateTime dtNow = DateTime.Now;
            string strNow = string.Format("\\{0}{1:00}{2:00}{3:00}{4:00}{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
            string strTempFolder = Path.GetTempPath() + strNow;
            string strTempPath = strTempFolder + "\\temp.xml";

            CreateFolder(strTempFolder);

            m_strErrorMessage = "";

            XmlTextWriter writer = null;

            try
            {
                writer = new XmlTextWriter(strTempPath, Encoding.UTF8);

                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();

                writer.WriteStartElement("RoadMan");

                writer.WriteStartAttribute("ver");
                writer.WriteString(m_strVer);
                writer.WriteEndAttribute();

				writer.WriteStartAttribute("SelectZoom");
                writer.WriteString(Options.Instance.ZoomOnSelectStreet.ToString());
				writer.WriteEndAttribute();
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                DeleteFolder(strTempFolder);
                return false;
            }

            if (!SaveGeneral(writer))
            {
                writer.Close();
                DeleteFolder(strTempFolder);
                return false;
            }

            string strProjectFolder = "";
            int nFolderEndIndex = strPath.LastIndexOf('\\');

            if (nFolderEndIndex >= 0)
            {
                strProjectFolder = strPath.Substring(0, nFolderEndIndex);
            }

            foreach (System.Windows.Forms.TabPage page in tabPages)
            {
                PanelDXFViewer panel = (PanelDXFViewer)page.Tag;

                if (panel == null)
                    continue;

                if (!panel.DXFControl.IsOpened)
                {
                    writer.Close();
                    m_strErrorMessage = "먼저 도면파일을(dxf) 열어야만 합니다.";
                    DeleteFolder(strTempFolder);
                    return false;
                }
                else if (!File.Exists(panel.DXFFilePath))
                {
                    writer.Close();
                    m_strErrorMessage = "도면 파일의 경로가 유효한 값이 아닙니다.\r\n" + panel.DXFFilePath;
                    DeleteFolder(strTempFolder);
                    return false;
                }

                if (strProjectFolder.Length > 0)
                    CheckRelativePath(panel, strProjectFolder);

                List<LayerData> arrLayers = panel.LayerForm.GetLayerList();

                if (arrLayers == null)
                {
                    writer.Close();
                    m_strErrorMessage = "Layer 정보를 읽어오는데 실패하였습니다.\r\n프로젝트 파일을 생성할 수 없습니다.";
                    DeleteFolder(strTempFolder);
                    return false;
                }

                m_dxfCtrl = panel.DXFControl;
                m_panel = panel;

                if (!SaveDXF(writer, arrLayers, panel))
                {
                    writer.Close();
                    DeleteFolder(strTempFolder);
                    return false;
                }

                // <DXF> 안으로 옮겼음
                // [2014/09/25] 김지웅
				//SaveOverlay(writer, panel.OverlayPanel);
				//{
				//	DeleteFolder(strTempFolder);
				//	return false;
				//}
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();

            return PostSave(strTempPath, strPath, option, strPassword);
        }

        private void CheckRelativePath(PanelDXFViewer panel, string strProjectFolder)
        {
            if (!panel.IsRelativePath)
            {
                int nFolderEndIndex = panel.DXFFilePath.LastIndexOf('\\');

                if (nFolderEndIndex >= 0)
                {
                    string strFolderName = panel.DXFFilePath.Substring(0, nFolderEndIndex);

                    if (strFolderName == strProjectFolder)
                    {
                        panel.IsRelativePath = true;
                        panel.RelativePath = "." + panel.DXFFilePath.Substring(nFolderEndIndex);
                    }
                }
            }

            if (!panel.UnderImagePainter.IsRelativePath)
            {
                int nFolderEndIndex = panel.UnderImagePainter.ImagePath.LastIndexOf('\\');

                if (nFolderEndIndex >= 0)
                {
                    string strFolderName = panel.UnderImagePainter.ImagePath.Substring(0, nFolderEndIndex);

                    if (strFolderName == strProjectFolder)
                    {
                        panel.UnderImagePainter.IsRelativePath = true;
                        panel.UnderImagePainter.RelativePath = "." + panel.UnderImagePainter.ImagePath.Substring(nFolderEndIndex);
                    }
                }
            }
        }

        private bool SaveGeneral(XmlTextWriter writer)
        {
            writer.WriteStartElement("General");

            writer.WriteStartElement("BackColor");
            writer.WriteString(Options.Instance.BackColor.ToArgb().ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("BackupFileCount");
            writer.WriteString(Options.Instance.BackupCount.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("ZoomOnSelectStreet");
            writer.WriteString(Options.Instance.ZoomOnSelectStreet.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("VisibleBackgroundImage");
            writer.WriteString(Options.Instance.VisibleBackgroundImage.ToString());
            writer.WriteEndElement();

            int nCompleteRatioOption = Options.Instance.CompleteRatioByArea ? 1 : 0;

            writer.WriteStartElement("CompleteRatioOption");
            writer.WriteString(nCompleteRatioOption.ToString());
            writer.WriteEndElement();

			writer.WriteStartElement("PrintHeader");
            writer.WriteString(Options.Instance.PrintHeader.ToString());
            writer.WriteEndElement();

			writer.WriteStartElement("PrintDate");
            writer.WriteString(Options.Instance.PrintDate.ToString());
            writer.WriteEndElement();

			writer.WriteStartElement("PrintHeaderText");
            writer.WriteString(Options.Instance.PrintHeaderText);
            writer.WriteEndElement();

		

            writer.WriteEndElement();
            return true;
        }

        /*public bool SaveProject(List<LayerData> arrLayers, DXFViewer.DXFControl dxfCtrl, string strPath)
        {
            m_strErrorMessage = "";

            if (m_strDXFFilePath.Length == 0)
            {
                m_strErrorMessage = "먼저 도면파일을(dxf) 열어야만 합니다.";
                return false;
            }
            else if (!File.Exists(m_strDXFFilePath))
            {
                m_strErrorMessage = "도면 파일의 경로가 유효한 값이 아닙니다.\r\n" + m_strDXFFilePath;
                return false;
            }

            if (arrLayers == null)
            {
                m_strErrorMessage = "Layer 정보를 읽어오는데 실패하였습니다.\r\n프로젝트 파일을 생성할 수 없습니다.";
                return false;
            }

            XmlTextWriter writer = null;

            try
            {
                writer = new XmlTextWriter(strPath, Encoding.UTF8);

                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            m_dxfCtrl = dxfCtrl;
            return SaveRoadMan(writer, arrLayers);
        }*/

        private bool SaveDXF(XmlTextWriter writer, List<LayerData> arrDatas, PanelDXFViewer panel)
        {
            writer.WriteStartElement("DXF");

            bool result = false;

            if (!SaveRegionName(writer))
                goto END_DOCUMENT;
			
            if (!SaveFilePath(writer))
                goto END_DOCUMENT;

            if (!SaveViewport(writer))
                goto END_DOCUMENT;

            if (!SaveLayers(writer, arrDatas))
                goto END_DOCUMENT;

            if (!SaveProcessLayers(writer))
                goto END_DOCUMENT;

            if (!SaveStreetShapes(writer))
                goto END_DOCUMENT;

            if (!SaveStreetCenterLines(writer))
                goto END_DOCUMENT;

            if (!SaveProcessSchedules(writer))
                goto END_DOCUMENT;

            if (!SaveProcessResults(writer))
                goto END_DOCUMENT;

            if (!SaveLandAddressList(writer))
                goto END_DOCUMENT;

			SaveUnderlay(writer, panel.UnderImagePainter);
            SaveOverlay(writer, panel.OverlayPanel);

            result = true;

            END_DOCUMENT:
            writer.WriteEndElement();

            return result;
        }

        private bool SaveLandAddressList(XmlTextWriter writer)
        {
            writer.WriteStartElement("LandAddressList");

            foreach (KeyValuePair<string, LandAddressData2> pair in m_panel.DataManager.LandAddressDatas)
            {
                if (pair.Value.Hatchs.Count == 0)
                    continue;

                if (!SaveLandAddress(writer, pair.Value))
                    return false;
            }

            writer.WriteEndElement();
            return true;
        }

        private bool SaveLandAddress(XmlTextWriter writer, LandAddressData2 data)
        {
            writer.WriteStartElement("LandAddress");

            writer.WriteStartElement("Name");
            writer.WriteString(data.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("PolyLines");

            foreach (DXFViewer.Hatch hatch in data.Hatchs)
            {
                UnE.Geometry.Polygon polygon = hatch.Polygon;
                int nVertexCount = polygon.GetVertexCount();

                if (nVertexCount == 0)
                    continue;

                writer.WriteStartElement("PolyLine");

                for (int i = 0; i < nVertexCount; i++)
                {
                    UnE.Geometry.Vertex2D vertex = polygon.GetVertex(i);
                    SaveVertex2D(writer, vertex, "Vertex2D");
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();   // PolyLines
            writer.WriteEndElement();   // LandAddress
            return true;
        }

        private bool SaveProcessResults(XmlTextWriter writer)
        {
            List<ProcessResult> results = m_panel.ProcessResults;

            if (results == null)
                return false;

            writer.WriteStartElement("ProcessResults");

            foreach (ProcessResult result in results)
            {
                writer.WriteStartElement("ProcessResult");

                if (result.ProcessSchedule != null)
                {
                    writer.WriteStartAttribute("name");
                    writer.WriteString(result.ProcessSchedule.ScheduleName);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("length");
                    writer.WriteString(result.ProcessSchedule.Length);
                    writer.WriteEndAttribute();
                }

                writer.WriteStartAttribute("desc");
                writer.WriteString(result.Description);
                writer.WriteEndAttribute();

                writer.WriteStartElement("Items");

                foreach (ResultProperty prop in result.ResultProperties)
                {
                    if (prop.ScheduleProperty == null)
                        continue;

                    prop.Sort();

                    writer.WriteStartElement("Item");

                    writer.WriteStartElement("Address");
                    writer.WriteString(prop.ScheduleProperty.StreetName);
                    writer.WriteEndElement();

                    if (!SaveHistoryList(writer, prop))
                        return false;

                    // Item
                    writer.WriteEndElement();
                }

                // Items
                writer.WriteEndElement();

                // ProcessResult
                writer.WriteEndElement();
            }

            // ProcessResults
            writer.WriteEndElement();

            return true;
        }

        private bool SaveHistoryList(XmlTextWriter writer, ResultProperty prop)
        {
            writer.WriteStartElement("HistoryList");

            foreach (ResultPropertyData data in prop.PropertyDatas)
            {
                writer.WriteStartElement("History");

                writer.WriteStartElement("ProjectName");
                writer.WriteString(data.ProjectName);
                writer.WriteEndElement();

                if (data.ProjectCost != null)
                {
                    writer.WriteStartElement("ProjectCost");
                    writer.WriteString(data.ProjectCost.Data.ToString());
                    writer.WriteEndElement();
                }

                if (data.BeginTime != null)
                {
                    writer.WriteStartElement("BeginDate");
                    writer.WriteString(ScheduleProperty.GetDateTimeString(data.BeginTime.Data));
                    writer.WriteEndElement();
                }

                if (data.EndTime != null)
                {
                    writer.WriteStartElement("EndDate");
                    writer.WriteString(ScheduleProperty.GetDateTimeString(data.EndTime.Data));
                    writer.WriteEndElement();
                }

                if (data.AccumulLength != null)
                {
                    writer.WriteStartElement("AccumulLength");
                    writer.WriteString(data.AccumulLength.Data.ToString());
                    writer.WriteEndElement();
                }

                if (data.UnitLength != null)
                {
                    writer.WriteStartElement("UnitLength");
                    writer.WriteString(data.UnitLength.Data.ToString());
                    writer.WriteEndElement();
                }

                if (data.AccumulArea != null)
                {
                    writer.WriteStartElement("AccumulArea");
                    writer.WriteString(data.AccumulArea.Data.ToString());
                    writer.WriteEndElement();
                }

                if (data.UnitArea != null)
                {
                    writer.WriteStartElement("UnitArea");
                    writer.WriteString(data.UnitArea.Data.ToString());
                    writer.WriteEndElement();
                }

                if (data.DirectionFromBegin != null)
                {
                    writer.WriteStartElement("CompleteFromBegin");
                    writer.WriteString(data.DirectionFromBegin.Data.ToString());
                    writer.WriteEndElement();
                }

                // History
                writer.WriteEndElement();
            }

            // HistoryList
            writer.WriteEndElement();
            return true;
        }

        private bool SaveStreetCenterLines(XmlTextWriter writer)
        {
            writer.WriteStartElement("StreetCenterLines");

            foreach (KeyValuePair<string, StreetCenterLine2> pair in m_panel.DataManager.StreetCenterLines)
            {
                writer.WriteStartElement("StreetCenterLine");

                writer.WriteStartElement("StreetName");
                writer.WriteString(pair.Key);
                writer.WriteEndElement();

                writer.WriteStartElement("PolyLines");

                foreach (KeyValuePair<DXFViewer.Shape, PolyLineEx> pair2 in pair.Value.PolyLines)
                {
                    if (!SavePolyLine(writer, pair2.Key.ID, pair2.Value))
                        return false;
                }

                // PolyLines
                writer.WriteEndElement();
                // StreetCenterLine
                writer.WriteEndElement();
            }

            // StreetCenterLines
            writer.WriteEndElement();
            return true;
        }

        private bool SavePolyLine(XmlTextWriter writer, int nTargetShapeID, DXFViewer.PolyLine polyLine)
        {
            writer.WriteStartElement("PolyLine");

            writer.WriteStartElement("TargetShapeID");
            writer.WriteString(nTargetShapeID.ToString());
            writer.WriteEndElement();

            int nVertexCount = polyLine.GetVertexSize();

            if (nVertexCount == 0)
            {
                writer.WriteEndElement();
                return false;
            }

            writer.WriteStartElement("Vertices");

            for (int i = 0; i < nVertexCount;i++)
            {
                PointF pt = polyLine.GetVertex(i);
                SaveVertex2D(writer, new UnE.Geometry.Vertex2D(pt.X, pt.Y), "Vertex");
            }

            // Vertices
            writer.WriteEndElement();
            // PolyLine
            writer.WriteEndElement();

            return true;
        }

        private bool SaveStreetShapes(XmlTextWriter writer)
        {
            writer.WriteStartElement("StreetShapes");
            int nIndex = 0;

            foreach (KeyValuePair<string, List<DXFViewer.Shape>> pair in m_panel.DataManager.StreetShapes)
            {
                writer.WriteStartElement("StreetShape");

                writer.WriteStartAttribute("index");
                writer.WriteString((nIndex++).ToString());
                writer.WriteEndAttribute();

                writer.WriteStartElement("StreetName");
                writer.WriteString(pair.Key);
                writer.WriteEndElement();

                writer.WriteStartElement("Shapes");

                foreach (DXFViewer.Shape shape in pair.Value)
                {
                    writer.WriteStartElement("ID");
                    writer.WriteString(shape.ID.ToString());
                    writer.WriteEndElement();
                }

                // Shapes
                writer.WriteEndElement();
                // StreetShape
                writer.WriteEndElement();
            }

            // StreetShapes
            writer.WriteEndElement();
            return true;
        }

		private bool SaveUnderlay(XmlTextWriter writer, UnE.Underlay.UnderlayImagePainter panel)
		{			
			string szPath = panel.ImagePath;
			string offsetx = string.Format("{0}", panel.Offset.X);
			string offsety = string.Format("{0}", panel.Offset.Y);
			string width = string.Format("{0}", panel.Size.Width);
			string height = string.Format("{0}", panel.Size.Height);

			if (szPath != "")
			{
				writer.WriteStartElement("Underlay");
				writer.WriteStartElement("OffsetX");
				writer.WriteString(offsetx);
				writer.WriteEndElement();

				writer.WriteStartElement("OffsetY");
				writer.WriteString(offsety);
				writer.WriteEndElement();

				writer.WriteStartElement("Width");
				writer.WriteString(width);
				writer.WriteEndElement();

				writer.WriteStartElement("Height");
				writer.WriteString(height);
				writer.WriteEndElement();

				writer.WriteStartElement("ImagePath");

                if (panel.IsRelativePath)
                    writer.WriteString(panel.RelativePath);
                else
				    writer.WriteString(szPath);

				writer.WriteEndElement();
				writer.WriteEndElement();
			}
			
			return true;
		}

		private bool SaveOverlay(XmlTextWriter writer, UnE.Overlay.OverlayPanel panel)
		{
			writer.WriteStartElement("Overlay");

            writer.WriteStartAttribute("visible");
            writer.WriteString(panel.VisibleOverlay.ToString());
            writer.WriteEndAttribute();

			writer.WriteStartElement("TextColor");			
			writer.WriteString(string.Format("{0}", panel.TextColor.ToArgb()));
			writer.WriteEndElement();

			writer.WriteStartElement("LineColor");			
			writer.WriteString(string.Format("{0}", panel.LineColor.ToArgb()));
			writer.WriteEndElement();

			writer.WriteStartElement("Elements");
			ArrayList arList = panel.EntityList;
			foreach(UnE.Overlay.OverlayElement element in arList)
			{
				writer.WriteStartElement("Element");
				element.SaveXML(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();

			writer.WriteEndElement();
			return true;
		}



        private bool SaveImportance(XmlTextWriter writer, ImportanceData data)
        {
            if (data == null)
                return true;

            writer.WriteStartElement("Importance");

            writer.WriteStartElement("PeopleRequest");
            writer.WriteString(string.Format("{0}", data.PeopleRequest));
            writer.WriteEndElement();

            writer.WriteStartElement("Needs");
            writer.WriteString(string.Format("{0}", data.Needs));
            writer.WriteEndElement();

            writer.WriteStartElement("Right");
            writer.WriteString(string.Format("{0}", data.Right));
            writer.WriteEndElement();

            writer.WriteStartElement("NoDate");
            writer.WriteString(string.Format("{0}", data.NoDate));
            writer.WriteEndElement();

            writer.WriteStartElement("LandStatus");
            writer.WriteString(string.Format("{0}", data.LandStatus));
            writer.WriteEndElement();

            writer.WriteStartElement("Around");
            writer.WriteString(string.Format("{0}", data.Around));
            writer.WriteEndElement();

            writer.WriteStartElement("Level");
            writer.WriteString(string.Format("{0}", data.Level));
            writer.WriteEndElement();

            writer.WriteEndElement();
            return true;
        }

        private bool SaveProcessSchedules(XmlTextWriter writer)
        {
            List<ProcessSchedule> schedules = m_panel.ProcessSchedules;

            if (schedules == null)
                return false;

            writer.WriteStartElement("ProcessSchedules");

            foreach (ProcessSchedule schedule in schedules)
            {
                writer.WriteStartElement("ProcessSchedule");

                writer.WriteStartAttribute("name");
                writer.WriteString(schedule.ScheduleName);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("length");
                writer.WriteString(schedule.Length);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("desc");
                writer.WriteString(schedule.Description);
                writer.WriteEndAttribute();

                writer.WriteStartElement("Items");

                foreach (ScheduleProperty prop in schedule.Properties)
                {
                    writer.WriteStartElement("Item");

                    writer.WriteStartElement("Address");
                    writer.WriteString(prop.StreetName);
                    writer.WriteEndElement();

                    SaveImportance(writer, prop.Importance);
                    /*if (prop.Importance != null)
                    {
                        writer.WriteStartElement("Importance");
                        writer.WriteString(string.Format("{0:F1}", prop.Importance.Data));
                        writer.WriteEndElement();
                    }*/

                    if (prop.Width != null)
                    {
                        writer.WriteStartElement("Width");
                        writer.WriteString(string.Format("{0:F1}", prop.Width.Data));
                        writer.WriteEndElement();
                    }

                    if (prop.Length != null)
                    {
                        writer.WriteStartElement("Length");
                        writer.WriteString(string.Format("{0}", prop.Length.Data));
                        writer.WriteEndElement();
                    }

                    if (prop.Area != null)
                    {
                        writer.WriteStartElement("Area");
                        writer.WriteString(string.Format("{0:F1}", prop.Area.Data));
                        writer.WriteEndElement();
                    }

                    if (!SaveLandAddressList(writer, prop))
                        return false;
                    /*writer.WriteStartElement("LandAddr");
                    writer.WriteString(prop.LandAddress);
                    writer.WriteEndElement();*/

                    if (prop.FinalDate != null)
                    {
                        writer.WriteStartElement("FinalDate");
                        writer.WriteString(ScheduleProperty.GetDateTimeString(prop.FinalDate.Data));
                        writer.WriteEndElement();
                    }

                    if (prop.FirstDate != null)
                    {
                        writer.WriteStartElement("FirstDate");
                        writer.WriteString(ScheduleProperty.GetDateTimeString(prop.FirstDate.Data));
                        writer.WriteEndElement();
                    }

                    writer.WriteStartElement("Category");
                    writer.WriteString(prop.Category);
                    writer.WriteEndElement();

                    writer.WriteStartElement("SubCategory");
                    writer.WriteString(prop.SubCategory);
                    writer.WriteEndElement();

                    if (!SavePurportOfLand(writer, prop))
                        return false;

                    if (!SaveCost(writer, prop))
                        return false;

                    writer.WriteStartElement("Complete");
                    writer.WriteString(prop.IsComplete.ToString());
                    writer.WriteEndElement();

                    if (!SaveSectors(writer, prop))
                        return false;

                    // Item
                    writer.WriteEndElement();
                }

                // Items
                writer.WriteEndElement();

                // ProcessSchedule
                writer.WriteEndElement();
            }

            // ProcessSchedules
            writer.WriteEndElement();

            return true;
        }

        private bool SavePurportOfLand(XmlTextWriter writer, ScheduleProperty prop)
        {
            writer.WriteStartElement("PurposeOfLand");
            
            if (prop.RiceField != null)
            {
                writer.WriteStartElement("RiceField");
                writer.WriteString(string.Format("{0:F1}", prop.RiceField.Data));
                writer.WriteEndElement();
            }

            if (prop.Field != null)
            {
                writer.WriteStartElement("Field");
                writer.WriteString(string.Format("{0:F1}", prop.Field.Data));
                writer.WriteEndElement();
            }

            if (prop.Land != null)
            {
                writer.WriteStartElement("Land");
                writer.WriteString(string.Format("{0:F1}", prop.Land.Data));
                writer.WriteEndElement();
            }

            if (prop.ETC != null)
            {
                writer.WriteStartElement("ETC");
                writer.WriteString(string.Format("{0:F1}", prop.ETC.Data));
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            return true;
        }

        private bool SaveCost(XmlTextWriter writer, ScheduleProperty prop)
        {
            writer.WriteStartElement("Cost");

            if (prop.LandCost != null)
            {
                writer.WriteStartElement("LandCost");
                writer.WriteString(prop.LandCost.Data.ToString());
                writer.WriteEndElement();
            }

            if (prop.ObjectCost != null)
            {
                writer.WriteStartElement("ObjectCost");
                writer.WriteString(prop.ObjectCost.Data.ToString());
                writer.WriteEndElement();
            }

            if (prop.AroundCost != null)
            {
                writer.WriteStartElement("AroundCost");
                writer.WriteString(prop.AroundCost.Data.ToString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            return true;
        }

        private bool SaveLandAddressList(XmlTextWriter writer, ScheduleProperty prop)
        {
            writer.WriteStartElement("LandAddrList");

            foreach (LandAddressData data in prop.LandAddressDatas)
            {
                if (!SaveLandAddress(writer, data))
                    return false;
            }

            writer.WriteEndElement();
            return true;
        }

        private bool SaveLandAddress(XmlTextWriter writer, LandAddressData addr)
        {
            writer.WriteStartElement("LandAddr");

            writer.WriteStartElement("TownName");
            writer.WriteString(addr.TownName);
            writer.WriteEndElement();

            writer.WriteStartElement("MajorAddr");
            writer.WriteString(addr.MajorAddr);
            writer.WriteEndElement();

            writer.WriteStartElement("MinorAddr");
            writer.WriteString(addr.MinorAddr);
            writer.WriteEndElement();

            if (addr.TotalArea != null)
            {
                writer.WriteStartElement("TotalArea");
                writer.WriteString(addr.TotalArea.Data.ToString());
                writer.WriteEndElement();
            }

            if (addr.StreetArea != null)
            {
                writer.WriteStartElement("StreetArea");
                writer.WriteString(addr.StreetArea.Data.ToString());
                writer.WriteEndElement();
            }

            if (addr.OwnerType != null)
            {
                writer.WriteStartElement("OwnerType");
                writer.WriteString(addr.OwnerType);
                writer.WriteEndElement();
            }

            if (addr.PublicEstimation != null)
            {
                writer.WriteStartElement("PublicEstimation");
                writer.WriteString(addr.PublicEstimation.Data.ToString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            return true;
        }

        private bool SaveSectors(XmlTextWriter writer, ScheduleProperty prop)
        {
            writer.WriteStartElement("Sectors");

            foreach (SchedulePropertySector sector in prop.Sectors)
            {
                if (!SaveSector(writer, sector))
                    return false;
            }

            writer.WriteEndElement();
            return true;
        }

        private bool SaveSector(XmlTextWriter writer, SchedulePropertySector sector)
        {
            writer.WriteStartElement("Sector");

            if (!SaveShape(writer, sector.Shape))
                return false;

            if (!SaveEditBoxHatch(writer, sector.Hatch))
                return false;

            writer.WriteEndElement();
            return true;
        }

        private bool SaveEditBoxVertices(XmlTextWriter writer, EditBoxHatch hatch)
        {
            writer.WriteStartElement("EditBoxVertices");

            int nVertexCount = hatch.GetEditBoxVertexCount();

            for (int i = 0; i < nVertexCount;i++)
            {
                UnE.Geometry.Vertex2D vertex = hatch.GetEditBoxVertex(i);

                writer.WriteStartElement("Vertex");

                writer.WriteStartAttribute("x");
                writer.WriteString(vertex.x.ToString());
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("y");
                writer.WriteString(vertex.y.ToString());
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("direct");
                writer.WriteString(hatch.GetDirectLink(i).ToString());
                writer.WriteEndAttribute();

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            return true;
        }

        private bool SavePolygon(XmlTextWriter writer, UnE.Geometry.Polygon polygon)
        {
            if (polygon == null)
                return false;

            writer.WriteStartElement("Polygon");

            int nVertexCount = polygon.GetVertexCount();

            for (int i = 0; i < nVertexCount;i++)
            {
                UnE.Geometry.Vertex2D vertex = polygon.GetVertex(i);

                writer.WriteStartElement("Vertex");

                writer.WriteStartAttribute("x");
                writer.WriteString(vertex.x.ToString());
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("y");
                writer.WriteString(vertex.y.ToString());
                writer.WriteEndAttribute();

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            return true;
        }

        private bool SaveEditBoxHatch(XmlTextWriter writer, EditBoxHatch hatch)
        {
            writer.WriteStartElement("EditBoxHatch");

            if (!SaveEditBoxVertices(writer, hatch))
                return false;

            writer.WriteStartElement("DirPos");
            writer.WriteString(hatch.DirPos.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("BeginIndex");
            writer.WriteString(hatch.BeginIndex.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("EndIndex");
            writer.WriteString(hatch.EndIndex.ToString());
            writer.WriteEndElement();

            if (!SavePolygon(writer, hatch.Polygon))
                return false;

            writer.WriteEndElement();
            return true;
        }

        private int GetLayerIndex(DXFViewer.Layer layer)
        {
            return m_dxfCtrl.Layers.IndexOf(layer);
        }

        private int GetShapeIndex(DXFViewer.Layer layer, DXFViewer.Shape shape)
        {
            return layer.Shapes.IndexOf(shape);
        }

        private bool SaveShape(XmlTextWriter writer, DXFViewer.Shape shape)
        {
            writer.WriteStartElement("Shape");

            int nLayerIndex = GetLayerIndex(shape.GetLayer());

            if (nLayerIndex < 0)
                return false;

            int nShapeIndex = GetShapeIndex(shape.GetLayer(), shape);

            if (nShapeIndex < 0)
                return false;

            writer.WriteStartElement("LayerIndex");
            writer.WriteString(nLayerIndex.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("ShapeIndex");
            writer.WriteString(nShapeIndex.ToString());
            writer.WriteEndElement();

            writer.WriteEndElement();
            return true;
        }

        private bool SaveProcessLayers(XmlTextWriter writer)
        {
            writer.WriteStartElement("ProcessLayers");

            if (!SaveProcessLayers(writer, "CompleteLayers", m_panel.DataManager.CompleteLayers))
            {
                writer.WriteEndElement();
                return false;
            }

            if (!SaveProcessLayers(writer, "IncompleteLayers", m_panel.DataManager.IncompleteLayers))
            {
                writer.WriteEndElement();
                return false;
            }

            if (!SaveProcessLayers(writer, "PartialLayers", m_panel.DataManager.PartialLayers))
            {
                writer.WriteEndElement();
                return false;
            }

            writer.WriteEndElement();
            return true;
        }

        private bool SaveProcessLayers(XmlTextWriter writer, string strElementName, List<LayerData> layers)
        {
            writer.WriteStartElement(strElementName);

            foreach (LayerData data in layers)
            {
                writer.WriteStartElement("Layer");
                writer.WriteString(data.LayerIndex.ToString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            return true;
        }

		private bool SaveSelectZoom(XmlTextWriter writer)
		{
			return false;
		}

        private bool SaveViewport(XmlTextWriter writer)
        {
            DXFViewer.Viewport viewport = m_dxfCtrl.GetViewport();
            if (viewport == null)
                return false;

            writer.WriteStartElement("Viewport");

            writer.WriteStartElement("Matrix");

            writer.WriteStartAttribute("f11");
            writer.WriteString(viewport.F11.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("f12");
            writer.WriteString(viewport.F12.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("f21");
            writer.WriteString(viewport.F21.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("f22");
            writer.WriteString(viewport.F22.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("fdx");
            writer.WriteString(viewport.FDx.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("fdy");
            writer.WriteString(viewport.FDy.ToString());
            writer.WriteEndAttribute();

            writer.WriteEndElement();

            SaveVertex2D(writer, viewport.TopLeft, "TopLeft");
            SaveVertex2D(writer, viewport.BottomLeft, "BottomLeft");
            SaveVertex2D(writer, viewport.BottomRight, "BottomRight");

            writer.WriteStartElement("Weight");
            writer.WriteString(viewport.Weight.ToString());
            writer.WriteEndElement();

            writer.WriteEndElement();
            return true;
        }

        private void SaveVertex2D(XmlTextWriter writer, UnE.Geometry.Vertex2D vertex, string strTagName)
        {
            writer.WriteStartElement(strTagName);

            writer.WriteStartAttribute("x");
            writer.WriteString(vertex.x.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("y");
            writer.WriteString(vertex.y.ToString());
            writer.WriteEndAttribute();

            writer.WriteEndElement();
        }

        private bool SaveLayers(XmlTextWriter writer, List<LayerData> arrDatas)
        {
            writer.WriteStartElement("Layers");

            foreach (LayerData layer in arrDatas)
            {
                if (!SaveLayer(writer, layer))
                    return false;
            }

            writer.WriteEndElement();
            return true;
        }

        private bool SaveLayer(XmlTextWriter writer, LayerData layer)
        {
            writer.WriteStartElement("Layer");

            writer.WriteStartAttribute("index");
            writer.WriteString(layer.LayerIndex.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartElement("Name");
            writer.WriteString(layer.LayerName);
            writer.WriteEndElement();

            writer.WriteStartElement("Visible");
            writer.WriteString(layer.Visible.ToString());
            writer.WriteEndElement();

			writer.WriteStartElement("Enabled");
			writer.WriteString(layer.Enabled.ToString());
			writer.WriteEndElement();

            writer.WriteStartElement("Color");

            writer.WriteStartAttribute("a");
            writer.WriteString(layer.Alpha.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("r");
            writer.WriteString(layer.Color.R.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("g");
            writer.WriteString(layer.Color.G.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("b");
            writer.WriteString(layer.Color.B.ToString());
            writer.WriteEndAttribute();

            writer.WriteEndElement();

            writer.WriteEndElement();
            return true;
        }

        private bool SaveRegionName(XmlTextWriter writer)
        {
            if (m_panel.RegionName.Length > 0)
            {
                writer.WriteStartElement("RegionName");
                writer.WriteString(m_panel.RegionName);
                writer.WriteEndElement();
            }

            return true;
        }

        private bool SaveFilePath(XmlTextWriter writer)
        {
            writer.WriteStartElement("FilePath");

            if (m_panel.IsRelativePath)
                writer.WriteString(m_panel.RelativePath);
            else
            {
                writer.WriteString(m_panel.DXFFilePath);
            }

            writer.WriteEndElement();

            return true;
        }

        public static bool ReadProjectOption(string strPath, out FileOption option, out byte[] arrKey)
        {
            arrKey = null;
            option = FileOption.NO_PASSWORD;
            FileStream file = new FileStream(strPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            if (file.Length < 2)
            {
                file.Close();
                return false;
            }

            int nOption = file.ReadByte();

            if (nOption < (int)FileOption.NO_PASSWORD || nOption >= (int)FileOption.TYPE_COUNT)
            {
                file.Close();
                return false;
            }

            option = (FileOption)nOption;

            if (nOption != (int)FileOption.NO_PASSWORD)
            {
                if (file.Length < 6)
                {
                    file.Close();
                    return false;
                }

                arrKey = new byte[4];
                file.Read(arrKey, 0, 4);
            }

            file.Close();
            return true;
        }

        private bool DecompressFile(string strPath, FileOption option, string strEncryptKey, out string strTempPath)
        {
            strTempPath = "";
            FileStream file = new FileStream(strPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            long nFileLength = file.Length;

            if (nFileLength < 2)
            {
                file.Close();
                return false;
            }

            int nOption = file.ReadByte();

            if (nOption != (int)option)
            {
                file.Close();
                return false;
            }

            if (option != FileOption.NO_PASSWORD && nFileLength < 6)
            {
                file.Close();
                return false;
            }

            byte[] arrBinary = null;

            if (option == FileOption.PASSWORD_SAVE_ONLY)
            {
                file.Seek(4, SeekOrigin.Current);

                arrBinary = new byte[nFileLength - 5];
                file.Read(arrBinary, 0, arrBinary.Length);
            }
            else if (option == FileOption.PASSWORD_READ_WRITE)
            {
                byte[] arrSrcKey = new byte[4];
                file.Read(arrSrcKey, 0, arrSrcKey.Length);

                byte[] arrTrgKey = MakeKey(strEncryptKey);

                for (int i=0;i<4;i++)
                {
                    if (arrSrcKey[i] != arrTrgKey[i])
                    {
                        file.Close();
                        return false;
                    }
                }

                arrBinary = new byte[nFileLength - 5];
                file.Read(arrBinary, 0, arrBinary.Length);
            }
            else// if (option == FileOption.NO_PASSWORD)
            {
                arrBinary = new byte[nFileLength - 1];
                file.Read(arrBinary, 0, arrBinary.Length);
            }

            file.Close();

            DateTime dtNow = DateTime.Now;
            string strNow = string.Format("\\{0}{1:00}{2:00}{3:00}{4:00}{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
            string strTempFolder = Path.GetTempPath() + strNow;
            string strTempZipPath = strTempFolder + "\\temp.zip";

            CreateFolder(strTempFolder);
            
            file = new FileStream(strTempZipPath, FileMode.Create);
            file.Write(arrBinary, 0, arrBinary.Length);
            file.Close();

            System.IO.Compression.ZipFile.ExtractToDirectory(strTempZipPath, strTempFolder);

            string[] strFiles = Directory.GetFiles(strTempFolder);

            foreach (string strFile in strFiles)
            {
                if (strFile != strTempZipPath)
                {
                    strTempPath = strFile;
                    return true;
                }
            }

            DeleteFolder(strTempFolder);
            return false;
        }

        public Dictionary<System.Windows.Forms.TabPage, DXFDatas> LoadProject(string strPath, FileOption option, string strPassword)
        {
            m_strErrorMessage = "";

            string strTempPath;

            if (!DecompressFile(strPath, option, strPassword, out strTempPath))
            {
                return null;
            }

            int nIndex = strTempPath.LastIndexOf('\\');

            if (nIndex < 0)
                return null;

            string strTempFolder = strTempPath.Substring(0, nIndex);

            nIndex = strPath.LastIndexOf('\\');

            if (nIndex < 0)
                return null;

            string strProjectFolder = strPath.Substring(0, nIndex);
            
            XmlTextReader reader = null;
            Dictionary<System.Windows.Forms.TabPage, DXFDatas> dicDXFDatas = null;
            bool stop = false;

            try
            {
                reader = new XmlTextReader(strTempPath);

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "RoadMan", true) != 0)
                                m_strErrorMessage = "다른 형식의 파일입니다.";
                            else
                            {
                                dicDXFDatas = ReadRoadMan(reader, strProjectFolder);

                                if (dicDXFDatas == null)
                                    stop = true;
                            }

                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                reader.Close();
                DeleteFolder(strTempFolder);
                return null;
            }

            reader.Close();

			int nCount = FormMain.Instance.GetTabPageCount();
			for (int i = 0; i < nCount; i++)
			{
				System.Windows.Forms.TabPage page = FormMain.Instance.GetTabPage(i);
				PanelDXFViewer pane = (PanelDXFViewer)page.Tag;
				if (pane != null)
				{
					string szPath = pane.DXFFilePath;
					if(! File.Exists(szPath))
					{
						FormFilePath path = new FormFilePath();
						DialogFormFrame framePath = new DialogFormFrame(path);
						path.DXFType = true;
						path.OriginalFilePath = szPath;
						if (framePath.ShowDialog() == System.Windows.Forms.DialogResult.OK)
						{
							if (File.Exists(path.NewFilePath))
							{
								pane.DXFFilePath = path.NewFilePath;
							}
							else
							{
                                UnE.Utility.UMessageBox.Show(m_panel, "경로가 존재하지 않는 파일이 있습니다.\n프로젝트 로딩을 취소합니다.", "프로젝트 오류");
								return null;
							}
						}
						else
						{
                            UnE.Utility.UMessageBox.Show(m_panel, "경로가 존재하지 않는 파일이 있습니다.\n프로젝트 로딩을 취소합니다.", "프로젝트 오류");
							return null;
						}
					}

					string szPath2 = pane.UnderImagePainter.ImagePath;
					if (szPath2 != "" && !File.Exists(szPath2))
					{
						FormFilePath path = new FormFilePath();
						DialogFormFrame framePath = new DialogFormFrame(path);
						path.DXFType = false;
						path.OriginalFilePath = szPath2;
						if (framePath.ShowDialog(FormMain.Instance) == System.Windows.Forms.DialogResult.OK)
						{
							if (File.Exists(path.NewFilePath))
							{
								pane.UnderImagePainter.SetImage(path.NewFilePath);
							}
							else
							{
                                UnE.Utility.UMessageBox.Show(m_panel, "경로가 존재하지 않는 파일이 있습니다.\n프로젝트 로딩을 취소합니다.", "프로젝트 오류");
								return null;
							}
						}
						else
						{
                            UnE.Utility.UMessageBox.Show(m_panel, "경로가 존재하지 않는 파일이 있습니다.\n프로젝트 로딩을 취소합니다.", "프로젝트 오류");
							return null;
						}
					}
				}
			}	

		


            DeleteFolder(strTempFolder);
            return dicDXFDatas;
        }

        private Dictionary<System.Windows.Forms.TabPage, DXFDatas> ReadRoadMan(XmlTextReader reader, string strProjectFolder)
        {
            if (reader == null)
                return null;

            bool stop = false;
            Dictionary<System.Windows.Forms.TabPage, DXFDatas> dicDXFDatas = new Dictionary<System.Windows.Forms.TabPage, DXFDatas>();

            try
            {
				System.Windows.Forms.TabPage page = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "ver", true) == 0)
                    {
                        m_strXMLVer = reader.Value.ToString();
                    }
                }

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:		
                            if (string.Compare(reader.Name, "DXF", true) == 0)
                            {
                                DXFDatas datas = ReadDXF(reader, strProjectFolder, out page);
                                if (datas == null)
                                    return null;
                                else
                                    dicDXFDatas[page] = datas;
                            }
                            else if (string.Compare(reader.Name, "General", true) == 0)
                            {
                                if (!ReadGeneral(reader))
                                    return null;
                            }
                            break;
							

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return dicDXFDatas;
        }

        private bool ReadGeneral(XmlTextReader reader)
        {
            if (reader.IsEmptyElement)
                return true;

            bool bData;
            int nData;
            string strData = "";
            bool stop = false;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "BackColor", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return false;

                                if (strData.Length > 0)
                                {
                                    if (!int.TryParse(strData, out nData))
                                        return false;

                                    Options.Instance.BackColor = Color.FromArgb(nData);
                                }
                            }
                            else if (string.Compare(reader.Name, "BackupFileCount", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return false;

                                if (strData.Length > 0)
                                {
                                    if (!int.TryParse(strData, out nData))
                                        return false;

                                    Options.Instance.BackupCount = nData;
                                }
                                else
                                    Options.Instance.BackupCount = 0;
                            }
                            else if (string.Compare(reader.Name, "ZoomOnSelectStreet", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return false;

                                if (strData.Length > 0)
                                {
                                    if (!bool.TryParse(strData, out bData))
                                        return false;

                                    Options.Instance.ZoomOnSelectStreet = bData;
                                }
                            }
                            else if (string.Compare(reader.Name, "VisibleBackgroundImage", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return false;

                                if (strData.Length > 0)
                                {
                                    if (!bool.TryParse(strData, out bData))
                                        return false;

                                    Options.Instance.VisibleBackgroundImage = bData;
                                }
                            }
							else if (string.Compare(reader.Name, "PrintHeader", true) == 0)
							{
								if (!ReadText(reader, ref strData, true))
									return false;

								if (strData.Length > 0)
								{
									if (!bool.TryParse(strData, out bData))
										return false;

									Options.Instance.PrintHeader = bData;
								}
							}
							else if (string.Compare(reader.Name, "PrintDate", true) == 0)
							{
								if (!ReadText(reader, ref strData, true))
									return false;

								if (strData.Length > 0)
								{
									if (!bool.TryParse(strData, out bData))
										return false;

									Options.Instance.PrintDate = bData;
								}
							}
							else if (string.Compare(reader.Name, "PrintHeaderText", true) == 0)
							{
								if (!ReadText(reader, ref strData, true))
									return false;

								if (strData.Length > 0)
								{
									Options.Instance.PrintHeaderText = strData;
								}
							}	
                            else if (string.Compare(reader.Name, "CompleteRatioOption", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return false;

                                if (strData.Length > 0)
                                {
                                    if (!int.TryParse(strData, out nData))
                                        return false;

                                    if (nData == 0)
                                        Options.Instance.CompleteRatioByArea = false;
                                    else
                                        Options.Instance.CompleteRatioByArea = true;
                                }
                            }
                            else
                                PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return true;
        }

		private ArrayList ReadOverlayElements(XmlTextReader reader)
		{
            if (reader.IsEmptyElement)
                return null;

			ArrayList arResult = new ArrayList();
			bool stop = false;
			try
			{
				while (reader.Read())
				{
					switch (reader.NodeType)
					{
						case XmlNodeType.Element:
							if (string.Compare(reader.Name, "Element", true) == 0)
							{
								string szText = reader.ReadInnerXml();							
								if (szText == "")
									continue;

								object obj = UnE.Overlay.OverlayFactory.Deserialize(szText);
								if(obj != null)
								{
                                    if (obj is UnE.Overlay.OverlayElement)
                                    {
                                        UnE.Overlay.OverlayElement element = (UnE.Overlay.OverlayElement)obj;
                                        element.OnPostXMLRead();
                                    }

									arResult.Add(obj);
								}
							}							
							else
								PassElement(reader);
							break;

						case XmlNodeType.EndElement:
							stop = true;
							break;
					}

					if (stop)
						break;
				}
			}
			catch (Exception e)
			{
				m_strErrorMessage = e.Message;				
			}

			return arResult;
		}

		private bool ReadUnderlay(XmlTextReader reader, string strProjectFolder, System.Windows.Forms.TabPage page)
		{
			if (page == null)
				return false;
			bool stop = false;

			PanelDXFViewer panel = (PanelDXFViewer)page.Tag;
			UnE.Underlay.UnderlayImagePainter ovPanel = panel.UnderImagePainter;
			try
			{
				while (reader.Read())
				{
					switch (reader.NodeType)
					{
						case XmlNodeType.Element:
							if (string.Compare(reader.Name, "ImagePath", true) == 0)
							{
								string szPath = "";
								if (!ReadText(reader, ref szPath))
									return false;
								if( szPath != "")
								{
                                    if (szPath.StartsWith("."))
                                    {
                                        ovPanel.IsRelativePath = true;
                                        ovPanel.RelativePath = szPath;
                                        ovPanel.SetImage(strProjectFolder + "\\" + szPath);
                                    }
                                    else
                                    {
                                        ovPanel.IsRelativePath = false;
                                        ovPanel.SetImage(szPath);
                                    }
									//ovPanel.UseUnderImage = true;
								}
							}
							else if (string.Compare(reader.Name, "OffsetX", true) == 0)
							{
								double szValue = 0.0;
								string szMessage1 = "Underlay";
								string szMessage2 = "offsetx";
								if (!ReadDouble(reader, ref szValue, szMessage1, szMessage2))
									return false;

								ovPanel.SetOffset((float)szValue, ovPanel.Offset.X);
							}
							else if (string.Compare(reader.Name, "OffsetY", true) == 0)
							{
								double szValue = 0.0;
								string szMessage1 = "Underlay";
								string szMessage2 = "offsety";
								if (!ReadDouble(reader, ref szValue, szMessage1, szMessage2))
									return false;

								ovPanel.SetOffset(ovPanel.Offset.X, (float)szValue);
							}
							else if (string.Compare(reader.Name, "Width", true) == 0)
							{
								double szValue = 0.0;
								string szMessage1 = "Underlay";
								string szMessage2 = "width";
								if (!ReadDouble(reader, ref szValue, szMessage1, szMessage2))
									return false;

								ovPanel.SetSize((float)szValue, ovPanel.Size.Height);
							}
							else if (string.Compare(reader.Name, "Height", true) == 0)
							{
								double szValue = 0.0;
								string szMessage1 = "Underlay";
								string szMessage2 = "height";
								if (!ReadDouble(reader, ref szValue, szMessage1, szMessage2))
									return false;

								ovPanel.SetSize(ovPanel.Size.Width, (float)szValue);
							}
							else
								PassElement(reader);
							break;

						case XmlNodeType.EndElement:
							stop = true;
							break;
					}

					if (stop)
						break;
				}
			}
			catch (Exception e)
			{
				m_strErrorMessage = e.Message;
				return false;
			}

			
			return true;
		}

		private bool ReadOverlay(XmlTextReader reader, System.Windows.Forms.TabPage page)
		{
			if (page == null)
				return false;
			bool stop = false;

			PanelDXFViewer panel = (PanelDXFViewer)page.Tag;
			UnE.Overlay.OverlayPanel ovPanel = panel.OverlayPanel;
			try
			{
                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "visible", true) == 0)
                    {
                        bool visible;

                        if (!bool.TryParse(reader.Value.ToString(), out visible))
                            return false;
                        else
                            ovPanel.VisibleOverlay = visible;
                    }
                }

				while(reader.Read())
				{
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "LineColor", true) == 0)
                            {
                                string szMsg1 = "LineColor";
								string szMsg2 = "LineColor";
								int nColor = 0;
								if (!ReadInt(reader, ref nColor, szMsg1, szMsg2))
                                    return false;

                                ovPanel.LineColor = Color.FromArgb(nColor);
                            }
                            else if (string.Compare(reader.Name, "TextColor", true) == 0)
                            {
								string szMsg1 = "TextColor";
								string szMsg2 = "TextColor";
								int nColor = 0;
								if (!ReadInt(reader, ref nColor, szMsg1, szMsg2))
									return false;

								ovPanel.TextColor = Color.FromArgb(nColor);
                            }
                            else if (string.Compare(reader.Name, "Elements", true) == 0)
                            {
                                ArrayList arElements = ReadOverlayElements(reader);

                                if (arElements != null)
                                {
                                    ovPanel.EntityList.AddRange(arElements);
                                    ovPanel.Invalidate();
                                }
                            }                            
                            else
                                PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
				return false;
            }
            return true;			
		}

        private DXFDatas ReadDXF(XmlTextReader reader, string strProjectFolder, out System.Windows.Forms.TabPage page)
        {
            page = null;

            if (reader == null)
                return null;

            bool stop = false;
            List<LayerData> arrLayers = null;
            Dictionary<string, List<int>> dicStreetShapes = null;
            Dictionary<string, StreetCenterLine> dicStreetCenterLines = null;

            page = FormMain.Instance.AddTabPage();
            m_panel = (PanelDXFViewer)page.Tag;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "RegionName", true) == 0)
                            {
                                string strRegionName = "";
                                if (!ReadText(reader, ref strRegionName, true))
                                    return null;

                                m_panel.RegionName = strRegionName;
                            }
                            else if (string.Compare(reader.Name, "FilePath", true) == 0)
                            {
                                string strDXFFilePath = "";
                                if (!ReadText(reader, ref strDXFFilePath))
                                    return null;

                                if (strDXFFilePath.StartsWith("."))
                                {
                                    m_panel.IsRelativePath = true;
                                    m_panel.RelativePath = strDXFFilePath;
                                    m_panel.DXFFilePath = strProjectFolder + "\\" + strDXFFilePath;
                                }
                                else
                                {
                                    m_panel.IsRelativePath = false;
                                    m_panel.DXFFilePath = strDXFFilePath;
                                }
                            }
                            else if (string.Compare(reader.Name, "Viewport", true) == 0)
                            {
                                if (!ReadViewport(reader))
                                    return null;
                            }
                            else if (string.Compare(reader.Name, "Layers", true) == 0)
                            {
                                arrLayers = ReadLayers(reader);

                                if (arrLayers == null)
                                    stop = true;
                            }
                            else if (string.Compare(reader.Name, "ProcessLayers", true) == 0)
                            {
                                if (!ReadProcessLayers(reader, arrLayers))
                                    stop = true;
                            }
                            else if (string.Compare(reader.Name, "StreetShapes", true) == 0)
                            {
                                dicStreetShapes = ReadStreetShapes(reader);

                                if (dicStreetShapes == null)
                                    stop = true;
                            }
                            else if (string.Compare(reader.Name, "StreetCenterLines", true) == 0)
                            {
                                dicStreetCenterLines = ReadStreetCenterLines(reader);

                                if (dicStreetCenterLines == null)
                                    stop = true;
                            }
                            else if (string.Compare(reader.Name, "ProcessSchedules", true) == 0)
                            {
                                if (!ReadProcessSchedules(reader))
                                    stop = true;
                            }
                            else if (string.Compare(reader.Name, "ProcessResults", true) == 0)
                            {
                                if (!ReadProcessResults(reader))
                                    stop = true;
                            }
                            else if (string.Compare(reader.Name, "LandAddressList", true) == 0)
                            {
                                if (!ReadLandAddressList(reader))
                                    stop = true;
                            }
                            else if (string.Compare(reader.Name, "Overlay", true) == 0)
                            {
                                if (!ReadOverlay(reader, page))
                                    return null;
                            }
							else if (string.Compare(reader.Name, "Underlay", true) == 0)
							{
								if (!ReadUnderlay(reader, strProjectFolder, page))
									return null;
							}
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            if (arrLayers == null)
                return null;

            DXFDatas datas = new DXFDatas(arrLayers, dicStreetShapes, dicStreetCenterLines);
            return datas;
        }

        private bool ReadLandAddressList(XmlTextReader reader)
        {
            bool stop = false;
            m_panel.DataManager.LandAddressDatas.Clear();

            if (reader.IsEmptyElement)
                return true;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "LandAddress", true) == 0)
                            {
                                if (!ReadLandAddress2(reader))
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return true;
        }

        private bool ReadLandAddress2(XmlTextReader reader)
        {
            bool stop = false;

            LandAddressData2 addr = null;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Name", true) == 0)
                            {
                                string strAddress = "";

                                if (!ReadText(reader, ref strAddress, true))
                                    return false;

                                LandAddressData2 addr2 = new LandAddressData2(strAddress);
                                string strLandAddr = addr2.ToString();
                                
                                if (!m_panel.DataManager.LandAddressDatas.TryGetValue(strLandAddr, out addr))
                                {
                                    addr = addr2;
                                    m_panel.DataManager.LandAddressDatas[strLandAddr] = addr;
                                }
                            }
                            else if (string.Compare(reader.Name, "PolyLines", true) == 0)
                            {
                                if (!ReadPolyLines(reader, addr))
                                    return false;
                            }
                            // XML Version 1.06을 위하여 남겨둠
                            else if (string.Compare(reader.Name, "PolyLine", true) == 0)
                            {
                                PolyLineEx polyLine = ReadPolyLine(reader, "Vertex2D");

                                if (addr != null)
                                    addr.MakeHatch(polyLine);
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return true;
        }

        private bool ReadPolyLines(XmlTextReader reader, LandAddressData2 addr)
        {
            bool stop = false;

            if (reader.IsEmptyElement)
                return true;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "PolyLine", true) == 0)
                            {
                                PolyLineEx polyLine = ReadPolyLine(reader, "Vertex2D");

                                if (addr != null)
                                    addr.MakeHatch(polyLine);
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return true;
        }

        private bool ReadProcessResults(XmlTextReader reader)
        {
            bool stop = false;
            //m_panel.ClearProcessResult();

            if (reader.IsEmptyElement)
                return true;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "ProcessResult", true) == 0)
                            {
                                if (!ReadProcessResult(reader))
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return true;
        }

        private bool ReadProcessResult(XmlTextReader reader)
        {
            bool stop = false;

            ProcessResult result = null;

            string strScheduleName = "";
            string strDescription = "";

            try
            {
                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "name", true) == 0)
                    {
                        strScheduleName = reader.Value.ToString();
                    }
                    else if (string.Compare(reader.Name, "length", true) == 0)
                    {
                        // ProcessSchedule의 것을 사용하므로 읽을 필요 없음
                    }
                    else if (string.Compare(reader.Name, "desc", true) == 0)
                    {
                        strDescription = reader.Value.ToString();
                    }
                }

                ProcessSchedule schedule = m_panel.ProcessScheduleForm.FindProcessSchedule(strScheduleName);

                if (schedule == null)
                    return false;

                result = m_panel.ProcessResultForm.FindProcessResult(schedule);

                if (result == null)
                    return false;

                result.ProcessSchedule = schedule;
                result.Description = strDescription;

                if (reader.IsEmptyElement)
                    return true;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Items", true) == 0)
                            {
                                if (!ReadProcessResultItems(reader, result))
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            m_panel.ProcessResultForm.UpdateProcessResult(result);
            return true;
        }

        private bool ReadProcessResultItems(XmlTextReader reader, ProcessResult result)
        {
            bool stop = false;

            if (reader.IsEmptyElement)
                return true;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Item", true) == 0)
                            {
                                if (!ReadProcessResultItem(reader, result))
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return true;
        }

        private ScheduleProperty FindScheduleProperty(string strStreetName, ProcessSchedule schedule)
        {
            if (schedule == null)
                return null;

            foreach (ScheduleProperty prop in schedule.Properties)
            {
                if (prop.StreetName == strStreetName)
                    return prop;
            }

            return null;
        }

        private bool ReadProcessResultItem(XmlTextReader reader, ProcessResult result)
        {
            bool stop = false;

            ResultProperty prop = new ResultProperty();

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Address", true) == 0)
                            {
                                string strAddress = "";

                                if (!ReadText(reader, ref strAddress, true))
                                    return false;

                                ScheduleProperty scheduleProperty = FindScheduleProperty(strAddress, result.ProcessSchedule);

                                if (scheduleProperty == null)
                                    return false;
                                else
                                    prop.ScheduleProperty = scheduleProperty;
                            }
                            else if (string.Compare(reader.Name, "HistoryList", true) == 0)
                            {
                                if (!ReadHistoryList(reader, prop))
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            ResultProperty resultProp = FindResultProperty(result.ResultProperties, prop.ScheduleProperty);

            if (resultProp != null)
                resultProp.CopyFrom(prop);
            else
                result.ResultProperties.Add(prop);

            return true;
        }

        private ResultProperty FindResultProperty(IList<ResultProperty> resultProperties, ScheduleProperty prop)
        {
            if (prop == null)
                return null;

            foreach (ResultProperty result in resultProperties)
            {
                if (result.ScheduleProperty == prop)
                    return result;
            }

            return null;
        }

        private bool ReadHistoryList(XmlTextReader reader, ResultProperty prop)
        {
            if (reader.IsEmptyElement)
                return true;

            bool stop = false;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "History", true) == 0)
                            {
                                ResultPropertyData data = ReadHistory(reader);

                                if (data == null)
                                    return false;
                                else
                                    prop.PropertyDatas.Add(data);
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return true;
        }

        private ResultPropertyData ReadHistory(XmlTextReader reader)
        {
            bool stop = false;

            DateTime dt;
            int nData;
            string strData = "";
            ResultPropertyData data = new ResultPropertyData();

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "ProjectName", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                data.ProjectName = strData;
                            }
                            else if (string.Compare(reader.Name, "ProjectCost", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                if (strData.Length > 0)
                                {
                                    long nCost;

                                    if (!long.TryParse(strData, out nCost))
                                        return null;
                                    else
                                        data.ProjectCost = new VariousData<long>(nCost);
                                }
                            }
                            else if (string.Compare(reader.Name, "BeginDate", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                if (strData.Length > 0)
                                {
                                    if (ScheduleProperty.ReadDateTimeString(strData, out dt))
                                        data.BeginTime = new VariousData<DateTime>(dt);
                                    else
                                        return null;
                                }
                            }
                            else if (string.Compare(reader.Name, "EndDate", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                if (strData.Length > 0)
                                {
                                    if (ScheduleProperty.ReadDateTimeString(strData, out dt))
                                        data.EndTime = new VariousData<DateTime>(dt);
                                    else
                                        return null;
                                }
                            }
                            else if (string.Compare(reader.Name, "AccumulLength", true) == 0 ||
                                string.Compare(reader.Name, "CompleteLength", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                if (strData.Length > 0)
                                {
                                    if (!int.TryParse(strData, out nData))
                                        return null;
                                    else
                                        data.AccumulLength = new VariousData<int>(nData);
                                }
                            }
                            else if (string.Compare(reader.Name, "UnitLength", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                if (strData.Length > 0)
                                {
                                    if (!int.TryParse(strData, out nData))
                                        return null;
                                    else
                                        data.UnitLength = new VariousData<int>(nData);
                                }
                            }
                            else if (string.Compare(reader.Name, "AccumulArea", true) == 0 ||
                                string.Compare(reader.Name, "CompleteArea", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                if (strData.Length > 0)
                                {
                                    if (!int.TryParse(strData, out nData))
                                        return null;
                                    else
                                        data.AccumulArea = new VariousData<int>(nData);
                                }
                            }
                            else if (string.Compare(reader.Name, "UnitArea", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                if (strData.Length > 0)
                                {
                                    if (!int.TryParse(strData, out nData))
                                        return null;
                                    else
                                        data.UnitArea = new VariousData<int>(nData);
                                }
                            }
                            else if (string.Compare(reader.Name, "CompleteFromBegin", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                if (strData.Length > 0)
                                {
                                    bool completeFromBegin;

                                    if (!bool.TryParse(strData, out completeFromBegin))
                                        return null;
                                    else
                                        data.DirectionFromBegin = new VariousData<bool>(completeFromBegin);
                                }
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return data;
        }

        private Dictionary<string, StreetCenterLine> ReadStreetCenterLines(XmlTextReader reader)
        {
            bool stop = false;
            Dictionary<string, StreetCenterLine> dicStreetCenterLines = new Dictionary<string, StreetCenterLine>();

            if (reader.IsEmptyElement)
                return dicStreetCenterLines;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "StreetCenterLine", true) == 0)
                            {
                                if (!ReadStreetCenterLine(reader, dicStreetCenterLines))
                                    return null;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return dicStreetCenterLines;
        }

        private bool ReadStreetCenterLine(XmlTextReader reader, Dictionary<string, StreetCenterLine> dicStreetCenterLines)
        {
            bool stop = false;

            if (reader.IsEmptyElement)
                return false;

            string strStreetName = "";
            // Key : Target(Boundary) Shape의 ID
            // Value : 중심선 PolyLine
            Dictionary<int, PolyLineEx> dicPolyLines = null;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "StreetName", true) == 0)
                            {
                                if (!ReadText(reader, ref strStreetName, true))
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "PolyLines", true) == 0)
                            {
                                dicPolyLines = ReadPolyLines(reader);

                                /*if (dicPolyLines == null)
                                    return false;*/
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            /*if (strStreetName.Length == 0)
            {
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", StreetName가 존재하지 않거나 비어 있습니다.";
                return false;
            }
            else if (dicPolyLines == null)
            {
                if (m_strErrorMessage.Length == 0)
                    m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", PolyLines가 존재하지 않거나 비어 있습니다.";

                return false;
            }*/

            if (strStreetName.Length > 0 && dicPolyLines != null)
            {
                StreetCenterLine centerLine = new StreetCenterLine();
                centerLine.StreetName = strStreetName;
                centerLine.PolyLines = dicPolyLines;

                dicStreetCenterLines[strStreetName] = centerLine;
            }

            return true;
        }

        // Key : Target(Boundary) Shape의 ID
        // Value : 중심선 PolyLine
        private Dictionary<int, PolyLineEx> ReadPolyLines(XmlTextReader reader)
        {
            bool stop = false;

            if (reader.IsEmptyElement)
                return null;

            Dictionary<int, PolyLineEx> dicPolyLines = new Dictionary<int, PolyLineEx>();

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "PolyLine", true) == 0)
                            {
                                if (!ReadPolyLine(reader, dicPolyLines))
                                    return null;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return dicPolyLines;
        }

        private bool ReadPolyLine(XmlTextReader reader, Dictionary<int, PolyLineEx> dicPolyLines)
        {
            bool stop = false;

            if (reader.IsEmptyElement)
                return false;

            int nTargetShapeID = -1;
            PolyLineEx polyLine = null;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "TargetShapeID", true) == 0)
                            {
                                string strID = "";

                                if (!ReadText(reader, ref strID, false))
                                    return false;

                                if (!int.TryParse(strID, out nTargetShapeID))
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Vertices", true) == 0)
                            {
                                polyLine = ReadPolyLine(reader, "Vertex");

                                if (polyLine == null)
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            if (nTargetShapeID <= 0)
            {
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", TargetShapeID가 존재하지 않거나 0보다 큰 값이 아닙니다.";
                return false;
            }
            else if (polyLine == null)
            {
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", Vertices가 존재하지 않거나 비어 있습니다.";
                return false;
            }

            dicPolyLines[nTargetShapeID] = polyLine;
            return true;
        }

        private PolyLineEx ReadPolyLine(XmlTextReader reader, string strTag)
        {
            bool stop = false;

            if (reader.IsEmptyElement)
                return null;

            ArrayList arrVertices = new ArrayList();

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, strTag, true) == 0)
                            {
                                UnE.Geometry.Vertex2D vertex = ReadVertex2D(reader);

                                if (vertex == null)
                                    return null;
                                else
                                    arrVertices.Add(vertex);
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            PolyLineEx polyLine = new PolyLineEx();
            polyLine.SetVertex(arrVertices);

            return polyLine;
        }

        private Dictionary<string, List<int>> ReadStreetShapes(XmlTextReader reader)
        {
            bool stop = false;
            Dictionary<string, List<int>> dicStreetShapes = new Dictionary<string, List<int>>();

            if (reader.IsEmptyElement)
                return dicStreetShapes;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "StreetShape", true) == 0)
                            {
                                if (!ReadStreetShape(reader, dicStreetShapes))
                                    return null;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return dicStreetShapes;
        }

        private bool ReadStreetShape(XmlTextReader reader, Dictionary<string, List<int>> dicStreetShapes)
        {
            bool stop = false;
            
            if (reader.IsEmptyElement)
                return false;

            string strStreetName = "";
            List<int> shapeIDs = null;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "StreetName", true) == 0)
                            {
                                if (!ReadText(reader, ref strStreetName, true))
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Shapes", true) == 0)
                            {
                                shapeIDs = ReadShapeIDs(reader);

                                if (shapeIDs == null)
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            if (strStreetName.Length == 0 || shapeIDs == null)
                return false;

            dicStreetShapes[strStreetName] = shapeIDs;
            return true;
        }

        private List<int> ReadShapeIDs(XmlTextReader reader)
        {
            bool stop = false;

            List<int> shapeIDs = new List<int>();

            if (reader.IsEmptyElement)
                return shapeIDs;

            string strID = "";
            int nID = -1;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "ID", true) == 0)
                            {
                                if (!ReadText(reader, ref strID, true))
                                    return null;

                                if (!int.TryParse(strID, out nID))
                                    return null;
                                else
                                    shapeIDs.Add(nID);
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return shapeIDs;
        }

        private bool ReadProcessSchedules(XmlTextReader reader)
        {
            bool stop = false;
            //m_panel.ClearProcessSchedule();

            if (reader.IsEmptyElement)
                return true;
            
            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "ProcessSchedule", true) == 0)
                            {
                                if (!ReadProcessSchedule(reader))
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return true;
        }

        private bool ReadProcessSchedule(XmlTextReader reader)
        {
            bool stop = false;
            
            ProcessSchedule schedule = new ProcessSchedule();

            try
            {
                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "name", true) == 0)
                    {
                        schedule.ScheduleName = reader.Value.ToString();
                    }
                    else if (string.Compare(reader.Name, "length", true) == 0)
                    {
                        schedule.Length = reader.Value.ToString();
                    }
                    else if (string.Compare(reader.Name, "desc", true) == 0)
                    {
                        schedule.Description = reader.Value.ToString();
                    }
                }

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Items", true) == 0)
                            {
                                if (!ReadProcessScheduleItems(reader, schedule))
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            m_panel.AddProcessSchedule(schedule);
            
            return true;
        }

        private bool ReadProcessScheduleItems(XmlTextReader reader, ProcessSchedule schedule)
        {
            bool stop = false;
            
            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Item", true) == 0)
                            {
                                if (!ReadProcessScheduleItem(reader, schedule))
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return true;
        }

        private bool ReadProcessScheduleItem(XmlTextReader reader, ProcessSchedule schedule)
        {
            bool stop = false;

            int nData;
            double dData;
            ScheduleProperty prop = new ScheduleProperty();

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Address", true) == 0)
                            {
                                string strAddress = "";

                                if (!ReadText(reader, ref strAddress, true))
                                    return false;

                                prop.StreetName = strAddress;
                            }
                            else if (string.Compare(reader.Name, "Importance", true) == 0)
                            {
                                ImportanceData data = ReadImportance(reader);

                                if (data == null)
                                    return false;
                                else
                                    prop.Importance = data;
                                /*string strImportance = "";

                                if (!ReadText(reader, ref strImportance, true))
                                    return false;

                                if (double.TryParse(strImportance, out dData))
                                    prop.Importance.Data = dData;
                                else
                                    return false;*/
                            }
                            else if (string.Compare(reader.Name, "Width", true) == 0)
                            {
                                string strWidth = "";

                                if (!ReadText(reader, ref strWidth, true))
                                    return false;

                                if (double.TryParse(strWidth, out dData))
                                    prop.Width = new VariousData<double>(dData);
                                else
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Length", true) == 0)
                            {
                                string strLength = "";

                                if (!ReadText(reader, ref strLength, true))
                                    return false;

                                if (int.TryParse(strLength, out nData))
                                    prop.Length = new VariousData<int>(nData);
                                else
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Area", true) == 0)
                            {
                                string strArea = "";

                                if (!ReadText(reader, ref strArea, true))
                                    return false;

                                if (double.TryParse(strArea, out dData))
                                    prop.Area = new VariousData<double>(dData);
                                else
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "LandAddrList", true) == 0)
                            {
                                if (!ReadLandAddrList(reader, prop))
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "FinalDate", true) == 0)
                            {
                                string strDate = "";

                                if (!ReadText(reader, ref strDate, true))
                                    return false;

                                if (strDate.Length == 0)
                                    prop.FinalDate = null;
                                else
                                {
                                    DateTime dt;

                                    if (ScheduleProperty.ReadDateTimeString(strDate, out dt))
                                        prop.FinalDate = new VariousData<DateTime>(dt);
                                    else
                                        prop.FinalDate = null;
                                }
                            }
                            else if (string.Compare(reader.Name, "FirstDate", true) == 0)
                            {
                                string strDate = "";

                                if (!ReadText(reader, ref strDate, true))
                                    return false;

                                if (strDate.Length == 0)
                                    prop.FirstDate = null;
                                else
                                {
                                    DateTime dt;

                                    if (ScheduleProperty.ReadDateTimeString(strDate, out dt))
                                        prop.FirstDate = new VariousData<DateTime>(dt);
                                    else
                                        prop.FirstDate = null;
                                }
                            }
                            else if (string.Compare(reader.Name, "Category", true) == 0)
                            {
                                string strCategory = "";

                                if (!ReadText(reader, ref strCategory, true))
                                    return false;

                                prop.Category = strCategory;
                            }
                            else if (string.Compare(reader.Name, "SubCategory", true) == 0)
                            {
                                string strSubCategory = "";

                                if (!ReadText(reader, ref strSubCategory, true))
                                    return false;

                                prop.SubCategory = strSubCategory;
                            }
                            else if (string.Compare(reader.Name, "PurposeOfLand", true) == 0)
                            {
                                if (!ReadPurposeOfLand(reader, prop))
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Cost", true) == 0)
                            {
                                if (!ReadCost(reader, prop))
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Complete", true) == 0)
                            {
                                string strComplete = "";

                                if (!ReadText(reader, ref strComplete, true))
                                    return false;

                                bool isComplete;

                                if (bool.TryParse(strComplete, out isComplete))
                                    prop.IsComplete = isComplete;
                                else
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Sectors", true) == 0)
                            {
                                if (!ReadSectors(reader, prop))
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            prop.Schedule = schedule;
            schedule.Properties.Add(prop);
            return true;
        }

        private bool ReadPurposeOfLand(XmlTextReader reader, ScheduleProperty prop)
        {
            if (reader.IsEmptyElement)
                return true;

            bool stop = false;
            double dData;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "RiceField", true) == 0)
                            {
                                string strRiceField = "";

                                if (!ReadText(reader, ref strRiceField, true))
                                    return false;

                                if (double.TryParse(strRiceField, out dData))
                                    prop.RiceField = new VariousData<double>(dData);
                                else
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Field", true) == 0)
                            {
                                string strField = "";

                                if (!ReadText(reader, ref strField, true))
                                    return false;

                                if (double.TryParse(strField, out dData))
                                    prop.Field = new VariousData<double>(dData);
                                else
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Land", true) == 0)
                            {
                                string strLand = "";

                                if (!ReadText(reader, ref strLand, true))
                                    return false;

                                if (double.TryParse(strLand, out dData))
                                    prop.Land = new VariousData<double>(dData);
                                else
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "ETC", true) == 0)
                            {
                                string strETC = "";

                                if (!ReadText(reader, ref strETC, true))
                                    return false;

                                if (double.TryParse(strETC, out dData))
                                    prop.ETC = new VariousData<double>(dData);
                                else
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return true;
        }

        private bool ReadCost(XmlTextReader reader, ScheduleProperty prop)
        {
            if (reader.IsEmptyElement)
                return true;

            bool stop = false;
            long nData;
            string strData = "";

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "LandCost", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return false;

                                if (long.TryParse(strData, out nData))
                                    prop.LandCost = new VariousData<long>(nData);
                                else
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "ObjectCost", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return false;

                                if (long.TryParse(strData, out nData))
                                    prop.ObjectCost = new VariousData<long>(nData);
                                else
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "AroundCost", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return false;

                                if (long.TryParse(strData, out nData))
                                    prop.AroundCost = new VariousData<long>(nData);
                                else
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return true;
        }

        private bool ReadLandAddrList(XmlTextReader reader, ScheduleProperty prop)
        {
            if (reader.IsEmptyElement)
                return true;

            bool stop = false;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "LandAddr", true) == 0)
                            {
                                LandAddressData addr = ReadLandAddr(reader);

                                if (addr == null)
                                    return false;
                                else
                                {
                                    prop.LandAddressDatas.Add(addr);

                                    LandAddressData2 addr2 = new LandAddressData2(addr);
                                    string strAddr2 = addr2.ToString();

                                    m_panel.DataManager.LandAddressDatas[strAddr2] = addr2;
                                }
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return true;
        }

        private LandAddressData ReadLandAddr(XmlTextReader reader)
        {
            bool stop = false;

            string strData = "";
            LandAddressData addr = new LandAddressData();

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "TownName", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                addr.TownName = strData;
                            }
                            else if (string.Compare(reader.Name, "MajorAddr", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                addr.MajorAddr = strData;
                            }
                            else if (string.Compare(reader.Name, "MinorAddr", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                addr.MinorAddr = strData;
                            }
                            else if (string.Compare(reader.Name, "TotalArea", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                if (strData.Length > 0)
                                {
                                    double dArea;

                                    if (!double.TryParse(strData, out dArea))
                                        return null;
                                    else
                                        addr.TotalArea = new VariousData<double>(dArea);
                                }
                            }
                            else if (string.Compare(reader.Name, "StreetArea", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                if (strData.Length > 0)
                                {
                                    double dArea;

                                    if (!double.TryParse(strData, out dArea))
                                        return null;
                                    else
                                        addr.StreetArea = new VariousData<double>(dArea);
                                }
                            }
                            else if (string.Compare(reader.Name, "OwnerType", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                addr.OwnerType = strData;
                            }
                            else if (string.Compare(reader.Name, "PublicEstimation", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                if (strData.Length > 0)
                                {
                                    long nCost;

                                    if (!long.TryParse(strData, out nCost))
                                        return null;
                                    else
                                        addr.PublicEstimation = new VariousData<long>(nCost);
                                }
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return addr;
        }

        private ImportanceData ReadImportance(XmlTextReader reader)
        {
            bool stop = false;

            int nData;
            string strData = "";
            ImportanceData data = new ImportanceData();

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "PeopleRequest", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                if (strData.Length > 0)
                                {
                                    if (!int.TryParse(strData, out nData))
                                        return null;

                                    data.PeopleRequest = nData;
                                }
                            }
                            else if (string.Compare(reader.Name, "Needs", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                if (strData.Length > 0)
                                {
                                    if (!int.TryParse(strData, out nData))
                                        return null;

                                    data.Needs = nData;
                                }
                            }
                            else if (string.Compare(reader.Name, "Right", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                if (strData.Length > 0)
                                {
                                    if (!int.TryParse(strData, out nData))
                                        return null;

                                    data.Right = nData;
                                }
                            }
                            else if (string.Compare(reader.Name, "NoDate", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                if (strData.Length > 0)
                                {
                                    if (!int.TryParse(strData, out nData))
                                        return null;

                                    data.NoDate = nData;
                                }
                            }
                            else if (string.Compare(reader.Name, "LandStatus", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                if (strData.Length > 0)
                                {
                                    if (!int.TryParse(strData, out nData))
                                        return null;

                                    data.LandStatus = nData;
                                }
                            }
                            else if (string.Compare(reader.Name, "Around", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                if (strData.Length > 0)
                                {
                                    if (!int.TryParse(strData, out nData))
                                        return null;

                                    data.Around = nData;
                                }
                            }
                            else if (string.Compare(reader.Name, "Level", true) == 0)
                            {
                                if (!ReadText(reader, ref strData, true))
                                    return null;

                                if (strData.Length > 0)
                                {
                                    if (!int.TryParse(strData, out nData))
                                        return null;

                                    data.Level = nData;
                                }
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return data;
        }

        private bool ReadSectors(XmlTextReader reader, ScheduleProperty prop)
        {
            if (reader.IsEmptyElement)
                return true;

            bool stop = false;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Sector", true) == 0)
                            {
                                if (!ReadSector(reader, prop))
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return true;
        }

        private bool ReadSector(XmlTextReader reader, ScheduleProperty prop)
        {
            bool stop = false;
            SchedulePropertySector_4_Read sector = new SchedulePropertySector_4_Read();

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Shape", true) == 0)
                            {
                                if (!ReadShape(reader, sector))
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "EditBoxHatch", true) == 0)
                            {
                                if (!ReadEditBoxHatch(reader, sector))
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            prop.Sectors.Add(sector);
            return true;
        }

        private bool ReadEditBoxVertices(XmlTextReader reader, EditBoxHatch hatch)
        {
            bool stop = false;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Vertex", true) == 0)
                            {
                                if (!ReadEditBoxVertex(reader, hatch))
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return true;
        }

        private bool ReadPolygon(XmlTextReader reader, System.Collections.ArrayList arrVertices)
        {
            bool stop = false;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Vertex", true) == 0)
                            {
                                UnE.Geometry.Vertex2D vertex = ReadVertex2D(reader);

                                if (vertex == null)
                                    return false;

                                arrVertices.Add(vertex);
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return true;
        }

        private bool ReadEditBoxHatch(XmlTextReader reader, SchedulePropertySector_4_Read sector)
        {
            bool stop = false;
            System.Collections.ArrayList arrVertices = new System.Collections.ArrayList();
            EditBoxHatch hatch = new EditBoxHatch(null);

            bool dirPos;
            int nIndex;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "EditBoxVertices", true) == 0)
                            {
                                if (!ReadEditBoxVertices(reader, hatch))
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "DirPos", true) == 0)
                            {
                                string strDirPos = "";

                                if (!ReadText(reader, ref strDirPos, true))
                                    return false;

                                if (bool.TryParse(strDirPos, out dirPos))
                                    hatch.DirPos = dirPos;
                                else
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "BeginIndex", true) == 0)
                            {
                                string strBeginIndex = "";

                                if (!ReadText(reader, ref strBeginIndex, true))
                                    return false;

                                if (int.TryParse(strBeginIndex, out nIndex))
                                    hatch.BeginIndex = nIndex;
                                else
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "EndIndex", true) == 0)
                            {
                                string strEndIndex = "";

                                if (!ReadText(reader, ref strEndIndex, true))
                                    return false;

                                if (int.TryParse(strEndIndex, out nIndex))
                                    hatch.EndIndex = nIndex;
                                else
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Polygon", true) == 0)
                            {
                                if (!ReadPolygon(reader, arrVertices))
                                    return false;
                                else
                                    hatch.SetVertex(arrVertices);
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            sector.Hatch = hatch;
            return true;
        }

        private bool ReadShape(XmlTextReader reader, SchedulePropertySector_4_Read sector)
        {
            bool stop = false;
            int nIndex;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "LayerIndex", true) == 0)
                            {
                                string strLayerIndex = "";

                                if (!ReadText(reader, ref strLayerIndex, true))
                                    return false;

                                if (int.TryParse(strLayerIndex, out nIndex))
                                    sector.LayerIndex = nIndex;
                                else
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "ShapeIndex", true) == 0)
                            {
                                string strShapeIndex = "";

                                if (!ReadText(reader, ref strShapeIndex, true))
                                    return false;

                                if (int.TryParse(strShapeIndex, out nIndex))
                                    sector.ShapeIndex = nIndex;
                                else
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return true;
        }


        private bool ReadViewport(XmlTextReader reader)
        {
            bool stop = false;
            m_panel.Viewport = new DXFViewer.Viewport();

            bool findMatrix = false, findTL = false, findBL = false, findBR = false, findWeight = false;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Matrix", true) == 0)
                            {
                                if (!ReadMatrix(reader))
                                    return false;
                                else
                                    findMatrix = true;
                            }
                            else if (string.Compare(reader.Name, "TopLeft", true) == 0)
                            {
                                m_panel.Viewport.TopLeft = ReadVertex2D(reader);

                                if (m_panel.Viewport.TopLeft == null)
                                    return false;
                                else
                                    findTL = true;
                            }
                            else if (string.Compare(reader.Name, "BottomLeft", true) == 0)
                            {
                                m_panel.Viewport.BottomLeft = ReadVertex2D(reader);

                                if (m_panel.Viewport.BottomLeft == null)
                                    return false;
                                else
                                    findBL = true;
                            }
                            else if (string.Compare(reader.Name, "BottomRight", true) == 0)
                            {
                                m_panel.Viewport.BottomRight = ReadVertex2D(reader);

                                if (m_panel.Viewport.BottomRight == null)
                                    return false;
                                else
                                    findBR = true;
                            }
                            else if (string.Compare(reader.Name, "Weight", true) == 0)
                            {
                                double dWeight = 0.0;

                                if (!ReadDouble(reader, ref dWeight, "Weight가", "Weight는"))
                                    return false;
                                else
                                {
                                    m_panel.Viewport.Weight = dWeight;
                                    findWeight = true;
                                }
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            if (!findMatrix)
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", Viewport에 Matrix가 존재하지 않습니다.";
            else if (!findTL)
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", Viewport에 TopLeft가 존재하지 않습니다.";
            else if (!findBL)
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", Viewport에 BottomLeft가 존재하지 않습니다.";
            else if (!findBR)
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", Viewport에 BottomRight가 존재하지 않습니다.";
            else if (!findWeight)
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", Viewport에 Weight가 존재하지 않습니다.";

            return findMatrix && findTL && findBL && findBR && findWeight;
        }

        private bool ReadMatrix(XmlTextReader reader)
        {
            bool stop = false;
            float f11 = 0.0f, f12 = 0.0f, f21 = 0.0f, f22 = 0.0f, fdx = 0.0f, fdy = 0.0f;
            bool findF11 = false, findF12 = false, findF21 = false, findF22 = false, findFDx = false, findFDy = false;

            bool isEmpty = reader.IsEmptyElement;

            while (reader.MoveToNextAttribute())
            {
                if (string.Compare(reader.Name, "f11", true) == 0)
                {
                    if (!float.TryParse(reader.Value.ToString(), System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Number, null, out f11))
                    {
                        m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", f11값은 숫자이어야 합니다.";
                        return false;
                    }
                    else
                        findF11 = true;
                }
                else if (string.Compare(reader.Name, "f12", true) == 0)
                {
                    if (!float.TryParse(reader.Value.ToString(), System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Number, null, out f12))
                    {
                        m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", f12값은 숫자이어야 합니다.";
                        return false;
                    }
                    else
                        findF12 = true;
                }
                else if (string.Compare(reader.Name, "f21", true) == 0)
                {
                    if (!float.TryParse(reader.Value.ToString(), System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Number, null, out f21))
                    {
                        m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", f21값은 숫자이어야 합니다.";
                        return false;
                    }
                    else
                        findF21 = true;
                }
                else if (string.Compare(reader.Name, "f22", true) == 0)
                {
                    if (!float.TryParse(reader.Value.ToString(), System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Number, null, out f22))
                    {
                        m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", f22값은 숫자이어야 합니다.";
                        return false;
                    }
                    else
                        findF22 = true;
                }
                else if (string.Compare(reader.Name, "fdx", true) == 0)
                {
                    if (!float.TryParse(reader.Value.ToString(), System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Number, null, out fdx))
                    {
                        m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", fdx값은 숫자이어야 합니다.";
                        return false;
                    }
                    else
                        findFDx = true;
                }
                else if (string.Compare(reader.Name, "fdy", true) == 0)
                {
                    if (!float.TryParse(reader.Value.ToString(), System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Number, null, out fdy))
                    {
                        m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", fdy값은 숫자이어야 합니다.";
                        return false;
                    }
                    else
                        findFDy = true;
                }
            }

            if (!isEmpty)
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }

            if (!findF11)
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", Matrix에 f11값이 존재하지 않습니다.";
            else if (!findF12)
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", Matrix에 f12값이 존재하지 않습니다.";
            else if (!findF21)
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", Matrix에 f21값이 존재하지 않습니다.";
            else if (!findF22)
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", Matrix에 f22값이 존재하지 않습니다.";
            else if (!findFDx)
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", Matrix에 fdx값이 존재하지 않습니다.";
            else if (!findFDy)
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", Matrix에 fdy값이 존재하지 않습니다.";

            m_panel.Viewport.F11 = f11;
            m_panel.Viewport.F12 = f12;
            m_panel.Viewport.F21 = f21;
            m_panel.Viewport.F22 = f22;
            m_panel.Viewport.FDx = fdx;
            m_panel.Viewport.FDy = fdy;

            return findF11 && findF12 && findF21 && findF22 && findFDx && findFDy;
        }

        private bool ReadProcessLayers(XmlTextReader reader, List<LayerData> layersAll)
        {
            bool stop = false;
            
            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "CompleteLayers", true) == 0)
                            {
                                if (!ReadProcessLayers(reader, m_panel.DataManager.CompleteLayers, layersAll))
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "IncompleteLayers", true) == 0)
                            {
                                if (!ReadProcessLayers(reader, m_panel.DataManager.IncompleteLayers, layersAll))
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "PartialLayers", true) == 0)
                            {
                                if (!ReadProcessLayers(reader, m_panel.DataManager.PartialLayers, layersAll))
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return true;
        }

        private LayerData FindLayerData(List<LayerData> layers, int nLayerIndex)
        {
            foreach (LayerData data in layers)
            {
                if (data.LayerIndex == nLayerIndex)
                    return data;
            }

            return null;
        }

        private bool ReadProcessLayers(XmlTextReader reader, List<LayerData> layers, List<LayerData> layersAll)
        {
            bool stop = false;
            layers.Clear();

            if (reader.IsEmptyElement)
                return true;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Layer", true) == 0)
                            {
                                uint nLayerIndex = 0;

                                if (!ReadUInt(reader, ref nLayerIndex, "Layer가", "Layer는"))
                                    return false;

                                LayerData data = FindLayerData(layersAll, (int)nLayerIndex);

                                if (data == null)
                                {
                                    m_strErrorMessage = string.Format("Line Number {0}, {1} 값은 유효하지 않습니다.", reader.LineNumber, nLayerIndex);
                                    return false;
                                }

                                layers.Add(data);
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return true;
        }

        private List<LayerData> ReadLayers(XmlTextReader reader)
        {
            bool stop = false;
            List<LayerData> arrLayers = new List<LayerData>();

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Layer", true) == 0)
                            {
                                LayerData layer = ReadLayer(reader);

                                if (layer != null)
                                    arrLayers.Add(layer);
                                else
                                    return null;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return arrLayers;
        }

        private LayerData ReadLayer(XmlTextReader reader)
        {
            bool stop = false;
            LayerData layer = new LayerData();

            bool findName = false, findVisible = false, findColor = false, findIndex = false;
			//bool findEnabled = false;
            while (reader.MoveToNextAttribute())
            {
                if (string.Compare(reader.Name, "index", true) == 0)
                {
                    int nIndex;

                    if (!int.TryParse(reader.Value.ToString(), out nIndex))
                    {
                        m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", index값은 0보다 같거나 큰 정수이어야 합니다.";
                        return null;
                    }
                    else
                    {
                        layer.LayerIndex = nIndex;
                        findIndex = true;
                    }
                }
            }

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Name", true) == 0)
                            {
                                string strLayerName = "";

                                if (!ReadText(reader, ref strLayerName))
                                {
                                    m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", Layer 이름은 비어있을수 없습니다.";
                                    return null;
                                }
                                
                                layer.LayerName = strLayerName;
                                findName = true;
                            }
                            else if (string.Compare(reader.Name, "Visible", true) == 0)
                            {
                                bool visible = false;

                                if (!ReadBoolean(reader, ref visible, "Visible이", "Visible은"))
                                    return null;
                                
                                layer.Visible = visible;
                                findVisible = true;
                            }
							else if( string.Compare(reader.Name, "Enabled", true) == 0)
							{
								bool enabled = true;

								if (!ReadBoolean(reader, ref enabled, "Enabled이", "Enabled은"))
									return null;

								layer.Enabled = enabled;
								//findEnabled = true;
							}
                            else if (string.Compare(reader.Name, "Color", true) == 0)
                            {
                                Color color = new Color();
                                int nAlpha = 255;

                                if (!ReadColor(reader, ref color, ref nAlpha))
                                    return null;
                                
                                layer.Color = color;
                                layer.Alpha = nAlpha;
                                findColor = true;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            if (findName && findVisible && findColor && findIndex)
                return layer;

            if (!findName)
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", Layer에 Name이 존재하지 않습니다.";
            else if (!findVisible)
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", Layer에 Visible이 존재하지 않습니다.";
            else if (!findColor)
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", Layer에 Color가 존재하지 않습니다.";
            else if (!findIndex)
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", Layer에 index가 존재하지 않습니다.";

            return null;
        }

        private bool ReadColor(XmlTextReader reader, ref Color color, ref int nAlpha)
        {
            bool stop = false;
            byte a = 0, r = 0, g = 0, b = 0;
            bool findR = false, findG = false, findB = false;

            bool isEmpty = reader.IsEmptyElement;

            while (reader.MoveToNextAttribute())
            {
                if (string.Compare(reader.Name, "a", true) == 0)
                {
                    if (!byte.TryParse(reader.Value.ToString(), out a))
                    {
                        m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", A값은 0~255사이의 정수이어야 합니다.";
                        return false;
                    }
                    else
                        nAlpha = a;
                }
                else if (string.Compare(reader.Name, "r", true) == 0)
                {
                    if (!byte.TryParse(reader.Value.ToString(), out r))
                    {
                        m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", R값은 0~255사이의 정수이어야 합니다.";
                        return false;
                    }
                    else
                        findR = true;
                }
                else if (string.Compare(reader.Name, "g", true) == 0)
                {
                    if (!byte.TryParse(reader.Value.ToString(), out g))
                    {
                        m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", G값은 0~255사이의 정수이어야 합니다.";
                        return false;
                    }
                    else
                        findG = true;
                }
                else if (string.Compare(reader.Name, "b", true) == 0)
                {
                    if (!byte.TryParse(reader.Value.ToString(), out b))
                    {
                        m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", B값은 0~255사이의 정수이어야 합니다.";
                        return false;
                    }
                    else
                        findB = true;
                }
            }

            if (!isEmpty)
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }

            if (!findR)
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", Color에 R값이 존재하지 않습니다.";
            else if (!findG)
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", Color에 G값이 존재하지 않습니다.";
            else if (!findB)
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", Color에 B값이 존재하지 않습니다.";

            color = Color.FromArgb((int)r, (int)g, (int)b);
            return findR && findG && findB;
        }

        private void PassElement(XmlTextReader reader)
        {
            if (reader.IsEmptyElement)
                return;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        return;
                }
            }
        }

        private bool ReadEditBoxVertex(XmlTextReader reader, EditBoxHatch hatch)
        {
            bool stop = false;
            double x = 0.0, y = 0.0;
            bool isDirect = false;
            bool findX = false, findY = false, findDirect = false;

            bool isEmpty = reader.IsEmptyElement;

            while (reader.MoveToNextAttribute())
            {
                if (string.Compare(reader.Name, "x", true) == 0)
                {
                    if (!double.TryParse(reader.Value.ToString(), System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Number, null, out x))
                    {
                        m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", x값은 숫자이어야 합니다.";
                        return false;
                    }
                    else
                        findX = true;
                }
                else if (string.Compare(reader.Name, "y", true) == 0)
                {
                    if (!double.TryParse(reader.Value.ToString(), System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Number, null, out y))
                    {
                        m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", y값은 숫자이어야 합니다.";
                        return false;
                    }
                    else
                        findY = true;
                }
                else if (string.Compare(reader.Name, "direct", true) == 0)
                {
                    if (!bool.TryParse(reader.Value.ToString(), out isDirect))
                    {
                        m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", direct 값은 boolean이어야 합니다.";
                        return false;
                    }
                    else
                        findDirect = true;
                }
            }

            if (!isEmpty)
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }

            if (!findX)
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", x값이 존재하지 않습니다.";
            else if (!findY)
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", y값이 존재하지 않습니다.";
            else if (!findDirect)
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", direct값이 존재하지 않습니다.";

            if (!findX || !findY || !findDirect)
                return false;

            hatch.AddEditBoxVertex2(new UnE.Geometry.Vertex2D(x, y));

            int nVertexCount = hatch.GetEditBoxVertexCount();
            hatch.SetDirectLink(nVertexCount - 1, isDirect);

            return true;
        }

        private UnE.Geometry.Vertex2D ReadVertex2D(XmlTextReader reader)
        {
            bool stop = false;
            double x = 0.0, y = 0.0;
            bool findX = false, findY = false;

            bool isEmpty = reader.IsEmptyElement;

            while (reader.MoveToNextAttribute())
            {
                if (string.Compare(reader.Name, "x", true) == 0)
                {
                    if (!double.TryParse(reader.Value.ToString(), System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Number, null, out x))
                    {
                        m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", x값은 숫자이어야 합니다.";
                        return null;
                    }
                    else
                        findX = true;
                }
                else if (string.Compare(reader.Name, "y", true) == 0)
                {
                    if (!double.TryParse(reader.Value.ToString(), System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Number, null, out y))
                    {
                        m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", y값은 숫자이어야 합니다.";
                        return null;
                    }
                    else
                        findY = true;
                }
            }

            if (!isEmpty)
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }

            if (!findX)
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", x값이 존재하지 않습니다.";
            else if (!findY)
                m_strErrorMessage = "Line Number " + reader.LineNumber.ToString() + ", y값이 존재하지 않습니다.";

            if (!findX || !findY)
                return null;

            return new UnE.Geometry.Vertex2D(x, y);
        }

        private bool ReadText(XmlTextReader reader, ref string strText, bool allowEmpty = false)
        {
            if (reader.IsEmptyElement)
            {
                strText = "";
                return allowEmpty;
            }

            if (!ReadElementText(reader, ref strText))
                strText = "";

            return true;
        }

        private bool ReadUInt(XmlTextReader reader, ref uint nData, string strMessage1, string strMessage2)
        {
            string strText = "";

            if (!ReadElementText(reader, ref strText))
            {
                m_strErrorMessage = string.Format("Line Number {0}, {1} 비어있습니다.", reader.LineNumber, strMessage1);
                return false;
            }

            if (!uint.TryParse(strText, out nData))
            {
                m_strErrorMessage = string.Format("Line Number {0}, {1} 0보다 크거나 같은 정수 형태의 숫자이어야만 합니다.", reader.LineNumber, strMessage2);
                return false;
            }

            return true;
        }

        private bool ReadInt(XmlTextReader reader, ref int nData, string strMessage1, string strMessage2)
        {
            string strText = "";

            if (!ReadElementText(reader, ref strText))
            {
                m_strErrorMessage = string.Format("Line Number {0}, {1} 비어있습니다.", reader.LineNumber, strMessage1);
                return false;
            }

            if (!int.TryParse(strText, out nData))
            {
                m_strErrorMessage = string.Format("Line Number {0}, {1} 정수 형태의 숫자이어야만 합니다.", reader.LineNumber, strMessage2);
                return false;
            }

            return true;
        }

        private bool ReadDouble(XmlTextReader reader, ref double dData, string strMessage1, string strMessage2)
        {
            string strText = "";

            if (!ReadElementText(reader, ref strText))
            {
                m_strErrorMessage = string.Format("Line Number {0}, {1} 비어있습니다.", reader.LineNumber, strMessage1);
                return false;
            }

            if (!double.TryParse(strText, System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Number, null, out dData))
            {
                m_strErrorMessage = string.Format("Line Number {0}, {1} 숫자이어야만 합니다.", reader.LineNumber, strMessage2);
                return false;
            }

            return true;
        }

        private bool ReadBoolean(XmlTextReader reader, ref bool bData, string strMessage1, string strMessage2)
        {
            string strText = "";

            if (!ReadElementText(reader, ref strText))
            {
                m_strErrorMessage = string.Format("Line Number {0}, {1} 비어있습니다.", reader.LineNumber, strMessage1);
                return false;
            }

            try
            {
                if (string.Compare(strText, "true", true) == 0)
                    bData = true;
                else if (string.Compare(strText, "false", true) == 0)
                    bData = false;
                else
                    bData = int.Parse(strText) == 0 ? false : true;
            }
            catch (Exception)
            {
                m_strErrorMessage = string.Format("Line Number {0}, {1} true, false, 0 또는 1로 표현되어야만 합니다.", reader.LineNumber, strMessage2);
                return false;
            }

            return true;
        }

        private bool ReadElementText(XmlTextReader reader, ref string strText)
        {
            bool stop = false, readText = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Text:
                        strText = reader.Value;
                        readText = true;
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return readText;
        }
    }
}
