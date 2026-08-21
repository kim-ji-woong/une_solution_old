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
    public partial class FormResult : Form
    {
        private TechType m_techType = TechType.None;
        private SoilCleanCost m_cost = null;
        private Dictionary<LandType, Overlay.AreaNCost> m_dicLandTypeArea = null;
        private double m_dInheritanceValue = 0.0, m_dExistanceValue = 0.0, m_dBioValue = 0.0;
        private DataGridView m_gridArea = null;
        private DataGridView m_gridPublicCost = null;
        private DataGridView m_gridCondition = null;
        private DataGridView m_gridValueCost = null;
        

        private Form m_frmCurrent = null;

        public FormResult()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            FormAnnualCapacity frmCapacity = new FormAnnualCapacity();
            frmCapacity.TopLevel = false;
            panelBody.Controls.Add(frmCapacity);
            frmCapacity.Dock = DockStyle.Fill;
            frmCapacity.Show();

            FormAnnualValue frmValue = new FormAnnualValue();
            frmValue.TopLevel = false;
            panelBody.Controls.Add(frmValue);
            frmValue.Dock = DockStyle.Fill;
            frmValue.Show();

            FormTotalValue frmTotal = new FormTotalValue();
            frmTotal.TopLevel = false;
            panelBody.Controls.Add(frmTotal);
            frmTotal.Dock = DockStyle.Fill;
            frmTotal.Show();
            
            FormEconomicValue frmEconomic = new FormEconomicValue();
            frmEconomic.TopLevel = false;
            panelBody.Controls.Add(frmEconomic);
            frmEconomic.Dock = DockStyle.Fill;
            frmEconomic.Show();

            FormEconomicSummary frmSummary = new FormEconomicSummary();
            frmSummary.TopLevel = false;
            panelBody.Controls.Add(frmSummary);
            frmSummary.Dock = DockStyle.Fill;
            frmSummary.Show();

            m_frmCurrent = frmCapacity;

            btnPrev.Location = btnNext.Location;
            btnNext.Location = btnSave.Location;
            btnSave.Visible = false;
        }

        public void Show(TechType techType, double dInheritanceValue, double dExistanceValue, double dBioValue, SoilCleanCost cost, Dictionary<LandType, Overlay.AreaNCost> dicLandTypeArea, DataGridView gridArea, DataGridView gridPublicCost, DataGridView gridCondition, DataGridView gridValueCost, IWin32Window owner)
        {
            m_techType = techType;
            m_cost = cost;
            m_dicLandTypeArea = dicLandTypeArea;
            m_dInheritanceValue = dInheritanceValue;
            m_dExistanceValue = dExistanceValue;
            m_dBioValue = dBioValue;
            m_gridArea = gridArea;
            m_gridPublicCost = gridPublicCost;
            m_gridCondition = gridCondition;
            m_gridValueCost = gridValueCost;

            if (m_frmCurrent is FormAnnualCapacity)
                MoveToAnnualCapacity();

            base.Show(owner);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (m_frmCurrent == null)
                return;

            if (m_frmCurrent is FormAnnualCapacity)
            {
                MoveToAnnualValue(((FormAnnualCapacity)m_frmCurrent).Capacities);
            }
            else if (m_frmCurrent is FormAnnualValue)
            {
                MoveToTotalValue(((FormAnnualValue)m_frmCurrent).Values);
            }
            else if( m_frmCurrent is FormTotalValue)
            {
                FormAnnualCapacity frm = null;
                foreach (Control ctrl in panelBody.Controls)
                {
                    if (ctrl is FormAnnualCapacity)
                    {
                        frm = (FormAnnualCapacity)ctrl;
                        break;
                    }
                }

                if (frm != null)
                    MoveToEconomicValue(frm.Capacities);
                //MoveToEconomicValue(((FormAnnualCapacity)m_frmCurrent).Capacities);
            }
            else if( m_frmCurrent is FormEconomicValue)
            {
                MoveToEconomicSummary(((FormEconomicValue)m_frmCurrent).Values);
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (m_frmCurrent == null)
                return;

            if (m_frmCurrent is FormAnnualValue)
            {
                MoveToAnnualCapacity();
            }
            else if (m_frmCurrent is FormTotalValue)
            {
                FormAnnualCapacity frm = null;

                foreach (Control ctrl in panelBody.Controls)
                {
                    if (ctrl is FormAnnualCapacity)
                    {
                        frm = (FormAnnualCapacity)ctrl;
                        break;
                    }
                }

                if (frm != null)
                    MoveToAnnualValue(frm.Capacities);
            }
            else if( m_frmCurrent is FormEconomicValue)
            {
                FormAnnualValue frm = null;

                foreach (Control ctrl in panelBody.Controls)
                {
                    if (ctrl is FormAnnualValue)
                    {
                        frm = (FormAnnualValue)ctrl;
                        break;
                    }
                }

                if (frm != null)
                    MoveToTotalValue(frm.Values);
            }
            else if( m_frmCurrent is FormEconomicSummary)
            {
                FormAnnualCapacity frm = null;
                foreach (Control ctrl in panelBody.Controls)
                {
                    if (ctrl is FormAnnualCapacity)
                    {
                        frm = (FormAnnualCapacity)ctrl;
                        break;
                    }
                }

                if (frm != null)
                    MoveToEconomicValue(frm.Capacities);
            }
        }

        private void MoveToTotalValue(DataGridView grid)
        {
            if (m_frmCurrent != null)
            {
                m_frmCurrent.Hide();
                m_frmCurrent = null;
            }

            foreach (Control ctrl in panelBody.Controls)
            {
                if (ctrl is FormTotalValue)
                {
                    m_frmCurrent = (FormTotalValue)ctrl;
                    break;
                }
            }

            if (m_frmCurrent == null)
                return;

            btnPrev.Visible = true;
            btnNext.Visible = true;
            btnSave.Visible = false;

            ((FormTotalValue)m_frmCurrent).Show(grid, m_techType, m_cost);
        }

        private void MoveToEconomicSummary(DataGridView grid)
        {
            if (m_frmCurrent != null)
            {
                m_frmCurrent.Hide();
                m_frmCurrent = null;
            }

            foreach (Control ctrl in panelBody.Controls)
            {
                if (ctrl is FormEconomicSummary)
                {
                    m_frmCurrent = (FormEconomicSummary)ctrl;
                    break;
                }
            }

            if (m_frmCurrent == null)
                return;

            btnPrev.Visible = true;
            btnNext.Visible = false;
            btnSave.Visible = true;

            ((FormEconomicSummary)m_frmCurrent).Show(m_gridCondition, grid);
        }

        private void MoveToEconomicValue(DataGridView grid)
        {
            if (m_frmCurrent != null)
            {
                m_frmCurrent.Hide();
                m_frmCurrent = null;
            }

            foreach (Control ctrl in panelBody.Controls)
            {
                if (ctrl is FormEconomicValue)
                {
                    m_frmCurrent = (FormEconomicValue)ctrl;
                    break;
                }
            }

            if (m_frmCurrent == null)
                return;

            btnPrev.Visible = true;
            btnNext.Visible = true;
            btnSave.Visible = false;

            ((FormEconomicValue)m_frmCurrent).Show(m_gridCondition, m_techType, m_dicLandTypeArea, m_gridValueCost, grid, m_gridPublicCost);
        }

        private void MoveToAnnualValue(DataGridView grid)
        {
            if (m_frmCurrent != null)
            {
                m_frmCurrent.Hide();
                m_frmCurrent = null;
            }

            foreach (Control ctrl in panelBody.Controls)
            {
                if (ctrl is FormAnnualValue)
                {
                    m_frmCurrent = (FormAnnualValue)ctrl;
                    break;
                }
            }

            if (m_frmCurrent == null)
                return;

            btnPrev.Visible = true;
            btnNext.Visible = true;
            btnSave.Visible = false;

            ((FormAnnualValue)m_frmCurrent).Show(grid, m_techType, m_dicLandTypeArea, m_dInheritanceValue, m_dExistanceValue, m_dBioValue);
        }

        private void MoveToAnnualCapacity()
        {
            if (m_frmCurrent != null)
            {
                m_frmCurrent.Hide();
                m_frmCurrent = null;
            }

            foreach (Control ctrl in panelBody.Controls)
            {
                if (ctrl is FormAnnualCapacity)
                {
                    m_frmCurrent = (FormAnnualCapacity)ctrl;
                    break;
                }
            }

            if (m_frmCurrent == null)
                return;

            btnPrev.Visible = true;
            btnNext.Visible = true;
            btnSave.Visible = false;

            ((FormAnnualCapacity)m_frmCurrent).Show(m_cost, m_dicLandTypeArea);
        }

        private void FormResult_FormClosing(object sender, FormClosingEventArgs e)
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

     
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "Project Files|*.xls|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "엑셀로 결과 저장";           

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                // Mouse Double Click으로 의도하지 않은 버튼이 클릭되는것을 막는다.
                FormMain.Instance.IgnorePushButton();

                FormAnnualCapacity frmCapacity = null;
                FormAnnualValue frmValue = null;
                FormTotalValue frmTotal = null;
                FormEconomicValue frmEconomic = null;

                foreach (Control ctrl in panelBody.Controls)
                {
                    if (ctrl is FormAnnualCapacity)
                        frmCapacity = (FormAnnualCapacity)ctrl;
                    else if (ctrl is FormAnnualValue)
                        frmValue = (FormAnnualValue)ctrl;
                    else if (ctrl is FormTotalValue)
                        frmTotal = (FormTotalValue)ctrl;
                    else if (ctrl is FormEconomicValue)
                        frmEconomic = (FormEconomicValue)ctrl;
                }

                if (frmCapacity == null || frmValue == null || frmTotal == null)
                    return;

                if (Data.ExcelReport.Export(dlg.FileName, FormMain.Instance, m_gridArea, m_gridPublicCost, m_gridCondition, m_gridValueCost, frmCapacity.Capacities, frmValue.Values, frmTotal.Values, frmEconomic.Values))
                    MessageBox.Show(this, "Excel문서가 생성되었습니다.", "Excel 내보내기", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void FormResult_Load(object sender, EventArgs e)
        {

        }
    }
}
