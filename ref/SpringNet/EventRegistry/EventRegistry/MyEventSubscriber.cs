using System;


namespace EventRegistry
{
	public class MyEventSubscriber
	{
		private bool _eventHandled = false;

		public MyEventSubscriber()
		{
		}

		public void HandleClientEvents(object sender, MyClientEventArgs args)
		{
			Console.WriteLine("HandleClientEvents handler in subscriber handled event with args: " + args.EventMessage);
			_eventHandled = true;
		}

		public bool EventHandled
		{
			get { return _eventHandled; }
		}

		public void NeverCall()
		{
			throw new Exception();
		}

		public string FakeEventHandler(object sender, MyClientEventArgs args)
		{
			throw new Exception();
		}
	}
}