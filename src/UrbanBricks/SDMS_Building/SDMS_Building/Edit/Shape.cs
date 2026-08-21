using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDMS_Building.Edit
{
    public class Shape
    {
        private static Image DefaultImage = global::SDMS_Building.Properties.Resources.poi_cctv_normal;

        private string m_strImagePath = "";
        private PointF m_ptPosition = new PointF();
        //private bool m_useLODImage = false;
        //private List<LODImage> m_lodImages = new List<LODImage>();
        private string m_strName = "";
        private Image m_img = null;
        private string m_strURL = "";
        private int m_nID = -1;

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public PointF Position
        {
            get { return m_ptPosition; }
            set { m_ptPosition = value; }
        }

        public string ImagePath
        {
            get { return m_strImagePath; }
            set { m_strImagePath = value; }
        }

        /*
        public bool UseLODImage
        {
            get { return m_useLODImage; }
            set { m_useLODImage = value; }
        }
        */

        /*
        public List<LODImage> LODImages
        {
            get { return m_lodImages; }
        }
        */

        public Image Image
        {
            get { return m_img; }
            set { m_img = value; }
        }

        public string URL
        {
            get { return m_strURL; }
            set { m_strURL = value; }
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public override string ToString()
        {
            return m_strName;
        }

        public void Draw(Graphics g)
        {
            Image img = m_img == null ? DefaultImage : m_img;
            g.DrawImage(img, m_ptPosition);
        }

        public bool HitTest(float x, float y)
        {
            Image img = m_img == null ? DefaultImage : m_img;

            if (x >= m_ptPosition.X && x <= m_ptPosition.X + img.Width &&
                y >= m_ptPosition.Y && y <= m_ptPosition.Y + img.Height)
                return true;

            return false;
        }
    }
}
