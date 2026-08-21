using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using SOPMonitoringSystem.Popup;
using System.Windows.Forms;

using Sections;
using UnE.SOP.Process;
using UnE.SOP;

namespace SOPMonitoringSystem
{  

	namespace Process
	{
		class WorkFlowStartNotifyProcess : ProcessSectionIF
		{
			public override event PreProcessEvent OnPreProcess;
			public override event PostProcessEvent OnPostProcess;
		 
			private PopupStartEvent popup = null;
			public SOPMonitoringSystem.Popup.PopupStartEvent Popup
			{
				get { return popup; }
				set { popup = value; }
			}

			private bool m_bNoPopup = false;
			public bool NoPopup
			{
				get { return m_bNoPopup; }
				set { m_bNoPopup = value; }
			}

			private PopupWorkflowOption popup2 = null;
			public SOPMonitoringSystem.Popup.PopupWorkflowOption PopupOption
			{
				get { return popup2; }
				set { popup2 = value; }
			}

			private bool bVirtualMode = true;
			public bool VirtualMode
			{
				get { return bVirtualMode; }
				set { bVirtualMode = value; }
			}

			private int    nActionStepID = -1;
			public int ActionStepID
			{
				get { return nActionStepID; }
				set { nActionStepID = value; }
			}

			/*private string szPositionName = "";
			public string PositionName
			{
				get { return szPositionName; }
				set { szPositionName = value; }
			}

			private bool hasPosition = false;
			public bool HasPosition
			{
				get { return hasPosition; }
				set { hasPosition = value; }
			}

            private bool m_usePSM = false;
            public bool UsePSM
            {
                get { return m_usePSM; }
                set { m_usePSM = value; }
            }

            private string m_strPSMMaterialName = "";
            public string PSMMaterialName
            {
                get { return m_strPSMMaterialName; }
                set { m_strPSMMaterialName = value; }
            }

            // 유해화학물질 누출시 대피거리(미터)
            private int m_nPSMDistance = 0;
            public int PSMDistance
            {
                get { return m_nPSMDistance; }
                set { m_nPSMDistance = value; }
            }*/

			private string szSopName = "";
			public string SOPName
			{
				get { return szSopName; }
				set { szSopName = value; }
			}

            private string m_strCategoryName = "";
            public string CategoryName
            {
                get { return m_strCategoryName; }
                set { m_strCategoryName = value; }
            }

			private ArrayList mCallList = null;
			public System.Collections.ArrayList CallList
			{
				get { return mCallList; }
				set { mCallList = value; }
			}

			/*private DateTime m_dtDetectTime;
			public System.DateTime DetectTime
			{
				get { return m_dtDetectTime; }
				set { m_dtDetectTime = value; }
			}*/

            private DialogResult m_DialgResult = DialogResult.None;
            public DialogResult DialogResult
            {
                get { return m_DialgResult; }
                set { m_DialgResult = value; }
            }

			public string Caller
			{
				get { return WebDBManager.SMSCaller; }
			}

			private string szTime = "";

			/*private bool m_useSMS = false;
			public bool UseSMS
			{
				get { return m_useSMS; }
				set { m_useSMS = value; }
			}

            private List<Shelter> m_shelters = null;
            public List<Shelter> Shelters
            {
                get { return m_shelters; }
                set { m_shelters = value; }
            }

            private bool m_useAmountSnowfall = false;
            public bool UseAmountSnowfall
            {
                get { return m_useAmountSnowfall; }
                set { m_useAmountSnowfall = value; }
            }

            // "5cm 이상"등과 같이 자유롭게 사용할 수 있도록 하기 위하여
            // 숫자 대신 문자열을 사용한다.
            private string m_strAmountSnowfall = "";
            public string AmountSnowfall
            {
                get { return m_strAmountSnowfall; }
                set { m_strAmountSnowfall = value; }
            }*/

            private UnE.SOP.Workstate.WorkflowOption m_option = null;
            public UnE.SOP.Workstate.WorkflowOption Option
            {
                get { return m_option; }
                set { m_option = value; }
            }

            private List<SOPParameter> m_userDefinedVariables = null;

