using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Sections
{
    public class PositionManager
    {
        protected float x, y;
		protected Shape m_shape = null;
		protected Button m_btnScroll = null;
		protected EditBox m_editBox = null;
		protected Section m_sectionParent = null;

        public PositionManager(Section sectionParent, float x = 0, float y = 0)
        {
            m_sectionParent = sectionParent;
            this.x = x;
            this.y = y;
        }

        public PositionManager(Section sectionParent, Shape shape, Button btnScroll, EditBox editBox, float x = 0, float y = 0)
        {
            m_sectionParent = sectionParent;
            m_shape = shape;
            m_btnScroll = btnScroll;
            m_editBox = editBox;
            this.x = x;
            this.y = y;
        }

		protected bool m_bEditable = true;
		public virtual bool Editable
		{
			get { return m_bEditable; }
			set { m_bEditable = value; }
		}

        public void SetShape(Shape shape)
        {
            m_shape = shape;
        }

        public void SetScrollButton(Button btnScroll)
        {
            m_btnScroll = btnScroll;
        }

        public void SetEditBox(EditBox editBox)
        {
            m_editBox = editBox;
        }

        public virtual PointF Position
        {
            get
            {
                return new PointF(x, y);
            }
            set
            {
				if (m_bEditable == false)
					return;

                if (x != value.X || y != value.Y)
                {
                    if (m_shape != null)
                        m_shape.ChangePosition(value);

                    x = value.X;
                    y = value.Y;

                    // 화살표 위치 변경
                    m_sectionParent.CalcArrowPositions();

                    if (m_btnScroll != null)
                        m_btnScroll.Location = new Point((int)(x + m_sectionParent.GetScrollButtonArea(true)), (int)(y + m_sectionParent.GetScrollButtonArea(false)));

                    if (m_editBox != null)
                        m_editBox.Position = value;
                }
            }
        }
    }
}
