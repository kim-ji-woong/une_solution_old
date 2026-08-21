using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace FireManagement
{
    public partial class FormEquipHistory : Form, Ubists.IReaderOwner
    {
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();

        private bool m_isClicked = false;

        private bool m_isWorking = false;
        private FireEquipment m_equip = null;

        public FormEquipHistory()
        {
            InitializeComponent();

            SetGridViewSize();
        }

        public void Hide()
        {
            m_isWorking = false;
            FormMain2.Instance.ViewControl.SetRFIDOwner();
            //FormMain2.Instance.EnableEdit();
            base.Hide();
        }

        public void Show(FireEquipment equipSelected = null)
        {

            m_isWorking = true;
            Reset();

            if (!FormMain2.Instance.IsPCMode)
            {
                FormMain2.Instance.RFIDReader.Owner = this;
                if (!FormMain2.Instance.RFIDReader.StartReading())
                {
                    m_isWorking = false;
                    FormMain2.Instance.ViewControl.SetRFIDOwner();
                    //FormMain2.Instance.EnableEdit();
                    return;
                }
            }

            if (equipSelected != null)
                SetEquipment(equipSelected);

            base.Show();
        }

        private void FormEquipHistory_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_isWorking = false;
            FormMain2.Instance.ViewControl.SetRFIDOwner();
            //FormMain2.Instance.EnableEdit();
        }

        public void OnReadTag(string strTag)
        {
            FireEquipment equip = FormMain2.Instance.DXFManager.FindEquipment(strTag);

            LogManager.Instance.WriteCheckLog(equip);
            SetEquipment(equip);
            m_equip = equip;
        }

        private void AddHistory(int nEquipID)
        {
            ArrayList arrHistory = FormMain2.Instance.IOManager.FindEquipmentHistoryList(nEquipID);
            if (arrHistory == null)
                return;

            foreach (FireEquipmentHistory history in arrHistory)
            {
                AddHistory(history);
            }
        }

        private void AddHistory(FireEquipmentHistory history)
        {
            DataGridViewRow row = new DataGridViewRow();

            row.Height = 45;

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = history.SOPGenUserID.ToString();
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = FireEquipmentHistory.GetStatusText(history.Status);
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = string.Format("{0} {1}:{2}:{3}", history.Time.ToShortDateString(), history.Time.Hour, history.Time.Minute, history.Time.Second);
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = history.CheckersOpinion;
            row.Cells.Add(cell);

            dataGridViewHistory.Rows.Add(row);
        }

        private void Reset()
        {
            textBoxEquipID.Text = "";
            textBoxEquipType.Text = "";
            textBoxRFIDTagID.Text = "";

            dataGridViewHistory.Rows.Clear();
        }

        public void SetEquipment(FireEquipment equip)
        {
            if (m_equip == equip)
                return;

            if (equip == null)
            {
                Reset();
                return;
            }

            textBoxEquipID.Text = equip.EquipID;
            textBoxEquipType.Text = FireEquipment.GetTypeName(equip.Type);
            textBoxRFIDTagID.Text = equip.RFIDTagID;

            dataGridViewHistory.Rows.Clear();
            
            if (equip.ID > 0)
                AddHistory(equip.ID);

            /*if (FormMain2.Instance.DXFManager.EquipmentHistory.ContainsKey(equip))
            {
                FireEquipmentHistory history = FormMain2.Instance.DXFManager.EquipmentHistory[equip];
                if (history.ID < 0)
                    AddHistory(history);
            }*/
        }

        public bool IsWorking
        {
            get { return m_isWorking; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormEquipHistory_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
            }

            m_isClicked = true;
        }

        private void FormEquipHistory_MouseMove(object sender, MouseEventArgs e)
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

        private void FormEquipHistory_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            FormEquipHistory_MouseDown(sender, e);
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            FormEquipHistory_MouseMove(sender, e);
        }

        private void label1_MouseUp(object sender, MouseEventArgs e)
        {
            FormEquipHistory_MouseUp(sender, e);
        }

        private void SetGridViewSize()
        {
            dataGridViewHistory.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewHistory.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewHistory.ColumnHeadersHeight = 30;

            dataGridViewHistory.ForeColor = System.Drawing.Color.FromArgb(1, 1, 1);
            dataGridViewHistory.Font = new Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        }

        private void FormEquipHistory_Load(object sender, EventArgs e)
        {


        }
    }
}
