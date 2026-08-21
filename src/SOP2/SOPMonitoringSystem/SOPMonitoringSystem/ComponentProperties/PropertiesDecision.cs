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
    public partial class PropertiesDecision : Form
    {
        private Sections.SectionDecision m_section;

        PropertyGridItem m_itemID = null;
        PropertyGridItem m_itemTask = null;

        public PropertiesDecision()
        {
            InitializeComponent();
            
            InitDicision();
        }

        // 판단 속성
        private void InitDicision()
        {
            PropertyGridItem CategoryNormal = axPropertyGrid.AddCategory("일반");
            m_itemID = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "ID", "");
            m_itemID.ReadOnly = true;

            m_itemTask = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemMultilineString, "업무내용", "");
            m_itemTask.Id = ID.ID_ITEM_TASK;
            CategoryNormal.Expanded = true;
        }

        // section 선택시 호출되는 함수
        public void GetSectionData(Sections.SectionDecision section)
        {
            m_section = section;
            Sections.SectionDataDecision data = (Sections.SectionDataDecision)section.Data;

            // section이 갖고 있는 정보를 속성창에 출력
            m_itemID.Value = data.ComponentID;
            m_itemTask.Value = section.Title; // 업무내용
        }

    }
}
