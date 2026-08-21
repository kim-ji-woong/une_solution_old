using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using GDK;

namespace GDK_tester
{
    public class spin_circle
    {
        public class option_type
        {
            public option_type()
            {
                inner_radius = 7.5F;
                outer_radius = 8.0F;
                tickness = 4.5F;
                count_spoke = 9;
            }
            public float inner_radius;
            public float outer_radius;
            public float tickness;
            public int count_spoke;
        }
        public spin_circle()
        {
            option = new option_type();
            progress = 0;
        }
        public bool create(Color color, PointF center)
        {
            this.progress = 0;
            this.color = color;
            this.center = center;
            this.colors = imp_gen_colors(color, true, option.count_spoke, option.count_spoke);
            this.angles = imp_gen_angles(option.count_spoke);
            this.created = (this.colors.Length >= option.count_spoke &&
                            this.angles.Length >= option.count_spoke);
            return this.created;
        }
        public void cleanup()
        {
            this.created = false;
            this.progress = 0;
            this.colors = null;
            this.angles = null;
        }
        public bool render(Graphics g)
        {
            if (created)
            {
                SmoothingMode pre = g.SmoothingMode;
                g.SmoothingMode = SmoothingMode.HighQuality;
                int pos = progress;
                for (int i = 0; i < option.count_spoke; ++i, ++pos)
                {
                    pos = pos % option.count_spoke;
                    imp_disp(g, imp_calc_coord(ref center, option.inner_radius, angles[pos]),
                                imp_calc_coord(ref center, option.outer_radius, angles[pos]),
                                colors[i], option.tickness);
                }
                g.SmoothingMode = pre;
                return true;
            }
            return false;
        }
        public void set_progress(int progress)
        {
            progress %= option.count_spoke;
        }
        public void update()
        {
            progress = ++progress % option.count_spoke;
        }

        protected Color[] imp_gen_colors(Color color, bool shade, int total, int count_spoke)
        {
            Color[] res = new Color[total];
            int inc = byte.MaxValue / count_spoke;
            int percent_darken = 0;

            for (int i = 0; i < total; ++i)
            {
                if (shade)
                {
                    if (i == 0 ||
                        i < total - count_spoke)
                    {
                        res[i] = color;
                    }
                    else
                    {
                        percent_darken = Math.Min(percent_darken + inc, byte.MaxValue);
                        res[i] = imp_calc_darken(ref color, percent_darken);
                    }
                }
                else
                {
                    res[i] = color;
                }
            }
            return res;
        }
        protected float[] imp_gen_angles(int count_spoke)
        {
            float[] res = new float[count_spoke];
            float a = 360.0F / count_spoke;
            for (int i = 0; i < count_spoke; ++i)
            {
                res[i] = (i == 0) ? a : res[i - 1] + a;
            }
            return res;
        }
        protected Color imp_calc_darken(ref Color color, int percent)
        {
            return Color.FromArgb(percent, color);
        }
        protected PointF imp_calc_coord(ref PointF center, float radius, float angle)
        {
            double factor = Math.PI * angle / 180.0;
            return new PointF((float)(center.X + radius * Math.Cos(factor)), (float)(center.Y + radius * Math.Sin(factor)));
        }
        protected void imp_disp(Graphics g, PointF start, PointF end, Color color, float thickness)
        {
            SolidBrush br = new SolidBrush(color);
            Pen pen = new Pen(new SolidBrush(color), thickness);
            pen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
            g.DrawLine(pen, start, end);
        }

        protected option_type option;
        protected bool created;
        protected int progress;
        protected Color color;
        protected PointF center;
        protected float[] angles;
        protected Color[] colors;
    }
}
