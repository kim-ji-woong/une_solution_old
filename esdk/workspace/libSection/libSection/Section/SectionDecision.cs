using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;

namespace Sections
{
    public class SectionDecision : Section
    {

        private static Image imgOut = null;
        private static Image imgInNormal = null;
        private static Image imgInSkipped = null;
        private static Image imgInProcessing = null;
        private static Image imgInProcessed = null;
        private static Image imgInWaiting = null;
        private static Image imgSelect = null;

        private static float m_fWidth = 100;
        private static float m_fHeight = 100;
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

        public SectionDecision(PanelSection ctrlParent)
            : base(ctrlParent)
        {
            InitShape();
        }

        public SectionDecision(PanelSection ctrlParent, float x, float y)
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

            arrBoundary.Add(new PointF(m_fWidth / 2, 0));
            arrBoundary.Add(new PointF(m_fWidth, m_fHeight / 2));
            arrBoundary.Add(new PointF(m_fWidth / 2, m_fHeight));
            arrBoundary.Add(new PointF(0, m_fHeight / 2));

            return arrBoundary;
        }

        public override void MakeData(string strStepName, string strTeamName)
        {
            m_data.SetDefaultID(strStepName, strTeamName);
        }

        public override Section Clone(PanelSection ctrlParent)
        {
            SectionDecision section = new SectionDecision(ctrlParent, m_posMgr.Position.X, m_posMgr.Position.Y);
            section.m_sizeMgr.RectSize = this.m_sizeMgr.RectSize;

            section.m_strText = this.m_strText;
            section.m_strSectionName = this.m_strSectionName;

            SectionDataDecision dataTrg = (SectionDataDecision)section.Data;
            SectionDataDecision dataSrc = (SectionDataDecision)this.Data;

            System.Windows.Forms.TabPage tabPage = (System.Windows.Forms.TabPage)ctrlParent.Parent;
            if (tabPage == null)
                return section;

            //string strComponentID = tabPage.Text + dataSrc.ComponentID.Substring(dataSrc.ComponentID.IndexOf('_'));
            //dataTrg.ComponentID = strComponentID;

            //if (strComponentID != dataTrg.ComponentID)
           //     return null;

            string szTeamName = ctrlParent.TeamName;
            dataTrg.SetDefaultID(tabPage.Text, szTeamName);            
            dataTrg.Title = dataSrc.Title;

            dataTrg.TextHorizontalAlign = dataSrc.TextHorizontalAlign;
            dataTrg.TextVerticalAlign = dataSrc.TextVerticalAlign;

            dataTrg.Expression = dataSrc.Expression;
            dataTrg.ShowExpression = dataSrc.ShowExpression;
            dataTrg.ShowTempExpression = dataSrc.ShowTempExpression;

            return section;
        }

        private void InitShape()
        {
            m_data = new SectionDataDecision();
            m_data.Owner = this;
            ArrayList arrBoundary = GetDefaultBoundary();
            SetBoundary(arrBoundary);

            if (m_painter == null)
            {
                if( imgOut == null)
                    imgOut = global::Sections.Properties.Resources.Decision_OUT;
                if( imgInNormal == null)
                    imgInNormal = global::Sections.Properties.Resources.Decision_IN;
                if( imgInSkipped == null)
                    imgInSkipped = global::Sections.Properties.Resources.Decision_IN_Skipped;
                if( imgInProcessing == null)
                    imgInProcessing = global::Sections.Properties.Resources.Decision_IN_Processing;
                if( imgInProcessed == null)
                    imgInProcessed = global::Sections.Properties.Resources.Decision_IN_Processed;
                if( imgInWaiting == null)
                    imgInWaiting = global::Sections.Properties.Resources.Decision_IN_Waiting;
                if( imgSelect == null)
                    imgSelect = global::Sections.Properties.Resources.Decision_OUT_red;
                m_painter = new ImagePainter(imgInNormal, imgInSkipped, imgInProcessing, imgInProcessed, imgInWaiting, imgOut, 53, 55, 8, 8);

                m_painter.ImageSelected = imgSelect;
            }

            m_shape.ImagePainter = m_painter;

            // String Format
            AdjustStringFormat();
        }

        public override void DrawText(Graphics g, PointF ptCurrent)
        {
            float xMB = m_editBox.GetCoord(EditBox.CoordType.X_MIDDLE);
            float yMB = m_editBox.GetCoord(EditBox.CoordType.Y_BOTTOM);

            float fFontHeight = TEXT_FONT.Size;

            float fWidth = m_shape.GetSize(true) - 6;
            float fHeight = m_shape.GetSize(false) - 6;
            float x = ptCurrent.X + 3;
            float y = ptCurrent.Y + 6;

            string szDisplayText = m_strText;
            if (m_data != null)
            {
                if (m_data.ShowTempExpression == true)
                    szDisplayText = m_data.Expression;
            }

            if (m_ctrlParent.VisibleSectionNumber && m_data.SectionNumber > 0)
                szDisplayText = m_data.SectionNumber.ToString() + ". " + szDisplayText;

            g.DrawString(szDisplayText, TEXT_FONT, m_brushText, new RectangleF(x, y, fWidth, fHeight), m_textFormat);
        }

        public override ComponentType GetComponentType()
        {
            return ComponentType.DECISION;
        }
    }
}
