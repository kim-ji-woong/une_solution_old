using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;
using SDMS.Help;
using DBUtility2;

namespace SDMS
{
    public partial class FormSMSHistory : PopupFormBase
    {
        private int m_nSiteID = 1;

        private ManualManager m_manualManager = null;

        public FormSMSHistory()
        {
            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

            InitializeComponent();

            InitCtrlSize(this);
            SetChildCtrlResize(this, 810, 550);

            FormMain.Instance.CustomizeGridView(dataGridView1);
            SetGridView();
            LoadData();

            m_manualManager = new ManualManager(this);
        }

        private SmsHistory m_Data = null;
        internal void SetData(SmsHistory data)
        {
            m_Data = data;

            UpdateGrid();
            UpdateLabel();
        }

        private void SetGridView()
        {
            float sizePer = 1.0f;
            if (FormMain.Instance.Resolution == Resolution.FourK)
                sizePer = 2.0f;
            else if (FormMain.Instance.Resolution == Resolution.Other)
                sizePer = 1.5f;
            colSelect.Width = (int)(40 * sizePer);
            colNo.Width = (int)(40 * sizePer);
            colName.Width = (int)(106 * sizePer);
            colTeam.Width = (int)(100 * sizePer);
            colGrade.Width = (int)(60 * sizePer);
        }

        private string szCaller = "";
        private void LoadData()
        {
            WebDBManager webDBManager = FormMain.Instance.DBManager;

            string szSQL2 = "SELECT PropertyValue FROM OptionSDMS where PropertyName='SMSCaller' and SiteID = " + m_nSiteID.ToString();
            ArrayList arResult2 = webDBManager.GetResultData(szSQL2);
            if (arResult2 == null || arResult2.Count == 0)
            {
                szCaller = "07088982203";
            }
            else
            {
                szCaller = arResult2[0].ToString();
            }
        }
        private int m_nSelectedMemeber = 0;
        private void UpdateLabel()
        {
            m_nSelectedMemeber = 0;
            lbTotalMember.Text = string.Format("총 {0}명 중 ",(m_Data.ExteanlMemberList.Count + m_Data.CompanyMemberList.Count));
            lbSelectMember.Text = string.Format("{0}명 선택",m_nSelectedMemeber);
            lbSelectMember.Location = new Point(lbTotalMember.Location.X + lbTotalMember.Width, lbTotalMember.Location.Y);
            editOrgMsg.Text =  m_Data.Message;

            Zone zone = m_Data.Zone;
            if( zone.Building != null)
            {
                string szGroupName = zone.Building.BuildingGroup.BuildingGroupName;
                string szBuildingName = zone.Building.BuildingName;
                string szFloor = zone.Floor.ToString();

                lbBuildingGroup.Text = szGroupName;
                lbBuilding.Text = szBuildingName;
                lbFloor.Text = szFloor;
            }
            else
            {
                lbBuildingGroup.Text = "-";
                lbBuilding.Text = zone.ZoneName;
                lbFloor.Text = "실외";
            }            

            EquipmentZone equip = m_Data.EquipZone;
            if( equip == null)
            {
                lbEquipZone.Text = "수동신고";
            }
            else
            {
                lbEquipZone.Text = equip.ZoneName;
            }

            lbBuilding.Location = new Point(lbBuildingGroup.Location.X + lbBuildingGroup.Width, lbBuildingGroup.Location.Y);
            lbFloor.Location = new Point(lbBuilding.Location.X + lbBuilding.Width, lbBuilding.Location.Y);
            lbEquipZone.Location = new Point(lbFloor.Location.X + lbFloor.Width, lbFloor.Location.Y); 
        }

        internal class CompanyMemberLevelSort : IComparer
        {            
            int IComparer.Compare(Object x, Object y)
            {
                DataCompanyMember member1 = (DataCompanyMember)x;
                DataCompanyMember member2 = (DataCompanyMember)y;
                if (member1.LevelID < member2.LevelID)
                    return -1;
                else if (member1.LevelID > member2.LevelID)
                    return 1;
                return 0;
            }
        }        

