using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using XtremePropertyGrid;

namespace SOPManager
{
    public partial class PropertiesLevel : Form
    {
        private PropertyGridItem m_itemLevel;
        private PropertyGridItem m_itemPeriod;
        private PropertyGridItem m_itemNumber;
        private PropertyGridItem m_itemProcessTime;
        private PropertyGridItem m_itemParent;

        private ArrayList m_arrLevelProperties = new ArrayList();
        public ArrayList LevelProperties
        {
            get { return m_arrLevelProperties; }
            set { m_arrLevelProperties = value; }
        }

        private string m_strParent;
        private string m_strLevelName;

        private string m_strTerm;
        private int m_nPeriodType;
        private DateTime m_dtBeginTime;
        private DateTime m_dtEndTime;
        private int m_nWeekDayOPtion;
        private string m_strNumber;
        private int m_nNumberType;
        private string m_strProcessTime;
        private int m_nProcessType;

        public string Number // 횟수
        {
            get { return m_strNumber; }
            set { m_strNumber = value; }
        }

        public int NumberType
        {
            get { return m_nNumberType; }
            set { m_nNumberType = value; }
        }

        public string Term // 기간
        {
            get { return m_strTerm; }
            set { m_strTerm = value; }
        }

        public int PeriodType //기간 타입
        {
            get { return m_nPeriodType; }
            set { m_nPeriodType = value; }
        }

        public DateTime BeginTime // 기간의 시작 시간
        {
            get { return m_dtBeginTime; }
            set { m_dtBeginTime = value; }
        }

        public DateTime EndTime // 기간의 끝 시간
        {
            get { return m_dtEndTime; }
            set { m_dtEndTime = value; }
        }
        
        public int WeekDayOPtion // 기간의 요일 옵션
        {
            get { return m_nWeekDayOPtion; }
            set { m_nWeekDayOPtion = value; }
        }

        public string ProcessTime // 처리시간
        {
            get { return m_strProcessTime; }
            set { m_strProcessTime = value; }
        }

        public int ProcessType
        {
            get { return m_nProcessType; }
            set { m_nProcessType = value; }
        }

        public PropertiesLevel()
        {
            InitializeComponent();
            Init();

            ClearProperties();
        }

        PropertyGridItem CategoryEtc = null;

        private void Init()
        {
            PropertyGridItem CategoryNormal = axPropertyGrid.AddCategory("일반");
            m_itemLevel = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "단계명", "");
            m_itemLevel.Id = ID.ID_ITEM_LEVEL;

            m_itemPeriod = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "기간", "");
            m_itemPeriod.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
            m_itemPeriod.Id = ID.ID_ITEM_PERIOD;

