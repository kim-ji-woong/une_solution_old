
namespace EventRegistry
{
	public delegate void SimpleClientEvent(object sender, MyClientEventArgs args);

	public class MyEventPublisher
	{
		private string _publisherName;

		public event SimpleClientEvent MyClientEvent1;

		public MyEventPublisher()
		{
		}

		public string PublisherName
		{
			get { return _publisherName; }
			set { _publisherName = value; }
		}

		public void ClientMethodThatTriggersEvent1()
		{
			if (MyClientEvent1 != null)
			{
				MyClientEvent1(this, new MyClientEventArgs("Event 1 raised from " + _publisherName));
			}
		}
	}
}