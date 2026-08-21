using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace Sections
{
    public class ShapeGroup : Shape
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

		protected bool m_bCollapse = true;
		public bool Collapse
		{
			get { return m_bCollapse; }
			set { m_bCollapse = value; }
		}
		protected RectangleF m_Region;

		public ShapeGroup(Section sectionParent)
            : base(sectionParent)
        {
            if (m_painter == null)
            {
                if (imgOut == null)
                    imgOut = global::Sections.Properties.Resources.Process_OUT;
                if (imgInNormal == null)
                    imgInNormal = global::Sections.Properties.Resources.Group_IN;
				if( imgInSkipped == null)
				    imgInSkipped = global::Sections.Properties.Resources.Internal_IN_Skipped;
				if( imgInProcessing == null)
				    imgInProcessing = global::Sections.Properties.Resources.Internal_IN_Processing;
				if( imgInProcessed == null)
				    imgInProcessed = global::Sections.Properties.Resources.Internal_IN_Processed;
				if( imgInWaiting == null)
				    imgInWaiting = global::Sections.Properties.Resources.Internal_IN_Waiting;
				if (imgSelect == null)
					imgSelect = global::Sections.Properties.Resources.Group_IN;

                m_painter = new ImagePainter(imgInNormal, imgInSkipped, imgInProcessing, imgInProcessed, imgInWaiting, imgOut, 48, 48, 5, 5);
				m_painter.ImageOut = imgOut;
                m_painter.ImageSelected = imgSelect;
            }

            if (m_circleImage == null)
            {
                m_circleImage = global::Sections.Properties.Resources.Blackcircle_Internal;
            }

            base.ImagePainter = m_painter;
        }


		private Pen m_penRegion = new Pen(Color.BlueViolet, 3);
        protected override bool DrawImage(Graphics g, float x, float y)
        {
			if (Collapse == true)
			{
				return base.DrawImage(g, x, y);			
			}
			else
			{
				//Debug.WriteLine(m_Region);
				g.DrawRectangle(m_penRegion, m_Region.Location.X, m_Region.Location.Y, m_Region.Width, m_Region.Height);
			}

            return true;
        }

		public void UpdateGroupRegion(RectangleF rectRegion)
		{
			m_Region = rectRegion;
		}
    }
}