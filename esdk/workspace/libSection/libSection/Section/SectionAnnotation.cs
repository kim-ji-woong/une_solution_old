using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;

namespace Sections
{
    public class SectionAnnotation : Section
    {
        private static float m_fWidth = 200;
        private static float m_fHeight = 82;
        private static PointF[] m_arrDefaultShape = null;

        private static Size m_Size = new Size(200, 82);

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


        public SectionAnnotation(PanelSection ctrlParent)
        {
            m_ctrlParent = ctrlParent;

            m_shape = new ShapeAnnotation(this);
            m_posMgr = new PositionManager(this, m_shape, m_btnScroll, m_editBox);
            m_sizeMgr = new SizeManager(m_editBox, m_shape, m_posMgr);

            InitShape();
        }

        public SectionAnnotation(PanelSection ctrlParent, float x, float y)
        {
            m_ctrlParent = ctrlParent;

            m_shape = new ShapeAnnotation(this);
            m_posMgr = new PositionManager(this, m_shape, m_btnScroll, m_editBox, x, y);
            m_sizeMgr = new SizeManager(m_editBox, m_shape, m_posMgr);

            InitShape();
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
            ArrayList arrBoundary= new ArrayList();

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
            SectionAnnotation section = new SectionAnnotation(ctrlParent, m_posMgr.Position.X, m_posMgr.Position.Y);
            section.m_sizeMgr.RectSize = this.m_sizeMgr.RectSize;

            section.m_strText = this.m_strText;
            section.m_strSectionName = this.m_strSectionName;

            SectionDataAnnotation dataTrg = (SectionDataAnnotation)section.Data;
            SectionDataAnnotation dataSrc = (SectionDataAnnotation)this.Data;

            System.Windows.Forms.TabPage tabPage = (System.Windows.Forms.TabPage)ctrlParent.Parent;
            if (tabPage == null)
                return section;

            string szTeamName = ctrlParent.TeamName;
            
            dataTrg.SetDefaultID(tabPage.Text, szTeamName);
            dataTrg.TextHorizontalAlign = dataSrc.TextHorizontalAlign;
            dataTrg.TextVerticalAlign = dataSrc.TextVerticalAlign;
            dataTrg.Title = dataSrc.Title;

            return section;
        }

        private void InitShape()
        {
            m_data = new SectionDataAnnotation();
            m_data.Owner = this;
            ArrayList arrBoundary = GetDefaultBoundary();
            SetBoundary(arrBoundary);

            AdjustStringFormat();
        }

        public override ComponentType GetComponentType()
        {
            return ComponentType.ANNOTATION;
        }

        protected override void DrawCompleteCount(Graphics g)
        {
        }
    }
}
