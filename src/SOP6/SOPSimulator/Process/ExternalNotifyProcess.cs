using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sections;
using System.Diagnostics;

using UnE.SOP;
using UnE.SOP.Process;
using UnE.SOP.Workstate;


namespace SOPMonitoringSystem
{
	namespace Process
	{
		public class ExternalNotifyProcess : ProcessSectionIF
		{
			private PopupExternalOption m_option = null;
			private SectionState mSectionState = null;

			public string Caller
			{
				get { return WebDBManager.SMSCaller; }
			}

			private ArrayList mCallList = null;
			public ArrayList CallList
			{
				get { return mCallList; }
				set { mCallList = value; }
			}

			private ArrayList mFaxList = null;
			public ArrayList FaxList
			{
				get { return mFaxList; }
				set { mFaxList = value; }
			}
		
			public ExternalNotifyProcess(SectionState state)
			{
				mSectionState = state;
				mCallList = new ArrayList();
				mFaxList = new ArrayList();
			}

			public override void Progress()
			{
				SectionDataExternal external = (SectionDataExternal)mSectionState.Section.Data;
				if (external == null)
					return;

				mCallList.Clear();
				mFaxList.Clear();
				ArrayList reciverTeam = external.SMSReceivers;
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
							mFaxList.Add(dataUsr.FaxNumber);
							mCallList.Add(dataUsr.PhoneNumber);
						}
					}
					else
					{
						mFaxList.Add(dataEx.FaxNumber);
						mCallList.Add(dataEx.PhoneNumber);
					}
				}

				// Section 패널을 사용 불가능 상태로 변경
				FormSOP.Instance.Invoke((MethodInvoker)delegate
				{
					FormSOP.Instance.GetPageHome().DiableShowPanels();
				});

				base.Progress();

				m_option = new PopupExternalOption();
				m_option.SetData(mSectionState);

				// 최상위 폼, 전면으로 보낸다
				Form form = (Form)m_option;
				FormSOP.Instance.Invoke((MethodInvoker)delegate
				{
					form.TopMost = true;
					form.BringToFront();
				});

                System.Drawing.Point ptCurrent = m_option.Location;
                m_option.StartPosition = FormStartPosition.Manual;
                m_option.Location = new System.Drawing.Point(FormFrame.Instance.Location.X + ptCurrent.X, FormFrame.Instance.Location.Y + ptCurrent.Y);

                if (m_option.ShowDialog() == DialogResult.OK)
				{
					mCallList.Clear();
					mFaxList.Clear();

					AddPhoneNumber(mCallList, m_option.ExternalTeamPhoneNumbers);
					AddPhoneNumber(mFaxList, m_option.ExternalTeamFaxNumbers);
				}   
				else
				{
					mSectionState.Cancel();
					nState = ProcessSectionState.END;
				}

				// Section 패널을 사용 상태로 변경
				FormSOP.Instance.Invoke((MethodInvoker)delegate
				{
					FormSOP.Instance.GetPageHome().ResotreShowPanels();
				});
			}

			private void AddPhoneNumber(ArrayList arrPhoneNumbers, Dictionary<string, string> dicPhoneNumbers)
			{
				foreach (KeyValuePair<string, string> pair in dicPhoneNumbers)
				{
					arrPhoneNumbers.Add(pair.Value);
				}
			}

			public override void Dispose()
			{
				base.Dispose();
			}

            public override void SendSMSMessage()
			{
				SectionDataExternal external = (SectionDataExternal)mSectionState.Section.Data;

				if (external.UseSMS && (mCallList.Count > 0))
				{
					string szMessage = "";
					//m_option.Invoke((MethodInvoker)delegate
					FormSOP.Instance.Invoke((MethodInvoker)delegate
					{
						szMessage = m_option.GetMessage();
					});
					SendMessage(mCallList, Caller, szMessage);
				}

				if (external.UseFax && (mFaxList.Count > 0))
				{
					// do nothing
				}

			}
		}
	}
}