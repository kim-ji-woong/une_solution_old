using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace DidViewer.Composition
{
    public partial class uEmergency : UserControl
    {
        private EmergencyMode m_mode = EmergencyMode.Fire;
        private ArrayList m_arrShowInfo = null;
        public ArrayList ArrShowInfo
        {
            get { return m_arrShowInfo; }
            set { m_arrShowInfo = value; }
        }

        private Font m_fontTitle = new System.Drawing.Font("나눔바른고딕", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        private Font m_fontData = new System.Drawing.Font("나눔바른고딕", 40F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        private Color m_foreColor = Color.FromArgb(0xf8, 0xfb, 0xff);
        private Color m_lineColor = Color.FromArgb(0xff, 0xdf, 0x90);

        public int ViewCompanyInfoIndex = -1;

        private Brush m_titleBrush = null;

        public uEmergency(EmergencyMode mode, ArrayList arr)
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.Size = new Size(900, 838);

            m_mode = mode;
            m_arrShowInfo = arr;

            if (mode == EmergencyMode.PSM)
            {
                m_lineColor = Color.FromArgb(0xc7, 0xff, 0xe1);                
            }

            foreach (Control item in this.Controls)
            {
                if (item is Label)
                    FormMain.Instance.SetDoubleBuffer(item as Label, true);
            }

            SetColor();

            m_titleBrush = new SolidBrush(m_lineColor);
            SetCompanyData();
        }
        private void SetColor()
        {
            if (m_mode != EmergencyMode.PSM)
                return;
            
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Panel)
                {
                    ctrl.BackColor = m_lineColor;
                }
            }
        }

        public void SetCompanyData()
        {
            int temp = ViewCompanyInfoIndex / 9;
            int beginIndex = temp * 9 * 3;
            //int beginIndex = ViewCompanyInfoIndex * 3;

            if (beginIndex < 0)
            {
                beginIndex = 0;
                ViewCompanyInfoIndex = 0;
            }

            int index = 1;
            for (int i = beginIndex; i < m_arrShowInfo.Count; i+=3)
            {
                string strCompanyName = m_arrShowInfo[i].ToString();
                string strLocation = m_arrShowInfo[i + 1].ToString();
                string strStay = m_arrShowInfo[i + 2].ToString();

                bool chkCompany = false;
                bool chkLocation = false;
                bool chkStay = false;

                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl.Name == "lblCompany" + index)
                    {
                        Label lbl = ctrl as Label;
                        lbl.Text = strCompanyName;

                        chkCompany = true;

                        if (!lbl.Visible)
                            lbl.Visible = true;
                    }
                    else if (ctrl.Name == "lblLocation" + index)
                    {
                        Label lbl = ctrl as Label;
                        lbl.Text = strLocation;

                        chkLocation = true;

                        if (!lbl.Visible)
                            lbl.Visible = true;
                    }
                    else if (ctrl.Name == "lblStay" + index)
                    {
                        Label lbl = ctrl as Label;
                        lbl.Text = strStay;

                        chkStay = true;

                        if (!lbl.Visible)
                            lbl.Visible = true;
                    }

                    if (chkCompany && chkLocation && chkStay)
                        break;
                }
                index++;

                if (index > 9)
                    break;
            }

            ViewCompanyInfoIndex = ViewCompanyInfoIndex + index - 1;
            
            if (index > 9)
                return;

            foreach (Control ctrl in this.Controls)
            {
                if (!ctrl.Name.Contains("lblCompany") && !ctrl.Name.Contains("lblLocation") && !ctrl.Name.Contains("lblStay"))
                    continue;

                for (int i = index; i < 10; i++)
                {
                    if (ctrl.Name == "lblCompany" + i || ctrl.Name == "lblLocation" + i || ctrl.Name == "lblStay" + i)
                    {
                        ctrl.Visible = false;
                    }
                } 
            }
        }

        public void ClearInfo()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (!ctrl.Name.Contains("lblCompany") && !ctrl.Name.Contains("lblLocation") && !ctrl.Name.Contains("lblStay"))
                    continue;

                for (int i = 1; i < 10; i++)
                {
                    if (ctrl.Name == "lblCompany" + i || ctrl.Name == "lblLocation" + i || ctrl.Name == "lblStay" + i)
                    {
                        ctrl.Visible = false;
                        ctrl.Text = "";                        
                    }
                }
            }
        }
        
        private void MakeUI()
        {
            Label lblCompany = CreateLabel(1);
            Label lblStay = CreateLabel(1);
            lblCompany.Location = new Point(312, 17);
            lblStay.Location = new Point(755, 17);
            lblCompany.Text = "업체명";
            lblStay.Text = "잔 류";

            for (int i = 0; i < m_arrShowInfo.Count; i+=2)
            {
                string strCompanyName = m_arrShowInfo[i].ToString();
                string strStay = m_arrShowInfo[i + 1].ToString();
                                
                Label lblCompanyData = CreateLabel(2);
                Label lblStayData = CreateLabel(2);

                lblCompanyData.BackColor = Color.Red;
                lblStayData.BackColor = Color.Pink;

                lblCompanyData.Text = strCompanyName;
                lblStayData.Text = strStay;

                lblCompanyData.Location = new Point(10, 70);
                lblCompanyData.Size = new Size(760, 57);

                lblStayData.Location = new Point(795, 70);
                lblStayData.Size = new Size(200, 57);

                Panel line = new Panel();
                line.Size = new Size(850, 3);
                line.Location = new Point(0, 140);
                line.BackColor = m_lineColor;

                this.Controls.Add(lblCompanyData);
                this.Controls.Add(lblStayData);
                this.Controls.Add(line);
            }

            this.Controls.Add(lblCompany);
            this.Controls.Add(lblStay);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">1:업체명,잔류 2:데이터</param>
        private Label CreateLabel(int type)
        {
            Label label = new Label();
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.AutoSize = false;
            label.BackColor = Color.Transparent;
            if (type == 1)
            {
                label.ForeColor = m_lineColor;
                label.Font = m_fontTitle;
            }
            else
            {
                label.ForeColor = m_foreColor;
                label.Font = m_fontData;
            }

            return label;
        }

        private void uEmergency_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawString("업체명", m_fontTitle, m_titleBrush, 149, 12);
            g.DrawString("위 치", m_fontTitle, m_titleBrush, 500, 12);
            g.DrawString("잔 류", m_fontTitle, m_titleBrush, 755, 12);
        }
    }
}
