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
        private PropertyGridItem m_itemInternalMessage = null;
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
            m_itemTitle.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
            m_itemTitle.Id = ID.ID_ITEM_TRANSMISSION_DESC;
            CategoryNormal.Expanded = true;
        }

        // 내부 상황전파 속성
        private void InitInternal()
        {
            PropertyGridItem CategoryInternal = axPropertyGrid.AddCategory("내부 상황전파");
            m_itemInternalPopup = CategoryInternal.AddChildItem(PropertyItemType.PropertyItemEnum, "팝업메시지", 1);
            m_itemInternalPopup.Constraints.Add("사용", 1);
            m_itemInternalPopup.Constraints.Add("사용안함", 2);
            m_itemInternalPopup.Id = ID.ID_ITEM_TRANSMISSION_POPUP;

            m_itemInternalMobile = CategoryInternal.AddChildItem(PropertyItemType.PropertyItemEnum, "모바일 APP", 1);
            m_itemInternalMobile.Constraints.Add("사용", 1);
            m_itemInternalMobile.Constraints.Add("사용안함", 2);
            m_itemInternalMobile.Id = ID.ID_ITEM_TRANSMISSION_MOBILE;

            m_itemInternalBroadcast = CategoryInternal.AddChildItem(PropertyItemType.PropertyItemEnum, "사내 방송", 1);
            m_itemInternalBroadcast.Constraints.Add("사용", 1);
            m_itemInternalBroadcast.Constraints.Add("사용안함", 2);
            m_itemInternalBroadcast.Id = ID.ID_ITEM_TRANSMISSION_BRODCAST;


            m_itemInternalMessage = m_itemInternalBroadcast.AddChildItem(PropertyItemType.PropertyItemString, "방송메세지", "");
            m_itemInternalMessage.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
            m_itemInternalMessage.Id = ID.ID_ITEM_BRODCAST_MESSAGE;
            m_itemInternalBroadcast.Expanded = true;

            CategoryInternal.Expanded = true;
        }

        // 외부 상황전파 속성
        private void InitExternal()
        {
            PropertyGridItem CategoryExternal = axPropertyGrid.AddCategory("외부 상황전파");
            m_itemExternalUseSMS = CategoryExternal.AddChildItem(PropertyItemType.PropertyItemEnum, "문자메시지", 1);
            m_itemExternalUseSMS.Constraints.Add("사용", 1);
            m_itemExternalUseSMS.Constraints.Add("사용안함", 2);
            m_itemExternalUseSMS.Id = ID.ID_ITEM_TRANSMISSION_SMS;

            m_itemExternalSMSMsg = m_itemExternalUseSMS.AddChildItem(PropertyItemType.PropertyItemString, "내용", "");
            m_itemExternalSMSMsg.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
            m_itemExternalSMSMsg.Id = ID.ID_ITEM_TRANSMISSION_CONTENT;

            m_itemExternalSMSReceivers = m_itemExternalUseSMS.AddChildItem(PropertyItemType.PropertyItemString, "수신처", "");
            m_itemExternalSMSReceivers.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
            m_itemExternalSMSReceivers.Id = ID.ID_ITEM_TRANSMISSION_RECEIVEPHONE;
            m_itemExternalUseSMS.Expanded = true;

            m_itemExternalUseFAX = CategoryExternal.AddChildItem(PropertyItemType.PropertyItemEnum, "e-FAX", 1);
            m_itemExternalUseFAX.Constraints.Add("사용", 1);
            m_itemExternalUseFAX.Constraints.Add("사용안함", 2);
            m_itemExternalUseFAX.Id = ID.ID_ITEM_TRANSMISSION_FAX;

            m_itemExternalFaxReceivers = m_itemExternalUseFAX.AddChildItem(PropertyItemType.PropertyItemString, "수신처", "");
            m_itemExternalFaxReceivers.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
            m_itemExternalFaxReceivers.Id = ID.ID_ITEM_TRANSMISSION_RECEIVEFAX;
            m_itemExternalUseFAX.Expanded = true;
            CategoryExternal.Expanded = true;
        }

        public void SetSection(Sections.SectionTransmission section)
        {
            m_section = section;

            Sections.SectionDataTransmission data = (Sections.SectionDataTransmission)section.Data;
			m_strTitle = section.Title;
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

            m_itemInternalMessage.MaskedText = data.DataInternal.BroadcastMessage;

			if (data.DataInternal.UseBroadcast == false)
            {
                m_itemInternalMessage.ReadOnly = true;
            }
            else
            {
                m_itemInternalMessage.ReadOnly = false;
            }

			m_strExternalSMSMessage = data.DataExternal.SMSMessage;
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

        private void axPropertyGrid_InplaceButtonDown(object sender, AxXtremePropertyGrid._DPropertyGridEvents_InplaceButtonDownEvent e)
        {
            Sections.SectionDataTransmission data = (Sections.SectionDataTransmission)m_section.Data;

            switch (e.button.Item.Id)
            {
                case ID.ID_ITEM_TRANSMISSION_DESC:
                    {
                        PopupNote popupNote = new PopupNote();
                        popupNote.InitText(e.button.Item.Id);
                        if (popupNote.ShowDialog() == DialogResult.OK)
                        {
                            e.button.Item.Value = m_strTitle;
                        }
                    }
                    break;
                case ID.ID_ITEM_TRANSMISSION_CONTENT:
                    {
                        PopupNote popupNote = new PopupNote();
                        popupNote.InitText(e.button.Item.Id);
                        if (popupNote.ShowDialog() == DialogResult.OK)
                        {
                            e.button.Item.Value = m_strExternalSMSMessage;
                        }
                    }
                    break;
                case ID.ID_ITEM_TRANSMISSION_RECEIVEFAX:
                    {
                        PopupSelectReceive popupReceive = new PopupSelectReceive(this);
                        popupReceive.InitGrid(e.button.Item.Id, data.DataExternal.FaxReceivers);
                        m_arrExternalFaxReceivers = data.DataExternal.FaxReceivers;
						m_faxReciverExternal = (ArrayList)data.DataExternal.FaxReceivers.Clone();
                        if (popupReceive.ShowDialog() == DialogResult.OK)
                        {
							if (DataUtil.CheckChangedData(e.button.Item.Value, m_strSelectedList))
							{
								data.DataExternal.FaxReceivers.Clear();
								foreach (Sections.ExternalTeamData Faxdata in m_faxReciverExternal)
								{
									data.DataExternal.FaxReceivers.Add(Faxdata);
								}
								e.button.Item.Value = m_strSelectedList;
							}
                        }
                    }
                    break;
                case ID.ID_ITEM_TRANSMISSION_RECEIVEPHONE:
                    {
                        PopupSelectReceive popupReceive = new PopupSelectReceive(this);
                        popupReceive.InitGrid(e.button.Item.Id, data.DataExternal.SMSReceivers);
                        m_arrExternalSMSReceivers = data.DataExternal.SMSReceivers;
						m_smsReciverExternal = (ArrayList)data.DataExternal.SMSReceivers.Clone();
                        if (popupReceive.ShowDialog() == DialogResult.OK)
                        {
							if (DataUtil.CheckChangedData(e.button.Item.Value, m_strSelectedList))
							{
								data.DataExternal.SMSReceivers.Clear();
								foreach (Sections.ExternalTeamData Faxdata in m_smsReciverExternal)
								{
									data.DataExternal.SMSReceivers.Add(Faxdata);
								}
								e.button.Item.Value = m_strSelectedList;
							}                          
                        }
                    }
                    break;
                case ID.ID_ITEM_BRODCAST_MESSAGE:
                    PopupBroadcastMessage popup = new PopupBroadcastMessage();
                    popup.InitText(data.DataInternal.BroadcastMessage);
                    if (popup.ShowDialog() == DialogResult.OK)
                    {
                        string szText = popup.GetMessage();
						DataUtil.CheckChangedData(data.DataInternal.BroadcastMessage, szText);
                        data.DataInternal.BroadcastMessage = szText;
                        e.button.Item.Value = szText;
                    }
                    break;
            }
        }
		private ArrayList m_smsReciverExternal = new ArrayList();
		private ArrayList m_faxReciverExternal = new ArrayList();
        private void axPropertyGrid_ValueChanged(object sender, AxXtremePropertyGrid._DPropertyGridEvents_ValueChangedEvent e)
        {
            Sections.SectionDataTransmission data = (Sections.SectionDataTransmission)m_section.Data;

            switch (e.item.Id)
            {
                case ID.ID_ITEM_BRODCAST_MESSAGE:
					DataUtil.CheckChangedData(data.DataInternal.BroadcastMessage, e.item.Value.ToString());
                    data.DataInternal.BroadcastMessage = e.item.Value.ToString();
                    break;
                case ID.ID_ITEM_TRANSMISSION_DESC:
                    m_strTitle = e.item.Value.ToString();
                    SetSectionText();
                    break;
                case ID.ID_ITEM_TRANSMISSION_POPUP:
					DataUtil.CheckChangedData(data.DataInternal.UsePopupMessage, IsSelected(e.item.Value.ToString()));
					data.DataInternal.UsePopupMessage = IsSelected(e.item.Value.ToString());
                    break;
                case ID.ID_ITEM_TRANSMISSION_MOBILE:
					DataUtil.CheckChangedData(data.DataInternal.UseMobileApp, IsSelected(e.item.Value.ToString()));
                    data.DataInternal.UseMobileApp = IsSelected(e.item.Value.ToString());
                    break;
                case ID.ID_ITEM_TRANSMISSION_BRODCAST:
					DataUtil.CheckChangedData(data.DataInternal.UseBroadcast, IsSelected(e.item.Value.ToString()));
                    data.DataInternal.UseBroadcast = IsSelected(e.item.Value.ToString());
                    if (data.DataInternal.UseBroadcast == true)
                    {
                        m_itemInternalMessage.ReadOnly = false;
                    }
                    else
                    {
                        m_itemInternalMessage.ReadOnly = true;
                    }

                    break;
                case ID.ID_ITEM_TRANSMISSION_CONTENT:
                    m_strExternalSMSMessage = e.item.Value.ToString();
                    SetSectionMessage();
                    break;
                case ID.ID_ITEM_TRANSMISSION_SMS:
                    bool isTransfer = false;
                    if (e.item.MaskedText == "사용")
                        isTransfer = true;
					DataUtil.CheckChangedData(data.DataExternal.UseSMS, isTransfer);
                    data.DataExternal.UseSMS = isTransfer;
                    break;
                case ID.ID_ITEM_TRANSMISSION_FAX:
                    isTransfer = false;
                    if (e.item.MaskedText == "사용")
                        isTransfer = true;
					DataUtil.CheckChangedData(data.DataExternal.UseFax, isTransfer);
                    data.DataExternal.UseFax = isTransfer;
                    break;
            }

            if (m_itemExternalUseSMS.MaskedText == "사용안함")
            {
                m_itemExternalSMSReceivers.ReadOnly = true;
                m_itemExternalSMSMsg.ReadOnly = true;
            }
            else
            {
                m_itemExternalSMSReceivers.ReadOnly = false;
                m_itemExternalSMSMsg.ReadOnly = false;
            }

            if (m_itemExternalUseFAX.MaskedText == "사용안함")
            {
                m_itemExternalFaxReceivers.ReadOnly = true;
            }
            else
                m_itemExternalFaxReceivers.ReadOnly = false;
        }

        public void SetAddReceive(int nID, Sections.ExternalTeamData exData)
        {
            Sections.SectionDataTransmission data = (Sections.SectionDataTransmission)m_section.Data;

            if (nID == ID.ID_ITEM_TRANSMISSION_RECEIVEPHONE)
            {
                bool isCheck = false;
				foreach (Sections.ExternalTeamData SMSdata in m_smsReciverExternal)
                {
                    if (SMSdata.TeamID == exData.TeamID)
                    {
                        isCheck = true;
                    }
                }
                if (!isCheck)
					m_smsReciverExternal.Add(exData);
            }
            else
            {
                bool isCheck = false;
				foreach (Sections.ExternalTeamData Faxdata in m_faxReciverExternal)
                {
                    if (Faxdata.TeamID == exData.TeamID)
                    {
                        isCheck = true;
                    }
                }
                if (!isCheck)
					m_faxReciverExternal.Add(exData);
            }
        }
		
        public void SetRemoveReceive(int nID, Sections.ExternalTeamData exData)
        {
            Sections.SectionDataTransmission data = (Sections.SectionDataTransmission)m_section.Data;

            if (nID == ID.ID_ITEM_TRANSMISSION_RECEIVEPHONE)
                RemoveExternalTeamData(exData, m_smsReciverExternal);
            else
                RemoveExternalTeamData(exData, m_faxReciverExternal);
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
			DataUtil.CheckChangedData(data.DataExternal.SMSMessage, m_strExternalSMSMessage);
            data.DataExternal.SMSMessage = m_strExternalSMSMessage;
        }

        private bool IsSelected(string strValue)
        {
            if (strValue == "사용" || strValue == "1")
                return true;

            return false;
        }

        // Section에 Data 입력
        public void SetSectionText()
        {
			DataUtil.CheckChangedData(m_section.Title, m_strTitle);

            m_section.Title = m_strTitle;

            Control ctrl = m_section.GetParent();
            ctrl.Refresh();
        }
    }
}
