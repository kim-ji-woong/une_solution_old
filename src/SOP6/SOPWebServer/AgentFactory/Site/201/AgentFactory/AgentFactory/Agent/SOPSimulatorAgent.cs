using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;

namespace AgentFactory.Agent
{
    class SOPSimulatorAgent : BaseAgent
    {
        // PassToControlUser : 현재 제어권을 가진 유저가 판단하도록 한다.
        // NotAllowed : 제어권 요청을 반려한다.
        // Permitted : 제어권 요청을 허가한다.
        public enum RequestControlResult { PassToControlUser = -1, NotAllowed, Permitted };

        private const string RequestSOPControlFunction = "RequestSOPControl";
        public override MethodProcessType CheckMethod(MethodType type, params object[] args)
        {
            return MethodProcessType.Default;
        }

        public override object RunMethod(MethodType type, params object[] args)
        {
            if (type == MethodType.Etc)
            {
                int nArgumentCount = args.Count();

                if (nArgumentCount > 0)
                {
                    string strCommand = args[0].ToString();

                    if (strCommand == RequestSOPControlFunction)
                    {
                        if (nArgumentCount >= 7 && args[1] is DirectDBManager && args[2] is int && args[3] is int && args[4] is int && args[5] is int && args[6] is int)
                        {
                            int nActionStepHistoryID = (int)args[2];
                            int nCurrentControlClientID = (int)args[3];
                            int nCurrentControlClientLevel = (int)args[4];
                            int nRequestClientID = (int)args[5];
                            int nRequestClientLevel = (int)args[6];

                            // Level은 값이 낮을수록 높은 등급이다.(단, 0보다 작으면 안된다.)
                            if (nRequestClientID >= 0 && nRequestClientLevel >= 0)
                            {
                                // 더 높은 등급의 유저가 제어권을 요청하면 그냥 넘겨주도록 한다.
                                if (nCurrentControlClientID >= 0 && nCurrentControlClientLevel >= 0)
                                {
                                    if (nRequestClientLevel >= nCurrentControlClientLevel)
                                        return (int)RequestControlResult.PassToControlUser;
                                    else
                                        return (int)RequestControlResult.Permitted;
                                }
                                else
                                    return (int)RequestControlResult.Permitted;
                            }
                            else
                                return (int)RequestControlResult.NotAllowed;
                        }
                    }
                }
            }

            return null;
        }

        // Return 값
        // 1 : 제어권을 요청한 user에게 넘겨준다.
        // 0 : 제어권을 요청한 user에게 넘겨주지 않는다.
        // -1 : 제어권 요청을 현재 제어권을 가지고 있는 User에게 알려서 해당 User가 판단하도록 한다.
        private int RequestSOPControl(object[] args)
        {
            RequestControlResult result = RequestControlResult.NotAllowed;
            int nArgumentCount = args.Count();

            if (nArgumentCount >= 5 && args[1] is DirectDBManager && args[2] is int && args[3] is int && args[4] is int)
            {
                DirectDBManager dbMgr = (DirectDBManager)args[1];
                int nActionStepHistoryID = (int)args[2];
                int nCurrentControlUserID = (int)args[3];
                int nRequestUserID = (int)args[4];

                dbMgr = dbMgr.Clone();

                if (dbMgr.Connect() == false)
                    return (int)result;

                SOPUser currentUser, requestUser;

                if (GetSOPUsers(dbMgr, nCurrentControlUserID, nRequestUserID, out currentUser, out requestUser) == false)
                {
                    dbMgr.Close();
                    return (int)result;
                }

                result = CheckRequestControl(dbMgr, nActionStepHistoryID, currentUser, requestUser);
                dbMgr.Close();
            }

            return (int)result;
        }

