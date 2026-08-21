using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sections;

namespace Sections
{
    public class SectionClipboard
    {
        public enum Action
        {
            Copy = 1,
            Cut = 2,
            Paste = 3,
            None = 4
        }

        protected Action m_CurrentAction = Action.None;
        public SectionClipboard.Action CurrentAction
        {
            get { return m_CurrentAction; }
        }

        protected ArrayList m_editSection = new ArrayList();
        public int EditSectionCount
        {
            get { return m_editSection.Count; }
        }

        protected PanelSection m_OrgPanel = null;

        protected SectionClipboard()
        {
        }

        public virtual void Copy(PanelSection panel)
        {
            if (panel.SelectedSection == null &&  panel.SelectedSectionList.Count == 0)
                return;

            m_CurrentAction = Action.Copy;

            m_editSection.Clear();
            m_editSection.AddRange(panel.SelectedSectionList);
            if(panel.SelectedSection != null && !m_editSection.Contains(panel.SelectedSection))
            {
                m_editSection.Add(panel.SelectedSection);
            }

            m_OrgPanel = panel;
        }

        public virtual void Cut(PanelSection panel)
        {
            if (panel.SelectedSection == null && panel.SelectedSectionList.Count == 0)
                return;

            Copy(panel);
            m_CurrentAction = Action.Cut;
            panel.Delete();

            m_OrgPanel = panel;
        }

        public virtual void Paste(PanelSection panel)
        {
            m_CurrentAction = Action.Paste;
            CopySections(panel);

            if (m_OrgPanel != null)
            {
                m_OrgPanel.ClearSelection();
                m_OrgPanel.Refresh();
            }

            m_OrgPanel = null;
        }

		public virtual void Paste(PanelSection panel, System.Drawing.PointF mvPt)
		{
			m_CurrentAction = Action.Paste;
			CopySections(panel, mvPt);

			if (m_OrgPanel != null)
			{
				m_OrgPanel.ClearSelection();
				m_OrgPanel.Refresh();
			}

			m_OrgPanel = null;
		}

        public virtual void Canel()
        {
            if (m_CurrentAction == Action.Copy)
            {

            }
            else if (m_CurrentAction == Action.Cut)
            {
                //UndoRedoManager.Instance.Undo();
            }
            m_OrgPanel = null;
            m_editSection.Clear();
            m_CurrentAction = Action.None;
        }


        private Dictionary<Section, Section> linkFromDic = new Dictionary<Section, Section>();
        protected bool CopySections(Sections.PanelSection pageTrg)
        {                      
            linkFromDic.Clear();

            ArrayList arrSectionsTrg = new ArrayList();
            ArrayList arrSectionsSrc = new ArrayList();
            ArrayList arArrowList = new ArrayList();

            int nBeginCount = arrSectionsTrg.Count;

            // Section 복사
            foreach (Sections.Section section in m_editSection)
            {
                Sections.Section sectionTrg = section.Clone(pageTrg);
                if (sectionTrg == null)
                {
                    return false;
                }

                sectionTrg.Position = new System.Drawing.PointF(sectionTrg.Position.X + 10.0f, sectionTrg.Position.Y + 10.0f);

                pageTrg.Sections.Add(sectionTrg);

                arrSectionsTrg.Add(sectionTrg);
                arrSectionsSrc.Add(section);

                linkFromDic.Add( section, sectionTrg);

                foreach (Arrow arrow in section.Arrows)
                {
                    if (!arArrowList.Contains(arrow))
                    {
                        arArrowList.Add(arrow);
                    }
                }
            }

            m_editSection.Clear();

            // 복사 대상을 (10,10) 만큼 이동한 Section으로 바꾸어서
            // 연속으로 붙여넣기 할때 계속 다른 위치에 복사되도록 한다.
            if (arrSectionsTrg.Count > 0)
                m_editSection.AddRange(arrSectionsTrg);

            foreach (Arrow arrow in arArrowList)
            {
                ArrowFrom(arrow);
            }            
            return true;
        }


