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
using System.Reflection;
using DBUtility;

namespace ControlTeamEditor
{
    public partial class FormMain : Form
    {
        private Size m_LargeSize = new Size(1078, 540);
        private Size m_SmallSize = new Size(399, 540);

        private DataManager m_dataMgr = null;
        private WebDBManager m_dbMgr = null;
        
        private Button[] mSelectBtns = null;
        private Button[] mDeleteBtns = null;
        private TextBox[] mEditMembers = null;
        private TextBox[] mEditJobs = null;

        public static void SetDoubleBuffer(Panel panel, bool bEnabled)
        {
            Type dgvType1 = panel.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(panel, bEnabled, null);
        }

        public static void SetDoubleBuffer(DataGridView gvView, bool bEnabled)
        {
            Type dgvType1 = gvView.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(gvView, bEnabled, null);
        }

        public FormMain(int nSiteID)
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            SetDoubleBuffer(gridMemebers, true);

            m_dbMgr = new WebDBManager(nSiteID);
            m_dataMgr = new DataManager(m_dbMgr, nSiteID);

            FormWorkSchedule frm = new FormWorkSchedule(nSiteID);
            frm.ShowDialog();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            InitTree();

            cmbWorkingTeam.DataSource = m_dataMgr.GetControlTeams();
            cmbWorkingTeam.DisplayMember = "DisplayText";

            cmbLocation.DataSource = m_dataMgr.GetControlRooms();
            cmbLocation.DisplayMember = "DisplayText";

            cmbTeam.DataSource = m_dataMgr.GetControlTeams();
            cmbTeam.DisplayMember = "DisplayText";

            List<DataControlTeamJobPosition> arPositions = m_dataMgr.GetJobPositions();
            editJob1.DataBindings.Add("text", arPositions[0], "JobName");
            editJob1.Tag = arPositions[0];
            editJob2.DataBindings.Add("text", arPositions[1], "JobName");
            editJob2.Tag = arPositions[1];
            editJob3.DataBindings.Add("text", arPositions[2], "JobName");
            editJob3.Tag = arPositions[2];
            editJob4.DataBindings.Add("text", arPositions[3], "JobName");
            editJob4.Tag = arPositions[3];
            editJob5.DataBindings.Add("text", arPositions[4], "JobName");
            editJob5.Tag = arPositions[4];
            editJob6.DataBindings.Add("text", arPositions[5], "JobName");
            editJob6.Tag = arPositions[5];
            editJob7.DataBindings.Add("text", arPositions[6], "JobName");
            editJob7.Tag = arPositions[6];
            editJob8.DataBindings.Add("text", arPositions[7], "JobName");
            editJob8.Tag = arPositions[7];

            mSelectBtns = new Button[] {
                btnSelect1,  btnSelect2, btnSelect3, btnSelect4, btnSelect5, btnSelect6, btnSelect7, btnSelect8
            };

            mDeleteBtns = new Button[] {
                btnDelete1,  btnDelete2, btnDelete3, btnDelete4, btnDelete5, btnDelete6, btnDelete7, btnDelete8
            };

            mEditJobs = new TextBox[] {
                editJob1,  editJob2, editJob3, editJob4, editJob5, editJob6, editJob7, editJob8
            };

            mEditMembers = new TextBox[] {
                editMember1,  editMember2, editMember3, editMember4, editMember5, editMember6, editMember7, editMember8
            };

            btnSelect1.Tag = editMember1;
            btnSelect2.Tag = editMember2;
            btnSelect3.Tag = editMember3;
            btnSelect4.Tag = editMember4;
            btnSelect5.Tag = editMember5;
            btnSelect6.Tag = editMember6;
            btnSelect7.Tag = editMember7;
            btnSelect8.Tag = editMember8;
            
            btnDelete1.Tag = editMember1;
            btnDelete2.Tag = editMember2;
            btnDelete3.Tag = editMember3;
            btnDelete4.Tag = editMember4;
            btnDelete5.Tag = editMember5;
            btnDelete6.Tag = editMember6;
            btnDelete7.Tag = editMember7;
            btnDelete8.Tag = editMember8;

