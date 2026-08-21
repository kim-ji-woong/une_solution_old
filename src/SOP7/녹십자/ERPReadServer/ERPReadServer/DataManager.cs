using dnsDBUtil;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamEditor.Model.Sop.Team;

namespace ERPReadServer
{
    public class DataManager
    {
        private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        private TeamEditor.DAL.DataManager m_teamDataManager = null;
        private SOPManager.DAL.DataManager m_sopDataManager = null;

        Dictionary<string, int> m_dicJobLevels = null;
        Dictionary<string, int> m_dicJobPositions = null;

        public DataManager(TeamEditor.DAL.DataManager teamDataManager, SOPManager.DAL.DataManager sopDataManager)
        {
            m_teamDataManager = teamDataManager;
            m_sopDataManager = sopDataManager;

            initData();
        }

        private void initData()
        {
            string strErrorMessage = "";

            m_dicJobLevels = LoadJobLevel(out strErrorMessage);
            m_dicJobPositions = LoadJobPosition(out strErrorMessage);
        }

        public bool ReflashERPData(DataTable dtTeam, DataTable dtMember, out string strErrorMessage)
        {
            strErrorMessage = "";

            // 읽어온 데이터로 정규조직 및 정규조직 멤버 데이터 형태로 만들기
            List<Regular> regulars_GCC = SetRegulars(dtTeam, out strErrorMessage);
            if (regulars_GCC == null)
            {
                Logger.Instance.Write("[ERROR] SetRegulars is null : " + strErrorMessage);
                return false;
            }

            List<RegularMember> regularMembers_GCC = SetRegularMembers(dtMember, out strErrorMessage);
            if (regularMembers_GCC == null)
            {
                Logger.Instance.Write("[ERROR] SetRegularMembers is null : " + strErrorMessage);
                return false;
            }

            List<Regular> regulars_UNE = m_teamDataManager.GetSelectManager().SelectRegulars(out strErrorMessage);
            if (regulars_UNE == null)
            {
                Logger.Instance.Write("[ERROR] SelectRegulars is null : " + strErrorMessage);
                return false;
            }

            // 녹십자 해당 부서만 추출(협력업체 제외처리) 필요 
            List<Regular> regulars_UNE_GCC = ExportRegularGCC(regulars_UNE, out strErrorMessage);
            if (regulars_UNE_GCC == null)
            {
                Logger.Instance.Write("[ERROR] ExportRegularGCC is null : " + strErrorMessage);
            }


            //string strCondition = "StatusID = 0";
            string strCondition = "";
            List<RegularMember> regularMembers_UNE = m_teamDataManager.GetSelectManager().SelectRegularMembers(strCondition, out strErrorMessage);
            if (regularMembers_UNE == null)
            {
                Logger.Instance.Write("[ERROR] SelectRegularMembers is null : " + strErrorMessage);
                return false;
            }

            // 녹십자 해당 멤버만 추출(협력업체 제외처리) 필요 
            List<RegularMember> regularMembers_UNE_GCC = ExportMemberGCC(regulars_UNE_GCC, regularMembers_UNE, out strErrorMessage);
            if (regularMembers_UNE_GCC == null)
            {
                Logger.Instance.Write("[ERROR] ExportMemberGCC is null : " + strErrorMessage);
            }

            // 비교 후 업데이트
            List<Regular> addRegulars = null;
            List<Regular> updateRegulars = null;
            List<Regular> removeRegulars = null;

            List<RegularMember> addRegularMembers = null;
            List<RegularMember> updateRegularMembers = null;
            List<RegularMember> removeRegularMembers = null;

            if (CompareRegulars(regulars_GCC, regulars_UNE_GCC, out addRegulars, out updateRegulars, out removeRegulars, out strErrorMessage) == false)
            {
                Logger.Instance.Write("[ERROR] CompareRegulars is fail : " + strErrorMessage);
                return false;
            }

            if (CompareRegularMembers(regularMembers_GCC, regularMembers_UNE_GCC, out addRegularMembers, out updateRegularMembers, out removeRegularMembers, out strErrorMessage) == false)
            {
                Logger.Instance.Write("[ERROR] CompareRegularMembers is fail : " + strErrorMessage);
                return false;
            }

            // 정규조직 추가
            foreach (Regular data in addRegulars)
            {
                if (!m_teamDataManager.GetCreateManager().AddRegular(data, out strErrorMessage))
                {
                    Logger.Instance.Write("[ERROR] AddRegular is fail: " + strErrorMessage);
                    continue;
                }
            }

            // 정규조직원 추가
            foreach (RegularMember data in addRegularMembers)
            {
                if (!m_teamDataManager.GetCreateManager().AddRegularMember(data, out strErrorMessage))
                {
                    Logger.Instance.Write("[ERROR] AddRegularMember is fail:" + strErrorMessage);
                    continue;
                }
            }

            // 정규조직 수정
            foreach (Regular data in updateRegulars)
            {
                if (!m_teamDataManager.GetUpdateManager().UpdateRegular(data, out strErrorMessage))
                {
                    Logger.Instance.Write("[ERROR] UpdateRegular is fail : " + strErrorMessage);
                    continue;
                }
            }

            // 정규조직원 수정
            foreach (RegularMember data in updateRegularMembers)
            {
                if (!m_teamDataManager.GetUpdateManager().UpdateRegularMember(data, out strErrorMessage))
                {
                    Logger.Instance.Write("[ERROR] UpdateRegularMember is fail : " + strErrorMessage);
                    continue;
                }
            }

            // 정규조직원 삭제
            foreach (RegularMember data in removeRegularMembers)
            {
                int nID = data.ID;

                // 연동된 계정 삭제
                Dictionary<SOPManager.Model.Sop.Account.User.Fields, object> dicConditions = new Dictionary<SOPManager.Model.Sop.Account.User.Fields, object>();
                dicConditions[SOPManager.Model.Sop.Account.User.Fields.MemberID] = nID;

                List<SOPManager.Model.Sop.Account.User> users = m_sopDataManager.GetSelectManager().SelectUsers(dicConditions, out strErrorMessage);
                if (users != null && users.Count > 0)
                {

                    foreach (SOPManager.Model.Sop.Account.User user in users)
                    {
                        // 해당 계정에 세션 삭제
                        Dictionary<SOPManager.Model.Sop.Account.Session.Fields, object> dicConditions_session = new Dictionary<SOPManager.Model.Sop.Account.Session.Fields, object>();
                        dicConditions_session[SOPManager.Model.Sop.Account.Session.Fields.AccountUserID] = user.ID;

                        List<SOPManager.Model.Sop.Account.Session> sessions = m_sopDataManager.GetSelectManager().SelectSessions(dicConditions_session, out strErrorMessage);
                        if (sessions != null && sessions.Count > 0)
                        {
                            foreach (SOPManager.Model.Sop.Account.Session session in sessions)
                            {
                                if (!m_sopDataManager.GetDeleteManager().DeleteSession(session.ID))
                                {
                                    Logger.Instance.Write("[ERROR] DeleteSession is fail");
                                    //return false;
                                    continue;
                                }
                            }
                        }

                        // 해당 계정의 옵션 삭제
                        Dictionary<SOPManager.Model.Sop.Account.Option.Fields, object> dicConditions_option = new Dictionary<SOPManager.Model.Sop.Account.Option.Fields, object>();
                        dicConditions_option[SOPManager.Model.Sop.Account.Option.Fields.UserID] = user.ID;

                        List<SOPManager.Model.Sop.Account.Option> options = m_sopDataManager.GetSelectManager().SelectOptions(dicConditions_option, out strErrorMessage);
                        if (options != null && options.Count > 0)
                        {
                            foreach (SOPManager.Model.Sop.Account.Option option in options)
                            {
                                if (!m_sopDataManager.GetDeleteManager().DeleteOption(option.ID))
                                {
                                    Logger.Instance.Write("[ERROR] DeleteOption is fail");
                                    //return false;
                                    continue;
                                }
                            }
                        }

                        if (!m_sopDataManager.GetDeleteManager().DeleteUser(user.ID))
                        {
                            Logger.Instance.Write("[ERROR] DeleteUser is fail");
                            //return false;
                            continue;
                        }
                    }
                }

                // 연동된 비상조직 멤버 정보 변경 필요!!
                Dictionary<TemporaryMember.Fields, object> dicConditions_temporaryMember = new Dictionary<TemporaryMember.Fields, object>();
                dicConditions_temporaryMember[TemporaryMember.Fields.RegularMemberID] = nID;

                List<TemporaryMember> temporaryMembers = m_teamDataManager.GetSelectManager().SelectTemporaryMembers(dicConditions_temporaryMember, out strErrorMessage);
                if (temporaryMembers != null && temporaryMembers.Count > 0)
                {
                    foreach (TemporaryMember temporary in temporaryMembers)
                    {
                        temporary.RegularMemberID = null;

                        if (!m_teamDataManager.GetUpdateManager().UpdateTemporaryMember(temporary, out strErrorMessage))
                        {

                            Logger.Instance.Write("[ERROR] UpdateTemporaryMember is fail :" + strErrorMessage);
                            //return false;
                            continue;
                        }
                    }
                }


                if (!m_teamDataManager.GetDeleteManager().DeleteRegularMember(nID, out strErrorMessage))
                {
                    Logger.Instance.Write("[ERROR] DeleteRegularMember is fail : " + strErrorMessage);
                    continue;
                }
            }

            // 정규조직 삭제
            foreach (Regular data in removeRegulars)
            {
                int nID = data.ID;

                if (!m_teamDataManager.GetDeleteManager().DeleteRegular(nID, out strErrorMessage))
                {
                    Logger.Instance.Write("[ERROR] DeleteRegular is fail : " + strErrorMessage);
                    continue;
                }
            }

            return true;
        }