            m_itemNumber = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "횟수", "");
            m_itemNumber.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
            m_itemNumber.Id = ID.ID_ITEM_NUMBER;

            m_itemProcessTime = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "처리시간", "");
            m_itemProcessTime.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
            m_itemProcessTime.Id = ID.ID_ITEM_PROCESS_TIME;
            CategoryNormal.Expanded = true;

            //PropertyGridItem CategoryEtc = axPropertyGrid.AddCategory("기타");
            CategoryEtc = axPropertyGrid.AddCategory("기타");
            m_itemParent = CategoryEtc.AddChildItem(PropertyItemType.PropertyItemEnum, "부모 단계", 1);
            m_itemParent.Id = ID.ID_ITEM_PARENT;
            CategoryEtc.Expanded = true;
        }

        public void ClearProperties()
        {
            //m_itemLevel.Value = "";
            m_itemPeriod.Value = "";
            m_itemNumber.Value = "";
            m_itemProcessTime.Value = "";
            m_itemParent.Value = "";
        }

        public void AddTitle(string strValue)
        {
            labelTitle.Text = strValue;
			
        }

        private bool CheckDuplicateTabName(TabPage page, string strNewName)
        {
			FormPageSOP pageLevel = FormMain.Instance.GetPageLevel();

            foreach (TabPage tabPage in pageLevel.TabControls.TabPages)
            {
                if (tabPage == page)
                    continue;

                if (tabPage.Text == strNewName)
                {
                    MessageBox.Show("이미 같은 이름이 존재합니다.");
                    m_itemLevel.Value = page.Text;
                    return false;
                }
            }

            return true;
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

        private void axPropertyGrid_ValueChanged(object sender, AxXtremePropertyGrid._DPropertyGridEvents_ValueChangedEvent e)
        {
			FormPageSOP pageLevel = FormMain.Instance.GetPageLevel();
            TabPage tabPage = pageLevel.TabControls.SelectedTab;
            switch(e.item.Id)
            {
                case ID.ID_ITEM_LEVEL:
                    string strValue = (string)e.item.Value;
                    if (!CheckDuplicateTabName(tabPage, strValue))
                        return;

					UndoRedoManager.Instance.SaveSnapshot();

                    pageLevel.OldTabPageText = tabPage.Text;
                    tabPage.Text = (string)e.item.Value;
                    pageLevel.GetBarLevelTree().ChangeLevelText(strValue);
                    ChangeComponentID(tabPage);

                    int nIndex = labelTitle.Text.LastIndexOf('/');
                    string strPath = labelTitle.Text.Substring(0, nIndex + 1) + strValue;
                    labelTitle.Text = strPath;

                    foreach(Data_ActionStep data in m_arrLevelProperties)
                    {
                        if (pageLevel.OldTabPageText == data.StepName)
                        {
                            data.StepName = tabPage.Text;
                            break;
                        }
                    }
                    break;
                case ID.ID_ITEM_PARENT:

					
					if (ChangeTree(GetTabPage(e.item.MaskedText), e.item.MaskedText))
					{
						tabPage.Tag = GetTabPage(e.item.MaskedText);
						m_strParent = m_itemParent.MaskedText = e.item.MaskedText;
					}
					else
					{
						tabPage.Tag = GetTabPage(m_strParent);
						m_itemParent.MaskedText = e.item.MaskedText = m_strParent;
						m_itemParent.Selected = true;
					}

                    break;
            }
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
					FormNewSOP pageDisaster = FormMain.Instance.GetPageDisaster();
					string strValue = pageDisaster.SelectedDetailCategory;

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

        private void axPropertyGrid_InplaceButtonDown(object sender, AxXtremePropertyGrid._DPropertyGridEvents_InplaceButtonDownEvent e)
        {
            switch (e.button.Item.Id)
            {
                case ID.ID_ITEM_PERIOD: // 기간
                    PopupProcessTerm popupTerm = new PopupProcessTerm();
                    popupTerm.SetTerm(m_strTerm);
                    if (popupTerm.ShowDialog() == DialogResult.OK)
                    {
                        e.button.Item.Value = m_strTerm;
                        LevelOptionList(ID.ID_ITEM_PERIOD, m_strTerm);
                    }
                    break;
                case ID.ID_ITEM_NUMBER:
                    PopupProcessNumber popupNumber = new PopupProcessNumber();
                    m_nNumberType = popupNumber.SetNumberType(m_strNumber);
                    if (popupNumber.ShowDialog() == DialogResult.OK)
                    {
                        e.button.Item.Value = m_strNumber;
                        LevelOptionList(ID.ID_ITEM_NUMBER, m_strNumber);
                    }
                    break;
                case ID.ID_ITEM_PROCESS_TIME:
                    PopupProcessTime popupProcessTime = new PopupProcessTime();
                    popupProcessTime.ItemID = ID.ID_ITEM_PROCESS_TIME;
                    popupProcessTime.SetProcessingTime(m_strProcessTime);
                    if (popupProcessTime.ShowDialog() == DialogResult.OK)
                    {
                        e.button.Item.Value = m_strProcessTime;
                        LevelOptionList(ID.ID_ITEM_PROCESS_TIME, m_strProcessTime);
                    }
                    break;
                case ID.ID_ITEM_PARENT:
                    AddParentName();
                    break;
            }
        }

        public void AddParentName()
        {
            m_itemParent.Constraints.Clear();

			FormPageSOP pageLevel = FormMain.Instance.GetPageLevel();
            TabPage currentTab = pageLevel.TabControls.SelectedTab;
            ArrayList arr = pageLevel.GetTabPages();
            m_itemParent.Constraints.Add(" ", 1);
            int i = 2;

			BarLevelTree treeForm = pageLevel.GetBarLevelTree();			
			TreeNode node = treeForm.FindNode(currentTab.Text);

            foreach (TabPage tabPage in arr)
            {
				
                if (currentTab != tabPage && tabPage.Tag != currentTab)
                {

					if (treeForm.CheckPathChildNode(node, tabPage.Text))
					{
						m_itemParent.Constraints.Add(tabPage.Text, i);
						i++;
					}
                }
            }
            this.Refresh();
        }

        // tabPage : 텍스트가 변경된 TabPage
        private void ChangeComponentID(TabPage tabPage)
        {
            FormMain frmMain = FormMain.Instance;
			FormPageSOP page = frmMain.GetPageLevel();

            if (page.OldTabPageText == tabPage.Text)
                return;

            Type type = typeof(Sections.PanelSectionEx);

            foreach (Control control in tabPage.Controls)
            {
                if (control.GetType() == type)
                {
                    Sections.PanelSectionEx panel = (Sections.PanelSectionEx)control;
                    panel.ResetComponentID(page.OldTabPageText, tabPage.Text);
                }
            }
        }
        
        public void SetSelectedTabName(string strTabName)
        {
            m_itemLevel.Value = strTabName;
        }

        public void GetLevelProperties(TabPage page)
        {
            m_strLevelName = page.Text;

            bool isOpen = false;
            
            //int nDisasterID = 0;
            ArrayList arr = new ArrayList();
            if (isOpen)
                arr = FormMain.Instance.ActionStep;
            else
                arr = m_arrLevelProperties;

            bool isAdd = false;
            if(arr.Count == 0)
            {
                InitLevelProperties(true);
            }

            foreach (Data_ActionStep data in arr)
            {
                if (data.StepName == page.Text /* && nDisasterID == data.DisasterID*/)
                {
                    // 기간
                    int nType = data.PeriodType;
                    int nWeekday = data.WeekdayOption;
                    m_itemPeriod.Value = GetPeriodData(data, nType);
                    m_strTerm = m_itemPeriod.Value.ToString();
                    m_nPeriodType = nType;

                    // 횟수
                    string strNumberType = GetNumberType(data.IterationType);
                    if (data.Iteration == 0)
                        data.Iteration = 1;
                    m_itemNumber.Value = strNumberType + " " + (data.Iteration) + "회";
                    m_strNumber = m_itemNumber.Value.ToString();

                    string strProcessType = GetProcessType(data.ProcessTimeType);
                    if (data.ProcessTimeType == 5)
                        m_itemProcessTime.Value = strProcessType;
                    else
                        m_itemProcessTime.Value = data.ProcessTime + " " + strProcessType;

                    m_strProcessTime = m_itemProcessTime.Value.ToString();

                    if (page.Tag != null)
                        m_strParent = m_itemParent.MaskedText = ((TabPage)page.Tag).Text;
                    else
                        m_strParent = m_itemParent.MaskedText = "";

                    m_itemParent.Selected = true;

                    /*foreach (TabPage tabPage in FormMain.Instance.GetPageLevel().TabControls.TabPages)
                    {
                        if ((int)tabPage.Tag == data.ParentStepID)
                        {
                            m_strParent = m_itemParent.MaskedText = tabPage.Text;
                            break;
                        }
                    }*/
                    isAdd = true;
                }
            }
            if(!isAdd)
                InitLevelProperties(false);
        }

        public void InitLevelProperties(bool isOpen)
        {
            if(isOpen)
                m_itemLevel.Value = "";
            else
                m_itemLevel.Value = m_strLevelName;

            //기간
            m_strTerm = "";
            m_nPeriodType = 0;
            m_nWeekDayOPtion = 127;
            m_itemPeriod.Value = "사용안함";

            // 횟수
            string strNumberType = GetNumberType(0);
            m_itemNumber.Value = strNumberType + " 1회";
            m_strNumber = m_itemNumber.Value.ToString();

            //처리시간
            string strProcessType = GetProcessType(5);
            m_itemProcessTime.Value = strProcessType;
            m_strProcessTime = m_itemProcessTime.Value.ToString();

            m_itemParent.Constraints.Clear();
            
            m_strParent = m_itemParent.MaskedText = "";
            m_itemParent.Selected = true;             
        }

        private string GetPeriodData(Data_ActionStep data, int nType)
        {
            m_nWeekDayOPtion = data.WeekdayOption;

            DateTime dtBegin = data.BeginTime;
            DateTime dtEnd = data.EndTime;

            m_dtBeginTime = dtBegin;
            m_dtEndTime = dtEnd;

            string strBeginYear = dtBegin.Year.ToString();
            string strEndYear = dtEnd.Year.ToString();

            string strBeginMonth = dtBegin.Month.ToString();
            string strBeginDay = dtBegin.Day.ToString();

            string strBeginHour = dtBegin.Hour.ToString();
            string strBeginMinute = dtBegin.Minute.ToString();

            string strEndMonth = dtEnd.Month.ToString();
            string strEndDay = dtEnd.Day.ToString();

            string strEndHour = dtEnd.Hour.ToString();
            string strEndMinute = dtEnd.Minute.ToString();

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

        public Data_ActionStep SavePropertiesLevel(TabPage page)
        {
            foreach (Data_ActionStep data in m_arrLevelProperties)
            {
                if(data.StepName == page.Text)
                {
                    //string strIteration = System.Text.RegularExpressions.Regex.Replace(Number, @"\D", "");
                    //int nIteration = int.Parse(strIteration);

                    //string strProcessTime = System.Text.RegularExpressions.Regex.Replace(ProcessTime, @"\D", "");
                    //if(strProcessTime == "")
                    //    strProcessTime = "0";
                    //int nProcessTime = int.Parse(strProcessTime);

                    ////Data_ActionStep data = new Data_ActionStep();
                    //data.StepName = page.Text;
                    //data.PeriodType = m_nPeriodType;
                    //data.BeginTime = m_dtBeginTime;
                    //data.EndTime = m_dtEndTime;
                    //data.WeekdayOption = m_nWeekDayOPtion;

                    //data.Iteration = nIteration;
                    //data.IterationType = NumberType;

                    //data.ProcessTime = nProcessTime;
                    //data.ProcessTimeType = ProcessType;

                    return data;
                }
            }
            return null;
        }

        public string GetNumberType(int nType)
        {
            string[] strOption = { "전체기간중", "연중", "월중", "주중", "하루중", "시간당" };

            return strOption[nType];
        }

        public string GetProcessType(int nType)
        {
            string[] strOption = { "개월", "주", "일", "시간", "분", "알수없음" };

            return strOption[nType];
        }
        
        public int SetNumberType(string strNumberType)
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
        
        private void LevelOptionList(int nID, string strValue)
        {
			bool bChangedData = false;
            TabPage tabPage = FormMain.Instance.GetPageLevel().TabControls.SelectedTab;
            foreach (Data_ActionStep data in m_arrLevelProperties)
            {
                if (data.StepName == tabPage.Text)
                {
                    switch (nID)
                    {
                        case ID.ID_ITEM_PERIOD: //기간
							if (data.PeriodType != m_nPeriodType)
								bChangedData = true;
                            data.PeriodType = m_nPeriodType;

							if (data.BeginTime != m_dtBeginTime)
								bChangedData = true;
                            data.BeginTime = m_dtBeginTime;

							if (data.EndTime != m_dtEndTime)
                            data.EndTime = m_dtEndTime;
                            data.WeekdayOption = m_nWeekDayOPtion;
                            break;
                        case ID.ID_ITEM_NUMBER: //횟수
                            string[] str = m_strNumber.Split(new char[] { ' ' });
                            string strType = System.Text.RegularExpressions.Regex.Replace(str[1], @"\D", "");
                            data.Iteration = int.Parse(strType);
                            data.IterationType = SetNumberType(str[0]);
                            break;
                        case ID.ID_ITEM_PROCESS_TIME: //처리기간
                            str = m_strProcessTime.Split(new char[] { ' ' });
                            strType = System.Text.RegularExpressions.Regex.Replace(str[0], @"\D", "");
                            data.ProcessTime = int.Parse(strType);
                            data.ProcessTimeType = m_nProcessType;
                            break;
                        case ID.ID_ITEM_PARENT: //부모단계
                            //data.ParentStepID = (int)tabPage.Tag;
                            break;
                    }
                }
            }

        }
    }
}
