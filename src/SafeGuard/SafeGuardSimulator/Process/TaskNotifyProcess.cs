using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Sections;
using UnE.SOP;
using UnE.SOP.Process;
using UnE.SOP.Workstate;
using UnE.SOP.TTS;

namespace SOPManager
{
	namespace Process
	{
		class TaskNotifyProcess : ProcessSectionIF
		{            	
			public override event PreProcessEvent OnPreProcess;
			public override event PostProcessEvent OnPostProcess;
		 
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

			private Section mSection = null;
			public Section SelectSection
			{
				get { return mSection; }
				set { mSection = value; }
			}

            //public string Caller
            //{
            //    get { return WebDBManager.SMSCaller; }
            //}

			private SectionState mSectionState = null;
			private ArrayList mCallList = new ArrayList();
			private ArrayList mTeamLeaderCallList = new ArrayList();

			public TaskNotifyProcess(SectionState state)
			{
				mSectionState = state;
				mSection = state.Section;
			}

			public void MakePhoneList(SectionDataProcess data)
			{
			}

			private void AddMember(Dictionary<int, string> dicMemberPhone, ArrayList arrMembers)
			{
			}

			public override void Progress()
			{
				if (mSection == null || mSection.GetComponentType() != Section.ComponentType.PROCESS)
					return;

				SectionDataProcess data = (SectionDataProcess)(mSection.Data);
				if (data == null)
					return;

				if( OnPreProcess != null)
				{
					OnPreProcess(this, new ProcessSectionEventArgs());
				}
				
				base.Progress();

				//if (data.MissionTransfer == true)
				{
					MakePhoneList(data);
				}

				if (OnPostProcess != null)
				{
                    Object[] param = { this, new ProcessSectionEventArgs() };
					FormMain.Instance.Invoke(OnPostProcess, param);
				}

				//if (data.MissionTransfer == true)
				{
                    //ArrayList callList = mCallList;
                    //if (data.TransferTeamLeaderOnly)
                    //{
                    //    callList = mTeamLeaderCallList;
                    //}

                    //string message = MakeMessage();

                    //if (message.Length > 0)
                    //{
                    //    ArrayList arrMessages = MakeMessageList(message);

                    //    //SendMessage(callList, szCaller, message);
                    //    foreach (string strMessage in arrMessages)
                    //    {
                    //        SendMessage(callList, Caller, strMessage);
                    //    }
                    //}

                    //string strBroadcastMessage = MakeBroadcastMessage();

                    //if (strBroadcastMessage.Length > 0)
                    //{
                    //    TTSManager.Instance.AddSpeech(strBroadcastMessage, 1, false);
                    //}
				}
				nState = ProcessSectionState.END;
			}

			private string MakeBroadcastMessage()
			{
				return "";
			}

			private string CheckNumberString(string szMessage)
			{
				int len = szMessage.Length;
				string strResult = "";
				//string strToken = "";

				for (int i = 0; i < len; i++)
				{
					char ch = szMessage.ElementAt(i);

					if (ch >= '0' && ch <= '9')
					{
						bool find = false;

						if (i == 0)
						{
							if (i == len - 1)
								find = true;
							else
							{
								char next = szMessage.ElementAt(i + 1);

								if (next < '0' || next > '9')
									find = true;
							}
						}
						else if (i == len - 1)
						{
							char prev = szMessage.ElementAt(i - 1);

							if (prev < '0' || prev > '9')
								find = true;
						}
						else
						{
							char next = szMessage.ElementAt(i + 1);
							char prev = szMessage.ElementAt(i - 1);

							if ((prev < '0' || prev > '9') && (next < '0' || next > '9'))
								find = true;
						}

						if (find)
						{
							strResult += "[[" + ch + "]]";
							continue;
						}
					}

					strResult += ch;
				}			

				return strResult;
			}

			private ArrayList MakeMessageList(string strMessage)
			{
				ArrayList arrMessages = new ArrayList();
				return arrMessages;
			}

			private string MakeMessage()
			{        
				return "";
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

