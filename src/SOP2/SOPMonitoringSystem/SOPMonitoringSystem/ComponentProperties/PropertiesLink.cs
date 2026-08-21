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

namespace SOPMonitoringSystem
{
    public partial class PropertiesLink : Form
    {
        private Sections.SectionLink m_section;

        PropertyGridItem m_itemID = null;
        PropertyGridItem m_itemLink = null;
        PropertyGridItem m_itemDescription = null;

        public PropertiesLink()
        {
            InitializeComponent();
            
            InitLink();
        }

        // 링크 속성
        private void InitLink()
        {
            PropertyGridItem CategoryNormal = axPropertyGrid.AddCategory("일반");
            m_itemID = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "ID", "");
            m_itemID.ReadOnly = true;

            m_itemLink = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "Process Link", "");
            m_itemLink.Id = ID.ID_ITEM_PROCESS_LINK;

            m_itemDescription = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemMultilineString, "표시내용", "");
            m_itemDescription.Id = ID.ID_ITEM_LINK_DESC;
            
            CategoryNormal.Expanded = true;
        }

        // section 선택시 호출되는 함수
        public void GetSectionData(Sections.SectionLink section)
        {
            m_section = section;
            Sections.SectionDataLink data = (Sections.SectionDataLink)section.Data;
            
            string strVlue  = "";
            if(data.LinkedSection != null)
                strVlue = data.LinkedSection.Data.ComponentID;
            
            // section이 갖고 있는 정보를 속성창에 출력
            m_itemID.Value = data.ComponentID;
            m_itemLink.MaskedText = strVlue;
            m_itemLink.Selected = true;
            m_itemDescription.Value = section.Title; // 업무내용 
        }
    }
}
