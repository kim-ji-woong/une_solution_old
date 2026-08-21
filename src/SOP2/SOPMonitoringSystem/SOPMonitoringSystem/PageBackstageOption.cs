using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using SOP;
using SDMS;
using DBUtility;

namespace SOPMonitoringSystem
{
    public interface IOptionChanged
    {
        event DeleteOptionChanged DeleteOptionChange;
        event WaitOptionChanged WaitOptionChange;
    }

    public delegate void DeleteOptionChanged(object sender, DeleteOptionChangeEventArgs e);
    public delegate void WaitOptionChanged(object sender, WaitOptionChangeEventArgs e);


    public partial class PageBackstageOption : Form, IOptionChanged
    {
        public event DeleteOptionChanged DeleteOptionChange;
        public event WaitOptionChanged WaitOptionChange;

        private Utility m_util = new Utility();
        string m_strFilePath;

        private int m_nDefaultBeginHour = 9;
        private int m_nDefaultBeginMinute = 0;
        private int m_nDefaultEndHour = 18;
        private int m_nDefaultEndMinute = 0;
        private string m_strPrevBeginHour = "";
        private string m_strPrevBeginMinute = "";
        private string m_strPrevEndHour = "";
        private string m_strPrevEndMinute = "";

        private bool m_ignoreChanged = false;

        public PageBackstageOption()
        {
            InitializeComponent();
            Initialize();
            InitConfigFile();

            Sections.WorkFlowManager.Instance.DeleteComplete = false;
            Sections.WorkFlowManager.Instance.WaitComplete = true;
            Sections.WorkFlowManager.Instance.InputWaitColor = btnWait.BackColor;
            Sections.WorkFlowManager.Instance.InProgressColor = btnProgress.BackColor;
            Sections.WorkFlowManager.Instance.CompleteColor = btnComplete.BackColor;
            Sections.WorkFlowManager.Instance.SkipColor = btnSkip.BackColor;

            cbSMS.Checked = FormMain.Instance.SMSOn;
            cbBroadcast.Checked = FormMain.Instance.UseBroadcast;
			checkBoxLegend.Checked = FormMain.Instance.ShowLegend;
        }

        public int getColor(int num)
        {
            int color = 0;

            switch(num)
            {
                case 0:
                    color = btnWait.BackColor.ToArgb();
                    break;
                case 1:
                    color = btnProgress.BackColor.ToArgb();
                    break;
                case 2:
                    color = btnComplete.BackColor.ToArgb();
                    break;
                case 3:
                    color = btnSkip.BackColor.ToArgb();
                    break;
            }

            return color;
        }

        public void Initialize()
        {
            Color colwait = Color.FromArgb((int)UInt32.Parse("4294957567"));
            Color colprogress = Color.FromArgb(172, 157, 247);// Color.FromArgb((int)UInt32.Parse("4294902015"));
            Color colcomplete = Color.FromArgb(154, 213, 247);// Color.FromArgb((int)UInt32.Parse("4290822336"));
            Color colskip = Color.FromArgb((int)UInt32.Parse("4294961535"));
            Color colodd = Color.FromArgb(234, 236, 236);// Color.FromArgb((int)UInt32.Parse("4292993535"));
            Color coleven = Color.FromArgb(255, 255, 255);// Color.FromArgb((int)UInt32.Parse("4292673535"));

            btnWait.BackColor = colwait;
            btnProgress.BackColor = colprogress;
            btnComplete.BackColor = colcomplete;
            btnSkip.BackColor = colskip;
            btnOdd.BackColor = colodd;
            btnEven.BackColor = coleven;

            m_ignoreChanged = true;
            int nBeginHour = 0, nBeginMinute = 0, nEndHour = 0, nEndMinute = 0;

            if (GetWorkingHours(ref nBeginHour, ref nBeginMinute, ref nEndHour, ref nEndMinute))
            {
                cboBeginHour.SelectedIndex = nBeginHour;
                cboBeginMinute.SelectedIndex = nBeginMinute;
                cboEndHour.SelectedIndex = nEndHour;
                cboEndMinute.SelectedIndex = nEndMinute;
            }
            else
            {
                cboBeginHour.SelectedIndex = m_nDefaultBeginHour;
                cboBeginMinute.SelectedIndex = m_nDefaultBeginMinute;
                cboEndHour.SelectedIndex = m_nDefaultEndHour;
                cboEndMinute.SelectedIndex = m_nDefaultEndMinute;
            }

            /*textBoxBeginHour.Text = m_nDefaultBeginHour.ToString();
            textBoxBeginMinute.Text = m_nDefaultBeginMinute.ToString();
            textBoxEndHour.Text = m_nDefaultEndHour.ToString();
            textBoxEndMinute.Text = m_nDefaultEndMinute.ToString();*/

            cbBroadcast.Checked = FormMain.Instance.LoadDBOption(SOPSimulatorConfig.GetPropertyName(SOPSimulatorConfig.ConfigType.USE_BROADCAST), "방송 사용여부");
            cbSMS.Checked = FormMain.Instance.LoadDBOption(SOPSimulatorConfig.GetPropertyName(SOPSimulatorConfig.ConfigType.USE_SMS), "문자 사용여부");

            m_ignoreChanged = false;

            ColorChange();
        }

