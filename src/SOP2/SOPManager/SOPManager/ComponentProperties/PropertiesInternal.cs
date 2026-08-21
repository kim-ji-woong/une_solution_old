using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XtremePropertyGrid;

namespace SOPManager
{
    public partial class PropertiesInternal : Form
    {
        private Sections.SectionInternal m_section;

        PropertyGridItem m_itemID = null;
        PropertyGridItem m_itemDescription = null;
        PropertyGridItem m_itemPopup = null;
        PropertyGridItem m_itemMobile = null;
        PropertyGridItem m_itemBroadcast = null;
        PropertyGridItem m_itemMessage = null;

        private string m_strDescription;
        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public PropertiesInternal()
        {
            InitializeComponent();

            InitInternal();
        }

        // 내부 상황전파 속성
        private void InitInternal()
        {
            PropertyGridItem CategoryNormal = axPropertyGrid.AddCategory("일반");
            m_itemID = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "ID", "");
            m_itemID.ReadOnly = true;

            m_itemDescription = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "표시내용", "");
            m_itemDescription.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
            m_itemDescription.Id = ID.ID_ITEM_INTERNAL_DESC;
            CategoryNormal.Expanded = true;

            PropertyGridItem CategoryEtc = axPropertyGrid.AddCategory("상황전파");
            m_itemPopup = CategoryEtc.AddChildItem(PropertyItemType.PropertyItemEnum, "팝업메시지", 1);
            m_itemPopup.Constraints.Add("사용", 1);
            m_itemPopup.Constraints.Add("사용안함", 2);
            m_itemPopup.Id = ID.ID_ITEM_POPUP;
            m_itemPopup.ReadOnly = true;

            m_itemMobile = CategoryEtc.AddChildItem(PropertyItemType.PropertyItemEnum, "모바일 APP", 1);
            m_itemMobile.Constraints.Add("사용", 1);
            m_itemMobile.Constraints.Add("사용안함", 2);
            m_itemMobile.Id = ID.ID_ITEM_MOBILE;

            m_itemBroadcast = CategoryEtc.AddChildItem(PropertyItemType.PropertyItemEnum, "사내 방송", 1);
            m_itemBroadcast.Constraints.Add("사용", 1);
            m_itemBroadcast.Constraints.Add("사용안함", 2);
            m_itemBroadcast.Id = ID.ID_ITEM_BRODCAST;

            m_itemMessage = m_itemBroadcast.AddChildItem(PropertyItemType.PropertyItemString, "메세지", "");
            m_itemMessage.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
            m_itemMessage.Id = ID.ID_ITEM_BRODCAST_MESSAGE;
            m_itemBroadcast.Expanded = true;

