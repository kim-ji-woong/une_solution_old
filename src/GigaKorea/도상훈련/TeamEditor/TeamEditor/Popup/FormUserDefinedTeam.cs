using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeamEditor.Popup
{
    public partial class FormUserDefinedTeam : Form
    {
        private FormSelectTemporaryMember m_frmOwner = null;

        public UserDefinedTeam SelectedTeam
        {
            get
            {
                if (gridUserDefinedTeam.SelectedCells.Count == 0)
                    return null;

                int nRowIndex = gridUserDefinedTeam.SelectedCells[0].RowIndex;

                if (nRowIndex < 0)
                    return null;

                DataGridViewRow row = gridUserDefinedTeam.Rows[nRowIndex];
                UserDefinedTeam team = (UserDefinedTeam)row.Tag;

                return team;
            }
        }

        public FormUserDefinedTeam(FormSelectTemporaryMember frm, TeamGrid gridSource)
        {
            InitializeComponent();
            m_frmOwner = frm;
            gridUserDefinedTeam.Type = TeamGrid.GridType.UserDefinedTeam;
            Init(gridSource);

            gridUserDefinedTeam.RefreshGrid();

            gridUserDefinedTeam.CellClick += gridUserDefinedTeam_CellClick;
        }

        private void Init(TeamGrid gridSource)
        {
            CopyGrid(gridSource);

            InitColumns();
        }

        private void CopyGrid(TeamGrid gridSource)
        {
            gridUserDefinedTeam.Columns.Clear();

            foreach (DataGridViewColumn column in gridSource.Columns)
            {
                DataGridViewColumn col = column.Clone() as DataGridViewColumn;
                gridUserDefinedTeam.Columns.Add(col);
            }
        }

        private void InitColumns()
        {
            foreach (DataGridViewColumn column in gridUserDefinedTeam.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void gridUserDefinedTeam_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            m_frmOwner.SelectedTeam = null;

            if (gridUserDefinedTeam.SelectedCells.Count > 0)
            {
                int nRowIndex = gridUserDefinedTeam.SelectedCells[0].RowIndex;

                if (nRowIndex < 0)
                    return;

                DataGridViewRow row = gridUserDefinedTeam.Rows[nRowIndex];

                if (row.IsNewRow == false)
                    m_frmOwner.SelectedTeam = row.Tag;
            }
        }

        private void gridUserDefinedTeam_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            TeamGrid gdv = sender as TeamGrid;
            if (gdv == null) return;

            foreach (DataGridViewRow row in gdv.Rows)
            {
                row.MinimumHeight = gdv.RowHeight;
            }
        }

    }
}
