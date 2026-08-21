using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;

namespace Sections
{
	public class SectionDataGroup : SectionData
	{
        // Default 문자열을 사용하여 작성된 ID 개수
        protected static Dictionary<string, int> DEFAULT_ID_COUNT = new Dictionary<string, int>();
		
        public static void ClearIDCount()
        {
            DEFAULT_ID_COUNT.Clear();
        }
		
		private ArrayList m_arrComponentList = new ArrayList();
		public ArrayList GroupItems
		{
			get { return m_arrComponentList; }
			set { m_arrComponentList = value; }
		}

        public override void SetDefaultID(string strStepName, string strTeamName)
        {
            MakeDefaultID(strStepName, strTeamName, DEFAULT_ID_COUNT, "Group");
        }

        protected override void AddDefaultID(string strTag, int nTagCount)
        {
            DEFAULT_ID_COUNT[strTag] = nTagCount;
        }

        // nTagCount가 strTag에 대한 최대값이면 최대값을 1만큼 낮춰준다.
        protected override void RemoveMaxDefaultCount(string strTag, int nTagCount)
        {
            if (DEFAULT_ID_COUNT.ContainsKey(strTag))
            {
                if (DEFAULT_ID_COUNT[strTag] == nTagCount)
                    DEFAULT_ID_COUNT[strTag] = nTagCount - 1;
            }
        }

		public void RemoveAllGroupMember()
		{
			SectionGroup group = (SectionGroup)Owner;
			foreach (Section section in GroupItems)
			{
				section.GroupSection = null;
				section.GroupMember = false;
				section.Editable = true;
				section.Movable = true;
				section.Select(false);
				section.Show();
				foreach (Arrow arrow in section.Arrows)
				{
					arrow.Visible = group.CheckArrowVisible(arrow, false);
				}

			}
			GroupItems.Clear();
		}

		public void RemoveGroupMember(Section section)
		{
			SectionGroup group = (SectionGroup)Owner;
			if (GroupItems.Contains(section))
			{
				section.GroupSection = null;
				section.GroupMember = false;
				section.Editable = true;
				section.Movable = true;
				GroupItems.Remove(section);
				section.Select(false);
				section.Show();
				foreach (Arrow arrow in section.Arrows)
				{
					arrow.Visible = group.CheckArrowVisible(arrow, false);
				}
			}		
		}

		public void UpdateMember()
		{
			SectionGroup group = (SectionGroup)Owner;
			m_bFirstAdd = true;
			foreach (Section section in GroupItems)
			{	
				bool bResult = false;
				PointF ptTop = section.GetArrowPoint(Arrow.ArrowPosition.TOP, out bResult);
				if (bResult == true)
				{
					if (m_bFirstAdd == true)
					{
						group.InitGroupBound(ptTop);
						m_bFirstAdd = false;
					}
					group.UpdateGroupBound(ptTop);
				}

				bResult = false;
				PointF ptBottom = section.GetArrowPoint(Arrow.ArrowPosition.BOTTOM, out bResult);
				if (bResult == true)
				{
					if (m_bFirstAdd == true)
					{
						group.InitGroupBound(ptBottom);
						m_bFirstAdd = false;
					}
					group.UpdateGroupBound(ptBottom);
				}

				bResult = false;
				PointF ptLeft = section.GetArrowPoint(Arrow.ArrowPosition.LEFT, out bResult);
				if (bResult == true)
				{
					if (m_bFirstAdd == true)
					{
						group.InitGroupBound(ptLeft);
						m_bFirstAdd = false;
					}
					group.UpdateGroupBound(ptLeft);
				}

				bResult = false;
				PointF ptRight = section.GetArrowPoint(Arrow.ArrowPosition.RIGHT, out bResult);
				if (bResult == true)
				{
					if (m_bFirstAdd == true)
					{
						group.InitGroupBound(ptRight);
						m_bFirstAdd = false;
					}
					group.UpdateGroupBound(ptRight);
				}
			}	
		}

		private bool m_bFirstAdd = true;
		public void AddGroupMember(Section section)
		{
			section.GroupSection = Owner;
			section.GroupMember = true;
			section.Editable = false;
			section.Movable = false;
			section.Select(false);
			section.Hide();
			SectionGroup group = (SectionGroup)Owner;
			foreach (Arrow arrow in section.Arrows)
			{
				bool bVisible = group.CheckArrowVisible(arrow, true);
				arrow.Visible = bVisible;							
			}

			foreach (Arrow arrow in group.Arrows)
			{
				bool bVisible = group.CheckArrowVisible(arrow, true);
				arrow.Visible = bVisible;	
			}
				 

			//Owner.AddChild(section);

			GroupItems.Add(section);

			bool bResult = false;
			PointF ptTop = section.GetArrowPoint(Arrow.ArrowPosition.TOP, out bResult);
			if (bResult == true)
			{
				if (m_bFirstAdd == true)
				{
					group.InitGroupBound(ptTop);
					m_bFirstAdd = false;
				}
				group.UpdateGroupBound(ptTop);
			}


			bResult = false;
			PointF ptBottom = section.GetArrowPoint(Arrow.ArrowPosition.BOTTOM, out bResult);
			if (bResult == true)
			{
				if (m_bFirstAdd == true)
				{
					group.InitGroupBound(ptBottom);
					m_bFirstAdd = false;
				}
				group.UpdateGroupBound(ptBottom);
			}

			bResult = false;
			PointF ptLeft = section.GetArrowPoint(Arrow.ArrowPosition.LEFT, out bResult);
			if (bResult == true)
			{
				if (m_bFirstAdd == true)
				{
					group.InitGroupBound(ptLeft);
					m_bFirstAdd = false;
				}
				group.UpdateGroupBound(ptLeft);
			}

			bResult = false;
			PointF ptRight = section.GetArrowPoint(Arrow.ArrowPosition.RIGHT, out bResult);
			if (bResult == true)
			{
				if (m_bFirstAdd == true)
				{
					group.InitGroupBound(ptRight);
					m_bFirstAdd = false;
				}
				group.UpdateGroupBound(ptRight);
			}
		}
	}
}
