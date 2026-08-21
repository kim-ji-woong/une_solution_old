namespace SOPManager.DAL
{
    using Model.Sop.Account;
    using Model.Sop.Category;
    using Model.Sop.Component;
    using Model.Sop.Config;
    using IDAL;
    using dnsDBUtil;
    using System.Collections;

    public class DeleteManager : QueryManager, IDelete
    {
        private string m_strErrorMessage = null;
        private DataManager m_dataManager = null;
        //private WebDBManager m_dbManager = null;

        public DeleteManager(DataManager dataManager)
        {
            m_dataManager = dataManager;
            m_dbManager = m_dataManager.GetDBManager() as WebDBManager;
        }

        public bool DeleteLevel(int id)
        {
            string tableName = Level.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteLevel(string strCondition)
        {
            string tableName = Level.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteUser(int id)
        {
            string tableName = User.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }


        // strCondition : where를 제외한 조건문
        public bool DeleteUser(string strCondition)
        {
            string tableName = User.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteOption(int id)
        {
            string tableName = Option.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }


        // strCondition : where를 제외한 조건문
        public bool DeleteOption(string strCondition)
        {
            string tableName = Option.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteActionStep(int id)
        {
            string tableName = ActionStep.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteActionStep(string strCondition)
        {
            string tableName = ActionStep.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteAnnotation(int id)
        {
            string tableName = Annotation.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteAnnotation(string strCondition)
        {
            string tableName = Annotation.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteArrow(int id)
        {
            string tableName = Arrow.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteArrow(string strCondition)
        {
            string tableName = Arrow.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteDecision(int id)
        {
            string tableName = Decision.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteDecision(string strCondition)
        {
            string tableName = Decision.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteDisaster(int id)
        {
            string tableName = Disaster.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteDisaster(string strCondition)
        {
            string tableName = Disaster.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteDisasterType(int id)
        {
            string tableName = DisasterType.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteDisasterType(string strCondition)
        {
            string tableName = DisasterType.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteDisasterCategory(int id)
        {
            string tableName = DisasterCategory.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteDisasterCategory(string strCondition)
        {
            string tableName = DisasterCategory.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteEndPoint(int id)
        {
            string tableName = EndPoint.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteEndPoint(string strCondition)
        {
            string tableName = EndPoint.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteExternalProgram(int id)
        {
            string tableName = ExternalProgram.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteExternalProgram(string strCondition)
        {
            string tableName = ExternalProgram.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteExternalProgramParameter(int id)
        {
            string tableName = ExternalProgramParameter.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteExternalProgramParameter(string strCondition)
        {
            string tableName = ExternalProgramParameter.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteGrid(int id)
        {
            string tableName = SectionGrid.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteGrid(string strCondition)
        {
            string tableName = SectionGrid.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteGridColumn(int id)
        {
            string tableName = SectionGridColumn.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where GridID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteGridColumn(string strCondition)
        {
            string tableName = SectionGridColumn.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteGridRow(int id)
        {
            string tableName = SectionGridRow.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where GridID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteGridRow(string strCondition)
        {
            string tableName = SectionGridRow.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteInternalTransmission(int id)
        {
            string tableName = InternalTransmission.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteInternalTransmission(string strCondition)
        {
            string tableName = InternalTransmission.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteLink(int id)
        {
            string tableName = Link.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteLink(string strCondition)
        {
            string tableName = Link.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteProcess(int id)
        {
            string tableName = Process.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteProcess(string strCondition)
        {
            string tableName = Process.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteProcessMission(int id)
        {
            string tableName = ProcessMission.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteProcessMission(string strCondition)
        {
            string tableName = ProcessMission.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteProcessExternalMission(int nProcessID, int nOrderIndex, int nProgramID, int nParameterIndex)
        {
            string tableName = ProcessExternalMission.TableName;
            string query = "";
            ArrayList res = null;

            bool isNullable;

            query = string.Format("delete from {0} where {1} = {2} and {3} = {4} and {5} = {6} and {7} = {8}", tableName,
                ProcessExternalMission.GetFieldName(ProcessExternalMission.Fields.ProcessID, out isNullable),
                nProcessID,
                ProcessExternalMission.GetFieldName(ProcessExternalMission.Fields.OrderIndex, out isNullable),
                nOrderIndex,
                ProcessExternalMission.GetFieldName(ProcessExternalMission.Fields.ProgramID, out isNullable),
                nProgramID,
                ProcessExternalMission.GetFieldName(ProcessExternalMission.Fields.ParameterIndex, out isNullable),
                nParameterIndex);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteProcessExternalMission(string strCondition)
        {
            string tableName = ProcessExternalMission.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteStepMember(int id)
        {
            string tableName = StepMember.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteStepMember(string strCondition)
        {
            string tableName = StepMember.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteSubDisasterCategory(int id)
        {
            string tableName = SubDisasterCategory.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteSubDisasterCategory(string strCondition)
        {
            string tableName = SubDisasterCategory.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteVersion(int id)
        {
            string tableName = Version.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteVersion(string strCondition)
        {
            string tableName = Version.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteSpecialMessage(int id)
        {
            string tableName = SpecialMessage.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteSpecialMessage(string strCondition)
        {
            string tableName = SpecialMessage.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteSession(int id)
        {
            string tableName = Session.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteSession(string strCondition)
        {
            string tableName = Session.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public bool DeleteLinkedSop(int id)
        {
            string tableName = LinkedSop.TableName;
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }


        // strCondition : where를 제외한 조건문
        public bool DeleteLinkedSop(string strCondition)
        {
            string tableName = LinkedSop.TableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        // strCondition : where를 제외한 조건문
        public bool DeleteTable(string strTableName, string strCondition)
        {
            string tableName = strTableName;
            string query = "";
            ArrayList res = null;

            if (strCondition == null || strCondition.Length == 0)
                query = string.Format("delete from {0}", tableName);
            else
                query = string.Format("delete from {0} where {1}", tableName, strCondition);

            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                return true;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }
        }

        public string GetErrorMessage()
        {
            return m_strErrorMessage;
        }
    }
}
