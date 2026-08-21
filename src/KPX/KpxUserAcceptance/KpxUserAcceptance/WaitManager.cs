using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility;
using System.Drawing;
using System.Collections;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace KpxUserAcceptance
{
    public class TabControlManager
    {
        protected class UserGroup
        {
            private int m_nID = -1;
            private string m_strGroupName = "";

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public string GroupName
            {
                get { return m_strGroupName; }
                set { m_strGroupName = value; }
            }

            public override string ToString()
            {
                return m_strGroupName;
            }

            public UserGroup()
            {
            }

            public UserGroup(int id, string groupName)
            {
                m_nID = id;
                m_strGroupName = groupName;
            }
        }

        protected DataGridView m_grid = null;
        protected WebDBManager m_dbMgr = null;

        protected static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });
        protected static UserGroup m_nullUserGroup = new UserGroup(-1, "없음");

        public DataGridView Grid
        {
            get { return m_grid; }
            set
            {
                m_grid = value;
                InitGridHandler();
            }
        }

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
            set { m_dbMgr = value; }
        }

        protected virtual void InitGridHandler()
        {
        }

        protected void SettingGridView(DataGridView gridView, string columnsName, string headerText, Color colHeaderBackground, int columnsWidth = 0)
        {
            gridView.Columns.Add(columnsName, headerText);
            gridView.Columns[columnsName].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridView.Columns[columnsName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridView.RowHeadersVisible = false;
            gridView.AllowUserToAddRows = false;
            gridView.RowHeadersVisible = false;
            gridView.Columns[columnsName].ReadOnly = true;
            //gridView.ReadOnly = true;
            gridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridView.BackgroundColor = Color.White;
            gridView.ColumnHeadersDefaultCellStyle.BackColor = colHeaderBackground;
            gridView.EnableHeadersVisualStyles = false;
            gridView.Columns[columnsName].SortMode = DataGridViewColumnSortMode.NotSortable;
            gridView.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            gridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            gridView.ColumnHeadersHeight = 40;
            //gridView.RowTemplate.Height = gridView.ColumnHeadersHeight = 40;
            gridView.MultiSelect = false;

            if (columnsWidth != 0)
            {

                gridView.Columns[columnsName].Width = columnsWidth;
                gridView.Columns[columnsName].MinimumWidth = columnsWidth;
            }
        }

        protected void gridCellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = m_grid.Rows[e.RowIndex];
            DataGridViewCell cell = row.Cells[e.ColumnIndex];

            if (cell is DataGridViewComboBoxCell)
            {
                if (cell.Value is string)
                {
                    DataGridViewComboBoxColumn col = (DataGridViewComboBoxColumn)m_grid.Columns[e.ColumnIndex];

                    foreach (UserGroup group in col.Items)
                    {
                        if (group.GroupName == cell.Value.ToString())
                        {
                            cell.Value = group;
                            break;
                        }
                    }
                }
            }
        }

        protected UserGroup GetUserGroup(int nGroupID)
        {
            DataGridViewComboBoxColumn col = (DataGridViewComboBoxColumn)m_grid.Columns[m_grid.Columns.Count - 1];

            foreach (UserGroup group in col.Items)
            {
                if (group.ID == nGroupID)
                    return group;
            }

            return null;
        }
    }

    public class WaitManager : TabControlManager
    {
        Random r = new Random(unchecked((int)DateTime.Now.Ticks) + 1); 

        public void InitGrid(Color colHeader)
        {
            //승인대기
            SettingGridView(m_grid, "Id", "ID", colHeader);
            SettingGridView(m_grid, "TeamName", "부서명", colHeader, 100);
            SettingGridView(m_grid, "UserName", "사용자명", colHeader, 70);
            SettingGridView(m_grid, "PhoneNumber", "핸드폰 번호", colHeader);
            SettingGridView(m_grid, "DeviceId", "장비ID", colHeader);

            DataGridViewCheckBoxColumn checkCol = new DataGridViewCheckBoxColumn();
            checkCol.Name = "AlarmAuth";
            checkCol.HeaderText = "알람해제";
            checkCol.ReadOnly = false;
            checkCol.TrueValue = true;
            checkCol.FalseValue = false;
            checkCol.Width = 80;
            m_grid.Columns.Add(checkCol);
            m_grid.Columns["AlarmAuth"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            checkCol = new DataGridViewCheckBoxColumn();
            checkCol.Name = "IsSms";
            checkCol.HeaderText = "문자수신";
            checkCol.ReadOnly = false;
            checkCol.TrueValue = true;
            checkCol.FalseValue = false;
            checkCol.Width = 80;
            m_grid.Columns.Add(checkCol);
            m_grid.Columns["IsSms"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            checkCol = new DataGridViewCheckBoxColumn();
            checkCol.Name = "Acceptance";
            checkCol.HeaderText = "승인";
            checkCol.ReadOnly = false;
            checkCol.TrueValue = true;
            checkCol.FalseValue = false;
            checkCol.MinimumWidth = 40;
            m_grid.Columns.Add(checkCol);
            m_grid.Columns["Acceptance"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_grid.Columns["Acceptance"].Width = 50;

            checkCol = new DataGridViewCheckBoxColumn();
            checkCol.Name = "Defer";
            checkCol.HeaderText = "보류";
            checkCol.TrueValue = true;
            checkCol.FalseValue = false;
            checkCol.Width = 40;
            m_grid.Columns.Add(checkCol);
            m_grid.Columns["Defer"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_grid.Columns["Defer"].Width = 50;

            m_grid.Columns["Id"].Visible = false;
            m_grid.Columns["DeviceId"].Visible = false;
            m_grid.CellContentClick += gridCellContentClick;

            DataGridViewComboBoxColumn comboCol = new DataGridViewComboBoxColumn();
            comboCol.Name = "colUserGroup";
            comboCol.HeaderText = "사용자 그룹";
            comboCol.Sorted = false;
            comboCol.ReadOnly = false;
            comboCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
            comboCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            comboCol.Items.Add(m_nullUserGroup);

            DataGridViewCellStyle comboDefCellStyle = new DataGridViewCellStyle();
            comboDefCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            comboCol.DefaultCellStyle = comboDefCellStyle;

            m_grid.CellEndEdit += gridCellEndEdit;

            m_grid.Columns.Add(comboCol);
            comboCol.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        void gridCellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (m_grid.Columns[e.ColumnIndex].Name == "AlarmAuth")
            {
                DataGridViewCheckBoxCell chk = m_grid.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewCheckBoxCell;

                if (Convert.ToBoolean(chk.Value))
                    chk.Value = false;
                else
                    chk.Value = true;
            }
            if (m_grid.Columns[e.ColumnIndex].Name == "IsSms")
            {
                DataGridViewCheckBoxCell chk = m_grid.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewCheckBoxCell;

                if (Convert.ToBoolean(chk.Value))
                    chk.Value = false;
                else
                    chk.Value = true;
            }
            if (m_grid.Columns[e.ColumnIndex].Name == "Acceptance")
            {
                DataGridViewCheckBoxCell chk = m_grid.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewCheckBoxCell;

                if (Convert.ToBoolean(chk.Value))
                    chk.Value = false;
                else
                    chk.Value = true;
            }
            if (m_grid.Columns[e.ColumnIndex].Name == "Defer")
            {
                DataGridViewCheckBoxCell chk = m_grid.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewCheckBoxCell;

                if (Convert.ToBoolean(chk.Value))
                    chk.Value = false;
                else
                    chk.Value = true;
            }
        }

        public void ReadUserGroups()
        {
            m_grid.Rows.Clear();

            DataGridViewComboBoxColumn col = (DataGridViewComboBoxColumn)m_grid.Columns[m_grid.Columns.Count - 1];
            int nItemCount = col.Items.Count;

            // 첫번째 아이템은 null data
            for (int i = nItemCount - 1; i >= 1; i--)
            {
                col.Items.RemoveAt(i);
            }

            string strSQL = "Select ID, GroupName from UserGroup";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                DBUtility.VariousData<int> id = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString());
                string strGroupName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);

                if (id == null || strGroupName == null)
                    continue;

                UserGroup group = new UserGroup();
                group.ID = id.Data;
                group.GroupName = strGroupName;

                col.Items.Add(group);
            }
        }

        public void Refresh()
        {
            try
            {
                m_grid.Rows.Clear();

                //ArrayList arrResult2 = m_dbMgr.GetResultData("select * from CertRequest", 0);
                string strQuery = "SELECT ID, TeamName, UserName, PhoneNumber, DeviceID, Defer, UserGroupID, SerialNumber FROM CertRequest WHERE CertCode IS NULL AND CertCodeLifeTime IS NULL AND MobileUserLevel IS NULL";

                ArrayList arrResult = m_dbMgr.GetResultData(strQuery, 0);
                if (arrResult == null) return;

                for (int i = 0; i < arrResult.Count - 7; i += 8)
                {
                    int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strTeamName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);
                    string strUserName = DBUtility.WebDBManager.GetStringField(arrResult[i + 2]);
                    string strPhoneNumber = DBUtility.WebDBManager.GetStringField(arrResult[i + 3]);
                    string strDeviceID = DBUtility.WebDBManager.GetStringField(arrResult[i + 4]);
                    if (strDeviceID == null) strDeviceID = string.Empty;
                    int nDefer = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                    int nUserGroupID = DBUtility.WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                    string strSerialNumber = DBUtility.WebDBManager.GetStringField(arrResult[i + 7]);

                    if (strSerialNumber == null)
                        continue;

                    string strDECPhoneNumber = string.Empty;
                    if (strPhoneNumber.Length > 0)
                        strDECPhoneNumber = DBUtility.AES256Cipher.AES_decrypt(strPhoneNumber, key);

                    UserGroup group = GetUserGroup(nUserGroupID);
                    int nRowIndex = m_grid.Rows.Add(nID, strTeamName, strUserName, strDECPhoneNumber, strDeviceID, false, false, (nDefer == 1) ? true : false, false, group);

                    if (nRowIndex >= 0)
                    {
                        DataGridViewRow row = m_grid.Rows[nRowIndex];
                        row.Tag = strSerialNumber;
                    }
                }
            }
            catch (ApplicationException app)
            {
                MessageBox.Show(app.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Save()
        {
            try
            {
                List<string> querys = new List<string>();
                List<string> trueList = new List<string>();
                List<string> falseList = new List<string>();

                int chgCnt = 0;

                bool IsNotUserGroupID = false;
                foreach (DataGridViewRow row in m_grid.Rows)
                {
                    int id = Convert.ToInt32(row.Cells["Id"].Value);
                    string phoneNumber = row.Cells["PhoneNumber"].Value.ToString();
                    string deviceId = row.Cells["DeviceId"].Value.ToString();
                    DataGridViewCheckBoxCell alarmAuthRow = row.Cells["AlarmAuth"] as DataGridViewCheckBoxCell;
                    DataGridViewCheckBoxCell isSmsRow = row.Cells["IsSms"] as DataGridViewCheckBoxCell;
                    DataGridViewCheckBoxCell acceptanceRow = row.Cells["Acceptance"] as DataGridViewCheckBoxCell;
                    DataGridViewCheckBoxCell deferRow = row.Cells["Defer"] as DataGridViewCheckBoxCell;
                    DataGridViewComboBoxCell userGroupCell = row.Cells[row.Cells.Count - 1] as DataGridViewComboBoxCell;
                    string strUserName = row.Cells["UserName"].Value.ToString();
                    string strSerialNumber = (string)row.Tag;

                    if (strSerialNumber == null)
                        continue;
                     
                    if (Convert.ToBoolean(deferRow.Value))
                    {
                        m_dbMgr.GetResultData("UPDATE CertRequest SET Defer=1 WHERE ID=" + id, 0);
                        chgCnt++;
                        continue;
                    }
                    if (Convert.ToBoolean(acceptanceRow.Value))
                    {
                        int nUserGroupID = -1;
                        if (userGroupCell.Value != null && userGroupCell.Value is UserGroup)
                        {
                            UserGroup group = (UserGroup)userGroupCell.Value;
                            nUserGroupID = group.ID;
                        }
                        if (nUserGroupID < 0 || userGroupCell == null || userGroupCell.Value == null)
                        {
                            IsNotUserGroupID = true;
                            continue;
                        }

                        int nMobileUserLevel = (Convert.ToBoolean(alarmAuthRow.Value)) ? 0 : 1;
                        int nSMS = (Convert.ToBoolean(isSmsRow.Value)) ? 1 : 0;

                        // 승인
                        if (CertDirect(id, strUserName, deviceId, nMobileUserLevel, phoneNumber, strSerialNumber, nSMS, nUserGroupID))
                        {
                            SendNotification(deviceId, "인증이 완료되었습니다.");
                            SendMessage("인증이 완료되었습니다.", phoneNumber);

                            trueList.Add(phoneNumber);
                            chgCnt++;
                        }
                        /*int certCode = r.Next(100000, 999999);
                         
                        querys.Add(string.Format("UPDATE CertRequest SET CertCode={0}, CertCodeLifeTime=date_format(date_add(now(), interval 10 minute), '%Y%m%d%H%i%s'), MobileUserLevel={1}, IsSms={2}, UserGroupID={4} WHERE ID={3}"
                            , certCode, (Convert.ToBoolean(alarmAuthRow.Value)) ? 0 : 1, (Convert.ToBoolean(isSmsRow.Value)) ? 1 : 0, id, nUserGroupID));

                        SendNotification(deviceId, "PTMS 인증코드 발송[" + certCode + "] 수신 후 10분내 입력해주세요.");
                        SendMessage("PTMS 인증코드 발송[" + certCode + "] 수신 후 10분내 입력해주세요.", phoneNumber);

                        trueList.Add(phoneNumber);
                        chgCnt++;*/
                    }
                    else
                    {
                        // 승인 거절 
                        querys.Add("DELETE FROM CertRequest WHERE ID = " + id);

                        SendNotification(deviceId, "PTMS 시스템에서 인증 거부되었습니다.");
                        SendMessage("PTMS 시스템에서 인증 거부되었습니다. ", phoneNumber);

                        falseList.Add(phoneNumber);
                        chgCnt++;
                    }
                }

                foreach (string item in querys)
                {
                    m_dbMgr.GetResultData(item, 0);
                }

                //SendMessage("PTMS 인증코드 발송[" + certCode + "] 수신 후 10분내 입력해주세요.", trueList);
                //SendMessage("승인 거절", falseList);                

                if (chgCnt == 0)
                {
                    if (IsNotUserGroupID)
                        MessageBox.Show("사용자 그룹을 선택하세요.");
                    else
                        MessageBox.Show("처리할 내역이 없습니다.");
                }
                else
                {
                    Refresh();

                    if (IsNotUserGroupID)
                        MessageBox.Show((chgCnt > 0) ? "사용자 그룹을 선택하지 않은 항목을 제외하고 적용되었습니다." : "사용자 그룹을 선택하세요.");
                    else
                        MessageBox.Show("적용되었습니다.");
                }
            }
            catch (ApplicationException app)
            {
                MessageBox.Show(app.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool CertDirect(int nRequestID, string strUserName, string strDeviceID, int nMobileUserLevel, string strPhoneNumber, string strSerialNumber,int nSMS, int nUserGroupID)
        {
            string strEncPhoneNumber = DBUtility.AES256Cipher.AES_encrypt(strPhoneNumber, key);

            if (m_dbMgr.BeginBatch() == false)
                return false;

            string strSQL = "Select max(ID) from User";
            ArrayList arrResult = m_dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
            {
                m_dbMgr.BatchRollback();
                return false;
            }

            int nID = 1;

            if (arrResult.Count > 0)
                nID = WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

            strSQL = "Insert into User (ID, UserName, Mobile, CompanyMemberID, ExternalMemberID, DeviceID, MobileUserLevel, PhoneNumber, SerialNumber, IsSms, UserGroup) values ";
            strSQL += string.Format("({7}, '{0}', 1, NULL, NULL, '{1}', {2}, '{3}', '{4}', {5}, {6})",
                strUserName, strDeviceID, nMobileUserLevel, strEncPhoneNumber, strSerialNumber, nSMS, nUserGroupID, nID);

            if (m_dbMgr.GetBatchData(strSQL) == null)
            {
                m_dbMgr.BatchRollback();
                return false;
            }

            strSQL = "Delete from CertRequest where ID = " + nRequestID.ToString();

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
            {
                m_dbMgr.BatchRollback();
                return false;
            }

            m_dbMgr.BatchCommit();
            return true;
        }

        private void SendMessage(string szMessage, List<string> memberList)
        {
            string szCaller = "01057891562";
            ArrayList arrMessages = (new SOPServer.Data.MessageDivider()).MakeMessageList(szMessage);

            if (arrMessages == null)
                return;

            using (libSMS.IMessageClient client = libSMS.MessageClientFactory.CreateMessageClient(500, "127.0.0.1"))
            {
                foreach (string strMessage in arrMessages)
                {
                    for (int i = 0; i < memberList.Count; i++)
                    {
                        client.SendSMS(szCaller, (string)memberList[i], strMessage);
                    }
                }
            }
        }
        private void SendMessage(string szMessage, string memberList)
        {
            string szCaller = "01057891562";
            ArrayList arrMessages = (new SOPServer.Data.MessageDivider()).MakeMessageList(szMessage);

            if (arrMessages == null)
                return;

            using (libSMS.IMessageClient client = libSMS.MessageClientFactory.CreateMessageClient(500, "127.0.0.1"))
            {
                foreach (string strMessage in arrMessages)
                {
                    client.SendSMS(szCaller, memberList, strMessage);
                    //for (int i = 0; i < memberList.Count; i++)
                    //{
                    //    client.SendSMS(szCaller, (string)memberList[i], strMessage);
                    //}
                }
            }
        }

        public string SendNotification(string deviceId, string message)
        {
            string SERVER_API_KEY = "AAAAu97zr8E:APA91bFwR605Gsk_WmWQmnvvAcQGoRE_zlFnBXNH0v3LsPzgA-WthiYpVLNXe6YgIxc5-mLwXyHL0bnSvzOxsGfymbKdeeHyAPpi0KQR3TTvqPx5siemrgMUTJKReZryQr-mabibmJTo";

            var value = message;
            string resultStr = "";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            request.Method = "POST";
            request.ContentType = "application/json;charset=utf-8;";
            request.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));

            var postData =
            new
            {
                /*data = new
                {
                    title = "KPX Message",
                    //title = textBox1.Text,
                    body = message,
                },*/

                notification = new
                {
                    body = message,
                    title = "TankAlarm",
                },

                // FCM allows 1000 connections in parallel.
                to = deviceId
            };

            //Linq to json
            string contentMsg = JsonConvert.SerializeObject(postData);
            System.Diagnostics.Trace.WriteLine("contentMsg = " + contentMsg);

            Byte[] byteArray = Encoding.UTF8.GetBytes(contentMsg);
            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            try
            {
                WebResponse response = request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                resultStr = reader.ReadToEnd();
                System.Diagnostics.Trace.WriteLine("response: " + resultStr);
                reader.Close();
                responseStream.Close();
                response.Close();
            }
            catch (Exception e)
            {
                resultStr = "";
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            return resultStr;
        }
    }
}
