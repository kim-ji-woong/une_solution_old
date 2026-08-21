using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using SOPMonitoringSystem.Popup;

//libSection
using Sections;
//libSOP
using UnE.SOP;
using UnE.SOP.Tree;
using UnE.SOP.History;
using UnE.SOP.Log;
using UnE.SOP.Data;
using UnE.SOP.Sections;
using UnE.SOP.Workstate;
using UnE.SOP.Process;

namespace SOPMonitoringSystem
{
	namespace Process
	{
        class WorkflowEndNotifyProcess : ProcessSectionIF
		{
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

			private string szPositionName = "사무실";
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

			public WorkflowEndNotifyProcess()
			{				
				szTime = GetTime();
			}

			public override void Progress()
			{
				base.Progress();

                if (m_useSMS == true)
                {
                    bool bSend = true;
                    if( UnE.SOP.ProxySOP.Instance.SiteID == 2 )
                    {
                        DateTime dtNow = DateTime.Now;
                        DateTime dtTarget = new DateTime(2017, 11, 3);
                        if (dtNow < dtTarget)
                            bSend = false;
                    }

                    if( bSend == true)
                    {
                        ArrayList arrCallList = mCallList.Clone() as ArrayList;
                        arrCallList = ControlTeamEditor.VaildMemberPhoneNumber.IsVaildPhoneNumber(arrCallList, ProxySOP.Instance.DBManager);

                        string strCaller = FormSOP.Instance.GetDefaultCallerPhoneNumber();

                        if (strCaller == null || strCaller.Length == 0)
                            strCaller = Caller;

                        SendMessage(arrCallList, strCaller, MakeMessage());
                    }
                    
                }

				nState = ProcessSectionState.END;		
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
                    tag1 = ("[모의훈련종료]현재시각");
                }
                else
                {
                    tag1 = ("[실제상황종료]현재시각");
                }
                
                int nIdx = szSopName.IndexOf("\\");
                string szTemp = "";
                if (nIdx != -1)
                {
                    szTemp = szSopName.Substring(nIdx + 1);
                    nIdx = szTemp.LastIndexOf("\\");
                    if( nIdx != -1)
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
