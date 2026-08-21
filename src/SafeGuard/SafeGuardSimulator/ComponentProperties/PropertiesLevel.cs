using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Design;

namespace SOPManager
{
	public class PropertiesLevel
	{
        private bool m_bEnabledSnapshot = true;

        [Browsable(false)]
        public bool EnabledSnapshot
        {
            get { return m_bEnabledSnapshot; }
            set { m_bEnabledSnapshot = value; }
        }


		private Data_ActionStep mData = null;
		public void SetData(Data_ActionStep data)
		{
			if (data == null)
				return;

			mData = data;

			// 기본값 설정 위치
			m_nProcessType = mData.ProcessTimeType;
			m_nNumberType = mData.IterationType;
			m_nPeriodType = mData.PeriodType;
			m_dtBeginTime = mData.BeginTime;
			m_dtEndTime = mData.EndTime;
			m_nWeekDayOPtion = mData.WeekdayOption;		
		}
		
		[Category("\t일반")]
		[Browsable(true)]
		[DisplayName("대분류")]
		[Description("SOP의 분류를 표시합니다.")]
		[ReadOnly(true)]
		public string CategoryName
		{
			get
			{
				return SopDocManager.Instance.CategoryName;
			}

			set
			{
				if (SopDocManager.Instance.CategoryName != value)
				{
					SopDocManager.Instance.CategoryName = value;
				}
				
			}
		}

		
		[Category("\t일반")]
		[Browsable(true)]
		[DisplayName("중분류")]
		[Description("SOP의 중분류를 표시합니다.")]
		[ReadOnly(true)]
		public string SubCategoryName
		{
			get
			{
				return SopDocManager.Instance.SubCategoryName;
			}

			set
			{
				if (SopDocManager.Instance.SubCategoryName != value)
				{
					SopDocManager.Instance.SubCategoryName = value;
				}

			}
		}

		
		[Category("\t일반")]
		[Browsable(true)]
		[DisplayName("재난명")]
		[Description("SOP의 재난명을 표시합니다.")]
		[ReadOnly(true)]
		public string DisasterName
		{
			get
			{
				return SopDocManager.Instance.DisasterName;
			}

			set
			{
				if (SopDocManager.Instance.DisasterName != value)
				{
					SopDocManager.Instance.DisasterName = value;
				}

			}
		}