			public WorkFlowStartNotifyProcess(string strCategoryName, string strSOPName, UnE.SOP.Sections.SectionTabPage tabPage)
			{
                LoadUsingSOPParameters(tabPage);

				popup = new PopupStartEvent(m_userDefinedVariables);
				popup2 = new PopupWorkflowOption(m_userDefinedVariables);
				szTime = GetTime();

                m_strCategoryName = strCategoryName;
                szSopName = strSOPName;
                m_option = MakeWorkflowOption(strCategoryName, GetSubCategryName(strSOPName));

                m_option.DetectTime = new DBUtility.VariousData<DateTime>(DateTime.Now);
				//m_dtDetectTime = DateTime.Now;
			}

            private void LoadUsingSOPParameters(UnE.SOP.Sections.SectionTabPage tabPage)
            {
                if (tabPage == null || tabPage.Tag == null)
                    return;

                if (tabPage.Tag is Data_ActionStep)
                {
                    Data_ActionStep actionStep = (Data_ActionStep)tabPage.Tag;

                    if (actionStep.UserDefinedConfig == null)
                        return;

                    Dictionary<string, SOPParameter> dicVariables = new Dictionary<string, SOPParameter>();

                    foreach (Control ctrl in tabPage.Controls)
                    {
                        if (ctrl is PanelSectionEx)
                        {
                            PanelSectionEx panel = (PanelSectionEx)ctrl;

                            foreach (Section section in panel.Sections)
                            {
                                if (section is SectionProcess)
                                {
                                    LoadUsingProcessParameters((SectionProcess)section, dicVariables, actionStep.UserDefinedConfig.Variables);
                                }
                                else if (section is SectionDecision)
                                {
                                    LoadUsingDecisionParameters((SectionDecision)section, dicVariables, actionStep.UserDefinedConfig.Variables);
                                }
                                else if (section is SectionInternal)
                                {
                                    ParseSectionParameters(((SectionDataInternal)section.Data).BroadcastMessage, dicVariables, actionStep.UserDefinedConfig.Variables);
                                }
                            }
                        }
                    }

                    m_userDefinedVariables = dicVariables.Values.ToList();
                }
            }

            private void LoadUsingDecisionParameters(SectionDecision section, Dictionary<string, SOPParameter> dicVariables, List<SOPParameter> parameters)
            {
                if (section.Data.Expression == null || section.Data.Expression.Length == 0)
                    return;

                ParseSectionParameters(section.Data.Expression, dicVariables, parameters);
            }

            private void LoadUsingProcessParameters(SectionProcess section, Dictionary<string, SOPParameter> dicVariables, List<SOPParameter> parameters)
            {
                foreach (MissionItem item in ((SectionDataProcess)section.Data).MissionItems)
                {
                    ParseSectionParameters(item.Mission, dicVariables, parameters);
                }
            }

            private void ParseSectionParameters(string str, Dictionary<string, SOPParameter> dicVariables, List<SOPParameter> parameters)
            {
                str = str.ToLower();
                int nIndex = str.IndexOf('{');

                while (nIndex >= 0)
                {
                    int nIndex2 = str.IndexOf('}', nIndex + 1);

                    if (nIndex2 > nIndex)
                    {
                        string strVariable = str.Substring(nIndex + 1, nIndex2 - nIndex - 1).Trim();
                        AddSectionParameter(strVariable, dicVariables, parameters);
                    }

                    nIndex = str.IndexOf('{', nIndex2 + 1);
                }
            }

            private void AddSectionParameter(string strVariableName, Dictionary<string, SOPParameter> dicVariables, List<SOPParameter> parameters)
            {
                SOPParameter param = null;

                if (dicVariables.TryGetValue(strVariableName, out param))
                    return;

                // parameters에 포함되지 않은것은 System 변수들이다.
                foreach (SOPParameter parameter in parameters)
                {
                    if (string.Compare(parameter.VariableName, strVariableName, true) == 0)
                    {
                        dicVariables[strVariableName] = parameter;
                        break;
                    }
                }
            }

