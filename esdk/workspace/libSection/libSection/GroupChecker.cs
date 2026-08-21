using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sections
{
	public class SectionGroupCheker
	{
		private PanelSection m_ctrParent = null;
		private SectionGroup m_groupSection = null;
		private string m_szLastErrorMsg = "";


		private ArrayList m_arSections = new ArrayList();
		private ArrayList m_arNewArrow = new ArrayList();
		private ArrayList m_arHideArrow = new ArrayList();

		public SectionGroupCheker(PanelSection panel)
		{
			m_ctrParent = panel;			
		}

		public bool Check(ArrayList arSections)
		{
			if (arSections == null)
			{
				m_szLastErrorMsg = "2개 이상의 Section을 먼저 선택 하세요.";
				return false;
			}

			if (arSections.Count <= 1)
			{
				m_szLastErrorMsg = "2개 이상의 Section을 먼저 선택 하세요.";
				return false;
			}

			m_arSections = arSections;
			
			ArrayList arStdalone = FindStandaloneSection();
			if (arStdalone.Count != 0)
			{
				m_szLastErrorMsg = "연결된 컴포넌트로만 그룹생성이 가능합니다.";
				return false;
			}



			return true;
		}

		public void MakeGroupArrow(SectionGroup section)
		{
			m_groupSection = section;

			m_arNewArrow.Clear();
			m_arHideArrow.Clear();
			
			ArrayList arGroupInArrows = new ArrayList();
			ArrayList arGroupOutArrows = new ArrayList();
			FindGroupArrow(ref arGroupInArrows, ref arGroupOutArrows);

			m_arNewArrow.AddRange(arGroupInArrows);
			m_arNewArrow.AddRange(arGroupOutArrows);

			MakeGroupArrow(arGroupInArrows, arGroupOutArrows);			
		}

		private ArrayList FindStandaloneSection()
		{
			ArrayList standaloneSelction = new ArrayList();
			foreach (Section section in m_arSections)
			{
				ArrayList arArrow = section.Arrows;
				if (arArrow == null || arArrow.Count == 0)
				{
					standaloneSelction.Add(section);
					continue;
				}

				bool bConnected = false;
				foreach (Arrow arrow in arArrow)
				{
					Section beginSection = arrow.BeginLink;
					Section endSection = arrow.EndLink;
					if (beginSection == section)
					{
						if (m_arSections.Contains(endSection))
						{
							bConnected = true;
							break;
						}
					}

					if( endSection == section)
					{
						if (m_arSections.Contains(beginSection))
						{
							bConnected = true;
							break;
						}
					}
				}
				if (bConnected == false)
				{
					standaloneSelction.Add(section);
				}
			}
			return standaloneSelction;
		}

		private void MakeGroupArrow(ArrayList inArrows, ArrayList outArrows)
		{
		    foreach(Arrow arrow in inArrows)
		    {
		        Arrow newArrow = new Arrow();
				Section beginSection = arrow.BeginLink;
				
		        newArrow.BeginLink = arrow.BeginLink;
		        newArrow.BeginPosition = arrow.BeginPosition;
		        newArrow.EndLink = m_groupSection;
		        newArrow.EndPosition = arrow.EndPosition;
				newArrow.Text = arrow.Text;

				newArrow.BeginLink.AddArrow(newArrow);
		        m_groupSection.AddArrow(newArrow);
				newArrow.CalcArrowLine();

				//if (beginSection.GetComponentType() == Section.ComponentType.GROUP)
				//{
				//	arrow.EndLink.RemoveArrow(arrow);
				//	m_groupSection.RemoveArrow(arrow);
				//}				
		    }

			foreach (Arrow arrow in outArrows)
			{
				Arrow newArrow = new Arrow();
				Section endSection = arrow.EndLink;
					
				newArrow.EndLink = arrow.EndLink;
				newArrow.EndPosition = arrow.EndPosition;
				newArrow.BeginLink = m_groupSection;
				newArrow.BeginPosition = arrow.BeginPosition;
				newArrow.Text = arrow.Text;

				newArrow.EndLink.AddArrow(newArrow);
				m_groupSection.AddArrow(newArrow);
				newArrow.CalcArrowLine();
				
				//if (endSection.GetComponentType() == Section.ComponentType.GROUP)
				//{
				//	m_groupSection.RemoveArrow(arrow);
				//	arrow.BeginLink.RemoveArrow(arrow);
				//}				
			}
		}

		private void FindGroupArrow(ref ArrayList inArrows, ref ArrayList outArrows)
		{

			foreach (Section section in m_arSections)
			{
				ArrayList arArrow = section.Arrows;
				foreach (Arrow arrow in arArrow)
				{
					Section outSection = arrow.BeginLink;
					Section inSection = arrow.EndLink;
					
					if (outSection == section)
					{						
						if (!m_arSections.Contains(inSection))
						{
							outArrows.Add(arrow);
							m_arHideArrow.Add(arrow);
						}
						else
						{
							m_arHideArrow.Add(arrow);
						}												
					}

					if (inSection == section)
					{
						if (!m_arSections.Contains(outSection))
						{
							inArrows.Add(arrow);
							m_arHideArrow.Add(arrow);
						}
						else
						{
							m_arHideArrow.Add(arrow);
						}
					}					
				}
			}
		}
			

		public string GetLastErrorMessage()
		{
			return m_szLastErrorMsg;
		}
	
		public ArrayList GetHideArraw()
		{
			return m_arHideArrow;
		}
		
		public ArrayList GetNewArrow()
		{
			return m_arNewArrow;
		}
	}
}
