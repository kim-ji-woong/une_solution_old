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
    public partial class PropertiesAnnotation : Form
    {
        private Sections.SectionAnnotation m_section;

        PropertyGridItem m_itemID = null;
        PropertyGridItem m_itemDescription = null;

        public PropertiesAnnotation()
        {
            InitializeComponent();

            InitAnnotation();
        }

        // 설명 속성
        private void InitAnnotation()
        {
            PropertyGridItem CategoryNormal = axPropertyGrid.AddCategory("일반");
            m_itemID = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "ID", "");
            m_itemID.ReadOnly = true;

            m_itemDescription = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemMultilineString, "내용", "");
            m_itemDescription.Id = ID.ID_ITEM_ANNOTATION_DESC;

            CategoryNormal.Expanded = true;
        }

        // section 선택시 호출되는 함수
        public void GetSectionData(Sections.SectionAnnotation section)
        {
            m_section = section;
            Sections.SectionDataAnnotation data = (Sections.SectionDataAnnotation)section.Data;

            // section이 갖고 있는 정보를 속성창에 출력
            m_itemID.Value = data.ComponentID;
            m_itemDescription.Value = section.Title; // 업무내용
        }

    }
}
