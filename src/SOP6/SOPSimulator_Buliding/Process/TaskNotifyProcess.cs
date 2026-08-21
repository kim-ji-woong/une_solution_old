using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using SOPMonitoringSystem.Popup;

using Sections;
using UnE.SOP;
using UnE.SOP.Process;
using UnE.SOP.Workstate;
using UnE.SOP.TTS;

namespace SOPMonitoringSystem
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

			public string Caller
			{
				get { return WebDBManager.SMSCaller; }
			}

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
				// CompanyMember ID, 전화번호
				Dictionary<int, string> dicMemberPhone = new Dictionary<int, string>();
				// 사용자 정의조직 전화번호
				ArrayList arrUserDefinedPhoneList = new ArrayList();
				ArrayList arTargetTeamList = data.TeamList;

				foreach (SOPTeam teamData in arTargetTeamList)
				{
					ArrayList arrMembers = null;

                    if (teamData.TeamType == SOPTeam.SOPTeamType.Normal || teamData.TeamType == SOPTeam.SOPTeamType.Holiday)   // 평일 조직 또는 야간 조직
					{
                        bool bEmergency = ((teamData.TeamType == SOPTeam.SOPTeamType.Normal) ? true : false);
						arrMembers = new ArrayList();
						
						FormSOP.Instance.SOPManager.GetCompanyMemberList(teamData.TeamID, bEmergency, ref arrMembers);
                   
					}
                    else if (teamData.TeamType == SOPTeam.SOPTeamType.External)    // 협력 회사 혹은 외부 기관
                    {
                        Data_ExternalTeam team = FormSOP.Instance.SOPManager.GetExternalTeam(teamData.TeamID);

                        if (team != null)
                        {
                            if (!arrUserDefinedPhoneList.Contains(team.PhoneNumber))
                                arrUserDefinedPhoneList.Add(team.PhoneNumber);
                        }
                    }
                    else if (teamData.TeamType == SOPTeam.SOPTeamType.UserDefined)    // 사용자 정의 조직
					{
						Data_ExternalTeam team = FormSOP.Instance.SOPManager.GetUserDefinedTeamMember(teamData.TeamID);
                        //Data_ExternalTeam team2 = FormSOP.Instance.SOPManager.GetUserDefinedTeamMember
						if (team != null)
						{
							if (!arrUserDefinedPhoneList.Contains(team.PhoneNumber))
								arrUserDefinedPhoneList.Add(team.PhoneNumber);
						}
					}
                    else if (teamData.TeamType == SOPTeam.SOPTeamType.Regular)    // 정규 조직
					{
						arrMembers = new ArrayList();
						FormSOP.Instance.SOPManager.GetRegularCompanyMemberList(teamData.TeamID, ref arrMembers);
					}

					if (arrMembers != null && arrMembers.Count > 0)
					{
						// 중복 제거를 위하여 Dictionary에 임시 저장
						AddMember(dicMemberPhone, arrMembers);

						if (arrMembers != null && arrMembers.Count > 0)
							mTeamLeaderCallList.Add(((Data_CompanyMember)arrMembers[0]).PhoneNumber);
					}
				}

				// Dictionary로부터 전화번호 리스트를 얻어옴
				foreach (KeyValuePair<int, string> pair in dicMemberPhone)
				{
					mCallList.Add(pair.Value);
				}

				foreach (string strPhoneNumber in arrUserDefinedPhoneList)
					mCallList.Add(strPhoneNumber);
			}

			private void AddMember(Dictionary<int, string> dicMemberPhone, ArrayList arrMembers)
			{
				foreach (Data_CompanyMember member in arrMembers)
				{
					if (dicMemberPhone.ContainsKey(member.ID))
						continue;

					dicMemberPhone[member.ID] = member.PhoneNumber;
				}
			}

			public override void Progress()
			{
                //System.Diagnostics.Trace.WriteLine("TaskProcess Begin : " + DateTime.Now);
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
					FormSOP.Instance.Invoke(OnPostProcess, param);
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

                //System.Diagnostics.Trace.WriteLine("TaskProcess End : " + DateTime.Now);
			}

			private string MakeBroadcastMessage()
			{
				string szMessage = "";
				SectionDataProcess data = (SectionDataProcess)(mSection.Data);

				if (data != null)
				{
					foreach (MissionItem item in data.MissionItems)
					{
						if (item.CheckItem == true)
						{
							MissionItemInfo info = FormSOP.Instance.GetMissionInfo(item);

							if (info != null && info.UseBroadcast)
							{
								if (szMessage.Length == 0)
									szMessage = item.Mission;
								else
									szMessage += ",," + item.Mission;
							}
						}
					}
				}

				szMessage = szMessage.Replace('(', ' ');
				szMessage = szMessage.Replace(")", "..");
				szMessage = szMessage.Replace('-', ',');
				szMessage = szMessage.Replace('&', ',');
				//szMessage = CheckNumberString(szMessage);

				return szMessage;
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
				strMessage = FormSOP.Instance.DBManager.SMS_ADD_TEXT + strMessage;
				ArrayList arrMessages = new ArrayList();

				int nByteLength = 0, nBeginIndex = 0;
				int nLen = strMessage.Length;

				for (int i = 0; i < nLen; i++)
				{
					if (strMessage.ElementAt(i) < 256)
						nByteLength++;
					else
						nByteLength += 2;

					if (nByteLength == 80 || (nByteLength == 79 && i < nLen - 1 && strMessage.ElementAt(i + 1) >= 256))
					{
						string strMsg = strMessage.Substring(nBeginIndex, i - nBeginIndex + 1);
						arrMessages.Add(strMsg);
					 
						nBeginIndex = i + 1;
						nByteLength = 0;
					}
				}

				if (nByteLength > 0)
				{
					string strMsg = strMessage.Substring(nBeginIndex);
					arrMessages.Add(strMsg);
				}

				return arrMessages;
			}

			private string MakeMessage()
			{				
				string szMessage = "";
				SectionDataProcess data = (SectionDataProcess)(mSection.Data);                
				if (data != null)
				{
					ArrayList missionList = new ArrayList();
					foreach (MissionItem item in data.MissionItems)
					{         
                      
						if (item.CheckItem == true)
						{
							MissionItemInfo info = FormSOP.Instance.GetMissionInfo(item);

							if (info != null && info.UseSMS)
								missionList.Add(item);
						}
					}

					if (missionList.Count == 0)
						return "";

                    int nMissionCount = missionList.Count;

                    for (int i = 0; i < nMissionCount; i++)
                    {
                        MissionItem item = (MissionItem)missionList[i];

                        if (nMissionCount > 1)
                            szMessage += string.Format("[{0}] {1}", i + 1, item.Mission);
                        else
                            szMessage += item.Mission;

                        if (i < nMissionCount - 1)
                            szMessage += "\n";
                    }
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

