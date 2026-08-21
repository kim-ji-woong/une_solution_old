using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPMonitoringSystem
{
    public partial class ShadowControl : Control
    {
        private PointF[] m_arrBoundary = new PointF[4] { new PointF(0.0f, 0.0f),
            new PointF(0.0f, 0.0f),
            new PointF(0.0f, 0.0f),
            new PointF(0.0f, 0.0f)};

        public ShadowControl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddPolygon(m_arrBoundary);

            System.Drawing.Drawing2D.PathGradientBrush brush = new System.Drawing.Drawing2D.PathGradientBrush(path);
            brush.WrapMode = System.Drawing.Drawing2D.WrapMode.Clamp;

            System.Drawing.Drawing2D.ColorBlend clrBlend = new System.Drawing.Drawing2D.ColorBlend(3);
            clrBlend.Colors = new Color[] { Color.Transparent, Color.FromArgb(180, Color.DimGray), Color.FromArgb(180, Color.DimGray) };

            clrBlend.Positions = new float[] { 0.0f, 0.1f, 1.0f };

            brush.InterpolationColors = clrBlend;
            g.FillPath(brush, path);
        }

        public void SetBoundary(Point ptTL, Size size)
        {
            this.Location = ptTL;
            this.Size = size;

            m_arrBoundary[0].X = 0.0f;
            m_arrBoundary[0].Y = 0.0f;
            m_arrBoundary[1].X = size.Width;
            m_arrBoundary[1].Y = m_arrBoundary[0].Y;
            m_arrBoundary[2].X = m_arrBoundary[1].X;
            m_arrBoundary[2].Y = size.Height;
            m_arrBoundary[3].X = m_arrBoundary[0].X;
            m_arrBoundary[3].Y = m_arrBoundary[2].Y;
        }
    }
}
