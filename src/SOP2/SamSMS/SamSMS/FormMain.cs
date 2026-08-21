using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace SamSMS
{
    public partial class FormMain : Form
    {
        static private FormMain m_Instance = null;
        static public SamSMS.FormMain Instance
        {
            get { return m_Instance; }
        }

        private DataManager m_DataMan = new DataManager();
        private bool bLengthOver = false;
        private ArrayList mCheckedTeam = new ArrayList();

        private ArrayList m_arTeamRoots = new ArrayList();
        public FormMain()
        {
            if (m_Instance == null)
                m_Instance = this;

            InitializeComponent();
            treeView1.CheckBoxes = true;
            lableLength.ForeColor = Color.Blue;

			//string abc = AES256Cipher.AES_encrypt("01052672290", key);
			//MessageBox.Show(abc);
			//Debug.WriteLine(abc);
        }
		private string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        private void FindRootNode(TreeNode rootNode, ArrayList teamList)
        {
            DataTeam rootTeam = null;
            foreach (DataTeam team in teamList)
            {
                if (team.ParentTeamID == -1)
                {
                    rootTeam = team;
                    rootNode.Text = team.TeamName;
                    rootNode.Tag = team;                           
                }
            }
            if (rootTeam != null)
            {
                teamList.Remove(rootTeam);
            }
        }

        private void MakeExternalNodes(ArrayList teamList)
        {
            foreach (DataTeam team in teamList)
            {
                if (team.ParentTeamID == -1)
                {
                    TreeNode node = new TreeNode(team.TeamName);
                    treeView1.Nodes.Add(node);
                    node.Tag = team;
                    m_arTeamRoots.Add(node);
                    ArrayList arChild = (ArrayList)teamList.Clone();
                    arChild.Remove(team);
                    MakeTreeView(node, arChild);
                }
            }
        }

        private void MakeTreeView(TreeNode nodeParent, ArrayList teamList)
        {
            if (teamList.Count == 0)
                return;

            DataTeam rootTeam = (DataTeam)nodeParent.Tag;
            foreach (DataTeam team in teamList)
            {
                if (team.ParentTeamID == rootTeam.ID)
                {
                    TreeNode node = new TreeNode(team.TeamName);
                    node.Tag = team;
                    nodeParent.Nodes.Add(node);

                    ArrayList arChild = (ArrayList)teamList.Clone();
                    arChild.Remove(team);
                    MakeTreeView(node, teamList);
                }
            }
        }

        private void InitCheck()
        {
            cbLevel4.Checked = true;
            cbPos1.Checked = true;
            cbPos2.Checked = true;
            cbToAll.Checked = true;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

            InitCheck();

            lbCompany.Text = string.Format("{0} 명", m_DataMan.CompanyMember);
            lbExternal.Text = string.Format("{0} 명", m_DataMan.ExternalMember);
            lbTimeOff.Text = string.Format("{0} 명", m_DataMan.TimeOffMember);

            lbLevelOne.Text = string.Format("{0} 명", m_DataMan.LevelOne);
            lbLevelTwo.Text = string.Format("{0} 명", m_DataMan.LevelTwo);
            lbLevelThree.Text = string.Format("{0} 명", m_DataMan.LevelThree);
            lbLevelFour.Text = string.Format("{0} 명", m_DataMan.LevelFour);

            TreeNode root = treeView1.Nodes[0]; // 삼천포 root
            m_arTeamRoots.Add(root);
            ArrayList arRegular = (ArrayList)m_DataMan.RegularTeamList.Clone();
            FindRootNode(root, arRegular);
            MakeTreeView(root, arRegular);

            ArrayList arExternal = (ArrayList)m_DataMan.ExternalTeamList.Clone();
            MakeExternalNodes(arExternal); 
           


        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {

            
            
            ArrayList arSendingMember = null;
            string szMessage = "메세지 전송방법 : ";
            if (tabControl1.SelectedTab == tabPageToAll)
            {
                szMessage += "전체 임직원 ";

                if (cbToExternal.Checked == true)
                {
                    szMessage += "협력업체 포함 ";
                }

                if (cbToTimeOff.Checked == true)
                {
                    szMessage += "휴가자 포함 ";
                }

                arSendingMember = m_DataMan.GetTargetMemberAll(cbToAll.Checked, cbToExternal.Checked, cbToTimeOff.Checked);
            }
            else if (tabControl1.SelectedTab == tabPageTeam)
            {
                szMessage += "부서별 ";
                if (cbPos1.Checked == true)
                    szMessage += "팀원 ";
                if (cbPos2.Checked == true)
                    szMessage += "팀장 ";

                mCheckedTeam.Clear();
                foreach (TreeNode node in treeView1.Nodes)
                {
                    if (node.Checked == true)
                    {
                        mCheckedTeam.Add(node);
                    }
                    GetCheckedTeam(node, ref mCheckedTeam);
                }

                ArrayList arTeam = new ArrayList();
                foreach (TreeNode node in mCheckedTeam)
                {
                    arTeam.Add(node.Tag);
                }

                arSendingMember = m_DataMan.GetTargetMemberTeam(cbPos1.Checked, cbPos2.Checked, arTeam);
            }
            else if (tabControl1.SelectedTab == tabPageLevel)
            {
                szMessage += "직급별";
                arSendingMember = m_DataMan.GetTargetMemberLevel(cbLevel1.Checked, cbLevel2.Checked, cbLevel3.Checked, cbLevel4.Checked);
            }

            if (arSendingMember == null || arSendingMember.Count == 0)
                return;

            string szMsg = textBox1.Text;
            if (szMsg == null || szMsg.Equals(""))
            {
                MessageBox.Show(this, "전송할 메세지가 없습니다.", "메세지 전송", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }      

            if (bLengthOver == true)
            {
                if (MessageBox.Show(this, "메세지 길이가 80바이트가 넘습니다. \n메세지가 잘리거나 분할되어 전송됩니다.\n계속 하시겠습니까?", "메세지 전송", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                {
                    return;
                }
            }
            szMessage += string.Format(" {0} 명", arSendingMember.Count);
            if (MessageBox.Show(this, szMessage + "\n\n위와 같이 메세지를 전송합니다.\n계속 하시겠습니까?", "메세지 전송", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            { 

                SendMessage(szMsg, arSendingMember);
            }
        }
        
        private void TestMessage(string szMessage, string szCall)
        {
            this.m_DataMan.SendSMS(szCall, "01043632290", szMessage);
        }

        private void SendMessage(string szMessage, ArrayList memberList)
        {
            string szCaller = WebDBManager.SMSCaller;
            ArrayList arCall = new ArrayList();

            foreach (SendingMember member in memberList)
            {
                string szPhone = member.PhoneNumber;
                if (szPhone != null && !szPhone.Equals(""))
                {
                    arCall.Add(szPhone);
                }               
            }

            //TestMessage(szMessage, "01025893257");
            if (arCall.Count > 0)
                this.m_DataMan.SendSMS(arCall, szCaller, szMessage);
        }

        private bool CheckHangul(char c)
        {
            if (char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.OtherLetter)
            {
                return true;
            }
            return false;
        }

        private void tbMessageTextChanged(object sender, EventArgs e)
        {
            string szMessage = textBox1.Text;
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
                bLengthOver = true;
            }
            else
            {
                lableLength.ForeColor = Color.Blue;
                bLengthOver = false;
            }

            string szLength = string.Format("{0}/{1} {2}", sumByte, 80, "바이트");
            lableLength.Text = szLength;
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            
        }


        private void GetCheckedTeam(TreeNode pNode, ref ArrayList arResult)
        {
            foreach (TreeNode node in pNode.Nodes)
            {
                if (node.Checked == true)
                {
                    arResult.Add(node);                    
                }
                GetCheckedTeam(node, ref arResult);
            } 
        }

        private void treeView1_AfterCheck_1(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            //treeView1.SelectedNode = node;
            if (node != null && node.Checked == true) // 체크 이벤트인 경우
            {
                int nChild = node.GetNodeCount(true);
                foreach (TreeNode child in node.Nodes)
                {
                    child.Checked = true;
                    
                }
                mCheckedTeam.Add(node);
                node.ExpandAll();

            }
            else if (node != null && node.Checked == false)
            {
                int nChild = node.GetNodeCount(true);
                foreach (TreeNode child in node.Nodes)
                {
                    child.Checked = false;
                }
                mCheckedTeam.Remove(node);
                node.Collapse(false);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            InitToAll();
            InitToTeam();
            InitToLevel();

            InitCheck();
        }
        private void InitToAll()
        {
            cbToAll.Checked = false;
            cbToExternal.Checked = false;
            cbToTimeOff.Checked = false;
        }

        private void InitToTeam()
        {
            cbPos1.Checked = false;
            cbPos2.Checked = false;

            foreach (TreeNode node in m_arTeamRoots)
            {
                node.Checked = false;
            }

            mCheckedTeam.Clear();
        }

        private void InitToLevel()
        {
            cbLevel1.Checked = false;
            cbLevel2.Checked = false;
            cbLevel3.Checked = false;
            cbLevel4.Checked = false;           
        }
    }
}
