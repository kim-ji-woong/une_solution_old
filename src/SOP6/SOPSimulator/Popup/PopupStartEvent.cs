using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnE.SOP;
using UnE.SOP.Workstate;
using DBUtility2;

namespace SOPMonitoringSystem.Popup
{
	//public delegate void EndCheckPosition(bool bResult);
    public partial class PopupStartEvent : Form, IWorkflowStartOption
	{
		public virtual event EndCheckPosition OnCheckPositionEnd;

        private WorkflowOption m_option = null;
        public WorkflowOption Option
        {
            get { return m_option; }
            set { m_option = value; }
        }

		private HistoryDisasterPosition mLastPoistion = null;
		public HistoryDisasterPosition LastPosition
		{
			get { return mLastPoistion; }
			set { mLastPoistion = value; }
		}

        /*private PSMMaterial m_psmMaterial = null;
        public PSMMaterial PSMMaterial
        {
            get { return m_psmMaterial; }
            set { m_psmMaterial = value; }
        }*/

        public Form GetInvokeForm()
        {
            return this;
        }

        public new bool IsHandleCreated()
        {
            return ((Form)this).IsHandleCreated;
        }

		/*private bool bSendSMS = true;
		public bool UseSMS
		{
			get { return bSendSMS; }
			set
            {
                if (bSendSMS != value)
                {
                    bSendSMS = value;
                    checkBox2.Checked = value;
                }
            }
		}*/

		private string disasterName = "";
		public string DisasterName
		{
			get { return disasterName; }
			set { disasterName = value; }
		}

        /*private DateTime m_dtDetect = new DateTime();
        public DateTime DetectTime
        {
            get { return m_dtDetect; }
        }*/


        private bool m_bSetUserTime = false;
        private DateTime m_dtUserInput = new DateTime();
        public DateTime InputTime
        {
            get { return m_dtUserInput; }
            set 
            {
                m_bSetUserTime = true;
                m_dtUserInput = value;
                radioManual.Checked = true;
            }
        }
	   
		public string PositionName
		{
			get
			{
				return textBoxPosition.Text;
			}
			set
			{
                SetPositionName(value);
				/*if (value == "..." || value == "")
				{
					btnRun.Enabled = false;
                    strPosition.Enabled = false;
				}
				else
				{
                    if (mLastPoistion != null && (textBoxPSMDistance.Visible == false || (textBoxPSMDistance.Visible == true && textBoxPSMDistance.Text.Length > 0)))
                        btnRun.Enabled = true;

                    //strPosition.Enabled = true;
				}
				strPosition.Text = value;*/
			}
		}

        /*private bool m_usePSM = false;
        public bool UsePSM
        {
            get { return m_usePSM; }
            set { m_usePSM = value; }
        }*/

		private ArrayList recentList = null;
        /*private Size m_sizeShelter, m_sizeNoShelter;
        private Point m_ptButtonRunShelter, m_ptButtonRunNoShelter;
        private Point m_ptButtonCancelShelter, m_ptButtonCancelNoShelter;*/

        private List<SOPParameter> m_userDefinedParameters = null;
        private PSMMaterial m_userDefinedMaterial = new PSMMaterial();

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

        public PopupStartEvent(List<SOPParameter> userDefinedParameters)
		{
			InitializeComponent();

            m_userDefinedParameters = userDefinedParameters;
			AdjustLocation(FormSOP.Instance);
			btnRun.Enabled = false;

            radioAuto.Checked = true;
            labelManualTime.Text = "";

            AcceptButton = btnRun;
            this.CancelButton = btnCancel;

            //InitPosition();
            InitGrid();

            SetUserDefinedVariableGrid();
            SetPosition(false, m_userDefinedParameters != null && m_userDefinedParameters.Count > 0);
		}

        private void InitGrid()
        {
            InitColumns(gridShelter);
            InitColumns(gridUserDefinedParameters);
        }

