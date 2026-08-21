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
    public partial class PropertiesEndPoint : Form
    {
        private Sections.SectionEndPoint m_section;

        PropertyGridItem m_itemID = null;
        PropertyGridItem m_itemType = null;
        PropertyGridItem m_itemDescription = null;

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

            m_itemType = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "Type", "");
            m_itemType.Id = ID.ID_ITEM_TYPE;

            m_itemDescription = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemMultilineString, "내용", "");
            m_itemDescription.Id = ID.ID_ITEM_ENDPOINT_DESC;
            
            CategoryNormal.Expanded = true;
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
        }

    }
}
