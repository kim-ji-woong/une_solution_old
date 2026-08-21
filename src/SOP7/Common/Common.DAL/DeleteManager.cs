namespace Common.DAL
{
    using Model;
    using Model.Option;
    using IDAL;
    using dnsDBUtil;
    using System.Collections;
    using Common.Model.History;

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

        // Option
        public bool DeleteOption(Options.OptionTarget eTargetName, int id)
        {
            string tableName = string.Format("Option{0}", eTargetName.ToString());
            string query = "";
            ArrayList res = null;

            if (eTargetName != Options.OptionTarget.NOT_DEFINED)
            {
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
            else
            {
                // Not Defined
                m_strErrorMessage = "TargetName Not Defined";
                return false;
            }
        }

        public bool DeleteOption(Options.OptionTarget eTargetName, string strPropertyName)
        {
            string tableName = string.Format("Option{0}", eTargetName.ToString());
            string query = "";
            ArrayList res = null;

            query = string.Format("delete from {0} where PropertyName = '{1}'", tableName, strPropertyName);
            res = m_dbManager.GetResultData(query);

            if (eTargetName != Options.OptionTarget.NOT_DEFINED)
            {
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
            else
            {
                // Not Defined
                m_strErrorMessage = "TargetName Not Defined";
                return false;
            }
        }

        // History
        public bool DeleteActionStepHistory(int id)
        {
            string tableName = ActionStepHistory.TableName;
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

        public bool DeleteActionStepHistory(string strCondition)
        {
            string tableName = ActionStepHistory.TableName;
            string query = "";
            ArrayList res = null;

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

        public bool DeleteComponentHistory(int id)
        {
            string tableName = ComponentHistory.TableName;
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

        public bool DeleteComponentHistory(string strCondition)
        {
            string tableName = ComponentHistory.TableName;
            string query = "";
            ArrayList res = null;

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

        public bool DeleteComponentHistoryDetail(int id)
        {
            string tableName = ComponentHistoryDetail.TableName;
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

        public bool DeleteComponentHistoryDetail(string strCondition)
        {
            string tableName = ComponentHistoryDetail.TableName;
            string query = "";
            ArrayList res = null;

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

        public bool DeleteActionStepAutoClose(int id)
        {
            string tableName = ActionStepAutoClose.TableName;
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

        public bool DeleteActionStepAutoClose(string strCondition)
        {
            string tableName = ActionStepAutoClose.TableName;
            string query = "";
            ArrayList res = null;

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

        public bool DeleteShelter(int id)
        {
            string tableName = Shelter.TableName;
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

        public bool DeleteShelter(string strCondition)
        {
            string tableName = Shelter.TableName;
            string query = "";
            ArrayList res = null;

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

        public bool DeleteSite(int id)
        {
            string tableName = Site.TableName;
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

        public bool DeleteSite(string strCondition)
        {
            string tableName = Site.TableName;
            string query = "";
            ArrayList res = null;

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