        private void InitColumns(DataGridView grid)
        {
            foreach (DataGridViewColumn column in grid.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void SetUserDefinedVariableGrid()
        {
            if (m_userDefinedParameters == null || m_userDefinedParameters.Count == 0)
                return;

            foreach (SOPParameter param in m_userDefinedParameters)
            {
                int nRowIndex = gridUserDefinedParameters.Rows.Add();

                if (nRowIndex < 0)
                    return;

                DataGridViewRow row = gridUserDefinedParameters.Rows[nRowIndex];

                row.Cells[0].Value = param.VariableName;
                row.Cells[1].Value = Sections.SectionDataDecision.GetVariableTypeName(param.Type);
                row.Cells[2].Value = param.Description == null ? "" : param.Description;
                row.Tag = param;
            }
        }

        private void SetPosition(bool showShelter, bool showUserDefinedConfig)
        {
            if (showShelter)
            {
                if (showUserDefinedConfig)
                {
                    this.Size = new Size(454, 749);
                    labelUserDefinedParameters.Location = new Point(12, 486);
                    gridUserDefinedParameters.Location = new Point(12, 508);
                    btnRun.Location = new Point(12, 669);
                    btnCancel.Location = new Point(95, 669);
                }
                else
                {
                    this.Size = new Size(454, 626);
                    btnRun.Location = new Point(12, 546);
                    btnCancel.Location = new Point(95, 546);
                }
            }
            else
            {
                if (showUserDefinedConfig)
                {
                    this.Size = new Size(454, 626);
                    labelUserDefinedParameters.Location = new Point(12, 363);
                    gridUserDefinedParameters.Location = new Point(12, 385);
                    btnRun.Location = new Point(12, 546);
                    btnCancel.Location = new Point(95, 546);
                }
                else
                {
                    this.Size = new Size(454, 449);
                    btnRun.Location = new Point(12, 369);
                    btnCancel.Location = new Point(95, 369);
                }
            }

            checkBoxShelterUse.Visible = gridShelter.Visible = showShelter;
            labelUserDefinedParameters.Visible = gridUserDefinedParameters.Visible = showUserDefinedConfig;
        }

        /*private void InitPosition()
        {
            int nHeight = gridShelter.Location.Y + gridShelter.Height - checkBoxShelterUse.Location.Y;

            m_sizeShelter = this.Size;
            m_sizeNoShelter = new Size(this.Size.Width, this.Size.Height - nHeight);

            m_ptButtonRunShelter = btnRun.Location;
            m_ptButtonRunNoShelter = new Point(btnRun.Location.X, btnRun.Location.Y - nHeight);

            m_ptButtonCancelShelter = btnCancel.Location;
            m_ptButtonCancelNoShelter = new Point(btnCancel.Location.X, btnCancel.Location.Y - nHeight);
        }*/

		private void AdjustLocation(Form parent)
		{
			Size size = parent.Size;
			Point p = parent.Location;
			int x = p.X + (size.Width / 2) - (this.Size.Width / 2);
			int y = p.Y + (size.Height / 2) - (this.Size.Height / 2);
			this.Location = new Point(x, y);
		}

	   
		public void SetRecentPosition(ArrayList arList)
		{
			recentList = arList;
			for( int i = 0 ; i < recentList.Count; i++)
			{
				HistoryDisasterPosition pos = (HistoryDisasterPosition)recentList[i];
                pos.DisasterName = disasterName;
				SetRecentPosition(pos.PoistionName);
			}
		}

        public void SetShelters(List<UnE.Spatial.Shelter> shelters, bool checkShelterUse)
        {
            bool showUserDefinedConfig = m_userDefinedParameters != null && m_userDefinedParameters.Count > 0;

            if (shelters == null || shelters.Count == 0)
            {
                //checkBoxShelterUse.Visible = false;
                //gridShelter.Visible = false;
                gridShelter.Rows.Clear();

                if (m_option != null)
                {
                    m_option.UsingShelters.Clear();
                    m_option.UseShelters = false;
                }

                /*this.Size = m_sizeNoShelter;
                btnRun.Location = m_ptButtonRunNoShelter;
                btnCancel.Location = m_ptButtonCancelNoShelter;*/
                SetPosition(false, showUserDefinedConfig);
            }
            else
            {
                if (m_option != null)
                    m_option.UseShelters = checkShelterUse;

                //checkBoxShelterUse.Visible = true;
                checkBoxShelterUse.Checked = checkShelterUse;
                checkBoxShelterUse.Enabled = true;
                //gridShelter.Visible = true;
                SetShelterGrid(shelters);

                /*this.Size = m_sizeShelter;
                btnRun.Location = m_ptButtonRunShelter;
                btnCancel.Location = m_ptButtonCancelShelter;*/
                SetPosition(true, showUserDefinedConfig);
            }
        }

        private void SetShelterGrid(List<UnE.Spatial.Shelter> shelters)
        {
            if (m_option != null)
                m_option.UsingShelters.Clear();

            gridShelter.Rows.Clear();

            foreach (UnE.Spatial.Shelter shelter in shelters)
            {
                DataGridViewRow row = MakeNewRow(gridShelter);

                row.Cells[0].Value = shelter.ShelterName;
                row.Cells[0].ReadOnly = true;

                row.Cells[1].Value = shelter.Description == null ? "" : shelter.Description;
                row.Cells[1].ReadOnly = true;

                row.Cells[2].ReadOnly = false;
                row.Tag = shelter;

                if (m_option != null)
                    m_option.UsingShelters.Add(shelter);
            }
        }

        public static DataGridViewRow MakeNewRow(DataGridView grid)
        {
            int nRowIndex = grid.Rows.Add();

            if (nRowIndex < 0)
                return null;

            return grid.Rows[nRowIndex];
            /*if (grid.AllowUserToAddRows)
            {
                DataGridViewRow row = (DataGridViewRow)grid.Rows[grid.Rows.Count - 1].Clone();
                grid.Rows.Add(row);

                return grid.Rows[grid.Rows.Count - 2];
            }
            else
            {
                grid.AllowUserToAddRows = true;

                DataGridViewRow row = (DataGridViewRow)grid.Rows[grid.Rows.Count - 1].Clone();
                grid.Rows.Add(row);

                grid.AllowUserToAddRows = false;
            }

            return grid.Rows[grid.Rows.Count - 1];*/
        }
		
		public void AddLastHistoryDisasterPoistion(HistoryDisasterPosition pos)
		{
			mLastPoistion = pos;
            if (mLastPoistion != null && (textBoxPSMDistance.Visible == false || (textBoxPSMDistance.Visible == true && textBoxPSMDistance.Text.Length > 0)))
				btnRun.Enabled = true;
		}

		public void SetRecentPosition(string str)
		{
            // grid 사용시
			DataGridViewRow row = new DataGridViewRow();
			DataGridViewCell cell = new DataGridViewTextBoxCell();
			cell.Value = str;
			row.Cells.Add(cell);


            // combo box 사용시
            cboPositionHistory.Items.Add(str);
		}

		public void btnRunClick(object sender, EventArgs e)
		{
            if (!CheckTimeValidation())
                return;

            if (mLastPoistion == null)
                mLastPoistion = MakeDisasterPosition();
			/*if (textBoxPosition.Text == "" || mLastPoistion == null)
				return;*/

            if (cboPSMType.Visible == true && cboPSMType.SelectedIndex > 0)
            {
                if (m_option != null && m_option is WorkflowOptionPSM)
                    ((WorkflowOptionPSM)m_option).PSMMaterial = (PSMMaterial)cboPSMType.Items[cboPSMType.SelectedIndex];
                //m_psmMaterial = (PSMMaterial)cboPSMType.Items[cboPSMType.SelectedIndex];
            }

            PreRun(textBoxPosition.Text);
			/*mLastPoistion.PoistionName = strPosition.Text;

			if (OnCheckPositionEnd != null)
			{
				OnCheckPositionEnd(true);
			}*/

            if (CheckUserDefinedConfig() == false)
                return;
			
			this.DialogResult = DialogResult.OK;
            this.Close();
		}

        private bool CheckUserDefinedConfig()
        {
            Dictionary<SOPParameter, string> dicResults = new Dictionary<SOPParameter, string>();

            if (m_userDefinedParameters == null || m_userDefinedParameters.Count == 0)
                return true;

            if (m_option != null)
            {
                foreach (DataGridViewRow row in gridUserDefinedParameters.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    if (row.Tag != null && row.Tag is SOPParameter)
                    {
                        SOPParameter param = (SOPParameter)row.Tag;

                        string strValue = row.Cells[3].Value == null ? "" : row.Cells[3].Value.ToString().Trim();

                        if (CheckVariableValidation(param, strValue) == false)
                        {
                            row.Cells[3].Selected = true;
                            gridUserDefinedParameters.Focus();
                            return false;
                        }
                        else
                            dicResults[param] = strValue;
                    }
                }

                foreach (KeyValuePair<SOPParameter, string> pair in dicResults)
                {
                    m_option.UserDefinedParameters[pair.Key] = pair.Value;
                }
            }

            return true;
        }

        private bool CheckVariableValidation(SOPParameter param, string strValue)
        {
            if (param.Type == Sections.SectionDataDecision.VariableType.BOOLEAN)
            {
                string strLower = strValue.ToLower();

                if (strLower == "1" || strLower == "0" ||
                    strLower == "true" || strLower == "false" ||
                    strLower == "참" || strLower == "거짓")
                    return true;
                else
                {
                    MessageBox.Show(param.VariableName + "은 1, 0, true, false, 참, 거짓 가운데 하나만 사용하여야 합니다.");
                    return false;
                }
            }
            else if (param.Type == Sections.SectionDataDecision.VariableType.DOUBLE)
            {
                double data;

                if (double.TryParse(strValue, out data) == false)
                {
                    MessageBox.Show(param.VariableName + "은 실수값만 입력되어야 합니다.");
                    return false;
                }
            }
            else if (param.Type == Sections.SectionDataDecision.VariableType.INTEGER)
            {
                int data;

                if (int.TryParse(strValue, out data) == false)
                {
                    MessageBox.Show(param.VariableName + "은 정수값만 입력되어야 합니다.");
                    return false;
                }
            }
            else if (param.Type == Sections.SectionDataDecision.VariableType.UNKNOWN)
            {
                return false;
            }

            return true;
        }

        private HistoryDisasterPosition MakeDisasterPosition()
        {
            HistoryDisasterPosition pos = new HistoryDisasterPosition();

            pos.BroadcastName = pos.PoistionName = textBoxPosition.Text.Trim();
            pos.BuildingID = "";
            pos.DisasterName = m_option.DisasterName;
            pos.FloorIndex = -999.0f;
            pos.HistoryActionStepID = -1;
            pos.IconID = -1;
            pos.ZoneID = -1;
            pos.X = pos.Y = pos.Z = 0.0f;

            if (m_option is WorkflowOptionPSM)
            {
                WorkflowOptionPSM option = (WorkflowOptionPSM)m_option;
                pos.PSMDistance = option.PSMDistance;
                pos.PSMMaterial = option.PSMMaterial == null ? "" : option.PSMMaterial.MaterialName;
            }
            else
            {
                pos.PSMDistance = 0;
                pos.PSMMaterial = null;
            }

            return pos;
        }

        public void PreRun(string strPositionName)
        {
            mLastPoistion.PoistionName = strPositionName;

            if (m_option != null && m_option is WorkflowOptionPSM)
            {
                WorkflowOptionPSM option = (WorkflowOptionPSM)m_option;

                if (option.PSMMaterial != null)
                {
                    mLastPoistion.PSMMaterial = option.PSMMaterial.MaterialName;

                    int nDistance;

                    if (int.TryParse(textBoxPSMDistance.Text, out nDistance))
                        mLastPoistion.PSMDistance = nDistance;
                }
            }
            /*if (m_psmMaterial != null)
            {
                mLastPoistion.PSMMaterial = m_psmMaterial.MaterialName;

                int nDistance;

                if (int.TryParse(textBoxPSMDistance.Text, out nDistance))
                    mLastPoistion.PSMDistance = nDistance;
            }*/

            ProxyMessenger.Instance.OnCheckPositionEnd(true);
            /*if (OnCheckPositionEnd != null)
            {
                OnCheckPositionEnd(true);
            }*/
        }

		private void btnCancelClick(object sender, EventArgs e)
		{
            //ProxyMessenger.Instance.OnCheckPositionEnd(false);
			/*if (OnCheckPositionEnd != null)
			{
				OnCheckPositionEnd(false);
			}*/
			this.DialogResult = DialogResult.Cancel;
            this.Close();
		}

		private void checkBox2_CheckedChanged(object sender, EventArgs e)
		{
            if (m_option != null)
                m_option.UseSmsMessage = checkBox2.Checked;
            //bSendSMS = checkBox2.Checked;
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			bool bCheck = checkBox1.Checked;
			if (bCheck == true)
			{
                IDisasterContainer diasterForm = ProxySOP.Instance.SOPDisasterContainer;
                if (diasterForm != null)
                {
                    diasterForm.SetCheckPoistion(this, true);
                }
				//strPosition.Enabled = false;
			}
			else
			{
                ProxyMessenger.Instance.OnCheckPositionEnd(false);
                /*if( OnCheckPositionEnd != null)
                    OnCheckPositionEnd(false);*/
                //IDisasterContainer diasterForm = ProxySOP.Instance.SOPDisasterContainer;
                //if (diasterForm != null)
                //{
                 //   
                //}
				//strPosition.Enabled = true;
				//mLastPoistion = null;
			}
		}

		private void textBoxPosition_TextChanged(object sender, EventArgs e)
		{
            string strPosition = textBoxPosition.Text.Trim();

			if (strPosition == "..." || strPosition.Length == 0)
			{
				btnRun.Enabled = false;
			}
			else
			{
				if (/*mLastPoistion != null && */(textBoxPSMDistance.Visible == false || (textBoxPSMDistance.Visible == true && textBoxPSMDistance.Text.Length > 0)))
					btnRun.Enabled = true;
			}

            if (m_option != null)
                m_option.PositionName = strPosition;
		}

        private void EnableTimeOptionControls(bool enabled)
        {
            labelManualTime.Visible = enabled;
            btnEditManualTime.Visible = enabled;
        }

        private void radioAuto_CheckedChanged(object sender, EventArgs e)
        {
            EnableTimeOptionControls(false);
        }

        private void radioManual_CheckedChanged(object sender, EventArgs e)
        {
            if(m_bSetUserTime == false)            
            {
                m_dtUserInput = DateTime.Now;
            }

            DateTime dtNow = m_dtUserInput;

            if (labelManualTime.Text == "")
            {
                labelManualTime.Text = string.Format("{0}-{1}-{2} {3}:{4}:00",
                    dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute);

                if (m_option != null)
                    m_option.DetectTime = new VariousData<DateTime>(new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, 0));
                //m_dtDetect = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, 0);
            }

            EnableTimeOptionControls(true);
        }

        private bool CheckTimeValidation()
        {
            if (radioAuto.Checked)
            {
                if (m_option != null)
                    m_option.DetectTime = new VariousData<DateTime>(DateTime.Now);
                //m_dtDetect = DateTime.Now;
            }

            return true;
        }

        private void btnEditManualTime_Click(object sender, EventArgs e)
        {
            DateTime time = m_option == null || m_option.DetectTime == null ? DateTime.Now : m_option.DetectTime.Data;
            PopupDetectTime popup = new PopupDetectTime(time);
            //PopupDetectTime popup = new PopupDetectTime(m_dtDetect);
            popup.Owner = this;

            if (popup.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                m_option.DetectTime = new VariousData<DateTime>(popup.DetectTime);

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

            if (m_option != null)
                m_option.UseShelters = checkBoxShelterUse.Checked;
        }

        private void cboPositionHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboPositionHistory.SelectedIndex >= 0)
            {
                string szText = (string)(cboPositionHistory.SelectedItem);
                SetPositionName(szText);
                //PositionName = szText;

                if (recentList.Count > cboPositionHistory.SelectedIndex)
                    LastPosition = (HistoryDisasterPosition)recentList[cboPositionHistory.SelectedIndex];
                else
                {
                    HistoryDisasterPosition pos = new HistoryDisasterPosition();
                    pos.PoistionName = szText;
                    LastPosition = pos;
                }

                if (textBoxPSMDistance.Visible == false || (textBoxPSMDistance.Visible == true && textBoxPSMDistance.Text.Length > 0))
                    btnRun.Enabled = true;
            }
        }

