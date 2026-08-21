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
    public partial class FormRightDisseminate : Form
    {
        private ArrayList m_arrInside = new ArrayList();
        private ArrayList m_arrOutside = new ArrayList();

        public FormRightDisseminate()
        {
            InitializeComponent();
            InitGridInside();
            InitGridOutside();
        }

        private void InitGridInside()
        {
            m_arrInside.Add("사내 방송");
            m_arrInside.Add("메신저 팝업");
            m_arrInside.Add("앱 전파");

            foreach (string strValue in m_arrInside)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = null;

                cell = new DataGridViewTextBoxCell();
                cell.Value = strValue;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = "";
                gridRow.Cells.Add(cell);

                cell = new DataGridViewCheckBoxCell();
                cell.Value = true;
                gridRow.Cells.Add(cell);

                dataGridInside.Rows.Add(gridRow);
            }
        }

        private void InitGridOutside()
        {
            m_arrOutside.Add("소방서");
            m_arrOutside.Add("경찰서");
            m_arrOutside.Add("지경부 종합상황실");
            m_arrOutside.Add("지경부 전력산업과");
            m_arrOutside.Add("전력거래소");
            m_arrOutside.Add("한국전력");

            foreach (string strValue in m_arrOutside)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = new DataGridViewTextBoxCell();

                cell.Value = strValue;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewCheckBoxCell();
                cell.Value = false;
                gridRow.Cells.Add(cell);

                dataGridOutside.Rows.Add(gridRow);
            }
        }
    }


}