        private List<RegularMember> ExportMemberGCC(List<Regular> gccRegulars, List<RegularMember> uneMembers, out string strResultMessage)
        {
            strResultMessage = "";
            List<RegularMember> gccMembers = null;

            if (gccRegulars == null || uneMembers == null)
            {
                strResultMessage = "Regular, RegularMember 데이터가 잘못 됐습니다.";
                return gccMembers;
            }

            Dictionary<int, Regular> dicGccRegular = MakeDicRegulars(gccRegulars, out strResultMessage);
            if (dicGccRegular == null)
                return gccMembers;

            gccMembers = new List<RegularMember>();

            foreach (RegularMember member in uneMembers)
            {
                if (dicGccRegular.ContainsKey(member.RegularID))
                {
                    gccMembers.Add(member);
                }
            }

            return gccMembers;
        }

        private List<Regular> ExportRegularGCC(List<Regular> uneRegulars, out string strResultMessage)
        {
            List<Regular> regular_GCC = null;
            strResultMessage = "";

            // 녹십자 루트 부서 찾기
            Regular gccRoot = GetRegularGccRoot(uneRegulars, out strResultMessage);
            if (gccRoot == null)
                return regular_GCC;
            /*
            Dictionary<int, Regular> dicRegularUNE = MakeDicRegulars(regulars, out strResultMessage);
            if (dicRegularUNE == null)
                return regular_GCC;
            */

            // 녹십자 관련 부서 찾기
            regular_GCC = new List<Regular>();
            regular_GCC.Add(gccRoot);
            //regular_GCC = GetChildRegular(gccRoot.ID, dicRegularUNE, regular_GCC);
            regular_GCC = GetChildRegular(gccRoot.ID, uneRegulars, regular_GCC);

            return regular_GCC;
        }

