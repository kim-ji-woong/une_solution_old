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

namespace SOPManager
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

    public partial class PropertiesExternal : Form, IPropertyExternal
    {
        private Sections.SectionExternal m_section;

        PropertyGridItem m_itemID = null;
        PropertyGridItem m_itemDescription = null;
        PropertyGridItem m_itemMsg = null;
        PropertyGridItem m_itemSMSMsg = null;
        PropertyGridItem m_itemReceive = null;
        PropertyGridItem m_itemFAX = null;
        PropertyGridItem m_itemReceiveFax = null;

        private ArrayList m_arrSMSRcv = new ArrayList();
        private ArrayList m_arrFaxRcv = new ArrayList();
        private ArrayList m_arrExternalTeam = new ArrayList();

        private string m_strDescription;
        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        private string m_strMemo;
        public string Memo
        {
            get { return m_strMemo; }
            set { m_strMemo = value; }
        }

        private string m_strSelectedList;
        public string SelectedList
        {
            get { return m_strSelectedList; }
            set 
			{
				
					m_strSelectedList = value; 

			}
        }

        public ArrayList SMSReceive
        {
            get{ return m_arrSMSRcv; }
            set{ m_arrSMSRcv = value; }
        }

        public ArrayList FaxReceive
        {
            get{ return m_arrFaxRcv; }
            set { m_arrFaxRcv = value; }
        }

        public PropertiesExternal()
        {
            InitializeComponent();

            m_arrExternalTeam = FormMain.Instance.ExternalTeam;
            InitExternal();
        }

        // 외부 상황전파 속성
        private void InitExternal()
        {
            PropertyGridItem CategoryNormal = axPropertyGrid.AddCategory("일반");
            m_itemID = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "ID", "");
            m_itemID.ReadOnly = true;

            m_itemDescription = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "표시내용", "");
            m_itemDescription.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
            m_itemDescription.Id = ID.ID_ITEM_EXTERNAL_DESC;
            CategoryNormal.Expanded = true;

            PropertyGridItem CategoryEtc = axPropertyGrid.AddCategory("상황전파");
            m_itemMsg = CategoryEtc.AddChildItem(PropertyItemType.PropertyItemEnum, "문자메시지", 1);
            m_itemMsg.Constraints.Add("사용", 1);
            m_itemMsg.Constraints.Add("사용안함", 2);
            m_itemMsg.Id = ID.ID_ITEM_SMS;

            m_itemSMSMsg = m_itemMsg.AddChildItem(PropertyItemType.PropertyItemString, "내용", "");
            m_itemSMSMsg.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
            m_itemSMSMsg.Id = ID.ID_ITEM_CONTENT;

            //PropertyGridItem itemAuto = itemMsg.AddChildItem(PropertyItemType.PropertyItemEnum, "자동발송", 2);
            //itemAuto.Constraints.Add("사용", 1);
            //itemAuto.Constraints.Add("사용안함", 2);

            m_itemReceive = m_itemMsg.AddChildItem(PropertyItemType.PropertyItemString, "수신처", "");
            m_itemReceive.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
            m_itemReceive.Id = ID.ID_ITEM_RECEIVE_PHONE;
            m_itemMsg.Expanded = true;

            m_itemFAX = CategoryEtc.AddChildItem(PropertyItemType.PropertyItemEnum, "e-FAX", 1);
            m_itemFAX.Constraints.Add("사용", 1);
            m_itemFAX.Constraints.Add("사용안함", 2);
            m_itemFAX.Id = ID.ID_ITEM_FAX;

            m_itemReceiveFax = m_itemFAX.AddChildItem(PropertyItemType.PropertyItemString, "수신처", "");
            m_itemReceiveFax.Flags = PropertyItemFlags.ItemHasEdit | PropertyItemFlags.ItemHasExpandButton;
            m_itemReceiveFax.Id = ID.ID_ITEM_RECEIVE_FAX;
            m_itemFAX.Expanded = true;
            CategoryEtc.Expanded = true;
        }

        private void axPropertyGrid_InplaceButtonDown(object sender, AxXtremePropertyGrid._DPropertyGridEvents_InplaceButtonDownEvent e)
        {
            Sections.SectionDataExternal data = (Sections.SectionDataExternal)m_section.Data;
            switch (e.button.Item.Id)
            {
                case ID.ID_ITEM_EXTERNAL_DESC:
					//m_strDescription = e.button.Item.Value.ToString();
                    PopupNote popupNote2 = new PopupNote();
                    popupNote2.InitText(e.button.Item.Id);
                    if (popupNote2.ShowDialog() == DialogResult.OK)
                    {
                        e.button.Item.Value = m_strDescription;
                    }
                    break;
                case ID.ID_ITEM_CONTENT:
					//m_strMemo = e.button.Item.Value.ToString();
                    PopupNote popupNote = new PopupNote();
                    popupNote.InitText(e.button.Item.Id);
                    if (popupNote.ShowDialog() == DialogResult.OK)
                    {
                        e.button.Item.Value = m_strMemo;
                    }
                    break;
                case ID.ID_ITEM_RECEIVE_FAX:
                    PopupSelectReceive popupReceive = new PopupSelectReceive(this);
                    popupReceive.InitGrid(e.button.Item.Id, data.FaxReceivers);
                    m_arrFaxRcv = data.FaxReceivers;
					m_faxReciver = (ArrayList)data.FaxReceivers.Clone();
                    if (popupReceive.ShowDialog() == DialogResult.OK)
                    {
						if (DataUtil.CheckChangedData(e.button.Item.Value, m_strSelectedList))
						{
							data.FaxReceivers.Clear();
							foreach (Sections.ExternalTeamData Faxdata in m_faxReciver)
							{
								data.FaxReceivers.Add(Faxdata);
							}
							e.button.Item.Value = m_strSelectedList;
						}						
                    }					
                    break;
                case ID.ID_ITEM_RECEIVE_PHONE:
                    popupReceive = new PopupSelectReceive(this);
                    popupReceive.InitGrid(e.button.Item.Id, data.SMSReceivers);
                    m_arrSMSRcv = data.SMSReceivers;
					m_smsReciver = (ArrayList)data.SMSReceivers.Clone();
                    if (popupReceive.ShowDialog() == DialogResult.OK)
                    {
						if (DataUtil.CheckChangedData(e.button.Item.Value, m_strSelectedList))
						{
							data.SMSReceivers.Clear();
							foreach (Sections.ExternalTeamData Faxdata in m_smsReciver)
							{
								data.SMSReceivers.Add(Faxdata);
							}
							e.button.Item.Value = m_strSelectedList;							
						}						
                    }					
                    break;
            }
        }

        private void axPropertyGrid_ValueChanged(object sender, AxXtremePropertyGrid._DPropertyGridEvents_ValueChangedEvent e)
        {
            Sections.SectionDataExternal data = (Sections.SectionDataExternal)m_section.Data;

            switch(e.item.Id)
            {
                case ID.ID_ITEM_EXTERNAL_DESC:
                    m_strDescription = e.item.Value.ToString();
                    SetSectionText();
                    break;
                case ID.ID_ITEM_CONTENT:
					m_strMemo = e.item.Value.ToString();
                    SetSectionMessage();
                    break;
                case ID.ID_ITEM_SMS:
                    bool isTransfer = false;
                    if(e.item.MaskedText == "사용")
                        isTransfer = true;
					if (DataUtil.CheckChangedData(data.UseSMS, isTransfer))
					{
						data.UseSMS = isTransfer;
					}                    
                    break;
                case ID.ID_ITEM_FAX:
                    isTransfer = false;
                    if(e.item.MaskedText == "사용")
                        isTransfer = true;

					if (DataUtil.CheckChangedData(data.UseSMS, isTransfer))
					{
						data.UseFax = isTransfer;
					}
                    break;
            }
            if (m_itemMsg.MaskedText == "사용안함")
            {
                m_itemReceive.ReadOnly = true;
                m_itemSMSMsg.ReadOnly = true;
            }
            else
            {
                m_itemReceive.ReadOnly = false;
                m_itemSMSMsg.ReadOnly = false;
            }

            if (m_itemFAX.MaskedText == "사용안함")
            {
                m_itemReceiveFax.ReadOnly = true;
            }
            else
                m_itemReceiveFax.ReadOnly = false;
        }

        // section 선택시 호출되는 함수
        public void GetSectionData(Sections.SectionExternal section)
        {
            m_section = section;
            Sections.SectionDataExternal data = (Sections.SectionDataExternal)section.Data;

            // section이 갖고 있는 정보를 속성창에 출력
            m_itemID.Value = data.ComponentID;
            m_itemDescription.Value = section.Title; // 업무내용
			m_strDescription = section.Title;

            m_itemSMSMsg.Value = data.SMSMessage;
			m_strMemo = data.SMSMessage;
            m_itemReceive.Value = OnSelectedPhone(data.SMSReceivers);
            m_itemReceiveFax.Value = OnSelectedFax(data.FaxReceivers);

            if (data.UseSMS)
            {
                m_itemMsg.MaskedText = "사용";
                m_itemReceive.ReadOnly = false;
                m_itemSMSMsg.ReadOnly = false;
            }
            else
            {
                m_itemMsg.MaskedText = "사용안함";
                m_itemReceive.ReadOnly = true;
                m_itemSMSMsg.ReadOnly = true;
            }

            if(data.UseFax)
            {
                m_itemFAX.MaskedText = "사용";
                m_itemReceiveFax.ReadOnly = false;
            }
            else
            {
                m_itemFAX.MaskedText = "사용안함";
                m_itemReceiveFax.ReadOnly = true;
            }
    
        }

        // Section에 Data 입력
        public void SetSectionText()
        {
			if (DataUtil.CheckChangedData(m_section.Title, m_strDescription))
			{
				m_section.Title = m_strDescription;
				Control ctrl = m_section.GetParent();
				ctrl.Refresh();
			}           
        }
        
        public void SetSectionMessage()
        {
            Sections.SectionDataExternal data = (Sections.SectionDataExternal)m_section.Data;            
			if (DataUtil.CheckChangedData(data.SMSMessage, m_strMemo))
			{
				data.SMSMessage = m_strMemo;
			}
        }

		ArrayList m_smsReciver = new ArrayList();
		ArrayList m_faxReciver = new ArrayList();
        public void SetAddReceive(int nID, Sections.ExternalTeamData exData)
        {
            Sections.SectionDataExternal data = (Sections.SectionDataExternal)m_section.Data;

            if (nID == ID.ID_ITEM_RECEIVE_PHONE)
            {
                bool isCheck = false;
				foreach (Sections.ExternalTeamData SMSdata in m_smsReciver)
                {
                    if (SMSdata.TeamID == exData.TeamID)
                    {
                        isCheck = true;
                    }
                }
                if(!isCheck)
					m_smsReciver.Add(exData);
            }
            else
            {
                bool isCheck = false;
				foreach (Sections.ExternalTeamData Faxdata in m_faxReciver)
                {
                    if (Faxdata.TeamID == exData.TeamID)
                    {
                        isCheck = true;
                    }
                }
                if(!isCheck)
					m_faxReciver.Add(exData);
            }
        }

        public void SetRemoveReceive(int nID, Sections.ExternalTeamData exData)
        {
            Sections.SectionDataExternal data = (Sections.SectionDataExternal)m_section.Data;

            if (nID == ID.ID_ITEM_RECEIVE_PHONE)
                RemoveExternalTeamData(exData, m_smsReciver);
            else
                RemoveExternalTeamData(exData, m_faxReciver);
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
