using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace DidUIEditor
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
                        if (string.Compare(reader.Name, "Emergency", true) == 0)
                        {
                            ReadEmergency(reader);
                        }
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
            Size size = new Size(1920 / 2, 1080 / 2);
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
                    point = new Point(Convert.ToInt32(layouts[0]) / 2, Convert.ToInt32(layouts[1]) / 2);
                    size = new Size(Convert.ToInt32(layouts[2]) / 2, Convert.ToInt32(layouts[3]) / 2);
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
            {
                page.strBackgroundIMG = strSystemPageBackImage;
                switch (page.strBackgroundIMG)
                {
                    case "systemstyle0.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._0_기본; break;
                    case "systemstyle1.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._1_근로자현황; break;
                    case "systemstyle2.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._2_안전조치사항_일반위험작업_; break;
                    case "systemstyle3.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._3_안전조치사항_화재작업_; break;
                    case "systemstyle4.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._4_안전조치사항_정전작업_; break;
                    case "systemstyle5.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._5_안전조치사항_밀폐공간작업_; break;
                    case "systemstyle6.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._6_안전조치사항_고소작업_; break;
                    case "systemstyle7.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._7_안전조치사항_굴착_작업_; break;
                    //case "systemstyle8.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._8_방재장비_배치도; break;
                    case "systemstyle9.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._9_안전조치사항; break;
                    case "systemstyle10.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._10_안전조치사항; break;
                    case "systemstyle11.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._11_안전조치사항; break;
                    case "systemstyle12.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._12_안전조치사항; break;
                    case "systemstyle13.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._13_안전조치사항; break;
                    case "systemstyle14.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._14_안전조치사항; break;
                    case "systemstyle15.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._15_안전조치사항; break;
                    case "systemstyle16.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._16_안전조치사항; break;
                    case "systemstyle17.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._17_안전조치사항; break;
                    case "systemstyle18.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._18_안전조치사항; break;
                    case "systemstyle19.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._19_안전조치사항; break;
                    case "systemstyle20.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._20_안전조치사항; break;
                    case "systemstyle21.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._21_안전조치사항; break;
                    case "systemstyle22.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._22_안전조치사항; break;
                    case "systemstyle23.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._23_안전조치사항; break;
                    case "systemstyle24.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._24_안전조치사항; break;
                    case "systemstyle25.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._25_안전조치사항; break;
                    case "systemstyle26.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._26_안전조치사항; break;
                    case "systemstyle27.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._27_안전조치사항; break;
                    case "systemstyle28.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._28_안전조치사항; break;
                    case "systemstyle29.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._29_안전조치사항; break;
                    case "systemstyle30.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._30_안전조치사항; break;
                    case "systemstyle31.png": page.BackgroundIMG = global::DidUIEditor.Properties.Resources._31_안전조치사항; break;
                }
            }
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
                                FormMain.Instance.WebMgr.Download(page.strBackgroundIMG);
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

                                media.MediaLocation = new Point(Convert.ToInt32(strPTs[0]) / 2, Convert.ToInt32(strPTs[1]) / 2);
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

                                media.MediaSize = new Size(Convert.ToInt32(strSizes[0]) / 2, Convert.ToInt32(strSizes[1]) / 2);
                            }
                        }
                        else if (string.Compare(reader.Name, "File", true) == 0)
                        {
                            string filePath = "";
                            if (ReadElementText(reader, ref filePath))
                            {
                                media.File = filePath;
                                FormMain.Instance.WebMgr.Download(media.File);
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

        #region Write
        public bool Save()
        {
            try
            {
                XmlTextWriter writer = new XmlTextWriter(m_strXMLFilePath, Encoding.UTF8);
                writer.Formatting = Formatting.Indented;

                writer.WriteStartDocument();

                writer.WriteStartElement("DID");

                writer.WriteStartAttribute("xmlns:xsi");
                writer.WriteString("http://www.w3.org/2001/XMLSchema-instance");
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("version");
                writer.WriteString("1.0");
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("xsi:noNamespaceSchemaLocation");
                writer.WriteString("http://unes.iptime.org:8001/Schema/DID.xsd");
                writer.WriteEndAttribute();

                if (FormMain.Instance.HaveNormalPages.Count > 0)
                {
                    if (!WriteNormal(writer))
                        return false;
                }
                if (FormMain.Instance.HaveEmergencyPages.Count > 0)
                {
                    if (!WriteEmergency(writer))
                        return false;
                }


                writer.WriteFullEndElement();

                writer.WriteEndDocument();
                writer.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            return true;
        }

        private bool WriteNormal(XmlTextWriter writer)
        {
            writer.WriteStartElement("Normal");

            foreach (Page page in FormMain.Instance.HaveNormalPages)
            {
                if (!WritePage(writer, page))
                    return false;
            }

            writer.WriteEndElement();

            return true;
        }

        private bool WriteEmergency(XmlTextWriter writer)
        {
            writer.WriteStartElement("Emergency");

            foreach (Page page in FormMain.Instance.HaveEmergencyPages)
            {
                writer.WriteStartElement("Disaster");
                writer.WriteStartAttribute("type");
                writer.WriteString(page.DisasterType.ToString().ToLower());
                writer.WriteEndAttribute();
                if (!WritePage(writer, page))
                    return false;
                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            return true;
        }

        private bool WritePage(XmlTextWriter writer, Page page)
        {
            if (page.PageType == PageType.System)
                writer.WriteStartElement("SystemPage");
            else if (page.PageType == PageType.User)
                writer.WriteStartElement("UserPage");
            else if (page.PageType == PageType.None)
            {
                if (page.Medias.Count == 0)
                    return false;

                if (WriteMedia(writer, page.Medias[0]))
                    return true;
                else
                    return false;
            }
            
            writer.WriteStartAttribute("name");
            if (page.PageType == PageType.System)
                writer.WriteString(page.Name + uniqueKey + page.strBackgroundIMG + uniqueKey);
            else
                writer.WriteString(page.Name);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("playSeconds");
            writer.WriteString(page.PlaySeconds.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("layout");
            Point pt = new Point(page.PageLocation.X * 2, page.PageLocation.Y * 2);
            Size size = new Size(page.PageSize.Width * 2, page.PageSize.Height * 2);            
            writer.WriteString(pt.X + "," + pt.Y + "," + size.Width + "," + size.Height);
            writer.WriteEndAttribute();

            if (page.strBackgroundIMG != null && page.strBackgroundIMG.Length > 0 && page.PageType == PageType.User) 
            {
                writer.WriteStartElement("Background");
                writer.WriteStartAttribute("layout");
                writer.WriteString("stretch");
                writer.WriteEndAttribute();
                writer.WriteString(page.strBackgroundIMG);
                writer.WriteEndElement();

                if (page.strBackgroundIMG != null && page.strBackgroundIMG.Length > 0)
                    FormMain.Instance.WebMgr.Upload(FormMain.Instance.MakeMediaFilePath(page.strBackgroundIMG));
            }

            foreach (Page childPage in page.ChildPages)
            {
                if (!WritePage(writer, childPage))
                    return false;
            }

            foreach (Media media in page.Medias)
            {
                if (!WriteMedia(writer, media))
                    return false;
            }

            writer.WriteEndElement();

            return true;
        }

        private bool WriteMedia(XmlTextWriter writer, Media media)
        {
            if (media.MediaType == MediaType.Image)
                writer.WriteStartElement("Image");
            else if (media.MediaType == MediaType.Movie)
            {
                writer.WriteStartElement("Movie");

                writer.WriteStartAttribute("runningSeconds");
                writer.WriteString(media.RunningSeconds.ToString());
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("beginSeconds");
                writer.WriteString(media.BeginSeconds.ToString());
                writer.WriteEndAttribute();
            }

            writer.WriteStartElement("Position");
            writer.WriteString(media.MediaLocation.X * 2 + "," + media.MediaLocation.Y * 2);
            writer.WriteEndElement();

            writer.WriteStartElement("Size");
            writer.WriteString(media.MediaSize.Width * 2 + "," + media.MediaSize.Height * 2);
            writer.WriteEndElement();

            writer.WriteStartElement("File");
            writer.WriteString(media.File);
            writer.WriteEndElement();
            if (media.File != null && media.File.Length > 0)
                FormMain.Instance.WebMgr.Upload(FormMain.Instance.MakeMediaFilePath(media.File));

            writer.WriteEndElement();

            return true;
        }
        #endregion
    }
}
