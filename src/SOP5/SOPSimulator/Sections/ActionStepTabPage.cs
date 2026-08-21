using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sections;

namespace SOPMonitoringSystem
{
	public class ActionStepTabPage : TabPage
	{
		private int m_nDisasterID = -1;
		public int DisasterID
		{
			get { return m_nDisasterID; }
			set { m_nDisasterID = value; }
		}

		private int m_nActionStepID = -1;
		public int ActionStepID
		{
			get { return m_nActionStepID; }
			set { m_nActionStepID = value; }
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

		public Sections.PanelSectionEx FindPanel(int nTeamID, int nTeamType)
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
