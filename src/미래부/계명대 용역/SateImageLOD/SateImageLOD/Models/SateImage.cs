using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SateImageLOD.Models
{
    public class SateImage
    {
        private int m_nID = -1;
        private string m_strURL = "";
        private string m_strImageName = "";
        private int m_nLODLevel = 0;
        private SateImage m_imageParent = null;
        private int m_nHIndex = 0;
        private int m_nVIndex = 0;
        private int m_nRegionID = 0;
        private List<SateImage> m_childImages = null;
        private List<int> m_childImageIDs = null;
        private Vertex2F m_vTL = new Vertex2F();
        private Vertex2F m_vBL = new Vertex2F();
        private Vertex2F m_vBR = new Vertex2F();
        // 이미지가 촬영된 시간
        private DateTime m_time = new DateTime();
        // 1픽셀이 몇 meter를 나타내는가를 의미
        private float m_fScale = 0.0f;
        private string m_strDescription = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string URL
        {
            get { return m_strURL; }
            set { m_strURL = value; }
        }

        public string ImageName
        {
            get { return m_strImageName; }
            set { m_strImageName = value; }
        }

        public int LODLevel
        {
            get { return m_nLODLevel; }
            set { m_nLODLevel = value; }
        }

        public int ParentImageID
        {
            get { return m_imageParent == null ? 0 : m_imageParent.ID; }
        }

        public int HIndex
        {
            get { return m_nHIndex; }
            set { m_nHIndex = value; }
        }

        public int VIndex
        {
            get { return m_nVIndex; }
            set { m_nVIndex = value; }
        }

        public int RegionID
        {
            get { return m_nRegionID; }
            set { m_nRegionID = value; }
        }

        public float TLx
        {
            get { return m_vTL.X; }
            set { m_vTL.X = value; }
        }

        public float TLy
        {
            get { return m_vTL.Y; }
            set { m_vTL.Y = value; }
        }

        public float BLx
        {
            get { return m_vBL.X; }
            set { m_vBL.X = value; }
        }

        public float BLy
        {
            get { return m_vBL.Y; }
            set { m_vBL.Y = value; }
        }

        public float BRx
        {
            get { return m_vBR.X; }
            set { m_vBR.X = value; }
        }

        public float BRy
        {
            get { return m_vBR.Y; }
            set { m_vBR.Y = value; }
        }

        public string Time
        {
            get
            {
                return string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}",
                    m_time.Year, m_time.Month, m_time.Day, m_time.Hour, m_time.Minute, m_time.Second);
            }
            set
            {
                try
                {
                    m_time = Convert.ToDateTime(value);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                }
            }
        }

        public float Scale
        {
            get { return m_fScale; }
            set { m_fScale = value; }
        }
        
        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public void SetParentImage(SateImage image)
        {
            if (image == this)
                return;

            m_imageParent = image;
            image.AddChildImage2(this);
        }

        // Parent에 영향주지 않음
        private void AddChildImage2(SateImage image)
        {
            if (this.m_childImages == null)
            {
                this.m_childImages = new List<SateImage>();
                this.m_childImageIDs = new List<int>();
            }
            else if (this.m_childImages.Contains(image))
                return;

            this.m_childImages.Add(image);
            this.m_childImageIDs.Add(image.ID);
        }

        public void AddChildImage(SateImage image)
        {
            if (image == this)
                return;

            image.m_imageParent = this;

            if (this.m_childImages == null)
            {
                this.m_childImages = new List<SateImage>();
                this.m_childImageIDs = new List<int>();
            }
            else if (this.m_childImages.Contains(image))
                return;

            this.m_childImages.Add(image);
            this.m_childImageIDs.Add(image.ID);
        }

        public List<SateImage> GetChildImages()
        {
            return m_childImages;
        }

        public List<int> GetChildImageIDs()
        {
            return m_childImageIDs;
        }
    }
}
