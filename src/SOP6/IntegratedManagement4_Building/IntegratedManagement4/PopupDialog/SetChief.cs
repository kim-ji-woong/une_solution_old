using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;

namespace IntegratedManagement4.PopupDialog
{
    public partial class SetChief : Form
    {
        private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        private WebDBManager m_dbMgr = null;
         
        private Dictionary<int, DataTeam> dicTeams = new Dictionary<int, DataTeam>();

        private DataTeam m_teamRegularRoot = null;
        private DataTeam m_teamNormalRoot = new DataTeam();
        private DataTeam m_teamEmergencyRoot = new DataTeam();
        private ArrayList m_teamExternalRoot = new ArrayList();
        private ArrayList m_teamUserDefinedRoot = new ArrayList();

        private Dictionary<int, DataCompanyMember> m_dicRegularMembers = new Dictionary<int, DataCompanyMember>();
        private Dictionary<DataTeam, ArrayList> m_dicRegularTeamMembers = new Dictionary<DataTeam, ArrayList>();
         
        private Dictionary<int, DataExternalMember> m_dicExternalMembers = new Dictionary<int, DataExternalMember>();
        private Dictionary<DataTeam, ArrayList> m_dicExternalTeamMembers = new Dictionary<DataTeam, ArrayList>();

        private Dictionary<int, DataCompanyMember> m_dicNormalMembers = new Dictionary<int, DataCompanyMember>();
        private Dictionary<DataTeam, ArrayList> m_dicNormalTeamMembers = new Dictionary<DataTeam, ArrayList>();

        private Chief m_chief = new Chief();
        public Chief Chief
        {
            get { return m_chief; }
        }
       
        private string m_strNickName = "";
        public string NickName
        {
            get { return m_strNickName; }
        }

        private bool m_bLeftMouseDown = false;
        private Point m_ptMove;
        private bool isSelectGridView = false;

        private Dictionary<int, DataTeam> m_dicNormalTeam = new Dictionary<int, DataTeam>();
        private ArrayList m_arrNormalTeam = new ArrayList();
        public ArrayList TemporaryNormalTeam
        {
            get { return m_arrNormalTeam; }
            set { m_arrNormalTeam = value; }
        }

        private Dictionary<int, DataTeam> m_dicEmergencyTeam = new Dictionary<int, DataTeam>();
        private ArrayList m_arrEmergencyTeam = new ArrayList();
        public ArrayList TemporaryEmergencyTeam
        {
            get { return m_arrEmergencyTeam; }
            set { m_arrEmergencyTeam = value; }
        }

        SOPTeamType m_SOPTeamType = SOPTeamType.None;
        TreeNode m_LastSelectedNode = null;
        DataGridViewRow m_LastSelectedRow = null;

        private double m_WindowRateWidth = 1d;
        public double WindowRateWidth
        {
            get { return m_WindowRateWidth; }
            set { m_WindowRateWidth = value; }
        }

        private double m_WindowRateHeight = 1d;
        public double WindowRateHeight
        {
            get { return m_WindowRateHeight; }
            set { m_WindowRateHeight = value; }
        }

        public SetChief(WebDBManager dbMgr, Chief chief)
        {
            InitializeComponent();

            this.m_dbMgr = dbMgr;
 
            DisplayRegularTeam();
            DisplayCompanyMember();

            DisplayExternalTeam();
            LoadExternalMember();

            LoadUserDefinedTeam();

            m_teamNormalRoot = new DataTeam();        
            m_teamEmergencyRoot = new DataTeam();

            LoadTeamporaryTeam("TemporaryNormalTeam",ref m_teamNormalRoot, m_dicNormalTeam, SOPTeamType.Normal);
            //LoadNormalMember();
            LoadTeamporaryTeam("TemporaryEmergencyTeam",ref m_teamEmergencyRoot, m_dicEmergencyTeam, SOPTeamType.Holiday);

            txt_DisplayText.Select();
            treeViewTeam.AfterSelect += treeViewTeam_AfterSelect;
            dataGridView1.ReadOnly = true;

            if (chief != null)
            {
                m_chief = chief;
                if (m_chief.SOPTYPE == SOPTeamType.Regular || m_chief.SOPTYPE == SOPTeamType.RegularMember)                
                    InitTree(m_teamRegularRoot);                
                else if (m_chief.SOPTYPE == SOPTeamType.External || m_chief.SOPTYPE == SOPTeamType.ExternalMember)                
                    InitTree(m_teamExternalRoot);
                else if (m_chief.SOPTYPE == PopupDialog.SOPTeamType.UserDefined)
                    InitTree(m_teamUserDefinedRoot);
                else if (m_chief.SOPTYPE == PopupDialog.SOPTeamType.Normal)
                    InitTree(m_teamNormalRoot);
                else if (m_chief.SOPTYPE == PopupDialog.SOPTeamType.Holiday)
                    InitTree(m_teamEmergencyRoot);
                else
                    InitTree(m_teamRegularRoot);

                //if (chief.Node != null && chief.DataTeam != null)
                //{
                //    treeViewTeam.Focus();

                //    TreeNode trNode = FindNode(chief.Node.Text, null, chief.Node.Level);
                //    treeViewTeam.SelectedNode = trNode;
                //}

                txt_DisplayText.Text = chief.DisplayText;
                OfficePhoneNumber oo = new OfficePhoneNumber(chief.CallerPhoneNumber, false);
                txt_CallerPhoneNumber.Text = oo.Number;      
            }
            else
            {
                InitTree(m_teamRegularRoot);
            }

            UpdateRadioButtons();
        }

