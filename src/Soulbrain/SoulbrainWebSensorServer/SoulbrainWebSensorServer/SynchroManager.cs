using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TeamEditor.BLL;
using TeamEditor.Model.Sop.Team;

namespace SoulbrainWebSensorServer
{
    public class SynchroManager
    {
        enum TeamOption { JobLevel = 0, JobPosition, Status }

        private TeamEditor.DAL.DataManager m_dataManager = null;
        private TeamEditor.BLL.ProcessManager m_processManager = null;

        //private WSopDataManager m_wsopDataMgr = null;
        private HrDataManager m_hrDataMgr = null;
        private Thread m_SynchroThread = null;

        private bool m_bTimerChk = false;                           // 이미 타이머 실행 유무 체크
        private DateTime m_dtLast = new DateTime();

        private Dictionary<string, int> m_dicJobLevel = null;
        private Dictionary<string, int> m_dicJobPosition = null;
        private Dictionary<string, int> m_dicStatus = null;

        private bool m_shutdownThread = false;
        public void Shutdown()
        {
            m_shutdownThread = true;
            m_SynchroThread.Abort();
        }

        private bool m_startThread = false;
        public void StartThread()
        {
            m_startThread = true;
        }
        public void StopThread()
        {
            m_startThread = false;
        }

        public SynchroManager(dnsDBUtil.WebDBManager hrDBManager, TeamEditor.DAL.DataManager dataManager, TeamEditor.BLL.ProcessManager processManager)
        {
            m_dataManager = dataManager;
            m_processManager = processManager;
            m_hrDataMgr = new HrDataManager(hrDBManager);

            LoadTeamOptions();

            m_SynchroThread = new Thread(new ThreadStart(SynchroThread));
            m_SynchroThread.Name = "SynchroMember.Thread";
            m_SynchroThread.Start();
        }

        private bool LoadTeamOptions()
        {
            m_dicJobLevel = new Dictionary<string, int>();
            m_dicJobPosition = new Dictionary<string, int>();
            m_dicStatus = new Dictionary<string, int>();

            string strErrorMessage = "";

            m_dicJobLevel = LoadTeamOptions(TeamOption.JobLevel, out strErrorMessage);
            if (m_dicJobLevel == null)
                return false;

            m_dicJobPosition = LoadTeamOptions(TeamOption.JobPosition, out strErrorMessage);
            if (m_dicJobPosition == null)
                return false;

            m_dicStatus = LoadTeamOptions(TeamOption.Status, out strErrorMessage);
            if (m_dicStatus == null)
                return false;

            return true;
        }

        private Dictionary<string, int> LoadTeamOptions(TeamOption option, out string strErrorMessage)
        {
            strErrorMessage = "";
            Dictionary<string, int> dicOptions = null;

            string strPropertyName = "";

            if (option == TeamOption.JobLevel)
            {
                strPropertyName = "JobLevel";
            }
            else if (option == TeamOption.JobPosition)
            {
                strPropertyName = "JobPosition";
            }
            else if (option == TeamOption.Status)
            {
                strPropertyName = "Status";
            }
            else
            {
                strErrorMessage = "TeamOption 제대로 된 값이 아닙니다.";
                return dicOptions;
            }

            // 조회 
            Dictionary<Options.Fields, object> dicConditions = new Dictionary<Options.Fields, object>();
            dicConditions[Options.Fields.PropertyName] = strPropertyName;

            dicOptions = new Dictionary<string, int>();

            List<Options> options = m_dataManager.GetSelectManager().SelectOptions(dicConditions, "", out strErrorMessage);
            if (options == null)
                return dicOptions;

            foreach (Options data in options)
            {
                dicOptions[data.PropertyValue] = data.PropertyID;
            }

            return dicOptions;
        }
            

    private void SynchroThread()
        {
            while (!m_shutdownThread)
            {
                if (m_startThread)
                {
                    SynchroMember();

                    Thread.Sleep(60 * 1000);
                }
            }
        }

