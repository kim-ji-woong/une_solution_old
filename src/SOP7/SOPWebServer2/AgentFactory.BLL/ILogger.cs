namespace AgentFactory.BLL
{
    public interface ILogger
    {
        void Write(string strLog);
        ILogger Clone(string strTag);
        void Close();
    }
}
