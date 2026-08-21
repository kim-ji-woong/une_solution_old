namespace BroadcastSimulator
{
    public class ClientProvider : TcpLib2.ClientServiceProvider
    {
        public override void OnReceiveData()
        {
        }

        public override void OnDropConnection()
        {
        }
    }
}
