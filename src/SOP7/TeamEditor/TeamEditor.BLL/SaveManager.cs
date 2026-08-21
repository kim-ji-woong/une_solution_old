using SDMS.Model.Sensor;
using SOPManager.Model.Sop.Account;
using System;
using System.Collections.Generic;
using System.Text;
using TeamEditor.BLL.Models.Request;
using TeamEditor.BLL.Models.Response;
using TeamEditor.BLL.Rollback;
using TeamEditor.IDAL;
using TeamEditor.Model.Sop.Team;

namespace TeamEditor.BLL
{
    public class SaveManager
    {
        private IDataManager m_dataManager = null;
        private SOPManager.IDAL.IDataManager m_sopDataManager = null;
        private SDMS.IDAL.IDataManager m_sdmsDataManager = null;

        public SaveManager(IDataManager dataManager, SOPManager.IDAL.IDataManager sopDataManager, SDMS.IDAL.IDataManager sdmsDataManager)
        {
            m_dataManager = dataManager;
            m_sopDataManager = sopDataManager;
            m_sdmsDataManager = sdmsDataManager;
        }

        public ResponseCommand Save(RequestCommand cmd)
        {
            cmd.DataManager = m_dataManager;

            if (cmd.Key == "AddRegularTeam")
            {
                ResponseCommandAddRegularTeam res = new ResponseCommandAddRegularTeam();

                RequestCommandAddRegularTeam req = cmd as RequestCommandAddRegularTeam;
                if (req == null)
                {
                    res.StrErrorMessage = "매개 변수가 잘못됐습니다.";
                    return null;
                }

                res.nOrgID = req.ID;
                cmd.SaveDB();
                res.nNewID = req.ID;

                return res;
            }
            else if (cmd.Key == "RemoveRegularTeam")
            {
                ResponseCommand res = new ResponseCommand();

                RequestCommandRemoveRegularTeam req = cmd as RequestCommandRemoveRegularTeam;
                if (req == null)
                {
                    res.StrErrorMessage = "매개 변수가 잘못됐습니다.";
                    return null;
                }

                cmd.SaveDB();

                return res;
            }
            else if (cmd.Key == "ChangeRegularTeamInfo")
            {
                ResponseCommand res = new ResponseCommand();

                RequestCommandChangeRegularTeamInfo req = cmd as RequestCommandChangeRegularTeamInfo;
                if (req == null)
                {
                    res.StrErrorMessage = "매개 변수가 잘못됐습니다.";
                    return null;
                }

                cmd.SaveDB();

                return res;
            }
            else if (cmd.Key == "ChangeRegularMemberInfo")
            {
                ResponseCommandChangeRegularMemberInfo res = new ResponseCommandChangeRegularMemberInfo();
                RequestCommandChangeRegularMemberInfo req = cmd as RequestCommandChangeRegularMemberInfo;
                if (req == null)
                {
                    res.StrErrorMessage = "매개 변수가 잘못됐습니다.";
                    return null;
                }
                
                cmd.SaveDB();
                res.nNewID = req.ID;

                return res;
            }
            else if (cmd.Key == "RemoveRegularMember")
            {
                ResponseCommandRemoveRegularMember res = new ResponseCommandRemoveRegularMember();
                RequestCommandRemoveRegularMember req = cmd as RequestCommandRemoveRegularMember;
                if (req == null)
                {
                    res.StrErrorMessage = "매개 변수가 잘못됐습니다.";
                    return null;
                }

                cmd.SaveDB();
                res.nNewID = req.ID;

                return res;
            }
            

            return null;
        }

        public ResponseUpdateRegularMember UpdateRegularMember(RequestUpdateRegularMember req)
        {
            string strErrorMessage = null;
            ResponseUpdateRegularMember res = new ResponseUpdateRegularMember();
            if (req.Member.ID > 0)
            {
                if (!m_dataManager.GetUpdateManager().UpdateRegularMember(req.Member, out strErrorMessage))
                {
                    res.Message = strErrorMessage;
                    res.Success = false;
                    return res;
                }
            }
            else
            {
                int nID = m_dataManager.GetSelectManager().GetMaxID(RegularMember.GetTableName(), out strErrorMessage);
                if (nID == -1)
                {
                    res.Message = strErrorMessage;
                    res.Success = false;
                    return res;
                }

                req.Member.ID = nID;
                RegularMember createMember = m_dataManager.GetCreateManager().AddRegularMember(req.Member, out strErrorMessage);
                if (createMember == null)
                {
                    res.Message = strErrorMessage;
                    res.Success = false;
                    return res;
                }

                res.NewID = nID;
            }

            res.Success = true;
            return res;
        }

        public MessageResult RemoveRegularMembers(RequestRemoveRegularMember req)
        {
            string strErrorMessage = null;
            MessageResult res = new MessageResult();

            RollbackManager rollback = new RollbackManager();
            // RegularMember 삭제
            if (RemoveRegularMember(req.Members, rollback, out strErrorMessage) == false)
            {
                rollback.Rollback(m_sdmsDataManager, m_dataManager, m_sopDataManager);
                res.Success = false;
                res.Message = strErrorMessage;
                return res;
            }

            res.Success = true;
            return res;
        }

        public ResponseUpdateTemporaryMember UpdateTemporaryMember(RequestUpdateTemporaryMember req)
        {
            string strErrorMessage = null;
            ResponseUpdateTemporaryMember res = new ResponseUpdateTemporaryMember();

            TemporaryMember member = new TemporaryMember();
            member.ID = req.TemporaryMemberInfo.ID;
            member.DisplaySOPName = req.TemporaryMemberInfo.DisplaySOPName;
            member.TeamID = req.TemporaryMemberInfo.Temporary.ID;
            member.IsNormal = req.TemporaryMemberInfo.IsNormal;
            member.RegularID = req.TemporaryMemberInfo.Regular?.ID;
            member.RegularMemberID = req.TemporaryMemberInfo.RegularMember?.ID;
            member.Role = req.TemporaryMemberInfo.Role;

            if (req.TemporaryMemberInfo.ID > 0)
            {
                if (!m_dataManager.GetUpdateManager().UpdateTemporaryMember(member, out strErrorMessage))
                {
                    res.Message = strErrorMessage;
                    res.Success = false;
                    return res;
                }
            }
            else
            {
                int nID = m_dataManager.GetSelectManager().GetMaxID(TemporaryMember.GetTableName(), out strErrorMessage);
                if (nID == -1)
                {
                    res.Message = strErrorMessage;
                    res.Success = false;
                    return res;
                }

                member.ID = nID;
                
                if (!m_dataManager.GetCreateManager().AddTemporaryMember(member, out strErrorMessage))
                {
                    res.Message = strErrorMessage;
                    res.Success = false;
                    return res;
                }

                res.NewID = nID;
            }

            res.Success = true;
            return res;
        }

        public MessageResult RemoveTemporaryMembers(RequestRemoveTemporaryMember req)
        {
            string strErrorMessage = null;
            MessageResult res = new MessageResult();

            if (req.Members == null || req.Members.Count == 0)
                return res;

            foreach (TemporaryMember item in req.Members)
            {
                if (!m_dataManager.GetDeleteManager().DeleteTemporaryMember(item.ID, out strErrorMessage))
                {
                    res.Success = false;
                    res.Message = strErrorMessage;
                    return res;
                }
            }

            res.Success = true;
            return res;
        }