        public void NotifyColor()
        {

        }

        private void InitConfigFile()
        {
			string strAppPath = "";
			try
			{
				strAppPath = Application.CommonAppDataPath + "\\SOP2";
			}
			catch (System.Deployment.Application.InvalidDeploymentException)
			{
				strAppPath = "C:\\ProgramData\\SOPMonitoringSystem\\SOPMonitoringSystem\\1.0.0.0\\SOP2";
			}
			if (!System.IO.Directory.Exists(strAppPath))
				System.IO.Directory.CreateDirectory(strAppPath);

            m_strFilePath = strAppPath + "\\config.ini";
            if (!System.IO.File.Exists(m_strFilePath))
            {
                //파일이 존재하지 않는 경우 config.ini 파일 생성;
                WriteConfigFile();
            }
            else
            {
                //파일이 존재할 경우 config.ini 파일을 읽어 색상 초기화
                ReadConfigFile();
            }
            
            ColorChange();
        }
        private bool bOnReadConfig = false;

        private void ReadConfigFile()
        {
            bOnReadConfig = true;
            string strwait = m_util.getinivalue("ColorInfo", "wait_color", m_strFilePath);
            string strprogress = m_util.getinivalue("ColorInfo", "progress_color", m_strFilePath);
            string strcomplete = m_util.getinivalue("ColorInfo", "complete_color", m_strFilePath);
            string strskip = m_util.getinivalue("ColorInfo", "skip_color", m_strFilePath);
            string strodd = m_util.getinivalue("ColorInfo", "odd_color", m_strFilePath);
            string streven = m_util.getinivalue("ColorInfo", "even_color", m_strFilePath);

            /*string strBeginHour = m_util.getinivalue("WorkTime", "begin_hour", m_strFilePath);
            string strBeginMinute = m_util.getinivalue("WorkTime", "begin_minute", m_strFilePath);
            string strEndHour = m_util.getinivalue("WorkTime", "end_hour", m_strFilePath);
            string strEndMinute = m_util.getinivalue("WorkTime", "end_minute", m_strFilePath);

            int nBeginHour = WebDBManager.GetIntField(strBeginHour, m_nDefaultBeginHour);
            int nBeginMinute = WebDBManager.GetIntField(strBeginMinute, m_nDefaultBeginMinute);
            int nEndHour = WebDBManager.GetIntField(strEndHour, m_nDefaultEndHour);
            int nEndMinute = WebDBManager.GetIntField(strEndMinute, m_nDefaultEndMinute);*/
            int nBeginHour = 0, nBeginMinute = 0, nEndHour = 0, nEndMinute = 0;

            GetWorkingHours(ref nBeginHour, ref nBeginMinute, ref nEndHour, ref nEndMinute);
            
            Color colwait = Color.FromArgb((int)UInt32.Parse(strwait));
            Color colprogress = Color.FromArgb((int)UInt32.Parse(strprogress));
            Color colcomplete = Color.FromArgb((int)UInt32.Parse(strcomplete));
            Color colskip = Color.FromArgb((int)UInt32.Parse(strskip));
            Color colodd = Color.FromArgb((int)UInt32.Parse(strodd));
            Color coleven = Color.FromArgb((int)UInt32.Parse(streven));

            /*btnWait.BackColor = colwait;
            btnProgress.BackColor = colprogress;
            btnComplete.BackColor = colcomplete;
            btnSkip.BackColor = colskip;*/
            btnOdd.BackColor = colodd;
            btnEven.BackColor = coleven;

            m_ignoreChanged = true;

            cboBeginHour.SelectedIndex = nBeginHour;
            cboBeginMinute.SelectedIndex = nBeginMinute;
            cboEndHour.SelectedIndex = nEndHour;
            cboEndMinute.SelectedIndex = nEndMinute;
            /*textBoxBeginHour.Text = nBeginHour.ToString();
            textBoxBeginMinute.Text = nBeginMinute.ToString();
            textBoxEndHour.Text = nEndHour.ToString();
            textBoxEndMinute.Text = nEndMinute.ToString();*/

            m_ignoreChanged = false;

            m_strPrevBeginHour = cboBeginHour.Text;
            m_strPrevBeginMinute = cboBeginMinute.Text;
            m_strPrevEndHour = cboEndHour.Text;
            m_strPrevEndMinute = cboEndMinute.Text;
            /*m_strPrevBeginHour = textBoxBeginHour.Text;
            m_strPrevBeginMinute = textBoxBeginMinute.Text;
            m_strPrevEndHour = textBoxEndHour.Text;
            m_strPrevEndMinute = textBoxEndMinute.Text;*/

            string strcontrol = m_util.getinivalue("OptionInfo", "control", m_strFilePath);
            string strruncomplete = m_util.getinivalue("OptionInfo", "complete", m_strFilePath);
            string strscenario = m_util.getinivalue("OptionInfo", "scenario", m_strFilePath);
            string strwatermark = m_util.getinivalue("OptionInfo", "watermark", m_strFilePath);
            string stralarm = m_util.getinivalue("OptionInfo", "alarm", m_strFilePath);
            string strShowMissionText = m_util.getinivalue("OptionInfo", "showMissionText", m_strFilePath);
            string strAutoFocusSection = m_util.getinivalue("OptionInfo", "autoFocusSection", m_strFilePath);

            bool isControl = false;
            if (strcontrol.ToLower() == "true")
                isControl = true;

            bool isComplete = false;
            if (strruncomplete.ToLower() == "true")
                isComplete = true;

            bool isScenario = false;
            if (strscenario.ToLower() == "true")
                isScenario = true;

            bool isWatermark = false;
            if (strwatermark.ToLower() == "true")
                isWatermark = true;

            bool isAlarm = false;
            if (stralarm.ToLower() == "true")
                isAlarm = true;

            bool showMissionText = true;
            if (strShowMissionText.ToLower() == "false")
                showMissionText = false;

            bool autoFocusSection = true;
            if (strAutoFocusSection.ToLower() == "false")
                autoFocusSection = false;

            rdoControl.Checked = isControl;
            rdoMonitoring.Checked = !isControl;
            checkBoxNext.Checked = isComplete;
            checkBoxRemove.Checked = isScenario;
            checkBoxWatermark.Checked = isWatermark;
            checkReceive.Checked = isAlarm;
            checkBoxShowMissionText.Checked = showMissionText;
            checkBoxAutoFocusSection.Checked = autoFocusSection;

            FormMain.Instance.ShowMissionText = showMissionText;
            FormMain.Instance.EnableFocusSection = autoFocusSection;

            bOnReadConfig = false;
        }

