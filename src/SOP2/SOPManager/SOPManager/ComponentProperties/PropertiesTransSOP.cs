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
    public partial class PropertiesTransSOP : Form
    {
        private Sections.SectionTransSOP m_section;

        PropertyGridItem m_itemID = null;
        PropertyGridItem m_itemSOP = null;
        PropertyGridItem m_itemDescription = null;

        private string m_strDescription;
        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        private string m_strTitle;
        public string Title
        {
            get { return m_strTitle; }
            set { m_strTitle = value; }
        }

        private string m_strFullPath;
        public string FullPath
        {
            get { return m_strFullPath; }
            set { m_strFullPath = value; }
        }

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
            m_itemSOP.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
            m_itemSOP.Id = ID.ID_ITEM_SOP;
            

            m_itemDescription = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "설명", "");
            m_itemDescription.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
            m_itemDescription.Id = ID.ID_ITEM_TRANSSOP_DESC;
            
            CategoryNormal.Expanded = true;
        }

        private void axPropertyGrid_InplaceButtonDown(object sender, AxXtremePropertyGrid._DPropertyGridEvents_InplaceButtonDownEvent e)
        {
            switch (e.button.Item.Id)
            {
                case ID.ID_ITEM_SOP:
                    PopupTransSOP popupTransSOP = new PopupTransSOP(m_section);
                    if (popupTransSOP.ShowDialog() == DialogResult.OK)
                    {
                        e.button.Item.Value = m_strFullPath;
                    }
                    break;
                case ID.ID_ITEM_TRANSSOP_DESC:
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
            switch (e.item.Id)
            {
                case ID.ID_ITEM_TRANSSOP_DESC:
                    m_strDescription = e.item.Value.ToString();
                    SetSectionDescription();
                    break;
            }
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

			m_strDescription = data.Description;
        }

        // Section에 Data 입력
        public void SetSectionTitle(TreeNode node, string strValue)
        {
            Sections.SectionDataTransSOP data = (Sections.SectionDataTransSOP)m_section.Data;

            m_section.Title = strValue + " / " + node.Text;
            data.Title = m_strFullPath;
            data.LinkedActionStepID = (int)node.Tag;
			
            Control ctrl = m_section.GetParent();
            ctrl.Refresh();
        }

        public void SetSectionDescription()
        {
            Sections.SectionDataTransSOP data = (Sections.SectionDataTransSOP)m_section.Data;
            data.Description = m_strDescription;
        }

    }
}
