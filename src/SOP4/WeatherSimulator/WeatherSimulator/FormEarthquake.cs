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
    public partial class FormEarthquake : Form, IGridOwner<float>
    {
        private TimePickerManager m_timePickerManager = null;
        private GridManager m_gridManager = null;

        public FormEarthquake()
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

            m_gridManager.ColumnIndeces.Add(3);
            m_gridManager.ColumnIndeces.Add(4);
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
                    row.Tag = new Earthquake();
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
                    rowCurrent.Tag = new Earthquake();
            }
        }

        public void LoadData(bool dbData)
        {
            List<Earthquake> earthquakeDatas = null;

            if (dbData)
            {
                ApplyCurrentData();
                earthquakeDatas = DataManager.Instance.EarthquakeDBDatas;
            }
            else
                earthquakeDatas = DataManager.Instance.EarthquakeCurrentDatas;

            dataGridView1.Rows.Clear();

            SetGrid(earthquakeDatas);
        }

        public void ApplyCurrentData()
        {
            List<Earthquake> currentDatas = DataManager.Instance.EarthquakeCurrentDatas;
            currentDatas.Clear();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow || IsNullData(row))
                    continue;

                Earthquake earthquake = (Earthquake)row.Tag;
                SetData(earthquake, row);
                currentDatas.Add(earthquake);
            }
        }

        private bool IsNullData(DataGridViewRow row)
        {
            for (int i = 1; i < dataGridView1.Columns.Count; i++)
            {
                if (row.Cells[i].Value != null)
                    return false;
            }

            return true;
        }

        private void SetData(Earthquake earthquake, DataGridViewRow row)
        {
            if (row.Cells[1].Tag != null)
                earthquake.Time = ((VariousData<DateTime>)row.Cells[1].Tag).Data;
            else
                earthquake.Time = DateTime.Now;

            earthquake.Location = row.Cells[2].Value == null ? null : row.Cells[2].Value.ToString();
            earthquake.Strength = (VariousData<float>)row.Cells[3].Tag;
            earthquake.TsunamiHeight = (VariousData<float>)row.Cells[4].Tag;
            earthquake.Etc = row.Cells[5].Value == null ? null : row.Cells[5].Value.ToString();
        }

        private void SetGrid(List<Earthquake> datas)
        {
            int nIndex = 1;

            foreach (Earthquake earthquake in datas)
            {
                DataGridViewRow row = FormMain.Instance.MakeNewRow(dataGridView1);

                row.Cells[0].Value = nIndex++;
                row.Cells[0].Tag = row.Cells[0].Value;
                row.Cells[0].ReadOnly = true;

                row.Cells[1].Value = earthquake.GetTimeString();
                row.Cells[1].Tag = new VariousData<DateTime>(earthquake.Time);

                if (earthquake.Location != null)
                {
                    row.Cells[2].Value = earthquake.Location;
                    row.Cells[2].Tag = row.Cells[2].Value;
                }

                if (earthquake.Strength != null)
                {
                    row.Cells[3].Value = string.Format("{0:F1}", earthquake.Strength.Data);
                    row.Cells[3].Tag = earthquake.Strength;
                }

                if (earthquake.TsunamiHeight != null)
                {
                    row.Cells[4].Value = string.Format("{0:F1} meter", earthquake.TsunamiHeight.Data);
                    row.Cells[4].Tag = earthquake.TsunamiHeight;
                }

                if (earthquake.Etc != null)
                {
                    row.Cells[5].Value = earthquake.Etc;
                    row.Cells[5].Tag = row.Cells[5].Value;
                }

                row.Cells[0].ReadOnly = true;
                row.Tag = earthquake;
            }
        }

        public string GetCellValueString(VariousData<float> data, int nColumnIndex)
        {
            if (nColumnIndex == 3)
                return string.Format("{0:F1}", data.Data);
            else if (nColumnIndex == 4)
                return string.Format("{0:F1} meter", data.Data);
            
            return "";
        }

        public bool IsValidData(float data, int nColumnIndex)
        {
            if (data < 0.0f)
                return false;

            if (nColumnIndex == 3 && data > 10.0f)
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