        public ResponseUpdateRegularTeam UpdateRegularTeam(RequestUpdateRegularTeam req)
        {
            string strErrorMessage = null;
            ResponseUpdateRegularTeam res = new ResponseUpdateRegularTeam();

            if (req.RegularTeam.ID > 0)
            {
                if (!m_dataManager.GetUpdateManager().UpdateRegular(req.RegularTeam, out strErrorMessage))
                {
                    res.Message = strErrorMessage;
                    res.Success = false;
                    return res;
                }
            }
            else
            {
                int nID = m_dataManager.GetSelectManager().GetMaxID(Regular.GetTableName(), out strErrorMessage);
                if (nID == -1)
                {
                    res.Message = strErrorMessage;
                    res.Success = false;
                    return res;
                }

                req.RegularTeam.ID = nID;
                if (!m_dataManager.GetCreateManager().AddRegular(req.RegularTeam, out strErrorMessage))
                {
                    res.Message = strErrorMessage;
                    res.Success = false;
                    return res;
                }

                res.NewID = nID;
            }

            res.Success = true;
            return res;
        }

        public MessageResult RemoveRegularTeams(RequestRemoveRegularTeam req)
        {
            string strErrorMessage = null;
            MessageResult res = new MessageResult();

            if (req.TeamIDs == null || req.TeamIDs.Count == 0)
            {
                res.Message = "삭제할 팀이 없습니다";
                res.Success = false;
                return res;
            }

            RollbackManager rollback = new RollbackManager();

            string strTeamIDs = string.Join(",", req.TeamIDs);
            string strConditions = string.Format("{0} in ({1})", RegularMember.Fields.RegularID, strTeamIDs);
            List<RegularMember> deleteMembers = m_dataManager.GetSelectManager().SelectRegularMembers(strConditions, out strErrorMessage);
            // RegularMember 삭제
            if (deleteMembers != null && deleteMembers.Count > 0)
            {
                if (RemoveRegularMember(deleteMembers, rollback, out strErrorMessage) == false)
                {
                    rollback.Rollback(m_sdmsDataManager, m_dataManager, m_sopDataManager);
                    res.Message = strErrorMessage;
                    res.Success = false;
                    return res;
                } 
            }

            strConditions = string.Format("{0} in ({1})", Regular.Fields.ID, strTeamIDs);
            List<Regular> deleteRegulars = m_dataManager.GetSelectManager().SelectRegulars(null, strConditions, out strErrorMessage);

            if (deleteRegulars != null && deleteRegulars.Count > 0)
            {
                // Regular 삭제
                if (RemoveRegular(deleteRegulars, rollback, out strErrorMessage) == false)
                {
                    rollback.Rollback(m_sdmsDataManager, m_dataManager, m_sopDataManager);
                    res.Message = strErrorMessage;
                    res.Success = false;
                    return res;
                } 
            }

            res.Success = true;
            return res;
        }

        public MessageResult RemoveTemporaryTeams(RequestRemoveTemporaryTeam req)
        {
            string strErrorMessage = null;
            MessageResult res = new MessageResult();

            if (req.TeamIDs == null || req.TeamIDs.Count == 0)
            {
                res.Message = "삭제할 팀이 없습니다";
                res.Success = false;
                return res;
            }

            string strTeamIDs = string.Join(",", req.TeamIDs);
            string strConditions = string.Format("{0} in ({1})", TemporaryMember.Fields.TeamID, strTeamIDs);
            List<TemporaryMember> deleteMembers = m_dataManager.GetSelectManager().SelectTemporaryMembers(null, strConditions, out strErrorMessage);            
            if (deleteMembers != null && deleteMembers.Count > 0)
            {
                foreach (TemporaryMember item in deleteMembers)
                {
                    if (!m_dataManager.GetDeleteManager().DeleteTemporaryMember(item.ID, out strErrorMessage))
                    {
                        res.Success = false;
                        res.Message = strErrorMessage;
                        return res;
                    }
                }
            }

            foreach (int teamID in req.TeamIDs)
            {
                if (!m_dataManager.GetDeleteManager().DeleteTemporary(teamID, out strErrorMessage))
                {
                    res.Message = strErrorMessage;
                    res.Success = false;
                    return res;
                } 
            }

            res.Success = true;
            return res;
        }

        public ResponseUpdateTemporaryTeam UpdateTemporaryTeam(RequestUpdateTemporaryTeam req)
        {
            string strErrorMessage = null;
            ResponseUpdateTemporaryTeam res = new ResponseUpdateTemporaryTeam();

            if (req.TemporaryTeam.ID > 0)
            {
                if (!m_dataManager.GetUpdateManager().UpdateTemporary(req.TemporaryTeam, out strErrorMessage))
                {
                    res.Message = strErrorMessage;
                    res.Success = false;
                    return res;
                }
            }
            else
            {
                int nID = m_dataManager.GetSelectManager().GetMaxID(Temporary.GetTableName(), out strErrorMessage);
                if (nID == -1)
                {
                    res.Message = strErrorMessage;
                    res.Success = false;
                    return res;
                }

                req.TemporaryTeam.ID = nID;
                if (!m_dataManager.GetCreateManager().AddTemporary(req.TemporaryTeam, out strErrorMessage))
                {
                    res.Message = strErrorMessage;
                    res.Success = false;
                    return res;
                }

                res.NewID = nID;
            }

            res.Success = true;
            return res;
        }

