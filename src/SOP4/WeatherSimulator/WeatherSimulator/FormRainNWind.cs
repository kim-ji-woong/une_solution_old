using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeatherSimulator
{
    public partial class FormRainNWind : Form, IGridOwner<float>
    {
        private TimePickerManager m_timePickerManager = null;
        private GridManager m_gridManager = null;

        public FormRainNWind()
        {
            InitializeComponent();

            InitGrid();
        }

        private void InitGrid()
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            m_timePickerManager = new TimePickerManager(dataGridView1, 1);
            m_gridManager = new GridManager(dataGridView1, this);

            m_gridManager.ColumnIndeces.Add(2);
            m_gridManager.ColumnIndeces.Add(3);
            m_gridManager.ColumnIndeces.Add(4);
            m_gridManager.ColumnIndeces.Add(5);
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (e.RowIndex > 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex - 1];
                row.Cells[0].Value = row.Index + 1;
                row.Cells[0].ReadOnly = true;

                if (row.Cells[1].Value == null)
                {
                    VariousData<DateTime> data = new VariousData<DateTime>(DateTime.Now);
                    row.Cells[1].Value = WeatherData.MakeTimeString(data.Data);
                    row.Cells[1].Tag = data;
                }

                if (row.Tag == null)
                    row.Tag = new RainNWind();
            }

            DataGridViewRow rowCurrent = dataGridView1.Rows[e.RowIndex];

            if (!rowCurrent.IsNewRow)
            {
                rowCurrent.Cells[0].Value = rowCurrent.Index + 1;
                rowCurrent.Cells[0].ReadOnly = true;

                if (rowCurrent.Cells[1].Value == null)
                {
                    VariousData<DateTime> data = new VariousData<DateTime>(DateTime.Now);
                    rowCurrent.Cells[1].Value = WeatherData.MakeTimeString(data.Data);
                    rowCurrent.Cells[1].Tag = data;
                }

                if (rowCurrent.Tag == null)
                    rowCurrent.Tag = new RainNWind();
            }
        }

        public void LoadData(bool dbData)
        {
            List<RainNWind> rainDatas = null;

            if (dbData)
            {
                ApplyCurrentData();
                rainDatas = DataManager.Instance.RainDBDatas;
            }
            else
                rainDatas = DataManager.Instance.RainCurrentDatas;

            dataGridView1.Rows.Clear();

            SetGrid(rainDatas);
        }

        public void ApplyCurrentData()
        {
            List<RainNWind> currentDatas = DataManager.Instance.RainCurrentDatas;
            currentDatas.Clear();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow || IsNullData(row))
                    continue;

                RainNWind rain = (RainNWind)row.Tag;
                SetData(rain, row);
                currentDatas.Add(rain);
            }
        }

        private bool IsNullData(DataGridViewRow row)
        {
            for (int i=1;i<dataGridView1.Columns.Count;i++)
            {
                if (row.Cells[i].Value != null)
                    return false;
            }

            return true;
        }

        private void SetData(RainNWind rain, DataGridViewRow row)
        {
            if (row.Cells[1].Tag != null)
                rain.Time = ((VariousData<DateTime>)row.Cells[1].Tag).Data;
            else
                rain.Time = DateTime.Now;

            rain.RainHour = (VariousData<float>)row.Cells[2].Tag;
            rain.RainDay = (VariousData<float>)row.Cells[3].Tag;
            rain.WindSpeedAve = (VariousData<float>)row.Cells[4].Tag;
            rain.WindSpeedMax = (VariousData<float>)row.Cells[5].Tag;
            rain.Region = row.Cells[6].Value == null ? null : row.Cells[6].Value.ToString();
        }

        private void SetGrid(List<RainNWind> datas)
        {
            int nIndex = 1;

            foreach (RainNWind rain in datas)
            {
                DataGridViewRow row = FormMain.Instance.MakeNewRow(dataGridView1);

                row.Cells[0].Value = nIndex++;
                row.Cells[0].Tag = row.Cells[0].Value;
                row.Cells[0].ReadOnly = true;

                row.Cells[1].Value = rain.GetTimeString();
                row.Cells[1].Tag = new VariousData<DateTime>(rain.Time);

                if (rain.RainHour != null)
                {
                    row.Cells[2].Value = GetCellValueString(rain.RainHour, 2);
                    row.Cells[2].Tag = rain.RainHour;
                }

                if (rain.RainDay != null)
                {
                    row.Cells[3].Value = GetCellValueString(rain.RainDay, 3);
                    row.Cells[3].Tag = rain.RainDay;
                }

                if (rain.WindSpeedAve != null)
                {
                    row.Cells[4].Value = GetCellValueString(rain.WindSpeedAve, 4);
                    row.Cells[4].Tag = rain.WindSpeedAve;
                }

                if (rain.WindSpeedMax != null)
                {
                    row.Cells[5].Value = GetCellValueString(rain.WindSpeedMax, 5);
                    row.Cells[5].Tag = rain.WindSpeedMax;
                }

                if (rain.Region != null)
                {
                    row.Cells[6].Value = rain.Region;
                    row.Cells[6].Tag = row.Cells[6].Value;
                }

                row.Cells[0].ReadOnly = true;
                row.Tag = rain;
            }
        }

        public string GetCellValueString(VariousData<float> data, int nColumnIndex)
        {
            if (nColumnIndex == 2)
                return string.Format("{0:F1} mm/hour", data.Data);
            else if (nColumnIndex == 3)
                return string.Format("{0:F1} mm/day", data.Data);
            else if (nColumnIndex == 4)
                return string.Format("{0:F1} m/sec", data.Data);
            else if (nColumnIndex == 5)
                return string.Format("{0:F1} m/sec", data.Data);

            return "";
        }

        public bool IsValidData(float data, int nColumnIndex)
        {
            if (data < 0.0f)
                return false;

            return true;
        }

        public void EditMode(bool isEditable)
        {
            dataGridView1.AllowUserToAddRows = isEditable;
            dataGridView1.ReadOnly = !isEditable;

            if (!dataGridView1.ReadOnly)
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    row.Cells[0].ReadOnly = true;
                }
            }
        }
    }
}
