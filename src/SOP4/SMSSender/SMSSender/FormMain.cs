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
using UnE.SOP;


namespace SMSSender
{
    public partial class FormMain : Form
    {
        // 입력된 문자길이가 80바이트가 넘는지 여부
        private bool m_bLengthOver = false;

        private string m_szServerIP = "127.0.0.1";

        private DBUtility.WebDBManager m_dbMgr = new DBUtility.WebDBManager(LoadSiteID());
        private UnE.SOP.SOPManager m_SopManager = null;

        private int m_nSiteID = 1;

        private FormReciver m_ReciverForm = null;
        
        // 전송할 대상 전화번호 목록
        private List<string> m_ReciverPhoneNumbers = null;

        // 전송할 ReciverListItem 목록
        private ArrayList m_arRecivers = new ArrayList();

        private DBUtility.VariousData<Point> m_startLocation = null;
        private string m_strDefaultSender = "";

        public Point StartLocation
        {
            set { m_startLocation = new DBUtility.VariousData<Point>(value); }
        }

        public FormMain()
        {
            InitializeComponent();

            m_nSiteID = LoadSiteID();
            UnE.SOP.ProxySOP.Instance.SiteID = m_nSiteID;
            m_szServerIP = GetServerIP();

            DBUtility.WebDBManager dbMgr = new DBUtility.WebDBManager(m_nSiteID);
            m_SopManager = new UnE.SOP.SOPManager(m_dbMgr);
            m_SopManager.LoadRegularMember();
            m_SopManager.LoadOtherTeams();
            m_SopManager.LoadExternalCompany();
            m_SopManager.LoadControlRoom();
            m_SopManager.LoadControlRoomMembers();
        }