        /// <summary>
        /// <para>인자 dicPathRegularMember는 업데이트 할 Regular, RegularMember 정보</para>
        /// <para>인자 dicIDRegulars, dicIDRegularMembers는 dicPathRegularMember과 비교할 데이터, 값이 NULL 이면 현재 DB의 Regular, RegularMember와 비교</para>
        /// 예) 협력업체 정보를 제외하고 업데이트를 진행하고 싶다면, 협력업체의 Regular, RegularMember를 제외하고 dicIDRegulars, dicIDRegularMembers 값을 넣고 실행
        /// </summary>
        /// <param name="dicPathRegularMember">KEY: Regular의 Path, Value: Regular의 RegularMember 리스트</param>
        /// <param name="strErrorMessage">오류 결과 메시지</param>
        /// <param name="dicIDRegulars">KEY: Regular의 ID, Value: 현재 DB의 Regular 데이터</param>
        /// <param name="dicIDRegularMembers">KEY: RegularMember의 ID, Value: 현재 DB의 RegularMember 데이터</param>
        /// <returns></returns>
        public bool UpdateRegularMemberData(Dictionary<string, List<RegularMember>> dicPathRegularMember, out string strErrorMessage, Dictionary<int, Regular> dicIDRegulars = null, Dictionary<int, RegularMember> dicIDRegularMembers = null)
        {
            strErrorMessage = "";
            Dictionary<string, Regular> dicPathRegulars = new Dictionary<string, Regular>();
            Dictionary<string, RegularMember> dicRegularMembers = new Dictionary<string, RegularMember>();

            if (dicPathRegularMember == null)
            {
                strErrorMessage = "dicPathRegularMember 인자가 제대로 된 값이 아닙니다.";
                return false;
            }

            

            
            Dictionary<string, Regular> dicPathRegulars_Current = null;
            Dictionary<string, RegularMember> dicRegularMembers_Current = null;

            // 비교 데이터 형식 맞추기 -----------------------------------------------------------------------------
            if (dicIDRegulars == null && dicIDRegularMembers == null)
            {   // 현재 DB의 Regular, RegularMember 불러오기
                if (LoadCurrentRegularMember(out dicPathRegulars_Current, out dicRegularMembers_Current, out dicIDRegularMembers, out strErrorMessage) == false)
                    return false;
            }
            else
            {
                if (SetCurrentRegularMember(dicIDRegulars, dicIDRegularMembers, out dicPathRegulars_Current, out dicRegularMembers_Current, out strErrorMessage) == false)
                    return false;
            }
            


            // 외부 조직 데이터 비교 형식에 맞춰 정렬 -----------------------------------------------------------------------------
            int nRegularNewID = -1;

            foreach (KeyValuePair<string, List<RegularMember>> pair in dicPathRegularMember)
            {
                string strTeamPath = pair.Key;
                List<RegularMember> members = pair.Value;

                Regular regular = null;

                if (dicPathRegulars_Current.ContainsKey(strTeamPath))
                {   // 기존 있던 팀이라면
                    regular = dicPathRegulars_Current[strTeamPath];
                    dicPathRegulars[strTeamPath] = regular;
                }
                else if (dicPathRegulars.ContainsKey(strTeamPath))
                {   // 새로 추가된 팀에 포함되어 있다면
                    //continue;
                    regular = dicPathRegulars[strTeamPath];
                }
                else
                {   // 새로 추가될 팀
                    regular = new Regular();
                    regular.ID = nRegularNewID;

                    nRegularNewID = nRegularNewID - 1;

                    int nIdx = strTeamPath.LastIndexOf('|');
                    if (nIdx >= 0)
                    {
                        string strParentName = strTeamPath.Substring(0, nIdx);

                        nIdx++;
                        string strTeamName = strTeamPath.Substring(nIdx);
                        regular.TeamName = strTeamName;

                        int? nParentTeamID = GetParentID(strParentName, dicPathRegulars_Current, ref dicPathRegulars, ref nRegularNewID);
                        if (nParentTeamID == null)
                            return false;

                        regular.ParentTeamID = nParentTeamID;
                    }
                    else
                    {
                        regular.TeamName = strTeamPath;
                        regular.ParentTeamID = null;
                    }

                    dicPathRegulars[strTeamPath] = regular;
                }

                foreach (RegularMember member in members)
                {
                    if (dicRegularMembers_Current.ContainsKey(member.MemberID))
                    {
                        RegularMember data = dicRegularMembers_Current[member.MemberID];
                        member.ID = data.ID;
                    }
                    else
                    {
                        member.ID = -1;
                    }

                    member.RegularID = regular.ID;
                    dicRegularMembers[member.MemberID] = member;
                }
            }



            // Regular 비교 -----------------------------------------------------------------------------
            Dictionary<string, Regular> dicAddRegulars = new Dictionary<string, Regular>();
            Dictionary<string, Regular> dicRemoveRegulars = CloenDicRegularData(dicPathRegulars_Current, out strErrorMessage);

            if (dicRemoveRegulars == null)
                return false;

            foreach (KeyValuePair<string, Regular> pair in dicPathRegulars)
            {
                string strPath = pair.Key;
                Regular regular = pair.Value;

                if (regular.ID < 0)
                {   // 새로 추가된 Regular
                    dicAddRegulars[strPath] = regular;
                }
                else if (dicPathRegulars_Current.ContainsKey(strPath))
                {   // 존재하는 Regular는 제외
                    dicRemoveRegulars.Remove(strPath);
                }
            }





            // RegularMember 비교 -----------------------------------------------------------------------------
            Dictionary<string, RegularMember> dicAddRegularMembers = new Dictionary<string, RegularMember>();
            Dictionary<string, RegularMember> dicModifiRegularMembers = new Dictionary<string, RegularMember>();
            //Dictionary<string, RegularMember> dicRemoveRegularMembers = CloenDicMemberData(dicRegularMembers_Current, out strErrorMessage);

            Dictionary<int, RegularMember> dicRemoveRegularMembers = CloneRegularMemberData(dicIDRegularMembers, out strErrorMessage);

            if (dicRemoveRegularMembers == null)
                return false;

            foreach (KeyValuePair<string, RegularMember> pair in dicRegularMembers)
            {
                string strMemberID = pair.Key;
                RegularMember member = pair.Value;

                if (dicRegularMembers_Current.ContainsKey(strMemberID))
                {   // 존재하는 RegularMember

                    // 삭제 목록에서 제외
                    dicRemoveRegularMembers.Remove(member.ID);

                    RegularMember data = dicRegularMembers_Current[strMemberID];

                    // 비교 후 업데이트 여부 확인
                    if (member.MemberName != data.MemberName ||
                        member.Email != data.Email ||
                        member.JobLevelID != data.JobLevelID ||
                        member.JobPositionID != data.JobPositionID ||
                        member.RegularID != data.RegularID ||
                        member.StatusID != data.StatusID ||
                        member.OfficePhoneNumber != data.OfficePhoneNumber ||
                        member.PhoneNumber != data.PhoneNumber)
                    {
                        dicModifiRegularMembers[strMemberID] = member;
                    }
                }
                else if (member.ID < 0)
                {   // 새로 추가된 RegularMember
                    dicAddRegularMembers[strMemberID] = member;
                }

            }




            // 변경된 데이터 DB 적용 -----------------------------------------------------------------------------------
            RollbackManager rollback = new RollbackManager();
            Dictionary<int, int> dicChangeRegularID = null;

            // Regular 추가 작업 및 Regular ID 변경 데이터 저장
            if (AddRegular(dicAddRegulars.Values, out dicChangeRegularID, rollback, out strErrorMessage) == false)
            {
                rollback.Rollback(m_sdmsDataManager, m_dataManager, m_sopDataManager);
                return false;
            }

            // RegularMember 추가
            if (AddRegularMember(dicAddRegularMembers.Values, dicChangeRegularID, rollback, out strErrorMessage) == false)
            {
                rollback.Rollback(m_sdmsDataManager, m_dataManager, m_sopDataManager);
                return false;
            }

            // RegularMember 업데이트
            if (UpdateRegularMember(dicModifiRegularMembers.Values, dicChangeRegularID, rollback, out strErrorMessage) == false)
            {
                rollback.Rollback(m_sdmsDataManager, m_dataManager, m_sopDataManager);
                return false;
            }

            // RegularMember 삭제
            if(RemoveRegularMember(dicRemoveRegularMembers.Values, rollback, out strErrorMessage) == false)
            {
                rollback.Rollback(m_sdmsDataManager, m_dataManager, m_sopDataManager);
                return false;
            }

            // Regular 삭제
            if (RemoveRegular(dicRemoveRegulars.Values, rollback, out strErrorMessage) == false)
            {
                rollback.Rollback(m_sdmsDataManager, m_dataManager, m_sopDataManager);
                return false;
            }

            return true;
        }

        private bool RemoveRegular(ICollection<Regular> regulars, RollbackManager rollback, out string strErrorMessage)
        {
            strErrorMessage = "";

            foreach (Regular regular in regulars)
            {
                if (RemoveFacilityManagers((int)FacilityManager.MemberTypes.RegularTeam, regular.ID, rollback) == false)
                    return false;

                if (RemoveTemporaryMembers(regular.ID, null, rollback) == false)
                    return false;
            }

            TeamRollbackData rollbackData = new TeamRollbackData();
            rollback.AddData(rollbackData);

            List<Regular> rollbackTeams = new List<Regular>();
            rollbackData.SetInsertRegulars(rollbackTeams);

            foreach (Regular regular in regulars)
            {
                if (m_dataManager.GetDeleteManager().DeleteRegular(regular.ID, out strErrorMessage) == false)
                    return false;
                else
                    rollbackTeams.Add(regular);

            }

            return true;
        }

        private bool RemoveRegularMember(ICollection<RegularMember> regularMembers, RollbackManager rollback, out string strErrorMessage)
        {
            strErrorMessage = "";

            foreach (RegularMember member in regularMembers)
            {
                if(RemoveFacilityManagers((int)FacilityManager.MemberTypes.RegularMember, member.ID, rollback) == false)
                    return false;

                if (RemoveTemporaryMembers(null, member.ID, rollback) == false)
                    return false;

                if (RemoveAccountUser(member.ID, rollback) == false)
                    return false;
            }

            TeamRollbackData rollbackData = new TeamRollbackData();
            rollback.AddData(rollbackData);

            List<RegularMember> rollbackMembers = new List<RegularMember>();
            rollbackData.SetInsertRegularMembers(rollbackMembers);

            foreach (RegularMember member in regularMembers)
            {
                if (m_dataManager.GetDeleteManager().DeleteRegularMember(member.ID, out strErrorMessage) == false)
                    return false;
                else
                    rollbackMembers.Add(member);
            }

            return true;
        }

