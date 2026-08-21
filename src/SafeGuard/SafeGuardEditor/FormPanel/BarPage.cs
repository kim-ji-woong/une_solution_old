using System.Collections;
using System.Windows.Forms;

namespace SOPManager
{
	public partial class BarPage : Form
	{
		private int m_nCheckCount = 0;

		public int CheckCount
		{
			get { return m_nCheckCount; }
			set { m_nCheckCount = value; }
		}

		private ArrayList m_arrCheckTeam = new ArrayList();

		public ArrayList CheckTeamList
		{
			get { return m_arrCheckTeam; }
			set { m_arrCheckTeam = value; }
		}

		public BarPage()
		{
			InitializeComponent();
		}

		public void ClearGrid()
		{
			m_nCheckCount = 0;
			m_arrCheckTeam.Clear();
			dataGridView.Rows.Clear();
		}

		// nSrcRowIndex에 있던 행을 nTrgRowIndex로 옮긴다.
		public void Reorder(int nSrcRowIndex, int nTrgRowIndex)
		{
			if (nSrcRowIndex < 0 || nTrgRowIndex < 0)
				return;

			if (nSrcRowIndex == nTrgRowIndex)
				return;

			if (nSrcRowIndex >= dataGridView.Rows.Count ||
				nTrgRowIndex >= dataGridView.Rows.Count)
				return;

			DataGridViewRow row = dataGridView.Rows[nSrcRowIndex];
			dataGridView.Rows.RemoveAt(nSrcRowIndex);

			if (nTrgRowIndex == dataGridView.Rows.Count)
				dataGridView.Rows.Add(row);
			else
				dataGridView.Rows.Insert(nTrgRowIndex, row);
		}

		public bool IsEnableDelete(int nRowIndex)
		{
			if (nRowIndex < 0 || nRowIndex > dataGridView.Rows.Count)
				return false;

			// 마지막 행은 삭제할 수 없음
			if (dataGridView.Rows.Count <= 1)
				return false;

			bool isChecked = (bool)dataGridView.Rows[nRowIndex].Cells[1].Value;

			// 현재 화면에 유일하게 보이는 행은 삭제할 수 없음
			if (m_nCheckCount == 1 && isChecked)
				return false;

			return true;
		}

		public bool DeleteDataGrid(int nRowIndex)
		{
			if (!IsEnableDelete(nRowIndex))
				return false;

			if ((bool)dataGridView.Rows[nRowIndex].Cells[1].Value)
				m_nCheckCount--;

			dataGridView.Rows.RemoveAt(nRowIndex);

			if (m_nCheckCount == 1)
			{
				foreach (DataGridViewRow row in dataGridView.Rows)
				{
					if ((bool)row.Cells[1].Value)
					{
						row.Cells[1].ReadOnly = true;
						break;
					}
				}
			}
			return true;
		}

		public void InsertDataGrid(int nRowIndex, StepMemberData team)
		{
			if (nRowIndex < 0 || nRowIndex > dataGridView.Rows.Count)
				return;

			DataGridViewRow gridRow = new DataGridViewRow();

			DataGridViewCell cell = new DataGridViewTextBoxCell();
			cell.Value = team.TeamName;

			gridRow.Cells.Add(cell);
			gridRow.Tag = team;
			cell = new DataGridViewCheckBoxCell();
			cell.Value = true;

			gridRow.Cells.Add(cell);

			m_nCheckCount++;

			CheckTeam checkTeam = new CheckTeam();
			checkTeam.TeamName = team.TeamName;
			checkTeam.TeamID = team.ID;
			checkTeam.Check = true;

			if (nRowIndex < dataGridView.Rows.Count)
			{
				m_arrCheckTeam.Insert(nRowIndex, checkTeam);
				dataGridView.Rows.Insert(nRowIndex, gridRow);
			}
			else
			{
				m_arrCheckTeam.Add(checkTeam);
				dataGridView.Rows.Add(gridRow);
			}

			if (dataGridView.Rows.Count == 1)
				dataGridView.Rows[0].Cells[1].ReadOnly = true;
			else
			{
				foreach (DataGridViewRow row in dataGridView.Rows)
				{
					row.Cells[1].ReadOnly = false;
				}
			}
		}

		public void SetDataGrid(ArrayList arrTeamNames)
		{
			ClearGrid();

			foreach (StepMemberData team in arrTeamNames)
			{
				if (team.TeamName == "")
					continue;
				DataGridViewRow gridRow = new DataGridViewRow();

				DataGridViewCell cell = new DataGridViewTextBoxCell();
				cell.Value = team.TeamName;
				gridRow.Cells.Add(cell);
				gridRow.Tag = team;

				cell = new DataGridViewCheckBoxCell();
				cell.Value = true;
				gridRow.Cells.Add(cell);

				dataGridView.Rows.Add(gridRow);
				m_nCheckCount++;

				CheckTeam checkTeam = new CheckTeam();
				checkTeam.TeamName = team.TeamName;
				checkTeam.TeamID = team.ID;
				checkTeam.Check = true;

				m_arrCheckTeam.Add(checkTeam);
			}

			if (dataGridView.Rows.Count == 1)
				dataGridView.Rows[0].Cells[1].ReadOnly = true;
		}

