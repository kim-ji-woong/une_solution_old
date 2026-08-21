using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace FireManagement
{
    public class Layer
    {

        private ArrayList m_arShapes = new ArrayList();

        public ArrayList Shapes
        {
            get { return m_arShapes; }
        }
        private ImageViewCtrl m_Owner = null;
        public ImageViewCtrl Owner
        {
            get { return m_Owner; }
        }

        public Layer(ImageViewCtrl ctrl)
        {
            m_Owner = ctrl;
        }

        private string m_szLayerName = "";
        public string LayerName
        {
            get { return m_szLayerName; }
            set { m_szLayerName = value; }
        }

        public void Add(Shape shape)
        {
            shape.Owner = this;
            m_arShapes.Add(shape);
        }

        public void Remove(Shape shape)
        {
            shape.Owner = null; ;
            m_arShapes.Remove(shape);
        }

        private bool m_bHidden = false;
        public bool Hidden
        {
            get { return m_bHidden; }
            set { m_bHidden = value; }
        }

        public void RemoveAll()
        {
            m_arShapes.Clear();
        }

        private bool m_bFrozen = false;
        public bool Frozen
        {
            get { return m_bFrozen; }
            set { m_bFrozen = value; }
        }

        private System.Drawing.Color mLineColor;
        public System.Drawing.Color LineColor
        {
            get { return mLineColor; }
            set { mLineColor = value; }
        }

        public virtual void Draw(System.Drawing.Graphics g)
        {
            if (m_bHidden == false)
            {
                foreach (Shape shape in m_arShapes)
                {
                    shape.Draw(g);
                }
            }
            
        }

        public Shape PickShape(float x, float y)
        {
            foreach (Shape shape in m_arShapes)
            {
                if (shape.IsPick(x, y))
                    return shape;
            }
            return null;
        }
    }

    public class Shape
    {
        protected Layer m_Owner = null;
        public Layer Owner
        {
            get { return m_Owner; }
            set { m_Owner = value; }
        }

        protected Size m_Size = new Size(20, 20);

        public Size Size
        {
            get { return m_Size; }
            set { m_Size = value; }
        }

        public bool IsPick(float x, float y)
        {
            RectangleF rect = new RectangleF((float)m_Pos.x, (float)m_Pos.y, 20.0f, 20.0f);
            return rect.Contains(x, y);
        }

        public Layer GetLayer()
        {
            return m_Owner;
        }
        protected int m_nID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private string m_szEquipID = "";
        public string EquipID
        {
            get { return m_szEquipID; }
            set { m_szEquipID = value; }
        }


        protected bool m_bSelected = false;

        public bool Selected
        {
            get { return m_bSelected; }
            set { m_bSelected = value; }
        }

        protected object m_tag = null;
        public object Tag
        {
            get { return m_tag; }
            set { m_tag = value; }
        }

        protected UnE.Geometry.Vertex2D m_Pos = new UnE.Geometry.Vertex2D();
        public UnE.Geometry.Vertex2D Position
        {
            get { return m_Pos; }
            set { m_Pos = value; }
        }
        public void SetPosition(UnE.Geometry.Vertex2D vCenter)
        {
            m_Pos = vCenter;
        }

        public void Move(UnE.Geometry.Vertex2D vCenter)
        {
            m_Pos = vCenter;
        }

        protected System.Drawing.Font m_Font = null;
        public System.Drawing.Font Font
        {
            get { return m_Font; }
            set { m_Font = value; }
        }

        protected System.Drawing.Color mLineColor;
        public System.Drawing.Color Color
        {
            get { return mLineColor; }
            set { mLineColor = value; }
        }

        public void Move(double x, double y)
        {
            m_Pos.x += x;
            m_Pos.y += y;
        }

        public virtual Shape Clone()
        {
            Shape s = new Shape();
            s.Font = this.Font;
            s.Tag = this.Tag;
            s.m_bSelected = this.m_bSelected;
            s.m_Pos = new UnE.Geometry.Vertex2D(m_Pos.x, m_Pos.y);
            s.mLineColor = this.mLineColor;
            return s;
        }
        public static Font idFont = new Font("맑은 고딕", 30.0f, FontStyle.Bold);
        protected System.Drawing.SolidBrush mBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
        public virtual void Draw(System.Drawing.Graphics g)
        {
            mBrush.Color = this.Color;
            //g.DrawString(m_szEquipID, idFont, mBrush, new PointF((float)m_Pos.x, (float)(m_Pos.y - 50.0f)));
            g.FillRectangle(mBrush, (float)m_Pos.x, (float)m_Pos.y, m_Size.Width , m_Size.Height);
        }
    }

    public class Text : Shape
    {
        private string m_szTitle = "";

        public string Title
        {
            get { return m_szTitle; }
            set { m_szTitle = value; }
        }

        System.Drawing.SolidBrush pen = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
        public virtual void Draw(System.Drawing.Graphics g)
        {
            pen.Color = this.Color;
            g.DrawString(m_szTitle, m_Font, mBrush, new System.Drawing.PointF((float)m_Pos.x, (float)m_Pos.y));
        }
    }

    public class ShapeGroup : Shape
    {
        private List<Shape> m_arShapes = new List<Shape>();


        public int GetShapeCount()
        {
            return m_arShapes.Count;
        }

        public Shape GetShape(int nIdx)
        {
            return m_arShapes[nIdx];
        }

        public virtual void Draw(System.Drawing.Graphics g)
        {
            //pen.Color = this.Color;
            //g.FillRectangle(pen, (float)m_Pos.x, (float)m_Pos.y, 10.0f, 10.0f);
        }
    }

    public class ShapeGroupOption
    {
        public ShapeGroupOption(System.Drawing.Image grp, int a, int n)
        {

        }
    }
}
