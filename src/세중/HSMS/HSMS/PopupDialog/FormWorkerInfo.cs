using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HSMS.PopupDialog
{
    public partial class FormWorkerInfo : Form
    {
        private DataWorker m_worker = null;

        public DataWorker Worker
        {
            get { return m_worker; }
            set { SetWorker(value); }
        }

        public FormWorkerInfo()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void SetWorker(DataWorker worker)
        {
            if (m_worker == worker)
                return;

            m_worker = worker;

            if (m_worker == null)
            {
                labelWorkerName.Text = "";
                textBoxWorkerNumber.Text = "";
                textBoxDepartment.Text = "";
                textBoxTeam.Text = "";
                textBoxJobPosition.Text = "";
                textBoxSensorID.Text = "";
                textBoxPhoneNumber.Text = "";
                textBoxCellPhoneNumber.Text = "";
                textBoxLevel.Text = "";
            }
            else
            {
                labelWorkerName.Text = m_worker.Name;
                textBoxWorkerNumber.Text = m_worker.MemberID;

                if (m_worker.Company != null)
                    textBoxDepartment.Text = m_worker.Company.CompanyName;
                else
                    textBoxDepartment.Text = "";

                if (m_worker.Team != null)
                    textBoxTeam.Text = m_worker.Team.Name;
                else
                    textBoxTeam.Text = "";

                if (m_worker.JobPosition != null)
                    textBoxJobPosition.Text = m_worker.JobPosition.Name;
                else
                    textBoxJobPosition.Text = "";

                textBoxSensorID.Text = m_worker.Sensor;
                textBoxPhoneNumber.Text = m_worker.OfficePhoneNumber;
                textBoxCellPhoneNumber.Text = m_worker.MobilePhoneNumber;
                textBoxLevel.Text = m_worker.EnterLevel.ToString() + "등급";
            }

            this.Refresh();
        }
    }
}
