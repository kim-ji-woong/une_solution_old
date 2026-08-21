using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using DidUIEditor;

namespace DidViewer
{
    public class XMLManager
    {
        private string m_strXMLFilePath = Application.StartupPath + "\\Files\\DID_UI.xml";
        private readonly string uniqueKey = "%^!";

        #region Read
        public void LoadXML()
        {
            string filePath = m_strXMLFilePath;

            XmlTextReader reader = new XmlTextReader(filePath);
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Normal", true) == 0)
                        {
                            ReadNormal(reader);
                        }
                        //if (string.Compare(reader.Name, "Emergency", true) == 0)
                        //{
                        //    ReadEmergency(reader);
                        //}
                        break;
                }
            }

            reader.Close();
        }

        private void ReadNormal(XmlTextReader reader)
        {
            bool stop = false;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "SystemPage", true) == 0)
                        {
                            Page page = ReadPage(reader, PageType.System);
                            if (page != null)
                                FormMain.Instance.HaveNormalPages.Add(page);
                        }
                        else if (string.Compare(reader.Name, "UserPage", true) == 0)
                        {
                            Page page = ReadPage(reader, PageType.User);
                            if (page != null)
                                FormMain.Instance.HaveNormalPages.Add(page);
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

        private void ReadEmergency(XmlTextReader reader)
        {
            bool stop = false;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Disaster", true) == 0)
                        {
                            string disasterType = reader.GetAttribute("type");
                            if (disasterType == "fire")
                                ReadEmergency(reader, DisasterType.Fire);
                            else if (disasterType == "psm")
                                ReadEmergency(reader, DisasterType.PSM);
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

        private void ReadEmergency(XmlTextReader reader, DisasterType disasterType)
        {
            bool stop = false;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "SystemPage", true) == 0)
                        {
                            Page page = ReadPage(reader, PageType.System);
                            page.DisasterType = disasterType;
                            if (page != null)
                                FormMain.Instance.HaveEmergencyPages.Add(page);
                        }
                        else if (string.Compare(reader.Name, "UserPage", true) == 0)
                        {
                            Page page = ReadPage(reader, PageType.User);
                            page.DisasterType = disasterType;
                            if (page != null)
                                FormMain.Instance.HaveEmergencyPages.Add(page);
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

        private Page ReadPage(XmlTextReader reader, PageType pageType)
        {
            bool isEmpty = reader.IsEmptyElement;

            Page page = null;

            string strName = null;
            int nPlaySec = 10; // default 10초
            bool bSetLayout = false;
            Point point = new Point(0, 0);
            Size size = new Size(1920, 1080);
            string strSystemPageBackImage = null;

            while (reader.MoveToNextAttribute())
            {
                if (string.Compare(reader.Name, "name", true) == 0)
                {
                    string fullName = reader.Value;
                    if (pageType == PageType.System)
                    {
                        int nIndex = fullName.IndexOf(uniqueKey);
                        if (nIndex > 0)
                        {
                            strSystemPageBackImage = fullName.Substring(nIndex).Replace(uniqueKey, "");
                            fullName = fullName.Substring(0, nIndex);
                        }
                    }

                    strName = fullName;
                }
                else if (string.Compare(reader.Name, "playSeconds", true) == 0)
                {
                    int.TryParse(reader.Value, out nPlaySec);
                }
                else if (string.Compare(reader.Name, "layout", true) == 0)
                {
                    string strLayout = reader.Value;
                    string[] layouts = strLayout.Split(',');
                    if (layouts.Length != 4)
                        continue;

                    bSetLayout = true;
                    point = new Point(Convert.ToInt32(layouts[0]), Convert.ToInt32(layouts[1]));
                    size = new Size(Convert.ToInt32(layouts[2]), Convert.ToInt32(layouts[3]));
                }
            }

            if (strName == null)
            {
                System.Diagnostics.Trace.WriteLine("ReadPage() - strName null ");
                return null;
            }

            if (pageType == PageType.System && strSystemPageBackImage == null)
            {
                System.Diagnostics.Trace.WriteLine("ReadPage() - strSystemPageBackImage null ");
                return null;
            }

            page = new Page();
            page.PageType = pageType;
            page.Name = strName;
            page.PlaySeconds = nPlaySec;
            if (pageType == PageType.System)
                page.strBackgroundIMG = strSystemPageBackImage;
            //if (bSetLayout)
            {
                page.PageLocation = point;
                page.PageSize = size;
            }

            if (isEmpty)
                return page;

            bool stop = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "UserPage", true) == 0)
                        {
                            Page childPage = ReadPage(reader, PageType.User);
                            if (childPage != null)
                            {
                                if (page.ChildPages == null)
                                    page.ChildPages = new List<Page>();
                                page.ChildPages.Add(childPage);
                            }
                        }
                        else if (string.Compare(reader.Name, "Background", true) == 0)
                        {
                            string strBackgroundPath = "";
                            if (ReadElementText(reader, ref strBackgroundPath))
                            {
                                page.strBackgroundIMG = strBackgroundPath;
                                if (!FormMain.Instance.DownloadList.Contains(page.strBackgroundIMG))
                                {
                                    if (FormMain.Instance.WebMgr.Download(page.strBackgroundIMG))
                                        FormMain.Instance.DownloadList.Add(page.strBackgroundIMG);
                                }
                            }
                        }
                        else if (string.Compare(reader.Name, "Image", true) == 0)
                        {
                            Media media = ReadMedia(reader, MediaType.Image);
                            if (media != null)
                                page.Medias.Add(media);
                        }
                        else if (string.Compare(reader.Name, "Movie", true) == 0)
                        {
                            Media media = ReadMedia(reader, MediaType.Movie);
                            if (media != null)
                                page.Medias.Add(media);
                        }
                        break;
                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return page;
        }

        private Media ReadMedia(XmlTextReader reader, MediaType mediaType)
        {
            Media media = new Media();
            media.MediaType = mediaType;

            bool stop = false;

            if (mediaType == MediaType.Movie)
            {
                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "runningSeconds", true) == 0)
                    {
                        int nRunningSec = 30;
                        int.TryParse(reader.Value, out nRunningSec);
                        media.RunningSeconds = nRunningSec;
                    }
                    else if (string.Compare(reader.Name, "beginSeconds", true) == 0)
                    {
                        int nBeginSec = 0;
                        int.TryParse(reader.Value, out nBeginSec);
                        media.BeginSeconds = nBeginSec;
                    }
                }
            }

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Position", true) == 0)
                        {
                            string strPT = "";
                            if (ReadElementText(reader, ref strPT))
                            {
                                string[] strPTs = strPT.Split(',');
                                if (strPTs.Length != 2)
                                    break;

                                media.MediaLocation = new Point(Convert.ToInt32(strPTs[0]), Convert.ToInt32(strPTs[1]));
                            }
                        }
                        else if (string.Compare(reader.Name, "Size", true) == 0)
                        {
                            string strSize = "";
                            if (ReadElementText(reader, ref strSize))
                            {
                                string[] strSizes = strSize.Split(',');
                                if (strSizes.Length != 2)
                                    break;

                                media.MediaSize = new Size(Convert.ToInt32(strSizes[0]), Convert.ToInt32(strSizes[1]));
                            }
                        }
                        else if (string.Compare(reader.Name, "File", true) == 0)
                        {
                            string filePath = "";
                            if (ReadElementText(reader, ref filePath))
                            {
                                media.File = filePath;
                                if (!FormMain.Instance.DownloadList.Contains(media.File))
                                {
                                    if (FormMain.Instance.WebMgr.Download(media.File))
                                        FormMain.Instance.DownloadList.Add(media.File);
                                }
                            }
                        }
                        break;
                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }


            return media;
        }

        private void PassElement(XmlTextReader reader)
        {
            if (reader.IsEmptyElement)
                return;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        PassElement(reader);
                        break;
                    case XmlNodeType.EndElement:
                        return;
                }
            }
        }

        private bool ReadElementText(XmlTextReader reader, ref string strText)
        {
            if (reader.IsEmptyElement)
            {
                strText = "";
                return true;
            }

            bool stop = false;
            strText = "";

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Text:
                        strText = reader.Value;
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return true;
        }

        private bool ReadElementInt(XmlTextReader reader, out int data)
        {
            bool stop = false;
            string strData = null;
            data = 0;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Text:
                        strData = reader.Value;
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            if (strData == null)
                return false;

            return int.TryParse(strData, out data);
        }
        #endregion
    }
}