        private void PopupStartEvent_Load(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                if (m_option != null)
                {
                    checkBox2.Checked = m_option.UseSmsMessage;
                    SetPositionName(m_option.PositionName);
                }

                if (UnE.SOP.ProxySOP.Instance.UsePSM)
                {
                    InitPSM();
                    labelPSMType.Visible = cboPSMType.Visible = labelPSMDistance.Visible = textBoxPSMDistance.Visible = m_option != null && m_option is WorkflowOptionPSM;
                    //labelPSMType.Visible = cboPSMType.Visible = labelPSMDistance.Visible = textBoxPSMDistance.Visible = m_usePSM;
                }
            });
        }

        private void SetPositionName(string strPositionName)
        {
            if (strPositionName == "..." || strPositionName == "")
            {
                btnRun.Enabled = false;
                textBoxPosition.Enabled = false;
            }
            else
            {
                if (mLastPoistion != null && (textBoxPSMDistance.Visible == false || (textBoxPSMDistance.Visible == true && textBoxPSMDistance.Text.Length > 0)))
                    btnRun.Enabled = true;

                //strPosition.Enabled = true;
            }

            textBoxPosition.Text = strPositionName;

            if (m_option != null)
                m_option.PositionName = strPositionName;
        }

        private void InitPSM()
        {
            cboPSMType.Items.Clear();

            m_userDefinedMaterial.MaterialName = "사용자 정의물질";
            cboPSMType.Items.Add(m_userDefinedMaterial);

            // Query Error
            //string strSQL = "select tank.MaterialType, m.MaterialName, max(tank.EvacInitDistance), max(tank.EvacDayDistance), max(tank.EvacNightDistance) ";
            //strSQL += "from PSMTank as tank, PSMMaterial as m ";
            //strSQL += "group by tank.MaterialType, m.MaterialName, m.ID having tank.MaterialType = m.ID";

            string strIFNull = FormSOP.Instance.DBManager.DatabaseType == WebDBManager.DBType.sqlserver ? "ISNULL" : "IFNULL";

            string strSQL = string.Format("select ID, MaterialName, {0}(EvacInitDistance, 0), {0}(EvacDayDistance, 0), {0}(EvacNightDistance, 0) ", strIFNull);
            strSQL += "from	PSMMaterial ";

            ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-4;i+=5)
            {
                VariousData<int> materialID = WebDBManager.GetIntField(arrResult[i].ToString());
                string materialName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> initDistance = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> dayDistance = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<int> nightDistance = WebDBManager.GetIntField(arrResult[i + 4].ToString());

                if (materialID == null || materialName == null || initDistance == null || dayDistance == null || nightDistance == null)
                    continue;

                PSMMaterial material = new PSMMaterial();

                material.MaterialID = materialID.Data;
                material.MaterialName = materialName;
                material.InitDistance = initDistance.Data;
                material.DayDistance = dayDistance.Data;
                material.NightDistance = nightDistance.Data;

                cboPSMType.Items.Add(material);
            }

            if (cboPSMType.Items.Count > 1)
                cboPSMType.SelectedIndex = 1;
        }

        private void cboPSMType_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cboPSMType.SelectedIndex == 0)
            {
                FormUserDefinedPSMMaterial frm = new FormUserDefinedPSMMaterial(cboPSMType.Items);
                frm.StartPosition = FormStartPosition.Manual;
                frm.DesktopLocation = this.DesktopLocation;

                if (frm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    PSMMaterial material = frm.Material;
                    int nIndex = cboPSMType.Items.Add(material);
                    cboPSMType.SelectedIndex = nIndex;
                }
                else
                    cboPSMType.SelectedIndex = -1;
            }
            else if (cboPSMType.SelectedIndex > 0)
            {
                bool isDayLight = Popup.SOPLoader.IsDayLight(DateTime.Now);
                PSMMaterial material = (PSMMaterial)cboPSMType.Items[cboPSMType.SelectedIndex];

                textBoxPSMDistance.Text = isDayLight ? material.DayDistance.ToString() : material.NightDistance.ToString();
            }
            else
                textBoxPSMDistance.Text = "";
        }

        private void textBoxPSMDistance_TextChanged(object sender, EventArgs e)
        {
            if (mLastPoistion != null && (textBoxPSMDistance.Visible == false || (textBoxPSMDistance.Visible == true && textBoxPSMDistance.Text.Length > 0)))
                btnRun.Enabled = true;
        }

        private void gridShelter_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            gridShelter.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void gridShelter_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (m_option == null)
                return;

            if (e.ColumnIndex == 2 && e.RowIndex >= 0)
            {
                DataGridViewRow row = gridShelter.Rows[e.RowIndex];

                if (row.IsNewRow)
                    return;

                UnE.Spatial.Shelter shelter = (UnE.Spatial.Shelter)row.Tag;

                if (shelter == null)
                    return;

                if (row.Cells[2].Value != null && (bool)row.Cells[2].Value == true)
                {
                    if (m_option.UsingShelters.Contains(shelter))
                        m_option.UsingShelters.Add(shelter);
                }
                else
                {
                    m_option.UsingShelters.Remove(shelter);
                }
            }
        }

        private void PopupStartEvent_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == System.Windows.Forms.DialogResult.Cancel)
            {
                ProxyMessenger.Instance.OnCheckPositionEnd(false);
            }
        }
    }
}
