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
    public partial class FormTyphoon : Form, IGridOwner<float>
    {
        private TimePickerManager m_timePickerManager = null;
        private GridManager m_gridManager = null;

        public FormTyphoon()
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
            m_gridManager.ColumnIndeces.Add(5);
            m_gridManager.ColumnIndeces.Add(7);
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
                    row.Tag = new Typhoon();
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
                    rowCurrent.Tag = new Typhoon();
            }
        }

        public void LoadData(bool dbData)
        {
            List<Typhoon> typhoonDatas = null;

            if (dbData)
            {
                ApplyCurrentData();
                typhoonDatas = DataManager.Instance.TyphoonDBDatas;
            }
            else
                typhoonDatas = DataManager.Instance.TyphoonCurrentDatas;

            dataGridView1.Rows.Clear();

            SetGrid(typhoonDatas);
        }

        public void ApplyCurrentData()
        {
            List<Typhoon> currentDatas = DataManager.Instance.TyphoonCurrentDatas;
            currentDatas.Clear();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow || IsNullData(row))
                    continue;

                Typhoon typhoon = (Typhoon)row.Tag;
                SetData(typhoon, row);
                currentDatas.Add(typhoon);
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

        private void SetData(Typhoon typhoon, DataGridViewRow row)
        {
            if (row.Cells[1].Tag != null)
                typhoon.Time = ((VariousData<DateTime>)row.Cells[1].Tag).Data;
            else
                typhoon.Time = DateTime.Now;

            typhoon.CenterLocation = row.Cells[2].Value == null ? null : row.Cells[2].Value.ToString();
            typhoon.CenterPressure = (VariousData<float>)row.Cells[3].Tag;
            typhoon.MaxSpeed = (VariousData<float>)row.Cells[4].Tag;
            typhoon.WindRadius = (VariousData<float>)row.Cells[5].Tag;
            typhoon.WindDirection = GetDirection(row);
            typhoon.MoveSpeed = (VariousData<float>)row.Cells[7].Tag;
            typhoon.Etc = row.Cells[8].Value == null ? null : row.Cells[8].Value.ToString();
        }

        private VariousData<Typhoon.Direction> GetDirection(DataGridViewRow row)
        {
            if (row.Cells[6].Value == null)
                return null;

            int nIndex = colWindDirection.Items.IndexOf(row.Cells[6].Value.ToString());
            Typhoon.Direction dir;

            if (Typhoon.ToDirection(nIndex, out dir))
                return new VariousData<Typhoon.Direction>(dir);

            return null;
        }

        private void SetGrid(List<Typhoon> datas)
        {
            int nIndex = 1;

            foreach (Typhoon typhoon in datas)
            {
                DataGridViewRow row = FormMain.Instance.MakeNewRow(dataGridView1);

                row.Cells[0].Value = nIndex++;
                row.Cells[0].Tag = row.Cells[0].Value;
                row.Cells[0].ReadOnly = true;

                row.Cells[1].Value = typhoon.GetTimeString();
                row.Cells[1].Tag = new VariousData<DateTime>(typhoon.Time);

                if (typhoon.CenterLocation != null)
                {
                    row.Cells[2].Value = typhoon.CenterLocation;
                    row.Cells[2].Tag = row.Cells[2].Value;
                }

                if (typhoon.CenterPressure != null)
                {
                    row.Cells[3].Value = GetCellValueString(typhoon.CenterPressure, 3);
                    row.Cells[3].Tag = typhoon.CenterPressure;
                }

                if (typhoon.MaxSpeed != null)
                {
                    row.Cells[4].Value = GetCellValueString(typhoon.MaxSpeed, 4);
                    row.Cells[4].Tag = typhoon.MaxSpeed;
                }

                if (typhoon.WindRadius != null)
                {
                    row.Cells[5].Value = GetCellValueString(typhoon.WindRadius, 5);
                    row.Cells[5].Tag = typhoon.WindRadius;
                }

                if (typhoon.WindDirection != null)
                {
                    int nDirection = (int)typhoon.WindDirection.Data;
                    row.Cells[6].Value = this.colWindDirection.Items[nDirection];
                    row.Cells[6].Tag = row.Cells[6].Value;
                }

                if (typhoon.MoveSpeed != null)
                {
                    row.Cells[7].Value = GetCellValueString(typhoon.MoveSpeed, 7);
                    row.Cells[7].Tag = typhoon.MoveSpeed;
                }

                if (typhoon.Etc != null)
                {
                    row.Cells[8].Value = typhoon.Etc;
                    row.Cells[8].Tag = row.Cells[8].Value;
                }

                row.Cells[0].ReadOnly = true;
                row.Tag = typhoon;
            }
        }

        public string GetCellValueString(VariousData<float> data, int nColumnIndex)
        {
            if (nColumnIndex == 3)
                return string.Format("{0:F1} hPa", data.Data);
            else if (nColumnIndex == 4)
                return string.Format("{0:F1} m/sec", data.Data);
            else if (nColumnIndex == 5)
                return string.Format("{0:F1} km", data.Data);
            else if (nColumnIndex == 7)
                return string.Format("{0:F1} km/hour", data.Data);

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
