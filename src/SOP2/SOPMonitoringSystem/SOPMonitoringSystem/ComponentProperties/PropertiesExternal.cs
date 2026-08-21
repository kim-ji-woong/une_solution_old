using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using XtremePropertyGrid;

namespace SOPMonitoringSystem
{
    public interface IPropertyExternal
    {
        string SelectedList
        {
            get;
            set;
        }

        void SetRemoveReceive(int nID, Sections.ExternalTeamData exData);
        void SetAddReceive(int nID, Sections.ExternalTeamData exData);
    }

    public partial class PropertiesExternal : Form//, IPropertyExternal
    {
        private Sections.SectionExternal m_section;

        PropertyGridItem m_itemID = null;
        PropertyGridItem m_itemDescription = null;
        PropertyGridItem m_itemMsg = null;
        PropertyGridItem m_itemSMSMsg = null;
        PropertyGridItem m_itemReceive = null;
        PropertyGridItem m_itemFAX = null;
        PropertyGridItem m_itemReceiveFax = null;

        public PropertiesExternal()
        {
            InitializeComponent();

            InitExternal();
        }

        // 외부 상황전파 속성
        private void InitExternal()
        {
            PropertyGridItem CategoryNormal = axPropertyGrid.AddCategory("일반");
            m_itemID = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "ID", "");
            m_itemID.ReadOnly = true;

            m_itemDescription = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemMultilineString, "표시내용", "");
            m_itemDescription.Id = ID.ID_ITEM_EXTERNAL_DESC;
            CategoryNormal.Expanded = true;

            PropertyGridItem CategoryEtc = axPropertyGrid.AddCategory("상황전파");
            m_itemMsg = CategoryEtc.AddChildItem(PropertyItemType.PropertyItemString, "문자메시지", "");
            m_itemMsg.Id = ID.ID_ITEM_SMS;

            m_itemSMSMsg = m_itemMsg.AddChildItem(PropertyItemType.PropertyItemMultilineString, "내용", "");
            m_itemSMSMsg.Id = ID.ID_ITEM_CONTENT;

            m_itemReceive = m_itemMsg.AddChildItem(PropertyItemType.PropertyItemString, "수신처", "");
            m_itemReceive.Id = ID.ID_ITEM_RECEIVE_PHONE;
            m_itemMsg.Expanded = true;

            m_itemFAX = CategoryEtc.AddChildItem(PropertyItemType.PropertyItemString, "e-FAX", "");
            m_itemFAX.Id = ID.ID_ITEM_FAX;

            m_itemReceiveFax = m_itemFAX.AddChildItem(PropertyItemType.PropertyItemString, "수신처", "");
            m_itemReceiveFax.Id = ID.ID_ITEM_RECEIVE_FAX;
            m_itemFAX.Expanded = true;
            CategoryEtc.Expanded = true;
        }

        // section 선택시 호출되는 함수
        public void GetSectionData(Sections.SectionExternal section)
        {
            m_section = section;
            Sections.SectionDataExternal data = (Sections.SectionDataExternal)section.Data;

            // section이 갖고 있는 정보를 속성창에 출력
            m_itemID.Value = data.ComponentID;
            m_itemDescription.Value = section.Title; // 업무내용

            if(data.UseSMS)
                m_itemMsg.MaskedText = "사용";
            else
                m_itemMsg.MaskedText = "사용안함";

            m_itemSMSMsg.Value = data.SMSMessage;

            m_itemReceive.Value = OnSelectedPhone(data.SMSReceivers);

            if(data.UseFax)
                m_itemFAX.MaskedText = "사용";
            else
                m_itemFAX.MaskedText = "사용안함";

            m_itemReceiveFax.Value = OnSelectedFax(data.FaxReceivers);
        }

        private string OnSelectedPhone(ArrayList arrTeam)
        {
            int nCount = 0;
            string strValue = "";
            foreach (Sections.ExternalTeamData data in arrTeam)
            {
                strValue +=data.TeamName;
                if (nCount != arrTeam.Count - 1)
                {
                    strValue += ", ";
                    nCount++;
                }
            }
            
            return strValue;
        }
        private string OnSelectedFax(ArrayList arrTeam)
        {
            int nCount = 0;
            string strValue = "";
            foreach (Sections.ExternalTeamData data in arrTeam)
            {
                strValue += data.TeamName;
                if (nCount != arrTeam.Count - 1)
                {
                    strValue += ", ";
                    nCount++;
                }
            }

            return strValue;
        }
    }
}