        private List<Regular> GetChildRegular(int nParentID, Dictionary<int, Regular> dicRegularUNE, List<Regular> gccRegulars)
        {
            List<Regular> regulars = gccRegulars;

            if (dicRegularUNE.ContainsKey(nParentID))
            {
                Regular regular = dicRegularUNE[nParentID];
                regulars.Add(regular);

                regulars = GetChildRegular(regular.ID, dicRegularUNE, regulars);
            }

            return regulars;
        }

        private List<Regular> GetChildRegular(int nParentID, List<Regular> uneRegulars, List<Regular> gccRegulars)
        {
            List<Regular> regulars = gccRegulars;

            foreach (Regular regular in uneRegulars)
            {
                if (regular.ParentTeamID == nParentID)
                {
                    regulars.Add(regular);
                    //uneRegulars.Remove(regular);

                    regulars = GetChildRegular(regular.ID, uneRegulars, regulars);
                }
            }

            return regulars;
        }

        private Regular GetRegularGccRoot(List<Regular> regulars, out string strResultMessage)
        {
            Regular gccRoot = null;
            strResultMessage = "";

            if (regulars == null)
            {
                strResultMessage = "부서 데이터가 잘못 됐습니다.";
                return gccRoot;
            }

            gccRoot = new Regular();

            foreach (Regular regular in regulars)
            {
                // 녹십자 루트 부서ID >> 1100 
                if (regular.ID == 1100)
                {
                    gccRoot = regular;
                    //regulars.Remove(gccRoot);
                    break;
                }
            }

            return gccRoot;
        }