		private string m_szStepName = "";
		[Category("\t일반")]
		[Browsable(true)]
		[DisplayName("단계명")]
		[Description("SOP의 단계를 표시합니다.")]
		public string StepName
		{
			get
			{
				if( mData != null)
				{
					m_szStepName = mData.StepName;
				}
				return m_szStepName;
			}
			
			set
			{
				if( m_szStepName != value)
				{
					if( mData != null)
					{
                        if( FormMain.Instance.GetPageLevel().GetBarLevelTree().ChangeActionStepName(m_szStepName, value))
                        {
                            if (m_bEnabledSnapshot == true)
                                UndoRedoManager.Instance.SaveSnapshot("단계 이름 변경");

                            mData.StepName = value;
                            // Change Tab Name
                            TabPage page = GetTabPage(m_szStepName);
                            if (page != null)
                            {
                                page.Text = value;
                                page.ToolTipText = value;
                            }      
                        }
                        else
                        {
                            UnE.Utility.UMessageBox.Show("지정한 이름이 사용중 입니다\n다른 이름을 지정 하십시요", "단계 이름 중복 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        
					}
				}
				m_szStepName = value;
			}
		}

		private string m_szDuration = "";
		[Category("\t일반")]
		[Browsable(false)]
		[DisplayName("처리 기간")]
		[Editor(typeof(TermEditor), typeof(UITypeEditor))]
		[Description("임무의 수행기간을 지정합니다.")]
		[ReadOnly(false)]
		public string Duration
		{
			get
			{
				if( mData != null)
				{
					m_szDuration = GetPeriodData(mData, mData.PeriodType);
				}
				return m_szDuration;
			}
			set
			{
				if (m_szDuration != value)
				{
					if (mData != null)
					{
						mData.PeriodType = m_nPeriodType;
						mData.BeginTime = m_dtBeginTime;
						mData.EndTime = m_dtEndTime;
						mData.WeekdayOption = m_nWeekDayOPtion;
						m_szDuration = value;						
					}
				}				
			}

		}

		private string m_szRepeat = "";
		[Category("\t일반")]
		[Browsable(false)]
		[DisplayName("횟수")]
		[Description("임무의 수행 횟수를 지정합니다.")]
		[Editor(typeof(RepeatTimeEditor), typeof(UITypeEditor))]
		[ReadOnly(false)]
		public string RepeatTime
		{
			get
			{
				if( mData != null)
				{
					string strNumberType = GetNumberType(mData.IterationType);
					if (mData.Iteration == 0)
						mData.Iteration = 1;
					m_szRepeat = strNumberType + " " + (mData.Iteration) + "회";					
				}
				return m_szRepeat;
			}
			set
			{
				if (m_szRepeat != value)
				{
					if (mData != null)
					{						

						string[] str = value.Split(new char[] { ' ' });					
						string strType = System.Text.RegularExpressions.Regex.Replace(str[1], @"\D", "");	
						try
						{
							int nIter = int.Parse(strType);

                            if (m_bEnabledSnapshot == true)
							    UndoRedoManager.Instance.SaveSnapshot("단계 처리횟수 변경");
							mData.Iteration = nIter;
							mData.IterationType = SetNumberType(str[0]);

							m_szRepeat = value;
						}
						catch (Exception)
						{ }	
					}
				}				
			}
		}

		private string m_szProcessTime = "";
		[Category("\t일반")]
		[Browsable(false)]
		[DisplayName("처리 시간")]
		[Description("임무의 처리 시간을 지정합니다.")]
		[Editor(typeof(PeriodEditor), typeof(UITypeEditor))]
		[ReadOnly(false)]
		public string ProcessTime
		{
			get
			{
				if (mData != null)
				{				
					string strProcessType = GetProcessType(mData.ProcessTimeType);
					if (mData.ProcessTimeType == 5)
						m_szProcessTime = strProcessType;
					else
						m_szProcessTime = mData.ProcessTime + " " + strProcessType;
					
				}
				return m_szProcessTime;
			}
			set
			{
				
				if (m_szProcessTime != value)
				{
					if (mData != null)
					{
                        if (m_bEnabledSnapshot == true)
						    UndoRedoManager.Instance.SaveSnapshot("단계 처리시간 변경");

						string[] str = value.Split(new char[] { ' ' });
						string strType = System.Text.RegularExpressions.Regex.Replace(str[0], @"\D", "");

						mData.ProcessTimeType = ProcessType;
						if(ProcessType != 5)
						{
							try
							{
								mData.ProcessTime = int.Parse(strType);
								m_szProcessTime = value;
							}catch(Exception)
							{ }
						}
						else
						{
							m_szProcessTime = value;
						}
						
					}
				}
				
			}
		}

		[Browsable(false)]
		public int ProcessType
		{
			get { return m_nProcessType; }
			set 
			{ 
				m_nProcessType = value; 
			}
		}		

		private int m_nProcessType;
		private int m_nPeriodType;
		private DateTime m_dtBeginTime;
		private DateTime m_dtEndTime;
		private int m_nWeekDayOPtion;
		private int m_nNumberType;

		[Browsable(false)]	
		public int PeriodType //기간 타입
		{
			get { return m_nPeriodType; }
			set { m_nPeriodType = value; }
		}
		[Browsable(false)]	
		public DateTime BeginTime // 기간의 시작 시간
		{
			get { return m_dtBeginTime; }
			set { m_dtBeginTime = value; }
		}
		[Browsable(false)]	
		public DateTime EndTime // 기간의 끝 시간
		{
			get { return m_dtEndTime; }
			set { m_dtEndTime = value; }
		}
		[Browsable(false)]	
		public int WeekDayOPtion // 기간의 요일 옵션
		{
			get { return m_nWeekDayOPtion; }
			set { m_nWeekDayOPtion = value; }
		}

		[Browsable(false)]	
		public int NumberType
		{
			get { return m_nNumberType; }
			set { m_nNumberType = value; }
		}



		private string GetNumberType(int nType)
		{
			string[] strOption = { "전체기간중", "연중", "월중", "주중", "하루중", "시간당" };

			return strOption[nType];
		}

		private string GetProcessType(int nType)
		{
			string[] strOption = { "개월", "주", "일", "시간", "분", "알수없음" };

			return strOption[nType];
		}

		private int SetNumberType(string strNumberType)
		{
			string[] strOption = { "전체기간중", "연중", "월중", "주중", "하루중", "시간당" };
			int nIndex = 0;
			foreach (string strValue in strOption)
			{
				if (strValue == strNumberType)
					break;

				nIndex++;
			}

			return nIndex;
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
					if (int.TryParse(strProcessTime[0], out nTime))
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
			catch (Exception e)
			{
				System.Diagnostics.Trace.WriteLine(e.StackTrace);
			}
			return null;
		}

		private string GetPeriodData(Data_ActionStep data, int nType)
		{
			int nWeekDayOPtion = data.WeekdayOption;

			DateTime dtBegin = data.BeginTime;
			DateTime dtEnd = data.EndTime;

			string strBeginYear = dtBegin.Year.ToString();
			string strEndYear = dtEnd.Year.ToString();

			string strBeginMonth = string.Format("{0:D2}", dtBegin.Month);
			string strBeginDay = string.Format("{0:D2}", dtBegin.Day);

			string strBeginHour = string.Format("{0:D2}", dtBegin.Hour);
			string strBeginMinute = string.Format("{0:D2}", dtBegin.Minute);

			string strEndMonth = string.Format("{0:D2}", dtEnd.Month);
			string strEndDay = string.Format("{0:D2}", dtEnd.Day);

			string strEndHour = string.Format("{0:D2}", dtEnd.Hour); 
			string strEndMinute = string.Format("{0:D2}", dtEnd.Minute);

			string strValue = "";
			// 0(사용 안함), 1(날짜 옵션, n1월 n2일 ~ m1월 m2일까지), 2(시간 옵션, n1시 n2분 ~ m1월 m2일까지), 3(날짜 옵션 + 시간 옵션),
			// 기간 Type이 10보다 크거나 같으면 고정 년도 사용
			switch (nType)
			{
				case 0:
					strValue = "사용안함";
					break;
				case 1: // 날짜
					strValue = strBeginMonth + "/" + strBeginDay + " ~ " + strEndMonth + "/" + strEndDay;
					break;
				case 2: // 시간
					strValue = strBeginHour + ":" + strBeginMinute + " ~ " + strEndHour + ":" + strEndMinute;
					break;
				case 3: // 날짜 + 시간
					strValue = strBeginMonth + "/" + strBeginDay + " " + strBeginHour + ":" + strBeginMinute + " ~ " +
										strEndMonth + "/" + strEndDay + " " + strEndHour + ":" + strEndMinute;
					break;
				case 11: // 고정년도 + 1
					strValue = strBeginYear + "/" + strBeginMonth + "/" + strBeginDay + " ~ " + strEndYear + "/" + strEndMonth + "/" + strEndDay;
					break;
				case 12: // 고정년도 + 2
					strValue = strBeginYear + " " + strBeginHour + ":" + strBeginMinute + " ~ " + strEndYear + " " + strEndHour + ":" + strEndMinute;
					break;
				case 13: // 고정년도 + 3
					strValue = strBeginYear + "/" + strBeginMonth + "/" + strBeginDay + " " + strBeginHour + ":" + strBeginMinute + " ~ " +
										strEndYear + "/" + strEndMonth + "/" + strEndDay + " " + strEndHour + ":" + strEndMinute;
					break;
			}

			return strValue;
		}

		private string m_szParentActionStep = "";
		[Category("기타")]
		[Browsable(false)]
		[DisplayName("부모단계")]
		[Description("부모단계를 지정합니다.")]
		[TypeConverter(typeof(ActionStepConverter))]
		public string LinkSection
		{
			get
			{
				m_szParentActionStep = "";
				if (mData != null)
				{
					int nStep = mData.ParentStepID;
					Data_ActionStep parentStep = ActionStepDropDownList.FindActionStep(nStep);
					if( parentStep != null)
					{
						m_szParentActionStep = parentStep.StepName;
					}
				}
				return m_szParentActionStep;
			}
			set
			{
				if (m_szParentActionStep != value)
				{
					if (mData != null)
					{

						FormPageSOP pageLevel = FormMain.Instance.GetPageLevel();
						TabPage tabPage = pageLevel.TabControls.SelectedTab;
						if (tabPage != null)
						{
							if (ChangeTree(GetTabPage(value), value))
							{
								tabPage.Tag = GetTabPage(value);
								m_szParentActionStep = value;


								Data_ActionStep parentStep = ActionStepDropDownList.FindActionStepName(value);
								if (parentStep != null)
									mData.ParentStepID = parentStep.ID;
								else
									mData.ParentStepID = -1;

								m_szParentActionStep = value;
							}
							else
							{
								tabPage.Tag = GetTabPage(m_szParentActionStep);
								
							}
						}						
					}
				}				
			}
		}

		private TabPage GetTabPage(string strTabName)
		{
			FormPageSOP pageLevel = FormMain.Instance.GetPageLevel();
			foreach (TabPage tabPage in pageLevel.TabControls.TabPages)
			{
				if (tabPage.Text == strTabName)
					return tabPage;
			}

			return null;
		}
		
		private bool ChangeTree(TabPage tabParent, string szParentName)
		{
			FormPageSOP pageLevel = FormMain.Instance.GetPageLevel();
			TabPage tabPage = pageLevel.TabControls.SelectedTab;

			BarLevelTree treeForm = pageLevel.GetBarLevelTree();
			if (tabParent == null)
			{				
				if (tabPage.Tag == null)
					return false;

				// 노드를 disaster 밑으로
				TreeNode cNode = treeForm.FindNode(tabPage.Text);
				if( cNode.Level != 3)
				{
					string strValue = SopDocManager.Instance.DisasterName;
					UndoRedoManager.Instance.SaveSnapshot();

					TreeNode pNode = treeForm.FindNode(strValue);
					if (!treeForm.SetChildNode(pNode, cNode))
						return false;
					treeForm.SelectNode(cNode);
				}
			}
			else
			{
				if (tabPage.Tag == tabParent)
					return false;
				// 노드를 부모 밑으로
				TreeNode cNode = treeForm.FindNode(tabPage.Text);
				TreeNode pNode = treeForm.FindNode(szParentName);
				if (cNode == null || pNode == null)
					return false;

				UndoRedoManager.Instance.SaveSnapshot();

				if (!treeForm.SetChildNode(pNode, cNode))
					return false;
				treeForm.SelectNode(cNode);
			}
			return true;
		}
		
	}

	internal class ActionStepDropDownList
	{
		internal static ArrayList ComponentList = new ArrayList();
		internal static ArrayList OrginalList = new ArrayList();

		internal static void UpdateList()
		{
			ComponentList.Clear();
			OrginalList.Clear();


			ArrayList actionSteps = FormMain.Instance.GetPageLevel().AllComponentList();

			FormPageSOP pageLevel = FormMain.Instance.GetPageLevel();
			ActionStepTabPage currentTab = (ActionStepTabPage)pageLevel.TabControls.SelectedTab;
			
			ArrayList arr = pageLevel.GetTabPages();			

			BarLevelTree treeForm = pageLevel.GetBarLevelTree();
			TreeNode node = treeForm.FindNode(currentTab.Text);
			
			ComponentList.Add("");
			OrginalList.Add(null);
			
			foreach (ActionStepTabPage tabPage in arr)
			{

				if (currentTab != tabPage && tabPage.Tag != currentTab)
				{

					if (treeForm.CheckPathChildNode(node, tabPage.Text))
					{
						ComponentList.Add(tabPage.Text);
						OrginalList.Add(tabPage.Data);					
					}
				}
			}			
		}

		internal static Data_ActionStep FindActionStep(int nActionStepID)
		{
			foreach (Data_ActionStep step in OrginalList)
			{

				if (step != null && step.ID == nActionStepID)
				{
					return step;
				}
			}
			return null;
		}

		internal static Data_ActionStep FindActionStepName(string szCompID)
		{
			foreach (Data_ActionStep step in OrginalList)
			{
				if (step != null && step.StepName == szCompID)
				{
					return step;
				}
			}
			return null;
		}
	}

	public class ActionStepConverter : StringConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override System.ComponentModel.TypeConverter.StandardValuesCollection
			   GetStandardValues(ITypeDescriptorContext context)
		{
			ActionStepDropDownList.UpdateList();
			return new StandardValuesCollection(ActionStepDropDownList.ComponentList);
		}
	}

}
