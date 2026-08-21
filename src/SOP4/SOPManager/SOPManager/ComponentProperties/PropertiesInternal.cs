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
	public class PropertiesInternal : SectionPropertiesBase
	{
		private string m_szBroadcastContent = "";
		private string m_szBroadcastConfig = "";
		private string m_szMobileConfig = "";
		private string m_szPopupConfig = "";

		[Category("\t\t일반")]
		[Browsable(true)]
		[DisplayName("제목")]
		[Description("내부 상황전파의 제목을 입력합니다.")]
        [Editor(typeof(BroadcastEditor), typeof(UITypeEditor))]
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
		[Browsable(false)]
		[DisplayName("팝업메시지")]
		[Description("팝업 메시지 전송을 설정합니다.")]
		[TypeConverter(typeof(PopupUsingTypeConverter))]		
		[ReadOnly(false)]
		public string PopupConfig
		{
			get
			{
				if (mSection != null)
				{
					Sections.SectionDataInternal data = (Sections.SectionDataInternal)mSection.Data;
					if (data.UsePopupMessage == false)
					{
						m_szPopupConfig = UsingType.TypeList[0];
					}
					else
					{
						m_szPopupConfig = UsingType.TypeList[1];
					}

				}
				return m_szPopupConfig;
			}
			set
			{
				if (m_szPopupConfig != value)
				{
					if (mSection != null)
					{
                        if (m_bEnabledSnapshot == true)
						    UndoRedoManager.Instance.SaveSnapshot("Popup 메시지 사용여부 편집");

						Sections.SectionDataInternal data = (Sections.SectionDataInternal)mSection.Data;
						if (value == UsingType.TypeList[0])
						{
							data.UsePopupMessage = false;
						}
						else
						{
							data.UsePopupMessage = true;
						}						

						if (mSection.GetParent() != null)
							mSection.GetParent().Refresh();

					}
				}
				m_szPopupConfig = value;
			}
		}

        private void ChangeState(bool bReadOnly)
        {
            try
            {
                PropertyDescriptor descriptor2 = TypeDescriptor.GetProperties(this.GetType())["SectionCommander"];
                ReadOnlyAttribute attribute2 = (ReadOnlyAttribute)descriptor2.Attributes[typeof(ReadOnlyAttribute)];
                FieldInfo fieldToChange2 = attribute2.GetType().GetField("isReadOnly",
                                                    System.Reflection.BindingFlags.NonPublic |
                                                    System.Reflection.BindingFlags.Instance);
                fieldToChange2.SetValue(attribute2, bReadOnly);

            }
            catch (Exception)
            {
            }

            try
            {
                PropertyDescriptor descriptor2 = TypeDescriptor.GetProperties(this.GetType())["SelectedTeamList"];
                ReadOnlyAttribute attribute2 = (ReadOnlyAttribute)descriptor2.Attributes[typeof(ReadOnlyAttribute)];
                FieldInfo fieldToChange2 = attribute2.GetType().GetField("isReadOnly",
                                                    System.Reflection.BindingFlags.NonPublic |
                                                    System.Reflection.BindingFlags.Instance);
                fieldToChange2.SetValue(attribute2, bReadOnly);
            }
            catch (Exception)
            {
            }
        }
	
		[Category("\t문자전파")]
		[Browsable(true)]
		[DisplayName("문자메시지")]
		[Description("모바일 문자 전송을 설정합니다.")]
		[TypeConverter(typeof(SmsUsingTypeConverter))]
		[ReadOnly(false)]
		public string MobileAppConfig
		{
			get
			{
				if (mSection != null)
				{
					Sections.SectionDataInternal data = (Sections.SectionDataInternal)mSection.Data;
					if (data.UseMobileApp == false)
					{
                        ChangeState(true);
						m_szMobileConfig = UsingType.TypeList[0];
					}
					else
					{
                        ChangeState(false);
						m_szMobileConfig = UsingType.TypeList[1];
					}

				}
				return m_szMobileConfig;
			}
			set
			{
				if (m_szMobileConfig != value)
				{
					if (mSection != null)
					{
                        if (m_bEnabledSnapshot == true)
						    UndoRedoManager.Instance.SaveSnapshot("모바일 앱 메시지 사용여부 편집");

						Sections.SectionDataInternal data = (Sections.SectionDataInternal)mSection.Data;
						if (value == UsingType.TypeList[0])
						{
							data.UseMobileApp = false;
                            data.UseBroadcast = true;

                            m_szBroadcastConfig = UsingType.TypeList[1];
						}
						else
						{
							data.UseMobileApp = true;
                            data.UseBroadcast = false;

                            m_szBroadcastConfig = UsingType.TypeList[0];
						}

                        ChangeState(!data.UseMobileApp);

                        if (formParent != null)
                        {
                            formParent.RefreshTabs(PropertyTabScope.Document);
                        }

						if (mSection.GetParent() != null)
							mSection.GetParent().Refresh();

                        if (formParent != null)
                            formParent.Refresh();

					}
				}
				m_szMobileConfig = value;
			}
		}

        private Sections.SectionCommander m_szSectionCommander = new Sections.SectionCommander();
        [Category("\t문자전파")]
        [Browsable(true)]
        [DisplayName("발신자")]
        [Description("문자 발신할 팀을 지정합니다.")]
        [Editor(typeof(SMSCommanderEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(StringConverter))]
        [ReadOnly(false)]
        public Sections.SectionCommander SectionCommander
        {
            get
            {
                if (mSection != null)
                {
                    Sections.SectionDataInternal data = (Sections.SectionDataInternal)mSection.Data;
                    if (data != null && data.Commander != null)
                    {
                        m_szSectionCommander.DisplayText = data.Commander.DisplayText;
                        m_szSectionCommander.IsTeamMember = data.Commander.IsTeamMember;
                        m_szSectionCommander.Team = data.Commander.Team;
                        m_szSectionCommander.TeamMemberID = data.Commander.TeamMemberID;

                    }
                }
                return m_szSectionCommander;
            }
            set
            {
                m_szSectionCommander = value;
                if (mSection != null)
                {
                    Sections.SectionDataInternal data = (Sections.SectionDataInternal)mSection.Data;
                    if (data != null)
                    {
                        if (m_szSectionCommander != null)
                        {
                            if (CompareCommander(m_szSectionCommander, data.Commander))
                            {
                                if (m_bEnabledSnapshot == true)
                                    UndoRedoManager.Instance.SaveSnapshot("대상 팀 편집");

                                data.Commander.DisplayText = value.DisplayText;
                                data.Commander.IsTeamMember = value.IsTeamMember;
                                data.Commander.Team = value.Team;
                                data.Commander.TeamMemberID = value.TeamMemberID;

                                if (formParent != null)
                                    formParent.Refresh();
                            } 
                        }
                    }
                }
            }
        }

        private bool CompareCommander(Sections.SectionCommander commander1, Sections.SectionCommander commander2)
        {
            if (commander1 == null)
                return false;

            if (commander2 == null)
                return true;

            if (commander1.Team != null && commander2.Team == null)
            {
                return true;
            }
            if (commander2.Team != null && commander1.Team == null)
            {
                return true;
            }
            if ((commander2.Team == null && commander1.Team == null) && (commander2.DisplayText == commander1.DisplayText))
            {
                return false;
            }

            if (commander1.DisplayText != commander2.DisplayText)
                return true;
            if (commander1.TeamMemberID != commander2.TeamMemberID)
                return true;
            if (commander1.Team.TeamType != commander2.Team.TeamType)
            {
                return true;
            }
            if (commander1.Team.TeamID != commander2.Team.TeamID)
            {
                return true;
            }

            return false;
        }


        private ArrayList mSelectedTeamList = null;

        [Category("\t문자전파")]
        [Browsable(true)]
        [DisplayName("수신자")]
        [Description("문자 수신할 팀을 지정합니다.")]
        [Editor(typeof(InternalReciverEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(TeamNameConverter))]
        [ReadOnly(false)]
        public ArrayList SelectedTeamList
        {
            get
            {
                if (mSection != null)
                {
                    Sections.SectionDataInternal data = (Sections.SectionDataInternal)mSection.Data;
                    if (data != null)
                    {
                        mSelectedTeamList = (ArrayList)data.TeamList.Clone();
                    }
                }
                return mSelectedTeamList;
            }
            set
            {
                mSelectedTeamList = value;
                if (mSection != null)
                {
                    Sections.SectionDataInternal data = (Sections.SectionDataInternal)mSection.Data;
                    if (data != null)
                    {
                        if (mSelectedTeamList != null)
                        {
                            ArrayList arrTeamList = data.TeamList;
                            if (CompareTeamList(arrTeamList, mSelectedTeamList))
                            {
                                if (m_bEnabledSnapshot == true)
                                    UndoRedoManager.Instance.SaveSnapshot("대상 팀 편집");
                                arrTeamList.Clear();
                                arrTeamList.AddRange(mSelectedTeamList);

                                if (mSection.GetParent() != null)
                                    mSection.GetParent().Refresh();

                                if (formParent != null)
                                    formParent.Refresh();
                            }
                        }
                    }
                }
            }
        }

        private bool CompareTeamList(ArrayList arMission, ArrayList arOrgMission)
        {
            if (arMission == null)
                return false;

            if (arOrgMission == null)
                return true;

            if (arMission.Count != arOrgMission.Count)
                return true;

            for (int i = 0; i < arMission.Count; i++)
            {
                Sections.SOPTeam item = (Sections.SOPTeam)arMission[i];
                Sections.SOPTeam item2 = (Sections.SOPTeam)arOrgMission[i];

                if (item.TeamID != item2.TeamID)
                    return true;
                if (item.TeamName != item2.TeamName)
                    return true;
                if (item.TeamType != item2.TeamType)
                    return true;
            }
            return false;
        }

        private string m_szTransferLeaderOnly = "";

        [Category("\t문자전파")]
        [Browsable(false)]
        [DisplayName("수신범위")]
        [Description("메시지 수신 범위를 결정합니다.")]
        [TypeConverter(typeof(MsgSendTypeConverter))]
        [ReadOnly(false)]
        public string TransferTeamLeaderOnly
        {
            get
            {
                m_szTransferLeaderOnly = MsgSendType.TypeList[0];
                if (mSection != null)
                {
                    Sections.SectionDataInternal data = (Sections.SectionDataInternal)mSection.Data;
                    if (data != null)
                    {
                        if (data.TransferTeamLeaderOnly)
                            m_szTransferLeaderOnly = MsgSendType.TypeList[0];
                        else
                            m_szTransferLeaderOnly = MsgSendType.TypeList[1];
                    }
                }
                return m_szTransferLeaderOnly;
            }
            set
            {
                if (m_szTransferLeaderOnly != value)
                {
                    Sections.SectionDataInternal data = (Sections.SectionDataInternal)mSection.Data;
                    if (data != null)
                    {
                        if (m_bEnabledSnapshot == true)
                            UndoRedoManager.Instance.SaveSnapshot("메시지 전송 범위 변경");

                        data.TransferTeamLeaderOnly = !data.TransferTeamLeaderOnly;

                        if (mSection.GetParent() != null)
                            mSection.GetParent().Refresh();
                    }
                }
                m_szTransferLeaderOnly = value;
            }
        }

        [Category("방송전파")]
		[Browsable(true)]
		[DisplayName("사용여부")]
		[Description("사내방송 전송을 설정합니다.")]
		[TypeConverter(typeof(UsingTypeConverter))]
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		[ReadOnly(false)]
		public string BroadcastConfig
		{
			get
			{
				if (mSection != null)
				{
					Sections.SectionDataInternal data = (Sections.SectionDataInternal)mSection.Data;
					if (data.UseBroadcast == false)
					{
						m_szBroadcastConfig = UsingType.TypeList[0];
					}
					else
					{
						m_szBroadcastConfig = UsingType.TypeList[1];
					}

				}
				return m_szBroadcastConfig;
			}
			set
			{
				if (m_szBroadcastConfig != value)
				{
					if (mSection != null)
					{
						Sections.SectionDataInternal data = (Sections.SectionDataInternal)mSection.Data;
						if (value == UsingType.TypeList[0])
						{
							data.UseBroadcast = false;
                            data.UseMobileApp = true;

                            m_szMobileConfig = UsingType.TypeList[1];
						}
						else
						{
							data.UseBroadcast = true;
                            data.UseMobileApp = false;

                            m_szMobileConfig = UsingType.TypeList[0];
						}
                        
						if (mSection.GetParent() != null)
							mSection.GetParent().Refresh();

                        if (formParent != null)
                            formParent.Refresh();
					}
				}
				m_szBroadcastConfig = value;
			}
		}

        [Category("\t\t일반")]
		[Browsable(true)]
		[DisplayName("전파 내용")]
		[Description("전파시킬 내용을 지정합니다.")]
		[Editor(typeof(BroadcastEditor), typeof(UITypeEditor))]
		[ReadOnly(false)]
		public string BroadcastContent
		{
			get
			{
				if (mSection != null)
				{
					Sections.SectionDataInternal data = (Sections.SectionDataInternal)mSection.Data;
					if (data != null)
					{
						m_szBroadcastContent = (data.BroadcastMessage == null ? "" : data.BroadcastMessage);
					}
				}
				return m_szBroadcastContent;
			}
			set
			{
				if (m_szBroadcastContent != value)
				{
					if (mSection != null)
					{
						Sections.SectionDataInternal data = (Sections.SectionDataInternal)mSection.Data;
						if (data != null)
						{
                            if (m_bEnabledSnapshot == true)
							    UndoRedoManager.Instance.SaveSnapshot("방송 내용 편집");

							data.BroadcastMessage = value;
						}
					}
				}
				m_szBroadcastContent = value;
			}
		}

        private string m_strAutoRun = "";

        [Category("\t\t일반")]
        [Browsable(true)]
        [DisplayName("자동실행")]
        [Description("자동실행 여부를 결정합니다.")]
        [TypeConverter(typeof(UsingTypeConverter))]
        [ReadOnly(false)]
        public string AutoRun
        {
            get
            {
                if (mSection != null)
                {
                    Sections.SectionDataInternal data = (Sections.SectionDataInternal)mSection.Data;
                    if (data != null)
                    {
                        if (data.AutoRun)
                            m_strAutoRun = UsingType.TypeList[1];
                        else
                            m_strAutoRun = UsingType.TypeList[0];
                    }
                }
                return m_strAutoRun;
            }
            set
            {
                m_strAutoRun = value;
                if (mSection != null)
                {
                    Sections.SectionDataInternal data = (Sections.SectionDataInternal)mSection.Data;
                    if (data != null)
                    {
                        bool autoRun = m_strAutoRun == UsingType.TypeList[1];

                        if (data.AutoRun != autoRun)
                        {
                            if (m_bEnabledSnapshot == true)
                                UndoRedoManager.Instance.SaveSnapshot("자동실행 편집");

                            data.AutoRun = autoRun;

                            if (mSection.GetParent() != null)
                                mSection.GetParent().Refresh();

                            if (formParent != null)
                                formParent.Refresh();
                        }
                    }
                }
            }
        }
	}
}
