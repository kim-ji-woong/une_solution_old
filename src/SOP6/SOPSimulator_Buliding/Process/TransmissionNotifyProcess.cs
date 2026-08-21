using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Sections;
using System.Diagnostics;

using UnE.SOP;
using UnE.SOP.Process;
using UnE.SOP.Workstate;

using UnE.SOP.TTS;


namespace SOPMonitoringSystem
{
    namespace Process
    {
        class TransmissionNotifyProcess : ProcessSectionIF
        {
            private IAnnounceMessage m_dialogForm = null;
            private SectionState mSectionState = null;

            private int nActionStepId = -1;
            private bool bVirtualMode = true;
            private string szPositionName = "사무실";
            private bool hasPosition = false;
            private string szFullPath = "";
            private string szSopName = "";

            public string Caller
            {
                get { return WebDBManager.SMSCaller; }
            }  

            public int ActionStepID
            {
                get { return nActionStepId; }
                set { nActionStepId = value; }
            }
            public bool VirtualMode
            {
                get { return bVirtualMode; }
                set { bVirtualMode = value; }
            }
            public string PositionName
            {
                get { return szPositionName; }
                set { szPositionName = value; }
            }            
            public bool HasPosition
            {
                get { return hasPosition; }
                set { hasPosition = value; }
            }            
            public string FullPath
            {
                get { return szFullPath; }
                set { szFullPath = value; }
            }

            private ArrayList mCallList = null;
            public System.Collections.ArrayList CallList
            {
                get { return mCallList; }
                set { mCallList = value; }
            }
            private string szTime = "";

            private ArrayList mExCallList = null;
            public ArrayList ExCallList
            {
                get { return mExCallList; }
                set { mExCallList = value; }
            }
            private ArrayList mExFaxList = null;
            public ArrayList ExFaxList
            {
                get { return mExFaxList; }
                set { mExFaxList = value; }
            }

            private WorkFlow m_workFlow = null;

            public TransmissionNotifyProcess(SectionState state)
            {
                mSectionState = state;
                WorkFlow work = mSectionState.Parent;
                m_workFlow = work;
                hasPosition = work.Option.HasPosition;
                szPositionName = work.Option.PositionName;
                szSopName = work.szSOPName;
                mCallList = FormSOP.Instance.GetAllMemberPhoneNumber();
                szTime = GetTime();

                mExCallList = new ArrayList();
                mExFaxList = new ArrayList();
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
                    tag1 = ("모의훈련 상황입니다. 현재시각 ");
                }
                else
                {
                    tag1 = ("현재시각 ");
                }

				string szSOP = szSopName.Replace('\\', (char)0x06);
                string tag2 = ("이 시작되었습니다.");
                string szMessage = "";
                if (HasPosition == true)
                {
                    string tag3 = (" 발생 위치는");
                    string tag4 = ("입니다.");
                    szMessage = tag1 + szTime + szSOP + tag2 + tag3 + szPositionName + tag4;
                }
                else
                {
                    szMessage = tag1 + szTime + szSOP + tag2;
                }
                return szMessage;
            }