        private void UpdateGrid()
        {
            int nCount = 1;

            ArrayList arCompanyMembers = m_Data.CompanyMemberList;
            ArrayList arMembers = new ArrayList();

            foreach( int nMemberID in arCompanyMembers)
            {
                DataCompanyMember member = FormMain.Instance.DataManager.GetCompanyMember(nMemberID);
                if (member != null)
                {
                    arMembers.Add(member);
                }
            }

            arMembers.Sort(new CompanyMemberLevelSort());

            foreach (DataCompanyMember member in arMembers)
            {                
                DataGridViewRow row = new DataGridViewRow();
                row.Tag = member;

                DataGridViewCheckBoxCell cell1 = new DataGridViewCheckBoxCell();
                cell1.Value = false;
                cell1.Tag = "내부";
                row.Cells.Add(cell1);

                DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                cell2.Value = nCount;
                row.Cells.Add(cell2);

                DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                cell3.Value = member.MemberName;
                row.Cells.Add(cell3);

                DataTeam team = member.GetFirstTeam();

                DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                cell4.Value = team == null ? "" : team.TeamName;
                row.Cells.Add(cell4);

                DataGridViewTextBoxCell cell5 = new DataGridViewTextBoxCell();
                cell5.Value = member.LevelID;
                row.Cells.Add(cell5);

                dataGridView1.Rows.Add(row);
                nCount++;
            }

            ArrayList arExternalMembers = m_Data.ExteanlMemberList;
            foreach (int nMemberID in arExternalMembers)
            {
                DataExternalMember member = FormMain.Instance.DataManager.GetExternalMember(nMemberID);
                if (member != null)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.Tag = member;

                    DataGridViewCheckBoxCell cell1 = new DataGridViewCheckBoxCell();
                    cell1.Value = false;
                    cell1.Tag = "외부";
                    row.Cells.Add(cell1);
                    
                    DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                    cell2.Value = nCount;
                    row.Cells.Add(cell2);

                    DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                    cell3.Value = member.Name;
                    row.Cells.Add(cell3);

                    //DataTeam team = member.GetFirstTeam();
                    DataTeam team = member.Team;

                    DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                    cell4.Value = team.TeamName;
                    row.Cells.Add(cell4);

                    DataGridViewTextBoxCell cell5 = new DataGridViewTextBoxCell();
                    cell5.Value = 100;
                    row.Cells.Add(cell5);

                    dataGridView1.Rows.Add(row);
                    nCount++;
                }
            }           
        }

        private void UpdateSelectInfo()
        {
            lbSelectMember.Text = string.Format("{0}명 선택", m_nSelectedMemeber);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int nCol = e.ColumnIndex;
            int nRow = e.RowIndex;

            if (nCol != 0 || nRow < 0)
                return;

            DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)dataGridView1.Rows[nRow].Cells[nCol];
            bool bValue = !(bool)cell.Value;
            cell.Value = bValue;

            if (bValue == true)
            {
                m_nSelectedMemeber += 1;
            }
            else
            {
                m_nSelectedMemeber -= 1;
            }