        private void SynchroMember()
        {
            DateTime dtNow = DateTime.Now;
            if ((dtNow - m_dtLast).TotalDays >= 1)
            {
                m_dtLast = DateTime.Now;
            }
            else
            {
                // 하루에 최초 한번 동작
                return;
            }

            // 타이머 실행 유무 체크
            if (m_bTimerChk == true)
                return;

            m_bTimerChk = true;                 // 타이머 실행 중 체크

            string strErrorMessage = "";
            Dictionary<string, HrRegular> dicHrRegulars = null;
            Dictionary<string, HrRegularMember> dicHrRegularMembers = null;
            
            List<HrRegular> rootRegulars = null;

            // DB 불러오기
            if (LoadHRData(out dicHrRegulars, out dicHrRegularMembers, out rootRegulars, out strErrorMessage) == false)
            {
                Logger.Instance.Write("LoadHRData 오류: " + strErrorMessage);
                m_bTimerChk = false;
                return;
            }

            Dictionary<string, List<RegularMember>> dicPathRegularMember = null;

            // 모듈에 맞춰 데이터 작업
            if (SetUpdateData(dicHrRegulars, dicHrRegularMembers, out dicPathRegularMember, out strErrorMessage) == false)
            {
                Logger.Instance.Write("SetUpdateData 오류: " + strErrorMessage);
                m_bTimerChk = false;
                return;
            }

            
            Dictionary<int, Regular> dicRegulars_Current;
            Dictionary<int, RegularMember> dicRegularMembers_Current;

            // 솔브레인 HR Root의 RegularMember 데이터 작업
            if (SetCurrentRegularMemberData(rootRegulars, out dicRegulars_Current, out dicRegularMembers_Current, out strErrorMessage) == false)
            {
                Logger.Instance.Write("SetCurrentRegularMemberData 오류: " + strErrorMessage);
                m_bTimerChk = false;
                return;
            }


            // 비교 및 동기화는 TeamEditor.BLL 에서 업데이트 모듈을 통해
            //if (m_processManager.GetSaveManager().UpdateRegularMemberData(dicPathRegularMember, out strErrorMessage, dicPathRegularMember_Current) == false)
            if (m_processManager.GetSaveManager().UpdateRegularMemberData(dicPathRegularMember, out strErrorMessage, dicRegulars_Current, dicRegularMembers_Current) == false)
            {
                Logger.Instance.Write("UpdateRegularMemberData 오류: " + strErrorMessage);
                m_bTimerChk = false;
                return;
            }


            m_bTimerChk = false;
        }

        private bool SetCurrentRegularMemberData(List<HrRegular> rootRegulars, out Dictionary<int, Regular> dicRegulars, out Dictionary<int, RegularMember> dicRegularMembers, out string strErrorMessage)
        {
            strErrorMessage = "";

            dicRegulars = new Dictionary<int, Regular>();
            dicRegularMembers = new Dictionary<int, RegularMember>();

            Dictionary<Regular.Fields, object> dicConditions = new Dictionary<Regular.Fields, object>();

            List<Regular> regulars = m_dataManager.GetSelectManager().SelectRegulars(dicConditions, out strErrorMessage);
            if (regulars == null)
                return false;

            Dictionary<RegularMember.Fields, object> dicConditions_RegularMember = new Dictionary<RegularMember.Fields, object>();
            string strAdditionalConditions = "";

            List<RegularMember> regularMembers = m_dataManager.GetSelectManager().SelectRegularMembers(dicConditions_RegularMember, strAdditionalConditions, out strErrorMessage);
            if (regularMembers == null)
                return false;

            foreach (Regular regular in regulars)
            {
                if (regular.ParentTeamID != null)
                    continue;

                bool bChk = false;

                foreach (HrRegular hrRegular in rootRegulars)
                {
                    if (hrRegular.TeamName == regular.TeamName)
                    {
                        bChk = true;
                        break;
                    }
                }

                if (bChk == true)
                {
                    // 루트 Regular
                    Regular team = new Regular();
                    team.ID = regular.ID;
                    team.ParentTeamID = null;
                    team.TeamName = regular.TeamName;
                    //team.Path = regular.TeamName;

                    dicRegulars[team.ID] = team;

                    // 자식 Regular 조회 
                    GetChildRegularTeam(team, regulars, ref dicRegulars);
                }
                
             
            }
            /*
            foreach (KeyValuePair<int, RegularTeam> pair in dicRegulars)
            {
                RegularTeam regular = pair.Value;

                dicPathRegularMember[regular.Path] = new List<RegularMember>();
            }

            foreach (RegularMember member in regularMembers)
            {
                if (dicRegulars.ContainsKey(member.RegularID))
                {
                    RegularTeam regular = dicRegulars[member.RegularID];

                    if (dicPathRegularMember.ContainsKey(regular.Path))
                    {
                        dicPathRegularMember[regular.Path].Add(member);
                    }
                    else
                    {
                        dicPathRegularMember[regular.Path] = new List<RegularMember>();
                        dicPathRegularMember[regular.Path].Add(member);
                    }
                } 
            }
            */

            foreach (KeyValuePair<int, Regular> pair in dicRegulars)
            {
                Regular regular = pair.Value;

                foreach (RegularMember member in regularMembers)
                {
                    if (member.RegularID == regular.ID)
                    {
                        dicRegularMembers[member.ID] = member;
                    }
                }
            }

            return true;
        }

