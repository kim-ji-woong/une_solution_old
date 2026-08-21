using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoadMan
{
    public partial class FormEditSection : Form
    {
        private static FormEditSection m_instance = null;
        public static FormEditSection Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new FormEditSection();

                return m_instance;
            }
        }

        private bool m_selectArea = true;
        private bool m_directPrev = false;
        private bool m_directNext = true;
        private bool m_callFromProperty = false;
        private ScheduleProperty m_currentProperty = null;
        private EditBoxHatch m_currentHatch = null;

        public bool SelectArea
        {
            get { return m_selectArea; }
            set
            {
                m_selectArea = value;

                if (m_selectArea)
                {
                    FormMain.Instance.CurrentPanel.Activity = PanelDXFViewer.ActivityType.SELECT_AREA;
                    EnableControl(false);
                }
                else
                {
                    FormMain.Instance.CurrentPanel.Activity = PanelDXFViewer.ActivityType.EDIT_SECTION;
                    EnableControl(true);
                }

                m_callFromProperty = true;
                radioSelectArea.Checked = m_selectArea;
                radioEditPolygon.Checked = !radioSelectArea.Checked;
                m_callFromProperty = false;
            }
        }

        public bool DirectPrev
        {
            get { return m_directPrev; }
            set
            {
                m_directPrev = value;

                m_callFromProperty = true;
                radioPrevDirect.Checked = m_directPrev;
                radioPrevBoundary.Checked = !radioPrevDirect.Checked;
                m_callFromProperty = false;
            }
        }

        public bool DirectNext
        {
            get { return m_directNext; }
            set
            {
                m_directNext = value;

                m_callFromProperty = true;
                radioNextDirect.Checked = m_directNext;
                radioNextBoundary.Checked = !radioNextDirect.Checked;
                m_callFromProperty = false;
            }
        }

        public ScheduleProperty CurrentProperty
        {
            get { return m_currentProperty; }
            set { m_currentProperty = value; }
        }

        public EditBoxHatch CurrentHatch
        {
            get { return m_currentHatch; }
            set { m_currentHatch = value; }
        }

        public FormEditSection()
        {
            InitializeComponent();

            Init();
        }

        private void radio_CheckedChanged(object sender, EventArgs e)
        {
            if (m_callFromProperty)
                return;

            if (sender == radioSelectArea || sender == radioEditPolygon)
            {
                SelectArea = radioSelectArea.Checked;
            }
            else if (sender == radioPrevDirect || sender == radioPrevBoundary)
            {
                m_directPrev = radioPrevDirect.Checked;
            }
            else if (sender == radioNextDirect || sender == radioNextBoundary)
            {
                m_directNext = radioNextDirect.Checked;
            }
        }

        public void Init()
        {
            PanelDXFViewer.ActivityType oldActivity = FormMain.Instance.CurrentPanel.Activity;

            SelectArea = true;
            DirectPrev = false;
            DirectNext = true;

            FormMain.Instance.CurrentPanel.Activity = oldActivity;

            radioSelectArea.Checked = SelectArea;
            radioEditPolygon.Checked = !radioSelectArea.Checked;

            radioPrevDirect.Checked = DirectPrev;
            radioPrevBoundary.Checked = !radioPrevDirect.Checked;

            radioNextDirect.Checked = DirectNext;
            radioNextBoundary.Checked = !radioNextDirect.Checked;
        }

        private void EnableControl(bool enabled)
        {
            radioPrevDirect.Enabled = radioPrevBoundary.Enabled = enabled;
            radioNextDirect.Enabled = radioNextBoundary.Enabled = enabled;
        }

        private void FormEditSection_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                FormMain.Instance.CurrentPanel.Activity = PanelDXFViewer.ActivityType.NONE;
            }
        }

        public bool ClearSelection()
        {
            if (CurrentProperty != null)
            {
                foreach (SchedulePropertySector sector in CurrentProperty.Sectors)
                {
                    sector.Hatch.Visible = false;
                }

                CurrentProperty = null;
                CurrentHatch = null;
                return true;
            }

            return false;
        }
    }
}
