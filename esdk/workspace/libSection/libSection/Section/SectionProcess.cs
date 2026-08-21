using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Diagnostics;

namespace Sections
{
    public class SectionProcess : Section
    {
        private static float m_fWidth = 200;
        private static float m_fHeight = 82;
        private string m_strTextUp = "";
        private string m_strTextDown = "";
        private static PointF[] m_arrDefaultShape = null;

        private static Size m_Size = new Size(200, 82);
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

        public string TextUP
        {
            get { return m_strTextUp; }
            set { m_strTextUp = value; }
        }

        public string TextDown
        {
            get { return m_strTextDown; }
            set { m_strTextDown = value; }
        }

        public override string Title
        {
            get { return TextUP; }
            set { TextUP = value; }
        }

		public override PointF Position
		{
			get
			{
				return m_posMgr.Position;
			}
			set
			{
				m_posMgr.Position = value;			
			}
		}

        public SectionProcess(PanelSection ctrlParent)
        {
            m_ctrlParent = ctrlParent;

            m_shape = new ShapeProcess(this);
            m_posMgr = new PositionManager(this, m_shape, m_btnScroll, m_editBox);
            m_sizeMgr = new SizeManager(m_editBox, m_shape, m_posMgr);

            InitShape();
        }

        public SectionProcess(PanelSection ctrlParent, float x, float y)
        {
            m_ctrlParent = ctrlParent;

            m_shape = new ShapeProcess(this);
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

            arrBoundary.Add(new PointF(0, 0));
            arrBoundary.Add(new PointF(m_fWidth, 0));
            arrBoundary.Add(new PointF(m_fWidth, m_fHeight));
            arrBoundary.Add(new PointF(0, m_fHeight));

            return arrBoundary;
        }

        public override void MakeData(string strStepName, string strTeamName)
        {
            m_data.SetDefaultID(strStepName, strTeamName);
        }

        public override Section Clone(PanelSection ctrlParent)
        {
            SectionProcess section = new SectionProcess(ctrlParent, m_posMgr.Position.X, m_posMgr.Position.Y);
            section.m_sizeMgr.RectSize = this.m_sizeMgr.RectSize;

            section.m_strText = this.m_strText;
            section.m_strSectionName = this.m_strSectionName;
            section.m_strTextDown = this.m_strTextDown;
            section.m_strTextUp = this.m_strTextUp;

            SectionDataProcess dataTrg = (SectionDataProcess)section.Data;
            SectionDataProcess dataSrc = (SectionDataProcess)this.Data;

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
            MissionItemFrom(dataTrg.MissionItems, dataSrc.MissionItems);
            CheckedItemFrom(dataTrg.CheckedItems, dataSrc.CheckedItems);
            dataTrg.MissionTransfer = dataSrc.MissionTransfer;
            dataTrg.ProcessingTime = ProcessingTimeFrom(dataSrc.ProcessingTime);
            SOPTeamFrom(dataTrg.TeamList, dataSrc.TeamList);
            dataTrg.TransferTeamLeaderOnly = dataSrc.TransferTeamLeaderOnly;
            dataTrg.UseProcessingTime = dataSrc.UseProcessingTime;

            dataTrg.Expression = dataSrc.Expression;
            dataTrg.ShowExpression = dataSrc.ShowExpression;
            dataTrg.ShowTempExpression = dataSrc.ShowTempExpression;

            dataTrg.Commander.Team = dataSrc.Commander.Team;
            dataTrg.Commander.DisplayText = dataSrc.Commander.DisplayText;
            dataTrg.Commander.IsTeamMember = dataSrc.Commander.IsTeamMember;
            dataTrg.Commander.TeamMemberID = dataSrc.Commander.TeamMemberID;

            dataTrg.TextHorizontalAlign = dataSrc.TextHorizontalAlign;
            dataTrg.TextVerticalAlign = dataSrc.TextVerticalAlign;

            return section;
        }
       
        private void SOPTeamFrom(ArrayList arrTrg, ArrayList arrTeamList)
        {
            foreach (SOPTeam team in arrTeamList)
            {
                SOPTeam teamTrg = new SOPTeam();

                teamTrg.TeamID = team.TeamID;
                teamTrg.TeamName = team.TeamName;
                teamTrg.TeamType = team.TeamType;

                arrTrg.Add(teamTrg);
            }
        }