            public override void Progress()
            {
                Sections.SectionDataTransmission data = (SectionDataTransmission)mSectionState.Section.Data;
                if (data == null)
                    return;

                bool isVirtual = (mSectionState.Parent.RunMode == WorkFlowMode.VIRTUAL ? true : false);
                VirtualMode = isVirtual;
                string szPoistion = mSectionState.Parent.Option.PositionName;
                szFullPath = FormSOP.Instance.GetActionStepPath(mSectionState.Parent.ActionStepID);
				char[] seperators = { '/', '\\', (char)0x06 };
                string[] arPath = szFullPath.Split(seperators);
                if (arPath.Length < 2)
                    return;


                mExCallList.Clear();
                mExFaxList.Clear();
                ArrayList reciverTeam = data.DataExternal.SMSReceivers;
                foreach (ExternalTeamData tData in reciverTeam)
                {
                    Data_ExternalTeam dataEx = FormSOP.Instance.SOPManager.GetExternalTeam(tData.TeamID);
                    if (dataEx == null)
                    {
                        Data_ExternalTeam dataUsr = FormSOP.Instance.SOPManager.GetUserDefinedTeam(tData.TeamID);
                        if (dataUsr == null)
                            continue;
                        else
                        {
                            mExFaxList.Add(dataUsr.FaxNumber);
                            mExCallList.Add(dataUsr.PhoneNumber);
                        }
                    }
                    else
                    {
                        mExFaxList.Add(dataEx.FaxNumber);
                        mExCallList.Add(dataEx.PhoneNumber);
                    }
                }

				FormSOP.Instance.Invoke((MethodInvoker)delegate
				{
					FormSOP.Instance.GetPageHome().DiableShowPanels();
				});

                base.Progress();

                IAnnounceMessage dialogForm = null;
                bool bCreate = false;
                
                if (arPath[0] == "자연재해")
                {
                    if (arPath[1] == "태풍")
                    {
                        dialogForm = new PopupTyphoon(isVirtual);
                        ((PopupTyphoon)dialogForm).SetData(mSectionState.Section);
                        bCreate = true;
                    }
                    else if (arPath[1] == "지진")
                    {
                        dialogForm = new PopupEarthquake(isVirtual);
                        ((PopupEarthquake)dialogForm).SetData(mSectionState.Section);
                        bCreate = true;
                    }
                    else if (arPath[1] == "폭설")
                    {
                        dialogForm = new PopupSnowfall(isVirtual);
                        ((PopupSnowfall)dialogForm).SetData(mSectionState.Section);
                        bCreate = true;
                    }
                }
				else if (arPath[0] == "태풍")
				{
					dialogForm = new PopupTyphoon(isVirtual);
					((PopupTyphoon)dialogForm).SetData(mSectionState.Section);
					bCreate = true;
				}
				else if (arPath[0] == "화재" || arPath[0] == "유출사고")
				{
					dialogForm = new PopupFire(isVirtual, szPoistion);
					((PopupFire)dialogForm).SetData(mSectionState.Section);
					bCreate = true;
				}
                
                if( bCreate == false)
                {
                    dialogForm = new PopupGeneral(isVirtual);
                    ((PopupGeneral)dialogForm).SetData(mSectionState.Section);
                }
        
                m_dialogForm = dialogForm;

                if (m_workFlow != null)
                {
                    string strLocation = m_workFlow.Option.LastPosition == null ? "[재난발생위치]" : m_workFlow.Option.LastPosition.PoistionName;
                    dialogForm.SystemMessage = ParseSpecialMessage(MakeMessage(), m_workFlow.Option.DetectTime.Data, strLocation, !isVirtual, FormSOP.Instance.IsNormal, m_workFlow.Option.AlarmMessage, m_workFlow.Option.LastPosition);
                    m_dialogForm.SenarioMessage = ParseSpecialMessage(data.DataInternal.BroadcastMessage, m_workFlow.Option.DetectTime.Data, strLocation, !isVirtual, FormSOP.Instance.IsNormal, m_workFlow.Option.AlarmMessage, m_workFlow.Option.LastPosition);
                }
                else
                {
                    dialogForm.SystemMessage = MakeMessage();
                    m_dialogForm.SenarioMessage = data.DataInternal.BroadcastMessage;
                }

				Form form = (Form)dialogForm;
				FormSOP.Instance.Invoke((MethodInvoker)delegate
				{
					form.TopMost = true;
					form.BringToFront();
				});

                if (((Form)dialogForm).ShowDialog() == DialogResult.OK)
                {
             
                }
                else
                {
                    mSectionState.Cancel();
                    nState = ProcessSectionState.END;
                }

				FormSOP.Instance.Invoke((MethodInvoker)delegate
				{
					FormSOP.Instance.GetPageHome().ResotreShowPanels();
				});
            }

            public override void Dispose()
            {
                base.Dispose();
            }
        
            public override void StartBrodcast()
            {
                Sections.SectionDataTransmission data = (SectionDataTransmission)mSectionState.Section.Data;
                if (data == null)
                    return;
                
                string szMessage = m_dialogForm.Message;
                if (m_dialogForm.UseSystemMessage == true)
                {
                    szMessage = MakeMessage();
                }
              
                if (szMessage != null && szMessage != "")
                {
                    if (data.DataInternal.UseBroadcast)
                    {
                        int nCount = m_dialogForm.Count;
                        TTSManager.Instance.AddSpeech(szMessage.Replace('/', ','), nCount, m_dialogForm.UseSiren);
                    }
                    if (data.DataInternal.UseMobileApp)
                        SendMessage(mCallList, Caller, szMessage);
                }
                
                if (data.DataInternal.UsePopupMessage)
                {
                    // doNothing
                }

                if (data.DataExternal.UseSMS && (mExCallList.Count > 0))
                {
					string szMessage2 = "";
					Type dialogType = m_dialogForm.GetType();
					if( dialogType == typeof(PopupTyphoon))
					{
						szMessage2 = ((PopupTyphoon)m_dialogForm).GetMessage();
					}
					else if(dialogType == typeof(PopupEarthquake))
					{
						szMessage2 = ((PopupEarthquake)m_dialogForm).GetMessage();
					}
					else if(dialogType == typeof(PopupSnowfall))
					{
						szMessage2 = ((PopupSnowfall)m_dialogForm).GetMessage();
					}
					else if(dialogType == typeof(PopupFire))
					{
						szMessage2 = ((PopupFire)m_dialogForm).GetMessage();
					}
					else if (dialogType == typeof(PopupSnowfall))
					{
						szMessage2 = ((PopupSnowfall)m_dialogForm).GetMessage();
					}
               
                    SendMessage(mExCallList, Caller, szMessage2);
                }

                if (data.DataExternal.UseFax && (mExFaxList.Count > 0))
                {
                    // do nothing
                }

                //mSectionState.Complete();
                //nState = ProcessState.END;
            }

        }
    }
}
