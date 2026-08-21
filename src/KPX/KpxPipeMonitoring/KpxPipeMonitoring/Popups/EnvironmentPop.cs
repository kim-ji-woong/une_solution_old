using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using DBUtility;

namespace KpxPipeMonitoring.Popups
{
    public partial class EnvironmentPop : Form
    {
        #region Form 이동
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();
        private bool m_isClicked = false;
        private Point m_ptOrigin = new Point();
        #endregion

        private bool m_useSMS = false;

        private VariousData<bool> m_useSMSDB = new VariousData<bool>(false);
        private int m_nUseSMSID = -1;    

        //알람 단위 시간
        private VariousData<double> m_beginPointDeltaTDB = null;
        private int m_nBeginPointDeltaTDB = -1;

        //알람 무시 시간
        private VariousData<double> m_ignoreTime = null;
        private int m_nIgnoreTime = -1;

        private string m_strPublicMessageDB = null;

        private CommonFunction commonFunction = null; 

        public EnvironmentPop()
        {
            this.DoubleBuffered = true;
            InitializeComponent();

            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    button_cancel_Click(null, null);
                } 
            }; 

            btnMemberInfo.Visible = false;
            btnManager.Visible = false; 
            label1.Visible = false;
            textBox_alarmOccurSecond.Visible = false;
            label12.Visible = false;

            labelTitle.Parent = pictureBoxTitle;
            btnClose.Parent = pictureBoxTitle;

            commonFunction = new CommonFunction();

            textBox_pipeStableRatio.KeyPress += textBox_Decimal_KeyPress;
            textBox_pipeStableAbsolute.KeyPress += textBox_Decimal_KeyPress;
            textBox_stableBeginWorkM.KeyPress += textBox_Integer_KeyPress;            
            textBox_pipeStableCTime.KeyPress += textBox_Integer_KeyPress;
            textBox_alarmInterval.KeyPress += textBox_Decimal_KeyPress;

            textBox_tankStableRatio.KeyPress += textBox_Decimal_KeyPress;
            textBox_tankStableAbsolute.KeyPress += textBox_Decimal_KeyPress; 
            textBox_tankStableCTime.KeyPress += textBox_Integer_KeyPress; 

            textBox_alarmIgnoreMinute.KeyPress += textBox_Integer_KeyPress;
            textBox_alarmOccurSecond.KeyPress += textBox_Integer_KeyPress;
             
            textBox_highLevel.KeyPress += textBox_Decimal_KeyPress;
            textBox_minTemp.KeyPress += textBox_Decimal_KeyPress;
            textBox_maxTemp.KeyPress += textBox_Decimal_KeyPress;
            textBox_leakTime.KeyPress += textBox_Decimal_KeyPress;
            textBox_leakLevel.KeyPress += textBox_Decimal_KeyPress; 

            radioButton_pipeStableRatio.CheckedChanged += radioButton_pipeStable_CheckedChanged;
            radioButton_pipeStableAbsolute.CheckedChanged += radioButton_pipeStable_CheckedChanged;

            radioButton_tankStableRatio.CheckedChanged += radioButton_tankStable_CheckedChanged;
            radioButton_tankStableAbsolute.CheckedChanged += radioButton_tankStable_CheckedChanged;

            checkBox_pipeStableCTimeUse.CheckedChanged += checkBox_pipeStableCTimeUse_CheckedChanged;
            textBox_pipeStableCTime.Enabled = false;

            checkBox_tankStableCTimeUse.CheckedChanged += checkBox_tankStableCTimeUse_CheckedChanged;
            textBox_tankStableCTime.Enabled = false;