        private bool RemoveAccountUser(int nUserID, RollbackManager rollback)
        {
            string strCondition = "";
            bool isNullable;

            // 연동 계정 옵션 제거
            strCondition = string.Format("{0} in ({1})", Option.GetFieldName(Option.Fields.UserID, out isNullable), nUserID);

            string strErrorMessage;
            List<Option> options = m_sopDataManager.GetSelectManager().SelectOptions(null, strCondition, null, out strErrorMessage);

            if (options == null)
                return false;

            TeamRollbackData rollbackData = new TeamRollbackData();
            rollback.AddData(rollbackData);

            List<Option> rollbackOptions = new List<Option>();
            rollbackData.SetInsertOptions(rollbackOptions);

            foreach (Option option in options)
            {
                if (m_sopDataManager.GetDeleteManager().DeleteOption(option.ID) == false)
                    return false;
                else
                    rollbackOptions.Add(option);
            }


            // 연동 계정 세션 제거
            strCondition = string.Format("{0} in ({1})", Session.GetFieldName(Session.Fields.AccountUserID, out isNullable), nUserID);

            List<Session> sessions = m_sopDataManager.GetSelectManager().SelectSessions(null, strCondition, null, out strErrorMessage);

            if (sessions == null)
                return false;

            rollbackData = new TeamRollbackData();
            rollback.AddData(rollbackData);

            List<Session> rollbackSessions = new List<Session>();
            rollbackData.SetInsertSessions(rollbackSessions);

            foreach (Session session in sessions)
            {
                if (m_sopDataManager.GetDeleteManager().DeleteSession(session.ID) == false)
                    return false;
                else
                    rollbackSessions.Add(session);
            }


            // 연동 계정 제거
            strCondition = string.Format("{0} in ({1})", User.GetFieldName(User.Fields.MemberID, out isNullable), nUserID);

            List<User> users = m_sopDataManager.GetSelectManager().SelectUsers(strCondition, out strErrorMessage);

            if (users == null)
                return false;

            rollbackData = new TeamRollbackData();
            rollback.AddData(rollbackData);

            List<User> rollbackUsers = new List<User>();
            rollbackData.SetInsertUsers(rollbackUsers);

            foreach (User user in users)
            {
                if (m_sopDataManager.GetDeleteManager().DeleteUser(user.ID) == false)
                    return false;
                else
                    rollbackUsers.Add(user);
            }

            return true;
        }

        private bool RemoveTemporaryMembers(int? nRegularTeamID, int? nRegularMemberID, RollbackManager rollback)
        {
            string strCondition = "";
            bool isNullable;

            if (nRegularTeamID != null)
            {
                strCondition = string.Format("{0} in ({1})", TemporaryMember.GetFieldName(TemporaryMember.Fields.RegularID, out isNullable), nRegularTeamID);
            }

            if (nRegularMemberID != null)
            {
                if (strCondition.Length == 0)
                    strCondition = string.Format("{0} in ({1})", TemporaryMember.GetFieldName(TemporaryMember.Fields.RegularMemberID, out isNullable), nRegularMemberID);
                else
                    strCondition += string.Format(" and {0} in ({1})", TemporaryMember.GetFieldName(TemporaryMember.Fields.RegularMemberID, out isNullable), nRegularMemberID);
            }

            if (strCondition.Length == 0)
                return true;

            string strErrorMessage;
            List<TemporaryMember> members = m_dataManager.GetSelectManager().SelectTemporaryMembers(null, strCondition, out strErrorMessage);

            if (members == null)
                return false;

            foreach (TemporaryMember member in members)
            {
                if (RemoveFacilityManagers((int)FacilityManager.MemberTypes.TemporaryMember, member.ID, rollback) == false)
                    return false;
            }

            TeamRollbackData rollbackData = new TeamRollbackData();
            rollback.AddData(rollbackData);

            List<TemporaryMember> rollbackMembers = new List<TemporaryMember>();
            rollbackData.SetInsertTemporaryMembers(rollbackMembers);

            foreach (TemporaryMember member in members)
            {
                if (m_dataManager.GetDeleteManager().DeleteTemporaryMember(member.ID, out strErrorMessage) == false)
                    return false;
                else
                    rollbackMembers.Add(member);
            }

            return true;
        }

        private bool RemoveFacilityManagers(int memberType, int memberID, RollbackManager rollback)
        {
            bool isNullable;
            string strErrorMessage;

            string strCondition = string.Format("{0} = {1} and {2} in ({3})",
                FacilityManager.GetFieldName(FacilityManager.Fields.MemberType, out isNullable),
                memberType,
                FacilityManager.GetFieldName(FacilityManager.Fields.MemberID, out isNullable),
                memberID);

            List<FacilityManager> managers = m_sdmsDataManager.GetSelectManager().SelectFacilityManagers(null, strCondition, out strErrorMessage);

            if (managers == null)
                return false;

            TeamRollbackData rollbackData = new TeamRollbackData();
            rollback.AddData(rollbackData);

            List<FacilityManager> rollbackManagers = new List<FacilityManager>();
            rollbackData.SetInsertFacilityManagers(rollbackManagers);

            foreach (FacilityManager manager in managers)
            {
                Dictionary<FacilityManager.Fields, object> dicConditions = new Dictionary<FacilityManager.Fields, object>();
                dicConditions[FacilityManager.Fields.ID] = manager.ID;

                if (m_sdmsDataManager.GetDeleteManager().DeleteFacilityManager(dicConditions, null, out strErrorMessage) == false)
                    return false;
                else
                    rollbackManagers.Add(manager);
            }

            strCondition = string.Format("{0} = {1} and {2} in ({3})",
                BuildingFacilityManager.GetFieldName(BuildingFacilityManager.Fields.MemberType, out isNullable),
                memberType,
                BuildingFacilityManager.GetFieldName(BuildingFacilityManager.Fields.MemberID, out isNullable),
                memberID);

            List<BuildingFacilityManager> buildingManagers = m_sdmsDataManager.GetSelectManager().SelectBuildingFacilityManagers(null, strCondition, out strErrorMessage);

            if (buildingManagers == null)
                return false;

            rollbackData = new TeamRollbackData();
            rollback.AddData(rollbackData);

            List<BuildingFacilityManager> rollbackBuildingManagers = new List<BuildingFacilityManager>();
            rollbackData.SetInsertBuildingFacilityManagers(rollbackBuildingManagers);

            foreach (BuildingFacilityManager manager in buildingManagers)
            {
                Dictionary<BuildingFacilityManager.Fields, object> dicConditions = new Dictionary<BuildingFacilityManager.Fields, object>();
                dicConditions[BuildingFacilityManager.Fields.ID] = manager.ID;

                if (m_sdmsDataManager.GetDeleteManager().DeleteBuildingFacilityManager(dicConditions, null, out strErrorMessage) == false)
                    return false;
                else
                    rollbackBuildingManagers.Add(manager);
            }

            strCondition = string.Format("{0} = {1} and {2} in ({3})",
                EquipZoneFacilityManager.GetFieldName(EquipZoneFacilityManager.Fields.MemberType, out isNullable),
                memberType,
                EquipZoneFacilityManager.GetFieldName(EquipZoneFacilityManager.Fields.MemberID, out isNullable),
                memberID);

            List<EquipZoneFacilityManager> equipZoneManagers = m_sdmsDataManager.GetSelectManager().SelectEquipZoneFacilityManagers(null, strCondition, out strErrorMessage);

            if (equipZoneManagers == null)
                return false;

            rollbackData = new TeamRollbackData();
            rollback.AddData(rollbackData);

            List<EquipZoneFacilityManager> rollbackEquipZoneManagers = new List<EquipZoneFacilityManager>();
            rollbackData.SetInsertEquipZoneFacilityManagers(rollbackEquipZoneManagers);

            foreach (EquipZoneFacilityManager manager in equipZoneManagers)
            {
                Dictionary<EquipZoneFacilityManager.Fields, object> dicConditions = new Dictionary<EquipZoneFacilityManager.Fields, object>();
                dicConditions[EquipZoneFacilityManager.Fields.ID] = manager.ID;

                if (m_sdmsDataManager.GetDeleteManager().DeleteEquipZoneFacilityManager(dicConditions, null, out strErrorMessage) == false)
                    return false;
                else
                    rollbackEquipZoneManagers.Add(manager);
            }

            return true;
        }

