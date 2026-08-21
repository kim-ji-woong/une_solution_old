using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DBUtility2;
using SDMS.Help;
using UnE.GUI;
using UnE.Sensor;

namespace SDMS
{
    public partial class FormReciverState : PopupFormBase
	{
		public enum MessageType { FACILITY_FAULT = 0, DETECT_FIRE };

        private ManualManager m_manualManager = null;

		public FormReciverState()
		{
			InitializeComponent();

            this.DoubleBuffered = true;
            FormMain.SetDoubleBuffer(gridRecivers, true);
             
            InitCtrlSize(this);
            //SetChildCtrlResize(this, this.Width, this.Height);

            FormMain.Instance.CustomizeGridView(gridRecivers);

            m_manualManager = new ManualManager(this);
            SetManualID();
		}

		private void FormReciverState_Load(object sender, EventArgs e)
		{ 
			LoadDB();
			ArrayList arRecivers = ReciverManager.Instance.GetReciverList();

            int nTemp;

			foreach (Reciver reciver in arRecivers)
			{
				DataGridViewRow row = new DataGridViewRow();
				row.Tag = reciver;

                string strReceiverType = reciver.Type.ToString();

                if (int.TryParse(strReceiverType, out nTemp))
                {
                    // 정의되지 않은 수신반 타입
                    strReceiverType = "수신반";
                }

                DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                cell4.Value = strReceiverType;
                row.Cells.Add(cell4);

				DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
				cell1.Value = reciver.Place;

				row.Cells.Add(cell1);

				DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
				cell2.Value = reciver.Address;
				row.Cells.Add(cell2);

				DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
				row.Cells.Add(cell3);

                if (reciver.Type == Reciver.ReciverType.유해물질수신반)
                {
                    if (reciver.State == 1)
                    {
                        cell3.Value = "접속중";
                        cell3.Style.ForeColor = Color.Green;
                    }
                    else
                    {
                        cell3.Value = "접속해제";
                        cell3.Style.ForeColor = Color.Red;
                    }
                }
                else
                {
                    if (reciver.State == 1)
                    {
                        cell3.Value = "통신준비";
                        cell3.Style.ForeColor = Color.Orange;
                    }
                    if (reciver.State == 11)
                    {
                        cell3.Value = "접속중";
                        cell3.Style.ForeColor = Color.Green;
                    }
                    else
                    {
                        cell3.Value = "접속해제";
                        cell3.Style.ForeColor = Color.Red;
                    }
                }
				

				gridRecivers.Rows.Add(row);
			}

            ReadGridWidth(FormMain.Instance.DBManager, UnE.SOP.ProxySOP.Instance.SiteID);

			timer1.Interval = 3000;
			timer1.Enabled = true;
			timer1.Start();
		}

        private void ReadGridWidth(WebDBManager dbMgr, int nSiteID)
        {
            string strSQL = "Select PropertyValue from OptionSDMS where PropertyName = 'ReceiverStateGridWidth' and SiteID = " + nSiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return;

            string strValue = WebDBManager.GetStringField(arrResult[0]);

            if (strValue == null)
                return;

            string[] tokens = strValue.Split(';');

            int nWidth = 0;
            int nTokenCount = tokens.Length;
            int nColumnCount = gridRecivers.Columns.Count;

            for (int i=0;i<nTokenCount && i<nColumnCount;i++)
            {
                if (int.TryParse(tokens[i].Trim(), out nWidth) == false)
                    continue;

                gridRecivers.Columns[i].Width = nWidth;
            }
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
                        if (reciver.Type == Reciver.ReciverType.유해물질수신반)
                        {
                            if (reciver.State == 1)
                            {
                                row.Cells[3].Value = "접속중";
                                row.Cells[3].Style.ForeColor = Color.Green;
                            }
                            else
                            {
                                row.Cells[3].Value = "접속해제";
                                row.Cells[3].Style.ForeColor = Color.Red;
                            }
                        }
                        else
                        {
                            if (reciver.State == 1)
                            {
                                row.Cells[3].Value = "통신준비";
                                row.Cells[3].Style.ForeColor = Color.Orange;
                            }
                            else if (reciver.State == 11)
                            {
                                row.Cells[3].Value = "접속중";
                                row.Cells[3].Style.ForeColor = Color.Green;
                            }
                            else
                            {
                                row.Cells[3].Value = "접속해제";
                                row.Cells[3].Style.ForeColor = Color.Red;
                            }
                        }

					}
				}
			}
		}

        private void SetManualID()
        {
            m_manualManager.Handle = this.Handle;

            m_manualManager.Clear();

            m_manualManager.SetID(this, "센서수신반");
            m_manualManager.ProcessEvent();
        }
	}
}