        private bool SetWorkingHours(string strBeginHour, string strBeginMinute, string strEndHour, string strEndMinute)
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;

            if (!SaveDBOption(dbMgr, SOPSimulatorConfig.GetPropertyName(SOPSimulatorConfig.ConfigType.WORKING_BEGIN_HOUR), strBeginHour + ":" + strBeginMinute, "근무시작시간(시간:분)"))
                return false;

            if (!SaveDBOption(dbMgr, SOPSimulatorConfig.GetPropertyName(SOPSimulatorConfig.ConfigType.WORKING_END_HOUR), strEndHour + ":" + strEndMinute, "근무종료시간(시간:분)"))
                return false;

            return true;
        }

        private bool SaveDBOption(WebDBManager dbMgr, string strPropertyName, string strPropertyValue, string strDescription = null)
        {
            string strSQL = "Select ID from OptionSOPSimulator where PropertyName = '" + strPropertyName + "'";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            if (arrResult.Count == 0)
                return InsertDBOption(dbMgr, strPropertyName, strPropertyValue, strDescription);

            int nID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);

            if (nID < 0)
                return InsertDBOption(dbMgr, strPropertyName, strPropertyValue, strDescription);

            return UpdateDBOption(dbMgr, nID, strPropertyValue);
        }

        private bool UpdateDBOption(WebDBManager dbMgr, int nID, string strPropertyValue)
        {
            string strSQL = string.Format("Update OptionSOPSimulator set PropertyValue = '{0}' where ID = {1}", strPropertyValue, nID);
            return dbMgr.GetResultData(strSQL, 0) != null;
        }

        private bool InsertDBOption(WebDBManager dbMgr, string strPropertyName, string strPropertyValue, string strDescription)
        {
            string strDesc = strDescription == null ? "NULL" : "'" + strDescription + "'";

            string strSQL = "Insert into OptionSOPSimulator (PropertyName, PropertyValue, Description) values ";
            strSQL += string.Format("('{0}', '{1}', {2})", strPropertyName, strPropertyValue, strDesc);

            return dbMgr.GetResultData(strSQL, 0) != null;
        }

        private bool GetWorkingHours(ref int nBeginHour, ref int nBeginMinute, ref int nEndHour, ref int nEndMinute)
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;

            if (!GetWorkingHours(dbMgr, SOPSimulatorConfig.GetPropertyName(SOPSimulatorConfig.ConfigType.WORKING_BEGIN_HOUR), ref nBeginHour, ref nBeginMinute))
                return false;

            if (!GetWorkingHours(dbMgr, SOPSimulatorConfig.GetPropertyName(SOPSimulatorConfig.ConfigType.WORKING_END_HOUR), ref nEndHour, ref nEndMinute))
                return false;

            return true;
        }

        private bool GetWorkingHours(WebDBManager dbMgr, string strPropertyName, ref int nHour, ref int nMinute)
        {
            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = '" + strPropertyName + "'";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            string strResult = WebDBManager.GetStringField(arrResult[0], "");

            if (!GetWorkingHours(strResult, ref nHour, ref nMinute))
                return false;

            return true;
        }

        private bool GetWorkingHours(string strWorkingHours, ref int nHour, ref int nMinute)
        {
            int nIndex = strWorkingHours.IndexOf(':');

            if (nIndex < 0)
                return false;

            string strHour = strWorkingHours.Substring(0, nIndex);
            string strMinute = strWorkingHours.Substring(nIndex + 1);

            if (!int.TryParse(strHour, out nHour))
                return false;

            if (!int.TryParse(strMinute, out nMinute))
                return false;

            if (nHour < 0 || nHour > 23)
                return false;

            if (nMinute < 0 || nMinute > 59)
                return false;

            return true;
        }

        private void WriteConfigFile()
        {
            if (bOnReadConfig == true)
                return;

            string strwait = "4294957567";
            string strprogress = Color.FromArgb(172, 157, 247).ToArgb().ToString();// "4294902015";
            string strcomplete = Color.FromArgb(154, 213, 247).ToArgb().ToString();//"4290822336";
            string strskip = "4294961535";
            string strodd = Color.FromArgb(234, 236, 236).ToArgb().ToString(); //"4294967295";
            string streven = Color.FromArgb(255, 255, 255).ToArgb().ToString(); //"4290822336";

            if (btnWait != null)
            {
                UInt32 wait = (UInt32)btnWait.BackColor.ToArgb();
                strwait = wait.ToString();
            }
            if (btnProgress != null)
            {
                UInt32 progress = (UInt32)btnProgress.BackColor.ToArgb();
                strprogress = progress.ToString();
            }
            if (btnComplete != null)
            {
                UInt32 complete = (UInt32)btnComplete.BackColor.ToArgb();
                strcomplete = complete.ToString();
            }
            if (btnSkip != null)
            {
                UInt32 skip = (UInt32)btnSkip.BackColor.ToArgb();
                strskip = skip.ToString();
            }
            if (btnOdd != null)
            {
                UInt32 odd = (UInt32)btnOdd.BackColor.ToArgb();
                strodd = odd.ToString();
            }
            if (btnEven != null)
            {
                UInt32 even = (UInt32)btnEven.BackColor.ToArgb();
                streven = even.ToString();
            }
            

            m_util.setinivalue("ColorInfo", "wait_color", strwait, m_strFilePath);
            m_util.setinivalue("ColorInfo", "progress_color", strprogress, m_strFilePath);
            m_util.setinivalue("ColorInfo", "complete_color", strcomplete, m_strFilePath);
            m_util.setinivalue("ColorInfo", "skip_color", strskip, m_strFilePath);

            m_util.setinivalue("ColorInfo", "odd_color", strodd, m_strFilePath);
            m_util.setinivalue("ColorInfo", "even_color", streven, m_strFilePath);

            string strcontrol = "true";//rdoControl.Checked.ToString();
            string strruncomplete = checkBoxNext.Checked.ToString();
            string strscenario = checkBoxRemove.Checked.ToString();
            string strwatermark = checkBoxWatermark.Checked.ToString();
            string stralarm = checkReceive.Checked.ToString();
            string strShowMissionText = checkBoxShowMissionText.Checked.ToString();
            string strAutoFocusSection = checkBoxAutoFocusSection.Checked.ToString();

            m_util.setinivalue("OptionInfo", "control", strcontrol, m_strFilePath);
            m_util.setinivalue("OptionInfo", "complete", strruncomplete, m_strFilePath);
            m_util.setinivalue("OptionInfo", "scenario", strscenario, m_strFilePath);
            m_util.setinivalue("OptionInfo", "watermark", strwatermark, m_strFilePath);
            m_util.setinivalue("OptionInfo", "alarm", stralarm, m_strFilePath);
            m_util.setinivalue("OptionInfo", "showMissionText", strShowMissionText, m_strFilePath);
            m_util.setinivalue("OptionInfo", "autoFocusSection", strAutoFocusSection, m_strFilePath);

            SetWorkingHours(cboBeginHour.Text, cboBeginMinute.Text, cboEndHour.Text, cboEndMinute.Text);
            /*m_util.setinivalue("WorkTime", "begin_hour", cboBeginHour.Text, m_strFilePath);
            m_util.setinivalue("WorkTime", "begin_minute", cboBeginMinute.Text, m_strFilePath);
            m_util.setinivalue("WorkTime", "end_hour", cboEndHour.Text, m_strFilePath);
            m_util.setinivalue("WorkTime", "end_minute", cboEndMinute.Text, m_strFilePath);*/
            /*m_util.setinivalue("WorkTime", "begin_hour", textBoxBeginHour.Text, m_strFilePath);
            m_util.setinivalue("WorkTime", "begin_minute", textBoxBeginMinute.Text, m_strFilePath);
            m_util.setinivalue("WorkTime", "end_hour", textBoxEndHour.Text, m_strFilePath);
            m_util.setinivalue("WorkTime", "end_minute", textBoxEndMinute.Text, m_strFilePath);*/
        }

        public void SOPInfo(string strPath)
        {
            dataGridSOPInfo.Rows.Clear();

            string[] strCategory = {"재난 카테고리", "재난 유형", "재난 상세 정의", "대응 단계"};
			string[] strValue = strPath.Split((char)0x06);
            int i = 0;
            foreach (string str in strCategory)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = null;

                cell = new DataGridViewTextBoxCell();
                cell.Value = strCategory[i];
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = strValue[i];
                gridRow.Cells.Add(cell);

                dataGridSOPInfo.Rows.Add(gridRow);
                i++;
            }
        }

        public void SOPVersion(VersionInfo version)
        {
            dataGridVersion.Rows.Clear();

            string[] strCategory = { "마지막으로 수정한 날짜", "만든 날짜", "버전", "설명", "만든 이" };

            int i = 0;
            foreach (string str in strCategory)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = null;

                cell = new DataGridViewTextBoxCell();
                cell.Value = strCategory[i];
                gridRow.Cells.Add(cell);

                dataGridVersion.Rows.Add(gridRow);
                i++;
            }

            dataGridVersion.Rows[0].Cells[1].Value = version.EndTime;
            dataGridVersion.Rows[1].Cells[1].Value = version.BeginTime;
            dataGridVersion.Rows[2].Cells[1].Value = version.VersionName;
            dataGridVersion.Rows[3].Cells[1].Value = version.Description;
            dataGridVersion.Rows[4].Cells[1].Value = version.UserName;
        }

        private void btnInitialize_Click(object sender, EventArgs e)
        {
            Initialize();

            WriteConfigFile();

            Sections.WorkFlowManager.Instance.InputWaitColor = btnWait.BackColor;
            Sections.WorkFlowManager.Instance.InProgressColor = btnProgress.BackColor;
            Sections.WorkFlowManager.Instance.CompleteColor = btnComplete.BackColor;
            Sections.WorkFlowManager.Instance.SkipColor = btnSkip.BackColor;
            Sections.WorkFlowManager.Instance.ChangeColor();
            if (FormMain.Instance.GetPageHome().tabControl.SelectedTab != null)
                FormMain.Instance.GetPageHome().tabControl.SelectedTab.Refresh();

            FormMain.Instance.GetPageHome().changecolor(0, btnWait.BackColor);
            FormMain.Instance.GetPageHome().changecolor(1, btnProgress.BackColor);
            FormMain.Instance.GetPageHome().changecolor(2, btnComplete.BackColor);
            FormMain.Instance.GetPageHome().changecolor(3, btnSkip.BackColor);
        }

        private void ColorButton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                if (btn == btnWait)
                {
                    btnWait.BackColor = colorDialog.Color;
                    Sections.WorkFlowManager.Instance.InputWaitColor = btnWait.BackColor;        
                    Sections.WorkFlowManager.Instance.ChangeColor();
                    if (FormMain.Instance.GetPageHome().tabControl.SelectedTab != null)
                        FormMain.Instance.GetPageHome().tabControl.SelectedTab.Refresh();

                    FormMain.Instance.GetPageHome().changecolor(0, colorDialog.Color); //0 : 대기 업무 / 1: 실행중 업무 / 2: 완료된 업무 / 3: 건너뛴 업무
                }
                else if (btn == btnProgress)
                {
                    btnProgress.BackColor = colorDialog.Color;
                    Sections.WorkFlowManager.Instance.InProgressColor = btnProgress.BackColor;
                    Sections.WorkFlowManager.Instance.ChangeColor();
                    if (FormMain.Instance.GetPageHome().tabControl.SelectedTab != null)
                        FormMain.Instance.GetPageHome().tabControl.SelectedTab.Refresh();

                    FormMain.Instance.GetPageHome().changecolor(1, colorDialog.Color); //0 : 대기 업무 / 1: 실행중 업무 / 2: 완료된 업무 / 3: 건너뛴 업무
                }
                else if (btn == btnComplete)
                {
                    btnComplete.BackColor = colorDialog.Color;
                    Sections.WorkFlowManager.Instance.CompleteColor = btnComplete.BackColor;
                    Sections.WorkFlowManager.Instance.ChangeColor();
                    if (FormMain.Instance.GetPageHome().tabControl.SelectedTab != null)
                        FormMain.Instance.GetPageHome().tabControl.SelectedTab.Refresh();

                    FormMain.Instance.GetPageHome().changecolor(2, colorDialog.Color); //0 : 대기 업무 / 1: 실행중 업무 / 2: 완료된 업무 / 3: 건너뛴 업무
                }
                else if (btn == btnSkip)
                {
                    btnSkip.BackColor = colorDialog.Color;
                    Sections.WorkFlowManager.Instance.SkipColor = btnSkip.BackColor;
                    Sections.WorkFlowManager.Instance.ChangeColor();
                    if (FormMain.Instance.GetPageHome().tabControl.SelectedTab != null)
                        FormMain.Instance.GetPageHome().tabControl.SelectedTab.Refresh();

                    FormMain.Instance.GetPageHome().changecolor(3, colorDialog.Color); //0 : 대기 업무 / 1: 실행중 업무 / 2: 완료된 업무 / 3: 건너뛴 업무
                }
                else if (btn == btnOdd)
                {
                    btnOdd.BackColor = colorDialog.Color;
                    FormMain.Instance.GetPageHome().ColorPanel1 = colorDialog.Color;
                    FormMain.Instance.GetPageHome().ColorChangedPanel();
                }
                else if (btn == btnEven)
                {
                    btnEven.BackColor = colorDialog.Color;
                    FormMain.Instance.GetPageHome().ColorPanel2 = colorDialog.Color;
                    FormMain.Instance.GetPageHome().ColorChangedPanel();
                }
            }
            
            WriteConfigFile();
        }
        
        public void ColorChange()
        {
            FormMain.Instance.GetPageHome().ColorPanel1 = btnOdd.BackColor;
            FormMain.Instance.GetPageHome().ColorPanel2 = btnEven.BackColor;
            FormMain.Instance.GetPageHome().ColorChangedPanel();

        }

        public bool GetVirtualMode()
        {
            return checkBoxWatermark.Checked;
        }

        public bool GetAfterNextLevel()
        {
            return checkBoxNext.Checked;
        }

        public bool GetAfterRemove()
        {
            return checkBoxRemove.Checked;
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox btn = (CheckBox)sender;

            if (btn == checkBoxWatermark)
            {
                Sections.TabPageManager.Instance.ChangeWaterMark(checkBoxWatermark.Checked);
                FormMain.Instance.GetPageHome().ChangeWaterMark(checkBoxWatermark.Checked);
            }
            else if(btn == checkBoxNext)
            {
                Sections.WorkFlowManager.Instance.WaitComplete = !Sections.WorkFlowManager.Instance.WaitComplete;
                if (WaitOptionChange != null)
                {
                    WaitOptionChange(this, new WaitOptionChangeEventArgs());
                }
            }
            else if (btn == checkBoxRemove)
            {
                Sections.WorkFlowManager.Instance.DeleteComplete = !Sections.WorkFlowManager.Instance.DeleteComplete;
                if (DeleteOptionChange != null)
                {
                    DeleteOptionChange(this, new DeleteOptionChangeEventArgs());
                }
            }
            else if (btn == checkReceive)
            {
                if (checkReceive.Checked == true)
                    FormMain.Instance.DoorBellCheck(true);
                else
                    FormMain.Instance.DoorBellCheck(false);
            }
            else if (btn == checkBoxShowMissionText)
            {
                FormMain.Instance.ShowMissionText = btn.Checked;

                if (!btn.Checked && PopupMissionText.Instance.Visible)
                    PopupMissionText.Instance.Visible = false;
            }
            else if (btn == checkBoxAutoFocusSection)
            {
                FormMain.Instance.EnableFocusSection = btn.Checked;
            }

            WriteConfigFile();
        }

        private void rdoControl_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton btn = (RadioButton)sender;
            if (rdoControl == btn)
                WriteConfigFile();

        }

        /*private void textBoxBeginHour_TextChanged(object sender, EventArgs e)
        {
            OnTextChanged(textBoxBeginHour, ref m_strPrevBeginHour, 0, 23);
        }

        private void textBoxBeginMinute_TextChanged(object sender, EventArgs e)
        {
            OnTextChanged(textBoxBeginMinute, ref m_strPrevBeginMinute, 0, 59);
        }

        private void textBoxEndHour_TextChanged(object sender, EventArgs e)
        {
            OnTextChanged(textBoxEndHour, ref m_strPrevEndHour, 0, 23);
        }

        private void textBoxEndMinute_TextChanged(object sender, EventArgs e)
        {
            OnTextChanged(textBoxEndMinute, ref m_strPrevEndMinute, 0, 59);
        }

        private bool OnTextChanged(TextBox textBox, ref string strPrevText, int nMin, int nMax)
        {
            try
            {
                int num = int.Parse(textBox.Text);

                if (num < nMin || num > nMax)
                {
                    MessageBox.Show(string.Format("{0}에서 {1} 사이의 값만 입력 가능합니다.", nMin, nMax));
                    textBox.Text = strPrevText;
                    return false;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                textBox.Text = strPrevText;
                return false;
            }

            strPrevText = textBox.Text;
            WriteConfigFile();
            return true;
        }*/

        public int BeginHour
        {
            get { return WebDBManager.GetIntField(cboBeginHour.Text, m_nDefaultBeginHour); }
            set { cboBeginHour.Text = value.ToString(); }
        }

        public int BeginMinute
        {
            get { return WebDBManager.GetIntField(cboBeginMinute.Text, m_nDefaultBeginMinute); }
            set { cboBeginMinute.Text = value.ToString(); }
        }

        public int EndHour
        {
            get { return WebDBManager.GetIntField(cboEndHour.Text, m_nDefaultEndHour); }
            set { cboEndHour.Text = value.ToString(); }
        }

        public int EndMinute
        {
            get { return WebDBManager.GetIntField(cboEndMinute.Text, m_nDefaultEndMinute); }
            set { cboEndMinute.Text = value.ToString(); }
        }

        private void cbBroadcast_CheckedChanged(object sender, EventArgs e)
        {
            bool bUse = cbBroadcast.Checked;
            FormMain.Instance.UseBroadcast = bUse;

            if (!m_ignoreChanged)
            {
                string strUse = bUse ? "1" : "0";
                FormMain.Instance.NetworkManager.SendChangedConfig(TCP_CLIENT.SOP_SIMULATOR, SOPSimulatorConfig.GetPropertyName(SOPSimulatorConfig.ConfigType.USE_BROADCAST), strUse);
                SaveDBOption(FormMain.Instance.DBManager, SOPSimulatorConfig.GetPropertyName(SOPSimulatorConfig.ConfigType.USE_BROADCAST), strUse, "방송 사용여부");
            }
        }

        private void cbSMS_CheckedChanged(object sender, EventArgs e)
        {
            bool bUse = cbSMS.Checked;
            FormMain.Instance.SMSOn = bUse;

            if (!m_ignoreChanged)
            {
                string strUse = bUse ? "1" : "0";
                FormMain.Instance.NetworkManager.SendChangedConfig(TCP_CLIENT.SOP_SIMULATOR, SOPSimulatorConfig.GetPropertyName(SOPSimulatorConfig.ConfigType.USE_SMS), strUse);
                SaveDBOption(FormMain.Instance.DBManager, SOPSimulatorConfig.GetPropertyName(SOPSimulatorConfig.ConfigType.USE_SMS), strUse, "문자 사용여부");
            }
        }

		private void checkBoxLegend_CheckedChanged(object sender, EventArgs e)
		{
			bool bShow = checkBoxLegend.Checked;
			FormMain.Instance.ShowLegend = bShow;
		}

        public bool GetVisbleMissionText()
        {
            return checkBoxShowMissionText.Checked;
        }

        private void cboWorkingHours_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_ignoreChanged)
            {
                if (FormMain.Instance.NetworkManager != null)
                {
                    if (sender == cboBeginHour || sender == cboBeginMinute)
                    {
                        FormMain.Instance.NetworkManager.SendChangedConfig(TCP_CLIENT.SOP_SIMULATOR, SOPSimulatorConfig.GetPropertyName(SOPSimulatorConfig.ConfigType.WORKING_BEGIN_HOUR), cboBeginHour.Text + ":" + cboBeginMinute.Text);
                    }
                    else
                    {
                        FormMain.Instance.NetworkManager.SendChangedConfig(TCP_CLIENT.SOP_SIMULATOR, SOPSimulatorConfig.GetPropertyName(SOPSimulatorConfig.ConfigType.WORKING_END_HOUR), cboEndHour.Text + ":" + cboEndMinute.Text);
                    }
                }

                SetWorkingHours(cboBeginHour.Text, cboBeginMinute.Text, cboEndHour.Text, cboEndMinute.Text);                
            }
        }

        private void labelAutoFocusSection_Click(object sender, EventArgs e)
        {
            checkBoxAutoFocusSection.Checked = !checkBoxAutoFocusSection.Checked;
            checkBox_CheckedChanged(checkBoxAutoFocusSection, null);
        }
    }

    public class DeleteOptionChangeEventArgs 
    {
        public DeleteOptionChangeEventArgs()
        {
        
        }
    }

    public class WaitOptionChangeEventArgs
    {
        public WaitOptionChangeEventArgs()
        {
        
        }
    }
}
