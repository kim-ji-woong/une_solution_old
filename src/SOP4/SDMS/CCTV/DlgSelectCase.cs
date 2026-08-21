using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;


namespace SDMS
{
	public partial class DlgSelectCase : Form, IPOIPopup
	{
		private static int m_nProcessingSensorHistoryID = -1;

		public static int ProcessingSensorHistoryID
		{
			get { return m_nProcessingSensorHistoryID; }
			set { m_nProcessingSensorHistoryID = value; }
		}

        public ISensor Sensor
        {
            get;
            set;
        }


        public static void CreateDialog()
        {
            if (m_Instance == null || m_Instance.IsDisposed == true)
            {
                m_Instance = new DlgSelectCase();
            }
        }

		private static DlgSelectCase m_Instance = new DlgSelectCase();

        private string m_strFireAlarmDepartmentName = "";
        private string m_strFireAlarmDepartmentPhoneNumber = "";

		public static DlgSelectCase Instance
		{
			get
			{
				return m_Instance;
			}
			set 
            {
                m_Instance = value;
            }
		}

        private int m_nHeight = 105; // 71
        private int m_nDetectFireCount = 0;
        public int DetectFireCount
        {
            get { return m_nDetectFireCount; }
            set
            {
                m_nDetectFireCount = value;

                int dx = btnStartSOP.Width;
                if (m_nDetectFireCount > 1)
                {
                    labelHeader.Visible = labelDetectCount.Visible = true;
                    labelDetectCount.Text = m_nDetectFireCount.ToString() + "개";

                    if( btnStartSOP.Visible == true)
                    {
                        this.Size = new Size(800, m_nHeight);
                    }
                    else
                    {
                        this.Size = new Size(800 - dx, m_nHeight);
                    }
                }
                else
                {
                    labelHeader.Visible = labelDetectCount.Visible = false;
                    if (btnStartSOP.Visible == true)
                    {
                        this.Size = new Size(650, m_nHeight);
                    }
                    else
                    {
                        this.Size = new Size(650 - dx, m_nHeight);
                    }
                }
            }
        }

        public void ViewCCTV()
        {
            FormMain.Instance.PageHome.Invoke((MethodInvoker)delegate
            {
                FormMain.Instance.ShowCCTVForm(true);

                if (mCurrentData.Sensor != null)
                {
                    EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(mCurrentData.Sensor.m_ZoneID);
                    Zone zone = equipZone.LinkedZone;
                    if (zone != null)
                    {
                        int nMode = 1;
                        if (mCurrentData.Sensor.Type == IFacility.FacilityType.PSM_SENSOR)
                        {
                            nMode = 2;
                        }

                        FormMain.Instance.CCTVPipe.Send("SetHistoryID(" + mCurrentData.SensorHistoryID + ")");
                        FormMain.Instance.PageHome.ShowBigCCTV(zone, nMode, true);

                        FormMain.Instance.SelectCCTVTab(false);

                    }
                }
                else
                {
                    MessageBox.Show("Cant find Zone");
                }
            });
        }

		private int GetSensorHistoryID(int nSensorID)
		{
			if (ProcessManager.Instance.CurrentDetectProcess.ContainsKey(nSensorID))
			{
				ProcessIF process = ProcessManager.Instance.CurrentDetectProcess[nSensorID];
				return process.SensorHistoryID;
			}
			return -1;
		}

        public bool ReportSpill()
        {
            FormMain.Instance.SendDetectMessageToSOPSimulator();

            ISensor m_Sensor = mCurrentData.Sensor;
            int nSensorID = m_Sensor.ID;
            int ZoneID = m_Sensor.EquipZoneID;

            int nHistoryID = GetSensorHistoryID(nSensorID);

            int nSOPGenUserID = FormMain.Instance.SOPGenUserID;
            return SDMS.NetworkManager.Instance.SendMessage(1, TCP_ID.PSM_DETECT_REPORT, nHistoryID, ZoneID, nSensorID, nSOPGenUserID);
        }

		public bool ReportFire()
		{
			FormMain.Instance.SendDetectMessageToSOPSimulator();

			ISensor m_Sensor = mCurrentData.Sensor;
			int nSensorID = m_Sensor.ID;
			int ZoneID = m_Sensor.EquipZoneID;

			int nHistoryID = GetSensorHistoryID(nSensorID);

			int nSOPGenUserID = FormMain.Instance.SOPGenUserID;
            return SDMS.NetworkManager.Instance.SendMessage(1, TCP_ID.FIRE_DETECT_REPORT, nHistoryID, ZoneID, nSensorID, nSOPGenUserID);
		}

        public bool ReportSecurity()
        {
            FormMain.Instance.SendDetectMessageToSOPSimulator();

            ISensor m_Sensor = mCurrentData.Sensor;
            int nSensorID = m_Sensor.ID;
            int EquipZoneID = m_Sensor.EquipZoneID;

            int nHistoryID = GetSensorHistoryID(nSensorID);

            int nSOPGenUserID = FormMain.Instance.SOPGenUserID;
            return SDMS.NetworkManager.Instance.SendMessage(1, TCP_ID.SECURITY_DETECT_REPORT, nHistoryID, EquipZoneID, nSensorID, nSOPGenUserID);
        }

		public void ReportFireTranning()
		{
			//int nSensorID = m_Sensor.ID;
			//int ZoneID = m_Sensor.EquipZoneID;

			//int nHistoryID = GetSensorHistoryID(nSensorID);
			//NetworkManager.Instance.SendMessage(TCP_ID.FIRE_DETECT_TRAINNING, nHistoryID, ZoneID, nSensorID);

			//FormMain.Instance.SendFireDetectMessageToSOPSimulator();
		}