        private Dictionary<int, Regular> MakeDicRegulars(List<Regular> regulars, out string strResultMessage)
        {
            Dictionary<int, Regular> dicRegulars = null;
            strResultMessage = "";

            if (regulars == null)
            {
                strResultMessage = "부서 데이터가 잘못 됐습니다.";
                return dicRegulars;
            }

            dicRegulars = new Dictionary<int, Regular>();

            foreach (Regular regular in regulars)
            {
                dicRegulars[regular.ID] = regular;
            }

            return dicRegulars;
        }

        


        public List<Regular> SetRegulars(DataTable dtTeam, out string strResultMessage)
        {
            List<Regular> regulars = null;
            strResultMessage = "";

            try
            {
                if (dtTeam == null || dtTeam.Rows.Count == 0)
                {
                    strResultMessage = "부서 관련 DataTable 데이터가 잘못 되어있습니다.";
                    return regulars;
                }

                regulars = new List<Regular>();

                foreach (DataRow dr in dtTeam.Rows)
                {
                    int nID = -1;
                    int nParentTeamID = -1;

                    Regular regular = new Regular();
                    regular.TeamName = dr["ORGTX"].ToString();

                    if (!Int32.TryParse(dr["ORGEH"].ToString(), out nID))
                    {
                        strResultMessage = "ORGTX: " + dr["ORGEH"].ToString() + ", nID 변환 실패";
                        Logger.Instance.Write("[ERROR] SetRegulars is fail : " + strResultMessage);
                        Console.WriteLine("ORGTX: " + dr["ORGEH"].ToString() + ", nID 변환 실패");
                        continue;
                    }
                    else
                        regular.ID = nID;

                    // 최상단 임시 데이터 제외
                    if (nID == 0 && regular.TeamName == "조직구조")
                        continue;

                    if (!Int32.TryParse(dr["UPORGEH"].ToString(), out nParentTeamID))
                    {
                        strResultMessage = "UPORGEH: " + dr["UPORGEH"].ToString() + ", nParentTeamID 변환 실패";
                        Logger.Instance.Write("[ERROR] SetRegulars is fail : " + strResultMessage);
                        Console.WriteLine("UPORGEH: " + dr["UPORGEH"].ToString() + ", nID 변환 실패");
                        continue;
                    }
                    else
                        regular.ParentTeamID = nParentTeamID;

                    // 최상단 임시 데이터 제외
                    if (regular.ParentTeamID == 0)
                        regular.ParentTeamID = null;

                    regulars.Add(regular);
                }
            }
            catch (Exception ex)
            {
                //Logger.Instance.Write("[ERROR] SetRegulars is fail : " + ex.Message);
                strResultMessage = ex.Message;
                regulars = null;
            }

            return regulars;
        }

