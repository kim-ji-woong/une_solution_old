using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;



namespace SOPManager
{
	public class PropertiesProcess : SectionPropertiesBase
	{
		[Category("\t\t일반")]
		[Browsable(true)]
		[DisplayName("임무제목")]
		[Description("프로세스의 임무를 표시합니다.")]
		[Editor(typeof(MissionEditor), typeof(UITypeEditor))]
		[ReadOnly(false)]
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
				m_szText = value;
				if (mSection != null && m_szText != mSection.Title)
				{
                    if (m_bEnabledSnapshot == true)
					    UndoRedoManager.Instance.SaveSnapshot("컴포넌트 내용 편집");

					mSection.Title = m_szText;
					if (mSection.GetParent() != null)
						mSection.GetParent().Refresh();
				}
			}
		}

        [Category("\t\t일반")]
		[Browsable(false)]
		[DisplayName("수식")]
		[Description("컴포넌트의 수식을 표시합니다.")]
		[Editor(typeof(ExprEditor), typeof(UITypeEditor))]
		[ReadOnly(false)]
		public string Expr
		{
			get
			{
				if (mSection != null)
				{
					Sections.SectionData data = mSection.Data;
					if (data != null)
					{
						m_szExpr = data.Expression;
					}
				}
				return m_szExpr;
			}
			set
			{
				m_szExpr = value;
				if (mSection != null)
				{
					Sections.SectionData data = mSection.Data;
					if (data != null)
					{
                        if (m_bEnabledSnapshot == true)
						    UndoRedoManager.Instance.SaveSnapshot("컴포넌트 수식 편집");

						data.Expression = m_szExpr;
						if (mSection.GetParent() != null)
							mSection.GetParent().Refresh();
					}
				}
			}
		}

        [Category("\t\t일반")]
		[Browsable(true)]
		[DisplayName("처리시간")]
		[Description("임무의 처리 시간을 지정합니다.")]
		[Editor(typeof(PeriodEditor), typeof(UITypeEditor))]
		[ReadOnly(false)]
		public string ProcessTime
		{
			get
			{
				if (mSection != null)
				{
					Sections.SectionDataProcess data = (Sections.SectionDataProcess)mSection.Data;
					if (data != null)
					{
						m_szProcessTime = GetProcessTime(data);
					}
				}
				return m_szProcessTime;
			}
			set
			{
				m_szProcessTime = value;
				if (mSection != null)
				{
					Sections.SectionDataProcess data = (Sections.SectionDataProcess)mSection.Data;
					if (data != null)
					{
						Sections.ProcessingTime time = SetTransTime(data, value);
						if( time != null)
						{
                            if (m_bEnabledSnapshot == true)
							    UndoRedoManager.Instance.SaveSnapshot("처리 시간 편집");

							data.ProcessingTime = time;

							if( time.Time != 0)
							{
								data.UseProcessingTime = true;
							}
							else
							{
								data.UseProcessingTime = false;
							}
							if (mSection.GetParent() != null)
								mSection.GetParent().Refresh();
						}
					}
				}
			}
		}

        private Sections.SectionCommander m_szSectionCommander = new Sections.SectionCommander();
        [Category("\t\t일반")]
        [Browsable(true)]
        [DisplayName("발신자")]
        [Description("문자 발신할 팀을 지정합니다.")]
        [Editor(typeof(SMSCommanderEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(StringConverter))]
        [RefreshProperties(RefreshProperties.All)]
        [ReadOnly(false)]
        public Sections.SectionCommander SectionCommander
        {
            get
            {
                if (mSection != null)
                {
                    Sections.SectionDataProcess data = (Sections.SectionDataProcess)mSection.Data;
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
                    Sections.SectionDataProcess data = (Sections.SectionDataProcess)mSection.Data;
                    if (data != null)
                    {
                        if (m_szSectionCommander != null)
                        {
                            if(CompareCommander(m_szSectionCommander, data.Commander))
                            {
                                if (m_bEnabledSnapshot == true)
                                    UndoRedoManager.Instance.SaveSnapshot("대상 팀 편집");
                                                                
                                data.Commander.DisplayText = value.DisplayText;
                                data.Commander.IsTeamMember = value.IsTeamMember;
                                data.Commander.Team = value.Team;
                                data.Commander.TeamMemberID = value.TeamMemberID;

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

        [Category("\t\t일반")]
		[Browsable(true)]
		[DisplayName("수신자")]
		[Description("프로세스를 수행할 팀을 지정합니다.")]
		[Editor(typeof(ProcessTeamEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TeamNameConverter))]
		[ReadOnly(false)]
		public ArrayList SelectedTeamList
		{
			get
			{
				if (mSection != null)
				{
					Sections.SectionDataProcess data = (Sections.SectionDataProcess)mSection.Data;
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
					Sections.SectionDataProcess data = (Sections.SectionDataProcess)mSection.Data;
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

                                SetSelectedTeam();

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
                    Sections.SectionDataProcess data = (Sections.SectionDataProcess)mSection.Data;
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
                    Sections.SectionDataProcess data = (Sections.SectionDataProcess)mSection.Data;
                    if (data != null)
                    {
                        bool autoRun = m_strAutoRun == UsingType.TypeList[1];

                        if (autoRun != data.AutoRun)
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

		private ArrayList mMissionList = null;
        [Category("\t\t일반")]
		[Browsable(false)]
		[DisplayName("세부임무")]
		[Description("세부 임무을 지정합니다.")]
		[ReadOnly(false)]
		public ArrayList MissionList
		{
			get
			{
				if (mSection != null)
				{
					Sections.SectionDataProcess data = (Sections.SectionDataProcess)mSection.Data;
					if (data != null)
					{
						mMissionList = data.MissionItems;
					}
				}
				return mMissionList;
			}
			set
			{
				mMissionList = value;
				if (mSection != null)
				{
					Sections.SectionDataProcess data = (Sections.SectionDataProcess)mSection.Data;
					if (data != null)
					{
						if (mMissionList != null)
						{
							ArrayList arrTeamList = data.MissionItems;
							if(CompareMissionList(mMissionList, arrTeamList))
							{
                                if (m_bEnabledSnapshot == true)
								    UndoRedoManager.Instance.SaveSnapshot("세부 임무 편집");
								
								arrTeamList.Clear();
								arrTeamList.AddRange(mMissionList);

								if (mSection.GetParent() != null)
									mSection.GetParent().Refresh();
							}
						}
					}
				}
			}
		}

		private bool CompareMissionList(ArrayList arMission, ArrayList arOrgMission)
		{
			if (arMission == null)
				return false;

			if (arOrgMission == null)
				return true;

			if (arMission.Count != arOrgMission.Count)
				return true;

			for (int i = 0; i < arMission.Count;i++ )
			{
				Sections.MissionItem item = (Sections.MissionItem)arMission[i];
				Sections.MissionItem item2 = (Sections.MissionItem)arOrgMission[i];

				if (item.Target != item2.Target)
					return true;
				if (item.Mission != item2.Mission)
					return true;
				if (item.TransmissionType != item2.TransmissionType)
					return true;
                if (CompareCommander(item.Commander, item2.Commander))
                    return true;
			}
			return false;
		}

		private void SetSelectedTeam()
		{
			string strValue = "";
			int nRow = 0;
			Sections.SectionDataProcess data = (Sections.SectionDataProcess)mSection.Data;
			foreach (Sections.SOPTeam team in data.TeamList)
			{
				strValue += team.TeamName;
				if (data.TeamList.Count > 1 && nRow != data.TeamList.Count - 1)
				{
					strValue += ", ";
					nRow++;
				}
			}

			Sections.SectionProcess processSection = (Sections.SectionProcess)mSection;
			processSection.TextDown = strValue;
		}

		private string m_szTransferMsg = "";

		[Category("\t임무전달")]
		[Browsable(false)]
		[DisplayName("임무메시지")]
		[Description("임무 대상자에게 메시지를 전송합니다.")]
		[TypeConverter(typeof(UsingTypeConverter))]
		[ReadOnly(false)]
		public string MissionTransfer
		{
			get
			{
				m_szTransferMsg = UsingType.TypeList[0];
				if (mSection != null)
				{
					Sections.SectionDataProcess data = (Sections.SectionDataProcess)mSection.Data;
					if (data != null)
					{
						if (data.MissionTransfer)
							m_szTransferMsg = UsingType.TypeList[1];
						else
							m_szTransferMsg = UsingType.TypeList[0];
					}
				}
				return m_szTransferMsg;
			}
			set
			{
				if (m_szTransferMsg != value)
				{
					Sections.SectionDataProcess data = (Sections.SectionDataProcess)mSection.Data;
					if (data != null)
					{
                        if (m_bEnabledSnapshot == true)
						    UndoRedoManager.Instance.SaveSnapshot("임무메시지 전송 타입 변경");
						data.MissionTransfer = !data.MissionTransfer;
						try
						{
							PropertyDescriptor descriptor = TypeDescriptor.GetProperties(this.GetType())["TransferTeamLeaderOnly"];	
							ReadOnlyAttribute attribute = (ReadOnlyAttribute)descriptor.Attributes[typeof(ReadOnlyAttribute)];
							FieldInfo fieldToChange = attribute.GetType().GetField("isReadOnly",
																System.Reflection.BindingFlags.NonPublic |
																System.Reflection.BindingFlags.Instance);
							fieldToChange.SetValue(attribute, !data.MissionTransfer);
						}
						catch(Exception)
						{ 
						}
						
						if (mSection.GetParent() != null)
							mSection.GetParent().Refresh();
					}
				}
				m_szTransferMsg = value;
			}
		}


		private string m_szTransferLeaderOnly = "";

		[Category("\t임무전달")]
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
					Sections.SectionDataProcess data = (Sections.SectionDataProcess)mSection.Data;
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
					Sections.SectionDataProcess data = (Sections.SectionDataProcess)mSection.Data;
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

		[Category("일반")]
		[Browsable(false)]
		[DisplayName("표시 옵션")]
		[Description("컴포넌트를 화면에 표시하는 방법입니다.")]
		[TypeConverter(typeof(DisplayTypeConverter))]
		[ReadOnly(false)]
		public string TextOption
		{
			get
			{
				if (mSection != null)
				{
					Sections.SectionData data = mSection.Data;
					if (data != null)
					{
						if (data.ShowExpression)
							m_szDisplay = DisplayTypes.TypeList[1];
						else
							m_szDisplay = DisplayTypes.TypeList[0];
					}
				}
				return m_szDisplay;
			}
			set
			{
				if (m_szDisplay != value)
				{
					Sections.SectionData data = mSection.Data;
					if (data != null)
					{
                        if (m_bEnabledSnapshot == true)
						    UndoRedoManager.Instance.SaveSnapshot("컴포넌트 표시 타입 변경");

						data.ShowExpression = !data.ShowExpression;

                        if (mSection.GetParent() != null)
							mSection.GetParent().Refresh();
					}
				}
				m_szDisplay = value;
			}
		}
		
		private string GetProcessTime(Sections.SectionDataProcess data)
		{
			string[] strType = { "개월", "주", "일", "시간", "분", "사용안함" };
			string strValue = "";

			switch (data.ProcessingTime.ProcessingType)
			{
				case Sections.ProcessingTime.Type.MONTH:
					strValue = strType[0];
					break;
				case Sections.ProcessingTime.Type.WEEK:
					strValue = strType[1];
					break;
				case Sections.ProcessingTime.Type.DAY:
					strValue = strType[2];
					break;
				case Sections.ProcessingTime.Type.HOUR:
					strValue = strType[3];
					break;
				case Sections.ProcessingTime.Type.MINUTE:
					strValue = strType[4];
					break;
				case Sections.ProcessingTime.Type.UNKNOWN:
					strValue = strType[5];
					break;
			}

			string strProcessTime;
			if (data.ProcessingTime.ProcessingType == Sections.ProcessingTime.Type.UNKNOWN)
			{
				strProcessTime = "사용안함";
			}
			else
			{
				strProcessTime = data.ProcessingTime.Time.ToString() + " " + strValue;
			}
			return strProcessTime;
		}

		private Sections.ProcessingTime SetTransTime(Sections.SectionDataProcess data, string szValue)
		{

			if (szValue == null || data == null)
				return null;
			
			Sections.ProcessingTime processTime = new Sections.ProcessingTime();			
			Sections.ProcessingTime orgProcessTime = data.ProcessingTime;

			try
			{
				string[] strProcessTime = szValue.Split(new char[] { ' ' });				
				string[] strOption = { "개월", "주", "일", "시간", "분", "사용안함" };

				int nType = 0;

				if (strProcessTime.Length > 1)
				{
					foreach (string strValue in strOption)
					{
						if (strValue == strProcessTime[1])
							break;

						nType++;
					}
					
					int nTime = -1;
					if(int.TryParse(strProcessTime[0], out nTime))
					{
						if (data.ProcessingTime.Time == nTime)
						{
							return null;
						}
					}

					processTime.Time = nTime;
				}
				else
				{
					nType = 5;
				}				
				
				switch (nType)
				{
					case 0:
						processTime.ProcessingType = Sections.ProcessingTime.Type.MONTH;
						break;
					case 1:
						processTime.ProcessingType = Sections.ProcessingTime.Type.WEEK;
						break;
					case 2:
						processTime.ProcessingType = Sections.ProcessingTime.Type.DAY;
						break;
					case 3:
						processTime.ProcessingType = Sections.ProcessingTime.Type.HOUR;
						break;
					case 4:
						processTime.ProcessingType = Sections.ProcessingTime.Type.MINUTE;
						break;
					case 5:
						processTime.ProcessingType = Sections.ProcessingTime.Type.UNKNOWN;
						processTime.Time = 0;
						break;
				}
				return processTime;

			}
			catch(Exception e)
			{
				System.Diagnostics.Trace.WriteLine(e.StackTrace);
			}
			return null;			
		}
	}
}
