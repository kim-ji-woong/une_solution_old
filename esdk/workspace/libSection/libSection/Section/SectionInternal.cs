using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;

namespace Sections
{
    public class SectionInternal : Section
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

        public SectionInternal(PanelSection ctrlParent)
        {
            m_ctrlParent = ctrlParent;
            m_editBox = new EditBox(this);

            m_shape = new ShapeInternal(this);
            m_posMgr = new PositionManager(this, m_shape, m_btnScroll, m_editBox);
            m_sizeMgr = new SizeManager(m_editBox, m_shape, m_posMgr);

            InitShape();
        }

        public SectionInternal(PanelSection ctrlParent, float x, float y)
        {
            m_ctrlParent = ctrlParent;
            m_editBox = new EditBox(this);

            m_shape = new ShapeInternal(this);
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

            arrBoundary.Add(new PointF(0, sub));
            arrBoundary.Add(new PointF(m_fWidth, 0));
            arrBoundary.Add(new PointF(m_fWidth, m_fHeightBig));
            arrBoundary.Add(new PointF(0, m_fHeightBig - sub));

            return arrBoundary;
        }

        public override void MakeData(string strStepName, string strTeamName)
        {
            m_data.SetDefaultID(strStepName, strTeamName);
        }

        public override Section Clone(PanelSection ctrlParent)
        {
            SectionInternal section = new SectionInternal(ctrlParent, m_posMgr.Position.X, m_posMgr.Position.Y);
            section.m_sizeMgr.RectSize = this.m_sizeMgr.RectSize;

            section.m_strText = this.m_strText;
            section.m_strSectionName = this.m_strSectionName;

            SectionDataInternal dataTrg = (SectionDataInternal)section.Data;
            SectionDataInternal dataSrc = (SectionDataInternal)this.Data;

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
            dataTrg.UseBroadcast = dataSrc.UseBroadcast;
            dataTrg.UseMobileApp = dataSrc.UseMobileApp;
            dataTrg.UsePopupMessage = dataSrc.UsePopupMessage;

            dataTrg.BroadcastMessage = dataSrc.BroadcastMessage;

            dataTrg.TextHorizontalAlign = dataSrc.TextHorizontalAlign;
            dataTrg.TextVerticalAlign = dataSrc.TextVerticalAlign;

            dataTrg.ShowExpression = dataSrc.ShowExpression;
            dataTrg.ShowMessageBox = dataSrc.ShowMessageBox;
            dataTrg.ShowTempExpression = dataSrc.ShowTempExpression;
            dataTrg.Expression = dataSrc.Expression;

            dataTrg.Commander.Team = dataSrc.Commander.Team;
            dataTrg.Commander.DisplayText = dataSrc.Commander.DisplayText;
            dataTrg.Commander.IsTeamMember = dataSrc.Commander.IsTeamMember;
            dataTrg.Commander.TeamMemberID = dataSrc.Commander.TeamMemberID;

            dataTrg.TeamList = (ArrayList)dataSrc.TeamList.Clone();

            return section;
        }

        private void InitShape()
        {
            m_data = new SectionDataInternal();
            m_data.Owner = this;

            ArrayList arrBoundary = GetDefaultBoundary();
            SetBoundary(arrBoundary);

            AdjustStringFormat();
        }

        public override ComponentType GetComponentType()
        {
            return ComponentType.INTERNAL;
        }
    }
}
