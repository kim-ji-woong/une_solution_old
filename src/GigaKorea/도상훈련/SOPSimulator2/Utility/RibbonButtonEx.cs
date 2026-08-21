using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SOPMonitoringSystem
{
    public enum BtnImgStartPosition { NONE, LEFT, MIDDLE, RIGHT, CENTER }
    public class RibbonButtonSmallToolbar : UnE.GUI.RibbonButton
    {
        protected override void DrawImage(Image img, Graphics g)
        {
            if (Text.Length == 0)
                g.DrawImage(img, 3, 3, this.Size.Width - 3 * 2, this.Size.Height - 3 * 2);
            else
                base.DrawImage(img, g);
        }
    }

    public class RibbonButtonQuick : UnE.GUI.RibbonButton
    {
        private BtnImgStartPosition position { get; set; }
        public RibbonButtonQuick()
            : base()
        {
        }

        public RibbonButtonQuick(int nInitButtonWidth)
            : base(nInitButtonWidth)
        { 
        }
        public RibbonButtonQuick(int nInitButtonWidth, BtnImgStartPosition position = BtnImgStartPosition.NONE)
            : base(nInitButtonWidth)
        {
            this.position = position;
        } 

        protected override void DrawImage(Image img, Graphics g)
        {
            if (this.position == BtnImgStartPosition.RIGHT)
            {
                if (img.Tag == null)
                    g.DrawImage(img, 30, 0, img.Width, img.Height);
                else if (img.Tag != null && img.Tag.ToString() == "OVER")
                    g.DrawImage(img, 0, 0, img.Width, img.Height);                
            }
            else if (this.position == BtnImgStartPosition.MIDDLE)
            {
                if (img.Tag == null)
                    g.DrawImage(img, 20, 0, img.Width, img.Height);
                else if (img.Tag != null && img.Tag.ToString() == "OVER")
                    g.DrawImage(img, 0, 0, img.Width, img.Height);
            }
            else if (this.position == BtnImgStartPosition.LEFT)
            {
                if (img.Tag == null)
                    g.DrawImage(img, 0, 0, img.Width, img.Height);
                else if (img.Tag != null && img.Tag.ToString() == "OVER")
                    g.DrawImage(img, 0, 0, img.Width, img.Height);
            }
            else if (this.position == BtnImgStartPosition.CENTER)
            {
                if (img.Tag == null)
                    g.DrawImage(img, 20, 0, img.Width, img.Height);
                else if (img.Tag != null && img.Tag.ToString() == "OVER")
                    g.DrawImage(img, 0, 0, img.Width, img.Height);
            }
            else //삼천포 기존 소스
            {
                int nWidth = Math.Min(this.Width / 2, this.Height / 2);
                int x = (Width - nWidth) / 2;
                int y = (Height - nWidth) / 2;
                if (y < 0)
                    y = 0;
                g.DrawImage(img, x, y, nWidth, nWidth);

                int tx = x + nWidth / 2 - m_rect.Width / 2;
                int ty = y + nWidth + 10;
                m_rect.X = tx;
                m_rect.Y = ty;
            } 
        }
    }
}
