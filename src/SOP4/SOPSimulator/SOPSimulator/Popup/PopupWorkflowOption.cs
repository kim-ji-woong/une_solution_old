using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnE.SOP;
using UnE.SOP.Workstate;

namespace SOPMonitoringSystem.Popup
{
    public partial class PopupWorkflowOption : Form
    {
        private WorkflowOption m_option = new WorkflowOption();
        public WorkflowOption Option
        {
            get { return m_option; }
            set
            {
                m_option = value;

                if (m_option == null)
                {
                    panelOption.Controls.Clear();
                    SetShelters(null, false);
                }
                else
                {
                    if (m_option is WorkflowOptionSnowFall)
                    {
                        FormStartEventOptionSnow frm = new FormStartEventOptionSnow((WorkflowOptionSnowFall)m_option);
                        frm.TopLevel = false;
                        panelOption.Controls.Clear();
                        panelOption.Controls.Add(frm);
                        frm.Dock = DockStyle.Fill;
                        frm.Show();
                    }
                    else if (m_option is WorkflowOptionEarthquake)
                    {
                        FormStartEventOptionEarthquake frm = new FormStartEventOptionEarthquake((WorkflowOptionEarthquake)m_option);
                        frm.TopLevel = false;
                        panelOption.Controls.Clear();
                        panelOption.Controls.Add(frm);
                        frm.Dock = DockStyle.Fill;
                        frm.Show();
                    }

                    SetShelters(m_option.UsingShelters, m_option.UseShelters);
                    checkBoxUseSMS.Checked = m_option.UseSmsMessage;

                    if (m_option.UseCurrentTime)
                        radioAuto.Checked = true;
                    else
                    {
                        radioManual.Checked = true;
                        DateTime time = m_option.DetectTime == null ? DateTime.Now : m_option.DetectTime.Data;
                        labelManualTime.Text = string.Format("{0}-{1}-{2} {3}:{4}:{5}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
                    }
                }
            }
        }
        /*private bool m_useSmsMessage = true;
        public bool UseSmsMessage
        {
            get { return m_useSmsMessage; }
            set { m_useSmsMessage = value; }
        }

        private DateTime m_dtDetect = new DateTime();
        public DateTime DetectTime
        {
            get { return m_dtDetect; }
        }

        private bool m_useAmountSnowfall = false;
        public bool UseAmountSnowfall
        {
            get { return m_useAmountSnowfall; }
            set
            {
                m_useAmountSnowfall = value;
                groupBoxAmountSnowfall.Visible = m_useAmountSnowfall;
            }
        }

        private string m_strAmountSnowfall = "";
        public string AmountSnowfall
        {
            get { return m_strAmountSnowfall; }
            set { m_strAmountSnowfall = value; }
        }*/

        private Size m_sizeShelter, m_sizeNoShelterWithoutOption, m_sizeNoShelterWithOption;
        private Point m_ptButtonRunShelter, m_ptButtonRunNoShelter;
        private Point m_ptButtonCancelShelter, m_ptButtonCancelNoShelter;

        /*public List<Shelter> UsingShelters
        {
            get
            {
                if (!checkBoxShelterUse.Checked)
                    return null;

                List<Shelter> shelters = new List<Shelter>();

                foreach (DataGridViewRow row in gridShelter.Rows)
                {
                    if (row.Cells[2].Value != null && (bool)row.Cells[2].Value == true)
                        shelters.Add((Shelter)row.Tag);
                }

                return shelters;
            }
        }*/

        public PopupWorkflowOption()
        {
            InitializeComponent();

            radioAuto.Checked = true;
            labelManualTime.Text = "";

            InitPosition();
            InitGrid();
        }

        public void SetShelters(List<UnE.Spatial.Shelter> shelters, bool checkUseShelter)
        {
            if (shelters == null || shelters.Count == 0)
            {
                checkBoxShelterUse.Visible = false;
                gridShelter.Visible = false;
                gridShelter.Rows.Clear();

                if (panelOption.Controls.Count == 0)
                    this.Size = m_sizeNoShelterWithoutOption;
                else
                    this.Size = m_sizeNoShelterWithOption;

                btnRun.Location = m_ptButtonRunNoShelter;
                btnCancel.Location = m_ptButtonCancelNoShelter;

                if (m_option is WorkflowOptionSnowFall)
                {
                    WorkflowOptionSnowFall option = (WorkflowOptionSnowFall)m_option;

                    if (option.UseAmountSnowFall)
                        this.Size = new Size(m_sizeShelter.Width, this.Size.Height);
                }
                /*if (m_useAmountSnowfall)
                    this.Size = new Size(m_sizeShelter.Width, this.Size.Height);*/
            }
            else
            {
                checkBoxShelterUse.Visible = true;
                checkBoxShelterUse.Checked = checkUseShelter;
                checkBoxShelterUse.Enabled = true;
                gridShelter.Visible = true;
                SetShelterGrid(shelters);

                this.Size = m_sizeShelter;
                btnRun.Location = m_ptButtonRunShelter;
                btnCancel.Location = m_ptButtonCancelShelter;
            }
        }

        private void SetShelterGrid(List<UnE.Spatial.Shelter> shelters)
        {
            gridShelter.Rows.Clear();

            foreach (UnE.Spatial.Shelter shelter in shelters)
            {
                DataGridViewRow row = PopupStartEvent.MakeNewRow(gridShelter);

                row.Cells[0].Value = shelter.ShelterName;
                row.Cells[0].ReadOnly = true;

                row.Cells[1].Value = shelter.Description == null ? "" : shelter.Description;
                row.Cells[1].ReadOnly = true;

                row.Cells[2].ReadOnly = false;
                row.Tag = shelter;
            }
        }

        private void InitGrid()
        {
            InitColumns(gridShelter);
        }

        private void InitColumns(DataGridView grid)
        {
            foreach (DataGridViewColumn column in grid.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void InitPosition()
        {
            m_sizeShelter = this.Size;
            m_sizeNoShelterWithoutOption = new Size(353, 241);
            m_sizeNoShelterWithOption = new Size(m_sizeShelter.Width, m_sizeNoShelterWithoutOption.Height);

            m_ptButtonRunShelter = btnRun.Location;
            m_ptButtonRunNoShelter = new Point(90, 162);

            m_ptButtonCancelShelter = btnCancel.Location;
            m_ptButtonCancelNoShelter = new Point(173, 162);
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            if (m_option != null)
            {
                if (radioAuto.Checked)
                    m_option.DetectTime = new DBUtility.VariousData<DateTime>(DateTime.Now);

                m_option.UseSmsMessage = checkBoxUseSMS.Checked;
                m_option.UseShelters = checkBoxShelterUse.Checked;

                if (m_option.UseShelters)
                {
                    m_option.UsingShelters.Clear();

                    foreach (DataGridViewRow row in gridShelter.Rows)
                    {
                        if (row.Cells[2].Value != null && (bool)row.Cells[2].Value == true)
                            m_option.UsingShelters.Add((UnE.Spatial.Shelter)row.Tag);
                    }
                }

                if (radioAuto.Checked)
                    m_option.DetectTime = new DBUtility.VariousData<DateTime>(DateTime.Now);
                else
                {
                    DateTime time;

                    if (DateTime.TryParse(labelManualTime.Text, out time))
                        m_option.DetectTime = new DBUtility.VariousData<DateTime>(time);
                }
            }
            /*if (radioAuto.Checked)
                m_dtDetect = DateTime.Now;

            m_strAmountSnowfall = textBoxAmountSnowfall.Text.Trim();
            UseSmsMessage = checkBox2.Checked;*/
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
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

                m_option.DetectTime = new DBUtility.VariousData<DateTime>(new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, 0));
                //m_dtDetect = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, 0);
            }

            EnableTimeOptionControls(true);
        }

        private void EnableTimeOptionControls(bool enabled)
        {
            labelManualTime.Visible = enabled;
            btnEditManualTime.Visible = enabled;
        }

        private void btnEditManualTime_Click(object sender, EventArgs e)
        {
            PopupDetectTime popup = new PopupDetectTime(m_option.DetectTime != null ? m_option.DetectTime.Data : DateTime.Now/*m_dtDetect*/);
            popup.Owner = this;

            if (popup.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                m_option.DetectTime = new DBUtility.VariousData<DateTime>(popup.DetectTime);

                labelManualTime.Text = string.Format("{0}-{1}-{2} {3}:{4}:{5}",
                    m_option.DetectTime.Data.Year, m_option.DetectTime.Data.Month, m_option.DetectTime.Data.Day, m_option.DetectTime.Data.Hour, m_option.DetectTime.Data.Minute, m_option.DetectTime.Data.Second);
                /*m_dtDetect = popup.DetectTime;

                labelManualTime.Text = string.Format("{0}-{1}-{2} {3}:{4}:{5}",
                    m_dtDetect.Year, m_dtDetect.Month, m_dtDetect.Day, m_dtDetect.Hour, m_dtDetect.Minute, m_dtDetect.Second);*/
            }
        }

        private void checkBoxShelterUse_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxShelterUse.Checked)
            {
                gridShelter.ReadOnly = false;

                foreach (DataGridViewRow row in gridShelter.Rows)
                {
                    row.Cells[0].ReadOnly = true;
                    row.Cells[1].ReadOnly = true;
                    row.Cells[2].ReadOnly = false;
                }
            }
            else
                gridShelter.ReadOnly = true;
        }
    }
}
