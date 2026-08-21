using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Sections
{
	public class SectionGroup : Section
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

		protected RectangleF m_GroupRegion = new RectangleF();
		public System.Drawing.RectangleF GroupRegion
		{
			get { return m_GroupRegion; }
			set { m_GroupRegion = value; }
		}

		protected bool m_bCollapse = true;
		protected SizeF m_orgSize;
		public System.Drawing.SizeF CollapseSize
		{
			get { return m_orgSize; }
			set { m_orgSize = value; }
		}

		protected PointF m_orgPos = new PointF();
		public bool Collapse
		{
			get { return m_bCollapse; }
			set 
			{ 
				// Save Original Size, position
				if( m_bCollapse != value && value == false)
				{
					m_orgSize = m_sizeMgr.RectSize;
					m_orgPos = m_posMgr.Position;
				}
				m_bCollapse = value;
				OnCollapseChanged(m_bCollapse);				
			}
		}

		public override bool AddArrow(Arrow arrow)
		{
			Section sectionBegin = arrow.BeginLink;
			if (sectionBegin == null)
				return false;

			Section sectionEnd = arrow.EndLink;
			if (sectionEnd == null)
				return false;

			foreach (Arrow _arrow in m_arrArrows)
			{
				if (_arrow.BeginLink == sectionBegin && _arrow.EndLink == sectionEnd)
					return false;
			}

			m_arrArrows.Add(arrow);
			return true;
		}

		private ArrayList m_arCollapseArrows = new ArrayList();
		public System.Collections.ArrayList CollapseArrows
		{
		    get { return m_arCollapseArrows; }
		    set { m_arCollapseArrows = value; }
		}
		//private ArrayList m_arExpandArrows = new ArrayList();
		//public System.Collections.ArrayList ExpandArrows
		//{
		//    get { return m_arExpandArrows; }
		//    set { m_arExpandArrows = value; }
		//}

		public override PointF Position
		{
			get
			{
				return m_posMgr.Position;
			}
			set
			{
				PointF posNew = value;
				OnUpdateGroupPosition(posNew);				
			}
		}


		public bool CheckArrowVisible(Arrow arrow, bool bCollapse)
		{
			Section beginSection = arrow.BeginLink;
			Section endSetion = arrow.EndLink;

			// 그룹 내 컴포넌트의 연결선은 둘다 펼쳐저 있는 경우만 보인다.
			if ((beginSection.GetComponentType() != Section.ComponentType.GROUP) &&
				(endSetion.GetComponentType() != Section.ComponentType.GROUP))
			{
				if (bCollapse == true)
					return false;


				if (beginSection.GroupMember == true && beginSection.GroupSection == this)
				{
					if (endSetion.GroupMember == true)
					{
						SectionGroup group = (SectionGroup)endSetion.GroupSection;
						if (group.m_bCollapse == true)
						{
							return false;
						}						
					}
				}
				if (endSetion.GroupMember == true && endSetion.GroupSection == this)
				{
					if (beginSection.GroupMember == true)
					{
						SectionGroup group = (SectionGroup)beginSection.GroupSection;
						if (group.m_bCollapse == true)
						{
							return false;
						}						
					}
				}
				//return true;
			}

			if (beginSection != this && endSetion != this)
			{
				if (bCollapse == true)
					return false;

				bool bBegin = (beginSection.GetComponentType() == Section.ComponentType.GROUP);
				bool bEnd = (endSetion.GetComponentType() == Section.ComponentType.GROUP);				
				SectionGroup endGroup = null;
				if (bEnd == false)
				{
					if (endSetion.GroupMember == true)
					{
						endGroup = (SectionGroup)endSetion.GroupSection;
					}
				}
				else
				{
					endGroup = (SectionGroup)endSetion;
				}

				SectionGroup beginGroup = null;
				if (bBegin == false)
				{
					if (beginSection.GroupMember == true)
					{
						beginGroup = (SectionGroup)beginSection.GroupSection;
					}
				}
				else
				{
					beginGroup = (SectionGroup)beginSection;
				}						

				bool bCheck1 = (endGroup != null && endGroup == this);
				bool bCheck2 = (beginGroup != null && beginGroup == this);
				
				// 연결되어 있는 Section이 그룹인지 체크
				if (bCheck1 == false && bCheck2 == false)
				{
					if (beginGroup != null)
					{
						if ((beginGroup.m_bCollapse == false && bBegin == true) ||
							(beginGroup.m_bCollapse == true && bBegin == false))
							return false;
					}
					if (endGroup != null)
					{
						if ((endGroup.m_bCollapse == false && bEnd == true) ||
							(endGroup.m_bCollapse == true && bEnd == false))
						{
							return false;
						}
					}
					return !bCollapse;
				}
				
				// Begin Group = this, end = non group
				if (bCheck2 == true || bCheck1 == true)
				{
					if (beginGroup != null)
					{
						if ((beginGroup.m_bCollapse == false && bBegin == true) ||
							(beginGroup.m_bCollapse == true && bBegin == false))
							return false;
					}
					if (endGroup != null)
					{
						if ((endGroup.m_bCollapse == false && bEnd == true) ||
							(endGroup.m_bCollapse == true && bEnd == false))
						{
							return false;
						}
					}
					return true;			
				}
			}

			if (beginSection == this && endSetion.GetComponentType() == Section.ComponentType.GROUP)
			{
				if (bCollapse == false)
					return false;

				SectionGroup group = (SectionGroup)endSetion;
				if (group.m_bCollapse == true)
				{
					return true;
				}
				else
				{
					return false;
				}
				
			}
			else if (beginSection == this && endSetion.GetComponentType() != Section.ComponentType.GROUP)
			{
				if (bCollapse == false)
					return false;

				if (endSetion.GroupMember == true)
				{
					SectionGroup group = (SectionGroup)endSetion.GroupSection;
					if (group.m_bCollapse == true)
					{
						return false;
					}
					else
					{
						return true;
					}
				}
				else				
				{
					return true;
				}				
			}

			if (endSetion == this && beginSection.GetComponentType() == Section.ComponentType.GROUP)
			{
				if (bCollapse == false)
					return false;

				SectionGroup group = (SectionGroup)beginSection;
				if (group.m_bCollapse == true)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else if (endSetion == this && beginSection.GetComponentType() != Section.ComponentType.GROUP)
			{
				if (bCollapse == false)
					return false;

				if (beginSection.GroupMember == true)
				{
					SectionGroup group = (SectionGroup)beginSection.GroupSection;
					if (group.m_bCollapse == true)
					{
						return false;
					}
					else
					{
						return true;
					}
				}
				else
				{
					return true;
				}
			}
			return !bCollapse;
		}


		private void OnCollapseChanged(bool bCollapsed)
		{
			((ShapeGroup)m_shape).Collapse = bCollapsed;
			((PositionManagerGrouop)m_posMgr).Collapse = bCollapsed;
			if (m_bCollapse == false)
			{
				m_sizeMgr.RectSize = m_GroupRegion.Size;
				m_posMgr.Position = m_GroupRegion.Location;
				m_sizeMgr.Editable = false;

				SectionDataGroup data = (SectionDataGroup)Data;
				foreach (Section section in data.GroupItems)
				{
					section.Editable = false;
					section.Movable = false;
					section.Select(false);
					section.Show();
					foreach (Arrow arrow in section.Arrows)
					{
						arrow.Visible = CheckArrowVisible(arrow, m_bCollapse);
					}
				}
				foreach (Arrow arrow in Arrows)
				{
					arrow.Visible = CheckArrowVisible(arrow, m_bCollapse);
				}				
			}
			else
			{
				m_sizeMgr.RectSize = m_orgSize;
				m_posMgr.Position = m_orgPos;
				m_sizeMgr.Editable = true;

				SectionDataGroup data = (SectionDataGroup)Data;
				foreach (Section section in data.GroupItems)
				{
					section.Editable = true;
					section.Movable = false;

					section.Select(false);
					section.Hide();
					foreach (Arrow arrow in section.Arrows)
					{
						arrow.Visible = CheckArrowVisible(arrow, m_bCollapse);
					}
				}

				foreach (Arrow arrow in Arrows)
				{
					arrow.Visible = CheckArrowVisible(arrow, m_bCollapse);
				}	
			}

			float cX = m_GroupRegion.Location.X + (m_GroupRegion.Width / 2.0f);
			float cY = m_GroupRegion.Location.Y + (m_GroupRegion.Height / 2.0f);

			CollapseEventArgs args = new CollapseEventArgs();
			args.Target = this;
			args.Width = m_GroupRegion.Width - m_orgSize.Width;
			args.Height = m_GroupRegion.Height - m_orgSize.Height;
			args.Collapse = m_bCollapse;
			args.Center = new PointF(cX, cY);
			if (GetParent() != null)
				GetParent().OnCollapseChanged(args);
		}

		public void UpdateGroupRegion()
		{
			((ShapeGroup)m_shape).UpdateGroupRegion(m_GroupRegion);
		}

		private void OnUpdateGroupPosition(PointF posNew)
		{
			PointF posCurrent = m_posMgr.Position;
			float dx = posCurrent.X - posNew.X;
			float dy = posCurrent.Y - posNew.Y;

			// GROUP RECT 를 업데이트한다.
			float x = m_GroupRegion.Location.X - dx;
			float y = m_GroupRegion.Location.Y - dy;
			m_GroupRegion.Location = new PointF(x, y);
			((ShapeGroup)m_shape).UpdateGroupRegion(m_GroupRegion);

			m_posMgr.Position = posNew;
			if (m_bCollapse == false)
			{
				m_editBox.Position = m_GroupRegion.Location;
				float orgX = m_orgPos.X - dx;
				float orgY = m_orgPos.Y - dy;
				m_orgPos = new PointF(orgX, orgY);
			}

			// 포함된 그룹의 Position을 이동한다.
			SectionDataGroup groupData = (SectionDataGroup)Data;
			foreach (Section section in groupData.GroupItems)
			{
				float sx = section.Position.X - dx;
				float sy = section.Position.Y - dy;
				bool bEditable = section.Movable;
				section.Movable = true;
				section.Position = new PointF(sx, sy);
				section.Movable = bEditable;
			}
		}

        public SectionGroup(PanelSection ctrlParent)
        {
            m_ctrlParent = ctrlParent;
            m_editBox = new EditBox(this);

            m_shape = new ShapeGroup(this);
            m_posMgr = new PositionManagerGrouop(this, m_shape, m_btnScroll, m_editBox);
			m_sizeMgr = new SizeManager(m_editBox, m_shape, m_posMgr);

            InitShape();
        }

		public SectionGroup(PanelSection ctrlParent, float x, float y)
        {
            m_ctrlParent = ctrlParent;
            m_editBox = new EditBox(this);

			m_shape = new ShapeGroup(this);
			m_posMgr = new PositionManagerGrouop(this, m_shape, m_btnScroll, m_editBox, x, y);
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
			SectionGroup section = new SectionGroup(ctrlParent, m_posMgr.Position.X, m_posMgr.Position.Y);
            section.m_sizeMgr.RectSize = this.m_sizeMgr.RectSize;
			section.m_sizeMgr.Editable = this.m_sizeMgr.Editable;
			section.m_posMgr.Editable = this.m_posMgr.Editable;
			section.m_GroupRegion = this.m_GroupRegion;
			section.m_orgPos = this.m_orgPos;
			section.m_orgSize = this.m_orgSize;

            section.m_strText = this.m_strText;
            section.m_strSectionName = this.m_strSectionName;

            SectionDataGroup dataTrg = (SectionDataGroup)section.Data;
            SectionDataGroup dataSrc = (SectionDataGroup)this.Data;

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
			dataTrg.GroupItems = dataSrc.GroupItems;

            return section;
        }

        private void InitShape()
        {
            m_data = new SectionDataGroup();
            m_data.Owner = this;

            ArrayList arrBoundary = GetDefaultBoundary();
            SetBoundary(arrBoundary);

            AdjustStringFormat();
        }

        public override ComponentType GetComponentType()
        {
            return ComponentType.GROUP;
        }

		public override Section Select(float x, float y)
		{
			if (m_bCollapse == true)
			{
				return base.Select(x, y);
			}
			else
			{
				if( m_GroupRegion.Contains(x, y))
				{					
					return this;
				}				
			}
			return null;
		}

		public void InitGroupBound(PointF addPoint)
		{
			m_GroupRegion.Location = new PointF( addPoint.X -10.0f, addPoint.Y - 10.0f);
			m_GroupRegion.Size = new SizeF(10.0f, 10.0f);
		}

        public void UpdateGroupBound()
        {
            if (this.Collapse)
                return;

            bool isFirst = true;
            float xLT = 0.0f, yLT = 0.0f;
            SectionDataGroup data = (SectionDataGroup)m_data;

            foreach (Section section in data.GroupItems)
            {
				bool bResult = false;
				PointF ptTop = section.GetArrowPoint(Arrow.ArrowPosition.TOP, out bResult);
				if (bResult == true)
				{
					if (isFirst == true)
					{
						InitGroupBound(ptTop);
						isFirst = false;
					}
					UpdateGroupBound(ptTop);
				}


				bResult = false;
				PointF ptBottom = section.GetArrowPoint(Arrow.ArrowPosition.BOTTOM, out bResult);
				if (bResult == true)
				{
					if (isFirst == true)
					{
						InitGroupBound(ptBottom);
						isFirst = false;
					}
					UpdateGroupBound(ptBottom);
				}

				bResult = false;
				PointF ptLeft = section.GetArrowPoint(Arrow.ArrowPosition.LEFT, out bResult);
				if (bResult == true)
				{
					if (isFirst == true)
					{
						InitGroupBound(ptLeft);
						isFirst = false;
					}
					UpdateGroupBound(ptLeft);
				}

				bResult = false;
				PointF ptRight = section.GetArrowPoint(Arrow.ArrowPosition.RIGHT, out bResult);
				if (bResult == true)
				{
					if (isFirst == true)
					{
						InitGroupBound(ptRight);
						isFirst = false;
					}
					UpdateGroupBound(ptRight);
				}
            }


			if(!isFirst)
			{
				xLT = m_GroupRegion.Left;
				yLT = m_GroupRegion.Top;
				OnUpdateGroupPosition(new PointF(xLT, yLT));
			}
        }

		public void UpdateGroupBound(PointF addPoint)
		{
			if (!m_GroupRegion.Contains(addPoint))
			{
				if (addPoint.X < m_GroupRegion.Left)
				{
					float dX = m_GroupRegion.Left - addPoint.X + 10.0f;
					m_GroupRegion.Location = new PointF(addPoint.X - 10.0f, m_GroupRegion.Location.Y);
					m_GroupRegion.Size = new Size((int)(m_GroupRegion.Width + dX), (int)m_GroupRegion.Height);
				}
				else if (addPoint.X > m_GroupRegion.Right)
				{
					float dWidth = addPoint.X - m_GroupRegion.Right + 10.0f;
					m_GroupRegion.Size = new Size((int)(m_GroupRegion.Width + dWidth), (int)m_GroupRegion.Height);
				}

				if (addPoint.Y < m_GroupRegion.Top)
				{
					float dY = m_GroupRegion.Top - addPoint.Y + 10.0f;
					m_GroupRegion.Location = new PointF(m_GroupRegion.Location.X, addPoint.Y- 10.0f);
					m_GroupRegion.Size = new Size((int)m_GroupRegion.Width, (int)(m_GroupRegion.Height + dY));
				}
				else if (addPoint.Y > m_GroupRegion.Bottom)
				{
					float dHeight = addPoint.Y - m_GroupRegion.Bottom + 10.0f;
					m_GroupRegion.Size = new Size((int)m_GroupRegion.Width, (int)(m_GroupRegion.Height + dHeight));
				}

				float x = (m_GroupRegion.Left + m_GroupRegion.Right) / 2.0f - Shape.GetSize(true) / 2.0f;
				float y = (m_GroupRegion.Bottom + m_GroupRegion.Top) / 2.0f - Shape.GetSize(false) / 2.0f;
				m_posMgr.Position = new PointF(x, y);
			}
		}

		public override void DrawText(Graphics g, PointF ptCurrent)
		{
			if( m_bCollapse == true)
			{
				base.DrawText(g, ptCurrent);
			}
		}
	}

	public class PositionManagerGrouop : PositionManager
	{
		protected bool m_bCollapse = true;
		public bool Collapse
		{
			get { return m_bCollapse; }
			set { m_bCollapse = value; }
		}

		public PositionManagerGrouop(Section sectionParent, float x = 0, float y = 0) 
			: base(sectionParent, x, y)
        {
            m_sectionParent = sectionParent;
            this.x = x;
            this.y = y;
        }

		public PositionManagerGrouop(Section sectionParent, Shape shape, Button btnScroll, EditBox editBox, float x = 0, float y = 0)
			: base(sectionParent, shape, btnScroll, editBox, x, y)
        {
            m_sectionParent = sectionParent;
            m_shape = shape;
            m_btnScroll = btnScroll;
            m_editBox = editBox;
            this.x = x;
            this.y = y;
        }

		public override PointF Position
		{
			get
			{
				return new PointF(x, y);				
			}
			set
			{
				if (x != value.X || y != value.Y)
				{
					if (m_shape != null)
						m_shape.ChangePosition(value);

					x = value.X;
					y = value.Y;

					// 화살표 위치 변경
					m_sectionParent.CalcArrowPositions();

					if (m_btnScroll != null)
						m_btnScroll.Location = new Point((int)(x + m_sectionParent.GetScrollButtonArea(true)), (int)(y + m_sectionParent.GetScrollButtonArea(false)));

					if (m_editBox != null)
						m_editBox.Position = value;
				}
			}
		}
	}
}
