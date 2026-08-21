using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XtremePropertyGrid;
using System.Collections;

namespace SOPManager
{
    public partial class PropertiesProcess : Form
    {
        private Sections.SectionProcess m_section;

        private PropertyGridItem m_itemID = null;
        private PropertyGridItem m_itemMission = null;
        private PropertyGridItem m_itemProcessing = null;
        private PropertyGridItem m_itemCheck = null;
        private PropertyGridItem m_itemTeamList = null;
        private PropertyGridItem m_itemMessage = null;
        private PropertyGridItem m_itemRecive = null;

        private ArrayList m_arrSelected = new ArrayList();
        public ArrayList ArrSelected
        {
            get { return m_arrSelected; }
            set { m_arrSelected = value; }
        }

        private Mission m_mission;
        public Mission Mission
        {
            get { return m_mission; }
            set { m_mission = value; }
        }

        private string m_strCheckItem;
        public string CheckItem
        {
            get { return m_strCheckItem; }
            set { m_strCheckItem = value; }
        }

        private string m_strSelectedTeam;
        public string SelectedTeam
        {
            get { return m_strSelectedTeam; }
            set { m_strSelectedTeam = value; }
        }

        private string m_strTransTime;
        public string TransTime
        {
            get { return m_strTransTime; }
            set { m_strTransTime = value; }
        }

        private int m_nProcessType;
        public int ProcessType
        {
            get { return m_nProcessType; }
            set { m_nProcessType = value; }
        }

        private int m_nTime;
        public int Time
        {
            get { return m_nTime; }
            set { m_nTime = value; }
        }

        public PropertiesProcess()
        {
            InitializeComponent();
            m_mission = new Mission();
            //m_mission.ArrMission = new ArrayList();
            InitProcess();
        }

        // Process 속성
        private void InitProcess()
        {
            PropertyGridItem CategoryNormal = axPropertyGrid.AddCategory("일반");
            m_itemID = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "ID", "");
            m_itemID.ReadOnly = true;

            m_itemMission = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "임무내용", "");
            m_itemMission.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
            m_itemMission.Id = ID.ID_ITEM_MISSION;

