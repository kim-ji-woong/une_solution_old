using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoadMan
{
	public partial class FormLotNumberSearch : Form
	{
		public FormLotNumberSearch()
		{
			InitializeComponent();

			textBox1.AutoCompleteMode = AutoCompleteMode.Suggest;
			textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;

			InitLotNumber();
		}
		
		private Dictionary<string, LandAddressData> m_arLotNumbers = new Dictionary<string, LandAddressData>();
		private Dictionary<LandAddressData, string> m_arLotNumNames = new Dictionary<LandAddressData, string>();

		private void InitLotNumber()
		{
			m_arLotNumbers.Clear();
			m_arLotNumNames.Clear();

			textBox1.Text = "";
			textBox1.AutoCompleteCustomSource.Clear();

			PanelDXFViewer pane = FormMain.Instance.CurrentPanel;
			if (pane != null)
			{
				ArrayList arData = new ArrayList();
				List<ProcessSchedule> arList = pane.ProcessSchedules;
				foreach (ProcessSchedule schedule in arList)
				{
					List<ScheduleProperty> arProperties =  (List<ScheduleProperty>)schedule.Properties;
					foreach (ScheduleProperty prop in arProperties)
					{
						List<LandAddressData> datas = (List<LandAddressData>)prop.LandAddressDatas;
						foreach (LandAddressData data in datas)
						{
							string szAddr = data.ToString();
							if (!m_arLotNumbers.ContainsKey(szAddr))
							{
								m_arLotNumbers.Add(szAddr, data);
								m_arLotNumNames.Add(data, szAddr);
								arData.Add(data);
								textBox1.AutoCompleteCustomSource.Add(szAddr);
							}
						}
					}
				}

				InitAllDataGrid(arData);
			}
		}

		private void SetAllData()
		{
			ArrayList arData = new ArrayList();
			foreach(LandAddressData value in m_arLotNumbers.Values)
			{
				arData.Add(value);
			}
			InitAllDataGrid(arData);
		}

		private void InitLotNumberForName(string szName)
		{
			PanelDXFViewer pane = FormMain.Instance.CurrentPanel;
			if (pane != null)
			{
				ArrayList arData = new ArrayList();
				List<ProcessSchedule> arList = pane.ProcessSchedules;
				foreach (ProcessSchedule schedule in arList)
				{
					List<ScheduleProperty> arProperties = (List<ScheduleProperty>)schedule.Properties;
					foreach (ScheduleProperty prop in arProperties)
					{
						List<LandAddressData> datas = (List<LandAddressData>)prop.LandAddressDatas;
						foreach (LandAddressData data in datas)
						{
							string szAddr = data.ToString();

							if (szAddr.Contains(szName) && !arData.Contains(szAddr))
							{						
								arData.Add(data);								
							}
						}
					}
				}
				InitAllDataGrid(arData);
			}
		}

		private void InitAllDataGrid(ArrayList arDatas)
		{
			gridAll.ClearSelection();
			gridAll.Rows.Clear();

			foreach(LandAddressData data in arDatas)
			{
				DataGridViewRow row = new DataGridViewRow();
				row.Tag = data;

				DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
				cell1.Value = data.ToString();
				cell1.ToolTipText = data.ToString();

				row.Cells.Add(cell1);

				gridAll.Rows.Add(row);
			}
			gridAll.Sort(colAllLotNum, ListSortDirection.Ascending);
		}


		private ArrayList m_arSelectData = new ArrayList();
		private void InitSelectedDataGrid(ArrayList arDatas)
		{
			gridSelected.ClearSelection();
			gridSelected.Rows.Clear();
			m_arSelectData.Clear();

			foreach (LandAddressData data in arDatas)
			{
				if (!m_arSelectData.Contains(data))
				{
					m_arSelectData.Add(data);

					DataGridViewRow row = new DataGridViewRow();
					row.Tag = data;

					DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
					cell1.Value = data.ToString();
					cell1.ToolTipText = data.ToString();

					row.Cells.Add(cell1);

					gridSelected.Rows.Add(row);
				}
			}
			gridSelected.Sort(colSelLotNum, ListSortDirection.Ascending);
		}

		public void SetData(ArrayList arLotNumbers)
		{
			ArrayList arDatas = new ArrayList();
			foreach (LandAddressData data in arLotNumbers)
			{
				if (m_arLotNumbers.ContainsValue(data))
				{				
					if(!arDatas.Contains(data))
					{
						arDatas.Add(data);
					}
				}				
			}

			InitSelectedDataGrid(arDatas);
		}

		public ArrayList GetData()
		{
			ArrayList arDatas = (ArrayList)m_arSelectData.Clone();
			return arDatas;
		}


		private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{

		}

		private void btnAdded_Click(object sender, EventArgs e)
		{
			DataGridViewSelectedRowCollection rows = gridAll.SelectedRows;
			if (rows == null)
				return;

			foreach (DataGridViewRow row in rows)
			{
				LandAddressData data = (LandAddressData)row.Tag;
				if(!m_arSelectData.Contains(data))
				{
					DataGridViewRow row2 = new DataGridViewRow();
					row2.Tag = data;
					DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
					cell1.Value = data.ToString();
					cell1.ToolTipText = data.ToString();
					row2.Cells.Add(cell1);
					gridSelected.Rows.Add(row2);

					m_arSelectData.Add(data);
				}
			}
		}

		private void btnRemoved_Click(object sender, EventArgs e)
		{
			DataGridViewSelectedRowCollection rows = gridSelected.SelectedRows;
			if (rows == null)
				return;

			foreach(DataGridViewRow row in rows)
			{
				LandAddressData data = (LandAddressData)row.Tag;
				
				m_arSelectData.Remove(data);
				gridSelected.Rows.Remove(row);
				
			}
		}

		private void button7_Click(object sender, EventArgs e)
		{
			SetAllData();
		}

		private void btnClearSelected_Click(object sender, EventArgs e)
		{
			gridSelected.ClearSelection();
			gridSelected.Rows.Clear();
			m_arSelectData.Clear();
		}

		private void btnLotNumSearch(object sender, EventArgs e)
		{
			string szTargetName = textBox1.Text;
			if(szTargetName == "")
			{
				SetAllData();
				return;
			}

			InitLotNumberForName(szTargetName);
		}

		private void button5_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			this.Close();
		}

		private void button4_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			this.Close();
		}
		
	}
}
