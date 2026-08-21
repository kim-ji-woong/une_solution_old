using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBUtility;
using System.Collections;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace HSMS
{
    public class LoginManager
    {
        private static LoginManager m_Instance = null;
        public static LoginManager Instance
        {
            get { return m_Instance; }
        }

        private NetworkManager m_NetMgr = null;
        private FormLoginMain m_frmMain = null;

        //private string m_szLoginCode = "";

        private int m_nLoginID = 0;
        private string m_strLoginTryID = "";
        private string m_strLoginUserID = "";
        private int m_nUserLevel = -1;

        private string m_szLoginID = "";

        public int LoginID
        {
            get { return m_nLoginID; }
        }
       
        public string LoginUserID
        {
            get { return m_strLoginUserID; }
        }

        public int LoginUserLevel
        {
            get { return m_nUserLevel; }
        }

        private bool m_bLoginState = false;
        public bool LoginState
        {
            get { return m_bLoginState; }
            set { m_bLoginState = value; }
        }

        public LoginManager(FormLoginMain frmMain)
        {
            m_Instance = this;           
            m_frmMain = frmMain;
            m_NetMgr = FormMain.Instance.NetMgr;
        }

        public void OnCheckLogin()
        {
            if (m_bLoginState == true)
            {
                m_NetMgr.CheckLogin(m_nLoginID, m_szLoginID);
            }
        }

        /*public bool RequestCode(string szUserID, string szEncrptPass)
        {
            string szKey = new string(new char[] { 'U', 'N', 'E', 'A', 'E', 'S', 'K', 'E', 'Y' });
            string key = "";
            UnE.Utility.Properties.GetProperty(szKey, ref key);
            string strEncrypt = DBUtility.AES256Cipher.AES_encrypt(szEncrptPass, key);
            return m_NetMgr.RequestCode(szUserID, strEncrypt);
        }*/

        private ArrayList GetMacAddressList()
        {
            ArrayList arrMacAddrList = new ArrayList();
            System.Net.NetworkInformation.NetworkInterface[] adapters = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();

            ArrayList arrRemove = new ArrayList();

            foreach (System.Net.NetworkInformation.NetworkInterface adapter in adapters)
            {
                System.Net.NetworkInformation.PhysicalAddress addr = adapter.GetPhysicalAddress();

                if (addr != null && !addr.ToString().Equals(""))
                {
                    string strMacAddr = addr.ToString();

                    if (arrMacAddrList.Contains(strMacAddr))
                    {
                        if (!arrRemove.Contains(strMacAddr))
                            arrRemove.Add(strMacAddr);
                    }
                    else
                        arrMacAddrList.Add(strMacAddr);
                }
            }

            foreach (string strMacAddr in arrRemove)
            {
                arrMacAddrList.Remove(strMacAddr);
            }

            return arrMacAddrList;
        }

        public bool RequestLogin(string strUserID, string strEncryptedPW)
        {
            ArrayList arrMacAddrList = GetMacAddressList();

            if (arrMacAddrList.Count == 0)
                return false;

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(strUserID);
            arrDatas.Add(strEncryptedPW);
            arrDatas.Add(FormMain.Instance.SiteID);
            
            foreach (string strMacAddr in arrMacAddrList)
            {
                arrDatas.Add(strMacAddr);
            }

            byte[] bytes = ClientProvider.MakeBytes(TCP_ID.LOGIN_USER, arrDatas);
            return m_NetMgr.Send(bytes, m_NetMgr.ClientProvider) > 0;
        }

        /*public void OnResultCode(string szCode)
        {
            m_frmMain.Invoke((MethodInvoker)delegate
            {
                m_szLoginCode = szCode;
                m_frmMain.SetCode(szCode);
            });
        }*/

        public void OnAcceptLogin(int nUserID, string szUserID, int nUserLevel)
        {
            m_nLoginID = nUserID;
            m_szLoginID = m_strLoginTryID;
            m_strLoginUserID = szUserID;
            m_nUserLevel = nUserLevel;
            m_bLoginState = true;

            UnE.Utility.Properties.SetProperty("isAdmin", m_nUserLevel);

            m_frmMain.Invoke((MethodInvoker)delegate
            {
                m_frmMain.AcceptLogin(m_strLoginUserID);
            });
        }

        public void OnRejectLogin(int nType)
        {
            if (m_frmMain != null)
            {
                Thread t = new Thread(RejectLoginThread);
                t.Start(nType);
            }
        }

        private void RejectLoginThread(object param)
        {
            int nType = (int)param;
            m_frmMain.Invoke((MethodInvoker)delegate
            {
                m_frmMain.RejectLogin(nType);
            });
        }

        public bool JoinUser(string szMemberID, string szPass, int nUserLevel, ArrayList arrMacAddrList, UnE.KeyValidator.CertOption option)
        {
            string szKey = new string(new char[] { 'U', 'N', 'E', 'A', 'E', 'S', 'K', 'E', 'Y' });
            string key = "";
            UnE.Utility.Properties.GetProperty(szKey, ref key);
            string strEncrypt = DBUtility.AES256Cipher.AES_encrypt(szPass, key);

            return m_NetMgr.RegisterUser(szMemberID, strEncrypt, nUserLevel, arrMacAddrList, option);
        }

        public void OnJoinUser(int nFailType)
        {
            if (m_frmMain != null)
            {
                Thread t = new Thread(JoinUserthread);
                t.Start(nFailType);
            }
        }

        private void JoinUserthread(object param)
        {
            int nResult = (int)param;

            m_frmMain.Invoke((MethodInvoker)delegate
            {
                if (nResult < 0 || nResult >= (int)JoinUserResult.TYPE_COUNT)
                {
                    MessageBox.Show("계정 생성에 실패하였습니다.");
                    return;
                }

                JoinUserResult result = (JoinUserResult)nResult;

                if (result == JoinUserResult.SUCCESS)
                {
                    m_frmMain.SuccessRegisterUser();
                }
                else
                {
                    m_frmMain.FailRegisterUser(result);
                }
            });
        }

        public bool LogIn(string strID, string strEncrypt, string szCode, bool isEncryptPass)
        {
            if (isEncryptPass)
            {
                m_strLoginTryID = strID;

                return m_NetMgr.LoginUser(strID, strEncrypt, szCode);
            }
            else
            {
                return LogIn(strID, strEncrypt, szCode);
            }            
        }

        public bool LogIn(string strID, string strPassword, string szCode)
        {
            string szKey = new string(new char[] { 'U', 'N', 'E', 'A', 'E', 'S', 'K', 'E', 'Y' });
            string key = "";
            UnE.Utility.Properties.GetProperty(szKey, ref key);
            string strEncrypt = DBUtility.AES256Cipher.AES_encrypt(strPassword, key);
            m_strLoginTryID = strID;
            return m_NetMgr.LoginUser(strID, strEncrypt, szCode);
        }

        /*public bool SetPassword(string szGenID, string szNewPass)
        {
            string szKey = new string(new char[] { 'U', 'N', 'E', 'A', 'E', 'S', 'K', 'E', 'Y' });
            string key = "";
            UnE.Utility.Properties.GetProperty(szKey, ref key);
            string strEncrypt2 = DBUtility.AES256Cipher.AES_encrypt(szNewPass, key);
            return m_NetMgr.SetPassword(szGenID, strEncrypt2);
        }*/

        public bool ChangePassword(string szUserID, string strCertCode, string strMacAddrList, string szNewPass)
        //public bool ChangePassword(string szUserID, string szPass, string szNewPass)
        {
            string szKey = new string(new char[] { 'U', 'N', 'E', 'A', 'E', 'S', 'K', 'E', 'Y' });
            string key = "";
            UnE.Utility.Properties.GetProperty(szKey, ref key);
            //string strEncrypt1 = DBUtility.AES256Cipher.AES_encrypt(szPass, key);
            string strEncrypt2 = DBUtility.AES256Cipher.AES_encrypt(szNewPass, key);

            return m_NetMgr.ChangePassword(szUserID, strCertCode, strMacAddrList, strEncrypt2);
        }

        public void OnChangePassword(int nSuccess)
        {
            if (m_frmMain != null)
            {
                Thread t = new Thread(ChangePasswordThread);
                t.Start(nSuccess);
            }
        }

        private void ChangePasswordThread(object param)
        {
            int nSuccess = (int)param;
            m_frmMain.Invoke((MethodInvoker)delegate
            {
                if (nSuccess == 0)
                {                    
                    m_frmMain.SuccessChangePassword();                   
                }
                else
                {
                    m_frmMain.FailChangePassword(nSuccess);
                }
            });
        }

        public bool LogOut()
        {
            m_bLoginState = false;
            return m_NetMgr.Logout(m_strLoginUserID);
        }

        public void OnLogout()
        {
            m_bLoginState = false;
            if (m_frmMain != null && !m_frmMain.IsDisposed)
            {
                try
                {
                    m_frmMain.Invoke((MethodInvoker)delegate
                    {
                        m_frmMain.SetLogout();
                    });
                }
                catch (System.Exception)
                {

                }
            }
        }

        public bool DeleteUser(string szUserID, string szPass)
        {
            string szKey = new string(new char[] { 'U', 'N', 'E', 'A', 'E', 'S', 'K', 'E', 'Y' });
            string key = "";
            UnE.Utility.Properties.GetProperty(szKey, ref key);
            string strEncrypt = DBUtility.AES256Cipher.AES_encrypt(szPass, key);
            return m_NetMgr.DeleteUser(szUserID, strEncrypt);
        }

        public void OnDeleteUser(int nResult, string strUserID)
        {
            
            m_frmMain.Invoke((MethodInvoker)delegate
            {
                m_frmMain.DeleteUser(nResult, strUserID);
            });
        }
    }
}
