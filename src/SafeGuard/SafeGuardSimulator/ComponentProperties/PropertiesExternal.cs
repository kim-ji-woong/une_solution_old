using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing.Design;

namespace SOPManager
{
	public class PropertiesExternal : SectionPropertiesBase
	{
		private ArrayList mSMSReciverList = new ArrayList();
		private string m_szSmsContent = "";
		private string m_szSmsConfig = "";

		private string m_szFaxConfig = "";
		private ArrayList mFaxReciverList = new ArrayList();

		[Category("\t\t일반")]
		[Browsable(true)]
		[DisplayName("제목")]
		[Description("외부상황전파의 제목을 입력합니다.")]
		[Editor(typeof(TextEditor), typeof(UITypeEditor))]
		public string Text
		{
			get
			{
				if (mSection != null)
				{
					m_szText = mSection.Title;
				}
				return m_szText;
			}

			set
			{
				if (mSection != null)
				{
                    if (m_bEnabledSnapshot == true)
					    UndoRedoManager.Instance.SaveSnapshot("컴포넌트 내용 편집");

					mSection.Title = value;
					if (mSection.GetParent() != null)
						mSection.GetParent().Refresh();
				}
				m_szText = value;
			}
		}

		[Category("\t문자전파")]
		[Browsable(true)]
		[DisplayName("사용여부")]
		[Description("문자세시지 전송을 설정합니다.")]
		[TypeConverter(typeof(SmsUsingTypeConverter))]
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		[ReadOnly(false)]
		public string SMSConfig
		{
			get
			{
				if (mSection != null)
				{
					Sections.SectionDataExternal data = (Sections.SectionDataExternal)mSection.Data;
					if (data.UseSMS == false)
					{
						m_szSmsConfig = UsingType.TypeList[0];
					}
					else
					{
						m_szSmsConfig = UsingType.TypeList[1];
					}
				}
				return m_szSmsConfig;
			}
			set
			{
				if (m_szSmsConfig != value)
				{
					if (mSection != null)
					{
                        if (m_bEnabledSnapshot == true)
						    UndoRedoManager.Instance.SaveSnapshot("SMS사용여부 편집");

						Sections.SectionDataExternal data = (Sections.SectionDataExternal)mSection.Data;
						if (value == UsingType.TypeList[0])
						{
							data.UseSMS = false;
						}
						else
						{
							data.UseSMS = true;
						}
						if (mSection.GetParent() != null)
							mSection.GetParent().Refresh();

					}
				}
				m_szSmsConfig = value;
			}
		}

        [Category("\t문자전파")]
		[Browsable(true)]
		[DisplayName("전송내용")]
		[Description("문자세시지 내용을 지정합니다.")]
		[Editor(typeof(TextEditor2), typeof(UITypeEditor))]
		[ReadOnly(false)]
		public string SMSContent
		{
			get
			{
				if (mSection != null)
				{
					Sections.SectionDataExternal data = (Sections.SectionDataExternal)mSection.Data;
					if (data != null)
					{
						m_szSmsContent = data.SMSMessage;						
					}
				}
				return m_szSmsContent;				
			}
			set 
			{
				if( m_szSmsContent != value)
				{
					if (mSection != null)
					{
						Sections.SectionDataExternal data = (Sections.SectionDataExternal)mSection.Data;
						if (data != null)
						{
                            if (m_bEnabledSnapshot == true)
							    UndoRedoManager.Instance.SaveSnapshot("SMS 내용 편집");

							data.SMSMessage = value;
						}
					}
				}
				m_szSmsContent = value;				
			}
		}


        [Category("\t문자전파")]
		[Browsable(true)]
		[DisplayName("수신처")]
		[Description("문자세시지 수신팀을 지정합니다.")]
		[Editor(typeof(ReciveTeamEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TeamNameConverter))]
		[ReadOnly(false)]
		public ArrayList SMSReciverList
		{
			get
			{
				if (mSection != null)
				{
					Sections.SectionDataExternal data = (Sections.SectionDataExternal)mSection.Data;
					if (data != null)
					{
						mSMSReciverList.Clear();
						mSMSReciverList.AddRange(data.SMSReceivers);
					}
				}
				return mSMSReciverList;
			}
			set
			{
				mSMSReciverList.Clear();
				if (value != null)
				{
					mSMSReciverList.AddRange(value);
				}

				if (mSection != null)
				{
					Sections.SectionDataExternal data = (Sections.SectionDataExternal)mSection.Data;
					if (data != null)
					{
						if (mSMSReciverList != null)
						{
							if (CompareExternalTeamList(mSMSReciverList, data.SMSReceivers))
							{
                                if (m_bEnabledSnapshot == true)
								    UndoRedoManager.Instance.SaveSnapshot("대상 팀 편집");

								ArrayList arrTeamList = data.SMSReceivers;
								arrTeamList.Clear();
								arrTeamList.AddRange(mSMSReciverList);

								if (mSection.GetParent() != null)
									mSection.GetParent().Refresh();
							}
						}
					}
				}
			}
		}



