using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Sections
{
    public class ImagePainter
    {
        private Image m_imgInNormal = null;
        private Image m_imgInSkipped = null;
        private Image m_imgInProcessing = null;
        private Image m_imgInProcessed = null;
        private Image m_imgInWaiting = null;
        private Image m_imgOut = null;

        private int m_nOriginSpaceX = 0;
        private int m_nOriginSpaceY = 0;
        private int m_nTargetSpaceX = 0;
        private int m_nTargetSpaceY = 0;

        private int m_nOriginImageWidth = 0;
        private int m_nOriginImageHeight = 0;

        public Image ImageInNormal
        {
            get { return m_imgInNormal; }
            set
            {
                m_imgInNormal = value;

                if (m_imgInNormal != null)
                {
                    m_nOriginImageWidth = m_imgInNormal.Width;
                    m_nOriginImageHeight = m_imgInNormal.Height;
                }
            }
        }

        public Image ImageInSkipped
        {
            get { return m_imgInSkipped; }
            set
            {
                m_imgInSkipped = value;

                if (m_imgInSkipped != null)
                {
                    m_nOriginImageWidth = m_imgInSkipped.Width;
                    m_nOriginImageHeight = m_imgInSkipped.Height;
                }
            }
        }

        public Image ImageInProcessing
        {
            get { return m_imgInProcessing; }
            set
            {
                m_imgInProcessing = value;

                if (m_imgInProcessing != null)
                {
                    m_nOriginImageWidth = m_imgInProcessing.Width;
                    m_nOriginImageHeight = m_imgInProcessing.Height;
                }
            }
        }

        public Image ImageInProcessed
        {
            get { return m_imgInProcessed; }
            set
            {
                m_imgInProcessed = value;

                if (m_imgInProcessed != null)
                {
                    m_nOriginImageWidth = m_imgInProcessed.Width;
                    m_nOriginImageHeight = m_imgInProcessed.Height;
                }
            }
        }

        public Image ImageInWaiting
        {
            get { return m_imgInWaiting; }
            set
            {
                m_imgInWaiting = value;

                if (m_imgInWaiting != null)
                {
                    m_nOriginImageWidth = m_imgInWaiting.Width;
                    m_nOriginImageHeight = m_imgInWaiting.Height;
                }
            }
        }

        private Image m_ImageSelected = null;
        public Image ImageSelected
        {
            get { return m_ImageSelected; }
            set {
                m_ImageSelected = value; }
        }

        private bool m_bEnableBlink = true;
        private bool m_bDrawOutImage = true;

        private bool m_bSelected = false;
        public bool Selected
        {
            get { return m_bSelected; }
            set 
            { 
                m_bSelected = value;
                if (m_bSelected == false)
                    m_bDrawOutImage = true;
            }
        }

        public Image ImageOut
        {
            get { return m_imgOut; }
            set { m_imgOut = value; }
        }

        public int OriginSpaceX
        {
            get { return m_nOriginSpaceX; }
            set { m_nOriginSpaceX = value; }
        }

        public int OriginSpaceY
        {
            get { return m_nOriginSpaceY; }
            set { m_nOriginSpaceY = value; }
        }

        public int TargetSpaceX
        {
            get { return m_nTargetSpaceX; }
            set { m_nTargetSpaceX = value; }
        }

        public int TargetSpaceY
        {
            get { return m_nTargetSpaceY; }
            set { m_nTargetSpaceY = value; }
        }

        public ImagePainter()
        {
        }

        public ImagePainter(Image imgInNormal, Image imgInSkipped, Image imgInProcessing, Image imgInProcessed, Image imgInWating, Image imgOut, int nOriginSpaceX, int nOriginSpaceY, int nTargetSpaceX, int nTargetSpaceY)
        {
            m_imgInNormal = imgInNormal;
            m_imgInSkipped = imgInSkipped;
            m_imgInProcessing = imgInProcessing;
            m_imgInProcessed = imgInProcessed;
            m_imgInWaiting = imgInWating;
            m_imgOut = imgOut;
            m_nOriginSpaceX = nOriginSpaceX;
            m_nOriginSpaceY = nOriginSpaceY;
            m_nTargetSpaceX = nTargetSpaceX;
            m_nTargetSpaceY = nTargetSpaceY;

            if (m_imgInNormal != null)
            {
                m_nOriginImageWidth = m_imgInNormal.Width;
                m_nOriginImageHeight = m_imgInNormal.Height;
            }
        }

        public bool Draw(Graphics g, float x, float y, float fWidth, float fHeight, Shape.ShapeStatus status)
        {
            Image imgIn = null;

            if (status == Shape.ShapeStatus.NORMAL)
                imgIn = m_imgInNormal;
            else if (status == Shape.ShapeStatus.SKIPPED)
                imgIn = m_imgInSkipped;
            else if (status == Shape.ShapeStatus.PROCESSING)
                imgIn = m_imgInProcessing;
            else if (status == Shape.ShapeStatus.PROCESSED)
                imgIn = m_imgInProcessed;
            else if (status == Shape.ShapeStatus.WAITING)
                imgIn = m_imgInWaiting;

            if (imgIn == null || m_imgOut == null || m_nOriginImageWidth == 0 || m_nOriginImageHeight == 0)
                return false;

            if (m_nOriginImageWidth - m_nOriginSpaceX * 2 <= 0)
                return false;

            if (m_nOriginImageHeight - m_nOriginSpaceY * 2 <= 0)
                return false;

            float fWidth2 = m_nOriginImageWidth * (fWidth - m_nTargetSpaceX * 2) / (m_nOriginImageWidth - m_nOriginSpaceX * 2);
            float fHeight2 = m_nOriginImageHeight * (fHeight - m_nTargetSpaceY * 2) / (m_nOriginImageHeight - m_nOriginSpaceY * 2);

            float spaceX2 = m_nOriginSpaceX * fWidth2 / m_nOriginImageWidth;
            float spaceY2 = m_nOriginSpaceY * fHeight2 / m_nOriginImageHeight;

            float _x = x - (spaceX2 - m_nTargetSpaceX);
            float _y = y - (spaceY2 - m_nTargetSpaceY);

            if (m_bEnableBlink == false)
                g.DrawImage(m_imgOut, x, y, fWidth, fHeight);
            else
            {
                if (m_bSelected == true && m_bDrawOutImage == true)
                {
                    if(m_ImageSelected != null)
                        g.DrawImage(m_ImageSelected, x, y, fWidth, fHeight);
                    //else
                    //    g.DrawImage(m_imgOut, x, y, fWidth, fHeight);
                }
                    
                else
                {
                    g.DrawImage(m_imgOut, x, y, fWidth, fHeight);
                } 
            }
                
                
            g.DrawImage(imgIn, _x, _y, fWidth2, fHeight2);

            return true;
        }

        public void DrawOutImage(bool bDraw)
        {
            m_bDrawOutImage = bDraw;
        }
        public void EnableBlink(bool bEnable)
        {
            m_bEnableBlink = bEnable;
        }
    }
}
