using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SOPMonitoringSystem
{
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
        public RibbonButtonQuick()
            : base()
        {
        }

        public RibbonButtonQuick(int nInitButtonWidth)
            : base(nInitButtonWidth)
        {
        }

        protected override void DrawImage(Image img, Graphics g)
        {
            g.DrawImage(img, 0, 0);
        }
    }
}
