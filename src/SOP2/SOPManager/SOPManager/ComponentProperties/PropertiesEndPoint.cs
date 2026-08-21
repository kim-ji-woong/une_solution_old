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
    public partial class PropertiesEndPoint : Form
    {
        private Sections.SectionEndPoint m_section;

        PropertyGridItem m_itemID = null;
        PropertyGridItem m_itemType = null;
        PropertyGridItem m_itemDescription = null;

        private string m_strDescription;
        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public PropertiesEndPoint()
        {
            InitializeComponent();

            InitEndPoint();
        }

        // 시작/끝 속성
        private void InitEndPoint()
        {
            PropertyGridItem CategoryNormal = axPropertyGrid.AddCategory("일반");
            m_itemID = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "ID", "");
            m_itemID.ReadOnly = true;

            m_itemType = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemEnum, "Type", "");
            m_itemType.Constraints.Add("시작", 1);
            m_itemType.Constraints.Add("끝", 2);
            m_itemType.Id = ID.ID_ITEM_TYPE;

            m_itemDescription = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "내용", "");
            m_itemDescription.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
            m_itemDescription.Id = ID.ID_ITEM_ENDPOINT_DESC;
            
            CategoryNormal.Expanded = true;
        }

        private void axPropertyGrid_InplaceButtonDown(object sender, AxXtremePropertyGrid._DPropertyGridEvents_InplaceButtonDownEvent e)
        {
            switch (e.button.Item.Id)
            {
                case ID.ID_ITEM_ENDPOINT_DESC:
                    PopupNote popupNote = new PopupNote();
                    popupNote.InitText(e.button.Item.Id);
                    if (popupNote.ShowDialog() == DialogResult.OK)
                    {
                        e.button.Item.Value = m_strDescription;
                    }
                    break;
            }
        }

        private void axPropertyGrid_ValueChanged(object sender, AxXtremePropertyGrid._DPropertyGridEvents_ValueChangedEvent e)
        {
            Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)m_section.Data;

            switch (e.item.Id)
            {
                case ID.ID_ITEM_TYPE:
                    bool isType = false;
                    if(e.item.MaskedText == "시작")
                        isType = true;
					DataUtil.CheckChangedData(data.IsBegin, isType);
                    data.IsBegin = isType;

                    break;
                case ID.ID_ITEM_ENDPOINT_DESC:
					
                    m_strDescription = e.item.Value.ToString();
                    SetSectionText();
                    break;
            }
        }

        // section 선택시 호출되는 함수
        public void GetSectionData(Sections.SectionEndPoint section)
        {
            m_section = section;
            Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)section.Data;

            // section이 갖고 있는 정보를 속성창에 출력
            m_itemID.Value = data.ComponentID;
            
            if(data.IsBegin)
                m_itemType.MaskedText = "시작"; // 임무 메시지
            else
                m_itemType.MaskedText = "끝";

            m_itemType.Selected = true;
            m_itemDescription.Value = section.Title; // 업무내용
			m_strDescription = section.Title;
        }

        // Section에 Data 입력
        public void SetSectionText()
        {            
			DataUtil.CheckChangedData(m_section.Title, m_strDescription);

			m_section.Title = m_strDescription;

            Control ctrl = m_section.GetParent();
            ctrl.Refresh();
        }

    }
}
