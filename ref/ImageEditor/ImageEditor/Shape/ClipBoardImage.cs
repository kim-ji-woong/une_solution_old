using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEditor
{
    class ClipBoardImage
    {
        public ClipBoardImage(Image img, bool type)
        {
            m_img = img;
            m_Type = type;
        }

        public ClipBoardImage(Rectangle rect, bool type)
        {
            m_rect = rect;
            m_Type = type;
        }

        //type이 true일때만
        public ClipBoardImage(Image img, Size size, bool type = true)
        {
            m_img = img;
            m_size = size;
            m_Type = type;
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

        private Rectangle m_rect;
        public Rectangle Rect
        {
            get { return m_rect; }
            set { m_rect = value; }
        }

        //타입이 true면 복사, false면 삭제
        private bool m_Type;
        public bool Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        private bool m_Selected = false;
        public bool Selected
        {
            get { return m_Selected; }
            set { m_Selected = value; }
        }
    }
}