        public bool BuzzerToggle(bool bOn)
        {
            ISensor m_Sensor = mCurrentData.Sensor;
            int nSensorID = m_Sensor.ID;
            ProcessIF process = ProcessManager.Instance.GetProcess(nSensorID);
            if (process != null)
            {
                int nSOPGenUserID = FormMain.Instance.SOPGenUserID;
                // 서버로 오작동 신고를 수행한다.
                return SDMS.NetworkManager.Instance.SendMessage(1, TCP_ID.PSM_BUZZER_STOP, bOn == true ? 1 : 0, nSensorID, nSOPGenUserID);
            }
            return false;
        }

        public bool ReportPSMReset(string strDescriptionText)
        {
            ISensor m_Sensor = mCurrentData.Sensor;
			int nSensorID = m_Sensor.ID;
			ProcessIF process = ProcessManager.Instance.GetProcess(nSensorID);
			if (process != null)
			{
				int nSOPGenUserID = FormMain.Instance.SOPGenUserID;
				// 서버로 오작동 신고를 수행한다.
                return SDMS.NetworkManager.Instance.SendMessage(1, TCP_ID.PSM_SENSOR_RESET, process.SensorHistoryID, nSensorID, nSOPGenUserID, strDescriptionText);
			}
			return false;
        }

		public bool ReportAbnormal(string strDescriptionText)
		{
            ISensor m_Sensor = mCurrentData.Sensor;
			int nSensorID = m_Sensor.ID;
			ProcessIF process = ProcessManager.Instance.GetProcess(nSensorID);
			if (process != null)
			{
				int nSOPGenUserID = FormMain.Instance.SOPGenUserID;
				// 서버로 오작동 신고를 수행한다.
                return SDMS.NetworkManager.Instance.SendMessage(1, TCP_ID.MALFUNCTION_REPORT, process.SensorHistoryID, nSensorID, nSOPGenUserID, strDescriptionText);
			}
			return false;
		}

        public bool ReportNoSecurity(string strDescriptionText)
        {
            ISensor m_Sensor = mCurrentData.Sensor;
            int nSensorID = m_Sensor.ID;
            ProcessIF process = ProcessManager.Instance.GetProcess(nSensorID);
            if (process != null)
            {
                int nSOPGenUserID = FormMain.Instance.SOPGenUserID;
                // 서버로 오작동 신고를 수행한다.
                return SDMS.NetworkManager.Instance.SendMessage(1, TCP_ID.MALFUNCTION_REPORT, process.SensorHistoryID, nSensorID, nSOPGenUserID, strDescriptionText);
            }
            return false;
        }