		public void SetDataGrid()
		{
			ClearGrid();

			ArrayList arrTeam = FormMain.Instance.GetPageLevel().UsingTeam;
			foreach (StepMemberData row in arrTeam)
			{
				string strCaption = row.TeamName;
                
				DataGridViewRow gridRow = new DataGridViewRow();
				gridRow.Tag = row;

				DataGridViewCell cell = new DataGridViewTextBoxCell();
				cell.Value = strCaption;
				gridRow.Cells.Add(cell);

				cell = new DataGridViewCheckBoxCell();
				cell.Value = true;
				gridRow.Cells.Add(cell);

				dataGridView.Rows.Add(gridRow);
				m_nCheckCount++;

				CheckTeam checkTeam = new CheckTeam();
				checkTeam.TeamName = row.TeamName;
				checkTeam.TeamID = row.ID;
				checkTeam.Check = true;

				m_arrCheckTeam.Add(checkTeam);
			}

			if (dataGridView.Rows.Count == 1)
				dataGridView.Rows[0].Cells[1].ReadOnly = true;
		}

		private void CellReadOnly(DataGridViewCheckBoxCell exceptCell)
		{
			foreach (DataGridViewRow row in dataGridView.Rows)
			{
				DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)row.Cells[1];

				if ((bool)cell.Value == true && cell != exceptCell)
				{
					cell.ReadOnly = true;
					break;
				}
			}
		}

		private void CellFree()
		{
			foreach (DataGridViewRow row in dataGridView.Rows)
			{
				DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)row.Cells[1];
				cell.ReadOnly = false;
			}
		}

		private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0)
				return;

			DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
			cell = (DataGridViewTextBoxCell)dataGridView.Rows[e.RowIndex].Cells[0];
			string strValue = cell.Value.ToString();

			DataGridViewCheckBoxCell checkCell = new DataGridViewCheckBoxCell();
			checkCell = (DataGridViewCheckBoxCell)dataGridView.Rows[e.RowIndex].Cells[1];
			if (checkCell.ReadOnly)
				return;

			int nRowIdx = e.RowIndex;
			StepMemberData team = (StepMemberData)dataGridView.Rows[e.RowIndex].Tag;
			CheckTeam checkTeam = FindTeam(team.TeamName, team.ID);

			switch ((bool)checkCell.Value)
			{
				case true:
					if (m_nCheckCount == 2)
					{
						// Check 속성이 true인 Cell이 하나만 남게 되므로 해당 Cell은 ReadOnly로 둔다.
						CellReadOnly(checkCell);
					}

					checkTeam.Check = false;
					checkCell.Value = false;

					m_nCheckCount--;
					break;

				case false:
					if (m_nCheckCount == 1)
					{
						// Check 속성이 true인 Cell이 두 개 이상이므로 모든 Cell의 ReadOnly 속성을 제거한다.
						CellFree();
					}

					checkTeam.Check = true;
					checkCell.Value = true;

					m_nCheckCount++;
					break;
			}

			FormMain.Instance.GetPageLevel().ShowPanel();
		}

        private CheckTeam FindTeam(string strValue, int nTeamID)
		{
			foreach (CheckTeam team in m_arrCheckTeam)
			{
				if (team.TeamName == strValue && team.TeamID == nTeamID)
				{
					return team;
				}
			}

			return null;
		}

		private void dataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0)
				return;

			DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
			cell = (DataGridViewTextBoxCell)dataGridView.Rows[e.RowIndex].Cells[0];
			string strValue = cell.Value.ToString();

			DataGridViewCheckBoxCell checkCell = new DataGridViewCheckBoxCell();
			checkCell = (DataGridViewCheckBoxCell)dataGridView.Rows[e.RowIndex].Cells[1];
			if (checkCell.ReadOnly)
				return;

			int nRowIdx = e.RowIndex;
			StepMemberData team = (StepMemberData)dataGridView.Rows[e.RowIndex].Tag;
			CheckTeam checkTeam = FindTeam(team.TeamName, team.ID);

			switch ((bool)checkCell.Value)
			{
				case true:
					if (m_nCheckCount == 2)
					{
						// Check 속성이 true인 Cell이 하나만 남게 되므로 해당 Cell은 ReadOnly로 둔다.
						CellReadOnly(checkCell);
					}

					checkTeam.Check = false;
					checkCell.Value = false;

					m_nCheckCount--;
					break;

				case false:
					if (m_nCheckCount == 1)
					{
						// Check 속성이 true인 Cell이 두 개 이상이므로 모든 Cell의 ReadOnly 속성을 제거한다.
						CellFree();
					}

					checkTeam.Check = true;
					checkCell.Value = true;

					m_nCheckCount++;
					break;
			}

			FormMain.Instance.GetPageLevel().ShowPanel();
		}
	}

	public class CheckTeam
	{
		private string m_strTeamName;
		private bool m_isCheck;

		public string TeamName
		{
			get { return m_strTeamName; }
			set { m_strTeamName = value; }
		}

		private int m_nTeamID = -1;

		public int TeamID
		{
			get { return m_nTeamID; }
			set { m_nTeamID = value; }
		}

        private Sections.SOPTeam.SOPTeamType m_nTeamType = Sections.SOPTeam.SOPTeamType.None;

        public Sections.SOPTeam.SOPTeamType TeamType
		{
			get { return m_nTeamType; }
			set { m_nTeamType = value; }
		}

		public bool Check
		{
			get { return m_isCheck; }
			set { m_isCheck = value; }
		}
	}
}