            m_itemProcessing = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "처리시간", "");
            m_itemProcessing.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
            m_itemProcessing.Id = ID.ID_ITEM_PROCESSING;

            m_itemTeamList = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "팀 List", "");
            m_itemTeamList.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
            m_itemTeamList.Id = ID.ID_ITEM_TEAMLIST;
            CategoryNormal.Expanded = true;

            PropertyGridItem CategoryEtc = axPropertyGrid.AddCategory("임무전달");
            m_itemMessage = CategoryEtc.AddChildItem(PropertyItemType.PropertyItemEnum, "임무메시지", 1);
            m_itemMessage.Id = ID.ID_ITEM_MISSION_MESSAGE;
            m_itemMessage.Constraints.Add("사용", 1);
            m_itemMessage.Constraints.Add("사용안함", 2);
            m_itemMessage.ReadOnly = true;

            m_itemRecive = CategoryEtc.AddChildItem(PropertyItemType.PropertyItemEnum, "수신범위", 2);
            m_itemRecive.Id = ID.ID_ITEM_RECIVE_RANGE;
            m_itemRecive.Constraints.Add("팀장에게만 전송", 1);
            m_itemRecive.Constraints.Add("팀 전체에게 전송", 2);
            m_itemRecive.ReadOnly = true;
            CategoryEtc.Expanded = true;
        }

        private void axPropertyGrid_InplaceButtonDown(object sender, AxXtremePropertyGrid._DPropertyGridEvents_InplaceButtonDownEvent e)
        {
            switch (e.button.Item.Id)
            {
                case ID.ID_ITEM_MISSION:
                    PopupMission popupMission = new PopupMission();
                    popupMission.InitText(m_section);
                    if (popupMission.ShowDialog() == DialogResult.OK)
                    {
						
                        e.button.Item.Value = m_mission.Title; // m_strMission;
                    }
                    break;
                case ID.ID_ITEM_PROCESSING:
                    PopupProcessTime popupProcTime = new PopupProcessTime();
                    popupProcTime.ItemID = ID.ID_ITEM_PROCESSING;
                    popupProcTime.GetProcessTime(m_strTransTime);
                    if (popupProcTime.ShowDialog() == DialogResult.OK)
                    {
                        e.button.Item.Value = m_strTransTime;
                    }
                    break;
                case ID.ID_ITEM_TEAMLIST:
                    PopupSelectTeam popupSelectTeam = new PopupSelectTeam(m_section);
                    popupSelectTeam.GetSection(m_section);
                    if (popupSelectTeam.ShowDialog() == DialogResult.OK)
                    {
						
                        e.button.Item.Value = m_strSelectedTeam;
                        Sections.SOPTeam team = new Sections.SOPTeam();
                    }
                    break;
            }
        }
        
        private void axPropertyGrid_ValueChanged(object sender, AxXtremePropertyGrid._DPropertyGridEvents_ValueChangedEvent e)
        {
            Sections.SectionDataProcess data = (Sections.SectionDataProcess)m_section.Data;
			
            switch (e.item.Id)
            {					
                case ID.ID_ITEM_MISSION:					
                    m_mission.Title = e.item.Value.ToString();
                    SetSectionUpText();
                    break;
                case ID.ID_ITEM_MISSION_MESSAGE:
                    bool isTransfer = false;
                    if(e.item.MaskedText == "사용")
                        isTransfer = true;
					
					if (data.MissionTransfer != isTransfer)
					{
						UndoRedoManager.Instance.SaveSnapshot();
						data.MissionTransfer = isTransfer;
					}
                    break;
                case ID.ID_ITEM_RECIVE_RANGE:
                    bool isRecive = false;
                    if(e.item.MaskedText == "팀장에게만 전송")
                        isRecive = true;
					if (data.TransferTeamLeaderOnly != isRecive)
					{
						UndoRedoManager.Instance.SaveSnapshot();
						data.TransferTeamLeaderOnly = isRecive;
					}                   
                    break;
            }
        }

        // section 선택시 호출되는 함수
        public void GetSectionData(Sections.SectionProcess section)
        {
			if (section == null)
			{
				// section이 갖고 있는 정보를 속성창에 출력
				m_itemID.Value = "";
				m_itemMission.Value = "";
				m_itemProcessing.Value = ""; // 처리시간
				m_itemTeamList.Value = "";
				m_itemMessage.MaskedText = "사용"; // 임무 메시지				
				m_itemMessage.Selected = true;
				m_itemRecive.MaskedText = "팀 전체에게 전송"; // 수신범위
				m_itemRecive.Selected = true;
			   
			}
			else
			{
				m_section = section;
				Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;

				// section이 갖고 있는 정보를 속성창에 출력
				m_itemID.Value = data.ComponentID;
				m_itemMission.Value = section.Title; // 임무내용

				m_itemProcessing.Value = GetProcessTime(); // 처리시간
				//m_itemCheck.Value       = GetCheckItem(); //m_strCheckItem; // 점검항목
				m_itemTeamList.Value = section.TextDown; // 팀List

				if (data.MissionTransfer)
					m_itemMessage.MaskedText = "사용"; // 임무 메시지
				else
					m_itemMessage.MaskedText = "사용안함";

				if (data.TransferTeamLeaderOnly)
					m_itemRecive.MaskedText = "팀장에게만 전송"; // 수신범위
				else
					m_itemRecive.MaskedText = "팀 전체에게 전송";
			}           
        }
        
        public string GetProcessTime()
        {
            string[] strType = { "개월", "주", "일", "시간", "분", "사용안함"};
            string strValue = "";
            Sections.SectionDataProcess data = (Sections.SectionDataProcess)m_section.Data;
            
            switch (data.ProcessingTime.ProcessingType)
            {
                case Sections.ProcessingTime.Type.MONTH:
                    strValue = strType[0];
                    m_nProcessType = 0;
                    break;
                case Sections.ProcessingTime.Type.WEEK:
                    strValue = strType[1];
                    m_nProcessType = 1;
                    break;
                case Sections.ProcessingTime.Type.DAY:
                    strValue = strType[2];
                    m_nProcessType = 2;
                    break;
                case Sections.ProcessingTime.Type.HOUR:
                    strValue = strType[3];
                    m_nProcessType = 3;
                    break;
                case Sections.ProcessingTime.Type.MINUTE:
                    strValue = strType[4];
                    m_nProcessType = 4;
                    break;
                case Sections.ProcessingTime.Type.UNKNOWN:
                    strValue = strType[5];
                    m_nProcessType = 5;
                    break;
            }

            string strProcessTime;
            if (data.ProcessingTime.ProcessingType == Sections.ProcessingTime.Type.UNKNOWN)
            {
                strProcessTime = "사용안함";
                m_nTime = 1;
            }
            else
            {
                strProcessTime = data.ProcessingTime.Time.ToString() + " " + strValue;
                m_nTime = data.ProcessingTime.Time;
            }
            return strProcessTime;
        }

        public string GetCheckItem()
        {
            Sections.SectionDataProcess data = (Sections.SectionDataProcess)m_section.Data;
            foreach (Sections.CheckedItem item in data.CheckedItems)
            {
                return item.Item;
            }

            return null;
        }

        // Section에 Data 입력
        public void SetSectionUpText()
        {
			if (!m_section.Title.Equals(m_mission.Title))
			{
				UndoRedoManager.Instance.SaveSnapshot();
				m_itemMission.Value = m_section.Title = m_mission.Title; //m_strMission;
				Control ctrl = m_section.GetParent();
				ctrl.Refresh();
			}            
        }

        public void SetSectionDownText()
        {
			m_section.TextDown = m_strSelectedTeam;
			Control ctrl = m_section.GetParent();
			ctrl.Refresh();			            
        }
   
        public void SetTransTime(int nType)
        {
            Sections.SectionDataProcess data = (Sections.SectionDataProcess)m_section.Data;
            Sections.ProcessingTime processTime = new Sections.ProcessingTime();
            string strTime = System.Text.RegularExpressions.Regex.Replace(m_strTransTime, @"\D", "");
            int nTime = int.Parse(strTime);
			
			if (processTime.Time != nTime || data.ProcessingTime != processTime)
			{
				UndoRedoManager.Instance.SaveSnapshot();
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
                    break;
            }

            processTime.Time = nTime;
            data.ProcessingTime = processTime;
        }
    }
}