		private void BtnReportFire_Click(object sender, EventArgs e)
		{

            if(mCurrentData.Sensor == null)
            {
                MessageBox.Show("지정된 센서가 없습니다.");
                return;
            }


            string szPopupMessage = "관련인원에게 화재발생을 전파합니다.\r\n화재발생으로 신고하시겠습니까?";
            if( m_bDlgType == 1)
            {
                szPopupMessage = "전직원에게 유해화학물질 누출을 전파합니다.\r\n유해화학물질 누출발생으로 신고하시겠습니까?";
            }
            else if(m_bDlgType == 2)
            {
                szPopupMessage = "관련인원에게 실제 상황으로 전파합니다.\r\n계속 하시겠습니까?";
            }          

            if (MessageBox.Show(szPopupMessage, "확인", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    return;
			
			if (FireDetectProcess.SoundPlayer.SoundLocation != null)
				FireDetectProcess.SoundPlayer.Stop();

            if (m_bDlgType == 1)
            {
                if (ReportSpill())
                {
                    FireDetectProcess.SoundPlayer.Stop();
                    this.Visible = false;
                    HideDialog();

                    ConfirmDialogManager.Instance.RemoveDialog(mCurrentData, true);
                    ConfirmDialogManager.Instance.ShowDialogNext();

                    FormMain.Instance.SelectCCTVTab(false);
                    FormMain.Instance.PageHome.Invoke((MethodInvoker)delegate
                    {
                        ISensor m_Sensor = mCurrentData.Sensor;
                        Zone zone = ZoneManager.Instance.GetZone(m_Sensor.m_ZoneID);
                        if (zone != null)
                        {
                            FormMain.Instance.PageHome.ShowBigCCTV(zone, 0);
                        }

                    });
                }
                else
                {
                    MessageBox.Show("서버가 연결되지 않았습니다. 잠시 후에 시도하세요", "확인", MessageBoxButtons.OK);
                }
            }
            else if (m_bDlgType == 2)
            {
                if (ReportSecurity())
                {
                    FireDetectProcess.SoundPlayer.Stop();
                    this.Visible = false;
                    HideDialog();
                    ConfirmDialogManager.Instance.RemoveDialog(mCurrentData, true);
                    ConfirmDialogManager.Instance.ShowDialogNext();

                    FormMain.Instance.SelectCCTVTab(false);
                    FormMain.Instance.PageHome.Invoke((MethodInvoker)delegate
                    {
                        ISensor m_Sensor = mCurrentData.Sensor;

                        Zone zone = ZoneManager.Instance.GetZone(m_Sensor.m_ZoneID);
                        if (zone != null)
                        {
                            FormMain.Instance.PageHome.ShowBigCCTV(zone, 0);
                        }
                    });
                }
                else
                {
                    MessageBox.Show("서버가 연결되지 않았습니다. 잠시 후에 시도하세요", "확인", MessageBoxButtons.OK);
                }
            }
            else if(m_bDlgType == 0)
            {
                if (ReportFire())
                {
                    FireDetectProcess.SoundPlayer.Stop();
                    this.Visible = false;              
                    HideDialog();
                    ConfirmDialogManager.Instance.RemoveDialog(mCurrentData, true);
                    ConfirmDialogManager.Instance.ShowDialogNext();

                    FormMain.Instance.SelectCCTVTab(false);
                    FormMain.Instance.PageHome.Invoke((MethodInvoker)delegate
                    {
                        ISensor m_Sensor = mCurrentData.Sensor;

                        Zone zone = ZoneManager.Instance.GetZone(m_Sensor.m_ZoneID);
                        if (zone != null)
                        {
                            FormMain.Instance.PageHome.ShowBigCCTV(zone, 0);
                        }
                    });
                }
                else
                {
                    MessageBox.Show("서버가 연결되지 않았습니다. 잠시 후에 시도하세요", "확인", MessageBoxButtons.OK);
                }
            }
			
		}

		public void Thread_BroadCast()
		{
            ISensor sensorZone = mCurrentData.Sensor;

			if (sensorZone == null || sensorZone.EquipZoneID <= 0)
				return;

			EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(sensorZone.EquipZoneID);

			if (equipZone == null)
				return;

			string strFireZoneName = equipZone.DisplayText;

			SimulationBroadcastOption option = SimulationBroadcastOption.Instance;

			if (option.UseBroadcast2 == false)
				return;

			string strSiren = option.UseSiren ? "1" : "0";
			int nRepeat = option.RadioRepeat - 1;

			string strServerName = FormMain.Instance.DBManager.LoadIni("Server_Name", "Simulation");
			string strPort = FormMain.Instance.DBManager.LoadIni("Port", "Simulation");

			string strMessage = "\"" + Simulation.GetBroadcastMessage(option.ReportMessage, strFireZoneName, nRepeat) + "\"";

            Simulation.Instance.RunSimulationTimer(strSiren, strServerName, strPort, strMessage);

			/*this.Invoke((MethodInvoker)delegate
			{
				try
				{
					string strServerName = FormMain.Instance.DBManager.LoadIni("Server_Name", "Simulation");
					string strPort = FormMain.Instance.DBManager.LoadIni("Port", "Simulation");
					using (libTTS.Broadcast br = new libTTS.Broadcast(strServerName, strPort))
					{
						SimulationBroadcastOption option = SimulationBroadcastOption.Instance;

						bool isDetectFire = option.UseBroadcast;
						bool isReportFire = option.UseBroadcast2;
						bool isSiren = option.UseSiren;
						int nRepeat = option.RadioRepeat;

						//Count가 0이면 사내방송이 안나옴
						int nCount = 0;

						//화재신고시 사내방송 실시여부
						if (isReportFire)
							nCount = nRepeat;

						string strMessage = option.ReportMessage;

						if (nCount == 0)
							nCount = 1;
						br.AddSpeech(strMessage, nCount, isSiren);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString());
				}
			});*/
		}

		private void BtnSoundOnOff_Click(object sender, EventArgs e)
		{
            ISensor m_Sensor = mCurrentData.Sensor;
			if (m_Sensor.SoundOn)
			{

                if (m_bDlgType == 1)
                {
                    btnSound.Text = "부저 ON";
                }
                else
                {
                    btnSound.Text = "소리 켜짐";
                }

                FireDetectProcess.SoundPlayer.Stop();
                //FireDetectProcess.Beep.Stop();

                // 실제 센서의 Buzzer 끄는 기능 사용안함
                //BuzzerToggle(false);
                
			}
			else
			{
                if (m_bDlgType == 1)
                {
                    btnSound.Text = "부저 OFF";
                }
                else
                {
                    btnSound.Text = "소리 꺼짐";
                }
                FireDetectProcess.SoundPlayer.Play();
                //FireDetectProcess.Beep.Play();

                // 실제 센서의 Buzzer 켜는 기능 사용안함
                //BuzzerToggle(true);
			}

			m_Sensor.SoundOn = !m_Sensor.SoundOn;
		}

		private void BtnReportMalfunction_Click(object sender, EventArgs e)
		{
            if (m_bDlgType == 1)
            {
                UnE.PSM.PSMSensor sensor = PSMManager.Instance.GetSensor(mCurrentData.Sensor.OrgSensorID);
                if (sensor.CurrentData >= sensor.LimitLevel1)
                {
                    UnE.PSM.PSMMaterial material = PSMManager.Instance.GetMaterial(sensor.MaterialType);

                    string strMsg = string.Format("현재 복구하려는 신호는 최소 알람기준인 {0}{1}을 넘어섰습니다.\n", sensor.LimitLevel1, material.UOM);
                    strMsg += "(유해 화학물질 리스트 참조)\n";
                    strMsg += string.Format("신호를 복구하기 위해서는 현장에서 감지한 센서의 농도가 {0}{1} 이하여야만 합니다.\n", sensor.LimitLevel1, material.UOM);
                    strMsg += "시스템의 사이렌 소리만 끄고자 할 경우 오른쪽 [부저 OFF] 버튼을 눌러주시기 바랍니다.";
                    MessageBox.Show(strMsg, "확인", MessageBoxButtons.OK);
                }
                else
                {
                    string strDescription = "신호복구시 남길 메모가 있으면 아래에 입력해 주세요.";
                    string strDescriptionText = "";

                    if (IMessageBox.InputMessageBox.Show("현장의 이상유무 확인하셨나요?\r\n현장 신호를 복구하시겠습니까?", "확인", MessageBoxButtons.YesNo, strDescription, ref strDescriptionText) == System.Windows.Forms.DialogResult.No)
                        return;

                    if (FireDetectProcess.SoundPlayer.SoundLocation != null)
                        FireDetectProcess.SoundPlayer.Stop();

                    if (ReportPSMReset(strDescriptionText))
                    {
                        FireDetectProcess.SoundPlayer.Stop();

                        DialogResult = DialogResult.Cancel;
                        HideDialog();

                        try
                        {
                            ConfirmDialogManager.Instance.RemoveDialog(mCurrentData, false);
                        }
                        catch (System.Exception)
                        {
                        }
                    }
                    else
                    {
                        MessageBox.Show("서버가 연결되지 않았습니다. 잠시 후에 시도하세요", "확인", MessageBoxButtons.OK);
                    }
                }
            }
            else if (m_bDlgType == 0)
            {
                string strDescription = "오작동 복구시 남길 메모가 있으면 아래에 입력해 주세요.";
                string strDescriptionText = "";

                if (IMessageBox.InputMessageBox.Show("화재 탐지결과를 오작동으로 신고하시겠습니까?", "확인", MessageBoxButtons.YesNo, strDescription, ref strDescriptionText) == System.Windows.Forms.DialogResult.No)
                    return;

                if (FireDetectProcess.SoundPlayer.SoundLocation != null)
                    FireDetectProcess.SoundPlayer.Stop();

                if (ReportAbnormal(strDescriptionText))
                {
                    FireDetectProcess.SoundPlayer.Stop();

                    DialogResult = DialogResult.Cancel;
                    HideDialog();

                    try
                    {
                        ConfirmDialogManager.Instance.RemoveDialog(mCurrentData, false);
                    }
                    catch (System.Exception)
                    {
                    }
                }
                else
                {
                    MessageBox.Show("서버가 연결되지 않았습니다. 잠시 후에 시도하세요", "확인", MessageBoxButtons.OK);
                }
            }
            else if (m_bDlgType == 2)
            {
                string strDescription = "신호복구시 남길 메모가 있으면 아래에 입력해 주세요.";
                string strDescriptionText = "";

                if (IMessageBox.InputMessageBox.Show("방범 탐지 신호를 복구하시겠습니까?\n신호를 복구해도 해당 시스템에서 복구되지 않으면 신호가 다시 발생할 수 있습니다.\n정말로 복구하시겠습니까?", "확인", MessageBoxButtons.YesNo, strDescription, ref strDescriptionText) == System.Windows.Forms.DialogResult.No)
                    return;

                if (FireDetectProcess.SoundPlayer.SoundLocation != null)
                    FireDetectProcess.SoundPlayer.Stop();

                if (ReportNoSecurity(strDescriptionText))
                {
                    FireDetectProcess.SoundPlayer.Stop();

                    DialogResult = DialogResult.Cancel;
                    HideDialog();

                    try
                    {
                        ConfirmDialogManager.Instance.RemoveDialog(mCurrentData, false);
                    }
                    catch (System.Exception)
                    {
                    }
                }
                else
                {
                    MessageBox.Show("서버가 연결되지 않았습니다. 잠시 후에 시도하세요", "확인", MessageBoxButtons.OK);
                }
            }	
		}

		public void BtnViewCCTV_Click(object sender, EventArgs e)
		{
			ViewCCTV();
        }

		static private int m_nTargetSpaceX = 30;
		static private int m_nTargetSpaceY = 50;

		private bool m_bVisible = false;

		private bool m_bLayerVisible = true;

		public bool LayerVisible
		{
			get { return m_bLayerVisible; }
			set
			{
			}
		}

		private void SetRealMode()
		{
            int dx = btnStartSOP.Width;
			bool bRealMode = true;
			if (bRealMode)
			{
				button1.Visible = false;
                
                if (m_nDetectFireCount <= 1)
                {
                    if (btnStartSOP.Visible == true)
                    {
                        Size = new Size(650, this.Size.Height);
                    }
                    else
                    {
                        Size = new Size(650 - dx, this.Size.Height);
                    }
                }
                else
                {
                    if (btnStartSOP.Visible == true)
                    {
                        Size = new Size(800, this.Size.Height);
                    }
                    else
                    {
                        Size = new Size(800 - dx, this.Size.Height);
                    } 
                }

                if (btnStartSOP.Visible == true)
                {
                    labelHeader.Location = new Point(640, labelHeader.Location.Y);
                    labelDetectCount.Location = new Point(745, labelDetectCount.Location.Y);
                }
                else
                {
                    labelHeader.Location = new Point(640 - dx, labelHeader.Location.Y);
                    labelDetectCount.Location = new Point(745 - dx, labelDetectCount.Location.Y);
                }                
			}
			else
			{
				button1.Visible = true;

                if (m_nDetectFireCount <= 1)
                {
                    if (btnStartSOP.Visible == true)
                    {
                        Size = new Size(800, this.Size.Height);
                    }
                    else
                    {
                        Size = new Size(800 - dx, this.Size.Height);
                    }
                }
                else
                {
                    if (btnStartSOP.Visible == true)
                    {
                        Size = new Size(945, this.Size.Height);
                    }
                    else
                    {
                        Size = new Size(945 - dx, this.Size.Height);
                    }
                }
			}
		}

        // 0 : fire 2: gas 3:security
        private int m_bDlgType = 0; 
       
		private SeletCaseData mCurrentData = null;

		public SeletCaseData CurrentData
		{
			get
			{
				return mCurrentData;
			}
			set
			{
				mCurrentData = value;

				if (mCurrentData.ViewOwner == null)
					InitData(mCurrentData.Sensor, mCurrentData.SensorHistoryID);
				else
					InitData(mCurrentData.ViewOwner, mCurrentData.Sensor, mCurrentData.SensorHistoryID);


                if (mCurrentData.ProcessType == ProcessType.PSMAlarm)
                {
                    m_bDlgType = 1;
                    this.Text = "누출 탐지신호 수신 - " + mCurrentData.Sensor.PositionName;

                    mBtnReportFire.Text = "누출 전파";
                    mBtnReportMalfunction.Text = "신호복구";
                    btnSound.Text = "부저 OFF";

                    UnE.PSM.PSMSensor sensor = PSMManager.Instance.GetSensor(mCurrentData.Sensor.OrgSensorID);
                    if( sensor != null)
                    {
                        label2.Text = sensor.Department;
                        label4.Text = sensor.PhoneNumber;
                    }
                    else
                    {
                        label2.Text = "";
                        label4.Text = "";
                    } 
                }
                else if(mCurrentData.ProcessType == ProcessType.SecurityAlarm)
                {
                    m_bDlgType = 2;
                    this.Text = "방범 신호 수신 - " + mCurrentData.Sensor.PositionName;

                    mBtnReportFire.Text = "상황 전파";
                    mBtnReportMalfunction.Text = "신호 복구";

                    label2.Text = m_strFireAlarmDepartmentName;
                    label4.Text = m_strFireAlarmDepartmentPhoneNumber;
                }
                else if(mCurrentData.ProcessType == ProcessType.FireAlarm)
                {
                    m_bDlgType = 0;
                    this.Text = szTitle + " - " + mCurrentData.Sensor.PositionName;

                    mBtnReportFire.Text = "화재 전파";
                    mBtnReportMalfunction.Text = "오작동/복구";

                    label2.Text = m_strFireAlarmDepartmentName;
                    label4.Text = m_strFireAlarmDepartmentPhoneNumber;
                }                
                
                this.btnStartSOP.Visible = mCurrentData.ShowOpenSOP;

                int dx = btnStartSOP.Width;
                if (m_nDetectFireCount > 1)
                {
                    labelHeader.Visible = labelDetectCount.Visible = true;
                    labelDetectCount.Text = m_nDetectFireCount.ToString() + "개";

                    if (mCurrentData.ShowOpenSOP == true)
                    {
                        this.Size = new Size(800, m_nHeight);
                    }
                    else
                    {
                        this.Size = new Size(800 - dx, m_nHeight);
                    }
                }
                else
                {
                    labelHeader.Visible = labelDetectCount.Visible = false;
                    if (mCurrentData.ShowOpenSOP == true)
                    {
                        this.Size = new Size(650, m_nHeight);
                    }
                    else
                    {
                        this.Size = new Size(650 - dx, m_nHeight);
                    }
                }

                if (mCurrentData.ShowOpenSOP == true)
                {
                    labelHeader.Location = new Point(640, labelHeader.Location.Y);
                    labelDetectCount.Location = new Point(745, labelDetectCount.Location.Y);
                }
                else
                {
                    labelHeader.Location = new Point(640 - dx, labelHeader.Location.Y);
                    labelDetectCount.Location = new Point(745 - dx, labelDetectCount.Location.Y);
                }      
			}
		}

        private bool GetFireAlarmDepartmentInfo(ref string strDepartment, ref string strPhoneNumber)
        {
            string strDepartmentTag = "FireAlarmDepartment", strPhoneNumberTag = "FireAlarmDepartmentPhoneNumber";

            string strSQL = "Select PropertyName, PropertyValue from OptionSDMS where (PropertyName = '" + strDepartmentTag;
            strSQL += "' or PropertyName = '" + strPhoneNumberTag + "') and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                string strPropertyName = DBUtility.WebDBManager.GetStringField(arrResult[i]);
                string strPropertyValue = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);

                if (strPropertyValue == null || strPropertyValue == null)
                    continue;

                if (string.Compare(strPropertyName, strDepartmentTag, true) == 0)
                    strDepartment = strPropertyValue;
                else if (string.Compare(strPropertyName, strPhoneNumberTag, true) == 0)
                    strPhoneNumber = strPropertyValue;
            }

            return true;
        }