        private void ReadSiteID()
        {
            DBUtility.Utility util = new DBUtility.Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {
                MessageBox.Show("Site ID가 지정되지 않았습니다. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            int nSiteId = 1;
            if (int.TryParse(szSiteID, out nSiteId))
            {
                UnE.SOP.ProxySOP.Instance.SiteID = nSiteId;
                m_nSiteID = nSiteId;
            }
            else
            {
                MessageBox.Show("잘못된 Site ID입니다.. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
        }

        public static int LoadSiteID()
        {
            DBUtility.Utility ini = new DBUtility.Utility();
            string strSiteID = ini.getinivalue("Server Connection Info", "siteid");
            //string strSiteID = m_dbMgr.LoadIni("siteid", "Server Connection Info");

            int nSiteID = 1;

            if (strSiteID.Length > 0)
            {
                int.TryParse(strSiteID, out nSiteID);
            }

            return nSiteID;
        }

        private string GetServerIP()
        {
            string strServerURL = DBUtility.RegUtil.ReadRegValue("Server Connection Info", "webserver_url", m_nSiteID);
            if (strServerURL == null || strServerURL == "")
                strServerURL = "192.168.0.195";

            int nIndex1 = strServerURL.IndexOf("http://");
            int nIndex2 = strServerURL.LastIndexOf(':');
            string strURL = strServerURL;

            if (nIndex1 >= 0 && nIndex2 >= 0)
            {
                int nBeginIndex = nIndex1 + "http://".Length;
                strURL = strServerURL.Substring(nBeginIndex, nIndex2 - nBeginIndex);
            }
            else if (nIndex1 >= 0)
            {
                int nBeginIndex = nIndex1 + "http://".Length;
                strURL = strServerURL.Substring(nBeginIndex);
            }
            else if (nIndex2 >= 0)
            {
                strURL = strServerURL.Substring(0, nIndex2);
            }

            System.Net.IPAddress[] addr = System.Net.Dns.GetHostAddresses(strURL);

            m_szServerIP = addr[0].ToString();

            return m_szServerIP;
        }

        private void SendMessage(string szMessage, List<string> memberList)
        {                       
            string szCaller = textBoxSender.Text;
            // 1. 메시지는 libSMS에서 알아서 쪼개도록 한다.
            // 2. 여러명에게 보낼 경우 인원수만큼 Loop를 돌리되 문자는 libSMS에서 한꺼번에 보내도록 한다.
            using (libSMS.IMessageClient client = libSMS.MessageClientFactory.CreateMessageClient(UnE.SOP.ProxySOP.Instance.SiteID, m_szServerIP))
            {
                client.BeginSend();

                for (int i = 0; i < memberList.Count; i++)
                {
                    client.SendSMS(szCaller, (string)memberList[i], szMessage);
                }

                client.EndSend();
            }
            // 1. 메시지를 80바이트 단위로 호출하는 쪽에서 쪼갠다.
            // 2. 여러명에게 보낼 경우 인원수만큼 Loop를 돌리면서 각각 보낸다.
            /*ArrayList arrMessages = (new SOPServer.Data.MessageDivider()).MakeMessageList(szMessage);

            if (arrMessages == null)
                return;

            using (libSMS.IMessageClient client = libSMS.MessageClientFactory.CreateMessageClient(UnE.SOP.ProxySOP.Instance.SiteID, m_szServerIP))
            {

                foreach (string strMessage in arrMessages)
                {
                    for (int i = 0; i < memberList.Count; i++)
                    {
                        client.SendSMS(szCaller, (string)memberList[i], strMessage);
                    }

                }
            }*/       
        }

        public string ValidPhoneNumber(string strPhoneNumber, out bool isValid)
        {
            isValid = true;

            string strResult = "";
            int nLen = strPhoneNumber.Length;

            for (int i = 0; i < nLen; i++)
            {
                char ch = strPhoneNumber.ElementAt(i);

                if (ch != ' ' && ch != '\t' && ch != '-')
                {
                    if (ch >= '0' && ch <= '9')
                        strResult += ch;
                    else
                    {
                        isValid = false;
                        return "";
                    }
                }
            }

            return strResult;
        }
        
        private void btnMsgSend_Click(object sender, EventArgs e)
        {
            string szCaller = textBoxSender.Text;
            if( szCaller == null || szCaller == "")
            {
                MessageBox.Show(this, "발신자가 없습니다.\n발신자 번호를 입력해 주세요.", "메시지 전송", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                bool bValid = false;
                szCaller = ValidPhoneNumber(szCaller.Trim(), out bValid);

                if (!bValid)
                {
                    textBoxSender.Focus();
                    string szMessage = "발신자 번호에 잘못된 번호가 있습니다.";
                    MessageBox.Show(this, szMessage + "\n발신자 번호를 확인하십시요.", "발신자 번호 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            string szReciver = this.textboxReciver.Text;
            if (szReciver == null || szReciver == "" )
            {
                MessageBox.Show(this, "수신자가 없습니다.\n수신자 번호를 입력해 주세요.", "메시지 전송", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            //if (m_ReciverPhoneNumbers == null || m_ReciverPhoneNumbers.Count == 0)
            //{
            //    MessageBox.Show(this, "수신자가 없습니다.\n수신자 번호를 입력해 주세요.", "메시지 전송", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}

            string szMsg = textboxContent.Text;
            if (szMsg == null || szMsg.Equals(""))
            {
                MessageBox.Show(this, "전송할 메시지가 없습니다.", "메시지 전송", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (m_bLengthOver == true)
            {
                /*if (MessageBox.Show(this, "메시지 길이가 80바이트가 넘습니다. \n메시지가 잘리거나 분할되어 전송됩니다.\n계속 하시겠습니까?", "메시지 전송", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                {
                    return;
                }*/
            }

            string[] nums = szReciver.Split(new char[]{';'}, StringSplitOptions.RemoveEmptyEntries);

            if (nums != null)
            {
                List<string> targetPhoneNumber = new List<string>();
                //targetPhoneNumber.AddRange(nums.ToList<string>());

                bool bInvalidPhoneNumber = false;
                foreach (string strNumber in nums)
                {
                    bool bValid = false;
                    string strPhoneNumber = ValidPhoneNumber(strNumber.Trim(), out bValid);
                    if (bValid == true)
                        targetPhoneNumber.Add(strPhoneNumber);
                    else
                    {
                        bInvalidPhoneNumber = true;
                    }
                }

                if (bInvalidPhoneNumber == false)
                {
                    if (targetPhoneNumber.Count >= 1)
                    {
                        string szMessage = string.Format("총 {0} 명에게 전송합니다.", targetPhoneNumber.Count);
                        if (MessageBox.Show(this, szMessage + "\n계속 하시겠습니까?", "메시지 전송", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            SendMessage(szMsg, targetPhoneNumber);
                        }
                    }
                }
                else
                {
                    textboxReciver.Focus();
                    string szMessage = "수신자 번호에 잘못된 번호가 있습니다.";
                    if (MessageBox.Show(this, szMessage + "\n수신자 번호를 확인하십시요.", "수신자 번호 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation) == DialogResult.OK)
                    {
                       
                    }
                }
            }          
        }

        private void btnClearReciver_Click(object sender, EventArgs e)
        {
            if (textboxReciver.Text == "")
                return;

            if(MessageBox.Show(this, "전체 수신자 리스트를 삭제합니다.\n계속하시겠습니까?", "삭제확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                textboxReciver.Clear();
                m_arRecivers.Clear();
            }
        }
        
        private List<string> MakePhoneNumber(ArrayList arRecivers)
        {
            List<string> arPhoneNUmber = new List<string>();
            Dictionary<string, object> validNumber = new Dictionary<string,object>();
            foreach(ReciverListItem item in arRecivers)
            {
                if(item.Type == 1)
                {
                    Data_CompanyMember member = (Data_CompanyMember)item.TargetObject;
                    if(member.PhoneNumber != null && member.PhoneNumber != "")
                    {
                         if(!validNumber.ContainsKey(member.PhoneNumber))
                         {
                             validNumber.Add(member.PhoneNumber, member);
                         }
                    }                   
                    
                }
                else if(item.Type == 2)
                {
                    ExternalCompanyMember member = (ExternalCompanyMember)item.TargetObject;
                    if(member.PhoneNumber != null && member.PhoneNumber != "")
                    {
                         if(!validNumber.ContainsKey(member.PhoneNumber))
                         {
                             validNumber.Add(member.PhoneNumber, member);
                         }
                    }                   
                    
                }
                else if(item.Type == 3)
                {
                    Data_RegularTeam team = (Data_RegularTeam)item.TargetObject;
                    if(team != null)
                    {
                        ArrayList arMembers = new ArrayList();
                        GetRegularTeamCompanyMembers(team, arMembers);
                        //m_SopManager.GetCompanyMemberList(team.ID, true, ref arMembers);
                        foreach(Data_CompanyMember member in arMembers)
                        {
                            if(member.PhoneNumber != null && member.PhoneNumber != "")
                            {
                                if(!validNumber.ContainsKey(member.PhoneNumber))
                                {
                                    validNumber.Add(member.PhoneNumber, member );
                                }
                            }
                        }                        
                    } 
                }   
                else if(item.Type == 4)
                {
                    ExternalCompanyTeam team = (ExternalCompanyTeam)item.TargetObject;
                    if(team != null)
                    {
                        ArrayList arrMembers = new ArrayList();
                        GetExternalTeamCompanyMembers(team, arrMembers);

                        foreach (ExternalCompanyMember member in arrMembers)//team.Members)
                        {
                            if(member.PhoneNumber != null && member.PhoneNumber != "")
                            {
                                if(!validNumber.ContainsKey(member.PhoneNumber))
                                {
                                    validNumber.Add(member.PhoneNumber, member );
                                }
                            }                            
                        }                        
                    } 
                } 
                else if(item.Type == 5)
                {
                    string szPhoneNumber = (string)item.TargetObject;
                    if(szPhoneNumber != null && szPhoneNumber != "")
                    {
                        if(!validNumber.ContainsKey(szPhoneNumber))
                        {
                            validNumber.Add(szPhoneNumber, null);
                        }                                                
                    } 
                }
            }

            arPhoneNUmber.AddRange(validNumber.Keys);
            return arPhoneNUmber;
        }

        private void GetExternalTeamCompanyMembers(ExternalCompanyTeam team, ArrayList arrMembers)
        {
            List<ExternalCompanyTeam> teams = m_SopManager.GetExternalCompanyTeams(team.CompanyID);
            
            foreach (ExternalCompanyTeam _team in teams)
            {
                arrMembers.AddRange(_team.Members);
            }
        }

        private void GetRegularTeamCompanyMembers(Data_RegularTeam team, ArrayList arrMembers)
        {
            m_SopManager.GetRegularCompanyMemberList(team.ID, ref arrMembers);

            foreach (Data_RegularTeam childTeam in team.ChildTeams)
            {
                GetRegularTeamCompanyMembers(childTeam, arrMembers);
            }
        }

        private void btnAddReciver_Click(object sender, EventArgs e)
        {
            m_ReciverForm = new FormReciver(m_SopManager);
            m_ReciverForm.Recivers = m_arRecivers;

            if (m_ReciverForm.ShowDialog(this) == DialogResult.OK)
            {
                m_arRecivers.Clear();
                m_arRecivers.AddRange(m_ReciverForm.Recivers);

                // 전화번호 가져오기
                m_ReciverPhoneNumbers = MakePhoneNumber(m_arRecivers);
                if(m_ReciverPhoneNumbers != null)
                {
                    // text박스 설정
                    string szText = "";
                    int nCount = 0;
                    foreach (string szPhoneNumber in m_ReciverPhoneNumbers)
                    {
                        if (nCount == 0)
                        {
                            szText += szPhoneNumber;
                        }
                        else
                        {
                            szText += ";";
                            szText += szPhoneNumber;
                        }
                        nCount++;
                    }
                    textboxReciver.Text = szText;
                }               
            }
        }        

        private bool CheckHangul(char c)
        {
            if (char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.OtherLetter)
            {
                return true;
            }
            return false;
        }

        private void textboxContent_TextChanged(object sender, EventArgs e)
        {
            string szMessage = textboxContent.Text;
            int sumByte = 0;
            if (szMessage != null && !szMessage.Equals(""))
            {
                char[] charArr = szMessage.ToCharArray();
                foreach (char c in charArr)
                {
                    if (CheckHangul(c) == true)
                    {
                        sumByte += 2;
                    }
                    else
                        sumByte += 1;
                }
            }

            if (sumByte > 80)
            {
                lableLength.ForeColor = Color.Red;
                m_bLengthOver = true;
            }
            else
            {
                lableLength.ForeColor = Color.Blue;
                m_bLengthOver = false;
            }

            string szLength = string.Format("{0}/{1} {2}", sumByte, 80, "바이트");
            lableLength.Text = szLength;
        }

        private void btnMsgClear_Click(object sender, EventArgs e)
        {
            if (textboxContent.Text == "")
                return;

            if (MessageBox.Show(this,"작성된 문자 내용을 삭제합니다.\n계속하시겠습니까?", "삭제확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                textboxContent.Clear();
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            if (m_startLocation != null)
            {
                this.Location = m_startLocation.Data;
                textBoxSender.Text = m_strDefaultSender;
            }

            textBoxSender.Text = GetSMSCaller();
            textboxReciver.Focus();
        }

        private string GetSMSCaller()
        {
            string strSQL = "Select PropertyValue from OptionSDMS where PropertyName = 'SMSCaller' and SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return "";

            string strPhoneNumber = DBUtility.WebDBManager.GetStringField(arrResult[0]);
            return strPhoneNumber == null ? "" : strPhoneNumber;
        }

        public void SetStartLocation(string[] args)
        {
            if (args == null)
                return;

            int nCount = args.Count();

            if (nCount >= 2)
            {
                int x, y;

                if (!int.TryParse(args[0].Trim(), out x))
                    return;

                if (!int.TryParse(args[1].Trim(), out y))
                    return;

                m_startLocation = new DBUtility.VariousData<Point>(new Point(x, y));

                if (nCount >= 3)
                {
                    m_strDefaultSender = args[2].Trim();
                }
            }
        }
    }
}
