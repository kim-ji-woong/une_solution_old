using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SDMS
{
    public partial class FormReciverState : Form
    {
        public enum MessageType { FACILITY_FAULT = 0, DETECT_FIRE };

        public FormReciverState()
        {
            InitializeComponent();
        }


        private void FormReciverState_Load(object sender, EventArgs e)
        {
            LoadDB();
            ArrayList arRecivers = ReciverManager.Instance.GetReciverList();

            foreach (Reciver reciver in arRecivers)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.Tag = reciver;

                DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
                cell1.Value = reciver.Place;

                row.Cells.Add(cell1);
                
                DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                cell2.Value = reciver.Address;
                row.Cells.Add(cell2);

                DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                row.Cells.Add(cell3);
                if( reciver.State == 1) 
                {
                    cell3.Value = "접속중";
                    cell3.Style.ForeColor = Color.Green;
                }
                else
                {
                    cell3.Value = "접속해제";
                    cell3.Style.ForeColor = Color.Red;
                }

                gridRecivers.Rows.Add(row);
            }

            timer1.Interval = 3000;
            timer1.Enabled = true;
            timer1.Start();
        }

        private void LoadDB()
        {  
		}
        
  
        private void btnOK_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            DialogResult = System.Windows.Forms.DialogResult.OK;
			
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateReciverState();
        }

        private void UpdateReciverState()
        {
			ArrayList arRecivers = ReciverManager.Instance.GetReciverList();
            foreach (Reciver reciver in arRecivers)
            {
                foreach (DataGridViewRow row in gridRecivers.Rows)
                {
                    Reciver recRow = (Reciver)row.Tag;
                    if (recRow.ID == reciver.ID)
                    {
                        if (reciver.State == 1)
                        {
                            row.Cells[2].Value = "접속중";
                            row.Cells[2].Style.ForeColor = Color.Green;
                        }
                        else
                        {
                            row.Cells[2].Value = "접속해제";
                            row.Cells[2].Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }
    }
}