            groupBox7.Visible = false;
        }

        private void EnvironmentPop_Load(object sender, EventArgs e)
        {
            LoadOptions();
            LoadStable();
            m_strPublicMessageDB = LoadPublicMessage();

            if (m_strPublicMessageDB != null)
                textBoxPublicMessage.Text = m_strPublicMessageDB;
        }

        #region 체크 이벤트
        void checkBox_pipeStableCTimeUse_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_pipeStableCTimeUse.Checked)
                textBox_pipeStableCTime.Enabled = true;
            else
                textBox_pipeStableCTime.Enabled = false;
        }
        void checkBox_tankStableCTimeUse_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_tankStableCTimeUse.Checked)
                textBox_tankStableCTime.Enabled = true;
            else
                textBox_tankStableCTime.Enabled = false;
        }

        void radioButton_pipeStable_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_pipeStableRatio.Checked)
                textBox_pipeStableRatio.Enabled = true;
            else
                textBox_pipeStableRatio.Enabled = false;

            if (radioButton_pipeStableAbsolute.Checked)
                textBox_pipeStableAbsolute.Enabled = true;
            else
                textBox_pipeStableAbsolute.Enabled = false;
        }
        void radioButton_tankStable_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_tankStableRatio.Checked)
                textBox_tankStableRatio.Enabled = true;
            else
                textBox_tankStableRatio.Enabled = false;

            if (radioButton_tankStableAbsolute.Checked)
                textBox_tankStableAbsolute.Enabled = true;
            else
                textBox_tankStableAbsolute.Enabled = false;
        }   
        #endregion

        #region TextBox 이벤트
        void textBox_Decimal_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;
             
            int keyCode = (int)e.KeyChar;  // 46: Point              
            if ((keyCode < 48 || keyCode > 57) && keyCode != 8 && keyCode != 46)
            {
                if (textBox.Name == "textBox_minTemp" && keyCode == 45)
                {
                    if (textBox.Text.Contains("-"))
                        e.Handled = true;
                    else
                        e.Handled = false;
                }
                else
                    e.Handled = true;
            }
            if (keyCode == 46)
            {
                if (string.IsNullOrEmpty(textBox.Text) || textBox.Text.Contains('.') == true)
                {
                    e.Handled = true;
                }
            }
        }

        void textBox_Integer_KeyPress(object sender, KeyPressEventArgs e)
        {
            //정수만
            int keyCode = (int)e.KeyChar;
            if ((keyCode < 48 || keyCode > 57) && keyCode != 8)
                e.Handled = true;
        } 
        #endregion
         
        public static string LoadPublicMessage()
        {
            string strSQL = "Select Message from PublicMessage where SiteID = " + MainForm.Instance.SiteID + " and ID = (Select max(ID) from PublicMessage)";
            ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            //string ss = System.Text.RegularExpressions.Regex.Replace(arrResult[0].ToString(), "(?<!\r)\n", "");

            return WebDBManager.GetStringField(arrResult[0]);
        }

        private void LoadOptions()
        {
            string strSQL = "Select ID, PropertyName, PropertyValue from kpx.Options where SiteID = " + MainForm.Instance.SiteID;
            ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-2;i+=3)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strPropertyName = WebDBManager.GetStringField(arrResult[i + 1]);
                string strPropertyValue = WebDBManager.GetStringField(arrResult[i + 2]);

                if (strPropertyName == null || strPropertyValue == null || id == null)
                    continue;

                if (string.Compare(strPropertyName, "UseSMS", true) == 0)
                {
                    int value;
                    string strValue = strPropertyValue.Trim().ToLower();

                    if (int.TryParse(strValue, out value))
                    {
                        if (value == 0)
                            m_useSMSDB = new VariousData<bool>(false);
                        else
                            m_useSMSDB = new VariousData<bool>(true);
                    }
                    else if (strValue == "true")
                        m_useSMSDB = new VariousData<bool>(true);
                    else if (strValue == "false")
                        m_useSMSDB = new VariousData<bool>(false);
                    else
                        continue;

                    m_useSMS = m_useSMSDB.Data;
                    m_nUseSMSID = id.Data;

                    if (m_useSMS)
                        CheckSMS(true);
                } 
                else if (string.Compare(strPropertyName, "BeginPointDeltaT", true) == 0)
                {
                    if (ReadPressureOption(strPropertyValue, ref m_beginPointDeltaTDB, textBox_alarmOccurSecond, false))
                        m_nBeginPointDeltaTDB = id.Data;
                }
                else if (string.Compare(strPropertyName, "IgnoreTime", true) == 0)
                {
                    if (ReadPressureOption(strPropertyValue, ref m_ignoreTime, textBox_alarmIgnoreMinute, false))
                        m_nIgnoreTime = id.Data; 
                } 
            }
        }

        private bool ReadPressureOption(string strValue, ref VariousData<double> data, TextBox textBox, bool isDigits)
        {
            double value;

            if (double.TryParse(strValue, out value))
            {
                data = new VariousData<double>(value);
                if (isDigits)
                    textBox.Text = string.Format("{0:F2}", value);
                else
                    textBox.Text = value.ToString();
                return true;
            }

            return false;
        }
         
        private void pictureBoxTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = Control.MousePosition;
                m_ptOrigin = this.Location;
            }

            m_isClicked = true;
        }

        private void pictureBoxTitle_MouseMove(object sender, MouseEventArgs e)
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

        private void pictureBoxTitle_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;

            m_isClicked = false;
        }

        private void CheckBoxSMS_Click(object sender, EventArgs e)
        {
            m_useSMS = !m_useSMS;
            CheckSMS(m_useSMS);
        }

        Image checkedEdge = global::KpxPipeMonitoring.Properties.Resources.CheckedEdge;
        Image unCheckedEdge = global::KpxPipeMonitoring.Properties.Resources.UncheckedEdge;
        private void CheckSMS(bool isChecked)
        {
            if (isChecked)
                pictureCheckBoxSMS.Image = checkedEdge;
            else
                pictureCheckBoxSMS.Image = unCheckedEdge;
        }

        #region 버튼 이벤트
        private void btnClose_Click(object sender, EventArgs e)
        {
            button_cancel_Click(null, null);
        }
        private void button_ok_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidPressureCheck(textBox_alarmIgnoreMinute) == false)
                    return;

                if (ValidPressureCheck(textBox_alarmOccurSecond) == false)
                    return;

                if (m_useSMS != m_useSMSDB.Data)
                    SaveUseSMS();

                if (textBoxPublicMessage.Text.Trim() != m_strPublicMessageDB)
                    SavePublicMessage();

                if (IsSamePressure(textBox_alarmIgnoreMinute.Text.Trim(), m_ignoreTime) == false)
                    SavePressure(textBox_alarmIgnoreMinute.Text, m_nIgnoreTime, "IgnoreTime", "알람 무시 시간(m)");

                if (IsSamePressure(textBox_alarmOccurSecond.Text.Trim(), m_beginPointDeltaTDB) == false)
                    SavePressure(textBox_alarmOccurSecond.Text, m_nBeginPointDeltaTDB, "BeginPointDeltaT", "작업 시작구간 단위시간(s)");

                this.Close();
            }
            catch (ApplicationException app)
            {
                UnE.Utility.UMessageBox.Show(app.Message);
            }
            catch (Exception ex)
            {
                UnE.Utility.UMessageBox.Show(ex.Message);
            }
            //this.Close();
        } 
        #endregion

        private bool SomethingChanged()
        {
            if (m_useSMS != m_useSMSDB.Data)
                return true; 

            if (textBoxPublicMessage.Text.Trim() != m_strPublicMessageDB)
                return true;

            return false;
        }

        private bool ValidPressureCheck(TextBox textBox)
        {
            string strPressure = textBox.Text.Trim();

            if (strPressure.Length == 0)
                return true;

            double pressure;

            if (double.TryParse(strPressure, out pressure) == false)
            {
                textBox.Focus();
                UnE.Utility.UMessageBox.Show("유효한 압력값이 아닙니다.");
                //MessageBox.Show("유효한 압력값이 아닙니다.");
                return false;
            }

            if (pressure <= 0.0)
            {
                textBox.Focus();
                UnE.Utility.UMessageBox.Show("압력값은 0보다 큰 값이어야 합니다.");
                //MessageBox.Show("압력값은 0보다 큰 값이어야 합니다.");
                return false;
            }

            return true;
        } 

        private void SavePublicMessage()
        {
            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

            int nID = commonFunction.GetMaxTableID("PublicMessage") + 1;

            string strSQL = string.Format("Insert into PublicMessage (ID, TimeStamp, Message, SiteID) values ({0}, '{1}', '{2}', {3})",
                nID, strTime, textBoxPublicMessage.Text.Trim(), MainForm.Instance.SiteID);
            if (MainForm.Instance.dbMgr.GetResultData(strSQL, 0) != null)
                MainForm.Instance.isChgPublicMsg = true;
        }

        private void SavePressure(string strPressure, int nID, string strPropertyName, string strDescription)
        {
            double data;

            if (double.TryParse(strPressure, out data) == false)
                return;

            if (nID < 0)
            {
                string strSQL = "Select ID from Options where PropertyName = '" + strPropertyName + "' and SiteID = " + MainForm.Instance.SiteID;
                ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null || arrResult.Count == 0)
                {
                    m_nUseSMSID = commonFunction.GetMaxTableID("Options") + 1;

                    strSQL = string.Format("Insert into Options (ID, PropertyName, PropertyValue, SiteID, Description) values ({0}, '{1}', '{2}', {3}, '{4}'",
                        nID, strPropertyName, strPressure, MainForm.Instance.SiteID, strDescription);

                    MainForm.Instance.dbMgr.GetResultData(strSQL, 0);
                    return;
                }

                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

                if (id == null)
                    return;

                nID = id.Data;
            }

            string strSQL2 = "Update Options set PropertyValue = '" + strPressure + "' where ID = " + nID.ToString();
            MainForm.Instance.dbMgr.GetResultData(strSQL2, 0); 
        }

        private bool IsSamePressure(string strPressure, VariousData<double> deltaPDB)
        {
            if (strPressure.Length == 0)
            {
                if (deltaPDB == null)
                    return true;
                else
                    return false;
            }
            else if (deltaPDB == null)
                return false;

            string strDBPressure = string.Format("{0:F2}", deltaPDB.Data);
            return strPressure == strDBPressure;
        }

        private void SaveUseSMS()
        {
            if (m_nUseSMSID < 0)
            {
                string strSQL = "Select ID from Options where PropertyName = 'UseSMS' and SiteID = " + MainForm.Instance.SiteID;
                ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null || arrResult.Count == 0)
                {
                    m_nUseSMSID = commonFunction.GetMaxTableID("Options") + 1;

                    strSQL = string.Format("Insert into Options (ID, PropertyName, PropertyValue, SiteID, Description) values ({0}, 'UseSMS', '{1}', {2}, '알람 발생시 문자메시지를 보낼 것인지 여부'",
                        m_nUseSMSID, m_useSMS ? 1 : 0, MainForm.Instance.SiteID);

                    MainForm.Instance.dbMgr.GetResultData(strSQL, 0);
                    return;
                }

                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

                if (id == null)
                    return;

                m_nUseSMSID = id.Data;
            }

            string strValue = m_useSMS ? "1" : "0";
            string strSQL2 = "Update Options set PropertyValue = '" + strValue + "' where ID = " + m_nUseSMSID.ToString();
            MainForm.Instance.dbMgr.GetResultData(strSQL2, 0);
        } 

        private void button_cancel_Click(object sender, EventArgs e)
        {
            if (SomethingChanged())
            {
                if (UnE.Utility.UMessageBox.Show("저장하지 않은 변경사항이 있습니다.\r\n그냥 창을 닫으시겠습니까?", "확인", MessageBoxButtons.YesNo)
                    == System.Windows.Forms.DialogResult.No)
                    return;
                /*if (MessageBox.Show("저장하지 않은 변경사항이 있습니다.\r\n그냥 창을 닫으시겠습니까?", "확인", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    return;*/
            }

            this.Close();
        }
        Image optionCloseMouseover = global::KpxPipeMonitoring.Properties.Resources.OptionClose_mouseover;
        Image optionCloseNormal = global::KpxPipeMonitoring.Properties.Resources.OptionClose_normal;
        private void btnClose_MouseEnter(object sender, EventArgs e)
        {
            this.btnClose.BackgroundImage = optionCloseMouseover;
        }

        private void btnClose_MouseLeave(object sender, EventArgs e)
        {
            this.btnClose.BackgroundImage = optionCloseNormal;
        }

        private void btnMemberInfo_Click(object sender, EventArgs e)
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = "TeamEditor.exe";
            startInfo.WorkingDirectory = GetExecutablePath();
            startInfo.ErrorDialog = true;
            startInfo.Arguments = "1 " + MainForm.Instance.SiteID + " 1";

            System.Diagnostics.Process process;
            try
            {
                process = System.Diagnostics.Process.Start(startInfo);
            }
            catch (Exception)
            {
                UnE.Utility.UMessageBox.Show("TeamEditor.exe를 실행할 수 없습니다.");
                //System.Windows.Forms.MessageBox.Show("TeamEditor.exe를 실행할 수 없습니다.");
            }
        }

        private string GetExecutablePath()
        {
            string strExePath = Application.ExecutablePath;
            int nIndex = strExePath.LastIndexOf('\\');
            string strTemp = strExePath.Substring(0, nIndex);

            return strTemp + "\\";
        }

        private void btnManager_Click(object sender, EventArgs e)
        {
            FormEditManager frm = new FormEditManager();
            frm.ShowDialog(this);
        } 

        #region 압력 유량 안정범위
        private void LoadStable()
        { 
            foreach (CommonFunction.TankInfo item in MainForm.Instance.tankInfo)
            { 
                comboBox_tankStable.Items.Add(item); 
            }
            
            comboBox_tankStable.DisplayMember = "strTankName";
            comboBox_tankStable.ValueMember = "nTankID";            
            comboBox_tankStable.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_tankStable.SelectedIndexChanged += comboBox_tankStable_SelectedIndexChanged;
            if (comboBox_tankStable.Items.Count > 0)
                comboBox_tankStable.SelectedIndex = 0;

            foreach (CommonFunction.PipeInfo item in MainForm.Instance.pipeInfo)
            {
                comboBox_pipeStable.Items.Add(item);
            }

            comboBox_pipeStable.DisplayMember = "strPipeName";
            comboBox_pipeStable.ValueMember = "nPipeID";
            comboBox_pipeStable.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_pipeStable.SelectedIndexChanged += comboBox_pipeStable_SelectedIndexChanged;
            if (comboBox_pipeStable.Items.Count > 0)
                comboBox_pipeStable.SelectedIndex = 0;  
        }

        void comboBox_pipeStable_SelectedIndexChanged(object sender, EventArgs e)
        {
            CommonFunction.PipeInfo pipeInfo = (CommonFunction.PipeInfo)comboBox_pipeStable.SelectedItem;
            int nSelectedPipeId = pipeInfo.nPipeID;

            foreach (CommonFunction.AlarmPipeOptionInfo item in MainForm.Instance.alarmPipeOptionInfo)
            {
                if (item.nPipeID == nSelectedPipeId)
                {
                    // 배관
                    textBox_pipeStableRatio.Text = item.nPipeStableRatio.ToString();
                    textBox_pipeStableAbsolute.Text = item.nPipeStableAbsolute.ToString(); 
                    textBox_pipeStableCTime.Text = item.nPipeStableCTime.ToString(); 

                    if (item.nPipeStableType == 0)
                        radioButton_pipeStableRatio.Checked = true;
                    else
                        radioButton_pipeStableAbsolute.Checked = true;

                    if (item.nPipeStableCTimeUse == 0)
                        checkBox_pipeStableCTimeUse.Checked = false;
                    else
                        checkBox_pipeStableCTimeUse.Checked = true;
                                         
                    break;
                }
            } 
        }

        void comboBox_tankStable_SelectedIndexChanged(object sender, EventArgs e)
        { 
            CommonFunction.TankInfo tankInfo = (CommonFunction.TankInfo)comboBox_tankStable.SelectedItem;
            int nSelectedTankId = tankInfo.nTankID;

            foreach (CommonFunction.AlarmTankOptionInfo item in MainForm.Instance.alarmTankOptionInfo)
            {
                if (item.nTankID == nSelectedTankId)
                { 
                    textBox_stableBeginWorkM.Text = item.nStableBeginWorkM.ToString(); 
                    textBox_alarmInterval.Text = TimeSpan.FromSeconds(item.nAlarmInterval).TotalMinutes.ToString(); //초단위 -> 분단위 변화
                     
                    if (item.nAlarmIntervalUse == 0)
                        checkBox_alarmIntervalUse.Checked = false;
                    else
                        checkBox_alarmIntervalUse.Checked = true;

                    // 탱크
                    textBox_tankStableRatio.Text = item.nTankStableRatio.ToString();
                    textBox_tankStableAbsolute.Text = item.nTankStableAbsolute.ToString(); 
                    textBox_tankStableCTime.Text = item.nTankStableCTime.ToString(); 

                    if (item.nTankStableType == 0)
                        radioButton_tankStableRatio.Checked = true;
                    else
                        radioButton_tankStableAbsolute.Checked = true;

                    if (item.nTankStableCTimeUse == 0)
                        checkBox_tankStableCTimeUse.Checked = false;
                    else
                        checkBox_tankStableCTimeUse.Checked = true; 

                    break;
                }
            }

            foreach (CommonFunction.TankInfo item in MainForm.Instance.tankInfo)
            {
                if (item.nTankID == nSelectedTankId)
                {
                    textBox_highLevel.Text = tankInfo.nHighLevel.ToString();
                    textBox_minTemp.Text = tankInfo.nMinTemp.ToString();
                    textBox_maxTemp.Text = tankInfo.nMaxTemp.ToString();
                    textBox_leakLevel.Text = tankInfo.nLeakLevel.ToString();
                    textBox_leakTime.Text = TimeSpan.FromSeconds(item.nLeakTime).TotalMinutes.ToString();
                }
            }
        }

        private void button_tankStableSave_Click(object sender, EventArgs e)
        {
            try
            {
                CommonFunction.AlarmTankOptionInfo alarmOptionInfo = MainForm.Instance.alarmTankOptionInfo[comboBox_tankStable.SelectedIndex];
                CommonFunction.TankInfo tankInfo = MainForm.Instance.tankInfo[comboBox_tankStable.SelectedIndex];

                int nTankID = alarmOptionInfo.nTankID;
                bool chgAllTank = checkBox_allTank.Checked;
                if (chgAllTank) 
                    nTankID = -1; 
                 
                int nOldStableBeginWorkM = alarmOptionInfo.nStableBeginWorkM;
                int nOldAlarmInterval = alarmOptionInfo.nAlarmInterval;
                bool bOldAlarmIntervalUse = (alarmOptionInfo.nAlarmIntervalUse == 0) ? false : true;

                int nStableBeginWorkM = 0;
                int.TryParse(textBox_stableBeginWorkM.Text, out nStableBeginWorkM);

                double nAlarmInterval = 0;
                double.TryParse(textBox_alarmInterval.Text, out nAlarmInterval);
                nAlarmInterval = Convert.ToInt32(TimeSpan.FromMinutes(nAlarmInterval).TotalSeconds);

                bool bAlarmIntervalUse = checkBox_alarmIntervalUse.Checked; 
                 
                // 탱크
                double nTankOldStableRatio = alarmOptionInfo.nTankStableRatio;
                double nTankOldStableAbsolute = alarmOptionInfo.nTankStableAbsolute; 
                int nTankOldStableCTime = alarmOptionInfo.nTankStableCTime;
                bool bTankOldStableCTimeUse = (alarmOptionInfo.nTankStableCTimeUse == 0) ? false : true;
                int nTankOldStableType = alarmOptionInfo.nTankStableType; 

                double nTankStableRatio = 0;
                double.TryParse(textBox_tankStableRatio.Text, out nTankStableRatio);
                nTankStableRatio = Math.Round(nTankStableRatio, 2);

                double nTankStableAbsolute = 0;
                double.TryParse(textBox_tankStableAbsolute.Text, out nTankStableAbsolute);
                nTankStableAbsolute = Math.Round(nTankStableAbsolute, 2);
                 
                int nTankStableCTime = 0;
                int.TryParse(textBox_tankStableCTime.Text, out nTankStableCTime);

                bool bTankStableCTimeUse = checkBox_tankStableCTimeUse.Checked;

                int nTankStableType = 0;
                if (radioButton_tankStableRatio.Checked)
                    nTankStableType = 0;
                else if (radioButton_tankStableAbsolute.Checked)
                    nTankStableType = 1;
                 
                double nOldHighLevel = tankInfo.nHighLevel;
                double nOldMinTemp = tankInfo.nMinTemp;
                double nOldMaxTemp = tankInfo.nMaxTemp;
                double nOldLeakLevel = tankInfo.nLeakLevel;
                double nOldLeakTime = tankInfo.nLeakTime;

                double nHighLevel = 0;
                double.TryParse(textBox_highLevel.Text, out nHighLevel);
                nHighLevel = Math.Round(nHighLevel, 2);

                double nMinTemp = 0;
                double.TryParse(textBox_minTemp.Text, out nMinTemp);
                nMinTemp = Math.Round(nMinTemp, 2);

                double nMaxTemp = 0;
                double.TryParse(textBox_maxTemp.Text, out nMaxTemp);
                nMaxTemp = Math.Round(nMaxTemp, 2);

                double nLeakLevel = 0;
                double.TryParse(textBox_leakLevel.Text, out nLeakLevel);
                nLeakLevel = Math.Round(nLeakLevel, 2);

                double nLeakTime = 0;
                double.TryParse(textBox_leakTime.Text, out nLeakTime);
                nLeakTime = Convert.ToInt32(TimeSpan.FromMinutes(nLeakTime).TotalSeconds);

                if (nTankOldStableRatio == nTankStableRatio && nTankOldStableAbsolute == nTankStableAbsolute && nTankOldStableCTime == nTankStableCTime && 
                    bTankOldStableCTimeUse == bTankStableCTimeUse && nTankOldStableType == nTankStableType && 
                    nOldStableBeginWorkM == nStableBeginWorkM && nOldAlarmInterval == nAlarmInterval &&
                    bOldAlarmIntervalUse == bAlarmIntervalUse && nOldHighLevel == nHighLevel && nOldMinTemp == nMinTemp && nOldMaxTemp == nMaxTemp &&
                    nOldLeakLevel == nLeakLevel && nOldLeakTime == nLeakTime)
                    throw new ApplicationException("변경된 내용이 없습니다.");

                if (chgAllTank)
                {
                    if (UnE.Utility.UMessageBox.Show("모든 탱크에 변경한 옵션이 적용됩니다.\r적용하시겠습니까?", "", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes) return;
                }
                this.Cursor = Cursors.WaitCursor;

                int nCommandID = commonFunction.GetMaxTableID("command") + 1;
                int nCommandHistoryID = commonFunction.GetMaxTableID("commandHistory") + 1;
                 
                // 공통
                if (nOldStableBeginWorkM != nStableBeginWorkM)
                {
                    SaveOptionSql(nCommandID, nCommandHistoryID, 6, nTankID, "StableBeginWorkM", nStableBeginWorkM);

                    //alarmOptionInfo.nStableBeginWorkM = nStableBeginWorkM;
                    nCommandID++;
                    nCommandHistoryID++;
                }
                if (nOldAlarmInterval != Convert.ToInt32(nAlarmInterval))
                {
                    SaveOptionSql(nCommandID, nCommandHistoryID, 6, nTankID, "AlarmInterval", nAlarmInterval);

                    //alarmOptionInfo.nAlarmInterval = Convert.ToInt32(nAlarmInterval);
                    nCommandID++;
                    nCommandHistoryID++;
                }
                if (bOldAlarmIntervalUse != bAlarmIntervalUse)
                {
                    SaveOptionSql(nCommandID, nCommandHistoryID, 6, nTankID, "AlarmIntervalUse", (bAlarmIntervalUse) ? 1 : 0);

                    //alarmOptionInfo.nAlarmIntervalUse = (bAlarmIntervalUse) ? 1 : 0;
                    nCommandID++;
                    nCommandHistoryID++;
                }
                 
                // 유량
                if (nTankOldStableRatio != nTankStableRatio)
                {
                    SaveOptionSql(nCommandID, nCommandHistoryID, 6, nTankID, "TankStableRatio", nTankStableRatio);

                    //alarmOptionInfo.nTankStableRatio = nTankStableRatio;
                    nCommandID++;
                    nCommandHistoryID++;
                }
                if (nTankOldStableAbsolute != nTankStableAbsolute)
                {
                    SaveOptionSql(nCommandID, nCommandHistoryID, 6, nTankID, "TankStableAbsolute", nTankStableAbsolute);

                    //alarmOptionInfo.nTankStableAbsolute = nTankStableAbsolute;
                    nCommandID++;
                    nCommandHistoryID++;
                }
                if (nTankOldStableType != nTankStableType)
                {
                    SaveOptionSql(nCommandID, nCommandHistoryID, 6, nTankID, "TankStableType", nTankStableType);

                    //alarmOptionInfo.nTankStableType = nTankStableType;
                    nCommandID++;
                    nCommandHistoryID++;
                }
                if (nTankOldStableCTime != nTankStableCTime)
                {
                    SaveOptionSql(nCommandID, nCommandHistoryID, 6, nTankID, "TankStableCTime", nTankStableCTime);

                    //alarmOptionInfo.nTankStableCTime = nTankStableCTime;
                    nCommandID++;
                    nCommandHistoryID++;
                }
                if (bTankOldStableCTimeUse != bTankStableCTimeUse)
                {
                    SaveOptionSql(nCommandID, nCommandHistoryID, 6, nTankID, "TankStableCTimeUse", (bTankStableCTimeUse) ? 1 : 0);

                    //alarmOptionInfo.nTankStableCTimeUse = (bTankStableCTimeUse) ? 1 : 0;
                    nCommandID++;
                    nCommandHistoryID++;
                }

                // 탱크 옵션
                if (nOldHighLevel != nHighLevel)
                {
                    SaveOptionSql(nCommandID, nCommandHistoryID, 7, nTankID, "HighLevel", nHighLevel);

                    tankInfo.nHighLevel = nHighLevel;
                    nCommandID++;
                    nCommandHistoryID++;
                }
                if (nOldMinTemp != nMinTemp)
                {
                    SaveOptionSql(nCommandID, nCommandHistoryID, 7, nTankID, "MinTemp", nMinTemp);

                    tankInfo.nMinTemp = nMinTemp;
                    nCommandID++;
                    nCommandHistoryID++;
                }
                if (nOldMaxTemp != nMaxTemp)
                {
                    SaveOptionSql(nCommandID, nCommandHistoryID, 7, nTankID, "MaxTemp", nMaxTemp);

                    tankInfo.nMaxTemp = nMaxTemp;
                    nCommandID++;
                    nCommandHistoryID++;
                }
                if (nOldLeakLevel != nLeakLevel)
                {
                    SaveOptionSql(nCommandID, nCommandHistoryID, 7, nTankID, "LeakLevel", nLeakLevel);

                    tankInfo.nLeakLevel = nLeakLevel;
                    nCommandID++;
                    nCommandHistoryID++;
                }
                if (nOldLeakTime != nLeakTime)
                {
                    SaveOptionSql(nCommandID, nCommandHistoryID, 7, nTankID, "LevelTime", nLeakTime);

                    tankInfo.nLeakTime = Convert.ToInt32(nLeakTime);
                    nCommandID++;
                    nCommandHistoryID++;
                }

                MainForm.Instance.DisplayOptions();
                this.Cursor = Cursors.Default;                
                UnE.Utility.UMessageBox.Show("저장되었습니다. "); 
            }
            catch (ApplicationException app)
            {
                this.Cursor = Cursors.Default;
                UnE.Utility.UMessageBox.Show(app.Message);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                UnE.Utility.UMessageBox.Show(ex.Message);
            }
        }

        private void button_pipeStableSave_Click(object sender, EventArgs e)
        {
            try
            {
                CommonFunction.AlarmPipeOptionInfo alarmOptionInfo = MainForm.Instance.alarmPipeOptionInfo[comboBox_pipeStable.SelectedIndex];
                CommonFunction.PipeInfo pipeInfo = MainForm.Instance.pipeInfo[comboBox_pipeStable.SelectedIndex];

                int nPipeID = alarmOptionInfo.nPipeID;
                bool chgAllPipe = checkBox_allPipe.Checked;
                if (chgAllPipe)
                    nPipeID = -1;
                 
                // 배관
                double nPipeOldStableRatio = alarmOptionInfo.nPipeStableRatio;
                double nPipeOldStableAbsolute = alarmOptionInfo.nPipeStableAbsolute;
                int nPipeOldStableCTime = alarmOptionInfo.nPipeStableCTime;
                bool bPipeOldStableCTimeUse = (alarmOptionInfo.nPipeStableCTimeUse == 0) ? false : true;
                int nPipeOldStableType = alarmOptionInfo.nPipeStableType;

                double nPipeStableRatio = 0;
                double.TryParse(textBox_pipeStableRatio.Text, out nPipeStableRatio);
                nPipeStableRatio = Math.Round(nPipeStableRatio, 2);

                double nPipeStableAbsolute = 0;
                double.TryParse(textBox_pipeStableAbsolute.Text, out nPipeStableAbsolute);
                nPipeStableAbsolute = Math.Round(nPipeStableAbsolute, 2);

                int nPipeStableCTime = 0;
                int.TryParse(textBox_pipeStableCTime.Text, out nPipeStableCTime);

                bool bPipeStableCTimeUse = checkBox_pipeStableCTimeUse.Checked;

                int nPipeStableType = 0;
                if (radioButton_pipeStableRatio.Checked)
                    nPipeStableType = 0;
                else if (radioButton_pipeStableAbsolute.Checked)
                    nPipeStableType = 1;
                 
                if (nPipeOldStableRatio == nPipeStableRatio &&
                    nPipeOldStableAbsolute == nPipeStableAbsolute && nPipeOldStableCTime == nPipeStableCTime &&
                    bPipeOldStableCTimeUse == bPipeStableCTimeUse && nPipeOldStableType == nPipeStableType)
                    throw new ApplicationException("변경된 내용이 없습니다.");

                if (chgAllPipe)
                {
                    if (UnE.Utility.UMessageBox.Show("모든 배관에 변경한 옵션이 적용됩니다.\r적용하시겠습니까?", "", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes) return;
                }
                this.Cursor = Cursors.WaitCursor;

                int nCommandID = commonFunction.GetMaxTableID("command") + 1;
                int nCommandHistoryID = commonFunction.GetMaxTableID("commandHistory") + 1;
                 
                // 압력
                if (nPipeOldStableRatio != nPipeStableRatio)
                {
                    SaveOptionSql(nCommandID, nCommandHistoryID, 9, nPipeID, "PipeStableRatio", nPipeStableRatio, "PipeID");

                    //alarmOptionInfo.nPipeStableRatio = nPipeStableRatio;
                    nCommandID++;
                    nCommandHistoryID++;
                }
                if (nPipeOldStableAbsolute != nPipeStableAbsolute)
                {
                    SaveOptionSql(nCommandID, nCommandHistoryID, 9, nPipeID, "PipeStableAbsolute", nPipeStableAbsolute, "PipeID");

                    //alarmOptionInfo.nPipeStableAbsolute = nPipeStableAbsolute;
                    nCommandID++;
                    nCommandHistoryID++;
                }
                if (nPipeOldStableType != nPipeStableType)
                {
                    SaveOptionSql(nCommandID, nCommandHistoryID, 9, nPipeID, "PipeStableType", nPipeStableType, "PipeID");

                    //alarmOptionInfo.nPipeStableType = nPipeStableType;
                    nCommandID++;
                    nCommandHistoryID++;
                }
                if (nPipeOldStableCTime != nPipeStableCTime)
                {
                    SaveOptionSql(nCommandID, nCommandHistoryID, 9, nPipeID, "PipeStableCTime", nPipeStableCTime, "PipeID");

                    //alarmOptionInfo.nPipeStableCTime = nPipeStableCTime;
                    nCommandID++;
                    nCommandHistoryID++;
                }
                if (bPipeOldStableCTimeUse != bPipeStableCTimeUse)
                {
                    SaveOptionSql(nCommandID, nCommandHistoryID, 9, nPipeID, "PipeStableCTimeUse", (bPipeStableCTimeUse) ? 1 : 0, "PipeID");

                    //alarmOptionInfo.nPipeStableCTimeUse = (bPipeStableCTimeUse) ? 1 : 0;
                    nCommandID++;
                    nCommandHistoryID++;
                }

                MainForm.Instance.DisplayOptions();
                this.Cursor = Cursors.Default;
                UnE.Utility.UMessageBox.Show("저장되었습니다. ");
            }
            catch (ApplicationException app)
            {
                this.Cursor = Cursors.Default;
                UnE.Utility.UMessageBox.Show(app.Message);
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                UnE.Utility.UMessageBox.Show(ex.Message);
            }
        }

        public void SaveOptionSql(int nCommandID, int nCommandHistoryID, int nCommandType, int nTankID, string strCommandName, object nCommandValue, string saveType = "TankID")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO command (ID, CommandType, TimeStamp, " + saveType + ", UserID, CommandName, CommandValue) ");
            sb.AppendFormat("VALUES ({0}, {1}, now(), {2}, {3}, '{4}', '{5}'); ", nCommandID, nCommandType, nTankID, MainForm.Instance.nUserID, strCommandName, nCommandValue);
            MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

            sb = new StringBuilder();
            sb.Append("INSERT INTO commandhistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, " + saveType + ", UserID, CmdID, CommandName, CommandValue) ");
            sb.AppendFormat("VALUES ({0}, {1}, now(), NULL, {2}, {3}, {4}, '{5}', '{6}'); ", nCommandHistoryID, nCommandType, nTankID, MainForm.Instance.nUserID, nCommandID, strCommandName, nCommandValue);
            MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
        }

        private void button_tankInit_Click(object sender, EventArgs e)
        { 
            CommonFunction.TankInfo tank = MainForm.Instance.tankInfo[comboBox_tankStable.SelectedIndex]; 
            if (tank == null) return;
             
            bool chgAllTank = checkBox_allTank.Checked;

            int nCommandID = commonFunction.GetMaxTableID("command") + 1;
            int nCommandHistoryID = commonFunction.GetMaxTableID("commandHistory") + 1;
             
            this.Cursor = Cursors.WaitCursor;

            int nTankID = tank.nTankID;
            if (chgAllTank)
            {
                nTankID = -1;
                if (UnE.Utility.UMessageBox.Show("모든 탱크의 옵션이 초기화됩니다.\r초기화하시겠습니까?", "", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                    return;
            }

            SaveOptionSql(nCommandID, nCommandHistoryID, 7, nTankID, "HighLevel", tank.nOrgHighLevel);
            nCommandID++;
            nCommandHistoryID++;
            SaveOptionSql(nCommandID, nCommandHistoryID, 7, nTankID, "MinTemp", tank.nOrgMinTemp);
            nCommandID++;
            nCommandHistoryID++;
            SaveOptionSql(nCommandID, nCommandHistoryID, 7, nTankID, "MaxTemp", tank.nOrgMaxTemp);
            nCommandID++;
            nCommandHistoryID++;
            SaveOptionSql(nCommandID, nCommandHistoryID, 7, nTankID, "LeakLevel", 1);
            nCommandID++;
            nCommandHistoryID++;
            SaveOptionSql(nCommandID, nCommandHistoryID, 7, nTankID, "LevelTime", 600); 

            tank.nHighLevel = tank.nOrgHighLevel;
            tank.nMinTemp = tank.nOrgMinTemp;
            tank.nMaxTemp = tank.nOrgMaxTemp;
            tank.nLeakLevel = 1;
            tank.nLeakTime = 600; 

            textBox_highLevel.Text = tank.nHighLevel.ToString();
            textBox_minTemp.Text = tank.nMinTemp.ToString();
            textBox_maxTemp.Text = tank.nMaxTemp.ToString();
            textBox_leakLevel.Text = "1";
            textBox_leakTime.Text = "600";
             
            this.Cursor = Cursors.Default;
            UnE.Utility.UMessageBox.Show("기본값으로 초기화되었습니다. ");
        }   
        #endregion   
    } 

    public class ComboItem
    {
        public string Text { get; set; }
        public int Value { get; set; }

        public ComboItem(string text, int value)
        {
            this.Text = text;
            this.Value = value;
        }
    }
}
