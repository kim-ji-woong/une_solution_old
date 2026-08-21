using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;

namespace Sections
{
    public class SectionLink : Section
    {
        private static float m_fDiameter = 100;
        private static PointF[] m_arrDefaultShape = null;

        private static Size m_Size = new Size(100, 100);
        public static Size DefaultSize
        {
            get { return m_Size; }
            set
            {
                if (value == null)
                    return;
                m_Size = value;
                m_fDiameter = value.Width;
            }
        }

        public SectionLink(PanelSection ctrlParent)
        {
            m_ctrlParent = ctrlParent;

            m_shape = new ShapeLink(this);
            m_posMgr = new PositionManager(this, m_shape, m_btnScroll, m_editBox);
            m_sizeMgr = new SizeManagerLink(m_editBox, m_shape, m_posMgr, ctrlParent);

            InitShape();
        }

        public SectionLink(PanelSection ctrlParent, float x, float y)
        {
            m_ctrlParent = ctrlParent;

            m_shape = new ShapeLink(this);
            m_posMgr = new PositionManager(this, m_shape, m_btnScroll, m_editBox, x, y);
            m_sizeMgr = new SizeManagerLink(m_editBox, m_shape, m_posMgr, ctrlParent);

            InitShape();
        }

        public static PointF[] GetDefaultShape()
        {
            if (m_arrDefaultShape != null)
                return m_arrDefaultShape;

            double dRadius = m_fDiameter / 2.0;
            double centerX = dRadius;
            double centerY = dRadius;

            int nSlice = 100;
            m_arrDefaultShape = new PointF[nSlice + 1];

            double delta = System.Math.PI * 2 / nSlice;

            for (int i = 0; i <= nSlice; i++)
            {
                double dAngle = delta * i;
                double x = centerX - dRadius * System.Math.Sin(dAngle);
                double y = centerY + dRadius * System.Math.Cos(dAngle);

                m_arrDefaultShape[i].X = (float)x;
                m_arrDefaultShape[i].Y = (float)y;
            }

            return m_arrDefaultShape;
        }

        public override void MakeData(string strStepName, string strTeamName)
        {
            m_data.SetDefaultID(strStepName, strTeamName);
        }

        public override Section Clone(PanelSection ctrlParent)
        {
            SectionLink section = new SectionLink(ctrlParent, m_posMgr.Position.X, m_posMgr.Position.Y);
            section.m_sizeMgr.RectSize = this.m_sizeMgr.RectSize;

            section.m_strText = this.m_strText;
            section.m_strSectionName = this.m_strSectionName;

            SectionDataLink dataTrg = (SectionDataLink)section.Data;
            SectionDataLink dataSrc = (SectionDataLink)this.Data;

            System.Windows.Forms.TabPage tabPage = (System.Windows.Forms.TabPage)ctrlParent.Parent;
            if (tabPage == null)
                return section;

            //string strComponentID = tabPage.Text + dataSrc.ComponentID.Substring(dataSrc.ComponentID.IndexOf('_'));
            //dataTrg.ComponentID = strComponentID;

           // if (strComponentID != dataTrg.ComponentID)
            //    return null;
            string szTeamName = ctrlParent.TeamName;
            dataTrg.SetDefaultID(tabPage.Text, szTeamName);

            dataTrg.TextHorizontalAlign = dataSrc.TextHorizontalAlign;
            dataTrg.TextVerticalAlign = dataSrc.TextVerticalAlign;

            dataTrg.Title = dataSrc.Title;
            
            return section;
        }

        private void InitShape()
        {
            m_data = new SectionDataLink();
            m_data.Owner = this;

            ArrayList arrBoundary = new ArrayList();

            arrBoundary.Add(new PointF(0, 0));
            arrBoundary.Add(new PointF(m_fDiameter, 0));
            arrBoundary.Add(new PointF(m_fDiameter, m_fDiameter));
            arrBoundary.Add(new PointF(0, m_fDiameter));

            SetBoundary(arrBoundary);

            AdjustStringFormat();
        }

        public override ComponentType GetComponentType()
        {
            return ComponentType.LINK;
        }

        // Link에서는 화살표가 시작될 수 없다.
        public override bool ArrowBegin
        {
            get { return false; }
        }
    }
}