        private string szTitle = "";
		public DlgSelectCase()
		{
			InitializeComponent();
            szTitle = this.Text;
            GetFireAlarmDepartmentInfo(ref m_strFireAlarmDepartmentName, ref m_strFireAlarmDepartmentPhoneNumber);

            ReadPosition();
		}

		public System.Windows.Forms.Timer CheckTimer
		{
			get { return timer1; }
		}

		private void InitData(ISensor sensor, int nSensorHistoryID)
		{
			ISensor m_Sensor = mCurrentData.Sensor;
			m_Sensor = sensor;
			mCurrentData.SensorHistoryID = nSensorHistoryID;

			SetRealMode();
			InitSound();

			timer1.Start();
		}

        private void InitData(ISensorTooltipOwner view, ISensor sensor, int nSensorHistoryID)
		{
			ISensor m_Sensor = mCurrentData.Sensor;
			m_Sensor = sensor;
			mCurrentData.SensorHistoryID = nSensorHistoryID;
			mCurrentData.ViewOwner = view;

			SetRealMode();
			InitSound();

			timer1.Start();
		}

		private void InitSound()
		{
			ISensor m_Sensor = mCurrentData.Sensor;
			if (m_Sensor == null)
				return;

			if (m_Sensor.SoundOn)
			{
				btnSound.Text = "소리 꺼짐";
				FireDetectProcess.PlaySound();
			}
			else
			{
				btnSound.Text = "소리 켜짐";
				FireDetectProcess.SoundPlayer.Stop();
			}
		}

