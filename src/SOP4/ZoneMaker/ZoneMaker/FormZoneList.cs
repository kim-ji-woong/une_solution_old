using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CCTVLocation
{
    public partial class FormZoneList : Form
    {
        private string m_strDataFilePath = "";

        public bool EditMode
        {
            get { return checkBoxEditMode.Checked; }
        }

        public string DataFilePath
        {
            get { return m_strDataFilePath; }
            set { m_strDataFilePath = value; }
        }

        public FormZoneList()
        {
            InitializeComponent();
        }

        private void FormZoneList_Load(object sender, EventArgs e)
        {
            checkBoxEditMode_CheckedChanged(null, null);
        }

        private DataGridViewRow MakeNewRow()
        {
            if (dataGridView1.AllowUserToAddRows)
            {
                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[dataGridView1.Rows.Count - 1].Clone();
                dataGridView1.Rows.Add(row);

                return dataGridView1.Rows[dataGridView1.Rows.Count - 2];
            }
            else
            {
                dataGridView1.AllowUserToAddRows = true;

                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[dataGridView1.Rows.Count - 1].Clone();
                dataGridView1.Rows.Add(row);

                dataGridView1.AllowUserToAddRows = false;
            }

            return dataGridView1.Rows[dataGridView1.Rows.Count - 1];
        }

        private void FormZoneList_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!FormMain.Instance.CloseApplication)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void checkBoxEditMode_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxEditMode.Checked)
            {
                dataGridView1.AllowUserToAddRows = true;
                dataGridView1.ReadOnly = false;

                btnDelete.Enabled = true;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Cells[0].ReadOnly = row.Cells[2].ReadOnly = true;
                }
            }
            else
            {
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.ReadOnly = true;

                btnDelete.Enabled = false;
            }
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (e.RowIndex > 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex - 1];
                row.Cells[0].Value = row.Index + 1;
                row.Cells[0].ReadOnly = row.Cells[2].ReadOnly = true;

                if (row.Tag == null)
                    row.Tag = new OutdoorZone();
            }

            DataGridViewRow rowCurrent = dataGridView1.Rows[e.RowIndex];

            if (!rowCurrent.IsNewRow)
            {
                rowCurrent.Cells[0].Value = rowCurrent.Index + 1;
                rowCurrent.Cells[0].ReadOnly = rowCurrent.Cells[2].ReadOnly = true;

                if (rowCurrent.Tag == null)
                    rowCurrent.Tag = new OutdoorZone();
            }
        }

        public OutdoorZone GetCurrentZone()
        {
            if (dataGridView1.SelectedCells.Count == 0)
                return null;

            int nRowIndex = dataGridView1.SelectedCells[0].RowIndex;

            if (nRowIndex < 0)
                return null;

            DataGridViewRow row = dataGridView1.Rows[nRowIndex];

            if (row.IsNewRow)
                return null;

            OutdoorZone zone = (OutdoorZone)row.Tag;
            return zone;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            if (row.IsNewRow)
                return;

            OutdoorZone zone = (OutdoorZone)row.Tag;

            if (row.Tag == null)
            {
                zone = new OutdoorZone();
                row.Tag = zone;
            }

            if (e.ColumnIndex == 1)
            {
                zone.ZoneName = row.Cells[e.ColumnIndex].Value == null ? "" : row.Cells[e.ColumnIndex].Value.ToString().Trim();
            }
        }

        public void UpdateData(OutdoorZone zone)
        {
            if (zone == null)
                return;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Tag == zone)
                {
                    row.Cells[2].Value = zone.GetBoundaryString();
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (m_strDataFilePath.Length == 0)
            {
                SaveFileDialog dlg = new SaveFileDialog();

                dlg.Filter = "Data Files|*.txt|All FIles|*.*";
                dlg.FilterIndex = 0;
                dlg.Title = "Data 파일 열기";

                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SaveFile(dlg.FileName);
                }
            }
            else
                SaveFile(m_strDataFilePath);
        }

        private void SaveFile(string strFilePath)
        {
            System.IO.StreamWriter writer = new System.IO.StreamWriter(strFilePath, false, Encoding.UTF8);

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                string strZoneName = row.Cells[1].Value == null ? "" : row.Cells[1].Value.ToString();
                string strBoundary = row.Cells[2].Value == null ? "" : row.Cells[2].Value.ToString();

                writer.Write(strZoneName + "\t");
                writer.WriteLine(strBoundary);
            }

            writer.Close();
            m_strDataFilePath = strFilePath;
        }

        public void OpenFile(string strFilePath)
        {
            dataGridView1.Rows.Clear();

            System.IO.StreamReader reader = new System.IO.StreamReader(strFilePath, Encoding.UTF8);

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine();

                if (strLine.Length == 0)
                    continue;

                int nIndex = strLine.IndexOf('\t');

                if (nIndex < 0)
                    continue;

                string strZoneName = strLine.Substring(0, nIndex);
                string strBoundary = strLine.Substring(nIndex + 1);

                DataGridViewRow row = MakeNewRow();

                row.Cells[1].Value = strZoneName;
                row.Cells[2].Value = strBoundary;

                OutdoorZone zone = (OutdoorZone)row.Tag;

                zone.ZoneName = strZoneName;

                if (!ReadBoundary(strBoundary, zone.Vertices))
                {
                    row.Cells[2].Value = null;
                }

                FormMain.Instance.AddOutdoorZone(zone);
            }

            reader.Close();
            m_strDataFilePath = strFilePath;

            if (!this.Visible)
                this.Show();
        }

        private bool ReadBoundary(string strBoundary, List<UnE.Geometry.Vertex2D> vertices)
        {
            string[] strCoords = strBoundary.Split(',');

            int nCoordCount = strCoords.Count();

            for (int j = 0; j < nCoordCount; j += 2)
            {
                try
                {
                    string strX = strCoords[j].Trim();
                    string strY = strCoords[j + 1].Trim();

                    double x, y;

                    if (double.TryParse(strX, out x) && double.TryParse(strY, out y))
                    {
                        UnE.Geometry.Vertex2D vertex = new UnE.Geometry.Vertex2D(x, y);
                        vertices.Add(vertex);
                    }
                }
                catch (System.IndexOutOfRangeException e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                    System.Diagnostics.Trace.WriteLine(j);
                    return false;
                }
            }

            return true;
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (dataGridView1.ReadOnly)
                return;

            if (dataGridView1.SelectedCells.Count == 0)
                return;

            int nRowIndex = dataGridView1.SelectedCells[0].RowIndex;
            DataGridViewRow row = dataGridView1.Rows[nRowIndex];

            if (row.IsNewRow)
                return;

            if (e.KeyCode == Keys.Delete)
            {
                if (row.Tag != null)
                {
                    OutdoorZone zone = (OutdoorZone)row.Tag;
                    FormMain.Instance.RemoveOutdoorZone(zone);
                }
                row.Cells[2].Value = null;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            if (row.IsNewRow || row.Tag == null)
                return;

            OutdoorZone zone = (OutdoorZone)row.Tag;
            FormMain.Instance.SelectOutdoorZone(zone);
        }
    }
}
