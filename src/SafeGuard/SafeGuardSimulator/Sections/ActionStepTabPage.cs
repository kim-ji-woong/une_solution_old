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
	public class ActionStepTabPage : UnE.SOP.Sections.SectionTabPage
	{
		private int m_nDisasterID = -1;
		public int DisasterID
		{
			get { return m_nDisasterID; }
			set { m_nDisasterID = value; }
		}

		private Data_ActionStep mData = new Data_ActionStep();
		public Data_ActionStep Data
		{
			get { return mData; }
			set 			
			{			
				mData = value;
			}
		}

        public ActionStepTabPage(TabControl tabControl)
            : base(tabControl)
        {
        }

		public void CopyActionStep(Data_ActionStep org, Data_ActionStep trg)
		{
			org.BeginTime = trg.BeginTime;
			org.DisasterID = trg.DisasterID;
			org.EndTime = trg.EndTime;
			org.ID = trg.ID;
			org.Iteration = trg.Iteration;
			org.IterationType = trg.IterationType;
			org.ParentStepID = trg.ParentStepID;
			org.PeriodType = trg.PeriodType;
			org.ProcessTime = trg.ProcessTime;
			org.ProcessTimeType = trg.ProcessTimeType;
			org.StepName = trg.StepName;
			org.WeekdayOption = org.WeekdayOption;		
		}

		private int m_nParentStepID = -1;
		public int ParentStepID
		{
			get { return m_nParentStepID; }
			set { m_nParentStepID = value; }
		}
		
		private string m_nStepPath = "";
		public string StepFullPath
		{
			get { return m_nStepPath; }
			set { m_nStepPath = value; }
		}

        public Sections.PanelSectionEx FindPanel(int nTeamID, Sections.SOPTeam.SOPTeamType nTeamType)
		{
			Type type = typeof(Sections.PanelSectionEx);

			foreach (Control ctrl in Controls)
			{
				if (ctrl.GetType() == type)
				{
					Sections.PanelSectionEx panel = (Sections.PanelSectionEx)ctrl;

					if (panel.TeamID == nTeamID && panel.TeamType == nTeamType)
						return panel;
				}
			}
			return null;
		}
	}
}
