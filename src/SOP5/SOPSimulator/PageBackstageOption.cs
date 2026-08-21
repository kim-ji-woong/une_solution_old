using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using SDMS;
using DBUtility;
using UnE.SOP;
using UnE.SOP.Sections;
using UnE.SOP.Workstate;

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

        private PopupTranslucentForm mTranslucentForm = new PopupTranslucentForm();

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

        private int m_nSiteID = 1;
        private string m_strExternalFolderPath = "";

        private bool m_systemInput = false;

        public string ExternalFolderPath
        {
            get { return m_strExternalFolderPath; }
        }

        public PageBackstageOption()
        {
            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

            InitializeComponent();
            Initialize();
            InitConfigFile();

            WorkFlowManager.Instance.DeleteComplete = false;
            WorkFlowManager.Instance.WaitComplete = true;
            WorkFlowManager.Instance.CurrentColor = btnCurrent.BackColor;
            //WorkFlowManager.Instance.InputWaitColor = btnWait.BackColor;
            WorkFlowManager.Instance.InProgressColor = btnProgress.BackColor;
            WorkFlowManager.Instance.CompleteColor = btnComplete.BackColor;
            WorkFlowManager.Instance.SkipColor = btnSkip.BackColor;

            cbExternalMemberSMS.Checked = FormSOP.Instance.SmsExternalCompanyMemberOn;
            cbSMS.Checked = FormSOP.Instance.SMSOn;
            cbBroadcast.Checked = FormSOP.Instance.UseBroadcast;
			checkBoxLegend.Checked = FormSOP.Instance.ShowLegend;
             
            if (!UnE.SOP.ProxySOP.Instance.UsePSM && !UnE.SOP.ProxySOP.Instance.UseIntrusion)
            {
                groupBox_sopCfg.Size = new System.Drawing.Size(200, 122);
                groupBox_ttsServer.Location = new Point(groupBox_sopCfg.Location.X, 194);
                btnPSMSensorSOPLink.Visible = false;
                btnIntrusionSensorSOPLink.Visible = false;
            }
            if (UnE.SOP.ProxySOP.Instance.UsePSM && !UnE.SOP.ProxySOP.Instance.UseIntrusion)
            {
                groupBox_sopCfg.Size = new System.Drawing.Size(200, 163);
                groupBox_ttsServer.Location = new Point(groupBox_sopCfg.Location.X, 240);
                btnPSMSensorSOPLink.Visible = true;
                btnIntrusionSensorSOPLink.Visible = false;
            }
            if (!UnE.SOP.ProxySOP.Instance.UsePSM && UnE.SOP.ProxySOP.Instance.UseIntrusion)
            {
                groupBox_sopCfg.Size = new System.Drawing.Size(200, 163);
                groupBox_ttsServer.Location = new Point(groupBox_sopCfg.Location.X, 240);
                btnIntrusionSensorSOPLink.Location = new Point(6, 108);
                btnPSMSensorSOPLink.Visible = false;
                btnIntrusionSensorSOPLink.Visible = true;
            }

            ReadSOPPlayOnDetectSensor();
        }

        private void ReadSOPPlayOnDetectSensor()
        {
            string strPropertyName = SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.RUN_SOP_ON_LOADED);
            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = '" + strPropertyName + "' and SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL, 0);

            int nMode = 0;

            if (arrResult == null || arrResult.Count == 0)
            {
                int nID = GetMaxID("OptionSOPSimulator", FormSOP.Instance.DBManager) + 1;
                strSQL = "Insert into OptionSOPSimulator (ID, PropertyName, PropertyValue, SiteID, Description) values ";
                strSQL += string.Format("({0}, '{1}', '{2}', {3}, '센서 탐지 로딩 완료시 자동 시작(0 : 자동실행, 1 : 실행은 않고 열기만 함, 2 : 신호무시)')", nID, strPropertyName, m_nSiteID, nMode);
                FormSOP.Instance.DBManager.GetResultData(strSQL, 0);
            }
            else
            {
                VariousData<int> value = WebDBManager.GetIntField(arrResult[0].ToString());

                if (value != null)
                {
                    nMode = value.Data;
                }
            }

            if (nMode < 0 || nMode > 2)
                nMode = 0;

            m_systemInput = true;

            if (nMode == 0)
            {
                // SOP 자동실행
                ProxySOP.Instance.OpenSOPOnFireDetect = true;
                FormSOP.Instance.SensorDetectLoadAndPlay = true;
                radioRunSOP.Checked = true;
            }
            else if (nMode == 1)
            {
                // SOP 열기만 한다.
                ProxySOP.Instance.OpenSOPOnFireDetect = true;
                FormSOP.Instance.SensorDetectLoadAndPlay = false;
                radioLoadSOP.Checked = true;
            }
            else// if (nMode == 2)
            {
                // 센서신호 신경안씀
                ProxySOP.Instance.OpenSOPOnFireDetect = false;
                FormSOP.Instance.SensorDetectLoadAndPlay = false;
                radioNoSOP.Checked = true;
            }

            m_systemInput = false;
        }

        public int getColor(int num)
        {
            int color = 0;

            switch(num)
            {
                case 0:
                    color = btnCurrent.BackColor.ToArgb();
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



            cbBroadcast.Checked = FormSOP.Instance.LoadDBOption(SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.USE_BROADCAST), "방송 사용여부");
            cbSMS.Checked = FormSOP.Instance.LoadDBOption(SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.USE_SMS), "문자 사용여부");
            cbExternalMemberSMS.Checked = FormSOP.Instance.LoadDBOption(SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.SMS_TO_EXTERNAL_MEMBER), "외부회사직원에게 문자 전송");

            m_ignoreChanged = false;

            ColorChange();
        }

        public void NotifyColor()
        {
        }

        private void InitConfigFile()
        {
            WebDBManager dbMgr = FormSOP.Instance.DBManager;
            string folderName = dbMgr.DatabaseName.Trim().Replace("_", "");
			string strAppPath = "";
			try
			{
                string comAppData = Application.CommonAppDataPath;
                string comAppParent = System.IO.Path.GetDirectoryName(comAppData);

                strAppPath = comAppParent + "\\" + folderName;
			}
			catch (System.Deployment.Application.InvalidDeploymentException)
			{
                strAppPath = "C:\\ProgramData\\UnE\\SOPSimulator\\SOP1";
			}
			if (!System.IO.Directory.Exists(strAppPath))
				System.IO.Directory.CreateDirectory(strAppPath);

            m_strFilePath = strAppPath + "\\config.ini";
            if (!System.IO.File.Exists(m_strFilePath))
            {
                //파일이 존재하지 않는 경우 config.ini 파일 생성;
                Color colCurrent = WorkFlowManager.Instance.CurrentColor;
                //Color colwait = Color.FromArgb((int)UInt32.Parse("4294957567"));
                Color colprogress = Color.FromArgb(172, 157, 247);// Color.FromArgb((int)UInt32.Parse("4294902015"));
                Color colcomplete = Color.FromArgb(154, 213, 247);// Color.FromArgb((int)UInt32.Parse("4290822336"));
                Color colskip = Color.FromArgb((int)UInt32.Parse("4294961535"));
                Color colodd = Color.FromArgb(234, 236, 236);// Color.FromArgb((int)UInt32.Parse("4292993535"));
                Color coleven = Color.FromArgb(255, 255, 255);// Color.FromArgb((int)UInt32.Parse("4292673535"));

                btnCurrent.BackColor = colCurrent;
                //btnWait.BackColor = colwait;
                btnProgress.BackColor = colprogress;
                btnComplete.BackColor = colcomplete;
                btnSkip.BackColor = colskip;
                btnOdd.BackColor = colodd;
                btnEven.BackColor = coleven;

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
            string strCurrent = m_util.getinivalue("ColorInfo", "current_color", m_strFilePath);

            if (strCurrent.Length == 0)
            {
                strCurrent = ((uint)WorkFlowManager.Instance.CurrentColor.ToArgb()).ToString();
                m_util.setinivalue("ColorInfo", "current_color", strCurrent, m_strFilePath);
            }

            //string strwait = m_util.getinivalue("ColorInfo", "wait_color", m_strFilePath);
            string strprogress = m_util.getinivalue("ColorInfo", "progress_color", m_strFilePath);
            string strcomplete = m_util.getinivalue("ColorInfo", "complete_color", m_strFilePath);
            string strskip = m_util.getinivalue("ColorInfo", "skip_color", m_strFilePath);
            string strodd = m_util.getinivalue("ColorInfo", "odd_color", m_strFilePath);
            string streven = m_util.getinivalue("ColorInfo", "even_color", m_strFilePath);

            int nBeginHour = 0, nBeginMinute = 0, nEndHour = 0, nEndMinute = 0;

            GetWorkingHours(ref nBeginHour, ref nBeginMinute, ref nEndHour, ref nEndMinute);

            Color colCurrent = Color.FromArgb((int)UInt32.Parse(strCurrent));
            //Color colwait = Color.FromArgb((int)UInt32.Parse(strwait));
            Color colprogress = Color.FromArgb((int)UInt32.Parse(strprogress));
            Color colcomplete = Color.FromArgb((int)UInt32.Parse(strcomplete));
            Color colskip = Color.FromArgb((int)UInt32.Parse(strskip));
            Color colodd = Color.FromArgb((int)UInt32.Parse(strodd));
            Color coleven = Color.FromArgb((int)UInt32.Parse(streven));

            btnCurrent.BackColor = colCurrent;
            //btnWait.BackColor = colwait;
            btnProgress.BackColor = colprogress;
            btnComplete.BackColor = colcomplete;
            btnSkip.BackColor = colskip;
            btnOdd.BackColor = colodd;
            btnEven.BackColor = coleven;

            m_ignoreChanged = true;

            cboBeginHour.SelectedIndex = nBeginHour;
            cboBeginMinute.SelectedIndex = nBeginMinute;
            cboEndHour.SelectedIndex = nEndHour;
            cboEndMinute.SelectedIndex = nEndMinute;

            m_ignoreChanged = false;

            m_strPrevBeginHour = cboBeginHour.Text;
            m_strPrevBeginMinute = cboBeginMinute.Text;
            m_strPrevEndHour = cboEndHour.Text;
            m_strPrevEndMinute = cboEndMinute.Text;

            string strcontrol = m_util.getinivalue("OptionInfo", "control", m_strFilePath);
            string strruncomplete = m_util.getinivalue("OptionInfo", "complete", m_strFilePath);
            string strscenario = m_util.getinivalue("OptionInfo", "scenario", m_strFilePath);
            string strwatermark = m_util.getinivalue("OptionInfo", "watermark", m_strFilePath);
            string stralarm = m_util.getinivalue("OptionInfo", "alarm", m_strFilePath);
            string strShowMissionText = m_util.getinivalue("OptionInfo", "showMissionText", m_strFilePath);
            string strAutoFocusSection = m_util.getinivalue("OptionInfo", "autoFocusSection", m_strFilePath);
            string strVisiblePerformer = m_util.getinivalue("OptionInfo", "visiblePerformer", m_strFilePath);
            //string strShowSectionBtn = m_util.getinivalue("OptionInfo", "showSectionBtn", m_strFilePath);
            string strShowColorIndex = m_util.getinivalue("OptionInfo", "showColorIndex", m_strFilePath);

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

            bool showMissionText = false;
            if (strShowMissionText.ToLower() == "true")
                showMissionText = true;

            bool autoFocusSection = true;
            if (strAutoFocusSection.ToLower() == "false")
                autoFocusSection = false;

            /*bool showSectionBtn = false;
            if (strShowSectionBtn.ToLower() == "true")
                showSectionBtn = true;*/

            bool showColorIndex = false;
            if (strShowColorIndex.ToLower() == "true")
                showColorIndex = true;

            bool visiblePerformer = false;
            if (strVisiblePerformer.ToLower() == "true")
                visiblePerformer = true;

            rdoControl.Checked = isControl;
            rdoMonitoring.Checked = !isControl;
            checkBoxNext.Checked = isComplete;
            checkBoxRemove.Checked = isScenario;
            checkBoxWatermark.Checked = isWatermark;
            checkReceive.Checked = isAlarm;
            checkBoxShowMissionText.Checked = showMissionText;
            checkBoxAutoFocusSection.Checked = autoFocusSection;
            checkBoxVisiblePerformer.Checked = visiblePerformer;

            FormSOP.Instance.ShowMissionText = showMissionText;
            FormSOP.Instance.EnableFocusSection = autoFocusSection;
            FormSOP.Instance.VisiblityToPerformer = visiblePerformer;

            //FormSOP.Instance.ShowSectionBtn = showSectionBtn;
            //this.chkShowButton.Checked = showSectionBtn;
            this.checkBoxLegend.Checked = showColorIndex;

            ArrayList arCategory = ReadDisasterCategory();
            if(arCategory != null)
            { 
                foreach(Data_DisasterCategory category in arCategory)
                {
                    ReadAutoCloseDB(category);
                    //ReadAutoCloseConfig(category.CategoryName);
                }
            }

            //ReadAutoCloseConfig("전체");
            ReadExternalRunOptions();

            bOnReadConfig = false;
        }

        private void ReadExternalRunOptions()
        {
            string strFilePath = GetCommonConfigPath();

            if (System.IO.File.Exists(strFilePath) == false)
                return;

            string strFolderPath = m_util.getinivalue("ExternalRun", "Folder", strFilePath);

            if (strFolderPath != null && strFolderPath.Length > 0)
            {
                m_strExternalFolderPath = strFolderPath;
                textBoxExternalFolderPath.Text = m_strExternalFolderPath;
            }
        }

        private void WriteAutoCloseDB(string szCategoryName, int nCategoryID = -1)
        {
            UnE.SOP.SOPCloseOption option = null;
            try
            {
                option = UnE.SOP.ProxySOP.Instance.OptionSOPAutoCloseSet[szCategoryName];
            }
            catch (Exception)
            { }

            if (option == null)
            {
                option = new SOPCloseOption();
                option.CategroyName = szCategoryName;

                UnE.SOP.ProxySOP.Instance.OptionSOPAutoCloseSet.Add(szCategoryName, option);
            }

            if (szCategoryName == "전체")
            {
                foreach (UnE.SOP.SOPCloseOption option2 in UnE.SOP.ProxySOP.Instance.OptionSOPAutoCloseSet.Values)
                {
                    if (option2.CategroyName == "전체")
                        continue;

                    option2.UseCloseSOPWaitInputTime = option.UseCloseSOPWaitInputTime;
                    option2.CloseSOPWaitInputTime = option.CloseSOPWaitInputTime;
                    option2.UseCloseSOPSensorReset = option.UseCloseSOPSensorReset;
                    option2.UseCloseSOPSensorResetWaitTime = option.UseCloseSOPSensorResetWaitTime;
                    option2.CloseSOPSensorResetWaitTime = option.CloseSOPSensorResetWaitTime;

                    WriteAutoCloseDB(option2.CategroyName);
                }
            }
            else
            {
                if (nCategoryID < 0)
                {
                    string strSQL = "Select ID from DisasterCategory where CategoryName = '" + szCategoryName + "' and SiteID = " + m_nSiteID.ToString();
                    ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL, 0);

                    if (arrResult != null && arrResult.Count > 0)                    
                        nCategoryID = WebDBManager.GetIntField(arrResult[0].ToString(), -1); 
                }

                if (nCategoryID < 0)
                    return;
                    
                SaveAutoCloseDB(option, nCategoryID);

                /*string szSectionName = "AutoClose_" + szCategoryName;
                m_util.setinivalue(szSectionName, "UseCloseSOPWaitInputTime", option.UseCloseSOPWaitInputTime.ToString(), m_strFilePath);
                m_util.setinivalue(szSectionName, "CloseSOPWaitInputTime", option.CloseSOPWaitInputTime.ToString(), m_strFilePath);
                m_util.setinivalue(szSectionName, "UseCloseSOPSensorReset", option.UseCloseSOPSensorReset.ToString(), m_strFilePath);
                m_util.setinivalue(szSectionName, "UseCloseSOPSensorResetWaitTime", option.UseCloseSOPSensorResetWaitTime.ToString(), m_strFilePath);
                m_util.setinivalue(szSectionName, "CloseSOPSensorResetWaitTime", option.CloseSOPSensorResetWaitTime.ToString(), m_strFilePath);*/
            }
        }
        /*private void WriteAutoCloseConfig(string szCategoryName)
        {
            UnE.SOP.SOPCloseOption option = null;
            try
            {
                option = UnE.SOP.ProxySOP.Instance.OptionSOPAutoCloseSet[szCategoryName];
            }
            catch (Exception)
            { }

            if (option == null)
            {
                option = new SOPCloseOption();
                option.CategroyName = szCategoryName;

                UnE.SOP.ProxySOP.Instance.OptionSOPAutoCloseSet.Add(szCategoryName, option);
            }

            if(szCategoryName == "전체")
            {
                foreach( UnE.SOP.SOPCloseOption option2 in UnE.SOP.ProxySOP.Instance.OptionSOPAutoCloseSet.Values)
                {
                    if (option2.CategroyName == "전체")
                        continue;

                    option2.UseCloseSOPWaitInputTime = option.UseCloseSOPWaitInputTime;
                    option2.CloseSOPWaitInputTime = option.CloseSOPWaitInputTime;
                    option2.UseCloseSOPSensorReset = option.UseCloseSOPSensorReset;
                    option2.UseCloseSOPSensorResetWaitTime = option.UseCloseSOPSensorResetWaitTime;
                    option2.CloseSOPSensorResetWaitTime = option.CloseSOPSensorResetWaitTime;

                    WriteAutoCloseConfig(option2.CategroyName);
                }
            }
            else
            {
                string szSectionName = "AutoClose_" + szCategoryName;
                m_util.setinivalue(szSectionName, "UseCloseSOPWaitInputTime", option.UseCloseSOPWaitInputTime.ToString(), m_strFilePath);
                m_util.setinivalue(szSectionName, "CloseSOPWaitInputTime", option.CloseSOPWaitInputTime.ToString(), m_strFilePath);
                m_util.setinivalue(szSectionName, "UseCloseSOPSensorReset", option.UseCloseSOPSensorReset.ToString(), m_strFilePath);
                m_util.setinivalue(szSectionName, "UseCloseSOPSensorResetWaitTime", option.UseCloseSOPSensorResetWaitTime.ToString(), m_strFilePath);
                m_util.setinivalue(szSectionName, "CloseSOPSensorResetWaitTime", option.CloseSOPSensorResetWaitTime.ToString(), m_strFilePath);
            }
        }*/

        private void ReadAutoCloseDB(Data_DisasterCategory category, UnE.SOP.SOPCloseOption option = null)
        {
            if (option == null)
            {
                if (UnE.SOP.ProxySOP.Instance.OptionSOPAutoCloseSet.TryGetValue(category.CategoryName, out option) == false)
                    option = null;
            }

            if (option == null)
            {
                option = new SOPCloseOption();
                option.CategroyName = category.CategoryName;

                UnE.SOP.ProxySOP.Instance.OptionSOPAutoCloseSet.Add(category.CategoryName, option);
            }

            string strPropertyTag = SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.SOP_AUTO_CLOSE);

            string strPropertyName = strPropertyTag + "_" + category.ID.ToString();
            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = '" + strPropertyName + "' and SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL, 0);

            bool readOption = false;

            if (arrResult != null && arrResult.Count > 0)
            {
                string strValue = WebDBManager.GetStringField(arrResult[0]);

                if (strValue != null)
                {
                    string[] tokens = strValue.Split(';');

                    if (tokens.Count() == 5)
                    {
                        int nCloseWaitTime = 0, nCloseSensorWaitTime = 0;
                        int nUseCloseWaitTime = 0, nUseCloseSensorReset = 0, nUseCloseSensorResetWaitTime = 0;

                        if (int.TryParse(tokens[0].Trim(), out nCloseWaitTime) && nCloseWaitTime >= 0)
                        {
                            option.CloseSOPWaitInputTime = nCloseWaitTime;

                            if (int.TryParse(tokens[1].Trim(), out nUseCloseWaitTime))
                            {
                                if (nUseCloseWaitTime == 0 || nUseCloseWaitTime == 1)
                                {
                                    option.UseCloseSOPWaitInputTime = nUseCloseWaitTime == 1;

                                    if (int.TryParse(tokens[2].Trim(), out nUseCloseSensorReset))
                                    {
                                        if (nUseCloseSensorReset == 0 || nUseCloseSensorReset == 1)
                                        {
                                            option.UseCloseSOPSensorReset = nUseCloseSensorReset == 1;

                                            if (int.TryParse(tokens[3].Trim(), out nCloseSensorWaitTime) && nCloseSensorWaitTime >= 0)
                                            {
                                                option.CloseSOPSensorResetWaitTime = nCloseSensorWaitTime;

                                                if (int.TryParse(tokens[4].Trim(), out nUseCloseSensorResetWaitTime))
                                                {
                                                    if (nUseCloseSensorResetWaitTime == 0 || nUseCloseSensorResetWaitTime == 1)
                                                    {
                                                        option.UseCloseSOPSensorResetWaitTime = nUseCloseSensorResetWaitTime == 1;
                                                        readOption = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (readOption == false)
            {
                option.CloseSOPWaitInputTime = 10;
                option.UseCloseSOPWaitInputTime = false;
                option.UseCloseSOPSensorReset = false;
                option.CloseSOPSensorResetWaitTime = 10;
                option.UseCloseSOPSensorResetWaitTime = false;

                SaveAutoCloseDB(option, category.ID);
            }
        }

        public void SetAutoCloseDB(int nCategoryID, string strCategoryName, string strCloseSOPWaitInputTime, string strUseCloseSOPWaitInputTime, string strUseCloseSOPSensorReset, string strCloseSOPSensorResetWaitTime, string strUseCloseSOPSensorResetWaitTime)
        {
            SOPCloseOption option = null;

            if (UnE.SOP.ProxySOP.Instance.OptionSOPAutoCloseSet.TryGetValue(strCategoryName, out option) == false)
                option = null;

            if (option == null)
            {
                option = new SOPCloseOption();
                option.CategroyName = strCategoryName;

                UnE.SOP.ProxySOP.Instance.OptionSOPAutoCloseSet.Add(strCategoryName, option);
            }

            int nCloseSOPWaitInputTime, nCloseSOPSensorResetWaitTime;
            int nUseCloseSOPWaitInputTime, nUseCloseSOPSensorReset, nUseCloseSOPSensorResetWaitTime;

            if (!int.TryParse(strCloseSOPWaitInputTime, out nCloseSOPWaitInputTime) || !int.TryParse(strCloseSOPSensorResetWaitTime, out nCloseSOPSensorResetWaitTime))
                return;

            if (!int.TryParse(strUseCloseSOPWaitInputTime, out nUseCloseSOPWaitInputTime) ||
                !int.TryParse(strUseCloseSOPSensorReset, out nUseCloseSOPSensorReset) ||
                !int.TryParse(strUseCloseSOPSensorResetWaitTime, out nUseCloseSOPSensorResetWaitTime))
                return;

            option.CloseSOPWaitInputTime = nCloseSOPWaitInputTime;
            option.UseCloseSOPWaitInputTime = nUseCloseSOPWaitInputTime == 1;
            option.UseCloseSOPSensorReset = nUseCloseSOPSensorReset == 1;
            option.CloseSOPSensorResetWaitTime = nCloseSOPSensorResetWaitTime;
            option.UseCloseSOPSensorResetWaitTime = nUseCloseSOPSensorResetWaitTime == 1;

            if (cmbDisasterType1.Text == strCategoryName)
            {
                ChangeCloseOptioUI(option);
            }

            if (cmbDisasterType2.Text == strCategoryName)
            {
                ChangeCloseOptioUI2(option);
            }
        }

        private void SaveAutoCloseDB(UnE.SOP.SOPCloseOption option, int nCategoryID)
        {
            string strPropertyValue = string.Format("{0};{1};{2};{3};{4}",
                    option.CloseSOPWaitInputTime,
                    option.UseCloseSOPWaitInputTime ? 1 : 0,
                    option.UseCloseSOPSensorReset ? 1 : 0,
                    option.CloseSOPSensorResetWaitTime,
                    option.UseCloseSOPSensorResetWaitTime ? 1 : 0);

            string strPropertyTag = SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.SOP_AUTO_CLOSE);
            string strPropertyName = strPropertyTag + "_" + nCategoryID.ToString();

            string strDescription = string.Format("SOP 자동종료 옵션(CategoryID : {0})", nCategoryID);
            SaveDBOption(FormSOP.Instance.DBManager, strPropertyName, strPropertyValue, strDescription);

            if (FormSOP.Instance.NetworkManager != null)
                FormSOP.Instance.NetworkManager.SendChangedConfig(TCP_CLIENT.SOP_SIMULATOR, strPropertyTag, nCategoryID.ToString() + ";" + option.CategroyName + ";" + strPropertyValue);
        }

        private void ReadAutoCloseDB(UnE.SOP.SOPCloseOption option)
        {
            string strSQL = "Select ID from DisasterCategory where CategoryName = '" + option.CategroyName + "' and SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            Data_DisasterCategory category = new Data_DisasterCategory();
            category.ID = id.Data;
            category.CategoryName = option.CategroyName;

            ReadAutoCloseDB(category, option);
        }

        /*private void ReadAutoCloseConfig(string szCategoryName)
        {
            UnE.SOP.SOPCloseOption option = null;
            try
            {
                option = UnE.SOP.ProxySOP.Instance.OptionSOPAutoCloseSet[szCategoryName];
            }
            catch (Exception)
            { }

            if (option == null)
            {
                option = new SOPCloseOption();
                option.CategroyName = szCategoryName;

                UnE.SOP.ProxySOP.Instance.OptionSOPAutoCloseSet.Add(szCategoryName, option);
            }

            string szSectionName = "AutoClose_" + szCategoryName;

            string strCloseSOPWaitInputTime = m_util.getinivalue(szSectionName, "CloseSOPWaitInputTime", m_strFilePath);
            if (strCloseSOPWaitInputTime == "")
            {
                strCloseSOPWaitInputTime = "10";
                m_util.setinivalue(szSectionName, "CloseSOPWaitInputTime", strCloseSOPWaitInputTime, m_strFilePath);
            }
            try
            {
                option.CloseSOPWaitInputTime = Convert.ToInt32(strCloseSOPWaitInputTime);
            }
            catch (Exception)
            { }


            string strUseCloseSOPWaitInputTime = m_util.getinivalue(szSectionName, "UseCloseSOPWaitInputTime", m_strFilePath);
            if (strUseCloseSOPWaitInputTime == "")
            {
                strUseCloseSOPWaitInputTime = "False";
                m_util.setinivalue(szSectionName, "UseCloseSOPWaitInputTime", strUseCloseSOPWaitInputTime, m_strFilePath);
            }
            try
            {
                option.UseCloseSOPWaitInputTime = Convert.ToBoolean(strUseCloseSOPWaitInputTime);
            }
            catch (Exception)
            { }



            string strUseCloseSOPSensorReset = m_util.getinivalue(szSectionName, "UseCloseSOPSensorReset", m_strFilePath);
            if (strUseCloseSOPSensorReset == "")
            {
                strUseCloseSOPSensorReset = "False";
                m_util.setinivalue(szSectionName, "UseCloseSOPSensorReset", strUseCloseSOPSensorReset, m_strFilePath);
            }
            try
            {
                option.UseCloseSOPSensorReset = Convert.ToBoolean(strUseCloseSOPSensorReset);
            }
            catch (Exception)
            { }


            string strCloseSOPSensorResetWaitTime = m_util.getinivalue(szSectionName, "CloseSOPSensorResetWaitTime", m_strFilePath);
            if (strCloseSOPSensorResetWaitTime == "")
            {
                strCloseSOPSensorResetWaitTime = "10";
                m_util.setinivalue(szSectionName, "CloseSOPSensorResetWaitTime", strCloseSOPSensorResetWaitTime, m_strFilePath);
            }
            try
            {
                option.CloseSOPSensorResetWaitTime = Convert.ToInt32(strCloseSOPSensorResetWaitTime);
            }
            catch (Exception)
            { }


            string strUseCloseSOPSensorResetWaitTime = m_util.getinivalue(szSectionName, "UseCloseSOPSensorResetWaitTime", m_strFilePath);
            if (strUseCloseSOPSensorResetWaitTime == "")
            {
                strUseCloseSOPSensorResetWaitTime = "False";
                m_util.setinivalue(szSectionName, "UseCloseSOPSensorResetWaitTime", strUseCloseSOPSensorResetWaitTime, m_strFilePath);
            }
            try
            {
                option.UseCloseSOPSensorResetWaitTime = Convert.ToBoolean(strUseCloseSOPSensorResetWaitTime);
            }
            catch (Exception)
            { }
            
        }*/

        /*private void ReadAutoCloseConfig(UnE.SOP.SOPCloseOption option)
        {
            string szCategoryName = option.CategroyName;
            string szSectionName = "AutoClose_" + szCategoryName;

            string strCloseSOPWaitInputTime = m_util.getinivalue(szSectionName, "CloseSOPWaitInputTime", m_strFilePath);
            if (strCloseSOPWaitInputTime == "")
            {
                strCloseSOPWaitInputTime = "10";
                m_util.setinivalue(szSectionName, "CloseSOPWaitInputTime", strCloseSOPWaitInputTime, m_strFilePath);
            }
            try
            {
                option.CloseSOPWaitInputTime = Convert.ToInt32(strCloseSOPWaitInputTime);
            }
            catch (Exception)
            { }


            string strUseCloseSOPWaitInputTime = m_util.getinivalue(szSectionName, "UseCloseSOPWaitInputTime", m_strFilePath);
            if (strUseCloseSOPWaitInputTime == "")
            {
                strUseCloseSOPWaitInputTime = "False";
                m_util.setinivalue(szSectionName, "UseCloseSOPWaitInputTime", strUseCloseSOPWaitInputTime, m_strFilePath);
            }
            try
            {
                option.UseCloseSOPWaitInputTime = Convert.ToBoolean(strUseCloseSOPWaitInputTime);
            }
            catch (Exception)
            { }



            string strUseCloseSOPSensorReset = m_util.getinivalue(szSectionName, "UseCloseSOPSensorReset", m_strFilePath);
            if (strUseCloseSOPSensorReset == "")
            {
                strUseCloseSOPSensorReset = "False";
                m_util.setinivalue(szSectionName, "UseCloseSOPSensorReset", strUseCloseSOPSensorReset, m_strFilePath);
            }
            try
            {
                option.UseCloseSOPSensorReset = Convert.ToBoolean(strUseCloseSOPSensorReset);
            }
            catch (Exception)
            { }


            string strCloseSOPSensorResetWaitTime = m_util.getinivalue(szSectionName, "CloseSOPSensorResetWaitTime", m_strFilePath);
            if (strCloseSOPSensorResetWaitTime == "")
            {
                strCloseSOPSensorResetWaitTime = "10";
                m_util.setinivalue(szSectionName, "CloseSOPSensorResetWaitTime", strCloseSOPSensorResetWaitTime, m_strFilePath);
            }
            try
            {
                option.CloseSOPSensorResetWaitTime = Convert.ToInt32(strCloseSOPSensorResetWaitTime);
            }
            catch (Exception)
            { }


            string strUseCloseSOPSensorResetWaitTime = m_util.getinivalue(szSectionName, "UseCloseSOPSensorResetWaitTime", m_strFilePath);
            if (strUseCloseSOPSensorResetWaitTime == "")
            {
                strUseCloseSOPSensorResetWaitTime = "False";
                m_util.setinivalue(szSectionName, "UseCloseSOPSensorResetWaitTime", strUseCloseSOPSensorResetWaitTime, m_strFilePath);
            }
            try
            {
                option.UseCloseSOPSensorResetWaitTime = Convert.ToBoolean(strUseCloseSOPSensorResetWaitTime);
            }
            catch (Exception)
            { }

        }*/

        private void SaveAutoCloseOption(UnE.SOP.SOPCloseOption option2)
        {
            string szCategoryName = option2.CategroyName;
            UnE.SOP.SOPCloseOption option = null;
            try
            {
                option = UnE.SOP.ProxySOP.Instance.OptionSOPAutoCloseSet[szCategoryName];
            }
            catch (Exception)
            { }

            if (option == null)
            {
                option = new SOPCloseOption();
                option.CategroyName = szCategoryName;

                UnE.SOP.ProxySOP.Instance.OptionSOPAutoCloseSet.Add(szCategoryName, option);
            }

            option.UseCloseSOPWaitInputTime = option2.UseCloseSOPWaitInputTime;
            option.CloseSOPWaitInputTime = option2.CloseSOPWaitInputTime;
            option.UseCloseSOPSensorReset = option2.UseCloseSOPSensorReset;
            option.UseCloseSOPSensorResetWaitTime = option2.UseCloseSOPSensorResetWaitTime;
            option.CloseSOPSensorResetWaitTime = option2.CloseSOPSensorResetWaitTime;
           
        }


        private bool SetWorkingHours(string strBeginHour, string strBeginMinute, string strEndHour, string strEndMinute)
        {
            WebDBManager dbMgr = FormSOP.Instance.DBManager;

            if (!SaveDBOption(dbMgr, SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.WORKING_BEGIN_HOUR), strBeginHour + ":" + strBeginMinute, "근무시작시간(시간:분)"))
                return false;

            if (!SaveDBOption(dbMgr, SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.WORKING_END_HOUR), strEndHour + ":" + strEndMinute, "근무종료시간(시간:분)"))
                return false;

            return true;
        }

        private int GetMaxID(string strTableName, WebDBManager dbMgr)
        {
            string strSQL = "select max(ID) from " + strTableName;
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            return WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }

        private bool SaveDBOption(WebDBManager dbMgr, string strPropertyName, string strPropertyValue, string strDescription = null)
        {
           
            string strSQL = "Select ID from OptionSOPSimulator where PropertyName = '" + strPropertyName + "' and SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            if (arrResult.Count == 0)
                return InsertDBOption(dbMgr,strPropertyName, strPropertyValue, strDescription);

            int nID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);

            if (nID < 0)
                return InsertDBOption(dbMgr,strPropertyName, strPropertyValue, strDescription);

            return UpdateDBOption(dbMgr, nID, strPropertyValue);
        }

        private bool UpdateDBOption(WebDBManager dbMgr, int nID, string strPropertyValue)
        {
            string strSQL = string.Format("Update OptionSOPSimulator set PropertyValue = '{0}' where ID = {1} and SiteID ={2}", strPropertyValue, nID, m_nSiteID);
            return dbMgr.GetResultData(strSQL, 0) != null;
        }

        private bool InsertDBOption(WebDBManager dbMgr, string strPropertyName, string strPropertyValue, string strDescription)
        {
            int nID = GetMaxID("OptionSOPSimulator", dbMgr) + 1;
            string strDesc = strDescription == null ? "NULL" : "'" + strDescription + "'";

            string strSQL = "Insert into OptionSOPSimulator (ID, PropertyName, PropertyValue, Description, SiteID) values ";
            strSQL += string.Format("({0}, '{1}', '{2}', {3}, {4})",nID, strPropertyName, strPropertyValue, strDesc, m_nSiteID);

            return dbMgr.GetResultData(strSQL, 0) != null;
        }

        private bool GetWorkingHours(ref int nBeginHour, ref int nBeginMinute, ref int nEndHour, ref int nEndMinute)
        {
            WebDBManager dbMgr = FormSOP.Instance.DBManager;

            if (!GetWorkingHours(dbMgr, SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.WORKING_BEGIN_HOUR), ref nBeginHour, ref nBeginMinute))
                return false;

            if (!GetWorkingHours(dbMgr, SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.WORKING_END_HOUR), ref nEndHour, ref nEndMinute))
                return false;

            return true;
        }

        private bool GetWorkingHours(WebDBManager dbMgr, string strPropertyName, ref int nHour, ref int nMinute)
        {
            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = '" + strPropertyName + "' and SiteID = " + m_nSiteID.ToString();
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

        private void WriteConfigFile(bool exceptAutoCloseDB = false)
        {
            if (bOnReadConfig == true)
                return;

            string strCurrent = ((uint)WorkFlowManager.Instance.CurrentColor.ToArgb()).ToString();
            //string strwait = "4294957567";
            string strprogress = "4294902015";
            string strcomplete = "4290822336";
            string strskip = "4294961535";
            string strodd = Color.FromArgb(234, 236, 236).ToArgb().ToString(); //"4294967295";
            string streven = Color.FromArgb(255, 255, 255).ToArgb().ToString(); //"4290822336";

            if (btnCurrent != null)
            {
                UInt32 current = (UInt32)btnCurrent.BackColor.ToArgb();
                strCurrent = current.ToString();
            }
            /*if (btnWait != null)
            {
                UInt32 wait = (UInt32)btnWait.BackColor.ToArgb();
                strwait = wait.ToString();
            }*/
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

            m_util.setinivalue("ColorInfo", "current_color", strCurrent, m_strFilePath);
            //m_util.setinivalue("ColorInfo", "wait_color", strwait, m_strFilePath);
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
            string strVisiblePerformer = checkBoxVisiblePerformer.Checked.ToString();

            //string strShowSectionBtn = chkShowButton.Checked.ToString();
            string strShowColorIndex = checkBoxLegend.Checked.ToString();

            m_util.setinivalue("OptionInfo", "control", strcontrol, m_strFilePath);
            m_util.setinivalue("OptionInfo", "complete", strruncomplete, m_strFilePath);
            m_util.setinivalue("OptionInfo", "scenario", strscenario, m_strFilePath);
            m_util.setinivalue("OptionInfo", "watermark", strwatermark, m_strFilePath);
            m_util.setinivalue("OptionInfo", "alarm", stralarm, m_strFilePath);
            m_util.setinivalue("OptionInfo", "showMissionText", strShowMissionText, m_strFilePath);
            m_util.setinivalue("OptionInfo", "autoFocusSection", strAutoFocusSection, m_strFilePath);
            m_util.setinivalue("OptionInfo", "visiblePerformer", strVisiblePerformer, m_strFilePath);
            //m_util.setinivalue("OptionInfo", "showSectionBtn", strShowSectionBtn, m_strFilePath);
            m_util.setinivalue("OptionInfo", "showColorIndex", strShowColorIndex, m_strFilePath);

            SetWorkingHours(cboBeginHour.Text, cboBeginMinute.Text, cboEndHour.Text, cboEndMinute.Text);

            if (!exceptAutoCloseDB)
            {
                ArrayList arCategory = ReadDisasterCategory();
                if (arCategory != null)
                {
                    foreach (Data_DisasterCategory category in arCategory)
                    {
                        WriteAutoCloseDB(category.CategoryName, category.ID);
                        //WriteAutoCloseConfig(category.CategoryName);
                    }
                }
            }
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

            dataGridVersion.Rows[0].Cells[1].Value = version.LastAccessedTime;
            dataGridVersion.Rows[1].Cells[1].Value = version.BeginTime;
            dataGridVersion.Rows[2].Cells[1].Value = version.VersionName;
            dataGridVersion.Rows[3].Cells[1].Value = version.Description;
            dataGridVersion.Rows[4].Cells[1].Value = version.UserName;
        }

        private void btnInitialize_Click(object sender, EventArgs e)
        {
            Color colCurrent = WorkFlowManager.Instance.CurrentColor;
            //Color colwait = Color.FromArgb((int)UInt32.Parse("4294957567"));
            Color colprogress = Color.FromArgb(172, 157, 247);// Color.FromArgb((int)UInt32.Parse("4294902015"));
            Color colcomplete = Color.FromArgb(154, 213, 247);// Color.FromArgb((int)UInt32.Parse("4290822336"));
            Color colskip = Color.FromArgb((int)UInt32.Parse("4294961535"));
            Color colodd = Color.FromArgb(234, 236, 236);// Color.FromArgb((int)UInt32.Parse("4292993535"));
            Color coleven = Color.FromArgb(255, 255, 255);// Color.FromArgb((int)UInt32.Parse("4292673535"));

            btnCurrent.BackColor = colCurrent;
            //btnWait.BackColor = colwait;
            btnProgress.BackColor = colprogress;
            btnComplete.BackColor = colcomplete;
            btnSkip.BackColor = colskip;
            btnOdd.BackColor = colodd;
            btnEven.BackColor = coleven;

            Initialize();

            WriteConfigFile();

            WorkFlowManager.Instance.CurrentColor = btnCurrent.BackColor;
            //WorkFlowManager.Instance.InputWaitColor = btnWait.BackColor;
            WorkFlowManager.Instance.InProgressColor = btnProgress.BackColor;
            WorkFlowManager.Instance.CompleteColor = btnComplete.BackColor;
            WorkFlowManager.Instance.SkipColor = btnSkip.BackColor;
            WorkFlowManager.Instance.ChangeColor();
            if (FormSOP.Instance.GetPageHome().tabControl.SelectedTab != null)
                FormSOP.Instance.GetPageHome().tabControl.SelectedTab.Refresh();

            FormSOP.Instance.GetPageHome().changecolor(0, btnCurrent.BackColor);
            FormSOP.Instance.GetPageHome().changecolor(1, btnProgress.BackColor);
            FormSOP.Instance.GetPageHome().changecolor(2, btnComplete.BackColor);
            FormSOP.Instance.GetPageHome().changecolor(3, btnSkip.BackColor);
        }

        private void ColorButton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            
            colorDialog.AllowFullOpen = true;
            colorDialog.FullOpen = true;

            int[] nUserColors = new int[]
            {
                ColorTranslator.ToOle(btnCurrent.BackColor),
                ColorTranslator.ToOle(btnProgress.BackColor),
                ColorTranslator.ToOle(btnComplete.BackColor),
                ColorTranslator.ToOle(btnSkip.BackColor),
                ColorTranslator.ToOle(btnOdd.BackColor),
                ColorTranslator.ToOle(btnEven.BackColor)
            };

            colorDialog.CustomColors = nUserColors;

            if (btn == btnCurrent)
            {                
                colorDialog.Color = btnCurrent.BackColor;     
            }
            else if (btn == btnProgress)
            {               
                colorDialog.Color = btnProgress.BackColor;
            }
            else if (btn == btnComplete)
            {               
                colorDialog.Color = btnComplete.BackColor;
            }
            else if (btn == btnSkip)
            {               
                colorDialog.Color = btnSkip.BackColor;
            }
            else if (btn == btnOdd)
            {                
                colorDialog.Color = btnOdd.BackColor;              
            }
            else if (btn == btnEven)
            {               
                colorDialog.Color = btnEven.BackColor;               
            }

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                if (btn == btnCurrent)
                {
                    btnCurrent.BackColor = colorDialog.Color;
                    WorkFlowManager.Instance.CurrentColor = btnCurrent.BackColor;
                    //WorkFlowManager.Instance.InputWaitColor = btnWait.BackColor;
                    WorkFlowManager.Instance.ChangeColor();
                    if (FormSOP.Instance.GetPageHome().tabControl.SelectedTab != null)
                        FormSOP.Instance.GetPageHome().tabControl.SelectedTab.Refresh();

                    FormSOP.Instance.GetPageHome().changecolor(0, colorDialog.Color); //0 : 대기 업무 / 1: 실행중 업무 / 2: 완료된 업무 / 3: 건너뛴 업무
                }
                else if (btn == btnProgress)
                {
                    btnProgress.BackColor = colorDialog.Color;
                    WorkFlowManager.Instance.InProgressColor = btnProgress.BackColor;
                    WorkFlowManager.Instance.ChangeColor();
                    if (FormSOP.Instance.GetPageHome().tabControl.SelectedTab != null)
                        FormSOP.Instance.GetPageHome().tabControl.SelectedTab.Refresh();

                    FormSOP.Instance.GetPageHome().changecolor(1, colorDialog.Color); //0 : 대기 업무 / 1: 실행중 업무 / 2: 완료된 업무 / 3: 건너뛴 업무
                }
                else if (btn == btnComplete)
                {
                    btnComplete.BackColor = colorDialog.Color;
                    WorkFlowManager.Instance.CompleteColor = btnComplete.BackColor;
                    WorkFlowManager.Instance.ChangeColor();
                    if (FormSOP.Instance.GetPageHome().tabControl.SelectedTab != null)
                        FormSOP.Instance.GetPageHome().tabControl.SelectedTab.Refresh();

                    FormSOP.Instance.GetPageHome().changecolor(2, colorDialog.Color); //0 : 대기 업무 / 1: 실행중 업무 / 2: 완료된 업무 / 3: 건너뛴 업무
                }
                else if (btn == btnSkip)
                {
                    btnSkip.BackColor = colorDialog.Color;
                    WorkFlowManager.Instance.SkipColor = btnSkip.BackColor;
                    WorkFlowManager.Instance.ChangeColor();
                    if (FormSOP.Instance.GetPageHome().tabControl.SelectedTab != null)
                        FormSOP.Instance.GetPageHome().tabControl.SelectedTab.Refresh();

                    FormSOP.Instance.GetPageHome().changecolor(3, colorDialog.Color); //0 : 대기 업무 / 1: 실행중 업무 / 2: 완료된 업무 / 3: 건너뛴 업무
                }
                else if (btn == btnOdd)
                {
                    btnOdd.BackColor = colorDialog.Color;
                    FormSOP.Instance.GetPageHome().ColorPanel1 = colorDialog.Color;
                    FormSOP.Instance.GetPageHome().ColorChangedPanel();
                }
                else if (btn == btnEven)
                {
                    btnEven.BackColor = colorDialog.Color;
                    FormSOP.Instance.GetPageHome().ColorPanel2 = colorDialog.Color;
                    FormSOP.Instance.GetPageHome().ColorChangedPanel();
                }
            }
            
            WriteConfigFile();
        }
        
        public void ColorChange()
        {
            FormSOP.Instance.GetPageHome().ColorPanel1 = btnOdd.BackColor;
            FormSOP.Instance.GetPageHome().ColorPanel2 = btnEven.BackColor;
            FormSOP.Instance.GetPageHome().ColorChangedPanel();

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

        private void chkShowButton_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_CheckedChanged(sender, e);
        }


        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox btn = (CheckBox)sender;

            if (btn == checkBoxWatermark)
            {
                bool bChecked = checkBoxWatermark.Checked;
                TabPageManager.Instance.ChangeWaterMark(bChecked);
                FormSOP.Instance.GetPageHome().ChangeWaterMark(bChecked);
                ProxySOP.Instance.UseWaterMark = bChecked;
            }
            else if(btn == checkBoxNext)
            {
                WorkFlowManager.Instance.WaitComplete = !WorkFlowManager.Instance.WaitComplete;
                if (WaitOptionChange != null)
                {
                    WaitOptionChange(this, new WaitOptionChangeEventArgs());
                }
            }
            else if (btn == checkBoxRemove)
            {
                WorkFlowManager.Instance.DeleteComplete = !WorkFlowManager.Instance.DeleteComplete;
                if (DeleteOptionChange != null)
                {
                    DeleteOptionChange(this, new DeleteOptionChangeEventArgs());
                }
            }
            else if (btn == checkReceive)
            {
                if (checkReceive.Checked == true)
                    FormSOP.Instance.DoorBellCheck(true);
                else
                    FormSOP.Instance.DoorBellCheck(false);
            }
            else if (btn == checkBoxShowMissionText)
            {
                FormSOP.Instance.ShowMissionText = btn.Checked;

                if (!btn.Checked && PopupMissionText.Instance.Visible)
                    PopupMissionText.Instance.Visible = false;
            }
            else if (btn == checkBoxAutoFocusSection)
            {
                FormSOP.Instance.EnableFocusSection = btn.Checked;
            }
            else if (btn == cbShowMissionStatus)
            {
                FormSOP.Instance.VisibleMissionStatus = btn.Checked;
            }
            else if (btn == checkBoxVisiblePerformer)
            {
                FormSOP.Instance.VisiblityToPerformer = checkBoxVisiblePerformer.Checked;
            }
            /*else if( btn == chkShowButton)
            {
                FormSOP.Instance.ShowSectionBtn = btn.Checked;
            }*/

            WriteConfigFile(true);
        }

        private void rdoControl_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton btn = (RadioButton)sender;
            if (rdoControl == btn)
                WriteConfigFile();

        }

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
            FormSOP.Instance.UseBroadcast = bUse;

            if (!m_ignoreChanged)
            {
                this.BeginInvoke(new Action(() =>
                {
                    string strUse = bUse ? "1" : "0";
                    if (FormSOP.Instance.NetworkManager != null)
                        FormSOP.Instance.NetworkManager.SendChangedConfig(TCP_CLIENT.SOP_SIMULATOR, SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.USE_BROADCAST), strUse);
                    SaveDBOption(FormSOP.Instance.DBManager, SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.USE_BROADCAST), strUse, "방송 사용여부");

                    UnE.SOP.TTS.TTSManager.Instance.UseBroadcast = bUse;
                }));   
            }
        }

        private void cbSMS_CheckedChanged(object sender, EventArgs e)
        {
            bool bUse = cbSMS.Checked;
            FormSOP.Instance.SMSOn = bUse;

            if (!m_ignoreChanged)
            {
                
                
                this.BeginInvoke(new Action(() =>
                {
                    string strUse = bUse ? "1" : "0";
               
                    SaveDBOption(FormSOP.Instance.DBManager, SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.USE_SMS), strUse, "문자 사용여부");
                    if (FormSOP.Instance.NetworkManager != null)
                        FormSOP.Instance.NetworkManager.SendChangedConfig(TCP_CLIENT.SOP_SIMULATOR, SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.USE_SMS), strUse);
                    SOPMonitoringSystem.Process.SMSManagerEx.Instance.UseSMS = bUse;   
                }));      
                
                             
            }
        }

        private void cbExternalMemberSMS_CheckedChanged(object sender, EventArgs e)
        {
            bool bUse = cbExternalMemberSMS.Checked;
            FormSOP.Instance.SmsExternalCompanyMemberOn = bUse;

            if (!m_ignoreChanged)
            {
                this.BeginInvoke(new Action(() =>
                {
                    string strUse = bUse ? "1" : "0";
                    if (FormSOP.Instance.NetworkManager != null)
                        FormSOP.Instance.NetworkManager.SendChangedConfig(TCP_CLIENT.SOP_SIMULATOR, SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.SMS_TO_EXTERNAL_MEMBER), strUse);
                    SaveDBOption(FormSOP.Instance.DBManager, SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.SMS_TO_EXTERNAL_MEMBER), strUse, "외부회사직원에게 문자 전송");

                }));
            }
        }

		private void checkBoxLegend_CheckedChanged(object sender, EventArgs e)
		{
			bool bShow = checkBoxLegend.Checked;
			FormSOP.Instance.ShowLegend = bShow;
            WriteConfigFile();
		}

        public bool GetVisbleMissionText()
        {
            return checkBoxShowMissionText.Checked;
        }

        private void cboWorkingHours_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_ignoreChanged)
            {
                if (FormSOP.Instance.NetworkManager != null)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        if (sender == cboBeginHour || sender == cboBeginMinute)
                        {
                            FormSOP.Instance.NetworkManager.SendChangedConfig(TCP_CLIENT.SOP_SIMULATOR, SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.WORKING_BEGIN_HOUR), cboBeginHour.Text + ":" + cboBeginMinute.Text);
                        }
                        else
                        {
                            FormSOP.Instance.NetworkManager.SendChangedConfig(TCP_CLIENT.SOP_SIMULATOR, SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.WORKING_END_HOUR), cboEndHour.Text + ":" + cboEndMinute.Text);
                        }
                    }));
                }

                SetWorkingHours(cboBeginHour.Text, cboBeginMinute.Text, cboEndHour.Text, cboEndMinute.Text);                
            }
        }

        private void labelAutoFocusSection_Click(object sender, EventArgs e)
        {
            checkBoxAutoFocusSection.Checked = !checkBoxAutoFocusSection.Checked;
            checkBox_CheckedChanged(checkBoxAutoFocusSection, null);
        }


		private Pen pen = new Pen(Color.WhiteSmoke);
		private void PageBackstageOption_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;			
			pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
			g.DrawLine(pen, new Point(20, 53), new Point(621, 53));
			g.DrawLine(pen, new Point(20, 270), new Point(621, 270));
			g.DrawLine(pen, new Point(20, 570), new Point(621, 570));
				
			pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
			g.DrawLine(pen, new Point(660, 30), new Point(660, 620));			
		}

        public void ShowTranslucentForm(Form targetForm, int x, int y, int width, int height, int nCommandID)
        {
            if (targetForm == null)
            {
                return;
            }

            //FormSOP.Instance.SetDisableToolBar();

            if (mTranslucentForm == null || mTranslucentForm.IsDisposed)
                mTranslucentForm = new PopupTranslucentForm();

            targetForm.ShowInTaskbar = false;
            if (mTranslucentForm.Visible == true)
            {
                mTranslucentForm.Detach();
            }

            targetForm.StartPosition = FormStartPosition.Manual;
            mTranslucentForm.AddContentForm(targetForm, x, y, targetForm.Size.Width, targetForm.Size.Height, this);
            mTranslucentForm.Parent = this;
            mTranslucentForm.ShowInTaskbar = false;
            mTranslucentForm.Show(this);
        }

        public void CloseTranslucentForm()
        {
            if (mTranslucentForm == null || mTranslucentForm.IsDisposed)
                return;

            mTranslucentForm.CloseExternal();
        }

        private void btnQuickMenu_Click(object sender, EventArgs e)
        {
            FormSOP.Instance.PopupQuickButtonSetup();
        }

        private void btnFireSensorSOPLink_Click(object sender, EventArgs e)
        {
            FormSOP.Instance.PopupSelectFireSensorSOPLink();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void PageBackstageOption_FormClosing(object sender, FormClosingEventArgs e)
        {
            //CloseTranslucentForm();
            Hide();
            e.Cancel = true;
        }

        private void m_CheckTimer_Tick(object sender, EventArgs e)
        {
            if( this.Visible == true)
            {

                UnE.SOP.TTS.TTSManager.Instance.SetState();
                UnE.SOP.TTS.SpeechState state = UnE.SOP.TTS.TTSManager.Instance.State;

                switch (state)
                {
                    case UnE.SOP.TTS.SpeechState.ERROR:
                        lbBState.Text = "연결 대기";
                        lbBState.ForeColor = System.Drawing.Color.Red;
                        break;
                    case UnE.SOP.TTS.SpeechState.STANDBY:
                    case UnE.SOP.TTS.SpeechState.STOP:
                        lbBState.Text = "정상 (대기)";
                        lbBState.ForeColor = System.Drawing.Color.Green;
                        break;

                    case UnE.SOP.TTS.SpeechState.PLAY:
                    case UnE.SOP.TTS.SpeechState.PAUSE:
                    case UnE.SOP.TTS.SpeechState.REPEAT:
                        lbBState.Text = "정상 (방송중)";
                        lbBState.ForeColor = System.Drawing.Color.Blue;
                        break;
                    default:
                        lbBState.Text = "접속 확인중";
                        lbBState.ForeColor = System.Drawing.Color.Black;
                        break;

                }
            }            
        }

        private void PageBackstageOption_VisibleChanged(object sender, EventArgs e)
        {
            

            if( this.Visible == true)
            {

                MakeDisasterCatgory();

                try
                {
                    cmbDisasterType1.SelectedIndex = 0;
                    cmbDisasterType2.SelectedIndex = 0;
                }
                catch(Exception)
                { }
                
                cmbDisasterType1_SelectionChangeCommitted(null, null);
                cmbDisasterType2_SelectionChangeCommitted(null, null);
          

                m_CheckTimer_Tick(null, null);

                m_CheckTimer.Interval = 2000;
                m_CheckTimer.Enabled = true;
                m_CheckTimer.Start();
            }
            else
            {
                m_CheckTimer.Enabled = false;
                m_CheckTimer.Stop();
            }
        }

        private ArrayList ReadDisasterCategory()
        {
            ArrayList arResult = new ArrayList();
            WebDBManager dbMgr = FormSOP.Instance.DBManager;
            string strSql = "SELECT ID, CategoryName FROM DisasterCategory WHERE SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSql, 0);
            if (arrResult != null)
            {
                for (int i = 0; i < arrResult.Count - 1; i += 2)
                {
                    Data_DisasterCategory dataNew = new Data_DisasterCategory();
                    dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                    dataNew.CategoryName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");

                    arResult.Add(dataNew);
                }
            }
            return arResult;
        }

        private void MakeDisasterCatgory()
        {
            cmbDisasterType1.Items.Clear();
            cmbDisasterType2.Items.Clear();

           // Data_DisasterCategory allCategory = new Data_DisasterCategory();
            //allCategory.ID = 1000;
            //allCategory.CategoryName = "전체";

           // cmbDisasterType1.Items.Add(allCategory);
           // cmbDisasterType2.Items.Add(allCategory);

            ArrayList arList = ReadDisasterCategory();
            cmbDisasterType1.Items.AddRange(arList.ToArray());
            cmbDisasterType2.Items.AddRange(arList.ToArray());
            
           // cmbDisasterType1.SelectedItem = allCategory;
           // cmbDisasterType2.SelectedItem = allCategory;

           // SOPCloseOption otpion = new SOPCloseOption();
           // otpion.CategroyName = "전체";
           // btnSaveSOPWaitTimeCloseOption.Tag = otpion;
           // btnSaveSOPSensorCloseOption.Tag = otpion;


        }

        private void radioSOP_CheckedChanged(object sender, EventArgs e)
        {
            if (m_systemInput)
                return;

            int nMode = 0;
            bool openSOPOnSensorDetect = ProxySOP.Instance.OpenSOPOnFireDetect;

            if (radioRunSOP.Checked)
            {
                nMode = 0;
            }
            else if (radioLoadSOP.Checked)
            {
                nMode = 1;
            }
            else// if (radioNoSOP.Checked)
            {
                nMode = 2;
            }

            SetSOPOnSensorDetect(nMode);

            string strPropertyName = SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.RUN_SOP_ON_LOADED);

            if (SaveDBOption(FormSOP.Instance.DBManager, strPropertyName, nMode.ToString()))
            {
                FormSOP.Instance.NetworkManager.SendChangedConfig(TCP_CLIENT.SOP_SIMULATOR, strPropertyName, nMode.ToString());

                if (openSOPOnSensorDetect != ProxySOP.Instance.OpenSOPOnFireDetect)
                    ProxyMessenger.Instance.OpenSopOnSensorDetect(ProxySOP.Instance.OpenSOPOnFireDetect);
            }
        }

        public void SetSOPOnSensorDetect(string strMode)
        {
            int nMode;

            if (int.TryParse(strMode, out nMode))
            {
                SetSOPOnSensorDetect(nMode);
            }
        }

        private void SetSOPOnSensorDetect(int nMode)
        {
            if (nMode == 0)
            {
                // SOP 자동실행
                ProxySOP.Instance.OpenSOPOnFireDetect = true;
                FormSOP.Instance.SensorDetectLoadAndPlay = true;
            }
            else if (nMode == 1)
            {
                // SOP 열기만 한다.
                ProxySOP.Instance.OpenSOPOnFireDetect = true;
                FormSOP.Instance.SensorDetectLoadAndPlay = false;
            }
            else if (nMode == 2)
            {
                // 센서신호 신경 안씀
                ProxySOP.Instance.OpenSOPOnFireDetect = false;
                FormSOP.Instance.SensorDetectLoadAndPlay = false;
            }
        }

        /*private void checkBoxOpenSOPOnDetectSensor_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBoxOpenSOPOnDetectSensor.Checked == true)
            {
                ProxySOP.Instance.OpenSOPOnFireDetect = true;
            }
            else
            {
                ProxySOP.Instance.OpenSOPOnFireDetect = false;
            }
        }*/

        private void ckbConfirmSendSMS_CheckedChanged(object sender, EventArgs e)
        {
            if(ckbConfirmSendSMS.Checked == true)
            {
                UnE.SOP.ProxySOP.Instance.ConfirmSendSMS = true;
                UnE.SOP.ProxySOP.Instance.ConfirmSMSAll = false;
            }
            else
            {
                UnE.SOP.ProxySOP.Instance.ConfirmSendSMS = false;
                UnE.SOP.ProxySOP.Instance.ConfirmSMSAll = false;
            }
        }

        /*private void checkBoxVisiblePerformer_CheckedChanged(object sender, EventArgs e)
        {
            FormSOP.Instance.VisiblityToPerformer = checkBoxVisiblePerformer.Checked;
        }*/

        private void btnPSMSensorSOPLink_Click(object sender, EventArgs e)
        {
            FormSOP.Instance.PopupSelectPSMSensorSOPLink();
        }

        private void btnIntrusionSensorSOPLink_Click(object sender, EventArgs e)
        {
            FormSOP.Instance.PopupSelectIntrusionSensorSOPLink();
        }

        private void rdSOPCloseWaitTime_CheckedChanged(object sender, EventArgs e)
        {
            UnE.SOP.SOPCloseOption option = (UnE.SOP.SOPCloseOption)btnSaveSOPWaitTimeCloseOption.Tag;
            if (option != null)
            {
                if (rdSOPCloseWaitTime.Checked == true)
                {
                    // 입력이 없을 경우 SOP 자동 종료
                    option.UseCloseSOPWaitInputTime = true;

                    string szTime = textSopInputWaitTime.Text;
                    int nTime = option.CloseSOPWaitInputTime;
                    if (int.TryParse(szTime, out nTime))
                    {
                        // 입력 대기 시간 
                        option.CloseSOPWaitInputTime = nTime;
                    }
                    else
                    {
                        textSopInputWaitTime.Text = nTime.ToString();
                    }
                }
            }            
        }

        private void rdNoCloseSOPWaitTime_CheckedChanged(object sender, EventArgs e)
        {
            UnE.SOP.SOPCloseOption option = (UnE.SOP.SOPCloseOption)btnSaveSOPWaitTimeCloseOption.Tag;
            if (option != null)
            {

                if (rdNoCloseSOPWaitTime.Checked == true)
                {
                    // 입력이 없을 경우 SOP 자동 종료 사용안함
                    option.UseCloseSOPWaitInputTime = false;
                }
            }
        }

        private void rdCloseSOPSensorClose_CheckedChanged(object sender, EventArgs e)
        {
            UnE.SOP.SOPCloseOption option = (UnE.SOP.SOPCloseOption)btnSaveSOPSensorCloseOption.Tag;
            if (option != null)
            {
                if (rdCloseSOPSensorClose.Checked == true)
                {
                    // 센서 종료시 SOP 자동 종료
                    option.UseCloseSOPSensorReset = true;
                    option.UseCloseSOPSensorResetWaitTime = false;                    
                }
            }
        }

        private void rdCloseSOPSensorCloseTimeWait_CheckedChanged(object sender, EventArgs e)
        {
            UnE.SOP.SOPCloseOption option = (UnE.SOP.SOPCloseOption)btnSaveSOPSensorCloseOption.Tag;
            if (option != null)
            {
                if (rdCloseSOPSensorCloseTimeWait.Checked == true)
                {

                    option.UseCloseSOPSensorReset = false;
                    option.UseCloseSOPSensorResetWaitTime = true;

                    // 센서 종료시 몇 분후  SOP 자동 종료
                    string szTime = this.txtSopSensorCloseTime.Text;
                    int nTime = option.CloseSOPSensorResetWaitTime;
                    if (int.TryParse(szTime, out nTime))
                    {
                        // 입력 대기 시간 
                        option.CloseSOPSensorResetWaitTime = nTime;
                    }
                    else
                    {
                        textSopInputWaitTime.Text = nTime.ToString();
                    }
                }
            }
        }

        private void rdNoCloseSOPSensorClose_CheckedChanged(object sender, EventArgs e)
        {
            UnE.SOP.SOPCloseOption option = (UnE.SOP.SOPCloseOption)btnSaveSOPSensorCloseOption.Tag;
            if (option != null)
            {
                if (rdNoCloseSOPSensorClose.Checked == true)
                {
                    // 센서 종료시 SOP 자동 종료 사용안함
                    option.UseCloseSOPSensorReset = false;
                    option.UseCloseSOPSensorResetWaitTime = false;
                }
            }
        }

        private void btnSaveSOPWaitTimeCloseOption_Click(object sender, EventArgs e)
        {
            UnE.SOP.SOPCloseOption option = (UnE.SOP.SOPCloseOption)btnSaveSOPWaitTimeCloseOption.Tag;
            if(option != null)
            {
                string szTime = textSopInputWaitTime.Text;
                int nTime = option.CloseSOPWaitInputTime;
                if (int.TryParse(szTime, out nTime))
                {
                    // 입력 대기 시간 
                    option.CloseSOPWaitInputTime = nTime;
                }
                else
                {
                    textSopInputWaitTime.Text = nTime.ToString();
                }

                SaveAutoCloseOption(option);
                WriteAutoCloseDB(option.CategroyName);
                //WriteAutoCloseConfig(option.CategroyName);
            }
        }

        private void btnSaveSOPSensorCloseOption_Click(object sender, EventArgs e)
        {
            UnE.SOP.SOPCloseOption option = (UnE.SOP.SOPCloseOption)btnSaveSOPSensorCloseOption.Tag;
            if (option != null)
            {
                string szTime = this.txtSopSensorCloseTime.Text;
                int nTime = option.CloseSOPSensorResetWaitTime;
                if (int.TryParse(szTime, out nTime))
                {
                    // 입력 대기 시간 
                    option.CloseSOPSensorResetWaitTime = nTime;
                }
                else
                {
                    textSopInputWaitTime.Text = nTime.ToString();
                }
                SaveAutoCloseOption(option);
                WriteAutoCloseDB(option.CategroyName);
                //WriteAutoCloseConfig(option.CategroyName);
            }
        }

        private void cmbDisasterType1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Data_DisasterCategory item = (Data_DisasterCategory)cmbDisasterType1.SelectedItem;
            if( item != null)
            {
                try
                {
                    UnE.SOP.SOPCloseOption option = new SOPCloseOption();
                    option.CategroyName = item.CategoryName;

                    ReadAutoCloseDB(option);
                    //ReadAutoCloseConfig(option);

                    btnSaveSOPWaitTimeCloseOption.Tag = option;
                    ChangeCloseOptioUI(option);
                }
                catch(Exception)
                {
                    btnSaveSOPWaitTimeCloseOption.Tag = null;
                }               
            }
        }

        private void cmbDisasterType2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Data_DisasterCategory item = (Data_DisasterCategory)cmbDisasterType2.SelectedItem;
            if (item != null)
            {
                try
                {
                    UnE.SOP.SOPCloseOption option = new SOPCloseOption();
                    option.CategroyName = item.CategoryName;

                    ReadAutoCloseDB(option);
                    //ReadAutoCloseConfig(option);

                    btnSaveSOPSensorCloseOption.Tag = option;
                    ChangeCloseOptioUI2(option);
                }
                catch (Exception)
                {
                    btnSaveSOPSensorCloseOption.Tag = null;
                }
            }
        }
        private void ChangeCloseOptioUI2(SOPCloseOption option)
        {
            if (option != null)
            {                
                txtSopSensorCloseTime.Text = option.CloseSOPSensorResetWaitTime.ToString();
                rdCloseSOPSensorCloseTimeWait.Checked = option.UseCloseSOPSensorResetWaitTime;
                rdCloseSOPSensorClose.Checked = option.UseCloseSOPSensorReset;
                rdNoCloseSOPSensorClose.Checked = (!option.UseCloseSOPSensorResetWaitTime && !option.UseCloseSOPSensorReset);
               
            }
        }
        private void ChangeCloseOptioUI(SOPCloseOption option)
        {
            if (option != null)
            {
                textSopInputWaitTime.Text = option.CloseSOPWaitInputTime.ToString();
                rdSOPCloseWaitTime.Checked = option.UseCloseSOPWaitInputTime;
                rdNoCloseSOPWaitTime.Checked = !option.UseCloseSOPWaitInputTime;
               
            }               
        }

        private void btnExternalFolderPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            dlg.Description = "외부실행 프로그램이 존재하는 폴더 경로를 지정하세요.";

            if (m_strExternalFolderPath.Length > 0)
                dlg.SelectedPath = m_strExternalFolderPath;
            else
                dlg.SelectedPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                textBoxExternalFolderPath.Text = dlg.SelectedPath;

                if (m_strExternalFolderPath != textBoxExternalFolderPath.Text)
                {
                    m_strExternalFolderPath = textBoxExternalFolderPath.Text;

                    string strFilePath = GetCommonConfigPath();
                    m_util.setinivalue("ExternalRun", "Folder", m_strExternalFolderPath, strFilePath);
                }
            }
        }

        private string GetCommonConfigPath()
        {
            return System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\config.ini";
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