		// xTarget, yTarget : Target POI의 좌표
		public void Show(int xTarget, int yTarget)
		{
			int x = xTarget + m_nTargetSpaceX;
			int y = yTarget - m_nTargetSpaceY;

			this.Location = new Point(x, y);

            btnStartSOP.Visible = mCurrentData.ShowOpenSOP;

			m_bVisible = true;

            
			this.Show();
		}

		// Panning이나 Orbit같은 동작을 위하여 잠시동안 임시로 꺼두는 것인가?
		private bool IsTemporaryHidden()
		{
			ISensor m_Sensor = mCurrentData.Sensor;
			if (mCurrentData.ViewOwner == null)
				return false;

			if (mCurrentData.POI == null)
				return false;

			return mCurrentData.ViewOwner.IsTemporaryHiddenPOI(mCurrentData.POI);
		}

		public void Hide(bool absolutely)
		{
			base.Hide();
			m_bVisible = false;
		}

		public void MoveTarget(int xTarget, int yTarget)
		{
			int x = xTarget + m_nTargetSpaceX;
			int y = yTarget - m_nTargetSpaceY;

			this.Location = new Point(x, y);
		}

		public bool IsVisible()
		{
			if (m_bLayerVisible == true && m_bVisible == true)
				return true;
			return Visible;
		}

