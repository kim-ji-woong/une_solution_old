using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;

namespace Sections
{
    public class SectionTransmission : Section
    {
        private static float m_fWidth = 150;
        private static float m_fHeight = 80;
        private static PointF[] m_arrDefaultShape = null;

        private static Size m_Size = new Size(150, 80);
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

        public SectionTransmission(PanelSection ctrlParent)
            : base(ctrlParent)
        {
            m_ctrlParent = ctrlParent;

            m_posMgr = new PositionManager(this, m_shape, m_btnScroll, m_editBox);
            m_sizeMgr = new SizeManager(m_editBox, m_shape, m_posMgr);

            InitShape();
        }

        public SectionTransmission(PanelSection ctrlParent, float x, float y)
            : base(ctrlParent, x, y)
        {
            m_ctrlParent = ctrlParent;

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
            ArrayList arrBoundary = new ArrayList();

            // Bezier Curve를 그리기 위한 기준점 설정
            UnE.Geometry.Vertex2D[] arrCurvePoints = new UnE.Geometry.Vertex2D[4];

            arrCurvePoints[0] = new UnE.Geometry.Vertex2D(0, 0);
            arrCurvePoints[1] = new UnE.Geometry.Vertex2D(m_fWidth / 3, m_fWidth * 0.2);
            arrCurvePoints[2] = new UnE.Geometry.Vertex2D(m_fWidth * 2 / 3, -m_fWidth * 0.2);
            arrCurvePoints[3] = new UnE.Geometry.Vertex2D(m_fWidth, 0);
            ////////////////////////////////////////////////////////////////

            // Bezier Curve 얻어오기
            int nResultCount = 100;
            UnE.Geometry.Vertex2D[] arrResultPoints = new UnE.Geometry.Vertex2D[nResultCount];

            UnE.Geometry.BezierCurve2D bezier = new UnE.Geometry.BezierCurve2D();

            if (!bezier.Calc(arrCurvePoints, arrCurvePoints.Count(), arrResultPoints, nResultCount))
                return arrBoundary;
            ////////////////////////////////////////////////////////////////

            // Boundary Vertex 설정
            for (int i=0;i<nResultCount;i++)
            {
                UnE.Geometry.Vertex2D vertex = arrResultPoints[i];
                arrBoundary.Add(new PointF((float)vertex.x, (float)vertex.y));
            }

            for (int i=nResultCount-1;i>=0;i--)
            {
                UnE.Geometry.Vertex2D vertex = arrResultPoints[i];
                arrBoundary.Add(new PointF((float)vertex.x, (float)vertex.y + m_fHeight));
            }
            ////////////////////////////////////////////////////////////////

            return arrBoundary;
        }

        public override void MakeData(string strStepName, string strTeamName)
        {
            m_data.SetDefaultID(strStepName, strTeamName);
        }

        public override Section Clone(PanelSection ctrlParent)
        {
            SectionTransmission section = new SectionTransmission(ctrlParent, m_posMgr.Position.X, m_posMgr.Position.Y);
            section.m_sizeMgr.RectSize = this.m_sizeMgr.RectSize;

            section.m_strText = this.m_strText;
            section.m_strSectionName = this.m_strSectionName;

            SectionDataTransmission dataTrg = (SectionDataTransmission)section.Data;
            SectionDataTransmission dataSrc = (SectionDataTransmission)this.Data;

            System.Windows.Forms.TabPage tabPage = (System.Windows.Forms.TabPage)ctrlParent.Parent;
            if (tabPage == null)
                return section;

            //string strComponentID = tabPage.Text + dataSrc.ComponentID.Substring(dataSrc.ComponentID.IndexOf('_'));
            //dataTrg.ComponentID = strComponentID;

            //if (strComponentID != dataTrg.ComponentID)
            //    return null;

            string szTeamName = ctrlParent.TeamName;
            dataTrg.SetDefaultID(tabPage.Text, szTeamName);

            dataTrg.TextHorizontalAlign = dataSrc.TextHorizontalAlign;
            dataTrg.TextVerticalAlign = dataSrc.TextVerticalAlign;

            dataTrg.Title = dataSrc.Title;

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
            m_data = new SectionDataTransmission();
            m_data.Owner = this;
            ArrayList arrBoundary = GetDefaultBoundary();
            SetBoundary(arrBoundary);

            if (m_painter == null)
            {
                if( imgOut == null)
                    imgOut = global::Sections.Properties.Resources.Transmission_OUT;
                if( imgInNormal == null)
                    imgInNormal = global::Sections.Properties.Resources.Transmission_IN;
                if( imgInSkipped == null)
                    imgInSkipped = global::Sections.Properties.Resources.Transmission_IN_Skipped;
                if( imgInProcessing == null)
                    imgInProcessing = global::Sections.Properties.Resources.Transmission_IN_Processing;
                if( imgInProcessed == null)
                    imgInProcessed = global::Sections.Properties.Resources.Transmission_IN_Processed;
                if( imgInWaiting == null)
                    imgInWaiting = global::Sections.Properties.Resources.Transmission_IN_Waiting;
                if (imgSelect == null)
                    imgSelect = global::Sections.Properties.Resources.Transmission_OUT_red;
                m_painter = new ImagePainter(imgInNormal, imgInSkipped, imgInProcessing, imgInProcessed, imgInWaiting, imgOut, 50, 50, 5, 5);

                m_painter.ImageSelected = imgSelect;
            }

            m_shape.ImagePainter = m_painter;

            AdjustStringFormat();
        }

        public override ComponentType GetComponentType()
        {
            return ComponentType.TRANSMISSION;
        }
    }
}
