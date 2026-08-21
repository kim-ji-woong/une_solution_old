using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XtremePropertyGrid;

namespace SOPMonitoringSystem
{
    public partial class PropertiesInternal : Form
    {
        private Sections.SectionInternal m_section;

        PropertyGridItem m_itemID = null;
        PropertyGridItem m_itemDescription = null;
        PropertyGridItem m_itemPopup = null;
        PropertyGridItem m_itemMobile = null;
        PropertyGridItem m_itemBroadcast = null;

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

            m_itemDescription = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemMultilineString, "표시내용", "");
            m_itemDescription.Id = ID.ID_ITEM_INTERNAL_DESC;
            CategoryNormal.Expanded = true;

            PropertyGridItem CategoryEtc = axPropertyGrid.AddCategory("상황전파");
            m_itemPopup = CategoryEtc.AddChildItem(PropertyItemType.PropertyItemString, "팝업메시지", "");
            m_itemPopup.Id = ID.ID_ITEM_POPUP;

            m_itemMobile = CategoryEtc.AddChildItem(PropertyItemType.PropertyItemString, "모바일 APP", "");
            m_itemMobile.Id = ID.ID_ITEM_MOBILE;

            m_itemBroadcast = CategoryEtc.AddChildItem(PropertyItemType.PropertyItemString, "사내 방송", "");
            m_itemBroadcast.Id = ID.ID_ITEM_BRODCAST;

            CategoryEtc.Expanded = true;
        }

        // section 선택시 호출되는 함수
        public void GetSectionData(Sections.SectionInternal section)
        {
            m_section = section;
            Sections.SectionDataInternal data = (Sections.SectionDataInternal)section.Data;

            // section이 갖고 있는 정보를 속성창에 출력
            m_itemID.Value = data.ComponentID;
            m_itemDescription.Value = section.Title; // 업무내용

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
        }

    }
}
