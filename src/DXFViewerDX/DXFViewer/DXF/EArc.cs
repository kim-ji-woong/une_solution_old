using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Direct2D1;

namespace DXFViewer
{
    public class EArc : DXFDotNet.EArc, IDrawableShape
    {
        protected EditBox m_editBox = null;

        protected SharpDX.Direct2D1.PathGeometry mArc = null;
        protected SharpDX.Direct2D1.EllipseGeometry mEllipseGeo = null;
        protected SharpDX.Vector2 vTL = new SharpDX.Vector2();
        protected SharpDX.Vector2 vCenter = new SharpDX.Vector2();

        public EArc() : base()
        {
        }

        public override DXFDotNet.EArc CreateEArc()
        {
            EArc arc = new EArc();
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

            DXFViewer.DXFControl ctrl = (DXFViewer.DXFControl)m_pOwnLayer.Owner;
            SharpDX.Direct2D1.RenderTarget g = ctrl.RenderTarget;

            m_editBox = new EditBox(ctrl);
            m_editBox.CreateDXResource();
              
            float radiusX = (float)m_dWidth / 2.0f;
            float radiusY = (float)m_dHeight / 2.0f;

            float a1 = (float)m_dBeginAngle;
            float a2 = a1 + (float)m_dEArcAngle;
            
            if (m_isEllipse)
            {
                SharpDX.Direct2D1.Ellipse ellipse = new SharpDX.Direct2D1.Ellipse(vCenter, radiusX, radiusY);
                mEllipseGeo = new SharpDX.Direct2D1.EllipseGeometry(g.Factory, ellipse);
            }
            else
            {
                SharpDX.Vector2 ptStart = EllipsticCoordinate(vCenter.X, vCenter.Y, radiusX, radiusY, a1);
                SharpDX.Vector2 ptEnd = EllipsticCoordinate(vCenter.X, vCenter.Y, radiusX, radiusY, a2);

                mArc = new SharpDX.Direct2D1.PathGeometry(g.Factory);
                GeometrySink sink2 = mArc.Open();
                sink2.BeginFigure(ptStart, FigureBegin.Hollow);

                ArcSegment seg = new ArcSegment();
                seg.SweepDirection = SweepDirection.Clockwise;
                seg.Size = new Size2F((float)radiusX, (float)radiusY);
                seg.ArcSize = ((Math.Abs(m_dEArcAngle) > 180.0f) ? ArcSize.Large : ArcSize.Small);
                seg.RotationAngle = (float)0.0;
                seg.Point = ptEnd;

                sink2.AddArc(seg);
                sink2.EndFigure(FigureEnd.Open);

                sink2.Close();
                sink2 = null;
            }  
            return true;
        }

        public bool DiscardDXResource()
        {
            m_editBox.DiscardDXResource();
            m_editBox = null;

            if (mArc != null)
            {
                mArc.Dispose();
                mArc = null;
            }

            if (mEllipseGeo != null)
            {
                mEllipseGeo.Dispose();
                mEllipseGeo = null;
            }
            return true;
        }
        
        public bool Draw(SharpDX.Direct2D1.RenderTarget g, bool bDrawText)
        {
            if (m_vTL == null)
		        return false;

            if (m_pOwnLayer == null || m_pOwnLayer.Owner == null)
                return false;

            DXFViewer.DXFControl ctrl = (DXFViewer.DXFControl)m_pOwnLayer.Owner;

	        System.Drawing.Pen pen = m_lineType.GetPen();
	        pen.Color = GetColor();
            
            System.Drawing.Color orColor = GetColor();
            SharpDX.Color color = new SharpDX.Color(orColor.R, orColor.G, orColor.B, orColor.A);

            SharpDX.Matrix3x2 orgMatrix = g.Transform;
            SharpDX.Matrix3x2 arcMatrix = SharpDX.Matrix3x2.Identity;
            arcMatrix.M11 = g.Transform.M11;
            arcMatrix.M12 = g.Transform.M12;
            arcMatrix.M21 = g.Transform.M21;
            arcMatrix.M22 = g.Transform.M22;
            arcMatrix.M31 = g.Transform.M31;
            arcMatrix.M32 = g.Transform.M32;

	        vTL.X = (float)m_vTL.x;
            vTL.Y = (float)m_vTL.y;
            vCenter = new SharpDX.Vector2((float)(vTL.X + m_dWidth / 2), (float)(vTL.Y - m_dHeight / 2));

            if (m_dXAxisAngle != 0.0)
	        {
                float angle = (float)m_dXAxisAngle;
                float rad = SharpDX.MathUtil.DegreesToRadians((float)angle);
                arcMatrix = (SharpDX.Matrix3x2.Rotation(rad, vTL) * arcMatrix);
	        }

            if (ctrl.DownToTop())
	        {
                arcMatrix = (SharpDX.Matrix3x2.Scaling(1.0f, -1.0f) * arcMatrix);               
                vTL.Y = - (float)m_vTL.y;
                vCenter.Y = -vCenter.Y;
	        }

            g.Transform = arcMatrix;
            if (Selectable && Selected && m_selectedShowingType != SelectedShowingType.NONE)
            {
                if (m_selectedShowingType == SelectedShowingType.EDIT_BOX)
                {
                    DrawEArc(g, pen);
                    m_editBox.Draw(g, (float)(vTL.X + m_dWidth / 2), (float)(vTL.Y - m_dHeight / 2));
                }
                else if (m_selectedShowingType == SelectedShowingType.BRIGHT_EFFECT)
                {
                    float fOldWidth = pen.Width;

                    pen.Width += 1;
                    
                    DrawEArc(g, pen);
                    
                    pen.Width = fOldWidth;

                    // 밝게 표현하기 위하여 배경색의 보색으로 그린다.
                    DrawEArc(g, ctrl.SelectedBrightPen1);
                    // 패턴을 주기 위하여 배경색으로 다시한번 그린다.
                    DrawEArc(g, ctrl.SelectedBrightPen2);
                }
            }
            else
            {
                DrawEArc(g, pen);
            }

            vTL.X = (float)m_vTL.x;
            vTL.Y = (float)m_vTL.y;
            g.Transform = orgMatrix;
            return true;
        }       

