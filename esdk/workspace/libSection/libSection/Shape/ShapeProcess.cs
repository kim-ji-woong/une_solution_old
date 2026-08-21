using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Sections
{
    // 아래위 두 개의 Shape을 가진다.
    public class ShapeProcess : Shape
    {
        private Shape m_shapeUp = null;
        private Shape m_shapeDown = null;
        private double m_dDownRatio = 0.4;
        private PointF[] m_arrDrawing = new PointF[4];

        private static Image m_circleImageAutoRun = null;
        private static float m_fCircleDiameter = 35;

        private ImagePainter m_painter = null;

        public ImagePainter ImagePainter
        {
            get { return m_painter; }
            set { m_painter = value; }
        }

        private static Pen MIDDLE_PEN = new Pen(Color.FromArgb(172, 172, 172), 1);
        private static Image imgOut = null;
        private static Image imgInNormal = null;
        private static Image imgInSkipped = null;
        private static Image imgInProcessing = null;
        private static Image imgInProcessed = null;
        private static Image imgInWaiting = null;
        private static Image imgSelect = null;

        protected static float radius = 30.0f;
        protected static float CornerRadius
        {
            get { return ShapeProcess.radius; }
            set { ShapeProcess.radius = value; }
        }

        protected static Color mMiddleLineColor = Color.FromArgb(172, 172, 172);
        protected static Color MiddleLineColor
        {
            get { return ShapeProcess.mMiddleLineColor; }
            set { ShapeProcess.mMiddleLineColor = value; }
        }

        protected static float mMiddleLineThick = 2.0f;
        protected static float MiddleLineThick
        {
            get { return ShapeProcess.mMiddleLineThick; }
            set { ShapeProcess.mMiddleLineThick = value; }
        }

        public ShapeProcess(Section sectionParent)
            : base(sectionParent)
        {
            if (m_painter == null)
            {
                if (imgOut == null)
                    imgOut = global::Sections.Properties.Resources.Process_OUT;
                if (imgInNormal == null)
                    imgInNormal = global::Sections.Properties.Resources.Process_IN;
                if( imgInSkipped == null)
                    imgInSkipped = global::Sections.Properties.Resources.Process_IN_Skipped;
                if( imgInProcessing == null)
                    imgInProcessing = global::Sections.Properties.Resources.Process_IN_Processing;
                if( imgInProcessed == null)
                    imgInProcessed = global::Sections.Properties.Resources.Process_IN_Processed;
                if( imgInWaiting == null)
                    imgInWaiting = global::Sections.Properties.Resources.Process_IN_Waiting;
                if (imgSelect == null)
                    imgSelect = global::Sections.Properties.Resources.Process_OUT_red;
                            
                m_painter = new ImagePainter(imgInNormal, imgInSkipped, imgInProcessing, imgInProcessed, imgInWaiting, imgOut, 50, 50, 5, 5);

                m_painter.ImageSelected = imgSelect;
            }

            if (m_circleImageAutoRun == null)
                m_circleImageAutoRun = global::Sections.Properties.Resources.autoRun_blue;

            base.ImagePainter = m_painter;
            
            DisableNotifier = true;
            m_shapeUp = new Shape(sectionParent);
            m_shapeUp.DisableNotifier = true;
            m_shapeDown = new Shape(sectionParent);
            m_shapeDown.DisableNotifier = true;
                                                             
            notifier.Mode = NotifierDrawMode.RECT;
        
        }

        public override void SetBoundary(ArrayList arrBoundary, float x, float y)
        {
            base.SetBoundary(arrBoundary, x, y);

            ArrayList arrUp = new ArrayList();
            ArrayList arrDown = new ArrayList();

            double dUpRatio = 1.0 - m_dDownRatio;
            float fDownInit = (float)(m_fHeight * dUpRatio);

            int nPointCount = arrBoundary.Count;

            for (int i = 0; i < nPointCount; i++)
            {
                PointF pt = (PointF)arrBoundary[i];
                PointF ptUp = new PointF(pt.X, (float)(pt.Y * dUpRatio));
                PointF ptDown = new PointF(pt.X, fDownInit + (float)(pt.Y * m_dDownRatio));

                arrUp.Add(ptUp);
                arrDown.Add(ptDown);
            }

            m_shapeDown.SetBoundary(arrDown, x, y);
            m_shapeUp.SetBoundary(arrUp, x, y);

            m_arrBoundaryRect[0] = m_shapeUp.GetScreenBoundaryRect(true);
            m_arrBoundaryRect[1] = m_shapeDown.GetScreenBoundaryRect(false);
        }

        public override void ChangePosition(PointF pt)
        {
            base.ChangePosition(pt);
            m_shapeUp.ChangePosition(pt);
            m_shapeDown.ChangePosition(pt);

            m_arrBoundaryRect[0] = m_shapeUp.GetScreenBoundaryRect(true);
            m_arrBoundaryRect[1] = m_shapeDown.GetScreenBoundaryRect(false);
        }

        public override bool ChangeSize(float x, float y, float fWidth, float fHeight)
        {
            if (m_bCheckSize)
            {
                if (fWidth < (float)m_MinSize.Width)
                    fWidth = (float)m_MinSize.Width;

                if (fHeight < (float)m_MinSize.Height)
                    fHeight = (float)m_MinSize.Height;
            }
           
            bool isResult = base.ChangeSize(x, y, fWidth, fHeight);

            float fDownHeight = (float)(fHeight * m_dDownRatio);
            float fUpHeight = fHeight - fDownHeight;

            bool bTemp = m_bCheckSize;
            m_bCheckSize = false;
            m_shapeUp.ChangeSize(x, y, fWidth, fUpHeight);
            m_shapeDown.ChangeSize(x, y, fWidth, fDownHeight);
            m_bCheckSize = bTemp;

            m_arrBoundaryRect[0] = m_shapeUp.GetScreenBoundaryRect(true);
            m_arrBoundaryRect[1] = m_shapeDown.GetScreenBoundaryRect(false);

            return isResult;
        }

        protected override bool Paint(Graphics g, float fx, float fy)
        {
            if (UseImage)
            {
                if (!DrawImage(g, fx, fy))
                    return false;

                PointF pt1 = new PointF(fx + m_fWidth * 57 / 1000, fy + m_fHeight / 2);
                PointF pt2 = new PointF(fx + m_fWidth * 943 / 1000, fy + m_fHeight / 2);
                g.DrawLine(MIDDLE_PEN, pt1, pt2);
            }
            else
            {
                Point[] bound = ClipBoundRect;

                // DRAW SELECT LINE
                int nWidth = (int)m_shapeUp.GetSize(true);
                int nHeight = (int)(m_shapeUp.GetSize(false) + m_shapeDown.GetSize(false));

                int x = (int)fx;
                int y = (int)fy;
                
                Pen pen2 = new Pen(mMiddleLineColor);
                pen2.Width = mMiddleLineThick;
                Pen pen = new Pen(OutLineColor);
                pen.Width = OutLineThick;               

                GraphicsPath path = new GraphicsPath();
                path.StartFigure();
                path.AddLine(x + radius, y, x + nWidth - radius, y);
                path.AddArc(x + nWidth - radius, y, radius, radius, 270, 90);
                path.AddLine(x + nWidth, y + radius, x + nWidth, y + nHeight - radius);
                path.AddArc(x + nWidth - radius, y + nHeight - radius, radius, radius, 0, 90);
                path.AddLine(x + nWidth - radius, y + nHeight, x + radius, y + nHeight);
                path.AddArc(x, y + nHeight - radius, radius, radius, 90, 90);
                path.AddLine(x, y + nHeight - radius, x, y + radius);
                path.AddArc(x, y, radius, radius, 180, 90);
                path.CloseFigure();

                g.SmoothingMode = SmoothingMode.AntiAlias;

                GraphicsPath ShadowPath = new GraphicsPath();
                int addThick = (int)(OutLineThick / 2.0f);
                ShadowPath.StartFigure();
                ShadowPath.AddLine(x + radius - addThick, y - addThick, x + nWidth - radius + addThick, y - addThick);
                ShadowPath.AddArc(x + nWidth - radius + addThick, y - addThick, radius, radius, 270, 90);
                ShadowPath.AddLine(x + nWidth + addThick, y + radius - addThick, x + nWidth + addThick, y + nHeight - radius + addThick);
                ShadowPath.AddArc(x + nWidth - radius + addThick, y + nHeight - radius + addThick, radius, radius, 0, 90);
                ShadowPath.AddLine(x + nWidth - radius + addThick, y + nHeight + addThick, x + radius - addThick, y + nHeight + addThick);
                ShadowPath.AddArc(x - addThick, y + nHeight - radius + addThick, radius, radius, 90, 90);
                ShadowPath.AddLine(x - addThick, y + nHeight - radius + addThick, x - addThick, y + radius - addThick);
                ShadowPath.AddArc(x - addThick, y - addThick, radius, radius, 180, 90);
                ShadowPath.CloseFigure();
                DrawShadow(g, ShadowPath);

                LinearGradientBrush brush = new LinearGradientBrush(new Point(x + nWidth / 2, y), new Point(x + nWidth / 2, y + nHeight), Color.White, FILL_BRUSH);
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
                g.DrawLine(pen2, new Point(x + (int)pen.Width + 5, y + (nHeight - (int)pen2.Width) / 2), new Point(x + nWidth - (int)pen.Width - 5, y + (nHeight - (int)pen2.Width) / 2));

                notifier.SetPosition((int)fx, (int)fy);
                notifier.Size = new Size((int)nWidth, (int)nHeight);
                notifier.Path = path;
                notifier.Paint(g);
            }

            if (((SectionDataProcess)m_sectionParent.Data).AutoRun)
                DrawCircle(g, m_circleImageAutoRun, fx, fy + m_fHeight - m_fCircleDiameter / 2, m_fCircleDiameter, m_fCircleDiameter);

            return true;                                
        }

        private void DrawCircle(Graphics g, Image img, float x, float y, float width, float height)
        {
            if (img != null)
                g.DrawImage(img, x, y, width, height);
        }

        protected override void DrawShadow(Graphics g, GraphicsPath path)
        {
            float fMoveX = 6.0f, fMoveY = 5.0f;
            //float fMoveX = 10.0f, fMoveY = 10.0f;
            g.TranslateTransform(fMoveX, fMoveY);                        

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;          

            System.Drawing.Drawing2D.PathGradientBrush brush = new System.Drawing.Drawing2D.PathGradientBrush(path);
            brush.WrapMode = System.Drawing.Drawing2D.WrapMode.Clamp;

            System.Drawing.Drawing2D.ColorBlend clrBlend = new System.Drawing.Drawing2D.ColorBlend(3);
            clrBlend.Colors = new Color[] { Color.Transparent, Color.FromArgb(180, Color.DimGray), Color.FromArgb(180, Color.DimGray) };

            clrBlend.Positions = new float[] { 0.0f, 0.1f, 1.0f };

            brush.InterpolationColors = clrBlend;         
       
            g.FillPath(brush, path);            

            g.TranslateTransform(-fMoveX, -fMoveY);
        }

        protected override void DrawShadow(Graphics g, PointF[] arrPolygons = null)
        {
            float fMoveX = 10.0f, fMoveY = 10.0f;
            g.TranslateTransform(fMoveX, fMoveY);

            if (arrPolygons == null)
                arrPolygons = m_arrBoundaryTransform;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddPolygon(arrPolygons);

            System.Drawing.Drawing2D.PathGradientBrush brush = new System.Drawing.Drawing2D.PathGradientBrush(path);
            brush.WrapMode = System.Drawing.Drawing2D.WrapMode.Clamp;

            System.Drawing.Drawing2D.ColorBlend clrBlend = new System.Drawing.Drawing2D.ColorBlend(3);
            clrBlend.Colors = new Color[] { Color.Transparent, Color.FromArgb(180, Color.DimGray), Color.FromArgb(180, Color.DimGray) };

            clrBlend.Positions = new float[] { 0.0f, 0.1f, 1.0f };

            brush.InterpolationColors = clrBlend;
            g.FillPath(brush, path);

            g.TranslateTransform(-fMoveX, -fMoveY);
        }

		public void SetFillColor(Color color, bool upside)
		{
			if (upside)
				m_shapeUp.SetFillColor(color);
			else
				m_shapeDown.SetFillColor(color);
		}

		public Color GetFillColor(bool upside)
		{
			return upside ? m_shapeUp.GetFillColor() : m_shapeDown.GetFillColor();
		}

        public override void SetTransparency(bool isLine, bool transparency)
        {
            m_shapeUp.SetTransparency(isLine, transparency);
            m_shapeDown.SetTransparency(isLine, transparency);
        }

        public override bool GetTransparency(bool isLine)
        {
            return m_shapeUp.GetTransparency(isLine);
        }

        public float GetSize(bool isHorz, bool upside)
        {
            return upside ? m_shapeUp.GetSize(isHorz) : m_shapeDown.GetSize(isHorz);
        }
    }
}
