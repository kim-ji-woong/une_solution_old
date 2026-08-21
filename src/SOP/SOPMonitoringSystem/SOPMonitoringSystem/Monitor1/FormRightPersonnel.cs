using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SOPMonitoringSystem
{
    public partial class FormRightPersonnel : Form
    {
        private Dictionary<int, MemberInfo> m_dicAllMembers = null;
        private Dictionary<int, MemberInfo> m_dicNoProcessingMembers = null;
        private Dictionary<int, MemberInfo> m_dicProcessingMembers = null;

        private int m_nCompanyMemberCount = 0;
        private int m_nProcessingCount = 0;
        private int m_nNoProcessingCount = 0;

        private Image m_imgMessage = null;
        private int m_nSelectedIndex = -1;

        public FormRightPersonnel()
        {
            InitializeComponent();

            //InitImage();
            InitPersonnel();
            InitMission();
        }

        private void InitPersonnel()
        {
            ArrayList arrPersonnel = new ArrayList();
            arrPersonnel.Add("총 직원 수");
            arrPersonnel.Add("SOP 전체 리소스");
            arrPersonnel.Add("임무 수행 인원 수");
            arrPersonnel.Add("임무 대기 인원 수");

            foreach (string strValue in arrPersonnel)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = null;

                cell = new DataGridViewTextBoxCell();
                cell.Value = strValue;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = "000 명";
                gridRow.Cells.Add(cell);

                dataGridPersonnel.Rows.Add(gridRow);
            }
        }

        private void InitMission()
        {
            /*ArrayList arrWork = new ArrayList();
            arrWork.Add("000");
            arrWork.Add("000");
            arrWork.Add("000");
            arrWork.Add("000");*/

            m_imgMessage = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.btn_Message);

            /*foreach (string strValue in arrWork)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = null;
                DataGridViewImageCell ImageCell = null;

                cell = new DataGridViewTextBoxCell();
                cell.Value = strValue;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = "000-0000-0000";
                gridRow.Cells.Add(cell);

                ImageCell = new DataGridViewImageCell();
                ImageCell.Value = m_imgMessage;
                gridRow.Cells.Add(ImageCell);

                dataGridNetwork.Rows.Add(gridRow);
            }*/
        }

        private void ReLoadNetwork(Dictionary<int, MemberInfo> dicMembers, bool isClear = true)
        {
            if (isClear)
                dataGridNetwork.Rows.Clear();

            if (dicMembers == null)
                return;

            foreach (KeyValuePair<int, MemberInfo> data in dicMembers)
            {
                MemberInfo member = data.Value;

                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = null;
                DataGridViewImageCell ImageCell = null;

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.Name;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = "000-0000-0000";
                gridRow.Cells.Add(cell);

                ImageCell = new DataGridViewImageCell();
                ImageCell.Value = m_imgMessage;
                gridRow.Cells.Add(ImageCell);

                dataGridNetwork.Rows.Add(gridRow);
            }
        }

        private void ReLoad()
        {
            dataGridPersonnel.Rows[0].Cells[1].Value = string.Format("{0} 명", m_nCompanyMemberCount);
            dataGridPersonnel.Rows[1].Cells[1].Value = string.Format("{0} 명", m_nProcessingCount + m_nNoProcessingCount);
            dataGridPersonnel.Rows[2].Cells[1].Value = string.Format("{0} 명", m_nProcessingCount);
            dataGridPersonnel.Rows[3].Cells[1].Value = string.Format("{0} 명", m_nNoProcessingCount);

            int nSelectedIndex = dataGridNetwork.SelectedRows.Count == 0 ? 0 : dataGridPersonnel.SelectedRows[0].Index;

            if (nSelectedIndex == 0)    // 전체 직원
            {
                 ReLoadNetwork(m_dicAllMembers, true);
            }
            else if (nSelectedIndex == 1)
            {
                ReLoadNetwork(m_dicProcessingMembers, true);
                ReLoadNetwork(m_dicNoProcessingMembers, false);
            }
            else if (nSelectedIndex == 2)
                ReLoadNetwork(m_dicProcessingMembers, true);
            else// if (nSelectedIndex == 3)
                ReLoadNetwork(m_dicNoProcessingMembers, true);
        }

        public void SetMemberInfo(Dictionary<int, MemberInfo> dicAllMembers, Dictionary<int, MemberInfo> dicProcessingMembers, Dictionary<int, MemberInfo> dicNoProcessingMembers)
        {
            m_dicAllMembers = dicAllMembers;
            m_dicProcessingMembers = dicProcessingMembers;
            m_dicNoProcessingMembers = dicNoProcessingMembers;

            int nCompanyMemberCount = m_dicAllMembers == null ? 0 : m_dicAllMembers.Count;
            int nProcessingCount = m_dicProcessingMembers == null ? 0 : m_dicProcessingMembers.Count;
            int nNoProcessingCount = m_dicNoProcessingMembers == null ? 0 : m_dicNoProcessingMembers.Count;

            if (m_nProcessingCount != nProcessingCount || m_nNoProcessingCount != nNoProcessingCount || m_nCompanyMemberCount == 0)
            {
                m_nCompanyMemberCount = nCompanyMemberCount;
                m_nProcessingCount = nProcessingCount;
                m_nNoProcessingCount = nNoProcessingCount;

                ReLoad();
            }
        }

        private void dataGridPersonnel_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (m_nSelectedIndex == e.RowIndex)
                return;

            m_nSelectedIndex = e.RowIndex;

            if (m_nSelectedIndex == 0)    // 전체 직원
            {
                ReLoadNetwork(m_dicAllMembers, true);
            }
            else if (m_nSelectedIndex == 1)
            {
                ReLoadNetwork(m_dicProcessingMembers, true);
                ReLoadNetwork(m_dicNoProcessingMembers, false);
            }
            else if (m_nSelectedIndex == 2)
                ReLoadNetwork(m_dicProcessingMembers, true);
            else// if (m_nSelectedIndex == 3)
                ReLoadNetwork(m_dicNoProcessingMembers, false);
        }
    }
}
