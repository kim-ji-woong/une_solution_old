using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct2D1;

namespace DXFViewer
{
    public class PolyLine : DXFDotNet.PolyLine, IDrawableShape
    {
        protected EditBox m_editBox = null;

        private PathGeometry m_PathGeom = null;
        private PathGeometry m_FillPolygon = null;

        protected SolidColorBrush mLineBrush = null;
        protected SolidColorBrush mPolygonBrush = null;
        protected SolidColorBrush mWhiteBrush = null;

        private SharpDX.RectangleF mEffectRect1 = new SharpDX.RectangleF();
        private SharpDX.RectangleF mEffectRect2 = new SharpDX.RectangleF();

        public PolyLine() : base()
        {  
        }

        public override DXFDotNet.PolyLine CreatePolyLine()
        {
            PolyLine arc = new PolyLine();
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
            m_PathGeom = new PathGeometry(g.Factory);
            m_FillPolygon = new PathGeometry(g.Factory);

            GeometrySink sink2 = m_FillPolygon.Open();

            GeometrySink sink = m_PathGeom.Open();            
            for (int i = 0; i < m_arrPoint.Length; i++)
            {
                SharpDX.Vector2 vec1 = new SharpDX.Vector2();

                vec1.X = m_arrPoint[i].X;
                vec1.Y = m_arrPoint[i].Y;
                if (i == 0)
                {
                    sink.BeginFigure(vec1, FigureBegin.Hollow);
                    sink2.BeginFigure(vec1, FigureBegin.Hollow);
                }
                else
                {
                    sink.AddLine(vec1);
                    sink2.AddLine(vec1);
                }
            }
            sink.EndFigure(FigureEnd.Open);
            sink2.EndFigure(FigureEnd.Closed);
            
            sink.Close();
            sink2.Close();
            sink.Dispose();
            sink2.Dispose();


            System.Drawing.Color orColor = GetColor();
            SharpDX.Color color = new SharpDX.Color(orColor.R, orColor.G, orColor.B, orColor.A);
            mLineBrush = new SolidColorBrush(g, color);

            mPolygonBrush = new SolidColorBrush(g, color);

            mWhiteBrush = new SolidColorBrush(g, SharpDX.Color.White);

            m_editBox.CreateDXResource();

            return true;
        }
        
        public bool DiscardDXResource()
        {
            mWhiteBrush.Dispose();
            mWhiteBrush = null;

            mPolygonBrush.Dispose();
            mPolygonBrush = null;

            mLineBrush.Dispose();
            mLineBrush = null;

            m_PathGeom.Dispose();
            m_PathGeom = null;

            m_editBox.DiscardDXResource();

            return true;
        }

        public bool Draw(SharpDX.Direct2D1.RenderTarget g, bool bDrawText)
        {
            if (m_arrPoint == null)
                return false;

            return DrawDirect2D(g);            
        }

        protected bool DrawDirect2D(SharpDX.Direct2D1.RenderTarget g)
        {
	        System.Drawing.Pen pen = m_lineType.GetPen();
	        pen.Color = GetColor();
            
            DXFViewer.DXFControl control = (DXFViewer.DXFControl)m_pOwner;
	        if (Selectable && Selected && m_selectedShowingType != SelectedShowingType.NONE)
	        {
		        if (m_selectedShowingType == SelectedShowingType.EDIT_BOX)
		        {                    
			        DXFDotNet.LineType lineType = control.GetSelectedLineType();
			        System.Drawing.Color penColor = pen.Color;
			        pen = lineType.GetPen();
			        pen.Color = penColor;

			        DrawLines(pen, g);

			        m_editBox.Draw(g, (float)m_vTL.x, (float)m_vTL.y);
			        m_editBox.Draw(g, (float)m_vTL.x, (float)m_vBR.y);
			        m_editBox.Draw(g, (float)m_vBR.x, (float)m_vBR.y);
			        m_editBox.Draw(g, (float)m_vBR.x, (float)m_vTL.y);
		        }
		        else if (m_selectedShowingType == SelectedShowingType.BRIGHT_EFFECT)
		        {
			        float fOldWidth = pen.Width;

			        pen.Width += 1;
			        DrawLines(pen, g);

			        pen.Width = fOldWidth;

			        // 밝게 표현하기 위하여 배경색의 보색으로 그린다.
                    DrawLines(control.SelectedBrightPen1, g);
			        
                    // 패턴을 주기 위하여 배경색으로 다시한번 그린다.
                    DrawLines(control.SelectedBrightPen2, g);

                    if (!m_isClosed)
                    {
                        mEffectRect1.Left = m_vSelectedBegin.x;
                        mEffectRect1.Top = m_vSelectedBegin.y;
                        mEffectRect1.Width = 1.0f;
                        mEffectRect1.Height = 1.0f;

                        mEffectRect2.Left = m_vSelectedEnd.x;
                        mEffectRect2.Top = m_vSelectedEnd.y;
                        mEffectRect2.Width = 1.0f;
                        mEffectRect2.Height = 1.0f;


                        g.FillRectangle(mEffectRect1, mWhiteBrush);
                        g.FillRectangle(mEffectRect2, mWhiteBrush);
                    }
		        }
		        else if (m_selectedShowingType == SelectedShowingType.DRAW_POLYGON)
		        {
			        System.Drawing.Color complementaryColor = control.SelectedBrightPen1.Color;
			        System.Drawing.Color brushColor = System.Drawing.Color.FromArgb(100, complementaryColor.R, complementaryColor.G, complementaryColor.B);

                    mPolygonBrush.Color = new SharpDX.Color(brushColor.R, brushColor.G, brushColor.B, brushColor.A);
                    g.FillGeometry(m_FillPolygon, mPolygonBrush);
		        }
	        }
	        else	
		        DrawLines(pen, g);

	        return true;
        }

        void DrawLines(System.Drawing.Pen pen, SharpDX.Direct2D1.RenderTarget g)
        {
            // Get Color
            System.Drawing.Color orColor = pen.Color;
            // Get Line width
            float fWidth = SetScalePenWidth(pen, g);

            // Set brush color
            mLineBrush.Color = new SharpDX.Color(orColor.R, orColor.G, orColor.B, orColor.A);
            // Draw Lines
            g.DrawGeometry(m_PathGeom, mLineBrush, pen.Width);

            pen.Width = fWidth;
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