            FillData();

            this.Size = m_LargeSize;
        }

        private int GetEditIndex(TextBox box)
        {
            for (int i = 0; i < mEditMembers.Length; i++)
            {
                if(mEditMembers[i] == box)
                {
                    return i;
                }
            }
            return 0;
        }

        private void SaveData()
        {
            m_dataMgr.SaveControlTeamMembers();
            m_dataMgr.SaveControlWorkingTeams();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveData();

            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ResizeForm()
        {
           if( this.Size == m_SmallSize)
           {
               this.Size = m_LargeSize;
           }
        }

        private void OnSelectMember(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            ResizeForm();

            DataGridViewSelectedRowCollection rows = gridMemebers.SelectedRows;
            if (rows.Count == 0)
                return;

            DataGridViewRow row = rows[0];
            DataCompanyMember member = (DataCompanyMember)row.Cells[1].Tag;

            TextBox textBox = (TextBox)btn.Tag;


            DataControlRoom room = (DataControlRoom)cmbLocation.SelectedItem;
            DataControlTeam team = (DataControlTeam)cmbTeam.SelectedItem;

            int i = GetEditIndex(textBox);
            DataControlTeamJobPosition job = (DataControlTeamJobPosition)(mEditJobs[i].Tag);
            DataControlTeamMember teamMember = m_dataMgr.GetControlTeamMember(room, team, job);
            if (teamMember != null)
            {
                teamMember.Member = member;
            }
            textBox.Text = member.MemberName;
            textBox.Tag = teamMember;
        }

        private void OnDeleteMember(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            TextBox textBox = (TextBox)btn.Tag;

            if (textBox != null)
            {
                textBox.Text = "";
                DataControlTeamMember team = (DataControlTeamMember)textBox.Tag;
                team.Member = null;
            }            
        }

        private void LoadRegularTeamTree(DataTeam teamRoot)
        {          
            ArrayList arrRegularTeam = teamRoot.ChildTeams;
            foreach (DataTeam data in arrRegularTeam)
            {
                if (data.ParentTeam == null)
                {
                    TreeNode node = treeViewTeam.Nodes.Add(data.TeamName.TrimEnd());
                    node.Tag = data;
                }
                else
                {
                    TreeNode child = FindNode(data.ParentTeam, treeViewTeam.Nodes);
                    if (child == null)
                        return;

                    TreeNode newNode = child.Nodes.Add(data.TeamName.TrimEnd());
                    newNode.Tag = data;
                }

                LoadRegularTeamTree(data);
            }
        }

        public void InitTree()
        {
            treeViewTeam.Nodes.Clear();

            DataTeam teamRoot = m_dataMgr.RegularTeamRoot;
            TreeNode rootNode = treeViewTeam.Nodes.Add(teamRoot.TeamName.TrimEnd());
            rootNode.Tag = teamRoot;

            LoadRegularTeamTree(teamRoot);

            treeViewTeam.ExpandAll();
            treeViewTeam.SelectedNode = treeViewTeam.Nodes[0];
        }

        private TreeNode FindNode(DataTeam nTag, TreeNodeCollection parentNodes = null)
        {
            TreeNodeCollection nodes = parentNodes == null ? treeViewTeam.Nodes : parentNodes;

            foreach (TreeNode node in nodes)
            {
                if (node.Tag == nTag)
                    return node;

                TreeNode result = FindNode(nTag, node.Nodes);
                if (result != null)
                    return result;
            }

            return null;
        }

        public TreeNode FindNode(string strValue, TreeNodeCollection parentNodes = null)
        {
            TreeNodeCollection nodes = parentNodes == null ? treeViewTeam.Nodes : parentNodes;

            foreach (TreeNode node in nodes)
            {
                if (strValue == node.Text)
                    return node;
                TreeNode result = FindNode(strValue, node.Nodes);
                if (result != null)
                    return result;
            }

            return null;
        }

        private int FindTagInNode(string strValue)
        {
            TreeNode node = FindNode(strValue, treeViewTeam.Nodes);
            if (node != null)
            {
                if (node.Text == strValue)
                    return (int)node.Tag;
            }

            return -1;
        }

        private void treeViewTeam_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = treeViewTeam.SelectedNode;
            if( node != null)
            {
                DataTeam team = (DataTeam)node.Tag;
                SetGrid(team);
            }
        }

