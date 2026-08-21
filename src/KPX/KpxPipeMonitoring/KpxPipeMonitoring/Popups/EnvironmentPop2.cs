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
    public partial class EnvironmentPop2 : Form
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

        public EnvironmentPop2()
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
            
            label1.Visible = false;
            textBox_alarmOccurSecond.Visible = false;
            label12.Visible = false;

            labelTitle.Parent = pictureBoxTitle;
            btnClose.Parent = pictureBoxTitle;

            commonFunction = new CommonFunction();
                        
            textBox_alarmIgnoreMinute.KeyPress += textBox_Integer_KeyPress;
            textBox_alarmOccurSecond.KeyPress += textBox_Integer_KeyPress;
                        
            checkBox_pipeStableCTimeUse.CheckedChanged += checkBox_pipeStableCTimeUse_CheckedChanged;
            
            checkBox_tankStableCTimeUse.CheckedChanged += checkBox_tankStableCTimeUse_CheckedChanged;

            groupBox7.Visible = false;

            
            SetSpinCtrl(nud_stableBeginWorkM, 0, 0, 1000); // 작업 시작 후 n분 기준
            SetSpinCtrl(nud_alarmInterval, 0, 0, 1000); // 한번 발생한 알람은 종료 후 n분 이내에 다시 발생하지 않는다
            SetSpinCtrl(nud_tankStableRatio, 0, 1, 100); // 유량 안정범위 비율
            SetSpinCtrl(nud_tankStableCTime, 0, 0, 1000); // 유량 안정범위 유지시간
            SetSpinCtrl(nud_density, 2, 0, 100); // 비중
            SetSpinCtrl(nud_highLevel, 2, 0, 100); // 레벨 상한
            SetSpinCtrl(nud_maxTemp, 1, -100, 100); // 온도 상한
            SetSpinCtrl(nud_minTemp, 1, -100, 100); // 온도 하한
            SetSpinCtrl(nud_pipeStableRatio, 0, 1, 100); // 압력 안정범위 비율
            SetSpinCtrl(nud_pipeStableCTime, 0, 0, 1000); // 압력 안정범위 유지시간

            SetSpinCtrl(nud_pipeStableRatio, 0, 5, 50);
        }

        private void SetSpinCtrl(NumericUpDown spin, int decimalPlaces, int minimum, int maximum)
        {
            spin.Minimum = minimum;
            spin.Maximum = maximum;            
            spin.DecimalPlaces = decimalPlaces;
            spin.Increment = (decimalPlaces > 0) ? 0.1m : 1;
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
                nud_pipeStableCTime.Enabled = true;
            else
                nud_pipeStableCTime.Enabled = false;
        }
        void checkBox_tankStableCTimeUse_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_tankStableCTimeUse.Checked)
                nud_tankStableCTime.Enabled = true;
            else
                nud_tankStableCTime.Enabled = false;
        }

        #endregion

        #region TextBox 이벤트        
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

        private void button_saveEtc_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (m_useSMS != m_useSMSDB.Data)
                    SaveUseSMS();

                if (textBoxPublicMessage.Text.Trim() != m_strPublicMessageDB)
                    SavePublicMessage();

                this.Cursor = Cursors.Default;
                UnE.Utility.UMessageBox.Show("저장되었습니다. ");
            }
            catch (ApplicationException app)
            {
                UnE.Utility.UMessageBox.Show(app.Message);
            }
            catch (Exception ex)
            {
                UnE.Utility.UMessageBox.Show(ex.Message);
            }
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
            //if (SomethingChanged())
            //{
            //    if (UnE.Utility.UMessageBox.Show("저장하지 않은 변경사항이 있습니다.\r\n그냥 창을 닫으시겠습니까?", "확인", MessageBoxButtons.YesNo)
            //        == System.Windows.Forms.DialogResult.No)
            //        return;
            //    /*if (MessageBox.Show("저장하지 않은 변경사항이 있습니다.\r\n그냥 창을 닫으시겠습니까?", "확인", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
            //        return;*/
            //}

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
            textBox_pipeName.Text = pipeInfo.strPipeName;
            textBox_pipeType.Text = pipeInfo.strPipeType;

            foreach (CommonFunction.AlarmPipeOptionInfo item in MainForm.Instance.alarmPipeOptionInfo)
            {
                if (item.nPipeID == nSelectedPipeId)
                {
                    // 배관                    
                    nud_pipeStableRatio.Value = (decimal)item.nPipeStableRatio;
                    nud_pipeStableCTime.Value = (decimal)item.nPipeStableCTime; 
                    
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
            textBox_tankName.Text = tankInfo.strTankName;
            textBox_tankType.Text = tankInfo.strType;
            textBox_liquidType.Text = tankInfo.strLiquidType;
            nud_density.Value = (decimal)tankInfo.nDensity;

            foreach (CommonFunction.AlarmTankOptionInfo item in MainForm.Instance.alarmTankOptionInfo)
            {
                if (item.nTankID == nSelectedTankId)
                {
                    nud_stableBeginWorkM.Value = (decimal)item.nStableBeginWorkM;
                    nud_alarmInterval.Text = TimeSpan.FromSeconds(item.nAlarmInterval).TotalMinutes.ToString(); //초단위 -> 분단위 변화
                     
                    if (item.nAlarmIntervalUse == 0)
                        checkBox_alarmIntervalUse.Checked = false;
                    else
                        checkBox_alarmIntervalUse.Checked = true;

                    // 탱크
                    nud_tankStableRatio.Value = (decimal)item.nTankStableRatio;
                    nud_tankStableCTime.Value = (decimal)item.nTankStableCTime; 
                    
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
                    nud_highLevel.Value = (decimal)tankInfo.nHighLevel;
                    nud_minTemp.Value = (decimal)tankInfo.nMinTemp;
                    nud_maxTemp.Value = (decimal)tankInfo.nMaxTemp;
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
                int.TryParse(nud_stableBeginWorkM.Value.ToString(), out nStableBeginWorkM);

                double nAlarmInterval = 0;
                double.TryParse(nud_alarmInterval.Value.ToString(), out nAlarmInterval);
                nAlarmInterval = Convert.ToInt32(TimeSpan.FromMinutes(nAlarmInterval).TotalSeconds);

                bool bAlarmIntervalUse = checkBox_alarmIntervalUse.Checked; 
                 
                // 탱크
                double nTankOldStableRatio = alarmOptionInfo.nTankStableRatio;
                double nTankOldStableAbsolute = alarmOptionInfo.nTankStableAbsolute; 
                int nTankOldStableCTime = alarmOptionInfo.nTankStableCTime;
                bool bTankOldStableCTimeUse = (alarmOptionInfo.nTankStableCTimeUse == 0) ? false : true;
                int nTankOldStableType = alarmOptionInfo.nTankStableType; 

                double nTankStableRatio = 0;
                double.TryParse(nud_tankStableRatio.Value.ToString(), out nTankStableRatio);
                nTankStableRatio = Math.Round(nTankStableRatio, 2);
                                 
                int nTankStableCTime = 0;
                int.TryParse(nud_tankStableCTime.Value.ToString(), out nTankStableCTime);

                bool bTankStableCTimeUse = checkBox_tankStableCTimeUse.Checked;
                
                double nOldHighLevel = tankInfo.nHighLevel;
                double nOldMinTemp = tankInfo.nMinTemp;
                double nOldMaxTemp = tankInfo.nMaxTemp;
                double nOldLeakLevel = tankInfo.nLeakLevel;
                double nOldLeakTime = tankInfo.nLeakTime;

                double nHighLevel = 0;
                double.TryParse(nud_highLevel.Value.ToString(), out nHighLevel);
                nHighLevel = Math.Round(nHighLevel, 2);

                double nMinTemp = 0;
                double.TryParse(nud_minTemp.Value.ToString(), out nMinTemp);
                nMinTemp = Math.Round(nMinTemp, 2);

                double nMaxTemp = 0;
                double.TryParse(nud_maxTemp.Value.ToString(), out nMaxTemp);
                nMaxTemp = Math.Round(nMaxTemp, 2);
                
                if (nTankOldStableRatio == nTankStableRatio && nTankOldStableCTime == nTankStableCTime && 
                    bTankOldStableCTimeUse == bTankStableCTimeUse && 
                    nOldStableBeginWorkM == nStableBeginWorkM && nOldAlarmInterval == nAlarmInterval &&
                    bOldAlarmIntervalUse == bAlarmIntervalUse && nOldHighLevel == nHighLevel && nOldMinTemp == nMinTemp && 
                    nOldMaxTemp == nMaxTemp && tankInfo.strLiquidType == textBox_liquidType.Text && 
                    tankInfo.nDensity.ToString() == nud_density.Value.ToString() && tankInfo.strTankName == textBox_tankName.Text && tankInfo.strType == textBox_tankType.Text)
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

                //탱크 옵션 변경
                if (tankInfo.strLiquidType != textBox_liquidType.Text)
                {
                    string strSQL = "";
                    if (nTankID == -1)
                        strSQL = "Update tank set LiquidType = '" + textBox_liquidType.Text + "'";
                    else
                        strSQL = "Update tank set LiquidType = '" + textBox_liquidType.Text + "'" + " where ID = " + nTankID;
                    MainForm.Instance.dbMgr.GetResultData(strSQL, 0);
                }

                if (tankInfo.nDensity.ToString() != nud_density.Value.ToString())
                {
                    string strSQL = "";
                    if (nTankID == -1)
                        strSQL = "Update tank set Density = " + nud_density.Value;
                    else
                        strSQL = "Update tank set Density = " + nud_density.Value + " where ID = " + nTankID;
                    MainForm.Instance.dbMgr.GetResultData(strSQL, 0);
                }

                if (tankInfo.strTankName.ToString() != textBox_tankName.Text)
                {
                    string strSQL = "";
                    if (nTankID == -1)
                        strSQL = "Update tank set Name = '" + textBox_tankName.Text + "'";
                    else
                        strSQL = "Update tank set Name = '" + textBox_tankName.Text + "' where ID = " + nTankID;
                    MainForm.Instance.dbMgr.GetResultData(strSQL, 0);

                    int selectedIndex = 0;
                    comboBox_tankStable.Items.Clear();
                    for (int i = 0; i < MainForm.Instance.tankInfo.Count; i++)
                    {
                        CommonFunction.TankInfo info = MainForm.Instance.tankInfo[i];
                        if (nTankID == -1 || info.nTankID == nTankID)
                        {
                            info.strTankName = textBox_tankName.Text;
                        }

                        comboBox_tankStable.Items.Add(info);

                        if (tankInfo.nTankID == info.nTankID)
                            selectedIndex = i;
                    }

                    comboBox_tankStable.SelectedIndex = selectedIndex;
                }

                if (tankInfo.strType.ToString() != textBox_tankType.Text)
                {
                    string strSQL = "";
                    if (nTankID == -1)
                        strSQL = "Update tank set Type = '" + textBox_tankType.Text + "'";
                    else
                        strSQL = "Update tank set Type = '" + textBox_tankType.Text + "' where ID = " + nTankID;
                    MainForm.Instance.dbMgr.GetResultData(strSQL, 0);
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
                double.TryParse(nud_pipeStableRatio.Value.ToString(), out nPipeStableRatio);
                nPipeStableRatio = Math.Round(nPipeStableRatio, 2);
                
                int nPipeStableCTime = 0;
                int.TryParse(nud_pipeStableCTime.Value.ToString(), out nPipeStableCTime);

                bool bPipeStableCTimeUse = checkBox_pipeStableCTimeUse.Checked;
                
                if (nPipeOldStableRatio == nPipeStableRatio && nPipeOldStableCTime == nPipeStableCTime && bPipeOldStableCTimeUse == bPipeStableCTimeUse
                    && pipeInfo.strPipeType == textBox_pipeType.Text && pipeInfo.strPipeName == textBox_pipeName.Text)
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

                if (pipeInfo.strPipeType != textBox_pipeType.Text)
                {
                    string strSQL = "";
                    if (nPipeID == -1)
                        strSQL = "Update pipe set Type = '" + textBox_pipeType.Text + "'";
                    else
                        strSQL = "Update pipe set Type = '" + textBox_pipeType.Text + "' where ID = " + nPipeID;
                    MainForm.Instance.dbMgr.GetResultData(strSQL, 0);

                    //CommonFunction.PipeInfo info = comboBox_pipeStable.SelectedItem as CommonFunction.PipeInfo;
                    //info.strPipeType = textBox_pipeType.Text;
                }

                if (pipeInfo.strPipeName != textBox_pipeName.Text)
                {
                    string strSQL = "";
                    if (nPipeID == -1)
                        strSQL = "Update pipe set Name = '" + textBox_pipeName.Text + "'";
                    else
                        strSQL = "Update pipe set Name = '" + textBox_pipeName.Text + "' where ID = " + nPipeID;
                    MainForm.Instance.dbMgr.GetResultData(strSQL, 0);

                    int selectedIndex = 0;
                    comboBox_pipeStable.SelectedIndexChanged -= comboBox_pipeStable_SelectedIndexChanged;
                    comboBox_pipeStable.Items.Clear();
                    for (int i = 0; i < MainForm.Instance.pipeInfo.Count; i++)
                    {
                        CommonFunction.PipeInfo info = MainForm.Instance.pipeInfo[i];
                        if (nPipeID == -1 || info.nPipeID == nPipeID)
                        {
                            info.strPipeName = textBox_pipeName.Text;
                        }

                        comboBox_pipeStable.Items.Add(info);

                        if (pipeInfo.nPipeID == info.nPipeID)
                            selectedIndex = i;
                    }

                    comboBox_pipeStable.SelectedIndex = selectedIndex;
                    comboBox_pipeStable.SelectedIndexChanged += comboBox_pipeStable_SelectedIndexChanged;
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

            nud_highLevel.Value = (decimal)tank.nHighLevel;
            nud_minTemp.Value = (decimal)tank.nMinTemp;
            nud_maxTemp.Text = tank.nMaxTemp.ToString();
             
            this.Cursor = Cursors.Default;
            UnE.Utility.UMessageBox.Show("기본값으로 초기화되었습니다. ");
        }
        #endregion
    } 
}
