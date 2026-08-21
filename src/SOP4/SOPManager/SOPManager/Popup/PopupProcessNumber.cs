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
    public partial class PopupProcessNumber : Form
    {
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();


		private string m_szProcessNubmer = "";
		public string ProcessNubmer
		{
			get { return m_szProcessNubmer; }
			set 
			{
				m_szProcessNubmer = value;
				SetNumberType(m_szProcessNubmer);
			}
		}
		
		private int m_nNumberType = 0;
		public int NumberType
		{
			get { return m_nNumberType; }
			set 
			{
				m_nNumberType = value;
				
			}
		}

        public PopupProcessNumber()
        {
            InitializeComponent();
            
            Init();
            cboOption.SelectedIndex = cboOption.Items.Count - 1;
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

        private void Init()
        {
            for (int i = 1; i <= 1000; i++ )
            {
                cboNumber.Items.Add(i);
            }
            cboNumber.SelectedIndex = 0;
        }

		private void PopupProcessNumber_Load(object sender, EventArgs e)
		{
			
		}

        private void btnOK_Click(object sender, EventArgs e)
        {
			m_szProcessNubmer = cboOption.Text + " " + cboNumber.Text + "회";
			m_nNumberType = cboOption.SelectedIndex;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cboOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (cboOption.SelectedIndex == cboOption.Items.Count - 1)
            //    cboNumber.Enabled = false;
            //else
            //    cboNumber.Enabled = true;
        }

        public int SetNumberType(string strNumberType)
        {
            if (strNumberType == null) return -1;
            string[] str = strNumberType.Split(new char[] { ' ' });
            string strNum = System.Text.RegularExpressions.Regex.Replace(str[1], @"\D", "");
            
            cboNumber.SelectedIndex = int.Parse(strNum) - 1;

            string[] strOption = { "전체기간중", "연중", "월중", "주중", "하루중", "시간당" };
            int nIndex = 0;
            foreach(string strValue in strOption)
            {
                if(strValue == str[0])
                    break;
                    
                nIndex++;
            }

            cboOption.SelectedIndex = nIndex;
            
            return nIndex;
        }

        private void PopupProcessNumber_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void PopupProcessNumber_MouseMove(object sender, MouseEventArgs e)
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

        private void PopupProcessNumber_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

		
        
    }
}