		public void HideDialog()
		{
			if (mCurrentData != null)
			{
				ISensor m_Sensor = mCurrentData.Sensor;
				//m_Sensor.SoundOn = true;
			}
            if( this.Visible == true)
            {
                SavePosition();
            }

			timer1.Stop();
			m_bLayerVisible = false;
			m_bVisible = false;
			Visible = false;
			//mCurrentData = null;
			//base.Close();

           

		}

        private Point ptPosition = new Point();
        private void ReadPosition()
        {
            int nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;
            
            // read position in registry
            string strX = DBUtility.RegUtil.ReadRegValue("SDMS", "DlgSelectPosX", nSiteID);
            string strY = DBUtility.RegUtil.ReadRegValue("SDMS", "DlgSelectPosY", nSiteID);

            int x = this.Location.X;
            int y = this.Location.Y;
            
            if (strX != null && strX != "")
                int.TryParse(strX, out x);

            if (strY != null && strY != "")
                int.TryParse(strY, out y);

            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;

            bool other = true;
            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                Screen sc = Screen.AllScreens[i];

                if (x >= sc.Bounds.X && sc.Bounds.X + sc.Bounds.Width > x &&
                    y >= sc.Bounds.Y && sc.Bounds.Y + sc.Bounds.Height > y)
                {
                    other = false;

                    int limitX = ptPosition.X + this.Size.Width;
                    if (limitX > sc.Bounds.X + sc.Bounds.Width)
                    {
                        x = sc.Bounds.X + sc.Bounds.Width - this.Width;
                    }

                    int limitY = ptPosition.Y + this.Size.Height;
                    if (limitY > sc.Bounds.Y + sc.Bounds.Height)
                    {
                        y = sc.Bounds.Y + sc.Bounds.Height - this.Height;
                    }

                    break;
                }
            }

            // 화면을 완전히 벗어난경우
            if (other)
            {
                x = (Screen.AllScreens.Length > 0) ? Screen.AllScreens[0].Bounds.X : 0;
                y = (Screen.AllScreens.Length > 0) ? Screen.AllScreens[0].Bounds.Y : 0;
            }

            if (this.Parent != null)
            {
                Point loc = this.Parent.PointToClient(ptPosition);
                this.Location = loc;
            }      
            else
            {
                ptPosition = new Point(x, y);
                this.Location = ptPosition;
            }
        }

        private void SavePosition()
        {
            int nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

            if (this.Parent != null)
            {
                Point loc = this.Parent.PointToClient(Location);
                ptPosition = loc;
            }
            else
            {
                ptPosition = this.Location;
            }
            
            // 삐져나갔나
            if (ptPosition.X < 0)
                ptPosition.X = 0;
            if (ptPosition.Y < 0)
                ptPosition.Y = 0;

            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                Screen sc = Screen.AllScreens[i];

                if (ptPosition.X >= sc.Bounds.X && sc.Bounds.X + sc.Bounds.Width > ptPosition.X &&
                    ptPosition.Y >= sc.Bounds.Y && sc.Bounds.Y + sc.Bounds.Height > ptPosition.Y)
                {                    
                    int limitX = ptPosition.X + this.Size.Width;                    
                    if (limitX > sc.Bounds.X + sc.Bounds.Width)
                    {                        
                        ptPosition.X = sc.Bounds.X + sc.Bounds.Width - this.Width;
                    }

                    int limitY = ptPosition.Y + this.Size.Height;
                    if (limitY > sc.Bounds.Y + sc.Bounds.Height)
                    {
                        ptPosition.Y = sc.Bounds.Y + sc.Bounds.Height - this.Height;
                    }

                    break;
                }
            }

