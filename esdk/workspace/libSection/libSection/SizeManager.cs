using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Sections
{
    public class SizeManager
    {
        protected EditBox.BoxPosition m_optChangeSize = EditBox.BoxPosition.NO_SELECT;
        protected PointF m_ptChangeSizeInit = new PointF();
        protected PointF m_ptChangeSizeStart = new PointF();
        protected SizeF m_initChangeSize = new SizeF();

        protected EditBox m_editBox = null;
        protected Shape m_shape = null;
        protected PositionManager m_posMgr = null;

        protected bool m_isEditable = true;

        public bool Editable
        {
            get { return m_isEditable; }
            set { m_isEditable = value; }
        }

        protected static Size m_MinSize = new Size(80, 40);
        public static Size MinSize
        {
            get { return m_MinSize; }
            set 
            {
                Section.MinSize = value;
                m_MinSize = value; 
            }
        }

        protected static bool m_bCheckSize = true;
        public static bool CheckSize
        {
            get { return m_bCheckSize; }
            set { m_bCheckSize = value; }
        }


        public SizeManager(EditBox editBox, Shape shape, PositionManager posMgr)
        {
            m_editBox = editBox;
            m_shape = shape;
            m_posMgr = posMgr;
        }

        public bool CheckMouse(float x, float y, bool isSelected, PanelSection ctrlParent)
        {
            if (ctrlParent.Editable)
            {
                if (m_editBox == null)
                    return false;

                if (isSelected && Editable)
                {
                    m_optChangeSize = m_editBox.CheckMouse((float)x, (float)y);

                    if (m_optChangeSize == EditBox.BoxPosition.NO_SELECT)
                    {
                        ctrlParent.Cursor = Cursors.Arrow;
                        return false;
                    }
	
                    switch (m_optChangeSize)
                    {
                        case EditBox.BoxPosition.TOP_LEFT:
                        case EditBox.BoxPosition.BOTTOM_RIGHT:
                            ctrlParent.Cursor = Cursors.SizeNWSE;
                            break;

                        case EditBox.BoxPosition.TOP_RIGHT:
                        case EditBox.BoxPosition.BOTTOM_LEFT:
                            ctrlParent.Cursor = Cursors.SizeNESW;
                            break;

                        case EditBox.BoxPosition.TOP_MIDDLE:
                        case EditBox.BoxPosition.BOTTOM_MIDDLE:
                            ctrlParent.Cursor = Cursors.SizeNS;
                            break;

                        case EditBox.BoxPosition.MIDDLE_LEFT:
                        case EditBox.BoxPosition.MIDDLE_RIGHT:
                            ctrlParent.Cursor = Cursors.SizeWE;
                            break;
                    }

                    return true;
                }

                m_optChangeSize = EditBox.BoxPosition.NO_SELECT;
                ctrlParent.Cursor = Cursors.Arrow;
            }

            return false;
        }

        public EditBox.BoxPosition GetChangeSizeOption()
        {
            return m_optChangeSize;
        }

        public void SetChangeSizeOriginPoint(float x, float y, PointF ptCurrent, float fWidth, float fHeight)
        {
            m_ptChangeSizeInit.X = x;
            m_ptChangeSizeInit.Y = y;

            m_ptChangeSizeStart.X = ptCurrent.X;
            m_ptChangeSizeStart.Y = ptCurrent.Y;

            m_initChangeSize.Width = fWidth;
            m_initChangeSize.Height = fHeight;
        }

        public bool ChangeSize(float x, float y, PositionManager posMgr)
        {
            if (m_optChangeSize == EditBox.BoxPosition.NO_SELECT)
                return false;

            float xMove = x - m_ptChangeSizeInit.X;
            float yMove = y - m_ptChangeSizeInit.Y;




            switch (m_optChangeSize)
            {
                case EditBox.BoxPosition.TOP_LEFT:
                    if (m_bCheckSize == true && (m_initChangeSize.Width - xMove) > m_MinSize.Width
                          && (m_initChangeSize.Height - yMove) > m_MinSize.Height)
                    {
                        posMgr.Position = new PointF(m_ptChangeSizeStart.X + xMove, m_ptChangeSizeStart.Y + yMove);
                        RectSize = new SizeF(m_initChangeSize.Width - xMove, m_initChangeSize.Height - yMove);
                    }

                    if (m_bCheckSize == false)
                    {
                        posMgr.Position = new PointF(m_ptChangeSizeStart.X + xMove, m_ptChangeSizeStart.Y + yMove);
                        RectSize = new SizeF(m_initChangeSize.Width - xMove, m_initChangeSize.Height - yMove);
                    }
                   
                    break;

                case EditBox.BoxPosition.BOTTOM_RIGHT:
                    if (m_bCheckSize == true && (m_initChangeSize.Width + xMove) > m_MinSize.Width
                          && (m_initChangeSize.Height + yMove) > m_MinSize.Height)
                    {
                        RectSize = new SizeF(m_initChangeSize.Width + xMove, m_initChangeSize.Height + yMove);
                    }
                    if (m_bCheckSize == false)
                    {
                        RectSize = new SizeF(m_initChangeSize.Width + xMove, m_initChangeSize.Height + yMove);
                    }                    
                    break;

                case EditBox.BoxPosition.TOP_RIGHT:
                    if (m_bCheckSize == true && (m_initChangeSize.Width + xMove) > m_MinSize.Width
                        && (m_initChangeSize.Height - yMove) > m_MinSize.Height)
                    {
                        posMgr.Position = new PointF(m_ptChangeSizeStart.X, m_ptChangeSizeStart.Y + yMove);
                        RectSize = new SizeF(m_initChangeSize.Width + xMove, m_initChangeSize.Height - yMove);
                    }

                    if (m_bCheckSize == false)
                    {
                        posMgr.Position = new PointF(m_ptChangeSizeStart.X, m_ptChangeSizeStart.Y + yMove);
                        RectSize = new SizeF(m_initChangeSize.Width + xMove, m_initChangeSize.Height - yMove);
                    }
                    break;

                case EditBox.BoxPosition.BOTTOM_LEFT:
                    if (m_bCheckSize == true && (m_initChangeSize.Width - xMove) > m_MinSize.Width
                        &&( m_initChangeSize.Height + yMove) > m_MinSize.Height)
                    {
                        posMgr.Position = new PointF(m_ptChangeSizeStart.X + xMove, m_ptChangeSizeStart.Y);
                        RectSize = new SizeF(m_initChangeSize.Width - xMove, m_initChangeSize.Height + yMove);
                    }
                    
                    if( m_bCheckSize == false)
                    {
                        posMgr.Position = new PointF(m_ptChangeSizeStart.X + xMove, m_ptChangeSizeStart.Y);
                        RectSize = new SizeF(m_initChangeSize.Width - xMove, m_initChangeSize.Height + yMove);
                    }
                    break;

                case EditBox.BoxPosition.TOP_MIDDLE:
                    if (m_bCheckSize == true && m_initChangeSize.Height - yMove > m_MinSize.Height)
                    {
                        posMgr.Position = new PointF(m_ptChangeSizeStart.X, m_ptChangeSizeStart.Y + yMove);
                        RectSize = new SizeF(m_initChangeSize.Width, m_initChangeSize.Height - yMove);
                    }
                    if( m_bCheckSize == false)
                    {
                        posMgr.Position = new PointF(m_ptChangeSizeStart.X, m_ptChangeSizeStart.Y + yMove);
                        RectSize = new SizeF(m_initChangeSize.Width, m_initChangeSize.Height - yMove);
                    }                   
                    break;

                case EditBox.BoxPosition.BOTTOM_MIDDLE:
                    if (m_bCheckSize == true && m_initChangeSize.Height + yMove > m_MinSize.Height)
                    {
                        posMgr.Position = new PointF(m_ptChangeSizeStart.X, m_ptChangeSizeStart.Y);
                        RectSize = new SizeF(m_initChangeSize.Width, m_initChangeSize.Height + yMove);
                    }       
            
                    if( m_bCheckSize == false)
                    {
                        posMgr.Position = new PointF(m_ptChangeSizeStart.X, m_ptChangeSizeStart.Y);
                        RectSize = new SizeF(m_initChangeSize.Width, m_initChangeSize.Height + yMove);
                    }
                    break;
                case EditBox.BoxPosition.MIDDLE_LEFT:
                    if (m_bCheckSize == true &&  (m_initChangeSize.Width - xMove) > m_MinSize.Width)
                    {
                        posMgr.Position = new PointF(m_ptChangeSizeStart.X + xMove, m_ptChangeSizeStart.Y);
                        RectSize = new SizeF(m_initChangeSize.Width - xMove, m_initChangeSize.Height);
                    }    
                    if( m_bCheckSize == false)
                    {
                        posMgr.Position = new PointF(m_ptChangeSizeStart.X + xMove, m_ptChangeSizeStart.Y);
                        RectSize = new SizeF(m_initChangeSize.Width - xMove, m_initChangeSize.Height);
                    }
                    break;
                case EditBox.BoxPosition.MIDDLE_RIGHT:
                    if (m_bCheckSize == true && (m_initChangeSize.Width + xMove) > m_MinSize.Width)
                    {
                        RectSize = new SizeF(m_initChangeSize.Width + xMove, m_initChangeSize.Height);
                    }
                    if( m_bCheckSize == false)
                    {
                        RectSize = new SizeF(m_initChangeSize.Width + xMove, m_initChangeSize.Height);
                    }
                    break;
            }

            return true;
        }

        public virtual SizeF RectSize
        {
            get
            {
                return new SizeF(m_shape.GetSize(true), m_shape.GetSize(false));
            }
            set
            {
               // System.Diagnostics.Trace.WriteLine(m_posMgr.Position);
                if (m_shape.ChangeSize(m_posMgr.Position.X, m_posMgr.Position.Y, value.Width, value.Height))
                {
                    
                    m_editBox.RectSize = value;
                }
            }
        }
    }
}
