using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using DidViewer;
using System.IO;
using AxWMPLib;

namespace DIDViewer
{
    public enum DisasterType { NORMAL = 0, FIRE, PSM }
    public enum PageType { SYSTEM = 0, USER }
    public enum MediaType { IMAGE = 0, MOVIE }

    public class cXmlMediaInfo
    {
        private MediaType m_mType = MediaType.IMAGE;
        public MediaType MediaType { get { return m_mType; } set { m_mType = value; } }

        private Point m_Position = new Point();//img
        public Point Position { get { return m_Position; } set { m_Position = value; } }

        private Size m_Size = new Size();
        public Size Size { get { return m_Size; } set { m_Size = value; } }

        private string m_sMediaFileName = "";
        public string MediaFileName { get { return m_sMediaFileName; } set { m_sMediaFileName = value; } }

        private Image m_mediaImage = null;
        public Image Image { get { return m_mediaImage; } set { m_mediaImage = value; } }

        private AxWindowsMediaPlayer m_mediaPlayer = null;
        public AxWindowsMediaPlayer Player { get { return m_mediaPlayer; } set { m_mediaPlayer = value; } }

        private int m_MovieBeginSeconds = 0;
        public int MovieBeginSeconds { get { return m_MovieBeginSeconds; } set { m_MovieBeginSeconds = value; } }

        private int m_MovieRunningSeconds = 0;
        public int MovieRunningSeconds { get { return m_MovieRunningSeconds; } set { m_MovieRunningSeconds = value; } }

        public Timer m_movieTimer = new Timer();
        public void timerTick() { m_mediaPlayer.Ctlcontrols.stop(); }
        
       
    }
    public class cXmlPageInfo
    {
        private PageType m_pageType = PageType.SYSTEM; //system page or user page
        public PageType PageType { get { return m_pageType; } set { m_pageType = value; } }

        private DisasterType m_disasterType = DisasterType.NORMAL;
        public DisasterType DisasterType { get { return m_disasterType; } set { m_disasterType = value; } }

        private string m_strName = "";
        public string Name { get { return m_strName; } set { m_strName = value; } }

        private Point m_backImgPosition = new Point();
        public Point BackImgPosition { get { return m_backImgPosition; } set { m_backImgPosition = value; } }

        private Size m_backImgSize = new Size();
        public Size BackImgSize { get { return m_backImgSize; } set { m_backImgSize = value; } }

        private string m_backImgFileName = "";
        public string BackImgFileName { get { return m_backImgFileName; } set { m_backImgFileName = value; } }

        private Image m_BackImage = null;
        public Image BackImage { get { return m_BackImage; } set { m_BackImage = value; } }

        private int m_playSeconds = 0;
        public int PlaySeconds { get { return m_playSeconds; } set { m_playSeconds = value; } }

        private List<cXmlPageInfo> m_childPages = new List<cXmlPageInfo>();
        public List<cXmlPageInfo> Childs
        {
            get { return m_childPages; }
        }

        private List<cXmlMediaInfo> m_Medias = new List<cXmlMediaInfo>();
        public List<cXmlMediaInfo> Medias
        {
            get { return m_Medias; }
        }
    }
}