		[Category("팩스전파")]
		[Browsable(false)]
		[DisplayName("사용여부")]
		[Description("Fax 전송을 설정합니다.")]
		[TypeConverter(typeof(UsingTypeConverter))]
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		[ReadOnly(false)]
		public string FaxConfig
		{
			get
			{
				if (mSection != null)
				{
					Sections.SectionDataExternal data = (Sections.SectionDataExternal)mSection.Data;
					if (data.UseFax == false)
					{
						m_szFaxConfig = UsingType.TypeList[0];
					}
					else
					{
						m_szFaxConfig = UsingType.TypeList[1];
					}

				}
				return m_szFaxConfig;
			}
			set
			{
				if (m_szFaxConfig != value)
				{
					if (mSection != null)
					{
						Sections.SectionDataExternal data = (Sections.SectionDataExternal)mSection.Data;
						if (value == UsingType.TypeList[0])
						{
                            if (data.UseFax == true)
                            {
                                if (m_bEnabledSnapshot == true)
                                    UndoRedoManager.Instance.SaveSnapshot("대상 팀 편집");
                                data.UseFax = false;
                            }							
						}
						else
						{
                            if (data.UseFax == false)
                            {
                                if (m_bEnabledSnapshot == true)
                                    UndoRedoManager.Instance.SaveSnapshot("대상 팀 편집");
                                data.UseFax = true;
                            }	
						}
						if (mSection.GetParent() != null)
							mSection.GetParent().Refresh();
					}
				}
				m_szFaxConfig = value;
			}
		}

        [Category("팩스전파")]
		[Browsable(false)]
		[DisplayName("수신처")]
		[Description("Fax 수신처를 지정합니다.")]
		[Editor(typeof(ReciveTeamEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TeamNameConverter))]
		[ReadOnly(false)]
		public ArrayList FaxReciverList
		{
			get
			{
				if (mSection != null)
				{
					Sections.SectionDataExternal data = (Sections.SectionDataExternal)mSection.Data;
					if (data != null)
					{
						mFaxReciverList.Clear();
						mFaxReciverList.AddRange(data.FaxReceivers);
					}
				}
				return mFaxReciverList;
			}
			set
			{
				mFaxReciverList.Clear();
				if (value != null)
				{
					mFaxReciverList.AddRange(value);
				}

				if (mSection != null)
				{
					Sections.SectionDataExternal data = (Sections.SectionDataExternal)mSection.Data;
					if (data != null)
					{
						if (mSMSReciverList != null)
						{
							if (CompareExternalTeamList(mFaxReciverList, data.FaxReceivers))
							{
                                if (m_bEnabledSnapshot == true)
								    UndoRedoManager.Instance.SaveSnapshot("대상 팀 편집");

								ArrayList arrTeamList = data.FaxReceivers;
								arrTeamList.Clear();
								arrTeamList.AddRange(mFaxReciverList);

								if (mSection.GetParent() != null)
									mSection.GetParent().Refresh();
							}							
						}
					}
				}
			}
		}


		private bool CompareExternalTeamList(ArrayList arTeamList, ArrayList arOrgTeamList)
		{
			if (arTeamList == null)
				return false;

			if (arOrgTeamList == null)
				return true;

			if (arTeamList.Count != arOrgTeamList.Count)
				return true;

			for (int i = 0; i < arTeamList.Count; i++)
			{
				Sections.ExternalTeamData item = (Sections.ExternalTeamData)arTeamList[i];
				Sections.ExternalTeamData item2 = (Sections.ExternalTeamData)arOrgTeamList[i];

				if (item.TeamID != item2.TeamID)
					return true;
				if (item.TeamName != item2.TeamName)
					return true;
				if (item.PhoneNumber != item2.PhoneNumber)
					return true;
				if (item.FaxNumber != item2.FaxNumber)
					return true;
			}
			return false;
		}
	}
}