        public void UpdateControl()
        {            
            FormMain.Instance.UpdateWindowRate(this, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(panel2, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(label4, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(ribbonButton1, WindowRateWidth, WindowRateHeight);

            FormMain.Instance.UpdateWindowRate(groupBox1, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(label1, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(label2, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(txt_DisplayText, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(txt_CallerPhoneNumber, WindowRateWidth, WindowRateHeight);

            FormMain.Instance.UpdateWindowRate(picDay, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblDay, WindowRateWidth, WindowRateHeight);

            FormMain.Instance.UpdateWindowRate(picNight, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblNight, WindowRateWidth, WindowRateHeight);

            FormMain.Instance.UpdateWindowRate(picRegular, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblRegular, WindowRateWidth, WindowRateHeight);

            FormMain.Instance.UpdateWindowRate(picExternal, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblExternal, WindowRateWidth, WindowRateHeight);

            FormMain.Instance.UpdateWindowRate(picUserDefined, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblUserDefined, WindowRateWidth, WindowRateHeight);

            FormMain.Instance.UpdateWindowRate(picNormal, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblNormal, WindowRateWidth, WindowRateHeight);

            FormMain.Instance.UpdateWindowRate(picHoliday, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblHoliday, WindowRateWidth, WindowRateHeight);

            FormMain.Instance.UpdateWindowRate(panel1, WindowRateWidth, WindowRateHeight);

            FormMain.Instance.UpdateWindowRate(treeViewTeam, WindowRateWidth, WindowRateHeight);

            FormMain.Instance.UpdateWindowRate(dataGridView1, WindowRateWidth, WindowRateHeight);

            FormMain.Instance.UpdateWindowRate(btn_ok, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(btn_cancel, WindowRateWidth, WindowRateHeight);
        }

        public TreeNode FindNode(string strValue, TreeNodeCollection parentNodes = null, int nSearchLevel = 0)
        {
            TreeNodeCollection nodes = parentNodes == null ? treeViewTeam.Nodes : parentNodes;

            foreach (TreeNode node in nodes)
            {
                if (node.Level >= nSearchLevel)
                {
                    if (strValue == node.Text)
                        return node;
                }

                TreeNode result = FindNode(strValue, node.Nodes);
                if (result != null)
                    return result;
            }

            return null;
        }
                
        private void UpdateRadioButtons()
        {
            if (m_chief == null) return;

            if (m_chief.DayLight_Day == true)
            {
                picDay.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__COMMON_ckb_enable;                
            }
            else
            {
                picDay.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__COMMON_ckb_disable;
            }

            if (m_chief.DayLight_Night == true)
            {
                picNight.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__COMMON_ckb_enable;  
            }
            else
            {
                picNight.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__COMMON_ckb_disable;  
            }

            if(m_chief.SOPTYPE == PopupDialog.SOPTeamType.Regular || m_chief.SOPTYPE == SOPTeamType.RegularMember)
            {
                treeViewTeam.Size = new Size((int)(189 * WindowRateWidth), (int)(321*WindowRateHeight));
                picRegular.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Enable2;
                picExternal.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
                picUserDefined.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
                picNormal.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
                picHoliday.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
            }
            else if(m_chief.SOPTYPE == PopupDialog.SOPTeamType.External || m_chief.SOPTYPE == SOPTeamType.ExternalMember)
            {
                treeViewTeam.Size = new Size((int)(189*WindowRateWidth), (int)(321*WindowRateHeight));
                picRegular.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
                picExternal.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Enable2;
                picUserDefined.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
                picNormal.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
                picHoliday.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
            }
            else if(m_chief.SOPTYPE == PopupDialog.SOPTeamType.UserDefined)
            {
                treeViewTeam.Size = new Size((int)(549*WindowRateWidth), (int)(321*WindowRateHeight));
                picRegular.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
                picExternal.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
                picUserDefined.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Enable2;
                picNormal.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
                picHoliday.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
            }
            else if(m_chief.SOPTYPE == PopupDialog.SOPTeamType.Normal)
            {
                treeViewTeam.Size = new Size((int)(549 * WindowRateWidth), (int)(321 * WindowRateHeight));
                picRegular.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
                picExternal.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
                picUserDefined.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
                picNormal.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Enable2;
                picHoliday.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
            }
            else if(m_chief.SOPTYPE == PopupDialog.SOPTeamType.Holiday)
            {
                treeViewTeam.Size = new Size((int)(549 * WindowRateWidth), (int)(321 * WindowRateHeight));
                picRegular.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
                picExternal.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
                picUserDefined.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
                picNormal.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Disable2;
                picHoliday.BackgroundImage = global::IntegratedManagement4.Properties.Resources.__SOPEDIT_Enable2;
            }
        }

        void treeViewTeam_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = treeViewTeam.SelectedNode;
            if (node == null)
                return;

            DataTeam team = node.Tag as DataTeam;
            if (team.ID < 0)
                return;

            m_LastSelectedNode = node;
            m_SOPTeamType = team.SOPTYPE;

            isSelectGridView = true;
            dataGridView1.Rows.Clear();            

            if(team.SOPTYPE == SOPTeamType.Regular)
            {
                txt_DisplayText.Text = team.TeamName;
                txt_CallerPhoneNumber.Text = team.PhoneNumber;

                if (m_dicRegularTeamMembers.ContainsKey(team))
                {
                    ArrayList arrCompanyMembers = m_dicRegularTeamMembers[team];

                    foreach (DataCompanyMember member in arrCompanyMembers)
                    {                        
                        dataGridView1.Rows.Add(member.ID, member.MemberName, (member.PositionName == "null") ? "" : member.PositionName, new PhoneNumber(member.PhoneNumber));
                        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Tag = member.SOPTYPE;
                    }
                }
            }
            else if( team.SOPTYPE == SOPTeamType.External)
            {
                txt_DisplayText.Text = team.TeamName;
                txt_CallerPhoneNumber.Text = team.PhoneNumber;

                if (m_dicExternalTeamMembers.ContainsKey(team))
                {
                    ArrayList arrCompanyMembers = m_dicExternalTeamMembers[team];

                    foreach (DataExternalMember member in arrCompanyMembers)
                    {                        
                        dataGridView1.Rows.Add(member.ID, member.Name, (member.PositionName == "null") ? "" : member.PositionName, new PhoneNumber(member.PhoneNumber));
                        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Tag = member.SOPTYPE;
                    }
                }
            }
            else if (team.SOPTYPE == SOPTeamType.UserDefined)
            {                
                txt_DisplayText.Text = team.TeamName;
                txt_CallerPhoneNumber.Text = team.PhoneNumber;
            }
            else if (team.SOPTYPE == SOPTeamType.Normal)
            {
                txt_DisplayText.Text = team.TeamName;
                txt_CallerPhoneNumber.Text = team.PhoneNumber;
            }
            else if (team.SOPTYPE == SOPTeamType.Holiday)
            {
                txt_DisplayText.Text = team.TeamName;
                txt_CallerPhoneNumber.Text = team.PhoneNumber;
            }

            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                item.Selected = false;

                txt_DisplayText.Text = team.TeamName;
                txt_CallerPhoneNumber.Text = team.PhoneNumber;
            }
        }  

        private void InitTree(object TeamInfo)
        {
            treeViewTeam.Nodes.Clear();

            if(TeamInfo is DataTeam)
            {
                //MakeTeam(treeViewTeam.Nodes, m_teamRegularRoot);
                MakeTeam(treeViewTeam.Nodes, TeamInfo as DataTeam);
            }
            else if (TeamInfo is ArrayList)
            {
                //MakeExternalTeams(treeViewTeam.Nodes, m_teamExternalRoot);
                MakeExternalTeams(treeViewTeam.Nodes, TeamInfo as ArrayList);
            }
                
            if (treeViewTeam.Nodes.Count > 0)
            {
                treeViewTeam.ExpandAll();
                //treeViewTeam.SelectedNode = treeViewTeam.Nodes[0];
            }

            dataGridView1.Rows.Clear();     
            //OnAfterTreeSelect();
        }

        private void DisplayRegularTeam()
        {
            dicTeams.Clear();
            //string szSQL = "SELECT R.ID, R.TeamName, R.ParentTeamID FROM RegularTeam as R";

            string strSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", FormMain.Instance.SiteID);
            ArrayList arrResult1 = m_dbMgr.GetResultData(strSQL);
            if (arrResult1 == null || arrResult1.Count == 0)
                return;

            int nTeamID = WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
            if (nTeamID == -1)
                return;

            ArrayList arrResult = ExecuteTeamList(nTeamID);
            //strSQL = string.Format("sp_TeamList2 {0}", nTeamID);
            //ArrayList arrResult = dbMgr.GetStoredProcedureData(strSQL, 0);
            if (arrResult == null || arrResult.Count == 0)
                return;

            // 자신의 Team, 부모 팀의 ID
            Dictionary<DataTeam, int> dicParentID = new Dictionary<DataTeam, int>();

            int nCount = arrResult.Count;

            for (int i = 0; i < nCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string szTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                DataTeam data = new DataTeam();
                data.ID = nID;
                data.TeamName = szTeamName;
                data.External = false;
                data.SOPTYPE = SOPTeamType.Regular;

                dicTeams[nID] = data;
                dicParentID[data] = nParentTeamID;
            }

            m_teamRegularRoot = new DataTeam();

            foreach (KeyValuePair<DataTeam, int> pair in dicParentID)
            {
                if (pair.Value < 0)
                {
                    m_teamRegularRoot = pair.Key;
                    m_teamRegularRoot.IsCompany = true;
                    continue;
                }

                if (!dicTeams.ContainsKey(pair.Value))
                    continue;

                DataTeam teamParent = dicTeams[pair.Value];
                pair.Key.ParentTeam = teamParent;
            } 
        }

        private void DisplayExternalTeam()
        {
            dicTeams.Clear();

            string szText2 = "SELECT et.ID, et.TeamName, et.ParentTeamID, et.PhoneNumber FROM ExternalTeam as et WHERE et.SiteID = {0} ";            

            string szSQL = string.Format(szText2, FormMain.Instance.SiteID);

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                return;

            // 자신의 Team, 부모 팀의 ID
            Dictionary<DataTeam, int> dicParentID = new Dictionary<DataTeam, int>();

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string szTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                String nPhoneNumber = WebDBManager.GetStringField(arrResult[i + 3].ToString(), "");
                //int nCompanyID = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);

                DataTeam data = new DataTeam();
                data.ID = nID;
                data.TeamName = szTeamName;
                data.External = true;
                data.SOPTYPE = SOPTeamType.External;
                data.PhoneNumber = nPhoneNumber;

                if (nParentTeamID == -1)
                {
                    //data.ParentTeam = teamCompany;
                    data.IsCompany = true;
                    data.CompanyName = szTeamName;

                    if (!m_teamExternalRoot.Contains(data))
                    {
                        m_teamExternalRoot.Add(data);
                    }
                }
                else
                {
                    dicParentID[data] = nParentTeamID;
                }

                dicTeams[nID] = data;
            }

            foreach (KeyValuePair<DataTeam, int> pair in dicParentID)
            {
                if (pair.Key.ParentTeam != null)
                    continue;

                if (!dicTeams.ContainsKey(pair.Value))
                    continue;

                DataTeam teamParent = dicTeams[pair.Value];
                pair.Key.ParentTeam = teamParent;
                pair.Key.CompanyName = teamParent.CompanyName;
            } 
        }

        private void DisplayCompanyMember()
        {
            m_dicRegularMembers.Clear();

            string strSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", FormMain.Instance.SiteID);
            ArrayList arrResult1 = m_dbMgr.GetResultData(strSQL);
            if (arrResult1 == null || arrResult1.Count == 0)
                return;

            int nTeamID = WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
            if (nTeamID == -1)
                return;

            ArrayList arrResult2 = ExecuteTeamList(nTeamID); 
            if (arrResult2 == null || arrResult2.Count == 0)
                return;

            string szTeamList = "";
            for (int i = 0; i < arrResult2.Count - 2; i += 3)
            {
                string szTeamID = WebDBManager.GetStringField(arrResult2[i].ToString(), "");
                if (szTeamList != "")
                {
                    szTeamList += ",";
                }
                szTeamList += szTeamID;
            }

            if (szTeamList == "")
            {
                return;
            }
            string szText = "select rm.RegularTeamID, rm.CompanyMemberID, (select PositionName from JobPosition as jp where jp.ID=rm.PositionID) as PositionName, MemberName, LevelID, MemberID, OfficePhoneNumber, PhoneNumber " +
                            " FROM CompanyMember as cm, RegularMemberList as rm WHERE cm.ID = rm.CompanyMemberID and rm.RegularTeamID in ({0})";

            strSQL = string.Format(szText, szTeamList);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return;

            int nCount = arrResult.Count;
            if (nCount == 0) return;

            DataCompanyMember member;

            for (int i = 0; i < nCount - 7; i += 8)
            {
                int nRegularTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                int nID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
                string strPositionName = WebDBManager.GetStringField(arrResult[i + 2].ToString(), "");
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 3], "");
                int nLevelID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
                string strMemberID = WebDBManager.GetStringField(arrResult[i + 5], ""); 
                string strOfficePhoneNumber = WebDBManager.GetStringField(arrResult[i + 6], "");
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 7], "");

                if (string.Compare(strPhoneNumber, "null", true) == 0 || strPhoneNumber == "")
                    strPhoneNumber = "";
                else
                    strPhoneNumber = AES256Cipher.AES_decrypt(strPhoneNumber, key);

                strPhoneNumber = ValidPhoneNumber(strPhoneNumber);

                if (string.Compare(strOfficePhoneNumber, "null", true) == 0)
                    strOfficePhoneNumber = "";

                if (!dicTeams.ContainsKey(nRegularTeamID))
                    continue;

                DataTeam team = dicTeams[nRegularTeamID];

                if (!m_dicRegularMembers.TryGetValue(nID, out member))
                {
                    member = new DataCompanyMember();

                    member.ID = nID;
                    member.MemberName = strMemberName;
                    member.LevelID = nLevelID;
                    member.MemberID = strMemberID;
                    member.OfficePhoneNumber = strOfficePhoneNumber;
                    member.PhoneNumber = strPhoneNumber;
                    member.PositionName = strPositionName;
                    member.SOPTYPE = SOPTeamType.RegularMember;

                    m_dicRegularMembers[nID] = member;
                } 

                ArrayList arrMembers = null;

                if (m_dicRegularTeamMembers.ContainsKey(team))
                {
                    arrMembers = m_dicRegularTeamMembers[team];                    
                }                    
                else
                {
                    arrMembers = new ArrayList();
                    m_dicRegularTeamMembers[team] = arrMembers;
                }

                arrMembers.Add(member); 
            }

            foreach (KeyValuePair<DataTeam, ArrayList> pair in m_dicRegularTeamMembers)
            {
                pair.Value.Sort();
            }             
        }

        private bool LoadUserDefinedTeam()
        {
            string strSQL = "select ID, TeamName, PhoneNumber, FaxNumber from UserDefinedTeam where SiteID = " + FormMain.Instance.SiteID;
            ArrayList arrResults = m_dbMgr.GetResultData(strSQL);

            if (arrResults == null)
                return false;

            int nResultCount = arrResults.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nID = WebDBManager.GetIntField(arrResults[i].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResults[i + 1], "");
                string strPhoneNumber = WebDBManager.GetStringField(arrResults[i + 2], "");
                string strFaxNumber = WebDBManager.GetStringField(arrResults[i + 3], "");

                if (strFaxNumber.ToUpper() == "NULL")
                    strFaxNumber = "";

                DataTeam data = new DataTeam();
                data.ID = nID;
                data.TeamName = strTeamName;
                data.PhoneNumber = strPhoneNumber;
                data.SOPTYPE = SOPTeamType.UserDefined;

                if (m_teamUserDefinedRoot.Contains(data) == false)
                    m_teamUserDefinedRoot.Add(data);
            }

            return true;
        }