        private void SetGrid(DataTeam team)
        {
            if (team == null)
                return;

            gridMemebers.ClearSelection();
            gridMemebers.Rows.Clear();

            ArrayList arMembers = m_dataMgr.GetTeamMembers(team);

            int nCount = 1;
            foreach(DataCompanyMember member in arMembers)
            {
                DataGridViewRow row = new DataGridViewRow();

                DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
                cell1.Value = nCount++;
                cell1.Tag = team;
                row.Cells.Add(cell1);

                DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                cell2.Value = member.LevelID;
                cell2.Tag = member;
                row.Cells.Add(cell2);
                
                DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                cell3.Value = member.MemberName;
                cell3.Tag = member;
                row.Cells.Add(cell3);

                gridMemebers.Rows.Add(row);
            }
        }
             

        private void FillData(DataControlRoom loc , DataControlTeam team)
        {
            for(int i = 0 ; i < mEditJobs.Length ; i++)
            {
                DataControlTeamJobPosition job = (DataControlTeamJobPosition)(mEditJobs[i].Tag);

                DataControlTeamMember member = m_dataMgr.GetControlTeamMember(loc, team, job);

                if( member != null )
                {
                    if (member.Member != null)
                        mEditMembers[i].Text = member.Member.MemberName;
                    else
                        mEditMembers[i].Text = "";
                    mEditMembers[i].Tag = member;
                }
                
            }
        }

        private void FillData()
        {
            DataControlRoom room = (DataControlRoom)cmbLocation.SelectedItem;
            DataControlTeam team = (DataControlTeam)cmbTeam.SelectedItem;
            if (room != null && team != null)
            {
                FillData(room, team);


                DataControlWorkingTeam work = m_dataMgr.GetWorkTeam(room.ID);
                if (work != null && work.Team != null)
                {
                    bool bFind = false;
                    foreach (DataControlTeam team2 in cmbWorkingTeam.Items)
                    {
                        if (team2.ID == work.Team.ID)
                        {
                            cmbWorkingTeam.SelectedItem = team2;
                            bFind = true;
                            break;
                        }
                    }
                    if (bFind == false)
                    {
                        cmbWorkingTeam.SelectedItem = cmbWorkingTeam.Items[0];
                    }
                }
                else
                {                    
                    cmbWorkingTeam.SelectedItem = cmbWorkingTeam.Items[0];                    
                }                
            }    
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataControlRoom room = (DataControlRoom)cmbLocation.SelectedItem;
            if (room != null)
            {
                lbLocation.Text = room.DisplayText;

            }
        }

        private void cmbLocation_SelectionChangeCommitted(object sender, EventArgs e)
        {
            FillData();
            
        }

        private void cmbTeam_SelectionChangeCommitted(object sender, EventArgs e)
        {
            FillData();
        }

        private void cmbWorkingTeam_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeWorkingTeam();
        }

        private void cmbWorkingTeam_SelectionChangeCommitted(object sender, EventArgs e)
        {            
        }

        private void OnSelectWorkingTeam(object sender, EventArgs e)
        {
            ChangeWorkingTeam();
        }

        private void ChangeWorkingTeam()
        {
            DataControlRoom room = (DataControlRoom)cmbLocation.SelectedItem;
            DataControlTeam team = (DataControlTeam)cmbWorkingTeam.SelectedItem;

            if (room != null && team != null)
            {
                DataControlWorkingTeam work = m_dataMgr.GetWorkTeam(room.ID);
                if (work != null)
                {
                    work.Team = team;
                }
            }
        }
    }
}