            UpdateSelectInfo();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
        }

        private ContextMenu m_PopupMenu = null;
        private MenuItem m_DetailMenu1 = null;
        private MenuItem m_DetailMenu2 = null;
        private MenuItem m_DetailMenu3 = null;

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {       
            if (e.Button == MouseButtons.Right)
            {
                if (m_PopupMenu == null)
                {
                    m_PopupMenu = new ContextMenu();
                    m_DetailMenu1 = new MenuItem("선택영역 체크하기");
                    m_DetailMenu1.Click += SetCheckAll;
                    m_PopupMenu.MenuItems.Add(m_DetailMenu1);

                    m_DetailMenu2 = new MenuItem("선택영역 선택반전");
                    m_DetailMenu2.Click += SetCheckRevers;
                    m_PopupMenu.MenuItems.Add(m_DetailMenu2);

                    m_DetailMenu3 = new MenuItem("선택영역 해제하기");
                    m_DetailMenu3.Click += SetCheckRelease;
                    m_PopupMenu.MenuItems.Add(m_DetailMenu3);
                }                

                int currentMouseOverRow = dataGridView1.HitTest(e.X, e.Y).RowIndex;
                if (currentMouseOverRow >= 0)
                {                   
                    DataGridViewRow row = dataGridView1.Rows[currentMouseOverRow];
                    if (row.Selected == false)
                    {                        
                        row.Selected = true;
                    }
                    Point pt = e.Location;
                    m_PopupMenu.Show(dataGridView1, pt);
                }      
            }
        }

        private void SetCheckAll(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rows = dataGridView1.SelectedRows;
            foreach (DataGridViewRow row in rows)
            {
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)row.Cells[0];

                bool bValue = (bool)cell.Value;
                if (bValue  == false)
                {
                    cell.Value = true;
                    m_nSelectedMemeber += 1;
                }
                
            }
            UpdateSelectInfo();
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            foreach(DataGridViewRow row in dataGridView1.Rows)
            {
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)row.Cells[0];
                bool bValue = (bool)cell.Value;
                if (bValue == false)
                {
                    cell.Value = true;
                    m_nSelectedMemeber += 1;
                }
            }
            UpdateSelectInfo();
        }

        private void SetCheckRevers(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rows = dataGridView1.SelectedRows;
            foreach (DataGridViewRow row in rows)
            {
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)row.Cells[0];
                bool bValue = !(bool)cell.Value;
                cell.Value = bValue;

                if (bValue == true)
                {
                    m_nSelectedMemeber += 1;
                }
                else
                {
                    m_nSelectedMemeber -= 1;
                }
            }
            UpdateSelectInfo();
        }
        
        private void btnSelectReverse_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)row.Cells[0];
                bool bValue = !(bool)cell.Value;
                cell.Value = bValue;

                if (bValue == true)
                {
                    m_nSelectedMemeber += 1;
                }
                else
                {
                    m_nSelectedMemeber -= 1;
                }
            }
            UpdateSelectInfo();
        }

        private void SetCheckRelease(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rows = dataGridView1.SelectedRows;
            foreach (DataGridViewRow row in rows)
            {
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)row.Cells[0];
                bool bValue = (bool)cell.Value;
                if (bValue == true)
                {
                    cell.Value = false;
                    m_nSelectedMemeber -= 1;
                }
            }
            UpdateSelectInfo();
        }

        private void btnReleaseAll_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)row.Cells[0];
                cell.Value = false;
            }

            m_nSelectedMemeber = 0;
            UpdateSelectInfo();
        }

        private List<libSMS.MessageContent> MakeMessage()
        {
            List<libSMS.MessageContent> list = new List<libSMS.MessageContent>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)row.Cells[0];
                bool bValue = (bool)cell.Value;
                if (bValue == true)
                {
                    libSMS.MessageContent msg = new libSMS.MessageContent();
                    msg.Caller = szCaller;

                    object obj = row.Tag;

                    if( obj.GetType() == typeof(DataCompanyMember))
                    {
                        DataCompanyMember member = (DataCompanyMember)obj;
                        msg.PhoneNumbers.Add(member.PhoneNumber);
                        //msg.Reciver = member.PhoneNumber;
                    }
                    else
                    {
                        DataExternalMember member = (DataExternalMember)obj;
                        msg.PhoneNumbers.Add(member.PhoneNumber);
                        //msg.Reciver = member.PhoneNumber;
                    }
                    //msg.EncryptCaller = false;
                    msg.Message = editNewMsg.Text;

                    list.Add(msg);
                }
            }
            return list;
        }

        private void btnSendSMS_Click(object sender, EventArgs e)
        {
            string szMessage = editNewMsg.Text;
            if (szMessage == "")
            {
                MessageBox.Show("문자내용이 없습니다.");
                return;
            }

            if( m_nSelectedMemeber <= 0)
            {
                m_nSelectedMemeber = 0;
                MessageBox.Show("지정된 인원이 없습니다.");
                return;
            }

            string szMsg = string.Format("지정된 인원 {0}명에게 전송을 합니다. 계속하시겠습니까?", m_nSelectedMemeber);

            if(MessageBox.Show(this, szMsg, "문자 전송", MessageBoxButtons.YesNo,MessageBoxIcon.Question ) == DialogResult.Yes)
            {
                string szServerIP = SDMS.NetworkWebManager.Instance.ServerIP;
                // send SMS    

                using(libSMS.IMessageClient client = libSMS.MessageClientFactory.CreateMessageClient(UnE.SOP.ProxySOP.Instance.SiteID))
                {
                    List<libSMS.MessageContent> arMessages = MakeMessage();
                    if (arMessages.Count > 0)
                    {
                        client.SendSMS(arMessages);
                    }
                }

                SaveSMSHistory(m_Data.SensorHistoryID, m_Data.ReactionHistoryID, editNewMsg.Text, 0);

                FormMain.Instance.PerformClickSelectReport();
            }


        }

        private void SaveSMSHistory(int SensorHistoryID, int nReactionHistoryID, string szMessage, int nSendType)
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strSQL = "select max(id) from SDMSSMSHistory";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null)
            {
                return;
            }

            string szCompanyMemberList = "";
            string szExternalMemberList = "";            
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)row.Cells[0];
                bool bValue = (bool)cell.Value;
                if (bValue == true)
                { 
                    object obj = row.Tag;
                    if (obj.GetType() == typeof(DataCompanyMember))
                    {
                        DataCompanyMember member = (DataCompanyMember)obj;
                        if (sb1.Length > 0)
                            sb1.Append(',');
                        sb1.Append(member.ID);
                    }
                    else
                    {
                        DataExternalMember member = (DataExternalMember)obj;
                        if (sb2.Length > 0)
                            sb2.Append(',');
                        sb2.Append(member.ID);
                    }                   
                }
            }

            szCompanyMemberList = sb1.ToString();
            szExternalMemberList = sb2.ToString();

            int nID = arrResult.Count == 0 ? 1 : WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

            strSQL = string.Format("Insert into SDMSSMSHistory (ID,SensorHistoryID, ReactionHistoryID, CompanyMemberIDList, ExternalCompanyMemberIDList, SMSMessage, SendType) values ({0}, {1}, {2}, '{3}', '{4}', '{5}', {6})",

                nID, SensorHistoryID, nReactionHistoryID, szCompanyMemberList, szExternalMemberList, szMessage, nSendType);

            if (dbMgr.GetResultData(strSQL) == null)
            {
                return;
            }
        }

        // ColumnHeader를 Click하여 정렬이 끝난후에 호출됨
        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // 번호를 새로 붙임
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells[1].Value = row.Index + 1;
            }
        }

        private void SetManualID()
        {
            m_manualManager.Handle = this.Handle;

            m_manualManager.Clear();

            m_manualManager.SetID(this, "SDMS_Report_SMS_Fire");
            m_manualManager.SetID(dataGridView1, "SDMS_Report_SMS_Fire");
            m_manualManager.SetID(btnSendSMS, "SDMS_Report_SMS_Fire"); 

            m_manualManager.ProcessEvent();
        }
    }
}