			public override void Progress()
			{
				if( OnPreProcess != null)
				{
					OnPreProcess(this, new ProcessSectionEventArgs());
				}
								
				// Section 패널을 사용 불가능 상태로 변경				
				FormSOP.Instance.Invoke((MethodInvoker)delegate
				{
					FormSOP.Instance.GetPageHome().DiableShowPanels();
				});

				base.Progress();

                string strSubCategory = GetSubCategryName(szSopName);
                UnE.SOP.Workstate.WorkflowOption option = m_option == null ? MakeWorkflowOption(m_strCategoryName, strSubCategory) : m_option;

				bool bCancel = false;

				if (option.HasPosition == true)
				{
                    popup.Option = option;

					string szSOP = szSopName.Replace('\\', (char)0x06);
					popup.Text = "시작 이벤트 옵션 - [" + szSOP + "]";
					//popup.PositionName = PositionName;
					//popup.UseSMS = m_useSMS;
                    //popup.UsePSM = m_usePSM;

					if (m_bNoPopup == true)
					{
						popup.btnRunClick(null, null);
						//PositionName = popup.PositionName;
                        //Shelters = popup.UsingShelters;
						//m_useSMS = popup.UseSMS;
						popup.DialogResult = System.Windows.Forms.DialogResult.OK;

                        PSMMaterial material = option is UnE.SOP.Workstate.WorkflowOptionPSM ? ((UnE.SOP.Workstate.WorkflowOptionPSM)option).PSMMaterial : null;
                        SetPSM(material);
                        //SetPSM(popup.PSMMaterial);
					}
					else
					{
                        int x = 0, y = 0;
                        UnE.SOP.Sections.SectionTabPage tabPage = null;
                        FormSOP.Instance.Invoke((MethodInvoker)delegate
                        {
                            tabPage = FormSOP.Instance.GetPageHome().GetTabPage(ActionStepID, !VirtualMode);
                            System.Drawing.Point pt = FormFrame.Instance.Location;
                            x = pt.X;
                            y = pt.Y;
                        });


                        bool bResult = false;
                        FormSOP.Instance.Invoke((MethodInvoker)delegate
                        { 
                            if (tabPage != null && tabPage.LinkedZoneName != null && tabPage.LinkedZoneName.Length > 0 && tabPage.LinkedZoneID >= 0)
                            {
                                // 연결된 화재 탐지 영역이 있더라도 Popup띄우도록 변경 2015-10-21
                                //bResult = true;
                                FormSOP.Instance.EnableOptions(false);

                                m_DialgResult = System.Windows.Forms.DialogResult.OK;

                                Zone zone = null;

                                if (!DataManager.Instance.DicZones.TryGetValue(tabPage.LinkedZoneID, out zone))
                                    zone = null;

                                HistoryDisasterPosition disasterPos = new HistoryDisasterPosition();
                                disasterPos.PoistionName = tabPage.LinkedZoneName;
                                disasterPos.BroadcastName = tabPage.LinkedZoneName;
                                disasterPos.DisasterName = "화재";

                                

                                UnE.Geometry.Vertex2D pos3D = zone == null || zone.Polygon == null ? new UnE.Geometry.Vertex2D() : zone.Polygon.CalcWeightCenter();
                                disasterPos.X = (float)pos3D.x;
                                disasterPos.Y = 0.0f;
                                disasterPos.Z = (float)pos3D.y;

                                if (zone.IsOutdoor == true)
                                    disasterPos.FloorIndex = -999.0f;
                                else
                                    disasterPos.FloorIndex = zone.Floor.FloorIndex;

                                if (zone.Building != null)
                                    disasterPos.BuildingID = zone.Building.BuildingID;
                                else
                                    disasterPos.BuildingID = "ZONE";

                                popup.DisasterName = disasterPos.DisasterName;
                                popup.AddLastHistoryDisasterPoistion(disasterPos);

                                popup.PreRun(tabPage.LinkedZoneName);

                                option.PositionName = disasterPos.PoistionName;
                                option.BroadcastPositionName = disasterPos.BroadcastName;
                                //PositionName = popup.PositionName = tabPage.LinkedZoneName;
                                popup.InputTime = tabPage.LinkedTime;
                                option.DetectTime = new DBUtility.VariousData<DateTime>(tabPage.LinkedTime);
                                //m_dtDetectTime = tabPage.LinkedTime;
                                //Shelters = popup.UsingShelters;
                                //m_useSMS = popup.UseSMS;

                                PSMMaterial material = option is UnE.SOP.Workstate.WorkflowOptionPSM ? ((UnE.SOP.Workstate.WorkflowOptionPSM)option).PSMMaterial : null;
                                SetPSM(material);
                                //SetPSM(popup.PSMMaterial);
                            }
                        });

                        if (bResult == false)
                        {
                            popup.Location = new System.Drawing.Point(x, y);
                            //popup.TopMost = true;

                            FormSOP.Instance.Invoke((MethodInvoker)delegate
                            {
                                m_DialgResult = popup.ShowDialog();
                            });

                            if (m_DialgResult == System.Windows.Forms.DialogResult.OK)
                            {
                                FormSOP.Instance.Invoke((MethodInvoker)delegate
                                {
                                    FormSOP.Instance.EnableOptions(false);
                                });
                                
                                //PositionName = popup.PositionName;
                                //Shelters = popup.UsingShelters;
                                //m_useSMS = popup.UseSMS;
                                //m_dtDetectTime = popup.DetectTime;

                                PSMMaterial material = option is UnE.SOP.Workstate.WorkflowOptionPSM ? ((UnE.SOP.Workstate.WorkflowOptionPSM)option).PSMMaterial : null;
                                SetPSM(material);
                                //SetPSM(popup.PSMMaterial);
                            }
                            else
                            {
                                option.UseSmsMessage = false;
                                //m_useSMS = false;
                                bCancel = true;
                            }
                        }
                        
					}
				}
				else
				{
                    if (!m_bNoPopup)
                    {
                        if (popup2 != null)
                        {
                            if (strSubCategory == "폭설")
                            {
                                //this.UseAmountSnowfall = true;
                            }

                            //popup2.UseAmountSnowfall = this.UseAmountSnowfall;

                            string szSOP = szSopName.Replace('\\', (char)0x06);
                            popup2.Text = "시작 이벤트 옵션 - [" + szSOP + "]";
                            //popup2.UseSmsMessage = m_useSMS;

                            // 재난위치가 없는 SOP들은 피난처를 기본으로 선택하지 않는다.
                            List<UnE.Spatial.Shelter> shelters = DataManager.Instance.LoadShelter();
                            option.UsingShelters = shelters;
                            //popup2.SetShelters(shelters, false);

                            popup2.Option = option;

                            System.Drawing.Point ptCurrent = popup2.Location;
                            popup2.StartPosition = FormStartPosition.CenterScreen;
                            popup2.Location = new System.Drawing.Point(FormFrame.Instance.Location.X + ptCurrent.X, FormFrame.Instance.Location.Y + ptCurrent.Y);

                            FormSOP.Instance.Invoke((MethodInvoker)delegate
                            {
                                m_DialgResult = popup2.ShowDialog();
                            });

                            if (m_DialgResult == System.Windows.Forms.DialogResult.OK)
                            {
                                //Shelters = popup2.UsingShelters;
                                //m_useSMS = popup2.UseSmsMessage;
                                //m_dtDetectTime = popup2.DetectTime;

                                //m_useAmountSnowfall = popup2.UseAmountSnowfall;
                                //m_strAmountSnowfall = popup2.AmountSnowfall;
                            }
                            else
                            {
                                option.UseSmsMessage = false;
                                //m_useSMS = false;
                                bCancel = true;
                            }
                        }
                    }
				}

                this.m_option = option;

				if (OnPostProcess != null)
				{
                    Object[] param = { this, new ProcessSectionEventArgs() };
					FormSOP.Instance.Invoke(OnPostProcess, param);
				}

				if (bCancel == false && m_option.UseSmsMessage == true)
				{
                    bool bSend = true;
                    if( UnE.SOP.ProxySOP.Instance.SiteID == 2 )
                    {
                        DateTime dtNow = DateTime.Now;
                        DateTime dtTarget = new DateTime(2017, 11, 3);
                        if (dtNow < dtTarget)
                            bSend = false;
                    }

                    if (bSend == true)
                    {

                        ArrayList arrCallList = mCallList.Clone() as ArrayList;
                        string message = MakeMessage();
                        message = FormSOP.Instance.DBManager.SMS_ADD_TEXT + message;

                        string strCaller = FormSOP.Instance.GetDefaultCallerPhoneNumber();

                        if (strCaller == null || strCaller.Length == 0)
                            strCaller = Caller;

                        arrCallList = ControlTeamEditor.VaildMemberPhoneNumber.IsVaildPhoneNumber(arrCallList, ProxySOP.Instance.DBManager);
                        SendMessage(arrCallList, strCaller, message);
                    }
				}
				nState = ProcessSectionState.END;

				FormSOP.Instance.Invoke((MethodInvoker)delegate
				{
					FormSOP.Instance.GetPageHome().ResotreShowPanels();
				});

			}

