using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;

namespace Sections
{
    public class SectionExternal : Section
    {
        private static float m_fWidth = 150;
        private static float m_fHeightBig = 82;
        private static float m_fHeightSmall = 62;
        private static PointF[] m_arrDefaultShape = null;

        private static Size m_Size = new Size(150, 82);
        public static Size DefaultSize
        {
            get { return m_Size; }
            set
            {
                if (value == null)
                    return;
                m_Size = value;
                m_fWidth = value.Width;
                m_fHeightBig = value.Height;
            }
        }

        public SectionExternal(PanelSection ctrlParent)
        {
            m_ctrlParent = ctrlParent;
            m_editBox = new EditBox(this);

            m_shape = new ShapeExternal(this);
            m_posMgr = new PositionManager(this, m_shape, m_btnScroll, m_editBox);
            m_sizeMgr = new SizeManager(m_editBox, m_shape, m_posMgr);

            InitShape();
        }

        public SectionExternal(PanelSection ctrlParent, float x, float y)
        {
            m_ctrlParent = ctrlParent;
            m_editBox = new EditBox(this);

            m_shape = new ShapeExternal(this);
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

            float sub = (m_fHeightBig - m_fHeightSmall) / 2;

            arrBoundary.Add(new PointF(0, 0));
            arrBoundary.Add(new PointF(m_fWidth, sub));
            arrBoundary.Add(new PointF(m_fWidth, m_fHeightBig - sub));
            arrBoundary.Add(new PointF(0, m_fHeightBig));

            return arrBoundary;
        }

        

        public override void MakeData(string strStepName, string strTeamName)
        {
            m_data.SetDefaultID(strStepName, strTeamName);
        }

        public override Section Clone(PanelSection ctrlParent)
        {
            SectionExternal section = new SectionExternal(ctrlParent, m_posMgr.Position.X, m_posMgr.Position.Y);
            section.m_sizeMgr.RectSize = this.m_sizeMgr.RectSize;

            section.m_strText = this.m_strText;
            section.m_strSectionName = this.m_strSectionName;

            SectionDataExternal dataTrg = (SectionDataExternal)section.Data;
            SectionDataExternal dataSrc = (SectionDataExternal)this.Data;

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
            ExternalTeamDataFrom(dataTrg.FaxReceivers, dataSrc.FaxReceivers);
            dataTrg.SMSMessage = dataSrc.SMSMessage;
            ExternalTeamDataFrom(dataTrg.SMSReceivers, dataSrc.SMSReceivers);
            dataTrg.UseFax = dataSrc.UseFax;
            dataTrg.UseSMS = dataSrc.UseSMS;

            return section;
        }

        private void ExternalTeamDataFrom(ArrayList arrTrg, ArrayList arrTeamData)
        {
            foreach (ExternalTeamData data in arrTeamData)
            {
                ExternalTeamData teamData = new ExternalTeamData(data.TeamID, data.TeamName, data.PhoneNumber, data.FaxNumber);
                arrTrg.Add(teamData);
            }
        }

        private void InitShape()
        {
            m_data = new SectionDataExternal();
            m_data.Owner = this;

            ArrayList arrBoundary = GetDefaultBoundary();
            SetBoundary(arrBoundary);

            AdjustStringFormat();
        }

        public override ComponentType GetComponentType()
        {
            return ComponentType.EXTERNAL;
        }
    }
}
