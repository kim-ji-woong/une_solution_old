namespace UnEService_Core.Interface
{
    public interface IWebDB
    {
        string[] RunQuery(string dbName, string dbType, string query);
        long BeginBatch(string dbName, string dbType, out string errorMessage);
        string BatchCommit(long transactionKey);
        string BatchRollback(long transactionKey);
        string[] BatchQuery(string query, long transactionKey);
    }
}
