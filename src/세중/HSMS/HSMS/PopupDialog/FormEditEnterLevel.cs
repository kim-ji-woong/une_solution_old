using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HSMS
{
    public partial class FormEditEnterLevel : Form
    {
        private int m_nEnterLevel = 0;
        private int m_nRowIndex = 0;
        private FormWorker m_frmWorker = null;

        public FormEditEnterLevel(int aEnterLevel, int aRowIndex, FormWorker frmWorker)
        {
            InitializeComponent();

            initComboBox();
            m_nEnterLevel = aEnterLevel;
            m_nRowIndex = aRowIndex;
            m_frmWorker = frmWorker;

            label4.Text = aEnterLevel.ToString() + "등급";
        }

        private void initComboBox()
        {
            cboEnterLevel.Items.Add("1등급");
            cboEnterLevel.Items.Add("2등급");
            cboEnterLevel.Items.Add("3등급");
            cboEnterLevel.Items.Add("4등급");
            cboEnterLevel.Items.Add("5등급");

            switch (m_nEnterLevel)
            {
                case 1: cboEnterLevel.SelectedIndex = 0;
                    break;
                case 2: cboEnterLevel.SelectedIndex = 1;
                    break;
                case 3: cboEnterLevel.SelectedIndex = 2;
                    break;
                case 4: cboEnterLevel.SelectedIndex = 3;
                    break;
                case 5: cboEnterLevel.SelectedIndex = 4;
                    break;
                default: cboEnterLevel.SelectedIndex = 0;
                    break;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            int nSelectedIndex = cboEnterLevel.SelectedIndex;
            int nEditEnterLevel = 1;

            switch (nSelectedIndex)
            {
                case 0: nEditEnterLevel = 1;
                    break;
                case 1: nEditEnterLevel = 2;
                    break;
                case 2: nEditEnterLevel = 3;
                    break;
                case 3: nEditEnterLevel = 4;
                    break;
                case 4: nEditEnterLevel = 5;
                    break;
                default: nEditEnterLevel = 1;
                    break;
            }

            m_frmWorker.EditEnterLevel(m_nRowIndex, nEditEnterLevel);

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
