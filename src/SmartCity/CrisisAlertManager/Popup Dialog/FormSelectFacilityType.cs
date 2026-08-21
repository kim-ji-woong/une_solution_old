using CrisisAlertManager.Data;
using CrisisAlertManager.Group;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrisisAlertManager.Popup_Dialog
{
    public partial class FormSelectFacilityType : Form
    {
        private uFormGroup m_uFormGroup = null;

        private bool m_bFireState = false;
        private bool m_bFloodState = false;
        private bool m_bHeatState = false;
        private bool m_bCollapseState = false;

        public FormSelectFacilityType(string strSelectFacilityType, uFormGroup parent)
        {
            InitializeComponent();

            Region = System.Drawing.Region.FromHrgn(FormMain.CreateRoundRectRgn(0, 0, this.Width, this.Height, 35, 35));

            m_uFormGroup = parent;

            if (strSelectFacilityType == null || strSelectFacilityType == "")
                return;

            string[] arrFacilityType = strSelectFacilityType.Split(',');
            int nCount = arrFacilityType.Length;

            for (int i = 0; i < nCount; i++)
            {
                string strFacilityType = arrFacilityType[i];

                if (strFacilityType.Contains(CommonString.FacilityType_Fire_Kor))
                {
                    m_bFireState = true;

                    RefreshFire();
                }
                else if (strFacilityType.Contains(CommonString.FacilityType_Flood_Kor))
                {
                    m_bFloodState = true;

                    RefreshFlood();
                }
                else if (strFacilityType.Contains(CommonString.FacilityType_Heat_Kor))
                {
                    m_bHeatState = true;

                    RefreshHeat();
                }
                else if (strFacilityType.Contains(CommonString.FacilityType_Collapse_Kor))
                {
                    m_bCollapseState = true;

                    RefreshCollapse();
                }
            }
        }

        #region 폼 이동
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();
        private bool m_isClicked = false;
        private Point m_ptOrigin = new Point();

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = Control.MousePosition;
                m_ptOrigin = this.Location;
            }

            m_isClicked = true;
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_isClicked)
                return;

            if (!m_bLeftMouseDown)
                return;

            Point ptScreen = Control.MousePosition;

            int dx = ptScreen.X - m_ptMove.X;
            int dy = ptScreen.Y - m_ptMove.Y;

            if (dx == 0 && dy == 0)
                return;

            Point ptCur = this.Location;
            this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
            m_ptMove.X += dx;
            m_ptMove.Y += dy;
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;

            m_isClicked = false;
        }
        #endregion

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            string strFacilityType = "";

            if (m_bFireState == true)
                strFacilityType += CommonString.FacilityType_Fire_Kor;

            if (m_bFloodState == true && strFacilityType.Length == 0)
                strFacilityType += CommonString.FacilityType_Flood_Kor;
            else if (m_bFloodState == true)
                strFacilityType += ", " + CommonString.FacilityType_Flood_Kor;

            if (m_bHeatState == true && strFacilityType.Length == 0)
                strFacilityType += CommonString.FacilityType_Heat_Kor;
            else if (m_bHeatState == true)
                strFacilityType += ", " + CommonString.FacilityType_Heat_Kor;

            if (m_bCollapseState == true && strFacilityType.Length == 0)
                strFacilityType += CommonString.FacilityType_Collapse_Kor;
            else if (m_bCollapseState == true)
                strFacilityType += ", " + CommonString.FacilityType_Collapse_Kor;

            m_uFormGroup.FacilityType = strFacilityType;

            this.DialogResult = DialogResult.Yes;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }

        private void btnFire_Click(object sender, EventArgs e)
        {
            m_bFireState = !m_bFireState;

            RefreshFire();
        }

        private void RefreshFire()
        {
            if (m_bFireState == true)
            {
                btnFire.ImageNormal = global::CrisisAlertManager.Properties.Resources.Checkbox_Click;
                btnFire.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.Checkbox_Click;
            }
            else
            {
                btnFire.ImageNormal = global::CrisisAlertManager.Properties.Resources.Checkbox_Normal;
                btnFire.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.Checkbox_Normal;
            }

            btnFire.Refresh();
        }

        private void btnFlood_Click(object sender, EventArgs e)
        {
            m_bFloodState = !m_bFloodState;

            RefreshFlood();
        }

        private void RefreshFlood()
        {
            if (m_bFloodState == true)
            {
                btnFlood.ImageNormal = global::CrisisAlertManager.Properties.Resources.Checkbox_Click;
                btnFlood.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.Checkbox_Click;
            }
            else
            {
                btnFlood.ImageNormal = global::CrisisAlertManager.Properties.Resources.Checkbox_Normal;
                btnFlood.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.Checkbox_Normal;
            }

            btnFlood.Refresh();
        }

        private void btnHeat_Click(object sender, EventArgs e)
        {
            m_bHeatState = !m_bHeatState;

            RefreshHeat();
        }

        private void RefreshHeat()
        {
            if (m_bHeatState == true)
            {
                btnHeat.ImageNormal = global::CrisisAlertManager.Properties.Resources.Checkbox_Click;
                btnHeat.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.Checkbox_Click;
            }
            else
            {
                btnHeat.ImageNormal = global::CrisisAlertManager.Properties.Resources.Checkbox_Normal;
                btnHeat.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.Checkbox_Normal;
            }

            btnHeat.Refresh();
        }

        private void btnCollapse_Click(object sender, EventArgs e)
        {
            m_bCollapseState = !m_bCollapseState;

            RefreshCollapse();
        }

        private void RefreshCollapse()
        {
            if (m_bCollapseState == true)
            {
                btnCollapse.ImageNormal = global::CrisisAlertManager.Properties.Resources.Checkbox_Click;
                btnCollapse.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.Checkbox_Click;
            }
            else
            {
                btnCollapse.ImageNormal = global::CrisisAlertManager.Properties.Resources.Checkbox_Normal;
                btnCollapse.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.Checkbox_Normal;
            }

            btnCollapse.Refresh();
        }
    }
}
