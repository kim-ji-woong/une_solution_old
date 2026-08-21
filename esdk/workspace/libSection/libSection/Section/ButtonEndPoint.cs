using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;

namespace Sections
{
    public class ButtonEndPoint : SectionButton
    { 
        
        private static float m_fWidth = 100;
        private static float m_fHeight = 40;
        //private static int m_nCircleVertexCount = 60;
        private static PointF[] m_arrDefaultShape = null;

        private static Size m_Size = new Size(100, 40);
        public static Size DefaultSize
        {
            get { return m_Size; }
            set
            {
                if (value == null)
                    return;
                m_Size = value;
                m_fWidth = value.Width;
                m_fHeight = value.Height;
            }
        }

        public ButtonEndPoint(PanelSection ctrlParent)
        {
            Section = new SectionEndPoint(ctrlParent);
            m_ctrlParent = ctrlParent;

            m_shape = new ShapeEndPoint(Section);
            m_posMgr = new PositionManager(Section, m_shape, m_btnScroll, m_editBox);
            m_sizeMgr = new SizeManager(m_editBox, m_shape, m_posMgr);

            InitShape();

            this.Notify(true);
        }

        public ButtonEndPoint(PanelSection ctrlParent, float x, float y)
        {
            Section = new SectionEndPoint(ctrlParent);
            m_ctrlParent = ctrlParent;

            m_shape = new ShapeEndPoint(Section);
            m_posMgr = new PositionManager(Section, m_shape, m_btnScroll, m_editBox, x, y);
            m_sizeMgr = new SizeManager(m_editBox, m_shape, m_posMgr);

            InitShape();

            this.Notify(true);
        }

        public static PointF[] GetDefaultShape()
        {
            if (m_arrDefaultShape != null)
                return m_arrDefaultShape;

            ArrayList arrBoundary = GetDefaultBoundary();

            int nPointCount = arrBoundary.Count;
            m_arrDefaultShape = new PointF[nPointCount];

            for (int i = 0; i < nPointCount; i++)
            {
                m_arrDefaultShape[i] = (PointF)arrBoundary[i];
            }

            return m_arrDefaultShape;
        }

        private static ArrayList GetDefaultBoundary()
        {
            ArrayList arrBoundary = new ArrayList();

            arrBoundary.Add(new PointF(0, 0));
            arrBoundary.Add(new PointF(m_fWidth, 0));
            arrBoundary.Add(new PointF(m_fWidth, m_fHeight));
            arrBoundary.Add(new PointF(0, m_fHeight));

            return arrBoundary;
        }

        public override void MakeData(string strStepName, string strTeamName)
        {
            m_data.SetDefaultID(strStepName, strTeamName);
        }

        public override Section Clone(PanelSection ctrlParent)
        {
            return null;
        }

        private void InitShape()
        {
            m_data = new SectionDataEndPoint();
            m_data.Owner = Section;

            ArrayList arrBoundary = GetDefaultBoundary();
            SetBoundary(arrBoundary);

            AdjustStringFormat();
        }

        public override ComponentType GetComponentType()
        {
            return ComponentType.ENDPOINT;
        }

        protected override void DrawCompleteCount(Graphics g)
        {
        }
    }
}