        private bool UpdateRegularMember(ICollection<RegularMember> regularMembers, Dictionary<int, int> dicChangeRegularID, RollbackManager rollback, out string strErrorMessage)
        {
            strErrorMessage = "";

            TeamRollbackData rollbackData = new TeamRollbackData();
            rollback.AddData(rollbackData);

            List<RegularMember> rollbackMembers = new List<RegularMember>();
            rollbackData.SetUpdateRegularMembers(rollbackMembers);

            foreach (RegularMember member in regularMembers)
            {
                if (dicChangeRegularID.ContainsKey(member.RegularID) == true)
                {
                    member.RegularID = dicChangeRegularID[member.RegularID];
                }

                if (member.PhoneNumber != null && member.PhoneNumber != "")
                {
                    member.PhoneNumber = LoadManager.EncryptString(member.PhoneNumber);
                }

                if (m_dataManager.GetUpdateManager().UpdateRegularMember(member, out strErrorMessage) == false)
                    return false;
                else
                    rollbackMembers.Add(member);
            }

            return true;
        }

        private bool AddRegularMember(ICollection<RegularMember> regularMembers, Dictionary<int, int> dicChangeRegularID, RollbackManager rollback, out string strErrorMessage)
        {
            strErrorMessage = "";

            TeamRollbackData rollbackData = new TeamRollbackData();
            rollback.AddData(rollbackData);

            List<RegularMember> rollbackMembers = new List<RegularMember>();
            rollbackData.SetDeleteRegularMembers(rollbackMembers);

            foreach (RegularMember member in regularMembers)
            {
                int nID = m_dataManager.GetSelectManager().GetMaxID(RegularMember.GetTableName(), out strErrorMessage);
                if (nID == -1)
                {
                    return false;
                }

                member.ID = nID;
                
                if (dicChangeRegularID.ContainsKey(member.RegularID) == true)
                {
                    member.RegularID = dicChangeRegularID[member.RegularID];
                }

                if (member.PhoneNumber != null && member.PhoneNumber != "")
                {
                    member.PhoneNumber = LoadManager.EncryptString(member.PhoneNumber);
                }

                if (m_dataManager.GetCreateManager().AddRegularMember(member) == false)
                    return false;
                else
                    rollbackMembers.Add(member);
            }


            return true;
        }

        private bool AddRegular(ICollection<Regular> regulars, out Dictionary<int, int> dicChangeRegularID, RollbackManager rollback, out string strErrorMessage)
        {
            strErrorMessage = "";
            dicChangeRegularID = new Dictionary<int, int>();

            TeamRollbackData rollbackData = new TeamRollbackData();
            rollback.AddData(rollbackData);

            List<Regular> rollbackTeams = new List<Regular>();
            rollbackData.SetDeleteRegulars(rollbackTeams);

            foreach (Regular regular in regulars)
            {
                // 이미 추가된 Regular 제외
                if (regular.ID > 0)
                    continue;

                int nID_old = regular.ID;

                int nID = m_dataManager.GetSelectManager().GetMaxID(Regular.GetTableName(), out strErrorMessage);
                if (nID == -1)
                {
                    return false;
                }

                regular.ID = nID;

                
                if (regular.ParentTeamID != null && regular.ParentTeamID < 0)
                {   // 부모 regular ID를 아직 모른다면 

                    if (dicChangeRegularID.ContainsKey((int)regular.ParentTeamID))
                        regular.ParentTeamID = dicChangeRegularID[(int)regular.ParentTeamID];
                    else
                    {   // 부모 regular가 아직 추가되지 않았다면 
                        regular.ParentTeamID = AddParentRegular((int)regular.ParentTeamID, regulars, dicChangeRegularID, rollbackTeams, out strErrorMessage);
                        if (regular.ParentTeamID < 0)
                        {
                            return false;
                        }
                    }
                }

                if (m_dataManager.GetCreateManager().AddRegular(regular) == false)
                    return false;
                else
                    rollbackTeams.Add(regular);

                dicChangeRegularID[nID_old] = nID;
            }

            return true;
        }

        private int AddParentRegular(int nParentTeamID, ICollection<Regular> regulars, Dictionary<int, int> dicChangeRegularID, List<Regular> rollbackTeams, out string strErrorMessage)
        {
            strErrorMessage = "";
            int nRegularID = nParentTeamID;

            foreach (Regular regular in regulars)
            {
                if (regular.ID == nParentTeamID)
                {
                    int nID_old = regular.ID;

                    int nID = m_dataManager.GetSelectManager().GetMaxID(Regular.GetTableName(), out strErrorMessage);
                    if (nID == -1)
                    {
                        return nRegularID;
                    }

                    regular.ID = nID;

                    if (regular.ParentTeamID != null && regular.ParentTeamID < 0)
                    {
                        if (dicChangeRegularID.ContainsKey((int)regular.ParentTeamID))
                            regular.ParentTeamID = dicChangeRegularID[(int)regular.ParentTeamID];
                        else
                        {
                            regular.ParentTeamID = AddParentRegular((int)regular.ParentTeamID, regulars, dicChangeRegularID, rollbackTeams, out strErrorMessage);
                            if (regular.ParentTeamID < 0)
                            {
                                return nRegularID;
                            }
                        }
                    }

                    if (m_dataManager.GetCreateManager().AddRegular(regular) == false)
                        return -1;
                    else
                        rollbackTeams.Add(regular);

                    dicChangeRegularID[nID_old] = nID;
                    nRegularID = nID;
                    break;
                }
            }

            return nRegularID;
        }

        private int? GetParentID(string strTeamPath, Dictionary<string, Regular> dicPathRegulars_Current, ref Dictionary<string, Regular> dicPathRegulars, ref int nRegularNewID)
        {
            int nParentID = -1;

            Regular regular = null;

            if (dicPathRegulars_Current.ContainsKey(strTeamPath))
            {
                regular = dicPathRegulars_Current[strTeamPath];
            }
            else if (dicPathRegulars.ContainsKey(strTeamPath))
            {
                regular = dicPathRegulars[strTeamPath];
            }
            else
            {
                regular = new Regular();
                regular.ID = nRegularNewID;

                nRegularNewID = nRegularNewID - 1;

                int nIdx = strTeamPath.LastIndexOf('|');
                if (nIdx >= 0)
                {
                    string strParentName = strTeamPath.Substring(0, nIdx);

                    nIdx++;
                    string strTeamName = strTeamPath.Substring(nIdx);

                    regular.TeamName = strTeamName;

                    int? nParentTeamID = GetParentID(strParentName, dicPathRegulars_Current, ref dicPathRegulars, ref nRegularNewID);
                    if (nParentTeamID == null)
                        return nParentTeamID;

                    regular.ParentTeamID = nParentTeamID;
                }
                else
                {
                    regular.TeamName = strTeamPath;
                    regular.ParentTeamID = null;
                }

                dicPathRegulars[strTeamPath] = regular;
            }

            nParentID = regular.ID;

            return nParentID;
        }