        private void GetChildRegularTeam(Regular parentTeam, List<Regular> regulars, ref Dictionary<int, Regular> dicRegulars)
        {
            if (dicRegulars == null)
                dicRegulars = new Dictionary<int, Regular>();

            foreach (Regular regular in regulars)
            {
                if (parentTeam.ID == regular.ParentTeamID)
                {
                    Regular child = new Regular();
                    child.ID = regular.ID;
                    child.ParentTeamID = regular.ParentTeamID;
                    child.TeamName = regular.TeamName;

                    //child.Path = parentTeam.Path + "|" + child.TeamName;

                    dicRegulars[child.ID] = child;

                    GetChildRegularTeam(child, regulars, ref dicRegulars);
                }
            }
        }

        private bool SetUpdateData(Dictionary<string, HrRegular> dicHrRegulars, Dictionary<string, HrRegularMember> dicHrRegularMembers, out Dictionary<string, List<RegularMember>> dicPathRegularMember, out string strErrorMessage)
        {
            strErrorMessage = "";
            dicPathRegularMember = new Dictionary<string, List<RegularMember>>();

            foreach (KeyValuePair<string, HrRegular> pair in dicHrRegulars)
            {
                HrRegular hrRegular = pair.Value;

                if (dicPathRegularMember.ContainsKey(hrRegular.Path) == false)
                    dicPathRegularMember[hrRegular.Path] = new List<RegularMember>();
            }

            foreach (KeyValuePair<string, HrRegularMember> pair in dicHrRegularMembers)
            {
                HrRegularMember hrRegularMember = pair.Value;

                if (hrRegularMember.ORG_CD != null && hrRegularMember.ORG_CD != "")
                {
                    if (dicHrRegulars.ContainsKey(hrRegularMember.ORG_CD))
                    {
                        HrRegular hrRegular = dicHrRegulars[hrRegularMember.ORG_CD];

                        if (dicPathRegularMember.ContainsKey(hrRegular.Path))
                        {
                            dicPathRegularMember[hrRegular.Path].Add(hrRegularMember);
                        } 
                        else
                        {
                            dicPathRegularMember[hrRegular.Path] = new List<RegularMember>();
                            dicPathRegularMember[hrRegular.Path].Add(hrRegularMember);
                        }
                    }
                    else
                    {
                        Console.WriteLine(hrRegularMember.ORG_CD);
                    }
                }
                else
                {
                    Console.WriteLine(hrRegularMember.MemberID);
                }
            }

            return true;
        }

