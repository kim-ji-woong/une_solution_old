using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MashupImage
{
    public class Project
    {
        private List<LOD> m_lods = new List<LOD>();
        private List<Shape> m_shapes = new List<Shape>();
        private string m_strProjectPath = "";

        public List<LOD> LODs
        {
            get { return m_lods; }
        }

        public List<Shape> Shapes
        {
            get { return m_shapes; }
        }

        public string ProjectPath
        {
            get { return m_strProjectPath; }
            set { m_strProjectPath = value; }
        }

        public bool Save(out string strErrorMessage)
        {
            XMLManager mgr = new XMLManager();

            bool result = mgr.Save(this);
            strErrorMessage = mgr.ErrorMessage;
            return result;
        }
    }

    public class LOD : IComparable
    {
        private string m_strID = "";
        private string m_strName = "";
        private string m_strFolderName = "";
        private int m_nImageWidth = 0;
        private int m_nImageHeight = 0;
        private int m_nImageHCount = 0;
        private int m_nImageVCount = 0;
        private int m_nImageTotalWidth = 0;
        private int m_nImageTotalHeight = 0;
        private Ratio m_ratio = new Ratio();
        private uint m_nAddPixel = 0;

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public string FolderName
        {
            get { return m_strFolderName; }
            set { m_strFolderName = value; }
        }

        public int ImageWidth
        {
            get { return m_nImageWidth; }
            set { m_nImageWidth = value; }
        }

        public int ImageHeight
        {
            get { return m_nImageHeight; }
            set { m_nImageHeight = value; }
        }

        public int ImageTotalWidth
        {
            get { return m_nImageTotalWidth; }
            set { m_nImageTotalWidth = value; }
        }

        public int ImageTotalHeight
        {
            get { return m_nImageTotalHeight; }
            set { m_nImageTotalHeight = value; }
        }

        public int ImageHCount
        {
            get { return m_nImageHCount; }
            set { m_nImageHCount = value; }
        }

        public int ImageVCount
        {
            get { return m_nImageVCount; }
            set { m_nImageVCount = value; }
        }

        public Ratio Ratio
        {
            get { return m_ratio; }
            set { m_ratio = value; }
        }

        // 이미지와 이미지 사이에 줄이 생기는것을 방지하기 위하여 추가로 더 그리는 Pixel
        public uint AddPixel
        {
            get { return m_nAddPixel; }
            set { m_nAddPixel = value; }
        }

        public void SetIndex(int nIndex)
        {
            m_strID = string.Format("l{0}", nIndex + 1);
            m_strName = string.Format("LOD{0}", nIndex + 1);
        }

        public override string ToString()
        {
            return m_strName;
        }

        public int CompareTo(object obj)
        {
            LOD lod = (LOD)obj;
            return this.m_nImageTotalWidth.CompareTo(lod.m_nImageTotalWidth);
        }
    }

    public class Ratio
    {
        private bool m_usePercent = true;
        private double m_dHPercent = 100.0;
        private double m_dVPercent = 100.0;
        private LOD m_lodBase = null;
        private int m_nBaseWidth = 0;
        private int m_nBaseHeight = 0;
        private int m_nCurrentWidth = 0;
        private int m_nCurrentHeight = 0;

        public bool UsePercent
        {
            get { return m_usePercent; }
            set { m_usePercent = value; }
        }

        public double HPercent
        {
            get { return m_dHPercent; }
            set { m_dHPercent = value; }
        }

        public double VPercent
        {
            get { return m_dVPercent; }
            set { m_dVPercent = value; }
        }

        public int BaseWidth
        {
            get { return m_nBaseWidth; }
            set { m_nBaseWidth = value; }
        }

        public int BaseHeight
        {
            get { return m_nBaseHeight; }
            set { m_nBaseHeight = value; }
        }

        public int CurrentWidth
        {
            get { return m_nCurrentWidth; }
            set { m_nCurrentWidth = value; }
        }

        public int CurrentHeight
        {
            get { return m_nCurrentHeight; }
            set { m_nCurrentHeight = value; }
        }

        public LOD BaseLOD
        {
            get { return m_lodBase; }
            set { m_lodBase = value; }
        }
    }

    public class Shape
    {
        private static Image DefaultImage = global::MashupImage.Properties.Resources._default;

        private string m_strImagePath = "";
        private PointF m_ptPosition = new PointF();
        private bool m_useLODImage = false;
        private List<LODImage> m_lodImages = new List<LODImage>();
        private string m_strName = "";
        private Image m_img = null;

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

        public bool UseLODImage
        {
            get { return m_useLODImage; }
            set { m_useLODImage = value; }
        }

        public List<LODImage> LODImages
        {
            get { return m_lodImages; }
        }

        public Image Image
        {
            get { return m_img; }
            set { m_img = value; }
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

    public class LODImage
    {
        private LOD m_lod = null;
        private PointF m_ptPosition = new PointF();
        private string m_strImagePath = "";

        public LOD LOD
        {
            get { return m_lod; }
            set { m_lod = value; }
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
    }
}
