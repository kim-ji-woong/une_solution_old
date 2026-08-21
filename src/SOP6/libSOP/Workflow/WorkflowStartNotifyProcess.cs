using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using SOPMonitoringSystem.Popup;
using System.Windows.Forms;
using Sections;

namespace SOPMonitoringSystem
{  

	namespace Process
	{
		class WorkFlowStartNotifyProcess : ProcessIF
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

			private string szPositionName = "";
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

			private string szSopName = "";
			public string SOPName
			{
				get { return szSopName; }
				set { szSopName = value; }
			}

			private ArrayList mCallList = null;
			public System.Collections.ArrayList CallList
			{
				get { return mCallList; }
				set { mCallList = value; }
			}

			private DateTime m_dtDetectTime;
			public System.DateTime DetectTime
			{
				get { return m_dtDetectTime; }
				set { m_dtDetectTime = value; }
			}

			public string Caller
			{
				get { return WebDBManager.SMSCaller; }
			}

			private string szTime = "";

			private bool m_useSMS = false;
			public bool UseSMS
			{
				get { return m_useSMS; }
				set { m_useSMS = value; }
			}
			public WorkFlowStartNotifyProcess()
			{
				popup = new PopupStartEvent();
				popup2 = new PopupWorkflowOption();
				szTime = GetTime();

				m_dtDetectTime = DateTime.Now;
			}

			public override void Progress()
			{
				if( OnPreProcess != null)
				{
					OnPreProcess(this, new ProcessEventArgs());
				}
								
				// Section 패널을 사용 불가능 상태로 변경				
				FormMain.Instance.Invoke((MethodInvoker)delegate
				{
					FormMain.Instance.GetPageHome().DiableShowPanels();
				});

				base.Progress();

				bool bCancel = false;
				if (hasPosition == true)
				{
					string szSOP = szSopName.Replace('\\', (char)0x06);
					popup.Text = "시작 이벤트 옵션 - [" + szSOP + "]";
					popup.PositionName = PositionName;
					popup.UseSMS = m_useSMS;
					if (m_bNoPopup == true)
					{
						popup.btnRunClick(null, null);
						PositionName = popup.PositionName;
						m_useSMS = popup.UseSMS;
						popup.DialogResult = System.Windows.Forms.DialogResult.OK;
					}
					else
					{
						if (popup.ShowDialog() == System.Windows.Forms.DialogResult.OK)
						{
							PositionName = popup.PositionName;
							m_useSMS = popup.UseSMS;
                            m_dtDetectTime = popup.DetectTime;
						}
						else
						{
							m_useSMS = false;
							bCancel = true;
						}
					}
					
				}
				else
				{
					if (popup2 != null)
					{
						string szSOP = szSopName.Replace('\\', (char)0x06);
						popup2.Text = "시작 이벤트 옵션 - [" + szSOP + "]";
						popup2.UseSmsMessage = m_useSMS;
						if (popup2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
						{
							m_useSMS = popup2.UseSmsMessage;
                            m_dtDetectTime = popup2.DetectTime;
						}
						else
						{
							m_useSMS = false;
							bCancel = true;
						}
						
					}					
				}

				if (OnPostProcess != null)
				{
					Object[] param = { this, new ProcessEventArgs() };
					FormMain.Instance.Invoke(OnPostProcess, param);
				}

				if (bCancel == false && m_useSMS == true)
				{
					string message = MakeMessage();
					message = FormMain.Instance.DBManager.SMS_ADD_TEXT + message;
					SendMessage(mCallList, Caller, message);
				}
				nState = ProcessState.END;

				FormMain.Instance.Invoke((MethodInvoker)delegate
				{
					FormMain.Instance.GetPageHome().ResotreShowPanels();
				});

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
				if (HasPosition == true)
				{
					string tag3 = ("\n[발생 위치]");
					//string tag4 = ("입니다.");
					szMessage = tag1 + szTime + "," + szSOP + tag3 + szPositionName;// +tag4; 
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
					int nSleepTime = ProcessManager.Instance.SleepTime + 10;
					System.Threading.Thread.Sleep(nSleepTime);
				}
				mThread.Join();
			}

		}
	}    
}
