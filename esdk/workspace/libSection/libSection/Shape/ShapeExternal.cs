using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Sections
{
    public class ShapeExternal : Shape
    {
        private ImagePainter m_painter = null;
        private static Image m_circleImage = null;
        private static float m_fCircleDiameter = 35;

        public ImagePainter ImagePainter
        {
            get { return m_painter; }
            set { m_painter = value; }
        }

        public static Image CircleImage
        {
            get { return m_circleImage; }
            set { m_circleImage = value; }
        }
        private static Image imgOut = null;
        private static Image imgInNormal = null;
        private static Image imgInSkipped = null;
        private static Image imgInProcessing = null;
        private static Image imgInProcessed = null;
        private static Image imgInWaiting = null;
        private static Image imgSelect = null;

        public ShapeExternal(Section sectionParent)
            : base(sectionParent)
        {
            if (m_painter == null)
            {
                if (imgOut == null)
                    imgOut = global::Sections.Properties.Resources.External_OUT;
                if (imgInNormal == null)
                    imgInNormal = global::Sections.Properties.Resources.External_IN;
                if( imgInSkipped == null)
                    imgInSkipped = global::Sections.Properties.Resources.External_IN_Skipped;
                if( imgInProcessing == null)
                    imgInProcessing = global::Sections.Properties.Resources.External_IN_Processing;
                if( imgInProcessed == null)
                    imgInProcessed = global::Sections.Properties.Resources.External_IN_Processed;
                if( imgInWaiting == null)
                    imgInWaiting = global::Sections.Properties.Resources.External_IN_Waiting;

                if (imgSelect == null)
                    imgSelect = global::Sections.Properties.Resources.External_OUT_red;
                m_painter = new ImagePainter(imgInNormal, imgInSkipped, imgInProcessing, imgInProcessed, imgInWaiting, imgOut, 48, 48, 5, 5);

                m_painter.ImageSelected = imgSelect;
            }

            if (m_circleImage == null)
            {
                m_circleImage = global::Sections.Properties.Resources.Blackcircle_External;
            }

            base.ImagePainter = m_painter;
        }

        protected override bool DrawImage(Graphics g, float x, float y)
        {
            if (m_imgPainter != null)
            {
                if (!m_imgPainter.Draw(g, x, y, m_fWidth, m_fHeight, Status))
                    return false;
            }
            else
                return false;

            if (m_circleImage != null)
            {
                //g.DrawImage(m_circleImage, x + m_fWidth - m_fCircleDiameter, y - m_fCircleDiameter / 2, m_fCircleDiameter, m_fCircleDiameter);
                g.DrawImage(m_circleImage, x - 5, y - m_fCircleDiameter / 2, m_fCircleDiameter, m_fCircleDiameter);
            }
            else
                return false;

            return true;
        }

        protected override bool Paint(Graphics g, float x, float y)
        {
            bool bResult = base.Paint(g, x, y);
         
            if (m_circleImage != null)
            {
                
                g.DrawImage(m_circleImage, x - 5, y - m_fCircleDiameter / 2, m_fCircleDiameter, m_fCircleDiameter);
            }

            return bResult;
        }
    }
}
