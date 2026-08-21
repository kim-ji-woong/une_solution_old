using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SOPMonitoringSystem
{
    public partial class DockingRightPersonnel : Form
    {
		SOPManager m_sopMgr = null;
        ArrayList m_arrCompanyMember = new ArrayList();
        ArrayList m_arrTeams = new ArrayList();
        ArrayList m_arrMember = new ArrayList();

        public DockingRightPersonnel()
        {
            InitializeComponent();

            InitPersonnel();
        }

        private void InitPersonnel()
        {
            ArrayList arrPersonnel = new ArrayList();
            arrPersonnel.Add(" 총 직원 수");
            arrPersonnel.Add(" SOP 전체 리소스");
            arrPersonnel.Add(" 임무 수행 인원 수");
            arrPersonnel.Add(" 임무 대기 인원 수");

            foreach (string strValue in arrPersonnel)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = null;

                cell = new DataGridViewTextBoxCell();
                cell.Value = strValue;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = "000 명";
                gridRow.Cells.Add(cell);

                dataGridPersonnel.Rows.Add(gridRow);
            }
        }

        public new bool Load(SOPManager sopMgr)
        {
            m_sopMgr = sopMgr;
            m_arrCompanyMember = sopMgr.CompanyMemberList;

            ReLoadNetwork(sopMgr.CompanyMemberList);
            dataGridPersonnel.Rows[0].Cells[1].Value = m_arrCompanyMember.Count.ToString() + " 명";

            return true;
        }

        private void ReLoadNetwork(ArrayList arrCompanyMember)
        {
            dataGridNetwork.Rows.Clear();
            foreach (Data_CompanyMember data in arrCompanyMember)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = null;

                cell = new DataGridViewTextBoxCell();
                cell.Value = data.MemberName;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = data.OfficePhoneNumber;
                gridRow.Cells.Add(cell);

                gridRow.Tag = data.ID;

                dataGridNetwork.Rows.Add(gridRow);
            }
        }

        private void dataGridPersonnel_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == 0)    // 전체 직원
            {
                ReLoadNetwork(m_arrCompanyMember);
            }
            else if (e.RowIndex == 1)
            {
                GetSOPTotalResource();
                ReLoadNetwork(m_arrMember);
                dataGridPersonnel.Rows[1].Cells[1].Value = m_arrMember.Count.ToString() + " 명";
            }
        }

        public ArrayList GetMemberPhoneNumber()
        {
            ArrayList arrPhoneNumber = new ArrayList();
            foreach (Data_CompanyMember data in m_arrCompanyMember)
            {
                arrPhoneNumber.Add(data.PhoneNumber);
            }

            return arrPhoneNumber;
        }

        public void GetSOPTotalResource()
        {
            FormMain.Instance.GetPageHome().GetTeamList();
            ArrayList arrMember = new ArrayList();
            ArrayList arrTeam = FormMain.Instance.GetPageHome().TeamList;
            m_arrMember.Clear();

            //전체 섹션의 프로세스의 팀 리스트 중
            foreach (Sections.SOPTeam team in arrTeam)
            {
                if (team.LevelNo > 0)
                {
                    ArrayList arrSOPMember = FormMain.Instance.GetLevelMember(team.LevelNo);
                    if (arrSOPMember == null) return;

                    foreach (Sections.SOPMember member in arrSOPMember)
                    {
                        foreach (Data_CompanyMember data in m_arrCompanyMember)
                        {
                            if (member.MemberID == data.ID && member.MemberName == data.MemberName)
                            {
                                arrMember.Add(data);
                            }
                        }
                    }
                }
                else
                {
                    ArrayList arrRegularTeam = team.RegularTeamIDList;
                    if (arrRegularTeam == null) continue;
                    foreach (int nRegularTeamID in arrRegularTeam)
                    {
                        foreach (Data_CompanyMember data in m_arrCompanyMember)
                        {
                            if (nRegularTeamID == data.RegularTeamID)
                            {
                                if (arrMember.Count == 0)
                                    arrMember.Add(data);
                                else
                                {
                                    bool isCheck = false;
                                    foreach (Data_CompanyMember member in arrMember)
                                    {
                                        if (member.RegularTeamID == data.RegularTeamID)
                                        {
                                            isCheck = true;
                                        }
                                    }
                                    if (!isCheck)
                                        arrMember.Add(data);
                                }
                            }
                        }
                    }
                }
            }
            m_arrMember = arrMember;
            dataGridPersonnel.Rows[1].Cells[1].Value = arrMember.Count.ToString() + " 명";
        }
    }
}
