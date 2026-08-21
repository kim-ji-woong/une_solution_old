using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DidViewer.Composition;

namespace DidViewer
{
    public enum DisasterType { Normal = 0, Fire, PSM }
    /// <summary>
    /// None은 Media
    /// </summary>
    public enum PageType { None = 0, System = 1, User }

    public class Page
    {
        private DisasterType m_disasterType = DisasterType.Normal;
        public DisasterType DisasterType
        {
            get { return m_disasterType; }
            set { m_disasterType = value; }
        }

        private PageType m_pageType = PageType.System;
        public PageType PageType
        {
            get { return m_pageType; }
            set { m_pageType = value; }
        }

        private int m_nSequence = -1;
        /// <summary>
        /// 순서
        /// </summary>
        public int Sequence
        {
            get { return m_nSequence; }
            set { m_nSequence = value; }
        }

        private string m_strName { get; set; }
        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        private int m_nPlaySeconds = 30;
        public int PlaySeconds
        {
            get { return m_nPlaySeconds; }
            set { m_nPlaySeconds = value; }
        }

        private Image m_backgroundIMG { get; set; }
        public Image BackgroundIMG
        {
            get { return m_backgroundIMG; }
            set { m_backgroundIMG = value; }
        }

        private string m_strBackgroundIMG { get; set; }
        public String strBackgroundIMG
        {
            get { return m_strBackgroundIMG; }
            set { m_strBackgroundIMG = value; }
        }

        private Point m_pageLocation { get; set; }
        public Point PageLocation
        {
            get { return m_pageLocation; }
            set { m_pageLocation = value; }
        }

        private Size m_pageSize { get; set; }
        public Size PageSize
        {
            get { return m_pageSize; }
            set { m_pageSize = value; }
        }

        private List<Page> m_childPages = new List<Page>();
        public List<Page> ChildPages
        {
            get { return m_childPages; }
            set { m_childPages = value; }
        }

        private List<Media> m_medias = new List<Media>();
        public List<Media> Medias
        {
            get { return m_medias; }
            set { m_medias = value; }
        }
    }

    public class EmergencyPage
    {
        public int Index = -1; // 0:업체정보, 1:대피로
        public EmergencyMode EmergencyMode = EmergencyMode.Fire;
        public ArrayList ArrShowInfo = new ArrayList();
        public int nEquipmentZoneID = -1;
        public string strMessage = "";
    }
}