        public List<RegularMember> SetRegularMembers(DataTable dtMember, out string strResultMessage)
        {
            List<RegularMember> regularMembers = null;
            strResultMessage = "";

            try
            {
                if (dtMember == null || dtMember.Rows.Count == 0)
                {
                    strResultMessage = "사원 관련 DataTable 데이터가 잘못 되어있습니다.";
                    return regularMembers;
                }

                regularMembers = new List<RegularMember>();

                foreach (DataRow dr in dtMember.Rows)
                {
                    int nID = -1;
                    int nRegularID = -1;
                    string strMemberName = "";
                    //string strMemberID = "";
                    //string strOfficePhoneNumber = "";
                    string strPhoneNumber = "";
                    //int nJobLevelID = -1;
                    //int nJobPositionID = -1;
                    string strJobLevel = "";
                    string strJobPosition = "";
                    string strEmail = null;

                    // 사원번호(PERNR) >> ID(PK), MemberID
                    // 사원 이름(ENAME) >> MemberName
                    // 조직 단위(ORGEH) >> RegularID
                    // 직위명(ZTITLE) >> JobPositionID
                    // 직군(PERSK) >> JobLevelID
                    // 그룹웨어ID(ZGWID_NUM) >> Email >> 그룹웨어ID @ gccorp.com
                    // 사무실 전화(ZOFFC_NUM) >> OfficePhoneNumber
                    // 휴대전화(ZHPON_NUM) >> PhoneNumber

                    RegularMember member = new RegularMember();

                    if (!Int32.TryParse(dr["PERNR"].ToString(), out nID))
                    {
                        strResultMessage = "PERNR: " + dr["PERNR"].ToString() + ", nID 변환 실패";
                        Logger.Instance.Write("[ERROR] SetRegularMembers is fail : " + strResultMessage);
                        Console.WriteLine("PERNR: " + dr["PERNR"].ToString() + ", nID 변환 실패");
                        continue;
                    }
                    else
                        member.ID = nID;

                    if (dr["ENAME"].ToString() != null && dr["ENAME"].ToString() != "")
                    {
                        strMemberName = dr["ENAME"].ToString();
                        strMemberName = strMemberName.Replace(" ", "");

                        member.MemberName = strMemberName;
                    }

                    if (!Int32.TryParse(dr["ORGEH"].ToString(), out nRegularID))
                    {
                        strResultMessage = "ORGEH: " + dr["ORGEH"].ToString() + ", nID 변환 실패";
                        Logger.Instance.Write("[ERROR] SetRegularMembers is fail : " + strResultMessage);
                        Console.WriteLine("ORGEH: " + dr["ORGEH"].ToString() + ", nID 변환 실패");
                        continue;
                    }
                    else
                        member.RegularID = nRegularID;

                    // 사번
                    //member.MemberID = dr["ZGWID_NUM"].ToString();
                    member.MemberID = dr["PERNR"].ToString();

                    // 이메일
                    if (dr["ZGWID_NUM"].ToString() != null && dr["ZGWID_NUM"].ToString() != "")
                    {
                        strEmail = dr["ZGWID_NUM"].ToString();
                        strEmail = strEmail.ToLower();
                        strEmail = strEmail + "@gccorp.com";
                    }
                        

                    // 직위, 직군 ID 조회 
                    strJobLevel = ChangeJobLevelGCCtoUNE(dr["PERSK"].ToString());
                    member.JobLevelID = GetJobLevelID(strJobLevel);

                    strJobPosition = dr["ZTITLE"].ToString();
                    member.JobPositionID = GetJobPositionID(strJobPosition);

                    member.OfficePhoneNumber = dr["ZOFFC_NUM"].ToString();

                    // 암호화 
                    strPhoneNumber = dr["ZHPON_NUM"].ToString();
                    member.PhoneNumber = EncryptString(strPhoneNumber);

                    member.StatusID = (int)RegularMember.WorkStatus.Normal;
                    member.Email = strEmail;

                    regularMembers.Add(member);
                }
            }
            catch (Exception ex)
            {
                //Logger.Instance.Write("[ERROR] SetRegularMembers is fail : " + ex.Message);
                strResultMessage = ex.Message;
                regularMembers = null;
            }

            return regularMembers;
        }

