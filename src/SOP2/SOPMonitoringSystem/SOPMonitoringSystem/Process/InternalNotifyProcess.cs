using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sections;
using System.Diagnostics;
using System.Drawing;



namespace SOPMonitoringSystem
{
    namespace Process
    {
        public class InternalNotifyProcess : ProcessIF
        {
            AnnounceMessage m_dialogForm = null;
            private Sections.SectionState mSectionState = null;

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

            public InternalNotifyProcess(Sections.SectionState state)
            {
                mSectionState = state;
                WorkFlow work = mSectionState.Parent;
                m_workFlow = work;
                hasPosition = work.HasPosition;
                szPositionName = work.Position;
                szSopName = work.szSOPName;
                mCallList = FormMain.Instance.GetAllMemberPhoneNumber();
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
                string szPoistion = mSectionState.Parent.Position;
                szFullPath = FormMain.Instance.GetActionStepPath(mSectionState.Parent.ActionStepID);
				char[] seperators = { '/', '\\', (char)0x06 };
                string[] arPath = szFullPath.Split(seperators);
                if( arPath.Length < 2 )
                    return;

				FormMain.Instance.Invoke((MethodInvoker)delegate
				{
					FormMain.Instance.GetPageHome().DiableShowPanels();
				});
				       
                base.Progress();

                //AnnounceMessage dialogForm = null;
                bool bCreate = false;
                if (arPath[0] == "자연재해")
                {
                    if (arPath[1] == "태풍")
                    {
                        m_dialogForm = new PopupTyphoon(isVirtual);
                        ((PopupTyphoon)m_dialogForm).SetData(mSectionState);
						
                        bCreate = true;
                    }
                    else if (arPath[1] == "지진")
                    {
                        m_dialogForm = new PopupEarthquake(isVirtual);
                        ((PopupEarthquake)m_dialogForm).SetData(mSectionState);
                        bCreate = true;
                    }
                    else if (arPath[1] == "폭설")
                    {
                        m_dialogForm = new PopupSnowfall(isVirtual);
                        ((PopupSnowfall)m_dialogForm).SetData(mSectionState);
                        bCreate = true;
                    }
                }
				else if (arPath[0] == "태풍")
				{
					m_dialogForm = new PopupTyphoon(isVirtual);
					((PopupTyphoon)m_dialogForm).SetData(mSectionState);
					bCreate = true;
				}
                else if (arPath[0] == "화재" || arPath[0] == "유출사고")
                {
                    m_dialogForm = new PopupFire(isVirtual, szPoistion);
                    ((PopupFire)m_dialogForm).SetData(mSectionState);
                    bCreate = true;
                }

                if (bCreate == false)
                {
                    m_dialogForm = new PopupGeneral(isVirtual);
                    ((PopupGeneral)m_dialogForm).SetData(mSectionState);

                    //return;
                }

                if (m_workFlow != null)
                {
                    string strLocation = m_workFlow.LastPosition == null ? "[재난발생위치]" : m_workFlow.LastPosition.PoistionName;
                    m_dialogForm.SenarioMessage = ParseSpecialMessage(data.BroadcastMessage, m_workFlow.DetectTime, strLocation, !isVirtual, FormMain.Instance.IsNormal);
                    m_dialogForm.SystemMessage = ParseSpecialMessage(MakeMessage(), m_workFlow.DetectTime, strLocation, !isVirtual, FormMain.Instance.IsNormal);
                }
                else
                {
                    m_dialogForm.SenarioMessage = data.BroadcastMessage;
                    m_dialogForm.SystemMessage = MakeMessage();
                }
               

				Form form = (Form)m_dialogForm;
				FormMain.Instance.Invoke((MethodInvoker)delegate
				{
					form.TopMost = true;
					form.BringToFront();					
				});

				if (((Form)m_dialogForm).ShowDialog() == DialogResult.OK)
				{

				}
				else
				{
					mSectionState.Cancel();
					nState = ProcessState.END;
				}

				FormMain.Instance.Invoke((MethodInvoker)delegate
				{
					FormMain.Instance.GetPageHome().ResotreShowPanels();
				});
            }

            public override void Dispose()
            {
                base.Dispose();
            }

            public void StartBrodcast()
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

                string szMessage = m_dialogForm.Message;
                if (m_dialogForm.UseSystemMessage == true)
                {
                    szMessage = MakeMessage();
                }

                if (szMessage == null || szMessage == "")
                    return;

                bool useSiren = m_dialogForm.UseSiren;

                int nCount = m_dialogForm.Count;

                // debug
                //data.UseBroadcast = true;

                if (data.UseBroadcast)
                {
                    TTSManager.Instance.AddSpeech(szMessage.Replace('/', ','), nCount, useSiren);
                }

                if (data.UseMobileApp)
                    SendMessage(mCallList, Caller, szMessage);

                if (data.UsePopupMessage)
                {
                    // doNothing
                }
            }
        }
    }
}