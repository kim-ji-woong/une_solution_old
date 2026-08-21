using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace HSMS
{
    public partial class FormEditEquipGroup : Form
    {
        private ArrayList m_arrGroupNames = new ArrayList();
        private string m_strGroupName = "";
        private string m_strEquipName = "";

        public string EquipName
        {
            get { return m_strEquipName; }
            set { m_strEquipName = value; }
        }

        public string GroupName
        {
            get { return m_strGroupName; }
            set { m_strGroupName = value; }
        }
 
        public FormEditEquipGroup()
        {
            InitializeComponent();
        }

        public void AddGroupName(string strGroupName)
        {
            if (!m_arrGroupNames.Contains(strGroupName))
                m_arrGroupNames.Add(strGroupName);
        }

        private void FormEditEquipGroup_Load(object sender, EventArgs e)
        {
            int nSelectedIndex = -1;

            foreach (string strGroupName in m_arrGroupNames)
            {
                cmbGroupName.Items.Add(strGroupName);

                if (strGroupName == m_strGroupName)
                    nSelectedIndex = cmbGroupName.Items.Count - 1;
            }

            if (cmbGroupName.Items.Count > 0)
            {
                if (nSelectedIndex >= 0)
                    cmbGroupName.SelectedIndex = nSelectedIndex;
                else
                    cmbGroupName.SelectedIndex = 0;
            }

            labelEquipName.Text = m_strEquipName;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            m_strGroupName = cmbGroupName.SelectedItem.ToString();
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }
    }
}