            public static UnE.SOP.Workstate.WorkflowOption MakeWorkflowOption(string strCategoryName, string strSubCategoryName)
            {
                UnE.SOP.Workstate.WorkflowOption option = null;

                if (strSubCategoryName == "폭설")
                    option = new UnE.SOP.Workstate.WorkflowOptionSnowFall();
                else if (strSubCategoryName == "지진")
                    option = new UnE.SOP.Workstate.WorkflowOptionEarthquake();
                else if (strCategoryName == "유출사고")
                    option = new UnE.SOP.Workstate.WorkflowOptionPSM();
                else
                    option = new UnE.SOP.Workstate.WorkflowOption();

                if (strCategoryName == "화재" || strCategoryName == "폭발" || strCategoryName == "테러")
                    option.HasPosition = true;

                return option;
            }

            private string GetSubCategryName(string strSOPName)
            {
                int nIndex = strSOPName.IndexOf('\\');

                if (nIndex < 0)
                    nIndex = strSOPName.IndexOf((char)0x06);

                if (nIndex < 0)
                    return "";

                return strSOPName.Substring(0, nIndex);
            }

            private void SetPSM(PSMMaterial material)
            {
                if (material != null && m_option != null && m_option is UnE.SOP.Workstate.WorkflowOptionPSM)
                {
                    UnE.SOP.Workstate.WorkflowOptionPSM option = (UnE.SOP.Workstate.WorkflowOptionPSM)m_option;
                    option.PSMMaterial = material;
                    //m_strPSMMaterialName = material.MaterialName;

                    if (SOPMonitoringSystem.Popup.SOPLoader.IsDayLight(DateTime.Now))
                    {
                        option.PSMDistance = material.DayDistance;
                        //m_nPSMDistance = material.DayDistance;
                    }
                    else
                    {
                        option.PSMDistance = material.NightDistance;
                        //m_nPSMDistance = material.NightDistance;
                    }
                }
            }