        private bool LoadHRData(out Dictionary<string, HrRegular> dicHrRegulars, out Dictionary<string, HrRegularMember> dicHrRegularMembers, out List<HrRegular> rootRegulars, out string strErrorMessage)
        {
            // DB 불러오기
            strErrorMessage = "";
            dicHrRegulars = null;
            dicHrRegularMembers = null;
            rootRegulars = null;

            List<HrTeamData> hrTeams = null;
            List<HrMemberData>  hrMembers = null;

            hrTeams = m_hrDataMgr.GetHrTeams(out strErrorMessage);
            if (hrTeams == null)
                return false;

            hrMembers = m_hrDataMgr.GetHrMembers(out strErrorMessage);
            if (hrMembers == null)
                return false;

            if (SetHRTeamData(hrTeams, out dicHrRegulars, out strErrorMessage) == false)
                return false;

            if (SetHRMemberData(hrMembers, out dicHrRegularMembers, out strErrorMessage) == false)
                return false;

            // 솔브레인 백업용 루트 Regular
            if (SetRootHRTeamData(hrTeams, out rootRegulars, out strErrorMessage) == false)
                return false;

            return true;
        }

        private bool SetRootHRTeamData(List<HrTeamData> hrTeams, out List<HrRegular> rootRegulars, out string strErrorMessage)
        {
            strErrorMessage = "";
            rootRegulars = new List<HrRegular>();

            if (hrTeams == null)
            {
                strErrorMessage = "hrTeams에 제대로 된 값이 들어있지 않습니다.";
                return false;
            }

            foreach (HrTeamData hrTeam in hrTeams)
            {
                int nLevel = -1;

                if (int.TryParse(hrTeam.ORG_LEVEL, out nLevel) == false)
                {
                    strErrorMessage = "ORG_LEVEL: " + hrTeam.ORG_LEVEL + ", 제대로 된 값이 들어있지 않습니다. ";
                    return false;
                }

                if (nLevel == 1)
                {   // 루트 Team
                    HrRegular hrRegular = new HrRegular();
                    hrRegular.ParentTeamID = null;
                    hrRegular.TeamName = hrTeam.ORG_NM;
                    hrRegular.ORG_CD = hrTeam.ORG_CD;
                    hrRegular.Path = hrTeam.ORG_NM;

                    rootRegulars.Add(hrRegular);
                }
            }

            return true;
        }

        private bool SetHRMemberData(List<HrMemberData> hrMembers, out Dictionary<string, HrRegularMember> dicHrRegularMembers, out string strErrorMessage)
        {
            strErrorMessage = "";
            dicHrRegularMembers = new Dictionary<string, HrRegularMember>();

            if (hrMembers == null)
            {
                strErrorMessage = "hrMembers에 제대로 된 값이 들어있지 않습니다.";
                return false;
            }

            foreach (HrMemberData hrMember in hrMembers)
            {
                HrRegularMember regularMember = new HrRegularMember();
                regularMember.MemberName = hrMember.NAME;
                regularMember.MemberID = hrMember.SABUN;
                regularMember.OfficePhoneNumber = hrMember.ADDRESS_OT;
                regularMember.PhoneNumber = hrMember.ADDRESS_HP;
                regularMember.Email = hrMember.ADDRESS_IM;

                //regularMember.RegularID
                regularMember.ORG_CD = hrMember.ORG_CD;

                //regularMember.JobLevelID
                if (hrMember.JIKCHAK_NM != null && hrMember.JIKCHAK_NM != "")
                {
                    int nJobLevelID = GetTeamOptionID(TeamOption.JobLevel, hrMember.JIKCHAK_NM, out strErrorMessage);
                    if (nJobLevelID == -1)
                        return false;

                    regularMember.JobLevelID = nJobLevelID;
                }

                //regularMember.JobPositionID
                if (hrMember.JIKWEE_NM != null && hrMember.JIKWEE_NM != "")
                {
                    int nJobPositionID = GetTeamOptionID(TeamOption.JobPosition, hrMember.JIKWEE_NM, out strErrorMessage);
                    if (nJobPositionID == -1)
                        return false;

                    regularMember.JobPositionID = nJobPositionID;
                }

                //regularMember.StatusID
                if (hrMember.STATUS_NM != null && hrMember.STATUS_NM != "")
                {
                    int nStatusID = GetTeamOptionID(TeamOption.Status, hrMember.STATUS_NM, out strErrorMessage);
                    if (nStatusID == -1)
                        return false;

                    regularMember.StatusID = nStatusID;
                }
                    
                dicHrRegularMembers[hrMember.SABUN] = regularMember;
            }

            return true;
        }