        private Dictionary<string, Regular> CloenDicRegularData(Dictionary<string, Regular> dicPathRegulars, out string strErrorMessage)
        {
            strErrorMessage = "";
            Dictionary<string, Regular> dicCloen = null;

            if (dicPathRegulars == null)
            {
                strErrorMessage = "데이터가 제대로 되지 않았습니다.";
                return dicCloen;
            }

            dicCloen = new Dictionary<string, Regular>();

            foreach (KeyValuePair<string, Regular> pair in dicPathRegulars)
            {
                string strPath = pair.Key;
                Regular regular = pair.Value;

                Regular data = new Regular();
                data.ID = regular.ID;
                data.TeamName = regular.TeamName;
                data.ParentTeamID = regular.ParentTeamID;

                dicCloen[strPath] = data;
            }

            return dicCloen;
        }

        private Dictionary<string, RegularMember> CloenDicMemberData(Dictionary<string, RegularMember> dicRegularMembers, out string strErrorMessage)
        {
            strErrorMessage = "";
            Dictionary<string, RegularMember> dicCloen = null;

            if (dicRegularMembers == null)
            {
                strErrorMessage = "데이터가 제대로 되지 않았습니다.";
                return dicCloen;
            }

            dicCloen = new Dictionary<string, RegularMember>();

            foreach (KeyValuePair<string, RegularMember> pair in dicRegularMembers)
            {
                string strMemberID = pair.Key;
                RegularMember member = pair.Value;

                RegularMember data = new RegularMember();
                data.ID = member.ID;
                data.MemberName = member.MemberName;
                data.MemberID = member.MemberID;

                data.RegularID = member.RegularID;
                data.JobLevelID = member.JobLevelID;
                data.JobPositionID = member.JobPositionID;
                data.OfficePhoneNumber = member.OfficePhoneNumber;
                data.PhoneNumber = member.PhoneNumber;
                data.Email = member.Email;
                data.StatusID = member.StatusID;

                dicCloen[strMemberID] = data;
            }

            return dicCloen;
        }

        private Dictionary<int, RegularMember> CloneRegularMemberData(Dictionary<int, RegularMember> regularMembers, out string strErrorMessage)
        {
            strErrorMessage = "";
            Dictionary<int, RegularMember> cloenMembers = null;

            if (regularMembers == null)
            {
                strErrorMessage = "데이터가 제대로 되지 않았습니다.";
                return cloenMembers;
            }

            cloenMembers = new Dictionary<int, RegularMember>();

            foreach (KeyValuePair<int, RegularMember> pair in regularMembers)
            {
                int nID = pair.Key;
                RegularMember member = pair.Value;

                RegularMember data = new RegularMember();
                data.ID = member.ID;
                data.MemberName = member.MemberName;
                data.MemberID = member.MemberID;

                data.RegularID = member.RegularID;
                data.JobLevelID = member.JobLevelID;
                data.JobPositionID = member.JobPositionID;
                data.OfficePhoneNumber = member.OfficePhoneNumber;
                data.PhoneNumber = member.PhoneNumber;
                data.Email = member.Email;
                data.StatusID = member.StatusID;

                cloenMembers[nID] = member;
            }

            return cloenMembers;
        }

        private bool SetCurrentRegularMember(Dictionary<int, Regular> dicIDRegulars, Dictionary<int, RegularMember> dicIDRegularMembers, out Dictionary<string, Regular> dicPathRegulars, out Dictionary<string, RegularMember> dicRegularMembers, out string strErrorMessage)
        {
            strErrorMessage = "";
            dicPathRegulars = new Dictionary<string, Regular>();
            dicRegularMembers = new Dictionary<string, RegularMember>();

            List<Regular> regulars = new List<Regular>();

            foreach (KeyValuePair<int, Regular> pair in dicIDRegulars)
            {
                Regular regular = pair.Value;

                regulars.Add(regular);
            }

            foreach (KeyValuePair<int, Regular> pair in dicIDRegulars)
            {
                Regular regular = pair.Value;

                if (regular.ParentTeamID == null)
                {   // 루트 Regular
                    RegularTeam team = new RegularTeam();
                    team.ID = regular.ID;
                    team.ParentTeamID = null;
                    team.TeamName = regular.TeamName;
                    team.Path = regular.TeamName;

                    dicPathRegulars[team.Path] = team;

                    // 자식 Regular 조회 
                    GetChildRegularTeam(team, regulars, ref dicPathRegulars);
                }
            }

            foreach (KeyValuePair<int, RegularMember> pair in dicIDRegularMembers)
            {
                RegularMember member = pair.Value;

                if (member.MemberID == null || member.MemberID == "")
                    continue;

                if (member.PhoneNumber != null && member.PhoneNumber != "")
                    member.PhoneNumber = LoadManager.DecryptString(member.PhoneNumber);

                dicRegularMembers[member.MemberID] = member;
            }

            return true;
        }

        private bool LoadCurrentRegularMember(out Dictionary<string, Regular> dicPathRegulars, out Dictionary<string, RegularMember> dicRegularMembers, out Dictionary<int, RegularMember> dicIDRegularMembers, out string strErrorMessage)
        {
            strErrorMessage = "";
            dicPathRegulars = null;
            dicRegularMembers = null;
            dicIDRegularMembers = null;

            Dictionary<Regular.Fields, object> dicConditions = new Dictionary<Regular.Fields, object>();

            List<Regular> regulars =  m_dataManager.GetSelectManager().SelectRegulars(dicConditions, out strErrorMessage);
            if (regulars == null)
                return false;

            Dictionary<RegularMember.Fields, object> dicConditions_RegularMember = new Dictionary<RegularMember.Fields, object>();
            string strAdditionalConditions = "";

            List<RegularMember> regularMembers = m_dataManager.GetSelectManager().SelectRegularMembers(dicConditions_RegularMember, strAdditionalConditions, out strErrorMessage);
            if (regularMembers == null)
                return false;


            if (SetRegularMemberData(regulars, regularMembers, out dicPathRegulars, out dicRegularMembers, out dicIDRegularMembers, out strErrorMessage) == false)
                return false;


            return true;
        }

        private bool SetRegularMemberData(List<Regular> regulars, List<RegularMember> regularMembers, out Dictionary<string, Regular> dicPathRegulars, out Dictionary<string, RegularMember> dicRegularMembers, out Dictionary<int, RegularMember> dicIDRegularMembers, out string strErrorMessage)
        {
            strErrorMessage = "";

            dicPathRegulars = new Dictionary<string, Regular>();
            dicRegularMembers = new Dictionary<string, RegularMember>();
            dicIDRegularMembers = new Dictionary<int, RegularMember>();

            if (regulars == null || regularMembers == null)
            {
                strErrorMessage = "조직정보에 제대로 된 값이 들어있지 않습니다.";
                return false;
            }

            foreach (Regular regular in regulars)
            {
                if (regular.ParentTeamID == null)
                {   // 루트 Regular
                    RegularTeam team = new RegularTeam();
                    team.ID = regular.ID;
                    team.ParentTeamID = null;
                    team.TeamName = regular.TeamName;
                    team.Path = regular.TeamName;

                    dicPathRegulars[team.Path] = team;

                    // 자식 Regular 조회 
                    GetChildRegularTeam(team, regulars, ref dicPathRegulars);
                }
            }

            foreach (RegularMember member in regularMembers)
            {
                if (member.PhoneNumber != null && member.PhoneNumber != "")
                    member.PhoneNumber = LoadManager.DecryptString(member.PhoneNumber);

                dicIDRegularMembers[member.ID] = member;

                if (member.MemberID == null || member.MemberID == "")
                    continue;

                dicRegularMembers[member.MemberID] = member;
            }

            return true;
        }

