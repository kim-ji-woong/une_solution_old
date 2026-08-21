using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Sections
{
    public class SizeManagerLink : SizeManager
    {
        PanelSection m_ctrlParent = null;

        public SizeManagerLink(EditBox editBox, Shape shape, PositionManager posMgr, PanelSection ctrlParent)
            : base(editBox, shape, posMgr)
        {
            m_ctrlParent = ctrlParent;
        }

        public override SizeF RectSize
        {
            get
            {
                return base.RectSize;
            }
            set
            {
                // 가로, 세로가 같은 크기로 늘어나거나 줄어들도록 한다.
                if (m_ctrlParent.Cursor == System.Windows.Forms.Cursors.SizeWE)
                    value.Height = value.Width;
                else if (m_ctrlParent.Cursor == System.Windows.Forms.Cursors.SizeNS)
                    value.Width = value.Height;
                else
                {
                    if (value.Width > value.Height)
                        value.Height = value.Width;
                    else
                        value.Width = value.Height;
                }

                base.RectSize = value;
            }
        }
    }
}