            // save position to registry
            DBUtility.RegUtil.WriteRegValue("SDMS", "DlgSelectPosX", ptPosition.X.ToString(), nSiteID);
            DBUtility.RegUtil.WriteRegValue("SDMS", "DlgSelectPosY", ptPosition.Y.ToString(), nSiteID);
        }

        public new void Show()
        {
            ReadPosition();

            base.Show();
        }

        public new void Show(IWin32Window owner)
        {
            ReadPosition();

            base.Show(owner);
        }

        public new DialogResult ShowDialog()
        {
            ReadPosition();

            return base.ShowDialog();
        }

		public new DialogResult ShowDialog(IWin32Window owner)
		{
            ReadPosition();

			return base.ShowDialog(owner);
		}

		// 상황 해제되었는지 감시
		private void timer1_Tick(object sender, EventArgs e)
		{
			ISensor m_Sensor = mCurrentData.Sensor;
			if (m_Sensor == null)
			{
				HideDialog();
				ConfirmDialogManager.Instance.RemoveDialog(mCurrentData, true);
			}
			else
			{
				ProcessIF process = ProcessManager.Instance.GetProcess(m_Sensor.ID);

				if (process == null)
				{
					HideDialog();
					ConfirmDialogManager.Instance.RemoveDialog(mCurrentData, false);
				}
				else if (m_nProcessingSensorHistoryID > 0 && mCurrentData.SensorHistoryID == m_nProcessingSensorHistoryID)
				{
					HideDialog();
					ConfirmDialogManager.Instance.RemoveDialog(mCurrentData, false);
				}
			}
		}

		private void DlgSelectCase_Load(object sender, EventArgs e)
		{
            //if (FormMain.Instance.ShowEquipZoneCCTV)
            //{
            //    // 이미 큰 CCTV 화면을 보고 있는 경우는 DlgSelectCase 창을 감춘다.
            //    BtnViewCCTV_Click(null, null);
            //}

            btnStartSOP.Visible = mCurrentData.ShowOpenSOP;
         
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (FireDetectProcess.SoundPlayer.SoundLocation != null)
				FireDetectProcess.SoundPlayer.Stop();

			ReportFireTranning();

			DialogResult = DialogResult.Yes;
			HideDialog();

			ConfirmDialogManager.Instance.RemoveDialog(mCurrentData);

			FormMain.Instance.PageHome.Invoke((MethodInvoker)delegate
			{
				ISensor m_Sensor = mCurrentData.Sensor;
				FormMain.Instance.PageHome.ShowBigCCTV(m_Sensor.POI.Zone, 0);
			});
		}

        // 경보에 따른 SOP가동 버튼 클릭 이벤트
        private void btnStartSOP_Click(object sender, EventArgs e)
        {
            if(mCurrentData != null)
            {
                ISensor sensor = mCurrentData.Sensor;
                int nSensorZoneHistoryID = mCurrentData.SensorHistoryID;
                if(sensor != null)
                {
                    int nEquipZone = sensor.EquipZoneID;
                    EquipmentZone zone = ZoneManager.Instance.GetEquipZone(nEquipZone);

                    if (zone != null)
                    {
                        ProcessIF process = ProcessManager.Instance.FindProcess(nSensorZoneHistoryID);
                        if( process == null)
                        {
                            process = FormMain.Instance.CurrentSensorDetectProcess;
                        }

                        if (FormMain.Instance.ProxyMessenger != null)
                        {
                            if (FormMain.Instance.ProxyMessenger.IsVisibleSOPSimulator() == false)
                                FormMain.Instance.ProxyMessenger.ShowSOPSimulator();
                        }

                        if( process != null)
                        {
                            FormMain.Instance.OpenSOPClicked(zone, mCurrentData.DetectTime, process);
                        }
                    }
                }                
            }            
        }

        private void DlgSelectCase_VisibleChanged(object sender, EventArgs e)
        {
            int i = 0;
            i++;
        }
	}

	public class ConfirmDialogManager
	{
		private static ConfirmDialogManager m_instance = null;

		public static ConfirmDialogManager Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new ConfirmDialogManager();
				return m_instance;
			}
			set { m_instance = value; }
		}

		private ArrayList m_arForm = new ArrayList();

		private ConfirmDialogManager()
		{
		}

		public SeletCaseData FindForm(int nSensorHistoryID)
		{
			foreach (SeletCaseData form in m_arForm)
			{
				if (form.SensorHistoryID == nSensorHistoryID)
				{
					return (SeletCaseData)form;
				}
			}
			return null;
		}

		public SeletCaseData ShowDialog(int nSensorHistoryID, int nSensorID)
		{
			if (m_arForm.Count == 0)
				return null;

			SeletCaseData form = FindForm(nSensorHistoryID);

			if (DlgSelectCase.Instance.CurrentData == form &&
				DlgSelectCase.Instance.Visible == true)
				return null;
			else
			{
				DlgSelectCase.Instance.HideDialog();
			}

			try
			{
				CloseAllDialog();

                if (DlgSelectCase.Instance.IsDisposed == true)
                {
                    DlgSelectCase.CreateDialog();
                }

				FormMain.Instance.Invoke((MethodInvoker)delegate
				{
					DlgSelectCase.Instance.CurrentData = form;
					DlgSelectCase.Instance.StartPosition = FormStartPosition.Manual;
					DlgSelectCase.Instance.Location = PageBackstageHome.Instance.SelectCaseDetectPosition();
					DlgSelectCase.Instance.TopMost = true;

                    //if (FormMain.Instance.PageHome.CCTVForm.Visible == true)
                    //{
                    //    DlgSelectCase.Instance.BtnViewCCTV_Click(null, null);
                    //}
                    //else
                    {
                        try
                        {
                            if (FormFrame.Instance.WindowState == FormWindowState.Minimized)
                            {
                                FormFrame.Instance.WindowState = FormWindowState.Maximized;
                            }
                            FormFrame.Instance.BringToFront();

                            if (DlgSelectCase.Instance.Visible == true)
                                DlgSelectCase.Instance.Visible = false;

                            DlgSelectCase.Instance.Show(FormFrame.Instance);
                            DlgSelectCase.Instance.Visible = true;
                            DlgSelectCase.Instance.BringToFront();
                        }
                        catch (Exception)
                        { }
                    }
				});
			}
			catch (System.Exception)
			{
			}
			return form;
		}

        private void ShowDialogThread()
        {
            FormMain.Instance.Invoke((MethodInvoker)delegate
			{
                try
                {

                    if (DlgSelectCase.Instance.Visible == true)
                        DlgSelectCase.Instance.Visible = false;


                    DlgSelectCase.Instance.Show(FormFrame.Instance);
                    DlgSelectCase.Instance.Visible = true;
                    DlgSelectCase.Instance.BringToFront();
                    
                }catch(Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                    System.Diagnostics.Trace.WriteLine(e.StackTrace);
                }
            });
        }
		public SeletCaseData ShowDialogNext()
		{
			if (m_arForm.Count == 0)
				return null;

			SeletCaseData form = (SeletCaseData)m_arForm[0];

			if (DlgSelectCase.Instance.CurrentData == form &&
				DlgSelectCase.Instance.Visible == true)
				return null;

			try
			{
				CloseAllDialog();

                if( DlgSelectCase.Instance.IsDisposed == true )
                {
                    DlgSelectCase.CreateDialog();
                }

				FormMain.Instance.Invoke((MethodInvoker)delegate
				{
					DlgSelectCase.Instance.CurrentData = form;
					DlgSelectCase.Instance.StartPosition = FormStartPosition.Manual;
					DlgSelectCase.Instance.Location = PageBackstageHome.Instance.SelectCaseDetectPosition();
					DlgSelectCase.Instance.TopMost = true;


                    try
                    {
                        if (FormFrame.Instance.WindowState == FormWindowState.Minimized)
                        {
                            FormFrame.Instance.WindowState = FormWindowState.Maximized;                            
                        }
                       
                        FormFrame.Instance.BringToFront();

                        if (DlgSelectCase.Instance.Visible == true)
                            DlgSelectCase.Instance.Visible = false;

                        DlgSelectCase.Instance.Show(FormFrame.Instance);
                        DlgSelectCase.Instance.Visible = true;
                        DlgSelectCase.Instance.BringToFront();
                                                
                    }
                    catch(Exception e)
                    {
                        System.Diagnostics.Trace.WriteLine(e.Message);
                        System.Diagnostics.Trace.WriteLine(e.StackTrace);
                    }
                    
				});
			}
			catch (System.Exception ex)
			{
                System.Diagnostics.Trace.WriteLine(ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
			}

			return form;
		}

		public void AddDialog(SeletCaseData form)
		{
			if (form == null)
				return;

			SeletCaseData form2 = FindForm(form.SensorHistoryID);
			if (form2 == null)
				m_arForm.Add(form);
		}

		public void AddDialogFirst(SeletCaseData form)
		{
			SeletCaseData form2 = FindForm(form.SensorHistoryID);
			if (form2 == null)
				m_arForm.Insert(0, form);
			else
			{
				m_arForm.Remove(form2);
				m_arForm.Insert(0, form);
			}
		}

		public void RemoveDialog(int nHistoryID, int nSensorID)
		{
			try
			{
				SeletCaseData form = null;
				foreach (SeletCaseData dForm in m_arForm)
				{
					if (dForm.SensorHistoryID == nHistoryID && dForm.Sensor.ID == nSensorID)
					{
						form = dForm;
					}
				}
				if (form != null)
				{
					RemoveDialog(form, true);
				}
			}
			catch (System.Exception)
			{
			}
		}

		public void RemoveDialog(SeletCaseData data, bool bRemoveOnly = true)
		{
			SeletCaseData data2 = FindForm(data.SensorHistoryID);
			m_arForm.Remove(data2);

			try
			{
				if (bRemoveOnly == false)
				{
					FormMain.Instance.Invoke((MethodInvoker)delegate
					{
						//if (!FormMain.Instance.ShowEquipZoneCCTV)
						{
							SeletCaseData form2 = (SeletCaseData)(ShowDialogNext());
							if (form2 != null)
							{
								int nID = form2.SensorHistoryID;
								int nSensorID = form2.Sensor.ID;
								FormMain.Instance.SelectSensorDetectProcess(nID, nSensorID);
							}
						}
					});
				}
				//if( ! form.IsDisposed)
				//    form.Dispose();
			}
			catch (System.Exception)
			{
			}
		}

		public void CloseAllDialog()
		{
			DlgSelectCase.Instance.HideDialog();
		}
	}

	public class SeletCaseData
	{
		public SeletCaseData(ProcessType type, ISensor sensor, int nSensorHistoryID, bool bShowOpenSOP, DateTime detectTime)
		{
            m_type = type;
			m_Sensor = sensor;
			SensorHistoryID = nSensorHistoryID;
            m_bShowOpenSOP = bShowOpenSOP;
            m_DetectTime = detectTime;
		}

        public SeletCaseData(ProcessType type, ISensorTooltipOwner view, ISensor sensor, int nSensorHistoryID, bool bShowOpenSOP, DateTime detectTime)
		{
            m_type = type;
			m_Sensor = sensor;
			SensorHistoryID = nSensorHistoryID;
			ViewOwner = view;
            m_bShowOpenSOP = bShowOpenSOP;
            m_DetectTime = detectTime;
		}

        private DateTime m_DetectTime;
        public DateTime DetectTime
        {
            get { return m_DetectTime; }
            set { m_DetectTime = value; }
        }

		private POI m_POI = null;

		public POI POI
		{
			get { return m_POI; }
			set { m_POI = value; }
		}

		private ISensor m_Sensor = null;

		public ISensor Sensor
		{
			get { return m_Sensor; }
			set { m_Sensor = value; }
		}

		private int m_nSensorHistoryID = -1;

		public int SensorHistoryID
		{
			get { return m_nSensorHistoryID; }
			set { m_nSensorHistoryID = value; }
		}

        private ISensorTooltipOwner m_viewOwner = null;

        public ISensorTooltipOwner ViewOwner
		{
			get { return m_viewOwner; }
			set { m_viewOwner = value; }
		}

        private bool m_bShowOpenSOP = false;
        public bool ShowOpenSOP
        {
            get { return m_bShowOpenSOP; }
            set { m_bShowOpenSOP = value; }
        }

        private ProcessType m_type;
        public ProcessType ProcessType
        {
            get { return m_type; }
            set { m_type = value; }
        }

	}
}