        private ProcessingTime ProcessingTimeFrom(ProcessingTime time)
        {
            ProcessingTime processing = new ProcessingTime();

            processing.ProcessingType = time.ProcessingType;
            processing.Time = time.Time;

            return processing;
        }

        private void MissionItemFrom(ArrayList arrTrg, ArrayList arrItems)
        {
            foreach (MissionItem item in arrItems)
            {
                MissionItem missionItem = new MissionItem();

                //missionItem.Transmission = item.Transmission;
                missionItem.Mission = item.Mission;
                missionItem.ArrCheckItem = item.ArrCheckItem;
                missionItem.CheckItem = item.CheckItem;
                missionItem.Target = item.Target;
                missionItem.TransmissionType = item.TransmissionType;

                if (missionItem.Commander == null)
                    missionItem.Commander = item.Commander;
                else if (item.Commander != null)
                {
                    missionItem.Commander.Team = item.Commander.Team;
                    missionItem.Commander.DisplayText = item.Commander.DisplayText;
                    missionItem.Commander.IsTeamMember = item.Commander.IsTeamMember;
                    missionItem.Commander.TeamMemberID = item.Commander.TeamMemberID;
                }

                arrTrg.Add(missionItem);
            }
        }

        private void CheckedItemFrom(ArrayList arrTrg, ArrayList arrItems)
        {
            foreach (CheckedItem item in arrItems)
            {
                CheckedItem checkedItem = new CheckedItem();

                checkedItem.Category = item.Category;
                checkedItem.Item = item.Item;
                checkedItem.ItemCount = item.ItemCount;
                checkedItem.Location = item.Location;
                checkedItem.SubCategory = item.SubCategory;
                arrTrg.Add(checkedItem);
            }
        }

		public void SetFillColor(Color color, bool upside)
		{
			ShapeProcess shape = (ShapeProcess)m_shape;
			shape.SetFillColor(color, upside);
		}

		public Color GetFillColor(bool upside)
		{
			ShapeProcess shape = (ShapeProcess)m_shape;
			return shape.GetFillColor(upside);
		}

        private void InitShape()
        {
            m_data = new SectionDataProcess();
            m_data.Owner = this;
            ShapeProcess shape = (ShapeProcess)m_shape;
            ArrayList arrBoundary = GetDefaultBoundary();
            SetBoundary(arrBoundary);
            
            AdjustStringFormat();
        }

        public override void DrawText(Graphics g, PointF ptCurrent)
        {
            float xMB = m_editBox.GetCoord(EditBox.CoordType.X_MIDDLE) ;
            float yMB = m_editBox.GetCoord(EditBox.CoordType.Y_BOTTOM) ;

            ShapeProcess shape = (ShapeProcess)m_shape;

            float fFontHeight = TEXT_FONT.Size;

            float fWidth = shape.GetSize(true, true) - 6;
            float fHeight = shape.GetSize(false, true) - 6;
            float x = ptCurrent.X + 3;

            float y = ptCurrent.Y  + 6;

            float middleY = ptCurrent.Y + shape.GetSize(false) / 2;
            fHeight = middleY - y;

            string szDisplayText = m_strTextUp;
            if (m_data != null)
            {
                if (m_data.ShowTempExpression == true)
                    szDisplayText = m_data.Expression;
            }

            if (m_ctrlParent.VisibleSectionNumber && m_data.SectionNumber > 0)
                szDisplayText = m_data.SectionNumber.ToString() + ". " + szDisplayText;

            g.DrawString(szDisplayText, TEXT_FONT, m_brushText, new RectangleF(x, y, fWidth, fHeight), m_textFormat);
            //g.DrawString(m_strTextUp, TEXT_FONT, m_brushText, new RectangleF(x, y, fWidth, fHeight), m_textFormat);

            fHeight = shape.GetSize(false, false) - 6;

            y = middleY + 6;

            g.DrawString(m_strTextDown, TEXT_FONT, m_brushText, new RectangleF(x, y, fWidth, fHeight), m_textFormat);
        }

        public override ComponentType GetComponentType()
        {
            return ComponentType.PROCESS;
        }
    }
}
