using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Direct2D1;

namespace DXFViewer
{
    public class Hatch : DXFDotNet.Hatch, IDrawableShape
    {
        protected EditBox m_editBox = null;
        protected SharpDX.Direct2D1.PathGeometry mPolygon = null;

        public Hatch() : base()
        {
        }
        
        public override DXFDotNet.Hatch CreateHatch()
        {
            Hatch arc = new Hatch();
            return arc;
        }

        public DXFDotNet.Shape GetShapeObject()
        {
            return this;
        }
        
        public bool CreateDXResource()
        {
            if (m_pOwnLayer == null || m_pOwnLayer.Owner == null)
                return false;

            if (m_arrPoint == null)
                return false;

            DXFViewer.DXFControl ctrl = (DXFViewer.DXFControl)m_pOwnLayer.Owner;
            SharpDX.Direct2D1.RenderTarget g = ctrl.RenderTarget;

            m_editBox = new EditBox(ctrl);
            m_editBox.CreateDXResource();
            
            mPolygon = new SharpDX.Direct2D1.PathGeometry(g.Factory);
            GeometrySink sink = mPolygon.Open();
            for (int i = 0; i < m_arrPoint.Length; i++)
            {
                SharpDX.Vector2 vec1 = new SharpDX.Vector2();

                vec1.X = m_arrPoint[i].X;
                vec1.Y = m_arrPoint[i].Y;
                if (i == 0)
                {
                    sink.BeginFigure(vec1, FigureBegin.Filled);                    
                }
                else
                {
                    sink.AddLine(vec1);                   
                }
            }           
            sink.EndFigure(FigureEnd.Closed);
            
            sink.Close();
            sink.Dispose();            

            return true;
        }

        public bool DiscardDXResource()
        {
            if (m_editBox != null)
            {
                m_editBox.DiscardDXResource();
                m_editBox = null;
            }

            if (mPolygon != null)
            {
                mPolygon.Dispose();
                mPolygon = null;
            }
            return true;
        }

        public override void SetVertex(System.Collections.ArrayList arrVertices)
        {
            base.SetVertex(arrVertices);
            
            DiscardDXResource();
            CreateDXResource();
        }
        public override bool UpdatePoint(bool bRefresh)
        {
            DiscardDXResource();
            CreateDXResource();
            return true;
        }
        
		public override bool UpdatePoint(int nIndex, float x, float y)
        {
            bool bResult = base.UpdatePoint(nIndex, x, y);
            return bResult;
        }

        public bool Draw(SharpDX.Direct2D1.RenderTarget g, bool bDrawText)
        {
            if (m_arrPoint == null)
                return true;

            m_brush.Color = GetColor();
            
            System.Drawing.Color orColor = m_brush.Color;
            SharpDX.Color color = new SharpDX.Color(orColor.R, orColor.G, orColor.B, orColor.A);
            using (SharpDX.Direct2D1.SolidColorBrush solidColorBrush = new SharpDX.Direct2D1.SolidColorBrush(g, color))
            {
                g.FillGeometry(mPolygon, solidColorBrush);

                if (Selectable && Selected)
                {
                    if (m_selectedShowingType == SelectedShowingType.EDIT_BOX)
                        m_editBox.Draw(g, m_ptCenter.X, m_ptCenter.Y);
                    else if (m_selectedShowingType == SelectedShowingType.BRIGHT_EFFECT ||
                        m_selectedShowingType == SelectedShowingType.DRAW_POLYGON)
                    {
                        System.Drawing.Color oldColor = System.Drawing.Color.FromArgb(100, 255 - orColor.R, 255 - orColor.G, 255 - orColor.B);
                        SharpDX.Color newColor = new SharpDX.Color(orColor.R, orColor.G, orColor.B, orColor.A);
                        solidColorBrush.Color = newColor;
                        g.FillGeometry(mPolygon, solidColorBrush);
                        
                    }
                }
            }
            return true;
        }

        public override bool CheckClipBounds(UnE.Geometry.Vertex2D vClipTL, UnE.Geometry.Vertex2D vClipBR)
        {
            return true;
        }

       
    }
}