        public bool LoadExternalMember()
        {
            m_dicExternalMembers.Clear(); 

            StringBuilder sb1 = new StringBuilder(); 
            sb1.Append("Select eml.ExternalCompanyTeamID, eml.ExternalCompanyMemberID, ecm.Name, ecm.PhoneNumber ");
            sb1.Append("     , (select PositionName from externaljobposition as ej where eml.JobPositionID = ej.ID) as PositionName ");
            sb1.Append("from ExternalCompanyMember as ecm, ExternalMemberList as eml, ExternalTeam as et ");
            sb1.AppendFormat("where eml.ExternalCompanyMemberID = ecm.ID and et.ID = eml.ExternalCompanyTeamID and et.SiteID = {0}", FormMain.Instance.SiteID);

            string szSQL = sb1.ToString();

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                return false;

            int nCount = arrResult.Count;
            if (nCount == 0)
                return true;

            DataExternalMember member;

            for (int i = 0; i < nCount - 3; i += 5)
            {
                int nTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                int nID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0); 
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 2], "");
                string szPhoneNumber = WebDBManager.GetStringField(arrResult[i + 3], "");
                string szPotionName = WebDBManager.GetStringField(arrResult[i + 4], "");

                if (!dicTeams.ContainsKey(nTeamID))
                    return false;

