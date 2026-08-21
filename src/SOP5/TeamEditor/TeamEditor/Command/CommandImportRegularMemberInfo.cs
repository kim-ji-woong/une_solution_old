using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace TeamEditor.Command
{
    /// <summary>
    /// CVS파일의 데이터를 업로드할때 사용되는 커맨드 클래스
    /// </summary>
    public class CommandImportRegularMemberInfo : CommandEx
    {
        #region Member Information

        /// <summary>
        /// Member Info Interface
        /// </summary>
        private interface IMemberInfo
        {
            void Do(CompanyMember member);

            void RollBack(CompanyMember member);
        }

        /// <summary>
        /// 추가된 멤버의 정보
        /// </summary>
        private class InsertMemberInfo : IMemberInfo
        {
            // << Un-Do 로 인한 삭제실행시 >>

            // 1. RegularMemberList
            // 2. FacilityManager
            // 3. BuildingFacilityManager
            // 4. EquipZoneFacilityManager
            // 5. TemporaryMemberList
            // 6. Duty
            // 7. SOPGenUser -> Update Null 처리
            // 8. CompanyMember

            // 순으로 삭제할 것..
            // 대신 Re-Do 시에 삭제한 데이터를 다시 원상복구 할 수 있도록 처리..(안해도 될 듯..?)

            private RegularTeam m_team = null;
            private string m_strMemberID = "";
            private string m_strName = "";
            private int m_nLevelID = -1;
            private int m_nPositionID = -1;
            private string m_strOfficePhoneNumber = "";
            private string m_strPhoneNumber = "";
            private CompanyMember.JobLevelSubInfo m_subLevel = null;
            private CompanyMember.JobGroupPosition m_groupPosition = null;
            private CompanyMember.JobPositionSubInfo m_subPosition = null;

            public string MemberID
            {
                get { return m_strMemberID; }
                set { m_strMemberID = value; }
            }
            public string Name
            {
                get { return m_strName; }
                set { m_strName = value; }
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
            public string OfficePhoneNumber
            {
                get { return m_strOfficePhoneNumber; }
                set { m_strOfficePhoneNumber = value; }
            }
            public string PhoneNumber
            {
                get { return m_strPhoneNumber; }
                set { m_strPhoneNumber = value; }
            }
            public CompanyMember.JobLevelSubInfo SubJobLevel
            {
                get { return m_subLevel; }
                set { m_subLevel = value; }
            }
            public CompanyMember.JobGroupPosition GroupPosition
            {
                get { return m_groupPosition; }
                set { m_groupPosition = value; }
            }
            public CompanyMember.JobPositionSubInfo SubJobPosition
            {
                get { return m_subPosition; }
                set { m_subPosition = value; }
            }
            public RegularTeam RegulaTeam
            {
                get { return m_team; }
                set { m_team = value; }
            }


            public void Do(CompanyMember member)
            {
                List<CompanyMember> members = DataManager.GetRegularMembers(m_team);

                if (members.Contains(member) == false)
                    members.Add(member);

                member.MemberID = m_strMemberID;
                member.Name = m_strName;
                member.PositionID = m_nPositionID;
                member.LevelID = m_nLevelID;
                member.SubJobPosition = m_subPosition;
                member.SubJobLevel = m_subLevel;
                member.GroupPosition = m_groupPosition;
                member.PhoneNumber = m_strPhoneNumber;
                member.OfficePhoneNumber = m_strOfficePhoneNumber;
            }

            public void RollBack(CompanyMember member)
            {
                List<CompanyMember> members = DataManager.GetRegularMembers(m_team);
                members.Remove(member);
            }


            public string GetInsertCompanyMemberSQL(DBUtility.WebDBManager dbMgr, CompanyMember member)
            {
                string strSQL = string.Empty;

                string strIFNull = dbMgr.DatabaseType == DBUtility.WebDBManager.DBType.sqlserver ? "ISNULL" : "IFNULL";

                strSQL += "SELECT " + strIFNull + "(MAX(ID), 0) + 1 AS MAX_ID FROM CompanyMember ";
                int nID = Convert.ToInt32(dbMgr.GetBatchData(strSQL)[0]);
                int nSubLevelID = DataManager.GetJobSubLevel(dbMgr, 1, member.SubJobLevel);

                member.ID = nID;

                // 직원 정보 수정
                strSQL = "INSERT INTO CompanyMember (ID, MemberName, LevelID, SubLevelID, MemberID, OfficePhoneNumber, PhoneNumber) ";
                strSQL += String.Format("VALUES ({0}, '{1}', {2}, {3}, {4}, {5}, {6}) ",
                    member.ID,
                    member.Name,
                    member.LevelID,
                    nSubLevelID < 0 ? "NULL" : nSubLevelID.ToString(),
                    String.IsNullOrWhiteSpace(member.MemberID) || member.MemberID.Length == 0 ? "NULL" : String.Format("'{0}'", member.MemberID),
                    String.IsNullOrWhiteSpace(member.OfficePhoneNumber) || member.OfficePhoneNumber.Length == 0 ? "NULL" : String.Format("'{0}'", member.OfficePhoneNumber),
                    String.IsNullOrWhiteSpace(member.PhoneNumber) || member.PhoneNumber.Length == 0 ? "NULL" : String.Format("'{0}'", DataManager.EncryptString(member.PhoneNumber))
                );

                return strSQL;
            }

            public string GetInsertRegularMemberSQL(DBUtility.WebDBManager dbMgr, CompanyMember member)
            {
                string strSQL = string.Empty;

                int nID = member.ID;
                int nPositionID = member.PositionID;
                int nSubPositionID = DataManager.GetJobSubPosition(dbMgr, 1, member.SubJobPosition);
                int nGroupPositionID = DataManager.GetGroupPosition(dbMgr, 1, member.GroupPosition);

                // 본부장 / 처장 / 실장 등의 직위에 대해서 팀장 ID 로 변환후 DB 저장
                if (member.PositionID < 0 && member.PositionID > -100)
                {
                    if (DataManager.GetJobPositionID("팀장", out nPositionID) == false)
                    {
                        nPositionID = member.PositionID;
                    }
                }

                

                strSQL = "INSERT INTO RegularMemberList (RegularTeamID, CompanyMemberID, PositionID, SubPositionID, GroupPositionID) ";
                strSQL += string.Format("VALUES ( {0}, {1}, {2}, {3}, {4} )",
                    m_team.TeamID,
                    member.ID,
                    nPositionID,
                    nSubPositionID < 0 ? "NULL" : nSubPositionID.ToString(),
                    nGroupPositionID < 0 ? "NULL" : nGroupPositionID.ToString());

                return strSQL;

            }

            public string[] GetDeleteSQL(DBUtility.WebDBManager dbMgr, CompanyMember member)
            {
                string[] arrSQL = new string[8] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };

                arrSQL[0] = String.Format("DELETE FROM RegularMemberList WHERE CompanyMemberID = {0} ", member.ID);
                arrSQL[1] = String.Format("DELETE FROM FacilityManager WHERE MemberType = 0 AND MemberID = {0} ", member.ID);
                arrSQL[2] = String.Format("DELETE FROM BuildingFacilityManager WHERE MemberType = 0 AND MemberID = {0} ", member.ID);
                arrSQL[3] = String.Format("DELETE FROM EquipZoneFacilityManager WHERE MemberType = 0 AND MemberID = {0} ", member.ID);
                arrSQL[4] = String.Format("DELETE FROM TemporaryMemberList WHERE MemberType = 1 AND MemberID = {0} ", member.ID);
                //arrSQL[5] = String.Format("DELETE FROM Duty WHERE MemberID = {0} ", member.ID);
                arrSQL[5] = String.Format("DELETE FROM SOPGenUserCommander WHERE MemberType = 8 and MemberID = {0} ", member.ID);
                arrSQL[6] = String.Format("UPDATE SOPGenUser SET MemberID = NULL WHERE MemberID = {0} ", member.ID);
                arrSQL[7] = String.Format("DELETE FROM CompanyMember WHERE ID = {0} ", member.ID);
                
                return arrSQL;
            }

        }

        /// <summary>
        /// 수정된 멤버의 이전 / 이후 데이터 정보
        /// </summary>
        private class UpdateMemberInfo : IMemberInfo
        {
            private bool m_isNewInfo = false;

            private RegularTeam m_old_team = null;
            private RegularTeam m_new_team = null;

            private string m_old_strName = "";
            private string m_new_strName = "";

            private int m_old_nLevelID = -1;
            private int m_new_nLevelID = -1;

            private int m_old_nPositionID = -1;
            private int m_new_nPositionID = -1;

            private string m_old_strOfficePhoneNumber = "";
            private string m_new_strOfficePhoneNumber = "";

            private string m_old_strPhoneNumber = "";
            private string m_new_strPhoneNumber = "";

            private CompanyMember.JobLevelSubInfo m_old_subLevel = null;
            private CompanyMember.JobLevelSubInfo m_new_subLevel = null;

            private CompanyMember.JobGroupPosition m_old_groupPosition = null;
            private CompanyMember.JobGroupPosition m_new_groupPosition = null;

            private CompanyMember.JobPositionSubInfo m_old_subPosition = null;
            private CompanyMember.JobPositionSubInfo m_new_subPosition = null;


            public string Old_Name
            {
                get { return m_old_strName; }
                set { m_old_strName = value; }
            }
            public string New_Name
            {
                get { return m_new_strName; }
                set { m_new_strName = value; }
            }

            public int Old_LevelID
            {
                get { return m_old_nLevelID; }
                set { m_old_nLevelID = value; }
            }
            public int New_LevelID
            {
                get { return m_new_nLevelID; }
                set { m_new_nLevelID = value; }
            }

            public int Old_PositionID
            {
                get { return m_old_nPositionID; }
                set { m_old_nPositionID = value; }
            }
            public int New_PositionID
            {
                get { return m_new_nPositionID; }
                set { m_new_nPositionID = value; }
            }

            public string Old_OfficePhoneNumber
            {
                get { return m_old_strOfficePhoneNumber; }
                set { m_old_strOfficePhoneNumber = value; }
            }
            public string New_OfficePhoneNumber
            {
                get { return m_new_strOfficePhoneNumber; }
                set { m_new_strOfficePhoneNumber = value; }
            }

            public string Old_PhoneNumber
            {
                get { return m_old_strPhoneNumber; }
                set { m_old_strPhoneNumber = value; }
            }
            public string New_PhoneNumber
            {
                get { return m_new_strPhoneNumber; }
                set { m_new_strPhoneNumber = value; }
            }

            public CompanyMember.JobLevelSubInfo Old_SubJobLevel
            {
                get { return m_old_subLevel; }
                set { m_old_subLevel = value; }
            }
            public CompanyMember.JobLevelSubInfo New_SubJobLevel
            {
                get { return m_new_subLevel; }
                set { m_new_subLevel = value; }
            }

            public CompanyMember.JobGroupPosition Old_GroupPosition
            {
                get { return m_old_groupPosition; }
                set { m_old_groupPosition = value; }
            }
            public CompanyMember.JobGroupPosition New_GroupPosition
            {
                get { return m_new_groupPosition; }
                set { m_new_groupPosition = value; }
            }

            public CompanyMember.JobPositionSubInfo Old_SubJobPosition
            {
                get { return m_old_subPosition; }
                set { m_old_subPosition = value; }
            }
            public CompanyMember.JobPositionSubInfo New_SubJobPosition
            {
                get { return m_new_subPosition; }
                set { m_new_subPosition = value; }
            }

            public RegularTeam Old_RegulaTeam
            {
                get { return m_old_team; }
                set { m_old_team = value; }
            }
            public RegularTeam New_RegulaTeam
            {
                get { return m_new_team; }
                set { m_new_team = value; }
            }


            public void Do(CompanyMember member)
            {
                m_isNewInfo = true;

                List<CompanyMember> members = DataManager.GetRegularMembers(m_old_team);
                members.Remove(member);

                members = DataManager.GetRegularMembers(m_new_team);
                members.Add(member);

                member.Name = m_new_strName;
                member.PositionID = m_new_nPositionID;
                member.LevelID = m_new_nLevelID;
                member.SubJobPosition = m_new_subPosition;
                member.SubJobLevel = m_new_subLevel;
                member.GroupPosition = m_new_groupPosition;
                member.PhoneNumber = m_new_strPhoneNumber;
                member.OfficePhoneNumber = m_new_strOfficePhoneNumber;
            }

            public void RollBack(CompanyMember member)
            {
                m_isNewInfo = false;

                List<CompanyMember> members = DataManager.GetRegularMembers(m_new_team);
                members.Remove(member);

                members = DataManager.GetRegularMembers(m_old_team);
                members.Add(member);

                member.Name = m_old_strName;
                member.PositionID = m_old_nPositionID;
                member.LevelID = m_old_nLevelID;
                member.SubJobPosition = m_old_subPosition;
                member.SubJobLevel = m_old_subLevel;
                member.GroupPosition = m_old_groupPosition;
                member.PhoneNumber = m_old_strPhoneNumber;
                member.OfficePhoneNumber = m_old_strOfficePhoneNumber;
            }


            public string[] GetUpdateSQL(DBUtility.WebDBManager dbMgr, CompanyMember member)
            {
                string[] arrSQL = new string[2] { "", "" };
                string strSQL = string.Empty;

                int nPositionID = member.PositionID;
                int nSubLevelID = DataManager.GetJobSubLevel(dbMgr, 1, member.SubJobLevel);
                int nSubPositionID = DataManager.GetJobSubPosition(dbMgr, 1, member.SubJobPosition);
                int nGroupPositionID = DataManager.GetGroupPosition(dbMgr, 1, member.GroupPosition);
                RegularTeam team = (m_isNewInfo ? m_new_team : m_old_team);

                // 본부장 / 처장 / 실장 등의 직책을 가진 직원에 대해서는 DB에 저장할 때, PositionID 값을 팀장(2) 로 저장
                if (member.PositionID < 0 && member.PositionID > -100)
                {
                    if (DataManager.GetJobPositionID("팀장", out nPositionID) == false)
                    {
                        nPositionID = member.PositionID;
                    }
                }

                // 직원 정보 수정
                strSQL = String.Format("UPDATE CompanyMember SET MemberName = '{0}', LevelID = {1}, SubLevelID = {2}, OfficePhoneNumber = {3}, PhoneNumber = {4} WHERE ID = {5} ",
                    member.Name,
                    member.LevelID,
                    nSubLevelID < 0 ? "NULL" : nSubLevelID.ToString(),
                    String.IsNullOrWhiteSpace(member.OfficePhoneNumber) || member.OfficePhoneNumber.Length == 0 ? "NULL" : String.Format("'{0}'", member.OfficePhoneNumber),
                    String.IsNullOrWhiteSpace(member.PhoneNumber) || member.PhoneNumber.Length == 0 ? "NULL" : String.Format("'{0}'", DataManager.EncryptString(member.PhoneNumber)),
                    member.ID
                    );
                arrSQL[0] = strSQL;


                // 정규조직원 정보 수정
                strSQL = String.Format("UPDATE RegularMemberList SET RegularTeamID = {0}, PositionID = {1}, SubPositionID = {2}, GroupPositionID = {3} WHERE CompanyMemberID = {4} ",
                    team.TeamID,
                    nPositionID,
                    nSubPositionID < 0 ? "NULL" : nSubPositionID.ToString(),
                    nGroupPositionID < 0 ? "NULL" : nGroupPositionID.ToString(),
                    member.ID
                    );
                arrSQL[1] = strSQL;

                return arrSQL;
            }

        }

        /// <summary>
        /// 삭제된 멤버의 정보
        /// </summary>
        private class DeleteMemberInfo : IMemberInfo
        {
            // << Data 삭제 순서 >>

            // 1. RegularMemberList
            // 2. FacilityManager
            // 3. BuildingFacilityManager
            // 4. EquipZoneFacilityManager
            // 5. TemporaryMemberList
            // 6. Duty
            // 7. SOPGenUser -> Update Null 처리
            // 8. CompanyMember

            // Re-Do 시에 다시 데이터 복구가 되어야 함..


            private RegularTeam m_team = null;
            public RegularTeam RegulaTeam
            {
                get { return m_team; }
                set { m_team = value; }
            }

            private int m_nID = -1;
            public int ID { get { return m_nID; } }

            private Stack<string> m_stackReverseSQL = new Stack<string>();


            public void Do(CompanyMember member)
            {
                List<CompanyMember> members = DataManager.GetRegularMembers(m_team);
                members.Remove(member);
            }

            public void RollBack(CompanyMember member)
            {
                List<CompanyMember> members = DataManager.GetRegularMembers(m_team);

                if (members.Contains(member) == false)
                    members.Add(member);
            }


            public string[] GetDeleteCompanyMemberSQL(DBUtility.WebDBManager dbMgr, CompanyMember member)
            {
                string[] arrSQL = new string[8] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };

                arrSQL[0] = String.Format("DELETE FROM RegularMemberList WHERE CompanyMemberID = {0} ", member.ID);
                arrSQL[1] = String.Format("DELETE FROM FacilityManager WHERE MemberType = 0 AND MemberID = {0} ", member.ID);
                arrSQL[2] = String.Format("DELETE FROM BuildingFacilityManager WHERE MemberType = 0 AND MemberID = {0} ", member.ID);
                arrSQL[3] = String.Format("DELETE FROM EquipZoneFacilityManager WHERE MemberType = 0 AND MemberID = {0} ", member.ID);
                arrSQL[4] = String.Format("DELETE FROM TemporaryMemberList WHERE MemberType = 1 AND MemberID = {0} ", member.ID);
                //arrSQL[5] = String.Format("DELETE FROM Duty WHERE MemberID = {0} ", member.ID);
                arrSQL[5] = String.Format("DELETE FROM SOPGenUserCommander WHERE MemberType = 8 and MemberID = {0} ", member.ID);
                arrSQL[6] = String.Format("UPDATE SOPGenUser SET MemberID = NULL WHERE MemberID = {0} ", member.ID);
                arrSQL[7] = String.Format("DELETE FROM CompanyMember WHERE ID = {0} ", member.ID);

                AddReverseSQL_RegularMemberList(dbMgr, member.ID);
                AddReverseSQL_FacilityManager(dbMgr, member.ID);
                AddReverseSQL_BuildingFacilityManager(dbMgr, member.ID);
                AddReverseSQL_EquipZoneFacilityManager(dbMgr, member.ID);
                AddReverseSQL_TemporaryMemberList(dbMgr, member.ID);
                //AddReverseSQL_Duty(dbMgr, member.ID);
                AddReverseSQL_SOPGenUser(dbMgr, member.ID);
                AddReverseSQL_CompanyMember(dbMgr, member.ID);

                m_nID = member.ID;

                return arrSQL;
            }

            public string[] GetReverseSQL(DBUtility.WebDBManager dbMgr, CompanyMember member)
            {
                List<string> liReverseSQL = new List<string>();

                while (m_stackReverseSQL.Count > 0)
                {
                    liReverseSQL.Add(m_stackReverseSQL.Pop());
                }

                return liReverseSQL.ToArray();
            }


            private void AddReverseSQL_RegularMemberList(DBUtility.WebDBManager dbMgr, int nID)
            {
                string strInsertFormat = "INSERT INTO RegularMemberList (RegularTeamID, CompanyMemberID, PositionID, SubPositionID, GroupPositionID) VALUES ({0}, {1}, {2}, {3}, {4}) ";
                string strSQL = string.Empty;

                strSQL = String.Format("SELECT RegularTeamID, CompanyMemberID, PositionID, SubPositionID, GroupPositionID FROM RegularMemberList WHERE CompanyMemberID = {0} ", nID);
                ArrayList arrResult = dbMgr.GetBatchData(strSQL);

                if (arrResult == null)
                    return;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 4; i += 5)
                {
                    int nRegularTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    int nCompanyMemberID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                    int nPositionID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                    int nSubPositionID = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                    int nGroupPositionID = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);

                    if (nRegularTeamID < 0 || nCompanyMemberID < 0 || nPositionID < 0)
                        continue;

                    strSQL = String.Format(strInsertFormat,
                        nRegularTeamID,
                        nCompanyMemberID,
                        nPositionID,
                        nSubPositionID < 0 ? "NULL" : nSubPositionID.ToString(),
                        nGroupPositionID < 0 ? "NULL" : nGroupPositionID.ToString());

                    m_stackReverseSQL.Push(strSQL);
                }
            }

            private void AddReverseSQL_FacilityManager(DBUtility.WebDBManager dbMgr, int nID)
            {
                string strInsertFormat = "INSERT INTO FacilityManager (ID, MemberID, MemberType, FacilityType, LevelLimit, Description, UpperLimit, SiteID) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}) ";
                string strSQL = string.Empty;

                strSQL = String.Format("SELECT ID, MemberID, MemberType, FacilityType, LevelLimit, Description, UpperLimit, SiteID FROM FacilityManager WHERE MemberType = 0 AND MemberID = {0} ", nID);
                ArrayList arrResult = dbMgr.GetBatchData(strSQL);

                if (arrResult == null)
                    return;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 7; i += 8)
                {
                    int nMgrID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    int nMemberID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                    int nMemberType = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                    int nFacilityType = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                    int nLevelLimit = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                    string strDesc = DBUtility.WebDBManager.GetStringField(arrResult[i + 5], null);
                    int nUpperLimit = DBUtility.WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                    int nSiteID = DBUtility.WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);

                    if (nMgrID < 0 || nMemberID < 0 || nMemberType < 0 || nFacilityType < 0 || nSiteID < 0)
                        continue;

                    strSQL = String.Format(strInsertFormat,
                        nMgrID,
                        nMemberID,
                        nMemberType,
                        nFacilityType,
                        nLevelLimit < 0 ? "NULL" : nLevelLimit.ToString(),
                        strDesc == null || strDesc == "null" ? "NULL" : "'" + strDesc + "'",
                        nUpperLimit,
                        nSiteID);

                    m_stackReverseSQL.Push(strSQL);
                }
            }

            private void AddReverseSQL_EquipZoneFacilityManager(DBUtility.WebDBManager dbMgr, int nID)
            {
                string strInsertFormat = "INSERT INTO EquipZoneFacilityManager (ID, MemberID, MemberType, SiteID, FacilityType, LevelLimit, EquipZoneID, Description, UpperLimit) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}) ";
                string strSQL = string.Empty;

                strSQL = String.Format("SELECT ID, MemberID, MemberType, SiteID, FacilityType, LevelLimit, EquipZoneID, Description, UpperLimit FROM EquipZoneFacilityManager WHERE MemberType = 0 AND MemberID = {0}", nID);
                ArrayList arrResult = dbMgr.GetBatchData(strSQL);

                if (arrResult == null)
                    return;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 8; i += 9)
                {
                    int nMgrID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    int nMemberID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                    int nMemberType = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                    int nSiteID = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                    int nFacilityType = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                    int nLevelLimit = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                    int nEquipZoneID = DBUtility.WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                    string strDesc = DBUtility.WebDBManager.GetStringField(arrResult[i + 7], null);
                    int nUpperLimit = DBUtility.WebDBManager.GetIntField(arrResult[i + 8].ToString(), -1);

                    if (nMgrID < 0 || nMemberID < 0 || nMemberType < 0 || nFacilityType < 0 || nEquipZoneID < 0 || nSiteID < 0)
                        continue;

                    strSQL = String.Format(strInsertFormat,
                        nMgrID,
                        nMemberID,
                        nMemberType,
                        nSiteID,
                        nFacilityType,
                        nLevelLimit < 0 ? "NULL" : nLevelLimit.ToString(),
                        nEquipZoneID,
                        strDesc == null || strDesc == "null" ? "NULL" : "'" + strDesc + "'",
                        nUpperLimit);

                    m_stackReverseSQL.Push(strSQL);
                }
            }

            private void AddReverseSQL_BuildingFacilityManager(DBUtility.WebDBManager dbMgr, int nID)
            {
                string strInsertFormat = "INSERT INTO BuildingFacilityManager (ID, MemberID, MemberType, FacilityType, LevelLimit, BuildingID, Description, UpperLimit, SiteID) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}) ";
                string strSQL = string.Empty;

                strSQL = String.Format("SELECT ID, MemberID, MemberType, FacilityType, LevelLimit, BuildingID, Description, UpperLimit, SiteID FROM BuildingFacilityManager WHERE MemberType = 0 AND MemberID = {0}", nID);
                ArrayList arrResult = dbMgr.GetBatchData(strSQL);

                if (arrResult == null)
                    return;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 8; i += 9)
                {
                    int nMgrID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    int nMemberID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                    int nMemberType = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                    int nFacilityType = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                    int nLevelLimit = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                    int nBuildingID = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                    string strDesc = DBUtility.WebDBManager.GetStringField(arrResult[i + 6], null);
                    int nUpperLimit = DBUtility.WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);
                    int nSiteID = DBUtility.WebDBManager.GetIntField(arrResult[i + 8].ToString(), -1);

                    if (nMgrID < 0 || nMemberID < 0 || nMemberType < 0 || nFacilityType < 0 || nBuildingID < 0 || nSiteID < 0)
                        continue;

                    strSQL = String.Format(strInsertFormat,
                        nMgrID,
                        nMemberID,
                        nMemberType,
                        nFacilityType,
                        nLevelLimit < 0 ? "NULL" : nLevelLimit.ToString(),
                        nBuildingID,
                        strDesc == null || strDesc == "null" ? "NULL" : "'" + strDesc + "'",
                        nUpperLimit,
                        nSiteID);

                    m_stackReverseSQL.Push(strSQL);
                }
            }

            private void AddReverseSQL_TemporaryMemberList(DBUtility.WebDBManager dbMgr, int nID)
            {
                string strInsertFormat = "INSERT INTO TemporaryMemberList (ID, MemberName, TemporaryTeamID, IsNormal, MemberID, IsTeamLeader, MemberType, MemberCount, Role) VALUES ({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8}) ";
                string strSQL = string.Empty;

                strSQL = String.Format("SELECT ID, MemberName, TemporaryTeamID, IsNormal, MemberID, IsTeamLeader, MemberType, MemberCount, Role FROM TemporaryMemberList WHERE MemberType = 1 AND MemberID = {0}", nID);
                ArrayList arrResult = dbMgr.GetBatchData(strSQL);

                if (arrResult == null)
                    return;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 8; i += 9)
                {
                    int nTempID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strMemberName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], "");
                    int nTemporaryTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                    bool isNormal = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0) == 0 ? false : true;
                    int nMemberID = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                    int nTeamLeader = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                    int nMemberType = DBUtility.WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                    int nMemberCount = DBUtility.WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);
                    int nRole = DBUtility.WebDBManager.GetIntField(arrResult[i + 8].ToString(), -1);

                    if (nTempID < 0 || nTemporaryTeamID < 0 || nMemberType < 0 || nRole < 0)
                        continue;

                    strSQL = String.Format(strInsertFormat,
                        nTempID,
                        strMemberName,
                        nTemporaryTeamID,
                        isNormal ? 1 : 0,
                        nMemberID < 0 ? "NULL" : nMemberID.ToString(),
                        nTeamLeader < 0 ? "NULL" : nTeamLeader.ToString(),
                        nMemberType,
                        nMemberCount < 0 ? "NULL" : nMemberCount.ToString(),
                        nRole);

                    m_stackReverseSQL.Push(strSQL);
                }
            }

            //private void AddReverseSQL_Duty(DBUtility.WebDBManager dbMgr, int nID)
            //{
            //    string strInsertFormat = "INSERT INTO Duty (ID, MemberID, InsertTime, TeamID, Description, SiteID) VALUES ({0}, {1}, '{2}', {3}, {4}, {5}) ";
            //    string strSQL = string.Empty;

            //    strSQL = String.Format("SELECT ID, MemberID, InsertTime, TeamID, Description, SiteID FROM Duty WHERE MemberID = {0}", nID);
            //    ArrayList arrResult = dbMgr.GetResultData(strSQL, 1);

            //    if (arrResult == null)
            //        return;

            //    int nResultCount = arrResult.Count;

            //    for (int i = 0; i < nResultCount - 5; i += 6)
            //    {
            //        int nDutyID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
            //        int nMemberID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
            //        string strInsertTime = DBUtility.WebDBManager.GetStringField(arrResult[i + 2], null);
            //        int nTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
            //        string strDesc = DBUtility.WebDBManager.GetStringField(arrResult[i + 4].ToString(), null);
            //        int nSiteID = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);

            //        if (nDutyID < 0 || nMemberID < 0 || strInsertTime == null || strInsertTime == "null" || nTeamID < 0 || nSiteID < 0)
            //            continue;

            //        strSQL = String.Format(strInsertFormat,
            //            nDutyID,
            //            nMemberID,
            //            strInsertTime,
            //            nTeamID,
            //            strDesc == null || strDesc == "null" ? "NULL" : "'" + strDesc + "'",
            //            nSiteID);

            //        m_stackReverseSQL.Push(strSQL);
            //    }
            //}

            private void AddReverseSQL_SOPGenUser(DBUtility.WebDBManager dbMgr, int nID)
            {
                string strUpdateFormat = "UPDATE SOPGenUser SET MemberID = {0} WHERE ID = {1}";
                string strSQL = string.Empty;

                strSQL = String.Format("SELECT ID, MemberID from SOPGenUser WHERE MemberID = {0}", nID);
                ArrayList arrResult = dbMgr.GetBatchData(strSQL);

                if (arrResult == null)
                    return;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 1; i += 2)
                {
                    int nGenID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    int nMemberID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);

                    if (nGenID < 0 || nMemberID < 0)
                        continue;

                    strSQL = String.Format(strUpdateFormat, nMemberID, nGenID);

                    m_stackReverseSQL.Push(strSQL);
                }
            }

            private void AddReverseSQL_CompanyMember(DBUtility.WebDBManager dbMgr, int nID)
            {
                string strInsertFormat = "INSERT INTO CompanyMember (ID, MemberName, LevelID, SubLevelID, MemberID, OfficePhoneNumber, PhoneNumber) VALUES ({0}, '{1}', {2}, {3}, {4}, {5}, {6}) ";
                string strSQL = string.Empty;

                strSQL = String.Format("SELECT ID, MemberName, LevelID, SubLevelID, MemberID, OfficePhoneNumber, PhoneNumber FROM CompanyMember WHERE ID = {0}", nID);
                ArrayList arrResult = dbMgr.GetBatchData(strSQL);

                if (arrResult == null)
                    return;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 6; i += 7)
                {
                    string strMemberName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], null);
                    int nLevelID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                    int nSubLevelID = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                    string strMemberID = DBUtility.WebDBManager.GetStringField(arrResult[i + 4], null);
                    string strOfficePhoneNumber = DBUtility.WebDBManager.GetStringField(arrResult[i + 5], null);
                    string strPhoneNumber = DBUtility.WebDBManager.GetStringField(arrResult[i + 6], null);

                    if (nID < 0 || strMemberName == null || nLevelID < 0)
                        continue;

                    strSQL = String.Format(strInsertFormat,
                        nID,
                        strMemberName,
                        nLevelID,
                        nSubLevelID < 0 ? "NULL" : nSubLevelID.ToString(),
                        strMemberID == null || strMemberID == "null" ? "NULL" : "'" + strMemberID + "'",
                        strOfficePhoneNumber == null || strOfficePhoneNumber == "null" ? "NULL" : "'" + strOfficePhoneNumber + "'",
                        strPhoneNumber == null || strPhoneNumber == "null" ? "NULL" : "'" + strPhoneNumber + "'");

                    m_stackReverseSQL.Push(strSQL);
                }
            }

        }

        #endregion Member Information

        private TeamTreeView m_tree = null;
        private ArrayList m_arrHeaderPosition = null;
        private ArrayList m_arrImportData = null;

        /// <summary>
        /// Self Team , Parent Team
        /// </summary>
        private Dictionary<TreeNode, TreeNode> m_DicNewTeams = null;
        /// <summary>
        /// Self, New Info
        /// </summary>
        private Dictionary<CompanyMember, IMemberInfo> m_DicNewMembers = null;
        /// <summary>
        /// Self, Before After Info
        /// </summary>
        private Dictionary<CompanyMember, IMemberInfo> m_DicChangeMembers = null;
        /// <summary>
        /// Self, Delete Info
        /// </summary>
        private Dictionary<CompanyMember, IMemberInfo> m_DicDeleteMembers = null;

        #region Column Index 

        private int m_nMemberID = -1;
        private int m_nName = -1;
        private int m_nTeamPath = -1;
        private int m_nDispatch = -1;
        private int m_nDispatchName = -1;
        private int m_nSubPosition = -1;
        private int m_nPositionGroup = -1;
        private int m_nLevel = -1;
        private int m_nPhoneNumber = -1;
        private int m_nOfficeNumber = -1;

        #endregion Column Index


        public CommandImportRegularMemberInfo(TeamTreeView tree, ArrayList arrHeaderPosition, ArrayList arrImportData)
        {
            m_DicNewTeams = new Dictionary<TreeNode, TreeNode>();
            m_DicNewMembers = new Dictionary<CompanyMember, IMemberInfo>();
            m_DicChangeMembers = new Dictionary<CompanyMember, IMemberInfo>();
            m_DicDeleteMembers = new Dictionary<CompanyMember, IMemberInfo>();

            m_tree = tree;
            m_arrHeaderPosition = arrHeaderPosition;
            m_arrImportData = arrImportData;
        }


        public override void Do()
        {
            // Team 생성
            int nIndex = -1;

            foreach (KeyValuePair<TreeNode, TreeNode> item in m_DicNewTeams)
            {
                nIndex = item.Value.Nodes.Count;

                if (item.Value.Nodes.Contains(item.Key) == false)
                {
                    item.Value.Nodes.Add(item.Key);
                }

                if (item.Key.Tag == null)
                {
                    item.Key.Tag = RegistRegularTeam(item.Key.Text, (item.Value.Tag as RegularTeam));
                }
            }

            m_tree.ExpandAll();


            // Member 생성
            foreach (KeyValuePair<CompanyMember, IMemberInfo> item in m_DicNewMembers)
            {
                item.Value.Do(item.Key);
            }

            // Member 수정
            foreach (KeyValuePair<CompanyMember, IMemberInfo> item in m_DicChangeMembers)
            {
                item.Value.Do(item.Key);
            }

            // Member 삭제
            foreach (KeyValuePair<CompanyMember, IMemberInfo> item in m_DicDeleteMembers)
            {
                item.Value.Do(item.Key);
            }

            RefreshMemberView();
        }

        public override void RollBack()
        {
            // Member 복원
            foreach (KeyValuePair<CompanyMember, IMemberInfo> item in m_DicDeleteMembers)
            {
                item.Value.RollBack(item.Key);
            }

            // Member 수정
            foreach (KeyValuePair<CompanyMember, IMemberInfo> item in m_DicChangeMembers)
            {
                item.Value.RollBack(item.Key);
            }

            // Member 삭제
            foreach (KeyValuePair<CompanyMember, IMemberInfo> item in m_DicNewMembers)
            {
                item.Value.RollBack(item.Key);
            }
            
            // Team 삭제
            List<TreeNode> listNewTeams = m_DicNewTeams.Keys.ToList<TreeNode>();

            for (int index = listNewTeams.Count - 1; index > -1; index--)
            {
                TreeNodeCollection nodes = listNewTeams[index].Parent.Nodes;
                nodes.Remove(listNewTeams[index]);
            }

            RefreshMemberView();
        }

        public override void SaveDB(DBUtility.WebDBManager dbMgr, bool dir)
        {
            if (dir)
                Import(dbMgr);
            else
                UnImport(dbMgr);

            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.COMPANY_MEMBER);
        }


        private bool Import(DBUtility.WebDBManager dbMgr)
        {
            // 저장 성공 여부
            bool rtnSave = true;

            dbMgr.BeginBatch();

            StringBuilder sb = new StringBuilder();

            string strIFNull = dbMgr.DatabaseType == DBUtility.WebDBManager.DBType.sqlserver ? "ISNULL" : "IFNULL";

            // Team 추가
            foreach (RegularTeam team in from teams in m_DicNewTeams.Keys.ToArray<TreeNode>()
                                         select teams.Tag as RegularTeam
                                        )
            {
                if (CheckNAddTeam(dbMgr, team.TeamID) == false)
                    continue;

                int nParentTeamID = team.ParentTeam.TeamID;
                if (nParentTeamID < 0)
                {
                    rtnSave = false;
                    break;
                }

                string strSQL = "SELECT (" + strIFNull + "(MAX(ID), 0) + 1) AS MAX_ID FROM RegularTeam";
                ArrayList arrResult = dbMgr.GetBatchData(strSQL);
                if (arrResult == null)
                {
                    rtnSave = false;
                    break;
                }

                team.TeamID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0);

                strSQL = String.Format("INSERT INTO RegularTeam (ID, TeamName, ParentTeamID) VALUES ({0}, '{1}', {2})",
                    team.TeamID,
                    team.TeamName,
                    nParentTeamID);

                sb.AppendLine(strSQL);

                if (dbMgr.GetBatchData(strSQL) == null)
                {
                    rtnSave = false;
                    break;
                }

            }

            // Member 추가
            foreach (KeyValuePair<CompanyMember, IMemberInfo> item in m_DicNewMembers)
            {
                InsertMemberInfo info = item.Value as InsertMemberInfo;

                if (rtnSave == false)
                    break;

                if (item.Key.ID > -1)
                    continue;

                if (info.RegulaTeam.TeamID < 0)
                {
                    rtnSave = false;
                    break;
                }

                string strSQL = info.GetInsertCompanyMemberSQL(dbMgr, item.Key);
                if (String.IsNullOrWhiteSpace(strSQL))
                {
                    rtnSave = false;
                    break;
                }

                ArrayList arrResult = dbMgr.GetBatchData(strSQL);

                if (arrResult == null)
                {
                    rtnSave = false;
                    break;
                }

                strSQL = info.GetInsertRegularMemberSQL(dbMgr, item.Key);
                if (String.IsNullOrWhiteSpace(strSQL))
                {
                    rtnSave = false;
                    break;
                }

                if (dbMgr.GetBatchData(strSQL) == null)
                {
                    rtnSave = false;
                    break;
                }

            }


            // Member 수정
            foreach (KeyValuePair<CompanyMember, IMemberInfo> item in m_DicChangeMembers)
            {
                UpdateMemberInfo info = item.Value as UpdateMemberInfo;

                if (rtnSave == false)
                    break;

                foreach (string strUpdateSQL in info.GetUpdateSQL(dbMgr, item.Key))
                {
                    if (dbMgr.GetBatchData(strUpdateSQL) == null)
                    {
                        rtnSave = false;
                        break;
                    }
                }

            }

            // Member 삭제
            foreach (KeyValuePair<CompanyMember, IMemberInfo> item in m_DicDeleteMembers)
            {
                DeleteMemberInfo info = item.Value as DeleteMemberInfo;

                if (rtnSave == false)
                    break;

                foreach (string strDeleteSQL in info.GetDeleteCompanyMemberSQL(dbMgr, item.Key))
                {
                    if (dbMgr.GetBatchData(strDeleteSQL) == null)
                    {
                        rtnSave = false;
                        break;
                    }
                }

            }


            if (rtnSave == false)
            {
                dbMgr.BatchRollback();
            }
            else
            {
                dbMgr.BatchCommit();


                foreach (RegularTeam team in from teams in m_DicNewTeams.Keys.ToArray<TreeNode>()
                                             select teams.Tag as RegularTeam
                                        )
                {
                    DataManager.SetRegularTeam(team.TeamID, team);
                }

            }


            return rtnSave;
        }

        private bool UnImport(DBUtility.WebDBManager dbMgr)
        {
            // 저장 성공 여부
            bool rtnSave = true;
            
            dbMgr.BeginBatch();

            // Member 복원
            foreach (KeyValuePair<CompanyMember, IMemberInfo> item in m_DicDeleteMembers)
            {
                if (rtnSave == false)
                    break;

                foreach (string strReverseSQL in (item.Value as DeleteMemberInfo).GetReverseSQL(dbMgr, item.Key))
                {
                    if (dbMgr.GetBatchData(strReverseSQL) == null)
                    {
                        rtnSave = false;
                        break;
                    }

                    item.Key.ID = (item.Value as DeleteMemberInfo).ID;

                }
            }

            // Member 수정
            foreach (KeyValuePair<CompanyMember, IMemberInfo> item in m_DicChangeMembers)
            {
                if (rtnSave == false)
                    break;

                foreach (string strUpdateSQL in (item.Value as UpdateMemberInfo).GetUpdateSQL(dbMgr, item.Key))
                {
                    if (dbMgr.GetBatchData(strUpdateSQL) == null)
                    {
                        rtnSave = false;
                        break;
                    }
                }

            }

            // Member 삭제
            foreach (KeyValuePair<CompanyMember, IMemberInfo> item in m_DicNewMembers)
            {
                if (rtnSave == false)
                    break;

                foreach (string strDeleteSQL in (item.Value as InsertMemberInfo).GetDeleteSQL(dbMgr, item.Key))
                {
                    if (dbMgr.GetBatchData(strDeleteSQL) == null)
                    {
                        rtnSave = false;
                        break;
                    }

                    item.Key.ID = -1;
                }

            }


            if (rtnSave == true)
            {
                // Team 삭제
                List<TreeNode> listNewTeams = m_DicNewTeams.Keys.ToList<TreeNode>();
                for (int index = listNewTeams.Count - 1; index > -1; index--)
                {
                    string strSQL = String.Format("DELETE FROM RegularTeam WHERE ID = {0} ", (listNewTeams[index].Tag as RegularTeam).TeamID);

                    if (dbMgr.GetBatchData(strSQL) == null)
                    {
                        rtnSave = false;
                        break;
                    }

                }
            }


            if (rtnSave == true)
            {
                dbMgr.BatchCommit();
            }
            else
            {
                dbMgr.BatchRollback();
            }

            return rtnSave;
        }


        #region File Read

        public void ReadImportData()
        {
            int nContentsCount = m_arrHeaderPosition.Count;

            int nIndex = 0;

            #region cvs 파일의 열 순서에 따른 데이터 위치 지정'

            foreach (string strHeaderName in from Headers in m_arrHeaderPosition.ToArray()
                                             select Headers)
            {
                switch (strHeaderName)
                {
                    case "사번":
                        m_nMemberID = nIndex;
                        break;
                    case "성명":
                        m_nName = nIndex;
                        break;
                    case "소속명":
                        m_nTeamPath = nIndex;
                        break;
                    case "파견지소속":
                        m_nDispatch = nIndex;
                        break;
                    case "파견지소속명":
                        m_nDispatchName = nIndex;
                        break;
                    case "직위명":
                        m_nSubPosition = nIndex;
                        break;
                    case "직군명":
                        m_nPositionGroup = nIndex;
                        break;
                    case "직급명":
                        m_nLevel = nIndex;
                        break;
                    case "휴대전화":
                        m_nPhoneNumber = nIndex;
                        break;
                    case "근무지전화":
                        m_nOfficeNumber = nIndex;
                        break;
                }

                nIndex++;
            }

            #endregion cvs 파일의 열 순서에 따른 데이터 위치 지정'

            #region 신규/수정 데이터 적용

            for (int nCnt = 0; nCnt < m_arrImportData.Count; nCnt += nContentsCount)
            {
                /*
                 * 멤버 생성에 필요한 데이터 추출
                */

                RegularTeam team = null;

                string strMemberID = String.Empty;
                string strMemberName = String.Empty;
                string strPositionName = String.Empty;
                string strSubPositionName = String.Empty;
                int nLevelID = 0;
                string strSubLevelName = String.Empty;
                string strPhoneNumber = String.Empty;
                string strPositionGroupName = String.Empty;
                string strOfficeNumber = String.Empty;

                if (m_nMemberID > -1)
                {
                    strMemberID = m_arrImportData[nCnt + m_nMemberID].ToString();
                }
                if (m_nName > -1)
                {
                    strMemberName = m_arrImportData[nCnt + m_nName].ToString();
                }
                if (m_nTeamPath > -1)
                {
                    // 직원이 소속된 팀 가져옴 (미생성된 팀일 경우 생성로직 실행)
                    team = FindTeam(m_arrImportData[nCnt + m_nTeamPath].ToString());
                }
                if(m_nSubPosition > -1)
                {
                    strSubPositionName = m_arrImportData[nCnt + m_nSubPosition].ToString();
                    
                    if (strSubPositionName.IndexOf("본부장") > -1)
                    {
                        strPositionName = "본부장";
                    }
                    else if (strSubPositionName.IndexOf("처장") > -1)
                    {
                        strPositionName = "처장";
                    }
                    else if (strSubPositionName.IndexOf("실장") > -1)
                    {
                        strPositionName = "실장";
                    }
                    else if (strSubPositionName.IndexOf("팀장") > -1)
                    {
                        strPositionName = "팀장";
                    }
                    else if (strSubPositionName.IndexOf("파트장") > -1)
                    {
                        strPositionName = "파트장";
                    }
                    else
                    {
                        strPositionName = "팀원";
                    }
                    

                    //switch (nLevelID)
                    //{
                    //    case 0:
                    //        strPositionName = "알 수 없음";
                    //        break;
                    //    case 1:
                    //    case 2:
                    //        strPositionName = "팀장";
                    //        break;
                    //    case 3:
                    //        strPositionName = "파트장";
                    //        break;
                    //    default:
                    //        strPositionName = "팀원";
                    //        break;
                    //}

                }
                if (m_nPositionGroup > -1)
                {
                    strPositionGroupName = m_arrImportData[nCnt + m_nPositionGroup].ToString();
                }
                if(m_nLevel > -1)
                {
                    if (m_arrImportData[nCnt + m_nLevel].ToString().IndexOf("직급") > -1)
                    {
                        nLevelID = Convert.ToInt32(m_arrImportData[nCnt + m_nLevel].ToString().Substring(0, 1));
                        strSubLevelName = m_arrImportData[nCnt + m_nLevel].ToString().Substring(3).Replace("(", "").Replace(")", "");
                    }

                    //switch (nLevelID)
                    //{
                    //    case 0:
                    //        strPositionName = "알 수 없음";
                    //        break;
                    //    case 1:
                    //    case 2:
                    //        strPositionName = "팀장";
                    //        break;
                    //    case 3:
                    //        strPositionName = "파트장";
                    //        break;
                    //    default:
                    //        strPositionName = "팀원";
                    //        break;
                    //}
                }
                if(m_nPhoneNumber > -1)
                {
                    strPhoneNumber = m_arrImportData[nCnt + m_nPhoneNumber].ToString();
                }
                if (m_nOfficeNumber > -1)
                {
                    strOfficeNumber = m_arrImportData[nCnt + m_nOfficeNumber].ToString();
                }

                ApplyRegularMemberInfo(
                    strMemberID,
                    strMemberName,
                    strPositionName,
                    strSubPositionName,
                    strPositionGroupName,
                    nLevelID,
                    strSubLevelName,
                    strPhoneNumber,
                    strOfficeNumber,
                    team);

            }

            #endregion 신규/수정 데이터 적용

            #region 데이터 삭제

            foreach (List<CompanyMember> usedMembers in DataManager.GetAllRegularMembers())
            {
                foreach (CompanyMember usedMember in usedMembers)
                {
                    if (IsContainTeam(m_tree.TopNode, DataManager.GetRegularTeamByCompanyMember(usedMember)) == false)
                        continue;

                    if (m_DicNewMembers.Keys.Contains(usedMember) == false &&
                        m_DicChangeMembers.Keys.Contains(usedMember) == false)
                    {
                        RemoveRegularMemberInfo(usedMember);
                    }

                }
            }

            // 삭제 적용은 모든 삭제데이터를 다 추출한 다음에 함. (검색하는 List의 배열순서가 깨짐)

            foreach(KeyValuePair<CompanyMember, IMemberInfo> item in m_DicDeleteMembers)
            {
                item.Value.Do(item.Key);
            }
            
            #endregion 데이터 삭제

            RefreshMemberView();
        }

        private void RefreshMemberView()
        {
            FormMain.Instance.RefreshRegularMemberGrid();
        }

        #endregion File Read


        #region Team

        private RegularTeam RegistRegularTeam(string strDeptName, RegularTeam parentTeam)
        {
            RegularTeam team = new RegularTeam();
            team.TeamName = strDeptName;
            DataManager.SetRegularMembers(team);
            team.ParentTeam = parentTeam;

            return team;
        }

        private RegularTeam FindTeam(string strTeamFullPath)
        {
            RegularTeam rtnTeam = (FindTeam(m_tree.TopNode, strTeamFullPath).Tag as RegularTeam);

            return rtnTeam;
        }

        // 재귀
        private TreeNode FindTeam(TreeNode node, string strTeamPath)
        {
            TreeNode rtnNode = null;
            string strDeptName = strTeamPath.Split('/')[0];

            if (node.Text == strDeptName && node.Parent == null)
            {
                rtnNode = node;
            }
            else
            {
                foreach (TreeNode childNode in node.Nodes)
                {
                    if (childNode.Text == strDeptName)
                    {
                        rtnNode = childNode;
                        break;
                    }
                }
            }

            if (rtnNode == null)
            {
                int nIndex = node.Nodes.Count;
                rtnNode = node.Nodes.Insert(nIndex, strDeptName);
                rtnNode.Tag = RegistRegularTeam(strDeptName, (node.Tag as RegularTeam));

                m_DicNewTeams.Add(rtnNode, node);

                node.Expand();
            }

            if (strTeamPath.Split('/').Length > 1)
            {
                return FindTeam(rtnNode, strTeamPath.Substring(strDeptName.Length + 1));
            }

            return rtnNode;
        }

        private bool IsContainTeam(TreeNode node, RegularTeam findTeam)
        {
            if (object.Equals(node.Tag, findTeam) == true)
                return true;

            foreach (TreeNode childNode in node.Nodes)
            {
                if (IsContainTeam(childNode, findTeam) == true)
                    return true;
            }

            return false;
        }

        private bool CheckNAddTeam(DBUtility.WebDBManager dbMgr, int nTeamID)
        {
            bool rtnHasNewTeam = false;

            string strSQL = "SELECT ID FROM RegularTeam WHERE ID = " + nTeamID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult != null)
            {
                if (arrResult.Count == 0)
                {
                    rtnHasNewTeam = true;
                }
            }

            return rtnHasNewTeam;
        }

        #endregion Team
        

        #region Member

        private void ApplyRegularMemberInfo(string strMemberID, string strMemberName, string strPositionName, string strSubPositionName, string strPositionGroupName, int nLevelID, string strSubLevelName, string strPhoneNumber, string strOfficeNumber, RegularTeam team)
        {
            CompanyMember member = DataManager.GetCompanyMemberByMemberID(strMemberID, m_tree);

            if (member == null)
            {
                RegistRegularMemberInfo(
                    strMemberID,
                    strMemberName,
                    strPositionName,
                    strSubPositionName,
                    strPositionGroupName,
                    nLevelID,
                    strSubLevelName,
                    strPhoneNumber,
                    strOfficeNumber,
                    team);
            }
            else
            {
                UpdateRegularMemberInfo(
                    member,
                    strMemberID,
                    strMemberName,
                    strPositionName,
                    strSubPositionName,
                    strPositionGroupName,
                    nLevelID,
                    strSubLevelName,
                    strPhoneNumber,
                    strOfficeNumber,
                    team);
            }
        }

        /// <summary>
        /// 신규 데이터인 경우
        /// </summary>
        private void RegistRegularMemberInfo(string strMemberID, string strMemberName, string strPositionName, string strSubPositionName, string strPositionGroupName, int nLevelID, string strSubLevelName, string strPhoneNumber, string strOfficeNumber, RegularTeam team)
        {
            InsertMemberInfo newMemberInfo = new InsertMemberInfo();

            CompanyMember member = new CompanyMember();

            List<CompanyMember> members = DataManager.GetRegularMembers(team);
            members.Add(member);

            TeamGrid.MemberID memberID = new TeamGrid.MemberID(strMemberID, true);
            DataManager.SetCompanyMemberMemberIDChanged(member, memberID.IsChanged);

            newMemberInfo.RegulaTeam = team;

            newMemberInfo.MemberID = strMemberID;
            newMemberInfo.Name = strMemberName;
            newMemberInfo.LevelID = nLevelID;
            newMemberInfo.SubJobPosition = CompanyMember.JobPositionSubInfo.GetSubPosition(strSubPositionName);
            if (newMemberInfo.SubJobPosition == null)
            {
                newMemberInfo.SubJobPosition = new CompanyMember.JobPositionSubInfo();
                newMemberInfo.SubJobPosition.Name = strSubPositionName;
            }

            int nPositionID = -1;
            if (DataManager.GetJobPositionID(strPositionName, out nPositionID))
            {
                newMemberInfo.PositionID = nPositionID;
            }

            newMemberInfo.SubJobLevel = CompanyMember.JobLevelSubInfo.GetJobSubLevel(strSubLevelName);
            if (newMemberInfo.SubJobLevel == null)
            {
                newMemberInfo.SubJobLevel = new CompanyMember.JobLevelSubInfo();
                newMemberInfo.SubJobLevel.Name = strSubLevelName;
            }

            newMemberInfo.GroupPosition = CompanyMember.JobGroupPosition.GetJobGroupPosition(strPositionGroupName);
            if (newMemberInfo.GroupPosition == null)
            {
                newMemberInfo.GroupPosition = new CompanyMember.JobGroupPosition();
                newMemberInfo.GroupPosition.Name = strPositionGroupName;
            }

            TeamGrid.PhoneNumber phoneNumber = new TeamGrid.PhoneNumber(strPhoneNumber, true);
            newMemberInfo.PhoneNumber = phoneNumber.ToString();

            TeamGrid.OfficePhoneNumber officePhoneNumber = new TeamGrid.OfficePhoneNumber(strOfficeNumber, true);
            newMemberInfo.OfficePhoneNumber = officePhoneNumber.ToString();             

            newMemberInfo.Do(member);

            m_DicNewMembers.Add(member, newMemberInfo);
        }

        /// <summary>
        /// 기존 데이터인 경우
        /// </summary>
        private void UpdateRegularMemberInfo(CompanyMember member, string strMemberID, string strMemberName, string strPositionName, string strSubPositionName, string strPositionGroupName, int nLevelID, string strSubLevelName, string strPhoneNumber, string strOfficeNumber, RegularTeam team)
        {
            // 원본 데이터 저장
            UpdateMemberInfo updateMemberInfo = new UpdateMemberInfo();
            updateMemberInfo.Old_RegulaTeam = DataManager.GetRegularTeamByCompanyMember(member);
            updateMemberInfo.Old_Name = member.Name;
            updateMemberInfo.Old_LevelID = member.LevelID;
            updateMemberInfo.Old_PositionID = member.PositionID;
            updateMemberInfo.Old_OfficePhoneNumber = member.OfficePhoneNumber;
            updateMemberInfo.Old_PhoneNumber = member.PhoneNumber;
            updateMemberInfo.Old_SubJobLevel = (member.SubJobLevel == null ? null : CompanyMember.JobLevelSubInfo.GetJobSubLevel(member.SubJobLevel.Name));
            updateMemberInfo.Old_GroupPosition = (member.GroupPosition == null ? null : CompanyMember.JobGroupPosition.GetJobGroupPosition(member.GroupPosition.Name));
            updateMemberInfo.Old_SubJobPosition = (member.SubJobPosition == null ? null : CompanyMember.JobPositionSubInfo.GetSubPosition(member.SubJobPosition.Name));

            // 신규 데이터 저장
            updateMemberInfo.New_RegulaTeam = team;
            updateMemberInfo.New_Name = strMemberName;
            updateMemberInfo.New_LevelID = nLevelID;
            int nPositionID = -1;
            if (DataManager.GetJobPositionID(strPositionName, out nPositionID))
            {
                updateMemberInfo.New_PositionID = nPositionID;
            }

            TeamGrid.OfficePhoneNumber officePhoneNumber = new TeamGrid.OfficePhoneNumber(strOfficeNumber, true);
            updateMemberInfo.New_OfficePhoneNumber = officePhoneNumber.ToString();

            TeamGrid.PhoneNumber phoneNumber = new TeamGrid.PhoneNumber(strPhoneNumber, true);
            updateMemberInfo.New_PhoneNumber = phoneNumber.ToString();

            updateMemberInfo.New_SubJobPosition = CompanyMember.JobPositionSubInfo.GetSubPosition(strSubPositionName);
            if (updateMemberInfo.New_SubJobPosition == null)
            {
                updateMemberInfo.New_SubJobPosition = new CompanyMember.JobPositionSubInfo();
                updateMemberInfo.New_SubJobPosition.Name = strSubPositionName;
            }

            updateMemberInfo.New_SubJobLevel = CompanyMember.JobLevelSubInfo.GetJobSubLevel(strSubLevelName);
            if (updateMemberInfo.New_SubJobLevel == null)
            {
                updateMemberInfo.New_SubJobLevel = new CompanyMember.JobLevelSubInfo();
                updateMemberInfo.New_SubJobLevel.Name = strSubLevelName;
            }

            updateMemberInfo.New_GroupPosition = CompanyMember.JobGroupPosition.GetJobGroupPosition(strPositionGroupName);
            if (updateMemberInfo.New_GroupPosition == null)
            {
                updateMemberInfo.New_GroupPosition = new CompanyMember.JobGroupPosition();
                updateMemberInfo.New_GroupPosition.Name = strPositionGroupName;
            }


            // 데이터 적용
            updateMemberInfo.Do(member);

            m_DicChangeMembers.Add(member, updateMemberInfo);
        }

        /// <summary>
        /// 일치하는 데이터가 없는 경우(삭제처리)
        /// </summary>
        private void RemoveRegularMemberInfo(CompanyMember member)
        {
            // 신규와 수정 데이터작업이 완료된 후 최종 결과데이터와 남은 데이터를 비교하여
            // 사용되지 않은 멤버에 대해서 삭제하여준다.

            DeleteMemberInfo removeMemberInfo = new DeleteMemberInfo();

            removeMemberInfo.RegulaTeam = DataManager.GetRegularTeamByCompanyMember(member);

            m_DicDeleteMembers.Add(member, removeMemberInfo);
        }

        #endregion Member

    }


}
