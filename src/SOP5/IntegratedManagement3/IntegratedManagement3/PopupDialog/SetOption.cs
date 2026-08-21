using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntegratedManagement3.PopupDialog
{
    public partial class SetOption : Form
    { 
        private LoginManager m_logInMgr = null;

        private int m_nCompanyMemberID = -1;
        public int ComapnyMember
        {
            get { return m_nCompanyMemberID; }
        }

        private String m_memberID = "";
        public String MemberID
        {
            get
            {
                return m_memberID;
            }
            set
            {
                m_memberID = value;                
            }
        }

        private String m_memberName = "";
        public String MemberName
        {
            get
            {
                return m_memberName;
            }
            set
            {
                m_memberName = value;
            }
        }

        private bool m_bLeftMouseDown = false;
        private Point m_ptMove;

        private double m_WindowRateWidth = 1d;
        public double WindowRateWidth
        {
            get { return m_WindowRateWidth; }
            set { m_WindowRateWidth = value; }
        }

        private double m_WindowRateHeight = 1d;
        public double WindowRateHeight
        {
            get { return m_WindowRateHeight; }
            set { m_WindowRateHeight = value; }
        }

        public SetOption(LoginManager logInMgr, String p_memberID = "", String p_memberName = "")
        {
            InitializeComponent();

            m_logInMgr = logInMgr;

            MemberID = p_memberID;            
            MemberName = p_memberName;
            if (MemberID != "")
                textBoxMemberID.Text = MemberID;

            if (MemberName != "")
                textBoxMemberName.Text = MemberName;
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            MemberID = "";
            MemberName = "";
            this.DialogResult = System.Windows.Forms.DialogResult.No;
        }

        public void UpdateControl()
        {
            FormMain.Instance.UpdateWindowRate(this, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(label1, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(panel1, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(btn_cancel, WindowRateWidth, WindowRateHeight);

            FormMain.Instance.UpdateWindowRate(labelMemberID, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(textBoxMemberID, WindowRateWidth, WindowRateHeight);

            FormMain.Instance.UpdateWindowRate(labelMemberName, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(textBoxMemberName, WindowRateWidth, WindowRateHeight);

            FormMain.Instance.UpdateWindowRate(btn_ok, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(ribbonButton2, WindowRateWidth, WindowRateHeight);
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            try
            {
                string strGenUserID = "";
                int nCompanyMemberID = m_logInMgr.GetMemberID(textBoxMemberID.Text, textBoxMemberName.Text, ref strGenUserID);
                if (nCompanyMemberID == -2)
                {
                    throw new ApplicationException("삭제된 직원이거나 직원 정보가 잘못되었습니다.");
                }
                else if (nCompanyMemberID < 0)
                {
                    throw new ApplicationException("입력된 직원 정보가 잘못되었습니다.");
                }
                else if (nCompanyMemberID == 0)
                {
                    throw new ApplicationException("이미 회원가입이 되어 있습니다.");
                }
                else
                {
                    labelMemberID.Tag = nCompanyMemberID;

                    m_nCompanyMemberID = nCompanyMemberID;

                    MemberID = textBoxMemberID.Text;
                    MemberName = textBoxMemberName.Text;

                    this.DialogResult = System.Windows.Forms.DialogResult.Yes;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                UnE.Utility.UMessageBoxRibbon.Show(ex.Message, "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SetOption_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void SetOption_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point pt = PointToScreen(new Point(e.X, e.Y));
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

        private void SetOption_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_bLeftMouseDown = false;
        }
    }    
}