                DataTeam team = dicTeams[nTeamID];

                if (string.Compare(szPhoneNumber, "null", true) == 0 || szPhoneNumber == "")
                    szPhoneNumber = "";
                else
                    szPhoneNumber = AES256Cipher.AES_decrypt(szPhoneNumber, key);

                szPhoneNumber = ValidPhoneNumber(szPhoneNumber);

                if (!m_dicExternalMembers.TryGetValue(nID, out member))
                {
                    member = new DataExternalMember();

                    member.ID = nID;
                    member.Name = strMemberName;
                    member.PhoneNumber = szPhoneNumber;
                    member.Team = team;
                    member.PositionName = szPotionName;
                    //member.TeamLeaders[team] = nLeader;
                    member.SOPTYPE = SOPTeamType.ExternalMember;

                    m_dicExternalMembers[nID] = member;
                } 

                ArrayList arrMembers = null;

                if (m_dicExternalTeamMembers.ContainsKey(team))
                    arrMembers = m_dicExternalTeamMembers[team];
                else
                {
                    arrMembers = new ArrayList();
                    m_dicExternalTeamMembers[team] = arrMembers;
                }

                //m_dicExternalMembers[nID] = data;
                arrMembers.Add(member);
            }

            return false;
        }

        private void LoadTeamporaryTeam(String pstrTableName,ref DataTeam p_teamRoot, Dictionary<int, DataTeam> p_dicTeam, SOPTeamType pType)
        {
            string strSQL = string.Format("Select ID, TeamName, ParentTeamID from " + pstrTableName + " where SiteID = {0} order by ParentTeamID, ID", FormMain.Instance.SiteID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null || arrResult.Count == 0)
                return;

            Dictionary<DataTeam, int> dicParentID = new Dictionary<DataTeam, int>();

            int nCount = arrResult.Count;

            for (int i = 0; i < nCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string szTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                DataTeam data = new DataTeam();
                data.ID = nID;
                data.TeamName = szTeamName;
                data.External = false;
                data.SOPTYPE = pType;

                p_dicTeam[nID] = data;
                dicParentID[data] = nParentTeamID;
            }

            foreach (KeyValuePair<DataTeam, int> pair in dicParentID)
            {
                if (pair.Value < 0)
                {
                    p_teamRoot = pair.Key;
                    p_teamRoot.IsCompany = true;
                    continue;
                }

                if (p_dicTeam.ContainsKey(pair.Value) == false)
                    continue;

                DataTeam teamParent = p_dicTeam[pair.Value];
                pair.Key.ParentTeam = teamParent;
            } 
        }

        public ArrayList ExecuteTeamList(int nRootTeamID, string strTableName = "RegularTeam")
        {
            string strSQL = "Select ID, TeamName, ParentTeamID from " + strTableName + " order by ParentTeamID, ID";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            if (nRootTeamID == 0)
                return arrResult;

            int nResultCount = arrResult.Count;

            ArrayList arrNewResult = new ArrayList();
            Dictionary<int, int> dicParentID = new Dictionary<int, int>();

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                VariousData<int> parentID = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                if (dicParentID.Count == 0)
                {
                    if (nID == nRootTeamID)
                    {
                        dicParentID[nID] = nID;

                        arrNewResult.Add(arrResult[i]);
                        arrNewResult.Add(arrResult[i + 1]);
                        arrNewResult.Add(arrResult[i + 2]);
                    }
                }
                else
                {
                    if (parentID == null)
                        continue;

                    if (dicParentID.ContainsKey(parentID.Data))
                    {
                        dicParentID[nID] = nID;

                        arrNewResult.Add(arrResult[i]);
                        arrNewResult.Add(arrResult[i + 1]);
                        arrNewResult.Add(arrResult[i + 2]); 
                    }
                }
            }

            return arrNewResult;
        }

        private void MakeTeam(TreeNodeCollection nodes, DataTeam team)
        {
            TreeNode node = new TreeNode();
            node.Text = team.TeamName;
            node.Tag = team;

            nodes.Add(node);

            foreach (DataTeam teamChild in team.ChildTeams)
            {
                MakeTeam(node.Nodes, teamChild);
            }
        }

        private void MakeExternalTeams(TreeNodeCollection nodes, ArrayList arrTeams)
        {
            foreach (DataTeam team in arrTeams)
            {
                MakeTeam(nodes, team);
            }
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

        private DataTeam GetTeam()
        {
            TreeNode node = treeViewTeam.SelectedNode;
            if (node == null && m_LastSelectedNode == null)            
                return null;

            if (node == null)
                node = m_LastSelectedNode;

            DataTeam team = node.Tag as DataTeam;
            return team;
        }

        private DataCompanyMember GetCompanyMember(DataTeam team)
        {
            DataGridViewRow row = dataGridView1.CurrentRow;
            if (row == null && m_LastSelectedRow == null)
                return null;
          
            if (row == null)
                row = m_LastSelectedRow;

            int nID = Convert.ToInt32(row.Cells[0].Value);
            ArrayList arrCompanyMembers = m_dicRegularTeamMembers[team];

            foreach (DataCompanyMember member in arrCompanyMembers)
            {
                if (member.ID == nID)
                    return member;
            }

            return null;
        }

        private DataExternalMember GetExternalMember(DataTeam team)
        {
            DataGridViewRow row = dataGridView1.CurrentRow;
            if (row == null && m_LastSelectedRow == null)
                return null;

            if (row == null)
                row = m_LastSelectedRow;

            int nID = Convert.ToInt32(row.Cells[0].Value);
            ArrayList arrCompanyMembers = m_dicExternalTeamMembers[team];

            foreach (DataExternalMember member in arrCompanyMembers)
            {                
                if (member.ID == nID)
                    return member;
            }

            return null;
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            try
            {
                //if (this.textBoxNickName.Text.Trim() == "")
                //{
                //    throw new ApplicationException("별명을 입력하세요.");
                //}

                if (txt_DisplayText.Text.Length == 0)
                    throw new ApplicationException("책임자 명칭을 입력하세요.");

                if (txt_CallerPhoneNumber.Text.Length == 0)
                    throw new ApplicationException("문자를 발송할 전화번호를 입력하세요.");

                OfficePhoneNumber offic = new OfficePhoneNumber();
                offic.Number = txt_CallerPhoneNumber.Text;
                if (!offic.IsValid)
                {
                    throw new ApplicationException("전화번호 형식이 맞지 않습니다.");
                }
               
                if (m_chief.DayLight_Day == false && m_chief.DayLight_Night == false)
                {
                    throw new ApplicationException("주간 또는 야갼 담당은 반드시 선택되어야 합니다.");
                }

                m_chief.DisplayText = txt_DisplayText.Text;
                txt_CallerPhoneNumber.Text = offic.Number;

                m_chief.CallerPhoneNumber = txt_CallerPhoneNumber.Text;

                if (isSelectGridView == true)
                {
                    m_chief.DisplayText = txt_DisplayText.Text;
                    m_chief.CallerPhoneNumber = txt_CallerPhoneNumber.Text;

                    DataTeam team = GetTeam();
                    if (team == null) return;

                    m_chief.Node = treeViewTeam.SelectedNode;
                    m_chief.DataTeam = team;                    
                    m_chief.SOPTYPE = m_SOPTeamType;

                    if (m_SOPTeamType == SOPTeamType.ExternalMember)
                    {
                        DataExternalMember member = GetExternalMember(team);
                        m_chief.ID = member.ID;
                    }
                    else if (m_SOPTeamType == SOPTeamType.RegularMember)
                    {
                        DataCompanyMember member = GetCompanyMember(team);
                        m_chief.ID = member.ID;
                    }
                    else
                    {
                        m_chief.ID = team.ID;
                    }                    
                }
                           
                this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                UnE.Utility.UMessageBoxRibbon.Show(ex.Message, "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
        }

        private void txt_CallerPhoneNumber_KeyPress(object sender, KeyPressEventArgs e)
        { 
            if(!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back) || e.KeyChar == Convert.ToChar("-"))) //숫자와 백스페이스를 제외한 나머지를 바로 처리
            {
                e.Handled = true;
            } 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            treeViewTeam.SelectedNode = m_chief.Node;
        }

        private void SetChief_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void SetChief_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point pt = PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {
                        Point ptCur = this.Location;
                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void SetChief_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            isSelectGridView = true;

            m_LastSelectedRow = this.dataGridView1.Rows[e.RowIndex];
            txt_DisplayText.Text = this.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            txt_CallerPhoneNumber.Text = this.dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            
            m_SOPTeamType = (SOPTeamType)this.dataGridView1.Rows[e.RowIndex].Tag;
        }

        private void Day_Click(object sender, EventArgs e)
        {
            m_chief.DayLight_Day = !m_chief.DayLight_Day;
            UpdateRadioButtons();
        }

        private void Night_Click(object sender, EventArgs e)
        {
            m_chief.DayLight_Night = !m_chief.DayLight_Night;
            UpdateRadioButtons();
        }

        private void Team_Regular_Click(object sender, EventArgs e)
        {
            InitTree(m_teamRegularRoot);
            m_chief.SOPTYPE = PopupDialog.SOPTeamType.Regular;
            UpdateRadioButtons();
        }

        private void Team_External_Click(object sender, EventArgs e)
        {
            InitTree(m_teamExternalRoot);
            m_chief.SOPTYPE = PopupDialog.SOPTeamType.External;
            UpdateRadioButtons();
        }

        private void Team_UserDefine_Click(object sender, EventArgs e)
        {
            InitTree(m_teamUserDefinedRoot);
            m_chief.SOPTYPE = PopupDialog.SOPTeamType.UserDefined;
            UpdateRadioButtons();
        }

        private void Team_Normal_Click(object sender, EventArgs e)
        {
            InitTree(m_teamNormalRoot);
            m_chief.SOPTYPE = PopupDialog.SOPTeamType.Normal;
            UpdateRadioButtons();
        }

        private void Team_Emergency_Click(object sender, EventArgs e)
        {
            InitTree(m_teamEmergencyRoot);
            m_chief.SOPTYPE = PopupDialog.SOPTeamType.Holiday;
            UpdateRadioButtons();
        }

        private void dataGridView1_CellBorderStyleChanged(object sender, EventArgs e)
        {

        }

        private void treeViewTeam_Click(object sender, EventArgs e)
        {
            treeViewTeam_AfterSelect(treeViewTeam, null);
        }
    }

    public enum SOPTeamType { None = -1, Normal = 0, Holiday, External, UserDefined, Regular, ExternalMember = 7, RegularMember = 8,ControlRoom = 10 };

    public class DataTeam
    {
        private int m_nID = -1;
        private string m_szTeamName = "";
        private DataTeam m_teamParent = null;
        private bool m_bExternal = false;
        private ArrayList m_arrChildTeams = new ArrayList();
        private string m_strCompanyName = "";
        private bool m_isCompany = false;


        public bool External
        {
            get { return m_bExternal; }
            set { m_bExternal = value; }
        }

        private SOPTeamType m_SOPType = SOPTeamType.Regular;
        public SOPTeamType SOPTYPE
        {
            get
            {
                return m_SOPType;
            }
            set
            {
                m_SOPType = value;
            }
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

        private string m_szPhoneNumber = "";
        public string PhoneNumber
        {
            get { return m_szPhoneNumber; }
            set { m_szPhoneNumber = value; }
        }


        public DataTeam ParentTeam
        {
            get { return m_teamParent; }
            set
            {
                if (m_teamParent != null)
                    m_teamParent.RemoveChild(this);

                m_teamParent = value;

                if (m_teamParent != null)
                    m_teamParent.AddChild(this);
            }
        }

        public ArrayList ChildTeams
        {
            get { return m_arrChildTeams; }
        }

        public string CompanyName
        {
            get { return m_strCompanyName; }
            set { m_strCompanyName = value; }
        }

        // Team이 아닌 Company인가?
        public bool IsCompany
        {
            get { return m_isCompany; }
            set { m_isCompany = value; }
        }

        protected void RemoveChild(DataTeam team)
        {
            if (team != null)
                m_arrChildTeams.Remove(team);
        }

        protected void AddChild(DataTeam team)
        {
            if (!m_arrChildTeams.Contains(team))
                m_arrChildTeams.Add(team);
        }

        public override string ToString()
        {
            return m_szTeamName;
        }
    }

    public class DataCompanyMember : IComparable
    {
        private int m_nID = -1;
        private string m_strMemberName = "";
        //private DataTeam m_team = null;
        private int m_nLevelID = -1;
        //private int m_nPositionID = -1;
        private string m_strMemberID = "";
        private string m_strPositionName = "";
        private string m_strPhoneNumber = "";
        private string m_strOfficePhoneNumber = "";
        private Dictionary<DataTeam, int> m_dicTeamPositions = new Dictionary<DataTeam, int>();

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

        /*public DataTeam Team
        {
            get { return m_team; }
            set { m_team = value; }
        }*/

        public int LevelID
        {
            get { return m_nLevelID; }
            set { m_nLevelID = value; }
        }

        /*public int PositionID
        {
            get { return m_nPositionID; }
            set { m_nPositionID = value; }
        }*/

        public string MemberID
        {
            get { return m_strMemberID; }
            set { m_strMemberID = value; }
        }

        public string PositionName
        {
            get { return m_strPositionName; }
            set { m_strPositionName = value; }
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

        public Dictionary<DataTeam, int> TeamPositions
        {
            get { return m_dicTeamPositions; }
        }

        /*public bool IsTeamLeader
        {
            get { return m_nPositionID == 2; }
        }*/

        public int GetFirstTeamPosition()
        {
            foreach (KeyValuePair<DataTeam, int> pair in m_dicTeamPositions)
            {
                return pair.Value;
            }

            return -1;
        }

        public DataTeam GetFirstTeam()
        {
            foreach (KeyValuePair<DataTeam, int> pair in m_dicTeamPositions)
            {
                return pair.Key;
            }

            return null;
        }

        private SOPTeamType m_SOPType = SOPTeamType.Regular;
        public SOPTeamType SOPTYPE
        {
            get
            {
                return m_SOPType;
            }
            set
            {
                m_SOPType = value;
            }
        }

        public bool IsTeamLeader(DataTeam team)
        {
            int nPosition;

            if (m_dicTeamPositions.TryGetValue(team, out nPosition))
            {
                return nPosition == 2;
            }

            return false;
        }

        public int CompareTo(object obj)
        {
            if (obj.GetType() != typeof(DataCompanyMember))
                return -1;
            DataCompanyMember member = (DataCompanyMember)obj;
            int nPosition = this.GetFirstTeamPosition();

            if (nPosition != member.GetFirstTeamPosition())
                return nPosition == 2 ? -1 : 1;

            if (this.m_nLevelID > member.m_nLevelID)
                return 1;
            else if (this.m_nLevelID < member.m_nLevelID)
                return -1;

            return this.m_strMemberID.CompareTo(member.m_strMemberID);
        }

        public override string ToString()
        {
            return m_strMemberName;
        }
    }

    public class DataExternalMember
    {
        private int m_nID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private string m_szName = "";

        public string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }

        private string m_szPositionName = "";

        public string PositionName
        {
            get { return m_szPositionName; }
            set { m_szPositionName = value; }
        }

        private string m_szPhoneNumber = "";

        public string PhoneNumber
        {
            get { return m_szPhoneNumber; }
            set { m_szPhoneNumber = value; }
        }

        /*private bool m_bTeamLeader = false;

        public bool TeamLeader
        {
            get { return m_bTeamLeader; }
            set { m_bTeamLeader = value; }
        }*/

        private DataTeam m_team = null;

        public DataTeam Team
        {
            get { return m_team; }
            set { m_team = value; }
        }

        private SOPTeamType m_SOPType = SOPTeamType.Regular;
        public SOPTeamType SOPTYPE
        {
            get
            {
                return m_SOPType;
            }
            set
            {
                m_SOPType = value;
            }
        }

        // 한 개인이 여러팀에 속해있을때 각 팀에 따라 팀장일수도 팀원일수도 있다.
        /*private Dictionary<DataTeam, bool> m_dicTeamLeaders = new Dictionary<DataTeam, bool>();

        public Dictionary<DataTeam, bool> TeamLeaders
        {
            get { return m_dicTeamLeaders; }
        }

        public DataTeam GetFirstTeam()
        {
            foreach (KeyValuePair<DataTeam, bool> pair in m_dicTeamLeaders)
            {
                return pair.Key;
            }
            
            return null;
        }*/

        public override string ToString()
        {
            return m_szName;
        }
    }

    public class Chief
    {
        private SOPTeamType m_SOPType = SOPTeamType.None;
        public SOPTeamType SOPTYPE
        {
            get
            {
                return m_SOPType;
            }
            set
            {
                m_SOPType = value;
            }
        }

        private string m_strDisplayText = "";
        public string DisplayText
        {
            get { return m_strDisplayText; }
            set { m_strDisplayText = value; }
        }

        private string m_strCallerPhoneNumber = "";
        public string CallerPhoneNumber
        {
            get { return m_strCallerPhoneNumber; }
            set { m_strCallerPhoneNumber = value; }
        }

        private DataTeam m_DataTeam = null;
        public DataTeam DataTeam
        {
            get { return m_DataTeam; }
            set { m_DataTeam = value; }
        }

        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private TreeNode m_node = null;
        public TreeNode Node
        {
            get { return m_node; }
            set { m_node = value; }
        }

        private Boolean m_DayLight_Day = false;
        public Boolean DayLight_Day
        {
            get
            {
                return m_DayLight_Day;
            }
            set
            {
                m_DayLight_Day = value;
            }
        }

        private Boolean m_DayLight_Night = false;
        public Boolean DayLight_Night
        {
            get
            {
                return m_DayLight_Night;
            }
            set
            {
                m_DayLight_Night = value;
            }
        }

    }

    public class Team
    {
        private int m_nTeamID = -1;
        private string m_strTeamName = "";
        private bool m_bVisible = true;

        public int TeamID
        {
            get { return m_nTeamID; }
            set { m_nTeamID = value; }
        }

        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }

        public bool Visible
        {
            get { return m_bVisible; }
            set { m_bVisible = value; }
        }
    }

    public class UserDefinedTeam : Team, IComparable
    {
        private string m_strPhoneNumber = "";
        private string m_strFaxNumber = "";

        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }

        public string FaxNumber
        {
            get { return m_strFaxNumber; }
            set { m_strFaxNumber = value; }
        }

        public int CompareTo(object obj)
        {
            UserDefinedTeam team1 = this;
            UserDefinedTeam team2 = (UserDefinedTeam)obj;

            int nResult = team1.TeamID.CompareTo(team2.TeamID);

            return nResult;
        }
    }

    public class Data_TemporaryTeam
    {
        /*public enum TeamType
        {
            Unknown = -1,
            RegularTeam = 0,        // 정규조직
            CompanyMember,          // 정직원
            ExternalCompanyTeam,    // 사용안함
            ExternalTeam,           // 외부 협력업체 회사 및 팀
            ExternalCompanyMember,  // 외부 협력업체 팀원
            UserDefinedTeam,        // 사용자 정의 조직
            JobLevel                // 직급
        };*/

        private int m_nID = 0;
        //private int m_nTeamID = 0;
        private int m_nParentTeamID = 0;
        private string m_strTeamName = "";
        //private int m_nLevelNo = 0;
        //protected TeamType m_teamType = TeamType.Unknown;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        /*public int TeamID
        {
            get { return m_nTeamID; }
            set { m_nTeamID = value; }
        }*/

        public int ParentTeamID
        {
            get { return m_nParentTeamID; }
            set { m_nParentTeamID = value; }
        }

        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }

        /*public int LevelNo
        {
            get { return m_nLevelNo; }
            set { m_nLevelNo = value; }
        }

        public TeamType GetTeamType()
        {
            return m_teamType;
        }

        public void SetTeamType(TeamType type)
        {
            m_teamType = type;
        }

        public static bool TryToTeamType(int nTeamType, out TeamType teamType)
        {
            teamType = TeamType.Unknown;

            if (nTeamType <= (int)TeamType.Unknown || nTeamType > (int)TeamType.JobLevel)
                return false;

            teamType = (TeamType)nTeamType;
            return true;
        }*/
    }

    public class Data_NormalTeam : Data_TemporaryTeam
    {
    }

    public class Data_EmergencyTeam : Data_TemporaryTeam
    {
    }

    public class PhoneNumber : IComparable
    {
        private int m_nHeader = 0, m_nBody = 0, m_nTail = 0;
        private int m_nBodyLen = 0; 
        private bool m_isBlank = false;

        public string Number
        {
            get { return GetPhoneNumber(); }
            set { SetPhoneNumber(value); }
        } 

        public bool IsValid
        {
            get
            {
                if (m_nHeader == 0 && m_nBody == 0 && m_nTail == 0)
                    return false;

                return true;
            }
        }

        public bool IsBlank
        {
            get { return this.m_isBlank; }
        }


        public PhoneNumber()
        {
        }

        public PhoneNumber(string strPhoneNumber)
        {
            if (!String.IsNullOrWhiteSpace(strPhoneNumber))
            {
                SetPhoneNumber(strPhoneNumber);
            }
            else
            {
                m_isBlank = true;
            } 
        }

        public int CompareTo(object obj)
        {
            PhoneNumber phone1 = this;
            PhoneNumber phone2 = (PhoneNumber)obj;

            if (phone1.m_nHeader < phone2.m_nHeader)
                return -1;
            else if (phone1.m_nHeader > phone2.m_nHeader)
                return 1;

            if (phone1.m_nBody < phone2.m_nHeader)
                return -1;
            else if (phone1.m_nBody > phone2.m_nBody)
                return 1;

            if (phone1.m_nTail < phone2.m_nTail)
                return -1;
            else if (phone1.m_nTail > phone2.m_nTail)
                return 1;

            return 0;
        }

        public override string ToString()
        {
            string strPhoneNumber = GetPhoneNumber();

            //if (!m_isChanged && strPhoneNumber.Length > 0)
            //    return TeamGrid.SECRET_VALUE;

            return strPhoneNumber;
        }

        private string GetPhoneNumber()
        {
            if (m_nBodyLen == 3)
                return string.Format("01{0}-{1:000}-{2:0000}", m_nHeader, m_nBody, m_nTail);
            else if (m_nBodyLen == 4)
                return string.Format("01{0}-{1:0000}-{2:0000}", m_nHeader, m_nBody, m_nTail);

            return "";
        }

        private void SetPhoneNumber(string strPhoneNumber)
        {
            string[] arrTokens = strPhoneNumber.Trim().Split('-');
            int nTokenCount = arrTokens.Count();

            m_nHeader = m_nBody = m_nTail = m_nBodyLen = 0;

            if (nTokenCount == 3)
                SetPhoneNumber2(arrTokens[0].Trim(), arrTokens[1].Trim(), arrTokens[2].Trim());
            else if (nTokenCount == 2)
                SetPhoneNumber2(arrTokens[0].Trim() + arrTokens[1].Trim());
            else if (nTokenCount == 1)
                SetPhoneNumber2(strPhoneNumber.Trim()); 
        }

        private bool SetPhoneNumber2(string strHead, string strBody, string strTail)
        {
            if (!strHead.StartsWith("01") || strHead.Length != 3)
                return false;

            char chHead = strHead.ElementAt(2);

            if (chHead < '0' || chHead > '9')
                return false;

            int nBody = 0, nTail = 0;
            int nBodyLen = strBody.Length;
            int nTailLen = strTail.Length;

            if (nBodyLen < 3 || nBodyLen > 4 || nTailLen != 4)
                return false;

            if (!int.TryParse(strBody, out nBody))
                return false;

            if (!int.TryParse(strTail, out nTail))
                return false;

            m_nHeader = chHead - '0';
            m_nBody = nBody;
            m_nTail = nTail;
            m_nBodyLen = nBodyLen;

            return true;
        }

        private bool SetPhoneNumber2(string strPhoneNumber)
        {
            int len = strPhoneNumber.Length;

            bool readNum = false;
            int nIndex1 = -1, nIndex2 = -1;

            for (int i = 0; i < len; i++)
            {
                char ch = strPhoneNumber.ElementAt(i);

                if (ch >= '0' && ch <= '9')
                {
                    readNum = true;
                }
                else if (ch == ' ' || ch == '\t')
                {
                    if (readNum)
                    {
                        readNum = false;

                        if (nIndex1 < 0)
                            nIndex1 = i;
                        else
                        {
                            nIndex2 = i;
                            break;
                        }
                    }
                }
            }

            if (nIndex1 >= 0 && nIndex2 > nIndex1)
            {
                string str1 = strPhoneNumber.Substring(0, nIndex1).Trim();
                string str2 = strPhoneNumber.Substring(nIndex1, nIndex2 - nIndex1 - 1).Trim();
                string str3 = strPhoneNumber.Substring(nIndex2).Trim();

                return SetPhoneNumber2(str1, str2, str3);
            }
            else if (nIndex1 >= 0)
            {
                string str1 = strPhoneNumber.Substring(0, nIndex1).Trim();
                string str2 = strPhoneNumber.Substring(nIndex1).Trim();

                int len1 = str1.Length;
                int len2 = str2.Length;

                if (len1 == 3 && (len2 == 7 || len2 == 8))
                {
                    return SetPhoneNumber2(str1, str2.Substring(0, len2 - 4), str2.Substring(len2 - 4));
                }
                else if ((len1 == 6 || len1 == 7) || len2 == 4)
                {
                    return SetPhoneNumber2(str1.Substring(0, 3), str1.Substring(3), str2);
                }
            }
            else
            {
                if (len == 10 || len == 11)
                {
                    string str1 = strPhoneNumber.Substring(0, 3);
                    string str2 = strPhoneNumber.Substring(3, len - 7);
                    string str3 = strPhoneNumber.Substring(len - 4);

                    return SetPhoneNumber2(str1, str2, str3);
                }
            }

            return false;
        }
    }

    public class OfficePhoneNumber : IComparable
    {
        string[] m_strfirstNums = { "070", "080", "010", "011", "019", "02", "031", "032", "033", "041", "042", "043", "044", "051", "052", "053", "054", "055", "061", "062", "063", "064" };

        private int m_nTotal = 0;
        private int m_nHeader = 0, m_nBody = 0, m_nTail = 0;
        private int m_nHeaderLen = 0, m_nBodyLen = 0;
        private bool m_isChanged = false;
        private bool m_isBlank = false;

        public string Number
        {
            get { return GetPhoneNumber(); }
            set { SetPhoneNumber(value); }
        }

        public bool IsChanged
        {
            get { return m_isChanged; }
            set { m_isChanged = value; }
        }

        public bool IsValid
        {
            get
            {
                if (m_nHeader == 0 && m_nBody == 0 && m_nTail == 0)
                    return false;

                return true;
            }
        }

        public bool IsBlank
        {
            get { return this.m_isBlank; }
        }


        public OfficePhoneNumber()
        {
        }

        public OfficePhoneNumber(string strPhoneNumber, bool isChanged)
        {
            if (!String.IsNullOrWhiteSpace(strPhoneNumber))
            {
                SetPhoneNumber(strPhoneNumber);
            }
            else
            {
                m_isBlank = true;
            }


            m_isChanged = isChanged;
        }

        public int CompareTo(object obj)
        {
            OfficePhoneNumber phone1 = this;
            OfficePhoneNumber phone2 = (OfficePhoneNumber)obj;

            if (phone1.m_nHeader < phone2.m_nHeader)
                return -1;
            else if (phone1.m_nHeader > phone2.m_nHeader)
                return 1;

            if (phone1.m_nBody < phone2.m_nHeader)
                return -1;
            else if (phone1.m_nBody > phone2.m_nBody)
                return 1;

            if (phone1.m_nTail < phone2.m_nTail)
                return -1;
            else if (phone1.m_nTail > phone2.m_nTail)
                return 1;

            return 0;
        }

        public override string ToString()
        {
            string strPhoneNumber = GetPhoneNumber();

            //if (!m_isChanged && strPhoneNumber.Length > 0)
            //    return TeamGrid.SECRET_VALUE;

            return strPhoneNumber;
        }

        private string GetPhoneNumber()
        {
            string strHead = "";
            if (m_nHeaderLen == 2)
                strHead = "{0:00}";
            else if (m_nHeaderLen == 3)
                strHead = "{0:000}";
            else if (m_nHeaderLen == 4)
                strHead = "{0:0000}";
            else
                return "";

            string strBody = "";
            if (m_nTotal > 1)
            {
                if (m_nBodyLen == 3)
                    strBody = "-{1:000}";
                else if (m_nBodyLen == 4)
                    strBody = "-{1:0000}";
                else
                    return "";
            }

            string strTail = "";
            if (m_nTotal > 2)
                strTail = "-{2:0000}";

            if (m_nTotal == 1)
                return string.Format(strHead, m_nHeader);
            else if (m_nTotal == 2)
                return string.Format(strHead + strBody, m_nHeader, m_nBody);
            else if (m_nTotal == 3)
                return string.Format(strHead + strBody + strTail, m_nHeader, m_nBody, m_nTail);

            return "";
        }

        private void SetPhoneNumber(string strPhoneNumber)
        {
            string[] arrTokens = strPhoneNumber.Trim().Split('-');
            int nTokenCount = arrTokens.Count();

            m_nHeader = m_nBody = m_nTail = m_nHeaderLen = m_nBodyLen = 0;

            if (nTokenCount == 3)
                SetPhoneNumber2(arrTokens[0].Trim(), arrTokens[1].Trim(), arrTokens[2].Trim());
            else if (nTokenCount == 2 && strPhoneNumber.Length > 9)
                SetPhoneNumber2(arrTokens[0].Trim() + arrTokens[1].Trim());
            else if (nTokenCount == 2 && strPhoneNumber.Length == 9)
                SetPhoneNumber2(arrTokens[0].Trim(), arrTokens[1].Trim(), "");
            else if (nTokenCount == 1)
                SetPhoneNumber2(strPhoneNumber.Trim());
            else
            {
                m_isChanged = false;
            }
        }

        private bool SetPhoneNumber2(string strHead, string strBody, string strTail)
        {
            int nHead = 0, nBody = 0, nTail = 0;
            int nHeadLen = strHead.Length;
            int nBodyLen = strBody.Length;
            int nTailLen = strTail.Length;

            //양식
            //00-000-0000
            //00-0000-0000
            //000-000-0000
            //000-0000-0000
            //0000-0000
            //0000

            //if (nHeadLen < 2 || nHeadLen > 3 || nBodyLen < 3 || nBodyLen > 4 || nTailLen != 4)
            //    return false;

            if (nHeadLen > 0 && !int.TryParse(strHead, out nHead))
                return false;

            if (nBodyLen > 0 && !int.TryParse(strBody, out nBody))
                return false;

            if (nTailLen > 0 && !int.TryParse(strTail, out nTail))
                return false;

            m_nHeader = nHead;
            m_nBody = nBody;
            m_nTail = nTail;
            m_nHeaderLen = nHeadLen;
            m_nBodyLen = nBodyLen;
            if (nHeadLen > 0 && nBodyLen > 0 && nTailLen > 0)
                m_nTotal = 3;
            else if (nHeadLen > 0 && nBodyLen > 0 && nTailLen == 0)
                m_nTotal = 2;
            else if (nHeadLen > 0 && nBodyLen == 0 && nTailLen == 0)
                m_nTotal = 1;
            return true;
        }

        private bool SetPhoneNumber2(string strPhoneNumber)
        {
            int len = strPhoneNumber.Length;

            bool readNum = false;
            int nIndex1 = -1, nIndex2 = -1;

            for (int i = 0; i < len; i++)
            {
                char ch = strPhoneNumber.ElementAt(i);

                if (ch >= '0' && ch <= '9')
                {
                    readNum = true;
                }
                else if (ch == ' ' || ch == '\t')
                {
                    if (readNum)
                    {
                        readNum = false;

                        if (nIndex1 < 0)
                            nIndex1 = i;
                        else
                        {
                            nIndex2 = i;
                            break;
                        }
                    }
                }
            }

            if (nIndex1 >= 0 && nIndex2 > nIndex1)
            {
                string str1 = strPhoneNumber.Substring(0, nIndex1).Trim();
                string str2 = strPhoneNumber.Substring(nIndex1, nIndex2 - nIndex1).Trim();
                string str3 = strPhoneNumber.Substring(nIndex2).Trim();

                return SetPhoneNumber2(str1, str2, str3);
            }
            else if (nIndex1 >= 0)
            {
                string str1 = strPhoneNumber.Substring(0, nIndex1).Trim();
                string str2 = strPhoneNumber.Substring(nIndex1).Trim();

                int len1 = str1.Length;
                int len2 = str2.Length;

                if (len1 == 3 && (len2 == 7 || len2 == 8))
                {
                    return SetPhoneNumber2(str1, str2.Substring(0, len2 - 4), str2.Substring(len2 - 4));
                }
                else if ((len1 == 6 || len1 == 7) || len2 == 4)
                {
                    return SetPhoneNumber2(str1.Substring(0, 3), str1.Substring(3), str2);
                }
            }
            else
            {
                string str1 = "";
                string str2 = "";
                string str3 = "";

                if (len == 4)
                {
                    str1 = strPhoneNumber;
                    return SetPhoneNumber2(str1, str2, str3);
                }
                else if (len == 8)
                {
                    str1 = strPhoneNumber.Substring(0, 4);
                    str2 = strPhoneNumber.Substring(4, 4);
                    return SetPhoneNumber2(str1, str2, str3);
                }
                else if (len == 9 || len == 10 || len == 11)
                {
                    string head2 = strPhoneNumber.Substring(0, 2);
                    string head3 = strPhoneNumber.Substring(0, 3);

                    if (m_strfirstNums.Contains(head2))
                    {
                        str1 = strPhoneNumber.Substring(0, 2);
                        str3 = strPhoneNumber.Substring(len - 4);
                        str2 = strPhoneNumber.Substring(str1.Length, len - str1.Length - str3.Length);
                        return SetPhoneNumber2(str1, str2, str3);
                    }
                    else if (m_strfirstNums.Contains(head3))
                    {
                        str1 = strPhoneNumber.Substring(0, 3);
                        str3 = strPhoneNumber.Substring(len - 4);
                        str2 = strPhoneNumber.Substring(str1.Length, len - str1.Length - str3.Length);
                        return SetPhoneNumber2(str1, str2, str3);
                    }
                }
            }

            return false;
        }
    

    } 
   
}
