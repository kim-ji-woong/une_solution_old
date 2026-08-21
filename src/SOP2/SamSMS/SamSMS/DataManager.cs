using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace SamSMS
{
    class DataManager
    {

        static private DataManager m_Instance;
        public SamSMS.DataManager Instance
        {
            get 
            {
                if (m_Instance == null)
                    m_Instance = new DataManager();    
                return m_Instance; 
            }
        }


        private string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });
        
        private WebDBManager m_dbMgr = new WebDBManager();

        /// <summary>
        /// 직급 정보, 휴직종류와 정보도 알수 있다.
        /// </summary>
        private Dictionary<int, DataJobPosition> dicJobPoistion = new Dictionary<int, DataJobPosition>();

        private Dictionary<int, DataJobLevel> dicJobLevel = new Dictionary<int, DataJobLevel>();

        /// <summary>
        /// 전체 팀의 정보, Parent ID가 -1인경우 최상위 팀  
        /// </summary>
        private Dictionary<int, DataTeam> dicTeam = new Dictionary<int, DataTeam>();

        private ArrayList m_arRegularTeam = new ArrayList();
        public System.Collections.ArrayList RegularTeamList
        {
            get { return m_arRegularTeam; }
            set { m_arRegularTeam = value; }
        }

        private ArrayList m_arExternalTeam = new ArrayList();
        public System.Collections.ArrayList ExternalTeamList
        {
            get { return m_arExternalTeam; }
            set { m_arExternalTeam = value; }
        }
        private ArrayList m_arrCompanyMember = new ArrayList();
        private ArrayList m_arExternalMember = new ArrayList();

        public int ExternalMember
        {
            get { return m_arExternalMember.Count; }
        }
        public int CompanyMember
        {
            get { return ( m_arrCompanyMember.Count ); }
        }
        
        public int TimeOffMember
        {
            get
            {
                int nCount = 0;
                foreach (DataCompanyMember data in m_arrCompanyMember)
                {
                    if (data.PositionID >= 100)
                    {
                        nCount++;
                    }
                }
                return nCount;
            }
        }

        public int LevelOne
        {
            get
            {
                int nCount = 0;
                foreach (DataCompanyMember data in m_arrCompanyMember)
                {
                    if( data.LevelID == 1)
                    {
                        nCount++;
                    }               
                }
                return nCount;
            }
        }

        public int LevelTwo
        {
            get
            {
                int nCount = 0;
                foreach (DataCompanyMember data in m_arrCompanyMember)
                {
                    if (data.LevelID == 2)
                    {
                        nCount++;
                    }  
                }
                return nCount;
            }
        }

        public int LevelThree
        {
            get
            {
                int nCount = 0;
                foreach (DataCompanyMember data in m_arrCompanyMember)
                {
                    if (data.LevelID == 3)
                    {
                        nCount++;
                    }  
                }
                return nCount;
            }
        }

        public int LevelFour
        {
            get
            {
                int nCount = 0;
                foreach (DataCompanyMember data in m_arrCompanyMember)
                {
                    if (data.LevelID >= 4 || data.LevelID == 0)
                    {
                        nCount++;
                    }  
                }
                return nCount;
            }
        }

        public DataManager()
        {            
            LoadJobPosition();
            LoadJobLevel();
            LoadAllTeam();

            LoadCompanyMember();
            LoadExternalMember();
        }

        public bool SendSMS(ArrayList arCall, string strSenderPhoneNumber, string strMsg)
        {
            m_dbMgr.SendSMS(arCall, strSenderPhoneNumber, strMsg);
            return true;
        }

        public bool SendSMS(string strPhoneNumber, string strSenderPhoneNumber, string strMsg)
        {
            string szResult =  m_dbMgr.SendSMS(strPhoneNumber, strSenderPhoneNumber, strMsg);
            return true;
        }

        private bool LoadJobLevel()
        {
            string strSQL = "SELECT ID, LevelName, LevelNo FROM JobLevel";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return false;

            int nCount = arrResult.Count;
            if (nCount == 0)
                return true;

            for (int i = 0; i < nCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string szLevelName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nLevel = WebDBManager.GetIntField(arrResult[i+2].ToString(), -1);

                DataJobLevel data = new DataJobLevel();
                data.ID = nID;
                data.LevelName = szLevelName;
                data.Level = nLevel;
                if (nID != -1)
                    dicJobLevel.Add(nID, data);
            }
            return true;
        }

        

        public bool LoadAllTeam()
        {
            m_arRegularTeam = LoadRegularTeam();
            m_arExternalTeam = LoadExternalTeam();
            return true;
        }

        private ArrayList LoadRegularTeam()
        {
            string szSQL = "SELECT R.ID, R.TeamName, R.ParentTeamID FROM RegularTeam as R";

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL, 0);
            if (arrResult == null)
                return null;

            ArrayList arResultTeamList = new ArrayList();
            int nCount = arrResult.Count;
            if (nCount == 0)
                return arResultTeamList;

            for (int i = 0; i < nCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string szTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                DataTeam data = new DataTeam();
                data.ID = nID;
                data.ParentTeamID = nParentTeamID;
                data.TeamName = szTeamName;
                data.External = false;
                arResultTeamList.Add(data);
            }        
            return arResultTeamList;
        }

        private Dictionary<int, DataTeam> LoadExternalCompanyToTeam()
        {
            string szSQLID = "SELECT TOP 1 ID FROM ExternalCompanyTeam ORDER BY ID Desc";
            ArrayList arrResult1 = m_dbMgr.GetResultData(szSQLID, 0);
            if (arrResult1 == null)
                return null;

            int nNextTeamID = WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
            if (nNextTeamID == -1)
                return null;
            nNextTeamID++;

            string szSQL = "SELECT DISTINCT C.ID, C.TeamName FROM ExternalCompanyTeam as E INNER JOIN ExternalTeam as C on CompanyID = C.ID";
            ArrayList arrResult = m_dbMgr.GetResultData(szSQL, 0);
            if (arrResult == null)
                return null;

            Dictionary<int, DataTeam> arResult = new Dictionary<int, DataTeam>();

            int nCount = arrResult.Count;
            if (nCount == 0)
                return arResult;

            for (int i = 0; i < nCount - 1; i += 2)            
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string szTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");

                DataTeam data = new DataTeam();
                data.ID = nNextTeamID;
                data.ParentTeamID = -1;
                data.TeamName = szTeamName;
                data.External = true;
                arResult.Add(nID, data);
                nNextTeamID++;
            }
            return arResult;
        }

        private ArrayList LoadExternalTeam()
        {
            Dictionary<int, DataTeam> dicTeamList = LoadExternalCompanyToTeam();
            if (dicTeamList == null)
                return null;            

            string szSQL = "SELECT E.ID, E.TeamName, E.ParentTeamID, E.CompanyID, C.TeamName " +
              "FROM ExternalCompanyTeam as E INNER JOIN ExternalTeam as C on E.CompanyID = C.ID";

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL, 0);
            if (arrResult == null)
                return null;

            ArrayList arResultTeamList = new ArrayList();
            int nCount = arrResult.Count;
            if (nCount == 0)
                return arResultTeamList;

            for (int i = 0; i < nCount - 4; i += 5)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string szTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nParentTeamID = WebDBManager.GetIntField(arrResult[i+2].ToString(), -1);
                int nCompanyID = WebDBManager.GetIntField(arrResult[i+3].ToString(), -1);
                string szCompanyName = WebDBManager.GetStringField(arrResult[i + 4], "");
                if (nParentTeamID == -1)
                {
                    DataTeam arData = dicTeamList[nCompanyID];
                    if (arData != null)
                    {
                        nParentTeamID = arData.ID;                            
                    }
                }

                DataTeam data = new DataTeam();
                data.ID = nID;
                data.ParentTeamID = nParentTeamID;
                data.TeamName = szTeamName;
                data.External = true;
                arResultTeamList.Add(data);
            }

            if (arResultTeamList.Count > 0)
            {
                foreach (KeyValuePair<int, DataTeam> pair in dicTeamList)
                {
                    arResultTeamList.Add(pair.Value);
                }
            }
            return arResultTeamList;
        }     
   

        public bool LoadJobPosition()
        {
            string strSQL = "SELECT ID, PositionName FROM JobPosition";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return false;
            
            int nCount = arrResult.Count;
            if (nCount == 0)
                return true;

            for (int i = 0; i < nCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string szPositionName = WebDBManager.GetStringField(arrResult[i + 1], "");

                DataJobPosition data = new DataJobPosition();
                data.ID = nID;
                data.PositionName = szPositionName;
                
                if( nID != -1)
                    dicJobPoistion.Add(nID, data);

            }
            return true;
        }

        public bool LoadExternalMember()
        {
            string szSQL = "SELECT ID, Name, PhoneNumber, IsTeamLeader, TeamID FROM ExternalCompanyMember";
            
            ArrayList arrResult = m_dbMgr.GetResultData(szSQL, 0);
            if (arrResult == null)
                return false;

            int nCount = arrResult.Count;
            if (nCount == 0)
                return true;

            for (int i = 0; i < nCount - 4; i += 5)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 1], "");
                string szPhoneNumber = WebDBManager.GetStringField(arrResult[i + 2].ToString(), "");
                bool nLeader = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0) == 1;
                int nTeamID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);

                if (string.Compare(szPhoneNumber, "null", true) == 0 || szPhoneNumber == "")
                    szPhoneNumber = "";
                else
                    szPhoneNumber = AES256Cipher.AES_decrypt(szPhoneNumber, key);

                szPhoneNumber = ValidPhoneNumber(szPhoneNumber);

                DataExternalMember data = new DataExternalMember();
                data.ID = nID;
                data.Name = strMemberName;
                data.PhoneNumber = szPhoneNumber;
                data.TeamLeader = nLeader;
                data.ExternalTeamID = nTeamID;

                m_arExternalMember.Add(data);

            }
            return false;
        }

        public bool LoadCompanyMember()
        {
            string strSQL = "select ID, MemberName, RegularTeamID, LevelID, PositionID, MemberID, SecondRegularTeamID, SecondPositionID, OfficePhoneNumber, PhoneNumber from CompanyMember";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            for (int i = 0; i < nCount - 9; i += 10)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nRegularTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                int nLevelID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0);
                int nPositionID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
                string strMemberID = WebDBManager.GetStringField(arrResult[i + 5], "");
                int nSecondRegularTeamID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);
                int nSecondPositionID = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
                string strOfficePhoneNumber = WebDBManager.GetStringField(arrResult[i + 8], "");
                //string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 9], "");
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 9], "");

                if (string.Compare(strPhoneNumber, "null", true) == 0 || strPhoneNumber == "")
                    strPhoneNumber = "";
                else
                    strPhoneNumber = AES256Cipher.AES_decrypt(strPhoneNumber, key);

                strPhoneNumber = ValidPhoneNumber(strPhoneNumber);

                if (string.Compare(strOfficePhoneNumber, "null", true) == 0)
                    strOfficePhoneNumber = "";

                DataCompanyMember data = new DataCompanyMember();
                data.ID = nID;
                data.MemberName = strMemberName;
                data.RegularTeamID = nRegularTeamID;
                data.LevelID = nLevelID;
                data.PositionID = nPositionID;
                data.MemberID = strMemberID;
                data.SecondRegularTeamID = nSecondRegularTeamID;
                data.SecondPositionID = nSecondPositionID;
                data.OfficePhoneNumber = strOfficePhoneNumber;
                data.PhoneNumber = strPhoneNumber;

                m_arrCompanyMember.Add(data);
                ////////////////////////////////////////////////////////////////
            }
            return true;
        }

        private string ValidPhoneNumber(string strPhoneNumber)
        {
            string strResult = "";
            int nLen = strPhoneNumber.Length;

            for (int i = 0; i < nLen; i++)
            {
                char ch = strPhoneNumber[i];

                if (ch != ' ' && ch != '\t' && ch != '-')
                    strResult += ch;
            }
            return strResult;
        }

        public ArrayList GetTargetMemberAll(bool bAll, bool bExternal, bool bTimeOff)
        {
            ArrayList arResult = new ArrayList();  
            foreach (DataCompanyMember data in m_arrCompanyMember)
            {
                if (bAll == true)
                {
                    if (data.PositionID < 100)
                    {
                        SendingMember send = new SendingMember();
                        send.PhoneNumber = data.PhoneNumber;
                        send.Name = data.MemberName;
                        send.TeamName = "ALL";
                        arResult.Add(send);
                    }
                }
                if (bTimeOff == true)
                {
                    if (data.PositionID >= 100)
                    {
                        SendingMember send = new SendingMember();
                        send.PhoneNumber = data.PhoneNumber;
                        send.Name = data.MemberName;
                        send.TeamName = "TimeOff";
                        arResult.Add(send);
                    }
                }                    
            }

            if (bExternal == true)
            {
                foreach (DataExternalMember data in m_arExternalMember)
                {
                    SendingMember send = new SendingMember();
                    send.PhoneNumber = data.PhoneNumber;
                    send.Name = data.Name;
                    send.TeamName = "External";
                    arResult.Add(send);
                }
            }
            return arResult;
        }

        public ArrayList GetTargetMemberTeam(bool bMember, bool bLeader, ArrayList arCheckedTeam)
        {
            ArrayList arResult = new ArrayList();

            Dictionary<int, DataCompanyMember> dicComMember = new Dictionary<int, DataCompanyMember>();
            foreach (DataTeam team in arCheckedTeam)
            {
                if (team.External == false) // Internal Team
                {
                    foreach (DataCompanyMember man in m_arrCompanyMember)
                    {
                        if (team.ID == man.RegularTeamID)
                        {
                            
                            if( man.PositionID == 2 || man.PositionID == 3 || man.PositionID == 4)
                            {
                                if( bLeader == true)
                                {
                                    if (!dicComMember.ContainsKey(man.ID))
                                    {
                                        dicComMember.Add(man.ID, man);
                                        SendingMember send = new SendingMember();
                                        send.PhoneNumber = man.PhoneNumber;
                                        send.Name = man.MemberName;
                                        send.TeamName = team.TeamName;
                                        arResult.Add(send); 
                                    }
                                }
                            }
                            else if( man.PositionID == 0 || man.PositionID == 1)
                            {
                                if (bMember == true)
                                {
                                    if (!dicComMember.ContainsKey(man.ID))
                                    {
                                        dicComMember.Add(man.ID, man);
                                        SendingMember send = new SendingMember();
                                        send.PhoneNumber = man.PhoneNumber;
                                        send.Name = man.MemberName;
                                        send.TeamName = team.TeamName;
                                        arResult.Add(send);
                                    }
                                }
                            }                            
                        } 
                    }
                }
            }

            Dictionary<int, DataExternalMember> dicExtMember = new Dictionary<int, DataExternalMember>();
            foreach (DataTeam team in arCheckedTeam)
            {
                if (team.External == true) // External Team
                {
                    foreach (DataExternalMember man in m_arExternalMember)
                    {
                        if (team.ID == man.ExternalTeamID)
                        {
                            if (man.TeamLeader == true && bLeader == true)
                            {
                                if (!dicExtMember.ContainsKey(man.ID))
                                {
                                    dicExtMember.Add(man.ID, man);
                                    SendingMember send = new SendingMember();
                                    send.PhoneNumber = man.PhoneNumber;
                                    send.Name = man.Name;
                                    send.TeamName = team.TeamName;
                                    arResult.Add(send);
                                }
                            }
                            if (man.TeamLeader == false && bMember == true)
                            {
                                if (!dicExtMember.ContainsKey(man.ID))
                                {
                                    dicExtMember.Add(man.ID, man);
                                    SendingMember send = new SendingMember();
                                    send.PhoneNumber = man.PhoneNumber;
                                    send.Name = man.Name;
                                    send.TeamName = team.TeamName;
                                    arResult.Add(send);
                                }
                            }                            
                        }
                    }
                }
            }
        
            return arResult;
        }

        public ArrayList GetTargetMemberLevel(bool bLevel1, bool bLevel2, bool bLevel3, bool bLevel4)
        {

            ArrayList arResult = new ArrayList();

            foreach (DataCompanyMember man in m_arrCompanyMember)
            {                
                if (man.LevelID >= 4 || man.LevelID == 0)                
                {
                    if (bLevel4 == true)
                    {                            
                        SendingMember send = new SendingMember();
                        send.PhoneNumber = man.PhoneNumber;
                        send.Name = man.MemberName;
                        send.TeamName = "4직급이하";
                        arResult.Add(send);
                    }
                }
                else if (man.LevelID == 3)
                {
                    if (bLevel3 == true)
                    {                           
                        SendingMember send = new SendingMember();
                        send.PhoneNumber = man.PhoneNumber;
                        send.Name = man.MemberName;
                        send.TeamName = "3직급";
                        arResult.Add(send);                            
                    }
                }
                else if (man.LevelID == 2)
                {
                    if (bLevel2 == true)
                    {                           
                        SendingMember send = new SendingMember();
                        send.PhoneNumber = man.PhoneNumber;
                        send.Name = man.MemberName;
                        send.TeamName = "2직급";
                        arResult.Add(send);                            
                    }
                }
                else if (man.LevelID == 1)
                {
                    if (bLevel1 == true)
                    {                           
                        SendingMember send = new SendingMember();
                        send.PhoneNumber = man.PhoneNumber;
                        send.Name = man.MemberName;
                        send.TeamName = "1직급";
                        arResult.Add(send);                            
                    }
                }
            }
            return arResult;
        }

    }

    class SendingMember
    {
        private string m_szName;
        public string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }
        private string m_szPhoneNumber;
        public string PhoneNumber
        {
            get { return m_szPhoneNumber; }
            set { m_szPhoneNumber = value; }
        }
        private string m_szTeamNam;
        public string TeamName
        {
            get { return m_szTeamNam; }
            set { m_szTeamNam = value; }
        }
    }

    class DataJobLevel
    {
        private int m_nID;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        private string m_szLevelName;
        public string LevelName
        {
            get { return m_szLevelName; }
            set { m_szLevelName = value; }
        }
        private int m_nLevel;
        public int Level
        {
            get { return m_nLevel; }
            set { m_nLevel = value; }
        }
    }

    class DataJobPosition
    {
        private int m_nID;       
        private string m_szPositionName;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string PositionName
        {
            get { return m_szPositionName; }
            set { m_szPositionName = value; }
        }        
    }

    class DataTeam
    {
        private int m_nID;        
        private string m_szTeamName;       
        private int m_nParentTeamID;
        private bool m_bExternal;
        public bool External
        {
            get { return m_bExternal; }
            set { m_bExternal = value; }
        }
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        public string TeamName
        {
            get { return m_szTeamName; }
            set { m_szTeamName = value; }
        }
        public int ParentTeamID
        {
            get { return m_nParentTeamID; }
            set { m_nParentTeamID = value; }
        }
    }

    class DataExternalMember
    {
        private int m_nID;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        private string m_szName;
        public string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }
        private string m_szPhoneNumber;
        public string PhoneNumber
        {
            get { return m_szPhoneNumber; }
            set { m_szPhoneNumber = value; }
        }
        bool m_bTeamLeader;
        public bool TeamLeader
        {
            get { return m_bTeamLeader; }
            set { m_bTeamLeader = value; }
        }
        int m_nExternalTeamID;
        public int ExternalTeamID
        {
            get { return m_nExternalTeamID; }
            set { m_nExternalTeamID = value; }
        }
    }

    class DataCompanyMember
    {
        private int m_nID;
        private string m_strMemberName = "";
        private int m_nRegularTeamID;
        private int m_nTemporaryTeamID;
        private int m_nLevelID;
        private int m_nPositionID;
        private int m_nTemporaryPositionID;
        private string m_strMemberID = "";
        private string m_strPhoneNumber = "";
        private string m_strOfficePhoneNumber = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string MemberName
        {
            get { return m_strMemberName; }
            set { m_strMemberName = value; }
        }

        public int RegularTeamID
        {
            get { return m_nRegularTeamID; }
            set { m_nRegularTeamID = value; }
        }

        public int LevelID
        {
            get { return m_nLevelID; }
            set { m_nLevelID = value; }
        }

        public int PositionID
        {
            get { return m_nPositionID; }
            set { m_nPositionID = value; }
        }

        public string MemberID
        {
            get { return m_strMemberID; }
            set { m_strMemberID = value; }
        }

        public int SecondRegularTeamID
        {
            get { return m_nTemporaryTeamID; }
            set { m_nTemporaryTeamID = value; }
        }

        public int SecondPositionID
        {
            get { return m_nTemporaryPositionID; }
            set { m_nTemporaryPositionID = value; }
        }

        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }

        public string OfficePhoneNumber
        {
            get { return m_strOfficePhoneNumber; }
            set { m_strOfficePhoneNumber = value; }
        }
    }
}