        private int GetTeamOptionID(TeamOption option, string strPropertyValue, out string strErrorMessage)
        {
            int nID = -1;
            strErrorMessage = "";

            // 조회 
            string strPropertyName = "";
            Dictionary<string, int> dicTeamOption = null;

            if (option == TeamOption.JobLevel)
            {
                dicTeamOption = m_dicJobLevel;
                strPropertyName = "JobLevel";
            }
            else if (option == TeamOption.JobPosition)
            {
                dicTeamOption = m_dicJobPosition;
                strPropertyName = "JobPosition";
            }
            else if (option == TeamOption.Status)
            {
                dicTeamOption = m_dicStatus;
                strPropertyName = "Status";
            }
            else
            {
                strErrorMessage = "TeamOption 제대로 된 값이 아닙니다.";
                return nID;
            }

            if (dicTeamOption.ContainsKey(strPropertyValue) == false)
            {   // 없으면 추가
                int nPropertyID = GetMaxPropertyID(strPropertyName, out strErrorMessage);
                if (nPropertyID == -1)
                    return nPropertyID;

                int nTeamOptionID = m_dataManager.GetSelectManager().GetMaxID(Options.TableName, out strErrorMessage);

                Options data = new Options();
                data.ID = nTeamOptionID;
                data.PropertyID = nPropertyID;
                data.PropertyName = strPropertyName;
                data.PropertyValue = strPropertyValue;

                if (m_dataManager.GetCreateManager().AddOptions(data, out strErrorMessage) == false)
                    return nID;

                LoadTeamOptions();
                nID = nPropertyID;
            }
            else
            {
                nID = dicTeamOption[strPropertyValue];
            }


            return nID;
        }

        private int GetMaxPropertyID(string strPropertyName, out string strErrorMessage)
        {
            int nID = 0;
            strErrorMessage = "";

            Dictionary<Options.Fields, object> dicConditions = new Dictionary<Options.Fields, object>();
            dicConditions[Options.Fields.PropertyName] = strPropertyName;

            List<Options> options = m_dataManager.GetSelectManager().SelectOptions(dicConditions, "", out strErrorMessage);
            if (options == null)
            {
                nID = -1;
                return nID;
            }

            foreach (Options option in options)
            {
                if (nID < option.PropertyID)
                    nID = option.PropertyID;
            }

            nID = nID + 1;
            return nID;
        }

        private bool SetHRTeamData(List<HrTeamData> hrTeams, out Dictionary<string, HrRegular> dicHrRegulars, out string strErrorMessage)
        {
            strErrorMessage = "";
            dicHrRegulars = new Dictionary<string, HrRegular>();

            if (hrTeams == null)
            {
                strErrorMessage = "hrTeams에 제대로 된 값이 들어있지 않습니다.";
                return false;
            }

            foreach (HrTeamData hrTeam in hrTeams)
            {
                int nLevel = -1;

                if (int.TryParse(hrTeam.ORG_LEVEL, out nLevel) == false)
                {
                    strErrorMessage = "ORG_LEVEL: " + hrTeam.ORG_LEVEL + ", 제대로 된 값이 들어있지 않습니다. ";
                    return false;
                } 

                if (nLevel == 1)
                {   // 루트 Team
                    HrRegular hrRegular = new HrRegular();
                    hrRegular.ParentTeamID = null;
                    hrRegular.TeamName = hrTeam.ORG_NM;
                    hrRegular.ORG_CD = hrTeam.ORG_CD;
                    hrRegular.Path = hrTeam.ORG_NM;

                    dicHrRegulars[hrTeam.ORG_CD] = hrRegular;

                    // 자식 HrRegular 조회 
                    GetChildHRTeam(hrRegular, hrTeams, ref dicHrRegulars);
                }
            }

            return true;
        }

