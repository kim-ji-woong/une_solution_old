using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Sections
{
    public class ShapeInternal : Shape
    {
        private ImagePainter m_painter = null;
        private static Image m_circleImageSMS = null;
        private static Image m_circleImageBroadcast = null;
        private static Image m_circleImageAutoRun = null;
        private static float m_fCircleDiameter = 35;

        public ImagePainter ImagePainter
        {
            get { return m_painter; }
            set { m_painter = value; }
        }

        public static Image CircleImageSMS
        {
            get { return m_circleImageSMS; }
            set { m_circleImageSMS = value; }
        }

        public static Image CircleImageBroadcast
        {
            get { return m_circleImageBroadcast; }
            set { m_circleImageBroadcast = value; }
        }

        public static Image CircleImageAutoRun
        {
            get { return m_circleImageAutoRun; }
            set { m_circleImageAutoRun = value; }
        }

        private static Image imgOut = null;
        private static Image imgInNormal = null;
        private static Image imgInSkipped = null;
        private static Image imgInProcessing = null;
        private static Image imgInProcessed = null;
        private static Image imgInWaiting = null;
        private static Image imgSelect = null;

        public ShapeInternal(Section sectionParent)
            : base(sectionParent)
        {
            if (m_painter == null)
            {
                if (imgOut == null)
                    imgOut = global::Sections.Properties.Resources.Internal_OUT;
                if (imgInNormal == null)
                    imgInNormal = global::Sections.Properties.Resources.Internal_IN;
                if( imgInSkipped == null)
                    imgInSkipped = global::Sections.Properties.Resources.Internal_IN_Skipped;
                if( imgInProcessing == null)
                    imgInProcessing = global::Sections.Properties.Resources.Internal_IN_Processing;
                if( imgInProcessed == null)
                    imgInProcessed = global::Sections.Properties.Resources.Internal_IN_Processed;
                if( imgInWaiting == null)
                    imgInWaiting = global::Sections.Properties.Resources.Internal_IN_Waiting;
                if (imgSelect == null)
                    imgSelect = global::Sections.Properties.Resources.Internal_OUT_red;

                m_painter = new ImagePainter(imgInNormal, imgInSkipped, imgInProcessing, imgInProcessed, imgInWaiting, imgOut, 48, 48, 5, 5);

                m_painter.ImageSelected = imgSelect;
            }

            if (m_circleImageSMS == null)
                m_circleImageSMS = global::Sections.Properties.Resources.sms_orange;

            if (m_circleImageBroadcast == null)
                m_circleImageBroadcast = global::Sections.Properties.Resources.broadcast_green;

            if (m_circleImageAutoRun == null)
                m_circleImageAutoRun = global::Sections.Properties.Resources.autoRun_blue;

            base.ImagePainter = m_painter;
        }

        protected override bool Paint(Graphics g, float x, float y)
        {
            bool bResult = base.Paint(g, x, y);
            SectionInternal section = (SectionInternal)m_sectionParent;
            SectionDataInternal data = (SectionDataInternal)section.Data;

            if (data.UseMobileApp && data.UseBroadcast)
            {
                DrawCircle(g, m_circleImageSMS, x , y - m_fCircleDiameter / 2, m_fCircleDiameter, m_fCircleDiameter);
                DrawCircle(g, m_circleImageBroadcast, x , y - m_fCircleDiameter / 2, m_fCircleDiameter, m_fCircleDiameter);
            }
            else if (data.UseMobileApp)
                DrawCircle(g, m_circleImageSMS, x , y - m_fCircleDiameter / 2, m_fCircleDiameter, m_fCircleDiameter);
            else if (data.UseBroadcast)
                DrawCircle(g, m_circleImageBroadcast, x , y - m_fCircleDiameter / 2, m_fCircleDiameter, m_fCircleDiameter);

            if (data.AutoRun)
                DrawCircle(g, m_circleImageAutoRun, x, y + m_fHeight - m_fCircleDiameter / 2, m_fCircleDiameter, m_fCircleDiameter);

            return bResult;
        }

        protected override bool DrawImage(Graphics g, float x, float y)
        {
            if (m_imgPainter != null)
            {
                if (!m_imgPainter.Draw(g, x, y, m_fWidth, m_fHeight, Status))
                    return false;
            }
            SectionInternal section = (SectionInternal)m_sectionParent;
            SectionDataInternal data = (SectionDataInternal)section.Data;

            if (data.UseMobileApp && data.UseBroadcast)
            {
                DrawCircle(g, m_circleImageSMS, x + m_fWidth - m_fCircleDiameter * 2 + 5, y - m_fCircleDiameter / 2, m_fCircleDiameter, m_fCircleDiameter);
                DrawCircle(g, m_circleImageBroadcast, x + m_fWidth - m_fCircleDiameter + 5, y - m_fCircleDiameter / 2, m_fCircleDiameter, m_fCircleDiameter);
            }
            else if (data.UseMobileApp)
                DrawCircle(g, m_circleImageSMS, x + m_fWidth - m_fCircleDiameter + 5, y - m_fCircleDiameter / 2, m_fCircleDiameter, m_fCircleDiameter);
            else if (data.UseBroadcast)
                DrawCircle(g, m_circleImageBroadcast, x + m_fWidth - m_fCircleDiameter + 5, y - m_fCircleDiameter / 2, m_fCircleDiameter, m_fCircleDiameter);
            /*if (m_circleImage != null)
            {
                g.DrawImage(m_circleImage, x + m_fWidth - m_fCircleDiameter + 5, y - m_fCircleDiameter / 2, m_fCircleDiameter, m_fCircleDiameter);
            }
            else
                return false;*/

            return true;
        }

        private void DrawCircle(Graphics g, Image img, float x, float y, float width, float height)
        {
            if (img != null)
                g.DrawImage(img, x, y, width, height);
        }
    }
}