            CategoryEtc.Expanded = true;
        }

        private void axPropertyGrid_InplaceButtonDown(object sender, AxXtremePropertyGrid._DPropertyGridEvents_InplaceButtonDownEvent e)
        {
            switch(e.button.Item.Id)
            {
                case ID.ID_ITEM_INTERNAL_DESC:
                    PopupNote popupNote = new PopupNote();
                    popupNote.InitText(e.button.Item.Id);
                    if (popupNote.ShowDialog() == DialogResult.OK)
                    {
                        e.button.Item.Value = m_strDescription;
                    }
                    break;

                case ID.ID_ITEM_BRODCAST_MESSAGE:
                    PopupBroadcastMessage popup = new PopupBroadcastMessage();
                    Sections.SectionDataInternal data = (Sections.SectionDataInternal)m_section.Data;
                    popup.InitText(data.BroadcastMessage);

                    if (popup.ShowDialog() == DialogResult.OK)
                    {
                        string szText = popup.GetMessage();
                        data.BroadcastMessage = szText;
                        e.button.Item.Value = szText;
                    }

                    break;
            }
        }

        private void axPropertyGrid_ValueChanged(object sender, AxXtremePropertyGrid._DPropertyGridEvents_ValueChangedEvent e)
        {
            switch(e.item.Id)
            {
                case ID.ID_ITEM_INTERNAL_DESC:
                    m_strDescription = e.item.Value.ToString();
                    SetSectionText();
                    break;
                case ID.ID_ITEM_BRODCAST_MESSAGE:

                    Sections.SectionDataInternal data = (Sections.SectionDataInternal)m_section.Data;

					DataUtil.CheckChangedData(data.BroadcastMessage, e.item.Value.ToString());
					data.BroadcastMessage = e.item.Value.ToString();
                    break;
                            
            }
        }

        private void axPropertyGrid_AfterEdit(object sender, AxXtremePropertyGrid._DPropertyGridEvents_AfterEditEvent e)
        {
            Sections.SectionDataInternal data = (Sections.SectionDataInternal)m_section.Data;
            switch(e.item.Id)
            {
                case ID.ID_ITEM_POPUP:
					DataUtil.CheckChangedData(data.UsePopupMessage, IsSelected(e.newValue));
                    data.UsePopupMessage = IsSelected(e.newValue);
                    break;
                case ID.ID_ITEM_MOBILE:

					DataUtil.CheckChangedData(data.UseMobileApp, IsSelected(e.newValue));
                    data.UseMobileApp = IsSelected(e.newValue);
                    CheckEnableMessage(data);
                    RefreshSection();
                    break;
                case ID.ID_ITEM_BRODCAST:

					DataUtil.CheckChangedData(data.UseBroadcast, IsSelected(e.newValue));
                    data.UseBroadcast = IsSelected(e.newValue);
                    CheckEnableMessage(data);
                    RefreshSection();
                    /*if (data.UseBroadcast == true)
                    {
                        m_itemMessage.ReadOnly = false;
                    }
                    else
                    {
                        m_itemMessage.ReadOnly = true;
                    }*/
                    break;
            }
        }

        private void CheckEnableMessage(Sections.SectionDataInternal data)
        {
            if (!data.UseMobileApp && !data.UseBroadcast)
                m_itemMessage.ReadOnly = true;
            else
                m_itemMessage.ReadOnly = false;
        }

        // section 선택시 호출되는 함수
        public void GetSectionData(Sections.SectionInternal section)
        {
            m_section = section;
            Sections.SectionDataInternal data = (Sections.SectionDataInternal)section.Data;

            // section이 갖고 있는 정보를 속성창에 출력
            m_itemID.Value = data.ComponentID;
            m_itemDescription.Value = section.Title; // 업무내용
			m_strDescription = section.Title;

            if(data.UsePopupMessage)
                m_itemPopup.MaskedText = "사용";
            else
                m_itemPopup.MaskedText = "사용안함";

            if(data.UseMobileApp)
                m_itemMobile.MaskedText = "사용";
            else
                m_itemMobile.MaskedText = "사용안함";

            if (data.UseBroadcast)
                m_itemBroadcast.MaskedText = "사용";
            else
                m_itemBroadcast.MaskedText = "사용안함";

            m_itemPopup.Selected = true;
            m_itemMobile.Selected = true;
            m_itemBroadcast.Selected = true;


            m_itemMessage.MaskedText = data.BroadcastMessage;
         
            if (data.UseBroadcast == false && data.UseMobileApp == false)
            {
                m_itemMessage.ReadOnly = true;
            }
            else
            {
                m_itemMessage.ReadOnly = false;
            }
        }

        private bool IsSelected(string strValue)
        {
            if (strValue == "사용")
                return true;

            return false;
        }

        // Section에 Data 입력
        public void SetSectionText()
        {
			DataUtil.CheckChangedData(m_section.Title, m_strDescription);

            m_section.Title = m_strDescription;

            Control ctrl = m_section.GetParent();
            ctrl.Refresh();
        }

        public void RefreshSection()
        {
            Control ctrl = m_section.GetParent();
            ctrl.Refresh();
        }
    }
}
