using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPManager
{
	public class LevelClipboard
	{
		protected static LevelClipboard m_Instance = null;
        public static LevelClipboard Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new LevelClipboard();
                return m_Instance;
            }
        }

		protected LevelClipboard()            
        {
        }


		private ActionStepTabPage m_tabPageCopySrc = null;

		public bool IsContainsData
		{
			get { return (m_tabPageCopySrc != null); }
		}

		public void Cancel()
		{
			m_tabPageCopySrc = null;
		}

		public void CopyTab(ActionStepTabPage page)
		{
			m_tabPageCopySrc = page;
		}

		public bool PasteTab(ActionStepTabPage page)
		{
			if (m_tabPageCopySrc == null || page == null)
				return false;

			if (m_tabPageCopySrc == page)
			{
				UnE.Utility.UMessageBoxRibbon.Show(FormMain.Instance, "같은 탭끼리는 복사할 수 없습니다.", "복사 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}

			string szMessage = string.Format("기존 [{0}] 단계에 포함된 모든 컴포넌트가 삭제되며, [{1}] 단계의 컴포넌트가 복사됩니다.\n계속하시겠습니까?",  page.Text, m_tabPageCopySrc.Text);

			if (UnE.Utility.UMessageBoxRibbon.Show(FormMain.Instance, szMessage, "붙여넣기", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				UndoRedoManager.Instance.SaveSnapshot("탭 붙여넣기");

				CopyTab(page, m_tabPageCopySrc);
				page.Refresh();
			}
			m_tabPageCopySrc = null;
			return true;
		}

		private bool CopyTab(ActionStepTabPage pageTrg, ActionStepTabPage pageSrc)
		{
			Type type = typeof(Sections.PanelSectionEx);
			ArrayList arrSectionsTrg = new ArrayList();
			ArrayList arrSectionsSrc = new ArrayList();
			linkFromDic.Clear();
			foreach (Control ctrl in pageSrc.Controls)
			{
				if (ctrl.GetType() == type)
				{
					Sections.PanelSectionEx panelSrc = (Sections.PanelSectionEx)ctrl;
					Sections.PanelSectionEx panelTrg = pageTrg.FindPanel(panelSrc.TeamID, panelSrc.TeamType);

					if (panelTrg == null)
					{
						string strError = string.Format("{0}탭에 {1} 패널이 존재하지 않습니다.", pageTrg.Text, panelSrc.TeamName);
						UnE.Utility.UMessageBoxRibbon.Show(strError);
						return false;
					}

					panelTrg.ClearData();
					if (!CopyPanel(arrSectionsTrg, arrSectionsSrc, panelTrg, panelSrc))
						return false;
				}
			}

			CopyLink(arrSectionsTrg, arrSectionsSrc, pageTrg.Text);
			return true;
		}

		private void CopyLink(ArrayList arrSectionsTrg, ArrayList arrSectionsSrc, string strActionStepName)
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
					m_tabPageCopySrc = null;
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
