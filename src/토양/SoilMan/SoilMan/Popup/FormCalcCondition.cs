using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoilMan.Popup
{
    public partial class FormCalcCondition : Form
    {
        private Form m_frmCurrent = null;

        public FormCalcCondition()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            FormSetArea frmArea = new FormSetArea();
            frmArea.TopLevel = false;
            panelBody.Controls.Add(frmArea);
            frmArea.Dock = DockStyle.Fill;
            frmArea.Show();

            FormConfirmArea frmConfirm = new FormConfirmArea();
            frmConfirm.TopLevel = false;
            panelBody.Controls.Add(frmConfirm);
            frmConfirm.Dock = DockStyle.Fill;

            FormInputCondition frmCondition = new FormInputCondition();
            frmCondition.TopLevel = false;
            panelBody.Controls.Add(frmCondition);
            frmCondition.Dock = DockStyle.Fill;

            m_frmCurrent = frmArea;

            btnPrev.Location = btnNext.Location;
            btnNext.Location = btnResult.Location;
            btnResult.Visible = false;
        }

        private void FormCalcCondition_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!FormMain.Instance.CloseApplication)
            {
                e.Cancel = true;
                FormMain.Instance.EnableCheckValueButton();
                this.Hide();

                FormMain.Instance.SelectionManager.ClearAllSelection();
                FormMain.Instance.NoOverlayDrawing();
                FormMain.Instance.DxfControl._Refresh();
            }
        }

        private void FormCalcCondition_Load(object sender, EventArgs e)
        {
        }

        public void Show()
        {
            FormMain.Instance.OverlayPainter.DrawType = GetDrawingType();
            base.Show();
        }

        public void Show(IWin32Window owner)
        {
            FormMain.Instance.OverlayPainter.DrawType = GetDrawingType();
            FormMain.Instance.OverlayPainter.SetSelectArea();
            base.Show(owner);
        }

        private Overlay.OverlayPainter.DrawingType GetDrawingType()
        {
            if (m_frmCurrent == null)
                return Overlay.OverlayPainter.DrawingType.NONE;
            else if (m_frmCurrent is FormSetArea)
            {
                FormSetArea frm = (FormSetArea)m_frmCurrent;
                return frm.DrawingType;
            }

            return Overlay.OverlayPainter.DrawingType.NONE;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (m_frmCurrent == null)
                return;

            if (m_frmCurrent is FormSetArea)
            {
                Dictionary<LandType, Overlay.AreaNCost> dicLandTypeAreas = FormMain.Instance.OverlayPainter.GetSelectedAreas();

                if (dicLandTypeAreas == null)
                    return;

                MoveToConfirmArea(dicLandTypeAreas);
            }
            else if (m_frmCurrent is FormConfirmArea)
            {
                Dictionary<LandType, Overlay.AreaNCost> dicLandTypeAreas = FormMain.Instance.OverlayPainter.GetSelectedAreas();

                if (dicLandTypeAreas == null)
                    return;

                MoveToInputCondition(dicLandTypeAreas);
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (m_frmCurrent == null)
                return;

            if (m_frmCurrent is FormConfirmArea)
            {
                MoveToSetArea();
            }
            else if (m_frmCurrent is FormInputCondition)
            {
                Dictionary<LandType, Overlay.AreaNCost> dicLandTypeAreas = FormMain.Instance.OverlayPainter.GetSelectedAreas();

                if (dicLandTypeAreas == null)
                    return;

                MoveToConfirmArea(dicLandTypeAreas);
            }
            
        }

        private void MoveToConfirmArea(Dictionary<LandType, Overlay.AreaNCost> dicLandTypeAreas)
        {
            if (m_frmCurrent != null)
            {
                m_frmCurrent.Hide();
                m_frmCurrent = null;
            }

            foreach (Control ctrl in panelBody.Controls)
            {
                if (ctrl is FormConfirmArea)
                {
                    m_frmCurrent = (FormConfirmArea)ctrl;
                    break;
                }
            }

            if (m_frmCurrent == null)
                return;

            btnPrev.Visible = true;
            btnNext.Visible = true;
            btnResult.Visible = false;

            ((FormConfirmArea)m_frmCurrent).SetLandTypeInfo(dicLandTypeAreas);
            m_frmCurrent.Show();
        }

        private void MoveToSetArea()
        {
            if (m_frmCurrent != null)
            {
                m_frmCurrent.Hide();
                m_frmCurrent = null;
            }

            foreach (Control ctrl in panelBody.Controls)
            {
                if (ctrl is FormSetArea)
                {
                    m_frmCurrent = (FormSetArea)ctrl;
                    break;
                }
            }

            if (m_frmCurrent == null)
                return;

            btnPrev.Visible = false;
            btnNext.Visible = true;
            btnResult.Visible = false;

            m_frmCurrent.Show();
        }

        private void MoveToInputCondition(Dictionary<LandType, Overlay.AreaNCost> dicLandTypeAreas)
        {
            if (m_frmCurrent != null)
            {
                m_frmCurrent.Hide();
                m_frmCurrent = null;
            }

            foreach (Control ctrl in panelBody.Controls)
            {
                if (ctrl is FormInputCondition)
                {
                    m_frmCurrent = (FormInputCondition)ctrl;
                    break;
                }
            }

            if (m_frmCurrent == null)
                return;

            btnPrev.Visible = true;
            btnNext.Visible = false;
            btnResult.Visible = true;

            ((FormInputCondition)m_frmCurrent).SetLandTypeInfo(dicLandTypeAreas);

            
            m_frmCurrent.Show();
        }

        private void btnResult_Click(object sender, EventArgs e)
        {
            if (m_frmCurrent is FormInputCondition)
            {
                FormInputCondition frm = (FormInputCondition)m_frmCurrent;

                if (!frm.CheckNullData())
                    return;

                FormConfirmArea frmArea = null;

                foreach (Control ctrl in panelBody.Controls)
                {
                    if (ctrl is FormConfirmArea)
                    {
                        frmArea = (FormConfirmArea)ctrl;
                        break;
                    }
                }

                FormMain.Instance.ShowResult(frm.SelectedTechType, frm.InheritanceValue, frm.ExistanceValue, frm.BioValue, frm.GetSoilCleanCost(frm.SelectedTechType), frmArea.LandTypeAreas, frmArea.GridArea, frmArea.GridCost, frm.GridCondition, frm.GridCost);
                //this.Close();
                this.Hide();
            }
        }

        public FormConfirmArea GetConfirmArea()
        {
            foreach (Control ctrl in panelBody.Controls)
            {
                if (ctrl is FormConfirmArea)
                {
                    return (FormConfirmArea)ctrl;
                }
            }

            return null;
        }

        public FormInputCondition GetInputCondition()
        {
            foreach (Control ctrl in panelBody.Controls)
            {
                if (ctrl is FormInputCondition)
                {
                    return (FormInputCondition)ctrl;
                }
            }

            return null;
        }
    }
}
