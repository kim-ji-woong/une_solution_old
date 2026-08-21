using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPMonitoringSystem.Popup
{
	public delegate void EndCheckPosition(bool bResult);
	public partial class PopupStartEvent : Form
	{
		public virtual event EndCheckPosition OnCheckPositionEnd;
		private HistoryDiasterPosition mLastPoistion = null;
		public SOPMonitoringSystem.HistoryDiasterPosition LastPoistion
		{
			get { return mLastPoistion; }
			set { mLastPoistion = value; }
		}
		private bool bSendSMS = true;
		public bool UseSMS
		{
			get { return bSendSMS; }
			set { bSendSMS = value; }
		}

		private string disasterName = "";
		public string DisasterName
		{
			get { return disasterName; }
			set { disasterName = value; }
		}

        private DateTime m_dtDetect = new DateTime();
        public DateTime DetectTime
        {
            get { return m_dtDetect; }
        }
	   
		public string PositionName
		{
			get
			{
				return strPosition.Text;
			}
			set
			{
				if (value == "..." || value == "")
				{
					button1.Enabled = false;
                    strPosition.Enabled = false;
				}
				else
				{
                    if (mLastPoistion != null)
                        button1.Enabled = true;

                    strPosition.Enabled = true;
				}
				strPosition.Text = value;
			}
		}

		private ArrayList recentList = null;


		public PopupStartEvent()
		{
			InitializeComponent();
			AdjustLocation(FormMain.Instance.FrmMain2);
			button1.Enabled = false;

            radioAuto.Checked = true;
            labelManualTime.Text = "";
		}

		private void AdjustLocation(Form parent)
		{
			Size size = parent.Size;
			Point p = parent.Location;
			int x = p.X + (size.Width / 2) - (this.Size.Width / 2);
			int y = p.Y + (size.Height / 2) - (this.Size.Height / 2);
			this.Location = new Point(x, y);
		}

	   
		public void SetRecentPosition(ArrayList arList)
		{
			recentList = arList;
			for( int i = 0 ; i < recentList.Count; i++)
			{
				HistoryDiasterPosition pos = (HistoryDiasterPosition)recentList[i];
                pos.DisasterName = disasterName;
				SetRecentPosition(pos.PoistionName);
			}
		}
		
		public void AddLastHistoryDisasterPoistion(HistoryDiasterPosition pos)
		{
			mLastPoistion = pos;
			if (mLastPoistion != null)
				button1.Enabled = true;
		}

		public void SetRecentPosition(string str)
		{
			DataGridViewRow row = new DataGridViewRow();
			DataGridViewCell cell = new DataGridViewTextBoxCell();
			cell.Value = str;
			row.Cells.Add(cell);
			dataGridView1.Rows.Add(row);
		}

		public void btnRunClick(object sender, EventArgs e)
		{
            if (!CheckTimeValidation())
                return;

			if (strPosition.Text == "" || mLastPoistion == null)
				return;

            if (checkBox2.Checked == true)
            {
                UseSMS = true;
            }
            else
            {
                UseSMS = false;
            }

			mLastPoistion.PoistionName = strPosition.Text;

			if (OnCheckPositionEnd != null)
			{
				OnCheckPositionEnd(true);
			}
			
			this.DialogResult = DialogResult.OK;
		}

		private void btnCancelClick(object sender, EventArgs e)
		{			
			if (OnCheckPositionEnd != null)
			{
				OnCheckPositionEnd(false);
			}
			this.DialogResult = DialogResult.Cancel;
		}

		private void checkBox2_CheckedChanged(object sender, EventArgs e)
		{
            if (checkBox2.Checked == true)
            {
                UseSMS = true;
            }
            else
            {
                UseSMS = false;
            }
		}

		private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			int row = e.RowIndex;
			int cell = e.ColumnIndex;
			if (row >= 0)
			{
				string szText = (string)(dataGridView1.Rows[row].Cells[cell].Value);
				PositionName = szText;
				LastPoistion = (HistoryDiasterPosition)recentList[row];
				button1.Enabled = true;
			}            
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			bool bCheck = checkBox1.Checked;
			if (bCheck == true)
			{
				strPosition.Enabled = false;
			}
			else
			{
				strPosition.Enabled = true;
				//mLastPoistion = null;
			}
		}

		private void strPosition_TextChanged(object sender, EventArgs e)
		{
			if (strPosition.Text == "..." || strPosition.Text == "")
			{
				button1.Enabled = false;
			}
			else
			{
				if (mLastPoistion != null)
					button1.Enabled = true;
			}
		}

        private void EnableTimeOptionControls(bool enabled)
        {
            labelManualTime.Visible = enabled;
            btnEditManualTime.Visible = enabled;
        }

        private void radioAuto_CheckedChanged(object sender, EventArgs e)
        {
            EnableTimeOptionControls(false);
        }

        private void radioManual_CheckedChanged(object sender, EventArgs e)
        {
            DateTime dtNow = DateTime.Now;

            if (labelManualTime.Text == "")
            {
                labelManualTime.Text = string.Format("{0}-{1}-{2} {3}:{4}:00",
                    dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute);

                m_dtDetect = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, 0);
            }

            EnableTimeOptionControls(true);
        }

        private bool CheckTimeValidation()
        {
            if (radioAuto.Checked)
                m_dtDetect = DateTime.Now;

            return true;
        }

        private void btnEditManualTime_Click(object sender, EventArgs e)
        {
            PopupDetectTime popup = new PopupDetectTime(m_dtDetect);
            popup.Owner = this;

            if (popup.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_dtDetect = popup.DetectTime;

                labelManualTime.Text = string.Format("{0}-{1}-{2} {3}:{4}:{5}",
                    m_dtDetect.Year, m_dtDetect.Month, m_dtDetect.Day, m_dtDetect.Hour, m_dtDetect.Minute, m_dtDetect.Second);
            }
        }
	}
}
