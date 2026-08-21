using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using Sections;

namespace SectionStyle.Fancy
{
    class Style : IShapeStyler
    {
        protected class ColorSet
        {
            private Color m_lineColor = Color.Gray;
            private Color m_fillStartColor = Color.White;
            private Color m_fillEndColor = Color.Gray;

            public Color LineColor
            {
                get { return m_lineColor; }
                set { m_lineColor = value; }
            }

            public Color FillStartColor
            {
                get { return m_fillStartColor; }
                set { m_fillStartColor = value; }
            }

            public Color FillEndColor
            {
                get { return m_fillEndColor; }
                set { m_fillEndColor = value; }
            }

            public ColorSet()
            {
            }

            public ColorSet(Color line, Color fillStart, Color fillEnd)
            {
                m_lineColor = line;
                m_fillStartColor = fillStart;
                m_fillEndColor = fillEnd;
            }
        }

        protected Shape m_shape = null;
        protected Section m_section = null;
        protected PointF[] m_arrDrawing = null;
        protected GraphicsPath m_path = new GraphicsPath();

        protected static Dictionary<Shape.ShapeStatus, ColorSet> m_dicStateColor = new Dictionary<Shape.ShapeStatus, ColorSet>();
        protected static ColorSet m_currentStateColor = null;

        private static Color m_processedTextColor = Color.FromArgb(179, 184, 200);
        private static Color m_normalTextColor = Color.FromArgb(58, 58, 58);
        private static Color m_processingTextColor = Color.White;

        private static Font MainFont = new Font("나눔바른고딕", 11.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(129)));

        protected Shape.ShapeStatus m_status = Shape.ShapeStatus.NORMAL;
        protected bool m_isCurrentState = false;
        protected ColorSet m_colorSet = null;

        public Style()
        {
            m_dicStateColor.TryGetValue(Shape.ShapeStatus.NORMAL, out m_colorSet);

            if (m_section != null)
            {
                m_section.TextFont = MainFont;
            }
        }

        public Shape.DrawType GetDrawType()
        {
            return Shape.DrawType.OwnDrawing;
        }

        public bool Draw(Graphics g, float x, float y, PathNotifier notifier)
        {
            return Paint(g, x, y, notifier);
        }

        public void OnCreate(float x, float y)
        {
            Create(x, y);
        }

        public void OnResize(float x, float y, float fWidth, float fHeight)
        {
            Resize(x, y, fWidth, fHeight);
        }

        public void OnMove(float x, float y)
        {
            Move(x, y);
        }

        public void SetState(Shape.ShapeStatus status)
        {
            if (m_colorSet == null || m_status != status)
            {
                m_status = status;

                if (m_isCurrentState)
                {
                    m_colorSet = m_currentStateColor;
                    SetTextColor(Shape.ShapeStatus.NORMAL);
                }
                else
                {
                    m_dicStateColor.TryGetValue(status, out m_colorSet);
                    SetTextColor(m_status);
                }
            }
        }

        public void SetCurrent(bool isCurrent)
        {
            if (m_isCurrentState != isCurrent)
            {
                m_isCurrentState = isCurrent;

                if (m_isCurrentState)
                {
                    m_colorSet = m_currentStateColor;
                    SetTextColor(Shape.ShapeStatus.NORMAL);
                }
                else
                {
                    m_dicStateColor.TryGetValue(m_status, out m_colorSet);
                    SetTextColor(m_status);
                }
            }
        }

        private void SetTextColor(Shape.ShapeStatus status)
        {
            if (m_section != null)
            {
                if (status == Shape.ShapeStatus.PROCESSED)
                    m_section.TextColor = m_processedTextColor;
                else if (status == Shape.ShapeStatus.PROCESSING)
                    m_section.TextColor = m_processingTextColor;
                else
                    m_section.TextColor = m_normalTextColor;
            }
        }

        protected virtual void DrawShadow(Graphics g, GraphicsPath path)
        {
            if (path == null)
                return;
            //float fMoveX = 10.0f, fMoveY = 10.0f;
            float fMoveX = 9.0f, fMoveY = 7.0f;
            g.TranslateTransform(fMoveX, fMoveY);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            PathGradientBrush brush = new PathGradientBrush(path);
            brush.WrapMode = WrapMode.Clamp;

            ColorBlend clrBlend = new ColorBlend(3);
            clrBlend.Colors = new Color[] { Color.Transparent, Color.FromArgb(180, Color.LightGray), Color.FromArgb(180, Color.LightGray) };

            clrBlend.Positions = new float[] { 0.0f, 0.1f, 1.0f };

            brush.InterpolationColors = clrBlend;
            g.FillPath(brush, path);

            g.TranslateTransform(-fMoveX, -fMoveY);
        }

        protected virtual void Create(float x, float y)
        {
        }

        protected virtual void Resize(float x, float y, float fWidth, float fHeight)
        {
        }

        protected virtual void Move(float x, float y)
        {
        }

        protected virtual bool Paint(Graphics g, float x, float y, PathNotifier notifier)
        {
            if (m_shape == null || m_colorSet == null)
                return false;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            int nWidth = (int)m_shape.GetSize(true);
            int nHeight = (int)m_shape.GetSize(false);

            DrawShadow(g, m_path);

            LinearGradientBrush brush = new LinearGradientBrush(
                new Point((int)x + nWidth / 2, (int)y),
                new Point((int)x + nWidth / 2, (int)y + nHeight),
                m_colorSet.FillStartColor,
                m_colorSet.FillEndColor
            );
            g.FillPath(brush, m_path);
            brush.Dispose();

            Pen pen = new Pen(m_colorSet.LineColor);
            pen.Width = 4;
            g.DrawPath(pen, m_path);
            pen.Dispose();

            if (notifier != null && m_shape.DisableNotifier == false)
            {
                notifier.SetPosition((int)x, (int)y);
                notifier.Size = new Size((int)m_shape.GetSize(true), (int)m_shape.GetSize(false));
                notifier.Path = m_path;
                notifier.Paint(g);
            }

            return true;
        }

        public static void InitStateColor()
        {
            m_dicStateColor[Shape.ShapeStatus.NORMAL] = m_dicStateColor[Shape.ShapeStatus.WAITING] = m_dicStateColor[Shape.ShapeStatus.SKIPPED] = new ColorSet(Color.FromArgb(228, 228, 228), Color.White, Color.FromArgb(242, 242, 242));
            m_dicStateColor[Shape.ShapeStatus.PROCESSING] = new ColorSet(Color.FromArgb(132, 154, 214), Color.FromArgb(80, 104, 169), Color.FromArgb(92, 122, 203));
            m_dicStateColor[Shape.ShapeStatus.PROCESSED] = new ColorSet(Color.FromArgb(89, 94, 108), Color.FromArgb(57, 65, 89), Color.FromArgb(101, 110, 132));
            m_currentStateColor = new ColorSet(Color.FromArgb(255, 136, 77), Color.FromArgb(216, 119, 70), Color.FromArgb(235, 178, 150));
        }
    }
}
