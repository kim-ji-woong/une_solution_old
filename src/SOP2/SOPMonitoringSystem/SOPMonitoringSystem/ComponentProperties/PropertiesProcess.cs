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

namespace SOPMonitoringSystem
{
    public partial class PropertiesProcess : Form
    {
        private Sections.SectionProcess m_section;

        private PropertyGridItem m_itemID = null;
        private PropertyGridItem m_itemMission = null;
        private PropertyGridItem m_itemProcessing = null;
        //private PropertyGridItem m_itemCheck = null;
        private PropertyGridItem m_itemTeamList = null;
        private PropertyGridItem m_itemMessage = null;
        private PropertyGridItem m_itemRecive = null;


        private string m_strCheckItem;
        public string CheckItem
        {
            get { return m_strCheckItem; }
            set { m_strCheckItem = value; }
        }

        public PropertiesProcess()
        {
            InitializeComponent();

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
            m_itemProcessing.Id = ID.ID_ITEM_PROCESSING;

            //m_itemCheck = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "점검항목", "");
            //m_itemCheck.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
            //m_itemCheck.Id = ID.ID_ITEM_CHECKITEM;

            m_itemTeamList = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "팀 List", "");
            m_itemTeamList.Id = ID.ID_ITEM_TEAMLIST;
            CategoryNormal.Expanded = true;

            PropertyGridItem CategoryEtc = axPropertyGrid.AddCategory("임무전달");
            m_itemMessage = CategoryEtc.AddChildItem(PropertyItemType.PropertyItemString, "임무메시지", "");
            m_itemMessage.Id = ID.ID_ITEM_MISSION_MESSAGE;

            m_itemRecive = CategoryEtc.AddChildItem(PropertyItemType.PropertyItemString, "수신범위", "");
            m_itemRecive.Id = ID.ID_ITEM_RECIVE_RANGE;
            CategoryEtc.Expanded = true;
        }

        // section 선택시 호출되는 함수
        public void GetSectionData(Sections.SectionProcess section)
        {
            m_section = section;
            Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;

            // section이 갖고 있는 정보를 속성창에 출력
            m_itemID.Value          = data.ComponentID;  
            m_itemMission.Value     = section.TextUP; // 임무내용
            m_itemProcessing.Value  = GetProcessTime(); // 처리시간
            //m_itemCheck.Value       = GetCheckItem(); // 점검항목
            m_itemTeamList.Value    = section.TextDown; // 팀List

            if(data.MissionTransfer)
                m_itemMessage.MaskedText = "사용"; // 임무 메시지
            else
                m_itemMessage.MaskedText = "사용안함";

            if (data.TransferTeamLeaderOnly)
                m_itemRecive.MaskedText = "팀장에게만 전송"; // 수신범위
            else
                m_itemRecive.MaskedText = "팀 전체에게 전송";
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

        private string GetCheckItem()
        {
            Sections.SectionDataProcess data = (Sections.SectionDataProcess)m_section.Data;
            
            foreach (Sections.CheckedItem check in data.CheckedItems)
            {
                m_strCheckItem = check.Item;
                return check.Item;
            }

            return null;
        }

        private void axPropertyGrid_InplaceButtonDown(object sender, AxXtremePropertyGrid._DPropertyGridEvents_InplaceButtonDownEvent e)
        {
            switch (e.button.Item.Id)
            {
                case ID.ID_ITEM_CHECKITEM:
                    PopupCheckItem popupCheckItem = new PopupCheckItem();
                    popupCheckItem.GetCheckItem(m_section);
                    if (popupCheckItem.ShowDialog() == DialogResult.OK)
                    {
                        e.button.Item.Value = m_strCheckItem;
                    }
                    break;
                case ID.ID_ITEM_MISSION:
                    PopupMission popupMission = new PopupMission();
                    popupMission.InitText(m_section);
                    if (popupMission.ShowDialog() == DialogResult.OK)
                    {
                        e.button.Item.Value = m_section.Title; //m_mission.Title; // m_strMission;
                    }
                    break;
            }
        }

    }
}