        private void GetChildRegularTeam(RegularTeam parentTeam, List<Regular> regulars, ref Dictionary<string, Regular> dicPathRegulars)
        {
            if (dicPathRegulars == null)
                dicPathRegulars = new Dictionary<string, Regular>();

            foreach (Regular regular in regulars)
            {
                if (parentTeam.ID == regular.ParentTeamID)
                {
                    RegularTeam child = new RegularTeam();
                    child.ID = regular.ID;
                    child.ParentTeamID = regular.ParentTeamID;
                    child.TeamName = regular.TeamName;

                    child.Path = parentTeam.Path + "|" + child.TeamName;

                    dicPathRegulars[child.Path] = child;

                    GetChildRegularTeam(child, regulars, ref dicPathRegulars);
                }
            }
        }

        public MessageResult SaveUpdateData(RequestSaveUpdateData data)
        {
            MessageResult result = new MessageResult();
            string strErrorMessage = "";

            Dictionary<int, int> dicChangeTeamID = new Dictionary<int, int>();  // 새로 추가된 팀 ID 변경 정보 >> 새로 추가된 팀 멤버 추가 시에 사용 
            Dictionary<int, int> dicChangeTemporaryID = new Dictionary<int, int>(); // 비상조직 팀 ID 변경 정보
            Dictionary<int, int> dicChangeRegularMemberID = new Dictionary<int, int>();
            Dictionary<int, int> dicChangeTemporaryMemberID = new Dictionary<int, int>();

            // 정규조직 팀 추가
            foreach (Regular regular in data.AddRegular)
            {
                int nID_OLD = regular.ID;

                int nID = m_dataManager.GetSelectManager().GetMaxID("SopTeamRegular", out strErrorMessage);
                if (nID == -1)
                {
                    result.Message = strErrorMessage;
                    result.Success = false;
                    return result;
                }

                if (regular.ParentTeamID != null && regular.ParentTeamID < 0 && 
                    dicChangeTeamID.ContainsKey((int)regular.ParentTeamID))
                    regular.ParentTeamID = dicChangeTeamID[(int)regular.ParentTeamID];

                regular.ID = nID;
                if (!m_dataManager.GetCreateManager().AddRegular(regular, out strErrorMessage))
                {
                    result.Message = "AddRegular 실패하였습니다.";
                    result.Success = false;
                    return result;
                }

                // 새로 추가된 팀 ID 변경 정보 저장
                dicChangeTeamID[nID_OLD] = nID;
            }

            // 비상조직 추가
            foreach (Temporary temporary in data.AddTemporary)
            {
                int nID_OLD = temporary.ID;

                int nID = m_dataManager.GetSelectManager().GetMaxID(Temporary.GetTableName(), out strErrorMessage);
                if (nID == -1)
                {
                    result.Message = strErrorMessage;
                    result.Success = false;
                    return result;
                }

                if (temporary.ParentTeamID != null && temporary.ParentTeamID < 0 &&
                    dicChangeTemporaryID.ContainsKey((int)temporary.ParentTeamID))
                    temporary.ParentTeamID = dicChangeTemporaryID[(int)temporary.ParentTeamID];

                temporary.ID = nID;
                temporary.IsNormal = true;
                temporary.SiteID = m_dataManager.SiteID;

                if (!m_dataManager.GetCreateManager().AddTemporary(temporary, out strErrorMessage))
                {
                    result.Message = "AddTemporary 실패하였습니다.";
                    result.Success = false;
                    return result;
                }

                // 새로 추가된 팀 ID 변경 정보 저장
                dicChangeTemporaryID[nID_OLD] = nID;
            }

            // 휴일 비상조직 추가
            foreach (Temporary temporary in data.AddTemporaryEmergency)
            {
                int nID_OLD = temporary.ID;

                int nID = m_dataManager.GetSelectManager().GetMaxID(Temporary.GetTableName(), out strErrorMessage);
                if (nID == -1)
                {
                    result.Message = strErrorMessage;
                    result.Success = false;
                    return result;
                }

                if (temporary.ParentTeamID != null && temporary.ParentTeamID < 0 &&
                    dicChangeTemporaryID.ContainsKey((int)temporary.ParentTeamID))
                    temporary.ParentTeamID = dicChangeTemporaryID[(int)temporary.ParentTeamID];

                temporary.ID = nID;
                temporary.IsNormal = false;
                temporary.SiteID = m_dataManager.SiteID;

                if (!m_dataManager.GetCreateManager().AddTemporary(temporary, out strErrorMessage))
                {
                    result.Message = "AddTemporary 실패하였습니다.";
                    result.Success = false;
                    return result;
                }

                // 새로 추가된 팀 ID 변경 정보 저장
                dicChangeTemporaryID[nID_OLD] = nID;
            }

            // 정규조직 멤버 추가
            foreach (RegularMember member in data.AddRegularMembers)
            {
                int nID = m_dataManager.GetSelectManager().GetMaxID("SopTeamRegularMember", out strErrorMessage);
                if (nID == -1)
                {
                    result.Message = strErrorMessage;
                    result.Success = false;
                    return result;
                }

                dicChangeRegularMemberID[member.ID] = nID;
                member.ID = nID;

                if (member.PhoneNumber != null)
                    member.PhoneNumber = LoadManager.EncryptString(member.PhoneNumber);

                if (member.RegularID < 0)
                {
                    if (dicChangeTeamID.ContainsKey(member.RegularID))
                        member.RegularID = dicChangeTeamID[member.RegularID];
                    else
                    {
                        result.Message = "정규조직 멤버 추가 실패, 새로운 팀 ID가 조회되지 않음.";
                        result.Success = false;
                        return result;
                    }
                }

                RegularMember createMember = m_dataManager.GetCreateManager().AddRegularMember(member, out strErrorMessage);
                if (createMember == null)
                {
                    result.Message = "AddRegularMember 실패하였습니다.";
                    result.Success = false;
                    return result;
                }
            }

            // 비상조직 멤버 추가
            foreach (TemporaryMember member in data.AddTemporaryMembers)
            {
                int nID = m_dataManager.GetSelectManager().GetMaxID(TemporaryMember.GetTableName(), out strErrorMessage);
                if (nID == -1)
                {
                    result.Message = strErrorMessage;
                    result.Success = false;
                    return result;
                }

                member.ID = nID;

                if (member.TeamID < 0)
                {
                    if (dicChangeTemporaryID.ContainsKey(member.TeamID))
                        member.TeamID = dicChangeTemporaryID[member.TeamID];
                    else
                    {
                        result.Message = "비상조직 멤버 추가 실패, 비상조직 ID가 조회되지 않음.";
                        result.Success = false;
                        return result;
                    }
                }

                // 신규 정규조직일 경우
                if (member.RegularID != null && member.RegularID < 0)
                {
                    if (dicChangeTeamID.ContainsKey((int)member.RegularID))
                        member.RegularID = dicChangeTeamID[(int)member.RegularID];
                    else
                    {
                        result.Message = "정규조직 멤버 추가 실패, 새로운 팀 ID가 조회되지 않음.";
                        result.Success = false;
                        return result;
                    }
                }

                // 신규 정규조직 멤버일 경우
                if (member.RegularMemberID != null && member.RegularMemberID < 0)
                {
                    if (dicChangeRegularMemberID.ContainsKey((int)member.RegularMemberID))
                        member.RegularMemberID = dicChangeRegularMemberID[(int)member.RegularMemberID];
                    else
                    {
                        result.Message = "정규조직 멤버 추가 실패, 새로운 팀 ID가 조회되지 않음.";
                        result.Success = false;
                        return result;
                    }
                }

                if (!m_dataManager.GetCreateManager().AddTemporaryMember(member, out strErrorMessage))
                {
                    result.Message = "AddRegularMember 실패하였습니다.";
                    result.Success = false;
                    return result;
                }
            }

            // 정규조직 수정
            foreach (Regular regular in data.UpdateRegular)
            {
                if (!m_dataManager.GetUpdateManager().UpdateRegular(regular, out strErrorMessage))
                {
                    result.Message = strErrorMessage;
                    result.Success = false;
                    return result;
                }
            }

            // 비상조직 수정
            foreach (Temporary temporary in data.UpdateTemporary)
            {
                temporary.SiteID = m_dataManager.SiteID;

                if (!m_dataManager.GetUpdateManager().UpdateTemporary(temporary, out strErrorMessage))
                {
                    result.Message = strErrorMessage;
                    result.Success = false;
                    return result;
                }
            }

            // 휴일 비상조직 수정
            foreach (Temporary temporary in data.UpdateTemporaryEmergency)
            {
                temporary.SiteID = m_dataManager.SiteID;

                if (!m_dataManager.GetUpdateManager().UpdateTemporary(temporary, out strErrorMessage))
                {
                    result.Message = strErrorMessage;
                    result.Success = false;
                    return result;
                }
            }

            // 정규조직 멤버 수정
            foreach (RegularMember member in data.UpdateRegularMembers)
            {
                member.PhoneNumber = LoadManager.EncryptString(member.PhoneNumber);

                if (!m_dataManager.GetUpdateManager().UpdateRegularMember(member, out strErrorMessage))
                {
                    result.Message = strErrorMessage;
                    result.Success = false;
                    return result;
                }
            }

            // 비상조직 멤버 수정
            foreach (TemporaryMember member in data.UpdateTemporaryMembers)
            {
                if (member.TeamID < 0)
                {
                    if (dicChangeTemporaryID.ContainsKey(member.TeamID))
                        member.TeamID = dicChangeTemporaryID[member.TeamID];
                    else
                    {
                        result.Message = "비상조직 멤버 추가 실패, 비상조직 ID가 조회되지 않음.";
                        result.Success = false;
                        return result;
                    }
                }

                // 신규 정규조직일 경우
                if (member.RegularID != null && member.RegularID < 0)
                {
                    if (dicChangeTeamID.ContainsKey((int)member.RegularID))
                        member.RegularID = dicChangeTeamID[(int)member.RegularID];
                    else
                    {
                        result.Message = "정규조직 멤버 추가 실패, 새로운 팀 ID가 조회되지 않음.";
                        result.Success = false;
                        return result;
                    }
                }

                // 신규 정규조직 멤버일 경우
                if (member.RegularMemberID != null && member.RegularMemberID < 0)
                {
                    if (dicChangeRegularMemberID.ContainsKey((int)member.RegularMemberID))
                        member.RegularMemberID = dicChangeRegularMemberID[(int)member.RegularMemberID];
                    else
                    {
                        result.Message = "정규조직 멤버 추가 실패, 새로운 팀 ID가 조회되지 않음.";
                        result.Success = false;
                        return result;
                    }
                }

                if (!m_dataManager.GetUpdateManager().UpdateTemporaryMember(member, out strErrorMessage))
                {
                    result.Message = strErrorMessage;
                    result.Success = false;
                    return result;
                }
            }

            // 정규조직 멤버 삭제
            foreach (RegularMember member in data.RemoveRegularMembers)
            {
                int nID = member.ID;

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
                                    //result.Message = "DeleteSession 실패.";
                                    //result.Success = false;
                                    //return result;
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
                                    //result.Message = "DeleteOption 실패.";
                                    //result.Success = false;
                                    //return result;
                                    continue;
                                }
                            }
                        }

