namespace AgentFactory.BLL
{
    public class BaseFactory
    {
        private static Factory m_factory = null;

        public static Factory GetFactory()
        {
            if (m_factory == null)
                m_factory = new FactoryEx();

            return m_factory;
        }
    }
}
