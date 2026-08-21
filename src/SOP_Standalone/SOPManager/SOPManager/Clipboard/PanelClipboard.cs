using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sections;

namespace SOPManager
{
	public class PanelClipboard
	{
		protected static PanelClipboard m_Instance = null;
		public static PanelClipboard Instance
		{
			get
			{
				if (m_Instance == null)
					m_Instance = new PanelClipboard();
				return m_Instance;
			}
		}

		protected PanelClipboard()
		{
		}


		private PanelSectionEx m_tabPaneCopySrc = null;

		public bool IsContainsData
		{
			get { return (m_tabPaneCopySrc != null); }
		}

		public void Cancel()
		{
			m_tabPaneCopySrc = null;
		}

		public void CopyPanel(PanelSectionEx page)
		{
			m_tabPaneCopySrc = page;
		}

		public bool PastePanel(PanelSectionEx page)
		{
			if (m_tabPaneCopySrc == null || page == null)
				return false;

			if (m_tabPaneCopySrc == page)
			{
				UnE.Utility.UMessageBoxRibbon.Show("같은 패널끼리는 복사할 수 없습니다.", "복사 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}

			string szMessage = string.Format("기존 [{0}] 패널에 포함된 모든 컴포넌트가 삭제되며, [{1}] 패널의 컴포넌트가 복사됩니다.\n계속하시겠습니까?", page.TeamName, m_tabPaneCopySrc.TeamName);

			if (UnE.Utility.UMessageBoxRibbon.Show(szMessage, "붙여넣기", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				UndoRedoManager.Instance.SaveSnapshot("패널 붙여넣기");

				CopyPanel(page, m_tabPaneCopySrc);
				page.Refresh();
			}

			m_tabPaneCopySrc = null;
			
			return true;
		}

		private bool CopyPanel(PanelSectionEx pageTrg, PanelSectionEx pageSrc)
		{
			ArrayList arrSectionsTrg = new ArrayList();
			ArrayList arrSectionsSrc = new ArrayList();
			
			linkFromDic.Clear();
			
			pageTrg.ClearData();
			if (!CopyPanel(arrSectionsTrg, arrSectionsSrc, pageTrg, pageSrc))
				return false;			

			CopyLink(arrSectionsTrg, arrSectionsSrc);
			return true;
		}

		private void CopyLink(ArrayList arrSectionsTrg, ArrayList arrSectionsSrc)
		{
			int nSectionCount = arrSectionsTrg.Count;

			for (int i = 0; i < nSectionCount; i++)
			{
				Sections.Section section = (Sections.Section)arrSectionsTrg[i];

				if (section.GetComponentType() == Sections.Section.ComponentType.LINK)
				{
					Sections.SectionDataLink data = (Sections.SectionDataLink)section.Data;
					Sections.Section sectionSrc = (Sections.Section)arrSectionsSrc[i];
					Sections.SectionDataLink dataSrc = (Sections.SectionDataLink)sectionSrc.Data;
					data.LinkedSection = FindLinkSection(dataSrc.LinkedSection);
				}
			}
		}

		private bool CopyPanel(ArrayList arrSectionsTrg, ArrayList arrSectionsSrc, Sections.PanelSectionEx pageTrg, Sections.PanelSectionEx pageSrc)
		{
			int nBeginCount = arrSectionsTrg.Count;

			ArrayList arArrowList = new ArrayList();

			// Section 복사
			foreach (Sections.Section section in pageSrc.Sections)
			{
				Sections.Section sectionTrg = section.Clone(pageTrg);
				if (sectionTrg == null)
				{
					UnE.Utility.UMessageBoxRibbon.Show("이미 같은 데이터가 존재합니다.\r\n복사를 계속할 수 없습니다.", "복사 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					m_tabPaneCopySrc = null;
					return false;
				}

				pageTrg.Sections.Add(sectionTrg);

				arrSectionsTrg.Add(sectionTrg);
				arrSectionsSrc.Add(section);

				linkFromDic.Add(section, sectionTrg);

				foreach (Sections.Arrow arrow in section.Arrows)
				{
					if (!arArrowList.Contains(arrow))
					{
						arArrowList.Add(arrow);
					}
				}
			}

			// Arrow 복사
			foreach (Sections.Arrow arrow in arArrowList)
			{
				ArrowFrom(arrow);
			}
			return true;
		}

		private Dictionary<Sections.Section, Sections.Section> linkFromDic = new Dictionary<Sections.Section, Sections.Section>();

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

		protected Sections.Section FindLinkSection(Sections.Section orgSection)
		{
			if (linkFromDic.ContainsKey(orgSection))
			{
				return linkFromDic[orgSection];
			}
			return null;
		}

	}
}
