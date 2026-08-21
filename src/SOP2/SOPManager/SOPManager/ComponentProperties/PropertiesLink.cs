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

namespace SOPManager
{
    public partial class PropertiesLink : Form
    {
        private Sections.SectionLink m_section;

        PropertyGridItem m_itemID = null;
        PropertyGridItem m_itemLink = null;
        PropertyGridItem m_itemDescription = null;

        private string m_strDescription;
        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

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
            
            m_itemDescription = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "표시내용", "");
            m_itemDescription.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
            m_itemDescription.Id = ID.ID_ITEM_LINK_DESC;

			m_itemLink = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemEnum, "Process Link", 1);
			//m_itemDescription.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
			m_itemLink.Id = ID.ID_ITEM_PROCESS_LINK;
			
            
            CategoryNormal.Expanded = true;
        }

        private void axPropertyGrid_InplaceButtonDown(object sender, AxXtremePropertyGrid._DPropertyGridEvents_InplaceButtonDownEvent e)
        {
            switch (e.button.Item.Id)
            {
                case ID.ID_ITEM_PROCESS_LINK:
                    AddProcessLinkData();
                    break;
                case ID.ID_ITEM_LINK_DESC:
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
            Sections.SectionDataLink data = (Sections.SectionDataLink)m_section.Data;
            ArrayList arrComponent = FormMain.Instance.GetPageLevel().AllComponentList();

			Sections.Section findSection = null;
            switch (e.item.Id)
            {
                case ID.ID_ITEM_PROCESS_LINK:
                    foreach (Sections.Section section in arrComponent)
                    {
                        if (e.item.MaskedText == section.Data.ComponentID)
                        {
							findSection = section;
                            break;
                        }
                    }

					if (findSection != null)
					{
						DataUtil.CheckChangedData(findSection, data.LinkedSection);
						data.LinkedSection = findSection;
						e.item.Selected = true;
					}
					else
					{
						string strVlue = "";
						data.LinkedSection = null;
						if (data.LinkedSection != null)
							strVlue = data.LinkedSection.Data.ComponentID;
						e.item.Value = strVlue;
					}
					
                    break;
                case ID.ID_ITEM_LINK_DESC:
                    m_strDescription = e.item.Value.ToString();
					SetSectionText();
					break;
            }
        }
        
        private void axPropertyGrid_AfterEdit(object sender, AxXtremePropertyGrid._DPropertyGridEvents_AfterEditEvent e)
        {
			//Sections.SectionDataLink data = (Sections.SectionDataLink)m_section.Data;
			//ArrayList arrComponent = FormMain.Instance.GetPageLevel().AllComponentList();
			//switch (e.item.Id)
			//{
			//    case ID.ID_ITEM_PROCESS_LINK:
			//        foreach (Sections.Section section in arrComponent)
			//        {
			//            if (e.item.MaskedText == section.Data.ComponentID)
			//            {
			//                CheckChangedData.CheckData(section, data.LinkedSection);
			//                data.LinkedSection = section;
			//                e.item.Selected = true;
			//                break;
			//            }
			//        }
			//        break;
			//}
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

        private void AddProcessLinkData()
        {
            m_itemLink.Constraints.Clear();
			m_itemLink.Constraints.Add("");
            ArrayList arrComponent = FormMain.Instance.GetPageLevel().AllComponentList();
            int i = 2;
            foreach (Sections.Section section in arrComponent)
            {
                string strComponentID = section.Data.ComponentID;
                m_itemLink.Constraints.Add(strComponentID, i);
                i++;
            }
        }

    }
}