        protected void DrawEArc(SharpDX.Direct2D1.RenderTarget g, System.Drawing.Pen pen)
        {
            float fLineWidth = SetScalePenWidth(pen, g);            
            System.Drawing.Color orColor = pen.Color;
            SharpDX.Color color = new SharpDX.Color(orColor.R, orColor.G, orColor.B, orColor.A);
            using(SharpDX.Direct2D1.SolidColorBrush solidColorBrush = new SharpDX.Direct2D1.SolidColorBrush(g, color))
            {
	            if (m_isEllipse)
                {
                    //SharpDX.Color4 c = solidColorBrush.Color;
                    //solidColorBrush.Color = SharpDX.Color.Aqua;
                    g.DrawGeometry(mEllipseGeo, solidColorBrush, pen.Width);
                    //solidColorBrush.Color = c;                   
                }
	            else
                {
                    //float radiusX = (float)m_dWidth / 2.0f;
                    //float radiusY = (float)m_dHeight / 2.0f;
                    //float a1 = (float)m_dBeginAngle;
                    //float a2 = a1 + (float)m_dEArcAngle;
                    //SharpDX.Vector2 ptStart = EllipsticCoordinate(vCenter.X, vCenter.Y, radiusX, radiusY, a1);
                    //SharpDX.Vector2 ptEnd = EllipsticCoordinate(vCenter.X, vCenter.Y, radiusX, radiusY, a2);
                    
                    // EARC - BoundingBox Check for Debugging
                    //SharpDX.Color4 c = solidColorBrush.Color;
                    //solidColorBrush.Color = SharpDX.Color.Aqua;
                    //SharpDX.Vector2 pt0 = EllipsticCoordinate(vCenter.X, vCenter.Y, radiusX, radiusY, 0.0f);
                    //SharpDX.Vector2 pt1 = EllipsticCoordinate(vCenter.X, vCenter.Y, radiusX, radiusY, 90.0f);
                    //g.DrawRectangle(new RectangleF(pt0.X, pt0.Y, 2.0f, 2.0f), solidColorBrush, pen.Width);
                    //g.DrawRectangle(new RectangleF(pt1.X, pt1.Y, 2.0f, 2.0f), solidColorBrush, pen.Width);
                    //SharpDX.Direct2D1.Ellipse ellipse = new SharpDX.Direct2D1.Ellipse(vCenter, radiusX, radiusY);
                    //SharpDX.Direct2D1.EllipseGeometry mEllipseGeo = new SharpDX.Direct2D1.EllipseGeometry(g.Factory, ellipse);
                    //g.DrawGeometry(mEllipseGeo, solidColorBrush, pen.Width);
                    //solidColorBrush.Color = c;
                    //g.DrawRectangle(new RectangleF(ptStart.X, ptStart.Y, 2.0f, 2.0f), solidColorBrush, pen.Width);
                    //g.DrawRectangle(new RectangleF((float)(vCenter.X - radiusX), (vCenter.Y - radiusY), (float)m_dWidth, (float)m_dHeight), solidColorBrush, pen.Width);
                    
                    g.DrawGeometry(mArc, solidColorBrush, pen.Width);
                }	
            }
	        pen.Width = fLineWidth;
        }

        private SharpDX.Vector2 EllipsticCoordinate(float cx, float cy, float fXAxis, float fYAxis, float fAngle)
        {
            double dblAngleRad = SharpDX.MathUtil.DegreesToRadians(fAngle);
            cx += (float)(fXAxis * System.Math.Cos(dblAngleRad));
            cy += (float)(fYAxis * System.Math.Sin(dblAngleRad));            
            return new SharpDX.Vector2(cx,cy);
        }

        private float SetScalePenWidth(System.Drawing.Pen brush, SharpDX.Direct2D1.RenderTarget g)
        {
            float fOldWidth = brush.Width;

            float fScaleX = g.Transform.M11;
            float fScaleY = g.Transform.M22;

            float fLineWidth = 1.0f / fScaleX * fOldWidth;
            brush.Width = fLineWidth;

            return fOldWidth;
        }

        public override bool CheckClipBounds(UnE.Geometry.Vertex2D vClipTL, UnE.Geometry.Vertex2D vClipBR)
        {
            return true;
        }
    }
}
