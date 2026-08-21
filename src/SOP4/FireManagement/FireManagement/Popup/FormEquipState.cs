using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FireManagement
{
    public partial class FormEquipState : Form
    {
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();


        public FormEquipState()
        {
            InitializeComponent();

            Set_GridView();
        }

        private void Set_GridView()
        {
            gridViewEquipState.ColumnCount = 1;
            gridViewEquipState.RowTemplate.Height = 40;

            gridViewEquipState.Font = new Font("맑은 고딕", 18, FontStyle.Bold);
            gridViewEquipState.Rows.Add("양호");
            gridViewEquipState.Rows.Add("불량/고장");
            gridViewEquipState.Rows.Add("수리중");
            gridViewEquipState.Rows.Add("기타");

            gridViewEquipState.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnComplete_Click(object sender, EventArgs e)
        {
            int nRowIndex = gridViewEquipState.CurrentCell.RowIndex;

            FormMain2.Instance.EquipmentChecker.GetStatusText(nRowIndex);

            this.Dispose();
            this.Close();
        }

        private void gridViewEquipState_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int nRowIndex = gridViewEquipState.CurrentCell.RowIndex;

            FormMain2.Instance.EquipmentChecker.GetStatusText(nRowIndex);

            this.Dispose();
            this.Close();
        }

        private void FormEquipState_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void FormEquipState_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {
                        Point ptCur = this.Location;
                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void FormEquipState_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            FormEquipState_MouseMove(sender, e);
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            FormEquipState_MouseDown(sender, e);
        }

        private void label1_MouseUp(object sender, MouseEventArgs e)
        {
            FormEquipState_MouseUp(sender, e);
        }

        private void FormEquipState_Load(object sender, EventArgs e)
        {

        }


    }
}