		protected bool CopySections(Sections.PanelSection pageTrg, System.Drawing.PointF mvPt)
		{
			linkFromDic.Clear();

			ArrayList arrSectionsTrg = new ArrayList();
			ArrayList arrSectionsSrc = new ArrayList();
			ArrayList arArrowList = new ArrayList();
			

			float mMinX = float.MaxValue;
			float mMinY = float.MaxValue;
			foreach (Section section in m_editSection)
			{
				if (section.Position.Y < mMinY)
				{
					mMinX = section.Position.X;
					mMinY = section.Position.Y;					
				}
			}

			float dx = mvPt.X - mMinX;
			float dy = mvPt.Y - mMinY;

			int nBeginCount = arrSectionsTrg.Count;

			// Section 복사
			foreach (Sections.Section section in m_editSection)
			{
				Sections.Section sectionTrg = section.Clone(pageTrg);
				if (sectionTrg == null)
				{
					return false;
				}

				System.Drawing.PointF pt = sectionTrg.Position;
				System.Drawing.PointF pt2 = new System.Drawing.PointF(pt.X + dx, pt.Y + dy);
				sectionTrg.Position = pt2;
				
				pageTrg.Sections.Add(sectionTrg);

				arrSectionsTrg.Add(sectionTrg);
				arrSectionsSrc.Add(section);

				linkFromDic.Add(section, sectionTrg);

				foreach (Arrow arrow in section.Arrows)
				{
					if (!arArrowList.Contains(arrow))
					{
						arArrowList.Add(arrow);
					}
				}
			}

			foreach (Arrow arrow in arArrowList)
			{				
				ArrowFrom(arrow, dx, dy);
				arrow.CalcArrowLine();
			}
			return true;
		}

		protected void ArrowFrom(Sections.Arrow arrowSrc, float dx, float dy)
		{
			Sections.Arrow arrowTrg = new Sections.Arrow();

			arrowTrg.BeginLink = FindLinkSection(arrowSrc.BeginLink);
			if (arrowTrg.BeginLink == null)
				return;

			arrowTrg.EndLink = FindLinkSection(arrowSrc.EndLink);
			if (arrowTrg.EndLink == null)
				return;

			arrowTrg.BeginPosition = arrowSrc.BeginPosition;
			arrowTrg.EndPosition = arrowSrc.EndPosition;
			arrowTrg.Text = arrowSrc.Text;

			Sections.Arrow.CopyPoints(arrowTrg, arrowSrc);

			arrowTrg.EndLink.AddArrow(arrowTrg);
			arrowTrg.BeginLink.AddArrow(arrowTrg);


			arrowTrg.EndLink.CalcArrowPositions();

		}


        protected void ArrowFrom(Sections.Arrow arrowSrc)
        {
            Sections.Arrow arrowTrg = new Sections.Arrow();

            arrowTrg.BeginLink = FindLinkSection(arrowSrc.BeginLink);
            if (arrowTrg.BeginLink == null)
                return;

            arrowTrg.EndLink = FindLinkSection(arrowSrc.EndLink);
            if (arrowTrg.EndLink == null)
                return;

            arrowTrg.BeginPosition = arrowSrc.BeginPosition;
            arrowTrg.EndPosition = arrowSrc.EndPosition;
            arrowTrg.Text = arrowSrc.Text;

            Sections.Arrow.CopyPoints(arrowTrg, arrowSrc);

            arrowTrg.EndLink.AddArrow(arrowTrg);
            arrowTrg.BeginLink.AddArrow(arrowTrg);            
        }

        protected Sections.Section FindLinkSection(Section orgSection)
        {
            if(linkFromDic.ContainsKey(orgSection))
            {
                return linkFromDic[orgSection];
            }
            return null;
        }

    }
}
