using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryReader
{
    public partial class FormSelectLibrary : Form
    {
        private Library m_libSelected = null;
        private string m_strFirstColumnName = "ID";
        private string m_strSecondColumnName = "도서관 이름";
        private bool m_visibleHelpString = true;

        public string FirstColumnName
        {
            get { return m_strFirstColumnName; }
            set { m_strFirstColumnName = value; }
        }

        public string SecondColumnName
        {
            get { return m_strSecondColumnName; }
            set { m_strSecondColumnName = value; }
        }

        public bool VisibleHelpString
        {
            get { return m_visibleHelpString; }
            set { m_visibleHelpString = value; }
        }

        public Library SelectedLibrary
        {
            get { return m_libSelected; }
        }

        public FormSelectLibrary(int nID, string strName, List<Library> libraries)
        {
            InitializeComponent();

            labelID.Text = nID.ToString();
            labelName.Text = strName;

            SetGrid(libraries);
        }

        public FormSelectLibrary(int nID, string strName, Dictionary<string, string> dicLibraries)
        {
            InitializeComponent();

            labelID.Text = nID.ToString();
            labelName.Text = strName;

            SetGrid(dicLibraries);
        }

        private void SetGrid(List<Library> libraries)
        {
            foreach (Library lib in libraries)
            {
                DataGridViewRow row = new DataGridViewRow();

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = lib.ID;
                row.Cells.Add(cell);

                string strName = lib.Name;

                if (lib.Addr1 != null && lib.Addr1.Length > 0)
                {
                    strName += " " + lib.Addr1;

                    if (lib.Addr2 != null && lib.Addr2.Length > 0)
                        strName += " " + lib.Addr2;
                }

                cell = new DataGridViewTextBoxCell();
                cell.Value = strName;
                row.Cells.Add(cell);

                dataGridView1.Rows.Add(row);
                row.Tag = lib;
            }
        }

        private void SetGrid(Dictionary<string, string> dicLibraries)
        {
            foreach (KeyValuePair<string, string> pair in dicLibraries)
            {
                DataGridViewRow row = new DataGridViewRow();

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = pair.Key;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = pair.Value;
                row.Cells.Add(cell);

                dataGridView1.Rows.Add(row);
                row.Tag = pair.Value;
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count == 0)
            {
                MessageBox.Show("도서관을 선택하세요.");
                return;
            }

            int nRowIndex = dataGridView1.SelectedCells[0].RowIndex;
            //m_libSelected = (Library)dataGridView1.Rows[nRowIndex].Tag;

            object obj = dataGridView1.Rows[nRowIndex].Tag;

            if (obj is Library)
                m_libSelected = (Library)obj;
            else if (obj is string)
            {
                Library lib = new Library();
                lib.Coord = (string)obj;
                m_libSelected = lib;
            }
            else
                m_libSelected = null;

            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            m_libSelected = null;
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void FormSelectLibrary_Load(object sender, EventArgs e)
        {
            colID.Name = m_strFirstColumnName;
            colName.Name = m_strSecondColumnName;

            if (!m_visibleHelpString)
            {
                labelID.Visible = false;
                labelName.Visible = false;
            }
        }
    }
}
