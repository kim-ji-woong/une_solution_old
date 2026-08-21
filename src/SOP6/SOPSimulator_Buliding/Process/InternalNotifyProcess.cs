using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sections;
using System.Diagnostics;
using System.Drawing;


using UnE.SOP;
using UnE.SOP.Process;
using UnE.SOP.Workstate;
using UnE.SOP.TTS;



namespace SOPMonitoringSystem
{
    namespace Process
    {
        public class InternalNotifyProcess : ProcessSectionIF
        {
            IAnnounceMessage m_dialogForm = null;
            private SectionState mSectionState = null;

            //private string szCaller = "027144133";
            private int nActionStepId = -1;
            private bool bVirtualMode = true;
            private string szPositionName = "사무실";
            private bool hasPosition = false;
            private string szFullPath = "";
            private string szSopName = "";

            public string Caller
            {
                get { return WebDBManager.SMSCaller; }
                //get { return szCaller; }
                //set { szCaller = value; }
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

            private WorkFlow m_workFlow = null;


            //private string m_szBroadcastMsg = "";

            public InternalNotifyProcess(SectionState state)
            {
                mSectionState = state;
                WorkFlow work = mSectionState.Parent;
                m_workFlow = work;
                hasPosition = work.Option.HasPosition;
                szPositionName = work.Option.PositionName;
                szSopName = work.szSOPName;
                mCallList = FormSOP.Instance.GetAllMemberPhoneNumber();

                if (FormSOP.Instance.SmsExternalCompanyMemberOn)
                    FormSOP.Instance.AddExternalCompanyMemberPhoneNumbers(mCallList);

                szTime = GetTime();
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
                SectionDataInternal data = (SectionDataInternal)mSectionState.Section.Data;
                if (data == null)
                    return;

                bool isVirtual = (mSectionState.Parent.RunMode == WorkFlowMode.VIRTUAL ? true : false);
                VirtualMode = isVirtual;
                string szPoistion = mSectionState.Parent.Option.PositionName;
                szFullPath = FormSOP.Instance.GetActionStepPath(mSectionState.Parent.ActionStepID);
				char[] seperators = { '/', '\\', (char)0x06 };
                string[] arPath = szFullPath.Split(seperators);
                if( arPath.Length < 2 )
                    return;

				FormSOP.Instance.Invoke((MethodInvoker)delegate
				{
					FormSOP.Instance.GetPageHome().DiableShowPanels();
				});
				       
                base.Progress();

                m_dialogForm = GetAnnounceMessage(mSectionState.Section, mSectionState.Parent);               

                if (m_workFlow != null)
                {
                    string strLocation = m_workFlow.Option.LastPosition == null ? "[재난발생위치]" : m_workFlow.Option.LastPosition.PoistionName;
                    m_dialogForm.SenarioMessage = ParseSpecialMessage(data.BroadcastMessage, m_workFlow.Option.DetectTime.Data, strLocation, !isVirtual, FormSOP.Instance.IsNormal, m_workFlow.Option.AlarmMessage, m_workFlow.Option.LastPosition);
                    m_dialogForm.SystemMessage = ParseSpecialMessage(MakeMessage(), m_workFlow.Option.DetectTime.Data, strLocation, !isVirtual, FormSOP.Instance.IsNormal, m_workFlow.Option.AlarmMessage, m_workFlow.Option.LastPosition);
                }
                else
                {
                    m_dialogForm.SenarioMessage = data.BroadcastMessage;
                    m_dialogForm.SystemMessage = MakeMessage();
                }
               

				Form form = (Form)m_dialogForm;
				FormSOP.Instance.Invoke((MethodInvoker)delegate
				{
					form.TopMost = true;
					form.BringToFront();					
				});

                Point ptCurrent = ((Form)m_dialogForm).Location;
                ((Form)m_dialogForm).StartPosition = FormStartPosition.Manual;
                ((Form)m_dialogForm).Location = new Point(FormSOP.Instance.Location.X + ptCurrent.X, FormSOP.Instance.Location.Y + ptCurrent.Y);

                //if (((Form)m_dialogForm).ShowDialog() == DialogResult.OK)
                //{

                //}
                //else
                //{
                //    mSectionState.Cancel();
                //    nState = ProcessSectionState.END;
                //}

				FormSOP.Instance.Invoke((MethodInvoker)delegate
				{
					FormSOP.Instance.GetPageHome().ResotreShowPanels();
				});
            }

            public static IAnnounceMessage GetAnnounceMessage(Sections.Section section, WorkFlow workFlow = null)
            {
                PanelSection panel = section.GetParent();

                if (panel == null)
                    return null;

                UnE.SOP.Sections.SectionTabPage page = (UnE.SOP.Sections.SectionTabPage)panel.Parent;

                string szFullPath = FormSOP.Instance.GetActionStepPath(page.ActionStepID);
                char[] seperators = { '/', '\\', (char)0x06 };
                string[] arPath = szFullPath.Split(seperators);
                if (arPath.Length < 2)
                    return null;

                IAnnounceMessage message = null;

                bool bCreate = false;
                if (arPath[0] == "자연재해")
                {
                    if (arPath[1] == "태풍")
                    {
                        message = new PopupTyphoon(page.VirtualMode);
                        ((PopupTyphoon)message).SetData(section);

                        bCreate = true;
                    }
                    else if (arPath[1] == "지진")
                    {
                        message = new PopupEarthquake(page.VirtualMode);
                        ((PopupEarthquake)message).SetData(section);
                        bCreate = true;
                    }
                    else if (arPath[1] == "폭설")
                    {
                        message = new PopupSnowfall(page.VirtualMode);
                        ((PopupSnowfall)message).SetData(section);
                        bCreate = true;
                    }
                }
                else if (arPath[0] == "태풍")
                {
                    message = new PopupTyphoon(page.VirtualMode);
                    ((PopupTyphoon)message).SetData(section);
                    bCreate = true;
                }
                else if (arPath[0] == "화재" || arPath[0] == "유출사고")
                {
                    string szPosition = workFlow == null || workFlow.Option == null ? "[재난발생위치]" : workFlow.Option.PositionName;
                    message = new PopupFire(page.VirtualMode, szPosition);
                    ((PopupFire)message).SetData(section);
                    bCreate = true;
                }

                if (bCreate == false)
                {
                    message = new PopupGeneral(page.VirtualMode);
                    ((PopupGeneral)message).SetData(section);
                }

                return message;
            }

            public override void Dispose()
            {
                base.Dispose();
            }

            public override void StartBrodcast()
            {
                Sections.SectionDataInternal data = (SectionDataInternal)mSectionState.Section.Data;
                if (data == null)
                    return;

                // 방송 메시지가 정의되지 않은 재난
                if (m_dialogForm == null)
                {
                    if (data.UseMobileApp)
                    {
                        string szMsg = MakeMessage();
                        SendMessage(mCallList, Caller, szMsg);
                    }

                    if (data.UsePopupMessage)
                    {
                        // doNothing
                    }
                    return;
                }

                string szMessage = data.BroadcastMessage;
                if (szMessage == null || szMessage == "")
                    return;

                bool useSiren = data.UseSiren;
                int nCount = data.RepeatCount;
                
                if (data.UseBroadcast)
                {
                    TTSManager.Instance.AddSpeech(szMessage.Replace('/', ','), nCount, useSiren);
                }

                if (data.UseMobileApp && FormSOP.Instance.SMSOn)
                    SendMessage(mCallList, Caller, szMessage);

                if (data.UsePopupMessage)
                {
                    // doNothing
                }
            }
        }
    }
}