        private int? GetJobLevelID(string strJobLevel)
        {
            int? nJobLevelID = null;

            if (strJobLevel == "" || strJobLevel == null)
                return nJobLevelID;

            if (m_dicJobLevels != null && m_dicJobLevels.ContainsKey(strJobLevel))
                nJobLevelID = m_dicJobLevels[strJobLevel];
            else
            {
                string strErrorMessage = "";

                // 다시 조회
                m_dicJobLevels = LoadJobLevel(out strErrorMessage);

                if (m_dicJobLevels == null)
                    return nJobLevelID;
                else if (m_dicJobLevels.ContainsKey(strJobLevel))
                    nJobLevelID = m_dicJobLevels[strJobLevel];
                else
                {
                    // 새로운 직위일 경우 새로 생성하여 id 부여
                    int nPropertyID = GetMaxJobID(m_dicJobLevels);

                    if (nPropertyID == -1)
                        return nJobLevelID;

                    nPropertyID = nPropertyID + 1;
                    int nOptionsID = m_teamDataManager.GetSelectManager().GetMaxID(Options.TableName, out strErrorMessage);

                    Options option = new Options();

                    option.ID = nOptionsID;
                    option.PropertyID = nPropertyID;
                    option.PropertyName = "JobLevel";
                    option.PropertyValue = strJobLevel;

                    if (m_teamDataManager.GetCreateManager().AddOptions(option, out strErrorMessage) == false)
                        return nJobLevelID;

                    m_dicJobLevels[strJobLevel] = nPropertyID;
                    nJobLevelID = nPropertyID;
                }
            }

            return nJobLevelID;
        }

        private int GetMaxJobID(Dictionary<string, int> dicJobs)
        {
            int nID = -1;

            if (dicJobs == null)
            {
                Logger.Instance.Write("[ERROR] GetMaxJobID is fail : dicJobs 가 null");
                return nID;
            }

            foreach(KeyValuePair<string, int> pair in dicJobs)
            {
                int nJobID = pair.Value;

                if (nJobID > nID)
                    nID = nJobID;
            }

            return nID;
        }



        private int? GetJobPositionID(string strJobPosition)
        {
            int? nJobPositionID = null;

            if (strJobPosition == "" || strJobPosition == null)
            {
                Logger.Instance.Write("[ERROR] GetJobPositionID is fail : strJobPosition 가 null");
                return nJobPositionID;
            }

            if (m_dicJobPositions != null && m_dicJobPositions.ContainsKey(strJobPosition))
                nJobPositionID = m_dicJobPositions[strJobPosition];
            else
            {
                string strErrorMessage = "";

                // 다시 조회
                m_dicJobPositions = LoadJobPosition(out strErrorMessage);

                if (m_dicJobPositions == null)
                    return nJobPositionID;
                else if (m_dicJobPositions.ContainsKey(strJobPosition))
                    nJobPositionID = m_dicJobPositions[strJobPosition];
                else
                {
                    // 새로운 직군일 경우 새로 생성하여 id 부여
                    int nPropertyID = GetMaxJobID(m_dicJobPositions);

                    if (nPropertyID == -1)
                        return nJobPositionID;

                    nPropertyID = nPropertyID + 1;
                    int nOptionsID = m_teamDataManager.GetSelectManager().GetMaxID(Options.TableName, out strErrorMessage);

                    Options option = new Options();

                    option.ID = nOptionsID;
                    option.PropertyID = nPropertyID;
                    option.PropertyName = "JobPosition";
                    option.PropertyValue = strJobPosition;

                    if (m_teamDataManager.GetCreateManager().AddOptions(option, out strErrorMessage) == false)
                        return nJobPositionID;

                    m_dicJobPositions[strJobPosition] = nPropertyID;
                    nJobPositionID = nPropertyID;
                }
            }

            return nJobPositionID;
        }

        private Dictionary<string, int> LoadJobLevel(out string strErrorMessage)
        {
            Dictionary<string, int> dicJobLevels = null;

            string strSQL = " PropertyName = 'JobLevel'";
            List<Options> options = m_teamDataManager.GetSelectManager().SelectOptions(strSQL, out strErrorMessage);

            if (options == null)
            {
                Logger.Instance.Write("[ERROR] LoadJobLevel is fail : " + strErrorMessage);
                return dicJobLevels;
            }
            
            dicJobLevels = new Dictionary<string, int>();

            foreach (Options option in options)
            {
                dicJobLevels[option.PropertyValue] = option.PropertyID;
            }

            return dicJobLevels;
        }

        private Dictionary<string, int> LoadJobPosition(out string strErrorMessage)
        {
            Dictionary<string, int> dicJobPositions = null;

            string strSQL = " PropertyName = 'JobPosition'";
            List<Options> options = m_teamDataManager.GetSelectManager().SelectOptions(strSQL, out strErrorMessage);

            if (options == null)
            {
                Logger.Instance.Write("[ERROR] LoadJobPosition is fail : " + strErrorMessage);
                return dicJobPositions;
            }
            
            dicJobPositions = new Dictionary<string, int>();

            foreach (Options option in options)
            {
                dicJobPositions[option.PropertyValue] = option.PropertyID;
            }

            return dicJobPositions;
        }