        private void GetChildHRTeam(HrRegular hrRegular, List<HrTeamData> hrTeams, ref Dictionary<string, HrRegular> dicHrRegulars)
        {   // 자식 HrRegular 조회 

            if (dicHrRegulars == null)
                dicHrRegulars = new Dictionary<string, HrRegular>();

            foreach (HrTeamData data in hrTeams)
            {
                if (hrRegular.ORG_CD == data.PRIOR_ORG_CD)
                {
                    HrRegular child = new HrRegular();
                    //child.ParentTeamID = null;
                    child.TeamName = data.ORG_NM;
                    child.ORG_CD = data.ORG_CD;
                    child.Path = hrRegular.Path + "|" + data.ORG_NM;

                    // ORG_CD 중복 예외처리 
                    if (dicHrRegulars.ContainsKey(data.ORG_CD) && data.ORG_NM == "")
                        continue;

                    dicHrRegulars[data.ORG_CD] = child;
                    hrRegular.Children.Add(child);

                    GetChildHRTeam(child, hrTeams, ref dicHrRegulars);
                }
            }
        }

        private int GetBelongID (List<Regular> regulars, string strBelongName)
        {
            int nRetID = -1;

            Regular regularRoot = null;

            regularRoot = GetRootRegular(regulars);
            if (regularRoot == null)
                return -1;

            foreach (Regular regular in regulars)
            {
                if (strBelongName == regular.TeamName)
                {
                    if (regular.ParentTeamID == null || regular.ParentTeamID == regularRoot.ID)
                    {
                        nRetID = regular.ID;
                        break;
                    }
                }
            }

            if (nRetID == -1)
            {   // 새로 추가된 소속
                int nID = GetRegularMaxID(regulars) + 1;
                int nParentTeamID = regularRoot.ID;

                Regular regularBelong = new Regular();
                regularBelong.ID = nID;
                regularBelong.ParentTeamID = nParentTeamID;
                regularBelong.TeamName = strBelongName;

                regulars.Add(regularBelong);
                nRetID = nID;
            }

            return nRetID;
        }

        private int GetRegularMaxID (List<Regular> regulars)
        {
            int nMaxID = 0;

            foreach (Regular regular in regulars)
            {
                if (nMaxID < regular.ID)
                    nMaxID = regular.ID;
            }

            return nMaxID;
        }

        private int GetTeamID(List<Regular> regulars, int nBelongID, string strTeamName)
        {
            int nRetID = -1;

            Regular regularRoot = null;

            regularRoot = GetRootRegular(regulars);
            if (regularRoot == null)
                return -1;

            if (strTeamName == CommonString.REGULAR_ROOT_TEAM)
                nRetID = regularRoot.ID;
            else
            {
                foreach (Regular regular in regulars)
                {
                    if (strTeamName == regular.TeamName && nBelongID == regular.ParentTeamID)
                    {
                        nRetID = regular.ID;
                        break;
                    }
                }
            }

            if (nRetID == -1)
            {   // 새로 추가된 팀
                int nID = GetRegularMaxID(regulars) + 1;
                int nParentTeamID = nBelongID;

                Regular regularBelong = new Regular();
                regularBelong.ID = nID;
                regularBelong.ParentTeamID = nParentTeamID;
                regularBelong.TeamName = strTeamName;

                regulars.Add(regularBelong);
                nRetID = nID;
            }

            return nRetID;
        }

        private Regular GetRootRegular(List<Regular> regulars)
        {
            Regular regular = null;

            foreach (Regular team in regulars)
            {
                if (team.TeamName == CommonString.REGULAR_ROOT && team.ParentTeamID == null)
                {
                    regular = team;
                    break;
                }
            }

            return regular;
        }

        private Regular GetGongjuRegular(List<Regular> regulars)
        {
            Regular regular = null;
            Regular root = GetRootRegular(regulars);

            if (root == null)
                return null;

            foreach (Regular team in regulars)
            {
                if (team.TeamName == CommonString.FACT_GONGJU && team.ParentTeamID == root.ID)
                {
                    regular = team;
                    break;
                }
            }

            return regular;
        }
    }
}