        // Return 값
        // 1 : 제어권을 요청한 user에게 넘겨준다.
        // 0 : 제어권을 요청한 user에게 넘겨주지 않는다.
        // -1 : 제어권 요청을 현재 제어권을 가지고 있는 User에게 알려서 해당 User가 판단하도록 한다.
        private RequestControlResult CheckRequestControl(DirectDBManager dbMgr, int nActionStepHistoryID, SOPUser currentUser, SOPUser requestUser)
        {
            if (AbleToControlSOP(dbMgr, requestUser, nActionStepHistoryID) == false)
                return RequestControlResult.NotAllowed;

            if (currentUser == null)
                return RequestControlResult.Permitted;

            if (currentUser.Type == SOPUser.UserType.Master)
                return RequestControlResult.PassToControlUser;
            else if (currentUser.Type == SOPUser.UserType.BuildingAdmin)
            {
                if (requestUser.Type == SOPUser.UserType.Master)
                    return RequestControlResult.Permitted;
                else
                    return RequestControlResult.PassToControlUser;
            }
            else if (currentUser.Type == SOPUser.UserType.NormalUser)
            {
                if (requestUser.Type == SOPUser.UserType.NormalUser)
                    return RequestControlResult.PassToControlUser;
                else
                    return RequestControlResult.Permitted;
            }

            return RequestControlResult.Permitted;
        }

        private bool AbleToControlSOP(DirectDBManager dbMgr, SOPUser user, int nActionStepHistoryID)
        {
            if (user.Type == SOPUser.UserType.Master)
                return true;

            string strSQL = "Select do.BuildingID from ActionStepHistory as ash, ActionStep as _as, Disaster as d, DisasterOwner as do ";
            strSQL += "where ash.ActionStepID = _as.ID and _as.DisasterID = d.ID and d.ID = do.DisasterID and ash.ID = " + nActionStepHistoryID.ToString();

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            // Query가 실패한 것은 해당 테이블이 없을 가능성이 있다.
            // 이 경우는 제어권을 허가한다.
            if (arrResult == null)
                return true;

            if (arrResult.Count > 0)
            {
                VariousData<int> buildingID = WebDBManager.GetIntField(arrResult[0].ToString());

                if (buildingID != null)
                {
                    if (buildingID.Data == user.BuildingID)
                        return true;
                    else
                        return false;
                }
            }

            return true;
        }

        private bool GetSOPUsers(DirectDBManager dbMgr, int nCurrentControlUserID, int nRequestUserID, out SOPUser currentUser, out SOPUser requestUser)
        {
            currentUser = null;
            requestUser = null;

            string strIDs = "(" + nRequestUserID.ToString();

            if (nCurrentControlUserID > 0)
                strIDs += ", " + nCurrentControlUserID.ToString() + ")";
            else
                strIDs += ")";

            string strSQL = "Select su.ID, sl.LevelName, sb.BuildingID from SOPGenUser as su, SOPGenLevel as sl, SOPGenUserBuilding as sb where su.UserLevel = sl.ID and su.ID = sb.UserID and su.ID in " + strIDs;
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strLevelName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> buildingID = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                if (id == null || strLevelName == null)
                    continue;

                SOPUser user = new SOPUser();
                user.ID = id.Data;

                if (user.SetUserType(strLevelName, buildingID) == false)
                    continue;

                if (id.Data == nCurrentControlUserID)
                    currentUser = user;
                else if (id.Data == nRequestUserID)
                    requestUser = user;
            }

            return requestUser != null;
        }
    }

    class SOPUser
    {
        // 일반유저, 건물관리자, 총괄관리자
        public enum UserType { Unknown = 0, NormalUser, BuildingAdmin, Master };

        private int m_nID = -1;
        private UserType m_userType = UserType.Unknown;
        private int m_nBuildingID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public UserType Type
        {
            get { return m_userType; }
            set { m_userType = value; }
        }

        public int BuildingID
        {
            get { return m_nBuildingID; }
            set { m_nBuildingID = value; }
        }

        public bool SetUserType(string strLevelName, VariousData<int> buildingID)
        {
            if (strLevelName.Contains("관리자"))
            {
                if (buildingID == null)
                {
                    m_userType = UserType.Master;
                    m_nBuildingID = -1;
                }
                else
                {
                    m_userType = UserType.BuildingAdmin;
                    m_nBuildingID = buildingID.Data;
                }
            }
            else
            {
                // 일반 관리요원일 경우 반드시 해당 건물정보가 있어야 한다.
                if (buildingID == null)
                    return false;

                m_userType = UserType.NormalUser;
                m_nBuildingID = buildingID.Data;
            }

            return true;
        }
    }
}