        public bool CompareRegulars(List<Regular> regulars_GCC, List<Regular> regulars_UNE, out List<Regular> addRegulars, out List<Regular> updateRegulars, out List<Regular> removeRegulars, out string strErrorMessage)
        {
            addRegulars = new List<Regular>();
            updateRegulars = new List<Regular>();
            removeRegulars = regulars_UNE;
            strErrorMessage = "";

            try
            {
                if (regulars_GCC == null || regulars_UNE == null)
                {
                    strErrorMessage = "비교할 Regular 데이터가 잘못 되었습니다.";
                    return false;
                }

                foreach (Regular regular in regulars_GCC)
                {
                    bool bCheck = false;

                    foreach (Regular data in regulars_UNE)
                    {
                        if (regular.ID == data.ID)
                        {
                            bCheck = true;

                            if (regular.TeamName != data.TeamName ||
                                regular.ParentTeamID != data.ParentTeamID)
                            {
                                updateRegulars.Add(regular);
                            }

                            removeRegulars.Remove(data);
                            break;
                        }
                    }

                    if (!bCheck)
                        addRegulars.Add(regular);
                }
            }
            catch (Exception ex)
            {
                strErrorMessage = ex.Message;
                return false;
            }

            return true;
        }

        public bool CompareRegularMembers(List<RegularMember> regularMembers_GCC, List<RegularMember> regularMembers_UNE, out List<RegularMember> addRegularMembers, out List<RegularMember> updateRegularMembers, out List<RegularMember> removeRegularMembers, out string strErrorMessage)
        {
            addRegularMembers = new List<RegularMember>();
            updateRegularMembers = new List<RegularMember>();
            removeRegularMembers = regularMembers_UNE;
            strErrorMessage = "";

            try
            {
                if (regularMembers_GCC == null || regularMembers_UNE == null)
                {
                    strErrorMessage = "비교할 RegularMember 데이터가 잘못 되었습니다.";
                    return false;
                }

                foreach (RegularMember member in regularMembers_GCC)
                {
                    bool bCheck = false;

                    foreach (RegularMember data in regularMembers_UNE)
                    {
                        if (member.ID == data.ID)
                        {
                            bCheck = true;

                            if (member.MemberName != data.MemberName ||
                                member.MemberID != data.MemberID ||
                                member.JobLevelID != data.JobLevelID ||
                                member.JobPositionID != data.JobPositionID ||
                                member.OfficePhoneNumber != data.OfficePhoneNumber ||
                                member.PhoneNumber != data.PhoneNumber ||
                                member.RegularID != data.RegularID ||
                                member.StatusID != data.StatusID ||
                                member.Email != data.Email)
                            {
                                updateRegularMembers.Add(member);
                            }

                            removeRegularMembers.Remove(data);
                            break;
                        }
                    }

                    if (bCheck == false)
                        addRegularMembers.Add(member);
                }

            }
            catch (Exception ex)
            {
                strErrorMessage = ex.Message;
                return false;
            }

            return true;
        }

        private string ChangeJobLevelGCCtoUNE(string gccJobLevel)
        {
            string jobLevel = "";

            if (gccJobLevel == "E3")
                jobLevel = "영업직";
            else if (gccJobLevel == "M1")
                jobLevel = "기술직";
            else if (gccJobLevel == "E6")
                jobLevel = "연구개발직(목암)";
            else if (gccJobLevel == "E1")
                jobLevel = "관리직";
            else if (gccJobLevel == "E4")
                jobLevel = "연구개발직";
            else if (gccJobLevel == "E2")
                jobLevel = "생산직";
            else if (gccJobLevel == "E0")
                jobLevel = "임원";

            return jobLevel;
        }

        public static string EncryptString(string str)
        {
            return AES256Cipher.AES_encrypt(str, key);
        }

        public static string DecryptString(string str)
        {
            return AES256Cipher.AES_decrypt(str, key);
        }
    }
}
