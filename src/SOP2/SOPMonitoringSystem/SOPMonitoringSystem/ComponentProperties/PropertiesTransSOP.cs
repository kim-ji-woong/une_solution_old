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
    public partial class PropertiesTransSOP : Form
    {
        private Sections.SectionTransSOP m_section;

        PropertyGridItem m_itemID = null;
        PropertyGridItem m_itemSOP = null;
        PropertyGridItem m_itemDescription = null;

        public PropertiesTransSOP()
        {
            InitializeComponent();

            InitTransSOP();
        }

        // SOP 전환 속성
        private void InitTransSOP()
        {
            PropertyGridItem CategoryNormal = axPropertyGrid.AddCategory("일반");
            m_itemID = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "ID", "");
            m_itemID.ReadOnly = true;

            m_itemSOP = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "SOP","");
            m_itemSOP.Id = ID.ID_ITEM_SOP;

            m_itemDescription = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemMultilineString, "설명", "");
            m_itemDescription.Id = ID.ID_ITEM_TRANSSOP_DESC;
            
            CategoryNormal.Expanded = true;
        }

        // section 선택시 호출되는 함수
        public void GetSectionData(Sections.SectionTransSOP section)
        {
            m_section = section;
            Sections.SectionDataTransSOP data = (Sections.SectionDataTransSOP)section.Data;

            // section이 갖고 있는 정보를 속성창에 출력
            m_itemID.Value = data.ComponentID;
            m_itemSOP.Value = data.Title;
            m_itemDescription.Value = data.Description; // 설명
        }

    }
}