			private string GetTime()
			{
				DateTime dtNow = DateTime.Now;   // 현재 날짜, 시간 얻기
				string szTime = dtNow.Hour.ToString() + ("시") + dtNow.Minute.ToString() + ("분 ");
				return szTime;
			}
			private string MakeMessage()
			{
				string tag1 = "";
				if (VirtualMode == true)
				{
					tag1 = ("[모의훈련시작]현재시각");
				}
				else
				{
					tag1 = ("[실제상황시작]현재시각");
				}

				int nIdx = szSopName.IndexOf("\\");
				string szTemp = "";
				if (nIdx != -1)
				{
					szTemp = szSopName.Substring(nIdx + 1);
					nIdx = szTemp.LastIndexOf("\\");
					if (nIdx != -1)
					{
						szTemp = szTemp.Substring(0, nIdx);
					}
				}
				string szSOP = szTemp.Replace('\\', (char)0x06);
				string szMessage = "";
				if (m_option != null && m_option.HasPosition == true)
				{
					string tag3 = ("\n[발생 위치]");
					//string tag4 = ("입니다.");
                    szMessage = tag1 + szTime + "," + szSOP + tag3 + m_option.PositionName;// +tag4; 
					//szMessage = tag1 + szTime + "," + szSOP + tag3 + szPositionName;// +tag4; 
				}
				else
				{
					szMessage = tag1 + szTime + "," + szSOP;// +tag2;
				}
				return szMessage;            
			}
			//private string MakeMessage()
			//{
			//    string tag1 = "";
			//    if (VirtualMode == true)
			//    {
			//        tag1 = ("모의훈련 상황입니다. 현재시각 ");
			//    }
			//    else
			//    {
			//        tag1 = ("현재시각 ");
			//    }
				
			//    string szSOP = szSopName.Replace('\\', '/');
			//    string tag2 = ("이 시작되었습니다.");
			//    string szMessage = "";
			//    if (HasPosition == true)
			//    {
			//        string tag3 = (" 발생 위치는");
			//        string tag4 = ("입니다.");
			//        szMessage = tag1 + szTime + szSOP + tag2 + tag3 + szPositionName + tag4; 
			//    }
			//    else
			//    {
			//        szMessage = tag1 + szTime + szSOP + tag2;
			//    }
			//    return szMessage;
			//}

			public override void Dispose()
			{
				base.Dispose();
			}

			public void Wait()
			{
				while (mThread == null)
				{
					int nSleepTime = ProcessSectionManager.Instance.SleepTime + 10;
					System.Threading.Thread.Sleep(nSleepTime);
				}
				mThread.Join();
			}

		}
	}    
}