                            if (!m_sopDataManager.GetDeleteManager().DeleteUser(user.ID))
                        {
                            //result.Message = "DeleteUser 실패.";
                            //result.Success = false;
                            //return result;
                            continue;
                        }
                    }
                }

                // 연동된 비상조직 멤버 정보 변경 필요!!
                Dictionary<TemporaryMember.Fields, object> dicConditions_temporaryMember = new Dictionary<TemporaryMember.Fields, object>();
                dicConditions_temporaryMember[TemporaryMember.Fields.RegularMemberID] = nID;

                List<TemporaryMember> temporaryMembers = m_dataManager.GetSelectManager().SelectTemporaryMembers(dicConditions_temporaryMember, out strErrorMessage);
                if (temporaryMembers != null && temporaryMembers.Count > 0)
                {
                    foreach(TemporaryMember temporary in temporaryMembers)
                    {
                        temporary.RegularMemberID = null;

                        if (!m_dataManager.GetUpdateManager().UpdateTemporaryMember(temporary, out strErrorMessage))
                        {
                            result.Message = strErrorMessage;
                            result.Success = false;
                            return result;
                        }
                    }
                }

                if (!m_dataManager.GetDeleteManager().DeleteRegularMember(nID, out strErrorMessage))
                {
                    //result.Message = strErrorMessage;
                    //result.Success = false;
                    //return result;
                    continue;
                }
            }

            // 비상조직 멤버 삭제
            foreach (TemporaryMember member in data.RemoveTemporaryMembers)
            {
                int nID = member.ID;

                if (!m_dataManager.GetDeleteManager().DeleteTemporaryMember(nID, out strErrorMessage))
                {
                    //result.Message = strErrorMessage;
                    //result.Success = false;
                    //return result;
                    continue;
                }
            }

            // 정규조직 삭제
            foreach (Regular regular in data.RemoveRegular)
            {
                int nID = regular.ID;

                // 연동된 비상조직 멤버 정보 변경 필요!!
                Dictionary<TemporaryMember.Fields, object> dicConditions_temporaryMember = new Dictionary<TemporaryMember.Fields, object>();
                dicConditions_temporaryMember[TemporaryMember.Fields.RegularID] = nID;

                List<TemporaryMember> temporaryMembers = m_dataManager.GetSelectManager().SelectTemporaryMembers(dicConditions_temporaryMember, out strErrorMessage);
                if (temporaryMembers != null && temporaryMembers.Count > 0)
                {
                    foreach (TemporaryMember temporary in temporaryMembers)
                    {
                        temporary.RegularID = null;

                        if (!m_dataManager.GetUpdateManager().UpdateTemporaryMember(temporary, out strErrorMessage))
                        {
                            result.Message = strErrorMessage;
                            result.Success = false;
                            return result;
                        }
                    }
                }

                if (!m_dataManager.GetDeleteManager().DeleteRegular(nID, out strErrorMessage))
                {
                    //result.Message = strErrorMessage;
                    //result.Success = false;
                    //return result;
                    continue;
                }
            }

            // 비상조직 삭제
            foreach (Temporary temporary in data.RemoveTemporary)
            {
                int nID = temporary.ID;

                if (!m_dataManager.GetDeleteManager().DeleteTemporary(nID, out strErrorMessage))
                {
                    //result.Message = strErrorMessage;
                    //result.Success = false;
                    //return result;
                    continue;
                }
            }

            // 휴일 비상조직 삭제
            foreach (Temporary temporary in data.RemoveTemporaryEmergency)
            {
                int nID = temporary.ID;

                if (!m_dataManager.GetDeleteManager().DeleteTemporary(nID, out strErrorMessage))
                {
                    //result.Message = strErrorMessage;
                    //result.Success = false;
                    //return result;
                    continue;
                }
            }

            result.Success = true;
            return result;
        }
    }

    
}
