using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RtspUrlEditor
{
    public partial class FormCCTVList : Form
    {
        private const int ID_INDEX = 0;
        private const int NAME_INDEX = 1;
        private const int POSITION_INDEX = 2;

        public FormCCTVList()
        {
            InitializeComponent();
        }

        private void FormCCTVList_Load(object sender, EventArgs e)
        {
            List<CCTV> cctvs = FormMain.Instance.DataManager.CCTVs;

            foreach (CCTV cctv in cctvs)
            {
                AddGrid(cctv);
            }
        }

        private void AddGrid(CCTV cctv)
        {
            int nRowIndex = gridCCTV.Rows.Add();

            if (nRowIndex < 0)
                return;

            DataGridViewRow row = gridCCTV.Rows[nRowIndex];

            row.Cells[ID_INDEX].Value = cctv.ID;
            row.Cells[NAME_INDEX].Value = cctv.CCTVName;
            row.Cells[POSITION_INDEX].Value = cctv.Zone == null ? "" : cctv.Zone.ZoneName;
            row.Tag = cctv;
        }

        private void gridCCTV_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.RowIndex < 0)
                    return;

                DataGridViewRow row = gridCCTV.Rows[e.RowIndex];

                if (row.Tag == null)
                    return;

                CCTV cctv = (CCTV)row.Tag;
                gridCCTV.DoDragDrop(cctv, DragDropEffects.All);
            }
        }

        private void gridCCTV_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(CCTV)))
            {
                e.Effect = DragDropEffects.Move;
            }
        }
    }
}
