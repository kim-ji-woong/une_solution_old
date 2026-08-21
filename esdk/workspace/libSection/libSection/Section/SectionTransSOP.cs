using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;

namespace Sections
{
    public class SectionTransSOP : Section
    {
        private static float m_fWidth = 100;
        private static float m_fHeight = 120;
        private static float m_fMiddlePosition = 80;
        private static PointF[] m_arrDefaultShape = null;

        private static Size m_Size = new Size(100, 120);
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

        private ImagePainter m_painter = null;

        public ImagePainter ImagePainter
        {
            get { return m_painter; }
            set { m_painter = value; }
        }

        public SectionTransSOP(PanelSection ctrlParent)
            : base(ctrlParent)
        {
            InitShape();
        }

        public SectionTransSOP(PanelSection ctrlParent, float x, float y)
            : base(ctrlParent, x, y)
        {
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
            ArrayList arrBoundary = new ArrayList();

            arrBoundary.Add(new PointF(0, 0));
            arrBoundary.Add(new PointF(m_fWidth, 0));
            arrBoundary.Add(new PointF(m_fWidth, m_fMiddlePosition));
            arrBoundary.Add(new PointF(m_fWidth / 2, m_fHeight));
            arrBoundary.Add(new PointF(0, m_fMiddlePosition));

            return arrBoundary;
        }

        public override void MakeData(string strStepName, string strTeamName)
        {
            m_data.SetDefaultID(strStepName, strTeamName);
        }

        public override Section Clone(PanelSection ctrlParent)
        {
            SectionTransSOP section = new SectionTransSOP(ctrlParent, m_posMgr.Position.X, m_posMgr.Position.Y);
            section.m_sizeMgr.RectSize = this.m_sizeMgr.RectSize;

            section.m_strText = this.m_strText;
            section.m_strSectionName = this.m_strSectionName;

            SectionDataTransSOP dataTrg = (SectionDataTransSOP)section.Data;
            SectionDataTransSOP dataSrc = (SectionDataTransSOP)this.Data;

            System.Windows.Forms.TabPage tabPage = (System.Windows.Forms.TabPage)ctrlParent.Parent;
            if (tabPage == null)
                return section;

            //string strComponentID = tabPage.Text + dataSrc.ComponentID.Substring(dataSrc.ComponentID.IndexOf('_'));
            //dataTrg.ComponentID = strComponentID;

            //if (strComponentID != dataTrg.ComponentID)
            //    return null;
            string szTeamName = ctrlParent.TeamName;
            dataTrg.SetDefaultID(tabPage.Text, szTeamName);      

            dataTrg.Title = dataSrc.Title;
            dataTrg.Description = dataSrc.Description;
            dataTrg.LinkedActionStepID = dataSrc.LinkedActionStepID;

            dataTrg.TextHorizontalAlign = dataSrc.TextHorizontalAlign;
            dataTrg.TextVerticalAlign = dataSrc.TextVerticalAlign;

            return section;
        }

        private static Image imgOut = null;
        private static Image imgInNormal = null;
        private static Image imgInSkipped = null;
        private static Image imgInProcessing = null;
        private static Image imgInProcessed = null;
        private static Image imgInWaiting = null;
        private static Image imgSelect = null;

        private void InitShape()
        {
            m_data = new SectionDataTransSOP();
            m_data.Owner = this;
            ArrayList arrBoundary = GetDefaultBoundary();
            SetBoundary(arrBoundary);

            if (m_painter == null)
            {
                if( imgOut == null)
                    imgOut = global::Sections.Properties.Resources.TRANS_SOP_OUT;

                if(imgInNormal == null)
                    imgInNormal = global::Sections.Properties.Resources.TRANS_SOP_IN;
                if( imgInSkipped == null)
                    imgInSkipped = global::Sections.Properties.Resources.Trans_SOP_IN_Skipped;
                if( imgInProcessing == null)
                    imgInProcessing = global::Sections.Properties.Resources.Trans_SOP_IN_Processing;
                if( imgInProcessed == null)
                    imgInProcessed = global::Sections.Properties.Resources.Trans_SOP_IN_Processed;
                if( imgInWaiting == null)
                    imgInWaiting = global::Sections.Properties.Resources.Trans_SOP_IN_Waiting;
                if (imgSelect == null)
                    imgSelect = global::Sections.Properties.Resources.TRANS_SOP_OUT_red;
                m_painter = new ImagePainter(imgInNormal, imgInSkipped, imgInProcessing, imgInProcessed, imgInWaiting, imgOut, 50, 50, 6, 6);

                m_painter.ImageSelected = imgSelect;
            }

            m_shape.ImagePainter = m_painter;

            AdjustStringFormat();
        }

        public override ComponentType GetComponentType()
        {
            return ComponentType.TRANSSOP;
        }

        // TransSOP에서는 화살표가 시작될 수 없다.
        public override bool ArrowBegin
        {
            get { return false; }
        }
    }
}
