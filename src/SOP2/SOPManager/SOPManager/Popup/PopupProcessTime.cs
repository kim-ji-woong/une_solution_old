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
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();

        PropertiesProcess m_propertiesProcess = null;
        PropertiesLevel m_propertiesLevel = null;
        string m_strValue = "";

        private int m_nNumber = 0;
        private int m_itemID = 0;
        public int ItemID
        {
            get { return m_itemID; }
            set { m_itemID = value; }
        }

        public PopupProcessTime()
        {
            InitializeComponent();

            m_propertiesProcess = FormMain.Instance.GetPageLevel().GetPropertiesProcess();
            m_propertiesLevel = FormMain.Instance.GetPageLevel().GetPropertiesLevel();

            cboUnit.SelectedIndex = cboUnit.Items.Count - 1;
            cboNumber.Enabled = false;

            Init();
            //InitProcessingTime();
        }

        public void Init()
        {
            for (int i = 1; i <= 1000; i++)
            {
                cboNumber.Items.Add(i);
            }
            cboNumber.SelectedIndex = 0;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cboUnit.SelectedIndex == cboUnit.Items.Count - 1)
                m_strValue = (string)cboUnit.SelectedItem;
            else
            {
                m_strValue = cboNumber.Text + " " + (string)cboUnit.SelectedItem;
            }
 
            switch (m_itemID)
            {
                case ID.ID_ITEM_PROCESSING: // 프로세스 속성
                    m_propertiesProcess.TransTime = m_strValue;
                    m_propertiesProcess.SetTransTime(cboUnit.SelectedIndex);
                    break;
                case ID.ID_ITEM_PROCESS_TIME:
                    m_propertiesLevel.ProcessTime = m_strValue;
                    m_propertiesLevel.ProcessType = cboUnit.SelectedIndex;
                    break;
            }
        

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cboUnit_SelectedIndexChanged(object sender, EventArgs e)
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

        private void cboNumber_SelectedIndexChanged(object sender, EventArgs e)
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

        private void PopupProcessTime_MouseDown(object sender, MouseEventArgs e)
        {
            m_bLeftMouseDown = true;
            m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
        }

        private void PopupProcessTime_MouseMove(object sender, MouseEventArgs e)
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

        private void PopupProcessTime_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

    }
}
