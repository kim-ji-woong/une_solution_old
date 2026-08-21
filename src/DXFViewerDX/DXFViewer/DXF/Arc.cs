using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Direct2D1;

namespace DXFViewer
{
    public class Arc : DXFDotNet.Arc, IDrawableShape
    {
        protected EditBox m_editBox = null;

        protected SharpDX.Direct2D1.PathGeometry mArc = null;
        protected SharpDX.Direct2D1.EllipseGeometry mEllipseGeo = null;
        protected SharpDX.Vector2 vCenter = new SharpDX.Vector2();

        public Arc() : base()
        {
        }

        public override DXFDotNet.Arc CreateArc()
        {
            Arc arc = new Arc();
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

            float a1 = (float)m_dBeginAngle;
            float a2 = a1 + (float)m_dArcAngle;
            
            float radius = (float)m_dRadius;
            vCenter = new SharpDX.Vector2((float)m_vCenter.x, (float)m_vCenter.y);

            if (m_isCircle)
            {
                SharpDX.Direct2D1.Ellipse ellipse = new SharpDX.Direct2D1.Ellipse(vCenter, radius, radius);
                mEllipseGeo = new SharpDX.Direct2D1.EllipseGeometry(g.Factory, ellipse);
            }
            else
            {
                SharpDX.Vector2 ptStart = EllipsticCoordinate(vCenter.X, vCenter.Y, radius, radius, a1);
                SharpDX.Vector2 ptEnd = EllipsticCoordinate(vCenter.X, vCenter.Y, radius, radius, a2);

                mArc = new SharpDX.Direct2D1.PathGeometry(g.Factory);
                GeometrySink sink2 = mArc.Open();
                sink2.BeginFigure(ptStart, FigureBegin.Hollow);

                ArcSegment seg = new ArcSegment();
                seg.SweepDirection = SweepDirection.Clockwise;
                seg.Size = new Size2F((float)radius, (float)radius);
                seg.ArcSize = ((Math.Abs(m_dArcAngle) > 180.0f) ? ArcSize.Large : ArcSize.Small);
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
            if(m_editBox != null)
            {
                m_editBox.DiscardDXResource();
                m_editBox = null;
            }

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

            vCenter.X = (float)m_vCenter.x;
            vCenter.Y = (float)m_vCenter.y;

            g.Transform = arcMatrix;


            if (Selectable && Selected && m_selectedShowingType != SelectedShowingType.NONE)
            {
                if (m_selectedShowingType == SelectedShowingType.EDIT_BOX)
                {
                    DrawArc(g, pen);
                    m_editBox.Draw(g, vCenter.X, vCenter.Y);
                }
                else if (m_selectedShowingType == SelectedShowingType.BRIGHT_EFFECT)
                {
                    float fOldWidth = pen.Width;

                    pen.Width += 1;

                    DrawArc(g, pen);

                    pen.Width = fOldWidth;

                    // 밝게 표현하기 위하여 배경색의 보색으로 그린다.
                    DrawArc(g, ctrl.SelectedBrightPen1);
                    // 패턴을 주기 위하여 배경색으로 다시한번 그린다.
                    DrawArc(g, ctrl.SelectedBrightPen2);
                }
            }
            else
            {
                DrawArc(g, pen);
            }

            g.Transform = orgMatrix;
            return true;
        }

        protected void DrawArc(SharpDX.Direct2D1.RenderTarget g, System.Drawing.Pen pen)
        {
            float fLineWidth = SetScalePenWidth(pen, g);            
            System.Drawing.Color orColor = pen.Color;
            SharpDX.Color color = new SharpDX.Color(orColor.R, orColor.G, orColor.B, orColor.A);
            using(SharpDX.Direct2D1.SolidColorBrush solidColorBrush = new SharpDX.Direct2D1.SolidColorBrush(g, color))
            {
	            if (m_isCircle)
                {
                    g.DrawGeometry(mEllipseGeo, solidColorBrush, pen.Width);               
                }
	            else
                {
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
        
		public override bool CheckClipBounds(UnE.Geometry.Vertex2D vClipTL,UnE.Geometry.Vertex2D vClipBR )
        {
            return true;
        }
    }
}
