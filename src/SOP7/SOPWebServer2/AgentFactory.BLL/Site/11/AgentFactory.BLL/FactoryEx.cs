namespace AgentFactory.BLL
{
    public class FactoryEx : Factory
    {
        public override BaseAgent MakeAgent(AgentType type)
        {
            return base.MakeAgent(type);
        }
    }
}
