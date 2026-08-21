using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEditor
{
    class EditImage
    {
        public EditImage(Image img, Point pt)
        {
            m_img = img;
            m_DrawPt = pt;
        }

        private Image m_img;
        public Image Img
        {
            get { return m_img; }
            set { m_img = value; }
        }

        private Size m_size;
        public Size size
        {
            get { return m_size; }
            set { m_size = value; }
        }

        private Point m_DrawPt;
        public Point DrawPt
        {
            get { return m_DrawPt; }
            set { m_DrawPt = value; }
        }
        

        private bool m_Selected = false;
        public bool Selected
        {
            get { return m_Selected; }
            set { m_Selected = value; }
        }
    }
}
