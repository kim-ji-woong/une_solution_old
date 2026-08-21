using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MashupImage
{
    public partial class ImagePanel : UserControl
    {
        private class ImageData
        {
            private Image m_img = null;
            private Point m_ptPosition = new Point();
            private Size m_size = new Size();

            public Image Image
            {
                get { return m_img; }
                set { m_img = value; }
            }

            public Point Position
            {
                get { return m_ptPosition; }
                set { m_ptPosition = value; }
            }

            public Size Size
            {
                get { return m_size; }
                set { m_size = value; }
            }
        }

        private List<ImageData> m_images = new List<ImageData>();
        private int m_nHCount = 0;
        private int m_nVCount = 0;

        private List<Shape> m_shapes = new List<Shape>();
        private IShapeOwner m_owner = null;

        #region 화면이동
        private Point m_ptMClicked = new Point();
        private Point m_ptPrev;
        private Point m_ptCurrent;
        private PointF m_ptOrigin = new PointF();
        private bool m_bTranslation = false;

        private float m_fTranX = 0;
        private float m_fTranY = 0;
        #endregion

        #region Zoom
        private float m_fPrevScale = 1.0f;
        private float m_fCurScale = 1.0f;
        #endregion

        #region Pick & Drag
        private PointF m_ptLClicked = new PointF();
        private Shape m_selectedShape = null;
        private PointF m_ptShapeOrigin = new PointF();
        #endregion

        public IShapeOwner Owner
        {
            get { return m_owner; }
            set { m_owner = value; }
        }

        public ImagePanel()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.MouseWheel += ImagePanel_MouseWheel;
        }

        private void ImagePanelcs_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(m_fTranX, m_fTranY);
            e.Graphics.ScaleTransform(m_fCurScale, m_fCurScale);

            foreach (ImageData data in m_images)
            {
                e.Graphics.DrawImage(data.Image, new Rectangle(data.Position, data.Size), new Rectangle(0, 0, data.Size.Width, data.Size.Height), GraphicsUnit.Pixel);
            }

            foreach (Shape shape in m_shapes)
            {
                shape.Draw(e.Graphics);
            }

            e.Graphics.ResetTransform();
        }

        public void SetLOD(LOD lod, bool refresh = true)
        {
            m_images.Clear();

            if (lod == null || Directory.Exists(lod.FolderName) == false)
            {
                if (refresh)
                    Refresh();

                return;
            }

            string[] files = Directory.GetFiles(lod.FolderName);
            int nFileCount = files.Count();

            if (nFileCount != lod.ImageHCount * lod.ImageVCount)
            {
                if (refresh)
                    Refresh();

                MessageBox.Show(string.Format("이미지 개수가 {0} * {1}이 되어야 합니다.", lod.ImageHCount, lod.ImageVCount));
                return;
            }

            for (int i=0;i< lod.ImageVCount; i++)
            {
                int y = i == 0 ? 0 : lod.ImageHeight + (lod.ImageHeight - (int)lod.AddPixel) * (i - 1) - (int)lod.AddPixel;

                for (int j = 0; j < lod.ImageHCount; j++)
                {
                    string strFile = files[i * lod.ImageHCount + j];
                    Image img = Image.FromFile(strFile);
                    ImageData data = new ImageData();

                    data.Image = img;

                    int x = j == 0 ? 0 : lod.ImageWidth + (lod.ImageWidth - (int)lod.AddPixel) * (j - 1) - (int)lod.AddPixel;

                    data.Position = new Point(x, y);
                    //data.Position = new Point(j * lod.ImageWidth, i * lod.ImageHeight);

                    if (j < lod.ImageHCount - 1 && i < lod.ImageVCount - 1)
                        data.Size = new Size(lod.ImageWidth, lod.ImageHeight);
                    else if (j == lod.ImageHCount - 1)
                        data.Size = new Size(img.Width, lod.ImageHeight);
                    else if (i == lod.ImageHCount - 1)
                        data.Size = new Size(lod.ImageWidth, img.Height);
                    else
                        data.Size = new Size(img.Width, img.Height);

                    m_images.Add(data);
                }
            }

            m_nHCount = lod.ImageHCount;
            m_nVCount = lod.ImageVCount;

            if (refresh)
                Refresh();
        }

        private void ImagePanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                m_ptMClicked.X = e.X;
                m_ptMClicked.Y = e.Y;

                m_bTranslation = true;
                m_ptPrev.X = e.X;
                m_ptPrev.Y = e.Y;

                m_ptCurrent = e.Location;
            }
            else if (e.Button == MouseButtons.Left)
            {
                float x, y;
                ScreenToGlobal(e.X, e.Y, out x, out y);
                m_selectedShape = SelectShape(x, y);

                if (m_owner != null)
                    m_owner.OnSelectShape(m_selectedShape);

                if (m_selectedShape != null)
                {
                    m_ptLClicked = new PointF(x, y);
                    m_ptShapeOrigin = m_selectedShape.Position;
                }
            }
        }

        private Shape SelectShape(float x, float y)
        {
            foreach (Shape shape in m_shapes)
            {
                if (shape.HitTest(x, y))
                    return shape;
            }

            return null;
        }

        private void ImagePanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                if (m_bTranslation == true)
                {
                    m_bTranslation = false;
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                m_selectedShape = null;
            }
        }

        private void ImagePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                if (m_bTranslation == true)
                {
                    m_ptCurrent.X = e.X;
                    m_ptCurrent.Y = e.Y;

                    Translate(m_ptPrev.X, m_ptPrev.Y, e.X, e.Y);

                    m_ptPrev = m_ptCurrent;
                    Invalidate();
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                if (m_selectedShape != null)
                {
                    float x, y;
                    ScreenToGlobal(e.X, e.Y, out x, out y);

                    float moveX = x - m_ptLClicked.X;
                    float moveY = y - m_ptLClicked.Y;

                    m_selectedShape.Position = new PointF(m_ptShapeOrigin.X + moveX, m_ptShapeOrigin.Y + moveY);

                    if (m_owner != null)
                        m_owner.OnMoveShape(m_selectedShape, m_ptShapeOrigin.X + moveX, m_ptShapeOrigin.Y + moveY);

                    Invalidate();
                }
            }
        }

        private void ImagePanel_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                ZoomIn(e.X, e.Y);
            else
                ZoomOut(e.X, e.Y);
        }

        private void ZoomIn(int x, int y)
        {
            if (m_fCurScale <= 10.0f)
            {
                Point pt = new Point(x, y);
                PointF pt1 = ScreenToGlobal(pt);

                m_fCurScale = m_fCurScale * 1.1f;

                PointF pt2 = ScreenToGlobal(pt);

                float dx = (pt2.X - pt1.X) * m_fCurScale;
                float dy = (pt2.Y - pt1.Y) * m_fCurScale;

                m_ptOrigin.X += dx;
                m_ptOrigin.Y += dy;

                m_fTranX += dx;
                m_fTranY += dy;

                m_fPrevScale = m_fCurScale;

                Refresh();
            }
        }


        private void ZoomOut(int x, int y)
        {
            if (m_fCurScale > 0.01f)
            {
                Point pt = new Point(x, y);
                PointF pt1 = ScreenToGlobal(pt);

                m_fCurScale = m_fCurScale / 1.1f;

                PointF pt2 = ScreenToGlobal(pt);

                float dx = (pt2.X - pt1.X) * m_fCurScale;
                float dy = (pt2.Y - pt1.Y) * m_fCurScale;

                m_ptOrigin.X += dx;
                m_ptOrigin.Y += dy;

                m_fTranX += dx;
                m_fTranY += dy;

                m_fPrevScale = m_fCurScale;

                Refresh();
            }
        }

        private void Translate(int prevX, int prevY, int x, int y)
        {
            m_ptOrigin.X += (x - prevX);
            m_ptOrigin.Y += (y - prevY);

            m_fTranX = m_ptOrigin.X;
            m_fTranY = m_ptOrigin.Y;
        }

        public PointF ScreenToGlobal(Point pt)
        {
            float dx = ((m_ptOrigin.X) + (m_ptCurrent.X - m_ptPrev.X)) / m_fCurScale;
            float dy = ((m_ptOrigin.Y) + (m_ptCurrent.Y - m_ptPrev.Y)) / m_fCurScale;

            float gx = (pt.X / m_fCurScale - dx);
            float gy = (pt.Y / m_fCurScale - dy);

            return new PointF(gx, gy);
        }

        public void ScreenToGlobal(int x, int y, out float gx, out float gy)
        {
            float dx = ((m_ptOrigin.X) + (m_ptCurrent.X - m_ptPrev.X)) / m_fCurScale;
            float dy = ((m_ptOrigin.Y) + (m_ptCurrent.Y - m_ptPrev.Y)) / m_fCurScale;

            gx = (x / m_fCurScale - dx);
            gy = (y / m_fCurScale - dy);
        }

        public void AddShape(Shape shape)
        {
            m_shapes.Add(shape);
        }

        public void ClearShapes()
        {
            m_shapes.Clear();
            m_selectedShape = null;
        }

        public void RemoveShape(Shape shape)
        {
            m_shapes.Remove(shape);

            if (m_selectedShape == shape)
                m_selectedShape = null;
        }
    }

    public interface IShapeOwner
    {
        void OnSelectShape(Shape shape);
        void OnMoveShape(Shape shape, float x, float y);
    }
}
