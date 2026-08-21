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
    public partial class PropertiesTransmission : Form, IPropertyExternal
    {
        private Sections.SectionTransmission m_section = null;

        // 일반
        private PropertyGridItem m_itemID = null;
        private PropertyGridItem m_itemTitle = null;
        //////////////////////////////////////////////////

        // 내부 상황전파
        private PropertyGridItem m_itemInternalPopup = null;
        private PropertyGridItem m_itemInternalMobile = null;
        private PropertyGridItem m_itemInternalBroadcast = null;
        //////////////////////////////////////////////////

        // 외부 상황전파
        private PropertyGridItem m_itemExternalUseSMS = null;
        private PropertyGridItem m_itemExternalSMSMsg = null;
        private PropertyGridItem m_itemExternalSMSReceivers = null;
        private PropertyGridItem m_itemExternalUseFAX = null;
        private PropertyGridItem m_itemExternalFaxReceivers = null;
        //////////////////////////////////////////////////

        private ArrayList m_arrExternalSMSReceivers = new ArrayList();
        private ArrayList m_arrExternalFaxReceivers = new ArrayList();
        private ArrayList m_arrExternalTeam = new ArrayList();

        private string m_strTitle = "";
        public string Title
        {
            get { return m_strTitle; }
            set { m_strTitle = value; }
        }

        private string m_strExternalSMSMessage = "";
        public string ExternalSMSMessage
        {
            get { return m_strExternalSMSMessage; }
            set { m_strExternalSMSMessage = value; }
        }

        private string m_strSelectedList = "";
        public string SelectedList
        {
            get { return m_strSelectedList; }
            set { m_strSelectedList = value; }
        }

        public ArrayList ExternalSMSReceivers
        {
            get { return m_arrExternalSMSReceivers; }
            set { m_arrExternalSMSReceivers = value; }
        }

        public ArrayList ExternalFaxReceivers
        {
            get { return m_arrExternalFaxReceivers; }
            set { m_arrExternalFaxReceivers = value; }
        }

        public PropertiesTransmission()
        {
            InitializeComponent();

            InitGrid();
        }

        private void InitGrid()
        {
            InitGeneral();
            InitInternal();
            InitExternal();
        }

        private void InitGeneral()
        {
            PropertyGridItem CategoryNormal = axPropertyGrid.AddCategory("일반");
            m_itemID = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "ID", "");
            m_itemID.ReadOnly = true;

            m_itemTitle = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "표시내용", "");
            m_itemTitle.Id = ID.ID_ITEM_INTERNAL_DESC;
            CategoryNormal.Expanded = true;
        }

        // 내부 상황전파 속성
        private void InitInternal()
        {
            PropertyGridItem CategoryInternal = axPropertyGrid.AddCategory("내부 상황전파");
            m_itemInternalPopup = CategoryInternal.AddChildItem(PropertyItemType.PropertyItemString, "팝업메시지", 1);
            m_itemInternalPopup.Id = ID.ID_ITEM_POPUP;

            m_itemInternalMobile = CategoryInternal.AddChildItem(PropertyItemType.PropertyItemString, "모바일 APP", 1);
            m_itemInternalMobile.Id = ID.ID_ITEM_MOBILE;

            m_itemInternalBroadcast = CategoryInternal.AddChildItem(PropertyItemType.PropertyItemString, "사내 방송", 1);
            m_itemInternalBroadcast.Id = ID.ID_ITEM_BRODCAST;

            CategoryInternal.Expanded = true;
        }

        // 외부 상황전파 속성
        private void InitExternal()
        {
            PropertyGridItem CategoryExternal = axPropertyGrid.AddCategory("외부 상황전파");
            m_itemExternalUseSMS = CategoryExternal.AddChildItem(PropertyItemType.PropertyItemString, "문자메시지", 1);
            m_itemExternalUseSMS.Id = ID.ID_ITEM_SMS;

            m_itemExternalSMSMsg = m_itemExternalUseSMS.AddChildItem(PropertyItemType.PropertyItemMultilineString, "내용", "");
            m_itemExternalSMSMsg.Id = ID.ID_ITEM_CONTENT;

            m_itemExternalSMSReceivers = m_itemExternalUseSMS.AddChildItem(PropertyItemType.PropertyItemString, "수신처", "");
            m_itemExternalSMSReceivers.Id = ID.ID_ITEM_RECEIVE_PHONE;
            m_itemExternalUseSMS.Expanded = true;

            m_itemExternalUseFAX = CategoryExternal.AddChildItem(PropertyItemType.PropertyItemString, "e-FAX", 1);
            m_itemExternalUseFAX.Id = ID.ID_ITEM_FAX;

            m_itemExternalFaxReceivers = m_itemExternalUseFAX.AddChildItem(PropertyItemType.PropertyItemString, "수신처", "");
            m_itemExternalFaxReceivers.Id = ID.ID_ITEM_RECEIVE_FAX;
            m_itemExternalUseFAX.Expanded = true;
            CategoryExternal.Expanded = true;
        }

        public void SetSection(Sections.SectionTransmission section)
        {
            m_section = section;

            Sections.SectionDataTransmission data = (Sections.SectionDataTransmission)section.Data;

            // section이 갖고 있는 정보를 속성창에 출력
            m_itemID.Value = data.ComponentID;
            m_itemTitle.Value = section.Title; // 업무내용

            if (data.DataInternal.UsePopupMessage)
                m_itemInternalPopup.MaskedText = "사용";
            else
                m_itemInternalPopup.MaskedText = "사용안함";

            if (data.DataInternal.UseMobileApp)
                m_itemInternalMobile.MaskedText = "사용";
            else
                m_itemInternalMobile.MaskedText = "사용안함";

            if (data.DataInternal.UseBroadcast)
                m_itemInternalBroadcast.MaskedText = "사용";
            else
                m_itemInternalBroadcast.MaskedText = "사용안함";

            m_itemInternalPopup.Selected = true;
            m_itemInternalMobile.Selected = true;
            m_itemInternalBroadcast.Selected = true;

            m_itemExternalSMSMsg.Value = data.DataExternal.SMSMessage;
            m_itemExternalSMSReceivers.Value = OnSelectedPhone(data.DataExternal.SMSReceivers);
            m_itemExternalFaxReceivers.Value = OnSelectedFax(data.DataExternal.FaxReceivers);

            if (data.DataExternal.UseSMS)
            {
                m_itemExternalUseSMS.MaskedText = "사용";
                m_itemExternalSMSReceivers.ReadOnly = false;
                m_itemExternalSMSMsg.ReadOnly = false;
            }
            else
            {
                m_itemExternalUseSMS.MaskedText = "사용안함";
                m_itemExternalSMSReceivers.ReadOnly = true;
                m_itemExternalSMSMsg.ReadOnly = true;
            }

            if (data.DataExternal.UseFax)
            {
                m_itemExternalUseFAX.MaskedText = "사용";
                m_itemExternalFaxReceivers.ReadOnly = false;
            }
            else
            {
                m_itemExternalUseFAX.MaskedText = "사용안함";
                m_itemExternalFaxReceivers.ReadOnly = true;
            }
        }

        private string OnSelectedPhone(ArrayList arrTeam)
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

        public void SetAddReceive(int nID, Sections.ExternalTeamData exData)
        {
            Sections.SectionDataTransmission data = (Sections.SectionDataTransmission)m_section.Data;

            if (nID == ID.ID_ITEM_RECEIVE_PHONE)
            {
                bool isCheck = false;
                foreach (Sections.ExternalTeamData SMSdata in data.DataExternal.SMSReceivers)
                {
                    if (SMSdata.TeamID == exData.TeamID)
                    {
                        isCheck = true;
                    }
                }
                if (!isCheck)
                    data.DataExternal.SMSReceivers.Add(exData);
            }
            else
            {
                bool isCheck = false;
                foreach (Sections.ExternalTeamData Faxdata in data.DataExternal.FaxReceivers)
                {
                    if (Faxdata.TeamID == exData.TeamID)
                    {
                        isCheck = true;
                    }
                }
                if (!isCheck)
                    data.DataExternal.FaxReceivers.Add(exData);
            }
        }

        public void SetRemoveReceive(int nID, Sections.ExternalTeamData exData)
        {
            Sections.SectionDataTransmission data = (Sections.SectionDataTransmission)m_section.Data;

            if (nID == ID.ID_ITEM_RECEIVE_PHONE)
                RemoveExternalTeamData(exData, data.DataExternal.SMSReceivers);
            else
                RemoveExternalTeamData(exData, data.DataExternal.FaxReceivers);
        }

        public void RemoveExternalTeamData(Sections.ExternalTeamData exData, ArrayList arr)
        {
            foreach (Sections.ExternalTeamData data in arr)
            {
                if (data.TeamID == exData.TeamID)
                {
                    arr.Remove(data);
                    return;
                }
            }
        }

        public void SetSectionMessage()
        {
            Sections.SectionDataTransmission data = (Sections.SectionDataTransmission)m_section.Data;
            data.DataExternal.SMSMessage = m_strExternalSMSMessage;
        }

        private bool IsSelected(string strValue)
        {
            if (strValue == "사용")
                return true;

            return false;
        }

        // Section에 Data 입력
        public void SetSectionText()
        {
            m_section.Title = m_strTitle;

            Control ctrl = m_section.GetParent();
            ctrl.Refresh();
        }
    }
}
