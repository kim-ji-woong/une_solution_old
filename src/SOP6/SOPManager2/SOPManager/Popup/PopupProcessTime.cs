using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPManager
{
    public partial class PopupProcessTime : Form
    {
		protected bool m_bLeftMouseDown = false;
		protected Point m_ptMove = new Point();

        //PropertiesProcess m_propertiesProcess = null;
        //PropertiesLevel m_propertiesLevel = null;

		protected string m_strValue = "";
		public string ProcessTime
		{
			get { return m_strValue; }
			set { m_strValue = value; }
		}

		protected int m_nNumber = 1;
		protected int m_nSelectedUnit = 5;
		public int ProcessTimeType
		{
			get { return m_nSelectedUnit; }
			set { m_nSelectedUnit = value; }
		}      

        public PopupProcessTime()
        {
            InitializeComponent();

            //m_propertiesProcess = FormMain.Instance.GetPageLevel().GetPropertiesProcess();
            //m_propertiesLevel = FormMain.Instance.GetPageLevel().GetPropertiesLevel();

            cboUnit.SelectedIndex = cboUnit.Items.Count - 1;
            cboNumber.Enabled = false;

			m_strValue = "";
            Init();

            UpdateControlSize();
        }

        public void UpdateControlSize()
        {
            Double[] dWindowRate = FormMain.Instance.GetCurWindowRate();
            double WindowRateWidth = dWindowRate[0];
            double WindowRateHeight = dWindowRate[1];

            this.Size = new System.Drawing.Size((int)(this.Size.Width * WindowRateWidth), (int)(this.Size.Height * WindowRateHeight));

            FormMain.Instance.UpdateWindowRate(label4, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(cboNumber, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(cboUnit, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(btnOK, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(btnCancel, WindowRateWidth, WindowRateHeight);

            //foreach (Control ctl in this.Controls)
            //{
            //    HaveControl(ctl, WindowRateWidth, WindowRateHeight);
            //}
        }

        private void HaveControl(Control pctl, double WindowRateWidth, double WindowRateHeight)
        {
            foreach (Control ctl in pctl.Controls)
            {
                if (ctl.Controls.Count > 0)
                    HaveControl(ctl, WindowRateWidth, WindowRateHeight);

                FormMain.Instance.UpdateWindowRate(ctl, WindowRateWidth, WindowRateHeight);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void Init()
        {
            for (int i = 1; i <= 1000; i++)
            {
                cboNumber.Items.Add(i);
            }
            cboNumber.SelectedIndex = 0;
        }

		protected virtual void btnOK_Click(object sender, EventArgs e)
        {
            if (cboUnit.SelectedIndex == cboUnit.Items.Count - 1)
                m_strValue = (string)cboUnit.SelectedItem;
            else
            {
                m_strValue = cboNumber.Text + " " + (string)cboUnit.SelectedItem;
            }

			m_nSelectedUnit = cboUnit.SelectedIndex;
			//m_propertiesProcess.TransTime = m_strValue;
			//m_propertiesProcess.SetTransTime(cboUnit.SelectedIndex);		
		
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

		protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

		protected void cboUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;

            if (cbo.SelectedIndex == cboUnit.Items.Count - 1)
            {
                cboNumber.Enabled = false;
            }
            else
            {
                cboNumber.Enabled = true;
                cboNumber.Items.Clear();
                
                if (cbo.SelectedIndex == 0 || cbo.SelectedIndex == 1)
                {
                    for (int i=1; i<= 100; i++)
                    {
                        cboNumber.Items.Add(i);
                    }

                    if (m_nNumber > 100)
                        m_nNumber = 1;
                    cboNumber.SelectedIndex = m_nNumber - 1;
                }
                else
                {
                    for (int i = 1; i <= 1000; i++)
                    {
                        cboNumber.Items.Add(i);
                    
                    }

                    cboNumber.SelectedIndex = m_nNumber - 1;
                }
            }
        }

		protected void cboNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_nNumber = cboNumber.SelectedIndex + 1;
        }

        public void SetProcessingTime(string strTime)
        {
            if (strTime == null) return;

            string[] strProcessTime = strTime.Split(new char[] { ' ' });
            string[] strOption = { "개월", "주", "일", "시간", "분", "알수없음" };

            int nIndex = 0;

            if (strProcessTime.Length == 1)
            {
                nIndex = 5;
            }
            else
            {
                foreach (string strValue in strOption)
                {
                    if (strValue == strProcessTime[1])
                        break;

                    nIndex++;
                }
            }

            cboUnit.SelectedIndex = nIndex;

            if (nIndex != 5)
            {
                cboNumber.SelectedIndex = int.Parse(strProcessTime[0]) - 1;
                m_nNumber = int.Parse(strProcessTime[0]) - 1;
            }
        }

        public void GetProcessTime(string strTime)
        {
            if (strTime == null) return;

            string[] strProcessTime = strTime.Split(new char[] {' '});

            cboNumber.SelectedIndex = int.Parse(strProcessTime[0]) - 1;
            m_nNumber = int.Parse(strProcessTime[0]) - 1;

            string[] strOption = { "개월", "주", "일", "시간", "분", "알수없음" };
            int nIndex = 0;
            foreach (string strValue in strOption)
            {
                if (strValue == strProcessTime[1])
                    break;

                nIndex++;
            }

            cboUnit.SelectedIndex = nIndex;
        }

		protected void PopupProcessTime_MouseDown(object sender, MouseEventArgs e)
        {
            m_bLeftMouseDown = true;
            m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
        }

		protected void PopupProcessTime_MouseMove(object sender, MouseEventArgs e)
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

        protected void PopupProcessTime_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        }
    }


	public class PopupProcessSOP : PopupProcessTime
	{
		protected override void btnOK_Click(object sender, EventArgs e)
		{
			if (cboUnit.SelectedIndex == cboUnit.Items.Count - 1)
				m_strValue = (string)cboUnit.SelectedItem;
			else
			{
				m_strValue = cboNumber.Text + " " + (string)cboUnit.SelectedItem;
			}

			m_nSelectedUnit = cboUnit.SelectedIndex;

			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}
